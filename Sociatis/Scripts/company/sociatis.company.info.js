window.Sociatis = window.Sociatis || {};
Sociatis.Company = Sociatis.Company || {};

$(() => {
});

Sociatis.Company.Upgrade = function (companyID, goldCost, consCost, newQuality) {
    var msg = "Do you want to upgrade your compant to Q" + newQuality + ". It will cost " + goldCost + " gold and " + consCost + " construction points.";
    if (confirm(msg)) {
        var url = Sociatis.GetAdress("UpgradeCompany", "Company");
        var data = {
            companyID: companyID
        };
        $.postJSON(url, data, data => Sociatis.HandleJson(data, Sociatis.Company.PostUpgrade));
    }
}

Sociatis.Company.PostUpgrade = function (data) {
    $.notify(data.Message, "success");
    location.reload();
}