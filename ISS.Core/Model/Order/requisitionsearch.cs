using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISS.Core.Model.Order
{
    public class RequisitionSearch
    {
        [Display(Name = "Requisition Id")]
        public string RequisitionId { get; set; }

        [Display(Name = "From Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? FromDate { get; set; }

        [Display(Name = "To Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? ToDate { get; set; }

        [Display(Name = "Prod Status")]
        public string ProdStatus { get; set; }

        [Display(Name = "Planning Contact")]
        public string PlanningContact { get; set; }

        [Display(Name = "Create Date")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "Req Status")]
        public string ReqStatus { get; set; }

        [Display(Name = "Bus Unit")]
        public string BusUnit { get; set; }

        [Display(Name = "Lw Ven Name")]
        public string VendorName { get; set; }

        [Display(Name = "Lw City")]
        public string VenCity { get; set; }

        [Display(Name = "Lw Country")]
        public string VenCountry { get; set; }

        [Display(Name = "Lw Ven No")]
        public decimal VendorNo { get; set; }

        [Display(Name = "Lw Ven Loc")]
        public string VendorLoc { get; set; }

        [Display(Name = "Locked")]
        public DateTime Locked { get; set; }

        [Display(Name = "Size")]
        public string SizeShortDes { get; set; }        
    }
}
