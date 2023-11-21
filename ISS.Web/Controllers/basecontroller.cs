using System.Web.Mvc;
using LoggingUtil;
using Microsoft.Practices.Unity;
using ISS.Web.Filters;
using System;
using System.DirectoryServices.AccountManagement;
using ISS.Web.Helpers;
using ISS.Common;
using System.Web.Routing;
using System.Web;

namespace ISS.Web.Controllers
{
    [CustomHandleError]
    public abstract class BaseController : Controller
    {
        [Dependency]
        protected ILogger Logger { get; set; }


        #region Protected Methods

        protected override IAsyncResult BeginExecute(System.Web.Routing.RequestContext requestContext, AsyncCallback callback, object state)
        {

            string controller = requestContext.RouteData.Values["controller"].ToString().ToLower();
            string action = requestContext.RouteData.Values["action"].ToString().ToLower();
            Log("Begin Execute => " + controller + "/" + action);
            ViewData.Add("__Controller__", controller);
            ViewData.Add("__Action__", action);

            return base.BeginExecute(requestContext, callback, state);
        }

        /// <summary>
        ///     Called on [action executing] event.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!Request.IsAjaxRequest())
            {

                using (var context = new PrincipalContext(ContextType.Domain))
                {
                    try
                    {
                        ViewData.Add("UserId", GetCurrentUserName());
                        var principal = UserPrincipal.FindByIdentity(context, User.Identity.Name);
                        if (principal != null)
                        {
                            var fullName = string.Format("{0} {1}", principal.GivenName, principal.Surname);
                            ViewData.Add("UserName", fullName);                           
                        }
                    }
                    catch
                    {
                        ViewData.Add("UserName", GetCurrentUserName());
                       
                    }

                }

            }
            Logger.Info(filterContext.ActionDescriptor.ActionName);
            base.OnActionExecuting(filterContext);
        }

        public String GetCurrentUserName()
        {
            if (User != null && User.Identity != null && !String.IsNullOrEmpty(User.Identity.Name))
            {
                //HBIRESlijohn
                //HBIRES

                return User.Identity.Name.Replace("HBIRES", String.Empty).Replace("\\", String.Empty);
            }
            return String.Empty;
        }

        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {

            Log("End Execute => ");


            var browserInfo = Request.Browser.Browser;
            if (filterContext.Result is FileResult)
            {
                if (browserInfo == "IE")
                {
                    filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.Private);
                    Log("End  IE  Private => ");
                }
                else
                    filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            }
            else
            {
                filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
                filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
                filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
                filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                filterContext.HttpContext.Response.Cache.SetNoStore();
            }

            base.OnResultExecuting(filterContext);
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            string controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower();
            string action = filterContext.ActionDescriptor.ActionName.ToLower();
            string menuName = string.Format("{0}/{1}", controller, action);

            if (!Request.IsLocal && (!filterContext.HttpContext.Request.IsAuthenticated))
            {

                System.Web.Configuration.AuthenticationSection section =
    (System.Web.Configuration.AuthenticationSection)System.Web.Configuration.WebConfigurationManager.GetSection("system.web/authentication");
                String msg = String.Empty;
                if (!(section.Mode == System.Web.Configuration.AuthenticationMode.Windows))
                {
                    msg = "<p style='display:none;'> WINDOWS Auth not enabled.</p>";
                }

                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Index", area = "" }));
                SessionHelper.OutputMessage = "Authentication failed." + msg;
            }
            else
            {
                if (MenuConfigHelper.IsMenuNameExists(menuName))
                {
                    var menu = MenuConfigHelper.IsMenuAuthorised(menuName);
                    if (!(menu.HasValue && menu.Value))
                    {
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Index" ,area=""}));
                        SessionHelper.OutputMessage = "Not authorized to access this page";
                    }
                }
            }
        }

        #endregion Protected Methods

        #region Protected Methods

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected void Log(string message)
        {
            Logger.Info(message);
        }

        /// <summary>
        /// Logs the specified exception.
        /// </summary>
        /// <param name="severity">Severity of the Business Exception</param>
        protected void Log(Exception exception)
        {
            Logger.Error(exception);
        }
        /// <summary>
        /// Logs the specified exception.
        /// </summary>
        /// <param name="severity">Severity of the Business Exception</param>
        protected void Log(String Msg, Exception exception)
        {
            Logger.Error(Msg, exception);
        }
        #endregion

        protected ContentResult ISSJson(dynamic data)
        {
            //var ttt = Newtonsoft.Json.JsonConvert.SerializeObject(data);


            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer()
            { 
                MaxJsonLength = 2147483644,
                RecursionLimit = 100 
            };
           
            return   new ContentResult()
            {
                Content = serializer.Serialize( data ),
                ContentType = "application/json",
            };
        }

    }
}