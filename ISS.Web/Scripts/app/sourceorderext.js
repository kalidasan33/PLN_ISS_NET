var style_cd = null;
var color_cd = null;
var attr_cd = null;
var size_cd = null;
var rev_cd = null;
var uom_cd = null;
var demand_loc = null;
var qty_cd = null;
var currentTableRow = null;
var currentTableQty = null;
var currentQty = null;
var flag = 0;


function retrieveColorData() {
    var result = {
        Style_Cd: style_cd
    };
    return result;
}

function retrieveAttributeData() {
    var result = {
        Style_Cd: style_cd,
        Color_Cd: color_cd
    };
    return result;
}

function retrieveSizeData() {
    var result = {
        Style_Cd: style_cd,
        Color_Cd: color_cd,
        Attribute_Cd: attr_cd
    };
    return result;
}

function retrieveUomData() {
    var result = {
        Style_Cd: style_cd,
        Color_Cd: color_cd,
        Attribute_Cd: attr_cd,
        Size_Cd: size_cd
    };
    return result;
}

$(document).ready(function () {
    $("#frmRequisitions #DcLoc").focusout(function () {
        dcValidate();
    });

    $('#btnDelReq').bind('click', requisitions.DeleteRequisition);//btnSOSave
    $('#btnLeave').bind('click', onSave);
    $('#btnUp').bind('click', onSave);
    $('#btnDown').bind('click', onSave);
    $('.k-grid-add').click(function (e) {
        if (!requisitions.const.validator.validate())
            return false;
        else {
            flag = 1;
        }
    });
});

function onSave(e) {

    var grid = $("#grdRequisitionDetail").data("kendoGrid");
    var currentSelection = grid.select().parent();
    var selectedItem = grid.dataItem(currentSelection)
    if (selectedItem) {
        if (this.id == "btnLeave") {
            selectedItem.Qty = currentQty;
            requisitions.const.comment.close();
        }
        else if (this.id == "btnUp") {
            var qty = document.getElementById("lblUp").innerText;
            qty = qty.replace("-", ".");
            selectedItem.Qty = qty;
            requisitions.const.comment.close();
        }
        else if (this.id == "btnDown") {
            var qty = document.getElementById("lblDown").innerText;
            qty = qty.replace("-", ".");
            selectedItem.Qty = qty;
            requisitions.const.comment.close();
        }
    }
    grid.refresh();
}


function onGridChange(e) {
    var grid = $("#grdRequisitionDetail").data("kendoGrid");
    var currentSelection = grid.select().parent();
    if (currentSelection) {
        currentTableRow = currentSelection;
        var rowData = grid.dataItem(currentSelection);
        if (rowData) {
            style_cd = rowData.Style;
            color_cd = rowData.Color;
            attr_cd = rowData.Attribute;
            size_cd = rowData.Size;
            uom_cd = rowData.Uom;
            qty_cd = rowData.Qty;
            rowData.IsDirty = true;
        }
    }
}


function onColorSelect(e) {
    var dataItem = this.dataItem(e.item.index());
    color_cd = dataItem.Color;
    var grid = $("#grdRequisitionDetail").data("kendoGrid");
    if (grid) {
        var postData = { styleCode: style_cd, colorCode: color_cd };
        postData = JSON.stringify(postData);
        ISS.common.executeActionAsynchronous("../order/GetAttributeInfo", postData, loadAttributeInfo);
    }
}

