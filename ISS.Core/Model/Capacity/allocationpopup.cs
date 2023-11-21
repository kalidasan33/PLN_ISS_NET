using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISS.Core.Model.Common;
using System.ComponentModel.DataAnnotations;

namespace ISS.Core.Model.Capacity
{
    public class AllocationPopup : AllocationDetail
    {
        [Display(Name = "Style")]
        public string StyleCD { get; set; }

        [Display(Name = "Color")]
        public string ColorCD { get; set; }

        [Display(Name = "Attribute")]
        public string AttributeCD{ get; set; }

        [Display(Name = "Size")]
        public string SizeCD { get; set; }

        [Display(Name = "Order No")]
        public string ProdOrderNo { get; set; }
        
        public Decimal SAH { get; set; }

        [Display(Name = "Work Order Qt")]
        public Decimal WOQty { get; set; }

        [Display(Name = "Priority")]
        public Decimal Priority { get; set; }

        [Display(Name = "Due Date")]
        public string FormattedDueDate { get; set; }
    }
}
