using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISS.Core.Model.Order
{
    public class WOManagementSearch
    {
        public WOManagementSearch()
        {

        }

        public String Source { set; get; }

        [MaxLength(5)]
        public String Planner { set; get; }

        public String SuperOrder { set; get; }


        public String StyleType { set; get; }

        //[MaxLength(6)]
        [StringLength(6)]
        [Display(Name = "Style")]
        public String SStyle { set; get; }

        //[MaxLength(6)]
        //[Display(Name = "Mfg Style")]
        //public String MfgStyle { set; get; }

       // [MaxLength(4)]//changed from 3 to 4 as part of PFE 10/5/2018
        [StringLength(4)]
        [Display(Name = "Color")]
        public String SColor { set; get; }

       // [MaxLength(6)]
        [StringLength(6)]
        [Display(Name = "Attribute")]
        public String SAttribute { set; get; }

       

        [Display(Name = "Size")]
        public String SSize { set; get; }

        //[MaxLength(2)]
        [StringLength(2)]
        [Display(Name = "DC")]
        public String DC { set; get; }

        //[MaxLength(12)]
        [StringLength(12)]
        [Display(Name = "Rev")]
        public String Rev { set; get; }

        [Display(Name = "Capacity Group")]
        public String CapacityGroup { set; get; }

       // [MaxLength(15)]
        [StringLength(15)]
        [Display(Name = "Work Center")]       
        public String WorkCenter { set; get; }

        //[MaxLength(3)]
        [StringLength(3)]
        [Display(Name = "Sew Path")]
        public String MfgPathId { set; get; }

       // [MaxLength(12)]
        [StringLength(12)]
        [Display(Name = "Rule")]
        public String Rule { set; get; }

        //[MaxLength(12)]
        [StringLength(12)]
        [Display(Name = "Group Id")]
        public String GroupId { set; get; }

        [Display(Name = "Txt Path")]
        public String MFGPlant { set; get; }

        //[MaxLength(12)]
        [StringLength(12)]
        [Display(Name = "Head Size")]
        [RegularExpression("[0-9]+(,[0-9]+)*,?", ErrorMessage = "Invalid Head Size")]
        public String CylinderSize { set; get; }

        //[MaxLength(12)]
        [StringLength(12)]
        [Display(Name = "Dye/Ble")]
        public String DyeBle { set; get; }

        [Display(Name = "Txt Group")]
        public String TextileGroup { set; get; }

        [Display(Name = "Fabric Item")]
        public String Fabric { set; get; }

        //[MaxLength(12)]
        [StringLength(12)]
        [Display(Name = "Alt")]
        public String Alt { set; get; }

       // [MaxLength(12)]
        [StringLength(12)]
        [Display(Name = "Machine")]
        public String Machine { set; get; }

        // [MaxLength(12)]
        [StringLength(12)]
        [Display(Name = "Yarn")]
        public String Yarn { set; get; }

        [Display(Name = "Due Date")]
        public String DueDate { set; get; }

        [Display(Name = "Week")]
        public String Week { set; get; }

        [Display(Name = "More Weeks")]
        public String MoreWeeks { set; get; }

        [Display(Name = "Business Unit")]
        public String BusinessUnit { set; get; }

        [Display(Name = "Corp Division")]
        public String CorpDiv { get; set; }

        [Display(Name = "Suggested Lots")]
        public bool SuggestedLots { set; get; }

        [Display(Name = "Spill Overs")]
        public bool SpillOver { set; get; }

        [Display(Name = "Locked Lots")]
        public bool LockedLots { set; get; }

        [Display(Name = "Released Lots")]
        public bool ReleasedLots { set; get; }

        [Display(Name = "Released Lots This Week")]
        public bool ReleasedLotsThisWeek { set; get; }

        [Display(Name = "Customer Orders")]
        public bool CustomerOrders { set; get; }

        [Display(Name = "Events")]
        public bool Events { set; get; }

        [Display(Name = "Max Build")]
        public bool MaxBuild { set; get; }

        [Display(Name = "Forecast")]
        public bool Forecast { set; get; }

        [Display(Name = "Stock Target")]
        public bool StockTarget { set; get; }

        [Display(Name = "TILs/TILINCR")]
        public bool TILs { set; get; }

        [Display(Name = "Exclude \"Buy\" Orders")]
        public bool ExcludeBuyOrders { set; get; }

        [Display(Name = "Body Only")]
        public bool BodyOnly { set; get; }

        [Display(Name = "Trim Only")]
        public bool TrimOnly { set; get; }

        [Display(Name = "Group Only")]
        public bool GroupOnly { set; get; }

        [Display(Name = "BOM Mismatch")]
        public bool BOMMismatches { set; get; }

        //public List<WOManagementSearch> SearchCriteria { set; get; }
        
    }
}
