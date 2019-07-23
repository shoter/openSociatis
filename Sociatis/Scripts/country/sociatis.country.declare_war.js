$(function () {
    $("#SelectedCountryID").change(function () {
        var defenderID = $(this).val();
        if (defenderID) {
            var url = "/Country/GetWarDeclareInformation";
            Sociatis.AjaxBegin("Loading");
            $.post(url, {
                attackerID: CountryID,
                defenderID: defenderID
            }, function (data) {
                $("#DeclareInformations").html(data);
                Sociatis.AjaxEnd();
            });
        }

    });

});