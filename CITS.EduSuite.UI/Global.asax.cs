using CITS.EduSuite.Business.Models.Security;
using CITS.EduSuite.Business.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using CITS.EduSuite.Business.Services;
using System.Web.Http;

namespace CITS.EduSuite.UI
{
    public class MvcApplication : System.Web.HttpApplication
    {

        protected void Application_Start()
        {
            UnityConfig.RegisterComponents();

            //AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            log4netManager.Configure();
            SettingsMapping settings = new SettingsMapping();
           
          
            settings.SyncOrderSettings();
        }


        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            try
            {

                CookieProvider cookieProvider = new CookieProvider();
                HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authCookie != null)
                {
                    CITSEduSuitePrincipalData userData = cookieProvider.GetCookie(authCookie);
                    CITSEduSuitePrincipal newData = new CITSEduSuitePrincipal(userData.UserKey.ToString());
                    newData.UserKey = userData.UserKey;
                    newData.Name = userData.Name;
                    newData.CompanyKey = userData.CompanyKey;
                    newData.CompanyName = userData.CompanyName;
                    newData.BranchKey = userData.BranchKey;
                    newData.RoleKey = userData.RoleKey;
                    newData.IsTeacher = userData.IsTeacher;
                    newData.Photo = userData.Photo;
                    newData.CompanyLogo = userData.CompanyLogo;
                    newData.ApplicationKey = userData.ApplicationKey;
                    newData.EmployeeKey = userData.EmployeeKey;

                    newData.AllowAdmissionToAccoount = userData.AllowAdmissionToAccoount;
                    newData.AllowCenterShare = userData.AllowCenterShare;
                    newData.AllowSplitCostOfService = userData.AllowSplitCostOfService;
                    newData.AllowUniversityAccountHead = userData.AllowUniversityAccountHead;
                    newData.EducationTypeKey = userData.EducationTypeKey;

                    HttpContext.Current.User = newData;
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
