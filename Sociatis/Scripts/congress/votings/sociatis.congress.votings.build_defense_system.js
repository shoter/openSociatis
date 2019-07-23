$(function () {
    var $goldCost = $("#goldCostForDS");
    var $quality = $("#Quality");
    var $cpCost = $("#constructionPointsCostForDS");

    function updateGoldCost() {
       
        var quality = $quality.val();
        var goldCost = Sociatis.Global.DSGoldCost[quality - 1];
        $goldCost.text(goldCost + " gold");
        $cpCost.text(Sociatis.Global.DSCPCost[quality - 1]);
    };

    $quality.change(updateGoldCost);
    updateGoldCost();

});