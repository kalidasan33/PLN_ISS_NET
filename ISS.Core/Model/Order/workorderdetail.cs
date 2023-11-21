using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using ISS.Core.Model.Common;
using ISS.Common;
namespace ISS.Core.Model.Order
{
    public class WorkOrderDetail : ModelBase
    {
        public WorkOrderDetail()
        {
            ErrorStatus = false;
        }

        //Top Grid
        [Display(Name = "ID")]
       
        public int Id { get; set; }

        [Display(Name = "Create BD")]
        public bool CreateBd { get; set; }
        public string CreateBDInd { get; set; }

        [Display(Name = "DZ only")]
        public bool DozensOnly
        {
            get
            {
                if (CreateBd == true)
                {
                    return false;
                }
                return true;
            }
            set
            {
                if (value == true)
                {
                    CreateBd = false;
                }
                else
                {
                    CreateBd = true;
                }
            }
        }
        public string DozensOnlyInd { get; set; }

        [Required(ErrorMessage = "Selling Style is Required.")]
       // [MaxLength(6)]
        [StringLength(6)]
        [RegularExpression("^[a-zA-Z0-9]*$",ErrorMessage="Invalid characters")]
        [Display(Name = "Sell Style")]
        public string SellingStyle { get; set; }

        [Display(Name = "Style Description")]
        public string SellStyleDesc { get; set; }

        public string StyleDes { get; set; }

        [Required(ErrorMessage = "Color Code is Required.")]
        [MaxLength(4)]//PFE
      //  [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Invalid characters")]
        [Display(Name = "[Color]")]
        public string ColorCode { get; set; }

        [Required(ErrorMessage = "Attribute is Required.")]
        [MaxLength(6)]
        [Display(Name = "Attr")]
        public string Attribute { get; set; }

        public string AttributeDesc { get; set; }
        [Required(ErrorMessage = "Size is Required.")]
        
        public string Size { get; set; }

        public string SizeQty { get; set; }

        [Display(Name = "Size Desc")]
        public string SizeCde { get; set; }

        [Display(Name = "[Size]")]
        public string SizeShortDes { get; set; }

        [Required(ErrorMessage = "Revision is Required.")]
        [Display(Name = "[Rev]")]
        public decimal Revision { get; set; }

        public decimal MaxRevision { get; set; }
         [Required(ErrorMessage = "PKG is Required")]
         [StringLength(6)]
        [Display(Name = "PKG Style")]
        public string PKGStyle { get; set; }



        [Required(ErrorMessage = "MFG Path Id is Required.")]
        [MaxLength(3)]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Invalid characters")]
        [Display(Name = "[MFG Path Id]")]
        public string MfgPathId { get; set; }
        public string PrimeMFGLoc{get;set;}
        public decimal Priority{get;set;}
        

        //below items DT needs to be identified

        [Display(Name = "[Alt Id]")]
        public string AlternateId { get; set; }

        [Display(Name = "[Cut Path]")]
        public string CutPath { get; set; }

        [Display(Name = "Sew Plt")]
        public string SewPlt { get; set; }

        public string SewPltMfg { get; set; }

        [Display(Name = "Attrib Path")]
        public string AttributionPath { get; set; }

        [Required(ErrorMessage = "Total Dozens is required")]
        
        [Display(Name = " Sum Total Dz")]
        public decimal TotalDozens { get; set; }

        [Display(Name = "Sum Dz")]
        public decimal Dozens { get; set; }

        [Display(Name = "Sum Qty")]
        [Required(ErrorMessage = "Quantity is Required.")]
        public string DozenStr { get; set; }

        [Display(Name = "Sum Lbs")]
        public decimal Lbs { get; set; }
       public decimal ActualLbs { get; set; }
        [Display(Name = "[Pack Cd]")]
        public string PackCode { get; set; }

        [Display(Name="Pack Description")]
        public string PackDescription { get; set; }

        [Display(Name = "[Cat Cd]")]
        public string CategoryCode { get; set; }

        [Display(Name = "Category Description")]
        public string CategoryDescription { get; set; }

