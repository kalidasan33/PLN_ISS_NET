using ISS.Core.Model.Order;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISS.Core.Model.Common;
using Kendo.Mvc.Extensions;
using ISS.Common;
using ISS.Web.Helpers;

namespace ISS.Web.Controllers
{
    public partial class OrderController : BaseController
    {
        [HttpPost]
        public ActionResult RetrieveOrder([DataSourceRequest]DataSourceRequest request, RequisitionOrderSearch search, List<RequisitionOrder> SO, String Src)
        {
            if (String.IsNullOrEmpty(Src))
            {
                return this.Json(null, JsonRequestBehavior.AllowGet);
            }
            var data = orderService.GetRequisitionOrder(search);
            var datac = data.Count();
            var dis = data.Select(r => r.SuperOrder).Distinct().ToList();
            DataSourceResult resultSet=null;
            if (SO != null && SO.Count > 0)
            {
                var hash = SO.Select(p => p.SuperOrder);
                // var resultSet = data.Where(e => Sku.Any(p => p.Sku == e.getSKUString())).ToDataSourceResult(request);
                resultSet = data.Where(e => !hash.Contains(e.SuperOrder)).ToDataSourceResult(request);
            }
            else
            {  
                resultSet = data.ToDataSourceResult(request);
            }  
            return  ISSJson(resultSet ); 
        }

        [HttpPost]
        public ActionResult SummarizeData([DataSourceRequest]DataSourceRequest request, List<RequisitionDetail> reqDet, bool IsSummarized, String FormMode)
        {
            if (reqDet != null)
            {
               
                if (IsSummarized)
                {
                    var filteredList = reqDet.Where(w => !w.IsDeleted);
                    int i = 0;
                    filteredList.GroupBy(e => e.getSKUString(true))
                         .Each(e =>
                         {
                             //e.Key;
                             var list = e.ToList();
                             var item = list.FirstOrDefault();
                             var listHide=list.Skip(1).ToList();
                             if (listHide.Count > 0)
                             {
                                 item.IsSummarized=item.IsDirty = true;
                                 item.SummarizedQty = item.Qty;
                                 item.Qty = list.Sum(t => t.Qty.ConvertDzToEaches()).ConvertEachesToDz();
                                 item.Id = ( ++i).ToString();
                                 listHide.Each(p =>
                                 {
                                     p.isHide=p.IsDirty = true;
                                     p.Id = item.Id;
                                 });
                             }
                         });

                }
                else
                {                 
                   // reqDet.RemoveAll(e => e.isHide && !String.IsNullOrEmpty(e.Id));
                    var filteredList = reqDet.Where(w => !w.IsDeleted);
                    filteredList.Where(e=> e.IsSummarized).ToList()
                          .Each(item =>
                        {
                            // Qty      Summ       1st     2nd
                            // 20       10          5       5
                            // 10       10          5       5
                            // 5        10          5       5
                                var Qty = item.Qty.ConvertDzToEaches();

                                var listHide = reqDet.Where(e=> e.Id==item.Id
                                    && (!e.IsSummarized && e.isHide )
                                    
                                    ).ToList();

                                //  if(listHide.Count>0) QtyRem =listHide.Sum(w => w.Qty);
                                if (listHide.Count > 0)
                                {
                                    if (item.IsSummarized)
                                    {
                                        if (Qty <= item.SummarizedQty.ConvertDzToEaches())
                                        {
                                            item.Qty = Qty;
                                            Qty = 0;
                                        }
                                        else
                                        {
                                            item.Qty = item.SummarizedQty.ConvertDzToEaches();
                                            Qty = Qty - item.SummarizedQty.ConvertDzToEaches();
                                        }
                                        item.IsSummarized = false;
                                        item.Id = null;
                                    }
                                    item.isHide = false;
                                    listHide.Each(p =>
                                    {
                                        if (item.getSKUString() == p.getSKUString())
                                        {
                                            if (Qty <= p.Qty.ConvertDzToEaches())
                                            {
                                                p.Qty = Qty;
                                                Qty = 0;
                                                p.Qty = p.Qty.ConvertEachesToDz();
                                            }
                                            else
                                            {
                                                if (p.Qty == 0)
                                                {
                                                    p.Qty = Qty;
                                                    p.Qty = p.Qty.ConvertEachesToDz();
                                                    Qty = 0;
                                                }
                                                else
                                                {
                                                    Qty = Qty - p.Qty.ConvertDzToEaches();
                                                }
                                            }
                                            p.Id = null;
                                            p.isHide = false;
                                            p.IsSummarized = false;
                                        }//end check 
                                        else
                                        {
                                            reqDet.Remove(p);
                                        }
                                    });


                                    if (Qty > 0)
                                        item.Qty += Qty;
                                    item.Qty = item.Qty.ConvertEachesToDz();
                                }
                                            
                        }); // end looping summarized

                    
                    reqDet.RemoveAll(e => e.isHide && !String.IsNullOrEmpty(e.Id));
                }
                return Json(new { Status = true, data = reqDet.ToDataSourceResult(request) });
            }
            return Json(new { Status = false });
        }


