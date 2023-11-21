using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using ISS.BusinessRules.Contract.Information;
using ISS.Core.Model.Information;
using Kendo.Mvc.UI;
using System.Text;


namespace ISS.Web.Controllers
{
    public class InformationController : BaseController
    {

        private readonly IInformationService InfoService;
        private readonly ISS.BusinessRules.Contract.Common.IApplicationService appService;

        public InformationController(IInformationService informationService, ISS.BusinessRules.Contract.Common.IApplicationService _appService)
        {
            appService = _appService;
            InfoService = informationService;
        }

        [HttpGet]
        public ActionResult Releases(ReleasesSearch search)
        {
            return View(search);
        }

        [HttpPost]
        public ContentResult Releases([DataSourceRequest]DataSourceRequest request, ReleasesSearch search, String Src)
        {
            if (String.IsNullOrEmpty(Src))
            {
                return ISSJson(new List<Releases>().ToDataSourceResult(request));
            }
            var data = InfoService.GetReleases(search);
            var result = data.ToDataSourceResult(request);
            return ISSJson(result);
        }


        public JsonResult ExceptionDetails(string superOrder)
        {
            var data = InfoService.GetAS400Exceptions(superOrder);
            DataSourceResult result = new DataSourceResult();
            result.Data = data;
            return Json(result);
        }

        [HttpGet]
        public ActionResult SuggestedExceptions(ReleasesSearch search, bool? autoLoad)
        {
            ViewBag.autoLoad = autoLoad.HasValue ? autoLoad.Value : false;
            return View(search);
        }



        [HttpPost]
        public ContentResult SuggestedExceptions([DataSourceRequest]DataSourceRequest request, ReleasesSearch search, String Src)
        {
            if (String.IsNullOrEmpty(Src))
            {
                return ISSJson(new List<SuggestedException>().ToDataSourceResult(request));
            }
            var data = InfoService.GetSuggestedExceptions(search);
            var result = data.ToDataSourceResult(request);
            return ISSJson(result);
        }


        [HttpPost]
        public JsonResult SuggestedExceptionDetail([DataSourceRequest]DataSourceRequest request, SuggestedExceptionDetial search)
        {
            var data = InfoService.GetSuggestedExceptionDetail(search);
            string Str = "";
            if (data.Count > 0)
            {
                StringBuilder stb = new StringBuilder();
                for (int i = 0; i < data.Count; i++)
                {
                    stb.Append("Path Id: " + data[i].ConflictPath + "," + "  " + "Conflict : " + data[i].ConflictReason + "<br/>");
                }
                Str = "Conflicts for  : " + search.Style + " " + search.Color + " " + search.Atribute + " " + search.SizeShortDesc + " <br/>" + stb.ToString();
            }
            else
                Str = "Conflicts for  : " + search.Style + " " + search.Color + " " + search.Atribute + " " + search.SizeShortDesc + " <br/>" + "Path Id :"+ " , " + "Conflict :";
            return Json(Str);
        }

        [HttpGet]
        public ActionResult DCWorkOrders(DCWorkOrderSearch search)
        {
            if (search == null) search = new DCWorkOrderSearch();
            search.hasRemarks = true;
            return View(search);
        }

