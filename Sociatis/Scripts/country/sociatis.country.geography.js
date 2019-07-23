window.Sociatis = window.Sociatis || {};
Sociatis.Geography = (function () {
    var $expander = $("#geogrpahyView i#regionExpander");
    var $list = $("#geogrpahyView .geoDropdown .list");
    var $regions = $("#geogrpahyView .region");

    var isExpanded = false;

    var setExpandedState = function(newExpandedState)
    {
        isExpanded = newExpandedState;
        if (newExpandedState) {
            $expander.removeClass("fa-chevron-down");
            $expander.addClass("fa-chevron-up");

            $list.removeClass("colapsed");
            $list.addClass("expanded");
        }
        else {
            $expander.removeClass("fa-chevron-up");
            $expander.addClass("fa-chevron-down");

            $list.removeClass("expanded");
            $list.addClass("colapsed");
        }
    }

    var onExpandClick = function () {
        setExpandedState(!isExpanded);
    }

    var displayRegion = function (regionID) {
        $regions.addClass("hide");
        $regions.filter("[data-regionid='" + regionID + "']").removeClass("hide");
        setExpandedState(false);
    }

    var changeSpawnOptions = function (e, input, regionID) {
        var value = input.value.toLowerCase() === "true";
        var data =
            {
                regionID: regionID,
                enabled: value
            };

        Sociatis.AjaxBegin();

        var url = Sociatis.GetAdress("SetSpawnState", "Region");

        $.postJSON(url, data, (data) => Sociatis.HandleJson(data, onSpawnChange, null, () => {
            $("#region" + regionID + "spawn").val(!value);

            $("#region" + regionID + "spawn").trigger("refresh");
        }), (data) => Sociatis.AjaxErrorGeneric(data)
        );

        console.log(data);
    }

    var onSpawnChange = function (data, args) {
        var msg = data.Message;
        $.notify(msg, "success");
        Sociatis.AjaxEnd();
    }


    return {
        OnExpanderClick: onExpandClick,
        SelectRegionToDisplay: regionID => displayRegion(regionID),
        ChangeSpawnOption: (event, input, regionID) => changeSpawnOptions(event, input, regionID)
    };
})();


