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



namespace CITS.EduSuite.UI.Controllers
{
    [MessagesActionFilter]
    public class ExamScheduleController : BaseController
    {
        // GET: ExamScheduleList        
        private IExamScheduleService examScheduleService;
        //private IUniversityProgramService UniversityProgramService;

        public ExamScheduleController(IExamScheduleService objExamScheduleService)
        {
            this.examScheduleService = objExamScheduleService;
        }

        public ActionResult ExamScheduleList()
        {
            //ExamScheduleViewModel model = new ExamScheduleViewModel();
            //examScheduleService.FillDropDown(model);
            return View();
        }

        [HttpPost]
        public JsonResult GetExamScheduleListdetails(string SearchText)
        {
            int page = 1; int rows = 15;
            List<ExamScheduleViewModel> ExamScheduleList = new List<ExamScheduleViewModel>();
            ExamScheduleList = examScheduleService.GetExamSchedule(SearchText);
            int pageindex = Convert.ToInt32(page) - 1;
            int pagesize = rows;
            int totalrecords = ExamScheduleList.Count();
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)rows);

            var jsonData = new
            {
                total = totalpage,
                page,
                records = totalrecords,
                rows = ExamScheduleList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddEditExamSchedule(long? Id)
        {

            ExamScheduleViewModel model = new ExamScheduleViewModel();
            model.RowKey = Id ?? 0;
            model = examScheduleService.GetExamScheduleById(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult GetExamSchedule(ExamScheduleViewModel model)
        {
            examScheduleService.FillExamDetailsViewModel(model);

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult AddEditExamSchedule(ExamScheduleViewModel model)
        {
            if (ModelState.IsValid)
            {

                if (model.RowKey != 0)
                {
                    model = examScheduleService.UpdateExamSchedule(model);
                }
                else
                {
                    model = examScheduleService.CreateExamSchedule(model);
                }

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    return Json(model);

                }


                model.Message = "";
                return Json(model);
            }
            foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            {
            }

            model.Message = CITSEduSuiteResourceManager.GetApplicationString(AppConstants.Common.FAILED);
            return Json(model);
        }


        public JsonResult FillCourse(short key)
        {
            ExamScheduleViewModel model = new ExamScheduleViewModel();
            model.CourseTypeKey = key;
            model = examScheduleService.FillCourse(model);
            return Json(model.Courses, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FillUniversity(long key, short AcademicTermKey)
        {
            ExamScheduleViewModel model = new ExamScheduleViewModel();
            model.AcademicTermKey = AcademicTermKey;
            model.CourseKey = key;
            model = examScheduleService.FillUniversity(model);
            return Json(model.Universities, JsonRequestBehavior.AllowGet);
        }


        public JsonResult FillCourseYear(long key, short AcademicTermKey)
        {
            ExamScheduleViewModel model = new ExamScheduleViewModel();
            model.AcademicTermKey = AcademicTermKey;
            model.CourseKey = key;
            model = examScheduleService.FillCourseYear(model);
            return Json(model.CourseYears, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FillBatch(short BranchKey, short? UniversityMasterKey, long? CourseKey)
        {
            ExamScheduleViewModel model = new ExamScheduleViewModel();
            model.BranchKey = BranchKey;
            model.UniversityMasterKey = UniversityMasterKey ?? 0;
            model.CourseKey = CourseKey ?? 0;
            model = examScheduleService.FillBatch(model);
            return Json(model.Batches, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult FillSubjects(short? UniversityMasterKey, long? CourseKey, short? AcademicTermKey, short? CourseYear)
        {
            ExamScheduleViewModel model = new ExamScheduleViewModel();
            model.UniversityMasterKey = UniversityMasterKey ?? 0;
            model.CourseKey = CourseKey ?? 0;
            model.AcademicTermKey = AcademicTermKey ?? 0;
            model.CourseYear = CourseYear ?? 0;
            model = examScheduleService.FillSubjects(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult ResetExamSchedule(long RowKey, long ExamKey)
        {
            ExamScheduleViewModel objviewModel = new ExamScheduleViewModel();
            //objviewModel.StudentDivisionAllocationKey = RowKey ?? 0;
            //objviewModel.ApplicationKey = ApplicationKey ?? 0;
            try
            {
                objviewModel = examScheduleService.ResetExamSchedule(RowKey, ExamKey);
            }
            catch (Exception)
            {
                objviewModel.Message = CITSEduSuiteResourceManager.GetApplicationString(AppConstants.Common.FAILED);
            }
            return Json(objviewModel);

        }

        [HttpPost]
        public ActionResult DeleteExamSchedule(long? RowKey)
        {
            ExamScheduleViewModel model = new ExamScheduleViewModel();
            model.RowKey = RowKey ?? 0;
            try
            {
                model = examScheduleService.DeleteExamSchedule(model);
            }
            catch (Exception)
            {
                model.Message = CITSEduSuiteResourceManager.GetApplicationString(AppConstants.Common.FAILED);
            }
            return Json(model);
        }


        #region old Queries
        //[HttpPost]
        //public ActionResult ExamScheduleList(ExamScheduleViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        model = examScheduleService.CreateExamScheduleSchedule(model);
        //        if (model.Message != AppConstants.Common.SUCCESS)
        //        {
        //            ModelState.AddModelError("error_msg", model.Message);
        //            Toastr.AddToastMessage("Sucess", model.Message, ToastType.Error);
        //        }
        //        else
        //        {
        //            Toastr.AddToastMessage("Sucess", model.Message, ToastType.Success);
        //            return View("ExamScheduleList", model);
        //        }
        //    }
        //    else
        //    {
        //        var errors = ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList();
        //    }
        //    model.Message = CITSEduSuiteResourceManager.GetApplicationString(AppConstants.Common.FAILED);
        //    return View("ExamScheduleList", model);
        //}
        //[HttpPost]
        //public ActionResult GetExamScheduleStudentsList(ExamScheduleViewModel model)
        //{           
        //    model.ExamScheduleList = examScheduleService.GetExamScheduleStudentsList(model);
        //    ViewBag.TotalRecords = model.TotalRecords;
        //    ViewBag.PageIndex = model.PageIndex;

        //    return PartialView(model.ExamScheduleList);
        //}
        //[HttpGet]
        //public JsonResult GetCourseTypeBySyllabus(short AcademicTermKey)
        //{
        //    ExamScheduleViewModel objViewModel = new ExamScheduleViewModel();
        //    objViewModel.SearchAcademicTermKey =AcademicTermKey;
        //    objViewModel = examScheduleService.GetCourseTypeBySyllabus(objViewModel);
        //    return Json(objViewModel, JsonRequestBehavior.AllowGet);
        //}
        //[HttpGet]
        //public JsonResult GetCourseByCourseType(byte? CourseTypeKey, short AcademicTermKey)
        //{
        //    ExamScheduleViewModel objViewModel = new ExamScheduleViewModel();
        //    objViewModel.SearchCourseTypeKey = CourseTypeKey??0;
        //    objViewModel.SearchAcademicTermKey =AcademicTermKey;
        //    objViewModel = examScheduleService.GetCourseByCourseType(objViewModel);
        //    return Json(objViewModel, JsonRequestBehavior.AllowGet);
        //}
        //[HttpGet]
        //public JsonResult GetUniversityByCourse(short? CourseKey)
        //{
        //    ExamScheduleViewModel objViewModel = new ExamScheduleViewModel();
        //    objViewModel.SearchCourseKey = CourseKey??0;
        //    objViewModel = examScheduleService.GetUniversityByCourse(objViewModel);
        //    return Json(objViewModel, JsonRequestBehavior.AllowGet);
        //}
        //[HttpGet]
        //public JsonResult GetYearsBySyllabus(short AcademicTermKey,int? CourseKey)
        //{
        //    ExamScheduleViewModel objViewModel = new ExamScheduleViewModel();
        //    objViewModel.SearchAcademicTermKey =AcademicTermKey;
        //    objViewModel.SearchCourseKey = CourseKey??0;
        //    objViewModel = examScheduleService.GetYears(objViewModel);
        //    return Json(objViewModel, JsonRequestBehavior.AllowGet);
        //}
        //[HttpGet]
        //public JsonResult GetExamSubjects(int? CourseKey, short? UniversityKey, int? ExamYearKey)
        //{            
        //    ExamScheduleViewModel objViewModel = new ExamScheduleViewModel();
        //    objViewModel.SearchCourseKey = CourseKey??0;
        //    objViewModel.SearchUniversityKey = UniversityKey??0;
        //    objViewModel.SearchExamYearKey=ExamYearKey??0;
        //    objViewModel = examScheduleService.GetExamSubjects(objViewModel);
        //    return Json(objViewModel, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public JsonResult DeleteExamSchedule(Int16 id)
        //{
        //    ExamScheduleViewModel objViewModel = new ExamScheduleViewModel();
        //    objViewModel.RowKey = id;
        //    try
        //    {
        //        objViewModel = examScheduleService.DeleteExamSchedule(objViewModel);
        //    }
        //    catch (Exception)
        //    {
        //        objViewModel.Message = CITSEduSuiteResourceManager.GetApplicationString(AppConstants.Common.FAILED);
        //    }
        //    return Json(objViewModel, JsonRequestBehavior.AllowGet);
        //}
        #endregion old Queries

    }
}