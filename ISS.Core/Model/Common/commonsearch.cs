using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 

namespace ISS.Core.Model.Common
{
    public class CommonSearch
    {
        public CommonSearch()
        {
            AllowSelect = true;
            Columns =new List<GridColumn>();
        }

        public string ReadUrl { get; set; } 

        public bool AllowSelect { get; set; }
        public String GridName { get; set; }

        public bool HidePager { get; set; }
        public bool HideFilter { get; set; }
       


        /// <summary>
        /// Columns with Name and Title
        /// </summary>
        public List<GridColumn> Columns { set; get; }

    }
}
