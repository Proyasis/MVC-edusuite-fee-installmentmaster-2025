using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;


namespace CITS.EduSuite.UI.Controllers
{
    public class UniversityPaymentController : BaseController
    {
        private IUniversityPaymentService UniversityFeePaymenService;
        private ISharedService sharedService;
        private ISelectListService selectListService;
        private INotificationTemplateService notificationTemplateService;

        public UniversityPaymentController(IUniversityPaymentService objApplicationFeePaymentService, ISharedService objSharedService, INotificationTemplateService objnotificationTemplateService, ISelectListService objselectListService)
        {
            this.UniversityFeePaymenService = objApplicationFeePaymentService;
            this.sharedService = objSharedService;
            this.notificationTemplateService = objnotificationTemplateService;
            this.selectListService = objselectListService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UniversityPayment, ActionCode = ActionConstants.View)]
        [HttpGet]
        public ActionResult UniversityFeePaymentList(long? id)
        {
            List<UniversityPaymentViewmodel> objViewModel = new List<UniversityPaymentViewmodel>();
            objViewModel = UniversityFeePaymenService.GetUniversityFeePaymentsByApplication(id ?? 0);
            ViewBag.ApplicationKey = id ?? 0;
            return PartialView(objViewModel);

        }

        [HttpGet]
        public ActionResult UniversityFeePayment(long? id)
        {
            UniversityPaymentViewmodel objViewModel = new UniversityPaymentViewmodel();

            objViewModel.ApplicationKey = id ?? 0;
            return PartialView(objViewModel);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UniversityPayment, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditUniversityFeePayment(long? id, long? ApplicationKey)
        {
            UniversityPaymentViewmodel objViewModel = new UniversityPaymentViewmodel();
            objViewModel.RowKey = id ?? 0;
            objViewModel.ApplicationKey = ApplicationKey ?? 0;
            objViewModel = UniversityFeePaymenService.GetUniversityFeePaymentById(objViewModel);
            return PartialView(objViewModel);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UniversityPayment, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult AddEditUniversityFeePayment(UniversityPaymentViewmodel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey != 0)
                {

                    model = UniversityFeePaymenService.UpdateUniversityFee(model);
                }
                else
                {
                    model = UniversityFeePaymenService.CreateUniversityFee(model);
                }

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    if (model.AutoEmail == true || model.AutoSMS == true)
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

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UniversityPayment, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteUniveristyFeePayment(long id)
        {
            UniversityPaymentViewmodel objUniversityFeePaymentViewModel = new UniversityPaymentViewmodel();
            try
            {
                objUniversityFeePaymentViewModel = UniversityFeePaymenService.DeleteUniversityFee(id);
            }
            catch (Exception)
            {
                objUniversityFeePaymentViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objUniversityFeePaymentViewModel);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UniversityPayment, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteUniversityFeeOneByOne(long id)
        {
            UniversityPaymentViewmodel objApplicationFeePaymentViewModel = new UniversityPaymentViewmodel();
            try
            {
                objApplicationFeePaymentViewModel = UniversityFeePaymenService.DeleteUniversityFeeOneByOne(id);
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
            UniversityFeePrintViewModel model = new UniversityFeePrintViewModel();
            model.UniversityPaymentViewmodel.RowKey = id ?? 0;

            var modellist = UniversityFeePaymenService.ViewFeePrint(model.UniversityPaymentViewmodel.RowKey);
            return PartialView(modellist);
        }

        [HttpGet]
        public JsonResult CheckFeeTypeExists(short FeeTypeKey, long RowKey)
        {
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCashFlowTypeKey(short id)
        {
            byte CashFlowTypeKey = UniversityFeePaymenService.FillCashFlowTypeKey(id);
            return Json(CashFlowTypeKey, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UniversityPayment, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult UniversityPaymentDetails()
        {
            UniversityPaymentViewmodel objViewModel = new UniversityPaymentViewmodel();
            objViewModel.Branches = selectListService.FillBranches();
            objViewModel.Batches = selectListService.FillSearchBatch(objViewModel.BranchKey);
            objViewModel.Courses = selectListService.FillSearchCourse(objViewModel.BranchKey);
            objViewModel.Universities = selectListService.FillSearchUniversity(objViewModel.BranchKey);
            return View(objViewModel);
        }

        [HttpGet]
        public JsonResult FillBatch(short? BranchKey)
        {
            UniversityPaymentViewmodel model = new UniversityPaymentViewmodel();
            model.BranchKey = BranchKey ?? 0;
            model.Batches = selectListService.FillBatches(model.BranchKey);
            return Json(model.Batches, JsonRequestBehavior.AllowGet);
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

            applicationList = UniversityFeePaymenService.GetApplications(objViewModel, out TotalRecords);

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

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UniversityPayment, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddUniversityFeeDetails(long? id)
        {
            UniversityPaymentViewmodel objViewModel = new UniversityPaymentViewmodel();

            objViewModel.ApplicationKey = id ?? 0;
            return View(objViewModel);

        }

        // BulK University Payment
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UniversityPayment, ActionCode = ActionConstants.BulkAdd)]
        [HttpGet]
        public ActionResult AddBulkUniversityPayment()
        {
            UniversityPaymentBulkViewmodel objViewModel = new UniversityPaymentBulkViewmodel();
            UniversityFeePaymenService.FillDrodownLists(objViewModel);
            objViewModel.Branches = selectListService.FillBranches();

            ViewBag.ShowAcademicTerm = sharedService.ShowAcademicTerm();
            ViewBag.ShowUniversity = sharedService.ShowUniversity();
            return View(objViewModel);
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UniversityPayment, ActionCode = ActionConstants.BulkAdd)]
        [HttpPost]
        public ActionResult AddBulkUniversityPayment(UniversityPaymentBulkViewmodel model)
        {
            if (ModelState.IsValid)
            {
                model = UniversityFeePaymenService.CreateUniversityPaymentBulk(model);
                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);                   
                }
                else
                {
                    //return Redirect("UniversityPaymentDetails");
                    return Json(model);
                }
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList();
            }
            model.Message = EduSuiteUIResources.Failed;
            return Json(model);

        }
        [HttpPost]
        public ActionResult GetUniversityPaymentStudentsList(UniversityPaymentBulkViewmodel model)
        {
            model.UniversityPaymentBulklist = UniversityFeePaymenService.GetUniversityPaymentStudentsList(model);
            ViewBag.TotalRecords = model.TotalRecords;
            ViewBag.PageIndex = model.PageIndex;

            return PartialView(model.UniversityPaymentBulklist);
        }
        [HttpGet]
        public JsonResult GetCourseTypeBySyllabus(short AcademicTermKey)
        {
            UniversityPaymentBulkViewmodel objViewModel = new UniversityPaymentBulkViewmodel();
            objViewModel.SearchAcademicTermKey = AcademicTermKey;
            objViewModel = UniversityFeePaymenService.GetCourseTypeBySyllabus(objViewModel);
            return Json(objViewModel, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetCourseByCourseType(byte? CourseTypeKey, short AcademicTermKey)
        {
            UniversityPaymentBulkViewmodel objViewModel = new UniversityPaymentBulkViewmodel();
            objViewModel.SearchCourseTypeKey = CourseTypeKey ?? 0;
            objViewModel.SearchAcademicTermKey = AcademicTermKey;
            objViewModel = UniversityFeePaymenService.GetCourseByCourseType(objViewModel);
            return Json(objViewModel, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetUniversityByCourse(short? CourseKey)
        {
            UniversityPaymentBulkViewmodel objViewModel = new UniversityPaymentBulkViewmodel();
            objViewModel.SearchCourseKey = CourseKey ?? 0;
            objViewModel = UniversityFeePaymenService.GetUniversityByCourse(objViewModel);
            return Json(objViewModel, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetYearsBySyllabus(short AcademicTermKey, int? CourseKey)
        {
            UniversityPaymentBulkViewmodel objViewModel = new UniversityPaymentBulkViewmodel();
            objViewModel.SearchAcademicTermKey = AcademicTermKey;
            objViewModel.SearchCourseKey = CourseKey ?? 0;
            objViewModel = UniversityFeePaymenService.GetYears(objViewModel);
            return Json(objViewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CheckFeeTypeMode(short? id, int? Year, long Applicationkey)
        {
            //bool IsFeeTypeYear = applicationFeePaymentService.CheckFeeTypeMode(id);
            //return Json(IsFeeTypeYear, JsonRequestBehavior.AllowGet);
            UniversityPaymentDetailsmodel model = new UniversityPaymentDetailsmodel();
            model = UniversityFeePaymenService.CheckFeeTypeMode(id ?? 0, Year, Applicationkey);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CheckFeeTypeModeBulk(short? id)
        {
            //bool IsFeeTypeYear = applicationFeePaymentService.CheckFeeTypeMode(id);
            //return Json(IsFeeTypeYear, JsonRequestBehavior.AllowGet);
            UniversityPaymentBulkViewmodel model = new UniversityPaymentBulkViewmodel();
            model = UniversityFeePaymenService.CheckFeeTypeModeBulk(id ?? 0);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UniversityPayment, ActionCode = ActionConstants.View)]
        [HttpGet]
        public ActionResult UniversityTotalFeeDetails(long? id)
        {
            UniversityPaymentViewmodel objViewModel = new UniversityPaymentViewmodel();
            objViewModel.ApplicationKey = id ?? 0;
            var TotalFeeDetails = UniversityFeePaymenService.BindTotalFeeDetails(objViewModel);
            return PartialView(TotalFeeDetails);

        }

        private void SendNotification(UniversityPaymentViewmodel model)
        {


            NotificationDataViewModel notificationModel = new NotificationDataViewModel();
            NotificationHelper nofificationHelper = new NotificationHelper(notificationTemplateService);
            notificationModel.EmailTemplateName = Server.MapPath("~/Templates/NotificationTemplate/");
            notificationModel.RowKey = model.RowKey;
            notificationModel.AutoSMS = model.AutoSMS;
            notificationModel.AutoEmail = model.AutoEmail;
            notificationModel.TemplateKey = model.TemplateKey;
            notificationModel.MobileNumber = model.StudentMobile;
            notificationModel.EmailAddess = model.StudentEmail;
            nofificationHelper.SendNotificationInBackground(notificationModel);


        }

        [HttpGet]
        public JsonResult GetBalance(short PaymentModeKey, long? RowKey, long? BankAccountKey, short? branchKey)
        {
            decimal Balance = UniversityFeePaymenService.CheckShortBalance(PaymentModeKey, RowKey ?? 0, BankAccountKey ?? 0, branchKey ?? 0);
            return Json(Balance, JsonRequestBehavior.AllowGet);
        }
    }
}