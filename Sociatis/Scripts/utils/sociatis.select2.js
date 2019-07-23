window.Common = window.Common || {};
window.Common.Select2 = Common.Select2 || {};

Common.Select2 = (function () {
    var args = {};
    var formatDataDefault = function (data) {
        if (data.loading)
            return "Loading";

        return data.text;
    };

    var formatDataSelectionDefault = function (data) {
        return data.text;
    };

    var dataDefault = function (params) {

        var data = {
            Query: params.term, // search term
            PageSize: this.pageSize,
            PageNumber: params.page
        };

        for (var arg of args.additionalData)
            data[arg.key] = arg.value;

        return data;
    };

    var processArgs = function ($el) {
        return {
            delay: Number($el.data("select2-delay")),
            quietMillis: Number($el.data("select2-quietmillis")),
            cache: JSON.parse($el.data("select2-cache").toLowerCase()), //can parse literals and values
            pageSize: Number($el.data("select2-pagesize")),
            minimumInputLength: Number($el.data("select2-minimuminputlength")),
            templateResult: $el.data("select2-templateresult"),
            templateSelection: $el.data("select2-templateselection"),
            url: $el.data("select2-url"),
            data: Sociatis.GetFunctionFromString($el.data("select2-data")),
            containerCssClass: $el.data("select2-containercssclass"),
            dropdownCssClass: $el.data("select2-dropdowncssclass"),
            onChange: $el.data("select2-onchange"),
            additionalData: processAdditionalProperties($el)
        }
    }

    var processAdditionalProperties = function ($el) {
        var additional = [];
        var datas = $el.data();
        for (var property in datas) {
            if (datas.hasOwnProperty(property)) {
                var data = datas[property];
                if (property.startsWith("select2Add")) {
                    var match = property.match("select2Add([^^]+)");
                    if (match.length > 1) {
                        additional.push({
                            key: match[1],
                            value: data
                        });
                    }
                }
            }
        }

        return additional;
    }

    var initialize = function ($el) {
        if ($el.data("select2ajaxviewmodel")) {

            args = processArgs($el);
               
            $el.select2({
                ajax: {
                    url: args.url,
                    dataType: 'json',
                    delay: args.delay,
                    quietMillis: args.quietMillis,
                    data: (params) => {
                            

                        return args.data(params);
                    },
                    processResults: function (response, params) {
                        return {
                            results: response.Items,
                            pagination: {
                                more: response.HasMorePages
                            }
                        };
                    },
                    cache: args.cache
                },
                escapeMarkup: function (markup) { return markup; }, // It let us use HTML markup
                minimumInputLength: args.minimumInputLength,
                templateResult: Sociatis.GetFunctionFromString(args.templateResult),
                templateSelection: Sociatis.GetFunctionFromString(args.templateSelection),
                placeholder: "-- Select --",
                containerCssClass: args.containerCssClass,
                dropdownCssClass: args.dropdownCssClass,
                allowClear: true
            });
        }
    }

    $(() => {
        var $selects = $("[data-Select2AjaxViewModel]");
        $.each($selects, (index, element) => {
            initialize($(element));
        });
    });

    $(document).on("change", "[data-Select2AjaxViewModel]", function () {
        var onChange = $(this).data("select2-onchange");
        if (onChange)
            Sociatis.GetFunctionFromString(onChange)(this);

        return true;
    });

    return {
        initialize: initialize,
        formatDataDefault: formatDataDefault,
        formatDataSelectionDefault: formatDataSelectionDefault,
        dataDefault: dataDefault,
        initialize
    };

})();