function dcValidate() {
    if (requisitions.isValidFormMode()) {
        if ($("#frmRequisitions #DcLoc").val() != "") {
            var postData = { DcLoc: $("#frmRequisitions #DcLoc").val() };
            postData = JSON.stringify(postData);
            ISS.common.executeActionAsynchronous("../order/GetDcValidate", postData, function (stat, data) {
                if (stat && data) {
                    if (data.length > 0) {
                        if ($("#frmRequisitions #DcLoc").val() == data[0].DCLoc) {
                            var grid = $("#grdRequisitionDetail").data("kendoGrid");
                            var dataDS = grid.dataSource.data();
                            if (grid.dataSource._data.length > 0) {
                                for (var i = 0; i < dataDS.length; i++) {
                                    if ($("#frmRequisitions #DcLoc").val() != data[0].DCLoc) {
                                        ISS.common.showPopUpMessage('Invalid DC ! Try with another DC Loc');
                                        $("#frmRequisitions #DcLoc").val('');
                                        // $("#frmRequisitions #DcLoc").val('');
                                        return false;
                                    }

                                }
                                requisitions.validateRequisitionFull();
                            }
                        }
                    }
                    else {
                        $("#frmRequisitions #DcLoc").val('');
                        ISS.common.showPopUpMessage('Invalid DC ! Try with another DC Loc');
                        //setTimeout(function () { $("#frmRequisitions #DcLoc").focus(); }, 10);
                        //$("#frmRequisitions #DcLoc").val('');
                        return false;
                    }
                }

            });
        }
    }
}

function onAttributeSelect(e) {
    var dataItem = this.dataItem(e.item.index());
    attr_cd = dataItem.Attribute;

    var grid = $("#grdRequisitionDetail").data("kendoGrid");
    if (grid) {
        var postData = { styleCode: style_cd, colorCode: color_cd, attributeCode: attr_cd };
        postData = JSON.stringify(postData);
        ISS.common.executeActionAsynchronous("../order/GetSizeInfo", postData, loadSizeInfo);
    }
}

function onRevSelect(e) {
    var dataItem = this.dataItem(e.item.index());
    rev_cd = dataItem.Rev;
    revChanged();
}

function revChanged() {
    var grid = $("#grdRequisitionDetail").data("kendoGrid");
    if (grid) {
        var postData = { styleCode: style_cd, colorCode: color_cd, attributeCode: attr_cd, sizeCode: size_cd, revCode: rev_cd };
        postData = JSON.stringify(postData);
        ISS.common.executeActionAsynchronous("../order/GetStdCaseInfo", postData, loadStdCaseInfo);
    }
}

function onSizeSelect(e) {
    var dataItem = this.dataItem(e.item.index());
    size_cd = dataItem.Size;
    var grid = $("#grdRequisitionDetail").data("kendoGrid");
    var currentSelection = grid.select().parent();
    var selectedItem = grid.dataItem(currentSelection)
    if (selectedItem) {
        selectedItem.SizeLit = dataItem.SizeDesc;
    }

    if (grid) {
        var postData = { styleCode: style_cd, colorCode: color_cd, attributeCode: attr_cd, sizeCode: size_cd };
        postData = JSON.stringify(postData);
        ISS.common.executeActionAsynchronous("../order/GetRevisionAndUomInfo", postData, loadRevisionAndUomInfo);
    }
}

function onQtySelect(e) {
    var dataItem = this.dataItem(e.item.index());
    qty_cd = dataItem.Qty;

    var grid = $("#grdRequisitionDetail").data("kendoGrid");
    if (grid) {
        var postData = { qtyCode: qty_cd, colorCode: color_cd, attributeCode: attr_cd, sizeCode: size_cd };
        postData = JSON.stringify(postData);
        ISS.common.executeActionAsynchronous("../order/GetRevisionAndUomInfo", postData, loadRevisionAndUomInfo);
    }
}

