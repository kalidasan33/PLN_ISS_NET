using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISS.Common;
using ISS.Core.Model.Common;
using System.ComponentModel.DataAnnotations;

namespace ISS.Core.Model.Information
{
    public class StyleException : ModelBase
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
       
        [Display(Name = "Work Center")]
        public String WorkCenter { set; get; }

        [Display(Name = "Line of Business")]
        public String LOB { get; set; }
         
        [Display(Name = "Planner Code")]
        public String Planner { set; get; }

        [Display(Name = "Demand")]
        public decimal Demand { get; set; }

        [Display(Name = "Demand")]
        public String DemandStr { get {
            return ((int)(Demand / LOVConstants.Dozen)).ToString() + "/" +
                ((int)(Demand % LOVConstants.Dozen)).ToString();
        } }

        [Display(Name = "Path ID")]
        public string MFGPath { get; set; }

        [Display(Name = "Info")]
        public string ProductFamily { get; set; }

        private string _reason;
        [Display(Name = "Rule / Reason")]
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
        //public string Reason { get; set; } 
    }   
    
}
