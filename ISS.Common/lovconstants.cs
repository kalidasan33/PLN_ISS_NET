using System;

namespace ISS.Common
{
    public class LOVConstants
    {
        public static readonly int[] PageSizes = {  20, 40, 60, 80, 100 };

        public const Int32 DefaultPageSizes = 20;

        public const String DateFormatDisplay = "MM/dd/yyyy";
        //public const String DateFormatDisplayTimeStamp = "MM/dd/yyyy hh:mm";
        public const String DateFormatDisplayTimeStamp = @"MM/dd/yyyy HH\:mm\:ss";
        public const String DateFormatOracle = "yyyyMMdd";

        public const Int32 GlobalOrderVersion = 1;
        public const Int32 GlobalOrderContant = 20000000;
        public const String GlobalWildCard = "*";
        public const String GlobalSeperator = ",";

        public const String PlantAggregationSuffix = " (A) ";


        public const Int32 DefaultSuggWO = 32;

        public const String Delimeter = "~";

        public const String DemandType = "DD";
        public const String AODemandType = "FC";
        public const String AOActivityCode = "PUL";
        public const String DemandDriver = "ISS";
        public const String ExpeditePriority = "50";
        public const String CombinedFabInd = "Y";
        public const String PipeLineCategoryCD = "SEW";
        public const String DiscreteInd = "Y";


        public const String SummaryDetailedByrev = "Detail by Revision";
 
        public const String NonAOAttribute = "------";

        public const String Yes = "Y";
        public const String No = "N";
        public const Int32 LevelInd = 0;

        public const int Priority = 999999999;
        public const String EnableAuthorization = "EnableAuthorization";

        public const String KADemandDriver = "KERP";
         
        public class   ISSOrderTypeCode
        {

            public const String SummarizedRequisition = "RQ";
            public const String NonSummarizedRequisition = "SR";

        }


        /// <summary>
        /// Division Unit for Dozen
        /// </summary>
        public const Decimal Dozen = 12.0m;
      
        public class MATL_TYPE_CD
        {
            public const String Garment = "00";
            public const String Code1 = "01";
            public const String Code2 = "02";
            public const String Fabric = "F";
            public const String CUT = "CT";
        }

        public class ProductionStatus
        {
            public const String Released = "R";
            public const String Locked = "L";
            public const String Suggested = "S";
            public const String TextileSuggested = "P";
            public const string Allocated = "AL";
        }
       

        public class RequestStatus
        {
            public const String UnderConstruction = "UC";
        }

        public class MakeOrBuy
        {
            public const String Make = "M";
            public const String Buy = "B"; 

        }


        public class BulkOrderStatus
        {
            public const String Error = "E";
            public const String Pending = "N";
            public const String Processed = "Y";
            public const String Awaiting = "A";
            public const String Completed = "C";
            public const String Initiate = "I";
        }
        public class BOMLevels
        {
            public const String Level1 = "LV1";
            public const String Level2 = "LV2";
        }
        public class AVYXException
        {
            public const String DEL = "DEL";
            public const String APS = "APS";
            public const String TXT = "TXT";

        }

        public class StyleType
        {
            public const String SellingStyle = "Selling Style";
            public const String MfgStyle = "Mfg Style";
        }
        public class WOMDueDates
        {
            public const String DC = "DC";
            public const String Sew = "Sew";
            public const String Cut = "Cut";
            public const String BD = "B/D";
            public const String Atr = "Atr";
            public const String EarliestStart = "Earliest Start";

            /// <summary>
            /// For Mass Change
            /// </summary>
            public const String Start = "Start";


        }

         public class WOMWeeks
        {
            public const String CurrentPriorWeek = "Current + Prior Week";
            public const String PlanWeekOne = "Plan Week One";
            public const String PlanWeekTwo = "Plan Week Two";
            public const String PlanWeekThree = "Plan Week Three";


         }
       
 
        public class UOM
        {
            public const String DZ = "DZ";
            public const String CT = "CT";
            public const String LB = "LB";
            public const String EA = "EA";
        }
        

