using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace CITS.EduSuite.UI.Controllers
{
    
    public class ESSLStudentsController : ApiController
    {

        [System.Web.Http.HttpGet]
        public HttpResponseMessage Get()
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> Post(List<ESSLStudentsViewModel> StudentsList)
        {
            ESSLStudentsViewModel model = new ESSLStudentsViewModel();
            IESSLAttendanceService ESSLAttendanceService = new ESSLAttendanceService();
            try
            {
                model = ESSLAttendanceService.GetEmployeeDetails(StudentsList);
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