using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISS.DAL;
using System.Data;
using ISS.Common;
using System.Data.Common;
using KA.Core.Model.BulkOrder;
using ISS.Core.Model.Common;
using ISS.Core.Model.Order;
using Oracle.DataAccess.Client;
using ISS.Repository.Order;
using System.Transactions;



namespace KA.Repository.AttributionOrder
{
    public class AttributedWorkOrder : RepositoryBase
    {

        public Result SaveAttributedWO(WorkOrderHeader Woh)
        {

            Result res = new Result();
            try
            {
                res = InsertAttributedOrder(Woh, res);
            }
            catch(Exception ex)
            {
                res.ErrMsg = ex.Message;
              //  res.ErrDetails = ex.Message;
            }
            finally
            {
                res.Property1 = Woh.WODetails;
            }

            return res;

        }

        protected Result InsertAttributedOrder(WorkOrderHeader header, Result res)
        {
            var Msg = String.Empty;
            decimal ErrId = -1;
            bool Status = false;
            WorkOrderRepository woRep = new WorkOrderRepository();
            GetRoutingId(header);
            RecomputeDueDates(header);
            for (int i = 0; i < header.WODetails.Count; i++)
            {
                WorkOrderDetail detail = header.WODetails[i];

                detail.MfgPathId = !String.IsNullOrEmpty(detail.MfgPathId) ? detail.MfgPathId.ToUpper() : detail.MfgPathId;
                //detail.Size = header.WODetails[i].SizeList[0].SizeCD;
                detail.Size = header.WODetails[i].SizeList[i].SizeCD;   //Added By :UST(Gopikrishnan), Date:21-June-2017, Desc: For fixing error in getting multiple sizes from SKUSizeList for validating attribution.
                header.WODetails[i].PipeLineCat = LOVConstants.PipeLineCategoryCode.SEW;

                detail.AttributionInd = IsAttributionOrder(detail);

                if (detail.AttributionInd == LOVConstants.No)
                {
                    Msg = "Attribute '" + detail.Attribute + "' is not valid for Attributed Orders";
                    detail.ErrorMessage = Msg;
                    detail.ErrorStatus = true;
                    res.ErrMsg = Msg;
                    res.Status = false;
                    res.FailCount++;
                }

                else if (String.IsNullOrEmpty(detail.GarmentSKU.TrimEnd()))
                {
                    Msg = "SKU '" + detail.SellingStyle + " " + detail.ColorCode + " " + detail.Attribute + " " + detail.Size + "' does not have a Garment SKU";
                    detail.ErrorMessage = Msg;
                    detail.ErrorStatus = true;
                    res.ErrMsg = Msg;
                    res.Status = false;
                    res.FailCount++;
                }
                else if (header.WODetails[i].CurrOrderQty <= 0)
                {
                    Msg = "SKU '" + detail.SellingStyle + " " + detail.ColorCode + " " + detail.Attribute + " " + detail.Size + "' - quantity should be greater than zero.";
                    detail.ErrorMessage = Msg;
                    detail.ErrorStatus = true;
                    res.ErrMsg = Msg;
                    res.Status = false;
                    res.FailCount++;
                }
                else if (header.WODetails[i].CurrOrderQty >= 300000.0m)
                {
                    Msg = "SKU '" + detail.SellingStyle + " " + detail.ColorCode + " " + detail.Attribute + " " + detail.Size + "' - Quantity must be less than 300000.";
                    detail.ErrorMessage = Msg;
                    detail.ErrorStatus = true;
                    res.ErrMsg = Msg;
                    res.Status = false;
                    res.FailCount++;
                }
                else if (header.WODetails[i].PlannedDate < DateTime.Now.AddDays(-60))
                {
                    Msg = "Invalid Start Date. Date cannot be more than 60 days past due.";
                    detail.ErrorMessage = Msg;
                    detail.ErrorStatus = true;
                    res.ErrMsg = Msg;
                    res.Status = false;
                    res.FailCount++;
                }

                else if (string.IsNullOrWhiteSpace(header.WODetails[i].AttributionPath))
                {
                    Msg = "Attribute Path cannot be Null. Please select a valid MFG Path ID. ";
                    detail.ErrorMessage = Msg;
                    detail.ErrorStatus = true;
                    res.ErrMsg = Msg;
                    res.Status = false;
                    res.FailCount++;
                }
                  
                else
                {
                    Status = ValidateDetailBeforeSave(header, detail, out Msg, out ErrId);
                    res.Status = Status;
                }
            }
            if (res.FailCount == 0)
            {
                for (int i = 0; i < header.WODetails.Count; i++)
                {
                    WorkOrderDetail detail = header.WODetails[i];
                    detail.MfgPathId = !String.IsNullOrEmpty(detail.MfgPathId) ? detail.MfgPathId.ToUpper() : detail.MfgPathId;
                    //detail.Size = header.WODetails[i].SizeList[0].SizeCD;
                    detail.Size = header.WODetails[i].SizeList[i].SizeCD;   //Added By :UST(Gopikrishnan), Date:21-June-2017, Desc: For fixing error in getting multiple sizes from SKUSizeList for saving attribution.
                    if (res.Status)
                    {
                        res = SaveChanges(header, detail, res);

                    }
                    else
                    {
                        detail.ErrorMessage = Msg;
                        detail.ErrorStatus = true;
                        res.ErrMsg = Msg;
                        res.Status = false;
                    }
                }
            }

            return res;
        }

        protected string IsAttributionOrder(WorkOrderDetail detail)
        {
            String msg = String.Empty;
            var queryBuilder = new StringBuilder();
            detail.MfgPathId = !String.IsNullOrEmpty(detail.MfgPathId) ? detail.MfgPathId.ToUpper() : detail.MfgPathId;
            queryBuilder.Append("SELECT OPRSQL.ISS_ATTR_PKG.IsAttributionOrder('" + Val(detail.SellingStyle) + "','" + Val(detail.ColorCode) + "','" + Val(detail.Attribute) + "','" + Val(detail.Size) + "','" + Val(detail.MfgPathId) + "') from dual");



            System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
            var result = (String)ExecuteScalar(queryBuilder.ToString());
            msg = (result != null) ? result.Replace("|", "\n").Trim() : String.Empty;

            return msg;


        }
        public void RecomputeDueDates(WorkOrderHeader header)
        {
            WorkOrderRepository woRep = new WorkOrderRepository();
            decimal prodTime = 0;
            decimal LeadTime = 0;
            DateTime dtPlanDate = new DateTime();
            DateTime dtDueDate = new DateTime();

            for (int i = 0; i < header.WODetails.Count; i++)
            {
                dtDueDate = header.PlannedDate;
                dtDueDate = woRep.MoveToNextDay(dtDueDate, (int)DayOfWeek.Friday);
                //Wod.CutPath = !String.IsNullOrEmpty(WomDet.CutPath) ? WomDet.CutPath.ToUpper() : WomDet.CutPath;
                if (!string.IsNullOrEmpty(header.WODetails[i].RoutingId))
                {
                    prodTime = woRep.ProductionTime(header.WODetails[i].RoutingId);
                }
                if (prodTime > 0)
                {
                    dtDueDate = dtDueDate.AddDays(1);
                }
                WorkOrderCumulative woCum = new WorkOrderCumulative();
                woCum.DueDate = dtDueDate;
                woCum.BackSchedule = 0;
                woCum.MFGPlant = header.WODetails[i].AttributionPath;                 
                woCum.DemandLoc = header.Dc;
                woCum.StyleCode = header.WODetails[i].PKGStyle;
                woCum.ColorCode = header.WODetails[i].ColorCode;
                woCum.AttributeCode = header.WODetails[i].Attribute;
                woCum.SizeCode = header.WODetails[i].SizeList[0].SizeCD;
                woCum.MFGPathId = !String.IsNullOrEmpty(header.WODetails[i].MfgPathId) ? header.WODetails[i].MfgPathId.ToUpper() : header.WODetails[i].MfgPathId;
                woCum.RoutingId = header.WODetails[i].RoutingId;
                woCum.MakeOrBuyCode = header.WODetails[i].MorBCd;
                woRep.SetDueDate(woCum);
                header.WODetails[i].CurrentDueDate = woCum.CurrentDueDate;
                header.WODetails[i].LeadTime = woCum.PlanningLeadTime;
                header.WODetails[i].PlannedDate = woCum.PlanDate;
            }
        }

