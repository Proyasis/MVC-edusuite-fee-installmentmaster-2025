using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Provider;
using CITS.EduSuite.Business.Models.Security;
using System.Web.Security;
using System.Threading;
using HandlebarsDotNet;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI.Controllers
{
    [MessagesActionFilter]
    public class ExamScheduleController : BaseController
    {
        // GET: ExamScheduleList        
        private IExamScheduleService examScheduleService;
        private INotificationTemplateService notificationTemplateService;
        private ISharedService sharedService;
        private ISelectListService selectListService;
        public ExamScheduleController(IExamScheduleService objExamScheduleService, INotificationTemplateService objnotificationTemplateService,
            ISharedService objSharedService, ISelectListService objselectListService)
        {
            this.examScheduleService = objExamScheduleService;
            this.notificationTemplateService = objnotificationTemplateService;
            this.sharedService = objSharedService;
            this.selectListService = objselectListService;
        }

        public ActionResult ExamScheduleList()
        {
            ApplicationViewModel model = new ApplicationViewModel();

            model.Branches = selectListService.FillBranches();
            model.Batches = selectListService.FillSearchBatch(model.BranchKey);
            model.Courses = selectListService.FillSearchCourse(model.BranchKey);
            model.Universities = selectListService.FillSearchUniversity(model.BranchKey);
            return View(model);
        }

        [HttpPost]
        public JsonResult GetExamScheduleListdetails(string SearchText, short? BranchKey, long? CourseKey, short? UniversityKey, short? BatchKey, string sidx, string sord, int page, int rows)
        {

            long TotalRecords = 0;
            List<ExamScheduleViewModel> ExamScheduleList = new List<ExamScheduleViewModel>();
            ExamScheduleViewModel objViewModel = new ExamScheduleViewModel();

            objViewModel.SearchText = SearchText;
            objViewModel.BranchKey = BranchKey ?? 0;
            objViewModel.BatchKey = BatchKey ?? 0;
            objViewModel.CourseKey = CourseKey ?? 0;
            objViewModel.UniversityMasterKey = UniversityKey ?? 0;
            objViewModel.PageIndex = page;
            objViewModel.PageSize = rows;
            objViewModel.SortBy = sidx;
            objViewModel.SortOrder = sord;

            ExamScheduleList = examScheduleService.GetExamSchedule(objViewModel, out TotalRecords);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            var totalPages = (int)Math.Ceiling((float)TotalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = TotalRecords,
                rows = ExamScheduleList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetApplications(string ApplicantName, string MobileNumber, short? BranchKey, long? CourseKey, short? UniversityKey, short? BatchKey, string sidx, string sord, int page, int rows)
        {
            long TotalRecords = 0;
            List<ApplicationViewModel> applicationList = new List<ApplicationViewModel>();
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

            applicationList = examScheduleService.GetApplications(objViewModel, out TotalRecords);

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


        [HttpGet]
        public ActionResult AddEditExamSchedule(short? BranchKey, short? AcademicTermKey, short? CourseKey, short? UniversityMasterKey, short? BatchKey, short? ExamTermKey, short? ExamYear, short? SubjectKey)
        {

            ExamScheduleViewModel objViewModel = new ExamScheduleViewModel();
            objViewModel.BranchKey = BranchKey;
            objViewModel.AcademicTermKey = AcademicTermKey;
            objViewModel.CourseKey = CourseKey;
            objViewModel.UniversityMasterKey = UniversityMasterKey;
            objViewModel.BatchKey = BatchKey;
            objViewModel.ExamTermKey = ExamTermKey ?? 0;
            objViewModel.CourseYear = ExamYear;
            objViewModel.SubjectKey = SubjectKey;
            objViewModel = examScheduleService.GetExamScheduleById(objViewModel);
            ViewBag.ShowAcademicTerm = sharedService.ShowAcademicTerm();
            ViewBag.ShowUniversity = sharedService.ShowUniversity();
            return View(objViewModel);
        }

        [HttpPost]
        public ActionResult GetExamSchedule(ExamScheduleViewModel model)
        {
            examScheduleService.FillExamDetailsViewModel(model);

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult AddEditExamSchedule(ExamScheduleViewModel model)
        {
            if (ModelState.IsValid)
            {

                //if (model.RowKey != 0)
                //{
                model = examScheduleService.UpdateExamSchedule(model);
                //}
                //else
                //{
                //    model = examScheduleService.CreateExamSchedule(model);
                //}

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


        public JsonResult FillCourse(short key)
        {
            ExamScheduleViewModel model = new ExamScheduleViewModel();
            model.CourseTypeKey = key;
            model = examScheduleService.FillCourse(model);
            return Json(model.Courses, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FillUniversity(long key, short AcademicTermKey)
        {
            ExamScheduleViewModel model = new ExamScheduleViewModel();
            model.AcademicTermKey = AcademicTermKey;
            model.CourseKey = key;
            model = examScheduleService.FillUniversity(model);
            return Json(model.Universities, JsonRequestBehavior.AllowGet);
        }


        public JsonResult FillCourseYear(long key, short AcademicTermKey)
        {
            ExamScheduleViewModel model = new ExamScheduleViewModel();
            model.AcademicTermKey = AcademicTermKey;
            model.CourseKey = key;
            model = examScheduleService.FillCourseYear(model);
            return Json(model.CourseYears, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FillBatch(short BranchKey, short? UniversityMasterKey, long? CourseKey)
        {
            ExamScheduleViewModel model = new ExamScheduleViewModel();
            model.BranchKey = BranchKey;
            model.UniversityMasterKey = UniversityMasterKey ?? 0;
            model.CourseKey = CourseKey ?? 0;
            model = examScheduleService.FillBatch(model);
            return Json(model.Batches, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult FillSubjects(short? UniversityMasterKey, long? CourseKey, short? AcademicTermKey, short? CourseYear)
        {
            ExamScheduleViewModel model = new ExamScheduleViewModel();
            model.UniversityMasterKey = UniversityMasterKey ?? 0;
            model.CourseKey = CourseKey ?? 0;
            model.AcademicTermKey = AcademicTermKey ?? 0;
            model.CourseYear = CourseYear ?? 0;
            model = examScheduleService.FillSubjects(model);
            return Json(model.Subjects, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult ResetExamSchedule(long RowKey)
        {
            ExamScheduleViewModel objviewModel = new ExamScheduleViewModel();
            //objviewModel.StudentDivisionAllocationKey = RowKey ?? 0;
            //objviewModel.ApplicationKey = ApplicationKey ?? 0;
            try
            {
                objviewModel = examScheduleService.ResetExamSchedule(RowKey);
            }
            catch (Exception)
            {
                objviewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objviewModel);

        }

        [HttpPost]
        public ActionResult DeleteExamSchedule(short? BranchKey, short? BatchKey, short? UniversityMasterKey, long? CourseKey, short? AcademicTermKey, short? CourseYear, short? ExamTermKey, long? SubjectKey)
        {
            ExamScheduleViewModel model = new ExamScheduleViewModel();
            //model.RowKey = RowKey ?? 0;
            try
            {
                model.BatchKey = BatchKey ?? 0;
                model.UniversityMasterKey = UniversityMasterKey ?? 0;
                model.CourseKey = CourseKey ?? 0;
                model.AcademicTermKey = AcademicTermKey ?? 0;
                model.CourseYear = CourseYear ?? 0;
                model.BranchKey = BranchKey ?? 0;
                model.ExamTermKey = ExamTermKey ?? 0;
                model.SubjectKey = SubjectKey ?? 0;

                model = examScheduleService.DeleteExamSchedule(model);
            }
            catch (Exception)
            {
                model.Message = EduSuiteUIResources.Failed;
            }
            return Json(model);
        }

        [HttpGet]
        public ActionResult AddEditExamScheduleIndividual(long? Id)
        {

            ExamScheduleViewModel model = new ExamScheduleViewModel();
            model.ApplicationKey = Id ?? 0;
            model = examScheduleService.GetExamScheduleByIndividualId(model);
            ViewBag.ShowAcademicTerm = sharedService.ShowAcademicTerm();
            ViewBag.ShowUniversity = sharedService.ShowUniversity();
            return View(model);
        }

        [HttpPost]
        public ActionResult GetExamScheduleIndividual(ExamScheduleViewModel model)
        {
            examScheduleService.FillExamDetailsIndividualViewModel(model);

            return PartialView(model);
        }


        [HttpGet]
        public JsonResult FillSearchBatch(short? BranchKey)
        {
            ExamScheduleViewModel model = new ExamScheduleViewModel();
            model.BranchKey = BranchKey ?? 0;
            model.Batches = selectListService.FillBatches(model.BranchKey);
            return Json(model.Batches, JsonRequestBehavior.AllowGet);
        }

        public void SendNotificationInBackground(ExamScheduleViewModel model)
        {

            Thread bgThread = new Thread(new ParameterizedThreadStart(SendNotification));

            bgThread.IsBackground = true;
            bgThread.Start(model);

        }
        private void SendNotification(Object model)
        {
            List<NotificationDataViewModel> modelList = new List<NotificationDataViewModel>();
            ExamScheduleViewModel objViewModel = (ExamScheduleViewModel)model;
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

            //objdetails.URL = Url;
            //CommonHelper.RegisterHandleBarHelpers();
            if (System.IO.File.Exists(FilePath) && objViewModel.StudentEmail != "" && objViewModel.StudentEmail != null && objViewModel.AutoEmail == true)
            {
                //Mustache.FormatCompiler mCompiler = new Mustache.FormatCompiler();
                //Mustache.Generator generator = mCompiler.Compile(System.IO.File.ReadAllText(FilePath));
                //var result = generator.Render(json);
                var template = Handlebars.Compile(System.IO.File.ReadAllText(FilePath));
                var result = template(objViewModel);
                EmailViewModel emailModel = new EmailViewModel();
                emailModel.EmailBody = result;
                emailModel.EmailTo = objViewModel.StudentEmail;
                emailModel.EmailCC = objNOtificationViewModel.AdminEmailAddress;
                emailModel.EmailSubject = objNOtificationViewModel.EmailSubject;

                EmailHelper.SendEmail(emailModel);
                objNOtificationViewModel.NotificationTypeKey = DbConstants.NotificationType.Email;
                modelList.Add(objNOtificationViewModel);

            }
            if (objNOtificationViewModel.SMSTemplate != null && objViewModel.StudentPhone != null && objViewModel.AutoSMS == true)
            {
                //Mustache.FormatCompiler mCompiler = new Mustache.FormatCompiler();
                //Mustache.Generator generator = mCompiler.Compile(System.IO.File.ReadAllText(FilePath));
                //var result = generator.Render(json);

                var template = Handlebars.Compile(objNOtificationViewModel.SMSTemplate);
                var result = template(objViewModel);
                SMSViewModel smsModel = new SMSViewModel();
                smsModel.SMSContent = result;
                smsModel.SMSReceiptants = objViewModel.StudentPhone;
                smsModel.SMSTemplateID = objNOtificationViewModel.SMSTemplateID;
                SMSHelper.SendSMS(smsModel);
                var objSMSViewModel = (NotificationDataViewModel)objNOtificationViewModel;
                objSMSViewModel.NotificationTypeKey = DbConstants.NotificationType.SMS;
                objSMSViewModel.SMSTemplate = smsModel.SMSContent;
                modelList.Add(objSMSViewModel);
            }
        }
    }
}