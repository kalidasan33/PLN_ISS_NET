using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISS.Common;
using ISS.Core.Model.Order;

namespace ISS.Core.Model.Common
{
    public class SKU : ModelBase
    {

        public SKU()
        {

        }
        public SKU(RequisitionDetail item)
        {
            Style = item.Style;
            Color = item.Color;
            Attribute = item.Attribute;
            Size = item.Size;
            Rev = item.Rev;
            Qty = item.Qty;
            Dpr = item.Dpr;
            StdCase = item.StdCase;
        }

        public SKU(RequisitionOrder item)
        {
            Style = item.Style;
            Color = item.Color;
            Attribute = item.Attribute;
            Size = item.Size;
            Rev = item.Rev;
            Qty = item.Qty;
            


        }
       
        public string Style { get; set; }


        public string Color { get; set; }


        public string Attribute { get; set; }

        public string AttributeDesc { get; set; }

        public String Size { get; set; }

        public string SizeShortDes { get; set; }

        public string StyleDesc { get; set; }

        public decimal Rev { get; set; }

        public decimal Qty { get; set; }

        public decimal MaxRevision { get; set; }

        public decimal Dpr { get; set; }
        
        public decimal StdCase { get; set; }

        public string Uom { get; set; }

        public string UomDesc { get; set; }

        public string MfgPathId { get; set; }

        public string SewPlt { get; set; }

        public String Sku { get; set; }

        public String getSKUString(bool IncludeRevision=false)
        {
            return Style + LOVConstants.Delimeter + Color + LOVConstants.Delimeter + Attribute + LOVConstants.Delimeter + Size + ((IncludeRevision) ?(LOVConstants.Delimeter+ Rev.ToString()) : String.Empty);
        }

    }

}
