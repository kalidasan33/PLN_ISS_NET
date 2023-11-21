using ISS.Core.Model.Common;
using System;
using System.Collections.Generic;
using ISS.Core.Model.Order;
namespace ISS.BusinessRules.Contract.Common
{
    public interface IApplicationService
    {
        IList<SummaryFilterModal> GetAttributeList(SummaryFilterModal model);
        IList<Planner> GetPlannerList();
        IList<Planner> GetPlanningContactList();
        IList<Planner> GetBusinessContactList();
        IList<Planner> GetMFGPathList();
        IList<Planner> GetSeasonList(String BU);
        IList<Planner> GetSourceContactList();
        IList<Planner> GetReqApproverList();
        IList<Planner> GetProgramTypeList();
        IList<Planner> GetModeList();
        IList<Planner> GetPlannerListFull();
        string GetPlantWeek();
        IList<PlanWeek> GetPlanBeginEndDates();
        IList<Planner> GetPlannerNameAndCode();

        IList<WorkCenter> GetWorkCenterList(string capacityGroup, string planner);

        IList<PlantInfo> GetPlantList(string capcityGroup);

        IList<WorkCenter> GetAllocationWorkCenterList(string capcityGroup, string plant);

        IList<FabricInfo> GetTextileGroup(string fabrics);

        IList<PlanWeek> GetPlanWeekYear();

        IList<PlanWeek> GetPlanWeekYearBeginDate();
        IList<WorkCenter> GetAttributedWorkCenterList(string capacityGroup, string planner);

        bool ISSAvailable();
        ValidateHAAO ExternalSku(string Style, string Color, string Attribute, string Size);
        string GetDemandPolicy(string Style, string Color, string Attribute, string Size);
        IList<Planner> GetCorpDivisionList();
    }
    public interface ICachingApplicationService : IApplicationService
    {

    }
}
