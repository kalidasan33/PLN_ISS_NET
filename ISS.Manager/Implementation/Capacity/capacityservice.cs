using ISS.BusinessRules.Contract.Capacity;
using ISS.Core.Model.Capacity;
using ISS.Repository.Capacity;
using System.Collections.Generic;
using System.Data;

namespace ISS.BusinessService.Implementation.Capacity
{
    public class CapacityService : ICapacityService
    {

        public CapacityService()
        {
          
        }
        public DataTable GetCapacityAllocations(CapacitySearch serachCriteria, string action)
        {
            CapacityRepository repository = new CapacityRepository();
            return repository.GetCapacityAllocations(serachCriteria,action);
        }

        public IList<AllocationPopup> GetAllocationDetails(CapacitySearch searchCriteria)
        {
            CapacityRepository repository = new CapacityRepository();
            return repository.GetAllocationDetails(searchCriteria);
        }

        public IList<AggregatePopup> GetAggregationDetails(CapacitySearch searchCriteria)
        {
            CapacityRepository repository = new CapacityRepository();
            return repository.GetPlantAggregateDetails(searchCriteria);
        }
    }
}
