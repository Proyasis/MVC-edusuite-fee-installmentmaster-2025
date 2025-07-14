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
using Rotativa.MVC;
using Rotativa;

namespace CITS.EduSuite.UI.Controllers
{
    public class ApplicationFeePaymentController : BaseController
    {
        private IApplicationFeePaymentService applicationFeePaymentService;
        private INotificationTemplateService notificationTemplateService;
        private ISelectListService selectListService;
        private ISharedService sharedService;
        public ApplicationFeePaymentController(IApplicationFeePaymentService objApplicationFeePaymentService, INotificationTemplateService objnotificationTemplateService
            , ISelectListService objselectListService, ISharedService objsharedService)
        {
            this.applicationFeePaymentService = objApplicationFeePaymentService;
            this.notificationTemplateService = objnotificationTemplateService;
            this.selectListService = objselectListService;
            this.sharedService = objsharedService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.FeeDetails, ActionCode = ActionConstants.View)]
        [HttpGet]
        public ActionResult ApplicationFeePaymentList(long? id)
        {
            List<ApplicationFeePaymentViewModel> objViewModel = new List<ApplicationFeePaymentViewModel>();
            objViewModel = applicationFeePaymentService.GetApplicationFeePaymentsByApplication(id ?? 0);
            ViewBag.ApplicationKey = id ?? 0;
            return PartialView(objViewModel);

        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.FeeDetails, ActionCode = ActionConstants.View)]
        [HttpGet]
        public ActionResult ApplicationFeePayment(long? id)
        {
            ApplicationFeePaymentViewModel objViewModel = new ApplicationFeePaymentViewModel();

            objViewModel.ApplicationKey = id ?? 0;
            ViewBag.FeeSchedule = sharedService.CheckMenuActive(MenuConstants.FeeSchdeule);
            return PartialView(objViewModel);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.FeeDetails, ActionCode = ActionConstants.View)]
        [HttpGet]
        public ActionResult TotalFeeDetails(long? id)
        {
            ApplicationFeePaymentViewModel objViewModel = new ApplicationFeePaymentViewModel();
            objViewModel.ApplicationKey = id ?? 0;
            var TotalFeeDetails = applicationFeePaymentService.BindTotalFeeDetails(objViewModel);
            return PartialView(TotalFeeDetails);

        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.FeeDetails, ActionCode = ActionConstants.View)]
        [HttpGet]
        public ActionResult InstallmentFeeDetails(long? id)
        {
            ApplicationFeePaymentViewModel objViewModel = new ApplicationFeePaymentViewModel();
            objViewModel.ApplicationKey = id ?? 0;
            var InstallmentFeeDetails = applicationFeePaymentService.BindInstallmentFeeDetails(objViewModel);
            return PartialView(InstallmentFeeDetails);

        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.FeeDetails, ActionCode = ActionConstants.View)]
        [HttpGet]
        public ActionResult FeeScheduleDetails(long? id)
        {
            ApplicationFeePaymentViewModel objViewModel = new ApplicationFeePaymentViewModel();
            objViewModel.ApplicationKey = id ?? 0;
            var FeeScheduleDetails = applicationFeePaymentService.BindFeeScheduleDetails(objViewModel);
            return PartialView(FeeScheduleDetails);

        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.FeeDetails, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditApplicationFeePayment(long? id, long? ApplicationKey)
        {
            ApplicationFeePaymentViewModel objViewModel = new ApplicationFeePaymentViewModel();
            objViewModel.RowKey = id ?? 0;
            objViewModel.ApplicationKey = ApplicationKey ?? 0;
            objViewModel = applicationFeePaymentService.GetApplicationFeePaymentById(objViewModel);
            ViewBag.FeeSchedule = sharedService.CheckMenuActive(MenuConstants.FeeSchdeule);
            return PartialView(objViewModel);
        }

        [HttpPost]
        public ActionResult AddEditApplicationFeePayment(ApplicationFeePaymentViewModel model)
        {
            ApplicationFeePaymentViewModel objviewmodel = new ApplicationFeePaymentViewModel();
            objviewmodel = model;
            if (ModelState.IsValid)
            {
                var RowKey = model.RowKey;
                if (model.RowKey != 0)
                {
                    model = applicationFeePaymentService.UpdateApplicationFee(model);
                }
                else
                {
                    model = applicationFeePaymentService.CreateApplicationFee(model);

                    //for (long i = objviewmodel.ApplicationKey; i <= 500; i++)
                    //{
                    //    model = applicationFeePaymentService.CreateApplicationFee(objviewmodel);
                    //}
                    //int j = 0;
                    //while (j <= 500)
                    //{
                    //    model = applicationFeePaymentService.CreateApplicationFee(objviewmodel);
                    //    j++;
                    //    objviewmodel.ApplicationKey++;
                    //    objviewmodel.PurposeList.Clear();
                    //}
                }

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    if (RowKey == 0)
                    {
                        SendNotification(model);
                    }
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

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.FeeDetails, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteApplicationFeePayment(long id)
        {
            ApplicationFeePaymentViewModel objApplicationFeePaymentViewModel = new ApplicationFeePaymentViewModel();
            try
            {
                objApplicationFeePaymentViewModel = applicationFeePaymentService.DeleteApplicationFee(id);
            }
            catch (Exception)
            {
                objApplicationFeePaymentViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objApplicationFeePaymentViewModel);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.FeeDetails, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteApplicationFeeOneByOne(long id)
        {
            ApplicationFeePaymentViewModel objApplicationFeePaymentViewModel = new ApplicationFeePaymentViewModel();
            try
            {
                objApplicationFeePaymentViewModel = applicationFeePaymentService.DeleteApplicationFeeOneByOne(id);
            }
            catch (Exception)
            {
                objApplicationFeePaymentViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objApplicationFeePaymentViewModel);
        }

        [HttpGet]
        public ActionResult FeePrint(int? id)
        {
            ApplicationFeePrintViewModel model = new ApplicationFeePrintViewModel();
            model.ApplicationFeePaymentViewModel.RowKey = id ?? 0;

            var modellist = applicationFeePaymentService.ViewFeePrint(model.ApplicationFeePaymentViewModel.RowKey);
            return PartialView(modellist);
        }

        [HttpGet]
        public ActionResult DownloadFeePrint(int? id)
        {
            ApplicationFeePrintViewModel model = new ApplicationFeePrintViewModel();
            model.ApplicationFeePaymentViewModel.RowKey = id ?? 0;

            var modellist = applicationFeePaymentService.ViewFeePrint(model.ApplicationFeePaymentViewModel.RowKey);
            modellist.PrintDownload = true;
            return new PartialViewAsPdf("FeePrint", modellist)
            {
                RotativaOptions =
                {
                    IsBackgroundDisabled = true
                }
            };

        }

        [HttpGet]
        public JsonResult CheckFeeTypeExists(short FeeTypeKey, int FeeYear)
        {
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCashFlowTypeKey(short? id)
        {
            byte CashFlowTypeKey = applicationFeePaymentService.FillCashFlowTypeKey(id ?? 0);
            return Json(CashFlowTypeKey, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CheckFeeTypeMode(short? id, int? Year, long Applicationkey)
        {
            //bool IsFeeTypeYear = applicationFeePaymentService.CheckFeeTypeMode(id);
            //return Json(IsFeeTypeYear, JsonRequestBehavior.AllowGet);
            ApplicationFeePaymentDetailViewModel model = new ApplicationFeePaymentDetailViewModel();
            model = applicationFeePaymentService.CheckFeeTypeMode(id ?? 0, Year, Applicationkey);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.FeeDetails, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult FeeDetailslist()
        {
            ApplicationViewModel objViewModel = new ApplicationViewModel();

            objViewModel = applicationFeePaymentService.GetSearchDropdownList(objViewModel);
            objViewModel.Batches = selectListService.FillSearchBatch(objViewModel.BranchKey);
            objViewModel.Courses = selectListService.FillSearchCourse(objViewModel.BranchKey);
            objViewModel.Universities = selectListService.FillSearchUniversity(objViewModel.BranchKey);
            return View(objViewModel);
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

            applicationList = applicationFeePaymentService.GetApplications(objViewModel, out TotalRecords);

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
        public ActionResult AddFeeDetails(long? id)
        {
            ApplicationFeePaymentViewModel objViewModel = new ApplicationFeePaymentViewModel();

            objViewModel.ApplicationKey = id ?? 0;
            return View(objViewModel);

        }

        private void SendNotification(ApplicationFeePaymentViewModel model)
        {


            //NotificationDataViewModel notificationModel = new NotificationDataViewModel();
            //NotificationHelper nofificationHelper = new NotificationHelper(notificationTemplateService);
            //notificationModel.EmailTemplateName = Server.MapPath("~/Templates/NotificationTemplate/");
            //notificationModel.RowKey = model.RowKey;
            //notificationModel.AutoSMS = model.AutoSMS;
            //notificationModel.AutoEmail = model.AutoEmail;
            //notificationModel.TemplateKey = model.TemplateKey;
            //notificationModel.MobileNumber = model.StudentMobile;
            //notificationModel.EmailAddess = model.StudentEmail;
            //nofificationHelper.SendNotificationInBackground(notificationModel);


            NotificationDataViewModel notificationModel = new NotificationDataViewModel();
            NotificationHelper nofificationHelper = new NotificationHelper(notificationTemplateService);
            notificationModel.EmailTemplateName = Server.MapPath("~/Templates/NotificationTemplate/");
            notificationModel.RowKey = model.RowKey;
            notificationModel.AutoSMS = model.AutoSMS;
            notificationModel.AutoEmail = model.AutoEmail;
            notificationModel.TemplateKey = model.TemplateKey;
            notificationModel.MobileNumber = model.StudentMobile;
            notificationModel.EmailAddess = model.StudentEmail;
            //notificationModel.GuardianMobileNumber = model.StudentGuardianPhone;
            notificationModel.PushNotificationTemplateKey = DbConstants.PushNotificationTemplate.Fee;

            nofificationHelper.SendNotificationInBackground(notificationModel);


        }

        [HttpGet]
        public JsonResult FillPaymentModeSub(short? PaymentModeKey)
        {
            ApplicationFeePaymentViewModel model = new ApplicationFeePaymentViewModel();
            model.PaymentModeKey = PaymentModeKey ?? 0;
            model = applicationFeePaymentService.FillPaymentModeSub(model);
            return Json(model.PaymentModeSub, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetReceiptNo(short? ReceiptNumberConfigurationKey)
        {
            ApplicationFeePaymentViewModel model = new ApplicationFeePaymentViewModel();
            model.ReceiptNumberConfigurationKey = ReceiptNumberConfigurationKey;
            model = applicationFeePaymentService.GenerateReceiptNo(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetFeeTypeByReceiptType(short? ReceiptNumberConfigurationKey)
        {
            ApplicationFeePaymentViewModel model = new ApplicationFeePaymentViewModel();
            model.ReceiptNumberConfigurationKey = ReceiptNumberConfigurationKey;
            model = applicationFeePaymentService.FillFeeTypes(model);
            return Json(model.FeeTypes, JsonRequestBehavior.AllowGet);
        }
    }
}