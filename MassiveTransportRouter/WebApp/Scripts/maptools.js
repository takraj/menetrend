"use strict";

// Namespace definition
var MTR = MTR || {};
MTR.MapTools = MTR.MapTools || {};

// Create Map Object
MTR.MapTools.createMap = function (tag) {
    // Global map options
    MTR.MapTools.mapOptions = {
        center: new google.maps.LatLng(47.5, 19.08),
        zoom: 12,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };

    // Global map variable
    MTR.MapTools.map = new google.maps.Map(document.getElementById(tag), MTR.MapTools.mapOptions);

    // Container Variables, Arrays
    MTR.MapTools.nodes = new Array();
}

// Adds a Marker to the Map
MTR.MapTools.addMarkerToMap = function(lat, lon, title) {
    var myLatlng = new google.maps.LatLng(lat, lon);
    MTR.MapTools.nodes.push({
        marker: new google.maps.Marker({
            position: myLatlng,
            map: MTR.MapTools.map,
            title: title
        }),
        infoWindow: null
    });
}

// Returns the list of Node Coordinates
MTR.MapTools.getNodeCoords = function() {
    var waypoints = new Array();

    MTR.MapTools.nodes.forEach(function (node) {
        waypoints.push(node.marker.position);
    });

    return waypoints;
}

// Creates an InfoWindow
MTR.MapTools.createInfoWindow = function(node, content, isOpened) {
    node.infoWindow = new google.maps.InfoWindow({ content: content });
    if (isOpened) node.infoWindow.open(MTR.MapTools.map, node.marker);
    google.maps.event.addListener(node.marker, 'click', function () {
        node.infoWindow.open(MTR.MapTools.map, node.marker);
    });
}

// Setup infoWindows
MTR.MapTools.setupInfoWindows = function () {
    MTR.MapTools.nodes.forEach(function (node, index) {
        if (index == 0) {
            MTR.MapTools.createInfoWindow(node, "Kezdőpont: " + node.marker.title, true);
        } else if (index == (MTR.MapTools.nodes.length - 1)) {
            MTR.MapTools.createInfoWindow(node, "Végpont: " + node.marker.title, true);
        } else {
            MTR.MapTools.createInfoWindow(node, "<i>Köztes pont: " + node.marker.title + "</i>", false);
        }
    });
}

// Draw path
MTR.MapTools.drawPath = function () {
    // Draw Route
    var line = new google.maps.Polyline({
        path: MTR.MapTools.getNodeCoords(),
        strokeColor: "#0000FF",
        strokeOpacity: 1.0,
        strokeWeight: 2,
        map: MTR.MapTools.map,
        icons: [{
            icon: { path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW },
            offset: '100%'
        }]
    });

    // Animating Arrow Icon
    var count = 0;
    var offsetId = window.setInterval(function () {
        count = (++count) % 100;

        var icons = line.get('icons');
        icons[0].offset = count + '%';
        line.set('icons', icons);
    }, 40);
}