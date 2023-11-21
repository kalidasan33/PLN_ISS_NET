using ISS.Core.Model.Information;
using ISS.DAL;
using ISS.Common;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Diagnostics;
using ISS.Common;
using System;

namespace ISS.Repository.Information
{
    public class InformationRepository : RepositoryBase
    {
        public IList<Releases> GetReleases(ReleasesSearch search)
        {
            StringBuilder query = new StringBuilder(string.Empty);

            query.Append("select decode(nvl(a.attribution_ind,'N'),'N',  decode(a.remote_update_cd ,'A' ,  'Waiting for AS400' ,'W','Inprocess AS400','R','Failed to create on AS400',  'F','Failed to create on AS400','B','Bom mismatch. Waiting refresh of AS400 BOM', 'P','Created on AS400',   'D','Pending Delete in ISS',          'Unknown state') , decode(a.remote_update_cd ,'A' ,  'Waiting for SAP/ANET' ,'W','Inprocess SAP/ANET','R','Failed to create on SAP/ANET',  'F','Failed to create on SAP/ANET','B','Bom mismatch. Waiting refresh of SAP/ANET BOM', 'P','Created on SAP/ANET',   'D','Pending Delete in ISS',          'Unknown state')  )   \"RemoteUpdateCode\"     ,  a.update_date \"UpdatedDate\",a.order_id \"OrderId\", nvl(d.selling_style_cd,b.selling_style_cd) \"SellingStyle\",nvl(d.style_cd,b.style_cd)\"StyleCode\", nvl(d.color_cd,b.color_cd) \"ColorCode\",nvl(d.attribute_cd,b.attribute_cd) \"AttributeCode\",c.size_short_desc \"SizeShortDesc\",b.demand_loc DCloc,  b.sew_plant \"SewPlant\",b.cut_plant \"CutPlant\",b.txt_plant \"textilePlant\", round( b.total_curr_order_qty/12,0) \"TotalCurrentOrderQuantity\" ,   nvl(x.xcpn_reason,nvl(a.remote_xcpn_reason,' ')) \"Reason\" , nvl(b.multi_sku, ' ') \"MultiSKU\",  a.super_order \"SuperOrder\",b.cutting_alt \"CuttingAlt\",round(b.lbs,0) \"FabricLbs\", round(b.greige_lbs,0) \"GreigeLbs\",b.create_bd_ind \"CreateBD\",b.dozens_only_ind \"DzOnly\" , decode(nvl(a.attribution_ind,'N'),'N','TTS','SAP/ANET')  RemoteSystem " +
                
              //",b.size_cd \"SizeCode\", d.style_cd \"StyleCode2\",d.color_cd \"ColorCode2\",  d.attribute_cd \"AttributeCode2\", d.size_cd \"SizeCode2\" " +

            " from iss_prod_order a , iss_prod_order_view b, item_size c, iss_prod_order_xcpn x, iss_prod_order_detail d, ( SELECT DISTINCT e.order_version , e.super_order , e.capacity_alloc , e.pipeline_category_cd FROM            iss_prod_order_detail e ) e" +

           "  where a.order_version = 1   and a.order_version = b.order_version and a.super_order = b.super_order and  a.production_status = 'R'    and a.iss_order_type_cd <> 'RQ'   and a.remote_release_date <= sysdate and a.remote_release_date is not null  and b.size_cd = c.size_cd (+)   and a.order_version = x.order_version (+)  and a.super_order = x.super_order (+) and x.order_version = d.order_version  (+) and x.order_label = d.order_label (+) and x.super_order = d.super_order (+) AND      a.order_version = e.order_version AND      a.super_order = e.super_order AND     NVL(e.capacity_alloc,0) = NVL(b.sew_work_center,0) AND      e.pipeline_category_cd = 'SEW'");//PFE- To handle with NVL


            if (!string.IsNullOrWhiteSpace(search.WorkCenter)) query.Append(" and e.capacity_alloc in ( " + search.WorkCenter + "  ) ");

            if (!string.IsNullOrWhiteSpace(search.Planner)) query.Append(" and a.planner_cd in ( " + search.Planner + "  ) ");

            if (!string.IsNullOrWhiteSpace(search.SellingStyle)) query.Append(" and b.selling_style_cd like '" + Val(search.SellingStyle) + "%' ");

            if (!string.IsNullOrWhiteSpace(search.MfgStyle)) query.Append(" and b.style_cd like '" + Val(search.MfgStyle) + "%' ");

            if (!string.IsNullOrWhiteSpace(search.Color)) query.Append(" and b.color_cd like '" + Val(search.Color) + "%' ");


            if (!string.IsNullOrWhiteSpace(search.Attribute)) query.Append(" and b.attribute_cd like '" + Val(search.Attribute) + "%' ");

            if (!string.IsNullOrWhiteSpace(search.SewPlant)) query.Append(" and b.sew_plant= '" + Val(search.SewPlant) + "' ");

            if (!string.IsNullOrWhiteSpace(search.CutPlant)) query.Append(" and b.cut_plant= '" + Val(search.CutPlant) + "' ");

            if (!string.IsNullOrWhiteSpace(search.TextilePlant)) query.Append(" and b.txt_plant= '" + Val(search.TextilePlant) + "' ");

            if (!string.IsNullOrWhiteSpace(search.LoginId)) query.Append(" and UPPER(a.user_id) = '" + Val(search.LoginId).ToUpper() + "' ");

            query.Append(" order by a.update_date desc,a.super_order,d.order_label,b.style_cd,b.color_cd,b.attribute_cd,b.size_cd");

            Debug.WriteLine(query.ToString());

            var date = DateTime.Now;
            IDataReader reader = ExecuteReader(query.ToString());

            var result = (new DbHelper()).ReadData<Releases>(reader);

            var ts = DateTime.Now - date;

            Debug.WriteLine("Generic Minuts -" + ts.Minutes + " Sec-" + ts.Seconds);

            //date = DateTime.Now;
            //DataTable dt = new DataTable();
            //reader = ExecuteReader(query.ToString());
            //dt.Load(reader);
            //ts = DateTime.Now - date;

            //Debug.WriteLine("Table Minuts -" + ts.Minutes + " Sec-" + ts.Seconds);

            return result;

        }

        public IList<ExceptionDetail> GetAS400Exceptions(string superOrder)
        {
            string query = " select  d.super_order \"SuperOrder\", d.style_cd \"Style\", d.color_cd \"Color\", d.attribute_cd \"Attribute\",b.size_short_desc \"SizeShortDesc\", nvl(x.xcpn_reason,'  ') \"Reason\"  from iss_prod_order_xcpn x, iss_prod_order_detail d , item_size b  where d.order_version = 1  and d.super_order = '" + Val(superOrder) + "'   and b.size_cd = d.size_cd (+)   and x.order_version = d.order_version   and x.order_label = d.order_label  ";


            IDataReader reader = ExecuteReader(query);

            var result = (new DbHelper()).ReadData<ExceptionDetail>(reader);
            return result;

        }

        public IList<SuggestedException> GetSuggestedExceptions(ReleasesSearch search)
        {
            StringBuilder query = new StringBuilder(string.Empty);

            query.Append("select a.style \"Style\",a.color \"Color\",a.attribute \"Atribute\",c.size_short_desc \"SizeShortDesc\",a.location \"DmdLoc\",b.CONFLICT_PATH \"MfgPath\",min(b.conflict_reason) \"Reason\",conflict_sku \"ConflictSKU\",a.order_size \"OrderSize\" ,a.MFG_REVISION_NO \"Revision\"    " +

            " from plan_order_description a , plan_order_conflict b, item_size c  " +

           "  where a.order_version = 20000001  and a.order_version = b.order_version  and a.order_label = b.order_label  ");

            if (!string.IsNullOrWhiteSpace(search.Style)) query.Append(" and a.style like '" + Val(search.Style) + "%' ");
            if (!string.IsNullOrWhiteSpace(search.Attribute)) query.Append(" and a.attribute like '" + Val(search.Attribute) + "%' ");
            if (!string.IsNullOrWhiteSpace(search.Planner)) query.Append(" and a.planner_code = '" + Val(search.Planner) + "' ");
            if (!string.IsNullOrWhiteSpace(search.Color)) query.Append(" and a.color like '" + Val(search.Color) + "%' ");

            query.Append(" and a.order_size = c.size_cd (+) ");

            query.Append(" group by a.style,a.color,a.attribute,c.size_short_desc,a.location,a.order_size,b.CONFLICT_PATH ,a.MFG_REVISION_NO,b.conflict_sku  ");

            //========================================== UNION ==========================================================
            query.Append(" union all ");
            //========================================== UNION ==========================================================


            query.Append(" select a.selling_style_cd style, a.selling_color_cd color,a.selling_attribute_cd attribute_cd,c.size_short_desc ,a.demand_loc location,a.MFG_PATH_ID,'Sew Workcenter defaulting to [CATCHALL]' conflict_reason  , min(a.garment_style) conflict_sku ,a.selling_size_cd order_size, a.MFG_REVISION_NO Revision    " +

          " from iss_prod_order_view a,item_size c  " +

         "  where order_version =1 and production_status = 'P' and make_or_buy_cd = 'M'  and SEW_WORK_CENTER =  '[CATCHALL]'    and a.selling_size_cd = c.size_cd (+)   ");

            if (!string.IsNullOrWhiteSpace(search.Style)) query.Append(" and a.selling_style_cd like '" + Val(search.Style) + "%' ");
            if (!string.IsNullOrWhiteSpace(search.Attribute)) query.Append(" and a.selling_attribute_cd like '" + Val(search.Attribute) + "%' "); ;
            if (!string.IsNullOrWhiteSpace(search.Planner)) query.Append(" and a.planner_cd = '" + Val(search.Planner) + "' ");
            if (!string.IsNullOrWhiteSpace(search.Color)) query.Append(" and a.selling_color_cd like '" + Val(search.Color) + "%' ");


            query.Append("  group by a.selling_style_cd , a.selling_color_cd,a.selling_attribute_cd, c.size_short_desc,a.selling_size_cd ,a.selling_size_cd, a.demand_loc,a.MFG_PATH_ID ,a.MFG_REVISION_NO   ");

            query.Append("order by 1,2,3,4,5,6  ");

            Debug.WriteLine(query.ToString());

            IDataReader reader = ExecuteReader(query.ToString());

            var result = (new DbHelper()).ReadData<SuggestedException>(reader);
            return result;

        }


