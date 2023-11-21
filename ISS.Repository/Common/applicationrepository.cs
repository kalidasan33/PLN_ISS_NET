using ISS.Common;
using ISS.Core.Model.Common;
using ISS.DAL;
using ISS.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ISS.Core.Model.Order;

namespace ISS.Repository.Common
{
    public class ApplicationRepository : RepositoryBase
    {
        public IList<SummaryFilterModal> GetAttributeList(SummaryFilterModal model)
        {
            StringBuilder attributeQuery = new StringBuilder();
            StringBuilder attributeQueryWhere = new StringBuilder(String.Empty);
           attributeQuery.Append (
            "select distinct attribute_cd \"Attribute\"  from avyx_demand_driver ");

           if (!string.IsNullOrWhiteSpace(model.Style))
               attributeQueryWhere.Append("  style = '" + Val(model.Style) + "' ");
           if (!string.IsNullOrWhiteSpace(model.Color))
           {
               if (!String.IsNullOrEmpty(attributeQueryWhere.ToString())) attributeQueryWhere.Append(" and ");
               attributeQueryWhere.Append("  color = '" + Val(model.Color) + "' ");
           }
           if (!String.IsNullOrEmpty(attributeQueryWhere.ToString()))
           {
               attributeQuery.Append(" where ").Append(attributeQueryWhere.ToString());
           }
            IDataReader reader = ExecuteReader(attributeQuery.ToString());
            var result = (new DbHelper()).ReadData<SummaryFilterModal>(reader);
            return result;
        }
        public IList<Planner> GetPlannerList()
        {
            string plannerQuery =
           "select distinct planner_cd \"PlannerCd\"  from avyx_summary_view where planner_cd is not null   order by 1";
          

            IDataReader reader = ExecuteReader(plannerQuery);
            var result = (new DbHelper()).ReadData<Planner>(reader);
            return result;
        }
        public IList<Planner> GetPlanningContactList()
        {
            string planQuery =
           "select planner_name || ' - ' || planner_cd \"PlannerContact\", planner_cd \"PlannerCode\"  from planner where e_mail_address is not null order by 1 ";

            IDataReader reader = ExecuteReader(planQuery);
            var result = (new DbHelper()).ReadData<Planner>(reader);
            return result;
        }
        public IList<Planner> GetBusinesUnitList()
        {
            string businessQuery =
           "select corp_business_unit \"BusinessUnit\" from corp_business_unit where active_cd = 'A' order by 1 ";

            IDataReader reader = ExecuteReader(businessQuery);
            var result = (new DbHelper()).ReadData<Planner>(reader);
            return result;
        }

        public IList<Planner> GetMFGPathList()
        {
            string MFGPathQuery =
           "select distinct plant_cd \"MFGPathId\" from plant";
            IDataReader reader = ExecuteReader(MFGPathQuery);
            var result = (new DbHelper()).ReadData<Planner>(reader);
            return result;
        }

        public IList<Planner> GetSeasonList(String BU)
        {
            string seaQuery =
           "select season_year||'^'||season_name \"SeasonCode\" , season_year||' '||season_name as \"SeasonName\"  from business_unit_season where corp_business_unit = '" + BU + "' order by 2 desc ";

            IDataReader reader = ExecuteReader(seaQuery);
            var result = (new DbHelper()).ReadData<Planner>(reader);
            result.ForEach(e =>
            {
                e.SeasonCode = (e.SeasonCode + "").Trim().ToUpper();
                e.SeasonName = (e.SeasonName + "").Trim().ToUpper();
            });
            return result;
        }
        public IList<Planner> GetSourceContactList()
        {
            string sourceQuery =
           "select src_contact_cd \"SourceContactCd\", src_contact_name \"SourceContactName\" from src_contact order by 2 ";

            IDataReader reader = ExecuteReader(sourceQuery);
            var result = (new DbHelper()).ReadData<Planner>(reader);
            return result;
        }
        public IList<Planner> GetReqApproverList()
        {
            string approverQuery =
           "select 'PLANR' \"PlannerCd\",'PLANNER' \"PlannerName\" from dual ";

            IDataReader reader = ExecuteReader(approverQuery);
            var result = (new DbHelper()).ReadData<Planner>(reader);
            return result;
        }

