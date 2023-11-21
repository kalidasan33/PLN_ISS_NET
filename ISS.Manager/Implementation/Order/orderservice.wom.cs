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
    public partial class OrderService 
    {
        public List<WOMDetail> GetWODetail(WOManagementSearch search)
        {
            var searchRepository = new WOManagement();
            var dataResult = searchRepository.GetWODetail(search);
            return dataResult;
        }
        public List<WOMDetail> GetWODetailExport(WOManagementSearch search)
        {
            var searchRepository = new WOManagement();
            var dataResult = searchRepository.GetWODetailExport(search);
            return dataResult;
        }

        public List<WOMDetail> getProductionOrders(string superOrder)
        {
            var searchRepository = new WOManagement();
            var dataResult = searchRepository.getProductionOrders( superOrder);
            return dataResult;
        }
        //Asif 10/9/2018 To Add Selling Sku popup while clicking in Selling Style in WOM screen

        public List<WOMDetail> getSellingSku(string superOrder)
        {
            var searchRepository = new WOManagement();
            var dataResult = searchRepository.getSellingSku(superOrder);
            return dataResult;
        }

        public List<WOMDetail> getProductionOrdersLbs(string superOrder)
        {
            var searchRepository = new WOManagement();
            var dataResult = searchRepository.getProductionOrdersLbs( superOrder);
            return dataResult;
        }
        public List<WOMDetail> getFabricDetail(string superOrder)
        {
            var searchRepository = new WOManagement();
            var dataResult = searchRepository.getFabricDetail(superOrder);
            return dataResult;
        }

        public List<WOMPipeline> getPipelineDates(string superOrder)
        {
            var searchRepository = new WOManagement();
            var dataResult = searchRepository.getPipelineDates(superOrder);
            return dataResult;
        }
        
        public  List<WorkOrderDetail> PopulateMachineTypes(WorkOrderDetail Wod)
        {
            var searchRepository = new WorkOrderRepository();
            var dataResult = searchRepository.PopulateMachineTypes(Wod, false);
            return dataResult;
        }

        public List<WOMDetail> getAlternateId(SKU sku)
        {
            var searchRepository = new WOManagement();
            var dataResult = searchRepository.getAlternateId(sku);
            return dataResult;
        }
        public List<WOMDetail> PopulateCutPathTxtPath(WorkOrderDetail Wod)
        {
            var searchRepository = new WOManagement();
            var dataResult = searchRepository.PopulateCutPathTxtPath(Wod);
            return dataResult;
        }
        public List<WOMDetail> PopulateCutPathTxtPath(string SuperOrder, string DyeCode, string CutPath)
        {
            var searchRepository = new WOManagement();
            var dataResult = searchRepository.PopulateCutPathTxtPath(SuperOrder, DyeCode, CutPath);
            return dataResult;
        }
        public List<WOMDetail> GetSuperOrderDetail(string SuperOrder)
        {
            var searchRepository = new WOManagement();
            var dataResult = searchRepository.GetSuperOrderDetail(SuperOrder);
            return dataResult;
        }

        public List<WorkOrderDetail> GetGarmentSKU(String SuperOrder)
        {
            var searchRepository = new WOManagement();
            var dataResult = searchRepository.GetGarmentSKU(SuperOrder);
            return dataResult;
        }
        public bool ValidatePlant(String Plant)
        {
            var searchRepository = new WOManagement();
            var dataResult = searchRepository.ValidatePlant(Plant);
            return dataResult;
        }

        public int ValidateAltBefore(String SuperOrder)
        {
            var searchRepository = new WOManagement();
            var dataResult = searchRepository.ValidateAltBefore(SuperOrder);
            return dataResult;
        }

		public int ValidateAlt(String SuperOrder)
        {
            var searchRepository = new WOManagement();
            var dataResult = searchRepository.ValidateAlt(SuperOrder);
            return dataResult;
        }
        public bool DeleteOrder(RequisitionDetail item)
        {
            var searchRepository = new SourceOrderRepository ();
            var dataResult = searchRepository.DeleteOrder(item);
            return dataResult;
        }
        public bool UpdateWOMOrder(WOMDetail wom)
        {
            var searchRepository = new WOManagement ();
            var dataResult = searchRepository.UpdateWOMOrder(wom);
            return dataResult;
        } 

        //WOM Save
        public Result UpdateChange(List<WOMDetail>WomDet)
        {
            var searchRepository = new WOManagement();
            var dataResult = searchRepository.UpdateChange(WomDet);
            return dataResult;
        }

        //End Save
        public bool UpdateWOMGroupedOrders(List<WOMDetail> wom)
        {
            var searchRepository = new WOManagement ();
            var dataResult = searchRepository.UpdateWOMGroupedOrders(wom);
            return dataResult;
        }

        public string GetNote(string superOrder)
        {
            var searchRepository = new WorkOrderRepository();
            var dataResult = searchRepository.GetNote(superOrder);
            return dataResult;
        } 
    }
}
