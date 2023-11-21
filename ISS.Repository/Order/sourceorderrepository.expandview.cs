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

namespace ISS.Repository.Order
{
    public partial class SourceOrderRepository : RepositoryBase
    {


        public List<RequisitionExpandView> GetReqExpandDetails(string requisitionId)
        {
            string reqTable = "";

            if (IsReqActive(requisitionId, 1))
                reqTable = "ISS_PROD_ORDER_VIEW";
            else
                reqTable = "ISS_PROD_ORDER_ARCHIVE_VIEW";


            var queryBuilder = new StringBuilder();

            queryBuilder.Append("select i.selling_style_cd \"Style\", i.selling_color_cd \"Color\", i.selling_attribute_cd \"Attribute\", i.selling_size_cd \"Size\", nvl(i.mfg_revision_no,0) \"Revision\" " +
              ", i.iss_program_type_cd, i.unit_of_measure \"UOM\", sum(I.curr_order_qty)  \"Eaches\", i.mfg_path_id  \"MfgPath\", sr.std_case_qty \"StdQty\", S.STYLE_DESC \"StyleDesc\" " +
              ", C.COLOR_DESC \"ColorDesc\", DECODE(A.ATTRIBUTE_DESC,'PLAIN GARMENTS','------',A.ATTRIBUTE_DESC) \"AttributeDesc\", nvl(Z.SIZE_DESC_LONG,'NONE')  \"SizeDesc\" " +
              ", Z.SIZE_SHORT_DESC \"SizeShortDesc\", nvl(SR.REVISION_DESC,'NONE')  \"RevisionDesc\", nvl(P.PACK_DESC,'NONE') \"PackDesc\", NVL(MP.MRKT_PROGRAM_SHORT_DESC,'NONE')  \"MPDesc\" ");
            queryBuilder.Append("FROM  " + reqTable + " I, STYLE S, COLOR C, ATTRIBUTE A, ITEM_SIZE Z, SKU_REVISION SR, PACK P, MRKT_PROGRAM MP ");
            queryBuilder.Append("WHERE  I.REQSN_VERSION = 1 AND I.SELLING_STYLE_CD = S.STYLE_CD AND I.SELLING_COLOR_CD = C.COLOR_CD " +
               "AND I.SELLING_ATTRIBUTE_CD = A.ATTRIBUTE_CD AND I.SELLING_SIZE_CD = Z.SIZE_CD AND I.SELLING_STYLE_CD = SR.STYLE_CD " +
               "AND I.SELLING_COLOR_CD = SR.COLOR_CD AND I.SELLING_ATTRIBUTE_CD = SR.ATTRIBUTE_CD " +
               "AND I.SELLING_SIZE_CD = SR.SIZE_CD AND NVL(I.MFG_REVISION_NO,0) = SR.REVISION_NO " +
               "AND I.PACK_CD = P.PACK_CD AND S.MRKT_PROGRAM_CD = MP.MRKT_PROGRAM_CD(+)  ");
            if (!String.IsNullOrWhiteSpace(requisitionId))
            {
                queryBuilder.Append(" AND I.REQSN_ID = '" + Val(requisitionId.ToUpper()) + "' ");
            }
            queryBuilder.Append("group by i.selling_style_cd, i.selling_color_cd, i.selling_attribute_cd, i.selling_size_cd " +
              ", nvl(i.mfg_revision_no,0), i.iss_program_type_cd, i.unit_of_measure, i.mfg_path_id " +
              ", sr.std_case_qty, S.STYLE_DESC, C.COLOR_DESC " +
              ", DECODE(A.ATTRIBUTE_DESC    ,'PLAIN GARMENTS','------',A.ATTRIBUTE_DESC), nvl(Z.SIZE_DESC_LONG,'NONE') " +
              ", Z.SIZE_SHORT_DESC, nvl(SR.REVISION_DESC,'NONE'), nvl(P.PACK_DESC,'NONE'), NVL(MP.MRKT_PROGRAM_SHORT_DESC,'NONE') " +
              "order by 1, 2, 3, 4, 5 ");


            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            IDataReader reader = ExecuteReader(queryBuilder.ToString());

            var result = (new DbHelper()).ReadData<RequisitionExpandView>(reader);
            return result;
        }

