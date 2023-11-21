using System;
using ISS.Common;
using ISS.Core.Model.Common;
using ISS.Core.Model.Order;
using ISS.Repository.Common;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISS.Repository.Order
{
    public partial class WOManagement  
    {
        public List<WOMDetail> MeetsFilterGrid(IEnumerable<WOMDetail> womDetails, WOManagementSearch search)
        {
            //bool filterMet = true;
            string criteriaFormat = "{0},";
            ISS.Repository.Common.ApplicationRepository repositoryPlanner = new ISS.Repository.Common.ApplicationRepository();
            var resultPlanDate = repositoryPlanner.GetPlanBeginEndDates();
            foreach (var wom in womDetails)
            {
                
                StringBuilder criteria = new StringBuilder(string.Format(criteriaFormat, LOVConstants.WOFilterGrid.DEPENDED_DEMAND));

                // Suggested  & other lots section
                switch (wom.OrderStatus.ToLower())
                {
                    case "s":
                        criteria.AppendFormat(criteriaFormat, OrderStatus.Suggested.GetDescription());
                        if (!search.SuggestedLots)
                            wom.IsHide = true;
                        break;
                    case "l":
                        criteria.AppendFormat(criteriaFormat, OrderStatus.Locked.GetDescription());
                        if (!search.LockedLots)
                            wom.IsHide = true;
                        break;
                    case "r":
                        criteria.AppendFormat(criteriaFormat, OrderStatus.Released.GetDescription());
                        // If released lots then check for released date
                        var CurWeek = resultPlanDate.FirstOrDefault();
                        if (wom.RemoteUpdateDate >= CurWeek.Week_Begin_Date && wom.RemoteUpdateDate <= CurWeek.Week_End_Date)
                        {
                            criteria.AppendFormat(criteriaFormat, LOVConstants.WOFilterGrid.RELEASED_THIS_WEEK);
                            if (!search.ReleasedLotsThisWeek)
                                wom.IsHide = true;
                        }
                        else
                        {
                            if (!search.ReleasedLots)
                                wom.IsHide = true;
                        }
                        break;
                }

                if (wom.OrderStatus.Equals(OrderStatus.Suggested.GetDescription()))
                {
                    if(!string.IsNullOrWhiteSpace(wom.Enforcement))
                    {
                        criteria.AppendFormat(criteriaFormat, wom.OrderStatus + wom.Enforcement);
                    }

                    if(! String.IsNullOrEmpty( wom.Enforcement))
                    {
                        criteria.AppendFormat(criteriaFormat, LOVConstants.WOFilterGrid.SPILL_OVER);
                        if (!search.SpillOver)
                            wom.IsHide = true;
                    }
                }

                // Trim & body only section
                if(string.IsNullOrWhiteSpace(wom.SpreadTypeCode))
                {
                    criteria.AppendFormat(criteriaFormat, SpreadTypes.BothSpread.GetDescription());
                }
                else 
                {
                    criteria.AppendFormat(criteriaFormat, wom.SpreadTypeCode.Equals(SpreadTypes.Trim.GetDescription()) ? SpreadTypes.Trim.GetDescription() : SpreadTypes.Body.GetDescription());
                    if (!search.BodyOnly || !search.TrimOnly)
                        wom.IsHide = true;
                }

             

                // Customer order, Events, Max build etc. section
                if (!string.IsNullOrWhiteSpace(wom.DemandSource))
                {
                    if (wom.DemandSource.Contains(DemandSourceTypes.Event.ToString()))
                    {
                        criteria.AppendFormat(criteriaFormat, DemandSourceTypes.Event.GetDescription());
                        if (!search.Events)
                            wom.IsHide = true;
                    }
                    else if (wom.DemandSource.Contains(DemandSourceTypes.CustomerOrder.ToString()))
                    {
                        criteria.AppendFormat(criteriaFormat, DemandSourceTypes.CustomerOrder.GetDescription());
                        if (!search.CustomerOrders)
                            wom.IsHide = true;
                    }
                    else if (wom.DemandSource.Contains(DemandSourceTypes.FCST.ToString()))
                    {
                        criteria.AppendFormat(criteriaFormat, DemandSourceTypes.FCST.GetDescription());
                        if(!search.Forecast)
                            wom.IsHide = true;
                    }
                    else if (wom.DemandSource.Contains(DemandSourceTypes.Maxbuild.ToString()))
                    {
                        criteria.AppendFormat(criteriaFormat, DemandSourceTypes.Maxbuild.GetDescription());
                        if (!search.MaxBuild)
                            wom.IsHide = true;
                    }
                    else if (wom.DemandSource.Contains(DemandSourceTypes.TIL.ToString()))
                    {
                        criteria.AppendFormat(criteriaFormat, DemandSourceTypes.TIL.GetDescription());
                        if (!search.TILs)
                            wom.IsHide = true;
                    }

                    //if (!search.Events || !search.CustomerOrders || !search.MaxBuild || !search.TILs || !search.StockTarget)
                    //    wom.IsHide = true;
                }

                //Exclude buy order
                if ( (wom.MakeOrBuy+String.Empty).Equals(LOVConstants.MakeOrBuy.Buy,StringComparison.OrdinalIgnoreCase))
                {
                    criteria.AppendFormat(criteriaFormat, LOVConstants.WOFilterGrid.EXCLUDE_BUY_ORDER);
                    if (search.ExcludeBuyOrders)
                        wom.IsHide = true;
                }

                // Group only
                if (!string.IsNullOrEmpty(wom.GroupId))
                {
                    criteria.AppendFormat(criteriaFormat, LOVConstants.WOFilterGrid.GROUP_ONLY);
                   
                }
                else
                {
                    if (search.GroupOnly)
                        wom.IsHide = true;
                }
               
               
                wom.FilterGridCriteria = criteria.ToString(); 
				wom.NoteInd = LOVConstants.No;
            }                
            return womDetails.ToList();
        }
    }

}
