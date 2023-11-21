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
using Oracle.DataAccess.Client;
using System.Data.Common;


namespace ISS.Repository.Order
{
    public partial class WorkOrderRepository : RepositoryBase
    {
        public WorkOrderRepository()
            : base()
        {

        }

        public WorkOrderRepository(DbTransaction trans)
            : base(trans)
        {
            
        }
        //Color Code based on Style
        public List<SKU> GetColorCode(string style)
        {
            var query = new StringBuilder();
            //query.Append("SELECT distinct s.STYLE_DESC \"StyleDesc\" ,a.color_cd \"Color\", a.attribute_cd \"Attribute\",a.size_cd \"Size\" FROM sku A,style s WHERE a.style_cd=s.style_cd and a.STYLE_CD = '" + Val(style) + "' and a.end_date_ind <> 'Y' and rownum<2 ");
            query.Append("SELECT distinct s.STYLE_DESC \"StyleDesc\" ,a.color_cd \"Color\",a.attribute_cd \"Attribute\",a.size_cd \"Size\" FROM sku A,style s,item_size i, sku_revision sr WHERE a.style_cd=s.style_cd and a.size_cd = i.size_cd and a.STYLE_CD = '" + Val(style) + "' and a.style_Cd = sr.style_Cd and a.color_cd = sr.color_Cd and a.attribute_cd = sr.attribute_cd and a.size_Cd = sr.size_Cd and a.end_date_ind <> 'Y' and rownum<2 ");
         
            //  query.Append("SELECT DISTINCT a.style_cd \"Style\", s.style_desc \"StyleDesc\" , a.color_cd \"Color\" , a.attribute_cd \"Attribute\" , a.size_cd \"Size\" , i.size_short_desc \"SizeShortDes\" ,   a.mfg_revision_no   \"Rev\", s.unit_of_measure \"UOM\" , DECODE ( s.unit_of_measure, 'DZ', 0.00, 0 ) \"Qty\", trunc(sr.std_case_qty/12,0)+ mod(sr.std_case_qty,12)/10 \"StdCase\" , 9999 \"Dpr\" FROM sku a, style s, item_size i,sku_revision sr  WHERE a.style_cd = s.style_cd AND i.size_cd = a.size_cd AND a.style_cd = sr.STYLE_CD AND a.style_cd ='" + Val(style) + "' and rownum<2 ");

            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<SKU>(reader);
            if (result != null && result.Count > 0)
            {
                result[0].Dpr = 9999M;
                var sizeList = GetSize(style, result[0].Color, result[0].Attribute);
                var siz = sizeList.FirstOrDefault(r => r.Size == result[0].Size);
                if (siz != null)
                {
                    result[0].SizeShortDes = siz.SizeShortDes;
                    result[0].Uom = siz.Uom;
                    result[0].StdCase = siz.StdCase;
                    result[0].Rev = siz.Rev;
                    var stdList=GetStdCaseQty(style, result[0].Color, result[0].Attribute, result[0].Size, siz.Rev.ToString());
                    if (stdList.Count > 0)
                    {
                        result[0].StdCase = stdList[0].StdCase;
                    }
                }

            }
            return result;
        }
        public List<SKU> GetAOColorCode(string style)
        {
            var query = new StringBuilder();
            //query.Append("SELECT distinct s.STYLE_DESC \"StyleDesc\" ,a.color_cd \"Color\", a.attribute_cd \"Attribute\",a.size_cd \"Size\" FROM sku A,style s WHERE a.style_cd=s.style_cd and a.STYLE_CD = '" + Val(style) + "' and a.end_date_ind <> 'Y' and rownum<2 ");
            query.Append("SELECT distinct s.STYLE_DESC \"StyleDesc\" ,a.color_cd \"Color\", decode(a.attribute_cd,'------', null, a.attribute_cd) \"Attribute\", decode(a.attribute_cd,'------',null, a.size_Cd) \"Size\" FROM sku A,style s,item_size i, sku_revision sr WHERE a.style_cd=s.style_cd and a.size_cd = i.size_cd and a.STYLE_CD = '" + Val(style) + "' and a.style_Cd = sr.style_Cd and a.color_cd = sr.color_Cd and a.attribute_cd = sr.attribute_cd and a.size_Cd = sr.size_Cd and a.end_date_ind <> 'Y' and rownum<2 ");
            //  query.Append("SELECT DISTINCT a.style_cd \"Style\", s.style_desc \"StyleDesc\" , a.color_cd \"Color\" , a.attribute_cd \"Attribute\" , a.size_cd \"Size\" , i.size_short_desc \"SizeShortDes\" ,   a.mfg_revision_no   \"Rev\", s.unit_of_measure \"UOM\" , DECODE ( s.unit_of_measure, 'DZ', 0.00, 0 ) \"Qty\", trunc(sr.std_case_qty/12,0)+ mod(sr.std_case_qty,12)/10 \"StdCase\" , 9999 \"Dpr\" FROM sku a, style s, item_size i,sku_revision sr  WHERE a.style_cd = s.style_cd AND i.size_cd = a.size_cd AND a.style_cd = sr.STYLE_CD AND a.style_cd ='" + Val(style) + "' and rownum<2 ");

            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<SKU>(reader);
            if (result != null && result.Count > 0)
            {
                result[0].Dpr = 9999M;
                var sizeList = GetSize(style, result[0].Color, result[0].Attribute);
                var siz = sizeList.FirstOrDefault(r => r.Size == result[0].Size);
                if (siz != null)
                {
                    result[0].SizeShortDes = siz.SizeShortDes;
                    result[0].Uom = siz.Uom;
                    result[0].StdCase = siz.StdCase;
                    result[0].Rev = siz.Rev;
                    var stdList = GetStdCaseQty(style, result[0].Color, result[0].Attribute, result[0].Size, siz.Rev.ToString());
                    if (stdList.Count > 0)
                    {
                        result[0].StdCase = stdList[0].StdCase;
                    }
                }

            }
            return result;
        }


        //Color Code Based on Style WO Detail
        public List<WorkOrderDetail> GetColorCodes(string style)
        {
            var query = new StringBuilder();

            query.Append("SELECT DISTINCT a.color_cd \"ColorCode\" from sku A where  ");
            if (!string.IsNullOrWhiteSpace(style)) query.Append("style_cd = '" + Val(style) + "'");
            query.Append(" and a.end_date_ind <> 'Y'");
            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<WorkOrderDetail>(reader);
            return result;
        }



        //Attribute Code and Description based on Style and Color WO Detail
        public List<WorkOrderDetail> GetAttributeCodes(string style, string color)
        {
            var query = new StringBuilder();
            query.Append(" SELECT distinct s.attribute_cd \"Attribute\", a.attribute_desc \"AttributeDesc\" FROM sku s, attribute a  WHERE s.attribute_cd = a.attribute_cd");
            query.Append(" and s.style_cd = '" + Val(style) + "'");
            query.Append(" and s.color_cd='" + Val(color) + "'");
            query.Append("and s.end_date_ind <> 'Y' order by  s.attribute_cd");
            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<WorkOrderDetail>(reader);
            return result;
        }


        //Attribute Code and Description based on Style and Color
        public List<SKU> GetAttribute(string style, string color)
        {
            var query = new StringBuilder();
            //query.Append("SELECT distinct s.attribute_cd \"Attribute\", a.attribute_desc \"AttributeDesc\" FROM sku s, attribute a  WHERE s.attribute_cd = a.attribute_cd");
            //query.Append("SELECT distinct a.attribute_cd \"Attribute\", attr.attribute_desc \"AttributeDesc\" , a.size_cd \"Size\" FROM sku a, attribute attr  WHERE a.attribute_cd = attr.attribute_cd");
            query.Append("SELECT distinct a.attribute_cd \"Attribute\", attr.attribute_desc \"AttributeDesc\" , a.size_cd \"Size\" FROM sku a, attribute attr,style s,item_size i, sku_revision sr WHERE a.attribute_cd = attr.attribute_cd and  a.style_cd=s.style_cd and a.size_cd = i.size_cd");
            query.Append(" and a.style_cd = '" + Val(style) + "' and a.style_Cd = sr.style_Cd ");
            query.Append(" and a.color_cd='" + Val(color) + "' and a.attribute_cd = sr.attribute_cd and a.size_Cd = sr.size_Cd ");
            query.Append(" and a.end_date_ind <> 'Y'");
            query.Append(" order by 1");
            //--and a.end_date_ind <> 'Y'"
            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<SKU>(reader);
            if (result != null && result.Count > 0)
            {
                result[0].Dpr = 9999M;
                var sizeList = GetSize(style, color, result[0].Attribute);
                var siz = sizeList.FirstOrDefault(r => r.Size == result[0].Size);
                if (siz != null)
                {
                    result[0].Size = siz.Size;
                    result[0].SizeShortDes = siz.SizeShortDes;
                    result[0].Uom = siz.Uom;
                    result[0].StdCase = siz.StdCase;
                    result[0].Rev = siz.Rev;

                }

            }
            return result;

        }

        public List<SKU> GetAOAttribute(string style, string color)
        {
            var query = new StringBuilder();//decode(a.attribute_cd,'------', null, a.attribute_cd) \"Attribute\", decode(a.attribute_cd,'------',null, a.size_Cd)
            //query.Append("SELECT distinct s.attribute_cd \"Attribute\", a.attribute_desc \"AttributeDesc\" FROM sku s, attribute a  WHERE s.attribute_cd = a.attribute_cd");
            //query.Append("SELECT distinct a.attribute_cd \"Attribute\", attr.attribute_desc \"AttributeDesc\" , a.size_cd \"Size\" FROM sku a, attribute attr  WHERE a.attribute_cd = attr.attribute_cd");
            query.Append("SELECT distinct decode(a.attribute_cd,'------', null, a.attribute_cd) \"Attribute\", attr.attribute_desc \"AttributeDesc\" , decode(a.attribute_cd,'------',null, a.size_Cd) \"Size\" FROM sku a, attribute attr,style s,item_size i, sku_revision sr WHERE a.attribute_cd = attr.attribute_cd and  a.style_cd=s.style_cd and a.size_cd = i.size_cd");
            query.Append(" and a.style_cd = '" + Val(style) + "' and a.style_Cd = sr.style_Cd ");
            query.Append(" and a.color_cd='" + Val(color) + "' and a.attribute_cd = sr.attribute_cd and a.size_Cd = sr.size_Cd ");
            query.Append(" and a.end_date_ind <> 'Y'");
            query.Append(" order by 1");
            //--and a.end_date_ind <> 'Y'"
            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<SKU>(reader);
            if (result != null && result.Count > 0)
            {
                result[0].Dpr = 9999M;
                var sizeList = GetSize(style, color, result[0].Attribute);
                var siz = sizeList.FirstOrDefault(r => r.Size == result[0].Size);
                if (siz != null)
                {
                    result[0].Size = siz.Size;
                    result[0].SizeShortDes = siz.SizeShortDes;
                    result[0].Uom = siz.Uom;
                    result[0].StdCase = siz.StdCase;
                    result[0].Rev = siz.Rev;

                }

            }
            return result;

        }
        //Size Short Desc and Size Code based on Style,Color,Attribute
        public List<SKU> GetSize(string style, string color, string attribute)
        {

            var query = new StringBuilder();


            query.Append("SELECT distinct i.size_short_desc  \"SizeShortDes\", a.size_cd \"Size\",  a.mfg_revision_no \"Rev\", s.unit_of_measure \"UOM\", trunc(sr.std_case_qty/12,0)+ mod(sr.std_case_qty,12)/100 \"StdCase\" , 9999 \"Dpr\"  FROM sku a, item_size i, style s,sku_revision sr  WHERE a.size_cd = i.size_cd and a.style_cd = s.style_cd AND a.style_cd = sr.STYLE_CD  and  a.size_cd = sr.size_cd and a.ATTRIBUTE_CD = sr.ATTRIBUTE_CD     and A.COLOR_CD=sr.COLOR_CD and a.end_date_ind <>'Y'    ");
            if (!string.IsNullOrWhiteSpace(style)) query.Append(" and A.STYLE_CD='" + Val(style) + "'");
            if (!string.IsNullOrWhiteSpace(color)) query.Append(" and A.COLOR_CD='" + Val(color) + "'");
            if (!string.IsNullOrWhiteSpace(attribute)) query.Append(" and A.ATTRIBUTE_CD='" + Val(attribute) + "'");
            query.Append(" order by i.size_short_desc ");

            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<SKU>(reader);
            return result;

        }

        public List<SKU> GetSiz(string style, string color, string attribute)
        {

            var query = new StringBuilder();


            query.Append("SELECT distinct i.size_short_desc  \"SizeShortDes\", a.size_cd \"Size\",  a.mfg_revision_no \"Rev\", s.unit_of_measure \"UOM\" , DECODE ( s.unit_of_measure, 'DZ', 0.00, 0 ) \"Qty\", trunc(sr.std_case_qty/12,0)+ mod(sr.std_case_qty,12)/100 \"StdCase\", 9999 \"Dpr\"  FROM sku a, item_size i, style s,sku_revision sr  WHERE a.size_cd = i.size_cd and a.style_cd = s.style_cd AND a.style_cd = sr.STYLE_CD");
            query.Append(" and A.STYLE_CD='" + Val(style) + "'");
            query.Append(" and A.COLOR_CD='" + Val(color) + "'");
            query.Append(" and A.ATTRIBUTE_CD='" + Val(attribute) + "'");
            query.Append(" and a.End_date_ind <> 'Y' ");
            query.Append(" order by  i.size_short_desc ");

            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<SKU>(reader);
            return result;

        }

        public List<WorkOrderDetail> GetSizes(string style, string color, string attribute)
        {
            var query = new StringBuilder();


            query.Append("select distinct i.size_short_desc \"SizeShortDes\",a.size_cd \"Size\" from sku a, item_size i where ");
            query.Append("  A.STYLE_CD='" + Val(style) + "'");
            query.Append(" and A.COLOR_CD='" + Val(color) + "'");
            query.Append(" and A.ATTRIBUTE_CD='" + Val(attribute) + "'");
            query.Append(" and a.End_date_ind <> 'Y' ");
            query.Append(" AND  a.size_cd = i.size_cd  order by i.size_short_desc ");


            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<WorkOrderDetail>(reader);
            return result;
        }

        public List<WorkOrderDetail> GetMaxRevisionsOld(string style, string color, string attribute, List<MultiSKUSizes> size)
        {
            var query = new StringBuilder();
            query.Append("select distinct mfg_revision_no \"MaxRevision\"  from sku  WHERE ");
            query.Append("STYLE_CD='" + Val(style) + "'");
            query.Append("AND COLOR_CD ='" + Val(color) + "'");
            query.Append("AND ATTRIBUTE_CD ='" + Val(attribute) + "'");
            query.Append("AND size_cd in (" + SizeList(size) + ")");
            query.Append(" and end_date_ind <> 'Y' ");
            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<WorkOrderDetail>(reader);
            return result;

        }
        
        public List<WorkOrderDetail> GetMaxRevisions(string style, string color, string attribute, List<MultiSKUSizes> size,string asrtCode)
        {
            return GetMaxRevisionsOld(style, color, attribute, size);
            var query = new StringBuilder();

            query.Append("select  max(mfg_revision_no) \"MaxRevision\" ");
             if (asrtCode == "A")
                query.Append("from mfg_sell_asrmt_sku_xref_view WHERE ");
            else
                query.Append("from mfg_selling_sku_xref WHERE ");
            query.Append("selling_style_cd='" + Val(style) + "'");
            query.Append("AND selling_color_cd ='" + Val(color) + "'");
            query.Append("AND selling_attribute_cd ='" + Val(attribute) + "'");
            query.Append("AND selling_size_cd in (" + SizeList(size) + ")");
            if (asrtCode == "A")
                query.Append(" and end_date_ind <> 'Y'    ");
            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<WorkOrderDetail>(reader);
            return result;

        }
        public List<SKU> GetMaxRevision(string style, string color, string attribute, string size)
        {

            var query = new StringBuilder();

            query.Append("select mfg_revision_no \"MaxRevision\"  from sku  WHERE ");
            if (!string.IsNullOrWhiteSpace(style)) query.Append("STYLE_CD='" + Val(style) + "'");
            if (!string.IsNullOrWhiteSpace(color)) query.Append("AND COLOR_CD ='" + Val(color) + "'");
            if (!string.IsNullOrWhiteSpace(attribute)) query.Append("AND ATTRIBUTE_CD ='" + Val(attribute) + "'");
            if (!string.IsNullOrWhiteSpace(size)) query.Append("AND size_cd ='" + Val(size) + "'");
            query.Append(" and end_date_ind <> 'Y' ");
            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<SKU>(reader);
            return result;

        }
        public List<SKU> GetUOM(string style, string color, string attribute, string size)
        {
            var query = new StringBuilder();
            query.Append("SELECT distinct s.unit_of_measure \"UOM\" FROM sku u, style s WHERE ");
            if (!string.IsNullOrWhiteSpace(style)) query.Append(" s.STYLE_CD='" + Val(style) + "'");
            if (!string.IsNullOrWhiteSpace(color)) query.Append("AND u.COLOR_CD ='" + Val(color) + "'");
            if (!string.IsNullOrWhiteSpace(attribute)) query.Append("AND u.ATTRIBUTE_CD ='" + Val(attribute) + "'");
            if (!string.IsNullOrWhiteSpace(size)) query.Append("AND u.size_cd ='" + Val(size) + "'");
            query.Append(" and u.end_date_ind <> 'Y' ");
            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<SKU>(reader);
            return result;

        }
        public List<SKU> GetMFG(string style, string color, string attribute, List<MultiSKUSizes> size, string pdc)
        {
            var query = new StringBuilder();
            query.Append("select a.mfg_path_id \"MfgPathId\",max(prime_mfg_location)\"SewPlt\" ,max(prime_mfg_location)\"SewPltMfg\" ,min( a.priority)\"Priority\" from ");
            query.Append("(select mfg_path_id,prime_mfg_location,5000 priority from mfg_path  where");
            query.Append(" style_cd ='" + Val(style) + "'");
            query.Append(" and color_cd = '" + Val(color) + "'");
            query.Append(" and attribute_cd ='" + Val(attribute) + "'");
            query.Append(" and size_cd in (" + SizeList(size) + ")");
            query.Append(" UNION ");
            query.Append("SELECT mfg_path_id, '' prime_mfg_location ,path_ranking_no priority  FROM PATH_RANKING  WHERE");
            query.Append(" style ='" + Val(style) + "'");
            query.Append(" AND color in ( '" + Val(color) + "','ALL')");
            query.Append(" AND attribute_cd ='" + Val(attribute) + "'");
            query.Append(" AND size_cd in (" + SizeList(size) + " ,'AL')");
            query.Append(" and demand_loc = '" + Val(pdc) + "'");
            query.Append(")a");
            query.Append(" group by a.mfg_path_id order by 4");

            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<SKU>(reader);
            return result;

        }

