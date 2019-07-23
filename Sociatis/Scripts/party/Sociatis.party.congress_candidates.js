$(function() {

    $("#CountryRegion").change(function () {
        var regionID = $(this).val();
        var partyID = $("#PartyID").val();
        
        if (regionID > 0) {
            var url = "/Party/CongressCandidatesForRegion";
            $.get(url, { partyID: partyID, regionID: regionID }, function (data) {
                $("#CandidatesTable").empty();
                $("#CandidatesTable").append(data);
            });
        } else if (regionID == "ALL") {
            var url = "/Party/CongressCandidatesForAllRegions";
            $.get(url, { partyID: partyID}, function (data) {
                $("#CandidatesTable").empty();
                $("#CandidatesTable").append(data);
            });
        }
    });
});