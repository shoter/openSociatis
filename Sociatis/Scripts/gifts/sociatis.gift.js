window.Sociatis = window.Sociatis || {};
Sociatis.Gifts = Sociatis.Gifts || {};

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

Sociatis.Gifts.SetMaxMoney = function (currencyID) {
    var $input = $("input[data-currencyid='" + currencyID + "']");
    var max = $input.attr('max');
    $input.val(max);
}

Sociatis.Gifts.SendMoneyGift = function (currencyID) {
    if (Sociatis.Gifts.ReceiveMoney === false) return;
    var $input = $("input[data-currencyid='" + currencyID + "']");
    var val = $input.val();
    var symbol = $input.data("symbol");
    var destination =
        {
            id: $("#DestinationID").val(),
            name: $("#DestinationID option:selected").text()
        };

    var data =
        {
            destinationID: destination.id,
            currencyID: $input.data("currencyid"),
            amount: val
        };

    var url = Sociatis.GetAdress("SendMoney", "Gift");

    if (confirm("Do you want to send " + val + " " + symbol + " to " + destination.name + "?")) {
        Sociatis.AjaxBegin();
        $.postJSON(url, data, (data) => Sociatis.HandleJson(data, Sociatis.Gifts.PostMoneyGift, destination.id));
    }
}

Sociatis.Gifts.PostMoneyGift = function (data, destinationID) {
    $.notify(data.Message, "success");
    var url = "/gift/" + destinationID;
    window.location.replace(url);
}

Sociatis.Gifts.SetMaxProduct = function (productID, quality) {
    var $input = $("input[data-productid='" + productID + "'][data-quality='"+quality+"']");
    var max = $input.attr('max');
    $input.val(max);
    $input.trigger("change");
}

Sociatis.Gifts.OnDestinationChange = function () {
    var destID = $("#DestinationID").val();
    
    Sociatis.Gifts.ClearFuel();
    if (!destID) {
        Sociatis.Gifts.UseFuel = false;
        Sociatis.Gifts.SetProductReceive(false, "You cannot send gift into void!");
        Sociatis.Gifts.SetMoneyReceive(false, "You cannot send money into void!");
        Sociatis.Gifts.CanReceiveProductGiftsID = null;
        Sociatis.Gifts.UseFuelDestinationID = null;
        return;
    }
    Sociatis.Gifts.SetMoneyReceive(true);
    Sociatis.Gifts.CanReceiveProductGifts(data => {
        if (data.Data)
            Sociatis.Gifts.WillUseFuel((data) => {
                if (data.Data) Sociatis.Gifts.RecalculateFuelForAll()
            });
        else
            Sociatis.Gifts.UseFuel = false;

    });
}

Sociatis.Gifts.ClearFuel = function () {
    $(".fuelCost").text("");
    $(".fuelCost").data("fuelcost", "");
}

Sociatis.Gifts.RecalculateFuelForAll = function () {
    var inputs = $("input[data-productid][data-quality]");

    $.each(inputs, (i, el) => {
        Sociatis.Gifts.RecalculateFuel(el);
    });
}

Sociatis.Gifts.SendProductGift = function (productID, quality, productName) {
    if (Sociatis.Gifts.ReceiveProducts === false) return;
    var $input = $("input[data-productid='" + productID + "'][data-quality='" + quality + "']");
    var $fuelNode = $(".fuelCost[data-productid='" + productID + "'][data-quality='" + quality + "']");
    var fuelCost = $fuelNode.data("fuelcost");
    var val = $input.val();
    var symbol = $input.data("symbol");
    var destination =
        {
            id: $("#DestinationID").val(),
            name: $("#DestinationID option:selected").text()
        };

    var data =
        {
            destinationID: destination.id,
            productID: productID,
            quality: quality,
            amount: val
        };

    var url = Sociatis.GetAdress("SendProduct", "Gift");

    var message = "Do you want to send " + val + " " + productName + " to " + destination.name + "? ";
    if (fuelCost > 0)
        message += "You will need to use " + fuelCost + " fuel.";

    if (confirm(message)) {
        Sociatis.AjaxBegin();
        $.postJSON(url, data, (data) => Sociatis.HandleJson(data, Sociatis.Gifts.PostProductGift, destination.id));
    }
}

