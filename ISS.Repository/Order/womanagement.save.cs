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


namespace ISS.Repository.Order
{
    public partial class WOManagement : RepositoryBase
    {
        //Validate

        //Delete

        //Insert


        //Object Mapping Functions WOM-WO & VV

        public Result UpdateChange(List<WOMDetail> WomDet)
        {
            WOMDetail wodet = new WOMDetail();
            Result res = new Result();
            for (int i = 0; i < WomDet.Count; i++)
            {
                res=CreateWoObj(WomDet[i]);
            }
            return res;

        }
        public Result CreateWoObj(WOMDetail WomDet)
        {
            WorkOrderDetail Wod = new WorkOrderDetail();
            Wod.PKGStyle = WomDet.Style;
            Wod.MfgPathId = WomDet.MfgPathId;
            Wod.ColorCode = WomDet.Color;
            Wod.Attribute = WomDet.Attribute;
            Wod.Size = WomDet.Size;
            Wod.SizeShortDes = WomDet.SizeShortDes;
            Wod.TotalDozens = WomDet.TotalDozens;
            Wod.SewPlt = WomDet.SewPath;
            Wod.Revision = WomDet.Revision;
            //Wod.AlternateId = !String.IsNullOrEmpty(WomDet.AltId) ? WomDet.AltId.ToUpper() : WomDet.AltId;
            Wod.AlternateId = !String.IsNullOrEmpty(WomDet.AltIdd) ? WomDet.AltIdd.ToUpper() : WomDet.AltIdd;
            Wod.CutPath = !String.IsNullOrEmpty(WomDet.CutPath) ? WomDet.CutPath.ToUpper() : WomDet.CutPath;
            Wod.TxtPath = !String.IsNullOrEmpty(WomDet.TxtPath) ? WomDet.TxtPath.ToUpper() : WomDet.TxtPath;
            Wod.CuttingAlt = !String.IsNullOrEmpty(WomDet.AltId) ? WomDet.AltId.ToUpper() : WomDet.AltId;
            Wod.DemandDriver = WomDet.DemandDriver;
            Wod.DemandSource = WomDet.DemandSource;
           
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
            Wod.WODetail = new List<WorkOrderDetail>();

            WorkOrderRepository WoRep = new WorkOrderRepository();
            Result res = new Result();
           
            var Msg = String.Empty;
            decimal ErrId = -1;
            bool Status = false;
            string asrmtCode = "";
            
            Status = ValidateDetailBeforeSave(WomDet, out Msg, out ErrId);
            res.Status = Status;
            //2nd Call
            if (res.Status)
            {
                WoRep.UpdateCumulative(Wod);
                Wod.WOCumulative.Reverse();
 				decimal grpId = 0;
                if (!String.IsNullOrEmpty(WomDet.GroupId))
                    grpId = Decimal.Parse(WomDet.GroupId);

                var woCum = Wod.WOCumulative.Where(x => x.StyleCd == Wod.PKGStyle).ToList();
                if (woCum.Count > 0)
                {
                    asrmtCode = woCum[0].AsrmtCode;
                }
                Wod.WODetail.Add(new WorkOrderDetail()
                {
                    PKGStyle = Wod.PKGStyle,
                    ColorCode = Wod.ColorCode,
                    SizeList = Wod.SizeList,
                    Attribute = Wod.Attribute,
                    MachineType = Wod.MachineType,
                    CuttingAlt = !String.IsNullOrEmpty(WomDet.AltId) ? WomDet.AltId.ToUpper() : WomDet.AltId,
                    AlternateId = !String.IsNullOrEmpty(Wod.AlternateId) ? Wod.AlternateId.ToUpper() : Wod.AlternateId,
                    CutPath = !String.IsNullOrEmpty(Wod.CutPath) ? Wod.CutPath.ToUpper() : Wod.CutPath,
                    TxtPath = !String.IsNullOrEmpty(Wod.TxtPath) ? Wod.TxtPath.ToUpper() : Wod.TxtPath,
                    SewPlt = Wod.SewPlt,
                    Note = WomDet.Note,
					GroupId= grpId,
                    SellingStyle = WomDet.SellingStyle,
                    Revision = WomDet.Revision,
                    AssortCode = asrmtCode,
                    DemandSource=WomDet.DemandSource,
                    DemandDriver=WomDet.DemandDriver
                });
                Wod.AssortCode = asrmtCode;
                
                //string style = "Hello";
                res = SaveChanges(Wod, WomDet);
               
            }
               
            else
            {
                WomDet.ErrorStatus = true;
                WomDet.ErrorMessage = Msg;
            }
           return res;
        }

