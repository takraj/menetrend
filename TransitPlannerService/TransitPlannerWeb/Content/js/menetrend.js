"use strict";

var TUSZ = TUSZ || {};

TUSZ.log = function(msg) {
    if (console) {
        console.log(msg)
    }
};

TUSZ.debug = function(msg) {
    if (console) {
        console.debug(msg)
    }
};

TUSZ.datepicker_defaults = {
    alwaysSetTime: true,
    altFieldTimeOnly: false,
    closeText: 'Bezár',
    prevText: 'Előző',
    nextText: 'Következő',
    currentText: 'Most',
    monthNames: ['Január','Február','Március','Április','Május','Június',
    'Július','Augusztus','Szeptember','Október','November','December'],
    monthNamesShort: ['Jan','Feb','Már','Ápr','Máj','Jún',
    'Júl','Aug','Szept','Okt','Nov','Dec'],
    dayNames: ['Vasárnap', 'Hétfő', 'Kedd', 'Szerda', 'Csütörtök', 'Péntek', 'Szombat'],
    dayNamesShort: ['Vas', 'Hé', 'Ke', 'Sze', 'Csü', 'Pén', 'Szo'],
    dayNamesMin: ['V', 'H', 'K', 'Sze', 'Cs', 'P', 'Szo'],
    weekHeader: 'Hét',
    dateFormat: 'dd.mm.yy',
    firstDay: 1,
    isRTL: false,
    showMonthAfterYear: true,
    yearSuffix: '',
    timeFormat: 'HH:mm',
    timeSuffix: '',
    timeOnlyTitle: 'Mikor',
    timeText: 'Idő',
    hourText: 'Óra',
    minuteText: 'Perc',
    showButtonPanel: true
};

TUSZ.create_datepicker = function(html_id, default_date, range_from, range_to) {
    var datepicker_settings = $.extend({}, TUSZ.datepicker_defaults);
    
    datepicker_settings.defaultDate = default_date;
    datepicker_settings.minDate = range_from;
    datepicker_settings.maxDate = range_to;
    
    jQuery(html_id).datepicker(datepicker_settings);
};

TUSZ.create_timepicker = function(html_id, default_datetime, range_from, range_to) {
    var datepicker_settings = $.extend({}, TUSZ.datepicker_defaults);
    
    datepicker_settings.defaultDate = default_datetime;
    datepicker_settings.minDate = range_from;
    datepicker_settings.maxDate = range_to;
    datepicker_settings.hour = default_datetime.getHours();
    datepicker_settings.minute = default_datetime.getMinutes();
    
    jQuery(html_id).datetimepicker(datepicker_settings);
};

TUSZ.get_date_from_datepicker = function(html_id) {
    return jQuery(html_id).datepicker('getDate');
};

TUSZ.get_datetime_from_datetimepicker = function(html_id) {
    return jQuery(html_id).datetimepicker('getDate');
};

TUSZ.set_date_of_datepicker = function(html_id, year, month, day) {
    return jQuery(html_id).datepicker('setDate', new Date(year, month, day));
};

TUSZ.set_datetime_of_datetimepicker = function(html_id, year, month, day, hour, minute) {
    return jQuery(html_id).datetimepicker('setDate', new Date(year, month, day, hour, minute));
};

TUSZ.create_filterable_tree = function(tree_html_id, filter_html_id, data_source) {
    var routes_tree = $(tree_html_id).fancytree({
        extensions: ["filter"],
        quicksearch: true,
        source: data_source,
        filter: {
            mode: "hide",
            autoApply: true
        },
        beforeSelect: function(event, data){
            return !data.node.isFolder();
        },
        beforeActivate: function(event, data){
            return !data.node.isFolder();
        }
    });
    var routes_tree = $(tree_html_id).fancytree("getTree");
    
    $(filter_html_id).keyup(function(e){
        var match = $(this).val();
        
        if (e && e.which === $.ui.keyCode.ESCAPE || $.trim(match) === ""){
            routes_tree.clearFilter();
        } else {
            routes_tree.filterNodes(match, true);
        }
        
        routes_tree.filterNodes(match, true);
    }).focus();
};