Sociatis.Gifts.PostProductGift = function (data, destinationID) {
    $.notify(data.Message, "success");
    var url = "/gift/" + destinationID;
    window.location.replace(url);
}

Sociatis.Gifts.RecalculateFuel = function (self) {
    if (Sociatis.Gifts.UseFuel === false)
        return;
    var $this = $(self);
    var productID = $this.data("productid");
    var quality = $this.data("quality");
    var val = $this.val();
    var destination =
        {
            id: $("#DestinationID").val(),
            name: $("#DestinationID option:selected").text()
        };

    var data =
        {
            destinationID: destination.id,
            productID: productID,
            quality: quality,
            amount: val
        };


    var url = Sociatis.GetAdress("GetFuelCost", "Gift");
    $.postJSON(url, data, (data) => Sociatis.HandleJson(data, Sociatis.Gifts.PostFuelCost, { id: productID, quality: quality }));
}

Sociatis.Gifts.WillUseFuel = function (callback) {
    

    var destinationID = $("#DestinationID").val();

    if (Sociatis.Gifts.UseFuelDestinationID === destinationID)
        return;

    var url = Sociatis.GetAdress("WillGiftUseFuel", "Gift");
    var data = {
        destinationID: destinationID
    };

    $.postJSON(url, data, (data) => Sociatis.HandleJson(data, data => {
        Sociatis.Gifts.UseFuel = data.Data;
        Sociatis.Gifts.UseFuelDestinationID = destinationID;
        callback(data);
    }));
}

Sociatis.Gifts.CanReceiveProductGifts = function (callback) {
   
    var destinationID = $("#DestinationID").val();
    if (Sociatis.Gifts.CanReceiveProductGiftsID === destinationID)
        return;

    Sociatis.AjaxBegin();
    var url = Sociatis.GetAdress("CanReceiveProductGifts", "Gift");
    var data = {
        destinationID: destinationID
    };

    $.postJSON(url, data, (data) => Sociatis.HandleJson(data, data => {
        Sociatis.Gifts.SetProductReceive(data.Data.result, data.Data.error);
        Sociatis.Gifts.CanReceiveProductGiftsID = destinationID;
        callback(data);
        Sociatis.AjaxEnd();
    }));
}

Sociatis.Gifts.SetProductReceive = function (state, error) {
    Sociatis.Gifts.ReceiveProducts = state;
    var $buttons = $("button.productSendButton");
    if ($buttons.attr("data-tooltip")) {
        $buttons.foundation("destroy");
        $buttons.attr("title", "");
        $buttons.removeAttr("title");
    }

    if (Sociatis.Gifts.ReceiveProducts) {

        $buttons.removeClass("disabled");
    }
    else {
        $buttons.addClass("disabled");
        $buttons.attr("title", error);
        $.each($buttons, (i, button) => {
            new Foundation.Tooltip($(button));
        });
    }
}
Sociatis.Gifts.SetMoneyReceive = function (state, error) {
    Sociatis.Gifts.ReceiveMoney = state;
    var $buttons = $("button.moneySendButton");

    if ($buttons.attr("data-tooltip")) {
        $buttons.foundation("destroy");
        $buttons.attr("title", "");
        $buttons.removeAttr("title");
    }

    if (Sociatis.Gifts.ReceiveMoney) {
        $buttons.removeClass("disabled");

    }
    else {
        $buttons.addClass("disabled");
        $buttons.attr("title", error);
        $.each($buttons, (i, button) => {
            new Foundation.Tooltip($(button));
        });
    }

}



Sociatis.Gifts.PostFuelCost = function (data, args) {
    Sociatis.Gifts.SetFuel(args.id, args.quality, data.Data);
}

Sociatis.Gifts.SetFuel = function (productID, quality, fuel) {
    var $node = $(".fuelCost[data-productid='" + productID + "'][data-quality='" + quality + "']");
    $node.text(fuel + " fuel");
    $node.data("fuelcost", fuel);

}