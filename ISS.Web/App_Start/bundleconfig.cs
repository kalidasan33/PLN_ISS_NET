using System.Web;
using System.Web.Optimization;
using System;

namespace ISS.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles,String version="")
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/kendo/2016.1.420/jquery.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/kendo.modernizr.custom.js"));

            bundles.Add(new ScriptBundle("~/bundles/kendo").Include(
                      "~/Scripts/kendo/2016.1.420/kendo.all.min.js",
                      "~/Scripts/kendo/jszip.js"
                      ));
            bundles.Add(new ScriptBundle("~/bundles/kendo2").Include(
                    "~/Scripts/kendo/2016.1.420/kendo.aspnetmvc.min.js"));
                    
            bundles.Add(new StyleBundle("~/Content/css"+version).Include(
                   "~/Content/ISS.css",
                   "~/Content/issReset.css",
                   "~/Content/Site.Kendo.Custom.css"));

            bundles.Add(new StyleBundle("~/content/Bootstrap1").Include(
                     "~/Content/kendo/2016.1.420/kendo.common-bootstrap.min.css"
                     ));
            bundles.Add(new StyleBundle("~/content/Bootstrap2").Include(
                       "~/Content/kendo/2016.1.420/kendo.bootstrap.min.css"
                    ));

            bundles.Add(new ScriptBundle("~/bundles/custom" + version).Include(
                 "~/Scripts/app/common.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/information" + version).Include(
             "~/Scripts/app/information.js"));

            bundles.Add(new ScriptBundle("~/bundles/summary" + version).Include(
             "~/Scripts/app/Summary.js"));

            bundles.Add(new ScriptBundle("~/bundles/sourceorder" + version).Include(
             "~/Scripts/app/SourceOrder.js"
             , "~/Scripts/app/SourceOrder2.js"
             , "~/Scripts/app/SourceOrder3.js"
             , "~/Scripts/app/SourceOrderExt.js"
             ));

            bundles.Add(new ScriptBundle("~/bundles/createWO" + version).Include(
            "~/Scripts/app/WorkOrder.js"
            , "~/Scripts/app/WorkOrder2.js"));

            bundles.Add(new ScriptBundle("~/bundles/createMSKUWO" + version).Include(
            "~/Scripts/app/MultiSKUWorkOrder.js"));

            bundles.Add(new ScriptBundle("~/bundles/Capacity" + version).Include(
          "~/Scripts/app/Capacity.js"));

            bundles.Add(new ScriptBundle("~/bundles/amrz" + version).Include(
          "~/Scripts/app/AttributeMrz.js"));

            bundles.Add(new ScriptBundle("~/bundles/WOM" + version).Include(
          "~/Scripts/app/WOM.js"
          , "~/Scripts/app/WOMEdit.js"
          , "~/Scripts/app/WOMSave.js"
          ));

            bundles.Add(new ScriptBundle("~/bundles/Textiles" + version).Include(
            "~/Scripts/app/Textiles.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/review" + version).Include(
           "~/areas/KA/Scripts/review.js"
           , "~/areas/KA/Scripts/review2.js"
           , "~/areas/KA/Scripts/review3.js"
           , "~/areas/KA/Scripts/reviewExt.js"

       ));

            bundles.Add(new ScriptBundle("~/bundles/AttributionOM" + version).Include(
          "~/areas/KA/Scripts/AttributionWOM.js"
          , "~/areas/KA/Scripts/AttributionWOMEdit.js"
          , "~/areas/KA/Scripts/AttributionWOMSave.js"
          ));


            bundles.Add(new ScriptBundle("~/bundles/AttributtedCWO" + version).Include(
          "~/areas/KA/Scripts/WorkOrder.js"
          , "~/areas/KA/Scripts/WorkOrder2.js"));

            bundles.Add(new ScriptBundle("~/bundles/Supply" + version).Include(
         "~/areas/KA/Scripts/MaterialSupply.js"));
        }
    }
}