        [HttpPost]
        public JsonResult UpdateRequisition(Requisition req, List<RequisitionDetail> reqDet)
        {
            if(req!=null){
               req.UpdatedBy= GetCurrentUserName();               
            }
            var resu = orderService.UpdateRequisition(req, reqDet);
            return Json(resu);
        }

        
        [HttpPost]
        public JsonResult InsertRequisition(Requisition req, List<RequisitionDetail> reqDet)
        {
            if (req != null)
            {
                req.UpdatedBy = req.CreatedBy = GetCurrentUserName();

            }
            var resu = orderService.InsertRequisition(req, reqDet);
            return Json(resu);
        }

 

        
        /// <summary>
        ///  For Retrieve Order
        /// </summary>
        /// <param name="list"></param>
        /// <param name="req"></param>
        /// <param name="SO"> List of Super Order collection</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult validateOrderItems(List<RequisitionOrder> list, Requisition req, List<RequisitionOrder> SO,bool SessionNew)
        {
           
            List<RequisitionDetail> validatedList = new List<RequisitionDetail>();
            if (SessionNew)
            {
                SessionHelper.RequisitionOrder = SO ??new List<RequisitionOrder> ();
            }
           
            validatedList = AddToOrderDetails(list, req, SessionHelper.RequisitionOrder);
            return this.Json(
                new {
                    hasErrors = validatedList.Any(t=> t.ErrorStatus),
                    ErrorCount=validatedList.Count(t=> t.ErrorStatus),
                    SuccessCount = validatedList.Count(t => !t.ErrorStatus),                    
                    data=validatedList                    
                }
                , JsonRequestBehavior.AllowGet); 
        }

        protected List<RequisitionDetail> AddToOrderDetails(List<RequisitionOrder> list, Requisition req, List<RequisitionOrder> SO)
        {
            List<RequisitionDetail> validatedList = new List<RequisitionDetail>();
            if (list != null && list.Count > 0)
            {
                var tempOrderIds = SO.Select(p => p.SuperOrder).ToList();
                list.ForEach(item =>
                {
                    var sku = new SKU(item);
                    if (tempOrderIds.Contains(item.SuperOrder))               
                    {
                        return;
                    }
                    else
                    {
                        SO.Add(new RequisitionOrder()
                            {
                                SuperOrder = item.SuperOrder
                            });
                        tempOrderIds.Add(item.SuperOrder);
                    }
                    var val = ValidateSKUForSourceOrder(sku, req.DcLoc, req.MFGPathId, req.LwCompany, req.VendorNo, req.LwVendorLoc, req.BusinessUnit,false);
                    var orderDetail = GetRequisitionDetailObject(item);
                    orderDetail.ErrorStatus = val.ErrorStatus;
                    orderDetail.ErrorMessage = val.ErrorMessage;
                    orderDetail.PlannedLeadTime = val.PlannedLeadTime;
                    orderDetail.TransportationTime = val.TransportationTime;
                    orderService.calculatePlannedAndScheduledDates(req, orderDetail);
                    orderDetail.IsMovedObject = orderDetail.IsDirty = true;
                    validatedList.Add(orderDetail);
                   

                });

               
            }
            return validatedList;
        }

