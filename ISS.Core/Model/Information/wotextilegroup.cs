using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISS.Core.Model.Common;
using System.ComponentModel.DataAnnotations;

namespace ISS.Core.Model.Information
{
    public class WOTextileGroup : ModelBase
    {

        [Display(Name = "Style")]
        public string StyleCode { get; set; }

        [Display(Name = "Textile Group")]
        public string TextileGroup { get; set; }

        [Display(Name = "User ID")]
        public string UserId { get; set; }

        private DateTime? _UpdatedDate;
        [Display(Name = "Update Date")]
        public DateTime? UpdatedDate
        {
            get
            {
                return (_UpdatedDate.HasValue) ? _UpdatedDate.Value.Date : _UpdatedDate;
            }
            set
            {
                _UpdatedDate = value;
            }
        }

         private DateTime? _CreatedDate;
         [Display(Name = "Create Date")]
         public DateTime? CreatedDate
         {
             get
             {
                 return (_CreatedDate.HasValue) ? _CreatedDate.Value.Date : _CreatedDate;
             }
             set
             {
                 _CreatedDate = value;
             }
         }        

        
        //public string Reason { get; set; }

        private string _reason;
       [Display(Name = "Exception Reason")]
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
