using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISS.DAL;
using ISS.Common;
using System.Data;
using ISS.Common;
using ISS.Core.Model.Common;
using ISS.Core.Model.Order;
using System.Diagnostics;
using Oracle.DataAccess.Client;
using System.Transactions;


namespace ISS.Repository.Order
{
    public partial class SourceOrderRepository : RepositoryBase
    {
        public IList<RequisitionOrder> GetRequisitionOrder(RequisitionOrderSearch search)
        {

            StringBuilder query = new StringBuilder(string.Empty);

            query.Append(" SELECT x.order_version \"OrderVersion\", x.super_order \"SuperOrder\", decode(lw_vendor_no,0,null,lw_vendor_no)  \"VendorNo\", lw_vendor_loc \"VendorLoc\",  x.style_cd \"Style\", x.color_cd \"Color\", x.attribute_cd \"Attribute\", x.size_lit \"SizeLit\", x.mfg_revision_no \"Rev\", x.unit_of_measure \"UOM\"," +
                " trunc( x.orig_order_qty/12.0,0 )+ mod(x.orig_order_qty,12)/100 \"Qty\", x.sew_plant \"SewPlant\", x.demand_loc \"Dc\", x.sew_start_date \"SewDate\", x.curr_due_date \"DcDate\",x.enforcement \"Enforcement\", x.rule_number \"RuleNo\", x.demand_source \"RuleDescription\", x.priority \"Priority\",   x.reqsn_id \"RequisitionId\", x.reqsn_version \"RequisitionVer\", x.size_cd \"Size\", x.style_desc \"StyleDesc\", production_status \"ProductionStatus \", trunc(x.std_case_qty/12,0)+ mod(x.std_case_qty,12)/100 \"StdCaseQty\", capacity_alloc \"WorkCenter\",planner_cd \"Planner\", x.earliest_start \"EarliestStart\", x.plan_date  \"PlanDate\" FROM ( ");

            //Inner query
            query.Append("select order_version, super_order, nvl(lw_vendor_no,0) lw_vendor_no, lw_vendor_loc, a.style_cd, a.color_cd, a.attribute_cd, b.size_short_desc size_lit, a.mfg_revision_no, a.unit_of_measure, curr_order_qty as orig_order_qty, a.sew_plant, a.demand_loc, (select curr_due_date From iss_prod_order_capacity where order_version=a.order_version and super_order = a.super_order)  sew_start_date, curr_due_date, decode(enforcement,11,'GS',3,'S','') as enforcement, rule_number, nvl(demand_source,'Create Lot') demand_source, a.priority, reqsn_id, reqsn_version, a.size_cd, s.style_desc, production_status, sr.std_case_qty, capacity_alloc,planner_cd, a.earliest_start, a.plan_date ");
            query.Append(" FROM iss_prod_order_view a, item_size b, style s, sku_revision sr ");
            query.Append("WHERE order_version = " + LOVConstants.GlobalOrderVersion + " ");

            if (!string.IsNullOrWhiteSpace(search.Planner)) query.Append(" AND PLANNER_CD = '" + Val(search.Planner) + "' ");
            if (!string.IsNullOrWhiteSpace(search.WorkCenter)) query.Append(" and CAPACITY_ALLOC= '" + Val(search.WorkCenter) + "' ");
            //if (!string.IsNullOrWhiteSpace(search.WorkCenter)) query.Append(" and RULE_NUMBER= '" + Val(search.Color) + "' ");
            if (!string.IsNullOrWhiteSpace(search.Style)) query.Append(" and a.SELLING_STYLE_CD like '" + Val(search.Style) + "%' ");
            if (!string.IsNullOrWhiteSpace(search.Color)) query.Append(" and a.SELLING_COLOR_CD like '" + Val(search.Color) + "%' ");
            if (!string.IsNullOrWhiteSpace(search.Attribute)) query.Append(" and a.SELLING_ATTRIBUTE_CD like '" + Val(search.Attribute) + "%' ");
            if (!string.IsNullOrWhiteSpace(search.SewPlant)) query.Append(" and SEW_PLANT= '" + Val(search.SewPlant) + "' ");
            if (!string.IsNullOrWhiteSpace(search.Size)) query.Append(" and SIZE_SHORT_DESC in (" + FormatInClause(Val(search.Size)) + ") ");
            if (!string.IsNullOrWhiteSpace(search.Dc)) query.Append(" and DEMAND_LOC= '" + Val(search.Dc) + "' ");



            query.Append(" AND  production_status in ('P')  and make_or_buy_cd = 'B'  and a.style_cd = s.style_cd  and a.size_cd = b.size_cd  and sr.style_cd = a.style_cd  and sr.color_cd = a.color_cd  and sr.attribute_cd = a.attribute_cd  and sr.size_cd = a.size_cd  and sr.revision_no = a.mfg_revision_no ");
            // Inner Query End


            query.Append(")  X , hbi_fiscal_calendar c1 , hbi_fiscal_calendar c2");

            ISS.Repository.Common.ApplicationRepository app = new Common.ApplicationRepository();
            var PlanDate = app.GetPlanDate();
            if (PlanDate.Count > 0)
            {
                query.Append(" WHERE trunc(x.earliest_start) = c1.calendar_date   and c2.calendar_date = trunc(to_date('" + PlanDate[0].Week_Begin_Date.ToString(LOVConstants.DateFormatOracle) + "','YYYYMMDD')) ");
            }

            if (search.SewDate.HasValue) query.Append("  AND  X.SEW_START_DATE  = to_date('" + search.SewDate.Value.ToString(LOVConstants.DateFormatDisplay) + "' ,'mm/dd/yyyy') ");
            query.Append(" and ((((c1.week_end_date - 1) - (c2.week_end_date - 1))/7)+1) <= " + search.SuggWO + "  ");

            query.Append(" order by style_cd, color_cd, attribute_cd, x.size_cd, rule_number, demand_loc, plan_date");


            Debug.WriteLine(query.ToString());

            IDataReader reader = ExecuteReader(query.ToString());

            var result = (new DbHelper()).ReadData<RequisitionOrder>(reader);
            return result;

        }


        public IList<VendorSearch> GetVendorDetails(Decimal lwNo, String lwLoc, String businessUnit)
        {

            StringBuilder query = new StringBuilder(string.Empty);

            query.Append("  select l.lw_vendor_vname  \"VendorName\",l.lw_vendor_city_addr5  \"VendorCity\",l.lw_vendor_country  \"LwVendorCountry\",bu.lw_company  \"LwCompany\",l.lw_vendor_no  \"LwVendorNo\",l.lw_vendor_loc  \"LwVendorLoc\",sv.vendor_id  \"VendorId\",sv.vendor_suffix  \"VendorSuffix\", pl.src_plant_cd  \"SrcPlant\"  from SVM_ISS_POREADY_FACILITIES sv, lawson_vendor l, corp_business_unit bu, Src_Vendor_Plant_Xref pl "
             + " where sv.lw_vendor_no = l.lw_vendor_no and sv.lw_vendor_loc = l.lw_vendor_loc and sv.src_division_cd = bu.corp_business_unit ");


            if (!string.IsNullOrWhiteSpace(businessUnit)) query.Append("  and bu.corp_business_unit = '" + Val(businessUnit) + "' ");
            query.Append(" and sv.vendor_id = pl.vendor_id and sv.vendor_suffix = pl.vendor_suffix and pl.src_division_cd = bu.corp_business_unit and sv.vendor_id = pl.vendor_id and sv.vendor_suffix = pl.vendor_suffix");
            query.Append(" and l.lw_vendor_no = " + (lwNo) + " ");
            if (!string.IsNullOrWhiteSpace(lwLoc)) query.Append(" and l.lw_vendor_loc= '" + Val(lwLoc) + "' ");




            Debug.WriteLine(query.ToString());

            IDataReader reader = ExecuteReader(query.ToString());

            var result = (new DbHelper()).ReadData<VendorSearch>(reader);
            return result;

        }


        public String getNewRequisitiuonId()
        {

            return (String)ExecuteFunction("OPRSQL.iss_reqsn_pkg.next_val", "RETURN_VALUE");
        }

        public List<string> SplitComment(List<string> comment)
        {
            List<string> plnrCmt = new List<string>();
            if (comment.Count > 0)
            {
                foreach (string s in comment)
                {
                    if (s != null)
                    {
                        if (s.Length > 50)
                        {
                            var pcmt = s.SplitByLength(50);
                            if (pcmt.ToArray().Length > 0)
                            {
                                plnrCmt.AddRange(pcmt);
                            }
                        }
                        else
                        {
                            plnrCmt.Add(s);
                        }
                    }
                }
            }

            return plnrCmt;
        }

        public bool AddOrderComment(OrderComment comment)
        {
            bool isSuccess = false;
            try
            {
                //comment.PlannerCommentLst.RemoveAll(str => String.IsNullOrEmpty(str));
                //comment.ApproverCommentLst.RemoveAll(str => String.IsNullOrEmpty(str));

                int prqCount = 0;
                if (comment.PlannerCommentLst != null) prqCount = comment.PlannerCommentLst.Count;
                int arqCount = 0;
                if (comment.ApproverCommentLst != null) arqCount = comment.ApproverCommentLst.Count;
                if (prqCount > 0 || arqCount > 0)
                {
                    string appComment = (comment.ApproverCommentLst != null) ? String.Join("|", SplitComment(comment.ApproverCommentLst)) : string.Empty;
                    string plannrComment = (comment.PlannerCommentLst != null) ? String.Join("|", SplitComment(comment.PlannerCommentLst)) : string.Empty;
                    arqCount = appComment.Count(x => x == '|') + 1;
                    prqCount = plannrComment.Count(x => x == '|') + 1;

                    var tt = ExecuteProcedure("OPRSQL.TM_ISS_REQSN_NOTE.add_note",
                        new OracleParameter()
                        {
                            ParameterName = "in_arq_count",
                            Value = arqCount,
                        },
                        new OracleParameter()
                        {
                            ParameterName = "in_prq_count",
                            Value = prqCount,
                        },
                        new OracleParameter()
                        {
                            ParameterName = "in_version",
                            Value = 1,
                        },
                        new OracleParameter()
                        {
                            ParameterName = "in_id",
                            Value = comment.RequisitionId,
                        },
                        new OracleParameter()
                        {
                            ParameterName = "in_arq_note",
                            Value = Val(appComment),
                        },
                        new OracleParameter()
                        {
                            ParameterName = "in_prq_note",
                            Value = Val(plannrComment),
                        }
                        );
                }
                isSuccess = true;
            }
            catch (Exception ee)
            {
                Log(ee);
            }
            return isSuccess;
        }

        public bool VerifyVendor(SKU sku, Decimal LwCompany, Decimal VendorNo, String LwVendorLoc)
        {

            //	Style / Color / Attribute / Size Code / Revision / Lawson Company / Vendor Number / Vendor Location
            //select oprsql.iss_reqsn_validate.verifyVendor('09453','B5V','------','NF',0,3850,62831,'ER02') from dual

            var queryBuilder = new StringBuilder();
            queryBuilder.Append(" select oprsql.iss_reqsn_validate.verifyVendor('" + Val(sku.Style) + "','" + Val(sku.Color) + "','" + Val(sku.Attribute) + "','" + (sku.Size) + "'," + (sku.Rev) + "," + (LwCompany) + "," + (VendorNo) + ",'" + Val(LwVendorLoc) + "') from dual ");

            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            var result = (String)ExecuteScalar(queryBuilder.ToString());

            return (result != null && result == "Y") ? true : false;
        }

