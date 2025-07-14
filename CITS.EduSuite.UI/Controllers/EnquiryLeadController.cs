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

using CITS.EduSuite;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI.Controllers
{
    public class EnquiryLeadController : BaseController
    {
        private IEnquiryLeadService enquiryLeadService;
        private ICookieAuthentationProvider cookieProvider;
        private INotificationTemplateService notificationTemplateService;

        public EnquiryLeadController(IEnquiryLeadService objEnquiryLeadService, ICookieAuthentationProvider objcookieProvider, INotificationTemplateService objnotificationTemplateService)
        {
            this.enquiryLeadService = objEnquiryLeadService;
            this.cookieProvider = objcookieProvider;
            this.notificationTemplateService = objnotificationTemplateService;
        }

        [HttpGet]
        public JsonResult GetLeadsAllocation(string SearchText, short? SearchBranchKey, byte? IsNewLead, short? SearchLeadStatusKey, string SearchDateAddedFrom, string SearchDateAddedTo, string sidx, string sord, int page, int rows)
        {
            long TotalRecords = 0;

            EnquiryLeadViewModel objViewModel = new EnquiryLeadViewModel();

            objViewModel.PageIndex = page;
            objViewModel.PageSize = rows;
            objViewModel.SortBy = sidx;
            objViewModel.SortOrder = sord;
            objViewModel.SearchText = SearchText;
            objViewModel.SearchBranchKey = SearchBranchKey;
            objViewModel.IsNewLead = IsNewLead;
            objViewModel.EnquiryLeadStatusKey = SearchLeadStatusKey;
            objViewModel.SearchFromDate = SearchDateAddedFrom == "" ? objViewModel.SearchFromDate : Convert.ToDateTime(SearchDateAddedFrom);
            objViewModel.SearchToDate = SearchDateAddedTo == "" ? objViewModel.SearchToDate : Convert.ToDateTime(SearchDateAddedTo);

            objViewModel = enquiryLeadService.GetLeadsAllocation(objViewModel);
            TotalRecords = objViewModel.TotalRecords;

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            var totalPages = (int)Math.Ceiling((float)TotalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = TotalRecords,
                rows = objViewModel.LeadsList
            };
            return Json(jsonData, System.Web.Mvc.JsonRequestBehavior.AllowGet);
        }


        [ActionAuthenticationAttribute(MenuCode = MenuConstants.LeadAllocation, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult LeadsList()
        {
            EnquiryLeadViewModel enquiryLead = new EnquiryLeadViewModel();

            enquiryLead = enquiryLeadService.GetSearchDropDownLists(enquiryLead);

            return View(enquiryLead);

        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.LeadAllocation, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditLeadAllocation()
        {
            EnquiryLeadViewModel enquiryLead = new EnquiryLeadViewModel();
            enquiryLeadService.GetSearchDropDownLists(enquiryLead);
            return PartialView(enquiryLead);
        }


        [ActionAuthenticationAttribute(MenuCode = MenuConstants.LeadAllocation, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult AddEditLeadAllocation(EnquiryLeadViewModel model)
        {
            enquiryLeadService.AllocateNewLeads(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EnquiryLead, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult EnquiryLeadList()
        {
            EnquiryLeadViewModel enquiryLead = new EnquiryLeadViewModel();

            enquiryLead = enquiryLeadService.GetSearchDropDownLists(enquiryLead);
            return View(enquiryLead);
        }

        [HttpPost]
        public ActionResult GetEnquiryLeads(EnquiryLeadViewModel model)
        {
            model.SearchName = model.SearchName ?? "";
            model.SearchPhone = model.SearchPhone ?? "";
            model.SearchEmail = model.SearchEmail ?? "";

            int TotalRecords;
            List<EnquiryLeadViewModel> enquiryLeadList = new List<EnquiryLeadViewModel>();
            enquiryLeadList = enquiryLeadService.GetEnquiryLeads(model, out TotalRecords);
            ViewBag.TotalRecords = TotalRecords;
            return PartialView(enquiryLeadList);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EnquiryLeadFeedback, ActionCode = ActionConstants.View)]
        [HttpGet]
        public ActionResult EnquiryLeadFeedbackList(long id, bool IsEditable)
        {
            List<EnquiryLeadFeedbackViewModel> enquiryLeadFeedbackList = enquiryLeadService.GetEnquiryLeadFeedbackByLeadId(id);
            ViewBag.EnquiryLeadKey = id;
            ViewBag.IsEditable = IsEditable;
            return PartialView(enquiryLeadFeedbackList);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EnquiryLead, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditEnquiryLead(long? id)
        {
            EnquiryLeadViewModel objViewModel = new EnquiryLeadViewModel();
            objViewModel.RowKey = id ?? 0;

            objViewModel = enquiryLeadService.GetEnquiryLeadById(objViewModel);
            return View(objViewModel);
        }

        [HttpPost]
        public ActionResult AddEditEnquiryLead(EnquiryLeadViewModel model)
        {

            if (ModelState.IsValid)
            {
                List<EnquiryLeadViewModel> modelList = new List<EnquiryLeadViewModel>();
                modelList.Add(model);
                model = enquiryLeadService.UpdateEnquiryLeads(modelList);


                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    return RedirectToAction("EnquiryLeadList");
                }

                model.Message = "";
                return View(model);
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList();
            }

            model.Message = EduSuiteUIResources.Failed;
            return View(model);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EnquiryLeadFeedback, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditEnquiryLeadFeedback(long? id, long? LeadKey)
        {
            EnquiryLeadFeedbackViewModel objViewModel = new EnquiryLeadFeedbackViewModel();
            objViewModel.RowKey = id ?? 0;
            objViewModel.EnquiryLeadKey = LeadKey ?? 0;
            objViewModel = enquiryLeadService.GetEnquiryLeadFeedbackById(objViewModel);
            objViewModel.EnquiryLeadKey = LeadKey ?? 0;
            return PartialView(objViewModel);
        }

        [HttpPost]
        public JsonResult AddEditEnquiryLeadFeedback(EnquiryLeadFeedbackViewModel model)
        {
            foreach (var modelValue in ModelState.Values)
            {
                modelValue.Errors.Clear();
            }
            if (ModelState.IsValid)
            {

                if (model.RowKey == 0)
                {
                    model = enquiryLeadService.CreateEnquiryLeadFeedback(model);
                }
                else
                {
                    model = enquiryLeadService.UpdateEnquiryLeadFeedback(model);
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
                    //    string[] Emails = model.NotificationMobileNo.Split('|');
                    //}

                    return Json(model);
                }
                return Json(model);
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList();
            }

            model.Message = EduSuiteUIResources.Failed;
            return Json(model);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EnquiryLead, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteEnquiryLead(Int16 id)
        {
            EnquiryLeadViewModel objViewModel = new EnquiryLeadViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = enquiryLeadService.DeleteEnquiryLead(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EnquiryLeadFeedback, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteEnquiryLeadFeedback(Int16 id)
        {
            EnquiryLeadFeedbackViewModel objViewModel = new EnquiryLeadFeedbackViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = enquiryLeadService.DeleteEnquiryLeadFeeback(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EnquiryLead, ActionCode = ActionConstants.BulkAdd)]
        [HttpGet]
        public ActionResult AddBulkLead()
        {
            EnquiryLeadViewModel objViewModel = new EnquiryLeadViewModel();

            objViewModel = enquiryLeadService.GetBranches(objViewModel);
            return View(objViewModel);
        }

        [HttpPost]
        public ActionResult AddBulkLead(List<EnquiryLeadViewModel> modelList)
        {
            EnquiryLeadViewModel objViewModel = new EnquiryLeadViewModel();
            objViewModel = enquiryLeadService.UpdateEnquiryLeads(modelList);
            if (objViewModel.Message != AppConstants.Common.SUCCESS)
            {
                ModelState.AddModelError("error_msg", objViewModel.Message);
            }
            else
            {
                return Json(objViewModel);
            }
            return Json(objViewModel);
        }

        [HttpPost]
        public JsonResult GetPendingLeadById(EnquiryLeadViewModel model)
        {
            int TotalRecords = 0;

            var leadList = enquiryLeadService.GetPendingLeadByBranch(model, out TotalRecords);
            var result = new
            {
                TotalRecords = TotalRecords,
                Leads = leadList,
                Employees = model.Employees,
                TelephoneCodes = model.TelephoneCodes
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetEmployeeByBranchId(short? id)
        {
            EnquiryLeadViewModel objViewModel = new EnquiryLeadViewModel();
            objViewModel.BranchKey = id ?? 0;

            objViewModel = enquiryLeadService.GetEmployeesByBranchId(objViewModel);
            return Json(objViewModel.Employees, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult CheckMobileNumberExists(string MobileNumber, short TelephoneCodeKey, long RowKey)
        {
            EnquiryLeadViewModel enquiryLead = new EnquiryLeadViewModel();
            enquiryLead = enquiryLeadService.CheckMobileNumberExists(MobileNumber, TelephoneCodeKey, RowKey);
            return Json(enquiryLead.IsSuccessful, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult CheckEmailAddressExists(string EmailAddress, long rowKey)
        {
            EnquiryLeadViewModel enquiryLead = new EnquiryLeadViewModel();
            enquiryLead = enquiryLeadService.CheckEmailAddressExists(EmailAddress, rowKey);
            return Json(enquiryLead.IsSuccessful, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CheckBulkLeadExists(List<EnquiryLeadViewModel> DuplicateMobileNumbers, List<EnquiryLeadViewModel> DuplicateEmails)
        {
            EnquiryLeadViewModel enquiryLead = new EnquiryLeadViewModel();

            if (DuplicateEmails != null && DuplicateEmails.Count > 0)
                DuplicateEmails = enquiryLeadService.CheckBulkEmailAddressExists(DuplicateEmails);

            if (DuplicateMobileNumbers != null && DuplicateMobileNumbers.Count > 0)
                DuplicateMobileNumbers = enquiryLeadService.CheckBulkMobileNumberExists(DuplicateMobileNumbers);
            var result = new
            {
                DuplicateMobileNumbers = DuplicateMobileNumbers,
                DuplicateEmails = DuplicateEmails
            };
            return Json(result);
        }

        [HttpGet]
        public JsonResult GetCallStatusByEnquiryStatus(short EnquiryStatusKey)
        {
            EnquiryLeadFeedbackViewModel Feedback = new EnquiryLeadFeedbackViewModel();
            Feedback.EnquiryLeadStatusKey = EnquiryStatusKey;
            enquiryLeadService.GetCallStatusByEnquiryStatus(Feedback);
            return Json(Feedback.EnquiryLeadCallStatuses, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult CheckCallStatusDuration(int? Id)
        {
            EnquiryLeadFeedbackViewModel ObjViewModel = new EnquiryLeadFeedbackViewModel();
            ObjViewModel.EnquiryLeadCallStatusKey = Id ?? 0;
            ObjViewModel = enquiryLeadService.CheckCallStatusDuration(ObjViewModel);
            return Json(ObjViewModel != null ? ObjViewModel.IsDuration : false, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetEnquiryLead(string SearchText)
        {
            EnquiryLeadViewModel objViewModel = new EnquiryLeadViewModel();
            List<EnquiryLeadViewModel> List = new List<EnquiryLeadViewModel>();
            objViewModel.SearchText = SearchText;

            List = enquiryLeadService.GetEnquiryLead(objViewModel);
            return Json(List, JsonRequestBehavior.AllowGet);
        }

        public void SendNotification(EnquiryLeadFeedbackViewModel model)
        {


            if (model.IsNewLeadNotification)
            {
                NotificationDataViewModel notificationModel = new NotificationDataViewModel();
                NotificationHelper nofificationHelper = new NotificationHelper(notificationTemplateService);
                notificationModel.EmailTemplateName = Server.MapPath("~/UploadedFiles/NotificationTemplate/");
                //notificationModel.UserKey = model.UserKey;
                notificationModel.PushNotificationUserkeys = model.UserKeys;
                notificationModel.PushNotificationTemplateKey = DbConstants.PushNotificationTemplate.LeadCountlimit;
                nofificationHelper.SendNotificationInBackground(notificationModel);
            }


            if (model.IsClosedNotification)
            {
                NotificationDataViewModel notificationModel = new NotificationDataViewModel();
                NotificationHelper nofificationHelper = new NotificationHelper(notificationTemplateService);
                notificationModel.EmailTemplateName = Server.MapPath("~/UploadedFiles/NotificationTemplate/");
                //notificationModel.UserKey = model.UserKey;
                notificationModel.PushNotificationUserkeys = model.UserKeys;
                notificationModel.PushNotificationTemplateKey = DbConstants.PushNotificationTemplate.LeadEmployeeLocked;
                nofificationHelper.SendNotificationInBackground(notificationModel);
                NotificationHub.PushUserLogout(new List<long>() { model.UserKey });
            }

        }

        [HttpGet]
        public JsonResult GetPhoneNumberLength(short? TelephoneCodeKey,  short? TelephoneCodeOptionalKey)
        {
            EnquiryLeadViewModel model = new EnquiryLeadViewModel();
            model.TelephoneCodeKey = TelephoneCodeKey;           
            model.TelephoneCodeOptionalKey = TelephoneCodeOptionalKey;
            model = enquiryLeadService.GetPhoneNumberLength(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

    }
}