using System.Web.Mvc;

namespace ISS.Web.Areas.KA
{
    public class KAAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "KA";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "KA_default",
                "KA/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}