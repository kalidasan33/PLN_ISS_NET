using ISS.BusinessRules.Contract.Common;
using KA.BusinessRules.Contract.MaterialSupply;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using ISS.Web.Helpers;
using KA.Core.Model.MaterialSupply;
using System.IO;

namespace ISS.Web.Areas.KA.Controllers
{
    public class BlankSupplyController : ISS.Web.Controllers.BaseController
    {
        private readonly IMaterialSupplyService bOrderService;

        public BlankSupplyController(IMaterialSupplyService bulkorderService)
        {          
            bOrderService = bulkorderService;
        }


        public ActionResult MaterialBlankSupply(MaterialBlankSupplySearch mbSupply, bool? autoLoad)
        {
            ViewBag.autoLoad = autoLoad.HasValue ? autoLoad.Value : false;
            var data = bOrderService.MaterialSupplyGetWeeks();
            ViewBag.MSGridHeader = data;
            
            return View(mbSupply);
        }

        [HttpPost]
        public ActionResult BlankSupplyDetails([DataSourceRequest]DataSourceRequest request, MaterialBlankSupplySearch mbSupply)
        {
            var data = bOrderService.MaterialSupplySearchDetails(mbSupply);
            var result = data.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
            //return null;
            
        }

        [HttpPost]
        public ActionResult ExportPABDetails(MaterialBlankSupplySearch mbSupply)
        {
            mbSupply.SizeCD = mbSupply.AllSizes;
            mbSupply.DC = mbSupply.AllDcs;

            string fileName = string.Format("PAB{0}", DateTime.Now.ToString("yyyyMMddHHmmss")) + ".xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            var data = bOrderService.MaterialSupplySearchDetails(mbSupply);
            var header = bOrderService.MaterialSupplyGetWeeks();
            PABExportExcel excelExport = new PABExportExcel();
            MemoryStream msExcel = excelExport.PABDetailsToExcel(header, data);
            var fsr = new FileStreamResult(msExcel, contentType);
            fsr.FileDownloadName = fileName;

            return fsr;
        }
        [HttpPost]
        public JsonResult GetDC([DataSourceRequest] DataSourceRequest result)
        {
            var data = bOrderService.GetDC();

            return Json(data.ToDataSourceResult(result), JsonRequestBehavior.AllowGet);
        }

    }
}