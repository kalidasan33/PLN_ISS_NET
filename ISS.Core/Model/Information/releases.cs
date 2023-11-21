using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISS.Core.Model.Common;
using System.ComponentModel.DataAnnotations;

namespace ISS.Core.Model.Information
{
    public class Releases : ModelBase
    {
        private DateTime? _UpdatedDate;
        [Display(Name = "Update Date")]
        public DateTime? UpdatedDate
        {
            get
            {
                return (_UpdatedDate.HasValue) ? _UpdatedDate : _UpdatedDate;
            }
            set
            {
                _UpdatedDate = value;
            }
        }
        

        [Display(Name = "Lot #")]
        public string OrderId { get; set; }

        [Display(Name = "Selling Style")]
        public string SellingStyle { get; set; }

        [Display(Name = "Mfg Style")]
        public string StyleCode { get; set; }

        [Display(Name = "Color")]
        public string ColorCode { get; set; }

        [Display(Name = "Attribute")]
        public string AttributeCode { get; set; }

        [Display(Name = "Size")]
        public string SizeShortDesc { get; set; }

          [Display(Name = "DC Loc")]
        public string DCloc { get; set; }

        [Display(Name = "Sew")]
        public string SewPlant { get; set; }

        [Display(Name = "Cut")]
        public string CutPlant { get; set; }

        [Display(Name = "Txt")]
        public string textilePlant { get; set; }

        [Display(Name = "Total Dozens")]
        public Decimal TotalCurrentOrderQuantity { get; set; }

        [Display(Name = "Status")]
        public string RemoteUpdateCode { get; set; }

        [Display(Name = "Exception")]
        public string Reason { get; set; }

        [Display(Name = "Group #")]
        public string MultiSKU { get; set; }

        [Display(Name = "ISS #")]
        public string SuperOrder { get; set; }

      
      
        [Display(Name = "Cutting Alt")]
        public string CuttingAlt { get; set; }

        [Display(Name = "Fabric Lbs")]
        public Decimal FabricLbs { get; set; }

     

        [Display(Name = "Create BD")]
        public String CreateBD { get; set; }

        [Display(Name = "Dz Only")]
        public String DzOnly { get; set; }

        [Display(Name = "Greige Lbs")]
        public Decimal? GreigeLbs { get; set; }

        [Display(Name = "Out Stream")]
        public String RemoteSystem { get; set; }

        //public string SizeCode { get; set; }
        //public string StyleCode2 { get; set; }
        //public string ColorCode2 { get; set; }
        //public string AttributeCode2 { get; set; }
        //public string SizeCode2 { get; set; }
    }
}
