using ISS.Core.Model.Capacity;
using System.Collections.Generic;
using System.Data;

namespace ISS.BusinessRules.Contract.Capacity
{
    public interface ICapacityService
    {
        IList<AllocationPopup> GetAllocationDetails(CapacitySearch searchCriteria);
        IList<AggregatePopup> GetAggregationDetails(CapacitySearch searchCriteria);
        DataTable GetCapacityAllocations(CapacitySearch serachCriteria, string action);
    }

    public interface ICachingInformationService : ICapacityService
    {

    }
}
