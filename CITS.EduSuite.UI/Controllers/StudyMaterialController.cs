using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Provider;
using System.Web.Security;
using CITS.EduSuite.Business.Models.Security;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI.Controllers
{
    public class StudyMaterialController : BaseController
    {
        private IStudyMaterialService studyMaterialService;
        private ISelectListService selectListService;

        public StudyMaterialController(IStudyMaterialService objStudyMaterialService, ISelectListService objselectListService)
        {
            this.studyMaterialService = objStudyMaterialService;
            this.selectListService = objselectListService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudyMaterial, ActionCode = ActionConstants.View)]
        [HttpGet]
        public ActionResult StudyMaterialList()
        {
            ApplicationViewModel objViewModel = new ApplicationViewModel();
            objViewModel.Branches = selectListService.FillBranches();
            objViewModel.Batches = selectListService.FillSearchBatch(objViewModel.BranchKey);
            objViewModel.Courses = selectListService.FillSearchCourse(objViewModel.BranchKey);
            objViewModel.Universities = selectListService.FillSearchUniversity(objViewModel.BranchKey);
            return View(objViewModel);

        }

        [HttpGet]
        public JsonResult GetApplications(string ApplicantName, string MobileNumber, short? BranchKey, long? CourseKey, short? UniversityKey, short? BatchKey, string sidx, string sord, int page, int rows)
        {
            long TotalRecords = 0;
            List<dynamic> applicationList = new List<dynamic>();
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

            applicationList = studyMaterialService.GetApplications(objViewModel, out TotalRecords);

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

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudyMaterial, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditStudyMaterial(long? Id)
        {
            StudyMaterialViewModel objViewModel = new StudyMaterialViewModel();
            objViewModel.ApplicationKey = Id ?? 0;
            studyMaterialService.FillFeeYears(objViewModel);

            return View(objViewModel);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudyMaterial, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult AddEditStudyMaterial(StudyMaterialViewModel model)
        {

            if (ModelState.IsValid)
            {

                model = studyMaterialService.UpdateStudyMaterial(model);

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    return View(model);
                }

                model.Message = "";
                return View(model);
            }
            foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            {
            }

            model.Message = EduSuiteUIResources.Failed;
            return View(model);
        }

        [HttpGet]
        public JsonResult GetBookDetails(string StudyMaterialName, short? SubjectYear, long? ApplicationKey)
        {

            long TotalRecords = 0;
            int page = 1, rows = 10;
            List<StudyMaterialViewModel> bookList = new List<StudyMaterialViewModel>();
            StudyMaterialViewModel objViewModel = new StudyMaterialViewModel();

            //
            objViewModel.StudyMaterialName = StudyMaterialName;
            //objViewModel.SearchEmployeeKey = SearchEmployeeKey;
            objViewModel.SubjectYear = SubjectYear ?? 0;
            objViewModel.ApplicationKey = ApplicationKey ?? 0;

            objViewModel = studyMaterialService.GetStudyMaterialById(objViewModel);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            var totalPages = (int)Math.Ceiling((float)TotalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = objViewModel.StudyMaterialList.Count,
                rows = objViewModel.StudyMaterialList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]

        public ActionResult BookList(long? id)
        {
            StudyMaterialViewModel objViewModel = new StudyMaterialViewModel();
            objViewModel.ApplicationKey = id ?? 0;
            studyMaterialService.FillFeeYears(objViewModel);

            return PartialView(objViewModel);
        }
        [HttpPost]
        public ActionResult BookList(StudyMaterialViewModel model)
        {

            if (ModelState.IsValid)
            {


                model = studyMaterialService.CreateStudyMaterial(model);

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

            model.Message = EduSuiteUIResources.Failed;
            return Json(model);

        }
        [HttpGet]
        public JsonResult GetAllBooks(string StudyMaterialName, short? SubjectYear, long? ApplicationKey)
        {

            long TotalRecords = 0;
            //int page = 1, rows = 10;
            List<StudyMaterialViewModel> bookList = new List<StudyMaterialViewModel>();
            StudyMaterialViewModel objViewModel = new StudyMaterialViewModel();

            //
            objViewModel.StudyMaterialName = StudyMaterialName ?? null;
            //objViewModel.SearchEmployeeKey = SearchEmployeeKey;
            objViewModel.SubjectYear = SubjectYear ?? 0;
            objViewModel.ApplicationKey = ApplicationKey ?? 0;
            studyMaterialService.BindAvailableBooks(objViewModel);

            //int pageIndex = Convert.ToInt32(page) - 1;
            //int pageSize = rows;
            //var totalPages = (int)Math.Ceiling((float)TotalRecords / (float)rows);

            var jsonData = new
            {
                //total = totalPages,
                //page,
                records = objViewModel.StudyMaterialList.Count,
                rows = objViewModel.StudyMaterialList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudyMaterial, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteStudyMaterial(long Id)
        {
            StudyMaterialDetailsModel Model = new StudyMaterialDetailsModel();

            StudyMaterialViewModel objviewModel = new StudyMaterialViewModel();

            Model.RowKey = Id;
            try
            {
                objviewModel = studyMaterialService.DeleteStudyMaterial(Model);
            }
            catch (Exception)
            {
                objviewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objviewModel);
        }

        private void GetUserKey(StudyMaterialViewModel model)
        {
            CookieProvider cookieProvider = new CookieProvider();
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                CITSEduSuitePrincipalData userData = cookieProvider.GetCookie(authCookie);
                model.UserKey = userData.UserKey;
                model.RoleKey = userData.RoleKey;
            }
        }

        [HttpGet]
        public JsonResult FillBatch(short? BranchKey)
        {
            ApplicationViewModel model = new ApplicationViewModel();
            model.BranchKey = BranchKey;
            model.Batches = selectListService.FillBatches(model.BranchKey);
            return Json(model.Batches, JsonRequestBehavior.AllowGet);
        }
    }
}