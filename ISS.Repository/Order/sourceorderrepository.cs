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
using System.Data.Common;


namespace ISS.Repository.Order
{
    public partial  class SourceOrderRepository : RepositoryBase
    {
        public SourceOrderRepository( )
            : base()
        {

        }
        public SourceOrderRepository(DbTransaction trans):base(trans)
        {
            
        }
        public IList<Requisition> SaveRequisitionDetail(Requisition requisition)
        {
            return null;
        }
        public IList<RequisitionDetail> GetRequisitionDetail(String requisitionId)
        {
            string query = " SELECT a.reqsn_id \"RequisitionId\",a.ORDER_VERSION as \"OrderVersion\", a.SUPER_ORDER as \"SuperOrder\",  a.style_cd \"Style\", b.style_desc \"Description\", a.color_cd \"Color\", a.attribute_cd \"Attribute\",  a.size_cd \"Size\", size_short_desc \"SizeLit\", a.demand_loc  \"DCLoc\"" +
                " ,nvl(a.mfg_revision_no,0) \"Rev\","+               
                "trunc((s.std_case_qty)/12,0)+ mod((s.std_case_qty),12)/100 "
                +"\"StdCase\" "
                +", a.unit_of_measure \"Uom\",trunc( a.curr_order_qty/12,0)+ mod(a.curr_order_qty,12)/100  \"Qty\" " +
                " , nvl(a.plan_date,a.curr_due_date) \"PlanDate\", nvl(a.curr_due_date,'') \"CurrDueDate\", nvl(a.lw_vendor_no,0) lw_vendor_no, nvl(a.lw_vendor_loc,'') as lw_vendor_loc, nvl(a.lw_company,0)  lw_company, rule_number \"Dpr\"" +
                " , Nvl(demand_source,'Create Lot') \"DemandSource\",  iss_order_type_cd \"OrderType\", mfg_path_id \"SewPath\", mfg_path_id \"MfgPathId\" FROM ISS_PROD_ORDER_VIEW a, style b, item_size i, sku_revision s where a.style_cd = b.style_cd and a.size_cd = i.size_cd and a.reqsn_version = 1" +
                " and a.reqsn_id = '" + Val(requisitionId) + "' and a.style_cd = s.style_cd and a.color_cd = s.color_cd and a.attribute_cd = s.attribute_cd and a.size_cd = s.size_cd " +
                "and a.mfg_revision_no = s.revision_no order by 3, 5, 6, 7, 9 ";

            IDataReader reader = ExecuteReader(query);
            var result = (new DbHelper()).ReadData<RequisitionDetail>(reader);
            if(result!=null)result.ForEach(e=> e.IsInserted=true);
            return result;
        }

        public bool GetStyleValidation(string StyleCode, string BusinessCode)
        {
            string query = " select COUNT(*) from style s, corp_division c where style_cd = '" + Val(StyleCode) + "' and s.corp_division_cd = c.corp_division_cd and c.corp_business_unit = '" + Val(BusinessCode) + "' ";
            var PrjCnt = ExecuteScalar(query);
            if (Convert.ToInt32(PrjCnt) > 0)
                return true;
            else
                return false;
        }
        public IList<RequisitionDetail> GetRevisionAndMatlCd(string style, string color, string attribute, string size)
        {
            var query = new StringBuilder();
            query.Append("select s.mfg_revision_no \"Rev\", s.iss_ind \"IssInd\", decode(y.matl_type_cd,'00','G','01','G','02','G','N') \"MatlCd\" from sku s, style y  where s.style_cd = y.style_cd ");
            if (!string.IsNullOrWhiteSpace(style)) query.Append(" AND s.style_cd='" + Val(style) + "'");
            if (!string.IsNullOrWhiteSpace(color)) query.Append(" AND s.color_cd ='" + Val(color) + "'");
            if (!string.IsNullOrWhiteSpace(attribute)) query.Append(" AND s.attribute_cd ='" + Val(attribute) + "'");
            if (!string.IsNullOrWhiteSpace(size)) query.Append(" AND s.size_cd ='" + Val(size) + "'");

            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<RequisitionDetail>(reader);
            return result;
        }

