
ISS = {};

var serverDate = null;
$(document).ready(function () {
    $(".cb").each(function (i, element) {
        $(element).customCheckBox();
    });
    ISS.common.initMenuSlide();
    ISS.common.notify = $("#popupNotification").kendoNotification({
        animation: {
            close: {
                effects: "slideIn:left",
                reverse: true
            }
        },
        position: {
            top: 300,
            left: Math.floor($(window).width() / 2 - 110),
            bottom: 0,
            right: 0
        },

    }).data("kendoNotification");

    ISS.common.progressBar = $("#progressbar").kendoProgressBar({
        min: 0,
        type: "percent",      
        animation: {
            duration: 300
        }

    }).data("kendoProgressBar");

    

    $('.datefor').append('<span class="datefordis"> (MM/DD/YYYY)</span>')

    ISS.common.handleFocusOut('.showpopupMsgCancel', '.btnshowconfOk');
       

    $(document).keydown(function (e) {
        var preventKeyPress;
        if (e.keyCode == 8) {
            var d = e.srcElement || e.target;
            switch (d.tagName.toUpperCase()) {
                case 'TEXTAREA':
                    preventKeyPress = d.readOnly || d.disabled;
                    break;
                case 'INPUT':
                    preventKeyPress = d.readOnly || d.disabled ||
                        (d.attributes["type"] && $.inArray(d.attributes["type"].value.toLowerCase(), ["radio", "checkbox", "submit", "button"]) >= 0);
                    break;
                case 'DIV':
                    preventKeyPress = d.readOnly || d.disabled || !(d.attributes["contentEditable"] && d.attributes["contentEditable"].value == "true");
                    break;
                default:
                    preventKeyPress = true;
                    break;
            }
        }
        else
            preventKeyPress = false;

        if (preventKeyPress)
            e.preventDefault();
    });

    $('form').on('keypress', function (e) {
        if (e.which == 13) {
            var tg=$(e.target)
            if (tg.hasClass('k-dropdown')) {
                var ddl = tg.find('input[data-role="dropdownlist"]').data("kendoDropDownList")
                if (ddl) {
                    ddl.open();
                    return false;
                }
            }
            else if (tg.data('role') == 'datepicker') {
                var dp = tg.data("kendoDatePicker")
                if (dp) {
                    dp.open();
                    return false;
                }
            }
            else if (tg.attr('role')=='combobox') {
                var ddl = tg.parent().next().data("kendoComboBox")
                if (ddl) {
                    ddl.open();
                    return false;
                }
            }
            else if (tg.attr('role') == 'listbox') {
                var ddl = tg.parent().next().data("kendoMultiSelect")
                if (ddl) {
                    ddl.open();
                    return false;
                }
            }
            
        }
       
    });

}).ajaxSend(function (xhr, er, settings) {

}).ajaxComplete(function (xhr, er, settings) {

}) 



