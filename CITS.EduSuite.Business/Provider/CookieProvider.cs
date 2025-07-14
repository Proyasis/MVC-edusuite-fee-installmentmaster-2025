using CITS.EduSuite.Business.Models.Security;
using CITS.EduSuite.Business.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Provider
{
    public class CookieProvider : ICookieAuthentationProvider
    {
        private Int64 UserKey { get; set; }
        private string Name { get; set; }

        private Int16 UserTypeKey { get; set; }

        //public HttpCookie CreateCookie(string userName, Int32 userKey)
        //{
        //    this.UserKey = userKey;
        //    this.Name = userName;

        //    var authTicket = FormsAuthentication.Encrypt(
        //        new FormsAuthenticationTicket(
        //            1,
        //            userName,
        //            DateTimeUTC.Now,
        //            DateTimeUTC.Now.AddDays(15),
        //            true,
        //                new CITSEduSuitePrincipalData()
        //                {
        //                    UserKey = this.UserKey,
        //                    Name = this.Name
        //                }.ToString().Serialize()
        //            )
        //        );

        //    var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, authTicket);

        //    return cookie;
        //}
        public HttpCookie CreateCookie(CITSEduSuitePrincipalData data)
        {
            this.UserKey = data.UserKey;
            this.Name = data.Name;
            this.UserTypeKey = data.RoleKey;
            var authTicket = FormsAuthentication.Encrypt(
                new FormsAuthenticationTicket(
                    1,
                    data.Name,
                    DateTimeUTC.Now,
                    DateTimeUTC.Now.AddDays(15),
                    true,
                       data.Serialize()
                    )
                );
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, authTicket);
            return cookie;
        }
        public void DeleteCookie(HttpRequestBase request, HttpResponseBase Response)
        {
            if (request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                HttpCookie formsCookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
                formsCookie.Expires = DateTimeUTC.Now.AddDays(-1);
                Response.Cookies.Add(formsCookie);
            }
        }

        public CITSEduSuitePrincipalData GetCookie(HttpCookie coockie)
        {

            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(coockie.Value);
            CITSEduSuitePrincipalData userData = ticket.UserData.Deserialize<CITSEduSuitePrincipalData>();
            return userData;
        }
    }
}
