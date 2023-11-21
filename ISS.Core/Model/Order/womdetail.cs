using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISS.Common;
using System.ComponentModel.DataAnnotations;
using ISS.Core.Model.Common;
using System.ComponentModel;

namespace ISS.Core.Model.Order
{
    public class WOMDetail : ModelBase
    {


        public bool IsDeleted { set; get; }
        public bool IsEdited { set; get; }
        public bool IsPFSChange { set; get; }
        public bool IsFieldChange { set; get; }

        public bool IsGrouped { set; get; }
        public bool IsUngrouped { set; get; }
        public bool IsHide { set; get; }
        public bool IsMerged { set; get; }
        public bool IsSKUChange { set; get; }

        public WOMDetail Cloned { set; get; }

        public List<WOMDetail> PFSList { set; get; }

        /// <summary>
        /// Used to check wether all the super orders loaded in the group
        /// </summary>
        public bool GroupLoadChecked { set; get; }


        public bool HasGroupIdChange { set; get; }


        [Display(Name = "Group Id")]
        public String GroupId { get; set; }

        [Display(Name = "Error")]
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

        public decimal OrderVersion { get; set; }

        [Display(Name = "AVYX Order # / Super Order")]
        public String SuperOrder { get; set; }

        public String ParentOrder { get; set; }
        public String OrderLabel { get; set; }

        public string FilterGridCriteria { get; set; }

        public String WorkCenter { set; get; }

        public string PipelineCategoryCode { get; set; }
        public string MatlTypeCode { get; set; }

        public string BillOfMtrlsId { get; set; }
        public decimal Usage { get; set; }
        public decimal StdUsuage { get; set; }
        public decimal StdLoss { get; set; }
        public decimal WasteFactor { get; set; }
        public string CuttingAlt { get; set; }
        public decimal FinishedWidth { get; set; }
        public decimal ConditionedWidth { get; set; }
        public decimal MachineCut { get; set; }


        [Display(Name = "ID")]
        public int Id { get; set; }

        [Display(Name = "Lot #")]
        public string OrderId { get; set; }

        [Display(Name = "Order Type")]
        public string OrderType { get; set; }

        [Display(Name = "Order Status")]
        public string OrderStatus { get; set; }

        [Display(Name = "Order Status")]
        public string OrderStatusDesc
        {
            get
            {
                if (!String.IsNullOrEmpty(OrderStatus))
                {
                    if (OrderStatus == ProductionStatus.Locked.GetDescription())
                    {
                        return ProductionStatus.Locked.ToString();
                    }
                    else if (OrderStatus == ProductionStatus.Released.GetDescription())
                    {
                        return ProductionStatus.Released.ToString();
                    }
                    else if (OrderStatus == ProductionStatus.Suggested.GetDescription())
                    {
                        return ProductionStatus.Suggested.ToString();
                    }
                }
                return OrderStatus;
            }
        }


        //[Required(ErrorMessage = "Selling Style is Required.")]
        // [MaxLength(6)]
        [StringLength(6)]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Invalid characters")]
        [Display(Name = "Sell Style")]
        [Required(ErrorMessage = "Style is Required")]
        public string SellingStyle { get; set; }

        [Display(Name = "Selling Color")]
        public string SellingColor { get; set; }

        public string StyleDes { get; set; }

        [Display(Name = "Selling Attr")]
        public string SellingAttribute { get; set; }

        public string SellingSize { get; set; }

        [Required(ErrorMessage = "Style is Required")]
        [Display(Name = "Style")]
        public string Style { get; set; }

        [Display(Name = "Garment SKU")]
        public string GarmentSKU { get; set; }
        public string GarmentStyle { get; set; }
        public string GarmentColor { get; set; }

        [Required(ErrorMessage = "Color is Required")]
        [Display(Name = "Color")]
        public string Color { get; set; }

        [Required(ErrorMessage = "Attr is Required")]
        [Display(Name = "Attr")]
        public string Attribute { get; set; }

        [Required(ErrorMessage = "Size is Required")]
        public String Size { get; set; }

        [Display(Name = "Size Desc")]
        public string SizeShortDes { get; set; }

        [Required(ErrorMessage = "Revision is Required.")]
        [Display(Name = "Rev #")]
        public decimal Revision { get; set; }

        /// <summary>
        /// Cutting Alt
        /// </summary>
        [Display(Name = "Alt Id")]
        public string AltId { get; set; }

        [Display(Name = "Alt Id")]
        public string AltIdd
        {
            get
            {
                return AltId;
            }
            set
            {
                AltId = value;
            }
        }

        /// <summary>
        /// CURR_DUE_DATE
        /// </summary>       
        [Display(Name = "DC Due Date")]
        public DateTime? CurrDueDate { get; set; }

