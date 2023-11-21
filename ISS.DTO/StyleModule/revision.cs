using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StyleNavigator.DTO.StyleModule
{
    public class Revision
    {
        public string STYLE_CD { get; set; }
        public string COLOR_CD { get; set; }
        public string ATTRIBUTE_CD { get; set; }
        public string SIZE_CD { get; set; }
        public string REVISION_NO { get; set; }
        public string BEGIN_EFFECT_DATE { get; set; }
        public string CASES_PER_PALLET { get; set; }
        public string CARTON_ID { get; set; }
        public string END_EFFECT_DATE { get; set; }
        public string GARMENTS_PER_PALLET { get; set; }
        public string PREVIOUS_STORAGE_CD { get; set; }
        public string REVISION_DESC { get; set; }
        public string STD_CASE_QTY { get; set; }
        public string STD_CASE_WEIGHT { get; set; }
        public string STD_PALLET_WEIGHT { get; set; }
        public string STD_PRODUCT_WEIGHT { get; set; }
        public string STD_INNER_PACK_UPS_QTY { get; set; }
        public string STD_INNER_PACK_WHS_QTY { get; set; }
        public string GARMENTS_PER_INNER_PACK { get; set; }
        public string STORAGE_CD { get; set; }
        public string USER_ID { get; set; }
        public string CREATE_DATE { get; set; }
        public string UPDATE_DATE { get; set; }
        public string EARLY_SETUP_IND { get; set; }
        public string RETAIL_DISPLAY_DEPTH { get; set; }
        public string RETAIL_DISPLAY_HEIGHT { get; set; }
        public string RETAIL_DISPLAY_WIDTH { get; set; }
        public string PLAN_START_DATE { get; set; }
        public string PLAN_END_DATE { get; set; }
    }
}