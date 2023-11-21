using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISS.Core.Model.Order;
using ISS.DAL;
using ISS.Common;
using System.Data;
using ISS.Common;
using ISS.Core.Model.Common;
using ISS.Repository.Common;

namespace ISS.Repository.Order
{
    public class SummaryRepository : RepositoryBase
    {

        #region  Order Summary

        public List<SummaryResult> GetSummary(SummaryFilterModal filterModal)
        {

            ApplicationRepository repositoryPlanner = new ApplicationRepository();
            var result = repositoryPlanner.GetPlanBeginEndDates();

            var beginweek = result.Select(x => x.Week_Begin_Date).FirstOrDefault();
            var endweek = result.Select(x => x.Week_End_Date).FirstOrDefault();

            filterModal.WeekBeginDate = beginweek;
            filterModal.WeekEndDate = endweek;
            //filterModal.SuggWO = 53;
            //filterModal.ExcessSuggWOGrtr2 = true;
            //filterModal.ExcessSuggSpillover = true;

            var groups = GetSummaryGroups(filterModal);
            if (groups.Count > 0)
            {
                var calculation = GetSummarycalculation(filterModal);
                //if (calculation.Count > 0)
                {

                    SummaryResult lastItem = null;
                    SummaryResult nextItem = null;
                    //SummaryResult newItem = null;
                    //if(filterModal.SkuBreaks)
                    //{
                    //    newItem = new SummaryResult();
                    //}

                    for (int i = 0; i < groups.Count; i++)
                    {
                        var item = groups[i];

                        // SKU changed
                        if (lastItem != null && lastItem.sku != item.sku)
                        {
                            lastItem.SkuBreakRow = true;
                            lastItem = null;
                        }

                        if (lastItem != null && filterModal.DisplayAs == LOVConstants.SummaryDetailedByrev)
                        {
                            if (lastItem.sku == item.sku && lastItem.rev_no != item.rev_no)
                            {
                                lastItem.SkuBreakRow = true;
                                lastItem = null;
                            }
                        }

                        if (i == groups.Count - 1)
                        {
                            item.SkuBreakRow = true;
                        }

                        List<SummaryResult> detail = null;
                        if (filterModal.DisplayAs == LOVConstants.SummaryDetailedByrev)
                        {
                            detail = calculation.Where(e => e.sku == item.sku && e.rev_no == item.rev_no && e.rule_number == item.rule_number).ToList();
                        }
                        else
                        {
                            detail = calculation.Where(e => e.sku == item.sku && e.rule_number == item.rule_number).ToList();
                        }

                        if (i + 1 < groups.Count)
                        {
                            nextItem = groups[i + 1];
                        }
                        else
                        {
                            nextItem = null;
                        }
                        CalculateSummaryData(item, detail, lastItem, filterModal, nextItem);


                        lastItem = item;

                        bool need9999 = false;


                        if (nextItem != null && nextItem.sku != item.sku)
                        {
                            need9999 = true;
                        }

                        else if (i + 1 == groups.Count) need9999 = true;

                        if (need9999)
                        {
                            var calculation99999 = calculation.Where(e => e.sku == item.sku && e.rule_number == 9999).ToList();
                            if (calculation99999.Count > 0)
                            {
                                var item9999 = calculation99999[0];
                                item9999.planner_cd = item.planner_cd;
                                item9999.cut_alloc = item.cut_alloc;
                                item9999.selling_style_cd = item.selling_style_cd;
                                item9999.selling_color_cd = item.selling_color_cd;
                                item9999.selling_size_cd = item.selling_size_cd;
                                item9999.size_short_desc = item.size_short_desc;
                                item9999.selling_attribute_cd = item.selling_attribute_cd;
                                item9999.demand_source = "Create Lots";
                                item9999.AttributionInd = item.AttributionInd;
                                CalculateSummaryData(item9999, calculation99999, lastItem, filterModal);
                                groups.Insert(i + 1, item9999);
                                i++;
                                lastItem = item9999;
                            }
                        }

                    } // End groups foreach


                } // end calculations count
            } // End groups count

            IEnumerable<SummaryResult> filteredList = groups;

            if (
                filterModal.ReleasedLots ||
                filterModal.LockedOrders ||
                filterModal.BuyOrders ||
                filterModal.SuggWOWK1 ||
                filterModal.SuggWOWK2 ||
                filterModal.SuggWOWK2Grtr2 ||
                filterModal.SpillOver
                )
                filteredList = filteredList.Where(p =>
                    ((filterModal.ReleasedLots) ? p.Released > 0 : false) ||
                    ((filterModal.LockedOrders) ? p.Locked > 0 : false) ||
                    ((filterModal.BuyOrders) ? p.BuyOrders > 0 : false) ||
                    ((filterModal.SuggWOWK1) ? p.SugWK1 > 0 : false) ||
                    ((filterModal.SuggWOWK2) ? p.SugWK2 > 0 : false) ||
                    ((filterModal.SuggWOWK2Grtr2) ? p.SugWK3Plus > 0 : false) ||
                    ((filterModal.SpillOver) ? p.SpillOver > 0 : false)
                    );

            if (!String.IsNullOrWhiteSpace(filterModal.RuleNo))
            {
                filteredList = filteredList.Where(f => f.rule_number.ToString().Equals(filterModal.RuleNo));
            }
            if (!String.IsNullOrWhiteSpace(filterModal.RuleDesc))
            {
                filteredList = filteredList.Where(f => (f.demand_source.ToUpper() + String.Empty).Equals(filterModal.RuleDesc.ToUpper()));
            }
            if (filteredList != null) groups = filteredList.ToList();

            groups.ForEach(f =>
            {
                f.Excess = (f.Excess / 12).RoundCustom(0);
                f.ExcessLot = (f.ExcessLot).RoundCustom(0);
                f.ExcessNetDemand = (f.ExcessNetDemand).RoundCustom(0);
                f.TotalNetDemand = (f.TotalNetDemand / 12).RoundCustom(0);
                f.LockOrReleaseBal = (f.LockOrReleaseBal / 12).RoundCustom(0);
                f.Released = (f.Released / 12).RoundCustom(0);
                f.Locked = (f.Locked / 12).RoundCustom(0);
                f.BuyOrders = (f.BuyOrders / 12).RoundCustom(0);
                f.SugWK1 = (f.SugWK1 / 12).RoundCustom(0);
                f.SugWK2 = (f.SugWK2 / 12).RoundCustom(0);
                f.SugWK3Plus = (f.SugWK3Plus / 12).RoundCustom(0);
                f.SpillOver = (f.SpillOver / 12).RoundCustom(0);
            });

            return groups;
        }
        private decimal rule = 9902;
        private void CalculateSummaryData(SummaryResult item, List<SummaryResult> detail, SummaryResult lastItem, SummaryFilterModal filterModal, SummaryResult nextItem = null)
        {
            //if (detail.Count > 0)
            {

                item.Released = detail.Where(e =>
                    e.production_status == LOVConstants.ProductionStatus.Released
                    && (e.make_or_buy_cd == LOVConstants.MakeOrBuy.Make || e.make_or_buy_cd == LOVConstants.MakeOrBuy.Buy)
                    ).Sum(e => e.curr_order_qty);


                item.Locked = detail.Where(e =>
                   e.production_status == LOVConstants.ProductionStatus.Locked
                   && (e.make_or_buy_cd == LOVConstants.MakeOrBuy.Make || e.make_or_buy_cd == LOVConstants.MakeOrBuy.Buy)
                   ).Sum(e => e.curr_order_qty);




                item.SpillOver = detail.Where(e =>
                         e.spill_over_ind == LOVConstants.SpillOver.Yes
                          && (e.make_or_buy_cd == LOVConstants.MakeOrBuy.Make || e.make_or_buy_cd == LOVConstants.MakeOrBuy.Buy)
                         ).Sum(e => e.curr_order_qty);


                item.SugWK1 = detail.Where(e =>
                (
                  e.production_status != LOVConstants.ProductionStatus.Locked &&
                  e.production_status != LOVConstants.ProductionStatus.Released
                )
                   && (e.spill_over_ind == LOVConstants.SpillOver.No)
                   && (e.make_or_buy_cd == LOVConstants.MakeOrBuy.Make || e.make_or_buy_cd == LOVConstants.MakeOrBuy.Buy)
                   && (e.weekord < 2)
                ).Sum(e => e.curr_order_qty);

                item.SugWK2 = detail.Where(e =>
                 (
                   e.production_status != LOVConstants.ProductionStatus.Locked &&
                   e.production_status != LOVConstants.ProductionStatus.Released
                 )
                 && e.spill_over_ind == LOVConstants.SpillOver.No
                 && (e.make_or_buy_cd == LOVConstants.MakeOrBuy.Make || e.make_or_buy_cd == LOVConstants.MakeOrBuy.Buy)
                 && e.weekord == 2
                 ).Sum(e => e.curr_order_qty);


                item.SugWK3Plus = detail.Where(e =>
                 (
                   e.production_status != LOVConstants.ProductionStatus.Locked &&
                   e.production_status != LOVConstants.ProductionStatus.Released
                 )
                 && e.spill_over_ind == LOVConstants.SpillOver.No
                 && (e.make_or_buy_cd == LOVConstants.MakeOrBuy.Make || e.make_or_buy_cd == LOVConstants.MakeOrBuy.Buy)
                 && e.weekord > 2 && (item.demand_source == "Create Lots" || e.weekord <= item.TimeFence) && e.weekord <= filterModal.SuggWO
                 && e.weekord < item.TimeFence
                 ).Sum(e => e.curr_order_qty);





                if (detail.Any(w => w.make_or_buy_cd == LOVConstants.MakeOrBuy.Buy))
                {
                    item.BuyOrders = item.SugWK1 + item.SugWK2 + item.SugWK3Plus;
                    item.SugWK1 = item.SugWK2 = item.SugWK3Plus = 0;
                    if (item.BuyOrders > 0)
                    {
                        var tt = 0;
                    }
                }

                item.Excess = ((((item.Released + item.BuyOrders + item.Locked + item.SugWK1
                      + item.SugWK2 +
                    (filterModal.ExcessSuggWOGrtr2 ? item.SugWK3Plus : 0) +
                    (filterModal.ExcessSuggSpillover ? item.SpillOver : 0))
                    - item.TotalNetDemand)) + ((lastItem != null) ? lastItem.Excess : 0M)
                    );
                if (item.Excess < 0) item.Excess = 0;

                var lotList = detail.Where(e =>
                    e.spill_over_ind == (filterModal.ExcessSuggSpillover ? e.spill_over_ind : LOVConstants.SpillOver.No) &&
                    (filterModal.ExcessSuggWOGrtr2 ? (e.weekord <= filterModal.SuggWO) : (e.weekord <= 2))).ToList();

                if (lotList.Count > 0)
                    item.lotSize = lotList.Average(e => e.lotSize) / 12.0m;
                else
                {
                    if (lastItem != null) item.lotSize = lastItem.lotSize;
                }

                //Excess
                var ExcessDz = (item.Excess / LOVConstants.Dozen).RoundCustom(0);
                //var Excess = ExcessDz  * LOVConstants.Dozen;


                //Exces % LOT
                if (item.lotSize != 0)
                    item.ExcessLot = (ExcessDz / item.lotSize) * 100.0M; //***


                // Excess % Net DMD
                if (((item.TotalNetDemand / LOVConstants.Dozen).RoundCustom(0)) != 0)
                {
                    item.ExcessNetDemand = ((ExcessDz) / ((item.TotalNetDemand / LOVConstants.Dozen).RoundCustom(0))) * 100;
                    if (item.ExcessNetDemand < 0) item.ExcessNetDemand = 0;
                }


                //1st Row: Total Net Demand - (Released + Locked) 									
                //2nd Row and below: Bal to Lock/Release from previous row + Total Net Demand - (Released + Locked)	 
                item.LockOrReleaseBal = item.TotalNetDemand - item.Released - item.Locked;
                if (item.LockOrReleaseBal < 0) item.LockOrReleaseBal = 0;
                if (lastItem != null && lastItem.sku == item.sku)
                {
                    item.LockOrReleaseBal = lastItem.LockOrReleaseBal / ((lastItem.Released == 0) ? 1 : lastItem.Released) + item.TotalNetDemand - item.Released - item.Locked;  //***
                    if (item.LockOrReleaseBal < 0) item.LockOrReleaseBal = 0;
                }

                if (detail.Where(e => e.curr_order_qty > 0).Count() == 0)
                {
                    item.SuggestedLotsComments = "No Planned Orders";

                }
                if (item.Excess > 0)
                {
                    if (nextItem != null && nextItem.sku == item.sku)
                    {
                        if (lastItem == null || lastItem.sku != item.sku)
                        {
                            item.SuggestedLotsComments = String.Empty;
                        }
                        else
                            item.SuggestedLotsComments = "Covered By Lot Size";
                    }
                    else if (nextItem == null || nextItem.sku != item.sku)
                    {
                        item.SuggestedLotsComments = "Excess to Demand";
                    }
                }
                //else
                //{
                //    item.SuggestedLotsComments = String.Empty;
                //}
            }

        }

