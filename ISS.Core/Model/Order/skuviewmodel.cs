using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISS.Core.Model.Order
{
    public class SkuViewModel
    {
        public string Style_Cd { get; set; }
        public string Color_Cd { get; set; }
        public string Attribute_Cd { get; set; }
        public string Size_Cd { get; set; }
        public string Size_Des { get; set; }
        public string Revision_Cd { get; set; }
        public string Asrt_Cd { get; set; }
        public string Rev_Cd { get; set; }
        public string Pak_Cd { get; set; }
        public string DemLoc_Cd { get; set; }
        public string OrginType_Cd { get; set; }
        public string SelectedSKU { get; set; }
        public string Pdc_Cd { get; set; }
        public decimal Qty { get; set; }
        public string ValHaa { get; set; }

        public List<MultiSKUSizes> SizeList { get; set; }
    }
}