using ISS.Common;
using ISS.Core.Model.Capacity;
using ISS.Core.Model.Order;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Web.Mvc;
using System.Linq;
using ISS.Core.Model.Common;
using ISS.Core.Model.Order;

namespace ISS.Web.Controllers
{
    public class SharedController : BaseController
    {
        private readonly ISS.BusinessRules.Contract.Common.IApplicationService appService;

        #region ctor
        public SharedController(ISS.BusinessRules.Contract.Common.IApplicationService applicationService)
        {
            appService = applicationService;
        }
        #endregion

        #region Common Search


        public PartialViewResult CommonSearch(ISS.Core.Model.Common.CommonSearch search)
        {
            return PartialView(search);
        }

         #endregion
       
        #region Get Planner
        public JsonResult GetPlanner([DataSourceRequest] DataSourceRequest request)
        {
            var plannerResult = appService.GetPlannerList();

            return Json(plannerResult.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get Corp Division
        public JsonResult GetCorpDivision([DataSourceRequest] DataSourceRequest request)
        {
            var plannerResult = appService.GetCorpDivisionList();

            return Json(plannerResult.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get HAA
        public ValidateHAAO ValidateHaaOrNot(string Style, string Color, string Attribute, string Size)
        {
            var HAA = appService.ExternalSku(Style, Color, Attribute, Size);
            return HAA;
        }
        #endregion

        #region Get demand policy
        public string ValidateDemandPolicy(string Style, string Color, string Attribute, string Size)
        {
            var HAA = appService.GetDemandPolicy(Style, Color, Attribute, Size);
            return HAA;
        }
        #endregion

        #region Get Planning Contact
        public JsonResult GetPlanningContact([DataSourceRequest] DataSourceRequest request)
        {
            var planResult = appService.GetPlanningContactList();

            return Json(planResult.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get Business Unit
        public JsonResult GetBusinessContact([DataSourceRequest] DataSourceRequest request)
        {
            var planResult = appService.GetBusinessContactList();

            return Json(planResult.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get MFg path Unit
        public JsonResult GetMFGPath([DataSourceRequest] DataSourceRequest request)
        {
            var planResult = appService.GetMFGPathList();

            return Json(planResult.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get Season 
        public JsonResult GetSeason([DataSourceRequest] DataSourceRequest request, string BusinessData)
        {
            var tt = BusinessData;
            var seaResult = appService.GetSeasonList(BusinessData);

            return Json(seaResult.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get Source Contact
        public JsonResult GetSourceContact([DataSourceRequest] DataSourceRequest request)
        {
            var planResult = appService.GetSourceContactList();

            return Json(planResult.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get Approver 
        public JsonResult GetApprover([DataSourceRequest] DataSourceRequest request)
        {
            var planResult = appService.GetReqApproverList();

            return Json(planResult.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get Program Type
        public JsonResult GetProgramType([DataSourceRequest] DataSourceRequest request)
        {
            var planResult = appService.GetProgramTypeList();

            return Json(planResult.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get Mode
        public JsonResult GetMode([DataSourceRequest] DataSourceRequest request)
        {
            var planResult = appService.GetModeList();

            return Json(planResult.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Get Work Center
        public JsonResult GetWorkCenter(SummaryFilterModal SummaryView)
        {
           
            var workCenterResult = appService.GetWorkCenterList((SummaryView.CapacityGroup == null ? CapacityGroup.Cut.ToString() : SummaryView.CapacityGroup), SummaryView.Planner);

            return Json(workCenterResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAttributedWorkCenter(SummaryFilterModal SummaryView)
        {

            var workCenterResult = appService.GetAttributedWorkCenterList((SummaryView.CapacityGroup == null ? CapacityGroup.Cut.ToString() : SummaryView.CapacityGroup), SummaryView.Planner);

            return Json(workCenterResult, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get Allocation Work Center
        public JsonResult GetAllocationWorkCenter(CapacitySearch searchView)
        {
            var workCenterResult = appService.GetAllocationWorkCenterList(searchView.CapacityGroup, searchView.Plant);

            return Json(workCenterResult, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get Plant
        public JsonResult GetPlant(CapacitySearch searchView)
        {
            var plantResult = appService.GetPlantList(searchView.CapacityGroup);

            return Json(plantResult, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get Textile Group
        public JsonResult GetTextileGroup([DataSourceRequest] DataSourceRequest request, string BusinessUnit)
        {
            var textileResult = appService.GetTextileGroup(BusinessUnit);
//textileResult.ToDataSourceResult(request)
            return Json(textileResult, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get Plan Week Year
        public JsonResult GetPlanWeekYear([DataSourceRequest] DataSourceRequest result)
        {
            var planResult = appService.GetPlanWeekYear();

            var weekresult = planResult.GroupBy(p=> new {p.Fiscal_Week, p.Fiscal_Year})
                                       .Select(t => new SelectListItem
                                                           {
                                                               Text = t.Key.Fiscal_Week + " / " + t.Key.Fiscal_Year,
                                                               Value = t.Key.Fiscal_Week + " / " + t.Key.Fiscal_Year
                                                           });

            return Json(weekresult.ToDataSourceResult(result), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get Attribute
        public JsonResult GetAttribute([DataSourceRequest] DataSourceRequest request, SummaryFilterModal model)
        {
            var attributeResult = appService.GetAttributeList(model);

            return Json(attributeResult.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
           
        }
        #endregion
    }
}