        public IList<SuggestedExceptionDetial> GetSuggestedExceptionDetail(SuggestedExceptionDetial search)
        {
            string query = " select distinct b.conflict_reason \"ConflictReason\", b.conflict_path \"ConflictPath\" from plan_order_description a , plan_order_conflict b  where b.order_version = '" + (LOVConstants.GlobalOrderVersion + LOVConstants.GlobalOrderContant) + "'      and a.order_version = b.order_version      and a.order_label = b.order_label    " +
                "and a.style like '" + Val(search.Style) + "%' and a.color like '" + Val(search.Color) + "%'   " +
                "and a.attribute like '" + Val(search.Atribute) + "%' and a.order_size = '" + Val(search.OrderSize) + "'  and b.conflict_path = '" + Val(search.MfgPath) + "' and  b.CONFLICT_SKU = '" + Val(search.ConflictSKU) + "' and a.location = '" + Val(search.DmdLoc) + "' " +
                "order by b.conflict_path, b.conflict_reason   ";
            //string query = " select distinct b.conflict_reason,b.conflict_path from plan_order_description a , plan_order_conflict b, item_size c  where b.order_version = '" + (LOVConstants.GlobalOrderVersion + LOVConstants.GlobalOrderContant) + "'      and a.order_version = b.order_version      and a.order_label = b.order_label    " +
            //   "and a.style = '" + Val(search.Style) + "' and a.color = '" + Val(search.Color) + "'   " +
            //   "and a.attribute = '" + Val(search.Atribute) + "' and c.size_short_desc  = '" + Val(search.SizeShortDesc) + "' and c.size_cd = a.order_size  " +
            //   "order by b.conflict_path, b.conflict_reason   ";

            IDataReader reader = ExecuteReader(query);

            var result = (new DbHelper()).ReadData<SuggestedExceptionDetial>(reader);
            return result;

        }
        public IList<DCWorkOrder> GetDCWorkOrders(DCWorkOrderSearch search)
        {
            StringBuilder query = new StringBuilder(string.Empty);

            query.Append(" select   TO_DATE ( TO_CHAR ( a.create_date, 'MM/DD/YYYY' ),'MM/DD/YYYY') \"CreatedDate\",a.PLANT_CD \"Plant\",a.REQ_NBR \"RequestNumber\",a.PROJECT_NO  \"ProjectNumber\"     ,a.FROM_STYLE \"FromStyle\",a.FROM_COLOR_CD \"FromColor\", a.FROM_STYLE_ATTRIBUTE \"FromStyleAttribute\",a.FROM_SIZE_CD \"FromSizeCd\",a.TO_STYLE   \"ToStyle\"     ,a.TO_COLOR_CD \"ToColor\",a.TO_STYLE_ATTRIBUTE \"ToStyleAttribute\",a.TO_SIZE_CD \"ToSizeCd\",     a.ORIG_DOZENS \"OriginalDozens\",a.DOZENS_COMPLETE \"CompleteDozens\"     ,a.PENDING_DOZENS \"PendingDozens\" , a.EXPECTED_DATE \"ExpectedDate\",a.REMARKS  \"Remarks\"  " +

            " from ACI_Rework a " +

           "   where 1 = 1   ");

            if (!string.IsNullOrWhiteSpace(search.Plant)) query.Append(" and a.PLANT_CD like '" + Val(search.Plant) + "%' ");
            if (!string.IsNullOrWhiteSpace(search.RequestNumber)) query.Append(" and a.REQ_NBR like '" + Val(search.RequestNumber) + "%' ");
            if (!string.IsNullOrWhiteSpace(search.projectNumber)) query.Append(" and a.PROJECT_NO like '" + Val(search.projectNumber) + "%' ");
            if (!string.IsNullOrWhiteSpace(search.FromStyle)) query.Append(" and a.FROM_STYLE like '" + Val(search.FromStyle) + "%' ");
            if (!string.IsNullOrWhiteSpace(search.FromColor)) query.Append(" and a.FROM_COLOR_CD like '" + Val(search.FromColor) + "%' ");
            if (!string.IsNullOrWhiteSpace(search.FromAttribute)) query.Append(" and a.FROM_STYLE_ATTRIBUTE like '" + Val(search.FromAttribute) + "%' ");
            if (!string.IsNullOrWhiteSpace(search.FromSizeCd)) query.Append(" and a.FROM_SIZE_CD like '" + Val(search.FromSizeCd) + "%' ");
            if (!string.IsNullOrWhiteSpace(search.ToStyle)) query.Append(" and a.TO_STYLE like '" + Val(search.ToStyle) + "%' ");
            if (!string.IsNullOrWhiteSpace(search.ToColor)) query.Append(" and a.TO_COLOR_CD like '" + Val(search.ToColor) + "%' ");
            if (!string.IsNullOrWhiteSpace(search.ToAttribute)) query.Append(" and a.TO_STYLE_ATTRIBUTE like '" + Val(search.ToAttribute) + "%' ");
            if (!string.IsNullOrWhiteSpace(search.ToSizeCd)) query.Append(" and a.TO_SIZE_CD like '" + Val(search.ToSizeCd) + "%' "); 

            if ( search.ExpectedDate.HasValue) query.Append(" and a.EXPECTED_DATE = trunc(to_date('" +search.ExpectedDate.Value.ToString("yyyyMMdd") + "','YYYYMMDD')) ");
            if (!string.IsNullOrWhiteSpace(search.Remarks)) query.Append("  and LTRIM(UPPER(a.REMARKS)) LIKE UPPER ('" + Val(search.Remarks) + "%') "); 
            if ( search.hasRemarks) query.Append(" and a.REMARKS is not null  "); 

     
      

         
            query.Append(" order by a.PLANT_CD,a.REQ_NBR,a.PROJECT_NO,a.FROM_STYLE, a.FROM_COLOR_CD,a.FROM_STYLE_ATTRIBUTE ,a.FROM_SIZE_CD,a.TO_STYLE,a.TO_COLOR_CD, a.TO_STYLE_ATTRIBUTE,a.TO_SIZE_CD, a.EXPECTED_DATE   ");

            Debug.WriteLine(query.ToString());

            IDataReader reader = ExecuteReader(query.ToString());

            var result = (new DbHelper()).ReadData<DCWorkOrder>(reader);
            return result;

        }

