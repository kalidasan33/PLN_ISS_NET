using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ISS.Core.Model.Order
{
    public class VendorSearch
    {
        [MaxLength(8)]
        [Display(Name = "Style")]
        public string ByStyle { get; set; }

        [MaxLength(4)]
        [Display(Name = "Color")]
        public string ByColor { get; set; }

        [Display(Name = "Bus Unit")]
        public string BusUnit { get; set; }

        [Display(Name = "Name")]
        public string ByName { get; set; }

        [Display(Name = "LW Ven Name")]
        public string VendorName { get; set; }

        [Display(Name = "LW City")]
        public string VendorCity { get; set; }

        [Display(Name = "LW Country")]
        public string LwVendorCountry { get; set; }

        [Display(Name = "LW")]
        public decimal LwCompany { get; set; }

        [Display(Name = "LW Ven No")]
        public decimal LwVendorNo { get; set; }

        [Display(Name = "LW Ven Loc")]
        public string LwVendorLoc { get; set; }

        [Display(Name = "Src Ven Id")]
        public decimal VendorId { get; set; }

        [Display(Name = "Src Ven Suffix")]
        public decimal VendorSuffix { get; set; }

        [Display(Name = "Src Plant")]
        public string SrcPlant { get; set; }
    }
}
