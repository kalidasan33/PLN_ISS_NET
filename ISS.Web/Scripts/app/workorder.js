
WO = {

    CreateOrder: {
        IsInit: true,
        currentRow: null,
        urlCalculate: '',
        rev: null,
        mfgPath: null,
        PKCode: null,
        CatCode: null,
        validator: null,
        popupValidator: null,
        editDataCum: null,
        editDataFab: null,
        isLoadingCompleted: true,
        isSave: false,
        isUpdateCumOnChange: true,
        plantMachineList: null,
        editPopUp: null,
        mfgPathVal: null,
        AltIdVal: null,
        StyleVal: null,
        CutPath: null,
        Week: null,
        Year: null,
        Date: null,
        TotalDz: null,
        currentTable: null,
    },

    addInputClass: function (frm) {
        $(frm + ' input:not(:checkbox):not(:button):not(:reset):not(:submit)').addClass('InputF');
    },

    doCreateWOReady: function (IsLoad) {
        WO.addInputClass('#frmWoEditor');
        ISS.common.toUpperCase('.InputF:not(.excludeF)');
        WO.CreateOrder.validator = $('#frmWO').kendoValidator().data("kendoValidator");
        WO.CreateOrder.popupValidator = $('#frmWoEditor').kendoValidator().data("kendoValidator");

        $('#SellingStyle').bind('focusout', WO.StyleChanged);
        $('#SellingStyle').bind('propertychange change keyup paste input', WO.multiSizeGridClear);
        $('#Revision').bind('focus', WO.revFocus);
        $('#Revision').bind('click', WO.revSearchClick);
        $('#MfgPathId').bind('click', WO.mfPathClick);
        $('#PackCode').bind('click', WO.PackCodeClick);
        $('#CategoryCode').bind('click', WO.CatCodeClick);
        $('#PKGStyle').bind('change', WO.PKGChanged);
        $('#btnWorkOrderClear').bind('click', WO.ClearOrders);
        $('#btnWorkOrderReCalc').bind('click', WO.ReCalcOrders);
        $('#Limit').bind('focusout', WO.LimitChanged);
        $('#PlannedWeek').bind('change', WO.changeYear);
        $('#DueDate').bind('change', WO.changeYear);
        $('#PlannedYear').bind('change', WO.changeYear);
        $('#grdwrkOrderFabric').on('change', '.chkPFSFab', WO.onChangePFS);
        $('#MfgPathId').bind('focusout', WO.loadMFGPathDtls);
        //$('#MfgPathId').bind('focusin', WO.loadMFGPathDtls);
        $("#frmWoEditor #CategoryCode").bind('focusout', WO.validateCategoryCodeCWO);

        $("#frmWoEditor #TotalDozens").bind('focusout', WO.updateLBS);

        $("#frmWoEditor #CategoryCode").focusin(function () {
            WO.CreateOrder.CategoryCode = $('#frmWoEditor #CategoryCode').val();
        });

        $("#frmWoEditor #TotalDozens").focusin(function () {
            WO.CreateOrder.TotalDz = $('#frmWoEditor #TotalDozens').val();
        });

        $("#AlternateId").focusin(function () {
            WO.CreateOrder.AltIdVal = $('#frmWoEditor #AlternateId').val();
        });
        $("#MfgPathId").focusin(function () {
            WO.CreateOrder.mfgPathVal = $('#frmWoEditor #MfgPathId').val();
        });
        $("#SellingStyle").focusin(function () {
            WO.CreateOrder.StyleVal = $('#frmWoEditor #SellingStyle').val();
        });
        $("#CutPath").focusin(function () {
            WO.CreateOrder.CutPath = $('#frmWoEditor #CutPath').val();
        });
        $('#MfgPathId').on('keypress', function (e) {
            if (e.keyCode == 13) {
                WO.mfPathClick();
                return false;
            }
        });
        $('#Revision').on('keypress', function (e) {
            if (e.keyCode == 13) {
                WO.revSearchClick();
                return false;
            }

        });
        $('#Revision').on('focusin', function () {
            WO.CreateOrder.Revision = this.value;
        });
        $('#Revision').on('focusout', function () {
            if (WO.CreateOrder.Revision != this.value) {
                WO.getPKGStyle();
                return false;
            }
            var result = WO.ValidateHAA();
            WO.HideField(result);
        });
        $('#MfgPathId').on('focusin', function () {
            WO.GetDemandDrivers();
        });
        $('#CategoryCode').on('keypress', function (e) {
            if (e.keyCode == 13) {
                WO.CatCodeClick();
                return false;

            }
        });
        $('#PackCode').on('keypress', function (e) {
            if (e.keyCode == 13) {
                WO.PackCodeClick();
                return false;

            }
        });
        $('#AlternateId').on('keypress', function (e) {
            if (e.keyCode == 13) {
                WO.showAlternateId();
                return false;
            }
        });
        $('#Dc').keypress(function (e) {
            var regex = new RegExp("^[a-zA-Z0-9]+$");
            var str = String.fromCharCode(!e.charCode ? e.which : e.charCode);
            if (regex.test(str)) {
                return true;
            }

            e.preventDefault();
            return false;
        });

        $("#Dc").click(function () {
            WO.DCCodeClick();
            return false;
        });

        $('#PurchaseOrder').keypress(function (e) {
            var regex = new RegExp("^[a-zA-Z0-9]+$");
            var str = String.fromCharCode(!e.charCode ? e.which : e.charCode);
            if (regex.test(str)) {
                return true;
            }

            e.preventDefault();
            return false;
        });

        $('#LineItem').keypress(function (evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 46 && charCode > 31
              && (charCode < 48 || charCode > 57))
                return false;

            return true;
        });

        WO.CreateOrder.Week = $("#PlannedWeek").val();
        WO.CreateOrder.Year = $("#PlannedYear").val();
        WO.CreateOrder.Date = $("#PlannedDate").val();

        WO.HideField(false);

    },
    validationPurOrder: function (e) {
        var regex = new RegExp("^[a-zA-Z0-9]+$");
        var str = String.fromCharCode(!e.charCode ? e.which : e.charCode);
        if (regex.test(str)) {
            return true;
        }

        event.preventDefault();
        return false;
    },
    isNumbers: function (evt) {
        var charCode = (evt.which) ? evt.which : event.keyCode;
        if (((charCode != 46) && (charCode < 48 || charCode > 57))) {
            if (event.preventDefault) event.preventDefault();
            else {
                if (event.returnValue) event.returnValue = false;
                return false;
            }

        }
        return true;

    },
    cellOnEdit: function (e) {
        if (e.model.isNew()) {
            e.container.find("td:eq(5) input").focus();
        }
        else {
            //var processSource = e.model.ProgramSource;
            //if (processSource == requisitions.const.ProgramSrc.OS) {
            //    var select = e.model.ProcessedToOS;
            //    if (select == "Y" || select == "I") {
            //        $('#grdRequisitionDetail').data("kendoGrid").closeCell();
            //        var msg = "Unable to Edit the row as Processed To OS Status is " + select;
            //        //alert("Unable to Edit the row. Status is" + select);
            //        ISS.common.notify.error(msg);
            //    }
            //}
            //else {
            //    $('#grdRequisitionDetail').data("kendoGrid").closeCell();
            //}
        }
    },
    updateLBS: function () {
        if ($("#MfgPathId").val() && WO.CreateOrder.TotalDz != $('#frmWoEditor #TotalDozens').val()) {
            var currWO = WO.getWODetailObject(new WorkOrderDetail());
            var gridFab = $('#grdwrkOrderFabric').data("kendoGrid");
            var gridDataFab = gridFab.dataSource.data();
            var gridCum = $("#grdwrkOrderCumulative").data("kendoGrid");
            var gridDataCum = gridCum.dataSource.data();
            var postData = { WO: currWO };
            postData = JSON.stringify(postData);
            ISS.common.blockUI(true);
            ISS.common.executeActionAsynchronous("../order/UpdateLBS", postData, function (stat, resData) {
                if (stat && resData) {
                    $("#Lbs").val(resData.dataCum.Lbs).change();
                    $("#VarianceQty").val(resData.dataCum.VarianceQty).change();
                    if (resData.dataCum.WOCumulative.length == gridDataCum.length) {
                        gridCum.dataSource.data(resData.dataCum.WOCumulative);
                        gridCum.refresh();
                    }
                    if (resData.dataCum.WOFabric.length == gridDataFab.length) {
                        gridFab.dataSource.data(resData.dataCum.WOFabric);
                        gridFab.refresh();
                    }
                }
                ISS.common.blockUI(false);
            });
        }
        return false;
    },
    onChangePFS: function (e) {
        var grid = $('#grdwrkOrderFabric').data("kendoGrid");
        var dataItem = grid.dataItem($(e.currentTarget).closest("tr"));
        dataItem.PullFromStock = $(e.currentTarget).prop('checked');
        var currWO = WO.getWODetailObject(new WorkOrderDetail());
        currWO.WOFabric = new Array()
        currWO.WOFabric.push(dataItem);
        var orderDetailGrid = $('#grdWorkOrderDetail').data("kendoGrid");
        var orderData = orderDetailGrid.dataSource.data();
        if (orderData.length > 0) {
            for (var j = 0; j < orderData.length; j++) {
                if (orderData[j].Id == dataItem.Id) {
                    currWO.Lbs = orderData[j].Lbs;
                    currWO.ActualLbs = orderData[j].ActualLbs;
                    currWO.VarianceQty = orderData[j].VarianceQty;
                }
            }
        }
        currWO.LimitLbs = $("#Limit").val();
        var postData = { WO: currWO };
        postData = JSON.stringify(postData);
        ISS.common.blockUI(true);
        ISS.common.executeActionAsynchronous("../order/OnChangePFS", postData, function (stat, resData) {
            if (stat && resData) {
                //Refresh Cumulative grid
                var gridCum = $("#grdwrkOrderCumulative").data("kendoGrid");
                var cumData = resData.dataCum.WOCumulative;
                for (var i = cumData.length - 1 ; i >= 0; i--) {
                    WO.changeDetDates(cumData[i]);
                }
                gridCum.dataSource.data(cumData)
                $("#Lbs").val(resData.dataCum.Lbs).change();

                $("#VarianceQty").val(resData.dataCum.VarianceQty).change();
                if (orderData.length > 0) {
                    for (var j = 0; j < orderData.length; j++) {
                        if (orderData[j].Id == resData.dataCum.Id) {
                            orderData[j].Lbs = resData.dataCum.Lbs;
                            orderData[j].ActualLbs = resData.dataCum.ActualLbs;
                            orderData[j].VarianceQty = resData.dataCum.VarianceQty;
                        }
                    }
                    orderDetailGrid.refresh();
                }
                var variance = resData.dataCum.Variance;
                WO.updateVariance(variance);
            }
            ISS.common.blockUI(false);
        });
    },

    HideField: function (value) {
        if (value == true) {
            $('#hidelabelfield').show();
            $('#hidevaluefield').show();
            $('#hideblankrow').show();
        }
        else {
            $('#hidelabelfield').hide();
            $('#hidevaluefield').hide();
            $('#hideblankrow').hide();
        }
    },
    HideFieldInline: function (value) {
        var grid = $('#grdWorkOrderDetail').data("kendoGrid");
        if (value == true) {
            grid.showColumn("DemandDriver");
            grid.showColumn("PurchaseOrder");
            grid.showColumn("LineItem");
            //setTimeout(function () {
            //    ISS.common.showPopUpMessage('Must fill in Purchase Order and Line Item.', ISS.common.MsgType.Warning);
            //}, 300)

        }
        else {
            //grid.datasource.fields.DemandDriver.editable = false;
            //grid.datasource.fields.PurchaseOrder.editable = false;
            //grid.datasource.fields.LineItem.editable = false;
            grid.hideColumn("DemandDriver");
            grid.hideColumn("PurchaseOrder");
            grid.hideColumn("LineItem");
        }
    },
    ValidateHAA: function () {
        var value = false;
        var style = $('#SellingStyle').val();
        var colors = $("#ColorCode").data("kendoComboBox").text().toUpperCase();
        var color = WO.getMultiselectItem(colors);
        var attribute = $("#Attribute").data("kendoComboBox").text().toUpperCase();

        var SizeL = '';
        for (var i = 0; i < WO.const.SKUSizeList.length; i++) {
            if (i == 0) {
                SizeL = WO.const.SKUSizeList[i].SizeCD;
            }
            else {
                SizeL = SizeL + ',' + WO.const.SKUSizeList[i].SizeCD;
            }
        }
        $.ajax({
            url: '../Order/validateHAA/',
            data: { Style: style, Color: colors, Attribute: attribute, Size: SizeL },
            type: 'get',
            async: false,
            dataType: "json",
            success: function (result) {
                value = result.Haa;
                WO.const.ErrorSku = false;
                if (result.Error == true) {
                    WO.const.ErrorSku = true;
                    ISS.common.showPopUpMessage('the external_sku_xref needs set up for this APS sku before creating work orders');
                }
            }
        })

        WO.const.validateHaaValue = value;
        return value;
    },
    ValidateHAAInLine: function () {
        var value = false;
        var style = style_cd;
        var colors = color_cd;
        var color = WO.getMultiselectItem(colors);
        var attribute = attr_cd;

        var SizeL = '';
        for (var i = 0; i < WO.const.SKUSizeList.length; i++) {
            if (i == 0) {
                SizeL = WO.const.SKUSizeList[i].SizeCD;
            }
            else {
                SizeL = SizeL + ',' + WO.const.SKUSizeList[i].SizeCD;
            }
        }
        $.ajax({
            url: '../Order/validateHAA/',
            data: { Style: style, Color: colors, Attribute: attribute, Size: SizeL },
            type: 'get',
            async: false,
            dataType: "json",
            success: function (result) {
                value = result.Haa;
                WO.const.ErrorSku = false;
                if (result.Error == true) {
                    WO.const.ErrorSku = true;
                    ISS.common.showPopUpMessage('the external_sku_xref needs set up for this APS sku before creating work orders');
                }
            }
        })

        WO.const.validateHaaValue = value;
        return value;
    },

    GetDemandDrivers: function () {
        var style = $('#SellingStyle').val();
        var colors = $("#ColorCode").data("kendoComboBox").text().toUpperCase();
        var color = WO.getMultiselectItem(colors);
        var attribute = $("#Attribute").data("kendoComboBox").text().toUpperCase();

        var SizeL = '';
        for (var i = 0; i < WO.const.SKUSizeList.length; i++) {
            if (i == 0) {
                SizeL = WO.const.SKUSizeList[i].SizeCD;
            }
            else {
                SizeL = SizeL + ',' + WO.const.SKUSizeList[i].SizeCD;
            }
        }
        var revisionNO = $('#Revision').val();

        var postData = {
            Style: style,
            Color: colors,
            Attribute: attribute,
            Size: SizeL,
            RevisonNo: revisionNO
        }

        var DDlDemandDriver = $('#DemandDriver').data('kendoComboBox');

        ISS.common.executeActionAsynchronous("../order/GetDemandDrivers", JSON.stringify(postData), function (stat, resData) {
            DDlDemandDriver.dataSource.data(resData);
        })

    },

    getRandomId: function (min, max) {
        return Math.floor(Math.random() * (max - min) + min);
    },
    createBdCheck: function (checkBox) {

        if (checkBox.name == 'CreateBd' && checkBox.checked) {
            $("#DozensOnly").prop('checked', false);
        }
        else
            $("#CreateBd").prop('checked', false);

    },
    PackCodeClick: function () {
        WO.loadPackCodeGrid();
        WO.CreateOrder.PKCode = ISS.common.popUp('.divPackCodePopup', 'Pack Code', null, function () {
            setTimeout(function () {
                $("#frmWoEditor #PackCode").focus();
            }, 0)

        });
    },
    loadPackCodeGrid: function () {
        var grid = $("#grdPackCode").data("kendoGrid");
        if (grid.pager.page() > 1) grid.pager.page(1)
        grid.dataSource.read();
        return false;
    },

    showPackCodeDetails: function (e) {
        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        $('#PackCode').val(dataItem.PackCode);
        if (WO.CreateOrder.isUpdateCumOnChange) {
            WO.updateCumulativeAndFabric();
        }
        WO.CreateOrder.PKCode.close();
        $('#PackCode').focus();
        return false;
    },
    onSizeCodeClick: function (e) {
        WO.woSizeInLinePopupClick(e);

    },
    CatCodeClick: function () {
        WO.loadCatCodeGrid();
        WO.CreateOrder.CatCode = ISS.common.popUp('.divCatCodePopup', 'Category Code', null, function () {
            setTimeout(function () {
                $("#frmWoEditor #CategoryCode").focus();
            }, 0)
        });
    },
    loadCatCodeGrid: function () {
        var grid = $("#grdCatCode").data("kendoGrid");
        if (grid.pager.page() > 1) grid.pager.page(1)
        grid.dataSource.read();
        return false;
    },

    DCCodeClick: function () {
        WO.loadDCCodeGrid();
        WO.CreateOrder.DCCode = ISS.common.popUp('.divDCPopup', 'Select DC', null, function () {
            setTimeout(function () {
                $("#frmWO #Dc").focus();
            }, 0)
        });
    },
    loadDCCodeGrid: function () {
        var grid = $("#grdDCCode").data("kendoGrid");
        if (grid.pager.page() > 1) grid.pager.page(1)
        grid.dataSource.read();
        return false;
    },
    showDCCodeDetails: function (e) {
        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        $('#Dc').val(dataItem.DCD);
        WO.CreateOrder.DCCode.close();
        $('#Dc').focus();
        return false;
    },

    showCatCodeDetails: function (e) {
        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        $('#CategoryCode').val(dataItem.CategoryCode);
        if (WO.CreateOrder.isUpdateCumOnChange) {
            WO.updateCumulativeAndFabric();
        }
        WO.CreateOrder.CatCode.close();
        $('#CategoryCode').focus();
        return false;
    },
    revFocus: function (e) {
        //$("#Revision").focus(function () { $(this).select(); });
        $(this).select();
    },
    onSizetoQty: function (e) {
        if (e.value != undefined) {
            var s = e.value.toString();
            if (s.indexOf('-') > -1) {
            }
            else {
                Focusedindex = 13;
                var result = WO.ValidateHAAInLine();
                WO.HideFieldInline(result);
                s = e.value.toString();
                size_qty = s;
                var grid = $("#grdWorkOrderDetail").data("kendoGrid");
                var selectedItem = grid.dataItem(currentTableRow)
                selectedItem ? selectedItem : selectedItem = selectrow;
                if (selectedItem) {
                    selectedItem.MfgPathId = MfgPathId_cd;
                    selectedItem.SizeQty = size_qty;
                    selectedItem.TotalDozens = size_qty;
                    selectedItem.Dozens = size_qty;
                    WO.const.SKUSizeList = [{ Size: size_desc, SizeCD: size_cd, Qty: size_qty }];
                    var skulst = JSON.stringify(WO.const.SKUSizeList);
                    selectedItem.SizeList = skulst;
                    $("#grdWorkOrderDetail").data("kendoGrid").refresh();
                    var SizeL = '';
                    for (var i = 0; i < WO.const.SKUSizeList.length; i++) {
                        if (i == 0) {
                            SizeL = WO.const.SKUSizeList[i].SizeCD;
                        }
                        else {
                            SizeL = SizeL + ',' + WO.const.SKUSizeList[i].SizeCD;
                        }
                    }
                    passingValdata = {
                        Style_Cd: style_cd,
                        Color_Cd: color_cd,
                        Attribute_Cd: attr_cd,
                        Size_Cd: SizeL,
                        Revision_Cd: rev_cd
                    }
                    passingValdata = JSON.stringify(passingValdata);
                    ISS.common.executeActionAsynchronous("../order/GetDemandDriversDrpdn", passingValdata, function (stat, Datares) {
                        if (stat && Datares) {
                            if (Datares.length == 0) {
                                //var result = WO.ValidateHAAInLine();
                                //WO.HideFieldInline(result);
                            }
                            else {
                                selectedItem.DemandDriver = Datares[0].Code;
                                $("#grdWorkOrderDetail").data("kendoGrid").refresh();
                            }
                        }
                    });
                    $("#grdWorkOrderDetail").data("kendoGrid").refresh();
                }

            }
        }
        else
            WO.const.SKUSizeList = e;


        //*************************
        //var result = WO.ValidateHAAInLine();
        //WO.HideFieldInline(result);
        //WO.CreateOrder.popupValidator.hideMessages(MfgPathId_cd);
        if (WO.CreateOrder.isUpdateCumOnChange) {
            WO.updateCumulativeAndFabric();
        }
        //alert("event fired");validationDemand
        //***********
        //Focusedindex = 13;
    },
    validationPurchase: function (e) {

    },
    validationLine: function (e) {
        if (WO.const.validateHaaValue == true) {
            currentTableRow = $(e).closest('tr');
            if (!e.value) {
                return;
            }
            var line_cd = e.value.toUpperCase()
            if (line_cd.indexOf('0') == 0) {
                ISS.common.showPopUpMessage('SKU requires line item number to be without leading zeros');
                return false;
            }
        }
    },
    revSearchClick: function () {

        var colors = $('#ColorCode').data("kendoComboBox").text().toUpperCase();
        var ColorCode = WO.getMultiselectItem(colors);
        // var ColorCode= $('#ColorCode').data("kendoMultiSelect").value();
        var Attribute = $('#Attribute').data("kendoComboBox").text().toUpperCase();
        var SizeList = WO.const.SKUSizeList;
        if ($('#SellingStyle').val() != '' && ColorCode != '' && Attribute != '' && SizeList.length > 0) {
            WO.loadRevDetailsGrid();
            WO.CreateOrder.rev = ISS.common.popUp('.divRevSearchPopup', 'Revision Search', null, function () {
                setTimeout(function () {
                    $("#frmWoEditor #Revision").focus();
                }, 0)
            });

            return false;
        }
        else {
            ISS.common.notify.error('Please enter Style, Color, Attribute and Size to generate revision.');
            return false;
        }
    },
    revSearchClickInLine: function () {

        var colors = color_cd;
        var ColorCode = WO.getMultiselectItem(colors);
        // var ColorCode= $('#ColorCode').data("kendoMultiSelect").value();
        var Attribute = attr_cd;
        var SizeList = WO.const.SKUSizeList;
        if (style_cd != '' && ColorCode != '' && Attribute != '' && SizeList.length > 0) {
            WO.loadRevDetailsGrid();
            WO.CreateOrder.rev = ISS.common.popUp('.divRevSearchPopup', 'Revision Search', null, function () {
                setTimeout(function () {
                    $("#frmWoEditor #Revision").focus();
                }, 0)
            });

            return false;
        }
        else {
            ISS.common.notify.error('Please enter Style, Color, Attribute and Size to generate revision.');
            return false;
        }
    },
    loadRevDetailsGrid: function () {
        var grid = $("#grdRevDetails").data("kendoGrid");
        if (grid.pager.page() > 1) grid.pager.page(1)
        grid.dataSource.read();
        return false;
    },

    searchRevDetails: function () {
        var colors = color_cd;
        //var colors = $('#ColorCode').data("kendoComboBox").text().toUpperCase();// Asif
        var searchCriteria = {
            SellingStyle: style_cd,
            ColorCode: WO.getMultiselectItem(colors),
            Attribute: attr_cd,
            SizeList: JSON.parse(JSON.stringify(WO.const.SKUSizeList)),
            Revision: rev_cd,
            AssortCode: asort_cd
        };
        return searchCriteria;
    },

    showDetails: function (e) {

        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        WO.CreateOrder.rev.close();
        var grid = $("#grdWorkOrderDetail").data("kendoGrid");
        var currentSelection = grid.select().parent();
        var selectedItem = grid.dataItem(currentSelection)
        selectedItem ? selectedItem : selectedItem = selectrow;
        if (selectedItem) {
            selectedItem.PKGStyle = "";
            pkgstyle = dataItem.PKGStyle;
            rev_cd = dataItem.NewRevision;
            selectedItem.PKGStyle = dataItem.PKGStyle;
            selectedItem.Revision = dataItem.NewRevision;
            selectedItem.NewColor = dataItem.ColorCode;
            $("#NewColor").val(dataItem.ColorCode);//change for path_dest_plant issue

            if (selectedItem.ErrorStatus == true) {
                $('tr[data-uid="' + selectedItem.uid + '"]').addClass("highlighted-row");
            }
            else {
                $('tr[data-uid="' + selectedItem.uid + '"]').removeClass("highlighted-row");
            }
            $("#grdWorkOrderDetail").data("kendoGrid").refresh();
        }

        WO.CreateOrder.popupValidator.hideMessages($('#Revision'));
        if (WO.CreateOrder.isUpdateCumOnChange) {
            WO.updateCumulativeAndFabric();
        }
        //WO.getChildSKU();
        Focusedindex = 11;
        return false;
    },

    showmfgPathDetails: function (e) {

        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));

        $('#MfgPathId').val(dataItem.MfgPathId);
        $('#SewPlt').val(dataItem.SewPlt);
        $('#AlternateId').val('');
        $('#CutPath').val('');
        $("#CylinderSizes").val('');
        WO.CreateOrder.popupValidator.hideMessages($('#MfgPathId'));
        if (WO.CreateOrder.isUpdateCumOnChange) {
            WO.updateCumulativeAndFabric();
        }

        WO.CreateOrder.mfgPath.close();
        $('#MfgPathId').focus();
        return false;
    },

    loadMFGPathDtls: function (e) {
        if (WO.CreateOrder.mfgPathVal != $('#frmWoEditor #MfgPathId').val()) {
            $('#frmWoEditor #SewPlt').val('');
            $('#AlternateId').val('');
            $('#CutPath').val('');
            $("#CylinderSizes").val('');
            if (WO.CreateOrder.isUpdateCumOnChange) {
                WO.updateCumulativeAndFabric();
            }
        }

        return false;
    },


    loadAltIdDtls: function (e) {
        if (WO.CreateOrder.AltIdVal != $("#AlternateId").val()) {
            if (WO.CreateOrder.isUpdateCumOnChange) {
                WO.updateCumulativeAndFabric();
            }
        }
        //$("#AlternateId").focus();
        return false;
    },

    loadCutPathDtls: function (e) {
        if (WO.CreateOrder.CutPath != $("#CutPath").val()) {
            if (WO.CreateOrder.isUpdateCumOnChange) {
                WO.updateCumulativeAndFabric();
            }
        }
        //$("#CutPath").focus();
        return false;
    },
    DefaultColor: function () {
        var color = $("#ColorCode").data("kendoComboBox");
        //var data = color.dataSource.data();
        var ds = $('#ColorCode').data().kendoComboBox.dataSource;
        ds.data;
        if (color.value == null) {
            if (data.length > 0)
                color.value(data[0].Color);
        }

    },
    StyleChanged: function () {
        var sStyle = $("#frmWoEditor #SellingStyle").val();
        if (sStyle == "") {
            $("#frmWoEditor #SellingStyle").focus();
            return false;
        }

        if (WO.CreateOrder.popupValidator.validateInput($('#SellingStyle'))) {
            //WO.CreateOrder.popupValidator.hideMessages($('#Revision'));
            //WO.CreateOrder.popupValidator.hideMessages($('#MfgPathId'));
            //WO.CreateOrder.popupValidator.hideMessages($('#PKGStyle'));
            if (WO.CreateOrder.StyleVal != $('#frmWoEditor #SellingStyle').val()) {

                var ds = $("#ColorCode").data("kendoComboBox");
                $("#Attribute").data("kendoComboBox").setDataSource();
                $("#Attribute").data("kendoComboBox").text('');
                // ds.dataSource.read();
                WO.loadColor();
                var gridCat = $("#grdCatCode").data("kendoGrid");
                gridCat.dataSource.data([]);

                var gridMFG = $("#grdmfPathDetails").data("kendoGrid");
                gridMFG.dataSource.data([]);

                var gridPack = $("#grdPackCode").data("kendoGrid");
                gridPack.dataSource.data([]);

                var gridRev = $("#grdRevDetails").data("kendoGrid");
                gridRev.dataSource.data([]);

                $('#Revision').val('');
                $("#MfgPathId").val('');
                $('#SewPlt').val('');
                $("#AlternateId").val('');
                $("#CutPath").val('');
                $("#SelectedSizes").html('');
                var postData = { SellingStyle: $('#SellingStyle').val() };
                $("#PKGStyle").val($('#SellingStyle').val());
                postData = JSON.stringify(postData);

                ISS.common.executeActionAsynchronous("../order/GetWOAsrtCode", postData, function (stat, data) {
                    if (stat) {


                        if (data.length > 0) {

                            $("#AsrtCode").val(data[0].AssortCode);
                            $("#PrimaryDC").val(data[0].PrimaryDC);
                            $("#PackCode").val(data[0].PackCode).change();
                            if ($("#Dc").val() == '') {
                                $("#Dc").val(data[0].PrimaryDC).change();
                            }
                            $("#OriginTypeCode").val(data[0].OriginTypeCode);
                            $("#BusinessUnit").val(data[0].CorpBusUnit);
                            if (WO.CreateOrder.isUpdateCumOnChange) {
                                WO.updateCumulativeAndFabric();
                            }

                        }

                    }

                    else {
                        $('#SellingStyle').val('')
                        ISS.common.notify.error('Please provide a valid style.');
                    }

                });
            }
        }


        return false;
    },
    validateCategoryCodeCWO: function () {

        var postData = { catCode: $('#frmWoEditor #CategoryCode').val() }
        if (WO.CreateOrder.CategoryCode != postData.catCode) {
            postData = JSON.stringify(postData);
            ISS.common.executeActionAsynchronous("ValidateCategoryCode", postData, function (stat, data) {
                if (stat && data) {


                }
                else {

                    ISS.common.showPopUpMessage("Invalid Category Code - " + $('#frmWoEditor #CategoryCode').val(), null, function () {
                        $('#frmWoEditor #CategoryCode').val("");
                        $('#frmWoEditor #CategoryCode').focus();
                    });
                }

            });
        }
    },


    loadColor: function (rowColor) {
        var postData = WO.retrieveColorData()
        var color = $("#ColorCode").data("kendoComboBox");
        var attribute = $("#Attribute").data("kendoComboBox");
        postData = JSON.stringify(postData);
        ISS.common.executeActionAsynchronous("../order/GetColorCodes", postData, function (stat, data) {
            if (stat) {
                if (data.length > 0) {

                    var ds = $('#ColorCode').data().kendoComboBox.dataSource;
                    ds.data(data);
                    //ds.data();
                    if (ds.data().length > 0) {
                        if (rowColor) {
                            color.value(rowColor);
                        }
                        else
                            color.value(data[0].Color);
                    }
                    WO.loadAttr();



                }

                else {
                    ISS.common.notify.error('Please provide a valid style.');
                }

            }
        });

    },

    loadAttr: function () {
        var postDataAttrib = WO.retrieveAttributeData();
        postDataAttrib = JSON.stringify(postDataAttrib);
        ISS.common.executeActionAsynchronous("../order/GetAttributeCodes", postDataAttrib, function (stat, data) {
            if (stat) {
                if (data.length > 0) {
                    var attrib = $("#Attribute").data('kendoComboBox');
                    attrib.dataSource.data(data);
                    if (attrib.value() == '')
                        attrib.value(data[0].Attribute)
                }
            }
        });
    },
    retrieveColorData: function () {
        return {
            SellingStyle: style_cd
        };
    },

    onColorChange: function (e) {
        if (e.sender.dataSource.filter()) {
            e.sender.dataSource.filter({});
        }
        if (ISS.common.validateComboChange(this, 'Invalid Color')) {
            WO.loadAttr();

            WO.const.SKUSizeList = [];
            $("#MultiSizeList").val('');
            var gridSize = $("#grdMultiSKUSize").data("kendoGrid");
            gridSize.dataSource.data([]);
            gridSize.refresh();
            var gridCat = $("#grdCatCode").data("kendoGrid");
            gridCat.dataSource.data([]);
            gridCat.refresh();
            var gridMFG = $("#grdmfPathDetails").data("kendoGrid");
            gridMFG.dataSource.data([]);
            gridMFG.refresh();
            var gridPack = $("#grdPackCode").data("kendoGrid");
            gridPack.dataSource.data([]);
            gridPack.refresh();
            var gridRev = $("#grdRevDetails").data("kendoGrid");
            gridRev.dataSource.data([]);
            gridRev.refresh();
            $('#Revision').val('');
            $("#MfgPathId").val('');
            $("#SelectedSizes").html('');
            if (WO.CreateOrder.isUpdateCumOnChange) {
                WO.updateCumulativeAndFabric();
            }
        }
        return false;
    },

    retrieveAttributeData: function () {
        var result = WO.retrieveColorData();
        var colors = $("#ColorCode").data("kendoComboBox").text().toUpperCase();
        var color = null;
        if (colors.length == 0) {
            color = " ";
        }

        else {
            color = WO.getMultiselectItem(colors);
        }
        result.ColorCode = color;
        $("#ColorCode").data("kendoComboBox").input.select();
        result.Src = "";
        return result;
    },

    LimitChanged: function () {
        var currWO = WO.getWODetailObject(new WorkOrderDetail());
        var postData = { WO: currWO };
        postData = JSON.stringify(postData);
        ISS.common.executeActionAsynchronous("../order/CalculateVariance", postData, function (stat, resData) {
            if (stat && resData) {
                if (resData.dataCum) {
                    var variance = resData.dataCum.Variance;
                    WO.updateVariance(variance);

                }
            }
            else {

            }
        });
        return false;
    },

    updateVariance: function (variance) {
        if (variance > 0) {
            $('#Variance').val('+' + variance).change().removeClass("fontgreen fontblue fontred").addClass("fontgreen");


        }
        else if (variance < 0) {
            $('#Variance').val(variance).change().removeClass("fontgreen fontblue fontred").addClass("fontred");


        }
        else {
            $('#Variance').val(variance).change().removeClass("fontgreen fontblue fontred").addClass("fontblue");


        }


    },
    onAttributeChange: function () {
        if (ISS.common.validateComboChange(this, 'Invalid Attribute')) {
            WO.const.SKUSizeList = [];
            $("#MultiSizeList").val('');
            var gridSize = $("#grdMultiSKUSize").data("kendoGrid");
            gridSize.dataSource.data([]);
            gridSize.refresh();
            var gridCat = $("#grdCatCode").data("kendoGrid");
            gridCat.dataSource.data([]);
            gridCat.refresh();
            var gridMFG = $("#grdmfPathDetails").data("kendoGrid");
            gridMFG.dataSource.data([]);
            gridMFG.refresh();
            var gridPack = $("#grdPackCode").data("kendoGrid");
            gridPack.dataSource.data([]);
            gridPack.refresh();
            var gridRev = $("#grdRevDetails").data("kendoGrid");
            gridRev.dataSource.data([]);
            gridRev.refresh();
            $('#Revision').val('');
            $("#MfgPathId").val('');
            $("#SelectedSizes").html('');
            if (WO.CreateOrder.isUpdateCumOnChange) {
                WO.updateCumulativeAndFabric();
            }
        }
        return false;
    },
    onOrdersToCreateChange: function () {
        var gridCum = $("#grdwrkOrderCumulative").data("kendoGrid");
        var ordersToCreate = $("#OrdersToCreate").data("kendoNumericTextBox");

        if (ordersToCreate.value() > 999) {
            ISS.common.notify.error('Number of orders to create should be less than 1000');
            ordersToCreate.value(1);
            return false;
        }
        //ordersToCreate.value(1);

        var dataCum = gridCum.dataSource.data();
        if (dataCum.length > 0) {
            var currWO = WO.getWODetailObject(new WorkOrderDetail());
            var postData = { WO: currWO };
            postData = JSON.stringify(postData);
            ISS.common.executeActionAsynchronous("../order/OrdersToCreate", postData, function (stat, resData) {
                if (stat && resData) {
                    //var gridCum = $("#grdwrkOrderCumulative").data("kendoGrid");
                    //var dataCum = gridCum.dataSource.data();
                    for (var i = dataCum.length - 1 ; i >= 0; i--) {
                        dataCum.remove(dataCum[i]);
                    }
                    if (resData.dataCum) {
                        var cumData = resData.dataCum.WOCumulative;
                        for (var i = cumData.length - 1 ; i >= 0; i--) {
                            WO.changeDetDates(cumData[i]);
                            dataCum.push(cumData[i]);
                        }
                    }
                }
                else {

                }

            });

        }
        return false;
    },
    retrieveSizeData: function () {
        var result = WO.retrieveColorData();
        var ColorCode = $("#ColorCode").data("kendoComboBox");
        var Attribute = $("#Attribute").data("kendoComboBox");
        result.ColorCode = ColorCode.value();
        result.Attribute = Attribute.text();
        return result;
    },


    onSizeChange: function () {
        WO.CreateOrder.popupValidator.hideMessages($('#Revision'));
        var postData = WO.retrieveColorData();
        var colors = color_cd;
        // var colors = $('#ColorCode').data("kendoComboBox").text().toUpperCase(); //Asif
        var ColorCode = WO.getMultiselectItem(colors);
        var Attribute = attr_cd;

        if (ColorCode != '' && Attribute != '' && WO.const.SKUSizeList.length > 0) {
            //if ($.trim($("#Revision").val()) == '') {
            postData.ColorCode = ColorCode;
            postData.Attribute = Attribute;

            postData.SizeList = WO.const.SKUSizeList;
            postData.AssortCode = $("#AsrtCode").val();
            postData = JSON.stringify(postData);
            //ISS.common.executeActionAsynchronous("../order/GetMaxRevision", postData, function (stat, data) {
            //    if (stat) {
            //        if (data.length > 0) {
            //            rev_cd = data[0].Revision;
            //            WO.getChildSKU();
            //        }
            //    }
            //    else {
            //        ISS.common.notify.error('Failed to retrieve Revision details.');
            //    }

            //});
            //} //end rev null check
            //else {
            //    if (WO.CreateOrder.isUpdateCumOnChange) {
            //        WO.updateCumulativeAndFabricGrid();
            //    }
            //}
        }

        return false;


    },

    getPKGStyle: function (callback) {
        WO.CreateOrder.popupValidator.hideMessages($('#PKGStyle'));
        var postData = WO.retrieveColorData();
        var colors = $("#ColorCode").data("kendoComboBox").value();
        var ColorCode = WO.getMultiselectItem(colors);
        var Attribute = $("#Attribute").data("kendoComboBox");
        if (ColorCode != '' && Attribute.text() != '' && WO.const.SKUSizeList.length > 0) {
            postData.ColorCode = ColorCode;
            postData.Attribute = Attribute.text();
            postData.SizeList = WO.const.SKUSizeList;
            postData.AssortCode = $("#AsrtCode").val();
            postData.Revision = $("#Revision").val();
            postData = JSON.stringify(postData);
            ISS.common.executeActionAsynchronous("../order/GetPKGStyle", postData, function (stat, data) {
                if (stat && data) {
                    if (data.length > 0) {
                        $("#PKGStyle").val(data[0].PKGStyle).change();
                    }
                    else {
                        $("#PKGStyle").val($('#SellingStyle').val()).change();
                    }
                }
                else {
                    ISS.common.notify.error('Failed to Load PKG Style.');
                }
                if (callback) callback(data.length > 0)
            });
        }
    },

    getChildSKU: function () {

        var postData = WO.retrieveColorData();
        var colors = color_cd;
        // var colors = $("#ColorCode").data("kendoComboBox").text().toUpperCase(); //Asif
        var color = WO.getMultiselectItem(colors);
        var attr = attr_cd;


        postData.ColorCode = color;
        postData.Attribute = attr;
        postData.OrginTypeCode = $("#OriginTypeCode").val();

        postData.SizeList = WO.const.SKUSizeList;
        postData.AssortCode = asort_cd;
        postData.Revision = rev_cd;
        postData = JSON.stringify(postData);
        ISS.common.executeActionAsynchronous("../order/GetWOChildSKU", postData, function (stat, data) {
            if (stat) {


                if (data.length > 0) {

                    $("#NewStyle").val(data[0].NewStyle);
                    $("#PKGStyle").val(data[0].NewStyle);
                    cur_row ? cur_row : cur_row = selectrow;
                    cur_row.PKGStyle = data[0].NewStyle;
                    pkgstyle = data[0].NewStyle;
                    $("#NewColor").val(data[0].NewColor);
                    $("#NewAttribute").val(data[0].NewAttribute);
                    $("#NewSize").val(data[0].NewSize);
                    $("#grdWorkOrderDetail").data("kendoGrid").refresh();
                    WO.updateStyleInfo();
                }
                else {
                    cur_row ? cur_row : cur_row = selectrow;
                    pkgstyle = style_cd;
                    $("#PKGStyle").val(style_cd);
                    cur_row.PKGStyle = pkgstyle;
                    $("#grdWorkOrderDetail").data("kendoGrid").refresh();
                }

            }

            else {
                ISS.common.notify.error('Failed to retrieve Style details.');
            }

        });
        return false;
    },
    updateStyleInfo: function () {

        var postData = { SellingStyle: $('#NewStyle').val() };
        postData = JSON.stringify(postData);

        ISS.common.executeActionAsynchronous("../order/GetWOAsrtCode", postData, function (stat, data) {
            if (stat) {


                if (data.length > 0) {
                    $("#AsrtCode").val(data[0].AssortCode);
                    asort_cd = data[0].AssortCode;
                    $("#PrimaryDC").val(data[0].PrimaryDC);
                    prm_cd = data[0].PrimaryDC;
                    $("#PackCode").val(data[0].PackCode).change();
                    PackCode_cd = data[0].PackCode;
                    if (data[0].PrimaryDC != null)
                        if ($("#Dc").val() == '') {
                            $("#Dc").val(data[0].PrimaryDC).change();
                        }
                    $("#OriginTypeCode").val(data[0].OriginTypeCode);
                    orgin_cd = data[0].OriginTypeCode;

                }

            }

            else {
                ISS.common.notify.error('Failed to retrieve Style details.');
            }

        });
        return false;
    },
    retrieveRevisionData: function () {
        var result = WO.retrieveColorData();
        var colors = $("#ColorCode").data("kendoComboBox").text().toUpperCase();
        var color = WO.getMultiselectItem(colors);
        var attr = $("#Attribute").data("kendoComboBox");
        var size = $("#SizeShortDes").data("kendoDropDownList");
        result.ColoCode = color;
        result.AttributeCode = attr.text().toUpperCase();
        result.SizeCode = size.value();
        return result;
    },

    onRevisionSelect: function () {
        var ds = $("#Revision").data("kendoDropDownList");
        ds.dataSource.read();
        if (WO.CreateOrder.isUpdateCumOnChange) {
            WO.updateCumulativeAndFabric();
        }
        return false;
    },

    onRevisionChange: function () {

    },
    retrievePKGData: function () {
        var result = WO.retrieveColorData();
        var colors = $("#ColorCode").data("kendoComboBox").text().toUpperCase();
        var color = WO.getMultiselectItem(colors);
        var attr = $("#Attribute").data("kendoComboBox");
        var size = $("#SizeShortDes").data("kendoDropDownList");
        var revision = $("#Revision").val();
        var asrtCode = $("#AsrtCode").val();
        result.Color_Cd = color;
        result.Attribute_Cd = attr.text().toUpperCase();
        result.Size_Cd = size.text();

        result.Asrt_Cd = asrtCode;
        return result;
    },

    onPackageSelect: function () {

    },

    PKGChanged: function () {
        if (WO.CreateOrder.isUpdateCumOnChange) {
            WO.updateCumulativeAndFabric();
        }
        return false;
    },

    retrieveMFGData: function () {
        var result = WO.retrieveColorData();
        var colors = $("#ColorCode").data("kendoComboBox").text();
        var color = WO.getMultiselectItem(colors);
        var attr = $("#Attribute").data("kendoComboBox");


        //var demLoc =  $("#Dc").val();
        var demLoc = $("#PrimaryDC").val();
        result.ColorCode = color;
        result.Attribute = attr.text().toUpperCase();
        result.SizeList = JSON.parse(JSON.stringify(WO.const.SKUSizeList));
        result.PrimaryDC = demLoc;
        return result;
    },


    mfPathClick: function () {
        var colors = $('#ColorCode').data("kendoComboBox").text();
        ColorCode = WO.getMultiselectItem(colors);
        //var ColorCode= $('#ColorCode').data("kendoMultiSelect").value();
        var Attribute = $('#Attribute').data("kendoComboBox").text();
        var SizeList = WO.const.SKUSizeList;
        if ($('#SellingStyle').val() != '' && ColorCode != '' && Attribute != '' && SizeList.length > 0) {
            WO.loadmfgPathDetailsGrid();
            WO.CreateOrder.mfgPath = ISS.common.popUp('.divmfgPathPopup', 'Select a Sew Plant', null, function () {
                setTimeout(function () {
                    $("#frmWoEditor #MfgPathId").focus();
                }, 0)
            });
        }
        else {
            ISS.common.notify.error('Please enter Style, Color, Attribute and Size to generate Mfg Path.');

        }
        return false;
    },

    loadmfgPathDetailsGrid: function () {
        var grid = $("#grdmfPathDetails").data("kendoGrid");
        if (grid.pager.page() > 1) grid.pager.page(1)
        grid.dataSource.read();
        return false;
    },


    changeYear: function (e, callback) {

        var Year = $("#PlannedYear").val();
        var Week = $("#PlannedWeek").val();
        var dueDate = $("#DueDate").text();
        var postData = { Week: $("#PlannedWeek").val(), Year: $("#PlannedYear").val(), dueDate: $("#DueDate").val() };
        postData = JSON.stringify(postData);
        ISS.common.executeActionAsynchronous("../order/GetPlannedDate", postData, function (stat, data) {
            if (stat) {
                if (data) {
                    if (data.ErrMsg && data.ErrMsg != '') {

                        ISS.common.notify.error(data.ErrMsg);
                        if (callback) callback(false);
                    }
                    else {
                        if (callback) callback(true);
                    }
                    $("#PlannedDate").val(data.result);
                }
            }
            else {
                ISS.common.notify.error('Failed to retrieve Planned Date.');
            }
            if (callback) callback(false);
        });

        WO.validateCreateWO();
        return false;
    },
    searchDataWO: function () {
        return ISS.common.getFormData($('#frmWO'));
    },
    showWOEditor: function (e) {
        $("#gridMode").val('add');
        WO.clearWOdetail();
        $("#gridMode").val('add');
        var gridCum = $("#grdwrkOrderCumulative").data("kendoGrid");
        var gridFab = $("#grdwrkOrderFabric").data("kendoGrid");
        WO.CreateOrder.editDataCum = gridCum.dataSource.data();
        WO.CreateOrder.editDataFab = gridFab.dataSource.data();
        var orderId = WO.getRandomId(1, 999);
        $("#OrderDetailId").val(orderId);

        var dsColor = $("#ColorCode").data("KendoComboBox");
        //dsColor.dataSource.read();
        var ds = $('#ColorCode').data().kendoComboBox.dataSource;
        ds.read();
        var dsAttr = $("#Attribute").data("kendoComboBox");
        var dsAttribute = $('#ColorCode').data().kendoComboBox.dataSource;
        dsAttribute.read();
        $("#DozensOnly").prop('checked', true);
        $('#CreateBd').prop('checked', false);
        //$("#myDiv").show();

        WO.CreateOrder.editPopUp = ISS.common.popUp('#myDiv', 'Edit', null, function (rr) {

            if (rr.userTriggered) {
                rr._defaultPrevented = true;
                ISS.common.showConfirmMessage('Pending changes will be lost.<br/> Do you want to continue by losing your changes?', null, function (reply) {
                    if (reply) {

                        WO.CreateOrder.editPopUp.close();
                        WO.ResetCumFabData(true);
                    }

                });
            }
        });


        WO.CreateOrder.currentRow = null;
        $("#SellingStyle").focus();
        return false;
    },

    loadWOGrid: function () {
        var grid = $("#grdWO").data("kendoGrid");
        if (WO.WO.IsInit) {
            grid.table.on('click', '.lnkReason', function (e) {
                WO.showExceptionDetailss(e);
            });
            WO.WO.IsInit = false;
        }
        if (grid.pager.page() > 1) grid.pager.page(1)
        grid.dataSource.read();

        ISS.common.collapsePanel('#panelbar-images', 0);
        return false;
    },

    searchDataExceptionDetails: function () {
        return WO.WO.Exception;
    },

    showExceptionDetailss: function (e) {
        var grid = $("#grdWO").data("kendoGrid");
        var gride = $("#grdExceptionDetails").data("kendoGrid");
        var dataItem = grid.dataItem($(e.currentTarget).closest("tr"));
        WO.WO.Exception = { SuperOrder: dataItem.SuperOrder };
        if (grid.dataSource._filter != null && grid.dataSource._filter.filters.length > 0)
            grid.dataSource.filter([]);
        gride.dataSource.read()
        ISS.common.popUp('.divExceptionDetails', 'ISS AS400 Exceptions')
    },

    RecalcWO: function () {
        var gridCum = $("#grdwrkOrderCumulative").data("kendoGrid");
        var gridFab = $("#grdwrkOrderFabric").data("kendoGrid");
        var gridWODet = $("#grdWorkOrderDetail").data("kendoGrid");
        var dataWODet = gridWODet.dataSource.data();
        var dataCum = gridCum.dataSource.data();
        var dataFab = gridFab.dataSource.data();
        WO.CreateOrder.validator.hideMessages();
        WO.CreateOrder.popupValidator.hideMessages();

        ISS.common.blockUI(true);

        var currWO = WO.getWODetailObject(new WorkOrderDetail());
        var postData = { WO: currWO };
        postData = JSON.stringify(postData);
        ISS.common.executeActionAsynchronous("../order/ReCalcWODetail", postData, function (stat, resData) {
            if (stat && resData) {

                // delete all from cumulative using ID
                for (var i = dataCum.length - 1 ; i >= 0; i--) {
                    dataCum.remove(dataCum[i]);
                }
                // delete all from Fabric using ID
                for (var i = dataFab.length - 1 ; i >= 0; i--) {
                    dataFab.remove(dataFab[i]);
                }
                //Add to Cumulative
                if (resData.dataCum) {
                    var cumData = resData.dataCum.WOCumulative;
                    for (var i = cumData.length - 1 ; i >= 0; i--)
                        dataCum.push(cumData[i]);
                    //Merging and grouping of Cumulative grid
                    //Add to Fabric
                    var fabData = resData.dataCum.WOFabric;
                    for (var i = fabData.length - 1; i >= 0; i--)
                        dataFab.push(fabData[i]);
                    var AltId = resData.dataCum.AlternateId;
                    $("#AlternateId").val(AltId).change();
                    var Cylsize = resData.dataCum.CylinderSizes;
                    $("#CylinderSizes").val(Cylsize).change();
                    var Lbs = resData.dataCum.Lbs;
                    $("#Lbs").val(Lbs).change();
                    //Add to textiles
                    var texData = resData.dataCum.WOTextiles;
                    var dataSource = [];
                    var ds = $("#TxtPlant").data("kendoComboBox").dataSource;
                    var dsmac = $("#MacType").data("kendoComboBox").dataSource;
                    var datasourcemac = [];
                    var limit = 0;
                    if (texData != null) {
                        for (var i = texData.length - 1; i >= 0; i--) {
                            if (texData[i].TxtPlant != null) {
                                ds.add({ Text: texData[i].TxtPlant, Value: texData[i].TxtPlant });
                                limit = texData[i].Limit;
                            }
                            if (texData[i].MacType != null)
                                dsmac.add({ Text: texData[i].MacType, Value: texData[i].MacType });
                        }
                    }

                    $('#Limit').val(limit).change();
                    if (resData.dataCum.CreateBDInd == "Y") {
                        $('#CreateBd').prop('checked', true);
                        $('#DozensOnly').prop('checked', false);
                    }
                    else {
                        $('#CreateBd').prop('checked', false);
                        $('#DozensOnly').prop('checked', true);
                    }
                    $('#Variance').val(resData.dataCum.Variance).change();
                    $('#CutPath').val(resData.dataCum.CutPath).change();
                }

                gridCum.refresh();
                gridFab.refresh();
                ISS.common.blockUI(false);
                return false;
            }
            else {
                ISS.common.blockUI(false);
            }
        });

        return false;

    },

    ReCalcOrders: function () {
        //if( $("#gridMode").val(''))
        //{
        var gridDetail = $("#grdWorkOrderDetail").data("kendoGrid");
        var dsWOData = gridDetail.dataSource.data();
        if (dsWOData.length <= 0) {
            ISS.common.showPopUpMessage('There are no orders to re-calculate.');
            return false;
        }

        ISS.common.showConfirmMessage('Are your Sure you want to Re-Calculate Order Details.?<br/>Press "Yes" to continue', null, function (reply) {
            if (reply) {
                WO.RecalcWO();
            }
        });

        return false;
        //}
        //else {
        //    ISS.common.notify.error('Pending changes are identified in the page <br/> Please save the changes and proceed.');
        //    return false;
        //}
    },
    cancelWODetail: function (isdelete) {

        WO.CreateOrder.validator.hideMessages();
        if (!isdelete) {
            ISS.common.showConfirmMessage('Pending changes will be lost.<br/> Do you want to continue by losing your changes?', null, function (reply) {
                if (reply) {
                    //
                    WO.ResetCumFabData();
                    return false;
                }
                else {
                    WO.CreateOrder.validator.showMessages();
                    return false;
                }
            })
        }
        else {
            WO.ResetCumFabData();
            return false;
        }

    },
    ResetCumFabData: function (close) {
        var gridCum = $("#grdwrkOrderCumulative").data("kendoGrid");
        var gridFab = $("#grdwrkOrderFabric").data("kendoGrid");
        var gridWODet = $("#grdWorkOrderDetail").data("kendoGrid");
        var dataWODet = gridWODet.dataSource.data();
        var dataCum = gridCum.dataSource.data();
        var dataFab = gridFab.dataSource.data();
        ISS.common.blockUI(true);
        var currWO = WO.getWODetailObject(new WorkOrderDetail());
        var postData = { WO: currWO };
        postData = JSON.stringify(postData);
        ISS.common.executeActionAsynchronous("../order/CancelWODetail", postData, function (stat, resData) {
            if (stat && resData) {

                // delete all from cumulative using ID
                for (var i = dataCum.length - 1 ; i >= 0; i--) {
                    dataCum.remove(dataCum[i]);
                }
                // delete all from Fabric using ID
                for (var i = dataFab.length - 1 ; i >= 0; i--) {
                    dataFab.remove(dataFab[i]);
                }
                //Add to Cumulative
                if (resData.dataCum) {
                    var cumData = resData.dataCum.WOCumulative;
                    if (cumData != null) {
                        for (var i = cumData.length - 1 ; i >= 0; i--)
                            dataCum.push(cumData[i]);
                    }
                    //Merging and grouping of Cumulative grid
                    //Add to Fabric
                    var fabData = resData.dataCum.WOFabric;
                    if (fabData != null) {
                        for (var i = fabData.length - 1; i >= 0; i--)
                            dataFab.push(fabData[i]);
                    }
                    var AltId = resData.dataCum.AlternateId;
                    $("#AlternateId").val(AltId).change();
                    var Cylsize = resData.dataCum.CylinderSizes;
                    $("#CylinderSizes").val(Cylsize).change();
                    var Lbs = resData.dataCum.Lbs;
                    $("#Lbs").val(Lbs).change();
                    //Add to textiles
                    var texData = resData.dataCum.WOTextiles;
                    var dataSource = [];
                    var ds = $("#TxtPlant").data("kendoComboBox").dataSource;
                    var dsmac = $("#MacType").data("kendoComboBox").dataSource;
                    var datasourcemac = [];
                    var limit = 0;
                    if (texData != null) {
                        for (var i = texData.length - 1; i >= 0; i--) {
                            if (texData[i].TxtPlant != null) {
                                ds.add({ Text: texData[i].TxtPlant, Value: texData[i].TxtPlant });
                                limit = texData[i].Limit;
                            }
                            if (texData[i].MacType != null)
                                dsmac.add({ Text: texData[i].MacType, Value: texData[i].MacType });
                        }
                    }
                    if (dataWODet.length == 0) {
                        WO.ClearHeaderInfo();
                    }
                    else {
                        if (dataWODet[0].PrimaryDC != null && dataWODet[0].PrimaryDC != undefined && dataWODet[0].PrimaryDC != "")
                            $("#Dc").val(dataWODet[0].PrimaryDC).change();
                        //$('#frmWO #DC').val(dataWODet[0].PrimaryDC).change();
                    }
                    $('#Limit').val(limit).change();
                    $('#VarianceQty').val('0');
                    if (resData.dataCum.CreateBDInd == "Y") {
                        $('#CreateBd').prop('checked', true);
                        $('#DozensOnly').prop('checked', false);
                    }
                    else {
                        $('#CreateBd').prop('checked', false);
                        $('#DozensOnly').prop('checked', true);
                    }
                    $('#Variance').val(resData.dataCum.Variance).change();
                    $('#CutPath').val(resData.dataCum.CutPath).change();

                }

                gridCum.refresh();
                gridFab.refresh();
                ISS.common.blockUI(false);
                return false;
            }
            else {
                ISS.common.blockUI(false);
            }
        });
        WO.clearWOdetail();
        // $("#myDiv").hide();
        if (!close) WO.CreateOrder.editPopUp.close();
        //ISS.common.blockUI();
        $("#gridMode").val('');
    },
    ClearHeaderInfo: function () {
        var gridMode = $("#gridMode").val();
        // if (gridMode == "") {
        var txtPlant = $("#TxtPlant").data("kendoComboBox");
        txtPlant.dataSource.data([]);
        txtPlant.text("");
        txtPlant.value("");
        var macType = $("#MacType").data("kendoComboBox");
        macType.dataSource.data([]);
        macType.text("");
        macType.value("");
        WO.CreateOrder.plantMachineList = null;

        //$("#Dc").val('');
        $("#OrderDetailId").val('');
        $("#Limit").val('');
        $("#Variance").val('');
        var ordersToCreate = $("#OrdersToCreate").data("kendoNumericTextBox");
        //ordersToCreate.text(1);
        ordersToCreate.value(1);

        var PlannerCode = $("#PlannerCd").data("kendoDropDownList");
        PlannerCode.text("-Select-");
        PlannerCode.value(" ");


        if (WO.CreateOrder.Week != null && WO.CreateOrder.Week != "") {
            var numerictextbox = $("#PlannedWeek").data("kendoNumericTextBox");
            numerictextbox.value(WO.CreateOrder.Week);
        }

        if (WO.CreateOrder.Year != null && WO.CreateOrder.Year != "") {
            var numerictextbox = $("#PlannedYear").data("kendoNumericTextBox");
            numerictextbox.value(WO.CreateOrder.Year);
        }

        if (WO.CreateOrder.Date != null && WO.CreateOrder.Date != "")
            $("#PlannedDate").val(WO.CreateOrder.Date);

        // }
    },
    ClearWOPage: function () {
        var gridWoD = $("#grdWorkOrderDetail").data("kendoGrid");
        var gridCum = $("#grdwrkOrderCumulative").data("kendoGrid");
        var gridFab = $("#grdwrkOrderFabric").data("kendoGrid");
        gridWoD.dataSource.data([]);
        gridCum.dataSource.data([]);
        gridFab.dataSource.data([]);
        WO.clearWOdetail();
        $("#Dc").val('');
        $("#OrderDetailId").val('');
        $("#Limit").val('');
        $("#Variance").val('');
        WO.CreateOrder.plantMachineList = null;
        return false;
    },
    ClearOrders: function () {

        ISS.common.showConfirmMessage('Clearing will remove all orders from the screen.<br/> Do you want to continue by losing your changes?', null, function (reply) {
            if (reply) {
                ISS.common.blockUI(true);
                WO.ClearWOPage();
                ISS.common.blockUI(false);
                return false;
            }
            else {
                return false;
            }
        })
        return false;
    },
    clearWOdetail: function () {
        DataUid = 0;
        selectrow = null;
        cur_row = null;
        id_cd = "";
        style_cd = "";
        color_cd = "";
        attr_cd = "";
        size_cd = "";
        rev_cd = "";
        pkgstyle = "";
        MfgPathId_cd = "";
        SewPlt_cd = "";
        PackCode_cd = "";
        $("#NewColor").val('');
        $("#NewSize").val('');
        $("#NewAttribute").val('');
        AlternateId_cd = "";
        $("#CylinderSizes").val('');
        $("#GroupId").val('');
        WO.const.SKUSizeList = [];
        $("#TotalDozens").val('');
        $("#Dozens").val('');
        $("#Lbs").val('');
        $("#Note").val('');

        PriorityCode_cd = "";

        prm_cd = "";
        asort_cd = "";
        var gridWoD = $("#grdWorkOrderDetail").data("kendoGrid");
        var wodData = gridWoD.dataSource.data();

        $('#SellingStyle').val('');
        var ColorCode = $("#ColorCode").data("KendoComboBox");
        //ColorCode.dataSource.data([]);
        // ColorCode.text("");
        var ds = $('#ColorCode').data().kendoComboBox.dataSource;
        $("#ColorCode").data("kendoComboBox").setDataSource();
        //ColorCode.text("");
        var Attribute = $("#Attribute").data("kendoComboBox");
        $("#Attribute").data("kendoComboBox").setDataSource();
        $("#PurchaseOrder").val('');
        $("#LineItem").val('');
        var DD = $("#DemandDriver").data("kendoComboBox");
        DD.dataSource.data([]);
        // Attribute.dataSource.data([]);
        //Attribute.text("");
        //Attribute.value("");
        var gridMode = $("#gridMode").val();
        if (wodData.length == 0 && gridMode == "") {
            var txtPlant = $("#TxtPlant").data("kendoComboBox");
            txtPlant.dataSource.data([]);
            txtPlant.text("");
            txtPlant.value("");
            var macType = $("#MacType").data("kendoComboBox");
            macType.dataSource.data([]);
            macType.text("");
            macType.value("");
            WO.CreateOrder.plantMachineList = null;

            $("#Dc").val('');
            $("#OrderDetailId").val('');
            $("#Limit").val('');
            $("#Variance").val('');
            var ordersToCreate = $("#OrdersToCreate").data("kendoNumericTextBox");
            //ordersToCreate.text(1);
            ordersToCreate.value(1);
            var PlannerCode = $("#PlannerCd").data("kendoDropDownList");
            PlannerCode.text("-Select-");
            PlannerCode.value(" ");

            if (WO.CreateOrder.Week != null && WO.CreateOrder.Week != "") {
                var numerictextbox = $("#PlannedWeek").data("kendoNumericTextBox");
                numerictextbox.value(WO.CreateOrder.Week);
            }

            if (WO.CreateOrder.Year != null && WO.CreateOrder.Year != "") {
                var numerictextbox = $("#PlannedYear").data("kendoNumericTextBox");
                numerictextbox.value(WO.CreateOrder.Year);
            }

            if (WO.CreateOrder.Date != null && WO.CreateOrder.Date != "")
                $("#PlannedDate").val(WO.CreateOrder.Date);

        }
        var gridSize = $("#grdMultiSKUSize").data("kendoGrid");
        gridSize.dataSource.data([]);
        gridSize.refresh();
        var gridCat = $("#grdCatCode").data("kendoGrid");
        gridCat.dataSource.data([]);
        gridCat.refresh();
        var gridMFG = $("#grdmfPathDetails").data("kendoGrid");
        gridMFG.dataSource.data([]);
        gridMFG.refresh();
        var gridPack = $("#grdPackCode").data("kendoGrid");
        gridPack.dataSource.data([]);
        gridPack.refresh();
        var gridRev = $("#grdRevDetails").data("kendoGrid");
        gridRev.dataSource.data([]);
        gridRev.refresh();
        WO.const.SKUSizeList = [];
        $("#MultiSizeList").val('');
        $('#Revision').val('');
        $("#MfgPathId").val('');
        $("#SewPlt").val('');
        $("#PackCode").val('');
        $("#PKGStyle").val('');
        $("#AlternateId").val('');
        $("#CylinderSizes").val('');
        $("#CutPath").val('');
        $("#TotalDozens").val('');
        $("#Dozens").val('0');
        $("#Lbs").val('0');
        $('#AttributionPath').val('');
        $("#CategoryCode").val('');
        $("#BodyTrim").val('');
        $("#PriorityCode").val(50);
        $("#GroupId").val('0');
        $("#Note").val('');
        $("#SelectedSizes").html('');
        WO.CreateOrder.validator.hideMessages();
        WO.CreateOrder.popupValidator.hideMessages();

        WO.CreateOrder.isLoadingCompleted = true;
        $("#gridMode").val('');
        return false;
    },

    onFabricDataBound: function () {
        var grid = $("#grdwrkOrderFabric").data("kendoGrid");
        var gridData = grid.dataSource.view();
        for (var i = 0; i < gridData.length; i++) {
            var row = grid.table.find("tr[data-uid='" + gridData[i].uid + "']");
            if (gridData[i].IsHide) {
                row.hide();
                continue;
            }
            else {
                row.show();
            }
        }
    },
    editOrderDetail: function (e) {
        $("#gridMode").val('edit');
        var tr = $(e.target).closest("tr");
        var data = this.dataItem(tr);
        WO.fillWODetail(data);
        var gridCum = $("#grdwrkOrderCumulative").data("kendoGrid");
        var gridFab = $("#grdwrkOrderFabric").data("kendoGrid");
        WO.CreateOrder.editDataCum = gridCum.dataSource.data();
        WO.CreateOrder.editDataFab = gridFab.dataSource.data();
        WO.CreateOrder.currentRow = data;
        if (data.CreateBd) {
            $("#DozensOnly").prop('checked', false);
            $('#CreateBd').prop('checked', true);
        }
        else {
            $("#DozensOnly").prop('checked', true);
            $('#CreateBd').prop('checked', false);
        }
        //$("#myDiv").show();
        WO.CreateOrder.editPopUp = ISS.common.popUp('#myDiv', 'Edit', null, function (e) {
            //if(e.userTriggered)
            //    WO.ResetCumFabData( true);
            if (e.userTriggered) {
                e._defaultPrevented = true;
                ISS.common.showConfirmMessage('Pending changes will be lost.<br/> Do you want to continue by losing your changes?', null, function (reply) {
                    if (reply) {

                        WO.CreateOrder.editPopUp.close();
                        WO.ResetCumFabData(true);
                    }

                });
            }
        });
        $("#SellingStyle").focus();
        return false;

    },



    deleteOrderDetail: function (e) {
        //if ($("#gridMode").val() == "") {
        ISS.common.showConfirmMessage('Do you want to delete?', null, function (reply) {
            if (reply) {
                var grid = $("#grdWorkOrderDetail").data("kendoGrid");
                var data = grid.dataSource.data();
                var tr = $(e.target).closest("tr");
                var itemtoRemove = grid.dataItem(tr);

                $("#gridMode").val('delete')
                var idToDelete = itemtoRemove.Id;
                $("#OrderDetailId").val(idToDelete);

                WO.ResetCumFabData(true);
                data.remove(itemtoRemove);

                grid.refresh();

                $("#gridMode").val('')
            }
        });
        return false;

        //}
        //else {
        //    ISS.common.notify.error('Pending changes are identified in the page <br/> Please save the changes and proceed.');
        //    return false;
        //}

    },

    getSizeDisplay: function (list) {
        var ordersizes = '';
        if (list.length > 0) {
            for (var i = 0; i < list.length; i++) {
                if (i > 0) {
                    ordersizes = ordersizes + ", " + list[i].Size + ' - ' + list[i].Qty;
                }
                else
                    ordersizes = list[i].Size + ' - ' + list[i].Qty;
            }
        }
        return ordersizes;
    },

    gridWorkOrderDataBound: function () {

        $(".k-grid-EditItem").find("span").addClass("k-icon k-edit");
        $(".k-grid-DeleteItem").find("span").addClass("k-icon k-delete");

        var grid = $("#grdWorkOrderDetail").data("kendoGrid");
        var gridData = grid.dataSource.view();
        var flag = false;
        for (var i = 0; i < gridData.length; i++) {
            var row = grid.table.find("tr[data-uid='" + gridData[i].uid + "']");

            if (gridData[i].ErrorStatus) {
                row.addClass("highlighted-row");
            }
            else {
                row.removeClass("highlighted-row");
            }
            if (gridData[i].SizeQty) {
                row.find('.sizeDisp').html(WO.getSizeDisplay(WO.const.SKUSizeList));
            }
        }

        var toolTipErr = $('.highlighted-row').kendoTooltip({
            filter: ".k-warning",
            content: function (e) {
                var dataItem = $("#grdWorkOrderDetail").data("kendoGrid").dataItem(e.target.closest("tr"));
                var content = dataItem.ErrorMessage;
                if (dataItem.ErrorMessage != null) {
                    var template = kendo.template($("#MultiSKUErrorTemplate").html());
                    return template(content);
                }
            }
        }).data("kendoTooltip");

        //grid.table.find("tr[data-uid='" + gridData[0].uid + "'] td:eq(" + Focusedindex + ")").focus();
        //grid.table.find("tr[data-uid='" + gridData[0].uid + "'] td:eq(" + Focusedindex + ")").addClass('k-state-focused');
        //grid.table.find("tr[data-uid='" + gridData[0].uid + "'] td:eq(" + Focusedindex + ")").attr('id', 'grdWorkOrderDetail_active_cell');
        grid.editCell(grid.table.find("tr[data-uid='" + DataUid + "'] td:eq(" + Focusedindex + ")"));

    },
    onGridChange: function (e) {
        //var grid = $("#grdWorkOrderDetail").data("kendoGrid");
        //var currentSelection = grid.select().parent();
        //if (currentSelection) {
        //    currentTableRow = currentSelection;
        //    var rowData = grid.dataItem(currentSelection);
        //    if (rowData) {
        //        style_cd = rowData.Style;
        //        color_cd = rowData.Color;
        //        attr_cd = rowData.Attribute;
        //        size_cd = rowData.Size;
        //        uom_cd = rowData.Uom;
        //        qty_cd = rowData.Qty;
        //        rowData.IsDirty = true;
        //    }
        //}
    },
    saveDataItem: function (e) {

    },
    gridDetailBond: function (e) {

    },
    editDataItem: function (e) {

    },
    ComputeCumFab: function () {
        WO.CreateOrder.isSave = false;
        WO.updateCumulativeAndFabric();

        return false;
    },

    SelectedSizesList: function (list) {
        $("#SelectedSizes").text("Selected Size(S): " + WO.getSizeDisplay(list));
    },
    SelectedSizesListInline: function (list) {
        var grid = $("#grdWorkOrderDetail").data("kendoGrid");
        var currentSelection = grid.select().parent();
        var selectedItem = grid.dataItem(currentSelection)
        selectedItem ? selectedItem : selectedItem = selectrow;
        if (selectedItem) {
            selectedItem.SizeQty = WO.getSizeDisplay(list);
            //$("#grdWorkOrderDetail").data("kendoGrid").refresh();
        }
    },
    saveWODetail: function () {
        //WO.CreateOrder.isSave = true;
        //WO.updateCumulativeAndFabric();

        WO.CreateOrder.popupValidator.hideMessages($('#VarianceQty'));
        if (WO.CreateOrder.isLoadingCompleted) {
            WO.SaveChangesToGrid();

            WO.validateCreateWO();

        }
        return false;
    },

    SaveChangesToGrid: function () {
        WO.CreateOrder.popupValidator.hideMessages($('#VarianceQty'));
        if (WO.CreateOrder.isLoadingCompleted) {
            if (WO.CreateOrder.popupValidator.validate()) {

                var qtyErr = '';
                if (WO.const.SKUSizeList.length == 0) {
                    ISS.common.showPopUpMessage('Please enter quantity for the required sizes.');
                    return false;
                }
                else {
                    for (var j = 0; j < WO.const.SKUSizeList.length; j++) {
                        if (WO.const.SKUSizeList[j].Qty == 0) {
                            qtyErr = 'Please enter quantity for the size : ' + WO.const.SKUSizeList[j].Size;
                            break;
                        }
                    }
                }

                if (qtyErr != '') {
                    ISS.common.showPopUpMessage(qtyErr);
                    return false;
                }

                if (WO.const.validateHaaValue == true) {
                    if ($("#DemandDriver").data("kendoComboBox").value() == undefined || $("#DemandDriver").data("kendoComboBox").value() == '') {
                        ISS.common.showPopUpMessage('SKU requires demand driver should not be null');
                        return false;
                    }

                    if ($("#PurchaseOrder").val() == undefined || $("#PurchaseOrder").val() == '') {
                        ISS.common.showPopUpMessage('SKU requires a purchase order to be other than NULL');
                        return false;
                    }

                    if ($("#LineItem").val() == undefined || $("#LineItem").val() == '' || $("#LineItem").val() <= 0) {
                        ISS.common.showPopUpMessage('SKU requires line item number to be other than NULL or zero');
                        return false;
                    }
                    if ($("#LineItem").val() > 0) {
                        if ($("#LineItem").val().indexOf('0') == 0) {
                            ISS.common.showPopUpMessage('SKU requires line item number to be without leading zeros');
                            return false;
                        }
                    }
                }

                ISS.common.blockUI(true);

                var grid = $("#grdWorkOrderDetail").data("kendoGrid");

                if ($("#gridMode").val() == 'add') {

                    var obj = WO.AddWODetail(new WorkOrderDetail());

                    var data = grid.dataSource.data();

                    if (data) {
                        data.push(obj);
                    }

                }
                else if ($("#gridMode").val() == 'edit') {
                    var data = WO.CreateOrder.currentRow

                    WO.AddWODetail(data);
                }

                grid.refresh();

                //$("#myDiv").hide();
                WO.CreateOrder.editPopUp.close();

                $("#gridMode").val('');
                WO.const.SKUSizeList = [];
                $("#MultiSizeList").val('');
                $("#VarianceQty").val('0');
                $("#TotalDozens").val('');
                $("#Dozens").val('0');
                $("#GroupId").val('0');
                $("#SelectedSizes").html('');
                var gridSize = $("#grdMultiSKUSize").data("kendoGrid");
                gridSize.dataSource.data([]);
                gridSize.refresh();
                ISS.common.blockUI();
            }
            else {

                ISS.common.notify.error('Please Correct the errors.');

            }
            WO.CreateOrder.isLoadingCompleted = true;
        }
        else {
            ISS.common.notify.error('Please wait, Cumulative and Fabric grids are being populated. ');
        }
        return false;
    },




    /// currWO -- Object passing to server
    //currInstance - refrence of grid row   - applicable for duplicate func
    updateCumulativeAndFabric: function (currWO, callback, currInstance, isDup) {
        var txtPlt = $("#TxtPlant").data("kendoComboBox");
        txtPlt.dataSource.data([]);
        txtPlt.text("");
        txtPlt.value("");
        var mctyp = $("#MacType").data("kendoComboBox");
        mctyp.dataSource.data([]);
        mctyp.text("");
        mctyp.value("");

        if (!currWO)
            currWO = WO.getWODetailObjectGrid(new WorkOrderDetail());
        if (currWO.SellingStyle != '' && currWO.ColorCode != '' && currWO.Attribute != '' && currWO.SizeList.length > 0) {
            //ISS.common.blockUI(true);

            ISS.common.blockUI(true);
            if (WO.CreateOrder.isLoadingCompleted) {
                var postData = { WO: currWO };
                postData = JSON.stringify(postData);
                ISS.common.executeActionAsynchronous("../order/CalculateCumulativeAndFabric", postData, function (stat, resData) {
                    if (stat && resData) {
                        PackCode_cd = resData.dataCum.PackCode;
                        AlternateId_cd = resData.dataCum.AlternateId;
                        CylinderSizes_cd = resData.dataCum.CylinderSizes;
                        cutpath_cd = resData.dataCum.CutPath;
                        Lbs_cd = resData.dataCum.Lbs;
                        SewPlt_cd = resData.dataCum.SewPlt
                        VarianceQty_cd = resData.dataCum.VarianceQty
                        cur_row ? cur_row : cur_row = selectrow;
                        if (cur_row) {
                            cur_row.AlternateId = AlternateId_cd;
                            cur_row.CylinderSizes = CylinderSizes_cd;
                            cur_row.Lbs = Lbs_cd;
                            cur_row.PackCode = PackCode_cd;
                            cur_row.CutPath = cutpath_cd;
                            cur_row.Lbs = Lbs_cd;
                            cur_row.PriorityCode = PriorityCode_cd;
                        }
                        $("#grdWorkOrderDetail").data("kendoGrid").refresh();
                        WO.CreateOrder.isLoadingCompleted = false;
                        var gridCum = $("#grdwrkOrderCumulative").data("kendoGrid");
                        var gridFab = $("#grdwrkOrderFabric").data("kendoGrid");
                        var dataCum = gridCum.dataSource.data();
                        var dataFab = gridFab.dataSource.data();
                        if (!isDup) {
                            // delete all from cumulative using ID
                            for (var i = dataCum.length - 1 ; i >= 0; i--) {
                                dataCum.remove(dataCum[i]);
                            }
                            // delete all from Fabric using ID
                            for (var i = dataFab.length - 1 ; i >= 0; i--) {
                                dataFab.remove(dataFab[i]);
                            }
                        }
                        //Add to Cumulative
                        if (resData.dataCum) {
                            var cumData = resData.dataCum.WOCumulative;
                            for (var i = cumData.length - 1; i >= 0; i--) {
                                WO.changeDetDates(cumData[i]);
                                dataCum.push(cumData[i]);
                            }
                            //Merging and grouping of Cumulative grid
                            //Add to Fabric
                            var fabData = resData.dataCum.WOFabric;
                            for (var i = fabData.length - 1; i >= 0; i--)
                                dataFab.push(fabData[i]);
                            var AltId = resData.dataCum.AlternateId;
                            if (resData.dataCum.SewPlt != $('#frmWoEditor #SewPlt').val()) {
                                $("#SewPlt").val(resData.dataCum.SewPlt).change();
                            }
                            $("#AlternateId").val(AltId).change();
                            var Cylsize = resData.dataCum.CylinderSizes;
                            $("#CylinderSizes").val(Cylsize).change();
                            var Lbs = resData.dataCum.Lbs;
                            $("#Lbs").val(Lbs).change();
                            // $("#Revision").val(resData.dataCum.Revision).change();
                            if (currInstance) {
                                currInstance.Lbs = Lbs;
                                currInstance.CylinderSizes = Cylsize;
                                currInstance.AlternateId = AltId;
                            }
                            $("#VarianceQty").val(resData.dataCum.VarianceQty).change();
                            //Add to textiles
                            var texData = resData.dataCum.WOTextiles;
                            //var dataSource = [];
                            //var ds = $("#TxtPlant").data("kendoComboBox").dataSource;
                            //var dsmac = $("#MacType").data("kendoComboBox").dataSource;
                            //var datasourcemac = [];
                            var limit = 0;

                            var tplant = '';
                            //txtPlt.dataSource.data([]);
                            //mctyp.dataSource.data([]);
                            if (resData.dataCum.TxtPath != null) {
                                if (resData.dataCum.WOTextiles != null) {
                                    WO.CreateOrder.plantMachineList = resData.dataCum.WOTextiles;
                                    for (var i = 0; i < resData.dataCum.WOTextiles.length; i++) {
                                        if (resData.dataCum.WOTextiles[i].TxtPlant != null) {
                                            if (tplant != resData.dataCum.WOTextiles[i].TxtPlant) {
                                                tplant = resData.dataCum.WOTextiles[i].TxtPlant;
                                                txtPlt.dataSource.add({
                                                    Text: resData.dataCum.WOTextiles[i].TxtPlant, Value: resData.dataCum.WOTextiles[i].TxtPlant
                                                });
                                            }
                                        }
                                    }
                                }
                                //txtPlt.dataSource.add({ Text: resData.dataCum.TxtPath, Value: resData.dataCum.TxtPath });
                                txtPlt.select(0);
                                WO.filterMachineType();
                            }
                            //if (resData.dataCum.MachineType != null) {
                            //    if (resData.dataCum.WOTextiles != null) {
                            //        for (var i = 0 ; i < resData.dataCum.WOTextiles.length; i++) {
                            //            if (resData.dataCum.WOTextiles[i].MacType != null) {
                            //                mctyp.dataSource.add({ Text: resData.dataCum.WOTextiles[i].MacType, Value: resData.dataCum.WOTextiles[i].MacType });
                            //            }
                            //        }
                            //    }
                            //}
                            if (texData.length > 0) {
                                for (var i = texData.length - 1; i >= 0; i--) {
                                    if (texData[i].TxtPlant != null) {

                                        // ds.add({ Text: texData[i].TxtPlant, Value: texData[i].TxtPlant });
                                        limit = texData[i].Limit;
                                    }
                                    //if (texData[i].MacType != null) {

                                    // dsmac.add({ Text: texData[i].MacType, Value: texData[i].MacType });
                                    // }
                                }
                            }



                            $('#Limit').val(limit).change();
                            if (resData.dataCum.CreateBDInd == "Y") {
                                cur_row ? cur_row : cur_row = selectrow;
                                if (cur_row) {
                                    Dozens_cd = false;
                                    Crea_cd = true;
                                    cur_row.CreateBd = true;
                                    cur_row.DozensOnly = false;
                                }
                                $("#grdWorkOrderDetail").data("kendoGrid").refresh();
                                $('#CreateBd').prop('checked', true);
                                $('#DozensOnly').prop('checked', false);
                                if (currInstance) {
                                    currInstance.CreateBd == true;
                                    currInstance.DozensOnly == false;
                                }
                            }
                            else {
                                cur_row ? cur_row : cur_row = selectrow;
                                if (cur_row) {
                                    Dozens_cd = true;
                                    Crea_cd = false;
                                    cur_row.CreateBd = false;
                                    cur_row.DozensOnly = true;
                                }
                                $("#grdWorkOrderDetail").data("kendoGrid").refresh();
                                $('#CreateBd').prop('checked', false);
                                $('#DozensOnly').prop('checked', true);

                                if (currInstance) {
                                    currInstance.CreateBd == false;
                                    currInstance.DozensOnly == true;
                                }
                            }
                            var variance = resData.dataCum.Variance;
                            WO.updateVariance(variance);
                            $('#CutPath').val(resData.dataCum.CutPath).change();
                            if (currInstance) {
                                currInstance.CutPath = resData.dataCum.CutPath;
                            }
                        }

                        gridCum.refresh();
                        gridFab.refresh();
                        WO.CreateOrder.isLoadingCompleted = true;
                        if (WO.CreateOrder.isSave) {
                            WO.SaveChangesToGrid();
                        }
                        ISS.common.blockUI();
                        if (callback) callback();


                    }
                    else {
                        // failed to retrieve
                    }

                });
            }
            //ISS.common.blockUI();
        }
        //else {
        //    ISS.common.showPopUpMessage('Please enter valid Style, Color, Attribute and Size');
        //}

        return false;
    },



    filterMachineType: function () {
        var lst = WO.CreateOrder.plantMachineList;
        if (lst != null) {
            var txtPlt = $("#TxtPlant").data("kendoComboBox").value();
            if (txtPlt != null && txtPlt != "") {
                var mctyp = $("#MacType").data("kendoComboBox");
                mctyp.dataSource.data([]);
                mctyp.text("");
                mctyp.value("");
                for (var i = 0 ; i < WO.CreateOrder.plantMachineList.length; i++) {
                    if (WO.CreateOrder.plantMachineList[i].TxtPlant == txtPlt) {
                        if (WO.CreateOrder.plantMachineList[i].MacType != null) {
                            mctyp.dataSource.add({ Text: WO.CreateOrder.plantMachineList[i].MacType, Value: WO.CreateOrder.plantMachineList[i].MacType });
                        }

                        if (WO.CreateOrder.isLoadingCompleted) {
                            if (WO.CreateOrder.isUpdateCumOnChange) {
                                WO.updateCumulativeAndFabric();
                            }
                        }
                    }
                }
                mctyp.select(0);
            }
        }
        return false;
    },

    groupCumulativeGrid: function () {
        var gridCum = $("#grdwrkOrderCumulative").data("kendoGrid");
        var dataCum = gridCum.dataSource.data();
        var postData = { WOC: dataCum };
        postData = JSON.stringify(postData);
        for (var i = dataCum.length - 1 ; i >= 0; i--) {

            dataCum.remove(dataCum[i]);

        }
        ISS.common.executeActionAsynchronous("../order/GroupCumulativeGrid", postData, function (stat, resData) {
            if (stat && resData) {

                if (resData.dataCum) {

                    var cumData = resData.dataCum;
                    for (var i = cumData.length - 1 ; i >= 0; i--)
                        dataCum.push(cumData[i]);

                }
            }
        });

    },
    getWODetailObject: function (obj) {
        var gridCum = $("#grdwrkOrderCumulative").data("kendoGrid");
        var gridFab = $("#grdwrkOrderFabric").data("kendoGrid");
        var gridWODet = $("#grdWorkOrderDetail").data("kendoGrid");
        var dataCum = gridCum.dataSource.data();
        var dataFab = gridFab.dataSource.data();
        var dataWODet = gridWODet.dataSource.data();
        var colors = $("#ColorCode").data("kendoComboBox").text().toUpperCase();
        var color = WO.getMultiselectItem(colors);
        obj["Id"] = $("#OrderDetailId").val();

        if ($('#CreateBd').prop('checked')) {
            obj["CreateBDInd"] = "Y";
            obj["DozensOnlyInd"] = "N";
        }
        if ($('#DozensOnly').prop('checked')) {
            obj["DozensOnlyInd"] = "Y";
            obj["CreateBDInd"] = "N";
        }

        obj["DozensOnly"] = $('#DozensOnly').val();

        obj["SellingStyle"] = $('#SellingStyle').val();
        obj["ColorCode"] = color;
        obj["ColorDyeCode"] = color;
        obj["AttributeCompCode"] = $("#Attribute").data("kendoComboBox").text().toUpperCase();
        obj["Attribute"] = $("#Attribute").data("kendoComboBox").text().toUpperCase();
        obj["Revision"] = $("#Revision").val();
        obj["PKGStyle"] = $("#PKGStyle").val();
        obj["MfgPathId"] = $("#MfgPathId").val();
        obj["SewPlt"] = $("#SewPlt").val();
        obj["PackCode"] = $("#PackCode").val();
        obj["NewStyle"] = $("#NewStyle").val();
        obj["NewColor"] = $("#NewColor").val();
        obj["NewSize"] = $("#NewSize").val();
        obj["NewAttribute"] = $("#NewAttribute").val();
        obj["AlternateId"] = $("#AlternateId").val();
        obj["CylinderSizes"] = $("#CylinderSizes").val();
        obj["GroupId"] = $("#GroupId").val();
        obj["SizeList"] = WO.const.SKUSizeList;
        obj["TotalDozens"] = $("#TotalDozens").val();
        obj["Dozens"] = $("#Dozens").val();
        obj["Lbs"] = $("#Lbs").val();
        obj["Note"] = $("#Note").val();
        obj["WOCumulative"] = dataCum;
        obj["WOFabric"] = dataFab;
        obj["OrderCount"] = $("#OrdersToCreate").data("kendoNumericTextBox").value();
        obj["DueDate"] = $("#DueDate").data("kendoDropDownList").value();
        obj["PlannedDate"] = $("#PlannedDate").val();
        obj["PriorityCode"] = $("#PriorityCode").val();
        obj["WODetail"] = dataWODet;
        obj["GridMode"] = $("#gridMode").val();
        obj["BodyTrim"] = $("#BodyTrim").val();
        obj["CutPath"] = $("#CutPath").val();
        obj["AttributionPath"] = $("#AttributionPath").val();
        obj["CategoryCode"] = $('#CategoryCode').val();
        obj["ErrorMessage"] = '';
        obj["ErrorStatus"] = false;
        obj["LimitLbs"] = $("#Limit").val();
        obj["VarianceQty"] = $("#VarianceQty").val();
        obj["ActualLbs"] = $("#VarianceQty").val();
        obj["PrimaryDC"] = $("#PrimaryDC").val();
        obj["AssortCode"] = $("#AsrtCode").val();

        return obj;

    },
    AddWODetail: function (obj) {

        var colors = $("#ColorCode").data("kendoComboBox").text().toUpperCase();
        var color = WO.getMultiselectItem(colors);

        obj["Id"] = $("#OrderDetailId").val();
        obj["CreateBd"] = $('#CreateBd').prop('checked');
        obj["DozensOnly"] = $('#DozensOnly').prop('checked');
        obj["SellingStyle"] = $('#SellingStyle').val();
        obj["ColorCode"] = color;
        obj["Attribute"] = $("#Attribute").data("kendoComboBox").text().toUpperCase();
        obj["Revision"] = $("#Revision").val();
        obj["PKGStyle"] = $("#PKGStyle").val();
        obj["MfgPathId"] = $("#MfgPathId").val();
        obj["SewPlt"] = $("#SewPlt").val();
        obj["PackCode"] = $("#PackCode").val();
        obj["AlternateId"] = $("#AlternateId").val();
        obj["CylinderSizes"] = $("#CylinderSizes").val();
        obj["GroupId"] = $("#GroupId").val();
        obj["SizeList"] = WO.const.SKUSizeList;

        obj["TotalDozens"] = $("#TotalDozens").val();
        obj["Dozens"] = $("#Dozens").val();
        obj["Lbs"] = $("#Lbs").val();
        obj["Note"] = $("#Note").val();
        obj["PriorityCode"] = $("#PriorityCode").val();
        obj["BodyTrim"] = $("#BodyTrim").val();
        obj["CutPath"] = $("#CutPath").val();
        obj["AttributionPath"] = $("#AttributionPath").val();
        obj["CategoryCode"] = $('#CategoryCode').val();
        obj["ErrorMessage"] = '';
        obj["ErrorStatus"] = false;
        obj["LimitLbs"] = $("#Limit").val();
        obj["VarianceQty"] = $("#VarianceQty").val();
        obj["ActualLbs"] = $("#VarianceQty").val();
        obj["AssortCode"] = $("#AsrtCode").val();
        obj["PrimaryDC"] = $("#PrimaryDC").val();

        if (WO.const.validateHaaValue == true) {
            obj["PurchaseOrder"] = $("#PurchaseOrder").val();
            obj["LineItem"] = $("#LineItem").val();
            obj["DemandDriver"] = $("#DemandDriver").data("kendoComboBox").value();
        }

        return obj;

    },


    fillWODetail: function (row) {


        $('#SellingStyle').val(row.SellingStyle).change();
        // var colorCode = $('#ColorCode').data("kendoMultiSelect");
        //colorCode.dataSource.data( WO.loadColor());
        WO.loadColor(row.ColorCode);
        $('#ColorCode').data("kendoComboBox").text(row.ColorCode);
        var attribute = $('#Attribute').data("kendoComboBox");

        attribute.dataSource.read();
        attribute.value(row.Attribute);
        attribute.text(row.Attribute);
        $("#Revision").val(row.Revision).change();
        $("#PKGStyle").val(row.PKGStyle);
        $("#MfgPathId").val(row.MfgPathId);
        $("#SewPlt").val(row.SewPlt);
        $("#PackCode").val(row.PackCode);
        $("#AlternateId").val(row.AlternateId);
        $("#CylinderSizes").val(row.CylinderSizes);
        $("#OrderDetailId").val(row.Id);
        $("#MultiSizeList").val(JSON.stringify(row.SizeList));
        $("#GroupId").val(row.GroupId);
        $("#TotalDozens").val(row.TotalDozens);
        $("#Dozens").val(row.Dozens);
        $("#Lbs").val(row.Lbs);
        $("#Note").val(row.Note);
        $("#PriorityCode").val(row.PriorityCode);
        $("#CutPath").val(row.CutPath);
        $("#BodyTrim").val(row.BodyTrim);
        $("#AttributionPath").val(row.AttributionPath);
        $('#CategoryCode').val(row.CategoryCode);
        $("#VarianceQty").val(row.ActualLbs).change();
        $("#PrimaryDC").val(row.PrimaryDC);
        $("#AsrtCode").val(row.AssortCode);

        WO.const.SKUSizeList = row.SizeList;
        var selectedSize = "";
        for (var i = 0; i < WO.const.SKUSizeList.length; i++) {
            selectedSize += WO.const.SKUSizeList[i].Size + " - " + WO.const.SKUSizeList[i].Qty + ", ";
        }
        selectedSize = selectedSize.slice(0, -2);
        $("#SelectedSizes").html("Selected Size(s): " + selectedSize);
        return false;


    },

    getMultiselectItem: function (items) {
        return items;
    },

    edit: function (e) {
        var input = e.container.find("input");
        setTimeout(function () {
            input.select();
        }, 25);

    },

};