        public bool IsReqActive(string ReqsnId, decimal ReqsnVersion)
        {
            bool isReqActive = true;
            var queryBuilder = new StringBuilder();
            queryBuilder.Append("SELECT 'x' from iss_reqsn  where reqsn_version = '" + ReqsnVersion + "'  and reqsn_id = '" + Val(ReqsnId) + "' ");
            queryBuilder.Append(" union SELECT 'y' from iss_reqsn_archive  where reqsn_version = '" + ReqsnVersion + "'  and reqsn_id = '" + Val(ReqsnId) + "' ");

            var result = (String)ExecuteScalar(queryBuilder.ToString());
            if (result == "y")
            {
                isReqActive = false;
            }
            return isReqActive;
        }

        public IList<Requisition> GetRequisitionHeaderExpandView(String RequisitionId)
        {
            string reqTable = "";
            string query = "";

            if (IsReqActive(RequisitionId, 1))
                reqTable = "ISS_REQSN";
            else
                reqTable = "ISS_REQSN_ARCHIVE";


            query = " SELECT a.reqsn_id \"RequisitionId\", a.production_status \"ProdStatus\", a.reqsn_status_cd \"ReqStatus\", b.reqsn_status_short_desc \"ReqStatusDesc\" ,  a.contact_planner_cd \"PlanningContact\", c.planner_name \"PlannerName\", a.src_contact_cd \"SourcingContact\", d.src_contact_name \"SourcingContactName\", e.reqsn_approver_cd \"RequisitionApproverId\", a.reqsn_version \"RequisitionVersion\" " +
             " , e.reqsn_approver_name \"RequisitionApprover\", a.corp_business_unit \"BusinessUnit\", a.demand_loc \"DcLoc\", a.curr_due_date \"PlannedDcDate\",   a.season_year  || ' ' || a.season_name \"Season\", a.iss_program_type_cd \"ProType\", f.iss_program_type_desc \"ProTypeDesc\", a.detail_trkg_ind \"ReqDetailTrackingVal\",  a.apprv_submit_date  \"ApprovalSubmitted\" " +
             " , a.apprv_response_date \"Approved\", a.rlse_to_src_date, a.extr_by_src_date,  a.rej_by_src_date, nvl(a.vendor_id,0)  \"VendorId\", nvl(a.vendor_suffix,0) \"VendorSuffix\",  a.transp_mode_cd, g.transp_mode_name \"Mode\", round( a.over_pct *100,0) \"OverPercentageD\", round(a.under_pct *100,0) \"UnderPercentageD\", nvl(a.lw_company,0) \"LwCompany\" , nvl(a.lw_vendor_no,0) \"VendorNo\", a.lw_vendor_loc \"LwVendorLoc\", nvl(l.lw_vendor_vname,'')   \"VendorName\",  nvl(lw_vendor_city_addr5,'') vendor_city, nvl(l.lw_vendor_country,'') vendor_country, a.create_date  \"CreatedOn\", a.create_user_id \"CreatedBy\", a.update_date  \"UpdatedOn\", a.update_user_id \"UpdatedBy\" " +
               " FROM " + reqTable + " a,  REQSN_STATUS b, PLANNER c, SRC_CONTACT d, REQSN_APPROVER e ,  ISS_PROGRAM_TYPE f, TRANSP_MODE g, lawson_vendor l where a.reqsn_status_cd = b.reqsn_status_cd(+)  and a.contact_planner_cd = c.planner_cd(+) and a.src_contact_cd = d.src_contact_cd(+)  and a.reqsn_approver_cd = e.reqsn_approver_cd(+) " +
               "and a.iss_program_type_cd = f.iss_program_type_cd(+)  and a.transp_mode_cd = g.transp_mode_cd(+)  and a.lw_vendor_no = l.lw_vendor_no(+) and a.lw_vendor_loc = l.lw_vendor_loc(+)  and a.reqsn_version = 1 and a.reqsn_id = '" + Val(RequisitionId.ToUpper()) + "' ";


            IDataReader reader = ExecuteReader(query);

            var result = (new DbHelper()).ReadData<Requisition>(reader);

            if (result.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("select plant_cd || ' - ' || plant_name from plant where plant_cd = \'" + result[0].DcLoc + "\'");
                var dcLoc = (string)ExecuteScalar(sb.ToString());
                result[0].DcLocName = dcLoc;

                sb = new StringBuilder();
                sb.Append("select corp_business_unit || ' ' || corp_business_unit_desc from corp_business_unit  where corp_business_unit =" + " \'" + result[0].BusinessUnit + "\' ");
                dcLoc = (string)ExecuteScalar(sb.ToString());
                result[0].CropBusinessUnit = dcLoc;
            }

            return result;
        }

