Sociatis.Hospital.NationalityOptions = (function () {
    var $newCountry = $("#newCountryID");
    var $newHealingPrice = $("#NewHealingPrice");
    var $newFreeHealing = $("#NewFreeHealing");
    var $newAddButton = $("#NewAddButton");
    var $table = $("#nationalityHealingOptions");
    var rowIndex = 0;
    var select2Options = {
        width: '100%'
    };

    var tryToAddNew = function () {
        if (validateNew()) {
            Sociatis.UI.BlockUI();
            createNewRow(rowIndex++);
            Sociatis.UI.UnblockUI();
        }
    };

    var validateNew = function () {
        var _ret = true;
        var countryID = $newCountry.val();
        if (!countryID) {
            _ret = false;
            $.notify("Country not selected!", "error");
        }

        var $existingSelects = $("select[name^='countryOptions'][name$='countryID']");
        $.each($existingSelects, function (i, element) {
            $element = $(element);
            if ($element.val() === countryID) {
                return _ret = error("You cannot select this country again!");
            }
        });

        var healingPrice = $newHealingPrice.val();
        var freeHealing = $newFreeHealing.is(":checked");
        if (isNumber(healingPrice) === false && freeHealing === false) {
            _ret = false;
            $.notify("Price not set!", "error");
        } else if (freeHealing === false && healingPrice < 0) {
            _ret = false;
            $.notify("Price cannot be negative!", "error");
        }

        


        return _ret;
    };

    var createNewRow = function (index) {
        var prefix = "countryOptions[" + index + "].";
        var row = $("tr#newNationalityOption").clone();

        var $row = $(row).insertBefore("tr#newNationalityOption");

        $row.removeAttr("id");

        $row.find("#newCountryID")
            .removeAttr("id")
            .attr("name", prefix + "countryID")
            .val($newCountry.val())

        $row.find("#NewFreeHealing")
            .removeAttr("id")
            .attr("name", prefix + "healingFree");

        $row.find("#NewHealingPrice")
            .removeAttr("id")
            .attr("name", prefix + "price");

        var $button = $("<button>")
            .attr("type", "button")
            .addClass("faButton")
            .attr("data-deleteButton", "true")
            .append($("<i>")
                .addClass("fa fa-trash")
                );

        $row.find("td:last-child")
            .html("")
            .append($button);
    }

    var onHealingFreeChange = function (event) {
        var $input = $(event.target);
        var value = $input.is(":checked");
        var $row = $input.parents("tr");

        if (value) {
            blockInput($row);
        }
        else {
            unblockInput($row);
        }
    };

    var get$Input = function ($row) {
        return $row.find("[name^='countryOptions'][name$='price']");
    };

    var blockInput = function ($row) {
        var $input = get$Input($row);
        $input.attr("disabled", "disabled");
        $input.val("");
    };

    var unblockInput = function ($row) {
        var $input = get$Input($row);
        $input.removeAttr("disabled");
    };

    var onNewHealingFreeChange = function (event)
    {
        var value = $(event.target).is(":checked");

        if (value) {
            $newHealingPrice.attr("disabled", "disabled");
            $newHealingPrice.val("");
        }
        else {
            $newHealingPrice.removeAttr("disabled");
        }
    }

    var onDelete = function (event) {
        var $row = $(event.target).parents("tr");
        $row.remove();
    }

    $(function () {
        $newAddButton.click(tryToAddNew);
        $newCountry.select2(select2Options);
        $("select[name^='countryOptions'][name$='countryID']").select2(select2Options);
        $table.on("change", "input[name^='countryOptions'][name$='healingFree']", onHealingFreeChange);
        $table.on("click", "button[data-deleteButton]", onDelete);
        $newFreeHealing.change(onNewHealingFreeChange);

    });

    var validate = function () {
        var success = true;
        var $rows = $table.find("tbody>tr");
        $.each($rows, function (i, row) {
            var $row = $(row);
            var $select = $row.find("select");
            var $price = $row.find("input[name$='price']");
            var $free = $row.find("input[name$='healingFree']");

            var countryName = $select.find("option:selected").text();

            var price = $price.val();
            var free = $free.is(":checked");

            if (!price && free === false)
                success = error("Price not set for " + countryName);
                

        });

        return success;
    };

    var error = function (msg) {
        $.notify(msg, "error");
        return false;
    };

    return {
        SetNationalitiesRowIndex: (index) => { rowIndex = index; },
        Validate : validate
    };

})();
