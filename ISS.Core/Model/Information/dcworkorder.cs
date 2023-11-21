using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISS.Core.Model.Common;
using System.ComponentModel.DataAnnotations;

namespace ISS.Core.Model.Information
{
    public class DCWorkOrder : ModelBase
    {




        private DateTime? _UpdatedDate;
        [Display(Name = "Update Date")]
        public DateTime? CreatedDate
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

        [Display(Name = "DC Loc")]
        public string Plant { get; set; }

        [Display(Name = "Request #")]
        public string RequestNumber { get; set; }

        [Display(Name = "Project #")]
        public string projectNumber { get; set; }   

        [Display(Name = "From Style")]
        public string FromStyle { get; set; }

        [Display(Name = "From Color")]
        public string FromColor { get; set; }

        [Display(Name = "From Attribute")]
        public string FromStyleAttribute { get; set; }

        [Display(Name = "From Size Cd")]
        public string FromSizeCd { get; set; }

        [Display(Name = "To Style")]
        public string ToStyle { get; set; }

        [Display(Name = "To Color")]
        public string ToColor { get; set; }

        [Display(Name = "To Attribute")]
        public string ToStyleAttribute { get; set; }

        [Display(Name = "To Size Cd")]
        public string ToSizeCd { get; set; }

        [Display(Name = "Original Dozens")]
        public Decimal OriginalDozens { get; set; }

        [Display(Name = "Complete Dozens")]
        public Decimal CompleteDozens { get; set; }

        [Display(Name = "Pending Dozens")]
        public Decimal PendingDozens { get; set; }

        private DateTime? _ExpectedDate;
        [Display(Name = "Expected Date")]
        public DateTime? ExpectedDate
        {
            get
            {
                return (_ExpectedDate.HasValue) ? _ExpectedDate.Value.Date : _ExpectedDate;
            }
            set
            {
                _ExpectedDate = value;
            }
        }

        private string _remarks;
        [Display(Name = "Remarks")]
        public string Remarks
        {
            get
            {
                return (_remarks+string.Empty).Trim();   
                //       To fix the issue with Null value handling in remark column CA# 361133-16
            
               
            }
            set
            {
                _remarks = value;
            }
        }
        //public string Remarks { get; set; }

    }   
    
}