function WorkOrderDetail(Id, WOCumulative, WOFabric, OrdersToCreate, PriorityCode, GridMode, SellingStyle, NewSize, NewAttribute, AlternateId, CylinderSizes, CreateBd, DozensOnly, ColorCode, Attribute, SizeShortDes, Revision, PKGStyle, MfgPathId, SewPlt, PackCode, NewStyle, Lbs, Note, NewColor, GroupId, SizeList, TotalDozens, Dozens, PlannedDate, DueDate, CutPath, BodyTrim, AttributionPath, CategoryCode, LimitLbs, ActualLbs, VarianceQty, AssortCode, PrimaryDC) {
    this.Id = Id ? Id : '';
    this.CreateBd = CreateBd ? CreateBd : '';
    this.DozensOnly = DozensOnly ? DozensOnly : '';
    this.SellingStyle = SellingStyle ? SellingStyle : '';
    this.ColorCode = ColorCode ? ColorCode : '';
    this.Attribute = Attribute ? Attribute : '';
    this.SizeShortDes = SizeShortDes ? SizeShortDes : '';
    this.Revision = Revision ? Revision : '';
    this.PKGStyle = PKGStyle ? PKGStyle : '';
    this.MfgPathId = MfgPathId;
    this.SewPlt = SewPlt;
    this.PackCode = PackCode;
    this.NewStyle = NewStyle;
    this.NewColor = NewColor;
    this.NewSize = NewSize;
    this.NewAttribute = NewAttribute;
    this.AlternateId = AlternateId;
    this.CylinderSizes = CylinderSizes;
    this.GroupId = GroupId;
    this.SizeList = SizeList;
    this.TotalDozens = TotalDozens;
    this.Dozens = Dozens;
    this.Lbs = Lbs;
    this.Note = Note;
    this.WOCumulative = WOCumulative;
    this.WOFabric = WOFabric;
    this.OrdersToCreate = OrdersToCreate;
    this.PlannedDate = PlannedDate;
    this.DueDate = DueDate;
    this.PriorityCode = PriorityCode;
    this.CutPath = CutPath;
    this.GridMode = GridMode;
    this.BodyTrim = BodyTrim;
    this.AttributionPath = AttributionPath;
    this.CategoryCode = CategoryCode;
    this.ErrorStatus = false;
    this.LimitLbs = LimitLbs;
    this.ActualLbs = ActualLbs;
    this.VarianceQty = VarianceQty;
    this.AssortCode = AssortCode;
    this.PrimaryDC = PrimaryDC;
};
