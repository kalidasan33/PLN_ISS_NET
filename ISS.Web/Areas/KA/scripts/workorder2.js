var style_cd = null;
var color_cd = null;
var attr_cd = null;
var size_cd = null;
var size_desc = null;
var id_cd = null;
var asort_cd = null;
var prod_fam = null;
var corpbus_cd = null
var pkgstyle = null;
var prm_cd = null;
var orgin_cd = null;
var rev_cd = null;
var MfgPathId_cd = null;
var Garment_cd = null;
var SewPlt_cd = null;
var sum_qty = null;
var PackCode_cd = null;
var NewStyle_cd = null;
var NewColor_cd = null;
var NewSize_cd = null;
var NewAttribute_cd = null;
var AlternateId_cd = null;
var CylinderSizes_cd = null;
var GroupId_cd = null;
var SizeList_cd = null;
var TotalDozens_cd = null;
var Dozens_cd = null;
var Lbs_cd = null;
var Note_cd = null;
var WOCumulative_cd = null;
var cutpath_cd = null;
var superO = null;
var cPath = null;
var size_qty = null;
var totalTextQty = null;
var PriorityCode_cd = 50;
var currentTableRow = null;
var currentTableQty = null;
var pkg_inp_style = null;
var line_cd = null;
var cur_row = null;
var Dozens_cd = null;
var Crea_cd = null;
var flag = 0;
var Focusedindex = 0;
var DataUid = 0;
var selectrow = null;
var mfg = null;
var flg = null;
var corct_val = null;

