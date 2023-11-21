using ISS.Common;
using ISS.Core.Model.Capacity;
using ISS.DAL;
using ISS.Repository.Common;
using ISS.Core.Model.Common;
using ISS.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
namespace ISS.Repository.Capacity
{
    public class CapacityRepository : RepositoryBase
    {
        public const string DATE_FORMAT = "MMM_dd_yyyy";
        #region Private Variables
        IList<AllocationDetail> _inTypeDetails = null;
        IList<AllocationDetail> _suggestedTypeDetails = null;
        IList<AllocationDetail> _spillTypeDetails = null;
        IList<AllocationDetail> _wipLockedTypeDetails = null;
        IList<AllocationDetail> _wipReleasedTypeDetails = null;
        #endregion

        public DataTable GetCapacityAllocations(CapacitySearch searchCriteria, string action)
        {
            StringBuilder queryBuilder = GetQueryString(searchCriteria);
            IDataReader reader = ExecuteReader(queryBuilder.ToString());
            var result = (new DbHelper()).ReadData<AllocationDetail>(reader);

            
           
            if(action == "Grid")
            return SetDynamicProperty(result, action);
            else
                return SetExcelDynamicProperty(result, action);
        }

        public IList<AggregatePopup> GetPlantAggregateDetails(CapacitySearch searchCriteria)
        {
            StringBuilder queryBuilder = new StringBuilder();

            queryBuilder.Append(" with cap_data as (select 'ISS_CAP_ALLOC_AGGREGATE' TableName, CORP_DIVISION_GROUP CorpDivisionGroup,CHILD_PLANT ChildPlant,PARENT_PLANT ParentPlant,CAPACITY_GROUP CapacityGroup,CAPACITY_ALLOC CapacityAlloc,INCLUDE_IND IncludeInd,");
            queryBuilder.Append(" USER_ID UserId,to_char(create_date,'MM/DD/YYYY') CreateDate, to_char(update_date,'MM/DD/YYYY') UpdateDate ");
            queryBuilder.Append(" from da.iss_cap_alloc_aggregate a");
            queryBuilder.Append(" union all");
            queryBuilder.Append(" select 'ISS_CAP_ALLOC_AGGREATE_PLAN' TableName, CORP_DIVISION_GROUP CorpDivisionGroup,CHILD_PLANT ChildPlant,PARENT_PLANT ParentPlant,CAPACITY_GROUP CapacityGroup,CAPACITY_ALLOC CapacityAlloc,INCLUDE_IND IncludeInd,");
            queryBuilder.Append(" USER_ID UserId,to_char(create_date,'MM/DD/YYYY') CreateDate, to_char(update_date,'MM/DD/YYYY') UpdateDate ");
            queryBuilder.Append(" from da.iss_cap_alloc_aggregate_plan a) ");
            queryBuilder.Append(" select * from cap_data ");

            if (!string.IsNullOrWhiteSpace(searchCriteria.Plant))
            {
                queryBuilder.Append(" where (cap_data.ChildPlant  in (" + FormatInClause(searchCriteria.Plant) + ") or ");
                queryBuilder.Append(" cap_data.ParentPlant  in (" + FormatInClause(searchCriteria.Plant) + ") ) ");
            }
            
            queryBuilder.Append(" order by 1 desc, 2,3,4,5,6 ");


            IDataReader reader = ExecuteReader(queryBuilder.ToString());
            var result = (new DbHelper()).ReadData<AggregatePopup>(reader);
            return result;
        }

        protected List<WorkCenter> GetAggregationPlants(CapacitySearch searchCriteria)
        {
        

            var query = new StringBuilder();

            query.Append("select distinct child_plant \"WorkCenter_Cd\" from da.iss_cap_alloc_aggregate_plan where  ");

            string plant = searchCriteria.Plant.Replace(ISS.Common.LOVConstants.PlantAggregationSuffix, "");

            query.Append(" PARENT_PLANT  in (" + FormatInClause(plant) + ")  ");

            if (!string.IsNullOrEmpty(searchCriteria.CapacityGroup)) 
            {
                query.Append(" and  CAPACITY_GROUP in (" + FormatInClause(searchCriteria.CapacityGroup) + ")  ");

            }
            if (!string.IsNullOrEmpty(searchCriteria.WorkCenter)) 
            {
                query.Append(" and   CAPACITY_ALLOC in (" + FormatInClause(searchCriteria.WorkCenter) + ")  ");

            }
            query.Append(" and   INCLUDE_IND = 'Y' ");
            query.Append(" order by 1");
            // select distinct capacity_alloc "WorkCenter_Cd" from avyx_summary_view where cut_alloc is not null  order by 1
            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<WorkCenter>(reader);
            
            return result;
        }

       
        public IList<AllocationPopup> GetAllocationDetails(CapacitySearch searchCriteria)
        {
            StringBuilder queryBuilder = new StringBuilder();

            // Save current Plant
            string plants = searchCriteria.Plant ;

            // Replace Aggregation Plant with list of real aggregation plants
            if (plants.IndexOf(ISS.Common.LOVConstants.PlantAggregationSuffix) >= 0) {

                // Replace (A) with blanks and trim

                searchCriteria.Plant = searchCriteria.Plant.Replace(ISS.Common.LOVConstants.PlantAggregationSuffix,"").Trim();
                List<WorkCenter> wc = GetAggregationPlants(searchCriteria);
                searchCriteria.Plant = plants; // Reset Original search critiera in case someone needs it 

                plants = searchCriteria.Plant.Replace(ISS.Common.LOVConstants.PlantAggregationSuffix, "").Trim();

                // Add in all child plants
                if (wc != null)
                {
                    foreach (WorkCenter w in wc)
                        plants += "," + w.WorkCenter_Cd.Trim();
                }


            }



            queryBuilder.Append("select  capacity_group \"CapacityGroup\", plant \"Plant\", workcenter \"WorkCenter\", style_cd \"StyleCD\", color_cd \"ColorCD\", attribute_cd \"AttributeCD\", due_date \"DueDate\", round(SAH, 2) \"SAH\", SIZE_SHORT_DESC \"SizeCD\",");
            queryBuilder.Append(" prod_order_no \"ProdOrderNo\", round(decode(sah,0,0,WORKORDER_QTY)/decode(capacity_group,'TEX',1,12)) \"WOQty\", priority \"Priority\"");
            queryBuilder.Append(" from (select  decode(p.production_status,'P','PLAN','WIP') capacity_type, o.style_cd,o.color_cd,o.attribute_cd,o.size_cd,  c.curr_due_date due_date,   decode(c.capacity_group,'TEX',o.total_curr_order_qty,((o.total_curr_order_qty * c.std_labor_time) / c.std_units)) sah, p.production_status, c.capacity_group, c.work_center_plant plant, c.ALLOC_GROUP workcenter, decode(c.capacity_group,'" + Val(searchCriteria.CapacityGroup.ToUpper()) + "',o.total_curr_order_qty,decode(o.order_label,p.super_order,o.total_curr_order_qty,0)) workorder_qty, decode(p.enforcement,1,'N','Y') spill_over, 0 owner_sah, o.order_label prod_order_no, p.priority");
            queryBuilder.Append(" from iss_prod_order_detail o, iss_prod_order_capacity c, iss_prod_order p");
            queryBuilder.Append(" where o.order_version = c.order_version    and o.order_label = c.order_label  and o.order_version = p.order_version    and o.super_order = p.super_order    and c.order_version = 1");
            if (!string.IsNullOrWhiteSpace(searchCriteria.CapacityGroup))
            {
                queryBuilder.Append(" and c.CAPACITY_GROUP  in (" + FormatInClause(searchCriteria.CapacityGroup.ToUpper()) + ")");
            }


            if (!string.IsNullOrWhiteSpace(plants))
            {
                queryBuilder.Append(" and  c.work_center_plant in (" + FormatInClause(plants.ToUpper()) + ")");
            }
            if (!string.IsNullOrWhiteSpace(searchCriteria.WorkCenter))
            {
                queryBuilder.Append(" and c.alloc_group = '" + Val(searchCriteria.WorkCenter) + "'");
            }
            if (!string.IsNullOrWhiteSpace(searchCriteria.PlanEndDate))
            {
                queryBuilder.Append(" and c.curr_due_date >=  to_date('" + searchCriteria.PlanEndDate + "','mm/dd/yyyy')-6 and c.curr_due_date <= to_date('" + searchCriteria.PlanEndDate + "','mm/dd/yyyy')");
            }

            queryBuilder.Append(") a , item_size i where");
            if (!string.IsNullOrWhiteSpace(searchCriteria.CapacityType))
            {
                queryBuilder.Append(" a.capacity_type = '" + Val(searchCriteria.CapacityType) + "' and");
            }
            queryBuilder.Append(" a.size_cd = i.size_cd");
            if (!string.IsNullOrWhiteSpace(searchCriteria.ProductionStatus))
            {
                queryBuilder.Append(" and a.PRODUCTION_STATUS = '" + searchCriteria.ProductionStatus.Trim() + "'");
            }
            if (!string.IsNullOrWhiteSpace(searchCriteria.SpillOver))
            {
                queryBuilder.Append(" and a.SPILL_OVER = '" + searchCriteria.SpillOver.Trim() + "'");
            }
            //queryBuilder.Append(" order by a.due_date,a.priority,a.prod_order_no,a.style_cd,a.color_cd,a.attribute_cd,a.size_cd ");

            IDataReader reader = ExecuteReader(queryBuilder.ToString());
            var result = (new DbHelper()).ReadData<AllocationPopup>(reader);

            

            return result;
        }

