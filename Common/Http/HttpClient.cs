using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace Common.Http
{
    public class XHttpClient
    {
        public static readonly XHttpClient DefaultClient = new XHttpClient();

        private HttpClient _httpClient;

        private HttpRetryFilter _httpRetryFilter;

        private HttpBaseProtocolFilter _baseFilter = new HttpBaseProtocolFilter();

        public HttpClientConfig HttpConfig { get; private set; }

        public RequestParam RequestParamBuilder
        {
            get
            {
                return new RequestParam();
            }
        }

        public XHttpClient()
        {
            _httpRetryFilter = new HttpRetryFilter();
            _httpClient = new HttpClient(_httpRetryFilter);
            HttpConfig = new HttpClientConfig(this, _httpClient, _httpRetryFilter, ApplyConfig);
            ApplyConfig();
        }

        void ApplyConfig()
        {
            _httpRetryFilter.RetryTimes = HttpConfig.RetryTimes;
            _httpRetryFilter.RetryHttpCodes = HttpConfig.HttpStatusCodesForRetry;
            HttpConfig.CustomHttpFilter.InnerFilter = _baseFilter;
        }

        internal void SetCache(bool useCache)
        {
            if (useCache)
            {
                _baseFilter.CacheControl.ReadBehavior = HttpCacheReadBehavior.Default;
                _baseFilter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.Default;
            }
            else
            {
                _baseFilter.CacheControl.ReadBehavior = HttpCacheReadBehavior.MostRecent;
                _baseFilter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.NoCache;
            }
        }

        string BuildUrl(string functionUrl, RequestParam param)
        {
            var url = (param == null || param.NeedBaseUrl ? HttpConfig.BaseUrl : "") + functionUrl;
            if (param != null)
            {
                foreach (var segment in param.UrlSegments)
                {
                    url = url.Replace("{" + segment.Key + "}", segment.Value.UrlEncoding());
                }

                foreach (var queryString in param.QueryStrings)
                {
                    url = url.AppendQueryString(queryString);
                }
            }

            return url;
        }

        void ConfigRequest(HttpRequestMessage request, RequestParam httpParam)
        {
            if (httpParam == null)
                return;

            httpParam.ApplyToRequester(request, HttpConfig);
        }

        public void GetAsync(string functionUrl, RequestParam httpParam, IResponseHandler responseHandler)
        {
            SendRequestAsync(HttpMethod.Get, functionUrl, httpParam, responseHandler);
        }

        public void PostAsync(string functionUrl, RequestParam httpParam, IResponseHandler responseHandler)
        {
            SendRequestAsync(HttpMethod.Post, functionUrl, httpParam, responseHandler);
        }

        public void PutAsync(string functionUrl, RequestParam httpParam, IResponseHandler responseHandler)
        {
            SendRequestAsync(HttpMethod.Put, functionUrl, httpParam, responseHandler);
        }

        public void DeleteAsync(string functionUrl, RequestParam httpParam, IResponseHandler responseHandler)
        {
            SendRequestAsync(HttpMethod.Delete, functionUrl, httpParam, responseHandler);
        }

        public void PatchAsync(string functionUrl, RequestParam httpParam, IResponseHandler responseHandler)
        {
            SendRequestAsync(HttpMethod.Patch, functionUrl, httpParam, responseHandler);
        }

        public async Task<T> GetAsync<T>(string functionUrl, RequestParam httpParam, Action<HttpProgress> onProgress = null, Action<HttpRequestMessage> onCancel = null)
        {
            return await SendRequestAsync<T>(HttpMethod.Get, functionUrl, httpParam, onProgress, onCancel);
        }

        public async Task<T> PostAsync<T>(string functionUrl, RequestParam httpParam, Action<HttpProgress> onProgress = null, Action<HttpRequestMessage> onCancel = null)
        {
            return await SendRequestAsync<T>(HttpMethod.Post, functionUrl, httpParam, onProgress, onCancel);
        }

        public async Task<T> PutAsync<T>(string functionUrl, RequestParam httpParam, Action<HttpProgress> onProgress = null, Action<HttpRequestMessage> onCancel = null)
        {
            return await SendRequestAsync<T>(HttpMethod.Put, functionUrl, httpParam, onProgress, onCancel);
        }

        public async Task<T> DeleteAsync<T>(string functionUrl, RequestParam httpParam, Action<HttpProgress> onProgress = null, Action<HttpRequestMessage> onCancel = null)
        {
            return await SendRequestAsync<T>(HttpMethod.Delete, functionUrl, httpParam, onProgress, onCancel);
        }

        public async Task<T> PatchAsync<T>(string functionUrl, RequestParam httpParam, Action<HttpProgress> onProgress = null, Action<HttpRequestMessage> onCancel = null)
        {
            return await SendRequestAsync<T>(HttpMethod.Patch, functionUrl, httpParam, onProgress, onCancel);
        }

        public async Task GetAsync(string functionUrl, RequestParam httpParam, Action<HttpProgress> onProgress = null, Action<HttpRequestMessage> onCancel = null)
        {
            await SendRequestAsync(HttpMethod.Get, functionUrl, httpParam, onProgress, onCancel);
        }

        public async Task PostAsync(string functionUrl, RequestParam httpParam, Action<HttpProgress> onProgress = null, Action<HttpRequestMessage> onCancel = null)
        {
            await SendRequestAsync(HttpMethod.Post, functionUrl, httpParam, onProgress, onCancel);
        }

        public async Task PutAsync(string functionUrl, RequestParam httpParam, Action<HttpProgress> onProgress = null, Action<HttpRequestMessage> onCancel = null)
        {
            await SendRequestAsync(HttpMethod.Put, functionUrl, httpParam, onProgress, onCancel);
        }

        public async Task DeleteAsync(string functionUrl, RequestParam httpParam, Action<HttpProgress> onProgress = null, Action<HttpRequestMessage> onCancel = null)
        {
            await SendRequestAsync(HttpMethod.Delete, functionUrl, httpParam, onProgress, onCancel);
        }

        public async Task PatchAsync(string functionUrl, RequestParam httpParam, Action<HttpProgress> onProgress = null, Action<HttpRequestMessage> onCancel = null)
        {
            await SendRequestAsync(HttpMethod.Patch, functionUrl, httpParam, onProgress, onCancel);
        }


        public async void SendRequestAsync(HttpMethod httpMethod, string functionUrl, RequestParam httpParam, IResponseHandler responseHandler)
        {
            if (responseHandler == null)
                responseHandler = new ResponseHandler();

            HttpRequestMessage request = BuildHttpRequest(httpMethod, functionUrl, httpParam);
            IProgress<HttpProgress> progress = BuildHttpProgress(responseHandler.OnProgress);
            CancellationTokenSource cancellationTokenSource = BuildCancelTokenSource();

            HttpResponseMessage response = null;
            try
            {
                response = await _httpClient.SendRequestAsync(request).AsTask(cancellationTokenSource.Token, progress);
                responseHandler.Handle(response);
            }
            catch (TaskCanceledException)
            {
                responseHandler.OnCancel?.Invoke(request);
            }
            catch (Exception ex)
            {
                responseHandler.OnFailed?.Invoke(new HttpResponseMessage() { Content = new HttpStringContent(ex.ToString()) });
            }
        }

        public async Task<T> SendRequestAsync<T>(HttpMethod httpMethod, string functionUrl, RequestParam httpParam, Action<HttpProgress> onProgress, Action<HttpRequestMessage> onCancel)
        {
            HttpRequestMessage request = BuildHttpRequest(httpMethod, functionUrl, httpParam);
            IProgress<HttpProgress> progress = BuildHttpProgress(onProgress);
            CancellationTokenSource cancellationTokenSource = BuildCancelTokenSource();

            try
            {
                return await _httpClient.SendRequestAsync(request).AsTask(cancellationTokenSource.Token, progress).ContinueWith(async responseTask =>
                {
                    var response = responseTask.Result;

                    var content = await response.Content.ReadAsStringAsync();
                    if (content is T)
                        return (T)Convert.ChangeType(content, typeof(T));

                    var serializer = SerializerFactory.GetSerializer(response.Content.Headers.ContentType.MediaType);

                    return serializer == null ? default(T) : serializer.Deserialize<T>(content);
                }).Unwrap();
            }
            catch (TaskCanceledException)
            {
                onCancel?.Invoke(request);
                return default(T);
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public async Task SendRequestAsync(HttpMethod httpMethod, string functionUrl, RequestParam httpParam, Action<HttpProgress> onProgress, Action<HttpRequestMessage> onCancel)
        {
            HttpRequestMessage request = BuildHttpRequest(httpMethod, functionUrl, httpParam);
            IProgress<HttpProgress> progress = BuildHttpProgress(onProgress);
            CancellationTokenSource cancellationTokenSource = BuildCancelTokenSource();

            try
            {
                await _httpClient.SendRequestAsync(request).AsTask(cancellationTokenSource.Token, progress);

            }
            catch (TaskCanceledException)
            {
                onCancel?.Invoke(request);
            }
            catch (Exception)
            {
                throw new Exception("send request error");
            }
        }

        private CancellationTokenSource BuildCancelTokenSource()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            if (HttpConfig.TimeOut != int.MaxValue && HttpConfig.TimeOut > 0)
            {
                cancellationTokenSource.CancelAfter(HttpConfig.TimeOut * 1000);
            }

            return cancellationTokenSource;
        }

        private static IProgress<HttpProgress> BuildHttpProgress(Action<HttpProgress> onProgress)
        {
            return new Progress<HttpProgress>(p => { onProgress?.Invoke(p); });
        }

        private HttpRequestMessage BuildHttpRequest(HttpMethod httpMethod, string functionUrl, RequestParam httpParam)
        {
            HttpRequestMessage request = new HttpRequestMessage(httpMethod, new Uri(BuildUrl(functionUrl, httpParam)));

            ConfigRequest(request, httpParam);
            return request;
        }
    }
}
