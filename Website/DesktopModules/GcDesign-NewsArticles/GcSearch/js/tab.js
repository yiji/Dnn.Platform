//tabs插件-jackin稍作改进
(function ($) {
    $.fn.extend({
        qmTabs: function () {
            var aTabBodys = $('#tabs-body > div');
            $(this).children("li").each(function (index) {
                $(this).click(function () {
                    $(this).removeClass().addClass('tab-nav-action').siblings().removeClass().addClass('tab-nav');
                    aTabBodys.hide().eq(index).show();
                });
            });
        }
    });
})(jQuery);