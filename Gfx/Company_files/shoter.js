/*!
 * jQuery PostJson Plugin
 *
 * https://github.com/shoter/hasAttr
 *
 * Copyright (c) 2017 Damian Łączak
 * Released under the MIT license
 */
(function ($) {

    var paddle = function ($paddle) {

        var getValue = function () {
           return  getLinked(self).val().toLowerCase() === "true";
        }

        var setValue = function (value) {
            return getLinked(self).val(value ? "true" : "false");
            $linked.val(value);
        }

        var onClick = function () {
            var $this = $(this);
            var activated = !getValue();
            setValue(!getValue());
            var $linked = getLinked($this);
            $linked.trigger("change");
        }

        var onChange = function () {
            var $this = $(this);
            if ($this.val().toLowerCase() === "true") {
                self.removeClass("disabled");
                self.addClass("activated");
            }
            else {
                self.addClass("disabled");
                self.removeClass("activated");
            }
        }

        var onRefresh = function () {
            onChange.call(this);
        }

        var getLinked = function ($paddle) {
            var linkedID = $(self).data("paddle-linkedid");
            return $("#" + linkedID);
        }

        var self = $paddle;
        getLinked(self).change(onChange);
        getLinked(self).on("refresh", onRefresh);
        self.click(onClick);
    }

    $.fn.paddle = function (args) {
        return this.each(function () {
            paddle($(this));
        });
       
    };

}(jQuery));

$(function () {
    $("[data-paddle]").paddle();
});