        public IList<StyleException> GetStyleExceptions(StyleSearch search)
        {
            StringBuilder query = new StringBuilder(string.Empty);






            query.Append(" select  a.STYLE_CD \"StyleCode\", a.COLOR_CD \"ColorCode\", a.ATTRIBUTE_CD \"AttributeCode\", a.SIZE_CD    \"SizeShortDesc\"  , a.PRIMARY_DC \"PrimaryDC\",a.CORP_BUSINESS_UNIT  \"LOB\",a.PLANNER_CD  \"Planner\",a.WORK_CENTER_CD  \"WorkCenter\",a.QUANTITY  \"Demand\",a.MFG_PATH \"MFGPath\",decode(a.exception_cd,'MTLA',a.super_order,a.PROD_FAMILY_CD )  \"ProductFamily\",a.EXCEPTION_REASON  \"Reason\"" +

            " from AVYX_EXCEPTION a" +

           "   where 1=1  ");



            if (!string.IsNullOrWhiteSpace(search.StyleCode)) query.Append(" and style_cd like '" + Val(search.StyleCode) + "%' ");

            if (!string.IsNullOrWhiteSpace(search.ColorCode)) query.Append(" and color_cd like '" + Val(search.ColorCode) + "%' ");

            if (!string.IsNullOrWhiteSpace(search.AttributeCode)) query.Append(" and ATTRIBUTE_CD like '" + Val(search.AttributeCode) + "%' ");
            if (!string.IsNullOrWhiteSpace(search.SizeShortDesc)) query.Append(" and SIZE_CD like '" + Val(search.SizeShortDesc) + "%' ");

            if (!string.IsNullOrWhiteSpace(search.PrimaryDC)) query.Append(" and PRIMARY_DC= '" + Val(search.PrimaryDC) + "' ");

            if (!string.IsNullOrWhiteSpace(search.LOB)) query.Append(" and CORP_BUSINESS_UNIT= '" + Val(search.LOB) + "' ");

            if (!string.IsNullOrWhiteSpace(search.Planner)) query.Append(" and PLANNER_CD= '" + Val(search.Planner) + "' ");

            if (!string.IsNullOrWhiteSpace(search.WorkCenter)) query.Append(" and WORK_CENTER_CD= '" + Val(search.WorkCenter) + "' ");

            if (search.APS || search.AVYX || search.ISS || search.NET || search.CWC || search.MTLA)
            {

                query.Append(" and EXCEPTION_CD in ( " ); 
                string exceptionCodes = string.Empty;
                if (search.APS) exceptionCodes = FormatIn(exceptionCodes, PlaningBox.APS.ToString());
                if (search.AVYX) exceptionCodes = FormatIn(exceptionCodes, PlaningBox.AVYX.ToString());
                if (search.ISS) exceptionCodes = FormatIn(exceptionCodes, PlaningBox.ISS.ToString());
                if (search.NET) exceptionCodes = FormatIn(exceptionCodes, PlaningBox.NET.ToString());
                if (search.CWC) exceptionCodes = FormatIn(exceptionCodes, PlaningBox.CWC.ToString());
                if (search.MTLA) exceptionCodes = FormatIn(exceptionCodes, PlaningBox.MTLA.ToString());


                query.Append(exceptionCodes+" ) ");
              
            }



            query.Append(" order by a.STYLE_CD,a.COLOR_CD,a.ATTRIBUTE_CD,a.SIZE_CD,a.PRIMARY_DC     ,a.CORP_BUSINESS_UNIT ,a.PLANNER_CD,a.WORK_CENTER_CD,a.QUANTITY  ,a.MFG_PATH,a.EXCEPTION_REASON  ");

            Debug.WriteLine(query.ToString());
            var date =  DateTime.Now;
            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<StyleException>(reader);
            var ts = DateTime.Now - date;

            Debug.WriteLine("Generic Minuts -" + ts.Minutes + " Sec-" + ts.Seconds);

            //date = DateTime.Now;
            //DataTable dt = new DataTable();
            //reader = ExecuteReader(query.ToString());
            //dt.Load(reader);
            //ts = DateTime.Now - date;

            //Debug.WriteLine("Table Minuts -" + ts.Minutes + " Sec-" + ts.Seconds);
            return result;

        }

        public IList<WOTextileGroup> GetWOTextileGroup(StyleSearch search)
        {
            StringBuilder query = new StringBuilder(string.Empty);

            query.Append("     select   a.STYLE_CD \"StyleCode\",a.FABRIC_GROUP \"TextileGroup\", a.EXCEPTION_REASON \"Reason\"    , a.USER_ID \"UserId\"   ,to_date(to_char(CREATE_DATE,'mm/dd/yyyy'),'mm/dd/yyyy') \"CreatedDate\"   ,to_date(to_char(UPDATE_DATE,'mm/dd/yyyy'),'mm/dd/yyyy') \"UpdatedDate\"    " +

              " from AVYX_EXCEPTION a ");

            query.Append("   where EXCEPTION_CD in ('"+ LOVConstants.AVYXException.TXT+"') ");


            if (!string.IsNullOrWhiteSpace(search.StyleCode)) query.Append(" and a.STYLE_CD like '" + Val(search.StyleCode) + "%' ");

            query.Append(" Group by a.STYLE_CD,a.FABRIC_GROUP,a.EXCEPTION_REASON,a.USER_ID      ,to_char(a.CREATE_DATE,'mm/dd/yyyy'),to_char(a.UPDATE_DATE,'mm/dd/yyyy') order by a.STYLE_CD,a.FABRIC_GROUP,a.EXCEPTION_REASON,a.USER_ID  ,  \"CreatedDate\" ,\"UpdatedDate\"");

            Debug.WriteLine(query.ToString());

            IDataReader reader = ExecuteReader(query.ToString());

            var result = (new DbHelper()).ReadData<WOTextileGroup>(reader);
            return result;

        }

        public IList<BlownAwayLot> GetBlownAwayLots(StyleSearch search)
        {
            StringBuilder query = new StringBuilder(string.Empty);

            query.Append(" select   a.STYLE_CD \"StyleCode\",a.COLOR_CD \"ColorCode\",a.ATTRIBUTE_CD \"AttributeCode\" ,a.SIZE_CD \"SizeShortDesc\",a.PLANT_CD \"Plant\"    ,a.QUANTITY \"LotQuantity\", a.SUPER_ORDER \"LotId\" , a.PLANNER_CD \"Planner\",a.EXCEPTION_REASON   \"Reason\" " +

            " from AVYX_EXCEPTION a " +

           "  where EXCEPTION_CD in ('"+ LOVConstants.AVYXException.DEL +"')  "); //DEL

            if (!string.IsNullOrWhiteSpace(search.StyleCode)) query.Append(" and a.STYLE_CD like '" + Val(search.StyleCode) + "%' ");
            if (!string.IsNullOrWhiteSpace(search.ColorCode)) query.Append(" and a.COLOR_CD like '" + Val(search.ColorCode) + "%' ");
            if (!string.IsNullOrWhiteSpace(search.AttributeCode)) query.Append(" and a.ATTRIBUTE_CD like '" + Val(search.AttributeCode) + "%' ");
            if (!string.IsNullOrWhiteSpace(search.SizeShortDesc)) query.Append(" and a.SIZE_CD like '" + Val(search.SizeShortDesc) + "%' ");
            if (!string.IsNullOrWhiteSpace(search.Plant)) query.Append(" and a.PLANT_CD like '" + Val(search.Plant) + "%' ");
            if (!string.IsNullOrWhiteSpace(search.Planner)) query.Append(" and a.PLANNER_CD like '" + Val(search.Planner) + "%' ");
            if (!string.IsNullOrWhiteSpace(search.Reason)) query.Append(" and LTRIM(UPPER(a.EXCEPTION_REASON)) LIKE UPPER( '" + Val(search.Reason) + "%') ");


            query.Append(" order by a.STYLE_CD,a.COLOR_CD,a.ATTRIBUTE_CD,a.SIZE_CD,a.PLANT_CD ,a.QUANTITY, a.SUPER_ORDER, a.PLANNER_CD ,a.EXCEPTION_REASON   ");

            Debug.WriteLine(query.ToString());

            IDataReader reader = ExecuteReader(query.ToString());

            var result = (new DbHelper()).ReadData<BlownAwayLot>(reader);
            return result;

        }

        public IList<BulksToAvyx> GetBulksToAvyx(String FromDate, String ToDate)
        {
            StringBuilder query = new StringBuilder(string.Empty);

            query.Append(" select   ka.ka_bulk_nbr \"BulkNumber\",  ka.ka_style_cd \"StyleCode\",ka.ka_color_cd \"ColorCode\",ka.ka_attribute_cd \"AttributeCode\" ,ka.ka_size_cd   \"SizeShortDesc\",ka.aps_style_cd \"ApsStyleCode\"    ,ka.aps_color_cd  \"ApsColorCode\", ka.aps_attribute_cd  \"ApsAttributeCode\" ,ka.ka_size_cd   \"ApsSizeShortDesc\",ka.demand_weekend_date \"DemandWeekendDate\"    ,ka.curr_order_qty  \"CurrOrderQty\", ka.corp_business_unit  \"CorpBusinessUnit\" , ka.demand_source  \"DemandSource\",ka.processed_to_avyx   \"ProcessedToAvyx\" , ka.create_date  \"CreateDate\",ka.update_date   \"ReActivatedDate\" ,ka.update_user_id   \"ReActivatedBy\" " +

            " From  Ka_component_preprocessor ka " +

           "  Where    processed_to_avyx = 'A' ");

            if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
            {
                query.Append(" and trunc(ka.update_date) between to_Date('" + FromDate + "','mm/dd/yyyy') and to_Date('" + ToDate + "','mm/dd/yyyy') ");
            }
            query.Append(" order by  ka.ka_bulk_nbr ");

            Debug.WriteLine(query.ToString());

            IDataReader reader = ExecuteReader(query.ToString());

            var result = (new DbHelper()).ReadData<BulksToAvyx>(reader);
            return result;

        }
        public IList<BulksToAvyx> GetBulksToComplete(String FromDate, String ToDate)
        {
            StringBuilder query = new StringBuilder(string.Empty);

            query.Append(" select   ka.ka_bulk_nbr \"BulkNumber\",  ka.ka_style_cd \"StyleCode\",ka.ka_color_cd \"ColorCode\",ka.ka_attribute_cd \"AttributeCode\" ,ka.ka_size_cd   \"SizeShortDesc\",ka.aps_style_cd \"ApsStyleCode\"    ,ka.aps_color_cd  \"ApsColorCode\", ka.aps_attribute_cd  \"ApsAttributeCode\" ,ka.ka_size_cd   \"ApsSizeShortDesc\",ka.demand_weekend_date \"DemandWeekendDate\"    ,ka.curr_order_qty  \"CurrOrderQty\", ka.corp_business_unit  \"CorpBusinessUnit\" , ka.demand_source  \"DemandSource\",ka.processed_to_avyx   \"ProcessedToAvyx\" , ka.create_date  \"CreateDate\", ka.update_date   \"CompletedDate\" ,ka.update_user_id   \"CompletedBy\"  " +

            " From  Ka_component_preprocessor ka " +

           "  Where    processed_to_avyx = 'C' ");

            if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
            {
                query.Append(" and trunc(ka.update_date) between to_Date('" + FromDate + "','mm/dd/yyyy') and to_Date('" + ToDate + "','mm/dd/yyyy') ");
            }
            query.Append(" order by  ka.ka_bulk_nbr ");

            Debug.WriteLine(query.ToString());

            IDataReader reader = ExecuteReader(query.ToString());

            var result = (new DbHelper()).ReadData<BulksToAvyx>(reader);
            return result;

        }