        public DateTime? CCurrDueDate { get; set; }
        

        [Display(Name = "DC Due Date")]
        public String CurrDueDateStr
        {
            get
            {
                return (CurrDueDate.HasValue) ? CurrDueDate.Value.ToString(LOVConstants.DateFormatDisplay) : string.Empty;
            }
        }



        [Display(Name = "Qty")]
        public decimal Qty { get; set; }

        [Display(Name = "Total Qty")]
        public decimal TotatalCurrOrderQty { get; set; }


//09/30/2019
        [Display(Name = "FQ Dz")]
        public decimal QtyDZ
       // public int QtyDZ
        {
            get
            {
                return Math.Ceiling(Qty / LOVConstants.Dozen);
                //return (int)(Qty / LOVConstants.Dozen).RoundCustom(0);
            }
            set
            {
                Qty = value * LOVConstants.Dozen;
            }
        }

        [Display(Name = "FQ")]
        public String QtyEach { get; set; }

        //[Display(Name = "FQ")]
        //public string QtyEachStr
        //{
        //    get
        //    {
        //        return QtyEach.ConvertDzToEaches().ToString();
        //    }
        //    set
        //    {
        //        QtyEach = Convert.ToDecimal(value).ConvertEachesToDz();
        //    }
        //}

        [Required(ErrorMessage = "TQ Dz is required")]
        [Display(Name = "TQ Dz")]
        [RegularExpression("^[1-9][0-9]*$", ErrorMessage = "TQ Dz must be greater than zero.")]
        public decimal TotalDozens
        {
            get
            {
                return Math.Ceiling((TotatalCurrOrderQty / LOVConstants.Dozen));
            }
            set
            {
                TotatalCurrOrderQty = value * LOVConstants.Dozen;
            }
        }

        //[Display(Name = "Dz")]
        //public decimal Dozens { get; set; }

        //[Display(Name = "Machine")]
        //public string MachinTypeCode { get; set; }


        [Display(Name = "Lbs")]
        public decimal? Lbs { get; set; }

        [Display(Name = "Lbs")]
        public decimal LbsStr
        {
            get
            {
                return Greigelbs.RoundCustom(2);
            }
        }


        [Display(Name = "Greige lbs")]
        public decimal Greigelbs { get; set; }

        [Display(Name = "M/C")]
        public string MC { get; set; }

        [Display(Name = "Cylinder")]
        public string CylinderSizes { get; set; }

        [Display(Name = "PFS Ind")]
        public bool PFSInd { get; set; }

        [Display(Name = "PFS")]
        public decimal? PFS
        {
            get
            {
                return PFSInd ? 1 : 0;
            }
            set
            {
                PFSInd = (value > 0);
            }
        }

        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        public DateTime? CStartDate { get; set; }

        [Display(Name = "Earliest Start Date")]
        public DateTime? EarliestStartDate { get; set; }

        //[Display(Name = "DC Due Date")]
        //public DateTime? DCDueDate { get; set; }

        //  [Display(Name = "DC Due Date")]
        //public String DCDueDateStr { get {
        //    return (DCDueDate.HasValue) ? DCDueDate.Value.ToString(LOVConstants.DateFormatDisplay) : string.Empty;
        //} }

        [Display(Name = "Txt")]
        public string TxtPath { get; set; }

        [Display(Name = "Cut")]
        public string CutPath { get; set; }

        [Display(Name = "Cut")]
        public string CuttPath
        {
            get
            {
                return CutPath;
            }
            set
            {
                CutPath = value;
            }
        }


        [Display(Name = "Path Id")]
        public String MfgPathId { set; get; }

        [Display(Name = "Path Id")]
        public string MfgPath
        {
            get
            {
                return MfgPathId;
            }
            set
            {
                MfgPathId = value;
            }
        }

        [Display(Name = "Atr")]
        public string Atr { get; set; } //duplicate


        [Display(Name = "DC")]
        [StringLength(3, ErrorMessage = "Invalid DC")]
        public string DcLoc { get; set; }

        [Display(Name = "Sequence #")]
        public decimal? Priority { get; set; }

        [Display(Name = "Priority Cd")]
        public decimal? ExpeditePriority { get; set; }

        [StringLength(6)]
        [Display(Name = "Cat Cd")]
        public string CategoryCode { get; set; }


        [StringLength(6)]
        [Display(Name = "Cat Cd")]
        public string CatCode
        {
            get
            {
                return CategoryCode ?? "";
            }
            set
            {
                CategoryCode = value;
            }
        }

        [Display(Name = "Category Description")]
        public string CategoryDescription { get; set; }

        [Display(Name = "Dmd Source")]
        public string DemandSource { get; set; }