        [Display(Name = "Priority Cd")]
        public decimal PriorityCode { get; set; }

        [Display(Name = "Body Trim")]
        [MaxLength(1)]
        [RegularExpression("^[sStT]*$", ErrorMessage = "Body Trim can be either S,T or Empty")]
        public string BodyTrim { get; set; }

        [Display(Name = "Cylinder Sizes")]
        public string CylinderSizes { get; set; }

        [Display(Name = "Note")]
        public string Note { get; set; }

        public string AssortCode { get; set; }

        public string CorpDivisionCode { get; set; }
        public string MatTypeCode { get; set; }

        public string CorpBusUnit { get; set; }

        public string SizeLine { get; set; }

        public string CorpDivCode { get; set; }

        public string QualityCode { get; set; }

        public string ColDivCode { get; set; }

        public string OriginTypeCode { get; set; }

        public string GtinLabcode { get; set; }

        public string InvChkInd { get; set; }

       // public string PackCode { get; set; }

        public string UoM { get; set; }

        public string DemandLoc { get; set; }

        public string ProdFamCode { get; set; }

        public string ProdClassCode { get; set; }

        public string PrimaryDC { get; set; }
        public decimal PKGQty { get; set; }
        
        public string NewStyle { get; set; }
        public string NewColor { get; set; }
        public string NewAttribute { get; set; }
        public string NewSize { get; set; }
        public string NewSizeDes { get; set; }
        public string RoutingId { get; set; }
        public string BoMId { get; set; }
        public decimal ScrapFactor { get; set; }
        public string PlannerCd { get; set; }
        public decimal Usuage { get; set; }
        public decimal WasteFactor { get; set; }
        public decimal StdUsuage { get; set; }
        public decimal StdLoss { get; set; }
        public decimal CylSize { get; set; }
        public string MorBCd { get; set; }
        public decimal LeadTime { get; set; }
        public string CapacityAlloc { get; set; }
        public string SupplyPlant { get; set; }
        public string ParentStyle { get; set; }
        public string ParentColor { get; set; }
        public string ParentSize { get; set; }
        public string ParentBoM { get; set; }
        public string ParentUoM { get; set; }
        public string DiscreteInd { get; set; }
        public decimal CurrOrderTotQty { get; set; }
        public decimal DemandQty { get; set; }
        public string CompCode { get; set; }
        public string SpreadCode { get; set; }
        public string PullFrmStkInd { get; set; }
        public decimal FinishedWidth { get; set; }
        public string CutMethodCode { get; set; }
        public string CuttingAlt { get; set; }
        public string DyeShadeCode { get; set; }
        public decimal ConditionedWidth { get; set; }
        public int LevelInd { get; set; }
        public string PipeLineCat { get; set; }
        public bool PullFromStock { get; set; }
        public string ActivityCode { get; set; }
        public decimal LimitLbs { get; set; }
        public string SetCyl { get; set; }
        /// <summary>
        /// A brief description of revision
        /// </summary>
        [Display(Name = "Revision Desc")]
        public string RevDescription { get; set; }
        [Display(Name="Revision")]
        public Decimal NewRevision { get; set; }
        public string LocationCode { get; set; }
        public List<WorkOrderCumulative>WOCumulative { get; set; }
        public List<WorkOrderFabric> WOFabric { get; set; }
        public List<WorkOrderTextiles> WOTextiles { get; set; }
        public List<WorkOrderDetail> WODetail{ get; set; }
        [Display(Name = "Group ID")]
        public decimal GroupId { get; set; }
          [Display(Name = "Size")]
        public List<MultiSKUSizes> SizeList { get; set; }
        //Temp Objects used for calculation purpose only
        public string GridMode { get; set; }
        public string TempStyle { get; set; }
        public string TempColor { get; set; }
        public string TempSize { get; set; }
        public string TempBoMId { get; set; }
        public string TempAttribute { get; set; }
        public string TempSizeDes { get; set; }
        public string TempMFGPathId { get; set; }
        public string TempRoutingId { get; set; }
        public string TempResourceId { get; set; }
        public string TempFabricGroup { get; set; }
        public string TempAltId { get; set; }
        public string TempSizeQty { get; set; }
        public decimal TempDemandDisplay { get; set; }
        public decimal TempCurrentTotal { get; set; }
        public decimal TempTotal { get; set; }
        public decimal TempTotalDozens { get; set; }
        public decimal TempPkgQty { get; set; }
        public string HiddenSizeDes { get; set; }
        public string MachineType { get; set; }
        public decimal CurrOrderQty { get; set; }
        public bool IsHide { get; set; }
        public string InventoryCheckInd { get; set; }
        public string BomSpecificId { get; set; }
        public string DyeCode { get; set; }
        public string SpreadTypeCode { get; set; }
        public string CombineInd { get; set; }
        public decimal MachineCut { get; set; }
        public string CombineFabInd { get; set; }
        public int rIndex { get; set; }
        public int fIndex { get; set; }
		public string NoteInd { get; set; }
        public DateTime PlannedDate { get; set; }
        public string DueDate { get; set; }
		public decimal OrderCount { get; set; }
        //MachineType,TextilePlant,Limit & Variance
        public string iStyle { get; set; }
        public string iColor { get; set; }
        public string iSize { get; set; }
        public string iAttribute { get; set; }
        public string iMFGPathId { get; set; }
        public string iDesPlt { get; set; }
        public string iSourcePlant { get; set; }
        public string iMacTypeCode { get; set; }
        public decimal iPriority { get; set; }
        public string iParentStyle { get; set; }
        public string iParentBomId { get; set; }
        public string iCuttingAlt { get; set; }
        public decimal Variance { get; set; }
        public decimal VarianceQty { get; set; }
        public string iSpreadType { get; set; }
        public string MFGPlant { get; set; }
        public string TxtPath { get; set; }

