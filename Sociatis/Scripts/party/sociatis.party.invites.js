$(() => {

});

function acceptInvite(button) {
    var inviteID = $(button).data("inviteid");
    var url = Sociatis.GetAdress("AcceptInvite", "Party", [{ Name: "inviteID", Value: inviteID }]);
    Sociatis.AjaxAction(url, "POST");
}

function declineInvite(button) {
    var inviteID = $(button).data("inviteid");
    var url = Sociatis.GetAdress("DeclineInvite", "Party", [{ Name: "inviteID", Value: inviteID }]);
    Sociatis.AjaxAction(url, "POST", json => {
        Sociatis.Popups.AddMessage(json.Message, Sociatis.Popups.MessageClasses.Success);

        $(button).parents(".partyInvite").fadeOut(700, "linear", function () {
            $(this).css({ "visibility": "hidden", display: 'block' }).slideUp();
        });

    });
}

function removeInvite(citizenID, partyID) {
    var url = Sociatis.GetAdress("RemoveInvite", "Party", [{ Name: "citizenID", Value: citizenID }, { Name: "partyID", Value: partyID }]);
    Sociatis.AjaxAction(url, "POST", json => {
        Sociatis.Popups.AddMessage(json.Message, Sociatis.Popups.MessageClasses.Success);

        $("div[data-citizenid='" + citizenID + "']").fadeOut(700, "linear", function () {
            $(this).css({ "visibility": "hidden", display: 'block' }).slideUp();
        });
    });
}