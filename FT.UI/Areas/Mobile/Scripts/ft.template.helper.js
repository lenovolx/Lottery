/** 
 * 对日期进行格式化， 
 * @param date 要格式化的日期 
 * @param format 进行格式化的模式字符串
 *     支持的模式字母有： 
 *     y:年, 
 *     M:年中的月份(1-12), 
 *     d:月份中的天(1-31), 
 *     h:小时(0-23), 
 *     m:分(0-59), 
 *     s:秒(0-59), 
 *     S:毫秒(0-999),
 *     q:季度(1-4)
 */
template.helper('dateFormat', function (date, format) {
	if (typeof date === "string") {
		var mts = date.match(/(\/Date\((\d+)\)\/)/);
		if (mts && mts.length >= 3) {
			date = parseInt(mts[2]);
		}
	}
	date = new Date(date);
	if (!date || date.toUTCString() == "Invalid Date") {
		return "";
	}
	var map = {
		"M": date.getMonth() + 1, //月份 
		"d": date.getDate(), //日 
		"h": date.getHours(), //小时 
		"m": date.getMinutes(), //分 
		"s": date.getSeconds(), //秒 
		"q": Math.floor((date.getMonth() + 3) / 3), //季度 
		"S": date.getMilliseconds() //毫秒 
	};

	format = format.replace(/([yMdhmsqS])+/g, function (all, t) {
		var v = map[t];
		if (v !== undefined) {
			if (all.length > 1) {
				v = '0' + v;
				v = v.substr(v.length - 2);
			}
			return v;
		}
		else if (t === 'y') {
			return (date.getFullYear() + '').substr(4 - all.length);
		}
		return all;
	});
	return format;
});
template.helper('letball', function(horc, team, betkey, betior, language) {
    //H|0.5  C|1
    var str = "";
    if (betkey !== "") {
        var letarry = betkey.split('|');
        var hc;
        if (horc === 'H') {
            hc = language === 'cn' ? "主胜" : "H";
            if (letarry[0] === 'H')
                str = "<span>" + team + "<em class=\"reduce\">-" + letarry[1] + "</em></span><span>" + hc + "&nbsp;" + betior + "</span>";
            else
                str = "<span>" + team + "</span><span>" + hc + "&nbsp;" + betior + "</span>";
        } else {
            hc = language === 'cn' ? "客胜" : "C";
            if (letarry[0] === 'C')
                str = "<span>" + team + "<em class=\"reduce\">-" + letarry[1] + "</em></span><span>" + hc + "&nbsp;" + betior + "</span>";
            else
                str = "<span>" + team + "</span><span>" + hc + "&nbsp;" + betior + "</span>";
        }
    }
    return str;
});

template.helper('letballpars', function (type, betkey) {
    var horc = type === 0 ? "H" : "C";
	var str = horc;
	if (betkey !== "") {
		var letarry = betkey.split('|');
		if (horc === 'H') {
			if (letarry[0] === 'H')
				str = betkey;
			else
				str = "H";
		} else {
			if (letarry[0] === 'C')
				str = betkey;
			else
				str = "C";
		}
	}
	return str;
});
template.helper('strmerge', function (str1, str2) {
    return str1 + ' vs ' + str2;
});
template.helper('ouball', function (key,betkey, betior, language) {
    if (language === 'cn') {
        if (key === 'O') return '<span>大' + betkey + '&nbsp;' + betior + '</span>';
        return '<span>小' + betkey + '&nbsp;' + betior + '</span>';
    } else
        return '<span>' + key + betkey + '&nbsp;' + betior + '</span>';
});
template.helper('amountoutput', function (amount) {
    return amount === 0 ? "-" : amount.toFixed(2);
});
template.helper('classmerge', function (key, ior) {
    return '<span class=\'bold\'>' + key + '</span><span>' + ior + '</span>';
});
template.helper("matchurl", function(s, l) {
    switch (s) {
    case 0:
        return l !== '' ? "/m/match?league=" + l : "/m/match";
    default:
        return l !== '' ? "/m/rollball?league=" + l : "/m/rollball";
    }
});