        [HttpPost]
        public JsonResult DCWorkOrders([DataSourceRequest]DataSourceRequest request, DCWorkOrderSearch search, String Src)
        {
            if (String.IsNullOrEmpty(Src))
            {
                return this.Json(null, JsonRequestBehavior.AllowGet);
            }
            var data = InfoService.GetDCWorkOrders(search);
            var resultSet = data.ToDataSourceResult(request);
            return this.Json(resultSet, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult StyleException(StyleSearch search)
        {
            search.APS = search.AVYX = search.ISS = search.NET = search.CWC  = search.MTLA = true;
            return View(search);
        }

        [HttpPost]
        public ContentResult StyleException([DataSourceRequest]DataSourceRequest request, StyleSearch search, String Src)
        {
            if (String.IsNullOrEmpty(Src))
            {
                return ISSJson(new List<StyleException>().ToDataSourceResult(request));
            }

            var data = InfoService.GetStyleExceptions(search);
            var resultSet = data.ToDataSourceResult(request);
            return ISSJson(resultSet);
        }

        [HttpGet]
        public ActionResult StyleWOTextileGroup(StyleSearch search)
        {
            return View(search);
        }

        [HttpPost]
        public JsonResult StyleWOTextileGroup([DataSourceRequest]DataSourceRequest request, StyleSearch search, String Src)
        {
            if (String.IsNullOrEmpty(Src))
            {
                return this.Json(null, JsonRequestBehavior.AllowGet);
            }
            var data = InfoService.GetWOTextileGroup(search);
            var resultSet = data.ToDataSourceResult(request);
            return this.Json(resultSet, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult BlownAwayLots(StyleSearch search)
        {
            return View(search);
        }

        [HttpPost]
        public JsonResult BlownAwayLots([DataSourceRequest]DataSourceRequest request, StyleSearch search, String Src)
        {
            if (String.IsNullOrEmpty(Src))
            {
                return this.Json(null, JsonRequestBehavior.AllowGet);
            }
            var data = InfoService.GetBlownAwayLots(search);
            var resultSet = data.ToDataSourceResult(request);
            return this.Json(resultSet, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult BulksToAvyx(BulksToAvyx search)
        {
            //DateTime today = DateTime.Today;
            //var bulkSearch = new BulksToAvyx();
            //bulkSearch.FromDate = StartOfWeek(DateTime.Now, DayOfWeek.Sunday);
            //bulkSearch.ToDate = today;
            return View(search);
        }

        [HttpPost]
        public ActionResult bulksToAvyx([DataSourceRequest]DataSourceRequest request, string hdExtractType, String FromDate, String ToDate)
        {
            string bulkStatus = hdExtractType;
            IList<BulksToAvyx> data;
            if (bulkStatus == "Active")
            {
                data = InfoService.GetBulksToAvyx(FromDate, ToDate);
            }
            else if (bulkStatus == "Completed")
            {
                data = InfoService.GetBulksToComplete(FromDate, ToDate);
            }
            else if (bulkStatus == "Error")
            {
                data = InfoService.GetBulksToError(FromDate, ToDate);
            }
            else
            {
                data = InfoService.GetBulksNoData(FromDate, ToDate);
            }
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var result = new ContentResult();
            serializer.MaxJsonLength = Int32.MaxValue;
            result.Content = serializer.Serialize(data.ToDataSourceResult(request));
            result.ContentType = "application/json";
            return result;
        }

        

        [HttpGet]
        public ActionResult bulkstoOneSource(BulkstoOneSource search)
        {
            DateTime today = DateTime.Today; 
            var bulkSearch = new BulkstoOneSource();
            bulkSearch.FromDate = StartOfWeek(DateTime.Now, DayOfWeek.Sunday);
            bulkSearch.ToDate = today;
            return View(bulkSearch);

        }

        [HttpPost]
        public ActionResult bulkstoOneSource([DataSourceRequest]DataSourceRequest request, string hdExtractType, String FromDate, String ToDate)
        {
            IList<BulkstoOneSource> data;
            string bulkStatus = hdExtractType;
            if (bulkStatus == "Pulled")
            {
                data = InfoService.GetBulksToPulled(FromDate, ToDate);
            }
            else if (bulkStatus == "Success")
            {
                data = InfoService.GetBulksToSuccess(FromDate, ToDate);
            }
            else if (bulkStatus == "ErrorOS")
            {
                data = InfoService.GetBulksToErrorOS(FromDate, ToDate);
            }
            else if (bulkStatus == "ErrorOSSecond")
            {
                data = InfoService.GetBulksToErrorOSSecond(FromDate, ToDate);
            }
            else
            {
                data = InfoService.GetBulksOSNoData(FromDate, ToDate);
            }
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var result = new ContentResult();
            serializer.MaxJsonLength = Int32.MaxValue;
            result.Content = serializer.Serialize(data.ToDataSourceResult(request));
            result.ContentType = "application/json";
            return result;
        }

        [HttpGet]
        public ActionResult KnightsApparelExpedite(KnightsApparelExpedite search)
        {
            //DateTime today = DateTime.Today;
            //var KnightsApparelSearch = new KnightsApparelExpedite();
            //KnightsApparelSearch.FromDate = StartOfWeek(DateTime.Now, DayOfWeek.Sunday);
            //KnightsApparelSearch.ToDate = today;
            //return View(KnightsApparelSearch);
            return View(search);
        }

        [HttpPost]
        public ActionResult KnightsApparelExpedite([DataSourceRequest]DataSourceRequest request, string hdExtractType, String FromDate, String ToDate, String StyleCode, String ColorCode, String AttributeCode, String SizeCode)
        {
            IList<KnightsApparelExpedite> data;
            string bulkStatus = hdExtractType;
            data = InfoService.GetKnightsApparelExpedite(FromDate, ToDate, StyleCode, ColorCode, AttributeCode, SizeCode);
           
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var result = new ContentResult();
            serializer.MaxJsonLength = Int32.MaxValue;
            result.Content = serializer.Serialize(data.ToDataSourceResult(request));
            result.ContentType = "application/json";
            return result;
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
        public JsonResult GetBulksActiveCount(BulksToAvyx cs, String FromDate, String ToDate)
        {
            var BulkId = InfoService.GetBulksActiveCount(FromDate, ToDate);
            var CompleteId = InfoService.GetCompleteCount(FromDate, ToDate);
            var ErrorId = InfoService.GetErrorCount(FromDate, ToDate);
            string bulk = BulkId;
            string complete = CompleteId;
            string error = ErrorId;
            return Json(new { bulk, complete, error });
        }

        [HttpPost]
        public JsonResult GetBulksOneSourceCount(String FromDate, String ToDate)
        {
            var PulledId = InfoService.GetBulksPulledCount(FromDate, ToDate);
            var SuccessId = InfoService.GetBulksSuccessfulCount(FromDate, ToDate);
            var ErrorId = InfoService.GetErrorosCount(FromDate, ToDate);
            var ErrorSecId = InfoService.GetErrorosSecondCount(FromDate, ToDate);
            string pull = PulledId;
            string success = SuccessId;
            string error = ErrorId;
            string errorSec = ErrorSecId;
            return Json(new { pull, success, error, errorSec });
        }

        [HttpPost]
        public ActionResult ExportBlownAwayLot(StyleSearch search)
        {
            var data = InfoService.GetBlownAwayLots(search);

            string[] excelColumns = new string[] { "StyleCode", "ColorCode", "AttributeCode", "SizeShortDesc", "Plant", "LotQuantityStr", "LotId",
                                        "Planner", "Reason"};

            string fileName = string.Format("BlownAwayLot{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));

            return new ISS.Web.Helpers.ExcelResult(fileName).
                AddSheet<BlownAwayLot>(data, "Blown Away Lot", excelColumns);

        }

        [HttpPost]
        public ActionResult ExportBulkstoAvyx(string hdExtractType, String FromDate, String ToDate)
        {
            string bulkStatus = hdExtractType;
            string[] excelColumns;
            IList<BulksToAvyx> data;
            if (bulkStatus == "Active")
            {
                data = InfoService.GetBulksToAvyx(FromDate, ToDate);

                excelColumns = new string[] { "StyleCode", "ColorCode", "AttributeCode", "SizeShortDesc", "ApsStyleCode", "ApsColorCode", "ApsAttributeCode",
                                        "ApsSizeShortDesc", "DemandWeekendDate", "CurrOrderQty", "CorpBusinessUnit", "DemandSource", "ProcessedToAvyx", "CreateDate", "ReActivatedDate", "ReActivatedBy"};

                string fileName = string.Format("BulksToAvyx{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));

                return new ISS.Web.Helpers.ExcelResult(fileName).
                    AddSheet<BulksToAvyx>(data, "Bulks To Avyx", excelColumns);
            }
            else if (bulkStatus == "Completed")
            {
                data = InfoService.GetBulksToComplete(FromDate, ToDate);

                excelColumns = new string[] { "StyleCode", "ColorCode", "AttributeCode", "SizeShortDesc", "ApsStyleCode", "ApsColorCode", "ApsAttributeCode",
                                        "ApsSizeShortDesc", "DemandWeekendDate", "CurrOrderQty", "CorpBusinessUnit", "DemandSource", "ProcessedToAvyx", "CreateDate", "CompletedDate", "CompletedBy"};

                string fileName = string.Format("BulksToComplete{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));

                return new ISS.Web.Helpers.ExcelResult(fileName).
                    AddSheet<BulksToAvyx>(data, "Bulks To Complete", excelColumns);
            }
            else if (bulkStatus == "Error")
            {
                data = InfoService.GetBulksToError(FromDate, ToDate);

                excelColumns = new string[] { "StyleCode", "ColorCode", "AttributeCode", "SizeShortDesc", "ApsStyleCode", "ApsColorCode", "ApsAttributeCode",
                                        "ApsSizeShortDesc", "DemandWeekendDate", "CurrOrderQty", "CorpBusinessUnit", "DemandSource", "ProcessedToAvyx", "CreateDate", "ErrorMsg"};

                string fileName = string.Format("BulksToError{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));

                return new ISS.Web.Helpers.ExcelResult(fileName).
                    AddSheet<BulksToAvyx>(data, "Bulks To Error", excelColumns);
            }

            else
            {
                data = InfoService.GetBulksNoData(FromDate, ToDate);

                excelColumns = new string[] { "StyleCode", "ColorCode", "AttributeCode", "SizeShortDesc", "ApsStyleCode", "ApsColorCode", "ApsAttributeCode",
                                        "ApsSizeShortDesc", "DemandWeekendDate", "CurrOrderQty", "CorpBusinessUnit", "DemandSource", "ProcessedToAvyx", "CreateDate", "ReActivatedDate", "ReActivatedBy"};

                string fileName = string.Format("BulksToNoData{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));

                return new ISS.Web.Helpers.ExcelResult(fileName).
                    AddSheet<BulksToAvyx>(data, "Bulks No Data", excelColumns);
            }
        }
        //
        [HttpPost]
        public ActionResult ExportKnightsApparelExpedite(string hdExtractType, String FromDate, String ToDate, String StyleCode, String ColorCode, String AttributeCode, String SizeCode)
        {
            IList<KnightsApparelExpedite> data;
            string[] excelColumns;
            data = InfoService.GetKnightsApparelExpedite(FromDate, ToDate, StyleCode, ColorCode, AttributeCode, SizeCode);

            excelColumns = new string[] { "BulkNumber", "StyleCode", "ColorCode", "AttributeCode", "SizeCode", "DemandDate", "SizeShortDesc", "GrossRequirement", "InTransitToDC", "Packing", "IssuedToWIP", "OnSite", "NotPlanned" };

            string fileName = string.Format("KnightsApparelExpedite{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));

            return new ISS.Web.Helpers.ExcelResult(fileName).
                AddSheet<KnightsApparelExpedite>(data, "Knights Apparel Expedite", excelColumns);
        }
        [HttpPost]
        public ActionResult ExportBulkstoOneSource(string hdExtractType, String FromDate, String ToDate)
        {
            
            string bulkStatus = hdExtractType;
            string[] excelColumns;
            IList<BulkstoOneSource> data;
            if (bulkStatus == "Pulled")
            {
                data = InfoService.GetBulksToPulled(FromDate, ToDate);

                excelColumns = new string[] { "BulkNumber", "LineNumber", "StyleCode", "ColorCode", "AttributeCode", "SizeShortDesc", "CurrDueDate", "ContactPlannerCd", 
                    "SrcContactCd", "DemandLoc", "CorpBusinessUnit", "MfgPathId", "MfgRevisionNo", "CurrOrderQty", "PlantCd", "ProcessedToOs", "OrgnCreateDate" };

                string fileName = string.Format("BulksToPulled{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));

                return new ISS.Web.Helpers.ExcelResult(fileName).
                    AddSheet<BulkstoOneSource>(data, "Bulks To Pulled", excelColumns);
            }
            else if (bulkStatus == "Success")
            {
                data = data = InfoService.GetBulksToSuccess(FromDate, ToDate);

                excelColumns = new string[] { "BulkNumber", "LineNumber", "StyleCode", "ColorCode", "AttributeCode", "SizeShortDesc", "RequisitionId", "ReqsnCreateDate", "ApproveDate", "ReqsnStatus", "CurrOrderQty", 
                    "ParentStyle", "CompStyle", "CompColor", "CompAttribute", "CompSize", "CorpBusinessUnit", "ExternalStyle", "ExternalAttribute", "ExternalSize", "ExternalVersion", "ExternalLogo", "Graphic", "Placement" };

                string fileName = string.Format("BulksToSuccess{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));

                return new ISS.Web.Helpers.ExcelResult(fileName).
                    AddSheet<BulkstoOneSource>(data, "Bulks To Success", excelColumns);
            }
            else if (bulkStatus == "ErrorOS")
            {
                data = InfoService.GetBulksToErrorOS(FromDate, ToDate);

                excelColumns = new string[] { "BulkNumber", "LineNumber", "StyleCode", "ColorCode", "AttributeCode", "SizeShortDesc", "RequisitionId", "ReqsnCreateDate", "ApproveDate", "ReqsnStatus", "CurrOrderQty", 
                    "ParentStyle", "CompStyle", "CompColor", "CompAttribute", "CompSize", "CorpBusinessUnit", "ExternalStyle", "ExternalAttribute", "ExternalSize", "ExternalVersion", "ExternalLogo", "Graphic", "Placement" };

                string fileName = string.Format("BulksToError{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));

                return new ISS.Web.Helpers.ExcelResult(fileName).
                    AddSheet<BulkstoOneSource>(data, "Bulks To Error", excelColumns);
            }
            else if (bulkStatus == "ErrorOSSecond")
            {
                data = InfoService.GetBulksToErrorOSSecond(FromDate, ToDate);

                excelColumns = new string[] { "BulkNumber", "LineNumber", "StyleCode", "ColorCode", "AttributeCode", "SizeShortDesc", "CurrDueDate", "CreateDate", "ContactPlannerCd", "SrcContactCd", "DemandLoc", "CorpBusinessUnit", "MfgPathId", "MfgRevisionNo", "CurrOrderQty", "PlantCd", "ProcessedToOs", "ErrMessage" };

                string fileName = string.Format("BulksToErrorSecond{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));

                return new ISS.Web.Helpers.ExcelResult(fileName).
                    AddSheet<BulkstoOneSource>(data, "Bulks To Error2nd", excelColumns);
            }

            else
            {
                data = InfoService.GetBulksOSNoData(FromDate, ToDate);

                excelColumns = new string[] { "BulkNumber", "LineNumber", "StyleCode", "ColorCode", "AttributeCode", "SizeShortDesc", "CurrDueDate", "ContactPlannerCd", 
                    "SrcContactCd", "DemandLoc", "CorpBusinessUnit", "MfgPathId", "MfgRevisionNo", "CurrOrderQty", "PlantCd", "ProcessedToOs", "OrgnCreateDate" };

                string fileName = string.Format("BulksToNoData{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));

                return new ISS.Web.Helpers.ExcelResult(fileName).
                    AddSheet<BulkstoOneSource>(data, "Bulks No Data", excelColumns);
            }
        }

        [HttpPost]
        public ActionResult ExportDCWorkOrders(DCWorkOrderSearch search)
        {

            var data = InfoService.GetDCWorkOrders(search);

            string[] excelColumns = new string[] { "CreatedDate", "Plant", "RequestNumber", "projectNumber", "FromStyle", "FromColor", "FromStyleAttribute",
                                        "FromSizeCd", "ToStyle", "ToColor", "ToStyleAttribute", "ToSizeCd", "OriginalDozens", "CompleteDozens", "PendingDozens", 
                                        "ExpectedDate", "Remarks"};

            string fileName = string.Format("DCWorkOrders{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));

            return new ISS.Web.Helpers.ExcelResult(fileName).
                AddSheet<DCWorkOrder>(data, "DC Work Orders", excelColumns);

        }

        [HttpPost]
        public ActionResult ExportStyleExceptions(StyleSearch search)
        {

            var data = InfoService.GetStyleExceptions(search);

            string[] excelColumns = new string[] { "StyleCode", "ColorCode", "AttributeCode", "SizeShortDesc", "PrimaryDC", "LOB", "Planner",
                                        "WorkCenter", "DemandStr", "MFGPath", "ProductFamily", "Reason"};

            string fileName = string.Format("StyleExceptions{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));

            return new ISS.Web.Helpers.ExcelResult(fileName).
                AddSheet<StyleException>(data, "Style Exceptions", excelColumns);

        }


        [HttpPost]
        public ActionResult ExportSuggestedExceptions(ReleasesSearch search)
        {
            var data = InfoService.GetSuggestedExceptions(search);

            string[] excelColumns = new string[] { "Style", "Color", "Atribute", "SizeShortDesc", "DmdLoc", "MfgPath", "Revision", "Reason", "ConflictSKU" };

            string fileName = string.Format("SuggestedExceptions{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));

            return new ISS.Web.Helpers.ExcelResult(fileName).
                AddSheet<SuggestedException>(data, "Suggested Exceptions", excelColumns);

        }

        [HttpPost]
        public ActionResult ExportStyleWOTextileGroup(StyleSearch search)
        {

            var data = InfoService.GetWOTextileGroup(search);

            string[] excelColumns = new string[] { "StyleCode", "TextileGroup", "Reason", "UserId", "CreatedDate", "UpdatedDate" };

            string fileName = string.Format("StyleWOTextileGroup{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));

            return new ISS.Web.Helpers.ExcelResult(fileName).
                AddSheet<WOTextileGroup>(data, "Style Without Textile Group", excelColumns);

        }

        [HttpPost]
        public ActionResult ExportReleases(ReleasesSearch search)
        {

            var data = InfoService.GetReleases(search);

            string[] excelColumns = new string[] { "UpdatedDate", "OrderId", "SellingStyle", "StyleCode", "ColorCode", "AttributeCode",
                                                    "SizeShortDesc", "DCloc", "SewPlant", "CutPlant", "textilePlant", "TotalCurrentOrderQuantity",
                                                    "RemoteUpdateCode", "Reason", "MultiSKU", "SuperOrder", "CuttingAlt", "FabricLbs",
                                                    "GreigeLbs", "CreateBD", "DzOnly"};

            string fileName = string.Format("Releases{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));

            return new ISS.Web.Helpers.ExcelResult(fileName).
                AddSheet<Releases>(data, "Releases", excelColumns);

        }


        public JsonResult ReleaseGetWorkCenter()
        {
            var workCenterResult = appService.GetWorkCenterList("Sew", null);

            return Json(workCenterResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReleaseGetPlanner()
        {
            var plannerResult = appService.GetPlannerList();

            return Json(plannerResult, JsonRequestBehavior.AllowGet);
        }
    }
}
