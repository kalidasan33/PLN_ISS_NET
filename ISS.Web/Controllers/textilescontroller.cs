using ISS.BusinessRules.Contract.Textiles;
using ISS.Common;
using ISS.Core.Model.Textiles;
using ISS.Web.Helpers;
using Kendo.Mvc.UI;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;


namespace ISS.Web.Controllers
{
    public partial class TextilesController : BaseController
    {

        private readonly ITextilesService textilesService;
        private string _columnFormat = "_{0}";
        private string _machineValueFormat = "{0}/{1}";
        private const string VIEW_NAME = "_headsizedetails";

        public TextilesController(ITextilesService textilesService)
        {
            this.textilesService = textilesService;
        }

        [HttpGet]
        public ActionResult Detail(TextilesSearch search)
        {
            SessionHelper.Clear(SessionConstant.TEXTILE_ALLOCATIONS);            
            search.IsSuggestedLotIncluded = true;
            return View(search);
        }

        [HttpPost]
        public PartialViewResult GetTextileGridAllocation(TextilesSearch search)
        {
            
            string allocationGrid = search.AllocGrid;

            if (string.IsNullOrWhiteSpace(allocationGrid))
            {
                return null;
            }

            if (search.ViewName.ToLower() == VIEW_NAME)
            {
                SessionHelper.Textiles = textilesService.GetTextileAllocations(search);

                SessionHelper.FROMDATE = search.FromWYear;
            }

            var gridResults = ReturnGridValues(allocationGrid, SessionHelper.Textiles);
            var result = SetDynamicValues(gridResults, allocationGrid, search.IsSuggestedLotIncluded);            
            string partialView = (result != null && result.Rows.Count > 0) ? search.ViewName : "_noRecords";
            Response.Headers.Add("count", result != null ? result.Rows.Count.ToString() : "-1");
            return PartialView(partialView, result);
        }

        public JsonResult ClearFilters()
        {
            SessionHelper.Textiles = null;
            return null;
        }

