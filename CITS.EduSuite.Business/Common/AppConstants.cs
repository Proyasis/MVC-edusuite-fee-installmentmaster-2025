using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Common
{
    public static class AppConstants
    {
        public static class Common
        {
            public const string SUCCESS = "Success";
            public const string FAILED = "Failed";
        }

        public static class ErrorResourceName
        {
            public static string ACCESSDENIED = "AccessDenied";
        }


        public static class ConfigSetting
        {
            public const string SYSTEM_TIMEOUT = "System Automatic Timeout Minutes";
            public const string SYSTEM_TIMEOUT_WARNING = "System Automatic Timeout Warning Minutes";
        }

        public static class UserStatus
        {
            public const string ACTIVE = "Active";
            public const string INACTIVE = "Inactive";
        }
    }
}
