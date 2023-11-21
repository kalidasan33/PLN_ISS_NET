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
        sellValidator: null,
        mfgPathValidator:null,
        popupValidator:null,
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
        Week: null,
        Year: null,
        Date: null,
        mfgLoaded: false,
        url:null,
        PkgVal:null,
    },

    addInputClass: function (frm) {
        $(frm + ' input:not(:checkbox):not(:button):not(:reset):not(:submit)').addClass('InputF');
    },

    doCreateWOReady: function (IsLoad) {
        WO.addInputClass('#frmWoEditor');
        ISS.common.toUpperCase('.InputF:not(.excludeF)');
        $('#ColorCode').addClass('InputF');
        WO.CreateOrder.validator = $('#frmWO').kendoValidator().data("kendoValidator");
        WO.CreateOrder.popupValidator = $('#frmWoEditor').kendoValidator().data("kendoValidator");
        $('#SellingStyle').bind('focusout', WO.StyleChanged);
        $('#SellingStyle').bind('propertychange change keyup paste input', WO.multiSizeGridClear);
        $('#Revision').bind('click', WO.revSearchClick);
        $('#MfgPathId').bind('click', WO.showMfgPathId);
        $('#PackCode').bind('click', WO.PackCodeClick);
        $('#CategoryCode').bind('click', WO.CatCodeClick);
        $('#PKGStyle').bind('focusout', WO.PKGChanged);
        $('#btnWorkOrderClear').bind('click', WO.ClearOrders);
        $('#btnWorkOrderReCalc').bind('click', WO.ReCalcOrders);
        $('#Limit').bind('focusout', WO.LimitChanged);
        //$('#PlannedWeek').bind('change', WO.changeYear);     
        //$('#PlannedYear').bind('change', WO.changeYear);      
        $('#MfgPathId').bind('focusout', WO.loadMfgPathDetails);
        $('#MfgPathId').bind('focusin', WO.loadMFGPathDtls);
        WO.CreateOrder.mfgPathValidator = $('#frmWoEditor #MfgPathId').kendoValidator().data("kendoValidator");
        
        $("#AlternateId").focusin(function () {
            WO.CreateOrder.AltIdVal = $('#frmWoEditor #AlternateId').val();
        });
        $("#MfgPathId").focusin(function () {
            WO.CreateOrder.mfgPathVal = $('#frmWoEditor #MfgPathId').val();
            WO.CreateOrder.mfgPathValidator.hideMessages();
        });
        $("#PKGStyle").focusin(function () {
            WO.CreateOrder.PkgVal = $('#frmWoEditor #PKGStyle').val();
            
        });
        
        $("#frmWoEditor #ColorCode").keyup(function (e) {
            var str = $(this).value();
            $("#frmWoEditor #ColorCode").value(str.toUpperCase())
        });

        $('#Revision').keypress(function (e) {
            if (e.which == 13) {
                WO.revSearchClick();
                return false;
            }
        });

        $('#MfgPathId').keypress(function (e) {
                if (e.which == 13) {
                    WO.showMfgPathId();
                return false;
            }

        });

        $('#PackCode').on('keypress', function (e) {
            if (e.keyCode == 13) {
                WO.PackCodeClick();
                return false;
            }
        });

        $("#SellingStyle").focusin(function () {
            WO.CreateOrder.StyleVal = $('#frmWoEditor #SellingStyle').val();
        });
        //$("#SellingStyle").focusout(function () {
        //    WO.CreateOrder.sellValidator = $('#frmWoEditor #SellingStyle').kendoValidator().data("kendoValidator");
        //});
        $('#frmWoEditor .k-grid-cancel-changes').on('keydown', function (e) {
            if (e.which == 9) {
                $('#frmWoEditor #SellingStyle').focus();
                return false;

            }
        });
        $('#grdwrkOrderDetail #SelectAll').bind('change',function () {
            WO.SelectAllDetails($(this).prop('checked'));
        });
        $("#grdwrkOrderDetail").on('change', '.chckbx', function () {
            setTimeout(function () {
                var gridDetail = $("#grdwrkOrderDetail").data("kendoGrid");
                $("#SelectAll").prop('checked', $('.chckbx:checked').length == gridDetail.dataSource.view().length);
            });
        });

        $("#MfgPathId").focusout(function () {
            WO.CreateOrder.mfgPathValidator = $('#frmWoEditor #MfgPathId').kendoValidator().data("kendoValidator");
        });

        $("#DozenStr").focusout(function (e) {
            if (WO.CreateOrder.popupValidator.validateInput($('#DozenStr'))) {
                //WO.onQtyCodeChanged($("#DozenStr").val(), true);
                //WO.DupliQtyChanged(this);
            }
        });

        WO.CreateOrder.Week = $("#PlannedWeek").val();
        WO.CreateOrder.Year = $("#PlannedYear").val();
        WO.CreateOrder.Date = $("#PlannedDate").val();

        ISS.common.eachesQtyRestriction('.DozenEaches');
        
    },
    SelectAllDetails: function (isSelected) {

        var grid = $('#grdwrkOrderDetail').data('kendoGrid');
        grid.table.find('.chckbx').prop('checked', isSelected);
    },
    replaceAndCheck: function (s) {
        s = $.trim(s.replace("-", "."));
        if (s.indexOf('.')==0) s = '0' + s;
        var arr = s.split('.');
        if (arr.length > 1 && arr[1].length > 2 ) {
            s=arr[0]+'.'+arr[1].substring(0,2)
        }
        if (s != "" && !s.match(/^\d+(\.\d{1,2})?$/)) {
            s = arr[0];
        }
        return s;
    },
    getEaches: function (val) {
        var intPart = parseInt(val);
        var decimalPart = val - intPart;
        var result = Math.round(((decimalPart) * 100));
        return parseInt(result);
    },
    onSizetoQtyEnter: function (e) {
        //********************
        var grid = $("#grdwrkOrderDetail").data("kendoGrid");
        WO.const.SKUSizeList = [];
        var s = e.value.toString();
        s = WO.replaceAndCheck(s);
        if (!$.isNumeric(s)) {
            var currentSelection = grid.select().parent();
            var selectedItem = grid.dataItem(currentSelection)
            selectedItem ? selectedItem : selectedItem = selectrow;
            Focusedindex = 12;
            ISS.common.showPopUpMessage('Please enter valid number.', null, function () {
                if (selectedItem) {
                    size_qty = "0-00";
                    selectedItem.DozenStr = "0-00";
                    selectedItem.MfgPathId = MfgPathId_cd
                    selectedItem.GarmentSKU = Garment_cd;
                    selectedItem.AttributionPath = SewPlt_cd;
                    WO.const.SKUSizeList = [{ Size: size_desc, SizeCD: size_cd, Qty: size_qty }];
                    DataUid = selectedItem.uid;
                }
                grid.refresh();
                return false;
            });
        }
        if (WO.getEaches(s) == 10) {
            var currentSelection = grid.select().parent();
            var selectedItem = grid.dataItem(currentSelection)
            selectedItem ? selectedItem : selectedItem = selectrow;
            Focusedindex = 12;
            if (selectedItem) {
                if (s.indexOf('.') == -1) s += '.';
                while (s.length < s.indexOf('.') + 3) s += '0';
                size_qty = s;
                selectedItem.DozenStr = s;
                selectedItem.MfgPathId = MfgPathId_cd
                selectedItem.GarmentSKU = Garment_cd;
                selectedItem.AttributionPath = SewPlt_cd;
                grid.table.find("tr[data-uid='" + selectedItem.uid + "'] td:eq(9)")[0].innerText = MfgPathId_cd;

                grid.table.find("tr[data-uid='" + selectedItem.uid + "'] td:eq(10)")[0].innerText = Garment_cd;

                grid.table.find("tr[data-uid='" + selectedItem.uid + "'] td:eq(11)")[0].innerText = SewPlt_cd;
                WO.const.SKUSizeList = [{ Size: size_desc, SizeCD: size_cd, Qty: size_qty }];
                DataUid = selectedItem.uid;
            }
           
            
        }
        else if (WO.getEaches(s) >= 12) {
            //ISS.common.showPopUpMessage("Eaches must be less than 12");
            //var grid = $("#grdwrkOrderDetail").data("kendoGrid");
            var currentSelection = grid.select().parent();
            var selectedItem = grid.dataItem(currentSelection)
            selectedItem ? selectedItem : selectedItem = selectrow;
            Focusedindex = 12;
            ISS.common.showPopUpMessage('Eaches must be less than 12.', null, function () {
                if (selectedItem) {
                    size_qty = parseInt(s) + "-00";;
                    selectedItem.DozenStr = parseInt(s) + "-00";
                    selectedItem.MfgPathId = MfgPathId_cd
                    selectedItem.GarmentSKU = Garment_cd;
                    selectedItem.AttributionPath = SewPlt_cd;
                    grid.table.find("tr[data-uid='" + selectedItem.uid + "'] td:eq(9)")[0].innerText = MfgPathId_cd;
                    grid.table.find("tr[data-uid='" + selectedItem.uid + "'] td:eq(10)")[0].innerText = Garment_cd;
                    grid.table.find("tr[data-uid='" + selectedItem.uid + "'] td:eq(11)")[0].innerText = SewPlt_cd;
                    WO.const.SKUSizeList = [{ Size: size_desc, SizeCD: size_cd, Qty: size_qty }];
                    DataUid = selectedItem.uid;
                }
                grid.refresh();
                return false;
            });
           
        }
        else {
            //var grid = $("#grdwrkOrderDetail").data("kendoGrid");
            var currentSelection = grid.select().parent();
            var selectedItem = grid.dataItem(currentSelection)
            selectedItem ? selectedItem : selectedItem = selectrow;
            Focusedindex = 12;
            if (selectedItem) {
                //size_qty = s;
                //selectedItem.DozenStr = s;
                var d = WO.getEaches(s);
                if (d) {
                    size_qty = s.replace(".", "-");
                    selectedItem.DozenStr = s.replace(".", "-");
                }
                else {
                    size_qty = parseInt(s) + "-00";
                    selectedItem.DozenStr = parseInt(s) + "-00";
                }
                selectedItem.MfgPathId = MfgPathId_cd
                selectedItem.GarmentSKU = Garment_cd;
                selectedItem.AttributionPath = SewPlt_cd;
                grid.table.find("tr[data-uid='" + selectedItem.uid + "'] td:eq(9)")[0].innerText = MfgPathId_cd;
                grid.table.find("tr[data-uid='" + selectedItem.uid + "'] td:eq(10)")[0].innerText = Garment_cd;
                grid.table.find("tr[data-uid='" + selectedItem.uid + "'] td:eq(11)")[0].innerText = SewPlt_cd;
                WO.const.SKUSizeList = [{ Size: size_desc, SizeCD: size_cd, Qty: size_qty }];
                DataUid = selectedItem.uid;
                grid.editCell(grid.table.find("tr[data-uid='" + DataUid + "'] td:eq(" + Focusedindex + ")"));
            }
            
        }
        
    },
   
    onChangePFS: function (e) {
        var grid = $('#grdwrkOrderFabric').data("kendoGrid");
        var dataItem = grid.dataItem($(e.currentTarget).closest("tr"));
        dataItem.PullFromStock = $(e.currentTarget).prop('checked');
        var currWO = WO.getWODetailObject(new WorkOrderDetail());
        currWO.WOFabric = new Array()
        currWO.WOFabric.push(dataItem);
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
                //$("#Lbs").val(resData.dataCum.Lbs).change();

                //$("#VarianceQty").val(resData.dataCum.VarianceQty).change();
            }
            ISS.common.blockUI(false);
        });
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
        WO.CreateOrder.PKCode = ISS.common.popUp('.divPackCodePopup', 'Pack Code', null, function (rr) {
            if (rr.userTriggered) {
                rr._defaultPrevented = true;
                WO.CreateOrder.PKCode.close();
                $('#PackCode').focus();
            }
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
        var grid = $("#grdwrkOrderDetail").data("kendoGrid");
        var selectedItem = grid.dataItem(currentTableRow);
        selectedItem ? selectedItem : selectedItem = selectrow;
        PackCode_cd = dataItem.PackCode;
        $('#PackCode').val(dataItem.PackCode);
        selectedItem.PackCode = dataItem.PackCode;
        grid.refresh();
        //if (WO.CreateOrder.isUpdateCumOnChange) {
        //    WO.updateCumulativeAndFabric();
        //}
        WO.CreateOrder.PKCode.close();
        Focusedindex = 13;
        var grid = $("#grdwrkOrderDetail").data("kendoGrid");
        var currentSelection = grid.select().parent();
        if (currentSelection) {
            currentTableRow = currentSelection;
            var rowData = grid.dataItem(currentSelection);
            rowData ? rowData : rowData = selectrow;
            if (rowData) {
                DataUid = rowData.uid;
            }
        }
        return false;
    },
    CatCodeClick: function () {
        WO.loadCatCodeGrid();
        WO.CreateOrder.CatCode = ISS.common.popUp('.divCatCodePopup', 'Category Code', null, function (rr) {
            if (rr.userTriggered) {
                rr._defaultPrevented = true;
                WO.CreateOrder.CatCode.close();
                $('#CategoryCode').focus();
            }
        });
    },
    loadCatCodeGrid: function () {
        var grid = $("#grdCatCode").data("kendoGrid");
        if (grid.pager.page() > 1) grid.pager.page(1)
        grid.dataSource.read();
        return false;
    },

    showCatCodeDetails: function (e) {
        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        $('#CategoryCode').val(dataItem.CategoryCode);
        //if (WO.CreateOrder.isUpdateCumOnChange) {
        //    WO.updateCumulativeAndFabric();
        //}
        WO.CreateOrder.CatCode.close();
        return false;
    },
    revSearchClick: function () {
        WO.MultiSizeData();
        var ColorCode= $('#ColorCode').data("kendoComboBox").value();
        //var colors = $('#ColorCode').data("kendoMultiSelect").value();
        //var ColorCode = WO.getMultiselectItem(colors);
        var Attribute= $('#Attribute').data("kendoComboBox").text();
        var SizeList= WO.const.SKUSizeList;
        if ($('#SellingStyle').val() != '' && ColorCode != '' && Attribute != '' && SizeList.length > 0) {
            WO.loadRevDetailsGrid();
            WO.CreateOrder.rev = ISS.common.popUp('.divRevSearchPopup', 'Revision Search', null, function (rr) {
                if (rr.userTriggered) {
                    rr._defaultPrevented = true;
                    WO.CreateOrder.rev.close();
                    $('#Revision').focus();
                }
            });
            
            return false;
        }
        else {
            ISS.common.showPopUpMessage('Please enter Style, Color, Attribute and Size to generate revision.');
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
        WO.MultiSizeData();
        //var colors = $('#ColorCode').data("kendoMultiSelect").value();
        var colors = $('#ColorCode').data("kendoComboBox").value();
        //ColorCode: WO.getMultiselectItem(colors)
        var searchCriteria = {
            SellingStyle: $('#SellingStyle').val(),
            ColorCode: colors,
            Attribute: $('#Attribute').data("kendoComboBox").text(),
            SizeList: JSON.parse(JSON.stringify( WO.const.SKUSizeList)),
            Revision: $('#Revision').val(),
            AssortCode: $('#AsrtCode').val()
        };
        
        
        return searchCriteria;
    },

    showDetails: function (e) {

        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        $('#PKGStyle').val(dataItem.PKGStyle);
        $('#Revision').val(dataItem.NewRevision);
        WO.CreateOrder.popupValidator.hideMessages($('#Revision'));
        //if (WO.CreateOrder.isUpdateCumOnChange) {
        //    WO.updateCumulativeAndFabric();
        //}
        WO.getChildSKU();
        WO.CreateOrder.rev.close();
        $('#Revision').focus();
        return false;
    },

    showmfgPathDetails: function (e) {

        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));

        $('#MfgPathId').val(dataItem.MfgPathId);
        $('#AttributionPath').val(dataItem.SewPlt);
        WO.CreateOrder.popupValidator.hideMessages($('#MfgPathId'));
        //if (WO.CreateOrder.isUpdateCumOnChange) {
        //    WO.updateCumulativeAndFabric();
        //}
        WO.getGarmentSKU();
        WO.CreateOrder.mfgPath.close();
        $('#MfgPathId').focus();
        return false;
    },

    loadMFGPathDtls: function (e) {
        WO.CreateOrder.mfgPathVal = $('#frmWoEditor #MfgPathId').val();
        WO.CreateOrder.mfgLoaded = false;
        //if (WO.CreateOrder.mfgPathVal != $('#frmWoEditor #MfgPathId').val()) {
        //    $('#frmWoEditor #SewPlt').val('');
           
            
        //}

        return false;
    },

    loadAltIdDtls: function (e) {
        if (WO.CreateOrder.isUpdateCumOnChange) {
            WO.updateCumulativeAndFabric();
        }

        return false;
    },

    StyleChanged: function () {
        if (WO.CreateOrder.popupValidator.validateInput($('#SellingStyle'))) {
            WO.CreateOrder.popupValidator.validateInput($('#frmWoEditor #SellingStyle'));
            //WO.CreateOrder.popupValidator.hideMessages($('#Revision'));
            //WO.CreateOrder.popupValidator.hideMessages($('#frmWoEditor #MfgPathId'));
            //WO.CreateOrder.popupValidator.hideMessages($('#frmWoEditor #PKGStyle'));
            if (WO.CreateOrder.StyleVal != $('#frmWoEditor #SellingStyle').val()) {

                var ds = $("#ColorCode").data("kendoComboBox");
                $("#Attribute").data("kendoComboBox").setDataSource();
                $("#Attribute").data("kendoComboBox").text('');
                $("#Size").data("kendoComboBox").text('');
                //ds.dataSource.read();
                WO.loadColor();
                //var ds = $("#ColorCode").data("kendoMultiSelect");

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
                $('#SewPlt').val('');
                $("#AlternateId").val('');
                $("#CutPath").val('');
                $("#SelectedSizes").html('');
                $("#GarmentSKU").val('');
                var postData = { SellingStyle: $('#SellingStyle').val() };
                $("#PKGStyle").val($('#SellingStyle').val());
                postData = JSON.stringify(postData);

                ISS.common.executeActionAsynchronous(WO.const.urlAsrtCode, postData, function (stat, data) {
                    if (stat) {


                        if (data.length > 0) {
                            $("#AsrtCode").val(data[0].AssortCode);
                            $("#PrimaryDC").val(data[0].PrimaryDC);
                            $("#PackCode").val(data[0].PackCode).change();
                            //Commented as DC for AO's needs to be populated after Attribute Changes
                           // $("#Dc").val(data[0].PrimaryDC).change();
                            $("#OriginTypeCode").val(data[0].OriginTypeCode);
                            $("#BusinessUnit").val(data[0].CorpBusUnit);
                            $("#ProdFamCode").val(data[0].ProdFamCode);
                            //if (WO.CreateOrder.isUpdateCumOnChange) {
                            //    WO.updateCumulativeAndFabric();
                            //}

                        }

                    }

                    else {

                        ISS.common.showPopUpMessage('Please provide a valid style.');
                    }

                });
            }
        }
        return false;
    },


    retrieveColorData: function () {
        return {
            SellingStyle: $('#SellingStyle').val()
        };
    },
    retrieveColorDataIn: function () {
        return {
            SellingStyle: style_cd
        };
    },

    onColorChange: function () {
        //var attribute = $("#Attribute").data("kendoComboBox");
        //attribute.dataSource.read();
        if (ISS.common.validateComboChange(this, 'Invalid Color')) {
            WO.loadAttr();
            WO.const.SKUSizeList = [];
            $("#MultiSizeList").val('');
            var ds = $("#Size").data("kendoComboBox");
            ds.dataSource.read();
            //var gridSize = $("#grdMultiSKUSize").data("kendoGrid");
            //gridSize.dataSource.data([]);
            //gridSize.refresh();
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
            //if (WO.CreateOrder.isUpdateCumOnChange) {
            //    WO.updateCumulativeAndFabric();
            //}
        }
        return false;
    },

    loadAttr: function (v) {
        $("#Size").data("kendoComboBox").text('');
        $("#Size").data("kendoComboBox").value('');
        var postDataAttrib = WO.retrieveAttributeData();
        postDataAttrib = JSON.stringify(postDataAttrib);
        ISS.common.executeActionAsynchronous("../../order/GetAttributeCodes", postDataAttrib, function (stat, data) {
            if (stat) {
                var attrib = $("#Attribute").data('kendoComboBox');
                attrib.dataSource.data(data);
                if (data.length > 0 && attrib.value() == '') {
                    attrib.value(data[0].Attribute);
                    $("#Size").data("kendoComboBox").text('');
                    WO.loadSize(v);
                }
                else if (!ISS.common.validateComboChange(attrib)) {
                    attrib.value('');
                    $("#Size").data("kendoComboBox").text('');
                }
                else {
                    WO.loadSize(v);
                }
            }
        });
       
    },
    isNumber: function (evt) {
        var qty = evt.value;
        var index = -1;
        if (qty != null && qty != "") {
            index = qty.indexOf('.');
        }
        var charCode = (evt.which) ? evt.which : event.keyCode
        if (
            ((charCode != 46 || index != -1) &&
            (charCode < 48 || charCode > 57))) {
            if (event.preventDefault) event.preventDefault();
            else {
                if (event.returnValue) event.returnValue = false;
                return false;
            }

        }

        return true;
    },
    loadSize: function (v) {

        var postDataAttrib = WO.retrieveSizeData();
        postDataAttrib = JSON.stringify(postDataAttrib);
        ISS.common.executeActionAsynchronous("../../order/GetSizes", postDataAttrib, function (stat, data) {
            if (stat) {
                if (data) {
                    var size = $("#Size").data('kendoComboBox');
                    size.dataSource.data(data);
                    if (v) {
                        size.value(v);
                        if (size.select() == -1) {
                            size.open()
                            size.close()
                        }
                    }
                    if (data.length > 0 && size.value() == '') {                         
                            size.value(data[0].Size);
                            if (size.select() == -1) {
                                size.open()
                                size.close()
                            }
                         
                    }
                    else if (!ISS.common.validateComboChange(size)) {
                        size.value(null);                        
                    }
                    size.trigger('change');
                }
            }
        });

    },
    retrieveAOColorData: function () {
        return {
            SellingStyle: style_cd
        };
    },
    retrieveAttributeData: function () {
        var result = WO.retrieveColorData();        
        var color = $('#ColorCode').data("kendoComboBox").value();       
        result.ColorCode = color;
        result.Src = "AO";
        return result;
    },
    retrieveAOAttributeData: function () {
        var result = WO.retrieveColorDataIn();
        var color = color_cd;
        result.ColorCode = color;
        result.Src = "AO";
        return result;
    },

    LimitChanged: function(){
        var currWO = WO.getWODetailObject(new WorkOrderDetail());
        var postData = { WO: currWO };
        postData = JSON.stringify(postData);
        //ISS.common.executeActionAsynchronous("../order/CalculateVariance", postData, function (stat, resData) {
        //    if (stat && resData) {
        //        if (resData.dataCum) {
        //            var variance = resData.dataCum.Variance;
        //            WO.updateVariance(variance);
                    
        //        }
        //    }
        //    else {

        //    }
        //});
            return false;
    },

    updateVariance :function(variance)
    {
        //if (variance > 0) {
        //    $('#Variance').val('+' + variance).change().removeClass("fontgreen fontblue fontred").addClass("fontgreen");

            
        //}
        //else if (variance < 0) {
        //    $('#Variance').val(variance).change().removeClass("fontgreen fontblue fontred").addClass("fontred");

            
        //}
        //else {
        //    $('#Variance').val(variance).change().removeClass("fontgreen fontblue fontred").addClass("fontblue");

            
        //}
       

    },
    onAttributeChange: function () {
        if (ISS.common.validateComboChange(this, 'Invalid Attribute')) {
            WO.const.SKUSizeList = [];
            $("#MultiSizeList").val('');
            //var gridSize = $("#grdMultiSKUSize").data("kendoGrid");
            //gridSize.dataSource.data([]);
            //gridSize.refresh();
            //var gridCat = $("#grdCatCode").data("kendoGrid");
            //gridCat.dataSource.data([]);
            //gridCat.refresh();
            //var gridMFG = $("#grdmfPathDetails").data("kendoGrid");
            //gridMFG.dataSource.data([]);
            //gridMFG.refresh();
            //var gridPack = $("#grdPackCode").data("kendoGrid");
            //gridPack.dataSource.data([]);
            //gridPack.refresh();
            //var gridRev = $("#grdRevDetails").data("kendoGrid");
            //gridRev.dataSource.data([]);
            //gridRev.refresh();
            $('#Revision').val('');
            $("#MfgPathId").val('');
            $("#SelectedSizes").html('');
            WO.loadSize();
        }
     	 return false;
    },
    onOrdersToCreateChange :function()
    {
        //var gridCum = $("#grdwrkOrderCumulative").data("kendoGrid");
        //var dataCum = gridCum.dataSource.data();
        //if(dataCum.length>0)
        //{

        var ordersToCreate = $("#OrdersToCreate").data("kendoNumericTextBox");
        if (ordersToCreate.value() > 999) {
            ISS.common.notify.error('Number of orders to create should be less than 1000');
            ordersToCreate.value(1);
            return false;
        }
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
           
        //}
        return false;
    },
    retrieveSizeData: function () {
        var result = WO.retrieveColorData();
        //var ColorCode = $("#ColorCode").data("kendoMultiSelect");
        var ColorCode = $('#ColorCode').data("kendoComboBox").value();
        var Attribute = $("#Attribute").data("kendoComboBox");
        result.ColorCode = ColorCode;
        result.Attribute = Attribute.text();
        return result;
    },
    retrieveAOSizeData: function () {
        var result = WO.retrieveColorDataIn();
        //var ColorCode = $("#ColorCode").data("kendoMultiSelect");
        var ColorCode = color_cd;
        var Attribute = attr_cd;
        result.ColorCode = ColorCode;
        result.Attribute = Attribute;
        return result;
    },
  
    onSizeChange: function () {
   
        if (ISS.common.validateComboChange(this, 'Invalid Size')) {
            WO.CreateOrder.popupValidator.hideMessages($('#Revision'));
            var postData = WO.retrieveColorData();
            //var colors = $("#ColorCode").data("kendoMultiSelect").value();
            //var ColorCode = WO.getMultiselectItem(colors);
            var ColorCode = $('#ColorCode').data("kendoComboBox").value();
            var Attribute = $("#Attribute").data("kendoComboBox");
            WO.MultiSizeData();
           
            if (ColorCode != '' && Attribute.text() != '' && WO.const.SKUSizeList.length > 0) {
              
                    postData.ColorCode = ColorCode;
                    postData.Attribute = Attribute.text();

                    postData.SizeList = WO.const.SKUSizeList;
                    postData.AssortCode = $("#AsrtCode").val();
                    //if ($.trim($("#Revision").val()) == '') {
                        pData = JSON.stringify(postData);
                        ISS.common.executeActionAsynchronous(WO.const.urlGetRevision, pData, function (stat, data) {
                            if (stat) {
                                if (data.length > 0) {
                                    $("#Revision").val(data[0].Revision).change();
                                    WO.getChildSKU();
                                }
                            }
                            else {
                                ISS.common.showPopUpMessage('Failed to retrieve Revision details.');
                            }
                        });
                    //} //end rev null check
                    postData.MfgPathId = $('#MfgPathId').val();
                    if (postData.MfgPathId != '') {
                        WO.validateGarmentSKU(postData, function (gv) {
                            $('#GarmentSKU').val(gv)
                        });
                    }
                
            }
        }
        return false;
    },
    getPKGStyle: function(callback){
        WO.CreateOrder.popupValidator.hideMessages($('#PKGStyle'));
        var postData = WO.retrieveColorData();
        WO.MultiSizeData();
        //var colors = $("#ColorCode").data("kendoMultiSelect").value();
        //var ColorCode = WO.getMultiselectItem(colors);
        var ColorCode = $('#ColorCode').data("kendoComboBox").value();
        var Attribute = $("#Attribute").data("kendoComboBox");
        if (ColorCode != '' && Attribute.text() != '' && WO.const.SKUSizeList.length > 0) {
            postData.ColorCode = ColorCode;
            postData.Attribute = Attribute.text();
            postData.SizeList = WO.const.SKUSizeList;
            postData.AssortCode = $("#AsrtCode").val();
            postData.Revision = $("#Revision").val();
            postData = JSON.stringify(postData);
            ISS.common.executeActionAsynchronous(WO.const.urlGetPKGStyle, postData, function (stat, data) {
                if (stat && data) {


                    if (data.length > 0) {

                        $("#PKGStyle").val(data[0].PKGStyle).change();
                    }

                }

                else {
                    ISS.common.showPopUpMessage('Failed to Load PKG Style.');
                }
                if(callback)callback(data.length > 0)
            });
        }
    },
    
    getChildSKUInline: function () {

        var postData = WO.retrieveColorDataIn();
        var ColorCode = color_cd;
        var attr = attr_cd;

        WO.MultiSizeData();
        postData.ColorCode = ColorCode;
        postData.Attribute = attr;
        postData.OrginTypeCode = $("#OriginTypeCode").val();

        postData.SizeList = WO.const.SKUSizeList;
        $("#AsrtCode").val(asort_cd);
        postData.AssortCode = asort_cd;
        postData.Revision = rev_cd;
        postData = JSON.stringify(postData);
        ISS.common.executeActionAsynchronous(WO.const.urlGetChildSKU, postData, function (stat, data) {
            if (stat) {
                if (data.length > 0) {
                    //$("#NewStyle").val(data[0].NewStyle);
                    //$("#PKGStyle").val(data[0].NewStyle);
                    //$("#NewColor").val(data[0].NewColor);
                    //$("#NewAttribute").val(data[0].NewAttribute);
                    //$("#NewSize").val(data[0].NewSize);
                    $("#NewStyle").val(data[0].NewStyle);
                    $("#PKGStyle").val(data[0].NewStyle);
                    cur_row ? cur_row : cur_row = selectrow;
                    cur_row.PKGStyle = data[0].NewStyle;
                    pkgstyle = data[0].NewStyle;
                    $("#NewColor").val(data[0].NewColor);
                    $("#NewAttribute").val(data[0].NewAttribute);
                    $("#NewSize").val(data[0].NewSize);
                    $("#grdwrkOrderDetail").data("kendoGrid").refresh();
                    WO.updateStyleInfoInline();
                }
            }
            else {
                ISS.common.showPopUpMessage('Failed to retrieve Style details.');
            }

        });
        return false;
    },
    getChildSKU: function () {

        var postData = WO.retrieveColorData();
        var ColorCode = $('#ColorCode').data("kendoComboBox").value();
        var attr = $("#Attribute").data("kendoComboBox");
     
        WO.MultiSizeData();
        postData.ColorCode = ColorCode;
        postData.Attribute = attr.text();
        postData.OrginTypeCode = $("#OriginTypeCode").val();
       
        postData.SizeList = WO.const.SKUSizeList;
        postData.AssortCode = $("#AsrtCode").val();
        postData.Revision = $("#Revision").val();
        postData = JSON.stringify(postData);
        ISS.common.executeActionAsynchronous(WO.const.urlGetChildSKU, postData, function (stat, data) {
            if (stat) {
                if (data.length > 0) {
                    $("#NewStyle").val(data[0].NewStyle);
                    $("#PKGStyle").val(data[0].NewStyle);
                    $("#NewColor").val(data[0].NewColor);
                    $("#NewAttribute").val(data[0].NewAttribute);
                    $("#NewSize").val(data[0].NewSize);
                    WO.updateStyleInfo();
                }
            }
            else {
                ISS.common.showPopUpMessage('Failed to retrieve Style details.');
            }

        });
        return false;
    },
    updateStyleInfoInline: function () {

        var postData = { SellingStyle: $('#NewStyle').val() };
        postData = JSON.stringify(postData);

        ISS.common.executeActionAsynchronous(WO.const.urlAsrtCode, postData, function (stat, data) {
            if (stat) {


                if (data.length > 0) {
                    $("#AsrtCode").val(data[0].AssortCode);
                    $("#PrimaryDC").val(data[0].PrimaryDC);
                    $("#PackCode").val(data[0].PackCode).change();
                    if (data[0].PrimaryDC != null){
                        $("#Dc").val(data[0].PrimaryDC).change();
                        $("#Dc").val(data[0].PrimaryDC);
                    }
                    $("#OriginTypeCode").val(data[0].OriginTypeCode);

                }

            }

            else {
                ISS.common.showPopUpMessage('Failed to retrieve Style details.');
            }

        });
        return false;
    },
    updateStyleInfo: function () {

        var postData = { SellingStyle: $('#NewStyle').val() };
        postData = JSON.stringify(postData);

        ISS.common.executeActionAsynchronous(WO.const.urlAsrtCode, postData, function (stat, data) {
            if (stat) {


                if (data.length > 0) {
                    $("#AsrtCode").val(data[0].AssortCode);
                    $("#PrimaryDC").val(data[0].PrimaryDC);
                    $("#PackCode").val(data[0].PackCode).change();
                    if (data[0].PrimaryDC != null)
                        $("#Dc").val(data[0].PrimaryDC).change();
                    $("#OriginTypeCode").val(data[0].OriginTypeCode);

                }

            }

            else {
                ISS.common.showPopUpMessage('Failed to retrieve Style details.');
            }

        });
        return false;
    },
    retrieveRevisionData: function () {
        var result = WO.retrieveColorData();
        //var colors = $("#ColorCode").data("kendoMultiSelect").value();
        //var color = WO.getMultiselectItem(colors);
        var ColorCode = $('#ColorCode').data("kendoComboBox").value();
        var attr = $("#Attribute").data("kendoComboBox");
        var size = $("#SizeShortDes").data("kendoComboBox");
        result.ColoCode = ColorCode;
        result.AttributeCode = attr.text();
        result.SizeCode = size.value();
        return result;
    },

    onRevisionSelect: function () {
        var ds = $("#Revision").data("kendoDropDownList");
        ds.dataSource.read();
        //if (WO.CreateOrder.isUpdateCumOnChange) {
        //    WO.updateCumulativeAndFabric();
        //}
        return false;
    },

    onRevisionChange: function () {
       
    },
    retrievePKGData: function () {
        var result = WO.retrieveColorData();
        //var colors = $("#ColorCode").data("kendoMultiSelect").value();
        //var color = WO.getMultiselectItem(colors);
        var color = $('#ColorCode').data("kendoComboBox").value();
        var attr = $("#Attribute").data("kendoComboBox");
        WO.MultiSizeData();
        var revision = $("#Revision").val();
        var asrtCode = $("#AsrtCode").val();
        result.ColorCode = color;
        result.Attribute = attr.text();
        result.SizeList = JSON.parse(JSON.stringify(WO.const.SKUSizeList));
        result.Revision = revision;
        result.AssortCode = asrtCode;
        return result;
     
    },

    onPackageSelect: function () {
       
    },

    PKGChanged: function () {
        if (WO.CreateOrder.PkgVal != $('#frmWoEditor #PKGStyle').val() && $('#frmWoEditor #PKGStyle').val() != $('#frmWoEditor #SellingStyle').val()) {
            var result = WO.retrievePKGData();
            result.PKGStyle = $('#frmWoEditor #PKGStyle').val();
            var postData = JSON.stringify(result);
            ISS.common.executeActionAsynchronous(WO.const.urlValidatePKGStyle, postData, function (stat, data) {
                if(stat)
                {
                    if(data)
                    {
                        if (data.ErrMsg && data.ErrMsg != '') {
                            ISS.common.notify.error(data.ErrMsg);
                        }
                        $('#frmWoEditor #PKGStyle').val('');
                        $('#frmWoEditor #PKGStyle').focus();
                        WO.CreateOrder.mfgPathValidator.hideMessages();
                    }
                }

            });
        }
        return false;
    },

    retrieveMFGData: function () {
        var result = WO.retrieveColorData();
        //var colors = $("#ColorCode").data("kendoMultiSelect").value();
        //var color = WO.getMultiselectItem(colors);
        var color = $('#ColorCode').data("kendoComboBox").value();
        var attr = $("#Attribute").data("kendoComboBox");
        WO.MultiSizeData();
     
        //var demLoc =  $("#Dc").val();
        var demLoc = $("#PrimaryDC").val();
        result.ColorCode = color;
        result.Attribute = attr.text();
        result.SizeList =JSON.parse(JSON.stringify( WO.const.SKUSizeList));
        result.PrimaryDC = demLoc;
        return result;
    },


    mfPathClick: function () {
        WO.MultiSizeData();
        //var colors = $('#ColorCode').data("kendoMultiSelect").value();
        //ColorCode = WO.getMultiselectItem(colors);
        var ColorCode = $('#ColorCode').data("kendoComboBox").value();
        var Attribute= $('#Attribute').data("kendoComboBox").text();
        var SizeList= WO.const.SKUSizeList;
        if ($('#SellingStyle').val() != '' && ColorCode != '' && Attribute != '' && SizeList.length > 0) {
            WO.loadmfgPathDetailsGrid();
            WO.CreateOrder.mfgPath = ISS.common.popUp('.divmfgPathPopup', 'Select a Sew Plant', null, function (rr) {
                if (rr.userTriggered) {
                    rr._defaultPrevented = true;
                    WO.CreateOrder.mfgPath.close();
                    $('#MfgPathId').focus();
                }
            });
        }
        else {
            ISS.common.showPopUpMessage('Please enter Style, Color, Attribute and Size to generate Mfg Path.');
           
        }
        return false;
    },

    loadmfgPathDetailsGrid: function () {
        var grid = $("#grdmfPathDetails").data("kendoGrid");
        if (grid.pager.page() > 1) grid.pager.page(1)
        grid.dataSource.read();
        return false;
    },


    changeYear: function (e,callback) {
       
        var Year = $("#PlannedYear").val();
        var Week = $("#PlannedWeek").val();
        var dueDate = "";
        var postData = { Week: $("#PlannedWeek").val(), Year: $("#PlannedYear").val(), dueDate: "" };
        postData = JSON.stringify(postData);
        ISS.common.executeActionAsynchronous(WO.const.urlChangeYear, postData, function (stat, data) {
            if (stat) {
                if (data) {
                    
                    if (data.ErrMsg && data.ErrMsg != '') {
                        ISS.common.notify.error(data.ErrMsg);
                        
                        if (callback) callback(false);
                    }
                    else {
                        $("#PlannedDate").val(data.result);
                        if (callback) callback(true);
                    }
                    if (!callback) {
                        $("#PlannedDate").val(data.result);
                        if (data.PlannedWeek) {
                            $("#PlannedWeek").data("kendoNumericTextBox").value(data.PlannedWeek);
                        }
                        if (data.PlannedYear) {
                            $("#PlannedYear").data("kendoNumericTextBox").value(data.PlannedYear);                           
                        }
                    }
                }
            }
            else {
                ISS.common.notify.error('Failed to retrieve Planned Date.');
            }
            if (callback) callback(false);
        });

     
        return false;
    },
    searchDataWO: function () {
        return ISS.common.getFormData($('#frmWO'));
    },
    showWOEditor: function (e) {
        $("#gridMode").val('add');
        WO.clearWOdetail();
        $("#gridMode").val('add');
        
        var orderId = WO.getRandomId(1, 999);
        $("#OrderDetailId").val(orderId);

        var dsAttr = $("#Attribute").data("kendoComboBox");
        dsAttr.dataSource.read();
       
        //if ($("#Dozens").val() == '0')
        //    $("#Dozens").val('');

        WO.CreateOrder.editPopUp = ISS.common.popUp('#myDiv', 'Add', null, function (e) {
            
            if (e.userTriggered) {
                e._defaultPrevented = true;
                ISS.common.showConfirmMessage('Pending changes will be lost.<br/> Do you want to continue by losing your changes?', null, function (reply) {
                    if (reply) {

                        WO.CreateOrder.editPopUp.close();
                       
                    }

                });
            }
        });
        WO.CreateOrder.currentRow = null;
        $("#SellingStyle").focus();
        return false;
    },
    showWOEditorInLine: function (e) {
        $("#gridMode").val('add');
        WO.clearWOdetail();
        //$("#gridMode").val('add');

        var orderId = WO.getRandomId(1, 999);
        $("#OrderDetailId").val(orderId);
        id_cd = $("#OrderDetailId").val();
        //var dsAttr = $("#Attribute").data("kendoComboBox");
        //dsAttr.dataSource.read();

        //if ($("#Dozens").val() == '0')
        //    $("#Dozens").val('');

        //WO.CreateOrder.editPopUp = ISS.common.popUp('#myDiv', 'Add', null, function (e) {

        //    if (e.userTriggered) {
        //        e._defaultPrevented = true;
        //        ISS.common.showConfirmMessage('Pending changes will be lost.<br/> Do you want to continue by losing your changes?', null, function (reply) {
        //            if (reply) {

        //                WO.CreateOrder.editPopUp.close();

        //            }

        //        });
        //    }
        //});
        //WO.CreateOrder.currentRow = null;
        //$("#SellingStyle").focus();
        //return false;
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
        var gridWODet = $("#grdwrkOrderDetail").data("kendoGrid");
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
                    //$('#Variance').val(resData.dataCum.Variance).change();
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

    ReCalcOrders:function()
    {  
            ISS.common.showConfirmMessage('Are your Sure you want to Re-Calculate Order Details.?<br/>Press "Yes" to continue', null, function (reply) {
                if (reply) {
                    WO.RecalcWO();
                }
            });
            
            return false; 
        
    },
    cancelWODetail: function (isdelete) {

        WO.CreateOrder.validator.hideMessages();
        if (!isdelete) {
            ISS.common.showConfirmMessage('Pending changes will be lost.<br/> Do you want to continue by losing your changes?', null, function (reply) {
                if (reply) {
                    WO.CreateOrder.editPopUp.close();
                    //WO.ResetCumFabData();
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
        //var gridCum = $("#grdwrkOrderCumulative").data("kendoGrid");
        //var gridFab = $("#grdwrkOrderFabric").data("kendoGrid");
        //var gridWODet = $("#grdwrkOrderDetail").data("kendoGrid");
        //var dataWODet = gridWODet.dataSource.data();
        //var dataCum = gridCum.dataSource.data();
        //var dataFab = gridFab.dataSource.data();
        ISS.common.blockUI(true);
        var currWO = WO.getWODetailObject(new WorkOrderDetail());
        var postData = { WO: currWO };
        postData = JSON.stringify(postData);
        ISS.common.executeActionAsynchronous(WO.const.urlCancelAWO, postData, function (stat, resData) {
            if (stat && resData) {
                $("#gridMode").val('delete');
                // delete all from cumulative using ID
                //for (var i = dataCum.length - 1 ; i >= 0; i--) {
                //    dataCum.remove(dataCum[i]);
                //}
                //// delete all from Fabric using ID
                //for (var i = dataFab.length - 1 ; i >= 0; i--) {
                //    dataFab.remove(dataFab[i]);
                //}
                //Add to Cumulative
                if (resData.dataCum) {
                    //var cumData = resData.dataCum.WOCumulative;
                    //if (cumData!=null) {
                    //    for (var i = cumData.length - 1 ; i >= 0; i--)
                    //        dataCum.push(cumData[i]);
                    //}
                    ////Merging and grouping of Cumulative grid
                    ////Add to Fabric
                    //var fabData = resData.dataCum.WOFabric;
                    //if (fabData!=null) {
                    //    for (var i = fabData.length - 1; i >= 0; i--)
                    //        dataFab.push(fabData[i]);
                    //}
                    //var AltId = resData.dataCum.AlternateId;
                    //$("#AlternateId").val(AltId).change();
                    //var Cylsize = resData.dataCum.CylinderSizes;
                    //$("#CylinderSizes").val(Cylsize).change();
                    //var Lbs = resData.dataCum.Lbs;
                    //$("#Lbs").val(Lbs).change();
                    //Add to textiles
                    //var texData = resData.dataCum.WOTextiles;
                    //var dataSource = [];
                    //var ds = $("#TxtPlant").data("kendoComboBox").dataSource;
                    //var dsmac = $("#MacType").data("kendoComboBox").dataSource;
                    //var datasourcemac = [];
                    //var limit = 0;
                    //if (texData != null) {
                    //    for (var i = texData.length - 1; i >= 0; i--) {
                    //        if (texData[i].TxtPlant != null) {
                    //            ds.add({ Text: texData[i].TxtPlant, Value: texData[i].TxtPlant });
                    //            limit = texData[i].Limit;
                    //        }
                    //        if (texData[i].MacType != null)
                    //            dsmac.add({ Text: texData[i].MacType, Value: texData[i].MacType });
                    //    }
                    //}

                    var wodtl = [];
                    for (var i = resData.dataCum.WODetail.length - 1 ; i >= 0; i--) {
                        if (resData.dataCum.Id != resData.dataCum.WODetail[i].Id)
                            wodtl.push(resData.dataCum.WODetail[i]);
                    }
                    if (wodtl.length > 0) {
                        if (wodtl[0].PrimaryDC != null && wodtl[0].PrimaryDC != undefined && wodtl[0].PrimaryDC != "")
                            $("#Dc").val(wodtl[0].PrimaryDC).change();
                        //$('#frmWO #DC').val(dataWODet[0].PrimaryDC).change();
                    }
                    else {
                        //WO.ClearHeaderInfo();
                        WO.ClearWOPage();
                    }

                    //if (resData.dataCum == null)
                    //{
                    //    WO.ClearHeaderInfo();
                    //}
                    //else {
                        
                    //}
                    //$('#Limit').val(limit).change();
                    //$('#VarianceQty').val('');
                    //if (resData.dataCum.CreateBDInd == "Y") {
                    //    $('#CreateBd').prop('checked', true);
                    //    $('#DozensOnly').prop('checked', false);
                    //}
                    //else {
                    //    $('#CreateBd').prop('checked', false);
                    //    $('#DozensOnly').prop('checked', true);
                    //}
                    //$('#Variance').val(resData.dataCum.Variance).change();
                    //$('#CutPath').val(resData.dataCum.CutPath).change();
                }

                //gridCum.refresh();
                //gridFab.refresh();
                ISS.common.blockUI(false);
                return false;
            }
            else {
                ISS.common.blockUI(false);
            }
        });
        WO.clearWOdetail();
        // $("#myDiv").hide();
        if(! close) WO.CreateOrder.editPopUp.close();
        //ISS.common.blockUI();
        $("#gridMode").val('');
    },
    ClearHeaderInfo: function(){
        
            //var txtPlant = $("#TxtPlant").data("kendoComboBox");
            //txtPlant.dataSource.data([]);
            //txtPlant.text("");
            //txtPlant.value("");
            //var macType = $("#MacType").data("kendoComboBox");
            //macType.dataSource.data([]);
            //macType.text("");
            //macType.value("");
            WO.CreateOrder.plantMachineList = null;

            $("#Dc").val('');
            $("#OrderDetailId").val('');
            //$("#Limit").val('');
            //$("#Variance").val('');
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

        
    },
    ClearWOPage: function () {
        var gridWoD = $("#grdwrkOrderDetail").data("kendoGrid");
        //var gridCum = $("#grdwrkOrderCumulative").data("kendoGrid");
        //var gridFab = $("#grdwrkOrderFabric").data("kendoGrid");
        gridWoD.dataSource.data([]);
        //gridCum.dataSource.data([]);
        //gridFab.dataSource.data([]);
        WO.clearWOdetail();
        
        $("#Dc").val('');
        $("#OrderDetailId").val('');
        var PlannerCode = $("#PlannerCd").data("kendoDropDownList");
        PlannerCode.text("-Select-");
        PlannerCode.value(" ");
        var ordersToCreate = $("#OrdersToCreate").data("kendoNumericTextBox");
        //ordersToCreate.text(1);
        ordersToCreate.value(1);

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
        //$("#Limit").val('');
        //$("#Variance").val('');
        WO.CreateOrder.plantMachineList = null;
        return false;
    },
    ClearOrders: function()
    {
        
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
        var gridWoD = $("#grdwrkOrderDetail").data("kendoGrid");
       var wodData= gridWoD.dataSource.data();
       
        $('#SellingStyle').val('');
        //var ColorCode = $("#ColorCode").data("kendoMultiSelect");
        var ColorCode = $('#ColorCode').data("kendoComboBox");
        ColorCode.dataSource.data([]);
         ColorCode.text("");
        ColorCode.value("");
        var Attribute = $("#Attribute").data("kendoComboBox");
        Attribute.dataSource.data([]);
       Attribute.text("");
       Attribute.value("");
       var Sz = $('#Size').data("kendoComboBox");
       Sz.dataSource.data([]);
       Sz.text("");
       Sz.value("");
       
       if (wodData.length == 0 ) {
           //var txtPlant = $("#TxtPlant").data("kendoComboBox");
           //txtPlant.dataSource.data([]);
           //txtPlant.text("");
           //txtPlant.value("");
           //var macType = $("#MacType").data("kendoComboBox");
           //macType.dataSource.data([]);
           //macType.text("");
           //macType.value("");
           WO.CreateOrder.plantMachineList = null;
           $("#Dc").data("kendoComboBox").value("");
           $("#Dc").val('');
           $("#OrderDetailId").val('');
           //$("#Limit").val('');
           //$("#Variance").val('');
           var ordersToCreate = $("#OrdersToCreate").data("kendoNumericTextBox");
           //ordersToCreate.text(1);
           ordersToCreate.value(1);
           var PlannerCode = $("#PlannerCd").data("kendoDropDownList");
           PlannerCode.text("-Select-");
           PlannerCode.value(" ");
       
       }
       //$("#Dc").val('');
       //$("#OrderDetailId").val('');
       //var ordersToCreate = $("#OrdersToCreate").data("kendoDropDownList");
       //ordersToCreate.text(1);
       //ordersToCreate.value(1);
       //var PlannerCode = $("#PlannerCd").data("kendoDropDownList");
       //PlannerCode.text("-Select-");
       //PlannerCode.value(" ");

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
        //$("#TotalDozens").val('0');
        //$("#Dozens").val('0');
        $("#DozenStr").val('');
        //$("#Lbs").val('0');
        $('#AttributionPath').val('');
        $("#CategoryCode").val('');
        $("#BodyTrim").val('');
        $("#PriorityCode").val(50);
        $("#GroupId").val('0'); 
        $("#Note").val('');
        $("#SelectedSizes").html('');
        $("#GarmentSKU").val('');
        $("#GStyle").val('');
        $("#GColor").val('');
        $("#GAttribute").val('');
        $("#GSize").val('');
        $("#BulkNumber").val('');
      
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
       
       
        //var gridCum = $("#grdwrkOrderCumulative").data("kendoGrid");
        //var gridFab = $("#grdwrkOrderFabric").data("kendoGrid");
        //WO.CreateOrder.editDataCum = gridCum.dataSource.data();
        //WO.CreateOrder.editDataFab = gridFab.dataSource.data();
        WO.CreateOrder.currentRow = data;
        //if (data.CreateBd) {
        //    $("#DozensOnly").prop('checked', false);
        //    $('#CreateBd').prop('checked', true);
        //}
        //else {
        //    $("#DozensOnly").prop('checked', true);
        //    $('#CreateBd').prop('checked', false);
        //}
        //$("#myDiv").show();
        WO.CreateOrder.editPopUp = ISS.common.popUp('#myDiv', 'Edit', null, function (e) {
            //if(e.userTriggered)
            //    WO.ResetCumFabData( true);
            if (e.userTriggered) {
                e._defaultPrevented = true;
                ISS.common.showConfirmMessage('Pending changes will be lost.<br/> Do you want to continue by losing your changes?', null, function (reply) {
                    if (reply) {

                        WO.CreateOrder.editPopUp.close();
                        //WO.ResetCumFabData(true);
                    }

                });
            }
        });
        $("#SellingStyle").focus();
        return false;

    },



    deleteOrderDetail: function (e) {
      //  if ($("#gridMode").val() == "") {
            ISS.common.showConfirmMessage('Do you want to delete?', null, function (reply) {
                if (reply) {
                    var grid = $("#grdwrkOrderDetail").data("kendoGrid");
                    var data = grid.dataSource.data();
                    var tr = $(e.target).closest("tr");
                    var itemtoRemove = grid.dataItem(tr);

                    $("#gridMode").val('delete');
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
                    ordersizes = ordersizes + ", " + list[i].Size; //+ ' - ' + list[i].Qty;
                }
                else
                    ordersizes = list[i].Size; //+' - '+list[i].Qty ;
            }
        }
        return ordersizes;
    },

    gridAODataBound:function()
    {
        
        $(".k-grid-EditItem").find("span").addClass("k-icon k-edit");
        $(".k-grid-DeleteItem").find("span").addClass("k-icon k-delete");

        var grid = $("#grdwrkOrderDetail").data("kendoGrid");
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
            //row.find('.sizeDisp').html(WO.getSizeDisplay(gridData[i].SizeList));
        }

        $('#SelectAll').uncheck();

        var toolTipErr = $('.highlighted-row').kendoTooltip({
            filter: ".k-warning",
            content: function (e) {
                var dataItem = $("#grdwrkOrderDetail").data("kendoGrid").dataItem(e.target.closest("tr"));
                var content = dataItem.ErrorMessage;
                if (dataItem.ErrorMessage != null) {
                    var template = kendo.template($("#MultiSKUErrorTemplate").html());
                    return template(content);
                }
            }
            }).data("kendoTooltip");
         grid.editCell(grid.table.find("tr[data-uid='" +DataUid + "'] td:eq(" +Focusedindex + ")"));
    },
    ComputeCumFab: function () {
        WO.CreateOrder.isSave = false;
        WO.updateCumulativeAndFabric();
        
        return false;
    },

    SelectedSizesList: function(list)
    {
        $("#SelectedSizes").text("Selected Size(S): " + WO.getSizeDisplay(list));
    },
    saveWODetail: function () {
        //WO.CreateOrder.isSave = true;
        //WO.updateCumulativeAndFabric();
      
            
            if (WO.CreateOrder.isLoadingCompleted) {
                WO.SaveChangesToGrid();

                //WO.validateCreateWO();
            
        }
        return false;
    },

    SaveChangesToGrid: function () {
        //WO.CreateOrder.popupValidator.hideMessages($('#VarianceQty'));
        if (WO.CreateOrder.isLoadingCompleted) {
            if (WO.CreateOrder.popupValidator.validate()) {

               var Sz = $('#Size').data("kendoComboBox");
               if (Sz.value() == "") {
                   ISS.common.showPopUpMessage('Please select Size.');
                   $('#Size').focus();
                   return false;
            }
               var qty = $('#DozenStr').val().replace('-', '.');
               if ($('#DozenStr').val() == "" || qty == 0) {
                   //ISS.common.showPopUpMessage('Please enter quantity.');
                   ISS.common.showPopUpMessage('Please enter quantity.', null, function () {
                       $('#DozenStr').focus();
                   });
                   
                   return false;
               }

               WO.getDCLocations();
                ISS.common.blockUI(true);
               
                var grid = $("#grdwrkOrderDetail").data("kendoGrid");

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
                    if (data.GroupId != null && data.GroupId != "0")
                    {
                        var groups = WO.getAllMSKU(data.GroupId);
                        for (i = 0; i < groups.length; i++) {
                            groups[i].Attribute = data.Attribute;
                            groups[i].MfgPathId = data.MfgPathId;
                            groups[i].Dc = data.Dc;
                            groups[i].AttributionPath = data.AttributionPath;
                        }
                    }
                }

                grid.refresh();

                //$("#myDiv").hide();
                WO.CreateOrder.editPopUp.close();

                $("#gridMode").val('');
                WO.const.SKUSizeList = [];
                $("#MultiSizeList").val('');
                //$("#VarianceQty").val('');
                //$("#TotalDozens").val('0');
                $("#DozenStr").val('');
                $("#GroupId").val('0');
                $("#SelectedSizes").html('');
                //var gridSize = $("#grdMultiSKUSize").data("kendoGrid");
                //gridSize.dataSource.data([]);
                //gridSize.refresh();
                
                Sz.dataSource.data([]);
                Sz.text("");
                Sz.value("");
                
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

    getDCLocations: function () {
        
        var dc = $("#Dc").data("kendoComboBox");
        var currDc = dc.value();
        //dc.dataSource.data([]);
        //dc.text("");
        //dc.value("");
       // if (ISS.common.validateComboChange(this, 'Invalid Color')) {
        var currAO = WO.getWODetailObjectInLine(new WorkOrderDetail());
           var postData = JSON.stringify(currAO);
            ISS.common.executeActionAsynchronous(WO.const.urlDCLocations, postData, function (stat, resData) {
                if (stat && resData) {
                    dc.dataSource.data(resData);
                    if(resData.length>0)
                    {
                        if (currDc) {
                            if (ISS.common.validateComboChange($("#Dc").data("kendoComboBox"), 'Dc ' +currDc+ ' is not valid for the order SKU')) {

                            }
                        }
                        else
                            dc.value(resData[0].DC);
                        
                    }
                }
                else {
                    ISS.common.notify.error('Failed to retrieve DC Location.');
                }
            });
        
       
        return false;
    },
    onDCChange : function(){
        ISS.common.validateComboChange(this, 'Entered DC not available in list.Please select a DC from dropdown');
        
    },
    /// currWO -- Object passing to server
    //currInstance - refrence of grid row   - applicable for duplicate func
    updateCumulativeAndFabric: function (currWO,callback,currInstance) {
        var txtPlt = $("#TxtPlant").data("kendoComboBox");
        txtPlt.dataSource.data([]);
        txtPlt.text("");
        txtPlt.value("");
        var mctyp = $("#MacType").data("kendoComboBox");
        mctyp.dataSource.data([]);
        mctyp.text("");
        mctyp.value("");

        if(! currWO)
         currWO = WO.getWODetailObject(new WorkOrderDetail());
        if (currWO.SellingStyle != '' && currWO.ColorCode != '' && currWO.Attribute != '' && currWO.SizeList.length > 0) {
            //ISS.common.blockUI(true);

            ISS.common.blockUI(true);
            if (WO.CreateOrder.isLoadingCompleted) {
                var postData = { WO: currWO };
                postData = JSON.stringify(postData);
                ISS.common.executeActionAsynchronous("../order/CalculateCumulativeAndFabric", postData, function (stat, resData) {
                    if (stat && resData) {
                        WO.CreateOrder.isLoadingCompleted = false;
                        var gridCum = $("#grdwrkOrderCumulative").data("kendoGrid");
                        var gridFab = $("#grdwrkOrderFabric").data("kendoGrid");
                        var dataCum = gridCum.dataSource.data();
                        var dataFab = gridFab.dataSource.data();
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
                            if (currInstance) {
                                currInstance.Lbs = Lbs;
                                currInstance.CylinderSizes = Cylsize;
                                currInstance.AlternateId = AltId;
                            }
                            //$("#VarianceQty").val(resData.dataCum.VarianceQty).change();
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
                                $('#CreateBd').prop('checked', true);
                                $('#DozensOnly').prop('checked', false);
                                 if (currInstance) {
                                currInstance.CreateBd ==true;
                             currInstance.DozensOnly ==false;
                                          }
                            }
                            else {
                                $('#CreateBd').prop('checked', false);
                                $('#DozensOnly').prop('checked', true);
                             if (currInstance) {
                                currInstance.CreateBd ==false;
                             currInstance.DozensOnly ==true;
                                }
                            }
                            //var variance = resData.dataCum.Variance;
                            //WO.updateVariance(variance);
                            $('#CutPath').val(resData.dataCum.CutPath).change();
                             if (currInstance)
                            {
                            currInstance.CutPath=resData.dataCum.CutPath;
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
                    }
                }
                mctyp.select(0);
            }
        }
        return false;
    },

    groupCumulativeGrid: function ()
    {
        var gridCum = $("#grdwrkOrderCumulative").data("kendoGrid");
        var dataCum = gridCum.dataSource.data();
        var postData = {WOC:dataCum};
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
        WO.MultiSizeData();
        //var gridCum = $("#grdwrkOrderCumulative").data("kendoGrid");
        //var gridFab = $("#grdwrkOrderFabric").data("kendoGrid");
        var gridWODet = $("#grdwrkOrderDetail").data("kendoGrid");
        //var dataCum = gridCum.dataSource.data();
        //var dataFab = gridFab.dataSource.data();
        var dataWODet = gridWODet.dataSource.data();
        var dataCum = null;
        var dataFab = null;
        //var colors = $("#ColorCode").data("kendoMultiSelect").value();
        //var color = WO.getMultiselectItem(colors);
        if ($('#ColorCode').data("kendoComboBox").value())
            var color = $('#ColorCode').data("kendoComboBox").value();
        obj["Id"] = $("#OrderDetailId").val();
        obj["CreateBd"] = $('#CreateBd').val();
        obj["DozensOnly"] = $('#DozensOnly').val();
        obj["SellingStyle"] = $('#SellingStyle').val();
        obj["ColorCode"] = color;
        obj["ColorDyeCode"] = color;
        obj["AttributeCompCode"] = $("#Attribute").data("kendoComboBox").text();
        obj["Attribute"] = $("#Attribute").data("kendoComboBox").text();
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
        obj["TotalDozens"] = "0";
        obj["Dozens"] = $("#DozenStr").val().replace('-', '.');
        obj["Lbs"] = "0";
        obj["Note"] = $("#Note").val();
        obj["WOCumulative"] = dataCum;
        obj["WOFabric"] = dataFab;
        obj["OrderCount"] = $("#OrdersToCreate").data("kendoNumericTextBox").value();
		obj["DueDate"] = "";
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
        obj["GStyle"]= $("#GStyle").val();
        obj["GColor"]= $("#GColor").val();
        obj["GAttribute"]= $("#GAttribute").val();
        obj["GSize"] = $("#GSize").val();
        obj["CorpBusUnit"] = $("#BusinessUnit").val();
        obj["ProdFamCode"] = $("#ProdFamCode").val();
        obj["DozenStr"] = $("#DozenStr").val();
       
            obj["BulkNumber"] = $("#BulkNumber").val();
      
        return obj;

    },

    getWODetailObjectInLine: function (obj) {
        WO.MultiSizeData();
        //var gridCum = $("#grdwrkOrderCumulative").data("kendoGrid");
        //var gridFab = $("#grdwrkOrderFabric").data("kendoGrid");
        var gridWODet = $("#grdwrkOrderDetail").data("kendoGrid");
        //var dataCum = gridCum.dataSource.data();
        //var dataFab = gridFab.dataSource.data();
        var dataWODet = gridWODet.dataSource.data();
        var dataCum = null;
        var dataFab = null;
        //var colors = $("#ColorCode").data("kendoMultiSelect").value();
        //var color = WO.getMultiselectItem(colors);
        var color = color_cd;
        obj["Id"] = $("#OrderDetailId").val();
        obj["CreateBd"] = $('#CreateBd').val();
        obj["DozensOnly"] = $('#DozensOnly').val();
        obj["SellingStyle"] = style_cd;
        obj["ColorCode"] = color;
        obj["ColorDyeCode"] = color;
        obj["AttributeCompCode"] = attr_cd;
        obj["Attribute"]= attr_cd;
        obj["Revision"] = rev_cd;
        obj["PKGStyle"] = pkgstyle;
        obj["MfgPathId"] = MfgPathId_cd;
        obj["SewPlt"] = SewPlt_cd;
        obj["PackCode"] = PackCode_cd;
        obj["NewStyle"] = $("#NewStyle").val();
        obj["NewColor"] = $("#NewColor").val();
        obj["NewSize"] = $("#NewSize").val();
        obj["NewAttribute"] = $("#NewAttribute").val();
        obj["AlternateId"] = $("#AlternateId").val();
        obj["CylinderSizes"] = $("#CylinderSizes").val();
        obj["GroupId"] = $("#GroupId").val();
        obj["SizeList"] = WO.const.SKUSizeList;
        obj["TotalDozens"] = "0";
        //obj["Dozens"] = sum_qty.replace('-', '.');
        obj["Lbs"] = "0";
        obj["Note"]= $("#Note").val();//PriorityCode_cd = 50;PackCode
        obj["WOCumulative"] = dataCum;
        obj["WOFabric"] = dataFab;
        obj["OrderCount"] = $("#OrdersToCreate").data("kendoNumericTextBox").value();
		obj["DueDate"] = "";
		obj["PlannedDate"] = $("#PlannedDate").val();
		obj["PriorityCode"]= PriorityCode_cd;
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
        obj["GStyle"]= $("#GStyle").val();
        obj["GColor"]= $("#GColor").val();
        obj["GAttribute"]= $("#GAttribute").val();
        obj["GSize"] = $("#GSize").val();
        obj["CorpBusUnit"] = $("#BusinessUnit").val();
        obj["ProdFamCode"] = $("#ProdFamCode").val();
        obj["DozenStr"] = $("#DozenStr").val();
       
            obj["BulkNumber"] = $("#BulkNumber").val();
      
        return obj;

    },

    AddWODetail: function (obj) {
        WO.MultiSizeData();
        //var colors = $("#ColorCode").data("kendoMultiSelect").value();
        //var color = WO.getMultiselectItem(colors);
        var color = $('#ColorCode').data("kendoComboBox").value();
       
        obj["Id"] = $("#OrderDetailId").val();
        obj["CreateBd"] = $('#CreateBd').prop('checked');
        obj["DozensOnly"] = $('#DozensOnly').prop('checked');
        obj["SellingStyle"] = $('#SellingStyle').val();
        obj["ColorCode"] = color;
        obj["Attribute"] = $("#Attribute").data("kendoComboBox").text();
        obj["Revision"] = $("#Revision").val();
        obj["PKGStyle"] = $("#PKGStyle").val();
        obj["MfgPathId"] = $("#MfgPathId").val();
        obj["SewPlt"] = $("#SewPlt").val();
        obj["PackCode"] = $("#PackCode").val();
        obj["AlternateId"] = $("#AlternateId").val();
        obj["CylinderSizes"] = $("#CylinderSizes").val();
        obj["GroupId"] = $("#GroupId").val();
        obj["SizeList"] = WO.const.SKUSizeList;
        
        obj["TotalDozens"] = "0";
        //obj["Dozens"] = $("#Dozens").val();
        obj["Dozens"] = $("#DozenStr").val().replace('-', '.');
        obj["Lbs"] = "0";
        obj["Note"] = $("#Note").val();
        obj["PriorityCode"] = $("#PriorityCode").data("kendoNumericTextBox").value();
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
        obj["GarmentSKU"] = $("#GarmentSKU").val();
        obj["PrimaryDC"] = $("#PrimaryDC").val();
        obj["GStyle"]= $("#GStyle").val();
        obj["GColor"]= $("#GColor").val();
        obj["GAttribute"]= $("#GAttribute").val();
        obj["GSize"] = $("#GSize").val();
        obj["CorpBusUnit"] = $("#BusinessUnit").val();
        obj["ProdFamCode"] = $("#ProdFamCode").val();
        obj["DozenStr"] = $("#DozenStr").val();
       
            obj["BulkNumber"] = $("#BulkNumber").val();
       
        return obj;

    },


    fillWODetail: function (row) {
        $('#SellingStyle').val(row.SellingStyle).change();
        var colorCode = $('#ColorCode').data("kendoComboBox");       
        colorCode.value(row.ColorCode);
        colorCode.text(row.ColorCode);
       
        if (row.SizeList.length > 0) {
            var sze = $('#Size').data("kendoComboBox");
            sze.value(row.SizeList[0].SizeCD);
            sze.text(row.SizeList[0].Size);
            WO.loadColor(row.ColorCode, row.SizeList[0].SizeCD);
            if (row.SizeList[0].Qty > 0)
                $("#DozenStr").val(row.SizeList[0].Qty.replace('.', '-'));
                //$("#Dozens").val(row.SizeList[0].Qty);
            else
                $("#DozenStr").val('');
        }
        else {
            WO.loadColor(row.ColorCode);
        }
         
        var attribute = $('#Attribute').data("kendoComboBox");
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
        //$("#TotalDozens").val(row.TotalDozens);
        //$("#Dozens").val(row.Dozens);
        //$("#Lbs").val(row.Lbs);
        $("#Note").val(row.Note);
        $("#PriorityCode").data("kendoNumericTextBox").value(row.PriorityCode);
        $("#CutPath").val(row.CutPath);
        $("#BodyTrim").val(row.BodyTrim);
        $("#AttributionPath").val(row.AttributionPath);
        $('#CategoryCode').val(row.CategoryCode);
        $("#VarianceQty").val(row.ActualLbs).change();
        $("#PrimaryDC").val(row.PrimaryDC);
        $("#GarmentSKU").val(row.GarmentSKU);
        $("#GStyle").val(row.GStyle);
        $("#GColor").val(row.GColor);
        $("#GAttribute").val(row.GAttribute);
        $("#GSize").val(row.GSize);
        $("#BusinessUnit").val(row.CorpBusUnit);
        $("#ProdFamCode").val(row.ProdFamCode);
        $("#BulkNumber").val(row.BulkNumber);
        $("#DozenStr").val(row.DozenStr);
       WO.const.SKUSizeList = row.SizeList;
        //var selectedSize = "";
        //for (var i = 0; i < WO.const.SKUSizeList.length; i++) {
        //    selectedSize += WO.const.SKUSizeList[i].Size + " - " + WO.const.SKUSizeList[i].Qty + ", ";
        //}
        //selectedSize = selectedSize.slice(0, -2);
        //$("#SelectedSizes").html("Selected Size(s): " + selectedSize);
        return false;


    },
    loadColor: function (rowColor,v) {
        var postData = WO.retrieveColorData()
        var color = $("#ColorCode").data("kendoComboBox");
        //$("#Size").data("kendoComboBox").value('');
        //$("#Size").data("kendoComboBox").text('');
        postData = JSON.stringify(postData);
        ISS.common.executeActionAsynchronous(WO.const.urlGetColor, postData, function (stat, data) {
            if (stat) {
                if (data ) {
                    var ds = $('#ColorCode').data().kendoComboBox.dataSource;
                    ds.data(data);
                    //ds.data();
                    if (data.length > 0) {
                        if (rowColor) {
                            color.value(rowColor);
                        }
                        else
                            color.value(data[0].Color);
                    }
                    else {
                        ISS.common.notify.error('Please provide a valid style.');
                    }                  
                    WO.loadAttr(v);
                } 
            }
        });
    },
    DefaultColor: function () {
        var color = $("#ColorCode").data("kendoMultiSelect");
        var data = color.dataSource.data();
        if (color.value == null) {
            if (data.length > 0)
                color.value(data[0].Color);
        }

    },
    getMultiselectItem: function (items) {
        var selColorCode = '';
        if (items.length > 0)
            selColorCode = items[0];
        return selColorCode;
    },
    edit: function (e) {
        var input = e.container.find("input");
        setTimeout(function () {
            input.select();
        }, 25);

    },
   
};



function WorkOrderDetail(Id, WOCumulative, WOFabric, OrdersToCreate, PriorityCode, GridMode, SellingStyle, NewSize, NewAttribute, AlternateId, CylinderSizes, CreateBd, DozensOnly, ColorCode, Attribute, SizeShortDes, Revision, PKGStyle, MfgPathId, SewPlt, PackCode, NewStyle, Lbs, Note, NewColor, GroupId, SizeList, TotalDozens, Dozens, PlannedDate, DueDate, CutPath, BodyTrim, AttributionPath,CategoryCode,LimitLbs,ActualLbs,VarianceQty,AssortCode,PrimaryDC) {
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
    this.TotalDozens = "0";
    this.Dozens = Dozens;
    this.Lbs = Lbs;
    this.Note = Note;
    this.WOCumulative=WOCumulative;
    this.WOFabric = WOFabric;
    this.OrdersToCreate = OrdersToCreate;
	this.PlannedDate = PlannedDate;
	this.DueDate = "";
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
