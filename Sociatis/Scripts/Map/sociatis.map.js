window.Sociatis = window.Sociatis || {};
Sociatis.Map = Sociatis.Map || {};
Sociatis.Map.Regions = [];

Sociatis.Map.Initialize = function (mapID) {
    Sociatis.Map.Earth = L.map(mapID).setView([51.505, -0.09], 5);


    // for all possible values and explanations see "Template Parameters" in https://msdn.microsoft.com/en-us/library/ff701716.aspx
    var imagerySet = "Aerial"; // AerialWithLabels | Birdseye | BirdseyeWithLabels | Road

    var bing = new L.BingLayer("ApaKA45jL6KJfA6be8gUQwxcSJw2TMHVoUXixpRLvhosFaOg5XIfMwcnsE4hnp3i", { type: imagerySet });

    Sociatis.Map.Earth.addLayer(bing);
    Sociatis.Map.Earth.options.minZoom = 1;
    Sociatis.Map.Earth.options.maxZoom = 9;
}

Sociatis.Map.InitializeGeoJSON = function (mapID) {
    Sociatis.Map.Earth = L.map(mapID).setView([51.505, -0.09], 5);


    // for all possible values and explanations see "Template Parameters" in https://msdn.microsoft.com/en-us/library/ff701716.aspx
    var imagerySet = "Aerial"; // AerialWithLabels | Birdseye | BirdseyeWithLabels | Road

    var bing = new L.BingLayer("ApaKA45jL6KJfA6be8gUQwxcSJw2TMHVoUXixpRLvhosFaOg5XIfMwcnsE4hnp3i", { type: imagerySet });

    Sociatis.Map.Earth.addLayer(bing);
    Sociatis.Map.Earth.options.minZoom = 1;
    Sociatis.Map.Earth.options.maxZoom = 5;

    Sociatis.Map.LGeoJSON = L.geoJson(Sociatis.Map.GeoJSON, { style: Sociatis.Map.Style, onEachFeature: Sociatis.Map.OnEach }).addTo(Sociatis.Map.Earth);
}

Sociatis.Map.ResetHighlight = e => {
    Sociatis.Map.LGeoJSON.resetStyle(e.target);
}

Sociatis.Map.Zoom = e => {
    Sociatis.Map.Earth.fitBounds(e.target.getBounds());
}

Sociatis.Map.Highlight = e => {
    var layer = e.target;

    layer.setStyle({
        weight: 5,
        color: '#FFFF00',
        dashArray: '',
        fillOpacity: 0.85
    });

    if (!L.Browser.ie && !L.Browser.opera && !L.Browser.edge) {
        layer.bringToFront();
    }
}

Sociatis.Map.OnEach = (feature, layer) => {
    layer.on({
        mouseover: Sociatis.Map.Highlight,
        mouseout: Sociatis.Map.ResetHighlight,
        click: Sociatis.Map.Zoom
    });
}

function addWERegion(region) {
    Sociatis.Map.Regions.push(region);
}

function createWERegion(region) {
    region.Polygon = region.Polygon.addTo(Sociatis.Map.Earth);
    return region;
}