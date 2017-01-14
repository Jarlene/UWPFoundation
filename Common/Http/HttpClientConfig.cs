using System;
using System.Collections.Generic;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Headers;


namespace Common.Http
{
    public class HttpClientConfig
    {
        public HttpClientConfig(XHttpClient xpHttpClient, HttpClient httpClient, ICustomHttpFilter retryHttpFilter, Action applyConfig)
        {
            _client = xpHttpClient;
            DefaultRequestHeader = httpClient.DefaultRequestHeaders;
            CustomHttpFilter = retryHttpFilter;
            ApplyConfig = applyConfig;
        }

        private XHttpClient _client;

        private static HttpStatusCode[] _defaultHttStatuspCodeForRetry = { HttpStatusCode.ServiceUnavailable };

        public Action ApplyConfig { get; set; }

        public string BaseUrl { get; set; } = string.Empty;

        public HttpRequestHeaderCollection DefaultRequestHeader { get; set; }

        public int TimeOut { get; set; } = 30;

        public int RetryTimes { get; set; } = 3;

        public List<HttpStatusCode> HttpStatusCodesForRetry { get; set; } = new List<HttpStatusCode>(_defaultHttStatuspCodeForRetry);

        public ICustomHttpFilter CustomHttpFilter { get; private set; }

        public UnicodeEncoding? ContentEncoding { get; set; } = null;

        public string MediaType { get; set; } = null;

        public HttpClientConfig SetContentEncoding(UnicodeEncoding encoding)
        {
            ContentEncoding = encoding;
            return this;
        }

        public HttpClientConfig SetMediaType(string mediaType)
        {
            MediaType = mediaType;
            return this;
        }

        public HttpClientConfig SetUseHttpCache(bool bUse)
        {
            _client.SetCache(bUse);
            return this;
        }

        public HttpClientConfig SetBaseUrl(string baseUrl)
        {
            BaseUrl = baseUrl;
            return this;
        }

        public HttpClientConfig SetDefaultHeaders(string name, string value)
        {
            DefaultRequestHeader.Append(name, value);
            return this;
        }

        public HttpClientConfig SetDefaultHeaders(params string[] nameValue)
        {
            if (nameValue.Length % 2 != 0)
                throw new ArgumentException();

            for (int i = 0; i < nameValue.Length; i += 2)
            {
                DefaultRequestHeader.Append(nameValue[i], nameValue[i + 1]);
            }
            return this;
        }

        public HttpClientConfig SetDefaultHeaders(string[] names, string[] values)
        {
            if (names.Length != values.Length)
                throw new ArgumentException();

            for (int i = 0; i < names.Length; i++)
            {
                DefaultRequestHeader.Append(names[i], values[i]);
            }
            return this;
        }

        public HttpClientConfig SetAuthorization(string scheme, string authorization)
        {
            DefaultRequestHeader.Authorization = new HttpCredentialsHeaderValue(scheme, authorization);
            return this;
        }

        public HttpClientConfig SetCookeie(string name, string value)
        {
            DefaultRequestHeader.Cookie.Add(new HttpCookiePairHeaderValue(name, value));
            return this;
        }

        public HttpClientConfig SetTimeOut(int timeOutSec)
        {
            TimeOut = timeOutSec;
            return this;
        }

        public HttpClientConfig SetRetryTimes(int retryTimes)
        {
            RetryTimes = retryTimes;
            return this;
        }

        public HttpClientConfig AddRetryStatusCode(HttpStatusCode statusCode)
        {
            HttpStatusCodesForRetry.Add(statusCode);
            return this;
        }

        public HttpClientConfig AppendHttpFilter(ICustomHttpFilter httpFilter)
        {
            var tempFilter = CustomHttpFilter;
            tempFilter.InnerFilter = httpFilter;
            CustomHttpFilter = httpFilter;
            return this;
        }
    }
}