TUSZ.get_selection_from_tree = function(tree_html_id) {
    try {
        return $(tree_html_id).fancytree("getActiveNode").key;
    } catch(e) {
        return "";
    }
};

TUSZ.add_item_to_children = function(children, title, key) {
    var additional_data = [
        {title: title, key: key}
    ];
    return children.concat(additional_data)
};

TUSZ.add_folder_to_tree_data = function(tree_data, folder_name, folder_id, children) {
    var additional_data = [
        {title: folder_name, key: folder_id, folder: true, children: children}
    ];
    return tree_data.concat(additional_data);
};

TUSZ.add_stop_to_autocomplete_list = function(autocomplete_list, stop_id, stop_name, stop_postal_code, stop_city, stop_street) {
    var additional_data = [{
        label: stop_name,
        identifier: stop_id,
        postal_code: stop_postal_code,
        city: stop_city,
        street: stop_street
    }];
    return autocomplete_list.concat(additional_data);
};

TUSZ.get_stop_id_by_label = function(stops, stop_label) {
    for (var i = 0; i < stops.length; i++) {
        if (stops[i].label == stop_label) {
            return stops[i].identifier;
        }
    }
    return -1;
}

TUSZ.create_autocomplete_for_stops = function(txt_html_id, data_source) {
    var autocomplete = jQuery(txt_html_id).autocomplete({
        minLength: 3,
        source: data_source
    });
    
    autocomplete.autocomplete("instance")._renderItem = function(ul, item) {
        return $("<li>")
        .append("<a><strong>").append(item.label).append("</strong><br />")
        .append("<em>")
        .append(item.postal_code)
        .append(", ").append(item.city)
        .append(", ").append(item.street)
        .append("</em></a>")
        .appendTo(ul);
    };
    
    $("ul.ui-autocomplete.ui-menu").css('fontSize', '10px');
};

TUSZ.create_map = function(html_id, lat, lon) {
    var mapOptions = {
        center: new google.maps.LatLng(lat, lon),
        zoom: 12,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };

    return new google.maps.Map(document.getElementById(html_id), mapOptions);
}

TUSZ.create_marker_label = function(route_short_name, base_color, text_color) {
    return "".concat(
        "<span class='routeNameBox routeNameBoxLabel' style='background-color: ",
        base_color,
        "; color: ",
        text_color,
        "'>",
        route_short_name,
        "</span>"
    );
}

TUSZ.replace_text = function(subject, original, replacement) {
    return subject.split(original).join(replacement);
}

TUSZ.create_info_window_content = function(route_short_name, route_base_color, route_text_color, route_type, route_direction, list_of_stops) {
    var infowindow_template = $("#template-infowindow").html();
    var infowindow_table_row_template = $("#template-infowindow-table").html();
    
    var rows = "";
    for (var i = 0; i < list_of_stops.length; i++) {
        var row = TUSZ.replace_text(infowindow_table_row_template, "##TIME##", list_of_stops[i].time);
        row = TUSZ.replace_text(row, "##STOP_NAME##", list_of_stops[i].stop_name);
        rows = rows + row;
    }
    
    var content = TUSZ.replace_text(infowindow_template, "##ROUTE_SHORT_NAME##", route_short_name);
    content = TUSZ.replace_text(content, "##ROUTE_BASE_COLOR##", route_base_color);
    content = TUSZ.replace_text(content, "##ROUTE_TEXT_COLOR##", route_text_color);
    content = TUSZ.replace_text(content, "##ROUTE_TYPE##", route_type);
    content = TUSZ.replace_text(content, "##ROUTE_DIRECTION##", route_direction);
    content = TUSZ.replace_text(content, "##ROUTE_TABLE_ROWS##", "<table class='trip-details'>" + rows + "</table>");
    
    return content;
}

