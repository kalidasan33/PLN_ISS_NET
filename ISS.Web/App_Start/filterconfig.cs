using System.Web;
using ISS.Web.Filters;
using System.Web.Mvc;

namespace ISS.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CustomHandleErrorAttribute());
            filters.Add(new ActionExecutingFilter());
        }
    }
}
