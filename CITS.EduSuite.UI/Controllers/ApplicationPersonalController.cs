using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Security;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Provider;

using System.Configuration;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI.Controllers
{
    public class ApplicationPersonalController : BaseController
    {
        private IApplicationPersonalService applicationPersonalService;
        private ISharedService sharedService;
        private INotificationTemplateService notificationTemplateService;
        public ApplicationPersonalController(IApplicationPersonalService objApplicationPersonalService,
            ISharedService objSharedService, INotificationTemplateService objnotificationTemplateService)
        {
            this.applicationPersonalService = objApplicationPersonalService;
            this.sharedService = objSharedService;
            this.notificationTemplateService = objnotificationTemplateService;
        }



        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Application, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditApplicationPersonal(long? id, long? ApplicationWebFormKey)
        {
            ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();
            objViewModel.RowKey = id ?? 0;
            objViewModel.ApplicationWebFormKey = ApplicationWebFormKey ?? 0;


            var ApplicationPersonal = applicationPersonalService.GetApplicationPersonalById(objViewModel);
            if (ApplicationPersonal == null)
            {
                ApplicationPersonal = new ApplicationPersonalViewModel();
            }
            else if (ApplicationPersonal.DateOfBirth != null)
            {
                ApplicationPersonal.Age = CommonHelper.CalculateAge(Convert.ToDateTime(ApplicationPersonal.DateOfBirth));
            }
            if (ApplicationWebFormKey == 0)
            {
                objViewModel.ApplicationWebFormKey = objViewModel.ApplicationWebFormKey;
            }
            else
            {
                objViewModel.ApplicationWebFormKey = ApplicationWebFormKey;
            }

            ViewBag.ShowAcademicTerm = sharedService.ShowAcademicTerm();
            ViewBag.ShowUniversity = sharedService.ShowUniversity();
            return PartialView(ApplicationPersonal);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Application, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult AddEditApplicationPersonal(ApplicationPersonalViewModel model)
        {
            //ApplicationPersonalViewModel objviewmodel = new ApplicationPersonalViewModel();
            //objviewmodel = model;

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {

                    model = applicationPersonalService.CreateApplicationPersonal(model);

                    //for (int i = 0; i <= 1000; i++)
                    //{
                    //    model = applicationPersonalService.CreateApplicationPersonal(objviewmodel);
                    //}
                    //int j = 0;
                    //while (j <= 500)
                    //{
                    //    model = applicationPersonalService.CreateApplicationPersonal(objviewmodel);
                    //    j++;
                    //}
                }
                else
                {
                    model = applicationPersonalService.UpdateApplicationPersonal(model);
                }

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    SendNotification(model);
                    return Json(model);
                }


                return Json(model);
            }
            foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            {
            }

            model.Message = EduSuiteUIResources.Failed;
            return Json(model);

        }

        [HttpGet]
        public JsonResult GetYearByMode(short? ModeKey, long? CourseKey, short? AcademicTermKey, short? UniversityKey)
        {
            ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();
            objViewModel.ModeKey = ModeKey ?? 0;
            objViewModel.CourseKey = CourseKey ?? 0;
            objViewModel.AcademicTermKey = AcademicTermKey ?? 0;
            objViewModel.UniversityKey = UniversityKey ?? 0;
            applicationPersonalService.GetYearByMode(objViewModel);
            return Json(objViewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCurrentYearByYear(int? StartYear, short? ModeKey, long? CourseKey, short? AcademicTermKey, short? UniversityKey)
        {
            ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();
            objViewModel.StartYear = StartYear ?? 0;
            objViewModel.ModeKey = ModeKey ?? 0;
            objViewModel.CourseKey = CourseKey ?? 0;
            objViewModel.AcademicTermKey = AcademicTermKey ?? 0;
            objViewModel.UniversityKey = UniversityKey ?? 0;
            applicationPersonalService.GetCurrentYearByYear(objViewModel);
            return Json(objViewModel.CurrentYears, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCourseTypeByAcademicTerm(short? AcademicTermKey)
        {
            ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();
            objViewModel.AcademicTermKey = AcademicTermKey ?? 0;
            applicationPersonalService.GetCourseType(objViewModel);
            return Json(objViewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCourseByCourseType(short? CourseTypeKey, short? AcademicTermKey)
        {
            ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();
            objViewModel.CourseTypeKey = CourseTypeKey ?? 0;
            objViewModel.AcademicTermKey = AcademicTermKey ?? 0;
            applicationPersonalService.GetCourseByCourseType(objViewModel);
            return Json(objViewModel, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetUniversity(short? AcademicTermKey, short? CourseKey)
        {
            ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();
            objViewModel.CourseKey = CourseKey ?? 0;
            objViewModel.AcademicTermKey = AcademicTermKey ?? 0;
            applicationPersonalService.GetUniversity(objViewModel);
            return Json(objViewModel, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult CheckPhoneExists(string MobileNumber, long rowKey)
        {
            ApplicationPersonalViewModel application = new ApplicationPersonalViewModel();
            application = applicationPersonalService.CheckPhoneExists(MobileNumber, rowKey);
            if (!application.IsSuccessful)
            {
                application.Message = ModelResources.CheckMobileExists;
            }
            return Json(application, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CheckEmailExists(string EmailAddress, long rowKey)
        {
            ApplicationPersonalViewModel application = new ApplicationPersonalViewModel();
            application = applicationPersonalService.CheckEmailExists(EmailAddress, rowKey);
            return Json(application.IsSuccessful, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult FetchApplicationFromEnquiry(long? id)
        {
            ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();
            objViewModel.EnquiryKey = id ?? 0;

            var ApplicationPersonal = applicationPersonalService.FetchApplicationFromEnquiry(objViewModel);
            if (ApplicationPersonal == null)
            {
                ApplicationPersonal = new ApplicationPersonalViewModel();
            }
            else if (ApplicationPersonal.DateOfBirth != null)
            {
                ApplicationPersonal.Age = CommonHelper.CalculateAge(Convert.ToDateTime(ApplicationPersonal.DateOfBirth));
            }
            string ClassRequired = ConfigurationManager.AppSettings["StudentClassRequired"].ToString();
            if (ClassRequired == "true")
            {
                ApplicationPersonal.ifClassRequired = true;
            }
            else
            {
                ApplicationPersonal.ifClassRequired = false;
            }
            ViewBag.ShowAcademicTerm = sharedService.ShowAcademicTerm();
            ViewBag.ShowUniversity = sharedService.ShowUniversity();
            return PartialView("AddEditApplicationPersonal", ApplicationPersonal);
        }

        [HttpGet]
        public ActionResult GetAdmissionFee(long id, short? CourseKey, short? AcademicTermKey, short? AdmissionCurrentYear, short? UniversityKey)
        {
            ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();
            objViewModel.RowKey = id;
            objViewModel.CourseKey = CourseKey ?? 0;
            objViewModel.AcademicTermKey = AcademicTermKey ?? 0;
            objViewModel.AdmissionCurrentYear = AdmissionCurrentYear ?? 0;
            objViewModel.UniversityKey = UniversityKey ?? 0;
            objViewModel = applicationPersonalService.GetAdmissionFeesByCourse(objViewModel);
            return PartialView(objViewModel);
        }

        [HttpGet]
        public ActionResult GetOfferDetails()
        {
            ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();

            //objViewModel.OfferDate = OfferDate ?? DateTime.UtcNow;
            objViewModel = applicationPersonalService.GetOfferDetails(objViewModel);
            return Json(objViewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FillClassDetails(short? UniversityKey, long? CourseKey, short? AcademicTermKey, short? CurrentYear)
        {
            ApplicationPersonalViewModel model = new ApplicationPersonalViewModel();
            model.UniversityKey = UniversityKey ?? 0;
            model.CourseKey = CourseKey ?? 0;
            model.AcademicTermKey = AcademicTermKey ?? 0;
            model.CurrentYear = CurrentYear ?? 0;
            model = applicationPersonalService.FillClassDetails(model);
            return Json(model.ClassDetails, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCasteByReligion(short? ReligionKey)
        {
            ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();

            objViewModel.ReligionKey = ReligionKey ?? 0;
            objViewModel = applicationPersonalService.FillCaste(objViewModel);
            return Json(objViewModel.Caste, JsonRequestBehavior.AllowGet);
        }

        private void SendNotification(ApplicationPersonalViewModel model)
        {


            NotificationDataViewModel notificationModel = new NotificationDataViewModel();
            NotificationHelper nofificationHelper = new NotificationHelper(notificationTemplateService);
            notificationModel.EmailTemplateName = Server.MapPath("~/Templates/NotificationTemplate/");
            notificationModel.RowKey = model.RowKey;
            notificationModel.AutoSMS = model.AutoSMS;
            notificationModel.AutoEmail = model.AutoEmail;
            notificationModel.TemplateKey = model.TemplateKey;
            notificationModel.EmailAddess = model.StudentEmail;
            notificationModel.MobileNumber = model.MobileNumber;
            nofificationHelper.SendNotificationInBackground(notificationModel);
        }

        [HttpGet]
        public JsonResult GetEmployeesByBranchId(short? id)
        {
            ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();
            objViewModel.BranchKey = id ?? 0;

            objViewModel = applicationPersonalService.GetEmployeesByBranchId(objViewModel);
            return Json(objViewModel.Employees, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckSecondLanguage(short? id)
        {
            bool HasSecondLanguage = applicationPersonalService.CheckSecondLanguage(id);
            return Json(HasSecondLanguage, JsonRequestBehavior.AllowGet);
        }
    }
}