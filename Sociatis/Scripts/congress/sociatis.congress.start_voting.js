$(function () {
    $("#votingType").change(function () {
        var value = $(this).val();
        var countryID = $("#CountryID").val();
        var url = Sociatis.GetAdress("EditOptionForm", "Congress", [{ Name: "votingType", Value: value }]);

        Sociatis.AjaxBegin();

        $.get(url, { countryID: countryID }, changeOptions);

    });

    $("#votingType").select2({
        dropdownAutoWidth: true
    });

    $("#commentRestriction").select2({
        minimumResultsForSearch: Infinity,
        dropdownAutoWidth: true
    });
});

function changeOptions(data)
{
    $("#options").html(data);
    $("#submitButton").removeClass("disabled");
    Sociatis.AjaxEnd();
}

