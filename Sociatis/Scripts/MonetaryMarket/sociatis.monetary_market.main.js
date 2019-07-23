$(function () {
    $("#BuyCurrencyID").change(function () {
        $("#SellCurrencyID option").show();
        hideSameCurrency();
        loadFlot();
        calculateOffer();
        

    });

    $("#Rate").change(function () {
        calculateOffer();
    });

    $("#Amount").change(function () {
        calculateOffer();
    });



    $("[name='OfferType']").click(function () {
        changeBuyCurrencyAddon();
        calculateOffer();
    });

    $("#SellCurrencyID").change(function () {
        loadFlot();
        calculateOffer();
    });


    $("<div id='flotTooltip'></div>").css({
        position: "absolute",
        display: "none",
        border: "1px solid #fdd",
        padding: "2px",
        "background-color": "#fee",
        opacity: 0.80
    }).appendTo("body");

    hideSameCurrency();
    loadFlot();

});

function loadFlot()
{
    var buyCurrencyID = $("#BuyCurrencyID").val();
    var sellCurrencyID = $("#SellCurrencyID").val();

    if (!buyCurrencyID || !sellCurrencyID)
        return;

    var url = "/MonetaryMarket/GetData";

    Sociatis.AjaxBegin("Loading");
    $.post(url, { buyCurrencyID: buyCurrencyID, sellCurrencyID: sellCurrencyID }, function (data) {

        Sociatis.HandleJson(data, GetData)
        Sociatis.AjaxEnd();
    });
}


function GetData(data)
{
    BuyCurrencySymbol = data.BuySymbol;
    SellCurrencySymbol = data.SellSymbol;

    $("#sellCurrencyAddon").html(SellCurrencySymbol);
    changeBuyCurrencyAddon();

    PreparePlot(data.Plot);

    $("#myOfferTBody").html("");

    var myTemplate = $.templates("#offerTemplate");

    var html = myTemplate.render({ offers: data.My });

    $("#myOfferTBody").html(html);

    if (data.Best.BuyPrice != undefined)
        $("#buyOffer").html(data.Best.BuyPrice);

    if (data.Best.BuyVolume != undefined)
        $("#buyVolume").html(data.Best.BuyVolume);

    if (data.Best.SellPrice != undefined)
        $("#sellOffer").html(data.Best.SellPrice);

    if (data.Best.SellVolume != undefined)
        $("#sellVolume").html(data.Best.SellVolume);

  
    
}

function xAxisLabelGenerator(x) {
    if (FlotData[x] == undefined)
        return "";
    return FlotData[x].Time;
}

function PreparePlot(data)
{
    if (data.length === 0)
    {
        var html = $("<div class='text-center'>No data available</div>")
        $("#MonetaryMarketChart").html(html);
        return;
    }
    var parsed = [];
    var i = 0;
    $.each(data, function (index, element) {
        parsed.push([i++, element.Value]);
    });

    FlotData = data;

    $.plot("#MonetaryMarketChart", [parsed],
        {
            series: {
                lines: {
                    show: true
                },
                points: {
                    show: false
                }
            },
            grid: {
                hoverable: true,
                clickable: false,
                mouseActiveRadius: 100
            },
            xaxis: {
                tickFormatter: xAxisLabelGenerator
            }
        });

    $("#MonetaryMarketChart").bind("plothover", function (event, pos, item) {
        if (item) {
            var id = item.datapoint[0];
                rate = item.datapoint[1].toFixed(2);

            $("#flotTooltip").html("Day " + FlotData[id].Day +" " + FlotData[id].Time + "<br/>Rate: " + rate + " " + SellCurrencySymbol)
                .css({ top: item.pageY + 5, left: item.pageX + 5 })
                .fadeIn(200);
        } else { 
            $("#flotTooltip").hide();
        }
    });

    
    
}



function changeBuyCurrencyAddon()
{
    $("#buyCurrencyAddon").html(BuyCurrencySymbol);
}

function hideSameCurrency()
{
    var buyID = $("#BuyCurrencyID").val();

    $("#SellCurrencyID option[value='" + buyID + "']").hide();

    var actuallSellID = $("#SellCurrencyID option:checked").val();

    if(buyID == actuallSellID)
        $($("#SellCurrencyID option").not("[value='" + buyID + "']")[0]).prop("selected", true);
}

function calculateOffer()
{
    var buyCurrencyID = $("#BuyCurrencyID").val();
    var sellCurrencyID = $("#SellCurrencyID").val();
    var amount = $("#Amount").val();
    var rate = $("#Rate").val();
    var offerTypeID = $("[name='OfferType']:checked").val();

    var buyCurrency = $('select#BuyCurrencyID option:selected').text();
    var sellCurrency = $('select#SellCurrencyID option:selected').text();

    if (!rate)
        return;

    if (!amount)
        return;

    if (!offerTypeID)
        return;

    if (!buyCurrencyID)
        return;

    if (!sellCurrencyID)
        return;

    var url = "/MonetaryMarket/CalculateOffer";

    $.post(url, { buyCurrencyID: buyCurrencyID, sellCurrencyID: sellCurrencyID, amount: amount, rate: rate, offerTypeID: offerTypeID }, function (data) {
        Sociatis.HandleJson(data, postCalculatedOffer, { sellCurrency: sellCurrency, buyCurrency, buyCurrency, rate: rate, amount: amount});
    });

}

function postCalculatedOffer(data, args)
{
    data = data.Data;
    var msg = "";
    if (data.offerTypeID == 1/*buy*/) {
        msg = "You will buy " + args.amount + " " + args.buyCurrency + " with rate of " + args.rate + " " + args.sellCurrency + " per 1 " + args.buyCurrency + ".";
        msg += "Overally you will get " + args.amount + " " + args.buyCurrency + " for maximum " + args.rate * args.amount + " " + args.sellCurrency + ".";
    }
    else {
        msg = "You will sell " + args.amount + " " + args.buyCurrency + " with rate of " + args.rate + " " + args.sellCurrency + " per 1 " + args.buyCurrency + ".";
        msg += "Overally you will sell " + args.amount + " " + args.buyCurrency + " with income of " + args.rate * args.amount + " " + args.sellCurrency + ".";
    }

    $("#mmHint.hint").text(msg);

    $("#offerCost").html(data.offerCost);
    $("#taxCost").html(data.taxCost);
    $("#sum").html(data.sum);
}