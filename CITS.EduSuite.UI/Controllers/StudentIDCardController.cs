using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Provider;
using CITS.EduSuite.Business.Models.Security;
using System.Web.Security;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI.Controllers
{
    [MessagesActionFilter]
    public class StudentIDCardController : BaseController
    {
        // GET: StudentIDCard


        private IStudentIDCardService studentIDCardService;
        //private IUniversityProgramService UniversityProgramService;
        private ISharedService sharedService;
        public StudentIDCardController(IStudentIDCardService objStudentIDCardService, ISharedService objSharedService)
        {
            this.studentIDCardService = objStudentIDCardService;
            this.sharedService = objSharedService;
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentIDCard, ActionCode = ActionConstants.View)]
        [HttpGet]
        public ActionResult StudentIDCardList()
        {
            StudentIDCardViewModels model = new StudentIDCardViewModels();
            studentIDCardService.FillDrodownLists(model);
            ViewBag.ShowAcademicTerm = sharedService.ShowAcademicTerm();
            ViewBag.ShowUniversity = sharedService.ShowUniversity();
            return View(model);
        }
        
        [HttpPost]
        public ActionResult GetStudentIDCardsList(StudentIDCardViewModels model)
        {
            model.SearchName = model.SearchName ?? "";
            model.SearchAdmissionNo = model.SearchAdmissionNo ?? "";

            List<StudentIDCardList> StudentIDCardList = new List<StudentIDCardList>();
            model.StudentIDCardList = studentIDCardService.GetStudentIDCards(model);
            foreach (StudentIDCardList objmodel in model.StudentIDCardList)
            {
                objmodel.CurrentYearText = CommonUtilities.GetYearDescriptionByCodeDetails(objmodel.CourseDuration ?? 0, objmodel.CurrentYear ?? 0, objmodel.AcademicTermKey ?? 0);
                
            }
            ViewBag.TotalRecords = model.TotalRecords;
            return PartialView(model.StudentIDCardList);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentIDCard, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult StudentIDCardList(StudentIDCardViewModels model)
        {
            if (ModelState.IsValid)
            {
                
                model = studentIDCardService.CreateStudentIDCard(model);
                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                    Toastr.AddToastMessage("Sucess", model.Message, ToastType.Error);
                }
                else
                {
                    Toastr.AddToastMessage("Sucess", model.Message, ToastType.Success);
                    ViewBag.ShowAcademicTerm = sharedService.ShowAcademicTerm();
                    ViewBag.ShowUniversity = sharedService.ShowUniversity();
                    return View("StudentIDCardList", model);
                }
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList();
            }
            model.Message = EduSuiteUIResources.Failed;
            ViewBag.ShowAcademicTerm = sharedService.ShowAcademicTerm();
            ViewBag.ShowUniversity = sharedService.ShowUniversity();
            return View("StudentIDCardList", model);
        }
        
        [HttpGet]
        public JsonResult GetCourseByCourseType(byte? CourseTypeKey)
        {
            StudentIDCardViewModels objViewModel = new StudentIDCardViewModels();
            objViewModel.SearchCourseTypeKey = CourseTypeKey ?? 0;
            objViewModel = studentIDCardService.GetCourseByCourseType(objViewModel);
            return Json(objViewModel.Courses, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetUniversityByCourse(short? CourseKey)
        {
            StudentIDCardViewModels objViewModel = new StudentIDCardViewModels();
            objViewModel.SearchCourseKey = CourseKey ?? 0;
            objViewModel = studentIDCardService.GetUniversityByCourse(objViewModel);
            return Json(objViewModel.Universities, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentIDCard, ActionCode = ActionConstants.Delete)]
        public ActionResult ResetStudentIDCard(Int64 id)
        {
            StudentIDCardViewModels objViewModel = new StudentIDCardViewModels();
            objViewModel.RowKey = id;
            try
            {
                objViewModel = studentIDCardService.ResetStudentIDCardList(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }

        [HttpPost]
        public ActionResult PrintStudentIdCard(StudentIDCardViewModels model)
        {
            model.SearchName = model.SearchName ?? "";
            model.SearchAdmissionNo = model.SearchAdmissionNo ?? "";
            List<string> ApplicationKeys = model.PrintApplicationKeys.Split(',').ToList();

            List<StudentIDCardList> StudentIDCardList = new List<StudentIDCardList>();
            model.StudentIDCardList = studentIDCardService.GetStudentIDCards(model);

            foreach (string Key in ApplicationKeys)
            {
                if (Key != "")
                {
                    StudentIDCardList StudentIDCard = new StudentIDCardList();
                    StudentIDCardList.Add(model.StudentIDCardList.Where(x => x.ApplicationKey == Convert.ToInt64(Key)).SingleOrDefault());
                }
            }

            return PartialView(StudentIDCardList);
        }

       
    }
}