$("#RegionID").select2({
    minimumResultsForSearch: 5,
});
$("#ProductID").select2({

});

$("#ProductID").on("change",  function (e) {
    if (this.value == 13 /*medicalSupplies*/) {
        $("#CompanyName").hide();
    }
    else
        $("#CompanyName").show();
});

