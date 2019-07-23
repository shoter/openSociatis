$(() => {
    joinRequestMessageChange($("#joinRequestModal textarea"));
});

function openJoinRequest() {
    $("#joinRequestModal").dialog({
        title: "Join request",
        resizable: false
    });
}

function joinRequestMessageChange(textarea) {
    var text = $(textarea).val();
    var $remaining = $("#joinRequestRemainingChar");
    var remaining = $remaining.data("fieldsize") - Sociatis.GetStringLength(text);
    
    if (remaining < 0) {
        $remaining.addClass("text-error");
        $remaining.removeClass("text-warning");
    }
    else if (remaining < 5) {
        $remaining.removeClass("text-error");
        $remaining.addClass("text-warning");
    }
    else {
        $remaining.removeClass("text-warning");
        $remaining.removeClass("text-error");
    }
    $remaining.html(remaining);
}