        public JsonResult ClearOrdersInSession()
        {
            SessionHelper.Clear(SessionConstant.REQ_DETAILS);            
            return null;
        }
        [HttpPost]
        public JsonResult validateRequisitionDetailRow(RequisitionDetail reqDet, Requisition req,bool? KA)
        {
            var planWeek = appService.GetPlanBeginEndDates();

            if (reqDet != null)
            {

                var sku = new SKU(reqDet);
                var val = ValidateSKUForSourceOrder(sku, req.DcLoc, req.MFGPathId, req.LwCompany, req.VendorNo, req.LwVendorLoc, req.BusinessUnit,KA);
                reqDet.ErrorStatus = val.ErrorStatus;
                reqDet.ErrorMessage = val.ErrorMessage;
                reqDet.PlannedLeadTime = val.PlannedLeadTime;
                reqDet.TransportationTime = val.TransportationTime;
                if (!KA.HasValue || !KA.Value)
                orderService.calculatePlannedAndScheduledDates(req, reqDet);

            }
            return this.Json(reqDet , JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult validateRequisitionDetail(List<RequisitionDetail> reqDet, Requisition req, bool? KA)
        {
            //var planWeek=appService.GetPlanBeginEndDates();
         
            if (reqDet != null && reqDet.Count > 0)
            {
               // reqDet.ForEach(item =>
                foreach(var item in reqDet)
                {
                    var sku = new SKU(item);
                    var val = ValidateSKUForSourceOrder(sku, req.DcLoc, req.MFGPathId, req.LwCompany, req.VendorNo, req.LwVendorLoc, req.BusinessUnit,KA);                  
                    item.ErrorStatus = val.ErrorStatus;
                    item.ErrorMessage = val.ErrorMessage;
                    item.PlannedLeadTime = val.PlannedLeadTime;
                    item.TransportationTime = val.TransportationTime;
                    if (!KA.HasValue || !KA.Value)
                    orderService.calculatePlannedAndScheduledDates(req, item);                    
                    //if(planWeek.Count>0){
                    //    item.PlanDate = planWeek[0].Week_End_Date.AddHours((int)val.PlannedLeadTime);
                    //}                    
                }
                //);
            }
            return this.Json(
                new {
                    hasErrors = reqDet.Any(t => t.ErrorStatus),
                    ErrorCount = reqDet.Count(t => t.ErrorStatus),
                    SuccessCount = reqDet.Count(t => !t.ErrorStatus),
                    data = reqDet 
                }
                , JsonRequestBehavior.AllowGet); 
        }
   
        private SOStyleValidaion ValidateSKUForSourceOrder(SKU sku, String DCLoc, String MGFPathId, Decimal LwCompany, Decimal VendorNo, String LwVendorLoc, String BUSUnit,bool? KA)
        {
            bool Vendor = true;
            if(!KA.HasValue  ||  !KA.Value)
                Vendor = orderService.VerifyVendor(sku, LwCompany, VendorNo, LwVendorLoc);
            var DCPath = orderService.VerifyDCMfgpath(sku, DCLoc);
            var Sewpath = orderService.VerifyDCMfgpath(sku, MGFPathId);
            var Plan = orderService.GetPlanningleadTime(sku, DCLoc, MGFPathId);
            var BUStyle = orderService.GetStyleValidation(sku.Style, BUSUnit);
            var skuMFG = orderService.VerifySKUDCCombination(sku, MGFPathId);
            var Rev = orderService.VerifyRevision(sku, MGFPathId);
            //TBD BU and Style **

            var item= new SOStyleValidaion
            {                
                VendorIsValid = Vendor,
                DCPathIsValid = DCPath,
                SewpathIsValid = Sewpath,
                ErrorStatus = (!(BUStyle && Vendor && DCPath && Sewpath && skuMFG && Rev)) ? true : false,
                ErrorMessage =  String.Empty
            };
            //(!(Vendor && DCPath && Sewpath))?"Invalid Style.":String.Empty
            if (!skuMFG) item.ErrorMessage = sku.getSKUString() + " - This is not a valid SKU/MFG Path combination. ";
            else if (!Rev) item.ErrorMessage = " Revision Code " + sku.Rev + " is not valid for SKU/MFG Path combination." + sku.getSKUString() + " / " + DCLoc;
            else if (!BUStyle) item.ErrorMessage = sku.getSKUString() + " - Invalid Business Unit. ";
            else if (!DCPath)
            {
                item.ErrorMessage = sku.getSKUString() + " - Invalid DC Location. ";
            }
            else if (!Vendor || !Sewpath) item.ErrorMessage += (String.IsNullOrEmpty(item.ErrorMessage)? sku.getSKUString():String.Empty)+" - Invalid vendor details.";
            if (Plan.Count > 0)
            {
                item.PlannedLeadTime = Plan[0].PlannedLeadTime;
                item.TransportationTime = Plan[0].TransportationTime;
            }
            return item;
        }

        private RequisitionDetail GetRequisitionDetailObject(RequisitionOrder order)
        {
            return new RequisitionDetail()
            {
                Style = order.Style, 
                Color = order.Color,
                Attribute = order.Attribute,
                Size = order.Size,
                SizeLit=order.SizeLit,
                Rev = order.Rev ,
                Uom=order.UOM,
                Qty = order.Qty,
                StdCase = order.StdCaseQty,
                PlannedLeadTime=order.PlannedLeadTime,
                TransportationTime=order.TransportationTime,
                Dpr=order.RuleNo,
                Description=orderService.GetStyleDesc(order.Style),
                SuperOrder=  order.SuperOrder,
                Priority= order.Priority,       
                OrderVersion=order.OrderVersion,
                IsDirty=true,
            };

        }
        
    }
}