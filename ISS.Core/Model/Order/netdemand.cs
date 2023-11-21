using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISS.Core.Model.Order
{
    public class NetDemand
    {
        [Display(Name = "Plant")]
        public string plant { get; set; }

        [Display(Name = "Cat")]
        public string cat { get; set; }

        [Display(Name = "Rule No-Description ")]
        public string rule_number { get; set; }

        [Display(Name = "Rule No")]
        public string rulenumber { get; set; }

        [Display(Name = "Rule Description")]
        public string ruleDescription { get; set; }

        [Display(Name = "Sequence")]
        public decimal priority_sequence { get; set; }

        [Display(Name = "Dzs")]
        public decimal qty { get; set; }

        [Display(Name = "PAB/Net Demand")]
        public decimal NET_Demand { get; set; }

        [Display(Name = "Consumed")]
        public string Consumed { get; set; }

        public string Style { get; set; }
        public string Color { get; set; }
        public string Attribute { get; set; }
        public string Size { get; set; }
        public string Size_Short { get; set; }
        public Boolean Summaize_NetDmd { get; set; }
    }
}
