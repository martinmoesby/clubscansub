var map, searchManager, pin;

function GetMap(includeClick) {

    map = new Microsoft.Maps.Map("#meetLocationMap");

    map.setView({
        mapTypeId: Microsoft.Maps.MapTypeId.aerial
    });

    if (includeClick  || includeClick === undefined) {
        Microsoft.Maps.Events.addHandler(map, 'click', displayLatLong);
    }

    Microsoft.Maps.loadModule('Microsoft.Maps.Search', function () {
        searchManager = new Microsoft.Maps.Search.SearchManager(map);
    });


    var lat = $("#latitude").val();
    var lon = $("#longitude").val();



    if (lat !== "" && lon !== "") {


        var location = new Microsoft.Maps.Location(lat, lon);

        var pushpinName = "Mødested";

        if ($('#addressName').val() !== "") {
            pushpinName = $('#addressName').val();
        }


        addPushpin(location,pushpinName);


        map.setView({
            center: location,
            zoom: 15
        });
    } else {

        geocodeMap();
    }

}

function geocodeMap() {

    var query = $('#addressStreetname').val() + "," +
        $('#addressZipcode').val() + " " +
        $('#addressCity').val() + ", " +
        $('#addressCountry').val();

   
    var searchRequest = {
        where: query,
        callback: function (r) {
            //Add the first result to the map and zoom into it.
            if (r && r.results && r.results.length > 0) {

                addPushpin(r.results[0].location);

                $("#latitude").val(r.results[0].location.latitude);
                $("#longitude").val(r.results[0].location.longitude);
            }
        },
        errorCallback: function (e) {
            //If there is an error, alert the user about it.
            alert("No results found.");
        }
    };

    //Make the geocode request.
    searchManager.geocode(searchRequest);
}

function displayLatLong(e) {
    if (e.targetType === "map") {
        var point = new Microsoft.Maps.Point(e.getX(), e.getY());
        var loc = e.target.tryPixelToLocation(point);

        document.getElementById("latitude").value = loc.latitude;
        document.getElementById("longitude").value = loc.longitude;

        getAdressByLocation(loc);

        addPushpin(loc);

    }
}

function addPushpin(location) {

    var pushpinName = "Mødested";

    //if ($('#addressName') !== undefined) {
    //    pushpinName = $('#addressName').val();
    //}
    console.log(pin);

    map.entities.clear();
//    map.layers.clear();

    //Create custom Pushpin
    if (pin === undefined) {
        var pin = new Microsoft.Maps.Pushpin(location,pushpinName);
        //Add the pushpin to the map
        map.entities.push(pin);

    } else {
        pin.setLocation(location);
    }

    pin.setOptions({
        title: pushpinName
    });

    map.setView({
        center: location
    });

}

function getAdressByLocation(location) {

    var point = location.latitude + "," + location.longitude;
    var types = "Address,Postcode1,CountryRegion,Locality";
    var key = "Am0MRTxgaifeEFyXMSZw5AWlnAaMTVOcK2xBxt2c9gVbGJjc0zIK7lOldalP4biR";
    var prod_key = "Anv6_xU3GX5rnzLtJ6EZbiHF2vRnfONCI9fVn43ibVRDJD-ol9rFRmOb5aGKDLAC";

    var query = "https://dev.virtualearth.net/REST/v1/Locations/" + point + "?includeEntityType=" + types + "&key=" + prod_key;

    //http://dev.virtualearth.net/REST/v1/Locations/56.02614171580097,12.604299224804851?key=Am0MRTxgaifeEFyXMSZw5AWlnAaMTVOcK2xBxt2c9gVbGJjc0zIK7lOldalP4biR


    $.getJSON(query, function (data) {

        $('#addressCity').val(data.resourceSets[0].resources[0].address.locality);
        $('#addressZipcode').val(data.resourceSets[0].resources[0].address.postalCode);
        $('#addressStreetname').val(data.resourceSets[0].resources[0].address.addressLine);
        $('#addressCountry').val(data.resourceSets[0].resources[0].address.countryRegion);

        if ($('#addressName').val() === "") {
            $('#addressName').val(data.resourceSets[0].resources[0].name);
        }

    });

}