        public DataTable SetDynamicProperty(IList<AllocationDetail> reader, string action)
        {
            var planWeek = new ApplicationRepository().GetPlanBeginEndDates()
                                                      .Distinct()
                                                      .ToList();
            DateTime weekEndDate = planWeek.LastOrDefault().Week_End_Date;
            List<string> columnHeaders = new List<string>();
            List<DateTime> planweekList = new List<DateTime>();
            columnHeaders.Add("Plant");
            columnHeaders.Add("Workcenter");
            columnHeaders.Add("Type");
            columnHeaders.Add("Plant1");
            columnHeaders.Add("Workcenter1");
            columnHeaders.Add("Type1");
            columnHeaders.Add("Prior");
            columnHeaders.Add("ProductionStatus");
            columnHeaders.Add("SpillOver");
            columnHeaders.Add("CapacityGroup");
            
            for (int i = 1; i <= 52; i++)
            {
                columnHeaders.Add(weekEndDate.ToString(DATE_FORMAT));
                planweekList.Add(weekEndDate);
                weekEndDate = weekEndDate.AddDays(7);
            }

            DataTable dtAllocation = new DataTable();

            foreach (var header in columnHeaders)
            {
                if (!dtAllocation.Columns.Contains(header))
                {
                    dtAllocation.Columns.Add(header);
                }
            }
            
            return SetGridValues(reader, dtAllocation, planweekList, action);
        }

        public DataTable SetExcelDynamicProperty(IList<AllocationDetail> reader, string action)
        {
            var planWeek = new ApplicationRepository().GetPlanBeginEndDates()
                                                      .Distinct()
                                                      .ToList();
            DateTime weekEndDate = planWeek.LastOrDefault().Week_End_Date;
            List<string> columnHeaders = new List<string>();
            List<DateTime> planweekList = new List<DateTime>();
            columnHeaders.Add("Plant");
            columnHeaders.Add("Target WOS");
            columnHeaders.Add("Actual WOS");
            columnHeaders.Add("SAH to Target");
            columnHeaders.Add("Workcenter");
            columnHeaders.Add("Type");
            columnHeaders.Add("Plant1");
            columnHeaders.Add("Target WOS1");
            columnHeaders.Add("Actual WOS1");
            columnHeaders.Add("SAH to Target1");
            columnHeaders.Add("Workcenter1");
            columnHeaders.Add("Type1");
            columnHeaders.Add("Prior");
            columnHeaders.Add("ProductionStatus");
            columnHeaders.Add("SpillOver");
            columnHeaders.Add("CapacityGroup");

            for (int i = 1; i <= 52; i++)
            {
                columnHeaders.Add(weekEndDate.ToString(DATE_FORMAT));
                planweekList.Add(weekEndDate);
                weekEndDate = weekEndDate.AddDays(7);
            }

            DataTable dtAllocation = new DataTable();

            foreach (var header in columnHeaders)
            {
                if (!dtAllocation.Columns.Contains(header))
                {
                    dtAllocation.Columns.Add(header);
                }
            }

            return SetGridValues(reader, dtAllocation, planweekList, action);
        }

        #region Private Methods

        private void CalculateWO(Dictionary<string, CapacityWeek> weekList, out decimal actual, out decimal targetSAH)
        {
            decimal Net = 0;
            decimal SumLR = 0;
            decimal sah = 0;
            actual = targetSAH = 0;
            var weekSupply = weekList.Any() ? weekList.Values.Sum(s => s.WeekSupply) : 0;
            var prior = weekList.Values.FirstOrDefault();
            if (weekSupply > 0)
            {
                SumLR = weekList.Values.Sum(s => s.Locked + s.Released);
                //SumLR += (prior.PriorLocked + prior.PriorReleased);
                var firstFew = weekList.Values.Take(Convert.ToInt32(weekSupply));
                Net = firstFew.Sum(s => s.Locked + s.Released);
                sah = firstFew.Sum(s => s.Initial);
                //Net += (prior.PriorLocked + prior.PriorReleased);//.RoundCustom(0);
                //sah += prior.PriorInitial;//.RoundCustom(0);
            }

            if (sah > 0)
            {
                actual = (SumLR / sah) * weekSupply;
                targetSAH = sah - Net;
            }
        }


        // PROD4 end of communication issue

