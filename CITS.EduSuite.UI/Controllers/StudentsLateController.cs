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
    public class StudentsLateController : BaseController
    {
        private IStudentLateService StudentLateService;
        private INotificationTemplateService notificationTemplateService;
        private ISharedService sharedService;
        private ISelectListService selectListService;
        public StudentsLateController(IStudentLateService objStudentLateService,
            INotificationTemplateService objnotificationTemplateService,
            ISharedService objsharedService,
             ISelectListService objSelectListService
            )
        {
            this.StudentLateService = objStudentLateService;
            this.notificationTemplateService = objnotificationTemplateService;
            this.sharedService = objsharedService;
            this.selectListService = objSelectListService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentLate, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult StudentLateList(long? id)
        {
            ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();

            objViewModel.ApplicationKey = id ?? 0;
            return View(objViewModel);

        }

        [HttpGet]
        public ActionResult BindStudentLateDetails(long? id)
        {
            List<StudentLateViewModel> objViewModel = new List<StudentLateViewModel>();
            objViewModel = StudentLateService.GetStudentLateDetails(id ?? 0);
            ViewBag.ApplicationKey = id ?? 0;
            return PartialView(objViewModel);
        }



        [HttpGet]
        public ActionResult StudentLateApplication(string AdmissionNo, long? ApplicationKey)
        {
            ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();

            if ((objViewModel.ApplicationKey ?? 0) == 0)
            {
                objViewModel.ApplicationKey = sharedService.GetApplicationKeyByCode(AdmissionNo);
            }
            if (objViewModel.ApplicationKey == 0)
            {
                ModelState.AddModelError("error_msg", EduSuiteUIResources.Student_CannotFind);
            }

            return PartialView(objViewModel);
        }
        //[HttpGet]
        //public ActionResult StudentLateApplications(long? id)
        //{
        //    ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();

        //    objViewModel.ApplicationKey = id ?? 0;
        //    string AdmissionNo = "";
        //    objViewModel = sharedService.FillApplicationDetails(AdmissionNo, objViewModel.ApplicationKey);
        //    return PartialView("StudentLateApplication", objViewModel);
        //}

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentLate, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditStudentLate(int? Id, long? ApplicationKey)
        {
            StudentLateViewModel objViewModel = new StudentLateViewModel();
            objViewModel.RowKey = Id ?? 0;
            objViewModel.ApplicationsKey = ApplicationKey ?? 0;

            objViewModel = StudentLateService.GetStudentLateById(objViewModel);
            if (objViewModel.Message != AppConstants.Common.SUCCESS)
            {
                ModelState.AddModelError("error_msg", objViewModel.Message);
            }
            return PartialView(objViewModel);
        }
        [HttpPost]
        public ActionResult AddEditStudentLate(StudentLateViewModel model)
        {
            ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();
            objViewModel.ApplicationKey = model.ApplicationsKey;

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = StudentLateService.CreateStudentLateDate(model);
                }
                else
                {
                    model = StudentLateService.UpdateStudentLateDate(model);
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

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentLate, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteStudentLate(int? Id)
        {
            StudentLateViewModel model = new StudentLateViewModel();

            model.RowKey = Id ?? 0;

            try
            {
                model = StudentLateService.DeleteStudentLate(model.RowKey);
            }
            catch (Exception)
            {
                model.Message = EduSuiteUIResources.Failed;
            }
            return Json(model);
        }
        [HttpPost]
        public ActionResult GetLateMinuteById(short? Id, DateTime? Date, long? ApplicationKey)
        {

            StudentLateViewModel model = new StudentLateViewModel();

            model = StudentLateService.GetLateMinuteById(Id ?? 0, Date ?? DateTimeUTC.Now, ApplicationKey ?? 0);

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        private void SendNotification(StudentLateViewModel model)
        {
            NotificationDataViewModel notificationModel = new NotificationDataViewModel();
            NotificationHelper nofificationHelper = new NotificationHelper(notificationTemplateService);
            notificationModel.EmailTemplateName = Server.MapPath("~/Templates/NotificationTemplate/");
            notificationModel.RowKey = model.RowKey;
            notificationModel.AutoSMS = model.AutoSMS;
            notificationModel.AutoEmail = model.AutoEmail;
            notificationModel.TemplateKey = model.TemplateKey;
            notificationModel.EmailAddess = model.StudentEmail;
            notificationModel.MobileNumber = model.StudentMobile;
            notificationModel.GuardianMobileNumber = model.StudentGuardianPhone;
            notificationModel.PushNotificationTemplateKey = DbConstants.PushNotificationTemplate.StudentLate;
            nofificationHelper.SendNotificationInBackground(notificationModel);

        }
    }
}