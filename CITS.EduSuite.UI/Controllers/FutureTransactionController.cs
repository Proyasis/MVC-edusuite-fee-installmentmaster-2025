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
    public class FutureTransactionController : BaseController
    {
        private IFutureTransactionService FutureTransactionService;
        public FutureTransactionController(IFutureTransactionService objFutureTransactionService)
        {
            this.FutureTransactionService = objFutureTransactionService;
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.FutureTransaction, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult FutureTransactionList()
        {
            FutureTransactionViewModel model = new FutureTransactionViewModel();
            //model.UserKey = GetUserKey();
            model = FutureTransactionService.FillBranches(model);
            return View(model);
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.FutureTransaction, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditFutureTransaction(long? id)
        {
            FutureTransactionViewModel model = new FutureTransactionViewModel();
            //model.UserKey = GetUserKey();
            model.RowKey = id ?? 0;
            var FutureTransaction = FutureTransactionService.GetFutureTransactionById(model);
            if (FutureTransaction == null)
            {
                FutureTransaction = new FutureTransactionViewModel();
            }
            //if (model.ExceptionMessage != null && model.ExceptionMessage != "")
            //{
            //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.FutureTransaction, id != null && id != 0 ? ActionConstant.Edit : ActionConstant.Add, DbConstants.LogType.Error, id, model.ExceptionMessage);
            //}
            return PartialView(FutureTransaction);
        }
        [HttpPost]
        public ActionResult AddEditFutureTransaction(FutureTransactionViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = FutureTransactionService.CreateFutureTransaction(model);
                    //if (model.ExceptionMessage != null && model.ExceptionMessage != "")
                    //{
                    //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.FutureTransaction, ActionConstant.Add, DbConstants.LogType.Error, model.RowKey, model.ExceptionMessage);
                    //}
                    //else
                    //{
                    //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.FutureTransaction, ActionConstant.Add, DbConstants.LogType.Info, model.RowKey, model.Message);
                    //}
                }
                else
                {
                    model = FutureTransactionService.UpdateFutureTransaction(model);
                    //if (model.ExceptionMessage != null && model.ExceptionMessage != "")
                    //{
                    //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.FutureTransaction, ActionConstant.Edit, DbConstants.LogType.Error, model.RowKey, model.ExceptionMessage);
                    //}
                    //else
                    //{
                    //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.FutureTransaction, ActionConstant.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                    //}
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
        public JsonResult GetFutureTransaction(string searchText, short? branchkey, string sidx, string sord, int page, int rows)
        {
            List<FutureTransactionViewModel> FutureTransactionList = new List<FutureTransactionViewModel>();
            FutureTransactionViewModel model = new FutureTransactionViewModel();
            model.BranchKey = branchkey ?? 0;
            model.PageIndex = page;
            model.PageSize = rows;
            model.SortBy = sidx;
            model.SortOrder = sord;

            FutureTransactionList = FutureTransactionService.GetFutureTransaction(model, searchText);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = model.TotalRecords;
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            if (FutureTransactionList.Count > 0)
            {
                model = FutureTransactionList[0];
                //if (model.ExceptionMessage != null && model.ExceptionMessage != "")
                //{
                //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.JobCard, ActionConstant.MenuAccess, DbConstants.LogType.Error, 0, model.ExceptionMessage);
                //}
            }

            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = FutureTransactionList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.FutureTransaction, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteFutureTransaction(int id)
        {
            FutureTransactionViewModel objViewModel = new FutureTransactionViewModel();
            objViewModel.RowKey = id;
            try
            {
                objViewModel = FutureTransactionService.DeleteFutureTransaction(objViewModel);
                //if (objViewModel.ExceptionMessage != null && objViewModel.ExceptionMessage != "")
                //{
                //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.FutureTransaction, ActionConstant.Delete, DbConstants.LogType.Error, id, objViewModel.ExceptionMessage);
                //}
                //else
                //{
                //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.FutureTransaction, ActionConstant.Delete, DbConstants.LogType.Info, id, objViewModel.Message);
                //}
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.FutureTransaction, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteFutureTransactionItem(long id)
        {
            FutureTransactionOtherAmountTypeViewModel objViewModel = new FutureTransactionOtherAmountTypeViewModel();
            FutureTransactionViewModel objFutureTransactionViewModel = new FutureTransactionViewModel();
            objViewModel.RowKey = id;
            try
            {
                objFutureTransactionViewModel = FutureTransactionService.DeleteFutureTransactionOtherAmountTypeItem(objViewModel);
                //if (objFutureTransactionViewModel.ExceptionMessage != null && objFutureTransactionViewModel.ExceptionMessage != "")
                //{
                //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.FutureTransaction, ActionConstant.OtherDeleteItem, DbConstants.LogType.Error, id, objFutureTransactionViewModel.ExceptionMessage);
                //}
                //else
                //{
                //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.FutureTransaction, ActionConstant.OtherDeleteItem, DbConstants.LogType.Info, id, objFutureTransactionViewModel.Message);
                //}
            }
            catch (Exception)
            {
                objFutureTransactionViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objFutureTransactionViewModel);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.FutureTransaction, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteFutureTransactionPayment(long id)
        {
            FutureTransactionViewModel objFutureTransactionViewModel = new FutureTransactionViewModel();
            try
            {
                objFutureTransactionViewModel = FutureTransactionService.DeleteFutureTransactionPayment(id);
                //if (objFutureTransactionViewModel.ExceptionMessage != null && objFutureTransactionViewModel.ExceptionMessage != "")
                //{
                //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.FutureTransaction, ActionConstant.DeleteItem, DbConstants.LogType.Error, id, objFutureTransactionViewModel.ExceptionMessage);
                //}
                //else
                //{
                //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.FutureTransaction, ActionConstant.DeleteItem, DbConstants.LogType.Info, id, objFutureTransactionViewModel.Message);
                //}
            }
            catch (Exception)
            {
                objFutureTransactionViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objFutureTransactionViewModel);
        }
        //Payment
       // [ActionAuthenticationAttribute(MenuCode = MenuConstants.FutureTransaction, ActionCode = ActionConstants.Payment)]
        [HttpGet]
        public ActionResult AddEditFutureTransactionPayment(long? id)
        {
            FutureTransactionPaymentViewModel model = FutureTransactionService.GetFutureTransactionPaymentById(id ?? 0);
            //model.UserKey = GetUserKey();
            var Url = "~/Views/FutureTransaction/FutureTransactionPayment.cshtml";
            //if (model.ExceptionMessage != null && model.ExceptionMessage != "")
            //{
            //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.FutureTransaction, ActionConstant.Payment, DbConstants.LogType.Error, id, model.ExceptionMessage);
            //}
            return PartialView(Url, model);
        }
        [HttpPost]
        public ActionResult AddEditFutureTransactionPayment(FutureTransactionPaymentViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.PaymentKey == 0)
                {
                    model = FutureTransactionService.CallFutureTransactionPayment(model);
                    //if (model.ExceptionMessage != null && model.ExceptionMessage != "")
                    //{
                    //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.FutureTransaction, ActionConstant.Payment, DbConstants.LogType.Error, model.PaymentKey, model.ExceptionMessage);
                    //}
                    //else
                    //{
                    //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.FutureTransaction, ActionConstant.Payment, DbConstants.LogType.Info, model.PaymentKey, model.Message);
                    //}

                }
                else
                {
                    //model = FutureTransactionService.UpdateSalesOrderPayment(model);
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

            model.Message = EduSuiteUIResources.Failed;
            return PartialView(model);

        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.FutureTransaction, ActionCode = ActionConstants.View)]
        [HttpGet]
        public ActionResult ViewFutureTransaction(int? id)
        {
            var FutureTransaction = FutureTransactionService.ViewFutureTransactionById(id);
            if (FutureTransaction == null)
            {
                FutureTransaction = new FutureTransactionViewModel();
            }
            //if (FutureTransaction.ExceptionMessage != null && FutureTransaction.ExceptionMessage != "")
            //{
            //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.FutureTransaction, ActionConstant.View, DbConstants.LogType.Error, id, FutureTransaction.ExceptionMessage);
            //}

            //return View(FutureTransaction);
            return PartialView(FutureTransaction);
        }

        [HttpGet]
        public JsonResult GetBalance(short PaymentModeKey, long? FutureTransactionPaymentRowKey, long? BankAccountKey, short? branchKey, byte? CashFlowTypeKey)
        {
            decimal Balance = FutureTransactionService.CheckShortBalance(PaymentModeKey, FutureTransactionPaymentRowKey ?? 0, BankAccountKey ?? 0, branchKey ?? 0, CashFlowTypeKey ?? 0);
            return Json(Balance, JsonRequestBehavior.AllowGet);
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

        [HttpGet]
        public JsonResult CheckBillNumberExists(string BillNo, long? RowKey)
        {
            FutureTransactionViewModel appUser = new FutureTransactionViewModel();
            appUser = FutureTransactionService.CheckBillNumberExists(BillNo, RowKey);
            return Json(appUser.IsSuccessful, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetComapanyStateKey(short? branchKey)
        {
            short stateKey = FutureTransactionService.GetComapanyStateKey(branchKey ?? 0);
            return Json(stateKey, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult FillBankAccount(short? key)
        {
            FutureTransactionViewModel model = new FutureTransactionViewModel();
            model.BranchKey = key ?? 0;
            model = FutureTransactionService.FillBankAccounts(model);
            return Json(model.BankAccounts, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetAccountGroup(long? accountHeadKey)
        {
            byte gropKey = FutureTransactionService.GetAccountGroup(accountHeadKey ?? 0);
            return Json(gropKey, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult FutureTransactionCalculation(long? masterKey,byte? cashFlowTypeKey)
        {
            FutureTransactionPaymentViewModel model = new FutureTransactionPaymentViewModel();
            model.MasterKey = masterKey ?? 0;
            model.CashFlowTypeKey = cashFlowTypeKey ?? 0;
            model = FutureTransactionService.FutureTransactionCalculation(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FillAccountHead(byte? Key)
        {
            FutureTransactionViewModel model = new FutureTransactionViewModel();
            model.AccountGroupKey = Key ?? 0;
            model = FutureTransactionService.FillAccountHeads(model);
            return Json(model.AccountHeads, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetHSNCodeDetails(long? id)
        {
            FutureTransactionViewModel model = new FutureTransactionViewModel();
            model.HSNCodeKey = id ?? 0;
            model = FutureTransactionService.GetHSNCodeDetailsById(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}