        public IList<Requisition> GetRequisition(String RequisitionId)
        {
            string reqTable = "";

            if (IsReqActive(RequisitionId, 1))
                reqTable = "ISS_REQSN";
            else
                reqTable = "ISS_REQSN_ARCHIVE";

            string query = " SELECT a.reqsn_id \"RequisitionId\", a.reqsn_version \"RequisitionVersion\", a.production_status \"ProdStatus\", a.reqsn_status_cd \"ReqStatus\", b.reqsn_status_short_desc \"ReqStatusDesc\",  a.contact_planner_cd \"PlanningContact\", c.planner_name, a.src_contact_cd \"SourcingContact\", d.src_contact_name, e.reqsn_approver_cd \"RequisitionApprover\"" +
              " , e.reqsn_approver_name , a.corp_business_unit \"BusinessUnit\", a.demand_loc \"DcLoc\", a.curr_due_date \"PlannedDcDate\", season_year||'^'||a.season_name \"Season\" , a.iss_program_type_cd \"ProType\", f.iss_program_type_desc, a.detail_trkg_ind \"ReqDetailTrackingVal\",  a.apprv_submit_date \"ApprovrSubmitDate\"" +
              " , a.apprv_response_date \"ApprovrResponseDate\", a.rlse_to_src_date \"RLSEtoSrcDate\", a.extr_by_src_date \"EXTRbySrcDate\",  a.rej_by_src_date \"rejBySrcDate\", nvl(a.vendor_id,0)  \"VendorId\", nvl(a.vendor_suffix,0) \"VendorSuffix\",  a.transp_mode_cd \"Mode\", g.transp_mode_name , a.over_pct*100 \"OverPercentageD\", a.under_pct *100 \"UnderPercentageD\", nvl(a.lw_company,0) \"LwCompany\" , nvl(a.lw_vendor_no,0) \"VendorNo\", a.lw_vendor_loc \"LwVendorLoc\", nvl(l.lw_vendor_vname,'')   \"VendorName\",  nvl(lw_vendor_city_addr5,'') vendor_city, nvl(l.lw_vendor_country,'') vendor_country, a.create_date  \"CreatedOn\", a.create_user_id \"CreatedBy\", a.update_date  \"UpdatedOn\", a.update_user_id \"UpdatedBy\" " +
                " FROM " + reqTable + " a,  REQSN_STATUS b, PLANNER c, SRC_CONTACT d, REQSN_APPROVER e ,  ISS_PROGRAM_TYPE f, TRANSP_MODE g, lawson_vendor l where a.reqsn_status_cd = b.reqsn_status_cd(+)  and a.contact_planner_cd = c.planner_cd(+) and a.src_contact_cd = d.src_contact_cd(+)  and a.reqsn_approver_cd = e.reqsn_approver_cd(+) " +
                "and a.iss_program_type_cd = f.iss_program_type_cd(+)  and a.transp_mode_cd = g.transp_mode_cd(+)  and a.lw_vendor_no = l.lw_vendor_no(+) and a.lw_vendor_loc = l.lw_vendor_loc(+)  and a.reqsn_version = 1 and a.reqsn_id = '" + Val(RequisitionId) + "' ";


            IDataReader reader = ExecuteReader(query);

            var result = (new DbHelper()).ReadData<Requisition>(reader);
            if(result!=null & result.Count>0){
               var vendor= GetVendorDetails(result[0].VendorNo,result[0].LwVendorLoc,result[0].BusinessUnit);
               if (vendor != null && vendor.Count > 0)
                   result[0].MFGPathId = vendor[0].SrcPlant;
            }
            return result;
        }
 

        public IList<VendorSearch> GetVendor(VendorSearch vendorSearch)
        {
            string query = " select l.lw_vendor_vname  \"VendorName\",l.lw_vendor_city_addr5  \"VendorCity\",l.lw_vendor_country  \"LwVendorCountry\",bu.lw_company  \"LwCompany\",l.lw_vendor_no  \"LwVendorNo\",l.lw_vendor_loc  \"LwVendorLoc\",sv.vendor_id  \"VendorId\",sv.vendor_suffix  \"VendorSuffix\", pl.src_plant_cd  \"SrcPlant\"  FRom SVM_ISS_POREADY_FACILITIES sv, lawson_vendor l, corp_business_unit bu, Src_Vendor_Plant_Xref pl " +
                " where sv.lw_vendor_no = l.lw_vendor_no and sv.lw_vendor_loc = l.lw_vendor_loc and sv.src_division_cd = bu.corp_business_unit  and bu.corp_business_unit = '" + Val(vendorSearch.ByName) + "' and sv.vendor_id = pl.vendor_id and sv.vendor_suffix = pl.vendor_suffix " +
                " and pl.src_division_cd = bu.corp_business_unit and sv.vendor_id = pl.vendor_id and sv.vendor_suffix = pl.vendor_suffix ";
                //"and l.lw_vendor_no = '" + Val(vendorSearch.) + "' and l.lw_vendor_loc = '" + Val(requisition.RequisitionId) + "' " +


            IDataReader reader = ExecuteReader(query);

            var result = (new DbHelper()).ReadData<VendorSearch>(reader);
            return result;
        }

