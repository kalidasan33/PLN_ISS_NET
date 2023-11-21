using ISS.Core.Model.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISS.Core.Model.Order
{
    public class DCCode : ModelBase
    {
        [Display(Name="DC Code")]
        public string DCD { get; set; }
        [Display(Name = "Description")]
        public string DCDescription { get; set; }

        public string DCDesc
        {
            get
            {
                return DCD + "-" + DCDescription;
            }
        }
    }

    public class CutpathInput
    {
        public string SuperOrder { get; set; }
        public string DyeCode { get; set; }
        public string CutPath { get; set; }
        public string Plant { get; set; }
    }
}
