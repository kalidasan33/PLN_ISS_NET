MS = {

    const: {
        validator: null,
        sizeVal: null
    },

    addInputClass: function (frm) {
        $(frm + ' input:not(:checkbox):not(:radio):not(:button):not(:reset):not(:submit)').addClass('InputF');
    },

    docBlankMSReady: function (IsLoad) {
        MS.addInputClass('#frmSupply');
        ISS.common.toUpperCase('.InputF:not(.excludeF)');
        MS.const.validator = $('#frmSupply').kendoValidator().data("kendoValidator");
        $('#btnSupplySearch').bind('click', MS.MaterialSupplySearch);
        $('#btnSupplyClear').bind('click', MS.ClearMaterialSupply);
        $('#btnPABExport').bind('click', MS.ExportPABDetails);
        $('#Style').bind('focusout', MS.StyleChanged);

        $('#ShowDz').check();

        ISS.common.menuEvent = function () {
            var grdSupply = $("#grdSupply").data("kendoGrid");
            if (grdSupply) {
                grdSupply.refresh();
            }

        }


        //$(window).on('keyup', function (e) {
        //    var code = (e.keyCode ? e.keyCode : e.which);
        //    if (code == 9) {
        //        //get element in focus
        //        var item_name = e.target.name;
        //        //get element in focus
        //        var focused_element = $(document.activeElement);
        //        //console.log(focused_element);
        //        var nextSib = focused_element[0].nextElementSibling;
        //        if (nextSib && nextSib.parentElement && nextSib.parentElement.nextElementSibling) {
        //            var select_element = nextSib.parentElement.nextElementSibling;
        //            //item_name will hold the name of the widget i.e. the value of the input field
        //            //.Name("myWidgetName")
        //            item_name = select_element.id;
        //            var obj_select = '#' + item_name;
        //            //true for multiselect widget
        //            if (select_element.type == 'select-multiple') {
        //                var obj_selector = $(obj_select).data("kendoMultiSelect");
        //                if (obj_selector) {
        //                    if (select_element.value == '') {
        //                        obj_selector.focus();
        //                        obj_selector.open();
        //                    }
        //                }
        //            }
        //            //else if (select_element.type == 'text') {
        //            //    //true for combobox widget
        //            //    var obj_selector = $(obj_select).data("kendoComboBox");
        //            //    if (obj_selector) {
        //            //        obj_selector.open();
        //            //    }
        //            //}
        //        }
        //    }
        //});

    },

    MaterialSupplySearch: function () {
        $('.kgridheight .k-grid-content').scrollLeft(0);
        $(".k-collapse").click();
        if (MS.const.validator.validate()) {
            var grid = $("#grdSupply").data("kendoGrid");
            grid.dataSource.read();
           
        }
        
        return false;
    },

    MSSearchParam: function () {
        var color = $('#Color').data("kendoComboBox").value();
        var attr = $("#Attribute").data("kendoComboBox").text();
        var size = $("#SizeCD").data("kendoMultiSelect").value();
        var dc = $("#DC").data("kendoMultiSelect").value();
        var msSearch = {
            Style: $('#Style').val(),
            Color: color,
            Attribute: attr,
            SizeCD: size.toString(),
            DC: dc.toString(),
            IncludeSuggLots: $('#IncludeSuggLots').prop('checked'),
            ShowDz: $('#ShowDz').prop('checked')
        };

        return msSearch;
    },

    ClearMaterialSupply: function () {
        $('#Style').val('');
        $('#ShowDz').check();
        $('#IncludeSuggLots').uncheck();

        var color = $("#Color").data("kendoComboBox");
        color.dataSource.data([]);
        color.text("");
        color.value("");

        var attr = $("#Attribute").data("kendoComboBox");
        attr.dataSource.data([]);
        attr.text("");
        attr.value("");

        var size = $("#SizeCD").data("kendoMultiSelect");
        size.dataSource.data([]);
        size.value('');

        var dc = $("#DC").data("kendoMultiSelect");
        dc.dataSource.data([]);
        dc.value('');

        var grid = $("#grdSupply").data("kendoGrid");
        grid.dataSource.data([]);
        grid.refresh();

        $("#AllSizes").val('');
        $("#AllDcs").val('');
        MS.const.sizeVal = null;

        MS.const.validator.hideMessages();
        
        return false;
    },

    ExportPABDetails: function () {
        var grid = $("#grdSupply").data("kendoGrid");
        var dsData = grid.dataSource.data();

        var size = $("#SizeCD").data("kendoMultiSelect").value();
        var dc = $("#DC").data("kendoMultiSelect").value();

        $("#AllSizes").val(size.toString());
        $("#AllDcs").val(dc.toString());

        if (dsData.length == 0) {
            ISS.common.showPopUpMessage('No details to export')
            return false;
        }

    },

    StyleChanged: function () {
        $('#Color').data("kendoComboBox").value(null);
        $('#Attribute').data("kendoComboBox").value(null);
        $('#SizeCD').data("kendoMultiSelect").value(null);
        $('#DC').val('');
        MS.loadColor();
    },

    loadColor: function () {
        var postData = MS.retrieveColorData()
        var color = $("#Color").data("kendoComboBox");
        postData = JSON.stringify(postData);
        ISS.common.executeActionAsynchronous(MS.const.urlGetColor, postData, function (stat, data) {
            if (stat) {
                if (data) {
                    color.dataSource.data(data);
                    if (color.value() == '' && data.length > 0)
                        color.value(data[0].Color);
                    MS.loadAttr();
                    if (data.length == 0) {
                        ISS.common.notify.error('Please provide a valid style.');
                    }
                }
            }

        });
    },

    retrieveColorData: function () {
        return {
            SellingStyle: $('#Style').val()
        };
    },

    onColorChange: function () {
        if (ISS.common.validateComboChange(this, 'Invalid Color')) {
            MS.loadAttr();
        }
    },

    loadAttr: function () {
        var postDataAttrib = MS.retrieveAttributeData();
        postDataAttrib = JSON.stringify(postDataAttrib);
        ISS.common.executeActionAsynchronous("../../order/GetAttributeCodes", postDataAttrib, function (stat, data) {
            if (stat) {
                var attrib = $("#Attribute").data('kendoComboBox');
                attrib.dataSource.data(data);
                if (data.length > 0 && attrib.value() == '') {
                    attrib.value(data[0].Attribute);
                    $("#SizeCD").data("kendoMultiSelect").value('');
                    MS.loadSize();
                }
                else if (!ISS.common.validateComboChange(attrib)) {
                    attrib.value('');
                    $("#SizeCD").data("kendoMultiSelect").value('');
                }
                else {
                    MS.loadSize();
                }
            }
        });

    },

    retrieveAttributeData: function () {
        var result = MS.retrieveColorData();
        var color = $('#Color').data("kendoComboBox").value();
        result.ColorCode = color;
        result.Src = "ALL";
        return result;
    },

    loadSize: function () {
        var postData = MS.retrieveSizeData();

        postData = JSON.stringify(postData);
        ISS.common.executeActionAsynchronous(MS.const.urlGetSizes, postData, function (stat, data) {
            if (stat) {
                //if (data.length > 0) {
                //    data.splice(0, 0, { SizeDesc: "ALL", Size: "ALL" });
                //}
                var size = $("#SizeCD").data("kendoMultiSelect");
                size.dataSource.data(data);
                if (MS.const.sizeVal != null)
                    size.value(MS.const.sizeVal);

                MS.const.sizeVal = null;

                
                //if (size.value() == '' && data.length > 0) {
                //    size.value(data[0].SizeDesc)
                //}
                //else if (!ISS.common.validateComboChange(size)) {
                //    size.value('');
                //}
                
            }
        });
    },

    retrieveSizeData: function () {
        var result = MS.retrieveAttributeData();
        var Attribute = $("#Attribute").data("kendoComboBox");
        result.Attribute = Attribute.text();
        return result;
    },

    loadForAWOM: function (style, color, attribute, size) {
        MS.const.sizeVal = size;
        $('#Style').val(style);
        MS.loadColor();
        var c = $('#Color').data("kendoComboBox");
        $('#Color').data("kendoComboBox").value(color);
        MS.loadAttr();
        $('#Attribute').data("kendoComboBox").value(attribute);
        MS.loadSize();
        $('#SizeCD').data("kendoMultiSelect").value(size);
    },

    onAttributeChange: function () {
        if (ISS.common.validateComboChange(this, 'Invalid Attribute')) {
            MS.loadSize();
        }
    },

    onAttrBound: function () {
        var attrib = this;
        var data = this.dataSource.data()
        if (data.length > 0 && attrib.value() == '') {
            attrib.value(data[0].Attribute);
            $("#SizeCD").data("kendoMultiSelect").text('');
            MS.loadSize();
        }
        else if (!ISS.common.validateComboChange(attrib)) {
            attrib.value('');
            $("#SizeCD").data("kendoMultiSelect").text('');
        }
        else {
            MS.loadSize();
        }
    },

    

    initChildGridSize: function (e) {
        var detailGrid = e.detailCell.find('>.k-grid').data().kendoGrid;
        var dataItem = e.data;
        detailGrid.dataSource.data(dataItem.SizeList);
    },

    initChildGridPAB: function (e) {
        var detailGrid = e.detailCell.find('>.k-grid').data().kendoGrid;
        var dataItem = e.data;
        detailGrid.dataSource.data(dataItem.PABList);
    },

    SuppyGridDataBound: function () {
        this.expandRow(this.tbody.find("tr.k-master-row").first());
    }

};