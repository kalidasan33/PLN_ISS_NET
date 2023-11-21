using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KA.BusinessRules.Contract.MaterialSupply;
using ISS.Core.Model.Common;
using KA.Repository.MaterialSupply;
using KA.Core.Model.MaterialSupply;
using ISS.Common;
using ISS.Repository.Common;


namespace KA.BusinessService.Implementation.MaterialSupply
{
    public partial class MaterialSupplyService : IMaterialSupplyService
    {
        const String WeekName = "Wk";

        public List<MaterialPAB> MaterialSupplySearchDetails(MaterialBlankSupplySearch mbSearch)
        {
            var searchRepository = new MaterialSupplyRepositoy();
            var FiscWeekList = MaterialSupplyGetWeeks();
            mbSearch.BeginWeek = FiscWeekList[0].FiscalWeek;
            mbSearch.BeginYear = FiscWeekList[0].FiscalYear;
            mbSearch.EndWeek = FiscWeekList[51].FiscalWeek;
            mbSearch.EndYear = FiscWeekList[51].FiscalYear;
            var result = searchRepository.MaterialSupplySearchDetails(mbSearch);
            List<MaterialPAB> SupplyByDC = new List<MaterialPAB>(); 
            var startIdx = 0;
            MaterialPAB currDC = null;
            Dictionary<String, dynamic> currSize = null;
            int counter = 0;
            while (startIdx < result.Count && counter<result.Count)
            {
                counter++;
                currSize = CalculatePABData(result, FiscWeekList, mbSearch.IncludeSuggLots, ref startIdx, mbSearch.ShowDz);
                currDC = SupplyByDC.FirstOrDefault(e => e.DC == currSize["DC"]);
                if (currDC == null)
                {
                    currDC = new MaterialPAB() { DC = currSize["DC"], SKU = currSize["SKU"] };
                    currDC.SizeList = new List<Dictionary<string, dynamic>>();
                    SupplyByDC.Add(currDC);
                }
                currDC.SizeList.Add(currSize);   
                
            }

           

            return SupplyByDC;
        }

