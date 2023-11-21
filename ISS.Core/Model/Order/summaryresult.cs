using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISS.Core.Model.Order
{
    public class SummaryResult
    {
        [Display(Name = "Planner")]
        public string planner_cd { get; set; }

        [Display(Name = "Work Center")]
        public string cut_alloc { get; set; }

        [Display(Name = "Rule No")]
        public Decimal rule_number { get; set; }

        public string _demand_source { get; set; }

        [Display(Name = "Rule Description")]
        public string demand_source
        {
            get
            {
                if (!String.IsNullOrEmpty(_demand_source))
                    return _demand_source.Replace("  ", " ");
                return string.Empty;
            }
            set
            {
                _demand_source = value;
            }
        }

        [Display(Name = "Rev No")]
        public Decimal rev_no { get; set; }

        [Display(Name = "Selling Style")]
        public string selling_style_cd { get; set; }

        [Display(Name = "Color")]
        public string selling_color_cd { get; set; }

        [Display(Name = "Attribute")]
        public string selling_attribute_cd { get; set; }

        [Display(Name = "Selling Size")]
        public string selling_size_cd { get; set; }

        [Display(Name = "Size")]
        public string size_short_desc { get; set; }

        [Display(Name = "Total Net Demand")]
        public Decimal TotalNetDemand { get; set; }

        [Display(Name = "Excess")]
        public Decimal Excess { get; set; }

        [Display(Name = "Excess % Lot")]
        public Decimal ExcessLot { get; set; }

        [Display(Name = "Excess % Net Demand")]
        public Decimal ExcessNetDemand { get; set; }

        [Display(Name = "Balance to Lock / Release")]
        public Decimal LockOrReleaseBal { get; set; }

        [Display(Name = "Released Lots")]
        public Decimal Released { get; set; }

        [Display(Name = "Locked Orders")]
        public Decimal Locked { get; set; }

        [Display(Name = "Buy Orders")]
        public Decimal BuyOrders { get; set; }

        [Display(Name = "Suggested WorkOrder WK 1")]
        public Decimal SugWK1 { get; set; }

        [Display(Name = "Suggested WorkOrder WK 2")]
        public Decimal SugWK2 { get; set; }

        [Display(Name = "Suggested WorkOrder WK > 2")]
        public Decimal SugWK3Plus { get; set; }

        [Display(Name = "Suggested Spillover")]
        public Decimal SpillOver { get; set; }

        [Display(Name = "Suggested Lots Comments AS of Last Run")]
        public string SuggestedLotsComments { get; set; }


        public Decimal planning_tm_fnc { get; set; }

        public string style { get; set; }

        public string sku { get; set; }
        public string spill_over_ind { get; set; }
        public Decimal weekord { get; set; }
        public Decimal curr_order_qty { get; set; }
        public string zero { get; set; }
        public string production_status { get; set; }
        public string make_or_buy_cd { get; set; }
        public string super_order { get; set; }
        public decimal RFailed { get; set; }
        public Decimal lotSize { get; set; }

        public bool SkuBreakRow { get; set; }
        public decimal TimeFence { get; set; }

        public string AttributionInd { get; set; }
       
    }
}
