using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace TUSZ.WebApp.Models
{
    public class Utility
    {
        public static void SetupCulture()
        {
            var newCulture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            newCulture.NumberFormat.CurrencyDecimalSeparator = ".";
            newCulture.NumberFormat.CurrencyGroupSeparator = "";

            newCulture.NumberFormat.NumberDecimalSeparator = ".";
            newCulture.NumberFormat.NumberGroupSeparator = "";

            newCulture.NumberFormat.PercentDecimalSeparator = ".";
            newCulture.NumberFormat.PercentGroupSeparator = "";

            System.Threading.Thread.CurrentThread.CurrentCulture = newCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = newCulture;
        }
    }
}