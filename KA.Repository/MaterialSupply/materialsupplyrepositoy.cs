using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISS.DAL;
using System.Data;
using ISS.Common;
using System.Data.Common;
using KA.Core.Model.MaterialSupply;
using ISS.Core.Model.Common;


namespace KA.Repository.MaterialSupply
{
    public partial class MaterialSupplyRepositoy : RepositoryBase
    {
        public MaterialSupplyRepositoy()
            : base()
        {

        }


       

        public List<MaterialPAB> MaterialSupplySearchDetails(MaterialBlankSupplySearch mbSupply)
        {
           
            var queryBuilder = new StringBuilder();
            // Inventory
            queryBuilder.Append("select Style Style,Color Color,Attribute Attribute,SizeCD SizeCD, size_short_desc \"Size\",PlantNo DC, fiscal_week FiscalWeek,fiscal_year FiscalYear,WEEK_BEGIN_DATE WeekBegDate,TranType TranType,sum(QtyOnHand) SumQty from (");
            //ON-HAND
            queryBuilder.Append("select style Style,color Color,attribute_cd Attribute,i.size_cd SizeCD, s.size_short_desc,plant_no PlantNo,on_hand_qty QtyOnHand,sysdate SDate, '" + LOVConstants.PABTypes.OnHand + "' TranType ");

            queryBuilder.Append(" from iss_inventory i , plant p,item_size s where i.plant_no = p.plant_cd and p.distribution_ind = 'Y'  ");
            if (!string.IsNullOrEmpty(mbSupply.Style))
            {
                queryBuilder.Append(" and style " + SetOperator(mbSupply.Style, false) + " ");
            }
            if (!string.IsNullOrEmpty(mbSupply.Color))
            {
                queryBuilder.Append(" and color  " + SetOperator(mbSupply.Color, false) + " ");
            }
            if (!string.IsNullOrEmpty(mbSupply.Attribute))
            {
                queryBuilder.Append(" and attribute_cd  " + SetOperator(mbSupply.Attribute, false) + " ");
            }
            if (!string.IsNullOrEmpty(mbSupply.SizeCD) && mbSupply.SizeCD != "ALL" && mbSupply.SizeCD != "AL")
            {
                queryBuilder.Append(" and i.size_cd " + SetOperator(mbSupply.SizeCD, true) + " ");
            }
            if (!string.IsNullOrEmpty(mbSupply.DC))
            {
                queryBuilder.Append(" and plant_no " + SetOperator(mbSupply.DC, false) + " ");
            }
            queryBuilder.Append(" and i.size_cd =s.size_cd ");
            queryBuilder.Append(" union all ");
            //IN TRANSIT
            queryBuilder.Append(" select d.style_cd Style,d.color_cd Color,d.attribute_cd Attribute,d.size_cd SizeCD, s.size_short_desc,d.demand_loc PlantNo,d.curr_order_qty QtyOnHand, d.curr_due_date SDate, '" + LOVConstants.PABTypes.InTransit + "' TranType ");
            queryBuilder.Append(" from iss_prod_order_detail d,iss_prod_order p,item_size s where d.order_version = 1 and p.order_version = d.order_version and p.super_order = d.order_label ");
            queryBuilder.Append(" and p.order_source_cd in ('MITS','SAP')");
            if (!string.IsNullOrEmpty(mbSupply.Style))
            {
                queryBuilder.Append(" and d.style_cd " + SetOperator(mbSupply.Style, false) + " ");
            }
            if (!string.IsNullOrEmpty(mbSupply.Color))
            {
                queryBuilder.Append(" and d.color_cd " + SetOperator(mbSupply.Color, false) + " ");
            }
            if (!string.IsNullOrEmpty(mbSupply.Attribute))
            {
                queryBuilder.Append(" and d.attribute_cd " + SetOperator(mbSupply.Attribute, false) + " ");
            }
            if (!string.IsNullOrEmpty(mbSupply.SizeCD) && mbSupply.SizeCD != "ALL" && mbSupply.SizeCD != "AL")
            {
                queryBuilder.Append(" and d.size_cd " + SetOperator(mbSupply.SizeCD, true) + " ");
            }
            if (!string.IsNullOrEmpty(mbSupply.DC))
            {
                queryBuilder.Append(" and d.demand_loc " + SetOperator(mbSupply.DC, false) + " ");
            }
            queryBuilder.Append(" and d.size_cd =s.size_cd ");
            queryBuilder.Append(" union all ");
            //WIP

            queryBuilder.Append(" select d.style_cd Style,d.color_cd Color,d.attribute_cd Attribute,d.size_cd SizeCD, s.size_short_desc ,d.demand_loc PlantNo,d.curr_order_qty QtyOnHand, d.curr_due_date SDate, '" + LOVConstants.PABTypes.WIP + "' TranType ");
            queryBuilder.Append(" from iss_prod_order_detail d,iss_prod_order p,item_size s where d.order_version = 1 and p.order_version = d.order_version and p.super_order = d.order_label ");
            queryBuilder.Append("and p.order_source_cd in ('XWIP')");
            if (!string.IsNullOrEmpty(mbSupply.Style))
            {
                queryBuilder.Append(" and d.style_cd " + SetOperator(mbSupply.Style, false) + " ");
            }
            if (!string.IsNullOrEmpty(mbSupply.Color))
            {
                queryBuilder.Append(" and d.color_cd " + SetOperator(mbSupply.Color, false) + " ");
            }
            if (!string.IsNullOrEmpty(mbSupply.Attribute))
            {
                queryBuilder.Append(" and d.attribute_cd " + SetOperator(mbSupply.Attribute, false) + " ");
            }
            if (!string.IsNullOrEmpty(mbSupply.SizeCD) && mbSupply.SizeCD != "ALL" && mbSupply.SizeCD != "AL")
            {
                queryBuilder.Append(" and d.size_cd " + SetOperator(mbSupply.SizeCD, true) + " ");
            }
            if (!string.IsNullOrEmpty(mbSupply.DC))
            {
                queryBuilder.Append(" and d.demand_loc " + SetOperator(mbSupply.DC, false) + " ");
            }
            queryBuilder.Append(" and d.size_cd =s.size_cd ");
            queryBuilder.Append(" union all ");
            //RELEASED,LOCKED & SUGGESTED
            queryBuilder.Append(" select COMP_STYLE Style,COMP_COLOR Color, comp_attribute_cd Attribute,comp_size_cd SizeCD, s.size_short_desc,comp_demand_loc PlantNo,a.allocation_qty QtyOnHand,a.due_date SDate,decode(p.production_status,'R','" + LOVConstants.PABTypes.Released + "','L','" + LOVConstants.PABTypes.Locked + "','" + LOVConstants.PABTypes.Suggested + "') TranType ");
            queryBuilder.Append(" from iss_prod_order_allocation a, iss_prod_order p,item_size s  where a.order_version =1 and  p.order_version = a.order_version and p.super_order = a.super_order  ");
            if (!string.IsNullOrEmpty(mbSupply.Style))
            {
                queryBuilder.Append(" and COMP_STYLE " + SetOperator(mbSupply.Style, false) + " ");
            }
            if (!string.IsNullOrEmpty(mbSupply.Color))
            {
                queryBuilder.Append(" and COMP_COLOR " + SetOperator(mbSupply.Color, false) + " ");
            }
            if (!string.IsNullOrEmpty(mbSupply.Attribute))
            {
                queryBuilder.Append(" and comp_attribute_cd " + SetOperator(mbSupply.Attribute, false) + " ");
            }
            if (!string.IsNullOrEmpty(mbSupply.SizeCD) && mbSupply.SizeCD != "ALL" && mbSupply.SizeCD != "AL")
            {
                queryBuilder.Append(" and comp_size_cd " + SetOperator(mbSupply.SizeCD, true) + " ");
            }
            if (!string.IsNullOrEmpty(mbSupply.DC))
            {
                queryBuilder.Append(" and comp_demand_loc " + SetOperator(mbSupply.DC, false) + " ");
            }
            queryBuilder.Append(" and comp_size_cd =s.size_cd ");
            //TODO pass fiscal_week fiscal_year which is hardcoded
            queryBuilder.Append(")x , fiscal_calendar f   where ");
            queryBuilder.Append(" ((fiscal_week< '" + mbSupply.BeginWeek + "'");
            queryBuilder.Append(" and fiscal_year<='" + mbSupply.BeginYear + "')");
            queryBuilder.Append(" or (fiscal_week>= '" + mbSupply.BeginWeek + "'");
            queryBuilder.Append(" and fiscal_year='" + mbSupply.BeginYear + "')");
            queryBuilder.Append(" or (fiscal_week<='" + mbSupply.EndWeek + "'");
            queryBuilder.Append(" and fiscal_year='" + mbSupply.EndYear + "')) ");
            queryBuilder.Append("and  WEEK_BEGIN_DATE=calendar_date ");
            queryBuilder.Append("and x.SDate between WEEK_BEGIN_DATE  and WEEK_END_DATE ");
            queryBuilder.Append("group by  Style,Color,Attribute,SizeCD,size_short_desc,PlantNo,fiscal_week,fiscal_year,WEEK_BEGIN_DATE,TranType ");
            queryBuilder.Append("order by   PlantNo,SizeCD,WeekBegDate");

            IDataReader reader = ExecuteReader(queryBuilder.ToString());

            var result = (new DbHelper()).ReadData<MaterialPAB>(reader);
            return result;
        }

