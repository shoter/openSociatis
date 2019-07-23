$(document).ready(function () {

    function fill() {     
        var countryID = $("#Register_CountryID").val();
        if (countryID) {
            Sociatis.AjaxBegin();
            var url = "/Account/GetStartingRegions";
            $.get(url, { countryID: countryID }, function (data) {
                $("#Register_RegionID").empty();
                $("#Register_RegionID").append(data);
                Sociatis.AjaxEnd();
            });
        }
    }

    $("#Register_CountryID").change(function () {

        fill();
    });

    $("#login").keyup(function (event) {
        if (event.keyCode === 13) {
            $("#login form").submit();
        }
    });

    fill();

});