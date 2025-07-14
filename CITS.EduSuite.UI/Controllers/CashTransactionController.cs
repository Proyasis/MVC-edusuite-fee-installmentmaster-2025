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
    public class CashTransactionController : BaseController
    {
        private ICashTransactionService cashTransactionService;

        public CashTransactionController(ICashTransactionService objcashTransactionService)
        {
            this.cashTransactionService = objcashTransactionService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.CashTransaction, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult CashTransactionList()
        {
            CashTransactionViewModel objCashTransactionViewModel = new CashTransactionViewModel();
            return View(objCashTransactionViewModel);
        }

        [HttpGet]
        public JsonResult GetCashTransactions(short? BranchKey, string SearchText, string SearchDate, string sidx, string sord, int page, int rows)
        {
            long TotalRecords = 0;

            List<CashTransactionViewModel> cashTransactionList = new List<CashTransactionViewModel>();
            CashTransactionViewModel objCashTransactionViewModel = new CashTransactionViewModel();
            objCashTransactionViewModel.FromBranchKey = BranchKey ?? 0;
            objCashTransactionViewModel.SearchText = SearchText;
            DateTime? FromDate = new DateTime();
            FromDate = null;
            objCashTransactionViewModel.SearchDate = SearchDate != "" ? DateTime.ParseExact(SearchDate, "dd/MM/yyyy", null) : FromDate;
            objCashTransactionViewModel.PageIndex = page;
            objCashTransactionViewModel.PageSize = rows;
            objCashTransactionViewModel.SortBy = sidx;
            objCashTransactionViewModel.SortOrder = sord;

            cashTransactionList = cashTransactionService.GetCashTransactions(objCashTransactionViewModel, out TotalRecords);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            var totalPages = (int)Math.Ceiling((float)TotalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = TotalRecords,
                rows = cashTransactionList
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        [ActionAuthenticationAttribute(MenuCode = MenuConstants.CashTransaction, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditCashTransaction(long? id, short? branchKey)
        {
            CashTransactionViewModel cashTransactionModel = new CashTransactionViewModel();
            cashTransactionModel.RowKey = id ?? 0;
            cashTransactionModel.FromBranchKey = branchKey ?? 0;
            cashTransactionModel = cashTransactionService.GetCashTransactionsById(cashTransactionModel);
            return PartialView(cashTransactionModel);
        }

        [HttpPost]
        public ActionResult AddEditCashTransaction(CashTransactionViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = cashTransactionService.CreateCashTransactions(model);
                }
                else
                {
                    model = cashTransactionService.UpdateCashTransactions(model);
                }

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    return Json(model);
                }

                model.Message = "";
                return PartialView(model);
            }
            foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            {
            }

            model.Message = EduSuiteUIResources.Failed;
            return PartialView(model);

        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.CashTransaction, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteCashTransaction(Int64 id)
        {
            CashTransactionViewModel objViewModel = new CashTransactionViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = cashTransactionService.DeleteCashTransactions(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }

        [HttpGet]
        public JsonResult GetToBranchById(short? id)
        {
            CashTransactionViewModel objCashTransactionViewModel = new CashTransactionViewModel();
            objCashTransactionViewModel.FromBranchKey = id;
            objCashTransactionViewModel = cashTransactionService.GetToBranchById(objCashTransactionViewModel);
            return Json(objCashTransactionViewModel.ToBranch, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetBalance(long? RowKey, short? branchKey)
        {
            decimal Balance = cashTransactionService.CheckShortBalance(RowKey ?? 0, branchKey ?? 0);
            return Json(Balance, JsonRequestBehavior.AllowGet);

        }

    }
}