        public List<SKU> GetRevisionAndUom(string style, string color, string attribute, string size)
        {
            var query = new StringBuilder();
            query.Append("SELECT distinct s.unit_of_measure \"UOM\", u.mfg_revision_no  \"Rev\" FROM sku u, style s WHERE ");
            if (!string.IsNullOrWhiteSpace(style)) query.Append(" s.STYLE_CD='" + Val(style) + "'");
            if (!string.IsNullOrWhiteSpace(color)) query.Append("AND u.COLOR_CD ='" + Val(color) + "'");
            if (!string.IsNullOrWhiteSpace(attribute)) query.Append("AND u.ATTRIBUTE_CD ='" + Val(attribute) + "'");
            if (!string.IsNullOrWhiteSpace(size)) query.Append("AND u.size_cd ='" + Val(size) + "'");
            query.Append(" and u.end_date_ind <> 'Y' ");
            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<SKU>(reader);
            return result;
        }

        public List<SKU> GetRevisionNumbers(string style, string color, string attribute, string size)
        {
            //var query = new StringBuilder();
            //query.Append("SELECT distinct  u.mfg_revision_no \"Rev\" FROM sku u, style s WHERE ");
            //query.Append(" s.STYLE_CD='" + Val(style) + "'");
            //if (!string.IsNullOrWhiteSpace(color)) query.Append("AND u.COLOR_CD ='" + Val(color) + "'");
            //if (!string.IsNullOrWhiteSpace(attribute)) query.Append("AND u.ATTRIBUTE_CD ='" + Val(attribute) + "'");
            //if (!string.IsNullOrWhiteSpace(size)) query.Append("AND u.size_cd ='" + Val(size) + "'");
            //query.Append(" and u.end_date_ind <> 'Y' ");
            var query = new StringBuilder();
            query.Append("SELECT s.revision_no \"Rev\" FROM sku_revision s WHERE ");
            query.Append(" s.style_cd ='" + Val(style) + "'");
            if (!string.IsNullOrWhiteSpace(color)) query.Append("AND s.color_cd ='" + Val(color) + "'");
            if (!string.IsNullOrWhiteSpace(attribute)) query.Append("AND s.attribute_cd ='" + Val(attribute) + "'");
            if (!string.IsNullOrWhiteSpace(size)) query.Append("AND s.size_cd ='" + Val(size) + "'");
            query.Append(" and sysdate between s.begin_effect_date and s.end_effect_date order by 1 ");
            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<SKU>(reader);
            return result;
        }

        public List<SKU> GetStdCaseQty(string style, string color, string attribute, string size, string rev)
        {
            var query = new StringBuilder();
            query.Append("select trunc(std_case_qty/12,0)+ mod(std_case_qty,12)/100   \"StdCase\" from sku_revision WHERE");
            query.Append("  STYLE_CD='" + Val(style) + "'");
            if (!string.IsNullOrWhiteSpace(color)) query.Append(" AND COLOR_CD ='" + Val(color) + "'");
            if (!string.IsNullOrWhiteSpace(attribute)) query.Append(" AND ATTRIBUTE_CD ='" + Val(attribute) + "'");
            if (!string.IsNullOrWhiteSpace(size)) query.Append(" AND size_cd ='" + Val(size) + "'");
            if (!string.IsNullOrWhiteSpace(size)) query.Append(" AND revision_no ='" + Val(rev) + "'");

            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<SKU>(reader);
            return result;

        }

        public List<WorkOrderDetail> GetRevisionInLine(string style, string color, string attribute, List<MultiSKUSizes> size, string asrtCode, string revCode)
        {

            var query = new StringBuilder();
            query.Append(" select distinct m.mfg_style_cd \"PKGStyle\",  m.mfg_color_cd \"ColorCode\", m.mfg_attribute_cd \"Attribute\"");
            query.Append(", m.mfg_size_cd \"SizeShortDes\", m.mfg_revision_no \"NewRevision\", s.revision_desc \"RevDescription\" ");

            if (asrtCode == "A")
                query.Append("from mfg_sell_asrmt_sku_xref_view m,  sku_revision s ");
            else
                query.Append("from mfg_selling_sku_xref m,   sku_revision s");
            query.Append(" where    m.selling_style_cd = s.style_cd and m.mfg_color_cd = s.color_cd ");
            query.Append(" and m.mfg_attribute_cd = s.attribute_cd and m.mfg_size_cd = s.size_cd and m.mfg_revision_no = s.revision_no");
            query.Append(" and m.selling_style_cd = '" + Val(style) + "'");
            query.Append(" and m.selling_color_cd = '" + Val(color) + "'");
            query.Append(" and m.selling_attribute_cd = '" + Val(attribute) + "'");
            query.Append(" and  m.selling_size_cd in (" + SizeList(size) + ")");
            if (asrtCode == "A")
                query.Append("and end_date_ind <> 'Y'");
            else
                query.Append(" and DC_RECEIVE_IND = 'Y'");
            query.Append(" and  m.mfg_revision_no = '" + Val(revCode) + "'");

            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<WorkOrderDetail>(reader);
            return result;

        }
        public List<WorkOrderDetail> GetPKGCheck(string style, string color, string attribute, List<MultiSKUSizes> size, string asrtCode, string pkgCode)
        {

            var query = new StringBuilder();
            query.Append(" select distinct m.mfg_style_cd \"PKGStyle\",  m.mfg_color_cd \"ColorCode\", m.mfg_attribute_cd \"Attribute\"");
            query.Append(", m.mfg_size_cd \"SizeShortDes\", m.mfg_revision_no \"NewRevision\", s.revision_desc \"RevDescription\" ");

            if (asrtCode == "A")
                query.Append("from mfg_sell_asrmt_sku_xref_view m,  sku_revision s ");
            else
                query.Append("from mfg_selling_sku_xref m,   sku_revision s");
            query.Append(" where    m.selling_style_cd = s.style_cd and m.mfg_color_cd = s.color_cd ");
            query.Append(" and m.mfg_attribute_cd = s.attribute_cd and m.mfg_size_cd = s.size_cd and m.mfg_revision_no = s.revision_no");
            query.Append(" and m.selling_style_cd = '" + Val(style) + "'");
            query.Append(" and m.selling_color_cd = '" + Val(color) + "'");
            query.Append(" and m.selling_attribute_cd = '" + Val(attribute) + "'");
            query.Append(" and  m.selling_size_cd in (" + SizeList(size) + ")");
            if (asrtCode == "A")
                query.Append("and end_date_ind <> 'Y'");
            else
                query.Append(" and DC_RECEIVE_IND = 'Y'");
            query.Append(" and  m.mfg_revision_no = '" + Val(pkgCode) + "'");

            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<WorkOrderDetail>(reader);
            return result;

        }

        public List<WorkOrderDetail> GetRevisions(string style, string color, string attribute, List<MultiSKUSizes> size, string asrtCode)
        {

            var query = new StringBuilder();
            query.Append(" select distinct m.mfg_style_cd \"PKGStyle\",  m.mfg_color_cd \"ColorCode\", m.mfg_attribute_cd \"Attribute\"");
            query.Append(", m.mfg_size_cd \"SizeShortDes\", m.mfg_fmfg_no \"NewRevision\", s.revision_desc \"RevDescription\" ");

            if (asrtCode == "A")
                query.Append("from mfg_sell_asrmt_sku_xref_view m,  sku_revision s ");
            else
                query.Append("from mfg_selling_sku_xref m,   sku_revision s");
            query.Append(" where    m.selling_style_cd = s.style_cd and m.mfg_color_cd = s.color_cd ");
            query.Append(" and m.mfg_attribute_cd = s.attribute_cd and m.mfg_size_cd = s.size_cd and m.mfg_revision_no = s.revision_no");
            query.Append(" and m.selling_style_cd = '" + Val(style) + "'");
            query.Append(" and m.selling_color_cd = '" + Val(color) + "'");
            query.Append(" and m.selling_attribute_cd = '" + Val(attribute) + "'");
            query.Append(" and  m.selling_size_cd in (" + SizeList(size) + ")");
            if (asrtCode == "A")
                query.Append("and end_date_ind <> 'Y'");
            else
                query.Append(" and DC_RECEIVE_IND = 'Y'");


            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<WorkOrderDetail>(reader);
            return result;

        }
        //MFG PathId
        public List<WorkOrderDetail> GetMFGPathId(string style, string color, string attribute, List<MultiSKUSizes> size, string dLoc)
        {


            var query = new StringBuilder();
            query.Append("select a.mfg_path_id \"MfgPathId\",max(prime_mfg_location)\"SewPlt\"  , max(prime_mfg_location) \"SewPltMfg\", min( a.priority)\"Priority\" from ");
            query.Append("(select mfg_path_id,prime_mfg_location,5000 priority from mfg_path  where");
            query.Append(" style_cd ='" + Val(style) + "'");
            query.Append(" and color_cd = '" + Val(color) + "'");
            query.Append(" and attribute_cd ='" + Val(attribute) + "'");
            query.Append(" and size_cd in (" + SizeList(size) + ")");
            query.Append(" UNION ");
            query.Append("SELECT mfg_path_id, '' prime_mfg_location ,path_ranking_no priority  FROM PATH_RANKING  WHERE");
            query.Append(" style ='" + Val(style) + "'");
            query.Append(" AND color in ( '" + Val(color) + "','ALL')");
            query.Append(" AND attribute_cd ='" + Val(attribute) + "'");
            query.Append(" AND size_cd in (" + SizeList(size) + " ,'AL')");
            query.Append(" and demand_loc = '" + Val(dLoc) + "'");
            query.Append(")a");
            query.Append(" group by a.mfg_path_id order by 4");

            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<WorkOrderDetail>(reader);
            return result;

        }


        //Get RoutingId etc based on MFGPathId
        public List<WorkOrderDetail> GetRoutingId(string style, string color, string attribute, string size, string mfgPathId)
        {
            //color = "B4I" ;
            var query = new StringBuilder();
            query.Append("select a.mfg_path_id \"MfgPathId\",a.prime_mfg_location \"PrimeMFGLoc\",a.routing_id \"RoutingId\"");
            query.Append(",a.bill_of_mtrls_id \"BoMId\",nvl(a.scrap_factor,0) \"ScrapFactor\",nvl(b.planner_cd,'UNK') \"PlannerCd\"");
            query.Append(",nvl(a.make_or_buy_cd,'M') \"MorBCd\",nvl(s.unit_of_measure,'EA') \"UoM\",nvl(b.lead_time,0)*7 \"LeadTime\"");
            query.Append(",nvl(a.REVISION_NO,0) \"Revision\", nvl(p.capacity_alloc,'') \"CapacityAlloc\" from  MFG_PATH_MFG_REV a , MFG_PATH_CHP b ,style s, prod_family p where");
            query.Append(" a.style_cd = '" + Val(style) + "'");
            query.Append(" and a.color_cd = '" + Val(color) + "'");
            query.Append(" and a.attribute_cd = '" + Val(attribute) + "'");
            query.Append(" and a.size_cd = '" + Val(size) + "'");
            query.Append(" and a.mfg_path_id = '" + Val(mfgPathId) + "'");
            query.Append(" and a.effect_end_date >=sysdate and a.style_cd = b.style_cd (+)  AND a.color_cd = b.color_cd (+) AND a.attribute_cd = b.attribute_cd (+)  AND a.size_cd  = b.size_cd (+)");
            query.Append(" AND a.mfg_path_id = b.mfg_path_id (+)  AND a.style_cd  = s.style_cd (+) and s.corp_prod_family_cd = p.prod_family_cd (+) order by a.mfg_path_id, a.prime_mfg_location");
            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<WorkOrderDetail>(reader);
            return result;
        }



        public List<WorkOrderFabric> GetFabricGroup(string style, string color, string attribute, string size, string mfgPathId)
        {
            var query = new StringBuilder();
            query.Append("Select a.resource_id \"ResourceId\",upper(x.FABRIC_GROUP) \"FabricGroup\"");
            query.Append("from iss_garment_resource a, iss_style_fabric_xref x where");
            query.Append(" a.style_cd = '" + Val(style) + "'");
            query.Append(" and a.color_cd = '" + Val(color) + "'");
            query.Append(" and a.attribute_cd = '" + Val(attribute) + "'");
            query.Append(" and a.size_cd = '" + Val(size) + "'");
            query.Append(" and a.mfg_path_id = '" + Val(mfgPathId) + "'");
            query.Append("and a.style_cd = x.style_cd (+)");
            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<WorkOrderFabric>(reader);
            return result;
        }

        public List<WorkOrderDetail> GetBDDefault(string style, string color, string attribute, string size, string mfgPathId)
        {
            var query = new StringBuilder();
            query.Append("select nvl(BD_DEFAULT,'Y') \"CreateBDInd\" from MFG_PATH_CHP where");

            query.Append(" style_cd = '" + Val(style) + "'");
            query.Append(" and color_cd = '" + Val(color) + "'");
            query.Append(" and attribute_cd = '" + Val(attribute) + "'");
            query.Append(" and size_cd = '" + Val(size) + "'");
            query.Append(" and mfg_path_id = '" + Val(mfgPathId) + "'");

            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<WorkOrderDetail>(reader);
            return result;
        }



        public List<WorkOrderDetail> LoadOrderFromMaster(string style, string color, string attribute, string size, string routingId, string boMId)
        {
           // color = "B4I";
            var query = new StringBuilder();
            query.Append("select  comp_style_cd \"NewStyle\", comp_color_cd \"NewColor\", comp_attribute_cd \"NewAttribute\"");
            query.Append(", comp_size_cd \"NewSize\",b.activity_cd \"ActivityCode\", nvl(waste_factor,0) \"WasteFactor\", nvl(usage,0) \"Usuage\"");
            query.Append(", nvl(r.location_cd,'??') \"SupplyPlant\", matl_type_cd \"MatTypeCode\", inventory_check_ind \"InventoryCheckInd\"");
            query.Append(", bom_spec_id \"BomSpecificId\",b.spread_comp_cd \"CompCode\",cutting_alt \"CuttingAlt\",c.spread_type_cd \"SpreadCode\", null \"DyeShadeCode\" ");
            query.Append(", 0 \"CylSize\",0 \"FinishedWidth\", ''\"CutMethodCode\",0 \"MachineCut\",0,nvl(s.asrmt_cd,'') \"AssortCode\",i.size_short_desc \"NewSizeDes\" ");
            query.Append("FROM BILL_OF_MTRLS b, activity_routing r, style s, spread_comp c, item_size i where");
            query.Append(" parent_style = '" + Val(style) + "'");
            query.Append(" AND parent_color = '" + Val(color) + "'");
            query.Append(" AND parent_attribute = '" + Val(attribute) + "'");
            //Needs to add each n every
            query.Append(" and parent_size =  '" + Val(size) + "'");
            query.Append(" AND bill_of_mtrls_id ='" + Val(boMId) + "'");
            query.Append(" and r.routing_id ='" + Val(routingId) + "'");
            query.Append(" and comp_size_cd = i.size_cd  and b.activity_cd = r.activity_cd   and b.comp_style_cd = s.style_cd");
            query.Append(" and s.matl_type_cd in ('00','01','01','02','CT','F') and b.spread_comp_cd = c.spread_comp_cd (+)");
            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<WorkOrderDetail>(reader);
            return result;
        }



        //PKG Style
        public List<WorkOrderDetail> GetPKGStyle(string style, string color, string attribute, List<MultiSKUSizes> size, string revision, string asrtCode)
        {
            var query = new StringBuilder();
            if (asrtCode == "A")
                query.Append("select distinct m.mfg_style_cd \"PKGStyle\", m.MFG_REVISION_NO \"NewRevision\" from mfg_sell_asrmt_sku_xref_view m, item_size i, sku_revision s where m.selling_size_cd = i.size_cd and m.selling_style_cd = s.style_cd and m.mfg_color_cd = s.color_cd and m.mfg_attribute_cd = s.attribute_cd and m.mfg_size_cd = s.size_cd and m.mfg_revision_no = s.revision_no ");
            else
                query.Append("select distinct m.mfg_style_cd \"PKGStyle\" from mfg_selling_sku_xref m, item_size i, sku_revision s where m.selling_size_cd = i.size_cd and m.selling_style_cd = s.style_cd and m.mfg_color_cd = s.color_cd and m.mfg_attribute_cd = s.attribute_cd and m.mfg_size_cd = s.size_cd and m.mfg_revision_no = s.revision_no ");
            query.Append(" and m.selling_style_cd like'" + Val(style) + "'");
            query.Append(" and m.selling_color_cd like'" + Val(color) + "'");
            query.Append(" and m.selling_attribute_cd like'" + Val(attribute) + "'");
            query.Append(" and i.size_short_desc  in (" + SizeShortDesList(size) + ")");
            query.Append(" and m.revision_no='" + Val(revision) + "'");
            if (asrtCode == "A")
                query.Append(" and end_date_ind <> 'Y'");
            else
                query.Append(" and DC_RECEIVE_IND = 'Y'");
            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<WorkOrderDetail>(reader);
            return result;

        }

