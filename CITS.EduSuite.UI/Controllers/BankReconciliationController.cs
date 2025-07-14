using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.Security;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Provider;
//using CITS.EduSuite.UI.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CITS.EduSuite.UI.Controllers
{
    public class BankReconciliationController : BaseController
    {
        private IBankReconciliationService BankReconciliationService;
        public BankReconciliationController(IBankReconciliationService objBankReconciliationService)
        {
            this.BankReconciliationService = objBankReconciliationService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.BankReconciliation, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditBankReconciliation()
        {
            BankReconciliationViewModel model = new BankReconciliationViewModel();
            var startDate = new DateTime(DateTimeUTC.Now.Year, DateTimeUTC.Now.Month, 1);
            model.FromDate = Convert.ToDateTime(startDate);
            model.ToDate = DateTimeUTC.Now;
            model = BankReconciliationService.GetBranches(model);
            model = BankReconciliationService.FillBankAccounts(model);
            return View(model);
        }

        [HttpGet]
        public ActionResult BankReconciliationList(long? bankAccountKey, string fromDate, string toDate)
        {
            BankReconciliationViewModel model = new BankReconciliationViewModel();
            fromDate = fromDate == "" ? DateTimeUTC.Now.ToString() : fromDate;
            toDate = toDate == "" ? DateTimeUTC.Now.ToString() : toDate;
            model.FromDate = Convert.ToDateTime(fromDate);
            model.ToDate = Convert.ToDateTime(toDate);
            model.BankAccountKey = bankAccountKey ?? 0;
            model = BankReconciliationService.GetBankReconciliationById(model);
            if (model == null)
            {
                model = new BankReconciliationViewModel();
            }
            //if (model.ExceptionMessage != null && model.ExceptionMessage != "")
            //{
            //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.BankReconciliation, bankAccountKey != null && bankAccountKey != 0 ? ActionConstant.Edit : ActionConstant.Add, DbConstants.LogType.Error, bankAccountKey, model.ExceptionMessage);
            //}
            return PartialView(model);
        }
         [ActionAuthenticationAttribute(MenuCode = MenuConstants.BankReconciliation, ActionCode = ActionConstants.View)]
        [HttpGet]
        public ActionResult ViewBankReconciliation(long? bankAccountKey, string fromDate, string toDate)
        {
            BankReconciliationViewModel model = new BankReconciliationViewModel();
            fromDate = fromDate == "" ? DateTimeUTC.Now.ToString() : fromDate;
            toDate = toDate == "" ? DateTimeUTC.Now.ToString() : toDate;
            model.FromDate = Convert.ToDateTime(fromDate);
            model.ToDate = Convert.ToDateTime(toDate);
            model.BankAccountKey = bankAccountKey ?? 0;
            model = BankReconciliationService.ViewBankReconciliation(model);
            if (model == null)
            {
                model = new BankReconciliationViewModel();
            }
            //if (model.ExceptionMessage != null && model.ExceptionMessage != "")
            //{
            //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.BankReconciliation, bankAccountKey != null && bankAccountKey != 0 ? ActionConstant.View : ActionConstant.Add, DbConstants.LogType.Error, bankAccountKey, model.ExceptionMessage);
            //}
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult MatchedStatementList(BankReconciliationViewModel model)
        {
            return PartialView(model);
        }
         [ActionAuthenticationAttribute(MenuCode = MenuConstants.BankReconciliation, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult AddEditBankReconciliation(BankReconciliationViewModel model)
        {

            if (ModelState.IsValid)
            {
                model = BankReconciliationService.CreateBankReconciliation(model);
                //if (model.ExceptionMessage != null && model.ExceptionMessage != "")
                //{
                //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.BankReconciliation, ActionConstant.Add, DbConstants.LogType.Error, model.BankAccountKey, model.ExceptionMessage);
                //}
                //else
                //{
                //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.BankReconciliation, ActionConstant.Add, DbConstants.LogType.Info, model.BankAccountKey, model.Message);
                //}

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    return View(model);
                }

                model.Message = "";
                return View(model);
            }
            foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            {
            }



            model.Message = EduSuiteUIResources.Failed;

            return View(model);
        }

        [HttpGet]
        public JsonResult FillBankAccount(short? key)
        {
            BankReconciliationViewModel model = new BankReconciliationViewModel();
            model.BranchKey = key ?? 0;
            model = BankReconciliationService.FillBankAccounts(model);
            return Json(model.BankAccounts, JsonRequestBehavior.AllowGet);
        }

        //private int GetUserKey()
        //{
        //    CookieProvider cookieProvider = new CookieProvider();
        //    HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
        //    if (authCookie != null)
        //    {
        //        CITSEduSuitePrincipalData userData = cookieProvider.GetCookie(authCookie);
        //        return userData.UserKey;
        //    }
        //    return 0;
        //}
    }
}