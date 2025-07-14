using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI.Controllers
{
    public class FeeRefundController : BaseController
    {

        private INotificationTemplateService notificationTemplateService;
        private IFeeRefundService feeRefundService;
        private ISharedService sharedService;
        private ISelectListService selectListService;

        public FeeRefundController(
           INotificationTemplateService objnotificationTemplateService,
           ISharedService objsharedService,
            ISelectListService objSelectListService,
            IFeeRefundService objFeeRefundService
           )
        {

            this.notificationTemplateService = objnotificationTemplateService;
            this.sharedService = objsharedService;
            this.selectListService = objSelectListService;
            this.feeRefundService = objFeeRefundService;
        }



        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentFeeRefund, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult FeeRefundList()
        {
            ApplicationViewModel model = new ApplicationViewModel();

            model.Branches = selectListService.FillBranches();
            model.Batches = selectListService.FillSearchBatch(model.BranchKey);
            model.Courses = selectListService.FillSearchCourse(model.BranchKey);
            model.Universities = selectListService.FillSearchUniversity(model.BranchKey);
            return View(model);
        }

        [HttpGet]
        public JsonResult GetStudentFeeRefund(string SearchText, short? BranchKey, long? CourseKey, short? UniversityKey, short? BatchKey, string FromDate, string ToDate, string sidx, string sord, int page, int rows)
        {
            long TotalRecords = 0;

            List<FeeRefundViewModel> feeRefundList = new List<FeeRefundViewModel>();
            FeeRefundViewModel objFeeRefundViewModel = new FeeRefundViewModel();
            objFeeRefundViewModel.ApplicantName = SearchText;
            objFeeRefundViewModel.BranchKey = BranchKey ?? 0;
            objFeeRefundViewModel.BatchKey = BatchKey ?? 0;
            objFeeRefundViewModel.CourseKey = CourseKey ?? 0;
            objFeeRefundViewModel.UniversityKey = UniversityKey ?? 0;
            objFeeRefundViewModel.PageIndex = page;
            objFeeRefundViewModel.PageSize = rows;
            objFeeRefundViewModel.SortBy = sidx;
            objFeeRefundViewModel.SortOrder = sord;

            feeRefundList = feeRefundService.GetStudentFeeRefund(objFeeRefundViewModel, FromDate, ToDate, out TotalRecords);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            var totalPages = (int)Math.Ceiling((float)TotalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = TotalRecords,
                rows = feeRefundList
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentFeeRefund, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult StudentFeeRefundList(long? id, long? ApplicationKey)
        {
            FeeRefundViewModel objViewModel = new FeeRefundViewModel();
            objViewModel.ApplicationKey = ApplicationKey ?? 0;
            objViewModel.RowKey = id ?? 0;
            if (objViewModel.ApplicationKey != 0)
            {
                ApplicationPersonalViewModel applicationPersonalViewModel = new ApplicationPersonalViewModel();
                applicationPersonalViewModel = sharedService.FillApplicationDetails("", objViewModel.ApplicationKey);
                if (applicationPersonalViewModel != null)
                {
                    objViewModel.ApplicationKey = applicationPersonalViewModel.ApplicationKey ?? 0;
                    objViewModel.AdmissionNo = applicationPersonalViewModel.AdmissionNo;
                }
            }
            return View(objViewModel);
        }

        [HttpGet]
        public ActionResult StudentApplication(string AdmissionNo, long? ApplicationKey, long? RowKey)
        {
            FeeRefundViewModel objViewModel = new FeeRefundViewModel();
            objViewModel.ApplicationKey = ApplicationKey ?? 0;
            if ((objViewModel.ApplicationKey) == 0)
            {
                objViewModel.ApplicationKey = sharedService.GetApplicationKeyByCode(AdmissionNo);
            }
            if (objViewModel.ApplicationKey == 0)
            {
                ModelState.AddModelError("error_msg", EduSuiteUIResources.Student_CannotFind);
            }
            objViewModel.RowKey = RowKey ?? 0;

            return PartialView(objViewModel);
        }

        [HttpGet]
        public ActionResult AddEditFeeRefund(long? id, long? ApplicationKey)
        {
            FeeRefundViewModel model = new FeeRefundViewModel();
            model.RowKey = id ?? 0;
            model.ApplicationKey = ApplicationKey ?? 0;
            var studentrefund = feeRefundService.GetStudentFeeRefundById(model);

            return PartialView(studentrefund);
        }

        [HttpGet]
        public ActionResult ReturnAdvanceList(long? id, long? ApplicationKey)
        {
            FeeRefundViewModel model = new FeeRefundViewModel();
            model.RowKey = id ?? 0;
            model.ApplicationKey = ApplicationKey ?? 0;
            model = feeRefundService.fillAdvances(model);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult AddEditFeeRefund(FeeRefundViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = feeRefundService.CreateFeeRefund(model);
                }
                else
                {
                    model = feeRefundService.UpdateFeeRefund(model);
                }

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    return Json(model);
                }

                //model.Message = "";
                return Json(model);
            }
            foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            {
            }

            model.Message = EduSuiteUIResources.Failed;
            return PartialView(model);

        }

        [HttpGet]
        public JsonResult FillPaymentModeSub(short? PaymentModeKey)
        {
            FeeRefundViewModel model = new FeeRefundViewModel();
            model.PaymentModeKey = PaymentModeKey ?? 0;
            model.PaymentModeSub = selectListService.FillPaymentModeSub(model.PaymentModeKey);
            return Json(model.PaymentModeSub, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBalance(short PaymentModeKey, long? RowKey, long? BankAccountKey, short? branchKey)
        {
            decimal Balance = feeRefundService.GetBalanceforRefund(PaymentModeKey, RowKey ?? 0, BankAccountKey ?? 0, branchKey ?? 0);
            return Json(Balance, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentFeeRefund, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteFeeRefund(Int64 id)
        {
            FeeRefundViewModel model = new FeeRefundViewModel();

            model.RowKey = id;
            try
            {
                model = feeRefundService.DeleteFeeRefund(model);
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
            FeeRefundViewModel model = new FeeRefundViewModel();
            model.BranchKey = BranchKey ?? 0;
            model.Batches = selectListService.FillBatches(model.BranchKey);
            return Json(model.Batches, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ApproveFeeRefund(long Id, short? ProcessStatus)
        {
            FeeRefundViewModel model = new FeeRefundViewModel();
            model.RowKey = Id;
            model.ProcessStatus = ProcessStatus;
            try
            {
                model = feeRefundService.ApproveFeeRefund(model);
            }
            catch (Exception)
            {
                model.Message = EduSuiteUIResources.Failed;
            }
            return Json(model);

        }

        [HttpGet]
        public ActionResult FeeRefundPrint(int? id)
        {
            FeeRefundPrintViewModel model = new FeeRefundPrintViewModel();
            model.FeeRefundViewModel.RowKey = id ?? 0;

            var modellist = feeRefundService.ViewFeePrint(model.FeeRefundViewModel.RowKey);
            return PartialView(modellist);
        }
    }
}