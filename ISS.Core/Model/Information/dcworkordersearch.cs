using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISS.Core.Model.Common;
using System.ComponentModel.DataAnnotations;

namespace ISS.Core.Model.Information
{
    public class DCWorkOrderSearch : ModelBase
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
    }
}
