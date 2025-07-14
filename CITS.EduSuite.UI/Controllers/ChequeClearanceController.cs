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
    public class ChequeClearanceController : BaseController 
    {
        private IChequeClearanceService ChequeClearanceService;

        public ChequeClearanceController(IChequeClearanceService objChequeClearanceService)
        {
            this.ChequeClearanceService = objChequeClearanceService;

        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.ChequeClearance, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult ChequeClearanceList()
        {
            ChequeClearanceViewModel model = new ChequeClearanceViewModel();
            //model.UserKey = GetUserKey();
            model = ChequeClearanceService.FillBranch(model);
            return View(model);
        }
        //[ActionAuthenticationAttribute(MenuCode = MenuConstants.ChequeClearance, ActionCode = ActionConstants.Clear)]
        [HttpGet]
        public ActionResult ClearCheques(long? id, byte? type)
        {
            ChequeClearanceViewModel model = new ChequeClearanceViewModel();
            model.TransactionTypeKey = type ?? 0;
            model.TransactionKey = id ?? 0;
            model = ChequeClearanceService.GetChequeClearanceById(model);
            if (model == null)
            {
                model = new ChequeClearanceViewModel();
            }
            //if (model.ExceptionMessage != null && model.ExceptionMessage != "")
            //{
            //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.ChequeClearance, ActionConstant.Clear, DbConstants.LogType.Error,id, model.ExceptionMessage);
            //}
            return PartialView(model);

        }
        [HttpPost]
        public ActionResult ClearCheques(ChequeClearanceViewModel ChequeClearance)
        {
            if (ModelState.IsValid)
            {
                ChequeClearance = ChequeClearanceService.CreateChequeClearance(ChequeClearance);
                //if (ChequeClearance.ExceptionMessage != null && ChequeClearance.ExceptionMessage != "")
                //{
                //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.ChequeClearance, ActionConstant.Add, DbConstants.LogType.Error,ChequeClearance.RowKey, ChequeClearance.ExceptionMessage);
                //}
                //else
                //{
                //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.ChequeClearance, ActionConstant.Add, DbConstants.LogType.Info,ChequeClearance.RowKey, ChequeClearance.Message);
                //}
                if (ChequeClearance.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", ChequeClearance.Message);
                }
                else
                {
                    return Json(ChequeClearance);
                }

                ChequeClearance.Message = "";
                return PartialView(ChequeClearance);
            }
            ChequeClearance.Message = EduSuiteUIResources.Failed;
            return PartialView(ChequeClearance);
        }

        public JsonResult GetChequeClearance(string searchText, short? branchKey)
        {
            int page = 1, rows = 15;
            List<ChequeClearanceViewModel> ChequeClearanceList = new List<ChequeClearanceViewModel>();
            ChequeClearanceList = ChequeClearanceService.GetChequeClearance(searchText, branchKey ?? 0);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = ChequeClearanceList.Count();
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            if (ChequeClearanceList.Count > 0)
            {
                ChequeClearanceViewModel model = ChequeClearanceList[0];
                //if (model.ExceptionMessage != null && model.ExceptionMessage != "")
                //{
                //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.ChequeClearance, ActionConstant.MenuAccess, DbConstants.LogType.Error,0, model.ExceptionMessage);
                //}
            }

            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = ChequeClearanceList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetBalance(short PaymentModeKey, long? RowKey, long? BankAccountKey,short? branchKey)
        {
            decimal Balance = ChequeClearanceService.CheckShortBalance(PaymentModeKey, RowKey ?? 0, BankAccountKey ?? 0, branchKey ?? 0);
            return Json(Balance, JsonRequestBehavior.AllowGet);
        }
        //[HttpGet]
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