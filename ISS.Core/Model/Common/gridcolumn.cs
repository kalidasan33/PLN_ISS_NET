using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISS.Core.Model.Common
{
    public class GridColumn
    {

        public string Name { get; set; }
        public string Title { get; set; }
        public int? Width { get; set; }
        public string Format { get; set; }
        public string CssClass { get; set; }
        public bool NeedCheckBox { get; set; }



    }
}
