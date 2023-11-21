using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using ISS.Common;

namespace ISS.Core.Model.Order
{
    public class Requisition 
    {
        public Requisition()
        {
            RequisitionComment = new OrderComment();
            OverPercentage = UnderPercentage = 0;
        }

        [Display(Name = "Requisition Id")]
        [Required]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$",ErrorMessage="Invalid characters")]
        public string RequisitionId { get; set; }

        [Display(Name = "Planning Contact")]
        [Required(ErrorMessage = "Planning Contact is required")]
        public string PlanningContact { get; set; }

        [Display(Name = "Sourcing Contact")]
        [Required(ErrorMessage = "Sourcing Contact is required")]
        public string SourcingContact { get; set; }

        [Display(Name = "Requisition Approver")]
        public string RequisitionApprover { get; set; }

        
        public decimal VendorId { get; set; }

        [Display(Name = "Vendor")]
        [Required(ErrorMessage="Please click on magnifying glass to search/enter a Vendor")]
        [RegularExpression("^([0-9]+)$")]
        public decimal VendorNo { get; set; }

        [Display(Name = "Vendor Name")]         
        public String VendorName { get; set; }

        [Display(Name = "Vendor Desc")] 
        public string VendorDesc { get; set; }

        [Display(Name = "Vendor Lawson Company")]      
        public Decimal LwCompany { get; set; }

        [Display(Name = "Vendor Location")]      
        public string LwVendorLoc { get; set; }
  
        public decimal VendorSuffix { get; set; }

        [Display(Name = "Business Unit")]
        public string BusinessUnit { get; set; }

        [Display(Name = "Season")]
        public string Season { get; set; }

        [Display(Name = "Prod Status")]
        public string ProdStatus { get; set; }

        [Display(Name = "Status")]
        public string ReqStatus { get; set; }

        [Display(Name = "Requisition Status")]
        public string ReqStatusDesc { get; set; }

        [Display(Name = "Created Date")]
        public DateTime? CreatedOn { get; set; }

        [Display(Name = "By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Updated Date")]
        public DateTime? UpdatedOn { get; set; }

        [Required(ErrorMessage = "Dc Loc is required.")]
        [Display(Name = "DC Loc")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$",ErrorMessage="Invalid characters")]
        public string DcLoc { get; set; }

        /// <summary>
        /// CURR_DUE_DATE
        /// </summary>
        [Display(Name = "Planned Ex Factory Date")]
        [Required]
        [RegularExpression(@"^(0[1-9]|1[012])[/](0[1-9]|[12][0-9]|3[01])[/](19|20|21|22)\d\d$",ErrorMessage="Invalid Format")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime PlannedDcDate { get; set; }


        /// <summary>
        /// DC_DUE_DATE
        /// </summary>
        public DateTime? DCDueDate { get; set; }

        [Display(Name = "Program Type")]
        public string ProType { get; set; }

        [Display(Name = "Program Type")]
        public string ProTypeDesc { get; set; }

        [Display(Name = "Mode")]
        public string Mode { get; set; }

        [Display(Name = "Detail Tracking")]
        public bool ReqDetailTracking { get; set; }

        public String ReqDetailTrackingVal
        {             
            set
            {
                ReqDetailTracking = (!String.IsNullOrEmpty(value) && value == LOVConstants.Yes);
            }
            get
            {
                return "N";
            }
        }

        [Display(Name = "Over %")]       
        [RangeAttribute(0, 10)]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Decimal values are invalid")]
        public int? OverPercentage { get; set; }


        private decimal? _OverPercentageD;
        public decimal? OverPercentageD
        {
            get
            {
                return _OverPercentageD;
            }
            set
            {
                _OverPercentageD = value;
                if (value.HasValue)
                    OverPercentage = (int)value.Value;
            }
        }



        [Display(Name = "Under %")]       
        [RangeAttribute(0, 99)]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Decimal values are invalid")]
        public int? UnderPercentage { get; set; }

        private decimal? _UnderPercentageD;
        public decimal? UnderPercentageD
        {
            get
            {
                return _UnderPercentageD;
            }
            set
            {
                _UnderPercentageD = value;
                if (value.HasValue)
                    UnderPercentage = (int)value.Value;
            }
        }


        [Display(Name = "Fis Week")]
        public decimal? FisWk { get; set; }

        [Display(Name = "Show Summary Only")]
        public bool ShowSummaryOnly { get; set; }

        [Display(Name = "By")]
        public string UpdatedBy { get; set; }

        public string MFGPathId { get; set; }

        public string SuperOrder { get; set; }
        public decimal OrderVersion { get; set; }
        public string DemandType { get; set; }
        public DateTime? OriginalDueDate { get; set; }
        public DateTime? DemandDate { get; set; }

        //public string DozsOnlyInd { get; set; }
        //public string CreateBDInd { get; set; }
        public string Priority { get; set; }
        public string SpreadTypeCD { get; set; }

        public DateTime? CurrentDueDate { get; set; }

        public DateTime? ApprovrSubmitDate { get; set; }
        public DateTime? ApprovrResponseDate { get; set; }
        public DateTime? RLSEtoSrcDate { get; set; }
        public DateTime? EXTRbySrcDate { get; set; }
        public DateTime? rejBySrcDate { get; set; }
        public decimal RequisitionVersion { get; set; }



        [Display(Name = "Submitted For Approval")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ApprovalSubmitted { get; set; }

        [Display(Name = "Approved")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Approved { get; set; }

      
        [Display(Name = "Approver User ID")]
        public string RequisitionApproverId { get; set; }

        public OrderComment RequisitionComment { get; set; }

        [Display(Name = "Planning Contact")]
        public string PlannerName { get; set; }

        [Display(Name = "Sourcing Contact")]
        public string SourcingContactName { get; set; }

        public string DcLocName { get; set; }
        public string CropBusinessUnit { get; set; }


       
        
    }
}
