using ISS.Core.Model.Order;
using KA.Core.Model.BulkOrder;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KA.BusinessRules.Contract.BulkOrder
{
    public partial interface IBulkOrderService
    {
        KeyValuePair<bool, String> UpdateBulkOrder(BulkOrderDetail req, List<BulkOrderDetail> items);

        List<BulkOrderDetail> GetBulkOrderDetail(string bulkNo, string programSource);
       
		List<BulkOrderDetail> BulkOrderSearchDetails(BulkOrderSearch bulkSearch);

        String GetLineNumber(List<BulkOrderDetail> BulkOrderDetail);

        bool DeleteBulkOrder(String bulkNo, string programSource);

        bool VerifyComponentOrder(BulkOrderDetail item, out String ErrMsg);

        KeyValuePair<bool, String> CompleteComponentProcess(BulkOrderDetail req, List<BulkOrderDetail> items);

        KeyValuePair<bool, String> ActivateComponentProcess(BulkOrderDetail req, List<BulkOrderDetail> items);
    }

    public interface ICachingBulkOrderService : IBulkOrderService
    {

    }
}
