using CITS.EduSuite.Business.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Common;
using System.Web;
using System.Web.Security;
using CITS.EduSuite.Business.Models.Security;
using System.Threading;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public static class CommonUtilities
    {
        public static CITSEduSuitePrincipal GetLoggedUser()
        {
            return (Thread.CurrentPrincipal as CITSEduSuitePrincipal);
        }
        public static string GetYearDescriptionByCodeDetails(int Duration, int Year, short AcademicTermKey)
        {
            Duration = Convert.ToInt32(AcademicTermKey == DbConstants.AcademicTerm.Semester ? Duration / 6 : Duration / 12);
            string YearText = AcademicTermKey == DbConstants.AcademicTerm.Semester ? " Semester" : " Year";
            string result;
            switch (Year)
            {
                case 1:
                    if (Duration < 1)
                    {
                        result = "Short Term";
                        break;
                    }
                    else
                    {
                        result = Year + "st " + YearText;
                        break;
                    }
                case 2:
                    result = Year + "nd " + YearText;
                    break;
                case 3:
                    result = Year + "rd " + YearText;
                    break;
                default:
                    result = Year + "th " + YearText;
                    break;


            }
            return result;
        }

        //Old Query Commented By Khaleefa on 29 Jun 2019
        public static string GetYearDescriptionByCode(int Year, short AcademicTermKey)
        {
            string YearText = AcademicTermKey == DbConstants.AcademicTerm.Semester ? " Semester" : " Year";
            string result;
            switch (Year)
            {
                case 0:
                    result = "Short Term";
                    break;
                case 1:
                    result = Year + "st " + YearText;
                    break;
                case 2:
                    result = Year + "nd " + YearText;
                    break;
                case 3:
                    result = Year + "rd " + YearText;
                    break;
                default:
                    result = Year + "th " + YearText;
                    break;


            }
            return result;
        }

        public static long GetUserKey()
        {
            try
            {
                return (Thread.CurrentPrincipal as CITSEduSuitePrincipal).UserKey;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }
    }
}
