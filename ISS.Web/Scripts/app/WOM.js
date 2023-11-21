var gridWOM = null;
var tool = null;
var worevcd = null;

function setGrid () {
    if (!gridWOM) {
        gridWOM = $("#grdWOM").data("kendoGrid");
        if (gridWOM) {
            WOM.const.columns = new Array();
            $.merge(WOM.const.columns, gridWOM.columns)
        }
    }
}
 WOM = {

    const: {
        IsInit: true,
        currentRow: null,
        currentPopUpRow: null,
        editPopUp: null,
        validator: null,
        validatorMass:null,
        validatorEdit: null,       
        RevPopup: null,
        DmdPopup: null,
        SKUSizeList: [],
        OrderStatus: { Locked: 'Locked', Released: 'Released', Suggested: 'Suggested' },
        saveIsProgress: false,
        saveItems: {},
        NotePopup: null,
        columns:null,
    },
      
    addInputClass: function (frm) {
        $(frm + ' input:not(:checkbox):not(:radio):not(:button):not(:reset):not(:submit)').addClass('InputF');
    },

    docWOMReady: function (IsLoad) {
       
        WOM.addInputClass('#frmWOMSearch')
        WOM.addInputClass('#frmWOMMassChange')
        WOM.addInputClass('#frmWOMEdit')
        ISS.common.toUpperCase('.InputF:not(.excludeF)');
        ISS.common.toInt('.IntField');
        WOM.const.validator = $('#frmWOMSearch').kendoValidator().data("kendoValidator");
        WOM.const.validatorMass = $('#frmWOMMassChange').kendoValidator().data("kendoValidator");

        $('#btnWOMSearch').bind('click', function () { WOM.loadWOMGrid(false); return false; });

        $('#btnDemandSave').bind('click', WOM.saveDemandDetails);
        $('#btnDemandCancel').bind('click', WOM.canceldmdEdit);
        $('#frmWOMSearch .InputF').keypress(function (e) {
            if (e.which == 13) {
                WOM.loadWOMGrid(false);
                return false;
            }
        });

        

        $('#btnWOMClear').bind('click',WOM.clearWOMbtnClear);
        $('#btnWOMMassClear').bind('click', function () {
            $('#frmWOMMassChange input:checkbox').uncheck();
            WOM.const.validatorMass.hideMessages();
        });

        $('#btnWOMMassChange').bind('click', WOM.massChangeClick);
        setTimeout('setGrid()', 2000);

        $('.k-grid-GroupGrid').bind('click', WOM.GroupGrid);
        $('.k-grid-UngroupGrid').bind('click', WOM.UngroupGrid);
        $('.k-grid-PendingGrid').bind('click', WOM.PendingGrid);
        $('.k-grid-SelectAllGrid').bind('click', WOM.selectAllRows);

        $('.k-grid-btndeletemulti').bind('click', WOM.deleteMultiple);

        $('.k-grid-btnundomulti').bind('click', WOM.undoMultiple);

        $('#btnWOMSave').bind('click', WOM.saveWOMdata);

        tool = $(".Filters").kendoTooltip({
            filter: "a",
            autoHide: false,
            content: kendo.template($("#kntemplate").html()),
            width: 450,
            height: 160,
            position: "bottom"
        });
        $("#kntemplate").empty()
        $(document).on('change', '#frmFillgrid :checkbox', function () {
            WOM.fillGridFilter();
            WOM.searchDataWOM();
        });
        $('#BOMMismatches').bind('change', function () {
            if ($(this).prop('checked'))
                $('#frmFillgrid #ReleasedLots, #ReleasedLotsThisWeek').prop('checked', $(this).prop('checked'))
        });
        WOM.docWOMReady2();
        WOM.docWOMReady3();
        $('#btnWOMExport').bind('click', WOM.exportWOMDetails);
        $('input[id="Views"]').bind('change', WOM.changeViews);
        $('#btnCancel').bind('click', WOM.cancelAllChanges);
    },
   
    hasPendingChanges:function(){

    },
    BUChange: function (e) {

        //var season = $("#Season").data("kendoDropDownList").dataSource.read();

        //requisitions.validateRequisitionFull();

    },
    cancelAllChanges: function () {
        if (WOM.getPendingChangesData().length > 0) {
            ISS.common.showConfirmMessage('Changes are pending.<br/> Do you want to continue with Searching and losing your changes?', null, function (ret) {
                if (ret) {
                    WOM.loadWOMGrid(true);
                }
            })
        }
        else {
            ISS.common.notify.info('No changes found.')
        }
        return false;
    },

    getPendingChangesData:function(){
        var filter = WOM.getPendingChangeFilter();
        var query = new kendo.data.Query(gridWOM.dataSource.data());
        return query.filter(filter).data;
    },

    SearchWOM: function () {

    },
    getFilterGridValues: function () {
        if ($('#frmFillgrid [type="checkbox"]').length > 0) {
            return ISS.common.getFormData($('#frmFillgrid [type="checkbox"]'));
        } else {
            var lst = getFilterStaticVal();
            return lst;
        } 
    },
    searchDataWOM: function () {
        var ret = ISS.common.getFormData($('#frmWOMSearch'));
        $.extend(ret, WOM.getFilterGridValues());
        ret.BOMMismatches = $('#BOMMismatches').prop('checked');
        //ret.SuggestedLots = $('#SpillOver').prop('checked');
        var plannr = $("#Planner").data("kendoMultiSelect");
        ret.Planner = plannr.value().toString();
        var workCenter = $("#WorkCenter").data("kendoMultiSelect");
        ret.WorkCenter = workCenter.value().toString();

        // US 56194 ISS Web Attr Filter

        var capGroup = $("#CapacityGroup").data("kendoMultiSelect");
        ret.CapacityGroup = capGroup.value().toString();

        var corpdiv = $("#CorpDiv").data("kendoMultiSelect");
        ret.CorpDiv = corpdiv.value().toString();
        var bu = $("#BusinessUnit").data("kendoMultiSelect");
        if (bu._request && bu._old.length>0) {
            ret.BusinessUnit = bu._old[0];
        } else {
            ret.BusinessUnit = bu.value().toString();
        }
        ret.Src = WOM.const.Src;
        $('#gridData').val(JSON.stringify(ret));
        return ret;
    },

    loadWOMGrid: function (skip) {
        //TBD edit validation check
       // //WOM.clearFields();
        if (WOM.const.IsInit) {
            setGrid();
            gridWOM.table.attr('tabindex', 0);
            gridWOM.table.keydown(WOM.SelectUsingShifytKey);
            gridWOM.lockedTable.attr('tabindex', 0);
            //gridWOM.lockedTable.hover(function () {
            //   // this.focus();
            //}, function () {
            //    //this.blur();
            //})
            gridWOM.lockedTable.keydown(WOM.SelectUsingShifytKey);
            
            gridWOM.lockedTable.on('click', '.lnkStyle', function (e) {
                WOM.showProdOrders(e);
            });
            //Asif 10/9/2018 To Add Selling Sku popup while clicking in Selling Style in WOM screen
            gridWOM.lockedTable.on('click', '.lnkSellStyle', function (e) {
                WOM.showSellsku(e);
            });
            gridWOM.table.on('click', '.lnkLbsStr', function (e) {
                WOM.showProdOrdersLbs(e);
            });
            gridWOM.table.on('click', '.lnkPipeDate', function (e) {
                WOM.showPipelineDates(e);
            });
            gridWOM.table.on('click', '.lnkPFS', function (e) {
                WOM.showPFSDetailsGrid(e);
            });
            gridWOM.lockedTable.on('click', '.lnkWOSellStyle', function (e) {
                WOM.gotoCreateWO(e);
            });
           
            WOM.const.IsInit = false;
        }
        if (! skip && WOM.getPendingChangesData().length > 0) {
            ISS.common.showConfirmMessage('Changes are pending.<br/> Do you want to continue with Searching and losing your changes?', null, function (ret) {
                if (ret) {
                    WOM.loadWOMGrid(true);                    
                }
            });
            return false;
        }
        if (WOM.validateSelection()) {
            WOM.clearEditFields();
            WOM.clearPendingGridAction();        
            if (gridWOM.pager.page() > 1) gridWOM.pager.page(1);
            gridWOM.dataSource.data([])
            WOM.const.Src = "A";
            gridWOM.dataSource.read();
            WOM.const.Src = null;


            //field: "FromStyle"operator: "eq"value: "M0372"
            //"neq"
            ISS.common.collapsePanel('#panelbar-images', 0);
            ISS.common.expandPanel('#panelbar-images', 2);
        }
        return false;
    },

    validateSelection:function(){
        var StrQ = $("#SStyle").val() +
        $("#SColor").val() +
        $("#SAttribute").val() +
        $("#SSize").val() +
        $("#DC").val() +
        $("#Rev").val() +

        $("#MfgPathId").val() +
        $("#Rule").val() +
        $("#GroupId").val() +
        $("#MFGPlant").val() +
        $("#CylinderSize").val() +
        $("#DyeBle").val() +
        $("#TextileGroup").val() +
        $("#Alt").val() +
        $("#Machine").val() +
        $("#Yarn").val() +
        $("#Fabric").val();     //Newly Added for fabric item.
        if (StrQ == '') {
            var plannr = $("#Planner").data("kendoMultiSelect");
            StrQ = plannr.value().toString();
        }
        if (StrQ == '') {
            var corpdiv = $("#CorpDiv").data("kendoMultiSelect");
            StrQ = corpdiv.value().toString();
        }
        if (StrQ == '') {
            var workCenter = $("#WorkCenter").data("kendoMultiSelect");
            StrQ = workCenter.value().toString();
        }
        if (StrQ == '') {
            var bu = $("#BusinessUnit").data("kendoMultiSelect");
            if (bu._request && bu._old.length > 0) {
                StrQ = bu._old[0];
            } else {
                StrQ = bu.value().toString();
            }
        }
        if (StrQ == '') {
            ISS.common.showPopUpMessage('Must fill in the selection criteria.',ISS.common.MsgType.Warning);
            return false;
        }
        return true;
    },

    clearPendingGridAction:function(){
        var th = $('.k-grid-PendingGrid');
        if (th.hasClass('k-state-selected')) {
            ISS.common.clearGridFilters(gridWOM);
            th.removeClass('k-state-selected');
        }

    },

    SelectUsingShifytKey: function (e) {
        ISS.common.SelectUsingShifytKey(e, WOM.onRowsSelected,gridWOM)
    },

    showPFSDetailsGrid:function(e){
        var dataItem = gridWOM.dataItem($(e.currentTarget).closest("tr"));
        WOM.showPFSDetails(dataItem);
    },
    showPipelineDates: function (e) {
        var dataItem = gridWOM.dataItem($(e.currentTarget).closest("tr"));
        settings = {
            columns: [{
                Name: "CategoryCode",
                Title: "Pipeline",
            },
             {
                 Name: "Info",
                 Title: "Info",
             },
            {
                Name: "Plant",
                Title: "Plant",
            },

            {
                Name: "StartDateStr",
                Title: "Start Date",
            },
            {
                Name: "EndDateStr",
                Title: "End Date",
            },
              {
                  Name: "StartWeek",
                  Title: "Start Week",
              }],
            AllowSelect: false,
            title: 'Pipeline Dates',
            url: WOM.const.urlPipelineDates,
            postData: { SuperOrder: dataItem.SuperOrder },
            handler: function (d) {
            }
        };
        ISS.common.CommonSearchShow(settings);

    },
    showProdOrdersLbs: function (e) {
        var dataItem = gridWOM.dataItem($(e.currentTarget).closest("tr"));
        settings = {
            columns: [{
                Name: "Style",
                Title: "Style",
            },
            {
                Name: "Color",
                Title: "Color",
            },
            {
                Name: "Attribute",
                Title: "Attribute",
            },
            {
                Name: "Size",
                Title: "Size",
            },
            {
                Name: "DcLoc",
                Title: "Plant",
            },
            {
                Name: "Qty",
                Title: "Qty",
                CssClass: "ob-right",
            }, {
                Name: "OrderId",
                Title: "Id",
            }],
            AllowSelect: false,
            title: 'Production Orders',
            url: WOM.const.urlProdOrderLbs,
            postData: { SuperOrder: dataItem.SuperOrder },
            handler: function (d) {
            }
        };
        ISS.common.CommonSearchShow(settings);

    },


    showProdOrdersLbs: function (e) {
        var dataItem = gridWOM.dataItem($(e.currentTarget).closest("tr"));
        settings = {
            columns: [{
                Name: "Style",
                Title: "Style",
            },
            {
                Name: "Color",
                Title: "Color",
            },
            {
                Name: "Attribute",
                Title: "Attribute",
            },
            {
                Name: "Size",
                Title: "Size",
            },
            {
                Name: "DcLoc",
                Title: "Plant",
            },            
            {
                Name: "Qty",
                Title: "Qty",
                CssClass: "ob-right",
            }, {
                Name: "OrderId",
                Title: "Id",
            }],
            AllowSelect: false,
            title: 'Production Orders',
            url: WOM.const.urlProdOrderLbs,
            postData: { SuperOrder: dataItem.SuperOrder },
            handler: function (d) {
            }
        };
        ISS.common.CommonSearchShow(settings);

    },



    showProdOrders:function(e){
        var dataItem = gridWOM.dataItem($(e.currentTarget).closest("tr"));
        
        
        settings = {
            columns: [{
                Name: "Style",
                Title: "Style",               
            },
            {
                Name: "Color",
                Title: "Color",               
            },
            {
                Name: "Attribute",
                Title: "Attribute",
            },
            {
                Name: "Size",
                Title: "Size",               
            },
            {
                Name: "DcLoc",
                Title: "Plant",               
            },
            {
                Name: "CurrDueDateStr",
                Title: "Sew Date",
                //Format: "{0:" + ISS.common.Settings.DateDiplay + "}"
            },
            {
                Name: "Qty",
                Title: "Qty",
                CssClass: "ob-right",
            },{
                Name: "WorkCenter",
                Title: "Work Center",               
            }],
            AllowSelect: false,
            title: 'Production Orders',
            url: WOM.const.urlProdOrder,
            postData: { SuperOrder: dataItem.SuperOrder },
            handler: function (d) {               
            }            
        };
        ISS.common.CommonSearchShow(settings);

    },


     //Asif 10/9/2018 To Add Selling Sku popup while clicking in Selling Style in WOM screen

    showSellsku: function (e) {
        var dataItem = gridWOM.dataItem($(e.currentTarget).closest("tr"));
               
        settings = {
            columns: [{
                Name: "SellingStyle",
                Title: "Selling Style",
            },
            {
                Name: "Color",
                Title: "Selling Color",
            },
            {
                Name: "Attribute",
                Title: "Selling Attribute",
            },
            {
                Name: "Size",
                Title: "Selling Size",
            },
             {
                 Name: "SizeShortDes",
                 Title: "Size Desc",
             },
            {
                Name: "DcLoc",
                Title: "Plant",
            },
            {
                Name: "CurrDueDateStr",
                Title: "Sew Date",
                //Format: "{0:" + ISS.common.Settings.DateDiplay + "}"
            },
            {
                Name: "Qty",
                Title: "Qty",
                CssClass: "ob-right",
            }, {
                Name: "WorkCenter",
                Title: "Work Center",
            }],
            AllowSelect: false,
            title: 'Selling Sku',
            url: WOM.const.urlSellsku,
            postData: { SuperOrder: dataItem.SuperOrder },
            handler: function (d) {
            }
        };
        ISS.common.CommonSearchShow(settings);

    },
 
    gridDataBound: function (e) {
        if (!gridWOM.dataSource.filter()) {
            WOM.fillGridFilter();
        }
        $('#FilteredColumns').html(ISS.common.getFilteredColumns("#" + e.sender.content.context.id));
        $('.k-grid-SelectAllGrid').text("Select All");
        var gridData = gridWOM.dataSource.view();
        var exclude = new Array();
        exclude.push('ErrorMessage');
        for (var i = 0; i < gridData.length; i++) {
            var row = gridWOM.element.find("tr[data-uid='" + gridData[i].uid + "']");
            var item = gridData[i];
            if (!gridData[i].IsDeleted && gridData[i].ErrorStatus) {
                row.addClass("highlighted-row");
            }
            else {
                row.removeClass("highlighted-row");
            }
            if (!(item.Completed && !item.ErrorStatus)) {
                if (item.IsDeleted) {
                    row.addClass("deleted-row").find('.k-grid-DeleteItem span').removeClass("k-icon k-delete").addClass("k-icon k-i-undo");
                    row.find('.chkbx').hide();
                }
                else {
                    row.removeClass("deleted-row").find('.k-grid-DeleteItem span').removeClass("k-icon k-i-undo").addClass("k-icon k-delete");
                }

                if (!WOM.IsAllowEdit(item)) {
                    row.find('.k-grid-EditItem,.k-grid-DeleteItem').hide();
                    //var editButton = gridWOM.tbody.find("tr[data-uid='" + gridData[i].uid + "'] .k-grid-edit-row");
                    //editButton.addClass("k-state-disabled");
                    //gridWOM.element.find("tr[data-uid='" + gridData[i].uid + "']").cancelRow();
                    //gridWOM.tbody.find("tr[data-uid='" + gridData[i].uid + "'] .k-grid-edit").remove();
                    //gridWOM.tbody.find("tr[data-uid='" + gridData[i].uid + "']").prevObject[0].isTextEdit = true;
                    //this.closeCell();
                }
                else if (item.IsDeleted) {
                    row.find('.k-grid-EditItem').hide();
                }

                if (gridData[i].UpdatedBy == "SVCADEXA") {
                    var cell = $("td:nth-child(6)", row);
                    cell.addClass("highlighted-rowgreen");
                }
                if ((((gridData[i].Cloned) ? gridData[i].Cloned.OrderStatus : gridData[i].OrderStatus) == 'R') && item.RemoteUpdateCode == 'F') {
                    row.addClass("highlighted-row")
                }

                ISS.common.highlightEditedCells(gridData[i], row, gridWOM.columns, exclude, false)
            }
            else {
                row.addClass('completed-row').find('.k-grid-EditItem,.k-grid-DeleteItem').hide();
            }
        }
        $(".k-grid-EditItem").find("span").addClass("k-icon k-edit");
        WOM.onRowsSelected();

        var tooltip = $(".highlighted-row").kendoTooltip({
            filter: ".k-warning",
            content: kendo.template($("#Errtemplate").html()),
            width: 120,
            position: "top"
        }).data("kendoTooltip")

    },

    //returns locked only
    selectedRows:function(del){
        ///k-grid-content-locked
        var arr = gridWOM.select();
        var  rows=new Array();
        arr.each(function (i, item) {
            if (del || (!$(item).hasClass('deleted-row'))) {
                if ($(item).closest('div').hasClass('k-grid-content-locked')) {
                    rows.push(item);
                }
            }
        });
        return rows;
    },
    IsAllowEdit: function (item) {
        if (item) {
            return !(item.MakeOrBuy == 'B' || (item.MakeOrBuy == 'M' &&
               ((((item.Cloned) ? item.Cloned.OrderStatus : item.OrderStatus) == 'R') && item.RemoteUpdateCode != 'F')));
        }
       // return !( item.MakeOrBuy == 'B' || (item.MakeOrBuy == 'M' && (item.OrderStatus == 'R' && item.RemoteUpdateCode !='F')));
    },

    OnCapacityChange: function (e) {
        //onPlannerClose(e);

        var wcMultiSelect = $("#WorkCenter").data("kendoMultiSelect");
        wcMultiSelect.dataSource.read();
    },

    removeOrderDetail: function (e) {                    
            var curRow = $(e.currentTarget).closest("tr");
            var dataItem = gridWOM.dataItem(curRow);
            if (dataItem) {
                if (!dataItem.IsDeleted) {
                    ISS.common.showConfirmMessage("Are you sure to delete this record?", null, function (ret) {
                        if (ret) {                             
                                dataItem.IsDeleted = !dataItem.IsDeleted;
                                dataItem.IsDirty = true;
                                if (dataItem.GroupId != null) {
                                    var garr = WOM.getAllMSKU(dataItem.GroupId, dataItem.SuperOrder);
                                    for (j = 0; j < garr.length; j++) {
                                        garr[j].IsDeleted = true;
                                    }
                                }
                                gridWOM.refresh();
                        }
                    });
                }
                else {
                    dataItem.IsDeleted = !dataItem.IsDeleted;
                    if (dataItem.GroupId != null) {
                        var garr = WOM.getAllMSKU(dataItem.GroupId, dataItem.SuperOrder);
                        for (j = 0; j < garr.length; j++) {
                            garr[j].IsDeleted = false;
                        }
                    }
                    gridWOM.refresh();
                } 
            }
         
        return false;
    },

    deleteMultiple: function () {         
        var rows = $(WOM.selectedRows()).not('.deleted-row');
        if (rows.length > 0) {
            ISS.common.showConfirmMessage('Are you sure to delete this record(s)?', null, function (ret) {
                if (ret) {
                    var arr = new Array();
                    for (i = 0; i < rows.length; i++) {
                        var dataItem = gridWOM.dataItem(rows[i]);
                        if (WOM.IsAllowEdit(dataItem)) {
                            dataItem.IsDeleted = true;
                            if (dataItem.GroupId != null) {
                                var garr = WOM.getAllMSKU(dataItem.GroupId, dataItem.SuperOrder);
                                for (j = 0; j < garr.length; j++) {
                                    garr[j].IsDeleted = true;
                                }
                            }
                        }//end allow edit
                        else {
                            ISS.common.notify.error('Edit not allowed.')
                        }
                    }
                    gridWOM.refresh(); 
                }
            });
        }
        else {
            ISS.common.showPopUpMessage('Please select at least one record.');
        }
        return false;
    },

    editOrderDetail: function (e) {
        var curRow = $(e.currentTarget).closest("tr");
        var dataItem = gridWOM.dataItem(curRow);

        if (dataItem) {
            WOM.editRow(dataItem);
        }
        return false;
    },

    onRowsSelected: function (arg) {
        var rows = WOM.selectedRows();
        var TotDz = 0.0;
        var TotFQDz = 0.0;
        var TotLbs = 0.0;
        var TotFinishedlbs = 0.0;

        $(rows).each(function (idx, item) {
            var data = gridWOM.dataItem(item);
            //WOM.const.currentRow = data;
            //WOM.const.currentRow.Cloned = null;
            //data.Cloned = WOM.const.currentRow; 
            if (!data.Cloned) {
                ISS.common.cloneAndStore(data);
            }
                      
            if (!data.CCurrDueDate) {
                data.CCurrDueDate = data.CurrDueDate;
            }
            if (!data.CStartDate) {
                data.CStartDate = data.StartDate;
            }
            window.sstyle = data.Style;
            window.ssstyle = data.SellingStyle;
            window.scolor = data.Color
            window.sattribute = data.Attribute;
            window.PDC = data.DcLoc;
            window.superO = data.SuperOrder;
            window.cPath = data.CutPath;
            window.cPath = data.CuttPath;
            window.sssize = data.Size;
            window.ssizecode = data.SizeShortDes;
            //if (window.sssize == undefined) {
            //    window.sssize = data.Size;
            //    window.ssizecode = data.SizeShortDes;
            //}
            window.srevision = data.Revision;
            if (data && !data.IsDeleted) {
                TotDz +=data.TotalDozens;
                TotFQDz += parseInt(data.QtyDZ);
                TotLbs += data.LbsStr ? data.LbsStr : 0;
                TotFinishedlbs += (data.Lbs) ? data.Lbs : 0;
            }
            else {
               // $(item).removeClass('k-state-selected');
            }
        });
        $('#lblDZ').text(TotDz);
        $('#lblFQDZ').text(TotFQDz);
        $('#lblGreige').text(Math.round(TotLbs * 100) / 100);
        $('#lblFinish').text(Math.round(TotFinishedlbs));
        
    },

    selectAllRows: function (e) {
        var th = $(this);
        if (th.text() == "Select All") {
            gridWOM.element.find('tbody tr:not([style*="display: none"])').addClass('k-state-selected');
            th.text("UnSelect All"); //:not(.deleted-row)
        }
        else {
            gridWOM.element.find('tbody tr.k-state-selected').removeClass('k-state-selected');
            th.text("Select All");
        }
        WOM.onRowsSelected();
        return false;
    },

    GroupGrid: function (e) {
        var rows = WOM.selectedRows();
        var SKUGroup = null;
        var SKUGroupDate = null;

        var dataColl = new Array();
        var arrSize = new Array();
        var flag=true;
        if (rows.length > 1) {
            for(i=0;i<rows.length;i++){
                var data = gridWOM.dataItem(rows[i]);
                if (data && !data.IsDeleted) {
                    if (WOM.IsAllowEdit(data)) {
                        if (data.GroupId != null || data.IsUngrouped) {
                            flag = false;
                            ISS.common.showPopUpMessage('Selected row is not a single SKU record.' + ((data.IsUngrouped) ? ' [Ungrouped record]' : ''));
                            break;
                        }
                        else {
                            var groupFields = data.Style + data.Color + data.Attribute + data.MfgPathId
                            + data.Revision
                            + data.SewPath
                            + data.TxtPath
                            + data.CutPath
                            + (data.AltId ? data.AltId : '');

                            if (SKUGroup != null) {
                                if (SKUGroup != groupFields || (!ISS.common.isEqualDate(SKUGroupDate, data.CurrDueDate))) {
                                    flag = false;
                                    ISS.common.showPopUpMessage('Multi Sku order must match by Selling sku, revision, sew cut txt plants alternate and due date ');
                                    break;
                                }
                                else if ($.inArray(data.Size, arrSize) >= 0) {
                                    flag = false;
                                    ISS.common.showPopUpMessage("Duplicate Size  " + data.SizeShortDes + " in Multi SKU " + groupFields); break;
                                }
                                else {

                                }
                            } else {
                                SKUGroup = groupFields; SKUGroupDate = data.CurrDueDate
                            }
                            arrSize.push(data.Size);
                            dataColl.push(data);
                        }// end else isMSKU
                    } else {
                        ISS.common.notify.error('Edit not allowed.')
                    }
                    }
            }// end for
            if (flag) {
                //TBD
                if (dataColl.length > 1) {
                    ISS.common.execute('GetGroupID', null, function (s, d) {
                        if (s && d) {
                            for (j = 0; j < dataColl.length; j++) {
                                var gdata = dataColl[j];
                                ISS.common.cloneAndStore(gdata);
                                gdata.IsGrouped = true;
                                gdata.GroupId = d + "";
                                gdata.OrderStatus = 'L';
                                WOM.setOrderType(gdata);
                                gdata.IsFieldChange = true;
                                gdata.IsEdited = true;
                            }
                            gridWOM.refresh();
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
        
        var rows = WOM.selectedRows();
        var GroupId = null;
        var flag = true;
        var arr = new Array();
        if (rows.length > 0) {
            for (i = 0; i < rows.length; i++) {
                var data = gridWOM.dataItem(rows[i]);
                if (data  && !data.IsDeleted) {
                    if (WOM.IsAllowEdit(data)) {
                        if (data.GroupId == null) {   //|| data.IsGrouped                       
                            flag = false;
                            ISS.common.showPopUpMessage('Selected row is not a multi-SKU record.');
                            //+((data.IsGrouped) ? ' [Grouped record]' : '')
                            break;
                        }
                        if (GroupId != null) {
                            if (GroupId != data.GroupId) {
                                flag = false;
                                ISS.common.showPopUpMessage('Not able to ungroup selected rows. Group Id is different');
                                break;
                            }
                        } else {
                            GroupId = data.GroupId;
                        }
                        arr.push(data)
                    }
                    else {
                        flag = false;
                        ISS.common.notify.error('Edit is not allowed.')
                    }
                }
            }// end for
            if (flag) {                
                ISS.common.showConfirmMessage('Do you want to Ungroup all the selected Multi SKU records?', null, function (ok) {
                    if (ok) {
                        var groups = WOM.getAllMSKU(GroupId, 'xx');
                        for (i = 0; i < groups.length; i++) {
                            var gdata = groups[i];
                            WOM.unGroupData(gdata);
                        }                        
                    }
                    else {
                        //for (i = 0; i < arr.length; i++) {
                        //    var gdata = arr[i];
                        //    WOM.unGroupData(gdata);
                        //}
                    }
                    gridWOM.refresh();
               // }, 'Yes', 'Selected Row(s) Only');
               
                }, 'Yes', 'No');
            }
        }
        else {
            ISS.common.showPopUpMessage('Please select at least one row.')
        }
        
        return false;
    },
    unGroupData: function (gdata) {
        ISS.common.cloneAndStore(gdata);
        gdata.IsUngrouped = (gdata.Cloned.GroupId != null) ;
        gdata.IsGrouped = false;
        gdata.GroupId = null;
        gdata.OrderStatus = 'S';
        WOM.setOrderType(gdata);
        gdata.IsFieldChange = true;
        gdata.IsEdited = true;
    },
    PendingGrid: function (e) {
        var th = $(this);
        
        if(th.hasClass('k-state-selected')){  
            th.removeClass('k-state-selected');
            ISS.common.clearGridFilters(gridWOM);          
        }
        else{
            th.addClass('k-state-selected');         
              
            gridWOM.dataSource.filter(WOM.getPendingChangeFilter());
            if (gridWOM.dataSource.page() > 1) gridWOM.dataSource.page(1); 
            
        }
        
        return false;
    },

    getPendingChangeFilter: function () {
        var filt = ISS.common.getFilter("or");
        filt.filters.push(ISS.common.getFilterItem("IsDeleted", "eq", true));
        filt.filters.push(ISS.common.getFilterItem("IsEdited", "eq", true));
        return filt;
    },

    getEditRowFilter: function () {
        var filt = ISS.common.getFilter("or");
        filt.filters.push(ISS.common.getFilterItem("dirty", "eq", true));
        filt.filters.push(ISS.common.getFilterItem("IsDeleted", "eq", true));
        filt.filters.push(ISS.common.getFilterItem("IsEdited", "eq", true));
        return filt;
    },

   
    fillGridFilter: function () {
        var filtGroup =   ISS.common.getFilter("or");
        var checkedGridFilters = WOM.getFilterGridValues();
        if (checkedGridFilters.BodyOnly || checkedGridFilters.TrimOnly || checkedGridFilters.GroupOnly) {            
            if (checkedGridFilters.BodyOnly) {
                filtGroup.filters.push(ISS.common.getFilterItem("FilterGridCriteria", "contains", "BO,"));
            }
            if (checkedGridFilters.TrimOnly) {
                filtGroup.filters.push(ISS.common.getFilterItem("FilterGridCriteria", "contains", "T,"));
            }
            if (checkedGridFilters.GroupOnly) {
                filtGroup.filters.push(ISS.common.getFilterItem("FilterGridCriteria", "contains", "G,"));
            }
        }

        var filtStatus = ISS.common.getFilter("or");
        if (checkedGridFilters.SuggestedLots) {
            filtStatus.filters.push(ISS.common.getFilterItem("FilterGridCriteria", "contains", "S,"));
        }
        if (checkedGridFilters.LockedLots) {
            filtStatus.filters.push(ISS.common.getFilterItem("FilterGridCriteria", "contains", "L,"));
        }
        if (checkedGridFilters.ReleasedLots) {
            filtStatus.filters.push(ISS.common.getFilterItem("FilterGridCriteria", "contains", "R,"));
        }
        if (checkedGridFilters.SpillOver) {
            filtStatus.filters.push(ISS.common.getFilterItem("FilterGridCriteria", "contains", "SY,"));
        }
        if (checkedGridFilters.ReleasedLotsThisWeek) {
            filtStatus.filters.push(ISS.common.getFilterItem("FilterGridCriteria", "contains", "RW,"));
        }
        if (filtStatus.filters.length == 0) filtStatus.filters.push(ISS.common.getFilterItem("FilterGridCriteria", "neq", ""));


        var filtSource = ISS.common.getFilter("or");
        
        if (checkedGridFilters.CustomerOrders) {
            filtSource.filters.push(ISS.common.getFilterItem("FilterGridCriteria", "contains", "C,"));
            filtSource.filters.push(ISS.common.getFilterItem("DemandSource", "contains", "PO"));
            filtSource.filters.push(ISS.common.getFilterItem("DemandSource", "contains", "OE"));
        }
        if (checkedGridFilters.Events) {
            filtSource.filters.push(ISS.common.getFilterItem("FilterGridCriteria", "contains", "E,"));
        }
        if (checkedGridFilters.TILs) {
            filtSource.filters.push(ISS.common.getFilterItem("FilterGridCriteria", "contains", "TI,"));
        }
        if (checkedGridFilters.Forecast) {
            filtSource.filters.push(ISS.common.getFilterItem("FilterGridCriteria", "contains", "F,"));
        }
        if (checkedGridFilters.MaxBuild) {
            filtSource.filters.push(ISS.common.getFilterItem("FilterGridCriteria", "contains", "MB,"));
        }
        if (checkedGridFilters.StockTarget) {
            filtSource.filters.push(ISS.common.getFilterItem("FilterGridCriteria", "contains", "ST,"));
        }

        if (filtSource.filters.length > 0)
            filtSource.filters.push(ISS.common.getFilterItem("DemandSource", "eq", ""));
        else
            filtSource.filters.push(ISS.common.getFilterItem("DemandSource", "contains", ""));


        var filtBuyOrd = ISS.common.getFilter("and");
        if (checkedGridFilters.ExcludeBuyOrders) {
            filtBuyOrd.filters.push(ISS.common.getFilterItem("FilterGridCriteria", "doesnotcontain", "B,"));
        }
        
     
        
        var filter = ISS.common.getFilter("and");
        filter.filters.push(filtStatus);
        filter.filters.push(filtSource);
       
        if (filtBuyOrd.filters.length > 0) {
            filter.filters.push(filtBuyOrd);
        }
        if (filtGroup.filters.length > 0) {
            filter.filters.push(filtGroup);
        }
    

        WOM.clearPendingGridAction();
        if (filter && gridWOM.dataSource.data().length > 0) {
            $.merge(filter.filters, WOM.getCustomFilter());
            gridWOM.dataSource.filter(filter);
        }
    },

    getCustomFilter:function(){
        var f = gridWOM.dataSource.filter();
        var ar=new Array();
        if (f && f.filters) {
            for (i = 0; i < f.filters.length ; i++) {
                if (f.filters[i].field) ar.push(f.filters[i]);
            }
        }
        return ar;
    },

    exportWOMDetails: function() {
        var dsData = gridWOM.dataSource.data();

        if (dsData.length == 0) {
            ISS.common.showPopUpMessage('No details to export')
            return false;
        }
       $('#gridColumns').val( $.map(gridWOM.columns, function (v, i) { if (!v.hidden && v.field) return v.field }).join(","))
       // $('#frmWOMDetail').submit();
    },
    ShowNote: function (e) {
        var settings = {
            title: 'Note',
            animation: false,
            width: '450px',
            height: '200px'
        };
     
       
        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        $('#txtWOMNote').val(dataItem.Note);
        if (WOM.IsAllowEdit(dataItem)) {
            $('#btnWOMNote').show()
        }
        else {
            $('#btnWOMNote').hide()
        }
        var superOrder = { superOrder: dataItem.SuperOrder }
        if (dataItem.NoteInd == 'N') {
            superOrder = JSON.stringify(superOrder);

            ISS.common.executeActionAsynchronous("../Order/GetNote", superOrder, function (stat, data) {
                if (stat && data) {
                    $("#txtWOMNote").val(data);
                }
            });
        }       
        $("#hfWOMNote").val(dataItem.SuperOrder);
        WOM.const.NotePopup = ISS.common.popUpCustom('.divNotePopup', settings);
        return false;
    },

    SaveNote: function () {
        var superOrder = $('#hfWOMNote').val();
        
        var data = gridWOM.dataSource.data();
        
        var res = $.grep(data, function (d) {
            return d.SuperOrder == superOrder;
        });

        if(res.length>0){
            var dataitem = res[0];
            dataitem.Note = $('#txtWOMNote').val();
            dataitem.NoteInd = 'Y';
            dataitem.IsEdited = true;
        }

        $('#txtWOMNote').val('');
        WOM.const.NotePopup.close();
        return false;
    },



    setDefaultView:function(){ 
        var Coll = WOM.const.columns;


        gridWOM.showColumn("GroupId")
        gridWOM.reorderColumn(2, Coll[2])
        gridWOM.lockColumn("GroupId")

        gridWOM.showColumn("OrderId")
        gridWOM.reorderColumn(3, Coll[3])
        gridWOM.lockColumn("OrderId")
                
        

        gridWOM.showColumn("OrderType")
         gridWOM.reorderColumn(4,Coll[4])
         gridWOM.lockColumn("OrderType")

         

        gridWOM.showColumn("OrderStatusDesc")
         gridWOM.reorderColumn(5,Coll[5])
         gridWOM.lockColumn("OrderStatusDesc")
         

        gridWOM.showColumn("SellingStyle")
        gridWOM.reorderColumn(6,Coll[6])
        gridWOM.lockColumn("SellingStyle")

        gridWOM.showColumn("Style")
        gridWOM.reorderColumn(7,Coll[7])
        gridWOM.lockColumn("Style")


        gridWOM.showColumn("Color")
        gridWOM.reorderColumn(8,Coll[8])
        gridWOM.lockColumn("Color")


        gridWOM.showColumn("Attribute")
        gridWOM.reorderColumn(9,Coll[9])
        gridWOM.lockColumn("Attribute")


        gridWOM.showColumn("SizeShortDes")
        gridWOM.reorderColumn(10,Coll[10])
        gridWOM.lockColumn("SizeShortDes")


        gridWOM.showColumn("Revision")
        gridWOM.reorderColumn(11,Coll[11])
        gridWOM.lockColumn("Revision")

        gridWOM.unlockColumn("CylinderSizes")

        gridWOM.showColumn("AltId")
        gridWOM.reorderColumn(12, Coll[12])
        gridWOM.lockColumn("AltId")
       

        gridWOM.showColumn("QtyDZ")
        gridWOM.reorderColumn(13,Coll[13])


        gridWOM.showColumn("TotalDozens")
        gridWOM.reorderColumn(14,Coll[14])


        gridWOM.showColumn("LbsStr")
        gridWOM.reorderColumn(15,Coll[15])


        gridWOM.showColumn("MC")
        gridWOM.reorderColumn(16,Coll[16])


        gridWOM.showColumn("CylinderSizes")
        gridWOM.reorderColumn(17,Coll[17])


        gridWOM.showColumn("PFSInd")
        gridWOM.reorderColumn(18,Coll[18])


        gridWOM.showColumn("StartDate")
        gridWOM.reorderColumn(19,Coll[19])


        gridWOM.showColumn("CurrDueDate")
        gridWOM.reorderColumn(20,Coll[20])


        gridWOM.showColumn("TxtPath")
        gridWOM.reorderColumn(21,Coll[21])


        gridWOM.showColumn("CutPath")
        gridWOM.reorderColumn(22,Coll[22])


        gridWOM.showColumn("SewPath")
        gridWOM.reorderColumn(23,Coll[23])


        gridWOM.showColumn("Atr")
        gridWOM.reorderColumn(24,Coll[24])


        gridWOM.showColumn("MfgPathId")
        gridWOM.reorderColumn(25,Coll[25])


        gridWOM.showColumn("DcLoc")
        gridWOM.reorderColumn(26,Coll[26])


        gridWOM.showColumn("ExpeditePriority")
        gridWOM.reorderColumn(27,Coll[27])


        gridWOM.showColumn("CategoryCode")
        gridWOM.reorderColumn(28,Coll[28])

        
        gridWOM.showColumn("DemandDriver")
        gridWOM.reorderColumn(29, Coll[29])


        gridWOM.showColumn("DemandSource")
        gridWOM.reorderColumn(30,Coll[30])


        gridWOM.showColumn("Rule")
        gridWOM.reorderColumn(31,Coll[31])


        gridWOM.showColumn("Priority")
        gridWOM.reorderColumn(32,Coll[32])


        gridWOM.showColumn("CreateBd")
        gridWOM.reorderColumn(33,Coll[33])


        gridWOM.showColumn("DozensOnly")
        gridWOM.reorderColumn(34,Coll[34])


        gridWOM.showColumn("BOMUpdate")
        gridWOM.reorderColumn(35,Coll[35])


        gridWOM.showColumn("SpillOver")
        gridWOM.reorderColumn(36,Coll[36])


        gridWOM.showColumn("PackCode")
        gridWOM.reorderColumn(37,Coll[37])


        gridWOM.showColumn("MakeOrBuy")
        gridWOM.reorderColumn(38,Coll[38])


        gridWOM.showColumn("Note")
        gridWOM.reorderColumn(39,Coll[39])


        gridWOM.showColumn("DemandDate")
        gridWOM.reorderColumn(40,Coll[40])


    },


    setCylinderView: function () {
        var Coll = WOM.const.columns;


        gridWOM.showColumn("OrderId")
        gridWOM.reorderColumn(2, Coll[3])
        gridWOM.lockColumn("OrderId")

        gridWOM.showColumn("OrderStatusDesc")
        gridWOM.reorderColumn(3, Coll[5])
        gridWOM.lockColumn("OrderStatusDesc")


        gridWOM.showColumn("SellingStyle")
        gridWOM.reorderColumn(4, Coll[6])
        gridWOM.lockColumn("SellingStyle")


        gridWOM.showColumn("Style")
        gridWOM.reorderColumn(5, Coll[7])
        gridWOM.lockColumn("Style")


        gridWOM.showColumn("Color")
        gridWOM.reorderColumn(6,Coll[8])
        gridWOM.lockColumn("Color")

        gridWOM.showColumn("SizeShortDes")
        gridWOM.reorderColumn(7, Coll[10])
        gridWOM.lockColumn("SizeShortDes")

        gridWOM.showColumn("CylinderSizes")
        gridWOM.reorderColumn(8, Coll[17])
        gridWOM.lockColumn("CylinderSizes")

        gridWOM.showColumn("Attribute")
        gridWOM.reorderColumn(9, Coll[9])
        gridWOM.lockColumn("Attribute")     

        gridWOM.showColumn("Revision")
        gridWOM.reorderColumn(10, Coll[11])
        gridWOM.lockColumn("Revision")


        gridWOM.showColumn("AltId")
        gridWOM.reorderColumn(11, Coll[12])
        gridWOM.lockColumn("AltId") 

        gridWOM.unlockColumn("GroupId")
        gridWOM.unlockColumn("OrderType")



        gridWOM.showColumn("QtyDZ")
        gridWOM.reorderColumn(12, Coll[13]) //QtyDZ

        gridWOM.showColumn("TotalDozens")
        gridWOM.reorderColumn(13, Coll[14])//TotalDozens

        gridWOM.showColumn("Lbs")
        gridWOM.reorderColumn(14, Coll[15]) //LbsStr

        gridWOM.showColumn("MC")
        gridWOM.reorderColumn(15, Coll[16]) //MC


        gridWOM.showColumn("PFSInd")
        gridWOM.reorderColumn(16, Coll[18])


        gridWOM.showColumn("TxtPath")
        gridWOM.reorderColumn(17, Coll[21])


        gridWOM.showColumn("CutPath")
        gridWOM.reorderColumn(18, Coll[22])



        gridWOM.showColumn("SewPath")
        gridWOM.reorderColumn(19, Coll[23])


        gridWOM.showColumn("Atr")
        gridWOM.reorderColumn(20, Coll[24])



        gridWOM.showColumn("MfgPathId")
        gridWOM.reorderColumn(21, Coll[25])



        gridWOM.showColumn("DcLoc")
        gridWOM.reorderColumn(22, Coll[26])

        gridWOM.showColumn("Rule")
        gridWOM.reorderColumn(23, Coll[31])


        gridWOM.showColumn("StartDate")
        gridWOM.reorderColumn(24, Coll[19])


        gridWOM.showColumn("CurrDueDate")
        gridWOM.reorderColumn(25, Coll[20])


        gridWOM.showColumn("ExpeditePriority")
        gridWOM.reorderColumn(26, Coll[27])


        gridWOM.showColumn("CategoryCode")
        gridWOM.reorderColumn(27, Coll[28])


        gridWOM.showColumn("SpillOver")
        gridWOM.reorderColumn(28, Coll[34])



        gridWOM.showColumn("CreateBd")
        gridWOM.reorderColumn(29, Coll[33])


        gridWOM.showColumn("DozensOnly")
        gridWOM.reorderColumn(30, Coll[34])


        gridWOM.showColumn("GroupId")
        gridWOM.reorderColumn(31, Coll[2])


        gridWOM.showColumn("PackCode")
        gridWOM.reorderColumn(32, Coll[37])


        gridWOM.showColumn("BOMUpdate")
        gridWOM.reorderColumn(33, Coll[35])


        gridWOM.showColumn("MakeOrBuy")
        gridWOM.reorderColumn(34, Coll[38])


        gridWOM.showColumn("DemandDriver")
        gridWOM.reorderColumn(35, Coll[29])

        gridWOM.showColumn("DemandSource")
        gridWOM.reorderColumn(36, Coll[30])


        gridWOM.showColumn("Note")
        gridWOM.reorderColumn(37, Coll[39])


        gridWOM.showColumn("OrderType")
        gridWOM.reorderColumn(38, Coll[4])


        gridWOM.showColumn("DemandDate")
        gridWOM.reorderColumn(39, Coll[40])


        gridWOM.showColumn("Priority")
        gridWOM.reorderColumn(40, Coll[32])



    },

    setMSKUView: function () {
        var Coll = WOM.const.columns;


        gridWOM.showColumn("SellingStyle")
        gridWOM.reorderColumn(2, Coll[6])
        gridWOM.lockColumn("SellingStyle")

        gridWOM.showColumn("Style")
        gridWOM.reorderColumn(3, Coll[7])
        gridWOM.lockColumn("Style")


        gridWOM.showColumn("Color")
        gridWOM.reorderColumn(4, Coll[8])
        gridWOM.lockColumn("Color")


        gridWOM.showColumn("Attribute")
        gridWOM.reorderColumn(5, Coll[9])
        gridWOM.lockColumn("Attribute")


        gridWOM.showColumn("SizeShortDes")
        gridWOM.reorderColumn(6, Coll[10])
        gridWOM.lockColumn("SizeShortDes")

        gridWOM.showColumn("CylinderSizes")
        gridWOM.reorderColumn(7, Coll[17])
        gridWOM.lockColumn("CylinderSizes")

        gridWOM.showColumn("Revision")
        gridWOM.reorderColumn(8, Coll[11])
        gridWOM.lockColumn("Revision")


        gridWOM.showColumn("AltId")
        gridWOM.reorderColumn(9, Coll[12])
        gridWOM.lockColumn("AltId")

        gridWOM.unlockColumn("GroupId")
        gridWOM.unlockColumn("OrderId")
        gridWOM.unlockColumn("OrderType")
        gridWOM.unlockColumn("OrderStatusDesc")

        //gridWOM.hide("ErrorMessage")
     

        gridWOM.showColumn("MC")
        gridWOM.reorderColumn(10, Coll[16])

        gridWOM.showColumn("GroupId")
        gridWOM.reorderColumn(11, Coll[2])



        gridWOM.showColumn("OrderId")
        gridWOM.reorderColumn(12, Coll[3])


        gridWOM.showColumn("QtyDZ")
        gridWOM.reorderColumn(13, Coll[13])


        gridWOM.showColumn("TotalDozens")
        gridWOM.reorderColumn(14, Coll[14])


        gridWOM.showColumn("LbsStr")
        gridWOM.reorderColumn(15, Coll[15])



        gridWOM.showColumn("PFSInd")
        gridWOM.reorderColumn(16, Coll[18])


        gridWOM.showColumn("StartDate")
        gridWOM.reorderColumn(17, Coll[19])


        gridWOM.showColumn("CurrDueDate")
        gridWOM.reorderColumn(18, Coll[20])


        gridWOM.showColumn("Rule")
        gridWOM.reorderColumn(19, Coll[31])

        gridWOM.showColumn("TxtPath")
        gridWOM.reorderColumn(20, Coll[21])


        gridWOM.showColumn("CutPath")
        gridWOM.reorderColumn(21, Coll[22])


        gridWOM.showColumn("SewPath")
        gridWOM.reorderColumn(22, Coll[23])


        gridWOM.showColumn("Atr")
        gridWOM.reorderColumn(23, Coll[24])


        gridWOM.showColumn("MfgPathId")
        gridWOM.reorderColumn(24, Coll[25])


        gridWOM.showColumn("DcLoc")
        gridWOM.reorderColumn(25, Coll[26])

        gridWOM.showColumn("OrderStatusDesc")
        gridWOM.reorderColumn(26, Coll[5])



        gridWOM.showColumn("ExpeditePriority")
        gridWOM.reorderColumn(27, Coll[27])


        gridWOM.showColumn("CategoryCode")
        gridWOM.reorderColumn(28, Coll[28])



        gridWOM.showColumn("SpillOver")
        gridWOM.reorderColumn(29, Coll[36])


        gridWOM.showColumn("CreateBd")
        gridWOM.reorderColumn(30, Coll[33])


        gridWOM.showColumn("DozensOnly")
        gridWOM.reorderColumn(31, Coll[34])


        gridWOM.showColumn("BOMUpdate")
        gridWOM.reorderColumn(32, Coll[35])

        gridWOM.showColumn("Note") 
        gridWOM.reorderColumn(33, Coll[39])

        gridWOM.showColumn("DemandDate")
        gridWOM.reorderColumn(34, Coll[40])

        gridWOM.showColumn("DemandDriver")
        gridWOM.reorderColumn(35, Coll[29])
        
        gridWOM.showColumn("DemandSource")
        gridWOM.reorderColumn(36, Coll[30])

     
        gridWOM.showColumn("OrderType")
        gridWOM.reorderColumn(37, Coll[4])
       

        gridWOM.showColumn("Priority")
        gridWOM.reorderColumn(38, Coll[32])

        

        gridWOM.showColumn("PackCode")
        gridWOM.reorderColumn(39, Coll[37])

        gridWOM.showColumn("MakeOrBuy")
        gridWOM.reorderColumn(40, Coll[38])



       

    },   

    changeViews: function () {
        $('#grdWOM .k-grid-content').scrollLeft(0)
        if ($(this).val() == "Default") {
          
            WOM.setDefaultView()
            ISS.common.notify.info('Switched to Default View')
        }
        else if ($(this).val() == "Cylinder") {
           
            WOM.setCylinderView()
            ISS.common.notify.info('Switched to Cylinder View')
        }
        else if ($(this).val() == "MSKU") {
           
            WOM.setMSKUView();
            ISS.common.notify.info('Switched to  Multi-SKU View ')
        }
        

    },

    clearFilterGrid: function () {
        $('#frmFillgrid #SuggestedLots').prop('checked', true)
          $('#frmFillgrid #CustomerOrders').prop('checked',true)
          $('#frmFillgrid #SpillOver').prop('checked',true)
          $('#frmFillgrid #Events').prop('checked',true)
          $('#frmFillgrid #LockedLots').prop('checked',true)
          $('#frmFillgrid #MaxBuild').prop('checked',true)
          $('#frmFillgrid #ReleasedLotsThisWeek').prop('checked',true)
          $('#frmFillgrid #StockTarget').prop('checked',true)
          $('#frmFillgrid #TILs').prop('checked',true)
          $('#frmFillgrid #ReleasedLots').prop('checked',false)
          $('#frmFillgrid #ExcludeBuyOrders').prop('checked', false)
          $('#frmFillgrid #Forecast').prop('checked',false)
          $('#frmFillgrid #BodyOnly').prop('checked', false)
          $('#frmFillgrid #TrimOnly').prop('checked', false)
          $('#frmFillgrid #GroupOnly').prop('checked', false)
    },

    gotoCreateWO: function (e) {
        var dataItem = gridWOM.dataItem($(e.currentTarget).closest("tr"));

        var url = location.protocol + '//' + location.hostname + WOM.const.urlGetCreateWO;
        var queryString = $.param({
            Dc: dataItem.DcLoc, TxtPlant: dataItem.TxtPath, SuperOrder: dataItem.SuperOrder,
            autoLoad: true

        });

        if (queryString != null && queryString.length > 0) {
            url += "?" + queryString;
        }
        window.open(url, "_blank");
        return false
    },

    undoChanges: function (dataItem) {
        if (dataItem.Cloned) {
            cur = dataItem.Cloned;
            cur.uid = null;
            if (cur.IsDeleted) cur.IsDeleted = false;
            idx = gridWOM.dataSource.indexOf(dataItem);
            gridWOM.dataSource.remove(dataItem)
            gridWOM.dataSource.insert(idx, cur);
        }
    },
    undoMultiple: function () {
        var rows = WOM.selectedRows(true);
        if (rows.length > 0) {
            ISS.common.blockUI(true);
            var cur = null;
            var idx = 0;            
            for (i = 0; i < rows.length; i++) {
                var dataItem = gridWOM.dataItem(rows[i]);
                if (dataItem) {
                    WOM.undoChanges(dataItem);
                    if (dataItem.IsDeleted) dataItem.IsDeleted = false;
                    if (dataItem.GroupId != null) {
                        var garr = WOM.getAllMSKU(dataItem.GroupId, dataItem.SuperOrder);
                        for (j = 0; j < garr.length; j++) {
                            var item = garr[j];
                            WOM.undoChanges(item);
                            if (item.IsDeleted) item.IsDeleted = false;
                        }
                    }
                }                
            }
            gridWOM.refresh();
            WOM.loadWOMGrid(true);
            ISS.common.blockUI(false);
        }
        else {
            ISS.common.showPopUpMessage('Please select at least one record.');
        }

        return false;
    },

    clearWOMbtnClear: function () {
      
        


            $('#frmWOMSearch input:checkbox').uncheck();
            WOM.clearFilterGrid();
            ISS.common.clearGridFilters(gridWOM);
            gridWOM.dataSource.data([]);
            WOM.const.validator.hideMessages();
            $('#frmWOMSearch .k-textbox').val('');
          
            

            var dueDate = $('#DueDate').data("kendoDropDownList");         
            dueDate.value('Earliest Start');

           // $('#BOMMismatches').prop('checked',false);
     
            var plannr = $("#Planner").data("kendoMultiSelect");
            plannr.value('');
            var corpdiv = $("#CorpDiv").data("kendoMultiSelect");
            corpdiv.value('');

            var capacityGroup = $('#CapacityGroup').data("kendoMultiSelect");
            capacityGroup.value('');

            var workCenter = $("#WorkCenter").data("kendoMultiSelect");
             workCenter.value('');

             var bu = $("#BusinessUnit").data("kendoMultiSelect");
             bu.value('');

             var week = $('#Week').data('kendoComboBox');
             week.value('Current + Prior Week');
                 

             var moreWeek = $('#MoreWeeks').data('kendoComboBox');
             moreWeek.value('52');
        

              
             return false;
    },

    edit: function (e) {
        var rows = WOM.selectedRows();
        var RowData = null;
        $(rows).each(function (ind, data) {
            RowData = gridWOM.dataItem(data);
            //superOrder = RowData.SuperOrder;
            //mfgpath = RowData.MfgPathId;
            //cutpath = RowData.CuttPath;
        });
        if (!WOM.IsAllowEdit(RowData)) {
            var msg = "Unable to Edit this row";
            //alert("Unable to Edit the row. Status is" + select);
            ISS.common.notify.error(msg);
            this.closeCell();
        }
        var input = e.container.find("input");
        setTimeout(function () {
            input.select();
        }, 25);

    },
    TextPathVal: function (e) {
        if (!e.value) {
            return;
        }
        var rows = WOM.selectedRows();
        var RowData = null;
        $(rows).each(function (ind, data) {
            RowData = gridWOM.dataItem(data);
            gridWOM.table.find("tr[data-uid='" + RowData.uid + "'] td:eq(8)")[0].innerText = e.value.toUpperCase();
            gridWOM.table.find("tr[data-uid='" + RowData.uid + "'] td:eq(8)")[0].className = 'k-dirty-cell';
            $(gridWOM.table.find("tr[data-uid='" + RowData.uid + "'] td:eq(8)")[0]).append('<span class="k-dirty" style="margin-left: -19px;"></span>');
            RowData.TxtPath = e.value.toUpperCase();
            RowData.IsEdited = true;
            RowData.IsFieldChange = true;
        });
    },
    CutPathVal: function (e) {
        if (!e.value) {
            return;
        }
        var rows = WOM.selectedRows();
        var RowData = null;
        $(rows).each(function (ind, data) {
            RowData = gridWOM.dataItem(data);
            gridWOM.table.find("tr[data-uid='" + RowData.uid + "'] td:eq(9)")[0].innerText = e.value.toUpperCase();
            gridWOM.table.find("tr[data-uid='" + RowData.uid + "'] td:eq(9)")[0].className = 'k-dirty-cell';
            $(gridWOM.table.find("tr[data-uid='" + RowData.uid + "'] td:eq(9)")[0]).append('<span class="k-dirty" style="margin-left: -19px;"></span>');
            RowData.CuttPath = e.value.toUpperCase();
            RowData.CutPath = e.value.toUpperCase();
            RowData.IsEdited = true;
            RowData.IsFieldChange = true;
        });
    },
};
 

