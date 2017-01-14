using Windows.Web.Http;

namespace Common.Http
{
    public static class HttpContentFactory
    {
        public static IHttpContent BuildHttpContent(HttpContentType type, object data)
        {
            switch (type)
            {
                case HttpContentType.Json:
                    return new HttpJsonContent(data);
                case HttpContentType.Xml:
                    return null;//TODO: add xml serializer
                case HttpContentType.Text:
                    return new HttpStringContent(data.ToString());
                default:
                    return null;
            }
        }
    }
}
