/*!
 * jQuery disabled Plugin
 *
 * --
 *
 * Copyright (c) 2017 Damian Łączak
 * Released under the MIT license
 */
(function ($) {

    $.fn.disable = function() {
        this.prop("disabled", true);
        this.addClass("disabled");
        return this;
    }

    $.fn.enable = function () {
        this.prop("disabled", false);
        this.removeClass("disabled");
        return this;
    }

}(jQuery));