window.Sociatis = window.Sociatis || {};
Sociatis.Shoutbox = Sociatis.Shoutbox || {};

$(() => {
    Sociatis.Shoutbox.MessageTemplate = $("#shoutboxMessageTemplate");
    Sociatis.Shoutbox.ChannelTemplate = $("#shoutboxChannelTemplate");
    Sociatis.Shoutbox.SubscribeTemplate = $("#shoutboxSubscribeTemplate");

    Sociatis.Shoutbox.Channels = new SimpleBar(document.getElementById('shoutboxChannels'), {
    })
    Sociatis.Shoutbox.Messages = new SimpleBar(document.getElementById('shoutboxContent'), {

    })


    Sociatis.Shoutbox.Messages.getScrollElement().addEventListener('scroll', (event) => {
        var container = event.target;
        var percentage = 100 * container.scrollTop / (container.scrollHeight - container.clientHeight);

        if (percentage > 85 && Sociatis.Shoutbox.CanDownloadNext) {
            Sociatis.Shoutbox.CanDownloadNext = false;
            var msgCount = $(Sociatis.Shoutbox.Messages.getContentElement()).children().length;
            setTimeout(() => {
                Sociatis.Shoutbox.DownloadMessages(Sociatis.Shoutbox.ActualChannelID, Sociatis.Shoutbox.LastMessageID, true);
            }, msgCount * 20);
        }
    });

    $('#shoutboxNewMessageText').keydown(function (e) {
        if (e.keyCode === 13 && e.shiftKey === false) {
            var val = $(this).val();

            var data =
                {
                    channelID: Sociatis.Shoutbox.ActualChannelID,
                    parentID: null,
                    content: val
                };

            var url = Sociatis.GetAdress("WriteMessage", "Shoutbox");

            Sociatis.AjaxBegin();
            $.postJSON(url, data, data => Sociatis.HandleJson(data, Sociatis.Shoutbox.OnPostMessage));

            $(this).val('');
            return false;
        }
    })
    Sociatis.Shoutbox.ActualChannelID = null;
    Sociatis.Shoutbox.Subscribed = store.get("SubscribedChannels");
    if (!Sociatis.Shoutbox.Subscribed || typeof (Sociatis.Shoutbox.Subscribed) !== "object") {
        Sociatis.Shoutbox.Subscribed = [];

        store.set("SubscribedChannels", Sociatis.Shoutbox.Subscribed);
    }



    Sociatis.Shoutbox.GetChannels(function (data) {
        var channels = data.Data;

        var i, j;
        for (i = 0; i < Sociatis.Shoutbox.Subscribed.length;) {
            var info = Sociatis.Shoutbox.Subscribed[i];
            var found = false;

            for (j = 0; j < channels.length; ++j) {
                var channel = channels[i];
                if (channel.ChannelID == info.ChannelID) {
                    found = true;
                    break;
                }
            }


            if (found == false) {
                Sociatis.Shoutbox.Unsubscribe(info.ChannelID, info.ChannelName);
            }
            else
                ++i;
        }

    });

    for (let i = 0; i < Sociatis.Shoutbox.Subscribed.length; ++i) {
        var info = Sociatis.Shoutbox.Subscribed[i];
        var model =
            {
                closeAble: true,
                channelID: info.ChannelID,
                channelName: info.ChannelName
            }

        Sociatis.Shoutbox.AddChannel(model);
    }
});

Sociatis.Shoutbox.OpenChannels = function () {
    Sociatis.Shoutbox.GetChannels(Sociatis.Shoutbox.LoadChannels);
    $(".shoutboxChannel").removeClass("active");
    $("#shoutboxChannelList").addClass("active");
    Sociatis.Shoutbox.ActualChannelID = null;
}

Sociatis.Shoutbox.GetChannels = function (callback) {
    var url = Sociatis.GetAdress("GetChannels", "Shoutbox");
    $.postJSON(url, null, (data) => Sociatis.HandleJson(data, callback));
}

Sociatis.Shoutbox.LoadChannels = function (data) {
    var channels = data.Data;
    Sociatis.Shoutbox.ClearMessages();

    channels.forEach((channel) => {
        var model =
            {
                subscribed: Sociatis.Shoutbox.IsSubscribed(channel.ChannelID),
                channelName: channel.ChannelName,
                channelID: channel.ChannelID
            }
        Sociatis.Shoutbox.AddSubscribe(model);
    });

    $("#shoutboxNewMessage").addClass("hide");
}

Sociatis.Shoutbox.OpenChannel = function (channelID) {
    var $channel = $(".shoutboxChannel[data-channelid='" + channelID + "']");
    if ($channel.length > 0) {
        $(".shoutboxChannel").removeClass("active");
        $channel.addClass("active");
        $("#shoutboxNewMessage").removeClass("hide");
        Sociatis.Shoutbox.ClearMessages();
        Sociatis.Shoutbox.ActualChannelID = channelID;
        Sociatis.Shoutbox.LastMessageID = null;
        Sociatis.Shoutbox.DownloadMessages(Sociatis.Shoutbox.ActualChannelID, Sociatis.Shoutbox.LastMessageID);
    }

}

