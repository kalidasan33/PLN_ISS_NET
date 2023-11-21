using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISS.Core.Model.Common;
using System.ComponentModel.DataAnnotations;

namespace ISS.Core.Model.Capacity
{
    public class AllocationDetail : ModelBase
    {
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Display(Name = "Capacity Group")]
        public string CapacityGroup { get; set; }

        public string Plant { get; set; }

        [Display(Name = "Work Center")]
        public string WorkCenter { get; set; }

        [Display(Name = "Capacity Type")]
        public string CapacityType { get; set; }

        [Display(Name = "Production Status")]
        public string ProductionStatus { get; set; }

        [Display(Name = "Spill Over")]
        public string SpillOver { get; set; }

        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }

        public Decimal ParentWOQty { get; set; }

        public Decimal Price { get; set; }

        public Decimal TotalSAH { get; set; }

        public Decimal OwnerSAH { get; set; }        
    }
}
