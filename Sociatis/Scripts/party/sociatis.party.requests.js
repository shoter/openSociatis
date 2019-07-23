$(() => {

});


function acceptJoinRequest(requestID) {
    var url = Sociatis.GetAdress("AcceptJoinRequest", "Party", [{ Name: "requestID", Value: requestID }]);
    Sociatis.AjaxAction(url, "POST", json => {
        Sociatis.Popups.AddMessage(json.Message, Sociatis.Popups.MessageClasses.Success);
        removeHtmlFromRequest(requestID);
    });
}
function declineJoinRequest(requestID) {
    var url = Sociatis.GetAdress("DeclineJoinRequest", "Party", [{ Name: "requestID", Value: requestID }]);
    Sociatis.AjaxAction(url, "POST", json => {
        Sociatis.Popups.AddMessage(json.Message, Sociatis.Popups.MessageClasses.Success);
        removeHtmlFromRequest(requestID);
    });
}

function removeHtmlFromRequest(requestID) {
    $(".partyRequest[data-requestid='" + requestID + "']").fadeOut(700, "linear", function () {
        $(this).css({ "visibility": "hidden", display: 'block' }).slideUp();
    });
}