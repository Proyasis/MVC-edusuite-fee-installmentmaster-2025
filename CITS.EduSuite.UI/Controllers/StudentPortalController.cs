using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.UI.Controllers
{
    public class StudentPortalController : BaseController
    {
        // GET: StudentPortal

        public IStudentPortalService StudentPortalServeice;
        public StudentPortalController(IStudentPortalService objStudentPortalService)
        {
            this.StudentPortalServeice = objStudentPortalService;
        }


        #region Student Login
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        [HttpGet]
        public ActionResult Index(string returnUrl)
        {

            var model = new LoginModel();
            model.ReturnUrl = returnUrl;
            model.RoleKey = DbConstants.Role.Students;
            return View(model);
        }
        #endregion
        public ActionResult StudentDashBoard()
        {

            return View();
        }

        [HttpGet]

        public JsonResult GetPersonalDetails(long ApplicationKey)
        {
            return Json(StudentPortalServeice.GetPersonalDetails(ApplicationKey), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]

        public JsonResult GetAttendanceCount(long ApplicationKey, long? ClassDetailsKey)
        {
            return Json(StudentPortalServeice.GetAttendanceDetails(ApplicationKey, ClassDetailsKey ?? 0), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]

        public JsonResult GetExamResults(long ApplicationKey)
        {
            return Json(StudentPortalServeice.GetAllExamResults(ApplicationKey), JsonRequestBehavior.AllowGet);
        }

      

        [HttpGet]
        public JsonResult GetUnitExamResults(long ApplicationKey,long SubjectKey)
        {
            return Json(StudentPortalServeice.GetUnitExamResults(ApplicationKey,SubjectKey), JsonRequestBehavior.AllowGet);
        }

        //[HttpGet]
        //public JsonResult GetNotification(long ApplicationKey, long ClassDetailsKey)
        //{
        //    return Json(StudentPortalServeice.GetNotification(ApplicationKey, ClassDetailsKey), JsonRequestBehavior.AllowGet);
        //}


        [HttpGet]
        public ActionResult StudentFee()
        {

            return View();
        }

        [HttpGet]
        public JsonResult BindTotalFeeDetails(long ApplicationKey, long ClassDetailsKey)
        {
            return Json(StudentPortalServeice.BindTotalFeeDetails(ApplicationKey, ClassDetailsKey), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult BindInstallmentFeeDetails(long ApplicationKey, long ClassDetailsKey)
        {
            return Json(StudentPortalServeice.BindInstallmentFeeDetails(ApplicationKey, ClassDetailsKey), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult BindFeePaymentDetails(long ApplicationKey, long ClassDetailsKey)
        {
            return Json(StudentPortalServeice.BindFeePaymentDetails(ApplicationKey, ClassDetailsKey), JsonRequestBehavior.AllowGet);
        }

    }
}