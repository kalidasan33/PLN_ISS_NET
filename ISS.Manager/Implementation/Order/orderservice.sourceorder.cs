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
        public IList<Requisition> SaveRequisition(Requisition requisition)
        {
            var searchRepository = new SourceOrderRepository();
            searchRepository.SaveRequisitionDetail(requisition);
            return null;
        }
        public IList<RequisitionDetail> GetRequisitionDetail(RequisitionDetail requisitiondetail)
        {
            var searchRepository = new SourceOrderRepository();
            var dataResult = searchRepository.GetRequisitionDetail(requisitiondetail.RequisitionId);
            return dataResult;
        }
        public bool GetStyleValidation(string StyleCode, string BusinessCode)
        {
            var searchRepository = new SourceOrderRepository();
            var dataResult = searchRepository.GetStyleValidation(StyleCode, BusinessCode);
            return dataResult;
        }
        public IList<Requisition> GetRequisition(String RequisitionId)
        {
            var searchRepository = new SourceOrderRepository();
            var dataResult = searchRepository.GetRequisition(RequisitionId);
            return dataResult;
        }
        public IList<RequisitionDetail> GetRevisionAndMatlCd(string style, string color, string attribute, string size)
        {
            var searchRepository = new SourceOrderRepository();
            var dataResult = searchRepository.GetRevisionAndMatlCd(style, color, attribute, size);
            return dataResult;
        }
        public IList<VendorSearch> GetVendor(VendorSearch vendorSearch)
        {
            var searchRepository = new SourceOrderRepository();
            var dataResult = searchRepository.GetVendor(vendorSearch);
            return dataResult;
        }
        public IList<RequisitionOrder> GetRequisitionOrder(RequisitionOrderSearch requisitionorder)
        {
            var searchRepository = new SourceOrderRepository();
            var dataResult = searchRepository.GetRequisitionOrder(requisitionorder);
            return dataResult;
        }

        public IList<VendorSearch> GetVendorSearch(VendorSearch vendorSearch)
        {
            var searchRepository = new SourceOrderRepository();
            var dataResult = searchRepository.GetVendorSearch(vendorSearch);
            return dataResult;
        }
        public String getNewRequisitiuonId()
        {
            var searchRepository = new SourceOrderRepository();
            var dataResult = searchRepository.getNewRequisitiuonId();
            return dataResult;
        }
        public IList<RequisitionSearch> GetRequisitionSearch(RequisitionSearch reqSearch)
        {
            var searchRepository = new SourceOrderRepository();
            var dataResult = searchRepository.GetRequisitionSearch(reqSearch);
            return dataResult;
        }
        public bool AddOrderComment(OrderComment comment)
        {
            var searchRepository = new SourceOrderRepository();
            var dataResult = searchRepository.AddOrderComment(comment);
            return dataResult;
        }
        public bool VerifyVendor(SKU sku, Decimal LwCompany, Decimal VendorNo, String LwVendorLoc)
        {
            var searchRepository = new SourceOrderRepository();
            var dataResult = searchRepository.VerifyVendor(sku, LwCompany, VendorNo, LwVendorLoc);
            return dataResult;
        }

        public bool VerifyDCMfgpath(SKU sku, String DCorSewPlant)
        {
            var searchRepository = new SourceOrderRepository();
            var dataResult = searchRepository.VerifyDCMfgpath(sku, DCorSewPlant);
            return dataResult;
        }

        public bool VerifySKUDCCombination(SKU sku, String mfgPathId)
        {
            var searchRepository = new SourceOrderRepository();
            var dataResult = searchRepository.VerifySKUDCCombination(sku, mfgPathId);
            return dataResult;
        }

        public bool VerifyRevision(SKU sku, String mfgPathId)
        {
            var searchRepository = new SourceOrderRepository();
            var dataResult = searchRepository.VerifyRevision(sku, mfgPathId);
            return dataResult;
        }

        public List<RequisitionOrder> GetPlanningleadTime(SKU sku, String DC, String SewPlant)
        {
            var searchRepository = new SourceOrderRepository();
            var dataResult = searchRepository.GetPlanningleadTime(sku, DC, SewPlant);
            return dataResult;
        }

        public String GetStyleDesc(String styleCode)
        {
            var searchRepository = new SourceOrderRepository();
            var dataResult = searchRepository.GetStyleDesc(styleCode);
            return dataResult;
        }

        public KeyValuePair<bool, String> InsertRequisition(Requisition req, List<RequisitionDetail> items)
        {
            var searchRepository = new SourceOrderRepository();
            var dataResult = searchRepository.InsertRequisition(req, items);
            return dataResult;
        }

        public KeyValuePair<bool, String> UpdateRequisition(Requisition req, List<RequisitionDetail> items)
        {
            var searchRepository = new SourceOrderRepository();
            var dataResult = searchRepository.UpdateRequisition(req, items);
            return dataResult;
        }

        public List<RequisitionExpandView> GetReqExpandDetails(string requisitionId)
        {
            var searchRepository = new SourceOrderRepository();
            var dataResult = searchRepository.GetReqExpandDetails(requisitionId);
            return dataResult;
        }

        public bool IsReqActive(string reqsnId, decimal reqsnVersion)
        {
            var searchRepository = new SourceOrderRepository();
            var dataResult = searchRepository.IsReqActive(reqsnId, reqsnVersion);
            return dataResult;
        }

        public IList<Requisition> GetRequisitionHeader(String requisitionId)
        {
            var searchRepository = new SourceOrderRepository();
            var dataResult = searchRepository.GetRequisitionHeaderExpandView(requisitionId);
            return dataResult;
        }

        public IList<RequisitionBOM> GetRequisitionBOM(RequisitionExpandView reqExView)
        {
            var searchRepository = new SourceOrderRepository();
            var dataResult = searchRepository.GetRequisitionBOM(reqExView);
            return dataResult;
        }

        public KeyValuePair<bool, String> RequisitionResetForConstruction(Requisition req)
        {
            var searchRepository = new SourceOrderRepository();
            var dataResult = searchRepository.RequisitionResetForConstruction(req);
            return dataResult;
        }

        public KeyValuePair<bool, String> ReleaseToSourcing(Requisition req)
        {
            var searchRepository = new SourceOrderRepository();
            var dataResult = searchRepository.ReleaseToSourcing(req);
            return dataResult;
        }

        public OrderComment GetOrderComments(Requisition req)
        {
            var searchRepository = new SourceOrderRepository();
            var dataResult = searchRepository.GetOrderComments(req);
            return dataResult;
        }
		public void calculatePlannedAndScheduledDates(Requisition req, RequisitionDetail item)        
        {
            var searchRepository = new SourceOrderRepository();
            searchRepository.calculatePlannedAndScheduledDates(req,item);
            
        }
        public  List<RequisitionDetail> GetSOStyleDetail(String Style)          
        {
            var searchRepository = new SourceOrderRepository();
            return searchRepository.GetSOStyleDetail( Style);
            
        }

        public List<RequisitionDetail> GetDCValidate(String DcLoc)
        {
            var searchRepository = new SourceOrderRepository();
            return searchRepository.GetDCValidate(DcLoc);

        }
        public List<RequisitionDetail> GetMFGValidate(String MFGPathId)
        {
            var searchRepository = new SourceOrderRepository();
            return searchRepository.GetMFGValidate(MFGPathId);

        }
        public List<RequisitionDetail> GetDCValidates(String DcLoc, String BusinessUnit)
        {
            var searchRepository = new SourceOrderRepository();
            return searchRepository.GetDCValidates(DcLoc, BusinessUnit);

        }
        public bool GetSOSKUBDDetail(SKU sku, String MFGPathId)      
        {
            var searchRepository = new SourceOrderRepository();
            return searchRepository.GetSOSKUBDDetail( sku,  MFGPathId) ;
            
        }

        public bool DeleteRequisition(Requisition req)
        {
            var searchRepository = new SourceOrderRepository();
           return  searchRepository.DeleteRequisition(req);
        }
        public string GetDemandDvr(String PoNum, String LiNo)
        {
            var searchRepository = new SourceOrderRepository();
            return searchRepository.Get_Demand_Source(PoNum, LiNo);

        }
      
    }
}