MSKUWO = {

    const: {
        sizePopup: null,
        woCurrentRow: null,
        SKUSizeList: [],
        SizeValidator: null,
        columns: null,
    },

    docWOReady2: function (IsLoad) {

        $('#btnWOSizeSave').bind('click', WO.onMultiSizeSave);

        $('#btnWorkOrderSave').bind('click', WO.InsertAttributeOrder);

        $('#AlternateId').bind('click', WO.showAlternateId);
        $('#AlternateId').bind('focusout', WO.loadAltIdDtls);
        $('#CutPath').bind('click', WO.PopulateCutPathTxtPath);
        $('.k-grid-btnWODuplicate').bind('click', WO.AWOSizeDuplicate);

        $('.k-grid-addNew').bind('click', WO.showWOEditor);
        $('.k-grid-add').bind('click', WO.showWOEditorInLine);
        $('.k-grid-GroupGrid').bind('click', WO.GroupGrid);
        $('.k-grid-UngroupGrid').bind('click', WO.UngroupGrid);

        $("#grdwrkOrderDetail").on('click','.chkbx', function () {
            if($(this).prop('checked'))
                $('.chkbx:checked').not(this).prop('checked', false);
        })
        WO.const.SizeValidator = $('#frmSizePopup').kendoValidator().data("kendoValidator");
        

        var toolTipNote = $('#grdwrkOrderDetail').kendoTooltip({
            filter: ".cnote",
            content: function (e) {
                var Note = '';
                var dataItem = $("#grdwrkOrderDetail").data("kendoGrid").dataItem(e.target.closest("tr"));
                if (dataItem.Note != null) {
                    var pattern = /\n/ig;
                    Note = dataItem.Note.replace(pattern, '<br />');
                }
                var content = Note;

                var template = kendo.template($("#MultiSKUNoteTemplate").html());
                return template(content);
            }
        }).data("kendoTooltip");
         
        $('#grdMultiSKUSize').on('keyup', function (e) {
            if (e.keyCode == 9) {                          
                var grid = $("#grdMultiSKUSize").data("kendoGrid");
                var item =  grid.current();
                var next=null;
                if (!item.hasClass('.SizQty')) {
                    if (e.shiftKey)
                      next = item.parent().prev().find('.SizQty');
                    else
                      next = item.parent().find('.SizQty');                    
                }
                else {
                    //next = item.parent().next().find('.SizQty');
                }
                if (next) {
                    grid.editCell(next);
                    grid.select(next);
                    grid.current(next);
                    if (e.preventDefault) e.preventDefault();
                    else {
                        if(e.returnValue)  e.returnValue = false;                    
                    }
               
                }
            }
        });

        $('#btnWOSizeSave').focusin(function () {
            var grid = $("#grdMultiSKUSize").data("kendoGrid");
            if (grid) grid.select().removeClass('k-state-selected')
        });

        $('#btnWOSizeSave').on('keydown', function (e) {

            if (e.keyCode == 9) {
                var grid = $("#grdMultiSKUSize").data("kendoGrid");
                var item = grid.current();
                var firstCell = grid.table.find("tr:first td:last");
                grid.current(firstCell);
                grid.editCell(firstCell);
                grid.table.focus();
            }

        });

    },
    
    onSave:function(e) {
        setTimeout(function () {
            var grid = $("#grdMultiSKUSize").data("kendoGrid");
            var qty = 0;
            $(grid.dataSource.data()).each(function (i, it) {
                qty += it.Qty;
            });
            $("#grdMultiSKUSize .k-footer-template td").last().html('Sum Dz:' + qty);
        },0);
    } , 

    onRemove:function(e) {        
        WO.multiGetSize(e.model.Size);
    },
    
    orderDuplicate: function () {
            var quantity = null;
            var checkedItems = $('input:checked');
            
            if ($("#gridMode").val() == '') {
                if (checkedItems.length > 0) {
                    if (checkedItems.length == 1) {
                        var arr = new Array();
                        var grid = $("#grdwrkOrderDetail").data("kendoGrid");
                        $(checkedItems).each(function (idx, item) {
                            var row = $(item).closest("tr");
                            var dataItem = grid.dataItem(row);
                            arr.push(dataItem)
                        })

                        var current = WO.getDuplicateInstance(arr[0])


                        WO.showSizePopup(JSON.stringify(current.SizeList), '', current.SellingStyle, current.ColorCode, current.Attribute, function (SkuList, orderType) {
                            var Slist = JSON.parse(JSON.stringify(SkuList));
                            //grid.dataSource.data().push(current)
                            if (SkuList.length > 1) {
                                if (orderType != 'Single Order(s)') {
                                    var totQty = 0;
                                    for (var i = 0; i < Slist.length; i++) {
                                        totQty += Slist[i].Qty;
                                    }
                                    current.SizeList = Slist;

                                    //current.TotalDozens = totQty;
                                    current.Dozens = totQty;
                                    if (totQty != null && totQty != undefined)
                                        current.DozenStr = totQty.toString().replace('.', '-');

                                            grid.dataSource.data().push(current)
                                            WO.duplicateCompleted(grid, current)
                                    
                                } //end sinle sku check
                                else {
                                    WO.duplicateSingleSKU(current, Slist, grid);
                                }
                            }
                            else {
                                current.SizeList = Slist;
                                //current.TotalDozens = Slist[0].Qty;
                                current.Dozens = Slist[0].Qty;
                                if (Slist[0].Qty != null && Slist[0].Qty != undefined)
                                    current.DozenStr = Slist[0].Qty.toString().replace('.', '-');
                                grid.dataSource.data().push(current)
                                WO.duplicateCompleted(grid, current)
                            }


                        }, 'Duplicate');
                    }
                    else {
                        ISS.common.notify.error('Please select only one row.');
                    }
                }
                else {
                    ISS.common.notify.error('Please select at least one row.')
                }
            }
            else {
                ISS.common.notify.error('You have pending changes. Please save the changes first.')
            }
       
        return false;
    },

    duplicateSingleSKU: function (current, Slist, grid) {
        if (Slist.length > 0) {
            if (current.MfgPathId)
                current.MfgPathId = (current.MfgPathId).toUpperCase();
            current = WO.getDuplicateInstance(current)
            current.SizeList = Slist.splice(0, 1);                      
            current.Size = current.SizeList[0].SizeCD;     //Added By:UST(Gopikrishnan),Date:20-June-2017,Desc:For showing corresponding size of each quantity by clicking duplicate
            current.SizeCde = current.SizeList[0].Size;   //Added By:UST(Gopikrishnan)
            current.Dozens = current.SizeList[0].Qty;
            if (current.SizeList[0].Qty != null && current.SizeList[0].Qty != undefined)
                current.DozenStr = current.SizeList[0].Qty.toString().replace('.', '-');
           
            Slist = Slist;
            WO.duplicateCompleted(grid, current, function () {
                WO.duplicateSingleSKU(current, Slist, grid)
                
            });
        }
    },

    getDuplicateInstance: function (current) {
        current = JSON.parse(JSON.stringify(current));
        current.uid = null;
        current.Id = WO.getRandomId(1, 999);;
        current.GroupId = '0';
        return current;
    },

    duplicateCompleted: function (grid, current, callback) {
        WO.validateGarmentSKU(current, function () {
            grid.dataSource.data().push(current)
            //grid.refresh();
            if (callback) callback();
        });
    },

    validateGarmentSKU: function (dataItem,callback) {
        ISS.common.executeActionAsynchronous(WO.const.urlGetGarmentSKU, JSON.stringify(dataItem), function (stat, data) {
            if (stat && data) {
                dataItem.GarmentSKU = data.GarmentSKU;
                if (callback) callback(data.GarmentSKU);

            }
            else {
                dataItem.GarmentSKU = null;
                if (callback) callback(null);

            }
        });
    },


    woSizePopupClick: function () {
        //var color = $("#ColorCode").data("kendoMultiSelect").value();
        var color = $('#ColorCode').data("kendoComboBox").value();
        var attr = $("#Attribute").data("kendoComboBox").text();
        var style = $('#SellingStyle').val();
        if (color != '' && attr != '' && style != '') {
            WO.showSizePopup($("#MultiSizeList").val(),'' ,style, color, attr)
        }
        else {
            ISS.common.showPopUpMessage('Please select style, color and attribute.');
        }
        return false;
    },

    showSizePopup: function (skulst, remove, style, color, attr,handler,mode) {
        var grid = $("#grdMultiSKUSize").data("kendoGrid");      
        if (skulst != '') {
            var myList = JSON.parse(skulst);
            if (myList != null) {
                WO.const.SKUSizeList = [];
                for (var j = 0; j < myList.length; j++) {
                    WO.const.SKUSizeList.push({ SizeCD: myList[j].SizeCD, Size: myList[j].Size, Qty: myList[j].Qty });
                }
            }
            //grid.dataSource.data(myList);
        }
               

        var settings = { style: style, color: color, attr: attr, handler: handler, mode: mode };
        WO.const.sizePopup = {};
        WO.const.sizePopup.settings=settings;
        var th=$('#SizeOrderType');
        if (mode && mode == 'Duplicate') {
            var ordTyp= th.data('kendoDropDownList');
            ordTyp.enable(true)
            ordTyp.value(null)
            th.closest(".k-widget").show()
        }
        else {
            th.data('kendoDropDownList').enable(false);
            th.closest(".k-widget").hide()
        }

        //WO.const.sizePopup = ISS.common.popUp('.divSizesPopup', 'Size List');
        WO.const.sizePopup = ISS.common.popUp('.divSizesPopup', 'Size List', null, function (rr) {
            if (rr.userTriggered) {
                rr._defaultPrevented = true;
                WO.const.sizePopup.close();
                $('#btnWOSizes').focus();
            }
        });
        WO.const.sizePopup.settings = settings;
        grid.dataSource.read();
    },
 

    multiGetSize: function () {
        if (WO.const.SKUSizeList.length > 0) {
            var grid = $("#grdMultiSKUSize").data("kendoGrid");
            var gridData = grid.dataSource.view();
            //var selectedSize = '';     
            for (var i = 0; i < gridData.length; i++) {
                for (var j = 0; j < WO.const.SKUSizeList.length; j++) {
                    if (gridData[i].SizeCD == WO.const.SKUSizeList[j].SizeCD) {
                        gridData[i].Qty = WO.const.SKUSizeList[j].Qty;
                    }
                }  
            }
            grid.refresh();
        }        
    },

    multiSizeLoad: function () {
        var color = '';
        var attr = '';
        var style = '';
            //if (WO.const.sizePopup.settings.mode == 'Duplicate') {
            //    color = WO.const.sizePopup.settings.color;
            //    attr = WO.const.sizePopup.settings.attr;
            //    style = WO.const.sizePopup.settings.style;
            //}
            //else {
                //var colors = $("#ColorCode").data("kendoMultiSelect").value();
                //color = WO.getMultiselectItem(colors);
                 color = $('#ColorCode').data("kendoComboBox").value();
                 attr = $("#Attribute").data("kendoComboBox").text();
                 style = $('#SellingStyle').val();
        //    }
        //var sizelst = WO.const.SKUSizeList;
        if (color != '' && attr != '' && style != '') {            
            //var searchCriteria = {
            //    Style_Cd: style,
            //    Color_Cd: color,
            //    Attribute_Cd: attr
            //};
            var searchCriteria = {
                SellingStyle: style,
                ColorCode: color,
                Attribute: attr
            };
            return searchCriteria;
        }
        return false;
    },


    woAddSize: function () {
        var siz = $("#Size").data("kendoMultiSelect").value();
        if (siz == "") {
            ISS.common.showPopUpMessage('Please select any size.');
            return false;
        }

        $('#Size :selected').each(function (i, selected) {

            var gridWO = $("#grdMultiSKUSize").data("kendoGrid");
            var gridData = gridWO.dataSource.view();
            if (gridData.length < 10) {
                gridWO.dataSource.add({ SizeCD: $(selected).val(), Size: $(selected).text(), Qty: 0 });
            }
            else {
                ISS.common.showPopUpMessage('You can select upto 10 sizes.');
            }
        });
        WO.multiGetSize(null);
        return false;
    },

    onMultiSizeSave: function () {
        WO.const.SKUSizeList = [];
        $("#SelectedSizes").html('');
        var skusize = '';
        var selectedSize = '';
        var grid = $("#grdMultiSKUSize").data("kendoGrid");
        var gridData = grid.dataSource.view();
        if (gridData.length > 0) {

            for (var i = 0; i < gridData.length; i++) {
                if (gridData[i].Qty > 0) {
                    WO.const.SKUSizeList.push({ SizeCD: gridData[i].SizeCD, Size: gridData[i].Size, Qty: gridData[i].Qty });
                    selectedSize += gridData[i].Size + " - " + gridData[i].Qty + ", ";
                }
                //else {
                //    skusize = gridData[i].Size;
                //}
            }

            if (WO.const.SKUSizeList.length == 0) {
                ISS.common.showPopUpMessage('Please enter quantity for the required sizes.');
                return false;
            } 
            if (WO.const.sizePopup.settings && WO.const.sizePopup.settings.mode == 'Duplicate' && WO.const.SKUSizeList.length>1)
            {
                if (!WO.const.SizeValidator.validate()) {
                    return false;
                }
            }
            if (WO.const.SKUSizeList.length <= 10) {

                WO.const.sizePopup.close();
                if (WO.const.sizePopup.settings.handler) { // used for duplicate popup
                    WO.const.sizePopup.settings.handler(WO.const.SKUSizeList, $('#SizeOrderType').data('kendoDropDownList').value());
                    return false;
                }
                else {

                    var totalText = $("#grdMultiSKUSize .k-footer-template").text().split(":")[1].trim();
                    var skulst = JSON.stringify(WO.const.SKUSizeList);
                    $("#MultiSizeList").val(skulst);
                    //$("#TotalDozens").val(totalText);
                    $("#Dozens").val(totalText);
                    if (WO.const.SKUSizeList.length > 1) {
                        if ($("#GroupId").val() == '0' || $("#GroupId").val() == '') {
                            //ISS.common.executeActionAsynchronous(WO.const.urlGetGroupId, null, function (stat, data) {
                            //    if (stat && data) {
                            //        $("#GroupId").val(data);
                            //    }
                            //});
                        }
                    }
                    else {
                        $("#GroupId").val('0');
                    }
                    WO.onSizeChange();
                    WO.SelectedSizesList(WO.const.SKUSizeList);

                    $("#Revision").focus();
                }

                //selectedSize = selectedSize.replace(/^,|,$/g, '');
                //selectedSize = selectedSize.slice(0, -2);
                //$("#lblSelectedSize").html("Selected Size(s): " + selectedSize);
            }
            else {
                ISS.common.showPopUpMessage('You can select upto 10 sizes.');
            }
        }
        else {
            
        }
        return false;
    },

    InsertAttributeOrder: function (e) {

        WO.changeYear(null, function (ress) {
            if (ress) {
                var isOk = false;
                var url = 'InsertAttributionOrder';
                var gridDetail = $("#grdwrkOrderDetail").data("kendoGrid");
                var dsWOData = gridDetail.dataSource.data();
                if (dsWOData.length > 0) {
                    WO.const.SKUSizeList = [];
                    for (var i = 0; i < dsWOData.length; i++) {
                        var sz = dsWOData[i].Size;
                        var qty = dsWOData[i].DozenStr;
                        var szdesc = dsWOData[i].SizeCde;
                        if (qty == '')
                            qty = 0;
                        WO.const.SKUSizeList.push({
                            SizeCD: sz, Size: szdesc, Qty: qty
                        });
                    }
                }
                //WO.MultiSizeDataInline(dsWOData); //Added By :UST(Gopikrishnan), Date:21-June-2017, Desc: For adding multiple sizes to SKUSizeList from datasource
                if (dsWOData.length <= 0) {
                    ISS.common.showPopUpMessage('Please enter at least one order detail.');
                    return false;
                }
                ISS.common.blockUI(true);
                //$(dsWOData).each(function (i, ite) { ite.ErrorStatus = false; });
                $(dsWOData).each(function (i, ite) {
                    ite.ErrorStatus = false; ite.SizeList = WO.const.SKUSizeList; ite.ProdFamCode = prod_fam, ite.CorpBusUnit = corpbus_cd, ite.PrimaryDC = prm_cd
                });
                //var gridCum = $("#grdwrkOrderCumulative").data("kendoGrid");
                //var dsWOCum = gridCum.dataSource.data();

                //var gridFab = $("#grdwrkOrderFabric").data("kendoGrid");
                //var dsWOFab = gridFab.dataSource.data();

                var dueDate = "";
                var orders = $("#OrdersToCreate").data("kendoNumericTextBox").value();
                var planner = $("#PlannerCd").data("kendoDropDownList").value();

                var woHeader = {
                    Dmd: "",
                    DueDate: dueDate,
                    PlannedWeek: $('#PlannedWeek').val(),
                    PlannedYear: $('#PlannedYear').val(),
                    PlannedDate: $('#PlannedDate').val(),
                    Dc: $('#Dc').val(),
                    OrdersToCreate: orders,
                    PlannerCd: planner,
                    //CorpBusUnit: corpbus_cd,
                    //PrimaryDC: prm_cd,
                    //ProdFamCode: prod_fam,
                    WODetails: dsWOData

                };
                // woHeader.SizeList = JSON.parse(JSON.stringify(WO.const.SKUSizeList));
                woHeader = JSON.stringify(woHeader);

                ISS.common.executeActionAsynchronous(url, woHeader, function (stat, data) {
                    ISS.common.blockUI();
                    if (stat && data) {
                        if (data.Status) {
                            // saved success  
                            if (data.FailCount > 0) {
                                var SMsg = '';
                                if (data.SuccessCount > 0) {
                                    SMsg = data.SuccessCount + " Order(s) created successfully.<br/>";
                                    // ISS.common.notify.success(SMsg);
                                }
                                var EMsg = data.FailCount + " Order(s) has error(s).";
                                ISS.common.showPopUpMessage(SMsg + EMsg, ISS.common.MsgType.Error, function () {
                                    WO.onErrorFailure(data);
                                    ISS.common.notify.error(EMsg);
                                });


                            }
                            else if (data.SuccessCount > 0 && data.FailCount == 0) {
                                var SMsg = data.SuccessCount + " Order(s) created successfully.";
                                // ISS.common.notify.success(SMsg);

                                ISS.common.showConfirmMessage(SMsg + '<br/> Do you want to clear the screen?', null, function (reply) {
                                    if (reply) {
                                        WO.ClearWOPage();
                                    }
                                    else {
                                        WO.GenerateNewGroupId();
                                    }
                                });

                            }
                        }
                        else {
                            // save failed
                            var pattern = /\n/ig;
                            var errorDesc = data.ErrMsg.replace(pattern, '<br />');
                            ISS.common.showPopUpMessage(errorDesc, ISS.common.MsgType.Error, function () {
                                WO.onErrorFailure(data);
                                ISS.common.notify.error(errorDesc);
                            });
                        }
                    }

                    else {
                        // save failed
                        var pattern = /\n/ig;
                        var errorDesc = data.ErrMsg.replace(pattern, '<br />');
                        ISS.common.showPopUpMessage(errorDesc, ISS.common.MsgType.Error, function () {
                            WO.onErrorFailure(data);
                            ISS.common.notify.error(errorDesc);
                        });
                    }


                }, 'POST');
            }

        });
        return false;

    },

    onErrorFailure: function (data) {
        var grid = $("#grdwrkOrderDetail").data("kendoGrid");
        var gridData = grid.dataSource.view();
        if (data.Property1 != null) {
            if (data.Property1.length == gridData.length) {
                for (var i = 0; i < gridData.length; i++) {
                    gridData[i].ErrorMessage = data.Property1[i].ErrorMessage
                    gridData[i].ErrorStatus = data.Property1[i].ErrorStatus
                }
                grid.refresh();
            }
        } 
    },

    onErrorCorrected: function (data) {
        var grid = $("#grdwrkOrderDetail").data("kendoGrid");
        var gridData = grid.dataSource.view();
        for (var i = 0; i < gridData.length; i++) {
            var row = grid.table.find("tr[data-uid='" + gridData[i].uid + "']");
            if (data.ErrDetails != null) {
                data.ErrDetails.forEach(function (item) {
                    if (gridData[i].Id == item.Id) {
                        grid.dataSource.getByUid(gridData[i].uid).set("ErrorMessage", '');
                        grid.dataSource.getByUid(gridData[i].uid).set("ErrorStatus", false);
                        row.removeClass("highlighted-row");
                    }
                });
            }
            //if (gridData[i].Id == data.Id) {
            //    grid.dataSource.getByUid(gridData[i].uid).set("ErrorMessage", '');
            //    grid.dataSource.getByUid(gridData[i].uid).set("ErrorStatus", false);
            //    row.removeClass("highlighted-row");
            //}
        }

    },

    GenerateNewGroupId: function () {
        WO.GroupIdGeneration();
    },

    GroupIdGeneration: function () {
        var grpIds = '';
        var grid = $("#grdwrkOrderDetail").data("kendoGrid");
        var gridData = grid.dataSource.view();
        var gridLen = {
            dgridCount: gridData.length
        };

        gridLen = JSON.stringify(gridLen);

        ISS.common.executeActionAsynchronous(WO.const.urlGetBulkGroupId, gridLen, function (stat, data) {
            if (stat && data) {
                var grid = $("#grdwrkOrderDetail").data("kendoGrid");
                var gridData = grid.dataSource.view();
                for (var i = 0; i < gridData.length; i++) {
                    if (gridData[i].GroupId > 0) {
                        grid.dataSource.getByUid(gridData[i].uid).set("GroupId", data[i]);
                        grpIds = grpIds + data[i] + ',';
                    }
                }
                //grid.dataSource.getByUid(groupUId).set("GroupId", data);
                //return data;
                if (grpIds != "") {
                    grpIds = grpIds.substring(0, grpIds.length - 1);
                    ISS.common.notify.info("New Group ID(s) " + grpIds + " generated");
                }
            }
        }, 'POST');
    },

    onCumulativeDataBound: function () {

        var grid = $("#grdwrkOrderCumulative").data("kendoGrid");
        var gridData = grid.dataSource.view();
        for (var i = 0; i < gridData.length; i++) {
            var row = grid.table.find("tr[data-uid='" + gridData[i].uid + "']");
            //if (gridData[i].PlanDate) {
            //    //gridData[i].PlanDate = WO.formatDate(gridData[i].PlanDate);
            //gridData[i].PlanDate = WO.format(gridData[i].PlanDate, 'MM/dd/yyyy')
            //}
            if (gridData[i].IsHide) {
                row.hide();
                continue;
            }
            else {
                row.show();
            }
        }
    },

    onMultiSkuQtyCodeChanged: function (e) {

        var grid = $("#grdMultiSKUSize").data("kendoGrid");
        var closeRow = $(e).closest('tr');

        var currentSelection = grid.select().parent();
        var selectedItem = grid.dataItem(currentSelection)
        if (selectedItem) {
            selectedItem.Qty = parseInt(e.value)
            //e.value = e.value.toFixed();
        }

    },

    grid_change: function (e) {
        var selectedCells = this.select();
        var selectedDataItems = [];
        for (var i = 0; i < selectedCells.length; i++) {
            var dataItem = this.dataItem(selectedCells[i].parentNode);
            if ($.inArray(dataItem, selectedDataItems) < 0) {
                //selectedDataItems.push(dataItem);
                dataItem.Qty = '10';
            }
        }
    },

    formatDate: function (date) {
        function pad(s) { return (s < 10) ? '0' + s : s; }
        var output = [pad(date.getMonth() + 1), pad(date.getDate()), date.getFullYear()].join('/');
        return output;
    },

    changeDetDates: function (d) {
        if (d.PlanDate) d.PlanDate = ISS.common.parseDate(d.PlanDate);
        if (d.CurrentDueDate) d.CurrentDueDate = ISS.common.parseDate(d.CurrentDueDate);
        if (d.DueDate) d.DueDate = ISS.common.parseDate(d.DueDate);

    },

    multiSizeGridClear: function () {
        WO.const.SKUSizeList = [];
        $("#MultiSizeList").val('');
        $("#SelectedSizes").html('');
        var gridSize = $("#grdMultiSKUSize").data("kendoGrid");
        gridSize.dataSource.data([]);
        gridSize.refresh();
    },

    showAlternateId: function (e) {
        //var colors = $("#ColorCode").data("kendoMultiSelect").value()
        //var ColorCode = WO.getMultiselectItem(colors);
        var ColorCode = $('#ColorCode').data("kendoComboBox").value();
        var Attribute = $("#Attribute").data("kendoComboBox").text();
        var Style = $("#SellingStyle").val();

        var pdata = {
            Style: Style,
            Color: ColorCode,
            Attribute: Attribute
        };


        settings = {
            columns: [{
                Name: "CuttingAlt",
                Title: "Alt Id",
            },
            {
                Name: "BillOfMtrlsId",
                Title: "Alt Desc",
            }
            ],
            AllowSelect: true,
            title: 'Select Alternate Id',
            url: WO.const.urlGetAlternateId,
            postData: pdata,
            handler: function (d) {
                $('#AlternateId').val(d.CuttingAlt);
                if (WO.CreateOrder.isUpdateCumOnChange) {
                    WO.updateCumulativeAndFabric();
                }
            }
        };
        ISS.common.CommonSearchShow(settings);

    },

    clearMultiSize: function (e) {

        var ds = $("#Size").data("kendoMultiSelect");
        ds.value([]);

        return false;
    },

    PopulateCutPathTxtPath: function (e) {
        WO.MultiSizeData();
        //var ColorCode = $("#ColorCode").data("kendoMultiSelect").value();
        var ColorCode = $('#ColorCode').data("kendoComboBox").value();
        var Attribute = $("#Attribute").data("kendoComboBox").text();
        var Style = $("#SellingStyle").val();
        var SizeList = WO.const.SKUSizeList;
        var MfgPathId = $("#MfgPathId").val();


        var pdata = {
            SellingStyle: Style,
            ColorCode: ColorCode,
            Attribute: Attribute,
            MFGPathId: MfgPathId,
            DyeCode: 'C',
            SizeCd: SizeList[0].SizeCD
        };

        settings = {
            columns: [{
                Name: "Source_Plant",
                Title: "Path Id",
            },
            {
                Name: "Source_Plant",
                Title: "Cut Plt",
            }],
            AllowSelect: true,
            title: 'Select a Cut Plant',
            url: WO.const.urlPopulateCutPathTxtPath,
            postData: pdata,
            handler: function (d) {
                $('#CutPath').val(d.Source_Plant);
                return false;
            }
        };
        ISS.common.CommonSearchShow(settings);
    },

    onColorBound: function (e) {
        //var ds = $("#Attribute").data("kendoComboBox");
        //if (ds != null && ds != undefined)
        //    ds.dataSource.read();

        //$("#Size").data("kendoComboBox").text('');
        //$("#Size").data("kendoComboBox").value('');
    },

    onAttributeBound: function (e) {
        //var attr = $("#Attribute").data("kendoComboBox").value();
        //if (attr != null && attr != undefined && attr != "") {
        //    var ds = $("#Size").data("kendoComboBox");
        //    if (ds != null && ds != undefined)
        //        ds.dataSource.read();
        //}

        //$("#Size").data("kendoComboBox").text('');
        //$("#Size").data("kendoComboBox").value('');
    },

    validateCreateWO: function () {
        var urlValidation = '../order/ValidateMultiSku';

        //var txtPlant = $("#TxtPlant").data("kendoComboBox").value();

        //var machineType = $("#MacType").data("kendoComboBox").value();

        var gridDetail = $("#grdwrkOrderDetail").data("kendoGrid");
        var dsWOData = gridDetail.dataSource.data();

        //var gridCum = $("#grdwrkOrderCumulative").data("kendoGrid");
        //var dsWOCum = gridCum.dataSource.data();

        //var gridFab = $("#grdwrkOrderFabric").data("kendoGrid");
        //var dsWOFab = gridFab.dataSource.data();

        //var dueDate = $("#DueDate").data("kendoComboBox").value();
        var dueDate = "";
        var orders = $("#OrdersToCreate").data("kendoNumericTextBox").value();
        var planner = $("#PlannerCd").data("kendoDropDownList").value();
        //var dmd = $("#Dmd").data("kendoDropDownList").value();

        if (dsWOData.length <= 0) {
            return false;
        }

        var woHeader = {
            Dmd: "",
            DueDate: dueDate,
            PlannedWeek: $('#PlannedWeek').val(),
            PlannedYear: $('#PlannedYear').val(),
            PlannedDate: $('#PlannedDate').val(),
            Dc: $('#Dc').val(),
            OrdersToCreate: orders,
            PlannerCd: planner,
            TxtPlant: null,
            MachinePlant: null,
            WODetails: dsWOData,
            WOCumulative: null,
            WOFabric: null
        };

        woHeader = JSON.stringify(woHeader);

        ISS.common.executeActionAsynchronous(urlValidation, woHeader, function (stat, data) {
            if (stat && data) {

                if (!data.Status) {
                    WO.onErrorCorrected(data);
                }
            }
        });
    },

    loadDetailsGrdFromWOM: function () {
        var url = '../order/GetOrderDetailByOrderLabel';
        var superOrder = $('#SuperOrder').val();

        var superOrder = {
            SuperOrder: superOrder
        };

        superOrder = JSON.stringify(superOrder);
        ISS.common.blockUI(true);
        ISS.common.executeActionAsynchronous(url, superOrder, function (stat, data) {
            if (stat && data) {

                //var data = getWOFromWOM();
                if (data != null && data.length > 0) {
                    var grid = $("#grdwrkOrderDetail").data("kendoGrid");
                    //var obj = WO.AddWODetail(data[0]);
                    var dataGrd = grid.dataSource.data();
                    dataGrd.push(data[0]);
                    grid.refresh();

                    WO.RecalcWO();

                    //ISS.common.blockUI(false);
                }
                else {
                    ISS.common.blockUI(false);
                    ISS.common.notify.error("Failed to retrieve details.");
                }
            }
            else {
                ISS.common.blockUI(false);
                ISS.common.notify.error("Failed to retrieve details.");
            }
        }, 'POST');

    },

    getGarmentSKU: function () {
        WO.MultiSizeData();
        var postData = WO.retrieveColorData();

        //var colors = $('#ColorCode').data("kendoMultiSelect").value();
        //ColorCode = WO.getMultiselectItem(colors);
        var ColorCode = $('#ColorCode').data("kendoComboBox").value();
        var Attribute = $("#Attribute").data("kendoComboBox");

        if (ColorCode != '' && Attribute.text() != '' && WO.const.SKUSizeList.length > 0) {
            postData.ColorCode = ColorCode;
            postData.Attribute = Attribute.text();

            postData.SizeList = WO.const.SKUSizeList;
            postData.MfgPathId = $("#MfgPathId").val();
            
            postData = JSON.stringify(postData);
            $("#GarmentSKU").val('');
            ISS.common.executeActionAsynchronous(WO.const.urlGetGarmentSKU, postData, function (stat, data) {
                if (stat) {
                    if (data != null && data != undefined) {
                        $("#GarmentSKU").val(data.GarmentSKU);
                    }
                }
                else {
                    ISS.common.showPopUpMessage('Failed to retrieve Garment SKU details.');
                }

            });
        }
        return false;
                

    },

    GroupGrid: function (e) {       
        var grid = $("#grdwrkOrderDetail").data("kendoGrid");
        //var rows = $('input:checked');
        var rows = $('.chckbx:checked');
        var SKUGroup = null;
        var attribute = null;
        var isBreak=false;
        var dataColl = new Array();
        var arrAtrrb = new Array();
        var flag = true;
        if (rows.length > 1) {

            $(rows).each(function (idx, item) {
                var row = $(item).closest("tr");
                var data = grid.dataItem(row);
                if (data && !isBreak) {
                    if (data.GroupId != null && data.GroupId != "0") {
                        flag = false;
                        ISS.common.showPopUpMessage('Selected row is not a single SKU record.');
                        isBreak = true;
                        //break;
                    }
                    else {
                        //var groupFields = data.Attribute + data.MfgPathId + data.Dc + data.AttributionPath;
                        var groupFields = data.Attribute + data.MfgPathId + data.PrimaryDC;

                        if (SKUGroup != null) {
                            if (SKUGroup != groupFields) {
                                flag = false;
                                ISS.common.showPopUpMessage('Multi Sku order must match by Selling attribute, MFG Path, DC and Attribution Path. ');
                                isBreak = true;
                            }
                            else if (attribute != "" && attribute != data.Attribute) {
                                flag = false;
                                ISS.common.showPopUpMessage("Different attributes  (" + data.Attribute + ", " + attribute + ") cannot be grouped - " + groupFields);
                                isBreak = true;
                            }
                            else {

                            }
                        } else {
                            SKUGroup = groupFields;
                        }
                        attribute = data.Attribute
                        arrAtrrb.push(data.Attribute);
                        dataColl.push(data);
                    }// end else isMSKU

                }
            });

            if (flag) {
                //TBD
                if (dataColl.length > 1) {
                    ISS.common.execute(WO.const.urlGetGroupId, null, function (s, d) {
                        if (s && d) {
                            for (j = 0; j < dataColl.length; j++) {
                                var gdata = dataColl[j];
                                //ISS.common.cloneAndStore(gdata);
                                gdata.GroupId = d;
                            }
                            grid.refresh();
                        }
                    });
                }
            }
        }
        else {
            ISS.common.showPopUpMessage('Please select at least two rows.')
        }
        return false;
    },

    UngroupGrid: function (e) {
        var grid = $("#grdwrkOrderDetail").data("kendoGrid");
        //var rows = $('input:checked');
        var rows = $('.chckbx:checked');
        var GroupId = null;
        var flag = true;
        var isBreak=false;
        var arr = new Array();
        if (rows.length > 0) {

            $(rows).each(function (idx, item) {
                var row = $(item).closest("tr");
                var data = grid.dataItem(row);

                if (data && !data.IsDeleted && !isBreak) {
                   
                    if (data.GroupId == null || data.GroupId == 0) {   //|| data.IsGrouped                       
                        flag = false;
                        ISS.common.showPopUpMessage('Selected row is not a multi-SKU record.');
                        isBreak=true;
                    }
                    if (GroupId != null) {
                        if (GroupId != data.GroupId) {
                            flag = false;
                            ISS.common.showPopUpMessage('Not able to ungroup selected rows. Group Id is different');
                            isBreak=true;
                        }
                    } else {
                        GroupId = data.GroupId;
                    }
                    arr.push(data)
                   
                }
            });

            if (flag) {
                ISS.common.showConfirmMessage('Do you want to Ungroup all the selected Multi SKU records?', null, function (ok) {
                    if (ok) {
                        var groups = WO.getAllMSKU(GroupId);
                        for (i = 0; i < groups.length; i++) {
                            var gdata = groups[i];
                            gdata.GroupId = 0;
                        }

                        grid.refresh();
                    }
                    
                    
                    
                }, 'Yes', 'No');
            }
        
            }
        else {
            ISS.common.showPopUpMessage('Please select at least one row.')
        }


        return false;
    },

    getAllMSKU: function (id) {
        var grid = $("#grdwrkOrderDetail").data("kendoGrid");
        var data = [];
        var gridData = grid.dataSource.view();
        
        for (i = 0; i < gridData.length; i++) {
            if (id == gridData[i].GroupId)
                data.push(gridData[i]);
            
        }

        //var filter = ISS.common.getFilter('and')
        //filter.filters.push(ISS.common.getFilterItem('GroupId', 'eq', id))
        //var query = new kendo.data.Query(grid.dataSource.data());
        //var data = query.filter(filter).data;
        return data;
    },

    MultisizeDataBound: function () {
        //var grid = $("#grdMultiSKUSize").data("kendoGrid");
        //var firstCell = grid.table.find("tr:first td:last");
        //if (firstCell != null) {
        //    grid.current(firstCell);
        //    grid.editCell(firstCell);
        //    grid.table.focus();
        //}
    },

    MultiSizeData: function () {
        WO.const.SKUSizeList = [];
        var sz = $('#Size').data("kendoComboBox");
        //var qty = $("#Dozens").val();
        var qty = $("#DozenStr").val().replace('-', '.');
        if (qty == '')
            qty = 0;
        WO.const.SKUSizeList.push({ SizeCD: size_cd, Size: size_desc, Qty: qty });
    },

    //Modified By :UST(Gopikrishnan), Date:21-June-2017, Desc: For adding multiple sizes to SKUSizeList array from datasource
    MultiSizeDataInline: function (dsWOData) {
            if (dsWOData.length > 0) {
                WO.const.SKUSizeList = [];
            for (var i = 0; i < dsWOData.length; i++) {
               // WO.const.SKUSizeList = [];
                //var sz = size_cd;
                ////var qty = $("#Dozens").val();
                //var qty = size_qty
                //if (qty == '')
                //    qty = 0;
                //WO.const.SKUSizeList.push({ SizeCD: size_cd, Size: size_desc, Qty: qty });
                var sz = dsWOData[i].SizeList[0].SizeCD;
                var qty = dsWOData[i].SizeList[0].Qty;
                if (qty == '')
                    qty = 0;
                WO.const.SKUSizeList.push({ SizeCD: sz, Size: dsWOData[i].SizeList[0].Size, Qty: qty });
            }
        }
    },

    KASizeChange: function () {
        WO.MultiSizeData();
        WO.onSizeChange();
       
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
    enterMfgPath: function (e) {
        var grid = $("#grdwrkOrderDetail").data("kendoGrid");
        var demLoc = $("#PrimaryDC").val();
        var postData = { SellingStyle: style_cd, ColorCode: color_cd, Attribute: attr_cd, SizeList: WO.const.SKUSizeList, PrimaryDC: demLoc };
        postData = JSON.stringify(postData);
        MfgPathId_cd = e.value.toUpperCase();
        ISS.common.executeActionAsynchronous(WO.const.urlMfgPath, postData, function (stat, data) {
            if (stat) {
                if (data.length > 0) {
                    var len = data.length;
                    flg = false;
                    for (var i = 0; i < len; i++) {
                        if (MfgPathId_cd == data[i].MfgPathId)
                        {
                            flg = true;
                            var selectedItem = grid.dataItem(currentTableRow);
                            selectedItem ? selectedItem : selectedItem = selectrow;
                            
                            $('#MfgPathId').val(data[i].MfgPathId);
                            
                            MfgPathId_cd = data[i].MfgPathId;
                            if (MfgPathId_cd)
                                MfgPathId_cd.toUpperCase();
                            grid.table.find("tr[data-uid='" + selectedItem.uid + "'] td:eq(9)")[0].innerText = data[i].MfgPathId;
                            //selectedItem.MfgPathId = data[i].MfgPathId;
                            //alert(d.MfgPathId);
                            Garment_cd = data[i].GarmentSKU;
                            $('#GarmentSKU').val(data[i].GarmentSKU);
                            //selectedItem.GarmentSKU = data[i].GarmentSKU;
                            grid.table.find("tr[data-uid='" +selectedItem.uid + "'] td:eq(10)")[0].innerText = data[i].GarmentSKU;
                            SewPlt_cd = data[i].SewPlt;
                            $('#AttributionPath').val(data[i].SewPlt);
                            grid.table.find("tr[data-uid='" + selectedItem.uid + "'] td:eq(11)")[0].innerText = data[i].SewPlt;
                            $("#GStyle").val(data[i].GStyle);
                            selectedItem.GStyle = data[i].GStyle;
                            $("#GColor").val(data[i].GColor);
                            selectedItem.GColor = data[i].GColor;
                            $("#GAttribute").val(data[i].GAttribute);
                            selectedItem.GAttribute = data[i].GAttribute;
                            $("#GSize").val(data[i].GSize);
                            $("#Dc").val(prm_cd);
                            selectedItem.GSize = data[i].GSize;
                            WO.CreateOrder.mfgLoaded = true;
                            
                            //grid.refresh();

                            WO.getDCLocations();
                        }
                        
                    }
                    if(flg == false)
                        ISS.common.showPopUpMessage('Please enter valid MFG Path Id.');
                }
                else {
                    ISS.common.showPopUpMessage('Failed to retrieve MFG Path Id.');
                }
            }
            
            
        });
        
    },
    showMfgPathIdInline: function (e) {

        var result = WO.retrieveColorDataIn();
        //var colors = $("#ColorCode").data("kendoMultiSelect").value();
        //var color = WO.getMultiselectItem(colors);
        var color = color_cd;
        var attr = attr_cd;
        WO.MultiSizeData();

        var demLoc = $("#PrimaryDC").val();
        result.ColorCode = color;
        result.Attribute = attr;
        result.SizeList = JSON.parse(JSON.stringify(WO.const.SKUSizeList));
        result.PrimaryDC = demLoc;
       
        //var SizeList = WO.const.SKUSizeList;
        if (style_cd != '' && result.ColorCode != '' && result.Attribute != '' && result.SizeList.length > 0) {
            settings = {
                columns: [{
                    Name: "MfgPathId",
                    Title: "Path ID",
                },
                {
                    Name: "SewPlt",
                    Title: "Attribute Plt",
                },
                {
                    Name: "GarmentSKU",
                    Title: "Garment SKU",
                }
                ],
                AllowSelect: true,
                title: 'Select a path',
                url: WO.const.urlMfgPath,
                postData: result,
                close: function () {
                    setTimeout(function () {
                        $('#MfgPathId').focus();
                    }, 0)
                    return false;
                },
                handler: function (d) {
                    var grid = $("#grdwrkOrderDetail").data("kendoGrid");
                    var selectedItem = grid.dataItem(currentTableRow);
                    selectedItem ? selectedItem : selectedItem = selectrow;
                    $('#MfgPathId').val(d.MfgPathId);
                    if (d.MfgPathId)
                        d.MfgPathId.toUpperCase();
                    MfgPathId_cd = d.MfgPathId;
                    selectedItem.MfgPathId = d.MfgPathId;
                    //alert(d.MfgPathId);
                    $('#GarmentSKU').val(d.GarmentSKU);
                    Garment_cd = d.GarmentSKU;
                    selectedItem.GarmentSKU = d.GarmentSKU;
                    $('#AttributionPath').val(d.SewPlt);
                    SewPlt_cd = d.SewPlt
                    selectedItem.AttributionPath = d.SewPlt;
                    $("#GStyle").val(d.GStyle);
                    selectedItem.GStyle = d.GStyle;
                    $("#GColor").val(d.GColor);
                    selectedItem.GColor = d.GColor;
                    $("#GAttribute").val(d.GAttribute);
                    selectedItem.GAttribute = d.GAttribute;
                    $("#GSize").val(d.GSize);
                    $("#Dc").val(prm_cd);
                    selectedItem.GSize = d.GSize;
                    WO.CreateOrder.mfgLoaded = true;
                    WO.getDCLocations();
                    grid.refresh();
                    
                    //$('#MfgPathId').focus();
                }
            };
            ISS.common.CommonSearchShow(settings);

        }
        else {
            ISS.common.showPopUpMessage('Please enter Style, Color, Attribute and Size to generate Mfg Path.');

        }
        Focusedindex = 9;
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
    showMfgPathId: function (e) {
        
        var result = WO.retrieveColorData();
        //var colors = $("#ColorCode").data("kendoMultiSelect").value();
        //var color = WO.getMultiselectItem(colors);
        var color = $('#ColorCode').data("kendoComboBox").value();
        var attr = $("#Attribute").data("kendoComboBox");
        WO.MultiSizeData();

        var demLoc = $("#PrimaryDC").val();
        result.ColorCode = color;
        result.Attribute = attr.text();
        result.SizeList = JSON.parse(JSON.stringify(WO.const.SKUSizeList));
        result.PrimaryDC = demLoc;
        //var SizeList = WO.const.SKUSizeList;
        if ($('#SellingStyle').val() != '' && result.ColorCode != '' && result.Attribute != '' && result.SizeList.length > 0) {
            settings = {
                columns: [{
                    Name: "MfgPathId",
                    Title: "Path ID",
                },
                {
                    Name: "SewPlt",
                    Title: "Attribute Plt",
                },
                {
                    Name: "GarmentSKU",
                    Title: "Garment SKU",
                }
                ],
                AllowSelect: true,
                title: 'Select a  path',
                url: WO.const.urlMfgPath,
                postData: result,
                close: function () {
                    setTimeout(function () {
                        $('#MfgPathId').focus();
                    }, 0)
                    return false;
                },
                handler: function (d) {
                    $('#MfgPathId').val(d.MfgPathId);
                    $('#GarmentSKU').val(d.GarmentSKU);
                    $('#AttributionPath').val(d.SewPlt);
                    $("#GStyle").val(d.GStyle);
                    $("#GColor").val(d.GColor);
                    $("#GAttribute").val(d.GAttribute);
                    $("#GSize").val(d.GSize);
                    WO.CreateOrder.mfgLoaded = true;
                    $('#MfgPathId').focus();
                }
            };
            ISS.common.CommonSearchShow(settings);

        }
        else {
            ISS.common.showPopUpMessage('Please enter Style, Color, Attribute and Size to generate Mfg Path.');

        }
        return false;

        
    },

    AWOSizeDuplicate: function () {
        //if (requisitions.isValidFormMode()) {
            //var quantity = null;
            var checkedItems = $('.chckbx:checked');
            //var checkedItems = $('input:checked');
            if (checkedItems.length == 1) {
                var arr = new Array();
                var grid = $("#grdwrkOrderDetail").data("kendoGrid");
                var dsWOData = grid.dataSource.data();
                $(dsWOData).each(function (i, ite) { ite.ErrorStatus = false; ite.SizeList = WO.const.SKUSizeList; });
                $(checkedItems).each(function (idx, item) {
                    var row = $(item).closest("tr");
                    var dataItem = grid.dataItem(row);
                    arr.push(dataItem)
                })
                if (grid) {
                    var data = grid.dataSource._data;
                    //quantity = arr[0].Qty;
                    var postData = { SellingStyle: arr[0].SellingStyle, ColorCode: arr[0].ColorCode, Attribute: arr[0].Attribute }
                    //var postData = WO.multiSizeLoad();
                    postData.Size = arr[0].SizeList[0].Size;
                    postData.Dozens = arr[0].Dozens;
                    postData.DozenStr = arr[0].DozenStr;
                    ISS.common.executeActionAsynchronous(WO.const.urlGetSkuSizes, JSON.stringify(postData), function (stat, data) {
                        if (stat && data) {
                            var content = '<table id="tblDuplicate">'
                            content += '<tr>';
                            content += '<td colspan="3">' + postData.SellingStyle + ' ' + postData.ColorCode + ' ' + postData.Attribute  + '</td><td>&nbsp;</td>';
                            content += '</tr>';
                            content += '<tr>';
                            //content += '<tr>';
                            //content += '<td>' + " ****************** " + '</td>';
                            //content += '<td></td>';
                            content += ' <tr class="blankrow1"></tr>';
                            content += '</tr>';
                            content += '<tr><td colspan="3">Create New Sizes</td><td>&nbsp;<td/></tr>';
                            content += ' <tr class="blankrow1"></tr>';
                            for (i = 0; i < data.length; i++) {
                                content += '<tr class="clstr">';
                                content += '<td width="100px">' + data[i].SizeDesc + '</td>';
                                if (postData.Size == data[i].SizeDesc) {
                                    content += '<td>' + '<input type="text" class="DozenEaches k-textbox"  MaxLength="12"  value="' + (postData.DozenStr) + '" /> </td>';
                                }
                                else
                                    content += '<td>' + '<input type="text"  MaxLength="12"  class="DozenEaches k-textbox"  /> </td>';

                                content += '<td>' + '<input type="hidden" value="' + data[i].Size + '"/> </td>';
                                content += '</tr>';
                                content += ' <tr class="blankrow1"></tr>';
                            }
                            content += '<tr width="20px">';
                            content += '<td></td>';
                            content += '<td >' + '<input type="submit" value="Ok" id="btnOk" onclick="javascript:WO.GetDuplicateSingle();" /> ';
                            content += '&nbsp;' + '<input type="submit" value="Cancel" id="btnCancel" onclick="javascript:WO.DuplicateCancel();"  /> </td>';
                            content += '</tr>';
                            content += "</table>"

                            var settings = {
                                title: 'Duplicate Sizes',
                                animation: false,
                                height: '600px',
                                width: '350px',
                            };
                            WO.const.sizePopup = ISS.common.popUpCustom('.divDuplicate', settings);
                            $(".divDuplicate").html(content);
                            
                        }

                    });
                }

            }
            else if (checkedItems.length > 1) {
                ISS.common.showConfirmMessage('You have selected multiple items. Are you sure want to duplicate same records?', null, function (ret) {
                    if (ret) {

                        var gridDetail = $("#grdwrkOrderDetail").data("kendoGrid");
                        var arr = new Array();
                        $(checkedItems).each(function (idx, item) {
                            var row = $(item).closest("tr");
                            var dataItem = gridDetail.dataItem(row);
                            arr.push(dataItem);

                        });

                        $(arr).each(function (idx, item) {
                            var current = WO.getDuplicateInstance(item);
                            var Slist = current.SizeList;
                            WO.duplicateSingleSKU(current, Slist, gridDetail);
                        });

                    }// end confirmation
                });
            }
            else {
                ISS.common.showPopUpMessage('Please select at least one record.');
            }
        //}
        return false;
    },

    GetDuplicateSingle: function () {
        var size = null;
        var quantity = null;
        var qty = null;
        var sizecd = null;
        var gridDetail = $("#grdwrkOrderDetail").data("kendoGrid");
        var dataDs = gridDetail.dataSource._data;
        var checkedItems = $('.chckbx:checked');
       
        for (i = 0; i <= $("#tblDuplicate .clstr").length - 1; i++) {
            qty = $($("#tblDuplicate .clstr")[i]).find("input[type='text']").val();
            var s = qty.replace('-', '.');
            
           if (ISS.common.getEaches(s) == 10) {
                if (s.indexOf('.') == -1) s += '.';
                while (s.length < s.indexOf('.') + 3) s += '0';
                s = s.replace('.', '-');
                $($("#tblDuplicate .clstr")[i]).find("input[type='text']").val(s);
                //return false;
            }
           else if (ISS.common.getEaches(s) >= 12) {
               ISS.common.showPopUpMessage('Eaches must be less than 12.', null, function () {
                   $($("#tblDuplicate .clstr")[i]).find("input[type='text']").val(parseInt(s) + "-00");
               });
               return false;
           }

        }

        var Slist = [];
        for (i = 0; i <= $("#tblDuplicate .clstr").length - 1; i++) {
            size = $($("#tblDuplicate .clstr")[i]).find("td:first-child").text();
            quantity = $($("#tblDuplicate .clstr")[i]).find("input[type='text']").val().replace('-', '.');
            quantity = ISS.common.replaceAndCheck(quantity);
            sizecd = $($("#tblDuplicate .clstr")[i]).find("input[type='hidden']").val();
            if (size != null && quantity != "" && quantity != null && quantity != 0) {
                Slist.push({ SizeCD: sizecd, Size: size, Qty: quantity, Edited: false });
            }
        }

        if (Slist.length <= 0) {
            ISS.common.showPopUpMessage("Please enter quantity for at least one size.");
            return false;
        }


        
        var grid = $("#grdwrkOrderDetail").data("kendoGrid");
        $(checkedItems).each(function (idx, item) {
            var row = $(item).closest("tr");
            var dataItem = grid.dataItem(row);
           
            if (Slist.length > 1) {
                $(Slist).each(function (idy, sz) {
                    if (dataItem.SizeList[0].SizeCD == sz.SizeCD) {
                        dataItem.SizeList[0].Qty = sz.Qty;
                        dataItem.Dozens = sz.Qty;
                        Slist.splice(idy, 1);
                    }
                    
                });
            }
 
            var current = WO.getDuplicateInstance(dataItem);                   
            WO.duplicateSingleSKU(current, Slist, grid);
            WO.const.sizePopup.close();               
            
        });


       
    },

    DuplicateCancel: function () {
        WO.const.sizePopup.close();
    },

    loadMfgPathDetails: function () {
        if (WO.CreateOrder.mfgPathVal != this.value && !WO.CreateOrder.mfgLoaded) {
            $('#GarmentSKU').val('');
            $('#AttributionPath').val('');
            $("#GStyle").val('');
            $("#GColor").val('');
            $("#GAttribute").val('');
            $("#GSize").val('');
            var result = WO.retrieveColorData();

            var color = $('#ColorCode').data("kendoComboBox").value();
            var attr = $("#Attribute").data("kendoComboBox");
            WO.MultiSizeData();

            var demLoc = $("#PrimaryDC").val();
            result.ColorCode = color;
            result.Attribute = attr.text();
            result.SizeList = JSON.parse(JSON.stringify(WO.const.SKUSizeList));
            result.PrimaryDC = demLoc;
            result.MfgPathId = $("#MfgPathId").val(); 
            //var SizeList = WO.const.SKUSizeList;
            if ($('#SellingStyle').val() != '' && result.ColorCode != '' && result.Attribute != '' && result.SizeList.length > 0) {
                ISS.common.execute(WO.const.urlMfgPathDetail, result, function (s, data) {
                    if (s && data) {
                        if (data.length > 0) {
                            var d = data[0];
                            $('#MfgPathId').val(d.MfgPathId);
                            $('#GarmentSKU').val(d.GarmentSKU);
                            $('#AttributionPath').val(d.SewPlt);
                            $("#GStyle").val(d.GStyle);
                            $("#GColor").val(d.GColor);
                            $("#GAttribute").val(d.GAttribute);
                            $("#GSize").val(d.GSize);
                            $('#PackCode').focus();
                        }
                    }
                });
            }
        }
    },

    onAOStyleCodeClick: function (e) {
        currentTableRow = $(e).closest('tr');
        if (!e.value) {
            return;
        }
        //**********************
        style_cd = e.value.toUpperCase();
        if (WO.CreateOrder.popupValidator.validateInput(style_cd)) {
            WO.CreateOrder.popupValidator.validateInput(style_cd);
            if (WO.CreateOrder.StyleVal != style_cd) {
                var grid = $("#grdwrkOrderDetail").data("kendoGrid");
                if (grid) {
                    var postData = { SellingStyle: style_cd };
                    var selectedItem = grid.dataItem(currentTableRow)
                    selectrow = grid.dataItem(currentTableRow);
                    if (selectedItem) {
                        DataUid = selectedItem.uid;
                        selectedItem.Revision = "";
                        selectedItem.MfgPathId = "";
                        selectedItem.SewPlt = "";
                        selectedItem.AlternateId = "";
                        selectedItem.CutPath = "";
                        selectedItem.SelectedSizes = "";
                        selectedItem.GarmentSKU = "";
                        selectedItem.SellingStyle = style_cd;
                        selectedItem.PKGStyle = style_cd;
                        selectedItem.IsDirty = true;
                       
                    }
                }
                postData = JSON.stringify(postData); 
                ISS.common.executeActionAsynchronous(WO.const.urlAsrtCode, postData, function (stat, data) {
                    if (stat) {
                        if (data.length > 0) {
                            $("#AsrtCode").val(data[0].AssortCode);
                            asort_cd = data[0].AssortCode;
                            $("#PrimaryDC").val(data[0].PrimaryDC);
                            prm_cd = data[0].PrimaryDC;
                            $("#PackCode").val(data[0].PackCode).change();
                            PackCode_cd = data[0].PackCode;
                            if ($("#Dc").val() == '') {
                                $("#Dc").val(data[0].PrimaryDC).change();
                            }
                            $("#OriginTypeCode").val(data[0].OriginTypeCode);
                            orgin_Type = data[0].OriginTypeCode;
                            $("#BusinessUnit").val(data[0].CorpBusUnit);
                            corpbus_cd = data[0].CorpBusUnit;
                            $("#ProdFamCode").val(data[0].ProdFamCode);
                            prod_fam = data[0].ProdFamCode;
                            
                            var postedData = { Style: style_cd };
                            postedData = JSON.stringify(postedData);
                            ISS.common.executeActionAsynchronous(WO.const.urlRetrieve, postedData, WO.loadDescriptionAndColor);
                            //if (WO.CreateOrder.isUpdateCumOnChange) {
                            //    WO.updateCumulativeAndFabric();
                            //}

                        }

                    }

                    else {
                        //$('#SellingStyle').val('')
                        ISS.common.notify.error('Please provide a valid style.');
                    }

                });

                Focusedindex = 4;
            }
        }

        return false;
    },
    loadDescriptionAndColor: function (stat, data) {
        if (stat && data.length > 0) {
            var grid = $("#grdwrkOrderDetail").data("kendoGrid");
            var selectedItem = grid.dataItem(currentTableRow);
            selectedItem ? selectedItem : selectedItem = selectrow;
            cur_row ? cur_row: cur_row = selectrow;
            WO.const.SKUSizeList = [];
            if (selectedItem) {
                selectedItem.SellingStyle = "";
                //selectedItem.SellStyleDesc = "";
                //selectedItem.GroupId = 0;
                selectedItem.Id = "";
                //selectedItem.CreateBd = "";
                //selectedItem.DozensOnly = true;
                selectedItem.ColorCode = "";
                selectedItem.Attribute = "";
                selectedItem.Size = "";
                //Newly Added
                selectedItem.SizeCde = "";
                //selectedItem.SizeQty = 0;
                selectedItem.Revision = "";
                selectedItem.MfgPathId = "";
                selectedItem.PKGStyle = "";
                selectedItem.AlternateId = "";
                selectedItem.CutPath = "";
                selectedItem.SewPlt = "";
                selectedItem.AttributionPath = "";
                selectedItem.TotalDozens = "";
                selectedItem.Dozens = "";
                selectedItem.Lbs = "";
                selectedItem.PackCode = "";
                selectedItem.CategoryCode = "";
                selectedItem.PriorityCode = "";
                selectedItem.BodyTrim = "";
                selectedItem.CylinderSizes = "";
                selectedItem.Note = "";
                selectedItem.ActualLbs = "";
                selectedItem.AssortCode = "";
                selectedItem.PurchaseOrder = "";
                selectedItem.LineItem = "";
                selectedItem.DemandDriver = "";
                selectedItem.ErrorMessage = "";
                selectedItem.SellingStyle = style_cd;
                selectedItem.Id = id_cd;
                selectedItem.ColorCode = data[0].Color;
                //selectedItem.SellStyleDesc = data[0].StyleDesc;
                selectedItem.Attribute = data[0].Attribute;
                selectedItem.Size = data[0].Size;
                //Newly Added
                selectedItem.SizeCde = data[0].SizeShortDes;
                selectedItem.Revision = data[0].Rev;
                //selectedItem.SizeLit = data[0].SizeShortDes;
                selectedItem.PKGStyle = style_cd;
                PackCode_cd = PackCode_cd;
                PriorityCode_cd  = PriorityCode_cd;

                selectedItem.PackCode = PackCode_cd;
                selectedItem.PriorityCode = PriorityCode_cd;
                style_cd = style_cd;
                color_cd = data[0].Color
                attr_cd = data[0].Attribute
                size_cd = data[0].Size
                size_desc = data[0].SizeShortDes;
                rev_cd = data[0].Rev;
                WO.const.SKUSizeList = [{ Size: data[0].SizeShortDes, SizeCD: size_cd }];
                //WO.getChildSKU();
                //var postedData = { Style_Cd: style_cd, Color_Cd: color_cd, Attribute_Cd: attr_cd, SizeList: WO.const.SKUSizeList, DemLoc_Cd: prm_cd };
                //postedData = JSON.stringify(postedData);
                //ISS.common.executeActionAsynchronous("../order/GetMFGId", postedData, function (stat, data) {
                //    if (stat && data) {
                //        if (data.length > 0) {//data[0].SewPlt
                //            MfgPathId_cd = data[0].MfgPathId;
                //            SewPlt_cd = data[0].SewPlt;
                //            selectedItem.MfgPathId = data[0].MfgPathId;
                //            selectedItem.SewPlt = data[0].SewPlt;
                //            $("#grdWorkOrderDetail").data("kendoGrid").refresh();

                //        }
                //    }

                //});
                selectedItem.ErrorStatus = data[0].ErrorStatus;
                if (selectedItem.ErrorStatus == true) {
                    $('tr[data-uid="' + selectedItem.uid + '"]').addClass("highlighted-row");
                }
                else {
                    $('tr[data-uid="' + selectedItem.uid + '"]').removeClass("highlighted-row");
                }
                $("#grdwrkOrderDetail").data("kendoGrid").refresh();
                flag = 0;
            }
        }
        else {
            var grid = $("#grdwrkOrderDetail").data("kendoGrid");
            var selectedItem = grid.dataItem(currentTableRow)
            if (selectedItem) {
                selectedItem.SellingStyle = "";
                selectedItem.Color = "";
                selectedItem.Description = "";
                selectedItem.Attribute = "";
                selectedItem.Size = "";

                $("#grdwrkOrderDetail").data("kendoGrid").refresh();
            }

            ISS.common.notify.error('Failed to retrieve Style details.');
        }
    },
    onColorChanged: function (e) {
        var dat = e.sender.dataSource.data();
        var dataItem = dat[e.sender.selectedIndex];
        if (dataItem)
            color_cd = dataItem.Color;
        var grid = $("#grdwrkOrderDetail").data("kendoGrid");
        if (grid) {
            var postData = { styleCode: style_cd, colorCode: color_cd };
            postData = JSON.stringify(postData);
            ISS.common.executeActionAsynchronous("../../order/GetAOAttributeInfo", postData, WO.loadAttributeInfo);

            Focusedindex = 4;
            var currentSelection = grid.select().parent();
            if (currentSelection) {
                currentTableRow = currentSelection;
                var rowData = grid.dataItem(currentSelection);
                rowData ? rowData : rowData = selectrow;
                if (rowData) {
                    DataUid = rowData.uid;
                }
            }

        }
    },
    loadAttributeInfo: function (stat, data) {
        if (stat && data.length > 0) {
            var grid = $("#grdwrkOrderDetail").data("kendoGrid");
            var currentSelection = grid.select().parent();
            WO.const.SKUSizeList = [];
            WO.const.SKUSizeList = [{ Size: data[0].SizeShortDes, SizeCD: data[0].Size }];
            size_desc = data[0].SizeShortDes;
            size_cd = data[0].Size;
            var selectedItem = grid.dataItem(currentSelection)
            selectedItem ? selectedItem : selectedItem = selectrow;
            if (selectedItem) {
                
                selectedItem.Attribute = "";
                selectedItem.Size = "";
                selectedItem.SizeQty = 0;
                selectedItem.Revision = "";
                selectedItem.MfgPathId = "";
                selectedItem.PKGStyle = "";
                selectedItem.AlternateId = "";
                selectedItem.CutPath = "";
                selectedItem.SewPlt = "";
                selectedItem.AttributionPath = "";
                selectedItem.TotalDozens = "";
                selectedItem.Dozens = "";
                selectedItem.Lbs = "";
                selectedItem.PackCode = "";
                selectedItem.CategoryCode = "";
                selectedItem.PriorityCode = "";
                selectedItem.BodyTrim = "";
                selectedItem.CylinderSizes = "";
                selectedItem.Note = "";
                selectedItem.ActualLbs = "";
                selectedItem.AssortCode = "";
                selectedItem.PurchaseOrder = "";
                selectedItem.LineItem = "";
                selectedItem.DemandDriver = "";
                selectedItem.ErrorMessage = "";
                rev_cd = data[0].Rev;
                attr_cd = data[0].Attribute;
                size_cd = data[0].Size;
                //size_desc = data[0].SizeShortDes;
                selectedItem.ColorCode = color_cd
                selectedItem.Attribute = data[0].Attribute;
                selectedItem.Size = data[0].Size;
                selectedItem.SizeCde = data[0].SizeShortDes;
                selectedItem.PKGStyle = style_cd;
                selectedItem.Revision = data[0].Rev;
                rev_cd = data[0].Rev;
                PackCode_cd = PackCode_cd;
                PriorityCode_cd = PriorityCode_cd;

                selectedItem.PackCode = PackCode_cd;
                selectedItem.PriorityCode = PriorityCode_cd;
                
                if (selectedItem.ErrorStatus == true) {
                    $('tr[data-uid="' + selectedItem.uid + '"]').addClass("highlighted-row");
                }
                else {
                    $('tr[data-uid="' + selectedItem.uid + '"]').removeClass("highlighted-row");
                }
                grid.refresh();
                //requisitions.validateRequisitionDetailRow(selectedItem, setgridValidateRefresh)
            }
        }
        else {
            var grid = $("#grdwrkOrderDetail").data("kendoGrid");
            var currentSelection = grid.select().parent();
            var selectedItem = grid.dataItem(currentSelection)
            selectedItem ? selectedItem : selectedItem = selectrow;
            if (selectedItem) {
                selectedItem.Attribute = "";
                selectedItem.Size = "";

                $("#grdwrkOrderDetail").data("kendoGrid").refresh();
            }
            ISS.common.notify.error('Failed to retrieve Style details.');
        }
    },
    onAttributeSelect: function (e) {
        var dataItem = this.dataItem(e.item.index());
        attr_cd = dataItem.Attribute;

        var grid = $("#grdwrkOrderDetail").data("kendoGrid");
        if (grid) {
            var postData = { styleCode: style_cd, colorCode: color_cd, attributeCode: attr_cd };
            postData = JSON.stringify(postData);
            ISS.common.executeActionAsynchronous("../order/GetSizeInfo", postData, WO.loadSizeInfo);
        }


    },

    onAttributeChange: function (e) {
        var dat = e.sender.dataSource.data();
        var dataItem = dat[e.sender.selectedIndex];
        if (dataItem)
            attr_cd = dataItem.Attribute;

        var grid = $("#grdwrkOrderDetail").data("kendoGrid");
        if (grid) {
            var postData = { styleCode: style_cd, colorCode: color_cd, attributeCode: attr_cd };
            postData = JSON.stringify(postData);
            ISS.common.executeActionAsynchronous("../../order/GetSizeInfo", postData, WO.loadSizeInfo);
        }

        Focusedindex = 5;
        var currentSelection = grid.select().parent();
        if (currentSelection) {
            currentTableRow = currentSelection;
            var rowData = grid.dataItem(currentSelection);
            rowData ? rowData : rowData = selectrow;
            if (rowData) {
                DataUid = rowData.uid;
            }
        }
    },
    loadSizeInfo: function (stat, data) {
        if (stat && data.length > 0) {
            var grid = $("#grdwrkOrderDetail").data("kendoGrid");
            var currentSelection = grid.select().parent();
            var selectedItem = grid.dataItem(currentSelection)
            selectedItem ? selectedItem : selectedItem = selectrow;
            WO.const.SKUSizeList = [{ Size: data[0].SizeShortDes, SizeCD: data[0].Size }];
            if (selectedItem) {
                selectedItem.Size = "";
                //selectedItem.SizeQty = 0;
                selectedItem.Revision = "";
                //selectedItem.DozensOnly = true;
                selectedItem.MfgPathId = "";
                selectedItem.Size = data[0].Size;
                //Newly Added
                selectedItem.SizeCde = data[0].SizeShortDes;
                selectedItem.PKGStyle = style_cd;
                selectedItem.AssortCode = asort_cd;
                selectedItem.Revision = data[0].Rev;
                rev_cd = data[0].Rev;
                style_cd = style_cd;
                size_cd = data[0].Size
                size_desc = data[0].SizeShortDes;
                PackCode_cd = PackCode_cd;
                PriorityCode_cd = PriorityCode_cd;

                selectedItem.PackCode = PackCode_cd;
                selectedItem.PriorityCode = PriorityCode_cd;
                
                WO.const.SKUSizeList = [{ Size: size_desc, SizeCD: size_cd }];
                if (selectedItem.ErrorStatus == true) {
                    $('tr[data-uid="' + selectedItem.uid + '"]').addClass("highlighted-row");
                }
                else {
                    $('tr[data-uid="' + selectedItem.uid + '"]').removeClass("highlighted-row");
                }
                $("#grdwrkOrderDetail").data("kendoGrid").refresh();
                //requisitions.validateRequisitionDetailRow(selectedItem, setgridValidateRefresh)
            }
        }
        else {
            var grid = $("#grdwrkOrderDetail").data("kendoGrid");
            var currentSelection = grid.select().parent();
            var selectedItem = grid.dataItem(currentSelection)
            selectedItem ? selectedItem : selectedItem = selectrow;
            if (selectedItem) {
                selectedItem.Size = "";
                selectedItem.Rev = "";
                selectedItem.Uom = "";
                selectedItem.Qty = "";
                selectedItem.StdCase = "";
                selectedItem.Dpr = "";
                $("#grdwrkOrderDetail").data("kendoGrid").refresh();
            }
            ISS.common.notify.error('Failed to retrieve Style details.');
        }
    },
    onSizeSelected: function (e) {
            var grid = $("#grdwrkOrderDetail").data("kendoGrid");
        //var dataItem = this.dataItem(e.item.index());
        var dat = e.sender.dataSource.data();
        var dataItem = dat[e.sender.selectedIndex];
        WO.const.SKUSizeList = [{ Size: dataItem.SizeDesc, SizeCD: dataItem.Size }];
        if (dataItem) {
            size_desc = dataItem.SizeDesc;
            size_cd = dataItem.Size;
        }
        var currentSelection = grid.select().parent();
        var selectedItem = grid.dataItem(currentSelection)
        selectedItem ? selectedItem : selectedItem = selectrow;
        //WO.const.SKUSizeList = [{ Size: data[0].SizeShortDes, SizeCD: data[0].Size }];
        if (selectedItem) {
            selectedItem.Size = size_cd;
            //selectedItem.SizeQty = 0;
            selectedItem.SizeCde = size_desc;
        }
        var postData = WO.retrieveColorDataIn();
        //var colors = $("#ColorCode").data("kendoMultiSelect").value();
        //var ColorCode = WO.getMultiselectItem(colors);
        var ColorCode = color_cd;
        var Attribute = attr_cd;
        WO.MultiSizeData();

        if (ColorCode != '' && Attribute != '' && WO.const.SKUSizeList.length > 0) {

            postData.ColorCode = ColorCode;
            postData.Attribute = Attribute;

            postData.SizeList = WO.const.SKUSizeList;
            $("#AsrtCode").val(asort_cd) 
            postData.AssortCode = $("#AsrtCode").val();
            //if ($.trim($("#Revision").val()) == '') {
                pData = JSON.stringify(postData);
                ISS.common.executeActionAsynchronous(WO.const.urlGetRevision, pData, function (stat, data) {
                    if (stat) {
                        if (data.length > 0) {
                            rev_cd = data[0].Revision;
                            $("#Revision").val(data[0].Revision);
                            WO.getChildSKUInline();
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

        Focusedindex = 8;
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

    
    onRowsAOSelected: function (arg) {
        var grid = $("#grdwrkOrderDetail").data("kendoGrid");
        var currentSelection = grid.select().parent();
        if (currentSelection) {
            currentTableRow = currentSelection;
            var rowData = grid.dataItem(currentSelection);
            if (rowData) {
                style_cd = rowData.SellingStyle;
                color_cd = rowData.ColorCode;
                attr_cd = rowData.Attribute;
                size_cd = rowData.Size;
                SizeList_cd = WO.const.SKUSizeList;
                rev_cd = rowData.Revision;
                pkgstyle = rowData.PKGStyle;
                TotalDozens_cd = rowData.TotalDozens;
                //Dozens_cd = rowData.Dozens;
                Lbs_cd = rowData.Lbs;
                size_qty = rowData.DozenStr;
                MfgPathId_cd = rowData.MfgPathId;
                PackCode_cd = rowData.PackCode;
                rowData.IsDirty = true;
                DataUid = rowData.uid;
            }
            

        }
    },

};


$.extend(WO, MSKUWO);