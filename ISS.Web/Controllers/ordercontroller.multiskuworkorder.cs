using ISS.Core.Model.Order;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
 
using ISS.Core.Model.Common;
using ISS.Common;

namespace ISS.Web.Controllers
{
    public partial class OrderController : BaseController
    {
        [HttpGet]
        public ActionResult CreateMultiSKUWorkOrder()
        {

            return View();
        }


        [HttpPost]
        public ActionResult CreateMultiSKUWorkOrder(String submit)
        {

            return View();
        }

        public JsonResult GetPlanner([DataSourceRequest] DataSourceRequest result)
        {
            var data = appService.GetPlannerNameAndCode();

            return Json(data.ToDataSourceResult(result), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMultiSkuSizes([DataSourceRequest]DataSourceRequest request, SkuViewModel searchCriteria)
        {
            List<MultiSKUSizes> lstSizes = new List<MultiSKUSizes>();
            var data = orderService.GetSizes(searchCriteria.Style_Cd, searchCriteria.Color_Cd, searchCriteria.Attribute_Cd).Select(x => new { SizeCD = x.Size, Size = x.SizeShortDes}).ToList().Distinct().ToList();

            if (data.Count > 0)
            {
                lstSizes = data.Select(s => new MultiSKUSizes { SizeCD = s.SizeCD, Size = s.Size, Qty = 0 }).OrderByDescending(s => s.Qty).OrderBy(s => s.SizeCD).ToList();
                if (searchCriteria.SizeList != null)
                {
                    foreach (MultiSKUSizes sizes in lstSizes)
                    {
                        var item = searchCriteria.SizeList.Where(x => x.SizeCD == sizes.SizeCD).ToList();
                        if (item.Count > 0)
                        {
                            sizes.Qty = item[0].Qty;
                        }
                    }
                }
                
            }
            //lstSizes.Where(s => s.Qty > 0).OrderBy(s => s.SizeCD).ToList().AddRange(lstSizes.Where(s => s.Qty == 0).OrderBy(s => s.SizeCD));

            var orderedList = lstSizes.Where(s => s.Qty > 0).OrderBy(s => s.SizeCD).ToList();
            orderedList.AddRange(lstSizes.Where(s => s.Qty == 0).OrderBy(s => s.SizeCD));

            var result = orderedList.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetGroupID()
        {
            var data = orderService.GetGroupID();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOrdersToCreate([DataSourceRequest] DataSourceRequest request)
        {
            var numbers = (from p in Enumerable.Range(1, 20)
                           select new SelectListItem
                           {
                               Text = p.ToString(),
                               Value = p.ToString()
                           });

            return Json(numbers.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult InsertMultiSku(WorkOrderHeader woHeader)
        {
            if (woHeader != null)
            {
                woHeader.UpdatedBy = woHeader.CreatedBy = GetCurrentUserName();

            }
            var resu = orderService.InsertWorkOrder(woHeader);
            //return Json(resu);

            return Json(resu);
        }

        [HttpPost]
        public JsonResult ValidateMultiSku(WorkOrderHeader woHeader)
        {
            if (woHeader != null)
            {
                woHeader.UpdatedBy = woHeader.CreatedBy = GetCurrentUserName();

            }
            var resu = orderService.ValidateMultiSku(woHeader);
            //return Json(resu);

            return Json(resu);
        }

        [HttpPost]
        public JsonResult GetCuttingAltId(SKU sku)
        {
            var data = orderService.GetCuttingAltId(sku);

            return Json(data);
        }

        [HttpPost]
        public JsonResult GetBulkGroupID(decimal dgridCount)
        {
            var data = orderService.GetBulkGroupID(dgridCount);

            return Json(data);
        }


        [HttpPost]
        public JsonResult GetOrderDetailByOrderLabel(string superOrder)
        {
            var data = orderService.GetOrderDetailByOrderLabel(superOrder);

            if (data.Count > 0)
            {
                List<MultiSKUSizes> lstSizes = new List<MultiSKUSizes>
                {
                    new MultiSKUSizes()
                    {
                        SizeCD = data[0].Size,
                        Size = data[0].SizeShortDes,
                        Qty = 0
                    }
                };

                data[0].SizeList = lstSizes;
                data[0].CreateBd = (data[0].CreateBDInd ==LOVConstants.Yes) ? true : false;
                data[0].DozensOnly = (data[0].DozensOnlyInd ==LOVConstants.Yes) ? true : false;

                Random r = new Random();
                data[0].Id = r.Next(1, 100);
            }

            return Json(data);
        }

        [HttpPost]
        public JsonResult GetGarmentSKU(WorkOrderDetail wrkOrder)
        {
            var data = orderService.GetGarmentSKU(wrkOrder.SellingStyle,wrkOrder.ColorCode,wrkOrder.Attribute,wrkOrder.SizeList,wrkOrder.MfgPathId);
            return Json(data);
        }

    }
}