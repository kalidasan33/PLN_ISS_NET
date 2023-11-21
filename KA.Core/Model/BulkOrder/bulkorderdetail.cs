using ISS.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KA.Core.Model.BulkOrder
{
    public class BulkOrderDetail
    {
        public BulkOrderDetail()
        {
            OverPercentage = UnderPercentage = 0;

        }
        [Display(Name = "Processed")]
        public string ProcessedToOS { get; set; }


        [Display(Name = "Bulk Number")]
        [Required(ErrorMessage = "Bulk Number is required")]
        public string BulkNumber { get; set; }

        public string LineNumber { get; set; }
        [Required]
        [Display(Name = "Style")]
        public string Style { get; set; }

        [Display(Name = "Color")]
        public string Color { get; set; }


        [Display(Name = "Attribute")]
        public string Attribute { get; set; }

        
        [Display(Name = "Size")]
        public string Size { get; set; }

        [Display(Name = "Size Desc")] 
        public string SizeLit { get; set; }

        public decimal RequisitionVersion { get; set; }

        [Display(Name = "Vendor Desc")]
        public string VendorDesc { get; set; }

        
        [Display(Name = "Requisition Id")]        
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Invalid characters")]
        public decimal RequisitionId { get; set; }
           
        public DateTime? ApproverResponseDate { get; set; }

        [Display(Name = "Reqsn Created Date")]
        public DateTime? ReqCreateDate { get; set; }

        [Display(Name = "Created Date")]
        public DateTime? CreatedOn { get; set; }

        [Display(Name = "By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Updated Date")]
        public DateTime? UpdatedOn { get; set; }

        
        public decimal VendorId { get; set; }
        
        public string VendorIdStr
        {
            get
            {
                return string.Empty;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    decimal i;
                    if (decimal.TryParse(value, out i))
                        VendorId = i;
                }
            }
        }

        public decimal VendorSuffix { get; set; }

        [Display(Name = "Planning Contact")]
        [Required(ErrorMessage = "Planning Contact is required")]
        public string PlanningContact { get; set; }

        public string OrderType { get; set; }

        [Display(Name = "Status")]
        public string ReqStatus { get; set; }

        [Display(Name = "Prod Status")]
        public string ProdStatus { get; set; }

        [Display(Name = "Sourcing Contact")]
        [Required(ErrorMessage = "Sourcing Contact is required")]
        public string SourcingContact { get; set; }

        [Display(Name = "Requisition Approver")]
        public string RequisitionApprover { get; set; }

        [Required(ErrorMessage = "Dc Loc is required.")]
        [Display(Name = "DC Loc")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Invalid characters")]
        public string DcLoc { get; set; }

        [Display(Name = "Curr Due Date")]
        public DateTime? CurrDueDate { get; set; }

        [Display(Name = "Season")]
        public string Season { get; set; }

        [Display(Name = "Mode")]
        public string TranspMode { get; set; }

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

        [Display(Name = "Corp Business Unit")]
        public string BusinessUnit { get; set; }

        [Display(Name = "Vendor Lawson Company")]
        public Decimal LwCompany { get; set; }

        [Display(Name = "Vendor")]
        public decimal? VendorNo { get; set; }


        [Display(Name = "Vendor Location")]
        public string LwVendorLoc { get; set; }

        [Display(Name = "Program Type")]
        public string ProType { get; set; }

        [Display(Name = "Detail Tracking")]
        public bool DetailTrkgInd { get; set; }

        public String DetailTrkgIndVal
        {
            set
            {
                DetailTrkgInd = (!String.IsNullOrEmpty(value) && value == LOVConstants.Yes);
            }
            get
            {
                return "N";
            }
        }

        [Display(Name = "Created Date")]
        public DateTime? ReqCreatedDate { get; set; }


        [Required(ErrorMessage = "Mfg Path is required.")]
        [Display(Name = "MFG Path")]
        public string MFGPathId { get; set; }

        public decimal Rev { get; set; }

        public String Revision
        {
            get
            {
                return string.Empty;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    decimal i;
                    if (decimal.TryParse(value, out i))
                        Rev = i;
                }
            }
        }


        [Display(Name = "UOM")]
        public string Uom { get; set; }

        public decimal TotatalCurrOrderQty { get; set; }

        [Display(Name = "Qty")]
        public decimal Qty { get; set; }

       

        public bool IsInserted { get; set; }

        /// <summary>
        /// Kendo grid property
        /// </summary>
        public bool IsDirty { get; set; }

        public bool IsDeleted { get; set; }

        public bool isHide { get; set; }

        [Display(Name = "Error Status")]
        public bool ErrorStatus { get; set; }

        [Display(Name = "Errors")]
        public string ErrorMessage { get; set; }


        [Display(Name = "Errors")]
        public string Exception { get; set; }

        public string VendorName { get; set; }

        public string Plant { get; set; }

        public DateTime? PlannedDcDate { get; set; }

        [Display(Name = "Qty")]
        public string Quantity { get { return this.Qty.ToString("0.00").Replace('.', '-'); } }

        #region Bulk Errors
        [Display(Name = "Program Source")]
        public string ProgramSource { get; set; }

        [Display(Name = "Program Source")]
        public string ProgramSourceDesc { get; set; }

        public string ContactPlannerCd { get; set; }
        [Display(Name = "APS Style")]
        public string APSStyle { get; set; }

        [Display(Name = "APS Color")]
        public string APSColor { get; set; }


        [Display(Name = "APS Attribute")]
        public string APSAttribute { get; set; }


        [Display(Name = "APS Size CD")]
        public string APSSize { get; set; }
        #endregion

        #region Component Preprocessor
        [Display(Name = "Demand Source")]
        public string DemandSource { get; set; }

        [Display(Name = "Priority Seq")]
        public decimal PrioritySeq { get; set; }
        public string UserId { get; set; }
        [Display(Name = "Demand Week End Date")]
        public DateTime? DemandWeekEndDate { get; set; }
        
        [Display(Name = "APS Size")]
        public string APSSizeLit { get; set; }
        [Display(Name = "Demand Week End Date")]
        public String DmdWkEndDate
        {
            get
            {
                //return (DemandWeekEndDate.HasValue ? DemandWeekEndDate.Value.ToString(LOVConstants.DateFormatDisplay) : ""); 
                return (DemandWeekEndDate.HasValue ? DemandWeekEndDate.Value.Year == 1 ? string.Empty : DemandWeekEndDate.Value.ToString(LOVConstants.DateFormatDisplay) : string.Empty); 
            }
        }

        [Display(Name = "Curr Due Date")]
        public string CurrDueDateStr
        {
            get
            {
                return (CurrDueDate.HasValue ? CurrDueDate.Value.ToShortDateString() : "");
            }
        }
        #endregion
    }
}
