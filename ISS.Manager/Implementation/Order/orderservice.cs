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

namespace ISS.BusinessService.Implementation.Order
{
    public partial class OrderService : IOrderService
    {
        

        public List<SummaryResult> GetSummary(SummaryFilterModal filterModal)
        {
            var searchRepository = new SummaryRepository() ;
            var result = searchRepository.GetSummary(filterModal);
            return result;
        }

        public IList<NetDemand> GetNetDemandTotal(NetDemand netDemand)
        {
            var searchRepository = new SummaryRepository();
            var result = searchRepository.GetNetDemand(netDemand);
            return result;
        }


    }
}
