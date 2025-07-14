using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Provider;
using CITS.EduSuite.Business.Models.Security;
using System.Web.Security;


namespace CITS.EduSuite.UI.Controllers
{
    public class EnquiryScheduleController : BaseController
    {
        private IEnquiryScheduleService enquiryScheduleService;
        private INotificationTemplateService notificationTemplateService;
        public EnquiryScheduleController(IEnquiryScheduleService objEnquiryScheduleService, INotificationTemplateService objNotificationTemplateService)
        {
            this.enquiryScheduleService = objEnquiryScheduleService;
            this.notificationTemplateService = objNotificationTemplateService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EnquirySchedule, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult ScheduleList(int? id, long? From, long? To)
        {
            EnquiryScheduleViewModel model = new EnquiryScheduleViewModel();

            model.ScheduleStatusKey = id ?? DbConstants.ScheduleStatus.Today;

            if (From != null)
            {
                model.SearchFromDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(From ?? 0).ToLocalTime();
            }

            if (To != null)
            {
                model.SearchToDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(To ?? 0).ToLocalTime();
            }
            model.ModuleKey = DbConstants.EnquiryModule.Enquiry;
            model = enquiryScheduleService.GetSearchDropDownLists(model);
            return View(model);
        }
        // GET: EnquirySchedule
        [HttpPost]
        public ActionResult EnquiryLeadScheduleList(EnquiryScheduleViewModel model)
        {
            model.SearchName = model.SearchName ?? "";
            model.SearchPhone = model.SearchPhone ?? "";
            model.SearchEmail = model.SearchEmail ?? "";
            model.SearchLocation = model.SearchLocation ?? "";

            List<EnquiryScheduleViewModel> enquiryLeadList = new List<EnquiryScheduleViewModel>();
            enquiryLeadList = enquiryScheduleService.GetEnquiryLeadSchedule(model);
            ViewBag.TotalRecords = model.TotalRecords;

            ViewBag.TodaysScheduleCount = model.TodaysScheduleCount;
            ViewBag.PendingScheduleCount = model.PendingScheduleCount;
            ViewBag.UpcomingScheduleCount = model.UpcomingScheduleCount;
            ViewBag.HistoryCount = model.HistoryCount;
            ViewBag.TomorrowCount = model.TomorrowCount;
            ViewBag.TodaysRecheduleCount = model.TodaysRecheduleCount;
            ViewBag.AllScheduleCount = model.AllScheduleCount;
            ViewBag.EnquiryScheduleCount = model.EnquiryScheduleCount;
            ViewBag.LeadScheduleCount = model.LeadScheduleCount;
            ViewBag.CouncellingScheduleCount = model.CouncellingScheduleCount;
            ViewBag.NewLeadCount = model.NewLeadCount;
            ViewBag.UnallocatedLeadCount = model.UnallocatedLeadCount;
            ViewBag.ShortlistedCount = model.ShortlistedCount;
            ViewBag.ShortlistPendingCount = model.ShortlistPendingCount;
            ViewBag.ClosedCount = model.ClosedCount;
            //ViewBag.CallDurationList = model.CallDurationList;
            //ViewBag.CallCountList = model.CallCountList;
            return PartialView(enquiryLeadList);
        }

        [HttpPost]
        public JsonResult GetScheduleSummary(EnquiryScheduleViewModel model)
        {
            model.SearchName = model.SearchName ?? "";
            model.SearchPhone = model.SearchPhone ?? "";
            model.SearchEmail = model.SearchEmail ?? "";

            EnquiryScheduleViewModel enquirySchedule = new EnquiryScheduleViewModel();
            enquirySchedule = enquiryScheduleService.GetScheduleSummary(model);
            return Json(enquirySchedule);

        }
        [HttpGet]
        public JsonResult GetCallStatusByEnquiryStatus(short? EnquiryStatusKey)
        {
            EnquiryScheduleViewModel Feedback = new EnquiryScheduleViewModel();
            Feedback.ScheduleStatusKey = EnquiryStatusKey;
            enquiryScheduleService.GetCallStatusByEnquiryStatus(Feedback);
            return Json(Feedback.ScheduleCallStatuses, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult HistoryList()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult GetHistoryList(string MobileNumber)
        {
            EnquiryScheduleViewModel model = new EnquiryScheduleViewModel();
            model.MobileNumber = MobileNumber;
            List<EnquiryScheduleViewModel> list = enquiryScheduleService.GetHistoryByMobileNumber(model);
            return PartialView(list);
        }

        [HttpPost]
        public ActionResult GetAllHistoryList(string MobileNumber, int? SearchType)
        {
            EnquiryScheduleViewModel model = new EnquiryScheduleViewModel();
            model.MobileNumber = MobileNumber;

            EnquiryScheduleViewModel list = enquiryScheduleService.GetAllHistoryByMobileNumber(model);
            if (SearchType == 1)
            {
                //SendNotification(model);
            }
            return PartialView(list);
        }

        [HttpPost]
        public JsonResult GetProductiveCallsList()
        {
            EnquiryScheduleViewModel objViewModel = new EnquiryScheduleViewModel();

            objViewModel = enquiryScheduleService.ProductiveCallsHistory(objViewModel);
            return Json(objViewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetEmployeesByBranchId(short? id)
        {
            EnquiryScheduleViewModel objViewModel = new EnquiryScheduleViewModel();
            objViewModel.BranchKey = id ?? 0;

            objViewModel = enquiryScheduleService.GetEmployeesByBranchId(objViewModel);
            SendNotification(objViewModel);
            return Json(objViewModel.Employees, JsonRequestBehavior.AllowGet);
        }

        public void SendNotification(EnquiryScheduleViewModel model)
        {

            NotificationDataViewModel notificationModel = new NotificationDataViewModel();
            NotificationHelper nofificationHelper = new NotificationHelper(notificationTemplateService);
            notificationModel.EmailTemplateName = Server.MapPath("~/UploadedFiles/NotificationTemplate/");

            notificationModel.PushNotificationUserkeys = model.UserKeys;
            notificationModel.MobileNumber = model.MobileNumber;
            notificationModel.PushNotificationTemplateKey = DbConstants.PushNotificationTemplate.MobileNumberSearch;
            notificationModel.MobileNumberSearchFeedback = model.Feedback;
            nofificationHelper.SendNotificationInBackground(notificationModel);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EnquirySchedule, ActionCode = ActionConstants.LeadTransfer)]
        [HttpGet]
        public ActionResult AllocateMultipleLead(int ScheduleSelectTypeKey, int ModuleKey)
        {
            EnquiryScheduleViewModel model = new EnquiryScheduleViewModel();
            model.ScheduleSelectTypeKey = ScheduleSelectTypeKey;
            model.ModuleKey = ModuleKey;
            model = enquiryScheduleService.GetFHDropDownLists(model);
            model.ScheduleSelectTypeKey = ScheduleSelectTypeKey;
            model.ModuleKey = ModuleKey;
            return PartialView(model);
        }
        [HttpPost]
        public JsonResult AllocateMultipleLead(EnquiryScheduleViewModel model)
        {

            enquiryScheduleService.AllocateMultipleStaff(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        //[HttpGet]
        //public ActionResult AddEditEnquiryLeadFeedback(long? id)
        //{
        //    EnquiryScheduleViewModel objViewModel = new EnquiryScheduleViewModel();
        //    EnquiryLeadFeedbackViewModel LeadFeedbackViewModel = new EnquiryLeadFeedbackViewModel();
        //    objViewModel = enquiryScheduleService.FillFeedbackDrodownLists(objViewModel);

        //    LeadFeedbackViewModel.CallTypes = objViewModel.CallTypes;
        //    LeadFeedbackViewModel.EnquiryLeadCallStatuses = objViewModel.LastLeadCallStatuses;
        //    LeadFeedbackViewModel.RowKey = Convert.ToInt32(id);

        //    LeadFeedbackViewModel.EnquiryLeadKey = id ?? 0;
        //    var Url = "~/Views/EnquiryLead/AddEditEnquiryLeadFeedback.cshtml";
        //    return PartialView(Url, LeadFeedbackViewModel);
        //}
        //[HttpPost]
        //public ActionResult AddEditEnquiryLeadFeedback(EnquiryLeadFeedbackViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (model.RowKey > 0)
        //        {
        //            model = enquiryScheduleService.CreateEnquiryLeadFeedback(model);
        //        }

        //        if (model.Message != AppConstants.Common.SUCCESS)
        //        {
        //            ModelState.AddModelError("error_msg", model.Message);
        //        }
        //        else
        //        {
        //            return Json(model);
        //        }

        //        model.Message = "";
        //        return Json(model);
        //    }

        //    model.Message = EduSuiteUIResources.Failed;
        //    return Json(model);
        //}

        #region Counceling Schedule

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.CounsellingScheduleList, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult CounsellingScheduleList(int? id, long? From, long? To)
        {
            EnquiryScheduleViewModel model = new EnquiryScheduleViewModel();
            model.ScheduleStatusKey = id ?? DbConstants.ScheduleStatus.Today;
            model = enquiryScheduleService.GetSearchDropDownLists(model);
            model.ModuleKey = DbConstants.EnquiryModule.CounsellingSchedule;
            return View(model);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.CounsellingCompletedScheduleList, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult CounsellingCompletedScheduleList(int? id, long? From, long? To)
        {
            EnquiryScheduleViewModel model = new EnquiryScheduleViewModel();
            model.ScheduleStatusKey = id ?? DbConstants.ScheduleStatus.Today;
            model = enquiryScheduleService.GetSearchDropDownLists(model);
            model.ModuleKey = DbConstants.EnquiryModule.CounsellingCompleted;
            return View(model);
        }
        #endregion 


        [HttpPost]
        public ActionResult EnquiryLeadSchedulePrint(EnquiryScheduleViewModel model)
        {
            model.SearchName = model.SearchName ?? "";
            model.SearchPhone = model.SearchPhone ?? "";
            model.SearchEmail = model.SearchEmail ?? "";
            model.SearchLocation = model.SearchLocation ?? "";

            List<EnquiryScheduleViewModel> enquiryLeadList = new List<EnquiryScheduleViewModel>();
            enquiryLeadList = enquiryScheduleService.GetEnquiryLeadSchedule(model);
            //ViewBag.TotalRecords = model.TotalRecords;

            //ViewBag.TodaysScheduleCount = model.TodaysScheduleCount;
            //ViewBag.PendingScheduleCount = model.PendingScheduleCount;
            //ViewBag.UpcomingScheduleCount = model.UpcomingScheduleCount;
            //ViewBag.HistoryCount = model.HistoryCount;
            //ViewBag.TomorrowCount = model.TomorrowCount;
            //ViewBag.TodaysRecheduleCount = model.TodaysRecheduleCount;
            //ViewBag.AllScheduleCount = model.AllScheduleCount;
            //ViewBag.EnquiryScheduleCount = model.EnquiryScheduleCount;
            //ViewBag.LeadScheduleCount = model.LeadScheduleCount;
            //ViewBag.CouncellingScheduleCount = model.CouncellingScheduleCount;
            //ViewBag.NewLeadCount = model.NewLeadCount;
            //ViewBag.UnallocatedLeadCount = model.UnallocatedLeadCount;
            //ViewBag.ShortlistedCount = model.ShortlistedCount;
            //ViewBag.ShortlistPendingCount = model.ShortlistPendingCount;
            //ViewBag.CallDurationList = model.CallDurationList;
            //ViewBag.CallCountList = model.CallCountList;
            return Json(enquiryLeadList);
        }

        [HttpGet]
        public JsonResult GetCallStatusByFHEnquiryStatus(short? EnquiryStatusKey)
        {
            EnquiryScheduleViewModel model = new EnquiryScheduleViewModel();
            model.FHEnquiryStatusKey = EnquiryStatusKey;
            model = enquiryScheduleService.FillFHCallStatuses(model);
            return Json(model.FHCallStatuses, JsonRequestBehavior.AllowGet);
        }
    }
}