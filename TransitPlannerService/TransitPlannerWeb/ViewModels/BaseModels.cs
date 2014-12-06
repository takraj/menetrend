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
}