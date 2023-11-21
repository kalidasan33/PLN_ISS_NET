using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 

namespace ISS.Core.Model.Common
{
  public  class Result
    {
      public Result()
      {
          ErrType = string.Empty;
      }
      public bool Status {set;get;}
      public String ErrMsg { get; set; }
      public String ErrType { get; set; }

      public dynamic Property1 { get; set; }

      public decimal Id { get; set; }

      public List<dynamic> ErrDetails { get; set; }
      public int SuccessCount { get; set; }
      public int  FailCount { get; set; }
    }
}