        public IList<BulksToAvyx> GetBulksNoData(String FromDate, String ToDate)
        {
            StringBuilder query = new StringBuilder(string.Empty);

            query.Append(" select   ka.ka_bulk_nbr \"BulkNumber\",  ka.ka_style_cd \"StyleCode\",ka.ka_color_cd \"ColorCode\",ka.ka_attribute_cd \"AttributeCode\" ,ka.ka_size_cd   \"SizeShortDesc\",ka.aps_style_cd \"ApsStyleCode\"    ,ka.aps_color_cd  \"ApsColorCode\", ka.aps_attribute_cd  \"ApsAttributeCode\" ,ka.ka_size_cd   \"ApsSizeShortDesc\",ka.demand_weekend_date \"DemandWeekendDate\"    ,ka.curr_order_qty  \"CurrOrderQty\", ka.corp_business_unit  \"CorpBusinessUnit\" , ka.demand_source  \"DemandSource\",ka.processed_to_avyx   \"ProcessedToAvyx\" , ka.create_date  \"CreateDate\",ka.update_date   \"ReActivatedDate\" ,ka.update_user_id   \"ReActivatedBy\", ka.update_date   \"CompletedDate\" ,ka.update_user_id   \"CompletedBy\"  " +

            " From  Ka_component_preprocessor ka " +

           "  Where    processed_to_avyx = 'R' order by   ka.ka_bulk_nbr  ");

            Debug.WriteLine(query.ToString());

            IDataReader reader = ExecuteReader(query.ToString());

            var result = (new DbHelper()).ReadData<BulksToAvyx>(reader);
            return result;

        }

        public IList<BulksToAvyx> GetBulksToError(String FromDate, String ToDate)
        {
            StringBuilder query = new StringBuilder(string.Empty);

            query.Append(" Select ka.ka_bulk_nbr \"BulkNumber\",  ka.ka_style_cd \"StyleCode\",ka.ka_color_cd \"ColorCode\",ka.ka_attribute_cd \"AttributeCode\" ,ka.ka_size_cd   \"SizeShortDesc\",ka.aps_style_cd \"ApsStyleCode\"    ,ka.aps_color_cd  \"ApsColorCode\", ka.aps_attribute_cd  \"ApsAttributeCode\" ,ka.ka_size_cd   \"ApsSizeShortDesc\",ka.demand_weekend_date \"DemandWeekendDate\"    ,ka.curr_order_qty  \"CurrOrderQty\", ka.corp_business_unit  \"CorpBusinessUnit\" , ka.demand_source  \"DemandSource\",ka.processed_to_avyx   \"ProcessedToAvyx\" , ka.create_date  \"CreateDate\", ke.error_message \"ErrorMsg\" " +
            " From    Ka_component_preprocessor ka  , ka_bulk_errors ke Where ka.ka_bulk_nbr  = ke.ka_bulk_nbr and ka.ka_style_cd = ke.ka_style_cd " +
            " and ka.ka_color_cd = ke.ka_color_cd and ka.ka_attribute_cd = ke.ka_attribute_cd and ka.ka_size_cd = ke.ka_size_cd and Ka.processed_to_avyx = 'E' ");
 
            if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
            {
                query.Append(" and trunc(ka.update_date) between to_Date('" + FromDate + "','mm/dd/yyyy') and to_Date('" + ToDate + "','mm/dd/yyyy') ");
            }
            query.Append(" order by  ka.ka_bulk_nbr ");
            Debug.WriteLine(query.ToString());

            IDataReader reader = ExecuteReader(query.ToString());

            var result = (new DbHelper()).ReadData<BulksToAvyx>(reader);
            return result;

        }

        public string GetBulksActiveCount(String FromDate, String ToDate)
        {
            StringBuilder query = new StringBuilder(string.Empty);
            //query.Append(" select   Count(*)  From  Ka_component_preprocessor ka Where   ka.processed_to_avyx = 'A' " );
            query.Append(" SELECT COUNT(DISTINCT ka.ka_bulk_nbr)  ||' ('|| count(*)||')'  as count FROM Ka_component_preprocessor ka Where   ka.processed_to_avyx = 'A'");
            if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
            {
                query.Append("and trunc(ka.update_date) between to_Date('" + FromDate + "','mm/dd/yyyy') and to_Date('" + ToDate + "','mm/dd/yyyy') ");
            }
            var  result = ExecuteScalar(query.ToString());
            return result.ToString();

        }
        public string GetCompleteCount(String FromDate, String ToDate)
        {
            StringBuilder query = new StringBuilder(string.Empty);

            query.Append(" SELECT COUNT(DISTINCT ka.ka_bulk_nbr)  ||' ('|| count(*)||')'  as count From  Ka_component_preprocessor ka Where   ka.processed_to_avyx = 'C' ");

            if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
            {
                query.Append("and trunc(ka.update_date) between to_Date('" + FromDate + "','mm/dd/yyyy') and to_Date('" + ToDate + "','mm/dd/yyyy') ");
            }
            var result = ExecuteScalar(query.ToString());
            return result.ToString();
        }

        public string GetErrorCount(String FromDate, String ToDate)
        {
            StringBuilder query = new StringBuilder(string.Empty);
            query.Append(" SELECT COUNT(DISTINCT ka.ka_bulk_nbr)  ||' ('|| count(*)||')'  as count From  Ka_component_preprocessor ka  , ka_bulk_errors ke " +
                "  Where ka.ka_bulk_nbr  = ke.ka_bulk_nbr and ka.ka_style_cd = ke.ka_style_cd " +
                "  and ka.ka_color_cd = ke.ka_color_cd and ka.ka_attribute_cd = ke.ka_attribute_cd " +
                "  and ka.ka_size_cd = ke.ka_size_cd and Ka.processed_to_avyx = 'E' ");

            if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
            {
                query.Append(" and trunc(ka.update_date) between to_Date('" + FromDate + "','mm/dd/yyyy') and to_Date('" + ToDate + "','mm/dd/yyyy') ");
            }
            query.Append(" order by  ka.ka_bulk_nbr ");
            var result = ExecuteScalar(query.ToString());
            return result.ToString();


        }


        public string GetBulksPulledCount(String FromDate, String ToDate)
        {
            StringBuilder query = new StringBuilder(string.Empty);
            query.Append(" SELECT COUNT(DISTINCT ka.ka_bulk_nbr)  ||' ('|| count(*)||')'  as count From   Ka_preprocessor ka where 1=1 ");
            //query.Append(" AND ka.processed_to_os = 'Y' ");
            if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
            {
                query.Append(" and  trunc(Ka.original_create_date) between to_Date('" + FromDate + "','mm/dd/yyyy') and to_Date('" + ToDate + "','mm/dd/yyyy') ");
            }
            var result = ExecuteScalar(query.ToString());
            return result.ToString();

        }
        public string GetBulksSuccessfulCount(String FromDate, String ToDate)
        {
            StringBuilder query = new StringBuilder(string.Empty);
            query.Append(" SELECT COUNT(DISTINCT ka.ka_bulk_nbr)  ||' ('|| count(*)||')'  as count  from ka_preprocessor ka  ,bill_of_mtrls bm, external_sku_xref es,iss_reqsn r , sku_revision_graphic sg, graphic g, placement p, attribute a  where ka.style_cd = bm.parent_style (+) and bm.comp_style_cd = es.style_cd (+) and bm.comp_color_cd = es.color_cd (+) and bm.comp_attribute_cd = es.attribute_cd (+) and bm.comp_size_cd = es.size_cd (+) and sysdate >= bm.effect_begin_date (+) and sysdate <= bm.effect_end_date (+) and trim(to_char(ka.reqsn_id)) = r.reqsn_id (+) and bm.comp_style_cd = sg.style_cd (+) and bm.comp_color_cd = sg.color_cd (+) and bm.comp_attribute_cd = sg.attribute_cd (+) and bm.comp_size_cd = sg.size_cd (+) and sg.active_cd = 'A' and sg.graphic_cd = g.graphic_cd (+)  and g.active_cd = 'Y'   and sg.placement_cd = p.placement_cd (+)  and p.active_cd = 'Y' and bm.comp_attribute_cd = a.attribute_cd (+) and ka.processed_to_os = 'Y' ");
            if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
            {
                query.Append(" and trunc(Ka.create_date) between to_Date('" + FromDate + "','mm/dd/yyyy') and to_Date('" + ToDate + "','mm/dd/yyyy') ");
            }
            var result = ExecuteScalar(query.ToString());
            return result.ToString();
        }