        public bool VerifyDCMfgpath(SKU sku, String DCorSewPlant)
        {

            //              i.	Style / Color / Attribute / Size Code / Revision / DC|SewPlant
            //select oprsql.iss_reqsn_validate.verifyMfgPath('09453','B5V','------','NF',0,'LA') from dual

            var queryBuilder = new StringBuilder();
            queryBuilder.Append(" select oprsql.iss_reqsn_validate.verifyMfgPath('" + Val(sku.Style) + "','" + Val(sku.Color) + "','" + Val(sku.Attribute) + "','" + (sku.Size) + "'," + (sku.Rev) + ", '" + Val(DCorSewPlant) + "') from dual ");

            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            var result = (String)ExecuteScalar(queryBuilder.ToString());

            return (result != null && result == "Y") ? true : false;
        }

        public bool VerifySKUDCCombination(SKU sku, string mfgPathId)
        {

            var queryBuilder = new StringBuilder();
            queryBuilder.Append(" select make_or_buy_cd  \"MakeOrBuyCode\" from mfg_path where style_cd = '" + Val(sku.Style) + "' and color_cd =  '" + Val(sku.Color) + "' and attribute_cd = '" + Val(sku.Attribute) + "' and size_cd = '" + (sku.Size) + "' and mfg_path_id = '" + Val(mfgPathId) + "'");

            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            IDataReader reader = ExecuteReader(queryBuilder.ToString());

            var result = (new DbHelper()).ReadData<RequisitionDetail>(reader);

            return (result.Count > 0) ? true : false;
        }

        public bool VerifyRevision(SKU sku, string mfgPathId)
        {

            var queryBuilder = new StringBuilder();
            queryBuilder.Append(" select make_or_buy_cd  \"MakeOrBuyCode\" from mfg_path where style_cd = '" + Val(sku.Style) + "' and color_cd =  '" + Val(sku.Color) + "' and attribute_cd = '" + Val(sku.Attribute) + "' and size_cd = '" + (sku.Size) + "' and mfg_path_id = '" + Val(mfgPathId) + "' and revision_no = '" + sku.Rev + "'");

            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            IDataReader reader = ExecuteReader(queryBuilder.ToString());

            var result = (new DbHelper()).ReadData<RequisitionDetail>(reader);

            return (result.Count > 0) ? true : false;
        }

        public List<RequisitionOrder> GetPlanningleadTime(SKU sku, String DC, String SewPlant)
        {


            var queryBuilder = new StringBuilder();

            queryBuilder.Append(" select nvl(sum(p.PLANNING_LEADTIME),0) PlannedLeadTime, nvl(min(DAYS_IN_TRANSIT),0) TransportationTime  from mfg_path m, activity_routing r, planning_leadtime p, transportation t  ");


            queryBuilder.Append(" where m.style_cd = '" + Val(sku.Style) + "' ");

            if (!String.IsNullOrWhiteSpace(sku.Color))
            {
                queryBuilder.Append("and m.color_cd = '" + Val(sku.Color) + "' ");
            }

            if (!String.IsNullOrWhiteSpace(sku.Attribute))
            {
                queryBuilder.Append("and attribute_cd = '" + Val(sku.Attribute) + "' ");
            }

            if (!String.IsNullOrWhiteSpace(sku.Size))
            {
                queryBuilder.Append("and m.size_cd = '" + Val(sku.Size) + "' ");
            }
            if (!String.IsNullOrWhiteSpace(SewPlant))
            {
                queryBuilder.Append("and m.mfg_path_id  = '" + Val(SewPlant) + "' ");
            }

            queryBuilder.Append("and m.revision_no  = " + (sku.Rev) + " ");


            queryBuilder.Append("  and m.routing_id = r.routing_id and r.activity_cd = p.activity_cd   and r.location_cd = p.location_cd and m.prime_mfg_location = t.from_plant_cd (+)    ");

            if (!String.IsNullOrWhiteSpace(DC))
            {
                queryBuilder.Append(" and '" + (DC) + "'= t.to_plant_cd (+)   ");
            }

            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            IDataReader reader = ExecuteReader(queryBuilder.ToString());

            var result = (new DbHelper()).ReadData<RequisitionOrder>(reader);
            return result;
        }



        public List<RequisitionDetail> FillOrderAdditinalDetails(SKU sku, String DC)
        {

            // select a.mfg_path_id,a.prime_mfg_location,a.routing_id,a.bill_of_mtrls_id,nvl(a.scrap_factor,0),nvl(b.planner_cd,'UNK'),nvl(a.make_or_buy_cd,'M'),nvl(s.unit_of_measure,'EA'),nvl(b.lead_time,0)*7,nvl(a.REVISION_NO,0), nvl(p.capacity_alloc,'')  from MFG_PATH a , MFG_PATH_CHP b ,style s, prod_family p where a.style_cd = '09453' and a.color_cd = '150' and a.attribute_cd = '------' and a.size_cd = 'NF' and a.mfg_path_id = 'KM' and a.revision_no = 0 
            //and a.effect_end_date >=sysdate  and a.style_cd = b.style_cd (+)  AND a.color_cd = b.color_cd (+)  AND a.attribute_cd = b.attribute_cd (+)  AND a.size_cd  = b.size_cd (+)  AND a.mfg_path_id = b.mfg_path_id (+)  and a.revision_no = b.revision_no (+) AND a.style_cd  = s.style_cd (+)   and s.corp_prod_family_cd = p.prod_family_cd (+) order by a.mfg_path_id, a.prime_mfg_location

            var queryBuilder = new StringBuilder();
            // changed table from mfg_path a to MFG_PATH_MFG_REV a
            queryBuilder.Append(" select a.mfg_path_id \"MfgPathId\",a.prime_mfg_location \"MfgLoc\",a.routing_id \"RoutinId\",a.bill_of_mtrls_id \"BillOfMATL\",nvl(a.scrap_factor,0) \"ScrapFactor\",nvl(b.planner_cd,'UNK') \"Planner\",nvl(a.make_or_buy_cd,'M') \"MakeOrBuy\",nvl(s.unit_of_measure,'EA') UOM,nvl(b.lead_time,0)*7 \"PlanningLeadTime\",nvl(a.REVISION_NO,0) \"Revision\", nvl(p.capacity_alloc,'') \"CapacityAlloc\", p.prod_family_cd \"ProdFamilyCd\" , matl_type_cd \"MatlCd\",asrmt_cd \"AsrmtCd\""
                + " from MFG_PATH a , MFG_PATH_CHP b ,style s, prod_family p where  ");


            queryBuilder.Append("  a.style_cd = '" + Val(sku.Style) + "' ");

            if (!String.IsNullOrWhiteSpace(sku.Color))
            {
                queryBuilder.Append("and a.color_cd = '" + Val(sku.Color) + "' ");
            }

            if (!String.IsNullOrWhiteSpace(sku.Attribute))
            {
                queryBuilder.Append("and a.attribute_cd = '" + Val(sku.Attribute) + "' ");
            }

            if (!String.IsNullOrWhiteSpace(sku.Size))
            {
                queryBuilder.Append("and a.size_cd = '" + Val(sku.Size) + "' ");
            }
            if (!String.IsNullOrWhiteSpace(DC))
            {
                //queryBuilder.Append("and a.mfg_path_id  = '" + Val(DC) + "' "); Asif
                queryBuilder.Append("and a.mfg_path_id  = '" + Val(DC) + "' ");
            }

            queryBuilder.Append("and a.revision_no  = " + (sku.Rev) + " ");


            queryBuilder.Append("  and a.effect_end_date >=sysdate  and a.style_cd = b.style_cd (+)  AND a.color_cd = b.color_cd (+)  AND a.attribute_cd = b.attribute_cd (+)  AND a.size_cd  = b.size_cd (+)  AND a.mfg_path_id = b.mfg_path_id (+)  and a.revision_no = b.revision_no (+) AND a.style_cd  = s.style_cd (+)   and s.corp_prod_family_cd = p.prod_family_cd (+)    ");

            queryBuilder.Append(" order by a.mfg_path_id, a.prime_mfg_location ");
            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            IDataReader reader = ExecuteReader(queryBuilder.ToString());

            var result = (new DbHelper()).ReadData<RequisitionDetail>(reader);
            return result;
        }

        public List<RequisitionDetail> FillOrderAdditinalFabricDetails(SKU sku, String DC)
        {
            //select a.resource_id,upper(x.FABRIC_GROUP)   from iss_garment_resource a, iss_style_fabric_xref x where a.style_cd = '09453' and a.color_cd = '150'
            //and a.attribute_cd = '------' and a.size_cd = 'NP' and a.mfg_path_id = 'KM' and a.style_cd = x.style_cd (+)
            var queryBuilder = new StringBuilder();

            queryBuilder.Append(" select a.resource_id \"sss\",upper(x.FABRIC_GROUP) \"FabricGroup\"   from iss_garment_resource a, iss_style_fabric_xref x where   ");


            queryBuilder.Append("  a.style_cd = '" + Val(sku.Style) + "' ");

            if (!String.IsNullOrWhiteSpace(sku.Color))
            {
                queryBuilder.Append("and a.color_cd = '" + Val(sku.Color) + "' ");
            }

            if (!String.IsNullOrWhiteSpace(sku.Attribute))
            {
                queryBuilder.Append("and a.attribute_cd = '" + Val(sku.Attribute) + "' ");
            }

            if (!String.IsNullOrWhiteSpace(sku.Size))
            {
                queryBuilder.Append("and a.size_cd = '" + Val(sku.Size) + "' ");
            }
            if (!String.IsNullOrWhiteSpace(DC))
            {
                queryBuilder.Append("and a.mfg_path_id  = '" + Val(DC) + "' ");
            }

            queryBuilder.Append("  and a.style_cd = x.style_cd (+)   ");

            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            IDataReader reader = ExecuteReader(queryBuilder.ToString());

            var result = (new DbHelper()).ReadData<RequisitionDetail>(reader);
            return result;
        }


        public String GetStyleDesc(String styleCode)
        {
            string query = " select style_desc from style  where style_cd = '" + Val(styleCode) + "' ";
            return (String)ExecuteScalar(query);

        }

        #region Validate Requisition

        public bool validateStyleBeforeSOSave(String styleCode)
        {

            //select 'Y' from style where style_cd = '09453' AND MATL_TYPE_CD IN ('00','01','02')

            var queryBuilder = new StringBuilder();
            queryBuilder.Append(" select 'Y' from style where style_cd = '" + Val(styleCode) + "' AND MATL_TYPE_CD IN ('" + LOVConstants.MATL_TYPE_CD.Garment + "','" + LOVConstants.MATL_TYPE_CD.Code1 + "','" + LOVConstants.MATL_TYPE_CD.Code2 + "') ");

            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            var result = (String)ExecuteScalar(queryBuilder.ToString());

            return (result != null && result == "Y") ? true : false;
        }

