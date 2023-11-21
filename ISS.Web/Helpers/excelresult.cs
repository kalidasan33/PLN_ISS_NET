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
    public class ExcelResult : ActionResult
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

        public ExcelResult(string fileName)
        {
            this.FileName = fileName;
            excelpackage = new ExcelPackage();
        }

        public ExcelResult AddSheet<T>(IEnumerable<T> list, string sheetName, string[] excelColumns)
        {
            int ordinal = 0;
            ExcelWorksheet sheet = excelpackage.Workbook.Worksheets.Add(sheetName);

            var prop= typeof(T).GetProperties();
            foreach (var column in excelColumns)
            {
                var pi = prop.FirstOrDefault(r => r.Name == column);
                if (pi!=null)
                {
                    var browsableAttribute = pi.GetCustomAttributes(typeof(BrowsableAttribute), false).FirstOrDefault() as BrowsableAttribute;

                    if (browsableAttribute == null || browsableAttribute.Browsable)
                    {
                        ++ordinal;

                        sheet.Cells[1, ordinal].Value = WriteHeaderName(pi);
                        sheet.Cells[1, ordinal].Style.Font.Bold = true;

                        if (list.Count() > 0)
                        {

                            if (pi.PropertyType == typeof(DateTime) || pi.PropertyType == typeof(DateTime?))
                                sheet.Cells[2, ordinal].LoadFromCollection(list.Select(x => new { Value = (GetPropValue(x, pi.Name) != null ? Convert.ToDateTime(GetPropValue(x, pi.Name)).ToShortDateString() : GetPropValue(x, pi.Name)) }));
                            else
                                sheet.Cells[2, ordinal].LoadFromCollection(list.Select(x => new { Value = GetPropValue(x, pi.Name) }));

                           
                        }
                    }
                }
            }

            sheet.Cells[sheet.Dimension.Address].AutoFilter = true;
            sheet.Cells[sheet.Dimension.Address].AutoFitColumns();

            return this;
        }

        private static string WriteHeaderName(System.Reflection.PropertyInfo pi)
        {
            var displayAttribute = pi.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
            string headerName = displayAttribute != null ? displayAttribute.Name : pi.Name;
            return headerName;
        }

        static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

    }
}