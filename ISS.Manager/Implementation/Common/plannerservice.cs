using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISS.Core.Model.Common;
using ISS.BusinessService.Contract.Common;
using ISS.Repository.Common;

namespace ISS.BusinessService.Implementation.Common
{
    public class PlannerService : IPlannerService
    {
        public string CurrentDataBaseKey { get; set; }

        public PlannerService()
        {
            ISS.BusinessService.Implementation.MetaDataService met = new ISS.BusinessService.Implementation.MetaDataService();
            CurrentDataBaseKey = met.GetDefaultDatabase();
        }
        public IList<Planner> GetPlannerList()
        {          
            var searchRepository = new PlannerRepository() { CurrentDataBaseKey = CurrentDataBaseKey };
            var result = searchRepository.GetPlannerList();
            return result;
        }

        public string GetPlantWeek()
        {
            var searchRepository = new PlannerRepository() { CurrentDataBaseKey = CurrentDataBaseKey };
            var result = searchRepository.GetPlantWeek();
            return result;
        }
    }
}
