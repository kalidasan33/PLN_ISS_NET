using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISS.Common;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;


namespace KA.Core.Model.MaterialSupply
{
    public class MaterialPAB
    {


        public decimal SumQty { get; set; }

        public DateTime WeekBegDate { get; set; }
        public DateTime WeekEndDate { get; set; }


        public String TranType { get; set; }

        public decimal FiscalWeek { get; set; }

        public decimal FiscalYear { get; set; }

        public String FiscalWeekStr { get; set; }

        public String FiscalWeekTitle { get; set; }


        public String Style { get; set; }


        public String Color { get; set; }


        public String Attribute { get; set; }


        public String SizeCD { get; set; }


        public String DC { get; set; }


        public String Size { get; set; }

        public String SKU { get; set; }
      

        public List<Dictionary<String, dynamic>> SizeList { get; set; }

        public String getGroup()
        { 
                return DC+SizeCD;            
        }

    }
}
