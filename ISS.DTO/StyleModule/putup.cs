using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StyleNavigator.DTO.StyleModule
{
    public class PutUp
    {
        public string STYLE_CD { get; set; }
        public string COLOR_CD { get; set; }
        public string ATTRIBUTE_CD { get; set; }
        public string SIZE_CD { get; set; }
        public string PUTUP_CD { get; set; }
        public string STD_CASE_QTY { get; set; }
        public string PACKAGING_CD { get; set; }
        public string PACKAGING_DESC { get; set; }
        public string PREPACK_CASE_UNITS { get; set; }
        public string STD_PRODUCT_WEIGHT { get; set; }
        public string STD_CASE_WEIGHT { get; set; }
        public string INNER_VOLUME { get; set; }
        public string OUTER_VOLUME { get; set; }
        public string GARMENTS_PER_INNER_PACK { get; set; }
        public string PALLET_HI { get; set; }
        public string PALLET_TI { get; set; }
        public string INACTIVE_DATE { get; set; }
        public string APS_OVERRIDE_IND { get; set; }
        public string TRANSMIT_IND { get; set; }
        public string USER_ID { get; set; }
        public string CREATE_DATE { get; set; }
        public string UPDATE_DATE { get; set; }
    }
}