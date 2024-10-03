namespace ClubScansub.Utility
{
    public static class IconsTextsAndColors
    {
        public static class Icons
        {
            public static string CALENDAR = "calendar_month";
            public static string END_TIME = "alarm_off";
            public static string START_TIME = "alarm";
            public static string CANCELLED = "cancel";
            
            public static string AMOUNT = "payments";
            public static string SLOTS = "event_seat";
            public static string EDUCATION = "school";
            public static string ENROLLED = "social_leaderboard";
            public static string COMPLETED = "check_circle";
            
            public static string USER = "person";
            public static string USER_10 = "group";
            public static string USER_ADD = "person_add";
            public static string USER_MAX = "person_add";
            public static string USER_MIN = "person_remove";
            public static string USER_CHECK = "person_check";

            public static string LOGIN = "login";
            public static string LOGOUT = "logout";
            
            public static string INFORMATION = "info";
            public static string DELETE = "delete";
            public static string CLOSE = "close";
            public static string SEARCH = "search";
            public static string SAVE = "save";

            public static string MAX_DEPTH = "vertical_align_bottom";
            public static string MIN_DEPTH = "vertical_align_top";


            public static string EVENTTYPE_BASE_COURSE = "school";
            public static string EVENTTYPE_SPEC_COURSE = "military_tech";
            public static string EVENTTYPE_TECH_COURSE = "editor_choice";
            public static string EVENTTYPE_PRO_COURSE = "social_leaderboard";
            public static string EVENTTYPE_BOAT = "sailing";
            public static string EVENTTYPE_BEACH = "beach_access";
            public static string EVENTTYPE_CLUB = "groups";
            public static string EVENTTYPE_OTHER = "other_admission";
            public static string EVENTTYPE_TRAVEL = "flight_takeoff";
            public static string EVENTTYPE_LIVEABOARD = "houseboat";
            public static string EVENTTYPE_NOT_EVENT = "unknown_document";

            public static string COURSE_SESSION_POOL = "pool";
            public static string COURSE_SESSION_ACADEMIC = "dictionary";
            public static string COURSE_SESSION_OW= "scuba_diving";

            public static string DIVETYPE_WRECK = "houseboat";
            public static string DIVETYPE_POOL = COURSE_SESSION_POOL;
            public static string DIVETYPE_NATURE = "emoji_nature";
            public static string DIVETYPE_TECH = EVENTTYPE_TECH_COURSE;


        }

        public static class Texts
        {
            public static string SCHEDULER_DAY = "Dag";
            public static string SCHEDULER_WEEK = "Uge";
            public static string SCHEDULER_MONTH = "Måned";
            public static string SCHEDULER_YEAR = "År";
            public static string SCHEDULER_TIMELINE = "Tidslinje";
            public static string SCHEDULER_PLANNER = "Årsplanner";
        }

        public static class ColorClasses
        {
            public static string BG_BOAT = "rz-background-color-success-dark";
            public static string FG_BOAT = "rz-color-on-success-dark";

            public static string BG_CLUB = "rz-background-color-success";
            public static string FG_CLUB = "rz-color-on-success";
            
            public static string BG_BEACH = "rz-background-color-success-light";
            public static string FG_BEACH = "rz-color-on-success-light";

            public static string BG_OTHER = "rz-background-color-info-dark";
            public static string FG_OTHER = "rz-color-on-info-dark";
            
            public static string BG_TRAVEL = "rz-background-color-info";
            public static string FG_TRAVEL = "rz-color-on-info";
            
            public static string BG_LIVEABOARD = "rz-background-color-info-light";
            public static string FG_LIVEABOARD = "rz-color-on-info-light";

            public static string BG_NOTEVENT = "rz-background-color-danger";
            public static string FG_NOTEVENT = "rz-color-on-danger";


            public static string BG_COURSE_BASE = "rz-background-color-warning-light";
            public static string FG_COURSE_BASE = "rz-color-on-warning-light";

            public static string BG_COURSE_SPEC = "rz-background-color-warning";
            public static string FG_COURSE_SPEC = "rz-color-on-warning";

            public static string BG_COURSE_TECH = "rz-background-color-warning-dark";
            public static string FG_COURSE_TECH = "rz-color-on-warning-dark";

            public static string BG_COURSE_PRO = "rz-background-color-warning-darker";
            public static string FG_COURSE_PRO = "rz-color-on-warning-darker";

            public static string BG_DEFAULT = "rz-background-color-info-lighter";
            public static string FG_DEFAULT = "rz-color-on-info-lighter";

            public static string SCHEDULER_SLOT_BG_TODAY = "rz-background-color-info-lighter";
            public static string SCHEDULER_SLOT_BG_WEEKEND = "rz-background-color-secondary-lighter";
            public static string SCHEDULER_SLOT_BG_HOLIDAY = "rz-background-color-danger-lighter";
            public static string SCHEDULER_SLOT_BG_WORKHOURS = "rz-background-color-success-lighter";

            public static string BG_ISCANCELLED = "rz-background-color-secondary-lighter";
        }

        public static string GetCourseIcon(CourseTypeEnum item)
        {
            switch (item)
            {
                case CourseTypeEnum.BaseCourse:
                    return Icons.EVENTTYPE_BASE_COURSE;
                case CourseTypeEnum.SpecialtyCourse:
                    return Icons.EVENTTYPE_SPEC_COURSE;
                case CourseTypeEnum.TechCourse:
                    return Icons.EVENTTYPE_TECH_COURSE;
                case CourseTypeEnum.ProCourse:
                    return Icons.EVENTTYPE_PRO_COURSE;

            }
            return "";
        }

        public static string GetDiveTypeIcon(DiveTypeEnum item)
        {
            switch (item)
            {
                case DiveTypeEnum.WreckDive:
                    return Icons.DIVETYPE_WRECK;
                case DiveTypeEnum.NatureDive:
                    return Icons.DIVETYPE_NATURE;
                case DiveTypeEnum.TechDive:
                    return Icons.DIVETYPE_TECH;
                case DiveTypeEnum.PoolDive:
                    return Icons.DIVETYPE_POOL;
                default:
                    return "";
            }
        }

        public static string GetCourseSessionIcon(CourseSessionTypeEnum item)
        {
            switch (item)
            {
                case CourseSessionTypeEnum.AcademicSession:
                    return Icons.COURSE_SESSION_ACADEMIC;
                case CourseSessionTypeEnum.PoolSession:
                    return Icons.COURSE_SESSION_POOL;
                case CourseSessionTypeEnum.OpenWaterSession:
                    return Icons.COURSE_SESSION_OW;
            }
            return "";
        }

        public static string GetCourseBackgroundColor(CourseTypeEnum item)
        {
            switch (item)
            {
                case CourseTypeEnum.BaseCourse:
                    return ColorClasses.BG_COURSE_BASE;
                case CourseTypeEnum.TechCourse:
                    return ColorClasses.BG_COURSE_TECH;
                case CourseTypeEnum.ProCourse:
                    return ColorClasses.BG_COURSE_PRO;
                case CourseTypeEnum.SpecialtyCourse:
                    return ColorClasses.BG_COURSE_SPEC;
                default:
                    return ColorClasses.BG_DEFAULT;
            }
        }

        public static string GetCourseColor(CourseTypeEnum item)
        {
            switch (item)
            {
                case CourseTypeEnum.BaseCourse:
                    return ColorClasses.FG_COURSE_BASE;
                case CourseTypeEnum.TechCourse:
                    return ColorClasses.FG_COURSE_TECH;
                case CourseTypeEnum.ProCourse:
                    return ColorClasses.FG_COURSE_PRO;
                case CourseTypeEnum.SpecialtyCourse:
                    return ColorClasses.FG_COURSE_SPEC;
                default:
                    return ColorClasses.FG_DEFAULT;
            }
        }

        public static string GetEventBackgroundColor(EventTypeEnum item)
        {
            switch (item)
            {
                case EventTypeEnum.Bådtur:
                    return ColorClasses.BG_BOAT;
                case EventTypeEnum.Stranddyk:
                    return ColorClasses.BG_BEACH;
                case EventTypeEnum.Rejse:
                    return ColorClasses.BG_TRAVEL;
                case EventTypeEnum.Liveaboard:
                    return ColorClasses.BG_LIVEABOARD;
                case EventTypeEnum.Klubture:
                    return ColorClasses.BG_CLUB;
                case EventTypeEnum.Other:
                    return ColorClasses.BG_OTHER;
                case EventTypeEnum.NotAnEvent:
                    return ColorClasses.BG_NOTEVENT;
                default:
                    return ColorClasses.BG_DEFAULT;
            }
        }
        public static string GetEventColor(EventTypeEnum item)
        {
            switch (item)
            {
                case EventTypeEnum.Bådtur:
                    return ColorClasses.FG_BOAT;
                case EventTypeEnum.Stranddyk:
                    return ColorClasses.FG_BEACH;
                case EventTypeEnum.Rejse:
                    return ColorClasses.FG_TRAVEL;
                case EventTypeEnum.Liveaboard:
                    return ColorClasses.FG_LIVEABOARD;
                case EventTypeEnum.Klubture:
                    return ColorClasses.FG_CLUB;
                case EventTypeEnum.Other:
                    return ColorClasses.FG_OTHER;
                case EventTypeEnum.NotAnEvent:
                    return ColorClasses.FG_NOTEVENT;
                default:
                    return ColorClasses.FG_DEFAULT;
            }
        }
        public static string GetEventIcon(EventTypeEnum item)
        {
            switch (item)
            {
                case EventTypeEnum.Bådtur:
                    return Icons.EVENTTYPE_BOAT;
                case EventTypeEnum.Stranddyk:
                    return Icons.EVENTTYPE_BEACH;
                case EventTypeEnum.Rejse:
                    return Icons.EVENTTYPE_TRAVEL;
                case EventTypeEnum.Liveaboard:
                    return Icons.EVENTTYPE_LIVEABOARD;
                case EventTypeEnum.Klubture:
                    return Icons.EVENTTYPE_CLUB;
                case EventTypeEnum.Other:
                    return Icons.EVENTTYPE_OTHER;
                case EventTypeEnum.NotAnEvent:
                default:
                    return Icons.EVENTTYPE_NOT_EVENT;
            }

        }
    }
}