        private StringBuilder GetQueryString(CapacitySearch searchCriteria)
        {
            DateTime endDate = new ApplicationRepository().GetPlanBeginEndDates().LastOrDefault().Week_End_Date;
            endDate = endDate.AddDays(52 * 7);
            string planDate = endDate.ToString("yyyyMMdd");

            if (!searchCriteria.ShowIndivWorkcenters && !searchCriteria.ShowAggregateWorkcenters)
                throw new Exception("At least one of  Individual or Aggregate View must be checked");
            
            StringBuilder queryBuilder = new StringBuilder();
            
            //queryBuilder.Append(" cap_data as (");
            queryBuilder.Append("select  capacity_group \"CapacityGroup\", plant \"Plant\", workcenter \"WorkCenter\", capacity_type \"CapacityType\", production_status \"ProductionStatus\", spill_over \"SpillOver\", due_date \"DueDate\", round(sum(SAH), 6) \"TotalSAH\", sum(parent_WORKORDER_QTY) \"ParentWOQty\",  sum(owner_sah) \"OwnerSAH\"");
            queryBuilder.Append(" from (select  decode(p.production_status,'P','PLAN','WIP') capacity_type,    c.curr_due_date due_date,   decode ( c.capacity_group, 'TEX', o.total_curr_order_qty, (   ( o.total_curr_order_qty * c.std_labor_time )/ c.std_units )) sah ,    p.production_status,    c.capacity_group,    c.work_center_plant plant,    c.ALLOC_GROUP workcenter,    decode(c.capacity_group,'CUT',o.total_curr_order_qty,decode(o.order_label,p.super_order,DECODE (o.parent_order,null,o.total_curr_order_qty,0), 0 )) parent_workorder_qty,    decode(p.enforcement,1,'N','Y') spill_over, 0 owner_sah");
            queryBuilder.Append(" from iss_prod_order_detail o, iss_prod_order_capacity c, iss_prod_order p");
            queryBuilder.Append(" where o.order_version = c.order_version    and o.order_label = c.order_label  and o.order_version = p.order_version    and o.super_order = p.super_order    and c.order_version = 1");
            if (!string.IsNullOrWhiteSpace(searchCriteria.CapacityGroup))
            {
                //queryBuilder.Append(" and c.CAPACITY_GROUP ='" + Val(searchCriteria.CapacityGroup.ToUpper()) + "'");
                queryBuilder.Append(" and c.CAPACITY_GROUP  in (" + FormatInClause(searchCriteria.CapacityGroup.ToUpper()) + ")");
            }
            if (!string.IsNullOrWhiteSpace(searchCriteria.Plant))
            {
                queryBuilder.Append(" and  c.work_center_plant in (" + FormatInClause(searchCriteria.Plant) + ")");
            }
            if (!string.IsNullOrWhiteSpace(searchCriteria.WorkCenter))
            {
                queryBuilder.Append(" and c.alloc_group in (" + FormatInClause(searchCriteria.WorkCenter) + ")");
            }
            queryBuilder.Append(" Union All   select  'IN' capacity_type, trunc(r.week_end_date) due_date,  r.total_capacity  sah,  ' ' production_status,   r.capacity_group,  r.plant,   r.capacity_alloc workcenter,   0 parent_workorder_qty,    'N' spill_over, 0 owner_sah ");
            //Azalia Soriano 10/09/2019
            //queryBuilder.Append(" from avyx_capacity_agg_view r");
            queryBuilder.Append(" from oprsql.iss_capacity_view r");
            if (!string.IsNullOrWhiteSpace(searchCriteria.CapacityGroup))
            {
                //queryBuilder.Append(" where r.CAPACITY_GROUP = '" + Val(searchCriteria.CapacityGroup.ToUpper()) + "'");
                queryBuilder.Append(" where r.CAPACITY_GROUP  in (" + FormatInClause(searchCriteria.CapacityGroup.ToUpper()) + ")");
            }
            if (!string.IsNullOrWhiteSpace(searchCriteria.Plant))
            {
                queryBuilder.Append(" and r.plant in (" + FormatInClause(searchCriteria.Plant) + ")");
            }
            if (!string.IsNullOrWhiteSpace(searchCriteria.WorkCenter))
            {
                queryBuilder.Append(" and r.capacity_alloc in (" + FormatInClause(searchCriteria.WorkCenter) + ")");
            }
            queryBuilder.Append(" ) a");

            queryBuilder.Append(" where (a.due_date <= to_date('" + planDate + "','yyyymmdd'))");
            
            queryBuilder.Append(" group by capacity_group,plant,workcenter,capacity_type, production_status, due_date,spill_over");
            queryBuilder.Append(" union all  select capacity_group, plant, capacity_alloc workcenter, 'WOS', 'P', 'N', sysdate, min(a.WEEKS_OF_SUPPLY), 0, 0 from avyx_capacity a");
            if (!string.IsNullOrWhiteSpace(searchCriteria.CapacityGroup))
            {
                queryBuilder.Append(" where a.CAPACITY_GROUP in (" + FormatInClause(searchCriteria.CapacityGroup.ToUpper()) + ")");
            }
            if (!string.IsNullOrWhiteSpace(searchCriteria.Plant))
            {
                queryBuilder.Append(" and  a.plant in (" + FormatInClause(searchCriteria.Plant) + ")");
            }
            if (!string.IsNullOrWhiteSpace(searchCriteria.WorkCenter))
            {
                queryBuilder.Append(" and a.capacity_alloc in (" + FormatInClause(searchCriteria.WorkCenter) + ")");
            }
            queryBuilder.Append(" group by capacity_group,plant,capacity_alloc ");
            

            // Build inner SQL to retrive raw capacity data
            string raw_sql = queryBuilder.ToString();


            queryBuilder.Clear();

            queryBuilder.Append("cap_data as (");
            queryBuilder.Append("select 'Ind' cssclass , x.\"CapacityGroup\",  x.\"Plant\",  x.\"WorkCenter\",  x.\"CapacityType\",  x.\"ProductionStatus\",  x.\"SpillOver\",  x.\"DueDate\",  x.\"TotalSAH\",  x.\"ParentWOQty\",   x.\"OwnerSAH\"  from (");
            queryBuilder.Append(raw_sql);
            queryBuilder.Append(" ) x ");
            queryBuilder.Append(") ");

            string with_cap = queryBuilder.ToString();

            queryBuilder.Clear();

            queryBuilder.Append("agg_data as (");
            queryBuilder.Append("select 'Agg' cssclass,x.\"CapacityGroup\",b.parent_plant||' (A) ' \"Plant\", x.\"WorkCenter\",x.\"CapacityType\",x.\"ProductionStatus\", x.\"SpillOver\", x.\"DueDate\", round (decode(x.\"CapacityType\",'WOS',avg(x.\"TotalSAH\"),sum(x.\"TotalSAH\")),3) \"TotalSAH\", sum(x.\"ParentWOQty\") \"ParentWOQty\", sum(x.\"OwnerSAH\") \"OwnerSAH\"  from (");
            queryBuilder.Append(raw_sql + ") x  inner join ( select distinct CHILD_PLANT plant_cd , parent_plant, CAPACITY_GROUP,CAPACITY_ALLOC  from da.iss_cap_alloc_aggregate_plan b where INCLUDE_IND = 'Y' union select distinct PARENT_PLANT plant_cd ,parent_plant, CAPACITY_GROUP,CAPACITY_ALLOC from da.iss_cap_alloc_aggregate_plan b where INCLUDE_IND = 'Y' ) b  on x.\"CapacityGroup\" = b.capacity_group and x.\"Plant\"  = b.plant_cd and x.\"WorkCenter\" = b.CAPACITY_ALLOC  group by  x.\"CapacityGroup\",b.parent_plant||' (A) ' , x.\"WorkCenter\", x.\"CapacityType\", x.\"ProductionStatus\", x.\"SpillOver\", x.\"DueDate\"");
            
            queryBuilder.Append(") ");

            string with_agg = queryBuilder.ToString();

            // Build With clauses

            queryBuilder.Clear();
            queryBuilder.Append("with ");

            if (searchCriteria.ShowIndivWorkcenters)
            {
                queryBuilder.Append(with_cap);
                 if (searchCriteria.ShowAggregateWorkcenters)
                     queryBuilder.Append(" , ");
            }
            if (searchCriteria.ShowAggregateWorkcenters) { 
                queryBuilder.Append(with_agg);
            }

            queryBuilder.Append(" , capacity_results as (");
            
            if (searchCriteria.ShowIndivWorkcenters && searchCriteria.ShowAggregateWorkcenters)
                queryBuilder.Append("select * from cap_data union all select * from agg_data") ;
            else if (searchCriteria.ShowIndivWorkcenters)
                queryBuilder.Append("select * from cap_data ") ;
            else 
                 queryBuilder.Append("select * from agg_data ");
           

            queryBuilder.Append(") select * from capacity_results ");

            if (!string.IsNullOrEmpty(searchCriteria.OrderBy))
                queryBuilder.Append(" " + searchCriteria.OrderBy + " ");
            else
                queryBuilder.Append(" order by decode(cssclass,'Agg',1,'Ind',2,3) ,3,4 ");

            string str = queryBuilder.ToString();

            return queryBuilder;
        }

        private void CleanseGridValues(IEnumerable<AllocationDetail> reader)
        {
            if (reader != null)
                foreach (AllocationDetail a in reader)
                {
                    // Reset WOS value to one decimal to handle AVG from Aggregation Plants

                    if (a.CapacityType == "WOS")
                    {
                        a.TotalSAH = Math.Round(a.TotalSAH, 1);
                    }
                }
        }

