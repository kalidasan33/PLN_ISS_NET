using ISS.BusinessRules.Contract.Capacity;
using ISS.Common;
using ISS.Core.Model.Capacity;
using ISS.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;


namespace ISS.Web.Controllers
{
    public class CapacityController : BaseController
    {
        
        private const string FILE_PATH = "CapacityAllocations{0}";
        
        private readonly ICapacityService CapacityService;

        public CapacityController(ICapacityService CapacityService)
        {
            this.CapacityService = CapacityService;
        }

        [HttpGet]
        public ActionResult Allocation(CapacitySearch search)
        {           
            return View(search);
        }


        [HttpPost]
        public PartialViewResult Allocation([DataSourceRequest]DataSourceRequest request, CapacitySearch search,String Src)
        {
            if (String.IsNullOrEmpty(Src)) return null;
            string action = "Grid";
            var data = CapacityService.GetCapacityAllocations(search,action);
            Response.Headers.Add("count", data != null ? data.Rows.Count.ToString() : "-1");
            return PartialView("_allocationDetails", data);
        }

        public JsonResult GetCapacitySortDirections([DataSourceRequest] DataSourceRequest result)
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "By Plant, Workcenter ", Value = " order by decode(cssclass,'Agg',1,'Ind',2,3) , \"Plant\", \"WorkCenter\" , \"DueDate\"" });
            list.Add(new SelectListItem { Text = "By Workcenter, Plant ", Value = " order by \"WorkCenter\", decode(cssclass,'Agg',1,'Ind',2,3) ,   \"Plant\" , \"DueDate\"" });
            return Json(list.ToDataSourceResult(result), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllCapacityGroup([DataSourceRequest] DataSourceRequest result)
        {
            var cgroupList = Enum.GetValues(typeof(CapacityGroupAll))
                                    .Cast<CapacityGroupAll>()
                                    .ToList()
                                    .Select(x => new SelectListItem { Text = x.ToString().ToUpper(), Value = x.ToString().ToUpper() })
                                    .OrderBy(t=>t.Text);

            return Json(cgroupList.ToDataSourceResult(result), JsonRequestBehavior.AllowGet);
        }

        // For binding the allocation popup grid
        [HttpPost]
        public ContentResult GetAggregationDetails([DataSourceRequest]DataSourceRequest request, CapacitySearch search)
        {
            IList<AggregatePopup> data = CapacityService.GetAggregationDetails(search);
           
            var resultSet = data.ToDataSourceResult(request);

            return this.ISSJson(resultSet);

        }

        // For binding the allocation popup grid
        [HttpPost]
        public ContentResult GetAllocationDetails([DataSourceRequest]DataSourceRequest request, CapacitySearch search)
        {
            IList<AllocationPopup> data = CapacityService.GetAllocationDetails(search);
            data.ToList().ForEach(s => s.FormattedDueDate = s.DueDate.ToString("MM/dd/yyyy"));

            var resultSet = data.ToDataSourceResult(request);

            return this.ISSJson(resultSet);
            
        }

        // Allocation popup grid export
        public ActionResult ExportAllocations(string CapacityGroup)
        {
            string fileName = string.Format(FILE_PATH, DateTime.Now.ToString("yyyyMMddHHmmss"));
            CapacitySearch search = new CapacitySearch() { CapacityGroup = CapacityGroup};
            search.Plant  = Request.Form["Plant"];
            search.WorkCenter = Request.Form["WorkCenter"];
            search.CapacityGroup = Request.Form["CapacityGroup"];
            search.OrderBy = Request.Form["OrderBy"];

            string check = Request.Form["CapIndView"];

            if (check != null) 
            {
                search.ShowIndivWorkcenters = true;
                 if (check == "false")
                    search.ShowIndivWorkcenters = false;
            }

            check = Request.Form["CapAggView"];

            if (check != null)
            {
                search.ShowAggregateWorkcenters = true;
                if (check == "false")
                    search.ShowAggregateWorkcenters = false;
            }


            string action = "Export";
            var data = CapacityService.GetCapacityAllocations(search, action);

            // Remove columns which need not be shown in the excel
            data.Columns.Remove("Workcenter1");
            data.Columns.Remove("ProductionStatus");
            data.Columns.Remove("SpillOver");
            data.Columns.Remove("Plant1");
            //Newly Added for removing Target WOS,Actual WOS,SAH to Target from column headers
            data.Columns.Remove("Target WOS1");
            data.Columns.Remove("Actual WOS1");
            data.Columns.Remove("SAH to Target1");
            //End
            data.Columns.Remove("Type1");
            data.Columns.Remove("CapacityGroup");
            //Newly Added for removing Prior from exporting
            data.Columns.Remove("Prior");
            //End

            DataSet ds = new DataSet();
            ds.Tables.Add(data);
            return new ExcelDataTableResult(fileName).AddDataTableSheet(ds, "CapacityAllocations");
        }

        // Allocation popup grid export
        public ActionResult ExportAllocationDetails(CapacitySearch search)
        {
            IList<AllocationPopup> data = CapacityService.GetAllocationDetails(search);

            string[] excelColumns = new string[] { "CapacityGroup", "Plant", "WorkCenter", "StyleCD", "ColorCD", "AttributeCD", "SizeCD", "ProdOrderNo", "WOQty",  "SAH", "DueDate", "Priority" };

            string fileName = string.Format("AllocationDetails{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));

            return new ExcelResult(fileName).
                AddSheet<AllocationPopup>(data, "Allocation Details", excelColumns);
        }

    }
}
