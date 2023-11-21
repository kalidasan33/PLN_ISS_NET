using ISS.BusinessService.Contract.Common;
using ISS.Core.Model.Common;
using ISS.Repository.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISS.BusinessService.Implementation.Common
{
    public class WorkCenterService : IWorkCenterService
    {
        public string CurrentDataBaseKey { get; set; }

        public WorkCenterService()
        {
            ISS.BusinessService.Implementation.MetaDataService met = new ISS.BusinessService.Implementation.MetaDataService();
            CurrentDataBaseKey = met.GetDefaultDatabase();
        }
        public IList<WorkCenter> GetWorkCenterList(string capacityGroup, string planner)
        {          
            var searchRepository = new WorkCenterRepository() { CurrentDataBaseKey = CurrentDataBaseKey };
            var result = searchRepository.GetWorkCenterList(capacityGroup, planner);
            return result;
        }
    }
}
