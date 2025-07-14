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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Dynamic;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI.Controllers
{
    public class InternalExamScheduleController : BaseController
    {
        public IInternalExamScheduleService internalExamService;
        private ISharedService sharedService;
        private INotificationTemplateService notificationTemplateService;
        public InternalExamScheduleController(IInternalExamScheduleService objInternalExamService,
            ISharedService objSharedService, INotificationTemplateService objnotificationTemplateService)
        {
            this.internalExamService = objInternalExamService;
            this.sharedService = objSharedService;
            this.notificationTemplateService = objnotificationTemplateService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.InternalExam, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult InternalExamScheduleList()
        {
            InternalExamViewModel objViewModel = new InternalExamViewModel();
            
            objViewModel = internalExamService.GetSearchDropdownList(objViewModel);
            return View(objViewModel);
        }

        [HttpPost]
        public JsonResult GetInternalExamScheduleListdetails(string SearchText, short? BranchKey, short? BatchKey)
        {
            int page = 1; int rows = 15;
            List<InternalExamViewModel> InternalExamScheduleList = new List<InternalExamViewModel>();
            InternalExamViewModel model = new InternalExamViewModel();
            model.SearchText = SearchText;
            model.BranchKey = BranchKey ?? 0;
            model.BatchKey = BatchKey ?? 0;


            InternalExamScheduleList = internalExamService.GetInternalExamSchedule(model);
            int pageindex = Convert.ToInt32(page) - 1;
            int pagesize = rows;
            int totalrecords = InternalExamScheduleList.Count();
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)rows);

            var jsonData = new
            {
                total = totalpage,
                page,
                records = totalrecords,
                rows = InternalExamScheduleList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.InternalExam, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditInternalExamSchedule(long? Id)
        {

            InternalExamViewModel model = new InternalExamViewModel();
            
            model.RowKey = Id ?? 0;
            model = internalExamService.GetInternalExamScheduleById(model);
            ViewBag.ShowAcademicTerm = sharedService.ShowAcademicTerm();
            ViewBag.ShowUniversity = sharedService.ShowUniversity();
            return View(model);
        }

        [HttpPost]
        public ActionResult GetInternalExamSchedule(InternalExamViewModel model)
        {
            internalExamService.FillInternalExamDetailsViewModel(model);

            return PartialView(model);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.InternalExam, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult AddEditInternalExamScheduleSubmit(InternalExamViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                if (model.RowKey != 0)
                {
                    model = internalExamService.UpdateInternalExamSchedule(model);
                }
                else
                {
                    model = internalExamService.CreateInternalExamSchedule(model);
                }

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    if (model.AutoEmail == true || model.AutoSMS == true)
                    {
                        SendNotificationInBackground(model);
                    }
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

        //public JsonResult FillCourseType(short key)
        //{
        //    InternalExamViewModel model = new InternalExamViewModel();
        //    model.AcademicTermKey = key;
        //    model = internalExamService.FillCourseType(model);
        //    return Json(model.CourseTypes, JsonRequestBehavior.AllowGet);
        //}


        public JsonResult FillCourse(short key)
        {
            InternalExamViewModel model = new InternalExamViewModel();
            model.CourseTypeKey = key;
            model = internalExamService.FillCourse(model);
            return Json(model.Courses, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FillUniversity(long key, short AcademicTermKey)
        {
            InternalExamViewModel model = new InternalExamViewModel();
            model.AcademicTermKey = AcademicTermKey;
            model.CourseKey = key;
            model = internalExamService.FillUniversity(model);
            return Json(model.Universities, JsonRequestBehavior.AllowGet);
        }


        public JsonResult FillCourseYear(long key, short AcademicTermKey)
        {
            InternalExamViewModel model = new InternalExamViewModel();
            model.AcademicTermKey = AcademicTermKey;
            model.CourseKey = key;
            model = internalExamService.FillCourseYear(model);
            return Json(model.CourseYears, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FillBatch(short BranchKey, short? UniversityMasterKey, long? CourseKey)
        {
            InternalExamViewModel model = new InternalExamViewModel();
            model.BranchKey = BranchKey;
            model.UniversityMasterKey = UniversityMasterKey ?? 0;
            model.CourseKey = CourseKey ?? 0;
            model = internalExamService.FillBatch(model);
            return Json(model.Batches, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult FillClassDetails(short? UniversityMasterKey, long? CourseKey, short? AcademicTermKey, short? CourseYear)
        {
            InternalExamViewModel model = new InternalExamViewModel();
            model.UniversityMasterKey = UniversityMasterKey ?? 0;
            model.CourseKey = CourseKey ?? 0;
            model.AcademicTermKey = AcademicTermKey ?? 0;
            model.CourseYear = CourseYear ?? 0;
            model = internalExamService.FillClassDetails(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.InternalExam, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult ResetInternalExamSchedule(long RowKey, long InternalExamKey)
        {
            InternalExamViewModel objviewModel = new InternalExamViewModel();
            //objviewModel.StudentDivisionAllocationKey = RowKey ?? 0;
            //objviewModel.ApplicationKey = ApplicationKey ?? 0;
            try
            {
                objviewModel = internalExamService.ResetInternalExamSchedule(RowKey, InternalExamKey);
            }
            catch (Exception)
            {
                objviewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objviewModel);

        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.InternalExam, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteInternalExamSchedule(long? RowKey)
        {
            InternalExamViewModel model = new InternalExamViewModel();
            model.RowKey = RowKey ?? 0;
            try
            {
                model = internalExamService.DeleteInternalExamSchedule(model);
            }
            catch (Exception)
            {
                model.Message = EduSuiteUIResources.Failed;
            }
            return Json(model);
        }
       


        [HttpGet]
        public JsonResult FillSearchBatch(short? BranchKey)
        {
            InternalExamViewModel model = new InternalExamViewModel();
            model.BranchKey = BranchKey ?? 0;
            model = internalExamService.FillSearchBatch(model);
            return Json(model.Batches, JsonRequestBehavior.AllowGet);
        }


        public void SendNotificationInBackground(InternalExamViewModel model)
        {

            Thread bgThread = new Thread(new ParameterizedThreadStart(SendNotification));

            bgThread.IsBackground = true;
            bgThread.Start(model);

        }
        private void SendNotification(Object model)
        {
            List<NotificationDataViewModel> modelList = new List<NotificationDataViewModel>();
            InternalExamViewModel objViewModel = (InternalExamViewModel)model;
            string Url = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, "");


            NotificationDataViewModel objNOtificationViewModel = new NotificationDataViewModel();
            string FilePath = Server.MapPath("~/Templates/NotificationTemplate/");
            objNOtificationViewModel.EmailTemplateName = Server.MapPath("~/Templates/NotificationTemplate/");
            objNOtificationViewModel.RowKey = objViewModel.RowKey;
            objNOtificationViewModel.AutoSMS = objViewModel.AutoSMS;
            objNOtificationViewModel.AutoEmail = objViewModel.AutoEmail;
            objNOtificationViewModel.TemplateKey = objViewModel.TemplateKey;
            objNOtificationViewModel = notificationTemplateService.GetNotificationData(objNOtificationViewModel);

            var jsonArray = JObject.Parse(objNOtificationViewModel.NotificationData);
            JsonSerializer jsonSerializer = new JsonSerializer();
            ExpandoObject json = jsonSerializer.Deserialize<ExpandoObject>(new JTokenReader(jsonArray));

            FilePath = FilePath + objNOtificationViewModel.EmailTemplateName;

            List<string> StudentsEmail = new List<string>();
            List<string> StudentsPhone = new List<string>();
            foreach (StudetMailDetails objstudentDetils in objViewModel.StudetMailDetails)
            {
                if (objstudentDetils.StudentEmail != null && objstudentDetils.StudentEmail != "")
                {
                    StudentsEmail.Add(objstudentDetils.StudentEmail);
                }
                if (objstudentDetils.StudentPhone != null && objstudentDetils.StudentPhone != "")
                {
                    StudentsPhone.Add(objstudentDetils.StudentPhone);
                }
            }
            //foreach (AttendanceDetailsViewModel objdetails in objViewModel.AttendanceDetails.Where(x => x.AttendanceStatus == false))
            //{
            //objdetails.URL = Url;

            if (System.IO.File.Exists(FilePath) && StudentsEmail.Count > 0 && objViewModel.AutoEmail == true)
            {
                //Mustache.FormatCompiler mCompiler = new Mustache.FormatCompiler();
                //Mustache.Generator generator = mCompiler.Compile(System.IO.File.ReadAllText(FilePath));
                //var result = generator.Render(json);
                var template = Handlebars.Compile(System.IO.File.ReadAllText(FilePath));
                var result = template(json);
                EmailViewModel emailModel = new EmailViewModel();
                emailModel.EmailBody = result;
                emailModel.EmailTolist = StudentsEmail;
                emailModel.EmailCC = objNOtificationViewModel.AdminEmailAddress;
                emailModel.EmailSubject = objNOtificationViewModel.EmailSubject;

                EmailHelper.SendEmailMultiple(emailModel);
                objNOtificationViewModel.NotificationTypeKey = DbConstants.NotificationType.Email;
                modelList.Add(objNOtificationViewModel);

            }
            if (objNOtificationViewModel.SMSTemplate != null && StudentsPhone.Count > 0 && objViewModel.AutoSMS == true)
            {
                //Mustache.FormatCompiler mCompiler = new Mustache.FormatCompiler();
                //Mustache.Generator generator = mCompiler.Compile(System.IO.File.ReadAllText(FilePath));
                //var result = generator.Render(json);
                var template = Handlebars.Compile(objNOtificationViewModel.SMSTemplate);
                var result = template(json);
                SMSViewModel smsModel = new SMSViewModel();
                smsModel.SMSContent = result;
                smsModel.SMSReceiptants = string.Join(",", StudentsPhone);
                SMSHelper.SendSMS(smsModel);
                var objSMSViewModel = (NotificationDataViewModel)objNOtificationViewModel;
                objSMSViewModel.NotificationTypeKey = DbConstants.NotificationType.SMS;
                objSMSViewModel.SMSTemplate = smsModel.SMSContent;
                modelList.Add(objSMSViewModel);
            }

            //}

        }
    }
}