       //Update Cut and TextPath
        protected void UpdateCutAndText(WorkOrderDetail Wod,WOMDetail WomDet)
        {
               foreach(var item in Wod.WOCumulative)
               {
                   if(item.Merged==false && item.MatlTypeCode==LOVConstants.MATL_TYPE_CD.CUT)
                   {
                       if (WomDet.CutPath != null && WomDet.CutPath != String.Empty && WomDet.CutPath != WomDet.Cloned.CutPath)
                       {
                           item.MFGPathId = !String.IsNullOrEmpty(WomDet.CutPath) ? WomDet.CutPath.ToUpper() : WomDet.CutPath;
                           item.DemandLoc = !String.IsNullOrEmpty(WomDet.CutPath) ? WomDet.CutPath.ToUpper() : WomDet.CutPath;
                           item.MFGPlant = !String.IsNullOrEmpty(WomDet.CutPath) ? WomDet.CutPath.ToUpper() : WomDet.CutPath;
                       }
                   }
                   else if(item.Merged==false && item.MatlTypeCode==LOVConstants.MATL_TYPE_CD.Fabric)
                   {
                       if(WomDet.TxtPath!=null && WomDet.TxtPath!=String.Empty && WomDet.TxtPath!=WomDet.Cloned.TxtPath)
                       {
                           item.MFGPathId = !String.IsNullOrEmpty(WomDet.TxtPath) ? WomDet.TxtPath.ToUpper() : WomDet.TxtPath;
                           item.DemandLoc = !String.IsNullOrEmpty(WomDet.TxtPath) ? WomDet.TxtPath.ToUpper() : WomDet.TxtPath;
                           item.MFGPlant = !String.IsNullOrEmpty(WomDet.TxtPath) ? WomDet.TxtPath.ToUpper() : WomDet.TxtPath;
                       }
                   }

                 //  item.CurrentOrderQty = item.CurrentOrderTotalQty;
                   }

            
          
        }
        //3rd method
        public Result SaveChanges(WorkOrderDetail Wod, WOMDetail WomDet)
        {
            WorkOrderHeader woHeader = new WorkOrderHeader();
            Result res = new Result();
            var Msg = String.Empty;
            decimal ErrId = -1;
            bool Status = false;
            
            UpdateHeader(woHeader, Wod, WomDet);
            UpdateCutAndText(Wod, WomDet);
            Status = IsValidAlt(woHeader, WomDet,out Msg, out ErrId);
            res.Status = Status;
            if (res.Status)
            {
                var detail = woHeader.WOCumulative.FirstOrDefault(e => e.LevelInd == 0);
                if (detail != null)
                {
                    detail.SuperOrder = WomDet.SuperOrder;
                    detail.RuleNo = WomDet.Rule.GetValueOrDefault();
                }

                WorkOrderRepository WoRep = new WorkOrderRepository();



                res = WoRep.InsertWOMOrderDetails(woHeader, WomDet);
            }
            else
            {
                WomDet.ErrorStatus = true;
                WomDet.ErrorMessage = Msg;
            }
            return res;
        }

