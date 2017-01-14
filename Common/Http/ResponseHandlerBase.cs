using System;
using Windows.Web.Http;

namespace Common.Http
{
    public class ResponseHandlerBase
    {
        public Action<HttpResponseMessage> OnSuccess { get; set; }

        public Action<HttpRequestMessage> OnCancel { get; set; }

        public Action<HttpResponseMessage> OnFinish { get; set; }

        public Action<HttpResponseMessage> OnFailed { get; set; }

        public Action<HttpProgress> OnProgress { get; set; }

        public virtual void Handle(HttpResponseMessage response)
        {
            ExecIfNotNull(OnFinish, response);

            if (response.IsSuccessStatusCode)
            {
                ExecIfNotNull(OnSuccess, response);
            }
            else
            {
                ExecIfNotNull(OnFailed, response);
            }
        }

        protected void ExecIfNotNull<HttpResponseMessage>(Action<HttpResponseMessage> action, HttpResponseMessage param)
        {
            if (action != null)
                action(param);
        }
    }

    public class XResponseHandler<T> : ResponseHandlerBase, IResponseHandler<T>
    {
        public new Action<HttpResponseMessage, T> OnSuccess { get; set; }

        protected void ExecIfNotNull(Action<HttpResponseMessage, T> action, HttpResponseMessage param1, T param2)
        {
            if (action != null)
                action(param1, param2);
        }

        public override async void Handle(HttpResponseMessage response)
        {
            ExecIfNotNull(OnFinish, response);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var serializer = SerializerFactory.GetSerializer(response.Content.Headers.ContentType.MediaType);

                ExecIfNotNull(OnSuccess, response, serializer.Deserialize<T>(content));
            }
            else
            {
                ExecIfNotNull(OnFailed, response);
            }
        }
    }

    public class ResponseHandler : ResponseHandlerBase, IResponseHandler
    { }
}