        public string GetErrorosCount(String FromDate, String ToDate)
        {
            StringBuilder query = new StringBuilder(string.Empty);
            query.Append(" SELECT COUNT(DISTINCT ka.ka_bulk_nbr)  ||' ('|| count(*)||')'  as count from ka_preprocessor ka  ,bill_of_mtrls bm, external_sku_xref es,iss_reqsn r , sku_revision_graphic sg, graphic g, placement p , attribute a  where ka.style_cd = bm.parent_style (+) and bm.comp_style_cd = es.style_cd (+) and bm.comp_color_cd = es.color_cd (+) and bm.comp_attribute_cd = es.attribute_cd (+) and bm.comp_size_cd = es.size_cd (+) and sysdate >= bm.effect_begin_date (+) and sysdate <= bm.effect_end_date (+) and trim(to_char(ka.reqsn_id)) = r.reqsn_id (+) and bm.comp_style_cd = sg.style_cd (+) and bm.comp_color_cd = sg.color_cd (+) and bm.comp_attribute_cd = sg.attribute_cd (+) and bm.comp_size_cd = sg.size_cd (+) and sg.active_cd(+) = 'A' and sg.graphic_cd = g.graphic_cd (+)  and g.active_cd(+) = 'Y'   and sg.placement_cd = p.placement_cd (+)  and p.active_cd(+) = 'Y' and ka.processed_to_os = 'E' and bm.comp_attribute_cd = a.attribute_cd (+) ");
            if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
            {
                //query.Append(" and Ka.create_date between to_Date('" + FromDate + "','mm/dd/yyyy') and to_Date('" + ToDate + "','mm/dd/yyyy') ");
            }
            var result = ExecuteScalar(query.ToString());
            return result.ToString();


        }

        public string GetErrorosSecondCount(String FromDate, String ToDate)
        {
            StringBuilder query = new StringBuilder(string.Empty);
            query.Append(" SELECT COUNT(DISTINCT ka.ka_bulk_nbr)  ||' ('|| count(*)||')'  as count from ka_preprocessor ka, ka_bulk_errors ke  where ka.processed_to_os = 'E'  and ka.ka_bulk_nbr = ke.ka_bulk_nbr and ka.ka_line_nbr = ke.ka_line_nbr ");
            var result = ExecuteScalar(query.ToString());
            return result.ToString();


        }


        public IList<BulkstoOneSource> GetBulksToPulled(string FromDate, string ToDate)
        {
            StringBuilder query = new StringBuilder(string.Empty);

            //query.Append(" select   ka.ka_bulk_nbr \"BulkNumber\",  ka.ka_line_nbr \"LineNumber\" , ka.style_cd \"StyleCode\", ka.color_cd \"ColorCode\",ka.attribute_cd \"AttributeCode\" , ka.size_cd   \"SizeShortDesc\" " +

            query.Append(" select   ka.ka_bulk_nbr \"BulkNumber\", ka.ka_line_nbr \"LineNumber\" ,ka.style_cd \"StyleCode\",ka.color_cd \"ColorCode\",ka.attribute_cd \"AttributeCode\" ,ka.size_cd   \"SizeShortDesc\",ka.curr_due_date \"CurrDueDate\", trunc(Ka.create_date) \"CreateDate\", ka.contact_planner_cd \"ContactPlannerCd\"    ,ka.src_contact_cd  \"SrcContactCd\", ka.demand_loc  \"DemandLoc\" ,ka.corp_business_unit   \"CorpBusinessUnit\", ka.mfg_path_id \"MfgPathId\", ka.mfg_revision_no \"MfgRevisionNo\", ka.curr_order_qty \"CurrOrderQty\", ka.plant_cd \"PlantCd\", ka.processed_to_os \"ProcessedToOs\", ka.original_create_date \"OrgnCreateDate\" " +

            " From  ka_preprocessor ka WHERE 1=1" );

           //"  where ka.processed_to_os = 'Y' ");
            if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
            {
                query.Append(" and trunc(Ka.original_create_date) between to_Date('" + FromDate + "','mm/dd/yyyy') and to_Date('" + ToDate + "','mm/dd/yyyy') ");
            }
            query.Append(" ORDER BY ka.ka_bulk_nbr, ka.ka_line_nbr ");
            Debug.WriteLine(query.ToString());

            IDataReader reader = ExecuteReader(query.ToString());

            var result = (new DbHelper()).ReadData<BulkstoOneSource>(reader);
            return result;

        }
        public IList<BulkstoOneSource> GetBulksToSuccess(string FromDate, string ToDate)
        {
            StringBuilder query = new StringBuilder(string.Empty);

            query.Append(" select   ka.ka_bulk_nbr \"BulkNumber\", ka.ka_line_nbr \"LineNumber\" ,ka.style_cd \"StyleCode\",ka.color_cd \"ColorCode\",ka.attribute_cd \"AttributeCode\" ,ka.size_cd   \"SizeShortDesc\", ka.reqsn_id \"RequisitionId\" , ka.reqsn_create_date \"ReqsnCreateDate\", ka.apprv_response_date \"ApproveDate\", DECODE(r.reqsn_status_cd,'QS','Queued for Sourcing' , DECODE(r.reqsn_status_cd,'IS','In Sourcing','')) \"ReqsnStatus\" , ka.curr_order_qty \"CurrOrderQty\", bm.parent_style \"ParentStyle\" , bm.comp_style_cd \"CompStyle\", bm.comp_color_cd \"CompColor\", bm.comp_attribute_cd \"CompAttribute\" , bm.comp_size_cd \"CompSize\", es.corp_division_code \"CorpBusinessUnit\", es.external_style \"ExternalStyle\", es.external_attribute \"ExternalAttribute\", es.external_size \"ExternalSize\", es.external_version  \"ExternalVersion\" , es.external_attribute \"ExternalLogo\" , g.graphic_cd  \"Graphic\", p.placement_cd \"Placement\" " +

            " From  ka_preprocessor ka  ,bill_of_mtrls bm, external_sku_xref es,iss_reqsn r , sku_revision_graphic sg, graphic g, placement p, attribute a " +

           "  where ka.style_cd = bm.parent_style (+) and bm.comp_style_cd = es.style_cd (+) and bm.comp_color_cd = es.color_cd (+) and bm.comp_attribute_cd = es.attribute_cd (+) and bm.comp_size_cd = es.size_cd (+) and sysdate >= bm.effect_begin_date (+) and sysdate <= bm.effect_end_date (+) and trim(to_char(ka.reqsn_id)) = r.reqsn_id (+) and bm.comp_style_cd = sg.style_cd (+) and bm.comp_color_cd = sg.color_cd (+) and bm.comp_attribute_cd = sg.attribute_cd (+) and bm.comp_size_cd = sg.size_cd (+) and sg.active_cd = 'A' and sg.graphic_cd = g.graphic_cd (+)  and g.active_cd = 'Y'   and sg.placement_cd = p.placement_cd (+)  and p.active_cd = 'Y' and bm.comp_attribute_cd = a.attribute_cd (+) and ka.processed_to_os = 'Y' ");
            if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
            {
                query.Append(" and trunc(Ka.create_date) between to_Date('" + FromDate + "','mm/dd/yyyy') and to_Date('" + ToDate + "','mm/dd/yyyy')  ");
            }
            query.Append(" ORDER BY ka.ka_bulk_nbr, ka.ka_line_nbr ");
            Debug.WriteLine(query.ToString());

            IDataReader reader = ExecuteReader(query.ToString());

            var result = (new DbHelper()).ReadData<BulkstoOneSource>(reader);
            return result;

        }

        public IList<BulkstoOneSource> GetBulksOSNoData(string FromDate, string ToDate)
        {
            StringBuilder query = new StringBuilder(string.Empty);

            query.Append(" select   ka.ka_bulk_nbr \"BulkNumber\", ka.ka_line_nbr \"LineNumber\" ,ka.style_cd \"StyleCode\",ka.color_cd \"ColorCode\",ka.attribute_cd \"AttributeCode\" ,ka.size_cd   \"SizeShortDesc\", ka.reqsn_id \"RequisitionId\" , ka.reqsn_create_date \"ReqsnCreateDate\", ka.apprv_response_date \"ApproveDate\", DECODE(r.reqsn_status_cd,'QS','Queued for Sourcing' , DECODE(r.reqsn_status_cd,'IS','In Sourcing','')) \"ReqsnStatus\" , ka.curr_order_qty \"CurrOrderQty\", bm.parent_style \"ParentStyle\" , bm.comp_style_cd \"CompStyle\", bm.comp_color_cd \"CompColor\", bm.comp_attribute_cd \"CompAttribute\" , bm.comp_size_cd \"CompSize\", es.corp_division_code \"CorpBusinessUnit\", es.external_style \"ExternalStyle\", es.external_attribute \"ExternalAttribute\", es.external_size \"ExternalSize\", es.external_version  \"ExternalVersion\" , es.external_attribute \"ExternalLogo\" , g.graphic_cd  \"Graphic\", p.placement_cd \"Placement\" " +

            " From  ka_preprocessor ka  ,bill_of_mtrls bm, external_sku_xref es,iss_reqsn r , sku_revision_graphic sg, graphic g, placement p, attribute a " +

           "  where ka.style_cd = bm.parent_style (+) and bm.comp_style_cd = es.style_cd (+) and bm.comp_color_cd = es.color_cd (+) and bm.comp_attribute_cd = es.attribute_cd (+) and bm.comp_size_cd = es.size_cd (+) and sysdate >= bm.effect_begin_date (+) and sysdate <= bm.effect_end_date (+) and trim(to_char(ka.reqsn_id)) = r.reqsn_id (+) and bm.comp_style_cd = sg.style_cd (+) and bm.comp_color_cd = sg.color_cd (+) and bm.comp_attribute_cd = sg.attribute_cd (+) and bm.comp_size_cd = sg.size_cd (+) and sg.active_cd = 'A' and sg.graphic_cd = g.graphic_cd (+)  and g.active_cd = 'Y'   and sg.placement_cd = p.placement_cd (+)  and p.active_cd = 'Y' and bm.comp_attribute_cd = a.attribute_cd (+) and ka.processed_to_os = 'RS' ");
            if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
            {
                query.Append(" and Ka.create_date between to_Date('" + FromDate + "','mm/dd/yyyy') and to_Date('" + ToDate + "','mm/dd/yyyy')  ");
            }
            Debug.WriteLine(query.ToString());

            IDataReader reader = ExecuteReader(query.ToString());

            var result = (new DbHelper()).ReadData<BulkstoOneSource>(reader);
            return result;

        }