        public bool ValidateDetailBeforeSOSave(Requisition req, RequisitionDetail item, out String ErrMsg)
        {

            //SELECT oprsql.iss_prod_order_validate.verify(38,'SUPER_ORDER|ORDER_VERSION|ORDER_ID|PRODUCTION_STATUS|DEMAND_TYPE|ORIG_DUE_DATE|DOZENS_ONLY_IND|CREATE_BD_IND|PRIORITY|SPREAD_TYPE_CD|CORP_BUSINESS_UNIT|SEW_PATH|SEW_PATH_REQSN|CUT_PATH|TXT_PATH|MACHINE|CURR_ORDER_TOTAL_QTY|DUE_DATE|DEMAND_LOC|DOZENS_ONLY_IND|CREATE_BD_IND|REQSN_VERSION|REQSN_ID|PLAN_DATE|SCHED_SHIP_DATE|LW_COMPANY|LW_VENDOR_NO|LW_VENDOR_LOC|VENDOR_ID|VENDOR_SUFFIX|STYLE_CD|SIZE_CD|MAKE_OR_BUY_CD|ATTRIBUTE_CD|COLOR_CD|GARMENT_STYLE|SELLING_STYLE_CD|MFG_REVISION_NO|','H002501189|1|RQ29918|L|DD|20150513|||1380476||BRA|5Z|5Z||||696|20150513|KM|||1|RQ29918|20150213|20150401|3850|62831|ER02|10815|101|09453|NB|B|------|MFK|||0') from dual

            var queryBuilder = new StringBuilder();
            queryBuilder.Append("SELECT oprsql.iss_prod_order_validate.verify(40,'SUPER_ORDER|ORDER_VERSION|ORDER_ID|PRODUCTION_STATUS|DEMAND_TYPE|ORIG_DUE_DATE|DOZENS_ONLY_IND|CREATE_BD_IND|PRIORITY|SPREAD_TYPE_CD|CORP_BUSINESS_UNIT|SEW_PATH|SEW_PATH_REQSN|CUT_PATH|TXT_PATH|MACHINE|CURR_ORDER_TOTAL_QTY|DUE_DATE|DEMAND_LOC|DOZENS_ONLY_IND|CREATE_BD_IND|REQSN_VERSION|REQSN_ID|PLAN_DATE|SCHED_SHIP_DATE|LW_COMPANY|LW_VENDOR_NO|LW_VENDOR_LOC|VENDOR_ID|VENDOR_SUFFIX|STYLE_CD|SIZE_CD|MAKE_OR_BUY_CD|ATTRIBUTE_CD|COLOR_CD|GARMENT_STYLE|SELLING_STYLE_CD|MFG_REVISION_NO|DEMAND_SOURCE|DEMAND_DRIVER|','" + item.SuperOrder + "|" + req.OrderVersion + "|" + Val(req.RequisitionId) + "|" + Val(req.ProdStatus) + "|" + Val(req.DemandType) + "|" + ((req.OriginalDueDate.HasValue) ? req.OriginalDueDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty) + "|" + Val(item.DozensOnlyInd) + "|" + Val(item.CreateBDInd) + "|" + (item.Priority) + "|" + Val(req.SpreadTypeCD) + "|" + Val(req.BusinessUnit) + "|" + Val(req.MFGPathId) + "|" + Val(req.MFGPathId) + "||||" + (item.Qty) + "|" + ((item.CurrDueDate.HasValue) ? item.CurrDueDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty) + "|" + Val(req.DcLoc) + "|||" + item.RequisitionVer + "|" + Val(req.RequisitionId) + "|" + ((item.PlanDate.HasValue) ? item.PlanDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty) + "|" + ((item.ScheduledShipDate.HasValue) ? item.ScheduledShipDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty) + "|" + (req.LwCompany) + "|" + (req.VendorNo) + "|" + Val(req.LwVendorLoc) + "|" + (req.VendorId) + "|" + (req.VendorSuffix) + "|" + Val(item.Style) + "|" + Val(item.Size) + "|" + Val(item.MakeOrBuyCode) + "|" + Val(item.Attribute) + "|" + Val(item.Color) + "|" + Val(item.GarmentStyle) + "|" + Val(item.SellingStyle) + "|" + (item.Rev) + "|" + Val(item.DemandSource) + "|" + (item.DemandDriver) + "') from dual ");

            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            var result = (String)ExecuteScalar(queryBuilder.ToString());
            //||||||||||||||||||||||||||||||||||||||
            ErrMsg = (result != null) ? result.Replace("|", String.Empty).Trim() : String.Empty;

            return String.IsNullOrEmpty(ErrMsg);

        }

        #endregion

        #region Insert Requisition


        public KeyValuePair<bool, String> InsertRequisition(Requisition req, List<RequisitionDetail> items)
        {
            String Msg = "Required fields are missing.";
            bool Status = false;
            if (req != null && items != null && items.Count > 0)
            {
                try
                {
                    req.RequisitionVersion = req.OrderVersion = 1; // hard coded for insert
                    req.DemandType = LOVConstants.DemandType;
                    req.CurrentDueDate = req.DemandDate = req.OriginalDueDate = req.DCDueDate = req.PlannedDcDate;
                    req.ProdStatus = LOVConstants.ProductionStatus.Locked;
                    req.ReqStatus = LOVConstants.RequestStatus.UnderConstruction;
                    String ErrMsgValDet = String.Empty;
                    foreach (var item in items)
                    {
                        SKU sku = new SKU(item);
                        if (item.isHide)
                        {
                            continue;
                        }
                        if (GetSOSKUBDDetail(sku, req.MFGPathId))
                        {
                            item.CreateBDInd = LOVConstants.Yes;
                            item.DozensOnlyInd = LOVConstants.No;

                        }
                        else
                        {
                            item.DozensOnlyInd = LOVConstants.Yes;
                            item.CreateBDInd = LOVConstants.No;
                        }
                        Status = false;
                        if (!item.IsMovedObject && String.IsNullOrEmpty(item.SuperOrder))
                        {
                            item.OrderLabel = item.SuperOrder = getNewOrderLabel().ToString();
                            FillAdditionalDetails(req, item);
                        }


                        if (item.Qty <= 0) { Msg = "SKU " + sku.getSKUString() + ". Quantity must be greater than zero."; break; }
                        else if (item.Qty >= 300000.0m) { Msg = "SKU " + sku.getSKUString() + ". Quantity must be less than 300000."; break; }
                        //  else if (item.StdCase == 0.01m) { Msg = "SKU " + sku.getSKUString() + ". Please select the revision for get the std case qty."; break; }
                        else if (!VerifyDCMfgpath(sku, req.DcLoc)) { Msg = "SKU " + sku.getSKUString() + " and demand location must exist in PATH_DEST_PLANT."; break; }
                        else if (!VerifyDCMfgpath(sku, req.MFGPathId)) { Msg = "Invalid Style " + sku.Style + " and  Sew Plant " + req.MFGPathId; break; }
                        else if (!VerifyVendor(sku, req.LwCompany, req.VendorNo, req.LwVendorLoc)) { Msg = "Invalid vendor details SKU " + sku.getSKUString(); break; }
                        else if (!VerifySKUDCCombination(sku, req.MFGPathId)) { Msg = "This is not a valid SKU/DC combination. " + sku.getSKUString() + " - " + req.DcLoc; break; }
                        else if (!VerifyRevision(sku, req.MFGPathId)) { Msg = " - Revision Code " + sku.Rev + " is not valid for SKU/DC combination. " + sku.getSKUString(); break; }
                        else if (!validateStyleBeforeSOSave(item.Style)) { Msg = "Invalid style " + sku.Style; break; }
                        else if (!ValidateDetailBeforeSOSave(req, item, out ErrMsgValDet)) { Msg = ErrMsgValDet + " - Style " + sku.Style; break; }
                        else if (!GetStyleValidation(sku.Style, req.BusinessUnit)) { Msg = "Invalid Business Unit. " + sku.getSKUString(); break; }
                        else if (!GetDCValidation(req.DcLoc)) { Msg = "Invalid Dc Loc. " + sku.getSKUString(); break; }
                        // Asif New change
                        //   else if (req.MFGPathId != item.MfgPathId) { Msg = "Mfg Path " + item.MfgPathId + " for the line item is not same as Header MfgPath " + req.MFGPathId + " for the sku . " + sku.getSKUString(); break; }

                        //TBD BU and Style **
                        // dc validatio GetStyleValidation
                        else
                        {
                            Status = true;
                        }
                    }
                    if (Status)
                    {
                        using (TransactionScope scope = new TransactionScope())
                        {
                            try
                            {
                                BeginTransaction();
                                foreach (var item in items)
                                {

                                    item.RequisitionVer = req.RequisitionVersion; // hard coded for insert
                                    item.MakeOrBuyCode = LOVConstants.MakeOrBuy.Buy;
                                    if (req.ShowSummaryOnly)
                                        item.OrderType = LOVConstants.ISSOrderTypeCode.SummarizedRequisition;
                                    else
                                        item.OrderType = LOVConstants.ISSOrderTypeCode.NonSummarizedRequisition;


                                    item.DemandQty = item.TotatalCurrOrderQty = item.Qty = item.Qty.ConvertDzToEaches(); //Converted   into eaches

                                    if (item.isHide)
                                    {
                                        if (item.IsMovedObject && !String.IsNullOrEmpty(item.SuperOrder))
                                        {
                                            Status = DeleteOrder(item);
                                        }
                                    }
                                    else if (item.IsMovedObject)
                                    {
                                        Status = InsertOrder(req, item);
                                    }
                                    else
                                    {
                                        Status = InsertOrderManual(req, item);
                                    }

                                    if (!Status)
                                    {
                                        Status = false;
                                        Msg = "Falied to insert order detail SKU " + item.getSKUString();
                                        break;
                                    }

                                }
                                if (Status && insertRequisitionHeader(req))
                                {
                                    CommitTransaction();
                                    if (req.RequisitionComment != null)
                                    {
                                        req.RequisitionComment.RequisitionId = req.RequisitionId;
                                        req.RequisitionComment.PlannerCommentLst = new List<String>() { req.RequisitionComment.PlannerComment };
                                        req.RequisitionComment.ApproverCommentLst = new List<String>() { req.RequisitionComment.ApproverComment };

                                        AddOrderComment(req.RequisitionComment);
                                    }
                                    Msg = "Sourced Order created successfully.";
                                }
                                else
                                {
                                    Status = false;
                                    Msg = "Failed to insert requisition details.";
                                }
                            }
                            catch (Exception ES)
                            {
                                RollbackTransaction();
                                Log("Save reqquisition-trans scope");
                                Log(ES);
                                Msg = ES.Message;
                            }
                        }// END TRANS SCOPE
                    } // end save start
                }
                catch (Exception ee)
                {
                    Log(ee);
                    Msg = ee.Message;
                }
                finally
                {

                }

            }

            return new KeyValuePair<bool, string>(Status, Msg);
        }



