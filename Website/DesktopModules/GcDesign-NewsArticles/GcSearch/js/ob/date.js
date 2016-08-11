function FloatPanel(n, t, i, r)
{
    var f = $("#" + n),
        u = $("#" + t),
        e = !1;
    i = i || "bottom",
    f.bind("mouseenter", function () {
        if (!e) {
            var t = $(this),
                n = t.offset();
            switch (i) {
                case "top": u.show().css({ top: n.top - u.height(), left: n.left, "z-index": "990", position: "absolute" });
                    break;
                case "left": u.show().css({ top: n.top + t.outerHeight(), right: n.left, "z-index": "990", position: "absolute" }); break; case "middle-left": u.show().css({ top: n.top - (u.outerHeight() - t.outerHeight()) / 2, left: n.left - u.outerWidth(), "z-index": "990", position: "absolute" });
                    break;
                case "right": u.show().css({ top: n.top, left: n.left + t.outerWidth(), "z-index": "990", position: "absolute" }); break; case "middle-right": u.show().css({ top: n.top - (u.outerHeight() - t.outerHeight()) / 2, left: n.left + t.outerWidth(), "z-index": "990", position: "absolute" });
                    break;
                case "right-bottom": u.show().css({ top: n.top + t.outerHeight(), left: n.left - (u.outerWidth() - f.outerWidth()), "z-index": "990", position: "absolute" });
                    break;
                case "bottom": u.show().css({ top: n.top + t.outerHeight(), left: n.left, "z-index": "990", position: "absolute" })
            }
            r && f.addClass(r)
        }
    }).bind("mouseleave", function (n) {
        var i = $(n.relatedTarget);
        i.attr("id") != t && i.parents("#" + t).length == 0 && (e = !1, r && f.removeClass(r), u.hide())
    }),
    u.bind("mouseleave", function (t) {
        var i = $(t.relatedTarget);
        i.attr("id") != n && i.parents("#" + n).length == 0 && (e = !1, u.hide(), r && f.removeClass(r))
    })
}
function gotoTop() {
    var n = $('<div class="backToTop"></div>').appendTo($("body")).click(function () {
        $("html, body").animate({ scrollTop: 0 }, 120)
    }),
    t = function () {
        var t = $(document).scrollTop(),
            i = $(window).height();
        t > 0 ? n.show() : n.hide(),
        window.XMLHttpRequest || n.css("top", t + i - 166)
    };
    n.css("left", $("#footer").offset().left + $("#footer").outerWidth()).show(),
    $(window).bind("scroll", t), $(function () {
        t()
    })
} function compareDate(n, t, i) {
    var n = n.getTime(), t = t.getTime();
    return i ? n - t > 0 :
        n - t >= 0
} function getDayToString(n) {
    var t = new Date,
        r = new Date(+t + 864e5),
        u = new Date(+t + 1728e5),
        i = n.toDateString();
    return t.toDateString() == i ? "今天" :
        r.toDateString() == i ? "明天" :
        u.toDateString() == i ? "后天" :
        dateArray[n.getDay()]
} function parseDate(n, t) {
    if (n !== undefined) {
        if (n.constructor == Date)
            return n;
        if (typeof n == "string") {
            var i = n.split("-");
            if (i.length == 3)
                return new Date(integer(i[0]), integer(i[1]) - 1, integer(i[2]));
            if (!/^-?\d+$/.test(n))
                return;
            n = integer(n)
        }
        return t.setDate(t.getDate() + n), t
    }
}
function getQueryString(n) {
    var i = new RegExp("(^|&)" + n + "=([^&]*)(&|$)", "i"), t = window.location.search.substr(1).match(i);
    return t != null ? unescape(t[2]) : null
}
function getTimeCompare(n, t) {
    return t - n > 0 && n + t - timeData[0] - timeData[1] != 0
}
function parseDate(n) {
    if (n !== undefined) {
        if (n.constructor == Date)
            return n;
        if (typeof n == "string") {
            var t = n.split("-");
            if (t.length == 3)
                return new Date(integer(t[0]), integer(t[1]) - 1, integer(t[2]))
        }
    }
}
function integer(n) {
    return parseInt(n, 10)
}
function changeToLeftTime(n) {
    var t = parseInt(n / 3600), i = parseInt((n - t * 3600) / 60), r = n % 60;
    return t = t > 9 ? t + "" : "0" + t, i = i > 9 ? i + "" : "0" + i, r = r > 9 ? r + "" : "0" + r, t + ":" + i + ":" + r
}
function initHotCityTab() {
    var n = [];
    $("#menuBoxhotcity ul").tabs("#hotunitsContent",
        {
            effect: "ajax",
            onBeforeClick: function (t, i) {
                var u = n[0],
                    r = this.getTabs().eq(i).parent(),
                    f = r.outerWidth(!0) + 1;
                pos = r.position(),
                u && (u.removeClass(), n = []),
                n.push(r),
                this.getTabs().eq(i).parent().addClass("current")
            }
        })
}
var mindate, maxdate, timeData, dateArray, criteriaSelectApi, DilogContent, $searchTab;
$("#citySelect").length && $("#citySelect").selectinput(
    {
        offset: [2, -184],
        css: {
            rootclass: "selectList selectListW120",
            headclass: "selectTitle-2",
            mouseon: "mouseon",
            current: "current"
        }
    }).selEnter(),