        //Newly Added
        public List<WorkOrderDetail> GetPathRankingAltId(string style, string color, string attribute, string size, string mfgPathId, string cutPath, string txtPath)
        {
            var resultwsize = new List<WorkOrderDetail>();
            var resultwcolor = new List<WorkOrderDetail>();
            var resultwcolorsize = new List<WorkOrderDetail>();
            var resultselling = new List<WorkOrderDetail>();
            var resultsellingwszie = new List<WorkOrderDetail>();
            var resultsellingwcolor = new List<WorkOrderDetail>();
            var resultsellingwcolorsize = new List<WorkOrderDetail>();
            var mfgquery = new StringBuilder();
            mfgquery.Append("Select Greige_Cloth_Path_ID as AlternateId From path_ranking p ");
            mfgquery.Append(" Where p.Mfg_Style_CD = '" + Val(style) + "'");
            mfgquery.Append(" and p.Mfg_color_cd ='" + Val(color) + "'");
            mfgquery.Append(" and p.Mfg_attribute_cd  ='" + Val(attribute) + "'");
            mfgquery.Append(" and p.Mfg_size_cd ='" + Val(size) + "'");
            mfgquery.Append(" and p.mfg_path_id ='" + Val(mfgPathId) + "'");
            mfgquery.Append(" and p.cut_part_path_id ='" + Val(cutPath) + "'");
            mfgquery.Append(" and p.finished_fabric_path_id ='" + Val(txtPath) + "'");
            mfgquery.Append(" and rownum = 1");
            mfgquery.Append(" Order by p.path_ranking_no desc");
            IDataReader reader = ExecuteReader(mfgquery.ToString());
            var result = (new DbHelper()).ReadData<WorkOrderDetail>(reader);
            if (result.Count != 0)
                return result;

            if (result.Count == 0)
            {
                var mfgquerywsize = new StringBuilder();
                mfgquerywsize.Append("Select Greige_Cloth_Path_ID as AlternateId From path_ranking p ");
                mfgquerywsize.Append(" Where p.Mfg_Style_CD = '" + Val(style) + "'");
                mfgquerywsize.Append(" and p.Mfg_color_cd ='" + Val(color) + "'");
                mfgquerywsize.Append(" and p.Mfg_attribute_cd  ='" + Val(attribute) + "'");
                mfgquerywsize.Append(" and p.Mfg_size_cd = 'AL'");
                mfgquerywsize.Append(" and p.mfg_path_id ='" + Val(mfgPathId) + "'");
                mfgquerywsize.Append(" and p.cut_part_path_id ='" + Val(cutPath) + "'");
                mfgquerywsize.Append(" and p.finished_fabric_path_id ='" + Val(txtPath) + "'");
                mfgquerywsize.Append(" and rownum = 1");
                mfgquerywsize.Append(" Order by p.path_ranking_no desc");
                IDataReader readerwsize = ExecuteReader(mfgquerywsize.ToString());
                resultwsize = (new DbHelper()).ReadData<WorkOrderDetail>(readerwsize);
                if (resultwsize.Count != 0)
                    return resultwsize;
            }

            if (resultwsize.Count == 0)
            {
                var mfgquerywscolor = new StringBuilder();
                mfgquerywscolor.Append("Select Greige_Cloth_Path_ID as AlternateId From path_ranking p ");
                mfgquerywscolor.Append(" Where p.Mfg_Style_CD = '" + Val(style) + "'");
                mfgquerywscolor.Append(" and p.Mfg_color_cd = 'ALL'");
                mfgquerywscolor.Append(" and p.Mfg_attribute_cd  ='" + Val(attribute) + "'");
                mfgquerywscolor.Append(" and p.Mfg_size_cd ='" + Val(size) + "'");
                mfgquerywscolor.Append(" and p.mfg_path_id ='" + Val(mfgPathId) + "'");
                mfgquerywscolor.Append(" and p.cut_part_path_id ='" + Val(cutPath) + "'");
                mfgquerywscolor.Append(" and p.finished_fabric_path_id ='" + Val(txtPath) + "'");
                mfgquerywscolor.Append(" and rownum = 1");
                mfgquerywscolor.Append(" Order by p.path_ranking_no desc");
                IDataReader readerwcolor = ExecuteReader(mfgquerywscolor.ToString());
                resultwcolor = (new DbHelper()).ReadData<WorkOrderDetail>(readerwcolor);
                if (resultwcolor.Count != 0)
                    return resultwcolor;
            }

            if (resultwcolor.Count == 0)
            {
                var mfgquerywcolorsize = new StringBuilder();
                mfgquerywcolorsize.Append("Select Greige_Cloth_Path_ID as AlternateId From path_ranking p ");
                mfgquerywcolorsize.Append(" Where p.Mfg_Style_CD = '" + Val(style) + "'");
                mfgquerywcolorsize.Append(" and p.Mfg_color_cd = 'ALL'");
                mfgquerywcolorsize.Append(" and p.Mfg_attribute_cd  ='" + Val(attribute) + "'");
                mfgquerywcolorsize.Append(" and p.Mfg_size_cd = 'AL'");
                mfgquerywcolorsize.Append(" and p.mfg_path_id ='" + Val(mfgPathId) + "'");
                mfgquerywcolorsize.Append(" and p.cut_part_path_id ='" + Val(cutPath) + "'");
                mfgquerywcolorsize.Append(" and p.finished_fabric_path_id ='" + Val(txtPath) + "'");
                mfgquerywcolorsize.Append(" and rownum = 1");
                mfgquerywcolorsize.Append(" Order by p.path_ranking_no desc");
                IDataReader readerwcolorsize = ExecuteReader(mfgquerywcolorsize.ToString());
                resultwcolorsize = (new DbHelper()).ReadData<WorkOrderDetail>(readerwcolorsize);
                if (resultwcolorsize.Count != 0)
                    return resultwcolorsize;
            }

            if (resultwcolorsize.Count == 0)
            {
                var sellingquery = new StringBuilder();
                sellingquery.Append("Select Greige_Cloth_Path_ID as AlternateId From path_ranking p ");
                sellingquery.Append("  Where p.Style = '" + Val(style) + "'");
                sellingquery.Append(" and p.color = '" + Val(color) + "'");
                sellingquery.Append(" and p.attribute_cd  ='" + Val(attribute) + "'");
                sellingquery.Append(" and p.size_cd = '" + Val(size) + "'");
                sellingquery.Append(" and p.mfg_path_id ='" + Val(mfgPathId) + "'");
                sellingquery.Append(" and p.cut_part_path_id ='" + Val(cutPath) + "'");
                sellingquery.Append(" and p.finished_fabric_path_id ='" + Val(txtPath) + "'");
                sellingquery.Append(" and rownum = 1");
                sellingquery.Append(" Order by p.path_ranking_no desc");
                IDataReader readerselling = ExecuteReader(sellingquery.ToString());
                resultselling = (new DbHelper()).ReadData<WorkOrderDetail>(readerselling);
                if (resultselling.Count != 0)
                    return resultselling;
            }

            if (resultselling.Count == 0)
            {
                var sellingquerywsize = new StringBuilder();
                sellingquerywsize.Append("Select Greige_Cloth_Path_ID as AlternateId From path_ranking p ");
                sellingquerywsize.Append("  Where p.Style = '" + Val(style) + "'");
                sellingquerywsize.Append(" and p.color = '" + Val(color) + "'");
                sellingquerywsize.Append(" and p.attribute_cd  ='" + Val(attribute) + "'");
                sellingquerywsize.Append(" and p.size_cd = 'AL'");
                sellingquerywsize.Append(" and p.mfg_path_id ='" + Val(mfgPathId) + "'");
                sellingquerywsize.Append(" and p.cut_part_path_id ='" + Val(cutPath) + "'");
                sellingquerywsize.Append(" and p.finished_fabric_path_id ='" + Val(txtPath) + "'");
                sellingquerywsize.Append(" and rownum = 1");
                sellingquerywsize.Append(" Order by p.path_ranking_no desc");
                IDataReader readersellingwsize = ExecuteReader(sellingquerywsize.ToString());
                resultsellingwszie = (new DbHelper()).ReadData<WorkOrderDetail>(readersellingwsize);
                if (resultsellingwszie.Count != 0)
                    return resultsellingwszie;
            }

            if (resultsellingwszie.Count == 0)
            {
                var sellingquerywcolor = new StringBuilder();
                sellingquerywcolor.Append("Select Greige_Cloth_Path_ID as AlternateId From path_ranking p ");
                sellingquerywcolor.Append("  Where p.Style = '" + Val(style) + "'");
                sellingquerywcolor.Append(" and p.color = 'ALL'");
                sellingquerywcolor.Append(" and p.attribute_cd  ='" + Val(attribute) + "'");
                sellingquerywcolor.Append(" and p.size_cd = '" + Val(size) + "'");
                sellingquerywcolor.Append(" and p.mfg_path_id ='" + Val(mfgPathId) + "'");
                sellingquerywcolor.Append(" and p.cut_part_path_id ='" + Val(cutPath) + "'");
                sellingquerywcolor.Append(" and p.finished_fabric_path_id ='" + Val(txtPath) + "'");
                sellingquerywcolor.Append(" and rownum = 1");
                sellingquerywcolor.Append(" Order by p.path_ranking_no desc");
                IDataReader readersellingwcolor = ExecuteReader(sellingquerywcolor.ToString());
                resultsellingwcolor = (new DbHelper()).ReadData<WorkOrderDetail>(readersellingwcolor);
                if (resultsellingwcolor.Count != 0)
                    return resultsellingwcolor;
            }

            if (resultsellingwcolor.Count == 0)
            {
                var sellingquerywcolorsize = new StringBuilder();
                sellingquerywcolorsize.Append("Select Greige_Cloth_Path_ID as AlternateId From path_ranking p ");
                sellingquerywcolorsize.Append("  Where p.Style = '" + Val(style) + "'");
                sellingquerywcolorsize.Append(" and p.color = 'ALL'");
                sellingquerywcolorsize.Append(" and p.attribute_cd  ='" + Val(attribute) + "'");
                sellingquerywcolorsize.Append(" and p.size_cd = 'AL'");
                sellingquerywcolorsize.Append(" and p.mfg_path_id ='" + Val(mfgPathId) + "'");
                sellingquerywcolorsize.Append(" and p.cut_part_path_id ='" + Val(cutPath) + "'");
                sellingquerywcolorsize.Append(" and p.finished_fabric_path_id ='" + Val(txtPath) + "'");
                sellingquerywcolorsize.Append(" and rownum = 1");
                sellingquerywcolorsize.Append(" Order by p.path_ranking_no desc");
                IDataReader readersellingwsizecolor = ExecuteReader(sellingquerywcolorsize.ToString());
                resultsellingwcolorsize = (new DbHelper()).ReadData<WorkOrderDetail>(readersellingwsizecolor);
                if (resultsellingwcolorsize.Count != 0)
                    return resultsellingwcolorsize;
            }

            return null;

        }
        //End


        //Planned Week
        public List<WorkOrderHeader> GetPlannedWeek(WorkOrderHeader wrkOrderHeader)
        {
            var query = new StringBuilder();

            query.Append("SELECT FISCAL_YEAR \"PlannedYear\",FISCAL_WEEK \"PlannedWeek\"  FROM FISCAL_CALENDAR where calendar_date =trunc(sysdate)");


            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<WorkOrderHeader>(reader);

            return result;

        }


        public decimal GetPlannedWeek()
        {
            var query = new StringBuilder();

            query.Append("SELECT FISCAL_WEEK \"PlannedWeek\"  FROM FISCAL_CALENDAR where calendar_date =trunc(sysdate)");


            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<WorkOrderHeader>(reader);
            var plannedweek = result.Select(x => x.PlannedWeek).FirstOrDefault();
            decimal dec = Convert.ToDecimal(plannedweek);
            return dec;

        }

        public decimal GetPlannedYear()
        {
            var query = new StringBuilder();

            query.Append("SELECT FISCAL_YEAR \"PlannedYear\"  FROM FISCAL_CALENDAR where calendar_date =trunc(sysdate)");


            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<WorkOrderHeader>(reader);
            var plannedyear = result.Select(x => x.PlannedYear).FirstOrDefault();
            decimal dec = Convert.ToDecimal(plannedyear);
            return dec;

        }

        public DateTime GetPlannedDate(decimal Week, decimal Year, string dueDate)
        {

            var query = new StringBuilder();
            if (dueDate == "DC")
                query.Append("SELECT max(week_end_date) \"PlannedDate\"  FROM FISCAL_CALENDAR where ");
            else
                query.Append("SELECT max(week_begin_date) \"PlannedDate\"  FROM FISCAL_CALENDAR where ");

            query.Append("fiscal_year=").Append(Year);
            query.Append("and fiscal_week=").Append(Week);
            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<WorkOrderHeader>(reader);
            DateTime plannedDate = result.Select(x => x.PlannedDate).FirstOrDefault();
           
            DateTime date = plannedDate.Date;


            return date;



        }

        public List<WorkOrderDetail> GetWOChildSKU(string style, string color, string attribute, string originTypeCode, string revision, List<MultiSKUSizes> size, string asrtCode)
        {
            var query = new StringBuilder();
            query.Append("select distinct comp_style_cd \"NewStyle\", comp_color_cd \"NewColor\",comp_attribute_cd \"NewAttribute\",comp_size_cd \"NewSize\"  from bill_of_mtrls b, style s, mfg_path m where comp_style_cd = s.style_cd");
            if(! String.IsNullOrEmpty(asrtCode)) query.Append(" and s.asrmt_cd = '" + Val(asrtCode) + "'");
            query.Append(" and s.origin_type_cd ='M'");
            query.Append(" and b.parent_style = m.style_cd and b.parent_color = m.color_cd  and b.parent_attribute = m.attribute_cd and b.parent_color = m.color_cd  and b.parent_attribute = m.attribute_cd and b.parent_size = m.size_cd And b.bill_of_mtrls_id = m.bill_of_mtrls_id and m.end_date_ind <> 'Y' ");
            query.Append(" and b.parent_style ='" + Val(style) + "'");
            query.Append(" and b.parent_color='" + Val(color) + "'");
            query.Append(" and b.parent_attribute = '" + Val(attribute) + "'");
            query.Append(" and b.parent_size in( " + SizeList(size) + ")");
            query.Append(" and m.revision_no = '" + Val(revision) + "'");
            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<WorkOrderDetail>(reader);
            return result;

        }


        //Assortment Code based on Style
        public List<WorkOrderDetail> GetWOAsrtCode(string style)
        {
            var query = new StringBuilder();

            query.Append("SELECT A.CORP_DIVISION_CD \"CorpDivisionCode\",A.MATL_TYPE_CD \"MatTypeCode\", B.CORP_BUSINESS_UNIT \"CorpBusUnit\",A.SIZE_LINE \"SizeLine\",A.MFG_CORP_DIVISION_CD \"CorpDivCode\"");
            query.Append(",A.QUALITY_CD \"QualityCode\",B.COLOR_DIVISION_CD \"ColDivCode\",A.ORIGIN_TYPE_CD \"OriginTypeCode\",A.GTIN_LABEL_CD \"GtinLabcode\", A.INVENTORY_CHECK_IND \"InvChkInd\"");
            query.Append(", A.PACK_CD \"PackCode\",A.UNIT_OF_MEASURE \"UoM\", nvl(s.prod_family_cd,'----') \"ProdFamCode\", nvl(s.product_class_cd,'----') \"ProdClassCode\"");
            query.Append(",A.PRIMARY_DC \"PrimaryDC\", A.STYLE_DESC \"StyleDes\",P.PACKAGE_QTY \"PKGQty\",A.ASRMT_CD \"AssortCode\" FROM STYLE A, STYLE_CHP S, CORP_DIVISION B, PACK P where");
            query.Append(" A.style_cd ='" + Val(style) + "'");
            query.Append(" AND A.CORP_DIVISION_CD  = B.CORP_DIVISION_CD and a.pack_cd = p.pack_cd and a.style_cd = s.style_cd (+)");
            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<WorkOrderDetail>(reader);
            return result;
        }
        //Fabric Details


        public List<WorkOrderDetail> LoadFabricGrid(string style, string color, string attribute, string size, string BoMId, string routingId)
        {
            var query = new StringBuilder();
            query.Append("select /*+ INDEX(b ibill_of_mtrls1) */ comp_style_cd \"NewStyle\", comp_color_cd \"NewColor\", comp_attribute_cd \"NewAttribute\"");
            query.Append(" ,comp_size_cd \"NewSize\",b.activity_cd, nvl(waste_factor,0) \"WasteFactor\", nvl(usage,0) \"Usuage\", nvl(r.location_cd,'??')\"SupplyPlant\"");
            query.Append(", matl_type_cd \"MatTypeCode\", inventory_check_ind, bom_spec_id  \"BomSpecificId\",b.spread_comp_cd \"CompCode\",cutting_alt \"CuttingAlt\",c.spread_type_cd \"SpreadCode\"");
            query.Append(", c.dye_shade_cd \"DyeShadeCode\",c.cylinder_size \"CylSize\" ,c.finished_width \"FinishedWidth\",c.cut_method_cd \"CutMethodCode\",c.conditioned_width \"ConditionedWidth\", c.machine_cut \"MachineCut\"");
            query.Append(",nvl(s.asrmt_cd,'') \"AssortCode\",i.size_short_desc, c.pull_frm_stock_ind \"PullFrmStkInd\"");
            query.Append(" FROM BILL_OF_MTRLS b, activity_routing r,style s, iss_cut_spec_basic_view  c, item_size i WHERE");
            query.Append(" parent_style ='" + Val(style) + "'");
            query.Append(" AND parent_color = '" + Val(color) + "'");
            query.Append(" AND parent_attribute = '" + Val(attribute) + "'");
            query.Append(" and parent_size = '" + Val(size) + "'");
            query.Append(" AND bill_of_mtrls_id ='" + Val(BoMId) + "'");
            query.Append(" and r.routing_id ='" + Val(routingId) + "'");
            query.Append(" and comp_size_cd = i.size_cd and b.activity_cd = r.activity_cd and b.comp_style_cd = s.style_cd and s.matl_type_cd in ('00','01','01','02','CT','F')");
            query.Append(" and b.bom_spec_id  = c.cutting_spec_id  (+) and b.cutting_alt = c.alternate_id   (+) and b.parent_size = c.size_cd(+)");
            query.Append(" and  b.parent_color = c.garment_color (+)  and b.comp_style_cd = c.fabric_style_cd   (+)");
            query.Append(" and b.spread_comp_cd  = c.spread_comp_cd (+)  and substr(b.comp_attribute_cd,4,3) = c.dye_cd (+)");

            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<WorkOrderDetail>(reader);
            return result;
        }

