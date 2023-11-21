using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ISS.Core.Model.Order
{
    public class WorkOrderHeader
    {
        //Header Fields
        [Display(Name = "Dmd Type")]
        public string Dmd { get; set; }

        [Display(Name = "Due Date")]
        public string DueDate { get; set; }

         [Display(Name = "Week")]
        public decimal PlannedWeek { get; set; }

         [Display(Name = "Year")]
        public decimal PlannedYear { get; set; }

        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime PlannedDate { get; set; }

        [Display(Name ="DC")]
        //[MaxLength(3, ErrorMessage = "Only 3 Digits are Allowed")]
        //[RegularExpression(@"^[A-Z0-9]+[A-Z0-9-_]*$", ErrorMessage = "Use Only Alpha Numeric")]
        public string Dc { get; set; }

        [Display(Name = "Orders To Create")]
        public decimal OrdersToCreate { get; set; }

        [Display(Name = "Planner Cd")]
        public string PlannerCd { get; set; }

        [Display(Name = "By")]
        public string CreatedBy { get; set; }

        [Display(Name = "By")]
        public string UpdatedBy { get; set; }

        public List<WorkOrderDetail> WODetails { get; set; }

        public IList<WorkOrderCumulative> WOCumulative { get; set; }

        public IList<WorkOrderFabric> WOFabric { get; set; }

        public string TxtPlant { get; set; }
        public string MachinePlant { get; set; }
        //For WOM Save
        public bool SkipConsumeOrders { get; set; }
        public string OrderType { get; set; }
        public string ProductionStatus { get; set; }
        public string DemandSource { get; set; }
        public decimal? Priority { get; set; }
        public string ExpeditePriority { get; set; }
        public string CategoryCode { get; set; }
        public string CreateBDInd { get; set; }
        public string DozensOnlyInd { get; set; }
        public decimal FQQty { get; set; }

        public string SuperOrder { get; set; }
        public decimal MinYear { get; set; }
        public decimal MaxYear { get; set; }
        public string LegalName { get; set; }
    }
}