        public List<MaterialPAB> MaterialSupplyGetWeeks()
        {
            ISS.Repository.Common.ApplicationRepository appRepo = new ISS.Repository.Common.ApplicationRepository();
            var lstWeeks = new List<MaterialPAB>();

            var BeginEndWeeks = appRepo.GetPlanBeginEndDates();
            if (BeginEndWeeks.Count > 0)
            {
                PlanWeek planWk = BeginEndWeeks[0];
                DateTime dtBeginDate = planWk.Week_Begin_Date;
                DateTime dtEndDate = planWk.Week_End_Date;
                decimal FYear = planWk.Fiscal_Year;
                MaterialPAB mPab = null;
                for (decimal wk = 0; wk < 52; wk++)
                {
                    mPab = new MaterialPAB();
                    mPab.FiscalWeek = (planWk.Fiscal_Week + wk) % 52;
                    if (planWk.Fiscal_Week > 1 && mPab.FiscalWeek == 1)
                        FYear++;
                    
                    if (mPab.FiscalWeek == 0) mPab.FiscalWeek = 52;
                    mPab.FiscalWeekStr = "Wk" + mPab.FiscalWeek.ToString();
                    mPab.FiscalWeekTitle = mPab.FiscalWeekStr + " <br />" + dtBeginDate.ToShortDateString();
                    mPab.FiscalYear = FYear;
                    mPab.WeekBegDate = dtBeginDate;
					mPab.WeekEndDate = dtEndDate;
                    lstWeeks.Add(mPab);
                    dtBeginDate = dtBeginDate.AddDays(7);
                    dtEndDate = dtEndDate.AddDays(7);
                }
            }


            return lstWeeks;
        }
        public List<MaterialBlankSupplySearch> GetDC()
        {
            var query = new StringBuilder();


            //query.Append("select distinct plant_cd DC from da.plant where distribution_ind='" + LOVConstants.Yes + "' ");
            query.Append("select distinct plant_cd DC from da.plant ");
            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<MaterialBlankSupplySearch>(reader);
            return result;
        }
    }
}
