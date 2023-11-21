
amrz = {
    const: {

        searchFromDate: null,
        searchToDate: null

    },
    Releases: {
        Exception: '',
        IsInit: true,
        Src: null,
    },

    addInputClass: function (frm) {
        $(frm + ' input:not(:checkbox):not(:button):not(:reset):not(:submit)').addClass('InputF');
    },

   


    docAttriMrzReady: function (IsLoad) {
        amrz.addInputClass('#frmAttributeMrz');
        $('#frmAttributeMrz #btnAttributeMrzSearch').bind('click', amrz.loadAttriMrzGrid);
        $('#frmAttributeMrz .InputF').keypress(function (e) {
            if (e.which == 13) {
                amrz.loadAttriMrzGrid();
                return false;

            }
        });
        ISS.common.toUpperCase('.InputF:not(.excludeF)');
        $('input[type="reset"]').bind('click', function () {
            var grid = $("#grdAttriMrz").data("kendoGrid");
            ISS.common.clearGridFilters(grid);
            grid.dataSource.data([]);
            setTimeout(function () {
            }, 200);
        })
        $('.k-grid-btndeleteAttributeMrz').bind('click', amrz.deleteAttriMrz);
     
        $('#grdAttriMrz #SelectAll').bind('change', function () {
            amrz.SelectAllDetails($(this).prop('checked'));
        });
        $("#grdAttriMrz").on('change', '.chckbx', function () {
            setTimeout(function () {
                var gridDetail = $("#grdAttriMrz").data("kendoGrid");
                $("#SelectAll").prop('checked', $('.chckbx:checked').length == gridDetail.dataSource.view().length);
            });
        });
    },
    SelectAllDetails: function (isSelected) {

        var grid = $('#grdAttriMrz').data('kendoGrid');
        grid.table.find('.chckbx').prop('checked', isSelected);
    },
    deleteAttriMrz: function (e) {
        var checkedItems = $('.chckbx:checked');
        if (checkedItems.length > 0) {
            ISS.common.showConfirmMessage('Do you want to delete?', null, function (reply) {
                if (reply) {
                    var selectedItem = new Array();
                    var grid = $("#grdAttriMrz").data("kendoGrid");
                    $(checkedItems).each(function (idx, item) {
                        var row = $(item).closest("tr");
                        var selectedData= grid.dataItem(row);
                        selectedItem.push(selectedData);
                        
                    });
                    ISS.common.executeActionAsynchronous('DeleteAttriMrz', JSON.stringify({ data: selectedItem }), function (stat) {
                        //if (stat) {
                        if (stat) {
                            var tp = ISS.common.MsgType.Success;
                            var SMsg = selectedItem.length + ' record(s) deleted successfully.'
                            ISS.common.showPopUpMessage(SMsg, tp, function (ret) {
                                amrz.loadAttriMrzGrid()
                            });
                        }
                        else {
                            ISS.common.notify.error('Failed to delete ' + selectedItem.length + ' record(s).');
                        }
                    });
                    //});
                }
            });
            
        }
        else {
            ISS.common.showPopUpMessage('Please select at least one record.');
        }
        return false;
    },
    searchAttributeMrzOrders: function () {
        var ret = ISS.common.getFormData($('#frmAttributeMrz'));
        ret.Src = amrz.Releases.Src;
        return ret;
    },

    loadAttriMrzGrid: function () {
        var grid = $("#grdAttriMrz").data("kendoGrid");
        amrz.Releases.Src = "A";
        if (grid.pager.page() > 1) grid.pager.page(1);
        grid.dataSource.read();
        amrz.Releases.Src = null;
        ISS.common.collapsePanel('#panelbar-images', 0);
        ISS.common.expandPanel('#panelbar-images', 1);
        return false;

    },
    gridAttriMrzDataBound: function () {
        $(".k-grid-DeleteItem").find("span").addClass("k-icon k-delete");
        $('#SelectAll').uncheck();
    },

};