        public void GetRoutingId(WorkOrderHeader header)
        {
            WorkOrderRepository woRep = new WorkOrderRepository();
            for (int j = 0; j < header.WODetails.Count; j++)
            {
                var routingid = woRep.GetRoutingId(header.WODetails[j].PKGStyle, header.WODetails[j].ColorCode, header.WODetails[j].Attribute, header.WODetails[j].Size, !String.IsNullOrEmpty(header.WODetails[j].MfgPathId) ? header.WODetails[j].MfgPathId.ToUpper() : header.WODetails[j].MfgPathId); // CA#94164-18- Garment Skus are missing
                if (routingid.Count > 0)
                {
                    for (int i = 0; i < routingid.Count; i++)
                    {
                        if (!String.IsNullOrEmpty(routingid[i].RoutingId))
                            header.WODetails[j].RoutingId = routingid[i].RoutingId.ToString();
                        else
                            header.WODetails[j].RoutingId = String.Empty;
                        if (!String.IsNullOrEmpty(routingid[i].BoMId))
                            header.WODetails[j].BoMId = routingid[i].BoMId.ToString();
                        else
                            header.WODetails[j].BoMId = String.Empty;

                        header.WODetails[j].LeadTime = routingid[i].LeadTime;
                        if (!String.IsNullOrEmpty(routingid[i].CapacityAlloc))
                            header.WODetails[j].CapacityAlloc = routingid[i].CapacityAlloc;
                        else
                            header.WODetails[j].CapacityAlloc = string.Empty;
                        if (!String.IsNullOrEmpty(routingid[i].MorBCd))
                            header.WODetails[j].MorBCd = routingid[i].MorBCd;
                        else
                            header.WODetails[j].MorBCd = string.Empty;
                        header.WODetails[j].PlannerCd = routingid[i].PlannerCd;
                        if (!String.IsNullOrEmpty(routingid[i].UoM))
                            header.WODetails[j].UoM = routingid[i].UoM.ToString();
                        else
                            header.WODetails[j].UoM = LOVConstants.UOM.DZ;

                    }
                }
                else
                {

                    header.WODetails[j].RoutingId = String.Empty;
                    header.WODetails[j].BoMId = String.Empty;
                    header.WODetails[j].LeadTime = 0;
                    header.WODetails[j].MorBCd = LOVConstants.MakeOrBuy.Make;
                    header.WODetails[j].PlannerCd = "UNK";
                    header.WODetails[j].UoM = LOVConstants.UOM.DZ;
                }
            }

        }

        public Result SaveChanges(WorkOrderHeader header, WorkOrderDetail detail, Result res)
        {

            res = InsertWOMOrderDetails(header, detail, res);
            return res;
        }
        public Result InsertWOMOrderDetails(WorkOrderHeader header, WorkOrderDetail detail, Result res)
        {

            try
            {
                if (res.Status)
                {
                    Requisition req = new Requisition();
                    req.OrderVersion = LOVConstants.GlobalOrderVersion;
                    req.DemandType = LOVConstants.AODemandType;
                    req.CurrentDueDate = header.PlannedDate;
                    req.DemandDate = header.PlannedDate;

                    String ErrMsgValDet = String.Empty;
                    for (int j = 0; j < header.OrdersToCreate; j++)
                    {
                        using (TransactionScope scope = new TransactionScope())
                        {
                            try
                            {
                                BeginTransaction();

                                if (res.Status)
                                {
                                    res = AttributionOrderDetail(detail, header, res, req);


                                    if (res.Status)
                                    {
                                        CommitTransaction();
                                        res.SuccessCount++;
                                    }
                                    else
                                    {
                                        RollbackTransaction();
                                        res.FailCount++;
                                    }

                                }
                            }
                            catch (Exception ES)
                            {
                                res.Status = false;
                                RollbackTransaction();
                                res.FailCount++;
                                Log("Save WOM-trans scope");
                                Log(ES);
                                res.ErrMsg = ES.Message;
                            }
                        }
                    }
                    //if (res.SuccessCount > 0)
                    //{
                    //    detail.ErrorMessage = res.ErrMsg = res.SuccessCount + " Order(s) created successfully.";
                    //    res.Status = true;
                    //}
                }
                else
                {
                    detail.ErrorStatus = true;
                    detail.ErrorMessage = res.ErrMsg;
                    res.Status = false;
                }
            }
            catch (Exception ee)
            {

                res.Status = false;
                detail.ErrorMessage = res.ErrMsg = ee.Message;
            }

            return res;
        }
        protected bool ValidateDetailBeforeSave(WorkOrderHeader header, WorkOrderDetail detail, out String ErrMsg, out decimal ErrId)
        {
            var queryBuilder = new StringBuilder();
            try
            {
                detail.MfgPathId = !String.IsNullOrEmpty(detail.MfgPathId) ? detail.MfgPathId.ToUpper() : detail.MfgPathId;
                detail.NoteInd = (detail.Note == "" || detail.Note == null) ? LOVConstants.No : LOVConstants.Yes;
                queryBuilder.Append("SELECT oprsql.iss_prod_order_validate.verify(19,'SUPER_ORDER|ORDER_VERSION|PRODUCTION_STATUS|PRIORITY|SEW_PATH|DEMAND_LOC|PACK_CD|DISCRETE_IND|ROW_NUMBER|STYLE_CD|SIZE_CD|ATTRIBUTE_CD|COLOR_CD|SELLING_STYLE_CD|MFG_REVISION_NO|MAKE_OR_BUY_CD|SELLING_COLOR_CD|SELLING_ATTRIBUTE_CD|SELLING_SIZE_CD','" +
                    //SUPER_ORDER|ORDER_VERSION|PRODUCTION_STATUS|
                    "1" + "|" + LOVConstants.GlobalOrderVersion + "|" + Val(LOVConstants.ProductionStatus.Locked) + "|" +

                    //PRIORITY|SEW_PATH|
                    detail.Priority + "|" + Val(detail.MfgPathId)

                  //|DEMAND_LOC|
                 + "|" + Val(header.Dc)
                    //PACK_CD|
                    + "|" + Val(detail.PackCode) + "|"

                    //DISCRETE_IND|ROW_NUMBER|
                    + ("Y") + "|" + "1" + "|" +

                    //STYLE_CD|SIZE_CD|ATTRIBUTE_CD|COLOR_CD|SELLING_STYLE_CD|
                    Val(detail.PKGStyle) + "|" + Val(detail.Size) + "|" + (detail.Attribute) + "|" + Val(detail.ColorCode) + "|" + Val(detail.SellingStyle) + "|"
                    //MFG_REVISION_NO|MAKE_OR_BUY|SELLING_COLOR_CD|SELLING_ATTRIBUTE_CD|SELLING_SIZE_CD // CA#94706-18- To fix Path Dest Plant issue
                    + detail.Revision + "|" + LOVConstants.MakeOrBuy.Make + "|" + Val(detail.ColorCode) + "|" + Val(detail.Attribute) + "|" + Val(detail.Size) + "') from dual ");

                System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
                var result = (String)ExecuteScalar(queryBuilder.ToString());
                ErrMsg = (result != null) ? result.Replace("|", "\n").Trim() : String.Empty;


                if (!String.IsNullOrEmpty(ErrMsg))
                    ErrId = 1; // error

                else
                {
                    ErrId = -1;


                }
            }
            catch (OracleException ee)
            {
                ErrId = 1;
                ErrMsg = ee.Message;
            }

            return String.IsNullOrEmpty(ErrMsg);
        }