        public class PipeLineCategoryCode
        {
            public const String  PKG="PKG";
             public const String  SEW="SEW";
             public const String  CUT="CUT";
             public const String  DBF="DBF";
        }
        public class BOMStyle
        {
            public const String PKG = "PKG";
            public const String MFG = "MFG";
            public const String SEL = "SEL";

        }

        public class SpillOver
        {
            public const String Yes = "Y";
            public const String No = "N"; 

        }

        public class WorkOrderType
        {
            public const String WO = "WO";
            public const String AttributedWO = "AO";
        }

        public class Width
        {
            public const Int32 Style = 80;
            public const Int32 Color = 80;
            public const Int32 Attribute = 80;
            public const Int32 Size = 80;
            public const Int32 Small1 = 50;
            public const Int32 Small2 = 60;
            public const Int32 Small2B = 70;

            public const Int32 Small3 = 80;
            public const Int32 Small3B = 90;




            public const Int32 Medium1 = 100;
            public const Int32 Medium2 = 110;
            public const Int32 Medium3 = 120;
            public const Int32 Medium4 = 140;

            public const Int32 Large1 = 150;
            public const Int32 Large2 = 180;
            public const Int32 Large2B = 200;

            public const Int32 Large3 = 220;
            public const Int32 Large4 = 250; 
                       

        }
        public class AllocationType
        {
            public const string MAC = "MAC";
            public const string CYL = "CYL";
            public const string DYE = "DYE";
            public const string YARN = "YARN";
            public const string CYLMAC = "CYL/MAC";
            public const string FAB = "FAB";
            public const string PRT = "PRT";
        }

		public class GridNames
        {
            public const string HEAD_SIZE = "headsize";
            public const string DYE_BLEACH = "dye";
            public const string DYE_BLEACH_EXP = "dyealloc";
            public const string MACHINE = "machine";
            public const string YARN = "yarn";
            public const string YARN_ITEM = "yarnitem";
            public const string FABRIC_ITEM = "fabricitem";
            public const string PRINT = "print";
        }

        public class ISSGroupType
        {
            public const String CutMaster = "M";
            public const String FinishMaster = "T";

        }
        
        public class DyeShadeCode
        {
            public const String CutPath = "C";
            public const String TxtPath = "T";

        }
        public class PipelineActivity
        {
            public const String DBF = "DBF";
            public const String DC = "DC";

        }
        public class ProductionCategories
        {
             public const String ATR = "ATR";
             public const String PKG = "PKG";
             public const String SEW = "SEW";
             public const String CUT = "CUT";
             public const String DBF = "DBF";
             public const String KNT = "KNT";
        }

          public class ProductionActivityCodes
          {
             public const String ATR = "ATR";
             public const String PKG = "PKG";
             public const String SEW = "SEW";
             public const String CUT = "CUT";
             public const String DBF = "DBF";
             public const String KNT = "KNT";
          }
          public class EditMode
          {
              public const int SaveMode = 1;
              public const int UpdateMode = 2;
              public const int DeleteMode = 3;
          }
        public class WOFilterGrid
        {
            public const string DEPENDED_DEMAND = "DD";
            public const string SUGGESTED = "S";
            public const string RELEASED = "R";
            public const string LOCKED = "L";
            public const string RELEASED_THIS_WEEK = "RW";
            public const string SPILL_OVER = "SY";
            public const string GROUP_ONLY = "G";
            public const string EXCLUDE_BUY_ORDER = "B";
        }
		 public class MaterialTypeCode
          {

              public const String Fabrics = "F";
              public const String CutPart = "CT";

          }

         public class PABTypes
         {

             public const String OnHand = "On Hand";
             public const String InTransit = "In-Transit";
             public const String WIP = "Supply / WIP";
             public const String Released = "Released";
             public const String Locked = "Locked";
             public const String Suggested = "Suggested";
             public const String PAB = "PAB";

         }

         public class AttributionInd
         {
             public const String AWOM = "Y";
             public const String WOM = "N";

         }
   
    }
}
