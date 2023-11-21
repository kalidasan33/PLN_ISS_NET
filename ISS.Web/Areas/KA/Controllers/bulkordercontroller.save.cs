using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KA.Core.Model.BulkOrder;
using KA.BusinessRules.Contract.BulkOrder;
using ISS.Common;
using System.Web.WebPages;
namespace ISS.Web.Areas.KA.Controllers
{
    public partial class BulkOrderController : ISS.Web.Controllers.BaseController
    {
        
        [HttpPost]
        public JsonResult UpdateBulkOrder(BulkOrderDetail req, List<BulkOrderDetail> reqDet)
        {
            if (req != null)
            {
                req.CreatedBy = GetCurrentUserName();
            }
            var resu = bulkOrdService.UpdateBulkOrder(req, reqDet);
            return Json(new { resu.Key, resu.Value, reqDet });
        }


        [HttpPost]
        public JsonResult DeleteBulkOrder(String BulkNumber, String ProgramSource)
        {
            var currReqList = bulkOrdService.GetBulkOrderDetail(BulkNumber, ProgramSource);
            if (currReqList != null && currReqList.Count > 0)
            {
                var currReq = currReqList[0];
                if (!currReqList.Any(e=> e.ProcessedToOS==LOVConstants.BulkOrderStatus.Processed)
                    )
                {

                    if (bulkOrdService.DeleteBulkOrder(BulkNumber, ProgramSource))
                    {
                        return Json(new
                        {
                            Status = true,
                            ErrMsg = "Bulk Order deleted successfully. " + BulkNumber
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            Status = false,
                            ErrMsg = "Failed to delete Bulk Order. " + BulkNumber
                        });
                    }
                }// status check
                else
                {
                    return Json(new
                    {
                        Status = false,
                        ErrMsg = "Not allowed to delete this Bulk Order. " + BulkNumber
                    });
                }
            } //end exist blk nbr check
            return Json(new
            {
                Status = false,
                ErrMsg = "Invalid Bulk Order detail. " + BulkNumber
            });
        }

        [HttpPost]
        public JsonResult GetLineNumberBulk(List<BulkOrderDetail> BulkOrderDetail,int count)
        {
            var LineNumber = bulkOrdService.GetLineNumber(BulkOrderDetail);
            List<string> list = new List<string>() { LineNumber };
            for (int i = 0; i < count - 1; i++)
            {
                LineNumber=ISS.Common.Utility.SequenceGenerator.NextValue(LineNumber);
                list.Add(LineNumber);
            }
                return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult BulkOrderComplete(BulkOrderDetail req, List<BulkOrderDetail> reqDet)
        {
            if (req != null)
            {
                req.CreatedBy = GetCurrentUserName();
            }
            var resu = bulkOrdService.CompleteComponentProcess(req, reqDet);
            return Json(new { resu.Key, resu.Value, reqDet });
        }
        [HttpPost]
        public JsonResult BulkOrderActivate(BulkOrderDetail req, List<BulkOrderDetail> reqDet)
        {
            if (req != null)
            {
                req.CreatedBy = GetCurrentUserName();
            }
            var resu = bulkOrdService.ActivateComponentProcess(req, reqDet);
            return Json(new { resu.Key, resu.Value, reqDet });
        }
    }
}