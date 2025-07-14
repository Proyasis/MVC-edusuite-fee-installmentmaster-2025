using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CITS.EduSuite.UI.Controllers
{
    public class ESSLController : Controller
    {
        private IESSLAttendanceService ESSLAttendanceService;

        public ESSLController(IESSLAttendanceService objESSLAttendanceService)
        {
            this.ESSLAttendanceService = objESSLAttendanceService;           
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.ESSLStudnts, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult ESSLStudentList()
        {
            ESSLStudentsDetailsViewModel objViewModel = new ESSLStudentsDetailsViewModel();

          
            return View(objViewModel);
        }

        [HttpGet]
        public JsonResult GetESSLStudent(string EmployeeCode, bool IsConnected, string sidx, string sord, int page, int rows)
        {
            long TotalRecords = 0;
            List<ESSLStudentsDetailsViewModel> applicationList = new List<ESSLStudentsDetailsViewModel>();
            ESSLStudentsDetailsViewModel objViewModel = new ESSLStudentsDetailsViewModel();

            objViewModel.EmployeeCode = EmployeeCode;
            objViewModel.IsConnected = IsConnected;
            objViewModel.PageIndex = page;
            objViewModel.PageSize = rows;
            objViewModel.SortBy = sidx;
            objViewModel.SortOrder = sord;

            applicationList = ESSLAttendanceService.GetESSLStudents(objViewModel, out TotalRecords);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            var totalPages = (int)Math.Ceiling((float)TotalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = TotalRecords,
                rows = applicationList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
    }
}