using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ISS.Core.Model.Order
{
    public class WorkOrderTextiles
    {
        //Header Fields in Textile/Fabric
        [Display(Name = "Txt Plant")]
        public string TxtPlant { get; set; }

        [Display(Name = "Machine Type")]
        public string MacType { get; set; }

        [Display(Name = "Limit")]
        public decimal Limit { get; set; }

       // [Display(Name = "Variance")]
        public string Variance { get; set; }

       

    }
}
