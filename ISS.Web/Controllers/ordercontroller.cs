using ISS.Core.Model.Order;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using ISS.Common;
using ISS.BusinessRules.Contract.Common;
using ISS.BusinessRules.Contract.Order;
using ISS.Web.Helpers;
using ISS.Core.Model.Common;
using Newtonsoft.Json;
 

namespace ISS.Web.Controllers
{
    public partial class OrderController : BaseController
    {
        private readonly IOrderService orderService;
        private readonly IApplicationService appService;

        public OrderController(IOrderService summService, IApplicationService applicationService)
        {
            orderService = summService;
            appService = applicationService;
        }


        [HttpGet]
        public PartialViewResult test(String Style, String Planner, String Size, String Color)
        {
            if (!string.IsNullOrEmpty(Style))
            {
                var data = orderService.GetSummary(new ISS.Core.Model.Order.SummaryFilterModal()
                {
                    Style = Style,
                    Planner = Planner,
                    Size = Size,
                    Color = Color
                });

                return PartialView("testSum",data);
            }
            return PartialView();
        }

        [HttpGet]
        public ActionResult Summary(SummaryFilterModal SummaryView, bool? autoLoad)
        {
            SummaryView.PlanWeek = GetPlantWeek();
            ViewBag.autoLoad = autoLoad.HasValue?autoLoad.Value:false;

            return View(SummaryView);
        }