        [Display(Name = "Error Status")]
        public bool ErrorStatus { get; set; }

        private String _ErrorMessage = null;

        [Display(Name = "Error")]
        public String ErrorMessage
        {
            get
            {
                return _ErrorMessage.HtmlEncode();
            }
            set
            {
                _ErrorMessage = value;
            }
        }

        public int SeqId { get; set; }
        public int ParentId { get; set; }

        //[Display(Name = "Garment SKU")]
        //public string GarmentSKU { get; set; }

        [Display(Name = "Garment SKU")]
        public string GarmentSKU { get { return GStyle + ' ' + GColor + ' ' + GAttribute + ' ' + GSize; } }

        public List<WorkOrderCumulative> ConsumedOrders { get; set; }

        /// <summary>
        /// For Garment SKU - Garment Style, Color, Attribute and Size
        /// </summary>
        public string GStyle { get; set; }
        public string GColor { get; set; }
        public string GAttribute { get; set; }
        public string GSize { get; set; }
        public DateTime CurrentDueDate { get; set; }

        public string SuperOrder { get; set; }
        public bool IsUngrouped { get; set; }
        //public string GrmntSKU { get; set; }

        [Display(Name = "Demand Source")]
        public string BulkNumber { get; set; }
        public string AttributionInd { get; set; }
        public string OrderReference { get; set; }

        public string SelectedDc { get; set; }

        //[StringLength(6)]
        //[Display(Name="Purchase Order")]
        //public string PurchaseOrder { get; set; }
        private string _PurchaseOrder;
        [StringLength(6)]
        [Display(Name = "Purchase Order")]
        public string PurchaseOrder
        {
            get { return _PurchaseOrder; }
            set
            {
                if (value != null)
                    value = value.ToUpper();
                _PurchaseOrder = value;
            }
        }


        [MaxLength(6)]
        [Display(Name = "Line Item")]
        public string LineItem { get; set; }

        [Display(Name = "Demand Driver")]
        public string DemandDriver { get; set; }

        [Display(Name = "Demand Source")]
        public string DemandSource { get; set; }

        public IList<DemandDrivers> DemandDriversData { get; set; }
    }
}