        private DataTable SetGridValues(IEnumerable<AllocationDetail> reader, DataTable dtAllocation, List<DateTime> planEndWeek, string action)
        {          
            int i = 0;
            DateTime priorWeek = new DateTime();

            CleanseGridValues( reader);

            var Result = reader.GroupBy(s => new { s.WorkCenter, s.Plant })
                               .Select(s => new AllocationDetail()
                                               {
                                                   WorkCenter = s.Key.WorkCenter,
                                                   Plant = s.Key.Plant,                                                  
                                                   CapacityGroup = s.Select(c => c.CapacityGroup).FirstOrDefault()
                                               });


            foreach (var alloc in Result)
            {
                Dictionary<string, CapacityWeek> extraRows = new Dictionary<string, CapacityWeek>();
                var capacityTypes = reader.Where(s => string.Compare(s.WorkCenter, alloc.WorkCenter, true) == 0 && string.Compare(s.Plant, alloc.Plant, true) == 0)
                                          .GroupBy(s => new { s.CapacityType })
                                          .Select(s => s);

                i = 0;
                foreach (var plan in planEndWeek.Distinct())
                {
                    if (dtAllocation.Columns.Contains(plan.ToString(DATE_FORMAT)))
                    {
                        decimal sah = 0;
                        decimal parentQty = 0;
                        decimal priorSAH = 0;

                        i++;
                        if (i == 1)
                        {
                            priorWeek = plan.AddDays(-7);
                        }
                                                
                        CapacityWeek week = new CapacityWeek()
                        {
                            Initial = 0,
                            Net = 0,
                            SpilledDz = 0,
                            Planned = 0,
                            PlanNet = 0,
                            Released = 0,
                            ReleasedDz = 0,
                            LockedDz = 0,
                            Locked = 0,
                            Spill = 0,
                            PlannedDz = 0,
                            WeekSupply = 0
                        };

                        foreach (var item in capacityTypes)
                        {
                            switch ((CapacityType)Enum.Parse(typeof(CapacityType), item.Key.CapacityType, true))
                            {
                                case CapacityType.IN:
                                    _inTypeDetails = new List<AllocationDetail>();
                                    _inTypeDetails = capacityTypes.Where(s => string.Compare(s.Key.CapacityType, CapacityType.IN.ToString(), true) == 0)
                                                                  .Select(s => s.ToList())
                                                                  .FirstOrDefault();

                                    sah = _inTypeDetails.Where(s => s.DueDate <= plan && s.DueDate > plan.AddDays(-7))
                                                        .Sum(s => s.TotalSAH);

                                    priorSAH = _inTypeDetails.Where(s => s.DueDate <= priorWeek)
                                                             .Sum(s => s.TotalSAH);

                                    week.Initial = sah;
                                    week.PriorInitial = priorSAH;
                                    break;
                                case CapacityType.WIP:
                                    _wipLockedTypeDetails = new List<AllocationDetail>();
                                    _wipReleasedTypeDetails = new List<AllocationDetail>();

                                    var wipDetails = capacityTypes.Where(s => string.Compare(s.Key.CapacityType, CapacityType.WIP.ToString(), true) == 0)
                                                                  .Select(s => s.ToList())
                                                                  .FirstOrDefault();

                                    _wipLockedTypeDetails = wipDetails.Where(s => string.Compare(s.ProductionStatus, ProductionStatus.Locked.GetDescription(), true) == 0)
                                                                      .ToList();

                                    _wipReleasedTypeDetails = wipDetails.Where(s => string.Compare(s.ProductionStatus, ProductionStatus.Released.GetDescription(), true) == 0)
                                                                        .ToList();

                                    sah = _wipLockedTypeDetails.Where(s => s.DueDate <= plan && s.DueDate > plan.AddDays(-7))
                                                               .Sum(s => s.TotalSAH);
                                    parentQty = _wipLockedTypeDetails.Where(s => s.DueDate <= plan && s.DueDate > plan.AddDays(-7))
                                                                     .Sum(s => s.ParentWOQty);
                                    priorSAH = _wipLockedTypeDetails.Where(s => s.DueDate <= priorWeek)
                                                                    .Sum(s => s.TotalSAH);

                                    var releasedsah = _wipReleasedTypeDetails.Where(s => s.DueDate <= plan && s.DueDate > plan.AddDays(-7))
                                                                             .Sum(s => s.TotalSAH);
                                    var releasedparentQty = _wipReleasedTypeDetails.Where(s => s.DueDate <= plan && s.DueDate > plan.AddDays(-7))
                                                                                   .Sum(s => s.ParentWOQty);
                                    var se = _wipReleasedTypeDetails.Where(s => s.DueDate <= priorWeek);
                                    var releasedpriorSAH = _wipReleasedTypeDetails.Where(s => s.DueDate <= priorWeek)
                                                                                  .Sum(s => s.TotalSAH.RoundCustom(0));

                                    week.Locked = sah;
                                    week.LockedDz = parentQty;
                                    week.Released = releasedsah;
                                    week.ReleasedDz = releasedparentQty;
                                    week.PriorLocked = priorSAH;
                                    week.PriorReleased = releasedpriorSAH;
                                    break;

                                case CapacityType.WOS:

                                    var _wosTypeDetails = capacityTypes.Where(s => string.Compare(s.Key.CapacityType, CapacityType.WOS.ToString(), true) == 0)
                                                                       .Select(s => s.ToList())
                                                                       .FirstOrDefault();

                                    var supply = _wosTypeDetails.Where(s => s.DueDate <= plan && s.DueDate > plan.AddDays(-7))
                                                                .Sum(t => t.TotalSAH);
                                    week.WeekSupply = supply;
                                    break;
                                case CapacityType.PLAN:
                                    _suggestedTypeDetails = new List<AllocationDetail>();
                                    _spillTypeDetails = new List<AllocationDetail>();

                                    var _planTypeDetails = capacityTypes.Where(s => string.Compare(s.Key.CapacityType, CapacityType.PLAN.ToString(), true) == 0)
                                                                        .Select(s => s.ToList())
                                                                        .FirstOrDefault();
                                    _suggestedTypeDetails = _planTypeDetails.Where(s => string.Compare(s.ProductionStatus, ProductionStatus.CapacitySuggested.GetDescription(), true) == 0 && string.Compare(s.SpillOver, LOVConstants.SpillOver.No, true) == 0)
                                                                            .ToList();
                                    _spillTypeDetails = _planTypeDetails.Where(s => string.Compare(s.ProductionStatus, ProductionStatus.CapacitySuggested.GetDescription(), true) == 0 && string.Compare(s.SpillOver, LOVConstants.SpillOver.Yes, true) == 0)
                                                                        .ToList();
                                    sah = _suggestedTypeDetails.Where(s => s.DueDate <= plan && s.DueDate > plan.AddDays(-7))
                                                               .Sum(s => s.TotalSAH);
                                    parentQty = _suggestedTypeDetails.Where(s => s.DueDate <= plan && s.DueDate > plan.AddDays(-7))
                                                                     .Sum(s => s.ParentWOQty);
                                    priorSAH = _suggestedTypeDetails.Where(s => s.DueDate <= priorWeek)
                                                                    .Sum(s => s.TotalSAH);
                                    var spilledsah = _spillTypeDetails.Where(s => s.DueDate <= plan && s.DueDate > plan.AddDays(-7))
                                                                      .Sum(s => s.TotalSAH);
                                    var spilledparentQty = _spillTypeDetails.Where(s => s.DueDate <= plan && s.DueDate > plan.AddDays(-7))
                                                                            .Sum(s => s.ParentWOQty);
                                    var spilledpriorSAH = _spillTypeDetails.Where(s => s.DueDate <= priorWeek)
                                                                           .Sum(s => s.TotalSAH);
                                    week.Spill = spilledsah;
                                    week.SpilledDz = spilledparentQty;
                                    week.Planned = sah;
                                    week.PlannedDz = parentQty;
                                    week.PriorSpill = spilledpriorSAH;
                                    week.PriorPlanned = priorSAH;
                                    break;
                            }

                        }

                        week.Net = week.Initial - (week.Locked + week.Released);
                        week.PlanNet = week.Net - (week.Planned + week.Spill);
                        week.PriorPlanNet = week.PriorNet - (week.PriorPlanned + week.PriorSpill);

                        if (i == 1)
                        {                            
                            week.Net = week.Net - (week.PriorLocked + week.PriorReleased);
                            week.PlanNet = week.Net - (week.PriorPlanned + week.PriorSpill);
                        }

                        if (!extraRows.Keys.Contains(plan.ToString(DATE_FORMAT)))
                        {
                            extraRows.Add(plan.ToString(DATE_FORMAT), week);
                        }

                    }
                }

                if(action == "Grid")
                FillGridValue(ref dtAllocation, extraRows, alloc);
                else
                    FillExportValue(ref dtAllocation, extraRows, alloc);
            }
            return dtAllocation;
        }

