var languestorage = {
    langue: app.siteConfig.sitelanguage,
    source: '1',
    pagenum: 1,
    pagesize: 8,
    timezone:app.siteConfig.clienttimezone
};
$(function () {
    $(document).on("pageInit", "#page-swiper", function (e, id, page) {
        league.loadLanguage(languestorage.langue);
        league.loadModul();
        var times = app.siteConfig.refreshtimesrb;
        //自动刷新
        var timer = new app.SiteTimer(function () {
            page.find('.league-refresh em').html('').html(times);
            times--;
            if (times === -1) {
                switch (languestorage.source) {
                    case '1':
                        times = app.siteConfig.refreshtimesrb;
                        break;
                    default:
                        times = app.siteConfig.refreshtimes;
                        break;
                }
                languestorage.pagenum = 1;
                league.loadTypeLeague(languestorage.pagenum);
            }
        }, 1000);
        //玩法切换
        page.on('click', '.tab-link', function () {
            languestorage.source = $(this).attr("data-type");
            switch (languestorage.source) {
                case '1':
                    times = app.siteConfig.refreshtimesrb;
                    break;
                default:
                    times = app.siteConfig.refreshtimes;
                    break;
            }
            languestorage.pagenum = 1;
            league.loadTypeLeague(languestorage.pagenum);
        });

        //手动刷新
        page.on('click', '.league-refresh', function () {
            switch (languestorage.source) {
                case '1':
                    times = app.siteConfig.refreshtimesrb;
                    break;
                default:
                    times = app.siteConfig.refreshtimes;
                    break;
            }
            languestorage.pagenum = 1;
            league.loadTypeLeague(languestorage.pagenum);
        });

        var $content = $(page).find(".content").on('refresh', function (e) {
            switch (languestorage.source) {
                case '1':
                    times = app.siteConfig.refreshtimesrb;
                    break;
                default:
                    times = app.siteConfig.refreshtimes;
                    break;
            }
            languestorage.pagenum = 1;
            league.loadTypeLeague(languestorage.pagenum);
            $.pullToRefreshDone($content);
        });
    });
    $.init();
});
var league = league || {};
(function () {
    league.loadMore = function () {
        languestorage.pagenum = parseInt(languestorage.pagenum) + 1;
        league.loadTypeLeague(languestorage.pagenum);
    };
    league.loadLanguage = function (language) {
        Zepto.i18n.properties({
            name: 'language',
            path: '/i18n/mobile/',
            mode: 'map',
            language: language,
            callback: function () {
                $(".title").html($.i18n.prop('home_head_title'));
                $(".tab1").text($.i18n.prop('league_tab1_text'));
                $(".tab2").text($.i18n.prop('league_tab2_text'));
            }
        });
    };
    league.loadModul = function () {
        var params = {
            "lang": app.siteConfig.sitelanguage,
            "action": "adv"
        };
        params.sign = app.createParamSign(params);
        $.showIndicator();
        $.ajax({
            type: 'POST',
            url: app.siteConfig.siteadvlist,
            data: params,
            dataType: 'json',
            context: $('body'),
            success: function (data) {
                if (data.code === 0) {
                    var html = template('adv_list_tpl', data);
                    $(".swiper-container").html(html);
                    $(".swiper-container").swiper({
                        speed: 400,
                        spaceBetween: 100,
                        autoplay: 3000
                    });
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
                setTimeout(league.loadTypeLeague(1), 500);
            }
        });
    };
    league.loadTypeLeague = function (page) {
        //console.log(languestorage);
        var params = {
            "lang": app.siteConfig.sitelanguage,
            "userid": app.siteConfig.siteuserid,
            "ticket": app.siteConfig.siteuserticket,
            "source": languestorage.source,
            "page": page,
            "size": languestorage.pagesize,
            "zone": languestorage.timezone
        };
        params.sign = app.createParamSign(params);
        $.showIndicator();
        $.ajax({
            type: 'POST',
            url: app.siteConfig.matchleague,
            data: params,
            dataType: 'json',
            context: $('body'),
            success: function (data) {
                if (data.code === 0) {
                    var html = template('leauge_list_tpl', data);
                    switch (languestorage.source) {
                        case '1':
                            $(".leaugerb_tpl_ul").html(html);
                            break;
                        default:
                            $(".leauge_tpl_ul").html(html);
                            break;
                    }
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
})();
