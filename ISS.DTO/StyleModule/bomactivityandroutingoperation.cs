using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StyleNavigator.DTO.StyleModule
{
    public class BomActivityAndRoutingOperation
    {
        public BomActivityAndRoutingOperation()
        {
            Boms = new List<dynamic>();
            Activities = new List<dynamic>();
            Operations = new List<dynamic>();
            CutSpecs = new List<dynamic>();
            CutSpecStructs = new List<dynamic>();
        }
        public IList<dynamic> Boms { get; set; }
        public IList<dynamic> Activities { get; set; }
        public IList<dynamic> Operations { get; set; }
        public IList<dynamic> CutSpecs { get; set; }
        public IList<dynamic> CutSpecStructs { get; set; }
    }
}