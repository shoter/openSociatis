$(function () {
    $(document).on("click", "button[name='removeOffer']", function () {
        var offerID = $(this).attr("data-ID");

        var url = "/MonetaryMarket/GetRemoveCost";

        Sociatis.AjaxBegin("Loading");
        $.post(url, { offerID: offerID }, function (data) {
            Sociatis.AjaxEnd();
            Sociatis.HandleJson(data, areYouSureWantDeleteThis, offerID)
        });
    });
});

function areYouSureWantDeleteThis(data, offerID)
{
    var message = "The removal of this offer is free of charge. Do you want to proceed?";
    if (data.Data.Cost != "")
        message = "The removal of this offer costs " + data.Data.Cost + "(tax). It will be deducted from reserved money. Do you want to proceed?";

    if(confirm(message))
    {
        var url = "/MonetaryMarket/RemoveOffer";
        Sociatis.AjaxBegin("Loading");
        $.post(url, { offerID: offerID }, function (data) {
            Sociatis.HandleJson(data, onRemove, offerID)
            Sociatis.AjaxEnd();
        });
    }
}

function onRemove(data, offerID)
{
    alert(data.Message);
    $("button[data-ID='" + offerID + "']").parents("tr").remove();
    Sociatis.ReloadSummary();
}