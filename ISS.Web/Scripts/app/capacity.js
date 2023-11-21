
Capacity = {

    Allocation: {
        IsInit: true,
        validator: null
       
   },

    docCapacityReady: function (IsLoad) {        
        $('#btnAllocationsSearch').bind('click', Capacity.loadAllocationsGrid)
        Capacity.Allocation.validator = $('#frmAllocations').kendoValidator().data("kendoValidator");
       

        ISS.common.menuEvent = function () {
            $("#grdAllocations").data("kendoGrid").refresh();
        }
      
        $('#btnAllocationPopupExport').bind("click", Capacity.exportallocationSearch);
        $('#btnAllocationsClear').bind('click', Capacity.clearAllFieldsinCapacity);
    }, 

    searchDataAllocations: function () {     //??    

        var capacity = $("#CapacityGroup").data("kendoMultiSelect");
        var plant = $("#Plant").data("kendoMultiSelect");
        var wc = $("#WorkCenter").data("kendoMultiSelect");
        var showInd = $("#CapIndView").prop("checked");
        var showAgg = $("#CapAggView").prop("checked");
        var orderBy = $("#OrderBy").data("kendoDropDownList");

        var search = {
            CapacityGroup : capacity.value().toString(),
            Plant: plant.value().toString(),
            WorkCenter: wc.value().toString(),
            ShowIndivWorkcenters: showInd,
            ShowAggregateWorkcenters: showAgg,
            OrderBy: orderBy.value().toString()
           
        }

        return search;
    },  
    clearAllFieldsinCapacity: function () {
       
        Capacity.Allocation.validator.hideMessages($('#frmAllocations #CapacityGroup'));
        Capacity.Allocation.validator.hideMessages($('#frmAllocations #Plant'));
        Capacity.Allocation.validator.hideMessages($('#frmAllocations #WorkCenter'));
        $("#Plant").data("kendoMultiSelect").dataSource.data([]);
        $("#WorkCenter").data("kendoMultiSelect").dataSource.data([]);
        var gridDetail = $("#grdAllocations").data("kendoGrid"); 
        ISS.common.clearGridFilters(gridDetail);
        var dsData = gridDetail.dataSource.data([]);
        Capacity.hideDynamicGridHeaders(gridDetail);
    },

    hideDynamicGridHeaders: function (grid) {
        $.each(grid.columns, function (index, item) {
            if (item.field.indexOf('_') != -1) {
                grid.hideColumn(item.field);
            }
        });

    },

    searchAllocationPopup: function () {        

        var capacity = $("#CapacityGroup").data("kendoMultiSelect");
        var plant = $("#wcPlant").val();
        var wc = $("#wc").val();

        var search = {
            CapacityGroup : capacity.value().toString(),
            Plant: plant,
            WorkCenter: wc,
            PlanEndDate: $("#PlanDate").val(),
            ProductionStatus:$("#status").val(),
            CapacityType : $("#capType").val(),
            SpillOver:$("#SpillOver").val()
        }

        return search;
    },

    onAggregateClick: function (e) {
      
  
        Capacity.showAggregationDetails();
        ISS.common.popUp('.divAggregatePopup', 'ISS Aggregation Details: ', null, null, function () {
        });
    }
    ,
    onAllocGridClick: function(e){        
              
        var row = $(e.currentTarget).closest("tr");        
        var idx = $('td', row).index($(e.currentTarget));
        var cellValue = $(e.currentTarget).text();
       
        if(cellValue != null && cellValue != 0)
        {           
            var grid =  $("#grdAllocations").data("kendoGrid");
            var item = grid.dataItem(row);
            var gridColumns = $.grep(grid.columns, function (element, index) {
                                    return element.hidden != true;
                                });            
            var title = gridColumns[idx - 3].title;

            var capacityType = item.Type1;
            $("#wcPlant").val(item.Plant1);
            $("#wc").val(item.Workcenter1);
            $("#capType").val(item.Type1);
            $("#status").val(item.ProductionStatus);
            $("#PlanDate").val(title);            
            $("#SpillOver").val(item.SpillOver);
            if (capacityType != null && capacityType != '') {
                Capacity.showAllocationDetails();
                ISS.common.popUp('.divAllocationPopup', 'ISS Order Details: ' + item.Type + ' for ' + title, null, null, function () {

                    // Filling values for popup export function
                    var capacity = $("#CapacityGroup").data("kendoMultiSelect");
                    $("#hdnDate").val(title);
                    $('#hdnStatus').val(item.ProductionStatus);
                    $('#Capacity').val(capacity.value().toString());
                    $('#hdnPlant').val($("#wcPlant").val());
                    $('#WC').val($("#wc").val());
                    $('#hdnSpill').val($("#SpillOver").val());
                    $('#hdnCapType').val($("#capType").val());
            });
            }
        }
    },  

    loadAllocationsGrid: function (e) {  

        var capacity = $("#CapacityGroup").data("kendoMultiSelect");
        var plant = $("#Plant").data("kendoMultiSelect");
        var wc = $("#WorkCenter").data("kendoMultiSelect");
        
        var capacityValue = capacity.value().toString();
        var plantValue = plant.value().toString();
        var wcValue = wc.value().toString();

        var showInd = $("#CapIndView").prop("checked");
        var showAgg = $("#CapAggView").prop("checked");

        var orderBy = $("#OrderBy").data("kendoDropDownList").value().toString();;




        Capacity.Allocation.validator.validate($('#frmAllocations #CapacityGroup'));
        Capacity.Allocation.validator.validate($('#frmAllocations #Plant'));
        Capacity.Allocation.validator.validate($('#frmAllocations #WorkCenter'));
        
        if (showInd == false && showAgg == false) {
            ISS.common.notify.error("Please select at least capacity View");
            return false;
        }

        //if (plantValue == '' || plantValue == null)
        //{
        //    ISS.common.notify.error("Please select a plant");
        //    return false;
        //}

        //if (wcValue == '' || wcValue == null) {
        //    ISS.common.notify.error("Please select a work center");
        //    return false;
        //}
        if (Capacity.Allocation.validator.validate()) {
            $('#allocationDetail').html(ISS.common.Loader);
            //if (!Capacity.isPanelExpanded('#panelbar-images', 1)) {
            //    $('#LoadIcon').html(ISS.common.Loader);
            //}
            //else {
            //    ISS.common.collapsePanel('#panelbar-images', 0);
            //}
            var search = {
                CapacityGroup: capacityValue,
                Plant: plantValue,
                WorkCenter: wcValue,
                Src: 'true',
                ShowIndivWorkcenters: showInd,
                ShowAggregateWorkcenters: showAgg,
                OrderBy: orderBy
            }

            ISS.common.executeActionAsynchronous('Allocation', JSON.stringify(search), function (stat, data, xhr) {
                if (stat && data) {
                    Capacity.diplayMessageAccordingToResult(xhr);
                    $('#allocationDetail').empty().html(data);
                    var grid = $("#grdAllocations").data("kendoGrid");

                    //popup
                    if (Capacity.Allocation.IsInit) {
                        grid.table.on('click', 'td', function (e) {
                            Capacity.onAllocGridClick(e);
                        });
                        //Capacity.Allocation.IsInit = false;
                    }
                }

            }, null, null, 'html', null);
            ISS.common.collapsePanel('#panelbar-images', 0);
            ISS.common.expandPanel('#panelbar-images', 1);
        }
        return false;             
    },   

    showAllocationDetails: function () {
        var grid = $("#grdAllocationDetail").data("kendoGrid");        
        grid.dataSource.read()
        return false;        
    },

    showAggregationDetails: function () {
        var grid = $("#grdAggregateDetail").data("kendoGrid");
        grid.dataSource.read()
        return false;
    },

    OnOrderByChange : function (e) {
    },

    OnCapacityChange: function (e) {
        var plant = $("#Plant").data("kendoMultiSelect");
        plant.value([]);
        $("#WorkCenter").data("kendoMultiSelect").value([]);
        plant.dataSource.read();        
    },

    openCapacity: function (e) {
        Capacity.Allocation.validator.hideMessages($('#frmAllocations #CapacityGroup'));
    },

    OnPlantChange: function (e) {
        var wcMulti = $("#WorkCenter").data("kendoMultiSelect");       
        wcMulti.dataSource.read();        
    },

    onCapacityGrpData: function () {

        var multiSelect = $('#WorkCenter').data("kendoMultiSelect");        
        multiSelect.value([]);       

        var capacity = $("#CapacityGroup").data("kendoMultiSelect");
        var capacitytId = capacity.value().toString();

        var plant = $("#Plant").data("kendoMultiSelect");
        var plantId = plant.value().toString();       
        var searchView = {
            Plant: plantId, CapacityGroup: capacitytId
        }
        return searchView;
    },

    onCapacityPlantData: function () {

        var capacity = $("#CapacityGroup").data("kendoMultiSelect");
        var capacityGroup = capacity.value().toString();

        var searchView = {
            CapacityGroup: capacityGroup
        }
        return searchView;
    },

    exportallocationSearch: function () {

        var gridDetail = $("#grdAllocationDetail").data("kendoGrid");
        var dsData = gridDetail.dataSource.data();

        if (dsData.length == 0) {
            ISS.common.showPopUpMessage('No details to export.')
            return false;
        } 
       
        $('#frmAllocationDetails').submit();
    },

    onPanelActivate: function (e) {
        var panel = e.item;
        if (panel) {
            if ($(e.item).hasClass('resultClass')) {
                var grid = $("#grdAllocations").data("kendoGrid");
                if (grid) {
                    grid.refresh();
                }
            }
        }
    },

    isPanelExpanded: function(item, idx)
    {
        var panel = $(item).data('kendoPanelBar');
        if (panel) {
            var item = panel.element.children("li").eq(idx);
            return item.is(".k-state-active");
        }
    },

    diplayMessageAccordingToResult: function (xhr) {
        // Display message acccording to result
        var count = xhr.getResponseHeader("count");
        if (count == "-1") {
            ISS.common.notify.error("Unexpected error occured. Please try again.");
        }
        else if (count == "0") {
            ISS.common.notify.info("No records to display. Please search again with another criteria");
        }        
        $('#LoadIcon').html(' ');
    },
};
 