        public IList<VendorSearch> GetVendorSearch(VendorSearch vendorSearch)
        {
            var queryBuilder = new StringBuilder();

            if (!String.IsNullOrWhiteSpace(vendorSearch.ByName))
            {
                queryBuilder.Append("select distinct l.lw_vendor_vname \"VendorName\", sv.src_division_cd \"BusUnit\", l.lw_vendor_city_addr5 \"VendorCity\", " +
                    "  l.lw_vendor_country \"LwVendorCountry\",  bu.lw_company \"LwCompany\", l.lw_vendor_no \"LwVendorNo\", l.lw_vendor_loc \"LwVendorLoc\", " +
                    "sv.vendor_id \"VendorId\", sv.vendor_suffix \"VendorSuffix\" ,pl.src_plant_cd \"SrcPlant\" from SVM_ISS_POREADY_FACILITIES sv, lawson_vendor l, corp_business_unit bu ,corp_purchase_sku ps , Src_Vendor_Plant_Xref pl,  Style s, corp_division cd ");
                queryBuilder.Append("where sv.lw_vendor_no = l.lw_vendor_no and sv.lw_vendor_loc = l.lw_vendor_loc  and sv.src_division_cd = bu.corp_business_unit " +
                    "and LOWER(l.lw_vendor_vname) like \'" + (vendorSearch.ByName.ToLower()) + "%\' ");

                queryBuilder.Append("and ps.end_date_ind = 'N' and bu.lw_company = ps.lw_company  and l.lw_vendor_no = ps.lw_vendor_no  and l.lw_vendor_loc = ps.lw_vendor_loc and sv.lw_vendor_no = ps.lw_vendor_no  and sv.lw_vendor_loc = ps.lw_vendor_loc and pl.src_division_cd = bu.corp_business_unit and sv.vendor_id = pl.vendor_id    and sv.vendor_suffix = pl.vendor_suffix");
                queryBuilder.Append("and ps.style_cd = s.style_cd AND s.corp_division_cd = cd.corp_division_cd AND cd.corp_business_unit = sv.src_division_cd ");
                queryBuilder.Append(" order by sv.src_division_cd, l.lw_vendor_vname");

            }
            else
            {
                if (vendorSearch.ByColor == null)
                {
                    queryBuilder.Append("select distinct ps.style_cd \"ByStyle\", '' \"ByColor\", sv.src_division_cd \"BusUnit\", l.lw_vendor_vname \"VendorName\", " +
                        " l.lw_vendor_city_addr5  \"VendorCity\", l.lw_vendor_country \"LwVendorCountry\", ps.lw_company \"LwCompany\", ps.lw_vendor_no \"LwVendorNo\", " +
                        " ps.lw_vendor_loc \"LwVendorLoc\", sv.vendor_id \"VendorId\", sv.vendor_suffix \"VendorSuffix\", pl.src_plant_cd  \"SrcPlant\" " +
                        " from corp_purchase_sku ps, SVM_ISS_POREADY_FACILITIES sv, lawson_vendor l, corp_business_unit bu, Src_Vendor_Plant_Xref pl, Style s, corp_division cd ");
                    queryBuilder.Append(" where ps.end_date_ind = 'N'  and bu.lw_company = ps.lw_company  and l.lw_vendor_no = ps.lw_vendor_no  " +
                       " and l.lw_vendor_loc = ps.lw_vendor_loc  and sv.lw_vendor_no = ps.lw_vendor_no  and sv.lw_vendor_loc = ps.lw_vendor_loc  " +
                       " and pl.src_division_cd = bu.corp_business_unit  and sv.vendor_id = pl.vendor_id  and sv.vendor_suffix = pl.vendor_suffix  " +
                       " and ps.style_cd = s.style_cd AND s.corp_division_cd = cd.corp_division_cd AND cd.corp_business_unit = sv.src_division_cd");
                }
                else
                {
                    queryBuilder.Append("select distinct ps.style_cd \"ByStyle\", ps.color_cd \"ByColor\", sv.src_division_cd \"BusUnit\", l.lw_vendor_vname \"VendorName\", " +
                       " l.lw_vendor_city_addr5  \"VendorCity\", l.lw_vendor_country \"LwVendorCountry\", ps.lw_company \"LwCompany\", ps.lw_vendor_no \"LwVendorNo\", " +
                       " ps.lw_vendor_loc \"LwVendorLoc\", sv.vendor_id \"VendorId\", sv.vendor_suffix \"VendorSuffix\", pl.src_plant_cd  \"SrcPlant\" " +
                       " from corp_purchase_sku ps, SVM_ISS_POREADY_FACILITIES sv, lawson_vendor l, corp_business_unit bu, Src_Vendor_Plant_Xref pl , Style s, corp_division cd ");
                    queryBuilder.Append(" where ps.end_date_ind = 'N'  and bu.lw_company = ps.lw_company  and l.lw_vendor_no = ps.lw_vendor_no  " +
                       " and l.lw_vendor_loc = ps.lw_vendor_loc  and sv.lw_vendor_no = ps.lw_vendor_no  and sv.lw_vendor_loc = ps.lw_vendor_loc  " +
                       " and pl.src_division_cd = bu.corp_business_unit  and sv.vendor_id = pl.vendor_id  and sv.vendor_suffix = pl.vendor_suffix   " +
                       " and ps.style_cd = s.style_cd AND s.corp_division_cd = cd.corp_division_cd AND cd.corp_business_unit = sv.src_division_cd");
                }

                if (!String.IsNullOrWhiteSpace(vendorSearch.ByStyle))
                {
                    queryBuilder.Append(" and ps.style_cd like \'" + Val(vendorSearch.ByStyle) + "\' ");
                }
                if (!String.IsNullOrWhiteSpace(vendorSearch.ByColor))
                {
                    queryBuilder.Append(" and ps.color_cd like \'" + Val(vendorSearch.ByColor) + "\' ");
                }
            }

            IDataReader reader = ExecuteReader(queryBuilder.ToString());

            var result = (new DbHelper()).ReadData<VendorSearch>(reader);
    
            return result;
        }