TUSZ.create_travel_marker = function(map, lat, lon, route_data) {
    var myLatlng = new google.maps.LatLng(lat, lon);
    var labelContent = TUSZ.create_marker_label(
        route_data.short_name,
        route_data.base_color,
        route_data.text_color
    );
    
    var marker = new MarkerWithLabel({
        position: myLatlng,
        map: map,
        title: route_data.short_name + ": " + route_data.direction,
        labelContent: labelContent,
        labelAnchor: new google.maps.Point(15, 0)
    });
    
    var infoWindowContent = TUSZ.create_info_window_content(
        route_data.short_name,
        route_data.base_color,
        route_data.text_color,
        route_data.type,
        route_data.direction,
        route_data.stops
    );
    var infoWindow = new google.maps.InfoWindow({ content: infoWindowContent });
    
    google.maps.event.addListener(marker, 'click', function() {
        infoWindow.open(map, marker);
    });
    
    return marker;
}

TUSZ.create_finish_marker = function(map, lat, lon, stop_name) {
    var myLatlng = new google.maps.LatLng(lat, lon);
    
    var marker = new google.maps.Marker({
        position: myLatlng,
        map: map,
        title: stop_name,
        icon: "https://chart.googleapis.com/chart?chst=d_map_pin_icon&chld=flag|ADDE63",
        shadow: "https://maps.gstatic.com/mapfiles/ms2/micons/msmarker.shadow.png"
    });
    
    var infoWindowContent = "<p><strong>Cél: </strong><br />" + stop_name + "</p>";
    var infoWindow = new google.maps.InfoWindow({ content: infoWindowContent });
    
    google.maps.event.addListener(marker, 'click', function() {
        infoWindow.open(map, marker);
    });
    
    return marker;
}

TUSZ.create_walking_marker = function(map, lat, lon, stop_name) {
    var myLatlng = new google.maps.LatLng(lat, lon);
    
    var marker = new google.maps.Marker({
        position: myLatlng,
        map: map,
        title: stop_name
    });
    
    var infoWindowContent = "<p><strong>Séta innen: </strong><br />" + stop_name + "</p>";
    var infoWindow = new google.maps.InfoWindow({ content: infoWindowContent });
    
    google.maps.event.addListener(marker, 'click', function() {
        infoWindow.open(map, marker);
    });
    
    return marker;
}

TUSZ.create_travel_line = function(map, lat_from, lon_from, lat_to, lon_to, color) {
    var from = new google.maps.LatLng(lat_from, lon_from);
    var to = new google.maps.LatLng(lat_to, lon_to);
    
    var line = new google.maps.Polyline({
        path: [from, to],
        strokeColor: color,
        strokeOpacity: 1.0,
        strokeWeight: 4,
        map: map,
        icons: [{
            icon: { path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW },
            offset: '50%'
        }]
    });
    
    return line;
}

TUSZ.create_walking_line = function(map, lat_from, lon_from, lat_to, lon_to) {
    var from = new google.maps.LatLng(lat_from, lon_from);
    var to = new google.maps.LatLng(lat_to, lon_to);
    
    var lineSymbol = {
        path: 'M 0,-1 0,1',
        strokeOpacity: 1.0,
        scale: 4
    };
    
    var line = new google.maps.Polyline({
        path: [from, to],
        strokeColor: "#B3002D",
        strokeOpacity: 1.0,
        strokeWeight: 2,
        map: map,
        icons: [{
            icon: { path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW },
            offset: '50%'
        },
        {
            icon: lineSymbol,
            offset: '0',
            repeat: '20px'
        }]
    });
    
    return line;
}

TUSZ.set_map_center = function(map, lat, lon) {
    var myLatlng = new google.maps.LatLng(lat, lon);
    map.setCenter(myLatlng);
}

TUSZ.create_route_data = function(short_name, base_color, text_color, type, direction) {
    return {
        short_name: short_name,
        direction: direction,
        base_color: base_color,
        text_color: text_color,
        type: type,
        stops: [],
        add_stop: function(time, stop_name) {
            this.stops.push({time: time, stop_name: stop_name});
        }
    }
}