        // Excess	((Release + Locked+Buy+Sug WK 1+Sug WK 2 +Sug>Wk2 + Spillover) - Total Net Dmd)/12   + Excess from previous row------- to represent the number in Dozens				
        //    if result is negative, then display 0, and the cumulative is reset to 0				
        //    NOTE: Only Include Sug WK>2 and/or Spillover if the boxes are checked)	

        //Excess % Lot	(Excess /( (Release + Locked+Buy+Sug WK 1+Sug WK 2 +Sug>Wk2 + Spillover)/LotSize)) * 100				
        //    NOTE: Only Include Sug WK>2 and/or Spillover if the boxes are checked)	

        //Excess % Net Dmd	(Excess / Total Net Demand) * 100				

        //Bal to Lock/Rel	1st Row: Total Net Demand - (Released + Locked) 				
        //    2nd Row and below: Bal to Lock/Release from previous row + Total Net Demand - (Released + Locked)	

        //Comments	If record found in SQL1 but not in SQL2 = "No Planned Orders"				
        //    if Excess > 0 and next record is the same SKU, "Covered By Lot Size"				
        //    if Excess > 0 and last record then "Excess to Demand"				

        private List<SummaryResult> GetSummaryGroups(SummaryFilterModal filterModal)
        {
            var queryBuilder = new StringBuilder();
            var capFilter = false;
            var attrWCFilter = false;

            string hint = " /*+ CHOOSE */ ";

            string alloc = "";
            
            // Check for non attr capacity filtr
            if  (!string.IsNullOrEmpty(filterModal.WorkCenter ))
            {

                if (filterModal.CapacityGroup == CapacityGroup.Sew.ToString().ToUpper())
                {
                    alloc = "d.sew_alloc";
                    capFilter = true;
                
                }
                if (filterModal.CapacityGroup == CapacityGroup.Src.ToString().ToUpper())
                {
                    alloc = "d.src_alloc";
                    capFilter = true;
                }
                if (filterModal.CapacityGroup == CapacityGroup.Cut.ToString().ToUpper())
                {
                    alloc = "d.cut_alloc";
                    capFilter = true;
                }
                if (filterModal.CapacityGroup == CapacityGroup.Tex.ToString().ToUpper())
                {
                    alloc = "d.fin_alloc";
                    capFilter = true;
                }
                attrWCFilter  = !capFilter;
            }

            


            queryBuilder.Append("select "
                   + (hint + (" d.planner_cd \"planner_cd\", "
                + (" d.rule_number, d.demand_source, "))));
            // + (alloc + " \"cut_alloc\", d.rule_number, d.demand_source, "))));



            queryBuilder.Append("d.selling_style_cd, d.selling_color_cd, d.selling_attribute_cd, d.selling_size_cd, i.size_short_desc, ");
            queryBuilder.Append("d.planning_tm_fnc \"TimeFence\", sum(d.current_qty) \"TotalNetDemand\", ");
            if (filterModal.DisplayAs == "Detail by Revision")
            {
                queryBuilder.Append("d.rev_no \"rev_no\" ,d.style ");
            }
            else
            {
                queryBuilder.Append(" 0 rev_no, \'\' style ");
            }
            queryBuilder.Append(" ,d.selling_style_cd||' '||d.selling_color_cd||' '||d.selling_attribute_cd||' '||d.selling_size_cd sku, d.attribution_ind  \"AttributionInd\" ");
            queryBuilder.Append("from avyx_demand_net_view d,  item_size i , style s  ");
            queryBuilder.Append("where d.selling_size_cd = i.size_cd and d.selling_style_cd = s.style_cd  ");

            if (!String.IsNullOrWhiteSpace(filterModal.Planner))
            {
                queryBuilder.Append("and d.planner_cd  in (" + FormatInClause(filterModal.Planner) + ")");
            }
            if (!String.IsNullOrWhiteSpace(filterModal.CorpDiv))
            {
                queryBuilder.Append("and s.corp_division_Cd  in (" + FormatInClause(filterModal.CorpDiv) + ")");
            }

            if (attrWCFilter)
                queryBuilder.Append(" and d.selling_attribute_cd <> '------'");

            if (!String.IsNullOrWhiteSpace(filterModal.WorkCenter) && capFilter)
            {
                queryBuilder.Append(" and " + alloc + " in (" + FormatInClause(filterModal.WorkCenter) + ")");
            }
            if (!String.IsNullOrWhiteSpace(filterModal.Style))
            {
                queryBuilder.Append("and d.selling_style_cd like '" + (filterModal.Style) + "%' ");
            }
            if (!String.IsNullOrWhiteSpace(filterModal.Color) && filterModal.Color != "ALL")
            {
                queryBuilder.Append("and d.selling_color_cd like '" + (filterModal.Color) + "%' ");
            }
            if (!String.IsNullOrWhiteSpace(filterModal.Attribute))
            {
                queryBuilder.Append("and d.selling_attribute_cd in (" + FormatInClause(filterModal.Attribute) + ") ");
            }
            if (!String.IsNullOrWhiteSpace(filterModal.Size) && filterModal.Size != "AL")
            {
                queryBuilder.Append("and i.size_short_desc like '" + (filterModal.Size) + "%' ");
            }
            //if (!String.IsNullOrWhiteSpace(filterModal.RuleNo))
            //{
            //    queryBuilder.Append("and d.rule_number like \'" + (filterModal.RuleNo) + "\' ");
            //}
            //if (!String.IsNullOrWhiteSpace(filterModal.RuleDesc))
            //{
            //    queryBuilder.Append("and UPPER(d.demand_source) like \'" + (filterModal.RuleDesc) + "%\' ");
            //}
            queryBuilder.Append("group by d.planner_cd, d.selling_style_cd, d.selling_color_cd, ");
            // +(alloc + ", d.selling_style_cd, d.selling_color_cd, "));
            queryBuilder.Append("d.selling_attribute_cd, d.selling_size_cd, i.size_short_desc, ");
            queryBuilder.Append("d.rule_number, d.demand_source, d.planning_tm_fnc, d.attribution_ind, ");
            if (filterModal.DisplayAs == "Detail by Revision")
            {
                queryBuilder.Append("d.rev_no , d.style");
            }
            else
            {
                queryBuilder.Append("0 , \'\' ");
            }



            string baseSql = queryBuilder.ToString();
            queryBuilder.Clear();

            alloc = "x.cut_alloc";

            if (!string.IsNullOrEmpty(filterModal.WorkCenter)) {
                if (filterModal.CapacityGroup.ToLower().Trim() == CapacityGroup.Sew.ToString().ToLower())
                {
                    alloc = "x.sew_alloc";
                }
                if (filterModal.CapacityGroup.ToLower().Trim() == CapacityGroup.Src.ToString().ToLower())
                {
                    alloc = "x.src_alloc";
                }
                if (filterModal.CapacityGroup.ToLower().Trim() == CapacityGroup.Tex.ToString().ToLower())
                {
                    alloc = "x.fin_alloc";
                }
                if (filterModal.CapacityGroup.ToLower().Trim() == CapacityGroup.Cut.ToString().ToLower())
                {
                    alloc = "x.cut_alloc";
                }
            }

            if (attrWCFilter )
            {
                alloc = " decode(upper(trim(ATR_GROUP_1)),'" + filterModal.CapacityGroup.ToUpper().Trim() + "', x.atr_alloc_1,";
                alloc += " upper(trim(ATR_GROUP_2)),'" + filterModal.CapacityGroup.ToUpper().Trim() +"', x.atr_alloc_2,x.atr_alloc_3) ";
            }


            queryBuilder.Append("with demand as (" + baseSql + ") , demand_bu as (");
            queryBuilder.Append("select a.*,nvl(x1.bu_style, a.selling_style_cd) s");
            queryBuilder.Append(", nvl(x1.bu_color, a.selling_color_cd) c");
            queryBuilder.Append(", decode(x1.bu_color, null, a.selling_attribute_cd, '------') attr");
            queryBuilder.Append(", nvl(x1.bu_size_cd, a.selling_size_cd) sz");
            queryBuilder.Append("  from demand a");
            queryBuilder.Append(" left join  bu_sku_xref x1 on a.selling_style_cd = x1.champ_style");
            queryBuilder.Append(" and a.selling_color_cd = x1.champ_color");
            queryBuilder.Append(" and a.selling_attribute_cd = x1.champ_attribute_cd");
            queryBuilder.Append(" and a.selling_size_cd = x1.champ_size_cd)");
            queryBuilder.Append(" select d.\"planner_cd\",");
            queryBuilder.Append(" d.rule_number,");
            queryBuilder.Append(" d.demand_source, ");
            queryBuilder.Append(" d.selling_style_cd,");
            queryBuilder.Append(" d.selling_color_cd, ");
            queryBuilder.Append(" d.selling_attribute_cd, ");
            queryBuilder.Append(" d.selling_size_cd, ");
            queryBuilder.Append(" d.size_short_desc, ");
            queryBuilder.Append(" d.\"TimeFence\", ");
            queryBuilder.Append(" d.\"TotalNetDemand\", ");
            queryBuilder.Append(" 0 rev_no, ");
            queryBuilder.Append(" '' style ,");
            queryBuilder.Append(" d.sku, ");
            queryBuilder.Append(" d.\"AttributionInd\", ");
            queryBuilder.Append(alloc + " cut_alloc");
            queryBuilder.Append(" from demand_bu d left join ");
            queryBuilder.Append(" da.iss_cap_alloc_xref x on d.s = x.style_cd");
            queryBuilder.Append(" and d.attr = x.attribute_cd");

            // US 56914 Add in WorkCenter filters
            if (!String.IsNullOrWhiteSpace(filterModal.WorkCenter) && attrWCFilter)
            {
                queryBuilder.Append(" where ( ATR_ALLOC_1 in (" + FormatInClause(filterModal.WorkCenter) + ")  ");
                queryBuilder.Append(" or  ATR_ALLOC_2 in (" + FormatInClause(filterModal.WorkCenter) + ")  ");
                queryBuilder.Append(" or ATR_ALLOC_3 in (" + FormatInClause(filterModal.WorkCenter) + ") ) ");
                
            }


            if (filterModal.DisplayAs == "Detail by Revision")
            {
                queryBuilder.Append(" order by selling_style_cd,selling_color_cd,selling_size_cd,rev_no,rule_number ");
            }
            else
            {
                //queryBuilder.Append(" order by selling_style_cd,selling_color_cd,selling_size_cd ");
                queryBuilder.Append(" ORDER BY d.\"planner_cd\", x.cut_alloc, selling_style_cd, selling_color_cd, selling_size_cd , d.rule_number ");
            }

            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            IDataReader reader = ExecuteReader(queryBuilder.ToString());

            var result = (new DbHelper()).ReadData<SummaryResult>(reader);
            return result;
        
        }



