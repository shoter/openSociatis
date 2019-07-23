$(function () {
    var $roomQualitySelect = $("#roomRentQuality");
    var $roomNights = $("#roomRentNights");
    var $totalCost = $("#roomRentCost");
    var $rentButton = $("#rentRoom");

    var hotelID = Sociatis.Global.HotelID;
    var quality = 1;
    var nights = 1;

    var onQualityChange = function (e) {
        quality = e.target.value;
        onParameterChange(quality, nights);
    }

    var onNightsChange = function (e) {
        nights = e.target.value;
        onParameterChange(quality, nights);
    }

    var onParameterChange = function (quality, nights) {
        if (!quality || !nights)
            nullifyRoomCost();
        else {
            getRoomCost(quality, nights, updateCost);
            canRentRoomGetInfo(quality, nights);
        }
    };

    var nullifyRoomCost = function (response) {
        if (response)
            $.notify(response.ErrorMessage, "error");
        $totalCost.text("-");
    };

    var updateCost = function (response) {
        $totalCost.text(response.Data);
        Sociatis.UI.UnblockUI();
    }

    var getRoomCost = function (quality, nights, callback) {
        Sociatis.UI.BlockUI();
        var url = "/Hotel/GetRoomCost";
        var data =
            {
                hotelID: hotelID,
                quality: quality,
                nights: nights
            };

        $.postJSON(url, data, function (data) { Sociatis.HandleJson(data, callback, null, nullifyRoomCost); Sociatis.UI.UnblockUI(); });
    }

    var canRentRoomGetInfo = function (quality, nights) {
        if (!quality || !nights)
            return;
        Sociatis.UI.BlockUI();
        var url = "/Hotel/CanRentRoom";
        var data =
            {
                hotelID: hotelID,
                quality: quality,
                nights: nights
            };

        $.postJSON(url, data, function (data) {
            Sociatis.HandleJson(data, unlockButton, null, lockButton);
        }, function () { lockButton("error"); });
    }

    var unlockButton = function () {
        if ($rentButton.attr("data-tooltip")) {
            $rentButton.foundation("destroy");
            $rentButton.attr("title", "");
            $rentButton.removeAttr("title");
        }
        $rentButton.removeAttr("disabled");
        $rentButton.removeClass("disabled");
        Sociatis.UI.UnblockUI();

    }

    var lockButton = function (response) {
        if ($rentButton.attr("data-tooltip")) {
            $rentButton.foundation("destroy");
            $rentButton.attr("title", "");
            $rentButton.removeAttr("title");
        }

        $rentButton.addClass("disabled");
        $rentButton.attr("title", response.ErrorMessage);
        $rentButton.attr("disabled", true);
        new Foundation.Tooltip($rentButton);
        Sociatis.UI.UnblockUI();
    };


    var init = function () {
        quality = $roomQualitySelect.val();
        nights = $roomNights.val();
        onParameterChange(quality, nights);
        $roomQualitySelect.change(onQualityChange);
        $roomNights.change(onNightsChange);
    }

    init();

});