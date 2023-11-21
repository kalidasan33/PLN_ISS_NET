using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Data;

namespace ISS.Web.Helpers
{
    public class ExcelDataTableResult : ActionResult
    {
     
        private ExcelPackage excelpackage;

        public ExcelPackage ExcelPackage { get { return excelpackage; } }

        public string FileName { get; private set; }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.AppendHeader("Content-type", "application/vnd.ms-excel");
            context.HttpContext.Response.AppendHeader("Content-disposition", string.Format("attachment; filename={0}.xlsx", this.FileName));
            byte[] a = excelpackage.GetAsByteArray();
            context.HttpContext.Response.OutputStream.Write(a, 0, a.Length);

        }

        public ExcelDataTableResult(string fileName)
        {
            this.FileName = fileName;
            excelpackage = new ExcelPackage();
        }

        public ExcelDataTableResult AddDataTableSheet(DataSet ds, string sheetName)
        {            
            bool isCapacity = sheetName.ToLower().Contains("capacity");
            ExcelWorksheet sheet = excelpackage.Workbook.Worksheets.Add(sheetName);
            int dtCount = 0;
            int previousCount = 0;
            foreach (DataTable dt in ds.Tables)
            {
                ++dtCount;
                var rowCount = dt.Rows.Count;
                int ordinal = 0;
                foreach (DataColumn column in dt.Columns)
                {
                    if (column != null)
                    {                       
                        ++ordinal;
                        var cellNo = dtCount > 1 ? previousCount + 2 : dtCount;
                        sheet.Cells[cellNo, ordinal].Value = WriteDataTableHeaderName(column.ColumnName, isCapacity);
                        sheet.Cells[cellNo, ordinal].Style.Font.Bold = true;

                        if (rowCount > 0)
                        {

                            if (dt.Columns[column.ColumnName].GetType() == typeof(DateTime) || dt.Columns[column.ColumnName].GetType() == typeof(DateTime?))
                                sheet.Cells[cellNo+1, ordinal].LoadFromCollection(dt.AsEnumerable().Select(x => new { Value = (GetDataRowValue(x, column.ColumnName) != null ? Convert.ToDateTime(GetDataRowValue(x, column.ColumnName)).ToShortDateString() : GetDataRowValue(x, column.ColumnName)) }));
                            else
                                sheet.Cells[cellNo+1, ordinal].LoadFromCollection(dt.AsEnumerable().Select(x => new { Value = GetDataRowValue(x, column.ColumnName) }));
                        }                      
                    }
                }

                sheet.Cells[sheet.Dimension.Address].AutoFilter = true;
                sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
                previousCount += rowCount;
            }
            return this;
        }

        private static string WriteDataTableHeaderName(string columnName, bool isCapacity)
        {
            if (isCapacity && columnName.Contains("_"))
            {
                DateTime dt = DateTime.ParseExact(columnName, "MMM_dd_yyyy", System.Globalization.CultureInfo.InvariantCulture);
                columnName = dt.ToString("MM/dd/yyyyy");
            }
            else
            {
                if (columnName.Contains("_"))
                {
                    columnName = columnName.Substring(1);
                }
                switch (columnName.ToLower())
                {
                    case "yarn":
                    case "yarnitem":
                        columnName = "Yarn Item";
                        break;
                    case "dye":
                        columnName = string.Empty;
                        break;
                    case "b":
                        columnName = "Bleach";
                        break;
                    case "d":
                        columnName = "Dye";
                        break;
                    case "plantcut":
                        columnName = "Plant/Cut";
                        break;
                    case "_n_a":
                        columnName = "N/A";
                        break;
                    case "headsize":
                        columnName = "Head Size";
                        break;
                }
            }
            return columnName;
        }

        static object GetDataRowValue(DataRow src, string colName)
        {
            string value = Convert.ToString(src[colName]);
            decimal outValue;

            if (decimal.TryParse(value, out outValue))
            {
                return Convert.ToDecimal(src[colName]);
            }
            else
            {
                return src[colName];
            }
        }
    }
}