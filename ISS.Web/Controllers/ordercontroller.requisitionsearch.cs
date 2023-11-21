using ISS.Core.Model.Order;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
  
using System.IO;
using ISS.Web.Helpers;
using ISS.Common;
namespace ISS.Web.Controllers
{
    public partial class OrderController : BaseController
    {
        [HttpGet]
        public ActionResult RequisitionSearch()
        {
            DateTime today = DateTime.Today; // As DateTime
            var data = appService.GetPlanBeginEndDates();
            var beginweek = data.Select(x => x.Week_Begin_Date).FirstOrDefault();

            var reqSearch = new RequisitionSearch();
            reqSearch.FromDate = beginweek;
            //reqSearch.ToDate = beginweek.AddDays(6);
            reqSearch.ToDate = today;

            return PartialView(reqSearch);
        }

        [HttpPost]
        public JsonResult RequisitionSearch([DataSourceRequest]DataSourceRequest request, RequisitionSearch reqSearch)
        {
            var data = orderService.GetRequisitionSearch(reqSearch);
            var result = data.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RequisitionExpandView(string RequisitionId)
        {
            var data = orderService.GetRequisitionHeader(RequisitionId);
            foreach (Requisition req in data)
            {
                var com = orderService.GetOrderComments(req);
                req.RequisitionComment = com;
            }
            return Json((data.Count > 0) ? data[0] : null, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RequisitionCommentsSave(OrderComment requisition)
        {
            var data = orderService.AddOrderComment(requisition);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ReqExpandBomComponents(RequisitionExpandView reqExpand)
        {
            var data = orderService.GetRequisitionBOM(reqExpand);
            return Json(data, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public PartialViewResult ReqExpandViewComponents(string requisitionId)
        {
            ViewBag.NeedSummary = false;
            List<RequisitionExpandView> lstReq = null;
            if (!String.IsNullOrEmpty(requisitionId))
            {
                lstReq = CalculateExpandView(requisitionId);
                var reqDetails = orderService.GetRequisitionDetail(new RequisitionDetail() { RequisitionId = requisitionId });
                if (reqDetails.Count > 0)
                {
                    var groups = reqDetails.GroupBy(group => group.getSKUString()).ToList();
                    if (groups.Count() != reqDetails.Count)
                    {
                        ViewBag.NeedSummary = true;
                    }
                }
            }
            return PartialView("_RequisitionExpandViewGrid", lstReq);

        }


        public ActionResult ReqExpandViewExport(string requisitionId)
        {
            string fileName = string.Format("SourcedWO-ExpandView{0}", DateTime.Now.ToString("yyyyMMddHHmmss")) + ".xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";


            List<Requisition> lstReqHeader = orderService.GetRequisitionHeader(requisitionId).ToList();
            foreach (Requisition req in lstReqHeader)
            {
                var com = orderService.GetOrderComments(req);
                req.RequisitionComment = com;
            }
            var listReq = CalculateExpandView(requisitionId);


            RequisitionExpandViewExport reqExportService = new RequisitionExpandViewExport();
            MemoryStream msExcel = reqExportService.ExportRequisitionExpandView(lstReqHeader, listReq);

            var fsr = new FileStreamResult(msExcel, contentType);
            fsr.FileDownloadName = fileName;

            return fsr;

        }

        private List<RequisitionExpandView> CalculateExpandView(string requisitionId)
        {
            List<RequisitionExpandView> lstReq = orderService.GetReqExpandDetails(requisitionId);
            foreach (RequisitionExpandView rev in lstReq)
            {
                var data = orderService.GetRequisitionBOM(rev);
                rev.BomComponents = data;
            }

            var UsageSum = 0.0m;
            var WQty = 0.0m;
            var WQtyFinal = 0.0m;



            lstReq.ForEach(item =>
                {
                    UsageSum = item.BomComponents.Sum(b => (b.BomLevel + "").Equals(LOVConstants.BOMLevels.Level1) ? b.Usage : 0);
                    item.StyleType = LOVConstants.BOMStyle.SEL;
                    item.BomComponents.Each(bomItem =>
                        {
                            if ((bomItem.BomLevel + "").Equals(LOVConstants.BOMLevels.Level1))
                            {
                                WQty = item.StdQty / UsageSum * bomItem.Usage;
                                bomItem.Dz = (WQty / 12.0m).RoundCustom(0).ToString();
                                WQtyFinal = bomItem.StdQty / UsageSum * bomItem.Usage;
                                if ((item.UOM == LOVConstants.UOM.DZ || item.UOM == LOVConstants.UOM.CT) && WQtyFinal > 12)
                                {
                                    bomItem.DisplayCases = (WQtyFinal / 12.0m).RoundCustom(0).ToString() + " DZ (" + WQtyFinal.RoundCustom(0) + ") per case ";
                                }
                                else
                                {
                                    bomItem.DisplayCases = WQtyFinal.RoundCustom(0) + " per case ";
                                }
                                bomItem.StyleType = LOVConstants.BOMStyle.PKG;

                            }
                            else
                            {
                                bomItem.DisplayCases = bomItem.Usage.RoundCustom(0) + " pieces per pack";
                                bomItem.StyleType = LOVConstants.BOMStyle.MFG;

                            }

                            //For display
                          //  bomItem.StdQty = (int)(bomItem.StdQty / LOVConstants.Dozen) * LOVConstants.Dozen + bomItem.StdQty % LOVConstants.Dozen;
                        });

                    //For display only
                   // item.StdQty = (int)(item.StdQty / LOVConstants.Dozen) * LOVConstants.Dozen + item.StdQty % LOVConstants.Dozen;
                });

            ViewBag.ExpandSummary = "sfsdf";
            return lstReq;
        }

        [HttpPost]
        public JsonResult ReleaseToSourcing(Requisition requisition)
        {
            var data = orderService.ReleaseToSourcing(requisition);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RequisitionResetForConstruction(Requisition requisition)
        {
            var data = orderService.RequisitionResetForConstruction(requisition);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult RequisitionComment(string RequisitionId)
        {
            var data = orderService.GetRequisitionHeader(RequisitionId);
            return Json((data.Count > 0) ? data[0] : null, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RequisitionCommentGet(string RequisitionId)
        {
            var data = orderService.GetRequisitionHeader(RequisitionId);
            foreach (Requisition req in data)
            {
                var com = orderService.GetOrderComments(req);
                req.RequisitionComment = com;
            }
            return Json((data.Count > 0) ? data[0] : null, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ARQComment(Requisition req)
        {
            var data = orderService.GetOrderComments(req);
            //return Json((data.Count > 0) ? data[0] : null, JsonRequestBehavior.AllowGet);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportSourceSearchDetails(RequisitionSearch reqSearch)
        {
            var data = orderService.GetRequisitionSearch(reqSearch);

            string[] excelColumns = new string[] { "RequisitionId", "ProdStatus", "PlanningContact", "CreateDate", "ReqStatus", "BusUnit", "VendorName", "VenCity", "VenCountry", "VendorNo", "VendorLoc", "Locked" };

            string fileName = string.Format("RequisitionSearchDetails{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));

            return new ExcelResult(fileName).
                AddSheet<RequisitionSearch>(data, "Requisition Search Details", excelColumns);
        }

        [HttpPost]
        public ContentResult GetDuplicateRecords(List<RequisitionDetail> list)
        {
            if (list != null)
            {
                list.Each(e =>
                {
                    e.isHide = false;
                    e.IsDeleted = false;
                    e.IsInserted = false;
                    e.SuperOrder = "";
                    e.IsMovedObject = false;
                    e.Id = null;
                    e.IsSummarized = false;
                });
            }
            return ISSJson(list);
        }

        [HttpPost]
        public ContentResult GetDuplicateRecordsSingle(List<RequisitionDetail> list, RequisitionDetail dataItem)
        {
            List<RequisitionDetail> duplicateList = new List<RequisitionDetail>();
            if (list != null)
            {
                if (dataItem != null && list.Count > 0)
                {

                    var size = dataItem.Size;
                    var sizeLit = dataItem.SizeLit;
                    var Qty = dataItem.Qty;
                    list.Each(item =>
                        {
                            if (item.Qty > 0)
                            {
                                var obj = dataItem.CloneModel<RequisitionDetail>();

                                if (list.Count > 1)
                                {
                                    if (item.Size == dataItem.Size)
                                    {
                                        dataItem.Qty = item.Qty;
                                        dataItem.Dpr = 9999;//CA#61076-18. To set DPr Rule as 9999 while duplicating the records
                                        //TBD
                                    }
                                    else
                                    {
                                        obj.Size = item.Size;
                                        obj.SizeLit = item.SizeLit;
                                        obj.Qty = item.Qty;
                                        obj.Dpr = 9999; //CA#61076-18. To set DPr Rule as 9999 while duplicating the records
                                        obj.isHide = false;
                                        obj.IsDeleted = false;
                                        obj.IsInserted = false;
                                        obj.SuperOrder = String.Empty;
                                        obj.Id = null;
                                        obj.IsSummarized = false;
                                        obj.IsMovedObject = false;
                                        var stdqty = orderService.GetStdCaseQty(obj.Style, obj.Color, obj.Attribute, obj.Size, obj.Rev.ToString());
                                        if (stdqty.Count > 0)
                                        obj.StdCase = stdqty[0].StdCase;
                                        duplicateList.Add(obj);
                                    }
                                }
                                else
                                {
                                    obj.Size = item.Size;
                                    obj.SizeLit = item.SizeLit;
                                    obj.Qty = item.Qty;
                                    obj.Dpr = 9999; //CA#61076-18. To set DPr Rule as 9999 while duplicating the records
                                    obj.Id = null;
                                    obj.IsSummarized = false;
                                    obj.isHide = false;
                                    obj.IsDeleted = false;
                                    obj.IsInserted = false;
                                    obj.SuperOrder = String.Empty;
                                    obj.IsMovedObject = false;
                                    var stdqty = orderService.GetStdCaseQty(obj.Style, obj.Color, obj.Attribute, obj.Size, obj.Rev.ToString());
                                    if(stdqty.Count>0)
                                    obj.StdCase = stdqty[0].StdCase;
                                    duplicateList.Add(obj);
                                }
                            }
                        });
                }

            }
            
            return ISSJson(new { newList = duplicateList, dataItem = dataItem });
        }

        [HttpPost]
        public ContentResult GetDuplicateRecordsBulkSingle(List<RequisitionDetail> list, RequisitionDetail dataItem)
        {
            List<RequisitionDetail> duplicateList = new List<RequisitionDetail>();
            if (list != null)
            {
                if (dataItem != null && list.Count > 0)
                {

                    var size = dataItem.Size;
                    var sizeLit = dataItem.SizeLit;
                    var Qty = dataItem.Qty;
                    list.Each(item =>
                    {
                        if (item.Qty > 0)
                        {
                            var obj = dataItem.CloneModel<RequisitionDetail>();

                             if (list.Count > 1)
                            {
                                if (item.Size == dataItem.Size)
                                {
                                    dataItem.Qty = item.Qty;
                                    dataItem.Dpr = 9999;//CA#61076-18. To set DPr Rule as 9999 while duplicating the records
                                    //TBD
                                }
                                else
                                {
                                    obj.Size = item.Size;
                                    obj.Qty = item.Qty;
                                    obj.Dpr = 9999;//CA#61076-18. To set DPr Rule as 9999 while duplicating the records
                                    obj.isHide = false;
                                    obj.IsDeleted = false;
                                    obj.IsInserted = false;
                                    obj.SuperOrder = String.Empty;
                                    obj.Id = null;
                                    obj.IsSummarized = false;
                                    obj.IsMovedObject = false;
                                    obj.ErrorStatus = item.ErrorStatus;
                                    duplicateList.Add(obj);
                                }
                            }
                            else
                            {
                                obj.Size = item.Size;
                                obj.Qty = item.Qty;
                                obj.Dpr = 9999;//CA#61076-18. To set DPr Rule as 9999 while duplicating the records
                                obj.Id = null;
                                obj.IsSummarized = false;
                                obj.isHide = false;
                                obj.IsDeleted = false;
                                obj.IsInserted = false;
                                obj.SuperOrder = String.Empty;
                                obj.IsMovedObject = false;
                                duplicateList.Add(obj);
                            }
                        }
                    });
                }

            }

            return ISSJson(new { newList = duplicateList, dataItem = dataItem });
        }
    }
}