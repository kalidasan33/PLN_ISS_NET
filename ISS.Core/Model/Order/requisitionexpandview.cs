using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISS.Common;

namespace ISS.Core.Model.Order
{
    public class RequisitionExpandView
    {
        [Display(Name = "Style")]
        public string Style { get; set; }
        [Display(Name = "Color")]
        public string Color { get; set; }
        [Display(Name = "Attribute")]
        public string Attribute { get; set; }
        [Display(Name = "Size")]
        public string Size { get; set; }
        [Display(Name = "Revision")]
        public decimal Revision { get; set; }
        [Display(Name = "UM")]
        public string UOM { get; set; }
        [Display(Name = "Eaches")]
        public decimal Eaches { get; set; }
        [Display(Name = "Standard Case Qty")]
        public decimal StdQty { get; set; }

        [Display(Name = "Qty")]
        public decimal Qty
        {
            get
            {
                //return decimal.Round((((UOM.ToUpper() == "DZ") || (UOM.ToUpper() == "CT")) ? (Eaches / 12).RoundCustom(2) : Eaches), 2, MidpointRounding.AwayFromZero);
                return decimal.Parse(((UOM.ToUpper() == "DZ") || (UOM.ToUpper() == "CT")) ? FormatDozens(Eaches, ".") : Eaches.ToString());
            }
        }

        [Display(Name = "Cases")]
        public decimal Cases
        {
            get
            {
                //return (Eaches / ((StdQty > 0) ? StdQty : 1)).RoundCustom(0);
                return ((StdQty > 0) ? (Eaches / StdQty) : 0).RoundCustom(0);
            }
        }

        [Display(Name = "Requisition Id")]
        public string RequisitionId { get; set; }

        [Display(Name = "Path")]
        public string MfgPath { get; set; }

        [Display(Name = "Size Desc")]
        public string SizeDesc { get; set; }

        public string MPDesc { get; set; }

        public string StyleDesc { get; set; }
        public string ColorDesc { get; set; }
        public string AttributeDesc { get; set; }
        public string RevisionDesc { get; set; }
        public string PackDesc { get; set; }
        
        public string StyleFormat 
        { 
            get
            {
                return StyleDesc + "/" + ColorDesc + "/" + AttributeDesc + "/" + SizeDesc + "/" + RevisionDesc + "/" + PackDesc + "/" + MPDesc;
            }
        }

        public IList<RequisitionBOM> BomComponents { get; set; }


        private string FormatDozens(decimal q, string del)
        {
            decimal d;
            decimal r;
            d = Math.Truncate(q / 12);
            r = (q % 12);
            r = Math.Round(r);
            if ((q < 0))
            {
                r = (r * -1);
            }
            return (d.ToString() + (del + (r.ToString().PadLeft(2, '0'))));
        }
        public String StyleType { get; set; }

        [Display(Name = "Qty")]
        public string QtyStd
        {
            get
            {
                return (((UOM.ToUpper() == "DZ") || (UOM.ToUpper() == "CT")) ? FormatDozens(Eaches,"-") : Eaches.ToString());
            }
        }

        [Display(Name = "Size Short Desc")]
        public string SizeShortDesc { get; set; }

        public String getSKUString(bool IncludeRevision = false)
        {
            return Style + LOVConstants.Delimeter + Color + LOVConstants.Delimeter + Attribute + LOVConstants.Delimeter + Size + ((IncludeRevision) ? (LOVConstants.Delimeter + Revision.ToString()) : String.Empty);
        }
    }
}
