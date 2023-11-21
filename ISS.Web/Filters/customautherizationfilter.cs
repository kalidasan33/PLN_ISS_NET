using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISS.MVC.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class CustomAuthorize : AuthorizeAttribute
    {
        private string[] UserProfilesRequired { get; set; }
        private bool IsAutherizationRequired=false;

        public CustomAuthorize()
        {
            var hbiGroups=string.Empty;
            var userGroups = ConfigurationManager.AppSettings["allow.app.groups"];
            var requiredAutherization = ConfigurationManager.AppSettings["authorize.groups"];
            if (userGroups.Length==0)
            {
                throw new ArgumentException("userProfilesRequired");
            }
            this.UserProfilesRequired = userGroups.Split(',').ToArray();
            bool.TryParse(requiredAutherization, out IsAutherizationRequired);
        }

        public override void OnAuthorization(AuthorizationContext context)
        {
            if (!this.IsAutherizationRequired || this.UserProfilesRequired.Length == 0)
            {
                return;
            }

            bool authorized = false;

            foreach (var role in this.UserProfilesRequired)
                if (HttpContext.Current.User.IsInRole(role))
                {
                    authorized = true;
                    break;
                }

            if (!authorized)
            {
                var url = new UrlHelper(context.RequestContext);
                var logonUrl = @"/Error/Index";
                context.Result = new RedirectResult(logonUrl);
                return;
            }
        }
    }
}