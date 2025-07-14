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
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI.Controllers
{

    public class EnquiryController : BaseController
    {
        private IEnquiryService enquiryService;
        private IUniversityCourseService UniversityCourseService;
        private ICookieAuthentationProvider cookieProvider;
        private INotificationTemplateService notificationTemplateService;
        private ISharedService sharedService;
        public EnquiryController(IEnquiryService objEnquiryService,
            IUniversityCourseService objUniversityCourseService,
            ICookieAuthentationProvider objcookieProvider,
            INotificationTemplateService objnotificationTemplateService,
            ISharedService objSharedService)
        {
            this.enquiryService = objEnquiryService;
            this.UniversityCourseService = objUniversityCourseService;
            this.cookieProvider = objcookieProvider;
            this.notificationTemplateService = objnotificationTemplateService;
            this.sharedService = objSharedService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Enquiry, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult EnquiryList()
        {
            EnquiryViewModel enquiry = new EnquiryViewModel();

            enquiry = enquiryService.GetSearchDropDownLists(enquiry);
            ViewBag.ShowAcademicTerm = sharedService.ShowAcademicTerm();
            return View(enquiry);
        }

        [HttpPost]
        public ActionResult GetEnquiries(EnquiryViewModel model)
        {
            model.SearchName = model.SearchName ?? "";
            model.SearchPhone = model.SearchPhone ?? "";
            model.SearchEmail = model.SearchEmail ?? "";
            int TotalRecords;
            List<EnquiryViewModel> enquiryList = new List<EnquiryViewModel>();

            enquiryList = enquiryService.GetEnquiries(model, out TotalRecords);
            ViewBag.TotalRecords = TotalRecords;


            ViewBag.EnquiryInboxCount = model.EnquiryInboxCount;
            ViewBag.EnquiryOutboxCount = model.EnquiryOutboxCount;
            ViewBag.EnquiryProccessingCount = model.EnquiryProccessingCount;
            return PartialView(enquiryList);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EnquiryFeedback, ActionCode = ActionConstants.View)]
        [HttpGet]
        public ActionResult EnquiryFeedbackList(long? id, bool IsEditable)
        {
            List<EnquiryFeedbackViewModel> enquiryFeedbackList = enquiryService.GetEnquiryFeedbackByEnquiryId(id ?? 0);
            ViewBag.EnquiryKey = id;
            ViewBag.IsEditable = IsEditable;
            return PartialView(enquiryFeedbackList);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Enquiry, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditEnquiry(long? id, long? LeadId)
        {
            EnquiryViewModel objViewModel = new EnquiryViewModel();
            objViewModel.RowKey = id ?? 0;

            objViewModel.EnquiryLeadKey = LeadId;
            objViewModel = enquiryService.GetEnquiryById(objViewModel);
            ViewBag.ShowAcademicTerm = sharedService.ShowAcademicTerm();
            ViewBag.ShowUniversity = sharedService.ShowUniversity();
            if (LeadId == null)
            {
                objViewModel.EnquiryLeadKey = objViewModel.EnquiryLeadKey;
            }
            else
            {
                objViewModel.EnquiryLeadKey = LeadId;
            }
            return View(objViewModel);
        }

        [HttpPost]
        public ActionResult AddEditEnquiry(EnquiryViewModel model)
        {

            var errors = ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList();

            if (ModelState.IsValid)
            {

                if (model.RowKey == 0)
                {
                    model = enquiryService.CreateEnquiry(model);
                }
                else
                {
                    model = enquiryService.UpdateEnquiry(model);
                }

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
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

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EnquiryFeedback, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditEnquiryCounsellingCompletedFeedback(long? id, long? EnquiryKey)
        {
            EnquiryFeedbackViewModel objViewModel = new EnquiryFeedbackViewModel();
            objViewModel.RowKey = id ?? 0;
            objViewModel.EnquiryKey = EnquiryKey ?? 0;
            objViewModel.ModuleKey = DbConstants.EnquiryModule.CounsellingCompleted;
            objViewModel = enquiryService.GetEnquiryFeedbackById(objViewModel);
            objViewModel.EnquiryKey = EnquiryKey ?? 0;
            return PartialView("AddEditEnquiryFeedback", objViewModel);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EnquiryFeedback, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditEnquiryFeedback(long? id, long? EnquiryKey)
        {
            EnquiryFeedbackViewModel objViewModel = new EnquiryFeedbackViewModel();
            objViewModel.RowKey = id ?? 0;
            objViewModel.EnquiryKey = EnquiryKey ?? 0;
            objViewModel.ModuleKey = DbConstants.EnquiryModule.Enquiry;
            objViewModel = enquiryService.GetEnquiryFeedbackById(objViewModel);
            objViewModel.EnquiryKey = EnquiryKey ?? 0;
            return PartialView(objViewModel);
        }

        [HttpPost]
        public JsonResult AddEditEnquiryFeedback(EnquiryFeedbackViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = enquiryService.CreateEnquiryFeedback(model);
                }
                else
                {
                    model = enquiryService.UpdateEnquiryFeedback(model);
                }

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                    Toastr.AddToastMessage(AppConstants.Common.FAILED, model.Message, ToastType.Error);
                }
                else
                {
                    SendNotification(model);

                    //if (model.NotificationEmails != null)
                    //{
                    //    string[] Emails = model.NotificationEmails.Split('|');
                    //}

                    //if (model.NotificationMobileNo != null)
                    //{
                    //    string[] Mobiles = model.NotificationMobileNo.Split('|');
                    //}

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

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Enquiry, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteEnquiry(Int16 id)
        {
            EnquiryViewModel objViewModel = new EnquiryViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = enquiryService.DeleteEnquiry(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EnquiryFeedback, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteEnquiryFeedback(Int16 id)
        {
            EnquiryFeedbackViewModel objViewModel = new EnquiryFeedbackViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = enquiryService.DeleteEnquiryFeeback(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }

        [HttpGet]
        public JsonResult GetDepartmentsByBranchId(short? id)
        {
            EnquiryViewModel objViewModel = new EnquiryViewModel();
            objViewModel.BranchKey = id ?? 0;

            objViewModel = enquiryService.GetDepartmentByBranchId(objViewModel);
            return Json(objViewModel, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult GetCourseTypeByAcademicTerm(short? AcademicTermKey, short? CountryKey)
        {
            EnquiryViewModel objViewModel = new EnquiryViewModel();
            objViewModel.CountryKey = CountryKey ?? 0;
            objViewModel.AcademicTermKey = AcademicTermKey ?? 0;
            objViewModel = enquiryService.GetCourseTypeByAcademicTerm(objViewModel);
            return Json(objViewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCountryByAcademicTerm(short AcademicTermKey)
        {
            EnquiryViewModel objViewModel = new EnquiryViewModel();
            objViewModel.AcademicTermKey = AcademicTermKey;

            objViewModel = enquiryService.GetCountryByAcademicTerm(objViewModel);
            return Json(objViewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCourseByCourseType(byte? CourseTypeKey, short? UniversityKey, short? AcademicTermKey)
        {
            EnquiryViewModel objViewModel = new EnquiryViewModel();
            objViewModel.CourseTypeKey = CourseTypeKey ?? 0;
            objViewModel.UniversityKey = UniversityKey ?? 0;
            objViewModel.AcademicTermKey = AcademicTermKey ?? 0;
            objViewModel = enquiryService.GetCourseByCourseType(objViewModel);
            return Json(objViewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetUniversityByCourse(short? AcademicTermKey, short? CourseKey)
        {
            EnquiryViewModel objViewModel = new EnquiryViewModel();
            objViewModel.CourseKey = CourseKey ?? 0;
            objViewModel.AcademicTermKey = AcademicTermKey ?? 0;
            objViewModel = enquiryService.GetUniversityByCourse(objViewModel);
            return Json(objViewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CheckMobileNumberExists(string MobileNumber, long rowKey, long? EnquiryLeadKey)
        {
            EnquiryViewModel enquiry = new EnquiryViewModel();
            enquiry = enquiryService.CheckMobileNumberExists(MobileNumber, rowKey, EnquiryLeadKey);
            return Json(enquiry.IsSuccessful, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult CheckEmailAddressExists(string EmailAddress, long rowKey, long? EnquiryLeadKey)
        {
            EnquiryViewModel enquiry = new EnquiryViewModel();
            enquiry = enquiryService.CheckEmailAddressExists(EmailAddress, rowKey, EnquiryLeadKey);
            return Json(enquiry.IsSuccessful, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CheckCounsellingTimeExists(string ConcellingTimeKey, long RowKey, string NextCallSchedule)
        {
            EnquiryViewModel enquiry = new EnquiryViewModel();
            enquiry.ConcellingTimeKey = ConcellingTimeKey;
            enquiry.RowKey = RowKey;
            enquiry.NextCallSchedule = NextCallSchedule != null && NextCallSchedule.Trim() != "" ? DateTime.ParseExact(NextCallSchedule, "dd/MM/yyyy", null) : enquiry.NextCallSchedule;

            enquiry = enquiryService.CheckCounsellingTimeExists(enquiry);
            return Json(enquiry.IsSuccessful, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCallStatusByEnquiryStatus(short EnquiryStatusKey)
        {
            EnquiryViewModel enquiry = new EnquiryViewModel();
            enquiry.EnquiryStatusKey = EnquiryStatusKey;
            enquiry.ModuleKey = DbConstants.EnquiryModule.Enquiry;
            enquiryService.GetCallStatusByEnquiryStatus(enquiry);
            return Json(enquiry.CallStatuses, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetEmployeesByBranchId(short? id)
        {
            EnquiryViewModel objViewModel = new EnquiryViewModel();
            objViewModel.BranchKey = id ?? 0;

            objViewModel = enquiryService.GetEmployeesByBranchId(objViewModel);
            return Json(objViewModel.Employees, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetEnquiryLead(string SearchText)
        {
            EnquiryLeadViewModel objViewModel = new EnquiryLeadViewModel();
            List<EnquiryLeadViewModel> List = new List<EnquiryLeadViewModel>();
            objViewModel.SearchText = SearchText;

            List = enquiryService.GetEnquiryLead(objViewModel);
            return Json(List, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CheckCallStatusDuration(int? Id)
        {
            EnquiryFeedbackViewModel ObjViewModel = new EnquiryFeedbackViewModel();
            ObjViewModel.EnquiryCallStatusKey = Id ?? 0;
            ObjViewModel = enquiryService.CheckCallStatusDuration(ObjViewModel);
            return Json(ObjViewModel != null ? ObjViewModel.IsDuration : false, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Enquiry, ActionCode = ActionConstants.FeeStructure)]
        [EncryptedActionParameter]
        [HttpGet]
        public ActionResult FeeStructure(short? AcademicTermKey, short? CountryKey, short? UniversityKey, short? CourseTypeKey, int? CourseKey)
        {
            //UniversityCourseViewModel model = new UniversityCourseViewModel();
            //model.AcademicTermKey = AcademicTermKey ?? 0;
            //model.CountryKey = CountryKey ?? 0;
            //model.UniversityMasterKey = UniversityKey ?? 0;
            //model.CourseTypeKey = CourseTypeKey ?? 0;
            //model.CourseKey = CourseKey ?? 0;
            //model.Duration = ProgramDurationKey ?? 0;
            //var modelList = UniversityCourseService.ViewFeeStructure(model);
            //var Url = "~/Views/UniversityProgram/ViewUniversityProgram.cshtml";
            //return PartialView(Url, modelList);
            return PartialView();
        }

        [HttpGet]
        public JsonResult GetEnquiry(string SearchText)
        {
            EnquiryViewModel objViewModel = new EnquiryViewModel();
            List<EnquiryViewModel> List = new List<EnquiryViewModel>();
            objViewModel.SearchText = SearchText;

            List = enquiryService.GetEnquiry(objViewModel);
            return Json(List, JsonRequestBehavior.AllowGet);
        }

        public void SendNotification(EnquiryFeedbackViewModel model)
        {

            List<NotificationDataViewModel> notificationModel = new List<NotificationDataViewModel>();
            // NotificationHelper nofificationHelper = new NotificationHelper(notificationTemplateService);

            if (model.IsClosedNotification)
            {

                notificationModel.Add(new NotificationDataViewModel
                {

                    PushNotificationUserkeys = model.UserKeys,
                    // PushNotificationTemplateKey = DbConstants.PushNotificationTemplate.LeadEmployeeLocked
                });

                //NotificationHub.PushUserLogout(new List<int>() { model.UserKey });
            }



            // nofificationHelper.SendMultipleNotificationInBackground(notificationModel);
        }

        [HttpGet]
        public JsonResult GetProgramDuration(byte? AcademicTermKey, byte? ProgramTypeKey, short? UniversityKey)
        {
            EnquiryViewModel objViewModel = new EnquiryViewModel();
            objViewModel.AcademicTermKey = AcademicTermKey ?? 0;
            objViewModel.CourseKey = ProgramTypeKey ?? 0;
            objViewModel.UniversityKey = UniversityKey ?? 0;
            objViewModel = enquiryService.FillCourseDuration(objViewModel);
            return Json(objViewModel.CourseDuration, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetEmployeeByBranchId(short? id)
        {
            EnquiryViewModel objViewModel = new EnquiryViewModel();
            objViewModel.BranchKey = id ?? 0;

            objViewModel = enquiryService.GetEmployeesByBranchId(objViewModel);
            return Json(objViewModel.Employees, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult GetPhoneNumberLength(short? TelephoneCodeKey, short? TelephoneCodeOptionalKey)
        {
            EnquiryViewModel model = new EnquiryViewModel();
            model.TelephoneCodeKey = TelephoneCodeKey ?? 0;
            model.TelephoneCodeOptionalKey = TelephoneCodeOptionalKey;
            model = enquiryService.GetPhoneNumberLength(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

    }
}