        private List<SummaryResult> GetSummaryGroupsOld(SummaryFilterModal filterModal)
        {
            var queryBuilder = new StringBuilder();

            string hint = " /*+ CHOOSE */ ";

            string alloc = "";
            alloc = "d.cut_alloc";
            if (filterModal.CapacityGroup == CapacityGroup.Sew.ToString())
            {
                alloc = "d.sew_alloc";
            }
            if (filterModal.CapacityGroup == CapacityGroup.Src.ToString())
            {
                alloc = "d.src_alloc";
            }

            //alloc = "d.sew_alloc";

            queryBuilder.Append("select "
                   + (hint + (" d.planner_cd \"planner_cd\", "
                   + (alloc + " \"cut_alloc\", d.rule_number, d.demand_source, "))));


            queryBuilder.Append("d.selling_style_cd, d.selling_color_cd, d.selling_attribute_cd, d.selling_size_cd, i.size_short_desc, ");
            queryBuilder.Append("d.planning_tm_fnc \"TimeFence\", sum(d.current_qty) \"TotalNetDemand\", ");
            if (filterModal.DisplayAs == "Detail by Revision")
            {
                queryBuilder.Append("d.rev_no \"rev_no\" ,d.style ");
            }
            else
            {
                queryBuilder.Append(" 0 rev_no, \'\' style ");
            }
            queryBuilder.Append(" ,d.selling_style_cd||' '||d.selling_color_cd||' '||d.selling_attribute_cd||' '||d.selling_size_cd sku, d.attribution_ind  \"AttributionInd\" ");
            queryBuilder.Append("from avyx_demand_net_view d,  item_size i , style s  ");
            queryBuilder.Append("where d.selling_size_cd = i.size_cd and d.selling_style_cd = s.style_cd  ");

            if (!String.IsNullOrWhiteSpace(filterModal.Planner))
            {
                queryBuilder.Append("and d.planner_cd  in (" + FormatInClause(filterModal.Planner) + ")");
            }
            if (!String.IsNullOrWhiteSpace(filterModal.CorpDiv))
            {
                queryBuilder.Append("and s.corp_division_Cd  in (" + FormatInClause(filterModal.CorpDiv) + ")");
            }
            if (!String.IsNullOrWhiteSpace(filterModal.WorkCenter))
            {
                queryBuilder.Append(" and " + alloc + " in (" + FormatInClause(filterModal.WorkCenter) + ")");
            }
            if (!String.IsNullOrWhiteSpace(filterModal.Style))
            {
                queryBuilder.Append("and d.selling_style_cd like '" + (filterModal.Style) + "%' ");
            }
            if (!String.IsNullOrWhiteSpace(filterModal.Color) && filterModal.Color != "ALL")
            {
                queryBuilder.Append("and d.selling_color_cd like '" + (filterModal.Color) + "%' ");
            }
            if (!String.IsNullOrWhiteSpace(filterModal.Attribute))
            {
                queryBuilder.Append("and d.selling_attribute_cd in (" + FormatInClause(filterModal.Attribute) + ") ");
            }
            if (!String.IsNullOrWhiteSpace(filterModal.Size) && filterModal.Size != "AL")
            {
                queryBuilder.Append("and i.size_short_desc like '" + (filterModal.Size) + "%' ");
            }
            //if (!String.IsNullOrWhiteSpace(filterModal.RuleNo))
            //{
            //    queryBuilder.Append("and d.rule_number like \'" + (filterModal.RuleNo) + "\' ");
            //}
            //if (!String.IsNullOrWhiteSpace(filterModal.RuleDesc))
            //{
            //    queryBuilder.Append("and UPPER(d.demand_source) like \'" + (filterModal.RuleDesc) + "%\' ");
            //}
            queryBuilder.Append("group by d.planner_cd, "
                        + (alloc + ", d.selling_style_cd, d.selling_color_cd, "));
            queryBuilder.Append("d.selling_attribute_cd, d.selling_size_cd, i.size_short_desc, ");
            queryBuilder.Append("d.rule_number, d.demand_source, d.planning_tm_fnc, d.attribution_ind, ");
            if (filterModal.DisplayAs == "Detail by Revision")
            {
                queryBuilder.Append("d.rev_no , d.style");
            }
            else
            {
                queryBuilder.Append("0 , \'\' ");
            }


            if (filterModal.DisplayAs == "Detail by Revision")
            {
                queryBuilder.Append(" order by selling_style_cd,selling_color_cd,selling_size_cd,rev_no,rule_number ");
            }
            else
            {
                queryBuilder.Append(" order by selling_style_cd,selling_color_cd,selling_size_cd ");
            }

            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            IDataReader reader = ExecuteReader(queryBuilder.ToString());

            var result = (new DbHelper()).ReadData<SummaryResult>(reader);
            return result;
        }

