
summary = {

    netDmdParam: {
        value: '',
        IsInit: true,
        rowNumber: 0
    },

    const: {
        netDmdPopup: null,
        Src: null,
    },

    addInputClass: function (frm) {
        $(frm + ' input:not(:checkbox):not(:button):not(:reset):not(:submit)').addClass('InputF');
    },

    docSummaryReady: function (IsLoad) {
       
        summary.addInputClass('#frmSummary'); 
        $('#readButton').bind('click', summary.loadSummaryGrid)
        $('.InputF').keypress(function (e) {
            if (e.which == 13) {
                summary.loadSummaryGrid();
                return false;
            }
        });
        $('input[type="reset"]').bind('click', function () {
            $('input:checkbox').uncheck();
            var grid = $("#summaryGrid").data("kendoGrid");
            ISS.common.clearGridFilters(grid);
            grid.dataSource.data([]);
            setTimeout(function () {
                var wcMultiSelect = $("#WorkCenter").data("kendoMultiSelect");
                wcMultiSelect.dataSource.read();
            }, 500);
        })
        ISS.common.menuEvent = function () {
           var grd= $("#summaryGrid").data("kendoGrid");
           if (!grd.dataSource._requestInProgress) grd.refresh()
        }
        //$('#exportButton').bind('click', summary.exportSummaryData);

        ISS.common.toUpperCase('.InputF:not(.excludeF)');

        $('#Color').bind('focusout', summary.refillAttr);
        $('#Style').bind('focusout', summary.refillAttr);

        //$('#exportButton').click(function(){
        //    summary.exportsummary();
        //});
    },

    searchDataSummary: function () {

        var plannr = $("#Planner").data("kendoMultiSelect");
        var id = plannr.value().toString();

        var attr = $("#Attribute").data("kendoMultiSelect");
        var allAttributes = attr.value().toString();

        var workCenter = $("#WorkCenter").data("kendoMultiSelect");
        var wc = workCenter.value().toString();

        var corpDiv = $("#CorpDiv").data("kendoMultiSelect");
        var corpid = corpDiv.value().toString();

        var SummaryView = {
            Planner: id,
            CapacityGroup: $("#CapacityGroup").val(),
            WorkCenter: wc,
            CorpDiv: corpid,
            RuleNo: $('#RuleNo').val(),
            RuleDesc: $('#RuleDesc').val(),
            Style: $('#Style').val(),
            Color: $('#Color').val(),
            Attribute: allAttributes,
            Size: $('#Size').val(),
            SortBy: '',
            PlanWeek: $('#PlanWeek').val(),
            DisplayAs: $("#displayAS").val(),
            ExcessSuggWOGrtr2: $('#ExcessSuggWOGrtr2').prop('checked'),
            ExcessSuggSpillover: $('#ExcessSuggSpillover').prop('checked'),
            SkuBreaks: $('#SkuBreaks').prop('checked'),
            SummarizeEventDmd: $('#SummarizeEventDmd').prop('checked'),
            SuggWO: $('#SuggWO').val(),
            //ReleasedLots: $('#ReleasedLots').prop('checked'),
            ReleasedLots: $('#ReleasedLots').prop('checked'),
            LockedOrders: $('#LockedOrders').prop('checked'),
            BuyOrders: $('#BuyOrders').prop('checked'),
            SuggWOWK1: $('#SuggWOWK1').prop('checked'),
            SuggWOWK2: $('#SuggWOWK2').prop('checked'),
            SuggWOWK2Grtr2: $('#SuggWOWK2Grtr2').prop('checked'),
            SpillOver: $('#SpillOver').prop('checked')
        };
        SummaryView.Src = summary.const.Src;
        return SummaryView;
    },

    loadSummaryGrid: function () {
        var grid = $("#summaryGrid").data("kendoGrid");
        var plannr = $("#Planner").data("kendoMultiSelect");
        var id = plannr.value().toString();

        var workCenter = $("#WorkCenter").data("kendoMultiSelect");
        var wc = workCenter.value().toString();

        var corpDiv = $("#CorpDiv").data("kendoMultiSelect");
        var corpid = corpDiv.value().toString();

        var attr = $("#Attribute").data("kendoMultiSelect");
        var allAttributes = attr.value().toString();


        var style = $('#Style').val();
        var color = $('#Color').val();
        var size = $('#Size').val();

        if (id == '' && wc == '' && style == '' && color == '' && allAttributes == '' && size == '' && corpid == '') {
            ISS.common.showPopUpMessage("Must select a Planner , Work Center, Corp div or Sku otherwise dataset is too large for system", ISS.common.MsgType.Warning);
            return false;
        }

        if (summary.netDmdParam.IsInit) {
            grid.table.on('click', '.lnkNetDmd', function (e) {
                summary.loadNetDemandGrd(e);
            });
            grid.lockedTable.on('click', '.lnkWOMStyle', function (e) {
                summary.gotoWOM(e);
            });
            grid.table.on('click', '.lnkSOBuyOrders', function (e) {
                summary.gotoSourcedOrder(e);
            });
            grid.table.on('click', '.lnkSuggWK1, .lnkSuggWK2, .lnkSugWK3Plus, .lnkSpillOver, .lnkReleasedOrder, .lnkLockedOrder', function (e) {
                summary.gotoWOM(e);
            });
            grid.table.on('click', '.lnkSuggComments', function (e) {
                summary.gotoSuggestedExceptions(e);
            });
            grid.lockedTable.on('click', '.lnkWOMAttribute', function (e) {
                summary.gotoWOM(e);
            })
            summary.netDmdParam.IsInit = false;
        }

        summary.const.Src = "A";
        if (grid.pager.page() > 1) grid.pager.page(1);
        grid.dataSource.read();
        summary.const.Src = null;
        ISS.common.collapsePanel('#panelbar-images', 0);
        ISS.common.expandPanel('#panelbar-images', 1);
        return false;

    },

    loadNetDemandGrd: function (e) {
        var grid = $("#summaryGrid").data("kendoGrid");
        var gridNetDmd = $("#netDmdGrid").data("kendoGrid");
        gridNetDmd.dataSource.data([])
        var dataItem = grid.dataItem($(e.currentTarget).closest("tr"));
        var postData = {
            Style: dataItem.selling_style_cd,
            Color: dataItem.selling_color_cd,
            Attribute: dataItem.selling_attribute_cd,
            Size: dataItem.selling_size_cd,
        };
        postData = JSON.stringify(postData);
        ISS.common.executeActionAsynchronous("../Shared/ValidateDemandPolicy", postData, function (stat, data) {
                if (data == "NET") {
                    ISS.common.showPopUpMessage('Net Demand Popup not available.', ISS.common.MsgType.Info);
                }
                else {
                    var netDemand = {
                        Style: dataItem.selling_style_cd,
                        Color: dataItem.selling_color_cd,
                        Attribute: dataItem.selling_attribute_cd,
                        Size: dataItem.selling_size_cd,
                        Size_Short: dataItem.size_short_desc,
                        Summaize_NetDmd: $('#SummarizeEventDmd').prop('checked')
                    }

                    var sku = '';
                    if (netDemand.Summaize_NetDmd) {
                        sku = "SUMMARIZED : " + netDemand.Style + '~' +
                        netDemand.Color + '~' + netDemand.Attribute + '~' + netDemand.Size_Short;
                    }
                    else {
                        sku = "DETAILED : " + netDemand.Style + '~' +
                        netDemand.Color + '~' + netDemand.Attribute + '~' + netDemand.Size_Short;
                    }

                    if (summary.const.netDmdPopup != null) {
                        summary.const.netDmdPopup.close();
                    }

                    //var sku = (netDemand.Summaize_NetDmd === true) ? "SUMMARIZED : " : "DETAILED : " + netDemand.Style + '~' +
                    //    netDemand.Color + '~' + netDemand.Attribute + '~' + netDemand.Size_Short;

                    gridNetDmd.dataSource.read({ netDemand: netDemand });
                    var settings = {
                        title: 'NET DEMAND : ' + sku,
                        animation: false,
                        width: '900px',
                    };
                    summary.const.netDmdPopup = ISS.common.popUpCustom('.divNetDemandPopup', settings);
                    //summary.const.netDmdPopup = ISS.common.popUp('.divNetDemandPopup', 'NET DEMAND ' + sku);
                    summary.const.netDmdPopup.title('NET DEMAND : ' + sku);
                }

            //}

        });
        

    },

    OnCapacityChange: function (e) {
        var dropdownlist = $("#SuggWO").data("kendoDropDownList");
        if ($("#CapacityGroup").val() === "Src") {
            dropdownlist.select(function (dataItem) {
                return dataItem.Text === "52";
            });
        }
        else {
            dropdownlist.select(function (dataItem) {
                return dataItem.Text === "3";
            });
        }

        var wcMultiSelect = $("#WorkCenter").data("kendoMultiSelect");
        wcMultiSelect.dataSource.read();
    },



    onClear: function () {
        var multiSelect = $('#Planner').data("kendoMultiSelect");
        multiSelect.value([]);

        $('#CapacityGroup').data('kendoDropDownList').value(0);

        $("input.txt").val('');

        $('input[type=checkbox]').attr('checked', false);

        var wcMultiSelect = $("#WorkCenter").data("kendoMultiSelect");
        wcMultiSelect.dataSource.read();

        //var grid = $("#summaryGrid").data("kendoGrid");
        //grid.dataSource.filter([]);
        //grid.dataSource.data([]);
        //grid.refresh();

        //grid.clearSelection();

        var grid = $("#summaryGrid").data("kendoGrid");
        ISS.common.clearGridFilters(grid);
        grid.dataSource.data([]);
        grid.refresh();
        //grid.dataSource.filter([]);



        //var required = $("#PlannerList").data("kendoMultiSelect");
        //var id = required.value();
        //$.get('../Order/PlannerAjaxAction/' + id, function (data) {
        //    $('#workCenterPlaceHolder').html(data);

        //});

        //$("#summaryGrid").data("kendoGrid").dataSource.data([]);
    },

    searchNetDemand: function () {

        return { sku: summary.netDmdParam.value };
    },

    loadNetDmdGrid: function () {
        var grid = $("#netDmdGrid").data("kendoGrid");
        if (grid.pager.page() > 1) grid.pager.page(1)
        grid.dataSource.read();
    },

    NetDmdClick: function (sku) {
        //alert(sku);
        summary.netDmdParam.value = sku;
        summary.loadNetDmdGrid();
        var modifiedSKU = sku.replace(",", "~");
        var settings = {
            title: 'NET DEMAND : ' + modifiedSKU,
            animation: false,
            width: '900px',
        };
        summary.const.netDmdPopup = ISS.common.popUpCustom('.divNetDemandPopup', settings);
        // ISS.common.popUp('.divNetDemandPopup', 'NET DEMAND: ' + modifiedSKU)


    },

    onCapacityGrpData: function () {
        var multiSelect = $('#WorkCenter').data("kendoMultiSelect");
        multiSelect.value([]);
        var plannr = $("#Planner").data("kendoMultiSelect");
        var id = plannr.value().toString();

        var SummaryView = {
            Planner: id, CapacityGroup: $("#CapacityGroup").val()
        }
        return SummaryView;
    },

    dataBound: function (e) {
        var grid = $("#summaryGrid").data("kendoGrid");
        var gridData = grid.dataSource.view();

        //var prevStyle = "", prevColor = "", pevAttribute = "", prevSize = "";
        //var nStyle = "", nColor = "", nAttribute = "", nSize = "";
        var SkuBreaks = $('#SkuBreaks').prop('checked');

        var datasrc = grid.dataSource.data();
        var len = datasrc.length;

        for (var i = 0; i < gridData.length; i++) {
            if (gridData[i].SuggestedLotsComments == "Excess to Demand") {
                grid.table.find("tr[data-uid='" + gridData[i].uid + "']").addClass("highlighted-row");
                $(".k-grid-content-locked").find("tr[data-uid='" + gridData[i].uid + "']").addClass("highlighted-row");
            }

            if (!(grid.dataSource._sort && grid.dataSource._sort.length > 0)) {
                if (SkuBreaks) {
                    if (gridData[i].SkuBreakRow == true) {
                        grid.table.find("tr[data-uid='" + gridData[i].uid + "']").addClass("skuBreak");
                        $(".k-grid-content-locked").find("tr[data-uid='" + gridData[i].uid + "']").addClass("skuBreak");
                    }
                }
            }
        }
        var tooltip = $(".toolLotSize").kendoTooltip({
            content: kendo.template($("#tooltemplate").html()),
            width: 120,
            position: "top"
        }).data("kendoTooltip")

        $('#FilteredColumns').html(ISS.common.getFilteredColumns("#summaryGrid"));
    },

    dataBoundNetDmd: function (e) {
        summary.netDmdParam.rowNumber = 0;

        var grid = $("#netDmdGrid").data("kendoGrid");
        var gridData = grid.dataSource.view();

        for (var i = 0; i < gridData.length; i++) {
            if (gridData[i].NET_Demand < 0) {
                grid.table.find("tr[data-uid='" + gridData[i].uid + "']").addClass("highlighted-row");
            }
        }
    },

    calculateNetDmdTotal: function () {
        var grid = $("#netDmdGrid").data("kendoGrid");
        var gridData = grid.dataSource.view();
        var total = 0;
        for (var i = 0; i < gridData.length; i++) {
            if (gridData[i].Consumed === "CONSUMED" || gridData[i].Consumed === "NET" || gridData[i].Consumed === "PARTIALLY CONSUMED") {
                total += gridData[i].qty;
            }
        }
        return total;
    },

    renderRecordNumber: function (data) {
        return ++summary.netDmdParam.rowNumber
    },

    SummaryInitialCheck: function () {
        var plannr = $("#Planner").data("kendoMultiSelect");
        var id = plannr.value().toString();

        var workCenter = $("#WorkCenter").data("kendoMultiSelect");
        var wc = workCenter.value().toString();

        var attr = $("#Attribute").data("kendoMultiSelect");
        var allAttributes = attr.value().toString();


        if (id == '' && wc == '' && $('#style').val() == undefined && $('#color').val() == undefined && allAttributes == '' && $('#size').val() == undefined) {
            ISS.common.showPopUpMessage("Must select a Planner , Workcenter or Sku otherwise dataset is too large for system", ISS.common.MsgType.Warning);
            return false;
        }
    },

    gotoWOM: function (e) {
        var grid = $("#summaryGrid").data("kendoGrid");

        var dataItem = grid.dataItem($(e.currentTarget).closest("tr"));

        var url = '';
        if (ISS.common.IsAttributed(dataItem.AttributionInd))
            url = location.protocol + '//' + location.hostname + summary.const.urlGetAWOM;
        else
            url = location.protocol + '//' + location.hostname + summary.const.urlGetWOM;

        var queryString = $.param({
            StyleType: "Selling Style", SStyle: dataItem.selling_style_cd,
            SColor: dataItem.selling_color_cd, SAttribute: dataItem.selling_attribute_cd,
            SSize: dataItem.size_short_desc,
            autoLoad: true

        });

        if (queryString != null && queryString.length > 0) {
            url += "?" + queryString;
        }

        if (e.currentTarget.className == 'linki2 lnkSuggWK1')
        {
            var addQueryString = $.param({
                DueDate: "Earliest Start", Week: "Plan Week One",
                MoreWeeks: "0", SuggestedLots: "true",
                LockedLots: "false", SpillOver: "false",
                ReleasedLots: "false", ReleasedLotsThisWeek: "false"
            });

            url += "&" + addQueryString;
        }
        else if (e.currentTarget.className == 'linki2 lnkSuggWK2') {
            var addQueryString = $.param({
                DueDate: "Earliest Start", Week: "Plan Week Two",
                MoreWeeks: "0", SuggestedLots: "true",
                LockedLots: "false", SpillOver: "false",
                ReleasedLots: "false", ReleasedLotsThisWeek: "false"
            });

            url += "&" + addQueryString;
        }
        else if (e.currentTarget.className == 'linki2 lnkSugWK3Plus') {
            var TimeFence = 0;
            if (dataItem.TimeFence != null)
                TimeFence = dataItem.TimeFence;
            var addQueryString = $.param({
                DueDate: "Earliest Start", Week: "Plan Week Three",
                MoreWeeks: TimeFence, SuggestedLots: "true",
                LockedLots: "false", SpillOver: "false",
                ReleasedLots: "false", ReleasedLotsThisWeek: "false"
            });

            url += "&" + addQueryString;
        }
        else if (e.currentTarget.className == 'linki2 lnkSpillOver') {
            var addQueryString = $.param({
                DueDate: "Earliest Start", Week: "Plan Week Three",
                MoreWeeks: "85", SuggestedLots: "false",
                LockedLots: "false", SpillOver: "true",
                ReleasedLots: "false", ReleasedLotsThisWeek: "false"
            });

            url += "&" + addQueryString;
        }
        else if (e.currentTarget.className == 'linki2 lnkLockedOrder') {
            var addQueryString = $.param({
                DueDate: "Earliest Start", Week: "Plan Week One",
                MoreWeeks: "85", SuggestedLots: "false",
                LockedLots: "true", SpillOver: "false",
                ReleasedLots: "false", ReleasedLotsThisWeek: "false"
            });

            url += "&" + addQueryString;
        }
        else if (e.currentTarget.className == 'linki2 lnkReleasedOrder') {
            var addQueryString = $.param({
                DueDate: "Earliest Start", Week: "Plan Week One",
                MoreWeeks: "85", SuggestedLots: "false",
                LockedLots: "false", SpillOver: "false",
                ReleasedLots: "false", ReleasedLotsThisWeek: "true"
            });

            url += "&" + addQueryString;
        }
        else if (e.currentTarget.className == 'linki2 lnkWOMStyle') {
            var addQueryString = $.param({
                DueDate: "Earliest Start", Week: "Current + Prior Week",
                MoreWeeks: "85", SuggestedLots: "true",
                LockedLots: "true", SpillOver: "true",
                ReleasedLots: "false", ReleasedLotsThisWeek: "true"
            });

            url += "&" + addQueryString;
        }

        window.open(url, "_blank");
        return false
    },
    gotoAWOM: function (e) {
        var grid = $("#summaryGrid").data("kendoGrid");

        var dataItem = grid.dataItem($(e.currentTarget).closest("tr"));

        var url = location.protocol + '//' + location.hostname + summary.const.urlGetAWOM;
        var queryString = $.param({
            StyleType: "Selling Style", SStyle: dataItem.selling_style_cd,
            SColor: dataItem.selling_color_cd, SAttribute: dataItem.selling_attribute_cd,
            SSize: dataItem.size_short_desc,
            autoLoad: true

        });

        if (queryString != null && queryString.length > 0) {
            url += "?" + queryString;
        }
        window.open(url, "_blank");
        return false;
    },

    gotoSourcedOrder: function (e) {
        var grid = $("#summaryGrid").data("kendoGrid");

        var dataItem = grid.dataItem($(e.currentTarget).closest("tr"));

        var url = location.protocol + '//' + location.hostname + summary.const.urlGetSO;
        var queryString = $.param({
            Style: dataItem.selling_style_cd,
            Color: dataItem.selling_color_cd, Attribute: dataItem.selling_attribute_cd,
            Size: dataItem.size_short_desc, SuggWO: $("#SuggWO").val(),
            autoLoad: true

        });

        
        if (queryString != null && queryString.length > 0) {
            url += "?" + queryString;
        }
        window.open(url, "_blank");
        return false
    },

    gotoSuggestedExceptions: function (e) {
        var grid = $("#summaryGrid").data("kendoGrid");

        var dataItem = grid.dataItem($(e.currentTarget).closest("tr"));

        var url = location.protocol + '//' + location.hostname + summary.const.urlGetExceptions;
        var queryString = $.param({
            Style: dataItem.selling_style_cd,
            Color: dataItem.selling_color_cd, Attribute: dataItem.selling_attribute_cd,
            autoLoad: true

        });


        if (queryString != null && queryString.length > 0) {
            url += "?" + queryString;
        }
        window.open(url, "_blank");
        return false
    },
    attributeData: function (e) {
    
        var style = $('#Style').val();
        var color = $('#Color').val();
       
        var passdata = {
            Style: style,
            Color: color
        }
        return passdata;

    },
    refillAttr:function () {
        var attribute = $("#Attribute").data("kendoMultiSelect"); 
        attribute.dataSource.read(); 
    },

    //exportsummary: function () {
    //    summaryfilter = summary.searchDataSummary();
    //    var grid = $("#summaryGrid").data("kendoGrid");
    //    var gridfilter = grid.dataSource.filter().filters;
    //    var parameterMap = grid.dataSource.transport.parameterMap;
    //    requestObject = JSON.stringify(parameterMap({ filter: grid.dataSource.filter() }));
    //    //parameterMap = grid.dataSource.transport.parameterMap;
    //    //var gridfilter = parameterMap({ sort: grid.dataSource.sort(), filter: grid.dataSource.filter(), group: grid.dataSource.group() });
    //    //JSON.stringify();
    //    $('#FiltersData').val(gridfilter);
    //    $('#request').val(requestObject);
    //},
   
};



