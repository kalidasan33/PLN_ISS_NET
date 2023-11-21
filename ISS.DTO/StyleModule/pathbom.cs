using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISS.DTO.StyleModule
{
    public class PathBom
    {
        public string STYLE_CD { get; set; }
        public string COLOR_CD { get; set; }
        public string ATTRIBUTE_CD { get; set; }
        public string SIZE_CD { get; set; }
        public string MFG_PATH_ID { get; set; }
        public string BILL_OF_MTRLS_ID { get; set; }
        public string ROUTING_ID { get; set; }
        public long Count { get; set; }

        
    }
}