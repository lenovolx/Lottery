$.extend($.fn.datagrid.methods, {
    editCell: function (jq, param) {
        return jq.each(function () {
            var opts = $(this).datagrid('options');
            var fields = $(this).datagrid('getColumnFields', true).concat($(this).datagrid('getColumnFields'));
            for (var i = 0; i < fields.length; i++) {
                var col = $(this).datagrid('getColumnOption', fields[i]);
                col.editor1 = col.editor;
                if (fields[i] != param.field) {
                    col.editor = null;
                }
            }
            $(this).datagrid('beginEdit', param.index);
            for (var i = 0; i < fields.length; i++) {
                var col = $(this).datagrid('getColumnOption', fields[i]);
                col.editor = col.editor1;
            }
        });
    }
});
function getSelectionIds(gridId) {
    var grid = gridId == null ? $("#dg") : $("#" + gridId);
    if (grid.size() === 0) return false;
    var items = grid.datagrid("getSelections");
    if (items.length > 0) {
        var idField = grid.datagrid('options').idField;
        var ids = [];
        var temp = "";
        for (var i = 0; i < items.length; i++) {
            temp = "";
            for (var j = 0; j < idField.split(',').length; j++) {
                temp += "|" + items[i][idField.split(',')[j]];
            }
            temp = temp != null ? temp.substring(1) : "";
            ids.push(temp);
        }
        return ids;
    }
    return [];
};
function getSelectionsRows(gridId) {
    var grid = gridId == null ? $("dg") : $("#" + gridId);
    if (grid.size() === 0) return false;
    var items = grid.datagrid("getSelections");
    if (items.length > 0) {
        return items;
    }
    else {
        return null;
    }
};

function getCheckedIds(gridId) {
    var grid = gridId == null ? $("dg") : $("#" + gridId);
    if (grid.size() === 0) return false;
    var items = grid.datagrid("getSelections");
    if (items.length > 0) {
        var idField = grid.datagrid('options').idField;
        var ids = [];
        var temp = "";
        for (var i = 0; i < items.length; i++) {
            temp = "";
            for (var j = 0; j < idField.split(',').length; j++) {
                temp += "|" + items[i][idField.split(',')[j]];
            }
            temp = temp != null ? temp.substring(1) : "";
            ids.push(temp);
        }
        return ids.join();
    }
    return "";
};
Date.prototype.Format = function (format) {
    var o = {
        "M+": this.getMonth() + 1, //month 
        "d+": this.getDate(),    //day 
        "h+": this.getHours(),   //hour 
        "m+": this.getMinutes(), //minute 
        "s+": this.getSeconds(), //second 
        "q+": Math.floor((this.getMonth() + 3) / 3),  //quarter 
        "S": this.getMilliseconds() //millisecond 
    };
    if (/(y+)/.test(format))
        format = format.replace(RegExp.$1,
                (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(format))
            format = format.replace(RegExp.$1,
                    RegExp.$1.length == 1 ? o[k] :
                        ("00" + o[k]).substr(("" + o[k]).length));
    return format;
};
function formatDatebox(value) {
    if (value == null || value == '') {
        return '';
    }
    var dt = parseToDate(value);//关键代码，将那个长字符串的日期值转换成正常的JS日期格式
    return dt.Format("yyyy-MM-dd"); //这里用到一个javascript的Date类型的拓展方法，这个是自己添加的拓展方法，在后面的步骤3定义
}
/*带时间*/
function formatDateBoxFull(value) {
    if (value == null || value == '') {
        return '';
    }
    var dt = parseToDate(value);
    return dt.Format("yyyy-MM-dd hh:mm:ss");
}
function formatLocked(value) {
    return value === "0" ? "未锁定" : "已锁定";
}
function formatMoney(value) {
    return value.toFixed(2);
}
function formatMoneyTaxRed(value) {
    return value <= 50 ? value.toFixed(2) : "<font color='red'><b>"+value.toFixed(2)+"</b></font>";
}
function parseToDate(value) {
    if (value == null || value === '') {
        return undefined;
    }

    var dt;
    if (value instanceof Date) {
        dt = value;
    }
    else {
        if (!isNaN(value)) {
            dt = new Date(value);
        }
        else if (value.indexOf('/Date') > -1) {
            value = value.replace(/\/Date\((-?\d+)\)\//, '$1');
            dt = new Date();
            dt.setTime(value);
        } else if (value.indexOf('/') > -1) {
            dt = new Date(Date.parse(value.replace(/-/g, '/')));
        } else {
            dt = new Date(value);
        }
    }
    return dt;
}
function convert(rows) {
    function exists(rows, parentId) {
        for (var i = 0; i < rows.length; i++) {
            if (rows[i].id == parentId) return true;
        }
        return false;
    }
    var nodes = [];
    for (var i = 0; i < rows.length; i++) {
        var row = rows[i];
        if (!exists(rows, row.parentId)) {
            nodes.push({
                id: row.id,
                text: row.name
            });
        }
    }
    var toDo = [];
    for (var i = 0; i < nodes.length; i++) {
        toDo.push(nodes[i]);
    }
    while (toDo.length) {
        var node = toDo.shift();
        for (var i = 0; i < rows.length; i++) {
            var row = rows[i];
            if (row.parentId == node.id) {
                var child = { id: row.id, text: row.name };
                if (node.children) {
                    node.children.push(child);
                } else {
                    node.children = [child];
                }
                toDo.push(child);
            }
        }
    }
    return nodes;
}
function RefushTab(_title) {
    if ($('#tt').tabs('exists', _title)) {
        var currTab = $('#tt').tabs('getTab', _title),
            iframe = $(currTab.panel('options').content),
            content = '<iframe scrolling="auto" frameborder="0"  src="' + iframe.attr('src') + '" style="width:100%;height:100%;"></iframe>';
        $('#tt').tabs('update', {
            tab: currTab,
            options: {
                content: content,
                closable: true
            }
        });
    }
}
function EasyLoading() {
    $("<div class=\"datagrid-mask\"></div>").css({ display: "block", width: "100%", height: $(window).height() }).appendTo("body");
    $("<div class=\"datagrid-mask-msg\"></div>").html("正在处理，请稍候。。。").appendTo("body").css({ display: "block", left: ($(document.body).outerWidth(true) - 190) / 2, top: ($(window).height() - 45) / 2 });
}
function EasyLoadEnd() {
    $(".datagrid-mask").remove();
    $(".datagrid-mask-msg").remove();
}
String.prototype.format = function () {
    var args = arguments;
    return this.replace(/\{(\d+)\}/g,
        function (m, i) {
            return args[i];
        });
};
