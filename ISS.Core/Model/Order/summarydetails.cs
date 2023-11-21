using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISS.Core.Model.Order
{
    public class SummaryDetails
    {
        public string SKU { get; set; }
        public string RULE_NUMBER { get; set; }
        public string SPILL_OVER_IND { get; set; }
        public string WEEKORD { get; set; }
        public string CURR_ORDER_QTY { get; set; }
        public string REV { get; set; }
        public string PRODUCTION_STATUS { get; set; }
        public string MAKE_OR_BUY_CD { get; set; }
        public string SUPER_ORDER { get; set; }
        public string RFAILED { get; set; }
        public string LOTSIZE { get; set; }

    }
}
