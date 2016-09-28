var userbet = {
    betvalue: 0,
    betcontent: []
};
var league_bettype = {
    bettype: 10,
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
    matchid: 0,
    timezon: app.siteConfig.clienttimezone
};
var selectbets = {
    league: '',
    amount:0,
    list: [],
    maxamout: app.siteConfig.sitebetmxnamount,
    minamount: app.siteConfig.sitebetminamount
};
var languestorage = {
    langue: app.siteConfig.sitelanguage
};
var timerpopup;
var selectMatch = 0; //选择的比赛场次数量
$(function() {
    "use strict";
    match.loadLanguage(languestorage.langue);
    $(document).on("pageInit", "#match-list", function(e, id, page) {
        var parms = app.parseUrlQuery(location.href);
        league_bettype.league = typeof(parms.league) == "undefined" ? "" : parms.league;
        var times = app.siteConfig.refreshtimes;
        match.loadLeagueMatch(league_bettype.bettype, league_bettype.league);
        var $content = page.find('.content');
        $content.on('click', '.tab-link', function() {
            league_bettype.bettype = $(this).attr("bettype");
            match.loadLeagueMatch(league_bettype.bettype, league_bettype.league);
            times = app.siteConfig.refreshtimes;
        });
        //清空选择
        page.find('#btn_clear').on('click', function() {
            match.clearMatch();
        });
        //投注
        //page.find('#btn_ret').on('click', function() {
        //    match.userBetTemplate(0);
        //});
        //手动刷新
        page.on('click', '.match-refresh', function() {
            match.loadLeagueMatch(league_bettype.bettype, league_bettype.league);
            times = app.siteConfig.refreshtimes;
        });
        //自动刷新
        var timer = new app.SiteTimer(function() {
            page.find('.match-refresh em').html('').html(times);
            times--;
            if (times === -1) {
                match.loadLeagueMatch(league_bettype.bettype, league_bettype.league);
                times = app.siteConfig.refreshtimes;
            }
        }, 1000);
        $(page).find(".content").on('refresh', function(e) {
            times = app.siteConfig.refreshtimes;
            match.loadLeagueMatch(league_bettype.bettype, league_bettype.league);
            $.pullToRefreshDone($content);
        });
    });
    //滚动方法
    match.bettypeSwiper();
    $.init();
});
var match = match || {};
(function() {
    //加载语言包
    match.loadLanguage = function(language) {
        Zepto.i18n.properties({
            name: 'language',
            path: '/i18n/mobile/',
            mode: 'map',
            language: language,
            callback: function() {
                $(".title").html($.i18n.prop('home_head_title'));
                $(".tab1").text($.i18n.prop('match_tab1_text'));
                $(".tab2").text($.i18n.prop('match_tab2_text'));
                $(".tab3").text($.i18n.prop('match_tab3_text'));
                $(".tab4").text($.i18n.prop('match_tab4_text'));
                $(".tab5").text($.i18n.prop('match_tab5_text'));
                $(".tab6").text($.i18n.prop('match_tab6_text'));
                $(".tab7").text($.i18n.prop('match_tab7_text'));
                $(".tab8").text($.i18n.prop('match_tab8_text'));
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
            success: function(data) {
                if (data.code === 0) {
                    var html = template('match_list_tpl_' + bettype, data.data);
                    $(".tab.active").find(".match_list_ul").html(html);
                } else if (data.code === 98 || data.code === 99) {
                    app.clearLogin(data.errors);
                } else {
                    $.toast(data.message || data.errors);
                }
            },
            error: function(xhr, type) {
                var response = JSON.parse(xhr.response);
                $.toast(response.ExceptionMessage);
            },
            complete: function (xhr) {
                $.hideIndicator();
            }
        });
    };
    //玩法选择
    match.bettypeSelect = function(betdb, league, hteam, cteam, matchid, betior, bettypeid, betkey, obj, index) {
        //userbet.betcontent = [];
        //切换选项卡之后清空已投注信息
        var betObj = eval("[" + userbet.betcontent.toString() + "]");
        if (betObj.length > 0) {
            betObj.forEach(function(o, index) {
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
        selectbet.CurrentScore = "";
        switch (bettypeid) {
            case 10:
                selectbet.team = hteam + '&nbsp;<font color=\'red\'>vs</font>&nbsp;' + cteam;
                if (betdb === "H") {
                    selectbet.bet = hteam;
                } else if (betdb === "N") {
                    selectbet.bet = app.siteConfig.sitelanguage === 'cn' ? "和局" : "Draw";
                } else {
                    selectbet.bet = cteam;
                }
                break;
            case 12:
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
            case 14:
                selectbet.team = hteam + '&nbsp;<font color=\'red\'>vs</font>&nbsp;' + cteam;
                if (betdb === "O") {
                    selectbet.bet = (app.siteConfig.sitelanguage === 'cn' ? "大" : "O") + "&nbsp;<font color=red'>" + betkey + "</font>";
                    selectbet.key = betkey;
                } else {
                    selectbet.bet = (app.siteConfig.sitelanguage === 'cn' ? "小" : "U") + "&nbsp;<font color=red'>" + betkey + "</font>";
                    selectbet.key = betkey;
                }
                break;
            case 16:
                selectbet.team = hteam + '&nbsp;<font color=\'red\'>vs</font>&nbsp;' + cteam;
                if (betkey.split('|')[0] === 'O') {
                    selectbet.bet = app.siteConfig.sitelanguage === 'cn' ? "单" : "Odd";
                } else {
                    selectbet.bet = app.siteConfig.sitelanguage === 'cn' ? "双" : "Even";
                }
                break;
            case 20:
                selectbet.team = hteam + '&nbsp;<font color=\'red\'>vs</font>&nbsp;' + cteam;
                selectbet.bet = betdb;
                break;
            case 30:
                selectbet.team = hteam + '&nbsp;<font color=\'red\'>vs</font>&nbsp;' + cteam;
                selectbet.bet = betdb;
                break;
            case 40:
                selectbet.team = hteam + '&nbsp;<font color=\'red\'>vs</font>&nbsp;' + cteam;
                var draw = app.siteConfig.sitelanguage === 'cn' ? "和" : "Draw";
                var betarry = betkey.split('|');
                if (betarry[0] === 'HH')
                    selectbet.bet = '<font color=\'red\'>' + hteam + '/' + hteam + '</font>';
                else if (betarry[0] === 'HC')
                    selectbet.bet = '<font color=\'red\'>' + hteam + '/' + cteam + '</font>';
                else if (betarry[0] === 'HN')
                    selectbet.bet = '<font color=\'red\'>' + hteam + '/' + draw + '</font>';
                else if (betarry[0] === 'CC')
                    selectbet.bet = '<font color=\'red\'>' + cteam + '/' + cteam + '</font>';
                else if (betarry[0] === 'CH')
                    selectbet.bet = '<font color=\'red\'>' + cteam + '/' + hteam + '</font>';
                else if (betarry[0] === 'CN')
                    selectbet.bet = '<font color=\'red\'>' + cteam + '/' + draw + '</font>';
                else if (betarry[0] === 'NN')
                    selectbet.bet = '<font color=\'red\'>' + draw + '/' + draw + '</font>';
                else if (betarry[0] === 'NC')
                    selectbet.bet = '<font color=\'red\'>' + draw + '/' + cteam + '</font>';
                else
                    selectbet.bet = '<font color=\'red\'>' + draw + '/' + hteam + '</font>';
                break;
            case 5:
                selectbet.team = hteam + '&nbsp;<font color=\'red\'>vs</font>&nbsp;' + cteam;
                selectbet.minlimit = parseInt(limitrow.attr("data-minlimit"));
                selectbet.maxlimit = parseInt(limitrow.attr("data-maxlimit"));
                switch (arguments[10]) {
                    case "50":
                        if (betdb === "H") {
                            selectbet.bet = hteam;
                        } else if (betdb === "N") {
                            selectbet.bet = app.siteConfig.sitelanguage === 'cn' ? "和局" : "Draw";
                        } else {
                            selectbet.bet = cteam;
                        }
                        break;
                    case "52":
                        var zhletarry = betkey.split('|');
                        selectbet.key = betkey;
                        if (betdb === 'H') {
                            selectbet.bet = hteam;
                            if (zhletarry[0] === 'H') {
                                selectbet.team = hteam + '&nbsp;<font color=\'red\'>' + zhletarry[1] + '</font>&nbsp;vs&nbsp;' + cteam;
                            } else {
                                selectbet.team = hteam + '&nbsp;vs&nbsp;' + cteam;
                            }
                        } else {
                            selectbet.bet = cteam;
                            if (zhletarry[0] === 'C') {
                                selectbet.team = hteam + '&nbsp;vs&nbsp;<font color=\'red\'>' + zhletarry[1] + '</font>&nbsp;' + cteam;
                            } else {
                                selectbet.team = hteam + '&nbsp;vs&nbsp;' + cteam;
                            }
                        }
                        break;
                }
                break;
        }
        var betTemp = "";
        //综合过关可以多选，其他玩法只能单选
        if (bettypeid != "5") {
            $('.bettype-' + bettypeid).removeClass('match-select');
            $(obj).addClass('match-select').siblings().removeClass('match-select');
            selectMatch = 1;
            userbet.betcontent = [];
            selectbets.list = [];
            betTemp = "{\"MatchID\":" + matchid + ",\"BetType\":" + bettypeid + ",\"BetContent\":\"" + betdb + "@" + betior + "\",\"BetKey\":\"" + selectbet.key + "\",\"BetScore\":\"\"}";
            userbet.betcontent.push(betTemp);
        } else {
            $(".-" + matchid).siblings().removeClass('match-select');
            $(obj).addClass('match-select');
            betObj.forEach(function(o, index) {
                if (o.MatchID == matchid) {
                    var temp = "{\"MatchID\":" + o.MatchID + ",\"BetType\":" + o.BetType + ",\"BetContent\":\"" + o.BetContent.split('@')[0] + "@" + o.BetContent.split('@')[1] + "\",\"BetKey\":\"" + o.BetKey + "\",\"BetScore\":\"\"}";
                    userbet.betcontent.splice($.inArray(temp, userbet.betcontent), 1);
                    selectMatch--;
                }
            });
            selectbets.list.forEach(function(o, index) {
                if (o.matchid == matchid) {
                    selectbets.list.splice($.inArray(o, selectbets.list), 1);
                }
            });
            betTemp = "{\"MatchID\":" + matchid + ",\"BetType\":" + arguments[10] + ",\"BetContent\":\"" + betdb + "@" + betior + "\",\"BetKey\":\"" + selectbet.key + "\",\"BetScore\":\"\"}";
            userbet.betcontent.push(betTemp);
            selectMatch++;
        }
        $(".selected_match").html($.i18n.prop('match_bet_select').format(selectMatch));
        selectbets.list.push(selectbet);
        selectbet = {};
        var minlimit = 0;
        var maxlimit = 0;
        selectbets.list.forEach(function(o, i) {
            if (o.minlimit > minlimit) {
                minlimit = o.minlimit;
            }
            if (maxlimit == 0) {
                maxlimit = o.maxlimit;
            }
            if (maxlimit > o.maxlimit) {
                maxlimit = o.maxlimit;
            }
        });
        if (bettypeid == "5") {
            if (selectbets.list.length >= minlimit && selectbets.list.length <= maxlimit) {
                $("#btn_ret").removeClass("disabled");
                $("#btn_ret").on("click", function() {
                    match.userBetTemplate(0);
                });
            } else {
                $("#btn_ret").addClass("disabled");
                $("#btn_ret").off("click");
            }
        } else {
            $("#btn_ret").removeClass("disabled");
            $("#btn_ret").on("click", function () {
                match.userBetTemplate(0);
            });
        }
        //console.log(selectbet);
    };
    //清除选择
    match.clearMatch = function() {
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
    match.confirmRet = function() {
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
        $.confirm($.i18n.prop('match_bet_comfirm_alert'), function() {
            var params = {
                "ticket": app.siteConfig.siteuserticket,
                "userid": app.siteConfig.siteuserid,
                "matchuserbet": "{\"BetValue\":" + parseFloat(userbet.betvalue) + ",\"MatchUserBetContent\":[" + userbet.betcontent.join(',') + "]}",
                "lang": app.siteConfig.sitelanguage,
                "source": 0,
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
                success: function(data) {
                    if (data.code === 0) {
                        $.alert($.i18n.prop('common_success_text'), '', function() {
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
    match.showLeagueMatch = function(bettype, obj) {
        $(obj).toggleClass("zhover");
    };
    //半全场、波胆玩法模版
    match.moreBetTemplate = function() {
        //比赛球队 head
        var html = ["<header class=\"bar bar-nav\">"];
        html.push("<h1 class=\"title\"><span> {{hteam}} VS {{cteam}}</span></h1>");
        html.push("</header>");
        html.push("<div class=\"content\">");
        html.push("<div class=\"list-block\">");
        //半全场 开始
        html.push("<ul>{{if type === 40}}");
        html.push("<li class=\"item-content row\">");
        html.push("<div class=\"item-media col-20 col-10\">半全场</div>");
        html.push("<div class=\"col-80 col-90\">");
        html.push("{{each bet as item i}}");
        html.push("<div class=\"row\">");
        html.push("<div class=\"col-33 text-center match-bettype bettype-40\" onclick=\"match.bettypeSelect('{{item[0].key}}','{{league}}', '{{hteam}}','{{cteam}}', '{{matchid}}', '{{item[0].ior}}', 40,'{{item[0].key}}', this,'{{item[0].index}}')\"><span>{{item[0].key}}</span><span>{{item[0].ior}}</span></div>");
        html.push("<div class=\"col-33 text-center match-bettype bettype-40\" onclick=\"match.bettypeSelect('{{item[1].key}}','{{league}}', '{{hteam}}','{{cteam}}', '{{matchid}}', '{{item[1].ior}}', 40,'{{item[1].key}}', this,'{{item[1].index}}')\"><span>{{item[1].key}}</span><span>{{item[1].ior}}</span></div>");
        html.push("<div class=\"col-33 text-center match-bettype bettype-40\" onclick=\"match.bettypeSelect('{{item[2].key}}','{{league}}', '{{hteam}}','{{cteam}}', '{{matchid}}', '{{item[2].ior}}', 40,'{{item[2].key}}', this,'{{item[2].index}}')\"><span>{{item[2].key}}</span><span>{{item[2].ior}}</span></div>");
        html.push("</div>");
        html.push("{{/each}}");
        html.push("</div>");
        html.push("</li>");
        html.push("{{else}}");
        //半全场 结束
        //波胆 开始
        html.push("<li class=\"item-content row\">");
        html.push("<div class=\"item-media col-20 col-10\">波胆</div>");
        html.push("<div class=\"col-80  col-90\">");
        html.push("{{each bet as item i}}");
        html.push("<div class=\"row\">");
        //正常25种波胆玩法开始
        html.push("{{if i!='D0'}}");
        html.push("<div class=\"col-20 text-center match-bettype bettype-30\" onclick=\"match.bettypeSelect('{{item[0].key}}', '{{league}}', '{{hteam}}','{{cteam}}', '{{matchid}}', '{{item[0].ior}}', 30,'{{item[0].key}}',this,'{{item[0].index}}')\"><span>{{item[0].key}}</span><span>{{item[0].ior}}</span></div>");
        html.push("<div class=\"col-20 text-center match-bettype bettype-30\" onclick=\"match.bettypeSelect('{{item[1].key}}', '{{league}}', '{{hteam}}','{{cteam}}', '{{matchid}}', '{{item[1].ior}}', 30,'{{item[1].key}}',this,'{{item[1].index}}')\"><span>{{item[1].key}}</span><span>{{item[1].ior}}</span></div>");
        html.push("<div class=\"col-20 text-center match-bettype bettype-30\" onclick=\"match.bettypeSelect('{{item[2].key}}', '{{league}}', '{{hteam}}','{{cteam}}', '{{matchid}}', '{{item[2].ior}}', 30,'{{item[2].key}}',this,'{{item[2].index}}')\"><span>{{item[2].key}}</span><span>{{item[2].ior}}</span></div>");
        html.push("<div class=\"col-20 text-center match-bettype bettype-30\" onclick=\"match.bettypeSelect('{{item[3].key}}', '{{league}}', '{{hteam}}','{{cteam}}', '{{matchid}}', '{{item[3].ior}}', 30,'{{item[3].key}}',this,'{{item[3].index}}')\"><span>{{item[3].key}}</span><span>{{item[3].ior}}</span></div>");
        html.push("<div class=\"col-20 text-center match-bettype bettype-30\" onclick=\"match.bettypeSelect('{{item[4].key}}', '{{league}}', '{{hteam}}','{{cteam}}', '{{matchid}}', '{{item[4].ior}}', 30,'{{item[4].key}}',this,'{{item[4].index}}')\"><span>{{item[4].key}}</span><span>{{item[4].ior}}</span></div>");
        html.push("{{else}}");
        //其他玩法单独处理
        html.push("<div class=\"text-center match-bettype bettype-30\" onclick=\"match.bettypeSelect('{{item[0].key}}', '{{league}}', '{{hteam}}','{{cteam}}', '{{matchid}}', '{{item[0].ior}}', 30,'{{item[0].key}}',this,'{{item[0].index}}')\"><span>{{item[0].key}}</span><span>{{item[0].ior}}</span></div>");
        html.push("{{/if}}");
        html.push("</div>");
        html.push("{{/each}}");
        html.push("</div>");
        html.push("</li>");
        html.push("{{/if}}");
        //波胆 结束
        html.push("</ul>");
        html.push("</div>");
        html.push("</div>");
        return html.join("");
    };
    //半全场、波胆玩法弹出层
    match.showMatchPopup = function(league, matchid, bettype, hteam, cteam, betkey, betior) {
        var betpopup = ["{\"league\":\"" + league + "\",\"matchid\":" + matchid + ",\"type\":" + bettype + ",\"hteam\":\"" + hteam + "\",\"cteam\":\"" + cteam + "\",\"bet\":{"],
            bet = [];
        var keysplit = betkey.split('|');
        var iorsplit = betior.split('|');
        //A,胜;B,平,;C,负;D,其他;
        if (bettype === 40) {
            bet.push("\"A\":[{\"key\":\"" + keysplit[0] + "\",\"ior\":" + iorsplit[0] + ",\"index\":0},{\"key\":\"" + keysplit[1] + "\",\"ior\":" + iorsplit[1] + ",\"index\":1},{\"key\":\"" + keysplit[2] + "\",\"ior\":" + iorsplit[2] + ",\"index\":2}]");
            bet.push("\"C\":[{\"key\":\"" + keysplit[3] + "\",\"ior\":" + iorsplit[3] + ",\"index\":3},{\"key\":\"" + keysplit[4] + "\",\"ior\":" + iorsplit[4] + ",\"index\":2},{\"key\":\"" + keysplit[5] + "\",\"ior\":" + iorsplit[5] + ",\"index\":5}]");
            bet.push("\"B\":[{\"key\":\"" + keysplit[6] + "\",\"ior\":" + iorsplit[6] + ",\"index\":6},{\"key\":\"" + keysplit[7] + "\",\"ior\":" + iorsplit[7] + ",\"index\":7},{\"key\":\"" + keysplit[8] + "\",\"ior\":" + iorsplit[8] + ",\"index\":8}]");
        } else {
            bet.push("\"A0\":[{\"key\":\"" + keysplit[0] + "\",\"ior\":" + iorsplit[0] + ",\"index\":0},{\"key\":\"" + keysplit[1] + "\",\"ior\":" + iorsplit[1] + ",\"index\":1},{\"key\":\"" + keysplit[2] + "\",\"ior\":" + iorsplit[2] + ",\"index\":2},{\"key\":\"" + keysplit[3] + "\",\"ior\":" + iorsplit[3] + ",\"index\":3},{\"key\":\"" + keysplit[4] + "\",\"ior\":" + iorsplit[4] + ",\"index\":4}]");
            bet.push("\"A1\":[{\"key\":\"" + keysplit[5] + "\",\"ior\":" + iorsplit[5] + ",\"index\":5},{\"key\":\"" + keysplit[6] + "\",\"ior\":" + iorsplit[6] + ",\"index\":6},{\"key\":\"" + keysplit[7] + "\",\"ior\":" + iorsplit[7] + ",\"index\":7},{\"key\":\"" + keysplit[8] + "\",\"ior\":" + iorsplit[8] + ",\"index\":8},{\"key\":\"" + keysplit[9] + "\",\"ior\":" + iorsplit[9] + ",\"index\":9}]");
            bet.push("\"B0\":[{\"key\":\"" + keysplit[10] + "\",\"ior\":" + iorsplit[10] + ",\"index\":10},{\"key\":\"" + keysplit[11] + "\",\"ior\":" + iorsplit[11] + ",\"index\":11},{\"key\":\"" + keysplit[12] + "\",\"ior\":" + iorsplit[12] + ",\"index\":12},{\"key\":\"" + keysplit[13] + "\",\"ior\":" + iorsplit[13] + ",\"index\":13},{\"key\":\"" + keysplit[14] + "\",\"ior\":" + iorsplit[14] + ",\"index\":14}]");
            bet.push("\"C0\":[{\"key\":\"" + keysplit[16] + "\",\"ior\":" + iorsplit[16] + ",\"index\":16},{\"key\":\"" + keysplit[17] + "\",\"ior\":" + iorsplit[17] + ",\"index\":17},{\"key\":\"" + keysplit[18] + "\",\"ior\":" + iorsplit[18] + ",\"index\":18},{\"key\":\"" + keysplit[19] + "\",\"ior\":" + iorsplit[19] + ",\"index\":19},{\"key\":\"" + keysplit[20] + "\",\"ior\":" + iorsplit[20] + ",\"index\":20}]");
            bet.push("\"C1\":[{\"key\":\"" + keysplit[21] + "\",\"ior\":" + iorsplit[21] + ",\"index\":21},{\"key\":\"" + keysplit[22] + "\",\"ior\":" + iorsplit[22] + ",\"index\":22},{\"key\":\"" + keysplit[23] + "\",\"ior\":" + iorsplit[23] + ",\"index\":23},{\"key\":\"" + keysplit[24] + "\",\"ior\":" + iorsplit[24] + ",\"index\":24},{\"key\":\"" + keysplit[25] + "\",\"ior\":" + iorsplit[25] + ",\"index\":25}]");
            bet.push("\"D0\":[{\"key\":\"" + keysplit[15] + "\",\"ior\":" + iorsplit[15] + ",\"index\":15}]");
        }
        betpopup.push(bet.join(','));
        betpopup.push("}}");
        var betinfo = $.parseJSON(betpopup.join(''));
        //console.log(betinfo);
        var bettemplate = match.moreBetTemplate();
        var render = template.compile(bettemplate);
        var html = render(betinfo);
        $.modal({
            text: html,
            buttons: [{
                text: $.i18n.prop('common_cacel_text'),
                bold: true,
                onClick: function() {
                    match.clearMatch();
                    $(".bet-context-" + matchid).html($.i18n.prop('match_bet_select_toast'));
                }
            }, {
                text: $.i18n.prop('common_confirm_text'),
                onClick: function() {
                    if (selectbets.list[0].bet != '')
                        match.confirmPopup(matchid);
                }
            }]
        });
    };
    //半全场、波胆弹出层确定
    match.confirmPopup = function(matchid) {
        $('.popup-modul').html($.i18n.prop('match_bet_select_toast'));
        $(".bet-context-" + matchid).html(selectbets.list[0].bet);
    };
    //关闭弹出层、定时器
    match.closeBetPopup = function() {
        $.closeModal();
        timerpopup.Stop();
    };
    //投注
    match.userBetTemplate = function(type) {
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
            success: function(data) {
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
                    html.push("<a class=\"icon icon-refresh pull-right timer-ten\" onclick=\"match.clickRefresh()\">");
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
                    $.each(selectbets.list, function(i, v) {
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
            error: function (xhr,type) {
                var response = JSON.parse(xhr.response);
                $.toast(response.ExceptionMessage);
            },
            complete: function() {
                $.hideIndicator();
                timerpopup = new app.SiteTimer(function() {
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
    match.changeBetAmount = function(obj) {
        var amount = parseFloat($(obj).val());
        if (amount < selectbets.minamount || amount > selectbets.maxamout || isNaN(amount)) {
            $('.canwin-amount').text(0);
            return;
        }
        var result = amount;
        selectbets.list.forEach(function(o, index) {
            result *= o.ior;
        });
        result -= amount;
        $('.canwin-amount').text(result.toFixed(2));
    };
    //比赛玩法swiper
    match.bettypeSwiper = function() {
        $(".swiper-container").swiper({
            pagination: '.swiper-pagination',
            slidesPerView: 'auto',
            spaceBetween: 0
        });
    };
    match.clickRefresh=function() {
        timers = 15;
        $('.timer-ten em').html('').html(timers);
        match.refushBetContentIOR();
    }
    //15秒倒计时刷新赔率
    match.refushBetContentIOR = function() {
        var bets = JSON.parse("[" + userbet.betcontent + "]");
        var ids = "";
        $.each(bets, function(i, v) {
            ids += v.MatchID + ",";
        });
        ids = ids.substr(0, ids.length - 1);
        var params = {
            userid: app.siteConfig.siteuserid,
            ticket: app.siteConfig.siteuserticket,
            matchids: ids,
            datatype: "json",
            lang: languestorage.langue,
            "zone": app.siteConfig.clienttimezone
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
            success: function(data) {
                if (data) {
                    $.each(bets, function(bi, bv) {
                        $.each(data.data, function(i, v) {
                            $.each(v.Odds, function(ci, cv) {
                                if (bv.BetType == cv.BetType && bv.MatchID == v.MatchId) {
                                    var sobj;
                                    var selindex;
                                    $.each(selectbets.list, function(si, sv) {
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