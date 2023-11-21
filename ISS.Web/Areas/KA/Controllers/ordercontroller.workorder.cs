using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISS.Web.Controllers ;
using ISS.Core.Model.Order;
using ISS.BusinessRules.Contract.Common;
using ISS.BusinessRules.Contract.Order;
using KA.BusinessRules.Contract;
using KA.BusinessRules.Contract.AttributionOrder;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Web.WebPages;
using ISS.Common;
namespace ISS.Web.Areas.KA.Controllers
{
    public partial class AttributionOrderController : ISS.Web.Controllers.BaseController 
    {


       

        [HttpGet]
        public ActionResult CreateWorkOrder(WorkOrderHeader woh, bool? autoLoad)
        {
               woh.Dmd = "FC";

            var plannedDate = appService.GetPlanWeekYearBeginDate();
            if (plannedDate.Count > 0)
            {
                woh.PlannedWeek = plannedDate[0].Fiscal_Week;
                woh.PlannedYear = plannedDate[0].Fiscal_Year;
                woh.PlannedDate = plannedDate[0].Week_Begin_Date;
                woh.MinYear = woh.PlannedYear - 1;
                woh.MaxYear = woh.PlannedYear + 1;
            }

            ViewBag.autoLoad = autoLoad.HasValue ? autoLoad.Value : false;
            

            return View(woh);
        }

        [HttpPost]
        public ActionResult CreateWorkOrder(WorkOrderHeader woh)
        {
            woh.Dmd = "FC";

            var plannedDate = appService.GetPlanWeekYearBeginDate();
            if (plannedDate.Count > 0)
            {
                woh.PlannedWeek = plannedDate[0].Fiscal_Week;
                woh.PlannedYear = plannedDate[0].Fiscal_Year;
                woh.PlannedDate = plannedDate[0].Week_Begin_Date;
            }

            

            return View(woh);

            
        }
         
        [HttpPost]
        public JsonResult InsertAttributionOrder(WorkOrderHeader header)
        {
            if (header != null)
            {
                header.UpdatedBy = header.CreatedBy = GetCurrentUserName();
            }
            header.WODetails.Each(e =>
            {
                if (!string.IsNullOrEmpty(e.DozenStr))
                e.CurrOrderQty = e.CurrOrderTotQty = e.DozenStr.Replace("-", ".").AsDecimal().ConvertDzToEaches();

            });
            var resu = attriborderService.InsertAttributionOrder(header);
            return Json(resu);
         
           

        }
        [HttpPost]
        public JsonResult GetKAMFGPathId(WorkOrderDetail wod)
        {
            if (wod.SellingStyle != null)
            {
                var data = attriborderService.GetKAMFGPathId(wod.SellingStyle,
                   wod.ColorCode,
                    wod.Attribute,
                   wod.SizeList,
                    wod.PrimaryDC).Select(x => new { MfgPathId = x.MfgPathId, SewPlt = x.SewPlt, GarmentSKU = x.GarmentSKU, GStyle=x.GStyle, GColor=x.GColor, GAttribute=x.GAttribute, GSize=x.GSize });
                //var result = data.ToDataSourceResult(request);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else return null;
        }

        [HttpPost]
        public JsonResult GetKAMFGPathDetails(WorkOrderDetail wod)
        {
            if (wod.SellingStyle != null)
            {
                var data = attriborderService.GetKAMFGPathDetails(wod.SellingStyle,
                   wod.ColorCode,
                    wod.Attribute,
                   wod.SizeList,
                    wod.PrimaryDC, wod.MfgPathId).Select(x => new { MfgPathId = x.MfgPathId, SewPlt = x.SewPlt, GarmentSKU = x.GarmentSKU, GStyle = x.GStyle, GColor = x.GColor, GAttribute = x.GAttribute, GSize = x.GSize });
                //var result = data.ToDataSourceResult(request);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else return null;
        }

        [HttpPost]
        public JsonResult ValidatePKGStyle(WorkOrderDetail wod)
        {
            bool data = false;
            String ErrMsg = String.Empty;
            if(wod.PKGStyle!=null)
            {
                data = attriborderService.ValidatePKGStyle(wod);
                
            }
            if(!data)
            {
                ErrMsg = "Invalid PKG Style";
            }
            return Json(new { data, ErrMsg }, JsonRequestBehavior.AllowGet);
            
        }

        [HttpPost]
        public JsonResult GetAODCLocations(WorkOrderDetail wod)
        {
            if (wod.SellingStyle != null)
            {
                var data = attriborderService.GetAODCLocations(wod.SellingStyle,
                   wod.ColorCode,
                    wod.Attribute).Select(x => new { DC = x.Dc, LegalName=x.LegalName});
               
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else return null;
        }

    }
}