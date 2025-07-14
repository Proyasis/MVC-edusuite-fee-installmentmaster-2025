using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CITS.EduSuite.UI.Controllers
{
    public class SelectListController : BaseController
    {
        public ISelectListService selectListService;

        public SelectListController(ISelectListService objSelectListService)
        {
            this.selectListService = objSelectListService;
        }
        [HttpGet]
        public JsonResult FillBranches()
        {
            return Json(selectListService.FillBranches(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult FillSalaryType()
        {
            return Json(selectListService.FillSalaryType(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FillClasses(short BranchKey)
        {
            return Json(selectListService.FillClasses(BranchKey), JsonRequestBehavior.AllowGet);
        }
        public JsonResult FillTeachersByClass(short BranchKey, long ClassDetailsKey)
        {
            return Json(selectListService.FillTeachersByClass(BranchKey, ClassDetailsKey), JsonRequestBehavior.AllowGet);
        }
        public JsonResult FillAllTeachers(short BranchKey)
        {
            return Json(selectListService.FillAllTeachers(BranchKey), JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public JsonResult FillAttendanceTypeByDate(DateTime Date, long ApplicationKey, short AttendanceStatusKey, short AttendanceTypeKey)
        {
            var Message = EduSuiteUIResources.ErrorAttendanceNotExists;
            List<SelectListModel> selectList = selectListService.FillAttendanceTypeByDate(Date, ApplicationKey, AttendanceStatusKey, AttendanceTypeKey);

            return Json(new
            {
                Message = Message,
                SelectList = selectList
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult AutoCompleteAdmissionNo(string query)
        {
            return Json(selectListService.AutoCompleteAdmissionNo(query), JsonRequestBehavior.AllowGet);
        }
        public JsonResult AutoCompleteEmployeeCode(string query)
        {
            return Json(selectListService.AutoCompleteEmployeeCode(query), JsonRequestBehavior.AllowGet);
        }		
		
        [HttpGet]
        public JsonResult FillAcademicTerms()
        {
            return Json(selectListService.FillAcademicTerms(), JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet]
        public JsonResult FillCourseTypesById(short? AcademicTermKey)
        {
            return Json(selectListService.FillCourseTypesById(AcademicTermKey), JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet]
        public JsonResult FillCoursesById(short? CourseTypeKey)
        {
            return Json(selectListService.FillCoursesById(CourseTypeKey), JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet]
        public JsonResult FillUniversitiesById(long? CourseKey)
        {
            return Json(selectListService.FillUniversitiesById(CourseKey), JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet]
        public JsonResult FillYearsById(short? AcademicTermKey, long? CourseKey)
        {
            return Json(selectListService.FillYearsById(AcademicTermKey ?? 0, CourseKey ?? 0), JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet]
        public JsonResult FillEmployeesById(short? BranchKey)
        {
            return Json(selectListService.FillEmployeesById(BranchKey ?? 0), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FillSearchBatch(short BranchKey)
        {
            return Json(selectListService.FillSearchBatch(BranchKey), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FillSearchCourse(short BranchKey)
        {
            return Json(selectListService.FillSearchCourse(BranchKey), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FillSearchUniversity(short BranchKey)
        {
            return Json(selectListService.FillSearchUniversity(BranchKey), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FillSearchBankAccounts(short BranchKey)
        {
            return Json(selectListService.FillSearchBankAccounts(BranchKey), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FillMenu(short MenuTypeKey)
        {
            return Json(selectListService.FillMenu(MenuTypeKey), JsonRequestBehavior.AllowGet);
        }
    }
}