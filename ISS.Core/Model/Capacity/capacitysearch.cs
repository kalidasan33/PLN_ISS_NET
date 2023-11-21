using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISS.Core.Model.Common;
using System.ComponentModel.DataAnnotations;


namespace ISS.Core.Model.Capacity
{
    public class CapacitySearch : ModelBase
    {
        [Display(Name = "Capacity Group")]
        [Required(ErrorMessage = "Please select Capacity group")]
        public string CapacityGroup { get; set; }
  
        
        [Required(ErrorMessage = "Please select a plant")]
        public string Plant { get; set; }

        [Display(Name="Work Center")]
        [Required(ErrorMessage="Please select a work center")]
        public string WorkCenter { get; set; }

        public string PlanEndDate { get; set; }

        public string CapacityType { get; set; }

        public string ProductionStatus { get; set; }

        public string SpillOver { get; set; }

        public bool ShowIndivWorkcenters { get; set; }

        public bool ShowAggregateWorkcenters { get; set; }

        [Display(Name = "Order By")]
        public string OrderBy { get; set; }


    }
}