        public IList<RequisitionBOM> GetRequisitionBOM(RequisitionExpandView reqExView)
        {


            string query = " select 'LV1' BomLevel,b.comp_style_cd Style,s.style_desc  StyleDesc ,b.comp_color_cd Color,c.color_desc ColorDesc " +
                " ,b.comp_attribute_cd Attribute,b.comp_size_cd SizeCD,i.size_abbreviation SizeAbb,p.revision_no Revision,b.usage Usage,r.std_case_qty StdQty,b.bom_seq_no ParSeq,0 ChildSeq " +
              " from mfg_path p,bill_of_mtrls b,sku_revision r,style s,color c,item_size i " +
                  " where p.style_cd = '" + Val(reqExView.Style) + "' and p.color_cd = '" + Val(reqExView.Color) + "' and p.attribute_cd = '" + Val(reqExView.Attribute) + "' and p.size_cd = '" + Val(reqExView.Size) + "'  and p.mfg_path_id = '" + Val(reqExView.MfgPath) + "' " +
              " and p.revision_no = '" + reqExView.Revision + "' and b.parent_style = p.style_cd and b.parent_color = p.color_cd and b.parent_attribute = p.attribute_cd and b.parent_size = p.size_cd " +
              " and b.bill_of_mtrls_id = nvl(p.bill_of_mtrls_id,'DC') and r.style_cd = p.style_cd and r.color_cd = p.color_cd and r.attribute_cd = p.attribute_cd " +
              " and r.size_cd = p.size_cd and r.revision_no = p.revision_no and s.style_cd = b.comp_style_cd and c.color_cd = b.comp_color_cd and i.size_cd = b.comp_size_cd and s.matl_type_cd in ('00','01','02') " +
              " union select 'LV2' BomLevel ,b1.comp_style_cd Style,s1.style_desc  StyleDesc,b1.comp_color_cd Color,c1.color_desc ColorDesc,b1.comp_attribute_cd Attribute " +
               " ,b1.comp_size_cd SizeCD,i1.size_abbreviation SizeAbb,p1.revision_no Revision,b1.usage Usage,r1.std_case_qty StdQty,v.bom_seq_no par_seq,b1.bom_seq_no child_seq " +
              " from (select b.comp_style_cd style_cd,b.comp_color_cd color_cd,b.comp_attribute_cd attribute_cd,b.comp_size_cd size_cd,b.bom_seq_no,p.mfg_path_id mfg_path_id " +
                " ,s.mfg_revision_no mfg_revision_no from mfg_path p,bill_of_mtrls b,sku s " +
                   " where p.style_cd = '" + Val(reqExView.Style) + "' and p.color_cd = '" + Val(reqExView.Color) + "' and p.attribute_cd = '" + Val(reqExView.Attribute) + "' and p.size_cd = '" + Val(reqExView.Size) + "' and p.mfg_path_id = '" + Val(reqExView.MfgPath) + "' and p.revision_no = '" + reqExView.Revision + "' " +
                    " and b.parent_style = p.style_cd and b.parent_color = p.color_cd and b.parent_attribute = p.attribute_cd and b.parent_size = p.size_cd and b.bill_of_mtrls_id = nvl(p.bill_of_mtrls_id,'DC') " +
                    " and s.style_cd = b.comp_style_cd and s.color_cd = b.comp_color_cd and s.attribute_cd = b.comp_attribute_cd and s.size_cd = b.comp_size_cd) v ,mfg_path p1 " +
                " ,bill_of_mtrls b1,sku_revision r1,style s1,color c1,item_size i1 " +
           " where p1.style_cd = v.style_cd and p1.color_cd = v.color_cd and p1.attribute_cd = v.attribute_cd and p1.size_cd = v.size_cd and p1.mfg_path_id = v.mfg_path_id " +
             " and p1.revision_no = v.mfg_revision_no and b1.parent_style = p1.style_cd and b1.parent_color = p1.color_cd and b1.parent_attribute = p1.attribute_cd " +
            " and b1.parent_size = p1.size_cd and b1.bill_of_mtrls_id = nvl(p1.bill_of_mtrls_id,'DC') and r1.style_cd = p1.style_cd  and r1.color_cd = p1.color_cd  and r1.attribute_cd = p1.attribute_cd " +
            " and r1.size_cd = p1.size_cd and r1.revision_no = p1.revision_no and s1.style_cd = b1.comp_style_cd and c1.color_cd = b1.comp_color_cd and i1.size_cd = b1.comp_size_cd and s1.matl_type_cd in ('00','01','02') order by 12,13";


            IDataReader reader = ExecuteReader(query);

            var result = (new DbHelper()).ReadData<RequisitionBOM>(reader);
            return result;
        }

