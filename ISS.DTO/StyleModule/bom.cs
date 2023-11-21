using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StyleNavigator.DTO.StyleModule
{
    public class BOM
    {
        public string PARENT_STYLE { get; set; }
        public string PARENT_COLOR { get; set; }
        public string PARENT_ATTRIBUTE { get; set; }
        public string PARENT_SIZE { get; set; }
        public string BILL_OF_MTRLS_ID { get; set; }
        public string COMP_STYLE_CD { get; set; }
        public string COMP_COLOR_CD { get; set; }
        public string COMP_ATTRIBUTE_CD { get; set; }
        public string COMP_SIZE_CD { get; set; }
        public string COMP_CD { get; set; }
        public string ACTIVITY_CD { get; set; }
        public string COST_OPER_CD { get; set; }
        public string WASTE_FACTOR { get; set; }
        public string USAGE { get; set; }
        public string BOM_COST_ROLLUP_IND { get; set; }
        public string RQMT_IND { get; set; }
        public string PUTUP_IND { get; set; }
        public string MARKER_ID { get; set; }
        public string DYE_CD { get; set; }
        public string SPREAD_COMP_CD { get; set; }
        public string CUTTING_ALT { get; set; }
        public string BOM_SPEC_ID { get; set; }
        public string EFFECT_BEGIN_DATE { get; set; }
        public string EFFECT_END_DATE { get; set; }
        public string USER_ID { get; set; }
        public string CREATE_DATE { get; set; }
        public string UPDATE_DATE { get; set; }
        public string BOM_SEQ_NO { get; set; }
        public string WASTE_SUB_1 { get; set; }
        public string WASTE_SUB_2 { get; set; }
        public string WASTE_SUB_3 { get; set; }
        public string Key { get { return GetRecordKey(); } }
        public string Url { get { return GetRecordUrl(); } }

        private string GetRecordKey()
        {
            return string.Format("{0} {1} {2} {3}", COMP_STYLE_CD, COMP_COLOR_CD, COMP_ATTRIBUTE_CD, COMP_SIZE_CD);
        }

        private string GetRecordUrl()
        {
            return string.Format("Home/SkuSearch/{0}/{1}/{2}/{3}", COMP_STYLE_CD, COMP_COLOR_CD, COMP_ATTRIBUTE_CD, COMP_SIZE_CD);
        }
    }
}