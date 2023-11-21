using System.Web;
using System.Web.Mvc;
using log4net;

namespace ISS.Web.Filters
{
    public class CustomHandleErrorAttribute : HandleErrorAttribute
    {
        private readonly ILog _logger;

        public CustomHandleErrorAttribute()
        {
            _logger = LogManager.GetLogger("MyLogger");
        }
      
        /// <summary>
        /// Called on [exception] event.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnException(ExceptionContext filterContext)
        {

            // log the error using log4net.
            _logger.Error("HandleError >> "+filterContext.Exception.Message, filterContext.Exception);

            if (filterContext.ExceptionHandled  || (System.Web.HttpContext.Current.Request.IsLocal && ( !(filterContext.HttpContext.IsCustomErrorEnabled))))
            {
                return;
            }

            if (new HttpException(null, filterContext.Exception).GetHttpCode() != 500)
            {
                return;
            }

            if (!ExceptionType.IsInstanceOfType(filterContext.Exception))
            {
                return;
            }

            // if the request is AJAX return JSON else view.
            if (filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                filterContext.Result = new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new
                    {
                        error = true,
                        message = filterContext.Exception.Message
                    }
                };
            }
            else
            {
                var controllerName = (string) filterContext.RouteData.Values["controller"];
                var actionName = (string) filterContext.RouteData.Values["action"];
                var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);

                filterContext.Result = new ViewResult
                {
                    ViewName = View,
                    MasterName = Master,
                    ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                    TempData = filterContext.Controller.TempData
                };
            }

          
            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = 500;

            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }
    }
}