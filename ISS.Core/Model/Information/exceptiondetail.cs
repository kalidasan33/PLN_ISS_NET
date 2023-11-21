using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISS.Core.Model.Common;
using System.ComponentModel.DataAnnotations;

namespace ISS.Core.Model.Information
{
    public class ExceptionDetail : ModelBase
    {      
      
        [Display(Name = "Style")]
        public string Style { get; set; }
      
        [Display(Name = "Color")]
        public string Color { get; set; }

        [Display(Name = "Attribute Cd")]
        public string Attribute { get; set; }

        [Display(Name = "Size Cd")]
        public string SizeShortDesc { get; set; }
 

        [Display(Name = "Exception")]
        public string Reason { get; set; }       

        [Display(Name = "ISS #")]
        public string SuperOrder { get; set; }
      
    }
}
