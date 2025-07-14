using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.Security;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Provider;
//using CITS.EduSuite.UI.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CITS.EduSuite.UI.Controllers
{
    [MessagesActionFilter]
    public class BankStatementController : BaseController
    {
        private IBankStatementService BankStatementService;

        public BankStatementController(IBankStatementService objBankStatementService)
        {
            this.BankStatementService = objBankStatementService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.BankStatement, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult BankStatementList()
        {
            BankStatementMasterViewModel model = new BankStatementMasterViewModel();
            //model.UserKey = GetUserKey();
            model = BankStatementService.GetBranches(model);
            return View(model);
        }

        public JsonResult GetBankStatement(string searchText, short? branchkey)
        {
            int page = 1, rows = 10;

            List<BankStatementMasterViewModel> BankStatementList = new List<BankStatementMasterViewModel>();
            BankStatementMasterViewModel model = new BankStatementMasterViewModel();
            model.BranchKey = branchkey ?? 0;
            BankStatementList = BankStatementService.GetBankStatements(model, searchText);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = BankStatementList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            if (BankStatementList.Count > 0)
            {
                model = BankStatementList[0];
                //if (model.ExceptionMessage != null && model.ExceptionMessage != "")
                //{
                //    model.UserKey = GetUserKey();
                //    ActivityLog.CreateActivityLog(model.UserKey, MenuConstant.BankStatement, ActionConstant.MenuAccess, DbConstants.LogType.Error, 0, model.ExceptionMessage);
                //}
            }

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = BankStatementList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.BankStatement, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditBankStatement(short? id)
        {
            BankStatementMasterViewModel model = new BankStatementMasterViewModel();
            model = BankStatementService.GetBankStatementById(id);
            if (model == null)
            {
                model = new BankStatementMasterViewModel();
            }
            //if (model.ExceptionMessage != null && model.ExceptionMessage != "")
            //{
            //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.BankStatement, id != null && id != 0 ? ActionConstant.Edit : ActionConstant.Add, DbConstants.LogType.Error, id, model.ExceptionMessage);
            //}

            //model.UserKey = GetUserKey();
            BankStatementService.GetBranches(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult AddEditBankStatement(BankStatementMasterViewModel BankStatement)
        {

            if (ModelState.IsValid)
            {
                if (BankStatement.RowKey == 0)
                {
                    BankStatement = BankStatementService.CreateBankStatementMaster(BankStatement);
                    //if (BankStatement.ExceptionMessage != null && BankStatement.ExceptionMessage != "")
                    //{
                    //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.BankStatement, ActionConstant.Add, DbConstants.LogType.Error, BankStatement.RowKey, BankStatement.ExceptionMessage);
                    //}
                    //else
                    //{
                    //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.BankStatement, ActionConstant.Add, DbConstants.LogType.Info, BankStatement.RowKey, BankStatement.Message);
                    //}
                }
                else
                {
                    BankStatement = BankStatementService.UpdateBankStatementMaster(BankStatement);
                    //if (BankStatement.ExceptionMessage != null && BankStatement.ExceptionMessage != "")
                    //{
                    //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.BankStatement, ActionConstant.Edit, DbConstants.LogType.Error, BankStatement.RowKey, BankStatement.ExceptionMessage);
                    //}
                    //else
                    //{
                    //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.BankStatement, ActionConstant.Edit, DbConstants.LogType.Info, BankStatement.RowKey, BankStatement.Message);
                    //}
                }

                if (BankStatement.Message != AppConstants.Common.SUCCESS)
                {
                    this.AddToastMessage(AppConstants.Common.FAILED, BankStatement.Message, ToastType.Error);
                    ModelState.AddModelError("error_msg", BankStatement.Message);
                }
                else
                {
                    this.AddToastMessage(AppConstants.Common.SUCCESS, BankStatement.Message, ToastType.Success);
                    return RedirectToAction("BankStatementList");
                }

                BankStatement.Message = "";
                return View(BankStatement);
            }

            BankStatement.Message = EduSuiteUIResources.Failed;
            return View(BankStatement);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.BankStatement, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditMultipleBankStatement(short? id)
        {
            BankStatementMasterViewModel model = new BankStatementMasterViewModel();
            model = BankStatementService.GetBankStatementById(id);
            if (model == null)
            {
                model = new BankStatementMasterViewModel();
            }
            //if (model.ExceptionMessage != null && model.ExceptionMessage != "")
            //{
            //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.BankStatement, id != null && id != 0 ? ActionConstant.Edit : ActionConstant.Add, DbConstants.LogType.Error, id, model.ExceptionMessage);
            //}

            //model.UserKey = GetUserKey();
            BankStatementService.GetBranches(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult AddEditMultipleBankStatement(BankStatementMasterViewModel BankStatement)
        {

            if (ModelState.IsValid)
            {
                if (BankStatement.RowKey == 0)
                {
                    BankStatement = BankStatementService.CreateBankStatementMaster(BankStatement);
                    //if (BankStatement.ExceptionMessage != null && BankStatement.ExceptionMessage != "")
                    //{
                    //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.BankStatement, ActionConstant.Add, DbConstants.LogType.Error, BankStatement.RowKey, BankStatement.ExceptionMessage);
                    //}
                    //else
                    //{
                    //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.BankStatement, ActionConstant.Add, DbConstants.LogType.Info, BankStatement.RowKey, BankStatement.Message);
                    //}
                }
                else
                {
                    BankStatement = BankStatementService.UpdateBankStatementMaster(BankStatement);
                    //if (BankStatement.ExceptionMessage != null && BankStatement.ExceptionMessage != "")
                    //{
                    //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.BankStatement, ActionConstant.Edit, DbConstants.LogType.Error, BankStatement.RowKey, BankStatement.ExceptionMessage);
                    //}
                    //else
                    //{
                    //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.BankStatement, ActionConstant.Edit, DbConstants.LogType.Info, BankStatement.RowKey, BankStatement.Message);
                    //}
                }

                if (BankStatement.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", BankStatement.Message);
                }
                else
                {
                    return RedirectToAction("BankStatementList");
                }

                BankStatement.Message = "";
                return View(BankStatement);
            }

            BankStatement.Message = EduSuiteUIResources.Failed;
            return View(BankStatement);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.BankStatement, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteBankStatement(Int16 id)
        {
            BankStatementMasterViewModel objViewModel = new BankStatementMasterViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = BankStatementService.DeleteBankStatement(objViewModel);
                //if (objViewModel.ExceptionMessage != null && objViewModel.ExceptionMessage != "")
                //{
                //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.BankStatement, ActionConstant.Delete, DbConstants.LogType.Error, id, objViewModel.ExceptionMessage);
                //}
                //else
                //{
                //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.BankStatement, ActionConstant.Delete, DbConstants.LogType.Info, id, objViewModel.Message);
                //}
            }
            catch (Exception ex)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.BankStatement, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteBankStatementItem(Int16 id)
        {
            BankStatementDetailsViewModel objViewModel = new BankStatementDetailsViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = BankStatementService.DeleteBankStatementItem(objViewModel);
                //if (objViewModel.ExceptionMessage != null && objViewModel.ExceptionMessage != "")
                //{
                //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.BankStatement, ActionConstant.DeleteItem, DbConstants.LogType.Error, id, objViewModel.ExceptionMessage);
                //}
                //else
                //{
                //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.BankStatement, ActionConstant.DeleteItem, DbConstants.LogType.Info, id, objViewModel.Message);
                //}
            }
            catch (Exception ex)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }

        [HttpGet]
        public JsonResult FillBankAccount(short? key)
        {
            BankStatementMasterViewModel model = new BankStatementMasterViewModel();
            model.BranchKey = key ?? 0;
            model = BankStatementService.FillBankAccounts(model);
            return Json(model.BankAccounts, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BankStatementDetailsList(long? rowKey, long? bankAccountKey, int? yearKey, byte? monthKey)
        {
            BankStatementMasterViewModel model = new BankStatementMasterViewModel();
            model.RowKey = rowKey ?? 0;
            model.BankAccountKey = bankAccountKey ?? 0;
            model.YearKey = yearKey ?? 0;
            model.MonthKey = monthKey ?? 0;
            model = BankStatementService.GetBankStatementDetails(model);
            return PartialView(model);
        }


        [HttpPost]
        public ActionResult ReadExcel(BankStatementMasterViewModel model)
        {
            return PartialView("BankStatementDetailsList", model);
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