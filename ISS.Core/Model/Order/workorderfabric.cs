using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ISS.Core.Model.Order
{
     public class WorkOrderFabric
    {
        //Fabric Grid fields
        [Display(Name = "ID")]
        public decimal Id { get; set; }

        [Display(Name = "Fabric")]
        public string Fabric { get; set; }

        [Display(Name = "Cyl Size")]
        public decimal CylSize { get; set; }

        [Display(Name = "Spread")]
        public string SpreadCode { get; set; }

        [Display(Name = "Comp Cd")]
        public string CompCode { get; set; }

        [Display(Name = "Dye Cd")]
        public string DyeCode { get; set; }

        [Display(Name = "Lbs")]
        public decimal Lbs { get; set; }

        [Display(Name = "Pull Frm Stock")]
        public bool PullFromStock { get; set; }
        public string PullFromStockInd { get; set; }
        [Display(Name = "Size Cd")]
        public string SizeShortDes { get; set; }
        public string Size { get; set; }
        public string ResourceId { get; set; }

        public string FabricGroup { get; set; }
        public string FabColorCode { get; set; }
        public string SupplyPlant { get; set; }

        public string ParentStyle { get; set; }
        public string ParentColor { get; set; }
        public string ParentAttribute { get; set; }
        public string ParentSize { get; set; }
        public string ParentSizeDes { get; set; }
        public string ParentBoMId { get; set; }
        public string ParentMFGPathId { get; set; }
        public string MatTypeCode { get; set; }
        public bool IsHide { get; set; }
        public bool Merged { get; set; }
        public int SeqId { get; set; }
        public int ParentId { get; set; }
    }
}
