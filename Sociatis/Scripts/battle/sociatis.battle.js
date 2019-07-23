$(function () {
    $("#preAttFightButton").click(function () {
        $("#isAttacker").val(true);
        openBattleDialog();
    });
    $("#preDefFightButton").click(function () {
        $("#isAttacker").val(false);
        openBattleDialog();
    });

    $("#realFightButton").click(function () {
        var weaponQuality = $("#weaponQuality").val();
        var battleID = $("#battleID").val();
        var isAttacker = $("#isAttacker").val();
        if (weaponQuality === "none")
            return;
        var url = "/Battle/Fight";

        Sociatis.AjaxBegin();
        $.post(url, { battleID: battleID, weaponQuality: weaponQuality, isAttacker: isAttacker }, function (data) {
            Sociatis.HandleJson(data, OnFightDone);
            Sociatis.AjaxEnd();
        });
    });

    $("#weaponQuality").change(function () {
        calculateDamage();

    });

});

function openBattleDialog()
{
    $("#weaponQuality").val("none");
    $("#battleDialog").dialog(
        {
            title: "Battle",
            resizable: false,
            dialogClass: "battleDialog",
            width: "auto"
        }
        );

    $("#battleDialog").dialog("open");
}

function calculateDamage()
{
    var weaponQuality = $("#weaponQuality").val();
    var battleID = $("#battleID").val();
    var isAttacker = $("#isAttacker").val();
    if (weaponQuality === "none")
        return;

    var url = "/Battle/DamageCalculation";

    Sociatis.AjaxBegin();
    $.post(url, { battleID: battleID, weaponQuality: weaponQuality, isAttacker : isAttacker }, function (data) {
        Sociatis.HandleJson(data, OnDamageCalculationDone);
        Sociatis.AjaxEnd();
    });
}

function OnDamageCalculationDone(jsonModel)
{
    var data = jsonModel.Data;
    $("#possibleDamage").text(data.damage + " dmg");
    $("#weaponBonus").text(data.weaponBonus + " dmg");
    $("#healthModifier").text(data.healthModifier + " %");
    $("#militaryRankModifier").text(data.militaryRankModifier + " %");
    $("#weaponStrengthModifier").text(data.weaponAndStrengthModifier + " %");
    $("#distanceModifier").text(data.distanceModifier + " %");
    $("#developmentModifier").text(data.developmentModifier + " %");
    $("#overallModifier").text(data.overallModifier + " %");
}

function OnFightDone(jsonModel) {
    alert(jsonModel.Message);
    calculateDamage();
    $("#citizenHPBar").width(jsonModel.HP + "%");
    $("#citizenHPText").text(jsonModel.HP + "% HP");
}