        protected Result AttributionOrderDetail(WorkOrderDetail detail, WorkOrderHeader woHeader, Result res, Requisition req)
        {
            bool isGarmentSKU = false;
            SourceOrderRepository soRepository = new SourceOrderRepository(trans);
            WorkOrderRepository woRep = new WorkOrderRepository(trans);
            if (detail != null)
            {
                isGarmentSKU = false;
                var CumulativeSuperOrder = String.Empty;
                if (!string.IsNullOrEmpty(detail.SuperOrder))
                {
                    CumulativeSuperOrder = detail.SuperOrder; //From AWOM

                    res.Status = soRepository.DeleteOrder(woHeader, detail.SuperOrder);
                    if (res.Status)
                    {

                        if (detail.IsUngrouped)
                        {
                            if (!woRep.WOMOrderDeleteGroupId(LOVConstants.GlobalOrderVersion, detail.SuperOrder))
                            {
                                res.Status = false;
                                res.ErrMsg = "Failed to ungroup order.";
                            }
                        }
                    }
                }
                var reqDetailSuperOrder = ConvertToRequisition(woHeader, req, soRepository, CumulativeSuperOrder, String.Empty, detail, isGarmentSKU);
                res.Status = soRepository.AddOrderTableManual(req, reqDetailSuperOrder);

                if (res.Status)
                {
                    res.Status = soRepository.AddInsertOrderManual(req, reqDetailSuperOrder);
                    if (detail.GroupId > 0)
                    {
                        res.Status = woRep.AddInsertGroupId(reqDetailSuperOrder.OrderVersion, reqDetailSuperOrder.SuperOrder, detail.GroupId.ToString());
                        //PrevStyle = detail.SellingStyle;
                    }
                    if (res.Status && (!String.IsNullOrWhiteSpace(detail.Note)))
                    {
                        res.Status = woRep.AddNote(reqDetailSuperOrder.OrderLabel, detail.Note);
                    }
                }


            }
            return res;

        }
        //CA# 367901-16- To take the planner code from MFG_PATH_CHP

        public RequisitionDetail GetPlannerCode(WorkOrderDetail item)
        {
            
            var query = new StringBuilder();
            item.MfgPathId = !String.IsNullOrEmpty(item.MfgPathId) ? item.MfgPathId.ToUpper() : item.MfgPathId;
            query.Append("select a.planner_cd Planner from mfg_path_chp a where ");
            query.Append(" a.style_cd = '" + Val(item.SellingStyle) + "'");
            query.Append(" and a.color_cd = '" + Val(item.ColorCode) + "'");
            query.Append(" and a.attribute_cd = '" + Val(item.Attribute) + "'");
            query.Append(" and a.size_cd = '" + Val(item.Size) + "'");
            query.Append(" and a.mfg_path_id = '" + Val(item.MfgPathId) + "'");


            IDataReader reader = ExecuteReader(query.ToString());
            var plannerlist = (new DbHelper()).ReadData<RequisitionDetail>(reader);
            if (plannerlist.Count > 0)
            {
                return plannerlist[0];
            }
            else

                return null;
        }

        private RequisitionDetail ConvertToRequisition(WorkOrderHeader woHeader, Requisition req,
            SourceOrderRepository soRepository, String superOrder, String ParentOrder, WorkOrderDetail woDetail, bool isGarmentSKU)
        {

            RequisitionDetail reqDetail = new RequisitionDetail();

            req.OrderVersion = LOVConstants.GlobalOrderVersion;
            reqDetail.OrderVersion = LOVConstants.GlobalOrderVersion;
            req.RequisitionVersion = LOVConstants.GlobalOrderVersion;
            if (string.IsNullOrEmpty(superOrder))
                reqDetail.OrderLabel = soRepository.getNewOrderLabel().ToString();
            else
                reqDetail.OrderLabel = superOrder;
            reqDetail.SuperOrder = reqDetail.OrderLabel;
            reqDetail.Style = woDetail.PKGStyle;
            reqDetail.Color = woDetail.ColorCode;
            reqDetail.Attribute = woDetail.Attribute;
            reqDetail.Size = woDetail.Size;
            reqDetail.ProdFamilyCd = woDetail.ProdFamCode;
            req.BusinessUnit = woDetail.CorpBusUnit;
            reqDetail.MatlCd = LOVConstants.MATL_TYPE_CD.Garment;
            reqDetail.DCLoc = woHeader.Dc;
            reqDetail.Uom = woDetail.UoM;
            req.DcLoc = woHeader.Dc;
            reqDetail.DemandQty = woDetail.CurrOrderQty;
            reqDetail.Rev = woDetail.Revision;
            reqDetail.MfgPathId = !String.IsNullOrEmpty(woDetail.MfgPathId) ? woDetail.MfgPathId.ToUpper() : woDetail.MfgPathId;
            req.MFGPathId = woDetail.AttributionPath; 
            req.OriginalDueDate = woDetail.CurrentDueDate;
            reqDetail.CurrDueDate = woDetail.CurrentDueDate;
            reqDetail.RoutinId = woDetail.RoutingId;
            reqDetail.BillOfMATL = woDetail.BoMId;
            reqDetail.CuttingAlt = !String.IsNullOrEmpty(woDetail.CuttingAlt) ? woDetail.CuttingAlt.ToUpper() : woDetail.CuttingAlt;
            reqDetail.PlanningLeadTime = woDetail.LeadTime;
            reqDetail.PackCD = woDetail.PackCode;
            reqDetail.CategoryCD = woDetail.CategoryCode;
            reqDetail.PlanDate = woDetail.PlannedDate;
            reqDetail.MachineCut = woDetail.MachineCut;
            reqDetail.CapacityAlloc = woDetail.CapacityAlloc;
            req.ProdStatus = String.IsNullOrEmpty(woHeader.ProductionStatus) ? LOVConstants.ProductionStatus.Locked : ((woHeader.ProductionStatus == LOVConstants.ProductionStatus.Suggested) ? LOVConstants.ProductionStatus.TextileSuggested : woHeader.ProductionStatus);
            if (String.IsNullOrEmpty(woHeader.PlannerCd) || woHeader.PlannerCd == "-Select-")
            {

                var plannerReq = GetPlannerCode(woDetail);
                if (plannerReq != null)
                    reqDetail.Planner = plannerReq.Planner;
                else reqDetail.Planner = "PLANN";

            }
               
            
            // reqDetail.Planner = (String.IsNullOrEmpty(woHeader.PlannerCd) || woHeader.PlannerCd == "-Select-") ? "PLANN" : woHeader.PlannerCd;
            reqDetail.AsrmtCd = woDetail.AssortCode;
            reqDetail.CapacityAlloc = woDetail.CapacityAlloc;
            reqDetail.OrderType = LOVConstants.WorkOrderType.WO;
            reqDetail.Priority = LOVConstants.Priority;

            reqDetail.TotatalCurrOrderQty = woDetail.CurrOrderTotQty;
            
            reqDetail.MakeOrBuyCode = woDetail.MorBCd;
            reqDetail.ExpeditePriority = woDetail.PriorityCode.ToString();

            reqDetail.Qty = woDetail.CurrOrderQty;
            
            reqDetail.PipeLineCategoryCD = woDetail.PipeLineCat;
            req.CreatedBy = woHeader.CreatedBy;
            reqDetail.AttributionInd = woDetail.AttributionInd;
            if (!string.IsNullOrEmpty(woDetail.BulkNumber))
            {
                reqDetail.DemandSource = woDetail.BulkNumber;
                reqDetail.DemandDriver = LOVConstants.KADemandDriver;
            }
            if(!String.IsNullOrEmpty(woDetail.OrderReference))
            {
                reqDetail.OrderReference = woDetail.OrderReference;
            }
            return reqDetail;
        }

