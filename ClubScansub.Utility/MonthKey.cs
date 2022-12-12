using ClubScansub.Utility.Extensions;
using Nager.Date;
using Nager.Date.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClubScansub.Utility
{
    public class MonthKey
    {
        private readonly DateTime date;
        private MonthKey nextKey;
        private MonthKey prevKey;
        public MonthKey()
            : this(DateTime.Now)
        {
        }


        public MonthKey(int monthKey)
            : this(DateTime.Now.FromMonthKey(monthKey))
        {
        }
        public MonthKey(DateTime date)
        {
            this.date = date.FirstDateOfMonth();
            Id = date.ToMonthKey();

        }

        public int Id { get; set; }
        public MonthKey GetNext()
        {
            if (nextKey == null)
            {
                nextKey = new MonthKey(date.AddMonths(1));
            }
            return nextKey;
        }
        public MonthKey GetPrevious()
        {
            if (prevKey == null)
            {
                prevKey = new MonthKey(date.AddMonths(-1));
            }
            return prevKey;
        }

        public override string ToString()
        {
            return date.ToString("MMMM yyyy");
        }

        public DateTime FirstDate
        {
            get
            {
                return date.Date;
            }
        }

        public DateTime LastDate
        {
            get
            {
                return date.AddMonths(1).AddDays(-1);
            }
        }

        public int NumberOfDays
        { 
            get
            {
                return LastDate.Day;
            }
        }
        public int Month => date.Month;
        public int Year => date.Year;

        public DateTime CalendarStartDate
        {
            get
            {

                return date.AddDays((int)date.DayOfWeek == 0 ? -6 : -(int)date.DayOfWeek + 1);
            }
        }

        public DateTime CalendarEndDate
        {
            get
            {
                return LastDate.AddDays((int)LastDate.DayOfWeek == 0 ? 0 : 7 - (int)LastDate.DayOfWeek);
            }
        }

        public int TotalCalendarDays
        {
            get 
            {
                return (int)(CalendarEndDate - CalendarStartDate).TotalDays + 1;
            }

        }
        public IEnumerable<PublicHoliday> GetHolidays()
        {
            return DateSystem.GetPublicHoliday(FirstDate, LastDate, CountryCode.DK);
        }

    }
}