function onStyleCodeClick(e) {
    //if (requisitions.const.validator.validate()) {
    currentTableRow = $(e).closest('tr');
    if (!e.value) {
        return;
    }
    style_cd = e.value.toUpperCase()
    var grid = $("#grdRequisitionDetail").data("kendoGrid");
    if (grid) {
        var postData = { Style: style_cd, BusinessCode: $("#frmRequisitions #BusinessUnit").val() };

        var selectedItem = grid.dataItem(currentTableRow)
        if (selectedItem) {
            selectedItem.Style = style_cd;
            selectedItem.IsDirty = true;
        }
        postData = JSON.stringify(postData);
        ISS.common.executeActionAsynchronous("../order/GetValidateStyle", postData, function (stat, data) {
            if (stat && data) {
                if (data.length > 0) {
                    if ($("#frmRequisitions #BusinessUnit").val() == data[0].BusinessUnit) {
                        setgridvalidatestatusstyle(data, currentTableRow);
                        ISS.common.executeActionAsynchronous("../order/GetRetrieve", postData, loadDescriptionAndColor);
                    }
                    else
                        ISS.common.notify.error('Style Business Unit is not same as the Business Unit in the header.');
                }
                else {
                    selectedItem.Style = ''; grid.refresh()
                    ISS.common.notify.error('Invalid Style -' + style_cd);
                }
            }
            else {
                selectedItem.Style = ''; grid.refresh()
                ISS.common.notify.error('Invalid Style -' + style_cd);
            }
        });

    }
    return true;
    // }
}

function replaceAndCheck(s){
    s = $.trim(s.replace("-", "."));
    if (s.indexOf('.')==0) s = '0' + s;
    var arr = s.split('.');
    if (arr.length > 1 && arr[1].length > 2 ) {
        s=arr[0]+'.'+arr[1].substring(0,2)
    }
    if (s != "" && !s.match(/^\d+(\.\d{1,2})?$/)) {
        s = arr[0];
    }
    return s;
}