        //MFG PathId
        public List<WorkOrderDetail> GetKAMFGPathId(string style, string color, string attribute, List<MultiSKUSizes> size, string dLoc)
        {
            var query = new StringBuilder();
            query.Append("select a.mfg_path_id \"MfgPathId\",max(prime_mfg_location)\"SewPlt\",min( a.priority)\"Priority\",  ");
            query.Append("a.bill_of_mtrls_id \"BoMId\", bm.comp_style_cd \"GStyle\" , bm.comp_color_cd \"GColor\", bm.comp_attribute_cd \"GAttribute\", bm.comp_size_cd \"GSize\", bm.activity_cd \"ActivityCode\" from ");
            query.Append("(select mfg_path_id,prime_mfg_location,5000 priority, style_cd, color_cd,attribute_cd, size_cd, bill_of_mtrls_id from mfg_path  where");
            query.Append(" style_cd ='" + Val(style) + "'");
            query.Append(" and color_cd = '" + Val(color) + "'");
            query.Append(" and attribute_cd ='" + Val(attribute) + "'");
            query.Append(" and size_cd in (" + SizeList(size) + ")");
            query.Append(" UNION ");
            query.Append("SELECT mfg_path_id, '' prime_mfg_location ,path_ranking_no priority, style as style_cd,color as color_cd, attribute_cd, size_cd, '' as bill_of_mtrls_id  FROM PATH_RANKING  WHERE");
            query.Append(" style ='" + Val(style) + "'");
            query.Append(" AND color in ( '" + Val(color) + "','ALL')");
            query.Append(" AND attribute_cd ='" + Val(attribute) + "'");
            query.Append(" AND size_cd in (" + SizeList(size) + " ,'AL')");
            query.Append(" and demand_loc = '" + Val(dLoc) + "'");
            query.Append(")a,   bill_of_mtrls bm  ");
            query.Append("WHERE a.style_cd = bm.parent_style(+) and a.color_cd = bm.parent_color(+) and a.attribute_cd = bm.parent_attribute(+) and a.size_cd = bm.parent_size(+) ");
            query.Append(" and a.bill_of_mtrls_id = bm.bill_of_mtrls_id (+) and nvl(bm.activity_cd,'PUL') = 'PUL'");
            query.Append(" group by a.mfg_path_id,  a.bill_of_mtrls_id, bm.comp_style_cd, bm.comp_color_cd, bm.comp_attribute_cd, bm.comp_size_cd, bm.activity_cd order by 3");

            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<WorkOrderDetail>(reader);
            return result;

        }

        public string SizeList(List<MultiSKUSizes> sizes)
        {

            string size = String.Empty;

            if (sizes != null)
            {
                var arrSize = sizes.Select(i => i.SizeCD).ToArray();
                size = "'" + string.Join("','", arrSize) + "'";
            }
            return size;
        }


        public bool UpdateWOMGroupedOrders(List<WOMDetail> wom)
        {
            // SELECT oprsql.iss_prod_order_validate.verify(28,'SUPER_ORDER|ORDER_VERSION|DOZENS_ONLY_IND|CREATE_BD_IND|PRIORITY|SPREAD_TYPE_CD|SEW_PATH|CUT_PATH|TXT_PATH|MACHINE|CURR_ORDER_QTY|CURR_ORDER_TOTAL_QTY|CUT_MASTER|DEMAND_LOC|DOZENS_ONLY_IND|CREATE_BD_IND|GARMENT_STYLE|DISCRETE_IND|ROW_NUMBER|CURR_FIN_LBS|STYLE_CD|SIZE_CD|MAKE_OR_BUY_CD|ATTRIBUTE_CD|COLOR_CD|GARMENT_STYLE|SELLING_STYLE_CD|MFG_REVISION_NO|','646327364|1|N|Y|1387241||KG|23|5M|N/A|1332|1332||KG|N|Y|MHG9D7|Y|10|26.115|PH5100|43|M|------|VB7|MHG9D7|MHHH51|0') from dual
            bool Stat = false;
            String Msg = String.Empty;
            //var groups = wom.GroupBy(g => new
            //{
            //    Attr = g.Attribute,
            //    MFGPath = g.MfgPathId,
            //    AltId = g.DcLoc
            //    //,CurDate = g.CurrDueDateStr
            //});
            //if (groups.Count() > 1)
            //{
            //    var invalid = groups.Skip(1).FirstOrDefault();
            //    invalid.ToList().ForEach(item =>
            //    {
            //        Stat = true;
            //        item.ErrorMessage = " Multi Sku order must match by Attribute, Path Id and DC .";
            //        Msg = "Failed to group orders.";
            //    });

            //}
            if (!Stat)
            {
                //Duplicate Garment TBD
                //var size = wom.Select(e => e.Size).ToList();
                //if (size.Count != size.Distinct().Count())
                //{
                //    var duplicate = string.Join(", ", size.Except(size.Distinct()).ToArray());
                //    Stat = true;
                //    Msg = "Duplicate Size " + duplicate;
                //}
            }
            if (Stat)
            {
                wom.ForEach(e =>
                {
                    e.ErrorStatus = Stat;
                    e.ErrorMessage = (String.IsNullOrEmpty(e.ErrorMessage) ? Msg : e.ErrorMessage);
                });
                return false;
            }

            decimal ErrId;
            bool valid = true;
            wom.ForEach(e =>
            {
                if (valid)
                {
                    valid = ValidateDetailBeforeSave(e, out Msg, out ErrId);
                    if (!valid)
                    {
                        e.ErrorStatus = true;
                        e.ErrorMessage = Msg;
                    }
                }
            });

            if (!valid)
            {
                return false;
            }
            using (TransactionScope scope = new TransactionScope())
            {
                BeginTransaction();
                try
                {
                    WorkOrderRepository wor = new WorkOrderRepository(trans);
                    for (int i = 0; i < wom.Count; i++)
                    {

                        var item = wom[i];
                        if (item.IsGrouped)
                        {
                            Stat = wor.AddInsertGroupId(item.OrderVersion, item.SuperOrder, item.GroupId);
                        }
                        if (item.IsEdited && item.IsFieldChange)
                            Stat = WOMOrderUpdate(item);//add order_reference col for update+ quantity

                        if (Stat && item.NoteInd == LOVConstants.Yes && !String.IsNullOrWhiteSpace(item.Note))
                        {
                            Stat = wor.AddNote(item.SuperOrder, item.Note);
                        }
                        if (!Stat)
                        {
                            item.ErrorMessage = "Failed to update order details.";
                            item.ErrorStatus = false;
                            RollbackTransaction();
                            return false;
                        }
                    }
                    CommitTransaction();
                    return true;
                }
                catch (OracleException ox)
                {
                    Log("UpdateWOMGroupedOrders", ox);
                    RollbackTransaction();
                }
            }
            return false;
        }

