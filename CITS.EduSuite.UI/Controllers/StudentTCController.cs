using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using HandlebarsDotNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace CITS.EduSuite.UI.Controllers
{
    public class StudentTCController : BaseController
    {
        private IStudentTCService StudentTCService;
        private ISelectListService selectListService;
        private INotificationTemplateService notificationTemplateService;
        public StudentTCController(IStudentTCService objStudentTCService,
            ISelectListService objselectListService
            , INotificationTemplateService objnotificationTemplateService)
        {
            this.StudentTCService = objStudentTCService;
            this.selectListService = objselectListService;
            this.notificationTemplateService = objnotificationTemplateService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentTC, ActionCode = ActionConstants.View)]
        [HttpGet]
        public ActionResult StudentTCList()
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

            applicationList = StudentTCService.GetApplications(objViewModel, out TotalRecords);

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

        public JsonResult FetchTCDetails(StudentTCViewModel model)
        {
            List<dynamic> FetchTCDetails = new List<dynamic>();
            FetchTCDetails = StudentTCService.FetchTCDetails(model);

            return Json(FetchTCDetails, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentTC, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditStudentTC(long? id)
        {
            long ApplicationKey = id != null ? Convert.ToInt64(id) : 0;
            var applicationIdenties = StudentTCService.GetStudentTcById(ApplicationKey);
            if (applicationIdenties == null)
            {
                applicationIdenties = new StudentTCViewModel();
            }
            return View(applicationIdenties);
        }

        [HttpPost]
        public ActionResult GetTCColumnDetails(StudentTCViewModel model)
        {
            model = StudentTCService.GetTCColumnDetails(model);
            //if (!model.IsSuccessful)
            //{
            //    return Json(model);
            //}
            return PartialView(model);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentTC, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult AddEditStudentTC(StudentTCViewModel model)
        {

            if (ModelState.IsValid)
            {

                if (model.RowKey == 0)
                {
                    model = StudentTCService.CreateStudentTC(model);
                }
                else
                {
                    model = StudentTCService.UpdateStudentTC(model);

                }
                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    return Json(model);
                   
                }

                model.Message = "";
                return Json(model);
            }
            foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            {
            }

            model.Message = EduSuiteUIResources.Failed;
            return Json(model);

        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UniversityCertificate, ActionCode = ActionConstants.Delete)]
        public ActionResult DeleteStuentTCDetails(long Id)
        {
            StudentTCDetailsViewModel objviewModel = new StudentTCDetailsViewModel();

            try
            {
                objviewModel = StudentTCService.DeleteStudentTCDetails(Id);
            }
            catch (Exception)
            {
                objviewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objviewModel);

        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UniversityCertificate, ActionCode = ActionConstants.Delete)]
        public ActionResult DeleteStuentTC(long Id)
        {
            StudentTCViewModel objviewModel = new StudentTCViewModel();
            try
            {
                objviewModel = StudentTCService.DeleteStudentTC(Id);
            }
            catch (Exception)
            {
                objviewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objviewModel);

        }

        [HttpGet]
        public JsonResult FillBatch(short? BranchKey)
        {
            StudentTCViewModel model = new StudentTCViewModel();
            model.BranchKey = BranchKey;
            model.Batches = selectListService.FillBatches(model.BranchKey);
            return Json(model.Batches, JsonRequestBehavior.AllowGet);
        }

        public ActionResult IssueStudentTC(long? id)
        {
            StudentTCViewModel model = new StudentTCViewModel();
            long ApplicationKey = id != null ? Convert.ToInt64(id) : 0;
            model = StudentTCService.GetStudentTcById(ApplicationKey);
            if (model == null)
            {
                model = new StudentTCViewModel();
            }
            return PartialView(model);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentTC, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult IssueStudentTC(StudentTCViewModel model)
        {

            if (ModelState.IsValid)
            {
                model = StudentTCService.IssueStudentTC(model);

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    //SendNotificationInBackground(model);
                    return Json(model);
                    //return RedirectToAction("StudentTCList");
                    //return RedirectToAction("StudentTCList", "StudentTC");
                }

                model.Message = "";
                return Json(model);
            }
            foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            {
            }

            model.Message = EduSuiteUIResources.Failed;
            return Json(model);

        }



        public void SendNotificationInBackground(StudentTCViewModel model)
        {

            Thread bgThread = new Thread(new ParameterizedThreadStart(SendNotification));

            bgThread.IsBackground = true;
            bgThread.Start(model);

        }
        private void SendNotification(Object model)
        {
            List<NotificationDataViewModel> modelList = new List<NotificationDataViewModel>();
            StudentTCViewModel objViewModel = (StudentTCViewModel)model;
            string Url = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, "");
            NotificationHelper nofificationHeper = new NotificationHelper(notificationTemplateService);
            nofificationHeper.hanldebarHelepers();
            NotificationDataViewModel objNOtificationViewModel = new NotificationDataViewModel();
            NotificationDataViewModel objPushNotificationModel = new NotificationDataViewModel();
            string FilePath = Server.MapPath("~/Templates/NotificationTemplate/");
            objNOtificationViewModel.EmailTemplateName = Server.MapPath("~/Templates/NotificationTemplate/");

            objNOtificationViewModel.AutoSMS = objViewModel.AutoSMS;

            objNOtificationViewModel.AutoEmail = objViewModel.AutoEmail;
            objNOtificationViewModel.TemplateKey = objViewModel.TemplateKey;

            bool IsSMS = objViewModel.AutoSMS ?? false;
            bool IsEmail = objViewModel.AutoEmail ?? false;

            FilePath = FilePath + objNOtificationViewModel.EmailTemplateName;
            long UserKey = DbConstants.User.UserKey;


            ExpandoObject jsons = new ExpandoObject();





            objNOtificationViewModel.RowKey = objViewModel.ApplicationKey;
            objNOtificationViewModel.PushNotificationTemplateKey = DbConstants.PushNotificationTemplate.StudentTC;

            objPushNotificationModel = notificationTemplateService.GetPushNotificationData(objNOtificationViewModel);
            if (objPushNotificationModel.PushNotificationContent != null && objPushNotificationModel.NotificationData != "" && objPushNotificationModel.NotificationData != null)
            {
                var jsonArray = JObject.Parse(objPushNotificationModel.NotificationData);
                JsonSerializer jsonSerializere = new JsonSerializer();
                jsons = jsonSerializere.Deserialize<ExpandoObject>(new JTokenReader(jsonArray));
                
            }
            if (IsEmail || IsSMS)
            {
                objNOtificationViewModel.TemplateKey = DbConstants.NotificationTemplate.StudentTC;

                objNOtificationViewModel = notificationTemplateService.GetNotificationData(objNOtificationViewModel);
                var jsonArray = JObject.Parse(objNOtificationViewModel.NotificationData);
                JsonSerializer jsonSerializer = new JsonSerializer();
                ExpandoObject json = jsonSerializer.Deserialize<ExpandoObject>(new JTokenReader(jsonArray));


                if (System.IO.File.Exists(FilePath) && objViewModel.StudentEmail != "" && objViewModel.StudentEmail != null && objViewModel.AutoEmail == true)
                {

                    var template = Handlebars.Compile(System.IO.File.ReadAllText(FilePath));
                    var result = template(json);
                    EmailViewModel emailModel = new EmailViewModel();
                    emailModel.EmailBody = result;
                    emailModel.EmailTo = objViewModel.StudentEmail;
                    emailModel.EmailCC = objNOtificationViewModel.AdminEmailAddress;
                    emailModel.EmailSubject = objNOtificationViewModel.EmailSubject;

                    EmailHelper.SendEmail(emailModel);
                    objNOtificationViewModel.NotificationTypeKey = DbConstants.NotificationType.Email;
                    modelList.Add(objNOtificationViewModel);

                }
                if (objNOtificationViewModel.SMSTemplate != null && objViewModel.StudentMobile != null && objViewModel.AutoSMS == true)
                {

                    var template = Handlebars.Compile(objNOtificationViewModel.SMSTemplate);
                    var result = template(json);
                    SMSViewModel smsModel = new SMSViewModel();
                    smsModel.SMSContent = result;
                    smsModel.SMSReceiptants = objViewModel.StudentMobile;
                    smsModel.SMSTemplateID = objNOtificationViewModel.SMSTemplateID;
                    SMSHelper.SendSMS(smsModel);
                    var objSMSViewModel = (NotificationDataViewModel)objNOtificationViewModel;
                    objSMSViewModel.NotificationTypeKey = DbConstants.NotificationType.SMS;
                    objSMSViewModel.SMSTemplate = smsModel.SMSContent;
                    modelList.Add(objSMSViewModel);
                }

            }



            if (objPushNotificationModel.NotificationData != null)
            {
                var template = Handlebars.Compile(objPushNotificationModel.PushNotificationContent);
                var result = template(jsons);
                byte[] utf8Bytes = Encoding.UTF8.GetBytes(result);
                String EncodedResult = Encoding.UTF8.GetString(utf8Bytes);
                objPushNotificationModel.PushNotificationContent = EncodedResult;

                NotificationHub.PushNotification(objPushNotificationModel);
                objPushNotificationModel.NotificationTypeKey = DbConstants.NotificationType.Push;
                modelList.Add(objPushNotificationModel);
            }

            notificationTemplateService.CreateNotification(modelList);
        }
    }
}