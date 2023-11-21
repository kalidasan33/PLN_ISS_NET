using ISS.Common;
using ISS.Core.Model.Order;
using System;
using System.Web.Mvc;

namespace ISS.Web.Controllers
{
    public partial class OrderController : BaseController
    {
        [HttpGet]
        public ActionResult CreateRequisitions(RequisitionOrderSearch reqSearch, bool? autoLoad)
        {
            var req = new Requisition()
            {
                ProdStatus = LOVConstants.ProductionStatus.Locked,
                ReqStatus = LOVConstants.RequestStatus.UnderConstruction
            };
            req.CreatedBy = req.UpdatedBy = GetCurrentUserName();
            req.CreatedOn = req.UpdatedOn = DateTime.Now.Date;
            req.ReqDetailTracking = false;
            ViewBag.autoLoad = autoLoad.HasValue ? autoLoad.Value : false;
            //ViewBag.FromSummary = reqSearch;
            
            return View( req);
        }
    }
}