ISS.common = {

    notify: null,
    progressBar: null,
    commonPopup: null,
    eventCont: null,
    popupIdx:0,
    menuEvent: function () {
    },
    Settings: {
        height: function () {
            return ($(window).height() - 200);
        },
        width: function () {
            return ($(window).width() - 300);
        },
        Mode: "F",
        speed: 500,
        NeedMenuSliding: true,
        CurrentDate: null,
        DateDiplay: 'MM/dd/yyyy',
        RetOrderInit: false,
        Currentgrid: null,
        Animation:true,

    },
    Loader: '<div class="k-loading-image"></div>',
    Message: {
        LoadFailed: "Failed to load records"

    },
    MsgType: {
        Error: 'Error',
        Warning: 'Warning',
        Success: 'Success',
        Info: 'Information',
        Confirm: 'Confirm',
    },
    const: {
        AttributeInd: 'Y'
    },

    validateComboChange:function(c,msg){
        if (c.value() != '') {
            var d=c.dataSource.data();
            for (i = 0; i < d.length; i++) {
                if (c.value() == d[i][c.options.dataValueField]) {
                    return true;
                }
            }
            c.value(null);
            if(msg)
                ISS.common.notify.error(msg);
            c.focus();
            return false;
        }
        return true;
    },

    handleFocusOut:function(current,target,type){
        $(current).data('foctgt', target).data('foctgttype', type).on('keydown', function (e) {
            if (e.keyCode == 9 && ! e.shiftKey) {
                th = $(this);
                var c = th.data('foctgt');
                var t = th.data('foctgttype');
                if (c && t) {
                    setTimeout(function () { $(c).data(t).focus() },10);
                }
                else if (c) {
                    setTimeout(function () { $(c).focus() }, 10);
                }
                else setTimeout(function () { th.focus() }, 10);
                if (e.preventDefault) e.preventDefault();
                else if (e.cancelable) e.cancelable = true;
                return false;
            }
        });
    },

    clearGridFilters: function (grid) {
        if (grid.dataSource._filter != null && grid.dataSource._filter.filters.length > 0)
            grid.dataSource.filter([]);
        if (grid.dataSource._sort && grid.dataSource._sort.length > 0)
            grid.dataSource.sort([])
        if (grid.dataSource.page() > 1) grid.dataSource.page(1);
    },

    getFilter: function (logic) {
        return filt = {
            filters: new Array(),
            logic: logic ? logic : "and"
        };
    },
    getFilterItem: function (field, operator, value) {
        return { field: field, operator: operator, value: value };
    },
    parseDate: function (v) {
        if (v != null) {
            if (v.indexOf('-') != -1) {
                var date = v.split('T')[0].split('-');
                return new Date(date[0], date[1] - 1, date[2]);
            }
            return new Date(parseInt((v.replace('/', '').replace('/', '')).replace('Date', '').replace('(', '').replace(')', '')))
        }
    },
    isEqualDate: function (d, s) {

        return (d.getFullYear() == s.getFullYear() &&
          d.getMonth() == s.getMonth() &&
          d.getDate() == s.getDate());
    },
    getMinDate: function (l) {
        if (l.length > 0) {
            var d = l[0];
            for (i = 1; i < l.length; i++) {
                if (d > l[i]) d = l[i];
            }
            return d;
        }
        return null;
    },

    initMenuSlide: function () {


        $('.menufixer').click(function () {
            ISS.common.toggleLeftMenu($(this), true);
        })


        if (ISS.common.Settings.NeedMenuSliding) { //
            ISS.common.toggleLeftMenu($('.menufixer'));
        }

    },

    dataBoungGrid:function (){
        this.table.find('.k-grid-Select').first().focus()
    },

    toggleLeftMenu: function (fixer, choose) {
        $lf = $('.leftMenu');
        var stat = $lf.css('display');
        $lf.animate({ width: 'toggle' }, ISS.common.speed);
        if (stat != 'none') {
            ISS.common.collapseMenu(ISS.common.speed, fixer);
            if (choose) ISS.common.setMenuVal(true);
            //$('.navRight').off('click', ISS.common.initMenuSlide)
        }
        else {
            ISS.common.expandMenu(ISS.common.speed, fixer)
            if (choose) ISS.common.setMenuVal(false);
            // $('.navRight').one('click', ISS.common.initMenuSlide)
        }

    },
     
    SelectUsingShifytKey: function (e, callback, grid) {
        if ((e.shiftKey == true && e.keyCode == 40) || (e.shiftKey == true && e.keyCode == 38)) {
            var arrows = [38, 40];
            if (arrows.indexOf(e.keyCode) >= 0) {
                if (!grid) {
                    grid = ISS.common.Settings.Currentgrid;
                }
                var row = $(grid.current()).closest("tr").closest("tr");

                if (row.hasClass('k-state-selected')) {
                    if (e.keyCode == 38) {
                        var id = row.next().removeClass('k-state-selected').data('uid');
                        if (grid.lockedTable) {
                            grid.lockedTable.find("tr[data-uid='" + id + "']").removeClass('k-state-selected');
                        }
                        if (grid.table) {
                            grid.table.find("tr[data-uid='" + id + "']").removeClass('k-state-selected');
                        }
                        if (callback) callback();
                    }
                    if (e.keyCode == 40) {
                        if (row.next().length != 0) {
                            var id = row.prev().removeClass('k-state-selected').data('uid');
                            if (grid.lockedTable) {
                                grid.lockedTable.find("tr[data-uid='" + id + "']").removeClass('k-state-selected');
                            }
                            if (grid.table) {
                                grid.table.find("tr[data-uid='" + id + "']").removeClass('k-state-selected');
                            }
                        }
                        if (callback) callback();
                    }
                }
                else {
                    row.addClass('k-state-selected');
                    if (grid.lockedTable) {
                        grid.lockedTable.find("tr[data-uid='" + row.data('uid') + "']").addClass('k-state-selected');
                    }
                    if (grid.table) {
                        grid.table.find("tr[data-uid='" + row.data('uid') + "']").addClass('k-state-selected');
                    }
                    if (callback) callback();

                }
            }
        }
    },

    setMenuVal: function (v) {
        ISS.common.setCookie('menustate', v)
    },

    getMenuVal: function () {
        return ISS.common.getCookie('menustate');
    },

    setCookie: function (cname, cvalue, exdays) {
        var d = new Date();
        d.setTime(d.getTime() + (30 * 24 * 60 * 60 * 1000));
        var expires = "expires=" + d.toUTCString();
        document.cookie = cname + "=" + cvalue + "; " + expires;
    },

    getCookie: function (cname) {
        var name = cname + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') c = c.substring(1);
            if (c.indexOf(name) == 0) return c.substring(name.length, c.length);
        }
        return "";
    },

    getFilteredColumns: function (gridName) {
        var filterHtml = '';
        var grid = $(gridName).data("kendoGrid");
        if (grid != null) {
            var arr = grid.dataSource.filter();
            if (arr && arr.filters.length > 0) {
                
                arr.filters.forEach(function (filter) {
                    if (filter.field && filter.field != 'IsDeleted' && filter.field!='IsEdited') {
                        if (filterHtml=='') filterHtml = "Filtered columns : ";
                        if (grid.columns && grid.columns.length > 0) {
                            var result = $.grep(grid.columns, function (column) { return column.field == filter.field });
                            var oper = grid.options.filterable.operators[grid.dataSource.options.schema.model.fields[filter.field].type][filter.operator];
                            if (typeof filter.value == "object") {
                                var convertedStartDate = new Date(filter.value);
                                var day = (convertedStartDate.getMonth() + 1) + "/" + convertedStartDate.getDate() + "/" + convertedStartDate.getFullYear();
                                filterHtml += result[0].title + " [" + oper + "] : " + day + ", ";
                            }
                            else
                                filterHtml += result[0].title + " [" + oper + "] : " + filter.value + ", ";
                        }
                    }
                });
                filterHtml = filterHtml.trim().slice(0, -1);
            }
        }
        return filterHtml;
    },

    gridDataBoundStyle: function (e) {
        $('#FilteredColumns').html(ISS.common.getFilteredColumns("#" + e.sender.content.context.id));
    },

    expandMenu: function (speed, th) {
        th.removeClass('collapsed').attr('title', 'Collapse menu').animate({ "margin-left": "166px" }, speed, function () {
            $('.navRight').width('80%');
            ISS.common.menuEvent();
        });

    },

    collapseMenu: function (speed, th) {
        th.addClass('collapsed').attr('title', 'Expand menu').animate({ "margin-left": "0", "margin-right": "10px" }, speed, function () {
            $('.navRight').width('95%');
            ISS.common.menuEvent();
        });
    },


    getFormData: function ($form) {
        var arr = $form.serializeArray();
        var res = {};
        $.map(arr, function (n, i) {
            res[n['name']] = n['value'];
        });

        return res;
    },

    toUpperCase: function (css) {
        $(css).keyup(function () {
            var start = this.selectionStart, end = this.selectionEnd;
            $(this).val($(this).val().toUpperCase());
            this.setSelectionRange(start, end);
        });
        $(css).bind('input propertychange', function () {
            var start = this.selectionStart, end = this.selectionEnd;
            $(this).val($(this).val().toUpperCase());
            this.setSelectionRange(start, end);
        });
    },


    toInt: function (css) {
        $(css).keydown(function (e) {
            if (
                (e.shiftKey == false && e.keyCode >= 48 && e.keyCode <= 57) ||
                (e.keyCode >= 96 && e.keyCode <= 105) || (e.keyCode >= 112 && e.keyCode <= 123) ||
                e.keyCode == 8 || e.keyCode == 37 || e.keyCode == 39 || e.keyCode == 46 || e.keyCode == 13
                || e.keyCode == 36 || e.keyCode == 35
                )
                return true;
            else if (e.keyCode == 9) {
                $(this).next('input').focus();                    
            }
            else
                return false;

        });
    },

    blockUI: function (block) {
        $body = $("body");
        if (block == true) {
            $body.addClass("loading");
        }
        else {
            $body.removeClass("loading");
        }
    },

    //Need to stringfy post data
    executeActionAsynchronous: function (actionUrl, postdata, callback, type, targetElement, datatype, contenttype) {
        ISS.common.execute(actionUrl, postdata, callback, type, targetElement, datatype, (contenttype ? contenttype : "application/json; charset=utf-8"), true);
    },


    // directly pass json data
    execute: function (actionUrl, postdata, callback, type, targetElement, datatype, contenttype, async) {
        var retData = null;
        var ajx = $.ajax({
            url: actionUrl,
            data: postdata,
            type: type ? type : (postdata ? "POST" : "GET"),
            async: async == false ? false : true,
            cache: false,
            contentType: contenttype,
            dataType: datatype ? datatype : "json",
            targetElement: targetElement,
            error: function (jqXHR, textStatus, errorThrown) {
                if (callback) { callback(false, jqXHR.responseText, jqXHR); }
            },
            success: function (data, errorThrown, xhr) {
                retData = data;
                if (callback) {
                    callback(true, data, xhr);
                }
            },
        });
        return retData;
    },

    executeSynchronous: function (actionUrl, postdata, type, targetElement, datatype, contenttype) {
        ISS.common.execute(actionUrl, postdata, null, type, targetElement, datatype, contenttype, false);
    },

    addPopupClass: function () {
        var cl='popupmode' +( ISS.common.popupIdx++ )
        $('head').append($('<style type = "text/css"> .' + cl + '{overflow: hidden;}</style>'));
        return cl;
    },

    popUp: function (id, title, url, closeCallback, openCallback) {
        var settings = {
            title: title,
            //  : true,
            content: url,
            close: closeCallback,
            open: openCallback,
            //  height: '400px',
            // width: '800px',
        };
        return ISS.common.popUpCustom(id, settings);
    },

    popUpCustom: function (id, settings, max) {
        var kwindow = $(id);
        var close = settings.close;
        var open = settings.open;
        var popModeCl = ISS.common.addPopupClass();
        var params = {
            width: (settings.width ? settings.width : ISS.common.Settings.width()),
            height: (settings.height ? settings.height : ISS.common.Settings.height()),
            //animation: (settings.animation ? settings.animation : ISS.common.Settings.Animation), CA#397434-16
            actions: ((!max) ? ["Maximize", "Close"] : ["Close"]),
            modal: true,

        };
        if (settings.animation == false) {
            params.animation = settings.animation
        }

        $.extend(params, settings);
        params.close = function (e) {         
            $('body').removeClass(popModeCl);
            this.restore();
            if (close)
                close(e,popModeCl);
           
        }
        params.open = function (e) {
            $('body').addClass(popModeCl);
            if (open)
                open(e);
        }

        kwindow.kendoWindow(params);
        kwindow = kwindow.data("kendoWindow").center().open();
        kwindow.title(settings.title)
        return kwindow;
    },
    //Reference
    //===================================
    //title: "Title",
    //actions: ["Refresh", "Close"],
    //open: onOpen,
    //activate: onActivate,
    //close: onClose,
    //deactivate: onDeactivate,


    showPopUpMessage: function (msg, head, callback, ok) {
        var title = (head) ? head : ISS.common.MsgType.Error;
        var elm = $('#showpopupMsg');
        elm.find('.content').html(msg);

        function CPclosed() {
            elm.find('.showpopupMsgOk').unbind('click');
            if (callback) setTimeout(callback);
        }
        if (ok) elm.find('.showpopupMsgOk').val(ok)
        elm.find('.showpopupMsgOk').bind('click', function () {
            elm.data("kendoWindow").close()
        });

        var settings = {
            title: title,
            height: 'auto',
            width: '400px',
            //animation: ISS.common.Settings.Animation,
            close: CPclosed

        };
        if (ISS.common.Settings.Animation == false) {

            settings.animation = ISS.common.Settings.Animation;

        }
        ISS.common.popUpCustom('#showpopupMsg', settings, true);
        elm.find('.showpopupMsgOk').focus()
    },

    showConfirmMessage: function (msg, head, callback, yes, no) {
        var flag = false;
        var title = (head) ? head : ISS.common.MsgType.Confirm;

        var elm = $('#showConfMsg');
        elm.find('.content').html(msg);

        function CMCompleted(ret) {
            elm.find('.showpopupMsgOk').unbind('click');
            elm.find('.showpopupMsgCancel').unbind('click');
            if (flag && callback) setTimeout(function () { callback(ret) },0);
        }
        yes = yes ? yes : 'Yes';
        no = no ? no : 'No';


        elm.find('.showpopupMsgOk').val(yes);
        elm.find('.showpopupMsgCancel').val(no);

        elm.find('.showpopupMsgOk').bind('click', function () {
            flag = true;
            elm.data("kendoWindow").close();
            CMCompleted(true);
        });
        elm.find('.showpopupMsgCancel').bind('click', function () {
            flag = true;
            elm.data("kendoWindow").close()
            CMCompleted(false);
        });
        var settings = {
            title: title,
            height: 'auto',  // (elm.height() + 100),
            width: '400px',
            //animation: ISS.common.Settings.Animation,
            close: function () {
                if (!flag)
                    CMCompleted(false);
            }
        };
        if (ISS.common.Settings.Animation == false) {

            settings.animation = ISS.common.Settings.Animation;

        }
        ISS.common.popUpCustom('#showConfMsg', settings, true)
        elm.find('.showpopupMsgOk').focus()
    },
    collapsePanel: function (item, idx) {
        var panel = $(item).data('kendoPanelBar');
        if (panel) {
            var item = panel.select(panel.element.children("li").eq(idx));
            panel.collapse(panel.select());
        }
    },
    expandPanel: function (item, idx) {
        var panel = $(item).data('kendoPanelBar');
        if (panel) {
            var item = panel.select(panel.element.children("li").eq(idx));
            panel.expand(panel.select());
        }
    },

    CommonSearchDataBound: function (e) {
        $(".k-grid-Select").find("span").addClass("k-icon k-i-tick");
        $('#FilteredCommonSearch').html(ISS.common.getFilteredColumns("#" + e.sender.content.context.id));
        if (e.sender.settings && e.sender.settings.DataBound) {
            e.sender.settings.DataBound(e.sender);
        }
        var pfsCol = this.table.find('.PFSChkpopup [type="checkbox"]').first();
        if (pfsCol)
            pfsCol.focus();
        
        if(e.sender.settings.AllowSelect)
            this.table.find('.k-grid-Select').first().focus();
    },

    CommonSearchSelected: function (e) {

        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        ISS.common.commonPopup.close();
        if (ISS.common.commonPopup.handler) ISS.common.commonPopup.handler(dataItem);
        return false;
    },

    getCommonSearchGrid: function (settings, callback) {

        var postData = {
            search: {
                Columns: settings.columns,
                AllowSelect: settings.AllowSelect,
                GridName: settings.GridName,
                HideFilter: settings.HideFilter,
                HidePager: settings.HidePager
            }
        }
        ISS.common.executeActionAsynchronous(ISS.common.Settings.urlCommonSearch, JSON.stringify(postData), function (ret, viewdata) {
            if (ret && viewdata) {
                ISS.common.commonPopup.handler = settings.handler;
                if (settings.dataSource) {
                    $(settings.targetElement).empty().html(viewdata)
                    var grid = $(settings.targetElement + ' #' + ((settings.GridName) ? settings.GridName : "grdCommonSearch")).data("kendoGrid");
                    grid.settings = settings;
                    grid.dataSource.data(settings.dataSource);
                    if (callback) callback(grid);
                }
                else {
                    ISS.common.executeActionAsynchronous(settings.url, JSON.stringify(settings.postData), function (ret, data) {
                        if (ret && data) {
                            $(settings.targetElement).empty().html(viewdata)
                            var grid = $(settings.targetElement + ' #' + ((settings.GridName) ? settings.GridName : "grdCommonSearch")).data("kendoGrid");
                            grid.settings = settings;
                            grid.dataSource.data((data.Data) ? data.Data : data);
                            if (callback) callback(grid);
                        }
                    }); // data call
                }
            }
        }, null, null, 'text'); // grid call

    },

    CommonSearchShow: function (settings) {
        settings.targetElement = settings.targetElement || '.cdivCommonSearch';

        $(settings.targetElement).empty().html(ISS.common.Loader);
        ISS.common.getCommonSearchGrid(settings);
        ISS.common.commonPopup = ISS.common.popUp(".cdivCommonSearch", settings.title, null, settings.close)
        ISS.common.commonPopup.title(settings.title);

    },

    cloneAndStore: function (data) {
        if (!data.Cloned) {
            var obj = new Object()
            for (prop in data) {
                if (prop != 'fields' && prop != 'defaults')
                    obj[prop] = data[prop];
            }
            data.Cloned = obj;
        }
    },

    highlightEditedCells: function (data, row, columns, exclude, clon) {
        clon = clon == false ? false : true;
        if (clon) ISS.common.cloneAndStore(data);
        var TCnt = row.find('td[role="gridcell"]').size();
        if (row.length > 1) {
            TCnt = row.first().find('td[role="gridcell"]').length
        }
        if (data.Cloned) {
            $(columns).each(function (id, item) {
                if (item.field) {
                    if (
                        ((data.Cloned[item.field] instanceof Date)?
                        (data.Cloned[item.field].getTime() != data[item.field].getTime()) :
                        (data.Cloned[item.field] != data[item.field]) 
                        ) &&
                        (!(data.Cloned[item.field] == null && data[item.field] == ''))
                        && $.inArray(item.field, exclude) < 0) {
                        var col = null;
                        if (id < TCnt) {
                            col = row.first().find('td :eq(' + id + ')');
                        }
                        else {
                            col = row.eq(1).find('td :eq(' + (id - TCnt) + ')');
                        }

                        if (col.find('.k-dirty').length == 0) {
                            col.prepend('<span class="k-dirty"></span>')
                        }
                    }
                }
            });
        }
    },
    equals: function (val1, val2) {
        return ((val1 == null && val2 == '') || (val1 == '' && val2 == null) || (val1 == val2));
        // (isNull(val1) && isNull(val2) )|| val1==val2
    },
    isNull: function (val) {
        return (val == null || val == '');
    },
    scrollTo: function (elm, targ) {
        $(((targ) ? targ : "html, body")).animate({ scrollTop: $(elm).offset().top }, 1000);
    },
    
     
    updateProgressBar: function (val, Msg) {       
        setTimeout(
            function () { ISS.common.progressBar.value(val) }, 0);

        $('#lblProgressStatus').html(Msg);
    },

    
    showProgressBar: function (start, max,msg) {
        $body = $("body");
        if (start) {
            ISS.common.progressBar.options.max = max;
            $body.addClass("Pbloading");
            ISS.common.progressBar.value(0);                       
            $('#lblProgressStatus').html(msg);
        }
        else {
            $('#lblProgressStatus').html('');
            $body.removeClass("Pbloading");
        }
    },

    IsAttributed: function (attribute) {
        var isAttrib = false;
        if (attribute == ISS.common.const.AttributeInd) {
            isAttrib = true;
        }

        return isAttrib;
    },

    eachesQtyRestriction: function (css) {
        $("body").on("keypress", css, function (evt) {
            var qty = $(this).val();
            var index = -1;
            if (qty != null && qty != "") {
                index = qty.indexOf('-');
            }
            var charCode = (evt.which) ? evt.which : event.keyCode
            if (
                ((charCode != 45 || index != -1) &&     // “-” CHECK MINUS, AND ONLY ONE.
                (charCode < 48 || charCode > 57))) {
                if (event.preventDefault) event.preventDefault();
                else {
                    if (event.returnValue) event.returnValue = false;
                    return false;
                }
            }
            return true;
        });
        ISS.common.eachesQtyChanged(css);
    },

    getQtyToEachDisplay: function (q) {
        
        var d = ISS.common.getEaches(q)
        if (d == 0) { if (q.search("-00") == -1) return q + '-00'; }
        else if (d == 10) { if (q.search("-10") == -1) return q + '0'; }
        return q;
    },

    convertQtyToEach :function(q) {
        return parseInt(q) *12 + ISS.common.getEaches(q)
    },


    convertEachToDecimal: function (val) {
        var intPart = parseInt(val/12);
        var decimalPart = val - intPart*12;
        if (decimalPart < 10)
            return intPart +'-0'+decimalPart;
         else   if (decimalPart >=10)
             return intPart +'-'+decimalPart;   
    },

    getEaches: function (val) {
        val = (val+'').replace('-', '.');
        var intPart = parseInt(val);
        var decimalPart = val - intPart;
        var result = Math.round(((decimalPart) * 100));
        return parseInt(result);
    },

    eachesQtyChanged: function (css) {
        $("body").on("focusout", css, function (e) {
            if (e.currentTarget.value == "")
                return false;

            var qty = e.currentTarget.value;
            var s = qty.replace('-', '.');

            if (!$.isNumeric(s))
                s = "0.00";

            s = ISS.common.replaceAndCheck(s);
            if (ISS.common.getEaches(s) == 10) {
                if (s.indexOf('.') == -1) s += '.';
                while (s.length < s.indexOf('.') + 3) s += '0';
                s = s.replace('.', '-');
                e.currentTarget.value = s;
                return false;
            }
            else if (ISS.common.getEaches(s) >= 12) {
                ISS.common.showPopUpMessage('Eaches must be less than 12.', null, function () {
                    e.currentTarget.value = parseInt(s) + "-00";
                    e.currentTarget.focus();
                    return false;
                });

            }

            var index = -1;
            if (s != null && s != "") {
                index = s.indexOf('.');
            }


            if (index == -1) {
                e.currentTarget.value = s + "-00";
            }
            else
                e.currentTarget.value = s.replace('.', '-');

            return true;
        });
    },

    replaceAndCheck: function (s) {
        s = $.trim(s.replace("-", "."));
        if (s.indexOf('.') == 0) s = '0' + s;
        var arr = s.split('.');
        if (arr.length > 1 && arr[1].length > 2) {
            s = arr[0] + '.' + arr[1].substring(0, 2)
        }
        if (s != "" && !s.match(/^\d+(\.\d{1,2})?$/)) {
            s = arr[0];
        }
        return s;
    },

    multiselectAll: function (multiselectname,Value) {
        var required = $("#" + multiselectname + "").data("kendoMultiSelect");
        var selectedValues = "";
        var strComma = "";
        var selectedValueUnitLength = "";
        selectedValueUnitLength = required.element[0].length;
        if (selectedValueUnitLength > 0) {
            for (var i = 0; i < required.element[0].length; i++) {
                //var item = required.element[0][i].innerText
                var item = required.element[0][i].value;
                if (Value == "") {
                    selectedValues += strComma + item;
                }
                else {                    
                    selectedValues += strComma + item;
                }
                strComma = ",";
            }

        }

        required.value(selectedValues.split(",")); 
        return false;
    },

    //multiselectAll: function (multiselectname, Value) {
    //    var required = $("#" + multiselectname + "").data("kendoMultiSelect");
    //    var selectedValues = "";
    //    var strComma = "";
    //    for (var i = 0; i < required.dataSource.data().length; i++) {
    //        var item = required.dataSource.data()[i]
    //        if (Value == "") {
    //            selectedValues += strComma + item;
    //        }
    //        else {
    //            selectedValues += strComma + item[Value];
    //        }
    //        strComma = ",";
    //    }
    //    required.value(selectedValues.split(","));
    //    return false;
    //},

    multiunselectAll: function (multiselectname) {
        var required = $("#" + multiselectname + "").data("kendoMultiSelect");
        required.value([]);
        return false;
    },
   
};

