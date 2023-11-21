using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISS.Core.Model.Common;
using System.ComponentModel.DataAnnotations;
using ISS.Common;


namespace ISS.Core.Model.Order
{
    public class RequisitionOrder : ModelBase
    {
        [Display(Name = "Vendor #")]
        public Decimal? VendorNo { get; set; }

        [Display(Name = "Vendor Loc")]
        public string VendorLoc { get; set; }

        public string Planner { get; set; }

        public string WorkCenter { get; set; }

        [Display(Name = "DPR Rule")]
        public decimal RuleNo { get; set; }
        [Display(Name = "DPR Rule Desc")]
        public string RuleDescription { get; set; }

        [Display(Name = "Style")]
        public string Style { get; set; }

        [Display(Name = "Color")]
        public string Color { get; set; }

        [Display(Name = "Attribute")]
        public string Attribute { get; set; }

        [Display(Name = "Size Code")]
        public string Size { get; set; }

        public decimal Rev { get; set; }
        public string UOM { get; set; }
        public decimal Qty { get; set; }

       

        
        [Display(Name = "Sew Plant")]
        public string SewPlant { get; set; }
        public string Dc { get; set; }

        [Display(Name = "Dc Date")]
        public DateTime? DcDate { get; set; }

        [Display(Name = "Sew Date")]
        public DateTime? SewDate { get; set; }

        public string S { get; set; }
        public string T { get; set; }
        
        public string V { get; set; }
        public string W { get; set; }
        public string X { get; set; }

        public decimal OrderVersion { get; set; }

         [Display(Name = "ISS #")]
        public string SuperOrder { get; set; }

        [Display(Name="SpillOver")]
        public string Enforcement { get; set; }

         [Display(Name = "DPR Priority Sequence")]
        public decimal Priority { get; set; }
        public string RequisitionId { get; set; }
        public string RequisitionVer { get; set; }
         
        [Display(Name = "Size")]
        public string SizeLit { get; set; }
        public string StyleDesc { get; set; }
        public string ProductionStatus { get; set; }
        public decimal StdCaseQty { get; set; }
        public DateTime? EarliestStart { get; set; }
        public DateTime? PlanDate { get; set; }

        public decimal PlannedLeadTime { get; set; }
        public decimal TransportationTime { get; set; }

        public String getSKUString(bool IncludeRevision=false)
        {
            return Style + LOVConstants.Delimeter + Color + LOVConstants.Delimeter + Attribute + LOVConstants.Delimeter + Size + ((IncludeRevision) ?(LOVConstants.Delimeter+ Rev.ToString() ): String.Empty);
        }
    }
}
