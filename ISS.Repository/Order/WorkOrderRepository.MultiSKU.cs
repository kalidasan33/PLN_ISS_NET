using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISS.DAL;
using ISS.Common;
using System.Data;
using ISS.Common;
using ISS.Core.Model.Common;
using ISS.Core.Model.Order;
using Oracle.DataAccess.Client;
using System.Transactions;


namespace ISS.Repository.Order
{
    public partial class WorkOrderRepository : RepositoryBase
    {
        public decimal GetGroupID()
        {
            var query = new StringBuilder();

            query.Append("select ISS_PROD_ORDER_SEQ.NEXTVAL FROM DUAL");

            var result = (decimal)ExecuteScalar(query.ToString());
            return result;

        }

        public Result WOValidations(WorkOrderHeader woHeader)
        {
            RecomputeDueDatesForSave(woHeader);
            bool Status = true;
            Result res = new Result();
            decimal ErrId = -1;
            List<dynamic> ErrList = new List<dynamic>();
            List<dynamic> ErrDtls = new List<dynamic>();
            KeyValuePair<bool, string> statusMsg = new KeyValuePair<bool, string>();
            //woHeader.MachinePlant = "R3";
            if (!ValidateTextile(woHeader))
            {
                res.ErrMsg = "Textile Plant is Invalid.  The Finishing Indicator must be set to a 'Y' in the APS " +
                        "Plant table. It is currently set to an 'N'.  Either select a valid Textile Plant " +
                        "or contact the Cost Dept if you believe the plant you selected is a valid Textile Plant.";
                Status = false;
            }
            //else if (woHeader.MachinePlant == null || woHeader.MachinePlant == "")
            //{
            //    res.ErrMsg = "Machine Type cannot be a blank value.";
            //    Status = false;
            //}
            else if (woHeader.WODetails.Count <= 0)
            {
                res.ErrMsg = "At least one order must be ceated.";
                Status = false;
            }
            else if (ValidateQuantity(woHeader, out ErrList))
            {
                res.ErrMsg = "Invalid Quantity.";
                Status = false;
                res.Id = ErrId;
                if (ErrList.Count > 0)
                    ErrDtls.AddRange(ErrList);
            }
            else if (CheckInvalidDates(woHeader, out ErrList))
            {
                res.ErrMsg = "Invalid Start Date. Date cannot be more than 60 days past due.";
                Status = false;
                res.Id = ErrId;
                if (ErrList.Count > 0)
                    ErrDtls.AddRange(ErrList);
            }
            else if (!ValidateRevisionReq(woHeader, out ErrList))
            {
                res.ErrMsg = "Invalid Revision.";
                Status = false;
                res.Id = ErrId;
                if (ErrList.Count > 0)
                    ErrDtls.AddRange(ErrList);
            }
            else
            {

                statusMsg = ValidateGroup(woHeader, out ErrList);
                if (statusMsg.Key != null)
                    Status = statusMsg.Key;

                if (statusMsg.Value != null)
                {
                    res.ErrMsg = statusMsg.Value;
                    res.Id = ErrId;
                    if (ErrList.Count > 0)
                    {
                        ErrDtls.AddRange(ErrList);
                    }
                }

                if (Status)
                {

                    var Msg = String.Empty;

                    for (int j = 0; j < woHeader.WODetails.Count; j++)
                    {

                        var woCum = woHeader.WOCumulative.Where(x => (x.Merged == false) && (x.StyleCode == woHeader.WODetails[j].PKGStyle) && (x.LevelInd == 0)).ToList();

                        //var woCum = woHeader.WOCumulative.Where(x => (x.Merged == false) && (x.StyleCode == woHeader.WODetails[j].PKGStyle)).ToList();
                        for (int i = 0; i < woCum.Count; i++)
                        {
                            WorkOrderCumulative item = woCum[i];

                            bool ress = ExternalSkuValidate(woHeader.WODetails[j].SellingStyle, woHeader.WODetails[j].ColorCode, woHeader.WODetails[j].Attribute, item.SizeCode);
                            if (ress == true)
                            {
                                woHeader.WODetails[j].DemandSource = Get_Demand_Source(woHeader.WODetails[j].PurchaseOrder, woHeader.WODetails[j].LineItem);
                            }
                            else
                            {
                                woHeader.WODetails[j].DemandSource = null;
                                woHeader.WODetails[j].DemandDriver = null;
                            }

                            Status = ValidateDetailBeforeWOSave(woHeader, item, out Msg, out ErrList);
                            if (!Status)
                            {
                                res.ErrMsg = Msg;
                                //res.Id = ErrId;
                                if (ErrList.Count > 0)
                                    ErrDtls.AddRange(ErrList);
                            }
                        }
                    }

                    if (Status && Msg == String.Empty)
                    {
                        woHeader.WODetails.ToList().ForEach(detail =>
                        {
                            if (detail.GroupId == 0)
                            {
                                var woFab = woHeader.WOFabric.Where(x => (x.Id == detail.Id) && (!x.Merged)).ToList();
                                woFab.ForEach(fab =>
                                {
                                    if ((fab.CylSize == 0) && ((fab.CompCode != null && fab.CompCode != "")))
                                    {
                                        res.ErrMsg = "Alternate not valid for all components";
                                        Status = false;
                                        res.Id = detail.Id;
                                        detail.ErrorStatus = true;
                                        detail.ErrorMessage = res.ErrMsg;
                                        ErrDtls.Add(detail);
                                    }
                                });
                            }
                        });
                    }


                    if (Status && Msg == String.Empty)
                    {
                        //for (int j = 0; j < woHeader.OrdersToCreate; j++)
                        //{
                        for (int i = 0; i < woHeader.WODetails.Count; i++)
                        {
                            WorkOrderDetail woDetail = woHeader.WODetails[i];
                            var consumData = ConsumeOrders(woDetail.SellingStyle, woDetail.ColorCode, woDetail.Attribute, woDetail.SizeList);

                            if (consumData.Value != "")
                            {
                                res.ErrMsg = consumData.Value;
                                Status = false;
                                res.ErrType = "ConsumeOrders";
                            }
                            res.Property1 = consumData.Key;
                            woDetail.ConsumedOrders = consumData.Key;
                        }
                        //}
                    }
                }
            }

            if (woHeader.PlannerCd == "-Select-")
            {
                woHeader.PlannerCd = String.Empty;
            }

            res.Status = Status;
            if (ErrDtls != null && ErrDtls.Count > 0)
            {
                res.ErrDetails = ErrDtls.Distinct().ToList();
            }

            return res;
        }

        public Result InsertWorkOrder(WorkOrderHeader woHeader)
        {
            Result res = new Result();
            res.ErrMsg = "Required fields are missing.";

            string PrevStyle = "";

            if (woHeader != null && woHeader.WODetails != null && woHeader.WODetails.Count > 0)
            {
                try
                {

                    res = WOValidations(woHeader);
                    if (!res.Status)
                    {
                        if (!woHeader.SkipConsumeOrders && res.ErrType == "ConsumeOrders")
                        {
                            res.Property1 = null;
                            return res;
                        }

                    }

                    if (woHeader.SkipConsumeOrders && res.ErrType == "ConsumeOrders")
                    {
                        res.Status = true;
                        res.ErrMsg = string.Empty;
                    }

                    if (res.Status)
                    {
                        Requisition req = new Requisition();
                        req.OrderVersion = LOVConstants.GlobalOrderVersion;
                        req.DemandType = woHeader.Dmd;
                        req.CurrentDueDate = woHeader.PlannedDate;
                        req.DemandDate = woHeader.PlannedDate;

                        String ErrMsgValDet = String.Empty;
                        int orderCounter = 0;

                        for (int j = 0; j < woHeader.OrdersToCreate; j++)
                        {
                            woHeader.WODetails.ToList().ForEach(detail =>
                            {
                                PrevStyle = "";

                                using (TransactionScope scope = new TransactionScope())
                                {
                                    try
                                    {
                                        BeginTransaction();
                                        foreach (var item in woHeader.WOCumulative)
                                        {
                                            item.SuperOrder = String.Empty;
                                        }
                                        res = WorkOrderDetail(detail, woHeader, res, req);

                                        if (res.Status)
                                        {
                                            CommitTransaction();
                                            orderCounter++;
                                        }

                                        else RollbackTransaction();
                                    }
                                    catch (Exception ES)
                                    {
                                        res.Status = false;
                                        RollbackTransaction();
                                        Log("Save createWO-trans scope");
                                        Log(ES);
                                        res.ErrMsg = ES.Message;
                                    }
                                }

                            }); // end Loop WO detail

                        }
                        //woHeader.WODetails.Count
                        //res.ErrMsg = woHeader.OrdersToCreate + " Order(s) created successfully.";
                        if (orderCounter > 0)
                        {
                            var recdCount = woHeader.WODetails.Count * woHeader.OrdersToCreate;
                            res.ErrMsg = recdCount + " Order(s) created successfully.";
                            res.Status = true;
                        }
                    }
                }
                catch (Exception ee)
                {
                    //Log(ee);
                    res.Status = false;
                    res.ErrMsg = ee.Message;
                }
                finally
                {

                }

            }

            return res;
        }

        public Result WorkOrderDetail(WorkOrderDetail detail, WorkOrderHeader woHeader, Result res, Requisition req)
        {
            SourceOrderRepository soRepository = new SourceOrderRepository(trans);

            foreach (var sizeItem in detail.SizeList)
            {
                var woC = woHeader.WOCumulative.Where(x =>
                      x.CumulativeId == detail.Id
                      && x.HiddenSizeDes == sizeItem.Size //PFE dobut
                     && (x.Merged == false)).ToList();

                var parents = woC.Select(x => x.ParentId).Distinct().ToArray();

                List<WorkOrderCumulative> cumSorted = new List<WorkOrderCumulative>();

                if (parents.Length > 0)
                {
                    foreach (int p in parents)
                    {
                        var cumSeq = woC.Where(x => x.ParentId == p).ToList();
                        cumSeq.Reverse();
                        foreach (var elemt in cumSeq)
                        {
                            var index = cumSorted.FindIndex(a => a.SeqId == p);
                            if (index > -1)
                                cumSorted.Insert(index + 1, elemt);
                            else
                                cumSorted.Add(elemt);

                        }
                    }
                    //
                }

                var woCum = cumSorted.Where(x =>
                      x.CumulativeId == detail.Id
                      && x.HiddenSizeDes == sizeItem.Size //PFE doubt
                     && (x.Merged == false)).ToList();

                if (woCum.Count > 0)
                {
                    var CumulativeSuperOrder = woCum.FirstOrDefault(r => r.LevelInd == 0);
                    var reqDetailSuperOrder = ConvertToRequisition(CumulativeSuperOrder, woHeader, req, soRepository, Convert.ToString(CumulativeSuperOrder.SuperOrder), String.Empty, detail);
                    res.Status = soRepository.AddOrderTableManual(req, reqDetailSuperOrder);
                    if (res.Status)
                    {
                        foreach (var item in woCum.Where(r => r.LevelInd != 0).ToList())
                        {
                            var parentOrder = woCum.FirstOrDefault(g => g.SeqId == item.ParentId);
                            var reqDetail = ConvertToRequisition(item, woHeader, req, soRepository, reqDetailSuperOrder.SuperOrder, parentOrder.OrderLabel, detail);

                            res.Status = soRepository.AddOrderTableManual(req, reqDetail);

                        }// end cumulative
                    }
                    if (res.Status)
                    {
                        res.Status = soRepository.AddInsertOrderManual(req, reqDetailSuperOrder);
                        if (detail.GroupId > 0)
                        {
                            res.Status = AddInsertGroupId(reqDetailSuperOrder.OrderVersion, reqDetailSuperOrder.SuperOrder, detail.GroupId.ToString());
                            //PrevStyle = detail.SellingStyle;
                        }
                        if (res.Status && (!String.IsNullOrWhiteSpace(detail.Note)))
                        {
                            res.Status = AddNote(reqDetailSuperOrder.OrderLabel, detail.Note);
                        }
                    }
                }
                else
                {
                    res.Status = false;
                    res.ErrMsg = "Work Order Creation Failed. Please provide valid details.";
                }
            } // end multi size loop

            if (res.Status)
            {
                if (detail.ConsumedOrders != null)
                {
                    var consumData = ConsumeOrders(detail.SellingStyle, detail.ColorCode, detail.Attribute, detail.SizeList);
                    IList<WorkOrderCumulative> consumeOdr = consumData.Key;
                    //IList<WorkOrderCumulative> consumeOdr = detail.ConsumedOrders;
                    if (consumeOdr != null)
                    {
                        foreach (WorkOrderCumulative woc in consumeOdr)
                        {
                            if (woc.EditMode != null)
                            {
                                if (woc.EditMode == (int)LOVConstants.EditMode.UpdateMode)
                                {
                                    UpdateComsumedQty(woc);
                                }
                                else if (woc.EditMode == (int)LOVConstants.EditMode.DeleteMode)
                                {
                                    DeletePlannedOrder(woc);
                                }
                            }
                        }
                    }
                }
            }

            return res;
        }

