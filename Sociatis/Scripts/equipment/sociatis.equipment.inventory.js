function showDelete(button) {
    var $button = $(button);
    var targetID = $button.data("showid");
    var $use = $("#"+$button.data("useid"));
    var $target = $("#"+targetID);
    $target.show(500);
    $button.hide(400, () =>
        $button.animate({ margin: 0, width: 0, border: 0 }, 400)
        );
    $use.animate({ marginLeft: 0 }, 800, () => $button.remove())
}