using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KA.BusinessRules.Contract.BulkOrder;
using KA.Repository.BulkOrder;
using System.Data;
using KA.Core.Model.BulkOrder;
using ISS.Common;

namespace KA.BusinessService.Implementation.BulkOrder
{
    public partial class BulkOrderService 
    {

        public KeyValuePair<bool, String> UpdateBulkOrder(BulkOrderDetail req, List<BulkOrderDetail> items)
        {
            var repository = new BulkOrderRepository();
            KeyValuePair<bool, String> result;
            if (req.ProgramSource == KAProgramSource.ISS2166.ToString())
            {
                result = repository.UpdateComponent(req, items);
            }
            else
            {
                result = repository.UpdateBulkOrder(req, items);
            }
            
           
            return result;
        }

        public bool DeleteBulkOrder(String bulkNo,string programSource)
        {
            bool result = false;
            var repository = new BulkOrderRepository();
            if(programSource==KAProgramSource.ISS2166.ToString())
            {
                result = repository.DeleteAllComponentOrders(bulkNo);

            }
            else
            {
                result=repository.DeleteBulkOrder(bulkNo);
            }
            return result;
        }

        public bool VerifyComponentOrder(BulkOrderDetail item, out String ErrMsg)
        {
            bool result = false;
            var repository = new BulkOrderRepository();

            result = repository.VerifyComponentOrder(item, out ErrMsg);
            
            return result;
        }

        public KeyValuePair<bool, String> CompleteComponentProcess(BulkOrderDetail req, List<BulkOrderDetail> items)
        {
            var repository = new BulkOrderRepository();
            KeyValuePair<bool, String> result = new KeyValuePair<bool,string>();
            if (req.ProgramSource == KAProgramSource.ISS2166.ToString())
            {
                result = repository.CompleteComponentProcess(req, items);
            }
            
            return result;
        }

        public KeyValuePair<bool, String> ActivateComponentProcess(BulkOrderDetail req, List<BulkOrderDetail> items)
        {
            var repository = new BulkOrderRepository();
            KeyValuePair<bool, String> result = new KeyValuePair<bool, string>();
            if (req.ProgramSource == KAProgramSource.ISS2166.ToString())
            {
                result = repository.ActivateComponentProcess(req, items);
            }

            return result;
        }
    }
}