        public IList<BulkstoOneSource> GetBulksToErrorOS(string FromDate, string ToDate)
        {
            StringBuilder query = new StringBuilder(string.Empty);

            query.Append(" select   ka.ka_bulk_nbr \"BulkNumber\", ka.ka_line_nbr \"LineNumber\" ,ka.style_cd \"StyleCode\",ka.color_cd \"ColorCode\",ka.attribute_cd \"AttributeCode\" ,ka.size_cd   \"SizeShortDesc\", ka.reqsn_id \"RequisitionId\" , ka.reqsn_create_date \"ReqsnCreateDate\", ka.apprv_response_date \"ApproveDate\", DECODE(r.reqsn_status_cd,'QS','Queued for Sourcing' , DECODE(r.reqsn_status_cd,'IS','In Sourcing','')) \"ReqsnStatus\" , ka.curr_order_qty \"CurrOrderQty\", bm.parent_style \"ParentStyle\" , bm.comp_style_cd \"CompStyle\", bm.comp_color_cd \"CompColor\", bm.comp_attribute_cd \"CompAttribute\" , bm.comp_size_cd \"CompSize\", es.corp_division_code \"CorpBusinessUnit\", es.external_style \"ExternalStyle\", es.external_attribute \"ExternalAttribute\", es.external_size \"ExternalSize\", es.external_version  \"ExternalVersion\" , es.external_attribute \"ExternalLogo\" , g.graphic_cd  \"Graphic\", p.placement_cd \"Placement\" " +

            " From  ka_preprocessor ka  ,bill_of_mtrls bm, external_sku_xref es,iss_reqsn r , sku_revision_graphic sg, graphic g, placement p, attribute a " +

           "  where ka.style_cd = bm.parent_style (+) and bm.comp_style_cd = es.style_cd (+) and bm.comp_color_cd = es.color_cd (+) and bm.comp_attribute_cd = es.attribute_cd (+) and bm.comp_size_cd = es.size_cd (+) and sysdate >= bm.effect_begin_date (+) and sysdate <= bm.effect_end_date (+) and trim(to_char(ka.reqsn_id)) = r.reqsn_id (+) and bm.comp_style_cd = sg.style_cd (+) and bm.comp_color_cd = sg.color_cd (+) and bm.comp_attribute_cd = sg.attribute_cd (+) and bm.comp_size_cd = sg.size_cd (+) and sg.active_cd(+) = 'A' and sg.graphic_cd = g.graphic_cd (+)  and g.active_cd(+) = 'Y'   and sg.placement_cd = p.placement_cd (+)  and p.active_cd(+) = 'Y' and ka.processed_to_os = 'E' and bm.comp_attribute_cd = a.attribute_cd (+) ");
            if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
            {
                //query.Append(" and Ka.create_date between to_Date('" + FromDate + "','mm/dd/yyyy') and to_Date('" + ToDate + "','mm/dd/yyyy')  ");
            }
            query.Append(" ORDER BY ka.ka_bulk_nbr, ka.ka_line_nbr ");
            Debug.WriteLine(query.ToString());

            IDataReader reader = ExecuteReader(query.ToString());

            var result = (new DbHelper()).ReadData<BulkstoOneSource>(reader);
            return result;

        }

        public IList<BulkstoOneSource> GetBulksToErrorOSSecond(string FromDate, string ToDate)
        {
            StringBuilder query = new StringBuilder(string.Empty);

            query.Append(" 	select   ka.ka_bulk_nbr \"BulkNumber\", ka.ka_line_nbr \"LineNumber\" ,ka.style_cd \"StyleCode\",ka.color_cd \"ColorCode\",ka.attribute_cd \"AttributeCode\" ,ka.size_cd   \"SizeShortDesc\", ka.curr_due_date \"CurrDueDate\" , ka.create_date \"CreateDate\", ka.contact_planner_cd \"ContactPlannerCd\", ka.src_contact_cd \"SrcContactCd\", ka.demand_loc \"DemandLoc\" , ka.corp_business_unit \"CorpBusinessUnit\", ka.mfg_path_id \"MfgPathId\", ka.mfg_revision_no \"MfgRevisionNo\" , ka.curr_order_qty \"CurrOrderQty\", ka.plant_cd \"PlantCd\", ka.processed_to_os \"ProcessedToOs\", ke.error_message \"ErrMessage\" " +

            " from ka_preprocessor ka, ka_bulk_errors ke " +

           "  where ka.processed_to_os = 'E'  and ka.ka_bulk_nbr = ke.ka_bulk_nbr and ka.ka_line_nbr = ke.ka_line_nbr  ");

            query.Append(" ORDER BY ka.ka_bulk_nbr, ka.ka_line_nbr ");
            Debug.WriteLine(query.ToString());

            IDataReader reader = ExecuteReader(query.ToString());

            var result = (new DbHelper()).ReadData<BulkstoOneSource>(reader);
            return result;

            

        }
        public IList<KnightsApparelExpedite> GetKnightsApparelExpedite1(String FromDate, String ToDate, String StyleCode, String ColorCode, String AttributeCode, String SizeCode)
        {
            StringBuilder query = new StringBuilder(string.Empty);

            //query.Append(" select   ka.ka_bulk_nbr \"BulkNumber\",  ka.ka_line_nbr \"LineNumber\" , ka.style_cd \"StyleCode\", ka.color_cd \"ColorCode\",ka.attribute_cd \"AttributeCode\" , ka.size_cd   \"SizeShortDesc\" " +
            query.Append("SELECT   x.bulknumber , x.stylecode, x.colorcode, x.attributecode, x.sizecode, x.sizeshortdesc, x.shipdate, SUM ( grossrequirement ) grossrequirement, SUM ( intransittodc ) intransittodc, SUM ( packing ) packing, SUM ( issuedtowip ) issuedtowip, SUM ( onsite ) onsite, SUM ( notplanned ) notplanned FROM ");
            query.Append(" (Select s.demand_source  BulkNumber , s.style StyleCode, s.color ColorCode, s.attribute_cd AttributeCode, s.size_cd SizeCode, i.size_desc SizeShortDesc, s.demand_weekend_date ShipDate, s.current_qty  GrossRequirement, (CASE WHEN co80000.complete_qty > 0 and co90000.complete_qty = 0 then co80000.complete_qty else 0 end) InTransitToDC,(CASE WHEN ((co21000.complete_qty > 0 or co70019.complete_qty > 0) and co80000.complete_qty = 0) then (case when co21000.complete_qty = 0 then co70019.complete_qty else co21000.complete_qty end) else 0 end) Packing,(CASE WHEN (co19995.complete_qty > 0 or co19990.complete_qty > 0) and co21000.complete_qty = 0 then (case when co19995.complete_qty = 0 then co19990.complete_qty else co19995.complete_qty end) else 0 end) IssuedToWIP ,(CASE WHEN co19980.complete_qty > 0 and co19990.complete_qty = 0 then co19980.complete_qty else 0 end) OnSite,(CASE WHEN co19980.complete_qty = 0 and co19990.complete_qty = 0 then co19980.complete_qty else 0 end) NotPlanned " +
            " From corp_prod_order cp, corp_prod_order_operation co90000 ,corp_prod_order_operation co80000 ,corp_prod_order_operation co70019    ,corp_prod_order_operation co21000  ,corp_prod_order_operation co19980 ,corp_prod_order_operation co19990 ,corp_prod_order_operation co19995    ,sku_gross_attr_demand s, item_size i  " +
            //" --, avyx_demand_net_view a " +
            " Where cp.order_status_cd = '1' and cp.prod_order_no = co90000 .prod_order_no  and co90000 .operation_no = 90000 and cp.prod_order_no = co80000 .prod_order_no  and co80000.operation_no = 80000 and cp.prod_order_no = co70019 .prod_order_no  and co70019 .operation_no = 70019  and cp.prod_order_no = co21000.prod_order_no  and co21000.operation_no = 21000 and cp.prod_order_no = co19995.prod_order_no  and co19995.operation_no = 19995  and cp.prod_order_no = co19990.prod_order_no and co19990.operation_no = 19990 and cp.prod_order_no = co19980.prod_order_no and co19980.operation_no = 19980 and cp.style = s.style  and cp.color = s.color and cp.attribute_cd = s.attribute_cd and cp.size_cd = s.size_cd and cp.size_cd = i.size_cd ");
            query.Append(" AND    cp.style like '" + StyleCode + "%' ");
            query.Append(" AND    cp.color like '" + ColorCode + "%' ");
            query.Append(" AND    cp.attribute_cd like '" + AttributeCode + "%' ");
            query.Append(" AND    cp.size_cd like '" + SizeCode + "%' ");
            if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
            {
                query.Append(" and trunc(s.create_date) between to_Date('" + FromDate + "','mm/dd/yyyy') and to_Date('" + ToDate + "','mm/dd/yyyy') ");
            }
            query.Append(" ) x GROUP BY x.bulknumber, x.stylecode, x.colorcode, x.attributecode, x.sizecode, x.sizeshortdesc, x.shipdate "); 
            Debug.WriteLine(query.ToString());

            IDataReader reader = ExecuteReader(query.ToString());

            var result = (new DbHelper()).ReadData<KnightsApparelExpedite>(reader);
            return result;
                


        }

