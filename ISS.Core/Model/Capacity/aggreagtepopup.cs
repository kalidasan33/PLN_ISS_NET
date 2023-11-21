using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISS.Core.Model.Common;
using System.ComponentModel.DataAnnotations;

namespace ISS.Core.Model.Capacity
{
    public class AggregatePopup : ModelBase
    {

        [Display(Name = "Table Name")]
        public string TableName { get; set; }

        [Display(Name = "Corp Division")]
        public string CorpDivisionGroup { get; set; }

        [Display(Name = "Child Plant")]
        public string ChildPlant { get; set; }

        [Display(Name = "Parent Plant")]
        public string ParentPlant { get; set; }


        [Display(Name = "Capacity Group")]
        public string CapacityGroup { get; set; }

        
        

        [Display(Name = "Work Center")]
        public string CapacityAlloc { get; set; }

        [Display(Name = "Include")]
        public string IncludeInd { get; set; }

        [Display(Name = "User Id")]
        public string UserId { get; set; }

        

        [Display(Name = "Create date")]
        public string CreateDate { get; set; }

        [Display(Name = "Update date")]
        public string UpdateDate { get; set; }

       
    }
}