        //Update header values with existing order info
//        Sew Path	Cut Path		Txt Path		DC
//Alt Id	M/C	Atr	Path Id 	DC	
//Start date and DC due date
        public void UpdateHeader(WorkOrderHeader woHeader,WorkOrderDetail Wod, WOMDetail WomDet)
        {
            woHeader.Dmd = WomDet.DemandType;
            woHeader.Dc = WomDet.DcLoc;
            woHeader.TxtPlant = !String.IsNullOrEmpty(WomDet.TxtPath) ? WomDet.TxtPath.ToUpper() : WomDet.TxtPath;
            //woHeader.CutPath = !String.IsNullOrEmpty(WomDet.CutPath) ? WomDet.CutPath.ToUpper() : WomDet.CutPath;
            //woHeader.AlternateId = !String.IsNullOrEmpty(WomDet.CutPath) ? WomDet.CutPath.ToUpper() : WomDet.CutPath;
            woHeader.MachinePlant = WomDet.MC;
            woHeader.WOFabric = Wod.WOFabric;
            woHeader.DemandSource = WomDet.DemandSource;
            Wod.DemandDriver = WomDet.DemandDriver;
            Wod.DemandSource = WomDet.DemandSource;
            //woHeader.PlannedDate = WomDet.CurrDueDate.GetValueOrDefault();
            woHeader.PlannedDate = WomDet.StartDate.GetValueOrDefault();
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
            woHeader.CreatedBy = WomDet.CreatedBy;
            SetHeaderDueDate(woHeader, WomDet);
        }
        public void SetHeaderDueDate(WorkOrderHeader woHeader, WOMDetail WomDet)
        {
            if (WomDet.CurrDueDate.Value.Date != WomDet.Cloned.CurrDueDate.Value.Date)
            {
                woHeader.DueDate = LOVConstants.PipelineActivity.DC;

            }
            //if (WomDet.StartDate.Value.Date != WomDet.Cloned.StartDate.Value.Date)
            //{
            //    woHeader.DueDate = LOVConstants.PipelineActivity.DBF;

            //}
            else

                woHeader.DueDate = LOVConstants.PipelineActivity.DBF;
        }
        protected bool IsValidAlt(WorkOrderHeader woHeader,WOMDetail WomDet,out String ErrMsg,out decimal ErrId)
        {

            String tempBOMId = String.Empty;
            String cuttingAlt = String.Empty;
            var Msg = String.Empty;
            WorkOrderRepository WoRep = new WorkOrderRepository();
            var woCum = woHeader.WOCumulative.Where(x => (x.Merged == false) && (x.MatlTypeCode == LOVConstants.MATL_TYPE_CD.CUT) && (x.BillOfMtrlsId != String.Empty)).ToList();
            for (int i = 0; i < woCum.Count; i++)
            {

                WorkOrderCumulative item = woCum[i];
                if (item.MatlTypeCode == LOVConstants.MATL_TYPE_CD.CUT && !String.IsNullOrWhiteSpace(item.BillOfMtrlsId))
                {

                    cuttingAlt = item.BillOfMtrlsId.Substring(item.BillOfMtrlsId.Length - 3, 3);
                    WomDet.AltId = !String.IsNullOrEmpty(WomDet.AltId) ? WomDet.AltId.ToUpper() : WomDet.AltId;
                    //if (cuttingAlt != WomDet.AltId)
                    if (cuttingAlt == WomDet.AltId)  //Modified for validating Alternate Id
                    {
                        tempBOMId = item.BillOfMtrlsId.Substring(0, item.BillOfMtrlsId.Length - 3) + WomDet.AltId;
                        var childList = WoRep.LoadFabricGrid(item.StyleCode, item.ColorCode, item.AttributeCode, item.SizeCode, tempBOMId, item.RoutingId);
                        if (childList.Count == 0)
                        {

                            Msg = "Invalid Alternate for cut part " + item.StyleCode + " " + item.ColorCode;


                        }
                    }
                }
            }


            ErrMsg = Msg;
                
            if (!String.IsNullOrEmpty(ErrMsg))
            {
                
                ErrId = 1;
            }

            else
            {
                ErrId = -1;


            }
            return String.IsNullOrEmpty(ErrMsg);
        }
       
