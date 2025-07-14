using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.UI.Controllers
{
    public class AccountTransactionController : Controller
    {
        private IAccountTransactionService AccountTransactionService;

        public AccountTransactionController(IAccountTransactionService objAccountTransactionService)
        {
            this.AccountTransactionService = objAccountTransactionService;
        }

        [HttpGet]
        public ActionResult AccountTransactionList(byte? id)
        {
            AccountTransactionViewModel objViewModel = new AccountTransactionViewModel();
            objViewModel = AccountTransactionService.GetBranches(objViewModel);
            return View(objViewModel);
        }

        [HttpGet]
        public JsonResult GetAccountTransactions(short? branchId)
        {
            int page = 1, rows = 10;

            List<AccountTransactionViewModel> AccountTransactionList = new List<AccountTransactionViewModel>();
            AccountTransactionViewModel objViewModel=new AccountTransactionViewModel();
            objViewModel.BranchKey=branchId??0;
            AccountTransactionList = AccountTransactionService.GetAccountTransactions(objViewModel);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = AccountTransactionList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = AccountTransactionList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddEditAccountTransaction(long? id,short? branchKey)
        {
            AccountTransactionViewModel AccountTransaction = new AccountTransactionViewModel();
            AccountTransaction.RowKey = id ?? 0;
            AccountTransaction.BranchKey = branchKey ?? 0;
            AccountTransaction = AccountTransactionService.GetAccountTransactionById(AccountTransaction);
            return PartialView(AccountTransaction);
        }

        [HttpPost]
        public ActionResult AddEditAccountTransaction(AccountTransactionViewModel AccountTransaction)
        {

            if (ModelState.IsValid)
            {
                if (AccountTransaction.RowKey == 0)
                {
                    AccountTransaction = AccountTransactionService.CreateAccountTransaction(AccountTransaction);
                }
                else
                {
                    AccountTransaction = AccountTransactionService.UpdateAccountTransaction(AccountTransaction);
                }

                if (AccountTransaction.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", AccountTransaction.Message);
                }
                else
                {
                    return Json(AccountTransaction);
                }

                AccountTransaction.Message = "";
                return PartialView(AccountTransaction);
            }

            AccountTransaction.Message = EduSuiteResourceManager.GetApplicationString(AppConstants.Common.FAILED);
            return PartialView(AccountTransaction);

        }

        [HttpPost]
        public ActionResult DeleteAccountTransaction(Int64 id)
        {
            AccountTransactionViewModel objViewModel = new AccountTransactionViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = AccountTransactionService.DeleteAccountTransaction(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteResourceManager.GetApplicationString(AppConstants.Common.FAILED);
            }
            return Json(objViewModel);
        }

        [HttpGet]
        public JsonResult GetPartyByPartyType(byte id, short BranchId)
        {
            AccountTransactionViewModel model = new AccountTransactionViewModel();
            model.PartyTypeKey = id;
            model.BranchKey = BranchId;
            AccountTransactionViewModel objViewModel = new AccountTransactionViewModel();
            objViewModel.Parties = AccountTransactionService.GetPartyByPartyType(model);
            return Json(objViewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetTransactionTypeByLedger(int Id)
        {
            return Json( AccountTransactionService.GetTransactionTypeByLedger(Id), JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult AddEditAccountTransactionPayment(long? id)
        {
            var employeeSalaryPayment = AccountTransactionService.GetAccountTransactionPaymentById(id ?? 0);
            var Url = "~/Views/Shared/PaymentWindow.cshtml";
            return PartialView(Url, employeeSalaryPayment);
        }

        [HttpPost]
        public ActionResult AddEditAccountTransactionPayment(PaymentWindowViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.PaymentKey == 0)
                {
                    model = AccountTransactionService.CreateTransactionPayment(model);
                }
                else
                {
                    model = AccountTransactionService.UpdateTransactionPayment(model);
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

            model.Message = EduSuiteResourceManager.GetApplicationString(AppConstants.Common.FAILED);
            return PartialView(model);

        }
    }
}