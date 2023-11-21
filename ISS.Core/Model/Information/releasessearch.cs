using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISS.Core.Model.Common;
using System.ComponentModel.DataAnnotations;

namespace ISS.Core.Model.Information
{
    public class ReleasesSearch : ModelBase
    {
        [MaxLength(15)]
        [Display(Name = "Work Center")]
        [Required]
        public String WorkCenter { set; get; }

        [MaxLength(5)]
        public String Planner { set; get; }

        [MaxLength(6)]
        [Display(Name = "Selling Style")]
        public String SellingStyle { set; get; }

        [MaxLength(6)]
        [Display(Name = "Mfg Style")]
        public String MfgStyle { set; get; }

        [MaxLength(4)]//PFE
        public String Color { set; get; }

        [MaxLength(6)]
        public String Attribute { set; get; }

        [MaxLength(3)]
        [Display(Name = "Cut Plant")]
        public String CutPlant { set; get; }

        [MaxLength(3)]
        [Display(Name = "Sew Plant")]
        public String SewPlant { set; get; }

        [MaxLength(3)]
        [Display(Name = "Textile Plant")]
        public String TextilePlant { set; get; }

        [MaxLength(8)]
        [Display(Name = "Login Id")]
        public String LoginId { set; get; }

        [MaxLength(3)]
        public String Style { set; get; }

    }
}
