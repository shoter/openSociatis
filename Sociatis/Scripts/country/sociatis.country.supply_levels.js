$(function () {

    $("[name$='CurrentSupplyProgramType']").change(function () {
        var supplyProgramTypeID = this.value;

        calcNewSupplyLevel();
        setNewTomorrowSupplyLevel($(this).parents("tr"), supplyProgramTypeID);
        $("#submitButton").removeAttr("disabled");
    });
});

function calcNewSupplyLevel() {
    var sum = 0.0;
    $("[name$='CurrentSupplyProgramType']").each(function () {
        var $this = $(this);
        sum += setNewSupplyLevel($this);

        
    });

    sum = Math.floor(sum * 100) / 100;

    $("#upkeepSum").text(sum);
}

function setNewSupplyLevel($ddl) {
    var val = $ddl.val();


    $ddl.parent().parent().find("[name='currentCost']").text(supplyProgramCost[val]);

    return supplyProgramCost[val];
}

function setNewTomorrowSupplyLevel($tr, supplyProgramTypeID)
{
    var url = "/Country/CalculateNextSupplyLevel";
    var regionID = $tr.attr("data-regionID");
    Sociatis.AjaxBegin();
    $.post(url, { regionID: regionID, supplyProgramTypeID: supplyProgramTypeID }, function (data) {
        $tr.find("[name='tomorrowSupplyLevel']").html(data.Data);
        Sociatis.AjaxEnd();
    });
}