function onQtyCodeChanged(e) {
    var s = e.value.toString();
    s=replaceAndCheck(s);
    if (getEaches(s) == 10) {
        var grid = $("#grdRequisitionDetail").data("kendoGrid");
        var currentSelection = grid.select().parent();
        var selectedItem = grid.dataItem(currentSelection)
        if (selectedItem) {            
            if (s.indexOf('.') == -1) s += '.';
            while (s.length < s.indexOf('.') + 3) s += '0';
            selectedItem.Qty = s;
        }
    }
    else if (getEaches(s) >= 12) {
        ISS.common.showPopUpMessage("Eaches must be less than 12");
        var grid = $("#grdRequisitionDetail").data("kendoGrid");
        var currentSelection = grid.select().parent();
        var selectedItem = grid.dataItem(currentSelection)
        if (selectedItem) {
            selectedItem.Qty = parseInt(s);
        }
    }
    else {
        var grid = $("#grdRequisitionDetail").data("kendoGrid");
        var currentSelection = grid.select().parent();
        var selectedItem = grid.dataItem(currentSelection)
        if (selectedItem) {

            selectedItem.Qty = s;
        }
    }

    setTimeout('requisitions.gridDetailBound()', 200);
}
function getvalues(a) {
    var arr = a.split('.');
    if (arr.length > 1) {
        return parseInt(arr[1]);
    }
    return 0;
}
function getEaches(val) {
    var intPart = parseInt(val);
    var decimalPart = val - intPart;
    var result = Math.round(((decimalPart) * 100));
    return parseInt(result);
}
function onQtyCodeClick(e) {
    currentTableQty = $(e).closest('tr');
    if (!e.value) {
        return;
    }
    var ev = e.value.replace("-", ".")
    var evEach = getEaches(ev);
    if (evEach == 10 || evEach == 1 || evEach == 2 || evEach == 3 || evEach == 4 || evEach == 5 || evEach == 6 || evEach == 7 || evEach == 8 || evEach == 9 || evEach == 11) {
        var grid = $("#grdRequisitionDetail").data("kendoGrid");
        var currentSelection = grid.select().parent();
        var selectedItem = grid.dataItem(currentSelection)
        if (selectedItem) {
            var s = ev.toString();
            if (s.indexOf('.') == -1) s += '.';
            while (s.length < s.indexOf('.') + 3) s += '0';
            selectedItem.Qty = s;
            currentQty = s;
            var stdqty = getQtyToEach(selectedItem.StdCase);
            var val = 0;
            var arr = s.split('.');
            if (arr.length > 1) {
                  val = parseInt(arr[0] * 12) + parseInt(arr[1]);
            }
            var equal = val / stdqty;
            var x = Math.floor(equal)
            if (equal == x) {
                ISS.common.showPopUpMessage("SKU is already in the Std Case Qty", "ISS");
                return false;
            }
            var upindzns = (x + 1) * stdqty;
            var up = (parseInt(upindzns / 12) + (upindzns % 12) / 100).toFixed(2);
            up = up.replace(".", "-");
            document.getElementById("lblUp").innerText = up;
            var dwnindzns = (x) * stdqty;
            var dwn = (parseInt(dwnindzns / 12) + (dwnindzns % 12) / 100).toFixed(2);
            dwn = dwn.replace(".", "-");
            document.getElementById("lblDown").innerText = dwn;
        }
    }
    else if (evEach == 0) {
        var grid = $("#grdRequisitionDetail").data("kendoGrid");
        var currentSelection = grid.select().parent();
        var selectedItem = grid.dataItem(currentSelection)
        if (selectedItem) {
            //var x1 = ev / selectedItem.StdCase;
            var s = ev.toString();
            
            selectedItem.Qty = s;
            currentQty = s;
           
            var stdqtyHyp = selectedItem.StdCase.toString(); 
            stdqtyHyp = stdqtyHyp.replace("-", ".");
            var stdqty = getQtyToEach(stdqtyHyp);
            var val = null;             
            var arr = s.split('.');
            if (arr.length > 1) {
                val = parseInt(arr[0] * 12) + parseInt(arr[1]);
            }
            else {
                val = parseInt(arr[0] * 12);
            }
            var equal = val / stdqty;
            var x = Math.floor(equal)
            if (equal == x) {
                ISS.common.showPopUpMessage("SKU is already in the Std Case Qty", "ISS");
                return false;
            }
            var upindzns = (x + 1) * stdqty;
            var up = (parseInt(upindzns / 12) + (upindzns % 12) / 100).toFixed(2);
            up = up.replace(".", "-");
            document.getElementById("lblUp").innerText = up;
            var dwnindzns = (x) * stdqty;
            var dwn = (parseInt(dwnindzns / 12) + (dwnindzns % 12) / 100).toFixed(2);
            dwn = dwn.replace(".", "-");
            document.getElementById("lblDown").innerText = dwn;
        }
    }
    var grid = $("#grdRequisitionDetail").data("kendoGrid");
    var currentSelection = grid.select().parent();
    var selectedItem = grid.dataItem(currentSelection)
    if (selectedItem) {
        // $('tr[data-uid="' + selectedItem.uid + '"] td:nth-child(10)').removeClass("highlighted-rowgreen");

        if (selectedItem.StdCase == 0.01) {
            ISS.common.showPopUpMessage('Please select the revision for std case..');
        }
        else {
            //ISS.common.showConfirmMessageCustom('Please select the value for Qty');
            //  $('tr[data-uid="' + selectedItem.uid + '"] td:nth-child(10)').addClass("highlighted-rowgreen");

            if (ev == 0) {
                ISS.common.showPopUpMessage("Please enter Std Case multiple Qty");
                return false;
            }
            if (evEach >= 12) {
                ISS.common.showPopUpMessage("Eaches must be less than 12");
                return false;
            }
            
            else {
                if (x == 0) {
                    $('#btnDown, .roundrnddwn').hide()
                }
                else {
                    $('#btnDown, .roundrnddwn').show()

                }
                if (equal != x) {
                    
                    var stdqty = getQtyToEachDisp(selectedItem.StdCase);
                    stdqty = (stdqty+'').replace(".", "-");
                    currentQty = currentQty.replace(".", "-");
                    document.getElementById("lblQty").innerText = "Qty " + currentQty + " is not a STD Case Multiple " + stdqty;
                    var settings = {
                        title: 'Std Case Qty - Rounding Message',
                        animation: false,
                        height: '163px',
                        width: '380px',
                    };
                    requisitions.const.comment = ISS.common.popUpCustom('.divQtyPopup', settings);
                }
                else
                    ISS.common.showPopUpMessage("SKU is already in the Std Case Qty", "ISS");

            }

        }
    }
    
}

