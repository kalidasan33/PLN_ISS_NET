using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISS.Web.Controllers;
using ISS.Core.Model.Order;
using Kendo.Mvc.UI;
using ISS.BusinessRules.Contract.Common;
using ISS.BusinessRules.Contract.Order;
using KA.BusinessRules.Contract.AttributionOrder;
using Kendo.Mvc.Extensions;
using ISS.Common;
using System.Web.WebPages;

namespace ISS.Web.Areas.KA.Controllers
{
    public partial class AttributionOrderController : BaseController
    {


        private readonly IOrderService orderService;
        private readonly IApplicationService appService;
        private readonly IAttributionOrderService attriborderService;

        public AttributionOrderController(IOrderService summService, IApplicationService applicationService, IAttributionOrderService attrorderService)
        {
            orderService = summService;
            appService = applicationService;
            attriborderService = attrorderService;
        }

        [HttpPost]
        public ContentResult WOManagement([DataSourceRequest]DataSourceRequest request, WOManagementSearch search, String Src)
        {
            if (String.IsNullOrEmpty(Src))
            {
                return ISSJson(new List<WOMDetail>().ToDataSourceResult(request));
            }
            var data = orderService.GetWODetail(search);

            return ISSJson(data.ToDataSourceResult(request));
        }

        [HttpGet]
        public ActionResult Management(WOManagementSearch SummaryView, bool? autoLoad)
        {
            ViewBag.PlanWeek = GetPlantWeek();
            ViewBag.autoLoad = autoLoad.HasValue ? autoLoad.Value : false;
            if (!(autoLoad.HasValue ? autoLoad.Value : false))
                SummaryView.SuggestedLots = SummaryView.SpillOver = SummaryView.LockedLots = SummaryView.ReleasedLotsThisWeek = true;

            SummaryView.CustomerOrders = SummaryView.Events = SummaryView.MaxBuild = SummaryView.TILs = SummaryView.StockTarget = SummaryView.Forecast = true;

            return View("WOManagement", SummaryView);
        }
        [HttpPost]
        public JsonResult SaveWOMdata(List<WOMDetail> data, String mode)
        {
            try
            {
                data.Each(e =>
                  {
                      if (!string.IsNullOrEmpty(e.QtyEach))
                        e.Qty = e.QtyEach.Replace("-",".").AsDecimal().ConvertDzToEaches();
                  }); 
                 
                String uName = GetCurrentUserName();
                Log("Mode ==>> " + mode + " User :" + uName);
                foreach (WOMDetail item in data)
                {
                    item.CreatedBy = item.UpdatedBy = uName;
                }
                if (mode == "Delete")
                {
                    if (DeleteWOMOrders(data))
                    {
                        return Json(new { Status = true, mode = mode, data = data });
                    }
                    else
                    {
                        return Json(new
                        {
                            Status = false,
                            mode = mode,
                            data = data,
                            success = data.Count(e => e.IsDeleted),
                            failed = data.Count(e => !e.IsDeleted)
                        });
                    }
                }
                else if (mode == "Grouped")
                {
                    if (UpdateWOMGroupedOrders(data))
                    {
                        return Json(new { Status = true, mode = mode, data = data });
                    }
                    else
                    {
                        return Json(new
                        {
                            Status = false,
                            mode = mode,
                            data = data,
                            success = data.Count(e => !e.ErrorStatus),
                            failed = data.Count(e => e.ErrorStatus)
                        });
                    }
                }
                else if (mode == "EditPFSUngroup")
                {
                    if (UpdateWOMOrders(data))
                    {
                        return Json(new { Status = true, mode = mode, data = data });
                    }
                    else
                    {
                        return Json(new
                        {
                            Status = false,
                            mode = mode,
                            data = data
                        });
                    }
                }
                else if (mode == "Recalc")
                {
                    if (CreateSKUChange(data))
                    {
                        return Json(new { Status = true, mode = mode, data = data });
                    }
                    else
                    {
                        return Json(new
                        {
                            Status = false,
                            mode = mode,
                            data = data
                        });
                    }

                }
                else if (mode == "Summarize")
                {
                    if (SummarizeAOMOrders(data))
                    {
                        return Json(new { Status = true, mode = mode, data = data });
                    }
                    else
                    {
                        return Json(new
                        {
                            Status = false,
                            mode = mode,
                            data = data
                        });
                    }
                }
            }
            catch (Exception ee)
            {
                data.Each(item =>
                {
                    item.ErrorStatus = true;
                    item.ErrorMessage = ee.Message;
                });
                return Json(new
                {
                    Status = false,
                    mode = mode,
                    data = data
                });
            }

            return Json(false);
        }

        /// <summary>
        /// Summarize
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool SummarizeAOMOrders(List<WOMDetail> data)
        {
            if (attriborderService.SummarizeAOMOrders(data))
            {
                return true;
            }

            return false;
        }
        /// <summary>
        /// EditPFSUngroup
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool UpdateWOMGroupedOrders(List<WOMDetail> data)
        {
            if (attriborderService.UpdateWOMGroupedOrders(data))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// EditPFSUngroup
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool UpdateWOMOrders(List<WOMDetail> data)
        {
            bool Status = true;

            data.ForEach(item =>
            {

                if (attriborderService.UpdateWOMOrder(item))
                {
                    item.ErrorStatus = false;
                }
                else
                {
                    item.ErrorStatus = true;
                    Status = false;
                }
            });
            return Status;
        }

        private bool DeleteWOMOrders(List<WOMDetail> data)
        {
            bool Status = true;
            var req = new RequisitionDetail();
            data.ForEach(item =>
            {
                item.IsDeleted = false;
                req.SuperOrder = item.SuperOrder;
                req.OrderVersion = item.OrderVersion;
                if (orderService.DeleteOrder(req))
                {
                    item.IsDeleted = true;
                }
                else
                {
                    Status = false;
                }
            });


            return Status;
        }

        protected string GetPlantWeek()
        {
            return appService.GetPlantWeek();
        }


        private bool CreateSKUChange(List<WOMDetail> data)
        {

            if (attriborderService.UpdateChange(data))
                return true;
            else
                return false;

            //Edit and save a single row 

        }

    }
}