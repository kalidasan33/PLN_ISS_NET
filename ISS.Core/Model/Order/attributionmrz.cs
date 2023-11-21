
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISS.Core.Model.Common;
using System.ComponentModel.DataAnnotations;

namespace ISS.Core.Model.Order
{
    public class AttributionMrz : ModelBase
    {
        [Display(Name = "DC Loc")]
        public String Plant { get; set; }

        [Display(Name = "Request #")]
        public String RequestNumber { get; set; }

        [Display(Name = "Project #")]
        public String projectNumber { get; set; }

        [Display(Name = "From Style")]
        public String FromStyle { get; set; }

        [Display(Name = "From Color")]
        public String FromColor { get; set; }

        [Display(Name = "From Attribute")]
        public String FromAttribute { get; set; }

        [Display(Name = "From Size Cd")]
        public String FromSizeCd { get; set; }

        [Display(Name = "To Style")]
        public String ToStyle { get; set; }

        [Display(Name = "To Color")]
        public String ToColor { get; set; }

        [Display(Name = "To Attribute")]
        public String ToAttribute { get; set; }

        [Display(Name = "To Size Cd")]
        public String ToSizeCd { get; set; }

        [Display(Name = "Expected Date")]
        public DateTime? ExpectedDate { get; set; }

        [Display(Name = "Remarks")]
        public String Remarks { get; set; }

        [Display(Name = "Has Remarks")]
        public bool hasRemarks { get; set; }


        public String GridMode { get; set; }

        [Display(Name = "Order Id")]
        public String OrderId { get; set; }

        [Display(Name = "Super Order")]
        public String SuperOrder { get; set; }

        [Display(Name = "Order Version")]
        public decimal OrderVersion { get; set; }

        [Display(Name = "Style")]
        public String Style { get; set; }

        [Display(Name = "Color")]
        public String Color { get; set; }

        [Display(Name = "Attribute")]
        public String Attribute { get; set; }

        [Display(Name = "Size")]
        public String Size { get; set; }

        [Display(Name = "Mfg Path ID")]
        public String MfgPathId { get; set; }

        [Display(Name = "Current Qty")]
        public decimal CurrQty { get; set; }

        [Display(Name = "Total Qty")]
        public decimal TotalQty { get; set; }

        [Display(Name = "Current Due Date")]
        public DateTime CurrDueDate { get; set; }

        
    }
}
