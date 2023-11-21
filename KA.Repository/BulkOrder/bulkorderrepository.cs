using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISS.DAL;
using System.Data;
using ISS.Common;
using System.Data.Common;
using KA.Core.Model.BulkOrder;
using ISS.Core.Model.Order;


namespace KA.Repository.BulkOrder
{
    public partial class BulkOrderRepository : RepositoryBase
    {
        public BulkOrderRepository()
            : base()
        {

        }

        public List<BulkOrderDetail> BulkOrderSearchDetails(BulkOrderSearch bulkSearch)
        {
            var queryBuilder = new StringBuilder();
            bool isAndNeeded = false;

            queryBuilder.Append(" SELECT d.BulkNumber, d.ProcessedToOS,d.DemandWeekendDate, d.VendorNo, d.LwVendorLoc, d.MFGPathId, d.DcLoc, d.ProgramSourceDesc, d.ProgramSource FROM (  ");
            queryBuilder.Append("SELECT i.KA_BULK_NBR as BulkNumber, i.PROCESSED_TO_OS as ProcessedToOS, to_date('01/01/0001','mm/dd/yyyy') as DemandWeekendDate, nvl(i.LW_VENDOR_NO,0) as VendorNo, i.LW_VENDOR_LOC as LwVendorLoc, i.MFG_PATH_ID as MFGPathId, i.DEMAND_LOC as  DcLoc, \'" + KAProgramSource.ISS2165.GetDescription() + "\' ProgramSourceDesc, \'" + KAProgramSource.ISS2165.ToString() + "\' ProgramSource, " +
                                   " row_number() OVER (PARTITION BY i.KA_BULK_NBR ORDER BY decode(i.PROCESSED_TO_OS, 'E', 1, 'N', 2, 'Y', 3, 4)) as r " +
                //" row_number() OVER (PARTITION BY i.KA_BULK_NBR ORDER BY i.PROCESSED_TO_OS) as r " +
                                    " from ka_preprocessor i, lawson_vendor l");
            queryBuilder.Append(" where i.lw_vendor_no = l.lw_vendor_no(+)  and i.lw_vendor_loc = l.lw_vendor_loc(+)  ");

            if (!string.IsNullOrEmpty(bulkSearch.BulkNumber))
            {
                queryBuilder.Append(" and RTRIM(i.KA_BULK_NBR) = \'" + bulkSearch.BulkNumber.TrimEnd() + "\' ");
            }
            if ((bulkSearch.FromDate.HasValue && bulkSearch.FromDate != DateTime.MinValue) && (bulkSearch.ToDate.HasValue && bulkSearch.ToDate != DateTime.MinValue))
            {
                queryBuilder.Append(" and trunc( i.CREATE_DATE) >= to_date(\'" + bulkSearch.FromDate.Value.ToString("MM/dd/yyyy") + "\','mm/dd/yyyy') and trunc(i.CREATE_DATE) <= to_date(\'" + bulkSearch.ToDate.Value.ToString("MM/dd/yyyy") + "\','mm/dd/yyyy') ");
            }
            else if (bulkSearch.ToDate.HasValue && !bulkSearch.FromDate.HasValue)
            {
                queryBuilder.Append(" and  trunc(i.CREATE_DATE )<= to_date(\'" + bulkSearch.ToDate.Value.ToString("MM/dd/yyyy") + "\','mm/dd/yyyy') ");
            }
            else if (bulkSearch.FromDate.HasValue && !bulkSearch.ToDate.HasValue)
            {
                queryBuilder.Append("  and trunc(i.CREATE_DATE) >= to_date(\'" + bulkSearch.FromDate.Value.ToString("MM/dd/yyyy") + "\','mm/dd/yyyy') ");
            }
            if (bulkSearch.ExcludeProcessed)
            {
                queryBuilder.Append("  and  i.PROCESSED_TO_OS <> '" + LOVConstants.BulkOrderStatus.Processed + "' ");
            }

            queryBuilder.Append("order by i.KA_BULK_NBR,  i.lw_vendor_no");

            queryBuilder.Append(") d Where d.r=1");

            queryBuilder.Append(" UNION ALL ");
            queryBuilder.Append(" SELECT d.BulkNumber, d.ProcessedToOS, d.DemandWeekendDate,  d.VendorNo, d.LwVendorLoc, d.MFGPathId, d.DcLoc, d.ProgramSourceDesc, d.ProgramSource FROM (  ");
            queryBuilder.Append("SELECT distinct i.KA_BULK_NBR as BulkNumber, decode(i.PROCESSED_TO_AVYX, 'C', 'Y', i.PROCESSED_TO_AVYX) as ProcessedToOS, i.DEMAND_WEEKEND_DATE as DemandWeekendDate, 0 as VendorNo, '' as LwVendorLoc, '' as MFGPathId, i.DEMAND_LOC as  DcLoc, \'" + KAProgramSource.ISS2166.GetDescription() + "\' ProgramSourceDesc, \'" + KAProgramSource.ISS2166.ToString() + "\' ProgramSource,  " +
                                   " row_number() OVER (PARTITION BY i.KA_BULK_NBR ORDER BY decode(i.PROCESSED_TO_AVYX, 'E', 1, 'N', 2, 'A', 3, 'C', 4, 5)) as r " + 
                                   //" row_number() OVER (PARTITION BY i.KA_BULK_NBR ORDER BY i.PROCESSED_TO_AVYX) as r " + 
                                " from KA_COMPONENT_PREPROCESSOR  i ");
            if (!string.IsNullOrEmpty(bulkSearch.BulkNumber) || bulkSearch.FromDate.HasValue || bulkSearch.ToDate.HasValue || bulkSearch.ExcludeProcessed || !bulkSearch.ExcludeProcessed)
                queryBuilder.Append(" where   ");
            

            if (!string.IsNullOrEmpty(bulkSearch.BulkNumber))
            {
                queryBuilder.Append(" RTRIM(i.KA_BULK_NBR) = \'" + bulkSearch.BulkNumber.TrimEnd() + "\' ");
                isAndNeeded = true;
            }
            if ((bulkSearch.FromDate.HasValue && bulkSearch.FromDate != DateTime.MinValue) && (bulkSearch.ToDate.HasValue && bulkSearch.ToDate != DateTime.MinValue))
            {
                if (isAndNeeded)
                    queryBuilder.Append(" and   ");
                queryBuilder.Append(" trunc( i.CREATE_DATE) >= to_date(\'" + bulkSearch.FromDate.Value.ToString("MM/dd/yyyy") + "\','mm/dd/yyyy') and trunc(i.CREATE_DATE) <= to_date(\'" + bulkSearch.ToDate.Value.ToString("MM/dd/yyyy") + "\','mm/dd/yyyy') ");
                isAndNeeded = true;
            }
            else if (bulkSearch.ToDate.HasValue && !bulkSearch.FromDate.HasValue)
            {
                if (isAndNeeded)
                    queryBuilder.Append(" and   ");
                queryBuilder.Append("  trunc(i.CREATE_DATE )<= to_date(\'" + bulkSearch.ToDate.Value.ToString("MM/dd/yyyy") + "\','mm/dd/yyyy') ");
                isAndNeeded = true;
            }
            else if (bulkSearch.FromDate.HasValue && !bulkSearch.ToDate.HasValue)
            {
                if (isAndNeeded)
                    queryBuilder.Append(" and   ");
                queryBuilder.Append("  trunc(i.CREATE_DATE) >= to_date(\'" + bulkSearch.FromDate.Value.ToString("MM/dd/yyyy") + "\','mm/dd/yyyy') ");
                isAndNeeded = true;
            }
            if (bulkSearch.ExcludeProcessed)
            {
                if (isAndNeeded)
                    queryBuilder.Append(" and   ");
                queryBuilder.Append("  i.PROCESSED_TO_AVYX <> '" + LOVConstants.BulkOrderStatus.Completed + "' ");
                queryBuilder.Append("  and  i.PROCESSED_TO_AVYX <> '" + LOVConstants.BulkOrderStatus.Awaiting + "' ");
            }
            if (!bulkSearch.ExcludeProcessed)
            {
                if (isAndNeeded)
                    queryBuilder.Append(" and   ");
                queryBuilder.Append(" ( i.PROCESSED_TO_AVYX = '" + LOVConstants.BulkOrderStatus.Completed + "' ");
                queryBuilder.Append("  or  i.PROCESSED_TO_AVYX = '" + LOVConstants.BulkOrderStatus.Awaiting + "'  or  i.PROCESSED_TO_AVYX = '" + LOVConstants.BulkOrderStatus.Error + "'  or  i.PROCESSED_TO_AVYX = '" + LOVConstants.BulkOrderStatus.Pending + "' or  i.PROCESSED_TO_AVYX = '" + LOVConstants.BulkOrderStatus.Processed + "' or  i.PROCESSED_TO_AVYX = '" + LOVConstants.BulkOrderStatus.Initiate + "')");
            }
            queryBuilder.Append(") d Where d.r=1");
            //queryBuilder.Append(" order by i.KA_BULK_NBR ");



            IDataReader reader = ExecuteReader(queryBuilder.ToString());

            var result = (new DbHelper()).ReadData<BulkOrderDetail>(reader);
            return result;
        }


