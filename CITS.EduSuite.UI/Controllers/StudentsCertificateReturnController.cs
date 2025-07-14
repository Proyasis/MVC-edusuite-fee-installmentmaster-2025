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
    public class StudentsCertificateReturnController : BaseController
    {
        private IStudentsCertificateReturnService studentsCertificateReturnservice;
        private ISelectListService selectListService;
        public StudentsCertificateReturnController(IStudentsCertificateReturnService objStudentsCertificateReturn, ISelectListService objselectListService)
        {
            this.studentsCertificateReturnservice = objStudentsCertificateReturn;
            this.selectListService = objselectListService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentsCertificateReturn, ActionCode = ActionConstants.View)]
        [HttpGet]
        public ActionResult StudentsCertificateReturnList()
        {
            ApplicationViewModel model = new ApplicationViewModel();

            model.Branches = selectListService.FillBranches();
            model.Batches = selectListService.FillSearchBatch(model.BranchKey);
            model.Courses = selectListService.FillSearchCourse(model.BranchKey);
            model.Universities = selectListService.FillSearchUniversity(model.BranchKey);
            return View(model);
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

            applicationList = studentsCertificateReturnservice.GetApplications(objViewModel, out TotalRecords);

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

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentsCertificateReturn, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditStudentsCertificateReturn(long? id, byte? ReturnTypeKey)
        {
            StudentsCertificateReturnViewModel model = new StudentsCertificateReturnViewModel();
            model.ApplicationKey = id ?? 0;
            model.CertificateStatusKey = ReturnTypeKey ?? 0;

            var applicationIdenties = studentsCertificateReturnservice.GetStudentsCertificateById(model);
            if (applicationIdenties == null)
            {
                applicationIdenties = new StudentsCertificateReturnViewModel();
            }
            return PartialView(applicationIdenties);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentsCertificateReturn, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult AddEditStudentsCertificateReturn(StudentsCertificateReturnViewModel model)
        {

            if (ModelState.IsValid)
            {


                model = studentsCertificateReturnservice.UpdateStudentsCertificateProcess(model);

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    return Json(model);
                }

                model.Message = "";
                return PartialView(model);
            }
            foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            {
            }

            model.Message = EduSuiteUIResources.Failed;
            return PartialView(model);

        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentsCertificateReturn, ActionCode = ActionConstants.Delete)]
        public ActionResult ResetStudentsCertificateReturn(long Id)
        {
            StudentsCertificateReturnViewModel model = new StudentsCertificateReturnViewModel();

            StudentsCertificateReturnDetail objviewModel = new StudentsCertificateReturnDetail();
            objviewModel.RowKey = Id;
            try
            {
                model = studentsCertificateReturnservice.DeleteStudentsCertificateProcess(objviewModel);
            }
            catch (Exception)
            {
                model.Message = EduSuiteUIResources.Failed;
            }
            return Json(objviewModel);

        }

        [HttpGet]
        public JsonResult GetCertificateDetailsByApplication(string id)
        {

            var CertificateDetailslogList = studentsCertificateReturnservice.GetCertificateDetailsByApplication(Convert.ToInt32(id));
            var jsonData = new
            {
                rows = CertificateDetailslogList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FillBatch(short? BranchKey)
        {
            StudentsCertificateReturnViewModel model = new StudentsCertificateReturnViewModel();
            model.BranchKey = BranchKey ?? 0;
            model.Batches = selectListService.FillBatches(model.BranchKey);
            return Json(model.Batches, JsonRequestBehavior.AllowGet);
        }
    }
}