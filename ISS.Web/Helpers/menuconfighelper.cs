using ISS.Core.Model.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml;
using ISS.Common;
using System.Web.Security;


namespace ISS.Web.Helpers
{
    public static class MenuConfigHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="menuName"> order/summary</param>
        /// <returns></returns>
        public static bool? IsMenuAuthorised(string menuName)
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings[LOVConstants.EnableAuthorization]))
            {
                return true;
            }
           
            List<MenuConfiguration> xmlValue = HttpContext.Current.Cache[SessionConstant.MENU_ITEMS_AUTH] as List<MenuConfiguration>;

            if (xmlValue != null)
            {
                List<UserRoles> roles = xmlValue.Where(m => m.MenuName.ToLower().Equals(menuName.ToLower()))
                                                .SelectMany(r => r.MenuRoles)
                                                .ToList();

                //var role = "SC-AVYX_ISS-KA-PLANNER-RW";
                //var UserRoles = roles.Where(e => role==(e.RoleName)).ToList();

                var UserRoles = roles.Where(e => HttpContext.Current.User.IsInRole(e.RoleName)).ToList();

                

                if (UserRoles.Count > 0)
                {
                    if (UserRoles.Any(e => e.Enabled.HasValue && e.Enabled.Value))
                    {
                        return true;
                    }
                    else if (UserRoles.Any(e => e.Enabled.HasValue && (!e.Enabled.Value)))
                    {
                        return false;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return false; // Role not found in DB
                }
            }
            return null;
        }

        public static bool IsMenuNameExists(string menuName)
        {
            List<MenuConfiguration> xmlValue = HttpContext.Current.Cache[SessionConstant.MENU_ITEMS_AUTH] as List<MenuConfiguration>;
            if (xmlValue == null)
            {
                return false;
            }
            return xmlValue.Any(m => m.MenuName.ToLower().Equals(menuName.ToLower()));
        }
        
        public static void RegisterMenuItems()
        {
            if (HttpContext.Current.Cache[SessionConstant.MENU_ITEMS_AUTH] == null)
            {
                string filePath = string.Format("~/App_Data/{0}", ConfigurationManager.AppSettings["RoleConfigFile"]);
                XmlDocument doc = new XmlDocument();
                doc.Load(HttpContext.Current.Server.MapPath(filePath));
                HttpContext.Current.Cache[SessionConstant.MENU_ITEMS_AUTH] = doc.DocumentElement.AsObject<List<MenuConfiguration>>();
            }
        }
    }
}