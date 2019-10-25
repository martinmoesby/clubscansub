var boatSource = {
    // Bådture
    id: "boat",
    url: eventsUrl,
    cache: false,
    extraParams: { eventtype: '@EventTypeEnum.Bådtur' },
};
var beachSource =
{
    id:"beach",
    //strandture
    url: eventsUrl,
    extraParams: { eventtype: '@EventTypeEnum.Stranddyk' },
};
var travelCourse = {
    //rejser
    id: "travel",
    url: eventsUrl,
    extraParams: { eventtype: '@EventTypeEnum.Rejse' },
};
var liveaboardSource =
{
    //liveaboards
    id: "liveaboard",
    url: eventsUrl,
    extraParams: { eventtype: '@EventTypeEnum.Liveaboard' },
};
var clubSource = {
    //klubture
    d: "club",
    url: eventsUrl,
    extraParams: { eventtype: '@EventTypeEnum.Klubture' },
};
var otherSource = {
    //Andre ture
    id: "other",
    url: eventsUrl,
    extraParams: { eventtype: '@EventTypeEnum.Other' },
};
var baseCourse = {
    //Kurser
    id: "basecourses",
    url: coursesUrl,
    extraParams: { coursetype: 0 }
};
var specCourse = {
    //Kurser
    id: "specialcourses",
    url: coursesUrl,
    extraParams: { coursetype: 3 }
};
var techCourse = {
    //Kurser
    id: "techcourses",
    url: coursesUrl,
    extraParams: { coursetype: 1 }
};
var proCourse = {
    //Kurser
    id: "procourses",
    url: coursesUrl,
    extraParams: { coursetype: 2 }
};

function eventsCheckedChanged(eventtype) {

    var isChecked = document.getElementById(eventtype + "events").checked;

    if (!isChecked) {
        calendar.getEventSourceById(eventtype).remove();

    } else {

        var eventsource = {};

        switch (eventtype) {
            case 'boat':
                eventsource = boatSource;
                break;
            case 'beach':
                eventsource = beachSource;
                break;
            case 'travel':
                eventsource = travelSource;
                break;
            case 'liveaboard':
                eventsource = liveaboardSource;
                break;
            case 'klub':
                eventsource = clubSource;
                break;
            case 'other':
                eventsource = otherSource;
                break;
            case 'basecourses':
                eventsource = baseCourse;
                break;
            case 'speccourses':
                eventsource = specCourse;
                break;
            case 'techcourses':
                eventsource = techCourse;
                break;
            case 'procourses':
                eventsource = proCourse;
                break;
            default:
                eventsource = baseCourse;
        }

        calendar.addEventSource(eventsource);
    }

}