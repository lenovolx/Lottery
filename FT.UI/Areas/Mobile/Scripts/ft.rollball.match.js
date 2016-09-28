var userbet = {
    betvalue: 0,
    betcontent: []
};
var league_bettype = {
    bettype: 60,
    league: ""
};
//选择玩法填充 展示用
var selectbet = {
    league: "", //联赛名称
    team: "", //球队
    hcn: "", //玩法
    ior: "", //赔率
    iorindex: 0,
    bet: "",
    key: "", //让球 大小球区分主客队
    maxamout: app.siteConfig.sitebetmxnamount,
    minamount: app.siteConfig.sitebetminamount,
    matchid: 0
};
var selectbets = {
    league: '',
    amount: 0,
    list: [],
    maxamout: app.siteConfig.sitebetmxnamount,
    minamount: app.siteConfig.sitebetminamount
};
var languestorage = {
    langue: app.siteConfig.sitelanguage
};
var timerpopup;
var selectMatch = 0; //选择的比赛场次数量
$(function () {
    "use strict";
    match.loadLanguage(languestorage.langue);
    $(document).on("pageInit", "#match-list", function (e, id, page) {
        var parms = app.parseUrlQuery(location.href);
        league_bettype.league = typeof (parms.league) == "undefined" ? "" : parms.league;
        var times = app.siteConfig.refreshtimesrb;
        match.loadLeagueMatch(league_bettype.bettype, league_bettype.league);
        var $content = page.find('.content');
        $content.on('click', '.tab-link', function () {
            league_bettype.bettype = $(this).attr("bettype");
            match.loadLeagueMatch(league_bettype.bettype, league_bettype.league);
            times = app.siteConfig.refreshtimesrb;
        });
        //清空选择
        page.find('#btn_clear').on('click', function () {
            match.clearMatch();
        });
        //投注
        //page.find('#btn_ret').on('click', function() {
        //    match.userBetTemplate(0);
        //});
        //手动刷新
        page.on('click', '.match-refresh', function () {
            match.loadLeagueMatch(league_bettype.bettype, league_bettype.league);
            times = app.siteConfig.refreshtimesrb;
        });
        //自动刷新
        var timer = new app.SiteTimer(function () {
            page.find('.match-refresh em').html('').html(times);
            times--;
            if (times === -1) {
                match.loadLeagueMatch(league_bettype.bettype, league_bettype.league);
                times = app.siteConfig.refreshtimesrb;
            }
        }, 1000);
        $(page).find(".content").on('refresh', function (e) {
            times = app.siteConfig.refreshtimesrb;
            match.loadLeagueMatch(league_bettype.bettype, league_bettype.league);
            $.pullToRefreshDone($content);
        });
    });
    //滚动方法
    match.bettypeSwiper();
    $.init();
});
var match = match || {};
(function () {
    //加载语言包
    match.loadLanguage = function (language) {
        Zepto.i18n.properties({
            name: 'language',
            path: '/i18n/mobile/',
            mode: 'map',
            language: language,
            callback: function () {
                $(".title").html($.i18n.prop('home_head_title'));
                $(".tab1").text($.i18n.prop('match_tab1_text'));
                $(".tab2").text($.i18n.prop('match_tab2_text'));
                $(".tab3").text($.i18n.prop('match_tab7_text'));
                $("#btn_clear").text($.i18n.prop('match_bet_clear'));
                $("#btn_ret").text($.i18n.prop('match_bet_confirm'));
                $(".selected_match").html($.i18n.prop('match_bet_select').format("0"));
            }
        });
    };
    //加载联赛赛事
    match.loadLeagueMatch = function (bettype, league) {
        var params = {
            "oddbettype": bettype,
            "bettype": "",
            "league": league,
            "ticket": app.siteConfig.siteuserticket,
            "userid": app.siteConfig.siteuserid,
            "lang": app.siteConfig.sitelanguage,
            "source": 1,
            "datatype": "json",
            "zone":app.siteConfig.clienttimezone
        };
        params.sign = app.createParamSign(params);
        $.showIndicator();
        $.ajax({
            type: 'POST',
            url: app.siteConfig.matchlist,
            data: params,
            dataType: 'json',
            context: $('body'),
            success: function (data) {
                if (data.code === 0) {
                    var html = template('match_list_tpl_' + bettype, data.data);
                    $(".tab.active").find(".match_list_ul").html(html);
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
    //玩法选择
    match.bettypeSelect = function (betdb, league, hteam, cteam, matchid, betior, bettypeid, betkey, obj, index, currentscore) {
        //userbet.betcontent = [];
        //切换选项卡之后清空已投注信息
        if (betior == "0") {
            return;
        }
        var betObj = eval("[" + userbet.betcontent.toString() + "]");
        if (betObj.length > 0) {
            betObj.forEach(function (o, index) {
                if (o.BetType != bettypeid && o.BetType != "50" && o.BetType != "52") {
                    userbet.betcontent = [];
                    selectbets.list = [];
                    selectMatch = 0;
                }
            });
        }
        var limitrow = $(obj).parent().parent().children().first();
        selectbets.league = league;//比赛名称
        selectbet.league = league;
        selectbet.ior = betior;
        selectbet.key = betdb;
        selectbet.iorindex = index;
        selectbet.matchid = matchid;
        selectbet.CurrentScore = currentscore;
        switch (bettypeid) {
            case 60:
                selectbet.team = hteam + '&nbsp;<font color=\'red\'>vs</font>&nbsp;' + cteam;
                if (betdb === "H") {
                    selectbet.bet = hteam;
                } else if (betdb === "N") {
                    selectbet.bet = app.siteConfig.sitelanguage === 'cn' ? "和局" : "Draw";
                } else {
                    selectbet.bet = cteam;
                }
                break;
            case 61:
                selectbet.team = hteam + '&nbsp;<font color=\'red\'>vs</font>&nbsp;' + cteam;
                if (betdb === "O") {
                    selectbet.bet = (app.siteConfig.sitelanguage === 'cn' ? "大" : "O") + "&nbsp;<font color=red'>" + betkey + "</font>";
                    selectbet.key = betkey;
                } else {
                    selectbet.bet = (app.siteConfig.sitelanguage === 'cn' ? "小" : "U") + "&nbsp;<font color=red'>" + betkey + "</font>";
                    selectbet.key = betkey;
                }

                break;
            case 63:
                var letarry = betkey.split('|');
                if (betdb === 'H') {
                    if (letarry[0] === 'H') {
                        selectbet.key = betkey;
                        selectbet.bet = hteam;
                        selectbet.team = hteam + '&nbsp;<font color=\'red\'>' + letarry[1] + '</font>&nbsp;vs&nbsp;' + cteam;
                    } else {
                        selectbet.key = betkey;
                        selectbet.bet = hteam;
                        selectbet.team = hteam + '&nbsp;vs&nbsp;' + cteam;
                    }
                } else {
                    if (letarry[0] === 'C') {
                        selectbet.key = betkey;
                        selectbet.bet = cteam;
                        selectbet.team = hteam + '&nbsp;vs&nbsp;<font color=\'red\'>' + letarry[1] + '</font>&nbsp;' + cteam;
                    } else {
                        selectbet.key = betkey;
                        selectbet.bet = cteam;
                        selectbet.team = hteam + '&nbsp;vs&nbsp;' + cteam;
                    }
                }
                break;
        }
        var betTemp = "";
        $('.bettype-' + bettypeid).removeClass('match-select');
        $(obj).addClass('match-select').siblings().removeClass('match-select');
        selectMatch = 1;
        userbet.betcontent = [];
        selectbets.list = [];
        betTemp = "{\"MatchID\":" + matchid + ",\"BetType\":" + bettypeid + ",\"BetContent\":\"" + betdb + "@" + betior + "\",\"BetKey\":\"" + selectbet.key + "\",\"BetScore\":\""+currentscore+"\"}";
        userbet.betcontent.push(betTemp);

        $(".selected_match").html($.i18n.prop('match_bet_select').format(selectMatch));
        selectbets.list.push(selectbet);
        selectbet = {};
        var minlimit = 0;
        var maxlimit = 0;

        $("#btn_ret").removeClass("disabled");
        $("#btn_ret").on("click", function () {
            match.userBetTemplate(0);
        });
        //console.log(selectbet);
    };
    //清除选择
    match.clearMatch = function () {
        userbet.betcontent = [];
        $(".selected_match").html($.i18n.prop('match_bet_select').format("0"));
        $('.match-bettype').removeClass('match-select');
        $(".popup-bet-context").html($.i18n.prop('match_bet_select_toast'));
        selectbet.bet = "";
        selectbet.team = "";
        selectbet.ior = "";
        selectbet.league = "";
        selectbet.key = "";
        selectbets.list = [];
        selectMatch = 0;
    };
    //确定交易
    match.confirmRet = function () {
        userbet.betvalue = $.trim($('.bet-amount').val());
        if (userbet.betvalue === '0' || userbet.betvalue === '') {
            $.toast($.i18n.prop('match_enter_amount_toast'));
            return;
        }
        if (parseFloat(userbet.betvalue) < selectbets.minamount || parseFloat(userbet.betvalue) > selectbets.maxamout) {
            $.toast($.i18n.prop('match_amount_exceed_toast'));
            $('.bet-amount').val("");
            return;
        }
        $.modal.prototype.defaults.modalButtonCancel = $.i18n.prop('common_cacel_text');
        $.modal.prototype.defaults.modalButtonOk = $.i18n.prop('common_confirm_text');
        $.confirm($.i18n.prop('match_bet_comfirm_alert'), function () {
            var params = {
                "ticket": app.siteConfig.siteuserticket,
                "userid": app.siteConfig.siteuserid,
                "matchuserbet": "{\"BetValue\":" + parseFloat(userbet.betvalue) + ",\"MatchUserBetContent\":[" + userbet.betcontent.join(',') + "]}",
                "lang": app.siteConfig.sitelanguage,
                "source": 1,
                "zone": app.siteConfig.clienttimezone
            };
            params.sign = app.createParamSign(params);
            $.showIndicator();
            $.ajax({
                type: 'POST',
                url: app.siteConfig.userbet,
                data: params,
                dataType: 'json',
                context: $('body'),
                success: function (data) {
                    if (data.code === 0) {
                        $.alert($.i18n.prop('common_success_text'), '', function () {
                            var balance = parseFloat($.fn.cookie('FT.Mobile.Amount')) - parseFloat(userbet.betvalue);
                            $.fn.cookie('FT.Mobile.Amount', balance);
                            $.closeModal(".popup-useret");
                            timerpopup.Stop();
                            match.clearMatch();
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
                    $.hideIndicator();
                }
            });
        });
    };
    //显示折叠
    match.showLeagueMatch = function (bettype, obj) {
        $(obj).toggleClass("zhover");
    };
    //关闭弹出层、定时器
    match.closeBetPopup = function () {
        $.closeModal();
        timerpopup.Stop();
    };
    //投注
    match.userBetTemplate = function (type) {
        timers = 15;
        if (selectbet.ior === "" && selectbet.team === "" && selectbet.league === "" && (!selectbets.list.length > 0)) {
            $.toast($.i18n.prop('match_bet_select_toast'));
            return;
        }
        var params = {
            userid: app.siteConfig.siteuserid,
            ticket: app.siteConfig.siteuserticket,
            lang: languestorage.langue
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
                    selectbets.amount = data.data.BalanceAmount.toFixed(2);
                    $.fn.cookie('FT.Mobile.Amount', data.data.BalanceAmount);
                    //
                    selectbets.betinfoTxt = $.i18n.prop('match_betinfo_text');
                    selectbets.betamountTxt = $.i18n.prop('match_betamount_text');
                    selectbets.betwonamountTxt = $.i18n.prop('match_effamount_text');
                    selectbets.betminTxt = $.i18n.prop('match_bet_min_text');
                    selectbets.betmaxTxt = $.i18n.prop('match_bet_max_text');
                    selectbets.betconfirmTxt = $.i18n.prop('match_bet_comfirm_alert');
                    selectbets.useramountTxt = $.i18n.prop('user_amount_text');

                    var html = [];
                    html.push("<header class=\"bar bar-nav\">");
                    html.push("<a class=\"button button-link button-nav pull-left\" onclick=\"match.closeBetPopup()\">");
                    html.push("<span class=\"icon icon-close\"></span>");
                    html.push("</a>");
                    html.push("<h1 class=\"title\">");
                    html.push("<span>{{league}}</span>{{useramountTxt}}:<em class=\"userAmount\">{{amount}}</em>");
                    html.push("</h1>");
                    html.push("<a class=\"icon icon-refresh pull-right timer-ten\" onclick=\"match.refushBetContentIOR()\">");
                    html.push("<em>15</em>");
                    html.push("</a>");
                    html.push("</header>");
                    html.push("<div class=\"content\">");
                    html.push("<div class=\"list-block\">");
                    html.push("<ul>");

                    html.push("<div class=\"item-content\">");
                    html.push("<div class=\"item-inner\">");
                    html.push("<div class=\"item-title label\">{{betinfoTxt}}：</div>");
                    html.push("<div class=\"item-input\">");
                    html.push("{{each list as item}}");
                    html.push("<div class=\"zh_iorli\"><span>{{#item.team}}</span><br/><span class=\"info-ior userbet{{#item.matchid}}\">{{#item.bet+'@&nbsp;'+item.ior}}</span></div>");
                    html.push("{{/each}}");
                    html.push("</div>");
                    html.push("</div>");
                    html.push("</div>");
                    html.push("</li>");
                    html.push("<li>");
                    html.push("<div class=\"item-content\">");
                    html.push("<div class=\"item-inner\">");
                    html.push("<div class=\"item-title label\">{{betamountTxt}}：</div>");
                    html.push("<div class=\"item-input\">");
                    html.push("<input type=\"number\" placeholder=\"{{betamountTxt}}\" class=\"bet-amount\" oninput=\"match.changeBetAmount(this)\" onpropertychange=\"match.changeBetAmount(this)\" onkeyup=\"if(this.value.length==1){this.value=this.value.replace(/[^1-9]/g,'')}else{this.value=this.value.replace(/\D/g,'')}\" onafterpaste=\"if(this.value.length==1){this.value=this.value.replace(/[^1-9]/g,'')}else{this.value=this.value.replace(/\D/g,'')}\">");
                    html.push("</div>");
                    html.push("</div>");
                    html.push("</div>");
                    html.push("</li>");
                    html.push("<li>");
                    html.push("<div class=\"item-content\">");
                    html.push("<div class=\"item-inner\">");
                    html.push("<div class=\"item-title label\">{{betwonamountTxt}}：</div>");
                    html.push("<div class=\"item-input\">");
                    html.push("<span class=\"canwin-amount\">0</span>");
                    html.push("</div>");
                    html.push("</div>");
                    html.push("</div>");
                    html.push("</li>");
                    html.push("<li>");
                    html.push("<div class=\"item-content\">");
                    html.push("<div class=\"item-inner\">");
                    html.push("<div class=\"item-title label\">{{betminTxt}}：</div>");
                    html.push("<div class=\"item-input\">");
                    html.push("<span class=\"bet-min\">{{minamount}}</span>");
                    html.push("</div>");
                    html.push("</div>");
                    html.push("</div>");
                    html.push("</li>");
                    html.push("<li>");
                    html.push("<div class=\"item-content\">");
                    html.push("<div class=\"item-inner\">");
                    html.push("<div class=\"item-title label\">{{betmaxTxt}}：</div>");
                    html.push("<div class=\"item-input\">");
                    html.push("<span class=\"bet-max\">{{maxamout}}</span>");
                    html.push("</div>");
                    html.push("</div>");
                    html.push("</div>");
                    html.push("</li>");
                    html.push("</ul>");
                    html.push("</div>");
                    html.push("<div class=\"content-block\">");
                    html.push("<a href=\"javascript:void(0)\" class=\"button button-big button-fill button-success\" onclick=\"match.confirmRet();\">{{betconfirmTxt}}</a>");
                    html.push("</div>");
                    html.push("</div>");

                    //根据赔率计算最大下注金额
                    var maxbet = app.siteConfig.sitebetmxnamount;
                    $.each(selectbets.list, function (i, v) {
                        maxbet = maxbet / v.ior;
                    });
                    selectbets.maxamout = parseInt((maxbet / 10).toFixed(1).split('.')[0]) * 10;
                    var betinfotemplate = html.join("");
                    var render = template.compile(betinfotemplate);
                    $(".userBetemplate").html("").html(render(selectbets));
                    $.popup('.popup-useret');

                } else if (data.code === 98 || data.code === 99) {
                    app.clearLogin(data.errors);
                } else {
                    $.fn.cookie('FT.Mobile.Amount', 0);
                }
            },
            error: function (xhr, type) {
                var response = JSON.parse(xhr.response);
                $.toast(response.ExceptionMessage);
            },
            complete: function () {
                $.hideIndicator();
                timerpopup = new app.SiteTimer(function () {
                    $('.timer-ten em').html('').html(timers);
                    timers--;
                    if (timers === -1) {
                        timers = 15;
                        match.refushBetContentIOR();
                    }
                }, 1000);

            }
        });
    };
    //修改投注金额，计算可赢金额
    match.changeBetAmount = function (obj) {
        var amount = parseFloat($(obj).val());
        if (amount < selectbets.minamount || amount > selectbets.maxamout || isNaN(amount)) {
            $('.canwin-amount').text(0);
            return;
        }
        var result = amount;
        selectbets.list.forEach(function (o, index) {
            result *= o.ior;
        });
        result -= amount;
        $('.canwin-amount').text(result.toFixed(2));
    };
    //比赛玩法swiper
    match.bettypeSwiper = function () {
        $(".swiper-container").swiper({
            pagination: '.swiper-pagination',
            slidesPerView: 'auto',
            spaceBetween: 0
        });
    };
    //15秒倒计时刷新赔率
    match.refushBetContentIOR = function () {
        timers = 15;
        var bets = JSON.parse("[" + userbet.betcontent + "]");
        var ids = "";
        $.each(bets, function (i, v) {
            ids += v.MatchID + ",";
        });
        ids = ids.substr(0, ids.length - 1);
        var params = {
            userid: app.siteConfig.siteuserid,
            ticket: app.siteConfig.siteuserticket,
            matchids: ids,
            datatype: "json",
            lang: languestorage.langue,
            zone: app.siteConfig.clienttimezone
        };
        var newiortext = "{0}@{1}";
        params.sign = app.createParamSign(params);
        $.ajax({
            type: 'POST',
            url: app.siteConfig.matchdetail,
            data: params,
            dataType: 'json',
            async: false,
            context: $('body'),
            success: function (data) {
                if (data) {
                    $.each(bets, function (bi, bv) {
                        $.each(data.data, function (i, v) {
                            $.each(v.Odds, function (ci, cv) {
                                if (bv.BetType == cv.BetType && bv.MatchID == v.MatchID) {
                                    var sobj;
                                    var selindex;
                                    $.each(selectbets.list, function (si, sv) {
                                        if (sv.matchid == v.MatchId) {
                                            sobj = sv;
                                            selindex = si;
                                        }
                                    });
                                    var newior = cv.BetIOR.split('|')[sobj.iorindex];
                                    var oldior = bv.BetContent.split('@')[1];
                                    //console.log(newior+'\n'+oldior);
                                    if (oldior != newior) {
                                        $(".userbet" + v.MatchID).text(newiortext.format(sobj.bet, newior));
                                        selectbets.list[selindex].ior = newior;
                                        var temp = "{\"MatchID\":" + bv.MatchID + ",\"BetType\":" + bv.BetType + ",\"BetContent\":\"" + bv.BetKey + "@" + oldior + "\",\"BetKey\":\"" + bv.BetKey + "\"}";
                                        userbet.betcontent.splice($.inArray(temp, userbet.betcontent), 1);
                                        userbet.betcontent.push("{\"MatchID\":" + bv.MatchID + ",\"BetType\":" + bv.BetType + ",\"BetContent\":\"" + bv.BetKey + "@" + newior + "\",\"BetKey\":\"" + bv.BetKey + "\"}");

                                    }
                                    return false;
                                }
                            });
                        });

                    });
                }
            }
        });
    };
})();