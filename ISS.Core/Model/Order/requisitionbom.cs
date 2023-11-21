using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISS.Core.Model.Order
{
    public class RequisitionBOM
    {
        public string BomLevel { get; set; }
        public string Style { get; set; }
        public string StyleDesc { get; set; }
        public string Color { get; set; }
        public string ColorDesc { get; set; }
        public string Attribute { get; set; }
        public string SizeCD { get; set; }
        public string SizeAbb { get; set; }
        public decimal Revision { get; set; }
        public decimal Usage { get; set; }
        public decimal StdQty { get; set; }
        public decimal ParSeq { get; set; }
        public decimal ChildSeq { get; set; }
        public decimal Path { get; set; }
        public decimal UsageSum { get; set; }

        public String Dz { get; set; }
        public String DisplayCases { get; set; }
        public String StyleType { get; set; }


    }
}
