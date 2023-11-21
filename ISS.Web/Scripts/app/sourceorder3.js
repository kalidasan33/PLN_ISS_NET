
var currentTableRow = null;
var currentStyle = null;
temp = {


    addInputClass: function (frm) {
        $(frm + ' input:not(:checkbox):not(:button):not(:reset):not(:submit)').addClass('InputF');
    },

    docRequisitionsReady3: function (IsLoad) {
        
        requisitions.addInputClass('#frmRetOrder');
        $('#frmRetOrder #btnRetOrderSearch').bind('click', requisitions.loadRetOrderGrid)
        $('#frmRetOrder .InputF').keypress(function (e) {
            if (e.which == 13) {
                //  requisitions.loadRetOrderGrid();
            }
        });
        ISS.common.toUpperCase('.InputF:not(.excludeF)');
        $('#btnRetOrderClear').bind('click', function () {
            var grid = $("#grdRetOrder").data("kendoGrid");
            $('.k-grid-RetOrderSelectAll').text("Select All");
            ISS.common.clearGridFilters(grid);
            grid.dataSource.data([]);
            $('#frmRetOrder input:not(input[type="submit"]):not(input[type="reset"])').val('');
            $('#lblFRetTot').text('0');
            $('#lblRowSelTot').text('0');
            //$("#SuggWO").data("kendoDropDownList").value(33);
            //grid.dataSource.filter([]);
        })
        $('.k-grid-btnRetOrderSelectAll').bind('click', requisitions.SelectedOrderDetails);
        $('.k-grid-btnRetOrderMoveAll').bind('click', requisitions.MoveAllOrderDetails);
        $('.k-grid-btnRetOrderMove').bind('click', requisitions.MoveOrderDetails);
        $('.k-grid-btnDuplicate').bind('click', requisitions.requisitionDuplicate);
        $('.k-grid-btndeletemulti').bind('click', requisitions.requisitionDeleteMultiple);
        $('.k-grid-btnRetOrderJump').bind('click', requisitions.gotoSummary);
        $('#btnClearSO').bind('click', requisitions.clearAllFieldsNewReq);
        $('#frmRequisitions').submit(function () {
            return false;
        })
        $('#btnSOSave').bind('click', function () {
            requisitions.InsertRequisition();
            return false;
        });


        requisitions.const.CreatedBy = $('#frmRequisitions #CreatedBy').val();

        

        $('#ShowSummaryOnly').bind('change', function () {
            requisitions.doSummarized($(this).prop('checked'));
        });

        $("#SelectAll").bind('change', function () {
            requisitions.SelectAllDetails($(this).prop('checked'));
        });
        $("#grdRequisitionDetail").on('change', '.chkbx', function () {
            setTimeout(function () {
                var gridDetail = $("#grdRequisitionDetail").data("kendoGrid");
                $("#SelectAll").prop('checked', $('.chkbx:checked').length == gridDetail.dataSource.view().length);
            });
        });
           
    },

    SelectUsingShifytKey: function (e) {
        ISS.common.SelectUsingShifytKey(e, requisitions.selectRowRetOrder)
    },
    SelectAllDetails: function (isSelected) {
       
        var grid = $('#grdRequisitionDetail').data('kendoGrid');        
        grid.table.find('.chkbx').prop('checked', isSelected);
       
            
    },
    selectRowRetOrder: function (arg) {
        var TotDz = 0.0;
        var grid = $("#grdRetOrder").data("kendoGrid");
        var rows = grid.select();
        $(rows).each(function (idx, item) {
            var data = grid.dataItem(item);
            if (data && !data.IsDeleted) {
                TotDz += ISS.common.convertQtyToEach(data.Qty);
            }
        });
        $('#lblFRetTot').text(ISS.common.convertEachToDecimal(TotDz));
        $('#lblRowSelTot').text(rows.length);


    },

    gridDataBoundStyle: function (e) {
        var gridData = e.sender.dataSource.view();
        for (var i = 0; i < gridData.length; i++) {
            var row = e.sender.table.find("tr[data-uid='" + gridData[i].uid + "']");
            var qty = gridData[i].Qty.toString();
            if (qty.indexOf(".") > 0)
                qty = qty.replace(".", "-");
            row.find(".retOrdQty").text(ISS.common.getQtyToEachDisplay(qty));
        }
        $('#FilteredColumns').html(ISS.common.getFilteredColumns("#grdRetOrder"));
        $('.k-grid-btnRetOrderSelectAll').text("Select All");
        $('#lblFRetTot').text('0');
        $('#lblRowSelTot').text('0');
    },

    gotoSummary: function () {

        var url = location.protocol + '//' + location.hostname + $(this).data('url');
        var queryString = $.param({
            Planner: $('#Planner').val(), WorkCenter: $('#WorkCenter').val(), Style: $('#Style').val(), Color: $('#Color').val(),
            Attribute: $('#Attribute').val(), Size: $('#Size').val(), SuggWO: $('#SuggWO').val(),
            autoLoad: true,
        });

        if (queryString != null && queryString.length > 0) {
            url += "?" + queryString;
        }
        window.open(url, "_blank");
        return false
    },

    InsertRequisition: function () {
        if (requisitions.isValidFormMode()) {
            if (requisitions.const.validator.validate()) {
                var gridDetail = $("#grdRequisitionDetail").data("kendoGrid");
                var dsData = gridDetail.dataSource.data();
                if (dsData.length == 0) {
                    ISS.common.showPopUpMessage('Please enter requisition detail.')
                    return false;
                }
                ISS.common.blockUI(true);
                var url = '../order/InsertRequisition';
                //if( $('#frmRequisitions #FormMode').val()=="Add")
                if ($('#frmRequisitions #FormMode').val() == "Edit") {
                    var url = '../order/UpdateRequisition';
                    //for (i = 0; i < dsData.length ; i++) {
                    //    dsData[i].IsDirty = dsData[i].dirty || dsData[i].IsMovedObject;
                    //}
                }

                var PostData = {
                    req: ISS.common.getFormData($('#frmRequisitions')),
                    reqDet: dsData
                };
                PostData.req.ReqDetailTracking = $('#ReqDetailTracking').prop('checked');
                PostData.req.ShowSummaryOnly = $('#ShowSummaryOnly').prop('checked');

                postData = JSON.stringify(PostData);

                ISS.common.executeActionAsynchronous(url, postData, function (stat, data) {
                    ISS.common.blockUI();
                    if (stat && data) {
                        if (data.Key) {
                            ISS.common.showPopUpMessage(data.Value, ISS.common.MsgType.Success, function () {
                                ISS.common.notify.success(data.Value);
                                gridDetail.dataSource.read();

                            });
                            $('#frmRequisitions #FormMode').val("Edit");
                        }
                        else {
                            ISS.common.notify.error(data.Value);
                        }
                    } // end stat
                    else {
                        ISS.common.showPopUpMessage('Failed to insert requisition details.');
                    }

                }); // end ajax
            }
            else {

            }// end validate
        }
        else {
            ISS.common.notify.error('Please generate Requisition Id.');
        }
    },

    updateRequisition: function () {

    },

    searchDataRetOrder: function () {
        var data = ISS.common.getFormData($('#frmRetOrder'));
        data.SO = requisitions.getRetrieveSuperOrders();
        data.Src = requisitions.const.Src;
        return data;
    },

    loadRetOrderGrid: function () {
        var grid = $("#grdRetOrder").data("kendoGrid");
        if (!ISS.common.Settings.RetOrderInit) {
            grid.table.on("keydown", requisitions.SelectUsingShifytKey)
            ISS.common.Settings.RetOrderInit = true;
            ISS.common.Settings.Currentgrid = grid;
        }
        requisitions.const.Src = "A";
        if (grid.pager.page() > 1) grid.pager.page(1)
        grid.dataSource.read();
        requisitions.const.Src = null;
        ISS.common.collapsePanel('#panelbar-images', 2);
        $('.k-grid-btnRetOrderSelectAll').text("Select All");
        return false;
    },

    getRetrieveSuperOrders: function () {
        var gridDetail = $("#grdRequisitionDetail").data("kendoGrid");
        var dsData = gridDetail.dataSource.data();
        var arr = new Array();
        if (dsData.length > 0) {
            $(dsData).each(function (idx, item) {
                arr.push({ SuperOrder: item.SuperOrder });
            });
        }
        return arr;
    },

    getRequisitionSKU: function () {
        var gridDetail = $("#grdRequisitionDetail").data("kendoGrid");
        var dsData = gridDetail.dataSource.data();
        var arr = new Array();
        if (dsData.length > 0) {
            $(dsData).each(function (idx, item) {
                arr.push(requisitions.getSKU(item));
            });
        }
        return arr;
    },
    getSKU: function (item, rev) {
        return {
            Style: item.Style,
            Color: item.Color,
            Attribute: item.Attribute,
            Size: item.Size,
            Revision: item.Rev,
            Sku: item.Style + '~' + item.Color + '~' + item.Attribute + '~' + item.Size + ((rev) ? ("~" + item.Rev) : '')
        };

    },

    getVendorDCInfos: function () {
        return {
            VendorNo: $('#frmRequisitions #VendorNo').val(),
            LwCompany: $('#frmRequisitions #LwCompany').val(),
            LwVendorLoc: $('#frmRequisitions #LwVendorLoc').val(),
            DcLoc: $('#frmRequisitions #DcLoc').val(),
            MFGPathId: $('#frmRequisitions #MFGPathId').val(),
            PlannedDcDate: $('#frmRequisitions #PlannedDcDate').val(),
            BusinessUnit: $("#BusinessUnit").data("kendoDropDownList").value(),
        };

    },
    changeDetDates: function (d) {
        if (d.PlanDate) d.PlanDate = ISS.common.parseDate(d.PlanDate);
        if (d.ScheduledShipDate) d.ScheduledShipDate = ISS.common.parseDate(d.ScheduledShipDate);
        if (d.CurrDueDate) d.CurrDueDate = ISS.common.parseDate(d.CurrDueDate);
        //DCDueDate
        //OriginalDueDate
        //DemandDate
    },

    MoveOrderDetails: function () {
        if (requisitions.const.validator.validate()) {
            var grid = $("#grdRetOrder").data("kendoGrid");
            var rows = grid.select();
            if (rows.length == 0) {
                ISS.common.showPopUpMessage('Please select at least one row.');
                return false;
            }
            //ISS.common.blockUI(true);
            var arrOrd = new Array();
            $(rows).each(function (idx, item) {
                var data = grid.dataItem(item);
                if (data)
                    arrOrd.push(data);
            })

            if (rows.length > 0)
                requisitions.MoveToOrderDetail(arrOrd);
        }
        else {
            ISS.common.showPopUpMessage('Please enter header details.');
        }
        
      

        return false;
       
    },
    SelectedOrderDetails: function (e) {
        var grid = $("#grdRetOrder").data("kendoGrid");
        var retData = grid.dataSource.data();
        var th = $(this);
        if (retData.length > 0) {
            if (th.text() == "Select All") {
                grid.element.find('tbody tr:not([style*="display: none"])').addClass('k-state-selected');
                th.text("UnSelect All");
            }
            else {
                grid.element.find('tbody tr.k-state-selected').removeClass('k-state-selected');
                th.text("Select All");
            }
        }
        requisitions.selectRowRetOrder();
        return false;
    },
    MoveToOrderDetail: function(rows)
    {

        var grid = $("#grdRetOrder").data("kendoGrid");
        var arrToRemove = new Array();
        var data = null;
        requisitions.const.Batch.TotCount = rows.length;
        requisitions.const.Batch.ProcCount = 0;
        requisitions.const.Batch.Success = 0;
        requisitions.const.Batch.Error = 0;

        var PostData = {           
            req: requisitions.getVendorDCInfos(),
            SO: requisitions.getRetrieveSuperOrders()
        };
        
        if (rows.length > requisitions.const.Batch.OrderCount)
        {
            ISS.common.showConfirmMessage('A total of ' +rows.length + ' records identified for moving ', null, function (reply) {
                if (reply) {
                    $('.k-grid-btnRetOrderSelectAll').text("Select All");
                    //start progress bar
                    ISS.common.showProgressBar(true, requisitions.const.Batch.TotCount, requisitions.getDispMsgMove(( '1-' 
                        + ( requisitions.const.Batch.OrderCount))
                    , requisitions.const.Batch.TotCount));
                    //ISS.common.blockUI(true);
                    requisitions.const.Batch.rows=rows;
                    data = rows.splice(0, requisitions.const.Batch.OrderCount);                  
                    PostData.list = data;
                    PostData.SessionNew = true;
                    requisitions.processBatch(PostData, requisitions.callbackNextSuccess, requisitions.batchCompletedSuccess); 
                }
            }, 'Continue', 'Cancel');
        }
        else
        { 
            ISS.common.blockUI(true);
            $('.k-grid-btnRetOrderSelectAll').text("Select All");
            PostData.list = rows;
            PostData.SessionNew = true;                
            requisitions.processBatch(PostData, null, function (retData) {
                //final message TBD
                requisitions.ClearOrdersInSession();
            });
        }
    },

    callbackNextSuccess: function (stat, PostData) {//  callbackNext Batch     
        if (requisitions.const.Batch.rows.length > 0) {
            PostData.list = requisitions.const.Batch.rows.splice(0, requisitions.const.Batch.OrderCount);
            PostData.SO = null;
            PostData.SessionNew = false;
            //update progress bar value
            ISS.common.updateProgressBar(requisitions.const.Batch.OrderCount + requisitions.const.Batch.ProcCount, requisitions.getDispMsgMove(((requisitions.const.Batch.OrderCount + requisitions.const.Batch.ProcCount + 1) + '-'
                + (requisitions.const.Batch.OrderCount + requisitions.const.Batch.ProcCount + ((requisitions.const.Batch.OrderCount < PostData.list.length) ? requisitions.const.Batch.OrderCount : PostData.list.length))), requisitions.const.Batch.TotCount));
            requisitions.processBatch(PostData, requisitions.callbackNextSuccess, requisitions.batchCompletedSuccess);
        }
        else {
            ISS.common.updateProgressBar(requisitions.const.Batch.TotCount, 'Moved ' + requisitions.const.Batch.TotCount + ' records.');
        }
    },
    batchCompletedSuccess: function (arrToRemove) { //batchCompleted
        if (requisitions.const.Batch.ProcCount == requisitions.const.Batch.TotCount) {
            requisitions.ClearOrdersInSession();
            setTimeout(function () { ISS.common.showProgressBar(false) },300);
        } 
    },

    // Processing batch data and returns call back
    processBatch: function (PostData, callbackNext, batchCompleted) {
        requisitions.print('Start processBatch')
        requisitions.ValidateOrderItems(JSON.stringify(PostData), function (stat, retData) {
            var dataRemove = PostData.list;
           
            if (callbackNext) callbackNext(stat, PostData);// will be used  next ajax call;
            requisitions.const.Batch.ProcCount += dataRemove.length;            
            requisitions.processValidateditems(retData, true, dataRemove, batchCompleted);
        });
    },
    print:function(t){
      if(location.hostname =='localhost') console.log(t+ ' >> '+new Date())
    },
    getDispMsgMove: function (c, tot) {
        var t = 'Moving ' + c + ' of ' + tot + ' records.';    
        return t;
    },
    ClearOrdersInSession:function()
    {
        ISS.common.blockUI();
        $('#lblFRetTot').text(0);
        $('#lblRowSelTot').text(0);

        $('.k-grid-btnRetOrderSelectAll').text("Select All");
        ISS.common.executeActionAsynchronous('ClearOrdersInSession', null, function () {            
        });
        requisitions.print('===== End ========')
        if (requisitions.const.Batch.Error > 0) {
                ISS.common.showPopUpMessage('Some orders did not transfer correctly.', null, function () {
                    ISS.common.notify.error(requisitions.const.Batch.Error + " order(s) did not move correctly.");
                    if (requisitions.const.Batch.Success > 0) {
                        ISS.common.notify.success(requisitions.const.Batch.Success + " order(s) transferred successfully.");

                    }
                    ISS.common.notify.info("Total " + (requisitions.const.Batch.Error+requisitions.const.Batch.Success ) + " orders transferred");
                });
        }
        else {
            ISS.common.notify.success(requisitions.const.Batch.Success + " order(s) transferred successfully.");
        }

    },
    AddToOrderDetail:function(resultData)
    {
        var gridDetail = $("#grdRequisitionDetail").data("kendoGrid");
        var dsData = gridDetail.dataSource.data();
        if (dsData) {
            for (i = 0; i < resultData.data.length; i++) {
                requisitions.changeDetDates(resultData.data[i])
                dsData.push(resultData.data[i]);
            }           
            gridDetail.dataSource.data(dsData);
                         
        }
    },
    RemoveFromOrderData : function(arrToRemove)
    {
        var grid = $("#grdRetOrder").data("kendoGrid");
        var rows = grid.dataSource.data();        
        $(arrToRemove).each(function (idx, item) {
            rows.remove(item);
        })
        grid.refresh();
    },

    processValidateditems: function (data, flash, arrToRemove, callback) {
        requisitions.print('-- Start add to RQ')
        requisitions.AddToOrderDetail(data); //???
        requisitions.print('--End add to RQ')
        requisitions.print('------ Start remove  RET ORder')
        requisitions.RemoveFromOrderData(arrToRemove);
        requisitions.print('------ SEnd  RET ORder')
        if (data.hasErrors) {
            requisitions.const.Batch.Error += data.ErrorCount;
            requisitions.const.Batch.Success += data.SuccessCount;
            }
        else {
            requisitions.const.Batch.Success += data.data.length;
        } 
        if (callback) callback(data,arrToRemove);
    },

    ValidateOrderItems: function(postData,callback)
    {
        ISS.common.executeActionAsynchronous('validateOrderItems', postData, function (stat, data) {
            if(callback)callback(stat,data)
        });      
    },
    MoveAllOrderDetails: function () {
        if (requisitions.const.validator.validate()) {
            var grid = $("#grdRetOrder").data("kendoGrid");
            var rows = grid.dataSource.data();
            if (rows.length == 0) {
                ISS.common.showPopUpMessage('No Records were found.');
            }
            else {
                if (rows.length > 0)
                    requisitions.MoveToOrderDetail(rows);
            }
        }
        else {
            ISS.common.showPopUpMessage('Please enter header details.');
            
        }
        return false;
             
    },
    validateRequisitionDetail: function (callback) {

        var gridDetail = $("#grdRequisitionDetail").data("kendoGrid");
        var dsData = gridDetail.dataSource.data();
        var PostData = {
            req: requisitions.getVendorDCInfos(),
            reqDet: dsData
        };
        postData = JSON.stringify(PostData);
        ISS.common.executeActionAsynchronous('validateRequisitionDetail', postData, function (stat, data) {
            if (stat) {
                if (data) {
                    //gridDetail.dataSource.data(data.data);
                    if (data.hasErrors) {
                        requisitions.showGridError(data.data[0]);
                        //if (data.ErrorCount == 1)
                        //    ISS.common.notify.error(data.ErrorCount + " record is not valid.");
                        //else
                        //    ISS.common.notify.error(data.ErrorCount + " records are not valid.");

                    }
                    else {
                        ISS.common.notify.success(data.data.length + " records validated.");
                    }
                } // end data
            } // end stat
            else {
                ISS.common.showPopUpMessage('Failed to validate line items.');
            }
            if (callback) callback(stat, data)
        }); // end ajax
    },

    showGridError: function (data) {
        $(data).each(function (idx, item) {
            if (item.ErrorStatus)
                ISS.common.notify.error(item.ErrorMessage);
        });
    },
    editDataItem: function (e) {
         
        var it = e.container.find('#Qty');
        if (it.val() != null)
            it.val(it.val().replace(".", "-"));
    },

    saveDataItem: function (e) {
        
        var it = e.container.find('#Qty');
        if (it.val() != null) {
            it.val(it.val().replace("-", "."))
            e.values.Qty = it.val().replace("-", ".");            
            if (e.preventDefault) e.preventDefault();
            else return false;
        }
        return false;
    },
    gridDetailBound: function (e) {
        
        $('.qtyfield.k-state-selected').removeAttr('role').removeClass('k-edit-cell')
        var exclude = new Array();
        exclude.push('ErrorMessage');
        exclude.push('StdCase');
        exclude.push('SizeLit');
        $("#SelectAll").prop('checked', false)
        var grid = $("#grdRequisitionDetail").data("kendoGrid");
        var gridData = grid.dataSource.view();
        var flag = false;
        for (var i = 0; i < gridData.length; i++) {
            var row = grid.table.find("tr[data-uid='" + gridData[i].uid + "']");
            if (gridData[i].isHide) {
                row.hide();
                continue;
            }
            else {
                row.show();
            }
            if (!gridData[i].IsDeleted && gridData[i].ErrorStatus) {
                row.addClass("highlighted-row");
            }
            else {
                row.removeClass("highlighted-row");
            }

            if (gridData[i].IsDeleted) {
                row.addClass("deleted-row").find('.k-grid-DeleteItem span').removeClass("k-icon k-delete").addClass("k-icon k-i-undo");

            }
            else {
                row.removeClass("deleted-row").find('.k-grid-DeleteItem span').removeClass("k-icon k-i-undo").addClass("k-icon k-delete");
            }
            if (gridData[i].StdCase == 0) {
                row.find('td:nth-child(10)').removeClass("skuBreak");
            }
            //var tqty = gridData[i].Qty / gridData[i].StdCase;

            var qty = gridData[i].Qty.toString();
            if (qty.indexOf('.') == -1 && qty.indexOf('-') == -1) qty += '.';
            while (qty.length < qty.indexOf('.') + 3) {
                flag = true;
                qty += '0';
            }
            gridData[i].Qty = qty;
            var qtyHyp = qty.replace(".", "-");
            row.find(".qtyfield").text(qtyHyp);

            var qtystd = getQtyToEachDisp(gridData[i].StdCase);
            var stdqtyHyp = (qtystd+'').replace(".", "-");
            
            row.find(".stdQtyfield").text(stdqtyHyp);

            var realqty = gridData[i].Qty.toString();

            var stdqty = getQtyToEach(gridData[i].StdCase);
            var val = null;

            var arr = realqty.split('.');
            if (arr.length > 1) {
                val = parseInt(arr[0] * 12) + parseInt(arr[1]);
            }
            else {
                val = gridData[i].Qty * 12;
            }
            
            var equal = val / stdqty;
            var x = Math.floor(equal)
            if (val != 0 && equal == x) {
                row.find('td:nth-child(10)').addClass("fieldbold");
            }
            else {
                row.find('td:nth-child(10)').removeClass("fieldbold");
            }
            ISS.common.highlightEditedCells(gridData[i], row, grid.columns, exclude)

        }
        var tooltip = $(".highlighted-row").kendoTooltip({
            filter: "a",
            content: kendo.template($("#Errtemplate").html()),
            width: 120,
            position: "top"
        }).data("kendoTooltip")
    },

    validateRequisitionDetailRow: function (row, callback) {

        var PostData = {
            req: requisitions.getVendorDCInfos(),
            reqDet: row
        };
        postData = JSON.stringify(PostData);
        ISS.common.executeActionAsynchronous('validateRequisitionDetailRow', postData, function (stat, data) {
            if (stat) {
                if (data) {
                    if (data.ErrorStatus) {
                        ISS.common.notify.error(data.ErrorMessage);
                        //ISS.common.notify.error(data.ErrorMessage);
                    }
                    else {
                        //  ISS.common.notify.success(data.data.length + " records validated.");
                    }
                    if (callback) callback(data, row);
                    return false;
                } // end data
            } // end stat
            else {
                ISS.common.showPopUpMessage('Failed to validate line item.');
            }
            if (callback) callback(data, row)
        }); // end ajax
    },

    BUChange: function (e) {

        var season = $("#Season").data("kendoDropDownList").dataSource.read();

        requisitions.validateRequisitionFull();

    },


    removeOrderDetail: function (e) {
        if (requisitions.isValidFormMode()) {
            var gridDetail = $("#grdRequisitionDetail").data("kendoGrid");
            var curRow = $(e.currentTarget).closest("tr");
            var dataItem = gridDetail.dataItem(curRow);
            if (dataItem) {
                if (!dataItem.IsDeleted) {
                    ISS.common.showConfirmMessage("Are you sure to delete this record?", null, function (ret) {
                        if (ret) {
                            if (dataItem.IsInserted) {
                                dataItem.IsDeleted = !dataItem.IsDeleted;
                                dataItem.IsDirty = true;
                                gridDetail.refresh();
                            }// end inserted - already inserted in DB
                            else {
                                gridDetail.dataSource.remove(dataItem);
                            }
                        }
                    });
                }
                else {
                    dataItem.IsDeleted = !dataItem.IsDeleted;
                    gridDetail.refresh();
                }

            }
        }
        return false;
    },

    seasonData: function () {
        return { BusinessData: $("#BusinessUnit").data("kendoDropDownList").value() }
    },

    validateRequisitionFull: function () {
        if (requisitions.isValidFormMode()) {
            var grid = $("#grdRequisitionDetail").data("kendoGrid");
            var dataDS = grid.dataSource.data();
            if (grid.dataSource._data.length > 0) {
                requisitions.validateRequisitionDetail(function (stat, data) {
                    if (stat) {
                        for (var i = 0; i < dataDS.length; i++) {
                            setgridvalidatestatus(data.data[i], dataDS[i]);
                        }
                        grid.refresh();
                    }
                });
            }
        }

    },

    isValidFormMode: function () {
        return ($("#frmRequisitions #RequisitionId").val() != "" && ($('#frmRequisitions #FormMode').val() == "Add" || $('#frmRequisitions #FormMode').val() == "Edit"))
    },

    doSummarized: function (is) {
        if (requisitions.isValidFormMode()) {
            var grid = $("#grdRequisitionDetail").data("kendoGrid");
            var data = grid.dataSource.data();

            var PostData = {
                IsSummarized: is,
                FormMode: $('#frmRequisitions #FormMode').val(),
                reqDet: data
            };
            postData = JSON.stringify(PostData);

            ISS.common.executeActionAsynchronous('SummarizeData', postData, function (stat, data) {
                if (stat && data) {
                    if (data.Status) {
                        for (i = 0; i < data.data.Data.length; i++) {
                            requisitions.changeDetDates(data.data.Data[i]);
                        }
                        grid.dataSource.data(data.data.Data);

                    }
                    else {
                        ISS.common.notify.error('Failed to summarize requisition detail.');
                    }
                } // end stat
                else {
                    ISS.common.notify.error('Failed to summarize requisition detail.');
                }

            }); // end ajax DuplicateCancel
        }
    },
    DuplicateCancel: function () {
        requisitions.const.comment.close();
    },

    onStyleCdClick: function (e) {
        ISS.common.executeActionAsynchronous("../order/GetRetrieve", null, loadDescriptionAndColor);
    },
    loadStdCaseInfo: function (stat, data) {
        if (stat && data.length > 0) {
            var grid = $("#grdRequisitionDetail").data("kendoGrid");
            var currentSelection = grid.select().parent();
            var selectedItem = grid.dataItem(currentSelection)
            if (selectedItem) {
                if (data[0].StdCase == 0 && data[0].StdCase == "")
                    data[0].StdCase = 1
                else
                    selectedItem.StdCase = data[0].StdCase;
                if (selectedItem.ErrorStatus == true) {
                    $('tr[data-uid="' + selectedItem.uid + '"]').addClass("highlighted-row");
                }
                else {
                    $('tr[data-uid="' + selectedItem.uid + '"]').removeClass("highlighted-row");
                }

                requisitions.validateRequisitionDetailRow(selectedItem, setgridValidateRefresh)
            }
        }
        else {
            var grid = $("#grdRequisitionDetail").data("kendoGrid");
            var currentSelection = grid.select().parent();
            var selectedItem = grid.dataItem(currentSelection)
            if (selectedItem) {
                selectedItem.StdCase = "";
                $("#grdRequisitionDetail").data("kendoGrid").refresh();
            }
            ISS.common.notify.error('Failed to Std Case details.');
        }
    },
    getEaches: function (val) {
        var intPart = parseInt(val);
        var decimalPart = val - intPart;
        var result = Math.round(((decimalPart) * 100));
        return parseInt(result);
    },
    GetDuplicateSingle: function () {
        var size = null;
        var quantity = null;
        var qty = null;
        var sizedesc = null;
        var gridDetail = $("#grdRequisitionDetail").data("kendoGrid");
        var dataDs = gridDetail.dataSource._data;
        var checkedItems = $('.chkbx:checked');
        var arr = new Array();

        for (i = 0; i <= $("#tblDuplicate .clstr").length - 1; i++) {
            qty = $($("#tblDuplicate .clstr")[i]).find("input[type='text']").val();
            qty = replaceAndCheck(qty);
            if (qty != "" && !qty.match(/^\d+(\.\d{1,2})?$/)) {
                ISS.common.showPopUpMessage("Invaild number");
                return false;
            }
            else if (requisitions.getEaches(qty) >= 12) {
                ISS.common.showPopUpMessage("Eaches must be less than 12");
                return false;
            }
        }
        var current=gridDetail.dataItem(gridDetail.select());
        for (i = 0; i <= $("#tblDuplicate .clstr").length - 1; i++) {
            size = $($("#tblDuplicate .clstr")[i]).find("td:first-child").text();
            quantity = $($("#tblDuplicate .clstr")[i]).find("input[type='text']").val();
            sizedesc = $($("#tblDuplicate .clstr")[i]).find("input[type='hidden']").val();
            if (size != null && quantity != "") {
                var dataItem = jQuery.extend({}, current);
                dataItem.Size = sizedesc;
                dataItem.SizeLit = size;
                dataItem.Qty = quantity.replace('-','.');
                arr.push(dataItem);
            }
        }
        var postData = { list: arr }

        $(checkedItems).each(function (idx, item) {
            var row = $(item).closest("tr");
            postData.dataItem = gridDetail.dataItem(row);  
        })
      
        ISS.common.executeActionAsynchronous("GetDuplicateRecordsSingle", JSON.stringify(postData), function (stat, data) {
            if (stat && data) {
                if (data.dataItem.Qty == 0) {
                    //delete
                    postData.dataItem.Qty = 0;
                    if (postData.dataItem.SuperOrder != '') {
                        // postData.dataItem.IsDeleted=true;
                    }
                    else {
                        //gridDetail.dataSource.remove(postData.dataItem);
                    }
                }
                else {
                    postData.dataItem.Qty = data.dataItem.Qty;
                }
                gridDetail.refresh();
                var dt = gridDetail.dataSource.data();
                $(data.newList).each(function (idx, item) {
                    requisitions.changeDetDates(item);
                    item.Cloned = {};
                    dt.push(item);

                });            
            }
        });
        requisitions.const.comment.close();
    },

    requisitionDeleteMultiple: function () {
        if (requisitions.isValidFormMode()) {
            var checkedItems = $('.chkbx:checked');
            if (checkedItems.length > 0) {
                ISS.common.showConfirmMessage('Are you sure to delete this record(s)?', null, function (ret) {
                    if (ret) {
                        var gridDetail = $("#grdRequisitionDetail").data("kendoGrid");
                        var arr = new Array();
                        $(checkedItems).each(function (idx, item) {
                            var row = $(item).closest("tr");
                            var dataItem = gridDetail.dataItem(row);

                            if (dataItem.IsInserted) {
                                dataItem.IsDeleted = true;
                                dataItem.IsDirty = true;

                            }// end inserted - already inserted in DB
                            else {
                                arr.push(dataItem)
                            }
                        });
                        if (arr.length > 0) {
                            $(arr).each(function (idx, item) {
                                gridDetail.dataSource.remove(item);
                            });
                        }
                        gridDetail.refresh();
                    }
                });
            }
            else {
                ISS.common.showPopUpMessage('Please select at least one record.');

            }
        }
        return false;
    },

    requisitionDuplicate: function () {
        if (requisitions.isValidFormMode()) {
            var quantity = null;
            var checkedItems = $('.chkbx:checked');
            if (checkedItems.length == 1) {
                var arr = new Array();
                var grid = $("#grdRequisitionDetail").data("kendoGrid");
                $(checkedItems).each(function (idx, item) {
                    var row = $(item).closest("tr");
                    var dataItem = grid.dataItem(row);
                    arr.push(dataItem)
                })
                if (grid) {
                    var data = grid.dataSource._data;
                    quantity = arr[0].Qty;
                    var postData = { Style_Cd: arr[0].Style, Color_Cd: arr[0].Color, Attribute_Cd: arr[0].Attribute, Revision_Cd: arr[0].Rev, Qty: arr[0].Qty, Size_Cd: arr[0].SizeLit }
                    ISS.common.executeActionAsynchronous("GetSkuSize", JSON.stringify(postData), function (stat, data) {
                        if (stat && data) {
                            var content = '<table id="tblDuplicate">'
                            content += '<tr>';
                            content += '<td colspan="3">' + postData.Style_Cd + ' ' + postData.Color_Cd + ' ' + postData.Attribute_Cd + ' ' + postData.Revision_Cd + '</td><td>&nbsp;</td>';
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
                                if (postData.Size_Cd == data[i].SizeDesc) {
                                    content += '<td>' + '<input type="text" class="k-textbox"  onkeypress="isNumber(this);" value="' + (postData.Qty+'').replace('.','-') + '" /> </td>';
                                }
                                else
                                    content += '<td>' + '<input type="text"  onkeypress="isNumber(this);" class="k-textbox" /> </td>';

                                content += '<td>' + '<input type="hidden" value="' + data[i].Size + '"/> </td>';
                                content += '</tr>';
                                content += ' <tr class="blankrow1"></tr>';
                            }
                            content += '<tr width="20px">';
                            content += '<td></td>';
                            content += '<td >' + '<input type="submit" value="Ok" id="btnOk" onclick="javascript:requisitions.GetDuplicateSingle();" /> ';
                            content += '&nbsp;' + '<input type="submit" value="Cancel" id="btnCancel" onclick="javascript:requisitions.DuplicateCancel();"  /> </td>';
                            content += '</tr>';
                            content += "</table>"

                            var settings = {
                                title: 'Duplicate Sizes',
                                animation: false,
                                height: '600px',
                                width: '350px',
                            };
                            requisitions.const.comment = ISS.common.popUpCustom('.divDuplicate', settings);
                            $(".divDuplicate").html(content);
                        }

                    });
                }

            }
            else if (checkedItems.length > 1) {
                ISS.common.showConfirmMessage('You have selected multiple items. Are you sure want to duplicate same records?', null, function (ret) {
                    if (ret) {

                        var gridDetail = $("#grdRequisitionDetail").data("kendoGrid");
                        var arr = new Array();
                        $(checkedItems).each(function (idx, item) {
                            var row = $(item).closest("tr");
                            var dataItem = gridDetail.dataItem(row);
                            arr.push(dataItem)
                        })
                        var postData = { list: arr }
                        ISS.common.executeActionAsynchronous("GetDuplicateRecords", JSON.stringify(postData), function (stat, data) {
                            if (stat && data) {
                                var dt = gridDetail.dataSource.data();
                                $(data).each(function (idx, item) {
                                    requisitions.changeDetDates(item);
                                    item.Cloned = {};
                                    dt.push(item);
                                });

                            }

                        });
                    }// end confirmation
                })
            }
            else {
                ISS.common.showPopUpMessage('Please select at least one record.');
            }
        }
        return false;
    },

    
};

$.extend(requisitions, temp);