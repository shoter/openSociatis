window.Sociatis = window.Sociatis || {};
Sociatis.Company = Sociatis.Company || {};

$(() => {
    $("input[data-managerid]").change(function () {
        var $this = $(this);
        var managerID = $this.data("managerid");
        Sociatis.Company.EnableSave(managerID);
        
    });
});

Sociatis.Company.EnableSave = function(managerID)
{
    var $saveButton = $("button[data-managerid='" + managerID + "']");
    $saveButton.removeAttr("disabled");
    $saveButton.removeClass("disabled");
}

Sociatis.Company.DisableSave = function (managerID) {
    var $saveButton = $("button[data-managerid='" + managerID + "']");
    $saveButton.attr("disabled", "");
    $saveButton.addClass("disabled");
}
Sociatis.Company.ChangeManager = function (managerID) {
    Sociatis.AjaxBegin();
    var $inputs = $("input[data-managerid='" + managerID + "']");
    var data =
        {
            managerID: managerID,
            priority: $inputs.closest("[data-priority]").val(),
            rights: []
        };

    var $rights = $inputs.closest("[data-rights]:checked");

    $.each($rights, (i, right) => {
        var $right = $(right);
        data.rights.push($right.val());
    });

    var url = Sociatis.GetAdress("ChangeManager", "Company");

    $.postJSON(url, data, data => Sociatis.HandleJson(data, Sociatis.Company.PostChangeManager, managerID));

    console.log(data);

}

Sociatis.Company.PostChangeManager = function (data, managerID) {
    $.notify(data.Message, "success");
    Sociatis.Company.DisableSave(managerID);
    Sociatis.AjaxEnd();
}