        public IList<Planner> GetProgramTypeList()
        {
            string prgmQuery =
           "select iss_program_type_cd \"ProgramCd\", iss_program_type_desc \"ProgramName\" from ISS_PROGRAM_TYPE order by 1,2 ";

            IDataReader reader = ExecuteReader(prgmQuery);
            var result = (new DbHelper()).ReadData<Planner>(reader);
            return result;
        }

        public IList<Planner> GetModeList()
        {
            string modeQuery =
           "select transp_mode_cd  \"ModeCd\", transp_mode_name  \"ModeName\" from transp_mode where active_cd='A' order by 1,2 ";

            IDataReader reader = ExecuteReader(modeQuery);
            var result = (new DbHelper()).ReadData<Planner>(reader);
            return result;
        }

        public string GetPlantWeek()
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append("SELECT oprsql.tm_iss_configuration.iss_plan_week from dual");


            IDataReader reader = ExecuteReader(queryBuilder.ToString());
            var result = (new DbHelper()).ReadData<PlanWeek>(reader);
            var planweek = result.Select(x => x.ISS_PLAN_WEEK).FirstOrDefault();
            if (planweek == null) throw new Exception("Plan week is not available. [oprsql.tm_iss_configuration.iss_plan_week]");
            DateTime dt = DateTime.ParseExact(planweek, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            return dt.ToString("MM/dd/yyyy");

        }

        public IList<PlanWeek> GetPlanBeginEndDates()
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append("SELECT WEEK_END_DATE  ,  WEEK_BEGIN_DATE, fiscal_week,fiscal_year  FROM iss_configuration c, fiscal_calendar f " +
                " where c.login_id = 'BGEN' and CONFIGURATION_TYPE='PWK' and f.calendar_date = trunc(c.update_date)");


            IDataReader reader = ExecuteReader(queryBuilder.ToString());
            var result = (new DbHelper()).ReadData<PlanWeek>(reader);
            if (result.Count == 0)
            {
                queryBuilder.Clear();
                queryBuilder.Append("SELECT WEEK_END_DATE, WEEK_BEGIN_DATE,fiscal_week FROM fiscal_calendar where calendar_date = trunc(sysdate)");
                reader = ExecuteReader(queryBuilder.ToString());
                result = (new DbHelper()).ReadData<PlanWeek>(reader);
            }
            return result;
        }

