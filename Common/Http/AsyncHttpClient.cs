using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Common.Http
{
    public class AsyncHttpClient
    {
        private Uri uri;
        private Dictionary<string, string> headers;
        private string encoding;

        public AsyncHttpClient Url(string url)
        {
            uri = new Uri(url);
            return this;
        }

        public AsyncHttpClient Uri(Uri uri)
        {
            this.uri = uri;
            return this;
        }


        public AsyncHttpClient Encoding(string encoding)
        {
            this.encoding = encoding;
            Header("Encoding", encoding);
            return this;
        }


        public AsyncHttpClient Header(string name, string value)
        {
            if (headers == null)
            {
                headers = new Dictionary<string, string>();
            }
            headers[name] = value;

            return this;
        }

        public AsyncHttpClient Cookies(string cookies)
        {
            if (cookies != null)
            {
                Header("Cookie", cookies);
            }
            return this;
        }

        public AsyncHttpClient Referer(string referer)
        {
            if (referer != null)
            {
                Header("Referer", referer);
            }
            return this;
        }

        public AsyncHttpClient UserAgent(string userAgent)
        {
            if (userAgent != null)
            {
                Header("User-Agent", userAgent);
            }
            return this;
        }

        public AsyncHttpClient ContentType(string contentType)
        {
            if (contentType != null)
            {
                Header("Content-Type", contentType);
            }
            return this;
        }

        public AsyncHttpClient Accept(string accept)
        {
            if (accept != null)
            {
                Header("Accept", accept);
            }
            return this;
        }


        public async Task<AsyncHttpResponse> Get()
        {
            var client = DoBuildHttpClient();

            try
            {
                using (var rsp = await client.GetAsync(uri))
                {
                    return new AsyncHttpResponse(rsp, encoding);
                }
            }
            catch (Exception ex)
            {
                return new AsyncHttpResponse(ex, encoding);
            }
        }

        public async Task<AsyncHttpResponse> Post(Dictionary<string, string> args)
        {
            var client = DoBuildHttpClient();

            var postData = new FormUrlEncodedContent(args);

            try
            {
                using (var rsp = await client.PostAsync(uri, postData))
                {
                    return new AsyncHttpResponse(rsp, encoding);
                }
            }
            catch (Exception ex)
            {
                return new AsyncHttpResponse(ex, encoding);
            }
        }

        private HttpClient DoBuildHttpClient()
        {

            HttpClient client = new HttpClient();

            if (headers != null)
            {
                foreach (var kv in headers)
                {
                    client.DefaultRequestHeaders.Add(kv.Key, kv.Value);
                }
            }

            return client;
        }
    }
}
