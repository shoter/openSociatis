$(function () {
    $("#CountryEmbargoID").change(function () {
        var value = $(this).val();
        if (value != "null")
            $(this).parents("form").submit();
        else
            $("#embargoInfo").html("");
    });
});