function getQtyToEach(q) {
    return parseInt(q) *12 + getEaches(q)
}

function getQtyToEachDisp(q) {
    var v =parseInt(q)
    var d = getEaches(q)
    if (d == 0) return q + '.00';
    else if (d == 10) return q + '0';
    else return q;
}

//QTY is not a STD Case Multiple "Sq"

//Please select an option.

//Round up to 

//Round down to 
function setgridvalidatestatusstyle(data, row) {
    if (data.length > 0) {
        if (data && row.length > 0) {
            var grid = $("#grdRequisitionDetail").data("kendoGrid");
            var selectedItem = grid.dataItem(currentTableRow)
            if (selectedItem) {
                selectedItem.PackCD = data[0].PackCD;
                selectedItem.PackageQty = data[0].PackageQty;
                selectedItem.MatlCd = data[0].MatlCd;
                selectedItem.AsrmtCd = data[0].AsrmtCd;
                selectedItem.BusinessUnit = data[0].BusinessUnit;
                selectedItem.ProdFamilyCd = data[0].ProdFamilyCd;
                //selectedItem = data.ProdClass;                     //for FABRIC_GROUP
                selectedItem.FabricGroup = data[0].FabricGroup;
                selectedItem.Uom = data[0].Uom;
                selectedItem.PipeLineCategoryCD = "SEW";
                selectedItem.GarmentStyle = data[0].GarmentStyle;
                selectedItem.SizeLine = data[0].SizeLine;
                selectedItem.ErrorStatus = data[0].ErrorStatus;
                selectedItem.ErrorMessage = data[0].ErrorMessage;

                //if (selectedItem.ErrorStatus == false)
                //    $("#grdRequisitionDetail tr").css("background-color", "red");
                if (selectedItem.ErrorStatus == true) {
                    $('tr[data-uid="' + selectedItem.uid + '"]').addClass("highlighted-row");
                    //grid.table.find("tr[data-uid='" + gridData.uid + "']").addClass("highlighted-row");

                }
                else {
                    $('tr[data-uid="' + selectedItem.uid + '"]').removeClass("highlighted-row");
                }

            }
        }
    }
}
function setgridvalidatestatus(data, row) {
    row.ErrorStatus = data.ErrorStatus;
    row.ErrorMessage = data.ErrorMessage;
    if (data.ErrorStatus == true) {
        $('tr[data-uid="' + row.uid + '"]').addClass("highlighted-row");
        return false;
    }
    else {
        $('tr[data-uid="' + row.uid + '"]').removeClass("highlighted-row");
    }
    if (data.PlanDate != null)
        row.PlanDate = ISS.common.parseDate(data.PlanDate);
    else
        row.PlanDate = ISS.common.parseDate(row.PlanDate);
    if (data.ScheduledShipDate != null)
        row.ScheduledShipDate = ISS.common.parseDate(data.ScheduledShipDate);
    else
        row.ScheduledShipDate = ISS.common.parseDate(row.ScheduledShipDate);
    if (data.CurrDueDate != null)
        row.CurrDueDate = ISS.common.parseDate(data.CurrDueDate);
    else
        row.CurrDueDate = ISS.common.parseDate(row.CurrDueDate);
    if (data.OriginalDueDate != null)
        row.OriginalDueDate = ISS.common.parseDate(data.OriginalDueDate);
    else
        row.OriginalDueDate = ISS.common.parseDate(row.OriginalDueDate);
    row.PlannedLeadTime = data.PlannedLeadTime;

    // $("#gridSellIn tr").css("background-color", "red");

    //error red

}

function setgridValidateRefresh(data, row) {
    setgridvalidatestatus(data, row)
    $("#grdRequisitionDetail").data("kendoGrid").refresh();

}

