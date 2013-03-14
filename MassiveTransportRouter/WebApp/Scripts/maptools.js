"use strict";

// !! -- TO DO -- !!

var MTR = MTR || {};
MTR.MapTools = MTR.MapTools || {};

MTR.MapTools.mapOptions = {
    center: new google.maps.LatLng(47.5, 19.08),
    zoom: 12,
    mapTypeId: google.maps.MapTypeId.ROADMAP
};

MTR.MapTools.map = new google.maps.Map(document.getElementById("map_canvas"), mapOptions);

// Container Variables, Arrays
MTR.MapTools.nodes = new Array();

// Adds a Marker to the Map
MTR.MapTools.addMarkerToMap = function(lat, lon, title) {
    var myLatlng = new google.maps.LatLng(lat, lon);
    MTR.MapTools.nodes.push({
        marker: new google.maps.Marker({
            position: myLatlng,
            map: map,
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
    if (isOpened) node.infoWindow.open(map, node.marker);
    google.maps.event.addListener(node.marker, 'click', function () {
        node.infoWindow.open(map, node.marker);
    });
}
