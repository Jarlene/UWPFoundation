using System;
using Windows.Web.Http;

namespace Common.Http
{
    public interface IResponseHandler
    {
        Action<HttpResponseMessage> OnSuccess { get; set; }

        Action<HttpResponseMessage> OnFailed { get; set; }

        Action<HttpResponseMessage> OnFinish { get; set; }

        Action<HttpProgress> OnProgress { get; set; }

        Action<HttpRequestMessage> OnCancel { get; set; }

        void Handle(HttpResponseMessage response);
    }

    public interface IResponseHandler<T> : IResponseHandler
    {
        new Action<HttpResponseMessage, T> OnSuccess { get; set; }
    }
}