        public IList<RequisitionSearch> GetRequisitionSearch(RequisitionSearch reqSearch)
        {
            var queryBuilder = new StringBuilder();

            queryBuilder.Append("select distinct reqsn_id \"RequisitionId\",i.production_status \"ProdStatus\",i.contact_planner_cd \"PlanningContact\",i.create_date \"CreateDate\" ,s.REQSN_STATUS_SHORT_DESC \"ReqStatus\" " +
                                    ",i.corp_business_unit \"BusUnit\", l.lw_vendor_vname \"VendorName\", l.lw_vendor_city_addr5 \"VenCity\",l.lw_vendor_country \"VenCountry\", " +
                                    "l.lw_vendor_no \"VendorNo\", l.lw_vendor_loc \"VendorLoc\",i.create_date \"Locked\"  " +
                                    "from iss_reqsn i, lawson_vendor l, reqsn_status s  ");
            queryBuilder.Append("where i.lw_vendor_no = l.lw_vendor_no(+)  and i.lw_vendor_loc = l.lw_vendor_loc(+)  and i.reqsn_status_cd = s.reqsn_status_cd  and i.production_status in ( 'R','L')  ");

            if (!String.IsNullOrWhiteSpace(reqSearch.RequisitionId))
            {
                queryBuilder.Append(" and i.reqsn_id = \'" + Val(reqSearch.RequisitionId.ToUpper()) + "\' ");
            }
            if ((reqSearch.FromDate.HasValue && reqSearch.FromDate != DateTime.MinValue) && (reqSearch.ToDate.HasValue && reqSearch.ToDate != DateTime.MinValue))
            {
                queryBuilder.Append("and i.create_date >= to_date(\'" + reqSearch.FromDate.Value.ToString("MM/dd/yyyy") + "\','mm/dd/yyyy') and i.create_date <= to_date(\'" + reqSearch.ToDate.Value.ToString("MM/dd/yyyy") + "\','mm/dd/yyyy') ");
            }
            else if (reqSearch.ToDate.HasValue && !reqSearch.FromDate.HasValue)
            {
                queryBuilder.Append("and i.create_date <= to_date(\'" + reqSearch.ToDate.Value.ToString("MM/dd/yyyy") + "\','mm/dd/yyyy') ");
            }
            else if (reqSearch.FromDate.HasValue && !reqSearch.ToDate.HasValue)
            {
                queryBuilder.Append("and i.create_date >= to_date(\'" + reqSearch.FromDate.Value.ToString("MM/dd/yyyy") + "\','mm/dd/yyyy') ");                
            }



            //queryBuilder.Append("and i.create_date >= to_date(\'" + reqSearch.FromDate.ToString("MM/dd/yyyy") + "\','mm/dd/yyyy') and i.create_date <= to_date(\'" + reqSearch.ToDate.ToString("MM/dd/yyyy") + "\','mm/dd/yyyy') ");
            queryBuilder.Append("order by i.contact_planner_cd , i.create_date desc, i.production_status,  l.lw_vendor_no");
            
          
            IDataReader reader = ExecuteReader(queryBuilder.ToString());

            var result = (new DbHelper()).ReadData<RequisitionSearch>(reader);
            return result;
        }
       
    }
}
