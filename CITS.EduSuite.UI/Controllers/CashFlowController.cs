using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Provider;
using CITS.EduSuite.Business.Models.Security;
using System.Web.Security;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI.Controllers
{
    public class CashFlowController : BaseController
    {
        private ICashFlowService cashFlowService;
        public CashFlowController(ICashFlowService objCashFlowService)
        {
            this.cashFlowService = objCashFlowService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.CashFlow, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult CashFlowList(byte? id)
        {
            CashFlowViewModel objViewModel = new CashFlowViewModel();

            //objViewModel.CashFlowTypeKey = id ?? DbConstants.CashFlowType.In;
            objViewModel = cashFlowService.FillDropdownListsForList(objViewModel);
            objViewModel = cashFlowService.FillSearchAcountHead(objViewModel);
            return View(objViewModel);
        }

        [HttpGet]
        public JsonResult GetCashFlows(string searchAccountHead, short? branchKey, byte? CashFlowTypeKey, long? AccountHeadKey, string SearchDate, string sidx, string sord, int page, int rows)
        {
            CashFlowViewModel model = new CashFlowViewModel();
            List<CashFlowViewModel> CashFlowList = new List<CashFlowViewModel>();
            model.AccountHeadName = searchAccountHead ?? "";
            model.BranchKey = branchKey ?? 0;
            model.CashFlowTypeKey = CashFlowTypeKey ?? 0;
            model.AccountHeadKey = AccountHeadKey ?? 0;
            DateTime? FromDate = new DateTime();
            FromDate = null;
            model.SearchDate = SearchDate != "" ? DateTime.ParseExact(SearchDate, "dd/MM/yyyy", null) : FromDate;
            model.PageIndex = page;
            model.PageSize = rows;
            model.SortBy = sidx;
            model.SortOrder = sord;
            CashFlowList = cashFlowService.GetCashFlows(model);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            long totalRecords = model.TotalRecords;
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = CashFlowList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.CashFlow, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditCashFlow(long? id, short? branchKey, bool IsPayment)
        {
            CashFlowViewModel model = new CashFlowViewModel();

            model.CashFlowKey = id ?? 0;
            model.BranchKey = branchKey ?? 0;
            model.IsPayment = IsPayment;


            model = cashFlowService.GetCashFlowById(model);
            if (IsPayment == true)
                ViewBag.Title = EduSuiteUIResources.Add + EduSuiteUIResources.Sla + EduSuiteUIResources.Edit + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Payment;
            else
                ViewBag.Title = EduSuiteUIResources.Add + EduSuiteUIResources.Sla + EduSuiteUIResources.Edit + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Receipt;
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult AddEditCashFlow(CashFlowViewModel model)
        {


            if (ModelState.IsValid)
            {
                //
                if (model.CashFlowKey == 0)
                {
                    model = cashFlowService.CreateCashFlow(model);


                }
                else
                {
                    model = cashFlowService.UpdateCashFlow(model);


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
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList();
            }

            model.Message = EduSuiteUIResources.Failed;

            //foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            //{
            //}
            return PartialView(model);

        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.CashFlow, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteCashFlow(long? id)
        {
            CashFlowViewModel objViewModel = new CashFlowViewModel();

            objViewModel.RowKey = id ?? 0;
            try
            {
                objViewModel = cashFlowService.DeleteCashFlow(objViewModel);


            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }

        [HttpGet]
        public JsonResult FillAccountHeadType(byte key)
        {
            CashFlowViewModel model = new CashFlowViewModel();
            model.AccountGroupKey = key;
            CashFlowViewModel objViewModel = new CashFlowViewModel();
            objViewModel = cashFlowService.FillAcountHeadType(model);
            return Json(objViewModel.AccountHeadTypes, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FillAccountHead(byte? Key, bool? isContra)
        {
            CashFlowViewModel model = new CashFlowViewModel();
            model.AccountGroupKey = Key ?? 0;
            model.IsContra = isContra ?? false;
            CashFlowViewModel objViewModel = new CashFlowViewModel();
            objViewModel = cashFlowService.FillAcountHead(model);
            return Json(objViewModel.AccountHeads, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult PrintRecieptList(int Id)
        {
            CashFlowViewModel model = new CashFlowViewModel();
            model.CashFlowKey = Id;
            model = cashFlowService.PrintCashFlowById(model);
            //if (model.Message != null && model.Message != "")
            //{
            //    

            //}
            return PartialView(model);
        }

        [HttpGet]
        public JsonResult GetBalance(short PaymentModeKey, long? RowKey, long? BankAccountKey, short? branchKey, byte? CashFlowTypeKey)
        {
            decimal Balance = cashFlowService.CheckShortBalance(PaymentModeKey, RowKey ?? 0, BankAccountKey ?? 0, branchKey ?? 0, CashFlowTypeKey ?? 0);
            return Json(Balance, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAccountHeadBalance(decimal? Amount, long? Rowkey, long? AccountHeadKey)
        {
            //decimal Balance = cashFlowService.GetAccountHeadBalance(Amount ?? 0, Rowkey ?? 0, AccountHeadKey ?? 0);
            //return Json(Balance, JsonRequestBehavior.AllowGet);
            return null;
        }

        //private void GetUserKey(CashFlowViewModel model)
        //{
        //    CookieProvider cookieProvider = new CookieProvider();
        //    HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
        //    if (authCookie != null)
        //    {
        //        CITSEduSuitePrincipalData userData = cookieProvider.GetCookie(authCookie);
        //        model.UserKey = userData.UserKey;
        //        model.RoleKey = userData.RoleKey;
        //    }

        //}

        [HttpGet]
        public ActionResult FillBankAccount(short? key, long? headKey)
        {
            CashFlowViewModel model = new CashFlowViewModel();
            model.BranchKey = key ?? 0;
            model.AccountHeadKey = headKey ?? 0;
            model = cashFlowService.FillBankAccounts(model);
            return Json(model.BankAccounts, JsonRequestBehavior.AllowGet);
        }

    }
}