        public IList<WorkCenter> GetWorkCenterList(string capacityGroup, string planner)
        {
            string workCenterQuery = "";

            
            

            // US 56194 Change to select multiple capacity groups
            // Start anew

            workCenterQuery = "select distinct 'SEW' cap_group ,sew_alloc \"WorkCenter_Cd\" from iss_cap_alloc_xref x where sew_alloc is not null and exists (select 'x' from avyx_demand_driver a where  a.selling_style_cd = x.style_cd) ";
            workCenterQuery += " union select distinct 'CUT' cap_group ,cut_alloc \"WorkCenter_Cd\" from iss_cap_alloc_xref x where cut_alloc is not null and exists (select 'x' from avyx_demand_driver a where  a.selling_style_cd = x.style_cd) ";
            workCenterQuery += " union select distinct 'SRC' cap_group ,src_alloc \"WorkCenter_Cd\" from iss_cap_alloc_xref x where src_alloc is not null and exists (select 'x' from avyx_demand_driver a where  a.selling_style_cd = x.style_cd) ";      
            workCenterQuery += " union select distinct 'TEX' cap_group ,finish_alloc \"WorkCenter_Cd\" from iss_cap_alloc_xref x where finish_alloc is not null and exists (select 'x' from avyx_demand_driver a where  a.selling_style_cd = x.style_cd) ";
                
            
            workCenterQuery += " union select distinct atr_group_1 cap_group, atr_alloc_1  \"WorkCenter_Cd\" from iss_cap_alloc_xref x where atr_alloc_1 is not null and exists (select 'x' from avyx_demand_driver a where  a.selling_style_cd = x.style_cd) ";
            workCenterQuery += " union select distinct atr_group_2 cap_group, atr_alloc_2  \"WorkCenter_Cd\" from iss_cap_alloc_xref x where atr_alloc_2 is not null and exists (select 'x' from avyx_demand_driver a where  a.selling_style_cd = x.style_cd) ";
            workCenterQuery += " union select distinct atr_group_3 cap_group, atr_alloc_3  \"WorkCenter_Cd\" from iss_cap_alloc_xref x where atr_alloc_3 is not null and exists (select 'x' from avyx_demand_driver a where  a.selling_style_cd = x.style_cd) ";


            workCenterQuery = "with cap as (" + workCenterQuery + ") select  \"WorkCenter_Cd\" from cap";
            workCenterQuery += " where cap_group in (" + FormatInClause(capacityGroup.ToUpper().Trim()) + ")";
            workCenterQuery += " union select '[CATCHALL]' \"WorkCenter_Cd\" from dual order by 1";

            // NET CODE 
            workCenterQuery = " with data as (select sew_alloc,cut_alloc,src_alloc,FINISH_ALLOC, atr_group_1, atr_alloc_1,";
            workCenterQuery += " atr_group_2, atr_alloc_2,atr_group_3, atr_alloc_3 from iss_cap_alloc_xref x ";
            workCenterQuery += " where exists (select 'x' from avyx_demand_driver a where a.selling_style_cd = x.style_cd and rownum = 1)";
            workCenterQuery += " or  exists (select 'x' from external_avyx_demand_driver a where a.selling_style_cd = x.style_cd and rownum = 1))";
            workCenterQuery += " , cap as ( select distinct 'SEW' cap_group, sew_alloc \"WorkCenter_Cd\" from data union ";
            workCenterQuery += " select distinct 'CUT' cap_group, cut_alloc \"WorkCenter_Cd\"  from data union ";
            workCenterQuery += " select distinct 'SRC' cap_group, src_alloc \"WorkCenter_Cd\"  from data union ";
            workCenterQuery += " select distinct 'TEX' cap_group, FINISH_ALLOC \"WorkCenter_Cd\"  from data union ";
            workCenterQuery += " select distinct atr_group_1 cap_group, atr_alloc_1 \"WorkCenter_Cd\"  from data where atr_group_1 is not null union ";
            workCenterQuery += " select distinct atr_group_2 cap_group, atr_alloc_2 \"WorkCenter_Cd\"  from data where atr_group_2 is not null union ";
            workCenterQuery += " select distinct atr_group_3 cap_group, atr_alloc_3 \"WorkCenter_Cd\"  from data where atr_group_3 is not null ) ";
            
            workCenterQuery += " select  \"WorkCenter_Cd\" from cap";
            workCenterQuery += " where cap_group in (" + FormatInClause(capacityGroup.ToUpper().Trim()) + ")";
            workCenterQuery += " union select '[CATCHALL]' \"WorkCenter_Cd\" from dual order by 1";


            /*

            if (capacityGroup == "Sew")
            {
                workCenterQuery = "select distinct sew_alloc \"WorkCenter_Cd\" from avyx_summary_view where sew_alloc is not null ";
            }
            else if (capacityGroup == "Src")
            {
                workCenterQuery = "select distinct src_alloc \"WorkCenter_Cd\" from avyx_summary_view where src_alloc is not null ";
            }
            else if (capacityGroup == "Cut")
            {
                workCenterQuery = "select distinct cut_alloc \"WorkCenter_Cd\" from avyx_summary_view where cut_alloc is not null ";
            }
//enhancement Asif
            else
            {
                workCenterQuery = "select distinct finish_alloc  \"WorkCenter_Cd\" from avyx_summary_view where finish_alloc  is not null ";
            }

            
            */

            IDataReader reader = ExecuteReader(workCenterQuery);
            var result = (new DbHelper()).ReadData<WorkCenter>(reader);
            return result;
        }

        public IList<WorkCenter> GetAttributedWorkCenterList(string capacityGroup, string planner)
        {
            
            var query = new StringBuilder();
            
                query.Append ("select distinct capacity_alloc \"WorkCenter_Cd\" from avyx_summary_view where cut_alloc is not null ");
            

            if ((!string.IsNullOrEmpty(planner)))
            {
                query.Append(" and planner_cd in (" + FormatInClause(planner) + ")");
                
            }

            query.Append(" order by 1");
           // select distinct capacity_alloc "WorkCenter_Cd" from avyx_summary_view where cut_alloc is not null  order by 1
            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<WorkCenter>(reader);
            return result;
        }
        public IList<Planner> GetPlannerNameAndCode()
        {
            string planQuery =
           "select planner_cd || ' - ' || planner_name \"PlannerCd\", planner_cd   \"PlannerCode\" from planner order by 1 ";

            IDataReader reader = ExecuteReader(planQuery);
            var result = (new DbHelper()).ReadData<Planner>(reader);
            return result;
        }
        public IList<PlantInfo> GetPlantList(string capcityGroup)
        {
            string plantQuery = "select distinct PLANT_CD from iss_cap_summary_view where capacity_group in (" + FormatInClause(capcityGroup) + ") and plant_cd is not null order by plant_cd";
            IDataReader reader = ExecuteReader(plantQuery);
            var result = (new DbHelper()).ReadData<PlantInfo>(reader);
            return result;
        }

