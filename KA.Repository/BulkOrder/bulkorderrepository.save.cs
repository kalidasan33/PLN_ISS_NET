using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISS.DAL;
using ISS.Common;
using System.Data;
using System.Data.Common;
using KA.Core.Model.BulkOrder;
using System.Transactions;
using ISS.Core.Model.Common;
using ISS.Repository.Order;

namespace KA.Repository.BulkOrder
{
    public partial class BulkOrderRepository : RepositoryBase
    {



        public KeyValuePair<bool, String> UpdateBulkOrder(BulkOrderDetail req, List<BulkOrderDetail> items)
        {
            String Msg = "Bulk order fields are missing.";
            bool Status = false;
            var bilkorderList = GetBulkOrderDetail(req.BulkNumber, req.ProgramSource, false);
            if (bilkorderList.Count == 0)
            {
                Msg = "Invalid bulk order number.";
                return new KeyValuePair<bool, string>(Status, Msg);
            }
            if (items.Count(e => !e.IsDeleted) > 0)
            {

                //items = items.Where(e => e.IsDirty).ToList();
                if (req != null)
                {

                    if (items != null && items.Count() > 0)
                    {
                        try
                        {
                            var currBulk = bilkorderList.FirstOrDefault();
                            // var Currreq =(List<RequisitionDetail>) GetRequisitionDetail(req.RequisitionId);
                            req.RequisitionVersion = currBulk.RequisitionVersion; // hard coded for insert
                            //req.DemandType = LOVConstants.DemandType;
                            //req.CurrentDueDate = req.DemandDate = req.OriginalDueDate = req.DCDueDate = req.PlannedDcDate;
                            req.ProdStatus = currBulk.ProdStatus;  //LOVConstants.ProductionStatus.Locked;
                            req.ReqStatus = currBulk.ReqStatus; //LOVConstants.RequestStatus.UnderConstruction;
                            req.OrderType = currBulk.OrderType;
                            String ErrMsgValDet = String.Empty;
                            SourceOrderRepository reqRepository = new SourceOrderRepository();
                            foreach (var item in items)
                            {
                                if (item.isHide) continue;
                                if (item.IsDeleted) { Status = true; continue; }
                                if (item.ProcessedToOS == LOVConstants.BulkOrderStatus.Processed) continue;
                                SKU sku = new SKU();
                                sku.Style = item.Style;
                                sku.Color = item.Color;
                                sku.Attribute = item.Attribute;
                                sku.Size = item.Size;
                                sku.Rev = Convert.ToDecimal(item.Rev);
                                sku.Qty = item.Qty;

                                Status = false;

                                if (item.Qty <= 0) { Msg = "SKU " + sku.getSKUString() + ". Quantity must be greater than zero."; break; }
                                else if (item.Qty >= 300000.0m) { Msg = "SKU " + sku.getSKUString() + ". Quantity must be less than 300000."; break; }
                                else if (!reqRepository.VerifyDCMfgpath(sku, req.DcLoc)) { Msg = "SKU " + sku.getSKUString() + " and demand location must exist in PATH_DEST_PLANT."; break; }
                                else if (!reqRepository.VerifyDCMfgpath(sku, req.MFGPathId)) { Msg = "Invalid Style " + sku.Style + " and  Sew Plant " + req.MFGPathId; break; }
                                //else if (!reqRepository.VerifyVendor(sku, req.LwCompany, req.VendorNo, req.LwVendorLoc)) { Msg = "Invalid vendor details SKU " + sku.getSKUString(); break; }
                                else if (!reqRepository.VerifySKUDCCombination(sku, req.MFGPathId)) { Msg = "This is not a valid SKU/DC combination. " + sku.getSKUString(); break; }
                                else if (!reqRepository.VerifyRevision(sku, req.MFGPathId)) { Msg = " - Revision Code " + sku.Rev + " is not valid for SKU/DC combination. " + sku.getSKUString() + " - " + req.DcLoc; ; break; }
                                else if (!reqRepository.validateStyleBeforeSOSave(item.Style)) { Msg = "Invalid style " + sku.Style; break; }

                                else if (!reqRepository.GetStyleValidation(sku.Style, req.BusinessUnit)) { Msg = "Invalid Business Unit. " + sku.getSKUString(); break; }
                                else if (!reqRepository.GetDCValidation(req.DcLoc)) { Msg = "Invalid Dc Loc. " + sku.getSKUString(); break; }
                                //TBD BU and Style **
                                // dc validatio
                                else
                                {
                                    //ISS.Core.Model.Order.Requisition r = null;
                                    //ISS.Core.Model.Order.RequisitionDetail rItem= null;
                                    //getRequisition(req, item,out r,out rItem);
                                    //rItem.SuperOrder = "xxx12345";//validation only
                                    //if (!reqRepository.ValidateDetailBeforeSOSave(r, rItem, out ErrMsgValDet))
                                    //{
                                    //    Msg = ErrMsgValDet + " - Style " + sku.Style; break;
                                    //}
                                    //else
                                    //{
                                    Status = true;
                                    //}
                                }
                            }
                            if (Status)
                            {
                                using (TransactionScope scope = new TransactionScope())
                                {
                                    try
                                    {
                                        BeginTransaction();
                                        foreach (var item in items)
                                        {
                                            item.RequisitionId = req.RequisitionId;
                                            item.RequisitionVersion = req.RequisitionVersion;
                                            item.BulkNumber = req.BulkNumber;
                                            item.OrderType = req.OrderType;  // LOVConstants.ISSOrderTypeCode.NonSummarizedRequisition;
                                            item.TotatalCurrOrderQty = item.Qty = item.Qty.ConvertDzToEaches();

                                            if (item.ProcessedToOS == LOVConstants.BulkOrderStatus.Processed) continue;

                                            if (item.IsDeleted)
                                            {
                                                if (item.ProcessedToOS != LOVConstants.BulkOrderStatus.Processed)
                                                    Status = DeleteLineItem(item);

                                            }
                                            //else if (item.IsMovedObject)
                                            //{
                                            //   // Status = InsertOrder(req, item);
                                            //}
                                            else if (item.IsInserted)
                                            {
                                                item.ProcessedToOS = LOVConstants.BulkOrderStatus.Pending;
                                                Status = UpdateBulkOrderItem(req, item);
                                                if (Status)
                                                {
                                                    Status = DeleteLineItemError(item);
                                                }
                                            }
                                            else
                                            {
                                                item.ProcessedToOS = LOVConstants.BulkOrderStatus.Pending;
                                                //TBD generate Line Item Nbr

                                                Status = InsertBulkOrderItem(req, item);
                                            }

                                            if (!Status)
                                            {
                                                Status = false;
                                                // Msg = "Falied to insert order detail SKU " + sku.getSKUString();
                                                break;
                                            }
                                        } //end for loop
                                        if (Status)
                                        {
                                            CommitTransaction();
                                        }
                                        if (Status)
                                        {
                                            Msg = "Bulk Order updated successfully.";
                                        }

                                    }
                                    catch (Exception ES)
                                    {
                                        RollbackTransaction();
                                        Log("Save bulk order-trans scope");
                                        Log(ES);
                                        Msg = ES.Message;
                                    }
                                }// END TRANS SCOPE
                            }// validation completed and save starts end
                        }
                        catch (Exception ee)
                        {
                            Log(ee);
                            Msg = ee.Message;
                        }
                        finally
                        {

                        }
                    }
                    else
                    {
                        Msg = "No detail to update.";
                    }
                }
            }
            else
            {
                Msg = "Please enter at least one bulk order detail.";
            }

            return new KeyValuePair<bool, string>(Status, Msg);
        }


