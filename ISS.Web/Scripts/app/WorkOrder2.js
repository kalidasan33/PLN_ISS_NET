var style_cd = null;
var color_cd = null;
var attr_cd = null;
var size_cd = null;
var size_desc = null;
var id_cd = null;
var asort_cd = null;
var corpbus_cd = null;
var pkgstyle = null;
var prm_cd = null;
var orgin_cd = null;
var rev_cd = null;
var MfgPathId_cd = null;
var SewPlt_cd = null;
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
var totalTextQty =  null;
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

MSKUWO = {

    const: {
        sizePopup: null,
        woCurrentRow: null,
        SKUSizeList: [],
        SizeValidator: null,
        columns: null,
    },

    docWOReady2: function (IsLoad) {



        $('#btnWOSizeSave').bind('click', WO.onMultiInLineSizeSave);

        $('#btnWorkOrderSave').bind('click', WO.InsertMultiSKU);
        //$("#Revision").bind('focusout', WO.focusFun);
        $('#AlternateId').bind('click', WO.showAlternateId);
        $('#AlternateId').bind('focusout', WO.loadAltIdDtls);
        $('#CutPath').bind('click', WO.PopulateCutPathTxtPath);
        $('#CutPath').bind('focusout', WO.loadCutPathDtls);

        $('#CutPath').on('keypress', function (e) {
            if (e.keyCode == 13) {
                WO.PopulateCutPathTxtPath();
                return false;
            }
           
         });

        $('.k-grid-btnWODuplicate').bind('click', WO.orderDuplicate);

        $('.k-grid-addNew').bind('click', WO.showWOEditor);
        $('.k-grid-add').bind('click', WO.showWOEditorVal);

        $("#grdWorkOrderDetail").on('click','.chkbx', function () {
            if($(this).prop('checked'))
                $('.chkbx:checked').not(this).prop('checked', false);
        })
        WO.const.SizeValidator = $('#frmSizePopup').kendoValidator().data("kendoValidator");
        

        var toolTipNote = $('#grdWorkOrderDetail').kendoTooltip({
            filter: ".cnote",
            content: function (e) {
                var Note = '';
                var dataItem = $("#grdWorkOrderDetail").data("kendoGrid").dataItem(e.target.closest("tr"));
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
            if(grid) grid.select().removeClass('k-state-selected')
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
        $('.k-grid-cancel-changes').on('keydown', function (e) {

            if (e.keyCode == 9) {
                $('#SellingStyle').focus();
                return false;
            }

        });
        
        $("#TotalDozens").focusin(function () {
            WO.CreateOrder.TotalDz = $('#frmWoEditor #TotalDozens').val();
        });

        $('#TotalDozens').on('focusout', function () {
            if (WO.CreateOrder.TotalDz != this.value) {
                WO.resetSizeQty();
                return false;
            }
        });

    },//onSizeCodeClick retrieveDemand
    
    setgridValidateRefresh: function (data, row) {
        $("#grdWorkOrderDetail").data("kendoGrid").refresh();
    },
    loadAfterMfgpath: function () {
       
    },
    
    onWOStyleCodeClick2: function (e) {
        currentTableRow = $(e).closest('tr');
        if (!e.value) {
            return;
        }
        style_cd = e.value.toUpperCase()
        var grid = $("#grdWorkOrderDetail").data("kendoGrid");
        if (grid) {
            var postData = { Style: style_cd};

            var selectedItem = grid.dataItem(currentTableRow)
            if (selectedItem) {
                selectedItem.SellingStyle = style_cd;
                selectedItem.IsDirty = true;
            }
            postData = JSON.stringify(postData);
            ISS.common.executeActionAsynchronous("../order/GetValidateWOStyle", postData, function (stat, data) {
                if (stat && data) {
                    if (data.length > 0) {
                        $('#Dc').val(data[0].PrimaryDC);
                        prm_cd = data[0].PrimaryDC;
                        $("#AssortCode").val(data[0].AssortCode);
                        $("#PrimaryDC").val(data[0].PrimaryDC);
                        $("#frmWOMEdit #PackCode").val(data[0].PackCode);
                        $("#Dc").val(data[0].PrimaryDC);
                        $("#OriginTypeCode").val(data[0].OriginTypeCode);
                        $("#BusinessUnit").val(data[0].CorpBusUnit); asort_cd = data[0].AssortCode;
                        corpbus_cd = data[0].CorpBusUnit;
                        //ISS.common.executeActionAsynchronous("../order/GetRevDetails", postData, WO.loadDescriptionAndColor);
                        ISS.common.executeActionAsynchronous("../order/GetRetrieve", postData, WO.loadDescriptionAndColor);
                        
                    }
                    else {
                        selectedItem.SellingStyle = ''; grid.refresh()
                        ISS.common.notify.error('Invalid Style -' + style_cd);
                    }
                }
                else {
                    selectedItem.SellingStyle = ''; grid.refresh()
                    ISS.common.notify.error('Invalid Style -' + style_cd);
                }
            });
        }
        //setTimeout(function () {
        //    var cell = $('#grdWorkOrderDetail').find('tbody tr:eq(' + 6 + ') td:eq(6)'); // or different cell
        //    cell.focus();
        //    $("#grdWorkOrderDetail").data("kendoGrid").refresh();
        //}, 500)
        
        return true;
    },
    
    showWOEditorVal: function (e) {
        $("#gridMode").val('add');
        WO.clearWOdetail();
        //$("#gridMode").val('add');
        //var gridCum = $("#grdwrkOrderCumulative").data("kendoGrid");
        //var gridFab = $("#grdwrkOrderFabric").data("kendoGrid");
        //WO.CreateOrder.editDataCum = gridCum.dataSource.data();
        //WO.CreateOrder.editDataFab = gridFab.dataSource.data();
        //var orderId = WO.getRandomId(1, 999);
        //$("#OrderDetailId").val(orderId);

        //var dsColor = $("#ColorCode").data("KendoComboBox");
        ////dsColor.dataSource.read();
        //var ds = $('#ColorCode').data().kendoComboBox.dataSource;
        //ds.read();
        //var dsAttr = $("#Attribute").data("kendoComboBox");
        //var dsAttribute = $('#ColorCode').data().kendoComboBox.dataSource;
        //dsAttribute.read();
        //$("#DozensOnly").prop('checked', true);
        //$('#CreateBd').prop('checked', false);
        ////$("#myDiv").show();

        //WO.CreateOrder.editPopUp = ISS.common.popUp('#myDiv', 'Edit', null, function (rr) {

        //    if (rr.userTriggered) {
        //        rr._defaultPrevented = true;
        //        ISS.common.showConfirmMessage('Pending changes will be lost.<br/> Do you want to continue by losing your changes?', null, function (reply) {
        //            if (reply) {

        //                WO.CreateOrder.editPopUp.close();
        //                WO.ResetCumFabData(true);
        //            }

        //        });
        //    }
        //});


        //WO.CreateOrder.currentRow = null;
        //$("#SellingStyle").focus();
        
        var orderId = WO.getRandomId(1, 999);
        $("#OrderDetailId").val(orderId);
        id_cd = $("#OrderDetailId").val();

    },
    
        
    getWODetailObjectGrid: function (obj) {
       
        var gridCum = $("#grdwrkOrderCumulative").data("kendoGrid");
        var gridFab = $("#grdwrkOrderFabric").data("kendoGrid");
        var gridWODet = $("#grdWorkOrderDetail").data("kendoGrid");
        var dataCum = gridCum.dataSource.data();
        var dataFab = gridFab.dataSource.data();
        var dataWODet = gridWODet.dataSource.data();
        var colors = color_cd;
        var color = WO.getMultiselectItem(colors);
        obj["Id"] = id_cd;
        PriorityCode_cd = 50;
        //obj["DozensOnly"] = $('#DozensOnly').val();
        //var grid = $("#grdWorkOrderDetail").data("kendoGrid");
        //var currentSelection = grid.select().parent();
        //var selectedItem = grid.dataItem(currentSelection)
        //if (selectedItem) {
        //    if (selectedItem.DozensOnly == true) {
        //        obj["DozensOnlyInd"] = "Y";
        //        obj["CreateBDInd"] = "N";
        //    }
        //    if (selectedItem.CreateBd == true) {
        //        obj["CreateBDInd"] = "Y";
        //        obj["DozensOnlyInd"] = "N";
        //    }
        //    obj["DozensOnly"] = $('#DozensOnly').val();
        //}
        if (Crea_cd == true) {
            obj["CreateBDInd"] = "Y";
            obj["DozensOnlyInd"] = "N";
        }
        if (Dozens_cd == true) {
            obj["DozensOnlyInd"] = "Y";
            obj["CreateBDInd"] = "N";
        }

        obj["DozensOnly"] = $('#DozensOnly').val();

        obj["DozensOnly"] = Dozens_cd;

        obj["SellingStyle"] = style_cd;
        obj["ColorCode"] = color_cd;
        obj["ColorDyeCode"] = color_cd;
        obj["AttributeCompCode"] = attr_cd;
        obj["Attribute"] = attr_cd;
        obj["Size"] = size_cd;
        obj["Revision"] = rev_cd;
        obj["PKGStyle"] = pkgstyle;
        obj["MfgPathId"] = MfgPathId_cd;
        obj["SewPlt"] = SewPlt_cd;
        obj["PackCode"] = PackCode_cd;
        obj["NewStyle"] = $("#NewStyle").val();
        obj["NewColor"] = $("#NewColor").val();
        obj["NewSize"] = $("#NewSize").val();
        obj["NewAttribute"] = $("#NewAttribute").val();
        obj["AlternateId"] = AlternateId_cd;
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
        obj["PriorityCode"] = PriorityCode_cd;
        obj["WODetail"] = dataWODet;
        obj["GridMode"] = $("#gridMode").val();
        obj["BodyTrim"] = $("#BodyTrim").val();
        //obj["CutPath"] = $("#CutPath").val();
        obj["CutPath"] = $("#CutPath").val().toUpperCase();
        obj["AttributionPath"] = $("#AttributionPath").val();
        obj["CategoryCode"] = $('#CategoryCode').val();
        obj["ErrorMessage"] = '';
        obj["ErrorStatus"] = false;
        obj["LimitLbs"] = $("#Limit").val();
        obj["VarianceQty"] = $("#VarianceQty").val();
        obj["ActualLbs"] = $("#VarianceQty").val();
        obj["PrimaryDC"] = prm_cd;
        obj["AssortCode"] = asort_cd;

        return obj;

    },
    onSizeCodeAsign: function (e) {
        ////var dataItem = this.dataItem(e.item.index());
        ////WO.const.SKUSizeList = [{ Size: dataItem.Size, SizeCD: dataItem.Size }];
        ////size_cd = dataItem.Size;
        //var val = size_qty;
        //var dataItem = this.dataItem(e.item.index());
        ////var myString = val.substr(val.indexOf("-") + 1)
        ////if (myString != 0) {
        ////    WO.const.SKUSizeList = [{ Size: size_cd, SizeCD: size_desc, Qty: myString }];
        ////}
        ////else
        ////    ISS.common.showPopUpMessage('Please enter the Qty.');
        //if (val != 0 && val != null) {
        //    WO.const.SKUSizeList = [{ Size: size_cd, SizeCD: size_desc, Qty: size_qty }];
        //}
        //else
        //    ISS.common.showPopUpMessage('Please enter the Qty.');
    },

    loadRevisionAndMFGInfo: function (stat, data) {
        if (stat && data.length > 0) {
            var grid = $("#grdWorkOrderDetail").data("kendoGrid");
            var currentSelection = grid.select().parent();
            var selectedItem = grid.dataItem(currentSelection)
            var Size_List = WO.const.SKUSizeList;
            if (selectedItem) {
                selectedItem.Revision = data[0].Revision;
                rev_cd = data[0].Revision;
                var postedData = { Style_Cd: style_cd, Color_Cd: color_cd, Attribute_Cd: attr_cd, SizeList: Size_List, DemLoc_Cd: prm_cd };
                postedData = JSON.stringify(postedData);
                ISS.common.executeActionAsynchronous("../order/GetMFGId", postedData, function (stat, data) {
                    if (stat && data) {
                        if (data.length > 0) {//data[0].SewPlt
                            selectedItem.MfgPathId = data[0].MfgPathId;
                            selectedItem.SewPlt = data[0].SewPlt;
                            //var result = WO.ValidateHAAInLine();
                            //WO.HideFieldInline(result);

                            MfgPathId_cd = data[0].MfgPathId;
                            SewPlt_cd = data[0].SewPlt;
                            currWO = WO.getWODetailObjectGrid(new WorkOrderDetail());
                            var postDataVal = { WO: currWO };
                            postDataVal = JSON.stringify(postDataVal);
                            ISS.common.executeActionAsynchronous("../order/CalculateCumulativeAndFabric", postDataVal, function (stat, resData) {
                                if (stat && resData) {
                                    PackCode_cd = resData.dataCum.PackCode;
                                    //NewStyle_cd
                                    //NewColor_cd
                                    //NewSize_cd
                                    //NewAttribute_cd
                                    AlternateId_cd = resData.dataCum.AlternateId;
                                    CylinderSizes_cd = resData.dataCum.CylinderSizes;
                                    //GroupId_cd
                                    //SizeList_cd
                                    //TotalDozens_cd
                                    //Dozens_cd
                                    cutpath_cd = resData.dataCum.CutPath;
                                    Lbs_cd = resData.dataCum.Lbs;
                                    //Note_cd
                                    SewPlt_cd = resData.dataCum.SewPlt
                                    //var AltId = resData.dataCum.AlternateId;
                                    //AlternateId = resData.dataCum.AlternateId;
                                    VarianceQty_cd = resData.dataCum.VarianceQty
                                    //AlternateId_cd = resData.dataCum.AlternateId;
                                    selectedItem.AlternateId = AlternateId_cd;
                                    selectedItem.CylinderSizes = CylinderSizes_cd;
                                    selectedItem.Lbs = Lbs_cd;
                                    selectedItem.PackCode = PackCode_cd;
                                    selectedItem.CutPath = cutpath_cd;
                                    selectedItem.Lbs = Lbs_cd;
                                    selectedItem.PriorityCode = PriorityCode_cd;
                                    if (totalTextQty != null) {
                                        selectedItem.TotalDozens = totalTextQty;
                                        selectedItem.Dozens = totalTextQty;
                                    }
                                    else {
                                        selectedItem.TotalDozens = size_qty;
                                        selectedItem.Dozens = size_qty;
                                    }
                                    $("#grdWorkOrderDetail").data("kendoGrid").refresh();
                                }
                            });
                            $("#grdWorkOrderDetail").data("kendoGrid").refresh();
                            
                        }
                    }

                });
                
                WO.getChildSKU();
               
            }
           
        }
    },

    loadStdCaseInfo: function (e) {
        
    },
    
    
    readDetailData: function () {
        return { Id: 1 };
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
            var checkedItems = $('.chkbx:checked');
            //if ($("#gridMode").val() == '') {
                if (checkedItems.length > 0) {
                    var arr = new Array();
                    var grid = $("#grdWorkOrderDetail").data("kendoGrid");
                    $(checkedItems).each(function (idx, item) {
                        var row = $(item).closest("tr");
                        var dataItem = grid.dataItem(row);
                        dataItem.SizeList = WO.const.SKUSizeList;
                        dataItem.ErrorStatus = false
                        arr.push(dataItem)
                    })
                    //$(dsWOData).each(function (i, ite) { ite.ErrorStatus = false; ite.SizeList = WO.const.SKUSizeList; });
                    var current = WO.getDuplicateInstance(arr[0]);


                    WO.showSizeInLinePopup(JSON.stringify(current.SizeList), '', current.SellingStyle, current.ColorCode, current.Attribute, function (SkuList, orderType) {
                        var Slist = JSON.parse(JSON.stringify(SkuList));
                        //grid.dataSource.data().push(current)
                        if (SkuList.length > 1) {
                            if (orderType != 'Single Order(s)') {
                                var totQty = 0;
                                for (var i = 0; i < Slist.length; i++) {
                                    totQty += Slist[i].Qty;
                                }
                                current.SizeList = Slist;
                                current.SizeQty = WO.getSizeDisplay(Slist)
                                current.TotalDozens = totQty;
                                current.Dozens = totQty;

                                ISS.common.executeActionAsynchronous("GetGroupID", null, function (stat, data) {
                                    if (stat && data) {
                                        current.GroupId = data;
                                        grid.dataSource.data().push(current)
                                        WO.duplicateCompleted(grid, current)
                                    }
                                });
                            } //end sinle sku check
                            else {
                                WO.duplicateSingleSKU(current, Slist, grid);
                            }
                        }
                        else {
                            current.SizeList = Slist;
                            current.Size = current.SizeList[0].SizeCD;
                            current.SizeLit = current.SizeList[0].Size;   //Added By:UST(Gopikrishnan)
                            current.SizeQty = Slist[0].Qty;
                            current.TotalDozens = Slist[0].Qty;
                            current.Dozens = Slist[0].Qty;
                            grid.dataSource.data().push(current)
                            WO.duplicateCompleted(grid, current)
                        }


                    }, 'Duplicate');
                }
                else {
                    ISS.common.notify.error('Please select at least one row.')
                }
            //}
            //else {
            //    ISS.common.notify.error('You have pending changes. Please save the changes first.')
            //}
        
        return false;
    },

    duplicateSingleSKU: function (current, Slist, grid) {
        if (Slist.length > 0) {
            current = WO.getDuplicateInstance(current)
            current.SizeList = Slist.splice(0, 1);
            current.Size = current.SizeList[0].SizeCD;
            current.SizeLit = current.SizeList[0].Size;   //Added By:UST(Gopikrishnan)
            current.SizeQty = current.SizeList[0].Qty;
            current.TotalDozens = current.SizeList[0].Qty;
            current.Dozens = current.SizeList[0].Qty;
            grid.dataSource.data().push(current)
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
        current = JSON.parse(JSON.stringify(current));
        var gridCum = $("#grdwrkOrderCumulative").data("kendoGrid");
        var gridFab = $("#grdwrkOrderFabric").data("kendoGrid");
       //cbl
       // current.WOCumulative = gridCum.dataSource.data();
        // current.WOFabric = gridFab.dataSource.data();
        //current.WODetail = grid.dataSource.data();
        current.WODetail = new Array();
        current.WODetail.push(JSON.parse(JSON.stringify(current)))
       
        current["OrderCount"] = $("#OrdersToCreate").data("kendoNumericTextBox").value();
        WO.updateCumulativeAndFabric(current, function () {        
            grid.refresh();
           // if (callback) callback(); //cbl
        }, current.WODetail[current.WODetail.length - 1], true);
        if (callback) callback();
    },
    woSizePopupClick: function (e) {
        var color = $("#ColorCode").data("kendoComboBox").value();
        var attr = $("#Attribute").data("kendoComboBox").text();
        var style = $('#SellingStyle').val();
        if (color != '' && attr != '' && style != '') {
            WO.showSizePopup($("#MultiSizeList").val(),'' ,style, color, attr)
        }
        else {
            ISS.common.notify.error('Please select style, color and attribute.');
        }
       
        return false;
    },
    woSizeInLinePopupClick: function (e) {
        var grid = $("#grdWorkOrderDetail").data("kendoGrid");
        var currentSelection = grid.select().parent();
        if (currentSelection) {
            currentTableRow = currentSelection;
            var rowData = grid.dataItem(currentSelection);
            rowData ? rowData : rowData = selectrow;
        }
        var color = color_cd;
        var attr = attr_cd;
        var style = style_cd;
        if (color != '' && attr != '' && style != '') {
            if (rowData.SizeList) {
                WO.showSizeInLinePopup(rowData.SizeList, '', style, color, attr)
            }
            else {
                WO.showSizeInLinePopup($("#MultiSizeList").val(), '', style, color, attr)
            }
            //WO.showSizeInLinePopup(rowData.SizeList, '', style, color, attr)
        }
        else {
            ISS.common.notify.error('Please select style, color and attribute.');
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
                    WO.const.SKUSizeList.push({ SizeCD: myList[j].SizeCD, Size: myList[j].Size, Qty: myList[j].Qty, Edited: false });
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

        WO.const.sizePopup = ISS.common.popUp('.divSizesPopup', 'Size List', null, function () {
            setTimeout(function () {
                $("#frmWoEditor #btnWOSizes").focus();
            }, 0)          
        });
        WO.const.sizePopup.settings = settings;
        grid.dataSource.read();
        
    },
    showSizeInLinePopup: function (skulst, remove, style, color, attr, handler, mode) {
        var grid = $("#grdMultiSKUSize").data("kendoGrid");
        if (skulst != '') {
            var myList = JSON.parse(skulst);
            if (myList != null) {
                WO.const.SKUSizeList = [];
                for (var j = 0; j < myList.length; j++) {
                    WO.const.SKUSizeList.push({ SizeCD: myList[j].SizeCD, Size: myList[j].Size, Qty: myList[j].Qty, Edited: false });
                }
            }
            //grid.dataSource.data(myList);
        }


        var settings = { style: style, color: color, attr: attr, handler: handler, mode: mode };
        WO.const.sizePopup = {};
        WO.const.sizePopup.settings = settings;
        var th = $('#SizeOrderType');
        if (mode && mode == 'Duplicate') {
            var ordTyp = th.data('kendoDropDownList');
            ordTyp.enable(true)
            ordTyp.value(null)
            th.closest(".k-widget").show()
        }
        else {
            th.data('kendoDropDownList').enable(false);
            th.closest(".k-widget").hide()
        }

        WO.const.sizePopup = ISS.common.popUp('.divInLineSizesPopup', 'Size List', null, function () {
            setTimeout(function () {
                $("#frmWoEditor #btnWOSizes").focus();
            }, 0)
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
            if (WO.const.sizePopup.settings.mode == 'Duplicate') {
                color = WO.const.sizePopup.settings.color;
                attr = WO.const.sizePopup.settings.attr;
                style = WO.const.sizePopup.settings.style;
            }
            else {
                var colors = $("#ColorCode").data("kendoComboBox").value();
                //  color = $("#ColorCode").data("kendoMultiSelect").value();
                color = WO.getMultiselectItem(colors);
                attr = $("#Attribute").data("kendoComboBox").text();
                 style = $('#SellingStyle').val();
            }
        var sizelst = WO.const.SKUSizeList;
        if (color != '' && attr != '' && style != '') {            
            var searchCriteria = {
                Style_Cd: style,
                Color_Cd: color,
                Attribute_Cd: attr,
                SizeList: sizelst
            };

            return searchCriteria;
        }
        return false;
    },
    multiInLineSizeLoad: function () {
        var color = '';
        var attr = '';
        var style = '';
        if (WO.const.sizePopup.settings.mode == 'Duplicate') {
            color = WO.const.sizePopup.settings.color;
            attr = WO.const.sizePopup.settings.attr;
            style = WO.const.sizePopup.settings.style;
        }
        else {
            var colors = color_cd;
            //  color = $("#ColorCode").data("kendoMultiSelect").value();
            color = WO.getMultiselectItem(colors);
            attr = attr_cd;
            style = style_cd;
        }
        var sizelst = WO.const.SKUSizeList;
        if (color != '' && attr != '' && style != '') {
            var searchCriteria = {
                Style_Cd: style,
                Color_Cd: color,
                Attribute_Cd: attr,
                SizeList: sizelst
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
       
        var th = $('#SizeOrderType');
        var ordTyp = th.data('kendoDropDownList');
        if (ordTyp) {
            var disble = ordTyp.element.context.disabled;
            if (!disble) {
                if ($('#SizeOrderType').val() == '' || $('#SizeOrderType').val() == null || $('#SizeOrderType').val() == '--Select order type--') {
                    ISS.common.showPopUpMessage('Please select an Order Type.');
                    return false;
                }

            }
        }

        var szLstPrev = WO.const.SKUSizeList;
        WO.const.SKUSizeList = [];
        $("#SelectedSizes").html('');
        var skusize = '';
        var selectedSize = '';
        var grid = $("#grdMultiSKUSize").data("kendoGrid");
        var gridData = grid.dataSource.view();
        if (gridData.length > 0) {

            for (var i = 0; i < gridData.length; i++) {
                if (gridData[i].Qty > 0) {
                    WO.const.SKUSizeList.push({ SizeCD: gridData[i].SizeCD, Size: gridData[i].Size, Qty: gridData[i].Qty, Edited: false });
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
            //CA#374991-16
            if ( WO.const.SKUSizeList.length <= 10 || $('#SizeOrderType').data('kendoDropDownList').value() == 'Single Order(s)' ) {

                if (szLstPrev.length > 0) {
                    for (var i = 0; i < szLstPrev.length; i++) {
                        for (var j = 0; j < WO.const.SKUSizeList.length; j++) {
                            if (szLstPrev[i].SizeCD == WO.const.SKUSizeList[j].SizeCD && szLstPrev[i].Qty != WO.const.SKUSizeList[j].Qty) {
                                WO.const.SKUSizeList[i].Edited = true;
                            }
                        }
                    }
                }
                //var myArray = WO.const.SKUSizeList.filter(function (x) { return szLstPrev.indexOf(x) < 0 })

                WO.const.sizePopup.close();
                $("#btnWOSizes").focus();
                if (WO.const.sizePopup.settings.handler) { // used for duplicate popup
                    WO.const.sizePopup.settings.handler(WO.const.SKUSizeList, $('#SizeOrderType').data('kendoDropDownList').value());
                    $('#btnWOSizes').focus();
                    return false;
                }
                else {

                    var totalText = $("#grdMultiSKUSize .k-footer-template").text().split(":")[1].trim();
                    var skulst = JSON.stringify(WO.const.SKUSizeList);
                    $("#MultiSizeList").val(skulst);
                    $("#TotalDozens").val(totalText);
                    $("#Dozens").val(totalText);
                    if (WO.const.SKUSizeList.length > 1) {
                        if ($("#GroupId").val() == '0' || $("#GroupId").val() == '') {
                            ISS.common.executeActionAsynchronous("../Order/GetGroupID", null, function (stat, data) {
                                if (stat && data) {
                                    $("#GroupId").val(data);
                                }
                            });
                        }
                    }
                    else {
                        $("#GroupId").val('0');
                    }
                    WO.onSizeChange();
                    WO.SelectedSizesList(WO.const.SKUSizeList);
                    $('#btnWOSizes').focus();
                }

                if (WO.const.SKUSizeList.length > 1) {
                    $("#TotalDozens").prop("disabled", true);
                }
                else {
                    $("#TotalDozens").prop("disabled", false);
                }
                setTimeout(function() {
                    $("#Revision").focus();
                }, 500)
                //$("#Revision").focus();
                //selectedSize = selectedSize.replace(/^,|,$/g, '');
                //selectedSize = selectedSize.slice(0, -2);
                //$("#lblSelectedSize").html("Selected Size(s): " + selectedSize);
            }

            else {
        //        if ($('#SizeOrderType').val() ==)
        //                        {
                
        //        }
        //else  {
                ISS.common.showPopUpMessage('You can select upto 10 sizes.');
        }
    //}
        }
        else { 
            
        }
        return false;
    },

    onMultiInLineSizeSave: function () {
        Focusedindex = 13;
        var th = $('#SizeOrderType');
        var ordTyp = th.data('kendoDropDownList');
        if (ordTyp) {
            var disble = ordTyp.element.context.disabled;
            if (!disble) {
                if ($('#SizeOrderType').val() == '' || $('#SizeOrderType').val() == null || $('#SizeOrderType').val() == '--Select order type--') {
                    ISS.common.showPopUpMessage('Please select an Order Type.');
                    return false;
                }

            }
        }

        var szLstPrev = WO.const.SKUSizeList;
        WO.const.SKUSizeList = [];
        $("#SelectedSizes").html('');
        var skusize = '';
        var selectedSize = '';
        var grid = $("#grdMultiSKUSize").data("kendoGrid");
        var gridData = grid.dataSource.view();
        if (gridData.length > 0) {

            for (var i = 0; i < gridData.length; i++) {
                if (gridData[i].Qty > 0) {
                    WO.const.SKUSizeList.push({ SizeCD: gridData[i].SizeCD, Size: gridData[i].Size, Qty: gridData[i].Qty, Edited: false });
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
            if (WO.const.sizePopup.settings && WO.const.sizePopup.settings.mode == 'Duplicate' && WO.const.SKUSizeList.length > 1) {
                if (!WO.const.SizeValidator.validate()) {
                    return false;
                }
            }
            //CA#374991-16
            if (WO.const.SKUSizeList.length <= 10 || $('#SizeOrderType').data('kendoDropDownList').value() == 'Single Order(s)') {

                if (szLstPrev.length > 0) {
                    for (var i = 0; i < szLstPrev.length; i++) {
                        for (var j = 0; j < WO.const.SKUSizeList.length; j++) {
                            if (szLstPrev[i].SizeCD == WO.const.SKUSizeList[j].SizeCD && szLstPrev[i].Qty != WO.const.SKUSizeList[j].Qty) {
                                WO.const.SKUSizeList[i].Edited = true;
                            }
                        }
                    }
                }
                //var myArray = WO.const.SKUSizeList.filter(function (x) { return szLstPrev.indexOf(x) < 0 })

                WO.const.sizePopup.close();
                $("#btnWOSizes").focus();
                if (WO.const.sizePopup.settings.handler) { // used for duplicate popup
                    WO.const.sizePopup.settings.handler(WO.const.SKUSizeList, $('#SizeOrderType').data('kendoDropDownList').value());
                    $('#btnWOSizes').focus();
                    return false;
                }
                else {
                    var grid = $("#grdWorkOrderDetail").data("kendoGrid");
                    var currentSelection = grid.select().parent();
                    //WO.const.currentTable = currentSelection;
                    var selectedItem = grid.dataItem(currentSelection)
                    selectedItem ? selectedItem : selectedItem = selectrow;
                    var totalText = $("#grdMultiSKUSize .k-footer-template").text().split(":")[1].trim();
                    var skulst = JSON.stringify(WO.const.SKUSizeList);
                    selectedItem.SizeList = skulst;
                    $("#MultiSizeList").val(skulst);
                    $("#TotalDozens").val(totalText);
                    totalTextQty = totalText;
                    $("#Dozens").val(totalText);
                    if (WO.const.SKUSizeList.length > 1) {
                        if ($("#GroupId").val() == '0' || $("#GroupId").val() == '') {
                            ISS.common.executeActionAsynchronous("../Order/GetGroupID", null, function (stat, data) {
                                if (stat && data) {
                                    $("#GroupId").val(data);
                                    selectedItem.GroupId = data;
                                }
                            });
                        }
                    }
                    else {
                        $("#GroupId").val('0');
                    }
                    
                    if (selectedItem) {
                        selectedItem.TotalDozens = totalText;
                        selectedItem.Dozens = totalText;
                        selectedItem.Lbs = totalText;
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
                        //
                    }
                    WO.onSizeChange();
                    WO.SelectedSizesListInline(WO.const.SKUSizeList);
                    $("#grdWorkOrderDetail").data("kendoGrid").refresh();
                    $('#btnWOSizes').focus();
                }

                if (WO.const.SKUSizeList.length > 1) {
                    $("#TotalDozens").prop("disabled", true);
                }
                else {
                    $("#TotalDozens").prop("disabled", false);
                }
                setTimeout(function () {
                    $("#Revision").focus();
                }, 500)
                //$("#Revision").focus();
                //selectedSize = selectedSize.replace(/^,|,$/g, '');
                //selectedSize = selectedSize.slice(0, -2);
                //$("#lblSelectedSize").html("Selected Size(s): " + selectedSize);
                var result = WO.ValidateHAAInLine();
                WO.HideFieldInline(result);
            }

            else {
                //        if ($('#SizeOrderType').val() ==)
                //                        {

                //        }
                //else  {
                ISS.common.showPopUpMessage('You can select upto 10 sizes.');
                setTimeout(function () {
                    $("#grdMultiSKUSize #Qty").focus();
                }, 25)
                return false;
            }
            //}
        }
        else {

        }
        WO.onSizetoQty(WO.const.SKUSizeList);
        return false;
    },

    InsertMultiSKU: function (e) {
        //e.preventDefault();
        ////WO.CreateOrder.popupValidator.hideMessages($('#MfgPathId'));
        ////if (WO.const.ErrorSku == true) {
        ////    ISS.common.showPopUpMessage('The external_sku_xref needs set up for this APS sku before creating work orders');
        ////    return false;
        ////}
        
        //if (size_qty == 0) {
        //    ISS.common.showPopUpMessage('Size quantity should not be zero.')
        //    return true;
        //}
        //else {
            WO.changeYear(null, function (ress) {
                if (ress) {
                    var mod = $("#gridMode").val();
                    //if (mod == "") {
                    var isOk = false;
                    var url = '../order/InsertMultiSku';

                    var urlValidation = '../order/ValidateMultiSku';

                    var txtPlant = $("#TxtPlant").data("kendoComboBox").value();

                    var machineType = $("#MacType").data("kendoComboBox").value();
                    //if (machineType == "")
                    //{
                    //    ISS.common.showPopUpMessage('Machine Type cannot be a blank value.');
                    //    return false;
                    //}

                    var gridDetail = $("#grdWorkOrderDetail").data("kendoGrid");
                    var dsWOData = gridDetail.dataSource.data();
                    if (dsWOData.length > 0) {
                        WO.const.SKUSizeList = [];
                        for (var i = 0; i < dsWOData.length; i++) {
                            var sz = dsWOData[i].Size;
                            var qty = dsWOData[i].Dozens;
                            //var szdesc = dsWOData[i].SizeCde;
                            var szdesc = dsWOData[i].SizeLit;
                            if (qty == '')
                                qty = 0;
                            WO.const.SKUSizeList.push({
                                SizeCD: sz, Size: szdesc, Qty: qty
                            });
                        }
                    }
                    if (dsWOData.length <= 0) {
                        ISS.common.showPopUpMessage('Please enter at least one order detail.');
                        return false;
                    }
                    ISS.common.blockUI(true);
                    $(dsWOData).each(function (i, ite) {
                        ite.ErrorStatus = false; ite.SizeList = WO.const.SKUSizeList; });
                    var gridCum = $("#grdwrkOrderCumulative").data("kendoGrid");
                    var dsWOCum = gridCum.dataSource.data();

                    var gridFab = $("#grdwrkOrderFabric").data("kendoGrid");
                    var dsWOFab = gridFab.dataSource.data();

                    var dueDate = $("#DueDate").data("kendoDropDownList").value();
                    var orders = $("#OrdersToCreate").data("kendoNumericTextBox").value();
                    var planner = $("#PlannerCd").data("kendoDropDownList").value();

                    var woHeader = {
                        Dmd: $('#Dmd').val(),
                        DueDate: dueDate,
                        PlannedWeek: $('#PlannedWeek').val(),
                        PlannedYear: $('#PlannedYear').val(),
                        PlannedDate: $('#PlannedDate').val(),
                        Dc: $('#Dc').val(),
                        OrdersToCreate: orders,
                        PlannerCd: planner,
                        TxtPlant: txtPlant,
                        MachinePlant: machineType,
                        WODetails: dsWOData,
                        WOCumulative: dsWOCum,
                        WOFabric: dsWOFab
                    };
                    woHeader = JSON.stringify(woHeader);

                    ISS.common.executeActionAsynchronous(url, woHeader, function (stat, data) {
                        ISS.common.blockUI();
                        if (stat && data) {
                            if (data.Status) {
                                // saved success                      
                                ISS.common.notify.success(data.ErrMsg);

                                ISS.common.showConfirmMessage(data.ErrMsg + ' Do you want to clear the screen?', null, function (reply) {
                                    if (reply) {
                                        WO.ClearWOPage();
                                    }
                                    else {
                                        WO.GenerateNewGroupId();
                                    }
                                });
                                //});

                            }
                            else {
                                ISS.common.blockUI();
                                if (data.ErrType != null && data.ErrType == "ConsumeOrders") {
                                    ISS.common.showConfirmMessage(data.ErrMsg + ' Do you want to continue?', null, function (reply) {
                                        if (reply) {
                                            ISS.common.blockUI(true);
                                            var woHeader = {
                                                Dmd: $('#Dmd').val(),
                                                DueDate: dueDate,
                                                PlannedWeek: $('#PlannedWeek').val(),
                                                PlannedYear: $('#PlannedYear').val(),
                                                PlannedDate: $('#PlannedDate').val(),
                                                Dc: $('#Dc').val(),
                                                OrdersToCreate: orders,
                                                PlannerCd: planner,
                                                TxtPlant: txtPlant,
                                                MachinePlant: machineType,
                                                SkipConsumeOrders: true,
                                                WODetails: dsWOData,
                                                WOCumulative: dsWOCum,
                                                WOFabric: dsWOFab
                                            };

                                            woHeader = JSON.stringify(woHeader);

                                            ISS.common.executeActionAsynchronous(url, woHeader, function (stat, data) {
                                                ISS.common.blockUI();
                                                if (stat && data) {
                                                    if (data.Status) {
                                                        ISS.common.notify.success(data.ErrMsg);
                                                        ISS.common.showConfirmMessage(data.ErrMsg + ' Do you want to clear the screen?', null, function (reply) {
                                                            if (reply) {
                                                                WO.ClearWOPage();
                                                            }
                                                            else {
                                                                WO.GenerateNewGroupId();
                                                            }
                                                        });
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
                                            });
                                        }
                                    });
                                }// end consume orders
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
                        }
                    }, 'POST');
                    //}
                    //else {
                    //    ISS.common.notify.error('Pending changes are identified in the page <br/> Please save the changes and proceed.');
                    //    return false;
                    //}
                }// end change year
                else {

                }
            })
            return false;
        //}
    },

   onErrorFailure: function (data) {
        var grid = $("#grdWorkOrderDetail").data("kendoGrid");
        var gridData = grid.dataSource.view();
        for (var i = 0; i < gridData.length; i++) {
            var row = grid.table.find("tr[data-uid='" + gridData[i].uid + "']");
            if (data.ErrDetails != null) {
                data.ErrDetails.forEach(function (item) {
                    if (gridData[i].Id == item.Id) {
                        var RowData = grid.dataItem(row);
                        RowData.ErrorMessage = item.ErrorMessage;
                        grid.dataSource.getByUid(gridData[i].uid).set("ErrorMessage", item.ErrorMessage);
                        grid.dataSource.getByUid(gridData[i].uid).set("ErrorStatus", true);
                    }
                });
            }

            //if (gridData[i].Id == data.Id) {
            //    grid.dataSource.getByUid(gridData[i].uid).set("ErrorMessage", data.ErrMsg);
            //    grid.dataSource.getByUid(gridData[i].uid).set("ErrorStatus", true);
            //}
        }

    },

    onErrorCorrected: function (data) {
        var grid = $("#grdWorkOrderDetail").data("kendoGrid");
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
        var grid = $("#grdWorkOrderDetail").data("kendoGrid");
        var gridData = grid.dataSource.view();
        var gridLen = {
            dgridCount: gridData.length
        };

        gridLen = JSON.stringify(gridLen);

        ISS.common.executeActionAsynchronous("../Order/GetBulkGroupID", gridLen, function (stat, data) {
            if (stat && data) {
                var grid = $("#grdWorkOrderDetail").data("kendoGrid");
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
        if (WO.CreateOrder.StyleVal != $('#frmWoEditor #SellingStyle').val()) {
            WO.const.SKUSizeList = [];
            $("#MultiSizeList").val('');
            $("#SelectedSizes").html('');
            var gridSize = $("#grdMultiSKUSize").data("kendoGrid");
            gridSize.dataSource.data([]);
            gridSize.refresh();
        }
    },
    //focusFun: function (e) {
    //    if ($("#Revision").val() == 0) {
    //        $("#Revision").focus();
    //        return false;
    //    }
    //},AlterIdChange
    showAlternateId: function (e) {
        var colors = $("#ColorCode").data("kendoComboBox").value()
        var ColorCode =WO.getMultiselectItem(colors) ;
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
                $('#AlternateId').focus();

            },
            close: function () {
                setTimeout(function () {
                    $("#frmWoEditor #AlternateId").focus();
                }, 0)
                return false;
            },
        };
        ISS.common.CommonSearchShow(settings);

    },
    showAlterpopupId: function (e) {
        var colors = $("#ColorCode").data("kendoComboBox").value()
        var ColorCode = WO.getMultiselectItem(colors);
        var Attribute = $("#Attribute").data("kendoComboBox").text();
        var Style = $("#SellingStyle").val();

        var pdata = {
            Style: style_cd,
            Color: color_cd,
            Attribute: attr_cd
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
                AlternateId_cd = d.CuttingAlt;
                $('#AlternateId').val(d.CuttingAlt);
                var grid = $("#grdWorkOrderDetail").data("kendoGrid");
                var row = $(e).closest("tr");
                if (row) {
                    var rowData = grid.dataItem(row);
                    rowData ? rowData : rowData = selectrow;
                    if (rowData) {
                        rowData.AlternateId = d.CuttingAlt;
                        grid.table.find("tr[data-uid='" + rowData.uid + "'] td:eq(15)")[0].innerHTML = d.CuttingAlt;   //Modified By :UST(Gopikrishnan), Date:27-June-2017, Description: As per the request Created BD & Dozens Only checkbox no need to be disabled because it makes the user to click the cell first for clicking the checkbox.
                        grid.table.find("tr[data-uid='" + rowData.uid + "'] td:eq(15)")[0].className = 'k-dirty-cell';
                        $(grid.table.find("tr[data-uid='" + rowData.uid + "'] td:eq(15)")[0]).append('<span class="k-dirty" style="margin-left: -26px;"></span>');
                    }
                }
                if (WO.CreateOrder.isUpdateCumOnChange) {
                    WO.updateCumulativeAndFabric();
                }
                //$('#AlternateId').focus();

            },
            close: function () {
                setTimeout(function () {
                    //$("#frmWoEditor #AlternateId").focus();
                }, 0)
                return false;
            },
        };
        ISS.common.CommonSearchShow(settings);

    },
    clearMultiSize: function (e) {

        var ds = $("#Size").data("kendoMultiSelect");
        ds.value([]);

        return false;
    },

    PopulateCutPathTxtPath: function (e) {

        //var ColorCode = $("#ColorCode").data("kendoMultiSelect").value();
        var colors = $('#ColorCode').data("kendoComboBox").value();
        var ColorCode = WO.getMultiselectItem(colors);
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
                WO.loadCutPathDtls();
                $('#CutPath').focus();
                return false;
            },
            close: function () {
            setTimeout(function () {
                $("#frmWoEditor #CutPath").focus();
            }, 0)
            return false;
        },
        };
        ISS.common.CommonSearchShow(settings);
    },

    onColorBound: function (e) {
       
        
        
    },

    validateCreateWO: function () {
        var urlValidation = '../order/ValidateMultiSku';

        var txtPlant = $("#TxtPlant").data("kendoComboBox").value();

        var machineType = $("#MacType").data("kendoComboBox").value();

        var gridDetail = $("#grdWorkOrderDetail").data("kendoGrid");
        var dsWOData = gridDetail.dataSource.data();
        $(dsWOData).each(function(i, ite) {ite.SizeList = WO.const.SKUSizeList; });
        var gridCum = $("#grdwrkOrderCumulative").data("kendoGrid");
        var dsWOCum = gridCum.dataSource.data();

        var gridFab = $("#grdwrkOrderFabric").data("kendoGrid");
        var dsWOFab = gridFab.dataSource.data();

        var dueDate = $("#DueDate").data("kendoDropDownList").value();
        var orders = $("#OrdersToCreate").data("kendoNumericTextBox").value();
        var planner = $("#PlannerCd").data("kendoDropDownList").value();
        //var dmd = $("#Dmd").data("kendoDropDownList").value();

        if (dsWOData.length <= 0) {
            return false;
        }

        var woHeader = {
            Dmd: $('#Dmd').val(),
            DueDate: dueDate,
            PlannedWeek: $('#PlannedWeek').val(),
            PlannedYear: $('#PlannedYear').val(),
            PlannedDate: $('#PlannedDate').val(),
            Dc: $('#Dc').val(),
            OrdersToCreate: orders,
            PlannerCd: planner,
            TxtPlant: txtPlant,
            MachinePlant: machineType,
            WODetails: dsWOData,
            WOCumulative: dsWOCum,
            WOFabric: dsWOFab
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
                    var grid = $("#grdWorkOrderDetail").data("kendoGrid");
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

    MultisizeDataBound: function () {
        var grid = $("#grdMultiSKUSize").data("kendoGrid");
        var firstCell = grid.table.find("tr:first td:last");
        if (firstCell != null) {
            grid.current(firstCell);
            grid.editCell(firstCell);
            grid.table.focus();
            
        }
       
        
    },
    //
    resetSizeQty: function () {
        if (WO.const.SKUSizeList.length == 1) {
            WO.const.SKUSizeList[0].Qty = $("#TotalDozens").val();
            var skulst = JSON.stringify(WO.const.SKUSizeList);
            $("#MultiSizeList").val(skulst);
            WO.SelectedSizesList(WO.const.SKUSizeList);
            $("#Dozens").val($("#TotalDozens").val());
        }
        
    },
    isZero: function (evt) {
        //var qty = evt.value;
        //if (qty == 0) {
        //    ISS.common.notify.error("Please enter valid qty.")
        //    Focusedindex = 13;
        //}
        //var index = -1;
        //if (qty != null && qty != "") {
        //    index = qty.indexOf('.');
        //}
        //var charCode = (evt.which) ? evt.which : event.keyCode
        //if (
        //    ((charCode != 46 || index != -1) &&
        //    (charCode < 48 || charCode > 57))) {
        //    if (event.preventDefault) event.preventDefault();
        //    else {
        //        if (event.returnValue) event.returnValue = false;
        //        return false;
        //    }

        //}
       // alert(qty);
        //return true;
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
    revFilterData: function () {
        return {
            SellingStyle: style_cd,
            ColorCode: color_cd,
            Attribute: attr_cd,
            SizeList: WO.const.SKUSizeList,
            AssortCode: asort_cd
        };
    },
    revFilterData1: function () {
        return {
            Style_Cd: style_cd,
            Color_Cd: color_cd,
            Attribute_Cd: attr_cd,
            SizeList: WO.const.SKUSizeList,
            Asrt_Cd: asort_cd
        };
    },
    retrieveColorsData: function () {
            var result = {
            Style_Cd: style_cd
        };
        return result;
    },
    retrieAttributeData: function () {
        var result = {
            Style_Cd: style_cd,
            Color_Cd: color_cd
        };
        return result;
    },
    retrieSizeData: function () {
        var result = {
            Style_Cd: style_cd,
            Color_Cd: color_cd,
            Attribute_Cd: attr_cd
        };
        return result;
    },
    retrieMFGData: function () {
        var result = {
            Style_Cd: style_cd,
            Color_Cd: color_cd,
            Attribute_Cd: attr_cd,
            SizeList: WO.const.SKUSizeList,
            DemLoc_Cd: prm_cd
        };
        return result;
    },
    MFGFilterData: function () {
        var result = {
            SellingStyle: style_cd,
            ColorCode: color_cd,
            Attribute: attr_cd,
            SizeList: [{ Size: size_cd, SizeCD: size_cd }],
            PrimaryDC: prm_cd
        };
        return result;
    },
    CutPathFilterData: function () {
        return {
            SuperOrder: superO,
            DyeCode: 'C',
            CutPath: cPath
        }
    },

    TxtPathFilterData: function () {
        return {
            SuperOrder: superO,
            DyeCode: 'T',
            CutPath: cPath
        }
    },// var dat = e.sender.dataSource.data();
    //var dataItem = dat[e.sender.selectedIndex];
    RevisionSelect: function (e) {
        var dataItem = this.dataItem(e.item.index());
        rev_cd = dataItem.NewRevision;
        WO.RevChanged();
    },
    RevisionChange: function (e) {
        var dat = e.sender.dataSource.data();
        var dataItem = dat[e.sender.selectedIndex];
        rev_cd = dataItem.Revision;
        WO.RevChanged();
    },
    RevChanged: function (e) {
        var grid = $("#grdWorkOrderDetail").data("kendoGrid");
        if (grid) {
            var postData = {
                Style_Cd: style_cd,
                Color_Cd: color_cd,
                Attribute_Cd: attr_cd,
                SizeList: WO.const.SKUSizeList,
                Asrt_Cd: asort_cd,
                Rev_Cd: rev_cd
            };
            postData = JSON.stringify(postData);
            ISS.common.executeActionAsynchronous("../order/GetSkuRevisionPkg", postData, WO.loadPKGInfo);
        }
    },
    loadPKGInfo: function (stat, data) {
        if (stat && data.length > 0) {
            var grid = $("#grdWorkOrderDetail").data("kendoGrid");
            var currentSelection = grid.select().parent();
            var selectedItem = grid.dataItem(currentSelection)
            if (selectedItem) {
                
                selectedItem.PKGStyle = data[0].PKGStyle;
                if (selectedItem.ErrorStatus == true) {
                    $('tr[data-uid="' + selectedItem.uid + '"]').addClass("highlighted-row");
                }
                else {
                    $('tr[data-uid="' + selectedItem.uid + '"]').removeClass("highlighted-row");
                }
                $("#grdWorkOrderDetail").data("kendoGrid").refresh();
                
            }
            WO.getChildSKU();
        }
    },
    retrieveDemand: function () {
        var SizeL = '';
        for(var i = 0; i < WO.const.SKUSizeList.length; i++) {
            if(i == 0) {
                SizeL = WO.const.SKUSizeList[i].SizeCD;
            }
            else {
                SizeL = SizeL + ',' +WO.const.SKUSizeList[i].SizeCD;
            }
        }
        //var revisionNO = rev_cd;
        return {
            Style_Cd: style_cd,
            Color_Cd: color_cd,
            Attribute_Cd: attr_cd,
            Size_Cd: SizeL,
            Revision_Cd: rev_cd
        };
    },
    onPKGStyleClick: function (e) {
        WO.revSearchClickInLine();
    },
    onPKGStyleChange: function (e) {
        currentTableRow = $(e).closest('tr');
        if (!e.value) {
            return;
        }
        pkg_inp_style = e.value.toUpperCase();
        rev_cd = e.value.toUpperCase();
        var grid = $("#grdWorkOrderDetail").data("kendoGrid");
        if (grid) {
            var postData = {
                Style_Cd: style_cd,
                Color_Cd: color_cd,
                Attribute_Cd: attr_cd,
                SizeList: WO.const.SKUSizeList,
                Asrt_Cd: asort_cd,
                Pak_Cd: pkg_inp_style
            };
            postData = JSON.stringify(postData);
            ISS.common.executeActionAsynchronous("../order/GetPKGCheckVal", postData, WO.validPKGStyles);
        }
    },
    validPKGStyles: function (stat, data) {
        if (stat && data.length > 0) {
            var grid = $("#grdWorkOrderDetail").data("kendoGrid");
            var currentSelection = grid.select().parent();
            var selectedItem = grid.dataItem(currentSelection)
            selectedItem ? selectedItem : selectedItem = selectrow;
            if (selectedItem) {
                if (pkg_inp_style == data[0].NewRevision) {
                    //selectedItem.PKGStyle = data[0].PKGStyle;
                    //selectedItem.Revision = data[0].NewRevision;
                    pkgstyle = data[0].PKGStyle;
                    rev_cd = data[0].NewRevision;
                    grid.table.find("tr[data-uid='" + selectedItem.uid + "'] td:eq(14)")[0].innerText = data[0].PKGStyle;
                    grid.table.find("tr[data-uid='" + selectedItem.uid + "'] td:eq(11)")[0].innerText = data[0].NewRevision;
                }
                else {
                    selectedItem.ErrorStatus = true;
                    
                }
                //selectedItem.PKGStyle = data[0].PKGStyle;
                if (selectedItem.ErrorStatus == true) {
                    $('tr[data-uid="' + selectedItem.uid + '"]').addClass("highlighted-row");
                }
                else {
                    $('tr[data-uid="' + selectedItem.uid + '"]').removeClass("highlighted-row");
                }
                Focusedindex = 11;
                var currentSelection = grid.select().parent();
                if (currentSelection) {
                    currentTableRow = currentSelection;
                    var rowData = grid.dataItem(currentSelection);
                    rowData ? rowData: rowData = selectrow;
                    if (rowData) {
                       DataUid = rowData.uid;
                    }
                }
                
            }
            WO.getChildSKU();
        }
        else
            ISS.common.showPopUpMessage('Invalid PKGStyle -' + pkg_inp_style);
    },
    
    
    onRowsWorkOrderSelected: function (arg) {
        var grid = $("#grdWorkOrderDetail").data("kendoGrid");
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
                Dozens_cd = rowData.Dozens;
                Lbs_cd = rowData.Lbs;
                size_qty = rowData.SizeQty;
                MfgPathId_cd = rowData.MfgPathId;
                rowData.IsDirty = true;
                DataUid = rowData.uid;
            }
            

        }
    },
    onWOStyleCodeClick: function (e) {
        currentTableRow = $(e).closest('tr');
        //var grd = $("#grdWorkOrderDetail").data("kendoGrid");
        //var currentSelection = grd.select().parent();
        //currentTableRow = currentSelection;
        //cur_row = grd.dataItem(currentSelection);
        if (!e.value) {
            return;
        }
        //**********************
        style_cd = e.value.toUpperCase();
        if (WO.CreateOrder.popupValidator.validateInput(style_cd)) {
            //WO.CreateOrder.popupValidator.hideMessages($('#Revision'));
            //WO.CreateOrder.popupValidator.hideMessages($('#MfgPathId'));
            //WO.CreateOrder.popupValidator.hideMessages($('#PKGStyle'));
            if (WO.CreateOrder.StyleVal != style_cd) {
                var grid = $("#grdWorkOrderDetail").data("kendoGrid");
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
                        selectedItem.SellingStyle = style_cd;
                        selectedItem.PKGStyle = style_cd;
                        selectedItem.IsDirty = true;
                    }
                }
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
                            if ($("#Dc").val() == '') {
                                $("#Dc").val(data[0].PrimaryDC).change();
                            }
                            $("#OriginTypeCode").val(data[0].OriginTypeCode);
                            orgin_Type = data[0].OriginTypeCode;
                            $("#BusinessUnit").val(data[0].CorpBusUnit);
                            corpbus_cd = data[0].CorpBusUnit;
                            var postedData = { Style: style_cd };
                            postedData = JSON.stringify(postedData);
                            ISS.common.executeActionAsynchronous("../order/GetRetrieve", postedData, WO.loadDescriptionAndColor);
                            //if (WO.CreateOrder.isUpdateCumOnChange) {
                            //    WO.updateCumulativeAndFabric();
                            //}

                        }

                    }

                    else {
                        $('#SellingStyle').val('')
                        ISS.common.notify.error('Please provide a valid style.');
                    }

                });

                Focusedindex = 6;
            }
        }

        return false;
    },
    loadDescriptionAndColor: function (stat, data) {
        if (stat && data.length > 0) {
            var grid = $("#grdWorkOrderDetail").data("kendoGrid");
            var selectedItem = grid.dataItem(currentTableRow);
            selectedItem ? selectedItem : selectedItem = selectrow;
            cur_row ? cur_row: cur_row = selectrow;
            WO.const.SKUSizeList = [];
            if (selectedItem) {
                selectedItem.SellingStyle = "";
                selectedItem.SellStyleDesc = "";
                selectedItem.GroupId = 0;
                selectedItem.Id = "";
                selectedItem.CreateBd = "";
                selectedItem.DozensOnly = true;
                selectedItem.ColorCode = "";
                selectedItem.Attribute = "";
                selectedItem.Size = "";
                //Newly Added for getting size description
                selectedItem.SizeLit = "";
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
                selectedItem.SellingStyle = style_cd;
                selectedItem.Id = id_cd;
                selectedItem.ColorCode = data[0].Color;
                selectedItem.SellStyleDesc = data[0].StyleDesc;
                selectedItem.Attribute = data[0].Attribute;
                selectedItem.Size = data[0].Size;
                selectedItem.Revision = data[0].Rev;
                selectedItem.SizeLit = data[0].SizeShortDes;
                //selectedItem.SizeCde = data[0].SizeShortDes;
                selectedItem.PKGStyle = style_cd;

                style_cd = style_cd;
                color_cd = data[0].Color
                attr_cd = data[0].Attribute
                size_cd = data[0].Size
                size_desc = data[0].SizeShortDes;
                rev_cd = data[0].Rev;
                WO.const.SKUSizeList = [{ Size: data[0].SizeShortDes, SizeCD: size_cd }];
                WO.getChildSKU();
                var postedData = { Style_Cd: style_cd, Color_Cd: color_cd, Attribute_Cd: attr_cd, SizeList: WO.const.SKUSizeList, DemLoc_Cd: prm_cd };
                postedData = JSON.stringify(postedData);
                ISS.common.executeActionAsynchronous("../order/GetMFGId", postedData, function (stat, data) {
                    if (stat && data) {
                        if (data.length > 0) {//data[0].SewPlt
                            MfgPathId_cd = data[0].MfgPathId;
                            SewPlt_cd = data[0].SewPlt;
                            selectedItem.MfgPathId = data[0].MfgPathId;
                            selectedItem.SewPlt = data[0].SewPlt;
                            $("#grdWorkOrderDetail").data("kendoGrid").refresh();

                        }
                    }

                });
                selectedItem.ErrorStatus = data[0].ErrorStatus;
                if (selectedItem.ErrorStatus == true) {
                    $('tr[data-uid="' + selectedItem.uid + '"]').addClass("highlighted-row");
                }
                else {
                    $('tr[data-uid="' + selectedItem.uid + '"]').removeClass("highlighted-row");
                }
                $("#grdWorkOrderDetail").data("kendoGrid").refresh();
                flag = 0;
            }
        }
        else {
            var grid = $("#grdWorkOrderDetail").data("kendoGrid");
            var selectedItem = grid.dataItem(currentTableRow)
            if (selectedItem) {
                selectedItem.SellingStyle = "";
                selectedItem.Color = "";
                selectedItem.Description = "";
                selectedItem.Attribute = "";
                selectedItem.Size = "";

                $("#grdWorkOrderDetail").data("kendoGrid").refresh();
            }

            ISS.common.notify.error('Failed to retrieve Style details.');
        }
    },
    onColorSelect: function (e) {

        var dataItem = this.dataItem(e.item.index());
        color_cd = dataItem.Color;
        var grid = $("#grdWorkOrderDetail").data("kendoGrid");
        if (grid) {
            
            //$("#MultiSizeList").val('');
            var postData = { styleCode: style_cd, colorCode: color_cd };
            postData = JSON.stringify(postData);
            
            ISS.common.executeActionAsynchronous("../order/GetAttributeInfo", postData, WO.loadAttributeInfo);
            if (WO.CreateOrder.isUpdateCumOnChange) {
                //WO.updateCumulativeAndFabric();
            }
        }
    },
    onColorChanged: function (e) {
        var dat = e.sender.dataSource.data();
        var dataItem = dat[e.sender.selectedIndex];
        dataItem ? color_cd = dataItem.Color : color_cd = color_cd;
        var grid = $("#grdWorkOrderDetail").data("kendoGrid");
        if (grid) {

            //$("#MultiSizeList").val('');
            var postData = { styleCode: style_cd, colorCode: color_cd };
            postData = JSON.stringify(postData);

            ISS.common.executeActionAsynchronous("../order/GetAttributeInfo", postData, WO.loadAttributeInfo);
            //if (WO.CreateOrder.isUpdateCumOnChange) {
            //    WO.updateCumulativeAndFabric();
            //}

            var c = Focusedindex;
            if (c == 6) {
                Focusedindex = 6;
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
        }
    },

    loadAttributeInfo: function (stat, data) {
        if (stat && data.length > 0) {
            var grid = $("#grdWorkOrderDetail").data("kendoGrid");
            var currentSelection = grid.select().parent();
            WO.const.SKUSizeList = [];
            WO.const.SKUSizeList = [{ Size: data[0].SizeShortDes, SizeCD: data[0].Size }];
            size_desc = data[0].SizeShortDes;
            size_cd = data[0].Size;
            
            var selectedItem = grid.dataItem(currentSelection)
            selectedItem ? selectedItem : selectedItem = selectrow;
            if (selectedItem) {
                selectedItem.DozensOnly = true;
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
                
                selectedItem.Attribute = data[0].Attribute;
                selectedItem.Size = data[0].Size;
                //Newly Added for getting size description
                selectedItem.SizeLit = data[0].SizeShortDes;
                selectedItem.PKGStyle = style_cd;
                selectedItem.Revision = data[0].Rev;
                rev_cd = data[0].Rev;
                WO.getChildSKU();
                var postedData = { Style_Cd: style_cd, Color_Cd: color_cd, Attribute_Cd: attr_cd, SizeList: WO.const.SKUSizeList, DemLoc_Cd: prm_cd };
                postedData = JSON.stringify(postedData);
                ISS.common.executeActionAsynchronous("../order/GetMFGId", postedData, function (stat, data) {
                    if (stat && data) {
                        if (data.length > 0) {//data[0].SewPlt
                            MfgPathId_cd = "";
                            SewPlt_cd = "";
                            MfgPathId_cd = data[0].MfgPathId;
                            SewPlt_cd = data[0].SewPlt;
                            selectedItem.MfgPathId = data[0].MfgPathId;
                            selectedItem.SewPlt = data[0].SewPlt;
                            $("#grdWorkOrderDetail").data("kendoGrid").refresh();

                        }
                    }

                });
                if (selectedItem.ErrorStatus == true) {
                    $('tr[data-uid="' + selectedItem.uid + '"]').addClass("highlighted-row");
                }
                else {
                    $('tr[data-uid="' + selectedItem.uid + '"]').removeClass("highlighted-row");
                }
                $("#grdWorkOrderDetail").data("kendoGrid").refresh();
                //requisitions.validateRequisitionDetailRow(selectedItem, setgridValidateRefresh)
            }
        }
        else {
            var grid = $("#grdWorkOrderDetail").data("kendoGrid");
            var currentSelection = grid.select().parent();
            var selectedItem = grid.dataItem(currentSelection)
            selectedItem ? selectedItem : selectedItem = selectrow;
            if (selectedItem) {
                selectedItem.Attribute = "";
                selectedItem.Size = "";

                $("#grdWorkOrderDetail").data("kendoGrid").refresh();
            }
            ISS.common.notify.error('Failed to retrieve Style details.');
        }
    },//
    onAttributeSelect: function (e) {
        var dataItem = this.dataItem(e.item.index());
        attr_cd = dataItem.Attribute;

        var grid = $("#grdWorkOrderDetail").data("kendoGrid");
        if (grid) {
            var postData = { styleCode: style_cd, colorCode: color_cd, attributeCode: attr_cd };
            postData = JSON.stringify(postData);
            ISS.common.executeActionAsynchronous("../order/GetSizeInfo", postData, WO.loadSizeInfo);
        }

        
    },
    onAttributeChange: function (e) {
        var dat = e.sender.dataSource.data();
        var dataItem = dat[e.sender.selectedIndex];
        if (dataItem.Attribute)
            attr_cd = dataItem.Attribute;

        var grid = $("#grdWorkOrderDetail").data("kendoGrid");
        if (grid) {
            var postData = { styleCode: style_cd, colorCode: color_cd, attributeCode: attr_cd };
            postData = JSON.stringify(postData);
            ISS.common.executeActionAsynchronous("../order/GetSizeInfo", postData, WO.loadSizeInfo);
        }

        Focusedindex = 7;
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
            var grid = $("#grdWorkOrderDetail").data("kendoGrid");
            var currentSelection = grid.select().parent();
            var selectedItem = grid.dataItem(currentSelection)
            selectedItem ? selectedItem : selectedItem = selectrow;
            WO.const.SKUSizeList = [{ Size: data[0].SizeShortDes, SizeCD: data[0].Size }];
            if (selectedItem) {
                selectedItem.Size = "";
                selectedItem.SizeQty = 0;
                selectedItem.Revision = "";
                selectedItem.DozensOnly = true;
                selectedItem.MfgPathId = "";
                selectedItem.Size = data[0].Size;
                //Newly added for getting size description
                selectedItem.SizeLit = data[0].SizeShortDes;
                selectedItem.PKGStyle = style_cd;
                selectedItem.AssortCode = asort_cd;
                selectedItem.Revision = data[0].Rev;
                rev_cd = data[0].Rev;
                style_cd = style_cd;
                size_cd = data[0].Size
                size_desc = data[0].SizeShortDes;
                //Newly added for getting size description
                WO.const.SKUSizeList = [{ Size: size_desc, SizeCD: size_cd }];
                var postedData = { Style_Cd: style_cd, Color_Cd: color_cd, Attribute_Cd: attr_cd, SizeList: WO.const.SKUSizeList, DemLoc_Cd: prm_cd };
                postedData = JSON.stringify(postedData);
                ISS.common.executeActionAsynchronous("../order/GetMFGId", postedData, function (stat, data) {
                    if (stat && data) {
                        if (data.length > 0) {//data[0].SewPlt
                            MfgPathId_cd = "";
                            SewPlt_cd = "";
                            MfgPathId_cd = data[0].MfgPathId;
                            SewPlt_cd = data[0].SewPlt;
                            selectedItem.MfgPathId = data[0].MfgPathId;
                            selectedItem.SewPlt = data[0].SewPlt;
                            $("#grdWorkOrderDetail").data("kendoGrid").refresh();

                        }
                    }

                });
                if (totalTextQty != null) {
                    selectedItem.TotalDozens = totalTextQty;
                    selectedItem.Dozens = totalTextQty;
                }
                else {
                    selectedItem.TotalDozens = size_qty;
                    selectedItem.Dozens = size_qty;
                }
                WO.const.SKUSizeList = [{ Size: size_cd, SizeCD: size_desc }];
                if (selectedItem.ErrorStatus == true) {
                    $('tr[data-uid="' + selectedItem.uid + '"]').addClass("highlighted-row");
                }
                else {
                    $('tr[data-uid="' + selectedItem.uid + '"]').removeClass("highlighted-row");
                }
                $("#grdWorkOrderDetail").data("kendoGrid").refresh();
                //requisitions.validateRequisitionDetailRow(selectedItem, setgridValidateRefresh)
            }
        }
        else {
            var grid = $("#grdWorkOrderDetail").data("kendoGrid");
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
                $("#grdWorkOrderDetail").data("kendoGrid").refresh();
            }
            ISS.common.notify.error('Failed to retrieve Style details.');
        }
    },
    onSizeSelect: function (e) {
       
        var dataItem = this.dataItem(e.item.index());
        WO.const.SKUSizeList = [{ Size: dataItem.SizeDesc, SizeCD: dataItem.Size }];
        size_desc = dataItem.SizeDesc;
        size_cd = dataItem.Size;

        var postData = WO.retrieveColorData();
        var colors = color_cd;
        var ColorCode = WO.getMultiselectItem(colors);
        var Attribute = attr_cd;
        var grid = $("#grdWorkOrderDetail").data("kendoGrid");
        var currentSelection = grid.select().parent();
        currentTableRow = currentSelection;
        cur_row ? cur_row : cur_row = selectrow;
        var selectedItem = grid.dataItem(currentSelection)
        selectedItem ? selectedItem : selectedItem = selectrow;
        if (selectedItem) {
            //if (selectedItem.Revision) {
           //Newly Added for getting color description
            selectedItem.Size = size_cd;
            selectedItem.SizeLit = size_desc;
           //End
            postData.ColorCode = ColorCode;
            postData.Attribute = Attribute;
            postData.SizeList = WO.const.SKUSizeList;
            postData.AssortCode = $("#AsrtCode").val();
            postData = JSON.stringify(postData);
            ISS.common.executeActionAsynchronous("../order/GetMaxRevision", postData, function (stat, data) {
                if (stat) {
                    if (data.length > 0) {
                        selectedItem.DozensOnly = true;
                        selectedItem.SizeQty = 0;
                        selectedItem.Revision = data[0].Revision;
                        rev_cd = data[0].Revision;
                        WO.getChildSKU();
                        //selectedItem.PKGStyle = $("#PKGStyle").val();GetMFGPathId
                        //pkgstyle = $("#PKGStyle").val();
                        $("#grdWorkOrderDetail").data("kendoGrid").refresh();
                    }
                }
                else {
                    ISS.common.notify.error('Failed to retrieve Revision details.');
                }

            });
            var postedData = { Style_Cd: style_cd, Color_Cd: color_cd, Attribute_Cd: attr_cd, SizeList: WO.const.SKUSizeList, DemLoc_Cd: prm_cd };
            postedData = JSON.stringify(postedData);
            ISS.common.executeActionAsynchronous("../order/GetMFGId", postedData, function (stat, data) {
                if (stat && data) {
                    if (data.length > 0) {//data[0].SewPlt
                        MfgPathId_cd = data[0].MfgPathId;
                        SewPlt_cd = data[0].SewPlt;
                        selectedItem.MfgPathId = data[0].MfgPathId;
                        selectedItem.SewPlt = data[0].SewPlt;
                        $("#grdWorkOrderDetail").data("kendoGrid").refresh();

                    }
                }

            });
            //}
            //else {
            //    if (WO.CreateOrder.isUpdateCumOnChange) {
            //        WO.updateCumulativeAndFabric();
            //    }
            //}
            //if (WO.CreateOrder.isUpdateCumOnChange) {
            //    WO.updateCumulativeAndFabric();
            //}
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

    onSizeChanged: function (e) {
        
        //var dataItem = this.dataItem(e.item.index());
        var dat = e.sender.dataSource.data();
        var dataItem = dat[e.sender.selectedIndex];
        //WO.const.SKUSizeList = [{ Size: dataItem.SizeDesc, SizeCD: dataItem.Size }];
        if (dataItem) {
            size_desc = dataItem.SizeDesc;
            size_cd = dataItem.Size;
        }
        WO.const.SKUSizeList = [{ Size: size_desc, SizeCD: size_cd }];
        var postData = WO.retrieveColorData();
        var colors = color_cd;
        var ColorCode = WO.getMultiselectItem(colors);
        var Attribute = attr_cd;
        var grid = $("#grdWorkOrderDetail").data("kendoGrid");
        var currentSelection = grid.select().parent();
        currentTableRow = currentSelection;
        cur_row ? cur_row : cur_row = selectrow;
        var selectedItem = grid.dataItem(currentSelection)
        selectedItem ? selectedItem : selectedItem = selectrow;
        if (selectedItem) {
            //if (selectedItem.Revision) {
            //Newly Added for getting sizeinfo 
            selectedItem.Size = '';
            selectedItem.SizeLit = '';
                selectedItem.Size = size_cd;
                selectedItem.SizeLit = size_desc;
                //End
                postData.ColorCode = ColorCode;
                postData.Attribute = Attribute;
                postData.SizeList = WO.const.SKUSizeList;
                postData.AssortCode = $("#AsrtCode").val();
                postData = JSON.stringify(postData);
                ISS.common.executeActionAsynchronous("../order/GetMaxRevision", postData, function (stat, data) {
                    if (stat) {
                        if (data.length > 0) {
                            selectedItem.DozensOnly = true;
                            selectedItem.CreateBd = false;
                            selectedItem.SizeQty = 0;
                            //selectedItem.Revision = data[0].Revision;
                            //rev_cd = data[0].Revision;
                            //WO.getChildSKU();
                            //selectedItem.PKGStyle = $("#PKGStyle").val();GetMFGPathId
                            //pkgstyle = $("#PKGStyle").val();
                            $("#grdWorkOrderDetail").data("kendoGrid").refresh();
                        }
                    }
                    else {
                        ISS.common.notify.error('Failed to retrieve Revision details.');
                    }

                });
                var postedData = { Style_Cd: style_cd, Color_Cd: color_cd, Attribute_Cd: attr_cd, SizeList: WO.const.SKUSizeList, DemLoc_Cd: prm_cd };
                postedData = JSON.stringify(postedData);
                ISS.common.executeActionAsynchronous("../order/GetMFGId", postedData, function (stat, data) {
                    if (stat && data) {
                        if (data.length > 0) {//data[0].SewPlt
                            MfgPathId_cd = data[0].MfgPathId;
                            SewPlt_cd = data[0].SewPlt;
                            selectedItem.MfgPathId = data[0].MfgPathId;
                            selectedItem.SewPlt = data[0].SewPlt;
                            $("#grdWorkOrderDetail").data("kendoGrid").refresh();

                        }
                    }

                });
            //}
            //else {
            //    if (WO.CreateOrder.isUpdateCumOnChange) {
            //        WO.updateCumulativeAndFabric();
            //    }
            //}
                //if (WO.CreateOrder.isUpdateCumOnChange) {
                //    WO.updateCumulativeAndFabric();
                //}
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
    
    onMFGSelect: function (e) {
        
        var dataItem = this.dataItem(e.item.index());
        //WO.const.SKUSizeList =[{ Size: dataItem.Size, SizeCD: dataItem.Size}];
        //size_cd = dataItem.Size;
        var grid = $("#grdWorkOrderDetail").data("kendoGrid");
        var currentSelection = grid.select().parent();
        var selectedItem = grid.dataItem(currentSelection)
        cur_row ? cur_row : cur_row = selectrow;
        selectedItem ? selectedItem : selectedItem = selectrow;
        //var result = WO.ValidateHAAInLine();
        //WO.HideFieldInline(result);
        if (selectedItem) {
            MfgPathId_cd = dataItem.MfgPathId;
            SewPlt_cd = dataItem.SewPlt_cd;
            selectedItem.MfgPathId = dataItem.MfgPathId;
            selectedItem.SewPlt = dataItem.SewPlt;
            pkgstyle = selectedItem.PKGStyle;
            $("#grdWorkOrderDetail").data("kendoGrid").refresh();
            if (WO.CreateOrder.isUpdateCumOnChange) {
                WO.updateCumulativeAndFabric();
            }
        }


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
    onMFGChanged: function (e) {
        //Focusedindex = 12;
        var dat = e.sender.dataSource.data();
        var dataItem = dat[e.sender.selectedIndex];
        //WO.const.SKUSizeList =[{ Size: dataItem.Size, SizeCD: dataItem.Size}];
        //size_cd = dataItem.Size;
        var grid = $("#grdWorkOrderDetail").data("kendoGrid");
        var currentSelection = grid.select().parent();
        var selectedItem = grid.dataItem(currentSelection)
        cur_row ? cur_row : cur_row = selectrow;
        selectedItem ? selectedItem : selectedItem = selectrow;
        //var result = WO.ValidateHAAInLine();
        //WO.HideFieldInline(result);
        if (selectedItem) {
            if(dataItem){
                    MfgPathId_cd = dataItem.MfgPathId;
                    SewPlt_cd = dataItem.SewPlt_cd;
                    selectedItem.MfgPathId = dataItem.MfgPathId;
                    selectedItem.SewPlt = dataItem.SewPlt;
                    pkgstyle = selectedItem.PKGStyle;
                    $("#grdWorkOrderDetail").data("kendoGrid").refresh();
                    if (WO.CreateOrder.isUpdateCumOnChange) {
                        WO.updateCumulativeAndFabric();
                    }
             }
        }

        Focusedindex = 12;
        var currentSelection = grid.select().parent();
        if (currentSelection) {
            currentTableRow = currentSelection;
            var rowData = grid.dataItem(currentSelection);
            rowData ? rowData: rowData = selectrow;
            if (rowData) {
                DataUid = rowData.uid;
            }
        }
    },
    AlterIdChange: function (e) {
        //Focusedindex = 15;
        var dat = e.sender.dataSource.data();
        var dataItem = dat[e.sender.selectedIndex];
        var grid = $("#grdWorkOrderDetail").data("kendoGrid");
        var currentSelection = grid.select().parent();
        var selectedItem = grid.dataItem(currentSelection)
        cur_row ? cur_row: cur_row = selectrow;
        selectedItem ? selectedItem: selectedItem = selectrow;
        if (selectedItem) {
            AlternateId_cd = dataItem.CuttingAlt;
            $('#AlternateId').val(AlternateId_cd);
            
            $("#grdWorkOrderDetail").data("kendoGrid").refresh();
            if (WO.CreateOrder.isUpdateCumOnChange) {
                WO.updateCumulativeAndFabric();
            }
        }
        Focusedindex = 15;
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
    AlterIdChange: function (e) {
        currentTableRow = $(e).closest('tr');
        if (!e.value) {
            return;
        }

        var grid = $("#grdWorkOrderDetail").data("kendoGrid");
        var currentSelection = grid.select().parent();
        var selectedItem = grid.dataItem(currentSelection)
        cur_row ? cur_row : cur_row = selectrow;
        selectedItem ? selectedItem : selectedItem = selectrow;
        if (selectedItem) {
            AlternateId_cd = e.value.toUpperCase();
            $('#AlternateId').val(AlternateId_cd);

            $("#grdWorkOrderDetail").data("kendoGrid").refresh();
            if (WO.CreateOrder.isUpdateCumOnChange) {
                WO.updateCumulativeAndFabric();
            }
        }
        Focusedindex = 15;
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
    //Modified By :UST(Gopikrishnan), Date:30-June-2017, Description: As per the request Created BD & Dozens Only checkbox no need to be disabled because it makes the user to click the cell first for clicking the checkbox, modification : Starts
    BDCheckClick: function (e) {
        var grid = $("#grdWorkOrderDetail").data("kendoGrid");
        if (e.checked == true) {
            //var currentSelection = grid.select().parent();
            var row = $(e).closest("tr");
            //if (currentSelection) {
            if (row) {
                //var rowData = grid.dataItem(currentSelection);
                var rowData = grid.dataItem(row);
                rowData ? rowData : rowData = selectrow;
                if (rowData) {
                    rowData.CreateBd = true;
                    rowData.DozensOnly = false;
                    
                    //grid.table.find("tr[data-uid='" + rowData.uid + "'] td:eq(21)")[0].innerHTML = "<input disabled='disabled' type='checkbox' data-bind='checked: DozensOnly'>";
                    grid.table.find("tr[data-uid='" + rowData.uid + "'] td:eq(21)")[0].innerHTML = "<input type='checkbox' data-bind='checked: DozensOnly'>"; 
                    grid.table.find("tr[data-uid='" + rowData.uid + "'] td:eq(21)")[0].className = 'k-dirty-cell';
                    $(grid.table.find("tr[data-uid='" + rowData.uid + "'] td:eq(21)")[0]).append('<span class="k-dirty" style="margin-left: -26px;"></span>');
                }
            }

            Focusedindex = 9;
            //var currentSelection = grid.select().parent();
            //if (currentSelection) {
                //currentTableRow = currentSelection;
            //var rowData = grid.dataItem(currentSelection);
            var row = $(e).closest("tr");
            if (row) {
                currentTableRow = row;
                var rowData = grid.dataItem(row);
                rowData ? rowData: rowData = selectrow;
                if (rowData) {
                    DataUid = rowData.uid;
                }
            }
            grid.refresh();
        }
    },

    DZCheckClick: function (e) {
        
        if (e.checked == true) {
            var grid = $("#grdWorkOrderDetail").data("kendoGrid");
            //var currentSelection = grid.select().parent();
            var row = $(e).closest("tr");
            //if (currentSelection) {
            if (row) {
                //var rowData = grid.dataItem(currentSelection);
                var rowData = grid.dataItem(row);
                rowData ? rowData : rowData = selectrow;
                if (rowData) {
                    rowData.CreateBd = false;
                    rowData.DozensOnly = true;
                   
                    grid.table.find("tr[data-uid='" + rowData.uid + "'] td:eq(20)")[0].innerHTML = "<input type='checkbox' data-bind='checked: CreateBd'>";   //Modified By :UST(Gopikrishnan), Date:27-June-2017, Description: As per the request Created BD & Dozens Only checkbox no need to be disabled because it makes the user to click the cell first for clicking the checkbox.
                    grid.table.find("tr[data-uid='" + rowData.uid + "'] td:eq(20)")[0].className = 'k-dirty-cell';
                    $(grid.table.find("tr[data-uid='" + rowData.uid + "'] td:eq(20)")[0]).append('<span class="k-dirty" style="margin-left: -26px;"></span>');
                }
            }

            Focusedindex = 10;
            //var currentSelection = grid.select().parent();
            //if (currentSelection) {
            //currentTableRow = currentSelection;
            //var rowData = grid.dataItem(currentSelection);
            var row = $(e).closest("tr");
            if (row) {
                currentTableRow = row;
                var rowData = grid.dataItem(row);
                rowData ? rowData: rowData = selectrow;
                if (rowData) {
                    DataUid = rowData.uid;
                }
            }
            grid.refresh();
        }
    },
    //Modification Ends

    PopulateCutPathPopUp: function(e) {
    var superOrder = null;
    var cutpath = null;
    var mfgpath = null;
    var grid = $("#grdWorkOrderDetail").data("kendoGrid");
        //var rows = WOM.selectedRows();
    var row = $(e).closest("tr");
    if (row) {
        //var rowData = grid.dataItem(currentSelection);
        var rowData = grid.dataItem(row);
        rowData ? rowData : rowData = selectrow;
        if (rowData) {
            superOrder = rowData.SuperOrder;
            mfgpath = rowData.MfgPathId;
            cutpath = rowData.CutPath;
            //});
        }
    }
    var result = {
        SuperOrder: superOrder
    };
    //if (name == "CutPath")
        result.DyeCode = 'C';
    //else
    //    result.DyeCode = 'T';
        result.MfgPathId = mfgpath;
    result.CutPath = cutpath;
    var Name = name;
    var form = 'frmWO';


    settings = {
        columns: [{
            Name: "Source_Plant",
            Title: "Path Id",
        },
        {
            Name: "Source_Plant",
            Title: "Cut Plant",
        }],
        AllowSelect: true,
        title: (result.DyeCode == 'C') ? 'Select a Cut Plant' : 'Select a Txt Plant',
        url: WO.const.urlPopulateCutPathTxtPath,
        //url: WO.const.urlPopulateCutPath,
        postData: result,
        handler: function (d) {
            var rowData = grid.dataItem(row);
            rowData ? rowData : rowData = selectrow;
            if (rowData) {
                    data.children['25'].innerText = d.Source_Plant;
                    rowData.CutPath = d.Source_Plant;                   
               }

            return false;
        },
        close: function () {
            setTimeout(function () {
                $("#frmWO #CutPath").focus();
            }, 25)
            return false;
        },
    };
    ISS.common.CommonSearchShow(settings);


},

    UpdateCumOnChange: function (e) {
        currentTableRow = $(e).closest('tr');
        if (!e.value) {
            return;
        }
        
        var grid = $("#grdWorkOrderDetail").data("kendoGrid");
        var currentSelection = grid.select().parent();
        var selectedItem = grid.dataItem(currentSelection)
        cur_row ? cur_row : cur_row = selectrow;
        selectedItem ? selectedItem : selectedItem = selectrow;
        if (selectedItem) {
            cutpath_cd = e.value.toUpperCase();
            selectedItem.CutPath = cutpath_cd

           // $("#grdWorkOrderDetail").data("kendoGrid").refresh();
            if (WO.CreateOrder.isUpdateCumOnChange) {
                WO.updateCumulativeAndFabric();
            }
        }
        Focusedindex = 18;
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


};


$.extend(WO, MSKUWO);