$(".searcSubmit").click(function () {
    $("#topSearchForm").attr("action", "/" + $("#citySelect option:selected").attr("name") + "_gongyu/")
}),
$("#headWrapper").after($("#mytujia")),
$("#headWrapper").after($("#mylandlord")),
FloatPanel("alldestination", "destination", "right-bottom", "active"),
FloatPanel("phonetab", "phonetabdrop", "right-bottom", "active"),
FloatPanel("mirmsg", "mirmsgdrop", "right-bottom", "active"),
mindate = parseDate(minDate),
maxdate = parseDate(maxDate),
$.fn.extend(
    {
        ie6fixedbug: function () {
            if ($("#kefuWrap").length > 0 && $.browser.version == 6 && !$.support.style) {
                $(this).css("position", "absolute");
                var n = $(this).offset().top - (document.body.scrollTop || document.documentElement.scrollTop), i, t = $(this)[0];
                window.onscroll = function () {
                    clearTimeout(i),
                    setTimeout(function () {
                        t.style.top = (document.body.scrollTop || document.documentElement.scrollTop) + n + "px"
                    }, 200)
                },
                window.onresize = function () {
                    t.style.top = (document.body.scrollTop || document.documentElement.scrollTop) + n + "px"
                }
            }
            return $(this)
        }
    }),
$(function () {
    function i(n) {
        staticFileRoot && $("[remind]").html("<img src='" + staticFileRoot + "/common/images/ui-anim_basic_16x16.gif'/>"),
        $.ajax(
            {
                type: "Get",
                url: n,
                success: function (n) {
                    for (var t in n)
                        $("[remind='" + t + "']") && (n[t] == "0" ? $("[remind='" + t + "']").parent().remove() : $("[remind='" + t + "']").html(n[t]))
                }
            })
    }
    var t, n;
    $("#kefuWrap").ie6fixedbug(),
    houseId || ($("#startDate").dateinput({ min: mindate, max: new Date(+maxdate - 864e5) }),
    $("#endDate").dateinput({ min: new Date(+mindate + 864e5), max: maxdate })),
    $("#mytujiamenu").length && i($("#mytujiamenu").attr("remindSummaryUrl")),
    $("#mylandlordmenu").hover(function () {
        var n = $($(this).attr("data-rel"));
        n.css(
            {
                position: "absolute",
                "z-index": "99999",
                left: $("#mylandlordmenu").offset().left,
                top: $("#mylandlordmenu").offset().top + $("#mylandlordmenu").height()
            }),
        n.show()
    }, function () {
        var n = $($(this).attr("data-rel"));
        t = setTimeout(function () {
            n.hide()
        }, 50),
        n.hover(function () {
            clearTimeout(t)
        }, function () {
            n.hide()
        })
    }),
    $("input").attr({ autocomplete: "off" }),
    n = $("#keyword"),
    n.blur(function () {
        n.val() || n.val("景点/地址/特色").addClass("defaultColor")
    }).focus(function () {
        n.val() == "景点/地址/特色" && n.val("").removeClass("defaultColor")
    }),
    $("#topSearchForm").submit(function () {
        n.val() == "景点/地址/特色" && n.val("")
    }), n.val() ? n.val() != "景点/地址/特色" && n.removeClass("defaultColor") : n.val("景点/地址/特色")
}),
$.lazydom(
    {
        items: $("div[data-lazydom]"),
        delay: 50, before: 100
    }),
