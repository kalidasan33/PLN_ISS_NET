using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ISS.Web.Filters
{
    public class ActionExecutingFilter : ActionFilterAttribute
    {


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            base.OnActionExecuting(filterContext);

            var controller=filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string action = filterContext.ActionDescriptor.ActionName;
            if (!(controller.Equals("home", StringComparison.OrdinalIgnoreCase) && action.Equals("Maintenance", StringComparison.OrdinalIgnoreCase)))
            {


                ISS.BusinessRules.Contract.Common.IApplicationService appService = DependencyResolver.Current.GetService<ISS.BusinessRules.Contract.Common.IApplicationService>();

                if (!appService.ISSAvailable())
                {

                    filterContext.Result = new RedirectToRouteResult(
                      new RouteValueDictionary 
                { 
                    { "controller", "Home" }, 
                    { "action", "Maintenance" } 
                });
                }
            }
        }
    }
}