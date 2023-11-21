MSKUWO = {

    CreateOrder: {
        IsInit: true

    },

    addInputClass: function (frm) {
        $(frm + ' input:not(:checkbox):not(:button):not(:reset):not(:submit)').addClass('InputF');
    },

    doCreateMSKUWOReady: function (IsLoad) {
        MSKUWO.addInputClass('#frmWO');
        //$('#frmWO #btnWOSearch').bind('click', WO.loadWOGrid)
        //$('#frmWO .InputF').keypress(function (e) {
        //    if (e.which == 13) {
        //        WO.loadWOGrid();
        //    }
        //});
        ISS.common.toUpperCase('.InputF:not(.excludeF)');
        //$('input[type="reset"]').bind('click', function () {
        //    var grid = $("#grdWO").data("kendoGrid");
        //    grid.dataSource.data([]);        
        //})
    },

    searchDataWO: function () {
        return ISS.common.getFormData($('#frmWO'));
    },

    loadWOGrid: function () {
        var grid = $("#grdWO").data("kendoGrid");
        if (WO.WO.IsInit) {
            grid.table.on('click', '.lnkReason', function (e) {
                WO.showExceptionDetailss(e);
            });
            MSKUWO.CreateOrder.IsInit = false;
        }
        if (grid.pager.page() > 1) grid.pager.page(1)
        grid.dataSource.read();

        ISS.common.collapsePanel('#panelbar-images', 0);
        return false;
    },

    searchDataExceptionDetails: function () {
        return MSKUWO.CreateOrder.Exception;
    },

    showExceptionDetailss: function (e) {
        var grid = $("#grdWO").data("kendoGrid");
        var gride = $("#grdExceptionDetails").data("kendoGrid");
        var dataItem = grid.dataItem($(e.currentTarget).closest("tr"));
        MSKUWO.CreateOrder.Exception = { SuperOrder: dataItem.SuperOrder };
        if (grid.dataSource._filter != null && grid.dataSource._filter.filters.length > 0)
            grid.dataSource.filter([]);
        gride.dataSource.read()
        ISS.common.popUp('.divExceptionDetails', 'ISS AS400 Exceptions')
    }

}