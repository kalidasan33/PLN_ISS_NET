using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISS.Core.Model.Common;
using System.ComponentModel.DataAnnotations;
using ISS.Common;
namespace ISS.Core.Model.Information
{
    public class BlownAwayLot : ModelBase
    {

        [Display(Name = "Style")]
        public string StyleCode { get; set; }

        [Display(Name = "Color")]
        public string ColorCode { get; set; }

        [Display(Name = "Attribute")]
        public string AttributeCode { get; set; }

        [Display(Name = "Size")]
        public string SizeShortDesc { get; set; }

        [Display(Name = "Plant")]
        public String Plant { set; get; }

        [Display(Name = "Lot Quantity")]
        public Decimal LotQuantity { set; get; }

        [Display(Name = "Lot Quantity")]
        public String LotQuantityStr
        {
            get
            {
                return ((int)(LotQuantity / LOVConstants.Dozen)).ToString() + "/" +
                    ((int)(LotQuantity % LOVConstants.Dozen)).ToString();
            }
        }

        [Display(Name = "Lot ID")]
        public String LotId { set; get; }  
   
        [Display(Name = "Planner Code")]
        public String Planner { set; get; }        

        
        //public string Reason { get; set; }

        private string _reason;
        [Display(Name = "Error / Exception")]
        public string Reason
        {
            get
            {
                return _reason.Trim();
            }
            set
            {
                _reason = value;

            }
        }

    }   
    
}