        public bool DeleteBulkOrder(String bulkNo)
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append(" BEGIN OPRSQL.KA_PREPROCESSOR_PKG.delete_ka_bulk (1,'KA_BULK_NBR','" + bulkNo + "');END; ");

            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            var result = (String)ExecuteScalar(queryBuilder.ToString());

            return (result == null || result == "Y") ? true : false;
        }

        public bool DeleteLineItem(BulkOrderDetail item)
        {

            var queryBuilder = new StringBuilder();
            queryBuilder.Append(" BEGIN OPRSQL.KA_PREPROCESSOR_PKG.delete_ka_bulk_Item(2,'KA_BULK_NBR|KA_LINE_NBR','" + item.BulkNumber + "|" + item.LineNumber + "');END; ");

            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            var result = (String)ExecuteScalar(queryBuilder.ToString());

            return (result == null || result == "Y") ? true : false;
        }

        public bool DeleteLineItemError(BulkOrderDetail item)
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append(" BEGIN OPRSQL.KA_PREPROCESSOR_PKG.delete_ka_bulk_errors(2,'KA_BULK_NBR|KA_LINE_NBR','" + item.BulkNumber + "|" + item.LineNumber + "');END; ");

            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            var result = (String)ExecuteScalar(queryBuilder.ToString());
            return (result == null || result == "Y") ? true : false;
        }


        protected bool UpdateBulkOrderItem(BulkOrderDetail req, BulkOrderDetail item)
        {

            //KA_BULK_NBR|KA_LINE_NBR|STYLE_CD|COLOR_CD|ATTRIBUTE_CD|SIZE_CD|REQSN_VERSION|REQSN_ID|APPRV_RESPONSE_DATE|REQSN_CREATE_DATE|VENDOR_ID|VENDOR_SUFFIX|CONTACT_PLANNER_CD|ISS_ORDER_TYPE_CD|REQSN_STATUS_CD|PRODUCTION_STATUS|SRC_CONTACT_CD|REQSN_APPROVER_CD|DEMAND_LOC|CURR_DUE_DATE|SEASON_YEAR|SEASON_NAME|TRANSP_MODE_CD|OVER_PCT|UNDER_PCT|CORP_BUSINESS_UNIT|LW_COMPANY|LW_VENDOR_NO|LW_VENDOR_LOC|ISS_PROGRAM_TYPE_CD|DETAIL_TRKG_IND|CREATE_DATE|MFG_PATH_ID|MFG_REVISION_NO|UNIT_OF_MEASURE|CURR_ORDER_QTY|PLANT_CD|PROCESSED_TO_OS_1

            var SeasonYear = String.Empty;
            var SeasonName = String.Empty;
            if (!string.IsNullOrEmpty(req.Season))
            {
                var arr = req.Season.Split('^');
                if (arr.Length > 1)
                {
                    SeasonYear = arr[0];
                    SeasonName = arr[1];
                }
            }

            item.Uom = "DZ";    // Updated by Cijith on 10/28 to defalt the UOM to DZ. Please take it out and give it in the correct place since i am not familor with the code.

            var queryBuilder = new StringBuilder();
            queryBuilder.Append(" Begin OPRSQL.KA_PREPROCESSOR_PKG.update_ka_bulk (38,'KA_BULK_NBR|KA_LINE_NBR|STYLE_CD|COLOR_CD|ATTRIBUTE_CD|SIZE_CD|REQSN_VERSION|REQSN_ID|APPRV_RESPONSE_DATE|REQSN_CREATE_DATE|VENDOR_ID|VENDOR_SUFFIX|CONTACT_PLANNER_CD|ISS_ORDER_TYPE_CD|REQSN_STATUS_CD|PRODUCTION_STATUS|SRC_CONTACT_CD|REQSN_APPROVER_CD|DEMAND_LOC|CURR_DUE_DATE|SEASON_YEAR|SEASON_NAME|TRANSP_MODE_CD|OVER_PCT|UNDER_PCT|CORP_BUSINESS_UNIT|LW_COMPANY|LW_VENDOR_NO|LW_VENDOR_LOC|ISS_PROGRAM_TYPE_CD|DETAIL_TRKG_IND|CREATE_DATE|MFG_PATH_ID|MFG_REVISION_NO|UNIT_OF_MEASURE|CURR_ORDER_QTY|PLANT_CD|PROCESSED_TO_OS','" + req.BulkNumber
                //KA_LINE_NBR|STYLE_CD|COLOR_CD|ATTRIBUTE_CD|SIZE_CD
                + "|" + item.LineNumber + "|" + Val(item.Style) + "|" + Val(item.Color)
                + "|" + Val(item.Attribute) + "|" + Val(item.Size) + "|"

                //REQSN_VERSION|REQSN_ID|APPRV_RESPONSE_DATE|REQSN_CREATE_DATE|VENDOR_ID
                 + req.RequisitionVersion + "|" + req.RequisitionId + "|" + ((req.ApproverResponseDate.HasValue) ? req.ApproverResponseDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty) + "|"
                  + ((req.ReqCreateDate.HasValue) ? req.ReqCreateDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty)
                  + "|" + (req.VendorId) + "|"

                //VENDOR_SUFFIX|CONTACT_PLANNER_CD|ISS_ORDER_TYPE_CD|REQSN_STATUS_CD|PRODUCTION_STATUS|SRC_CONTACT_CD
                 + req.VendorSuffix + "|" + Val(req.PlanningContact) + "|" + Val(req.OrderType) + "|"
                  + req.ReqStatus + "|" + Val(req.ProdStatus) + "|" + Val(req.SourcingContact) + "|"

                //REQSN_APPROVER_CD|DEMAND_LOC|CURR_DUE_DATE|SEASON_YEAR|SEASON_NAME|TRANSP_MODE_CD
                 + req.RequisitionApprover + "|" + Val(req.DcLoc) + "|" + ((item.CurrDueDate.HasValue) ? item.CurrDueDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty) + "|"
                  + SeasonYear + "|" + Val(SeasonName) + "|" + Val(req.TranspMode) + "|"

                //OVER_PCT|UNDER_PCT|CORP_BUSINESS_UNIT|LW_COMPANY|LW_VENDOR_NO|LW_VENDOR_LOC
                 + ((req.OverPercentage.HasValue) ? (req.OverPercentage.Value / 100.0M) : 0) + "|"
                 + ((req.UnderPercentage.HasValue) ? (req.UnderPercentage.Value / 100.0M) : 0) + "|" + Val(req.BusinessUnit) + "|"
                  + req.LwCompany + "|" + (req.VendorNo) + "|" + Val(req.LwVendorLoc) + "|"

                //ISS_PROGRAM_TYPE_CD|DETAIL_TRKG_IND|CREATE_DATE|MFG_PATH_ID|MFG_REVISION_NO
                + req.ProType + "|" + ((req.DetailTrkgInd) ? LOVConstants.Yes : LOVConstants.No)
                + "|"
                + ((req.CreatedOn.HasValue) ? req.CreatedOn.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty) + "|"
                  + req.MFGPathId + "|" + (item.Rev) + "|"

                //UNIT_OF_MEASURE|CURR_ORDER_QTY|PLANT_CD|PROCESSED_TO_OS_1
                 + item.Uom + "|" + (item.Qty) + "||" + item.ProcessedToOS
                + "');END; ");

            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            var result = (String)ExecuteScalar(queryBuilder.ToString());

            return (result == null || result == "Y") ? true : false;

        }



        protected bool InsertBulkOrderItem(BulkOrderDetail req, BulkOrderDetail item)
        {

            //KA_BULK_NBR|KA_LINE_NBR|STYLE_CD|COLOR_CD|ATTRIBUTE_CD|SIZE_CD|REQSN_VERSION|REQSN_ID|APPRV_RESPONSE_DATE|REQSN_CREATE_DATE|VENDOR_ID|VENDOR_SUFFIX|CONTACT_PLANNER_CD|ISS_ORDER_TYPE_CD|REQSN_STATUS_CD|PRODUCTION_STATUS|SRC_CONTACT_CD|REQSN_APPROVER_CD|DEMAND_LOC|CURR_DUE_DATE|SEASON_YEAR|SEASON_NAME|TRANSP_MODE_CD|OVER_PCT|UNDER_PCT|CORP_BUSINESS_UNIT|LW_COMPANY|LW_VENDOR_NO|LW_VENDOR_LOC|ISS_PROGRAM_TYPE_CD|DETAIL_TRKG_IND|CREATE_DATE|MFG_PATH_ID|MFG_REVISION_NO|UNIT_OF_MEASURE|CURR_ORDER_QTY|PLANT_CD|PROCESSED_TO_OS_1

            var SeasonYear = String.Empty;
            var SeasonName = String.Empty;
            if (!string.IsNullOrEmpty(req.Season))
            {
                var arr = req.Season.Split('^');
                if (arr.Length > 1)
                {
                    SeasonYear = arr[0];
                    SeasonName = arr[1];
                }
            }

            var queryBuilder = new StringBuilder();
            queryBuilder.Append(" Begin OPRSQL.KA_PREPROCESSOR_PKG.insert_ka_bulk(38,'KA_BULK_NBR|KA_LINE_NBR|STYLE_CD|COLOR_CD|ATTRIBUTE_CD|SIZE_CD|REQSN_VERSION|REQSN_ID|APPRV_RESPONSE_DATE|REQSN_CREATE_DATE|VENDOR_ID|VENDOR_SUFFIX|CONTACT_PLANNER_CD|ISS_ORDER_TYPE_CD|REQSN_STATUS_CD|PRODUCTION_STATUS|SRC_CONTACT_CD|REQSN_APPROVER_CD|DEMAND_LOC|CURR_DUE_DATE|SEASON_YEAR|SEASON_NAME|TRANSP_MODE_CD|OVER_PCT|UNDER_PCT|CORP_BUSINESS_UNIT|LW_COMPANY|LW_VENDOR_NO|LW_VENDOR_LOC|ISS_PROGRAM_TYPE_CD|DETAIL_TRKG_IND|CREATE_DATE|MFG_PATH_ID|MFG_REVISION_NO|UNIT_OF_MEASURE|CURR_ORDER_QTY|PLANT_CD|PROCESSED_TO_OS','" + req.BulkNumber
                //KA_LINE_NBR|STYLE_CD|COLOR_CD|ATTRIBUTE_CD|SIZE_CD
                + "|" + item.LineNumber + "|" + Val(item.Style) + "|" + Val(item.Color)
                + "|" + Val(item.Attribute) + "|" + Val(item.Size) + "|"

                //REQSN_VERSION|REQSN_ID|APPRV_RESPONSE_DATE|REQSN_CREATE_DATE|VENDOR_ID
                 + req.RequisitionVersion + "|" + req.RequisitionId + "|" + ((req.ApproverResponseDate.HasValue) ? req.ApproverResponseDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty) + "|"
                  + ((req.ReqCreateDate.HasValue) ? req.ReqCreateDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty)
                  + "|" + (req.VendorId) + "|"

                //VENDOR_SUFFIX|CONTACT_PLANNER_CD|ISS_ORDER_TYPE_CD|REQSN_STATUS_CD|PRODUCTION_STATUS|SRC_CONTACT_CD
                 + req.VendorSuffix + "|" + Val(req.PlanningContact) + "|" + Val(req.OrderType) + "|"
                  + req.ReqStatus + "|" + Val(req.ProdStatus) + "|" + Val(req.SourcingContact) + "|"

                //REQSN_APPROVER_CD|DEMAND_LOC|CURR_DUE_DATE|SEASON_YEAR|SEASON_NAME|TRANSP_MODE_CD
                 + req.RequisitionApprover + "|" + Val(req.DcLoc) + "|" + ((item.CurrDueDate.HasValue) ? item.CurrDueDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty) + "|"
                  + SeasonYear + "|" + Val(SeasonName) + "|" + Val(req.TranspMode) + "|"

                //OVER_PCT|UNDER_PCT|CORP_BUSINESS_UNIT|LW_COMPANY|LW_VENDOR_NO|LW_VENDOR_LOC
                 + ((req.OverPercentage.HasValue) ? (req.OverPercentage.Value / 100.0M) : 0) + "|"
                 + ((req.UnderPercentage.HasValue) ? (req.UnderPercentage.Value / 100.0M) : 0) + "|" + Val(req.BusinessUnit) + "|"
                  + req.LwCompany + "|" + (req.VendorNo) + "|" + Val(req.LwVendorLoc) + "|"

                //ISS_PROGRAM_TYPE_CD|DETAIL_TRKG_IND|CREATE_DATE|MFG_PATH_ID|MFG_REVISION_NO
                + req.ProType + "|" + ((req.DetailTrkgInd) ? LOVConstants.Yes : LOVConstants.No)
                + "|"
                + ((req.CreatedOn.HasValue) ? req.CreatedOn.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty) + "|"
                  + req.MFGPathId + "|" + (item.Rev) + "|"

                //UNIT_OF_MEASURE|CURR_ORDER_QTY|PLANT_CD|PROCESSED_TO_OS_1
                 + item.Uom + "|" + (item.Qty) + "||" + item.ProcessedToOS
                + "');END; ");

            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            var result = (String)ExecuteScalar(queryBuilder.ToString());

            return (result == null || result == "Y") ? true : false;

        }



        private void getRequisition(BulkOrderDetail req, BulkOrderDetail item, out ISS.Core.Model.Order.Requisition r,
         out   ISS.Core.Model.Order.RequisitionDetail rItem)
        {

            r = new ISS.Core.Model.Order.Requisition();
            rItem = new ISS.Core.Model.Order.RequisitionDetail();

            //rItem.SuperOrder =item.SuperOrder 
            //    r.OrderVersion =req.OrderVersion 
            r.RequisitionId = req.RequisitionId.ToString();
            r.ProdStatus = req.ProdStatus;
            // r.DemandType= req.DemandType;
            //      r.OriginalDueDate= req.OriginalDueDate;
            // rItem.DozensOnlyInd=item.DozensOnlyInd
            // rItem.CreateBDInd= item.CreateBDInd
            //      rItem.Priority =item.Priority 
            //r.SpreadTypeCD = req.SpreadTypeCD 
            r.BusinessUnit = req.BusinessUnit;
            r.MFGPathId = req.MFGPathId;
            r.MFGPathId = req.MFGPathId;
            r.DcLoc = req.DcLoc;
            r.RequisitionId = req.RequisitionId.ToString();
            // rItem.PlanDate=item.PlanDate;
            //  rItem.ScheduledShipDate=item.ScheduledShipDate;
            r.LwCompany = req.LwCompany;
            r.VendorNo = Convert.ToDecimal(req.VendorNo);
            r.LwVendorLoc = req.LwVendorLoc;
            r.VendorId = req.VendorId;
            r.VendorSuffix = req.VendorSuffix;

            rItem.Style = item.Style;
            rItem.Size = item.Size;
            //rItem.MakeOrBuyCode =item.MakeOrBuyCode ;
            rItem.Attribute = item.Attribute;
            rItem.Color = item.Color;
            //rItem.GarmentStyle=item.GarmentStyle;
            //rItem.SellingStyle =item.SellingStyle ;
            rItem.Rev = Convert.ToDecimal(item.Rev);
            rItem.Qty = item.Qty;
            rItem.RequisitionVer = item.RequisitionVersion;
            rItem.CurrDueDate = item.CurrDueDate;
        }


        private void SetRequisitionDefaults(BulkOrderDetail req)
        {
            req.RequisitionVersion = 1; // hard coded for insert
            //req.DemandType = LOVConstants.DemandType;
            //req.CurrentDueDate = req.DemandDate = req.OriginalDueDate = req.DCDueDate = req.PlannedDcDate;
            req.ProdStatus = LOVConstants.ProductionStatus.Locked;
            req.ReqStatus = LOVConstants.RequestStatus.UnderConstruction;

        }
        #region Component Save
        public KeyValuePair<bool, String> UpdateComponent(BulkOrderDetail req, List<BulkOrderDetail> items)
        {
            String Msg = "Failed to update Bulk Order.";
            bool Status = true;
            var bilkorderList = GetBulkOrderDetail(req.BulkNumber, req.ProgramSource, false);
            if (bilkorderList.Count == 0)
            {
                Msg = "Invalid bulk order number.";
                return new KeyValuePair<bool, string>(false, Msg);
            }
            
			if (req != null)
            {

                if (bilkorderList != null && bilkorderList.Where(e => e.ProcessedToOS == LOVConstants.BulkOrderStatus.Error).Count() > 0)
                {
                    try
                    {
                        foreach (var item in bilkorderList.Where(e => e.ProcessedToOS == LOVConstants.BulkOrderStatus.Error))
                        {

                            if (!VerifyComponentOrder(item, out Msg))
                            {
                                Status = false;
                                item.ErrorMessage = Msg;
                                item.ErrorStatus = true;
                            }
 							else
                                {
                                    item.ErrorMessage = String.Empty;
                                    item.ErrorStatus = false;
                                }
                            }
                            if (!Status)
                            {
                                return new KeyValuePair<bool, string>(Status, "Failed to Validate Orders To AVYX");
                            }
                           
                            if (Status)
                            {
                                string blkNbr = req.BulkNumber;
                                string processedToAVYX = LOVConstants.BulkOrderStatus.Pending;
                                string Update = string.Empty;
                                using (TransactionScope scope = new TransactionScope())
                                {
                                    try
                                    {
                                        BeginTransaction();
                                        Status = UpdateComponentOrderItem(blkNbr, processedToAVYX, Update);
                                        if(Status)
                                        {
                                           Status= DeleteComponentFromBulkError(blkNbr);
                                        }
                                        else
                                        {
                                            Status = false;
                                            
                                        }
                                        
                                        if (Status)
                                        {
                                            CommitTransaction();
                                        }
                                        if (Status)
                                        {
                                            Msg = "Order updated successfully.";
                                        }

                                    }
                                    catch (Exception ES)
                                    {
                                        RollbackTransaction();
                                        Log("Save Component order-trans scope");
                                        Log(ES);
                                        Msg = ES.Message;
                                    }
                                }// END TRANS SCOPE
                            }// validation completed and save starts end
                        }
                        catch (Exception ee)
                        {
                            Log(ee);
                            Msg = ee.Message;
                        }
                        finally
                        {

                        }
                    }
                    else
                    {
                        Msg = "No detail to update.";
                    }
                }
            

            return new KeyValuePair<bool, string>(Status, Msg);
        }

        public bool VerifyComponentOrder(BulkOrderDetail item,out String ErrMsg)
        
        {
            Decimal qty = item.Qty.ConvertDzToEaches();
            var queryBuilder = new StringBuilder();
            queryBuilder.Append("SELECT  OPRSQL.KA_PREPROCESSOR_PKG.verify_comp_bulk('" + Val(item.APSStyle) + "','" + Val(item.APSColor) + "','" + Val(item.APSAttribute) + "','" + Val(item.APSSize) + "','" + qty+ "')FROM dual");
            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            var result = ExecuteScalar(queryBuilder.ToString());
            ErrMsg = (result != null) ? result.ToString().Trim() : String.Empty;
            return String.IsNullOrEmpty(ErrMsg);
        }
        /// <summary>
        /// 
        /// Delete Components
        /// </summary>
        /// <param name="req"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        /// 
       

        public bool DeleteComponentFromBulkError(String blkNbr)//only need this
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append(" BEGIN OPRSQL.KA_PREPROCESSOR_PKG.delete_comp_bulk_errors(1,'KA_BULK_NBR','" + Val(blkNbr) + "');END; ");

            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            var result = (String)ExecuteScalar(queryBuilder.ToString());
            return (result == null || result == "Y") ? true : false;
        }
        /// <summary>
        /// For Updating KA_COMPONENT_PREPROCESSOR
        /// </summary>
        /// <param name="req"></param>
        /// <param name="item"></param>
        /// <returns></returns>

        protected bool UpdateComponentOrderItem(String blkNbr, String processedToAVYX, String UpdateUser)
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append(" Begin OPRSQL.KA_PREPROCESSOR_PKG.update_comp_bulk(3,'KA_BULK_NBR|PROCESSED_TO_AVYX|UPDATE_USER_ID','" 
               // KA_BULK_NBR|PROCESSED_TO_AVYX
                + Val(blkNbr) + "|"  + Val(processedToAVYX) + "|"  + Val(UpdateUser)
                + "');END; ");
            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            var result = (String)ExecuteScalar(queryBuilder.ToString());
            return (result == null || result == "Y") ? true : false;
        }
        /// <summary>
        /// For deleting all component orders with respect to Bulk Number
        /// </summary>
        /// <param name="bulkNo"></param>
        /// <returns></returns>
        public bool DeleteAllComponentOrders(String bulkNo)
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append(" BEGIN OPRSQL.KA_PREPROCESSOR_PKG.delete_comp_bulk (1,'KA_BULK_NBR','" + bulkNo + "');END; ");

            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            var result = (String)ExecuteScalar(queryBuilder.ToString());

            return (result == null || result == "Y") ? true : false; 
        }

        public KeyValuePair<bool, String> CompleteComponentProcess(BulkOrderDetail req, List<BulkOrderDetail> items)
        {
            String Msg = "Failed to complete Bulk Order.";
            bool Status = true;
            var bilkorderList = GetBulkOrderDetail(req.BulkNumber, req.ProgramSource, false);
            if (bilkorderList.Count == 0)
            {
                Msg = "Invalid bulk order number.";
                return new KeyValuePair<bool, string>(false, Msg);
            }

            if (req != null && bilkorderList != null)
            {

                if (bilkorderList.Where(e => e.ProcessedToOS == LOVConstants.BulkOrderStatus.Awaiting).Count() == bilkorderList.Count)
                {
                    try
                    {
                       
                        if (Status)
                        {
                            string blkNbr = req.BulkNumber;
                            string processedToAVYX = LOVConstants.BulkOrderStatus.Completed;
                            string UpdateUser = req.CreatedBy;
                            using (TransactionScope scope = new TransactionScope())
                            {
                                try
                                {
                                    BeginTransaction();
                                    Status = UpdateComponentOrderItem(blkNbr, processedToAVYX, UpdateUser);

                                    if (Status)
                                    {
                                        CommitTransaction();
                                    }
                                    if (Status)
                                    {
                                        Msg = "Order completed successfully.";
                                    }

                                }
                                catch (Exception ES)
                                {
                                    RollbackTransaction();
                                    Log("Complete Component order-trans scope");
                                    Log(ES);
                                    Msg = ES.Message;
                                    Status = false;
                                }
                            }// END TRANS SCOPE
                        }// validation completed and save starts end
                    }
                    catch (Exception ee)
                    {
                        Log(ee);
                        Msg = ee.Message;
                        Status = false;
                    }
                    finally
                    {

                    }
                }
                else
                {
                    Msg = "We cannot complete the process since processed to AVYX indicator of all items are not Awaiting.";
                    Status = false;
                }
            }
            else
            {
                Msg = "Bulk Order has some incorrect details.";
                Status = false;
            }


            return new KeyValuePair<bool, string>(Status, Msg);
        }

        public KeyValuePair<bool, String> ActivateComponentProcess(BulkOrderDetail req, List<BulkOrderDetail> items)
        {
            String Msg = "Failed to Re-Activate Bulk Order.";
            bool Status = true;

            //Due to Huge data in component table items are not passing from Client side
            var bilkorderList = GetBulkOrderDetail(req.BulkNumber, req.ProgramSource, false);
            if (bilkorderList.Count == 0)
            {
                Msg = "Invalid bulk order number.";
                return new KeyValuePair<bool, string>(false, Msg);
            }

            //if (req != null && items != null)
            {

                if (bilkorderList.Where(e => e.ProcessedToOS == LOVConstants.BulkOrderStatus.Completed).Count() == bilkorderList.Count)
                {
                    try
                    {

                        if (Status)
                        {
                            string blkNbr = req.BulkNumber;
                            string processedToAVYX = LOVConstants.BulkOrderStatus.Awaiting;
                            string UpdateUser = req.CreatedBy;
                            using (TransactionScope scope = new TransactionScope())
                            {
                                try
                                {
                                    BeginTransaction();
                                    Status = UpdateComponentOrderItem(blkNbr, processedToAVYX, UpdateUser);

                                    if (Status)
                                    {
                                        CommitTransaction();
                                    }
                                    if (Status)
                                    {
                                        Msg = "Order Re-Activate successfully.";
                                    }

                                }
                                catch (Exception ES)
                                {
                                    RollbackTransaction();
                                    Log("Re-Activate Component order-trans scope");
                                    Log(ES);
                                    Msg = ES.Message;
                                    Status = false;
                                }
                            }// END TRANS SCOPE
                        }// validation completed and save starts end
                    }
                    catch (Exception ee)
                    {
                        Log(ee);
                        Msg = ee.Message;
                        Status = false;
                    }
                    finally
                    {

                    }
                }
                else
                {
                    Msg = "We cannot Re-Activate the process since processed to AVYX indicator of all items are not Completed.";
                    Status = false;
                }
            }
            //else
            //{
            //    Msg = "Bulk Order has some incorrect details.";
            //    Status = false;
            //}


            return new KeyValuePair<bool, string>(Status, Msg);
        }
       #endregion

    }
}
