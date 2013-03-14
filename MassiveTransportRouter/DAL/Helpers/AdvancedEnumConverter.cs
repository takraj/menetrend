using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MTR.DataAccess.Helpers
{
    public class AdvancedEnumConverter<T> : DefaultTypeConverter
    {
        /// <summary>
        /// Converts the string to an object.
        /// </summary>
        /// <param name="culture">The culture used when converting.</param>
        /// <param name="text">The string to convert to an object.</param>
        /// <returns>The object created from the string.</returns>
        public override object ConvertFromString(System.Globalization.CultureInfo culture, string text)
        {
            try
            {
                if (text == null || text == "")
                {
                    text = Enum.GetNames(typeof(T)).FirstOrDefault();
                }
                return Enum.Parse(typeof(T), text, true);
            }
            catch
            {
                return base.ConvertFromString(culture, text);
            }
        }

        /// <summary>
        /// Determines whether this instance [can convert from] the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// <c>true</c> if this instance [can convert from] the specified type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvertFrom(Type type)
        {
            // We only care about strings.
            return type == typeof(string);
        }
    }
}