window.Sociatis = window.Sociatis || {};
Sociatis.Hospital = Sociatis.Hospital || {};

Sociatis.Hospital = (function () {
    var healingPrice = null;
    var healingEnabled = null;
    var healingFree = null;


    var $healingEnabled = $("#HealingEnabled");
    var $healingFree = $("#FreeHealing");
    var $healingPrice = $("#HealingPrice");

    var $saveButton = $("#manageHospital button");

    var autoUpdateHealingPriceState = function () {
        if (healingFree) {
            $healingPrice.disable();
            $healingPrice.val(0);
        }
        else
            $healingPrice.enable();
    };

    var onChange = function () {
        healingPrice = Number($healingPrice.val());
        healingEnabled = $healingEnabled.is(":checked");
        healingFree = $healingFree.is(":checked");

        autoUpdateHealingPriceState();
    };

    $(function () {
        $healingFree.change(onChange);
        $healingPrice.change(onChange);

        onChange();
    });

    return {
        GetHealingPrice: () => { return healingPrice; },
        GetHealingEnabled: () => { return healingEnabled; },
        GetHealingFree: () => { return healingFree; }
    };
})();