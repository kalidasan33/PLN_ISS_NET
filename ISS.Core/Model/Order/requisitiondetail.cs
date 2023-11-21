using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISS.Common;
using System.ComponentModel.DataAnnotations;
using ISS.Core.Model.Common;
namespace ISS.Core.Model.Order
{
    public class RequisitionDetail :ModelBase
    {

     
        public string Id { get; set; }

        [Display(Name = "Requisition Id")]
        [Required]
        public string RequisitionId { get; set; }

        [Required]
        [Display(Name = "Style")]      
        public string Style { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

      
        [Display(Name = "Color")]
        public string Color { get; set; }

       
        [Display(Name = "Attribute")]        
        public string Attribute { get; set; }

     
        [Display(Name = "Size CD")]     
        public string Size { get; set; }

        [Display(Name = "Size Desc")] 
        public string SizeLit { get; set; }

        [Display(Name = "Rev")]
        public decimal Rev { get; set; }

     
        [Display(Name = "UOM")]
        public string Uom { get; set; }

        [Display(Name = "Qty")]
        public decimal Qty { get; set; }

        [Display(Name = "Std Case Qty")]
        public decimal StdCase { get; set; }

        [Display(Name = "DPR Rule")]
        public decimal Dpr { get; set; }

        public string LineNumber { get; set; }

        [Display(Name = "Plan Date")]
        [DataType(DataType.Date)] // making data type as date     
        public DateTime? PlanDate { get; set; }

        /// <summary>
        /// CURR_DUE_DATE
        /// </summary>       
        public DateTime? CurrDueDate { get; set; }

        [Display(Name = "Error Status")]
        public bool ErrorStatus { get; set; }

        [Display(Name = "Error")]
        public string ErrorMessage { get; set; }

        [Display(Name = "Business Unit")]
        public string BusinessUnit{ get; set; }

	 

        public String SuperOrder { get; set; }
        public String OrderLabel { get; set; }


        public bool IsMovedObject { get; set; }

        public bool IsInserted { get; set; }

        /// <summary>
        /// Kendo grid property
        /// </summary>
        public bool IsDirty { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsSummarized { get; set; }

        public decimal SummarizedQty { get; set; }

        public bool isHide { get; set; }






        public decimal RequisitionVer { get; set; }

        public decimal OrderVersion { get; set; }

        public DateTime? ScheduledShipDate { get; set; }
        public DateTime? OriginalDueDate { get; set; }

        public decimal PlannedLeadTime { get; set; }
        public decimal TransportationTime { get; set; }
        
        public string MakeOrBuyCode { get; set; }

        public string GarmentStyle { get; set; }
        public string SellingStyle { get; set; }

        [Display(Name = "DPR Priority Sequence")]
        public decimal Priority { get; set; }

        public String getSKUString(bool IncludeRevision = false)
        {
            return Style + LOVConstants.Delimeter + Color + LOVConstants.Delimeter + Attribute + LOVConstants.Delimeter + Size + ((IncludeRevision) ? (LOVConstants.Delimeter + Rev.ToString()) : String.Empty);
        }


        public string DCLoc { get; set; }

        public string DemandSource { get; set; }
        public string DemandType { get; set; }
        public string DemandDriver { get; set; }


        public string OrderType { get; set; }
        public String SewPath { get; set; }


        public string ExpeditePriority { get; set; }
        public string CombinedFabInd { get; set; }
        public string CombinedInd { get; set; }

        public decimal DemandQty { get; set; }
        public decimal OriginalOrderQty { get; set; }
        public decimal TotatalCurrOrderQty { get; set; }



        public string TBD { get; set; }


        //  Fill by additional details


        public string MfgPathId { get; set; }
        public string MfgLoc { get; set; }
        public string RoutinId { get; set; }
        public string BillOfMATL { get; set; }
        public decimal ScrapFactor { get; set; }
        public string Planner { get; set; }
        public string MakeOrBuy { get; set; }
        
        public decimal PlanningLeadTime { get; set; }
 
        public string CapacityAlloc { get; set; }
        public string FabricGroup { get; set; }
        public string ResourceId { get; set; }
        public string ProdFamilyCd { get; set; }
        public string MatlCd { get; set; }
        public string AsrmtCd { get; set; }
        public string PipeLineCategoryCD { get; set; }
        
        public decimal Usage { get; set; }
        public decimal StdUsage { get; set; }
        public decimal StdLoss { get; set; }
        public decimal WasteFactor { get; set; }
        public decimal CylinderSize { get; set; }
        public decimal FinishedWidth { get; set; }
        public decimal ConditionedWidth { get; set; }


        public string CorpDivision { get; set; }
        public string SizeLine { get; set; }
        public string MFGCorpDivision { get; set; }
        [Display(Name = "Std Case Qty")]
        public string StdQuality { get { return this.StdCase.ToString("0.00").Replace('.', '-'); } }
        public string ColorDivisionCD { get; set; }
        public string OriginTypeCD { get; set; }
        public string GTINLabelCD { get; set; }
        public string InvCheckInd { get; set; }
        public string PackCD { get; set; }
        public string ProdClassCD { get; set; }
        public string PrimaryDC { get; set; }
        public decimal PackageQty { get; set; }
        public string DescreteInd { get; set; }
        public string CreateBDInd { get; set; }
        public string DozensOnlyInd { get; set; }

        
        public string PullFromStockInd { get; set; }

        public string CuttingAlt { get; set; }
        public string DyeCD { get; set; }
        public string CategoryCD { get; set; }
        public decimal MfgRevisionNO { get; set; }


        public string ParentOrder { get; set; }

        public string Quantity { get { return this.Qty.ToString("0.00").Replace('.','-'); } }

        public string DiscreteInd { get; set; }
        public string BomSpecId { get; set; }
        public string DyeShadeCode { get; set; }
        public string MachineTypeCode { get; set; }
        public string CutMethod { get; set; }
        public string SpreadCompCode { get; set; }
        public string SpreadTypeCode { get; set; }
        public decimal MachineCut { get; set; }
        public string AttributionInd { get; set; }
        public string OrderReference { get; set; }
    }
}