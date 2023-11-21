using ISS.Common;
using ISS.Core.Model.Common;
using ISS.Core.Model.Order;
using ISS.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace ISS.Web.Controllers
{
    public partial class OrderController
    {
       
        public Result CreateSKUChange(List<WOMDetail> data)
        {
            
            var res=orderService.UpdateChange(data);
            //Edit and save a single row 
            return res;
        }

        
    }
}