        /// <summary>
        /// For Insert new requisition only
        /// </summary>
        /// <param name="req"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool InsertOrder(Requisition req, RequisitionDetail item)
        {
            //added from update req
            //BEGIN OPRSQL.ISS_PROD_ORDER_MAINT.UPDATE_ORDER(21,'SUPER_ORDER|ORDER_VERSION|ORDER_ID|PRODUCTION_STATUS|DEMAND_TYPE|ORIG_DUE_DATE|CORP_BUSINESS_UNIT|SEW_PATH|TOTAL_CURR_ORDER_QTY|CURR_DUE_DATE|REQSN_VERSION|REQSN_ID|PLAN_DATE|SCHED_SHIP_DATE|LW_COMPANY|LW_VENDOR_NO|LW_VENDOR_LOC|VENDOR_ID|VENDOR_SUFFIX|DEMAND_LOC|MAKE_OR_BUY_CD','H003022546|1|RQ30006|L|DD|20150610|BRA|KM|2496|20150610|1|RQ30006|20150424|20150429|3850|62831|ER02|10815|101|KM|B');END;


            // from insert requisition
            //OPRSQL.ISS_PROD_ORDER_MAINT.UPDATE_ORDER(21,'SUPER_ORDER|ORDER_VERSION|ORDER_ID|PRODUCTION_STATUS|DEMAND_TYPE|ORIG_DUE_DATE|CORP_BUSINESS_UNIT|TOTAL_CURR_ORDER_QTY|CURR_DUE_DATE|REQSN_VERSION|REQSN_ID|PLAN_DATE|SCHED_SHIP_DATE|LW_COMPANY|LW_VENDOR_NO|LW_VENDOR_LOC|VENDOR_ID|VENDOR_SUFFIX|MAKE_OR_BUY_CD','H002500939|1|RQ29918|L|DD|20150513|BRA|1044|20150513|1|RQ29918|20150213|20150401|3850|62831|ER02|10815|101|B');

            var queryBuilder = new StringBuilder();
            queryBuilder.Append(" Begin OPRSQL.ISS_PROD_ORDER_MAINT.UPDATE_ORDER(21,'SUPER_ORDER|ORDER_VERSION|ORDER_ID|PRODUCTION_STATUS|DEMAND_TYPE|ORIG_DUE_DATE|CORP_BUSINESS_UNIT|SEW_PATH|TOTAL_CURR_ORDER_QTY|CURR_DUE_DATE|REQSN_VERSION|REQSN_ID|PLAN_DATE|SCHED_SHIP_DATE|LW_COMPANY|LW_VENDOR_NO|LW_VENDOR_LOC|VENDOR_ID|VENDOR_SUFFIX|DEMAND_LOC|MAKE_OR_BUY_CD','" + item.SuperOrder
                + "|" + req.OrderVersion + "|" + Val(req.RequisitionId) + "|" + Val(req.ProdStatus) + "|" + Val(req.DemandType) + "|" +
                ((req.OriginalDueDate.HasValue) ? req.OriginalDueDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty) + "|" +
                Val(req.BusinessUnit) + "|" + Val(req.MFGPathId) + "|" + (item.Qty) + "|" +
                ((item.CurrDueDate.HasValue) ? item.CurrDueDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty)
                + "|" + item.RequisitionVer + "|" + Val(req.RequisitionId) + "|"
                + ((item.PlanDate.HasValue) ? item.PlanDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty) + "|"
                + ((item.ScheduledShipDate.HasValue) ? item.ScheduledShipDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty)
                + "|" + ((req.LwCompany)) + "|" + (req.VendorNo) + "|" + Val(req.LwVendorLoc)
                + "|" + (req.VendorId) + "|"
                + (req.VendorSuffix) + "|" + Val(req.DcLoc) + "|" + Val(item.MakeOrBuyCode)
                + "');END; ");

            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            var result = (String)ExecuteScalar(queryBuilder.ToString());

            return (result == null || result == "Y") ? true : false;
        }

        protected bool insertRequisitionHeader(Requisition req)
        {

            //BEGIN OPRSQL.ISS_REQSN_MAINT.INSERT_REQSN(31,'REQSN_VERSION|REQSN_ID|CREATE_DATE|UPDATE_DATE|CREATE_USER_ID|UPDATE_USER_ID|PRODUCTION_STATUS|REQSN_STATUS_CD|CONTACT_PLANNER_CD|SRC_CONTACT_CD|REQSN_APPROVER_CD|DEMAND_LOC|CURR_DUE_DATE|SEASON_YEAR|SEASON_NAME|ISS_PROGRAM_TYPE_CD|DETAIL_TRKG_IND|APPRV_SUBMIT_DATE|APPRV_RESPONSE_DATE|RLSE_TO_SRC_DATE|EXTR_BY_SRC_DATE|REJ_BY_SRC_DATE|TRANSP_MODE_CD|OVER_PCT|UNDER_PCT|CORP_BUSINESS_UNIT|VENDOR_ID|VENDOR_SUFFIX|LW_COMPANY|LW_VENDOR_NO|LW_VENDOR_LOC','1|RQ29918|20150213|20150213|oprsprod|oprsprod|L|UC|BENNA|TCR|PLANR|KM|20150401||||N||||||NS|0|0|BRA|10815|101|3850|62831|ER02');END;
            var SeasonYear = String.Empty;
            var SeasonName = String.Empty;
            if (!string.IsNullOrEmpty(req.Season))
            {
                var arr = req.Season.Split('^');
                if (arr.Length > 1)
                {
                    SeasonYear = arr[0];
                    SeasonName = arr[1];
                }
            }

            var queryBuilder = new StringBuilder();
            queryBuilder.Append(" BEGIN OPRSQL.ISS_REQSN_MAINT.INSERT_REQSN(31,'REQSN_VERSION|REQSN_ID|CREATE_DATE|UPDATE_DATE|CREATE_USER_ID|UPDATE_USER_ID|PRODUCTION_STATUS|REQSN_STATUS_CD|CONTACT_PLANNER_CD|SRC_CONTACT_CD|REQSN_APPROVER_CD|DEMAND_LOC|CURR_DUE_DATE|SEASON_YEAR|SEASON_NAME|ISS_PROGRAM_TYPE_CD|DETAIL_TRKG_IND|APPRV_SUBMIT_DATE|APPRV_RESPONSE_DATE|RLSE_TO_SRC_DATE|EXTR_BY_SRC_DATE|REJ_BY_SRC_DATE|TRANSP_MODE_CD|OVER_PCT|UNDER_PCT|CORP_BUSINESS_UNIT|VENDOR_ID|VENDOR_SUFFIX|LW_COMPANY|LW_VENDOR_NO|LW_VENDOR_LOC','1|" + Val(req.RequisitionId) + "|" +
                DateTime.Now.ToString(LOVConstants.DateFormatOracle) + "|" +
                DateTime.Now.ToString(LOVConstants.DateFormatOracle) + "|" + Val(req.CreatedBy) + "|" + Val(req.UpdatedBy) + "|" + Val(req.ProdStatus) +
                "|" + Val(req.ReqStatus) + "|"
                + Val(req.PlanningContact) + "|" + Val(req.SourcingContact) + "|" + Val(req.RequisitionApprover) + "|" + Val(req.DcLoc) + "|" +
                ((req.CurrentDueDate.HasValue) ? req.CurrentDueDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty)
                + "|" + Val(SeasonYear) + "|" + Val(SeasonName) + "|" + Val(req.ProType) + "|" + ((req.ReqDetailTracking) ? LOVConstants.Yes : LOVConstants.No) + "||||||" + Val(req.Mode) + "|" + ((req.OverPercentage.HasValue) ? (req.OverPercentage.Value / 100.0M) : 0) + "|" + ((req.UnderPercentage.HasValue) ? (req.UnderPercentage.Value / 100.0M) : 0) + "|" + Val(req.BusinessUnit) + "|"
                    + (req.VendorId) + "|" + (req.VendorSuffix) + "|" + (req.LwCompany) + "|" + (req.VendorNo) + "|" + Val(req.LwVendorLoc) + "');END; ");

            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            var result = (String)ExecuteScalar(queryBuilder.ToString());

            return (result == null || result == "Y") ? true : false;
        }

        #endregion


        #region Update Requisition

        public KeyValuePair<bool, String> UpdateRequisition(Requisition req, List<RequisitionDetail> items)
        {
            String Msg = "Required fields are missing.";
            bool Status = false;
            var requisitionData = GetRequisition(req.RequisitionId);
            if (requisitionData.Count > 0)
            {
                if (!(requisitionData[0].ProdStatus == LOVConstants.ProductionStatus.Locked && requisitionData[0].ReqStatus == LOVConstants.RequestStatus.UnderConstruction))
                {
                    Msg = "Not allowed to update this requisition.";
                    return new KeyValuePair<bool, string>(Status, Msg);
                }

            }
            if (items.Count(e => !e.IsDeleted) > 0)
            {

                //items = items.Where(e => e.IsDirty).ToList();
                if (req != null)
                {

                    if (items != null && items.Count() > 0)
                    {
                        try
                        {
                            // var Currreq =(List<RequisitionDetail>) GetRequisitionDetail(req.RequisitionId);
                            req.RequisitionVersion = req.OrderVersion = 1; // hard coded for insert
                            req.DemandType = LOVConstants.DemandType;
                            req.CurrentDueDate = req.DemandDate = req.OriginalDueDate = req.DCDueDate = req.PlannedDcDate;
                            req.ProdStatus = LOVConstants.ProductionStatus.Locked;
                            req.ReqStatus = LOVConstants.RequestStatus.UnderConstruction;
                            String ErrMsgValDet = String.Empty;
                            foreach (var item in items)
                            {
                                if (item.isHide) continue;
                                if (item.IsDeleted) { Status = true; continue; }
                                SKU sku = new SKU(item);

                                if (GetSOSKUBDDetail(sku, req.MFGPathId))
                                {
                                    item.CreateBDInd = LOVConstants.Yes;
                                    item.DozensOnlyInd = LOVConstants.No;
                                }
                                else
                                {
                                    item.DozensOnlyInd = LOVConstants.Yes;
                                    item.CreateBDInd = LOVConstants.No;
                                }
                                Status = false;
                                //IsMovedObject   moed from lower grid. Existing orders
                                //IsInserted  edit records
                                if (!item.IsMovedObject && !item.IsInserted && String.IsNullOrEmpty(item.SuperOrder))
                                {
                                    item.OrderLabel = item.SuperOrder = getNewOrderLabel().ToString();
                                    FillAdditionalDetails(req, item);
                                }
                                else if (item.IsInserted)
                                {
                                    FillAdditionalDetailsForEdit(req, item);
                                }


                                if (item.Qty <= 0) { Msg = "SKU " + sku.getSKUString() + ". Quantity must be greater than zero."; break; }
                                else if (item.Qty >= 300000.0m) { Msg = "SKU " + sku.getSKUString() + ". Quantity must be less than 300000."; break; }
                                //  else if (item.StdCase == 0.01m) { Msg = "SKU " + sku.getSKUString() + ". Please select the revision for get the std case qty."; break; }
                                else if (!VerifyDCMfgpath(sku, req.DcLoc)) { Msg = "SKU " + sku.getSKUString() + " and demand location must exist in PATH_DEST_PLANT."; break; }
                                else if (!VerifyDCMfgpath(sku, req.MFGPathId)) { Msg = "Invalid Style " + sku.Style + " and  Sew Plant " + req.MFGPathId; break; }
                                else if (!VerifyVendor(sku, req.LwCompany, req.VendorNo, req.LwVendorLoc)) { Msg = "Invalid vendor details SKU " + sku.getSKUString(); break; }
                                else if (!VerifySKUDCCombination(sku, req.MFGPathId)) { Msg = "This is not a valid SKU/DC combination. " + sku.getSKUString(); break; }
                                else if (!VerifyRevision(sku, req.MFGPathId)) { Msg = " - Revision Code " + sku.Rev + " is not valid for SKU/DC combination. " + sku.getSKUString() + " - " + req.DcLoc; ; break; }
                                else if (!validateStyleBeforeSOSave(item.Style)) { Msg = "Invalid style " + sku.Style; break; }
                                else if (!ValidateDetailBeforeSOSave(req, item, out ErrMsgValDet)) { Msg = ErrMsgValDet + " - Style " + sku.Style; break; }
                                else if (!GetStyleValidation(sku.Style, req.BusinessUnit)) { Msg = "Invalid Business Unit. " + sku.getSKUString(); break; }
                                else if (!GetDCValidation(req.DcLoc)) { Msg = "Invalid Dc Loc. " + sku.getSKUString(); break; }
                                //Asif new change
                                // else if (req.MFGPathId != item.MfgPathId) { Msg = "Mfg Path " + item.MfgPathId + " for the line item is not same as Header MfgPath " + req.MFGPathId +  " for the sku . " + sku.getSKUString(); break; }


                                    //TBD BU and Style **
                                // dc validatio
                                else
                                {
                                    Status = true;
                                }
                            }
                            if (Status)
                            {
                                using (TransactionScope scope = new TransactionScope())
                                {
                                    try
                                    {
                                        BeginTransaction();
                                        foreach (var item in items)
                                        {
                                            item.RequisitionId = req.RequisitionId;
                                            item.RequisitionVer = req.RequisitionVersion; // hard coded for insert
                                            item.MakeOrBuyCode = LOVConstants.MakeOrBuy.Buy;
                                            if (req.ShowSummaryOnly)
                                                item.OrderType = LOVConstants.ISSOrderTypeCode.SummarizedRequisition;
                                            else
                                                item.OrderType = LOVConstants.ISSOrderTypeCode.NonSummarizedRequisition;



                                            item.DemandQty = item.TotatalCurrOrderQty = item.Qty = item.Qty.ConvertDzToEaches();


                                            if (item.isHide)
                                            {
                                                if (!String.IsNullOrEmpty(item.SuperOrder))
                                                {
                                                    Status = DeleteOrder(item);
                                                }
                                            }
                                            else if (item.IsDeleted)
                                            {
                                                Status = DeleteOrder(item);
                                            }
                                            else if (item.IsMovedObject)
                                            {
                                                Status = InsertOrder(req, item);
                                            }
                                            else if (item.IsInserted)
                                            {
                                                Status = UpdateOrder(req, item);
                                            }
                                            else
                                            {
                                                Status = InsertOrderManual(req, item);
                                            }

                                            if (!Status)
                                            {
                                                Status = false;
                                                Msg = "Falied to insert order detail SKU " + item.getSKUString();
                                                break;
                                            }
                                        }
                                        if (Status && UpdateRequisitionHeader(req))
                                        {
                                            CommitTransaction();
                                            if (req.RequisitionComment != null)
                                            {
                                                req.RequisitionComment.RequisitionId = req.RequisitionId;
                                                // AddOrderComment(req.RequisitionComment);
                                            }
                                            Msg = "Sourced Order updated successfully.";
                                        }
                                        else
                                        {
                                            Status = false;
                                            Msg = "Failed to update Source Order.";
                                        }
                                    }
                                    catch (Exception ES)
                                    {
                                        RollbackTransaction();
                                        Log("Save reqquisition-trans scope");
                                        Log(ES);
                                        Msg = ES.Message;
                                    }
                                }// END TRANS SCOPE
                            }// validation completed and save starts end
                        }
                        catch (Exception ee)
                        {
                            Log(ee);
                            Msg = ee.Message;
                        }
                        finally
                        {

                        }
                    }
                    else
                    {
                        Msg = "No detail to update.";
                    }
                }
            }
            else
            {
                Msg = "Please enter at least one requisition detail.";
            }

            return new KeyValuePair<bool, string>(Status, Msg);
        }

