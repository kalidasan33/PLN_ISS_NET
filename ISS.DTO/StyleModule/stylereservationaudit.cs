using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StyleNavigator.DTO.StyleModule
{
    public class StyleReservationAudit
    {
        public string STYLE_CD { get; set; }
        public string BEFORE_AFTER_CD { get; set; }
        public string CHANGE_DATE { get; set; }
        public string CORP_DIVISION_CD { get; set; }
        public string MATL_TYPE_CD { get; set; }
        public string ORIGIN_TYPE_CD { get; set; }
        public string RSVN_CONTACT { get; set; }
        public string RSVN_STATUS_CD { get; set; }
        public string USER_ID { get; set; }
        public string CREATE_DATE { get; set; }
        public string UPDATE_DATE { get; set; }
        public string UPDATE_CD { get; set; }
        public string TRANSACTION_NO { get; set; }
        public string DATE_TRANSFERRED { get; set; }
        public string AUDIT_PRINT_IND { get; set; }
        public string CORP_BUSINESS_UNIT { get; set; }
    }
}