        protected bool ValidateDetailBeforeSave(WOMDetail detail, out String ErrMsg, out decimal ErrId)
        {
            var queryBuilder = new StringBuilder();
            try
            {

                detail.NoteInd = (detail.Note == "" || detail.Note == null) ? LOVConstants.No : LOVConstants.Yes;
                detail.MfgPathId = !String.IsNullOrEmpty(detail.MfgPathId) ? detail.MfgPathId.ToUpper() : detail.MfgPathId;

                queryBuilder.Append("SELECT oprsql.iss_prod_order_validate.verify(19,'SUPER_ORDER|ORDER_VERSION|PRODUCTION_STATUS|PRIORITY|SEW_PATH|DEMAND_LOC|PACK_CD|DISCRETE_IND|ROW_NUMBER|STYLE_CD|SIZE_CD|ATTRIBUTE_CD|COLOR_CD|SELLING_STYLE_CD|MFG_REVISION_NO|MAKE_OR_BUY_CD|SELLING_COLOR_CD|SELLING_ATTRIBUTE_CD|SELLING_SIZE_CD','" +
                    //SUPER_ORDER|ORDER_VERSION|PRODUCTION_STATUS|
                    detail.SuperOrder + "|" + detail.OrderVersion + "|" + detail.OrderStatus + "|" +

                    //PRIORITY|SEW_PATH|
                    detail.Priority + "|" + Val(detail.MfgPathId)

                  //|DEMAND_LOC|
                 + "|" + Val(detail.DcLoc)
                    //PACK_CD|
                    + "|" + Val(detail.PackCode) + "|"

                    //DISCRETE_IND|ROW_NUMBER|
                    + (detail.DescreteInd) + "|" + "1" + "|" +

                    //STYLE_CD|SIZE_CD|ATTRIBUTE_CD|COLOR_CD|SELLING_STYLE_CD|
                    Val(detail.Style) + "|" + Val(detail.Size) + "|" + (detail.Attribute) + "|" + Val(detail.Color) + "|" + Val(detail.Style) + "|"
                    + detail.Revision.ToString() + "|" + LOVConstants.MakeOrBuy.Make + "|" + Val(detail.Color) + "|" + Val(detail.Attribute) + "|" + Val(detail.Size) + "') from dual ");
                //MFG_REVISION_NO|MAKE_OR_BUY_CD|SELLING_COLOR_CD|SELLING_ATTRIBUTE_CD|SELLING_SIZE_CD
                //+ "') from dual "); // CA#94706-18- To fix Path Dest Plant issue

                System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
                var result = (String)ExecuteScalar(queryBuilder.ToString());
                ErrMsg = (result != null) ? result.Replace("|", "\n").Trim() : String.Empty;


                if (!String.IsNullOrEmpty(ErrMsg))
                    ErrId = 1; // error

                else
                {
                    ErrId = -1;


                }
            }
            catch (OracleException ee)
            {
                ErrId = 1;
                ErrMsg = ee.Message;
            }

            return String.IsNullOrEmpty(ErrMsg);
        }

        private bool WOMOrderUpdate(WOMDetail wom)
        {
            wom.TotatalCurrOrderQty = wom.Qty;

            // CUT_MASTER|PRODUCTION_STATUS|ISS_ORDER_TYPE_CD|DOZENS_ONLY_IND|CREATE_BD_IND(Y/N)
            //BEGIN OPRSQL.ISS_PROD_ORDER_MAINT.UPDATE_ORDER(11,'SUPER_ORDER|ORDER_VERSION|MACHINE_TYPE_CD|CURR_ORDER_QTY|TOTAL_CURR_ORDER_QTY|PIPELINE_CATEGORY_CD|CURR_DUE_DATE|PACK_CD|CATEGORY_CD|EXPEDITE_PRIORITY|MAKE_OR_BUY_CD','646327364|1|R3|134664|134664|DC|20151121|3|62|60|M');END;

            // wom.Qty = wom.QtyDZ * LOVConstants.Dozen;
            // wom.TotatalCurrOrderQty = wom.TotalDozens * LOVConstants.Dozen;
            bool IsDateChange = false;
            //wom.PipelineCategoryCode = LOVConstants.ProductionCategories.SEW;
            if (wom.Cloned != null)
            {
                if (wom.CurrDueDate.HasValue && wom.CurrDueDate.Value.Date != wom.Cloned.CurrDueDate.Value.Date)
                {
                    wom.PipelineCategoryCode = LOVConstants.PipelineActivity.DC;
                    IsDateChange = true;
                }
            }
            {
                if (wom.CCurrDueDate.Value.Date != wom.CurrDueDate.Value.Date)
                {
                    wom.PipelineCategoryCode = LOVConstants.PipelineActivity.DC;
                    IsDateChange = true;
                }
            }
           
            //if (wom.StartDate.Value.Date != wom.Cloned.StartDate.Value.Date)
            //{
            //    wom.PipelineCategoryCode = LOVConstants.PipelineActivity.DBF;
            //    IsDateChange = true;
            //}
            //ORDER_ID|DEMAND_TYPE|ORIG_DUE_DATE|CORP_BUSINESS_UNIT|SCHED_SHIP_DATE|STYLE_CD|COLOR_CD|ATTRIBUTE_CD|SIZE_CD|MFG_REVISION_NO|DEMAND_QTY|ORIG_ORDER_QTY
            //PRIORITY|DEMAND_SOURCE|PLANNER_CD|DEMAND_DATE|DISCRETE_IND order_reference
            var queryBuilder = new StringBuilder();
            queryBuilder.Append(" Begin OPRSQL.ISS_PROD_ORDER_MAINT.UPDATE_ORDER(" +
                ((IsDateChange) ? 16 : 14)
                + ",'SUPER_ORDER|ORDER_VERSION|MACHINE_TYPE_CD|CURR_ORDER_QTY|TOTAL_CURR_ORDER_QTY" +
                ((IsDateChange) ? "|PIPELINE_CATEGORY_CD|CURR_DUE_DATE" : string.Empty)
                + "|PACK_CD|CATEGORY_CD|EXPEDITE_PRIORITY|MAKE_OR_BUY_CD|CUT_MASTER|PRODUCTION_STATUS|ISS_ORDER_TYPE_CD|ORDER_REFERENCE|USER_ID','" +

               // SUPER_ORDER|ORDER_VERSION|MACHINE_TYPE_CD|CURR_ORDER_QTY|TOTAL_CURR_ORDER_QTY
               Val(wom.SuperOrder) + "|" + wom.OrderVersion + "|" + Val(wom.MC) + "|" + (wom.Qty) + "|" + (wom.TotatalCurrOrderQty) +

               ((IsDateChange) ? (
                //PIPELINE_CATEGORY_CD*|CURR_DUE_DATE
                "|" + Val(wom.PipelineCategoryCode) + "|" +
                ((wom.PipelineCategoryCode == LOVConstants.PipelineActivity.DC) ?
                ((wom.CurrDueDate.HasValue) ? wom.CurrDueDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty) :
                ((wom.StartDate.HasValue) ? wom.StartDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty)
                )
                ) : string.Empty
               )

               //|PACK_CD|CATEGORY_CD|EXPEDITE_PRIORITY
               + "|" + Val(wom.PackCode) + "|" + Val(wom.CategoryCode) + "|" + (wom.ExpeditePriority.HasValue ? (wom.ExpeditePriority.Value + string.Empty) : string.Empty) +

                   //MAKE_OR_BUY_CD|CUT_MASTER|PRODUCTION_STATUS|ISS_ORDER_TYPE_CD|DOZENS_ONLY_IND|CREATE_BD_IND
              "|" + Val(wom.MakeOrBuy) + "|" + Val(wom.GroupId) + "|" + Val(wom.OrderStatus) + "|" + Val(wom.OrderType) + "|" + Val(wom.OrderRef) + "|" + Val(wom.CreatedBy) + "');END; ");

            var result = (String)ExecuteScalar(queryBuilder.ToString());

            return (result == null || result == "Y") ? true : false;
        }

