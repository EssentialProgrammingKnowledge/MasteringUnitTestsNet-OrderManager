using System.Globalization;

namespace OrderManager.UI
{
    public static class Extensions
    {
        public static string ToCurrencyString(this decimal value)
        {
            return value.ToString("C", new CultureInfo("pl-PL"));
        }

        public static DateTime ToLocalDateTime(this DateTime value)
        {
            return value.Kind == DateTimeKind.Utc ? value.ToLocalTime() : value;
        }
    }
}