        public IList<KnightsApparelExpedite> GetKnightsApparelExpedite(String FromDate, String ToDate, String StyleCode, String ColorCode, String AttributeCode, String SizeCode)
        {
            StringBuilder query = new StringBuilder(string.Empty);

            //query.Append(" SELECT a.bulknumber, a.stylecode, a.colorcode, a.attributecode,a.sizecode,a.sizeshortdesc,a.demanddate,a.grossrequirement, a.NetDemandRequirement, CASE when a.intransittodc1 - (SUM ( a.grossrequirement ) OVER ( PARTITION BY a.stylecode, a.colorcode, a.attributecode, a.sizecode, a.sizeshortdesc ORDER BY a.demanddate RANGE UNBOUNDED PRECEDING )) >= 0 then a.grossrequirement else 0 end intransittodc, a.packing,  CASE when a.issuedtowip1 - (SUM ( a.grossrequirement ) OVER ( PARTITION BY a.stylecode, a.colorcode, a.attributecode, a.sizecode, a.sizeshortdesc ORDER BY a.demanddate RANGE UNBOUNDED PRECEDING )) >= 0 then a.grossrequirement else 0 end issuedtowip, a.onsite, ( CASE           WHEN (   ( CASE when a.intransittodc1 - (SUM ( a.grossrequirement ) OVER ( PARTITION BY a.stylecode, a.colorcode, a.attributecode, a.sizecode, a.sizeshortdesc ORDER BY a.demanddate RANGE UNBOUNDED PRECEDING )) >= 0 then a.grossrequirement else 0 end ) +  ( a.packing ) +  ( CASE when a.issuedtowip1 - (SUM ( a.grossrequirement ) OVER ( PARTITION BY a.stylecode, a.colorcode, a.attributecode, a.sizecode, a.sizeshortdesc ORDER BY a.demanddate RANGE UNBOUNDED PRECEDING )) >= 0 then a.grossrequirement else 0 end ) +  ( a.onsite )) = 0 THEN ( a.grossrequirement ) ELSE  ( a.notplanned1 ) END ) notplanned ");
            //query.Append("From (SELECT   x.bulknumber,x.stylecode,x.colorcode,x.attributecode,x.sizecode,x.sizeshortdesc,x.demanddate,  (x.net) NetDemandRequirement,  (x.grossrequirement) grossrequirement        ,SUM (x.intransittodc) intransittodc1,CASE WHEN ( SUM (x.grossrequirement)) = 144 THEN SUM (x.grossrequirement)  ELSE SUM (x.packing) END packing ,SUM (x.issuedtowip) issuedtowip1,SUM (x.onsite) onsite, Sum (x.notplanned) notplanned1  FROM     (SELECT s.demand_source bulknumber ");
            //query.Append(" ,s.style stylecode,s.color colorcode,s.attribute_cd attributecode,s.size_cd sizecode,i.size_desc sizeshortdesc,s.demand_weekend_date demanddate       ,s.current_qty grossrequirement, nvl(n1.current_qty,0) net, (CASE WHEN co80000.complete_qty > 0  AND co90000.complete_qty = 0  THEN co80000.complete_qty ELSE 0  END            ) intransittodc " +
            //" , (CASE WHEN (    (   co21000.complete_qty > 0  OR co70019.complete_qty > 0  ) AND co80000.complete_qty = 0 ) THEN (CASE WHEN co21000.complete_qty = 0 THEN co70019.complete_qty ELSE co21000.complete_qty END) ELSE 0  END) packing  " +
            //    //" --, avyx_demand_net_view a " +
            //    ", (CASE WHEN (   co19995.complete_qty > 0 OR co19990.complete_qty > 0 ) AND    co21000.complete_qty = 0 THEN (CASE WHEN co19995.complete_qty = 0 THEN co19990.complete_qty  ELSE co19995.complete_qty END ) ELSE 0  END) issuedtowip  " +
            //    ", (CASE   WHEN co19980.complete_qty > 0 AND co19990.complete_qty = 0 THEN co19980.complete_qty ELSE 0 END ) onsite " +
            //    ", (CASE WHEN co19980.complete_qty = 0 AND co19990.complete_qty = 0  THEN co19980.complete_qty ELSE 0  END ) notplanned " +
            //    " FROM   corp_prod_order cp,corp_prod_order_operation co90000,corp_prod_order_operation co80000,corp_prod_order_operation co70019,corp_prod_order_operation co21000 ,corp_prod_order_operation co19980,corp_prod_order_operation co19990,corp_prod_order_operation co19995          ,sku_gross_attr_demand s, avyx_attr_demand_driver n1, item_size i WHERE  cp.order_status_cd (+) = '1' " +
            //    " AND    cp.prod_order_no = co90000.prod_order_no(+) AND    co90000.operation_no(+) = 90000 AND    cp.prod_order_no = co80000.prod_order_no(+) AND    co80000.operation_no(+) = 80000  AND    cp.prod_order_no = co70019.prod_order_no(+)  AND    co70019.operation_no(+) = 70019   AND    cp.prod_order_no = co21000.prod_order_no(+)  AND    co21000.operation_no(+) = 21000 AND    cp.prod_order_no = co19995.prod_order_no(+)  AND    co19995.operation_no(+) = 19995     AND    cp.prod_order_no = co19990.prod_order_no(+) AND    co19990.operation_no(+) = 19990  AND    cp.prod_order_no = co19980.prod_order_no(+) AND    co19980.operation_no(+) = 19980  " +
            //    " AND    cp.style (+) = s.style AND    cp.color (+) = s.color  AND    cp.attribute_cd (+) = s.attribute_cd  AND    cp.size_cd (+) = s.size_cd  AND s.demand_source = n1.demand_source(+)  AND s.style = n1.style(+) AND s.color = n1.color(+) AND s.attribute_cd = n1.attribute_cd(+) AND s.size_cd = n1.size_cd(+)  AND s.size_cd = i.size_cd(+)   ");
            ///*
            //query.Append(" AND    cp.style like '" + StyleCode + "%' ");
            //query.Append(" AND    cp.color like '" + ColorCode + "%' ");
            //query.Append(" AND    cp.attribute_cd like '" + AttributeCode + "%' ");
            //query.Append(" AND    i.size_desc like '" + SizeCode + "%' ");
            //if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
            //{
            //    query.Append(" and trunc(s.demand_weekend_date) between to_Date('" + FromDate + "','mm/dd/yyyy') and to_Date('" + ToDate + "','mm/dd/yyyy') ");
            //}
            // */
            //query.Append(" ) x " ) ;


            string strQry = (@"SELECT a.bulknumber,
                                     a.stylecode,
                                     a.colorcode,
                                     a.attributecode,
                                     a.sizecode,
                                     a.sizeshortdesc,
                                     a.demanddate,
                                     a.grossrequirement,
                                     a.netdemandrequirement,
                                     CASE
                                         WHEN   a.intransittodc1
                                              - (SUM (a.netdemandrequirement) --(SUM (a.grossrequirement)



                                                     OVER (PARTITION BY a.stylecode,
                                                                        a.colorcode,
                                                                        a.attributecode,
                                                                        a.sizecode,
                                                                        a.sizeshortdesc
                                                           ORDER BY a.demanddate
                                                           RANGE UNBOUNDED PRECEDING)) >= 0
                                         THEN
                                             a.netdemandrequirement --a.grossrequirement

                                         ELSE
                                             0
                                     END
                                         intransittodc,
                                              CASE
                                         WHEN   a.packing
                                              - (SUM (a.netdemandrequirement)
                                                     OVER (PARTITION BY a.stylecode,
                                                                        a.colorcode,
                                                                        a.attributecode,
                                                                        a.sizecode,
                                                                        a.sizeshortdesc
                                                           ORDER BY a.demanddate
                                                           RANGE UNBOUNDED PRECEDING)) >= 0
                                         THEN
                                             a.netdemandrequirement
                                         ELSE
                                             0
                                     END packing,
                                     CASE
                                         WHEN   a.issuedtowip1
                                              - (SUM (a.netdemandrequirement)



                                                     OVER (PARTITION BY a.stylecode,
                                                                        a.colorcode,
                                                                        a.attributecode,
                                                                        a.sizecode,
                                                                        a.sizeshortdesc
                                                           ORDER BY a.demanddate
                                                           RANGE UNBOUNDED PRECEDING)) >= 0
                                         THEN
                                             a.netdemandrequirement

                                         ELSE
                                             0
                                     END
                                         issuedtowip,
                                     CASE
                                         WHEN    a.onsite
                                              - (SUM (a.netdemandrequirement)
                                                     OVER (PARTITION BY a.stylecode,
                                                                        a.colorcode,
                                                                        a.attributecode,
                                                                        a.sizecode,
                                                                        a.sizeshortdesc
                                                           ORDER BY a.demanddate
                                                           RANGE UNBOUNDED PRECEDING)) >= 0
                                         THEN
                                             a.netdemandrequirement
                                         ELSE
                                             0
                                     END
                                         onsite        ,

                                     (CASE
                                          WHEN (  (CASE
                                                       WHEN   a.intransittodc1
                                                            - (SUM (a.netdemandrequirement)
                                                                   OVER (PARTITION BY a.stylecode,
                                                                                      a.colorcode,
                                                                                      a.attributecode,
                                                                                      a.sizecode,
                                                                                      a.sizeshortdesc
                                                                         ORDER BY a.demanddate
                                                                         RANGE UNBOUNDED PRECEDING)) >= 0
                                                       THEN
                                                           a.netdemandrequirement
                                                       ELSE
                                                           0
                                                   END)
                                                + (CASE
                                                       WHEN   a.packing
                                                            - (SUM (a.netdemandrequirement)



                                                                   OVER (PARTITION BY a.stylecode,
                                                                                      a.colorcode,
                                                                                      a.attributecode,
                                                                                      a.sizecode,
                                                                                      a.sizeshortdesc
                                                                         ORDER BY a.demanddate
                                                                         RANGE UNBOUNDED PRECEDING)) >= 0
                                                       THEN
                                                           a.netdemandrequirement

                                                       ELSE
                                                           0
                                                   END)

                                                + (CASE
                                                       WHEN   a.issuedtowip1
                                                            - (SUM (a.netdemandrequirement)
                                                                  OVER (PARTITION BY a.stylecode,




                                                                                      a.colorcode,
                                                                                      a.attributecode,
                                                                                      a.sizecode,
                                                                                      a.sizeshortdesc
                                                                         ORDER BY a.demanddate
                                                                         RANGE UNBOUNDED PRECEDING)) >= 0
                                                       THEN
                                                           a.netdemandrequirement

                                                       ELSE
                                                           0
                                                   END)
                                                + (CASE
                                                       WHEN   a.onsite
                                                            - (SUM (a.netdemandrequirement)
                                                                  OVER (PARTITION BY a.stylecode,
                                                                                      a.colorcode,
                                                                                      a.attributecode,
                                                                                      a.sizecode,
                                                                                      a.sizeshortdesc
                                                                         ORDER BY a.demanddate
                                                                         RANGE UNBOUNDED PRECEDING)) >= 0
                                                       THEN
                                                           a.netdemandrequirement
                                                       ELSE
                                                           0
                                                   END)) = 0

                                          THEN
                                              (a.netdemandrequirement)



                                          ELSE
                                              (a.notplanned1)
                                      END)
                                         notplanned
                                FROM (  SELECT x.bulknumber,
                                               x.stylecode,
                                               x.colorcode,
                                               x.attributecode,
                                               x.sizecode,
                                               x.sizeshortdesc,
                                               x.demanddate,
                                               (x.net) netdemandrequirement,
                                               (x.grossrequirement) grossrequirement,
                                               SUM (x.intransittodc) intransittodc1,
                                               CASE
                                                   WHEN (SUM (x.net)) = 144
                                                   THEN
                                                       SUM (x.net)



                                                   ELSE
                                                       SUM (x.packing)
                                               END
                                                   packing,
                                               SUM (x.issuedtowip) issuedtowip1,
                                               SUM (x.onsite) onsite,
                                               SUM (x.notplanned) notplanned1
                                          FROM (SELECT s.demand_source bulknumber,
                                                      s.style stylecode,
                                                       s.color colorcode,
                                                       s.attribute_cd attributecode,
                                                       s.size_cd sizecode,
                                                       i.size_desc sizeshortdesc,
                                                       s.demand_weekend_date demanddate,
                                                       s.current_qty grossrequirement,
                                                       NVL (n1.current_qty, 0) net,
                                                       (CASE
                                                            WHEN     co80000.complete_qty > 0
                                                                 AND co90000.complete_qty = 0
                                                            THEN
                                                                co80000.complete_qty
                                                            ELSE
                                                                0
                                                        END)
                                                           intransittodc,
                                                       (CASE
                                                            WHEN (    (   co21000.complete_qty > 0
                                                                       OR co70019.complete_qty > 0)
                                                                  AND co80000.complete_qty = 0)
                                                            THEN
                                                                (CASE
                                                                     WHEN co21000.complete_qty = 0
                                                                     THEN
                                                                         co70019.complete_qty
                                                                     ELSE
                                                                         co21000.complete_qty
                                                                 END)
                                                            ELSE
                                                                0
                                                        END)
                                                           packing,
                                                       (CASE
                                                            WHEN     (   co19995.complete_qty > 0
                                                                      OR co19990.complete_qty > 0)
                                                                 AND co21000.complete_qty = 0
                                                            THEN
                                                                (CASE
                                                                     WHEN co19995.complete_qty = 0
                                                                     THEN
                                                                         co19990.complete_qty
                                                                     ELSE
                                                                         co19995.complete_qty
                                                                 END)
                                                            ELSE
                                                                0
                                                        END)
                                                           issuedtowip,
                                                       (CASE
                                                            WHEN     co19980.complete_qty > 0
                                                                 AND co19990.complete_qty = 0
                                                            THEN
                                                                co19980.complete_qty
                                                            ELSE
                                                                0
                                                        END)
                                                           onsite,
                                                       (CASE
                                                            WHEN     co19980.complete_qty = 0
                                                                 AND co19990.complete_qty = 0
                                                            THEN
                                                                co19980.complete_qty
                                                            ELSE
                                                                0
                                                        END)
                                                           notplanned
                                                  FROM corp_prod_order cp,
                                                       corp_prod_order_operation co90000,
                                                       corp_prod_order_operation co80000,
                                                       corp_prod_order_operation co70019,
                                                       corp_prod_order_operation co21000,
                                                       corp_prod_order_operation co19980,
                                                       corp_prod_order_operation co19990,
                                                       corp_prod_order_operation co19995,
                                                       sku_gross_attr_demand s,
                                                       avyx_attr_demand_driver n1,
                                                       item_size i
                                                 WHERE     cp.order_status_cd(+) = '1'
                                                       AND cp.prod_order_no = co90000.prod_order_no(+)
                                                       AND co90000.operation_no(+) = 90000
                                                       AND cp.prod_order_no = co80000.prod_order_no(+)
                                                       AND co80000.operation_no(+) = 80000
                                                       AND cp.prod_order_no = co70019.prod_order_no(+)
                                                       AND co70019.operation_no(+) = 70019
                                                       AND cp.prod_order_no = co21000.prod_order_no(+)
                                                       AND co21000.operation_no(+) = 21000
                                                       AND cp.prod_order_no = co19995.prod_order_no(+)
                                                       AND co19995.operation_no(+) = 19995
                                                       AND cp.prod_order_no = co19990.prod_order_no(+)
                                                       AND co19990.operation_no(+) = 19990
                                                       AND cp.prod_order_no = co19980.prod_order_no(+)
                                                       AND co19980.operation_no(+) = 19980
                                                       AND cp.style(+) = s.style
                                                       AND cp.color(+) = s.color
                                                       AND cp.attribute_cd(+) = s.attribute_cd
                                                       AND cp.size_cd(+) = s.size_cd
                                                       AND s.demand_source = n1.demand_source(+)
                                                       AND s.style = n1.style(+)
                                                       AND s.color = n1.color(+)
                                                       AND s.attribute_cd = n1.attribute_cd(+)
                                                       AND s.size_cd = n1.size_cd(+)
                                                       AND s.size_cd = i.size_cd(+)) x ");
            query.Append(strQry);

            query.Append(" WHERE    x.stylecode like '" + StyleCode + "%' ");
            query.Append(" AND    x.colorcode like '" + ColorCode + "%' ");
            query.Append(" AND    x.attributecode like '" + AttributeCode + "%' ");
            query.Append(" AND    x.sizeshortdesc like '" + SizeCode + "%' ");
            if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
            {
                query.Append(" and trunc(x.demanddate) between to_Date('" + FromDate + "','mm/dd/yyyy') and to_Date('" + ToDate + "','mm/dd/yyyy') ");
            }

            query.Append("    GROUP BY x.bulknumber ,x.stylecode,x.colorcode,x.attributecode       ,x.sizecode,x.sizeshortdesc,x.demanddate, x.grossrequirement, x.net) a  ORDER BY a.demanddate ");
             
                       
            Debug.WriteLine(query.ToString());

            IDataReader reader = ExecuteReader(query.ToString());

            var result = (new DbHelper()).ReadData<KnightsApparelExpedite>(reader);
            return result;



        }
    }
}

