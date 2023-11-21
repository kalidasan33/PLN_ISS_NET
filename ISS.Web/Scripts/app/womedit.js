

tWOM = {


    docWOMReady2: function (IsLoad) {
        window.sserror = false;
        WOM.const.validatorEdit = $("#frmWOMEdit").kendoValidator().data("kendoValidator")
        $('#frmWOMEdit #Style').bind('focusin', WOM.StyleChangedFocusIn);
        $('#frmWOMEdit #Style').bind('focusout',WOM.StyleChanged);
        $('#frmWOMEdit #Revision').bind('click', WOM.revSearchClick);
        $('#frmWOMEdit #MfgPathId').bind('click', WOM.mfgPathClick);
        $('#frmWOMEdit #PackCode').bind('click', WOM.PackCodeClick);
        $('#frmWOMEdit #CategoryCode').bind('click', WOM.CatCodeClick);
       
        $('#frmWOMEdit #MC').bind('click', WOM.showMachineCodes);
        $('#frmWOMEdit #AltId').bind('click', WOM.showAlternateId);
        $('#frmWOMEdit #CutPath').bind('click', function () {
            WOM.PopulateCutPathTxtPath('CutPath');
            return false;
        });
        
        $("#frmWOMEdit #CategoryCode").focusin(function () {
            WOM.const.CategoryCode = $('#frmWOMEdit #CategoryCode').val();
        });
        $("#frmWOMEdit #CategoryCode").bind('focusout', WOM.validateCategoryCodeWOM);
       
        $('#frmWOMEdit #TxtPath').bind('click', function () {
            WOM.PopulateCutPathTxtPath('TxtPath');
            return false;
        });
        $('#frmDemandId #LineItem').on('keypress', function (e) {
            WOM.isNumber(e);
        });
       
        $('#frmWOMEdit #Revision').keypress(function (e) {
            if (e.which == 13) {
                WOM.revSearchClick();
                $('#frmWOMEdit #AltId').focus();
                return false;
            }
        });
        $('#frmWOMEdit #MfgPathId').keypress(function (e) {
            if (e.which == 13) {
                WOM.mfgPathClick();
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


        $("#frmWOMMassChange #CatCD").focusin(function () {
            WOM.const.CategoryCode = $('#frmWOMMassChange #CatCD').val();
        });
        $("#frmWOMMassChange #CatCD").bind('focusout', WOM.validateCategoryCodeWOM);



        $('#frmWOMMassChange #btnCatSearch').bind('click', function () {            
            WOM.CatCodeClick('CatCD');
            return false;
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
        $('#frmWOMMassChange #MfgPathId').keypress(function (e) {
            if (e.which == 13) {
                WOM.showSewPath();
                return false;
            }
        });

        $('#frmWOMEdit #Revision').on('focusin', function () {
            WOM.const.Revision = this.value;
           
        });
        $('#frmWOMEdit #Revision').on ('focusout', function () {
            if (WOM.const.Revision != this.value) {
                WOM.getPKGStyle();
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
        $('.EditCancel').on('keydown', function (e) {

            if (e.keyCode == 9) {
                $('#frmWOMEdit #OrderStatus').data("kendoDropDownList").focus();
                return false;
            }

        });
        
       
        $('.cancelGridPFS').on('keydown', function (e) {

            if (e.keyCode == 9) {
                var grid = $("#grdFabricDetailPFS").data("kendoGrid");
                var item = grid.current();
                var firstCell = grid.table.find("tr:first td:last");
                grid.current(firstCell);
                grid.editCell(firstCell);
                grid.table.focus();
                return false;
            }
          
        });
        
    },
    revisionStyChange: function (e) {
        if (!e.value) {
            return;
        }
        worevcd = e.value.toUpperCase();
        WOM.getPKGStyleInLin();
        return false;
    },

    SetGridDataValues: function (data) {
        data.IsEdited = true;
        data.IsFieldChange = true;
        if (data.DozensOnly == false) {
            data.CreateBd = true;
            data.DozensOnlyInd = "N";
            data.CreateBDInd = "Y";
        }
    },

    retrieveSellingData:function(){
        var result= {
            SellingStyle: WOM.const.currentRow.SellingStyle,
            ColorCode:  WOM.const.currentRow.SellingColor,
            Attribute:WOM.const.currentRow.SellingAttribute,
            Size: $("#frmWOMEdit #Size").data("kendoComboBox").value(),
        };
        result.SizeList = new Array();
        result.SizeList.push({ SizeCD: result.Size })
        return result;
    },

    getPKGStyle: function (callback) {
       
        var postData = WOM.retrieveRevData();
        postData.AssortCode = WOM.const.currentRow.AssortCode;
        postData.Revision = $("#Revision").val();
        postData.SellingStyle = $("#SellingStyle").val()
            postData = JSON.stringify(postData);
            
            ISS.common.executeActionAsynchronous("../order/GetPKGStyle", postData, function (stat, data) {
                if (stat && data) { 
                    if (data.length > 0) { 
                        $("#Style").val(data[0].PKGStyle).change();
                    }
                    else {
                        $("#Style").val($("#SellingStyle").val()).change();
                    }
                }

                else {
                    ISS.common.showPopUpMessage('Failed to Load Style.');
                }
                if (callback) callback(data.length > 0)
            });
        
    },

    getPKGStyleInLin: function (callback) {
        WOM.const.SKUSizeList = [];
        var rows = WOM.selectedRows();
        var RowData = null;
        $(rows).each(function (ind, data) {
            RowData = gridWOM.dataItem(data);
        });
        WOM.const.SKUSizeList = [{ Size: RowData.SizeShortDes, SizeCD: RowData.Size }];
        var postData = { SellingStyle: RowData.SellingStyle, ColorCode: RowData.Color, Attribute: RowData.Attribute, SizeList: WOM.const.SKUSizeList, Revision: worevcd, AssortCode: RowData.AssortCode };
        postData = JSON.stringify(postData);
        ISS.common.executeActionAsynchronous("../order/GetPKGSty", postData, function (stat, dat) {
            if (stat && dat) {
                if (dat.length > 0) {
                    var rowsin = WOM.selectedRows();
                    $(rowsin).each(function (ind, data) {
                        var RowData = gridWOM.dataItem(data);
                        data.children['7'].innerText = dat[0].PKGStyle;
                        RowData.Style = dat[0].PKGStyle;
                        data.children['11'].innerText = dat[0].NewRevision;
                        RowData.Revision = dat[0].NewRevision;
                        RowData.IsEdited = true;
                        RowData.IsFieldChange = true;
                    });
                   
                }
                else {
                    //ISS.common.showPopUpMessage('In valid.');
                }
            }

            else {
                ISS.common.showPopUpMessage('Failed to Load Style.');
            }
            if (callback) callback(data.length > 0)
            //Newly Added For Changing Revision and Stylefor same group
            if (RowData.GroupId != null) {
                RowData.Revision = worevcd;
                WOM.RevisionStyleGroupChange(RowData);
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
        var ds = $("#frmWOMEdit #Color").data("kendoComboBox");
        result.ColorCode = '';
        
        result.ColorCode=ds.value()  ;
        result.Src = "";
        return result;
    },

    retrieveSizeData: function () {
        var result =WOM.retrieveAttributeData();
        var Attribute = $("#frmWOMEdit #Attribute").data("kendoComboBox");
        result.Attribute = Attribute.text();
        return result;
    },



    retrieveRevData: function () {
        var result = WOM.retrieveSizeData();
        result.SizeList = new Array();
        var size = $("#frmWOMEdit #Size").data("kendoComboBox");
        var sizeCd = '';
        var sizeDesc = '';
         
            sizeCd = size.value();
            sizeDesc = size.text();
        
        result.SizeList.push({ SizeCD: sizeCd, Size: sizeDesc })

        //result.SizeList.push({ SizeCD: size.value() })
        result.Size = sizeCd ;
      
        return result;
    },

    StyleChangedFocusIn :function(){
        WOM.const.sellStyle = this.value;
    },
    StyleChanged: function () {
        
        if (WOM.const.sellStyle != this.value && this.value!='') {
            var ds = $("#frmWOMEdit #Color").data("kendoComboBox");
            WOM.loadColor();
            var postData = WOM.retrieveColorData();
            postData = JSON.stringify(postData);
            if ($.trim(postData.Style) != '') {
                ISS.common.executeActionAsynchronous("../order/GetWOAsrtCode", postData, function (stat, data) {
                    if (stat) {
                        if (data.length > 0) {
                            $("#AssortCode").val(data[0].AssortCode);
                            $("#PrimaryDC").val(data[0].PrimaryDC);
                            $("#frmWOMEdit #PackCode").val(data[0].PackCode).change();
                            $("#Dc").val(data[0].PrimaryDC).change();
                            $("#OriginTypeCode").val(data[0].OriginTypeCode);
                            $("#BusinessUnit").val(data[0].CorpBusUnit);
                            // WOM.updateCumulativeAndFabric();
                            ds.refresh();
                        }
                    }
                    else {
                        ISS.common.notify.error('Failed to retrieve Style details.');

                    }

                });
            }
            
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
   
  
    onColorBound: function () {
        // var attr = $('#frmWOMEdit #Attribute').data("kendoComboBox")
       // attr.dataSource.read();
    },

    onAttrBound: function () {
        WOM.loadSize();
    },


    onSizeChange: function () {
        //WOM.CreateOrder.validator.hideMessages($('#frmWO #Revision'));
        if (ISS.common.validateComboChange(this, 'Invalid size')) {
            var postData = WOM.retrieveRevData();


            postData.AssortCode = $("#AssortCode").val();//TBD
            postData = JSON.stringify(postData);

            ISS.common.executeActionAsynchronous("../order/GetMaxRevision", postData, function (stat, data) {
                if (stat) {
                    if (data.length > 0) {
                        $("#frmWOMEdit #Revision").val(data[0].Revision).change();
                        //WOM.getChildSKU(); 
                    }
                }
                else {
                    ISS.common.showPopUpMessage('Failed to retrieve Revision details.');
                }

            });
        }


    },

    revSearchClick: function () {        
        WOM.loadRevDetailsGrid();
        WOM.const.RevPopup = ISS.common.popUp('.divRevSearchPopup', 'Revision Search', null, function () {
            setTimeout(function () {
                $("#frmWOMEdit #Revision").focus();
            }, 0)          
        });
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
        ISS.common.executeActionAsynchronous('GetWOAsrtCode', postData, function (stat, data) {
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
                    ISS.common.executeActionAsynchronous('GetWOChildSKU', pData, function (st, res) {
                        if (st && res && res.length > 0) {

                            var color = $('#frmWOMEdit #Color').data("kendoComboBox");
                            color.value(data.Color);
                            $("#frmWOMEdit #Attribute").val(res[0].NewAttribute);
                            var size = $('#frmWOMEdit #Size').data("kendoComboBox");
                            size.value(res[0].NewSize);

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
        WOM.const.RevPopup = ISS.common.popUp('.divPackCodePopup', 'Pack Code', null, function () {
            setTimeout(function () {
                $("#frmWOMEdit #PackCode").focus();
            }, 0)
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
        if (WOM.const.RevPopup.close())
            setTimeout(function () {
                $("#frmWOMEdit #PackCode").focus();
            }, 0)
        $('#frmWOMEdit #PackCode').val(dataItem.PackCode).focus();
       // WOM.updateCumulativeAndFabric();
       
        return false;
    },
    CatCodeClick: function (catName) {
       
        if (catName != null && catName == "CatCD") {
            var gridData = gridWOM.dataSource.view();
            if (gridData.length > 0) {
                WOM.loadCatCodeGrid();
                WOM.const.RevPopup = ISS.common.popUp('.divCatCodePopup', 'Category Code', null, function () {
                    setTimeout(function () {
                        $("#frmWOMEdit #CategoryCode").focus();
                    }, 0)
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
            WOM.const.RevPopup = ISS.common.popUp('.divCatCodePopup', 'Category Code', null, function () {
                setTimeout(function () {
                    $("#frmWOMEdit #CategoryCode").focus();
                }, 0)
            });
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
        $('#' + WOM.const.RevPopup.settings.Src).val(dataItem.CategoryCode).focus();//'#frmWOMEdit #CategoryCode'
        //$('#frmWOMMassChange #CatCD').val(dataItem.CategoryCode);
       
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
            },
            close: function () {
                setTimeout(function () {
                    $("#frmWOMEdit #MC").focus();
                }, 0)
                return false;
            },

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
            },
            close: function () {
                setTimeout(function () {
                    $("#frmWOMEdit #AltId").focus();
                }, 0)
                return false;
            },
        };
        ISS.common.CommonSearchShow(settings);

    },


    mfgPathClick: function () {
        var ColorCode = $('#frmWOMEdit #Color').data("kendoComboBox").value();
        var Attribute = $('#frmWOMEdit #Attribute').data("kendoComboBox").text();
        var Size = $('#frmWOMEdit #Size').data("kendoComboBox").value();

        if ($('#frmWOMEdit #Style').val() != '' && ColorCode != '' && Attribute != '' && Size!='') {
            WOM.loadmfgPathDetailsGrid();
            WOM.const.RevPopup = ISS.common.popUp('.divmfgPathPopup', 'Select a Sew Plant', null, function () {
                setTimeout(function () {
                    $("#frmWOMEdit #MfgPathId").focus();
                }, 0)
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
       // WOM.CreateOrder.validator.hideMessages($('#frmWO #MfgPathId'));    
       
        return false;

    },
    TotDozChangedEdit: function () {
       
        var TotDoz = $('#frmWOMEdit #TotalDozens').data("kendoNumericTextBox").value();
        $('#frmWOMEdit #QtyDZ').val(WOM.TotDozChanged(WOM.const.currentRow, TotDoz)).change();
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

        var res = { SuperOrder: (WOM.const.currentRow)?WOM.const.currentRow.SuperOrder :null}
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
            title: (res.DyeCode=='C')? 'Select a Cut Plant':'Select a Txt Plant',
            url: WOM.const.urlPopulateCutPathTxtPath,
            postData: res,
            handler: function (d) {
                $('#'+form+' #' + Name).val(d.Source_Plant).focus();
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
            setTimeout(function(){
                $("#frmWOMEdit #ReqSearchButton").focus();
            },0)
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

    ValidateProdStatus:function(order,OldOrderStatus){
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
        ISS.common.cloneAndStore(data);
        
        // $('#frmWOMEdit #gridMode').val(data.gridMode);
        $('#frmWOMEdit #SellingStyle').val(data.SellingStyle);
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
        
         var size = $('#frmWOMEdit #Size').data("kendoComboBox");
         var arr = new Array();
         arr.push({ Size: data.Size, SizeDesc: data.SizeShortDes })
         size.dataSource.data(arr);
         size.value(data.Size);        
        
        
         $('#frmWOMEdit #Revision').val(data.Revision);
         $('#frmWOMEdit #AltId').val(data.AltId);
         $('#frmWOMEdit #TotalDozens').data("kendoNumericTextBox").value(data.TotalDozens);
         $('#frmWOMEdit #QtyDZ').val(data.QtyDZ).change();
         $('#frmWOMEdit #MC').val(data.MC);
         $('#frmWOMEdit #CylinderSizes').val(data.CylinderSizes);
         $('#frmWOMEdit #PFSInd').prop('checked', data.PFSInd);
         $('#frmWOMEdit #StartDate').data("kendoDatePicker").value(data.StartDate);
         $('#frmWOMEdit #CurrDueDate').data("kendoDatePicker").value(data.CurrDueDate);
         $('#frmWOMEdit #TxtPath').val(data.TxtPath);
         $('#frmWOMEdit #CutPath').val(data.CutPath);
         $('#frmWOMEdit #SewPath').val(data.SewPath);
         $('#frmWOMEdit #Atr').val(data.Atr);
         $('#frmWOMEdit #MfgPathId').val(data.MfgPathId);
         $('#frmWOMEdit #DcLoc').val(data.DcLoc);
         $('#frmWOMEdit #ExpeditePriority').val(data.ExpeditePriority);
         $('#frmWOMEdit #CategoryCode').val(data.CategoryCode);
         $('#frmWOMEdit #PackCode').val(data.PackCode);
         $('#frmWOMEdit #LbsStr').val(data.LbsStr);
         $('#frmWOMEdit #MC').val(data.MC);

         $('#frmWOMEdit #CreateBd[value="CreateBd"]').prop('checked', data.CreateBd)
         $('#frmWOMEdit #CreateBd[value="DozensOnly"]').prop('checked', data.DozensOnly)
                
         $('#frmWOMEdit #BOMUpdate[type="checkbox"]').prop('checked', data.BOMUpdate);

         
       
         WOM.const.validatorEdit.hideMessages();
         $("#frmWOMEdit #Note").val(data.Note);
         if (data.NoteInd == 'N') {
             var superOrder = { superOrder: data.SuperOrder }
             superOrder = JSON.stringify(superOrder);
             ISS.common.executeActionAsynchronous("../Order/GetNote", superOrder, function (stat, data) {
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
    canceldmdEdit: function (e, flg) {
        if (!flg) WOM.const.DmdPopup.close();
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
        
         var style = $('#frmWOMEdit #Color').data("kendoComboBox");
         style.value('');
         style.dataSource.read();

         var attr = $('#frmWOMEdit #Attribute').data("kendoComboBox");
         attr.value('');       

         var size = $('#frmWOMEdit #Size').data("kendoComboBox");
         size.value('');

         $('#frmWOMEdit #Revision').val('');
         $('#frmWOMEdit #AltId').val('');
         $('#frmWOMEdit #TotalDozens').data("kendoNumericTextBox").value(null)
         $('#frmWOMEdit #QtyDZ').val('');
         $('#frmWOMEdit #MC').val('');
         $('#frmWOMEdit #CylinderSizes').val('');
         $('#frmWOMEdit #PFSInd').uncheck();
         $('#frmWOMEdit #StartDate').val('');
         $('#frmWOMEdit #CurrDueDate').val('');
         $('#frmWOMEdit #TxtPath').val('');
         $('#frmWOMEdit #CutPath').val('');
         $('#frmWOMEdit #SewPath').val('');
         $('#frmWOMEdit #Atr').val('');
         $('#frmWOMEdit #MfgPathId').val('');
         $('#frmWOMEdit #DcLoc').val('');
         $('#frmWOMEdit #ExpeditePriority').val('');
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
            gridWOM.refresh();
            if (sel >= 0) {
                sel++;
               var cr= gridWOM.lockedTable.find('tr:nth-child('+sel+')').addClass('k-state-selected') ;
               gridWOM.table.find('tr:nth-child(' + sel + ')').addClass('k-state-selected');
               // ISS.common.scrollTo(cr)
            }
            WOM.cancelEdit();
        }
        return false;
    },

    validateEdit: function () {
        if (WOM.const.validatorEdit.validate()) {
            var tt = $('#frmWOMEdit #TotalDozens').data("kendoNumericTextBox").value()
            if (tt==null || tt <= 0) {
                ISS.common.showPopUpMessage('Order quantity must be greater than 0.',null,function(){
                $("#frmWOMEdit #TotalDozens").prev().focus()
            });
                return false;
            }
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
        data.Color = $("#frmWOMEdit #Color").data("kendoComboBox").value();
        
        data.Attribute = $("#frmWOMEdit #Attribute").data("kendoComboBox").text();
        data.Revision= $("#frmWOMEdit #Revision").val();
        data.AltId= $("#frmWOMEdit #AltId").val();
        data.MC= $("#frmWOMEdit #MC").val();
        //Cy.linderSizes= $("#frmWOMEdit #CylinderSizes").val();
        data.StartDate= $("#frmWOMEdit #StartDate").data("kendoDatePicker").value();
        data.CurrDueDate = $("#frmWOMEdit #CurrDueDate").data("kendoDatePicker").value();

        WOM.resetDateOffset(data.StartDate)
        WOM.resetDateOffset(data.CurrDueDate)

        data.TxtPath= $("#frmWOMEdit #TxtPath").val();
        data.CutPath= $("#frmWOMEdit #CutPath").val();
        data.SewPath= $("#frmWOMEdit #SewPath").val(); 
        data.Atr= $("#frmWOMEdit #Atr").val();
        data.MfgPathId= $("#frmWOMEdit #MfgPathId").val();
        data.DcLoc= $("#frmWOMEdit #DcLoc").val();
        data.ExpeditePriority = $("#frmWOMEdit #ExpeditePriority").val();
        data.CategoryCode= $("#frmWOMEdit #CategoryCode").val();
        data.PackCode = $("#frmWOMEdit #PackCode").val();
        var rdval = $('#frmWOMEdit #CreateBd:checked').val();
        data.DozensOnly = false; data.CreateBd = false;
        if (rdval == 'CreateBd') { data.CreateBd = true; }
        else if (rdval == 'DozensOnly') { data.DozensOnly = true;  }    
        data.BOMUpdate= $('#frmWOMEdit #BOMUpdate[type="checkbox"]').prop('checked');

        data.Note = $("#frmWOMEdit #Note").val();
        if (!ISS.common.equals(order.Cloned.Note, order.Note)) { order.NoteInd = 'Y'; }
        data.TotalDozens = $('#frmWOMEdit #TotalDozens').data("kendoNumericTextBox").value();
        data.QtyDZ = $('#frmWOMEdit #QtyDZ').val();
        var size = $("#frmWOMEdit #Size").data("kendoComboBox");
            data.Size = size.value() ;        
            data.SizeShortDes = size.text();

        if (MSKU) {
            order.Note = data.Note;
            order.TotalDozens = data.TotalDozens;
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
    getEditedDataInline1: function (order) {
        var MSKU = (order.GroupId != null);
        var data = null;
        if (MSKU) {
            data = {};
        }
        else {
            data = order;
        }
        data.OrderStatus = order.OrderStatus;

        //datagridMode= $("#frmWOMEdit #gridMode").val();
        data.AssortCode= order.OrderStatus;
        data.PrimaryDC = order.PrimaryDC;
        data.OrderDetailId = order.OrderDetailId;
        data.OriginTypeCode = order.OriginTypeCode;
        //  .RoutingId= $("#frmWOMEdit #RoutingId").val();
            
        data.BusinessUnit = order.BusinessUnit;
     
        data.Style = order.Style;
        data.Color = order.Color;
        
        data.Attribute = order.Attribute;
        data.Revision = order.Revision;
        data.AltId = order.AltId;
        data.MC = order.MC;
        //Cy.linderSizes= $("#frmWOMEdit #CylinderSizes").val();
        data.StartDate = order.StartDate;
        data.CurrDueDate = order.CurrDueDate;

        //WOM.resetDateOffset(data.StartDate)
        //WOM.resetDateOffset(data.CurrDueDate)

        data.TxtPath = order.TxtPath;
        data.CuttPath = order.CutPath;
        data.CutPath = order.CutPath;
        data.SewPath = order.SewPath;
        data.Atr = order.Atr;
        data.MfgPathId = order.MfgPathId;
        data.DcLoc = order.DcLoc.toUpperCase();
        data.ExpeditePriority = order.ExpeditePriority;
        data.CategoryCode = order.CategoryCode;
        data.PackCode = order.PackCode;
        //var rdval 
        data.DozensOnly = order.DozensOnly;
        data.CreateBd = order.CreateBd; 
        
        data.BOMUpdate = order.BOMUpdate;

        data.Note = order.Note;
        //if (!ISS.common.equals(order.Cloned.Note, order.Note)) { order.NoteInd = 'Y'; }
        data.TotalDozens = order.TotalDozens;
        data.QtyDZ = order.QtyDZ;
        //var size = $("#frmWOMEdit #Size").data("kendoComboBox");
        data.Size = order.Size;
        data.SizeShortDes = order.SizeShortDes;

        if (MSKU) {
            order.Note = data.Note;
            order.TotalDozens = data.TotalDozens;
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
    getEditedDataInline: function (data) {

        data.DcLoc = data.DcLoc.toUpperCase();
        order = data.Cloned;
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
            order.TotalDozens = data.TotalDozens;
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
            //ISS.common.showPopUpMessage('Changing Status to Suggested will ungroup orders.', 'Information');
            ISS.common.notify.error('Changing Status to Suggested will ungroup orders.');

        }
        for (var i = 0; i < list.length; i++) {
            var mOrder = list[i];
            ISS.common.cloneAndStore(mOrder);
            var flag = false;
            //TBD IsDeleted Check
            if (!ISS.common.equals(data.AssortCode , order.AssortCode)) { mOrder.AssortCode = data.AssortCode; flag = true; }
            if (!ISS.common.equals(data.PrimaryDC , order.PrimaryDC)) { mOrder.PrimaryDC = data.PrimaryDC; flag = true; }
            if (!ISS.common.equals(data.OrderDetailId , order.OrderDetailId)) { mOrder.OrderDetailId = data.OrderDetailId; flag = true; }
            if (!ISS.common.equals(data.OriginTypeCode , order.OriginTypeCode)) { mOrder.OriginTypeCode = data.OriginTypeCode; flag = true; }
                
            if (!ISS.common.equals(data.BusinessUnit , order.BusinessUnit) ){ mOrder.BusinessUnit = data.BusinessUnit; flag = true; }
                
            if (!ISS.common.equals(data.Style , order.Style)) { mOrder.Style = data.Style; flag = true; }
            if (!ISS.common.equals(data.Color , order.Color) ){ mOrder.Color = data.Color; flag = true; }
            if (!ISS.common.equals(data.Attribute , order.Attribute)) { mOrder.Attribute = data.Attribute; flag = true; }
                
            if (!ISS.common.equals(data.Revision , order.Revision)) { mOrder.Revision = data.Revision; flag = true; }
            if (!ISS.common.equals(data.AltId , order.AltId) ){ mOrder.AltId = data.AltId; flag = true; }
            if (!ISS.common.equals(data.MC , order.MC) ){ mOrder.MC = data.MC; flag = true; }
            if (!ISS.common.equals(data.StartDate, order.StartDate)) { mOrder.StartDate = data.StartDate; flag = true; }
            if (!ISS.common.equals(data.CurrDueDate, order.CurrDueDate)) { mOrder.CurrDueDate = data.CurrDueDate; flag = true; }
            if (!ISS.common.equals(data.TxtPath , order.TxtPath)) { mOrder.TxtPath = data.TxtPath; flag = true; }
            if (!ISS.common.equals(data.CutPath , order.CutPath)) { mOrder.CutPath = data.CutPath; flag = true; }
            if (!ISS.common.equals(data.SewPath , order.SewPath)) { mOrder.SewPath = data.SewPath; flag = true; }
            if (!ISS.common.equals(data.Atr, order.Atr) ){ mOrder.Atr = data.Atr; }
            if (!ISS.common.equals(data.MfgPathId, order.MfgPathId)) { mOrder.MfgPathId = data.MfgPathId; flag = true; }
            if (!ISS.common.equals(data.MfgPath, order.MfgPath)) { mOrder.MfgPath = data.MfgPath; flag = true; }
            if (!ISS.common.equals(data.DcLoc , order.DcLoc) ){ mOrder.DcLoc = data.DcLoc; flag = true; }
            if (!ISS.common.equals(data.ExpeditePriority, order.ExpeditePriority)) { mOrder.ExpeditePriority = data.ExpeditePriority; flag = true; }
            if (!ISS.common.equals(data.CategoryCode , order.CategoryCode) ){ mOrder.CategoryCode = data.CategoryCode; flag = true; }
            if (!ISS.common.equals(data.PackCode , order.PackCode)) { mOrder.PackCode = data.PackCode; flag = true; }
            if (!ISS.common.equals(data.CreateBd , order.CreateBd)) { mOrder.CreateBd = data.CreateBd; flag = true; }
            if (!ISS.common.equals(data.DozensOnly , order.DozensOnly) ){ mOrder.DozensOnly = data.DozensOnly; flag = true; }
            if (!ISS.common.equals(data.BOMUpdate, order.BOMUpdate)) { mOrder.BOMUpdate = data.BOMUpdate; flag = true; }

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

        order.AssortCode = data.AssortCode;
        order.PrimaryDC = data.PrimaryDC;
        order.OrderDetailId = data.OrderDetailId;
        order.OriginTypeCode = data.OriginTypeCode;
        order.BusinessUnit = data.BusinessUnit;
        order.Style = data.Style;
        order.Color = data.Color;
        order.Attribute = data.Attribute;
        order.Revision = data.Revision;
        order.AltId = data.AltId;
        order.MC = data.MC;
        order.StartDate = data.StartDate;
        order.CurrDueDate = data.CurrDueDate;
        order.TxtPath = data.TxtPath;
        order.CutPath = data.CutPath;
        order.SewPath = data.SewPath;
        order.Atr = data.Atr;
        order.MfgPathId = data.MfgPathId;
        order.DcLoc = data.DcLoc;
        order.ExpeditePriority = data.ExpeditePriority;
        order.CategoryCode = data.CategoryCode;
        order.PackCode = data.PackCode;
        order.CreateBd = data.CreateBd;
        order.DozensOnly = data.DozensOnly;
        order.BOMUpdate = data.BOMUpdate;
        if (data.OrderStatus != order.OrderStatus) {
            order.OrderStatus = data.OrderStatus;
            WOM.setOrderType(order);
        }

    },
   
    setOrderType: function (order) {
        if (order.OrderStatus == 'L') {
            order.OrderType = 'WO';
            order.OrderStatusDesc = WOM.const.OrderStatus.Locked;

        } else if (order.OrderStatus == 'R') {
            order.OrderType = 'WO';
            order.OrderStatusDesc = WOM.const.OrderStatus.Released;

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
            $("#frmWOMMassChange #Dozen").val('')
            $("#frmWOMMassChange #OrderStatusMC").data("kendoDropDownList").value('')
            $("#frmWOMMassChange #ExpeditePriority").val('')
            $("#frmWOMMassChange #CatCD").val('')
            $("#frmWOMMassChange #DueDateStr").data("kendoDropDownList").value('Start')
            $("#frmWOMMassChange #MDueDate").data("kendoDatePicker").value(null)
            $("#frmWOMMassChange #MColor").val('')
            $("#frmWOMMassChange #MAttribute").val('')
            $("#frmWOMMassChange #AltId").val('')
            $("#frmWOMMassChange #Rev").val('')
            
            $("#frmWOMMassChange #MachineMC").data("kendoComboBox").value('')
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
            TotalDozens: $("#frmWOMMassChange #Dozen").val(),
            OrderStatus: $("#frmWOMMassChange #OrderStatusMC").data("kendoDropDownList").value(),
            ExpeditePriority: $("#frmWOMMassChange #Priority").val(),
            CategoryCode: $("#frmWOMMassChange #CatCD").val(),
            DueDateStr: $("#frmWOMMassChange #DueDateStr").data("kendoDropDownList").value(),
            DueDate: $("#frmWOMMassChange #MDueDate").data("kendoDatePicker").value(),
            Color: $("#frmWOMMassChange #MColor").val(),
            Attribute: $("#frmWOMMassChange #MAttribute").val(),
            AltId: $("#frmWOMMassChange #AltId").val(),
            AltIdd: $("#frmWOMMassChange #AltId").val(),
            Revision: $("#frmWOMMassChange #Rev").val(),
           // TextilePlant: $("#frmWOMMassChange #TextilePlant").data("kendoComboBox").value(),//TxtPath
            MC: $("#frmWOMMassChange #MachineMC").data("kendoComboBox").value().toUpperCase(),
            Limit: $("#frmWOMMassChange #Limit").val(),
            FitToMachine: $("#frmWOMMassChange #FitToMachine").prop('checked'),
            BOMUpdate: $("#frmWOMMassChange #BOMUpdate").prop('checked'),           
        };
        

        //TxtPath
        //data.QtyDZ = QtyDz;
      
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
        var prop = WOM.getNotNullProps(data, ['OrderStatus', 'DueDate', 'DueDateStr','DozensOnlyOff', 'Limit', 'FitToMachine' ]);
        var DueDate=null;
        if(! ISS.common.isNull(data.DueDate) && WOM.validate60daysDate(data.DueDate)){
            DueDate = data.DueDate;
            WOM.resetDateOffset(DueDate);

        }
        if (MSKUFlag && !ISS.common.isNull(data.OrderStatus) && data.OrderStatus =='S') {
            ISS.common.showPopUpMessage('Changing Status to Suggested will ungroup orders.', 'Information');
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
            if (prop.length > 0) {
                item.IsEdited = true;
                item.IsFieldChange = true;
                for (j = 0; j < prop.length; j++) {
                    item[prop[j]] = data[prop[j]];
                }
            }
            if (item.TotalDozens != item.Cloned.TotalDozens) {
                item.QtyDZ = WOM.TotDozChanged(item, data.TotalDozens);
            }

            if (DueDate && data.DueDateStr == 'Start')
                item.StartDate = DueDate;
            else if (DueDate && data.DueDateStr == 'DC')
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
                        "Textile Plant is Invalid.  The Finishing Indicator must be set to a 'Y' in the APS   Plant table. It is currently set to an 'N'.  Either select a valid Textile Plant    or contact the Cost Dept if you believe the plant you selected is a valid Textile Plant.");
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
    selectEvent: function () {

        $('#frmWOMEdit #AltId').focus();
    },
    loadColor: function (attr ) {
        var postData = WOM.retrieveColorData();
        var color = $("#frmWOMEdit #Color").data("kendoComboBox");
        postData = JSON.stringify(postData);
        ISS.common.executeActionAsynchronous("../order/GetColorCodes", postData, function (stat, data) {
            if (stat) {
                if (data.length > 0) {
                    color.dataSource.data(data);
                    var ds = color.dataSource.data();
                    if (color.value()=='' && ds.length > 0)
                        color.value(ds[0].Color);
                    var attrib = $("#Attribute").data("kendoComboBox");
                    attrib.dataSource.read(); 
                } 
            }
            else {
                ISS.common.notify.error('Please provide a valid style.');
            }

        });
    },
    loadSize: function () {
                           
        var postData = WOM.retrieveSizeData();
     
        postData = JSON.stringify(postData);
        ISS.common.executeActionAsynchronous("../order/GetSizes", postData, function (stat, data) {
            if (stat) {
               
                var size = $("#frmWOMEdit #Size").data("kendoComboBox");
                    size.dataSource.data(data);
                    
                          
            }
        });
    },

    validateCategoryCodeWOM: function () {
        var $th=$(this)
        var postData = { catCode: $th.val() }
        if (WOM.const.CategoryCode != postData.catCode) {
            postData = JSON.stringify(postData);
            ISS.common.executeActionAsynchronous("ValidateCategoryCode", postData, function (stat, data) {
                if (stat && (! data)) {             
                  
                    ISS.common.showPopUpMessage("Invalid Category Code - "+ $th.val(), null, function () {
                        $th.val("");
                        $th.focus();
                    });
                }

            });
        }      
    },

    FilterData: function () {
        return {
            Style_Cd: sstyle,
            Color_Cd: scolor,
            Attribute_Cd: sattribute,
            Size_Cd: sssize
        };
    },

    MFGFilterData:function(){
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
            AssortCode: "A"
        };
    },

    CutPathFilterData:function(){
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
    },

    ColorSelect: function (e) {
        var dataItem = this.dataItem(e.item.index());
        if (dataItem)
            scolor = dataItem.Color;
        //scolor = this.value();
    },

    ColorChange: function (e) {
        var datas = e.sender.dataSource.data();
        var dataItem = datas[e.sender.selectedIndex];
        if (dataItem)
            scolor = dataItem.Color;
    },

    AttributeSelect: function (e) {
        var dataItem = this.dataItem(e.item.index());
        if (dataItem)
            sattribute = dataItem.Attribute;
        //sattribute = this.value();
    },

    AttributeChange: function (e) {
        var datas = e.sender.dataSource.data();
        var dataItem = datas[e.sender.selectedIndex];
        if (dataItem)
            sattribute = dataItem.Attribute;
    },

    SizeSelect: function (e) {
        var dataItem = this.dataItem(e.item.index());
        if (dataItem) {
            sssize = dataItem.Size;
            ssizecode = dataItem.SizeDesc;
        }
    },

    SizeChange:function(e){
        var datas = e.sender.dataSource.data();
        var dataItem = datas[e.sender.selectedIndex];
        if (dataItem) {
            sssize = dataItem.Size;
            ssizecode = dataItem.SizeDesc;
        }
        var rows = WOM.selectedRows();
        $(rows).each(function (ind, data) {
            var RowData = gridWOM.dataItem(data);
            data.children['10'].innerText = dataItem.SizeDesc;
            RowData.SizeShortDes = dataItem.SizeDesc;
        });
    },

   RevisionSelect: function (e) {
       var dataItem = this.dataItem(e.item.index());
       var rows = WOM.selectedRows();
       $(rows).each(function (ind, data) {          
           if (dataItem.PKGStyle != "0") {
               var RowData = gridWOM.dataItem(data);
               data.children['7'].innerText = dataItem.PKGStyle;
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
               data.children['7'].innerText = dataItem.PKGStyle;
               RowData.Style = dataItem.PKGStyle;
           }
           sserror = false;
       });
       srevision = dataItem.Rev;
   },

   SellingSValidate: function (e) {
       if (!e.value) {
           return;
       }
       $('#grdWOM #Style').val(e.value.toUpperCase());
       sstyle = e.value.toUpperCase();
       var rows = WOM.selectedRows();
       $(rows).each(function (ind, data) {
           var RowData = gridWOM.dataItem(data);
           data.children['7'].innerText = e.value.toUpperCase();
           RowData.Style = e.value.toUpperCase();
       });
       //$("#grdWOM").data("kendoGrid").refresh();
       var postdata = { SellingStyle: sstyle };
       ISS.common.executeActionAsynchronous("../order/GetWOAsrtCode", JSON.stringify(postdata), function (stat, data) {
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
       
       //$("#grdWOM").data("kendoGrid").refresh();
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
           data.children['7'].innerText = dataItem.PKGStyle;
           RowData.Style = dataItem.PKGStyle;
           sserror = false;
           WOM.const.RevPopup.close();
       });
   },

   BDCheckClick: function (e) {
       if (e.checked == true) {
           //var rows = WOM.selectedRows();
           //var focusedCell = gridWOM.current();
           //var focusedCellIndex = focusedCell.index();
           var rows = $(e).closest("tr");
           var grid = gridWOM.dataItem(rows);
           if (WOM.IsAllowEdit(grid)) {
               $(rows).each(function (ind, data) {
                   var RowData = gridWOM.dataItem(data);
                   RowData.CreateBd = true;
                   RowData.DozensOnly = false;

                   var row = gridWOM.element.find("tr[data-uid='" + grid.uid + "']");
                   row.addClass("k-state-selected");
                   WOM.onRowsSelected();

                   //gridWOM.table.find("tr[data-uid='" + RowData.uid + "'] td:eq(21)")[0].innerHTML = "<input disabled='disabled' type='checkbox' data-bind='checked: DozensOnly'>";
                   gridWOM.table.find("tr[data-uid='" + RowData.uid + "'] td:eq(21)")[0].innerHTML = "<input type='checkbox' data-bind='checked: DozensOnly'>";  //Modified By :UST(Gopikrishnan), Date:28-June-2017, Description: As per the request Created BD & Dozens Only checkbox no need to be disabled because it makes the user to click the cell first for clicking the checkbox.
                   gridWOM.table.find("tr[data-uid='" + RowData.uid + "'] td:eq(21)")[0].className = 'k-dirty-cell';
                   $(gridWOM.table.find("tr[data-uid='" + RowData.uid + "'] td:eq(21)")[0]).append('<span class="k-dirty" style="margin-left: -26px;"></span>');

                   RowData.IsEdited = true;
                   RowData.IsFieldChange = true;
               });
               gridWOM.refresh();

               //Focusedindex = focusedCellIndex             
               var row = gridWOM.element.find("tr[data-uid='" + grid.uid + "']");
               row.addClass("k-state-selected");              
           }
           else {
               var msg = "Unable to Edit this row";
               ISS.common.notify.error(msg);
               grid.CreateBd = false;
               var row = gridWOM.element.find("tr[data-uid='" + grid.uid + "']");
               row.addClass("k-state-selected");
               gridWOM.table.find("tr[data-uid='" + grid.uid + "'] td:eq(20)")[0].innerHTML = "<input disabled = 'disabled' type='checkbox' data-bind='checked: CreateBd'>";
           }
       }
       else {
           var rows = $(e).closest("tr");
           var grid = gridWOM.dataItem(rows);
           if (!WOM.IsAllowEdit(grid)) {
               var msg = "Unable to Edit this row";
               ISS.common.notify.error(msg);
               grid.CreateBd = true;
               var row = gridWOM.element.find("tr[data-uid='" + grid.uid + "']");
               row.addClass("k-state-selected");
               gridWOM.table.find("tr[data-uid='" + grid.uid + "'] td:eq(20)")[0].innerHTML = "<input disabled='disabled' type='checkbox' data-bind='checked: CreateBd'>";
               gridWOM.refresh();

               var row = gridWOM.element.find("tr[data-uid='" + grid.uid + "']");
               row.addClass("k-state-selected");
               //gridWOM.table.find("tr[data-uid='" + grid.uid + "'] td:eq(20) input").focus();
           }
       }
   },

   DZCheckClick: function (e) {
       if (e.checked == true) {
           //var rows = WOM.selectedRows();
           var rows = $(e).closest("tr");
           var grid = gridWOM.dataItem(rows);
           if (WOM.IsAllowEdit(grid)) {
               $(rows).each(function (ind, data) {
                   var RowData = gridWOM.dataItem(data);
                   RowData.CreateBd = false;
                   RowData.DozensOnly = true;

                   var row = gridWOM.element.find("tr[data-uid='" + grid.uid + "']");
                   row.addClass("k-state-selected");
                   WOM.onRowsSelected();

                   gridWOM.table.find("tr[data-uid='" + RowData.uid + "'] td:eq(20)")[0].innerHTML = "<input type='checkbox' data-bind='checked: CreateBd'>";
                   gridWOM.table.find("tr[data-uid='" + RowData.uid + "'] td:eq(20)")[0].className = 'k-dirty-cell';
                   $(gridWOM.table.find("tr[data-uid='" + RowData.uid + "'] td:eq(20)")[0]).append('<span class="k-dirty" style="margin-left: -26px;"></span>');

                   RowData.IsEdited = true;
                   RowData.IsFieldChange = true;
               });
               gridWOM.refresh();

               var row = gridWOM.element.find("tr[data-uid='" + grid.uid + "']");
               row.addClass("k-state-selected");
              // gridWOM.table.find("tr[data-uid='" + grid.uid + "'] td:eq(21) input").focus();
           }
           else {
               var msg = "Unable to Edit this row";
               ISS.common.notify.error(msg);
               grid.DozensOnly = false;
               var row = gridWOM.element.find("tr[data-uid='" + grid.uid + "']");
               row.addClass("k-state-selected");
               gridWOM.table.find("tr[data-uid='" + grid.uid + "'] td:eq(21)")[0].innerHTML = "<input disabled = 'disabled' type='checkbox' data-bind='checked: DozensOnly'>";
           }

       }
       else {
           var rows = $(e).closest("tr");
           var grid = gridWOM.dataItem(rows);
           if (!WOM.IsAllowEdit(grid)) {
               var msg = "Unable to Edit this row";
               ISS.common.notify.error(msg);
               grid.DozensOnly = true;
               var row = gridWOM.element.find("tr[data-uid='" + grid.uid + "']");
               row.addClass("k-state-selected");
               gridWOM.table.find("tr[data-uid='" + grid.uid + "'] td:eq(21)")[0].innerHTML = "<input disabled = 'disabled' type='checkbox' data-bind='checked: DozensOnly'>";
               gridWOM.refresh();

               var row = gridWOM.element.find("tr[data-uid='" + grid.uid + "']");
               row.addClass("k-state-selected");
               //gridWOM.table.find("tr[data-uid='" + grid.uid + "'] td:eq(21) input").focus();
           }
       }
   },

   OrderChange: function (e) {
       var RowData = null;
       var datas = e.sender.dataSource.data();
       var dataItem = datas[e.sender.selectedIndex];
       var rows = WOM.selectedRows();
       $(rows).each(function (ind, data) {
           //var RowData = gridWOM.dataItem(data);
           RowData = gridWOM.dataItem(data);
           RowData.OrderStatus = dataItem.Value;
       });

       //Newly Added For Changing Order Status For Same Group
       if(RowData.GroupId != null)
           WOM.OrderStatusGroupChange(RowData);
       //End
   },

   onDemandSourceClick: function (e) {
       var PurOrder = null;
       var LineItm = null;
       var dmdDriver = null;
       var isHaa = null;
       var ruleNum = null;
       var oStatus = null;

       var rows = WOM.selectedRows();
       $(rows).each(function (ind, data) {
           var RowData = gridWOM.dataItem(data);
           var s = RowData.DemandSource;
           ruleNum = RowData.Rule;
           oStatus = RowData.OrderStatus;
           var sString = s
           if (sString){
               var fields = sString.split('_');
               isHaa = fields[1];
               PurOrder = fields[2];
               LineItm = fields[3];
               dmdDriver = RowData.DemandDriver;
           }
               
       });
       if (ruleNum == "9999" && (oStatus == "L" || oStatus == "P")) {
           var settings = {
               title: 'HAA Edit',
               animation: false,
               width: '700px',
               height: '200px'
           };
           passingValdata = {
               ValHaa: isHaa
           }
           passingValdata = JSON.stringify(passingValdata);
           ISS.common.executeActionAsynchronous("../order/GetPopupHaaAO", passingValdata, function (stat, Datares) {
               if (stat && Datares) {
                   if (Datares == true) {
                       WOM.const.DmdPopup = ISS.common.popUpCustom('.divDemandPopup', settings);
                       $('#frmDemandId #PurchaseOrder').val(PurOrder);
                       $('#frmDemandId #LineItem').val(LineItm);
                       $('#frmDemandId #DemandDriver').data("kendoDropDownList").value(dmdDriver);
                   }

               }
           });
       }
       
   },
   saveDemandDetails: function () {
       if ($('#frmDemandId #PurchaseOrder').val() != '' && $('#frmDemandId #LineItem').val() != '' && $('#frmDemandId #DemandDriver').data("kendoDropDownList").value() != '') {
           var rows = WOM.selectedRows();
           var poNum = $('#frmDemandId #PurchaseOrder').val().toUpperCase();
           var postData = { PoNo: $('#frmDemandId #PurchaseOrder').val().toUpperCase(), LineNo: $('#frmDemandId #LineItem').val() };
           postData = JSON.stringify(postData);
           ISS.common.executeActionAsynchronous("../order/GetDemandDvr", postData, function (stat, dat) {
               if (stat && dat) {
                   if (dat.length > 0) {
                       //alert("HAA is - " + dat + $("#frmDemandId #DemandDriver").data("kendoDropDownList").value())
                       var dmdrver = $("#frmDemandId #DemandDriver").data("kendoDropDownList").value();
                       $(rows).each(function (ind, data) {
                           var RowData = gridWOM.dataItem(data);
                           RowData.IsEdited = true;
                           RowData.DemandDriver = dmdrver;
                           RowData.DemandSource = dat;
                           gridWOM.table.find("tr[data-uid='" + RowData.uid + "'] td:eq(16)")[0].innerText = dmdrver;
                           gridWOM.table.find("tr[data-uid='" + RowData.uid + "'] td:eq(16)")[0].className = 'k-dirty-cell';
                           if (dmdrver == "PDQ")
                               $(gridWOM.table.find("tr[data-uid='" + RowData.uid + "'] td:eq(16)")[0]).append('<span class="k-dirty" style="margin-left: -31px;"></span>');
                           else
                               $(gridWOM.table.find("tr[data-uid='" + RowData.uid + "'] td:eq(16)")[0]).append('<span class="k-dirty" style="margin-left: -45px;"></span>');
                           gridWOM.table.find("tr[data-uid='" + RowData.uid + "'] td:eq(17)")[0].innerText = dat;
                           gridWOM.table.find("tr[data-uid='" + RowData.uid + "'] td:eq(17)")[0].className = 'k-dirty-cell';
                           //$(gridWOM.table.find("tr[data-uid='" + RowData.uid + "'] td:eq(17)")[0]).append('<span class="k-dirty" style="margin-left: -145px;"></span>');

                       });
                   }
                   
               }
               
           });
           WOM.const.DmdPopup.close();
       }
       else {
           ISS.common.showPopUpMessage('PurchaseOrder/LineItem/DemandDriver should not be empty.');
       }
       return false;
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
   MfgPathChange: function (e) {
       var RowData = null;
       var datas = e.sender.dataSource.data();
       var dataItem = datas[e.sender.selectedIndex];
       var rows = WOM.selectedRows();
       if (dataItem) {
           $(rows).each(function (ind, data) {
               //var RowData = gridWOM.dataItem(data);
               RowData = gridWOM.dataItem(data);
               gridWOM.table.find("tr[data-uid='" + RowData.uid + "'] td:eq(10)")[0].innerText = dataItem.SewPltMfg;
               gridWOM.table.find("tr[data-uid='" + RowData.uid + "'] td:eq(10)")[0].className = 'k-dirty-cell';
               $(gridWOM.table.find("tr[data-uid='" + RowData.uid + "'] td:eq(10)")[0]).append('<span class="k-dirty" style="margin-left: -19px;"></span>');
               gridWOM.table.find("tr[data-uid='" + RowData.uid + "'] td:eq(11)")[0].innerText = dataItem.SewPltMfg;
               gridWOM.table.find("tr[data-uid='" + RowData.uid + "'] td:eq(11)")[0].className = 'k-dirty-cell';
               $(gridWOM.table.find("tr[data-uid='" + RowData.uid + "'] td:eq(11)")[0]).append('<span class="k-dirty" style="margin-left: -19px;"></span>');
               RowData.SewPath = dataItem.SewPltMfg;
               RowData.Atr = dataItem.SewPltMfg;
               RowData.MfgPathId = dataItem.MfgPathId;
           });

           //Newly Added for changing MFG Path in the same group
           if (RowData.GroupId != null)
               WOM.MFGPathGroupChange(RowData);
           //End
       }
   },

    
   revisionSearchClick: function () {
       WOM.loadRevisionDetailsGrid();
       WOM.const.RevPopup = ISS.common.popUp('.divRevisionSearchPopup', 'Revision Search', null, function () {
           setTimeout(function () {
               $("#grdWOM #Revision").focus();
           }, 25)
       });
   },

   loadRevisionDetailsGrid: function () {
       var grid = $("#grdRevisionDetails").data("kendoGrid");
       if (grid.pager.page() > 1) grid.pager.page(1)
       grid.dataSource.read();
       return false;
   },

   searchRevisionDetails: function () {
       
       var style = null;
       var color = null;
       var attribute = null;
       var size = null;
       var assort = null;
       var rows = WOM.selectedRows();
       $(rows).each(function (ind, data) {
           var RowData = gridWOM.dataItem(data);
           style = RowData.SellingStyle;
           color = RowData.SellingColor;
           attribute = RowData.SellingAttribute;
           size = RowData.Size;
           assort = RowData.AssortCode

       });
       var result= {
           SellingStyle: style,
           ColorCode: color,
           Attribute: attribute,
           Size: size,
           AssortCode: assort
       };
       result.SizeList = new Array();
       result.SizeList.push({ SizeCD: result.Size })

       return result;
   },

   showDetails: function (e) {

       var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
       var rows = WOM.selectedRows();
       $(rows).each(function (ind, data) {
           var RowData = gridWOM.dataItem(data);
           data.children['7'].innerText = dataItem.PKGStyle;
           RowData.Style = dataItem.PKGStyle;
           data.children['12'].innerText = dataItem.NewRevision;
           RowData.Revision = dataItem.NewRevision;
           RowData.IsEdited = true;
           RowData.IsFieldChange = true;
           sserror = false;
           WOM.const.RevPopup.close();

           //Newly Added For Changing Revision and Style for same group
           if (RowData.GroupId != null) {
               WOM.RevisionStyleGroupChange(RowData);
           }

       });

   },

   showAlternateIdPopUp: function (e) {

       var res = WOM.searchRevisionDetails();
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
               //  $('#frmWOMEdit #AltIdd').val(d.AltId).focus();
               var rows = WOM.selectedRows();
               $(rows).each(function (ind, data) {
                   var RowData = gridWOM.dataItem(data);
                   data.children['13'].innerText = d.CuttingAlt;
                   RowData.AltIdd = d.CuttingAlt;
                   RowData.IsEdited = true;
                   RowData.IsFieldChange = true;
                   sserror = false;
               });
              
           },
           close: function () {
               setTimeout(function () {
                   $("#frmWOMEdit #AltIdd").focus();
               }, 25)
               return false;
           },
       };
       ISS.common.CommonSearchShow(settings);

   },

   showMachineCodesPopUp: function (e) {

       var superOrder = null;
       var rows = WOM.selectedRows();
       var RowData = null;
       $(rows).each(function (ind, data) {
           RowData = gridWOM.dataItem(data);
           superOrder = RowData.SuperOrder
       });
       var result = {
           SuperOrder: superOrder

       };


       //result.Plant = $('#grdWOM #CutPath').val();
       result.Plant = RowData.CuttPath;
       settings = {
           columns: [{
               Name: "iMacTypeCode",
               Title: "Machine",
           }],
           AllowSelect: true,
           title: 'Select Machine',
           url: WOM.const.urlMachineCodes,
           postData: result,
           handler: function (d) {
               //$('#frmWOMEdit #MC').val(d.iMacTypeCode).focus();
               //return false;
               var rows = WOM.selectedRows();
               $(rows).each(function (ind, data) {
                   var RowData = gridWOM.dataItem(data);
                   data.children['17'].innerText = d.iMacTypeCode;
                   RowData.MC = d.iMacTypeCode;
                   RowData.IsEdited = true;
                   RowData.IsFieldChange = true;
                   sserror = false;
               });

           },
           close: function () {
               setTimeout(function () {
                   $("#frmWOMEdit #MC").focus();
               }, 25)
               return false;
           },

       };
       ISS.common.CommonSearchShow(settings);

   },

   PopulateCutPathTxtPathPopUp: function (name) {
       var superOrder = null;
       var cutpath = null;
       var mfgpath = null;
       var rows = WOM.selectedRows();
       $(rows).each(function (ind, data) {
           var RowData = gridWOM.dataItem(data);
           superOrder = RowData.SuperOrder;
           mfgpath = RowData.MfgPathId;
           cutpath = RowData.CuttPath;
       });
       var result = {
           SuperOrder: superOrder
       };
       if (name == "CuttPath")
           result.DyeCode = 'C';
       else
           result.DyeCode = 'T';
       result.MFGPathId = mfgpath;
       result.CuttPath = cutpath;
       var Name = name;
       var form = 'frmWOMDetail';


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
           url: WOM.const.urlPopulateCutPathTxtPath,
           postData: result,
           handler: function (d) {
               //$('#' + form + ' #' + Name).val(d.Source_Plant).focus();
               var rows = WOM.selectedRows();
               $(rows).each(function (ind, data) {
                   var RowData = gridWOM.dataItem(data);
                   if (name == 'TxtPath') {
                       data.children['22'].innerText = d.Source_Plant;
                       RowData.TxtPath = d.Source_Plant;
                       RowData.IsEdited = true;
                       RowData.IsFieldChange = true;
                       sserror = false;
                   }
                   else {
                       data.children['23'].innerText = d.Source_Plant;
                       RowData.CuttPath = d.Source_Plant;
                       RowData.IsEdited = true;
                       RowData.IsFieldChange = true;
                       sserror = false;
                   }
               });

               //  d.Priority ??
               if (Name == 'TxtPath') {
                   WOM.validatePlant(d.Source_Plant)
               }
               return false;
           },
           close: function () {
               setTimeout(function () {
                   //$("#frmWOMEdit #CutPath").focus();
                   if (name == "CuttPath")
                       //$("#frmWOMEdit #CuttPath").focus();
                       $("#frmWOMDetail #CuttPath").focus();
                   else
                       $("#frmWOMDetail #TxtPath").focus();
               }, 25)
               return false;
           },
       };
       ISS.common.CommonSearchShow(settings);
   },

    //Added for changing Alternate in same group
   ChangeGroupAlt: function (e) {
       var RowData = null;
       var dataItem = e.currentTarget;
       var rows = WOM.selectedRows();
       $(rows).each(function (ind, data) {
           //var RowData = gridWOM.dataItem(data);
           RowData = gridWOM.dataItem(data);
           RowData.AltIdd = dataItem.value;
           //data.children['8'].innerText = dataItem.value;
           RowData.IsEdited = true;
           RowData.IsFieldChange = true;
       });


       if (RowData.GroupId != null) {
           var arr = new Array();
           var MSKUIDList = new Array();
           var MSKUFlag = false;
           var rows = $(WOM.selectedRows()).not('.deleted-row');
           if (rows.length > 0) {
               for (i = 0; i < rows.length; i++) {
                   var dataItem = gridWOM.dataItem(rows[i]);
                   if (WOM.IsAllowEdit(dataItem)) {
                       arr.push(dataItem);
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

           for (i = 0; i < arr.length; i++) {
               var item = arr[i];
               ISS.common.cloneAndStore(item);
               if (!ISS.common.isNull(RowData.AltIdd) && RowData.AltIdd != item.AltIdd) {
                   var stat = item.AltIdd;
                   item.AltIdd = RowData.AltIdd;
                   item.IsEdited = true;
                   item.IsFieldChange = true;
               }
           }
           gridWOM.refresh();
           WOM.onRowsSelected();
           return false;
       }     
    
   },
    //End

    //Added for changing CutPath in same group
   ChangeGroupCutPath: function (e) {
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

       if (RowData.GroupId != null) {
           var arr = new Array();
           var MSKUIDList = new Array();
           var MSKUFlag = false;
           var rows = $(WOM.selectedRows()).not('.deleted-row');
           if (rows.length > 0) {
               for (i = 0; i < rows.length; i++) {
                   var dataItem = gridWOM.dataItem(rows[i]);
                   if (WOM.IsAllowEdit(dataItem)) {
                       arr.push(dataItem);
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

           for (i = 0; i < arr.length; i++) {
               var item = arr[i];
               ISS.common.cloneAndStore(item);
               if (!ISS.common.isNull(RowData.CuttPath) && RowData.CuttPath != item.CuttPath) {
                   var stat = item.CuttPath;
                   item.CuttPath = RowData.CuttPath;
                   item.IsEdited = true;
                   item.IsFieldChange = true;
               }
           }
           gridWOM.refresh();
           WOM.onRowsSelected();
           return false;
       }         
      
   },
    //End

    //Added for getting AlternateId while changing TextPath from Path Ranking
   getAltIdForTxtPathChange: function (e) {
       var rows = WOM.selectedRows();
       var RowData = null;
       $(rows).each(function (ind, data) {
           RowData = gridWOM.dataItem(data);
       });
       WOM.const.SKUSizeList = [{ Size: RowData.SizeShortDes, SizeCD: RowData.Size }];
       var postData = { SellingStyle: RowData.SellingStyle, ColorCode: RowData.Color, Attribute: RowData.Attribute, SizeCde: RowData.Size, CutPath: RowData.CutPath, MfgPathId: RowData.MfgPathId, TxtPath: RowData.TxtPath };
       postData = JSON.stringify(postData);
       ISS.common.executeActionAsynchronous("../order/GetPathRankingAltId", postData, function (stat, dat) {
           if (stat && dat) {
               if (dat.length > 0) {
                   var rowsin = WOM.selectedRows();
                   $(rowsin).each(function (ind, data) {
                       var RowData = gridWOM.dataItem(data);
                       data.children['13'].innerText = dat[0].AlternateId;
                       RowData.AltIdd = dat[0].AlternateId;
                       RowData.IsEdited = true;
                       RowData.IsFieldChange = true;
                   });

               }
           }
          
       });

   },
    //End

    //Added for changing TextPath in same group
   ChangeGroupTextPath: function (e) {
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

       if (RowData.GroupId != null) {
           var arr = new Array();
           var MSKUIDList = new Array();
           var MSKUFlag = false;
           var rows = $(WOM.selectedRows()).not('.deleted-row');
           if (rows.length > 0) {
               for (i = 0; i < rows.length; i++) {
                   var dataItem = gridWOM.dataItem(rows[i]);
                   if (WOM.IsAllowEdit(dataItem)) {
                       arr.push(dataItem);
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

           for (i = 0; i < arr.length; i++) {
               var item = arr[i];
               ISS.common.cloneAndStore(item);
               if (!ISS.common.isNull(RowData.TxtPath) && RowData.TxtPath != item.TxtPath) {
                   var stat = item.TxtPath;
                   item.TxtPath = RowData.TxtPath;
                   item.IsEdited = true;
                   item.IsFieldChange = true;
               }
           }
           gridWOM.refresh();
           WOM.onRowsSelected();
           return false;
       }
            
   },
    //End

    //Added For Changing Start Date For Same Group
   StartDateGroupChange: function (e) {
       var RowData = null;
       var dataItem = e.sender;
       var rows = WOM.selectedRows();
       $(rows).each(function (ind, data) {
           RowData = gridWOM.dataItem(data);
           RowData.StartDate = dataItem._value;
       });

       if (RowData.GroupId != null) {
           var arr = new Array();
           var MSKUIDList = new Array();
           var MSKUFlag = false;
           var rows = $(WOM.selectedRows()).not('.deleted-row');
           if (rows.length > 0) {
               for (i = 0; i < rows.length; i++) {
                   var dataItem = gridWOM.dataItem(rows[i]);
                   if (WOM.IsAllowEdit(dataItem)) {
                       arr.push(dataItem);
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

           var DueDate = null;
           if (!ISS.common.isNull(RowData.StartDate) && WOM.validate60daysDate(RowData.StartDate)) {
               DueDate = RowData.StartDate;
               WOM.resetDateOffset(DueDate);

           }

           for (i = 0; i < arr.length; i++) {
               var item = arr[i];
               ISS.common.cloneAndStore(item);
               if (!ISS.common.isNull(RowData.StartDate) && RowData.StartDate != item.StartDate) {
                   var stat = item.StartDate;
                   item.StartDate = RowData.StartDate;
                   item.IsEdited = true;
                   item.IsFieldChange = true;
               }
           }
           gridWOM.refresh();
           WOM.onRowsSelected();
           return false;
       }

   },
    //End

    //Added For Changing DC Due Date For Same Group
   DCDueDateGroupChange: function (e) {
       var RowData = null;
       var dataItem = e.sender;
       var rows = WOM.selectedRows();
       $(rows).each(function (ind, data) {
           RowData = gridWOM.dataItem(data);
           RowData.CurrDueDate = dataItem._value;
       });

       if (RowData.GroupId != null) {
           var arr = new Array();
           var MSKUIDList = new Array();
           var MSKUFlag = false;
           var rows = $(WOM.selectedRows()).not('.deleted-row');
           if (rows.length > 0) {
               for (i = 0; i < rows.length; i++) {
                   var dataItem = gridWOM.dataItem(rows[i]);
                   if (WOM.IsAllowEdit(dataItem)) {
                       arr.push(dataItem);
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

           var DueDate = null;
           if (!ISS.common.isNull(RowData.CurrDueDate) && WOM.validate60daysDate(RowData.CurrDueDate)) {
               DueDate = RowData.CurrDueDate;
               WOM.resetDateOffset(DueDate);

           }

           for (i = 0; i < arr.length; i++) {
               var item = arr[i];
               ISS.common.cloneAndStore(item);
               if (!ISS.common.isNull(RowData.CurrDueDate) && RowData.CurrDueDate != item.CurrDueDate) {
                   var stat = item.CurrDueDate;
                   item.CurrDueDate = RowData.CurrDueDate;
                   item.IsEdited = true;
                   item.IsFieldChange = true;
               }
           }
           gridWOM.refresh();
           WOM.onRowsSelected();
           return false;
       }  

   },
    //End

   //Added for updating BOM Update in same group
   BOMUpdateGroup: function (RowData) {
       var arr = new Array();
       var MSKUIDList = new Array();
       var MSKUFlag = false;
       var rows = $(WOM.selectedRows()).not('.deleted-row');
       if (rows.length > 0) {
           for (i = 0; i < rows.length; i++) {
               var dataItem = gridWOM.dataItem(rows[i]);
               if (WOM.IsAllowEdit(dataItem)) {
                   arr.push(dataItem);
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


       for (i = 0; i < arr.length; i++) {
           var item = arr[i];
           ISS.common.cloneAndStore(item);
           if (RowData.BOMUpdate != item.BOMUpdate) {
               var stat = item.BOMUpdate;
               item.BOMUpdate = RowData.BOMUpdate;
               item.IsEdited = true;
               item.IsFieldChange = true;
           }
       }
       gridWOM.refresh();
       WOM.onRowsSelected();
       return false;
   },
   //End

    //Added for Changing Revision and PKGStyle for same group
   RevisionStyleGroupChange: function (RowData) {      
       var arr = new Array();
       var MSKUIDList = new Array();
       var MSKUFlag = false;
       var rows = $(WOM.selectedRows()).not('.deleted-row');
       if (rows.length > 0) {
           for (i = 0; i < rows.length; i++) {
               var dataItem = gridWOM.dataItem(rows[i]);
               if (WOM.IsAllowEdit(dataItem)) {
                   arr.push(dataItem);
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

       for (i = 0; i < arr.length; i++) {
           var item = arr[i];
           ISS.common.cloneAndStore(item);
           if (!ISS.common.isNull(RowData.Revision) && RowData.Revision != item.Revision) {
               var stat = item.Revision;
               item.Revision = RowData.Revision;
               item.IsEdited = true;
               item.IsFieldChange = true;
           }
           if (!ISS.common.isNull(RowData.Style) && RowData.Style != item.Style) {
               var stat = item.Style;
               item.Style = RowData.Style;
               item.IsEdited = true;
               item.IsFieldChange = true;
           }
       }
       gridWOM.refresh();
       WOM.onRowsSelected();
       return false;
       

   },
    //End

    //Newly Added For Changing Order Status For Same Group
   OrderStatusGroupChange : function(RowData) {
       var arr = new Array();
       var MSKUIDList = new Array();
       var MSKUFlag = false;
       var rows = $(WOM.selectedRows()).not('.deleted-row');
       if (rows.length > 0) {
           for (i = 0; i < rows.length; i++) {
               var dataItem = gridWOM.dataItem(rows[i]);
               if (WOM.IsAllowEdit(dataItem)) {
                   arr.push(dataItem);
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

       var DueDate = null;
       if (!ISS.common.isNull(RowData.DueDate) && WOM.validate60daysDate(RowData.DueDate)) {
           DueDate = RowData.DueDate;
           WOM.resetDateOffset(DueDate);

       }
       if (MSKUFlag && !ISS.common.isNull(RowData.OrderStatus) && RowData.OrderStatus == 'S') {
           ISS.common.showPopUpMessage('Changing Status to Suggested will ungroup orders.', 'Information');
       }

       for (i = 0; i < arr.length; i++) {
           var item = arr[i];
           ISS.common.cloneAndStore(item);                  
           if (!ISS.common.isNull(RowData.OrderStatus) && RowData.OrderStatus != item.OrderStatus) {
               var stat = item.OrderStatus;
               item.OrderStatus = RowData.OrderStatus;
               WOM.ValidateProdStatus(item, stat);
               //WOM.setOrderType(item);
               item.IsEdited = true;
               item.IsFieldChange = true;
           }
           WOM.setOrderType(item);
       }
       gridWOM.refresh();
       WOM.onRowsSelected();
       return false;
   },
    //End

    //Newly Added for changing MFG Path in the same group
   MFGPathGroupChange: function (RowData) {
       var arr = new Array();
       var MSKUIDList = new Array();
       var MSKUFlag = false;
       var rows = $(WOM.selectedRows()).not('.deleted-row');
       if (rows.length > 0) {
           for (i = 0; i < rows.length; i++) {
               var dataItem = gridWOM.dataItem(rows[i]);
               if (WOM.IsAllowEdit(dataItem)) {
                   arr.push(dataItem);
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

       for (i = 0; i < arr.length; i++) {
           var item = arr[i];
           ISS.common.cloneAndStore(item);
           if (!ISS.common.isNull(RowData.SewPath) && RowData.SewPath != item.SewPath) {
               var stat = item.SewPath;
               item.SewPath = RowData.SewPath;
               item.IsEdited = true;
               item.IsFieldChange = true;
           }
           if (!ISS.common.isNull(RowData.Atr) && RowData.Atr != item.Atr) {
               var stat = item.Atr;
               item.Atr = RowData.Atr;
               item.IsEdited = true;
               item.IsFieldChange = true;
           }
           if (!ISS.common.isNull(RowData.MfgPathId) && RowData.MfgPathId != item.MfgPath) {
               var stat = item.MfgPathId;
               item.MfgPath = RowData.MfgPathId;
               item.IsEdited = true;
               item.IsFieldChange = true;
           }
       }
       gridWOM.refresh();
       WOM.onRowsSelected();
       return false;
   },
    //End

   BOMUpdateCheckClick: function (e) {
       var RowData = null;
       var dataItem = e.checked;
       //var rows = WOM.selectedRows();
       var rows = $(e).closest("tr");
       var grid = gridWOM.dataItem(rows);
       if (WOM.IsAllowEdit(grid)) {
           $(rows).each(function (ind, data) {
               RowData = gridWOM.dataItem(data);
               RowData.BOMUpdate = dataItem;

               var row = gridWOM.element.find("tr[data-uid='" + grid.uid + "']");
               row.addClass("k-state-selected");
               WOM.onRowsSelected();

               RowData.IsEdited = true;
               RowData.IsFieldChange = true;

           });

           if (RowData != null && RowData.GroupId != null)
               WOM.BOMUpdateGroup(RowData)
       }
       else {
           var msg = "Unable to Edit this row";
           ISS.common.notify.error(msg);
           if (e.checked = false)
               grid.BOMUpdate = true;
           else
               grid.BOMUpdate = false;
           gridWOM.table.find("tr[data-uid='" + grid.uid + "'] td:eq(22)")[0].innerHTML = "<input disabled = 'disabled' type='checkbox' data-bind='checked: BOMUpdate'>";
           gridWOM.refresh();

           var row = gridWOM.element.find("tr[data-uid='" + grid.uid + "']");
           row.addClass("k-state-selected");
       }
   },

}

$.extend(WOM, tWOM);
 