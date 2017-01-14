using Windows.Web.Http.Filters;

namespace Common.Http
{
    public interface ICustomHttpFilter : IHttpFilter
    {
        IHttpFilter InnerFilter { get; set; }
    }
}