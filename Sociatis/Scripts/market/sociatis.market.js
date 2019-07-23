function calculateTotal(amount, offerID) {
   
    var $input = $("input[data-offerid='" + offerID + "']");
    if (!(amount) || !(offerID))
        return;

    var min = Number($input.attr("min"));
    var max = Number($input.attr("max"));

    if (amount < min) {
        $input.val(min);
        amount = min;
    } else if (amount > max) {
        $input.val(max);
        amount = max;
    }

    var data =
        {
            offerID: offerID,
            amount: amount
        };


    var url = Sociatis.GetAdress("CalculateTotal", "MarketOffer");

    var updateField = function (data) {
        $("div[data-pricecostofferid='" + offerID + "']").text(data.ProductPrice);
        $("div[data-fuelcostofferid='" + offerID + "']").text(data.FuelPrice);
        console.log(data);
    };

    $.postJSON(url, data, data => Sociatis.HandleJson(data, updateField));
}

function recalculateTotal(offerID) {
    var amount = $("input[data-offerid='" + offerID + "']").val();
    calculateTotal(amount, offerID);

}