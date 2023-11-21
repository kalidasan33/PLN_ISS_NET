
requisitions = {

    const: {
        flag: false,
        url: '',
        validator: null,
        CreatedBy:'',
        CreatedOn: '',
        comment: null,
        DeletedRows: new Array(),
        Src: null,
        Batch: { OrderCount: 50 , rows:null , TotCount:0, ProcCount:null, Error:0, Success:0},
        vendor: null,
        comment: null,
        popup: null,
        searchFromDate: null,
        searchToDate: null,
        ProgramSrc: { OS: 'ISS2165', AVYX: 'ISS2166' },
        validationNeeded: false
    },

    docRequisitionsReady: function () {
        requisitions.docRequisitionsReady2();
        requisitions.docRequisitionsReady3();
        

        ISS.common.toUpperCase('.clsupper');
        requisitions.const.validator = $("#frmRequisitions").kendoValidator().data("kendoValidator")
        //if ($("#PlannedDcDate").val() == '{0:01/01/0001}')
        $("#PlannedDcDate").val(serverDate);

        $('#PlannedDcDate').bind('focusout', requisitions.checkFutureDate);
        $('#frmRequisitions #VendorNo').val('');
        $('#frmRequisitions #OverPercentage').focusout(function (e) {
            if($('#frmRequisitions #OverPercentage').val() == "")
                $('#frmRequisitions #OverPercentage').val(0)
        });
        $('#frmRequisitions #UnderPercentage').focusout(function (e) {
            if ($('#frmRequisitions #UnderPercentage').val() == "")
                $('#frmRequisitions #UnderPercentage').val(0)
        });
        $('#frmRequisitions #RequisitionId').keydown(function (e) {
            if (requisitions.isValidFormMode()) {
                return false;
            }
        });
        $("#CreatedOn").val(requisitions.const.CreatedOn);
        $("#UpdatedOn").val(requisitions.const.CreatedOn);

        $('#VendorNo').keydown(function (e) {           
                return false;            
        });

        
    },
      
    GetNewRequisition: function () {c
        //$('#frmRequisitions #FormMode').val("Edit"); all data will be cleared msg
        requisitions.clearAllFieldsNewReq();
        requisitions.readonlyFieldsReq(false);        
        $('#ReqOrderRetieiveDetail').show();
        $('#frmRetOrder').show();
        ISS.common.executeActionAsynchronous("../Order/GetNewRequisition", null, function (stat, data) {
            if (stat) {
                $("#frmRequisitions #RequisitionId").val(data);
                $('#frmRequisitions #FormMode').val("Add");
                $('#btnSOSave').val('Save');
                $('#btnSOSave, #btnDelReq').show();
                $('#btnBulkComplete').show();
                $('#btnBulkActivate').show();
            }
            else
                ISS.common.showPopUpMessage('Failed to retrieve RequisitionId.');
        });
    },
    GeTestMsg: function () {
        requisitions.const.comment =  ISS.common.popUpCustom('.divDuplicate', settings);
    },

    readonlyFieldsReq: function (flag) {
        $("#BusinessUnit").data("kendoDropDownList").readonly(flag);
        $("#Season").data("kendoDropDownList").readonly(flag);
        $("#PlanningContact").data("kendoDropDownList").readonly(flag);
        $("#SourcingContact").data("kendoDropDownList").readonly(flag);
        $("#RequisitionApprover").data("kendoDropDownList").readonly(flag);
        $("#ProType").data("kendoDropDownList").readonly(flag);
        $("#TranspMode").data("kendoDropDownList").readonly(flag);
        //$("#PlannedDcDate").data("kendoDatePicker").readonly(flag);

        //$('#frmRequisitions #VendorNo').attr('readonly', true);
        //$('#frmRequisitions .lblVenSeach').attr('readonly', true);
        //$('#frmRequisitions #LwVendorLoc').attr('readonly', true);
        //$('#frmRequisitions #LwCompany').attr('readonly', true);
        //$('#frmRequisitions #VendorId').attr('readonly', true);
        //$('#frmRequisitions #VendorSuffix').attr('readonly', true);
        $('#frmRequisitions #DcLoc').attr('readonly', flag);

        $('#frmRequisitions #MFGPathId').attr('readonly', flag);

        //$('#frmRequisitions #ProdStatus').attr('readonly', true);
        //$('#frmRequisitions #ReqStatus').attr('readonly', true);
        //$('#frmRequisitions #CreatedBy').attr('disabled', true);
        //$('#frmRequisitions #UpdatedBy').attr('disabled', true);
        $("#frmRequisitions #OverPercentage").attr('readonly', flag);
        $("#frmRequisitions #UnderPercentage").attr('readonly', flag);
       ///  $("#frmRequisitions #grdRequisitionDetail").attr('readonly', flag);
        //$('input[type="vendorButton"]').attr('disabled', 'disabled');
        //$("#grdRequisitionDetail").kendoGrid({
        //    editable: false
        //});

        //var entityGrid = $("#grdRequisitionDetail").data("kendoGrid");
        //var data = entityGrid.dataSource.data();
        //var totalNumber = data.length;

        //for (var i = 0; i < totalNumber; i++) {
        //    var model = data[i];
        //    if (model)
        //        model.fields["cell"].editable = false;
        //}
        //var entityGrid = $("#grdRequisitionDetail").("kendoGrid");
    },
    
    clearAllFieldsNewReq: function () {
        ISS.common.notify.hide();
        $('#frmRequisitions #FormMode').val("");
        var busUnit = $("#BusinessUnit").data("kendoDropDownList");
        busUnit.select(0);
        var season = $("#Season").data("kendoDropDownList");
        season.select(function (dataItem) {
            return dataItem.SeasonName === "--Select--";
        });
        var planningContact = $("#PlanningContact").data("kendoDropDownList");
        planningContact.select(0);
        var sourcingContact = $("#SourcingContact").data("kendoDropDownList");
        sourcingContact.select(0);
        var reqApprover = $("#RequisitionApprover").data("kendoDropDownList");
        reqApprover.select(function (dataItem) {
            return dataItem.PlannerName === "--Select--";
        });
        var proType = $("#ProType").data("kendoDropDownList");
        proType.select(function (dataItem) {
            return dataItem.ProgramName === "--Select--";
        });
        var mode = $("#TranspMode").data("kendoDropDownList");
        mode.value('NS')
       
        $("#frmRequisitions #BulkNumber").attr('readonly', false);
        //var plannedDcDate = $("#PlannedDcDate").data("kendoDatePicker"); 
        //plannedDcDate.value('');
        $("#CreatedOn").val('');
        //$("#CreatedOn").val(requisitions.const.CreatedOn);     
        //$("#UpdatedOn").val(requisitions.const.CreatedOn);
        $('#frmRequisitions #PlannedDcDate').val('');
        $('#frmRequisitions #VendorNo').val('');
        $('#frmRequisitions #RequisitionId').val('');
        $('#frmRequisitions #BulkNumber').val('');
        $('#frmRequisitions .lblVenSeach').text('');
        $('#frmRequisitions #LwVendorLoc').val('0');
        $('#frmRequisitions #LwCompany').val('0');
        $('#frmRequisitions #VendorId').val('0');
        $('#frmRequisitions #VendorSuffix').val('0');
        $('#frmRequisitions #DcLoc').val('');
        $("#frmRequisitions #MFGPathId").val('');
        //$('#frmRequisitions #ProdStatus').val('L');
        //$('#frmRequisitions #ReqStatus').val('UC');


        //$('#frmRequisitions #CreatedBy').val(requisitions.const.CreatedBy);
        //$('#frmRequisitions #UpdatedBy').val(requisitions.const.CreatedBy);
        $("#frmRequisitions #OverPercentage").val(0);
        $("#frmRequisitions #UnderPercentage").val(0);
        $("#frmRequisitions #ProgramSource").val('');
        $("#frmRequisitions #ProgramSourceDesc").val('');
        //var OverPer = $("#frmRequisitions #OverPercentage").data("kendoNumericTextBox");
        //OverPer.value(0);
        //var UnderPer = $("#frmRequisitions #UnderPercentage").data("kendoNumericTextBox"); 
        //UnderPer.value(0);
        var gridDetail = $("#grdRequisitionDetail").data("kendoGrid");
        ISS.common.clearGridFilters(gridDetail);
        var dsData = gridDetail.dataSource.data([]);

        //requisitions.clearReqComments();
        //requisitions.clearReqExpandView();
        
        //$('#RequisitionComment_PlannerComment').val('')
        //$('#RequisitionComment_ApproverComment').val('')
        $('#frmRequisitions #ReqDetailTracking').uncheck(); 
        $('#frmRequisitions #DetailTrkgInd').uncheck();
        //$('#frmRequisitions #ShowSummaryOnly').val('');
        //$('#frmRequisitions #ShowSummaryOnly').uncheck()
        $('#btnRetOrderClear').click()
        requisitions.vendorClearName();        
        requisitions.vendorClearStyle();
        $('#btnRequisitionSearchClear').click();
        requisitions.const.validator.hideMessages();
        ISS.common.notify.hide();
        $('#btnSOSave, #btnDelReq').show();
        $('#btnBulkComplete').show();
        $('#btnBulkActivate').show();
       
    },
    DeleteRequisition: function () {
        if ($("#frmRequisitions #BulkNumber").val().trim() != '') {
            if ($('#frmRequisitions #FormMode').val() == "Edit") {
                var postData = { BulkNumber: $("#frmRequisitions #BulkNumber").val(), ProgramSource: $("#frmRequisitions #ProgramSource").val() };
                ISS.common.showConfirmMessage('Are you sure to delete this bulk order?', null, function (stat) {
                    if (stat) {
                        postData = JSON.stringify(postData);
                        ISS.common.executeActionAsynchronous("DeleteBulkOrder", postData, function (stat, data) {
                            if (stat) {
                                if (data.Status) {
                                    requisitions.clearAllFieldsNewReq();
                                    ISS.common.showPopUpMessage(data.ErrMsg, ISS.common.MsgType.Success);
                                }
                                else {
                                    ISS.common.showPopUpMessage(data.ErrMsg, ISS.common.MsgType.Error);
                                }
                            }
                            else {
                                ISS.common.showPopUpMessage('Failed to delete the bulk order.');
                            }
                        });
                    }
                });

            }
            else {
                ISS.common.showPopUpMessage("Cannot delete. Must clear first.");
                ISS.common.notify.error("Cannot delete. Must clear first.");
            }
        }
        else {
            ISS.common.showPopUpMessage("Nothing to delete.");
            ISS.common.notify.error("Nothing to delete.");
        }
        return false;
    },
    dataFunc: function () {
        var style = '5762';
        return {
            id: style // here we pass the site ID to server
        };
    },
    searchDataReleases: function () {
        //return ISS.common.getFormData($('#frmRequisitions'));
        //var dropDownVal = $("#BusinessUnit").val();
        //alert(dropDownVal)
        var Bunit = {
            BusinessUnit: $("#BusinessUnit").val()
        };
        return Bunit;
    },
    loadReleasesGrid: function () {
        var grid = $("#grdRequisitionOrder").data("kendoGrid");
        if (grid.pager.page() > 1) grid.pager.page(1)
        grid.dataSource.read();
        return false;
    },
    OnBusinessChange: function (e) {
        var dropdownlist = $("#Season").data("kendoDropDownList");
        dropdownlist.dataSource.read();

    },
    checkFutureDate: function () {
        //if ($("#PlannedDcDate").val() == '') return false;
        if (requisitions.isValidFormMode()) {
            var tt = new Date();
            tt.setFullYear(serverDate.getFullYear());
            tt.setMonth(serverDate.getMonth() + 1);
            tt.setDate(serverDate.getDate())
            tt.setDate(365)
            var EnteredDate = new Date(serverDate);
            if (EnteredDate == 'Invalid Date') {
                ISS.common.showPopUpMessage('Invalid date format.');
                return false;
            }
            else if (EnteredDate < serverDate) {
                ISS.common.showPopUpMessage('Please enter future date.');
                return false;
            }
            else if (EnteredDate >= tt) {
                ISS.common.showPopUpMessage('Invalid DC date.');
                return false;
            }
            else if ($("#frmRequisitions #DcLoc").val() != "") {
                var postData = { DcLoc: $("#frmRequisitions #DcLoc").val(), BusinessUnit: $("#frmRequisitions #BusinessUnit").val() };
                postData = JSON.stringify(postData);
                ISS.common.executeActionAsynchronous(requisitions.const.urlGetDcValidates, postData, function (stat, data) {
                    if (stat) {
                        if (data.length == 0) {
                            var grid = $("#grdRequisitionDetail").data("kendoGrid");
                            var dataDS = grid.dataSource.data();
                            if (grid.dataSource._data.length > 0) {
                                //for (var i = 0; i < dataDS.length; i++) {
                                //    if ($("#frmRequisitions #DcLoc").val() != data[0].DCLoc) {
                                //        ISS.common.showPopUpMessage('Invalid DC ! Try with another DC Loc');
                                //        //setTimeout(function () { $("#frmRequisitions #DcLoc").focus(); }, 10);
                                //        return false;
                                //    }

                                //}
                            }
                        }
                        else {
                            requisitions.validateRequisitionFull();
                        }

                    }

                });
            }
            //requisitions.validateRequisitionFull();
        }
    },
    businessDropdown: function () {

        var dropdownlist = $("#BusinessUnit").data("kendoDropDownList");
        var BusinessData = {
            BusinessUnit: $("#BusinessUnit").val()
        };
        return BusinessData;
    },

    onError: function (e, status) {
        // alert(e.errors);
       ISS.common.showPopUpMessage(e.errors);
    },
    sample: function (e) {
        document.addEventListener('focusout', function (e) {
            var TABKEY = 9;
            if (event.target.id == 'Style') {
                var grid = $("#grdRequisitionDetail").data("kendoGrid");
                if (grid) {
                    var data = grid.dataSource._data;
                    var fData = data;
                    var formattedData = JSON.stringify(data);
                    var postData = { Style: $("#frmRequisitions #Style").val(), BusinessCode: $("#frmRequisitions #BusinessUnit").val() };
                    postData = JSON.stringify(postData);
                    ISS.common.executeActionAsynchronous(requisitions.const.urlGetRetrieve, postData, function (stat, data) {
                        if (stat) {
                            if (data.Data.length > 0) {
                                for (var i = 0; i < data.Data.length; i++) {
                                    $("#frmRequisitions #Color").val(data.Data[i].Color);
                                    $("#frmRequisitions #Description").val(data.Data[i].StyleDesc);
                                }

                                //$.each(data.Data, function (i, item) {
                                //    $("#frmRequisitions #Color").val(data.Data[0].Color);
                                //    $("#frmRequisitions #Description").val(item.StyleDesc);
                                //});

                            }
                            else {
                                $("#frmRequisitions #Style").val('');
                            }
                        }
                        else {
                            ISS.common.showPopUpMessage('Failed to retrieve Style details.');

                        }
                    });


                }
                return false;
            };

        }, false);
    },
    sync_handler: function (e) {
        this.read();
    },

    onSave: function (e) {
        $("#grdRequisitionDetail").data("kendoGrid").refresh();
    },

    gridonclick: function (e) {
        var fieldName = e.container.find("input").attr("name");
        var grid = $("#grdRequisitionDetail").data("kendoGrid");
        var data = grid.dataSource._data;
        var fData = data;
        var formattedData = JSON.stringify(data);
        if (fieldName == 'Style') {
            if (grid) {
                var postData = { Style: $("#frmRequisitions #Style").val(), BusinessCode: $("#frmRequisitions #BusinessUnit").val() };
                style_cd = postData.Style;
                postData = JSON.stringify(postData);
                ISS.common.executeActionAsynchronous(requisitions.const.urlGetRetrieve, postData, function (stat, data) {
                    if (stat) {
                        var dsData = $("#grdRequisitionDetail").data("kendoGrid").dataSource.data();
                        //undefined
                        //if(requisitions.validateRequisitionDetailRow(dsData[0])

                        if (data.Data.length > 0 || dsData[0].Style != "") {
                            for (var i = 0; i < data.Data.length; i++) {
                                $("#frmRequisitions #Color").val(data.Data[i].Color);
                                $("#frmRequisitions #Description").val(data.Data[i].StyleDesc);
                            }

                            //$.each(data.Data, function (i, item) {
                            //    $("#frmRequisitions #Color").val(data.Data[0].Color);
                            //    $("#frmRequisitions #Description").val(item.StyleDesc);
                            //});

                        }
                        else {
                            $("#frmRequisitions #Style").val('');
                        }
                    }
                    else {
                        ISS.common.showPopUpMessage('Failed to retrieve Style details.');

                    }
                });
            }
            return false;
        } 
    },
};




