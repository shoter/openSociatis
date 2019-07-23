$(function () {
    var $materials = $("#constructionMaterialsNeeded");
    var $quality = $("#Quality");
    var tooltip = new DynamicTooltip("#hotelCreateRoomButton");
    var hotelID = Sociatis.Global.hotelID;

    var onQualityChange = function (e) {
        var value = e.target.value;
        updateCost(value);
        updateAvailability(value);
    };



    var updateCost = function (quality) {
        var url = "/Hotel/CreateRoomCost";
        var data = {
            hotelID: hotelID,
            quality: quality
        };
        Sociatis.UI.BlockUI();
        $.postJSON(url, data, function (data) {
            Sociatis.HandleJson(data, updateCostSuccess);
        });
    };

    var updateAvailability = function (quality) {
        var url = "/Hotel/CanCreateRoom";
        var data = {
            hotelID: hotelID,
            quality: quality
        };
        Sociatis.UI.BlockUI();
        $.postJSON(url, data, function (data) {
            Sociatis.HandleJson(data, updateAvailabilitySuccess, null, updateAvailabilityError);
        });
    };
    var updateAvailabilityError = function (response) {
        tooltip.lock(response.ErrorMessage);
        Sociatis.UI.UnblockUI();
    };
    var updateAvailabilitySuccess = function (response) {
        tooltip.unlock();
        Sociatis.UI.UnblockUI();

    };

    var updateCostSuccess = function(response)
    {
        var cost = response.Data;
        $materials.text(cost);
        Sociatis.UI.UnblockUI();
    }

    $quality.change(onQualityChange);

    var quality = $quality.val();
    updateCost(quality);
    updateAvailability(quality);

});