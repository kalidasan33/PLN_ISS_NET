using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISS.Common;
using ISS.Core.Model.Common;
using ISS.Core.Model.Order;
using ISS.DAL;
using ISS.Common;
using System.Data;
using System.Diagnostics;
using Oracle.DataAccess.Client;
using System.Transactions;
using System.Data.Common;

namespace ISS.Repository.Order
{
    public partial class WOManagement : RepositoryBase
    {
        public WOManagement()
            : base()
        {

        }
        public WOManagement(DbTransaction trans)
            : base(trans)
        {

        }

        public List<WOMDetail> GetWODetail(WOManagementSearch search)
        {
            StringBuilder query = new StringBuilder(string.Empty);

            bool IsAO = false;

            bool atrWc = false;
            bool capWc = false;

            string capGroups = search.CapacityGroup;


            // Check for non attr capacity filtr
            if (!string.IsNullOrEmpty(search.WorkCenter))
            {

                if (capGroups.IndexOf(CapacityGroup.Sew.ToString().ToUpper()) >= 0) 
                {
                    capGroups = capGroups.Replace("SEW", "");
                    capWc = true;

                }
                if (capGroups.IndexOf(CapacityGroup.Src.ToString().ToUpper()) >= 0) 
                {
                    capGroups = capGroups.Replace("SRC", "");
                    capWc = true;
                }
                if (capGroups.IndexOf(CapacityGroup.Cut.ToString().ToUpper()) >= 0) 
                {
                    capGroups = capGroups.Replace("CUT", "");
                    capWc = true;
                }
                if (capGroups.IndexOf(CapacityGroup.Tex.ToString().ToUpper()) >= 0) 
                {
                    capGroups = capGroups.Replace("TEX", "");
                    capWc = true;
                }
                capGroups = capGroups.Replace(",", "").Replace("''", "");

                atrWc = !string.IsNullOrEmpty(capGroups);
                
            }


            if (!String.IsNullOrEmpty(search.Source) && search.Source == LOVConstants.WorkOrderType.AttributedWO)
            {
                IsAO = true;
            }

            query.Append("select  a.order_version \"OrderVersion\", a.super_order \"SuperOrder\", a.multi_sku \"GroupId\", a.order_id \"OrderId\", a.iss_order_type_cd \"OrderType\", decode(a.production_status,'P','S', a.production_status) \"OrderStatus\", nvl(a.selling_style_cd,a.style_cd) \"SellingStyle\", a.style_cd \"Style\", color_cd \"Color\",   attribute_cd \"Attribute\", b.size_short_desc \"SizeShortDes\", nvl(a.cylinder_size,0) \"CylinderSizes\",a.mfg_revision_no \"Revision\", a.cutting_alt \"AltId\",  a.curr_order_qty  \"Qty\"" +

                ((IsAO) ? " ,trunc(a.curr_order_qty / 12, 0) || case   when (mod(a.curr_order_qty, 12))= 0 then '-00'   else replace(mod(a.curr_order_qty, 12) / 100 ,'.','-') end \"QtyEach\" " : "")

                + " , a.total_curr_order_qty \"TotatalCurrOrderQty\" ,round( a.greige_lbs,6) \"Greigelbs\",  a.machine_type_cd \"MC\" , (select Count(PULL_FROM_STOCK_IND) PFS FROM iss_prod_order_detail WHERE order_version=1 and super_order = a.super_order and PULL_FROM_STOCK_IND = 'Y') PFS,  nvl(a.txt_start_date,a.earliest_start)    \"StartDate\",  a.curr_due_date  \"CurrDueDate\", nvl(a.rule_number,0) \"Rule\", NVL(a.txt_path,a.cut_path)\"TxtPath\", a.cut_path \"CutPath\", nvl(a.sew_plant,' ') \"SewPath\", a.sew_path \"MfgPathId\",a.sew_plant \"Atr\", a. demand_loc \"DcLoc\", a.expedite_priority \"ExpeditePriority\",a.category_cd \"CategoryCode\", a.demand_type \"DemandType\",   decode(a.enforcement,11,'GS',3,'S','') Enforcement,a.priority \"Priority\", a.create_bd_ind \"CreateBDInd\", a.dozens_only_ind \"DozensOnlyInd\",a.pack_cd \"PackCode\",'N' as bom_refresh,  a.make_or_buy_cd \"MakeOrBuy\", a.note_ind \"NoteInd\",  decode(a.production_status,'R',CAST (NULL AS DATE),a.demand_date) \"DemandDate\", a.demand_source \"DemandSource\",    a.size_cd \"Size\",  a.spread_type_cd \"SpreadTypeCode\", a.REMOTE_UPDATE_CD \"RemoteUpdateCode\", a.dye_cd \"DyeBle\", a.bom_spec_id \"BoMId\", a.scrap_factor \"ScrapFactor\", a.unit_of_measure \"UOM\", a.order_status_cd ,a.demand_driver \"DemandDriver\",  nvl(sew_work_center,'') \"WorkCenter\", nvl(a.asrmt_cd,'N') \"AssortCode\", EARLIEST_START \"EarliestStartDate\",nvl(a.garment_style,a.style_cd) \"GarmentStyle\", a.remote_release_date  \"RemoteUpdateDate\", a.discrete_ind \"DescreteInd\", nvl(a.selling_color_cd,a.color_cd) \"SellingColor\", nvl(a.selling_attribute_cd,a.attribute_cd) \"SellingAttribute\",nvl(a.selling_size_cd,a.size_cd) \"SellingSize\", a.lbs \"Lbs\",  a.order_reference \"OrderRef\", p.user_id \"UpdatedBy\" ");
            //,nvl(attribution_ind,'N') AttributionInd

            if (IsAO)
            {
                query.Append(" , (select al.comp_style || '~' || al.comp_color || '~' || al.comp_attribute_cd || '~' || al.comp_size_cd || '~' || al.comp_revision_no  ");
                query.Append(" from iss_prod_order_allocation al ");



                query.Append(" where al.order_version(+) =" + LOVConstants.GlobalOrderVersion + "     and al.order_label(+) = a.super_order and nvl(al.activity_cd,'PUL')='PUL' and rownum <=1) \"GarmentSKU\"   ");

            }
            query.Append(" from  ( select  a.* from iss_prod_order_view a, style s where  ");

            //if (!IsAO)
            //{
            //    query.Append(" a.attribute_cd ='" + LOVConstants.NonAOAttribute + "'");
            //}
            //else if (IsAO)
            //{
            //    query.Append(" a.attribute_cd !='" + LOVConstants.NonAOAttribute + "'");
            //}

            if (!IsAO)
            {
                query.Append(" nvl(attribution_ind,'N') ='" + LOVConstants.AttributionInd.WOM + "'");
            }
            else if (IsAO)
            {
                query.Append(" nvl(attribution_ind,'N') ='" + LOVConstants.AttributionInd.AWOM + "'");
            }

            //if (!IsAO)
            //{
            //    query.Append(" a.attribute_cd ='" + LOVConstants.NonAOAttribute + "'");
            //}
            //else if (IsAO)
            //{
            //    query.Append(" a.attribute_cd !='" + LOVConstants.NonAOAttribute + "'");
            //}

            var str2 = new StringBuilder("");

            if (!String.IsNullOrEmpty(search.SuperOrder)) str2.Append(" and a.super_order='" + search.SuperOrder + "' ");

            //and o.STYLE_CD = 'MHHH51'  and o.COLOR_CD = 'CLR'  and o.ATTRIBUTE_CD = 'ATTR' 
            if (search.StyleType == LOVConstants.StyleType.SellingStyle)
            {
                if (!string.IsNullOrWhiteSpace(search.SStyle)) str2.Append(" and o.SELLING_STYLE_CD  like '" + (search.SStyle) + "%' ");

                if (!string.IsNullOrWhiteSpace(search.SColor)) str2.Append(" and  o.SELLING_COLOR_CD   like '" + (search.SColor) + "%' ");

                if (!string.IsNullOrWhiteSpace(search.SAttribute)) str2.Append(" and o.SELLING_ATTRIBUTE_CD like '" + Val(search.SAttribute) + "%' ");
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(search.SStyle)) str2.Append(" and o.STYLE_CD  like '" + (search.SStyle) + "%' ");
                if (!string.IsNullOrWhiteSpace(search.SAttribute)) str2.Append(" and  o.ATTRIBUTE_CD like '" + Val(search.SAttribute) + "%' ");
                if (!string.IsNullOrWhiteSpace(search.SColor)) str2.Append(" and o.COLOR_CD like '" + (search.SColor) + "%' ");
            }

            if (!string.IsNullOrWhiteSpace(search.DC)) str2.Append(" and o.DEMAND_LOC like '" + (search.DC) + "%' ");
            if (!string.IsNullOrWhiteSpace(search.MfgPathId)) str2.Append(" and o.MFG_PLANT like '" + (search.MfgPathId) + "%' ");

