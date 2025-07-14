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
    public class BankAccountController : BaseController
    {
        private IBankAccountService BankAccountService;

        public BankAccountController(IBankAccountService objBankAccountService)
        {
            this.BankAccountService = objBankAccountService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.BankAccount, ActionCode = ActionConstants.View)]
        [HttpGet]
        public ActionResult BankAccountList()
        {
            BankAccountViewModel objViewModel = new BankAccountViewModel();
            objViewModel = BankAccountService.GetBranches(objViewModel);
            return View(objViewModel);
        }

        [HttpGet]
        public JsonResult GetBankAccounts(string searchText,short? BranchKey,string sidx, string sord, int page, int rows)
        {
            long TotalRecords = 0;

            List<BankAccountViewModel> bankAccountList = new List<BankAccountViewModel>();
            BankAccountViewModel objViewModel = new BankAccountViewModel();
            objViewModel.searchText = searchText ?? "";
            objViewModel.BranchKey = BranchKey ?? 0;
            objViewModel.PageIndex = page;
            objViewModel.PageSize = rows;
            objViewModel.SortBy = sidx;
            objViewModel.SortOrder = sord;
            bankAccountList = BankAccountService.GetBankAccounts(objViewModel, out TotalRecords);                    
           
            var totalPages = (int)Math.Ceiling((float)TotalRecords / (float)rows);
            var jsonData = new
            {
                total = totalPages,
                page,
                records = TotalRecords,
                rows = bankAccountList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.BankAccount, ActionCode = ActionConstants.AddEdit)]
        public ActionResult AddEditBankAccount(long? id)
        {
            var BankAccount = BankAccountService.GetBankAccountById(id ?? 0);
            if (BankAccount == null)
            {
                BankAccount = new BankAccountViewModel();
            }
            return PartialView(BankAccount);
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.BankAccount, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult AddEditBankAccount(BankAccountViewModel BankAccount)
        {

            if (ModelState.IsValid)
            {
                if (BankAccount.RowKey == 0)
                {
                    BankAccount = BankAccountService.CreateBankAccount(BankAccount);
                }
                else
                {
                    BankAccount = BankAccountService.UpdateBankAccount(BankAccount);
                }

                if (BankAccount.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", BankAccount.Message);
                }
                else
                {
                    return Json(BankAccount);
                }

                BankAccount.Message = "";
                return PartialView(BankAccount);
            }

            BankAccount.Message = EduSuiteUIResources.Failed;
            return PartialView(BankAccount);

        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.BankAccount, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteBankAccount(Int64 id)
        {
            BankAccountViewModel objViewModel = new BankAccountViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = BankAccountService.DeleteBankAccount(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }

        [HttpGet]
        public JsonResult GetAccountBalanceByAccount(long id)
        {
            CashFlowViewModel objViewModel = new CashFlowViewModel();
            objViewModel.BankAccountBalance = BankAccountService.GetAccountBalanceByAccount(id);
            return Json(objViewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetBankBranches(short? id)
        {
            BankAccountViewModel objViewModel = new BankAccountViewModel();
            objViewModel.BranchKey = id ?? 0;

            objViewModel = BankAccountService.GetBankBranches(objViewModel);
            return Json(objViewModel.BankBranches, JsonRequestBehavior.AllowGet);
        }
    }
}