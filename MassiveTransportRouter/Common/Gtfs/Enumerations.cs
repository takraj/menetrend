namespace MTR.Common.Gtfs
{
    public enum E_WheelchairSupport
    {
        NO_INFO = 0,
        YES = 1,
        NO = 2
    }

    public enum E_RouteType
    {
        TRAM = 0,
        SUBWAY = 1,
        RAIL = 2,
        BUS = 3,
        FERRY = 4,
        STREET_LEVEL_CABLE_CAR = 5,
        GONDOLA_LIFT = 6,
        FUNICULAR_CLIFF_RAIL = 7
    }

    public enum E_TripDirection
    {
        FORWARD = 0,
        BACKWARD = 1
    }

    public enum E_LocationType
    {
        STOP = 0,
        STATION = 1
    }

    public enum E_CalendarExceptionType
    {
        ADDED = 1,
        REMOVED = 2
    }
}