            // US 56194 Add Attibute filter if only ATR Workcenters are selected 

            if (!string.IsNullOrWhiteSpace(search.WorkCenter))
            {


                string s = "";

                if (capWc)
                {
                    s = "  nvl(o.CAPACITY_ALLOC ,'[CATCHALL') in ( " + FormatInClause(search.WorkCenter) + " ) ";
                    if (atrWc) s += " or ";
                }

                if (atrWc)
                {
                    s += " c.atr_alloc_1 in ( " + FormatInClause(search.WorkCenter) + " ) ";
                    s += " or c.atr_alloc_2 in ( " + FormatInClause(search.WorkCenter) + " ) ";
                    s += " or c.atr_alloc_3 in ( " + FormatInClause(search.WorkCenter) + " ) ";
                }

                str2.Append(" and (" + s + ")");
            }
            

            if (!string.IsNullOrWhiteSpace(search.Rev)) str2.Append(" and o.MFG_REVISION_NO like '" + (search.Rev) + "%' ");

            // Dynamic Join based on BOMUpdates Only
            if (search.BOMMismatches) str2.Append("  and o.Super_Order = p.Super_Order and p.remote_update_cd = 'F'  and Upper(p.Remote_Xcpn_Reason) like Upper('BOM Mismatch%' ) ");


            if (!String.IsNullOrEmpty(str2.ToString()))
            {
                query.Append(" and (a.order_version,a.super_order) in (select o.order_version ,o.super_order  from iss_prod_order_detail o ");
                
                // US56914 ISS WEb Attr 
                if (atrWc) query.Append(" ,   da.iss_cap_alloc_xref c ");
                
                
                // Dynamic Join based on BOMUpdates Only
                if (search.BOMMismatches) query.Append(" , iss_prod_order p ");
                query.Append(" where o.order_version = 1   and o.matl_type_cd in ( '00','CT') ");


                // US56914 ISS WEb Attr 
                if (atrWc)
                    query.Append(" and  o.selling_style_cd = c.style_cd (+)  and o.selling_attribute_cd = c.attribute_cd  (+) ");



                query.Append(str2.ToString());
                // query.Append(" and o.make_or_buy_cd <> 'B' "); //??TBD

                query.Append("  )  "); //end _type_cd in ( '00','CT'

            }


            str2.Clear();
            if (!string.IsNullOrWhiteSpace(search.MFGPlant)) str2.Append(" and o.MFG_PLANT in (" + FormatInClause(search.MFGPlant) + " ) ");
            if (!string.IsNullOrWhiteSpace(search.Machine)) str2.Append(" and o.MACHINE_TYPE_CD in (" + FormatInClause(search.Machine) + " ) ");
            if (!string.IsNullOrWhiteSpace(search.CylinderSize)) str2.Append(" and o.CYLINDER_SIZE in (" + FormatInClauseNumeric(search.CylinderSize) + " ) ");

            if (!string.IsNullOrWhiteSpace(search.DyeBle)) str2.Append(" and  decode(o.dye_shade_cd,'P','D','S','B',o.dye_shade_cd) in (" + FormatInClause(search.DyeBle) + ") ");
            if (!string.IsNullOrWhiteSpace(search.TextileGroup)) str2.Append(" and o.FABRIC_GROUP in (" + FormatInClause(search.TextileGroup) + " ) ");
            if (!string.IsNullOrWhiteSpace(search.Fabric)) str2.Append(" and o.style_cd in (" + FormatInClause(search.Fabric) + " ) ");   //Newly Added
            if (!string.IsNullOrWhiteSpace(search.Yarn)) str2.Append(" and o.order_label = a.ORDER_LABEL and a.COMP_STYLE in (" + FormatInClause(search.Yarn) + " ) ");
            if (!string.IsNullOrWhiteSpace(search.Alt)) str2.Append(" and o.CUTTING_ALT in (" + FormatInClause(search.Alt) + " ) ");
            if (!string.IsNullOrWhiteSpace(search.GroupId)) str2.Append(" and a.MULTI_SKU in (" + FormatInClause(search.GroupId) + " ) ");

            

            if (!String.IsNullOrEmpty(str2.ToString()))
            {
                query.Append(" and (a.order_version,a.super_order) in 	(select o.order_version ,o.super_order  from iss_prod_order_detail o  ");
                if (!string.IsNullOrWhiteSpace(search.Yarn)) query.Append(" ,iss_prod_order_allocation a ");
                query.Append("  where o.order_version = 1  and o.matl_type_cd = 'F'   ");
                if (!string.IsNullOrWhiteSpace(search.Yarn)) query.Append(" and o.order_version = a.order_version and o.order_label = a.ORDER_LABEL ");
                query.Append(str2.ToString());
                query.Append(" )  ");
            }

            str2.Clear();
            if (!string.IsNullOrWhiteSpace(search.Planner)) str2.Append(" and o.PLANNER_CD in (" + FormatInClause(search.Planner) + " ) ");
            if (!string.IsNullOrWhiteSpace(search.Rule)) str2.Append(" and o.RULE_NUMBER in (" + FormatInClause(search.Rule) + " ) ");
            if (!string.IsNullOrWhiteSpace(search.BusinessUnit) && search.BusinessUnit != "All") str2.Append(" and o.CORP_BUSINESS_UNIT in (" + FormatInClause(search.BusinessUnit) + " ) ");

            if (!String.IsNullOrEmpty(str2.ToString()))
            {
                query.Append(" and (a.order_version,a.super_order) in (select o.order_version ,o.super_order  from iss_prod_order o where o.order_version = 1  ");
                query.Append(str2.ToString());


                query.Append(")  ");
            }
            query.Append(" AND a.style_cd = s.style_cd ");
            if (!string.IsNullOrWhiteSpace(search.CorpDiv)) query.Append("  and s.corp_division_cd in (" + FormatInClause(search.CorpDiv) + " ) ");

            query.Append("   )  a, item_size b, item_size b1 , iss_prod_order p ");
            query.Append("  where   a.size_cd = b.size_cd ");
            if (!string.IsNullOrWhiteSpace(search.SSize))
                query.Append(" and b1.size_short_desc in ( " + FormatInClause(search.SSize) + ") ");


            ISS.Repository.Common.ApplicationRepository repositoryPlanner = new ISS.Repository.Common.ApplicationRepository();
            var resultPlaDate = repositoryPlanner.GetPlanBeginEndDates();

            var WeekEnd = resultPlaDate.Select(x => x.Week_End_Date).FirstOrDefault();

            var MinDate = DateTime.Now;
            var NewDate = DateTime.Now;
            var offset = 0;

            var flag = true;
            if (search.Week == LOVConstants.WOMWeeks.CurrentPriorWeek)
            {
                MinDate = WeekEnd.AddDays(-250);
                NewDate = WeekEnd;
            }
            else if (search.Week == LOVConstants.WOMWeeks.PlanWeekOne)
            {
                MinDate = WeekEnd.AddDays(1);
                NewDate = WeekEnd.AddDays(7);
            }
            else if (search.Week == LOVConstants.WOMWeeks.PlanWeekTwo)
            {
                MinDate = WeekEnd.AddDays(7 * 1 + 1);
                NewDate = WeekEnd.AddDays(7 * 2);
            }
            else if (search.Week == LOVConstants.WOMWeeks.PlanWeekThree)
            {
                MinDate = WeekEnd.AddDays(7 * 2 + 1);
                NewDate = WeekEnd.AddDays(7 * 3);
            }
            else
            {
                //TBD
                flag = false;

                if (!string.IsNullOrEmpty(search.Week))
                {
                    MinDate = Convert.ToDateTime(search.Week);
                    NewDate = MinDate;
                }
            }

            if (!String.IsNullOrEmpty(search.MoreWeeks))
            {
                offset = int.Parse(search.MoreWeeks);
                NewDate = NewDate.AddDays(7 * offset);
            }



            var DateField = "";
            if (search.DueDate == LOVConstants.WOMDueDates.DC)
            {
                DateField = " a.curr_due_date ";
            }
            else if (search.DueDate == LOVConstants.WOMDueDates.Sew)
            {
                DateField = " a.sew_start_date ";
            }
            else if (search.DueDate == LOVConstants.WOMDueDates.Cut)
            {
                DateField = " a.cut_start_date ";
            }
            else if (search.DueDate == LOVConstants.WOMDueDates.BD)
            {
                DateField = " a.txt_start_date ";
            }
            else
            {
                DateField = " a.earliest_start ";
            }

