using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Security;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Provider;

using System.Configuration;
using CITS.EduSuite.Business.Models.Resources;


namespace CITS.EduSuite.UI.Controllers
{
    public class ApplicationWebFormController : BaseController
    {
        private IApplicationWebFormService applicationWebFormService;
        private ISharedService sharedService;
        private INotificationTemplateService notificationTemplateService;

        public ApplicationWebFormController(IApplicationWebFormService objApplicationWebFormService,
          ISharedService objSharedService, INotificationTemplateService objnotificationTemplateService)
        {
            this.applicationWebFormService = objApplicationWebFormService;
            this.sharedService = objSharedService;
            this.notificationTemplateService = objnotificationTemplateService;
        }


        public ActionResult Index()
        {
            ApplicationWebFormViewModel objViewModel = new ApplicationWebFormViewModel();
            //objViewModel.RowKey = id ?? 0;
            var Applicationwebform = applicationWebFormService.GetApplicationWebFormById(objViewModel);
            if (Applicationwebform == null)
            {
                Applicationwebform = new ApplicationWebFormViewModel();
            }
            else if (Applicationwebform.DateOfBirth != null)
            {
                Applicationwebform.Age = CommonHelper.CalculateAge(Convert.ToDateTime(Applicationwebform.DateOfBirth));
            }

            ViewBag.ShowAcademicTerm = sharedService.ShowAcademicTerm();
            ViewBag.ShowUniversity = sharedService.ShowUniversity();
            return View("AddEditApplicationWebForm", Applicationwebform);
        }

        [HttpPost]
        public ActionResult AddEditApplicationWebForm(ApplicationWebFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = applicationWebFormService.CreateApplicationWebForm(model);
                }
                else
                {
                    model = applicationWebFormService.UpdateApplicationWebForm(model);
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

        public JsonResult GetCourseTypeByAcademicTerm(short? AcademicTermKey)
        {
            ApplicationWebFormViewModel objViewModel = new ApplicationWebFormViewModel();
            objViewModel.AcademicTermKey = AcademicTermKey ?? 0;
            applicationWebFormService.GetCourseType(objViewModel);
            return Json(objViewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCourseByCourseType(short? CourseTypeKey, short? AcademicTermKey)
        {
            ApplicationWebFormViewModel objViewModel = new ApplicationWebFormViewModel();
            objViewModel.CourseTypeKey = CourseTypeKey ?? 0;
            objViewModel.AcademicTermKey = AcademicTermKey ?? 0;
            applicationWebFormService.GetCourseByCourseType(objViewModel);
            return Json(objViewModel, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetUniversity(short? AcademicTermKey, short? CourseKey)
        {
            ApplicationWebFormViewModel objViewModel = new ApplicationWebFormViewModel();
            objViewModel.CourseKey = CourseKey ?? 0;
            objViewModel.AcademicTermKey = AcademicTermKey ?? 0;
            applicationWebFormService.GetUniversity(objViewModel);
            return Json(objViewModel, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult CheckPhoneExists(string MobileNumber, long rowKey)
        {
            ApplicationWebFormViewModel application = new ApplicationWebFormViewModel();
            application = applicationWebFormService.CheckPhoneExists(MobileNumber, rowKey);
            return Json(application.IsSuccessful, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CheckPhoneExistsByUI(string MobileNumber, long rowKey)
        {
            ApplicationWebFormViewModel application = new ApplicationWebFormViewModel();
            application = applicationWebFormService.CheckPhoneExists(MobileNumber, rowKey);
            if (!application.IsSuccessful)
            {
                application.Message = ModelResources.CheckMobileExists;
            }
            return Json(application, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCasteByReligion(short? ReligionKey)
        {
            ApplicationWebFormViewModel objViewModel = new ApplicationWebFormViewModel();

            objViewModel.ReligionKey = ReligionKey ?? 0;
            objViewModel = applicationWebFormService.FillCaste(objViewModel);
            return Json(objViewModel.Caste, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetEmployeesByBranchId(short? id)
        {
            ApplicationWebFormViewModel objViewModel = new ApplicationWebFormViewModel();
            objViewModel.BranchKey = id ?? 0;

            objViewModel = applicationWebFormService.GetEmployeesByBranchId(objViewModel);
            return Json(objViewModel.Employees, JsonRequestBehavior.AllowGet);
        }
        private void SendNotification(ApplicationWebFormViewModel model)
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

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.ApplicationWebForm, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult ApplicationWebFormList()
        {
            ApplicationWebFormViewModel objViewModel = new ApplicationWebFormViewModel();

            applicationWebFormService.FillBranches(objViewModel);
            applicationWebFormService.FillWebFormStatus(objViewModel);
            applicationWebFormService.FillWebEnquiryStatus(objViewModel);
            ViewBag.ShowAcademicTerm = sharedService.ShowAcademicTerm();
            ViewBag.ShowUniversity = sharedService.ShowUniversity();
            return View(objViewModel);
        }

        [HttpGet]
        public JsonResult GetApplicationWebForm(string ApplicantName, short? BranchKey, short? WebFormStatusKey,short? EnquiryStatusKey, string sidx, string sord, int page, int rows)
        {
            long TotalRecords = 0;
            List<ApplicationWebFormViewModel> applicationList = new List<ApplicationWebFormViewModel>();
            ApplicationWebFormViewModel objViewModel = new ApplicationWebFormViewModel();

            objViewModel.ApplicantName = ApplicantName;
            objViewModel.BranchKey = BranchKey ?? 0;
            objViewModel.WebFormStatusKey = WebFormStatusKey ?? 0;
            objViewModel.EnquiryStatusKey = EnquiryStatusKey;
            objViewModel.PageIndex = page;
            objViewModel.PageSize = rows;
            objViewModel.SortBy = sidx;
            objViewModel.SortOrder = sord;

            applicationList = applicationWebFormService.GetApplicationWebForm(objViewModel, out TotalRecords);

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

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.ApplicationWebForm, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteApplicationWebForm(long id)
        {
            ApplicationWebFormViewModel objViewModel = new ApplicationWebFormViewModel();

            //objViewModel.RowKey = id;
            try
            {
                objViewModel = applicationWebFormService.DeleteApplicationWebForm(id);

            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);

        }
    }
}