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
    public class BulkstoOneSource : ModelBase
    {
        //
        [Display(Name = "Bulk Number")]
        public string BulkNumber { get; set; }

        [Display(Name = "Line Number")]
        public string LineNumber { get; set; }

        [Display(Name = "Style")]
        public string StyleCode { get; set; }

        [Display(Name = "Color")]
        public string ColorCode { get; set; }

        [Display(Name = "Attribute")]
        public string AttributeCode { get; set; }

        [Display(Name = "Size")]
        public string SizeShortDesc { get; set; }

        [Display(Name = "Requisition")]
        public decimal? RequisitionId { get; set; }

        private DateTime? _CurrDueDate;
        [Display(Name = "Curr Due Date")]
        public DateTime? CurrDueDate
        {
            get
            {
                return (_CurrDueDate.HasValue) ? _CurrDueDate.Value.Date : _CurrDueDate;
            }
            set
            {
                _CurrDueDate = value;
            }
        }

        private DateTime? _ApproveDate;
        [Display(Name = "Approve Date")]
        public DateTime? ApproveDate
        {
            get
            {
                return (_ApproveDate.HasValue) ? _ApproveDate.Value.Date : _ApproveDate;
            }
            set
            {
                _ApproveDate = value;
            }
        }

        private DateTime? _CreateDate;
        [Display(Name = "Create Date")]
        public DateTime? CreateDate
        {
            get
            {
                return (_CreateDate.HasValue) ? _CreateDate.Value.Date : _CreateDate;
            }
            set
            {
                _CreateDate = value;
            }
        }
        
        [Display(Name = "Contact Planner")]
        public string ContactPlannerCd { set; get; }

        [Display(Name = "Src Contact")]
        public string SrcContactCd { get; set; }

        [Display(Name = "Demand Loc")]
        public string DemandLoc { set; get; }

        [Display(Name = "Corp Business Unit")]
        public string CorpBusinessUnit { get; set; }

        [Display(Name = "Mfg Path Id")]
        public string MfgPathId { set; get; }

        [Display(Name = "Mfg Revision No")]
        public string MfgRevisionNo { get; set; }

        [Display(Name = "Curr Order Qty")]
        public decimal CurrOrderQty { set; get; }

        [Display(Name = "Plant Cd")]
        public string PlantCd { get; set; }

        [Display(Name = "Processed To Os")]
        public string ProcessedToOs { get; set; }

        [Display(Name = "Reqsn Status")]
        public string ReqsnStatus { get; set; }


        [Display(Name = "Reqsn Create Date")]
        public DateTime ReqsnCreateDate { get; set; }

        [Display(Name = "Parent Style")]
        public string ParentStyle { get; set; }

        [Display(Name = "Comp Style")]
        public string CompStyle { get; set; }

        [Display(Name = "Comp Color")]
        public string CompColor { set; get; }

        [Display(Name = "Comp Attribute")]
        public string CompAttribute { get; set; }

        [Display(Name = "Comp Size")]
        public string CompSize { set; get; }

        [Display(Name = "Ext Style")]
        public string ExternalStyle { get; set; }

        [Display(Name = "Ext Attribute")]
        public string ExternalAttribute { set; get; }

        [Display(Name = "Ext Size")]
        public string ExternalSize { set; get; }

        [Display(Name = "Ext Version")]
        public string ExternalVersion { get; set; }

        [Display(Name = "Ext Logo")]
        public string ExternalLogo { set; get; }

        [Display(Name = "Graphic")]
        public string Graphic { get; set; }

        [Display(Name = "Placement")]
        public string Placement { set; get; }

        [Display(Name = "Error Message")]
        public string ErrMessage { set; get; }


        //private DateTime? _ReActivatedDate;
        //[Display(Name = "Re Activated Date")]
        //public DateTime? ReActivatedDate
        //{
        //    get
        //    {
        //        return (_ReActivatedDate.HasValue) ? _ReActivatedDate.Value.Date : _ReActivatedDate;
        //    }
        //    set
        //    {
        //        _ReActivatedDate = value;
        //    }
        //}
        private DateTime? _OrgnCreateDate;
        [Display(Name = "Original Create Date")]
        public DateTime? OrgnCreateDate
        {
            get
            {
                return (_OrgnCreateDate.HasValue) ? _OrgnCreateDate.Value.Date : _OrgnCreateDate;
            }
            set
            {
                _OrgnCreateDate = value;
            }
        }

        [Display(Name = "From Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? FromDate { set; get; }

        [Display(Name = "To Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? ToDate { set; get; }
    }
}