        private List<SummaryResult> GetSummarycalculation(SummaryFilterModal filterModal)
        {
            var queryBuilder = new StringBuilder();
            if (filterModal.DisplayAs == "Detail by Revision")
            {
                queryBuilder.Append("SELECT  /*+ CHOOSE */    selling_style_cd||' '||selling_color_cd||' '||selling_attribute_cd||' '||selling_size_cd \"sku\" ,rule_number,spill_over_ind     , (((c1.week_end_date - 1) - (c2.week_end_date - 1))/7)+1 \"weekord\", SUM(curr_order_qty) \"curr_order_qty\", nvl(MFG_REVISION_NO,-1) \"rev_no\" ,o.production_status, DECODE(make_or_buy_cd,'B','B','M') \"make_or_buy_cd\",COUNT (o.super_order), DECODE(o.remote_update_cd, 'F', COUNT(o.remote_update_cd),0) AS \"RFailed\", TRUNC( SUM(curr_order_qty) / COUNT(o.Super_order),0)  \"lotSize\"  " +
                        "FROM iss_prod_order_view o ,hbidf_fiscal_calendar c1, hbi_fiscal_calendar c2, hbi_fiscal_calendar c3 ");
            }
            else
            {
                queryBuilder.Append("SELECT    /*+ CHOOSE */   selling_style_cd||' '||selling_color_cd||' '||selling_attribute_cd||' '||selling_size_cd \"sku\" ,rule_number,spill_over_ind     , (((c1.week_end_date - 1) - (c2.week_end_date - 1))/7)+1 \"weekord\", SUM(curr_order_qty) \"curr_order_qty\", 0 \"rev_no\",o.production_status, DECODE(make_or_buy_cd,'B','B','M') \"make_or_buy_cd\",COUNT (o.super_order), DECODE(o.remote_update_cd, 'F', COUNT(o.remote_update_cd),0) AS \"RFailed\", TRUNC( SUM(curr_order_qty) / COUNT(o.Super_order),0)  \"lotSize\"  " +
                        "FROM iss_prod_order_view o ,hbi_fiscal_calendar c1, hbi_fiscal_calendar c2, hbi_fiscal_calendar c3 ");
            }

            //"WHERE      o.order_version = 1 AND o.order_status_cd <> '3' AND c1.calendar_date = TRUNC(o.earliest_start) AND c2.calendar_date = TRUNC(TO_DATE('20150405','YYYYMMDD'))  " +
            //         "AND c3.calendar_date = TRUNC(DECODE(o.production_status,'L',TO_DATE('20150329','YYYYMMDD'),'P',TO_DATE('20150329','YYYYMMDD'),o.remote_release_date)) AND c3.week_begin_date <=  TO_DATE('20150329','YYYYMMDD') " +
            //         "AND c3.week_end_date >=  TO_DATE('20150329','YYYYMMDD')  AND (o.order_version,o.super_order) IN ( SELECT  /*+ CHOOSE */  o1.order_version,o1.super_order   FROM iss_prod_order_detail o1,iss_prod_order p1   WHERE p1.order_version = o1.order_version AND p1.super_order = o1.super_order AND p1.planner_cd IN ('LISAM')  )  ");

            queryBuilder.Append("where  o.order_version = 1 and o.order_status_cd <> '3' and c1.calendar_date = trunc(o.earliest_start) ");

            if (filterModal.WeekEndDate.HasValue)
            {
                queryBuilder.Append("and c2.calendar_date = trunc(to_date('" + filterModal.WeekEndDate.Value.AddDays(1).ToString(LOVConstants.DateFormatOracle) + "','YYYYMMDD'))  ");
            }

            if (filterModal.WeekBeginDate.HasValue) //trunc(c3.calendar_date)
            {
                if (filterModal.DisplayAs == "Detail by Revision")
                {
                    queryBuilder.Append("and trunc(c3.calendar_date) = trunc(decode(o.production_status,'L',to_date('" + filterModal.WeekBeginDate.Value.ToString(LOVConstants.DateFormatOracle) + "','YYYYMMDD'),'P',to_date('" + filterModal.WeekBeginDate.Value.ToString(LOVConstants.DateFormatOracle) + "','YYYYMMDD'),o.remote_release_date)) ");
                }
                else
                {
                    queryBuilder.Append("and c3.calendar_date = trunc(decode(o.production_status,'L',to_date('" + filterModal.WeekBeginDate.Value.ToString(LOVConstants.DateFormatOracle) + "','YYYYMMDD'),'P',to_date('" + filterModal.WeekBeginDate.Value.ToString(LOVConstants.DateFormatOracle) + "','YYYYMMDD'),o.remote_release_date)) ");
                }
            }
            if (!String.IsNullOrWhiteSpace(filterModal.Style))
            {
                queryBuilder.Append("and selling_style_cd like '" + (filterModal.Style) + "%' ");
            }
            if (!String.IsNullOrWhiteSpace(filterModal.Color))
            {
                queryBuilder.Append("and selling_color_cd like '" + (filterModal.Color) + "%' ");
            }
            if (!String.IsNullOrWhiteSpace(filterModal.Attribute))
            {
                queryBuilder.Append("and selling_attribute_cd like '" + (filterModal.Attribute) + "%' ");
            }
            if (filterModal.WeekBeginDate.HasValue)
            {
                queryBuilder.Append("and c3.week_begin_date <=  to_date('" + filterModal.WeekBeginDate.Value.ToString(LOVConstants.DateFormatOracle) + "','YYYYMMDD') ");
            }
            if (filterModal.WeekBeginDate.HasValue)
            {
                queryBuilder.Append("and c3.week_end_date >=  to_date('" + filterModal.WeekBeginDate.Value.ToString(LOVConstants.DateFormatOracle) + "','YYYYMMDD') ");
            }
            if ((!String.IsNullOrWhiteSpace(filterModal.Planner)) || (!String.IsNullOrWhiteSpace(filterModal.Style)) || (!String.IsNullOrWhiteSpace(filterModal.WorkCenter)) || (!String.IsNullOrWhiteSpace(filterModal.CorpDiv)))
            {
                if (filterModal.DisplayAs == "Detail by Revision")
                {
                    queryBuilder.Append(" and (o.order_version,o.super_order) in ( select /*+ CHOOSE */ o1.order_version,o1.super_order  from iss_prod_order_detail o1,iss_prod_order p1,style s   where p1.order_version = o1.order_version and p1.super_order = o1.super_order and o1.selling_style_cd = s.style_cd ");
                }
                else
                {
                    queryBuilder.Append(" and (o.order_version,o.super_order) in ( select /*+ CHOOSE */ o1.order_version,o1.super_order  from iss_prod_order_detail o1,iss_prod_order p1,style s   where p1.order_version = o1.order_version and p1.super_order = o1.super_order and o1.selling_style_cd = s.style_cd ");
                }

                if (!String.IsNullOrWhiteSpace(filterModal.Planner))
                {
                    queryBuilder.Append("and p1.planner_cd in (" + FormatInClause(filterModal.Planner) + ") ");
                }
                if (!String.IsNullOrWhiteSpace(filterModal.CorpDiv))
                {
                    queryBuilder.Append("and s.corp_division_Cd  in (" + FormatInClause(filterModal.CorpDiv) + ")");
                }
                if (!String.IsNullOrWhiteSpace(filterModal.Style))
                {
                    queryBuilder.Append("and o1.selling_style_cd like '" + filterModal.Style + "%' ");
                }
                if (!String.IsNullOrWhiteSpace(filterModal.WorkCenter))
                {
                    // US 56914 ISS Web Capacity Group

                    string s = " and exists (select 'x' from iss_prod_order_capacity c where c.order_version = o1.order_version";
                    s += " and c.super_order = o1.super_order and c.ALLOC_GROUP  in  ( " + FormatInClause(filterModal.WorkCenter) + " )  ) ";
                    queryBuilder.Append(s);
                    //queryBuilder.Append(" and nvl(o1.capacity_alloc,'[CATCHALL]')  in (" + FormatInClause(filterModal.WorkCenter) + ") ");
                }
                queryBuilder.Append(" ) ");//
            }
            if (filterModal.DisplayAs == "Detail by Revision")
            {
                queryBuilder.Append("GROUP BY selling_style_cd||' '||selling_color_cd||' '||selling_attribute_cd||' '||selling_size_cd ,rule_number,spill_over_ind   ,(((c1.week_end_date - 1) - (c2.week_end_date - 1))/7)+1, nvl(MFG_REVISION_NO,-1), o.production_status, DECODE(make_or_buy_cd,'B','B','M'),o.remote_update_cd  ");
            }
            else
            {
                queryBuilder.Append("GROUP BY selling_style_cd||' '||selling_color_cd||' '||selling_attribute_cd||' '||selling_size_cd ,rule_number,spill_over_ind   ,(((c1.week_end_date - 1) - (c2.week_end_date - 1))/7)+1,0,o.production_status, DECODE(make_or_buy_cd,'B','B','M'),o.remote_update_cd  ");
            }
            if (filterModal.DisplayAs == "Detail by Revision")
            {
                queryBuilder.Append("ORDER BY selling_style_cd||' '||selling_color_cd||' '||selling_attribute_cd||' '||selling_size_cd, nvl(MFG_REVISION_NO,-1) ,rule_number ,(((c1.week_end_date - 1) - (c2.week_end_date - 1))/7)+1,spill_over_ind ");
            }
            else
            {
                queryBuilder.Append("ORDER BY selling_style_cd||' '||selling_color_cd||' '||selling_attribute_cd||' '||selling_size_cd ,rule_number ,(((c1.week_end_date - 1) - (c2.week_end_date - 1))/7)+1,spill_over_ind ");
            }


            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            IDataReader reader = ExecuteReader(queryBuilder.ToString());

            var result = (new DbHelper()).ReadData<SummaryResult>(reader);
            return result;
        }

