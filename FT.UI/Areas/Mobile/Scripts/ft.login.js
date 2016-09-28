var languestorage = {
    langue: app.siteConfig.sitelanguage,
    allZone: [], defaultZone: ""
};
$(function () {
    $(document).on("pageInit", "#page-layout", function (e, id, page) {
        login.loadLanguage(languestorage.langue);
        login.loadTimeZone(languestorage.langue);
        var $content = $(page).find('.content');
        $content.on('click', '.btn_login', function () {
            var username = page.find(".user_name").val();
            var userpass = page.find(".user_pass").val();
            var rember = page.find(".rember").prop('checked');
            if ($.trim(username) === "") {
                $.toast($.i18n.prop('login_username_place'));
                return;
            }
            if ($.trim(userpass) === "") {
                $.toast($.i18n.prop('login_userpass_place'));
                return;
            }
            var params = {
                "username": username,
                "password": hex_md5(userpass),
                "plat": 1,
                "rember": rember,
                "lang": languestorage.langue
            };
            params.sign = app.createParamSign(params);
            $.showIndicator();
            $.ajax({
                type: 'POST',
                url: app.siteConfig.userlogin,
                data: params,
                dataType: 'json',
                context: $('body'),
                success: function (data) {
                    if (data.code === 0) {
                        var obj = $.parseJSON(data.data);
                        $.fn.cookie('FT.Mobile.Ticket', obj.Ticket, { expires: 7, path: '/m' });
                        $.fn.cookie('FT.Mobile.UserName', obj.User.LoginName, { expires: 7, path: '/m' });
                        $.fn.cookie('FT.Mobile.UserId', obj.User.Id, { expires: 7, path: '/m' });
                        $.fn.cookie('FT.Mobile.Amount', obj.User.BalanceAmount, { expires: 7, path: '/m' });
                        $.fn.cookie('FT.Mobile.MinBetAmount', obj.System.MinBetAmount, { expires: 7, path: '/m' });
                        $.fn.cookie('FT.Mobile.MaxBetAmount', obj.System.MaxBetAmount, { expires: 7, path: '/m' });
                        if ($.fn.cookie('FT.Mobile.Langue') === null) {
                            $.fn.cookie('FT.Mobile.Langue', 'cn', { expires: 7, path: '/m' });
                        }
                        location.href = '/m';
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
        });
        //语言切换
        $('.langue-change').on('click', function () {
            var html = [];
            html.push("{{if langue !== 'cn'}}<p class=\"panel-lang\" lang=\"cn\" onclick=\"login.closePanel(this)\">简体中文</p>{{/if}}");
            html.push("{{if langue !== 'en'}}<p class=\"panel-lang\" lang=\"en\" onclick=\"login.closePanel(this)\">English</p>{{/if}}");
            html.push("{{if langue !== 'pt'}}<p class=\"panel-lang\" lang=\"pt\" onclick=\"login.closePanel(this)\">Portugal</p>{{/if}}");
            var chargetemplate = html.join("");
            var render = template.compile(chargetemplate);
            $(".panel-template").html("").html(render(languestorage));
            $.openPanel('#panel-right-lang');
        });
        
    });
    $.init();
});
var login = login || {};
(function () {
    login.loadLanguage = function (language) {
        Zepto.i18n.properties({
            name: 'language',
            path: '/i18n/mobile/',
            mode: 'map',
            language: language,
            callback: function () {
                $(".title").html($.i18n.prop('login_head_title'));
                $(".user_name").attr('placeholder', $.i18n.prop('login_username_place'));
                $(".user_pass").attr('placeholder', $.i18n.prop('login_userpass_place'));
                $(".user_timezone").attr('placeholder', $.i18n.prop('login_timezone_place'));
                $(".checkbox").html($.i18n.prop('login_rember_pass'));
                $('.btn_login').text($.i18n.prop('login_login_btn'));
            }
        });
        if (language === 'pt')
            $('.langue-change span').text(languageArry.pt);
        else if (language === 'en')
            $('.langue-change span').text(languageArry.en);
        else
            $('.langue-change span').text(languageArry.cn);
    };
    login.closePanel = function (obj) {
        login.loadLanguage($(obj).attr('lang'));
        languestorage.langue = $(obj).attr('lang');
        login.loadTimeZone(languestorage.langue);
        $.fn.cookie('FT.Mobile.Langue', languestorage.langue, { expires: 7 });
        $.closePanel();
    };
    login.loadTimeZone = function (_language) {
        $(".user_timezone").picker({
            toolbar: false,
            cols: [
                {
                    textAlign: 'center',
                    values: login.getTimeZone(_language).allZone
                }
            ],
            inputReadOnly: true,
            createNew: true
        }).on("change", function() {
            var tz = $(this).val();
            switch (_language) {
            case "en":
                login.setTimeZone(localTimeZone.en, tz);
                break;
            case "pt":
                login.setTimeZone(localTimeZone.pt, tz);
                break;
            default:
                login.setTimeZone(localTimeZone.cn, tz);
                break;
            }
        }).val(login.getDefaultZone(_language));
    };
    login.getTimeZone = function(_language) {
        $(".user_timezone").val(login.getDefaultZone(_language));
        switch (_language) {
        case "en":
            languestorage.allZone = localTimeZone.en;
            break;
        case "pt":
            languestorage.allZone = localTimeZone.pt;
            break;
        default:
            languestorage.allZone = localTimeZone.cn;
            break;
        }
        return languestorage;
    };
    login.setTimeZone = function(obj, val) {
        var index;
        $.each(obj, function(i, v) {
            if (v === val) {
                index = i;
                return false;
            }
        });
        $.fn.cookie('FT.Client.TimeZone', localTimeZone.PointValue[index], { expires: 7, path: '/m' });
    };
    login.getDefaultZone = function(_language) {
        var index;
        var defaultZone = $.fn.cookie('FT.Client.TimeZone') == null ? 8 : parseFloat($.fn.cookie('FT.Client.TimeZone'));
        $.each(localTimeZone.PointValue, function(i, v) {
            if (v === defaultZone) {
                index = i;
                return false;
            }
        });
        switch (_language) {
        case "en":
            return localTimeZone.en[index];
        case "pt":
            return localTimeZone.pt[index];
        default:
            return localTimeZone.cn[index];
        }
    };
})();
