using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StyleNavigator.DTO.StyleModule
{
    public class ITEM_SIZE_AUDIT
    {
        public string SIZE_CD { get; set; }
        public string BEFORE_AFTER_CD { get; set; }
        public string CHANGE_DATE { get; set; }
        public string UPDATE_CD { get; set; }
        public string TRANSACTION_NO { get; set; }
        public string DATE_TRANSFERRED { get; set; }
        public string AUDIT_PRINT_IND { get; set; }
        public string SIZE_DESC { get; set; }
        public string SIZE_SHORT_DESC { get; set; }
        public string ACTUAL_SIZE_NO { get; set; }
        public string ACTUAL_SIZE_UOM_CD { get; set; }
        public string GTIN_SIZE_CD { get; set; }
        public string SIZE_DESC_LONG { get; set; }
        public string MAJOR_SORT_SIZE_SEQ_CD { get; set; }
        public string MINOR_SORT_SIZE_SEQ_NO { get; set; }
        public string SIZE_ABBREVIATION { get; set; }
        public string USER_ID { get; set; }
        public string CREATE_DATE { get; set; }
        public string UPDATE_DATE { get; set; }
        public string CORP_BUSINESS_UNIT { get; set; }
        public string MFG_BUSINESS_UNIT { get; set; }
        public string USER_BUSINESS_UNIT { get; set; }
    }
}
