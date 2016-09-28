var languestorage = {
    langue: app.siteConfig.sitelanguage
};
$(function () {
    "use strict";
    bank.loadLanguage();
    $(document).on("pageInit", "#banklist", function (e, id, page) {
        bank.loadUserBank();
        //银行卡绑定
        $(page).find('.add-bank').on('click', function () {
            bank.popopOperate($.i18n.prop('bank_li_text'), 'add', this);
        });
    });
    $.init();
});
var bank = bank || {};
(function () {
    bank.loadLanguage = function () {
        Zepto.i18n.properties({
            name: 'language',
            path: '/i18n/mobile/',
            mode: 'map',
            language: app.siteConfig.sitelanguage,
            callback: function () {
                $(".title").html($.i18n.prop('bank_li_text'));
                $(".add").html($.i18n.prop('bank_add_title'));
            }
        });
    };
    bank.popopOperate = function(title, type, obj) {
        var popupdata = {
            title: title,
            type: type,
            account: app.siteConfig.siteusername,
            tltRealname: $.i18n.prop('bank_username_text'),
            plcRealname: $.i18n.prop('bank_username_place'),
            tltType: $.i18n.prop('bank_typename_text'),
            plcType: $.i18n.prop('bank_typename_place'),
            tltBranch: $.i18n.prop('bank_branch_text'),
            plcBranch: $.i18n.prop('bank_branch_place'),
            tltBankNum: $.i18n.prop('bank_cardnum_text'),
            plcBankNum: $.i18n.prop('bank_cardnum_place'),
            btnDelete: $.i18n.prop('common_delete_text'),
            btnConfirm: $.i18n.prop('common_confirm_text')
        };
        if (type === 'edit') {
            popupdata.realname = $(obj).attr('data-realname');
            popupdata.banktype = $(obj).attr('data-banktype');
            popupdata.bankcardnum = $(obj).attr('data-bankcardnum');
            popupdata.bankbranch = $(obj).attr('data-bankbranch');
            popupdata.bankid = $(obj).attr('data-id');
        }
        var html = ["<header class=\"bar bar-nav\">"];
        html.push("<a class=\"button button-link button-nav pull-left close-popup\"><span class=\"icon icon-close\"></span></a><h1 class=\"title\"><span> {{title}}</span></h1>");
        html.push("</header>");
        html.push("<div class=\"content\">");
        html.push("<div class=\"list-block\">");

        html.push("<ul>");
        html.push("<li style=\"margin-top:1rem;\">");
        html.push("<div class=\"item-content\" > ");
        html.push("<div class=\"item-inner recharge_card\">");
        html.push("<div class=\"item-title label\">{{tltRealname}}：</div>");
        html.push("<div class=\"item-input\">");
        html.push("    <input type=\"text\" placeholder=\"{{plcRealname}}\" value=\"{{realname}}\" class=\"realname\" />");
        html.push("    <input type=\"hidden\" value=\"{{bankid}}\" class=\"bankid\" />");
        html.push("   </div>");
        html.push(" </div>");
        html.push("  </div>");
        html.push(" </li>");
        html.push(" <li>");
        html.push(" <div class=\"item-content\">");
        html.push("   <div class=\"item-inner recharge_card no-bordert\">");
        html.push("      <div class=\"item-title label\">{{tltType}}：</div>");
        html.push("     <div class=\"item-input\">");
        html.push("          <input type=\"text\" placeholder=\"{{plcType}}\" value=\"{{banktype}}\" class=\"banktype\" />");
        html.push("   </div>");
        html.push("   </div>");
        html.push("    </div>");
        html.push("  </li>");
        html.push(" <li>");
        html.push("  <div class=\"item-content recharge_card no-bordert\">");
        html.push("     <div class=\"item-inner\">");
        html.push("       <div class=\"item-title label\">{{tltBranch}}：</div>");
        html.push("    <div class=\"item-input\">");
        html.push("        <input type=\"text\" placeholder=\"{{plcBranch}}\" value=\"{{bankbranch}}\" class=\"bankbranch\" />");
        html.push("  </div>");
        html.push("  </div>");
        html.push(" </div>");
        html.push("  </li>");
        html.push(" <li>");
        html.push("    <div class=\"item-content recharge_card no-bordert\">");
        html.push("     <div class=\"item-inner\">");
        html.push("         <div class=\"item-title label\">{{tltBankNum}}：</div>");
        html.push("       <div class=\"item-input\">");
        html.push("         <input type=\"number\" placeholder=\"{{plcBankNum}}\" value=\"{{bankcardnum}}\" class=\"bankcardnum\" />");
        html.push("     </div>");
        html.push("  </div>");
        html.push(" </div>");
        html.push("</li>");

        html.push("</ul>");
        html.push("</div>");
        html.push("<div class=\"content-block\">");
        html.push("<div class=\"row\">");
        if (type === 'edit') {
            html.push("<div class=\"col-50\"><a href=\"javascript:void(0);\" class=\"button button-big button-fill\" onclick=\"bank.delUserBank('{{bankid}}')\">{{btnDelete}}</a></div>");
            html.push("<div class=\"col-50\"><a href=\"javascript:void(0);\" class=\"button button-big button-fill button-success\" onclick=\"bank.operateUserBank('{{type}}')\">{{btnConfirm}}</a></div>");
        } else {
            html.push("<a href=\"javascript:void(0);\" class=\"button button-big button-fill button-success\" onclick=\"bank.operateUserBank('{{type}}')\">{{btnConfirm}}</a>");
        }
        html.push("</div>");
        html.push("</div>");
        var chargetemplate = html.join("");
        var render = template.compile(chargetemplate);
        $(".popup-userbank").html("").html(render(popupdata));
        $.popup('.popup-userbank');
    };
    bank.operateUserBank = function(type) {
        var realname = $('.realname').val();
        var banktype = $('.banktype').val();
        var bankcardnum = $('.bankcardnum').val();
        var bankbranch = $('.bankbranch').val();
        if ($.trim(realname) === '') {
            $.toast($.i18n.prop('bank_username_place'));
            return;
        }
        if ($.trim(banktype) === '') {
            $.toast($.i18n.prop('bank_typename_place'));
            return;
        }
        if ($.trim(bankcardnum) === '') {
            $.toast($.i18n.prop('bank_branch_place'));
            return;
        }
        if ($.trim(bankbranch) === '') {
            $.toast($.i18n.prop('bank_cardnum_place'));
            return;
        }
        var posrurl = app.siteConfig.adduserbank;
        var params = {
            userid: app.siteConfig.siteuserid,
            ticket: app.siteConfig.siteuserticket,
            realname: escape(realname),
            banktype: escape(banktype),
            bankcardnum: bankcardnum,
            bankbranch: escape(bankbranch)
        };
        if (type === "edit") {
            params.bankid = $('.bankid').val();
            posrurl = app.siteConfig.edituserbank;
        }
        params.sign = app.createParamSign(params);
        $.showIndicator();
        $.ajax({
            type: 'POST',
            url: posrurl,
            data: params,
            dataType: 'json',
            context: $('body'),
            success: function(data) {
                if (data.code === 0) {
                    $.alert($.i18n.prop('common_success_text'), '', function () {
                        $.closeModal('.popup-userbank');
                        bank.loadUserBank();
                    });
                } else if (data.code === 98 || data.code === 99) {
                    app.clearLogin(data.errors);
                } else {
                    $.toast(data.errors);
                }
            },
            error: function (xhr, type) {
                var response = JSON.parse(xhr.response);
                $.toast(response.ExceptionMessage);
            },
            complete: function(xhr) {
                $.hideIndicator();
            }
        });
    };
    //加载卡信息
    bank.loadUserBank = function() {
        var params = {
            "ticket": app.siteConfig.siteuserticket,
            "userid": app.siteConfig.siteuserid,
            lang: languestorage.langue
        };
        params.sign = app.createParamSign(params);
        $.showIndicator();
        $.ajax({
            type: 'POST',
            url: app.siteConfig.userbanklist,
            data: params,
            dataType: 'json',
            context: $('body'),
            success: function(data) {
                if (data.code === 0 && data.data != null) {
                    var html = template('bank_tpl', data);
                    $(".bank_ul").html('').html(html);
                } else if (data.code === 98 || data.code === 99) {
                    app.clearLogin(data.errors);
                } else {
                    $.toast(data.message || data.errors);
                }
            },
            error: function (xhr, type) {
                var response = JSON.parse(xhr.response);
                $.toast(response.ExceptionMessage);
            },
            complete: function(xhr) {
                $.hideIndicator();
            }
        });
    };
    //删除卡信息
    bank.delUserBank = function (bankid) {
        $.modal.prototype.defaults.modalButtonCancel = $.i18n.prop('common_cacel_text');
        $.modal.prototype.defaults.modalButtonOk = $.i18n.prop('common_confirm_text');
        $.confirm($.i18n.prop('common_delete_confirm_toast'), function () {
            var params = {
                userid: app.siteConfig.siteuserid,
                ticket: app.siteConfig.siteuserticket,
                bankid: bankid,
                lang: languestorage.langue
            };
            params.sign = app.createParamSign(params);
            $.showIndicator();
            $.ajax({
                type: 'POST',
                url: app.siteConfig.deluserbank,
                data: params,
                dataType: 'json',
                context: $('body'),
                success: function(data) {
                    if (data.code === 0) {
                        $.alert($.i18n.prop('common_success_text'), '', function () {
                            $.closeModal('.popup-userbank');
                            bank.loadUserBank();
                        });
                    } else if (data.code === 98 || data.code === 99) {
                        app.clearLogin(data.errors);
                    } else {
                        $.toast(data.errors);
                    }
                },
                error: function (xhr, type) {
                    var response = JSON.parse(xhr.response);
                    $.toast(response.ExceptionMessage);
                },
                complete: function(xhr) {
                    $.hideIndicator();
                }
            });
        });
    };
})();