        public IList<WorkCenter> GetAllocationWorkCenterList(string capcityGroup, string plant)
        {
            

            StringBuilder workCenterQuery = new StringBuilder();
            workCenterQuery.Append("select distinct CAPACITY_ALLOC \"WorkCenter_Cd\" from iss_cap_summary_view  where capacity_group in (" + FormatInClause(capcityGroup) + ")");
            
            workCenterQuery.Append(" and plant_cd in (" + FormatInClause(plant) + ")");
           
            workCenterQuery.Append(" and capacity_alloc is not null order by capacity_alloc");

            IDataReader reader = ExecuteReader(workCenterQuery.ToString());
            var result = (new DbHelper()).ReadData<WorkCenter>(reader);
            return result;
        }


        public IList<FabricInfo> GetTextileGroup(String BusinessUnit)
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append("select distinct r.FABRIC_GROUP \"FabricGroup\" from iss_style_fabric_xref r, style s, corp_division c");
            queryBuilder.Append(" where r.style_cd = s.Style_cd  and s.corp_division_cd = c.corp_division_cd");
            if (!String.IsNullOrEmpty(BusinessUnit))
            {

                queryBuilder.Append(" and c.corp_business_unit in (" + FormatInClause(BusinessUnit) + ")");
            }
            queryBuilder.Append(" and r.fabric_group <> 'UNK' order by r.FABRIC_GROUP ");
            var reader = ExecuteReader(queryBuilder.ToString());
            return (new DbHelper()).ReadData<FabricInfo>(reader);
        }

        public IList<PlanWeek> GetPlanWeekYear()
        {
            PlanWeek planDate = GetPlanDate().FirstOrDefault();
            var queryBuilder = new StringBuilder("SELECT distinct f.fiscal_week \"Fiscal_Week\", f.fiscal_year \"Fiscal_Year\" FROM fiscal_calendar f");
            queryBuilder.Append(" where f.calendar_date between (select min(calendar_date) calendar_date from fiscal_calendar where fiscal_week = " + planDate.Fiscal_Week + "and fiscal_year = " + planDate.Fiscal_Year + ") and ");
            queryBuilder.Append("(select  /*+ INDEX(d ISS_PROD_ORDER_DETAIL)  */ max(d.curr_due_date - d.PLANNING_LEADTIME) calendar_date from iss_prod_order_detail d  where d.order_version = " + LOVConstants.GlobalOrderVersion + " and d.matl_type_cd = 'F'");
            queryBuilder.Append("and exists (select 'x' from iss_prod_order o where  d.order_version = o.order_version and d.super_order = o.super_order and o.production_status = 'P' and rownum =1 )  ) order by f.fiscal_year ,f.fiscal_week");
            var reader = ExecuteReader(queryBuilder.ToString());
            return (new DbHelper()).ReadData<PlanWeek>(reader);
        }

  
   
  

        public IList<PlanWeek> GetPlanWeekYearBeginDate()
        {
            var queryBuilderDate = new StringBuilder();
            queryBuilderDate.Append("SELECT oprsql.tm_iss_configuration.iss_plan_week from dual");


            var pDate = ExecuteScalar(queryBuilderDate.ToString());
            
            //var plandate = GetPlantWeek();
            //DateTime dt = DateTime.Parse(plandate);
            var queryBuilder = new StringBuilder("SELECT  FISCAL_WEEK \"Fiscal_Week\", FISCAL_YEAR \"Fiscal_Year\", WEEK_BEGIN_DATE \"Week_Begin_Date\" FROM FISCAL_CALENDAR ");
            queryBuilder.Append(" where calendar_date  = to_date('" + pDate + "','YYYYMMDD')  ");
            var reader = ExecuteReader(queryBuilder.ToString());
            return (new DbHelper()).ReadData<PlanWeek>(reader);
        } 

        public IList<PlanWeek> GetPlanDate()
        {
            string query = "SELECT oprsql.tm_iss_configuration.iss_plan_week from dual";            
            string pDate = Convert.ToString(ExecuteScalar(query));
            StringBuilder queryBuilder = new StringBuilder("SELECT  FISCAL_WEEK \"Fiscal_Week\", FISCAL_YEAR \"Fiscal_Year\", WEEK_BEGIN_DATE \"Week_Begin_Date\" FROM FISCAL_CALENDAR where calendar_date  = ");
            if (!string.IsNullOrWhiteSpace(pDate))
            {
                queryBuilder.Append(" trunc(to_date('" + pDate + "','YYYYMMDD'))");
            }
            else
            {
                queryBuilder.Append(" trunc(sysdate+7)");
            }

            var reader = ExecuteReader(queryBuilder.ToString());
            return (new DbHelper()).ReadData<PlanWeek>(reader);
        }

        public bool ISSAvailable()
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append("SELECT oprsql.tm_iss_configuration.iss_available from dual");

            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            var result = (String)ExecuteScalar(queryBuilder.ToString());

            return (result == "Y") ? true : false;
            //return true;
        }
        public ValidateHAAO ExternalSku(string style, string color, string attribute, string size)
        {
            var output = new ValidateHAAO();
            var str_array = size.Split(',');
            for (var i = 0; i < str_array.Length; i++)
            {
                string query = "select COUNT(*) from EXTERNAL_SKU_MASTER where selling_style_cd='" + Val(style) + "' and selling_color_cd = '" + Val(color) + "' and selling_attribute_cd = '" + Val(attribute) + "' and selling_size_cd = '" + Val(str_array[i]) + "'  and customer_cd='HAA'";
                var skucount = ExecuteScalar(query);
                if (Convert.ToInt32(skucount) > 0)
                {
                    output.Haa = true;
                    return output;
                }
                else
                {
                    string Query1 = "select COUNT(*) from Codes_Table where category = 'ECD' and CODE=(select CORP_DIVISION_CD from Style where STYLE_CD='" + Val(style) + "')";
                    var corp_division_count = ExecuteScalar(Query1);
                    if (Convert.ToInt32(corp_division_count) > 0)
                    {
                        string Query2 = "select Count(*) from  external_sku_xref where STYLE_CD='" + Val(style) + "' AND COLOR_CD='" + Val(color) + "' AND ATTRIBUTE_CD='" + Val(attribute) + "' AND SIZE_CD='" + Val(str_array[i]) + "'";
                        var Count_Xref = ExecuteScalar(Query2);
                        if (Convert.ToInt32(Count_Xref) > 0)
                        {
                            output.Haa = true;
                            return output;
                        }
                        else
                        {
                            output.Haa = false;
                            output.Error = true;
                            return output;
                        }
                    }
                }
            }
            output.Haa = false;
            return output;
        }
        
        public IList<Planner> GetPlannerListFull()
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append("select distinct planner_cd \"PlannerCd\" from avyx_summary_view where planner_cd is not null ");
           queryBuilder.Append("UNION ");
           queryBuilder.Append("select distinct planner_cd from iss_prod_order where "); 
            queryBuilder.Append("order_version =' "+LOVConstants.GlobalOrderVersion+"'");
            queryBuilder.Append(" order by 1 ");
            IDataReader reader = ExecuteReader(queryBuilder.ToString());
            var result = (new DbHelper()).ReadData<Planner>(reader);
            return result;
        }
        public IList<Planner> GetCorpDivisionList()
        {
            string corpQuery =
           "select c.Corp_Division_CD || ' - ' || c.corp_division_short_desc \"CorpDivDesc\", c.Corp_Division_CD \"CorpDivCd\"  from corp_division c where c.active_cd = 'A' order by c.corp_division_cd";

            IDataReader reader = ExecuteReader(corpQuery);
            var result = (new DbHelper()).ReadData<Planner>(reader);
            return result;
        }

        public string GetDemandPolicy(string style, string color, string attribute, string size)
        {
            
            var queryBuilder = new StringBuilder();
            queryBuilder.Append("select Demand_Policy from AVYX_DEMAND_NET_VIEW where selling_style_cd = '" + Val(style) + "' and selling_color_cd = '" + Val(color) + "' and selling_attribute_cd = '" + Val(attribute) + "' and selling_size_cd = '" + Val(size) + "' and rownum=1  ");

            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            var result = (string)ExecuteScalar(queryBuilder.ToString());
            return result;

        }
    }
}
