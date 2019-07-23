$(function () {

    $("#CountryID").change(function () {
        var val = $(this).val();
        var url = "/Company/MarketOfferPrice";
        if (val == "" || val == "0" || val == 0) {
            $("#MarketFee").html("Free");
            $("#freeHint").show();
        }
        else {
            Sociatis.AjaxBegin();
            $.get(url, { countryID: val }, function (data) {
                $("#MarketFee").html(data);
                $("#freeHint").hide();
                Sociatis.AjaxEnd();
            });
        }

        calculateCost();
    });


    $("#ProductID").change(function () {
        prepareQualityList($("#ProductID").val());
        calculateCost();
    });

    $("#Price").change(function () {
        calculateCost();
    });

    $("#Amount").change(function () {
        calculateCost();
    });


    prepareQualityList($("#ProductID").val());

});

function prepareQualityList(productID)
{
    var $list = $("select#Quality");
    $list.html("");

    $.each(ProductQualityList[productID], function () {
        $list.append($("<option />").val(this).text("Quality " + this));
    });
}

function calculateCost() {
    var price = $("#Price").val();
    var amount = $("#Amount").val();
    var productID = $("#ProductID").val();
    var countryID = $("#CountryID").val();

    if ($.isNumeric(price) && $.isNumeric(amount)) {
        var url = "/Company/GetProductCost";
        $.get(url, {
            companyID: CompanyID,
            price: price,
            amount: amount,
            productID: productID,
            countryID: countryID
        }, function (data) {
            $("#CalculateCost").html(data);
        });
    }
}