        public KeyValuePair<bool, String> RequisitionResetForConstruction(Requisition req)
        {
            bool resetToEdit = false;
            string Msg = "";
            try
            {                 
                var queryBuilder = new StringBuilder();
                queryBuilder.Append("BEGIN OPRSQL.iss_reqsn_pkg.RESET_FOR_EDIT(");
                queryBuilder.Append(Val(req.RequisitionVersion.ToString()) + ",\'" + Val(req.RequisitionId.ToUpper()) + "\'");
                queryBuilder.Append(");END;");
                var result = (String)ExecuteScalar(queryBuilder.ToString());
                resetToEdit = true;
            }
            catch (Exception ee)
            {
                Log(ee);
                Msg = ee.Message;
            }
            return new KeyValuePair<bool, string>(resetToEdit, Msg);

        }

        public bool ValidateVendorSuffixandId(Requisition req)
        {
            string SQL = "";
            bool validateVendor = false;
            if (((req.VendorNo <= 0)
                           || (req.LwVendorLoc == "")
                           || ((req.VendorId <= 0)
                           || ((req.VendorSuffix <= 0)
                           || (req.LwCompany <= 0)))))
            {
                validateVendor = false;
            }
            else
            {

                SQL = ("select \'x\' from svm_iss_poready_facilities where VENDOR_SUFFIX = "
                            + (req.VendorSuffix + (" and vendor_id = "
                            + (req.VendorId + " and rownum = 1"))));

                var result = (string)ExecuteScalar(SQL);

                if (result == "x")
                {
                    validateVendor = true;
                }
            }

            return validateVendor;
        }

        public bool ValidateForRelease(Requisition req)
        {
            bool validateForRelease = true;
            string SQL = "";
            //if (sec.IsReqsnPlanner(IRequisition_GetColumn("CORP_BUSINESS_UNIT").ToUpper()))
            //{

            //}

            if (ValidateVendorSuffixandId(req))
            {

                SQL = ("select style_cd \"Style\",color_cd \"Color\",attribute_cd \"Attribute\",nvl(i.size_short_desc,o.size_cd) \"Sizes\" " + (" from iss_prod_order_view o, item_size i where " + (" o.reqsn_version = "
                            + (req.RequisitionVersion + (" and o.reqsn_id  = \'"
                            + (req.RequisitionId + ("\' and o.plan_status_cd  <> \'A\' " + " and i.size_cd = o.size_cd (+)")))))));

                //IDataReader reader = ExecuteReader(SQL);

                //var result = (new DbHelper()).ReadData<SKU>(reader);

                //if (result.Count > 0)
                //{
                //    validateForRelease = true;
                //}
            }
            return validateForRelease;
        }

        public KeyValuePair<bool, String> ReleaseToSourcing(Requisition req)
        {
            bool resetToEdit = false;
            string Msg = "";
            try
            {
                if (ValidateForRelease(req))
                {
                    if (req.ShowSummaryOnly)
                    {
                        if (!SummarizeAndInsert(req))
                        {
                            return new KeyValuePair<bool, string>(resetToEdit, Msg);
                        }
                    }

                    var queryBuilder = new StringBuilder();
                    queryBuilder.Append("BEGIN OPRSQL.iss_reqsn_pkg.RELEASE_TO_SOURCING(");
                    queryBuilder.Append(req.RequisitionVersion.ToString() + ",\'" + Val(req.RequisitionId.ToUpper()) + "\' ");
                    queryBuilder.Append(");END;");

                    var result = (String)ExecuteScalar(queryBuilder.ToString());
                    resetToEdit = true;
                }
            }
            catch (Exception ee)
            {
                Log(ee);
                Msg = ee.Message;
            }

            return new KeyValuePair<bool, string>(resetToEdit, Msg);
        }

