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
    public class StudentEarlyDepartureController : BaseController
    {
       private IStudentEarlyDepartureService StudentEarlyDepartureService;
        private INotificationTemplateService notificationTemplateService;
        private ISharedService sharedService;
        public StudentEarlyDepartureController(IStudentEarlyDepartureService objStudentsAbscondersService, INotificationTemplateService objnotificationTemplateService, ISharedService objsharedService)
        {
            this.StudentEarlyDepartureService = objStudentsAbscondersService;
            this.notificationTemplateService = objnotificationTemplateService;
            this.sharedService = objsharedService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentEarlyDeparture, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult StudentEarlyDepartureList(long? id)
        {
            ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();

            objViewModel.ApplicationKey = id ?? 0;
            return View(objViewModel);

        }

        [HttpGet]
        public ActionResult BindStudentEarlyDepartureDetails(long? id)
        {
            List<StudentEarlyDepartureViewModel> objViewModel = new List<StudentEarlyDepartureViewModel>();
            objViewModel = StudentEarlyDepartureService.GetStudentEarlyDepartureDetails(id ?? 0);
            ViewBag.ApplicationKey = id ?? 0;
            return PartialView(objViewModel);
        }

        [HttpGet]
        public ActionResult StudentEarlyDepartureApplication(string AdmissionNo, long? ApplicationKey)
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
        //public ActionResult StudentEarlyDepartureApplications(long? id)
        //{
        //    ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();

        //    objViewModel.ApplicationKey = id ?? 0;
        //    string AdmissionNo = "";
        //    objViewModel = sharedService.FillApplicationDetails(AdmissionNo, objViewModel.ApplicationKey);
        //    return PartialView("StudentEarlyDepartureApplication", objViewModel);
        //}

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentEarlyDeparture, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditStudentEarlyDeparture(int? Id, long? ApplicationKey)
        {
            StudentEarlyDepartureViewModel objViewModel = new StudentEarlyDepartureViewModel();
            objViewModel.RowKey = Id ?? 0;
            objViewModel.ApplicationsKey = ApplicationKey ?? 0;

            objViewModel = StudentEarlyDepartureService.GetStudentEarlyDepartureById(objViewModel);

            return PartialView(objViewModel);
        }
        [HttpPost]
        public ActionResult AddEditStudentEarlyDeparture(StudentEarlyDepartureViewModel model)
        {
            ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();
            objViewModel.ApplicationKey = model.ApplicationsKey;
            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = StudentEarlyDepartureService.CreateStudentEarlyDepartureDate(model);
                }
                else
                {
                    model = StudentEarlyDepartureService.UpdateStudentEarlyDepartureDate(model);
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

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentEarlyDeparture, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteStudentEarlyDeparture(int? Id)
        {
            StudentEarlyDepartureViewModel model = new StudentEarlyDepartureViewModel();

            model.RowKey = Id ?? 0;

            try
            {
                model = StudentEarlyDepartureService.DeleteStudentEarlyDeparture(model.RowKey);
            }
            catch (Exception)
            {
                model.Message = EduSuiteUIResources.Failed;
            }
            return Json(model);
        }

        private void SendNotification(StudentEarlyDepartureViewModel model)
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
            notificationModel.PushNotificationTemplateKey = DbConstants.PushNotificationTemplate.StudentEarlyDeparture;
            nofificationHelper.SendNotificationInBackground(notificationModel);

        }
    }
}