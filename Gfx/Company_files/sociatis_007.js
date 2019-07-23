window.Sociatis = window.Sociatis || {};
Sociatis.Global = Sociatis.Global || {};
Sociatis.SubmitByLink = function (/*<a>*/ link) {
    $(link).parents("form").submit();
};

Sociatis.ShowMessage = function (jsonMessage) {
    alert(jsonMessage.Message);
};

Sociatis.AjaxBegin = function (message /*<h1>Loading...</h1>*/) {
    if (message === undefined)
        message = "<h1>Loading</h1>";
    Sociatis.UI.BlockUI({ message: message });
};

Sociatis.AjaxEnd = function () {
    Sociatis.RemoveAutoHref();
    Sociatis.UI.UnblockUI();
};

Sociatis.AjaxError = function (xhr) {
    if (xhr.readyState == 0 || xhr.status == 0)
        return;  // it's not really an error

    var responseText = xhr.responseText;
    $.notify(responseText, "error");
    Sociatis.UI.UnblockUI();
};

Sociatis.AjaxErrorGeneric = function (error) {
    if (Sociatis.Refreshing === true)
        return;
    console.log(error);
    $.notify(error, "error");
    Sociatis.UI.UnblockUI();
};


Sociatis.RemoveAutoHref = function () {
    $('a[href$="#!"]').each(function (index, element) {
        element.removeAttribute("href");
    });
};


Sociatis.Clean = function(node) {
    for (var n = 0; n < node.childNodes.length; n++) {
        var child = node.childNodes[n];
        if
    (
            child.nodeType === 8
            ||
            (child.nodeType === 3 && !/\S/.test(child.nodeValue))
        ) {
            node.removeChild(child);
            n--;
        }
        else if (child.nodeType === 1) {
            Sociatis.Clean(child);
        }
    }
}

Sociatis.AjaxAction = function(url, httpMethod, callback)
{
    if (Sociatis.Refreshing === true)
        return;
    Sociatis.AjaxBegin();
    $.ajax({
        type: httpMethod,
        url: url,
        success: data => {
            Sociatis.HandleJson(data, callback);
            Sociatis.AjaxEnd();
        },
        error: data => {
            Sociatis.AjaxError(data);
        }

    });
}

Sociatis.ShowAjaxSuccess = function (data) {
    $.notify(data.Message, "success");
    Sociatis.AjaxEnd();
}


//parameters is array. parameter structure. Name/Value field
Sociatis.GetAdress = function (ActionName, ControllerName, parameters) {
    var url = window.location.href;
    //deletes everything after localhost/server url whatever
    //http://localhost:25388/Customer/asd/1 => http://localhost:25388
    var regex = /\/([\w?=\-?]+\/?)+$/;
    url = url.replace(regex, "");

    url += "/" + ControllerName;
    url += "/" + ActionName;
    if (parameters)
        for (var i = 0; i < parameters.length; ++i) {
            if (i === 0)
                url += "?";
            else
                url += "&";

            url += parameters[i].Name + "=" + parameters[i].Value;
        }
    return url;
};

Sociatis.Refreshing = false;
window.onbeforeunload = function () {
    Sociatis.Refreshing = true;
    console.log("Refresh!");
}

Sociatis.PagingTo = function (pageNumber, inputField) {
    $("[name='PagingParam.PageNumber']").val(pageNumber);
    var $form = $(inputField).parents("form");
    $form.submit();
};

Sociatis.GetStringLength = function (str) {
    if (!str)
        return 0;
    if (str.indexOf("\r") >= 0)
        return str.length;
    return str.replaceAll("\n", "\r\n").length;
}

String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};

//Function handles error passed by json. If no error occurs then function_execute is called. If there is an error then proper message is displayed.
///@JsonModel : mandatory. JsonModelBase returned by controller
///@function_execute : optional. Function which will be executed and JsonModel will be passed as 1st parameter. [ function_execute(jsonModel) ]
///@parameters : optional. Will be passed to function_execute. [ function_execute(JsonModel, parameters) ]. function will be called only once.
Sociatis.HandleJson = function (jsonModel, function_execute, parameters, function_execute_error) {

    if (jsonModel.Status === Sociatis.Enums.JsonStatus.Error) {
        $.notify(jsonModel.ErrorMessage, "error");
        if (function_execute_error)
            function_execute_error();
        Sociatis.UI.UnblockUI();
    }
    else if(jsonModel.Status === Sociatis.Enums.JsonStatus.Redirect)
    {
        window.location.href = jsonModel.Url;
        Sociatis.UI.BlockUI({}, 100);
    }
    else if (function_execute !== undefined) {
        if (parameters !== undefined)
            function_execute(jsonModel, parameters);
        else
            function_execute(jsonModel);
    }

};

Sociatis.GetFunctionFromString = function (string) {
    var scope = window;
    var scopeSplit = string.split('.');
    for (i = 0; i < scopeSplit.length - 1; i++) {
        scope = scope[scopeSplit[i]];

        if (!scope) return;
    }

    return scope[scopeSplit[scopeSplit.length - 1]];
}


///Execute without parameters
Sociatis.ReloadSummary = function () {
    var url = "/Entity/EntitySummary";

    Sociatis.AjaxBegin();
    $.post(url, function (data) {
        $(".entitySummary").html(data);
        Sociatis.AjaxEnd();
    });
};
/**
 * A linear interpolator for hexadecimal colors
 * @param {String} a
 * @param {String} b
 * @param {Number} amount
 * @example
 * // returns #7F7F7F
 * lerpColor('#000000', '#ffffff', 0.5)
 * @returns {String}
 */
Sociatis.Lerp = function(a, b, amount) {

    var ah = parseInt(a.replace(/#/g, ''), 16),
        ar = ah >> 16, ag = ah >> 8 & 0xff, ab = ah & 0xff,
        bh = parseInt(b.replace(/#/g, ''), 16),
        br = bh >> 16, bg = bh >> 8 & 0xff, bb = bh & 0xff,
        rr = ar + amount * (br - ar),
        rg = ag + amount * (bg - ag),
        rb = ab + amount * (bb - ab);

    return '#' + ((1 << 24) + (rr << 16) + (rg << 8) + rb | 0).toString(16).slice(1);
}


function stickFooter() {
    return;
    var footer = $("#footer");
    var pos = footer.position();
    var height = $(window).height();
    var docHeight = $(document).height(); // returns height of HTML document
    height = height - pos.top;
    height = height - footer.height();
    if (docHeight > height)
    {
        footer.css({
            'margin-top': 0 + 'px'
        });
    }
    if (height > 0) {
        footer.css({
            'margin-top': height + 'px'
        });
    }
}


$(document).ready(function () {

    $.ajaxSetup({
        "error": function (xhr) {
            if (!(xhr.readyState == 0 || xhr.status == 0))
                $.notify("error", "error");
        }
    });

});

Sociatis.RemoveAutoHref();

function changePage(pageNumber) {
    $("#PageNumber").val(pageNumber);
    document.forms[0].submit();
}

String.prototype.endsWith = function (suffix) {
    return this.indexOf(suffix, this.length - suffix.length) !== -1;
};

// jQuery plugin to prevent double submission of forms
jQuery.fn.preventDoubleSubmission = function () {
    $(this).on('submit', function (e) {
        var $form = $(this);

        if ($form.data('submitted') === true) {
            // Previously submitted - don't submit again
            e.preventDefault();
        } else {
            // Mark it so that the next submit can be ignored
            $form.data('submitted', true);
        }
    });

    // Keep chainability
    return this;
};

function isNumber(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}

//use like this
// $('form').preventDoubleSubmission();