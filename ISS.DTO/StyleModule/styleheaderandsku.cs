using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StyleNavigator.DTO.StyleModule
{
    public class StyleHeaderAndSku
    {
        public StyleHeaderAndSku()
        {
            Styles = new List<dynamic>();
            StyleChps = new List<dynamic>();
        }
        public IList<dynamic> Styles { get; set; }
        public IList<dynamic> StyleChps { get; set; }
    }
}