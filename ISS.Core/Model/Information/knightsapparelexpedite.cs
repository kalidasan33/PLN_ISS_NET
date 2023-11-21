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
    public class KnightsApparelExpedite : ModelBase
    {
        [Display(Name = "Bulk Number")]
        public string BulkNumber { get; set; }

        [Display(Name = "Style")]
        public string StyleCode { get; set; }

        [Display(Name = "Color")]
        public string ColorCode { get; set; }

        [Display(Name = "Attribute")]
        public string AttributeCode { get; set; }

        [Display(Name = "Size Description")]
        public string SizeShortDesc { get; set; }

        [Display(Name = "Size")]
        public string SizeCode { get; set; }

        private DateTime? _ShipDate;
        [Display(Name = "Ship Date")]
        public DateTime? ShipDate
        {
            get
            {
                return (_ShipDate.HasValue) ? _ShipDate.Value.Date : _ShipDate;
            }
            set
            {
                _ShipDate = value;
            }
        }

        private DateTime? _DemandDate;
        [Display(Name = "Demand Date")]
        public DateTime? DemandDate
        {
            get
            {
                return (_DemandDate.HasValue) ? _DemandDate.Value.Date : _DemandDate;
            }
            set
            {
                _DemandDate = value;
            }
        }

        [Display(Name = "Gross Requirement")]
        public decimal? GrossRequirement { get; set; }

        [Display(Name = "Net Demand Requirement")]
        public decimal  NetDemandRequirement { get; set; }

        [Display(Name = "Order Number")]
        public decimal? ProOrderNo { get; set; }

        [Display(Name = "In-Transit to DC")]
        public decimal? InTransitToDC { get; set; }

        [Display(Name = "Packing")]
        public decimal? Packing { get; set; }

        [Display(Name = "Issued To WIP")]
        public decimal? IssuedToWIP { get; set; }

        [Display(Name = "OnSite")]
        public decimal? OnSite { get; set; }

        [Display(Name = "Planned But Not Picked")]
        public decimal? PlannedButNotPicked { get; set; }

        [Display(Name = "Not Planned")]
        public decimal? NotPlanned { get; set; }

        [Display(Name = "From Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? FromDate { set; get; }

        [Display(Name = "To Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? ToDate { set; get; }
    }
}
