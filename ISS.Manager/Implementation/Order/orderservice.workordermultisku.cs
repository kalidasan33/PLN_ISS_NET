using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISS.Core.Model.Common;
using ISS.BusinessRules.Contract.Order;
using ISS.Repository.Order;
using System.Data;
using ISS.Core.Model.Order;
using ISS.Common;
namespace ISS.BusinessService.Implementation.Order
{
    public partial class OrderService :IOrderService
    {
        public decimal GetGroupID()
        {
            var searchRepository = new WorkOrderRepository();
            var result = searchRepository.GetGroupID();
            return result;
        }

        public Result InsertWorkOrder(WorkOrderHeader woHeader)
        {
            //String Msg = "Required fields are missing.";
            //bool Status = false;
            var searchRepository = new WorkOrderRepository();
            var result = searchRepository.InsertWorkOrder(woHeader);

            return result;
        }

        public Result ValidateMultiSku(WorkOrderHeader woHeader)
        {
            //String Msg = "Required fields are missing.";
            //bool Status = false;
            var searchRepository = new WorkOrderRepository();
            var result = searchRepository.WOValidations(woHeader);

            return result;
        }

        public List<WorkOrderCumulative> GetCuttingAltId(SKU sku)
        {
            var searchRepository = new WorkOrderRepository();
            var result = searchRepository.GetCuttingAltId(sku);

            return result;
        }

        public List<decimal> GetBulkGroupID(decimal dgridCount)
        {
            var searchRepository = new WorkOrderRepository();
            var result = searchRepository.GetBulkGroupID(dgridCount);

            return result;
        }
    }
}
