using ISS.Core.Model.Order;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISS.Common;
using ISS.BusinessRules.Contract.Common;
using ISS.BusinessRules.Contract.Order;
using Kendo.Mvc.Extensions;
using ISS.Core.Model.Common;



namespace ISS.Web.Controllers
{
    public partial class OrderController : BaseController
    {


        [HttpGet]
        public ActionResult CreateWorkOrder(WorkOrderHeader woh, bool? autoLoad)
        {
            //WorkOrderHeader woh = new WorkOrderHeader();
            //woh.PlannedWeek = GetPlannedWeek();
            //woh.PlannedYear = GetPlannedYear();
            //woh.PlannedDate = orderService.GetPlannedDate(woh.PlannedWeek, woh.PlannedYear, string.Empty); 

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
        public JsonResult CreateWorkOrder([DataSourceRequest]DataSourceRequest request, [Bind(Prefix = "models")] IList<WorkOrderDetail> resultData)
        {
            var result = resultData.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public ActionResult CreateWorkOrder([DataSourceRequest]DataSourceRequest request, [Bind(Prefix = "models")] IList<WorkOrderDetail> resultData)
        //{
        //    //woh.PlannedWeek = GetPlannedWeek();
        //    //woh.PlannedYear = GetPlannedYear();
        //    //woh.PlannedDate = GetPlannedDate(woh.PlannedWeek, woh.PlannedYear);

        //    return View();
        //}
        [HttpGet]
        public ActionResult WorkOrderEdit(WorkOrderDetail wod)
        {
            return PartialView(wod);
            //return View(wof);
        }

        [HttpGet]
        public ActionResult GetFabric(WorkOrderFabric wof)
        {
            return PartialView(wof);
            //return View(wof);
        }

        [HttpGet]
        public ActionResult GetCumulative(WorkOrderCumulative woc)
        {
            return PartialView(woc);
            //return View(wof);
        }

        protected decimal GetPlannedWeek()
        {
            return orderService.GetPlannedWeek();
        }
        protected decimal GetPlannedYear()
        {
            return orderService.GetPlannedYear();
        }

        [HttpPost]
        public JsonResult GetPlannedDate(decimal Week, decimal Year, string dueDate)
        {
            var date = orderService.GetPlannedDate(Week, Year, dueDate);
            var cDate = DateTime.Now;
            String ErrMsg = String.Empty;
            if ((DateTime.Now.Date - date).TotalDays > 60)
            {
                ErrMsg = "Planned Date cannot be more than 60 days past due.";
            }
            else if (((date - DateTime.Now.Date).TotalDays > 365))
            {
                ErrMsg = "Planned Date cannot be more than 1 year (365 days) in the future";
            }

            if (ErrMsg != String.Empty || date == DateTime.MinValue)
            { 
                var plannedDate = appService.GetPlanWeekYearBeginDate();
                if (plannedDate.Count > 0)
                {
                    var PlannedWeek = plannedDate[0].Fiscal_Week;
                    var PlannedYear = plannedDate[0].Fiscal_Year;
                    date = plannedDate[0].Week_Begin_Date;

                    return Json(new { result = date.ToShortDateString(), ErrMsg, PlannedWeek, PlannedYear }, JsonRequestBehavior.AllowGet);
                } 
            }
            var result = date.ToShortDateString();

            return Json(new { result, ErrMsg }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetDueDate([DataSourceRequest] DataSourceRequest result)
        {
            var dueDateList = Enum.GetValues(typeof(WrkOrderDueDate))
                                    .Cast<WrkOrderDueDate>()
                                    .ToList().Select(x => new SelectListItem { Text = x.GetDescription().ToString(), Value = x.GetDescription().ToString() });

            return Json(dueDateList.ToDataSourceResult(result), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetOrders()
        {
            var dc = new List<SelectListItem>();
            for (var i = 1; i <= 20; i++)
            {
                dc.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });

            }
            return Json(dc, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public JsonResult UpdateWODetails([DataSourceRequest]DataSourceRequest request, [Bind(Prefix = "models")] IList<WorkOrderDetail> resultData)
        {
            var result = resultData.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]


        public JsonResult GetColorCodes(WorkOrderDetail wod)
        {
            if (wod.SellingStyle != null)
            {
                var data = orderService.GetColorCodes(wod.SellingStyle).Select(x => new { Color = x.ColorCode, ColorName = x.ColorCode });
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else return null;
        }

        [HttpPost]
        public JsonResult GetAttributeCodes(WorkOrderDetail wod, String Src)
        {

            if (wod.SellingStyle != null)
            {
                if (!String.IsNullOrEmpty(Src) && Src == LOVConstants.WorkOrderType.AttributedWO)
                {
                    var data = orderService.GetAttributeCodes(wod.SellingStyle,
                        wod.ColorCode).Where(x => (x.Attribute != LOVConstants.NonAOAttribute))
                        .Select(x => new { Attribute = x.Attribute, AttributeDesc = x.AttributeDesc });

                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var data = orderService.GetAttributeCodes(wod.SellingStyle,
                       wod.ColorCode)
                       .Select(x => new { Attribute = x.Attribute, AttributeDesc = x.AttributeDesc });

                    return Json(data, JsonRequestBehavior.AllowGet);
                }

            }
            else return null;

        }


        [HttpPost]
        public JsonResult GetSizes(WorkOrderDetail wod)
        {

            if (wod.SellingStyle != null)
            {
                var data = orderService.GetSizes(wod.SellingStyle,
                    wod.ColorCode,
                    wod.Attribute).Select(x => new { SizeDesc = x.SizeShortDes, Size = x.Size });

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else return null;

        }
        [HttpPost]
        public JsonResult GetWOAsrtCode(WorkOrderDetail wod)
        {

            if (wod.SellingStyle != null)
            {
                var data = orderService.GetWOAsrtCode(wod.SellingStyle).Select(x => new { x.AssortCode, x.PrimaryDC, x.PackCode, x.OriginTypeCode, x.CorpBusUnit, x.ProdFamCode });

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else return null;
        }


        [HttpPost]
        public JsonResult GetWOChildSKU(WorkOrderDetail wod)
        {



            var data = orderService.GetWOChildSKU(wod.SellingStyle,
               wod.ColorCode,
              wod.Attribute,
              wod.OriginTypeCode,
               wod.Revision.ToString(),
               wod.SizeList,
               wod.AssortCode).Select(x => new { x.NewStyle, x.NewColor, x.NewAttribute, x.NewSize });

            return Json(data, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public JsonResult GetMaxRevision(WorkOrderDetail wod)
        {


            var data = orderService.GetMaxRevisions(wod.SellingStyle,
                wod.ColorCode,
               wod.Attribute,
              wod.SizeList, wod.AssortCode).Select(x => new { Revision = x.MaxRevision, RevisionCode = x.MaxRevision });
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetPKGStyle(WorkOrderDetail wod)
        {
            var data = orderService.GetPKGStyle(wod.SellingStyle,
                wod.ColorCode,
               wod.Attribute,
                wod.SizeList,
                wod.Revision.ToString(),
                 wod.AssortCode).Select(x => new { PKGStyle = x.PKGStyle, PKGStyleCode = x.PKGStyle });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPKGSty(WorkOrderDetail wod)
        {
            var data = orderService.GetPKGStyle(wod.SellingStyle,
                wod.ColorCode,
               wod.Attribute,
                wod.SizeList,
                wod.Revision.ToString(),
                 wod.AssortCode).Select(x => new { PKGStyle = x.PKGStyle, NewRevision = x.NewRevision });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //Newly Added
        public JsonResult GetPathRankingAltId(WorkOrderDetail wod)
        {
            var data = orderService.GetPathRankingAltId(wod.SellingStyle,
                wod.ColorCode,
               wod.Attribute,
                //wod.SizeList,
                wod.SizeCde,
                wod.MfgPathId,
                 wod.CutPath,
                 wod.TxtPath);
            if(data != null)
                data.Select(x => new { AlternateId = x.AlternateId });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //End

        /// <summary>
        /// Getting value for manufacturing path popup
        /// </summary>
        /// <param name="request"></param>
        /// <param name="searchCriteria"></param>
        /// <returns></returns>
        public JsonResult GetMFGPathId([DataSourceRequest]DataSourceRequest request, WorkOrderDetail wod)
        {
            if (wod.SellingStyle != null)
            {
                var data = orderService.GetMFGPathId(wod.SellingStyle,
                   wod.ColorCode,
                    wod.Attribute,
                   wod.SizeList,
                    wod.PrimaryDC).Select(x => new { MfgPathId = x.MfgPathId, SewPlt = x.SewPlt });
                var result = data.ToDataSourceResult(request);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else return null;
        }
        [HttpPost]
        public JsonResult GetRevisions([DataSourceRequest]DataSourceRequest request, string Style, string Color, string Attribute, string Size)
        {


            var data = orderService.GetMaxRevision(Style, Color, Attribute, Size);
            var result = data.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        //[HttpPost]
        //public JsonResult GetPlannedDate([DataSourceRequest]DataSourceRequest request, decimal Week, decimal Year)
        //{
        //    var data = orderService.GetPlannedDate(Week, Year);
        //    var result = data.ToShortDateString();
        //    return Json(result, JsonRequestBehavior.AllowGet);

        //}


        [HttpPost]
        public JsonResult CalculateCumulativeAndFabric(WorkOrderDetail WO)
        {

            var dataCum = orderService.UpdateCumulative(WO);
            var resultData = new { dataCum };


            return Json(resultData, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult CalculateVariance(WorkOrderDetail WO)
        {

            var dataCum = orderService.CalculateVariance(WO);
            var resultData = new { dataCum };


            return Json(resultData, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult OrdersToCreate(WorkOrderDetail WO)
        {

            var dataCum = orderService.OrdersToCreate(WO);
            var resultData = new { dataCum };


            return Json(resultData, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult OnChangePFS(WorkOrderDetail WO)
        {

            var dataCum = orderService.OnChangePFS(WO);
            var resultData = new { dataCum };


            return Json(resultData, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public JsonResult UpdateLBS(WorkOrderDetail WO)
        {

            var dataCum = orderService.UpdateLBS(WO);
            var resultData = new { dataCum };


            return Json(resultData, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public JsonResult CancelWODetail(WorkOrderDetail WO)
        {
            var dataCum = orderService.CancelWODetail(WO);
            var resultData = new { dataCum };


            return Json(resultData, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public JsonResult ReCalcWODetail(WorkOrderDetail WO)
        {
            var dataCum = orderService.ReCalcWODetail(WO);
            var resultData = new { dataCum };


            return Json(resultData, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public JsonResult GetRevDetails([DataSourceRequest]DataSourceRequest request, WorkOrderDetail wod)
        {
            //var data = orderService.GetRevisions(wod.SellingStyle,
            //    wod.ColorCode,
            //    wod.Attribute,
            //    wod.SizeShortDes,
            //    wod.AssortCode
            //    );
            var data = orderService.GetRevisions(wod.SellingStyle,
             wod.ColorCode,
             wod.Attribute,
             wod.SizeList,
             wod.AssortCode
             );

            var result = data.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetPackCode([DataSourceRequest]DataSourceRequest request)
        {
            var data = orderService.GetPackCode();
            var result = data.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetCatCode([DataSourceRequest]DataSourceRequest request)
        {
            var data = orderService.GetCatCode();
            var result = data.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ValidateCategoryCode(String catCode)
        {
            return Json(orderService.ValidateCategoryCode(catCode));
        }

        #region Get Dc Codes
        [HttpPost]
        public JsonResult GetDcCodes([DataSourceRequest] DataSourceRequest request)
        {
            var DcResult = orderService.GetDCCode();

            return Json(DcResult.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get Demand Drivers
        public JsonResult GetDemandDrivers(string Style, string Color, string Attribute, string Size, string RevisonNo)
        {
            var DcResult = orderService.GetDemandDrivers(Style, Color, Attribute, Size, RevisonNo);

            return Json(DcResult, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public JsonResult validateHAA(string Style, string Color, string Attribute, string Size)
        {
            var HAA = appService.ExternalSku(Style, Color, Attribute, Size);
            return Json(HAA, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPopupHaaAO(String ValHaa)
        {
            return Json(orderService.GetPopupHaaAO(ValHaa));
        }
    }

}