        private bool SummarizeAndInsert(Requisition req)
        {
            var requisitionData= GetRequisition(req.RequisitionId);
            
            var reqDet = GetRequisitionDetail(req.RequisitionId);
            if (requisitionData.Count>0 &&  reqDet.Count > 0)
            {
                SetRequisitionDefaults(requisitionData[0]);
                reqDet.GroupBy(eg => eg.getSKUString(true))
                    .ToList().ForEach(e =>
                         {
                             //e.Key;
                             var list = e.ToList();
                             var item = list.FirstOrDefault();
                             var listHide = list.Skip(1).ToList();
                             if (listHide.Count > 0)
                             {
                                 item.IsSummarized = item.IsDirty = true;
                                 item.SummarizedQty = item.Qty;
                                 item.Qty = list.Sum(t => t.Qty.ConvertDzToEaches());
                                 listHide.ForEach(p =>
                                 {
                                     p.IsDeleted= true;
                                     DeleteOrder(p);
                                 });
                                 requisitionData[0].ShowSummaryOnly = true;
                                 SetRequisitionDetailDefaults(requisitionData[0], item);
                                 UpdateOrderQty(requisitionData[0], item);
                             }
                         });
                return true;

            }
            return false;
        }

        public OrderComment GetOrderComments(Requisition req)
        {
            string arqComment = "", prqComment = "";
            OrderComment comment = new OrderComment();
            try
            {
                comment.RequisitionId = req.RequisitionId;
                comment.RequisitionVersion = req.RequisitionVersion;

                var queryBuilder = new StringBuilder();
                queryBuilder.Append("select order_note \"Comments\" from iss_reqsn_note ");
                queryBuilder.Append("where reqsn_id = \'" + Val(req.RequisitionId.ToUpper()) + "\' and reqsn_version = " + req.RequisitionVersion + " and iss_note_type_cd  = \'ARQ\' ");
                queryBuilder.Append("order by note_seq_no");

                //arqComment = (String)ExecuteScalar(queryBuilder.ToString());
                IDataReader reader = ExecuteReader(queryBuilder.ToString());

                var result = (new DbHelper()).ReadData<CommentNotes>(reader);
                if (result.Count > 0)
                {
                    List<string> lstAppr = new List<string>();
                    result.ForEach(c =>
                    {
                        lstAppr.Add(c.Comments);
                    });

                    arqComment = String.Join("\n", result.Select(x => x.Comments).ToList());

                    comment.ApproverCommentLst = lstAppr;
                    comment.ApproverComment = arqComment;
                }

                

                var queryBuilderPRQ = new StringBuilder();
                queryBuilderPRQ.Append("select order_note \"Comments\" from iss_reqsn_note ");
                queryBuilderPRQ.Append("where reqsn_id = \'" + Val(comment.RequisitionId.ToUpper()) + "\' and reqsn_version = " + comment.RequisitionVersion + " and iss_note_type_cd  = \'PRQ\' ");
                queryBuilderPRQ.Append("order by note_seq_no");

                //arqComment = (String)ExecuteScalar(queryBuilderPRQ.ToString());
                IDataReader readerPlanner = ExecuteReader(queryBuilderPRQ.ToString());

                var resultPlanner = (new DbHelper()).ReadData<CommentNotes>(readerPlanner);
                if (resultPlanner.Count > 0)
                {
                    List<string> lstPlnnr = new List<string>();
                    foreach (CommentNotes c in resultPlanner)
                    {
                        lstPlnnr.Add(c.Comments);
                    }
                    comment.PlannerCommentLst = lstPlnnr;

                    //resultPlanner.ForEach(c => {
                    //    comment.PlannerCommentLst.Add(c.Comments);
                    //});

                    prqComment = String.Join("\n", resultPlanner.Select(x => x.Comments).ToList());
                    comment.PlannerComment = prqComment;
                }


            }
            catch (Exception ee)
            {
                Log(ee);
            }
            return comment;

        }

    }
}
