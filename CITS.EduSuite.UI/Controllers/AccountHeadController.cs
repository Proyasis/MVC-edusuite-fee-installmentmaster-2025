using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
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
    public class AccountHeadController : BaseController
    {
        private IAccountHeadService AccountHeadService;
        public AccountHeadController(IAccountHeadService objAccountHeadService)
        {
            this.AccountHeadService = objAccountHeadService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.AccountHead, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult AccountHeadList()
        {
            AccountHeadViewModel model = new AccountHeadViewModel();
            AccountHeadService.FillAccountGroup(model);
            AccountHeadService.FillSearchAccountHeadType(model);
            return View(model);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.AccountHead, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditAccountHead(int? id, short? type, byte? group)
        {
            AccountHeadViewModel model = new AccountHeadViewModel();
            model = AccountHeadService.GetAccountHeadById(id, type, group);
            if (model == null)
            {
                model = new AccountHeadViewModel();
            }
            model.AccountGroupKey = group ?? model.AccountGroupKey;
            model.AccountHeadTypeKey = type ?? model.AccountHeadTypeKey;
            return PartialView(model);

        }
        [HttpPost]
        public ActionResult AddEditAccountHead(AccountHeadViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = AccountHeadService.CreateAccountHead(model);
                }
                else
                {
                    model = AccountHeadService.UpdateAccountHead(model);
                }

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    return Json(model);
                }


                return PartialView(model);
            }



            model.Message = EduSuiteUIResources.Failed;

            return PartialView(model);
        }
        public JsonResult GetAccountHead(string searchText, byte? SearchAccountGroupKey, short? SearchAccountHeadTypeKey, string sidx, string sord, int page, int rows)
        {
            List<AccountHeadViewModel> AccountHeadList = new List<AccountHeadViewModel>();

            AccountHeadViewModel model = new AccountHeadViewModel();
            model.PageIndex = page;
            model.PageSize = rows;
            model.SortBy = sidx;
            model.SortOrder = sord;
            searchText = searchText == null ? "" : searchText;
            model.SearchAccountGroupKey = SearchAccountGroupKey ?? 0;
            model.SearchAccountHeadTypeKey = SearchAccountHeadTypeKey ?? 0;

            AccountHeadList = AccountHeadService.GetAccountHead(searchText, model);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = model.TotalRecords;
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            if (AccountHeadList.Count > 0)
            {
                model = AccountHeadList[0];

            }

            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = AccountHeadList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.AccountHead, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteAccountHead(short id)
        {
            AccountHeadViewModel objViewModel = new AccountHeadViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = AccountHeadService.DeleteAccountHead(objViewModel);
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
            AccountHeadViewModel model = new AccountHeadViewModel();
            model.AccountGroupKey = key;
            model = AccountHeadService.FillAccountHeadType(model);
            return Json(model.AccountHeadType, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult FillSearchAccountHeadType(byte key)
        {
            AccountHeadViewModel model = new AccountHeadViewModel();
            model.SearchAccountGroupKey = key;
            model = AccountHeadService.FillSearchAccountHeadType(model);
            return Json(model.SearchAccountHeadType, JsonRequestBehavior.AllowGet);
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