        [HttpPost]
        public ContentResult Summary([DataSourceRequest]DataSourceRequest request, SummaryFilterModal SummaryView,String  Src)
        {
            if (String.IsNullOrEmpty(Src))
            {
                return ISSJson(new List<SummaryResult>().ToDataSourceResult(request));
            }
            var data = orderService.GetSummary(SummaryView);

            var result = data.ToDataSourceResult(request);
            return ISSJson(result);
            //return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSuggestedWOWeek([DataSourceRequest] DataSourceRequest request)
        {
            var numbers = (from p in Enumerable.Range(3, 50)
                           select new SelectListItem
                           {
                               Text = p.ToString(),
                               Value = p.ToString()
                           });

            return Json(numbers.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetCapacityGroup([DataSourceRequest] DataSourceRequest result)
        {
            /*var cgroupList = Enum.GetValues(typeof(CapacityGroup))
                                    .Cast<CapacityGroup>()
                                    .ToList().Select(x => new SelectListItem { Text = x.ToString(), Value = x.ToString() });
            */

            // US 56194 ISS Web Attr Filter

            var cgroupList = Enum.GetValues(typeof(CapacityGroupAll))
                                    .Cast<CapacityGroupAll>()
                                    .ToList()
                                    .Select(x => new Planner { PlannerCd = x.ToString().ToUpper() })
                                    .OrderBy(t => t.PlannerCd);
             

            return Json(cgroupList.ToDataSourceResult(result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult PlannerAjaxAction(string id, string color)
        {
            if (Request.IsAjaxRequest())
            {
                //GetWorkCenter(color, id);
            }

            return PartialView("~/Views/Shared/Partials/_WorkCenter.cshtml");
        }

        protected string GetPlantWeek()
        {
            return appService.GetPlantWeek();
        }

        [HttpPost]
        public JsonResult NetDemand([DataSourceRequest]DataSourceRequest request, NetDemand netDemand)
        {
            //string[] strSplit = sku.Split(new string[] { "," }, StringSplitOptions.None);

            var data = orderService.GetNetDemandTotal(netDemand);
            DataSourceResult result = new DataSourceResult();
            result.Data = data;
            return Json(result);
        }

        [HttpPost]
        public ActionResult ExportGetSummaryDetails(SummaryFilterModal SummaryView, String[] Planner, String[] WorkCenter, [DataSourceRequest] DataSourceRequest request, List<Kendo.Mvc.FilterDescriptor> FiltersData)
        {
            //Kendo.Mvc.FilterDescriptor Filters = JsonConvert.DeserializeObject<Kendo.Mvc.FilterDescriptor>(FiltersData);
            if (Planner != null)
                SummaryView.Planner = string.Join(",", Planner);
            if (WorkCenter != null)
                SummaryView.WorkCenter = string.Join(",", WorkCenter);

            List<SummaryResult> data = orderService.GetSummary(SummaryView);

            string[] excelColumns = new string[] { "planner_cd", "cut_alloc", "rule_number", "demand_source", "rev_no", "selling_style_cd", "selling_color_cd",
                                        "selling_attribute_cd", "size_short_desc", "TotalNetDemand", "Excess", "ExcessLot", "ExcessNetDemand", "LockOrReleaseBal",
                                        "Released", "Locked", "BuyOrders", "SugWK1", "SugWK2", "SugWK3Plus", "SpillOver", "SuggestedLotsComments"};

            string fileName = string.Format("Summary{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));

            return new ExcelResult(fileName).
                AddSheet<SummaryResult>(data, "Plan Order Summary", excelColumns);

        }

        public ActionResult ExportRequisitionDetails(String requisitionId)
        {
            var data = orderService.GetRequisitionDetail(new RequisitionDetail() { RequisitionId = requisitionId });

            string[] excelColumns = new string[] { "Style", "Description", "Color", "Attribute", "Size", "SizeLit" , "Rev", "Uom", "Quantity", "StdQuality", "Dpr", "PlanDate" };

            string fileName = string.Format("RequisitionDetails{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));

            return new ExcelResult(fileName).
                AddSheet<RequisitionDetail>(data, "Requisition Details", excelColumns);

        }

        [HttpPost]
        public ActionResult ExportNetDemandDetails(NetDemand netDemand)
        {
            var data = orderService.GetNetDemandTotal(netDemand);

            string[] excelColumns = new string[] { "plant", "cat", "rule_number", "priority_sequence", "qty", "NET_Demand", "Consumed" };

            string fileName = string.Format("NetDemand{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));

            return new ExcelResult(fileName).
                AddSheet<NetDemand>(data, "Net Demand", excelColumns);

        }

       
        [HttpPost]
        public JsonResult GetRequisitionDetail([DataSourceRequest]DataSourceRequest request, String RequisitionId)
        {
            var data = orderService.GetRequisitionDetail(new RequisitionDetail() {RequisitionId=RequisitionId });
            var result = data.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetRequisition(String RequisitionId)
        {
            //requisition.RequisitionId = "RQ29946";
            var data = orderService.GetRequisition(RequisitionId);
            return Json((data.Count>0)?data[0]:null, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetNewRequisition()
        {
            //requisition.RequisitionId = "RQ29946";
            var data = orderService.getNewRequisitiuonId();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetVendor([DataSourceRequest]DataSourceRequest request, VendorSearch vendorSearch)
        {
            var data = orderService.GetVendorSearch(vendorSearch);

            var result = data.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateRequsitionDetails([DataSourceRequest]DataSourceRequest request, [Bind(Prefix = "models")] IList<RequisitionDetail> resultData)
        {
            //resultData[0].StdCase = 3434343;
            var result = resultData.ToDataSourceResult(request);
            return Json(result);
        }

        [HttpPost]
        public JsonResult GetRetrieve(string Style, string BusinessCode)
        {
            var data = orderService.GetColorCode(Style);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetAORetrieve(string Style, string BusinessCode)
        {
            var data = orderService.GetAOColorCode(Style);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetValidateStyle(string Style)
        {
            var data = orderService.GetSOStyleDetail(Style);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetValidateWOStyle(string Style)
        {
            var data = orderService.GetWOStyleDetail(Style);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDcValidate(string DcLoc)
        {
            var data = orderService.GetDCValidate(DcLoc);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetMFGValidate(string MFGPathId)
        {
            var data = orderService.GetMFGValidate(MFGPathId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDcValidates(string DcLoc, string BusinessUnit)
        {
            var data = orderService.GetDCValidates(DcLoc, BusinessUnit);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDemandDvr(string PoNo, string LineNo)
        {
            var data = orderService.GetDemandDvr(PoNo, LineNo);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAttributeInfo(string styleCode, string colorCode)
        {
            var data = orderService.GetAttribute(styleCode, colorCode);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAOAttributeInfo(string styleCode, string colorCode)
        {
            var data = orderService.GetAOAttribute(styleCode, colorCode);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetSizeInfo(string styleCode, string colorCode, string attributeCode)
        {
            var data = orderService.GetSize(styleCode, colorCode, attributeCode);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult DeleteRequisition(String RequisitionId)
        {
            var currReqList = orderService.GetRequisition(RequisitionId);
            if (currReqList != null &&  currReqList.Count>0)
            {
                var currReq = currReqList[0];
                if (currReq.ProdStatus == LOVConstants.ProductionStatus.Locked
                    && currReq.ReqStatus == LOVConstants.RequestStatus.UnderConstruction
                    )
                {
                    if (orderService.DeleteRequisition(currReq))
                    {
                        return Json(new
                        {
                            Status = true,
                            ErrMsg = "Requisition deleted successfully. " + RequisitionId
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            Status = false,
                            ErrMsg = "Failed to delete requisition. " + RequisitionId
                        });
                    }
                }// status check
                else
                {
                    return Json(new
                    {
                        Status = false,
                        ErrMsg = "Not allowed to delete this requisition. " + RequisitionId
                    });
                }
            }
            return Json(new
            {
                Status = false,
                ErrMsg = "Invalid requisition detail. " + RequisitionId
            });
        }

        [HttpPost]
        public JsonResult GetRevisionAndUomInfo(string styleCode, string colorCode, string attributeCode, string sizeCode)
        {
            var data = orderService.GetRevisionAndUom(styleCode, colorCode, attributeCode, sizeCode);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetUomInfo([DataSourceRequest]DataSourceRequest request, string styleCode, string colorCode, string attributeCode, string sizeCode)
        {
            var data = orderService.GetUOM(styleCode, colorCode, attributeCode, sizeCode);
            var result = data.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetStdCaseInfo(string styleCode, string colorCode, string attributeCode, string sizeCode, string revCode)
        {
            var data = orderService.GetStdCaseQty(styleCode, colorCode, attributeCode, sizeCode, revCode);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetStdCaseDuplicate(string Style, string Color, string Attribute, string Size, string Rev)
        {
            var data = orderService.GetStdCaseQty(Style, Color, Attribute, Size, Rev);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetSkuColors(SkuViewModel searchCriteria)
        {
            var data = SkuData(searchCriteria.Style_Cd).Select(x => new { Color = x.ColorCode, ColorName = x.ColorCode }).Distinct();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSkuAttributes(SkuViewModel searchCriteria)
        {
            var data = orderService.GetAttribute(searchCriteria.Style_Cd, searchCriteria.Color_Cd).Select(x => new { Attribute = x.Attribute, AttributeDesc = x.AttributeDesc }).Distinct();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
       
        public JsonResult GetPKGCheckVal(SkuViewModel searchCriteria)
        {
            var data = orderService.GetPKGCheck(searchCriteria.Style_Cd, searchCriteria.Color_Cd, searchCriteria.Attribute_Cd, searchCriteria.SizeList, searchCriteria.Asrt_Cd, searchCriteria.Pak_Cd).Select(x => new { PKGStyle = x.PKGStyle, NewRevision = x.NewRevision }).Distinct();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSkuRevisionPkg(SkuViewModel searchCriteria)
        {
            var data = orderService.GetRevisionInLine(searchCriteria.Style_Cd, searchCriteria.Color_Cd, searchCriteria.Attribute_Cd, searchCriteria.SizeList, searchCriteria.Asrt_Cd, searchCriteria.Rev_Cd).Select(x => new { PKGStyle = x.PKGStyle }).Distinct();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSkuSizes(SkuViewModel searchCriteria)
        {
            var data = orderService.GetSize(searchCriteria.Style_Cd, searchCriteria.Color_Cd, searchCriteria.Attribute_Cd).Select(x => new { Size = x.Size, SizeDesc = x.SizeShortDes }).Distinct();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSkuSize(SkuViewModel searchCriteria)
        {
            var data = orderService.GetSiz(searchCriteria.Style_Cd, searchCriteria.Color_Cd, searchCriteria.Attribute_Cd).Select(x => new { Size = x.Size, SizeDesc = x.SizeShortDes, Qty = x.Qty, Rev = x.Rev }).Distinct();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSkuUom(SkuViewModel searchCriteria)
        {
            var data = orderService.GetUOM(searchCriteria.Style_Cd, searchCriteria.Color_Cd, searchCriteria.Attribute_Cd, searchCriteria.Size_Cd).Select(x => new { Uom = x.Uom, UomDesc = x.Uom });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMFGId(SkuViewModel searchCriteria)
        {
            var data = orderService.GetMFG(searchCriteria.Style_Cd, searchCriteria.Color_Cd, searchCriteria.Attribute_Cd, searchCriteria.SizeList, searchCriteria.DemLoc_Cd).Select(x => new { MfgPathId = x.MfgPathId, SewPltMfg = x.MfgPathId + "-" + x.SewPlt, SewPlt = x.SewPlt});
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult  GetRevisionNumbers(SkuViewModel searchCriteria)
        {
            var data = orderService.GetRevisionNumbers(searchCriteria.Style_Cd, searchCriteria.Color_Cd, searchCriteria.Attribute_Cd, searchCriteria.Size_Cd).Select(x => new { Rev = x.Rev, RevDesc = x.Rev });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
       

        public JsonResult GetDemandDriversDrpdn(SkuViewModel searchCriteria)
        {
            var DcResult = orderService.GetDemandDrivers(searchCriteria.Style_Cd, searchCriteria.Color_Cd, searchCriteria.Attribute_Cd, searchCriteria.Size_Cd, searchCriteria.Revision_Cd);

            return Json(DcResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDemandDriver([DataSourceRequest] DataSourceRequest request)
        {
            var planResult = orderService.GetDemandDriver();

            return Json(planResult.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetDemandDriver()
        //{
        //    var DcResult = orderService.GetDemandDriver();

        //    return Json(DcResult, JsonRequestBehavior.AllowGet);
        //}
        private IList<WorkOrderDetail> SkuData(string styleCode)
        {
            var result = orderService.GetColorCodes(styleCode).ToList();
            return result;
        }


        private IEnumerable<dynamic> GetColors()
        {
            var data = orderService.GetColorCode("000141");
            var result = data.Select(x => new { Color = "#C", ColorName = "#C" });
            return result.AsEnumerable<dynamic>();
        }

        #region Dummy Code - Added for demonstration

        public ActionResult GridWithForienKey()
        {
            ViewBag.Message = "Your GridWithForienKey page.";
            ViewData["Months"] = new List<dynamic>();
            return View();
        }

     
        

        #endregion
        [HttpPost]
        public JsonResult GetAttribute([DataSourceRequest]DataSourceRequest request, string Style, string BusinessCode, string Color)
        {

            if (orderService.GetStyleValidation(Style, BusinessCode))
            {
                return this.Json(new DataSourceResult
                {
                    Errors = "Style entered has to be Business Unit ‘BRA’"
                });
            }
            else
            {

                var data = orderService.GetAttribute(Style, Color);
                var result = data.ToDataSourceResult(request);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetSize([DataSourceRequest]DataSourceRequest request, string Style, string BusinessCode, string Color, string Attribute)
        {

            if (orderService.GetStyleValidation(Style, BusinessCode))
            {
                return this.Json(new DataSourceResult
                {
                    Errors = "Style entered has to be Business Unit ‘BRA’"
                });
            }
            else
            {

                var data = orderService.GetSize(Style, Color, Attribute);
                var result = data.ToDataSourceResult(request);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult GetRev([DataSourceRequest]DataSourceRequest request, string Style, string BusinessCode, string Color, string Attribute, string Size)
        {

            if (orderService.GetStyleValidation(Style, BusinessCode))
            {
                return this.Json(new DataSourceResult
                {
                    Errors = "Style entered has to be Business Unit ‘BRA’"
                });
            }
            else
            {

                var data = orderService.GetMaxRevision(Style, Color, Attribute, Size);
                var result = data.ToDataSourceResult(request);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult GetUOM([DataSourceRequest]DataSourceRequest request, string Style, string BusinessCode, string Color, string Attribute, string Size)
        {

            if (orderService.GetStyleValidation(Style, BusinessCode))
            {
                return this.Json(new DataSourceResult
                {
                    Errors = "Style entered has to be Business Unit ‘BRA’"
                });
            }
            else
            {

                var data = orderService.GetUOM(Style, Color, Attribute, Size);
                var result = data.ToDataSourceResult(request);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetSkuRevision(WorkOrderDetail searchCriteria)
        {
            //var data = orderService.GetRevisions(searchCriteria.Style_Cd, searchCriteria.Color_Cd, searchCriteria.Attribute_Cd, searchCriteria.SizeList, searchCriteria.Asrt_Cd).Select(x => new { Revision = x.NewRevision }).Distinct();
            var data = orderService.GetRevisions(searchCriteria.SellingStyle,
            searchCriteria.ColorCode,
            searchCriteria.Attribute,
            searchCriteria.SizeList,
            searchCriteria.AssortCode
            );
            if (data.Count() <= 0)
            {
                data = new List<WorkOrderDetail>();
                data.Add(new WorkOrderDetail { Revision = 0 });
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRevisionNumbersInline(WorkOrderDetail searchCriteria)
        {
            //var data = orderService.GetRevisionNumbers(searchCriteria.Style_Cd, searchCriteria.Color_Cd, searchCriteria.Attribute_Cd, searchCriteria.Size_Cd).Select(x => new { Rev = x.Rev, RevDesc = x.Rev });
            var data = orderService.GetRevisions(searchCriteria.SellingStyle,
            searchCriteria.ColorCode,
            searchCriteria.Attribute,
            searchCriteria.SizeList,
            searchCriteria.AssortCode
            );

            if (data.Count() <= 0)
            {
                data = new List<WorkOrderDetail>();
                data.Add(new WorkOrderDetail { Revision = 0, PKGStyle = "0" });
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult AttributeMrz(AttributionMrz search)
        {
            return View(search);
        }

        [HttpPost]
        public ContentResult AttributeMrz([DataSourceRequest]DataSourceRequest request, AttributionMrz search, String Src)
        {
            if (String.IsNullOrEmpty(Src))
            {
                return ISSJson(new List<AttributionMrz>().ToDataSourceResult(request));
            }
            var data = orderService.GetAttributeMrz(search.OrderId, search.Style, search.Color, search.Attribute, search.Size);

            var result = data.ToDataSourceResult(request);
            return ISSJson(result);
            
        }

        [HttpPost]
        public JsonResult DeleteAttriMrz(List<AttributionMrz> data)
        {
            try
            {
                String uName = GetCurrentUserName();
                data.ForEach(item =>
                {
                    if (orderService.DeleteAttributeMrzData(item, uName))
                    {
                         Json(new { Status = true });
                    }
                    else
                    {
                         Json(new { Status = false });
                    }
                });
            }
            catch (Exception ee)
            {
                return Json(new
                {
                    Status = false
                });
            }

            return Json(false);
        }       

    }
}