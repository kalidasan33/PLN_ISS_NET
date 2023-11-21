
temp = {

   



    docRequisitionsReady2: function () {
        $('#searchVendor').hide();
        $('#frmReqExpandView, #frmRequisitions').submit(function (e) {
            return false;
        });
        $('#btnVendorSearchByName, #btnVendorSearchByStyle').bind('click', requisitions.loadVendorSearchGrid);

        $('#btnBulkOrderSearch').bind('click', requisitions.loadBulkOdrSearchGrid);

        $('#btnReqComments').bind('click', requisitions.requisitionCommentsClick);

        $('#btnReqExpandView').bind('click', requisitions.requisitionExpandClick);

        $('#btnReqCommentsSave').bind('click', requisitions.saveReqComments);

        $('#btnVendorClearName').bind('click', requisitions.vendorClearName);

        $('#btnVendorClearStyle').bind('click', requisitions.vendorClearStyle);

        $('#btnRequisitionSearchClear').bind('click', requisitions.reqSearchClear);

        $('#frmReqExpandView #exportButton').bind('click', requisitions.exportRequisitionExpandView);

        $('#btnExportRequisitionDetails').bind('click', requisitions.exportRequisitionDetails);

        $('#btnRequisitionSearchExport').bind('click', requisitions.exportRequisitionSearch);

        $('#btnBulkComplete').bind('click', requisitions.BulkCompleteProcess); 

        $('#btnBulkActivate').bind('click', requisitions.BulkActivateProcess);

        $('#frmReqExpandView #RequisitionId').keydown(function (event) {
            if (event.keyCode == 13) {
                event.preventDefault();
                return false;
            }
        });
        $('#ReqOrderDetails').keydown(function (event) {
            if (event.keyCode == 13) {
                event.preventDefault();
                return false;
            }
        });
        $('#frmSearchVendor .InputF').keypress(function (e) {
            if (e.which == 13) {
                requisitions.loadVendorSearchGrid();
                return false;
            }
        });

        $('#frmRequisitions #BulkNumber').keypress(function (e) {
            if (e.which == 13) {
                var rId = $('#frmRequisitions #BulkNumber').val();
                if ($.trim(rId) != '') {

                    if (requisitions.isValidFormMode()) {
                        ISS.common.showConfirmMessage('Pending changes will be lost.<br/> Do you want to continue with reading and losing your changes?', null, function (reply) {
                            if (reply) {
                                var reqId = {
                                    reqId: rId
                                };
                                var pgmSource = null;
                                //requisitions.const.validator.validate($('#frmRequisitions #BulkNumber'))
                                if (pgmSource == null) {
                                    ISS.common.blockUI(true);
                                    ISS.common.executeActionAsynchronous("../BulkOrder/BulkOrderSearchEnter", JSON.stringify(reqId), function (stat, data) {
                                        if (data.length == 1) {
                                            if (stat && data) {
                                                pgmSource = data[0].ProgramSource;
                                                requisitions.const.validationNeeded = true;
                                                requisitions.fillRequisition(data[0].BulkNumber, pgmSource);
                                            }
                                        }
                                        else if (data.length > 1) {
                                            requisitions.const.popup = ISS.common.popUp('.divBulkOrderSearchPopup', 'List View Knights Apparel Bulk')
                                            var rId = $('#frmRequisitions #BulkNumber').val();
                                            $('#frmRequisitions #BulkNumber').val(data[0].BulkNumber)
                                            $('#frmBulkOrdSearch #lblReqDateError').text('')
                                            $('#frmBulkOrdSearch #FromDate').val('');
                                            $('#frmBulkOrdSearch #ToDate').val('');
                                            var grid = $("#grdBulkSearch").data("kendoGrid");
                                            grid.dataSource.data(data);
                                            return false;
                                        }
                                        else
                                            ISS.common.showPopUpMessage("Invalid bulk number");
                                        ISS.common.blockUI(false);

                                    }, 'POST');
                                }

                                
                            }
                        })
                    }
                    else {
                        //requisitions.fillRequisition(reqId);

                        var reqId = {
                            reqId: rId
                        };
                        var pgmSource = null;
                        //requisitions.const.validator.validate($('#frmRequisitions #BulkNumber'))
                        if (pgmSource == null) {
                            ISS.common.blockUI(true);
                            ISS.common.executeActionAsynchronous("../BulkOrder/BulkOrderSearchEnter", JSON.stringify(reqId), function (stat, data) {
                                if (data.length == 1) {
                                    if (stat && data) {
                                        pgmSource = data[0].ProgramSource;
                                        requisitions.const.validationNeeded = true;
                                        requisitions.fillRequisition(data[0].BulkNumber, pgmSource);
                                    }
                                }
                                else if (data.length > 1) {
                                        requisitions.const.popup = ISS.common.popUp('.divBulkOrderSearchPopup', 'List View Knights Apparel Bulk')
                                        var rId = $('#frmRequisitions #BulkNumber').val();
                                        $('#frmBulkOrdSearch #BulkNumber').val(data[0].BulkNumber)
                                        $('#frmBulkOrdSearch #lblReqDateError').text('')
                                        $('#frmBulkOrdSearch #FromDate').val('');
                                        $('#frmBulkOrdSearch #ToDate').val('');
                                        var grid = $("#grdBulkSearch").data("kendoGrid");
                                        grid.dataSource.data(data);
                                        ISS.common.blockUI(false);
                                        return false;
                                }
                                else
                                    ISS.common.showPopUpMessage("Invalid bulk number");
                                ISS.common.blockUI(false);

                            }, 'POST');
                        }
                    }
                }
                return false;
            }
        }).focusout(function () {
            setTimeout(function () { requisitions.const.validator.hideMessages() }, 0);
        });



        $('#frmSearchVendor :radio').click(function () {
            var selected = $(this).val();
            requisitions.vendorSearchCriteria(selected);
        });

        $('.lblVenSeach').text('');
        ISS.common.toUpperCase('.InputF:not(.excludeF)');

        $("#btnResettoConstruction").kendoButton().css("visibility", "hidden");
        $("#btnReleasetoSourcing").kendoButton().css("visibility", "hidden");
    },

    fillRequisition: function (id, pgmSource) {
        var bulkOrdNo = {
            bulkOrdNo: id,
            programSource: pgmSource
        };
        //requisitions.const.validator.validate($('#frmRequisitions #BulkNumber'))
        ISS.common.blockUI(true);
        ISS.common.executeActionAsynchronous("../BulkOrder/GetBulkOrderDetail", JSON.stringify(bulkOrdNo), function (stat, data) {
            if (stat && data) {
                if (data.length > 0)
                    requisitions.getHeaderData(0, data);
            }             
            ISS.common.blockUI(false);
             
        },'POST');
        //$("#grdRequisitionDetail").data("kendoGrid").dataSource.read();
        
       
        return false;
    },

    toggleProgramSource: function (pgmSource) {
        if (pgmSource == requisitions.const.ProgramSrc.OS) { //OneSource
            var grid = $('#grdRequisitionDetail').data("kendoGrid");
                grid.hideColumn("APSStyle");
                grid.hideColumn("APSColor");
                grid.hideColumn("APSAttribute");
                grid.hideColumn("APSSizeLit");
                grid.hideColumn("DmdWkEndDate");
                //grid.hideColumn("BusinessUnit");
                grid.hideColumn("DemandSource");
                grid.hideColumn("PrioritySeq");
                grid.hideColumn("UserId");
                
                grid.showColumn(0);
                grid.showColumn("LineNumber");
                grid.showColumn("Rev");
                grid.showColumn("CurrDueDate");
                grid.showColumn(grid.columns.length-1)

                $('.k-grid-btnDuplicate').show();
                $('.k-grid-add').show();
                $('.k-grid-btndeletemulti').show();

                $("#PlanningContact").closest(".k-widget").show();
                $("label[for='PlanningContact']").show();

                $("#Season").closest(".k-widget").show();
                $("label[for='Season']").show();

                $("#SourcingContact").closest(".k-widget").show();
                $("label[for='SourcingContact']").show();

                $("#MFGPathId").show();
                $("label[for='MFGPathId']").show();

                $("label[for='VendorDesc']").show();

                $("#TranspMode").closest(".k-widget").show();
                $("label[for='TranspMode']").show();

                $("#RequisitionApprover").closest(".k-widget").show();
                $("label[for='RequisitionApprover']").show();

                $("#ProType").closest(".k-widget").show();
                $("label[for='ProType']").show();

                $("#OverPercentage").show();
                $("label[for='OverPercentage']").show();

                $("#UnderPercentage").show();
                $("label[for='UnderPercentage']").show();

                $("#DetailTrkgInd").show();
                $("label[for='DetailTrkgInd']").show();

                $('.trhideClass').show();
                grid.refresh();
            
        }
        else { //AVYX
            var grid = $('#grdRequisitionDetail').data("kendoGrid");

            grid.hideColumn(0);
            grid.hideColumn(grid.columns.length - 1)

            grid.hideColumn("LineNumber");
            grid.hideColumn("Rev");
            grid.hideColumn("CurrDueDate");

            grid.showColumn("APSStyle");
            grid.showColumn("APSColor");
            grid.showColumn("APSAttribute");
            grid.showColumn("APSSizeLit");
            grid.showColumn("DmdWkEndDate");
            //grid.showColumn("BusinessUnit");
            grid.showColumn("DemandSource");
            grid.showColumn("PrioritySeq");
            grid.showColumn("UserId");
           
            $('.k-grid-btnDuplicate').hide();
            $('.k-grid-add').hide();
            $('.k-grid-btndeletemulti').hide();

            $("#PlanningContact").closest(".k-widget").hide();
            $("label[for='PlanningContact']").hide();

            $("#Season").closest(".k-widget").hide();
            $("label[for='Season']").hide();

            $("#SourcingContact").closest(".k-widget").hide();
            $("label[for='SourcingContact']").hide();

            $("#MFGPathId").hide();
            $("label[for='MFGPathId']").hide();

            $("label[for='VendorDesc']").hide();

            $("#TranspMode").closest(".k-widget").hide();
            $("label[for='TranspMode']").hide();

            $("#RequisitionApprover").closest(".k-widget").hide();
            $("label[for='RequisitionApprover']").hide();

            $("#ProType").closest(".k-widget").hide();
            $("label[for='ProType']").hide();

            $("#OverPercentage").hide();
            $("label[for='OverPercentage']").hide();

            $("#UnderPercentage").hide();
            $("label[for='UnderPercentage']").hide();

            $("#DetailTrkgInd").hide();
            $("label[for='DetailTrkgInd']").hide();

            $('.trhideClass').hide();

            grid.refresh();
        }
    },

    getHeaderData: function (index, data, needValidation) {
        if (index < data.length)
            var header = data[index];

        var header = data[index];
        var pgmSource = header.ProgramSource;
        requisitions.toggleProgramSource(pgmSource);
        requisitions.loadHeader(header); 
     
        var AllReleased = true;
        var AnyReleased = false; // header read only in this case
        var isComplete = true;
        var isActivate = true;

        for (var i = 0; i < data.length; i++) {
            if (data[i].CurrDueDate) data[i].CurrDueDate = ISS.common.parseDate(data[i].CurrDueDate.toString());
           // if (data[i].DemandWeekEndDate) data[i].DemandWeekEndDate = ISS.common.parseDate(data[i].DemandWeekEndDate.toString()).toDateString();
            if (data[i].ProcessedToOS == "E" || data[i].ProcessedToOS == "N" || data[i].ProcessedToOS == "I") {
                AllReleased = false;
            }
            if (data[i].ProcessedToOS == "Y"){
                AnyReleased = true;
            }
            if (data[i].ProcessedToOS != "A") {
                isComplete = false;
            }
            if (data[i].ProcessedToOS != "C") {
                isActivate = false;
            }
        } 

        var grid = $('#grdRequisitionDetail').data("kendoGrid");
        grid.dataSource.data(data);

        if (pgmSource == requisitions.const.ProgramSrc.OS) {
            //$('#btnDelReq, #btnBulkComplete, #btnSOSave,').hide();
            $('#btnSOSave, #btnDelReq').hide();
            $('#btnBulkComplete').hide();
            $('#btnBulkActivate').hide();
            if (AllReleased && data.length != 0) {
                //$('#btnSOSave, #btnDelReq').hide();
                $('#frmRequisitions #FormMode').val("Read");
                requisitions.readonlyFieldsReq(true);
                ISS.common.notify.info('This is a read only bulk order.');
            }
            else if (AnyReleased) {
                //$('#btnSOSave, #btnDelReq').hide();
                $('#frmRequisitions #FormMode').val("Read");
                requisitions.readonlyFieldsReq(true);
                ISS.common.notify.info('Some line items already released. Bulk header will not be editable.')
            }
            else {
                $('#btnSOSave, #btnDelReq').show();
                requisitions.readonlyFieldsReq(false);
            }
           
        }
        else if (pgmSource == requisitions.const.ProgramSrc.AVYX) {
            requisitions.readonlyFieldsReq(true);
            //$('#btnDelReq, #btnBulkComplete, #btnSOSave,').hide();
            $('#btnSOSave, #btnDelReq').hide();
            $('#btnBulkComplete').hide();
            $('#btnBulkActivate').hide();
            if (AllReleased && data.length != 0 && !isComplete && !isActivate) {
                $('#frmRequisitions #FormMode').val("Read");
                ISS.common.notify.info('This is a read only bulk order.')
            }
            else if (AnyReleased) {
                $('#frmRequisitions #FormMode').val("Read");
                ISS.common.notify.info('Some line items already released. Bulk header will not be editable.')
            }
            else if (isComplete) {
                $('.k-grid-btnDuplicate').hide();
                $('.k-grid-add').hide();
                $('.k-grid-btndeletemulti').hide();
                $('#btnBulkComplete').show();
                $('#frmRequisitions #FormMode').val("Read");
            }
            else if (isActivate) {
                $('.k-grid-btnDuplicate').hide();
                $('.k-grid-add').hide();
                $('.k-grid-btndeletemulti').hide();
                $('#btnBulkActivate, #btnDelReq').show();
                requisitions.readonlyFieldsReq(false);
                //$('#frmRequisitions #FormMode').val("Read");
            }
            else {
                $('#btnSOSave, #btnDelReq').show();
                requisitions.readonlyFieldsReq(false);
                if (requisitions.const.validationNeeded)
                    requisitions.verifyBulkOrder();
            }

        }

        requisitions.const.validationNeeded = false;

        return false;
    },


    loadHeader: function (data) {
        requisitions.clearAllFieldsNewReq();
        if (data) {
            
            $('#frmRequisitions #BulkNumber').val(data.BulkNumber);
            var busUnit = $("#BusinessUnit").data("kendoDropDownList");
            busUnit.value(data.BusinessUnit);


            var season = $("#Season").data("kendoDropDownList");
            season.dataSource.read();
            season.value($.trim(data.Season));
            var planningContact = $("#PlanningContact").data("kendoDropDownList");
            planningContact.value(data.PlanningContact);
            var sourcingContact = $("#SourcingContact").data("kendoDropDownList");
            sourcingContact.value(data.SourcingContact);
            var reqApprover = $("#RequisitionApprover").data("kendoDropDownList");
            reqApprover.value(data.RequisitionApprover);
            var proType = $("#ProType").data("kendoDropDownList");
            proType.value(data.ProType);
            var mode = $("#TranspMode").data("kendoDropDownList");
            mode.value(data.TranspMode);

            var businessUnit = $("#BusinessUnit").data("kendoDropDownList");
            businessUnit.value(data.BusinessUnit);

            //var plannedDcDate = $("#PlannedDcDate").data("kendoDatePicker");
            //plannedDcDate.value(data.PlannedDcDate);

            var CreDate = ISS.common.parseDate(data.CreatedOn);
            var UpdateDate = ISS.common.parseDate(data.UpdatedOn);
            if (CreDate != null && CreDate != undefined)
                $('#frmRequisitions #CreatedOn').val(requisitions.formatDate(CreDate));
            if (UpdateDate != null && UpdateDate != undefined)
            $('#frmRequisitions #UpdatedOn').val(requisitions.formatDate(UpdateDate));

            $('#frmRequisitions #VendorNo').val((data.VendorNo == '0') ? '' : data.VendorNo);
            var vendorDesc = ((data.VendorName == null) ? '' : data.VendorName + ", ") + ((data.LwVendorLoc == null) ? '' : data.LwVendorLoc + ", ") + ((data.VendorId == null || data.VendorId == '0') ? '' : data.VendorId + ", ") + ((data.BusinessUnit == null || data.BusinessUnit == '') ? '' : data.BusinessUnit) + " &nbsp; &nbsp;&nbsp; &nbsp; - " + ((data.MFGPathId == null) ? '' : data.MFGPathId);
            $('#frmRequisitions .lblVenSeach').html(vendorDesc);
            $('#frmRequisitions #LwVendorLoc').val(data.LwVendorLoc);
            $('#frmRequisitions #LwCompany').val(data.LwCompany);
            $('#frmRequisitions #VendorId').val(data.VendorId);
            $('#frmRequisitions #VendorSuffix').val(data.VendorSuffix);
            $('#frmRequisitions #DcLoc').val(data.DcLoc);

            $('#frmRequisitions #ProdStatus').val(data.ProdStatus);
            $('#frmRequisitions #ReqStatus').val(data.ReqStatus);

            $('#frmRequisitions #OverPercentage').val(data.OverPercentage);
            $('#frmRequisitions #UnderPercentage').val(data.UnderPercentage);
            //$("#frmRequisitions #OverPercentage").data("kendoNumericTextBox").value(data.OverPercentage);
            //$("#frmRequisitions #UnderPercentage").data("kendoNumericTextBox").value(data.UnderPercentage);BulkNumber
            $('#frmRequisitions #CreatedBy').val(data.CreatedBy);
            $('#frmRequisitions #UpdatedBy').val(data.UpdatedBy);
            $('#frmRequisitions #MFGPathId').val(data.MFGPathId);
            $('#frmRequisitions #RequisitionId').val(data.RequisitionId);
            if (data.ReqDetailTracking)
                $('#frmRequisitions #ReqDetailTracking').prop('checked', true);
            else
                $('#frmRequisitions #ReqDetailTracking').prop('checked', false);
            if (data.DetailTrkgInd)
                $('#frmRequisitions #DetailTrkgInd').prop('checked', true);
            else
                $('#frmRequisitions #DetailTrkgInd').prop('checked', false);
            $('#frmRequisitions #FormMode').val("Edit");

            $("#frmRequisitions #BulkNumber").attr('readonly', true);

            $('#frmRequisitions #ProgramSourceDesc').val(data.ProgramSourceDesc);
            $('#frmRequisitions #ProgramSource').val(data.ProgramSource);

            //if (data.ProcessedToOS == 'E') {
            //    $('#btnSOSave, #btnDelReq').hide();
            //}
            //else {
            //    $('#btnSOSave, #btnDelReq').show();
            //}

            //if (!(data.ProdStatus == 'L' && data.ReqStatus == 'UC')) {
                //requisitions.readonlyFieldsReq(true);
            //    $('#frmRequisitions #vendorButton').attr('disabled', "disabled");
            //    $('#ReqOrderRetieiveDetail').hide();
            //    $('#frmRetOrder').hide();
            //    $('#btnSOSave, #btnDelReq').hide();
            //    $('#frmRequisitions #FormMode').val("Read");
            //    ISS.common.showPopUpMessage('This is a read only requisition for ' + data.BusinessUnit, ISS.common.MsgType.Info)
            //}
            //else {
            //    requisitions.readonlyFieldsReq(false);
            //    $('#frmRequisitions #vendorButton').attr('enabled', "enabled");
            //    $('#ReqOrderRetieiveDetail').show();
            //    $('#frmRetOrder').show();
            //    $('#btnSOSave').val('Update');
            //    $('#btnSOSave, #btnDelReq').show();
            //}


            //$('#frmRequisitions #ShowSummaryOnly').uncheck();

            
        }
        else {
            ISS.common.showPopUpMessage('Bulk Order detail not found.');
        }
        requisitions.const.validator.hideMessages()

        return false;
    },

    dataBound: function (e) {
        var grid = $("#grdBulkSearch").data("kendoGrid");
        var gridData = grid.dataSource.view();
       
        var datasrc = grid.dataSource.data();
        var len = datasrc.length;

        for (var i = 0; i < gridData.length; i++) {
            if (gridData[i].ProcessedToOS == "E") {
                grid.table.find("tr[data-uid='" + gridData[i].uid + "']").addClass("highlighted-row");
                //$(".k-grid-content-locked").find("tr[data-uid='" + gridData[i].uid + "']").addClass("highlighted-row");
            }

            
        }
       
    },

    formatDate: function (date) {
        function pad(s) { return (s < 10) ? '0' + s : s; }
        var output = [pad(date.getMonth() + 1), pad(date.getDate()), date.getFullYear()].join('/');
        return output;
    },

    /*
    suggWODataBound: function (e) {
        e.sender.value('32');
    },*/
    vendorSearchClick: function () {
        requisitions.const.vendor = ISS.common.popUp('.divVendorSearchPopup', 'Vendor Search', null, function () {
            if ($('#frmSearchVendor :radio:checked').val() == "byStyleAndColor") {
                $('#ByStyle').focus()
            }
            else {
                $('#ByName').focus()
            }
        });
    },
    
    vendorSearchCriteria: function (selectedValue) {
        $('#frmSearchVendor #ByName').val('');
        $('#frmSearchVendor #ByStyle').val('');
        $('#frmSearchVendor #ByColor').val('');

        if (selectedValue === 'byVendor') {
            $('#frmSearchVendor #searchStyleColor').hide();
            $('#frmSearchVendor #searchVendor').show();
            var grid = $("#grdVendorByName").data("kendoGrid");
            if (grid.dataSource._filter != null && grid.dataSource._filter.filters.length > 0)
                grid.dataSource.filter([]);
            grid.dataSource.data([]);
            grid.refresh();
        }
        else {
            $('#frmSearchVendor #searchStyleColor').show();
            $('#frmSearchVendor #searchVendor').hide();
            var grid = $("#grdVendorByStyle").data("kendoGrid");
            if (grid.dataSource._filter != null && grid.dataSource._filter.filters.length > 0)
                grid.dataSource.filter([]);
            grid.dataSource.data([]);
            grid.refresh();
        }

    },
    
    searchVendorDetails: function () {
        // if (! requisitions.IsValidVendorSearch()) throw 'Invalid search'
        var vendorSearch = {
            ByName: $('#frmSearchVendor #ByName').val(),
            ByStyle: $('#frmSearchVendor #ByStyle').val(),
            ByColor: $('#frmSearchVendor #ByColor').val()
        };
        return vendorSearch;
    },

    vendorClearName: function () {
        $('#frmSearchVendor #ByName').val('');
        var grid = $("#grdVendorByName").data("kendoGrid");
        if (grid.dataSource._filter != null && grid.dataSource._filter.filters.length > 0)
            grid.dataSource.filter([]);
        grid.dataSource.data([]);
        grid.refresh();
        return false;
    },

    vendorClearStyle: function () {
        $('#frmSearchVendor #ByStyle').val('');
        $('#frmSearchVendor #ByColor').val('');
        var grid = $("#grdVendorByStyle").data("kendoGrid");
        if (grid.dataSource._filter != null && grid.dataSource._filter.filters.length > 0)
            grid.dataSource.filter([]);
        grid.dataSource.data([]);
        grid.refresh();
        return false;

    },

    

    loadVendorSearchGrid: function () {
        var selectedValue = $('input:radio[name=vendorSearch]:checked').val();
        if (selectedValue === 'byVendor') {
            if ($('#frmSearchVendor #ByName').val() == '') {
                ISS.common.showPopUpMessage('Please enter vendor name.');
                return false;
            }

            var grid = $("#grdVendorByName").data("kendoGrid");
            if (grid.pager.page() > 1) grid.pager.page(1)
            grid.dataSource.read();
        }
        else if (selectedValue === 'byStyleAndColor') {
            if ($('#frmSearchVendor #ByStyle').val() == '' && $('#frmSearchVendor #ByColor').val() == '') {
                ISS.common.showPopUpMessage('Please enter either style or color.');
                return false;
            }

            var grid = $("#grdVendorByStyle").data("kendoGrid");
            if (grid.pager.page() > 1) grid.pager.page(1)
            grid.dataSource.read();
        }


        return false;

    },

    IsValidVendorSearch: function () {
        var selectedValue = $('input:radio[name=vendorSearch]:checked').val();
        if (selectedValue === 'byVendor') {
            if ($('#frmSearchVendor #ByName').val() == '') {
                return false;
            }
        }
        else if (selectedValue === 'byStyleAndColor') {
            if ($('#frmSearchVendor #ByStyle').val() == '' && $('#frmSearchVendor #ByColor').val() == '') {
                return false;
            }

        }
        return true;
    },

    rowDataBoundVN: function (e) {
        $(".k-grid-Select").find("span").addClass("k-icon k-i-tick");
        $('#FilteredSearchColumnsVN').html(ISS.common.getFilteredColumns("#" + e.sender.content.context.id));
    },


    rowDataBoundV: function (e) {
        $(".k-grid-Select").find("span").addClass("k-icon k-i-tick");
        $('#FilteredSearchColumnsV').html(ISS.common.getFilteredColumns("#" + e.sender.content.context.id));
    },

    rowDataBound: function (e) {
        $(".k-grid-Select").find("span").addClass("k-icon k-i-tick");
        $('#FilteredSearchColumns').html(ISS.common.getFilteredColumns("#" + e.sender.content.context.id));
    },

    showDetails: function (e) {
        //e.preventDefault();

        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        $('#frmRequisitions #VendorNo').val(dataItem.LwVendorNo);

        var vendorDesc = dataItem.VendorName + ", " + dataItem.LwVendorLoc + ", " + dataItem.VendorId + ", " + dataItem.BusUnit + " &nbsp; &nbsp;&nbsp; &nbsp; - " + dataItem["SrcPlant"];
        $('#frmRequisitions .lblVenSeach').html(vendorDesc);
        $('#frmRequisitions #LwVendorLoc').val(dataItem.LwVendorLoc);
        $('#frmRequisitions #LwCompany').val(dataItem.LwCompany);
        $('#frmRequisitions #VendorId').val(dataItem.VendorId);
        $('#frmRequisitions #VendorSuffix').val(dataItem.VendorSuffix);
        var busUnit = $("#BusinessUnit").data("kendoDropDownList");
        busUnit.value(dataItem.BusUnit);
        $('#frmRequisitions #MFGPathId').val(dataItem["SrcPlant"]);

        requisitions.const.validator.validate($('#frmRequisitions #VendorNo'))
        requisitions.const.vendor.close();
        requisitions.const.validator.hideMessages()
        requisitions.validateRequisitionFull();
    },

    verifyBulkOrder: function () {
        var grid = $("#grdRequisitionDetail").data("kendoGrid");
        var lstBulkOrders = grid.dataSource.data();
        //requisitions.const.validator.validate($('#frmRequisitions #BulkNumber'))
        ISS.common.blockUI(true);
        ISS.common.executeActionAsynchronous("../BulkOrder/ValidateBulkOrder", JSON.stringify(lstBulkOrders), function (stat, data) {
            if (stat && data) {
                var grid = $("#grdRequisitionDetail").data("kendoGrid");
                grid.dataSource.data(data);

                var err = false;
                for (var i = 0; i < data.length; i++) {
                    if (data[i].ErrorStatus) {
                        err = true;
                        break;
                    }
                }

                if (err)
                    ISS.common.notify.error("Bulk Order verified and has some issues.");
                else
                    ISS.common.notify.success("Bulk Order verified successfully.");
                    
            }
            ISS.common.blockUI(false);

        }, 'POST');
        //$("#grdRequisitionDetail").data("kendoGrid").dataSource.read();


        return false;
    },

   
    
    //-------------------------------Requisition Search----------------------------------------

    bulkOrderSearchClick: function () {
        requisitions.const.popup = ISS.common.popUp('.divBulkOrderSearchPopup', 'List View Knights Apparel Bulk')

        return false;
    },


    loadBulkOdrSearchGrid: function () {
        $('#frmBulkOrdSearch #lblReqDateError').text('')
        var fromDate = $('#frmBulkOrdSearch #FromDate').val();
        var toDate = $('#frmBulkOrdSearch #ToDate').val();
        if (fromDate != "") {
            if (!requisitions.isDate(fromDate)) {
                $('#frmBulkOrdSearch #lblReqDateError').text('Invalid From Date.');
                return false;
            }
        }
        if (toDate != "") {
            if (!requisitions.isDate(toDate)) {
                $('#frmBulkOrdSearch #lblReqDateError').text('Invalid To Date.');
                return false;
            }
        }

        if (fromDate != "" && toDate != "") {
            if (Date.parse(fromDate) > Date.parse(toDate)) {
                $('#frmBulkOrdSearch #lblReqDateError').text('From date cannot be greater than To date');
                return false;
            }
        }

        var grid = $("#grdBulkSearch").data("kendoGrid");
        if (grid.pager.page() > 1) grid.pager.page(1)
        grid.dataSource.read();
        return false;
    },

    searchDataRequisition: function () {
        var bulkSearch = {
            BulkNumber: $('#frmBulkOrdSearch #BulkNumber').val(),
            FromDate: $('#frmBulkOrdSearch #FromDate').val(),
            ToDate: $('#frmBulkOrdSearch #ToDate').val(),
            ExcludeProcessed: $('#frmBulkOrdSearch #ExcludeProcessed').prop('checked')
        };
        return bulkSearch;
    },

    reqSearchClear: function () {
        var grid = $("#grdBulkSearch").data("kendoGrid");
        if (grid.dataSource._filter != null && grid.dataSource._filter.filters.length > 0)
            grid.dataSource.filter([]);
        grid.dataSource.data([]);
        grid.refresh();
        $('#frmBulkOrdSearch #BulkNumber').val('');
        $('#frmBulkOrdSearch #FromDate').val(requisitions.const.searchFromDate);
        $('#frmBulkOrdSearch #ToDate').val(requisitions.const.searchToDate);

        return false;
    },

    isDate: function (valDate) {
        var currVal = valDate;

        //if(currVal == '')
        //    return false;

        //Declare Regex 
        var rxDatePattern = /^(\d{1,2})(\/|-)(\d{1,2})(\/|-)(\d{4})$/;
        var dtArray = currVal.match(rxDatePattern); // is format OK?

        if (dtArray == null)
            return false;

        //Checks for mm/dd/yyyy format.
        dtMonth = dtArray[1];
        dtDay = dtArray[3];
        dtYear = dtArray[5];


        if (dtMonth < 1 || dtMonth > 12)
            return false;
        else if (dtDay < 1 || dtDay > 31)
            return false;
        else if ((dtMonth == 4 || dtMonth == 6 || dtMonth == 9 || dtMonth == 11) && dtDay == 31)
            return false;
        else if (dtMonth == 2) {
            var isleap = (dtYear % 4 == 0 && (dtYear % 100 != 0 || dtYear % 400 == 0));
            if (dtDay > 29 || (dtDay == 29 && !isleap))
                return false;
        }
        return true;
    },


    selectRequisition: function (e) {

        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        $('#frmRequisitions #BulkNumber').val(dataItem.BulkNumber);
        requisitions.const.popup.close();
        requisitions.const.validationNeeded = true;
        requisitions.fillRequisition(dataItem.BulkNumber, dataItem.ProgramSource);
        return false;
    },

    
    readDetailData: function () {
        return { BulkNumber: $('#BulkNumber').val() };
    },

    loadRequisitionHeader: function (data) {
        

    },
    formatDate: function (date) {
        function pad(s) { return (s < 10) ? '0' + s : s; }
        var output = [pad(date.getMonth() + 1), pad(date.getDate()), date.getFullYear()].join('/');
        return output;
    },

    


    //-------------------------------Requisition Comments----------------------------------------
    requisitionCommentsClick: function () {
        var requisition = $('#frmRequisitions #RequisitionId').val();
        if (requisition != "") {
            requisitions.clearReqComments();
            requisitions.const.comment = ISS.common.popUp('.divRequisitionCommentsPopup', 'Requisition Comments');
            if ($('#frmRequisitions #FormMode').val() == "Add") {
                
                $('#frmReqComments #RequisitionId').val(requisition);
                $('#frmReqComments #lblUpdatedBy').text(requisitions.const.CreatedOn);
                $('#frmReqComments #lblUpdatedByAppr').text(requisitions.const.CreatedOn);
                $('#frmReqComments #lblRequisitionApproverId').text($("#RequisitionApprover").data("kendoDropDownList").value());
                $('#frmReqComments #lblCreatedBy').text(requisitions.const.CreatedBy);
                $('#frmReqComments #txtPlannerComment').val($('#RequisitionComment_PlannerComment').val());
                $('#frmReqComments #txtApproverComment').val($('#RequisitionComment_ApproverComment').val());
            } else {
                requisitions.searchReqComments();
            }
            
        }
        return false;
    },

    loadReqCommentsHeader: function (data) {
        var updateOn = new Date(parseInt(data.UpdatedOn.substr(6)));
        $('#frmReqComments #RequisitionId').val(data.RequisitionId);
        $('#frmReqComments #lblRequisitionVersion').text(data.RequisitionVersion);
        $('#frmReqComments #lblPlanningContact').text(requisitions.FormatValue(data.PlannerName));
        $('#frmReqComments #lblRequisitionApprover').text(requisitions.FormatValue(data.RequisitionApprover));
        $('#frmReqComments #lblCreatedBy').text(requisitions.FormatValue(data.CreatedBy));
        $('#frmReqComments #lblRequisitionApproverId').text(requisitions.FormatValue(data.RequisitionApproverId));
        $('#frmReqComments #lblUpdatedBy').text(requisitions.FormatValue(requisitions.format(updateOn, 'MM/dd/yyyy')));
        $('#frmReqComments #lblUpdatedByAppr').text(requisitions.FormatValue(requisitions.format(updateOn, 'MM/dd/yyyy')));
        $('#frmReqComments #CommentReqVersionNo').val(data.RequisitionVersion);
        $('#frmReqComments #txtPlannerComment').val(data.RequisitionComment.PlannerComment);
        $('#frmReqComments #txtApproverComment').val(data.RequisitionComment.ApproverComment);
        //requisitions.getRequisitionComments();
    },

    searchReqComments: function (IsExp) {
        ISS.common.blockUI(true);
        var requisition = $('#frmRequisitions #RequisitionId').val();
        if (IsExp) {
            requisition = $('#frmReqExpandView #RequisitionId').val();
        }


        var requisitionId = {
            RequisitionId: requisition
        };

  requisitions.clearReqComments();
        $('#frmReqComments #RequisitionId').val(requisition);
        ISS.common.executeActionAsynchronous("../Order/RequisitionCommentGet", JSON.stringify(requisitionId), function (stat, data) {
            if (stat && data) {
                requisitions.loadReqCommentsHeader(data);
                ISS.common.blockUI(false);
            }
            else {
                ISS.common.blockUI(false);
            }

        });
    },

    clearReqComments: function () {
        $('#frmReqComments .reqCommentClear').text('');
        $('#frmReqComments #txtPlannerComment').val('');
        $('#frmReqComments #txtApproverComment').val('');
        $('#frmReqComments #RequisitionId').val('');
    },

    saveReqComments: function () {
        //var requisition = returnRequisitionComments;

        if ($('#frmRequisitions #FormMode').val() == "Add") {
            $('#RequisitionComment_PlannerComment').val($('#frmReqComments #txtPlannerComment').val());
            $('#RequisitionComment_ApproverComment').val($('#frmReqComments #txtApproverComment').val());
            requisitions.const.comment.close();
        }
        else {
            var plannerComment = null;
            var approverComment = null;
           // if ($("#frmReqComments #txtPlannerComment").val() != '') {
                 plannerComment = $("#frmReqComments #txtPlannerComment").val().split("\n");
            //}
           // if ($("#frmReqComments #txtApproverComment").val() != '') {
                approverComment = $("#frmReqComments #txtApproverComment").val().split("\n");
           // }
            var requisition = {
                RequisitionId: $('#frmReqComments #RequisitionId').val(),
                PlanningContact: $('#frmReqComments #lblPlanningContact').text(),
                RequisitionApprover: $('#frmReqComments #lblRequisitionApprover').text(),
                CreatedBy: $('#frmReqComments #lblCreatedBy').text(),
                RequisitionApproverId: $('#frmReqComments #lblRequisitionApproverId').text(),
                UpdatedBy: $('#frmReqComments #lblUpdatedBy').text(),
                PlannerCommentLst: plannerComment,
                ApproverCommentLst: approverComment,
                requisitionVersion: $('#frmReqComments #CommentReqVersionNo').val()
            };

            ISS.common.executeActionAsynchronous("../Order/RequisitionCommentsSave", JSON.stringify(requisition), function (stat, data) {
                if (stat && data)
                    requisitions.const.comment.close();
            });
        }
        return false;
    },

    getRequisitionComments: function () {
        var reqComments = {
            RequisitionId: $('#frmReqComments #RequisitionId').val(),
            PlanningContact: $('#frmReqSearch #lblPlanningContact').val(),
            RequisitionApprover: $('#frmReqSearch #lblRequisitionApprover').val(),
            CreatedBy: $('#frmReqSearch #lblCreatedBy').val(),
            RequisitionApproverId: $('#frmReqSearch #lblRequisitionApproverId').val(),
            UpdatedBy: $('#frmReqSearch #lblUpdatedBy').val(),
            requisitionVersion: $('#frmReqComments #CommentReqVersionNo').val()
        };
    },


    //-------------------------------Requisition Expand View----------------------------------------
    requisitionExpandClick: function () {

        if ($('#frmRequisitions #FormMode').val() == "Add") {
            ISS.common.notify.error('Please save the requisition.');
            return false;
        }
        if ($(this).attr('id') == 'btnReqExpandView') {

            var requisition = $('#frmRequisitions #RequisitionId').val();

            $('#frmReqExpandView #RequisitionId').val(requisition);

            requisitions.searchReqExpandView();

            requisitions.const.vendor = ISS.common.popUp('.divRequisitionExpandPopup', 'Requisition Expanded View');
        }
        return false;
    
    },

    loadReqExpandViewHeader: function (data) {
        var pattern = /\n/ig;
        if (data.RequisitionComment.ApproverComment != null) {
            data.RequisitionComment.ApproverComment = data.RequisitionComment.ApproverComment.replace(pattern, '<br />');
        }
        if (data.RequisitionComment.PlannerComment != null) {
            data.RequisitionComment.PlannerComment = data.RequisitionComment.PlannerComment.replace(pattern, '<br />');
        }
        $('#frmReqExpandView #lblBusinessUnit').text(requisitions.FormatValue(data.CropBusinessUnit));
        var createdOn = new Date(parseInt(data.CreatedOn.substr(6)));
        var updateOn = new Date(parseInt(data.UpdatedOn.substr(6)));
        var plannedDate = new Date(parseInt(data.PlannedDcDate.substr(6)));
        var approvalSub = new Date(parseInt(data.ApprovalSubmitted.substr(6)));
        var approved = new Date(parseInt(data.Approved.substr(6)));
        $('#frmReqExpandView #lblCreatedOn').text(requisitions.FormatValue(requisitions.format(createdOn, 'MM/dd/yyyy')));
        $('#frmReqExpandView #lblReqStatus').text(requisitions.FormatValue(data.ReqStatusDesc));
        $('#frmReqExpandView #lblUpdatedOn').text(requisitions.FormatValue(requisitions.format(updateOn, 'MM/dd/yyyy')));
        $('#frmReqExpandView #lblUpdatedBy').text(requisitions.FormatValue(data.UpdatedBy));
        $('#frmReqExpandView #lblPlannedDcDate').text(requisitions.format(plannedDate, 'MM/dd/yyyy'));
        $('#frmReqExpandView #lblPlanningContact').text(requisitions.FormatValue(data.PlannerName));
        $('#frmReqExpandView #lblDcLoc').text(requisitions.FormatValue(data.DcLocName));
        $('#frmReqExpandView #lblSourcingContact').text(requisitions.FormatValue(data.SourcingContactName));
        $('#frmReqExpandView #lblSeason').text(requisitions.FormatValue(data.Season));
        $('#frmReqExpandView #lblRequisitionApprover').text(requisitions.FormatValue(data.RequisitionApprover));
        $('#frmReqExpandView #lblProType').text(requisitions.FormatValue(data.ProTypeDesc));
        $('#frmReqExpandView #lblVendorName').text(requisitions.FormatValue(data.VendorName));
        $('#frmReqExpandView #lblMode').text(requisitions.FormatValue(data.Mode));
        $('#frmReqExpandView #lblOverPercentage').text(requisitions.FormatValue(data.OverPercentage));
        $('#frmReqExpandView #lblUnderPercentage').text(requisitions.FormatValue(data.UnderPercentage));
        $('#frmReqExpandView #lblApprovalSubmitted').text(requisitions.FormatValue(requisitions.format(approvalSub, 'MM/dd/yyyy')));
        $('#frmReqExpandView #lblApproved').text(requisitions.FormatValue(requisitions.format(approved, 'MM/dd/yyyy')));
        $('#frmReqExpandView #ExReqVersionNo').val(data.RequisitionVersion);
        $('#frmReqExpandView #lblApproverComment').html(requisitions.FormatValue(data.RequisitionComment.ApproverComment));
        $('#frmReqExpandView #lblPlannerComment').html(requisitions.FormatValue(data.RequisitionComment.PlannerComment)); 
        $('#frmReqExpandView #ExVendorNo').val(data.VendorNo);
        $('#frmReqExpandView #ExVendorLocNo').val(data.LwVendorLoc);
        $('#frmReqExpandView #ExVendorId').val(data.VendorId);
        $('#frmReqExpandView #ExVendorSuffixNo').val(data.VendorSuffix);
        $('#frmReqExpandView #ExLwCompanyNo').val(data.LwCompany);

        $("#btnResettoConstruction").kendoButton().css("visibility", "hidden");
        $("#btnReleasetoSourcing").kendoButton().css("visibility", "hidden");

        if (data.ProdStatus == "R" && data.OrderType!='SR') {
            $("#btnResettoConstruction").kendoButton().css("visibility", "visible");
        }
        if (data.ProdStatus == "L") {
            $("#btnReleasetoSourcing").kendoButton().css("visibility", "visible");
        }
    },



    format: function (time, format) {
        var t = new Date(time);
        var tf = function (i) { return (i < 10 ? '0' : '') + i };
        return format.replace(/yyyy|MM|dd|HH|mm|ss/g, function (a) {
            switch (a) {
                case 'yyyy':
                    return tf(t.getFullYear());
                    break;
                case 'MM':
                    return tf(t.getMonth() + 1);
                    break;
                case 'mm':
                    return tf(t.getMinutes());
                    break;
                case 'dd':
                    return tf(t.getDate());
                    break;
                case 'HH':
                    return tf(t.getHours());
                    break;
                case 'ss':
                    return tf(t.getSeconds());
                    break;
            }
        })
    },

    searchReqExpandView: function () {
        var requisition = $('#frmReqExpandView #RequisitionId').val();

        var requisitionId = {
            RequisitionId: requisition
        };


        requisitions.clearReqExpandView();

        ISS.common.blockUI(true);
        ISS.common.executeActionAsynchronous("../Order/RequisitionExpandView", JSON.stringify(requisitionId), function (stat, data) {
            if (stat && data) {

                requisitions.loadReqExpandViewHeader(data);
                ISS.common.blockUI(false);
            }
            else {
                ISS.common.blockUI(false);
                ISS.common.notify.error('Requisition details missing.');
            }
        });

        ISS.common.executeActionAsynchronous("../Order/ReqExpandViewComponents", JSON.stringify(requisitionId), function (stat, data) {
            if (stat && data) {
                ISS.common.blockUI(true);
                $('#expandgrid').html(data);
                ISS.common.blockUI(false);
            }
            else {
                ISS.common.blockUI(false);
            }
        }, null, null, 'html');

    },

    clearReqExpandView: function () {
        $('#frmReqExpandView .reqExpandClear').text('');
        $('#tblExpandView tbody tr').not(function () { if ($(this).has('th').length) { return true } }).remove();
    },

    FormatValue: function (obj) {
        var formattedValue = obj;
        if (obj == null || obj == '01/01/01' || obj == 'NaN/NaN/NaN') {
            formattedValue = '';
        }
        return formattedValue;
    },

    exportRequisitionExpandView: function () {

        var requisition = $('#frmReqExpandView #RequisitionId').val();
        //alert(requisition);
        if (requisition == null || requisition == '') {
            ISS.common.showPopUpMessage("Please select a requisition id to export the details.", ISS.common.MsgType.Warning);
            return false;
        }
        location.href = location.protocol + '//' + location.hostname + $(this).data('url') + "?requisitionId=" + requisition;

    },

    exportRequisitionDetails: function () {
        var gridDetail = $("#grdRequisitionDetail").data("kendoGrid");
        var dsData = gridDetail.dataSource.data();
        if (dsData.length == 0) {
            ISS.common.showPopUpMessage('No detail to export.')
            return false;
        }

      
        $('#bulkOdrNo').val($('#BulkNumber').val());
        $('#ExpProgramSource').val($('#ProgramSource').val());


        //$('#frmExportRequisition').submit();
    },

    exportRequisitionSearch: function () {
        var gridDetail = $("#grdBulkSearch").data("kendoGrid");
        var dsData = gridDetail.dataSource.data();
        if (dsData.length == 0) {
            ISS.common.showPopUpMessage('No requisition search detail to export.')
            return false;
        }

        //var bulkNumber = $('#BulkNumber').val();
        //$('#bulkOdrNo').val(bulkNumber);

        $('#frmBulkOrdSearch').submit();
    },

    resetToConstruction: function () {
        var requisition = $('#frmReqExpandView #RequisitionId').val();
        var requisitionVersion = $('#frmReqExpandView #ExReqVersionNo').val();

        var requisitionId = {
            RequisitionId: requisition,
            RequisitionVersion: requisitionVersion
        };
        ISS.common.blockUI(true);
        ISS.common.executeActionAsynchronous("../Order/RequisitionResetForConstruction", JSON.stringify(requisitionId), function (stat, data) {
            if (stat && data) {
                if (data.Key) {
                    $("#btnResettoConstruction").kendoButton().css("visibility", "hidden");
                    $("#btnReleasetoSourcing").kendoButton().css("visibility", "visible");
                    requisitions.searchReqExpandView();
                    ISS.common.blockUI(false);
                }
                else {
                    ISS.common.blockUI(false);
                    if (data.Value != "") {
                        ISS.common.showPopUpMessage(data.Value, ISS.common.MsgType.Error);
                    }
                }

            }
            else {
                ISS.common.blockUI(false);
            }
        });
    },

    releaseToSourcing: function () {
       // requisitions.execreleaseToSourcing();
           
        if ($('#NeedSummary').val() == 'True') {
            ISS.common.showConfirmMessage('The requisition not summarized. <br/>It must be summarized in order to submit to sourcing. Would you like to summarize and submit now?', null, function (ret) {
                if(ret)
                    requisitions.execreleaseToSourcing(true);
            });
        }
        else {
            requisitions.execreleaseToSourcing();
        }
      
    },

    execreleaseToSourcing: function (needSum) {

        var requisition = $('#frmReqExpandView #RequisitionId').val();
        var requisitionVersion = $('#frmReqExpandView #ExReqVersionNo').val();
        var VendorNo = $('#frmReqExpandView #ExVendorNo').val();
        var LwVendorLoc = $('#frmReqExpandView #ExVendorLocNo').val();
        var VendorId = $('#frmReqExpandView #ExVendorId').val();
        var VendorSuffix = $('#frmReqExpandView #ExVendorSuffixNo').val();
        var LwCompany = $('#frmReqExpandView #ExLwCompanyNo').val();

        var requisitionId = {
            RequisitionId: requisition,
            RequisitionVersion: requisitionVersion,
            VendorNo: VendorNo,
            LwVendorLoc: LwVendorLoc,
            VendorId: VendorId,
            VendorSuffix: VendorSuffix,
            LwCompany: LwCompany,
            ShowSummaryOnly: (needSum) ? needSum : false,
        };
        ISS.common.blockUI(true);
        ISS.common.executeActionAsynchronous("../Order/ReleaseToSourcing", JSON.stringify(requisitionId), function (stat, data) {
            if (stat && data) {
                if (data.Key) {
                    $("#btnReleasetoSourcing").kendoButton().css("visibility", "hidden");
                    $("#btnResettoConstruction").kendoButton().css("visibility", "visible");
                    requisitions.searchReqExpandView();
                    ISS.common.blockUI(false);
                }
                else {
                    ISS.common.blockUI(false);
                    if (data.Value != "") {
                        ISS.common.showPopUpMessage("Cannot be released to sourcing. <br /> Details: " + data.Value, ISS.common.MsgType.Error);
                    }

                }
            }
            else {
                ISS.common.blockUI(false);
                ISS.common.showPopUpMessage("Cannot be released to sourcing. Please recheck your data.", ISS.common.MsgType.Error);
            }
        });
    },

    ExpandCommentClick: function () {
        
      
        requisitions.clearReqComments();
        requisitions.searchReqComments(true);
        requisitions.const.comment = ISS.common.popUp('.divRequisitionCommentsPopup', 'Requisition Comments')
    },

    BulkCompleteProcess: function () {
        $('#frmRequisitions #FormMode').val("Edit");
        if (requisitions.isValidFormMode()) {
            if (requisitions.const.validator.validate()) {
                var gridDetail = $("#grdRequisitionDetail").data("kendoGrid");
                var dsData = gridDetail.dataSource.data();
                if (dsData.length == 0) {
                    ISS.common.showPopUpMessage('Please enter bulk order detail.')
                    return false;
                }
                ISS.common.blockUI(true);
                
                var PostData = {
                    req: ISS.common.getFormData($('#frmRequisitions')),
                    //reqDet: dsData
                };
                
                ISS.common.executeActionAsynchronous(requisitions.const.urlCompleteProcess, JSON.stringify(PostData), function (stat, data) {
                    ISS.common.blockUI();
                    if (stat && data) {
                        if (data.Key) {
                            ISS.common.showPopUpMessage(data.Value, ISS.common.MsgType.Success, function () {
                                ISS.common.notify.success(data.Value);
                                requisitions.const.validationNeeded = false;
                                requisitions.fillRequisition(PostData.req.BulkNumber)

                            });
                            //$('#frmRequisitions #FormMode').val("Edit");
                        }
                        else {
                            ISS.common.notify.error(data.Value);
                            for (y = 0; y < data.reqDet.length; y++) {
                                PostData.reqDet[y].ErrorStatus = data.reqDet[y].ErrorStatus;
                                PostData.reqDet[y].ErrorMessage = data.reqDet[y].ErrorMessage;
                            }
                            gridDetail.refresh();
                        }
                    } // end stat
                    else {
                        ISS.common.showPopUpMessage('Failed to complete the order details.');
                    }

                }); // end ajax
            }
            else {

            }// end validate
        }
        else {
            ISS.common.notify.error('Please enter valid bulk order number.');
        }
    },

    BulkActivateProcess: function () {
        $('#frmRequisitions #FormMode').val("Edit");
        if (requisitions.isValidFormMode()) {
          //  if (requisitions.const.validator.validate()) {
                var gridDetail = $("#grdRequisitionDetail").data("kendoGrid");
                var dsData = gridDetail.dataSource.data();
                if (dsData.length == 0) {
                    ISS.common.showPopUpMessage('Please enter bulk order detail.')
                    return false;
                }
                ISS.common.blockUI(true);
                
                var PostData = {
                    req: ISS.common.getFormData($('#frmRequisitions')),
                   // reqDet: dsData
                };
                
                ISS.common.executeActionAsynchronous(requisitions.const.urlActivateProcess, JSON.stringify(PostData), function (stat, data) {
                    ISS.common.blockUI();
                    if (stat && data) {
                        if (data.Key) {
                            ISS.common.showPopUpMessage(data.Value, ISS.common.MsgType.Success, function () {
                                ISS.common.notify.success(data.Value);
                                requisitions.const.validationNeeded = false;
                                requisitions.fillRequisition(PostData.req.BulkNumber)

                            });
                            //$('#frmRequisitions #FormMode').val("Edit");
                        }
                        else {
                            ISS.common.notify.error(data.Value);
                            for (y = 0; y < data.reqDet.length; y++) {
                                PostData.reqDet[y].ErrorStatus = data.reqDet[y].ErrorStatus;
                                PostData.reqDet[y].ErrorMessage = data.reqDet[y].ErrorMessage;
                            }
                            gridDetail.refresh();
                        }
                    } // end stat
                    else {
                        ISS.common.showPopUpMessage('Failed to Re-Activate the order details.');
                    }

                }); // end ajax
            //}
            //else {

            //}// end validate
        }
        else {
            ISS.common.notify.error('Please enter valid bulk order number.');
        }
    }


    
};


$.extend(requisitions, temp);




