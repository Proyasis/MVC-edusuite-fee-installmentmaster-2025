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
    public class StudentDiaryController : BaseController
    {
        private IStudentDiaryService StudentDiaryService;
        private INotificationTemplateService notificationTemplateService;
        private ISharedService sharedService;
        public StudentDiaryController(IStudentDiaryService objStudentDiaryService, INotificationTemplateService objnotificationTemplateService, ISharedService objsharedService)
        {
            this.StudentDiaryService = objStudentDiaryService;
            this.notificationTemplateService = objnotificationTemplateService;
            this.sharedService = objsharedService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentDiary, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult StudentDiaryList(long? id)
        {
            ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();

            objViewModel.ApplicationKey = id ?? 0;
            return View(objViewModel);

        }

        [HttpGet]
        public ActionResult BindStudentDiaryDetails(long? id)
        {
            List<StudentDiaryViewModel> objViewModel = new List<StudentDiaryViewModel>();
            objViewModel = StudentDiaryService.GetStudentDiaryDetails(id ?? 0);
            ViewBag.ApplicationKey = id ?? 0;

            return PartialView(objViewModel);
        }

        [HttpGet]
        public ActionResult StudentDiaryApplication(string AdmissionNo, long? ApplicationKey)
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
        //public ActionResult StudentDiaryApplications(long? id)
        //{
        //    ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();

        //    objViewModel.ApplicationKey = id ?? 0;
        //    string AdmissionNo = "";
        //    objViewModel = sharedService.FillApplicationDetails(AdmissionNo, objViewModel.ApplicationKey);
        //    return PartialView("StudentDiaryApplication", objViewModel);
        //}

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentDiary, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditStudentDiary(int? Id, long? ApplicationKey)
        {
            StudentDiaryViewModel objViewModel = new StudentDiaryViewModel();
            objViewModel.RowKey = Id ?? 0;
            objViewModel.ApplicationsKey = ApplicationKey ?? 0;

            objViewModel = StudentDiaryService.GetStudentDiaryById(objViewModel);

            return PartialView(objViewModel);
        }
        [HttpPost]
        public ActionResult AddEditStudentDiary(StudentDiaryViewModel model)
        {
            ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();
            objViewModel.ApplicationKey = model.ApplicationsKey;
            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = StudentDiaryService.CreateStudentDiaryDate(model);
                }
                else
                {
                    model = StudentDiaryService.UpdateStudentDiaryDate(model);
                }
                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    SendNotification(model);

                    //return View("StudentDiaryList", objViewModel);
                    return Json(model);
                }
                return Json(model);
                //return View("StudentDiaryList", objViewModel);
            }
            foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            {
            }
            model.Message = EduSuiteUIResources.Failed;
            return Json(model);
            //return View("StudentDiaryList", objViewModel);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentDiary, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteStudentDiary(int? Id)
        {
            StudentDiaryViewModel model = new StudentDiaryViewModel();

            model.RowKey = Id ?? 0;

            try
            {
                model = StudentDiaryService.DeleteStudentDiary(model.RowKey);
            }
            catch (Exception)
            {
                model.Message = EduSuiteUIResources.Failed;
            }
            return Json(model);
        }

        private void SendNotification(StudentDiaryViewModel model)
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
            nofificationHelper.SendNotificationInBackground(notificationModel);

        }
    }
}