Sociatis.Shoutbox.IsSubscribed = function (channelID) {
    for (let i = 0; i < Sociatis.Shoutbox.Subscribed.length; ++i) {
        if (Sociatis.Shoutbox.Subscribed[i].ChannelID === channelID)
            return true;
    }
    return false;
}

function closeChannel(channelID, event) {
    event.stopImmediatePropagation();
}

Sociatis.Shoutbox.AddChannel = function (model) {
    var html = Sociatis.Shoutbox.ChannelTemplate.render(model);
    $(html).insertBefore("#shoutboxChannelList");
    Sociatis.Clean($("#shoutboxChannels")[0]);
}

Sociatis.Shoutbox.RemoveChannel = function (channelID) {
    var $channel = $(".shoutboxChannel[data-channelid='" + channelID + "']");
    $channel.remove();
}

Sociatis.Shoutbox.OnPostMessage = function (data) {
    Sociatis.Shoutbox.AddMessage(Sociatis.Shoutbox.TransformServerMessageIntoModel(data.Data));
    Sociatis.AjaxEnd();
}

Sociatis.Shoutbox.DownloadMessages = function (channelID, lastMessageID /*can be null*/, withoutAjaxBegin) {
    if (!withoutAjaxBegin)
        Sociatis.AjaxBegin();
    var data = {
        channelID: Sociatis.Shoutbox.ActualChannelID,
        lastMessageID: lastMessageID,
        pageSize: 10
    };

    var url = Sociatis.GetAdress("GetMessages", "Shoutbox");

    $.postJSON(url, data, data => Sociatis.HandleJson(data, Sociatis.Shoutbox.AddDownloadedMessages));
}

Sociatis.Shoutbox.AddDownloadedMessages = function (data) {


    for (let i = 0; i < data.Messages.length; ++i) {
        let msg = data.Messages[i];
        Sociatis.Shoutbox.AddMessage(Sociatis.Shoutbox.TransformServerMessageIntoModel(msg), true);
        Sociatis.Shoutbox.LastMessageID = msg.MessageID;

    }
    Sociatis.Shoutbox.CanDownloadNext = true;
    Sociatis.AjaxEnd();
}

Sociatis.Shoutbox.TransformServerMessageIntoModel = function (msg) {
    var date = new Date(msg.TimeUTCMilliseconds);

    var hour = ((date.getHours() < 10 ? '0' : '') + date.getHours());
    var minutes = ((date.getMinutes() < 10 ? '0' : '') + date.getMinutes());

    if (!msg.AuthorImageURL)
        msg.AuthorImageURL = "/Content/Images/placeholder.png";
    return {
        imgURL: msg.AuthorImageURL,
        authorID: msg.AuthorID,
        authorName: msg.AuthorName,
        time: hour + ":" + minutes,
        day: "day " + msg.Day,
        message: msg.Message
    };
}


Sociatis.Shoutbox.AddMessage = function (model, append) {
    var html = Sociatis.Shoutbox.MessageTemplate.render(model);
    if (append)
        $(Sociatis.Shoutbox.Messages.getContentElement()).append(html);
    else
        $(Sociatis.Shoutbox.Messages.getContentElement()).prepend(html);

}

Sociatis.Shoutbox.AddSubscribe = function (model) {
    var html = Sociatis.Shoutbox.SubscribeTemplate.render(model);
    $(Sociatis.Shoutbox.Messages.getContentElement()).append(html);
}

Sociatis.Shoutbox.ClearMessages = function () {
    $(Sociatis.Shoutbox.Messages.getContentElement()).empty();
}

Sociatis.Shoutbox.Unsubscribe = function (channelID, channelName) {
    var model =
        {
            subscribed: false,
            channelName: channelName,
            channelID: channelID
        }

    var $actual = $(".shoutboxSubscribe[data-channelid='" + channelID + "']");

    $actual.replaceWith(Sociatis.Shoutbox.SubscribeTemplate.render(model));

    Sociatis.Shoutbox.RemoveChannel(channelID);

    for (let i = 0; i < Sociatis.Shoutbox.Subscribed.length; ++i) {
        console.log("Checking " + i);
        if (Sociatis.Shoutbox.Subscribed[i].ChannelID === channelID) {
            console.log("Deleting " + i);
            Sociatis.Shoutbox.Subscribed.splice(i, 1);
            --i;
        }
    }
    store.set("SubscribedChannels", Sociatis.Shoutbox.Subscribed);
}

Sociatis.Shoutbox.Subscribe = function (channelID, channelName) {

    if (Sociatis.Shoutbox.IsSubscribed(channelID))
        return;

    var model =
        {
            subscribed: true,
            channelName: channelName,
            channelID: channelID
        }

    var $actual = $(".shoutboxSubscribe[data-channelid='" + channelID + "']");

    $actual.replaceWith(Sociatis.Shoutbox.SubscribeTemplate.render(model));

    var channelModel =
        {
            channelID: channelID,
            channelName: channelName,
            closeAble: true
        };

    Sociatis.Shoutbox.AddChannel(channelModel);
    Sociatis.Shoutbox.Subscribed.push({ ChannelID: channelID, ChannelName: channelName });
    store.set("SubscribedChannels", Sociatis.Shoutbox.Subscribed);
}