        public List<BulkOrderDetail> GetBulkOrderDetail(string bulkNo, string programSource, bool withErr = true)
        {
            string query = "";
            if (programSource == KAProgramSource.ISS2165.ToString())
            {
                query = " SELECT i.KA_BULK_NBR \"BulkNumber\", i.CORP_BUSINESS_UNIT \"BusinessUnit\", nvl(i.LW_VENDOR_NO,0) \"VendorNo\", i.LW_VENDOR_LOC \"LwVendorLoc\", i.MFG_PATH_ID \"MFGPathId\", nvl(l.lw_vendor_vname,'') \"VendorName\", " +
                   " i.DEMAND_LOC \"DcLoc\", i.ISS_PROGRAM_TYPE_CD \"ProType\", i.SEASON_YEAR \"SeasonYear\", i.season_year||'^'||i.season_name \"Season\", i.TRANSP_MODE_CD \"TranspMode\", i.CONTACT_PLANNER_CD  \"PlanningContact\", i.CREATE_DATE \"CreatedOn\", " +
                   "i.DETAIL_TRKG_IND \"DetailTrkgIndVal\", (nvl(i.OVER_PCT,0) * 100) \"OverPercentageD\", nvl(i.UNDER_PCT,0)* 100  \"UnderPercentageD\", i.KA_LINE_NBR \"LineNumber\", i.STYLE_CD \"Style\", i.COLOR_CD \"Color\", i.REQSN_STATUS_CD \"ReqStatus\", i.PLANT_CD \"Plant\", " +
                   "i.ATTRIBUTE_CD \"Attribute\", i.SIZE_CD \"Size\", s.size_short_desc \"SizeLit\", i.MFG_REVISION_NO \"Revision\", decode( i.PROCESSED_TO_OS,'I','DZ',i.UNIT_OF_MEASURE) \"Uom\", i.SRC_CONTACT_CD \"SourcingContact\", i.PRODUCTION_STATUS  \"ProdStatus\"," +
                   "trunc( i.CURR_ORDER_QTY/12,0)+ mod(i.CURR_ORDER_QTY,12)/100 \"Qty\", i.CURR_DUE_DATE \"CurrDueDate\", i.PROCESSED_TO_OS \"ProcessedToOS\", i.REQSN_VERSION \"RequisitionVersion\", nvl(i.REQSN_ID,0) \"RequisitionId\" , i.APPRV_RESPONSE_DATE \"ApproverResponseDate\", " +
                   " i.REQSN_CREATE_DATE \"ReqCreateDate\", nvl(i.VENDOR_ID,0) \"VendorIdStr\", nvl(i.VENDOR_SUFFIX,0) \"VendorSuffix\", i.ISS_ORDER_TYPE_CD \"OrderType\", i.REQSN_APPROVER_CD \"RequisitionApprover\", nvl(i.LW_COMPANY,0)  \"LwCompany\", \'" + KAProgramSource.ISS2165.GetDescription() + "\' ProgramSourceDesc, \'" + KAProgramSource.ISS2165.ToString() + "\' ProgramSource " +
                   " From ka_preprocessor i, item_size s, lawson_vendor l" +
                   " WHERE  i.size_cd = s.size_cd(+) and RTRIM(i.KA_BULK_NBR) = '" + Val(bulkNo.TrimEnd()) + "'  and i.lw_vendor_no = l.lw_vendor_no(+) and i.lw_vendor_loc = l.lw_vendor_loc(+) " +
                   " order by i.KA_BULK_NBR ";
            }
            else
            {
                query = " SELECT i.KA_BULK_NBR \"BulkNumber\", i.CORP_BUSINESS_UNIT \"BusinessUnit\", nvl(k.LW_VENDOR_NO,0) \"VendorNo\", k.LW_VENDOR_LOC \"LwVendorLoc\", k.MFG_PATH_ID \"MFGPathId\", nvl(l.lw_vendor_vname,'') \"VendorName\", " +
               " i.DEMAND_LOC \"DcLoc\", '' \"ProType\", '' \"SeasonYear\", '' \"Season\", '' \"TranspMode\", k.CONTACT_PLANNER_CD  \"PlanningContact\", i.CREATE_DATE \"CreatedOn\", " +
               "'' \"DetailTrkgIndVal\", 0 \"OverPercentageD\", 0  \"UnderPercentageD\", k.KA_LINE_NBR \"LineNumber\", i.KA_STYLE_CD \"Style\", i.KA_COLOR_CD \"Color\", '' \"ReqStatus\", '' \"Plant\", " +
               "i.KA_ATTRIBUTE_CD \"Attribute\", i.KA_SIZE_CD \"Size\", s.size_short_desc \"SizeLit\", '' \"Revision\", nvl(k.UNIT_OF_MEASURE,'DZ') \"Uom\", '' \"SourcingContact\", ''  \"ProdStatus\"," +
               "trunc( i.CURR_ORDER_QTY/12,0)+ mod(i.CURR_ORDER_QTY,12)/100 \"Qty\", k.CURR_DUE_DATE \"CurrDueDate\", i.PROCESSED_TO_AVYX \"ProcessedToOS\", 0 \"RequisitionVersion\", 0 \"RequisitionId\" , " +
               "    '' \"OrderType\", '' \"RequisitionApprover\", 0  \"LwCompany\", \'" + KAProgramSource.ISS2166.GetDescription() + "\' ProgramSourceDesc, \'" + KAProgramSource.ISS2166.ToString() + "\' ProgramSource,  " +
               "i.APS_STYLE_CD \"APSStyle\",  i.APS_COLOR_CD \"APSColor\", i.APS_ATTRIBUTE_CD \"APSAttribute\", i.APS_SIZE_CD \"APSSize\", sz.size_short_desc \"APSSizeLit\", i.DEMAND_WEEKEND_DATE \"DemandWeekEndDate\", i.DEMAND_SOURCE  \"DemandSource\", i.PRIORITY_SEQ  \"PrioritySeq\", i.USER_ID  \"UserId\" " +
               " From KA_COMPONENT_PREPROCESSOR i, item_size s, lawson_vendor l, KA_BULK_ERRORS k, item_size sz " +
               " WHERE  i.KA_SIZE_CD = s.size_cd(+) and i.APS_SIZE_CD = sz.size_cd(+) and RTRIM(i.KA_BULK_NBR) = '" + Val(bulkNo.TrimEnd()) + "'  and k.lw_vendor_no = l.lw_vendor_no(+) and k.lw_vendor_loc = l.lw_vendor_loc(+) and i.KA_BULK_NBR = k.KA_BULK_NBR(+) " +
               " and i.APS_SIZE_CD = k.APS_SIZE_CD(+)  and i.APS_STYLE_CD = k.APS_STYLE_CD(+) and i.APS_COLOR_CD = k.APS_COLOR_CD(+) and i.APS_ATTRIBUTE_CD = k.APS_ATTRIBUTE_CD(+) " +
               " order by i.KA_BULK_NBR ";
            }

            IDataReader reader = ExecuteReader(query);
            var bulkOrdDtl = (new DbHelper()).ReadData<BulkOrderDetail>(reader);

            if (withErr)
            {
                string querydetail = " SELECT i.KA_BULK_NBR \"BulkNumber\", i.KA_LINE_NBR \"LineNumber\", i.KA_STYLE_CD \"Style\", i.DEMAND_LOC \"DcLoc\", i.CURR_DUE_DATE \"CurrDueDate\", i.CORP_BUSINESS_UNIT \"BusinessUnit\", nvl(i.LW_COMPANY,'0') \"LwCompany\", " +
                   " nvl(i.LW_VENDOR_NO,0) \"VendorNo\", i.LW_VENDOR_LOC \"LwVendorLoc\", i.CURR_ORDER_QTY \"Qty\", i.PROCESSED_IND \"ProcessedToOS\", i.ERROR_MESSAGE  \"Exception\", i.CREATE_DATE \"CreatedOn\", i.PROGRAM_SOURCE \"ProgramSource\"  " +
                   " From ka_bulk_errors i " +
                   " WHERE RTRIM(i.KA_BULK_NBR) = '" + Val(bulkNo.TrimEnd()) + "'  " +
                   " order by i.KA_BULK_NBR ";

                reader = ExecuteReader(querydetail);
                var bulkOrdError = (new DbHelper()).ReadData<BulkOrderDetail>(reader);

                if (bulkOrdDtl != null) bulkOrdDtl.ForEach(e =>
                {
                    e.IsInserted = true;
                    if (e.ProcessedToOS == LOVConstants.BulkOrderStatus.Error)
                    {
                        e.ErrorStatus = true;
                        e.ErrorMessage = e.Exception = string.Join(", ", bulkOrdError.Where(x => x.Exception != null && e.LineNumber == x.LineNumber).Select(x => Convert.ToString(x.Exception)).ToArray());
                    }
                    //e.ProgramSource = bulkOrdError.Select(x => x.ProgramSource).FirstOrDefault();
                    //if (!string.IsNullOrEmpty(e.ProgramSource))
                    //    e.ProgramSourceDesc = Enum.GetName(typeof(KAProgramSource), e.ProgramSource);


                });
            }
            return bulkOrdDtl;
        }
        public String GetLineNumber(List<BulkOrderDetail> BulkOrderDetail)
        {
            List<String> LineNumber = BulkOrderDetail.Where(s => !String.IsNullOrEmpty(s.LineNumber)).Select(e => e.LineNumber).Distinct().ToList();
            string lineNumber = ISS.Common.Utility.SequenceGenerator.CurrentValue(LineNumber);

            return ISS.Common.Utility.SequenceGenerator.NextValue(lineNumber);

        }


        
    }
}
