using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISS.Common;

namespace ISS.Core.Model.Order
{
   public  class WOMPipeline
    {

       public string CategoryCode { get; set; }

       public string Info { get; set; }

       public string Plant { get; set; }

       public decimal StartWeek { get; set; }
       
       public DateTime? StartDate { get; set; }

     
       public DateTime? EndDate { get; set; }


       public String StartDateStr
       {
           get
           {
               return (StartDate.HasValue) ? StartDate.Value.ToString(LOVConstants.DateFormatDisplay) : string.Empty;
           }
       }

       public String EndDateStr
       {
           get
           {
               return (EndDate.HasValue) ? EndDate.Value.ToString(LOVConstants.DateFormatDisplay) : string.Empty;
           }
       }
    }
}