        private void FillGridValue(ref DataTable dtAllocation, Dictionary<string, CapacityWeek> extraRows, AllocationDetail alloc)
        {
            decimal actualWOS = 0;
            decimal targetSAH = 0;
            CalculateWO(extraRows, out actualWOS, out targetSAH);
            IList<AllocationDetail> typeDetails = null;            

            for (int j = 1; j <= 11; j++)
            {
                DataRow row = dtAllocation.NewRow();
                row["CapacityGroup"] = alloc.CapacityGroup;
                row["Plant1"] = alloc.Plant;
                row["Workcenter1"] = alloc.WorkCenter;
                switch (j)
                {
                    case 1:
                        if (Val((alloc.CapacityGroup).ToUpper()) == "TEX")
                        
                        {
                            row["Plant"] = alloc.Plant;
                            row["Workcenter"] = alloc.WorkCenter;
                            row["Type"] = "Capacity Lbs";
                            typeDetails = _inTypeDetails;
                            foreach (var column in extraRows)
                            {
                                row["prior"] = column.Value.PriorInitial.RoundCustom(0).ToString("f2");
                                row[column.Key] = column.Value.Initial.RoundCustom(0).ToNumberString();
                            }
                        }
                             else if (Val((alloc.CapacityGroup).ToUpper()) == "CUT")
                        {
                        row["Plant"] = alloc.Plant;                      
                        row["Workcenter"] = alloc.WorkCenter;
                        row["Type"] = "Capacity Dzs";
                        typeDetails = _inTypeDetails;
                        foreach (var column in extraRows)
                        {
                            row["prior"] = column.Value.PriorInitial.RoundCustom(0).ToString("f2");
                            row[column.Key] = column.Value.Initial.RoundCustom(0).ToNumberString();
                        }
                        }
                        else {
                        row["Plant"] = alloc.Plant;                      
                        row["Workcenter"] = alloc.WorkCenter;
                        row["Type"] = "Capacity Hrs";
                        typeDetails = _inTypeDetails;
                        foreach (var column in extraRows)
                        {
                            row["prior"] = column.Value.PriorInitial.RoundCustom(0).ToString("f2");
                            row[column.Key] = column.Value.Initial.RoundCustom(0).ToNumberString();
                        }
                        }
                        break;
                    case 2:
                        if (Val((alloc.CapacityGroup).ToUpper()) == "TEX")
                        {
                            row["Type"] = "Locked Lots Lbs";
                            row["Type1"] = CapacityType.WIP.ToString();
                            typeDetails = _wipLockedTypeDetails;

                            foreach (var column in extraRows)
                            {
                                row["prior"] = column.Value.PriorLocked.RoundCustom(0).ToString("f2");
                                row[column.Key] = (alloc.CapacityGroup.Equals("CUT", StringComparison.InvariantCultureIgnoreCase) || alloc.CapacityGroup.Equals("SRC", StringComparison.InvariantCultureIgnoreCase)) ? "N/A" : column.Value.Locked.RoundCustom(0).ToNumberString();
                            }
                        }
                        else 
                        {
                            row["Type"] = "Locked Lots Hrs";
                            row["Type1"] = CapacityType.WIP.ToString();
                            typeDetails = _wipLockedTypeDetails;

                            foreach (var column in extraRows)
                            {
                                row["prior"] = column.Value.PriorLocked.RoundCustom(0).ToString("f2");
                                row[column.Key] = (alloc.CapacityGroup.Equals("CUT", StringComparison.InvariantCultureIgnoreCase) || alloc.CapacityGroup.Equals("SRC", StringComparison.InvariantCultureIgnoreCase)) ? "N/A" : column.Value.Locked.RoundCustom(0).ToNumberString();
                            }
                        
                        }
                        break;
                    case 3:                       
                        row["Type"] = "Locked Lots Dzs";
                        row["Type1"] = CapacityType.WIP.ToString();
                        typeDetails = _wipLockedTypeDetails;
                        foreach (var column in extraRows)
                        {
                            row["prior"] = (column.Value.PriorLockedDz / 12).RoundCustom(0).ToString("f2");
                            row[column.Key] = (column.Value.LockedDz / 12).RoundCustom(0).ToNumberString();
                        }
                        break;
                    case 4:
                        if (Val((alloc.CapacityGroup).ToUpper()) == "TEX")
                        {

                            row["Plant"] = "Target WOS";
                            row["Workcenter"] = extraRows.Values.Sum(s => s.WeekSupply);
                            row["Type"] = "Released Lots Lbs";
                            row["Type1"] = CapacityType.WIP.ToString();
                            typeDetails = _wipReleasedTypeDetails;

                            foreach (var column in extraRows)
                            {
                                row["prior"] = column.Value.PriorReleased.RoundCustom(0).ToString("f2");
                                row[column.Key] = (alloc.CapacityGroup.Equals("CUT", StringComparison.InvariantCultureIgnoreCase) || alloc.CapacityGroup.Equals("SRC", StringComparison.InvariantCultureIgnoreCase)) ? "N/A" : column.Value.Released.RoundCustom(0).ToNumberString();
                            }
                        }
                        else 
                        {

                            row["Plant"] = "Target WOS";
                            row["Workcenter"] = extraRows.Values.Sum(s => s.WeekSupply);
                            row["Type"] = "Released Lots Hrs";
                            row["Type1"] = CapacityType.WIP.ToString();
                            typeDetails = _wipReleasedTypeDetails;

                            foreach (var column in extraRows)
                            {
                                row["prior"] = column.Value.PriorReleased.RoundCustom(0).ToString("f2");
                                row[column.Key] = (alloc.CapacityGroup.Equals("CUT", StringComparison.InvariantCultureIgnoreCase) || alloc.CapacityGroup.Equals("SRC", StringComparison.InvariantCultureIgnoreCase)) ? "N/A" : column.Value.Released.RoundCustom(0).ToNumberString();
                            }
                        }
                        break;
                    case 5:                        
                        row["Type"] = "Released Lots Dzs";
                        row["Type1"] = CapacityType.WIP.ToString();
                        typeDetails = _wipReleasedTypeDetails;
                        foreach (var column in extraRows)
                        {
                            row["prior"] = (column.Value.PriorReleasedDz / 12).RoundCustom(0).ToString("f2");
                            row[column.Key] = (column.Value.ReleasedDz / 12).RoundCustom(0).ToNumberString();
                        }
                        break;
                    case 6:
                        row["Plant"] = "Actual WOS";                        
                        row["Type"] = "Net - L/R";
                        row["Workcenter"] = actualWOS > 0 ? actualWOS.RoundCustom(1) : actualWOS;
                        foreach (var column in extraRows)
                        {
                            row["prior"] = column.Value.PriorNet.RoundCustom(0).ToString("f2");
                            row[column.Key] = column.Value.Net.RoundCustom(0).ToNumberString();
                        }
                        break;
                    case 7:
                        if (Val((alloc.CapacityGroup).ToUpper()) == "TEX")
                        {
                            row["Plant"] = "SAH To Target";
                            row["Workcenter"] = targetSAH > 0 ? targetSAH.RoundCustom(0).ToNumberString() : targetSAH.ToNumberString();
                            row["Type"] = "Suggested Lots Lbs";
                            row["Type1"] = CapacityType.PLAN.ToString();
                            typeDetails = _suggestedTypeDetails;

                            foreach (var column in extraRows)
                            {
                                row["prior"] = column.Value.PriorPlanned.RoundCustom(0).ToString("f2");
                                row[column.Key] = (alloc.CapacityGroup.Equals("CUT", StringComparison.InvariantCultureIgnoreCase) || alloc.CapacityGroup.Equals("SRC", StringComparison.InvariantCultureIgnoreCase)) ? "N/A" : column.Value.Planned.RoundCustom(0).ToNumberString();
                            }
                        }
                        else
                        {
                            row["Plant"] = "SAH To Target";
                            row["Workcenter"] = targetSAH > 0 ? targetSAH.RoundCustom(0).ToNumberString() : targetSAH.ToNumberString();
                            row["Type"] = "Suggested Lots Hrs";
                            row["Type1"] = CapacityType.PLAN.ToString();
                            typeDetails = _suggestedTypeDetails;

                            foreach (var column in extraRows)
                            {
                                row["prior"] = column.Value.PriorPlanned.RoundCustom(0).ToString("f2");
                                row[column.Key] = (alloc.CapacityGroup.Equals("CUT", StringComparison.InvariantCultureIgnoreCase) || alloc.CapacityGroup.Equals("SRC", StringComparison.InvariantCultureIgnoreCase)) ? "N/A" : column.Value.Planned.RoundCustom(0).ToNumberString();
                            }

                        }
                        break;
                    case 8:                       
                        row["Type"] = "Suggested Lots Dzs";
                        row["Type1"] = CapacityType.PLAN.ToString();
                        typeDetails = _suggestedTypeDetails;
                        foreach (var column in extraRows)
                        {
                            row["prior"] = (column.Value.PriorPlannedDz / 12).RoundCustom(0).ToString("f2");
                            row[column.Key] = (column.Value.PlannedDz / 12).RoundCustom(0).ToNumberString();
                        }
                        break;
                    case 9:
                        if (Val((alloc.CapacityGroup).ToUpper()) == "TEX")
                        {
                            row["Type"] = "Spillover Lots Lbs";
                            row["Type1"] = CapacityType.PLAN.ToString();
                            typeDetails = _spillTypeDetails;
                            foreach (var column in extraRows)
                            {
                                row["prior"] = column.Value.PriorSpill.RoundCustom(0).ToString("f2");
                                row[column.Key] = (alloc.CapacityGroup.Equals("CUT", StringComparison.InvariantCultureIgnoreCase) || alloc.CapacityGroup.Equals("SRC", StringComparison.InvariantCultureIgnoreCase)) ? "N/A" : column.Value.Spill.RoundCustom(0).ToNumberString();
                            }
                        }
                        else
                        {
                            row["Type"] = "Spillover Lots Hrs";
                            row["Type1"] = CapacityType.PLAN.ToString();
                            typeDetails = _spillTypeDetails;
                            foreach (var column in extraRows)
                            {
                                row["prior"] = column.Value.PriorSpill.RoundCustom(0).ToString("f2");
                                row[column.Key] = (alloc.CapacityGroup.Equals("CUT", StringComparison.InvariantCultureIgnoreCase) || alloc.CapacityGroup.Equals("SRC", StringComparison.InvariantCultureIgnoreCase)) ? "N/A" : column.Value.Spill.RoundCustom(0).ToNumberString();
                            }
                        }
                        break;
                    case 10:                        
                        row["Type"] = "Spillover Lots Dzs";
                        row["Type1"] = CapacityType.PLAN.ToString();
                        typeDetails = _spillTypeDetails;
                        foreach (var column in extraRows)
                        {
                            row["prior"] = (column.Value.PriorSpilledDz / 12).RoundCustom(0).ToString("f2");
                            row[column.Key] = (column.Value.SpilledDz / 12).RoundCustom(0).ToNumberString();
                        }
                        break;
                    case 11:                        
                        foreach (var column in extraRows)
                        {
                            row["Type"] = "Net";
                            row["prior"] = column.Value.PriorPlanNet.RoundCustom(0).ToString("n2");
                            row[column.Key] = column.Value.PlanNet.RoundCustom(0).ToNumberString();
                        }
                        break;
                }
                row["ProductionStatus"] = typeDetails != null && typeDetails.Any() ? typeDetails.Select(s => s.ProductionStatus).FirstOrDefault() : string.Empty;
                row["SpillOver"] = typeDetails != null && typeDetails.Any() ? typeDetails.Select(s => s.SpillOver).FirstOrDefault() : string.Empty;
                dtAllocation.Rows.Add(row);
            }
            dtAllocation.Rows.Add(dtAllocation.NewRow());
        }

