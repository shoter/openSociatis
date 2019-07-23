/*!
 * jQuery PostJson Plugin
 *
 * https://github.com/shoter/hasAttr
 *
 * Copyright (c) 2017 Damian Łączak
 * Released under the MIT license
 */
(function ($) {

    $.postJSON = (url, data, success, error) => {
        if (error)
            return $.post(url, data, success, "json")
                .error(error);
        else
        return $.post(url, data, success, "json");
    }

}(jQuery));