using ISS.Core.Model.Order;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using ISS.Common;

namespace ISS.Web.Helpers
{
    public class RequisitionExpandViewExport
    {
        public MemoryStream ExportRequisitionExpandView(List<Requisition> lstReqHeader, List<RequisitionExpandView> lstReqExView)
        {
            int rowIndex = 1, gRowIndex = 1;
            var fileStream = new MemoryStream();
            bool isMfgStart = false;

            using (ExcelPackage excelpackage = new ExcelPackage())
            {
                ExcelWorksheet worksheet = excelpackage.Workbook.Worksheets.Add("Source Order - Expanded View");

                foreach (Requisition headerReq in lstReqHeader)
                {
                    worksheet.Cells[rowIndex, 1].Value = "Requisition Number: ";
                    worksheet.Cells[rowIndex, 1].Style.Font.Bold = true;
                    worksheet.Cells[rowIndex, 2].Value = headerReq.RequisitionId;

                    worksheet.Cells[rowIndex, 4].Value = "Business Unit: ";
                    worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;
                    worksheet.Cells[rowIndex, 5].Value = headerReq.CropBusinessUnit;
                    rowIndex++;

                    worksheet.Cells[rowIndex, 1].Value = "Create Date: ";
                    worksheet.Cells[rowIndex, 1].Style.Font.Bold = true;
                    worksheet.Cells[rowIndex, 2].Value = ((headerReq.CreatedOn.HasValue) ? headerReq.CreatedOn.Value.ToString(LOVConstants.DateFormatDisplay) : String.Empty);


                    worksheet.Cells[rowIndex, 4].Value = "Status: ";
                    worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;
                    worksheet.Cells[rowIndex, 5].Value = headerReq.ReqStatusDesc;
                    rowIndex++;

                    worksheet.Cells[rowIndex, 1].Value = "Last Update: ";
                    worksheet.Cells[rowIndex, 1].Style.Font.Bold = true;
                    worksheet.Cells[rowIndex, 2].Value = ((headerReq.UpdatedOn.HasValue) ? headerReq.UpdatedOn.Value.ToString(LOVConstants.DateFormatDisplay) : String.Empty);
                    
                    worksheet.Cells[rowIndex, 4].Value = "Updated By: ";
                    worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;
                    worksheet.Cells[rowIndex, 5].Value = headerReq.UpdatedBy;

                    worksheet.Cells[rowIndex, 7].Value = "Planned Ex Factory Date: ";
                    worksheet.Cells[rowIndex, 7].Style.Font.Bold = true;
                    worksheet.Cells[rowIndex, 8].Value = headerReq.PlannedDcDate.ToShortDateString();
                    rowIndex++;

                    worksheet.Cells[rowIndex, 1].Value = "Planning Contact: ";
                    worksheet.Cells[rowIndex, 1].Style.Font.Bold = true;
                    worksheet.Cells[rowIndex, 2].Value = headerReq.PlannerName;


                    worksheet.Cells[rowIndex, 4].Value = "DC Location: ";
                    worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;
                    worksheet.Cells[rowIndex, 5].Value = headerReq. DcLocName;
                    rowIndex++;

                    worksheet.Cells[rowIndex, 1].Value = "Sourcing Contact: ";
                    worksheet.Cells[rowIndex, 1].Style.Font.Bold = true;
                    worksheet.Cells[rowIndex, 2].Value = headerReq.SourcingContactName;


                    worksheet.Cells[rowIndex, 4].Value = "Season: ";
                    worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;
                    worksheet.Cells[rowIndex, 5].Value = headerReq.Season;
                    rowIndex++;

                    worksheet.Cells[rowIndex, 1].Value = "Requisition Approver: ";
                    worksheet.Cells[rowIndex, 1].Style.Font.Bold = true;
                    worksheet.Cells[rowIndex, 2].Value = headerReq.RequisitionApprover;


                    worksheet.Cells[rowIndex, 4].Value = "Program Type: ";
                    worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;
                    worksheet.Cells[rowIndex, 5].Value = headerReq.ProType;
                    rowIndex++;

                    worksheet.Cells[rowIndex, 1].Value = "Submitted for Approval: ";
                    worksheet.Cells[rowIndex, 1].Style.Font.Bold = true;
                    worksheet.Cells[rowIndex, 2].Value = headerReq.ApprovalSubmitted;


                    worksheet.Cells[rowIndex, 4].Value = "Approved: ";
                    worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;
                    worksheet.Cells[rowIndex, 5].Value = headerReq.Approved;
                    rowIndex++;


                    worksheet.Cells[rowIndex, 1].Value = "Vendor: ";
                    worksheet.Cells[rowIndex, 1].Style.Font.Bold = true;
                    worksheet.Cells[rowIndex, 2].Value = headerReq.VendorName;


                    worksheet.Cells[rowIndex, 4].Value = "Mode: ";
                    worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;
                    worksheet.Cells[rowIndex, 5].Value = headerReq.Mode;

                    worksheet.Cells[rowIndex, 7].Value = "Over %: ";
                    worksheet.Cells[rowIndex, 7].Style.Font.Bold = true;
                    worksheet.Cells[rowIndex, 8].Value = headerReq.OverPercentage;

                    worksheet.Cells[rowIndex, 10].Value = "Under %: ";
                    worksheet.Cells[rowIndex, 10].Style.Font.Bold = true;
                    worksheet.Cells[rowIndex, 11].Value = headerReq.UnderPercentage;
                    rowIndex++;

                   // int commentIndex = rowIndex;
                    worksheet.Cells[rowIndex, 1].Value = "Planner Comment: ";
                    worksheet.Cells[rowIndex, 1].Style.Font.Bold = true;
                    if (headerReq.RequisitionComment.PlannerComment != null)
                    {
                        //string[] aComments = headerReq.RequisitionComment.ApproverComment.Split('\n');
                        //var strr = "";
                        //foreach (string comnt in aComments)
                        //{
                        //    strr +=comnt;
                       // commentIndex++;
                        //}
                        worksheet.Cells[rowIndex, 2].Value = headerReq.RequisitionComment.PlannerComment;
                    }
                    //worksheet.Cells[rowIndex, 2].Value = headerReq.RequisitionComment.PlannerComment;

                    //commentIndex = rowIndex;
                    worksheet.Cells[rowIndex, 4].Value = "Approver Comment: ";
                    worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;
                    if (headerReq.RequisitionComment.ApproverComment != null)
                    {
                        //string[] aComments = headerReq.RequisitionComment.ApproverComment.Split('\n');
                        //var strr = "";
                        //foreach (string comnt in aComments)
                        //{
                        //    strr +=comnt;
                     //   commentIndex++;
                        //}
                        worksheet.Cells[rowIndex, 5].Value = headerReq.RequisitionComment.ApproverComment;
                    }
                    //worksheet.Cells[rowIndex, 5].Value = headerReq.RequisitionComment.ApproverComment;
                   
                    rowIndex++;

                }

                rowIndex++;
                gRowIndex = rowIndex++;

                worksheet.Cells[rowIndex, 1].Value = "";
                worksheet.Cells[rowIndex, 1].Style.Font.Bold = true;

                worksheet.Cells[rowIndex, 2].Value = "Style";
                worksheet.Cells[rowIndex, 2].Style.Font.Bold = true;

                worksheet.Cells[rowIndex, 3].Value = "Color";
                worksheet.Cells[rowIndex, 3].Style.Font.Bold = true;

                worksheet.Cells[rowIndex, 4].Value = "Attribute";
                worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;

                worksheet.Cells[rowIndex, 5].Value = "Size";
                worksheet.Cells[rowIndex, 5].Style.Font.Bold = true;

                worksheet.Cells[rowIndex, 6].Value = "Rev";
                worksheet.Cells[rowIndex, 6].Style.Font.Bold = true;

                worksheet.Cells[rowIndex, 7].Value = "UM";
                worksheet.Cells[rowIndex, 7].Style.Font.Bold = true;

                worksheet.Cells[rowIndex, 8].Value = "Qty";
                worksheet.Cells[rowIndex, 8].Style.Font.Bold = true;

                worksheet.Cells[rowIndex, 9].Value = "Eaches";
                worksheet.Cells[rowIndex, 9].Style.Font.Bold = true;

                worksheet.Cells[rowIndex, 10].Value = "Cases";
                worksheet.Cells[rowIndex, 10].Style.Font.Bold = true;

                using (var range = worksheet.Cells[rowIndex, 1, rowIndex, 10])
                {
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                }
                rowIndex++;

                //foreach (RequisitionExpandView headerReq in lstReqExView)
                for (int i = 0; i < lstReqExView.Count; i++)
                {
                    RequisitionExpandView ExReq = lstReqExView[i];
                    worksheet.Cells[rowIndex, 1].Value = "SEL";
                    worksheet.Cells[rowIndex, 1].Style.Font.Bold = true;

                    worksheet.Cells[rowIndex, 2].Value = ExReq.Style;
                    worksheet.Cells[rowIndex, 2].Style.Font.Bold = true;

                    worksheet.Cells[rowIndex, 3].Value = ExReq.Color;
                    worksheet.Cells[rowIndex, 3].Style.Font.Bold = true;

                    worksheet.Cells[rowIndex, 4].Value = ExReq.Attribute;
                    worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;

                    worksheet.Cells[rowIndex, 5].Value = ExReq.SizeShortDesc;
                    worksheet.Cells[rowIndex, 5].Style.Font.Bold = true;

                    worksheet.Cells[rowIndex, 6].Value = ExReq.Revision;
                    worksheet.Cells[rowIndex, 6].Style.Font.Bold = true;

                    worksheet.Cells[rowIndex, 7].Value = ExReq.UOM;
                    worksheet.Cells[rowIndex, 7].Style.Font.Bold = true;

                    worksheet.Cells[rowIndex, 8].Value = ExReq.QtyStd;
                    worksheet.Cells[rowIndex, 8].Style.Font.Bold = true;

                    worksheet.Cells[rowIndex, 9].Value = ExReq.Eaches.RoundCustom(0);
                    worksheet.Cells[rowIndex, 9].Style.Font.Bold = true;

                    worksheet.Cells[rowIndex, 10].Value = ExReq.Cases;
                    worksheet.Cells[rowIndex, 10].Style.Font.Bold = true;
                    rowIndex++;

                    worksheet.Cells[rowIndex, 1].Value = ExReq.StyleFormat;
                    worksheet.Cells[rowIndex, 1, rowIndex, 10].Merge = true;
                    rowIndex++;

                    isMfgStart = false;
                    if (ExReq.BomComponents.Count > 0)
                    {
                        foreach (RequisitionBOM reqbom in ExReq.BomComponents)
                        {
                            if (isMfgStart)
                            {
                                worksheet.Cells[rowIndex, 1].Value = reqbom.StyleType;
                                worksheet.Cells[rowIndex, 2].Value = reqbom.Style;
                                worksheet.Cells[rowIndex, 3].Value = reqbom.StyleDesc;
                                rowIndex++;

                                BOMLevelStyle(reqbom, rowIndex, isMfgStart, worksheet, reqbom.Usage * ExReq.BomComponents.Where(r => (r.StyleType == LOVConstants.BOMStyle.PKG)).Sum(r=>r.Usage));
                                rowIndex++;
                            }
                            else
                            {
                                BOMLevelStyle(reqbom, rowIndex, isMfgStart, worksheet, reqbom.Usage * ExReq.BomComponents.Where(r => (r.StyleType == LOVConstants.BOMStyle.PKG)).Sum(r => r.Usage));
                                rowIndex++;
                            }

                            if (reqbom.StyleType == LOVConstants.BOMStyle.PKG)
                            {
                                isMfgStart = true;
                            }
                            else
                            {
                                isMfgStart = false;
                            }

                        }
                    }

                    if ((i == lstReqExView.Count - 1) || (lstReqExView[i + 1].Style != ExReq.Style))
                    {
                        worksheet.Cells[rowIndex, 1, rowIndex, 10].Merge = true;
                        worksheet.Cells[rowIndex, 1].Value = "**Style " + ExReq.Style + " Total DZ : " + lstReqExView.Where(r => r.Style == ExReq.Style).Sum(r => r.Qty).ToString("0.00") + " (" + lstReqExView.Where(r => r.Style == ExReq.Style).Sum(r => r.Eaches) + " eaches) Number of Cases : " + lstReqExView.Where(r => r.Style == ExReq.Style).Sum(r => r.Cases);
                        
                        worksheet.Cells[rowIndex, 1].Style.Font.Bold = true;
                        rowIndex++;
                    }
                }


                worksheet.Cells[gRowIndex, 1, gRowIndex, 10].Merge = true;
                worksheet.Cells[gRowIndex, 1].Value = "**Grand Total DZ : " + lstReqExView.Sum(r => r.Qty).ToString("0.00") + " (" + lstReqExView.Sum(r => r.Eaches) + " eaches) Number of Cases : " + lstReqExView.Sum(r => r.Cases);
           
                worksheet.Cells[gRowIndex, 1].Style.Font.Bold = true;
                rowIndex++;
 
                //excelpackage.Save();

                excelpackage.SaveAs(fileStream);
                fileStream.Position = 0;
            }

            return fileStream;

        }

        public void BOMLevelStyle(RequisitionBOM reqbom, int rowIndex, bool isMfgStart, ExcelWorksheet worksheet, decimal qty)
        {
            if (!isMfgStart && (reqbom.StyleType == LOVConstants.BOMStyle.PKG))
            {
                worksheet.Cells[rowIndex, 1].Value = reqbom.StyleType;
                worksheet.Cells[rowIndex, 2].Value = reqbom.Style;
                worksheet.Cells[rowIndex, 3].Value = reqbom.Color;
                worksheet.Cells[rowIndex, 8].Value = qty.ToString("0.00");
            }
            else
            {
                worksheet.Cells[rowIndex, 3].Value = reqbom.Color + "(" + reqbom.ColorDesc + ")";
            }

            worksheet.Cells[rowIndex, 3].Value = reqbom.Color;
            worksheet.Cells[rowIndex, 4].Value = reqbom.Attribute;
            worksheet.Cells[rowIndex, 5].Value = reqbom.SizeAbb;
            worksheet.Cells[rowIndex, 6].Value = reqbom.Revision;
            worksheet.Cells[rowIndex, 9].Value = reqbom.DisplayCases;

        }

    
    }
}