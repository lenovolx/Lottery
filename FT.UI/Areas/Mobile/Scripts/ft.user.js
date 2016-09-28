var userstroge = {
    langue: app.siteConfig.sitelanguage,
    changeamount: 0,
    cashrealname: "",
    cashbanktype: "",
    cashbankbranch: "",
    hassafepass: false
};
var bankmodal = null;
$(function () {
    "use strict";
    $(document).on("pageInit", "#ucenter", function (e, id, page) {
        user.loadLanguage(userstroge.langue);
        var $content = $(page).find('.content');
        //加载用户信息
        user.loadInformation();
        //充值
        $content.on('click', '.user_recharge', function () {
            user.popopOperate($.i18n.prop('user_recharge_text'), 2, 'User_Recharge');
        });
        //提现
        $content.on('click', '.user_cash', function () {
            user.popopOperate($.i18n.prop('user_cash_text'), 5, 'User_Cash');
        });
        //转账
        $content.on('click', '.user_trad', function () {
            user.popopOperate($.i18n.prop('user_trad_text'), 1, 'User_Transfer');
        });
        //银行卡绑定
        $content.on('click', '.user_addbank', function () {
            user.popopOperate($.i18n.prop('password_li_text'), 4, 'Add_BankAccount');
        });
        //修改密码
        $content.on('click', '.user_modifypass', function () {
            user.popopOperate($.i18n.prop('password_li_text'), 3, 'Modify_Pass');
        });
        //安全密码
        $content.on('click', '.user_safepass', function () {
            user.popopOperate($.i18n.prop('user_safepass_text'), 6, 'Modify_SafePass');
        });
        //退出登录
        $('.user_logout').on('click', function () {
            $.modal.prototype.defaults.modalButtonCancel = $.i18n.prop('common_cacel_text');
            $.modal.prototype.defaults.modalButtonOk = $.i18n.prop('common_confirm_text');
            $.confirm($.i18n.prop('user_exitlog_toast'), function () {
                var params = {
                    ticket: app.siteConfig.siteuserticket,
                    lang: userstroge.langue
                };
                params.sign = app.createParamSign(params);
                $.ajax({
                    type: 'POST',
                    url: app.siteConfig.userlogout,
                    data: params,
                    dataType: 'json',
                    context: $('body'),
                    success: function (data) {
                        $.fn.cookie('FT.Mobile.Amount', 0, { expires: -1, path: '/m' });
                        $.fn.cookie('FT.Mobile.Ticket', '', { expires: -1, path: '/m' });
                        $.fn.cookie('FT.Mobile.UserId', 0, { expires: -1, path: '/m' });
                        $.fn.cookie('FT.Mobile.UserName', '', { expires: -1, path: '/m' });
                        location.href = '/m/login';
                    }
                });
            });
        });
        //语言切换
        $('.langue-change').on('click', function () {
            var html = [];
            html.push("{{if langue !== 'cn'}}<p class=\"panel-lang\" lang=\"cn\" onclick=\"user.closePanel(this)\">简体中文</p>{{/if}}");
            html.push("{{if langue !== 'en'}}<p class=\"panel-lang\" lang=\"en\" onclick=\"user.closePanel(this)\">English</p>{{/if}}");
            html.push("{{if langue !== 'pt'}}<p class=\"panel-lang\" lang=\"pt\" onclick=\"user.closePanel(this)\">Portugal</p>{{/if}}");
            var chargetemplate = html.join("");
            var render = template.compile(chargetemplate);
            $(".panel-template").html("").html(render(userstroge));
            $.openPanel('#panel-right-lang');
        });
    });
    $.init();
});
var user = user || {};
(function () {
    user.loadLanguage = function (language) {
        Zepto.i18n.properties({
            name: 'language',
            path: '/i18n/mobile/',
            mode: 'map',
            language: language,
            callback: function () {
                $(".title").html($.i18n.prop('user_head_title'));
                $(".user_recharge").html($.i18n.prop('user_recharge_text'));
                $(".user_cash").html($.i18n.prop('user_cash_text'));
                $(".user_trad").html($.i18n.prop('user_trad_text'));
                $(".trad_li").html($.i18n.prop('trad_li_text'));
                $(".history_li").html($.i18n.prop('history_li_text'));
                $(".bank_li").html($.i18n.prop('bank_li_text'));
                $(".password_li").html($.i18n.prop('password_li_text'));
                $(".user-logout").html($.i18n.prop('user_logout_text'));
                $(".safepass_li").html($.i18n.prop('user_safepass_text'));
            }
        });
        if (language === 'pt')
            $('.langue-change span').text(languageArry.pt);
        else if (language === 'en')
            $('.langue-change span').text(languageArry.en);
        else
            $('.langue-change span').text(languageArry.cn);
    };

    user.popopOperate = function (title, type, templatename) {
        var popupdata = { title: title, type: type, account: app.siteConfig.siteusername, btnText: $.i18n.prop('common_confirm_text'), safepass: userstroge.hassafepass };
        var html = ["<header class=\"bar bar-nav\">"];
        html.push("<a class=\"button button-link button-nav pull-left close-popup\"><span class=\"icon icon-close\"></span></a>");
        html.push("<h1 class=\"title\"><span> {{title}}</span></h1>");
        html.push("</header>");
        html.push("<div class=\"content\">");
        html.push("<div class=\"list-block\">");
        html.push("<div class=\"list-block\">");

        html.push("<ul>");

        html.push("{{include '" + templatename + "'}}");

        html.push("</ul>");
        html.push("</div>");
        html.push("<div class=\"content-block\">");
        html.push("<div class=\"row\">");
        if (type === 2)
            html.push("<div class=\"row\"><a href=\"javascript:void(0);\" class=\"button button-big button-fill button-dark btn-info\">{{btnText}}</a></div>");
        else
            html.push("<div class=\"row\"><a href=\"javascript:void(0);\" class=\"button button-big button-fill button-success btn-info\" onclick=\"user.operateSlect({{type}})\">{{btnText}}</a></div>");
        html.push("</div>");
        html.push("</div>");
        html.push("</div>");
        var chargetemplate = html.join("");
        var render = template.compile(chargetemplate);
        $(".popup-usercenter").html("").html(render(popupdata));

        switch (templatename) {
            case 'User_Recharge':
                $(".cardNum").attr('placeholder', $.i18n.prop('user_rechargecard_place'));
                $(".card-account").text($.i18n.prop('user_account_text'));
                $(".card-amount").text($.i18n.prop('card_amount_text'));
                $(".button-flot").text($.i18n.prop('card_check_text'));
                break;
            case 'User_Cash':
                $(".bankcardnum").attr('placeholder', $.i18n.prop('cash_banknum_place'));
                $(".cashamount").attr('placeholder', $.i18n.prop('cash_amount_place'));
                $(".button-flot").text($.i18n.prop('cash_savedcard_text'));
                $(".cash-card").text($.i18n.prop('cash_cardnum_text') + ":");
                $(".cash-amount").text($.i18n.prop('card_amount_text') + ":");
                $(".div-safecode").text($.i18n.prop('user_safepass_text') + ":");
                $(".safecode").attr('placeholder', $.i18n.prop('user_safepass_place'));
                break;
            case 'User_Transfer':
                $(".account").attr('placeholder', $.i18n.prop('tran_account_place'));
                $(".amount").attr('placeholder', $.i18n.prop('tran_amount_place'));
                $(".lab-account").text($.i18n.prop('tran_account_text') + ":");
                $(".lab-amount").text($.i18n.prop('card_amount_text') + ":");
                $(".div-safecode").text($.i18n.prop('user_safepass_text') + ":");
                $(".safecode").attr('placeholder', $.i18n.prop('user_safepass_place'));
                break;
            case 'Modify_Pass':
                $(".lab-old").text($.i18n.prop('pass_old_text') + ":");
                $(".lab-new").text($.i18n.prop('pass_new_text') + ":");
                $(".lab-confirm").text($.i18n.prop('pass_confirm_text') + ":");
                $(".oldpass").attr('placeholder', $.i18n.prop('pass_old_place'));
                $(".newpass").attr('placeholder', $.i18n.prop('pass_new_place'));
                $(".renewpass").attr('placeholder', $.i18n.prop('pass_confirm_place'));
                break;
            default:
                $(".lab-old").text($.i18n.prop('pass_old_text') + ":");
                $(".lab-safe").text($.i18n.prop('user_safepass_text') + ":");
                $(".lab-confirm").text($.i18n.prop('pass_confirm_text') + ":");
                $(".oldpass").attr('placeholder', $.i18n.prop('pass_old_place'));
                $(".newpass").attr('placeholder', $.i18n.prop('user_safepass_place'));
                $(".renewpass").attr('placeholder', $.i18n.prop('pass_confirm_place'));
                break;
        }
        $.popup('.popup-usercenter');
    };
    //校验充值卡是否有效
    user.validCrad = function (obj) {
        var cardnum = $('.cardNum').val();
        if ($.trim(cardnum) === '') {
            $.toast($.i18n.prop('user_rechargecard_place'));
            return;
        }
        var params = {
            userid: app.siteConfig.siteuserid,
            ticket: app.siteConfig.siteuserticket,
            card: cardnum,
            lang: userstroge.langue
        };
        params.sign = app.createParamSign(params);
        $.showIndicator();
        $.ajax({
            type: 'POST',
            url: app.siteConfig.validcard,
            data: params,
            dataType: 'json',
            context: $('body'),
            success: function (data) {
                if (data.code === 0 && data.data !== null) {
                    if (data.data.IsUsed === 0) {
                        $('.recharge_num').text(data.data.Amount);
                        userstroge.changeamount = data.data.Amount;
                        $('.btn-info').removeClass('button-dark').addClass('button-success').off('click').on('click', function () {
                            user.operateSlect(2);
                        });
                    } else {
                        $.toast($.i18n.prop('card_invalid_text'));
                        $('.cardNum').val('');
                    }
                } else if (data.code === 98 || data.code === 99) {
                    app.clearLogin(data.errors);
                } else {
                    $.toast($.i18n.prop('card_invalid_text'));
                    $('.cardNum').val('');
                }
            },
            error: function (xhr, type) {
                var response = JSON.parse(xhr.response);
                $.toast(response.ExceptionMessage);
            },
            complete: function (xhr) {
                $.hideIndicator();
            }
        });
    };
    //popup层操作
    user.operateSlect = function (type) {
        var params = {
            userid: app.siteConfig.siteuserid,
            ticket: app.siteConfig.siteuserticket,
            lang: userstroge.langue
        };
        var posturl = "";
        if (type === 2) {
            var card = $('.cardNum').val();
            if ($.trim(card) === '') {
                $.toast($.i18n.prop('user_rechargecard_place'));
                return;
            }
            posturl = app.siteConfig.usertrad;
            params.tuser = app.siteConfig.siteuserid;
            params.card = card;
            params.type = 2;
        } else if (type === 1) {
            var account = $('.account').val();
            var amount = $('.amount').val();
            var safecode = $('.safecode').val();
            if ($.trim(account) === '') {
                $.toast($.i18n.prop('tran_account_place'));
                return;
            }
            if ($.trim(amount) === '') {
                $.toast($.i18n.prop('tran_amount_place'));
                return;
            }
            if ($.trim(safecode) === '') {
                $.toast($.i18n.prop('user_safepass_place'));
                return;
            }
            posturl = app.siteConfig.usertrad;
            params.type = 1;
            params.fuser = app.siteConfig.siteuserid;
            params.tuser = escape(account);
            params.amount = amount;
            params.code = hex_md5(safecode);
            userstroge.changeamount = parseFloat(amount);
        } else if (type === 3 || type === 6) {
            var oldpass = $('.oldpass').val();
            var newpass = $('.newpass').val();
            var renewpass = $('.renewpass').val();
            if ($.trim(oldpass) === '') {
                $.toast($.i18n.prop('pass_old_place'));
                return;
            }
            if ($.trim(newpass) === '') {
                $.toast(type === 3 ? $.i18n.prop('pass_new_place') : $.i18n.prop('user_safepass_place'));
                return;
            }
            if ($.trim(renewpass) === '') {
                $.toast($.i18n.prop('pass_confirm_place'));
                return;
            }
            if ($.trim(renewpass) !== $.trim(newpass)) {
                $.toast($.i18n.prop('pass_noteq_toast'));
                return;
            }
            posturl = app.siteConfig.modifypass;
            params.pwd = hex_md5(oldpass);
            params.npwd = hex_md5(newpass);
            params.type = type;
        } else if (type === 5) {
            var bankcardnum = $.trim($('.bankcardnum').val());
            var cashamount = $.trim($('.cashamount').val());
            var useramount = parseFloat($('.item-amount').html());
            var safecode = $('.safecode').val();
            if ($.trim(bankcardnum) === '') {
                $.toast($.i18n.prop('cash_banknum_place'));
                return;
            }
            if ($.trim(cashamount) === '') {
                $.toast($.i18n.prop('cash_amount_place'));
                return;
            }
            if (parseFloat(cashamount) > useramount) {
                $.toast('提现金额超过可用余额');
                return;
            }
            if ($.trim(safecode) === '') {
                $.toast($.i18n.prop('user_safepass_place'));
                return;
            }
            posturl = app.siteConfig.usercash;
            params.realname = escape(userstroge.cashrealname);
            params.bankbranch = escape(userstroge.cashbankbranch);
            params.banktype = escape(userstroge.cashbanktype);
            params.bankcardnum = bankcardnum;
            params.amount = cashamount;
            params.code = hex_md5(safecode);
            userstroge.changeamount = parseFloat(cashamount);
        }
        params.sign = app.createParamSign(params);
        $.showIndicator();
        $.ajax({
            type: 'POST',
            url: posturl,
            data: params,
            dataType: 'json',
            context: $('body'),
            success: function (data) {
                if (data.code === 0) {
                    $.modal.prototype.defaults.modalButtonOk = $.i18n.prop('common_confirm_text');
                    $.alert($.i18n.prop('common_success_text'), '', function () {
                        var useramount = parseFloat($('.item-amount').html());
                        switch (type) {
                            case 2:
                                useramount = useramount + userstroge.changeamount;
                                $.fn.cookie('FT.Mobile.Amount', useramount);
                                $('.item-amount').html(useramount);
                                break;
                            case 1:
                                useramount = useramount - userstroge.changeamount;
                                $.fn.cookie('FT.Mobile.Amount', useramount);
                                $('.item-amount').html(useramount);
                                break;
                            case 5:
                                useramount = useramount - userstroge.changeamount;
                                $.fn.cookie('FT.Mobile.Amount', useramount);
                                $('.item-amount').html(useramount);
                                break;
                            case 6:
                                userstroge.hassafepass = true;
                                break;
                        }
                        $.closeModal('.popup-usercenter');
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
            complete: function (xhr) {
                $.hideIndicator();
            }
        });
    };
    user.closePanel = function (obj) {
        userstroge.langue = $(obj).attr('lang');
        user.loadLanguage(userstroge.langue);
        user.loadInformation();
        $.fn.cookie('FT.Mobile.Langue', userstroge.langue, { expires: 7 });
        $.closePanel();
    };
    user.loadInformation = function () {
        $.modal.prototype.defaults.modalButtonOk = $.i18n.prop('common_confirm_text');
        var params = {
            userid: app.siteConfig.siteuserid,
            ticket: app.siteConfig.siteuserticket,
            lang: userstroge.langue
        };
        params.sign = app.createParamSign(params);
        $.showIndicator();
        $.ajax({
            type: 'POST',
            url: app.siteConfig.usercenter,
            data: params,
            dataType: 'json',
            context: $('body'),
            success: function (data) {
                if (data.code === 0 && data.data !== null) {
                    $.fn.cookie('FT.Mobile.Amount', data.data.BalanceAmount);
                    userstroge.hassafepass = data.data.HasSafePass;
                    var html = template('userInfomation', {
                        username: data.data.LoginName,
                        amount: data.data.BalanceAmount,
                        amounttitle: $.i18n.prop('user_amount_text')
                    });
                    $(".user-infomation").html(html);
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
            complete: function (xhr) {
                $.hideIndicator();
            }
        });
    };
    user.openCashModal = function () {
        var params = {
            "ticket": app.siteConfig.siteuserticket,
            "userid": app.siteConfig.siteuserid,
            lang: userstroge.langue
        };
        params.sign = app.createParamSign(params);
        $.showIndicator();
        $.ajax({
            type: 'POST',
            url: app.siteConfig.userbanklist,
            data: params,
            dataType: 'json',
            context: $('body'),
            success: function (data) {
                if (data.code === 0 && data.data != null) {
                    var html = ["<div class=\"cardli_box\">"];
                    html.push("<ul class=\"cardli-content\">");
                    $.each(data.data, function (i, v) {
                        html.push("<li onclick=\"user.selectCashAccount('" + v.RealName + "','" + v.BankType + "','" + v.BankCardNum + "','" + v.BankBranch + "')\"><span class=\"card-name\">" + v.RealName + "</span><span class=\"card_num\">" + v.BankType + "(" + v.BankCardNum + ")</span></li>");
                    });
                    html.push("</ul>");
                    html.push("</div>");
                    bankmodal = $.modal({
                        text: html.join("")
                    });
                }
            },
            error: function (xhr, type) {
                var response = JSON.parse(xhr.response);
                $.toast(response.ExceptionMessage);
            },
            complete: function (xhr) {
                $.hideIndicator();
            }
        });
    };
    user.selectCashAccount = function (realname, banktype, bankcardnum, bankbranch) {
        userstroge.cashrealname = realname;
        userstroge.cashbankbranch = bankbranch;
        userstroge.cashbanktype = banktype;
        $('.bankcardnum').val(bankcardnum);
        $.closeModal(bankmodal);
    };
})();