        public string DemandType { get; set; }

        [Display(Name = "Suggested Date")]
        public DateTime? DemandDate { get; set; }

        [Display(Name = "Dmd Driver")]
        public string DemandDriver { get; set; }

        [Display(Name = "Dye/Ble")]
        public String DyeCode { set; get; }

        public string DyeShadeCode { get; set; }

        [Display(Name = "Rule #")]
        public Decimal? Rule { get; set; }



        [Display(Name = "Spill Over")]
        public string Enforcement { get; set; }

        [Display(Name = "Create B/D")]
        public bool CreateBd { get; set; }

        [Display(Name = "DZ only")]
        public bool DozensOnly { get; set; }
        //public bool DozensOnly {
        //    get
        //    {
        //        if (CreateBd == true)
        //        {
        //            return false;
        //        }
        //        return true;
        //    }
        //    set
        //    {
        //        if (value == true)
        //        {
        //            CreateBd = false;
        //        }
        //        else
        //        {
        //            CreateBd = true;
        //        }
        //    }
        //}

        public string CreateBDInd
        {
            get
            {

                return ((CreateBd) ? LOVConstants.Yes : LOVConstants.No);

            }
            set
            {
                CreateBd = (value != null && value == LOVConstants.Yes);
            }
        }

        public string DozensOnlyInd
        {
            get
            {
                return ((DozensOnly) ? LOVConstants.Yes : LOVConstants.No);
            }
            set
            {
                DozensOnly = (value != null && value == LOVConstants.Yes);
            }
        }



        [Display(Name = "Off")]
        public bool DozensOnlyOff { get; set; }

        [Display(Name = "BOM Update")]
        public bool BOMUpdate { set; get; }


        [StringLength(6)]
        [Display(Name = "Pack Cd")]
        public string PackCode { get; set; }

        [Display(Name = "Pack Description")]
        public string PackDescription { get; set; }

        [Display(Name = "M or B")]
        public string MakeOrBuy { get; set; }

        [Display(Name = "Note")]
        public string Note { get; set; }

        public string NoteInd { get; set; }




        // ==============================================================================



        public string PrimeMFGLoc { get; set; }

        [Display(Name = "Sew Pit")]
        public string SewPit { get; set; }

        [Display(Name = "Attrib Path")]
        public string AttributionPath { get; set; }


        [Display(Name = "Body Trim")]
        public string BodyTrim { get; set; }

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

        public string UOM { get; set; }

        public string ProdFamCode { get; set; }

        public string ProdClassCode { get; set; }

        public string PrimaryDC { get; set; }
        public decimal PKGQty { get; set; }


        public string RoutingId { get; set; }
        public string BoMId { get; set; }
        public decimal ScrapFactor { get; set; }

        [Display(Name = "Sew")]
        public string SewPath { get; set; }
        [Display(Name = "By")]
        public string CreatedBy { get; set; }

        [Display(Name = "By")]
        public string UpdatedBy { get; set; }
        public string MorBCd { get; set; }
        public decimal LeadTime { get; set; }
        public string CapacityAlloc { get; set; }
        public string SupplyPlant { get; set; }
        public string SpreadTypeCode { get; set; }
        public string SpreadCompCode { get; set; }

        public string RemoteUpdateCode { get; set; }

        public DateTime? RemoteUpdateDate { get; set; }

        public string DescreteInd { get; set; }

        public string MFGPlant { get; set; }


        /// <summary>
        /// A brief description of revision
        /// </summary>
        [Display(Name = "Revision Desc")]
        public string RevDescription { get; set; }

        [Display(Name = "Revision")]
        public Decimal NewRevision { get; set; }

        public string LocationCode { get; set; }

        [Display(Name = "Default View")]
        public string Views { set; get; }

        /// <summary>
        /// For filling cut path txt path procedure
        /// </summary>
        public string Source_Plant { get; set; }

        [Display(Name = "Order Ref")]
        public string OrderRef { get; set; }

        public string AttributionInd { get; set; }


        public String SKU
        {
            get
            {
                return getSKUString();
            }
        }


        public String getSKUString(bool includeSize = false, bool IncludeRevision = false)
        {
            return Style + LOVConstants.Delimeter + Color + LOVConstants.Delimeter + Attribute + ((includeSize) ? (LOVConstants.Delimeter + Size) : String.Empty) + ((IncludeRevision) ? (LOVConstants.Delimeter + Revision.ToString()) : String.Empty);
        }

        public String SellingSKU
        {
            get
            {
                return getSellingSKUString();
            }
        }

        public String getSellingSKUString()
        {
            return SellingStyle + LOVConstants.Delimeter + SellingColor + LOVConstants.Delimeter + SellingAttribute;
        }

    }
}
