using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace StyleNavigator.DTO.StyleModule
{
    public class StyleReservation
    {
        public string STYLE_CD { get; set; }
        public string CORP_DIVISION_CD { get; set; }
        public string MATL_TYPE_CD { get; set; }
        public string ORIGIN_TYPE_CD { get; set; }
        public string RSVN_CONTACT { get; set; }
        public string RSVN_STATUS_CD { get; set; }
        public string USER_ID { get; set; }
        public string CREATE_DATE { get; set; }
        public string UPDATE_DATE { get; set; }
    }
}