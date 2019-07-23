window.Sociatis = window.Sociatis || {};
Sociatis.Trade = Sociatis.Trade || {};


$(() => {
    $("input[data-currencyid]").change(function () {
        var $this = $(this);
        var val = $this.val();
        var min = parseFloat($this.attr("min"));
        var max = parseFloat($this.attr("max"));
        if (!(parseFloat(val))) {
            val = min;
        }

        if (val < min) val = min;
        if (val > max) val = max;

        $this.val(val);
    });

});

Sociatis.Trade.SetMaxMoney = function (currencyID) {
    var $input = $("input[data-currencyid='" + currencyID + "']");
    var max = $input.attr('max');
    $input.val(max);
}

Sociatis.Trade.SetMaxProduct = function (productID, quality) {
    var $input = $("input[data-productid='" + productID + "'][data-quality='" + quality + "']");
    var max = $input.attr('max');
    $input.val(max);
    $input.trigger("change");
}

Sociatis.Trade.AddMoney = function (tradeID, currencyID) {
    if (Sociatis.Trade.CanAddMoney === false) return;

    var $input = $("input[data-currencyid='" + currencyID + "']");
    var val = $input.val();
    var symbol = $input.data("symbol");

    var data =
        {
            tradeID: Sociatis.Trade.ActualTradeID,
            currencyID: currencyID,
            amount: val
        };

    var url = Sociatis.GetAdress("AddMoney", "Trade");

    if (confirm("Do you want to add " + val + " " + symbol + "?")) {
        Sociatis.AjaxBegin();
        $.postJSON(url, data, (data) => Sociatis.HandleJson(data, Sociatis.Trade.PostMoneyAdd));
    }
}

Sociatis.Trade.PostMoneyAdd = function (data, destinationID) {
    $.notify(data.Message, "success");
    Sociatis.Trade.Reload();
}

Sociatis.Trade.Reload = function () {
    var url = "/trade/" + Sociatis.Trade.ActualTradeID;
    window.location.replace(url);
}

Sociatis.Trade.AddProduct = function (productID, quality, productName) {
    if (Sociatis.Trade.CanAddProduct === false)
        return;

    var $input = $("input[data-productid='" + productID + "'][data-quality='" + quality + "']");
    var $fuelNode = $(".fuelCost[data-productid='" + productID + "'][data-quality='" + quality + "']");
    var fuelCost = $fuelNode.data("fuelcost");
    var val = $input.val();
    var symbol = $input.data("symbol");

    var data =
        {
            tradeID: Sociatis.Trade.ActualTradeID,
            productID: productID,
            quality: quality,
            amount: val
        };

    var url = Sociatis.GetAdress("AddProduct", "Trade");

    var message = "Do you want to add " + val + " " + productName + "?";
    if (fuelCost > 0)
        message += " You will need to use " + fuelCost + " fuel.";

    if (confirm(message)) {
        Sociatis.AjaxBegin();
        $.postJSON(url, data, (data) => Sociatis.HandleJson(data, Sociatis.Trade.PostProductAdd));
    }
}

Sociatis.Trade.PostProductAdd = function (data) {
    $.notify(data.Message, "success");
    Sociatis.Trade.Reload();
}