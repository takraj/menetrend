// Global

var TUSZ = TUSZ || {}

TUSZ.log = function(msg) {
    if (console) {
        console.log(msg)
    }
}

TUSZ.debug = function(msg) {
    if (console) {
        console.debug(msg)
    }
}

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
    dayNames: ['Hétfő','Kedd','Szerda','Csütörtök','Péntek','Szombat','Vasárnap'],
    dayNamesShort: ['Hé','Ke','Sze','Csü','Pén','Szo','Vas'],
    dayNamesMin: ['H','K','Sze','Cs','P','Szo','V'],
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
}

TUSZ.create_datepicker = function(html_id, default_date, range_from, range_to) {
    var datepicker_settings = $.extend({}, TUSZ.datepicker_defaults);
    
    datepicker_settings.defaultDate = default_date;
    datepicker_settings.minDate = range_from;
    datepicker_settings.maxDate = range_to;
    
    jQuery(html_id).datepicker(datepicker_settings);
}

TUSZ.create_timepicker = function(html_id, default_datetime, range_from, range_to) {
    var datepicker_settings = $.extend({}, TUSZ.datepicker_defaults);
    
    datepicker_settings.defaultDate = default_datetime;
    datepicker_settings.minDate = range_from;
    datepicker_settings.maxDate = range_to;
    datepicker_settings.hour = default_datetime.getHours();
    datepicker_settings.minute = default_datetime.getMinutes();
    
    jQuery(html_id).datetimepicker(datepicker_settings);
}

TUSZ.get_date_from_datepicker = function(html_id) {
    return jQuery(html_id).datepicker('getDate')
}

TUSZ.get_datetime_from_datetimepicker = function(html_id) {
    return jQuery(html_id).datetimepicker('getDate')
}

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
        match = $(this).val();
        
        if (e && e.which === $.ui.keyCode.ESCAPE || $.trim(match) === ""){
            routes_tree.clearFilter();
        } else {
            routes_tree.filterNodes(match, true);
        }
        
        routes_tree.filterNodes(match, true);
    }).focus();
}

TUSZ.get_selection_from_tree = function(tree_html_id) {
    try {
        return $(tree_html_id).fancytree("getActiveNode").key;
    } catch(e) {
        return "";
    }
}

TUSZ.add_item_to_children = function(children, title, key) {
    var additional_data = [
        {title: title, key: key}
    ]
    return children.concat(additional_data)
}

TUSZ.add_folder_to_tree_data = function(tree_data, folder_name, folder_id, children) {
    var additional_data = [
        {title: folder_name, key: folder_id, folder: true, children: children}
    ]
    return tree_data.concat(additional_data)
}

// Admin UI specific

TUSZ.admin_ui = TUSZ.admin_ui || {}
