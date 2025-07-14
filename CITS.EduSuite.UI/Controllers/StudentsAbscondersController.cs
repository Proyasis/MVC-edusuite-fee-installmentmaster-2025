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
    public class StudentsAbscondersController : BaseController
    {
        private IStudentAbscondersService StudentAbscondersService;
        private INotificationTemplateService notificationTemplateService;
        private ISharedService sharedService;
        public StudentsAbscondersController(IStudentAbscondersService objStudentsAbscondersService, INotificationTemplateService objnotificationTemplateService, ISharedService objsharedService)
        {
            this.StudentAbscondersService = objStudentsAbscondersService;
            this.notificationTemplateService = objnotificationTemplateService;
            this.sharedService = objsharedService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentAbsconders, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult StudentAbscondersList(long? id)
        {
            ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();

            objViewModel.ApplicationKey = id ?? 0;
            return View(objViewModel);

        }

        [HttpGet]
        public ActionResult BindStudentAbscondersDetails(long? id)
        {
            List<StudentAbscondersViewModel> objViewModel = new List<StudentAbscondersViewModel>();
            objViewModel = StudentAbscondersService.GetStudentAbscondersDetails(id ?? 0);
            ViewBag.ApplicationKey = id ?? 0;
            return PartialView(objViewModel);
        }



        [HttpGet]
        public ActionResult StudentAbscondersApplication(string AdmissionNo, long? ApplicationKey)
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
        //public ActionResult StudentAbscondersApplications(long? id)
        //{
        //    ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();

        //    objViewModel.ApplicationKey = id ?? 0;
        //    string AdmissionNo = "";
        //    objViewModel = sharedService.FillApplicationDetails(AdmissionNo, objViewModel.ApplicationKey);
        //    return PartialView("StudentAbscondersApplication", objViewModel);
        //}

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentAbsconders, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditStudentAbsconders(int? Id, long? ApplicationKey)
        {
            StudentAbscondersViewModel objViewModel = new StudentAbscondersViewModel();
            objViewModel.RowKey = Id ?? 0;
            objViewModel.ApplicationsKey = ApplicationKey ?? 0;

            objViewModel = StudentAbscondersService.GetStudentAbscondersById(objViewModel);

            return PartialView(objViewModel);
        }
        [HttpPost]
        public ActionResult AddEditStudentAbsconders(StudentAbscondersViewModel model)
        {
            ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();
            objViewModel.ApplicationKey = model.ApplicationsKey;

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = StudentAbscondersService.CreateStudentAbscondersDate(model);
                }
                else
                {
                    model = StudentAbscondersService.UpdateStudentAbscondersDate(model);
                }
                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    SendNotification(model);

                   // return View("StudentAbscondersList", objViewModel);
                    return Json(model);
                }

               // return View("StudentAbscondersList", objViewModel);
                return Json(model);
            }
            foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            {
            }
            model.Message = EduSuiteUIResources.Failed;
            //return View("StudentAbscondersList", objViewModel);
            return Json(model);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentAbsconders, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteStudentAbsconders(int? Id)
        {
            StudentAbscondersViewModel model = new StudentAbscondersViewModel();

            model.RowKey = Id ?? 0;

            try
            {
                model = StudentAbscondersService.DeleteStudentAbsconders(model.RowKey);
            }
            catch (Exception)
            {
                model.Message = EduSuiteUIResources.Failed;
            }
            return Json(model);
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentAbsconders, ActionCode = ActionConstants.Edit)]
        [HttpPost]
        public ActionResult UpdateStatusStudentAbsconders(int? Id)
        {
            StudentAbscondersViewModel model = new StudentAbscondersViewModel();

            model.RowKey = Id ?? 0;

            try
            {
                model = StudentAbscondersService.UpdateStatusStudentAbsconders(model.RowKey);
            }
            catch (Exception)
            {
                model.Message = EduSuiteUIResources.Failed;
            }
            return Json(model);
        }

        private void SendNotification(StudentAbscondersViewModel model)
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
            notificationModel.PushNotificationTemplateKey = DbConstants.PushNotificationTemplate.StudentAbsconders;
            nofificationHelper.SendNotificationInBackground(notificationModel);

        }
    }
}