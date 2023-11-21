using KA.Core.Model.MaterialSupply;
using OfficeOpenXml;
using OfficeOpenXml.Table.PivotTable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ISS.Web.Helpers
{
    public class PABExportExcel
    {
        public MemoryStream PABDetailsToExcel(List<MaterialPAB> lstPABHeader, List<MaterialPAB> lstPABData)
        {
            int rowIndex = 1, colIndex = 0;
            bool first = true;
            var fileStream = new MemoryStream();
            using (ExcelPackage excelpackage = new ExcelPackage())
            {
                ExcelWorksheet worksheet = excelpackage.Workbook.Worksheets.Add("PAB Data");

                worksheet.Cells[rowIndex, ++colIndex].Value = "SKU";
                worksheet.Cells[rowIndex, ++colIndex].Value = "DC";
                worksheet.Cells[rowIndex, ++colIndex].Value = "Size";
                worksheet.Cells[rowIndex, ++colIndex].Value = "PABType";
                worksheet.Cells[rowIndex, ++colIndex].Value = "Week";
                worksheet.Cells[rowIndex, ++colIndex].Value = "Qty";

                foreach (MaterialPAB hData in lstPABData)
                {
                    foreach (Dictionary<String, dynamic> dicVal in hData.SizeList)
                    {
                        var hValue = dicVal["PABList"];
                        foreach (var dicPab in hValue)
                        {
                        //for (int index = 0; index < hValue.Count; index++)
                        //{
                            //var dicPab = hValue.ElementAt(index);
                            rowIndex++;
                            colIndex = 0;
                            worksheet.Cells[rowIndex, 1].Value = dicVal["SKU"];
                            worksheet.Cells[rowIndex, 2].Value = dicVal["DC"];
                            worksheet.Cells[rowIndex, 3].Value = dicVal["Size"];
                            worksheet.Cells[rowIndex, 4].Value = dicPab["PABType"];
                            worksheet.Cells[rowIndex, 5].Value = "Total";
                            worksheet.Cells[rowIndex, 6].Value = dicPab["Total"];
                            
                            foreach (MaterialPAB header in lstPABHeader)
                            {
                               // if (first || dicPab[header.FiscalWeekStr] != 0)
                                {
                                    rowIndex++;
                                    colIndex = 0;
                                    worksheet.Cells[rowIndex, 1].Value = dicVal["SKU"];
                                    worksheet.Cells[rowIndex, 2].Value = dicVal["DC"];
                                    worksheet.Cells[rowIndex, 3].Value = dicVal["Size"];
                                    worksheet.Cells[rowIndex, 4].Value = dicPab["PABType"];
                                    worksheet.Cells[rowIndex, 5].Value = header.FiscalWeekTitle.Replace("<br />", "\n");
                                    worksheet.Cells[rowIndex, 6].Value = dicPab[header.FiscalWeekStr];
                                }
                            }
                            first = false;
                        }
                    }
                }

                int iColCnt = worksheet.Dimension.End.Column;
                int iRowCnt = worksheet.Dimension.End.Row;

                
                ExcelWorksheet worksheet2 = excelpackage.Workbook.Worksheets.Add("PAB");

                var pivotTable = worksheet2.PivotTables.Add(worksheet2.Cells["A2"], worksheet.Cells["A1:F" + iRowCnt], "pivTable");

                //Assign which Rows and Columns of A1:F10 are data or headers
                pivotTable.ShowHeaders = true;
                pivotTable.FirstHeaderRow = 1;
                pivotTable.FirstDataCol = 1;
                pivotTable.FirstDataRow = 2;
                pivotTable.ColumGrandTotals = false;
                pivotTable.RowGrandTotals = false;
               
                //pivotTable.ApplyWidthHeightFormats = false;

                //Row Labels
                pivotTable.RowFields.Add(pivotTable.Fields["SKU"]);
                pivotTable.RowFields.Add(pivotTable.Fields["DC"]);
                pivotTable.RowFields.Add(pivotTable.Fields["Size"]);
                pivotTable.RowFields.Add(pivotTable.Fields["PABType"]);
                pivotTable.DataOnRows = false;

                //Column Labels
                pivotTable.ColumnFields.Add(pivotTable.Fields["Week"]);

                //Values
                pivotTable.DataFields.Add(pivotTable.Fields["Qty"]);

                ExcelPivotTableField field = pivotTable.Fields["Size"];
                field.SubtotalTop = false;
                field.SubTotalFunctions = eSubTotalFunctions.None;

                ExcelPivotTableField dcfield = pivotTable.Fields["DC"];
                dcfield.SubtotalTop = false;
                dcfield.SubTotalFunctions = eSubTotalFunctions.None;

                ExcelPivotTableField skufield = pivotTable.Fields["SKU"];
                skufield.SubtotalTop = false;
                skufield.SubTotalFunctions = eSubTotalFunctions.None;

                excelpackage.Workbook.Worksheets.MoveToStart("PAB");

                excelpackage.SaveAs(fileStream);
                fileStream.Position = 0;
            }
            return fileStream;
        }


        /*
        public MemoryStream PABDetailsToExcel(List<MaterialPAB> lstPABHeader, List<Dictionary<String, dynamic>> lstPABData)
        {
            int rowIndex = 1, colIndex = 2;
            
            var fileStream = new MemoryStream();
            using (ExcelPackage excelpackage = new ExcelPackage())
            {
                ExcelWorksheet worksheet = excelpackage.Workbook.Worksheets.Add("PAB");

                worksheet.Cells[rowIndex, 1].Value = "PAB Type";
                worksheet.Cells[rowIndex, 2].Value = "Total";
                foreach (MaterialPAB header in lstPABHeader)
                {
                    worksheet.Cells[rowIndex, ++colIndex].Value = header.FiscalWeekTitle.Replace("<br />", "\n");
                    worksheet.Column(colIndex).Width = 12;
                }
                worksheet.Cells[rowIndex, 1, rowIndex, 54].Style.Font.Bold = true;
                worksheet.Cells[rowIndex, 1, rowIndex, 54].Style.WrapText = true;

                foreach (Dictionary<String, dynamic> dicVal in lstPABData)
                {
                    rowIndex++;
                    colIndex = 0;
                    foreach (var firstRowCell in worksheet.Cells[worksheet.Dimension.Start.Row, worksheet.Dimension.Start.Column, 1, worksheet.Dimension.End.Column])
                    {
                        var hKey = firstRowCell.Text.Split('\n');
                        if (hKey.Length > 0)
                        {
                            var hValue = dicVal[hKey[0].Replace(" ", String.Empty)];
                            worksheet.Cells[rowIndex, ++colIndex].Value = hValue;
                        }
                    }
                   
                }
                
                excelpackage.SaveAs(fileStream);
                fileStream.Position = 0;
            }
            return fileStream;
        }
         */   
    }
}