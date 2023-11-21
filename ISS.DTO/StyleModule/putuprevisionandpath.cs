using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StyleNavigator.DTO.StyleModule
{
    public class PutUpRevisionAndPath
    {
        public PutUpRevisionAndPath()
        {
            PutUps = new List<dynamic>();
            Revisions = new List<dynamic>();
            Paths = new List<dynamic>();
        }
        public IList<dynamic> PutUps { get; set; }
        public IList<dynamic> Revisions { get; set; }
        public IList<dynamic> Paths { get; set; }
    }
}