       //1st Method
        protected bool ValidateDetailBeforeSave(WOMDetail woDet, out String ErrMsg, out decimal ErrId)
        {
            
            var queryBuilder = new StringBuilder();
            try
            {
                woDet.AltId = !String.IsNullOrEmpty(woDet.AltId) ? woDet.AltId.ToUpper() : woDet.AltId;
                woDet.CutPath = !String.IsNullOrEmpty(woDet.CutPath) ? woDet.CutPath.ToUpper() : woDet.CutPath;
                woDet.TxtPath = !String.IsNullOrEmpty(woDet.TxtPath) ? woDet.TxtPath.ToUpper() : woDet.TxtPath;
                woDet.NoteInd = (woDet.Note == "" || woDet.Note == null) ? LOVConstants.No : LOVConstants.Yes;

                if (Val(woDet.AltId) == null)
                {
                    //ISS Change to validate SKU/Mfg Path / DC
                    queryBuilder.Append("SELECT oprsql.iss_prod_order_validate.verify(34,'SUPER_ORDER|ORDER_VERSION|PRODUCTION_STATUS|DOZENS_ONLY_IND|CREATE_BD_IND|PRIORITY|SPREAD_TYPE_CD|SEW_PATH|CUT_PATH|TXT_PATH|MACHINE|DEMAND_LOC|PACK_CD|CATEGORY_CD|DOZENS_ONLY_IND|CREATE_BD_IND|GARMENT_STYLE|EXPEDITE_PRIORITY|DISCRETE_IND|ROW_NUMBER|CURR_FIN_LBS|STYLE_CD|SIZE_CD|MAKE_OR_BUY_CD|ATTRIBUTE_CD|COLOR_CD|GARMENT_STYLE|SELLING_STYLE_CD|SELLING_COLOR_CD|SELLING_ATTRIBUTE_CD|SELLING_SIZE_CD|MFG_REVISION_NO|DEMAND_SOURCE|DEMAND_DRIVER|','" +
                        //SUPER_ORDER|ORDER_VERSION|PRODUCTION_STATUS|DOZENS_ONLY_IND|CREATE_BD_IND|
                        Val(woDet.SuperOrder) + "|" + "1" + "|" + Val(woDet.OrderStatus) + "|" + Val(woDet.DozensOnlyInd) + "|" + Val(woDet.CreateBDInd) + "|" +

                        //PRIORITY|SPREAD_TYPE_CD|SEW_PATH|CUT_PATH|
                        woDet.Priority + "|" + Val(null) + "|" + Val(woDet.MfgPathId) + "|" + Val(woDet.CutPath)

                        //TXT_PATH|MACHINE|DEMAND_LOC|
                        + "|" + Val(woDet.TxtPath) + "|" + Val(woDet.MC) + "|" + Val(woDet.DcLoc)
                        //PACK_CD|CATEGORY_CD|DOZENS_ONLY_IND|CREATE_BD_IND|GARMENT_STYLE|
                        + "|" + Val(woDet.PackCode) + "|" + Val(woDet.CategoryCode) + "|"+ Val(woDet.DozensOnlyInd) + "|" + Val(woDet.CreateBDInd) + "|" + Val(woDet.GarmentStyle) + "|"

                        //EXPEDITE_PRIORITY|DISCRETE_IND|ROW_NUMBER|CURR_FIN_LBS|
                        + woDet.ExpeditePriority + "|" + ("Y") + "|" + "1" + "|" + woDet.Lbs + "|" +

                        //STYLE_CD|SIZE_CD|MAKE_OR_BUY_CD|ATTRIBUTE_CD|COLOR_CD|GARMENT_STYLE|SELLING_STYLE_CD|SELLING_COLOR_CD|SELLING_ATTRIBUTE_CD|SELLING_SIZE_CD|
                        Val(woDet.Style) + "|" + Val(woDet.Size) + "|" + Val(woDet.MakeOrBuy) + "|" + (woDet.Attribute) + "|" + Val(woDet.Color) + "|" + Val(woDet.GarmentStyle) + "|" + Val(woDet.SellingStyle) + "|" + Val(woDet.SellingColor) + "|" + Val(woDet.SellingAttribute) + "|" + Val(woDet.SellingSize) + "|"
                        //MFG_REVISION_NO
                        + woDet.Revision + "|" + Val(woDet.DemandSource) + "|" + Val(woDet.DemandDriver) + "') from dual ");
                }
                else
                {
                    queryBuilder.Append("SELECT oprsql.iss_prod_order_validate.verify(35,'SUPER_ORDER|ORDER_VERSION|PRODUCTION_STATUS|DOZENS_ONLY_IND|CREATE_BD_IND|PRIORITY|SPREAD_TYPE_CD|SEW_PATH|CUT_PATH|TXT_PATH|MACHINE|DEMAND_LOC|PACK_CD|CATEGORY_CD|CUTTING_ALT|DOZENS_ONLY_IND|CREATE_BD_IND|GARMENT_STYLE|EXPEDITE_PRIORITY|DISCRETE_IND|ROW_NUMBER|CURR_FIN_LBS|STYLE_CD|SIZE_CD|MAKE_OR_BUY_CD|ATTRIBUTE_CD|COLOR_CD|GARMENT_STYLE|SELLING_STYLE_CD|SELLING_COLOR_CD|SELLING_ATTRIBUTE_CD|SELLING_SIZE_CD|MFG_REVISION_NO|DEMAND_SOURCE|DEMAND_DRIVER|','" +
                        //SUPER_ORDER|ORDER_VERSION|PRODUCTION_STATUS|DOZENS_ONLY_IND|CREATE_BD_IND|
                        Val(woDet.SuperOrder) + "|" + "1" + "|" + Val(woDet.OrderStatus) + "|" + Val(woDet.DozensOnlyInd) + "|" + Val(woDet.CreateBDInd) + "|" +

                        //PRIORITY|SPREAD_TYPE_CD|SEW_PATH|CUT_PATH|
                        woDet.Priority + "|" + Val(null) + "|" + Val(woDet.MfgPathId) + "|" + Val(woDet.CutPath)

                      //TXT_PATH|MACHINE|DEMAND_LOC|
                     + "|" + Val(woDet.TxtPath) + "|" + Val(woDet.MC) + "|" + Val(woDet.DcLoc)
                        //PACK_CD|CATEGORY_CD|CUTTING_ALT|DOZENS_ONLY_IND|CREATE_BD_IND|GARMENT_STYLE|
                        + "|" + Val(woDet.PackCode) + "|" + Val(woDet.CategoryCode) + "|" + Val(woDet.AltId) + "|" + Val(woDet.DozensOnlyInd) + "|" + Val(woDet.CreateBDInd) + "|" + Val(woDet.GarmentStyle) + "|"

                        //EXPEDITE_PRIORITY|DISCRETE_IND|ROW_NUMBER|CURR_FIN_LBS|
                       + woDet.ExpeditePriority + "|" + ("Y") + "|" + "1" + "|" + woDet.Lbs + "|" +

                        //STYLE_CD|SIZE_CD|MAKE_OR_BUY_CD|ATTRIBUTE_CD|COLOR_CD|GARMENT_STYLE|SELLING_STYLE_CD|SELLING_COLOR_CD|SELLING_ATTRIBUTE_CD|SELLING_SIZE_CD|
                        Val(woDet.Style) + "|" + Val(woDet.Size) + "|" + Val(woDet.MakeOrBuy) + "|" + (woDet.Attribute) + "|" + Val(woDet.Color) + "|" + Val(woDet.GarmentStyle) + "|" + Val(woDet.SellingStyle) + "|" + Val(woDet.SellingColor) + "|" + Val(woDet.SellingAttribute) + "|" + Val(woDet.SellingSize) + "|"
                        //MFG_REVISION_NO
                        + woDet.Revision + "|" + Val(woDet.DemandSource) + "|" + Val(woDet.DemandDriver) + "') from dual ");
                }
                System.Diagnostics.Debug.WriteLine(queryBuilder.ToString());
                var result = (String)ExecuteScalar(queryBuilder.ToString());
                //||||||||||||||||||||||||||||||||||||||
                ErrMsg = (result != null) ? result.Replace("|", "\n").Trim() : String.Empty;


                if (!String.IsNullOrEmpty(ErrMsg))
                    ErrId = 1; // error

                else
                {
                    ErrId = -1;


                }
            }
            catch (OracleException ee) {
                ErrId = 1;
                ErrMsg = ee.Message;
            }
            return String.IsNullOrEmpty(ErrMsg);
        }

        public bool ExternalSku(string style, string color, string attribute)
        {

            string query = "select COUNT(*) from EXTERNAL_SKU_MASTER where selling_style_cd='" + Val(style) + "' and selling_color_cd = '" + Val(color) + "' and selling_attribute_cd = '" + Val(attribute) + "'  and customer_cd='HAA'";
            var skucount = ExecuteScalar(query);
            if (Convert.ToInt32(skucount) > 0)
                return true;
            else
                return false;

        }
        
    }

}
