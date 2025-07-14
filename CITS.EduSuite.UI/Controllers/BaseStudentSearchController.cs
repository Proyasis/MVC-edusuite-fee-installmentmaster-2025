using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Provider;
using System.Web.Security;
using CITS.EduSuite.Business.Models.Security;


namespace CITS.EduSuite.UI.Controllers
{
    public class BaseStudentSearchController : BaseController
    {
          // GET: StudentsSummaryReport
        public IBaseStudentSearchService studentsSearchService;
        private ISharedService sharedService;
        public BaseStudentSearchController(IBaseStudentSearchService ObjstudentsSearchService,
            ISharedService objSharedService)
        {
            this.studentsSearchService = ObjstudentsSearchService;
            this.sharedService = objSharedService;
        }

        // GET: BaseStudentSearch
        public ActionResult Index()
        {
            BaseSearchStudentsViewModel model = new BaseSearchStudentsViewModel();
            
            studentsSearchService.FillDropDownLists(model);

            ViewBag.ShowAcademicTerm = sharedService.ShowAcademicTerm();
            ViewBag.ShowUniversity = sharedService.ShowUniversity();
            return PartialView(model);    
        }


        [HttpPost]
        public JsonResult GetCourseTypeByAcademicTerm(BaseSearchStudentsViewModel model)
        {
            studentsSearchService.FillCourseTypes(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetCourseByCourseType(BaseSearchStudentsViewModel model)
        {

            studentsSearchService.FillCourse(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetUniversityByCourse(BaseSearchStudentsViewModel model)
        {
            studentsSearchService.FillUniversityMasters(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetYearsByAcademicTermKey(BaseSearchStudentsViewModel model)
        {
            studentsSearchService.FillYears(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

       

       
    }
}