        private RequisitionDetail ConvertToRequisition(WorkOrderCumulative item, WorkOrderHeader woHeader, Requisition req,
            SourceOrderRepository soRepository, String superOrder, String ParentOrder, WorkOrderDetail woDetail)
        {


            RequisitionDetail reqDetail = new RequisitionDetail();
            //WorkOrderDetail woDetail = new WorkOrderDetail();
            WorkOrderCumulative woCumParent = new WorkOrderCumulative();
            req.OrderVersion = LOVConstants.GlobalOrderVersion;
            item.OrderVersion = LOVConstants.GlobalOrderVersion;
            req.RequisitionVersion = LOVConstants.GlobalOrderVersion;

            req.CreatedBy = woHeader.CreatedBy;

            if (item.LevelInd == 0 && !String.IsNullOrEmpty(superOrder))
            {
                item.OrderLabel = superOrder;
            }
            else
            {
                item.OrderLabel = soRepository.getNewOrderLabel().ToString();

            }



            if (String.IsNullOrEmpty(superOrder) && item.LevelInd == 0)
            {
                superOrder = item.OrderLabel;
            }

            reqDetail.OrderLabel = item.OrderLabel;
            reqDetail.SuperOrder = superOrder;
            reqDetail.OrderVersion = item.OrderVersion;
            item.SuperOrder = superOrder;

            reqDetail.ParentOrder = ParentOrder;


            //reqDetail.Style = (item.PKGStyle == null) ? item.SellingStyle : item.PKGStyle;
            reqDetail.Style = item.StyleCode;
            reqDetail.Color = item.ColorCode;
            reqDetail.Attribute = item.AttributeCode;// item.AttributeCompCode;
            //reqDetail.Size = item.Size;
            reqDetail.Size = item.SizeCode;
            reqDetail.Dpr = item.RuleNo;
            //reqDetail.DCLoc = woHeader.Dc;
            if (item.LevelInd == 0)
            {
                reqDetail.DCLoc = woHeader.Dc;
                req.DcLoc = woHeader.Dc;
                reqDetail.Rev = item.Revision;
            }
            else
            {
                reqDetail.DCLoc = item.DemandLoc;
                req.DcLoc = item.DemandLoc;
                reqDetail.Rev = 0;
            }

            reqDetail.ProdFamilyCd = item.ProdFamilyCode;
            reqDetail.FabricGroup = item.FabricGroup;
            reqDetail.MakeOrBuyCode = item.MakeOrBuyCode;
            if (item.MatlTypeCode == LOVConstants.MaterialTypeCode.Fabrics)
            {
                if (!String.IsNullOrEmpty(woHeader.TxtPlant))
                {
                    reqDetail.DCLoc = woHeader.TxtPlant.ToUpper();
                    req.DcLoc = woHeader.TxtPlant.ToUpper();
                    reqDetail.MfgPathId = woHeader.TxtPlant.ToUpper();
                    req.MFGPathId = woHeader.TxtPlant.ToUpper();
                }
                if (woHeader.MachinePlant != null)
                    reqDetail.MachineTypeCode = woHeader.MachinePlant.ToUpper();
            }
            else if (item.MatlTypeCode == LOVConstants.MaterialTypeCode.CutPart)
            {
                reqDetail.DCLoc = item.CutPath;
                req.DcLoc = item.CutPath;
                reqDetail.MfgPathId = item.CutPath;
                req.MFGPathId = item.CutPath;
            }
            else
            {
                reqDetail.MfgPathId = item.MFGPathId;
                //req.MFGPathId = item.MFGPathId;
                req.MFGPathId = woDetail.SewPlt;
            }

            reqDetail.PipeLineCategoryCD = item.PipelineCategoryCode;
            reqDetail.DemandQty = item.DemandQty;
            req.OriginalDueDate = item.CurrentDueDate;
            reqDetail.CurrDueDate = item.CurrentDueDate;
            reqDetail.Qty = item.CurrentOrderQty;
            reqDetail.TotatalCurrOrderQty = item.CurrentOrderQty;
            reqDetail.MatlCd = item.MatlTypeCode;
            reqDetail.RoutinId = item.RoutingId;
            reqDetail.BillOfMATL = item.BillOfMtrlsId;
            //reqDetail.MfgPathId = item.MFGPathId;
            reqDetail.CuttingAlt = !String.IsNullOrEmpty(item.CuttingAlt) ? item.CuttingAlt.ToUpper() : item.CuttingAlt;
            reqDetail.Usage = item.Usuage;
            reqDetail.StdUsage = item.StdUsuage;
            reqDetail.StdLoss = item.StdLoss;
            reqDetail.WasteFactor = item.WasteFactor;
            reqDetail.PullFromStockInd = item.PullFromStockIndicator;
            //reqDetail.DyeCD = item.ColorDyeCode;
            reqDetail.DyeCD = item.DyeCode;
            reqDetail.DyeShadeCode = item.DyeShadeCode;
            //reqDetail.MachineTypeCode = item.MachineTypeCode;
            reqDetail.CutMethod = item.CutMethod;
            reqDetail.CylinderSize = item.CylinderSize;
            reqDetail.FinishedWidth = item.FinishedWidth;
            reqDetail.ConditionedWidth = item.ConditionedWidth;
            reqDetail.SpreadCompCode = item.SpreadCompCode;
            reqDetail.SpreadTypeCode = item.SpreadTypeCode;
            if (item.LevelInd == 0)
            {
                reqDetail.ScrapFactor = 0;
                reqDetail.PackCD = item.PackCode;
            }
            else
            {
                reqDetail.ScrapFactor = item.ScrapFactor;
                reqDetail.PackCD = "";
            }

            reqDetail.CategoryCD = item.CategoryCode;
            reqDetail.Uom = item.UnitOfMeasure;
            reqDetail.ResourceId = item.ResourceId;
            reqDetail.PlanningLeadTime = item.PlanningLeadTime;
            //reqDetail.CombinedInd = item.CombineInd;
            reqDetail.PlanDate = item.PlanDate;

            reqDetail.MachineCut = item.MachineCut;
            reqDetail.CapacityAlloc = item.CapacityAlloc;
            reqDetail.ExpeditePriority = item.ExpeditePriority.ToString();
            //reqDetail.CombinedFabInd = item.CombineFabInd;
            req.ProdStatus = LOVConstants.ProductionStatus.Locked;

            req.BusinessUnit = item.BusinessUnit;
            reqDetail.Planner = (String.IsNullOrEmpty(woHeader.PlannerCd) || woHeader.PlannerCd == "-Select-") ? "PLANN" : woHeader.PlannerCd; //Change
            reqDetail.BomSpecId = item.BomSpecId;
            reqDetail.AsrmtCd = item.AsrmtCode;
            reqDetail.CapacityAlloc = item.CapacityAlloc;
            reqDetail.OrderType = LOVConstants.WorkOrderType.WO;
            reqDetail.CombinedInd = LOVConstants.Yes;
            reqDetail.CombinedFabInd = LOVConstants.Yes;
            reqDetail.DozensOnlyInd = woDetail.DozensOnly ? LOVConstants.Yes : LOVConstants.No;
            reqDetail.CreateBDInd = woDetail.CreateBd ? LOVConstants.Yes : LOVConstants.No;
            //reqDetail.PullFromStockInd = woDetail.PullFromStock ? LOVConstants.Yes : LOVConstants.No;
            reqDetail.PullFromStockInd = item.PullFromStockIndicator;
            reqDetail.DiscreteInd = LOVConstants.DiscreteInd;
            reqDetail.Priority = LOVConstants.Priority;


            bool res = ExternalSkuValidate(woDetail.SellingStyle, woDetail.ColorCode, woDetail.Attribute, item.SizeCode);
            if (res == true)
            {
                reqDetail.DemandDriver = woDetail.DemandDriver;
                if (String.IsNullOrEmpty(woDetail.DemandSource))
                {
                    reqDetail.DemandSource = Get_Demand_Source(woDetail.PurchaseOrder, woDetail.LineItem);
                }
                else
                {
                    reqDetail.DemandSource = woDetail.DemandSource;
                }
            }
            else
            {
                reqDetail.DemandDriver = null;
                reqDetail.DemandSource = null;
            }
            UpdateRequistion(reqDetail, woHeader,req);

            return reqDetail;


        }

        //For updating Requistion object with existing values during WOM Save
        protected void UpdateRequistion(RequisitionDetail reqDetail, WorkOrderHeader woHeader, Requisition req)
        {
            req.ProdStatus = String.IsNullOrEmpty(woHeader.ProductionStatus) ? LOVConstants.ProductionStatus.Locked : ((woHeader.ProductionStatus == LOVConstants.ProductionStatus.Suggested) ? LOVConstants.ProductionStatus.TextileSuggested : woHeader.ProductionStatus);
            reqDetail.OrderType = String.IsNullOrEmpty(woHeader.OrderType) ? LOVConstants.WorkOrderType.WO : woHeader.OrderType;
            //reqDetail.DemandSource = woHeader.DemandSource;
            reqDetail.Priority = (woHeader.Priority == null) ? LOVConstants.Priority : woHeader.Priority.GetValueOrDefault();
            reqDetail.ExpeditePriority = String.IsNullOrEmpty(woHeader.ExpeditePriority) ? reqDetail.ExpeditePriority : woHeader.ExpeditePriority;
            reqDetail.CategoryCD = String.IsNullOrEmpty(woHeader.CategoryCode) ? reqDetail.CategoryCD : woHeader.CategoryCode;
            reqDetail.DozensOnlyInd = String.IsNullOrEmpty(woHeader.DozensOnlyInd) ? reqDetail.DozensOnlyInd : woHeader.DozensOnlyInd;
            reqDetail.CreateBDInd = String.IsNullOrEmpty(woHeader.CreateBDInd) ? reqDetail.CreateBDInd : woHeader.CreateBDInd;
            // reqDetail.Qty = (woHeader.FQQty == null) ? reqDetail.Qty : woHeader.FQQty;
        }

        protected bool UpdateComsumedQty(WorkOrderCumulative woCum)
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append(" Begin OPRSQL.ISS_PROD_ORDER_MAINT.UPDATE_ORDER(5,'SUPER_ORDER|ORDER_VERSION|CURR_ORDER_QTY|TOTAL_CURR_ORDER_QTY|MAKE_OR_BUY_CD','" + woCum.SuperOrder
                + "|" + LOVConstants.GlobalOrderVersion + "|" + woCum.CurrentOrderQty + "|" + woCum.CurrentOrderTotalQty + "|" +
                  LOVConstants.MakeOrBuy.Make + "');END; ");


            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            var result = (String)ExecuteScalar(queryBuilder.ToString());

            return (result == null || result == "Y") ? true : false;
        }