$("#navBox li[data-rel]").menu(), function (n) {
    n.fn.homeselect = function (t) {
        var r = n(this).data("selectinput"),
            e = r.getHead(),
            o = r.getSelect(),
            u, i, f;
        t = r.getConf(),
        u = n("<div class='chamber fl'><ul/></div>"),
        i = u.find("ul"),
        i.addClass("clearfix"),
        o.find("option").each(function (t, r) {
            var u = n(r),
                f = u.prop("selected") ? "current" : "";
            i.append(n('<li><a href="" class="' + f + '" data-index=' + t + ">" + u.text() + "</a></li>"))
        }),
        f = i.find("a"),
        i.find("li").bind("click", function () {
            r.setIndex(n(this).index()),
            f.removeClass("current"),
            n(this).find("a").addClass("current")
        }), e.empty().html(u)
    }, n("*[leftSeconds]").each(function () {
        var t = n(this),
            i = t.attr("leftSeconds") - 0,
            r = window.setInterval(function () {
                var n = changeToLeftTime(i--);
                t.html(n), i < 0 && (window.clearInterval(r),
                r = null,
                window.location.href = window.location.href)
            }, 1e3)
    })
}(jQuery),
dateArray = ["星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六"],
criteriaSelectApi = $("#criteriaSelect").selectinput(
    {
        offset: [0, -1],
        css: {
            rootclass: "selectList selectW245",
            headclass: "selectTitle",
            mouseon: "mouseon",
            current: "current"
        }
    }).selEnter(), $("#searchHouse").click(function () {
        $("#mainSearchForm").attr("action", "/" + $("#criteriaSelect option:selected").attr("name") + "_gongyu/"),
        $("#mainSearchForm").trigger("submit")
    }), $(function () {
        var t = $("#startDate").data("dateinput"),
            n = $("#endDate").data("dateinput"),
            u = $("#startDate").val(), r, i;
        u && (i = t.getValue(), $("#startDate").next().text(getDayToString(i))),
        r = $("#endDate").val(),
        r && (i = n.getValue(), $("#endDate").next().text(getDayToString(i))),
        t.change(function (i, r) {
            var u = parseDate(n.getInput().val()),
                f = new Date(+r + 864e5);
            t.getInput().next().next().hide(), !u || compareDate(r, u) ? n.setMin(f).setValue(f).show() : u && n.setMin(f), $("#startDate").next().text(getDayToString(r))
        }), n.change(function (i, r) { var u = parseDate(t.getInput().val()), f = new Date(+r - 864e5); n.getInput().next().next().hide(), (!u || compareDate(u, r)) && t.setValue(f).show(), $("#endDate").next().text(getDayToString(r)) }), $.each([t, n], function (i, r) { var e = r.getInput(), u = !i ? n.getInput() : t.getInput(), f = u.next().next(); r.onEmpty(function () { e.next().empty(), u.val("").next().empty(), f.show() }); u.bind("focus", function () { f.hide() }).bind("blur", function () { u.val() || f.show() }), u.val() ? f.hide() : f.show() }), $(".datewatermark").bind("click", function () { $(this).prev().prev().trigger("click") }), $("#addEmailBtn").each(function () { var n = $(this); n.overlay({ mask: { color: "#000", loadSpeed: 200, opacity: .6 }, close: ".closeBtn", closeOnClick: !1, closeOnEsc: !0, target: "#dialog", top: "30%", fixed: !1, onBeforeLoad: function () { var n = this.getOverlay(); $("#dialog p").html(DilogContent), $.ajax({ type: "Post", url: "/Edmemail/Subscribe/", data: { email: $("#edmemail").val() }, success: function (n) { if (n.IsSuccess) { var t = "http://mail." + $("#edmemail").val().split("@")[1]; $("#dialog p").html('恭喜您已成功订阅，现在就去<a href="' + t + '" target="_blank" style="color: #6699CC;text-decoration: underline;margint: 0 5px;">验证邮箱</a>即可参与抽取iPhone5大奖的活动哦，周周都有惊喜').show() } else $("#dialog p").html(n.Message).show() } }) } }) })
    }), $(function () { $("#J-sidebarCode").css("left", $("#footer").offset().left + $("#footer").outerWidth()).show(), $backToTopFun = function () { var n = $(document).scrollTop(), t = $(window).height(); $("#J-sidebarCode").css("left", $("#footer").offset().left + $("#footer").outerWidth()), window.XMLHttpRequest || $("#J-sidebarCode").css("top", n + 300) }, $(window).bind("scroll", $backToTopFun), $(window).resize($backToTopFun), $("#J-closeBtn").click(function () { $("#J-sidebarCode").hide() }), $backToTopFun() }), DilogContent = $("#dialog p").html(), $searchTab = $("#searchBox > div.index-searchBox > div"), $("#searchBox > div.search-tab > ul").tabs("#searchBox > div.index-searchBox > div", { onBeforeClick: function (n, t) { criteriaSelectApi.hide(); var r = $searchTab.eq(t), i = r.find("select"); $searchTab.find("select").attr("disabled", !0), $searchTab.find("input").attr("disabled", !0), i.attr("disabled", !1), r.find("input").attr("disabled", !1), $.each(i, function (n, t) { $(t).data("selectinput") || i.selectinput({ offset: [0, -1], css: { rootclass: "selectList selectListW60", headclass: "selectTitle", mouseon: "mouseon", current: "current" } }) }) } }), FloatPanel("worldplace", "worldDestination"), $carousel = $("#img_carousel").carousel({ loop: !0, indicator: !0, duration: .3 }); $("#focusimage .u-upBtn").on("click", function (n) { n.preventDefault(), $carousel.carousel("prev") }); $("#focusimage .u-nextBtn").on("click", function (n) { n.preventDefault(), $carousel.carousel("next") }); setInterval(function () { $carousel.carousel("next") }, 3e3), initHotCityTab(), function (n) { n.fn.maskPic = function (t) { var i = n.extend({}, n.fn.maskPic.defaults, t); return this.each(function () { var t = n(this), r = t.find("li"); r.each(function () { var r = n(this), u = n(this).find("img").attr("width"), f = n(this).find("img").attr("height"), e = "<span style='display:none;width:" + u + "px;height:" + f + "px;position:absolute;left:0px; top:0px; background:rgb(0, 0, 0); opacity:0; cursor: pointer;' class='" + i.maskClass + "'></span>"; r.find("a").append(e) }), r.hover(function () { n(this).find("." + i.maskClass).css({ opacity: 0, display: "block" }).end().siblings().find("." + i.maskClass).css({ opacity: i.opacity, display: "block" }) }), t.mouseleave(function () { n("." + i.maskClass).css("opacity", 0) }) }) }, n.fn.maskPic.defaults = { maskClass: "mask", opacity: "0.3" } }(jQuery)