        private void SetRequisitionDefaults(Requisition req)
        {
            req.RequisitionVersion = req.OrderVersion = 1; // hard coded for insert
            req.DemandType = LOVConstants.DemandType;
            req.CurrentDueDate = req.DemandDate = req.OriginalDueDate = req.DCDueDate = req.PlannedDcDate;
            req.ProdStatus = LOVConstants.ProductionStatus.Locked;
            req.ReqStatus = LOVConstants.RequestStatus.UnderConstruction;



        }
        private void SetRequisitionDetailDefaults(Requisition req, RequisitionDetail item)
        {
            //SKU sku = new SKU(item);

            //if (GetSOSKUBDDetail(sku, req.MFGPathId))
            //{
            //    item.CreateBDInd = LOVConstants.Yes;
            //    item.DozensOnlyInd = LOVConstants.No;
            //}
            //else
            //{
            //    item.DozensOnlyInd = LOVConstants.Yes;
            //    item.CreateBDInd = LOVConstants.No;
            //}

            item.RequisitionId = req.RequisitionId;
            item.RequisitionVer = req.RequisitionVersion; // hard coded for insert
            item.MakeOrBuyCode = LOVConstants.MakeOrBuy.Buy;
            if (req.ShowSummaryOnly)
                item.OrderType = LOVConstants.ISSOrderTypeCode.SummarizedRequisition;
            else
                item.OrderType = LOVConstants.ISSOrderTypeCode.NonSummarizedRequisition;



            item.DemandQty = item.TotatalCurrOrderQty = item.Qty;

        }


        protected bool UpdateRequisitionHeader(Requisition req)
        {

            //BEGIN OPRSQL.ISS_REQSN_MAINT.INSERT_REQSN(31,'REQSN_VERSION|REQSN_ID|CREATE_DATE|UPDATE_DATE|CREATE_USER_ID|UPDATE_USER_ID|PRODUCTION_STATUS|REQSN_STATUS_CD|CONTACT_PLANNER_CD|SRC_CONTACT_CD|REQSN_APPROVER_CD|DEMAND_LOC|CURR_DUE_DATE|SEASON_YEAR|SEASON_NAME|ISS_PROGRAM_TYPE_CD|DETAIL_TRKG_IND|APPRV_SUBMIT_DATE|APPRV_RESPONSE_DATE|RLSE_TO_SRC_DATE|EXTR_BY_SRC_DATE|REJ_BY_SRC_DATE|TRANSP_MODE_CD|OVER_PCT|UNDER_PCT|CORP_BUSINESS_UNIT|VENDOR_ID|VENDOR_SUFFIX|LW_COMPANY|LW_VENDOR_NO|LW_VENDOR_LOC','1|RQ29918|20150213|20150213|oprsprod|oprsprod|L|UC|BENNA|TCR|PLANR|KM|20150401||||N||||||NS|0|0|BRA|10815|101|3850|62831|ER02');END;

            //BEGIN OPRSQL.ISS_REQSN_MAINT.UPDATE_REQSN(6,'REQSN_VERSION|REQSN_ID|CURR_DUE_DATE|SEASON_YEAR|SEASON_NAME|ISS_PROGRAM_TYPE_CD','1|RQ30006|20150429|2014|PIPE - JAN|HCM');END;
            var SeasonYear = String.Empty;
            var SeasonName = String.Empty;
            if (!string.IsNullOrEmpty(req.Season))
            {
                var arr = req.Season.Split('^');
                if (arr.Length > 1)
                {
                    SeasonYear = arr[0];
                    SeasonName = arr[1];
                }
            }

            var queryBuilder = new StringBuilder();
            queryBuilder.Append(" BEGIN OPRSQL.ISS_REQSN_MAINT.UPDATE_REQSN(29,'REQSN_VERSION|REQSN_ID|UPDATE_DATE|UPDATE_USER_ID|PRODUCTION_STATUS|REQSN_STATUS_CD|CONTACT_PLANNER_CD|SRC_CONTACT_CD|REQSN_APPROVER_CD|DEMAND_LOC|CURR_DUE_DATE|SEASON_YEAR|SEASON_NAME|ISS_PROGRAM_TYPE_CD|DETAIL_TRKG_IND|APPRV_SUBMIT_DATE|APPRV_RESPONSE_DATE|RLSE_TO_SRC_DATE|EXTR_BY_SRC_DATE|REJ_BY_SRC_DATE|TRANSP_MODE_CD|OVER_PCT|UNDER_PCT|CORP_BUSINESS_UNIT|VENDOR_ID|VENDOR_SUFFIX|LW_COMPANY|LW_VENDOR_NO|LW_VENDOR_LOC','" + req.RequisitionVersion + "|" + Val(req.RequisitionId) + "|" +
                DateTime.Now.ToString(LOVConstants.DateFormatOracle) + "|" + Val(req.UpdatedBy) + "|" + Val(req.ProdStatus) +
                "|" + Val(req.ReqStatus) + "|"
                + Val(req.PlanningContact) + "|" + Val(req.SourcingContact) + "|" + Val(req.RequisitionApprover) + "|" + Val(req.DcLoc) + "|" +
                ((req.CurrentDueDate.HasValue) ? req.CurrentDueDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty)
                + "|" + Val(SeasonYear) + "|" + Val(SeasonName) + "|" + Val(req.ProType) + "|" + ((req.ReqDetailTracking) ? LOVConstants.Yes : LOVConstants.No) + "||||||" + Val(req.Mode) + "|" + ((req.OverPercentage.HasValue) ? (req.OverPercentage.Value / 100.0M) : 0) + "|" + ((req.UnderPercentage.HasValue) ? (req.UnderPercentage.Value / 100.0M) : 0) + "|" + Val(req.BusinessUnit) + "|"
                    + (req.VendorId) + "|" + (req.VendorSuffix) + "|" + (req.LwCompany) + "|" + (req.VendorNo) + "|" + Val(req.LwVendorLoc) + "');END; ");

            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            var result = (String)ExecuteScalar(queryBuilder.ToString());

            return (result == null || result == "Y") ? true : false;
        }




        /// <summary>
        /// Update existing order
        /// </summary>
        /// <param name="req"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        protected bool UpdateOrderQty(Requisition req, RequisitionDetail item)
        {

            // BEGIN OPRSQL.ISS_PROD_ORDER_MAINT.UPDATE_ORDER(9,'SUPER_ORDER|ORDER_VERSION|ORDER_ID|DEMAND_TYPE|ORIG_DUE_DATE|PLANNER_CD|TOTAL_CURR_ORDER_QTY|PLAN_DATE|MAKE_OR_BUY_CD','645746578|1|RQ30536|DD|20150717|KH|1260|20150605|B');END;



            var queryBuilder = new StringBuilder();
            queryBuilder.Append(" Begin OPRSQL.ISS_PROD_ORDER_MAINT.UPDATE_ORDER(9,'SUPER_ORDER|ORDER_VERSION|ORDER_ID|DEMAND_TYPE|ORIG_DUE_DATE|TOTAL_CURR_ORDER_QTY|PLAN_DATE|MAKE_OR_BUY_CD','" + item.SuperOrder
                + "|" + req.OrderVersion + "|" + Val(req.RequisitionId) + "|" + Val(req.DemandType) + "|" +
                ((req.OriginalDueDate.HasValue) ? req.OriginalDueDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty) + "|" +
                  (item.Qty) + "|" +
                ((item.PlanDate.HasValue) ? item.PlanDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty) +
              "|" + Val(item.MakeOrBuyCode)
                + "');END; ");


            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            var result = (String)ExecuteScalar(queryBuilder.ToString());

            return (result == null || result == "Y") ? true : false;
        }

