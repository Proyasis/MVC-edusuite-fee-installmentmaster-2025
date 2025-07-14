using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CITS.EduSuite.Business.Models.Security;

namespace CITS.EduSuite.Business.Provider
{
    public interface ICookieAuthentationProvider
    {
        
        HttpCookie CreateCookie(CITSEduSuitePrincipalData data);
        void DeleteCookie(HttpRequestBase request, HttpResponseBase Response);
        CITSEduSuitePrincipalData GetCookie(HttpCookie coockie);
    }
}
