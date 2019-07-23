
DynamicTooltip = (function () {
    var DynamicTooltip = function ($$element) {
        this.$element = $($$element);
    };

    DynamicTooltip.prototype.unlock = function () {
        if (this.$element.attr("data-tooltip")) {
            this.$element.foundation("destroy");
        }
        this.$element.attr("title", "");
        this.$element.removeAttr("title");
        this.$element.removeAttr("disabled");
        this.$element.removeClass("disabled");
    }

    DynamicTooltip.prototype.lock = function (message) {
        this.unlock(); 
        this.$element.addClass("disabled");
        this.$element.attr("title", message);
        this.$element.attr("disabled", true);
        new Foundation.Tooltip(this.$element);
    }

    return DynamicTooltip;
})();