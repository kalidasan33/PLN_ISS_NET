using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISS.Core.Model.Textiles
{
    public class TextilesSearch
    {
         [Required(ErrorMessage = "Please Select a Business Unit.")]
        [Display(Name = "Business Unit")]
        public string BusinessUnit { get; set; }

         [Required(ErrorMessage = "Please Select a Textile Group.")]
        [Display(Name = "Textile Group")]
        public string TextileGroup { get; set; }

        [Display(Name = "From Wk/Year")]
        public string FromWYear { get; set; }

        [Display(Name = "To Wk/Year")]
        public string ToWYear { get; set; }

        [Display(Name = "Include Suggested Lots")]
        public bool IsSuggestedLotIncluded { get; set; }

        public string Planner { get; set; }

        public string AllocGrid { get; set; }

        public string ViewName { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NoWeeks { get; set; }
    }
}
