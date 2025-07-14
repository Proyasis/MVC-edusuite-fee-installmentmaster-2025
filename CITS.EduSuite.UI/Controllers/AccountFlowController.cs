using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Security;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CITS.EduSuite.UI.Controllers
{
    public class AccountFlowController : BaseController
    {
        private IAccountFlowService AccountFlowService;


        public AccountFlowController(IAccountFlowService ObjAccountflowService)
        {
            this.AccountFlowService = ObjAccountflowService;
        }
        [HttpGet]
        public ActionResult AccountFlowList()
        {
            return View();
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Ledger, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult ViewLedger()
        {
            AccountFlowViewModel model = new AccountFlowViewModel();
            //
            AccountFlowService.FillBranches(model);
            AccountFlowService.FillAccountHead(model);

            return View(model);
        }
        [HttpGet]
        public ActionResult BindLedgerById(long id, string fromDate, string toDate, short? BranchKey, int? appUserKey, bool? PeriodOnly, bool? isCashFlow)
        {
            AccountFlowViewModel model = new AccountFlowViewModel();
           //
            model.BranchKey = BranchKey ?? 0;
            model = AccountFlowService.GetLedgerById(model, id, fromDate, toDate,  PeriodOnly ?? false, (isCashFlow ?? false));
            if (model == null)
            {
                model = new AccountFlowViewModel();
            }
            return PartialView(model);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.BalanceSheet, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult BalanceSheet()
        {
            AccountFlowViewModel model = new AccountFlowViewModel();
           //
            var Month = DateTimeUTC.Now.Month;
            if (Month >= 4)
            {
                model.FromDate = new DateTime(DateTimeUTC.Now.Year, 4, 1);
            }
            else
            {
                model.FromDate = new DateTime(DateTimeUTC.Now.AddYears(-1).Year, 4, 1);
            }
            AccountFlowService.FillBranches(model);
            return View(model);
        }
        [HttpPost]
        public ActionResult GetBalanceSheet(string fromDate, string toDate, short? branchkey)
        {
            AccountFlowViewModel model = new AccountFlowViewModel();
           //
            model.BranchKey = branchkey ?? 0;
            model = AccountFlowService.GetBalanceSheet(model, fromDate, toDate);
            if (model == null)
            {
                model = new AccountFlowViewModel();
            }
            if (model.Message != null && model.Message != "")
            {
                //model.UserKey = GetUserKey();
                //ActivityLog.CreateActivityLog(model.UserKey, MenuConstant.BalanceSheet, ActionConstant.MenuAccess, DbConstants.LogType.Error, 0, model.ExceptionMessage);
            }
            return PartialView(model);
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.DayBook, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult DayBook()
        {
            AccountFlowViewModel model = new AccountFlowViewModel();
           //
            AccountFlowService.FillBranches(model);
            return View(model);
        }

        [HttpGet]
        public ActionResult GetDayBook(string fromDate, string toDate, short? branchkey, bool? periodOnly)
        {
            AccountFlowViewModel model = new AccountFlowViewModel();
            model.BranchKey = branchkey ?? 0;
            model = AccountFlowService.GetDayBook(model, fromDate, toDate, periodOnly ?? false);
            if (model == null)
            {
                model = new AccountFlowViewModel();
            }

            return PartialView(model);
        }


        [ActionAuthenticationAttribute(MenuCode = MenuConstants.DayBook, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult DayBookSeprate()
        {
            AccountFlowViewModel model = new AccountFlowViewModel();
            //
            AccountFlowService.FillBranches(model);
            return View(model);
        }
        [HttpPost]
        public ActionResult GetDayBookSeprate(string fromDate, string toDate, short? branchkey)
        {
            AccountFlowViewModel model = new AccountFlowViewModel();
            List<dynamic> dayBookList = new List<dynamic>();
            model.BranchKey = branchkey ?? 0;
            dayBookList = AccountFlowService.GetDayBookSeprate(model, fromDate, toDate);
           

            return Json(dayBookList);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.TrialBalance, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult TrialBalance()
        {
            AccountFlowViewModel model = new AccountFlowViewModel();
           //
            AccountFlowService.FillBranches(model);
            return View(model);
        }
        [HttpGet]
        public ActionResult GetTrialBalance(string fromDate, string toDate, short? branchkey)
        {
            AccountFlowViewModel model = new AccountFlowViewModel();
            model.BranchKey = branchkey ?? 0;
            model = AccountFlowService.GetTrialBalance(model, fromDate, toDate);
            if (model == null)
            {
                model = new AccountFlowViewModel();
            }

            return PartialView(model);
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.IncomeStatment, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult IncomeStatement()
        {
            AccountFlowViewModel model = new AccountFlowViewModel();
           //
            AccountFlowService.FillBranches(model);
            return View(model);
        }
        [HttpGet]
        public ActionResult GetIncomeStatement(string fromDate, string toDate, short? branchkey)
        {
            AccountFlowViewModel model = new AccountFlowViewModel();
            model.BranchKey = branchkey ?? 0;
            model = AccountFlowService.GetIncomeStatement(model, fromDate, toDate);
            if (model == null)
            {
                model = new AccountFlowViewModel();
            }

            return PartialView(model);
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.CashBook, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult CashBook()
        {
            AccountFlowViewModel model = new AccountFlowViewModel();
           //
            AccountFlowService.FillBranches(model);
            return View(model);
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.BankBook, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult BankBook()
        {
            AccountFlowViewModel model = new AccountFlowViewModel();
           //
            AccountFlowService.FillBranches(model);
            AccountFlowService.FillBankAccount(model);
            return View(model);
        }

        [HttpGet]
        public JsonResult FillBanks(short? branchkey)
        {
            AccountFlowViewModel model = new AccountFlowViewModel();
            model.BranchKey = branchkey ?? 0;
            model = AccountFlowService.FillBankAccount(model);
            return Json(model.BankAccounts, JsonRequestBehavior.AllowGet);

        }
       

        public ActionResult GSTEFilingList()
        {
            AccountFlowViewModel model = new AccountFlowViewModel();
           //
           AccountFlowService.FillBranches(model);
            return View(model);
        }

        [HttpGet]
        public string GetGSTEFiling(byte month, short year, short? branchkey, byte? gstFlow)
        {
            AccountFlowViewModel model = new AccountFlowViewModel();
            model.BranchKey = branchkey ?? 0;
            //
            var resultString = AccountFlowService.GetGSTEFilingReport(model, month, year, gstFlow ?? 1);
            if (model == null)
            {
                model = new AccountFlowViewModel();
            }
            if (model.Message != null && model.Message != "")
            {
                //model.UserKey = GetUserKey();

            }


            return resultString;
        }

        [HttpGet]
        public ActionResult GetGSTPayable(byte month, short year, short? branchkey)
        {
            GSTEFilingTotalViewModel model = new GSTEFilingTotalViewModel();
            model.BranchKey = branchkey ?? 0;
            //
            model = AccountFlowService.GetGSTEFilingTotalReport(model, month, year);
            if (model == null)
            {
                model = new GSTEFilingTotalViewModel();
            }
            if (model.Message != null && model.Message != "")
            {
                //model.UserKey = GetUserKey();

            }
            return PartialView(model);
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.CashFlowStatement, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult CashFlowStatement()
        {
            AccountFlowViewModel model = new AccountFlowViewModel();
            //
            AccountFlowService.FillBranches(model);
            return View(model);
        }


        [ActionAuthenticationAttribute(MenuCode = MenuConstants.ProfitAndLossAccount, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult ProfitAndLossAccount()
        {
            AccountFlowViewModel model = new AccountFlowViewModel();
            //
            AccountFlowService.FillBranches(model);
            return View(model);
        }
        [HttpGet]
        public ActionResult GetProfitAndLossAccount(string fromDate, string toDate, short? branchkey)
        {
            ProfitAndLossAccountViewModel model = new ProfitAndLossAccountViewModel();
            model.BranchKey = branchkey ?? 0;
            //
            model = AccountFlowService.GetProfitAndLoss(model, fromDate, toDate);
            if (model == null)
            {
                model = new ProfitAndLossAccountViewModel();
            }
            if (model.Message != null && model.Message != "")
            {
                //model.UserKey = GetUserKey();

            }
            return PartialView(model);
        }
    }


}