            if (flag || string.IsNullOrEmpty(search.GroupId))
            {
                query.Append(" and trunc( " + DateField + ")  >= to_date('" + MinDate.ToString(LOVConstants.DateFormatOracle) + "','YYYYMMDD') and trunc(" + DateField + ")  <= to_date('" + NewDate.ToString(LOVConstants.DateFormatOracle) + "','YYYYMMDD')   ");
            }

            if (search.StyleType == LOVConstants.StyleType.SellingStyle)
            {
                query.Append(" and A.SELLING_size_cd = b1.size_cd  ");
            }
            else
            {
                query.Append(" and A.size_cd = b1.size_cd  ");
            }
            query.Append(" and a.production_status in ('L','P','R')  and a.order_status_cd <> '3' AND a.order_version = p.order_version AND a.super_order = p.super_order and rownum < 10000000  ");


            

            query.Append(" order by a.multi_sku,nvl(a.rule_number,999),a.PRIORITY,a.earliest_start,a.style_cd, a.color_cd, a.attribute_cd, a.size_cd, a.demand_loc");

            Debug.WriteLine(query.ToString());

            //  var date = DateTime.Now;
            IDataReader reader = ExecuteReader(query.ToString());

            var result = (new DbHelper()).ReadData<WOMDetail>(reader);
            // var ts = DateTime.Now - date;
            // Debug.WriteLine("Generic Minuts -" + ts.Minutes + " Sec-" + ts.Seconds);

            result = MeetsFilterGrid(result, search);
            return result;

        }

        public List<WOMDetail> GetWODetailExport(WOManagementSearch search)
        {
            StringBuilder query = new StringBuilder(string.Empty);

            bool IsAO = false;
            if (!String.IsNullOrEmpty(search.Source) && search.Source == LOVConstants.WorkOrderType.AttributedWO)
            {
                IsAO = true;
            }

            query.Append("select  a.order_version \"OrderVersion\", a.super_order \"SuperOrder\", a.multi_sku \"GroupId\", a.order_id \"OrderId\", a.iss_order_type_cd \"OrderType\", decode(a.production_status,'P','S', a.production_status) \"OrderStatus\", nvl(a.selling_style_cd,a.style_cd) \"SellingStyle\", a.style_cd \"Style\", color_cd \"Color\",   attribute_cd \"Attribute\", b.size_short_desc \"SizeShortDes\", nvl(a.cylinder_size,0) \"CylinderSizes\",a.mfg_revision_no \"Revision\", a.cutting_alt \"AltId\",  a.curr_order_qty  \"Qty\"" +

                ((IsAO) ? " ,trunc(a.curr_order_qty / 12, 0) || case   when (mod(a.curr_order_qty, 12))= 0 then '-00'   else replace(to_char(mod(a.curr_order_qty, 12) / 100,'fm.00') ,'.','-') end \"QtyEach\" " : "")

                + " , a.total_curr_order_qty \"TotatalCurrOrderQty\" ,round( a.greige_lbs,6) \"Greigelbs\",  a.machine_type_cd \"MC\" , (select Count(PULL_FROM_STOCK_IND) PFS FROM iss_prod_order_detail WHERE order_version=1 and super_order = a.super_order and PULL_FROM_STOCK_IND = 'Y') PFS,  nvl(a.txt_start_date,a.earliest_start)    \"StartDate\",  a.curr_due_date  \"CurrDueDate\", nvl(a.rule_number,0) \"Rule\", NVL(a.txt_path,a.cut_path)\"TxtPath\", a.cut_path \"CutPath\", nvl(a.sew_plant,' ') \"SewPath\", a.sew_path \"MfgPathId\", a.sew_plant \"Atr\", a. demand_loc \"DcLoc\", a.expedite_priority \"ExpeditePriority\",a.category_cd \"CategoryCode\", a.demand_type \"DemandType\",   decode(a.enforcement,11,'GS',3,'S','') Enforcement,a.priority \"Priority\", a.create_bd_ind \"CreateBDInd\", a.dozens_only_ind \"DozensOnlyInd\",a.pack_cd \"PackCode\",'N' as bom_refresh,  a.make_or_buy_cd \"MakeOrBuy\", a.note_ind \"NoteInd\",  decode(a.production_status,'R',CAST (NULL AS DATE),a.demand_date) \"DemandDate\", a.demand_source \"DemandSource\",    a.size_cd \"Size\",  a.spread_type_cd \"SpreadTypeCode\", a.REMOTE_UPDATE_CD \"RemoteUpdateCode\", a.dye_cd \"DyeBle\", a.bom_spec_id \"BoMId\", a.scrap_factor \"ScrapFactor\", a.unit_of_measure \"UOM\", a.order_status_cd ,a.demand_driver \"DemandDriver\",  nvl(sew_work_center,'') \"WorkCenter\", nvl(a.asrmt_cd,'N') \"AssortCode\", EARLIEST_START \"EarliestStartDate\",nvl(a.garment_style,a.style_cd) \"GarmentStyle\", a.remote_release_date  \"RemoteUpdateDate\", discrete_ind \"DescreteInd\", nvl(a.selling_color_cd,a.color_cd) \"SellingColor\", nvl(a.selling_attribute_cd,a.attribute_cd) \"SellingAttribute\",nvl(a.selling_size_cd,a.size_cd) \"SellingSize\", a.lbs \"Lbs\",  a.order_reference \"OrderRef\"  ");
            //,nvl(attribution_ind,'N') AttributionInd

            if (IsAO)
            {
                query.Append(" , (select al.comp_style || '~' || al.comp_color || '~' || al.comp_attribute_cd || '~' || al.comp_size_cd || '~' || al.comp_revision_no  ");
                query.Append(" from iss_prod_order_allocation al ");



                query.Append(" where al.order_version(+) =" + LOVConstants.GlobalOrderVersion + "     and al.order_label(+) = a.super_order and nvl(al.activity_cd,'PUL')='PUL' and rownum <=1) \"GarmentSKU\"   ");

            }
            query.Append(" from  ( select  a.* from iss_prod_order_view a, style s where  ");

            //if (!IsAO)
            //{
            //    query.Append(" a.attribute_cd ='" + LOVConstants.NonAOAttribute + "'");
            //}
            //else if (IsAO)
            //{
            //    query.Append(" a.attribute_cd !='" + LOVConstants.NonAOAttribute + "'");
            //}

            if (!IsAO)
            {
                query.Append(" nvl(attribution_ind,'N') ='" + LOVConstants.AttributionInd.WOM + "'");
            }
            else if (IsAO)
            {
                query.Append(" nvl(attribution_ind,'N') ='" + LOVConstants.AttributionInd.AWOM + "'");
            }

            //if (!IsAO)
            //{
            //    query.Append(" a.attribute_cd ='" + LOVConstants.NonAOAttribute + "'");
            //}
            //else if (IsAO)
            //{
            //    query.Append(" a.attribute_cd !='" + LOVConstants.NonAOAttribute + "'");
            //}

            var str2 = new StringBuilder("");

            if (!String.IsNullOrEmpty(search.SuperOrder)) str2.Append(" and a.super_order='" + search.SuperOrder + "' ");

            //and o.STYLE_CD = 'MHHH51'  and o.COLOR_CD = 'CLR'  and o.ATTRIBUTE_CD = 'ATTR' 
            if (search.StyleType == LOVConstants.StyleType.SellingStyle)
            {
                //if (!string.IsNullOrWhiteSpace(search.SStyle)) str2.Append(" and o.SELLING_STYLE_CD  " + SetOperator(search.SStyle)); // Changed-Req#36116-18

                if (!string.IsNullOrWhiteSpace(search.SStyle)) str2.Append(" and o.SELLING_STYLE_CD like '" + (search.SStyle) + "%' ");

                if (!string.IsNullOrWhiteSpace(search.SColor)) str2.Append(" and  o.SELLING_COLOR_CD  " + SetOperator(search.SColor));

                if (!string.IsNullOrWhiteSpace(search.SAttribute)) str2.Append(" and o.SELLING_ATTRIBUTE_CD = '" + Val(search.SAttribute) + "' ");
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(search.SStyle)) str2.Append(" and o.STYLE_CD  " + SetOperator(search.SStyle));
                if (!string.IsNullOrWhiteSpace(search.SAttribute)) str2.Append(" and  o.ATTRIBUTE_CD ='" + Val(search.SAttribute) + "' ");
                if (!string.IsNullOrWhiteSpace(search.SColor)) str2.Append(" and o.COLOR_CD " + SetOperator(search.SColor));
            }

            if (!string.IsNullOrWhiteSpace(search.DC)) str2.Append(" and o.DEMAND_LOC " + SetOperator(search.DC));
            if (!string.IsNullOrWhiteSpace(search.MfgPathId)) str2.Append(" and o.MFG_PLANT " + SetOperator(search.MfgPathId));
            if (!string.IsNullOrWhiteSpace(search.WorkCenter)) str2.Append(" and nvl(o.CAPACITY_ALLOC ,'[CATCHALL') in ( " + FormatInClause(search.WorkCenter) + " ) ");
            if (!string.IsNullOrWhiteSpace(search.Rev)) str2.Append(" and o.MFG_REVISION_NO " + SetOperator(search.Rev));

