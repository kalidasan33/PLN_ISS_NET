using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISS.Core.Model.Common;
using System.ComponentModel.DataAnnotations;
using ISS.Common;

namespace ISS.Core.Model.Order
{
    public class RequisitionOrderSearch:ModelBase
    {
        [Display(Name = "Vendor No")]
        public decimal VendorNo { get; set; }

        [Display(Name = "Vendor Loc")]
        public string VendorLoc { get; set; }


        public RequisitionOrderSearch()
        {

            SuggWO = LOVConstants.DefaultSuggWO;
        }
       
        [MaxLength(15)]
        [Display(Name = "Work Center")]       
        public String WorkCenter { set; get; }

        [MaxLength(5)]
        public String Planner { set; get; }

        [Display(Name = "Style")]
        public string Style { get; set; }

        [Display(Name = "Color")]
        public string Color { get; set; }

        [Display(Name = "Attribute")]
        public string Attribute { get; set; }

        [Display(Name = "Size")]
        public string Size { get; set; }

        [Display(Name = "Rev")]
        public string Rev { get; set; }

        [Display(Name = "UOM")]
        public string Uom { get; set; }

        [Display(Name = "Qty")]
        public decimal Quantity { get; set; }

        [Display(Name = "Sew Plant")]
        public string SewPlant { get; set; }

        [Display(Name = "DC")]
        public string Dc { get; set; }

        [Display(Name = "Sew Date")]
        public DateTime? SewDate { get; set; }

        [Display(Name = "DC Due Date")]
        public DateTime? DcDueDate { get; set; }

        [Display(Name = "Spill Over")]
        public string SpillOver { get; set; }

        [Display(Name = "DPR Rule")]
        public string DPRRule { get; set; }

        [Display(Name = "DPR Rule Desc")]
        public string DPRRuleDescription { get; set; }

        [Display(Name = "Sugg. WO Week")]
        public Int32 SuggWO { get; set; }
    }
}