        private bool WOMOrderDetailedUpdate(WOMDetail wom)
        {
            wom.TotatalCurrOrderQty = wom.Qty;

            // CUT_MASTER|PRODUCTION_STATUS|ISS_ORDER_TYPE_CD|DOZENS_ONLY_IND|CREATE_BD_IND(Y/N)
            //BEGIN OPRSQL.ISS_PROD_ORDER_MAINT.UPDATE_ORDER(11,'SUPER_ORDER|ORDER_VERSION|MACHINE_TYPE_CD|CURR_ORDER_QTY|TOTAL_CURR_ORDER_QTY|PIPELINE_CATEGORY_CD|CURR_DUE_DATE|PACK_CD|CATEGORY_CD|EXPEDITE_PRIORITY|MAKE_OR_BUY_CD','646327364|1|R3|134664|134664|DC|20151121|3|62|60|M');END;

            // wom.Qty = wom.QtyDZ * LOVConstants.Dozen;
            // wom.TotatalCurrOrderQty = wom.TotalDozens * LOVConstants.Dozen;
            bool IsDateChange = false;
            wom.PipelineCategoryCode = LOVConstants.ProductionCategories.SEW;
            if (wom.CurrDueDate.Value.Date != wom.Cloned.CurrDueDate.Value.Date)
            {
                //wom.PipelineCategoryCode = LOVConstants.PipelineActivity.DC;
                IsDateChange = true;
            }
            if (wom.StartDate.Value.Date != wom.Cloned.StartDate.Value.Date)
            {
                //wom.PipelineCategoryCode = LOVConstants.PipelineActivity.DBF;
                IsDateChange = true;
            }
            //ORDER_ID|DEMAND_TYPE|ORIG_DUE_DATE|CORP_BUSINESS_UNIT|SCHED_SHIP_DATE|STYLE_CD|COLOR_CD|ATTRIBUTE_CD|SIZE_CD|MFG_REVISION_NO|DEMAND_QTY|ORIG_ORDER_QTY
            //PRIORITY|DEMAND_SOURCE|PLANNER_CD|DEMAND_DATE|DISCRETE_IND 
            var queryBuilder = new StringBuilder();
            queryBuilder.Append(" Begin OPRSQL.ISS_PROD_ORDER_MAINT.UPDATE_ORDER(" +
                ((IsDateChange) ? 15 : 13)
                + ",'SUPER_ORDER|ORDER_VERSION|MACHINE_TYPE_CD|CURR_ORDER_QTY|TOTAL_CURR_ORDER_QTY" +
                ((IsDateChange) ? "|PIPELINE_CATEGORY_CD|CURR_DUE_DATE" : string.Empty)
                + "|PACK_CD|CATEGORY_CD|EXPEDITE_PRIORITY|MAKE_OR_BUY_CD|CUT_MASTER|PRODUCTION_STATUS|ISS_ORDER_TYPE_CD|USER_ID','" +

               // SUPER_ORDER|ORDER_VERSION|MACHINE_TYPE_CD|CURR_ORDER_QTY|TOTAL_CURR_ORDER_QTY
               Val(wom.SuperOrder) + "|" + wom.OrderVersion + "|" + Val(wom.MC) + "|" + (wom.Qty) + "|" + (wom.TotatalCurrOrderQty) +

               ((IsDateChange) ? (
                //PIPELINE_CATEGORY_CD*|CURR_DUE_DATE
                "|" + Val(wom.PipelineCategoryCode) + "|" +
                ((wom.PipelineCategoryCode == LOVConstants.PipelineActivity.DC) ?
                ((wom.CurrDueDate.HasValue) ? wom.CurrDueDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty) :
                ((wom.StartDate.HasValue) ? wom.StartDate.Value.ToString(LOVConstants.DateFormatOracle) : String.Empty)
                )
                ) : string.Empty
               )

               //|PACK_CD|CATEGORY_CD|EXPEDITE_PRIORITY
               + "|" + Val(wom.PackCode) + "|" + Val(wom.CategoryCode) + "|" + (wom.ExpeditePriority.HasValue ? (wom.ExpeditePriority.Value + string.Empty) : string.Empty) +

                   //MAKE_OR_BUY_CD|CUT_MASTER|PRODUCTION_STATUS|ISS_ORDER_TYPE_CD|DOZENS_ONLY_IND|CREATE_BD_IND
              "|" + Val(wom.MakeOrBuy) + "|" + Val(wom.GroupId) + "|" + Val(wom.OrderStatus) + "|" + Val(wom.OrderType) + "|" + Val(wom.CreatedBy) + "');END; ");

            var result = (String)ExecuteScalar(queryBuilder.ToString());

            return (result == null || result == "Y") ? true : false;
        }

        public bool UpdateWOMOrder(WOMDetail wom)
        {
            // SELECT oprsql.iss_prod_order_validate.verify(28,'SUPER_ORDER|ORDER_VERSION|DOZENS_ONLY_IND|CREATE_BD_IND|PRIORITY|SPREAD_TYPE_CD|SEW_PATH|CUT_PATH|TXT_PATH|MACHINE|CURR_ORDER_QTY|CURR_ORDER_TOTAL_QTY|CUT_MASTER|DEMAND_LOC|DOZENS_ONLY_IND|CREATE_BD_IND|GARMENT_STYLE|DISCRETE_IND|ROW_NUMBER|CURR_FIN_LBS|STYLE_CD|SIZE_CD|MAKE_OR_BUY_CD|ATTRIBUTE_CD|COLOR_CD|GARMENT_STYLE|SELLING_STYLE_CD|MFG_REVISION_NO|','646327364|1|N|Y|1387241||KG|23|5M|N/A|1332|1332||KG|N|Y|MHG9D7|Y|10|26.115|PH5100|43|M|------|VB7|MHG9D7|MHHH51|0') from dual

            String Msg;
            decimal ErrId;
            var Stat = ValidateDetailBeforeSave(wom, out Msg, out ErrId);
            if (!Stat)
            {
                wom.ErrorStatus = true;
                wom.ErrorMessage = Msg;
                return false;
            }
            using (TransactionScope scope = new TransactionScope())
            {
                BeginTransaction();
                try
                {
                    SourceOrderRepository sourceRep = new SourceOrderRepository(trans);
                    WOManagement womRep = new WOManagement(trans);
                    bool stat = true;
                    if (wom.IsEdited && wom.IsFieldChange)
                        stat = WOMOrderUpdate(wom);
                    if (wom.IsUngrouped)
                    {
                        stat = womRep.WOMOrderDeleteGroupId(wom);
                    }

                    if (Stat && wom.NoteInd == LOVConstants.Yes && !String.IsNullOrWhiteSpace(wom.Note))
                    {
                        WorkOrderRepository wor = new WorkOrderRepository(trans);
                        Stat = wor.AddNote(wom.SuperOrder, wom.Note);
                    }
                    if (stat)
                    {
                        CommitTransaction();
                        return true;
                    }
                    else
                    {
                        wom.ErrorMessage = "Failed to update order details.";
                        wom.ErrorStatus = true;
                        RollbackTransaction();
                    }
                }
                catch (OracleException ox)
                {
                    Log(ox);
                    wom.ErrorMessage = ox.Message;
                    wom.ErrorStatus = true;
                    RollbackTransaction();
                }
            }
            return false;
        }

