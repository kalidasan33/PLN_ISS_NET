using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace ISS.Core.Model.Order
{
   public class WorkOrderCumulative
    {
       [Display(Name = "Id")]
       public int CumulativeId { get; set; }
       
       //Cumulative Totals Grid fields
        [Display(Name = "Style Cd")]
        public string StyleCode { get; set; }

        [Display(Name = "Color/Dye Cd")]
        public string ColorDyeCode { get; set; }

        [Display(Name = "Attr/Comp Cd")]
        public string AttributeCompCode { get; set; }

        [Display(Name = "Size")]
        public string Size { get; set; }

        [Display(Name = "Rev")]
        public decimal Revision { get; set; }

        [Display(Name = "Dz")]
        public decimal Dozens { get; set; }

        [Display(Name = "Total Dz")]
        public decimal TotalDozens { get; set; }

        [Display(Name = "Lbs")]
        public decimal Lbs { get; set; }

        public string ParentStyle { get; set; }
        public string ParentColor { get; set; }
        public string ParentAttribute { get; set; }
        public string ParentSize { get; set; }
        public string ParentSizeDes { get; set; }
        public string ParentBoMId { get; set; }
        public string ParentMFGPathId { get; set; }
        public string MatTypeCode { get; set; }
        public bool IsHide { get; set; }
        public int LevelInd { get; set; }
        public string HiddenSizeDes { get; set; }
        public bool IsProcessed { get; set; }
        public bool Merged { get; set;}

       //Calculation and Save objects

        public string OrderLabel { get; set; }
        public decimal OrderVersion { get; set; }
        public string SuperOrder { get; set; }
        public string ParentOrder { get; set; }
        public string OrderId { get; set; }
        public string StyleCd { get; set; }
        public string StyleDesc { get; set; }
        public string ColorCode { get; set; }
        public string AttributeCode { get; set; }
        public string SizeCode { get; set; }
        public string SizeLit { get; set; }
        //public string Revision { get; set; }
        public string DemandLoc { get; set; }

        public string ProdFamilyCode { get; set; }
        public string FabricGroup { get; set; }
        public string MakeOrBuyCode { get; set; }
        public string MFGPlant { get; set; }
        public string PipelineCategoryCode { get; set; }
        public decimal DemandQty { get; set; }
        public DateTime CurrentDueDate { get; set; }
        public decimal CurrentOrderQty { get; set; }
        public decimal CurrentOrderTotalQty { get; set; }
        public string MatlTypeCode { get; set; }
        public string RoutingId { get; set; }
        public string BillOfMtrlsId { get; set; }
        public string MFGPathId { get; set; }

        public string BomSpecId { get; set; }
        public string MFGStyleCode { get; set; }
        public string MFGColorCode { get; set; }
        public string MFGAttributeCode { get; set; }
        public string MFGSizeCode { get; set; }
        public string MasterOrderNo { get; set; }
        public string CuttingAlt { get; set; }
        public decimal Usuage { get; set; }
        public decimal StdUsuage { get; set; }
        public decimal StdLoss { get; set; }
        public decimal WasteFactor { get; set; }
        public string FabricType { get; set; }
        public string DyeCode { get; set; }


        public string DyeShadeCode { get; set; }
        public string MachineTypeCode { get; set; }
        public string CutMethod { get; set; }
        public string PullFromStockIndicator { get; set; }
        public decimal CylinderSize { get; set; }
        public decimal FinishedWidth { get; set; }
        public decimal ConditionedWidth { get; set; }
        public string SpreadCompCode { get; set; }
        public string SpreadTypeCode { get; set; }
        public string VendorNo { get; set; }
        public string AllocationInd { get; set; }
        public string OffLoadInd { get; set; }
        public decimal ScrapFactor { get; set; }


        public string PackCode { get; set; }
        public decimal PackQty { get; set; }
        public string CategoryCode { get; set; }
        public string UnitOfMeasure { get; set; }
        public string ResourceId { get; set; }
        public string ActivityId { get; set; }
        public DateTime? DueDate { get; set; }
        public string DueDateDc { get; set; }
        public decimal BackSchedule { get; set; }
        public decimal PlanningLeadTime { get; set; }
        public string CombineInd { get; set; }
        public decimal CombineSeq { get; set; }
        public string Required { get; set; }
        public DateTime PlanDate { get; set; }

        public decimal MFGRevisionNo { get; set; }
        public decimal MachineCut { get; set; }
        public DateTime SchedShipDate { get; set; }
        public decimal StdCaseQty { get; set; }
        public string AsrmtCode { get; set; }
        public string CapacityAlloc { get; set; }
        public string SellingStyleCode { get; set; }
        public string SellingColorCode { get; set; }
        public string SellingAttributeCode { get; set; }
        public string SellingSizeCode { get; set; }

        public decimal ExpeditePriority { get; set; }
        public string GrmtStyleCode { get; set; }
        public string GrmtColorCode { get; set; }
        public string CombineFabInd { get; set; }
        public decimal CombineFabSeq { get; set; }
        public DateTime SewStartDate { get; set; }
        public string BusinessUnit { get; set; }

        public decimal RuleNo { get; set; }
        public int EditMode { get; set; }


        public decimal OrgTotalDozens { get; set; }
        public decimal OrgLbs { get; set; }

        public string CutPath { get; set; }
        public string TxtPath { get; set; }
        public string PlannerCd { get; set; }

        public int SeqId { get; set; }
        public int ParentId { get; set; }

    }
}
