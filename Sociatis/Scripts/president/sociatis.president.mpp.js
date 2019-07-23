window.Sociatis = window.Sociatis || {};
Sociatis.President = Sociatis.President || {};

Sociatis.President = (function() {

    var $goldCost = $("#mppNeededGold");
    var $goldDiv = $("#mppGoldCost");
    var $country = $("[name='OtherCountryID']");
    var $days = $("#mppDays");
    var countryID;
    var updateGold = function (data) {
        if (data.Data > 0) {
            $goldCost.text(data.Data);
            $goldDiv.show();
        }
        else {
            $goldDiv.hide();
        }
        Sociatis.AjaxEnd();
    }

    var getGoldCost = function()
    {
        if (!($country.val())) {
            $goldDiv.hide();
            return;
        }
        var url = Sociatis.GetAdress("GetGoldCostForMPPOffer", "Country");
        var data =
            {
                firstCountryID: countryID,
                secondCountryID: $country.val(),
                length: $days.val()
            };
        Sociatis.AjaxBegin();
        $.postJSON(url, data, data => Sociatis.HandleJson(data, updateGold));
    }

    $(() => {
        $days.change(function () {
            var $this = $(this);
            var val = $this.val();
            var max = Number($this.attr("max"));
            var min = Number($this.attr("min"));
            if (val < min)
                $this.val(min);
            if (val > max)
                $this.val(max);
                    
            getGoldCost();
        });

        $country.change(function () {
            getGoldCost();
        });
    });

    getGoldCost();

    return {
        SetCountryID: cID => { countryID = cID; }
    };

})();