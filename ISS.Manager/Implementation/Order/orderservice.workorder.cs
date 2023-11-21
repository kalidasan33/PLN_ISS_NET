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
    public partial class OrderService :IOrderService
    {
        public IList<SKU>GetColorCode(string style)
        {
            var searchRepository = new WorkOrderRepository();
            var dataResult = searchRepository.GetColorCode(style);
            return dataResult;
        }
        public IList<SKU> GetAOColorCode(string style)
        {
            var searchRepository = new WorkOrderRepository();
            var dataResult = searchRepository.GetAOColorCode(style);
            return dataResult;
        }
        public IList<WorkOrderDetail> GetColorCodes(string style)
        {
            var searchRepository = new WorkOrderRepository();
            var dataResult = searchRepository.GetColorCodes(style);
            return dataResult;
        }

        public IList<WorkOrderDetail> GetAttributeCodes(string style, string color)
        {
            var searchRepository = new WorkOrderRepository();
            var dataResult = searchRepository.GetAttributeCodes(style, color);
            return dataResult;
        }

        public IList<WorkOrderDetail> GetSizes(string style, string color, string attribute)
        {
            var searchRepository = new WorkOrderRepository();
            var dataResult = searchRepository.GetSizes(style, color, attribute);
            return dataResult;
        }
        //public IList<WorkOrderDetail> GetMaxRevisions(string style, string color, string attribute, string size)
        //{
        //    var searchRepository = new WorkOrderRepository();
        //    var dataResult = searchRepository.GetMaxRevisions(style, color, attribute, size);
        //    return dataResult;
        //}
        public IList<WorkOrderDetail> GetMaxRevisions(string style, string color, string attribute, List<MultiSKUSizes> size,string AsrtCode)
        {
            var searchRepository = new WorkOrderRepository();
            var dataResult = searchRepository.GetMaxRevisions(style, color, attribute, size,AsrtCode);
            return dataResult;
        }

        public IList<SKU> GetAttribute(string style,string color)
        {
            var searchRepository = new WorkOrderRepository();
            var dataResult = searchRepository.GetAttribute(style,color);
            return dataResult;
        }
        public IList<SKU> GetAOAttribute(string style, string color)
        {
            var searchRepository = new WorkOrderRepository();
            var dataResult = searchRepository.GetAOAttribute(style, color);
            return dataResult;
        }
        public IList<SKU> GetSize(string style,string color,string attribute)
        {
            var searchRepository = new WorkOrderRepository();
            var dataResult = searchRepository.GetSize(style,color,attribute);
            return dataResult;
        }
        public IList<SKU> GetSiz(string style, string color, string attribute)
        {
            var searchRepository = new WorkOrderRepository();
            var dataResult = searchRepository.GetSiz(style, color, attribute);
            return dataResult;
        }
        //public IList<WorkOrderDetail> GetMFGPathId(string style, string color, string attribute, string size,string dLoc)
        //{
        //    var searchRepository = new WorkOrderRepository();
        //    var dataResult = searchRepository.GetMFGPathId(style,color,attribute,size,dLoc);
        //    return dataResult;
        //}
        public IList<WorkOrderDetail> GetMFGPathId(string style, string color, string attribute, List<MultiSKUSizes> size, string dLoc)
        {
            var searchRepository = new WorkOrderRepository();
            var dataResult = searchRepository.GetMFGPathId(style, color, attribute, size, dLoc);
            return dataResult;
        }
        
        public IList<SKU> GetMaxRevision(string style,string color, string attribute, string size)
        {
            var searchRepository = new WorkOrderRepository();
            var dataResult = searchRepository.GetMaxRevision(style,color,attribute,size);
            return dataResult;
        }

       
        public IList<SKU> GetUOM(string style, string color, string attribute, string size)
        {
            var searchRepository = new WorkOrderRepository();
            var dataResult = searchRepository.GetUOM(style, color, attribute, size);
            return dataResult;
        }
        public IList<SKU> GetMFG(string style, string color, string attribute, List<MultiSKUSizes> size, string pdc)
        {
            var searchRepository = new WorkOrderRepository();
            var dataResult = searchRepository.GetMFG(style, color, attribute, size, pdc);
            return dataResult;
        }
        //public IList<WorkOrderDetail> GetRevisions(string style,string color,string attribute,string size,string asrtCode)
        //{
        //    var searchRepository = new WorkOrderRepository();
        //    var dataResult = searchRepository.GetRevisions(style,color,attribute,size,asrtCode);
        //    return dataResult;
        //}

        public IList<WorkOrderDetail> GetRevisions(string style, string color, string attribute, List<MultiSKUSizes> size, string asrtCode)
        {
            var searchRepository = new WorkOrderRepository();
            var dataResult = searchRepository.GetRevisions(style, color, attribute, size, asrtCode);
            return dataResult;
        }
        public IList<WorkOrderDetail> GetRevisionInLine(string style, string color, string attribute, List<MultiSKUSizes> size, string asrtCode, string revCode)
        {
            var searchRepository = new WorkOrderRepository();
            var dataResult = searchRepository.GetRevisionInLine(style, color, attribute, size, asrtCode, revCode);
            return dataResult;
        }
        //
        public IList<WorkOrderDetail> GetPKGCheck(string style, string color, string attribute, List<MultiSKUSizes> size, string asrtCode, string pkgCode)
        {
            var searchRepository = new WorkOrderRepository();
            var dataResult = searchRepository.GetPKGCheck(style, color, attribute, size, asrtCode, pkgCode);
            return dataResult;
        }
        public IList<SKU> GetRevisionAndUom(string style, string color, string attribute, string size)
        {
            var searchRepository = new WorkOrderRepository();
            var dataResult = searchRepository.GetRevisionAndUom(style, color, attribute, size);
            return dataResult;
        }

        public IList<SKU> GetRevisionNumbers(string style, string color, string attribute, string size)
        {
            var searchRepository = new WorkOrderRepository();
            var dataResult = searchRepository.GetRevisionNumbers(style, color, attribute, size);
            return dataResult;
        }
        public IList<WorkOrderDetail> GetPKGStyle(string style, string color, string attribute, List<MultiSKUSizes> size, string revision, string asrtCode)
        {
            var searchRepository = new WorkOrderRepository();
            var dataResult = searchRepository.GetPKGStyle(style,color,attribute,size,revision,asrtCode);
            return dataResult;
        }
        //Newly Added
        public IList<WorkOrderDetail> GetPathRankingAltId(string style, string color, string attribute, string size, string mfgPathId, string cutPath, string txtPath)
        {
            var searchRepository = new WorkOrderRepository();
            var dataResult = searchRepository.GetPathRankingAltId(style, color, attribute, size, mfgPathId, cutPath, txtPath);
            return dataResult;
        }
        //End
        public IList<SKU> GetStdCaseQty(string style, string color, string attribute, string size, string rev)
        {
            var searchRepository = new WorkOrderRepository();
            var dataResult = searchRepository.GetStdCaseQty(style, color, attribute, size, rev);
            return dataResult;
        }
  
        //public IList<WorkOrderDetail> GetWOChildSKU(string style, string color, string attribute, string originTypeCode, string revision, string size, string asrtCode)
        //{
        //    var searchRepository = new WorkOrderRepository();
        //    var dataResult = searchRepository.GetWOChildSKU(style, color, attribute,originTypeCode, revision, size, asrtCode);
        //    return dataResult;
        //}
        public IList<WorkOrderDetail> GetWOChildSKU(string style, string color, string attribute, string originTypeCode, string revision, List<MultiSKUSizes> size, string asrtCode)
        {
            var searchRepository = new WorkOrderRepository();
            var dataResult = searchRepository.GetWOChildSKU(style, color, attribute, originTypeCode, revision, size, asrtCode);
            return dataResult;
        }

        public IList<WorkOrderDetail> GetWOAsrtCode(string style)
        {
            var searchRepository = new WorkOrderRepository();
            var dataResult = searchRepository.GetWOAsrtCode(style);
            return dataResult;
        }
        public Decimal GetPlannedWeek()
        {
            var searchRepository = new WorkOrderRepository();
            var result = searchRepository.GetPlannedWeek();
            return result;
        }
        public Decimal GetPlannedYear()
        {
            var searchRepository = new WorkOrderRepository();
            var result = searchRepository.GetPlannedYear();
            return result;
        }

        public DateTime GetPlannedDate(decimal Week,decimal Year,string dueDate)
        {
            var searchRepository = new WorkOrderRepository();
            var result = searchRepository.GetPlannedDate(Week,Year,dueDate);
            //if (result.Date.Year==0001)
            //    return DateTime.Now;
            //else
                return result;
        }

        public IList<WorkOrderDetail> GetPackCode()
        {
            var searchRepository = new WorkOrderRepository();
            var dataResult = searchRepository.GetPackCode();
            return dataResult;
        }
       public IList<WorkOrderDetail> GetCatCode()
        {
            var searchRepository = new WorkOrderRepository();
            var dataResult = searchRepository.GetCatCode();
            return dataResult;
        }
        //Calculate Cumulative 

        public WorkOrderDetail UpdateCumulative(WorkOrderDetail wod)
        {
            var searchRepository = new WorkOrderRepository();
            var result = searchRepository.UpdateCumulative(wod);
            return result;
        }

        public WorkOrderDetail CancelWODetail(WorkOrderDetail Wod)
        {
            var searchRepository = new WorkOrderRepository();
            var result = searchRepository.CancelWODetail(Wod);
            return result;
        }

        public WorkOrderDetail ReCalcWODetail(WorkOrderDetail Wod)
        {
            var searchRepository = new WorkOrderRepository();
            var result = searchRepository.ReCalcWODetail(Wod);
            return result;
        }
        public WorkOrderDetail CalculateVariance(WorkOrderDetail wod)
        {
            var searchRepository = new WorkOrderRepository();
            var result = searchRepository.CalculateVariance(wod);
            return result;
        }

        public WorkOrderDetail OnChangePFS(WorkOrderDetail wod)
        {
            var searchRepository = new WorkOrderRepository();
            var result = searchRepository.OnChangePFS(wod);
            return result;
        }

        public WorkOrderDetail UpdateLBS(WorkOrderDetail wod)
        {
            var searchRepository = new WorkOrderRepository();
            var result = searchRepository.UpdateLBS(wod);
            return result;
        }
        public WorkOrderDetail OrdersToCreate(WorkOrderDetail wod)
        {
            var searchRepository = new WorkOrderRepository();
            var result = searchRepository.OrdersToCreate(wod);
            return result;
        }
        public List<WorkOrderDetail> GetOrderDetailByOrderLabel(string SuperOrder)
        {
            var searchRepository = new WorkOrderRepository();
            var result = searchRepository.GetOrderDetailByOrderLabel(SuperOrder);
            return result;
        }

        public WorkOrderDetail GetGarmentSKU(string style, string color, string attribute, List<MultiSKUSizes> size, string mfgPathId)
        {
            var searchRepository = new WorkOrderRepository();
            var result = searchRepository.GetGarmentSKU(style, color, attribute, size,mfgPathId);
            return result;
        }
        public bool ValidateCategoryCode(String CatCode)
        {
            var searchRepository = new WorkOrderRepository();
            var IsSuccess=searchRepository.ValidateCategoryCode(CatCode);
            return IsSuccess;
        }

        public IList<DCCode> GetDCCode()
        {
            var searchRepository = new WorkOrderRepository();
            var dataResult = searchRepository.GetDCCode();
            return dataResult;
        }

        public IList<DemandDrivers> GetDemandDrivers(string style, string color, string attribute, string size, string RevisionNO)
        {
            var searchRepository = new WorkOrderRepository();
            var dataResult = searchRepository.GetDemandDrivers(style, color, attribute, size, RevisionNO);
            return dataResult;
        }
        public IList<DemandDrivers> GetDemandDriver()
        {
            var searchRepository = new WorkOrderRepository();
            var dataResult = searchRepository.GetDemandDriver();
            return dataResult;
        }
        public List<WorkOrderDetail> GetWOStyleDetail(String Style)
        {
            var searchRepository = new WorkOrderRepository();
            return searchRepository.GetWOStyleDetail(Style);

        }
        public bool GetPopupHaaAO(string valHaa)
        {
            var searchRepository = new WorkOrderRepository();
            var dataResult = searchRepository.GetPopupHaaAO(valHaa);
            return dataResult;
        }


        public IList<AttributionMrz> GetAttributeMrz(string orderid,string style, string color, string attribute, string size)
        {
            var searchRepository = new WorkOrderRepository();
            var dataResult = searchRepository.GetAttributeM(orderid,style, color, attribute, size);
            return dataResult;
        }

        public bool DeleteAttributeMrzData(AttributionMrz item, String userId)
        {
            var searchRepository = new WorkOrderRepository();
            var dataResult = searchRepository.DeleteAttributeMrzData(item, userId);
            return dataResult;
        }
    }
}
