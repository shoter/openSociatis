$(function () {

    $("#CountryRegion").change(function () {
        var regionID = $(this).val();
        var countryID = $("#CountryID").val();

        if (regionID > 0) {
            var url = "/Congress/CongressCandidatesForRegion";
            $.get(url, { countryID: countryID, regionID: regionID }, function (data) {
                $("#CandidatesTable").empty();
                $("#CandidatesTable").append(data);
            });
        } else if (regionID == "ALL") {
            var url = "/Congress/CongressCandidatesForAllRegions";
            $.get(url, { countryID: countryID }, function (data) {
                $("#CandidatesTable").empty();
                $("#CandidatesTable").append(data);
            });
        }
    });
});