using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISS.Core.Model.Textiles
{
    public class TextileFilterModel
    {
        public string PlantFilter { get; set; }
        public string HeadsizeFilter { get; set; }
        public string DyeFilter { get; set; }
        public string CutSizeFilter { get; set; }
        public string MachineFilter { get; set; }
        public string YarnFilter { get; set; }
        public string AllocationGrid { get; set; }
        public bool IsSuggestIncluded { get; set; }
        public string ViewName { get; set; }
    }
}