using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Models.Security;
using CITS.EduSuite.Business.Provider;
using System.Web.Security;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI.Controllers
{
    public class AccountHeadOpeningBalanceController : BaseController
    {
        private IAccountHeadOpeningBalanceService AccountHeadOpeningBalanceService;
        public AccountHeadOpeningBalanceController(IAccountHeadOpeningBalanceService objAccountHeadOpeningBalanceService)
        {
            this.AccountHeadOpeningBalanceService = objAccountHeadOpeningBalanceService;
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.AccountHeadOpeningBalance, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult AccountHeadOpeningBalanceList()
        {
            AccountHeadOpeningBalanceViewModel model = new AccountHeadOpeningBalanceViewModel();
            model = AccountHeadOpeningBalanceService.FillBranches(model);
            return View(model);
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.AccountHeadOpeningBalance, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditAccountHeadOpeningBalance(short? id)
        {
            AccountHeadOpeningBalanceViewModel model = new AccountHeadOpeningBalanceViewModel();
            model = AccountHeadOpeningBalanceService.GetAccountHeadOpeningBalanceById(id);
            if (model == null)
            {
                model = new AccountHeadOpeningBalanceViewModel();
            }
           
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult AddEditAccountHeadOpeningBalance(AccountHeadOpeningBalanceViewModel model)
        {

            if (ModelState.IsValid)
            {
                model = AccountHeadOpeningBalanceService.CreateAccountHeadOpeningBalance(model);
               

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

            model.Message = EduSuiteUIResources.Failed;
            return PartialView(model);
        }
        public JsonResult GetAccountHeadOpeningBalance(short? BranchKey)
        {
            int page = 1, rows = 15;
            List<AccountHeadOpeningBalanceViewModel> CompanyList = new List<AccountHeadOpeningBalanceViewModel>();
            CompanyList = AccountHeadOpeningBalanceService.GetAccountHeadOpeningBalance((BranchKey ?? 0));
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = CompanyList.Count();
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            if (CompanyList.Count > 0)
            {
                AccountHeadOpeningBalanceViewModel model = CompanyList[0];
              
            }

            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = CompanyList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.AccountHeadOpeningBalance, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteAccountHeadOpeningBalance(short id)
        {
            AccountHeadOpeningBalanceViewModel objViewModel = new AccountHeadOpeningBalanceViewModel();
            objViewModel.BranchKey = id;
            try
            {
                objViewModel = AccountHeadOpeningBalanceService.DeleteAccountHeadOpeningBalance(objViewModel);
               

            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
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