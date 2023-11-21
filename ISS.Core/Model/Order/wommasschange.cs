using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using ISS.Common;

namespace ISS.Core.Model.Order
{
    public class WOMMassChange 
    {
        public WOMMassChange()
        {
            CreateBd = "Off";
        } 

        [Display(Name = "Sew Path")]
        public String MfgPathId { set; get; }

        //[MaxLength(6)]
        [StringLength(6)]
        [Display(Name = "Cut Path")]
        public String CutPath { set; get; }

        [Display(Name = "Txt Path")]
        public String Txtpath { set; get; }

       
        [Display(Name = "DC")]
        [StringLength(3, ErrorMessage = "Invalid DC")]
        public String DC { set; get; }

        [Display(Name = "Dozen")]
        public int? Dozen { set; get; }

        [Display(Name = "Quantity")]
        public String QtyEach   { set; get; }

        [Display(Name = "Order Status")]
        public String OrderStatusMC { set; get; }

        [Display(Name = "Priority")]
        [StringLength(2, ErrorMessage = "Priority must be between 0 and 99")]
        public String Priority { set; get; }

         [StringLength(6)]
        [Display(Name = "Cat Cd")]
        public String CatCD { set; get; }

        [Display(Name = "Due Date")]
        public String DueDateStr { set; get; }

        [Display(Name = "Due Date")]
        [RegularExpression(@"^(0[1-9]|1[012])[/](0[1-9]|[12][0-9]|3[01])[/](19|20|21|22)\d\d$", ErrorMessage = "Invalid Format")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? MDueDate { set; get; }

        [Display(Name = "BOM Update")]
        public bool BOMUpdate { set; get; } 

        

        [Display(Name = "Create BD")]
        public String CreateBd { set; get; }

        [Display(Name = "DZ Only")]
        public bool DZOnly { set; get; } 

        [Display(Name = "Off")]
        public bool CreateBDOff { set; get; } 

        [MaxLength(4)] //PFE
        [Display(Name = "Color")]
        public String MColor { set; get; }

        [MaxLength(6)]
        [Display(Name = "Attribute")]
        public String MAttribute { set; get; } 

        [Display(Name = "Rev")]
        public String Rev { set; get; }

        [Display(Name = "Alt Id")]
        public String AltId { set; get; }

        [Display(Name = "Textile Plant")]
        public String TextilePlant { set; get; }    

        [Display(Name = "Machine")]
        public String MachineMC { set; get; }

        [Display(Name = "Limit")]
        public decimal Limit { set; get; }

        [Display(Name = "Fit to Machine")]
        public bool FitToMachine { set; get; }
    }
}