            // Dynamic Join based on BOMUpdates Only
            if (search.BOMMismatches) str2.Append("  and o.Super_Order = p.Super_Order and p.remote_update_cd = 'F'  and Upper(p.Remote_Xcpn_Reason) like Upper('BOM Mismatch%' ) ");


            if (!String.IsNullOrEmpty(str2.ToString()))
            {
                query.Append(" and (a.order_version,a.super_order) in (select o.order_version ,o.super_order  from iss_prod_order_detail o ");
                // Dynamic Join based on BOMUpdates Only
                if (search.BOMMismatches) query.Append(" , iss_prod_order p ");
                query.Append(" where o.order_version = 1   and o.matl_type_cd in ( '00','CT') ");
                query.Append(str2.ToString());
                // query.Append(" and o.make_or_buy_cd <> 'B' "); //??TBD
                query.Append("  )  "); //end _type_cd in ( '00','CT'

            }


            str2.Clear();
            if (!string.IsNullOrWhiteSpace(search.MFGPlant)) str2.Append(" and o.MFG_PLANT in (" + FormatInClause(search.MFGPlant) + " ) ");
            if (!string.IsNullOrWhiteSpace(search.Machine)) str2.Append(" and o.MACHINE_TYPE_CD in (" + FormatInClause(search.Machine) + " ) ");
            if (!string.IsNullOrWhiteSpace(search.CylinderSize)) str2.Append(" and o.CYLINDER_SIZE in (" + FormatInClauseNumeric(search.CylinderSize) + " ) ");

            if (!string.IsNullOrWhiteSpace(search.DyeBle)) str2.Append(" and  decode(o.dye_shade_cd,'P','D','S','B',o.dye_shade_cd) in (" + FormatInClause(search.DyeBle) + ") ");
            if (!string.IsNullOrWhiteSpace(search.TextileGroup)) str2.Append(" and o.FABRIC_GROUP in (" + FormatInClause(search.TextileGroup) + " ) ");
            if (!string.IsNullOrWhiteSpace(search.Yarn)) str2.Append(" and o.order_label = a.ORDER_LABEL and a.COMP_STYLE in (" + FormatInClause(search.Yarn) + " ) ");
            if (!string.IsNullOrWhiteSpace(search.Alt)) str2.Append(" and o.CUTTING_ALT in (" + FormatInClause(search.Alt) + " ) ");
            if (!string.IsNullOrWhiteSpace(search.GroupId)) str2.Append(" and a.MULTI_SKU in (" + FormatInClause(search.GroupId) + " ) ");

            if (!String.IsNullOrEmpty(str2.ToString()))
            {
                query.Append(" and (a.order_version,a.super_order) in 	(select o.order_version ,o.super_order  from iss_prod_order_detail o  ");
                if (!string.IsNullOrWhiteSpace(search.Yarn)) query.Append(" ,iss_prod_order_allocation a ");
                query.Append("  where o.order_version = 1  and o.matl_type_cd = 'F'   ");
                if (!string.IsNullOrWhiteSpace(search.Yarn)) query.Append(" and o.order_version = a.order_version and o.order_label = a.ORDER_LABEL ");
                query.Append(str2.ToString());
                query.Append(" )  ");
            }

            str2.Clear();
            if (!string.IsNullOrWhiteSpace(search.Planner)) str2.Append(" and o.PLANNER_CD in (" + FormatInClause(search.Planner) + " ) ");
            if (!string.IsNullOrWhiteSpace(search.Rule)) str2.Append(" and o.RULE_NUMBER in (" + FormatInClause(search.Rule) + " ) ");
            if (!string.IsNullOrWhiteSpace(search.BusinessUnit) && search.BusinessUnit != "All") str2.Append(" and o.CORP_BUSINESS_UNIT in (" + FormatInClause(search.BusinessUnit) + " ) ");

            if (!String.IsNullOrEmpty(str2.ToString()))
            {
                query.Append(" and (a.order_version,a.super_order) in (select o.order_version ,o.super_order  from iss_prod_order o where o.order_version = 1  ");
                query.Append(str2.ToString());


                query.Append(")  ");
            }
            query.Append(" AND a.style_cd = s.style_cd ");
            if (!string.IsNullOrWhiteSpace(search.CorpDiv)) query.Append("  and s.corp_division_cd in (" + FormatInClause(search.CorpDiv) + " ) "); 

            query.Append("   )  a, item_size b, item_size b1  ");
            query.Append("  where   a.size_cd = b.size_cd ");
            if (!string.IsNullOrWhiteSpace(search.SSize))
                query.Append(" and b1.size_short_desc in ( " + FormatInClause(search.SSize) + ") ");


            ISS.Repository.Common.ApplicationRepository repositoryPlanner = new ISS.Repository.Common.ApplicationRepository();
            var resultPlaDate = repositoryPlanner.GetPlanBeginEndDates();

            var WeekEnd = resultPlaDate.Select(x => x.Week_End_Date).FirstOrDefault();

            var MinDate = DateTime.Now;
            var NewDate = DateTime.Now;
            var offset = 0;

            var flag = true;
            if (search.Week == LOVConstants.WOMWeeks.CurrentPriorWeek)
            {
                MinDate = WeekEnd.AddDays(-250);
                NewDate = WeekEnd;
            }
            else if (search.Week == LOVConstants.WOMWeeks.PlanWeekOne)
            {
                MinDate = WeekEnd.AddDays(1);
                NewDate = WeekEnd.AddDays(7);
            }
            else if (search.Week == LOVConstants.WOMWeeks.PlanWeekTwo)
            {
                MinDate = WeekEnd.AddDays(7 * 1 + 1);
                NewDate = WeekEnd.AddDays(7 * 2);
            }
            else if (search.Week == LOVConstants.WOMWeeks.PlanWeekThree)
            {
                MinDate = WeekEnd.AddDays(7 * 2 + 1);
                NewDate = WeekEnd.AddDays(7 * 3);
            }
            else
            {
                //TBD
                flag = false;

                if (!string.IsNullOrEmpty(search.Week))
                {
                    MinDate = Convert.ToDateTime(search.Week);
                    NewDate = MinDate;
                }
            }

            if (!String.IsNullOrEmpty(search.MoreWeeks))
            {
                offset = int.Parse(search.MoreWeeks);
                NewDate = NewDate.AddDays(7 * offset);
            }



            var DateField = "";
            if (search.DueDate == LOVConstants.WOMDueDates.DC)
            {
                DateField = " a.curr_due_date ";
            }
            else if (search.DueDate == LOVConstants.WOMDueDates.Sew)
            {
                DateField = " a.sew_start_date ";
            }
            else if (search.DueDate == LOVConstants.WOMDueDates.Cut)
            {
                DateField = " a.cut_start_date ";
            }
            else if (search.DueDate == LOVConstants.WOMDueDates.BD)
            {
                DateField = " a.txt_start_date ";
            }
            else
            {
                DateField = " a.earliest_start ";
            }

            if (flag || string.IsNullOrEmpty(search.GroupId))
            {
                query.Append(" and trunc( " + DateField + ")  >= to_date('" + MinDate.ToString(LOVConstants.DateFormatOracle) + "','YYYYMMDD') and trunc(" + DateField + ")  <= to_date('" + NewDate.ToString(LOVConstants.DateFormatOracle) + "','YYYYMMDD')   ");
            }

            if (search.StyleType == LOVConstants.StyleType.SellingStyle)
            {
                query.Append(" and A.SELLING_size_cd = b1.size_cd  ");
            }
            else
            {
                query.Append(" and A.size_cd = b1.size_cd  ");
            }
            query.Append(" and a.production_status in ('L','P','R')  and a.order_status_cd <> '3'  ");

            query.Append(" order by a.multi_sku,nvl(a.rule_number,999),a.PRIORITY,a.earliest_start,a.style_cd, a.color_cd, a.attribute_cd, a.size_cd, a.demand_loc");

            Debug.WriteLine(query.ToString());

            //  var date = DateTime.Now;
            IDataReader reader = ExecuteReader(query.ToString());

            var result = (new DbHelper()).ReadData<WOMDetail>(reader);
            // var ts = DateTime.Now - date;
            // Debug.WriteLine("Generic Minuts -" + ts.Minutes + " Sec-" + ts.Seconds);