        /// <summary>
        /// Update existing order
        /// </summary>
        /// <param name="req"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        protected bool UpdateOrder(Requisition req, RequisitionDetail item)
        {

            //OPRSQL.ISS_PROD_ORDER_MAINT.UPDATE_ORDER(19,'SUPER_ORDER|ORDER_VERSION|ORDER_ID|PRODUCTION_STATUS|DEMAND_TYPE|ORIG_DUE_DATE|CORP_BUSINESS_UNIT|TOTAL_CURR_ORDER_QTY|CURR_DUE_DATE|REQSN_VERSION|REQSN_ID|PLAN_DATE|SCHED_SHIP_DATE|LW_COMPANY|LW_VENDOR_NO|LW_VENDOR_LOC|VENDOR_ID|VENDOR_SUFFIX|MAKE_OR_BUY_CD','H002500939|1|RQ29918|L|DD|20150513|BRA|1044|20150513|1|RQ29918|20150213|20150401|3850|62831|ER02|10815|101|B');

            //BEGIN OPRSQL.ISS_PROD_ORDER_MAINT.UPDATE_ORDER(10,'SUPER_ORDER|ORDER_VERSION|ORDER_ID|DEMAND_TYPE|ORIG_DUE_DATE|PLANNER_CD|TOTAL_CURR_ORDER_QTY|CURR_DUE_DATE|SCHED_SHIP_DATE|MAKE_OR_BUY_CD','645441773|1|RQ30006|DD|20150610|KH|4272|20150610|20150429|B');END;


            //var queryBuilder = new StringBuilder();
            //queryBuilder.Append(" Begin OPRSQL.ISS_PROD_ORDER_MAINT.UPDATE_ORDER(24,'SUPER_ORDER|ORDER_VERSION|ORDER_ID|DEMAND_TYPE|ORIG_DUE_DATE|CORP_BUSINESS_UNIT|TOTAL_CURR_ORDER_QTY|CURR_DUE_DATE|REQSN_VERSION|REQSN_ID|PLAN_DATE|SCHED_SHIP_DATE|LW_COMPANY|LW_VENDOR_NO|LW_VENDOR_LOC|VENDOR_ID|VENDOR_SUFFIX|STYLE_CD|COLOR_CD|ATTRIBUTE_CD|SIZE_CD|MFG_REVISION_NO|MAKE_OR_BUY_CD|USER_ID','" + item.SuperOrder
            //    + "|" + req.OrderVersion + "|" + Val(req.RequisitionId) + "|" + Val(req.DemandType) + "|" +
            //    ((req.OriginalDueDate.HasValue) ? req.OriginalDueDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty) + "|" +
            //    Val(req.BusinessUnit) + "|" + (item.Qty) + "|" +
            //    ((item.CurrDueDate.HasValue) ? item.CurrDueDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty)
            //    + "|" + item.RequisitionVer + "|" + Val(req.RequisitionId) + "|"
            //    + ((item.PlanDate.HasValue) ? item.PlanDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty) + "|"
            //    + ((item.ScheduledShipDate.HasValue) ? item.ScheduledShipDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty)
            //    + "|" + ((req.LwCompany)) + "|" + (req.VendorNo) + "|" + Val(req.LwVendorLoc)
            //    + "|" + (req.VendorId) + "|"
            //    + (req.VendorSuffix) + 
            //    "|" + Val(item.Style) + "|" + Val(item.Color) + "|" + Val(item.Attribute) + "|" + Val(item.Size) +
            //    "|" + (item.Rev) + "|" + Val(item.MakeOrBuyCode) + "|" + Val(req.CreatedBy)
            //    + "');END; ");

            var queryBuilder = new StringBuilder();
            queryBuilder.Append(" Begin OPRSQL.ISS_PROD_ORDER_MAINT.UPDATE_ORDER(27,'SUPER_ORDER|ORDER_VERSION|ORDER_ID|DEMAND_TYPE|ORIG_DUE_DATE|CORP_BUSINESS_UNIT|TOTAL_CURR_ORDER_QTY|CURR_DUE_DATE|REQSN_VERSION|REQSN_ID|PLAN_DATE|SCHED_SHIP_DATE|LW_COMPANY|LW_VENDOR_NO|LW_VENDOR_LOC|VENDOR_ID|VENDOR_SUFFIX|STYLE_CD|COLOR_CD|ATTRIBUTE_CD|SIZE_CD|MFG_REVISION_NO|MFG_PATH_ID|MFG_PLANT|ROUTING_ID|MAKE_OR_BUY_CD|USER_ID','" + item.SuperOrder
                + "|" + req.OrderVersion + "|" + Val(req.RequisitionId) + "|" + Val(req.DemandType) + "|" +
                ((req.OriginalDueDate.HasValue) ? req.OriginalDueDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty) + "|" +
                Val(req.BusinessUnit) + "|" + (item.Qty) + "|" +
                ((item.CurrDueDate.HasValue) ? item.CurrDueDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty)
                + "|" + item.RequisitionVer + "|" + Val(req.RequisitionId) + "|"
                + ((item.PlanDate.HasValue) ? item.PlanDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty) + "|"
                + ((item.ScheduledShipDate.HasValue) ? item.ScheduledShipDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty)
                + "|" + ((req.LwCompany)) + "|" + (req.VendorNo) + "|" + Val(req.LwVendorLoc)
                + "|" + (req.VendorId) + "|"
                + (req.VendorSuffix) +
                "|" + Val(item.Style) + "|" + Val(item.Color) + "|" + Val(item.Attribute) + "|" + Val(item.Size) +
                "|" + (item.Rev) + "|" + Val(req.MFGPathId) + "|" + Val(req.MFGPathId) + "|" + Val(item.RoutinId) + "|" + Val(item.MakeOrBuyCode) + "|" + Val(req.CreatedBy)
                + "');END; ");

            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            var result = (String)ExecuteScalar(queryBuilder.ToString());

            return (result == null || result == "Y") ? true : false;
        }

        public void FillAdditionalDetailsForEdit(Requisition req, RequisitionDetail item)
        {
            var sku = new SKU(item);
            var list = FillOrderAdditinalDetails(sku, req.DcLoc);
            if (list != null && list.Count > 0)
            {
                var obj = list[0];                
                item.RoutinId = obj.RoutinId;
            }
        }
        public void FillAdditionalDetails(Requisition req, RequisitionDetail item)
        {
            var sku = new SKU(item);
            //var list = FillOrderAdditinalDetails(sku, req.DcLoc);
            var list = FillOrderAdditinalDetails(sku, req.MFGPathId);
            if (list != null && list.Count > 0)
            {
                var obj = list[0];
                item.MfgPathId = req.MFGPathId;
                item.MfgLoc = obj.MfgLoc;
                item.RoutinId = obj.RoutinId;
                item.BillOfMATL = obj.BillOfMATL;
                item.ScrapFactor = obj.ScrapFactor;
                item.Planner = obj.Planner;
                item.MakeOrBuy = obj.MakeOrBuy;
                //item.Uom = obj.Uom;
                item.PlanningLeadTime = obj.PlanningLeadTime;
                //item.Rev = obj.Rev;
                item.CapacityAlloc = obj.CapacityAlloc;
                item.ProdFamilyCd = obj.ProdFamilyCd;
                item.MatlCd = obj.MatlCd;
                item.AsrmtCd = obj.AsrmtCd;
            }
            list = FillOrderAdditinalFabricDetails(sku, req.DcLoc);
            if (list != null && list.Count > 0)
            {
                var obj = list[0];
                item.FabricGroup = obj.FabricGroup;
                item.ResourceId = obj.ResourceId;
            }

            list = GetSOStyleDetail(item.Style);
            if (list != null && list.Count > 0)
            {
                var obj = list[0];
                item.PackCD = obj.PackCD;
                item.PackageQty = obj.PackageQty;
                item.MatlCd = obj.MatlCd;
                item.AsrmtCd = obj.AsrmtCd;
                item.BusinessUnit = obj.BusinessUnit;  // validate with BU
                item.ProdFamilyCd = obj.ProdFamilyCd;
                // item.Uom = obj.Uom;
                item.Description = obj.Description;
                item.PipeLineCategoryCD = obj.PipeLineCategoryCD;
                item.GarmentStyle = obj.GarmentStyle;

            }



            item.DemandType = LOVConstants.DemandType;
            item.DemandDriver = LOVConstants.DemandDriver;

            item.ExpeditePriority = LOVConstants.ExpeditePriority; //
            item.CombinedInd = item.CombinedFabInd = LOVConstants.CombinedFabInd; //
            item.Priority = LOVConstants.Priority;
            item.PipeLineCategoryCD = LOVConstants.PipeLineCategoryCD;
            item.DescreteInd = LOVConstants.Yes;

            item.ResourceId = "N/A";
            item.Usage = 0.0m;
            item.StdUsage = 0.0m;
            item.StdLoss = 0.0m;
            item.WasteFactor = 0.0m;
            item.CylinderSize = 0.0m;
            item.FinishedWidth = 0.0m;
            item.ConditionedWidth = 0.0m;
            item.ScrapFactor = 0;
            item.PullFromStockInd = LOVConstants.No;
            var Plan = GetPlanningleadTime(sku, req.DcLoc, req.MFGPathId);
            if (Plan.Count > 0)
            {
                item.PlannedLeadTime = Plan[0].PlannedLeadTime;
                item.TransportationTime = Plan[0].TransportationTime;
            }
        }

        /// <summary>DEMAND_QTY
        /// Add manually added order. Not the retrieved order
        /// </summary>
        /// <param name="req"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool InsertOrderManual(Requisition req, RequisitionDetail item)
        {


            calculatePlannedAndScheduledDates(req, item);
            AddOrderTableManual(req, item);
            AddInsertOrderManual(req, item);
            return true;
        }



