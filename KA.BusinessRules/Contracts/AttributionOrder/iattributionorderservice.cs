using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISS.Core.Model.Order;
using ISS.Core.Model.Common;

namespace KA.BusinessRules.Contract.AttributionOrder
{
   public partial interface IAttributionOrderService
    {
       Result InsertAttributionOrder(WorkOrderHeader woHeader);

       List<WorkOrderDetail> GetKAMFGPathId(string style, string color, string attribute, List<MultiSKUSizes> size, string dLoc);

       bool UpdateWOMOrder(WOMDetail wom);

       bool UpdateWOMGroupedOrders(List<WOMDetail> wom);

       bool UpdateChange(List<WOMDetail> WomDet);
       bool SummarizeAOMOrders(List<WOMDetail> wom);

       bool ValidatePKGStyle(WorkOrderDetail wod);
       List<WorkOrderDetail> GetKAMFGPathDetails(string style, string color, string attribute, List<MultiSKUSizes> size, string dLoc, string mfgPathId);
       List<WorkOrderHeader> GetAODCLocations(string style, string color, string attribute);
    }
   public interface ICachingAttributionOrderService : IAttributionOrderService
   {

   }
}
