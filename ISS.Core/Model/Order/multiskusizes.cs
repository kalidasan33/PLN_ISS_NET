using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISS.Core.Model.Order
{
    public class MultiSKUSizes
    {
        [Display(Name = "Size")]
        public string Size { get; set; }
        public string SizeCD { get; set; }
        [Display(Name = "Dzs")]
        [Range(0.0,1000000000000.0)]
        public decimal Qty { get; set; }
    }
}
