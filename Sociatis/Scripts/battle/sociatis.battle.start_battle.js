$(function () {
    $("#SelectedRegionID").change(function () {
        var regionID = $(this).val();

        if (regionID === "null") {
            $("#StartInformations").html("");
        }
        else {
            var url = "/War/GetBattleStartInformation";
            Sociatis.AjaxBegin("Loading");
            $.post(url, {
                warID: WarID,
                regionID: regionID
            }, function (data) {
                $("#StartInformations").html(data);
                Sociatis.AjaxEnd();
            });
        }

    });

});

function validateStartBattleForm()
{
    var value = $("#SelectedRegionID").val();

    return value !== "null";
}