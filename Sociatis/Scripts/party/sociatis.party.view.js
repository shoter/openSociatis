$(() => {


});

function inviteCitizen(select, partyID) {
    var $select = $(select);
    var partyID = $select.data("select2-add-partyid");
    var citizenID = $select.val();

    if (citizenID && partyID) {
        var url = Sociatis.GetAdress("InviteAjax", "Party", [{ Name: "partyID", Value: partyID }, { Name: "citizenID", Value: citizenID }]);
        Sociatis.AjaxAction(url, "POST", json => {
            Sociatis.Popups.AddMessage(json.Message, Sociatis.Popups.MessageClasses.Success, 10000);
            
        });
    }
}