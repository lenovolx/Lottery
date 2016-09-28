var languageArry = { "cn": "简体中文", "en": "English", "pt": "Portugal" };
var app = app || {};
(function () {
    var hostapi = "/api/", signsecrt = "ZD4417JEFFDDSCC50H3FAE3C787D0E23";
    //获取url参数
    app.parseUrlQuery = function (url) {
        var query = {}, i, param;
        if (url.indexOf("?") >= 0)
            url = url.split("?")[1];
        else
            return query;
        var params = url.split("&");
        for (i = 0; i < params.length; i++) {
            param = params[i].split("=");
            query[param[0]] = param[1];
        }
        return query;
    };
    //参数签名
    app.createParamSign = function (params) {
        var param = [], sign = [];
        for (var i in params) {
            if (params.hasOwnProperty(i)) {
                if (params[i] !== "")
                    param.push([i, params[i]]);
            }
        }
        //不为空参数生成sign值操作
        $.each(param.sort(function (x, y) {
            return x[0].localeCompare(y[0]);
        }), function (i, v) {
            sign.push(v[0] + "=" + v[1]);
        });
        var str = sign.join("&");
        return hex_md5(str + "&key=" + signsecrt).toUpperCase();
    };
    //网站配置
    app.siteConfig = {
        sitelanguage: $.fn.cookie('FT.Mobile.Langue') == null ? 'cn' : $.fn.cookie('FT.Mobile.Langue'), //网站语言
        siteuserticket: $.fn.cookie('FT.Mobile.Ticket') == null ? 0 : $.fn.cookie('FT.Mobile.Ticket'), //Ticket
        siteuserid: $.fn.cookie('FT.Mobile.UserId') == null ? 0 : parseInt($.fn.cookie('FT.Mobile.UserId')), //UserId
        siteusername: $.fn.cookie('FT.Mobile.UserName') == null ? '' : $.fn.cookie('FT.Mobile.UserName'), //UserName
        sitebetminamount: $.fn.cookie('FT.Mobile.MinBetAmount') == null ? 0 : parseFloat($.fn.cookie('FT.Mobile.MinBetAmount')), //MinBetAmount
        sitebetmxnamount: $.fn.cookie('FT.Mobile.MaxBetAmount') == null ? 0 : parseFloat($.fn.cookie('FT.Mobile.MaxBetAmount')), //MaxBetAmount
        clienttimezone: $.fn.cookie('FT.Client.TimeZone') == null ? 8 : parseFloat($.fn.cookie('FT.Client.TimeZone')), //MaxBetAmount
        refreshtimes: 180, //数据定时刷新时间(默认180秒)
        refreshtimesrb: 25, //滚球数据定时刷新时间(默认30秒)
        matchlist: hostapi + "match/list", //获取比赛
        matchdetail: hostapi + "match/detail", //比赛明细
        userlogin: hostapi + "user/login", //用户登录
        userlogout: hostapi + "user/logout", //用户登录
        userregister: hostapi + "user/register", //用户注册
        userbet: hostapi + "user/bet", //用户投注
        matchleague: hostapi + "match/league", //联赛信息
        validcard: hostapi + "system/card", //充值卡校验
        usertrad: hostapi + "user/trade", //用户充值转账
        modifypass: hostapi + "user/editpass", //用户修改密码
        adduserbank: hostapi + "user/adduserbank", //添加提现账户
        deluserbank: hostapi + "user/DelUserBank", //删除提现账户
        userbanklist: hostapi + "user/userbank", //获取用户绑定帐号列表
        singleuserbank: hostapi + "user/SingleUserBank", //获取用户绑定帐号
        edituserbank: hostapi + "user/EditUserBank", //修改用户绑定帐号
        userhistory: hostapi + "user/History", //修改用户绑定帐号
        userbetrecord: hostapi + "user/BetRecord", //修改用户绑定帐号
        usercenter: hostapi + "user/center", //获取用户基本信息
        usercash: hostapi + "user/cash", //用户提现
        betcontent: hostapi + "user/contentdetail", //投注详情
        siteadvlist: hostapi + "system/adv",//轮播图
        sitedictionary: hostapi + "system/dict"//系统字典
    };
    //网站定时器
    app.SiteTimer = function (callback, times) {
        var timerObj = setInterval(callback, times);
        this.Stop = function () {
            if (timerObj) {
                clearInterval(timerObj);
                timerObj = null;
            }
            return this;
        }
        this.Start = function () {
            if (!timerObj) {
                this.Stop();
                timerObj = setInterval(callback, times);
            }
            return this;
        }
        this.Reset = function (newT) {
            times = newT;
            return this.Stop().Start();
        }
    };
    //提示退出
    app.clearLogin = function(msg) {
        $.alert(msg, function () {
            $.fn.cookie('FT.Mobile.Ticket', '', { expires: -1, path: '/m' });
            $.fn.cookie('FT.Mobile.Amount', 0, { expires: -1, path: '/m' });
            $.fn.cookie('FT.Mobile.UserId', 0, { expires: -1, path: '/m' });
            $.fn.cookie('FT.Mobile.UserName', '', { expires: -1, path: '/m' });
            location.href = "/m/login";
        });
    };
})();
var dateFormat = function () {
    var token = /d{1,4}|m{1,4}|yy(?:yy)?|([HhMsTt])\1?|[LloSZ]|"[^"]*"|'[^']*'/g,
        timezone = /\b(?:[PMCEA][SDP]T|(?:Pacific|Mountain|Central|Eastern|Atlantic) (?:Standard|Daylight|Prevailing) Time|(?:GMT|UTC)(?:[-+]\d{4})?)\b/g,
        timezoneClip = /[^-+\dA-Z]/g,
        pad = function (val, len) {
            val = String(val);
            len = len || 2;
            while (val.length < len) val = "0" + val;
            return val;
        };
    return function (date, mask, utc) {
        var dF = dateFormat;
        // You can't provide utc if you skip other args (use the "UTC:" mask prefix)
        if (arguments.length == 1 && Object.prototype.toString.call(date) == "[object String]" && !/\d/.test(date)) {
            mask = date;
            date = undefined;
        }

        // Passing date through Date applies Date.parse, if necessary
        date = date ? new Date(date) : new Date;
        if (isNaN(date)) throw SyntaxError("invalid date");

        mask = String(dF.masks[mask] || mask || dF.masks["default"]);

        // Allow setting the utc argument via the mask
        if (mask.slice(0, 4) === "UTC:") {
            mask = mask.slice(4);
            utc = true;
        }

        var _ = utc ? "getUTC" : "get",
            d = date[_ + "Date"](),
            D = date[_ + "Day"](),
            m = date[_ + "Month"](),
            y = date[_ + "FullYear"](),
            H = date[_ + "Hours"](),
            M = date[_ + "Minutes"](),
            s = date[_ + "Seconds"](),
            L = date[_ + "Milliseconds"](),
            o = utc ? 0 : date.getTimezoneOffset(),
            flags = {
                d: d,
                dd: pad(d),
                ddd: dF.i18n.dayNames[D],
                dddd: dF.i18n.dayNames[D + 7],
                m: m + 1,
                mm: pad(m + 1),
                mmm: dF.i18n.monthNames[m],
                mmmm: dF.i18n.monthNames[m + 12],
                yy: String(y).slice(2),
                yyyy: y,
                h: H % 12 || 12,
                hh: pad(H % 12 || 12),
                H: H,
                HH: pad(H),
                M: M,
                MM: pad(M),
                s: s,
                ss: pad(s),
                l: pad(L, 3),
                L: pad(L > 99 ? Math.round(L / 10) : L),
                t: H < 12 ? "a" : "p",
                tt: H < 12 ? "am" : "pm",
                T: H < 12 ? "A" : "P",
                TT: H < 12 ? "AM" : "PM",
                Z: utc ? "UTC" : (String(date).match(timezone) || [""]).pop().replace(timezoneClip, ""),
                o: (o > 0 ? "-" : "+") + pad(Math.floor(Math.abs(o) / 60) * 100 + Math.abs(o) % 60, 4),
                S: ["th", "st", "nd", "rd"][d % 10 > 3 ? 0 : (d % 100 - d % 10 != 10) * d % 10]
            };

        return mask.replace(token, function ($0) {
            return $0 in flags ? flags[$0] : $0.slice(1, $0.length - 1);
        });
    };
}();
dateFormat.masks = {
    "default": "ddd mmm dd yyyy HH:MM:ss",
    shortDate: "m/d/yy",
    mediumDate: "mmm d, yyyy",
    longDate: "mmmm d, yyyy",
    fullDate: "dddd, mmmm d, yyyy",
    shortTime: "h:MM TT",
    mediumTime: "h:MM:ss TT",
    longTime: "h:MM:ss TT Z",
    isoDate: "yyyy-mm-dd",
    isoTime: "HH:MM:ss",
    isoDateTime: "yyyy-mm-dd'T'HH:MM:ss",
    isoUtcDateTime: "UTC:yyyy-mm-dd'T'HH:MM:ss'Z'"
};
dateFormat.i18n = {
    dayNames: [
        "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat",
        "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"
    ],
    monthNames: [
        "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec",
        "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"
    ]
};
Date.prototype.format = function (mask, utc) {
    return dateFormat(this, mask, utc);
};
String.prototype.format = function() {
    var args = arguments;
    return this.replace(/\{(\d+)\}/g,
        function(m, i) {
            return args[i];
        });
};