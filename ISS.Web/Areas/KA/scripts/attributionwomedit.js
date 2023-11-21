

tWOM = {


    docWOMReady2: function (IsLoad) {
        window.sserror = false;
        WOM.const.validatorEdit = $("#frmWOMEdit").kendoValidator().data("kendoValidator")
        $('#frmWOMEdit #Style').bind('focusout', WOM.StyleChanged);
        $('#frmWOMEdit #Revision').bind('click', WOM.revSearchClick);
        $('#frmWOMEdit #MfgPathId').bind('click', WOM.showMfgPathId);
        $('#frmWOMEdit #PackCode').bind('click', WOM.PackCodeClick);
        $('#frmWOMEdit #CategoryCode').bind('click', WOM.CatCodeClick);
        $('#frmWOMEdit #MC').bind('click', WOM.showMachineCodes);
        $('#frmWOMEdit #AltId').bind('click', WOM.showAlternateId);
        //$('#frmWOMEdit #CutPath').bind('click', WOM.PopulateCutPathTxtPath);
        //$('#frmWOMEdit #TxtPath').bind('click', WOM.PopulateCutPathTxtPath);   
        $('#frmWOMEdit #CutPath').bind('click', function () {
            WOM.PopulateCutPathTxtPath('CutPath');
            return false;
        });
        $('#frmWOMEdit #TxtPath').bind('click', function () {
            WOM.PopulateCutPathTxtPath('TxtPath');
            return false;
        });
        $('#frmWOMMassChange #btnCatSearch').bind('click', function () {
            WOM.CatCodeClick('CatCD');
            return false;
        });
        $('#frmWOMEdit #Revision').keypress(function (e) {
            if (e.which == 13) {
                WOM.revSearchClick();
                return false;
            }
        });
        $('#frmWOMEdit #MfgPathId').keypress(function (e) {
            if (e.which == 13) {
                WOM.showMfgPathId();
                return false;
            }
        });
        $('#frmWOMEdit #PackCode').keypress(function (e) {
            if (e.which == 13) {
                WOM.PackCodeClick();
                return false;
            }
        });
        $('#frmWOMEdit #CategoryCode').keypress(function (e) {
            if (e.which == 13) {
                WOM.CatCodeClick();
                return false;
            }
        });
        $('#frmWOMEdit #MC').keypress(function (e) {
            if (e.which == 13) {
                WOM.showMachineCodes();
                return false;
            }
        });
        $('#frmWOMEdit #AltId').keypress(function (e) {
            if (e.which == 13) {
                WOM.showAlternateId();
                return false;
            }
        });
        $('#frmWOMEdit #CutPath').keypress(function (e) {
            if (e.which == 13) {
                WOM.PopulateCutPathTxtPath('CutPath');
                return false;
            }
        });
        $('#frmWOMEdit #TxtPath').keypress(function (e) {
            if (e.which == 13) {
                WOM.PopulateCutPathTxtPath('TxtPath');
                return false;
            }
        });
        $('#frmWOMMassChange #btnCutPathSearch').bind('click', WOM.showCutPath);
        $('#frmWOMMassChange #Txtpath').bind('focusout', WOM.validateTXTPath);
        $('#frmWOMMassChange #btnSewPathSearch').bind('click', WOM.showSewPath);

        $('#frmWOMMassChange #CatCD').keypress(function (e) {
            if (e.which == 13) {
                WOM.CatCodeClick('CatCD');
                return false;
            }
        });
        $('#frmWOMMassChange #CutPath').keypress(function (e) {
            if (e.which == 13) {
                WOM.showCutPath();
                return false;
            }
        });
        //$('#frmWOMMassChange #MfgPathId, #btnSewPathSearch').keypress(function (e) {
        $('#frmWOMMassChange #btnSewPathSearch').keypress(function (e) {
            if (e.which == 13) {
                WOM.showSewPath();
                return false;
            }
        });
        $('#frmWOMEdit .EditCancel').on('keydown', function (e) {
            if (e.which == 9) {
                $('#frmWOMEdit #OrderStatus').data("kendoDropDownList").focus();
                return false;

            }
        });

        $('#fabricDetailGrid').on('change', '.PFSChkpopup :checkbox', function (e) {
            var grid=$('#fabricDetailGrid #grdFabricDetailPFS').data("kendoGrid");
            var dataItem = grid.dataItem($(e.currentTarget).closest("tr"));
            dataItem.PFSInd = $(e.currentTarget).prop('checked');
            if (dataItem.PFSInd) dataItem.PFS = 1; else dataItem.PFS = 0;
            dataItem.IsEdited = !dataItem.IsEdited;
        });
        $('.cancelGridPFS').bind('click', function () {
            ISS.common.commonPopup.close();
        });
        $('.saveGridPFS').bind('click', WOM.savePFSDetails); 
        $('.EditCancel').bind('click', WOM.cancelEdit);
        $('.saveEdit').bind('click', WOM.saveEdit);

        ISS.common.eachesQtyRestriction('.DozenEaches');

    },

    SetGridDataValues: function (data) {
        data.IsEdited = true;
        data.IsFieldChange = true;
        data.IsPFSChange = true;
    },

    retrieveSellingData:function(){
        var result= {
            SellingStyle: WOM.const.currentRow.SellingStyle,
            ColorCode: WOM.const.currentRow.SellingColor,
            Attribute: WOM.const.currentRow.SellingAttribute,
            Size: WOM.const.currentRow.SellingSize,
        };
        result.SizeList = new Array();
        result.SizeList.push({ SizeCD: result.Size })
        return result;
    },

    onColorBound:function(){
        //var attr = $('#frmWOMEdit #Attribute').data("kendoDropDownList")       
        //attr.dataSource.read();
    },

    onAttrBound: function () {
        //var attrib = this;
        //var data = this.dataSource.data()
        //if (data.length > 0 && attrib.value() == '') {
        //    attrib.value(data[0].Attribute);
        //    $("#Size").data("kendoComboBox").text('');
        //   // WOM.loadSize();
        //}
        //else if (!ISS.common.validateComboChange(attrib)) {
        //    attrib.value('');
        //    $("#Size").data("kendoComboBox").text('');
        //}
        //else {
        //    //WOM.loadSize();
        //}
    },
    loadColor: function () {
        var postData = WOM.retrieveColorData()
        var color = $("#Color").data("kendoComboBox");
        postData = JSON.stringify(postData);
        ISS.common.executeActionAsynchronous(WOM.const.urlGetColor, postData, function(stat, data) {
            if (stat) {
                if (data) {
                    color.dataSource.data(data);                   
                    if (color.value() == '' && data.length > 0)
                        color.value(data[0].Color);                    
                    var attrib = $("#Attribute").data("kendoComboBox");
                    attrib.dataSource.read();
                    if(data.length == 0) {
                        ISS.common.notify.error('Please provide a valid style.');
                    }
                }
            }

            });
    },
    loadSize: function () {

        var postData = WOM.retrieveSizeData();

        postData = JSON.stringify(postData);
        ISS.common.executeActionAsynchronous(WOM.const.urlGetSizes, postData, function (stat, data) {
            if (stat) {
                var size = $("#frmWOMEdit #Size").data("kendoComboBox");
                size.dataSource.data(data);
                if (size.value() == '' && data.length > 0) {                   
                    size.value(data[0].SizeDesc)
                    if (size.select() == -1) {
                        size.open()
                        size.close()
                    }                    
                }
                else if (!ISS.common.validateComboChange(size)) {
                    size.value(null);
                }
                size.trigger('change')
            }
        });
    },
    retrieveColorData: function () {
        return {
            SellingStyle: $('#frmWOMEdit #Style').val()
        };
    },

    retrieveAttributeData: function () {
        var result = WOM.retrieveColorData();
        var color = $('#Color').data("kendoComboBox").value();       
        result.ColorCode = color;
        result.Src = "AO";
        return result;
    },
    getMultiselectItem: function (items) {
        var selColorCode = '';
        if (items.length > 0)
            selColorCode = items[0];
        return selColorCode;
    },
    retrieveSizeData: function () {
        var result = WOM.retrieveAttributeData();
        var Attribute = $("#frmWOMEdit #Attribute").data("kendoComboBox");
        result.Attribute = Attribute.text();
        return result;
    },

    retrieveRevData: function () {
        var result = WOM.retrieveSizeData();
        result.SizeList = new Array();
        //var size = $("#frmWOMEdit #Size").data("kendoMultiSelect");
        var size = $("#frmWOMEdit #Size").data("kendoComboBox");
        var sizeCd = '';
        var sizeDesc = '';
        sizeCd = size.value();
        sizeDesc = size.text();
        //var v = size.value();
        //if (v.length > 0) {
        //    sizeCd = v[0];
        //    sizeDesc = size.dataItems()[0].SizeDesc;
        //}
        result.SizeList.push({ SizeCD: sizeCd, Size: sizeDesc })
        result.Size = sizeCd;
        return result;
    },


    StyleChanged: function () {
        
        $("#frmWOMEdit").kendoValidator().data("kendoValidator")
        WOM.loadColor();
        var postData = WOM.retrieveColorData();
        postData = JSON.stringify(postData);
        if ($.trim(postData.Style) != '') {
            ISS.common.executeActionAsynchronous(WOM.const.urlGetWOAsrtCode, postData, function (stat, data) {
                if (stat) { 
                    if (data.length > 0) {
                        $("#AssortCode").val(data[0].AssortCode);
                        $("#PrimaryDC").val(data[0].PrimaryDC);
                        $("#frmWOMEdit #PackCode").val(data[0].PackCode).change();
                        $("#Dc").val(data[0].PrimaryDC).change();
                        $("#OriginTypeCode").val(data[0].OriginTypeCode);
                        $("#BusinessUnit").val(data[0].CorpBusUnit);
                        // WOM.updateCumulativeAndFabric();
                    }
                }
                else {
                    ISS.common.notify.error   ('Failed to retrieve Style details.');

                }

            });
        }

    },

    onColorChange: function () {
        if (ISS.common.validateComboChange(this, 'Invalid Color')) {
            var ds = $("#frmWOMEdit #Attribute").data("kendoComboBox");
            ds.dataSource.read();
        }
    },

    

    onAttributeChange: function () {
        if (ISS.common.validateComboChange(this, 'Invalid Attribute')) {
            WOM.loadSize();
        }
    },
   
  

    onSizeChange: function () {
        //WOM.CreateOrder.validator.hideMessages($('#frmWO #Revision'));
        if (ISS.common.validateComboChange(this, 'Invalid Attribute')) {
            var postData = WOM.retrieveRevData();


            postData.AssortCode = $("#AssortCode").val();//TBD
            postData = JSON.stringify(postData);

            ISS.common.executeActionAsynchronous(WOM.const.urlGetMaxRevision, postData, function (stat, data) {
                if (stat) {
                    if (data.length > 0) {
                        $("#frmWOMEdit #Revision").val(data[0].Revision).change();
                        //WOM.getChildSKU(); 
                    }
                }
                else {
                    ISS.common.notify.error('Failed to retrieve Revision details.');
                }

            });
        }

    },

    revSearchClick: function () {
        //if ($("#frmWOMEdit #SellingColor").val() == '') {
        //    ISS.common.showPopUpMessage('Please enter a Selling Style.');
        //    return false;
        //}
        //else {
            WOM.loadRevDetailsGrid();
            WOM.const.RevPopup = ISS.common.popUp('.divRevSearchPopup', 'Revision Search', null, function (rr) {
                if (rr.userTriggered) {
                    rr._defaultPrevented = true;
                    WOM.const.RevPopup.close();
                    $('#Revision').focus();
                }
            });
        //}
    },

    loadRevDetailsGrid: function () {
        var grid = $("#grdRevDetails").data("kendoGrid");
        if (grid.pager.page() > 1) grid.pager.page(1)
        grid.dataSource.read();
        return false;
    },

    searchRevDetails: function () {
        var res = WOM.retrieveSellingData()           
        res["Revision"] = $('#frmWOMEdit #Revision').val();
        res["AssortCode"] = $('#frmWOMEdit #AssortCode').val()
        return res;
    },

    showRevDetails: function (e) {

        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        $('#frmWOMEdit #Style').val(dataItem.PKGStyle);
        if (dataItem.PKGStyle != WOM.const.currentRow.Cloned.Style) {
            $("#frmWOMEdit #IsSKUChange").val('true');
        }
        $('#frmWOMEdit #Revision').val(dataItem.NewRevision).focus();
        WOM.const.RevPopup.close();
        var postData ={wod: { SellingStyle : WOM.const.currentRow.SellingStyle }};        
        postData = JSON.stringify(postData);
        ISS.common.executeActionAsynchronous(WOM.const.urlGetWOAsrtCode, postData, function (stat, data) {
            if (stat && data) {
                if (data.length > 0) {
                    $("#frmWOMEdit #AssortCode").val(data[0].AssortCode);
                    $("#frmWOMEdit #PrimaryDC").val(data[0].PrimaryDC);
                    $("#frmWOMEdit #PackCode").val(data[0].PackCode).change();
                    $("#frmWOMEdit #DcLoc").val(data[0].PrimaryDC).change();
                    $("#frmWOMEdit #OriginTypeCode").val(data[0].OriginTypeCode);
                    $("#frmWOMEdit #BusinessUnit").val(data[0].CorpBusUnit);
                
                    var pData = WOM.retrieveSellingData();
                    pData.OriginTypeCode = data[0].OriginTypeCode
                    pData.Revision = dataItem.NewRevision
                    pData.AssortCode = data[0].AssortCode
                    pData = { wod: pData };
                    pData = JSON.stringify(pData);
                    ISS.common.executeActionAsynchronous(WOM.const.urlGetWOChildSKU, pData, function (st, res) {
                        if (st && res && res.length>0) {
                            //$("#frmWOMEdit #NewStyle").val(res[0].NewStyle); PKG Style already changed added
                            $("#frmWOMEdit #Color").val(res[0].NewColor);
                            $("#frmWOMEdit #Attribute").val(res[0].NewAttribute);                             
                            //$("#frmWOMEdit #Size").val(data[0].NewSize);
                        }
                        $('#frmWOMEdit #Revision').focus();
                    });
                }
            }
        });
        return false    ;
    },

    PackCodeClick: function () {
        WOM.loadPackCodeGrid();
        WOM.const.RevPopup = ISS.common.popUp('.divPackCodePopup', 'Pack Code', null, function (rr) {
            if (rr.userTriggered) {
                rr._defaultPrevented = true;
                WOM.const.RevPopup.close();
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
        WOM.const.RevPopup.close();
        $('#frmWOMEdit #PackCode').val(dataItem.PackCode).focus();
        // WOM.updateCumulativeAndFabric();
        return false;
    },
    CatCodeClick: function (catName) {
        if (catName != null && catName == "CatCD") {
            var gridData = gridWOM.dataSource.view();
            if (gridData.length > 0) {
                WOM.loadCatCodeGrid();
                WOM.const.RevPopup = ISS.common.popUp('.divCatCodePopup', 'Category Code', null, function (rr) {
                    if (rr.userTriggered) {
                        rr._defaultPrevented = true;
                        WOM.const.RevPopup.close();
                        $('#CategoryCode').focus();
                    }
                });
                WOM.const.RevPopup.settings = { Src: catName }
            }
            else {
                ISS.common.showPopUpMessage("Please select an Order first.");
                return false;
            }
        }
        else {
            WOM.loadCatCodeGrid();
            WOM.const.RevPopup = ISS.common.popUp('.divCatCodePopup', 'Category Code', null, function (rr) {
                if (rr.userTriggered) {
                    rr._defaultPrevented = true;
                    WOM.const.RevPopup.close();
                    $('#CategoryCode').focus();
                }
            });
            WOM.const.RevPopup.settings = { Src: $(this).attr('name') }
            WOM.const.RevPopup.settings = { Src: 'CategoryCode' }
        }
    },
    loadCatCodeGrid: function () {
        var grid = $("#grdCatCode").data("kendoGrid");
        if (grid.pager.page() > 1) grid.pager.page(1)
        grid.dataSource.read();
        return false;
    },

    showCatCodeDetails: function (e) {
        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        WOM.const.RevPopup.close();
        $('#' + WOM.const.RevPopup.settings.Src).val(dataItem.CategoryCode).focus();
        return false;
    },


    showMachineCodes: function (e) {
        
        var res = { SuperOrder: WOM.const.currentRow.SuperOrder }
        res.Plant = $('#frmWOMEdit #CutPath').val();  
        settings = {
            columns: [{
                Name: "iMacTypeCode",
                Title: "Machine",
            } ],
            AllowSelect: true,
            title: 'Select Machine',
            url: WOM.const.urlMachineCodes,
            postData: res,
            handler: function (d) {
                $('#frmWOMEdit #MC').val(d.iMacTypeCode).focus();
                return false;
            }
        };
        ISS.common.CommonSearchShow(settings);

    },



    showAlternateId: function (e) {

        var res = WOM.retrieveSellingData();
        settings = {
            columns: [{
                Name: "CuttingAlt",
                Title: "Alt Id",
            }],
            AllowSelect: true,
            title: 'Select Alternate Id',
            url: WOM.const.urlGetAlternateId,
            postData: res,
            handler: function (d) {
                $('#frmWOMEdit #AltId').val(d.AltId).focus();
            }
        };
        ISS.common.CommonSearchShow(settings);

    },


    mfgPathClick: function () {
        var ColorCode = $('#frmWOMEdit #Color').data("kendoComboBox").value();
        //var ColorCode = $("#frmWOMEdit #Color").data("kendoMultiSelect").value();
        var Attribute = $('#frmWOMEdit #Attribute').data("kendoComboBox").text();
        var Size = $('#frmWOMEdit #Size').data("kendoComboBox").value();
        //if (ColorCode.length > 0)
        //    ColorCode = ColorCode[0];
        //if (Size.length > 0)
        //    Size = Size[0];
        if ($('#frmWOMEdit #Style').val() != '' && ColorCode != '' && Attribute != '' && Size!='') {
            WOM.loadmfgPathDetailsGrid();
            WOM.const.RevPopup = ISS.common.popUp('.divmfgPathPopup', 'Select a Sew Plant', null, function (rr) {
                if (rr.userTriggered) {
                    rr._defaultPrevented = true;
                    WOM.const.RevPopup.close();
                    $('#frmWOMEdit #MfgPathId').focus();
                }
                else {
                    $('#frmWOMEdit #MfgPathId').focus();
                }
                
            });
            
        }
        else {
            ISS.common.notify.error('Please enter Style, Color, Attribute and Size to select Path Id.');

        }
        return false;
    },


    retrieveMFGData: function () {
        var result = WOM.retrieveRevData();
        var demLoc = $("#frmWOMEdit #DcLoc").val();
        result.PrimaryDC = demLoc;
        return result;
    },


    loadmfgPathDetailsGrid: function () {
        var grid = $("#grdmfPathDetails").data("kendoGrid");
        if (grid.pager.page() > 1) grid.pager.page(1)
        grid.dataSource.read();
        return false;
    },

    showmfgPathDetails: function (e) {
        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        WOM.const.RevPopup.close();
        $('#frmWOMEdit #MfgPathId').val(dataItem.MfgPathId).focus();
        $('#frmWOMEdit #SewPath').val(dataItem.SewPlt);
        return false;

    },
    TotDozChangedEdit: function () {
       
        var TotDoz = $('#frmWOMEdit #TotalDozens').data("kendoNumericTextBox").value();
        $('#frmWOMEdit #QtyEach').val(WOM.TotDozChanged(WOM.const.currentRow, TotDoz)).change();
    },
    TotDozChanged: function (data, TotDoz) {      
        if (data.DescreteInd == "Y") {
            data.ScrapFactor = 0;
        }
        if (data.ScrapFactor <= 1) {
            TotDoz = TotDoz - (TotDoz * data.ScrapFactor)
        }
        var Doz = Math.round(TotDoz);
        return Doz;
    },

    PopulateCutPathTxtPath: function (name) {

        var res = { SuperOrder: (WOM.const.currentRow) ? WOM.const.currentRow.SuperOrder : null }
        res.DyeCode = $('#frmWOMEdit #' + name).data('dyecode');
        res.MFGPathId = $('#frmWOMEdit #MfgPathId').val();
        res.CutPath = $('#frmWOMEdit #CutPath').val();

        var Name = name;
        var form = 'frmWOMEdit'; // $(this).closest('form').attr('id');


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
            title: (res.DyeCode == 'C') ? 'Select a Cut Plant' : 'Select a Txt Plant',
            url: WOM.const.urlPopulateCutPathTxtPath,
            postData: res,
            handler: function (d) {
                $('#' + form + ' #' + Name).val(d.Source_Plant).focus();
                //  d.Priority ??
                if (Name == 'TxtPath') {
                    WOM.validatePlant(d.Source_Plant)
                }
                return false;
            }
        };
        ISS.common.CommonSearchShow(settings);
    },

    validatePlant: function (Plant,callback) {
        ISS.common.executeActionAsynchronous('ValidatePlant',JSON.stringify( { Plant: Plant }), function (stat, res) {
            if (stat ) {
                if (!res) {
                    if (callback) {
                        callback(false);
                    }
                    else {
                        ISS.common.notify.error('Plant is not valid ' + Plant);
                    }                   
                }
                return res;
            }
        });
    },

    validateSEDate:function(e){
        if(! WOM.validate60daysDate( e.sender.value())){       
            e.sender.value(WOM.const.currentRow[e.sender.element[0].name]);
        }            
    },
    validate60daysDate:function(ed){
        var tt = new Date();
        tt.setFullYear(serverDate.getFullYear());
        tt.setMonth(serverDate.getMonth() + 1);
        tt.setDate(serverDate.getDate())
        tt.setDate(-60)        
        if (ed == 'Invalid Date') {
            ISS.common.notify.error('Invalid date format.');
            return false;
        }
        else if (ed < tt) {
            ISS.common.notify.error('Date cannot be 60 days past due.');
            return false;
        }
        return true;
    },

    showPFSDetails: function (data) {
        var ds=new Array();
        ds.push(data);
        settings = {
            columns: [
            {
                Name: "OrderId",
                Title: "Lot #",
            }, 
            {
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
                Title: "Size Code",
            },
            {
                Name: "SizeShortDes",
                Title: "Size",
            },
            {
                Name: "QtyDZ",
                Title: "FQ Dz",
            },
            {
                Name: "TotalDozens",
                Title: "Total Dz",
            },
            ],
            AllowSelect: false,            
            dataSource:ds ,
            targetElement: '#orderDetailgrid',
            GridName: 'grdOrderDetailPFS',
            HideFilter: true,
            HidePager: true,
        };
        ISS.common.getCommonSearchGrid(settings);

        settings = {
            columns: [
             {
                 Name: "GarmentStyle",
                 Title: "Grmt Style",
             },
            {
                Name: "GarmentColor",
                Title: "Grmt Color",
            },
            {
                Name: "Style",
                Title: "Fabric",
            },
            {
                Name: "CylinderSizes",
                Title: "Cyl Size",
            },
            {
                Name: "SpreadTypeCode",
                Title: "Spread",
            },
            {
                Name: "SpreadCompCode",
                Title: "Comp Cd",
            },
            {
                Name: "DyeCode",
                Title: "Dye Cd",
            },
            {
                Name: "MachineCut",
                Title: "Machine Cut",
            },
            {
                Name: "Lbs",
                Title: "Lbs",
            },
            {
                Name: "PFSInd",
                Title: "Pull From Stock",
                NeedCheckBox:true,
                CssClass: 'PFSChkpopup',
            },
            ],
            AllowSelect: false,
           
            targetElement: '#fabricDetailGrid',
            GridName: 'grdFabricDetailPFS',
            HideFilter: true,
            HidePager: true,
            DataBound: WOM.grdfabricdetailpfsbound ,
        };
        if (!WOM.IsAllowEdit(data) || data.IsDeleted) {
            $('.saveGridPFS').hide();
        }
        else {
            $('.saveGridPFS').show();
        }
        if (data.PFSList) { // if already loaded
            settings.dataSource = data.PFSList;
        }
        else {
            settings.url = WOM.const.urlPFSDetails;
            settings.postData = data;
        }
        settings.currentPopUpRow = data;
        ISS.common.getCommonSearchGrid(settings);
        ISS.common.commonPopup = ISS.common.popUp("#FabricPopup", 'Edit', null, function () {
            $("#fabricDetailGrid, #orderDetailgrid").empty();
        })
        ISS.common.commonPopup.title('Edit');
        
    },

    savePFSDetails:function(){
        var grid = $('#fabricDetailGrid #grdFabricDetailPFS').data("kendoGrid");
        grid.settings.currentPopUpRow.PFSList = JSON.parse(JSON.stringify(grid.dataSource.data()));
        grid.settings.currentPopUpRow.IsEdited = true;
        grid.settings.currentPopUpRow.IsPFSChange = true;
        ISS.common.cloneAndStore(grid.settings.currentPopUpRow);
        grid.settings.currentPopUpRow.Cloned.PFSInd = !grid.settings.currentPopUpRow.PFSInd;
        ISS.common.commonPopup.close();
        gridWOM.refresh();
    },

    grdfabricdetailpfsbound: function (grid) {
        var gridData = grid.dataSource.view();       
        for (var i = 0; i < gridData.length; i++) {
            var row = grid.table.find("tr[data-uid='" + gridData[i].uid + "']");
            var item = gridData[i]; 
            if (item.IsHide) {
                    row.hide();
                }           
        }      
    },


    showPFSDetailsEdit: function (e) {
        if(WOM.isValidEditMode())
            WOM.showPFSDetails(WOM.const.currentRow);
    },

    onChangeProdStatus: function (e) {
        return true;
        var th = this;
        var val = this.value();
        if ( val == 'R' || val == 'L') {
            var pdata = {
                AssortCode: $('#frmWOMEdit #AssortCode').val(),
                TxtPath: $('#frmWOMEdit #TxtPath').val(),
                OrderStatus: val,
                OldOrderStatus:WOM.const.currentRow.OrderStatus,
                SuperOrder: WOM.const.currentRow.SuperOrder,
            }
            ISS.common.execute('ValidateProdStatus', pdata, function (stat, data) {
                if (stat && data) {
                    if (!data.Status && data.Msg) {
                        ISS.common.showPopUpMessage(data.Msg)
                    }
                    if (data.BOMUpdate) $('#frmWOMEdit #BOMUpdate[type="checkbox"]').checked();
                    if (data.OrderStatus) th.value(data.OrderStatus);
                }
            });
        }
        else {
            WOM.const.currentRow.BOMUpdate ? $('#frmWOMEdit #BOMUpdate[type="checkbox"]').check() : $('#frmWOMEdit #BOMUpdate[type="checkbox"]').uncheck();
        }
    },

    ValidateProdStatus: function (order, OldOrderStatus) {
        return false;
        var pdata = {
            AssortCode: order.AssortCode, TxtPath: order.TxtPath,
            OrderStatus: order.OrderStatus,
            OldOrderStatus: OldOrderStatus,
            SuperOrder: order.SuperOrder
        };
        var retData = ISS.common.execute('ValidateProdStatus', pdata, function (stat , data) {
            if (stat && data) {
                if (!data.Status && data.Msg) {
                    ISS.common.notify.error(data.Msg)
                }
                if (data.BOMUpdate) order.BOMUpdate = data.BOMUpdate;
                if (data.OrderStatus) {
                    order.OrderStatus = data.OrderStatus;
                    WOM.setOrderType(order);
                    //grid refresh required
                }
            }
        });
         
    },

    isValidEditMode: function () {
        return (WOM.const.currentRow) ? true : false;
    },

    editRow: function (data) {
       // $('.WOMEditor').show()
        WOM.const.editPopUp = ISS.common.popUp('.WOMEditor', 'Edit', null, function (e) {
            if (e.userTriggered)
                WOM.cancelEdit(null,true);
        });
       // ISS.common.scrollTo('#Note');
        WOM.const.currentRow = data;
        if (data.Note == null) data.Note = '';
        if (!data.Cloned) data.QtyEach = ISS.common.getQtyToEachDisplay(data.QtyEach);
        ISS.common.cloneAndStore(data);
        
        // $('#frmWOMEdit #gridMode').val(data.gridMode);
        $('#frmWOMEdit #AssortCode').val(data.AssortCode);
         $('#frmWOMEdit #PrimaryDC').val(data.PrimaryDC);
         //$('#frmWOMEdit #OrderDetailId').val(data.OrderDetailId);
         $('#frmWOMEdit #OriginTypeCode').val(data.OriginTypeCode);
        // $('#frmWOMEdit #RoutingId').val(data.RoutingId);
       
         $('#frmWOMEdit #BusinessUnit').val(data.BusinessUnit);
         $('#frmWOMEdit #OrderStatus').data("kendoDropDownList").value(data.OrderStatus);
         $('#frmWOMEdit #Style').val(data.Style);

        
         var color = $('#frmWOMEdit #Color').data("kendoComboBox");
         color.value(data.Color);
         WOM.loadColor();
         
         var attr = $('#frmWOMEdit #Attribute').data("kendoComboBox");
         attr.text(data.Attribute);

         WOM.loadSize();
         var size = $('#frmWOMEdit #Size').data("kendoComboBox");
         var arr = new Array();
         arr.push({ Size: data.Size, SizeDesc: data.SizeShortDes })
         size.dataSource.data(arr);
         size.value(data.Size);
        
         $('#frmWOMEdit #Revision').val(data.Revision);
         //$('#frmWOMEdit #AltId').val(data.AltId);
        //$('#frmWOMEdit #TotalDozens').data("kendoNumericTextBox").value(data.TotalDozens);
         $('#frmWOMEdit #QtyEach').val(data.QtyEach).change();
         //$('#frmWOMEdit #MC').val(data.MC);
         //$('#frmWOMEdit #CylinderSizes').val(data.CylinderSizes);
         //$('#frmWOMEdit #PFSInd').prop('checked', data.PFSInd);
        // $('#frmWOMEdit #StartDate').data("kendoDatePicker").value(data.StartDate);
         $('#frmWOMEdit #CurrDueDate').data("kendoDatePicker").value(data.CurrDueDate);
         //$('#frmWOMEdit #TxtPath').val(data.TxtPath);
         //$('#frmWOMEdit #CutPath').val(data.CutPath);
         //$('#frmWOMEdit #SewPath').val(data.SewPath);
         $('#frmWOMEdit #Atr').val(data.Atr);
         $('#frmWOMEdit #MfgPathId').val(data.MfgPathId);
         $('#frmWOMEdit #MFGPlant').val(data.MFGPlant);
         $('#frmWOMEdit #DcLoc').val(data.DcLoc);
         $('#frmWOMEdit #ExpeditePriority').data("kendoNumericTextBox").value(data.ExpeditePriority);
         //$('#frmWOMEdit #CategoryCode').val(data.CategoryCode);
         ///$('#frmWOMEdit #PackCode').val(data.PackCode);
         //$('#frmWOMEdit #LbsStr').val(data.LbsStr);
         //$('#frmWOMEdit #MC').val(data.MC);

         //$('#frmWOMEdit #CreateBd[value="CreateBd"]').prop('checked', data.CreateBd)
        // $('#frmWOMEdit #CreateBd[value="DozensOnly"]').prop('checked', data.DozensOnly)
                
         //$('#frmWOMEdit #BOMUpdate[type="checkbox"]').prop('checked', data.BOMUpdate);

         
       
         WOM.const.validatorEdit.hideMessages();
         $("#frmWOMEdit #Note").val(data.Note);
         if (data.NoteInd == 'N') {
             var superOrder = { superOrder: data.SuperOrder }
             superOrder = JSON.stringify(superOrder);
             ISS.common.executeActionAsynchronous(WOM.const.urlGetNote, superOrder, function (stat, data) {
                 if (stat && data) {
                     $("#frmWOMEdit #Note").val(data);
                 }
             });
         }
         $('#frmWOMEdit #OrderStatus').data("kendoDropDownList").focus();
         return false;


    },

   
    cancelEdit: function (e,flg) {
        WOM.clearEditFields();       
        //$('.WOMEditor').hide();
        if (!flg) WOM.const.editPopUp.close();
        //ISS.common.scrollTo('.linkDetailspanel');
        return false;
    },

    clearEditFields: function () {
       
        WOM.const.validatorEdit.hideMessages();
        WOM.const.currentRow = null;
         $('#frmWOMEdit #gridMode').val('');
         $('#frmWOMEdit #AssortCode').val('');
         $('#frmWOMEdit #PrimaryDC').val('');
         $('#frmWOMEdit #OrderDetailId').val('');
         $('#frmWOMEdit #OriginTypeCode').val('');
        //  $('#frmWOMEdit #RoutingId').val('');
                 
         $('#frmWOMEdit #BusinessUnit').val('');
         $('#frmWOMEdit #OrderStatus').data("kendoDropDownList").value('L');
         $('#frmWOMEdit #Style').val('');
        
         //var style = $('#frmWOMEdit #Color').data("kendoMultiSelect");
         //style.value('');
        //style.dataSource.read();

         var style = $('#frmWOMEdit #Color').data("kendoComboBox");
         style.value('');
         style.dataSource.read();

         var attr = $('#frmWOMEdit #Attribute').data("kendoComboBox");
         attr.value('');       

         var size = $('#frmWOMEdit #Size').data("kendoComboBox");
         size.value(null);

         $('#frmWOMEdit #Revision').val('');
         $('#frmWOMEdit #AltId').val('');
         //$('#frmWOMEdit #TotalDozens').data("kendoNumericTextBox").value(null)
         $('#frmWOMEdit #QtyEach').val('');
         $('#frmWOMEdit #MC').val('');
         $('#frmWOMEdit #CylinderSizes').val('');
         $('#frmWOMEdit #PFSInd').uncheck();
        // $('#frmWOMEdit #StartDate').val('');
         $('#frmWOMEdit #CurrDueDate').val('');
         $('#frmWOMEdit #TxtPath').val('');
         $('#frmWOMEdit #CutPath').val('');
         $('#frmWOMEdit #SewPath').val('');
         $('#frmWOMEdit #Atr').val('');
         $('#frmWOMEdit #MfgPathId').val('');
         $('#frmWOMEdit #MFGPlant').val('');
         $('#frmWOMEdit #DcLoc').val('');
         $('#frmWOMEdit #ExpeditePriority').data("kendoNumericTextBox").value(0);
         $('#frmWOMEdit #CategoryCode').val('');
         $('#frmWOMEdit #PackCode').val('');       
          
         $('#frmWOMEdit input[id="CreateBd"]').prop('checked', false)
         $('#frmWOMEdit #LbsStr').val('');
         $('#frmWOMEdit #MC').val('');
         $('#frmWOMEdit #Note').val('');
         return false;
    },

    EnableFields:function(stat){
        $('#frmWOMEdit :input[type!="hidden"]').not('#OrderStatus,input:disabled').prop('readonly', !stat);
    },
    saveEdit: function () {
        if (WOM.validateEdit()) {
            var data = WOM.const.currentRow;
            data.IsEdited = true;
            WOM.getEditedData(data);
            var sel = gridWOM.select().index();
            
            if (sel >= 0) {
                sel++;
               var cr= gridWOM.lockedTable.find('tr:nth-child('+sel+')').addClass('k-state-selected') ;
               gridWOM.table.find('tr:nth-child(' + sel + ')').addClass('k-state-selected');
               // ISS.common.scrollTo(cr)
            }
            WOM.cancelEdit();
            gridWOM.refresh();
        }
        return false;
    },

    validateEdit: function () {
        if (WOM.const.validatorEdit.validate()) {
            //var tt = $('#frmWOMEdit #TotalDozens').data("kendoNumericTextBox").value()
            //if (tt==null || tt <= 0) {
            //    ISS.common.showPopUpMessage('Order quantity must be greater than 0.',null,function(){
            //    $("#frmWOMEdit #TotalDozens").prev().focus()
            //});
            //    return false;
            //}
            return true;
        }
        return false;
    },

    getAllMSKU: function (id,superOrder) {
        var filter = ISS.common.getFilter('and')
        filter.filters.push(ISS.common.getFilterItem('GroupId', 'eq', id))
        filter.filters.push(ISS.common.getFilterItem('SuperOrder', "neq", superOrder))
        var query = new kendo.data.Query(gridWOM.dataSource.data());
        var data = query.filter(filter).data;       
        return data;
    },
    getEditedData: function (order) {
        var MSKU = (order.GroupId != null);
        var data = null;
        if (MSKU) {
            data = {};
        }
        else {
            data = order;
        }
        data.OrderStatus = $("#frmWOMEdit #OrderStatus").data("kendoDropDownList").value();

        //datagridMode= $("#frmWOMEdit #gridMode").val();
        data.AssortCode= $("#frmWOMEdit #AssortCode").val();
        data.PrimaryDC= $("#frmWOMEdit #PrimaryDC").val();
        data.OrderDetailId= $("#frmWOMEdit #OrderDetailId").val();
        data.OriginTypeCode= $("#frmWOMEdit #OriginTypeCode").val();
        //  .RoutingId= $("#frmWOMEdit #RoutingId").val();
            
        data.BusinessUnit= $("#frmWOMEdit #BusinessUnit").val();
     
        data.Style= $("#frmWOMEdit #Style").val();
        //var clr=$("#frmWOMEdit #Color").data("kendoMultiSelect").value();
        //if(clr.length >0)
        //    data.Color =clr[0];
        data.Color = $("#frmWOMEdit #Color").data("kendoComboBox").value();
        data.Attribute= $("#frmWOMEdit #Attribute").data("kendoComboBox").text();        
        data.Revision= $("#frmWOMEdit #Revision").val();
        //data.AltId= $("#frmWOMEdit #AltId").val();
        data.MC= $("#frmWOMEdit #MC").val();
        //Cy.linderSizes= $("#frmWOMEdit #CylinderSizes").val();
        //data.StartDate= $("#frmWOMEdit #StartDate").data("kendoDatePicker").value();
        data.CurrDueDate = $("#frmWOMEdit #CurrDueDate").data("kendoDatePicker").value();

       // WOM.resetDateOffset(data.StartDate)
        WOM.resetDateOffset(data.CurrDueDate)

        //data.TxtPath= $("#frmWOMEdit #TxtPath").val();
        //data.CutPath= $("#frmWOMEdit #CutPath").val();
        //data.SewPath= $("#frmWOMEdit #SewPath").val(); 
        data.Atr= $("#frmWOMEdit #Atr").val();
        data.MfgPathId = $("#frmWOMEdit #MfgPathId").val();
        data.MFGPlant = $("#frmWOMEdit #MFGPlant").val();
        data.DcLoc= $("#frmWOMEdit #DcLoc").val();
        data.ExpeditePriority = $("#frmWOMEdit #ExpeditePriority").data("kendoNumericTextBox").value();
        data.CategoryCode= $("#frmWOMEdit #CategoryCode").val();
        //data.PackCode = $("#frmWOMEdit #PackCode").val();
        var rdval = $('#frmWOMEdit #CreateBd:checked').val();
        data.DozensOnly = false; data.CreateBd = false;
        if (rdval == 'CreateBd') { data.CreateBd = true; }
        else if (rdval == 'DozensOnly') { data.DozensOnly = true;  }    
        data.BOMUpdate= $('#frmWOMEdit #BOMUpdate[type="checkbox"]').prop('checked');

        data.Note = $("#frmWOMEdit #Note").val();
        if (!ISS.common.equals(order.Cloned.Note, order.Note)) { order.NoteInd = 'Y'; }
        //data.TotalDozens = $('#frmWOMEdit #TotalDozens').data("kendoNumericTextBox").value();
        data.QtyEach = $('#frmWOMEdit #QtyEach').val();

        var size = $("#frmWOMEdit #Size").data("kendoComboBox");
        //if (size.value().length >0)
        //    data.Size = size.value()[0];
        //if (size.dataItems().length>0)
        //    data.SizeShortDes = size.dataItems()[0].SizeDesc;
        data.Size = size.value();
        data.SizeShortDes = size.text();

        if (MSKU) {
            order.Note = data.Note;
            //order.TotalDozens = data.TotalDozens;
            order.QtyEach = data.QtyEach;
            order.Size = data.Size;
            order.SizeShortDes = data.SizeShortDes;
            order.IsFieldChange = true;
            WOM.updateMSKUOrder(order, data);
        }
        else {
            WOM.setOrderType(data);
            data.IsFieldChange = true;
        }
        return data;
    },

    getEditedDataInline: function (data) {
        order = data.Cloned;
        //var MSKU = (order.GroupId != null);
        if (order) {
            var MSKU = (order.GroupId != null);
        }
        //var data = null;
        //if (MSKU) {
        //    data =  WOM.const.currentRow;
        //}
        //else {
        //    data = order;
        //}
        //data.OrderStatus = $("#frmWOMEdit #OrderStatus").data("kendoDropDownList").value();

        //datagridMode= $("#frmWOMEdit #gridMode").val();

        //WOM.resetDateOffset(data.StartDate)
        //WOM.resetDateOffset(data.CurrDueDate)

        //if (!ISS.common.equals(order.Cloned.Note, order.Note)) { order.NoteInd = 'Y'; }

        if (MSKU) {
            order.Note = data.Note;
            //order.TotalDozens = data.TotalDozens;
            order.QtyDZ = data.QtyDZ;
            order.Size = data.Size;
            order.SizeShortDes = data.SizeShortDes;
            order.IsFieldChange = true;
            WOM.updateMSKUOrder(order, data);
        }
        else {
            WOM.setOrderType(data);
            data.IsFieldChange = true;
        }
        return data;
    },

    resetDateOffset: function (d) {
        if (d.getTimezoneOffset()<0)
            d.setTime(d.getTime() - d.getTimezoneOffset() * 60 * 1000)
    },
    updateMSKUOrder: function (order, data) {
        var list = WOM.getAllMSKU(order.GroupId, order.SuperOrder);
        if (data.OrderStatus != order.OrderStatus && data.OrderStatus == 'S') {
            ISS.common.showPopUpMessage('Changing Status to Suggested will ungroup orders.','Information');
        }
        for (var i = 0; i < list.length; i++) {
            var mOrder = list[i];
            ISS.common.cloneAndStore(mOrder);
            var flag = false;
            //TBD IsDeleted Check
            if (!ISS.common.equals(data.AssortCode , order.AssortCode)) { mOrder.AssortCode = data.AssortCode; flag = true; }
           // if (!ISS.common.equals(data.PrimaryDC , order.PrimaryDC)) { mOrder.PrimaryDC = data.PrimaryDC; flag = true; }
           // if (!ISS.common.equals(data.OrderDetailId , order.OrderDetailId)) { mOrder.OrderDetailId = data.OrderDetailId; flag = true; }
           // if (!ISS.common.equals(data.OriginTypeCode , order.OriginTypeCode)) { mOrder.OriginTypeCode = data.OriginTypeCode; flag = true; }
                
            if (!ISS.common.equals(data.BusinessUnit , order.BusinessUnit) ){ mOrder.BusinessUnit = data.BusinessUnit; flag = true; }
                
           // if (!ISS.common.equals(data.Style , order.Style)) { mOrder.Style = data.Style; flag = true; }
           // if (!ISS.common.equals(data.Color , order.Color) ){ mOrder.Color = data.Color; flag = true; }
            if (!ISS.common.equals(data.Attribute , order.Attribute)) { mOrder.Attribute = data.Attribute; flag = true; }
                
           // if (!ISS.common.equals(data.Revision , order.Revision)) { mOrder.Revision = data.Revision; flag = true; }
            //if (!ISS.common.equals(data.AltId , order.AltId) ){ mOrder.AltId = data.AltId; flag = true; }
           // if (!ISS.common.equals(data.MC , order.MC) ){ mOrder.MC = data.MC; flag = true; }
            //if (!ISS.common.equals(data.StartDate, order.StartDate)) { mOrder.StartDate = data.StartDate; flag = true; }
            if (!ISS.common.equals(data.CurrDueDate, order.CurrDueDate)) { mOrder.CurrDueDate = data.CurrDueDate; flag = true; }
            //if (!ISS.common.equals(data.TxtPath , order.TxtPath)) { mOrder.TxtPath = data.TxtPath; flag = true; }
            //if (!ISS.common.equals(data.CutPath , order.CutPath)) { mOrder.CutPath = data.CutPath; flag = true; }
            //if (!ISS.common.equals(data.SewPath , order.SewPath)) { mOrder.SewPath = data.SewPath; flag = true; }
          //  if (!ISS.common.equals(data.Atr, order.Atr) ){ mOrder.Atr = data.Atr; }
            if (!ISS.common.equals(data.MfgPathId, order.MfgPathId)) { mOrder.MfgPathId = data.MfgPathId; flag = true; }
            if (!ISS.common.equals(data.MfgPath, order.MfgPath)) { mOrder.MfgPath = data.MfgPath; flag = true; }
            if (!ISS.common.equals(data.DcLoc , order.DcLoc) ){ mOrder.DcLoc = data.DcLoc; flag = true; }
          //  if (!ISS.common.equals(data.ExpeditePriority, order.ExpeditePriority)) { mOrder.ExpeditePriority = data.ExpeditePriority; flag = true; }
           // if (!ISS.common.equals(data.CategoryCode , order.CategoryCode) ){ mOrder.CategoryCode = data.CategoryCode; flag = true; }
            //if (!ISS.common.equals(data.PackCode , order.PackCode)) { mOrder.PackCode = data.PackCode; flag = true; }
           // if (!ISS.common.equals(data.CreateBd , order.CreateBd)) { mOrder.CreateBd = data.CreateBd; flag = true; }
          //  if (!ISS.common.equals(data.DozensOnly , order.DozensOnly) ){ mOrder.DozensOnly = data.DozensOnly; flag = true; }
          //  if (!ISS.common.equals(data.BOMUpdate, order.BOMUpdate)) { mOrder.BOMUpdate = data.BOMUpdate; flag = true; }

            if (data.OrderStatus != order.OrderStatus) {
                flag = true;               
                var stat = mOrder.OrderStatus;
                mOrder.OrderStatus = data.OrderStatus;
                WOM.ValidateProdStatus(mOrder, stat);
                WOM.setOrderType(mOrder);                 
            }
            if (flag) {               
                mOrder.IsEdited = true;
                mOrder.IsFieldChange = true;
                //mOrder.ErrorStatus = true;
            }
            else {
                break;
            }
        }

       // order.AssortCode = data.AssortCode;
       // order.PrimaryDC = data.PrimaryDC;
       // order.OrderDetailId = data.OrderDetailId;
       // order.OriginTypeCode = data.OriginTypeCode;
       // order.BusinessUnit = data.BusinessUnit;
       // order.Style = data.Style;
       // order.Color = data.Color;
       // order.Attribute = data.Attribute;
       // order.Revision = data.Revision;
       // //order.AltId = data.AltId;
       // order.MC = data.MC;
       //// order.StartDate = data.StartDate;
       // order.CurrDueDate = data.CurrDueDate;
       // order.TxtPath = data.TxtPath;
       // order.CutPath = data.CutPath;
       // order.SewPath = data.SewPath;
       // order.Atr = data.Atr;
       // order.MfgPathId = data.MfgPathId;
       // order.MFGPlant = data.MFGPlant;
       // order.DcLoc = data.DcLoc;
       // order.ExpeditePriority = data.ExpeditePriority;
       // order.CategoryCode = data.CategoryCode;
       // //order.PackCode = data.PackCode;
       // order.CreateBd = data.CreateBd;
       // order.DozensOnly = data.DozensOnly;
       // order.BOMUpdate = data.BOMUpdate;
       // if (data.OrderStatus != order.OrderStatus) {
       //     order.OrderStatus = data.OrderStatus;
       //     WOM.setOrderType(order);
       //     if (order.OrderStatus == 'R') {
       //         WOM.doSummarize(order.GroupId, false, true);
       //     }
       // }

    },
   
    setOrderType: function (order) {
        if (order.OrderStatus == 'L') {
            order.OrderType = 'WO';
            order.OrderStatusDesc = WOM.const.OrderStatus.Locked;

        } else if (order.OrderStatus == 'R') {
            order.OrderType = 'WO';
            order.OrderStatusDesc = WOM.const.OrderStatus.Released;
            if (order.OrderRef == null || order.OrderRef == '') order.OrderRef = order.DemandSource;
            else if (order.OrderRef.search(order.DemandSource) == -1) {
                order.OrderRef += (',' + order.DemandSource);
                }
        }
        else if (order.OrderStatus == 'S') {
            order.OrderType = 'SW';
            order.GroupId = null;
            order.OrderStatusDesc = WOM.const.OrderStatus.Suggested;
        }
    },

    clearMassChange : function () {
        
            $("#frmWOMMassChange #MfgPathId").val('')
            $("#frmWOMMassChange #CutPath").val('')
            $("#frmWOMMassChange #Txtpath").val('')
            $("#frmWOMMassChange #DC").val('')
            $("#frmWOMMassChange #QtyEach").val('')
            $("#frmWOMMassChange #OrderStatusMC").data("kendoDropDownList").value('')
            $("#frmWOMMassChange #Priority").val()
            $("#frmWOMMassChange #CatCD").val('')
            //$("#frmWOMMassChange #DueDateStr").data("kendoDropDownList").value('Start')
            $("#frmWOMMassChange #MDueDate").data("kendoDatePicker").value(null)
            $("#frmWOMMassChange #MColor").val('')
            $("#frmWOMMassChange #MAttribute").val('')
            $("#frmWOMMassChange #AltId").val('')
            $("#frmWOMMassChange #Rev").val('')
            //$("#frmWOMMassChange #TextilePlant").data("kendoComboBox").value('')
            //$("#frmWOMMassChange #MachineMC").data("kendoComboBox").value('')
            $("#frmWOMMassChange #Limit").val('')
            $("#frmWOMMassChange #FitToMachine").prop('checked', false)
            $("#frmWOMMassChange #BOMUpdate").prop('checked', false)
            $('#frmWOMMassChange #CreateBd').prop('checked',false); 
    },

    getMassChangeData: function () {
        var data = {
            MfgPathId: $("#frmWOMMassChange #MfgPathId").val(),
            CutPath: $("#frmWOMMassChange #CutPath").val(),
            TxtPath: $("#frmWOMMassChange #Txtpath").val(),
            DcLoc: $("#frmWOMMassChange #DC").val(),
            TotalDozens: $("#frmWOMMassChange #QtyEach").val(),
            OrderStatus: $("#frmWOMMassChange #OrderStatusMC").data("kendoDropDownList").value(),
            ExpeditePriority: $("#frmWOMMassChange #Priority").val(),
            CategoryCode: $("#frmWOMMassChange #CatCD").val(),
           // DueDateStr: $("#frmWOMMassChange #DueDateStr").data("kendoDropDownList").value(),
            DueDate: $("#frmWOMMassChange #MDueDate").data("kendoDatePicker").value(),
            Color: $("#frmWOMMassChange #MColor").val(),
            Attribute: $("#frmWOMMassChange #MAttribute").val(),
            //AltId: $("#frmWOMMassChange #AltId").val(),
            Revision: $("#frmWOMMassChange #Rev").val(),
           // TextilePlant: $("#frmWOMMassChange #TextilePlant").data("kendoComboBox").value(),//TxtPath
            //MC: $("#frmWOMMassChange #MachineMC").data("kendoComboBox").value().toUpperCase(),
            Limit: $("#frmWOMMassChange #Limit").val(),
            FitToMachine: $("#frmWOMMassChange #FitToMachine").prop('checked'),
            BOMUpdate: $("#frmWOMMassChange #BOMUpdate").prop('checked'),           
        };
        

        //TxtPath
        
      
        // DozensOnlyOff || CreateBd || DozensOnly   Skipped here.

        return data;
    },
    massChangeClick: function () {
        var arr = new Array();
        var MSKUIDList = new Array();
        var MSKUFlag = false;
        if (!WOM.const.validatorMass.validateInput($("#frmWOMMassChange #DC"))) {
            return false
        }
        if (!WOM.const.validatorMass.validateInput($("#frmWOMMassChange #Priority"))) {
            return false
        }
       
        var rows = $(WOM.selectedRows()).not('.deleted-row');
        if (rows.length > 0) {
            for (i = 0; i < rows.length; i++) {
                var dataItem = gridWOM.dataItem(rows[i]);
                if (WOM.IsAllowEdit(dataItem)) {
                    arr.push(dataItem);
                    //dataItem.loadedMSKU = false;
                    if (dataItem.GroupId != null && $.inArray(dataItem.GroupId, MSKUIDList) == -1) {
                        var mList = WOM.getAllMSKU(dataItem.GroupId, dataItem.SuperOrder);
                        $.merge(arr, mList);
                        MSKUIDList.push(dataItem.GroupId)
                        MSKUFlag = true;
                    }
                }
            }
        }
        else {
            ISS.common.showPopUpMessage('Please select at least one record.');
            return false;
        }
       
      
        var data = WOM.getMassChangeData();
        
        var prop = WOM.getNotNullProps(data, ['OrderStatus', 'DueDate', 'DozensOnlyOff', 'Limit', 'FitToMachine']);
        var DueDate=null;
        if(! ISS.common.isNull(data.DueDate) && WOM.validate60daysDate(data.DueDate)){
            DueDate = data.DueDate;
            WOM.resetDateOffset(DueDate);

        }
        var IsReleased = false;
        var ReleasedGID = new Array();

        if (MSKUFlag && !ISS.common.isNull(data.OrderStatus) ){
            if (data.OrderStatus == 'S') {
                ISS.common.showPopUpMessage('Changing Status to Suggested will ungroup orders.', 'Information');
            } else if (data.OrderStatus == 'R') {
                IsReleased = true;
            }
        }

        // Update records start

        var rdval = $('#frmWOMMassChange #CreateBd:checked').val();
        DozensOnly = false; CreateBd = false; DozensOnlyOff = false;
        if (rdval == 'CreateBd') { CreateBd = true; }
        else if (rdval == 'DozensOnly') { DozensOnly = true; }
        else if (rdval == 'DozensOnlyOff') { DozensOnlyOff = true; }

        for (i = 0; i < arr.length; i++) {
            var item = arr[i];
            ISS.common.cloneAndStore(item);
            if (IsReleased) {
                if (item.Cloned.OrderStatus!='R' &&  item.GroupId != null && $.inArray(item.GroupId, ReleasedGID) == -1) {
                    ReleasedGID.push(item.GroupId);
                }
            }
            if (prop.length > 0) {
                item.IsEdited = true;
                item.IsFieldChange = true;
                for (j = 0; j < prop.length; j++) {
                    item[prop[j]] = data[prop[j]];
                }
            }
            if (data.TotalDozens != null && data.TotalDozens != '') {
                item.QtyEach = data.TotalDozens;
            }  
            if(DueDate)
                item.CurrDueDate = DueDate;
            if (DueDate || DozensOnlyOff || CreateBd || DozensOnly) { item.IsEdited = true; item.IsFieldChange = true; }

            if (DozensOnlyOff) {
                item.CreateBd = false;
                item.DozensOnly = false;
            }
            else if (CreateBd) {
                item.CreateBd = true;
                item.DozensOnly = false;
            }
            else if (DozensOnly) {
                item.CreateBd = false;
                item.DozensOnly = true;
            }

            if (!ISS.common.isNull(data.OrderStatus) && data.OrderStatus != item.OrderStatus) {
                var stat = item.OrderStatus;
                item.OrderStatus = data.OrderStatus;
                WOM.ValidateProdStatus(item, stat);
                WOM.setOrderType(item);
                item.IsEdited = true;
                item.IsFieldChange = true;
            }
        }
        //TBD 'Limit', 'FitToMachine','Machine'
        if (ReleasedGID.length > 0) {
            for (t = 0; t < ReleasedGID.length; t++) {
                WOM.doSummarize(ReleasedGID[t],  false,true);
            }
        }
        gridWOM.refresh();
        WOM.clearMassChange();
        WOM.onRowsSelected();
        return false;
    },

    getNotNullProps: function (data,exclude) {
        var arr =new Array();
        if (data) {
            for (prop in data) {
                if (!ISS.common.isNull(data[prop]) && $.inArray(prop, exclude)==-1) {
                    arr.push(prop);
                }
            }
        }
        return arr;
    },

    validateTXTPath: function (ev) {
        var e = this;
        if (e.value != null && e.value != '') {
            WOM.validatePlant(e.value, function (r) {
                if (!r) {
                    e.value = '';
                    ISS.common.showPopUpMessage(
                        "Textile Plant is Invalid.  The Finishing Indicator must be set to a 'Y' in the APS   Plant table. It is currently set to an 'N'.  Either select a valid Textile Plant    or contact the Cost Dept if you believe the plant you selected is a valid Textile Plant.", null, function () {
                            e.focus();
                        });
                }
            });
        }
    },

    showSewPath: function (e) {
        var gridData = gridWOM.dataSource.view();
        if (gridData.length > 0) {
            var wod = null;
            var selectedItem = gridWOM.dataItem(gridWOM.select());
            if (selectedItem == null) {
                var d = WOM.getCurrentRowBySize();
                if (d != null) {
                    var szLst = {}
                    wod = { Style: d.Style, ColorCode: d.Color, Attribute: d.Attribute, Size: d.Size, PrimaryDC: d.DcLoc, SellingStyle: d.Style }
                    wod.SizeList = new Array();
                    wod.SizeList.push({ SizeCD: d.Size })
                }
            }
            else {
                wod = { Style: selectedItem.Style, ColorCode: selectedItem.Color, Attribute: selectedItem.Attribute, Size: selectedItem.Size, PrimaryDC: selectedItem.DcLoc, SellingStyle: selectedItem.Style }
                wod.SizeList = new Array();
                wod.SizeList.push({ SizeCD: selectedItem.Size })
            }
            //res.Plant = $('#frmWOMEdit #CutPath').val();
            settings = {
                columns: [{
                    Name: "MfgPathId",
                    Title: "Path Id",
                },
                {
                    Name: "SewPlt",
                    Title: "Sew Plt",
                }],
                AllowSelect: true,
                title: 'Select a Sew Plant',
                url: WOM.const.urlGetMfgPath,
                postData: wod,
                close: function () {
                    $('#frmWOMMassChange #btnSewPathSearch').focus();
                    return false;
                },
                handler: function (d) {
                    $('#frmWOMMassChange #MfgPathId').val(d.MfgPathId).focus();
                    return false;
                }
            };
            ISS.common.CommonSearchShow(settings);
        }
        else {
            ISS.common.showPopUpMessage("Please select an Order first.");
        }
        return false;
    },

    getCurrentRowBySize: function () {
        
        var currentRow = null;
        var dsSort = [];
        dsSort.push({ field: "Size", dir: "asc" });
        gridWOM.dataSource.sort(dsSort);
        var gridData = gridWOM.dataSource.view();
        //var s = gridData.sort(dsSort);
        if (gridData.length > 0) {
            currentRow = gridData[0];
        }
        return currentRow;
    },

    showCutPath: function () {
       
        var gridData = gridWOM.dataSource.view();
        if (gridData.length > 0) {
            //var dsSort = [];
            //dsSort.push({ field: "SuperOrder", dir: "asc" });
            //gridWOM.dataSource.sort(dsSort);
            //var gridData1 = gridWOM.dataSource.view();
            var wod = null;
            var selectedItem = gridWOM.dataItem(gridWOM.select());
            if (selectedItem == null) {
                var d = gridData[0];
                if (d != null) {
                    var szLst = {}
                    wod = { SuperOrder: d.SuperOrder, DyeCode: "C", CutPath: d.CutPath }
                }
            }
            else {
                wod = { SuperOrder: selectedItem.SuperOrder, DyeCode: "C", CutPath: selectedItem.CutPath }
            }
            
            settings = {
                columns: [{
                    Name: "Source_Plant",
                    Title: "Path Id",
                },
                {
                    Name: "Source_Plant",
                    Title: "Sew Plt",
                }],
                AllowSelect: true,
                title: 'Select a Cut Plant',
                url: WOM.const.urlPopulateCutPathTxtPath,
                postData: wod,
                handler: function (d) {
                    $('#frmWOMMassChange #CutPath').val(d.Source_Plant).focus();
                    return false;
                }
            };
            ISS.common.CommonSearchShow(settings);
        }
        else {
            ISS.common.showPopUpMessage("Please select an Order first.");
        }
        return false;
    },

    showMfgPathId: function (e) {

        var ColorCode = $('#frmWOMEdit #Color').data("kendoComboBox").value();
        
        var Attribute = $('#frmWOMEdit #Attribute').data("kendoComboBox").text();
        var Size = $('#frmWOMEdit #Size').data("kendoComboBox").value();
        var SizeList = new Array();
        SizeList.push({ SizeCD: Size })
       
        var result = { SellingStyle: $('#frmWOMEdit #Style').val() }
        var demLoc = $("#frmWOMEdit #PrimaryDC").val();
        result.ColorCode = ColorCode;
        result.Attribute = Attribute;
        result.SizeList = JSON.parse(JSON.stringify(SizeList));
        result.PrimaryDC = demLoc;
        //var SizeList = WO.const.SKUSizeList;
        if ($('#frmWOMEdit #Style').val() != '' && ColorCode != '' && Attribute != '' && Size != '') {
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
                url: WOM.const.urlMfgPath,
                postData: result,
                close: function () {
                    setTimeout(function () {
                        $('#frmWOMEdit #MfgPathId').focus();
                    }, 0)
                    return false;
                },
                handler: function (d) {
                    $('#frmWOMEdit #MfgPathId').val(d.MfgPathId);
                    $('#frmWOMEdit #MFGPlant').val(d.SewPlt);
                    $('#frmWOMEdit #MfgPathId').focus();
                }
            };
            ISS.common.CommonSearchShow(settings);

        }
        else {
            ISS.common.showPopUpMessage('Please enter Style, Color, Attribute and Size to generate Mfg Path.');

        }
        return false;


    },

    SellingSValidate: function () {
        var postdata = { SellingStyle: $('#grdWOM #Style').val() };
        sstyle = $('#grdWOM #Style').val();
        ISS.common.executeActionAsynchronous(WOM.const.urlGetWOAsrtCode, JSON.stringify(postdata), function (stat, data) {
            if (stat) {
                if (data) {
                    if (data.length == 0) {
                        sserror = true;
                        ISS.common.notify.error('Please provide a valid style.');
                    }
                    else {
                        sserror = false;
                    }
                }
            }

        });
    },

    FilterData: function () {
        return {
            Style_Cd: sstyle,
            Color_Cd: scolor,
            Attribute_Cd: sattribute,
            Size_Cd: sssize
        }
    },

    AttrFilterData: function () {
        return {
            wod: {
                SellingStyle: sstyle,
                ColorCode: scolor
            },
            Src: 'AO'
        }
    },

    ColorSelect: function (e) {
        var dataItem = this.dataItem(e.item.index());
        scolor = dataItem.Color;
    },

    ColorChange: function (e) {
        var datas = e.sender.dataSource.data();
        var dataItem = datas[e.sender.selectedIndex];
        scolor = dataItem.Color;
    },

    AttributeSelect: function (e) {
        var dataItem = this.dataItem(e.item.index());
        sattribute = dataItem.Attribute;
    },

    AttributeChange: function (e) {
        var datas = e.sender.dataSource.data();
        var dataItem = datas[e.sender.selectedIndex];
        sattribute = dataItem.Attribute;
    },

    SizeSelect: function (e) {
        var dataItem = this.dataItem(e.item.index());
        sssize = dataItem.Size;
        ssizecode = dataItem.SizeDesc;
    },

    SizeChange: function (e) {
        var datas = e.sender.dataSource.data();
        var dataItem = datas[e.sender.selectedIndex];
        sssize = dataItem.Size;
        ssizecode = dataItem.SizeDesc;
    },

    MFGFilterData: function () {
        return {
            SellingStyle: sstyle,
            ColorCode: scolor,
            Attribute: sattribute,
            SizeList: [{ Size: ssizecode, SizeCD: sssize }],
            PrimaryDC: PDC
        };
    },

    RevisionFilterData: function () {
        return {
            SellingStyle: ssstyle,
            ColorCode: scolor,
            Attribute: sattribute,
            SizeList: [{ Size: ssizecode, SizeCD: sssize }],
            AssortCode: "N"
        };
    },


    RevisionSelect: function (e) {
        var dataItem = this.dataItem(e.item.index());
        var rows = WOM.selectedRows();
        $(rows).each(function (ind, data) {
            if (dataItem.PKGStyle != "0") {
                var RowData = gridWOM.dataItem(data);
                data.children['8'].innerText = dataItem.PKGStyle;
                RowData.Style = dataItem.PKGStyle;
            }
            sserror = false;
        });
        srevision = dataItem.Rev;
    },

    RevisionChange: function (e) {
        var datas = e.sender.dataSource.data();
        var dataItem = datas[e.sender.selectedIndex];
        var rows = WOM.selectedRows();
        $(rows).each(function (ind, data) {
            if (dataItem.PKGStyle != "0") {
                var RowData = gridWOM.dataItem(data);
                data.children['8'].innerText = dataItem.PKGStyle;
                RowData.Style = dataItem.PKGStyle;
            }
            sserror = false;
        });
        srevision = dataItem.Rev;
    },

    StyleSearchClick: function (e) {
        WOM.loadStyleDetailsGrid();
        WOM.const.RevPopup = ISS.common.popUp('.divRevSearchPopup', 'Revision Search', null, function () {
            setTimeout(function () {
            }, 0)
        });
    },

    loadStyleDetailsGrid: function () {
        var grid = $("#grdStyleDetails").data("kendoGrid");
        if (grid.pager.page() > 1) grid.pager.page(1)
        grid.dataSource.read();
        return false;
    },

    showPStyleDetails: function (e) {
        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        var rows = WOM.selectedRows();
        $(rows).each(function (ind, data) {
            var RowData = gridWOM.dataItem(data);
            data.children['8'].innerText = dataItem.PKGStyle;
            RowData.Style = dataItem.PKGStyle;
            sserror = false;
            WOM.const.RevPopup.close();
        });
    },

    OrderChange: function (e) {
        var datas = e.sender.dataSource.data();
        var dataItem = datas[e.sender.selectedIndex];
        var rows = WOM.selectedRows();
        $(rows).each(function (ind, data) {
            var RowData = gridWOM.dataItem(data);
            RowData.OrderStatus = dataItem.Value;
        });
    },

    MfgPathChange: function (e) {
        var datas = e.sender.dataSource.data();
        var dataItem = datas[e.sender.selectedIndex];
        var rows = WOM.selectedRows();
        $(rows).each(function (ind, data) {
            var RowData = gridWOM.dataItem(data);
            //$(gridWOM.table.find("tr[data-uid='" + RowData.uid + "'] td")).each(function (i, d) {
            //    alert(i);
            //});
            gridWOM.table.find("tr[data-uid='" + RowData.uid + "'] td:eq(2)")[0].innerText = dataItem.SewPlt;
            gridWOM.table.find("tr[data-uid='" + RowData.uid + "'] td:eq(2)")[0].className = 'k-dirty-cell';
            $(gridWOM.table.find("tr[data-uid='" + RowData.uid + "'] td:eq(2)")[0]).append('<span class="k-dirty" style="margin-left: -19px;"></span>');
            RowData.SewPath = dataItem.SewPlt;
            RowData.Atr = dataItem.SewPlt;
            RowData.MfgPathId = dataItem.MfgPathId;
        });
    }

}

$.extend(WOM, tWOM);
 