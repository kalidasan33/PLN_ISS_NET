using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KA.BusinessRules.Contract.AttributionOrder;
using ISS.Core.Model.Order;
using ISS.Core.Model.Common;
using KA.Repository.AttributionOrder;
using ISS.Repository.Order;
namespace KA.BusinessService.Implementation.AttributionOrder
{
   public partial class AttributionOrderService:IAttributionOrderService
    {
       public Result InsertAttributionOrder(WorkOrderHeader woHeader)
       {
           var searchRepository = new AttributedWorkOrder();
           var result = searchRepository.SaveAttributedWO(woHeader);
           return result;
       }

       public List<WorkOrderDetail> GetKAMFGPathId(string style, string color, string attribute, List<MultiSKUSizes> size, string dLoc)
       {
           AttributedWorkOrder repository = new AttributedWorkOrder();
           return repository.GetKAMFGPathId(style, color, attribute, size, dLoc);
       }

       public bool UpdateWOMOrder(WOMDetail wom)
       {
           var searchRepository = new AttributedWorkOrder();
           var dataResult = searchRepository.UpdateWOMOrder(wom);
           return dataResult;
       }
       public bool UpdateWOMGroupedOrders(List<WOMDetail> wom)
       {
           var searchRepository = new AttributedWorkOrder();
           var dataResult = searchRepository.UpdateWOMGroupedOrders(wom);
           return dataResult;
       }
       //WOM Save
       public bool UpdateChange(List<WOMDetail> WomDet)
       {
           var searchRepository = new AttributedWorkOrder();
           var dataResult = searchRepository.UpdateChange(WomDet);
           return dataResult;
       }

       public bool SummarizeAOMOrders(List<WOMDetail> wom)
       {
           var searchRepository = new AttributedWorkOrder();
           var dataResult = searchRepository.SummarizeAOMOrders(wom);
           return dataResult;
       }

       public List<WorkOrderDetail> GetKAMFGPathDetails(string style, string color, string attribute, List<MultiSKUSizes> size, string dLoc, string mfgPathId)
       {
           AttributedWorkOrder repository = new AttributedWorkOrder();
           return repository.GetKAMFGPathDetails(style, color, attribute, size, dLoc, mfgPathId);
       }
       public bool ValidatePKGStyle(WorkOrderDetail wod)
       {
           var searchRepository = new WorkOrderRepository();
          String PkgStyle=wod.PKGStyle.Trim();
          
           var dataResult = searchRepository.GetPKGStyle(wod.SellingStyle,wod.ColorCode,wod.Attribute,wod.SizeList,wod.Revision.ToString(),wod.AssortCode);
           return (dataResult.Any(style=>style.PKGStyle==PkgStyle));
           
       }
       public List<WorkOrderHeader> GetAODCLocations(string style, string color, string attribute)
       {
           AttributedWorkOrder repository = new AttributedWorkOrder();
           return repository.GetAODCLocations(style, color, attribute);
       }
    }
}