function loadDescriptionAndColor(stat, data) {
    if (stat && data.length > 0) {
        var grid = $("#grdRequisitionDetail").data("kendoGrid");
        var selectedItem = grid.dataItem(currentTableRow)
        if (selectedItem) {
            selectedItem.Style = style_cd;
            selectedItem.Color = data[0].Color;
            selectedItem.Description = data[0].StyleDesc;
            selectedItem.Attribute = data[0].Attribute;
            selectedItem.Size = data[0].Size;
            selectedItem.SizeLit = data[0].SizeShortDes;
            selectedItem.Rev = data[0].Rev;
            selectedItem.Uom = data[0].Uom;
            //selectedItem.Qty = data[0].Qty;
            selectedItem.StdCase = data[0].StdCase;
            selectedItem.Dpr = data[0].Dpr;
            //selectedItem.PlanDate = data.PlanDate;
            style_cd = data[0].Style;
            color_cd = data[0].Color
            attr_cd = data[0].Attribute
            size_cd = data[0].Size
            rev_cd = data[0].Rev
            //setTimeout('revChanged()',1000);
            if (flag == 1) {
                selectedItem.DemandSource = "";
                selectedItem.DemandDriver = "ISS";
                selectedItem.OrderType = "RQ";
                selectedItem.MakeOrBuy = "B";
                selectedItem.DCLoc = $("#frmRequisitions #DcLoc").val();
            }
            selectedItem.ErrorStatus = data[0].ErrorStatus;
            if (selectedItem.ErrorStatus == true) {
                $('tr[data-uid="' + selectedItem.uid + '"]').addClass("highlighted-row");
            }
            else {
                $('tr[data-uid="' + selectedItem.uid + '"]').removeClass("highlighted-row");
            }
            //$("#grdRequisitionDetail").data("kendoGrid").refresh();
            flag = 0;
            requisitions.validateRequisitionDetailRow(selectedItem, setgridValidateRefresh)
        }
    }
    else {
        var grid = $("#grdRequisitionDetail").data("kendoGrid");
        var selectedItem = grid.dataItem(currentTableRow)
        if (selectedItem) {
            selectedItem.Style = "";
            selectedItem.Color = "";
            selectedItem.Description = "";
            selectedItem.Attribute = "";
            selectedItem.Size = "";
            selectedItem.Rev = "";
            selectedItem.Uom = "";
            //selectedItem.Qty = "";
            selectedItem.StdCase = "";
            selectedItem.Dpr = "";
            $("#grdRequisitionDetail").data("kendoGrid").refresh();
        }

        ISS.common.notify.error('Failed to retrieve Style details.');
    }
}

function loadAttributeInfo(stat, data) {
    if (stat && data.length > 0) {
        var grid = $("#grdRequisitionDetail").data("kendoGrid");
        var currentSelection = grid.select().parent();
        var selectedItem = grid.dataItem(currentSelection)
        if (selectedItem) {
            selectedItem.Attribute = data[0].Attribute;
            selectedItem.Size = data[0].Size;
            selectedItem.SizeLit = data[0].SizeShortDes;
            selectedItem.Uom = data[0].Uom;
            selectedItem.Rev = data[0].Rev;
            //selectedItem.Qty = data[0].Qty;
            selectedItem.StdCase = data[0].StdCase;
            selectedItem.Dpr = data[0].Dpr;
            if (selectedItem.ErrorStatus == true) {
                $('tr[data-uid="' + selectedItem.uid + '"]').addClass("highlighted-row");
            }
            else {
                $('tr[data-uid="' + selectedItem.uid + '"]').removeClass("highlighted-row");
            }
            //$("#grdRequisitionDetail").data("kendoGrid").refresh();
            requisitions.validateRequisitionDetailRow(selectedItem, setgridValidateRefresh)
        }
    }
    else {
        var grid = $("#grdRequisitionDetail").data("kendoGrid");
        var currentSelection = grid.select().parent();
        var selectedItem = grid.dataItem(currentSelection)
        if (selectedItem) {
            selectedItem.Attribute = "";
            selectedItem.Size = "";
            selectedItem.Uom = "";
            selectedItem.Rev = "";
            selectedItem.Qty = "";
            selectedItem.StdCase = "";
            selectedItem.Dpr = "";
            $("#grdRequisitionDetail").data("kendoGrid").refresh();
        }
        ISS.common.notify.error('Failed to retrieve Style details.');
    }
}