        public bool AddOrderTableManual(Requisition req, RequisitionDetail item)
        {
            var OrderActivity = getNewActivityId();

            // item.SuperOrder = OrderLabel;
            //BEGIN OPRSQL.TM_ISS_PROD_ORDER_DETAIL.TABLE_INSERT(63,'ORDER_LABEL|ORDER_VERSION|SUPER_ORDER|PARENT_ORDER      |STYLE_CD|COLOR_CD|ATTRIBUTE_CD|SIZE_CD|DEMAND_LOC     |PROD_FAMILY_CD|FABRIC_GROUP|MAKE_OR_BUY_CD|MFG_PLANT|PIPELINE_CATEGORY_CD    |DEMAND_QTY|CURR_DUE_DATE|CURR_ORDER_QTY|TOTAL_CURR_ORDER_QTY|MATL_TYPE_CD|ROUTING_ID|BILL_OF_MTRLS_ID|MFG_PATH_ID|BOM_SPEC_ID|MFG_STYLE_CD|MFG_COLOR_CD|MFG_ATTRIBUTE_CD|MFG_SIZE_CD|MASTER_ORDER_NO|CUTTING_ALT|USAGE|STD_USAGE|STD_LOSS|WASTE_FACTOR|FABRIC_TYPE|DYE_CD|DYE_SHADE_CD|MACHINE_TYPE_CD|CUT_METHOD|PULL_FROM_STOCK_IND|CYLINDER_SIZE|FINISHED_WIDTH|CONDITIONED_WIDTH|SPREAD_COMP_CD|SPREAD_TYPE_CD     |VENDOR_NO|ALLOCATION_IND|OFFLOAD_IND|SCRAP_FACTOR|PACK_CD|CATEGORY_CD|UNIT_OF_MEASURE|RESOURCE_ID     |ACTIVITY_ID|PLANNING_LEADTIME|COMBINE_IND|PLAN_DATE|MFG_REVISION_NO|     MACHINE_CUT|SCHED_SHIP_DATE|ASRMT_CD|CAPACITY_ALLOC|EXPEDITE_PRIORITY|COMBINE_FAB_IND','645441775|1|645441775||09453|150|------|NF|KM|----||B|5Z|SEW|120|20150612|120|120|00|SOURCED          5Z||KM||||||||0|0|0|0||||||N|0|0|0||||||0|||DZ|N/A|745063883|182|Y|20150424|0|0|20150430||BRA_R09453|50|Y');END;
            
            //Changes for Asia Recovery
            var query = new StringBuilder();
            query.Append("select routing_id \"RoutingId\" from mfg_path where style_cd= '" + Val(item.Style) + "' and color_cd= '" + Val(item.Color) + "' and attribute_cd=  '" + Val(item.Attribute) + "'   and size_cd= '" + Val(item.Size) + "' and mfg_path_id=  '" + Val(item.MfgPathId) + "'");
            IDataReader reader = ExecuteReader(query.ToString());
            var routingId = (new DbHelper()).ReadData<WorkOrderDetail>(reader);
            if (routingId != null && routingId.Count > 0)
            {
                item.RoutinId = routingId[0].RoutingId;
            }

            item.CuttingAlt =  !String.IsNullOrEmpty(item.CuttingAlt) ? item.CuttingAlt.ToUpper() : item.CuttingAlt;

            var queryBuilder = new StringBuilder();
            queryBuilder.Append(" Begin OPRSQL.TM_ISS_PROD_ORDER_DETAIL.TABLE_INSERT(64,'ORDER_LABEL|ORDER_VERSION|SUPER_ORDER|PARENT_ORDER|STYLE_CD|COLOR_CD|ATTRIBUTE_CD|SIZE_CD|DEMAND_LOC|PROD_FAMILY_CD|FABRIC_GROUP|MAKE_OR_BUY_CD|MFG_PLANT|PIPELINE_CATEGORY_CD|DEMAND_QTY|CURR_DUE_DATE|CURR_ORDER_QTY|TOTAL_CURR_ORDER_QTY|MATL_TYPE_CD|ROUTING_ID|BILL_OF_MTRLS_ID|MFG_PATH_ID|BOM_SPEC_ID|MFG_STYLE_CD|MFG_COLOR_CD|MFG_ATTRIBUTE_CD|MFG_SIZE_CD|MASTER_ORDER_NO|CUTTING_ALT|USAGE|STD_USAGE|STD_LOSS|WASTE_FACTOR|FABRIC_TYPE|DYE_CD|DYE_SHADE_CD|MACHINE_TYPE_CD|CUT_METHOD|PULL_FROM_STOCK_IND|CYLINDER_SIZE|FINISHED_WIDTH|CONDITIONED_WIDTH|SPREAD_COMP_CD|SPREAD_TYPE_CD|VENDOR_NO|ALLOCATION_IND|OFFLOAD_IND|SCRAP_FACTOR|PACK_CD|CATEGORY_CD|UNIT_OF_MEASURE|RESOURCE_ID|ACTIVITY_ID|PLANNING_LEADTIME|COMBINE_IND|PLAN_DATE|MFG_REVISION_NO|MACHINE_CUT|SCHED_SHIP_DATE|ASRMT_CD|CAPACITY_ALLOC|EXPEDITE_PRIORITY|COMBINE_FAB_IND|USER_ID','"
                + item.OrderLabel + "|" + req.OrderVersion + "|" + item.SuperOrder + "|" + item.ParentOrder +

                //STYLE_CD|COLOR_CD|ATTRIBUTE_CD|SIZE_CD|DEMAND_LOC
                "|" + Val(item.Style) + "|" + Val(item.Color) + "|" + Val(item.Attribute) + "|" + Val(item.Size) + "|" + Val(req.DcLoc) +

                //|PROD_FAMILY_CD|FABRIC_GROUP|MAKE_OR_BUY_CD|MFG_PLANT|PIPELINE_CATEGORY_CD
                "|" + Val(item.ProdFamilyCd) + "|" + Val(item.FabricGroup) + "|" + Val(item.MakeOrBuyCode) + "|" + Val(req.MFGPathId) + "|" + Val(item.PipeLineCategoryCD) +

                //|DEMAND_QTY|CURR_DUE_DATE|CURR_ORDER_QTY|TOTAL_CURR_ORDER_QTY|MATL_TYPE_CD|ROUTING_ID|BILL_OF_MTRLS_ID|
                "|" + (item.DemandQty) + "|" + ((item.CurrDueDate.HasValue) ? item.CurrDueDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty) + "|" + (item.Qty) + "|" + (item.TotatalCurrOrderQty) + "|" + Val(item.MatlCd) + "|" + Val(item.RoutinId) + "|" + Val(item.BillOfMATL) +

                //MFG_PATH_ID|BOM_SPEC_ID|MFG_STYLE_CD|MFG_COLOR_CD|MFG_ATTRIBUTE_CD|MFG_SIZE_CD|MASTER_ORDER_NO|CUTTING_ALT|
                "|" + Val(item.MfgPathId) + "|" + Val(item.BomSpecId) + "|" + Val(item.TBD) + "|" + Val(item.TBD) + "|" + Val(item.TBD) + "|" + Val(item.TBD) + "|" + Val(item.TBD) + "|" + Val(item.CuttingAlt) +


                //USAGE|STD_USAGE|STD_LOSS|WASTE_FACTOR|FABRIC_TYPE|DYE_CD|DYE_SHADE_CD|MACHINE_TYPE_CD|CUT_METHOD|
                "|" + (item.Usage) + "|" + (item.StdUsage) + "|" + (item.StdLoss) + "|" + (item.WasteFactor) + "|" + Val(item.TBD) + "|" + Val(item.DyeCD) + "|" + Val(item.DyeShadeCode) + "|" + Val(item.MachineTypeCode) + "|" + Val(item.CutMethod) +


                //PULL_FROM_STOCK_IND|CYLINDER_SIZE|FINISHED_WIDTH|CONDITIONED_WIDTH|SPREAD_COMP_CD|SPREAD_TYPE_CD
                "|" + item.PullFromStockInd + "|" + (item.CylinderSize) + "|" + (item.FinishedWidth) + "|" + (item.ConditionedWidth) + "|" + Val(item.SpreadCompCode) + "|" + Val(item.SpreadTypeCode) +

                //|VENDOR_NO|ALLOCATION_IND|OFFLOAD_IND|SCRAP_FACTOR|PACK_CD|CATEGORY_CD|UNIT_OF_MEASURE|RESOURCE_ID
                "|" + ((req.VendorNo == 0) ? "" : req.VendorNo.ToString()) + "|" + Val(item.TBD) + "|" + Val(item.TBD) + "|" + (item.ScrapFactor) + "|" + Val(item.PackCD) + "|" + Val(item.CategoryCD) + "|" + Val(item.Uom) + "|" + Val(item.ResourceId) +


                //|ACTIVITY_ID|PLANNING_LEADTIME|COMBINE_IND|PLAN_DATE|MFG_REVISION_NO|
                "|" + OrderActivity + "|" + (item.PlanningLeadTime) + "|" + Val(item.CombinedInd) + "|" + ((item.PlanDate.HasValue) ? item.PlanDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty) + "|" + (item.Rev) +


                //MACHINE_CUT|SCHED_SHIP_DATE|ASRMT_CD|CAPACITY_ALLOC|EXPEDITE_PRIORITY|COMBINE_FAB_IND
                "|" + (item.MachineCut) + "|" + ((item.ScheduledShipDate.HasValue) ? item.ScheduledShipDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty) + "|" + Val(item.AsrmtCd) + "|" + Val(item.CapacityAlloc) + "|" + Val(item.ExpeditePriority) + "|" + Val(item.CombinedFabInd) + "|" + Val(req.CreatedBy)
                + "');END; ");

            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            var result = (String)ExecuteScalar(queryBuilder.ToString());

            return (result == null || result == "Y") ? true : false;
        }

        /// <summary>
        /// For Update requisition only
        /// </summary>
        /// <param name="req"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool AddInsertOrderManual(Requisition req, RequisitionDetail item)
        {

            //BEGIN OPRSQL.ISS_PROD_ORDER_MAINT.INSERT_ORDER(32,'SUPER_ORDER|ORDER_VERSION|ORDER_ID|PRODUCTION_STATUS|DEMAND_TYPE|DEMAND_QTY|ORIG_ORDER_QTY|              ORIG_DUE_DATE|REMOTE_UPDATE_CD|REMOTE_STATUS_CD|REMOTE_RELEASE_DATE|REMOTE_XCPN_REASON|DOZENS_ONLY_IND|CREATE_BD_IND|PRIORITY|SPREAD_TYPE_CD|            CORP_BUSINESS_UNIT|  PLANNER_CD|DEMAND_SOURCE|DEMAND_DRIVER|RULE_NUMBER|REQSN_VERSION|REQSN_ID|ISS_ORDER_TYPE_CD   |LW_COMPANY|LW_VENDOR_NO|LW_VENDOR_LOC|VENDOR_ID|VENDOR_SUFFIX    |DEMAND_DATE|DISCRETE_IND|EXPEDITE_PRIORITY','645441775|1|RQ30006|L|DD|120|0|20150612|||||N|Y|999999999||SHR|PLANN||ISS|9999|1|RQ30006|SR|3850|62831|ER02|0|0|20150611|Y|50');END;
//REMOTE_UPDATE_CD = ‘A’
                //REMOTE_STATUS_CD = ‘E’
                //REMOTE_RELEASE_DATE = system date/time


            var queryBuilder = new StringBuilder();
            queryBuilder.Append(" Begin OPRSQL.ISS_PROD_ORDER_MAINT.INSERT_ORDER(35,'SUPER_ORDER|ORDER_VERSION|ORDER_ID|PRODUCTION_STATUS|DEMAND_TYPE|DEMAND_QTY|ORIG_ORDER_QTY|ORIG_DUE_DATE|REMOTE_UPDATE_CD|REMOTE_STATUS_CD|REMOTE_RELEASE_DATE|REMOTE_XCPN_REASON|DOZENS_ONLY_IND|CREATE_BD_IND|PRIORITY|SPREAD_TYPE_CD|CORP_BUSINESS_UNIT|PLANNER_CD|DEMAND_SOURCE|DEMAND_DRIVER|RULE_NUMBER|REQSN_VERSION|REQSN_ID|ISS_ORDER_TYPE_CD|LW_COMPANY|LW_VENDOR_NO|LW_VENDOR_LOC|VENDOR_ID|VENDOR_SUFFIX|DEMAND_DATE|DISCRETE_IND|EXPEDITE_PRIORITY|USER_ID|ATTRIBUTION_IND|ORDER_REFERENCE','" + item.SuperOrder
                + "|" + req.OrderVersion + "|" + Val(req.RequisitionId) + "|" + Val(req.ProdStatus) + "|" + Val(req.DemandType)

                //DEMAND_QTY|ORIG_ORDER_QTY
                + "|" + (item.DemandQty) + "|" + (item.OriginalOrderQty)

                //ORIG_DUE_DATE
                + "|" + ((req.OriginalDueDate.HasValue) ? req.OriginalDueDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty)

                //REMOTE_UPDATE_CD|REMOTE_STATUS_CD|REMOTE_RELEASE_DATE|REMOTE_XCPN_REASON|DOZENS_ONLY_IND|CREATE_BD_IND|PRIORITY|SPREAD_TYPE_CD|
                + "|" + 'A' + "|" + 'E' + "|" + DateTime.Now.ToString("MM/dd/yyyy h:mm") + "|" + Val(item.TBD) + "|" + Val(item.DozensOnlyInd) + "|" + Val(item.CreateBDInd) + "|" + (item.Priority) + "|" + Val(item.TBD)

                //CORP_BUSINESS_UNIT
                  + "|" + Val(req.BusinessUnit)


                //PLANNER_CD | DEMAND_SOURCE | DEMAND_DRIVER | RULE_NUMBER | REQSN_VERSION | REQSN_ID | ISS_ORDER_TYPE_CD
                + "|" + Val(item.Planner) + "|" + Val(item.DemandSource) + "|" + Val(item.DemandDriver) + "|" + (item.Dpr) + "|" + req.RequisitionVersion + "|" + Val(req.RequisitionId) + "|" + Val(item.OrderType)


                //|LW_COMPANY|LW_VENDOR_NO|LW_VENDOR_LOC|VENDOR_ID|VENDOR_SUFFIX
                + "|" + ((req.LwCompany)) + "|" + (req.VendorNo) + "|" + Val(req.LwVendorLoc) + "|" + (req.VendorId) + "|" + (req.VendorSuffix)

                //|DEMAND_DATE|DISCRETE_IND|EXPEDITE_PRIORITY|USER_ID|ORDER_REFERENCE
                 + "|" + ((req.DemandDate.HasValue) ? req.DemandDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty) + "|" + Val(item.DiscreteInd) + "|" + Val(item.ExpeditePriority) + "|" + Val(req.CreatedBy)

                 + "|" + (!String.IsNullOrEmpty(item.AttributionInd) ? item.AttributionInd : LOVConstants.No) + "|" + Val(item.OrderReference) + "');END; ");

            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            var result = (String)ExecuteScalar(queryBuilder.ToString());

            return (result == null || result == "Y") ? true : false;

        }