        protected bool ValidateDetailBeforeWOSave(WorkOrderHeader woHeader, WorkOrderCumulative woCum, out String ErrMsg, out List<dynamic> ErrList)
        {
            WorkOrderDetail woDetail = new WorkOrderDetail();
            var queryBuilder = new StringBuilder();
            ErrList = new List<dynamic>();

            woDetail = woHeader.WODetails.Single(x => x.Id == woCum.CumulativeId);

            woDetail.DozensOnlyInd = woDetail.DozensOnly ? LOVConstants.Yes : LOVConstants.No;
            woDetail.CreateBDInd = woDetail.CreateBd ? LOVConstants.Yes : LOVConstants.No;
            woDetail.AlternateId = !String.IsNullOrEmpty(woDetail.AlternateId) ? woDetail.AlternateId.ToUpper() : woDetail.AlternateId;
            woDetail.NoteInd = (woDetail.Note == "" || woDetail.Note == null) ? LOVConstants.No : LOVConstants.Yes;

            var GrmtStyle = woHeader.WOCumulative.Where(x => x.MatlTypeCode == "00").Reverse().ToList();


            queryBuilder.Append("SELECT oprsql.iss_prod_order_validate.verify(68,'SUPER_ORDER|ORDER_VERSION|ORDER_ID|PRODUCTION_STATUS|DEMAND_TYPE|DEMAND_QTY|ORIG_ORDER_QTY|ORIG_DUE_DATE|REMOTE_UPDATE_CD|REMOTE_STATUS_CD|REMOTE_RELEASE_DATE|REMOTE_XCPN_REASON|DOZENS_ONLY_IND|CREATE_BD_IND|PRIORITY|SPREAD_TYPE_CD|CORP_BUSINESS_UNIT|PLANNER_CD|DEMAND_SOURCE|DEMAND_DRIVER|RULE_NUMBER|SEW_PATH|SEW_PATH_REQSN|CUT_PATH|ATTR_PLANT|TXT_PATH|REFRESH_TXT|MACHINE|CURR_LBS|CYL_SIZES|CUT_MASTER|FINISH_MASTER|DUE_DATE_ACTIVITY|DUE_DATE|REFRESH|NOTE|NOTE_IND|REQSN_VERSION|REQSN_ID|ISS_ORDER_TYPE_CD|LW_COMPANY|LW_VENDOR_NO|LW_VENDOR_LOC|VENDOR_ID|VENDOR_SUFFIX|DEMAND_DATE|GARMENT_STYLE|DISCRETE_IND|EXPEDITE_PRIORITY|SIZE_LINE|CURR_FIN_LBS|CURR_ORDER_QTY|PACK_CD|DEMAND_LOC|CATEGORY_CD|CUTTING_ALT|STYLE_CD|COLOR_CD|ATTRIBUTE_CD|SIZE_CD|MAKE_OR_BUY_CD|GARMENT_STYLE|SELLING_STYLE_CD|MFG_REVISION_NO|SELLING_COLOR_CD|SELLING_ATTRIBUTE_CD|SELLING_SIZE_CD|DISCRETE_IND|','" +
                //SUPER_ORDER|ORDER_VERSION|ORDER_ID|PRODUCTION_STATUS|
                "1" + "|" + "1" + "|" + Val(woCum.OrderId) + "|" + Val(LOVConstants.ProductionStatus.Locked) + "|" +
                //DEMAND_TYPE|DEMAND_QTY|ORIG_ORDER_QTY|ORIG_DUE_DATE|
                Val(woHeader.Dmd) + "|" + "0" + "|" + Val("0") + "|" + (null)
                //REMOTE_UPDATE_CD|REMOTE_STATUS_CD|REMOTE_RELEASE_DATE|REMOTE_XCPN_REASON|DOZENS_ONLY_IND|CREATE_BD_IND|
                + "|" + Val(null) + "|" + Val(null) + "|" + Val(null) + "|" + Val(null) + "|" + Val(woDetail.DozensOnlyInd) + "|" + Val(woDetail.CreateBDInd) + "|"
                //PRIORITY|SPREAD_TYPE_CD|CORP_BUSINESS_UNIT|
                + ("999999999") + "|" + Val(woCum.SpreadTypeCode) + "|" + Val(woCum.BusinessUnit) + "|" +
                //PLANNER_CD|
                Val((String.IsNullOrEmpty(woHeader.PlannerCd) || woHeader.PlannerCd == "-Select-") ? "PLANN" : woHeader.PlannerCd) + "|" +
                //DEMAND_SOURCE|DEMAND_DRIVER|RULE_NUMBER|SEW_PATH|SEW_PATH_REQSN|CUT_PATH|ATTR_PLANT|
                (woDetail.DemandSource) + "|" + (woDetail.DemandDriver) + "|" + ("0") + "|" + (woDetail.MfgPathId) + "|" + Val(null) + "|" + woDetail.CutPath + "|" + Val(null) + "|"
                //TXT_PATH|REFRESH_TXT|MACHINE|CURR_LBS|
                + (woHeader.TxtPlant.ToUpper()) + "|" + (woHeader.TxtPlant.ToUpper()) + "|" + ((woHeader.MachinePlant == null) ? null : woHeader.MachinePlant.ToUpper()) + "|" + (woDetail.Lbs) + "|"
                //CYL_SIZES|CUT_MASTER|FINISH_MASTER|DUE_DATE_ACTIVITY|
                + woDetail.CylinderSizes + "|" + (null) + "|" + (null) + "|" + Val(woHeader.DueDate) + "|" +
                //DUE_DATE |
                Val((woHeader.PlannedDate!=null) ? woHeader.PlannedDate.ToString(LOVConstants.DateFormatOracle) : String.Empty) + "|"
                // REFRESH | NOTE | NOTE_IND | REQSN_VERSION | REQSN_ID | ISS_ORDER_TYPE_CD |
                + Val("N") + "|" + Val(woDetail.Note) + "|" + Val(woDetail.NoteInd) + "|" + Val("1") + "|" + Val(null) + "|" + ("SW") +
                //LW_COMPANY | LW_VENDOR_NO | LW_VENDOR_LOC | VENDOR_ID | VENDOR_SUFFIX | 
                "|" + Val("0") + "|" + Val("0") + "|" + Val(null) + "|" + Val("0") + "|" + Val("0") + "|" +
                //DEMAND_DATE | 
                Val(Val((woHeader.PlannedDate != null || woHeader.PlannedDate==DateTime.MinValue) ? woHeader.PlannedDate.ToString(LOVConstants.DateFormatOracle) : String.Empty)) + "|" +
                //GARMENT_STYLE | DISCRETE_IND | EXPEDITE_PRIORITY | SIZE_LINE | CURR_FIN_LBS | 
                ((GrmtStyle.Count > 0) ? (GrmtStyle[0].StyleCode) : null) + "|" + Val(LOVConstants.Yes) + "|" + woCum.ExpeditePriority + "|" + Val(null) + "|" + woDetail.Lbs + "|" +
                //CURR_ORDER_QTY | PACK_CD |
                woCum.CurrentOrderQty + "|" + woCum.PackCode + "|"
                // DEMAND_LOC | CATEGORY_CD | CUTTING_ALT | STYLE_CD | COLOR_CD | 
                + woHeader.Dc + "|" + woDetail.CategoryCode + "|" + woDetail.AlternateId + "|" + woCum.StyleCode + "|" + woCum.ColorCode + "|" +
                //ATTRIBUTE_CD | SIZE_CD | MAKE_OR_BUY_CD | GARMENT_STYLE | 
                woCum.AttributeCode + "|" + woCum.SizeCode + "|" + woCum.MakeOrBuyCode + "|" + ((GrmtStyle.Count>0)?(GrmtStyle[0].StyleCode):null) + "|" +
                //SELLING_STYLE_CD | MFG_REVISION_NO| SELLING_COLOR_CD|SELLING_ATTRIBUTE_CD|SELLING_SIZE_CD| DISCRETE_IND |
                woDetail.SellingStyle + "|" + woDetail.Revision + "|" + woDetail.ColorCode + "|" + woDetail.Attribute + "|" + woCum.SizeCode + "|" + Val(LOVConstants.Yes) + "') from dual ");


            //queryBuilder.Append("SELECT oprsql.iss_prod_order_validate.verify(38,'SUPER_ORDER|ORDER_VERSION|ORDER_ID|PRODUCTION_STATUS|DEMAND_TYPE|ORIG_DUE_DATE|DOZENS_ONLY_IND|CREATE_BD_IND|PRIORITY|SPREAD_TYPE_CD|CORP_BUSINESS_UNIT|SEW_PATH|SEW_PATH_REQSN|CUT_PATH|TXT_PATH|MACHINE|CURR_ORDER_TOTAL_QTY|DUE_DATE|DEMAND_LOC|DOZENS_ONLY_IND|CREATE_BD_IND|REQSN_VERSION|REQSN_ID|PLAN_DATE|SCHED_SHIP_DATE|LW_COMPANY|LW_VENDOR_NO|LW_VENDOR_LOC|VENDOR_ID|VENDOR_SUFFIX|STYLE_CD|SIZE_CD|MAKE_OR_BUY_CD|ATTRIBUTE_CD|COLOR_CD|GARMENT_STYLE|SELLING_STYLE_CD|MFG_REVISION_NO|','" + item.SuperOrder + "|" + req.OrderVersion + "|" + Val(req.RequisitionId) + "|" + Val(req.ProdStatus) + "|" + Val(req.DemandType) + "|" + ((req.OriginalDueDate.HasValue) ? req.OriginalDueDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty) + "|" + Val(item.DozensOnlyInd) + "|" + Val(item.CreateBDInd) + "|" + (item.Priority) + "|" + Val(req.SpreadTypeCD) + "|" + Val(req.BusinessUnit) + "|" + Val(req.MFGPathId) + "|" + Val(req.MFGPathId) + "||||" + (item.Qty) + "|" + ((item.CurrDueDate.HasValue) ? item.CurrDueDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty) + "|" + Val(req.DcLoc) + "|||" + item.RequisitionVer + "|" + Val(req.RequisitionId) + "|" + ((item.PlanDate.HasValue) ? item.PlanDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty) + "|" + ((item.ScheduledShipDate.HasValue) ? item.ScheduledShipDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty) + "|" + (req.LwCompany) + "|" + (req.VendorNo) + "|" + Val(req.LwVendorLoc) + "|" + (req.VendorId) + "|" + (req.VendorSuffix) + "|" + Val(item.Style) + "|" + Val(item.Size) + "|" + Val(item.MakeOrBuyCode) + "|" + Val(item.Attribute) + "|" + Val(item.Color) + "|" + Val(item.GarmentStyle) + "|" + Val(item.SellingStyle) + "|" + (item.Rev) + "') from dual ");

            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            var result = (String)ExecuteScalar(queryBuilder.ToString());
            //||||||||||||||||||||||||||||||||||||||
            ErrMsg = (result != null) ? result.Replace("|", "\n").Trim() : String.Empty;

            var list = ErrMsg.Split('\n').Where(s => !string.IsNullOrWhiteSpace(s));
            ErrMsg = string.Join("\n", list);

            if (!String.IsNullOrEmpty(ErrMsg))
            {
                woDetail.ErrorMessage = ErrMsg;
                woDetail.ErrorStatus = true;
                ErrList.Add(woDetail);
            }
            //    ErrId = (decimal)woCum.CumulativeId;
            //else
            //    ErrId = -1;

            return String.IsNullOrEmpty(ErrMsg);

        }

        //protected bool ValidateDetailBeforeWOSave(WorkOrderHeader woHeader, WorkOrderDetail woDetail, out String ErrMsg)
        //{
        //    //WorkOrderDetail woDetail = new WorkOrderDetail();
        //    var queryBuilder = new StringBuilder();

        //    //woDetail = woHeader.WODetails.Single(x => x.Id == woCum.CumulativeId);
        //    WorkOrderCumulative woCum = woHeader.WOCumulative.FirstOrDefault(x => x.CumulativeId == woDetail.Id);

