$(function () {
    $("[data-removeOffer]").click(function () {
        var offerID = this.getAttribute("data-id");
        var $row = $(this).parents("div.row.flex");

        if (confirm("You will not receive any refund for removing this offer. Are you sure?")) {
            var url = "/MarketOffer/RemoveOffer";

            Sociatis.AjaxBegin("Removing offer");
            $.post(url, { offerID: offerID }, function (data) {
                Sociatis.HandleJson(data, onOfferRemove, $row)
                Sociatis.AjaxEnd();
            });
        }
    });


});


function onOfferRemove(data, $row) {
    var message = data.Message;
    $row.remove();
    alert(message);
};