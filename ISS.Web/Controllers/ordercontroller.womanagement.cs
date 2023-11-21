using ISS.Common;
using ISS.Core.Model.Common;
using ISS.Core.Model.Order;
using ISS.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace ISS.Web.Controllers
{
    public partial class OrderController : BaseController
    {


        [HttpGet]
        public ActionResult WOManagement(WOManagementSearch SummaryView, bool? autoLoad)
        {
            ViewBag.PlanWeek = GetPlantWeek();
            ViewBag.autoLoad = autoLoad.HasValue ? autoLoad.Value : false;
           
            if (!(autoLoad.HasValue ? autoLoad.Value : false))
                SummaryView.SuggestedLots = SummaryView.SpillOver = SummaryView.LockedLots = SummaryView.ReleasedLotsThisWeek = true;

            SummaryView.CustomerOrders = SummaryView.Events = SummaryView.MaxBuild = SummaryView.TILs = SummaryView.StockTarget = true;
            return View(SummaryView);
        }

        [HttpPost]
        public ContentResult WOManagement([DataSourceRequest]DataSourceRequest request, WOManagementSearch search, String Src)
        {

            
            if (String.IsNullOrEmpty(Src))
            {
                return ISSJson(new List<WOMDetail>().ToDataSourceResult(request));
            }
            var data=orderService.GetWODetail(search);
            
            return ISSJson( data.ToDataSourceResult(request)); 
        }


        [HttpPost]
        public ContentResult getProductionOrders([DataSourceRequest]DataSourceRequest request, string SuperOrder)
        {
            var data = orderService.getProductionOrders(SuperOrder);
            return ISSJson(data.ToDataSourceResult(request));
        }

        //Asif 10/9/2018 To Add Selling Sku popup while clicking in Selling Style in WOM screen

        [HttpPost]
        public ContentResult getSellingSku([DataSourceRequest]DataSourceRequest request, string SuperOrder)
        {
            var data = orderService.getSellingSku(SuperOrder);
            return ISSJson(data.ToDataSourceResult(request));
        }

       
        [HttpPost]
        public ContentResult getProductionOrdersLBS([DataSourceRequest]DataSourceRequest request, string SuperOrder)
        {
            var data = orderService.getProductionOrdersLbs(SuperOrder);
            return ISSJson(data.ToDataSourceResult(request));
        }

        [HttpPost]
        public ContentResult getFabricDetail([DataSourceRequest]DataSourceRequest request, string SuperOrder)
        {
            var data = orderService.getFabricDetail(SuperOrder);
            return ISSJson(data.ToDataSourceResult(request));
        }

        [HttpPost]
        public ContentResult getPipelineDates([DataSourceRequest]DataSourceRequest request, string SuperOrder)
        {
            var data = orderService.getPipelineDates(SuperOrder);
            return ISSJson(data.ToDataSourceResult(request));
        }


        [HttpPost]
        public ContentResult PopulateMachineTypes([DataSourceRequest]DataSourceRequest request, String SellingStyle, String ColorCode, String Attribute, String Plant, String MFGPathId)
        {
            if (String.IsNullOrEmpty(SellingStyle) && String.IsNullOrEmpty(ColorCode) && String.IsNullOrEmpty(Attribute) && String.IsNullOrEmpty(Plant))
            {
                return ISSJson(new List<String>().ToDataSourceResult(request));
            }
            var Wod = new WorkOrderDetail();
            Wod.iStyle = SellingStyle;
            Wod.iColor = ColorCode;
            Wod.iAttribute = Attribute;
            Wod.iDesPlt = Plant;
            Wod.iMFGPathId = MFGPathId;
            var data = orderService.PopulateMachineTypes(Wod);
            return ISSJson(data.ToDataSourceResult(request));
        }


         [HttpPost]
        public ContentResult getAlternateId([DataSourceRequest]DataSourceRequest request, String SellingStyle, String ColorCode, String Attribute, String Size )
        {
           
            var sku = new SKU();
            sku.Style = SellingStyle;
            sku.Color = ColorCode;
            sku.Attribute = Attribute;
            sku.Size = Size;

            var data = orderService.GetCuttingAltId(sku);
            return ISSJson(data.ToDataSourceResult(request));
        } 
        
        [HttpPost]
         public ContentResult PopulateCutPathTxtPath([DataSourceRequest]DataSourceRequest request, String SellingStyle, String ColorCode, String Attribute, String Plant, String MFGPathId, String DyeCode, String SizeCd)
        {
            if (String.IsNullOrEmpty(SellingStyle) && String.IsNullOrEmpty(ColorCode) && String.IsNullOrEmpty(Attribute)  )
            {
                return ISSJson(new List<String>().ToDataSourceResult(request));
            }
            var Wod = new WorkOrderDetail();
            Wod.iStyle = SellingStyle;
            Wod.iColor = ColorCode;
            Wod.iAttribute = Attribute;            
            Wod.iMFGPathId = MFGPathId;
            Wod.DyeCode = DyeCode;
            Wod.iSize = SizeCd;
            var data = orderService.PopulateCutPathTxtPath(Wod);
            return ISSJson(data.ToDataSourceResult(request));
        }



        [HttpPost]
        public ContentResult PopulateCutPathTxtPathBySuperOrder([DataSourceRequest]DataSourceRequest request, String SuperOrder, String DyeCode, String CutPath)
        {
            var data = orderService.PopulateCutPathTxtPath(SuperOrder, DyeCode, CutPath);
            return ISSJson(data.ToDataSourceResult(request));
        }

        public ContentResult PopulateMachineTypesBySuperOrder(String SuperOrder,String Plant)
        {

            var grmnt =orderService.GetGarmentSKU(SuperOrder);
            if (grmnt.Count > 0)
            {
                grmnt[0].iDesPlt = Plant;
                grmnt[0].SizeList = new List<MultiSKUSizes>
                {
                    new MultiSKUSizes()
                    {
                        SizeCD = grmnt[0].iSize,
                        Qty = 0
                    }
                };

                return ISSJson( orderService.PopulateMachineTypes(grmnt[0]));
            }

            return null;

        }

        // routes.MapMvcAttributeRoutes(); on init
       // [Route("{productId:int}/{productTitle}")]
        public JsonResult ValidatePlant(String Plant)
        { 
            return Json(orderService.ValidatePlant(Plant));           
        }

         [HttpPost]
        public ContentResult GetPFSDetail([DataSourceRequest]DataSourceRequest request, WOMDetail wo)
        {  
            return ISSJson(GetFsabricList(wo).ToDataSourceResult(request));
        }

         private WOMDetail GetGarmentStyle(Dictionary<String,WOMDetail> dict,WOMDetail item)
         {
             if (!String.IsNullOrEmpty(item.ParentOrder))
             {
                 if(dict.ContainsKey(item.ParentOrder))
                 {
                     var parent=dict[item.ParentOrder];
                     if(parent.MatlTypeCode==LOVConstants.MATL_TYPE_CD.Garment){
                        return parent;
                     }
                     return GetGarmentStyle(dict,parent);
                 }
             }
             return item;
         }
         private List<WOMDetail> GetFsabricList(  WOMDetail wo)
         {
             var data = orderService.GetSuperOrderDetail(wo.SuperOrder);
             var fabrics = data.Where(e => e.OrderLabel != wo.SuperOrder && e.MatlTypeCode == LOVConstants.MATL_TYPE_CD.Fabric).ToList();
             var dict=data.ToDictionary(d=> d.OrderLabel);
           
             fabrics.Each(item =>
                 {

                     var parent = GetGarmentStyle(dict, item);
                     if (parent!=null)
                     {
                         item.GarmentStyle = parent.Style;
                         item.GarmentColor = parent.Color;
                     }
                     if(item.StdLoss!=1)
                     item.Lbs =  item.Qty / (1 - item.StdLoss);
                     if(item.Lbs.HasValue)
                        item.Lbs = item.Lbs.Value.RoundCustom(2);
                 });
             //fabrics.GroupBy(group => group.Style).Each(item =>
             //    {
             //        if (item.Count() > 1)
             //        {
             //            item.Each(child =>
             //                {
             //                    child.IsHide = true;
             //                });
             //            var curr=item.FirstOrDefault();
             //            var instance = new WOMDetail()
             //            {                          
             //                GarmentStyle =curr.GarmentStyle,
             //                GarmentColor = curr.GarmentColor,
             //                Style = curr.Style,
             //                CylinderSizes = curr.CylinderSizes,
             //                SpreadTypeCode = curr.SpreadTypeCode,
             //                SpreadCompCode = curr.SpreadCompCode,
             //                DyeCode = curr.DyeCode,
             //                MachineCut = curr.MachineCut,
             //                Lbs = item.Sum(r=> r.Lbs).Value.RoundCustom(2),
             //                PFSInd=curr.PFSInd,
             //                IsMerged=true,
             //            };
             //            fabrics.Insert(0,instance);
             //        }
             //    });
             return fabrics;
         }

         
         public JsonResult CloneWOMDetail(  WOMDetail wo)
         {
             return Json(wo);
         }

         public JsonResult SKUChange(WOMDetail wo)
         {
             return Json(wo);
         }

         public JsonResult ValidateProdStatus(String AssortCode, String TxtPath, String OrderStatus, String SuperOrder, String OldOrderStatus)
         {
             if (OrderStatus == LOVConstants.ProductionStatus.Released)
             {
                 if (!orderService.ValidatePlant(TxtPath))
                 {
                     return Json(new
                     {
                         Status = false,
                         Msg = "Textile Plant is Invalid.  The Finishing Indicator must be set to a 'Y' in the APS Plant table.  It is currently set to an 'N'.<br/>  Either select a valid Textile Plant or contact the Cost Dept if you believe the plant you selected is a valid Textile Plant.",
                     });
                 }
             }
             if (AssortCode == "A" && (OldOrderStatus==LOVConstants.ProductionStatus.Locked || OldOrderStatus==LOVConstants.ProductionStatus.Suggested))
             {
                 var before = orderService.ValidateAltBefore(SuperOrder);
                 var validate = orderService.ValidateAlt(SuperOrder);
                 if (validate > 0)
                 {
                     return Json(new
                     {
                         Status = false,
                         BOMUpdate = true,
                         OrderStatus = LOVConstants.ProductionStatus.Locked,
                         Msg = "Alternate is not valid for all components, Status will be 'L-ocked' and Update BOM is now checked."
                     });
                 }
                 else if (before > 1)
                 {
                     return Json(new
                     {
                         Status = true,
                         BOMUpdate = true,
                     });
                 }
             }
             return Json(new
             {
                 Status = true,
             });
         }

         public JsonResult getAllMultiSKU(List<WOMDetail> multiList,String GroupId,String token)
         {
             WOManagementSearch search = new WOManagementSearch() { GroupId= GroupId};
             var data = orderService.GetWODetail(search);
             var filteredData = from d in data
                                where (! multiList.Any(e => e.SuperOrder == d.SuperOrder))
                                select d;
             return Json(new { data = filteredData, token = token}); 
         }

         [HttpPost]
         public ActionResult ExportWOMDetails(String gridData, String gridColumns)
         {
             List<WOMDetail> data = new List<WOMDetail>();
             if (!string.IsNullOrWhiteSpace(gridData))
             {
                 WOManagementSearch search = JsonConvert.DeserializeObject<WOManagementSearch>(gridData);
                 data = orderService.GetWODetailExport(search);

                 data.RemoveAll(s => s.IsHide);
             }

             string[] excelColumns = new string[] { "GroupId", "OrderId", "OrderType", "OrderStatusDesc", "SellingStyle", "Style", "Color", "Attribute", "SizeShortDes", "Revision", "AltId", "QtyDZ", "TotalDozens", "LbsStr", "MC", "CylinderSizes", "PFSInd", "StartDate", "CurrDueDate", "TxtPath", "CutPath", "SewPath", "Atr", "MfgPathId", "DcLoc", "ExpeditePriority", "CategoryCode", "DemandDriver", "DemandSource", "Rule", "Priority", "CreateBd", "DozensOnly", "BOMUpdate", "SpillOver", "PackCode", "MakeOrBuy", "Note", "DemandDate" };
             if(!String.IsNullOrEmpty(gridColumns)){
                 excelColumns = gridColumns.Split(',');
                 excelColumns=excelColumns.Skip(1).ToArray();
             }
             string fileName = string.Format("WOM_Details_{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));

             return new ExcelResult(fileName).
                 AddSheet<WOMDetail>(data, "WO Details", excelColumns);
         }


         [HttpPost]
         public JsonResult SaveWOMdata(List<WOMDetail> data,String mode)
         {
             try
             {
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
                 if (mode == "Grouped")
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
                 if (mode == "EditPFSUngroup")
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
                     var res = CreateSKUChange(data);
                     return Json(new
                     {
                         Status = res.Status,
                         mode = mode,
                         data = data
                     });
                 }
             }
             catch (Exception ee){
                 data.Each(item => {
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
         /// EditPFSUngroup
         /// </summary>
         /// <param name="data"></param>
         /// <returns></returns>
         private bool UpdateWOMGroupedOrders(List<WOMDetail> data)
         {
             if (orderService.UpdateWOMGroupedOrders(data))
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
                
                 if (orderService.UpdateWOMOrder(item))
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

         public JsonResult GetPlanners([DataSourceRequest] DataSourceRequest result)
         {
             var data = appService.GetPlannerListFull();

             return Json(data.ToDataSourceResult(result), JsonRequestBehavior.AllowGet);
         }

		[HttpPost]
         public JsonResult GetNote(string superOrder)
         {
             var data = orderService.GetNote(superOrder);

             return Json(data);
         }


        public JsonResult GetOrderStatus()
        {
            var listOrderStatus = new List<SelectListItem>(){
                            new SelectListItem()
                                {
                                    Text = ProductionStatus.Suggested.ToString(),
                                    Value =ProductionStatus.Suggested.GetDescription()
                                } ,
                                 new SelectListItem()
                                {
                                    Text = ProductionStatus.Locked.ToString(),
                                    Value =ProductionStatus.Locked.GetDescription()
                                } ,
                                 new SelectListItem()
                                {
                                    Text = ProductionStatus.Released.ToString(),
                                    Value =ProductionStatus.Released.GetDescription()
                                } ,


                            };
            return Json(listOrderStatus, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMFGPathIdDD(WorkOrderDetail wod)
        {
            var data = orderService.GetMFGPathId(wod.SellingStyle,
                   wod.ColorCode,
                    wod.Attribute,
                   wod.SizeList,
                    wod.PrimaryDC).Select(x => new { MfgPathId = x.MfgPathId, SewPlt = x.MfgPathId + "-" + x.SewPlt, SewPltMfg = x.SewPlt });
            data = data.ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult WOMGetCatCode()
        {
            var data = orderService.GetCatCode();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult WOMGetPackCode()
        {
            var data = orderService.GetPackCode();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult WOMgetAlternateId(WorkOrderDetail wod)
        {
            //CuttingAlt
            var sku = new SKU();
            sku.Style = wod.SellingStyle;
            sku.Color = wod.ColorCode;
            sku.Attribute = wod.Attribute ;
            sku.Size = wod.SizeList[0].SizeCD;

            var data = orderService.GetCuttingAltId(sku);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PopulateCutPath(CutpathInput CI)
        {
            var data = orderService.PopulateCutPathTxtPath(CI.SuperOrder, CI.DyeCode, CI.CutPath);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult WOMPopulateMachine(CutpathInput CI)
        {
            var grmnt = orderService.GetGarmentSKU(CI.SuperOrder);
            if (grmnt.Count > 0)
            {
                grmnt[0].iDesPlt = CI.CutPath;
                grmnt[0].SizeList = new List<MultiSKUSizes>
                {
                    new MultiSKUSizes()
                    {
                        SizeCD = grmnt[0].iSize,
                        Qty = 0
                    }
                };
                grmnt[0].Size = grmnt[0].iSize;

                return Json(orderService.PopulateMachineTypes(grmnt[0]), JsonRequestBehavior.AllowGet);
            }

            return null;

        }  
    }
}
