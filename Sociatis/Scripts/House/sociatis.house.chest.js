$(function () {


});

function showTransfer(itemID, button) {
    //$.notify(itemID);
    var $button = $(button);
    var $div = $("div[data-transfer_itemid='" + itemID + "']");

    var $buttons = $button.parent().find("button");

    $div.show(500);
    $buttons.hide(400, () =>
        $buttons.animate({ margin: 0, width: 0, border: 0 }, 400)
    );
}


function showDrop(itemID, button) {
    //$.notify(itemID);
    var $button = $(button);
    var $buttons = $button.parent().find("button");
    var $div = $("div[data-drop_itemid='" + itemID + "']");


    $div.show(500);
    $buttons.hide(400, () =>
        $buttons.animate({ margin: 0, width: 0, border: 0 }, 400)
    );
}