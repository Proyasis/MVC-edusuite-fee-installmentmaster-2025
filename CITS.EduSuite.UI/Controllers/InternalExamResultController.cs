using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Provider;
using System.Web.Security;
using CITS.EduSuite.Business.Models.Security;
using System.Threading;
using HandlebarsDotNet;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI.Controllers
{
    public class InternalExamResultController : BaseController
    {
        public IInternalExamResultService internalExamResultService;
        private INotificationTemplateService notificationTemplateService;
        public InternalExamResultController(IInternalExamResultService objInternalExamResultService, INotificationTemplateService objnotificationTemplateService)
        {
            this.internalExamResultService = objInternalExamResultService;
            this.notificationTemplateService = objnotificationTemplateService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.InternalExamResult, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult InternalExamResultList()
        {
            InternalExamResultViewModel model = new InternalExamResultViewModel();
            
            model = internalExamResultService.GetSearchDropDownLists(model);
            return View(model);
        }

        [HttpGet]
        public JsonResult GetInternalExamResult(long? SearchEmployeeKey, short? BranchKey, long? ClassDetailsKey, short? BatchKey)
        {

            int page = 1; int rows = 15;
            List<InternalExamResultViewModel> internalExamResultList = new List<InternalExamResultViewModel>();
            InternalExamResultViewModel objViewModel = new InternalExamResultViewModel();
            
            objViewModel.SearchEmployeeKey = SearchEmployeeKey ?? 0;
            objViewModel.BranchKey = BranchKey ?? 0;
            objViewModel.ClassDetailsKey = ClassDetailsKey ?? 0;
            objViewModel.BatchKey = BatchKey ?? 0;


            internalExamResultList = internalExamResultService.GetInternalExamResult(objViewModel);
            int pageindex = Convert.ToInt32(page) - 1;
            int pagesize = rows;
            int totalrecords = internalExamResultList.Count();
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)rows);
            var jsonData = new
            {
                total = totalpage,
                page,
                records = totalrecords,
                rows = internalExamResultList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]

        public ActionResult GetAllSubjectDetails(long? InternalExamKey, long? ClassDetailsKey)
        {
            InternalExamResultViewModel objViewModel = new InternalExamResultViewModel();

            objViewModel.InternalExamKey = InternalExamKey;
            objViewModel.ClassDetailsKey = ClassDetailsKey;
            return View(objViewModel);
        }

        [HttpGet]
        public ActionResult GetSubjectDetils(InternalExamResultViewModel model)
        {
            long TotalRecords = 0;
            int page = 1, rows = 10;

            InternalExamResultViewModel objViewModel = new InternalExamResultViewModel();

            objViewModel.InternalExamKey = model.InternalExamKey;
            objViewModel.ClassDetailsKey = model.ClassDetailsKey ?? 0;

            objViewModel = internalExamResultService.GetInternalExamResultDetails(objViewModel);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            var totalPages = (int)Math.Ceiling((float)TotalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = objViewModel.InternaleExamResultSubjectDetails.Count,
                rows = objViewModel.InternaleExamResultSubjectDetails
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.InternalExamResult, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditStudentsMarkList(long? InternalExamKey, long? SubjectKey, long? ClassDetailsKey)
        {
            InternalExamResultViewModel objViewModel = new InternalExamResultViewModel();


            objViewModel.InternalExamKey = InternalExamKey;
            objViewModel.ClassDetailsKey = ClassDetailsKey ?? 0;
            objViewModel.SubjectKey = SubjectKey;

            objViewModel = internalExamResultService.StudentMarkDetils(objViewModel);

            return PartialView(objViewModel);


        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.InternalExamResult, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult AddEditStudentsMarkList(InternalExamResultViewModel model)
        {
            if (ModelState.IsValid)
            {
               
                model = internalExamResultService.UpdateInternalExamResult(model);
                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    if (model.AutoEmail == true || model.AutoSMS == true)
                        SendNotificationInBackground(model);
                    return Json(model);
                    //return View("GetAllSubjectDetails", model);
                }

                model.Message = "";
                return PartialView(model);
            }

            model.Message = EduSuiteUIResources.Failed;
            return PartialView(model);

        }
      

        [HttpGet]
        public JsonResult GetEmployeesByBranchId(short? id)
        {
            InternalExamResultViewModel objViewModel = new InternalExamResultViewModel();
            objViewModel.BranchKey = id ?? 0;
            
            objViewModel = internalExamResultService.GetEmployeesByBranchId(objViewModel);
            return Json(objViewModel.Employees, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.InternalExamResult, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteInternalExamResult(long? InternalExamKey, long? InternalExamDetailsKey, long? SubjectKey)
        {
            InternalExamResultViewModel model = new InternalExamResultViewModel();

            try
            {
                model = internalExamResultService.DeleteInternalExamResult(InternalExamKey, InternalExamDetailsKey, SubjectKey);
            }
            catch (Exception)
            {
                model.Message = EduSuiteUIResources.Failed;
            }
            return Json(model);
        }


        [HttpGet]
        public JsonResult FillSearchClassDetails(short Key)
        {
            InternalExamResultViewModel model = new InternalExamResultViewModel();
            model.BranchKey = Key;
            model = internalExamResultService.FillSearchClassDetails(model);
            return Json(model.ClassDetails, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult FillSearchBatch(short? BranchKey, long? ClassDetailsKey)
        {
            InternalExamResultViewModel model = new InternalExamResultViewModel();
            model.BranchKey = BranchKey ?? 0;
            model.ClassDetailsKey = ClassDetailsKey ?? 0;
            model = internalExamResultService.FillSearchBatch(model);
            return Json(model.Batches, JsonRequestBehavior.AllowGet);
        }

        public void SendNotificationInBackground(InternalExamResultViewModel model)
        {

            Thread bgThread = new Thread(new ParameterizedThreadStart(SendNotification));

            bgThread.IsBackground = true;
            bgThread.Start(model);

        }
        private void SendNotification(Object model)
        {
            List<NotificationDataViewModel> modelList = new List<NotificationDataViewModel>();
            InternalExamResultViewModel objViewModel = (InternalExamResultViewModel)model;
            string Url = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, "");


            NotificationDataViewModel objNOtificationViewModel = new NotificationDataViewModel();
            string FilePath = Server.MapPath("~/Templates/NotificationTemplate/");
            objNOtificationViewModel.EmailTemplateName = Server.MapPath("~/Templates/NotificationTemplate/");
            objNOtificationViewModel.RowKey = 0;
            objNOtificationViewModel.AutoSMS = objViewModel.AutoSMS;
            objNOtificationViewModel.AutoEmail = objViewModel.AutoEmail;
            objNOtificationViewModel.TemplateKey = objViewModel.TemplateKey;
            objNOtificationViewModel = notificationTemplateService.GetNotificationData(objNOtificationViewModel);

            //var jsonArray = JObject.Parse(objNOtificationViewModel.NotificationData);
            //JsonSerializer jsonSerializer = new JsonSerializer();
            //ExpandoObject json = jsonSerializer.Deserialize<ExpandoObject>(new JTokenReader(jsonArray));
            FilePath = FilePath + objNOtificationViewModel.EmailTemplateName;
            foreach (InternalExamResultDetail objdetails in objViewModel.InternalExamResultDetails)
            {
                //objdetails.URL = Url;
                //CommonHelper.RegisterHandleBarHelpers();
                if (System.IO.File.Exists(FilePath) && objdetails.StudentEmail != "" && objdetails.StudentEmail != null && objViewModel.AutoEmail == true)
                {
                    //Mustache.FormatCompiler mCompiler = new Mustache.FormatCompiler();
                    //Mustache.Generator generator = mCompiler.Compile(System.IO.File.ReadAllText(FilePath));
                    //var result = generator.Render(json);
                    var template = Handlebars.Compile(System.IO.File.ReadAllText(FilePath));
                    var result = template(objdetails);
                    EmailViewModel emailModel = new EmailViewModel();
                    emailModel.EmailBody = result;
                    emailModel.EmailTo = objdetails.StudentEmail;
                    emailModel.EmailCC = objNOtificationViewModel.AdminEmailAddress;
                    emailModel.EmailSubject = objNOtificationViewModel.EmailSubject;

                    EmailHelper.SendEmail(emailModel);
                    objNOtificationViewModel.NotificationTypeKey = DbConstants.NotificationType.Email;
                    modelList.Add(objNOtificationViewModel);

                }
                if (objNOtificationViewModel.SMSTemplate != null && objdetails.MobileNumber != null && objViewModel.AutoSMS == true)
                {
                    //Mustache.FormatCompiler mCompiler = new Mustache.FormatCompiler();
                    //Mustache.Generator generator = mCompiler.Compile(System.IO.File.ReadAllText(FilePath));
                    //var result = generator.Render(json);

                    var template = Handlebars.Compile(objNOtificationViewModel.SMSTemplate);
                    var result = template(objdetails);
                    SMSViewModel smsModel = new SMSViewModel();
                    smsModel.SMSContent = result;
                    smsModel.SMSReceiptants = objdetails.MobileNumber;
                    smsModel.SMSTemplateID = objNOtificationViewModel.SMSTemplateID;
                    SMSHelper.SendSMS(smsModel);
                    var objSMSViewModel = (NotificationDataViewModel)objNOtificationViewModel;
                    objSMSViewModel.NotificationTypeKey = DbConstants.NotificationType.SMS;
                    objSMSViewModel.SMSTemplate = smsModel.SMSContent;
                    modelList.Add(objSMSViewModel);
                }

            }



            notificationTemplateService.CreateNotification(modelList);

        }
    }
}