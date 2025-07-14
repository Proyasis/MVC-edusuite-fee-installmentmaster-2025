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
    public class StudentLeaveController : BaseController
    {
       private IStudentLeaveService StudentLeaveService;
        private INotificationTemplateService notificationTemplateService;
        private ISharedService sharedService;
        public StudentLeaveController(IStudentLeaveService objStudentLeaveService, INotificationTemplateService objnotificationTemplateService, ISharedService objsharedService)
        {
            this.StudentLeaveService = objStudentLeaveService;
            this.notificationTemplateService = objnotificationTemplateService;
            this.sharedService = objsharedService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentLeave, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult StudentLeaveList(long? id)
        {
            ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();

            objViewModel.ApplicationKey = id ?? 0;
            return View(objViewModel);

        }

        [HttpGet]
        public ActionResult BindStudentLeaveDetails(long? id)
        {
            List<StudentLeaveViewModel> objViewModel = new List<StudentLeaveViewModel>();
            objViewModel = StudentLeaveService.GetStudentLeaveDetails(id ?? 0);
            ViewBag.ApplicationKey = id ?? 0;
            return PartialView(objViewModel);
        }



        [HttpGet]
        public ActionResult StudentLeaveApplication(string AdmissionNo, long? ApplicationKey)
        {
            ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();

            if ((objViewModel.ApplicationKey??0) == 0)
            {
                objViewModel.ApplicationKey = sharedService.GetApplicationKeyByCode(AdmissionNo);
            }
            if (objViewModel.ApplicationKey == 0)
            {
                ModelState.AddModelError("error_msg", EduSuiteUIResources.Student_CannotFind);
            }
            return PartialView(objViewModel);
        }
       

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentLeave, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditStudentLeave(int? Id, long? ApplicationKey)
        {
            StudentLeaveViewModel objViewModel = new StudentLeaveViewModel();
            objViewModel.RowKey = Id ?? 0;
            objViewModel.ApplicationsKey = ApplicationKey ?? 0;

            objViewModel = StudentLeaveService.GetStudentLeaveById(objViewModel);
            if (objViewModel.Message != AppConstants.Common.SUCCESS)
            {
                ModelState.AddModelError("error_msg", objViewModel.Message);
            }
            return PartialView(objViewModel);
        }
        [HttpPost]
        public ActionResult AddEditStudentLeave(StudentLeaveViewModel model)
        {
            ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();
            objViewModel.ApplicationKey = model.ApplicationsKey;

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = StudentLeaveService.CreateStudentLeaveDate(model);
                }
                else
                {
                    model = StudentLeaveService.UpdateStudentLeaveDate(model);
                }
                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    SendNotification(model);
                    
                    //return View("StudentLeaveList", objViewModel);
                    return Json(model);
                }

                //return View("StudentLeaveList", objViewModel);
                return Json(model);
            }
            foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            {
            }
            model.Message = EduSuiteUIResources.Failed;
            //return View("StudentLeaveList", objViewModel);
            return Json(model);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentLeave, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteStudentLeave(int? Id)
        {
            StudentLeaveViewModel model = new StudentLeaveViewModel();

            model.RowKey = Id ?? 0;

            try
            {
                model = StudentLeaveService.DeleteStudentLeave(model.RowKey);
            }
            catch (Exception)
            {
                model.Message = EduSuiteUIResources.Failed;
            }
            return Json(model);
        }

        private void SendNotification(StudentLeaveViewModel model)
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
            notificationModel.PushNotificationTemplateKey = DbConstants.PushNotificationTemplate.StudentLeave;
            nofificationHelper.SendNotificationInBackground(notificationModel);

        }
    }
}