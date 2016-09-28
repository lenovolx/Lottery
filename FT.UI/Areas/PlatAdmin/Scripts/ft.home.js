$(function () {
    home.loadLanguage($.cookie('FT.PlatAdmin.Langue'));
    var firstMenu = $(".easyui-accordion").children("div:first").children().eq(1).find("a");
    home.addTab(firstMenu.attr("menuname"), firstMenu.attr("menurl"), firstMenu.attr("menuicon"), firstMenu.attr("menuid"));
    $("#logout").on("click", function () {
        $.messager.confirm($.i18n.prop('common_systips_text'), $.i18n.prop('common_exitsys_title'), function (r) {
            if (r) {
                window.location.href = "/PlatAdmin/Login/Off";
            }
        });
    });
    setTimeout(home.getAdminInformation, 300);
});
var home = home || {};
var url = "";
(function () {
    home.loadLanguage = function (language) {
        jQuery.i18n.properties({
            name: 'language',
            path: '/i18n/admin/',
            mode: 'map',
            language: language,
            callback: function () {
                $("#editpass").text($.i18n.prop('home_modifypass_text'));
                $("#logout").text($.i18n.prop('home_safeout_text'));
                $(".opass").text($.i18n.prop('home_currentpass_text'));
                $(".npass").text($.i18n.prop('home_newpass_text'));
                $(".cpass").text($.i18n.prop('home_confirmpass_text'));
                $("#btn_ok").text($.i18n.prop('common_ok_text'));
                $("#btn_cancel").text($.i18n.prop('common_cancel_text'));
                $(".panel-header").children("div:first").html($.i18n.prop('home_sysmenu_text'));
            }
        });
        switch (language) {
            case 'en':
                $('.language-text').text('English');
                break;
            case 'pt':
                $('.language-text').text('Portugal');
                break;
            default:
                $('.language-text').text('简体中文');
                break;
        }
    };
    home.addTab = function(title, url, icon, menuid) {
        $('#tt').tabs({
            onSelect: function(title) {
                var p = $(this).tabs('getTab', title);
                var url = p.find('iframe').attr('src');
                p.find('iframe').attr('src', url);
            }
        });
        if ($('#tt').tabs('exists', title)) {
            $('#tt').tabs('select', title);
        } else {
            var content = '<iframe scrolling="auto" id="' + icon + '" frameborder="0"  src="' + url + '?MenuId=' + menuid + '" style="width:100%;height:100%;"></iframe>';
            $('#tt').tabs('add', {
                title: title,
                content: content,
                closable: true,
                icon: icon,
                selected: true
            });
        }
    };
    home.modifyPassword = function() {
        $('#dlgchangePassword').dialog('open').dialog('setTitle', '修改密码');
        $('#fmchangePassword').form('clear');
        url = '/PlatAdmin/System/ModifyPassword';
    };
    home.saveForm = function() {
        $('#fmchangePassword').form('submit', {
            url: url,
            onSubmit: function () {
                return $(this).form('validate');
            },
            success: function (result) {
                result = $.parseJSON(result);
                if (result.code != 0) {
                    $.messager.show({
                        title: '系统提示',
                        msg: result.errors
                    });
                } else {
                    $.messager.show({
                        title: '系统提示',
                        msg: '密码修改成功'
                    });
                    $('#dlgchangePassword').dialog('close');
                }
            }
        });
    };
    home.getAdminInformation = function () {
        var temphtml = "<a href=\"javascript:void(0)\" style=\"display: inline;\">{0}</a> {1}";
        var html = "";
        $.getJSON("/PlatAdmin/Partial/AdminInformation?_=" + Math.random(), function(data) {
            if (data) {
                html = temphtml.format(data.LoginName, "");
                if (data.AgentLevel !== 0)
                    html = temphtml.format(data.LoginName, "信用额度 <font color=\"red\"><b class=\"Amount\"> " + data.CreditLimit.toFixed(2) + " </b></font>");
                $(".loyout_login").html(html);
            }
        });
    };
})();