//settings = {
//    columns: [{
//        Name: "BusinessUnit",
//        Title: "Business Unit",
//        width: 200
//    }, {
//        Name: "PlannerCd",
//        Title: "Planner Cd"
//    }],
//    AllowSelect: false,
//    title: 'sample',
//    url: '../shared/GetBusinessContact',
//    postData: {},
//    dataSource:null,
//    targetElement :null,
//    handler: function (d) {
//        var tt = 21;
//    },
//    close: null,
//};
//ISS.common.CommonSearchShow(settings);

//Custom Jquery function
(function ($) {

    $.fn.customCheckBox = function () {

        var checkboxClasses = {
            customClass: 'customCheckbox',
            checkedClass: 'cb2',
            uncheckedClass: 'cb1',
            checkedClassDisabled: 'cb2-disabled',
            uncheckedClassDisabled: 'cb1-disabled'
        }
        if ($(this).parent("." + checkboxClasses.customClass).length == 0) {
            var checkboxSpan = $(this).wrap('<span class="' + checkboxClasses.customClass + '" />');
            $(this).css("opacity", "0");
            $(this).attr('autocomplete', 'off');
            if ($(this).parent("." + checkboxClasses.customClass).length > 0) {
                if ($(this).prop('disabled') == false) {
                    if ($(this).prop('checked') == true) {
                        $(this).parent("." + checkboxClasses.customClass).addClass(checkboxClasses.checkedClass);
                    } else {
                        $(this).parent("." + checkboxClasses.customClass).addClass(checkboxClasses.uncheckedClass);
                    }
                } else {
                    if ($(this).prop('checked') == true) {
                        $(this).parent("." + checkboxClasses.customClass).addClass(checkboxClasses.checkedClassDisabled);
                    } else {
                        $(this).parent("." + checkboxClasses.customClass).addClass(checkboxClasses.uncheckedClassDisabled);
                    }
                }
            }
            if ($(this).prop('disabled') == false) {

                $(this).change(function () {
                    if (!$(this).prop('readonly')) {
                        if ($(this).prop('checked') == true) {
                            // check
                            $(this).parent("." + checkboxClasses.customClass).removeClass(checkboxClasses.uncheckedClass);
                            $(this).parent("." + checkboxClasses.customClass).addClass(checkboxClasses.checkedClass);

                        } else {
                            // uncheck
                            $(this).parent("." + checkboxClasses.customClass).removeClass(checkboxClasses.checkedClass);
                            $(this).parent("." + checkboxClasses.customClass).addClass(checkboxClasses.uncheckedClass);
                        }
                    }
                });

            }
        }
    },

    $.fn.check = function () {
        $(this).each(function (Index) { $(this).attr("checked", "true").trigger('change'); });

        return this;
    },
    $.fn.uncheck = function () {
        $(this).each(function (Index) { $(this).removeAttr("checked").trigger('change'); });

        return this;
    }

    


})(jQuery);