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
    public class BankTransactionController : BaseController
    {
        private IBankTransactionService bankTransactionService;
        private ISelectListService selectListService;

        public BankTransactionController(IBankTransactionService objbankTransactionService, ISelectListService objselectListService)
        {
            this.bankTransactionService = objbankTransactionService;
            this.selectListService = objselectListService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.BankTransaction, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult BankTransactionList()
        {
            BankTransactionViewModel objBankTransactionViewModel = new BankTransactionViewModel();
            objBankTransactionViewModel = bankTransactionService.GetBankTransactionTypes(objBankTransactionViewModel);
            objBankTransactionViewModel.SearchBankAccounts = selectListService.FillSearchBankAccounts(objBankTransactionViewModel.BranchKey);
            return View(objBankTransactionViewModel);
        }

        [HttpGet]
        public JsonResult GetBankTransactionsByType(byte? BankTransactionTypeKey, short? BranchKey, string SearchText, string SearchDate, long? SearchBankAccountKey, string sidx, string sord, int page, int rows)
        {
            long TotalRecords = 0;

            List<BankTransactionViewModel> bankTransactionList = new List<BankTransactionViewModel>();
            BankTransactionViewModel objBankTransactionViewModel = new BankTransactionViewModel();
            objBankTransactionViewModel.BankTransactionTypeKey = BankTransactionTypeKey ?? 0;
            objBankTransactionViewModel.SearchBankAccountKey = SearchBankAccountKey ?? 0;
            objBankTransactionViewModel.BranchKey = BranchKey ?? 0;
            objBankTransactionViewModel.SearchText = SearchText;
            DateTime? FromDate = new DateTime();
            FromDate = null;
            objBankTransactionViewModel.SearchDate = SearchDate != "" ? DateTime.ParseExact(SearchDate, "dd/MM/yyyy", null) : FromDate;
            objBankTransactionViewModel.PageIndex = page;
            objBankTransactionViewModel.PageSize = rows;
            objBankTransactionViewModel.SortBy = sidx;
            objBankTransactionViewModel.SortOrder = sord;

            bankTransactionList = bankTransactionService.GetBankTransactionsByType(objBankTransactionViewModel, out TotalRecords);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            var totalPages = (int)Math.Ceiling((float)TotalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = TotalRecords,
                rows = bankTransactionList
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        [ActionAuthenticationAttribute(MenuCode = MenuConstants.BankTransaction, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditBankTransaction(long? id, byte? bankTransactionTypeKey, short? branchKey)
        {
            BankTransactionViewModel bankTransactionModel = new BankTransactionViewModel();
            bankTransactionModel.RowKey = id ?? 0;
            bankTransactionModel.BankTransactionTypeKey = bankTransactionTypeKey ?? 0;
            bankTransactionModel.BranchKey = branchKey ?? 0;
            bankTransactionModel = bankTransactionService.GetBankTransactionById(bankTransactionModel);

            if (bankTransactionTypeKey == DbConstants.BankTransactionType.Deposit)
                ViewBag.Title = EduSuiteUIResources.Add + EduSuiteUIResources.Sla + EduSuiteUIResources.Edit + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Deposit;
            else if (bankTransactionTypeKey == DbConstants.BankTransactionType.Withdrawal)
                ViewBag.Title = EduSuiteUIResources.Add + EduSuiteUIResources.Sla + EduSuiteUIResources.Edit + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.withdraw;
            else
                ViewBag.Title = EduSuiteUIResources.Add + EduSuiteUIResources.Sla + EduSuiteUIResources.Edit + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.AccountTransfer;

            return PartialView(bankTransactionModel);
        }

        [HttpPost]
        public ActionResult AddEditBankTransaction(BankTransactionViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = bankTransactionService.CreateBankTransaction(model);
                }
                else
                {
                    model = bankTransactionService.UpdateBankTransaction(model);
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

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.BankTransaction, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteBankTransaction(Int64 id)
        {
            BankTransactionViewModel objViewModel = new BankTransactionViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = bankTransactionService.DeleteBankTransaction(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }

        [HttpGet]
        public JsonResult GetBankAccountById(long? id, short? BranchKey)
        {
            BankTransactionViewModel objBankTransactionViewModel = new BankTransactionViewModel();
            objBankTransactionViewModel.FromBankAccountKey = id;
            objBankTransactionViewModel.BranchKey = BranchKey ?? 0;
            objBankTransactionViewModel = bankTransactionService.GetBankAccountById(objBankTransactionViewModel);
            return Json(objBankTransactionViewModel.ToBankAccounts, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult FillBankAccount(long? id, short? BranchKey)
        {
            BankTransactionViewModel model = new BankTransactionViewModel();

            model.BranchKey = BranchKey ?? 0;
            model = bankTransactionService.FillBankAccounts(model);
            return Json(model.FromBankAccounts, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetBalance(long? RowKey, long? BankAccountKey, short? branchKey, byte? BankTransactionTypeKey)
        {
            decimal Balance = bankTransactionService.CheckShortBalance(RowKey ?? 0, BankAccountKey ?? 0, branchKey ?? 0, BankTransactionTypeKey ?? 0);
            return Json(Balance, JsonRequestBehavior.AllowGet);

        }

    }
}