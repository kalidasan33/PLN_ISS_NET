using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISS.Core.Model.Common;
using System.ComponentModel.DataAnnotations;

namespace ISS.Core.Model.Information
{
    public class SuggestedException : ModelBase
    {


        [Display(Name = "Style")]
        public string Style { get; set; }

        [Display(Name = "Color")]
        public string Color { get; set; }

        [Display(Name = "Attribute")]
        public string Atribute { get; set; }

        [Display(Name = "Size")]
        public string SizeShortDesc { get; set; }

        [Display(Name = "Dmd Loc")]
        public string DmdLoc { get; set; }

        [Display(Name = "Mfg Path")]
        public string MfgPath { get; set; }

        [Display(Name = "Exception")]
        public string Reason { get; set; }

        [Display(Name = "Conflict SKU")]
        public string ConflictSKU { get; set; }

        [Display(Name = "Order Size")]
        public string OrderSize { get; set; }

        [Display(Name = "Revision")]
        public decimal Revision { get; set; }
    }
}
