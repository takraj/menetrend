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
    MTR.MapTools.pathCoords = new Array();
}

// Adds a node for the route path
MTR.MapTools.addNode = function (lat, lon) {
    MTR.MapTools.pathCoords.push(new google.maps.LatLng(lat, lon));
}

// Adds a Marker to the Map
MTR.MapTools.addMarkerToMap = function(lat, lon, title, labelcontent, popupcontent) {
    var myLatlng = new google.maps.LatLng(lat, lon);
    MTR.MapTools.nodes.push({
        marker: new MarkerWithLabel({
            position: myLatlng,
            map: MTR.MapTools.map,
            title: title,
            labelContent: labelcontent,
            labelAnchor: new google.maps.Point(15, 0)
        }),
        infoWindow: null,
        popupcontent: popupcontent
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
        if (index == (MTR.MapTools.nodes.length - 1)) {
            MTR.MapTools.createInfoWindow(node, "Végpont: " + node.marker.title, true);
            node.marker.setIcon("https://chart.googleapis.com/chart?chst=d_map_pin_icon&chld=flag|ADDE63");
            var shadow = new google.maps.MarkerImage("https://maps.gstatic.com/mapfiles/ms2/micons/msmarker.shadow.png", null, null, new google.maps.Point(16, 32));
            node.marker.setShadow(shadow);
        } else {
            MTR.MapTools.createInfoWindow(node, node.popupcontent, false);
        }
    });
}

// Draw path
MTR.MapTools.drawPath = function () {
    // Draw Route
    var line = new google.maps.Polyline({
        path: MTR.MapTools.pathCoords,
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