        public List<WorkOrderDetail> GetPackCode()
        {
            var query = new StringBuilder();
            query.Append("select pack_cd \"PackCode\",pack_desc \"PackDescription\" from pack order by pack_cd");
            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<WorkOrderDetail>(reader);
            return result;
        }
        public List<WorkOrderDetail> GetWOStyleDetail(String Style)
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append("SELECT A.CORP_DIVISION_CD \"CorpDivisionCode\",A.MATL_TYPE_CD \"MatTypeCode\", B.CORP_BUSINESS_UNIT \"CorpBusUnit\",A.SIZE_LINE \"SizeLine\",A.MFG_CORP_DIVISION_CD \"CorpDivCode\",A.QUALITY_CD \"QualityCode\",B.COLOR_DIVISION_CD \"ColDivCode\",A.ORIGIN_TYPE_CD \"OriginTypeCode\",A.GTIN_LABEL_CD \"GtinLabcode\", A.INVENTORY_CHECK_IND \"InvChkInd\", A.PACK_CD \"PackCode\",A.UNIT_OF_MEASURE \"UoM\", nvl(s.prod_family_cd,'----') \"ProdFamCode\", nvl(s.product_class_cd,'----') \"ProdClassCode\",A.PRIMARY_DC \"PrimaryDC\", A.STYLE_DESC \"StyleDes\",P.PACKAGE_QTY \"PKGQty\",A.ASRMT_CD \"AssortCode\" FROM STYLE A, STYLE_CHP S, CORP_DIVISION B, PACK P where A.style_cd ='" + Style + "' AND A.CORP_DIVISION_CD  = B.CORP_DIVISION_CD and a.pack_cd = p.pack_cd and a.style_cd = s.style_cd (+)			");

            IDataReader reader = ExecuteReader(queryBuilder.ToString());

            var result = (new DbHelper()).ReadData<WorkOrderDetail>(reader);
            return result;
        }
        public List<WorkOrderDetail> GetCatCode()
        {
            var query = new StringBuilder();
            query.Append("select iss_category_cd \"CategoryCode\",iss_category_desc \"CategoryDescription\" from iss_category order by iss_category_cd");
            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<WorkOrderDetail>(reader);
            return result;
        }

        public decimal GetMachineLimit(WorkOrderDetail Wod)
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append("SELECT oprsql.iss_textiles.compute_machine_limit('" + Val(Wod.iParentStyle) + "','" + Val(Wod.ParentColor) + "','" + Val(Wod.ParentSize) + "','" + Val(Wod.iParentBomId) + "','" + Val(!String.IsNullOrEmpty(Wod.AlternateId) ? Wod.AlternateId.ToUpper() : Wod.AlternateId) + "','" + Val(Wod.MfgPathId) + "','" + Val(Wod.MachineType) + "') from dual");


            // IDataReader reader = ExecuteReader(queryBuilder.ToString());
            var result = (Decimal)ExecuteScalar(queryBuilder.ToString());


