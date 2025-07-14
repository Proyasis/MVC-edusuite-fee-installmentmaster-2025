using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Interfaces;

using CITS.EduSuite.Business.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Provider;
using System.Web.Security;
using CITS.EduSuite.Business.Models.Security;
using HandlebarsDotNet;
using System.Threading;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Dynamic;
using System.Text;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI.Controllers
{
    public class AttendanceController : BaseController
    {
        public IAttendanceService attendanceService;
        private INotificationTemplateService notificationTemplateService;
        public AttendanceController(IAttendanceService objAttendanceService, INotificationTemplateService objnotificationTemplateService)
        {
            this.attendanceService = objAttendanceService;
            this.notificationTemplateService = objnotificationTemplateService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentAttendance, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult AttendanceList()
        {
            AttendanceViewModel objViewModel = new AttendanceViewModel();

            objViewModel = attendanceService.GetSearchDropdownList(objViewModel);
            objViewModel.AttendanceDate = DateTimeUTC.Now;
            objViewModel.SearchFromDate = DateTimeUTC.Now;
            return View(objViewModel);
        }

        [HttpPost]
        public ActionResult GetAttendance(AttendanceViewModel model)
        {

            model = attendanceService.FillAttendanceDetailsViewModel(model);
            if (model.Message != null && model.Message != AppConstants.Common.SUCCESS)
            {
                ModelState.AddModelError("error_msg", model.Message);
            }
            return PartialView(model);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentAttendance, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditAttendance(long? Id)
        {
            AttendanceViewModel model = new AttendanceViewModel();

            model.RowKey = Id ?? 0;


            model = attendanceService.GetAttendanceById(model);
            return View(model);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentAttendance, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult AddEditAttendanceSubmit(AttendanceViewModel model)
        {
            if (ModelState.IsValid)
            {
                //GetUserKey(model);
                //if (model.RowKey != 0)
                //{
                model = attendanceService.UpdateAttendance(model);
                //}
                //else
                //{
                //    model = attendanceService.CreateAttendance(model);
                //}

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    SendNotificationInBackground(model);
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

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentAttendance, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteAttendance(long? RowKey)
        {
            AttendanceViewModel model = new AttendanceViewModel();

            model.RowKey = RowKey ?? 0;

            try
            {
                model = attendanceService.DeleteAttendance(model);
            }
            catch (Exception)
            {
                model.Message = EduSuiteUIResources.Failed;
            }
            return Json(model);
        }

        [HttpPost]
        public JsonResult GetAttendancedetails(string SearchText, string SearchFromDate, string SearchDate, short? BranchKey, long? ClassDetailsKey, short? BatchKey, byte? AttendanceStatusKey, string sidx, string sord, int page, int rows)
        {
            long TotalRecords = 0;
            AttendanceViewModel model = new AttendanceViewModel();
            List<AttendanceViewModel> AttendanceList = new List<AttendanceViewModel>();
            model.SearchText = SearchText;
            model.AttendanceDate = SearchDate != "" ? DateTime.ParseExact(SearchDate, "dd/MM/yyyy", null) : DateTimeUTC.Now;
            model.SearchFromDate = SearchFromDate != "" ? DateTime.ParseExact(SearchFromDate, "dd/MM/yyyy", null) : DateTimeUTC.Now;
            model.BranchKey = BranchKey ?? 0;
            model.ClassDetailsKey = ClassDetailsKey ?? 0;
            model.BatchKey = BatchKey ?? 0;
            model.AttendanceStatusKey = AttendanceStatusKey ?? 0;
            model.PageIndex = page;
            model.PageSize = rows;
            model.SortBy = sidx;
            model.SortOrder = sord;
            AttendanceList = attendanceService.GetAttendance(model, out TotalRecords);
            int siNo = ((page - 1) * rows) + 1;
            foreach (AttendanceViewModel item in AttendanceList)
            {
                item.SlNo = siNo++;
            }
            int pageindex = Convert.ToInt32(page) - 1;
            int pagesize = rows;
            var totalpage = (int)Math.Ceiling((float)TotalRecords / (float)rows);

            var jsonData = new
            {
                total = totalpage,
                page,
                records = TotalRecords,
                rows = AttendanceList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AttendanceQuikList()
        {
            return View();
        }

        [HttpPost]
        public JsonResult CheckAttendanceBlocked(long RowKey, long ApplicationKey, short AttendanceTypeKey, DateTime AttendanceDate)
        {
            return Json(attendanceService.CheckAttendanceBlocked(RowKey, ApplicationKey, AttendanceTypeKey, AttendanceDate));
        }

        #region DropDownChange Events

        [HttpGet]
        public JsonResult FillClassDetails(short? Branchkey)
        {
            AttendanceViewModel model = new AttendanceViewModel();


            model.BranchKey = Branchkey ?? 0;
            model = attendanceService.FillClassDetails(model);
            return Json(model.ClassDetails, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FillBatch(short BranchKey, long ClassDetailsKey)
        {
            AttendanceViewModel model = new AttendanceViewModel();

            model.BranchKey = BranchKey;
            model.ClassDetailsKey = ClassDetailsKey;
            model = attendanceService.FillBatch(model);
            return Json(model.Batches, JsonRequestBehavior.AllowGet);
        }

        #endregion DropDownChange Events

        public void SendNotificationInBackground(AttendanceViewModel model)
        {

            Thread bgThread = new Thread(new ParameterizedThreadStart(SendNotification));

            bgThread.IsBackground = true;
            bgThread.Start(model);

        }
        private void SendNotification(Object model)
        {
            List<NotificationDataViewModel> modelList = new List<NotificationDataViewModel>();
            AttendanceViewModel objViewModel = (AttendanceViewModel)model;
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

            foreach (AttendanceDetailsViewModel objdetails in objViewModel.AttendanceDetails)
            {

                long? AttendanceDetailsRowkey = 0;
                byte AttendanceStatusKey = 0;
                foreach (AttendanceStatusDetailsViewModel details in objdetails.AttendanceStatusDetailsViewModel)
                {
                    AttendanceDetailsRowkey = details.AttendanceDetailRowKey;
                    AttendanceStatusKey = details.AttendanceStatusKey;
                }
                if (AttendanceStatusKey == DbConstants.AttendanceStatus.Absent)
                {

                    objNOtificationViewModel.RowKey = AttendanceDetailsRowkey ?? 0;

                    objNOtificationViewModel.PushNotificationTemplateKey = DbConstants.PushNotificationTemplate.StudentAttendance;

                    objPushNotificationModel = notificationTemplateService.GetPushNotificationData(objNOtificationViewModel);
                    if (objPushNotificationModel.PushNotificationContent != null && objPushNotificationModel.NotificationData != "" && objPushNotificationModel.NotificationData != null)
                    {
                        var jsonArray = JObject.Parse(objPushNotificationModel.NotificationData);
                        JsonSerializer jsonSerializere = new JsonSerializer();
                        jsons = jsonSerializere.Deserialize<ExpandoObject>(new JTokenReader(jsonArray));



                    }
                    if (IsEmail || IsSMS)
                    {
                        objNOtificationViewModel.TemplateKey = DbConstants.NotificationTemplate.StudentAttendance;

                        objNOtificationViewModel = notificationTemplateService.GetNotificationData(objNOtificationViewModel);
                        var jsonArray = JObject.Parse(objNOtificationViewModel.NotificationData);
                        JsonSerializer jsonSerializer = new JsonSerializer();
                        ExpandoObject json = jsonSerializer.Deserialize<ExpandoObject>(new JTokenReader(jsonArray));


                        if (System.IO.File.Exists(FilePath) && objdetails.StudentEmail != "" && objdetails.StudentEmail != null && objViewModel.AutoEmail == true)
                        {

                            var template = Handlebars.Compile(System.IO.File.ReadAllText(FilePath));
                            var result = template(json);
                            EmailViewModel emailModel = new EmailViewModel();
                            emailModel.EmailBody = result;
                            emailModel.EmailTo = objdetails.StudentEmail;
                            emailModel.EmailCC = objNOtificationViewModel.AdminEmailAddress;
                            emailModel.EmailSubject = objNOtificationViewModel.EmailSubject;

                            EmailHelper.SendEmail(emailModel);
                            objNOtificationViewModel.NotificationTypeKey = DbConstants.NotificationType.Email;
                            modelList.Add(objNOtificationViewModel);

                        }
                        if (objNOtificationViewModel.GuardianSMSTemplate != null && objdetails.GuardianMobileNumber != null && objViewModel.AutoSMS == true && objNOtificationViewModel.GuardianSMS == true)
                        {

                            var template = Handlebars.Compile(objNOtificationViewModel.GuardianSMSTemplate);
                            var result = template(json);
                            SMSViewModel smsModel = new SMSViewModel();
                            smsModel.SMSContent = result;
                            smsModel.SMSReceiptants = objdetails.GuardianMobileNumber;
                            smsModel.SMSTemplateID = objNOtificationViewModel.SMSTemplateID;
                            SMSHelper.SendSMS(smsModel);
                            var objSMSViewModel = (NotificationDataViewModel)objNOtificationViewModel;
                            objSMSViewModel.NotificationTypeKey = DbConstants.NotificationType.SMS;
                            objSMSViewModel.SMSTemplate = smsModel.SMSContent;
                            modelList.Add(objSMSViewModel);
                        }
                        if (objNOtificationViewModel.SMSTemplate != null && objdetails.MobileNumber != null && objViewModel.AutoSMS == true)
                        {

                            var template = Handlebars.Compile(objNOtificationViewModel.SMSTemplate);
                            var result = template(json);
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


        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentAttendance, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteAttendanceBulk(string Keys)
        {
            AttendanceViewModel model = new AttendanceViewModel();
            List<long> RowKeys = new List<long>();
            if (Keys != null)
            {
                Keys = String.IsNullOrEmpty((Keys ?? "0")) ? "0" : (Keys ?? "0");
                RowKeys = (Keys).Split(',').Select(row => Int64.Parse(row)).ToList();

            }

            model = attendanceService.DeleteBulkAttendance(model, RowKeys);

            return Json(model);
        }
    }
}