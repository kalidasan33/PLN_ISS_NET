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
        //DataTable GetFirstQuery(SummaryFilterModal filterModal);
        List<SummaryResult> GetSummary(SummaryFilterModal filterModal);

        IList<NetDemand> GetNetDemandTotal(NetDemand netDemand);
        IList<RequisitionDetail> GetRequisitionDetail(RequisitionDetail requisitiondetail);
        IList<RequisitionDetail> GetRevisionAndMatlCd(string style, string color, string attribute, string size);
        bool GetStyleValidation(string StyleCode, string BusinessCode);
        IList<Requisition> GetRequisition(String RequisitionId);
        IList<RequisitionOrder> GetRequisitionOrder(RequisitionOrderSearch requisitionorder);
        IList<VendorSearch> GetVendor(VendorSearch vendorSearch);
        IList<VendorSearch> GetVendorSearch(VendorSearch vendorSearch);
        IList<Requisition> SaveRequisition(Requisition requisition);
        bool DeleteRequisition(Requisition req);
        //Work Order
        IList<SKU> GetColorCode(string style);
        IList<SKU> GetAOColorCode(string style);
        IList<WorkOrderDetail> GetColorCodes(string style);
        IList<SKU> GetAttribute(string style, string color);
        IList<SKU> GetAOAttribute(string style, string color);
        IList<WorkOrderDetail> GetAttributeCodes(string style, string color);
        IList<SKU> GetSize(string style, string color, string attribute);
        IList<SKU> GetSiz(string style, string color, string attribute);
        IList<WorkOrderDetail> GetSizes(string style, string color, string attribute);
        //IList<WorkOrderDetail> GetMFGPathId(string style, string color, string attribute, string size, string dLoc);
        IList<WorkOrderDetail> GetMFGPathId(string style, string color, string attribute, List<MultiSKUSizes> size, string dLoc);
        IList<SKU> GetMaxRevision(string style, string color, string attribute, string size);
        // IList<WorkOrderDetail> GetMaxRevisions(string style, string color, string attribute, string size);
        IList<WorkOrderDetail> GetMaxRevisions(string style, string color, string attribute, List<MultiSKUSizes> size,string AsrtCode);
        IList<SKU> GetUOM(string style, string color, string attribute, string size);
        IList<SKU> GetMFG(string style, string color, string attribute, List<MultiSKUSizes> size, string pdc);
        //IList<WorkOrderDetail> GetRevisions(string style, string color, string attribute, string size, string asrtCode); 
        IList<WorkOrderDetail> GetRevisions(string style, string color, string attribute, List<MultiSKUSizes> size, string asrtCode);
        IList<WorkOrderDetail> GetRevisionInLine(string style, string color, string attribute, List<MultiSKUSizes> size, string asrtCode, string revCode);
        IList<WorkOrderDetail> GetPKGCheck(string style, string color, string attribute, List<MultiSKUSizes> size, string asrtCode, string pkgCode);
        IList<WorkOrderDetail> GetPKGStyle(string style, string color, string attribute, List<MultiSKUSizes> size, string revision, string asrtCode);
        //IList<WorkOrderDetail> GetPathRankingAltId(string style, string color, string attribute, List<MultiSKUSizes> size, string mfgPathId, string cutPath, string txtPath);
        IList<WorkOrderDetail> GetPathRankingAltId(string style, string color, string attribute, string size, string mfgPathId, string cutPath, string txtPath);
        // IList<WorkOrderDetail> GetWOChildSKU(string style, string color, string attribute, string originTypeCode, string revision, string size, string asrtCode);
        IList<WorkOrderDetail> GetWOChildSKU(string style, string color, string attribute, string originTypeCode, string revision, List<MultiSKUSizes> size, string asrtCode);
        IList<SKU> GetRevisionAndUom(string style, string color, string attribute, string size);
        IList<SKU> GetRevisionNumbers(string style, string color, string attribute, string size);
        IList<WorkOrderDetail> GetWOAsrtCode(string style);
        IList<SKU> GetStdCaseQty(string style, string color, string attribute, string size, string rev);
        IList<AttributionMrz> GetAttributeMrz(string orderid, string style, string color, string attribute, string size);
        

        WorkOrderDetail UpdateCumulative(WorkOrderDetail wod);
        WorkOrderDetail CalculateVariance(WorkOrderDetail wod);
        WorkOrderDetail OrdersToCreate(WorkOrderDetail wod);
        WorkOrderDetail OnChangePFS(WorkOrderDetail wod);
        WorkOrderDetail UpdateLBS(WorkOrderDetail wod);
        WorkOrderDetail CancelWODetail(WorkOrderDetail wod);
        WorkOrderDetail ReCalcWODetail(WorkOrderDetail wod);
        IList<WorkOrderDetail> GetPackCode();
        IList<WorkOrderDetail> GetCatCode();
        //Source Order Retrieve
        String getNewRequisitiuonId();
        bool AddOrderComment(OrderComment comment);
        IList<RequisitionSearch> GetRequisitionSearch(RequisitionSearch reqSearch);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sku">SKU details required</param>
        /// <param name="req">LwCompany, LwVendorLoc and Vendor# required</param>
        /// <returns></returns>
        bool VerifyVendor(SKU sku, Decimal LwCompany, Decimal VendorNo, String LwVendorLoc);
        bool VerifyDCMfgpath(SKU sku, String DCorSewPlant);
        bool VerifySKUDCCombination(SKU sku, String mfgPathId);
        bool VerifyRevision(SKU sku, String mfgPathId);

        /// <summary>
        /// SKU details, DC and SewPlant required
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        List<RequisitionOrder> GetPlanningleadTime(SKU sku, String DC, String SewPlant);

        String GetStyleDesc(String styleCode);

        KeyValuePair<bool, String> InsertRequisition(Requisition req, List<RequisitionDetail> items);
        decimal GetPlannedWeek();
        decimal GetPlannedYear();
        DateTime GetPlannedDate(decimal Week, decimal Year, string dueDate);
        List<RequisitionExpandView> GetReqExpandDetails(string requisitionId);

        bool IsReqActive(string reqsnId, decimal reqsnVersion);

        IList<Requisition> GetRequisitionHeader(String requisitionId);

        IList<RequisitionBOM> GetRequisitionBOM(RequisitionExpandView reqExView);

        KeyValuePair<bool, String> RequisitionResetForConstruction(Requisition req);

        KeyValuePair<bool, String> ReleaseToSourcing(Requisition req);

        OrderComment GetOrderComments(Requisition req);
        void calculatePlannedAndScheduledDates(Requisition req, RequisitionDetail item);

        List<RequisitionDetail> GetSOStyleDetail(String Style);

        List<RequisitionDetail> GetDCValidate(String DcLoc);

        List<RequisitionDetail> GetMFGValidate(String MFGPathId);

        List<RequisitionDetail> GetDCValidates(String DcLoc, String BusinessUnit);
        string GetDemandDvr(String PoNumber, String LineNumber);

        bool GetSOSKUBDDetail(SKU sku, String MFGPathId);

        KeyValuePair<bool, String> UpdateRequisition(Requisition req, List<RequisitionDetail> items);

        decimal GetGroupID();

        List<WOMDetail> GetWODetail(WOManagementSearch search);
        List<WOMDetail> GetWODetailExport(WOManagementSearch search);
        Result InsertWorkOrder(WorkOrderHeader woHeader);
        Result ValidateMultiSku(WorkOrderHeader woHeader);
        List<WorkOrderCumulative> GetCuttingAltId(SKU sku);
        List<decimal> GetBulkGroupID(decimal dgridCount);
       
        List<WorkOrderDetail> GetOrderDetailByOrderLabel(string SuperOrder);

        bool DeleteAttributeMrzData(AttributionMrz item, String userId);

        WorkOrderDetail GetGarmentSKU(string style, string color, string attribute, List<MultiSKUSizes> size, string mfgPathId);

        bool ValidateCategoryCode(String CatCode);

        IList<DCCode> GetDCCode();
        IList<DemandDrivers> GetDemandDrivers(string style, string color, string attribute, string size, string RevisionNO);
        IList<DemandDrivers> GetDemandDriver();
        bool GetPopupHaaAO(string valHaa);
    }

    public interface ICachingInformationService : IOrderService
    {

    }
}