        public bool SummarizeAOMOrders(List<WOMDetail> wom)
        {

            bool Stat = false;
            String Msg = String.Empty;

            if (Stat)
            {
                wom.ForEach(e =>
                {
                    e.ErrorStatus = Stat;
                    e.ErrorMessage = (String.IsNullOrEmpty(e.ErrorMessage) ? Msg : e.ErrorMessage);
                });
                return false;
            }

            decimal ErrId;
            bool valid = true;
            wom.ForEach(e =>
            {
                if (!e.IsHide)
                {
                    valid = ValidateDetailBeforeSave(e, out Msg, out ErrId);
                    if (!valid)
                    {
                        e.ErrorStatus = true;
                        e.ErrorMessage = Msg;
                    }
                }
            });

            if (!valid)
            {
                return false;
            }
            using (TransactionScope scope = new TransactionScope())
            {
                BeginTransaction();
                try
                {
                    WorkOrderRepository wor = new WorkOrderRepository(trans);
                    for (int i = 0; i < wom.Count; i++)
                    {
                        if (!wom[i].IsHide)
                        {
                            var item = wom[i];
                            if (item.IsGrouped)
                            {
                                Stat = wor.AddInsertGroupId(item.OrderVersion, item.SuperOrder, item.GroupId);
                            }
                            if (item.IsEdited && item.IsFieldChange)
                                Stat = WOMOrderUpdate(item);//add order_reference col for update+ quantity

                            if (Stat && item.NoteInd == LOVConstants.Yes && !String.IsNullOrWhiteSpace(item.Note))
                            {
                                Stat = wor.AddNote(item.SuperOrder, item.Note);
                            }

                            if (!Stat)
                            {
                                item.ErrorMessage = "Failed to Summarize order details.";
                                item.ErrorStatus = false;
                                RollbackTransaction();
                                return false;
                            } 
                        }
                    }
                    if (Stat)
                    {
                        var req = new RequisitionDetail();
                        SourceOrderRepository soRep = new SourceOrderRepository();
                        foreach (WOMDetail item in wom)
                        {
                            if (item.IsHide)
                            {
                                req.SuperOrder = item.SuperOrder;
                                req.OrderVersion = item.OrderVersion;
                                soRep.DeleteOrder(req);
                            }
                        }

                        CommitTransaction();
                    }
                    return true;
                }
                catch (OracleException ox)
                {
                    Log("SummarizeAOMOrders", ox);
                    RollbackTransaction();
                }
            }
            return false;
        }
        private void UpdateAOMOrder()
        {

        }
        public bool UpdateChange(List<WOMDetail> WomDet)
        {
            WOMDetail wodet = new WOMDetail();
            bool status = false;
            for (int i = 0; i < WomDet.Count; i++)
            {
                status = CreateWoObj(WomDet[i]);
            }
            return status;

        }
        public bool CreateWoObj(WOMDetail WomDet)
        {
            //bool status = false;
            WorkOrderDetail Wod = new WorkOrderDetail();
            Wod.PKGStyle = WomDet.Style;
            Wod.MfgPathId = !String.IsNullOrEmpty(WomDet.MfgPathId) ? WomDet.MfgPathId.ToUpper() : WomDet.MfgPathId;
            Wod.ColorCode = WomDet.Color;
            Wod.Attribute = WomDet.Attribute;
            Wod.Size = WomDet.Size;
            Wod.SizeShortDes = WomDet.SizeShortDes;
            Wod.TotalDozens = WomDet.Qty;
            Wod.SewPlt = WomDet.SewPath;
            Wod.CutPath = WomDet.CutPath;
            Wod.TxtPath = WomDet.TxtPath;
            Wod.CuttingAlt = !String.IsNullOrEmpty(WomDet.AltId) ? WomDet.AltId.ToUpper() : WomDet.AltId;
            Wod.BulkNumber = WomDet.DemandSource;
            Wod.OrderReference = WomDet.OrderRef;
            Wod.SizeList = new List<MultiSKUSizes>();
            Wod.SizeList.Add(new MultiSKUSizes()
            {
                Size = Wod.SizeShortDes,
                SizeCD = Wod.Size,
                Qty = Wod.TotalDozens
            });
            Wod.SellingStyle = WomDet.SellingStyle;
            Wod.Revision = WomDet.Revision;
            Wod.AssortCode = WomDet.AssortCode;
            Wod.PriorityCode = WomDet.ExpeditePriority.Value;
            Wod.PackCode = WomDet.PackCode;
            //Wod.AttributionPath = WomDet.MFGPlant;  CA#383957-16
            Wod.AttributionPath = WomDet.SewPath;
            Wod.IsUngrouped = WomDet.IsUngrouped;
            Wod.UoM = WomDet.UOM;
            Wod.WODetail = new List<WorkOrderDetail>();

            WorkOrderRepository WoRep = new WorkOrderRepository();
            Result res = new Result();

            var Msg = String.Empty;
            decimal ErrId = -1;
            bool Status = true;

            Status = ValidateDetailBeforeSave(WomDet, out Msg, out ErrId);
            res.Status = Status;
            //2nd Call
            if (res.Status)
            {
                var assrmtCode = WoRep.GetWOAsrtCode(Wod.SellingStyle);


                decimal grpId = 0;
                if (!String.IsNullOrEmpty(WomDet.GroupId))
                    grpId = Decimal.Parse(WomDet.GroupId);


                if (assrmtCode.Count > 0)
                {
                    Wod.ProdFamCode = assrmtCode[0].ProdFamCode;
                    Wod.OriginTypeCode = assrmtCode[0].OriginTypeCode;
                    Wod.PrimaryDC = assrmtCode[0].PrimaryDC;
                    Wod.CorpBusUnit = assrmtCode[0].CorpBusUnit;
                    Wod.AssortCode = assrmtCode[0].AssortCode;
                }

                var garmentSku = WoRep.GetGarmentSKU(Wod.SellingStyle, Wod.ColorCode, Wod.Attribute, Wod.SizeList, !String.IsNullOrEmpty(Wod.MfgPathId) ? Wod.MfgPathId.ToUpper() : Wod.MfgPathId);
                if (!string.IsNullOrEmpty(garmentSku.GarmentSKU))
                {
                    Wod.GStyle = garmentSku.GStyle;
                    Wod.GColor = garmentSku.GColor;
                    Wod.GAttribute = garmentSku.GAttribute;
                    Wod.GSize = garmentSku.GSize;
                }


                Wod.WODetail.Add(new WorkOrderDetail()
                {
                    PKGStyle = Wod.PKGStyle,
                    ColorCode = Wod.ColorCode,
                    SizeList = Wod.SizeList,
                    Attribute = Wod.Attribute,
                    MachineType = Wod.MachineType,
                    CuttingAlt = !String.IsNullOrEmpty(WomDet.AltId) ? WomDet.AltId.ToUpper() : WomDet.AltId,
                    AlternateId = Wod.AlternateId,
                    CutPath = Wod.CutPath,
                    SewPlt = Wod.SewPlt,
                    Note = WomDet.Note,
                    GroupId = grpId,
                    SellingStyle = WomDet.SellingStyle,
                    Revision = WomDet.Revision,
                    AssortCode = Wod.AssortCode,
                    MfgPathId = !String.IsNullOrEmpty(Wod.MfgPathId) ? Wod.MfgPathId.ToUpper() : Wod.MfgPathId,
                    Size = Wod.Size,
                    SizeShortDes = Wod.SizeShortDes,
                    TotalDozens = Wod.TotalDozens,
                    ProdFamCode = Wod.ProdFamCode,
                    OriginTypeCode = Wod.OriginTypeCode,
                    PrimaryDC = Wod.PrimaryDC,
                    CorpBusUnit = Wod.CorpBusUnit,
                    PackCode = Wod.PackCode,
                    PriorityCode = Wod.PriorityCode,
                    AttributionPath = Wod.AttributionPath,
                    IsUngrouped = Wod.IsUngrouped,
                    GStyle = Wod.GStyle,
                    GColor = Wod.GColor,
                    GAttribute = Wod.GAttribute,
                    GSize = Wod.GSize,
                    UoM = Wod.UoM,
                    BulkNumber = Wod.BulkNumber,
                    OrderReference = Wod.OrderReference
                });

                //Status = SaveChanges(Wod, WomDet);
                Status = ConvertToWO(Wod, WomDet);
            }

            else
            {
                WomDet.ErrorStatus = true;
                WomDet.ErrorMessage = Msg;
            }
            return Status;
        }
        public bool ConvertToWO(WorkOrderDetail Wod, WOMDetail WomDet)
        {
            WorkOrderHeader woHeader = new WorkOrderHeader();
            Result res = new Result();

            UpdateHeader(woHeader, Wod, WomDet);

            var detail = Wod.WODetail.FirstOrDefault();
            if (detail != null)
            {
                detail.SuperOrder = WomDet.SuperOrder;
               // detail.Dozens = WomDet.Qty;
                detail.CurrOrderQty = WomDet.Qty;
                detail.CurrOrderTotQty = WomDet.Qty;

            }
            res = InsertAttributedOrder(woHeader, res);
            if (!res.Status)
            {
                WomDet.ErrorStatus = true;
                WomDet.ErrorMessage = res.ErrMsg;
            }

            return res.Status;
        }

