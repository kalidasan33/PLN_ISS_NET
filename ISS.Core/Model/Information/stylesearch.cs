using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISS.Core.Model.Common;
using System.ComponentModel.DataAnnotations;

namespace ISS.Core.Model.Information
{
    public class StyleSearch : ModelBase
    {


        [Display(Name = "Style")]
        public string StyleCode { get; set; }

        [Display(Name = "Color")]
        public string ColorCode { get; set; }

        [Display(Name = "Attribute")]
        public string AttributeCode { get; set; }

        [Display(Name = "Size")]
        public string SizeShortDesc { get; set; }

        [Display(Name = "Primary DC")]
        public String PrimaryDC { get; set; }
              
        [Display(Name = "Line of Business")]
        public String LOB { get; set; }

        [MaxLength(5)]
        [Display(Name = "Planner Code")]
        public String Planner { set; get; }

        [MaxLength(15)]
        [Display(Name = "Work Center")]
        [Required]
        public String WorkCenter { set; get; }

        [Display(Name = "APS")]
        public bool APS { get; set; }

        [Display(Name = "AVYX")]
        public bool AVYX { get; set; }

        [Display(Name = "ISS")]
        public bool ISS { get; set; }

        [Display(Name = "NET")]
        public bool NET { get; set; }

        [Display(Name = "CWC")]
        public bool CWC { get; set; }

        [Display(Name = "MATL-A")]
        public bool MTLA { get; set; }

        [Display(Name = "Plant")]
        public String Plant { get; set; }

        [Display(Name = "Error / Exception")]
        public String Reason { get; set; }      
 
    }
}