        public Decimal getNewOrderLabel()
        {

            return (Decimal)ExecuteScalar("select ORDER_LABEL_SEQUENCE.NEXTVAL FROM DUAL ");
        }

        public Decimal getNewActivityId()
        {

            return (Decimal)ExecuteScalar("select ACTIVITY_ID_SEQUENCE.NEXTVAL FROM DUAL ");
        }



        public bool DeleteRequisition(Requisition req)
        {

            // BEGIN OPRSQL.ISS_REQSN_MAINT.DELETE_REQSN(2,'REQSN_VERSION|REQSN_ID','1|RQ33562');END;

            var queryBuilder = new StringBuilder();
            queryBuilder.Append(" BEGIN OPRSQL.ISS_REQSN_MAINT.DELETE_REQSN(2,'REQSN_VERSION|REQSN_ID','" + req.RequisitionVersion + "|"
                + req.RequisitionId + "');END; ");

            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            var result = (String)ExecuteScalar(queryBuilder.ToString());

            return (result == null || result == "Y") ? true : false;
        }

        public bool DeleteOrder(RequisitionDetail item)
        {

            // BEGIN OPRSQL.ISS_PROD_ORDER_MAINT.DELETE_ORDER(2,'SUPER_ORDER|ORDER_VERSION','645441776|1');END; 

            var queryBuilder = new StringBuilder();
            queryBuilder.Append(" BEGIN OPRSQL.ISS_PROD_ORDER_MAINT.DELETE_ORDER(2,'SUPER_ORDER|ORDER_VERSION','" + item.SuperOrder + "|"
                + item.OrderVersion + "');END; ");

            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            var result = (String)ExecuteScalar(queryBuilder.ToString());

            return (result == null || result == "Y") ? true : false;
        }
        //Delete WO
        public bool DeleteOrder(WorkOrderHeader woHeader, string SuperOrder)
        {
            // BEGIN OPRSQL.ISS_PROD_ORDER_MAINT.DELETE_ORDER_DETAIL(2,'SUPER_ORDER|ORDER_VERSION','E000501573|1');END;
            var queryBuilder = new StringBuilder();
            queryBuilder.Append("BEGIN OPRSQL.ISS_PROD_ORDER_MAINT.DELETE_ORDER_DETAIL(25,'SUPER_ORDER|ORDER_VERSION','" +
                //SUPER_ORDER|ORDER_VERSION
                Val(SuperOrder) + "|" + "1" + "');end;");
            var result = (String)ExecuteScalar(queryBuilder.ToString());
            return (result == null || result == "Y") ? true : false;
        }
        #endregion

        public void calculatePlannedAndScheduledDates(Requisition req, RequisitionDetail item)
        {
            req.DCDueDate = req.PlannedDcDate;
            item.PlanDate = req.PlannedDcDate;
            item.ScheduledShipDate = req.PlannedDcDate;
            var dcDate = req.PlannedDcDate;

            var trans = item.TransportationTime;
            var leadTime = item.PlannedLeadTime;

            while (trans > 0)
            {
                if (dcDate.DayOfWeek == DayOfWeek.Saturday)
                {
                    dcDate = dcDate.AddDays(2);
                }
                else if (dcDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    dcDate = dcDate.AddDays(1);
                }
                else
                {
                    trans--;
                    dcDate = dcDate.AddDays(1);

                }
            }// end while trans>0

            if (dcDate.DayOfWeek == DayOfWeek.Saturday)
            {
                dcDate = dcDate.AddDays(2);
            }
            else if (dcDate.DayOfWeek == DayOfWeek.Sunday)
            {
                dcDate = dcDate.AddDays(1);
            }

            while (leadTime > 0)
            {
                if (item.PlanDate.Value.DayOfWeek == DayOfWeek.Saturday)
                {
                    item.PlanDate = item.PlanDate.Value.AddDays(-1);
                }
                else if (item.PlanDate.Value.DayOfWeek == DayOfWeek.Sunday)
                {
                    item.PlanDate = item.PlanDate.Value.AddDays(-2);
                }
                else
                {
                    leadTime--;
                    item.PlanDate = item.PlanDate.Value.AddDays(-1);
                }
            }// end while LeadTime>0
            if (item.PlanDate.Value.Date < DateTime.Now.Date)
            {
                item.PlanDate = getNextFriday(DateTime.Now.Date);
            }

            item.CurrDueDate = req.OriginalDueDate = item.OriginalDueDate = getNextFriday(dcDate); // req.PlannedDcDate;
            req.DemandDate = dcDate;
        }



        public DateTime getNextFriday(DateTime PlanDate)
        {

            //next friday

            if (PlanDate.DayOfWeek == DayOfWeek.Friday)
            {

            }
            else if (PlanDate.DayOfWeek == DayOfWeek.Sunday)
            {
                PlanDate = PlanDate.AddDays(5);
            }
            else if (PlanDate.DayOfWeek == DayOfWeek.Monday)
            {
                PlanDate = PlanDate.AddDays(4);
            }
            else if (PlanDate.DayOfWeek == DayOfWeek.Tuesday)
            {
                PlanDate = PlanDate.AddDays(3);
            }
            else if (PlanDate.DayOfWeek == DayOfWeek.Wednesday)
            {
                PlanDate = PlanDate.AddDays(2);
            }
            else if (PlanDate.DayOfWeek == DayOfWeek.Thursday)
            {
                PlanDate = PlanDate.AddDays(1);
            }
            else if (PlanDate.DayOfWeek == DayOfWeek.Saturday)
            {
                PlanDate = PlanDate.AddDays(6);
            }
            return PlanDate;
        }


        /// <summary>
        /// on Style change
        /// </summary>
        /// <param name="Style"></param>
        /// <returns></returns>
        public List<RequisitionDetail> GetSOStyleDetail(String Style)
        {

            //	Style / Color / Attribute / Size Code / Revision / Lawson Company / Vendor Number / Vendor Location
            //select oprsql.iss_reqsn_validate.verifyVendor('09453','B5V','------','NF',0,3850,62831,'ER02') from dual

            var queryBuilder = new StringBuilder();
            queryBuilder.Append("SELECT A.CORP_DIVISION_CD \"CorpDivision\",A.MATL_TYPE_CD \"MatlCd\", B.CORP_BUSINESS_UNIT \"BusinessUnit\",A.SIZE_LINE \"SizeLine\",A.MFG_CORP_DIVISION_CD \"MFGCorpDivision\", A.QUALITY_CD \"QualityCD\", B.COLOR_DIVISION_CD \"ColorDivisionCD\", A.ORIGIN_TYPE_CD \"OriginTypeCD\", A.GTIN_LABEL_CD \"GTINLabelCD\", A.INVENTORY_CHECK_IND \"InvCheckInd\", A.PACK_CD \"PackCD\",A.UNIT_OF_MEASURE \"Uom\" , nvl(s.prod_family_cd,'----') \"ProdFamilyCd\", nvl(s.product_class_cd,'----') \"ProdClassCD\",A.PRIMARY_DC \"PrimaryDC\", A.STYLE_DESC \"Description\" ,P.PACKAGE_QTY \"PackageQty\",A.ASRMT_CD \"AsrmtCd\" FROM STYLE A, STYLE_CHP S, CORP_DIVISION B, PACK P  	 where A.style_cd = '" + Val(Style) + "' AND A.CORP_DIVISION_CD  = B.CORP_DIVISION_CD			and a.pack_cd = p.pack_cd 			and a.style_cd = s.style_cd (+)			");

            IDataReader reader = ExecuteReader(queryBuilder.ToString());

            var result = (new DbHelper()).ReadData<RequisitionDetail>(reader);
            return result;
        }

        public List<RequisitionDetail> GetDCValidate(String DcLoc)
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append("SELECT Plant_CD \"DcLoc\" FROM plant WHERE Plant_CD = '" + Val(DcLoc) + "'  ");
            IDataReader reader = ExecuteReader(queryBuilder.ToString());
            var result = (new DbHelper()).ReadData<RequisitionDetail>(reader);
            return result;

            //var PrjCnt = ExecuteScalar(query);
            //if (Convert.ToInt32(PrjCnt) > 0)
            //return true;
            //else
            //return false;
        }

        public List<RequisitionDetail> GetMFGValidate(String MFGPathId)
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append(" SELECT --+ INDEX(x, IMFG_PATH1)   " + "\n");
            queryBuilder.Append(" mfg_path_id \"MFGPathId\" from mfg_path where mfg_path_id = '" + Val(MFGPathId) + "' AND ROWNUM = 1 ");
            IDataReader reader = ExecuteReader(queryBuilder.ToString());
            var result = (new DbHelper()).ReadData<RequisitionDetail>(reader);
            return result;

        }
        public bool GetDCValidation(String DcLoc)
        {
            string query = "SELECT count(*) FROM plant WHERE Plant_CD = '" + Val(DcLoc) + "'  ";
            var DcCount = ExecuteScalar(query);
            if (Convert.ToInt32(DcCount) > 0)
                return true;
            else
                return false;

        }
        public List<RequisitionDetail> GetDCValidates(String DcLoc, String BusUnit)
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append("SELECT Plant_CD \"DcLoc\" FROM plant WHERE Plant_CD = '" + Val(DcLoc) + "' AND corp_business_unit = '" + Val(BusUnit) + "' ");
            IDataReader reader = ExecuteReader(queryBuilder.ToString());
            var result = (new DbHelper()).ReadData<RequisitionDetail>(reader);
            return result;
        }
        public string Get_Demand_Source(string PONumber, string Line_Number)
        {
            var queryBuilder = new StringBuilder();
            //queryBuilder.Append("select OPRSQL.external_data_pkg.concatnate_demand('PO'" + ",\'" + "HAA" + "\',\'" + PONumber + "\',\'" + Line_Number + "\') from dual ");
            queryBuilder.Append("SELECT SUBSTR ( oprsql.external_data_pkg.concatnate_demand ('PO'" + ",\'" + "HAA" + "\',\'" + PONumber + "\',\'" + Line_Number + "\') ,1, LENGTH ( oprsql.external_data_pkg.concatnate_demand ('PO'" + ",\'" + "HAA" + "\',\'" + PONumber + "\',\'" + Line_Number + "\') )-1) from dual ");

            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            var result = (string)ExecuteScalar(queryBuilder.ToString());
            return result;

        }
        public bool GetSOSKUBDDetail(SKU sku, String MFGPathId)
        {

            //	Style / Color / Attribute / Size Code / Revision / Lawson Company / Vendor Number / Vendor Location
            //select oprsql.iss_reqsn_validate.verifyVendor('09453','B5V','------','NF',0,3850,62831,'ER02') from dual

            var queryBuilder = new StringBuilder();
            queryBuilder.Append("select nvl(BD_DEFAULT,'Y') from MFG_PATH_CHP  where ");


            queryBuilder.Append("   style_cd = '" + Val(sku.Style) + "' ");

            if (!String.IsNullOrWhiteSpace(sku.Color))
            {
                queryBuilder.Append("and color_cd = '" + Val(sku.Color) + "' ");
            }

            if (!String.IsNullOrWhiteSpace(sku.Attribute))
            {
                queryBuilder.Append("and attribute_cd = '" + Val(sku.Attribute) + "' ");
            }

            if (!String.IsNullOrWhiteSpace(sku.Size))
            {
                queryBuilder.Append("and size_cd = '" + Val(sku.Size) + "' ");
            }
            if (!String.IsNullOrWhiteSpace(MFGPathId))
            {
                queryBuilder.Append("and mfg_path_id  = '" + Val(MFGPathId) + "' ");
            }

            var result = (String)ExecuteScalar(queryBuilder.ToString());

            return (result != null && result == "Y");
        }
    }

}