            result = MeetsFilterGrid(result, search);
            return result;

        }

        public List<WOMDetail> GetSuperOrderDetail(string SuperOrder)
        {
            StringBuilder query = new StringBuilder(string.Empty);

            query.Append(" select ORDER_VERSION OrderVersion,ORDER_LABEL OrderLabel, SUPER_ORDER SuperOrder,PARENT_ORDER ParentOrder,STYLE_CD Style, COLOR_CD Color,ATTRIBUTE_CD Attribute,SIZE_CD \"Size\",DEMAND_LOC DcLoc,MFG_PLANT MFGPlant ,PIPELINE_CATEGORY_CD PipelineCategoryCode, CURR_DUE_DATE  CurrDueDate,CURR_ORDER_QTY Qty, MATL_TYPE_CD MatlTypeCode, MFG_PATH_ID MfgPathId, case    when NVL(PULL_FROM_STOCK_IND,'N')='N' then 0   else 1   end PFS,BILL_OF_MTRLS_ID BillOfMtrlsId, ROUTING_ID RoutingId, SPREAD_TYPE_CD SpreadTypeCode, SPREAD_COMP_CD SpreadCompCode, DYE_CD DyeCode,DYE_SHADE_CD DyeShadeCode,cast (CYLINDER_SIZE as varchar2(100)) CylinderSizes, NVL(STD_USAGE,0) StdUsuage,NVL(STD_LOSS,0) StdLoss, NVL(USAGE,0) Usage,NVL(WASTE_FACTOR,0) WasteFactor, UNIT_OF_MEASURE UOM,NVL(SCRAP_FACTOR,0) ScrapFactor, CUTTING_ALT AltId,CONDITIONED_WIDTH ConditionedWidth,FINISHED_WIDTH FinishedWidth,MACHINE_CUT MachineCut   " +

                " FROM iss_prod_order_detAIL WHERE ORDER_VERSION = " + LOVConstants.GlobalOrderVersion + " and super_order = '" + SuperOrder + "'");



            Debug.WriteLine(query.ToString());


            IDataReader reader = ExecuteReader(query.ToString());

            var result = (new DbHelper()).ReadData<WOMDetail>(reader);


            return result;

        }



        public List<WOMDetail> getProductionOrders(string superOrder)
        {
            string query = " select style_cd \"Style\", color_cd \"Color\", attribute_cd \"Attribute\", size_cd \"Size\", demand_loc \"DcLoc\", trunc(curr_due_date) \"CurrDueDate\" ,ceil(curr_order_qty/12) \"Qty\" ,CAPACITY_ALLOC \"WorkCenter\" from iss_prod_order_detail where order_version = " + LOVConstants.GlobalOrderVersion + " and super_order = '" + Val(superOrder) + "' and parent_order is not null and matl_type_cd = '00' ";
            IDataReader reader = ExecuteReader(query);

            var result = (new DbHelper()).ReadData<WOMDetail>(reader);
            return result;

        }

        //Asif 10/9/2018 To Add Selling Sku popup while clicking in Selling Style in WOM screen

        public List<WOMDetail> getSellingSku(string superOrder)
        {
            string query = " select d.selling_style_cd \"SellingStyle\", d.selling_color_cd \"Color\", d.selling_attribute_cd \"Attribute\", d.selling_size_cd \"Size\", s.size_short_desc \"SizeShortDes\",  d.demand_loc \"DcLoc\", trunc(d.curr_due_date) \"CurrDueDate\" ,ceil(d.curr_order_qty/12) \"Qty\" ,d.CAPACITY_ALLOC \"WorkCenter\" from  iss_prod_order_detail d, item_size s where order_version = " + LOVConstants.GlobalOrderVersion + " and super_order = '" + Val(superOrder) + "' and parent_order is null and matl_type_cd = '00' and d.selling_size_cd = s.size_cd  ";
            IDataReader reader = ExecuteReader(query);

            var result = (new DbHelper()).ReadData<WOMDetail>(reader);
            return result;

        }

        public List<WOMDetail> getProductionOrdersLbs(string superOrder)
        {
            string query = " select  style_cd \"Style\", color_cd \"Color\", attribute_cd \"Attribute\", size_cd \"Size\", mfg_plant \"DcLoc\", decode(matl_type_cd,'F',curr_order_qty,ceil(curr_order_qty/12)) \"Qty\", order_label \"OrderId\" from iss_prod_order_detail where order_version = 1 and super_order = '" + Val(superOrder) + "' and parent_order is not null and matl_type_cd in ( '00','F')   union all select COMP_STYLE,comp_color,comp_attribute_cd, COMP_SIZE_CD,COMP_DEMAND_LOC,ALLOCATION_QTY, order_label from iss_prod_order_allocation a, style s  where order_version =  " + LOVConstants.GlobalOrderVersion + " and super_order = '" + Val(superOrder) + "' and a.comp_style = s.style_cd and s.matl_type_cd in ( 'G','Y')  order by \"OrderId\"  ";

            IDataReader reader = ExecuteReader(query);

            var result = (new DbHelper()).ReadData<WOMDetail>(reader);
            return result;

        }


        public List<WOMDetail> getFabricDetail(string superOrder)
        {
            string query = "select ORDER_VERSION \"OrderVersion\" ,ORDER_LABEL \"OrderId\",SUPER_ORDER \"SuperOrder\",PARENT_ORDER \"ParentOrder\" ,style_cd \"Style\", color_cd \"Color\", attribute_cd \"Attribute\", size_cd \"Size\",DEMAND_LOC  \"DcLoc\",MFG_PLANT \"SupplyPlant\",PIPELINE_CATEGORY_CD \"PipelineCategoryCode\" ,CURR_DUE_DATE \"CurrDueDate\" ,CURR_ORDER_QTY \"Qty\",MATL_TYPE_CD \"MatlTypeCode\",MFG_PATH_ID \"MfgPathId\", case    when NVL(PULL_FROM_STOCK_IND,'N')='N' then 0   else 1 end  \"PFS\" ,BILL_OF_MTRLS_ID BillOfMtrlsId,ROUTING_ID RoutingId, SPREAD_TYPE_CD \"SpreadTypeCode\", SPREAD_COMP_CD \"SpreadCompCode\",DYE_CD DyeCode,DYE_SHADE_CD DyeShadeCode, CYLINDER_SIZE CylinderSizes, NVL(STD_USAGE,0) StdUsuage, NVL(STD_LOSS,0) StdLoss,NVL(USAGE,0) Usage,NVL(WASTE_FACTOR,0) WasteFactor, UNIT_OF_MEASURE UOM, NVL(SCRAP_FACTOR,0) ScrapFactor, CUTTING_ALT CuttingAlt, CONDITIONED_WIDTH ConditionedWidth, FINISHED_WIDTH FinishedWidth, MACHINE_CUT MachineCut FROM iss_prod_order_detAIL WHERE order_version =  " + LOVConstants.GlobalOrderVersion + " and super_order = '" + Val(superOrder) + "'";

            IDataReader reader = ExecuteReader(query);

            var result = (new DbHelper()).ReadData<WOMDetail>(reader);
            return result;

        }

        public List<WOMPipeline> getPipelineDates(string superOrder)
        {
            //string query = "select  o.PIPELINE_CATEGORY_CD CategoryCode, ACTIVITY_LOC Plant, START_DATE StartDate, COMPLETE_DATE EndDate,f.fiscal_week StartWeek from iss_prod_order_activity a, iss_prod_order_detail o,fiscal_calendar f where o.order_version = " + LOVConstants.GlobalOrderVersion + " and o.super_order = '" + Val(superOrder) + "' and o.order_version = a.order_version and o.order_label = a.order_label and trunc(a.complete_date) = trunc(f.calendar_date) and (a.complete_date - a.start_date)  > 0 group by  o.PIPELINE_CATEGORY_CD,ACTIVITY_LOC,START_DATE,COMPLETE_DATE ,f.fiscal_week order by COMPLETE_DATE desc";

            // CA#249684-16

            //Champion Double Down Azalia Soriano 10/01/2019
            //query.Append("select  o.PIPELINE_CATEGORY_CD CategoryCode, ACTIVITY_LOC Plant, START_DATE StartDate, COMPLETE_DATE EndDate,f.fiscal_week StartWeek from iss_prod_order_activity a, iss_prod_order_detail o,fiscal_calendar f where o.order_version = " + LOVConstants.GlobalOrderVersion + " and o.super_order = '" + Val(superOrder) + "' and o.order_version = a.order_version and o.order_label = a.order_label and trunc(a.complete_date) = trunc(f.calendar_date) and (a.complete_date - a.start_date)  > 0 and o.PIPELINE_CATEGORY_CD !='DBF' and a.activity_cd not like 'LEAD%' ");
            //query.Append("union select  o.PIPELINE_CATEGORY_CD CategoryCode, ACTIVITY_LOC Plant, START_DATE StartDate, COMPLETE_DATE EndDate,f.fiscal_week StartWeek from iss_prod_order_activity a, iss_prod_order_detail o,fiscal_calendar f where o.order_version = " + LOVConstants.GlobalOrderVersion + " and o.super_order = '" + Val(superOrder) + "' and o.order_version = a.order_version and o.order_label = a.order_label and trunc(a.complete_date) = trunc(f.calendar_date) and (a.complete_date - a.start_date)  > 0 and o.PIPELINE_CATEGORY_CD ='DBF' and o.parent_order in (select order_label From iss_prod_order_detail d where d.order_version = 1 and d.order_label = o.parent_order and d.pipeline_category_cd = 'CUT') and a.activity_cd not like 'LEAD%' group by o.PIPELINE_CATEGORY_CD,ACTIVITY_LOC,START_DATE,COMPLETE_DATE ,f.fiscal_week order by EndDate desc");

            StringBuilder query = new StringBuilder("");
            query.Append("select  a.activity_cd CategoryCode, DECODE(c.capacity_group, null,'',c.capacity_group||'~'||c.alloc_group) Info, ACTIVITY_LOC Plant, START_DATE StartDate, COMPLETE_DATE EndDate,f.fiscal_week StartWeek from iss_prod_order_activity a, iss_prod_order_detail o,fiscal_calendar f , iss_prod_order_capacity c where o.order_version = " + LOVConstants.GlobalOrderVersion + " and o.super_order = '" + Val(superOrder) + "' and o.order_version = a.order_version and o.order_label = a.order_label and trunc(a.complete_date) = trunc(f.calendar_date) and (a.complete_date - a.start_date)  > 0 and o.PIPELINE_CATEGORY_CD !='DBF' and a.activity_cd not like 'LEAD%' and a.order_version = c.order_version(+) and a.order_label = c.order_label(+) and a.activity_cd = c.activity_cd(+) and a.activity_loc = c.work_center_plant(+) ");
            query.Append("union select  a.activity_cd CategoryCode,al.comp_style || '~' || al.comp_color || '~' || al.comp_attribute_cd || '~' || al.comp_size_cd || '~' || al.comp_revision_no Info, ACTIVITY_LOC Plant, START_DATE StartDate, COMPLETE_DATE EndDate,f.fiscal_week StartWeek from iss_prod_order_activity a, iss_prod_order_detail o,fiscal_calendar f, iss_prod_order_allocation al where o.order_version = " + LOVConstants.GlobalOrderVersion + " and o.super_order = '" + Val(superOrder) + "' and o.order_version = a.order_version and o.order_label = a.order_label and trunc(a.complete_date) = trunc(f.calendar_date) and a.activity_cd in ('PUL') and a.order_version = al.order_version and a.order_label = al.order_label and a.activity_cd = al.activity_cd ");
            query.Append("union select  o.PIPELINE_CATEGORY_CD CategoryCode, '' Info,ACTIVITY_LOC Plant, START_DATE StartDate, COMPLETE_DATE EndDate,f.fiscal_week StartWeek from iss_prod_order_activity a, iss_prod_order_detail o,fiscal_calendar f where o.order_version = " + LOVConstants.GlobalOrderVersion + " and o.super_order = '" + Val(superOrder) + "' and o.order_version = a.order_version and o.order_label = a.order_label and trunc(a.complete_date) = trunc(f.calendar_date) and (a.complete_date - a.start_date)  > 0 and o.PIPELINE_CATEGORY_CD ='DBF' and o.parent_order in (select order_label From iss_prod_order_detail d where d.order_version = 1 and d.order_label = o.parent_order and d.pipeline_category_cd = 'CUT') and a.activity_cd not like 'LEAD%' group by o.PIPELINE_CATEGORY_CD,ACTIVITY_LOC,START_DATE,COMPLETE_DATE ,f.fiscal_week order by EndDate desc");


            IDataReader reader = ExecuteReader(query.ToString());

            var result = (new DbHelper()).ReadData<WOMPipeline>(reader);
            return result;

        }


        public List<WOMDetail> getAlternateId(SKU sku)
        {
            string query = "SELECT distinct BILL_OF_MTRLS_ID AltId FROM BILL_OF_MTRLS A WHERE A.PARENT_STYLE = '" + Val(sku.Style) + "' AND A.PARENT_COLOR = '" + Val(sku.Color) + "' AND A.PARENT_Size = '" + Val(sku.Size) + "' AND A.PARENT_ATTRIBUTE = '" + Val(sku.Attribute) + "'  and a.bill_of_mtrls_id is not null";


            IDataReader reader = ExecuteReader(query);

            var result = (new DbHelper()).ReadData<WOMDetail>(reader);
            return result;

        }

        public List<WorkOrderDetail> GetGarmentSKU(String SuperOrder)
        {
            string query = "SELECT style_cd iStyle,color_cd iColor,attribute_cd iAttribute,size_cd iSize,mfg_path_id iMfgPathId from iss_prod_order_detail a where  a.super_order = '" + Val(SuperOrder) + "' and a.matl_type_cd = '" + LOVConstants.MATL_TYPE_CD.Garment + "' and a.PIPELINE_CATEGORY_CD = '" + LOVConstants.PipeLineCategoryCD + "' and rownum = 1";

            IDataReader reader = ExecuteReader(query);

            var result = (new DbHelper()).ReadData<WorkOrderDetail>(reader);
            return result;

        }

        public List<WOMDetail> PopulateCutPathTxtPath(string SuperOrder, string DyeCode, string CutPath)
        {
            var grmnt = GetGarmentSKU(SuperOrder);
            if (grmnt.Count > 0)
            {
                grmnt[0].DyeCode = DyeCode;
                grmnt[0].CutPath = CutPath;
                return PopulateCutPathTxtPath(grmnt[0]);
            }

            return new List<WOMDetail>();
        }
        /// <summary>
        /// Returns Priority and Source_plant
        /// </summary>
        /// <param name="Wod"></param>
        /// <returns></returns>
        public List<WOMDetail> PopulateCutPathTxtPath(WorkOrderDetail Wod)
        {
            var query = new StringBuilder();
            var PrimePath = String.Empty;


            if (Wod.DyeCode == LOVConstants.DyeShadeCode.CutPath)
            { //Cut path

                //'System.Data.OracleClient.OracleDataReader' to type 'Oracle.DataAccess.Client.OracleDataReader'.
                //"ORESULTSET"

                query.Append(" SELECT PRIME_MFG_LOCATION  MfgPathId from mfg_path a where " + "a.style_cd = '" + Val(Wod.iStyle) + "' and a.color_cd = '" + Val(Wod.iColor) + "' and a.attribute_cd = '" + Val(Wod.iAttribute) + "' and a.size_cd = '" + Val(Wod.iSize) + "' and a.mfg_path_id = '" + Val(Wod.iMFGPathId) + "' ");

                IDataReader reader = ExecuteReader(query.ToString());
                var result = (new DbHelper()).ReadData<WorkOrderDetail>(reader);
                if (result.Count > 0) PrimePath = result[0].MfgPathId;

                List<OracleParameter> parameters = new List<OracleParameter>(){
                new OracleParameter()
                {
                    ParameterName = "IPARENTSTYLE",
                    Value = Val(Wod.iStyle)
                },
                 new OracleParameter()
                {
                    ParameterName = "IPARENTCOLOR",
                    Value = Val(Wod.iColor)
                },
                 new OracleParameter()
                {
                    ParameterName = "IPARENTATTRIBUTE",
                    Value = Val(Wod.iAttribute)
                },                   
                 new OracleParameter()
                {
                    ParameterName = "IPARENTSIZE",
                    Value = Val(Wod.iSize) 
                },
                 new OracleParameter()
                {
                    ParameterName = "IMFGPATHID",
                    Value =Val(Wod.iMFGPathId) 
                },
                 new OracleParameter()
                {
                    ParameterName = "IDESTPLANT",
                    Value = PrimePath
                },
                 new OracleParameter()
                {
                    ParameterName = "IDYECD",
                    Value =  Val(Wod.DyeCode)
                }
                 };

                IDataReader reader2 = ExecuteSPReader("OPRSQL.ISS_TEXTILES.RS_GET_SOURCING_PRIORITIES", parameters.ToArray());
                var result2 = (new DbHelper()).ReadData<WOMDetail>(reader2);
                return result2;
            }
            else
            {
                PrimePath = Wod.CutPath;
                //p.dye_shade_cd = 'T' 
                //==============================
                query.Clear();
                query.Append("select s.source_plant ,min(s.priority) as Priority   from iss_garment_resource g   ,iss_planning_param p ,iss_sourcing_priority s  where   p.mfg_style_cd = '" + Val(Wod.iStyle) + "'  and  p.color_cd = '" + Val(Wod.iColor) + "' and p.attribute_cd ='" + Val(Wod.iAttribute) + "'   and  p.dye_shade_cd = 'T'  and p.plant_sku_priority =  s.plant_sku_priority   and s.destination_plant  ='" + Val(PrimePath) + "'   and g.style_cd = p.mfg_style_cd and g.color_cd = p.color_cd and g.attribute_cd =  p.attribute_cd and g.size_cd = '" + Val(Wod.iSize) + "' and g.mfg_path_id = '" + Val(Wod.iMFGPathId) + "' and g.dye_shade_cd = s.dye_shade_cd  and rownum<10  group by s.source_plant order by min(s.priority)");

                var reader3 = ExecuteReader(query.ToString());
                var result3 = (new DbHelper()).ReadData<WOMDetail>(reader3);
                return result3;

            }

        }

        public bool ValidatePlant(String Plant)
        {
            string query = "Select 1 As  Temp from Plant where plant_cd = '" + Val(Plant) + "' and Finishing_Ind = 'Y' ";
            IDataReader reader = ExecuteReader(query);
            var result = (new DbHelper()).ReadData<WorkOrderDetail>(reader);
            return result.Count > 0;
        }
        public int ValidateAltBefore(String SuperOrder)
        {
            string query = "SELECT count (Distinct(a.Cutting_Alt)) from ISS_PROD_ORDER_DETAIL a WHERE a.order_version = 1 and a.super_Order = '" + Val(SuperOrder) + "'   ";
            return (int)(decimal)ExecuteScalar(query);
        }
        public int ValidateAlt(String SuperOrder)
        {
            string query = "SELECT count (a.CYLINDER_SIZE) from ISS_PROD_ORDER_DETAIL a WHERE a.order_version = 1 and a.super_Order = '" + Val(SuperOrder) + "'  AND a.CYLINDER_SIZE = 0 and a.PIPELINE_CATEGORY_CD = 'DBF' and a.spread_type_cd is not null  ";

            return (int)(decimal)ExecuteScalar(query);
        }

        public bool UpdateWOMGroupedOrders(List<WOMDetail> wom)
        {
            // SELECT oprsql.iss_prod_order_validate.verify(28,'SUPER_ORDER|ORDER_VERSION|DOZENS_ONLY_IND|CREATE_BD_IND|PRIORITY|SPREAD_TYPE_CD|SEW_PATH|CUT_PATH|TXT_PATH|MACHINE|CURR_ORDER_QTY|CURR_ORDER_TOTAL_QTY|CUT_MASTER|DEMAND_LOC|DOZENS_ONLY_IND|CREATE_BD_IND|GARMENT_STYLE|DISCRETE_IND|ROW_NUMBER|CURR_FIN_LBS|STYLE_CD|SIZE_CD|MAKE_OR_BUY_CD|ATTRIBUTE_CD|COLOR_CD|GARMENT_STYLE|SELLING_STYLE_CD|MFG_REVISION_NO|','646327364|1|N|Y|1387241||KG|23|5M|N/A|1332|1332||KG|N|Y|MHG9D7|Y|10|26.115|PH5100|43|M|------|VB7|MHG9D7|MHHH51|0') from dual
            bool Stat = false;
            String Msg = String.Empty;
            //data.Style + data.Color + data.Attribute + data.MfgPathId
            var groups = wom.GroupBy(g => new
            {
               // woDet.CutPath = !String.IsNullOrEmpty(woDet.CutPath) ? woDet.CutPath.ToUpper() : woDet.CutPath;
                Style = g.Style,
                Color = g.Color,
                Attr = g.Attribute,
                MFGPath = g.MfgPathId,
                Revision = g.Revision,
                TxtPath = !String.IsNullOrEmpty(g.TxtPath) ? g.TxtPath.ToUpper() : g.TxtPath,
                CutPath = !String.IsNullOrEmpty(g.CutPath) ? g.CutPath.ToUpper() : g.CutPath,
                AltId = g.AltId,
                CurDate = g.CurrDueDateStr
            });



            if (groups.Count() > 1)
            {
                var invalid = groups.Skip(1).FirstOrDefault();
                invalid.ToList().ForEach(item =>
                    {
                        Stat = true;
                        item.ErrorMessage = " Multi Sku order must match by Selling sku, revision, sew cut txt plants alternate and due date.";
                        Msg = "Failed to group orders.";
                    });

            }
            if (!Stat)
            {
                var size = wom.Select(e => e.Size).ToList();
                if (size.Count != size.Distinct().Count())
                {
                    var duplicate = string.Join(", ", size.Except(size.Distinct()).ToArray());
                    Stat = true;
                    Msg = "Duplicate Size " + duplicate;
                }
            }
            if (Stat)
            {
                wom.ForEach(e =>
                {
                    e.ErrorStatus = Stat;
                    e.ErrorMessage = (String.IsNullOrEmpty(e.ErrorMessage) ? Msg : e.ErrorMessage);
                });
                return false;
            }

            decimal ErrId;
            bool valid = true;
            wom.ForEach(e =>
            {
                if (valid)
                {
                    valid = ValidateDetailBeforeSave(e, out Msg, out ErrId);
                    if (!valid)
                    {
                        e.ErrorStatus = true;
                        e.ErrorMessage = Msg;
                    }
                }
            });

            if (!valid)
            {
                return false;
            }
            using (TransactionScope scope = new TransactionScope())
            {
                BeginTransaction();
                try
                {
                    WorkOrderRepository wor = new WorkOrderRepository(trans);
                    for (int i = 0; i < wom.Count; i++)
                    {

                        var item = wom[i];
                        if (item.IsGrouped)
                        {
                            Stat = WOMOrderInsertGroupId(item.OrderVersion, item.SuperOrder, item.GroupId);
                        }

                        if (Stat && item.IsPFSChange)
                        {
                            Stat = WOMOrderUpdatePFS(item);
                        }
                        if (item.IsEdited && item.IsFieldChange)
                            Stat = WOMOrderUpdate(item);

                        if (Stat && item.NoteInd == LOVConstants.Yes && !String.IsNullOrWhiteSpace(item.Note))
                        {
                            Stat = wor.AddNote(item.SuperOrder, item.Note);
                        }
                        if (!Stat)
                        {
                            item.ErrorMessage = "Failed to update order details.";
                            item.ErrorStatus = false;
                            RollbackTransaction();
                            return false;
                        }
                    }
                    CommitTransaction();
                    return true;
                }
                catch (OracleException ox)
                {
                    Log("UpdateWOMGroupedOrders", ox);
                    RollbackTransaction();
                }
            }
            return false;
        }

        /// <summary>
        /// No SKU change PFS && Ungrouped
        /// </summary>
        /// <param name="wom"></param>
        /// <returns></returns>
        public bool UpdateWOMOrder(WOMDetail wom)
        {
            // SELECT oprsql.iss_prod_order_validate.verify(28,'SUPER_ORDER|ORDER_VERSION|DOZENS_ONLY_IND|CREATE_BD_IND|PRIORITY|SPREAD_TYPE_CD|SEW_PATH|CUT_PATH|TXT_PATH|MACHINE|CURR_ORDER_QTY|CURR_ORDER_TOTAL_QTY|CUT_MASTER|DEMAND_LOC|DOZENS_ONLY_IND|CREATE_BD_IND|GARMENT_STYLE|DISCRETE_IND|ROW_NUMBER|CURR_FIN_LBS|STYLE_CD|SIZE_CD|MAKE_OR_BUY_CD|ATTRIBUTE_CD|COLOR_CD|GARMENT_STYLE|SELLING_STYLE_CD|MFG_REVISION_NO|','646327364|1|N|Y|1387241||KG|23|5M|N/A|1332|1332||KG|N|Y|MHG9D7|Y|10|26.115|PH5100|43|M|------|VB7|MHG9D7|MHHH51|0') from dual

            String Msg;
            decimal ErrId;
            var Stat = ValidateDetailBeforeSave(wom, out Msg, out ErrId);
            if (!Stat)
            {
                wom.ErrorStatus = true;
                wom.ErrorMessage = Msg;
                return false;
            }
            using (TransactionScope scope = new TransactionScope())
            {
                BeginTransaction();
                try
                {
                    bool stat = true;

                    if (wom.IsUngrouped)
                    {
                        stat = WOMOrderDeleteGroupId(wom);
                    }
                    if (stat && wom.IsPFSChange)
                    {
                        stat = WOMOrderUpdatePFS(wom);
                    }
                    if (wom.IsEdited && wom.IsFieldChange)
                        stat = WOMOrderUpdate(wom);

                    if (Stat && wom.NoteInd == LOVConstants.Yes && !String.IsNullOrWhiteSpace(wom.Note))
                    {
                        WorkOrderRepository wor = new WorkOrderRepository(trans);
                        Stat = wor.AddNote(wom.SuperOrder, wom.Note);
                    }
                    if (stat)
                    {
                        CommitTransaction();
                        return true;
                    }
                    else
                    {
                        wom.ErrorMessage = "Failed to update order details.";
                        wom.ErrorStatus = true;
                        RollbackTransaction();
                    }
                }
                catch (OracleException ox)
                {
                    Log(ox);
                    wom.ErrorMessage = ox.Message;
                    wom.ErrorStatus = true;
                    RollbackTransaction();
                }
            }
            return false;
        }

        private bool WOMOrderUpdatePFS(WOMDetail wom)
        {
            bool stat = true;
            if (wom.PFSList != null)
            {

                var items = wom.PFSList.Where(e => e.IsEdited && !e.IsHide).ToList();
                for (var i = 0; i < items.Count; i++)
                {
                    if (stat)
                    {
                        var item = items[i];
                        if (item.IsMerged)
                        {
                            wom.PFSList.Where(e => e.Style == item.Style && !e.IsMerged).ToList().ForEach(PItem =>
                                {
                                    if (stat)
                                    {
                                        PItem.PFSInd = item.PFSInd;
                                        stat = WOMOrderUpdatePFSItem(PItem);
                                    }
                                });
                        }
                        else
                        {
                            stat = WOMOrderUpdatePFSItem(item);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (!stat)
            {
                wom.ErrorStatus = true;
                wom.ErrorMessage = "Failed to update PFS detail.";
            }
            return stat;
        }

        private bool WOMOrderUpdatePFSItem(WOMDetail item)
        {
            //BEGIN OPRSQL.TM_ISS_PROD_ORDER_DETAIL.TABLE_UPDATE(3,'ORDER_LABEL|ORDER_VERSION|PULL_FROM_STOCK_IND','646180788|1|Y');END;
            String query = "BEGIN OPRSQL.TM_ISS_PROD_ORDER_DETAIL.TABLE_UPDATE(4,'ORDER_LABEL|ORDER_VERSION|PULL_FROM_STOCK_IND|USER_ID','" + item.OrderLabel + "|" + item.OrderVersion + "|" +
                ((item.PFSInd) ? LOVConstants.Yes : LOVConstants.No)
                + "|" + item.CreatedBy + "');END;";

            var result = (String)ExecuteScalar(query);

            return (result == null || result == "Y") ? true : false;
        }


        public bool WOMOrderInsertGroupId(decimal OrderVersion, String SuperOrder, String GroupId)
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append(" BEGIN OPRSQL.TM_ISS_PROD_ORDER_GROUP.TABLE_INSERT(4,'ORDER_VERSION|SUPER_ORDER|ISS_GROUP_TYPE_CD|ISS_GROUP_ID','"
                + OrderVersion
                + "|" + SuperOrder + "|" + LOVConstants.ISSGroupType.CutMaster + "|" + GroupId
                + "');END; ");

            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            var result = (String)ExecuteScalar(queryBuilder.ToString());

            return (result == null || result == "Y") ? true : false;

        }


        public bool WOMOrderDeleteGroupId(WOMDetail item)
        {
            //BEGIN OPRSQL.TM_ISS_PROD_ORDER_GROUP.TABLE_DELETE(3,'ORDER_VERSION|SUPER_ORDER|ISS_GROUP_TYPE_CD','1|E000578479|M|');END;
            String query = "BEGIN OPRSQL.TM_ISS_PROD_ORDER_GROUP.TABLE_DELETE(3,'SUPER_ORDER|ORDER_VERSION|ISS_GROUP_TYPE_CD','" + item.SuperOrder + "|" + item.OrderVersion + "|" + LOVConstants.ISSGroupType.CutMaster
                + "');END;";

            var result = (String)ExecuteScalar(query);

            return (result == null || result == "Y") ? true : false;
        }

        /// <summary>
        /// NON SKU Fields only
        /// </summary>
        /// <param name="wom"></param>
        /// <returns></returns>
        private bool WOMOrderUpdate(WOMDetail wom)
        {
            // CUT_MASTER|PRODUCTION_STATUS|ISS_ORDER_TYPE_CD|DOZENS_ONLY_IND|CREATE_BD_IND(Y/N)
            //BEGIN OPRSQL.ISS_PROD_ORDER_MAINT.UPDATE_ORDER(11,'SUPER_ORDER|ORDER_VERSION|MACHINE_TYPE_CD|CURR_ORDER_QTY|TOTAL_CURR_ORDER_QTY|PIPELINE_CATEGORY_CD|CURR_DUE_DATE|PACK_CD|CATEGORY_CD|EXPEDITE_PRIORITY|MAKE_OR_BUY_CD','646327364|1|R3|134664|134664|DC|20151121|3|62|60|M');END;

            // wom.Qty = wom.QtyDZ * LOVConstants.Dozen;
            // wom.TotatalCurrOrderQty = wom.TotalDozens * LOVConstants.Dozen;
            bool IsDateChange = false;
            if (wom.Cloned != null)
            {
            if (wom.CurrDueDate.Value.Date != wom.Cloned.CurrDueDate.Value.Date)
            {
                wom.PipelineCategoryCode = LOVConstants.PipelineActivity.DC;
                IsDateChange = true;
            }
            if (wom.StartDate.Value.Date != wom.Cloned.StartDate.Value.Date)
            {
                wom.PipelineCategoryCode = LOVConstants.PipelineActivity.DBF;
                IsDateChange = true;
                }
            }
            else
            {
                if (wom.CCurrDueDate.Value.Date != wom.CurrDueDate.Value.Date)
                {
                    wom.PipelineCategoryCode = LOVConstants.PipelineActivity.DC;
                    IsDateChange = true;
                }
                else if (wom.CStartDate.Value.Date != wom.StartDate.Value.Date)
                {
                    wom.PipelineCategoryCode = LOVConstants.PipelineActivity.DBF;
                    IsDateChange = true;
            }
            }

            var queryBuilder = new StringBuilder();
            queryBuilder.Append(" Begin OPRSQL.ISS_PROD_ORDER_MAINT.UPDATE_ORDER(" +
                ((IsDateChange) ? 19 : 17)
                + ",'SUPER_ORDER|ORDER_VERSION|MACHINE_TYPE_CD|CURR_ORDER_QTY|TOTAL_CURR_ORDER_QTY" +
                ((IsDateChange) ? "|PIPELINE_CATEGORY_CD|CURR_DUE_DATE" : string.Empty)
                + "|PACK_CD|CATEGORY_CD|EXPEDITE_PRIORITY|MAKE_OR_BUY_CD|CUT_MASTER|PRODUCTION_STATUS|ISS_ORDER_TYPE_CD|DOZENS_ONLY_IND|CREATE_BD_IND|USER_ID|DEMAND_DRIVER|DEMAND_SOURCE','" +

               // SUPER_ORDER|ORDER_VERSION|MACHINE_TYPE_CD|CURR_ORDER_QTY|TOTAL_CURR_ORDER_QTY
               Val(wom.SuperOrder) + "|" + wom.OrderVersion + "|" + Val(wom.MC) + "|" + (wom.Qty) + "|" + (wom.TotatalCurrOrderQty) +

               ((IsDateChange) ? (
                //PIPELINE_CATEGORY_CD*|CURR_DUE_DATE
                "|" + Val(wom.PipelineCategoryCode) + "|" +
                ((wom.PipelineCategoryCode == LOVConstants.PipelineActivity.DC) ?
                ((wom.CurrDueDate.HasValue) ? wom.CurrDueDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty) :
                ((wom.StartDate.HasValue) ? wom.StartDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty)
                )
                ) : string.Empty
               )

               //|PACK_CD|CATEGORY_CD|EXPEDITE_PRIORITY
               + "|" + Val(wom.PackCode) + "|" + Val(wom.CategoryCode) + "|" + (wom.ExpeditePriority.HasValue ? (wom.ExpeditePriority.Value + string.Empty) : string.Empty) +

                   //MAKE_OR_BUY_CD|CUT_MASTER|PRODUCTION_STATUS|ISS_ORDER_TYPE_CD|DOZENS_ONLY_IND|CREATE_BD_IND
              "|" + Val(wom.MakeOrBuy) + "|" + Val(wom.GroupId) + "|" + Val(wom.OrderStatus) + "|" + Val(wom.OrderType) + "|" + Val((wom.DozensOnly) ? LOVConstants.Yes : LOVConstants.No) + "|" + Val((wom.CreateBd) ? LOVConstants.Yes : LOVConstants.No) + "|" + Val(wom.CreatedBy) + "|" + Val(wom.DemandDriver) + "|" + Val(wom.DemandSource) + "');END; ");


            var result = (String)ExecuteScalar(queryBuilder.ToString());

            return (result == null || result == "Y") ? true : false;
        }

    }

}