        //    queryBuilder.Append("SELECT oprsql.iss_prod_order_validate.verify(65,'SUPER_ORDER|ORDER_VERSION|ORDER_ID|PRODUCTION_STATUS|DEMAND_TYPE|DEMAND_QTY|ORIG_ORDER_QTY|ORIG_DUE_DATE|REMOTE_UPDATE_CD|REMOTE_STATUS_CD|REMOTE_RELEASE_DATE|REMOTE_XCPN_REASON|DOZENS_ONLY_IND|CREATE_BD_IND|PRIORITY|SPREAD_TYPE_CD|CORP_BUSINESS_UNIT|PLANNER_CD|DEMAND_SOURCE|DEMAND_DRIVER|RULE_NUMBER|SEW_PATH|SEW_PATH_REQSN|CUT_PATH|ATTR_PLANT|TXT_PATH|REFRESH_TXT|MACHINE|CURR_LBS|CYL_SIZES|CUT_MASTER|FINISH_MASTER|DUE_DATE_ACTIVITY|DUE_DATE|REFRESH|NOTE|NOTE_IND|REQSN_VERSION|REQSN_ID|ISS_ORDER_TYPE_CD|LW_COMPANY|LW_VENDOR_NO|LW_VENDOR_LOC|VENDOR_ID|VENDOR_SUFFIX|DEMAND_DATE|GARMENT_STYLE|DISCRETE_IND|EXPEDITE_PRIORITY|SIZE_LINE|CURR_FIN_LBS|CURR_ORDER_QTY|PACK_CD|DEMAND_LOC|CATEGORY_CD|CUTTING_ALT|STYLE_CD|COLOR_CD|ATTRIBUTE_CD|SIZE_CD|MAKE_OR_BUY_CD|GARMENT_STYLE|SELLING_STYLE_CD|MFG_REVISION_NO|DISCRETE_IND|','" +
        //        Val("1") + "|" + "1" + "|" + Val(woCum.OrderId) + "|" + Val(LOVConstants.ProductionStatus.Locked) + "|" + Val(woHeader.Dmd) + "|" + "0" + "|" + Val("0") + "|" + (null) + "|" + Val(null) + "|" + Val(null) + "|" + Val(null) + "|" + Val(null) + "|" + Val(woDetail.DozensOnlyInd) + "|" + Val(woDetail.CreateBDInd) + "|" + (woDetail.Priority) + "|" + Val(null) + "|" + Val(woCum.BusinessUnit) + "|" + Val(woDetail.PlannerCd) + "|" +
        //        Val(null) + "|" + null + "|" + ("0") + "|" + (woDetail.SewPlt) + "|" + Val(null) + "|" + woDetail.CutPath + "|" + Val(null) + "|" + (woHeader.TxtPlant) + "|" + (woHeader.TxtPlant) + "|" + (woHeader.MachinePlant) + "|" + (woDetail.Lbs) + "|" + woDetail.CylinderSizes + "|" + (null) + "|" + (null) + "|" + Val(woHeader.DueDate) + "|" + Val((woCum.DueDate.HasValue) ? woCum.DueDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty) + "|" + Val("N") + "|" + Val(woDetail.Note) + "|" + Val(woDetail.NoteInd) + "|" + Val("1") + "|" + Val(null) + "|" + ("SW") +
        //        "|" + Val("0") + "|" + Val("0") + "|" + Val(null) + "|" + Val("0") + "|" + Val("0") + "|" + Val(woHeader.DueDate) + "|" + Val(woCum.GrmtStyleCode) + "|" + Val(LOVConstants.Yes) + "|" + woDetail.PriorityCode + "|" + Val(null) + "|" + woCum.Lbs + "|" + woDetail.CurrOrderQty + "|" + woDetail.PackCode + "|" + woDetail.DemandLoc + "|" + woDetail.CategoryCode + "|" + woDetail.CuttingAlt + "|" + woDetail.PKGStyle + "|" + woDetail.ColorCode + "|" + woDetail.Attribute + "|" + woDetail.Size + "|" + woCum.MakeOrBuyCode + "|" + woCum.GrmtStyleCode + "|" + woCum.SellingStyleCode + "|" + "0" + "|" + Val(LOVConstants.Yes) + "') from dual ");


        //    //queryBuilder.Append("SELECT oprsql.iss_prod_order_validate.verify(38,'SUPER_ORDER|ORDER_VERSION|ORDER_ID|PRODUCTION_STATUS|DEMAND_TYPE|ORIG_DUE_DATE|DOZENS_ONLY_IND|CREATE_BD_IND|PRIORITY|SPREAD_TYPE_CD|CORP_BUSINESS_UNIT|SEW_PATH|SEW_PATH_REQSN|CUT_PATH|TXT_PATH|MACHINE|CURR_ORDER_TOTAL_QTY|DUE_DATE|DEMAND_LOC|DOZENS_ONLY_IND|CREATE_BD_IND|REQSN_VERSION|REQSN_ID|PLAN_DATE|SCHED_SHIP_DATE|LW_COMPANY|LW_VENDOR_NO|LW_VENDOR_LOC|VENDOR_ID|VENDOR_SUFFIX|STYLE_CD|SIZE_CD|MAKE_OR_BUY_CD|ATTRIBUTE_CD|COLOR_CD|GARMENT_STYLE|SELLING_STYLE_CD|MFG_REVISION_NO|','" + item.SuperOrder + "|" + req.OrderVersion + "|" + Val(req.RequisitionId) + "|" + Val(req.ProdStatus) + "|" + Val(req.DemandType) + "|" + ((req.OriginalDueDate.HasValue) ? req.OriginalDueDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty) + "|" + Val(item.DozensOnlyInd) + "|" + Val(item.CreateBDInd) + "|" + (item.Priority) + "|" + Val(req.SpreadTypeCD) + "|" + Val(req.BusinessUnit) + "|" + Val(req.MFGPathId) + "|" + Val(req.MFGPathId) + "||||" + (item.Qty) + "|" + ((item.CurrDueDate.HasValue) ? item.CurrDueDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty) + "|" + Val(req.DcLoc) + "|||" + item.RequisitionVer + "|" + Val(req.RequisitionId) + "|" + ((item.PlanDate.HasValue) ? item.PlanDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty) + "|" + ((item.ScheduledShipDate.HasValue) ? item.ScheduledShipDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty) + "|" + (req.LwCompany) + "|" + (req.VendorNo) + "|" + Val(req.LwVendorLoc) + "|" + (req.VendorId) + "|" + (req.VendorSuffix) + "|" + Val(item.Style) + "|" + Val(item.Size) + "|" + Val(item.MakeOrBuyCode) + "|" + Val(item.Attribute) + "|" + Val(item.Color) + "|" + Val(item.GarmentStyle) + "|" + Val(item.SellingStyle) + "|" + (item.Rev) + "') from dual ");

        //    System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
        //    var result = (String)ExecuteScalar(queryBuilder.ToString());
        //    //||||||||||||||||||||||||||||||||||||||
        //    ErrMsg = (result != null) ? result.Replace("|", String.Empty).Trim() : String.Empty;

        //    return String.IsNullOrEmpty(ErrMsg);

        //}

        public bool AddInsertGroupId(decimal OrderVersion, String SuperOrder, String GroupId)
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append(" BEGIN OPRSQL.TM_ISS_PROD_ORDER_GROUP.TABLE_INSERT(4,'ORDER_VERSION|SUPER_ORDER|ISS_GROUP_TYPE_CD|ISS_GROUP_ID','"
                + OrderVersion
                + "|" + SuperOrder + "|" + LOVConstants.ISSGroupType.CutMaster + "|" + GroupId
                + "');END; ");

            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            var result = (String)ExecuteScalar(queryBuilder.ToString());

