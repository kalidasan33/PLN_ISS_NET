using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISS.Common;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;


namespace KA.Core.Model.MaterialSupply
{
    public class MaterialBlankSupplySearch
    {
       
        [Display(Name = "Style")]
        [Required(ErrorMessage = "Style is required")]
        public String Style { get; set; }

        [Display(Name = "Color")]
        [Required(ErrorMessage = "Color is required")]
        public String Color { get; set; }

        [Display(Name = "Attribute")]
        [Required(ErrorMessage = "Attribute is required")]
        public String Attribute { get; set; }

        [Display(Name = "Size")]
        public String SizeCD { get; set; }

        [Display(Name = "DC")]
        public String DC { get; set; }

        [Display(Name = "Size Desc")]
        public String SizeShortDes { get; set; }

        [Display(Name = "Include suggested lots in PAB calculation")]
        public bool IncludeSuggLots { get; set; }

        public decimal BeginWeek { get; set; }

        public decimal BeginYear { get; set; }
        public decimal EndWeek { get; set; }

        public decimal EndYear { get; set; }

        public String AllSizes { get; set; }
        public String AllDcs { get; set; }

        [Display(Name = "Show values in dozens")]
        public bool ShowDz { get; set; }
        
    }
}