        private Dictionary<String, dynamic> CalculatePABData(List<MaterialPAB> result, List<MaterialPAB> FiscWeekList, bool IncludeSuggLots, ref int startIdx, bool isDozen)
        {

            #region init
            Dictionary<String, dynamic> SupplyBySize = new Dictionary<String, dynamic>(); 
            var OnHand = new Dictionary<String, dynamic>(); OnHand.Add("PABType", LOVConstants.PABTypes.OnHand);
            var InTransit = new Dictionary<String, dynamic>(); InTransit.Add("PABType", LOVConstants.PABTypes.InTransit);
            var WIP = new Dictionary<String, dynamic>(); WIP.Add("PABType", LOVConstants.PABTypes.WIP);
            var Released = new Dictionary<String, dynamic>(); Released.Add("PABType", LOVConstants.PABTypes.Released);
            var Locked = new Dictionary<String, dynamic>(); Locked.Add("PABType", LOVConstants.PABTypes.Locked);
            var Suggested = new Dictionary<String, dynamic>(); Suggested.Add("PABType", LOVConstants.PABTypes.Suggested);
            var PAB = new Dictionary<String, dynamic>(); PAB.Add("PABType", LOVConstants.PABTypes.PAB);

            var PABList = new List<Dictionary<String, dynamic>>(){ OnHand, InTransit, WIP, Released, Locked, Suggested, PAB };
            SupplyBySize.Add("PABList",PABList);

           
            decimal TotInTransit = 0;
            decimal TotWIP = 0;
            decimal TotReleased = 0;
            decimal TotLocked = 0;
            decimal TotSuggested = 0;


            AddFullWeeks(OnHand);
            AddFullWeeks(InTransit);
            AddFullWeeks(WIP);
            AddFullWeeks(Released);
            AddFullWeeks(Locked);
            AddFullWeeks(Suggested);
            AddFullWeeks(PAB);
    
            #endregion 

            if (result.Count > 0)
            {
                DateTime FiscWeek = new DateTime().Date;
                int Week = 1;
                int weekIdx = 0;
              
                if (FiscWeekList.Count > 0)
                {
                    FiscWeek = FiscWeekList[weekIdx].WeekBegDate;
                    Week = (int)FiscWeekList[weekIdx].FiscalWeek;
                }
                String currWeek = WeekName + Week;
                String PrevWeek = currWeek;
                bool week1Flag = true;
                bool first = true;
                String Group = String.Empty;
                if (result.Count > startIdx) Group = result[startIdx].getGroup();
                for (int idx = startIdx; idx < result.Count; idx++)
                {
                    var item = result[idx];
                    if (Group != item.getGroup())
                    {
                        startIdx = idx ; //setting next group start
                        break;
                    }
                    else if (idx == result.Count-1)
                    {
                        startIdx = result.Count; //setting group end and result end
                    }
                    else
                    {
                        startIdx = idx;
                    }
                    if (first)
                    {
                        first = false;
                        SupplyBySize.Add("SizeCD", item.SizeCD);
                        SupplyBySize.Add("Size", item.Size);
                        SupplyBySize.Add("DC", item.DC);
                        SupplyBySize.Add("SKU", item.Style +"~"+item.Color +"~"+item.Attribute );
                    }

                    if (item.WeekBegDate <= FiscWeek)
                    {
                        if (item.TranType == LOVConstants.PABTypes.InTransit)
                        {
                            if (!isDozen)
                            {
                                InTransit[currWeek] += (item.SumQty);
                                PAB[currWeek] += (item.SumQty);
                            }
                            else
                            {
                                if (item.SumQty != 0)
                                {
                                    InTransit[currWeek] += ((item.SumQty) / 12).RoundCustom(2);
                                    PAB[currWeek] += ((item.SumQty) / 12).RoundCustom(2);
                                }
                            }
                        }
                        else if (item.TranType == LOVConstants.PABTypes.WIP)
                        {
                            if (!isDozen)
                            {
                                WIP[currWeek] += (item.SumQty);
                                PAB[currWeek] += (item.SumQty);
                            }
                            else
                            {
                                if (item.SumQty != 0)
                                {
                                    WIP[currWeek] += ((item.SumQty) / 12).RoundCustom(2);
                                    PAB[currWeek] += ((item.SumQty) / 12).RoundCustom(2);
                                }
                            }
                        }
                        else if (item.TranType == LOVConstants.PABTypes.Released)
                        {
                            if (!isDozen)
                            {
                                Released[currWeek] += (item.SumQty);
                                PAB[currWeek] -= (item.SumQty);
                            }
                            else
                            {
                                if (item.SumQty != 0)
                                {
                                    Released[currWeek] += ((item.SumQty) / 12).RoundCustom(2);
                                    PAB[currWeek] -= ((item.SumQty) / 12).RoundCustom(2);
                                }
                            }
                        }
                        else if (item.TranType == LOVConstants.PABTypes.Locked)
                        {
                            if (!isDozen)
                            {
                                Locked[currWeek] += (item.SumQty);
                                PAB[currWeek] -= (item.SumQty);
                            }
                            else
                            {
                                if (item.SumQty != 0)
                                {
                                    Locked[currWeek] += ((item.SumQty) / 12).RoundCustom(2);
                                    PAB[currWeek] -= ((item.SumQty) / 12).RoundCustom(2);
                                }
                            }
                        }
                        else if (item.TranType == LOVConstants.PABTypes.Suggested)
                        {
                            if (!isDozen)
                            {
                                Suggested[currWeek] += (item.SumQty);
                                if (IncludeSuggLots)
                                {
                                    PAB[currWeek] -= (item.SumQty);
                                }
                            }
                            else
                            {
                                if (item.SumQty != 0)
                                {
                                    Suggested[currWeek] += ((item.SumQty) / 12).RoundCustom(2);
                                    if (IncludeSuggLots)
                                    {
                                        PAB[currWeek] -= ((item.SumQty) / 12).RoundCustom(2);
                                    }
                                }
                            }
                        }
                        else  //On Hand
                        {
                            //if (week1Flag) // on hand calc required only for Week 1
                            //{
                            if (!isDozen)
                            {
                                OnHand[currWeek] += (item.SumQty);
                                PAB[currWeek] += (item.SumQty);
                            }
                            else
                            {
                                if (item.SumQty != 0)
                                {
                                    OnHand[currWeek] += ((item.SumQty) / 12).RoundCustom(2);
                                    PAB[currWeek] += ((item.SumQty) / 12).RoundCustom(2);
                                }
                            }
                           // }
                        }
                        if (idx == result.Count - 1 || Group != result[idx+1].getGroup())
                        {
                            //Summing the last record
                            TotInTransit += (InTransit[currWeek]);
                            TotWIP += (WIP[currWeek]);
                            TotReleased += (Released[currWeek]);
                            TotSuggested += (Suggested[currWeek]);
                            TotLocked += (Locked[currWeek]);
                        }
                    }// end current week
                    else
                    {
                        //week skipped
                        week1Flag = false;

                        PrevWeek = currWeek;
                        TotInTransit += (InTransit[currWeek]);
                        TotWIP += (WIP[currWeek]);
                        TotReleased += (Released[currWeek]);
                        TotSuggested += (Suggested[currWeek]);
                        TotLocked += (Locked[currWeek]);
                        
                        if (getNextWeek(ref Week, ref weekIdx, ref currWeek, ref FiscWeek, FiscWeekList))
                        {
                            PAB[currWeek] = OnHand[currWeek] = PAB[PrevWeek];
                            idx--; // Used for changing the FiscWeekand exec last object again.
                            continue;
                        }
                        else
                        {
                            // this case will not happen
                            // throw new NotFiniteNumberException("Fiscal week over flow");
                        }
                    }// end week skipped

                } // end for loop result set
                PrevWeek = currWeek;
                while (getNextWeek(ref Week, ref weekIdx, ref currWeek, ref FiscWeek, FiscWeekList))
                {
                    PAB[currWeek] = OnHand[currWeek] = PAB[PrevWeek];
                }
            }

            OnHand.Add("Total", null);
            InTransit.Add("Total", TotInTransit);
            WIP.Add("Total", TotWIP);
            Released.Add("Total", TotReleased);
            Locked.Add("Total", TotLocked);
            Suggested.Add("Total", TotSuggested);
            PAB.Add("Total", null);



            return SupplyBySize;
        }

        private bool getNextWeek(ref int week, ref int weekIdx, ref string currWeek, ref DateTime FiscWeek,
            IList<MaterialPAB> FiscWeekList)
        {
            weekIdx++;
            if (FiscWeekList.Count <= weekIdx) return false;
            FiscWeek = FiscWeekList[weekIdx].WeekEndDate;
            week = (int)FiscWeekList[weekIdx].FiscalWeek;
            currWeek = WeekName + week;
            return true;
        }

        private void AddFullWeeks(Dictionary<String, dynamic> dict)
        {
            for (int i = 1; i < 53; i++)
            {
                dict.Add(WeekName + i, 0m);
            }
        }

        public List<MaterialPAB> MaterialSupplyGetWeeks()
        {
            var searchRepository = new MaterialSupplyRepositoy();
            var result = searchRepository.MaterialSupplyGetWeeks();

            return result;
        }
        public IList<MaterialBlankSupplySearch> GetDC()
        {
            var searchRepository = new MaterialSupplyRepositoy();
            var result = searchRepository.GetDC();
            return result;
        }
    }
}
