using ISS.Core.Model.Order;
using ISS.Core.Model.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISS.BusinessRules.Contract.Order
{
    public partial interface IOrderService
    {

        List<WOMDetail> getProductionOrders(string superOrder);
        //Asif 10/9/2018 To Add Selling Sku popup while clicking in Selling Style in WOM screen
        List<WOMDetail> getSellingSku(string superOrder);
        List<WOMDetail> getProductionOrdersLbs(string superOrder);

        List<WOMDetail> getFabricDetail(string superOrder);
        List<WOMPipeline> getPipelineDates(string superOrder);
        List<WorkOrderDetail> PopulateMachineTypes(WorkOrderDetail Wod);

        List<WOMDetail> getAlternateId(SKU sku);
        List<WOMDetail> PopulateCutPathTxtPath(WorkOrderDetail Wod);
        List<WOMDetail> PopulateCutPathTxtPath(string SuperOrder, string DyeCode, String CutPath);
        List<WOMDetail> GetSuperOrderDetail(string SuperOrder);

        List<WorkOrderDetail> GetGarmentSKU(String SuperOrder);

        bool ValidatePlant(String Plant);

        int ValidateAltBefore(String SuperOrder);
        int ValidateAlt(String SuperOrder);
        bool DeleteOrder(RequisitionDetail item);
 		
		//WOM Save
        Result UpdateChange(List<WOMDetail> WomDet);

        
        bool UpdateWOMOrder(WOMDetail wom);
        bool UpdateWOMGroupedOrders(List<WOMDetail> wom);

        string GetNote(string superOrder);
        List<WorkOrderDetail> GetWOStyleDetail(string Style);
    }

   
}
