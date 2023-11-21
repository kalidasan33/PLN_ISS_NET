using ISS.BusinessRules.Contract.Common;
using KA.BusinessRules.Contract.BulkOrder;
using KA.Core.Model.BulkOrder;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using ISS.Web.Helpers;
using ISS.Core.Model.Common;

namespace ISS.Web.Areas.KA.Controllers
{
    public partial class BulkOrderController : ISS.Web.Controllers.BaseController
    {
        private readonly  IBulkOrderService bulkOrdService;
        private readonly IApplicationService appService;

        public BulkOrderController(IBulkOrderService bulkOrderService, IApplicationService applicationService)
        {
            bulkOrdService = bulkOrderService;
            appService = applicationService;
        }

        

        // GET: KA/BulkOrder
        public ActionResult Review()
        {
            BulkOrderDetail bulkdetail = new BulkOrderDetail();
            return View(bulkdetail);
        }


        public ActionResult Requisition()
        {
            BulkOrderDetail bulkdetail = new BulkOrderDetail();
            return View(bulkdetail);
        }


        [HttpGet]
        public ActionResult BulkOrderSearch()
        {
            DateTime today = DateTime.Today; // As DateTime
            
            var bulkSearch = new BulkOrderSearch();
            bulkSearch.FromDate = StartOfWeek(DateTime.Now, DayOfWeek.Sunday);
            bulkSearch.ToDate = today;
            bulkSearch.ExcludeProcessed = true;

            return PartialView(bulkSearch);
        }

        public DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }

        [HttpPost]
        public JsonResult GetBulkOrderDetail(string bulkOrdNo, string programSource)
        {
            var data = bulkOrdService.GetBulkOrderDetail(bulkOrdNo, programSource);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult BulkOrderSearchDetails([DataSourceRequest]DataSourceRequest request, BulkOrderSearch bulkSearch)
        {
            var data = bulkOrdService.BulkOrderSearchDetails(bulkSearch);
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var result = new ContentResult();
            serializer.MaxJsonLength = Int32.MaxValue;
            result.Content = serializer.Serialize(data.ToDataSourceResult(request));
            result.ContentType = "application/json";
            return result;
        }

        [HttpPost]
        public JsonResult BulkOrderSearchEnter(string reqId)
        {
            BulkOrderSearch bulkSearch = new BulkOrderSearch();
            bulkSearch.BulkNumber = reqId;
            var data = bulkOrdService.BulkOrderSearchDetails(bulkSearch);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetLineNumber(List<BulkOrderDetail> BulkOrderDetail)
        {
            var data = bulkOrdService.GetLineNumber(BulkOrderDetail);
           
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ExportBulkOrderSearchDetails(BulkOrderSearch bulkSearch)
        {
            var data = bulkOrdService.BulkOrderSearchDetails(bulkSearch);

            string[] excelColumns = new string[] { "BulkNumber", "ProcessedToOS", "DmdWkEndDate", "VendorNo", "LwVendorLoc", "MFGPathId", "DcLoc" };

            string fileName = string.Format("BulkOrderSearchDetails{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));

            return new ExcelResult(fileName).
                AddSheet<BulkOrderDetail>(data, "BulkOrder Search Details", excelColumns);
        }

        public ActionResult ExportBulkOrderDetails(string BulkNumber, string ProgramSource)
        {
            var data = bulkOrdService.GetBulkOrderDetail(BulkNumber, ProgramSource);
            var excelColumns = new string[] { "BulkNumber","LineNumber", "Style", "Color", "Attribute", "Size", "SizeLit", "Rev", "Uom", "Quantity", "CurrDueDate", "Exception" }; ;
            if (ProgramSource == ISS.Common.KAProgramSource.ISS2165.ToString())
            {
                excelColumns = new string[] { "BulkNumber", "LineNumber", "Style", "Color", "Attribute", "Size", "SizeLit", "Rev", "Uom", "Quantity", "CurrDueDate", "Exception" };
            }
            else
            {
                excelColumns = new string[] { "Style", "Color", "Attribute", "Size", "SizeLit", "Quantity", "APSStyle", "APSColor", "APSAttribute", "APSSizeLit", "DmdWkEndDate", "DemandSource", "PrioritySeq", "UserId", "Exception", "BulkNumber" };
            }
            

            string fileName = string.Format("BulkOderDetails{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));

            return new ExcelResult(fileName).
                AddSheet<BulkOrderDetail>(data, "Bulk Order Details", excelColumns);

        }

        [HttpPost]
        public JsonResult ValidateBulkOrder(List<BulkOrderDetail> lstBulkOrders)
        {
            string errMsg = "";
            lstBulkOrders.ForEach(e =>
            {
                e.ErrorStatus = !bulkOrdService.VerifyComponentOrder(e, out errMsg);
                e.ErrorMessage = errMsg;
            });
            
            //var data = lstBulkOrders;

            return Json(lstBulkOrders, JsonRequestBehavior.AllowGet);
        }
    }
}