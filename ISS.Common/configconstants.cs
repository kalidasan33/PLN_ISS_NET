using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISS.Common
{
    public class ConfigConstants
    {
        public class UserRoles
        {
            public const String SC_AVYX_ISS_ADMIN_PLANNER_RW = "SC-AVYX_ISS-ADMIN-PLANNER-RW";
            public const String SC_AVYX_ISS_WO_PLANNER_RW = "SC-AVYX_ISS-WO-PLANNER-RW";
            public const String SC_AVYX_ISS_SO_PLANNER_RW = "SC-AVYX_ISS-SO-PLANNER-RW";
            public const String SC_AVYX_ISS_PLANNER_USER_RW = "SC-AVYX_ISS-PLANNER-USER-RW";
            public const String SC_AVYX_ISS_PLANNER_RO = "SC-AVYX_ISS-PLANNER-RO";
            public const String SC_AVYX_ISS_KA_PLANNER_RW = "SC-AVYX_ISS-KA-PLANNER-RW";
            public const String SC_AVYX_ISS_MRZ_PLANNER_RW = "SC-AVYX_ISS-MRZ-PLANNER-RW";

        }

        public class Controllers
        {
            public const String Order = "order";
            public const String Capacity = "capacity";
            public const String Allocation = "allocation";
            public const String Information = "information";
        }

        public class Actions
        {
            public const String MAIN_MENU = "home/index";
            public const String SUMMARY = "order/summary";
            public const String WORK_ORDER = "order/createworkorder";
            public const String MULTI_SKU = "order/insertmultisku";
            public const String WORK_ORDER_MANAGEMENT = "order/womanagement";
            public const String WORK_ATTRIBUTE_MRZ = "order/AttributeMrz";

            public const String SOURCED_ORDER = "order/createrequisitions";
            public const string SO_ADD = "order/getrequisitiondetail";
            public const string SO_ADD_SAVE = "order/insertrequisition";
            public const string SO_UPDATE_SAVE = "order/updaterequisition";
            public const string SO_ADD_COMMENT = "order/requisitioncommentssave";
            public const string SO_EXPVIEW_ADD_COMMENT = "order/requisitioncommentget";
            public const string SO_RESET_TO_CONSTRUCTION = "order/requisitionresetforconstruction";
            public const string SO_RELEASE_TO_SOURCING = "order/releasetosourcing";

            

            public const String CAPACITY = "capacity/allocation";
            public const String TEXTILES = "textiles/detail";
            public const String INFORMATION_REL = "information/releases";
            public const String INFORMATION_SUG = "information/suggestedexceptions";
            public const String INFORMATION_DCWO = "information/dcworkorders";
            public const String INFORMATION_STYLE = "information/styleexceptions";
            public const String INFORMATION_WO_TEX = "information/stylewotextilegroup";
            public const String INFORMATION_BLOWN = "information/blownawaylots";
            public const String INFORMATION_BULKTOAVYX = "information/bulkstoavyx";
            public const String INFORMATION_BULKSTOONESOURCE = "information/bulkstoonesource";
            public const String INFORMATION_KNIGHTSAPPARELEXPEDITE = "information/knightsapparelexpedite";
            
            
            public const string KA_BULKORDER = "bulkorder/review";
            public const string KA_BULKORDER_SAVE = "bulkorder/updatebulkorder";
            public const string KA_BULKORDER_DELETE = "bulkorder/deletebulkorder";

            public const string KA_ATTRIBUTION_MANAGEMENT = "AttributionOrder/Management";
            public const String KA_ATTRIBUTTED_WORK_ORDER = "AttributionOrder/createworkorder";

            public const String KA_ATTRIBUTTED_WORK_ORDER_SAVE = "AttributionOrder/insertmultisku";

            public const string WO_UPDATE_SAVE = "order/savewomdata";
            public const String KA_ATTRIBUTION_MANAGEMENT_SAVE = "AttributionOrder/savewomdata";

            //newly added....
            public const String KA_BLANK_SUPPLY = "BlankSupply/MaterialBlankSupply";
        }
    }
}
