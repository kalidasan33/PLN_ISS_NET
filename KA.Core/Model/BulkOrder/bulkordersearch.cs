using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KA.Core.Model.BulkOrder
{
    public class BulkOrderSearch
    {

        [Display(Name = "Bulk Number")]
        public string BulkNumber { get; set; }

        [Display(Name = "From Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? FromDate { get; set; }

        [Display(Name = "To Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? ToDate { get; set; }

        [Display(Name = "Exclude Processed")]
        public bool ExcludeProcessed { get; set; }  

    }
}
