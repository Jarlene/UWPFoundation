using System;
using System.Collections.Generic;
using Windows.Data.Json;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace Common.Http
{
    public class RequestParam
    {
        public IHttpContent Body { get; set; }

        public string SchemeAuthorization { get; set; }

        public string Authorization { get; set; }

        public DateTime? IfModifiedSince { get; set; } = null;

        public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>();

        public Dictionary<string, string> QueryStrings { get; } = new Dictionary<string, string>();

        public Dictionary<string, string> UrlSegments { get; } = new Dictionary<string, string>();

        public UnicodeEncoding? ContentEncoding { get; set; } = null;

        public string MediaType { get; set; } = null;

        public bool NeedBaseUrl { get; set; } = true;

        public RequestParam SetContentEncoding(UnicodeEncoding encoding)
        {
            ContentEncoding = encoding;
            return this;
        }

        public RequestParam SetMediaType(string mediaType)
        {
            MediaType = mediaType;
            return this;
        }

        public RequestParam SetIfModifiedSince(DateTime dt)
        {
            IfModifiedSince = dt;
            return this;
        }

        public RequestParam AddHeader(string key, string value)
        {
            Headers[key] = value;
            return this;
        }

        public RequestParam AddHeader(params string[] keyValues)
        {
            Headers.AppendForDict<string>(keyValues);
            return this;
        }

        public RequestParam AddHeader(string[] keys, string[] values)
        {
            Headers.AppendForDict<string, string>(keys, values);
            return this;
        }

        public RequestParam AddQueryString(string key, string value)
        {
            QueryStrings[key] = value;
            return this;
        }

        public RequestParam AddQueryString(params string[] keyValues)
        {
            QueryStrings.AppendForDict<string>(keyValues);
            return this;
        }

        public RequestParam AddQueryString(string[] keys, string[] values)
        {
            QueryStrings.AppendForDict<string, string>(keys, values);
            return this;
        }

        public RequestParam AddUrlSegements(string key, string value)
        {
            UrlSegments[key] = value;
            return this;
        }

        public RequestParam AddUrlSegements(params string[] keyValues)
        {
            UrlSegments.AppendForDict<string>(keyValues);
            return this;
        }

        public RequestParam AddUrlSegements(string[] keys, string[] values)
        {
            UrlSegments.AppendForDict<string, string>(keys, values);
            return this;
        }

        public RequestParam SetObjectBody(object obj, HttpContentType contentType)
        {
            return SetBody(HttpContentFactory.BuildHttpContent(contentType, obj));
        }

        public RequestParam SetBody(IHttpContent body)
        {
            Body = body;
            return this;
        }

        public RequestParam SetJsonObjectBody(IJsonValue jsonValue)
        {
            return SetBody(new HttpJsonContent(jsonValue));
        }

        public RequestParam SetJsonStringBody(string jsonValue)
        {
            return SetBody(new HttpJsonContent(jsonValue));
        }

        public RequestParam SetStringBody(string body)
        {
            Body = new HttpStringContent(body);
            return this;
        }

        public RequestParam SetStreamBody(IInputStream body)
        {
            Body = new HttpStreamContent(body);
            return this;
        }

        public RequestParam SetAuthorization(string scheme, string authorization)
        {
            SchemeAuthorization = scheme;
            Authorization = authorization;
            return this;
        }

        public RequestParam SetNeedBaseUrl(bool needBaseUrl)
        {
            NeedBaseUrl = needBaseUrl;
            return this;
        }

        internal void ApplyToRequester(HttpRequestMessage requester, HttpClientConfig config)
        {
            HandleBody(config);
            requester.Content = Body;

            foreach (var header in Headers)
            {
                requester.Headers.Append(header.Key, header.Value);
            }

            if (SchemeAuthorization != null && Authorization != null)
            {
                requester.Headers.Authorization = new HttpCredentialsHeaderValue(SchemeAuthorization, Authorization);
            }

            if (IfModifiedSince.HasValue)
            {
                requester.Headers.IfModifiedSince = new DateTimeOffset(IfModifiedSince.Value);
            }
        }

        private async void HandleBody(HttpClientConfig config)
        {
            if (Body is HttpStringContent)
            {
                var content = await Body.ReadAsStringAsync();
                if (ContentEncoding == null && MediaType == null)
                {
                    if (config.ContentEncoding != null && config.MediaType != null)
                    {
                        Body = new HttpStringContent(content, config.ContentEncoding.Value, config.MediaType);
                    }
                    else if (config.ContentEncoding != null)
                    {
                        Body = new HttpStringContent(content, config.ContentEncoding.Value);
                    }
                }
                else if (MediaType == null)
                {
                    if (config.MediaType != null)
                    {
                        Body = new HttpStringContent(content, ContentEncoding.Value, config.MediaType);
                    }
                }
                else if (ContentEncoding == null)
                {
                    if (config.ContentEncoding != null)
                    {
                        Body = new HttpStringContent(content, config.ContentEncoding.Value, MediaType);
                    }
                }
                else
                {
                    Body = new HttpStringContent(content, ContentEncoding.Value, MediaType);
                }
            }
        }
    }
}
