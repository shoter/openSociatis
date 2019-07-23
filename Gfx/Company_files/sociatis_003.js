window.Sociatis = window.Sociatis || {};

$(function () {
	$(document).on("click", ".popupMessage .close", function () {
        Sociatis.Popups.RemoveMessage(this);
	}); //On popoulMessage close


});



Sociatis.Popups = (function () {
    var template = $.templates("#popupMessageTemplate");

	return {
        AddMessage: function (message, messageClass, autoRemoveAfter) {
            var html = template.render({ message: message, messageClass: messageClass });
            var guid = Sociatis.Utils.GenerateGUID();
            var $html = $(html).data("guid", guid);
            $("#popupMessageList").append($html);

            if (autoRemoveAfter > 0) {
                setTimeout(() => Sociatis.Popups.RemoveMessageByGUID(guid), autoRemoveAfter);
            }
            
            return guid;
        },

        RemoveMessageByGUID: function (guid) {
            var messages = $(".popupMessage");
            $.each(messages, (i, message) => {
                if ($(message).data("guid") === guid) {
                    Sociatis.Popups.RemoveMessage(message);
                    return true;
                }
            });
            return false;
        },

        RemoveMessage: function (message) {
            $message = $(message);
            $parent = $message.parent();
            $parent.fadeOut(700, "linear", function () {
                $(this).css({ "visibility": "hidden", display: 'block' }).slideUp();
            }); //fade out function
        },

        MessageClasses: {
            Error: "error",
            Warning: "warning",
            Info: "info",
            Success: "success"
        }
	};

})();