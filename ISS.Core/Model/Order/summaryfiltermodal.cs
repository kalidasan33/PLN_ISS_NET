using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using ISS.Core.Model.Common;

namespace ISS.Core.Model.Order
{
    public class SummaryFilterModal
    {
        //public SummaryFilterModal()
        //{
        //    PlannerCD_List = new List<Planner>();
        //}

        public string Planner { get; set; }

        [Display(Name = "Corp Division")]
        public string CorpDiv { get; set; }

        [Display(Name = "Capacity Group")]
        public string CapacityGroup { get; set; }

        [Display(Name = "Work Center")]
        public string WorkCenter { get; set; }

        [Display(Name = "Rule No")]
        public string RuleNo { get; set; }

        [Display(Name = "Rule Description")]
        public string RuleDesc { get; set; }

        [Display(Name = "Style")]
        public string Style { get; set; }

        [Display(Name = "Color")]
        public string Color { get; set; }

        [Display(Name = "Attribute")]
        public string Attribute { get; set; }

        [Display(Name = "Size")]
        public string Size { get; set; }

        [Display(Name = "RevNo")]
        public decimal rev_no { get; set; }

        public string SortBy { get; set; }

        [Display(Name = "Plan Week")]
        public string PlanWeek { get; set; }


         [Display(Name = "Display As")]
        public string DisplayAs { get; set; }

        [Display(Name = "Excess Include Sugg. WO > WK2")]
        public Boolean ExcessSuggWOGrtr2 { get; set; }

        [Display(Name = "Excess Include Sugg. Spillover")]
        public Boolean ExcessSuggSpillover { get; set; }

        [Display(Name = "Show Sku Breaks")]
        public Boolean SkuBreaks { get; set; }

        [Display(Name = "Summarize Event Demand in Pop-Up")]
        public Boolean SummarizeEventDmd { get; set; }

        [Display(Name = "Sugg. WO Week")]
        public Int32 SuggWO { get; set; }

        [Display(Name = "Released Lots")]
        public bool ReleasedLots { get; set; }

        [Display(Name = "Locked Orders")]
        public bool LockedOrders { get; set; }

        [Display(Name = "Buy Orders")]
        public bool BuyOrders { get; set; }

        [Display(Name = "Suggested WorkOrder WK 1")]
        public bool SuggWOWK1 { get; set; }

        [Display(Name = "Suggested WorkOrder WK 2")]
        public bool SuggWOWK2 { get; set; }

        [Display(Name = "Suggested WorkOrder > WK 2")]
        public bool SuggWOWK2Grtr2 { get; set; }

        [Display(Name = "Spillover")]
        public bool SpillOver { get; set; }

        public DateTime? WeekBeginDate { get; set; }
        public DateTime? WeekEndDate { get; set; }

        //public IList<Planner> PlannerCD_List { get; set; }
    }
}
