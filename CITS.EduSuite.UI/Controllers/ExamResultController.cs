using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Provider;
using System.Web.Security;
using CITS.EduSuite.Business.Models.Security;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI.Controllers
{
    public class ExamResultController : BaseController
    {
        public IExamResultService ExamResultService;
        private ISelectListService selectListService;
        public ExamResultController(IExamResultService objExamResultService, ISelectListService objselectListService)
        {
            this.ExamResultService = objExamResultService;
            this.selectListService = objselectListService;
        }

        [HttpGet]
        public ActionResult ExamResultList()
        {
            ApplicationViewModel model = new ApplicationViewModel();

            model.Branches = selectListService.FillBranches();
            model.Batches = selectListService.FillSearchBatch(model.BranchKey);
            model.Courses = selectListService.FillSearchCourse(model.BranchKey);
            model.Universities = selectListService.FillSearchUniversity(model.BranchKey);
            return View(model);
        }

        [HttpGet]
        public JsonResult GetExamResult(string SearchText, short? BranchKey, long? CourseKey, short? UniversityKey, short? BatchKey, string sidx, string sord, int page, int rows)
        {

            long TotalRecords = 0;
            List<ExamResultViewModel> ExamResultList = new List<ExamResultViewModel>();
            ExamResultViewModel objViewModel = new ExamResultViewModel();

            objViewModel.SearchText = SearchText ?? "";
            objViewModel.BranchKey = BranchKey ?? 0;
            objViewModel.BatchKey = BatchKey ?? 0;
            objViewModel.CourseKey = CourseKey ?? 0;
            objViewModel.UniversityMasterKey = UniversityKey ?? 0;
            objViewModel.PageIndex = page;
            objViewModel.PageSize = rows;
            objViewModel.SortBy = sidx;
            objViewModel.SortOrder = sord;
            ExamResultList = ExamResultService.GetExamResult(objViewModel, out TotalRecords);
            int pageindex = Convert.ToInt32(page) - 1;
            int pagesize = rows;
            var totalpage = (int)Math.Ceiling((float)TotalRecords / (float)rows);
            var jsonData = new
            {
                total = totalpage,
                page,
                records = TotalRecords,
                rows = ExamResultList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]

        public ActionResult GetAllSubjectDetails(short? BranchKey, short? AcademicTermKey, short? CourseKey, short? UniversityMasterKey, short? BatchKey, short? ExamTermKey, short? ExamYear)
        {
            ExamResultViewModel objViewModel = new ExamResultViewModel();

            objViewModel.BranchKey = BranchKey;
            objViewModel.AcademicTermKey = AcademicTermKey;
            objViewModel.CourseKey = CourseKey;
            objViewModel.UniversityMasterKey = UniversityMasterKey;
            objViewModel.BatchKey = BatchKey;
            objViewModel.ExamTermKey = ExamTermKey;
            objViewModel.ExamYear = ExamYear;

            return View(objViewModel);
        }

        [HttpGet]
        public ActionResult GetSubjectDetils(ExamResultViewModel model)
        {
            long TotalRecords = 0;
            int page = 1, rows = 10;

            ExamResultViewModel objViewModel = new ExamResultViewModel();

            objViewModel.BranchKey = model.BranchKey;
            objViewModel.AcademicTermKey = model.AcademicTermKey;
            objViewModel.CourseKey = model.CourseKey;
            objViewModel.UniversityMasterKey = model.UniversityMasterKey;
            objViewModel.BatchKey = model.BatchKey;
            objViewModel.ExamTermKey = model.ExamTermKey;
            objViewModel.ExamYear = model.ExamYear;

            objViewModel = ExamResultService.GetExamResultDetails(objViewModel);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            var totalPages = (int)Math.Ceiling((float)TotalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = objViewModel.ExamResultSubjectDetail.Count,
                rows = objViewModel.ExamResultSubjectDetail
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult AddEditStudentsMarkList(short? BranchKey, short? AcademicTermKey, short? CourseKey, short? UniversityMasterKey, short? BatchKey, short? ExamTermKey, short? ExamYear, long? SubjectKey)
        {
            ExamResultViewModel objViewModel = new ExamResultViewModel();


            objViewModel.BranchKey = BranchKey;
            objViewModel.AcademicTermKey = AcademicTermKey;
            objViewModel.CourseKey = CourseKey;
            objViewModel.UniversityMasterKey = UniversityMasterKey;
            objViewModel.BatchKey = BatchKey;
            objViewModel.ExamTermKey = ExamTermKey;
            objViewModel.ExamYear = ExamYear;
            objViewModel.SubjectKey = SubjectKey;

            objViewModel = ExamResultService.StudentMarkDetils(objViewModel);

            return PartialView(objViewModel);


        }

        [HttpPost]

        public ActionResult AddEditStudentsMarkList(ExamResultViewModel model)
        {
            if (ModelState.IsValid)
            {

                model = ExamResultService.UpdateExamResult(model);
                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    return Json(model);
                }

                return Json(model);
            }
            foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            {
            }
            return Json(model);

        }
        private void GetUserKey(ExamResultViewModel model)
        {
            CookieProvider cookieProvider = new CookieProvider();
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                CITSEduSuitePrincipalData userData = cookieProvider.GetCookie(authCookie);
                model.UserKey = userData.UserKey;
                //model.RoleKey = userData.RoleKey;
            }
        }


        [HttpPost]

        public ActionResult DeleteExamResult(long? ExamScheduleKey, long? SubjectKey)
        {
            ExamResultViewModel model = new ExamResultViewModel();

            try
            {
                model = ExamResultService.DeleteExamResult(ExamScheduleKey, SubjectKey);
            }
            catch (Exception)
            {
                model.Message = EduSuiteUIResources.Failed;
            }
            return Json(model);
        }
        [HttpPost]

        public ActionResult ResetExamResult(long Id)
        {
            ExamResultViewModel model = new ExamResultViewModel();

            try
            {
                model = ExamResultService.ResetExamResult(Id);
            }
            catch (Exception)
            {
                model.Message = EduSuiteUIResources.Failed;
            }
            return Json(model);
        }


        [HttpGet]
        public JsonResult GetApplications(string ApplicantName, string MobileNumber, short? BranchKey, long? CourseKey, short? UniversityKey, short? BatchKey, string sidx, string sord, int page, int rows)
        {

            long TotalRecords = 0;
            List<ApplicationViewModel> applicationList = new List<ApplicationViewModel>();
            ApplicationViewModel objViewModel = new ApplicationViewModel();

            objViewModel.ApplicantName = ApplicantName ?? "";
            objViewModel.MobileNumber = MobileNumber ?? "";
            objViewModel.BranchKey = BranchKey ?? 0;
            objViewModel.BatchKey = BatchKey ?? 0;
            objViewModel.CourseKey = CourseKey ?? 0;
            objViewModel.UniversityKey = UniversityKey ?? 0;
            objViewModel.PageIndex = page;
            objViewModel.PageSize = rows;
            objViewModel.SortBy = sidx;
            objViewModel.SortOrder = sord;

            applicationList = ExamResultService.GetApplications(objViewModel, out TotalRecords);

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


        [HttpGet]
        public ActionResult AddEditStudentsMarkListIndividual(long? id)
        {
            ExamResultViewModel objViewModel = new ExamResultViewModel();

            objViewModel.ApplicationKey = id;

            objViewModel = ExamResultService.StudentMarkDetilsByIndividual(objViewModel);

            return PartialView(objViewModel);


        }

        [HttpGet]
        public JsonResult FillBatch(short? BranchKey)
        {
            ExamResultViewModel model = new ExamResultViewModel();
            model.BranchKey = BranchKey ?? 0;
            model.Batches = selectListService.FillBatches(model.BranchKey);
            return Json(model.Batches, JsonRequestBehavior.AllowGet);
        }
    }
}