
information = {
    const: {
        
        searchFromDate: null,
        searchToDate: null
        
    },
    Releases: {
        Exception: '',
        IsInit: true,
        Src:null,
    },

    addInputClass: function (frm) {
        $(frm + ' input:not(:checkbox):not(:button):not(:reset):not(:submit)').addClass('InputF');
    },

    docReleasesReady: function (IsLoad) {
        information.addInputClass('#frmReleases');
        $('#frmReleases #btnReleasesSearch').bind('click', information.loadReleasesGrid)
        $('#frmReleases .InputF').keypress(function (e) {
            if (e.which == 13) {
                information.loadReleasesGrid();
                return false;

            }
        });
        ISS.common.toUpperCase('.InputF:not(.excludeF)');
        $('input[type="reset"]').bind('click', function () {
            var grid = $("#grdReleases").data("kendoGrid");
            ISS.common.clearGridFilters(grid);
            grid.dataSource.data([]);
            //grid.dataSource.filter([]);
        })

        ISS.common.menuEvent = function () {            
            var grd = $("#grdReleases").data("kendoGrid");
            if (!grd.dataSource._requestInProgress) grd.refresh()
        }

        $('#btnBlownAwayLotExport').bind('click', information.exportBlownAwayLotData);
    },

    searchDataReleases: function () {
        //debugger;
        //var datar = window.clipboardData.getData('Text');
        //alert(datar);
        //var dtarr = ClipboardEvent.clipboardData;
        //alert(dtarr);
        //var serialized = clipboardData.getData('text/plain')
        var tt = ISS.common.getFormData($('#frmReleases'));
        var multiselect = $("#WorkCenter").data("kendoMultiSelect");
        var items = multiselect.value();
        var Workcentes = "";
        for (var i = 0; i < items.length; i++) {
            if (i > 0) {
                Workcentes = Workcentes + ',' + "'" + items[i] + "'";
            }
            else {
                Workcentes = "'" + items[i] + "'";
            }
        }
        tt.WorkCenter = Workcentes;

        var multiselectPlanners = $("#Planner").data("kendoMultiSelect");
        var itemsP = multiselectPlanners.value();
        var PlannersM = "";
        for (var i = 0; i < itemsP.length; i++) {
            if (i > 0) {
                PlannersM = PlannersM + ',' + "'" + itemsP[i] + "'";
            }
            else {
                PlannersM = "'" + itemsP[i] + "'";
            }
        }
        tt.Planner = PlannersM;
        tt.Src=information.Releases.Src;
        return tt;
    },
    excelExport: function(e) {
        var sheet = e.workbook.sheets[0];
 
        for (var rowIndex = 1; rowIndex < sheet.rows.length; rowIndex++) {
            var row = sheet.rows[rowIndex];
            for (var cellIndex = 0; cellIndex < row.cells.length; cellIndex++) {
                if (cellIndex == 1) {
                    row.cells[cellIndex].format = "MM/dd/yyyy hh:mm:ss";
                }
            }
        }
    },
    loadReleasesGrid: function () {
        var grid = $("#grdReleases").data("kendoGrid");
        if (information.Releases.IsInit) {
            grid.table.on('click', '.lnkReason', function (e) {
                information.showExceptionDetailss(e);
                return false;
            });
            information.Releases.IsInit = false;
        }
        information.Releases.Src = "A";
        if (grid.pager.page() > 1) grid.pager.page(1);
        grid.dataSource.read();
        information.Releases.Src=null;

        ISS.common.collapsePanel('#panelbar-images', 0);
        ISS.common.expandPanel('#panelbar-images', 1);
        return false;
    },

    searchDataExceptionDetails: function () {
        var tt = information.Releases.Exception;
        tt.Src = information.Releases.Src;
        return tt;
    },

    showExceptionDetailss: function (e) {
        var grid = $("#grdReleases").data("kendoGrid");
        var gridPopUp = $("#grdExceptionDetails").data("kendoGrid");
        var dataItem = grid.dataItem($(e.currentTarget).closest("tr"));
        information.Releases.Exception = { SuperOrder: dataItem.SuperOrder };
        ISS.common.clearGridFilters(gridPopUp);
        information.Releases.Src = "A";
        gridPopUp.dataSource.read();
        information.Releases.Src = null;
        ISS.common.popUp('.divExceptionDetails', 'ISS AS400 Exceptions')
    },



    //=======================================================================================
    //============================== Suggested Exceptions ===================================
    //=======================================================================================

    docSEReady: function (IsLoad) {
        information.addInputClass('#frmSE');
        $('#frmSE #btnSESearch').bind('click', information.loadSEGrid);
        $('#frmSE .InputF').keypress(function (e) {
            if (e.which == 13) {
                information.loadSEGrid();
                return false;

            }
        });
        ISS.common.toUpperCase('.InputF:not(.excludeF)');
        $('input[type="reset"]').bind('click', function () {
            var grid = $("#grdSE").data("kendoGrid");
            ISS.common.clearGridFilters(grid);
            grid.dataSource.data([]);
            //grid.dataSource.filter([]);
        })
    },

    searchDataSE: function () {
        var tt = ISS.common.getFormData($('#frmSE'));
        tt.Src = information.Releases.Src;
        return tt;
    },

    loadSEGrid: function () {
        var grid = $("#grdSE").data("kendoGrid");
        if (information.Releases.IsInit) {
            grid.table.on('click', '.lnkExc', function (e) {
                information.showSEExceptionDetails(e);
            });
            information.Releases.IsInit = false;
        }

        information.Releases.Src = "A";
        if (grid.pager.page() > 1) grid.pager.page(1);
        grid.dataSource.read();
        information.Releases.Src = null;
        ISS.common.collapsePanel('#panelbar-images', 0);
        ISS.common.expandPanel('#panelbar-images', 1);
        return false;

    },

    showSEExceptionDetails: function (e) {
        var grid = $("#grdSE").data("kendoGrid");
        var dataItem = grid.dataItem($(e.currentTarget).closest("tr"));
        var postData = {
            Style: dataItem.Style, Color: dataItem.Color,
            Atribute: dataItem.Atribute, SizeShortDesc: dataItem.SizeShortDesc, OrderSize: dataItem.OrderSize, MfgPath: dataItem.MfgPath, DmdLoc: dataItem.DmdLoc, ConflictSKU: dataItem.ConflictSKU
        };
        ISS.common.executeActionAsynchronous('SuggestedExceptionDetail', JSON.stringify(postData), function (data, msg) {
            //msg = 'k-webkit k-webkit41 js flexbox flexboxlegacy canvas canvastext ';
            if (data)
                ISS.common.showPopUpMessage(msg, ISS.common.MsgType.Info);
        });

    },
    SetViewButtongridDataBound: function (e) {

        $(e.sender.content).find('.viewic1').removeClass('k-button').attr('href', 'javascript:void(0)');
    },

    //=======================================================================================
    //============================== DC Work Orders ===================================
    //=======================================================================================


    docDCWorkOrdersReady: function (IsLoad) {
        information.addInputClass('#frmDCWorkOrders');
        $('#frmDCWorkOrders #btnDCWorkOrdersSearch').bind('click', information.loadDCWorkOrdersGrid);
        $('#frmDCWorkOrders .InputF').keypress(function (e) {
            if (e.which == 13) {
                information.loadDCWorkOrdersGrid();
                return false;

            }
        });
        ISS.common.toUpperCase('.InputF:not(.excludeF)');
        $('input[type="reset"]').bind('click', function () {
            var grid = $("#grdDCWorkOrders").data("kendoGrid");
            ISS.common.clearGridFilters(grid);
            grid.dataSource.data([]);
            setTimeout(function () {
                $('#hasRemarks').check();
            }, 200);
        })
    },

    searchDataDCWorkOrders: function () {
        var ret = ISS.common.getFormData($('#frmDCWorkOrders'));
        ret.hasRemarks = $('#hasRemarks').prop('checked');
        ret.Src = information.Releases.Src;
        
        return ret;
    },

    loadDCWorkOrdersGrid: function () {
        var grid = $("#grdDCWorkOrders").data("kendoGrid");
        information.Releases.Src = "A";
        if (grid.pager.page() > 1) grid.pager.page(1);
        grid.dataSource.read();
        information.Releases.Src = null;
        ISS.common.collapsePanel('#panelbar-images', 0);
        ISS.common.expandPanel('#panelbar-images', 1);
        return false;

    },

    //=======================================================================================
    //============================== Style Exception ===================================
    //=======================================================================================

    docStyleExceptionsReady: function (IsLoad) {
        information.addInputClass('#frmStyleExceptions');
        $('#frmStyleExceptions #btnStyleExceptionsSearch').bind('click', information.loadStyleExceptionsGrid);
        $('#frmStyleExceptions .InputF').keypress(function (e) {
            if (e.which == 13) {
                information.loadStyleExceptionsGrid();
                return false;
            }
        });
        ISS.common.toUpperCase('.InputF:not(.excludeF)');
        $('input[type="reset"]').bind('click', function () {
            var grid = $("#grdStyleExceptions").data("kendoGrid");
            ISS.common.clearGridFilters(grid);
            grid.dataSource.data([]);
            setTimeout(function () {
                $('#APS').check();
                $('#AVYX').check();
                $('#ISS').check();
                $('#NET').check();
                $('#CWC').check();
            }, 200);
            //grid.dataSource.filter([]);
        })
    },

    searchStyleExceptions: function () {
        var postData = ISS.common.getFormData($('#frmStyleExceptions'));
        postData.APS = $('#APS').prop('checked');
        postData.AVYX = $('#AVYX').prop('checked');
        postData.ISS = $('#ISS').prop('checked');
        postData.NET = $('#NET').prop('checked');
        postData.CWC = $('#CWC').prop('checked');
        postData.MTLA = $('#MTLA').prop('checked');
        postData.Src = information.Releases.Src;
        return postData;
    },

    loadStyleExceptionsGrid: function () {
        if (!($('#APS').prop('checked') || $('#AVYX').prop('checked') || $('#ISS').prop('checked') || $('#NET').prop('checked') || $('#CWC').prop('checked') || $('#MTLA').prop('checked') )) {
            ISS.common.showPopUpMessage('Please select at least one exception: APS,	AVYX, ISS, NET, CWC,MATL-A');
            return false;
        }
        var grid = $("#grdStyleExceptions").data("kendoGrid");
        information.Releases.Src = "A";
        if (grid.pager.page() > 1) grid.pager.page(1);
        grid.dataSource.read();
        information.Releases.Src = null;
        ISS.common.collapsePanel('#panelbar-images', 0);
        ISS.common.expandPanel('#panelbar-images', 1);
        return false;

    },

    //=======================================================================================
    //============================== W/O Textile Group ===================================
    //=======================================================================================

    docWOTextileGroupReady: function (IsLoad) {
        information.addInputClass('#frmWOTextileGroup');
        $('#frmWOTextileGroup #btnWOTextileGroupSearch').bind('click', information.loadWOTextileGroupGrid);
        $('#frmWOTextileGroup .InputF').keypress(function (e) {
            if (e.which == 13) {
                information.loadWOTextileGroupGrid();
                return false;

            }
        });
        ISS.common.toUpperCase('.InputF:not(.excludeF)');
        $('input[type="reset"]').bind('click', function () {
            var grid = $("#grdWOTextileGroup").data("kendoGrid");
            ISS.common.clearGridFilters(grid);
            grid.dataSource.data([]);
            //grid.dataSource.filter([]);
        })
    },

    searchDataWOTextileGroup: function () {
        var tt = ISS.common.getFormData($('#frmWOTextileGroup'));
        tt.Src = information.Releases.Src;
        return tt;
    },

    loadWOTextileGroupGrid: function () {
        var grid = $("#grdWOTextileGroup").data("kendoGrid");
        information.Releases.Src = "A";
        if (grid.pager.page() > 1) grid.pager.page(1);
        grid.dataSource.read();
        information.Releases.Src = null;
        ISS.common.collapsePanel('#panelbar-images', 0);
        ISS.common.expandPanel('#panelbar-images', 1);
        return false;

    },

    //=======================================================================================
    //============================== Blown away Lot ===================================
    //=======================================================================================

    docBlownAwayLotReady: function (IsLoad) {
        information.addInputClass('#frmBlownAwayLot');
        $('#frmBlownAwayLot #btnBlownAwayLotSearch').bind('click', information.loadBlownAwayLotGrid);
        $('#frmBlownAwayLot .InputF').keypress(function (e) {
            if (e.which == 13) {
                information.loadBlownAwayLotGrid();
                return false;

            }
        });
        ISS.common.toUpperCase('.InputF:not(.excludeF)');
        $('input[type="reset"]').bind('click', function () {
            var grid = $("#grdBlownAwayLot").data("kendoGrid");
            ISS.common.clearGridFilters(grid);
            grid.dataSource.data([]);
            // $("#grdBlownAwayLot").data("kendoGrid").dataSource.data([]);
            // $("#grdBlownAwayLot").data("kendoGrid").dataSource.filter([]);             
            //grid.dataSource.data([]);
        })
       
    },

    searchDataBlownAwayLot: function () {
        var tt = ISS.common.getFormData($('#frmBlownAwayLot'));
        tt.Src = information.Releases.Src;
        return tt;
    },

    loadBlownAwayLotGrid: function () {
        var grid = $("#grdBlownAwayLot").data("kendoGrid");
        information.Releases.Src = "A";
        if (grid.pager.page() > 1) grid.pager.page(1);
        grid.dataSource.read();
        information.Releases.Src = null;
        ISS.common.collapsePanel('#panelbar-images', 0);
        ISS.common.expandPanel('#panelbar-images', 1);
        return false;

    },

    gridDataBoundStyle: function (e) {
        $('#FilteredColumns').html(ISS.common.getFilteredColumns("#" + e.sender.content.context.id));
    },

    //=======================================================================================
    //============================== Bulks to AVYX ===================================
    //=======================================================================================

    docBulkstoAVYXReady: function (IsLoad) {
        information.addInputClass('#frmBulkstoAVYX');
        $('#frmBulkstoAVYX #btnBulktoAvyxSearch').bind('click', information.loadCountAvyxGrid);
        ISS.common.toUpperCase('.InputF:not(.excludeF)');
        $('input[type="reset"]').bind('click', function () {
            var grid = $("#grdBulkstoAVYX").data("kendoGrid");
            ISS.common.clearGridFilters(grid);
            grid.dataSource.data([]);
            
        })
        information.loadCountAvyxGrid();
        $("#grdBulksComplte").hide();
        $("#grdBulksError").hide();
    },
    DataBulkstoAVYX: function () {
        var tt = ISS.common.getFormData($('#frmBulkstoAVYX'));
        return tt;
    },
    loadBulkstoAVYXGrid: function () {
        $("#hdExtractType").val("Active");
        $("#grdBulkstoAVYX").show();
        $("#grdBulksComplte").hide();
        $("#grdBulksError").hide();
        var grid = $("#grdBulkstoAVYX").data("kendoGrid");
        if (grid.pager.page() > 1) grid.pager.page(1);
        //var data = information.DataBulkstoAVYX()
        //data.param = "A";
        //information.Releases.Src = "A";
        grid.dataSource.read();
        //information.Releases.Src = null;
        ISS.common.collapsePanel('#panelbar-images', 0);
        ISS.common.expandPanel('#panelbar-images', 1);
        return false;

    },
    loadBulkstoComplteGrid: function () {
        $("#hdExtractType").val("Completed");
        $("#grdBulksComplte").show();
        $("#grdBulkstoAVYX").hide();
        $("#grdBulksError").hide();
        var grid = $("#grdBulksComplte").data("kendoGrid");
        if (grid.pager.page() > 1) grid.pager.page(1);
        grid.dataSource.read();
        ISS.common.collapsePanel('#panelbar-images', 0);
        ISS.common.expandPanel('#panelbar-images', 1);
        return false;

    },
    loadBulksErrorGrid: function () {
        $("#hdExtractType").val("Error");
        $("#grdBulksError").show();
        $("#grdBulkstoAVYX").hide();
        $("#grdBulksComplte").hide();
        var grid = $("#grdBulksError").data("kendoGrid");
        if (grid.pager.page() > 1) grid.pager.page(1);
        grid.dataSource.read();
        ISS.common.collapsePanel('#panelbar-images', 0);
        ISS.common.expandPanel('#panelbar-images', 1);
        return false;

    },
    loadCountAvyxGrid: function () {
       
        var grid1 = $("#grdBulkstoAVYX").data("kendoGrid");
        if (grid1 != undefined) {
            ISS.common.clearGridFilters(grid1);
            grid1.dataSource.data([]);
        }
        var grid2 = $("#grdBulksComplte").data("kendoGrid");
        if (grid2 != undefined) {
            ISS.common.clearGridFilters(grid2);
            grid2.dataSource.data([]);
        }
        var grid3 = $("#grdBulksError").data("kendoGrid");
        if (grid3 != undefined) {
            ISS.common.clearGridFilters(grid3);
            grid3.dataSource.data([]);
        }
        ISS.common.blockUI(true);
        var postData = { FromDate: $("#frmBulkstoAVYX #FromDate").val(), ToDate: $("#frmBulkstoAVYX #ToDate").val() };
        ISS.common.executeActionAsynchronous("../Information/GetBulksActiveCount", JSON.stringify(postData), function (stat, data) {
            if (stat && data) {
                $('#lblBActive').text(data.bulk);
                $('#lblBComplete').text(data.complete);
                $('#lblBError').text(data.error);
            }
            ISS.common.blockUI(false);

        }, 'POST');
    },

    //=======================================================================================
    //============================== Bulks to OneSource ===================================
    //=======================================================================================

    docBulkstoOneSourceReady: function (IsLoad) {
        information.addInputClass('#frmBulkstoOneSource');
        $('#frmBulkstoOneSource #btnBulktoOnesourceSearch').bind('click', information.loadCountGrid);
        ISS.common.toUpperCase('.InputF:not(.excludeF)');
        $('input[type="reset"]').bind('click', function () {
            var grid = $("#grdBulksPulled").data("kendoGrid");
            ISS.common.clearGridFilters(grid);
            grid.dataSource.data([]);

        })
        information.loadCountGrid();
        $("#grdBulksSuccessful").hide();
        $("#grdBulksinError").hide();
        $("#grdBulksinErrorSecond").hide();

    },

    gridDataBoundStyle1: function (e) {
        $('#FilteredColumns').html(ISS.common.getFilteredColumns("#" +e.sender.content.context.id));
        },
   
    DataBulkstoOneSource: function () {
        var ret = ISS.common.getFormData($('#frmBulkstoOneSource'));
        return ret;
     },
                           
    loadBulksPulledGrid: function () {
        $("#hdExtractType").val("Pulled");
        
        $("#grdBulksPulled").show();
        $("#grdBulksSuccessful").hide();
        $("#grdBulksinError").hide();
        $("#grdBulksinErrorSecond").hide();
        var grid = $("#grdBulksPulled").data("kendoGrid");
        if (grid.pager.page() > 1) grid.pager.page(1);
        //var data = information.DataBulkstoAVYX()
        //data.param = "A";
        //information.Releases.Src = "A";
        grid.dataSource.read();
        //information.Releases.Src = null;
        ISS.common.collapsePanel('#panelbar-images', 0);
        ISS.common.expandPanel('#panelbar-images', 1);
        return false;

    },
    loadBulksSuccessfulGrid: function () {
        $("#hdExtractType").val("Success");
        $("#grdBulksSuccessful").show();
        $("#grdBulksPulled").hide();
        $("#grdBulksinError").hide();
        $("#grdBulksinErrorSecond").hide();
        var grid = $("#grdBulksSuccessful").data("kendoGrid");
        if (grid.pager.page() > 1) grid.pager.page(1);
        grid.dataSource.read();
        ISS.common.collapsePanel('#panelbar-images', 0);
        ISS.common.expandPanel('#panelbar-images', 1);
        return false;

    },
    loadAllBulksinErrorGrid: function () {
        $("#hdExtractType").val("ErrorOS");
        $("#grdBulksinError").show();
        $("#grdBulksPulled").hide();
        $("#grdBulksSuccessful").hide();
        $("#grdBulksinErrorSecond").hide();
        var grid = $("#grdBulksinError").data("kendoGrid");
        if (grid.pager.page() > 1) grid.pager.page(1);
        grid.dataSource.read();
        ISS.common.collapsePanel('#panelbar-images', 0);
        ISS.common.expandPanel('#panelbar-images', 1);
        return false;

    },
    loadAllBulksinErrorSecondGrid: function () {
        $("#hdExtractType").val("ErrorOSSecond");
        $("#grdBulksinErrorSecond").show();
        $("#grdBulksinError").hide();
        $("#grdBulksPulled").hide();
        $("#grdBulksSuccessful").hide();
        var grid = $("#grdBulksinErrorSecond").data("kendoGrid");
        if (grid.pager.page() > 1) grid.pager.page(1);
        grid.dataSource.read();
        ISS.common.collapsePanel('#panelbar-images', 0);
        ISS.common.expandPanel('#panelbar-images', 1);
        return false;

    },
    loadCountGrid: function () {
        var grid1 = $("#grdBulksPulled").data("kendoGrid");
        if (grid1 != undefined) {
            ISS.common.clearGridFilters(grid1);
            grid1.dataSource.data([]);
        }
        var grid2 = $("#grdBulksSuccessful").data("kendoGrid");
        if (grid2 != undefined) {
            ISS.common.clearGridFilters(grid2);
            grid2.dataSource.data([]);
        }
        var grid3 = $("#grdBulksinError").data("kendoGrid");
        if (grid3 != undefined) {
            ISS.common.clearGridFilters(grid3);
            grid3.dataSource.data([]);
        }
        var grid4 = $("#grdBulksinErrorSecond").data("kendoGrid");
        if (grid4 != undefined) {
            ISS.common.clearGridFilters(grid4);
            grid4.dataSource.data([]);
        }
        ISS.common.blockUI(true);
        var postData = { FromDate: $("#frmBulkstoOneSource #FromDate").val(), ToDate: $("#frmBulkstoOneSource #ToDate").val() };
        ISS.common.executeActionAsynchronous("../Information/GetBulksOneSourceCount", JSON.stringify(postData), function (stat, data) {
            if (stat && data) {
                $('#lblBPull').text(data.pull);
                $('#lblBSuccess').text(data.success);
                $('#lblBError').text(data.error);
                $('#lblBErrorSec').text(data.errorSec);
            }
            ISS.common.blockUI(false);

        }, 'POST');
    },
    

    //=======================================================================================
    //============================== Knights Apparel Expedite ===================================
    //=======================================================================================

    docKnightsApparelExpediteReady: function (IsLoad) {
        information.addInputClass('#frmKnightsApparelExpedite');
        $('#frmKnightsApparelExpedite #btnKnightsApparelExpediteSearch').bind('click', information.loadKnightsApparelExpedite);
        ISS.common.toUpperCase('.InputF:not(.excludeF)');
        $('input[type="reset"]').bind('click', function () {
            var grid = $("#grdKnightsApparelExpedite").data("kendoGrid");
            ISS.common.clearGridFilters(grid);
            grid.dataSource.data([]);

        })
        information.loadCountGrid();
        //$("#grdBulksSuccessful").hide();
        //$("#grdBulksinError").hide();
        //$("#grdBulksinErrorSecond").hide();

    },

    
    DataKnightsApparelExpedite: function () {
        var ret = ISS.common.getFormData($('#frmKnightsApparelExpedite'));
        return ret;
    },

    loadKnightsApparelExpedite: function () {
        //$("#hdExtractType").val("Pulled");

        //$("#grdBulksPulled").show();
        //$("#grdBulksSuccessful").hide();
        //$("#grdBulksinError").hide();
        //$("#grdBulksinErrorSecond").hide();
        var grid = $("#grdKnightsApparelExpedite").data("kendoGrid");
        if (grid.pager.page() > 1) grid.pager.page(1);
        //var data = information.DataBulkstoAVYX()
        //data.param = "A";
        //information.Releases.Src = "A";
        grid.dataSource.read();
        //information.Releases.Src = null;
        ISS.common.collapsePanel('#panelbar-images', 0);
        ISS.common.expandPanel('#panelbar-images', 1);
        return false;

    },
    
    
   
    
};



