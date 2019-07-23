$(function () {
    var update = function(checkbox)
    {
        var checked = checkbox.checked;
        var $target = $("[name='" + checkbox.dataset.target + "']");
        if (checked) {
            $target.val("");
            $target.attr("disabled", "true");
        } else {
            $target.removeAttr("disabled");
        }
    }

    $("[id^='price'][id$='Disable']").change(function (e) {
        update(e.target);
    });

    $("[id^='price'][id$='Disable']").each(function (i, c) {
        update(c);
    });

});