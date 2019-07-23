$(function () {
    $("#regionID").change(function () {
        PrepareTravelSummary();
    });

    $("#ticketQuality").change(function () {
        PrepareTravelSummary();
    });

    $("#countryID").change(function () {
        var countryID = this.value;
        if (countryID === "null")
            return;

        var url = "/Citizen/RegionsForTravel";

        Sociatis.AjaxBegin("Loading");
        $.post(url, { countryID: countryID }, function (data) {
            $("#regionID").html("");
            $.each(data, function (key, value) {
                var html = $("<option value='" + value.Value + "'>" + value.Text + "</option>");
                $("#regionID").append(html);
            });
            PrepareTravelSummary();
            Sociatis.AjaxEnd();
        });
    });

});

function disableTravelButton()
{
    $("#travelButton").attr("disabled", "");
}

function enableTravelButton()
{
    $("#travelButton").removeAttr("disabled");
}

function PrepareTravelSummary()
{
    var regionID = $("#regionID").val();
    var ticketQuality = $("#ticketQuality").val();

    if (!regionID)
        return;

    var url = "/Citizen/TravelSummary";
    
    Sociatis.AjaxBegin();
    $.post(url, { regionID: regionID, ticketQuality: ticketQuality }, function (data) {
        Sociatis.HandleJson(data, OnTravelSummaryReceived);
        Sociatis.AjaxEnd();
            });
}

function OnTravelSummaryReceived(model)
{
    if(model.CanTravel == true)
    {
        enableTravelButton();
    } else
    {
        disableTravelButton();
    }

    $("#TravelSummary").html(model.Content);
}