$(function () {
    $("#ProducedProductID").change(function () {
        var val = this.value;
        if (val === "")
            return;
        updateQualityModifications(val);
        updateFertilityModifications(val);

    });

    $("form#calculator").submit(function () {
        $(":input[disabled]").removeAttr("disabled");
    });

    function updateQualityModifications(productID) {
        Sociatis.UI.BlockUI();

        var url = Sociatis.GetAdress("CanUseQuality", "Calculator");
        var data = {
            productTypeID: productID
        };

        $.postJSON(url, data, (data) => Sociatis.HandleJson(data, function (data) {
            if (data.Data) {
                unblock("#Quality");
            } else {
                block("#Quality");
                $("#Quality").val(1);
            }

            Sociatis.UI.UnblockUI();

        }));
    };

    function updateFertilityModifications(productID) {
        Sociatis.UI.BlockUI();

        var url = Sociatis.GetAdress("CanUseFertility", "Calculator");
        var data = {
            productTypeID: productID
        };

        $.postJSON(url, data, (data) => Sociatis.HandleJson(data, function (data) {

            if (data.Data) {
                unblock("#ResourceQuality");
            } else {
                block("#ResourceQuality");
            }
            Sociatis.UI.UnblockUI();
        }));
    }

    function block(element) {
        $(element).attr("disabled", true);
    }

    function unblock(element) {
        $(element).removeAttr("disabled");
    }

});


