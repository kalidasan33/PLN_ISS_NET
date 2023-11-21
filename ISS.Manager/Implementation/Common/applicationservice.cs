using ISS.BusinessRules.Contract.Common;
using ISS.Core.Model.Common;
using ISS.Repository.Common;
using System;
using System.Collections.Generic;
using ISS.Core.Model.Order;

namespace ISS.BusinessService.Implementation.Common
{
    public class ApplicationService : IApplicationService
    {
        public ApplicationService()
        {
           
        }

        public IList<SummaryFilterModal> GetAttributeList(SummaryFilterModal model)
        {
            var searchRepository = new ApplicationRepository();
            var result = searchRepository.GetAttributeList(model);
            return result;

        }

        public IList<Planner> GetPlannerList()
        {
            var searchRepository = new ApplicationRepository();
            var result = searchRepository.GetPlannerList();
            return result;
        }
        
        public IList<Planner> GetPlanningContactList()
        {
            var searchRepository = new ApplicationRepository();
            var result = searchRepository.GetPlanningContactList();
            return result;
        }

        public IList<Planner> GetBusinessUnitList()
        {
            var searchRepository = new ApplicationRepository();
            var result = searchRepository.GetBusinesUnitList();
            return result;
        }
        public string GetPlantWeek()
        {
            var searchRepository = new ApplicationRepository();
            var result = searchRepository.GetPlantWeek();
            return result;
        }

        public IList<WorkCenter> GetWorkCenterList(string capacityGroup, string planner)
        {
            var searchRepository = new ApplicationRepository();
            var result = searchRepository.GetWorkCenterList(capacityGroup, planner);
            return result;
        }
        public IList<WorkCenter> GetAttributedWorkCenterList(string capacityGroup, string planner)
        {
            var searchRepository = new ApplicationRepository();
            var result = searchRepository.GetAttributedWorkCenterList(capacityGroup, planner);
            return result;
        }
        public IList<Planner> GetBusinessContactList()
        {
            var searchRepository = new ApplicationRepository();
            var result = searchRepository.GetBusinesUnitList();
            return result;
        }


        public IList<Planner> GetMFGPathList()
        {
            var searchRepository = new ApplicationRepository();
            var result = searchRepository.GetMFGPathList();
            return result;
        }

        public IList<Planner> GetSeasonList(String BU)
        {
            var searchRepository = new ApplicationRepository();
            var result = searchRepository.GetSeasonList(BU);
            return result;
        }

        public IList<Planner> GetSourceContactList()
        {
            var searchRepository = new ApplicationRepository();
            var result = searchRepository.GetSourceContactList();
            return result;
        }
        public IList<Planner> GetReqApproverList()
        {
            var searchRepository = new ApplicationRepository();
            var result = searchRepository.GetReqApproverList();
            return result;
        }

        public IList<Planner> GetProgramTypeList()
        {
            var searchRepository = new ApplicationRepository();
            var result = searchRepository.GetProgramTypeList();
            return result;
        }

        public IList<Planner> GetModeList()
        {
            var searchRepository = new ApplicationRepository();
            var result = searchRepository.GetModeList();
            return result;
        }
        public IList<PlanWeek> GetPlanBeginEndDates()
        {
            var searchRepository = new ApplicationRepository();
            var result = searchRepository.GetPlanBeginEndDates();
            return result;
        }
        public IList<Planner> GetPlannerNameAndCode()
        {
            var searchRepository = new ApplicationRepository();
            var result = searchRepository.GetPlannerNameAndCode();
            return result;
        }
		public IList<PlantInfo> GetPlantList(string capcityGroup)
        {
            var searchRepository = new ApplicationRepository();
            var result = searchRepository.GetPlantList(capcityGroup);
            return result;
        }

        public IList<WorkCenter> GetAllocationWorkCenterList(string capcityGroup, string plant)
        {
            var searchRepository = new ApplicationRepository();
            return searchRepository.GetAllocationWorkCenterList(capcityGroup, plant);            
        }
       
        public IList<FabricInfo> GetTextileGroup(string fabric)
        {
            var searchRepository = new ApplicationRepository();
             return searchRepository.GetTextileGroup(fabric);            
        }

        public IList<PlanWeek> GetPlanWeekYear()
        {
            var searchRepository = new ApplicationRepository();
            return searchRepository.GetPlanWeekYear(); 
        }

        public IList<PlanWeek> GetPlanWeekYearBeginDate()
        {
            var searchRepository = new ApplicationRepository();
            return searchRepository.GetPlanWeekYearBeginDate(); 
        }

        public bool ISSAvailable()
        {
            var searchRepository = new ApplicationRepository();
            return searchRepository.ISSAvailable();
        }
        public IList<Planner> GetPlannerListFull()
        {
            var searchRepository = new ApplicationRepository();
            var result = searchRepository.GetPlannerListFull();
            return result;
        }
        public IList<Planner> GetCorpDivisionList()
        {
            var searchRepository = new ApplicationRepository();
            var result = searchRepository.GetCorpDivisionList();
            return result;
        }

        public ValidateHAAO ExternalSku(string Style, string Color, string Attribute, string Size)
        {
            var searchRepository = new ApplicationRepository();
            return searchRepository.ExternalSku(Style, Color, Attribute, Size);
        }


        public string GetDemandPolicy(string Style, string Color, string Attribute, string Size)
        {
            var searchRepository = new ApplicationRepository();
            return searchRepository.GetDemandPolicy(Style, Color, Attribute, Size);
        }
    }
}
