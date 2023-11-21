using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StyleNavigator.DTO.StyleModule
{
    public class SKU
    {
        public string STYLE_CD { get; set; }
        public string COLOR_CD { get; set; }
        public string ATTRIBUTE_CD { get; set; }
        public string SIZE_CD { get; set; }
        public string CUSTOMS_GENDER_CD { get; set; }
        public string FABRIC_CONT_CD { get; set; }
        public string FUTURE_LIFE_CYCLE_CD { get; set; }
        public string FUTURE_LIFE_CYCLE_DATE { get; set; }
        public string HTSUS_TARIFF_CD { get; set; }
        public string INNER_PACK_CD { get; set; }
        public string INVENTORY_TYPE_CD { get; set; }
        public string LIFE_CYCLE_CD { get; set; }
        public string MFG_REVISION_NO { get; set; }
        public string OUTER_PACK_CD { get; set; }
        public string PRIMARY_DC_REVISION_NO { get; set; }
        public string PROD_SEGMENT_CD { get; set; }
        public string STD_PRODUCT_WEIGHT { get; set; }
        public string WALMART_RPLN_ID { get; set; }
        public string WALMART_RPLN_PRICE { get; set; }
        public string X_SIZE_CD { get; set; }
        public string END_DATE_IND { get; set; }
        public string EFFECT_END_DATE { get; set; }
        public string CVS_FABRIC_CSTN_CD { get; set; }
        public string CVS_GARMENT_TYPE_CD { get; set; }
        public string USER_ID { get; set; }
        public string CVS_FABRIC_TYPE_CD { get; set; }
        public string CVS_USER_ID { get; set; }
        public string CREATE_DATE { get; set; }
        public string CVS_UPDATE_DATE { get; set; }
        public string UPDATE_DATE { get; set; }
        public string CUSTOMS_RULING_NO { get; set; }
        public string PLAN_STATUS_CD { get; set; }
        public string REP_STYLE_OVERRIDE { get; set; }
        public string ISS_IND { get; set; }
        public string NRF_COLOR_CD { get; set; }
        public string Key { get { return GetRecordKey(); } }
        private string GetRecordKey()
        {
            return string.Format("{0} {1} {2} {3}", STYLE_CD, COLOR_CD, ATTRIBUTE_CD, SIZE_CD);
        }
    }
}