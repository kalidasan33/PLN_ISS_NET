using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StyleNavigator.DTO.StyleModule
{
    public class StyleAttribute
    {
        public string ATTRIBUTE_CD { get; set; }
        public string ATTRIBUTE_DESC { get; set; }
        public string ACTIVE_CD { get; set; }
        public string USER_ID { get; set; }
        public string CREATE_DATE { get; set; }
        public string UPDATE_DATE { get; set; }
        public string PROCESS_CD { get; set; }
    }
}