        [HttpGet]
        public JsonResult GetFilters(string filter, string plant = null)
        {
            IList<string> source = new List<string>();
            IList<TextileAllocation> filterdValue = SessionHelper.Textiles;

            if ((filterdValue != null && filterdValue.Any()) && !string.IsNullOrWhiteSpace(filter))
            {
                if (!string.IsNullOrWhiteSpace(plant))
                {
                    string[] plants = plant.Split(',');
                    filterdValue = filterdValue.Where(p => plants.Contains(p.Plant))
                                               .ToList();
                }

                switch (filter.ToLower())
                {
                    case "plant":
                        source = filterdValue.Select(s => s.Plant)
                                            .Distinct()
                                            .OrderBy(o => o)
                                            .ToList();
                        break;
                    case "headsize":
                        source = filterdValue.Where(s => s.Alloc_Type == LOVConstants.AllocationType.CYL || s.Alloc_Type == LOVConstants.AllocationType.CYLMAC)
                                             .Select(s => Convert.ToString(s.Cylinder_Size))
                                            .Distinct()
                                            .OrderBy(o => o)
                                            .ToList();
                        break;
                    case "cut":
                        source = filterdValue.Where(s => s.Alloc_Type == LOVConstants.AllocationType.MAC || s.Alloc_Type == LOVConstants.AllocationType.CYLMAC)
                                            .Select(s => Convert.ToString(s.Machine_Cut))
                                            .Distinct()
                                            .OrderBy(o => o)
                                            .ToList();
                        break;
                    case "machine":
                        source = filterdValue.Where(s => !string.IsNullOrWhiteSpace(s.Machine_Type_CD))
                                            .Select(s => s.Machine_Type_CD)
                                            .OrderBy(o => o)
                                            .Distinct()
                                            .ToList();
                        break;
                    case "yarn":
                        source = filterdValue.Where(s => !string.IsNullOrWhiteSpace(s.Resource_ID))
                                            .Select(s => s.Resource_ID)
                                            .OrderBy(o => o)
                                            .Distinct()
                                            .ToList();
                        break;
                    case "dye":
                        source = filterdValue.Where(s => !string.IsNullOrWhiteSpace(s.Dye_Shade_CD))
                                            .Select(s => s.Dye_Shade_CD)
                                            .OrderBy(o => o)
                                            .Distinct()
                                            .ToList();
                        break;
                }
            }
            return Json(source, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public PartialViewResult GetFilteredDataSource([DataSourceRequest] DataSourceRequest request, TextileFilterModel model)
        {
            DataTable gridModel = getFilteredData(model);
            string partialView = (gridModel != null && gridModel.Rows.Count > 0) ? model.ViewName : "_noRecords";
            Response.Headers.Add("count", gridModel != null ? gridModel.Rows.Count.ToString() : "-1");
            return PartialView(partialView, gridModel);
        }

        private DataTable getFilteredData(TextileFilterModel model)
        {
            DataTable gridModel = new DataTable();
            if (SessionHelper.Textiles != null && model != null)
            {
                string[] plantFilters = string.IsNullOrWhiteSpace(model.PlantFilter) ? null : model.PlantFilter.Split(',');
                string[] headFilters = string.IsNullOrWhiteSpace(model.HeadsizeFilter) ? null : model.HeadsizeFilter.Split(',');
                string[] machineFilters = string.IsNullOrWhiteSpace(model.MachineFilter) ? null : model.MachineFilter.Split(',');
                string[] yarnFilters = string.IsNullOrWhiteSpace(model.YarnFilter) ? null : model.YarnFilter.Split(',');
                string[] cutFilters = string.IsNullOrWhiteSpace(model.CutSizeFilter) ? null : model.CutSizeFilter.Split(',');
                string[] dyeFilters = string.IsNullOrWhiteSpace(model.DyeFilter) ? null : model.DyeFilter.Split(',');

                IList<TextileAllocation> textile = SessionHelper.Textiles;
                if (plantFilters != null)
                {
                    textile = textile.Where(p => plantFilters.Contains(p.Plant))
                                     .ToList();
                }

                var gridResults = ReturnGridValues(model.AllocationGrid, textile);
                IList<List<TextileAllocation>> filteredResults = gridResults;               
                switch (model.AllocationGrid.ToLower())
                {
                    case LOVConstants.GridNames.HEAD_SIZE:

                        if (headFilters != null)
                        {
                            filteredResults = gridResults.Select(s => s.Where(p => headFilters.Contains(Convert.ToString(p.Cylinder_Size)))
                                                                       .ToList())
                                                         .ToList();
                        }
                        if (cutFilters != null)
                        {
                            filteredResults = filteredResults.Select(s => s.Where(p => cutFilters.Contains(Convert.ToString(p.Machine_Cut)))
                                                                           .ToList())
                                                             .ToList();
                        }

                        break;
                    case LOVConstants.GridNames.DYE_BLEACH:
                        if (dyeFilters != null)
                        {
                            filteredResults = gridResults.Select(s => s.Where(p => dyeFilters.Contains(p.Dye_Shade_CD))
                                                                       .ToList())
                                                         .ToList();
                        }

                        break;
                    case LOVConstants.GridNames.MACHINE:
                        if (machineFilters != null)
                        {
                            filteredResults = gridResults.Select(s => s.Where(p => machineFilters.Contains(p.Machine_Type_CD))
                                                                       .ToList())
                                                         .ToList();
                        }

                        break;
                    case LOVConstants.GridNames.YARN:
                    case LOVConstants.GridNames.YARN_ITEM:
                        if (yarnFilters != null)
                        {
                            filteredResults = gridResults.Select(s => s.Where(p => yarnFilters.Contains(p.Resource_ID))
                                                                       .ToList())
                                                         .ToList();
                        }

                        break;
                        //Newly Added
                    case LOVConstants.GridNames.FABRIC_ITEM:
                        filteredResults = gridResults;
                        break;
                    case LOVConstants.GridNames.PRINT:
                        filteredResults = gridResults;
                        break;
                        //End
                }

                gridModel = SetDynamicValues(filteredResults, model.AllocationGrid, model.IsSuggestIncluded);
            }
            return gridModel;
        }


        public ActionResult ExportOverviewAllocations(string gridValue, bool IsSuggestedLotIncluded, string filteredData)
        {
            DataSet ds = new DataSet();
            string gridName = string.Empty;
            string sheetName = "TextileGridsAllocations";
            string format = "Textile{0}Allocations";
            string fileFormat = "{0}" + DateTime.Now.ToString("yyyyMMddHHmmss");
           
            switch (gridValue.ToLower())
            {
                case LOVConstants.GridNames.HEAD_SIZE:
                    gridName = LOVConstants.GridNames.HEAD_SIZE;                    
                    sheetName = string.Format(format, "HeadSize");

                    break;
                case LOVConstants.GridNames.DYE_BLEACH_EXP:
                    gridName = LOVConstants.GridNames.DYE_BLEACH;
                    sheetName = string.Format(format, "DyeBleach");                    
                    break;
                case LOVConstants.GridNames.MACHINE:
                    gridName = LOVConstants.GridNames.MACHINE;
                    sheetName = string.Format(format, "Machine");                   
                    break;               
                case LOVConstants.GridNames.YARN_ITEM:
                    gridName = LOVConstants.GridNames.YARN_ITEM;
                    sheetName = string.Format(format, "YarnItem");                   
                    break;
            }
            
            DataTable dtResult = null;
            if (!string.IsNullOrEmpty(filteredData))
            {
                TextileFilterModel search = JsonConvert.DeserializeObject<TextileFilterModel>(filteredData);
                search.AllocationGrid = gridName;                
                dtResult = getFilteredData(search);
            }
            else
            {
               var result =  ReturnGridValues(gridName, SessionHelper.Textiles);
               dtResult = SetDynamicValues(result, gridName, IsSuggestedLotIncluded);
            }

            if (dtResult != null)
            {
                ds.Tables.Add(dtResult);
            }
            return new ExcelDataTableResult(string.Format(fileFormat, sheetName)).AddDataTableSheet(ds, sheetName);
            
        }
        #region Private Methods

        private IList<List<TextileAllocation>> ReturnGridValues(string allocGrid, IList<TextileAllocation> result)
        {
            if (result == null) { return null; }
            var machine = result.Where(s => s.Alloc_Type == LOVConstants.AllocationType.MAC || s.Alloc_Type == LOVConstants.AllocationType.CYLMAC)
                                         .GroupBy(g => g.Plant)
                                         .Select(s => s.ToList())
                                         .ToList();
            var dye = result.Where(s => s.Alloc_Type == LOVConstants.AllocationType.DYE)
                                         .GroupBy(g => g.Plant)
                                         .Select(s => s.ToList())
                                         .ToList();
            var yarn = result.Where(s => s.Alloc_Type == LOVConstants.AllocationType.YARN)
                                         .GroupBy(g => g.Plant)
                                         .Select(s => s.ToList())
                                         .ToList();
            //Newly Added
            var fabric = result.Where(s => s.Alloc_Type == LOVConstants.AllocationType.FAB)
                                         .GroupBy(g => g.Plant)
                                         .Select(s => s.ToList())
                                         .ToList();

            var print = result.Where(s => s.Alloc_Type == LOVConstants.AllocationType.PRT)
                                         .GroupBy(g => g.Plant)
                                         .Select(s => s.ToList())
                                         .ToList();
            //End
            var tempResult = result.Where(s => s.Alloc_Type == LOVConstants.AllocationType.CYL || s.Alloc_Type == LOVConstants.AllocationType.CYLMAC);
            tempResult.ToList().ForEach(s => s.PlantCut = s.Plant + " " + s.Machine_Cut);
            var head = tempResult.GroupBy(g => g.PlantCut)
                                 .Select(s => s.ToList())
                                 .ToList();
            IList<List<TextileAllocation>> textiles = new List<List<TextileAllocation>>();
            if (!string.IsNullOrWhiteSpace(allocGrid))
            {
                switch (allocGrid.ToLower())
                {
                    case LOVConstants.GridNames.HEAD_SIZE:
                        textiles = head;
                        break;
                    case LOVConstants.GridNames.DYE_BLEACH:
                        textiles = dye;
                        break;
                    case LOVConstants.GridNames.MACHINE:
                        textiles = machine;
                        break;
                    case LOVConstants.GridNames.YARN:
                    case LOVConstants.GridNames.YARN_ITEM:
                        textiles = yarn;
                        break;
                //Newly Added
                    case LOVConstants.GridNames.FABRIC_ITEM:
                        textiles = fabric;
                        break;
                    case LOVConstants.GridNames.PRINT:
                        textiles = print;
                        break;
                //End
                }
            }

            return textiles;
        }

        private DataTable SetDynamicValues(IList<List<TextileAllocation>> textAllocations, string gridName, bool isSuggestInclude = true)
        {
            if (textAllocations == null) { return null; }

            DataTable dt = new DataTable();

            decimal totalSuggestedFabric = 0;
            decimal totalLockedFabric = 0;
            decimal totalReleasedFabric = 0;
            decimal totalPlantsTotal = 0;
            decimal totalPlantAlloc = 0;
            decimal totalPlantNet = 0;
            int textAllocationCount = 0;

            if (string.Compare(gridName, "headsize", true) == 0)
            {
                dt.Columns.Add("PlantCut");
            }
            else
            {
                dt.Columns.Add("Plant");
            }
            var cylinderSizes = textAllocations.SelectMany(s => s.Select(d => d.Cylinder_Size))
                                                .OrderBy(s => s)
                                                .Distinct();
            var dyeShades = textAllocations.SelectMany(s => s.Select(d => d.Dye_Shade_CD))
                                           .Where(s => !string.IsNullOrWhiteSpace(s))
                                           .OrderBy(s => s)
                                           .Distinct();
            var machine = textAllocations.SelectMany(s => s.Select(m => m.Machine_Type_CD))
                                        .Where(s => !string.IsNullOrWhiteSpace(s))
                                        .Distinct();
            var yarns = textAllocations.SelectMany(s => s.Select(r => r.Resource_ID))
                                        .OrderBy(s => s)
                                        .Distinct();

            foreach (var item in textAllocations)
            {
                if (item != null && item.Any())
                {
                    string plant = string.Compare(gridName, "headsize", true) == 0 ? item.FirstOrDefault().PlantCut : item.FirstOrDefault().Plant;

                    var suggested = isSuggestInclude ? item.Where(s => string.Compare(s.Production_Status, LOVConstants.ProductionStatus.TextileSuggested, true) == 0) : new List<TextileAllocation>();
                    var locked = item.Where(s => string.Compare(s.Production_Status, LOVConstants.ProductionStatus.Locked, true) == 0);
                    var released = item.Where(s => string.Compare(s.Production_Status, LOVConstants.ProductionStatus.Released, true) == 0);
                    var allocated = item.Where(s => string.Compare(s.Production_Status, LOVConstants.ProductionStatus.Allocated, true) == 0);

                    switch (gridName.ToLower())
                    {
                        case LOVConstants.GridNames.HEAD_SIZE:

                            if (!dt.Columns.Contains("HeadSize")) { dt.Columns.Add("HeadSize"); };

                            foreach (var size in cylinderSizes)
                            {
                                if (!dt.Columns.Contains(string.Format(_columnFormat, size))) { dt.Columns.Add(string.Format(_columnFormat, size)); };
                            }
                            if (!dt.Columns.Contains("Total"))
                            {
                                dt.Columns.Add("Total");
                            };

                            var sugHead = suggested.Where(s => (s.Alloc_Type == LOVConstants.AllocationType.CYL || s.Alloc_Type == LOVConstants.AllocationType.CYLMAC));
                            var lockHead = locked.Where(s => (s.Alloc_Type == LOVConstants.AllocationType.CYL || s.Alloc_Type == LOVConstants.AllocationType.CYLMAC));
                            var relHead = released.Where(s => (s.Alloc_Type == LOVConstants.AllocationType.CYL || s.Alloc_Type == LOVConstants.AllocationType.CYLMAC));
                            var allocHead = allocated.Where(s => (s.Alloc_Type == LOVConstants.AllocationType.CYL || s.Alloc_Type == LOVConstants.AllocationType.CYLMAC));

                            IEnumerable<TextileAllocation> textileAllocation = null;
                            decimal grandTotal = 0;
                            decimal totalAlloc = 0;
                            for (int i = 1; i < 7; i++)
                            {
                                DataRow row = dt.NewRow();
                                switch (i)
                                {
                                    //Suggested values
                                    case 1:
                                        row["PlantCut"] = plant;
                                        row["HeadSize"] = "Suggested";
                                        textileAllocation = sugHead;

                                        break;
                                    //Locked
                                    case 2:
                                        row["HeadSize"] = "Locked";
                                        textileAllocation = lockHead;

                                        break;
                                    case 3://Released
                                        row["HeadSize"] = "Released";
                                        textileAllocation = relHead;

                                        break;
                                    case 4://Total
                                        row["HeadSize"] = "Total";
                                        textileAllocation = null;
                                        break;
                                    case 5://Alloc
                                        row["HeadSize"] = "Alloc";
                                        textileAllocation = allocHead;
                                        break;
                                    case 6://Net
                                        row["HeadSize"] = "Net";
                                        row["Total"] = (totalAlloc - grandTotal).ToNumberString();
                                        break;

                                }
                                decimal total = 0;                               
                                foreach (var size in cylinderSizes)
                                {
                                    decimal columnSum = 0;
                                    decimal columnAlloc = 0;
                                    decimal headAlloc = 0;
                                    if (i <= 3)
                                    {
                                        if (textileAllocation != null && textileAllocation.Any())
                                        {
                                            var temp = textileAllocation.Where(s => s.Cylinder_Size == size);
                                            headAlloc = (temp != null) ? temp.Sum(s => s.LBS).RoundCustom(0) : 0;
                                        }
                                        row[string.Format(_columnFormat, size)] = headAlloc.ToNumberString();
                                        total += headAlloc;
                                    }
                                    else
                                    {
                                        if (textileAllocation != null && textileAllocation.Any())
                                        {
                                            var result = textileAllocation.Where(s => s.Cylinder_Size == size);
                                            columnAlloc = result != null ? result.Sum(s => s.LBS).RoundCustom(0) : 0;
                                        }

                                        var Value = item.Where(s => (s.Alloc_Type == LOVConstants.AllocationType.CYL || s.Alloc_Type == LOVConstants.AllocationType.CYLMAC) && !s.Production_Status.Equals(LOVConstants.ProductionStatus.Allocated, StringComparison.InvariantCultureIgnoreCase) && s.Cylinder_Size == size);
                                        if (!isSuggestInclude)
                                        {
                                            Value = Value.Where(y => string.Compare(y.Production_Status, LOVConstants.ProductionStatus.Locked, true) == 0 || string.Compare(y.Production_Status, LOVConstants.ProductionStatus.Released, true) == 0);
                                        }
                                        columnSum = Value.Sum(s => s.LBS).RoundCustom(0);
                                        totalAlloc += columnAlloc;

                                        row[string.Format(_columnFormat, size)] = (i == 4) ? columnSum.ToNumberString() : (i == 5) ? columnAlloc.ToNumberString() : (columnAlloc - columnSum).ToNumberString();
                                    }
                                }
                                if (i <= 3)
                                {
                                    row["Total"] = total.ToNumberString();
                                    grandTotal += total;
                                }
                                else
                                {
                                    if (i == 4)
                                    {
                                        row["Total"] = grandTotal.ToNumberString();
                                    }
                                    else if (i == 5)
                                    {
                                        row["Total"] = totalAlloc.ToNumberString();
                                    }
                                }
                                dt.Rows.Add(row);
                            }
                            if (textAllocations.Count > 1) { dt.Rows.Add(dt.NewRow()); }
                            break;
                        case LOVConstants.GridNames.DYE_BLEACH:
                            if (!dt.Columns.Contains("Dye")) { dt.Columns.Add("Dye"); };

                            foreach (var dye in dyeShades)
                            {
                                if (!dt.Columns.Contains(dye))
                                {
                                    dt.Columns.Add(dye);
                                }
                            }
                            if (!dt.Columns.Contains("Total"))
                            {
                                dt.Columns.Add("Total");
                            };

                            var sugDye = suggested.Where(s => (s.Alloc_Type == LOVConstants.AllocationType.DYE));
                            var lockDye = locked.Where(s => (s.Alloc_Type == LOVConstants.AllocationType.DYE));
                            var relDye = released.Where(s => (s.Alloc_Type == LOVConstants.AllocationType.DYE));
                            var allocatedDye = allocated.Where(s => (s.Alloc_Type == LOVConstants.AllocationType.DYE));

                            IEnumerable<TextileAllocation> textileDyeAllocation = null;
                            decimal grandDyeTotal = 0;
                            decimal totalDyeAlloc = 0;
                            for (int i = 1; i < 7; i++)
                            {
                                DataRow row = dt.NewRow();
                                switch (i)
                                {
                                    //Suggested values
                                    case 1:
                                        row["Plant"] = plant;
                                        row["Dye"] = "Suggested";
                                        textileDyeAllocation = sugDye;

                                        break;
                                    //Locked
                                    case 2:
                                        row["Dye"] = "Locked";
                                        textileDyeAllocation = lockDye;

                                        break;
                                    case 3://Released
                                        row["Dye"] = "Released";
                                        textileDyeAllocation = relDye;

                                        break;
                                    case 4://Total
                                        row["Dye"] = "Total";
                                        textileDyeAllocation = null;
                                        break;
                                    case 5://Alloc
                                        row["Dye"] = "Alloc";
                                        textileDyeAllocation = allocatedDye;
                                        break;
                                    case 6://Net
                                        row["Dye"] = "Net";
                                        row["Total"] = (totalDyeAlloc - grandDyeTotal).ToNumberString();

                                        break;

                                }
                                decimal dyeTotal = 0;
                                foreach (var dye in dyeShades)
                                {
                                    decimal columnSum = 0;
                                    decimal columnAlloc = 0;
                                    decimal dyeAlloc = 0;
                                    
                                    if (i <= 3)
                                    {
                                        if (textileDyeAllocation != null && textileDyeAllocation.Any())
                                        {
                                            var dyeAllocation = textileDyeAllocation.Where(s => s.Dye_Shade_CD == dye);
                                            dyeAlloc = dyeAllocation != null ? dyeAllocation.Sum(s => s.LBS.RoundCustom(0)) : 0;                                           
                                        }
                                        row[dye] = dyeAlloc.ToNumberString();
                                        dyeTotal += dyeAlloc;
                                    }
                                    else
                                    {
                                        if (textileDyeAllocation != null && textileDyeAllocation.Any())
                                        {
                                            var result = textileDyeAllocation.Where(s => s.Dye_Shade_CD == dye);
                                            columnAlloc = result != null ? result.Sum(s => s.LBS.RoundCustom(0)) : 0;
                                            totalDyeAlloc += columnAlloc;
                                        }
                                        var Value = item.Where(s => (s.Alloc_Type == LOVConstants.AllocationType.DYE && !s.Production_Status.Equals(LOVConstants.ProductionStatus.Allocated, StringComparison.InvariantCultureIgnoreCase)) && s.Dye_Shade_CD == dye);
                                        if (!isSuggestInclude)
                                        {
                                            Value = Value.Where(y => string.Compare(y.Production_Status, LOVConstants.ProductionStatus.Locked, true) == 0 || string.Compare(y.Production_Status, LOVConstants.ProductionStatus.Released, true) == 0);
                                        }
                                        columnSum = Convert.ToDecimal(Value.Sum(s => s.LBS.RoundCustom(0)).ToNumberString());                                        
                                        row[dye] = (i == 4) ? columnSum.ToNumberString() : (i == 5) ? columnAlloc.ToNumberString() : (columnAlloc - columnSum).ToNumberString();
                                    }
                                }
                                if (i <= 3)
                                {
                                    row["Total"] = dyeTotal.ToNumberString();
                                    grandDyeTotal += dyeTotal;
                                }
                                else
                                {
                                    if (i == 4)
                                    {
                                        row["Total"] = grandDyeTotal.ToNumberString();
                                    }
                                    else if (i == 5)
                                    {
                                        row["Total"] = totalDyeAlloc.ToNumberString();
                                    }
                                }
                                dt.Rows.Add(row);
                            }
                            if (textAllocations.Count > 1) { dt.Rows.Add(dt.NewRow()); }
                            break;
                        case LOVConstants.GridNames.MACHINE:

                            if (!dt.Columns.Contains("Machine")) { dt.Columns.Add("Machine"); };

                            foreach (var item1 in machine)
                            {
                                var mType = item1.Replace("/", "_");
                                if (!dt.Columns.Contains(string.Format(_columnFormat, mType))) { dt.Columns.Add(string.Format(_columnFormat, mType)); };
                            }

                            if (!dt.Columns.Contains("Total"))
                            {
                                dt.Columns.Add("Total");
                            };

                            var sugMachine = suggested.Where(s => (s.Alloc_Type.Equals(LOVConstants.AllocationType.MAC, StringComparison.InvariantCultureIgnoreCase) || s.Alloc_Type.Equals(LOVConstants.AllocationType.CYLMAC, StringComparison.InvariantCultureIgnoreCase)));
                            var lockMachine = locked.Where(s => (s.Alloc_Type.Equals(LOVConstants.AllocationType.MAC, StringComparison.InvariantCultureIgnoreCase) || s.Alloc_Type.Equals(LOVConstants.AllocationType.CYLMAC, StringComparison.InvariantCultureIgnoreCase)));
                            var relMachine = released.Where(s => (s.Alloc_Type.Equals(LOVConstants.AllocationType.MAC, StringComparison.InvariantCultureIgnoreCase) || s.Alloc_Type.Equals(LOVConstants.AllocationType.CYLMAC, StringComparison.InvariantCultureIgnoreCase)));
                            var allocatedMachine = allocated.Where(s => (s.Alloc_Type.Equals(LOVConstants.AllocationType.MAC, StringComparison.InvariantCultureIgnoreCase) || s.Alloc_Type.Equals(LOVConstants.AllocationType.CYLMAC, StringComparison.InvariantCultureIgnoreCase)));

                            IEnumerable<TextileAllocation> textileMachineAllocation = null;
                            decimal grandMachineTotal = 0;
                            decimal totalMachineAlloc = 0;
                            decimal grandMachineAvgTotal = 0;
                            decimal totalMachineAvgAlloc = 0;

                            for (int i = 1; i < 7; i++)
                            {
                                decimal LBS = 0;
                                decimal avg = 0;
                                decimal ClmnAverage = 0;
                                decimal average = 0;
                                DataRow row = dt.NewRow();
                                switch (i)
                                {
                                    //Suggested values
                                    case 1:
                                        row["Plant"] = plant;
                                        row["Machine"] = "Suggested";
                                        textileMachineAllocation = sugMachine;

                                        break;
                                    //Locked
                                    case 2:
                                        row["Machine"] = "Locked";
                                        textileMachineAllocation = lockMachine;

                                        break;
                                    case 3://Released
                                        row["Machine"] = "Released";
                                        textileMachineAllocation = relMachine;

                                        break;
                                    case 4://Total
                                        row["Machine"] = "Total";
                                        textileMachineAllocation = null;
                                        break;
                                    case 5://Alloc
                                        row["Machine"] = "Alloc";                                       
                                        textileMachineAllocation = allocatedMachine;
                                        break;
                                    case 6://Net
                                        row["Machine"] = "Net";
                                        row["Total"] = string.Format(_machineValueFormat, (totalMachineAlloc - grandMachineTotal).ToNumberString(), (totalMachineAvgAlloc - grandMachineAvgTotal).ToNumberString());

                                        break;

                                }
                                decimal machineTotal = 0;
                                decimal machineAvgTotal = 0;
                                foreach (var item1 in machine)
                                {
                                    string mType = item1.Replace("/", "_");
                                    decimal columnSum = 0;
                                    string columnAlloc = "0/0";
                                    LBS = avg = 0;
                                    var itemValue = item.Where(s => (s.Alloc_Type == LOVConstants.AllocationType.MAC || s.Alloc_Type == LOVConstants.AllocationType.CYLMAC) && s.Production_Status.Equals(LOVConstants.ProductionStatus.Allocated, StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrWhiteSpace(s.Machine_Type_CD) && s.Machine_Type_CD.Equals(item1, StringComparison.InvariantCultureIgnoreCase));
                                    if(itemValue!=null)
                                    {
                                        avg = itemValue.Sum(s => s.AVG.RoundCustom(0));
                                    }
                                    IEnumerable<TextileAllocation> machineAlloc = null;
                                    if (i <= 3)
                                    {
                                        if (textileMachineAllocation != null && textileMachineAllocation.Any())
                                        {
                                            machineAlloc = textileMachineAllocation.Where(s => !string.IsNullOrWhiteSpace(s.Machine_Type_CD) && s.Machine_Type_CD.Equals(item1, StringComparison.InvariantCultureIgnoreCase));
                                        }
                                        if (machineAlloc != null && machineAlloc.Any())
                                        {
                                            machineTotal += LBS = machineAlloc.Sum(s => s.LBS).RoundCustom(0);                                                                                         
                                        }
                                        average = (avg > 0 ? (LBS / avg) : avg).RoundCustom(0);
                                        machineAvgTotal += average;
                                        row[string.Format(_columnFormat, mType)] = string.Format(_machineValueFormat, LBS.ToNumberString(), average.ToNumberString());
                                    }
                                    else
                                    {
                                        if (textileMachineAllocation != null && textileMachineAllocation.Any())
                                        {
                                            var result = textileMachineAllocation.Where(s => !string.IsNullOrWhiteSpace(s.Machine_Type_CD) && s.Machine_Type_CD.Equals(item1, StringComparison.InvariantCultureIgnoreCase));
                                            if (result != null)
                                            {
                                                totalMachineAlloc += LBS = result.Sum(s => s.LBS).RoundCustom(0);                                               
                                                average = (avg > 0 ? (LBS / avg) : avg).RoundCustom(0);
                                                totalMachineAvgAlloc += average;
                                                machineAvgTotal += average;
                                                columnAlloc = string.Format(_machineValueFormat, LBS.ToNumberString(), average.ToNumberString());
                                            }
                                        }
                                        var Value = item.Where(s => (s.Alloc_Type == LOVConstants.AllocationType.MAC || s.Alloc_Type == LOVConstants.AllocationType.CYLMAC) && !s.Production_Status.Equals(LOVConstants.ProductionStatus.Allocated, StringComparison.InvariantCultureIgnoreCase)  && !string.IsNullOrWhiteSpace(s.Machine_Type_CD) &&  s.Machine_Type_CD.Equals(item1, StringComparison.InvariantCultureIgnoreCase));
                                        if (!isSuggestInclude)
                                        {
                                            Value = Value.Where(y => string.Compare(y.Production_Status, LOVConstants.ProductionStatus.Locked, true) == 0 || string.Compare(y.Production_Status, LOVConstants.ProductionStatus.Released, true) == 0);
                                        }
                                        columnSum = Value.Sum(s => s.LBS).RoundCustom(0);
                                        //var columnAvg = Value.Sum(s => s.AVG).RoundCustom(0);
                                        var columnAvg = itemValue.Sum(s => s.AVG.RoundCustom(0));
                                        ClmnAverage = columnAvg > 0 ? (columnSum / columnAvg) : columnAvg;
                                        string machineValue = string.Format(_machineValueFormat, columnSum.ToNumberString(), ClmnAverage.ToNumberString());
                                        string netValue = string.Format(_machineValueFormat, (LBS - columnSum).ToNumberString(), (average - ClmnAverage).ToNumberString());
                                        row[string.Format(_columnFormat, mType)] = (i == 4) ? machineValue : (i == 5) ? columnAlloc : netValue;
                                    }
                                }

                                if (i <= 3)
                                {
                                    row["Total"] = string.Format(_machineValueFormat, machineTotal.ToNumberString(), machineAvgTotal.ToNumberString());
                                    grandMachineAvgTotal += machineAvgTotal;
                                    grandMachineTotal += machineTotal;
                                }
                                else
                                {
                                    if (i == 4)
                                    {
                                        row["Total"] = string.Format(_machineValueFormat, grandMachineTotal.ToNumberString(), grandMachineAvgTotal.ToNumberString());
                                    }
                                    else if (i == 5)
                                    {
                                        row["Total"] = string.Format(_machineValueFormat, totalMachineAlloc.ToNumberString(), totalMachineAvgAlloc.ToNumberString());
                                    }
                                }
                                dt.Rows.Add(row);
                            }
                            if (textAllocations.Count > 1) { dt.Rows.Add(dt.NewRow()); }
                            break;
                        //case LOVConstants.GridNames.YARN:

                        //    if (!dt.Columns.Contains("Yarn"))
                        //    {
                        //        dt.Columns.Add("Yarn");
                        //    }

                        //    foreach (var yarn in yarns)
                        //    {
                        //        if (!dt.Columns.Contains(string.Format(_columnFormat, yarn)))
                        //        {
                        //            dt.Columns.Add(string.Format(_columnFormat, yarn));
                        //        };
                        //    }
                        //    if (!dt.Columns.Contains("Total"))
                        //    {
                        //        dt.Columns.Add("Total");
                        //    };

                        //    var sugYarn = suggested.Where(s => (s.Alloc_Type == LOVConstants.AllocationType.YARN));
                        //    var lockYarn = locked.Where(s => (s.Alloc_Type == LOVConstants.AllocationType.YARN));
                        //    var relYarn = released.Where(s => (s.Alloc_Type == LOVConstants.AllocationType.YARN));
                        //    var allocatedYarn = allocated.Where(s => (s.Alloc_Type == LOVConstants.AllocationType.YARN));

                        //    IEnumerable<TextileAllocation> textileYarnAllocation = null;
                        //    decimal grandYarnTotal = 0;
                        //    decimal totalYarnAlloc = 0;
                        //    for (int i = 1; i < 7; i++)
                        //    {
                        //        DataRow row = dt.NewRow();
                        //        switch (i)
                        //        {
                        //            //Suggested values
                        //            case 1:
                        //                row["Plant"] = plant;
                        //                row["Yarn"] = "Suggested";
                        //                textileYarnAllocation = sugYarn;

                        //                break;
                        //            //Locked
                        //            case 2:
                        //                row["Yarn"] = "Locked";
                        //                textileYarnAllocation = lockYarn;

                        //                break;
                        //            case 3://Released
                        //                row["Yarn"] = "Released";
                        //                textileYarnAllocation = relYarn;

                        //                break;
                        //            case 4://Total
                        //                row["Yarn"] = "Total";
                        //                break;
                        //            case 5://Alloc
                        //                row["Yarn"] = "Alloc";
                        //                textileYarnAllocation = allocatedYarn;
                        //                break;
                        //            case 6://Net
                        //                row["Yarn"] = "Net";
                        //                row["Total"] = (totalYarnAlloc - grandYarnTotal).ToNumberString();
                        //                break;

                        //        }
                        //        decimal yarnTotal = 0;
                        //        foreach (var item1 in yarns)
                        //        {
                        //            decimal columnSum = 0;
                        //            decimal columnAlloc = 0;
                        //            decimal value = 0;
                                   
                        //            if (i <= 3)
                        //            {
                        //                if (textileYarnAllocation != null && textileYarnAllocation.Any())
                        //                {
                        //                    var yarnAlloc = textileYarnAllocation.Where(s => s.Resource_ID == item1);
                        //                    value = yarnAlloc != null ? yarnAlloc.Sum(s => s.LBS).RoundCustom(0) : 0;
                        //                }
                        //                yarnTotal += value;
                        //                row[string.Format(_columnFormat, item1)] = value.ToNumberString();
                        //            }
                        //            else
                        //            {
                        //                if (textileYarnAllocation != null && textileYarnAllocation.Any())
                        //                {
                        //                    var result = textileYarnAllocation.Where(s => s.Resource_ID == item1);
                        //                    columnAlloc = result != null ? Convert.ToDecimal(result.Sum(s=>s.LBS).RoundCustom(0)) : 0;
                        //                }
                        //                var yarnValue = item.Where(s => (s.Alloc_Type == LOVConstants.AllocationType.YARN && !s.Production_Status.Equals(LOVConstants.ProductionStatus.Allocated, StringComparison.InvariantCultureIgnoreCase)) && s.Resource_ID == item1);
                        //                if (!isSuggestInclude)
                        //                {
                        //                    yarnValue = yarnValue.Where(y => string.Compare(y.Production_Status, LOVConstants.ProductionStatus.Locked, true) == 0 || string.Compare(y.Production_Status, LOVConstants.ProductionStatus.Released, true) == 0);
                        //                }
                        //                columnSum = Convert.ToDecimal(yarnValue.Sum(s => s.LBS).RoundCustom(0));

                        //                row[string.Format(_columnFormat, item1)] = (i == 4) ? columnSum.ToNumberString() : (i == 5) ? columnAlloc.ToNumberString() : (columnAlloc - columnSum).ToNumberString();
                        //            }
                        //        }
                        //        if (i <= 3)
                        //        {
                        //            row["Total"] = yarnTotal.ToNumberString();
                        //            grandYarnTotal += yarnTotal;
                        //        }
                        //        else
                        //        {
                        //            if (i == 4)
                        //            {
                        //                row["Total"] = grandYarnTotal.ToNumberString();
                        //            }
                        //            else if (i == 5)
                        //            {
                        //                row["Total"] = totalYarnAlloc.ToNumberString();
                        //            }
                        //        }
                        //        dt.Rows.Add(row);
                        //    }
                        //    if (textAllocations.Count > 1) { dt.Rows.Add(dt.NewRow()); }
                        //    break;
                        case LOVConstants.GridNames.YARN_ITEM:
                            var yarnItems = item.Select(s => s.Resource_ID)
                                                .OrderBy(s => s)
                                                .Distinct();
                            var sugYarnItem = suggested.Where(s => (s.Alloc_Type == LOVConstants.AllocationType.YARN));
                            var lockYarnItem = locked.Where(s => (s.Alloc_Type == LOVConstants.AllocationType.YARN));
                            var relYarnItem = released.Where(s => (s.Alloc_Type == LOVConstants.AllocationType.YARN));
                            var allocatedYarnItem = allocated.Where(s => (s.Alloc_Type == LOVConstants.AllocationType.YARN));
                            decimal allocYarnItem = allocatedYarnItem.Sum(s => s.LBS).RoundCustom(0);
                            string fromDate = "";
                            List<YarnItem> yrnItms = null;
                            if (SessionHelper.FROMDATE != null)
                            {
                                fromDate = SessionHelper.FROMDATE;
                            }

                            string[] weekYears = fromDate != null ? fromDate.Split('/') : null;
                
                            int week = 0;
                            int year = 0;
              

                            if (weekYears != null && weekYears.Count() > 0)
                            {
                                week = Convert.ToInt32(weekYears[0].Trim());
                                year = Convert.ToInt32(weekYears[1].Trim());

                                yrnItms = textilesService.GetYarnDesc(week, year);
                            }


                            if (!dt.Columns.Contains("YarnItem"))
                            {
                                dt.Columns.Add("YarnItem");
                            }
                            if (!dt.Columns.Contains("Suggested"))
                            {
                                dt.Columns.Add("Suggested");
                            }
                            if (!dt.Columns.Contains("Locked"))
                            {
                                dt.Columns.Add("Locked");
                            }
                            if (!dt.Columns.Contains("Released"))
                            {
                                dt.Columns.Add("Released");
                            }
                            if (!dt.Columns.Contains("Total"))
                            {
                                dt.Columns.Add("Total");
                            }
                            if (!dt.Columns.Contains("Alloc"))
                            {
                                dt.Columns.Add("Alloc");
                            }
                            if (!dt.Columns.Contains("Adj"))
                            {
                                dt.Columns.Add("Adj");
                            }
                            if (!dt.Columns.Contains("Net"))
                            {
                                dt.Columns.Add("Net");
                            }
                            int count = 0;
                            foreach (var yarn in yarnItems)
                            {
                                decimal yarnTotal = 0;
                                decimal yarnAlloc = 0;
                                DataRow row = dt.NewRow();
                                ++count;
                                if (count == 1)
                                {
                                    row["Plant"] = plant;
                                }
                                row["YarnItem"] = yarn;
                                if (yrnItms != null)
                                {
                                    var yarnDesc = yrnItms.Where(s => s.YarnItm == yarn && s.Plant == plant).ToList();
                                    if (yarnDesc != null && yarnDesc.Count > 0)
                                    {
                                        if (!string.IsNullOrEmpty(yarnDesc[0].YarnDesc))
                                            row["YarnItem"] = yarn + " - " + yarnDesc[0].YarnDesc;
                                    }
                                }
                                
                                row["Suggested"] = sugYarnItem.Where(y => y.Resource_ID == yarn).Sum(s => s.LBS).RoundCustom(0).ToNumberString();
                                row["Locked"] = lockYarnItem.Where(y => y.Resource_ID == yarn).Sum(s => s.LBS).RoundCustom(0).ToNumberString();
                                row["Released"] = relYarnItem.Where(y => y.Resource_ID == yarn).Sum(s => s.LBS).RoundCustom(0).ToNumberString();
                                var yarnValue = item.Where(s => (s.Alloc_Type == LOVConstants.AllocationType.YARN  && !s.Production_Status.Equals(LOVConstants.ProductionStatus.Allocated, StringComparison.InvariantCultureIgnoreCase)) && s.Resource_ID == yarn);
                                if (!isSuggestInclude)
                                {
                                    yarnValue = yarnValue.Where(y => string.Compare(y.Production_Status, LOVConstants.ProductionStatus.Locked, true) == 0 || string.Compare(y.Production_Status, LOVConstants.ProductionStatus.Released, true) == 0);
                                }
                                yarnTotal = yarnValue.Sum(s => s.LBS.RoundCustom(0));
                                yarnAlloc = (allocatedYarnItem != null && allocatedYarnItem.Any()) ? allocatedYarnItem.Where(y => y.Resource_ID == yarn).Sum(s => s.LBS).RoundCustom(0) : 0;
                                row["Total"] = yarnTotal.ToNumberString();
                                row["Alloc"] = yarnAlloc.ToNumberString();
                                row["Adj"] = 0;
                                row["Net"] = (yarnAlloc - yarnTotal).RoundCustom(0).ToNumberString();
                                dt.Rows.Add(row);
                            }
                            if (textAllocations.Count > 1) { dt.Rows.Add(dt.NewRow()); }
                            break;

                            //Newly Added
                        case LOVConstants.GridNames.FABRIC_ITEM:
                            //var fabricItems = item.Select(s => s.Resource_ID)
                            //                    .OrderBy(s => s)
                            //                    .Distinct();
                            textAllocationCount++;
                            List<string> fabricItems = new List<string>();
                            fabricItems = item.Select(s => s.Resource_ID).Distinct().ToList();

                          //  List<string> Textilegroups = new List<string>();
                          //  Textilegroups = item.Select(s => s.Dye_Shade_CD).Distinct().ToList();
                           
                            var sugfabricItem = suggested.Where(s => (s.Alloc_Type == LOVConstants.AllocationType.FAB));
                            var lockfabricItem = locked.Where(s => (s.Alloc_Type == LOVConstants.AllocationType.FAB));
                            var relfabricItem = released.Where(s => (s.Alloc_Type == LOVConstants.AllocationType.FAB));
                            var allocatedfabricItem = allocated.Where(s => (s.Alloc_Type == LOVConstants.AllocationType.FAB));
                            decimal allocfabricItem = allocatedfabricItem.Sum(s => s.LBS).RoundCustom(0);
                            IEnumerable<TextileAllocation> textileFabricAllocation = null;
                            if (!dt.Columns.Contains("Fabric"))
                            {
                                dt.Columns.Add("Fabric");
                            }
                            if (!dt.Columns.Contains("FabricItem"))
                            {
                                dt.Columns.Add("FabricItem");
                            }
                            if (!dt.Columns.Contains("Suggested"))
                            {
                                dt.Columns.Add("Suggested");
                            }
                            if (!dt.Columns.Contains("Locked"))
                            {
                                dt.Columns.Add("Locked");
                            }
                            if (!dt.Columns.Contains("Released"))
                            {
                                dt.Columns.Add("Released");
                            }
                            if (!dt.Columns.Contains("Total"))
                            {
                                dt.Columns.Add("Total");
                            }
                            if (!dt.Columns.Contains("Alloc"))
                            {
                                dt.Columns.Add("Alloc");
                            }
                            if (!dt.Columns.Contains("Net"))
                            {
                                dt.Columns.Add("Net");
                            }
                            count = 0;
                            decimal suggestedTotal = 0;
                            decimal lockedTotal = 0;
                            decimal releasedTotal = 0;
                            decimal totalPlant = 0;
                            decimal allocTotal = 0;
                            decimal netTotal = 0;


                            decimal suggestedTextileTotal = 0;
                            decimal lockedTextileTotal = 0;
                            decimal releasedTextileTotal = 0;
                            decimal totalTextilegroup = 0;
                          //  decimal allocTextileTotal = 0;
                           // decimal netTextileTotal = 0;
                            //string fabrics2 = null;

                            //foreach (var fabric in fabricItems)
                            for (int i = 0; i < fabricItems.Count; i++)
                            {                              
                                int lastindex = fabricItems.Count - 1;
                                var fabricsItems = item.Where(s => s.Resource_ID == fabricItems[i]);
                                string fabrics = fabricsItems.FirstOrDefault().Dye_Shade_CD;

                                

                                decimal fabricTotal = 0;
                                decimal fabricAlloc = 0;                                

                                DataRow row = dt.NewRow();
                                ++count;

                                if (count == 1)
                                {
                                    row["Plant"] = plant;
                                }
                                row["Fabric"] = fabrics;
                                row["FabricItem"] = fabricItems[i];
                                row["Suggested"] = sugfabricItem.Where(y => y.Resource_ID == fabricItems[i]).Sum(s => s.LBS).RoundCustom(0).ToNumberString();
                                suggestedTotal += sugfabricItem.Where(y => y.Resource_ID == fabricItems[i]).Sum(s => s.LBS).RoundCustom(0);
                                suggestedTextileTotal += sugfabricItem.Where(y => y.Resource_ID == fabricItems[i]).Sum(s => s.LBS).RoundCustom(0);
                                row["Locked"] = lockfabricItem.Where(y => y.Resource_ID == fabricItems[i]).Sum(s => s.LBS).RoundCustom(0).ToNumberString();
                                lockedTotal += lockfabricItem.Where(y => y.Resource_ID == fabricItems[i]).Sum(s => s.LBS).RoundCustom(0);
                                lockedTextileTotal += lockfabricItem.Where(y => y.Resource_ID == fabricItems[i]).Sum(s => s.LBS).RoundCustom(0);
                                row["Released"] = relfabricItem.Where(y => y.Resource_ID == fabricItems[i]).Sum(s => s.LBS).RoundCustom(0).ToNumberString();
                                releasedTotal += relfabricItem.Where(y => y.Resource_ID == fabricItems[i]).Sum(s => s.LBS).RoundCustom(0);
                                releasedTextileTotal += relfabricItem.Where(y => y.Resource_ID == fabricItems[i]).Sum(s => s.LBS).RoundCustom(0);
                                var fabricValue = item.Where(s => (s.Alloc_Type == LOVConstants.AllocationType.FAB && !s.Production_Status.Equals(LOVConstants.ProductionStatus.Allocated, StringComparison.InvariantCultureIgnoreCase)) && s.Resource_ID == fabricItems[i]);
                                if (!isSuggestInclude)
                                {
                                    fabricValue = fabricValue.Where(y => string.Compare(y.Production_Status, LOVConstants.ProductionStatus.Locked, true) == 0 || string.Compare(y.Production_Status, LOVConstants.ProductionStatus.Released, true) == 0);
                                }
                                fabricTotal = fabricValue.Sum(s => s.LBS.RoundCustom(0));
                                totalPlant += fabricValue.Sum(s => s.LBS.RoundCustom(0));
                                totalTextilegroup += fabricValue.Sum(s => s.LBS.RoundCustom(0));  
                                //fabricAlloc = (allocatedfabricItem != null && allocatedfabricItem.Any()) ? allocatedfabricItem.Where(y => y.Resource_ID == fabricItems[i]).Sum(s => s.LBS).RoundCustom(0) : 0;
                                //allocTotal += fabricAlloc;
                                row["Total"] = fabricTotal.ToNumberString();
                                //row["Alloc"] = fabricAlloc.ToNumberString();
                                row["Alloc"] = 0;
                                //row["Net"] = (fabricAlloc - fabricTotal).RoundCustom(0).ToNumberString();
                                row["Net"] = 0;
                                //netTotal += (fabricAlloc - fabricTotal).RoundCustom(0);

                                var j=i+1;

                                if (lastindex == i)
                                {
                                    j = i;
                                    i++;
                                };

                                if (i < fabricItems.Count -1)
                                
                                    {
                                        i++;
                                    }
                                    var fabricsItems2 = item.Where(s => s.Resource_ID == fabricItems[j]);
                                    string fabrics2 = fabricsItems2.FirstOrDefault().Dye_Shade_CD;

                                    if (i <= fabricItems.Count )
                                    {
                                        i--;
                                    }
                                
                                if(fabricItems[i] != null)
                                    dt.Rows.Add(row);
                                //if (i > 0)
                                //{
                                if (fabrics2 != fabrics ||  lastindex == i)
                                {
                                 //   dt.Rows.Remove(row);
                                    row = dt.NewRow();
                                    row["Fabric"] = fabrics + " TOTAL";
                                    row["Suggested"] = suggestedTextileTotal.ToNumberString();
                                    row["Locked"] = lockedTextileTotal.ToNumberString();
                                    row["Released"] = releasedTextileTotal.ToNumberString();
                                    row["Total"] = totalTextilegroup.ToNumberString();
                                    //row["Alloc"] = fabricAlloc.ToNumberString();
                                    //totalPlantAlloc += fabricAlloc;
                                    //netTotal = (fabricAlloc - totalPlant).RoundCustom(0);
                                    row["Alloc"] = 0;
                                    // row["Alloc"] = allocfabricItem.ToNumberString();
                                    // totalPlantAlloc += allocfabricItem;
                                    // netTotal = (allocfabricItem - totalPlant).RoundCustom(0);
                                    //row["Net"] = netTotal.ToNumberString();
                                    row["Net"] = 0;
                                    // row["Net"] = netTextileTotal.ToNumberString();
                                    // totalPlantNet += netTotal;
                                    dt.Rows.Add(row);
                                    suggestedTextileTotal = 0;
                                    lockedTextileTotal = 0;
                                    releasedTextileTotal = 0;
                                    totalTextilegroup = 0;
                                    //allocTextileTotal = 0;
                                    //netTextileTotal = 0;
                                  //  i--;
                                }

                                //}
                               

                               //  fabrics2 = fabrics;

                                if(lastindex == i)
                                {
                                    row = dt.NewRow();
                                    row["Plant"] = plant + " TOTAL";
                                    row["Suggested"] = suggestedTotal.ToNumberString();
                                    totalSuggestedFabric += suggestedTotal;
                                    row["Locked"] = lockedTotal.ToNumberString();
                                    totalLockedFabric += lockedTotal;
                                    row["Released"] = releasedTotal.ToNumberString();
                                    totalReleasedFabric += releasedTotal;
                                    row["Total"] = totalPlant.ToNumberString();
                                    totalPlantsTotal += totalPlant;
                                    //row["Alloc"] = fabricAlloc.ToNumberString();
                                    //totalPlantAlloc += fabricAlloc;
                                    //netTotal = (fabricAlloc - totalPlant).RoundCustom(0);
                                    row["Alloc"] = allocfabricItem.ToNumberString();
                                    totalPlantAlloc += allocfabricItem;
                                    netTotal = (allocfabricItem - totalPlant).RoundCustom(0);
                                    //row["Net"] = netTotal.ToNumberString();
                                    row["Net"] = netTotal.ToNumberString();
                                    totalPlantNet += netTotal;
                                    dt.Rows.Add(row);

                                    if(textAllocationCount == textAllocations.Count)
                                    {
                                        row = dt.NewRow();
                                        row = dt.NewRow();
                                        row["Plant"] = "FABRIC TOTAL";
                                        row["Suggested"] = totalSuggestedFabric.ToNumberString();
                                        row["Locked"] = totalLockedFabric.ToNumberString();
                                        row["Released"] = totalReleasedFabric.ToNumberString();
                                        row["Total"] = totalPlantsTotal.ToNumberString();
                                        row["Alloc"] = totalPlantAlloc.ToNumberString();
                                        row["Net"] = totalPlantNet.ToNumberString();
                                        dt.Rows.Add(row);
                                    }

                                }

                            }
                            if (textAllocations.Count > 1) { dt.Rows.Add(dt.NewRow()); }
                            break;

                        case LOVConstants.GridNames.PRINT:
                            //var printItems = item.Select(s => s.Resource_ID)
                            //                    .OrderBy(s => s)
                            //                    .Distinct();
                            textAllocationCount++;
                            List<string> printItems = new List<string>();
                            printItems = item.Select(s => s.Resource_ID).Distinct().OfType<string>().ToList();
                            var sugPrintItem = suggested.Where(s => (s.Alloc_Type == LOVConstants.AllocationType.PRT));
                            var lockPrintItem = locked.Where(s => (s.Alloc_Type == LOVConstants.AllocationType.PRT));
                            var relPrintItem = released.Where(s => (s.Alloc_Type == LOVConstants.AllocationType.PRT));
                            var allocatedPrintItem = allocated.Where(s => (s.Alloc_Type == LOVConstants.AllocationType.PRT));
                            decimal allocPrintItem = allocatedPrintItem.Sum(s => s.LBS).RoundCustom(0);
                            //List<YarnItem> yrnItms = null;
                             textileFabricAllocation = null;
                            if (!dt.Columns.Contains("Fabric"))
                            {
                                dt.Columns.Add("Fabric");
                            }
                            if (!dt.Columns.Contains("FabricItem"))
                            {
                                dt.Columns.Add("FabricItem");
                            }
                            if (!dt.Columns.Contains("Suggested"))
                            {
                                dt.Columns.Add("Suggested");
                            }
                            if (!dt.Columns.Contains("Locked"))
                            {
                                dt.Columns.Add("Locked");
                            }
                            if (!dt.Columns.Contains("Released"))
                            {
                                dt.Columns.Add("Released");
                            }
                            if (!dt.Columns.Contains("Total"))
                            {
                                dt.Columns.Add("Total");
                            }
                            if (!dt.Columns.Contains("Alloc"))
                            {
                                dt.Columns.Add("Alloc");
                            }
                            if (!dt.Columns.Contains("Net"))
                            {
                                dt.Columns.Add("Net");
                            }
                            count = 0;
                            decimal suggestedTotalPrint = 0;
                            decimal lockedTotalPrint = 0;
                            decimal releasedTotalPrint = 0;
                            decimal totalPlantPrint = 0;
                            decimal allocTotalPrint = 0;
                            decimal netTotalPrint = 0;

                            decimal suggestedSummarizeTotalPrint = 0;
                            decimal lockedSummarizeTotalPrint = 0;
                            decimal releasedSummarizeTotalPrint = 0;
                            decimal totalSummarizePlantPrint = 0;
                          //  decimal allocSummarizeTotalPrint = 0;
                            //decimal netSummarizeTotalPrint = 0;
							
                            //foreach (var print in printItems)
                            for (int i = 0; i < printItems.Count; i++)                           
                            {
                                int lastindex = printItems.Count - 1;

                                var printsitems = item.Where(s => s.Resource_ID == printItems[i]);
                                string prints = printsitems.FirstOrDefault().Dye_Shade_CD;

                                decimal printTotal = 0;
                                decimal printAlloc = 0;
                                DataRow row = dt.NewRow();
                                ++count;
                                if (count == 1)
                                {
                                    row["Plant"] = plant;
                                }
                                row["Fabric"] = prints;
                                row["FabricItem"] = printItems[i];
                                row["Suggested"] = sugPrintItem.Where(y => y.Resource_ID == printItems[i]).Sum(s => s.LBS).RoundCustom(0).ToNumberString();
                                suggestedTotalPrint += sugPrintItem.Where(y => y.Resource_ID == printItems[i]).Sum(s => s.LBS).RoundCustom(0);
                                suggestedSummarizeTotalPrint += sugPrintItem.Where(y => y.Resource_ID == printItems[i]).Sum(s => s.LBS).RoundCustom(0);
                                row["Locked"] = lockPrintItem.Where(y => y.Resource_ID == printItems[i]).Sum(s => s.LBS).RoundCustom(0).ToNumberString();
                                lockedTotalPrint += lockPrintItem.Where(y => y.Resource_ID == printItems[i]).Sum(s => s.LBS).RoundCustom(0);
                                lockedSummarizeTotalPrint += lockPrintItem.Where(y => y.Resource_ID == printItems[i]).Sum(s => s.LBS).RoundCustom(0);						  
                                row["Released"] = relPrintItem.Where(y => y.Resource_ID == printItems[i]).Sum(s => s.LBS).RoundCustom(0).ToNumberString();
                                releasedTotalPrint += relPrintItem.Where(y => y.Resource_ID == printItems[i]).Sum(s => s.LBS).RoundCustom(0);
                                releasedSummarizeTotalPrint += relPrintItem.Where(y => y.Resource_ID == printItems[i]).Sum(s => s.LBS).RoundCustom(0);
                                var printValue = item.Where(s => (s.Alloc_Type == LOVConstants.AllocationType.PRT && !s.Production_Status.Equals(LOVConstants.ProductionStatus.Allocated, StringComparison.InvariantCultureIgnoreCase)) && s.Resource_ID == printItems[i]);
                                if (!isSuggestInclude)
                                {
                                    printValue = printValue.Where(y => string.Compare(y.Production_Status, LOVConstants.ProductionStatus.Locked, true) == 0 || string.Compare(y.Production_Status, LOVConstants.ProductionStatus.Released, true) == 0);
                                }
                                printTotal = printValue.Sum(s => s.LBS.RoundCustom(0));
                                totalPlantPrint += printTotal;
                                totalSummarizePlantPrint += printTotal;
                                //printAlloc = (allocatedPrintItem != null && allocatedPrintItem.Any()) ? allocatedPrintItem.Where(y => y.Resource_ID == printItems[i]).Sum(s => s.LBS).RoundCustom(0) : 0;
                                //allocTotalPrint += printAlloc;
                                row["Total"] = printTotal.ToNumberString();
                                //row["Alloc"] = printAlloc.ToNumberString();
                                //row["Net"] = (printAlloc - printTotal).RoundCustom(0).ToNumberString();
                                row["Alloc"] = 0;
                                row["Net"] = 0;
                                //netTotalPrint += (printAlloc - printTotal).RoundCustom(0);
                               
                                var j = i + 1;

                                if (lastindex == i)
                                {
                                    j = i;
                                    i++;
                                };

                                if (i < printItems.Count - 1)
                                {
                                    i++;
                                }
                                var printsitems2 = item.Where(s => s.Resource_ID == printItems[j]);
                                string prints2 = printsitems2.FirstOrDefault().Dye_Shade_CD;

                                if (i <= printItems.Count)
                                {
                                    i--;
                                }
                                
                                if (printItems[i] != null)
                                      dt.Rows.Add(row);
                                if (prints2 != prints || lastindex == i)
                                {
                                    //   dt.Rows.Remove(row);
                                    row = dt.NewRow();
                                    row["Fabric"] = prints + " TOTAL";
                                    row["Suggested"] = suggestedSummarizeTotalPrint.ToNumberString();
                                    row["Locked"] = lockedSummarizeTotalPrint.ToNumberString();
                                    row["Released"] = releasedSummarizeTotalPrint.ToNumberString();
                                    row["Total"] = totalSummarizePlantPrint.ToNumberString();
                                    //row["Alloc"] = fabricAlloc.ToNumberString();
                                    //totalPlantAlloc += fabricAlloc;
                                    //netTotal = (fabricAlloc - totalPlant).RoundCustom(0);
                                    row["Alloc"] = 0;
                                    // row["Alloc"] = allocfabricItem.ToNumberString();
                                    // totalPlantAlloc += allocfabricItem;
                                    // netTotal = (allocfabricItem - totalPlant).RoundCustom(0);
                                    //row["Net"] = netTotal.ToNumberString();
                                    row["Net"] = 0;
                                    // row["Net"] = netTextileTotal.ToNumberString();
                                    // totalPlantNet += netTotal;
                                    dt.Rows.Add(row);
                                    suggestedSummarizeTotalPrint = 0;
                                    lockedSummarizeTotalPrint = 0;
                                    releasedSummarizeTotalPrint = 0;
                                    totalSummarizePlantPrint = 0;
                                    //allocSummarizeTotalPrint = 0;
                                   // netSummarizeTotalPrint = 0;
                                    //  i--;
                                }
                                if (lastindex == i)
                                {
                                    row = dt.NewRow();
                                    row["Plant"] = plant + " TOTAL";
                                    row["Suggested"] = suggestedTotalPrint.ToNumberString();
                                    totalSuggestedFabric += suggestedTotalPrint;
                                    row["Locked"] = lockedTotalPrint.ToNumberString();
                                    totalLockedFabric += lockedTotalPrint;
                                    row["Released"] = releasedTotalPrint.ToNumberString();
                                    totalReleasedFabric += releasedTotalPrint;
                                    row["Total"] = totalPlantPrint.ToNumberString();
                                    totalPlantsTotal += totalPlantPrint;
                                    row["Alloc"] = allocPrintItem.ToNumberString();
                                    totalPlantAlloc += allocPrintItem;
                                    //row["Net"] = netTotalPrint.ToNumberString();
                                    //netTotalPrint = (printAlloc - totalPlantPrint).RoundCustom(0);
                                    netTotalPrint = (allocPrintItem - totalPlantPrint).RoundCustom(0);
                                    row["Net"] = netTotalPrint.ToNumberString();
                                    totalPlantNet += netTotalPrint;
                                    dt.Rows.Add(row);

                                    if (textAllocationCount == textAllocations.Count)
                                    {
                                        row = dt.NewRow();
                                        row = dt.NewRow();
                                        row["Plant"] = "PRINT TOTAL";
                                        row["Suggested"] = totalSuggestedFabric.ToNumberString();
                                        row["Locked"] = totalLockedFabric.ToNumberString();
                                        row["Released"] = totalReleasedFabric.ToNumberString();
                                        row["Total"] = totalPlantsTotal.ToNumberString();
                                        row["Alloc"] = totalPlantAlloc.ToNumberString();
                                        row["Net"] = totalPlantNet.ToNumberString();
                                        dt.Rows.Add(row);
                                    }

                                }
                            }
                            if (textAllocations.Count > 1) { dt.Rows.Add(dt.NewRow()); }
                            break;
                            //ENd
                    }
                }
            }
            return dt;
        }

        #endregion



        [HttpPost]
        public JsonResult GetWeekDetails(TextilesSearch filter)
        {
            List<string> lstDates = new List<string>();
            DateTime dtBeginDate = new DateTime();
            DateTime dtEndDate = new DateTime();

            string[] weekYears = filter.FromWYear != null ? filter.FromWYear.Split('/') : null;
            string[] toWeekYears = filter.ToWYear != null ? filter.ToWYear.Split('/') : null;
            int week = 0;
            int year = 0;
            int toWeek = 0;
            int toYear = 0;

            if (weekYears != null && weekYears.Count() > 0)
            {
                week = Convert.ToInt32(weekYears[0].Trim());
                year = Convert.ToInt32(weekYears[1].Trim());
            }

            if (toWeekYears != null && toWeekYears.Count() > 0)
            {
                toWeek = Convert.ToInt32(toWeekYears[0].Trim());
                toYear = Convert.ToInt32(toWeekYears[1].Trim());
            }

            string begindate = textilesService.GetBeginEndDates(week, year);
            string enddate = textilesService.GetBeginEndDates(toWeek, toYear);

            if (!string.IsNullOrEmpty(begindate))
            {
                dtBeginDate = Convert.ToDateTime(begindate);
                filter.StartDate = dtBeginDate;
            }

            if (!string.IsNullOrEmpty(enddate))
            {
                dtEndDate = Convert.ToDateTime(enddate);
                filter.EndDate = dtEndDate;
            }

            int moreWks = (int)(dtEndDate - dtBeginDate).TotalDays + 1;
            filter.NoWeeks = moreWks;

            return Json(filter);
        }

    }
}
