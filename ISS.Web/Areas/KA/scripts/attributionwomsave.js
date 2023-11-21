

tWOM = {


    docWOMReady3: function (IsLoad) {
        
    }, 

    saveWOMdata: function () {

        var filter1 = WOM.getEditRowFilter();
        var grid = $("#grdWOM").data("kendoGrid");
        if (grid.editable && !grid.editable.validatable.validate()) {
            ISS.common.notify.error('Validation Failed.');
        }
        else {
            if (sserror == false) {
                var query1 = new kendo.data.Query(gridWOM.dataSource.data());
                var data1 = query1.filter(filter1).data;

                if (data1.length > 0) {
                    for (var i = 0; i < data1.length; i++) {
                        WOM.getEditedDataInline(data1[i]);
                        WOM.SetGridDataValues(data1[i]);
                    }
                    var filter = WOM.getPendingChangeFilter();

                    var query = new kendo.data.Query(gridWOM.dataSource.data());
                    var data = query.filter(filter).data;

                    ISS.common.showConfirmMessage('Are you sure you want to save changes?', null, function (ok) {
                        if (ok) {
                            WOM.setFormSaveMode(true);
                            for (i = data.length - 1; i >= 0; i--) {
                                if (data[i].Completed == true && (!data[i].ErrorStatus)) {
                                    data.splice(i, 1);
                                }
                            }
                            WOM.const.saveItems.data = data;

                            // gridWOM.dataSource.data(data); // Replacing DS with pending data
                            // gridWOM.dataSource.filter(filter);
                            //if (gridWOM.dataSource.page() > 1) gridWOM.dataSource.page(1);
                            ISS.common.blockUI(true);
                            var deleted = WOM.const.saveItems.Deleted;
                            var grouped = WOM.const.saveItems.Grouped;
                            var summarize = WOM.const.saveItems.Summarize;
                            var recFlag = true;
                            for (i = 0; i < data.length; i++) { // Each row 
                                var item = data[i];
                                if (item.Completed != true || item.ErrorStatus) {//??
                                    item.Completed = false;
                                    recFlag = false;
                                    item.DozensOnlyInd = (item.DozensOnly) ? 'Y' : 'N';
                                    item.CreateBDInd = (item.CreateBd) ? 'Y' : 'N';
                                    item.ErrorStatus = false;
                                    if (item.Summarized) {
                                        if (!summarize[item.GroupId]) {
                                            summarize[item.GroupId] = new Array();
                                        }
                                        summarize[item.GroupId].push(item);
                                    }
                                    else if (item.IsDeleted) {
                                        deleted.push(item);
                                    }
                                    else if (WOM.needRecalculation(item)) {
                                        //SKU change + GROUPING    Not taken care
                                        //ungrouping will happen auto
                                        var relist = new Array();
                                        relist.push(item);
                                        WOM.processSave(relist, 'Recalc');
                                        WOM.const.saveItems.Recalc[item.SuperOrder] = item;
                                    }
                                    else if (item.IsGrouped) {
                                        if (!grouped[item.GroupId]) {
                                            grouped[item.GroupId] = new Array();
                                        }
                                        grouped[item.GroupId].push(item);
                                    }
                                    else {
                                        //update &&  UNGROUP
                                        //IsFieldChange
                                        var relist = new Array();
                                        relist.push(item);
                                        WOM.processSave(relist, 'EditPFSUngroup');
                                        WOM.const.saveItems.EditPFSUngroup[item.SuperOrder] = item;
                                    }
                                } // end completed
                            }
                            if (recFlag) {
                                ISS.common.blockUI();
                                ISS.common.notify.error('There is no record to save.')
                            }

                            // Start saving deleted
                            if (deleted.length > 0) {
                                WOM.processSave(deleted, 'Delete');
                            }

                            //Start saving Grouped                 
                            for (prop in grouped) {
                                WOM.processSave(grouped[prop], 'Grouped');

                            }
                            for (prop in summarize) {
                                WOM.processSave(summarize[prop], 'Summarize');
                            }
                        }
                    }); // end are u sure?
                }
                else {
                    ISS.common.notify.error('There is no updates to save.');
                }
            }
            else {
                ISS.common.notify.error('Provide Valid Selling Style.');
            }
        }

        return false;
    },

   
    processSave:function(data,mode){
        ISS.common.executeActionAsynchronous(WOM.const.urlSaveWOMdata, JSON.stringify({ data: data, mode: mode }), function (stat, data) {
            if (stat && data) {
                if (data.mode == 'Delete') {
                    WOM.deleteCompleted(data);
                }
                else if (data.mode == 'Recalc') {
                    WOM.recalcCompleted(data);
                }
                else if (data.mode == 'Grouped') {
                    WOM.groupedCompleted(data);
                }
                else if (data.mode == 'Summarize') {
                    WOM.summarizeCompleted(data);
                }
                else if (data.mode == 'EditPFSUngroup') {
                    WOM.EditPFSUngroupCompleted(data);
                }
               WOM.saveCompleted()
            } // end stat
            else {
                ISS.common.blockUI();
                ISS.common.showPopUpMessage('Failed to save attributed work order details.');
            }

        }); // end ajax save
    },

    saveCompleted:function(){
        var data = WOM.const.saveItems.data;
        

        var flag = true;
        var succ = 0;
        var failed = 0;
        for (i = 0; i < data.length; i++) {
            if (!data[i].Completed) {
                flag = false;
                break;
            }
            if (data[i].IsHide) { }
            else if (data[i].ErrorStatus)++failed;
            else ++succ;
        }
        if (flag) {
            ISS.common.blockUI();
            WOM.const.saveIsProgress = false;
            var tp = ISS.common.MsgType.Info;
            if (failed == 0) tp = ISS.common.MsgType.Success;
            if (succ == 0) tp = ISS.common.MsgType.Error;

            var SMsg = ((succ > 0) ? (succ + " Order(s) updated successfully. <br/>") : '') +
               ((failed > 0) ? (failed + " Order(s) has errors ") : "");

            if (failed == 0) {
                ISS.common.showPopUpMessage(SMsg, tp, function (ret) {
                    WOM.loadWOMGrid(true);
                });
            }
            else {
                //ISS.common.showConfirmMessage(SMsg + '<br/><br/>Changes are pending <br/> Do you want to continue with refreshing and  losing your changes?', null, function (rett) {
                //    if (rett) {
                //        WOM.loadWOMGrid(true);
                //    }
                //    else {
                        ISS.common.clearGridFilters(gridWOM); 
                        var th = $('.k-grid-PendingGrid');
                        if (th.hasClass('k-state-selected')) {
                            th.removeClass('k-state-selected');
                        }
                //    }
                //});
            }
            gridWOM.refresh();
        }
    },

    EditPFSUngroupCompleted: function (data) {
        //SuperOrder  Item
        var group = WOM.const.saveItems.EditPFSUngroup;
      
            var item = group[data.data[0].SuperOrder];
            if (item) {
                item.ErrorStatus =data.data[0].ErrorStatus;
                item.ErrorMessage = data.data[0].ErrorMessage;
                
                    item.Completed = true;
            }
        
        if (!data.Status) {
            ISS.common.notify.error('Failed to update order details.');
        }
    },

    groupedCompleted: function (data) {
        //GroupId List
            var grouped = WOM.const.saveItems.Grouped;       
            var order = data.data[0].GroupId;
            if (grouped[order]) {
                var item = grouped[order];
                for (i = 0; i < data.data.length; i++) {
                    item[i].ErrorStatus = data.data[i].ErrorStatus;
                    item[i].ErrorMessage = data.data[i].ErrorMessage;
                    
                        item[i].Completed = true;
                }
               
            }           
        if (!data.Status) {
            ISS.common.notify.error('Failed to update order details.');
        }
    },
    summarizeCompleted: function (data) {
        //GroupId List
        var summarized = WOM.const.saveItems.Summarize;
            var order = data.data[0].GroupId;
            if (summarized[order]) {
                var item = summarized[order];
                for (i = 0; i < data.data.length; i++) {
                    item[i].ErrorStatus = data.data[i].ErrorStatus;
                    item[i].ErrorMessage = data.data[i].ErrorMessage;                    
                        item[i].Completed = true;
                }               
            }           
        if (!data.Status) {
            ISS.common.notify.error('Failed to update summarized detail.');
        }
    },
    recalcCompleted: function (data) {
        //SuperOrder  Item
        var group = WOM.const.saveItems.Recalc;
       
            var item = group[data.data[0].SuperOrder];
            if (item) {
                item.ErrorStatus = data.data[0].ErrorStatus;
                item.ErrorMessage = data.data[0].ErrorMessage;
                 
                    item.Completed = true;
            }
        if (!data.Status) {
            ISS.common.notify.error('Failed to update order details.');
        }
    },
    deleteCompleted: function (data) {
        //List
        var list = WOM.const.saveItems.Deleted;
        for (i = 0; i < list.length; i++) {
            if (data.data[i].IsDeleted) {               
                list[i].Completed = true;
                list[i].SuperOrder = null;
            }
            else {              
                list[i].Completed = true;
                list[i].ErrorStatus = true;
                list[i].ErrorMessage = 'Failed to delete order.';
            }
        }
        if (data.Status) {
            ISS.common.notify.error(list.length + ' record(s) deleted successfully.');
        }
        else{
            ISS.common.notify.error('Failed to delete '+data.failed+' record(s).');
        }
       
    },
    setFormSaveMode: function (t) {
        if (t) {
            WOM.const.saveIsProgress = true;
            WOM.const.saveItems = {
                Deleted:[], //Single list
                EditPFSUngroup: {   }, // superorder item
                Grouped: {    },// groupID, List
                Recalc: {}, // superorder item
                Summarize:{},
                data : null,
            };
        }
        else {
            WOM.const.saveIsProgress = false;
            WOM.const.saveItems = {};
        }
    },

    needRecalculation:function(d){
        var order = d.Cloned;
        if (!order) return false;
        return (
           !ISS.common.equals(d.Style, order.Style) ||
           !ISS.common.equals(d.Color, order.Color) ||
           !ISS.common.equals(d.Attribute, order.Attribute) ||
           !ISS.common.equals(d.Size, order.Size) ||
           !ISS.common.equals(d.Revision, order.Revision) ||
          
           !ISS.common.equals(d.MfgPathId, order.MfgPathId) ||
           !ISS.common.equals(d.DcLoc, order.DcLoc)
             //!ISS.common.equals(d.AltId, order.AltId) ||
          // !ISS.common.equals(d.TxtPath, order.TxtPath) ||
          // !ISS.common.equals(d.CutPath, order.CutPath) ||
          // !ISS.common.equals(d.SewPath, order.SewPath) ||
          // !ISS.common.equals(d.Atr, order.Atr) ||
         // || !ISS.common.equals(d.BOMUpdate, order.BOMUpdate) 
           //|| !ISS.common.equals(d.Note, order.Note)
            );

    },
    isSaveInProgress: function () {
        return WOM.const.saveIsProgress;
    },

   
}

$.extend(WOM, tWOM);
 