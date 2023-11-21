using System.ComponentModel.DataAnnotations;

namespace ISS.Core.Model.Textiles
{
    public class TextileAllocation
    { 
        public string Alloc_Type { get; set; }

        public string Plant { get; set; }

        public decimal Cylinder_Size { get; set; }

        public string Dye_Shade_CD { get; set; }

        public string Resource_ID { get; set; }

        public string Machine_Type_CD { get; set; }

        public string Production_Status { get; set; }

        public decimal Machine_Cut { get; set; }

        public decimal LBS { get; set; }

        public decimal AVG { get; set; }

        [Display(Name = "Head Size")]
        public string HeadSize { get; set; }   

               

        public string PlantCut { get; set; }

        public System.Collections.Generic.List<YarnItem> YarnItems { get; set; }

    }
}