            return result;

        }
        public decimal GetStdLoss(WorkOrderDetail Wod)
        {
            Wod.CuttingAlt = !String.IsNullOrEmpty(Wod.CuttingAlt) ? Wod.CuttingAlt.ToUpper() : Wod.CuttingAlt;
            var queryBuilder = new StringBuilder();
            queryBuilder.Append("SELECT oprsql.iss_textiles.std_process_loss('" + Val(Wod.ParentStyle) + "','" + Val(Wod.ParentSize) + "','"
                + Val(Wod.CuttingAlt) + "','" + Val(Wod.SewPlt) + "','" + Val(Wod.PrimeMFGLoc) + "','" + Val(Wod.NewStyle) + "','"
                + Val(Wod.DyeShadeCode) + "','" + Wod.CylSize + "','" + Wod.FinishedWidth + "','" + Wod.ConditionedWidth + "','" + Val(Wod.SupplyPlant) + "','" + Val(Wod.CompCode) + "') from dual");


            // IDataReader reader = ExecuteReader(queryBuilder.ToString());
            var result = (Decimal)ExecuteScalar(queryBuilder.ToString());


            return result;

        }
        //           

        public List<WorkOrderDetail> PopulateMachineTypes(WorkOrderDetail Wod, bool isFromWO)
        {
            if (Wod.iStyle == null)
            {
                var garmntDetails = Wod.WOCumulative.Where(x => x.PipelineCategoryCode == LOVConstants.PipeLineCategoryCode.SEW && x.CumulativeId == Wod.Id && !x.Merged).ToList();
                if (garmntDetails.Count > 0)
                {
                    Wod.iStyle = garmntDetails[0].StyleCd;
                    Wod.iColor = garmntDetails[0].ColorCode;
                    Wod.iAttribute = garmntDetails[0].AttributeCode;
                    Wod.iDesPlt = Wod.iDesPlt;
                    Wod.iMFGPathId = garmntDetails[0].MFGPathId;
                }
                else
                {
                    Wod.iStyle = Wod.SellingStyle;
                    Wod.iColor = Wod.ColorCode;
                    Wod.iAttribute = Wod.Attribute;
                    Wod.iDesPlt = Wod.TxtPath;
                    Wod.iMFGPathId = Wod.MfgPathId;
                }
            }
            
            var query = new StringBuilder();
            Wod.iDesPlt = String.IsNullOrEmpty(Wod.CutPath) ? Wod.iDesPlt : Wod.CutPath;
            Wod.Size = String.IsNullOrEmpty(Wod.Size) ? Wod.iSize : Wod.Size;
            query.Append(" select distinct s.source_plant \"iSourcePlant\" ,s.machine_type_cd \"iMactypeCode\",s.priority \"iPriority\" from iss_garment_resource g,iss_planning_param p,iss_sourcing_priority s where");

            query.Append(" p.mfg_style_cd ='" + Val(Wod.iStyle) + "'");
            query.Append(" and p.color_cd ='" + Val(Wod.iColor) + "'");
            query.Append(" and p.attribute_cd = '" + Val(Wod.iAttribute) + "'");
            query.Append(" and p.dye_shade_cd = '" + Val("T") + "'");
            query.Append("  and p.plant_sku_priority = s.plant_sku_priority");
            query.Append(" and s.destination_plant = '" + Val(Wod.iDesPlt) + "'");
            query.Append(" and s.source_plant = '" + Val(Wod.iDesPlt) + "'");
            query.Append("  and g.style_cd = p.mfg_style_cd and g.color_cd = p.color_cd and g.attribute_cd = p.attribute_cd and g.dye_shade_cd = s.dye_shade_cd");
            query.Append(" and g.mfg_path_id ='" + Val(Wod.iMFGPathId) + "'");
            //query.Append(" and s.source_plant ='" + Val(Wod.iDesPlt) + "'");
            query.Append(" and g.size_cd in( '" + Val(Wod.Size) + "')");
            query.Append("  and g.dye_shade_cd = s.dye_shade_cd order by s.priority");
            System.Diagnostics.Debug.WriteLine(query.ToString());
            IDataReader reader = ExecuteReader(query.ToString());
            
            var result = (new DbHelper()).ReadData<WorkOrderDetail>(reader);
            if (result.Count == 0 && isFromWO)
            {
                result = new List<WorkOrderDetail>
                {
                    new WorkOrderDetail()
                    {
                        iSourcePlant=Wod.SupplyPlant,
                        iMacTypeCode=null,
                        iPriority=1
                    }

                };
            }
            return result;

        }
        
        public List<WorkOrderDetail> GetSpreadXcpns(WorkOrderDetail Wod)
        {

            List<OracleParameter> parameters = new List<OracleParameter>();

            parameters.Add(new OracleParameter()
            {
                ParameterName = "iStyle_cd",
                Value = Wod.PKGStyle
            });
            parameters.Add(new OracleParameter()
            {
                ParameterName = "iColor_cd",
                Value = Wod.ColorCode
            });
            parameters.Add(new OracleParameter()
            {
                ParameterName = "iAttribute_cd",
                Value = Wod.iAttribute
            });
            parameters.Add(new OracleParameter()
            {
                ParameterName = "iSize_cd",
                Value = Wod.iSize

            });
            parameters.Add(new OracleParameter()
            {
                ParameterName = "iPlant_cd",
                Value = Wod.MfgPathId

            });
            parameters.Add(new OracleParameter()
            {
                ParameterName = "iSpread_type_cd",
                Value = Wod.iSpreadType

            });
            IDataReader reader = ExecuteSPReader("OPRSQL.ISS_SCHEDULE_DATA.RS_SPREAD_XCPN", parameters.ToArray());
            var result = (new DbHelper()).ReadData<WorkOrderDetail>(reader);
            return result;
        }
        public List<WorkOrderDetail> SetCombineXcpns(WorkOrderDetail Wod)
        {

            List<OracleParameter> parameters = new List<OracleParameter>();

            parameters.Add(new OracleParameter()
            {
                ParameterName = "iStyle_cd",
                Value = Wod.PKGStyle
            });
            parameters.Add(new OracleParameter()
            {
                ParameterName = "iColor_cd",
                Value = Wod.ColorCode
            });
            parameters.Add(new OracleParameter()
            {
                ParameterName = "iAttribute_cd",
                Value = Wod.iAttribute
            });
            parameters.Add(new OracleParameter()
            {
                ParameterName = "iSize_cd",
                Value = Wod.iSize

            });
            parameters.Add(new OracleParameter()
            {
                ParameterName = "iPlant_cd",
                Value = Wod.MfgPathId

            });
            parameters.Add(new OracleParameter()
            {
                ParameterName = "iCutting_alt",
                Value = Wod.iCuttingAlt

            });
            IDataReader reader = ExecuteSPReader("OPRSQL.ISS_SCHEDULE_DATA.RS_COMBINE_XCPN", parameters.ToArray());
            var result = (new DbHelper()).ReadData<WorkOrderDetail>(reader);
            return result;
        }
        //Update cumulative grid 

        public WorkOrderDetail UpdateCumulative(WorkOrderDetail Wod)
        {
            if (Wod.WOCumulative == null)
                Wod.WOCumulative = new List<WorkOrderCumulative>();
            if (Wod.WOFabric == null)
                Wod.WOFabric = new List<WorkOrderFabric>();
            if (Wod.WOTextiles == null)
                Wod.WOTextiles = new List<WorkOrderTextiles>();
            Wod.LevelInd = LOVConstants.LevelInd;
            ClearCumulativeAndFabric(Wod);
            if (Wod.MfgPathId != null)
            {
                Wod.SeqId = 1;
                Wod.Lbs = 0;
                Wod.VarianceQty = 0;
               
                UpdateSKU(Wod);
                
                    var machineType = PopulateMachineTypes(Wod, true);
                    if (machineType.Count > 0)
                    {
                        if (String.IsNullOrEmpty(Wod.MachineType))

                            Wod.MachineType = machineType[0].iMacTypeCode;

                        if (Wod.LimitLbs == 0)
                            Wod.LimitLbs = GetMachineLimit(Wod);

                        foreach (var item in machineType)
                        {
                            if (String.IsNullOrEmpty(Wod.MachineType))
                                Wod.MachineType = item.iMacTypeCode;
                            Wod.WOTextiles.Add(new WorkOrderTextiles()
                            {
                                MacType = item.iMacTypeCode,
                                TxtPlant = item.iSourcePlant,
                                Limit = Wod.LimitLbs
                            });
                        }
                    }
                   
                    //Wod.CutPath = Wod.MfgPathId;
                    Wod.iSpreadType = " ";
                CalculateVariance(Wod);


            }

            else if (Wod.PKGStyle != string.Empty)
            {
                if (Wod.SizeList != null)
                {
                    Wod.SeqId = 1;
                            for (int i = 0; i < Wod.SizeList.Count; i++)
                            {
                                
                                Wod.WOCumulative.Insert(0, new WorkOrderCumulative()
                                {
                                    CumulativeId = Wod.Id,
                                    StyleCode = Wod.PKGStyle,
                                    ColorDyeCode = Wod.ColorCode,
                                    AttributeCompCode = Wod.Attribute,
                                    Size = Wod.SizeList[i].Size,
                                    Revision = Wod.Revision,
                                    TotalDozens = Wod.SizeList[i].Qty,
                                    Dozens = Wod.SizeList[i].Qty,
                                    IsHide = false,
                                    LevelInd = Wod.LevelInd,
                                    BusinessUnit = Wod.CorpBusUnit,
                                    HiddenSizeDes = Wod.SizeList[i].SizeCD,
                                    MatlTypeCode = Wod.MatTypeCode,
                                    PipelineCategoryCode = LOVConstants.PipeLineCategoryCode.PKG,
                                    SeqId=Wod.SeqId

                                });
                            
                        
                    }
                   
                }
                else
                {
                    
                       
                                Wod.WOCumulative.Add( new WorkOrderCumulative()
                                {
                                    CumulativeId = Wod.Id,
                                    StyleCode = Wod.PKGStyle,
                                    ColorDyeCode = Wod.ColorCode,
                                    AttributeCompCode = Wod.Attribute,
                                    Revision = Wod.Revision,
                                    LevelInd = Wod.LevelInd,
                                    BusinessUnit = Wod.CorpBusUnit,
                                    MatlTypeCode = Wod.MatTypeCode,
                                    PipelineCategoryCode = LOVConstants.PipeLineCategoryCode.PKG

                                });
                                
                           
                    
                }
                    
            }
            else if (Wod.SellingStyle != string.Empty)
            {
                
                Wod.WOCumulative.Insert(0, new WorkOrderCumulative()
                {
                    CumulativeId = Wod.Id,
                    StyleCode = Wod.SellingStyle,
                    ColorDyeCode = Wod.ColorCode,
                    AttributeCompCode = Wod.Attribute,
                    Size = Wod.SizeShortDes,
                    Revision = Wod.Revision,
                    BusinessUnit = Wod.CorpBusUnit,
                    MatlTypeCode = LOVConstants.MATL_TYPE_CD.Garment,
                    PipelineCategoryCode = LOVConstants.PipeLineCategoryCode.PKG
                });
            }

           
            
            
        // GroupCumulativeGrid(Wod);

         if (Wod.WOCumulative != null)
         {
             foreach (var item in Wod.WOCumulative)
             {
                 if (item.PipelineCategoryCode == LOVConstants.PipeLineCategoryCode.PKG)
                 {
                     if (Wod.CuttingAlt != null)
                         item.CuttingAlt = !String.IsNullOrEmpty(Wod.CuttingAlt) ? Wod.CuttingAlt.ToUpper() : Wod.CuttingAlt;
                     else
                         item.CuttingAlt = !String.IsNullOrEmpty(Wod.AlternateId) ? Wod.AlternateId.ToUpper() : Wod.AlternateId;
                     item.PullFromStockIndicator = "N";
                     item.ResourceId = Wod.TempResourceId;
                     item.FabricGroup = Wod.TempFabricGroup;
                 }
                 else if (item.PipelineCategoryCode == LOVConstants.PipeLineCategoryCode.DBF)
                 {
                     item.ResourceId = Wod.TempResourceId;
                     item.FabricGroup = Wod.TempFabricGroup;
                 }
                 else
                 {
                     item.ResourceId = "";
                     item.FabricGroup = "";
                 }
             }

             GroupCumulativeGrid(Wod);
             OrdersToCreate(Wod);
           
         }
            if (Wod.WOFabric != null)
            {
               
               
                SetCylSizes(Wod);
               GroupFabric(Wod);
             
               Wod.WOFabric.Reverse();
            }
          
         //RecomputeDueDates(Wod);
         SetSpreadTypes(Wod);
            Wod.Lbs = Wod.Lbs.RoundCustom(0);
            return Wod;
        }

        public void SetSpreadTypes(WorkOrderDetail Wod)
        {
            var SpreadExcpns = GetSpreadXcpns(Wod);
            SetCombinedRules(Wod);

        }
        public void SetCombinedRules(WorkOrderDetail Wod)
        {

            for (int i = 0; i < Wod.WOCumulative.Count; i++)
            {
                if (Wod.WOCumulative[i].CumulativeId == Wod.Id && Wod.WOCumulative[i].MatlTypeCode == LOVConstants.MATL_TYPE_CD.Fabric)
                {
                    Wod.WOCumulative[i].CuttingAlt = !String.IsNullOrEmpty(Wod.WOCumulative[i].CuttingAlt) ? Wod.WOCumulative[i].CuttingAlt.ToUpper() : Wod.WOCumulative[i].CuttingAlt;
                    Wod.iCuttingAlt = Wod.WOCumulative[i].CuttingAlt;
                    break;
                }
            }
               SetCombineXcpns(Wod);
               for (int j = 0; j < Wod.WOCumulative.Count; j++)
               {
                   if (Wod.WOCumulative[j].CumulativeId == Wod.Id && Wod.WOCumulative[j].MatlTypeCode == LOVConstants.MATL_TYPE_CD.Fabric)
                   {
                       if(Wod.WOCumulative[j].SpreadCompCode!=null)
                       {
                           Wod.WOCumulative[j].CombineInd=LOVConstants.No;
                           Wod.WOCumulative[j].CombineFabInd=LOVConstants.No;
                       }
                       else
                       {
                            Wod.WOCumulative[j].CombineInd=LOVConstants.Yes;
                           Wod.WOCumulative[j].CombineFabInd=LOVConstants.Yes;
                       }
                       Wod.WOCumulative[j].CombineFabSeq= 0;
                       Wod.WOCumulative[j].CombineSeq = 0;
                   }
               }
        }
        public void ClearCumulativeAndFabric(WorkOrderDetail Wod)
        {
            if (Wod.WOCumulative != null)
            {
                if (Wod.WOCumulative.Count > 0)
                {
                    foreach (var item in Wod.WOCumulative.ToList())
                    {
                        if (item.CumulativeId == Wod.Id || item.Merged == true)
                        {
                            Wod.WOCumulative.Remove(item);
                        }
                    }
                }
            }
            if (Wod.WOFabric != null)
            {
                if (Wod.WOFabric.Count > 0)
                {
                    foreach (var item in Wod.WOFabric.ToList())
                    {
                        if (item.Id == Wod.Id)
                        {
                            Wod.WOFabric.Remove(item);
                        }
                    }
                }
            }
        }
       public WorkOrderDetail UpdateSKU(WorkOrderDetail Wod)
        {
            if (!String.IsNullOrWhiteSpace(Wod.PKGStyle))
            {
               
                if (String.IsNullOrWhiteSpace(Wod.ColorCode))
                {
                    var colorCode = GetColorCodes(Wod.SellingStyle);
                    Wod.ColorCode = colorCode[0].ColorCode;
                }
                if (String.IsNullOrWhiteSpace(Wod.Attribute))
                {
                    var attribute = GetAttributeCodes(Wod.SellingStyle, Wod.ColorCode);
                    Wod.Attribute = attribute[0].Attribute;
                    Wod.AttributeDesc = attribute[0].AttributeDesc;
                }
                if (Wod.SizeList!=null)
                {
                    string mfgPathId = Wod.MfgPathId;
                    for (int i = 0; i < Wod.SizeList.Count; i++)
                    {

                        if (Wod.WOCumulative.Count > 0)
                        {
                            if (Wod.WOCumulative[Wod.WOCumulative.Count - 1].SeqId != null)
                            {
                                //Wod.SeqId = Wod.WOCumulative[Wod.WOCumulative.Count - 1].SeqId + 1;
                                int seq = Wod.WOCumulative.Select(x => x.SeqId).ToArray().Max();
                                Wod.SeqId = seq + 1;
                                //PFE
                                Wod.NewColor = Wod.ColorCode;
                            }
                        }
                        else
                        {
                            Wod.SeqId = 1;
                        }
                        Wod.ParentId = 0;

                        var getAsrtCode = GetWOAsrtCode(Wod.PKGStyle);
                        if (getAsrtCode.Count > 0)
                        {
                            Wod.AssortCode = getAsrtCode[0].AssortCode;
                            Wod.OriginTypeCode = getAsrtCode[0].OriginTypeCode;
                            Wod.PKGQty = getAsrtCode[0].PKGQty;
                            Wod.MatTypeCode = getAsrtCode[0].MatTypeCode;
                            Wod.UoM = getAsrtCode[0].UoM;
                            Wod.ParentUoM = getAsrtCode[0].UoM;
                            Wod.ProdFamCode = getAsrtCode[0].ProdFamCode;
                            Wod.CorpBusUnit = getAsrtCode[0].CorpBusUnit;
                            Wod.TempPkgQty = getAsrtCode[0].PKGQty;
                            Wod.PackCode = getAsrtCode[0].PackCode;
                            Wod.TempFabricGroup = getAsrtCode[0].ProdClassCode;
                            Wod.DemandLoc = getAsrtCode[0].PrimaryDC;
                            if (Wod.AssortCode == "A")
                                Wod.PipeLineCat = LOVConstants.PipeLineCategoryCode.PKG;
                            else
                                Wod.PipeLineCat = LOVConstants.PipeLineCategoryCode.SEW;
                        }
                        Wod.LevelInd = LOVConstants.LevelInd;
                        
                        Wod.NewStyle = Wod.PKGStyle;
                        Wod.ParentStyle = Wod.SellingStyle;
                        //Wod.NewColor = Wod.ColorCode; Asif
                        //Wod.NewColor = Wod.ColorCode;
                        //Added by Gopi;For getting the component color in cumulative grid.
                        if (!String.IsNullOrWhiteSpace(Wod.NewColor))
                              Wod.NewColor = Wod.NewColor;
                        else
                            Wod.NewColor = Wod.ColorCode;
                        //End
                        Wod.ParentColor = Wod.ColorCode;
                        Wod.NewAttribute = Wod.Attribute;
                        Wod.NewSize = Wod.SizeList[i].SizeCD;
                        Wod.NewSizeDes = Wod.SizeList[i].Size;
                        Wod.SizeShortDes = Wod.NewSizeDes;
                        Wod.ParentSize = Wod.SizeList[i].SizeCD;
                        Wod.TotalDozens = Wod.SizeList[i].Qty;
                        Wod.Dozens = Wod.SizeList[i].Qty;
                        Wod.DemandQty = Wod.TotalDozens * LOVConstants.Dozen;
                        Wod.CurrOrderQty = Wod.TotalDozens * LOVConstants.Dozen;
                        Wod.CurrOrderTotQty = Wod.TotalDozens * LOVConstants.Dozen;
                        Wod.TempTotalDozens = Wod.SizeList[i].Qty;
                        Wod.TempMFGPathId =mfgPathId;
                        Wod.HiddenSizeDes = Wod.SizeList[i].Size; //PFE 5Nov18
                        Wod.SpreadCode = String.Empty;
                        GetNewRoutingId(Wod);
                        Wod.RoutingId = Wod.TempRoutingId;
                        Wod.BoMId = Wod.TempBoMId;
                        Wod.MfgPathId = Wod.TempMFGPathId;
                        if (Wod.MatTypeCode == LOVConstants.MATL_TYPE_CD.CUT)
                        GetFabricGroup(Wod);
                        GetBDDefault(Wod);
                        if (Wod.MatTypeCode == LOVConstants.MATL_TYPE_CD.CUT)
                        {
                            Wod.IsHide = true;
                        }
                        else
                        {
                            Wod.IsHide = false;
                        }
                        Wod.rIndex = 0;
                        for (int j = 0; j < Wod.WOCumulative.Count; j++)
                        {
                            if (Wod.WOCumulative[j].MatlTypeCode == LOVConstants.MATL_TYPE_CD.Fabric)
                            {
                                Wod.rIndex = j;
                            }
                        }
                        if (Wod.rIndex == 0)
                            Wod.rIndex = Wod.WOCumulative.Count - 1;

                        LoadCutAndTextPath(Wod);
                        AddToCumulative(Wod);
                        Wod.ParentId = Wod.SeqId;
                        LoadOrderFromMaster(Wod);
                        LoadChildsFromGarment(Wod);
                        LoadChildsFromFabric(Wod);
                    }
                }

                
                return Wod;
            }
            else return null;
        }
        public void OrderFabric(WorkOrderDetail Wod)
       {

           var fabric = Wod.WOFabric.OrderBy(x => x.SizeShortDes)
               .ThenByDescending(x => x.Fabric)
               .ThenBy(x => x.DyeCode)
               .ThenByDescending(x=>x.SpreadCode)
               .ThenByDescending(x=>x.CylSize)
               .GroupBy(x=>x.Id).SelectMany(x=>x.OrderBy(y=>y.Id)).Distinct().ToList();
           Wod.WOFabric.Clear();
            foreach(var item in fabric)
            {
               
                Wod.WOFabric.Add(new WorkOrderFabric()
                          {
                              Id=item.Id,
                              Fabric =item.Fabric,
                              DyeCode = item.DyeCode,
                              CompCode = item.CompCode,
                              CylSize = item.CylSize,
                              SpreadCode =item.SpreadCode,
                              SizeShortDes = item.SizeShortDes,
                              ParentStyle = item.ParentStyle,
                              ParentColor = item.ParentColor,
                              ParentAttribute = item.ParentAttribute,
                              ParentBoMId = item.ParentBoMId,
                              ParentMFGPathId = item.ParentMFGPathId,
                              ParentSize = item.ParentSize,
                              ParentSizeDes = item.ParentSizeDes,
                              IsHide=item.IsHide,
                              Merged=item.Merged,
                              Lbs = item.Lbs,
                              PullFromStockInd = item.PullFromStockInd,
                              PullFromStock = item.PullFromStock
                          });
            }

           
       }
        public void AddToFabric(WorkOrderDetail Wod)
        {
           
                Wod.WOFabric.Add(new WorkOrderFabric()
                {
                    Id = Wod.Id,
                    Fabric = Wod.NewStyle,
                    DyeCode = Wod.TempColor,
                    CompCode = Wod.CompCode,
                    CylSize = Wod.CylSize,
                    SpreadCode = Wod.SpreadCode,
                    SizeShortDes = Wod.SizeShortDes,
                    ParentStyle = Wod.TempStyle,
                    ParentColor = Wod.TempColor,
                    ParentAttribute = Wod.TempAttribute,
                    ParentBoMId = Wod.TempBoMId,
                    ParentMFGPathId = Wod.TempMFGPathId,
                    ParentSize = Wod.TempSize,
                    ParentSizeDes = Wod.TempSizeDes,
                    Lbs = Wod.TempDemandDisplay,
                    PullFromStockInd = Wod.PullFrmStkInd,
                    PullFromStock = Wod.PullFromStock,
                    SeqId = Wod.SeqId,
                    ParentId = Wod.ParentId

                });
            
        }

        public void AddToCumulative(WorkOrderDetail Wod)
        {

            if (Wod.MatTypeCode == LOVConstants.MATL_TYPE_CD.Fabric)
                {
                    Wod.TempColor = Wod.NewAttribute.Substring(Wod.NewAttribute.Length - 3, 3);
                    Wod.TempSizeDes = Wod.CylSize.ToString();
                    Wod.TempAttribute = Wod.CompCode;
                    Wod.TempSize = Wod.Size;
                }
                else
                {
                    Wod.TempColor = Wod.NewColor;
                    Wod.TempSizeDes = Wod.SizeShortDes;
                    Wod.TempAttribute = Wod.NewAttribute;
                    Wod.TempSize = Wod.Size;
                }

                Wod.WOCumulative.Insert(Wod.rIndex + 1, new WorkOrderCumulative()
                {
                    CumulativeId = Wod.Id,
                    StyleCode = Wod.NewStyle,
                    ColorDyeCode = Wod.TempColor,
                    AttributeCompCode = Wod.TempAttribute,
                    Size = Wod.TempSizeDes,
                    Revision = Wod.Revision,
                    TotalDozens = Wod.TotalDozens,
                    Dozens = Wod.TotalDozens,
                    LevelInd = Wod.LevelInd,
                    IsHide = Wod.IsHide,
                    HiddenSizeDes = Wod.HiddenSizeDes, // mismatch issue for WO Creation
                    Lbs = Wod.TempCurrentTotal,
                    StyleCd = Wod.NewStyle,
                    ColorCode = Wod.NewColor,
                    AttributeCode = Wod.NewAttribute,
                    SizeCode = Wod.NewSize,
                    DemandLoc = Wod.DemandLoc,
                    ProdFamilyCode = Wod.ProdFamCode,
                    FabricGroup = Wod.TempFabricGroup,
                    MakeOrBuyCode = Wod.MorBCd,
                    MFGPlant = Wod.PrimeMFGLoc,
                    PipelineCategoryCode = Wod.PipeLineCat,
                    DemandQty = Wod.DemandQty.RoundCustom(0),
                    CurrentOrderQty = Wod.CurrOrderQty,
                    CurrentOrderTotalQty = Wod.CurrOrderQty,
                    MatlTypeCode = Wod.MatTypeCode,
                    RoutingId = Wod.RoutingId,
                    BillOfMtrlsId = Wod.BoMId,
                    MFGPathId = Wod.MfgPathId,
                    BomSpecId = Wod.BomSpecificId,
                    CuttingAlt = !String.IsNullOrEmpty(Wod.CuttingAlt) ? Wod.CuttingAlt.ToUpper() : Wod.CuttingAlt,
                    Usuage = Wod.Usuage,
                    StdUsuage = Wod.StdUsuage,
                    StdLoss = Wod.StdLoss,
                    WasteFactor = Wod.WasteFactor,
                    DyeCode = Wod.DyeCode,
                    DyeShadeCode = Wod.DyeShadeCode,
                    MachineTypeCode = Wod.MachineType,
                    CutMethod = Wod.CutMethodCode,
                    PullFromStockIndicator = Wod.PullFrmStkInd,
                    CylinderSize = Wod.CylSize,
                    FinishedWidth = Wod.FinishedWidth,
                    ConditionedWidth = Wod.ConditionedWidth,
                    SpreadCompCode = Wod.CompCode,
                    SpreadTypeCode = Wod.SpreadCode,
                    ScrapFactor = Wod.ScrapFactor,
                    PackCode = Wod.PackCode,
                    CategoryCode = Wod.CategoryCode,
                    UnitOfMeasure = Wod.UoM,
                    ResourceId = Wod.TempResourceId,
                    PlanningLeadTime = Wod.LeadTime,
                    CombineInd = Wod.CombineInd,
                    MFGRevisionNo = Wod.Revision,
                    MachineCut = Wod.MachineCut,
                    AsrmtCode = Wod.AssortCode,
                    CapacityAlloc = Wod.CapacityAlloc,
                    ExpeditePriority = Wod.PriorityCode,
                    CombineFabInd = Wod.CombineFabInd,
                    BusinessUnit = Wod.CorpBusUnit,
                    CutPath = Wod.CutPath,
                    TxtPath = Wod.TxtPath,
                    SeqId = Wod.SeqId,
                    ParentId = Wod.ParentId

                });

        }
        public void GetNewRoutingId(WorkOrderDetail Wod)
        {
            var routingid = GetRoutingId(Wod.NewStyle, Wod.NewColor, Wod.NewAttribute, Wod.NewSize,Wod.TempMFGPathId);
            if (routingid.Count > 0)
            {
                for (int i = 0; i < routingid.Count; i++)
                {
                    if (!String.IsNullOrEmpty(routingid[i].RoutingId))
                        Wod.TempRoutingId = routingid[i].RoutingId.ToString();
                    else
                        Wod.TempRoutingId = String.Empty;
                    if (!String.IsNullOrEmpty(routingid[i].BoMId))
                        Wod.TempBoMId = routingid[i].BoMId.ToString();
                    else
                        Wod.TempBoMId = String.Empty;
                    if (!String.IsNullOrEmpty(routingid[i].MfgPathId))
                        Wod.TempMFGPathId = routingid[i].MfgPathId.ToString();
                    else
                        Wod.TempMFGPathId = String.Empty;
                    if (!String.IsNullOrEmpty(routingid[i].PrimeMFGLoc))
                    {
                        Wod.PrimeMFGLoc = routingid[i].PrimeMFGLoc;
                        if (String.IsNullOrEmpty(Wod.SewPlt))
                            Wod.SewPlt = Wod.PrimeMFGLoc;
                    }
                    else
                    {
                        Wod.PrimeMFGLoc = String.Empty;
                    }
                   
                    if (routingid[i].Revision!=null&&  Wod.LevelInd != LOVConstants.LevelInd)
                    {
                        Wod.Revision = routingid[i].Revision;
                      
                    }
                   
                    if (!String.IsNullOrEmpty(routingid[i].MorBCd))
                        Wod.MorBCd = routingid[i].MorBCd.ToString();
                    else
                        Wod.MorBCd = String.Empty;
                    Wod.ScrapFactor = routingid[i].ScrapFactor;

                    if (!String.IsNullOrEmpty(routingid[i].UoM))
                        Wod.UoM = routingid[i].UoM.ToString();
                    else
                        Wod.UoM = String.Empty;

                    Wod.LeadTime = routingid[i].LeadTime;
                    if (!String.IsNullOrEmpty(routingid[i].CapacityAlloc))
                        Wod.CapacityAlloc = routingid[i].CapacityAlloc;
                    else
                        Wod.CapacityAlloc = string.Empty;

                    Wod.PlannerCd = routingid[i].PlannerCd;
                    
                }
            }
            else
            {

                Wod.TempRoutingId = String.Empty;
                Wod.TempBoMId = String.Empty;
                Wod.MorBCd = "M";
                Wod.ScrapFactor = 0;
                Wod.UoM = LOVConstants.UOM.EA;
                Wod.LeadTime = 0;
                Wod.PlannerCd = "UNK";
            }
            
        }
        public void GetFabricGroup(WorkOrderDetail Wod)
        {
          
            var fabGroup = GetFabricGroup(Wod.NewStyle, Wod.NewColor, Wod.NewAttribute, Wod.NewSize, Wod.MfgPathId);
            if (fabGroup.Count > 0)
            {
                for (int i = 0; i < fabGroup.Count; i++)
                {
                   
                           Wod.TempResourceId=fabGroup[i].ResourceId;
                           Wod.TempFabricGroup = fabGroup[i].FabricGroup;

                       
                }
            }
            else
            {
               
                Wod.TempResourceId="";
                           Wod.TempFabricGroup = "";
               
            }
            
           
          
        }
        public void GetBDDefault(WorkOrderDetail Wod)
        {
            var bdDefault = GetBDDefault(Wod.NewStyle, Wod.NewColor, Wod.NewAttribute, Wod.NewSize, Wod.MfgPathId);
            if (bdDefault.Count > 0)
            {
                for (int i = 0; i < bdDefault.Count; i++)
                {
                    Wod.CreateBDInd=bdDefault[i].CreateBDInd;
                    if(Wod.CreateBDInd==LOVConstants.Yes)
                    {
                        Wod.DozensOnlyInd=LOVConstants.No;
                        Wod.DozensOnly=false;
                        Wod.CreateBd=true;
                    }
                    else
                    {
                        Wod.DozensOnlyInd = LOVConstants.Yes;
                        Wod.DozensOnly = true;
                        Wod.CreateBd = false;
                        Wod.CreateBDInd = LOVConstants.No;
                    }
                }
            }
            
            
        }
        
        public void LoadOrderFromMaster(WorkOrderDetail Wod)
        {
            var newOrder = LoadOrderFromMaster(Wod.NewStyle, Wod.NewColor, Wod.NewAttribute, Wod.NewSize, Wod.RoutingId, Wod.BoMId);
            if(newOrder.Count>0)
            {
               
                if (Wod.LevelInd > 0)
                    Wod.LevelInd = LOVConstants.LevelInd;
               
                Wod.LevelInd = Wod.LevelInd + 1;
                for (int i = 0; i <newOrder.Count; i++)
                {
                    if (Wod.WOCumulative.Count > 0)
                    {
                        if (Wod.WOCumulative[Wod.WOCumulative.Count - 1].SeqId != null)
                        {
                           
                            int seq = Wod.WOCumulative.Select(x => x.SeqId).ToArray().Max();
                            Wod.SeqId = seq + 1;
                        }
                    }

                    Wod.IsHide = false;
                    Wod.ParentStyle = Wod.NewStyle;
                    Wod.iParentStyle = Wod.ParentStyle;
                    Wod.ParentSize = Wod.NewSize;
                        Wod.TempMFGPathId = newOrder[i].SupplyPlant.ToString();
                    Wod.TempStyle = newOrder[i].NewStyle;
                    Wod.TempColor = newOrder[i].NewColor;
                    Wod.TempSize = newOrder[i].NewSize;
                    Wod.TempAttribute = newOrder[i].NewAttribute;
                    Wod.TempSizeDes = newOrder[i].NewSizeDes;//PFE- Issue with Size description in Cumulative grid
                    Wod.TempAltId = !String.IsNullOrEmpty(newOrder[i].AlternateId) ? newOrder[i].AlternateId.ToUpper() : newOrder[i].AlternateId;
                    Wod.NewStyle = newOrder[i].NewStyle;
                    Wod.NewColor = newOrder[i].NewColor;
                    Wod.NewAttribute = newOrder[i].NewAttribute;
                    Wod.NewSize = newOrder[i].NewSize;
                    Wod.SizeShortDes = newOrder[i].NewSizeDes; // PFE -Issue with Size description in Cumulative grid
                    Wod.ParentStyle = Wod.NewStyle;
                    Wod.ParentSize = Wod.NewSize;
                    Wod.TotalDozens = Wod.TempTotalDozens;
                    Wod.PKGQty = Wod.TempPkgQty;
                    Wod.SpreadCode = newOrder[i].SpreadCode;
                    Wod.MatTypeCode = newOrder[i].MatTypeCode;
                    Wod.DemandLoc = newOrder[i].SupplyPlant.ToString();
                    Wod.CuttingAlt = !String.IsNullOrEmpty(newOrder[i].CuttingAlt) ? newOrder[i].CuttingAlt.ToUpper() : newOrder[i].CuttingAlt;
                    Wod.FinishedWidth = newOrder[i].FinishedWidth;
                    Wod.ConditionedWidth = newOrder[i].ConditionedWidth;
                    Wod.WasteFactor = newOrder[i].WasteFactor;
                    Wod.SpreadCode = newOrder[i].SpreadCode;//SpreadTypeCode 
                   
                    var getAsrtCode = GetWOAsrtCode(newOrder[i].NewStyle);
                    if (getAsrtCode.Count > 0)
                    {
                        Wod.AssortCode = getAsrtCode[0].AssortCode;
                        Wod.OriginTypeCode = getAsrtCode[0].OriginTypeCode;
                        Wod.MatTypeCode = getAsrtCode[0].MatTypeCode;
                        Wod.UoM = getAsrtCode[0].UoM;
                    }
                    if (Wod.PKGQty > 0)
                        Wod.Usuage = newOrder[i].Usuage / Wod.PKGQty;
                    else
                        Wod.Usuage = 0;

                    if (Wod.WasteFactor != 1)
                        Wod.StdUsuage = newOrder[i].Usuage / (1 - Wod.WasteFactor);
                    else
                        Wod.StdUsuage = 0;
                    Wod.CompCode = newOrder[i].CompCode;
                    Wod.StdLoss = 0;
                    Wod.PullFrmStkInd = LOVConstants.No;
                    Wod.CutMethodCode = newOrder[i].CutMethodCode;
                   
                    //*****
                    Wod.MachineCut = newOrder[i].MachineCut;
                    Wod.BomSpecificId = newOrder[i].BomSpecificId;
                    if (newOrder[i].NewAttribute.Length > 4)
                        Wod.DyeCode = newOrder[i].NewAttribute.Substring(newOrder[i].NewAttribute.Length - 3);
                    if (Wod.BoMId != String.Empty)
                    {
                        LoadPath(Wod);
                   }
                    Wod.ParentBoM = Wod.TempBoMId;
                    Wod.BoMId = Wod.TempBoMId;
                    Wod.RoutingId = Wod.TempRoutingId;
                    CalculateCurrentOrderQuantity(Wod);
                    Wod.TotalDozens = Wod.DemandQty / 12;
                    if (newOrder[i].MatTypeCode == LOVConstants.MATL_TYPE_CD.Fabric)
                    {
                        Wod.PipeLineCat = LOVConstants.PipeLineCategoryCode.DBF;
                        if (Wod.PullFrmStkInd == LOVConstants.Yes)
                            Wod.PullFromStock = true;
                        else
                            Wod.PullFromStock = false;
                        Wod.rIndex = 0;
                        
                        for (int j = 0; j < Wod.WOCumulative.Count; j++)
                        {
                            if (Wod.WOCumulative[j].MatlTypeCode == LOVConstants.MATL_TYPE_CD.Fabric)
                            {
                                Wod.rIndex = j;
                            }
                        }
                        if (Wod.rIndex == 0)
                            Wod.rIndex = Wod.WOCumulative.Count - 1;
                        LoadCutAndTextPath(Wod);
                        AddToCumulative(Wod);

                        AddToFabric(Wod);
                      
                    }
                    else if (newOrder[i].MatTypeCode == LOVConstants.MATL_TYPE_CD.Garment)
                    {
                        Wod.PipeLineCat = LOVConstants.PipeLineCategoryCode.SEW;
                        Wod.iStyle = Wod.TempStyle;
                        Wod.iColor = Wod.TempColor;
                        Wod.iAttribute = Wod.TempAttribute;
                        Wod.iSize = Wod.TempSize;
                        Wod.iMFGPathId = Wod.TempMFGPathId;
                        
                        Wod.rIndex = 0;
                        for (int j = 0; j < Wod.WOCumulative.Count; j++)
                        {
                            if (Wod.WOCumulative[j].MatlTypeCode == LOVConstants.MATL_TYPE_CD.Garment)
                            {
                                Wod.rIndex = j;
                            }
                        }
                        if (Wod.rIndex == 0)
                            Wod.rIndex = Wod.WOCumulative.Count - 1;
                        Wod.TempDemandDisplay = 0;
                        AddToCumulative(Wod);
                      
                    }
                    else if (newOrder[i].MatTypeCode == LOVConstants.MATL_TYPE_CD.CUT)
                    {
                        Wod.PipeLineCat = LOVConstants.PipeLineCategoryCode.CUT;
                        SetAlternateId(Wod);
                        Wod.IsHide = true;
                        LoadCutAndTextPath(Wod);
                        AddToCumulative(Wod);
                      
                    }
                }
             
            }
            
        }

        public void LoadChildsFromGarment(WorkOrderDetail Wod)
        {
            if(Wod.WOCumulative.Count>0)
            {
              
                for (int i = 0; i < Wod.WOCumulative.Count;i++ )
                {
                    if (Wod.WOCumulative[i].CumulativeId == Wod.Id && Wod.WOCumulative[i].StyleCode != Wod.PKGStyle )
                    {

                        if (Wod.WOCumulative[i].MatlTypeCode == LOVConstants.MATL_TYPE_CD.Garment && !Wod.WOCumulative[i].IsProcessed)
                        {
                            Wod.WOCumulative[i].IsProcessed = true;
                            Wod.TempStyle = Wod.WOCumulative[i].StyleCode;
                            Wod.TempColor = Wod.WOCumulative[i].ColorCode;
                            Wod.TempAttribute = Wod.WOCumulative[i].AttributeCode;
                            Wod.TempSize = Wod.WOCumulative[i].SizeCode;
                            Wod.TempRoutingId = Wod.WOCumulative[i].RoutingId;
                            Wod.TempBoMId = Wod.WOCumulative[i].BillOfMtrlsId;
                            Wod.TempTotalDozens = Wod.WOCumulative[i].TotalDozens;
                            Wod.SizeShortDes = Wod.WOCumulative[i].Size;
                            Wod.LevelInd = Wod.WOCumulative[i].LevelInd + 1;
                            Wod.rIndex = i;
                            Wod.ParentId = Wod.WOCumulative[i].SeqId;
                            LoadChildOrder(Wod);
                            LoadChildsFromCut(Wod);
                        }
                       else if (Wod.WOCumulative[i].MatlTypeCode == LOVConstants.MATL_TYPE_CD.CUT && !Wod.WOCumulative[i].IsProcessed)
                       {
                           LoadChildsFromCut(Wod);
                       } 

                    }
                }
            }
        }
       
        public void LoadChildsFromCut(WorkOrderDetail Wod)
        {
            if(Wod.WOCumulative.Count>0)
            {
               
                for (int i = 0; i < Wod.WOCumulative.Count;i++ )
                {
                    if (Wod.WOCumulative[i].CumulativeId == Wod.Id && Wod.WOCumulative[i].StyleCode != Wod.PKGStyle )
                    {

                        if (Wod.WOCumulative[i].MatlTypeCode == LOVConstants.MATL_TYPE_CD.CUT && !Wod.WOCumulative[i].IsProcessed)
                           
                        {
                            Wod.WOCumulative[i].IsProcessed = true;
                            Wod.TempTotalDozens = Wod.WOCumulative[i].TotalDozens;
                            Wod.NewStyle = Wod.WOCumulative[i].StyleCode;
                            Wod.NewColor = Wod.WOCumulative[i].ColorCode;
                            Wod.NewAttribute = Wod.WOCumulative[i].AttributeCode;
                            Wod.NewSize = Wod.WOCumulative[i].SizeCode;
                            Wod.TempBoMId = Wod.WOCumulative[i].BillOfMtrlsId;
                            Wod.TempRoutingId = Wod.WOCumulative[i].RoutingId;
                            Wod.SizeShortDes = Wod.WOCumulative[i].Size;
                            Wod.LevelInd = Wod.WOCumulative[i].LevelInd + 1;
                            Wod.ParentId = Wod.WOCumulative[i].SeqId;

                            Wod.rIndex = i;
                            if (Wod.TempBoMId != String.Empty && Wod.TempRoutingId != String.Empty)
                           LoadFabricDetails(Wod);
                        }
                       
                    }
                }
            }
        }
        public void LoadChildsFromFabric(WorkOrderDetail Wod)
        {
            if (Wod.WOCumulative.Count > 0)
            {
              
                for (int i = 0; i < Wod.WOCumulative.Count; i++)
                {
                    if (Wod.WOCumulative[i].CumulativeId == Wod.Id && Wod.WOCumulative[i].StyleCode != Wod.PKGStyle)
                    {

                        if (Wod.WOCumulative[i].MatlTypeCode == LOVConstants.MATL_TYPE_CD.Fabric && !Wod.WOCumulative[i].IsProcessed)
                           
                        {
                            Wod.WOCumulative[i].IsProcessed = true;
                            Wod.TempTotalDozens = Wod.WOCumulative[i].CurrentOrderQty;
                            Wod.TempStyle = Wod.WOCumulative[i].StyleCode;
                            Wod.TempColor = Wod.WOCumulative[i].ColorCode;
                            Wod.TempAttribute = Wod.WOCumulative[i].AttributeCode;
                            Wod.TempSize = Wod.WOCumulative[i].SizeCode;
                            Wod.TempBoMId = Wod.WOCumulative[i].BillOfMtrlsId;
                            string test = Wod.WOCumulative[i].BomSpecId;
                            Wod.TempRoutingId = Wod.WOCumulative[i].RoutingId;
                            Wod.LevelInd = Wod.WOCumulative[i].LevelInd + 1;
                            Wod.ParentId = Wod.WOCumulative[i].SeqId;
                            if (Wod.TempBoMId != String.Empty && Wod.TempRoutingId != String.Empty)
                                LoadChildOrder(Wod);
                          
                        }

                    }
                }
            }
        }
        public void LoadChildOrder(WorkOrderDetail Wod)
        {
            var newChildOrder = LoadOrderFromMaster(Wod.TempStyle, Wod.TempColor, Wod.TempAttribute, Wod.TempSize, Wod.TempRoutingId, Wod.TempBoMId);
            if(newChildOrder.Count>0)
            {
               
                for(int i=0;i<newChildOrder.Count;i++)
                {
                    if (Wod.WOCumulative.Count > 0)
                    {
                        if (Wod.WOCumulative[Wod.WOCumulative.Count - 1].SeqId != null)
                        {
                            int seq = Wod.WOCumulative.Select(x => x.SeqId).ToArray().Max();
                            Wod.SeqId = seq + 1;
                        }
                    }
                    Wod.TotalDozens = Wod.TempTotalDozens;
                    if (newChildOrder[i].MatTypeCode == LOVConstants.MATL_TYPE_CD.CUT)
                        //Wod.TempMFGPathId = newChildOrder[i].SupplyPlant.ToString();
                        Wod.TempMFGPathId = String.IsNullOrEmpty(Wod.CutPath) ? newChildOrder[i].SupplyPlant.ToString() : Wod.CutPath;
                    if(newChildOrder[i].MatTypeCode==LOVConstants.MATL_TYPE_CD.Fabric && !String.IsNullOrEmpty(Wod.TxtPath))
                        Wod.TempMFGPathId = Wod.TxtPath;
                    Wod.TempStyle = newChildOrder[i].NewStyle;
                    Wod.TempColor = newChildOrder[i].NewColor;
                    Wod.TempSize = newChildOrder[i].NewSize;
                    Wod.TempAttribute = newChildOrder[i].NewAttribute;
                    Wod.TempAltId = !String.IsNullOrEmpty(newChildOrder[i].AlternateId) ? newChildOrder[i].AlternateId.ToUpper() : newChildOrder[i].AlternateId;
                    Wod.SupplyPlant = newChildOrder[i].SupplyPlant.ToString();
                    Wod.NewStyle = newChildOrder[i].NewStyle;
                    Wod.NewColor = newChildOrder[i].NewColor;
                    Wod.NewAttribute = newChildOrder[i].NewAttribute;
                    Wod.NewSize = newChildOrder[i].NewSize;
                    Wod.DemandLoc = newChildOrder[i].SupplyPlant.ToString();
                    Wod.CuttingAlt = !String.IsNullOrEmpty(newChildOrder[i].CuttingAlt) ? newChildOrder[i].CuttingAlt.ToUpper() : newChildOrder[i].CuttingAlt;
                    Wod.FinishedWidth = newChildOrder[i].FinishedWidth;
                    Wod.ConditionedWidth = newChildOrder[i].ConditionedWidth;
                    Wod.CompCode = newChildOrder[i].CompCode;
                    Wod.CylSize=newChildOrder[i].CylSize;
                    Wod.IsHide = false;
                    Wod.SpreadCode = newChildOrder[i].SpreadCode;//SpreadTypeCode
                    var getAsrtCode = GetWOAsrtCode(newChildOrder[i].NewStyle);
                    if (getAsrtCode.Count > 0)
                    {
                        Wod.AssortCode = getAsrtCode[0].AssortCode;
                        Wod.OriginTypeCode = getAsrtCode[0].OriginTypeCode;
                        Wod.PKGQty = getAsrtCode[0].PKGQty;
                        Wod.MatTypeCode = getAsrtCode[0].MatTypeCode;
                        Wod.UoM = getAsrtCode[0].UoM;
                       
                    }
                    if (Wod.PKGQty > 0)
                        Wod.Usuage = newChildOrder[i].Usuage / Wod.PKGQty;
                    else
                        Wod.Usuage = 0;
                    Wod.WasteFactor = newChildOrder[i].WasteFactor;
                    Wod.PullFrmStkInd = LOVConstants.No;
                    if (Wod.WasteFactor != 1)
                        Wod.StdUsuage = newChildOrder[i].Usuage / (1 - Wod.WasteFactor);
                    else
                        Wod.StdUsuage = 0;
                    GetNewRoutingId(Wod);
                    Wod.BoMId = Wod.TempBoMId;
                    Wod.StdLoss = GetStdLoss(Wod);
                    Wod.RoutingId = Wod.TempRoutingId;
                    CalculateCurrentOrderQuantity(Wod);
                    Wod.TotalDozens = Wod.DemandQty / 12;
                    LoadCutAndTextPath(Wod);
                    //*****
                    Wod.MachineCut = newChildOrder[i].MachineCut;
                    Wod.CutMethodCode = newChildOrder[i].CutMethodCode;
                    Wod.DyeShadeCode = newChildOrder[i].DyeShadeCode;
                    Wod.BomSpecificId = newChildOrder[i].BomSpecificId;
                    if (newChildOrder[i].NewAttribute.Length > 4)
                        Wod.DyeCode = newChildOrder[i].NewAttribute.Substring(newChildOrder[i].NewAttribute.Length - 3);

                    if (newChildOrder[i].MatTypeCode == LOVConstants.MATL_TYPE_CD.CUT)//CutPart
                    {
                        Wod.IsHide = true;
                        Wod.PipeLineCat = LOVConstants.PipeLineCategoryCode.CUT;
                       
                        SetAlternateId(Wod);
                      Wod.rIndex = Wod.WOCumulative.Count - 1;
                        Wod.TempDemandDisplay = 0;
                        AddToCumulative(Wod);
                       
                    }
                     else if (newChildOrder[i].MatTypeCode == LOVConstants.MATL_TYPE_CD.Garment)
                     {
                         Wod.PipeLineCat = LOVConstants.PipeLineCategoryCode.SEW;
                        
                         Wod.rIndex = Wod.WOCumulative.Count - 1;
                         Wod.TempDemandDisplay = 0;
                         AddToCumulative(Wod);
                       
                     }
                     else if (newChildOrder[i].MatTypeCode == LOVConstants.MATL_TYPE_CD.Fabric)// && Wod.BoMId != String.Empty)
            
                    {
                       
                        Wod.PipeLineCat = LOVConstants.PipeLineCategoryCode.DBF;
                        if (Wod.PullFrmStkInd == LOVConstants.Yes)
                            Wod.PullFromStock = true;
                        else
                            Wod.PullFromStock = false;
                     
                       Wod.rIndex = Wod.WOCumulative.Count - 1;
                        Wod.TotalDozens = 0;
                       
                            AddToCumulative(Wod);
                            AddToFabric(Wod);
                        
                     
                    }
                   
                }
              
            }

           
        }

       
        public void LoadFabricDetails(WorkOrderDetail Wod)
        {
            var fabDetails = LoadFabricGrid(Wod.NewStyle, Wod.NewColor, Wod.NewAttribute, Wod.NewSize, Wod.TempBoMId, Wod.TempRoutingId);
           
            if (fabDetails.Count > 0)
            {
                foreach (var item in fabDetails)
                {
                    if (Wod.WOCumulative.Count > 0)
                    {
                        if (Wod.WOCumulative[Wod.WOCumulative.Count - 1].SeqId != null)
                        {
                            int seq = Wod.WOCumulative.Select(x => x.SeqId).ToArray().Max();
                            Wod.SeqId = seq + 1;
                        }
                    }
                    Wod.TotalDozens = Wod.TempTotalDozens;
                    Wod.IsHide = false;
                    if (item.MatTypeCode == LOVConstants.MATL_TYPE_CD.Fabric && !String.IsNullOrEmpty(Wod.TxtPath))
                        Wod.TempMFGPathId = Wod.TxtPath;
                    Wod.iDesPlt = Wod.MfgPathId;
                    Wod.NewStyle = item.NewStyle;
                    Wod.NewColor = item.NewColor; 
                    Wod.NewAttribute = item.NewAttribute;
                    Wod.NewSize = item.NewSize;
                    Wod.PipeLineCat = LOVConstants.PipeLineCategoryCode.DBF;
                    Wod.DemandLoc = item.SupplyPlant.ToString();
                    Wod.CuttingAlt = !String.IsNullOrEmpty(item.CuttingAlt) ? item.CuttingAlt.ToUpper() : item.CuttingAlt;
                    Wod.CutMethodCode = item.CutMethodCode;
                    Wod.SpreadCode = item.SpreadCode;//SpreadTypeCode
                            Wod.TempColor = Wod.NewAttribute.Substring(Wod.NewAttribute.Length - 3, 3);
                            var getAsrtCode = GetWOAsrtCode(item.NewStyle);
                            if (getAsrtCode.Count > 0)
                            {
                                Wod.AssortCode = getAsrtCode[0].AssortCode;
                                Wod.OriginTypeCode = getAsrtCode[0].OriginTypeCode;
                                Wod.PKGQty = getAsrtCode[0].PKGQty;
                                Wod.MatTypeCode = getAsrtCode[0].MatTypeCode;
                                Wod.UoM = getAsrtCode[0].UoM;
                            }

                            Wod.Usuage = item.Usuage / Wod.PKGQty;
                            Wod.WasteFactor = item.WasteFactor;
                            Wod.StdUsuage = item.Usuage / (1 - Wod.WasteFactor);
                            Wod.FinishedWidth = item.FinishedWidth;
                            Wod.DyeShadeCode = item.DyeShadeCode;
                            Wod.CuttingAlt = !String.IsNullOrEmpty(item.CuttingAlt) ? item.CuttingAlt.ToUpper() : item.CuttingAlt;
                            Wod.ConditionedWidth = item.ConditionedWidth;
                            Wod.CompCode = item.CompCode;
                            Wod.CylSize = item.CylSize;
                            
                            GetNewRoutingId(Wod);
                            Wod.BoMId = Wod.TempBoMId;
                    //88888
                            Wod.MfgPathId = Wod.TempMFGPathId;
                    //*****
                            Wod.MachineCut = item.MachineCut;
                            Wod.BomSpecificId = item.BomSpecificId;
                            if (item.NewAttribute.Length > 4)
                                Wod.DyeCode = item.NewAttribute.Substring(item.NewAttribute.Length - 3);
                    ///888888
                            Wod.StdLoss = GetStdLoss(Wod);
                            Wod.RoutingId = Wod.TempRoutingId;
                            Wod.SpreadCode = item.SpreadCode;
                            if (item.PullFrmStkInd != null)
                                Wod.PullFrmStkInd = item.PullFrmStkInd;
                            else
                                Wod.PullFrmStkInd = LOVConstants.No;
                           
                            CalculateCurrentOrderQuantity(Wod);
                            if (Wod.PullFrmStkInd == LOVConstants.Yes)
                                Wod.PullFromStock = true;
                            else
                                Wod.PullFromStock = false;
                            Wod.TotalDozens = 0;
                            LoadCutAndTextPath(Wod);
                                AddToCumulative(Wod);
                                AddToFabric(Wod);
                                Wod.rIndex = Wod.rIndex + 1;
                
                        }
                }
         
        }
               
     public void SetCylSizes(WorkOrderDetail Wod)
        {
         if(Wod.WOFabric!=null && Wod.WOFabric.Count>0)
         {
             Wod.CylinderSizes = String.Empty;
                 for (int i = Wod.WOFabric.Count - 1; i >= 0; i--)
                 {
                     if (Wod.WOFabric[i].Id == Wod.Id)
                     {
                         if (Wod.SetCyl == null)
                         {
                             Wod.SetCyl = Wod.WOFabric[i].SizeShortDes;
                         }
                        
                         string s1 = Wod.WOFabric[i].CylSize.ToString();
                         if (Wod.SetCyl == Wod.WOFabric[i].SizeShortDes)
                         {
                             if (Wod.CylinderSizes == null && s1.Length > 0)
                             {
                                 Wod.CylinderSizes = s1 + ",";

                             }
                             else if (!Wod.CylinderSizes.Contains(s1))
                             {
                                
                                 Wod.CylinderSizes = s1 + "," + Wod.CylinderSizes; 

                             }
                         }
                     }
                 }
                 if (Wod.CylinderSizes != null && Wod.CylinderSizes.Length > 0)
                     Wod.CylinderSizes = Wod.CylinderSizes.Remove(Wod.CylinderSizes.Length - 1, 1);
         }
        
        }

            public void LoadPath(WorkOrderDetail Wod)
        {
            GetNewRoutingId(Wod);
            GetFabricGroup(Wod);
           
            
        }
        public void LoadCutAndTextPath(WorkOrderDetail Wod)
            {
                if (Wod.MatTypeCode == LOVConstants.MATL_TYPE_CD.Fabric)
                {
                    if (Wod.TxtPath != null && Wod.TxtPath != String.Empty)
                    {
                        Wod.TempMFGPathId = Wod.TxtPath;
                        Wod.DemandLoc = Wod.TxtPath;
                        Wod.MFGPlant = Wod.TxtPath;
                    }
                    else
                    {
                        Wod.TxtPath = Wod.TempMFGPathId;
                    }
                }
                else if (Wod.MatTypeCode == LOVConstants.MATL_TYPE_CD.CUT)
                {
                    if (Wod.CutPath != null && Wod.CutPath != String.Empty)
                    {
                        Wod.TempMFGPathId = Wod.CutPath;
                        Wod.DemandLoc = Wod.CutPath;
                        Wod.MFGPlant = Wod.CutPath;
                    }
                    else
                    {
                        Wod.CutPath = Wod.TempMFGPathId;
                    }
                }
            }
      

        public void CalculateCurrentOrderQuantity(WorkOrderDetail Wod)
        {
            decimal totalDozens = Wod.TotalDozens;
            decimal eaches = totalDozens * LOVConstants.Dozen;
            decimal scrap = Wod.ScrapFactor;
            decimal PkgQty = Wod.PKGQty;
            decimal usuage = Wod.Usuage;
            decimal waste = Wod.WasteFactor;
            decimal stdLoss = Wod.StdLoss;
            decimal stdUsuage = Wod.StdUsuage;
            decimal orderCount = Wod.OrderCount;
            decimal unitOfMeasure = LOVConstants.Dozen;
            decimal qty = 0, finqty = 0;
            decimal qty1 = 0, currUnitOfMeasure = 0;
            string style = Wod.NewStyle;

            Wod.DiscreteInd = LOVConstants.DiscreteInd;
            if (Wod.DiscreteInd == LOVConstants.Yes)
                scrap = 0;

            qty = eaches;

            if (scrap <= 1)
            {

                qty = qty - (qty * scrap);
            }

            if (Wod.UoM == LOVConstants.UOM.DZ)
                currUnitOfMeasure = LOVConstants.Dozen;
            else
                currUnitOfMeasure = 1;
            usuage = usuage * currUnitOfMeasure / unitOfMeasure;
            qty = qty * usuage;
            if (waste > 0)
                qty = qty / (1 - waste);

            if (scrap > 0)
                qty = qty / (1 - scrap);


            qty1 = qty;
            Wod.DemandQty = qty1;
            if (Wod.MatTypeCode == LOVConstants.MATL_TYPE_CD.Fabric)
            {

                if (Wod.PullFrmStkInd == LOVConstants.No && Wod.DozensOnlyInd == LOVConstants.No)
                {
                    qty = (qty1 / (1 - stdLoss));

                    finqty = qty1;
                    qty1 = qty;
                    Wod.Lbs = Wod.Lbs + qty1;
                    if (Wod.VarianceQty == 0)
                        Wod.VarianceQty = qty1;
                    else
                        Wod.VarianceQty = Wod.VarianceQty + qty1;
                    Wod.ActualLbs = Wod.VarianceQty;
                    Wod.TempDemandDisplay = qty1.RoundCustom(2);
                }
                else if (Wod.PullFrmStkInd == LOVConstants.No && Wod.DozensOnlyInd == LOVConstants.Yes)
                {
                    qty = (qty1 / (1 - stdLoss));

                    finqty = qty1;
                    qty1 = qty;
                    Wod.TempDemandDisplay = qty1.RoundCustom(2);
                }
                else if (Wod.PullFrmStkInd == LOVConstants.Yes && Wod.DozensOnlyInd == LOVConstants.No)
                {
                    qty = (qty1 / (1 - stdLoss));

                    finqty = 0;
                    Wod.TempDemandDisplay = qty.RoundCustom(2);
                }
                else
                {

                    finqty = 0;
                }

            }

            
            if (finqty > 0)
                Wod.CurrOrderQty = finqty;
            else
                Wod.CurrOrderQty = Wod.DemandQty;
            Wod.TempTotal = Wod.CurrOrderTotQty;
            Wod.CurrOrderTotQty = finqty;

            DisplayQuantity(Wod);
        }

        public void DisplayQuantity(WorkOrderDetail Wod)
        {
            decimal demand = Wod.DemandQty;
            decimal currOrdertoQty = Wod.CurrOrderTotQty;
            Wod.TempCurrentTotal = Math.Round(currOrdertoQty);
            Wod.CurrOrderTotQty = Wod.TempTotal;

        }


        public WorkOrderDetail CalculateVariance(WorkOrderDetail Wod)
       {
           decimal newVariance = Wod.LimitLbs -Wod.VarianceQty;
           Wod.Variance = newVariance;
           Wod.Variance = newVariance.RoundCustom(3);

           return Wod;
       }
        public WorkOrderDetail OrdersToCreate(WorkOrderDetail Wod)
       {
          if(Wod.WOCumulative!=null && Wod.WOCumulative.Count>0)
          {
              for (int i=0;i<Wod.WOCumulative.Count;i++)
              {
                  if (Wod.WOCumulative[i].MatlTypeCode == LOVConstants.MATL_TYPE_CD.Garment || Wod.WOCumulative[i].MatlTypeCode == LOVConstants.MATL_TYPE_CD.CUT)
                  {
                      if(Wod.WOCumulative[i].OrgTotalDozens==0)
                      {
                          Wod.WOCumulative[i].OrgTotalDozens = Wod.WOCumulative[i].TotalDozens;
                      }
                      Wod.WOCumulative[i].TotalDozens = Wod.WOCumulative[i].OrgTotalDozens * Wod.OrderCount;
                      Wod.WOCumulative[i].Dozens = Wod.WOCumulative[i].OrgTotalDozens * Wod.OrderCount;
                  }
                  else if (Wod.WOCumulative[i].MatlTypeCode==LOVConstants.MATL_TYPE_CD.Fabric)
                  {
                      if (Wod.WOCumulative[i].OrgLbs == 0)
                      {
                          Wod.WOCumulative[i].OrgLbs = Wod.WOCumulative[i].Lbs;
                      }
                      Wod.WOCumulative[i].Lbs = Wod.WOCumulative[i].OrgLbs * Wod.OrderCount;
                  }
              }
              Wod.WOCumulative.Reverse();
          }

           return Wod;
       }
       public WorkOrderDetail UpdateLBS (WorkOrderDetail Wod)
        {
            if (Wod.WOCumulative != null)
            {
            decimal Id = Wod.Id;
            decimal tDZ = Wod.TotalDozens;
            string Style = Wod.SellingStyle;
            decimal qty = 0, newqty = 0;
            Wod.Lbs = 0;
           
                for (int i = 0; i < Wod.WOCumulative.Count; i++)
                {
                    if (Wod.WOCumulative[i].CumulativeId == Wod.Id && Wod.WOCumulative[i].StyleCode == Wod.PKGStyle && Wod.WOCumulative[i].LevelInd == 0)
                    {



                        Wod.WOCumulative[i].Lbs = Wod.TempCurrentTotal;
                        Wod.WOCumulative[i].DemandQty = Wod.DemandQty.RoundCustom(0);
                        Wod.WOCumulative[i].CurrentOrderQty = Wod.TotalDozens*LOVConstants.Dozen;
                        Wod.WOCumulative[i].CurrentOrderTotalQty = Wod.TotalDozens * LOVConstants.Dozen;
                        Wod.WOCumulative[i].TotalDozens = Wod.TotalDozens;
                        Wod.WOCumulative[i].Dozens = Wod.TotalDozens;

                    }

                }

                UpdateQuantity(Wod);
                RecalcLBS(Wod);
                GroupCumulativeGrid(Wod);
                GroupFabric(Wod);
            }
            return Wod;
        }
        public void UpdateQuantity(WorkOrderDetail Wod)
       {
           for (int i = 0; i < Wod.WOCumulative.Count; i++)
           {
               if (Wod.WOCumulative[i].CumulativeId == Wod.Id && Wod.WOCumulative[i].LevelInd != 0 && Wod.WOCumulative[i].Merged==false)
               {
                  
                   
                   Wod.ScrapFactor=Wod.WOCumulative[i].ScrapFactor;
                   Wod.PKGQty=Wod.WOCumulative[i].PackQty;
                   Wod.Usuage=Wod.WOCumulative[i].Usuage;
                   Wod.WasteFactor=Wod.WOCumulative[i].WasteFactor;
                   Wod.StdLoss=Wod.WOCumulative[i].StdLoss;
                   Wod.StdUsuage=Wod.WOCumulative[i].StdUsuage;
                   Wod.UoM = Wod.WOCumulative[i].UnitOfMeasure;
                   Wod.PullFrmStkInd = Wod.WOCumulative[i].PullFromStockIndicator;
                   Wod.MatTypeCode = Wod.WOCumulative[i].MatlTypeCode;
                  Wod.TotalDozens = Wod.WOCumulative.Where(x =>
                       x.CumulativeId == Wod.Id
                       && x.SeqId == Wod.WOCumulative[i].ParentId
                      ).Select(y=>y.TotalDozens).First();
                   
                   CalculateCurrentOrderQuantity(Wod);

                   Wod.TotalDozens = Wod.DemandQty / 12;
                   Wod.WOCumulative[i].Lbs = Wod.TempCurrentTotal;
                   Wod.WOCumulative[i].DemandQty = Wod.DemandQty.RoundCustom(0);
                   Wod.WOCumulative[i].CurrentOrderQty = Wod.CurrOrderQty;
                   Wod.WOCumulative[i].CurrentOrderTotalQty = Wod.CurrOrderQty;
                   if( Wod.WOCumulative[i].MatlTypeCode!=LOVConstants.MaterialTypeCode.Fabrics)
                   {
                       Wod.WOCumulative[i].TotalDozens = Wod.TotalDozens;
                       Wod.WOCumulative[i].Dozens = Wod.TotalDozens;
                   }
                   else
                   {
                       for(int j=0;j< Wod.WOFabric.Count;j++)
                       {
                           if (Wod.WOCumulative[i].CumulativeId == Wod.WOFabric[j].Id && Wod.WOFabric[j].Merged == false && Wod.WOCumulative[i].SeqId == Wod.WOFabric[j].SeqId && Wod.WOCumulative[i].ParentId == Wod.WOFabric[j].ParentId)
                           {
                               Wod.WOFabric[j].Lbs = Wod.TempDemandDisplay;
                           }
                       }
                   }
                   
               }

           }

       }
        public void RecalcLBS(WorkOrderDetail Wod)
       {

           Wod.Lbs = Wod.Lbs.RoundCustom(0);
       }
        public WorkOrderDetail OnChangePFS(WorkOrderDetail Wod)
        {
            string Style = Wod.WOFabric[0].Fabric;
            decimal Id = Wod.WOFabric[0].Id;
            decimal Lbs = Wod.WOFabric[0].Lbs;
            decimal qty = 0, newqty = 0;
            if (Wod.WOFabric[0].PullFromStock == true)
                Wod.WOFabric[0].PullFromStockInd = LOVConstants.Yes;
            else
                Wod.WOFabric[0].PullFromStockInd = LOVConstants.No;
            string PFSInd = Wod.WOFabric[0].PullFromStockInd;
            for (int i = 0; i < Wod.WOCumulative.Count; i++)
            {
                if (Wod.WOCumulative[i].CumulativeId == Id && Wod.WOCumulative[i].StyleCode == Style && Wod.WOCumulative[i].HiddenSizeDes == Wod.WOFabric[0].SizeShortDes)
                {
                    Wod.WOCumulative[i].PullFromStockIndicator = Wod.WOFabric[0].PullFromStockInd;
                    qty = Wod.WOCumulative[i].CurrentOrderQty;
                    break;

                }
            }
            for (int j = 0; j < Wod.WOCumulative.Count; j++)
            {
                if (Wod.WOCumulative[j].Merged == true && Wod.WOCumulative[j].CumulativeId == Id && Wod.WOCumulative[j].StyleCode == Style)
                {
                    if (Wod.WOFabric[0].PullFromStockInd == LOVConstants.No)
                    {
                        newqty = Wod.WOCumulative[j].Lbs + qty;
                        Wod.WOCumulative[j].Lbs = newqty.RoundCustom(0);
                        Wod.ActualLbs = Wod.ActualLbs + Lbs;
                        Wod.Lbs = Wod.ActualLbs.RoundCustom(0);
                        Wod.VarianceQty = Wod.VarianceQty + Lbs;
                        break;
                    }
                    else
                    {
                        newqty = Wod.WOCumulative[j].Lbs - qty;
                        Wod.WOCumulative[j].Lbs = newqty.RoundCustom(0);
                        Wod.ActualLbs = Wod.ActualLbs - Lbs;
                        Wod.Lbs = Wod.ActualLbs.RoundCustom(0);
                        Wod.VarianceQty = Wod.VarianceQty - Lbs;
                        break;
                    }
                }
            }
            CalculateVariance(Wod);
            return Wod;
        }

        
        public string SizeList(List<MultiSKUSizes>sizes)
        {

            string size=String.Empty;

            if(sizes!=null)
            {
                var arrSize=sizes.Select(i=>i.SizeCD).ToArray();
                size="'"+string.Join("','",arrSize)+"'";
            }  
            return size;
        }

        public string SizeShortDesList(List<MultiSKUSizes> sizes)
        {

            string sizedes = String.Empty;

            if (sizes != null)
            {
                var arrSize = sizes.Select(i => i.Size).ToArray();
                sizedes = "'" + string.Join("','", arrSize) + "'";
            }
            return sizedes;
        }
        public void GroupFabric(WorkOrderDetail Wod)
        {


            if (Wod.WOFabric != null)
            {
                foreach (var item in Wod.WOFabric.ToList())
                {
                    if (item.Merged)
                    {
                        Wod.WOFabric.Remove(item);
                    }
                }
                Wod.WOFabric.GroupBy(woFabric => new
                {
                   woFabric.SizeShortDes,
                    woFabric.Fabric,
                    woFabric.DyeCode,
                    woFabric.SpreadCode,
                    woFabric.CylSize,
                    woFabric.PullFromStockInd,
                    woFabric.CompCode,
                    woFabric.Id
                }).ToList().ForEach(item =>
                {
                    if (item.Count() > 0)
                    {
                        var fabric = new WorkOrderFabric();
                        fabric.Lbs = item.Sum(l => l.Lbs);
                        var first = item.FirstOrDefault();
                        fabric.Id = first.Id;
                        fabric.Fabric = first.Fabric;
                        fabric.DyeCode = first.DyeCode;
                        fabric.CompCode = first.CompCode;
                        fabric.CylSize = first.CylSize;
                        fabric.SpreadCode = first.SpreadCode;
                        fabric.SizeShortDes = first.SizeShortDes;
                        fabric.ParentStyle = first.ParentStyle;
                        fabric.ParentColor = first.ParentColor;
                        fabric.ParentAttribute = first.ParentAttribute;
                        fabric.ParentBoMId = first.ParentBoMId;
                        fabric.ParentMFGPathId = first.ParentMFGPathId;
                        fabric.ParentSize = first.ParentSize;
                        fabric.ParentSizeDes = first.ParentSizeDes;
                        fabric.PullFromStockInd = first.PullFromStockInd;
                        fabric.PullFromStock = first.PullFromStock;
                        item.ToList().ForEach(child =>
                        {
                            child.IsHide = true;
                        });
                        fabric.Merged = true;
                        Wod.WOFabric.Add(fabric);
                    }
                });
            }
        }
        public void GroupCumulativeGrid(WorkOrderDetail Wod)
        {
            if (Wod.WOCumulative != null)
            {
                foreach (var item in Wod.WOCumulative.ToList())
                {
                    if (item.Merged)
                    {
                        Wod.WOCumulative.Remove(item);
                    }
                }

                Wod.WOCumulative.Where(wocum => wocum.MatlTypeCode == LOVConstants.MATL_TYPE_CD.Fabric).GroupBy(woCumulative => new
          {
              woCumulative.StyleCode,
              woCumulative.ColorDyeCode,
              woCumulative.AttributeCompCode,
             woCumulative.Size
          }).ToList().ForEach(item =>
              {
                  if (item.Count() > 0)
                  {
                      var cumulative = new WorkOrderCumulative();
                      cumulative.Lbs = item.Sum(l => l.Lbs);
                      var first = item.FirstOrDefault();
                      cumulative.StyleCode = first.StyleCode;
                      cumulative.ColorDyeCode = first.ColorDyeCode;
                      cumulative.CumulativeId = first.CumulativeId;
                      cumulative.AttributeCompCode = first.AttributeCompCode;
                      cumulative.Size = first.Size;
                      cumulative.HiddenSizeDes = first.HiddenSizeDes;
                      cumulative.LevelInd = first.LevelInd;
                      cumulative.MatlTypeCode = first.MatlTypeCode;
                     
                      item.ToList().ForEach(child =>
                      {
                          child.IsHide = true;
                      });
                      cumulative.Merged = true;
                      Wod.WOCumulative.Add(cumulative);


                  }
              });
            }
           
        }

        public WorkOrderDetail ReCalcWODetail(WorkOrderDetail Wod)
        {
            if (Wod.WODetail.Count > 0)
            {
                ClearCumulativeAndFabric(Wod);
                for (int i = 0; i < Wod.WODetail.Count; i++)
                {
                    Wod.SellingStyle = Wod.WODetail[i].SellingStyle;
                    Wod.PKGStyle = Wod.WODetail[i].PKGStyle;
                    Wod.ColorCode = Wod.WODetail[i].ColorCode;
                    Wod.Attribute = Wod.WODetail[i].Attribute;
                    Wod.MfgPathId = Wod.WODetail[i].MfgPathId;
                    Wod.SizeList = Wod.WODetail[i].SizeList;
                    Wod.Revision = Wod.WODetail[i].Revision;
                    Wod.Id = Wod.WODetail[i].Id;
                    UpdateCumulative(Wod);

                }
                
            }
            return Wod;
        }
        public WorkOrderDetail CancelWODetail(WorkOrderDetail Wod)
        {
            if (Wod.GridMode == "add")
            {
                ClearCumulativeAndFabric(Wod);
                GroupCumulativeGrid(Wod);
                if (Wod.WOCumulative != null)
                    Wod.WOCumulative.Reverse();
                if (Wod.WOFabric != null)
                    Wod.WOFabric.Reverse();
                return Wod;
            }
            else if (Wod.GridMode == "edit")
            {

                ClearCumulativeAndFabric(Wod);

                if (Wod.WODetail.Count > 0)
                {
                    for (int i = 0; i < Wod.WODetail.Count; i++)
                    {
                        if (Wod.WODetail[i].Id == Wod.Id)
                        {
                            Wod.SellingStyle = Wod.WODetail[i].SellingStyle;
                            Wod.PKGStyle = Wod.WODetail[i].PKGStyle;
                            Wod.ColorCode = Wod.WODetail[i].ColorCode;
                            Wod.Attribute = Wod.WODetail[i].Attribute;
                            Wod.MfgPathId = Wod.WODetail[i].MfgPathId;
                            Wod.SizeList = Wod.WODetail[i].SizeList;
                            Wod.Revision = Wod.WODetail[i].Revision;
                            Wod.Id = Wod.WODetail[i].Id;
                            UpdateCumulative(Wod);

                        }
                    }

                }
                return Wod;
            }

            else if (Wod.GridMode == "delete")
            {
                ClearCumulativeAndFabric(Wod);
                GroupCumulativeGrid(Wod);
                if (Wod.WOCumulative != null)
                    Wod.WOCumulative.Reverse();
                if (Wod.WOFabric != null)
                    Wod.WOFabric.Reverse();
                return Wod;

            }
            else
                return null;
        }

        public void SetAlternateId(WorkOrderDetail Wod)
        {
            Wod.AlternateId = !String.IsNullOrEmpty(Wod.AlternateId) ? Wod.AlternateId.ToUpper() : Wod.AlternateId;
            if (!String.IsNullOrWhiteSpace(Wod.AlternateId))
            {
                if (!String.IsNullOrWhiteSpace(Wod.TempBoMId))
                {
                    Wod.TempBoMId = Wod.TempBoMId.Substring(0, Wod.TempBoMId.Length - 3) + Wod.AlternateId;
                    Wod.BoMId = Wod.TempBoMId;

                }


            }
            if (Wod.TempBoMId != null && Wod.TempBoMId != String.Empty)
            {
                Wod.AlternateId = Wod.TempBoMId.Substring(Wod.TempBoMId.Length - 3);

            }
            if (Wod.TempBoMId != null && Wod.TempBoMId.Length > 3)
            {
                Wod.iParentBomId = Wod.TempBoMId.Substring(0, Wod.TempBoMId.Length - 3);
            }
        }
        public bool ValidateCategoryCode(String catCode)
        {
            bool IsSuccess;
            var query = new StringBuilder();
            query.Append("select distinct iss_category_cd \"CategoryCode\" from iss_category where iss_category_cd='" + Val(catCode) + "'");
            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<WorkOrderDetail>(reader);
            if (result.Count != 0)
                IsSuccess = true;
            else
                IsSuccess = false;
            return IsSuccess;
        }


        //Get All Dc
        public List<DCCode> GetDCCode()
        {
            var query = new StringBuilder();

            query.Append("SELECT PLANT_CD \"DCD\",PLANT_NAME \"DCDescription\" FROM pLANT");

            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<DCCode>(reader);
           
            return result;
        }

        //Get All Demand Drivers
        public List<DemandDrivers> GetDemandDrivers(string style, string color, string attribute, string size,string RevisionNO)
        {
            var query = new StringBuilder();
            //query.Append("Select c.code \"Code\",c.code || '-' || c.short_description  \"Description\" from external_customer_detail d, codes_table c where d.Customer_cd = (select m.customer_cd from external_sku_master m where m.selling_style_cd = '" + style + "' and m.selling_color_cd = '" + color + "'   and m.selling_attribute_cd = '" + attribute + "'  and m.selling_size_cd in (" + size + ") and m.selling_revision_no = '" + RevisionNO + "' and rownum = 1 )  and d.category = 'DDR' and d.display_ind = 'Y' and d.status_cd = 'A' and d.category = c.category  and d.code = c.code order by d.code_value");
            //query.Append("Select c.code \"Code\",c.code || '-' || c.short_description  \"Description\" from external_customer_detail d, codes_table c ");
            query.Append("Select c.code \"Code\",c.code || '-' || c.short_description  \"Description\" from external_customer_detail d, codes_table c where d.Customer_cd = (select m.customer_cd from external_sku_master m where m.selling_style_cd = '" + style + "' and m.selling_color_cd = '" + color + "'   and m.selling_attribute_cd = '" + attribute + "'  and m.selling_size_cd in ('" + size + "') and rownum = 1 )  and d.category = 'DDR' and d.display_ind = 'Y' and d.status_cd = 'A' and d.category = c.category  and d.code = c.code order by d.code_value");
            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<DemandDrivers>(reader);
            return result;
        }
        public List<DemandDrivers> GetDemandDriver()
        {
            var query = new StringBuilder();
            query.Append("Select c.code \"Code\", c.code || '-' || c.short_description  \"Description\" from external_customer_detail d, codes_table c where d.Customer_cd = (select m.customer_cd from external_sku_master m where rownum = 1 )  and d.category = 'DDR' and d.display_ind = 'Y' and d.status_cd = 'A' and d.category = c.category  and d.code = c.code order by d.code_value");
            //query.Append("Select c.code \"Code\",c.code || '-' || c.short_description  \"Description\" from external_customer_detail d, codes_table c ");
            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<DemandDrivers>(reader);
            return result;
        }
        public bool GetPopupHaaAO(string valHaa)
        {
            bool IsSuccess;
            var query = new StringBuilder();
            query.Append("select distinct customer_cd \"customer_cd\" from external_customer_header where status_cd = 'A' and customer_cd ='" + Val(valHaa) + "'");
            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<WorkOrderDetail>(reader);
            if (result.Count != 0)
                IsSuccess = true;
            else
                IsSuccess = false;
            return IsSuccess;
        }
        public List<AttributionMrz> GetAttributeM(string orderid, string style, string color, string attribute, string size)
        {
            var query = new StringBuilder();
            query.Append("Select v.order_version \"OrderVersion\", v.order_id \"OrderId\", v.super_order \"SuperOrder\", v.style_cd \"Style\", v.color_cd \"Color\", v.attribute_cd \"Attribute\", v.size_cd \"Size\", v.mfg_path_id \"MfgPathId\", v.curr_order_qty \"CurrQty\", v.total_curr_order_qty \"TotalQty\" ,v.production_status \"ProdStatus\", v.curr_due_date \"CurrDueDate\" ");
            query.Append("from ISS_PROD_ORDER_VIEW v, iss_prod_order p where v.order_version = p.order_version and v.super_order = p.super_order and v.order_id = p.order_id    and p.iss_order_type_cd = 'WO' ");
            //query.Append(" and p.attribution_ind = 'Y'  ");
            query.Append(" and p.order_id <> p.super_order   ");
            if (!string.IsNullOrWhiteSpace(style)) query.Append(" and v.style_cd ='" + Val(style) + "' ");
            if (!string.IsNullOrWhiteSpace(color)) query.Append(" and v.color_cd ='" + Val(color) + "' ");
            if (!string.IsNullOrWhiteSpace(attribute)) query.Append(" and v.attribute_cd ='" + Val(attribute) + "' ");
            if (!string.IsNullOrWhiteSpace(size)) query.Append("and v.size_cd ='" + Val(size) + "' ");
            if (!string.IsNullOrWhiteSpace(orderid)) query.Append(" and v.order_id ='" + Val(orderid) + "' ");
            query.Append("and v.production_status = 'R'  and v.order_id is not null ");
            query.Append("and v.Order_id not in (Select z.Order_id from ISS_PROD_ORDER_MRZ_AUDIT z) "); 
            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<AttributionMrz>(reader);
            return result;
        }

        public bool DeleteAttributeMrzData(AttributionMrz item, String userId)
        {
                var queryBuilder = new StringBuilder();

                queryBuilder.Append("INSERT INTO ISS_PROD_ORDER_MRZ_AUDIT");
                queryBuilder.Append("(Order_ID, Order_version, Super_Order, Style_cd, Color_cd, Attribute_Cd, Size_cd, Quantity, Curr_Due_Date, Status_cd, User_id, Create_Date, Update_Date)");
                queryBuilder.Append(" VALUES('" + Val(item.OrderId) + "', " + item.OrderVersion + ", '" + Val(item.SuperOrder) + "', '" + Val(item.Style) + "', '" + Val(item.Color) + "', '" + Val(item.Attribute) + "',");
                queryBuilder.Append(" '" + Val(item.Size) + "', " + item.TotalQty + ", to_date('" + item.CurrDueDate + "','mm/dd/yyyy hh:mi:ss am'), 'R', '" + userId + "', sysdate, sysdate)");

                System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
                var result = (String)ExecuteScalar(queryBuilder.ToString());

                //return (result == null || result == "Y") ? true : false;

                var query = new StringBuilder();
                query.Append("INSERT INTO PROD_ORDER_LOG");
                query.Append("(PROD_ORDER_NO, TRANSACTION_NO, TRANSACTION_DATE, TRANSACTION_TIME, UPDATE_CD, STATUS_CD, CREATE_DATE, UPDATE_DATE)");
                query.Append(" VALUES('" + Val(item.OrderId) + "', avyx_trans_no.NEXTVAL, sysdate, 'DUMMY', 'D', 'E', sysdate, sysdate)");

                System.Diagnostics.Debug.WriteLine(query.ToString());
                var status = (String)ExecuteScalar(query.ToString());
                return (status == null || status == "Y") ? true : false;

        }
        
    }
}
