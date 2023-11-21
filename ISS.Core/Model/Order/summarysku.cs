using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISS.Core.Model.Order
{
    public class SummarySKU
    {
        public string PLANNER_CD { get; set; }
        public string CUT_ALLOC { get; set; }
        public string RULE_NUMBER { get; set; }
        public string DEMAND_SOURCE { get; set; }
        public string SELLING_STYLE_CD { get; set; }
        public string SELLING_COLOR_CD { get; set; }
        public string SELLING_ATTRIBUTE_CD { get; set; }
        public string SELLING_SIZE_CD { get; set; }
        public string SIZE_SHORT_DESC { get; set; }
        public string PLANNING_TM_FNC { get; set; }
        public string CURRENT_QTY { get; set; }
        public string REV_NO { get; set; }
        public string STYLE { get; set; }
    }
}
