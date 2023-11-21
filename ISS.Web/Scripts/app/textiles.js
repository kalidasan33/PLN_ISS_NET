
textiles = {

    Textiles: {
        Exception: '',
        IsInit: true,
        validator: null

    },

    const: {
        Src: null
    },

    addInputClass: function (frm) {
        $(frm + ' input:not(:checkbox):not(:button):not(:reset):not(:submit)').addClass('InputF');
    },

    docTextilesReady: function (IsLoad) {
        textiles.addInputClass('#frmTextiles');
        textiles.Textiles.validator = $('#frmTextiles').kendoValidator().data("kendoValidator");
        $('#frmTextiles #btnTextilesSearch').bind('click', textiles.loadTextilesGrid)
        $('#btnTextilesFilter').bind('click', textiles.loadFilteredTextilesGrid);
        //$('#btnJumptoWorkorder').bind('click', textiles.gotoWorkorder);
        $('#frmTextiles .InputF').keypress(function (e) {
            if (e.which == 13) {
                textiles.loadTextilesGrid();
            }
        });
        ISS.common.toUpperCase('.InputF:not(.excludeF)');
        $('input[type="reset"]').bind('click', function () {

            textiles.Textiles.validator.hideMessages($('#frmTextiles #BusinessUnit'));
            textiles.Textiles.validator.hideMessages($('#frmTextiles #TextileGroup'));
            $('#TextileHead').empty();
            $('#TextilesDye').empty();
            $('#TextilesMachine').empty();
            $('#TextilesYarn').empty();
            $('#TextilesFabric').empty();
            $('#TextilesPrint').empty();

            var noRecords = $('.norecords');
            if (noRecords) {
                noRecords.hide();
            }

            textiles.filterDataClear();
        })

        ISS.common.menuEvent = function () {
            var grdHeadSize = $("#grdHeadSize").data("kendoGrid");
            if (grdHeadSize) {
                grdHeadSize.refresh();
            }
            var grdDyeBleach = $("#grdDyeBleach").data("kendoGrid");
            if (grdDyeBleach) {
                grdDyeBleach.refresh();
            }
            var grdMachineUtilization = $("#grdMachineUtilization").data("kendoGrid");
            if (grdMachineUtilization) {
                grdMachineUtilization.refresh();
            }
            var grdYarnDetails = $("#grdYarnDetails").data("kendoGrid");
            if (grdYarnDetails) {
                grdYarnDetails.refresh();
            }
            var grdFabricDetails = $("#grdFabricDetails").data("kendoGrid");
            if (grdFabricDetails) {
                grdFabricDetails.refresh();
            }
            var grdPrintDetails = $("#grdPrintDetails").data("kendoGrid");
            if (grdPrintDetails) {
                grdPrintDetails.refresh();
            }
        }
         
    },

  
    
    exportHeadSizekendo: function (e) {
        var sheet = e.workbook.sheets[0];

        for (var rowIndex = 1; rowIndex < sheet.rows.length; rowIndex++) {
            var row = sheet.rows[rowIndex];
            for (var cellIndex = 2; cellIndex < row.cells.length; cellIndex++) {
                if (row.cells[cellIndex].value!=null)
                    row.cells[cellIndex].value = parseFloat((row.cells[cellIndex].value + '').replaceAll(',', ''));
                row.cells[cellIndex].format = "#,##0";
            }
        }
    },

    exportDyeBleachkendo: function (e) {
        var sheet = e.workbook.sheets[0];

        for (var rowIndex = 1; rowIndex < sheet.rows.length; rowIndex++) {
            var row = sheet.rows[rowIndex];
            for (var cellIndex = 2; cellIndex < row.cells.length; cellIndex++) {
                if (row.cells[cellIndex].value != null)
                    row.cells[cellIndex].value = parseFloat((row.cells[cellIndex].value + '').replaceAll(',', ''));
                row.cells[cellIndex].format = "#,##0";
            }
        }
    },

    exportMachineUtlkendo: function (e) {
        var sheet = e.workbook.sheets[0];

        for (var rowIndex = 1; rowIndex < sheet.rows.length; rowIndex++) {
            var row = sheet.rows[rowIndex];
            for (var cellIndex = 2; cellIndex < row.cells.length; cellIndex++) {
                if (row.cells[cellIndex].value != null)
                    row.cells[cellIndex].value = parseFloat((row.cells[cellIndex].value + '').replaceAll(',', ''));
                row.cells[cellIndex].format = "#,##0";
            }
        }
    },

    exportYarnkendo: function (e) {
        var sheet = e.workbook.sheets[0];

        for (var rowIndex = 1; rowIndex < sheet.rows.length; rowIndex++) {
            var row = sheet.rows[rowIndex];
            for (var cellIndex = 2; cellIndex < row.cells.length; cellIndex++) {
                if (row.cells[cellIndex].value != null)
                    row.cells[cellIndex].value = parseFloat((row.cells[cellIndex].value + '').replaceAll(',', ''));
                row.cells[cellIndex].format = "#,##0";
            }
        }
    },

    exportFabrickendo: function (e) {
        var sheet = e.workbook.sheets[0];

        for (var rowIndex = 1; rowIndex < sheet.rows.length; rowIndex++) {
            var row = sheet.rows[rowIndex];
            for (var cellIndex = 3; cellIndex < row.cells.length; cellIndex++) {
                if (row.cells[cellIndex].value != null)
                    row.cells[cellIndex].value = parseFloat((row.cells[cellIndex].value + '').replaceAll(',', ''));
                row.cells[cellIndex].format = "#,##0";
            }
        }
    },

    exportPrintkendo: function (e) {
        var sheet = e.workbook.sheets[0];

        for (var rowIndex = 1; rowIndex < sheet.rows.length; rowIndex++) {
            var row = sheet.rows[rowIndex];
            for (var cellIndex = 3; cellIndex < row.cells.length; cellIndex++) {
                if (row.cells[cellIndex].value != null)
                    row.cells[cellIndex].value = parseFloat((row.cells[cellIndex].value + '').replaceAll(',', ''));
                row.cells[cellIndex].format = "#,##0";
            }
        }
    },

    onFromYearDataBound :function(){
        
        var frmYear = $('#FromWYear').data('kendoDropDownList');
        var data = frmYear.dataSource.data();
        $('#ToWYear').data('kendoDropDownList').dataSource.data(data);
        
    },
    filterDataClear: function () {
        ISS.common.executeActionAsynchronous('ClearFilters', null, function () {
        });

        $('#FilterPlant').data('kendoMultiSelect').dataSource.data([]);
        $('#FilterHeadSize').data('kendoMultiSelect').dataSource.data([]);
        $('#FilterCutSize').data('kendoMultiSelect').dataSource.data([]);
        $('#FilterDye').data('kendoMultiSelect').dataSource.data([]);
        $('#FilterMachine').data('kendoMultiSelect').dataSource.data([]);
        $('#FilterYarn').data('kendoMultiSelect').dataSource.data([]);

        // Collapse all grid panels
        ISS.common.collapsePanel('#panelbar-images', 1);
        ISS.common.collapsePanel('#panelbar-images', 2);
        ISS.common.collapsePanel('#panelbar-images', 3);
        ISS.common.collapsePanel('#panelbar-images', 4);
        ISS.common.collapsePanel('#panelbar-images', 5);
        ISS.common.collapsePanel('#panelbar-images', 6);
        ISS.common.collapsePanel('#panelbar-images', 7);
    },
    hideDynamicGridHeaders: function (grid) {
        $.each(grid.columns, function (index, item) {
            if (item.field.indexOf('_') != -1) {
                grid.hideColumn(item.field);
            }
        });

    },
    onPanelActivate: function (e) {
        var panel = e.item;
        if (panel) {
            if ($(e.item).hasClass('headcssClass')) {
                var grdHeadSize = $("#grdHeadSize").data("kendoGrid");
                if (grdHeadSize) {
                    grdHeadSize.refresh();
                }
            }
            else if ($(e.item).hasClass('machinecssClass')) {
                var grdHeadSize = $("#grdMachineUtilization").data("kendoGrid");
                if (grdHeadSize) {
                    grdHeadSize.refresh();
                }
            }
        }
    },

    searchDataTextiles: function () {
        return ISS.common.getFormData($('#frmTextiles'));
    },

    displayLoadingImage: function () {
        $('#TextileHead').html(ISS.common.Loader);
        $('#TextilesDye').html(ISS.common.Loader);
        $('#TextilesMachine').html(ISS.common.Loader);
        $('#TextilesYarn').html(ISS.common.Loader);
        $('#TextilesFabric').html(ISS.common.Loader);
        $('#TextilesPrint').html(ISS.common.Loader);
        $('#LoadMessage').show();
        $('#LoadMessage').html(ISS.common.Loader);
    },

    loadTextilesGrid: function () {

        // validations
        var business = $("#BusinessUnit").data('kendoMultiSelect').value().toString();
        var group = $("#TextileGroup").data('kendoMultiSelect').value().toString();
        //if (business == null || business == '') {
        //    ISS.common.notify.error("Please select a business unit");
        //    return false;
        //}
        //if (group == null || group == '') {
        //    ISS.common.notify.error("Please select a textile group");
        //    return false;
        //}
        textiles.Textiles.validator.validate($('#frmTextiles #BusinessUnit'));
        textiles.Textiles.validator.validate($('#frmTextiles #TextileGroup'));
        if (textiles.Textiles.validator.validate()) {
            textiles.displayLoadingImage();

            // Clear filter criterias
            textiles.filterDataClear();
            if ($('#planYear').val() == '') {
                $('#planYear').val($('#FromWYear').val());
            }
            ISS.common.executeActionAsynchronous('GetTextileGridAllocation', JSON.stringify(textiles.GridRead("headsize", "_headSizeDetails")),
                function (stat, heasizedata, xhr) {
                    if (stat && heasizedata) {
                        $('#TextileHead').empty().html($.trim(heasizedata));

                        // Fill filter drop downs
                        setTimeout(textiles.fillFilters, 3000);
                        textiles.diplayMessageAccordingToResult(xhr);
                        var grid = $("#grdHeadSize").data("kendoGrid");
                        if (grid != null) {
                            grid.table.on('click', '.lnkTextiles', function (e) {
                                textiles.gotoWOMHeadSize(e);
                            });
                        }

                        ISS.common.executeActionAsynchronous('GetTextileGridAllocation', JSON.stringify(textiles.GridRead("dye", "_dyeBleachDetails")), function (stat, dyebldata) {
                            if (stat && dyebldata) {
                                $('#TextilesDye').empty().html(dyebldata);

                                var grid = $("#grdDyeBleach").data("kendoGrid");
                                if (grid != null) {
                                    grid.table.on('click', '.lnkTextiles', function (e) {
                                        textiles.gotoWOMDyeBleach(e);
                                    });
                                }
                            }
                        }, null, null, 'html', null);

                        ISS.common.executeActionAsynchronous('GetTextileGridAllocation', JSON.stringify(textiles.GridRead("machine", "_machineUtilization")), function (stat, machineutdata) {
                            if (stat && machineutdata) {
                                $('#TextilesMachine').empty().html(machineutdata);

                                var grid = $("#grdMachineUtilization").data("kendoGrid");
                                if (grid != null) {
                                    grid.table.on('click', '.lnkTextiles', function (e) {
                                        textiles.gotoWOMMachineUtilization(e);
                                    });
                                }
                            }
                        }, null, null, 'html', null);

                        ISS.common.executeActionAsynchronous('GetTextileGridAllocation', JSON.stringify(textiles.GridRead("yarnitem", "_yarnDetails")), function (stat, yarnitemdata) {
                            if (stat && yarnitemdata) {
                                $('#TextilesYarn').empty().html(yarnitemdata);

                                var grid = $("#grdYarnDetails").data("kendoGrid");
                                if (grid != null) {
                                    grid.table.on('click', '.lnkTextiles', function (e) {
                                        textiles.gotoWOMYarn(e);
                                    });
                                }
                            }
                        }, null, null, 'html', null);

                        ISS.common.executeActionAsynchronous('GetTextileGridAllocation', JSON.stringify(textiles.GridRead("fabricitem", "_fabricDetails")), function (stat, fabricitemdata) {
                            if (stat && fabricitemdata) {
                                $('#TextilesFabric').empty().html(fabricitemdata);

                                var grid = $("#grdFabricDetails").data("kendoGrid");
                                if (grid != null) {
                                    grid.table.on('click', '.lnkTextiles', function (e) {
                                        textiles.gotoWOMFabric(e);
                                    });
                                }
                            }
                        }, null, null, 'html', null);

                        ISS.common.executeActionAsynchronous('GetTextileGridAllocation', JSON.stringify(textiles.GridRead("print", "_printDetails")), function (stat, printitemdata) {
                            if (stat && printitemdata) {
                                $('#TextilesPrint').empty().html(printitemdata);

                                var grid = $("#grdPrintDetails").data("kendoGrid");
                                if (grid != null) {
                                    grid.table.on('click', '.lnkTextiles', function (e) {
                                        textiles.gotoWOMPrint(e);
                                    });
                                }
                            }
                        }, null, null, 'html', null);
                    }
                }, null, null, 'html', null); //End First Ajax call

            ISS.common.collapsePanel('#panelbar-images', 1);
        }
        return false;
    },

    diplayMessageAccordingToResult: function (xhr) {
        // Display message acccording to result
        ISS.common.notify.options.autoHideAfter = 0;
        var count = xhr.getResponseHeader("count");
        if (count == "-1") {
            ISS.common.notify.error("Unexpected error occured. Please try again.");
        }
        else if (count == "0") {
            ISS.common.notify.info("No records to display. Please search again with another criteria");
        }
        else {

            ISS.common.notify.success("Search results have been loaded. <br/>Please click on a Section Title or a Section drop down arrow to add Filter Criteria and/or review results.");


        }
        $(document).one("click", function () {
            ISS.common.notify.hide();
        })
        ISS.common.notify.options.autoHideAfter = 5000;
        $('#LoadMessage').html(' ');
    },

    fillFilters: function () {
        var plant = $('#FilterPlant').data('kendoMultiSelect');
        plant.dataSource.read();
        var head = $('#FilterHeadSize').data('kendoMultiSelect');
        head.dataSource.read();
        var cut = $('#FilterCutSize').data('kendoMultiSelect');
        cut.dataSource.read();
        var dye = $('#FilterDye').data('kendoMultiSelect');
        dye.dataSource.read();
        var machine = $('#FilterMachine').data('kendoMultiSelect');
        machine.dataSource.read();
        var yarn = $('#FilterYarn').data('kendoMultiSelect');
        yarn.dataSource.read();
    },
    gotoWorkorder: function () {
        var url = location.protocol + '//' + location.hostname + $(this).data('url');
        var queryString = $.param({
            PlantFilter: $('#FilterPlant').val(), HeadsizeFilter: $('#FilterHeadSize').val(), DyeFilter: $('#FilterDye').val(), CutSizeFilter: $('#FilterCutSize').val(),
            MachineFilter: $('#FilterMachine').val(), YarnFilter: $('#FilterYarn').val(),
        });
        if (queryString != null && queryString.length > 0) {
            url += "?" + queryString;
        }
        window.open(url, "_blank");
        return false
    },

    plantFillData: function () {
        return { filter: "Plant", plant: null };
    },

    headFillData: function () {
        return {
            filter: "headsize", plant: $('#FilterPlant').data('kendoMultiSelect').value().join()
        };
    },

    cutFillData: function () {
        return { filter: "cut", plant: $('#FilterPlant').data('kendoMultiSelect').value().join() };
    },

    machineFillData: function () {
        return {
            filter: "machine", plant: $('#FilterPlant').data('kendoMultiSelect').value().join()
        };
    },

    yarnFillData: function () {
        return {
            filter: "yarn", plant: $('#FilterPlant').data('kendoMultiSelect').value().join()
        };
    },

    dyeFillData: function () {
        return { filter: "dye", plant: $('#FilterPlant').data('kendoMultiSelect').value().join() };
    },

    loadFilteredTextilesGrid: function () {
        textiles.displayLoadingImage();

        ISS.common.executeActionAsynchronous('GetFilteredDataSource', JSON.stringify(textiles.FilterRead("headsize", "_headSizeDetails")), function (stat, data, xhr) {
            if (stat && data) {
                $('#TextileHead').empty().html(data);
                textiles.diplayMessageAccordingToResult(xhr);
//asif

                var grid = $("#grdHeadSize").data("kendoGrid");
                if (grid != null) {
                    grid.table.on('click', '.lnkTextiles', function (e) {
                        textiles.gotoWOMHeadSize(e);
                    });
                }

            }
        }, null, null, 'html', null);

        ISS.common.executeActionAsynchronous('GetFilteredDataSource', JSON.stringify(textiles.FilterRead("dye", "_dyeBleachDetails")), function (stat, data) {
            if (stat && data) {
                $('#TextilesDye').empty().html(data);
//asif
                var grid = $("#grdDyeBleach").data("kendoGrid");
                if (grid != null) {
                    grid.table.on('click', '.lnkTextiles', function (e) {
                        textiles.gotoWOMDyeBleach(e);
                    });
                }
            }
        }, null, null, 'html', null);

        ISS.common.executeActionAsynchronous('GetFilteredDataSource', JSON.stringify(textiles.FilterRead("machine", "_machineUtilization")), function (stat, data) {
            if (stat && data) {
                $('#TextilesMachine').empty().html(data);

//asif
                var grid = $("#grdMachineUtilization").data("kendoGrid");
                if (grid != null) {
                    grid.table.on('click', '.lnkTextiles', function (e) {
                        textiles.gotoWOMMachineUtilization(e);
                    });
                }

            }
        }, null, null, 'html', null);

        ISS.common.executeActionAsynchronous('GetFilteredDataSource', JSON.stringify(textiles.FilterRead("yarnitem", "_yarnDetails")), function (stat, data) {
            if (stat && data) {
                $('#TextilesYarn').empty().html(data);

//asif

                var grid = $("#grdYarnDetails").data("kendoGrid");
                if (grid != null) {
                    grid.table.on('click', '.lnkTextiles', function (e) {
                        textiles.gotoWOMYarn(e);
                    });
                }
            }
        }, null, null, 'html', null);

        ISS.common.executeActionAsynchronous('GetFilteredDataSource', JSON.stringify(textiles.FilterRead("fabricitem", "_fabricDetails")), function (stat, data) {
            if (stat && data) {
                $('#TextilesFabric').empty().html(data);
//asif

                var grid = $("#grdFabricDetails").data("kendoGrid");
                if (grid != null) {
                    grid.table.on('click', '.lnkTextiles', function (e) {
                        textiles.gotoWOMFabric(e);
                    });
                }
            }
        }, null, null, 'html', null);

        ISS.common.executeActionAsynchronous('GetFilteredDataSource', JSON.stringify(textiles.FilterRead("printitem", "_printDetails")), function (stat, data) {
            if (stat && data) {
                $('#TextilesPrint').empty().html(data);
//asif
                var grid = $("#grdPrintDetails").data("kendoGrid");
                if (grid != null) {
                    grid.table.on('click', '.lnkTextiles', function (e) {
                        textiles.gotoWOMPrint(e);
                    });
                }

            }
        }, null, null, 'html', null);

        return false;
    },

    onTextileGridDataBound: function (e) {
        var gridName = e.sender.content.context.id;
        var grid = $("#" + gridName).data("kendoGrid");
        var data = grid.dataSource.data();

        $.each(data, function (i, row) {
            var status = '';
            switch (gridName) {
                case "grdHead":
                case "grdHeadSize":
                    status = row.HeadSize;
                    break;
                case "grdDye":
                case "grdDyeBleach":
                    status = row.Dye;
                    break;
                case "grdMachine":
                case "grdMachineUtilization":
                    status = row.Machine;
                    break;
                case "grdYarn":
                    status = row.Yarn;
                    break;
            }
            var currentRow = $('tr[data-uid="' + row.uid + '"]');
            var columnCount = $(currentRow).children('td').length;
            var color = '';
            var isNet = false;

            switch (status) {
                case "Suggested":
                    color = "blue";
                    break;
                case "Locked":
                    color = "darkgreen";
                    break;
                case "Released":
                    color = "purple";
                    break;
                case "Alloc":
                    currentRow.find('a').addClass('not-active').removeAttr('href').css('color', 'purple');
                    color = "purple";
                    break;
                case "Total":
                    currentRow.find('a').addClass('not-active').removeAttr('href').css('color', 'black');
                    color = "black";
                    break;
                case "Net":
                    currentRow.find('a').addClass('not-active').removeAttr('href').css('color', 'black');
                    isNet = true;
                    color = "Red";
                    break;
            }

            if (isNet) {
                for (var i = 2; i <= columnCount; i++) {
                    var column = $(currentRow).children('td').eq(i);
                    var cellValue = parseInt(column.text());

                    if (isNaN(cellValue) || cellValue < 0) {
                        column.find('a').css("color", color);
                        column.css("color", color);
                    }
                }

            }
            else {
                for (var i = 1; i < columnCount - 1; i++) {
                    $(currentRow).children('td').eq(i).find('a').css("color", color);
                    $(currentRow).children('td').eq(i).css("color", color);
                }
            }

        });
    },

    onBusUnitChange: function (e) {
        var TextileGroup = $("#TextileGroup").data("kendoMultiSelect");
        TextileGroup.value([]);
        TextileGroup.dataSource.read();

    },

    onYarnItemDataBound: function (e) {
        var grid = $("#grdYarnDetails").data("kendoGrid");
        var data = grid.dataSource.data();

        $.each(data, function (i, row) {
            var status = '';
            var color = '';
            var currentRow = $('tr[data-uid="' + row.uid + '"]');
            var columnCount = $(currentRow).children('td').length;
            var column = $(currentRow).children('td').eq(columnCount - 1);
            var colIdx = $("td", currentRow).index(this);
            //var columnIndex = $(currentRow).closest('td').index();
            //var colName = grid.find('th').eq(columnIndex).text();
            var cellValue = parseInt(column.text());

            var SuggColumn = $(currentRow).children('td').eq(2);
            SuggColumn.find('a').css("color", "blue");
            $(currentRow).children('td').eq(3).find('a').css("color", "darkgreen");
            $(currentRow).children('td').eq(4).find('a').css("color", "purple");
            if (isNaN(cellValue) || cellValue < 0) {
                column.css("color", "red");
                column.find('a').css("color", "red");
            }
            //else {
            //    column.find('a').css("color", color);
            //}
        });
    },

    //Newly Added
    onFabricItemDataBound: function (e) {
        var grid = $("#grdFabricDetails").data("kendoGrid");
        var data = grid.dataSource.data();

        $.each(data, function (i, row) {
            var status = '';
            var color = '';
            var currentRow = $('tr[data-uid="' + row.uid + '"]');
            var columnCount = $(currentRow).children('td').length;
            var column = $(currentRow).children('td').eq(columnCount - 1);
            var colIdx = $("td", currentRow).index(this);
            var cellValue = parseInt(column.text());

            var SuggColumn = $(currentRow).children('td').eq(3);
            SuggColumn.find('a').css("color", "blue");
            $(currentRow).children('td').eq(4).find('a').css("color", "darkgreen");
            $(currentRow).children('td').eq(5).find('a').css("color", "purple");
            if (isNaN(cellValue) || cellValue < 0) {
                column.css("color", "red");
                column.find('a').css("color", "red");
            }
        });
    },

    onPrintItemDataBound: function (e) {
        var grid = $("#grdPrintDetails").data("kendoGrid");
        var data = grid.dataSource.data();

        $.each(data, function (i, row) {
            var status = '';
            var color = '';
            var currentRow = $('tr[data-uid="' + row.uid + '"]');
            var columnCount = $(currentRow).children('td').length;
            var column = $(currentRow).children('td').eq(columnCount - 1);
            var colIdx = $("td", currentRow).index(this);
            var cellValue = parseInt(column.text());

            var SuggColumn = $(currentRow).children('td').eq(2);
            SuggColumn.find('a').css("color", "blue");
            $(currentRow).children('td').eq(3).find('a').css("color", "darkgreen");
            $(currentRow).children('td').eq(4).find('a').css("color", "purple");
            if (isNaN(cellValue) || cellValue < 0) {
                column.css("color", "red");
                column.find('a').css("color", "red");
            }
        });
    },
    //End

    fabricData: function () {

        var busUnit = $("#BusinessUnit").data("kendoMultiSelect");
        var bu = busUnit.value().toString();

        var passdata = {
            BusinessUnit: bu
        }
        return passdata;
    },
    GridRead: function (allocGrid, viewName) {
        var planFromDate = $("#FromWYear").val() != '' ? $("#FromWYear").val() : $("#planYear").val();
        var planToDate = $("#ToWYear").val() != '' ? $("#ToWYear").val() : $("#planYear").val();
        var search = {
            BusinessUnit: $("#BusinessUnit").data('kendoMultiSelect').value().join(),
            TextileGroup: $("#TextileGroup").data('kendoMultiSelect').value().join(),
            FromWYear: planFromDate,
            ToWYear: planToDate,
            IsSuggestedLotIncluded: $("#IsSuggestedLotIncluded").is(':checked'),
            Planner: $('#Planner').val(),
            ViewName: viewName,
            AllocGrid: allocGrid
        };
        return search;
    },
    FilterRead: function (allocGrid, viewName) {
        var model =
         {
             PlantFilter: $('#FilterPlant').data('kendoMultiSelect').value().join(),
             IsSuggestIncluded: $("#IsSuggestedLotIncluded").is(':checked'),
             HeadsizeFilter: $('#FilterHeadSize').data('kendoMultiSelect').value().join(),
             DyeFilter: $('#FilterDye').data('kendoMultiSelect').value().join(),
             MachineFilter: $('#FilterMachine').data('kendoMultiSelect').value().join(),
             YarnFilter: $('#FilterYarn').data('kendoMultiSelect').value().join(),
             CutSizeFilter: $('#FilterCutSize').data('kendoMultiSelect').value().join(),
             ViewName: viewName,
             AllocationGrid: allocGrid
         };
        $('#filteredData').val(JSON.stringify(model));
        return model;
    },

    onPlantChange: function () {
        var headFilter = $('#FilterHeadSize').data('kendoMultiSelect');
        headFilter.value([]);
        headFilter.dataSource.read();

        var cutFilter = $('#FilterCutSize').data('kendoMultiSelect');
        cutFilter.value([]);
        cutFilter.dataSource.read();

        var dyeFilter = $('#FilterDye').data('kendoMultiSelect');
        dyeFilter.value([]);
        dyeFilter.dataSource.read();

        var machineFilter = $('#FilterMachine').data('kendoMultiSelect');
        machineFilter.value([]);
        machineFilter.dataSource.read();

        var yarnFilter = $('#FilterYarn').data('kendoMultiSelect');
        yarnFilter.value([]);
        yarnFilter.dataSource.read();

    },
    onFromYearChange: function () {
        if ($('#planYear').val() == '') {
            $('#planYear').val($('#FromWYear').val());
        }
    },

    gotoWOMHeadSize: function (e) {

        if (e.currentTarget.className.indexOf("not-active") >= 0)
            return false;

        var grid = $("#grdHeadSize").data("kendoGrid");

        var dataItem = grid.dataItem($(e.currentTarget).closest("tr"));

        //var colIdx = $(e.target).closest('td').index();
        var rowIdx = $(e.target).closest('tr').index();

        

        //var colName = $('#grdHeadSize').find('th').eq(colIdx).text();

        var pattern = /_/ig;
        var colName = $(e.currentTarget).data('headsize').replace(pattern, '');

        var suggested = 'false', locked = 'false', spillover = 'false', released = 'false';
        var headSz = dataItem.HeadSize;
        if (headSz == 'Suggested') {
            suggested = 'true';
        }
        else if (headSz == 'Locked') {
            locked = 'true';
        }
        else if (headSz == 'Released') {
            released = 'true';
        }

        var planFromDate = $("#FromWYear").val() != '' ? $("#FromWYear").val() : $("#planYear").val();
        var planToDate = $("#ToWYear").val() != '' ? $("#ToWYear").val() : $("#planYear").val();
        var search = {
            FromWYear: planFromDate,
            ToWYear: planToDate
        };

        search = JSON.stringify(search);

        var moreWks = 1;
        var endDate = '';
        ISS.common.executeActionAsynchronous("../Textiles/GetWeekDetails", search, function (stat, data) {
            if (stat && data) {
              //  moreWks = data.NoWeeks;

                var plant = '';
                var endDate = data.EndDate;
                if (endDate) endDate = ISS.common.parseDate(endDate);
                endDate = textiles.getFormattedDate(endDate);
//cijith
                var startDate = data.StartDate;
                if (startDate) startDate = ISS.common.parseDate(startDate);
                startDate = textiles.getFormattedDate(startDate);
                moreWks = (data.ToWYear.substr(0, 2) - data.FromWYear.substr(0, 2)) + 1;

                if (moreWks < 0) {
                    moreWks += 52;
                }

                var url = location.protocol + '//' + location.hostname + textiles.const.urlGetWOM;

                if (dataItem.PlantCut != null) {
                    var arr = dataItem.PlantCut.split(' ');
                    if (arr.length > 0)
                        plant = arr[0];
                }
                else {
                    var displayedData = $("#grdHeadSize").data().kendoGrid.dataSource.view();
                    var suggData = null;
                    if (displayedData.length > 0) {
                        if (rowIdx < displayedData.length) {
                            if (headSz == 'Locked') {
                                suggData = displayedData[rowIdx - 1];
                            }
                            else if (headSz == 'Released') {
                                suggData = displayedData[rowIdx - 2];
                            }

                            if (suggData != null) {
                                if (suggData.PlantCut != null) {
                                    var arr = suggData.PlantCut.split(' ');
                                    if (arr.length > 0)
                                        plant = arr[0];
                                }
                            }
                        }
                    }

                }
                var queryString = $.param({
                    MFGPlant: plant, Planner: $('#Planner').val(), Week: startDate, CylinderSize: colName,
                    TextileGroup: $("#TextileGroup").data('kendoMultiSelect').value().toString(),
                    BusinessUnit: $("#BusinessUnit").data('kendoMultiSelect').value().toString(), MoreWeeks: moreWks,
                    SuggestedLots: suggested, LockedLots: locked, SpillOver: "false",
                    ReleasedLots: "false", ReleasedLotsThisWeek: released,
                    autoLoad: true

                });


                if (queryString != null && queryString.length > 0) {
                    url += "?" + queryString;
                }
                window.open(url, "_blank");
            }
            else {
                moreWks = 1;

                var plant = '';


                var url = location.protocol + '//' + location.hostname + textiles.const.urlGetWOM;

                var arr = dataItem.PlantCut.split(' ');
                if (arr.length > 0)
                    plant = arr[0];
                var queryString = $.param({
                    MFGPlant: plant, Planner: $('#Planner').val(),
                    CylinderSize: '0', TextileGroup: $("#TextileGroup").data('kendoMultiSelect').value().toString(),
                    BusinessUnit: $("#BusinessUnit").data('kendoMultiSelect').value().toString(), MoreWeeks: moreWks,
                    SuggestedLots: suggested, LockedLots: locked, SpillOver: "false",
                    ReleasedLots: "false", ReleasedLotsThisWeek: released,
                    autoLoad: true

                });


                if (queryString != null && queryString.length > 0) {
                    url += "?" + queryString;
                }
                window.open(url, "_blank");
            }
        }, 'POST');


        return false
    },

    gotoWOMDyeBleach: function (e) {

        if (e.currentTarget.className.indexOf("not-active") >= 0)
            return false;

        var grid = $("#grdDyeBleach").data("kendoGrid");

        var dataItem = grid.dataItem($(e.currentTarget).closest("tr"));

        var colIdx = $(e.target).closest('td').index();
        var rowIdx = $(e.target).closest('tr').index();

        //var colName = $('#grdDyeBleach').find('th').eq(colIdx).text();
        var pattern = /_/ig;
        var colName = $(e.currentTarget).data('dye').replace(pattern, '');

        if (colName == 'Bleach')
            colName = 'B';
        else if (colName == 'Dye')
            colName = 'D';

        var suggested = 'false', locked = 'false', spillover = 'false', released = 'false';
        var headSz = dataItem.Dye;
        if (headSz == 'Suggested') {
            suggested = 'true';
        }
        else if (headSz == 'Locked') {
            locked = 'true';
        }
        else if (headSz == 'Released') {
            released = 'true';
        }

        var planFromDate = $("#FromWYear").val() != '' ? $("#FromWYear").val() : $("#planYear").val();
        var planToDate = $("#ToWYear").val() != '' ? $("#ToWYear").val() : $("#planYear").val();
        var search = {
            FromWYear: planFromDate,
            ToWYear: planToDate
        };

        search = JSON.stringify(search);

        var moreWks = 1;
        var endDate = '';
        ISS.common.executeActionAsynchronous("../Textiles/GetWeekDetails", search, function (stat, data) {
            if (stat && data) {
               // moreWks = data.NoWeeks;

                var plant = '';
                var endDate = data.EndDate;
                if (endDate) endDate = ISS.common.parseDate(endDate);
                endDate = textiles.getFormattedDate(endDate);
//cijith
                var startDate = data.StartDate;
                if (startDate) startDate = ISS.common.parseDate(startDate);
                startDate = textiles.getFormattedDate(startDate);
                moreWks = (data.ToWYear.substr(0, 2) - data.FromWYear.substr(0, 2)) + 1;

                if (moreWks < 0) {
                    moreWks += 52;
                }

                var url = location.protocol + '//' + location.hostname + textiles.const.urlGetWOM;

                if (dataItem.Plant != null) {
                    plant = dataItem.Plant;
                }
                else {
                    var displayedData = $("#grdDyeBleach").data().kendoGrid.dataSource.view();
                    var suggData = null;
                    if (displayedData.length > 0) {
                        if (rowIdx < displayedData.length) {
                            if (headSz == 'Locked') {
                                suggData = displayedData[rowIdx - 1];
                            }
                            else if (headSz == 'Released') {
                                suggData = displayedData[rowIdx - 2];
                            }

                            if (suggData.Plant != null) {
                                plant = suggData.Plant;
                            }
                        }
                    }

                }
                var queryString = $.param({
                    MFGPlant: plant, Planner: $('#Planner').val(), Week: startDate, DyeBle: colName,
                    TextileGroup: $("#TextileGroup").data('kendoMultiSelect').value().toString(),
                    BusinessUnit: $("#BusinessUnit").data('kendoMultiSelect').value().toString(), MoreWeeks: moreWks,
                    SuggestedLots: suggested, LockedLots: locked, SpillOver: "false",
                    ReleasedLots: "false", ReleasedLotsThisWeek: released,
                    autoLoad: true

                });


                if (queryString != null && queryString.length > 0) {
                    url += "?" + queryString;
                }
                window.open(url, "_blank");
            }
            else {
                moreWks = 1;

                var plant = '';


                var url = location.protocol + '//' + location.hostname + textiles.const.urlGetWOM;

                var arr = dataItem.Plant.split(' ');
                if (arr.length > 0)
                    plant = arr[0];
                var queryString = $.param({
                    MFGPlant: plant, Planner: $('#Planner').val(),
                    DyeBle: colName, TextileGroup: $("#TextileGroup").data('kendoMultiSelect').value().toString(),
                    BusinessUnit: $("#BusinessUnit").data('kendoMultiSelect').value().toString(), MoreWeeks: moreWks,
                    SuggestedLots: suggested, LockedLots: locked, SpillOver: "false",
                    ReleasedLots: "false", ReleasedLotsThisWeek: released,
                    autoLoad: true

                });


                if (queryString != null && queryString.length > 0) {
                    url += "?" + queryString;
                }
                window.open(url, "_blank");
            }
        }, 'POST');


        return false
    },

    /*
     gotoWOMYarn: function (e) {
 
         if (e.currentTarget.className.indexOf("not-active") >= 0)
             return false;
 
         var grid = $("#grdYarnDetails").data("kendoGrid");
 
         var dataItem = grid.dataItem($(e.currentTarget).closest("tr"));
 
         var colIdx = $(e.target).closest('td').index();
         var rowIdx = $(e.target).closest('tr').index();
 
         var colName = $('#grdYarnDetails').find('th').eq(colIdx).text();
 
         var yarn = dataItem.YarnItem;
         var suggested = 'false', locked = 'false', spillover = 'false', released = 'false';
         
         if (colName == 'Suggested') {
             suggested = 'true';
         }
         else if (colName == 'Locked') {
             locked = 'true';
         }
         else if (colName == 'Released') {
             released = 'true';
         }
 
         var planFromDate = $("#FromWYear").val() != '' ? $("#FromWYear").val() : $("#planYear").val();
         var planToDate = $("#ToWYear").val() != '' ? $("#ToWYear").val() : $("#planYear").val();
         var search = {
             FromWYear: planFromDate,
             ToWYear: planToDate
         };
 
         search = JSON.stringify(search);
 
         var moreWks = 1;
         var endDate = '';
         ISS.common.executeActionAsynchronous("../Textiles/GetWeekDetails", search, function (stat, data) {
             if (stat && data) {
                 moreWks = data.NoWeeks;
 
                 var plant = '';
                 var endDate = data.EndDate;
                 if (endDate) endDate = ISS.common.parseDate(endDate);
                 endDate = textiles.getFormattedDate(endDate);
                 var url = location.protocol + '//' + location.hostname + textiles.const.urlGetWOM;
 
                 if (dataItem.Plant != null) {
                     plant = dataItem.Plant;
                 }
                 else {
                     var displayedData = $("#grdYarnDetails").data().kendoGrid.dataSource.view();
                     var suggData = null;
                     if (displayedData.length > 0) {
                         if (rowIdx < displayedData.length) {
                             if (headSz == 'Locked') {
                                 suggData = displayedData[rowIdx - 1];
                             }
                             else if (headSz == 'Released') {
                                 suggData = displayedData[rowIdx - 2];
                             }
 
                             if (suggData.Plant != null) {
                                 plant = suggData.Plant;
                             }
                         }
                     }
 
                 }
                 var queryString = $.param({
                     MFGPlant: plant, Planner: $('#Planner').val(), Week: endDate, Yarn: yarn,
                     TextileGroup: $("#TextileGroup").data('kendoMultiSelect').value().toString(),
                     BusinessUnit: $("#BusinessUnit").data('kendoMultiSelect').value().toString(), MoreWeeks: moreWks,
                     SuggestedLots: suggested, LockedLots: locked, SpillOver: "false",
                     ReleasedLots: "false", ReleasedLotsThisWeek: released,
                     autoLoad: true
 
                 });
 
 
                 if (queryString != null && queryString.length > 0) {
                     url += "?" + queryString;
                 }
                 window.open(url, "_blank");
             }
             else {
                 moreWks = 1;
 
                 var plant = '';
 
 
                 var url = location.protocol + '//' + location.hostname + textiles.const.urlGetWOM;
 
                 var arr = dataItem.Plant.split(' ');
                 if (arr.length > 0)
                     plant = arr[0];
                 var queryString = $.param({
                     MFGPlant: plant, Planner: $('#Planner').val(),
                     DyeBle: colName, TextileGroup: $("#TextileGroup").data('kendoMultiSelect').value().toString(),
                     BusinessUnit: $("#BusinessUnit").data('kendoMultiSelect').value().toString(), MoreWeeks: moreWks,
                     SuggestedLots: suggested, LockedLots: locked, SpillOver: "false",
                     ReleasedLots: "false", ReleasedLotsThisWeek: released,
                     autoLoad: true
 
                 });
 
 
                 if (queryString != null && queryString.length > 0) {
                     url += "?" + queryString;
                 }
                 window.open(url, "_blank");
             }
         }, 'POST');
 
 
         return false
     },
     */

    gotoWOMMachineUtilization: function (e) {

        if (e.currentTarget.className.indexOf("not-active") >= 0)
            return false;

        var grid = $("#grdMachineUtilization").data("kendoGrid");

        var dataItem = grid.dataItem($(e.currentTarget).closest("tr"));

        var colIdx = $(e.target).closest('td').index();
        var rowIdx = $(e.target).closest('tr').index();

        //var colName = $('#grdMachineUtilization').find('th').eq(colIdx + 2).text();
        
        //var colName = $('#grdMachineUtilization').find('th').eq(colIdx).text();

        var pattern = /_/ig;
        var colName = $(e.currentTarget).data('machine').replace(pattern, '');

        var suggested = 'false', locked = 'false', spillover = 'false', released = 'false';
        var headSz = dataItem.Machine;
        if (headSz == 'Suggested') {
            suggested = 'true';
        }
        else if (headSz == 'Locked') {
            locked = 'true';
        }
        else if (headSz == 'Released') {
            released = 'true';
        }

        var planFromDate = $("#FromWYear").val() != '' ? $("#FromWYear").val() : $("#planYear").val();
        var planToDate = $("#ToWYear").val() != '' ? $("#ToWYear").val() : $("#planYear").val();
        var search = {
            FromWYear: planFromDate,
            ToWYear: planToDate
        };

        search = JSON.stringify(search);

        var moreWks = 1;
        var endDate = '';
        ISS.common.executeActionAsynchronous("../Textiles/GetWeekDetails", search, function (stat, data) {
            if (stat && data) {
              //  moreWks = data.NoWeeks;

                var plant = '';
                var endDate = data.EndDate;
                if (endDate) endDate = ISS.common.parseDate(endDate);
                endDate = textiles.getFormattedDate(endDate);
//cijith
                var startDate = data.StartDate;
                if (startDate) startDate = ISS.common.parseDate(startDate);
                startDate = textiles.getFormattedDate(startDate);
                moreWks = (data.ToWYear.substr(0, 2) - data.FromWYear.substr(0, 2)) + 1;

                if (moreWks < 0) {
                    moreWks += 52;
                }

                var url = location.protocol + '//' + location.hostname + textiles.const.urlGetWOM;

                if (dataItem.Plant != null) {
                    plant = dataItem.Plant;
                }
                else {
                    var displayedData = $("#grdMachineUtilization").data().kendoGrid.dataSource.view();
                    var suggData = null;
                    if (displayedData.length > 0) {
                        if (rowIdx < displayedData.length) {
                            if (headSz == 'Locked') {
                                suggData = displayedData[rowIdx - 1];
                            }
                            else if (headSz == 'Released') {
                                suggData = displayedData[rowIdx - 2];
                            }

                            if (suggData.Plant != null) {
                                plant = suggData.Plant;
                            }
                        }
                    }

                }
                var queryString = $.param({
                    MFGPlant: plant, Planner: $('#Planner').val(), Week: startDate, Machine: colName,
                    TextileGroup: $("#TextileGroup").data('kendoMultiSelect').value().toString(),
                    BusinessUnit: $("#BusinessUnit").data('kendoMultiSelect').value().toString(),
                    SuggestedLots: suggested, LockedLots: locked, SpillOver: "false",
                    ReleasedLots: "false", ReleasedLotsThisWeek: released, MoreWeeks: moreWks,
                    autoLoad: true

                });


                if (queryString != null && queryString.length > 0) {
                    url += "?" + queryString;
                }
                window.open(url, "_blank");
            }
            else {
                moreWks = 1;

                var plant = '';


                var url = location.protocol + '//' + location.hostname + textiles.const.urlGetWOM;

                var arr = dataItem.Plant.split(' ');
                if (arr.length > 0)
                    plant = arr[0];
                var queryString = $.param({
                    MFGPlant: plant, Planner: $('#Planner').val(),
                    Machine: colName, TextileGroup: $("#TextileGroup").data('kendoMultiSelect').value().toString(),
                    BusinessUnit: $("#BusinessUnit").data('kendoMultiSelect').value().toString(), MoreWeeks: moreWks,
                    SuggestedLots: suggested, LockedLots: locked, SpillOver: "false",
                    ReleasedLots: "false", ReleasedLotsThisWeek: released,
                    autoLoad: true

                });


                if (queryString != null && queryString.length > 0) {
                    url += "?" + queryString;
                }
                window.open(url, "_blank");
            }
        }, 'POST');


        return false
    },

    gotoWOMYarn: function (e) {

        if (e.currentTarget.className.indexOf("not-active") >= 0)
            return false;

        var grid = $("#grdYarnDetails").data("kendoGrid");

        var dataItem = grid.dataItem($(e.currentTarget).closest("tr"));

        var colIdx = $(e.target).closest('td').index();
        var rowIdx = $(e.target).closest('tr').index();

        //var colName = $('#grdYarnDetails').find('th').eq(colIdx).text();

        var pattern = /_/ig;
        var colName = $(e.currentTarget).data('yarn').replace(pattern, '');

        var yarn = dataItem.YarnItem;
        var suggested = 'false', locked = 'false', spillover = 'false', released = 'false';

        if (colName == 'Suggested') {
            suggested = 'true';
        }
        else if (colName == 'Locked') {
            locked = 'true';
        }
        else if (colName == 'Released') {
            released = 'true';
        }

        var planFromDate = $("#FromWYear").val() != '' ? $("#FromWYear").val() : $("#planYear").val();
        var planToDate = $("#ToWYear").val() != '' ? $("#ToWYear").val() : $("#planYear").val();
        var search = {
            FromWYear: planFromDate,
            ToWYear: planToDate
        };

        search = JSON.stringify(search);

        var moreWks = 1;
        var endDate = '';
        ISS.common.executeActionAsynchronous("../Textiles/GetWeekDetails", search, function (stat, data) {
            if (stat && data) {
                //moreWks = data.NoWeeks;

                var plant = '';
                var endDate = data.EndDate;
                if (endDate) endDate = ISS.common.parseDate(endDate);
                endDate = textiles.getFormattedDate(endDate);
//cijith
                var startDate = data.StartDate;
                if (startDate) startDate = ISS.common.parseDate(startDate);
                startDate = textiles.getFormattedDate(startDate);
                moreWks = (data.ToWYear.substr(0, 2) - data.FromWYear.substr(0, 2)) + 1;

                if (moreWks < 0) {
                    moreWks += 52;
                }

                var url = location.protocol + '//' + location.hostname + textiles.const.urlGetWOM;

                if (dataItem.Plant != null) {
                    plant = dataItem.Plant;
                }
                else {
                    var displayedData = $("#grdYarnDetails").data().kendoGrid.dataSource.view();
                    var suggData = null;
                    if (displayedData.length > 0) {
                        suggData = displayedData[rowIdx];
                        if (rowIdx < displayedData.length) {
                            if (suggData.Plant != null) {
                                plant = suggData.Plant;
                            }
                            else {
                                for (var k = rowIdx; k >= 0; k--) {
                                    if (displayedData[k].Plant != null) {
                                        plant = displayedData[k].Plant;
                                        break;
                                    }
                                }

                                if (plant == '') {
                                    plant = displayedData[0].Plant;
                                }
                            }
                            
                        }
                    }

                }
                var queryString = $.param({
                    MFGPlant: plant, Planner: $('#Planner').val(), Week: startDate, Yarn: yarn,
                    TextileGroup: $("#TextileGroup").data('kendoMultiSelect').value().toString(),
                    BusinessUnit: $("#BusinessUnit").data('kendoMultiSelect').value().toString(), MoreWeeks: moreWks,
                    SuggestedLots: suggested, LockedLots: locked, SpillOver: "false",
                    ReleasedLots: "false", ReleasedLotsThisWeek: released,
                    autoLoad: true

                });


                if (queryString != null && queryString.length > 0) {
                    url += "?" + queryString;
                }
                window.open(url, "_blank");
            }
            else {
                moreWks = 1;

                var plant = '';


                var url = location.protocol + '//' + location.hostname + textiles.const.urlGetWOM;

                var arr = dataItem.Plant.split(' ');
                if (arr.length > 0)
                    plant = arr[0];
                var queryString = $.param({
                    MFGPlant: plant, Planner: $('#Planner').val(),
                    DyeBle: colName, TextileGroup: $("#TextileGroup").data('kendoMultiSelect').value().toString(),
                    BusinessUnit: $("#BusinessUnit").data('kendoMultiSelect').value().toString(), MoreWeeks: moreWks,
                    SuggestedLots: suggested, LockedLots: locked, SpillOver: "false",
                    ReleasedLots: "false", ReleasedLotsThisWeek: released,
                    autoLoad: true

                });


                if (queryString != null && queryString.length > 0) {
                    url += "?" + queryString;
                }
                window.open(url, "_blank");
            }
        }, 'POST');


        return false
    },

    //Newly Added   
    gotoWOMFabric: function (e) {

            if (e.currentTarget.className.indexOf("not-active") >= 0)
                return false;
          
            var grid = $("#grdFabricDetails").data("kendoGrid");

            var dataItem = grid.dataItem($(e.currentTarget).closest("tr"));

            var colIdx = $(e.target).closest('td').index();
            var rowIdx = $(e.target).closest('tr').index();
           
            //var colName = $('#grdYarnDetails').find('th').eq(colIdx).text();

            var pattern = /_/ig;
            var colName = $(e.currentTarget).data('fabric').replace(pattern, '');

            var fabric = dataItem.FabricItem;
            var suggested = 'false', locked = 'false', spillover = 'false', released = 'false';

            if (colName == 'Suggested') {
                suggested = 'true';
            }
            else if (colName == 'Locked') {
                locked = 'true';
            }
            else if (colName == 'Released') {
                released = 'true';
            }

            var planFromDate = $("#FromWYear").val() != '' ? $("#FromWYear").val() : $("#planYear").val();
            var planToDate = $("#ToWYear").val() != '' ? $("#ToWYear").val() : $("#planYear").val();
            var search = {
                FromWYear: planFromDate,
                ToWYear: planToDate
            };

            search = JSON.stringify(search);

            var moreWks = 1;
            var endDate = '';
            ISS.common.executeActionAsynchronous("../Textiles/GetWeekDetails", search, function (stat, data) {
                if (stat && data) {
                   // moreWks = data.NoWeeks;
                    
                    var plant = '';
                    var endDate = data.EndDate;
                    if (endDate) endDate = ISS.common.parseDate(endDate);
                    endDate = textiles.getFormattedDate(endDate);
//cijith
                    var startDate = data.StartDate;
                    if (startDate) startDate = ISS.common.parseDate(startDate);
                    startDate = textiles.getFormattedDate(startDate);
                    moreWks = (data.ToWYear.substr(0, 2) - data.FromWYear.substr(0, 2)) + 1;

                    if (moreWks < 0)

                    {
                        moreWks += 52;
                    }

                    var url = location.protocol + '//' + location.hostname + textiles.const.urlGetWOM;

                    if (dataItem.Plant != null) {
                        plant = dataItem.Plant;
                    }
                    else {
                        var displayedData = $("#grdFabricDetails").data().kendoGrid.dataSource.view();
                        var suggData = null;
                        if (displayedData.length > 0) {
                            suggData = displayedData[rowIdx];
                            if (rowIdx < displayedData.length) {
                                if (suggData.Plant != null) {
                                    plant = suggData.Plant;
                                }
                                else {
                                    for (var k = rowIdx; k >= 0; k--) {
                                        if (displayedData[k].Plant != null) {
                                            plant = displayedData[k].Plant;
                                            break;
                                        }
                                    }

                                    if (plant == '') {
                                        plant = displayedData[0].Plant;
                                    }
                                }
                            
                            }
                        }

                    }
                   
                    
                    if (plant.length == 2) {
                        var queryString = $.param({
                            //MFGPlant: plant, Planner: $('#Planner').val(), Week: endDate, Fabric: fabric,
                            //  MFGPlant: dataItem.Plant.substr(0,2), Planner: $('#Planner').val(), Week: startDate, Fabric: fabric,
                            MFGPlant: plant, Planner: $('#Planner').val(), Week: startDate, Fabric: fabric,
                            TextileGroup: dataItem.Fabric.replace(" TOTAL",""),
                            BusinessUnit: $("#BusinessUnit").data('kendoMultiSelect').value().toString(), MoreWeeks: moreWks,
                            SuggestedLots: suggested, LockedLots: locked, SpillOver: "false",
                            ReleasedLots: "false", ReleasedLotsThisWeek: released,
                            autoLoad: true

                        });

                    }
                   else if (plant.length > 2) {
                        plant = dataItem.Plant.substr(0, 2);
                        if (plant == "FA") {
                            var queryString = $.param({
                                //MFGPlant: plant, Planner: $('#Planner').val(), Week: endDate, Fabric: fabric,
                                //  MFGPlant: dataItem.Plant.substr(0,2), Planner: $('#Planner').val(), Week: startDate, Fabric: fabric,
                                 Planner: $('#Planner').val(), Week: startDate, Fabric: fabric,
                                 TextileGroup: $("#TextileGroup").data('kendoMultiSelect').value().toString().replace(" TOTAL", ""),
                                BusinessUnit: $("#BusinessUnit").data('kendoMultiSelect').value().toString(), MoreWeeks: moreWks,
                                SuggestedLots: suggested, LockedLots: locked, SpillOver: "false",
                                ReleasedLots: "false", ReleasedLotsThisWeek: released,
                                autoLoad: true

                            });
                        }
                        else {
                            var queryString = $.param({
                                //MFGPlant: plant, Planner: $('#Planner').val(), Week: endDate, Fabric: fabric,
                                //  MFGPlant: dataItem.Plant.substr(0,2), Planner: $('#Planner').val(), Week: startDate, Fabric: fabric,
                                MFGPlant: plant, Planner: $('#Planner').val(), Week: startDate, Fabric: fabric,
                                TextileGroup: $("#TextileGroup").data('kendoMultiSelect').value().toString().replace(" TOTAL", ""),
                               BusinessUnit: $("#BusinessUnit").data('kendoMultiSelect').value().toString(), MoreWeeks: moreWks,
                                SuggestedLots: suggested, LockedLots: locked, SpillOver: "false",
                                ReleasedLots: "false", ReleasedLotsThisWeek: released,
                                autoLoad: true

                            });
                        }
                    }


                    if (queryString != null && queryString.length > 0) {
                        url += "?" + queryString;
                    }
                    window.open(url, "_blank");
                }
                else {
                    moreWks = 1;

                    var plant = '';


                    var url = location.protocol + '//' + location.hostname + textiles.const.urlGetWOM;

                    var arr = dataItem.Plant.split(' ');
                    if (arr.length > 0)
                        plant = arr[0];
                    var queryString = $.param({
                        MFGPlant: dataItem.Plant.substr(0, 2), Planner: $('#Planner').val(),
                        DyeBle: colName, TextileGroup: dataItem.Fabric.replace(" TOTAL", ""),
                        BusinessUnit: $("#BusinessUnit").data('kendoMultiSelect').value().toString(), MoreWeeks: moreWks,
                        SuggestedLots: suggested, LockedLots: locked, SpillOver: "false",
                        ReleasedLots: "false", ReleasedLotsThisWeek: released,
                        autoLoad: true

                    });


                    if (queryString != null && queryString.length > 0) {
                        url += "?" + queryString;
                    }
                    window.open(url, "_blank");
                }
            }, 'POST');


            return false
    },
    
    gotoWOMPrint: function (e) {

            if (e.currentTarget.className.indexOf("not-active") >= 0)
                return false;

            var grid = $("#grdPrintDetails").data("kendoGrid");

            var dataItem = grid.dataItem($(e.currentTarget).closest("tr"));

            var colIdx = $(e.target).closest('td').index();
            var rowIdx = $(e.target).closest('tr').index();

            //var colName = $('#grdYarnDetails').find('th').eq(colIdx).text();

            var pattern = /_/ig;
            var colName = $(e.currentTarget).data('print').replace(pattern, '');

            var print = dataItem.FabricItem;
            var suggested = 'false', locked = 'false', spillover = 'false', released = 'false';

            if (colName == 'Suggested') {
                suggested = 'true';
            }
            else if (colName == 'Locked') {
                locked = 'true';
            }
            else if (colName == 'Released') {
                released = 'true';
            }

            var planFromDate = $("#FromWYear").val() != '' ? $("#FromWYear").val() : $("#planYear").val();
            var planToDate = $("#ToWYear").val() != '' ? $("#ToWYear").val() : $("#planYear").val();
            var search = {
                FromWYear: planFromDate,
                ToWYear: planToDate
            };

            search = JSON.stringify(search);

            var moreWks = 1;
            var endDate = '';
            ISS.common.executeActionAsynchronous("../Textiles/GetWeekDetails", search, function (stat, data) {
                if (stat && data) {
                   // moreWks = data.NoWeeks;

                    var plant = '';
                    var endDate = data.EndDate;
                    if (endDate) endDate = ISS.common.parseDate(endDate);
                    endDate = textiles.getFormattedDate(endDate);
//cijith
                    var startDate = data.StartDate;
                    if (startDate) startDate = ISS.common.parseDate(startDate);
                    startDate = textiles.getFormattedDate(startDate);
                    moreWks = (data.ToWYear.substr(0, 2) - data.FromWYear.substr(0, 2)) + 1;

                    if (moreWks < 0) {
                        moreWks += 52;
                    }

                    var url = location.protocol + '//' + location.hostname + textiles.const.urlGetWOM;

                    if (dataItem.Plant != null) {
                        plant = dataItem.Plant;
                    }
                    else {
                        var displayedData = $("#grdPrintDetails").data().kendoGrid.dataSource.view();
                        var suggData = null;
                        if (displayedData.length > 0) {
                            suggData = displayedData[rowIdx];
                            if (rowIdx < displayedData.length) {
                                if (suggData.Plant != null) {
                                    plant = suggData.Plant;
                                }
                                else {
                                    for (var k = rowIdx; k >= 0; k--) {
                                        if (displayedData[k].Plant != null) {
                                            plant = displayedData[k].Plant;
                                            break;
                                        }
                                    }

                                    if (plant == '') {
                                        plant = displayedData[0].Plant;
                                    }
                                }
                            
                            }
                        }

                    }
                    if (plant.length == 2) {

                        var queryString = $.param({
                            MFGPlant: plant, Planner: $('#Planner').val(), Week: startDate, Fabric: print,
                            TextileGroup: dataItem.Fabric.replace(" TOTAL", ""),
                            BusinessUnit: $("#BusinessUnit").data('kendoMultiSelect').value().toString(), MoreWeeks: moreWks,
                            SuggestedLots: suggested, LockedLots: locked, SpillOver: "false",
                            ReleasedLots: "false", ReleasedLotsThisWeek: released,
                            autoLoad: true

                        });
                    }

                    else if (plant.length > 2) {
                        plant = dataItem.Plant.substr(0, 2);

                        if (plant == "PR") {
                            
                               
                                var queryString = $.param({
                                    Planner: $('#Planner').val(), Week: startDate, Fabric: print,
                                    TextileGroup: $("#TextileGroup").data('kendoMultiSelect').value().toString().replace(" TOTAL", ""),
                                    BusinessUnit: $("#BusinessUnit").data('kendoMultiSelect').value().toString(), MoreWeeks: moreWks,
                                    SuggestedLots: suggested, LockedLots: locked, SpillOver: "false",
                                    ReleasedLots: "false", ReleasedLotsThisWeek: released,
                                    autoLoad: true

                            });
                        }
                        else {
                            var queryString = $.param({
                                MFGPlant: plant, Planner: $('#Planner').val(), Week: startDate, Fabric: print,
                                TextileGroup: $("#TextileGroup").data('kendoMultiSelect').value().toString().replace(" TOTAL", ""),
                                BusinessUnit: $("#BusinessUnit").data('kendoMultiSelect').value().toString(), MoreWeeks: moreWks,
                                SuggestedLots: suggested, LockedLots: locked, SpillOver: "false",
                                ReleasedLots: "false", ReleasedLotsThisWeek: released,
                                autoLoad: true

                            });
                        }
                    }

                    if (queryString != null && queryString.length > 0) {
                        url += "?" + queryString;
                    }
                    window.open(url, "_blank");
                }
                else {
                    moreWks = 1;

                    var plant = '';


                    var url = location.protocol + '//' + location.hostname + textiles.const.urlGetWOM;

                    var arr = dataItem.Plant.split(' ');
                    if (arr.length > 0)
                        plant = arr[0];
                    var queryString = $.param({
                        MFGPlant: dataItem.Plant.substr(0, 2), Planner: $('#Planner').val(),
                        DyeBle: colName, TextileGroup: dataItem.print.replace(" TOTAL", ""),
                        BusinessUnit: $("#BusinessUnit").data('kendoMultiSelect').value().toString(), MoreWeeks: moreWks,
                        SuggestedLots: suggested, LockedLots: locked, SpillOver: "false",
                        ReleasedLots: "false", ReleasedLotsThisWeek: released,
                        autoLoad: true

                    });


                    if (queryString != null && queryString.length > 0) {
                        url += "?" + queryString;
                    }
                    window.open(url, "_blank");
                }
            }, 'POST');


            return false
        },

    //End

    getFormattedDate: function (date) {
        var year = date.getFullYear();
        var month = (1 + date.getMonth()).toString();
        month = month.length > 1 ? month : '0' + month;
        var day = date.getDate().toString();
        day = day.length > 1 ? day : '0' + day;
        return month + '/' + day + '/' + year;
    },
};
  String.prototype.replaceAll = function(search, replacement) {
        var target = this;
        return target.replace(new RegExp(search, 'g'), replacement);
    };
