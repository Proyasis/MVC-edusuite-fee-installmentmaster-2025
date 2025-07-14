using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.Security;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Provider;

//using CITS.EduSuite.Data;

namespace CITS.EduSuite.UI.Controllers
{
    public class ApplicationController : BaseController
    {
        private IApplicationService applicationService;
        private ISharedService sharedService;
        private ISelectListService selectListService;
        public ApplicationController(IApplicationService objApplicationService, ISharedService objSharedService, ISelectListService objselectListService)
        {
            this.applicationService = objApplicationService;
            this.sharedService = objSharedService;
            this.selectListService = objselectListService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Application, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult ApplicationList()
        {
            ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();

            objViewModel = applicationService.GetSearchDropdownList(objViewModel);
            ViewBag.ShowAcademicTerm = sharedService.ShowAcademicTerm();
            ViewBag.ShowUniversity = sharedService.ShowUniversity();
            ViewBag.IsApplicationSchedule = sharedService.CheckMenuActive(MenuConstants.ApplicationSchedule);
            return View(objViewModel);
        }

        [HttpGet]
        public JsonResult GetApplications(string ApplicantName, string MobileNumber, short? BranchKey, long? CourseKey, short? UniversityKey, short? BatchKey, bool? HasPhoto, string sidx, string sord, int page, int rows)
        {
            long TotalRecords = 0;
            List<ApplicationPersonalViewModel> applicationList = new List<ApplicationPersonalViewModel>();
            ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();


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
            objViewModel.HasOffer = HasPhoto ?? false;


            applicationList = applicationService.GetApplications(objViewModel, out TotalRecords);

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



        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Application, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteApplication(long id)
        {
            ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();

            //objViewModel.RowKey = id;
            try
            {
                objViewModel = applicationService.DeleteApplication(id);
                if (objViewModel.IsSuccessful)
                {
                    DeleteFileFolder(UrlConstants.ApplicationUrl + objViewModel.AdmissionNo);
                }
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);

        }

        private void DeleteFileFolder(string FilePath)
        {
            if (System.IO.Directory.Exists(Server.MapPath(FilePath)))
            {
                var path = Server.MapPath(FilePath);
                System.IO.Directory.Delete(path, true);
            }
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Application, ActionCode = ActionConstants.AddEdit)]
        [EncryptedActionParameter]
        [HttpGet]
        public ActionResult AddEditApplication(long? id, long? ApplicationWebFormKey)
        {
            ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();
            EnquiryViewModel model = new EnquiryViewModel();

            objViewModel.RowKey = id ?? 0;
            objViewModel.ApplicationWebFormKey = ApplicationWebFormKey ?? 0;
            model.EnquiryName = "";
            if (objViewModel.RowKey == 0 && objViewModel.ApplicationWebFormKey == 0)
            {
                ViewBag.InterestedEnquiryCount = applicationService.GetInterestedEnquiry(model).Count();
            }
            return View(objViewModel);
        }

        [HttpGet]
        public ActionResult ApplicationPhoto(long id)
        {
            ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();
            objViewModel = applicationService.GetApplicantPhotoById(id);
            return PartialView(objViewModel);
        }

        [HttpPost]
        public ActionResult UploadPhoto(HttpPostedFileBase PhotoFile, long ApplicationKey)
        {
            ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();
            objViewModel.RowKey = ApplicationKey;
            objViewModel = applicationService.UpdateApplicantPhoto(objViewModel);
            if (objViewModel.IsSuccessful)
            {
                UploadFile(PhotoFile, Server.MapPath(UrlConstants.ApplicationUrl + objViewModel.RowKey + "/"), objViewModel.ApplicantPhoto);
            }
            return Json(objViewModel);
        }

        [HttpPost]
        public ActionResult DeleteApplicantPhoto(long id)
        {
            ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();
            objViewModel = applicationService.DeleteApplicantPhoto(id);
            if (objViewModel.IsSuccessful)
            {
                if (objViewModel.StudentPhotoPath != null)
                {
                    //DeleteFile(UrlConstants.ApplicationUrl + objViewModel.RowKey + "/" + objViewModel.ApplicantPhoto);
                }
                else
                {
                    DeleteFile(UrlConstants.ApplicationUrl + objViewModel.RowKey + "/" + objViewModel.ApplicantPhoto);
                }
            }
            return Json(objViewModel);
        }


        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Application, ActionCode = ActionConstants.View)]
        [EncryptedActionParameter]
        [HttpGet]
        public ActionResult ViewApplication(long? id)
        {
            ApplicationViewModel objViewModel = new ApplicationViewModel();
            objViewModel.RowKey = id ?? 0;
            //objViewModel = applicationService.ViewApplicationById(id);
            return View(objViewModel);
        }
        public JsonResult GetStudentNavigation(long id)
        {
            ApplicationViewModel objViewModel = new ApplicationViewModel();
            objViewModel = applicationService.ViewApplicationById(id);
            return Json(objViewModel, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ViewApplicationDetails(long id)
        {
            ApplicationViewModel objViewModel = new ApplicationViewModel();
            objViewModel = applicationService.GetApplicationDetailsById(id);
            ViewBag.ShowAttendance = sharedService.CheckMenuActive(MenuConstants.StudentAttendance);
            ViewBag.ShowStudentLeave = sharedService.CheckMenuActive(MenuConstants.StudentLeave);
            ViewBag.ShowStudentLate = sharedService.CheckMenuActive(MenuConstants.StudentLate);
            ViewBag.ShowStudentAbsconders = sharedService.CheckMenuActive(MenuConstants.StudentAbsconders);
            ViewBag.ShowStudentEarlyDeparture = sharedService.CheckMenuActive(MenuConstants.StudentEarlyDeparture);
            ViewBag.ShowStudentDiary = sharedService.CheckMenuActive(MenuConstants.StudentDiary);
            ViewBag.ShowExamSchedule = sharedService.CheckMenuActive(MenuConstants.ExamSchedule);
            ViewBag.ShowExamResult = sharedService.CheckMenuActive(MenuConstants.ExamResult);
            ViewBag.ShowUniversityPayment = sharedService.CheckMenuActive(MenuConstants.UniversityPayment);
            ViewBag.ShowStudentIDCard = sharedService.CheckMenuActive(MenuConstants.StudentIDCard);
            ViewBag.ShowUniversityCertificate = sharedService.CheckMenuActive(MenuConstants.UniversityCertificate);
            ViewBag.ShowInternalExamResult = sharedService.CheckMenuActive(MenuConstants.InternalExamResult);
            ViewBag.ShowUnitTestResult = sharedService.CheckMenuActive(MenuConstants.UnitTestResult);
            ViewBag.ShowStudyMaterialt = sharedService.CheckMenuActive(MenuConstants.StudyMaterial);
            ViewBag.FeeSchedule = sharedService.CheckMenuActive(MenuConstants.FeeSchdeule);
            return PartialView(objViewModel);
        }
        [HttpGet]
        public JsonResult PrintApplication(long id)
        {
            ApplicationViewModel objViewModel = new ApplicationViewModel();
            objViewModel = applicationService.GetApplicationDetailsById(id);
            var Branch = sharedService.GetBranchDetailById(objViewModel.PersonalDetails.BranchKey);
            var json = new
            {
                Application = objViewModel,
                Branch
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AttendanceAcademicPerfomance(long Id)
        {
            List<List<dynamic>> AttendanceAcademicList = new List<List<dynamic>>();
            AttendanceAcademicList = applicationService.AttendanceAcademicPerfomance(Id);

            return Json(AttendanceAcademicList, JsonRequestBehavior.AllowGet);
        }

        private void UploadFile(HttpPostedFileBase PhotoFile, string FilePath, string FileName)
        {
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }
            if (PhotoFile != null)
            {
                PhotoFile.SaveAs(FilePath + FileName);
            }
        }

        private void DeleteFile(string FilePath)
        {
            if (System.IO.File.Exists(Server.MapPath(FilePath)))
            {
                System.IO.File.Delete(Server.MapPath(FilePath));
            }

        }

        [HttpGet]
        public ActionResult IntersetedEnquiryList()
        {
            return PartialView();
        }

        [HttpGet]
        public JsonResult GetInterestedEnquiry(string SearchText, int page, int rows)
        {

            List<EnquiryViewModel> enquiryList = new List<EnquiryViewModel>();
            EnquiryViewModel model = new EnquiryViewModel();
            model.EnquiryName = SearchText;



            var Take = rows;
            var Skip = (page - 1) * rows;


            var enquiryListQuery = applicationService.GetInterestedEnquiry(model);
            int totalRecords = enquiryListQuery.Count();
            enquiryList = enquiryListQuery.OrderBy(row => row.NextCallSchedule).Skip(Skip).Take(Take).ToList();
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;

            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = enquiryList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        //[HttpGet]
        //public JsonResult GetEmployeesByBranchId(short? id)
        //{
        //    ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();
        //    objViewModel.BranchKey = id ?? 0;
        //    
        //    objViewModel = applicationService.GetEmployeesByBranchId(objViewModel);
        //    return Json(objViewModel.CounsellingEmployees, JsonRequestBehavior.AllowGet);
        //}

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Application, ActionCode = ActionConstants.ApplicationStatus)]
        [HttpGet]
        public ActionResult AddEditEnrollmentNo(string Keys)
        {
            ApplicationEnRollmentNoViewModel model = new ApplicationEnRollmentNoViewModel();
            List<long> ApplicationKeys = new List<long>();
            if (Keys != null)
            {
                Keys = String.IsNullOrEmpty((Keys ?? "0")) ? "0" : (Keys ?? "0");
                ApplicationKeys = (Keys).Split(',').Select(row => Int64.Parse(row)).ToList();

            }
            model.IsClass = false;
            model = applicationService.GetEnrollmentNo(model, ApplicationKeys);

            foreach (EnRollmentNoDetailsViewModel item in model.EnRollmentNoDetailsViewModel)
            {
                item.CurrentYears = selectListService.ApplicationCourseYear(item.ApplicationKey ?? 0).ToList();
            }
            ViewBag.Title = EduSuiteUIResources.Add + EduSuiteUIResources.Sla + EduSuiteUIResources.Edit + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.EnrollmentNo;
            return PartialView(model);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Application, ActionCode = ActionConstants.ChangeClass)]
        [HttpGet]
        public ActionResult AddEditClass(string Keys)
        {
            ApplicationEnRollmentNoViewModel model = new ApplicationEnRollmentNoViewModel();
            List<long> ApplicationKeys = new List<long>();
            if (Keys != null)
            {
                Keys = String.IsNullOrEmpty((Keys ?? "0")) ? "0" : (Keys ?? "0");
                ApplicationKeys = (Keys).Split(',').Select(row => Int64.Parse(row)).ToList();

            }
            model.IsClass = true;
            model = applicationService.GetEnrollmentNo(model, ApplicationKeys);

            foreach (EnRollmentNoDetailsViewModel item in model.EnRollmentNoDetailsViewModel)
            {
                item.CurrentYears = selectListService.ApplicationCourseYear(item.ApplicationKey ?? 0).ToList();
            }
            ViewBag.Title = EduSuiteUIResources.Add + EduSuiteUIResources.Sla + EduSuiteUIResources.Edit + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.ClassCode;
            return PartialView("AddEditEnrollmentNo", model);
        }
        [HttpGet]

        public JsonResult GetStudentDetailsByStudentKey(long id)
        {
            ApplicationEnRollmentNoViewModel model = applicationService.GetStudentDetailsByStudentKey(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddEditEnrollmentNo(ApplicationEnRollmentNoViewModel model)
        {
            if (ModelState.IsValid)
            {
                model = applicationService.UpdateEnrollmentNo(model);

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    return Json(model);

                }


                model.Message = "";
                return View(model);
            }
            foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            {
            }

            model.Message = EduSuiteUIResources.Failed;
            return PartialView(model);

        }
        [HttpGet]
        public JsonResult CheckApplicationKeyExists(long ApplicationKey)
        {
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FillSearchBatch(short? BranchKey)
        {
            ApplicationPersonalViewModel model = new ApplicationPersonalViewModel();
            model.BranchKey = BranchKey ?? 0;
            model = applicationService.FillSearchBatch(model);
            return Json(model.Batches, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public FileContentResult DownloadFilePhoto(string filename, string Studentname, string AdmissionNo)
        {
            string Photoname = Studentname + EduSuiteUIResources.OpenBracket + AdmissionNo + EduSuiteUIResources.CloseBracket;
            string FullPath = Server.MapPath(filename);
            return File(System.IO.File.ReadAllBytes(FullPath), System.Web.MimeMapping.GetMimeMapping(FullPath), Photoname + "_" + Path.GetFileName(FullPath));
        }
    }
}