function loadSizeInfo(stat, data) {
    if (stat && data.length > 0) {
        var grid = $("#grdRequisitionDetail").data("kendoGrid");
        var currentSelection = grid.select().parent();
        var selectedItem = grid.dataItem(currentSelection)
        if (selectedItem) {
            selectedItem.Size = data[0].Size;
            selectedItem.Rev = data[0].Rev;
            selectedItem.Uom = data[0].Uom;
            //selectedItem.Qty = data[0].Qty;
            selectedItem.StdCase = data[0].StdCase;
            selectedItem.Dpr = data[0].Dpr;
            if (selectedItem.ErrorStatus == true) {
                $('tr[data-uid="' + selectedItem.uid + '"]').addClass("highlighted-row");
            }
            else {
                $('tr[data-uid="' + selectedItem.uid + '"]').removeClass("highlighted-row");
            }
            //$("#grdRequisitionDetail").data("kendoGrid").refresh();
            requisitions.validateRequisitionDetailRow(selectedItem, setgridValidateRefresh)
        }
    }
    else {
        var grid = $("#grdRequisitionDetail").data("kendoGrid");
        var currentSelection = grid.select().parent();
        var selectedItem = grid.dataItem(currentSelection)
        if (selectedItem) {
            selectedItem.Size = "";
            selectedItem.Rev = "";
            selectedItem.Uom = "";
            selectedItem.Qty = "";
            selectedItem.StdCase = "";
            selectedItem.Dpr = "";
            $("#grdRequisitionDetail").data("kendoGrid").refresh();
        }
        ISS.common.notify.error('Failed to retrieve Style details.');
    }
}

function loadRevisionAndUomInfo(stat, data) {
    if (stat && data.length > 0) {
        var grid = $("#grdRequisitionDetail").data("kendoGrid");
        var currentSelection = grid.select().parent();
        var selectedItem = grid.dataItem(currentSelection)
        if (selectedItem) {
            selectedItem.Rev = data[0].Rev;
            selectedItem.Uom = data[0].Uom;
            if (selectedItem.ErrorStatus == true) {
                $('tr[data-uid="' + selectedItem.uid + '"]').addClass("highlighted-row");
            }
            else {
                $('tr[data-uid="' + selectedItem.uid + '"]').removeClass("highlighted-row");
            }
            //$("#grdRequisitionDetail").data("kendoGrid").refresh();
            var postData = { styleCode: style_cd, colorCode: color_cd, attributeCode: attr_cd, sizeCode: size_cd, revCode: selectedItem.Rev };
            postData = JSON.stringify(postData);
            ISS.common.executeActionAsynchronous("../order/GetStdCaseInfo", postData, loadStdCaseInfo);
            requisitions.validateRequisitionDetailRow(selectedItem, setgridValidateRefresh);
        }
    }
    else {
        var grid = $("#grdRequisitionDetail").data("kendoGrid");
        var currentSelection = grid.select().parent();
        var selectedItem = grid.dataItem(currentSelection)
        if (selectedItem) {
            selectedItem.Rev = "";
            selectedItem.Uom = "";
            $("#grdRequisitionDetail").data("kendoGrid").refresh();
        }
        ISS.common.notify.error('Failed to retrieve Uom details.');
    }
}

function loadStdCaseInfo(stat, data) {
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
        ISS.common.notify.error('Failed to get Std Case details.');
    }
}

function isNumber(evt) {
    var qty = evt.value;
    var index = -1;
    if (qty != null && qty != "") {
        index = qty.indexOf('-');
    }
   
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (
        ((charCode != 45 || index != -1) &&     // “-” CHECK MINUS, AND ONLY ONE.
        (charCode < 48 || charCode > 57))       ) {
        if (event.preventDefault) event.preventDefault();
        else {
            if(event.returnValue)  event.returnValue = false;
            return false;
        }

    }

    return true;
}