        /*
        private List<SummaryResult> GetSummarycalculation(SummaryFilterModal filterModal)
        {
            var queryBuilder = new StringBuilder();

            queryBuilder.Append(" select      selling_style_cd||' '||selling_color_cd||' '||selling_attribute_cd||' '||selling_size_cd sku ,rule_number,spill_over_ind     , (((c1.week_end_date - 1) - (c2.week_end_date - 1))/7)+1 weekord, sum(curr_order_qty) curr_order_qty, 0,o.production_status     , decode(make_or_buy_cd,'B','B','M') as make_or_buy_cd,count (o.super_order), Decode(o.remote_update_cd, 'F', Count(o.remote_update_cd),0) as RFailed    "+
            ", trunc( sum(curr_order_qty) / Count(o.Super_order),0) lotSize " +

                " from iss_prod_order_view o ,hbi_fiscal_calendar c1, hbi_fiscal_calendar c2, hbi_fiscal_calendar c3 where   " +
                "   o.order_version = 1 and o.order_status_cd <> '3' and c1.calendar_date = trunc(o.earliest_start) ");

            if (filterModal.WeekEndDate.HasValue)
            {
                queryBuilder.Append("and c2.calendar_date = trunc(to_date('" + filterModal.WeekEndDate.Value.AddDays(1).ToString(LOVConstants.DateFormatOracle) + "','YYYYMMDD'))  ");
            }

            if (filterModal.WeekBeginDate.HasValue)
            {
                queryBuilder.Append("and trunc(c3.calendar_date) = trunc(decode(o.production_status,'L',to_date('" + filterModal.WeekBeginDate.Value.ToString(LOVConstants.DateFormatOracle) + "','YYYYMMDD'),'P',to_date('" + filterModal.WeekBeginDate.Value.ToString(LOVConstants.DateFormatOracle) + "','YYYYMMDD'),o.remote_release_date)) ");
            }



            if (!String.IsNullOrWhiteSpace(filterModal.Style))
            {
                queryBuilder.Append("and selling_style_cd like '" + (filterModal.Style) + "' ");
            }
            if (!String.IsNullOrWhiteSpace(filterModal.Color))
            {
                queryBuilder.Append("and selling_color_cd like '" + (filterModal.Color) + "' ");
            }
            if (!String.IsNullOrWhiteSpace(filterModal.Attribute))
            {
                queryBuilder.Append("and selling_attribute_cd like '" + (filterModal.Attribute) + "' ");
            }
            //if (!String.IsNullOrWhiteSpace(filterModal.Size))
            //{
            //    queryBuilder.Append("and selling_size_cd like '" + (filterModal.Size) + "' ");
            //}
            if (filterModal.WeekBeginDate.HasValue)
            {
                queryBuilder.Append("and c3.week_begin_date <=  to_date('" + filterModal.WeekBeginDate.Value.ToString(LOVConstants.DateFormatOracle) + "','YYYYMMDD') ");
            }
            if (filterModal.WeekBeginDate.HasValue)
            {
                queryBuilder.Append("and c3.week_end_date >=  to_date('" + filterModal.WeekBeginDate.Value.ToString(LOVConstants.DateFormatOracle) + "','YYYYMMDD') ");
            }
            if ((!String.IsNullOrWhiteSpace(filterModal.Planner)) || (!String.IsNullOrWhiteSpace(filterModal.Style)) || (!String.IsNullOrWhiteSpace(filterModal.WorkCenter)))
            {
                queryBuilder.Append(" and (o.order_version,o.super_order) in ( select  o1.order_version,o1.super_order                                             from iss_prod_order_detail o1,iss_prod_order p1   where p1.order_version = o1.order_version and p1.super_order = o1.super_order ");
                if (!String.IsNullOrWhiteSpace(filterModal.Planner))
                {
                    queryBuilder.Append("and p1.planner_cd in (" + FormatInClause(filterModal.Planner) + ") ");
                }
                if (!String.IsNullOrWhiteSpace(filterModal.Style))
                {
                    queryBuilder.Append("and o1.selling_style_cd like '" + filterModal.Style + "' ");
                }
                if (!String.IsNullOrWhiteSpace(filterModal.WorkCenter))
                {
                    queryBuilder.Append(" and nvl(o1.capacity_alloc,'[CATCHALL]')  in (" + FormatInClause(filterModal.WorkCenter) + ") ");
                }
                queryBuilder.Append(" ) ");
            }

            queryBuilder.Append(" group by selling_style_cd||' '||selling_color_cd||' '||selling_attribute_cd||' '||selling_size_cd ,rule_number,spill_over_ind     ,(((c1.week_end_date - 1) - (c2.week_end_date - 1))/7)+1,0,o.production_status, decode(make_or_buy_cd,'B','B','M'),o.remote_update_cd  order by selling_style_cd||' '||selling_color_cd||' '||selling_attribute_cd||' '||selling_size_cd ,rule_number     ,(((c1.week_end_date - 1) - (c2.week_end_date - 1))/7)+1,spill_over_ind  ");


            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            IDataReader reader = ExecuteReader(queryBuilder.ToString());

            var result = (new DbHelper()).ReadData<SummaryResult>(reader);
            return result;
        }
        */
        public IList<NetDemand> GetNetDemandTotal(NetDemand netDemand)
        {
            string SQL = "";
            string strPlants = "";

            string vSQL;

            vSQL = ("select DISTINCT Plant Plant_cd from dpr_fcst_demand_view d " + ("where d.style = \'"
                        + (netDemand.Style + ("\' and d.color = \'"
                        + (netDemand.Color + ("\' and d.attribute_cd = \'"
                        + (netDemand.Attribute + ("\'and d.size_cd = \'"
                        + (netDemand.Size + "\' ")))))))));

            IDataReader reader = ExecuteReader(vSQL);

            var result = (new DbHelper()).ReadData<PlantInfo>(reader);
            PlantInfo[] Plants = result.ToArray();

            for (int i = 0; (i <= (Plants.Length - 1)); i++)
            {
                if ((i > 0))
                {
                    SQL = (SQL + " Union All ");
                }
                //if (((Plants[i].Plant_cd == "**") && ((Plants.Length - 1) > 0)))
                //{
                //    strPlants = "";
                //    for (int j = 1; (j <= (Plants.Length - 1)); j++)
                //    {
                //        if ((Plants[i].Plant_cd != Plants[j].Plant_cd))
                //        {
                //            if ((strPlants != ""))
                //            {
                //                strPlants = (strPlants + ",");
                //            }
                //            strPlants = (strPlants + ("\'"
                //                        + (Plants[j] + "\'")));
                //        }
                //    }
                //}

                //CA# 9592-17 Net demand popup not showing any data for style 2135 color 00U.

                if (((Plants.Length - 1) > 0))
                {
                    strPlants = "";
                    for (int j = 0; (j <= (Plants.Length - 1)); j++)
                    {
                        if ((Plants[i].Plant_cd != Plants[j].Plant_cd))
                        {
                            if ((strPlants != ""))
                            {
                                strPlants = (strPlants + ",");
                            }
                            strPlants = (strPlants + ("\'"
                                        + (Plants[j].Plant_cd + "\'"))); //CA# 44304-17
                        }
                    }
                }
                if (!netDemand.Summaize_NetDmd)
                {

                    SQL = ((SQL + " select \'")
                                + (Plants[i].Plant_cd + ("\' plant,decode(d.DEMAND_DRIVER,\'HSF\',\'FCST\',\'DPR\') cat,to_char(d.rule_number) || \'-\' || d.demand_source rule_number , TO_CHAR ( d.rule_number ) rulenumber, d.demand_source ruleDescription , " +
                                " priority_sequence,CURRENT_QTY qty, 0 NET_Demand, decode(sign(d.priority_sequence-x.priority),-1,\'CONSUMED\',\'NET\') Consumed " +
                                "from dpr_fcst_demand_view d," + (" (select  0 sorted ,\'SUG\',  0 qty , 0 rule_number, nvl(min(o.priority_sequence),999999999) priority f" +
                                "rom avyx_demand_driver o where  " + ("  selling_style_cd = \'"
                                + (netDemand.Style + ("\' and selling_color_cd = \'"
                                + (netDemand.Color + ("\' and selling_attribute_cd = \'"
                                + (netDemand.Attribute + ("\' and selling_size_cd = \'"
                                + (netDemand.Size + ("\'" + (" and Plant = \'"
                                + (Plants[i].Plant_cd + ("\')x " + ("where d.style = \'"
                                + (netDemand.Style + ("\' and d.color = \'"
                                + (netDemand.Color + ("\' and d.attribute_cd = \'"
                                + (netDemand.Attribute + ("\'and d.size_cd = \'"
                                + (netDemand.Size + ("\' and d.Plant = \'"
                                + (Plants[i].Plant_cd + ("\' " + "Union All ")))))))))))))))))))))))))));
                }
                else
                {
                    SQL = ((SQL + " select \'")
                                + (Plants[i].Plant_cd + (@"' plant,decode(d.DEMAND_DRIVER,'HSF','FCST','DPR') cat,to_char(d.rule_number) || '-' || d.demand_source rule_number ,TO_CHAR ( d.rule_number ) rulenumber, d.demand_source ruleDescription,  min(priority_sequence) priority_sequence,sum(CURRENT_QTY) qty, 0 NET_Demand, decode(sign(min(d.priority_sequence-x.priority)),-1,'CONSUMED','NET') Consumed from dpr_fcst_demand_view d," + (" (select  0 sorted ,\'SUG\',  0 qty , 0 rule_number, nvl(min(o.priority_sequence),999999999) priority f" +
                                "rom avyx_demand_driver o where  " + ("  selling_style_cd = \'"
                                + (netDemand.Style + ("\' and selling_color_cd = \'"
                                + (netDemand.Color + ("\' and selling_attribute_cd = \'"
                                + (netDemand.Attribute + ("\' and selling_size_cd = \'"
                                + (netDemand.Size + ("\'" + (" and Plant = \'"
                                + (Plants[i].Plant_cd + ("\')x " + ("where d.style = \'"
                                + (netDemand.Style + ("\' and d.color = \'"
                                + (netDemand.Color + ("\' and d.attribute_cd = \'"
                                + (netDemand.Attribute + ("\'and d.size_cd = \'"
                                + (netDemand.Size + ("\' and d.Plant = \'"
                                + (Plants[i].Plant_cd + ("\' " + ("Group By d.DEMAND_DRIVER, d.rule_number, d.demand_source " + "Union All "))))))))))))))))))))))))))));
                }
                if ((Plants[i].Plant_cd == "**"))
                {
                    // sql = sql & " select '**', 'On Hand' cat, '0' rule_number,0 priority_sequence,round(sum(x.qty)/12) qty,0,'' "
                    //SQL = (SQL + (" select \'**\' plant, \'On Hand\' cat, \'0\' rule_number, '0' rulenumber, '0' ruleDescription, 0 priority_sequence,sum(x.qty) qty,0 NET_Demand,\'\' Consumed " + (" from ( " + (" select r.quantity_start qty  from resource_profile r  where resource_version = 1 " + (" and resource_name like \'"
                    //            + (netDemand.Style + ("~"
                    //            + (netDemand.Color + ("~"
                    //            + (netDemand.Attribute + ("~"
                    //            + (netDemand.Size + ("~"
                    //            + (Plants[i].Plant_cd + "\'  and r.profile_type = \'IN\'"))))))))))))));CA#280719-16 To improve net demand Pop up performance
                    SQL = (SQL + (" select \'**\' plant, \'On Hand\' cat, \'0\' rule_number, '0' rulenumber, '0' ruleDescription, 0 priority_sequence,sum(x.qty) qty,0 NET_Demand,\'\' Consumed " + (" from  ( select r.on_hand_qty qty  from iss_inventory r   where r.style = \'"
                                + (netDemand.Style + ("\' and r.color = \'"
                                + (netDemand.Color + ("\' and r.attribute_cd = \'"
                                + (netDemand.Attribute + ("\'and r.size_cd = \'"
                                + (netDemand.Size + ("\' and r.Plant_no = \'"
                                + (Plants[i].Plant_cd + "'"))))))))))));

                    //if (((Plants.Length - 1) > 0))
                    //{
                    //    SQL = (SQL + (" Union All " + (" select r.on_hand_qty  *-1 qty  from iss_inventory r   where r.style = \'"
                    //            + (netDemand.Style + ("\' and r.color = \'"
                    //            + (netDemand.Color + ("\' and r.attribute_cd = \'"
                    //            + (netDemand.Attribute + ("\'and r.size_cd = \'"
                    //            + (netDemand.Size + ("\' and r.Plant_no in  ("
                    //            + (strPlants + ")"))))))))))));
                    //}

                    if (((Plants.Length - 1) > 0))
                    {
                        SQL = (SQL + (" Union All " + (" select r.on_hand_qty  *-1 qty  from iss_inventory r   where r.style = \'"
                                + (netDemand.Style + ("\' and r.color = \'"
                                + (netDemand.Color + ("\' and r.attribute_cd = \'"
                                + (netDemand.Attribute + ("\'and r.size_cd = \'"
                                + (netDemand.Size + ("\' and r.Plant_no in  ("
                                + (strPlants + ")"))))))))))));
                    }
                    SQL = (SQL + " )x ");

                    //if (((Plants.Length - 1) > 0))
                    //{
                    //    SQL = (SQL + (" Union All " + (" select r.on_hand_qty  *-1 qty  from iss_inventory r   where resource_version = 1 " + (" and resource_name like \'"
                    //                + (netDemand.Style + ("~"
                    //                + (netDemand.Color + ("~"
                    //                + (netDemand.Attribute + ("~"
                    //                + (netDemand.Size + ("%\'  and r.profile_type = \'IN\'" + (" and oprsql.db_widgit.parse(resource_name,5,\'~\',1) in ("
                    //                + (strPlants + ") "))))))))))))));
                    //}
                    //SQL = (SQL + " )x ");
                }
                else
                {
                    // sql = sql & "select '" & Plants(i) & "' plant,'On Hand' cat, '0' rule_number,0 priority_sequence,round(r.quantity_start/12) qty,0,''  from resource_profile r  where resource_version = 1 "
                    //SQL = (SQL + ("select \'"
                    //            + (Plants[i].Plant_cd + ("\' plant,\'On Hand\' cat, \'0\' rule_number, '0' rulenumber, '0' ruleDescription, 0 priority_sequence,r.quantity_start qty,0 NET_Demand,\'\' Consumed  from resource_p" +
                    //            "rofile r  where resource_version = 1 " + ("and resource_name like \'"
                    //            + (netDemand.Style + ("~"
                    //            + (netDemand.Color + ("~"
                    //            + (netDemand.Attribute + ("~"
                    //            + (netDemand.Size + ("~"
                    //            + (Plants[i].Plant_cd + "\'  and r.profile_type = \'IN\'"))))))))))))));CA#280719-16 To improve net demand Pop up performance
                    SQL = (SQL + ("select \'"
                               + (Plants[i].Plant_cd + ("\' plant,\'On Hand\' cat, \'0\' rule_number, '0' rulenumber, '0' ruleDescription, 0 priority_sequence,r.on_hand_qty qty,0 NET_Demand,\'\' Consumed  from iss_inventory r" +
                               " where r.style = \'"
                               + (netDemand.Style + ("\' and r.color = \'"
                               + (netDemand.Color + ("\' and r.attribute_cd = \'"
                               + (netDemand.Attribute + ("\'and r.size_cd = \'"
                               + (netDemand.Size + ("\' and r.Plant_no = \'"
                               + (Plants[i].Plant_cd + "' ")))))))))))));
                }
                // & " union all select '" & Plants(i) & "' plant,'DC Conversion' cat, '0' rule_number,0 priority_sequence ,round(sum(p.curr_order_qty)/12) qty,0,'' from iss_prod_order_detail p, iss_prod_order p1 "
                SQL = (SQL + (" union all select \'"
                            + (Plants[i].Plant_cd + ("\' plant,\'DC Conversion\' cat, \'0\' rule_number, '0' rulenumber, '0' ruleDescription, 0 priority_sequence ,sum(p.curr_order_qty) qty,0 NET_Demand,\'\' Consumed from" +
                            " iss_prod_order_detail p, iss_prod_order p1 " + (" where p.order_version = 1 and selling_style_cd = \'"
                            + (netDemand.Style + ("\' and selling_color_cd = \'"
                            + (netDemand.Color + ("\' and selling_attribute_cd = \'"
                            + (netDemand.Attribute + ("\' and selling_size_cd = \'"
                            + (netDemand.Size + ("\'" + ("  and p.order_version = p1.order_version and p.super_order = p1.super_order and p1.production_status <> \'P\' and p1.order_source_cd = \'RWRK\'" + " and decode(p1.order_source_cd ,\'MITS\',\'Y\',decode(substr(p1.super_Order,1,2),\'IN\',\'Y\',\'N\')) = \'N\' "))))))))))))));
                if (((Plants[i].Plant_cd == "**") && ((Plants.Length - 1) > 0)))
                {
                    SQL = (SQL + (" and p.demand_loc not in ("
                                + (strPlants + ") group by p1.order_source_cd ")));
                }
                else if ((Plants[i].Plant_cd != "**"))
                {
                    SQL = (SQL + (" and p.demand_loc in (\'"
                                + (Plants[i].Plant_cd + "\') group by p1.order_source_cd ")));  //CA# 44304-17
                }
                else
                {
                    SQL = (SQL + " group by p1.order_source_cd ");
                }
                // & " union all select '" & Plants(i) & "' plant,'INTRANSIT' cat, '0' rule_number,0 priority_sequence,round(sum(p.curr_order_qty)/12) qty,0,'' from iss_prod_order_detail p, iss_prod_order p1  where "
                //SQL = (SQL + (" union all select \'"
                //            + (Plants[i].Plant_cd + ("\' plant,\'INTRANSIT\' cat, \'0\' rule_number, '0' rulenumber, '0' ruleDescription, 0 priority_sequence,sum(p.curr_order_qty) qty, 0 NET_Demand,\'\' Consumed from iss_" +
                //            "prod_order_detail p, iss_prod_order p1  where p.order_version = 1 and " + (" selling_style_cd = \'"
                //            + (netDemand.Style + ("\' and selling_color_cd = \'"
                //            + (netDemand.Color + ("\' and selling_attribute_cd = \'"
                //            + (netDemand.Attribute + ("\' and selling_size_cd = \'"
                //            + (netDemand.Size + ("\'" + ("  and p.order_version = p1.order_version and p.super_order = p1.super_order and p1.production_status <> \'P\' " + " and decode(p1.order_source_cd ,\'MITS\',\'Y\',decode(substr(p1.super_Order,1,2),\'IN\',\'Y\',\'N\')) = \'Y\' "))))))))))))));

                SQL = (SQL + (" union all select \'"
                           + (Plants[i].Plant_cd + ("\' plant,\'INTRANSIT\' || \'-\' ||p1.order_source_cd cat, \'0\' rule_number, '0' rulenumber, '0' ruleDescription, 0 priority_sequence,sum(p.curr_order_qty) qty, 0 NET_Demand,\'\' Consumed from iss_" +
                           "prod_order_detail p, iss_prod_order p1  where p.order_version = 1 and " + (" selling_style_cd = \'"
                           + (netDemand.Style + ("\' and selling_color_cd = \'"
                           + (netDemand.Color + ("\' and selling_attribute_cd = \'"
                           + (netDemand.Attribute + ("\' and selling_size_cd = \'"
                           + (netDemand.Size + ("\'" + ("  and p.order_version = p1.order_version and p.super_order = p1.super_order and p1.production_status <> \'P\' " + " and decode(p1.order_source_cd ,\'MITS\',\'Y\',decode(substr(p1.super_Order,1,2),\'IN\',\'Y\',\'N\')) = \'Y\' "))))))))))))));

                if (((Plants[i].Plant_cd == "**") && ((Plants.Length - 1) > 0)))
                {
                    SQL = (SQL + (" and p.demand_loc not in ("
                                + (strPlants + ") group by p1.order_source_cd ")));
                }
                else if ((Plants[i].Plant_cd != "**"))
                {
                    SQL = (SQL + (" and p.demand_loc in (\'"
                                + (Plants[i].Plant_cd + "\') group by p1.order_source_cd  ")));
                }
                //CA#44304-17
                else
                {
                    SQL = (SQL + ("  group by p1.order_source_cd  "));
                }
                //SQL = (SQL + (" union all select \'"
                //     + (Plants[i].Plant_cd + ("\' plant,\'INTRANSIT\' cat, \'0\' rule_number, '0' rulenumber, '0' ruleDescription, 0 priority_sequence,sum(p.curr_order_qty) qty, 0 NET_Demand,\'\' Consumed from iss_" +
                //     "prod_order_detail p, iss_prod_order p1  where p.order_version = 1 and " + (" selling_style_cd = \'"
                //     + (netDemand.Style + ("\' and selling_color_cd = \'"
                //     + (netDemand.Color + ("\' and selling_attribute_cd = \'"
                //     + (netDemand.Attribute + ("\' and selling_size_cd = \'"
                //     + (netDemand.Size + ("\'" + ("  and p.order_version = p1.order_version and p.super_order = p1.super_order and p1.production_status <> \'P\' and p1.order_source_cd in (\'SAP\') " + " and decode(p1.order_source_cd ,\'MITS\',\'Y\',decode(substr(p1.super_Order,1,2),\'IN\',\'Y\',\'N\')) = \'Y\' "))))))))))))));

                SQL = (SQL + (" union all select \'"
                        + (Plants[i].Plant_cd + ("\' plant,\'INTRANSIT\' || \'-\' ||p1.order_source_cd cat, \'0\' rule_number, '0' rulenumber, '0' ruleDescription, 0 priority_sequence,sum(p.curr_order_qty) qty, 0 NET_Demand,\'\' Consumed from iss_" +
                        "prod_order_detail p, iss_prod_order p1  where p.order_version = 1 and " + (" selling_style_cd = \'"
                        + (netDemand.Style + ("\' and selling_color_cd = \'"
                        + (netDemand.Color + ("\' and selling_attribute_cd = \'"
                        + (netDemand.Attribute + ("\' and selling_size_cd = \'"
                        + (netDemand.Size + ("\'" + ("  and p.order_version = p1.order_version and p.super_order = p1.super_order and p1.production_status <> \'P\' and p1.order_source_cd in (\'SAP\') " + " and decode(p1.order_source_cd ,\'MITS\',\'Y\',decode(substr(p1.super_Order,1,2),\'IN\',\'Y\',\'N\')) = \'N\' "))))))))))))));
                if (((Plants[i].Plant_cd == "**") && ((Plants.Length - 1) > 0)))
                {
                    SQL = (SQL + (" and p.demand_loc not in ("
                                + (strPlants + ") group by p1.order_source_cd  ")));
                }
                else if ((Plants[i].Plant_cd != "**"))
                {
                    SQL = (SQL + (" and p.demand_loc in (\'"
                                + (Plants[i].Plant_cd + "\') group by p1.order_source_cd  ")));
                }

                //CA#44304-17
                else
                {
                    SQL = (SQL + ("  group by p1.order_source_cd  "));
                }

                // & " union all select '" & Plants(i) & "' plant,'WIP' cat, '0' rule_number,0 priority_sequence ,round(sum(p.curr_order_qty)/12) qty,0,'' from iss_prod_order_detail p, iss_prod_order p1 "
                //SQL = (SQL + (" union all select \'"
                //            + (Plants[i].Plant_cd + ("\' plant,\'WIP\' cat, \'0\' rule_number, '0' rulenumber, '0' ruleDescription, 0 priority_sequence ,sum(p.curr_order_qty) qty,0 NET_Demand,\'\' Consumed from iss_prod_" +
                //            "order_detail p, iss_prod_order p1 " + (" where p.order_version = 1 and  selling_style_cd = \'"
                //            + (netDemand.Style + ("\' and selling_color_cd = \'"
                //            + (netDemand.Color + ("\' and selling_attribute_cd = \'"
                //            + (netDemand.Attribute + ("\' and selling_size_cd = \'"
                //            + (netDemand.Size + ("\'" + (" and p.order_version = p1.order_version and p.super_order = p1.super_order and p1.production_status <> \'P\' and p1.order_source_cd <> \'RWRK\' " +
                //            "" + " and decode(p1.order_source_cd ,\'MITS\',\'Y\',decode(substr(p1.super_Order,1,2),\'IN\',\'Y\',\'N\')) = \'N\'"))))))))))))));

                SQL = (SQL + (" union all select \'"
                            + (Plants[i].Plant_cd + ("\' plant,\'WIP\' || \'-\' ||p1.order_source_cd cat, \'0\' rule_number, '0' rulenumber, '0' ruleDescription, 0 priority_sequence ,sum(p.curr_order_qty) qty,0 NET_Demand,\'\' Consumed from iss_prod_" +
                            "order_detail p, iss_prod_order p1 " + (" where p.order_version = 1 and  selling_style_cd = \'"
                            + (netDemand.Style + ("\' and selling_color_cd = \'"
                            + (netDemand.Color + ("\' and selling_attribute_cd = \'"
                            + (netDemand.Attribute + ("\' and selling_size_cd = \'"
                            + (netDemand.Size + ("\'" + (" and p.order_version = p1.order_version and p.super_order = p1.super_order and p1.production_status <> \'P\' and p1.order_source_cd not in (\'RWRK\',\'SAP\')" +
                            "" + " and decode(p1.order_source_cd ,\'MITS\',\'Y\',decode(substr(p1.super_Order,1,2),\'IN\',\'Y\',\'N\')) = \'N\'"))))))))))))));


                if (((Plants[i].Plant_cd == "**")
                            && ((Plants.Length - 1)
                            > 0)))
                {
                    SQL = (SQL + (" and p.demand_loc not in ("
                                + (strPlants + ")  ")));
                }
                else if ((Plants[i].Plant_cd != "**"))
                {
                    SQL = (SQL + (" and p.demand_loc in (\'"
                                + (Plants[i].Plant_cd + "\') group by p1.order_source_cd ")));
                }
                //44304-17
                //if ((i == (Plants.Length - 1)))
                //{
                  
                //    SQL = (SQL + (" group by demand_loc, p1.order_source_cd " + " order by plant desc, priority_sequence"));
                //}
                if ((i == (Plants.Length - 1)) && (Plants[i].Plant_cd != "**"))
                {

                    SQL = (SQL + ("  order by plant desc, priority_sequence, cat"));
                }
                else if ((i == (Plants.Length - 1)))
                {

                    SQL = (SQL + (" group by demand_loc, p1.order_source_cd " + " order by plant desc, priority_sequence, cat"));
                }
            }

            IDataReader NDreader = ExecuteReader(SQL);

            var netresult = (new DbHelper()).ReadData<NetDemand>(NDreader);
            return netresult;


        }


