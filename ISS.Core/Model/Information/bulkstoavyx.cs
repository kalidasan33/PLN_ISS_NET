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
    public class BulksToAvyx : ModelBase
    {
        [Display(Name = "Bulk Count")]
        public string BulkCount { get; set; }

        [Display(Name = "Bulk Number")]
        public string BulkNumber { get; set; }

        [Display(Name = "Style")]
        public string StyleCode { get; set; }

        [Display(Name = "Color")]
        public string ColorCode { get; set; }

        [Display(Name = "Attribute")]
        public string AttributeCode { get; set; }

        [Display(Name = "Size")]
        public string SizeShortDesc { get; set; }

        [Display(Name = "Aps Style")]
        public string ApsStyleCode { get; set; }

        [Display(Name = "Aps Color")]
        public string ApsColorCode { get; set; }

        [Display(Name = "Aps Attribute")]
        public string ApsAttributeCode { get; set; }

        [Display(Name = "Aps Size")]
        public string ApsSizeShortDesc { get; set; }

        [Display(Name = "Demand Weekend Date")]
        public DateTime? DemandWeekendDate { get; set; }

        [Display(Name = "Curr Order Qty")]
        public Decimal CurrOrderQty { set; get; }

        [Display(Name = "Corp Business Unit")]
        public string CorpBusinessUnit { set; get; }

        [Display(Name = "Demand Source")]
        public string DemandSource { get; set; }

        [Display(Name = "ProcessedToAvyx")]
        public string ProcessedToAvyx { get; set; }

        [Display(Name = "Create Date")]
        public DateTime CreateDate { get; set; }

        private DateTime? _ReActivatedDate;
        [Display(Name = "Re Activated Date")]
        public DateTime? ReActivatedDate
        {
            get
            {
                return (_ReActivatedDate.HasValue) ? _ReActivatedDate.Value.Date : _ReActivatedDate;
            }
            set
            {
                _ReActivatedDate = value;
            }
        }

        [Display(Name = "Re Activated By")]
        public String ReActivatedBy { set; get; }

        private DateTime? _CompletedDate;
        [Display(Name = "Completed Date")]
        public DateTime? CompletedDate
        {
            get
            {
                return (_CompletedDate.HasValue) ? _CompletedDate.Value.Date : _CompletedDate;
            }
            set
            {
                _CompletedDate = value;
            }
        }

        [Display(Name = "Completed By")]
        public String CompletedBy { set; get; }

        public String Status { set; get; }

        [Display(Name = "Error Message")]
        public String ErrorMsg { set; get; }

        [Display(Name = "From Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? FromDate { set; get; }

        [Display(Name = "To Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? ToDate { set; get; }

    }  
}
