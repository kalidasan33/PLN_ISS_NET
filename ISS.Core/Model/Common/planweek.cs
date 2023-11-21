using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISS.Core.Model.Common
{
    public class PlanWeek : ModelBase
    {
        [Display(Name = "Week Begin Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Week_Begin_Date { get; set; }

        [Display(Name = "Week End Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Week_End_Date { get; set; }

        [Display(Name = "Plan Week")]
        public string ISS_PLAN_WEEK { get; set; }


        public decimal Fiscal_Week { get; set; }

        public decimal Fiscal_Year { get; set; }



    }
}