        public void UpdateHeader(WorkOrderHeader woHeader, WorkOrderDetail Wod, WOMDetail WomDet)
        {
            woHeader.Dmd = WomDet.DemandType;
            woHeader.Dc = WomDet.DcLoc;
            woHeader.TxtPlant = WomDet.TxtPath;
            woHeader.MachinePlant = WomDet.MC;
            woHeader.WOFabric = Wod.WOFabric;
            woHeader.PlannedDate = WomDet.CurrDueDate.GetValueOrDefault();
            woHeader.WODetails = Wod.WODetail;
            woHeader.WOCumulative = Wod.WOCumulative;
            woHeader.OrderType = WomDet.OrderType;
            woHeader.ProductionStatus = WomDet.OrderStatus;
            woHeader.DemandSource = WomDet.DemandSource;
            woHeader.ExpeditePriority = WomDet.ExpeditePriority.GetValueOrDefault().ToString();
            woHeader.Priority = WomDet.Priority;
            woHeader.CategoryCode = WomDet.CategoryCode;
            woHeader.CreateBDInd = WomDet.CreateBDInd;
            woHeader.DozensOnlyInd = WomDet.DozensOnlyInd;
            woHeader.FQQty = WomDet.Qty;
            woHeader.OrdersToCreate = 1;
            woHeader.CreatedBy = WomDet.CreatedBy;
        }

        ////WOM Insert
        //public Result InsertWOMOrderDetails(WorkOrderHeader woHeader, WOMDetail womDet)
        //{
        //    Result res = new Result();
        //    bool Status = true;
        //    WorkOrderRepository woRep = new WorkOrderRepository();

        //    using (TransactionScope scope = new TransactionScope())
        //    {
        //        try
        //        {
        //            if (res.Status)
        //                res = InsertAttributedOrder(woHeader, res);

        //        }
        //        catch (Exception ES)
        //        {
        //            res.Status = false;
        //            RollbackTransaction();
        //            Log("Save WOM-trans scope");
        //            Log(ES);
        //            res.ErrMsg = ES.Message;
        //        }
        //    }

        //    return res;
        //}

        public List<WorkOrderDetail> GetKAMFGPathDetails(string style, string color, string attribute, List<MultiSKUSizes> size, string dLoc, string mfgPathId)
        {
            var query = new StringBuilder();
            query.Append("select a.mfg_path_id \"MfgPathId\",max(prime_mfg_location)\"SewPlt\",min( a.priority)\"Priority\",  ");
            query.Append("a.bill_of_mtrls_id \"BoMId\", bm.comp_style_cd \"GStyle\" , bm.comp_color_cd \"GColor\", bm.comp_attribute_cd \"GAttribute\", bm.comp_size_cd \"GSize\", bm.activity_cd \"ActivityCode\" from ");
            query.Append("(select mfg_path_id,prime_mfg_location,5000 priority, style_cd, color_cd,attribute_cd, size_cd, bill_of_mtrls_id from mfg_path  where");
            query.Append(" style_cd ='" + Val(style) + "'");
            query.Append(" and color_cd = '" + Val(color) + "'");
            query.Append(" and attribute_cd ='" + Val(attribute) + "'");
            query.Append(" and size_cd in (" + SizeList(size) + ")");
            query.Append(" UNION ");
            query.Append("SELECT mfg_path_id, '' prime_mfg_location ,path_ranking_no priority, style as style_cd,color as color_cd, attribute_cd, size_cd, '' as bill_of_mtrls_id  FROM PATH_RANKING  WHERE");
            query.Append(" style ='" + Val(style) + "'");
            query.Append(" AND color in ( '" + Val(color) + "','ALL')");
            query.Append(" AND attribute_cd ='" + Val(attribute) + "'");
            query.Append(" AND size_cd in (" + SizeList(size) + " ,'AL')");
            query.Append(" and demand_loc = '" + Val(dLoc) + "'");
            query.Append(")a,   bill_of_mtrls bm  ");
            query.Append("WHERE a.style_cd = bm.parent_style(+) and a.color_cd = bm.parent_color(+) and a.attribute_cd = bm.parent_attribute(+) and a.size_cd = bm.parent_size(+) ");
            query.Append(" and a.bill_of_mtrls_id = bm.bill_of_mtrls_id (+) and nvl(bm.activity_cd,'PUL') = 'PUL' and a.mfg_path_id = '" + Val(mfgPathId) + "' ");
            query.Append(" group by a.mfg_path_id,  a.bill_of_mtrls_id, bm.comp_style_cd, bm.comp_color_cd, bm.comp_attribute_cd, bm.comp_size_cd, bm.activity_cd order by 3");

            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<WorkOrderDetail>(reader);
            return result;

        }
        public List<WorkOrderHeader> GetAODCLocations(string style, string color, string attribute)
        {
            var query = new StringBuilder();
            query.Append(" SELECT distinct dest_plant_cd \"DC\",P.LEGAL_NAME \"LegalName\" ");
            query.Append(" FROM PATH_DEST_PLANT A, PLANT P where");
            query.Append(" style_cd ='" + Val(style) + "'");
            query.Append(" and color_cd = '" + Val(color) + "'");
            query.Append(" and attribute_cd ='" + Val(attribute) + "'");
            query.Append(" and a.dest_plant_cd = P.PLANT_CD ");
            query.Append(" and P.distribution_ind = 'Y'");
            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<WorkOrderHeader>(reader);
            return result;

        }
    }
}