            return (result == null || result == "Y") ? true : false;

        }

        public bool ValidateGroupID(decimal groupId)
        {
            decimal? result = null;
            try
            {
                if (groupId != 0)
                {
                    var queryBuilder = new StringBuilder();
                    queryBuilder.Append("Select count(ISS_GROUP_ID)  from iss_prod_order_group where ISS_GROUP_ID = '" + groupId + "'");

                    System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
                    result = (decimal)ExecuteScalar(queryBuilder.ToString());
                }
            }
            catch (Exception ee)
            {

            }
            return (result != null && result >= 1) ? true : false;
        }

        public bool ValidateTextile(WorkOrderHeader woHeader)
        {
            decimal? result = null;
            if (woHeader.TxtPlant != null)
            {
                var queryBuilder = new StringBuilder();
                queryBuilder.Append("Select count(1) from Plant where plant_cd = '" + Val(woHeader.TxtPlant.ToUpper()) + "' and Finishing_Ind = 'Y' ");

                System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
                result = (decimal)ExecuteScalar(queryBuilder.ToString());
            }
            return (result != null && result > 0) ? true : false;
        }

        private KeyValuePair<List<WorkOrderCumulative>, string> ConsumeOrders(string sellingStyle, string sellingColor, string sellingAttribute, List<MultiSKUSizes> multiSizes)
        {
            decimal qty = 0;
            string warMssg = "";
            bool hasOrders = false;
            var plannedOrders = ReadPlannedOrders(sellingStyle, sellingColor, sellingAttribute);
            List<WorkOrderCumulative> consumedList = new List<WorkOrderCumulative>();

            foreach (MultiSKUSizes size in multiSizes)
            {
                hasOrders = false;
                qty = size.Qty * 12;
                if (qty > 0)
                {
                    var orders = plannedOrders.Where(x => (x.SizeCode == size.SizeCD)).ToList();
                    //decimal orderSum = orders.Sum(x => x.CurrentOrderQty);


                    if (orders.Count > 0)
                    {
                        foreach (WorkOrderCumulative woc in orders)
                        {
                            if (qty > 0)
                            {
                                hasOrders = true;
                                if (woc.CurrentOrderQty <= qty)
                                {
                                    qty = qty - woc.CurrentOrderQty;
                                    woc.EditMode = (int)LOVConstants.EditMode.DeleteMode;
                                }
                                else
                                {
                                    woc.EditMode = (int)LOVConstants.EditMode.UpdateMode;
                                    woc.CurrentOrderQty = woc.CurrentOrderQty - qty;
                                    qty = 0;
                                }
                                consumedList.Add(woc);
                            }
                        }
                    }

                    if (!hasOrders)
                    {
                        warMssg = "Order: " + sellingStyle + " " + sellingColor + " " + sellingAttribute + " " + size.Size + " did not have suggested orders to consume. ";
                    }

                }
            }

            return new KeyValuePair<List<WorkOrderCumulative>, string>(consumedList, warMssg);
        }

        private IList<WorkOrderCumulative> ReadPlannedOrders(string sellingStyle, string sellingColor, string sellingAttribute)
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append("select order_version \"OrderVersion\",super_order \"SuperOrder\",style_cd \"StyleCd\",color_cd \"ColorCode\",attribute_cd \"AttributeCode\",size_cd \"SizeCode\",selling_style_cd \"SellingStyleCode\",selling_color_cd \"SellingColorCode\",v.total_curr_order_qty \"CurrentOrderTotalQty\",curr_order_qty  \"CurrentOrderQty\", v.priority \"ExpeditePriority\", v.rule_number \"RuleNo\",v.discrete_ind,v.scrap_factor \"ScrapFactor\" " +
                " from iss_prod_order_view v where order_version = 1 and production_status = 'P' and make_or_buy_cd in ('M') and selling_style_cd = '" + Val(sellingStyle) +
                "' and selling_color_cd =  '" + Val(sellingColor) + "' and selling_attribute_cd = '" + Val(sellingAttribute) +
                "'  order by selling_style_cd,selling_color_cd,selling_attribute_cd,selling_size_cd,EARLIEST_START,priority,total_curr_order_qty ");

            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            IDataReader reader = ExecuteReader(queryBuilder.ToString());

            var result = (new DbHelper()).ReadData<WorkOrderCumulative>(reader);
            return result;
        }

        public void RecomputeDueDates(WorkOrderDetail woDetail)
        {
            string DemandLoc = "", prevDemandLoc = "", activityCd = "", lastDc = "";
            decimal prodTime = 0;
            decimal LeadTime = 0;
            DateTime dtPlanDate = new DateTime();
            if (woDetail.DueDate == null || woDetail.DueDate == "")
            {
                woDetail.DueDate = LOVConstants.WOMDueDates.DC;
            }

            DateTime dtDueDate = woDetail.PlannedDate;

            if (woDetail.DueDate == LOVConstants.WOMDueDates.BD || woDetail.DueDate == LOVConstants.PipeLineCategoryCode.DBF)
            {
                dtDueDate = MoveToNextDay(dtDueDate, (int)DayOfWeek.Monday);
            }
            else
            {
                dtDueDate = MoveToNextDay(dtDueDate, (int)DayOfWeek.Friday);
            }

            var prodCategoryList = Enum.GetValues(typeof(ProductionCategories))
                                    .Cast<ProductionCategories>()
                                    .Where(x => x != ProductionCategories.KNT)
                                    .OrderByDescending(x => x)
                                    .ToList();


            if (woDetail.DueDate == LOVConstants.WOMDueDates.BD || woDetail.DueDate == LOVConstants.PipeLineCategoryCode.DBF)
            {
                foreach (ProductionCategories prodCat in prodCategoryList)
                {
                    activityCd = prodCat.ToString();

                    var categoryList = woDetail.WOCumulative.Where(x => x.PipelineCategoryCode == activityCd && (!x.Merged)).ToList();
                    for (int i = 0; i < categoryList.Count; i++)
                    {
                        WorkOrderCumulative item = categoryList[i];

                        if(!item.Merged)
                        {
                            DemandLoc = item.MFGPlant;
                            lastDc = prevDemandLoc;

                            if (i == 0)
                            {
                                item.BackSchedule = 0;

                                if (prevDemandLoc != "")
                                {
                                    if (prevDemandLoc == DemandLoc)
                                    {
                                        prodTime = 0;
                                        if (item.RoutingId != "")
                                        {
                                            prodTime = ProductionTime(item.RoutingId);
                                        }

                                        if (prodTime > 0)
                                        {
                                            dtDueDate = dtDueDate.AddDays(1);
                                        }
                                    }
                                    item.MFGPlant = prevDemandLoc;
                                }

                                item.DueDate = dtDueDate;
                                SetDueDate(item);
                                LeadTime = item.PlanningLeadTime;
                                dtPlanDate = item.PlanDate;
                                dtDueDate = item.CurrentDueDate;
                            }
                            else
                            {
                                item.CurrentDueDate = dtDueDate;
                                item.PlanningLeadTime = LeadTime;
                                item.PlanDate = dtPlanDate;
                            }

                            item.MFGPlant = DemandLoc;
                            prevDemandLoc = DemandLoc;
                        }
                    }
                }
            }
            else
            {
                string supplyPlant = "", tempStyle = "";
                decimal prevProdTime = 0;
                prodTime = 0;

                foreach (ProductionCategories prodCat in prodCategoryList)
                {
                    activityCd = prodCat.ToString();

                    var categoryList = woDetail.WOCumulative.Where(x => x.PipelineCategoryCode == activityCd).ToList();

                    for (int i = 0; i < categoryList.Count; i++)
                    {
                        WorkOrderCumulative item = categoryList[i];

                        if (!item.Merged)
                        {
                            if (item.PlanDate == DateTime.MinValue)
                            {
                                item.PlanDate = woDetail.PlannedDate;
                            }

                            if (i == 0)
                            {
                                supplyPlant = item.MFGPlant;
                                DemandLoc = item.DemandLoc;
                                lastDc = supplyPlant;
                                tempStyle = item.StyleCode;
                                item.BackSchedule = 1;
                                prodTime = 0;
                                if (item.RoutingId != "")
                                {
                                    prodTime = ProductionTime(item.RoutingId);
                                }

                                item.DueDate = dtDueDate;
                                LeadTime = item.PlanningLeadTime;
                                dtPlanDate = item.PlanDate;
                                dtDueDate = item.CurrentDueDate;

                                if (prevDemandLoc != "")
                                {
                                    if (prevProdTime > 0)
                                    {
                                        dtPlanDate = dtPlanDate.AddDays(-1);
                                        dtPlanDate = PreviousWorkDay(dtPlanDate, item.MFGPlant);
                                        LeadTime = (dtDueDate - dtPlanDate).Days;
                                        item.PlanDate = dtPlanDate;
                                        item.PlanningLeadTime = LeadTime;
                                    }
                                }

                                item.MFGPlant = lastDc;
                                prevDemandLoc = supplyPlant;
                                prevProdTime = prodTime;
                            }
                            else
                            {
                                item.CurrentDueDate = dtDueDate;
                                item.PlanningLeadTime = LeadTime;
                                item.PlanDate = dtPlanDate;
                            }
                        }
                    }
                    dtDueDate = dtPlanDate;
                }
            }
        }

        protected DateTime PreviousWorkDay(DateTime planDate, string plant)
        {
            DateTime dtNewDate = planDate;

            int aday = (int)dtNewDate.DayOfWeek;

            if (aday == (int)DayOfWeek.Saturday)
            {
                dtNewDate = dtNewDate.AddDays(-1);
            }
            else if (aday == (int)DayOfWeek.Sunday)
            {
                dtNewDate = dtNewDate.AddDays(-2);
            }

            return dtNewDate;

        }

        public DateTime MoveToNextDay(DateTime planDate, int planDay)
        {
            DateTime dtNewDate = planDate;

            int aday = (int)dtNewDate.DayOfWeek;

            while (aday != planDay)
            {
                dtNewDate = dtNewDate.AddDays(1);
                aday = (int)dtNewDate.DayOfWeek;
            }

            return dtNewDate;

        }

        public bool CheckInvalidDates(WorkOrderHeader woHeader, out List<dynamic> ErrList)
        {
            bool isInvalidDate = false;
            ErrList = new List<dynamic>();
            for (int k = 0; k < woHeader.WODetails.Count; k++)
            {
                WorkOrderDetail woDetail = woHeader.WODetails[k];
                var woCum = woHeader.WOCumulative.Where(x => (!x.Merged) && (x.CumulativeId == woDetail.Id)).ToList();
                for (int i = 0; i < woCum.Count; i++)
                {
                    if (!woCum[i].Merged)
                    {
                        WorkOrderCumulative item = woCum[i];

                        //if (item.BomSpecId != null && item.BomSpecId != "" && item.MakeOrBuyCode != LOVConstants.MakeOrBuy.Buy)
                        if (item.BillOfMtrlsId != null && item.BillOfMtrlsId != "" && item.MakeOrBuyCode != LOVConstants.MakeOrBuy.Buy )
                        {
                            if (!woDetail.DozensOnly && item.PlanDate < DateTime.Now.Date.AddDays(-60))
                            {
                                isInvalidDate = true;
                                //ErrId = item.CumulativeId;

                                woDetail.ErrorStatus = true;
                                woDetail.ErrorMessage = "Invalid Start Date. Date cannot be more than 60 days past due.";
                                ErrList.Add(woDetail);
                                //break;
                            }
                        }
                    }
                }
            }
            return isInvalidDate;
        }

        public bool ValidateQuantity(WorkOrderHeader woHeader, out List<dynamic> ErrList)
        {
            bool isInvalid = false;
            ErrList = new List<dynamic>();
            for (int k = 0; k < woHeader.WODetails.Count; k++)
            {
                WorkOrderDetail woDetail = woHeader.WODetails[k];

                var sizelst = woDetail.SizeList;

                for (int g = 0; g < sizelst.Count; g++)
                {
                    MultiSKUSizes msz = sizelst[g];
                    if (msz.Qty <= 0)
                    {
                        isInvalid = true;

                        woDetail.ErrorStatus = true;
                        woDetail.ErrorMessage = "Invalid Quantity. Quantity must be greater than zero.";
                        ErrList.Add(woDetail);
                    }
                    else if (msz.Qty >= 300000.0m)
                    {
                        isInvalid = true;

                        woDetail.ErrorMessage = "Quantity must be less than 300000.";
                        woDetail.ErrorStatus = true;
                        ErrList.Add(woDetail);
                    }
                }

            }
            return isInvalid;
        }

        public KeyValuePair<bool, String> ValidateGroup(WorkOrderHeader woHeader, out List<dynamic> ErrList)
        {

            string strKey = "";
            string strSku = "-1";
            bool Status = true;
            string Msg = "";
            ErrList = new List<dynamic>();

            var woCum = woHeader.WOCumulative.Where(x => (!x.Merged)).ToList();

            for (int i = 0; i < woHeader.WODetails.Count; i++)
            {
                WorkOrderDetail woDetail = woHeader.WODetails[i];
                if (woDetail.GroupId > 0)
                {
                    woDetail.CuttingAlt = !String.IsNullOrEmpty(woDetail.CuttingAlt) ? woDetail.CuttingAlt.ToUpper() : woDetail.CuttingAlt;
                    strKey = woDetail.SellingStyle + woDetail.ColorCode + woDetail.Attribute + woDetail.SewPlt + woDetail.CutPath + woDetail.MfgPathId + woDetail.CuttingAlt + woHeader.PlannedDate.ToString();

                    if (strSku == "-1")
                        strSku = strKey;

                    if (strSku != strKey)
                        strSku = "";
                }
            }

            if (strSku == "")
            {
                Msg = "Multi Sku order must match by selling sku, revision, sew cut txt plants , alternate and due date.";
                Status = false;
            }


            string keyfab = ""; strKey = "";
            int count = 0;
            //int eId = -1;
            List<dynamic> ErrIds = new List<dynamic>();
            if (woHeader.WOFabric != null)
            {

                woHeader.WODetails.ToList().ForEach(detail =>
                {
                    if (detail.GroupId > 0)
                    {
                        var flag = true;
                        var fablist = woHeader.WOFabric.Where(x => (x.Id == detail.Id) && (!x.Merged)).ToList();

                        for (int k = 0; k < detail.SizeList.Count; k++)
                        {
                            if (!flag) break;
                            var Current = fablist.Where(e => e.SizeShortDes == detail.SizeList[k].Size).ToList();
                            if (Current.Count>0)
                            {
                                var currentComponent = woHeader.WOCumulative.Where(s => (s.MatlTypeCode == LOVConstants.MATL_TYPE_CD.Garment) && (s.PipelineCategoryCode != "PKG") && (s.HiddenSizeDes == detail.SizeList[k].Size)).Select(s => s.StyleCode).ToList();
                                var CompareList=detail.SizeList.Where(w => w.Size != detail.SizeList[k].Size).ToList();
                                var otherComponent = woHeader.WOCumulative.Where(s => (s.MatlTypeCode == LOVConstants.MATL_TYPE_CD.Garment) && (s.PipelineCategoryCode != "PKG") && (s.HiddenSizeDes != detail.SizeList[k].Size)).Select(s => s.StyleCode).ToList();
                                for (int m = 0; m < currentComponent.Count; m++)
                                {
                                    for (int l = 0; l < otherComponent.Count; l++)
                                    {
                                        //Current item Componnent find
                                        if (currentComponent[m] == otherComponent[l])
                                        {
                                            for (int y = 0; y < CompareList.Count; y++)
                                            {
                                                // component style eqal then only 
                                                var Compare = fablist.Where(e => e.SizeShortDes == CompareList[y].Size).ToList();
                                                Current.ForEach(currentItem =>
                                                {
                                                    var keyCurrent = currentItem.Fabric + currentItem.DyeCode + currentItem.CylSize + currentItem.SpreadCode;

                                                    //If component style from size OO (in this example) equals component style from size OS THEN
                                                    //Add garmentselmethod over here


                                                    //if (Compare.Any(tr => tr.Fabric == currentItem.Fabric))
                                                    //{

                                                    // check fabric component same as current compoent
                                                    if (!Compare.Any(t => (t.Fabric + t.DyeCode + t.CylSize + t.SpreadCode).Equals(keyCurrent)))
                                                    {
                                                        flag = false;
                                                        Status = false;
                                                        //eId = detail.Id;

                                                        //Msg = "Size:  " + currentItem.SizeShortDes
                                                        //    + " has different fabrics requirements than other sizes [" + detail.SizeList[y].Size + "]";
                                                        Msg = "Size:  " + CompareList[y].Size
                                                           + " has different fabrics requirements than other sizes";
                                                        detail.ErrorStatus = true;
                                                        detail.ErrorMessage = Msg;
                                                        ErrIds.Add(detail);
                                                    }

                                                    // }

                                                });
                                                if (!flag) break;
                                            } // End compare other sizes
                                        }
                                    }
                                }
                            }
                            if (!flag) break;
                        }// end detail Size
                    }
                });

                if (ErrIds.Count > 0)
                {
                    ErrList = ErrIds;
                }


                //  string keyfab = ""; strKey = "";
                //int count = 0;
                //woHeader.WODetails.ToList().ForEach(detail =>
                //            {
                //                if (detail.GroupId > 0)
                //                {
                //                    var fablist = woHeader.WOFabric.Where(x => (x.Id == detail.Id) && (!x.Merged)).ToList();
                //                    for (int i = 0; i < fablist.Count; i++)
                //                    {
                //                        WorkOrderFabric fabz = fablist[i];
                //                        strKey = fabz.Fabric + fabz.DyeCode + fabz.CylSize + fabz.SpreadCode;
                //                        var fabComp = woHeader.WOFabric.Where(x => (x.SizeShortDes != fabz.SizeShortDes) && (!x.Merged) && (x.Id == detail.Id)).ToList();
                //                        count = 0;
                //                        if (fabComp.Count > 0)
                //                        {
                //                            for (int j = 0; j < fabComp.Count; j++)
                //                            {
                //                                WorkOrderFabric fab = fabComp[j];
                //                                keyfab = fab.Fabric + fab.DyeCode + fab.CylSize + fab.SpreadCode;

                //                                if (strKey == keyfab)
                //                                {
                //                                    count++;
                //                                    break;
                //                                }
                //                            }


                //                            if (count == 0)
                //                            {
                //                                Status = false;
                //                                Msg = "Size:  " + fabz.SizeShortDes + " has different fabrics requirements than other sizes";
                //                                //break;
                //                            }
                //                        }
                //                    }
                //                }
                //            });

                /*
                for (int i = 0; i < woHeader.WOFabric.Count; i++)
                {
                    if (!woHeader.WOFabric[i].Merged)
                    {
                        WorkOrderFabric fabz = woHeader.WOFabric[i];
                        strKey = fabz.Fabric + fabz.DyeCode + fabz.CylSize + fabz.SpreadCode;
                        var fabComp = woHeader.WOFabric.Where(x => (x.SizeShortDes != fabz.SizeShortDes) && (!x.Merged)).ToList();
                        count = 0;
                        if (fabComp.Count > 0)
                        {
                            for (int j = 0; j < fabComp.Count; j++)
                            {
                                WorkOrderFabric fab = fabComp[j];
                                keyfab = fab.Fabric + fab.DyeCode + fab.CylSize + fab.SpreadCode;

                                if (strKey == keyfab)
                                {
                                    count++;
                                    break;
                                }
                            }


                            if (count == 0)
                            {
                                Status = false;
                                Msg = "Size:  " + fabz.SizeShortDes + " has different fabrics requirements than other sizes";
                                //break;
                            }
                        }
                    }
                }
                 */
            }

            /*
             Status = true;//TBD
            woHeader.WODetails.Where(e=> e.GroupId>0).ToList().ForEach(WOItem => {
                var fabList = woHeader.WOFabric.Where(e => e.Id == WOItem.Id && (!e.Merged)); // All fabric list except Merged
                var SizeGruops=fabList.GroupBy(g => g.SizeShortDes);// distinct sizes
                var FirstSize = SizeGruops.FirstOrDefault();
                SizeGruops = SizeGruops.Skip(1);

               var KeySet=GetFabricGroupedSet(FirstSize);

                SizeGruops.ToList().ForEach(item =>
                    {
                        var KeySubSet = GetFabricGroupedSet(item);
                        if (KeySet.Count != KeySubSet.Count)
                        {
                            Status = false;
                            return ;
                        }
                        foreach (var s in KeySet)
                        {
                            if (!KeySubSet.Contains(s))
                            {
                                Status = false;
                                Msg = "Size:  " + item.SizeShortDes + " has different fabrics requirements than other sizes";
                                return;
                            }
                        }

                         OR
                        if (KeySet.Except(KeySubSet).Any())
                        {
                            Status = false;
                            return;
                        }
                        if (KeySubSet.Except(KeySet).Any())
                        {
                            Status = false;
                            return;
                        }
                    });

            });
           */

            //var fabrics = woHeader.WOCumulative.Where(x => (!x.Merged) && (x.MatlTypeCode == "F")).ToList();
            //string keyfab = ""; strKey = "";
            //int count = 0;
            //for (int i = 0; i < woCum.Count; i++)
            //{
            //    if (woCum[i].MatlTypeCode == "F" && woCum[i].CurrentOrderQty > 0)
            //    {
            //        WorkOrderCumulative item = woCum[i];
            //        strKey = item.StyleCode + item.ColorDyeCode + item.CylinderSize + item.SpreadTypeCode;
            //        count = 0;
            //        for (int j = 0; j < fabrics.Count; j++)
            //        {
            //            WorkOrderCumulative fab = fabrics[j];
            //            keyfab = fab.StyleCode + fab.ColorDyeCode + fab.CylinderSize + fab.SpreadTypeCode;

            //            if (strKey == keyfab)
            //            {
            //                count++;
            //                //break;
            //            }
            //        }


            //        if (count == 0)
            //        {
            //            Status = false;
            //            Msg = "Size:  " + item.HiddenSizeDes + " has different fabrics requirements than other sizes";
            //            //break;
            //        }
            //    }
            //}




            return new KeyValuePair<bool, string>(Status, Msg);
        }


        public HashSet<String> GetFabricGroupedSet(IGrouping<String, WorkOrderFabric> sizeList)
        {

            var KeyList = sizeList.GroupBy(fab => fab.Fabric + fab.DyeCode + fab.CylSize + fab.SpreadCode).Select(r => r.Key);
            return new HashSet<String>(KeyList);
        }

        public decimal ProductionTime(string routingId)
        {
            decimal res = 0;
            var queryBuilder = new StringBuilder();
            queryBuilder.Append("select sum(planning_leadtime) from activity_routing r, planning_leadtime p where r.routing_id = \'" + Val(routingId) + "\' And r.activity_cd = p.activity_cd and r.location_cd = p.location_cd");
            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            var resu = ExecuteScalar(queryBuilder.ToString());
            var result = Decimal.TryParse(Convert.ToString(resu), out  res);
            return res;
        }

        public void SetDueDate(WorkOrderCumulative woCum)
        {
            string strDueDate = "";
            Int32 backSchedule = 0;
            string routingId = "";
            string DC = "";
            string plant = "";
            DateTime dueDate = new DateTime();
            decimal leadTime = 0;
            if ((woCum.BackSchedule == 1))
            {
                backSchedule = 1;
            }
            else
            {
                backSchedule = 0;
            }
            leadTime = 0;
            dueDate = woCum.DueDate.Value;
            strDueDate = woCum.DueDate.Value.ToString("yyyyMMdd");
            routingId = woCum.RoutingId;
            DC = woCum.DemandLoc;
            plant = woCum.MFGPlant;
            //DC = "KM";

            if (routingId != null && DC != null && plant != null)
            {
                var queryBuilder = new StringBuilder();
                queryBuilder.Append("SELECT OPRSQL.LTS_UTIL.RoutingLeadTime('" + Val(plant) + "','" + Val(DC) + "','" + Val(routingId) + "','" + Val(strDueDate) + "','" + backSchedule + "') from dual");

                leadTime = (Decimal)ExecuteScalar(queryBuilder.ToString());
            }
            else
            {
                woCum.PlanningLeadTime = GetPlanningLeadTime(woCum.StyleCode, woCum.ColorCode, woCum.AttributeCode, woCum.SizeCode, woCum.MFGPathId);
            }

            if (woCum.MakeOrBuyCode == LOVConstants.MakeOrBuy.Buy)
            {
                leadTime = woCum.PlanningLeadTime;

                if (woCum.MatlTypeCode != "00")
                {
                    leadTime = 0;
                }
            }

            woCum.PlanningLeadTime = leadTime;

            if (backSchedule == 1)
            {
                woCum.CurrentDueDate = dueDate;
            }
            else
            {
                woCum.CurrentDueDate = dueDate.AddDays((double)leadTime);
            }

            dueDate = woCum.CurrentDueDate;

            woCum.PlanDate = dueDate.AddDays(-(double)leadTime);


        }

        public List<string> SplitNote(string[] comment)
        {
            List<string> note = new List<string>();

            if (comment != null && comment.Length > 0)
            {
                foreach (string s in comment)
                {
                    if (s.Length > 50)
                    {
                        var pcmt = s.SplitByLength(50);
                        if (pcmt.ToArray().Length > 0)
                        {
                            note.AddRange(pcmt);
                        }
                    }
                    else
                    {
                        note.Add(s);
                    }

                }
            }

            return note;
        }

        public bool AddNote(string superOrder, string note)
        {
            try
            {
                int noteCount = 0;
                var noteArray = note.Split('\n');
                string strNote = (noteArray != null) ? String.Join("|", SplitNote(noteArray)) : string.Empty;
                //note = note.Replace('\n', '|');
                noteCount = strNote.Count(x => x == '|') + 1;

                var queryBuilder = new StringBuilder();
                queryBuilder.Append(" BEGIN OPRSQL.TM_ISS_PROD_ORDER_NOTE.ADD_NOTE(" + noteCount.ToString() + ",1,'" + Val(superOrder) + "','PL','" + Val(note) + "');END; ");

                System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
                var result = (String)ExecuteScalar(queryBuilder.ToString());

                return (result == null || result == "Y") ? true : false;
            }
            catch (Exception en)
            {
                Log(en);
            }
            return true;// Note table field length isue causes rollback.This is temporary
        }

        public string GetNote(string superOrder)
        {
            string note = "";
            var queryBuilder = new StringBuilder();
            queryBuilder.Append("select order_note \"Comments\" from iss_prod_order_note ");
            queryBuilder.Append("where ORDER_LABEL = \'" + Val(superOrder.ToUpper()) + "\' and order_version = " + LOVConstants.GlobalOrderVersion + " and iss_note_type_cd  = \'PL\' ");
            queryBuilder.Append("order by note_seq_no");

            IDataReader reader = ExecuteReader(queryBuilder.ToString());

            var result = (new DbHelper()).ReadData<CommentNotes>(reader);
            if (result.Count > 0)
            {
                note = String.Join("\n", result.Select(x => x.Comments).ToList());
            }

            return note;
        }

        public void RecomputeDueDatesForSave(WorkOrderHeader woHeader)
        {
            woHeader.WODetails.ForEach(detail =>
            {
                string DemandLoc = "", prevDemandLoc = "", activityCd = "", lastDc = "";
                decimal prodTime = 0;
                decimal LeadTime = 0;
                //Azalia Soriano Ci 308369 4/27/2021
                //Start
                decimal prevProdTime = 0;
                decimal prevLeadTime = 0;
                //End
                DateTime dtPlanDate = new DateTime();
                if (woHeader.DueDate == null || woHeader.DueDate == "")
                {
                    woHeader.DueDate = LOVConstants.WOMDueDates.DC;
                }

                DateTime dtDueDate = woHeader.PlannedDate;

                if (woHeader.DueDate == LOVConstants.WOMDueDates.BD || woHeader.DueDate == LOVConstants.PipeLineCategoryCode.DBF)
                {
                    dtDueDate = MoveToNextDay(dtDueDate, (int)DayOfWeek.Monday);
                }
                else
                {
                    dtDueDate = MoveToNextDay(dtDueDate, (int)DayOfWeek.Friday);
                }

                var prodCategoryList = Enum.GetValues(typeof(ProductionCategories))
                                        .Cast<ProductionCategories>()
                                        .Where(x => x != ProductionCategories.KNT)
                                        .OrderByDescending(x => x)
                                        .ToList();

                //prodCategoryList.Remove(ProductionCategories.KNT);
                if (woHeader.WOCumulative != null)
                {
                    if (woHeader.DueDate == LOVConstants.WOMDueDates.BD || woHeader.DueDate == LOVConstants.PipeLineCategoryCode.DBF)
                    {
                        //Azalia Soriano CI 308369 4/27/2021
                        prevProdTime = 0;
                        prevLeadTime = 0;

                        foreach (ProductionCategories prodCat in prodCategoryList)
                        {
                            activityCd = prodCat.ToString();
                            
                            var categoryList = woHeader.WOCumulative.Where(x => x.PipelineCategoryCode == activityCd && (!x.Merged) && (x.CumulativeId == detail.Id)).ToList();
                            //var categoryList = woCum.Where(x => x.PipelineCategoryCode == activityCd).ToList();
                            for (int i = 0; i < categoryList.Count; i++)
                            {
                                WorkOrderCumulative item = categoryList[i];

                                if (!item.Merged)
                                {

                                    if (item.MatlTypeCode == LOVConstants.MaterialTypeCode.Fabrics)
                                    {
                                        if (!String.IsNullOrEmpty(woHeader.TxtPlant))
                                        {
                                            item.DemandLoc = woHeader.TxtPlant.ToUpper();
                                            item.MFGPlant = woHeader.TxtPlant.ToUpper();
                                            //item.MFGPathId = woHeader.TxtPlant.ToUpper();
                                        }
                                    }
                                    else if (item.MatlTypeCode == LOVConstants.MaterialTypeCode.CutPart)
                                    {
                                        if (!String.IsNullOrEmpty(item.CutPath))
                                        {
                                            item.DemandLoc = item.CutPath.ToUpper();
                                            item.MFGPlant = item.CutPath.ToUpper();
                                            //item.MFGPathId = item.CutPath;
                                        }
                                    }

                                    DemandLoc = item.MFGPlant;
                                    lastDc = prevDemandLoc;

                                    if (i == 0)
                                    {
                                        item.BackSchedule = 0;

                                        if (prevDemandLoc != "")
                                        {
                                            if (prevDemandLoc == DemandLoc)
                                            {
                                                prodTime = 0;
                                                if (item.RoutingId != "")
                                                {
                                                    prodTime = ProductionTime(item.RoutingId);
                                                }

                                                //Azalia Sorian CI 308369 4/27/2021
                                                //if (prodTime > 0)
                                                if (prodTime > 0 && (prevProdTime > 0 || prevLeadTime > 0))
                                                {
                                                    dtDueDate = dtDueDate.AddDays(1);
                                                }
                                            }
                                            item.MFGPlant = prevDemandLoc;
                                            //Azalia Sorian CI 308369 4/27/2021
                                            prevProdTime = 0;
                                            prevLeadTime = 0;
                                        }

                                        item.DueDate = dtDueDate;
                                        SetDueDate(item);
                                        LeadTime = item.PlanningLeadTime;
                                        dtPlanDate = item.PlanDate;
                                        dtDueDate = item.CurrentDueDate;
                                    }
                                    else
                                    {
                                        item.CurrentDueDate = dtDueDate;
                                        item.PlanningLeadTime = LeadTime;
                                        item.PlanDate = dtPlanDate;
                                    }

                                    item.MFGPlant = DemandLoc;
                                    prevDemandLoc = DemandLoc;
                                    //Azalia Soriano CI 308369 4/27/2021
                                    if (prodTime > prevProdTime)
                                    {
                                        prevProdTime = prodTime;
                                    }
                                    //Azalia Soriano CI 308369 5/11/2021
                                    if (LeadTime > prevLeadTime)
                                    {
                                        prevLeadTime = LeadTime;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        string supplyPlant = "", tempStyle = "";
                        //Azalia Soriano Ci 308369 4/27/2021
                        //decimal prevProdTime = 0;
                        prevProdTime = 0;
                        prodTime = 0;
                        int j = 0;
                        string lastActivity;

                        prodCategoryList.Reverse();

                        lastActivity = prodCategoryList.ToArray().Last().ToString();

                        var woCum = woHeader.WOCumulative.Where(x => !x.Merged && (x.CumulativeId == detail.Id)).ToList();

                        foreach (ProductionCategories prodCat in prodCategoryList)
                        {
                            activityCd = prodCat.ToString();

                            var categoryList = woCum.Where(x => x.PipelineCategoryCode == activityCd).ToList();
                            if (categoryList.Count > 0)
                            {
                                for (int i = 0; i < categoryList.Count; i++)
                                {
                                    WorkOrderCumulative item = categoryList[i];
                                    WorkOrderCumulative item2 = null;

                                    if (!item.Merged)
                                    {
                                        if (i == 0)
                                        {
                                            if (item.MatlTypeCode == LOVConstants.MaterialTypeCode.Fabrics)
                                            {
                                                if (!String.IsNullOrEmpty(woHeader.TxtPlant))
                                                {
                                                    item.DemandLoc = woHeader.TxtPlant.ToUpper();
                                                    item.MFGPlant = woHeader.TxtPlant.ToUpper();
                                                }
                                            }

                                            supplyPlant = item.MFGPlant;
                                            DemandLoc = item.DemandLoc;
                                            lastDc = supplyPlant;

                                            tempStyle = item.StyleCode;

                                            item.BackSchedule = 1;

                                            if (i < woCum.Count)
                                            {
                                                if (activityCd != lastActivity)
                                                {
                                                    j = j + 1;
                                                    if (j < woCum.Count)
                                                    {
                                                        item2 = woCum[j];
                                                        if (item2 != null)
                                                        {
                                                            if (item2.MatlTypeCode == LOVConstants.MaterialTypeCode.Fabrics)
                                                            {
                                                                if (!String.IsNullOrEmpty(woHeader.TxtPlant))
                                                                {
                                                                    item2.DemandLoc = woHeader.TxtPlant.ToUpper();
                                                                }
                                                            }

                                                            supplyPlant = item2.DemandLoc;
                                                            item.MFGPlant = supplyPlant;
                                                        }
                                                    }
                                                }

                                            }

                                            prodTime = 0;
                                            if (!string.IsNullOrEmpty(item.RoutingId))
                                            {
                                                prodTime = ProductionTime(item.RoutingId);
                                            }

                                            item.DueDate = dtDueDate;
                                            SetDueDate(item);
                                            LeadTime = item.PlanningLeadTime;
                                            dtPlanDate = item.PlanDate;
                                            dtDueDate = item.CurrentDueDate;

                                            if (prevDemandLoc != "")
                                            {
                                                if (prevProdTime > 0 && item2 != null)
                                                {
                                                    dtPlanDate = dtPlanDate.AddDays(-1);
                                                    dtPlanDate = PreviousWorkDay(dtPlanDate, item.MFGPlant);
                                                    LeadTime = (dtDueDate - dtPlanDate).Days;
                                                    item.PlanDate = dtPlanDate;
                                                    item.PlanningLeadTime = LeadTime;
                                                }
                                            }

                                            item.MFGPlant = lastDc;
                                            prevDemandLoc = supplyPlant;
                                            prevProdTime = prodTime;
                                        }
                                        else
                                        {
                                            item.CurrentDueDate = dtDueDate;
                                            item.PlanningLeadTime = LeadTime;
                                            item.PlanDate = dtPlanDate;
                                        }
                                    }

                                    j++;
                                }
                                dtDueDate = dtPlanDate;
                            }
                        }
                    }
                }
            });
        }

        public bool DeletePlannedOrder(WorkOrderCumulative woCum)
        {

            var queryBuilder = new StringBuilder();
            queryBuilder.Append(" BEGIN OPRSQL.ISS_PROD_ORDER_MAINT.DELETE_ORDER(2,'SUPER_ORDER|ORDER_VERSION','" + woCum.SuperOrder + "|"
                + LOVConstants.GlobalOrderVersion + "');END; ");

            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            var result = (String)ExecuteScalar(queryBuilder.ToString());

            return (result == null || result == "Y") ? true : false;
        }

        public List<WorkOrderCumulative> GetCuttingAltId(SKU sku)
        {
            string query = "SELECT distinct CUTTING_ALT  \"CuttingAlt\", BILL_OF_MTRLS_ID  \"BillOfMtrlsId\",'54' \"MFGPathId\" FROM BILL_OF_MTRLS A WHERE A.PARENT_STYLE = '" + Val(sku.Style + 'A') + "' AND A.PARENT_COLOR = '" + Val(sku.Color) + "' AND A.PARENT_ATTRIBUTE = '" + Val(sku.Attribute) + "'  and a.bill_of_mtrls_id is not null";


            IDataReader reader = ExecuteReader(query);

            var result = (new DbHelper()).ReadData<WorkOrderCumulative>(reader);
            return result;

        }

        public List<decimal> GetBulkGroupID(decimal dgridCount)
        {

            List<decimal> lstGroupId = new List<decimal>();

            for (decimal d = 0; d < dgridCount; d++)
            {
                var query = new StringBuilder();
                query.Append("select ISS_PROD_ORDER_SEQ.NEXTVAL FROM DUAL");
                var result = (decimal)ExecuteScalar(query.ToString());
                lstGroupId.Add(result);
            }

            return lstGroupId;

        }

        public decimal GetPlanningLeadTime(string style, string color, string attribute, string size, string mfgPathId)
        {
            decimal? leadTime = 0;
            if (style != null && color != null && attribute != null && size != null && mfgPathId != null)
            {
                var query = new StringBuilder();
                query.Append("select nvl(b.lead_time,0)*7");
                query.Append(" from MFG_PATH  a , MFG_PATH_CHP b ,style s, prod_family p where");
                query.Append(" a.style_cd = '" + Val(style) + "'");
                query.Append(" and a.color_cd = '" + Val(color) + "'");
                query.Append(" and a.attribute_cd = '" + Val(attribute) + "'");
                query.Append(" and a.size_cd = '" + Val(size) + "'");
                query.Append(" and a.mfg_path_id = '" + Val(mfgPathId) + "'");
                query.Append(" and a.effect_end_date >=sysdate and a.style_cd = b.style_cd (+)  AND a.color_cd = b.color_cd (+) AND a.attribute_cd = b.attribute_cd (+)  AND a.size_cd  = b.size_cd (+)");
                query.Append(" AND a.mfg_path_id = b.mfg_path_id (+)  AND a.style_cd  = s.style_cd (+) and s.corp_prod_family_cd = p.prod_family_cd (+) order by a.mfg_path_id, a.prime_mfg_location");
                leadTime = (decimal?)ExecuteScalar(query.ToString());

            }
            //return (leadTime.Value == null) ? 0 : leadTime.Value.RoundCustom(0);
            return (leadTime.HasValue) ? leadTime.Value.RoundCustom(0) : 0;

        }
        //WOM Insert
        public Result InsertWOMOrderDetails(WorkOrderHeader woHeader, WOMDetail womDet)
        {
            Result res = new Result();
            bool Status = false;

            RecomputeDueDatesForSave(woHeader);
            res = ValidateDetailBeforeWOMSave(woHeader, womDet);
            if (res.Status)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        BeginTransaction();
                        SourceOrderRepository soRep = new SourceOrderRepository(trans);
                        Status = soRep.DeleteOrder(woHeader, womDet.SuperOrder);
                        if (Status)
                        {
                            res.Status = Status;
                            Requisition req = new Requisition();
                            req.OrderVersion = LOVConstants.GlobalOrderVersion;
                            req.DemandType = woHeader.Dmd;
                            req.CurrentDueDate = woHeader.PlannedDate;
                            req.DemandDate = woHeader.PlannedDate;
                            woHeader.OrdersToCreate = 1;

                            woHeader.WODetails.ToList().ForEach(detail =>
                            {
                                if (res.Status)
                                {
                                    res = WorkOrderDetail(detail, woHeader, res, req);
                                }
                            });
                            if (womDet.IsUngrouped)
                            {
                                if (!WOMOrderDeleteGroupId(womDet.OrderVersion, womDet.SuperOrder))
                                {
                                    res.Status = false;
                                    res.ErrMsg = "Failed to ungroup order.";
                                }
                            }
                            if (res.Status)
                                CommitTransaction();
                            else
                                RollbackTransaction();
                        }
                    }
                    catch (Exception ES)
                    {
                        res.Status = false;
                        RollbackTransaction();
                        Log("Save WOM-trans scope");
                        Log(ES);
                        res.ErrMsg = ES.Message;
                    }
                }
            }
            else
            {
                womDet.ErrorStatus = true;
                womDet.ErrorMessage = res.ErrMsg;
            }
            return res;
        }
        protected Result ValidateDetailBeforeWOMSave(WorkOrderHeader woHeader, WOMDetail womDet)
        {

            bool Status = true;
            Result res = new Result();
            decimal ErrId = -1;
            List<dynamic> ErrList = new List<dynamic>();
            List<dynamic> ErrDtls = new List<dynamic>();
            var Msg = String.Empty;
            KeyValuePair<bool, string> statusMsg = new KeyValuePair<bool, string>();
            //woHeader.MachinePlant = "R3";
            if (!ValidateTextile(woHeader))
            {
                res.ErrMsg = "Textile Plant is Invalid.  The Finishing Indicator must be set to a 'Y' in the APS " +
                        "Plant table. It is currently set to an 'N'.  Either select a valid Textile Plant " +
                        "or contact the Cost Dept if you believe the plant you selected is a valid Textile Plant.";
                Status = false;
            }
            else if (woHeader.CreateBDInd == "Y")
            {
                if (woHeader.MachinePlant == null || woHeader.MachinePlant == "")
                {
                    res.ErrMsg = "Machine Type cannot be a blank value.";
                    Status = false;
                }
            }

            else if (woHeader.WODetails.Count <= 0)
            {
                res.ErrMsg = "At least one order must be ceated.";
                Status = false;
            }
            else if (CheckInvalidDates(woHeader, out ErrList))
            {
                res.ErrMsg = "Invalid Start Date. Date cannot be more than 60 days past due.";
                Status = false;
                res.Id = ErrId;
                if (ErrList.Count > 0)
                    ErrDtls.AddRange(ErrList);
            }
            else if (!ValidateRevision(woHeader, womDet, out ErrList))
            {
                res.ErrMsg = "Invalid Revision.";
                Status = false;
                res.Id = ErrId;
                if (ErrList.Count > 0)
                    ErrDtls.AddRange(ErrList);
            }
            else
            {

                statusMsg = ValidateGroup(woHeader, out ErrList);
                if (statusMsg.Key != null)
                {
                    Status = statusMsg.Key;
                    res.Id = ErrId;
                }

                if (statusMsg.Value != null)
                {
                    res.ErrMsg = statusMsg.Value;
                    if (ErrList.Count > 0)
                    {
                        ErrDtls.AddRange(ErrList);
                    }
                }
            }
            res.Status = Status;
            return res;
        }

        public bool WOMOrderDeleteGroupId(decimal OrderVersion, String SuperOrder)
        {
            //BEGIN OPRSQL.TM_ISS_PROD_ORDER_GROUP.TABLE_DELETE(3,'ORDER_VERSION|SUPER_ORDER|ISS_GROUP_TYPE_CD','1|E000578479|M|');END;
            String query = "BEGIN OPRSQL.TM_ISS_PROD_ORDER_GROUP.TABLE_DELETE(3,'SUPER_ORDER|ORDER_VERSION|ISS_GROUP_TYPE_CD','" + SuperOrder + "|" + OrderVersion + "|" + LOVConstants.ISSGroupType.CutMaster
                + "');END;";

            var result = (String)ExecuteScalar(query);

            return (result == null || result == "Y") ? true : false;
        }


        public List<WorkOrderDetail> GetOrderDetailByOrderLabel(string SuperOrder)
        {
            StringBuilder query = new StringBuilder(string.Empty);

            query.Append("select  a.dozens_only_ind  \"DozensOnlyInd\", a.create_bd_ind  \"CreateBDInd\", a.selling_style_cd  \"SellingStyle\", a.style_cd  \"PKGStyle\", ");
            query.Append("a.color_cd  \"ColorCode\", a.attribute_cd  \"Attribute\", a.size_cd  \"Size\", i.Size_short_Desc  \"SizeShortDes\", a.mfg_revision_no  \"Revision\", ");
            query.Append("a.pack_cd  \"PackCode\", a.category_cd  \"CategoryCode\", a.priority, a.expedite_priority  \"PriorityCode\", a.cutting_alt  \"AlternateId\", ");
            query.Append("a.cut_path  \"CutPath\", nvl(a.sew_plant,' ')  \"SewPlt\", a.sew_path, a.MFG_PATH_ID  \"MfgPathId\" ");
            query.Append("from iss_prod_order_view a, item_size i ");
            query.Append("where ORDER_VERSION = " + LOVConstants.GlobalOrderVersion + " and SUPER_ORDER = '" + SuperOrder + "' and a.size_cd = i.size_cd");

            //query.Append(" select STYLE_CD PKGStyle, COLOR_CD ColorCode,ATTRIBUTE_CD Attribute,SIZE_CD \"Size\",MFG_PATH_ID MfgPathId, SELLING_STYLE_CD SellingStyle, MFG_REVISION_NO Revision, PACK_CD PackCode, CUTTING_ALT AlternateId  " +
            //        " FROM iss_prod_order_detail WHERE ORDER_VERSION = " + LOVConstants.GlobalOrderVersion + " and ORDER_LABEL = '" + SuperOrder + "'");
            IDataReader reader = ExecuteReader(query.ToString());

            var result = (new DbHelper()).ReadData<WorkOrderDetail>(reader);


            return result;

        }

        public bool ValidateRevisionDetails(string style, string color, string attribute, List<MultiSKUSizes> size, string asrtCode, decimal revision)
        {
            var query = new StringBuilder();
            query.Append(" select distinct s.revision_no \"NewRevision\", s.revision_desc \"RevDescription\" ");
            query.Append("from  sku_revision s ");
            //if (asrtCode == "A")
            //    query.Append("from mfg_sell_asrmt_sku_xref_view m,  sku_revision s ");
            //else
            //    query.Append("from mfg_selling_sku_xref m,   sku_revision s");
            //query.Append(" where    m.mfg_style_cd = s.style_cd and m.mfg_color_cd = s.color_cd ");
            //query.Append(" and m.mfg_attribute_cd = s.attribute_cd and m.mfg_size_cd = s.size_cd");
            query.Append(" where s.style_cd like '" + Val(style) + "'");
            query.Append(" and s.color_cd like '" + Val(color) + "'");
            query.Append(" and s.attribute_cd like '" + Val(attribute) + "'");
            query.Append(" and  s.size_cd in (" + SizeList(size) + ")");
            //if (asrtCode == "A")
            //    query.Append("and end_date_ind <> 'Y'");
            //else
            //    query.Append(" and DC_RECEIVE_IND = 'Y'");
            query.Append(" and  s.revision_no = " + revision + "");


            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<WorkOrderDetail>(reader);

            return (result != null && result.Count > 0) ? true : false;

        }

        public bool ValidateRevision(WorkOrderHeader woHeader, WOMDetail womDet, out List<dynamic> ErrList)
        {
            bool isValidRev = true;
            ErrList = new List<dynamic>();
            for (int k = 0; k < woHeader.WODetails.Count; k++)
            {
                WorkOrderDetail detail = woHeader.WODetails[k];
                //if (!ValidateRevisionDetails(detail.SellingStyle, detail.ColorCode, detail.Attribute, detail.SizeList, detail.AssortCode, detail.Revision))
                if (!ValidateRevisionDetails(womDet.SellingStyle, womDet.SellingColor, womDet.SellingAttribute, detail.SizeList, detail.AssortCode, detail.Revision))
                {
                    isValidRev = false;

                    detail.ErrorStatus = true;
                    detail.ErrorMessage = "Invalid Revision.";
                    ErrList.Add(detail);
                }
            }

            return isValidRev;
        }

        public bool ValidateRevisionReq(WorkOrderHeader woHeader, out List<dynamic> ErrList)
        {
            bool isValidRev = true;
            ErrList = new List<dynamic>();
            for (int k = 0; k < woHeader.WODetails.Count; k++)
            {
                WorkOrderDetail detail = woHeader.WODetails[k];
                if (!ValidateRevisionDetails(detail.SellingStyle, detail.ColorCode, detail.Attribute, detail.SizeList, detail.AssortCode, detail.Revision))
                {
                    isValidRev = false;

                    detail.ErrorStatus = true;
                    detail.ErrorMessage = "Invalid Revision.";
                    ErrList.Add(detail);
                }
            }

            return isValidRev;
        }

        public WorkOrderDetail GetGarmentSKU(string style, string color, string attribute, List<MultiSKUSizes> size, string mfgPathId)
        {
            WorkOrderDetail objWO = new WorkOrderDetail();
            var query = new StringBuilder();
            query.Append("select  bm.comp_style_cd \"GStyle\" , bm.comp_color_cd \"GColor\", bm.comp_attribute_cd \"GAttribute\", bm.comp_size_cd \"GSize\", bm.activity_cd \"ActivityCode\"  ");
            query.Append("FROM BILL_OF_MTRLS bm, mfg_path mp where");
            query.Append(" mp.style_cd = '" + Val(style) + "'");
            query.Append(" AND mp.color_cd = '" + Val(color) + "'");
            query.Append(" AND mp.attribute_cd = '" + Val(attribute) + "'");
            query.Append(" and mp.size_cd in (" + SizeList(size) + ")");
            query.Append(" AND mp.mfg_path_id = '" + Val(mfgPathId) + "'");
            query.Append(" and mp.style_cd = bm.parent_style and mp.color_cd = bm.parent_color and mp.attribute_cd = bm.parent_attribute ");
            query.Append(" and mp.size_cd = bm.parent_size and mp.bill_of_mtrls_id = bm.bill_of_mtrls_id and bm.activity_cd='PUL'  ");
            /*and s.item_type_cd = 'GR'*/
            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<WorkOrderDetail>(reader);
            if (result.Count > 0)
            {
                objWO = result[0];
            }
            return objWO;
        }

        public bool ExternalSkuValidate(string style, string color, string attribute, string size)
        {
            var str_array = size.Split(',');
            for (var i = 0; i < str_array.Length; i++)
            {
                string query = "select COUNT(*) from EXTERNAL_SKU_MASTER where selling_style_cd='" + Val(style) + "' and selling_color_cd = '" + Val(color) + "' and selling_attribute_cd = '" + Val(attribute) + "' and selling_size_cd = '" + Val(str_array[i]) + "'  and customer_cd='HAA'";
                var skucount = ExecuteScalar(query);
                if (Convert.ToInt32(skucount) > 0)
                {
                    return true;
                }
                else
                {
                    string Query1 = "select COUNT(*) from Codes_Table where category = 'ECD' and CODE=(select CORP_DIVISION_CD from Style where STYLE_CD='" + Val(style) + "')";
                    var corp_division_count = ExecuteScalar(Query1);
                    if (Convert.ToInt32(corp_division_count) > 0)
                    {
                        string Query2 = "select Count(*) from  external_sku_xref where STYLE_CD='" + Val(style) + "' AND COLOR_CD='" + Val(color) + "' AND ATTRIBUTE_CD='" + Val(attribute) + "' AND SIZE_CD='" + Val(str_array[i]) + "'";
                        var Count_Xref = ExecuteScalar(Query2);
                        if (Convert.ToInt32(Count_Xref) > 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }


        public string Get_Demand_Source(string PONumber, string Line_Number)
        {
            var queryBuilder = new StringBuilder();
            //queryBuilder.Append("select OPRSQL.external_data_pkg.concatnate_demand('PO'" + ",\'" + "HAA" + "\',\'" + PONumber + "\',\'" + Line_Number + "\') from dual ");
            queryBuilder.Append("SELECT SUBSTR ( oprsql.external_data_pkg.concatnate_demand ('PO'" + ",\'" + "HAA" + "\',\'" + PONumber + "\',\'" + Line_Number + "\') ,1, LENGTH ( oprsql.external_data_pkg.concatnate_demand ('PO'" + ",\'" + "HAA" + "\',\'" + PONumber + "\',\'" + Line_Number + "\') )-1) from dual ");

            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            var result = (string)ExecuteScalar(queryBuilder.ToString());
            return result;

        }
    }
}
