using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StyleNavigator.DTO.StyleModule
{
    public class SkuAttribute
    {
        public SkuAttribute()
        {
            Colors = new List<dynamic>();
            Attributes = new List<dynamic>();
            Sizes = new List<dynamic>();
        }
        public IList<dynamic> Colors { get; set; }
        public IList<dynamic> Attributes { get; set; }
        public IList<dynamic> Sizes { get; set; }
    }
}