        private void FillExportValue(ref DataTable dtAllocation, Dictionary<string, CapacityWeek> extraRows, AllocationDetail alloc)
        {
            decimal actualWOS = 0;
            decimal targetSAH = 0;
            CalculateWO(extraRows, out actualWOS, out targetSAH);
            IList<AllocationDetail> typeDetails = null;

            for (int j = 1; j <= 11; j++)
            {
                DataRow row = dtAllocation.NewRow();
                row["CapacityGroup"] = alloc.CapacityGroup;
                row["Plant1"] = alloc.Plant;
                //newly added
                row["Target WOS1"] = extraRows.Values.Sum(s => s.WeekSupply);
                row["Actual WOS1"] = actualWOS > 0 ? actualWOS.RoundCustom(1) : actualWOS;
                row["SAH to Target1"] = targetSAH > 0 ? targetSAH.RoundCustom(0).ToNumberString() : targetSAH.ToNumberString();
                //end
                row["Workcenter1"] = alloc.WorkCenter;
                switch (j)
                {
                    case 1:
                        if (Val((alloc.CapacityGroup).ToUpper()) == "TEX")
                        {
                            row["Plant"] = alloc.Plant;
                            ////newly added
                            row["Target WOS"] = extraRows.Values.Sum(s => s.WeekSupply);
                            row["Actual WOS"] = actualWOS > 0 ? actualWOS.RoundCustom(1) : actualWOS;
                            row["SAH to Target"] = targetSAH > 0 ? targetSAH.RoundCustom(0).ToNumberString() : targetSAH.ToNumberString();
                            ////end
                            row["Workcenter"] = alloc.WorkCenter;
                            row["Type"] = "Capacity Lbs";
                            typeDetails = _inTypeDetails;
                            foreach (var column in extraRows)
                            {
                                row["prior"] = column.Value.PriorInitial.RoundCustom(0).ToString("f2");
                                row[column.Key] = column.Value.Initial.RoundCustom(0).ToNumberString();
                            }
                        }
                        else if (Val((alloc.CapacityGroup).ToUpper()) == "CUT")
                        {
                            row["Plant"] = alloc.Plant;
                            ////newly added
                            row["Target WOS"] = extraRows.Values.Sum(s => s.WeekSupply);
                            row["Actual WOS"] = actualWOS > 0 ? actualWOS.RoundCustom(1) : actualWOS;
                            row["SAH to Target"] = targetSAH > 0 ? targetSAH.RoundCustom(0).ToNumberString() : targetSAH.ToNumberString();
                            ////end
                            row["Workcenter"] = alloc.WorkCenter;
                            row["Type"] = "Capacity Dzs";
                            typeDetails = _inTypeDetails;
                            foreach (var column in extraRows)
                            {
                                row["prior"] = column.Value.PriorInitial.RoundCustom(0).ToString("f2");
                                row[column.Key] = column.Value.Initial.RoundCustom(0).ToNumberString();
                            }
                        }
                        else
                        {
                            row["Plant"] = alloc.Plant;
                            ////newly added
                            row["Target WOS"] = extraRows.Values.Sum(s => s.WeekSupply);
                            row["Actual WOS"] = actualWOS > 0 ? actualWOS.RoundCustom(1) : actualWOS;
                            row["SAH to Target"] = targetSAH > 0 ? targetSAH.RoundCustom(0).ToNumberString() : targetSAH.ToNumberString();
                            ////end
                            row["Workcenter"] = alloc.WorkCenter;
                            row["Type"] = "Capacity Hrs";
                            typeDetails = _inTypeDetails;
                            foreach (var column in extraRows)
                            {
                                row["prior"] = column.Value.PriorInitial.RoundCustom(0).ToString("f2");
                                row[column.Key] = column.Value.Initial.RoundCustom(0).ToNumberString();
                            }
                        }
                        break;
                    case 2:
                        //newly added
                        if (Val((alloc.CapacityGroup).ToUpper()) == "TEX")
                        {
                            row["Plant"] = alloc.Plant;
                            row["Target WOS"] = extraRows.Values.Sum(s => s.WeekSupply);
                            row["Actual WOS"] = actualWOS > 0 ? actualWOS.RoundCustom(1) : actualWOS;
                            row["SAH to Target"] = targetSAH > 0 ? targetSAH.RoundCustom(0).ToNumberString() : targetSAH.ToNumberString();
                            row["Workcenter"] = alloc.WorkCenter;
                            //end
                            row["Type"] = "Locked Lots Lbs";
                            row["Type1"] = CapacityType.WIP.ToString();
                            typeDetails = _wipLockedTypeDetails;
                            foreach (var column in extraRows)
                            {
                                row["prior"] = column.Value.PriorInitial.RoundCustom(0).ToString("f2");
                                row[column.Key] = column.Value.Initial.RoundCustom(0).ToNumberString();
                            }
                        }
                        else
                        {
                            row["Plant"] = alloc.Plant;
                            row["Target WOS"] = extraRows.Values.Sum(s => s.WeekSupply);
                            row["Actual WOS"] = actualWOS > 0 ? actualWOS.RoundCustom(1) : actualWOS;
                            row["SAH to Target"] = targetSAH > 0 ? targetSAH.RoundCustom(0).ToNumberString() : targetSAH.ToNumberString();
                            row["Workcenter"] = alloc.WorkCenter;
                            //end
                            row["Type"] = "Locked Lots Hrs";
                            row["Type1"] = CapacityType.WIP.ToString();
                            typeDetails = _wipLockedTypeDetails;

                            foreach (var column in extraRows)
                            {
                                row["prior"] = column.Value.PriorLocked.RoundCustom(0).ToString("f2");
                                row[column.Key] = (alloc.CapacityGroup.Equals("CUT", StringComparison.InvariantCultureIgnoreCase) || alloc.CapacityGroup.Equals("SRC", StringComparison.InvariantCultureIgnoreCase)) ? "N/A" : column.Value.Locked.RoundCustom(0).ToNumberString();
                            }
                        }
                        break;
                    case 3:
                        //newly added

                        row["Plant"] = alloc.Plant;
                        row["Target WOS"] = extraRows.Values.Sum(s => s.WeekSupply);
                        row["Actual WOS"] = actualWOS > 0 ? actualWOS.RoundCustom(1) : actualWOS;
                        row["SAH to Target"] = targetSAH > 0 ? targetSAH.RoundCustom(0).ToNumberString() : targetSAH.ToNumberString();
                        row["Workcenter"] = alloc.WorkCenter;
                        //end
                        row["Type"] = "Locked Lots Dzs";
                        row["Type1"] = CapacityType.WIP.ToString();
                        typeDetails = _wipLockedTypeDetails;
                        foreach (var column in extraRows)
                        {
                            row["prior"] = (column.Value.PriorLockedDz / 12).RoundCustom(0).ToString("f2");
                            row[column.Key] = (column.Value.LockedDz / 12).RoundCustom(0).ToNumberString();
                        }
                        break;
                    case 4:
                        //row["Plant"] = "Target WOS";
                        //newly added
                        if (Val((alloc.CapacityGroup).ToUpper()) == "TEX")
                        {
                            row["Plant"] = alloc.Plant;
                            row["Target WOS"] = extraRows.Values.Sum(s => s.WeekSupply);
                            row["Actual WOS"] = actualWOS > 0 ? actualWOS.RoundCustom(1) : actualWOS;
                            row["SAH to Target"] = targetSAH > 0 ? targetSAH.RoundCustom(0).ToNumberString() : targetSAH.ToNumberString();
                            row["Workcenter"] = alloc.WorkCenter;
                            //end
                            //row["Workcenter"] = extraRows.Values.Sum(s => s.WeekSupply);
                            row["Type"] = "Released Lots Lbs";
                            row["Type1"] = CapacityType.WIP.ToString();
                            typeDetails = _wipReleasedTypeDetails;
                            foreach (var column in extraRows)
                            {
                                row["prior"] = column.Value.PriorInitial.RoundCustom(0).ToString("f2");
                                row[column.Key] = column.Value.Initial.RoundCustom(0).ToNumberString();
                            }
                        }
                        else
                        {
                            row["Plant"] = alloc.Plant;
                            row["Target WOS"] = extraRows.Values.Sum(s => s.WeekSupply);
                            row["Actual WOS"] = actualWOS > 0 ? actualWOS.RoundCustom(1) : actualWOS;
                            row["SAH to Target"] = targetSAH > 0 ? targetSAH.RoundCustom(0).ToNumberString() : targetSAH.ToNumberString();
                            row["Workcenter"] = alloc.WorkCenter;
                            //end
                            //row["Workcenter"] = extraRows.Values.Sum(s => s.WeekSupply);
                            row["Type"] = "Released Lots Hrs";
                            row["Type1"] = CapacityType.WIP.ToString();
                            typeDetails = _wipReleasedTypeDetails;

                            foreach (var column in extraRows)
                            {
                                row["prior"] = column.Value.PriorReleased.RoundCustom(0).ToString("f2");
                                row[column.Key] = (alloc.CapacityGroup.Equals("CUT", StringComparison.InvariantCultureIgnoreCase) || alloc.CapacityGroup.Equals("SRC", StringComparison.InvariantCultureIgnoreCase)) ? "N/A" : column.Value.Released.RoundCustom(0).ToNumberString();
                            }
                        }
                        break;
                    case 5:
                        //newly added
                        row["Plant"] = alloc.Plant;
                        row["Target WOS"] = extraRows.Values.Sum(s => s.WeekSupply);
                        row["Actual WOS"] = actualWOS > 0 ? actualWOS.RoundCustom(1) : actualWOS;
                        row["SAH to Target"] = targetSAH > 0 ? targetSAH.RoundCustom(0).ToNumberString() : targetSAH.ToNumberString();
                        row["Workcenter"] = alloc.WorkCenter;
                        //end
                        row["Type"] = "Released Lots Dzs";
                        row["Type1"] = CapacityType.WIP.ToString();
                        typeDetails = _wipReleasedTypeDetails;
                        foreach (var column in extraRows)
                        {
                            row["prior"] = (column.Value.PriorReleasedDz / 12).RoundCustom(0).ToString("f2");
                            row[column.Key] = (column.Value.ReleasedDz / 12).RoundCustom(0).ToNumberString();
                        }
                        break;
                    case 6:
                        //row["Plant"] = "Actual WOS";
                        //newly added
                        row["Plant"] = alloc.Plant;
                        row["Target WOS"] = extraRows.Values.Sum(s => s.WeekSupply);
                        row["Actual WOS"] = actualWOS > 0 ? actualWOS.RoundCustom(1) : actualWOS;
                        row["SAH to Target"] = targetSAH > 0 ? targetSAH.RoundCustom(0).ToNumberString() : targetSAH.ToNumberString();
                        row["Workcenter"] = alloc.WorkCenter;
                        //end
                        row["Type"] = "Net - L/R";
                        //row["Workcenter"] = actualWOS > 0 ? actualWOS.RoundCustom(1) : actualWOS;
                        foreach (var column in extraRows)
                        {
                            row["prior"] = column.Value.PriorNet.RoundCustom(0).ToString("f2");
                            row[column.Key] = column.Value.Net.RoundCustom(0).ToNumberString();
                        }
                        break;
                    case 7:
                        //row["Plant"] = "SAH To Target";
                        //newly added
                        if (Val((alloc.CapacityGroup).ToUpper()) == "TEX")
                        {
                            row["Plant"] = alloc.Plant;
                            row["Target WOS"] = extraRows.Values.Sum(s => s.WeekSupply);
                            row["Actual WOS"] = actualWOS > 0 ? actualWOS.RoundCustom(1) : actualWOS;
                            row["SAH to Target"] = targetSAH > 0 ? targetSAH.RoundCustom(0).ToNumberString() : targetSAH.ToNumberString();
                            row["Workcenter"] = alloc.WorkCenter;
                            //end
                            //row["Workcenter"] = targetSAH > 0 ? targetSAH.RoundCustom(0).ToNumberString() : targetSAH.ToNumberString();
                            row["Type"] = "Suggested Lots Lbs";
                            row["Type1"] = CapacityType.PLAN.ToString();
                            typeDetails = _suggestedTypeDetails;
                            foreach (var column in extraRows)
                            {
                                row["prior"] = column.Value.PriorInitial.RoundCustom(0).ToString("f2");
                                row[column.Key] = column.Value.Initial.RoundCustom(0).ToNumberString();
                            }
                        }
                        else
                        {
                            row["Plant"] = alloc.Plant;
                            row["Target WOS"] = extraRows.Values.Sum(s => s.WeekSupply);
                            row["Actual WOS"] = actualWOS > 0 ? actualWOS.RoundCustom(1) : actualWOS;
                            row["SAH to Target"] = targetSAH > 0 ? targetSAH.RoundCustom(0).ToNumberString() : targetSAH.ToNumberString();
                            row["Workcenter"] = alloc.WorkCenter;
                            //end
                            //row["Workcenter"] = targetSAH > 0 ? targetSAH.RoundCustom(0).ToNumberString() : targetSAH.ToNumberString();
                            row["Type"] = "Suggested Lots Hrs";
                            row["Type1"] = CapacityType.PLAN.ToString();
                            typeDetails = _suggestedTypeDetails;

                            foreach (var column in extraRows)
                            {
                                row["prior"] = column.Value.PriorPlanned.RoundCustom(0).ToString("f2");
                                row[column.Key] = (alloc.CapacityGroup.Equals("CUT", StringComparison.InvariantCultureIgnoreCase) || alloc.CapacityGroup.Equals("SRC", StringComparison.InvariantCultureIgnoreCase)) ? "N/A" : column.Value.Planned.RoundCustom(0).ToNumberString();
                            }
                        }
                        break;
                    case 8:
                        //newly added
                        row["Plant"] = alloc.Plant;
                        row["Target WOS"] = extraRows.Values.Sum(s => s.WeekSupply);
                        row["Actual WOS"] = actualWOS > 0 ? actualWOS.RoundCustom(1) : actualWOS;
                        row["SAH to Target"] = targetSAH > 0 ? targetSAH.RoundCustom(0).ToNumberString() : targetSAH.ToNumberString();
                        row["Workcenter"] = alloc.WorkCenter;
                        //end
                        row["Type"] = "Suggested Lots Dzs";
                        row["Type1"] = CapacityType.PLAN.ToString();
                        typeDetails = _suggestedTypeDetails;
                        foreach (var column in extraRows)
                        {
                            row["prior"] = (column.Value.PriorPlannedDz / 12).RoundCustom(0).ToString("f2");
                            row[column.Key] = (column.Value.PlannedDz / 12).RoundCustom(0).ToNumberString();
                        }
                        break;
                    case 9:
                        //newly added
                        if (Val((alloc.CapacityGroup).ToUpper()) == "TEX")
                        {
                            row["Plant"] = alloc.Plant;
                            row["Target WOS"] = extraRows.Values.Sum(s => s.WeekSupply);
                            row["Actual WOS"] = actualWOS > 0 ? actualWOS.RoundCustom(1) : actualWOS;
                            row["SAH to Target"] = targetSAH > 0 ? targetSAH.RoundCustom(0).ToNumberString() : targetSAH.ToNumberString();
                            row["Workcenter"] = alloc.WorkCenter;
                            //end
                            row["Type"] = "Spillover Lots Lbs";
                            row["Type1"] = CapacityType.PLAN.ToString();
                            typeDetails = _spillTypeDetails;
                            foreach (var column in extraRows)
                            {
                                row["prior"] = column.Value.PriorInitial.RoundCustom(0).ToString("f2");
                                row[column.Key] = column.Value.Initial.RoundCustom(0).ToNumberString();
                            }
                        }
                        else
                        {
                            row["Plant"] = alloc.Plant;
                            row["Target WOS"] = extraRows.Values.Sum(s => s.WeekSupply);
                            row["Actual WOS"] = actualWOS > 0 ? actualWOS.RoundCustom(1) : actualWOS;
                            row["SAH to Target"] = targetSAH > 0 ? targetSAH.RoundCustom(0).ToNumberString() : targetSAH.ToNumberString();
                            row["Workcenter"] = alloc.WorkCenter;
                            //end
                            row["Type"] = "Spillover Lots Hrs";
                            row["Type1"] = CapacityType.PLAN.ToString();
                            typeDetails = _spillTypeDetails;
                            foreach (var column in extraRows)
                            {
                                row["prior"] = column.Value.PriorSpill.RoundCustom(0).ToString("f2");
                                row[column.Key] = (alloc.CapacityGroup.Equals("CUT", StringComparison.InvariantCultureIgnoreCase) || alloc.CapacityGroup.Equals("SRC", StringComparison.InvariantCultureIgnoreCase)) ? "N/A" : column.Value.Spill.RoundCustom(0).ToNumberString();
                            }
                        }
                        break;
                    case 10:
                        //newly added
                        row["Plant"] = alloc.Plant;
                        row["Target WOS"] = extraRows.Values.Sum(s => s.WeekSupply);
                        row["Actual WOS"] = actualWOS > 0 ? actualWOS.RoundCustom(1) : actualWOS;
                        row["SAH to Target"] = targetSAH > 0 ? targetSAH.RoundCustom(0).ToNumberString() : targetSAH.ToNumberString();
                        row["Workcenter"] = alloc.WorkCenter;
                        //end
                        row["Type"] = "Spillover Lots Dzs";
                        row["Type1"] = CapacityType.PLAN.ToString();
                        typeDetails = _spillTypeDetails;
                        foreach (var column in extraRows)
                        {
                            row["prior"] = (column.Value.PriorSpilledDz / 12).RoundCustom(0).ToString("f2");
                            row[column.Key] = (column.Value.SpilledDz / 12).RoundCustom(0).ToNumberString();
                        }
                        break;
                    case 11:
                        //newly added
                        row["Plant"] = alloc.Plant;
                        row["Target WOS"] = extraRows.Values.Sum(s => s.WeekSupply);
                        row["Actual WOS"] = actualWOS > 0 ? actualWOS.RoundCustom(1) : actualWOS;
                        row["SAH to Target"] = targetSAH > 0 ? targetSAH.RoundCustom(0).ToNumberString() : targetSAH.ToNumberString();
                        row["Workcenter"] = alloc.WorkCenter;
                        //end
                        foreach (var column in extraRows)
                        {
                            row["Type"] = "Net";
                            row["prior"] = column.Value.PriorPlanNet.RoundCustom(0).ToString("n2");
                            row[column.Key] = column.Value.PlanNet.RoundCustom(0).ToNumberString();
                        }
                        break;
                }
                row["ProductionStatus"] = typeDetails != null && typeDetails.Any() ? typeDetails.Select(s => s.ProductionStatus).FirstOrDefault() : string.Empty;
                row["SpillOver"] = typeDetails != null && typeDetails.Any() ? typeDetails.Select(s => s.SpillOver).FirstOrDefault() : string.Empty;
                dtAllocation.Rows.Add(row);
            }
            dtAllocation.Rows.Add(dtAllocation.NewRow());
        }

        private string GetValueInFormat(decimal qty, decimal Dz = 0)
        {
            string valueFormat = "{0}({1})";
            return qty > 0 ? string.Format(valueFormat, Math.Round(qty, 0), Math.Round(Dz / 12, 0)) : qty.ToString();
        }

        #endregion
    }

}

