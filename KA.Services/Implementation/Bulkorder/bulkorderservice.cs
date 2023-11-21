using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KA.BusinessRules.Contract.BulkOrder;
using KA.Repository.BulkOrder;
using System.Data;
using KA.Core.Model.BulkOrder;
using ISS.Core.Model.Order;
using KA.Repository.MaterialSupply; 

namespace KA.BusinessService.Implementation.BulkOrder
{
    public partial class BulkOrderService : IBulkOrderService
    {

        public BulkOrderService()
        {
          
        }

        public List<BulkOrderDetail> GetBulkOrderDetail(string bulkNo, string programSource)
        {
            BulkOrderRepository repository = new BulkOrderRepository();
            return repository.GetBulkOrderDetail(bulkNo, programSource);
        }

        public List<BulkOrderDetail> BulkOrderSearchDetails(BulkOrderSearch bulkSearch)
        {
            BulkOrderRepository repository = new BulkOrderRepository();
            return repository.BulkOrderSearchDetails(bulkSearch);
        }
       public string GetLineNumber(List<BulkOrderDetail> BulkOrderDetail)
        {
            BulkOrderRepository repository = new BulkOrderRepository();
            return repository.GetLineNumber(BulkOrderDetail);
        }
      
    }
}
