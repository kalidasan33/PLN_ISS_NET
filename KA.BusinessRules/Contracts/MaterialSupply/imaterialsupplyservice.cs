using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KA.Core.Model.MaterialSupply;
using ISS.Core.Model.Common;

namespace KA.BusinessRules.Contract.MaterialSupply
{
    public partial interface IMaterialSupplyService
    {

        List<MaterialPAB> MaterialSupplySearchDetails(MaterialBlankSupplySearch mbSearch);
  		List<MaterialPAB> MaterialSupplyGetWeeks();
        IList<MaterialBlankSupplySearch> GetDC();
 
    }
    public interface ICachingAttributionOrderService : IMaterialSupplyService
   {

   }
}