        /*
        public IList<NetDemand> GetNetDemandTotal(string style, string color, string attribute, string size)
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append("select '**' plant,decode(d.DEMAND_DRIVER,'HSF','FCST','DPR') cat,to_char(d.rule_number) || '-' || d.demand_source rule_number " +
                ", min(priority_sequence) priority_sequence,sum(CURRENT_QTY) qty, 0 NET_Demand, decode(sign(min(d.priority_sequence-x.priority)),-1,'CONSUMED','NET') Consumed " +
                "from dpr_fcst_demand_view d, (select  0 sorted ,'SUG',  0 qty , 0 rule_number, nvl(min(o.priority_sequence),999999999) priority " +
                " from avyx_demand_driver o " +
                " where    selling_style_cd = '" + Val(style)+ "' and selling_color_cd = '" + Val(color) + "' and selling_attribute_cd = '" + Val(attribute) + "' " +
                " and selling_size_cd = '" + Val(size) + "' and Plant = '**' " +
                            ")x " +
                "where d.style = '" + Val(style) + "' and d.color = '" + Val(color) + "' and d.attribute_cd = '" + Val(attribute) + "' and d.size_cd = '" + Val(size) + "' and d.Plant = '**' " +
                "Group By d.DEMAND_DRIVER, d.rule_number, d.demand_source ");
            
            queryBuilder.Append("union all select '**', 'On Hand' cat, '0' rule_number,0 priority_sequence,sum(x.qty) qty,0 NET_Demand,'' Consumed " + 
                " from (  select r.quantity_start qty " +
                "from resource_profile r  " +
               " where resource_version = 1  and resource_name like '" + Val(style) + "~" + Val(color) + "~" + Val(attribute) + "~" + Val(size) + "**'  and r.profile_type = 'IN' " +
            
                "Union All  " +
            
                "select r.quantity_start  *-1 qty  " +
               " from resource_profile r  " +
               " where resource_version = 1  and resource_name like '" + Val(style) + "~" + Val(color) + "~" + Val(attribute) + "~" + Val(size) + "%'  and r.profile_type = 'IN' " +
                    "and oprsql.db_widgit.parse(resource_name,5,'~',1) in ('5V')  )x " );

            queryBuilder.Append("union all select '**' plant,'DC Conversion' cat, '0' rule_number,0 priority_sequence ,sum(p.curr_order_qty) qty,0 NET_Demand,'' Consumed " +
                "from iss_prod_order_detail p, iss_prod_order p1  " +
                "where selling_style_cd = '" + Val(style) + "' and selling_color_cd = '" + Val(color) + "' and selling_attribute_cd = '" + Val(attribute) + "' and selling_size_cd = '" + Val(size) + "' " +
                "and p.super_order = p1.super_order and p1.production_status <> 'P' and p1.order_source_cd = 'RWRK' " +
                "and decode(p1.order_source_cd ,'MITS','Y',decode(substr(p1.super_Order,1,2),'IN','Y','N')) = 'N'  and p.demand_loc not in ('5V') " +
                "group by p1.order_source_cd  " );

            queryBuilder.Append("union all select '**' plant,'INTRANSIT' cat, '0' rule_number,0 priority_sequence,sum(p.curr_order_qty) qty,0 NET_Demand,'' Consumed " +
                "from iss_prod_order_detail p, iss_prod_order p1 " +
                "where  selling_style_cd = '" + Val(style) + "' and selling_color_cd = '" + Val(color) + "' and selling_attribute_cd = '" + Val(attribute) + "' and selling_size_cd = '" + Val(size) + "'" +
                "and p.super_order = p1.super_order and p1.production_status <> 'P'  " +
                "and decode(p1.order_source_cd ,'MITS','Y',decode(substr(p1.super_Order,1,2),'IN','Y','N')) = 'Y'  and p.demand_loc not in ('5V') ");


            queryBuilder.Append("union all select '**' plant,'WIP' cat, '0' rule_number,0 priority_sequence ,sum(p.curr_order_qty) qty,0 NET_Demand,'' Consumed " +
                "from iss_prod_order_detail p, iss_prod_order p1  " +
                "where selling_style_cd = '" + Val(style) + "' and selling_color_cd = '" + Val(color) + "' and selling_attribute_cd = '" + Val(attribute) + "' and selling_size_cd = '" + Val(size) + "' and p.super_order = p1.super_order " +
                "and p1.production_status <> 'P' and p1.order_source_cd <> 'RWRK'  " +
                "and decode(p1.order_source_cd ,'MITS','Y',decode(substr(p1.super_Order,1,2),'IN','Y','N')) = 'N' and p.demand_loc not in ('5V') ");
           

            IDataReader reader = ExecuteReader(queryBuilder.ToString());

            var result = (new DbHelper()).ReadData<NetDemand>(reader);
            return result;
        }
        */
        public IList<NetDemand> GetNetDemand(NetDemand netDemand)
        {


            var data = GetNetDemandTotal(netDemand);

            IList<NetDemand> netDmnds = new List<NetDemand>();


            (from f in data
             select f).ToList().ForEach(f =>
             {
                 f.qty = Math.Round(f.qty / 12, 1);

             });

            var uniquePlants = data
                                  .Select(p => p.plant)
                                  .Distinct();

            foreach (var plant in uniquePlants)
            {
                //var result = data.Where(s => !s.cat.Contains("DPR") && s.plant == plant).ToList();

                var result = data.Where(s => (s.cat.Contains("On Hand") || s.cat.Contains("INTRANSIT") || s.cat.Contains("DC Conversion") || s.cat.Contains("WIP")) && s.plant == plant).ToList();

                decimal sum = result.Select(t => t.qty).Sum().RoundCustom(1);


                var val = (from f in result
                           select f)
                            .LastOrDefault();

                (from f in result
                 where (f.cat == val.cat && f.plant == val.plant)
                 select f).ToList().ForEach(f =>
                 {
                     f.NET_Demand = sum;
                     f.Consumed = "TOTAL INV";

                 });

                //var drpResult = data.Where(s => s.cat.Contains("DPR") && s.plant == plant).ToList();
                var drpResult = data.Where(s => !(s.cat.Contains("On Hand") || s.cat.Contains("INTRANSIT") || s.cat.Contains("DC Conversion") || s.cat.Contains("WIP")) && s.plant == plant).ToList();
                //quantity divide by 12
                decimal currentTotal = sum;

                /*
                var queryq = drpResult
                               .OrderBy(i => i.plant)
                               .Select(i =>
                                   {
                                       currentTotal -= i.qty.RoundCustom(1);
                                       return new NetDemand()
                                       {
                                           plant = i.plant,
                                           qty = i.qty,
                                           NET_Demand = currentTotal,
                                           cat = i.cat,
                                           Consumed = "CONSUMED",
                                           priority_sequence = i.priority_sequence,
                                           rule_number = i.rule_number
                                       };
                                   }
                               ).ToList();


                bool isFirst = false;
               (from c in queryq 
                    where (c.NET_Demand<0)
                    select c).ToList().ForEach(f =>
                     {
                         f.Consumed = (isFirst) ? "NET" : "PARTIALLY CONSUMED";
                         isFirst=true;
                     });

              

               ((List<NetDemand>)netDmnds).AddRange(result);
                ((List<NetDemand>)netDmnds).AddRange(queryq);
                */





                //var query = "";

                var query = drpResult
                               .OrderBy(i => i.plant)
                               .Select(i =>
                               {
                                   currentTotal -= i.qty.RoundCustom(1);
                                   return new
                                   {
                                       plant = i.plant,
                                       qty = i.qty,
                                       NET_Demand = currentTotal,
                                       cat = i.cat,
                                       Consumed = "CONSUMED",
                                       priority_sequence = i.priority_sequence,
                                       rule_number = i.rule_number,
                                       rulenumber = i.rulenumber,
                                       ruleDescription = i.ruleDescription
                                   };
                               }
                               ).ToList();




                var dprNewquery = query
                    .Where(c => c.NET_Demand < 0)
                    .Select(i =>
                    {
                        return new
                        {
                            plant = i.plant,
                            qty = i.qty,
                            NET_Demand = i.NET_Demand,
                            cat = i.cat,
                            Consumed = "PARTIALLY CONSUMED",
                            priority_sequence = i.priority_sequence,
                            rule_number = i.rule_number,
                            rulenumber = i.rulenumber,
                            ruleDescription = i.ruleDescription
                        };
                    }
                    ).FirstOrDefault();





                var dprquery = query
                   .Where(c => c.NET_Demand < 0)
                   .Select(i =>
                   {
                       return new
                       {
                           plant = i.plant,
                           qty = i.qty,
                           NET_Demand = i.NET_Demand,
                           cat = i.cat,
                           Consumed = "NET",
                           priority_sequence = i.priority_sequence,
                           rule_number = i.rule_number,
                           rulenumber = i.rulenumber,
                           ruleDescription = i.ruleDescription
                       };
                   }
                   ).Skip(1).ToList();

                dprquery.Insert(0, dprNewquery);

                //netDmnds.Concat(result);

                query.RemoveAll(i => i.NET_Demand < 0);

                query.AddRange(dprquery);

                foreach (var c in result)
                {
                    netDmnds.Add(new NetDemand
                    {
                        plant = c.plant,
                        cat = c.cat,
                        rule_number = c.rule_number,
                        rulenumber = c.rulenumber,
                        ruleDescription = c.ruleDescription,
                        priority_sequence = c.priority_sequence,
                        qty = c.qty,
                        NET_Demand = c.NET_Demand,
                        Consumed = c.Consumed
                    });
                }


                foreach (var c in query)
                {
                    if (c != null)
                    {
                        netDmnds.Add(new NetDemand
                        {
                            plant = c.plant,
                            cat = c.cat,
                            rule_number = c.rule_number,
                            rulenumber = c.rulenumber,
                            ruleDescription = c.ruleDescription,
                            priority_sequence = c.priority_sequence,
                            qty = c.qty,
                            NET_Demand = c.NET_Demand,
                            Consumed = c.Consumed
                        });
                    }
                }

            }

            return netDmnds;


        }



        #endregion
    }
}
