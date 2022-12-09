using System;
using System.Collections.Generic;
using System.Text;

namespace ClubScansub.Utility.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime FirstDateOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month,1);
        }

        public static DateTime NextMonthFirstDateOfMonth(this DateTime date)
        {
            var d = date.AddMonths(1);

            return new DateTime(d.Year, d.Month, 1);
        }

        public static DateTime PreviousMonthFirstDateOfMonth(this DateTime date)
        {
            var d = date.AddMonths(-1);
            return new DateTime(d.Year, d.Month, 1);
        }

        public static int ToMonthKey(this DateTime date)
        {
            return date.Year * 100 + date.Month;
        }

        public static DateTime FromMonthKey(this DateTime date, int MonthKey)
        {
            int y = MonthKey / 100;
            int m = MonthKey % 100;

            return new DateTime(y,m,1); 
        }
    }
}
