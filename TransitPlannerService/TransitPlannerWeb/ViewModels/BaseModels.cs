using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransitPlannerWeb.ViewModels
{
    public enum TuszErrorCode
    {
        NO_ERROR,
        LOGIN_ERROR,
        ERROR_IN_MESSAGE,
        NO_SUCH_REPORT,
        NO_REPORTS,
        NO_SUCH_ROUTE,
        NO_PLAN_CREATED,
        NO_ROUTES,
        NO_ROUTE_SELECTED,
        CORE_SERVICE_IS_UNREACHABLE,
        CORE_SERVICE_FAULT,
        UNKNOWN_ERROR
    }

    public class BaseModel
    {
        public TuszErrorCode ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class AdminBaseModel : BaseModel
    {
        public string Username { get; set; }
    }

    public class RouteBadgeModel
    {
        public int FontSize { get; set; }
        public int BadgeSize { get; set; }
        public string BadgeLabel { get; set; }
        public string BadgeBackgroundColor { get; set; }
        public string BadgeLabelColor { get; set; }
    }

    // javascriptben a datetime 0-tól számozza a honapokat
    public class JsDateTime
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
    }
}