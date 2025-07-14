using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public static class DateTimeUTC
    {
        private static TimeZoneInfo timeZoneInfo
        {
            get
            {
                try { return System.Configuration.ConfigurationManager.AppSettings["TimeZone"] != null && System.Configuration.ConfigurationManager.AppSettings["TimeZone"] != "" ? TimeZoneInfo.FindSystemTimeZoneById(System.Configuration.ConfigurationManager.AppSettings["TimeZone"]) : TimeZoneInfo.Local; }
                catch(Exception ex) { return TimeZoneInfo.Local; }
            }
        }
        public static DateTime Now { get { return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo); } }
        public static DateTime Tomorrow { get { return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo).AddDays(1); } }
    }
}
