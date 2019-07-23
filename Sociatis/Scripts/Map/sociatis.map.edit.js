window.Sociatis = window.Sociatis || {};
Sociatis.Map = Sociatis.Map || {};
Sociatis.Map.Regions = [];

Sociatis.Map.Initialize = function (mapID) {
    var mapBounds = [[-85, -179.99999755],
    [85, 179.98204259]];
    var mapMinZoom = 0;
    var mapMaxZoom = 3;
    var options = { center: [0, 0], zoom: 0, atmosphere: false };
    Sociatis.Map.Earth = new WE.map(mapID, options);

   

    var bingKey = 'ApaKA45jL6KJfA6be8gUQwxcSJw2TMHVoUXixpRLvhosFaOg5XIfMwcnsE4hnp3i';

    var bingA = Sociatis.Map.Earth.initMap(WebGLEarth.Maps.BING, ['Aerial', bingKey]);
    Sociatis.Map.Earth.setBaseMap(bingA);


    var layer = WE.tileLayer(Sociatis.Global.MapUrl, {
        bounds: mapBounds,
        minZoom: mapMinZoom,
        maxZoom: mapMaxZoom,
        opacity: 1.0//0.6
    });
    var OSM = WE.tileLayer("https://maps.wikimedia.org/osm-intl/{z}/{x}/{y}.png", {
    //var OSM = WE.tileLayer('http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        subdomains: 'abc',
        opacity: 0.7
    }).addTo(Sociatis.Map.Earth);
    //layer.addTo(Sociatis.Map.Earth);
    Sociatis.Map.Earth.panInsideBounds(mapBounds);
}

function addWERegion(region) {
    Sociatis.Map.Regions.push(region);
}

function createWERegion(region) {
    region.Polygon = region.Polygon.addTo(Sociatis.Map.Earth);
    return region;
}

$(() => {
    Sociatis.Map.Initialize("map");
});



Sociatis.Map.EditInit = function () {
    Sociatis.Map.Earth.on("click", onClick);

    function onClick(e) {
        if ($("#createPolygon").is(":checked")) {
            addWERegion(
                createWERegion(createNewRegion(e.latitude, e.longitude))
            );
        }
        else {
            var regions = Sociatis.Map.Regions;
            for (var i = 0; i < regions.length; ++i) {
                if (isInside([e.latitude, e.longitude], regions[i].Polygon)) {
                    onPolygonClick(regions[i]);
                    break;
                }

            }
        }

    }
}




function saveMap() {
    makeSaveRequest(createPostData());
}

function createPostData() {
    var data = {};
    data.regions = [];
    Sociatis.Map.Regions.forEach(region => {
        var dataRegion =
            {
                RegionID: region.Info.regionID,
                PolygonID: region.Info.polygonID,
                Editable: region.Info.editable,
                Color: region.Info.color,
                Opacity: region.Info.opacity,
                FillColor: region.Info.fillColor,
                FillOpacity: region.Info.fillOpacity,
                Weight: region.Info.weight,
                Points: createPostPointsData(region.Polygon),
                Fertility: region.Info.fertility
            };

        data.regions.push(dataRegion);
    });

    return data;
}

function makeSaveRequest(data) {
    Sociatis.AjaxBegin();
    saveRegion(data, 0);
}

function saveRegion(regionData, i) {
    if (regionData.regions.length <= i) {
        Sociatis.AjaxEnd();
        return;
    }

    var requestData =
        {
            region: regionData.regions[i]
        };

    $.post({
        url: "/Map/SaveSingleRegion",
        data: requestData,
        success: data => Sociatis.HandleJson(data, () => {
            $.notify("Region #" + i + " - " + data.Message, "success");
            saveRegion(regionData, i + 1);
        })

    });
}

function createPostPointsData(polygon) {
    var data = [];
    polygon.getPoints().forEach(point => {
        var dataPoint = {
            Latitude: point.lat,
            Longitude: point.lng
        };

        data.push(dataPoint);
    });
    return data;
}

function onPolygonClick(region) {
    var info = region.Info;

    $("#setRegion").data("region", region);
    $("#RegionID").val(info.regionID);
    $("#Editable").val(info.editable);
    $("#Color").val(info.color);
    $("#Opacity").val(info.opacity);
    $("#FillColor").val(info.fillColor);
    $("#FillOpacity").val(info.fillOpacity);
    $("#Weight").val(info.weight);

    for (var i = 1; i <= info.fertility.length; ++i) {
        $("#resource_" + i).val(info.fertility[i - 1]);
    }

}

function createNewRegion(lat, long) {
    var polygon = WE.polygon([[lat - 1, long - 1], [lat + 1, long - 1], [lat + 1, long + 1], [lat - 1, long + 1]], {
        color: '#ffff00',
        opacity: 1,
        fillColor: '#ff0000',
        fillOpacity: 0.1,
        editable: true,
        weight: 2
    });

    var Info =
        {
            regionID: null,
            polygonID: null,
            color: '#ffff00',
            opacity: 1,
            fillColor: '#ff0000',
            fillOpacity: 0.1,
            editable: true,
            weight: 2
        };

    return { Polygon: polygon, Info: Info };
}

function setRegion(button) {
    var region = $(button).data("region");
    var polygon = region.Polygon;
    var info = region.Info;

    info.regionID = $("#RegionID").val();
    info.editable = $("#Editable").is(":checked");
    info.color = $("#Color").val();
    info.opacity = $("#Opacity").val();
    info.fillColor = $("#FillColor").val();
    info.fillOpacity = $("#FillOpacity").val();
    info.weight = $("#Weight").val();

    for (var i = 1; i <= info.fertility.length; ++i) {
        info.fertility[i - 1] = $("#resource_" + i).val();
    }



    polygon.setStrokeColor(info.color, info.opacity);
    polygon.setFillColor(info.fillColor, info.fillOpacity);
    polygon.showDraggers(info.editable);

    region.Info = info;
    region.Polygon = polygon;

}

$(() => {
    setTimeout(() => Sociatis.Map.EditInit(), 1000);
});

function isInside(point, polygon) {

    var points = polygon.getPoints();
    var vs = [];

    for (var i = 0; i < points.length; ++i)
        vs.push([points[i].lat, points[i].lng]);


    // ray-casting algorithm based on
    // http://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html

    var x = point[0], y = point[1];

    var inside = false;
    for (var i = 0, j = vs.length - 1; i < vs.length; j = i++) {
        var xi = vs[i][0], yi = vs[i][1];
        var xj = vs[j][0], yj = vs[j][1];

        var intersect = ((yi > y) != (yj > y))
            && (x < (xj - xi) * (y - yi) / (yj - yi) + xi);
        if (intersect) inside = !inside;
    }

    return inside;
};