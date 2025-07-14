using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace CITS.EduSuite.UI.Controllers
{

    public class ESSLAttendanceController : ApiController
    {
        [System.Web.Http.HttpGet]
        public HttpResponseMessage Get()
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> Post(List<ESSLAttendanceViewModel> AttendanceList)
        {
            ESSLAttendanceViewModel model = new ESSLAttendanceViewModel();
            IESSLAttendanceService ESSLAttendanceService = new ESSLAttendanceService();
            try
            {
                model = ESSLAttendanceService.GetAttendanceDetails(AttendanceList);
                if (model.IsSuccessful)
                {
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.Conflict);
                }
            }
            catch (Exception ex)
            {

            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}