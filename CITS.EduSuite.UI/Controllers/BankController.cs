using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CITS.EduSuite.UI.Controllers
{
    public class BankController : BaseController
    {
        private IBankService BankService;
        public BankController(IBankService objBankService)
        {
            this.BankService = objBankService;
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Bank, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult BankList()
        {
            return View();
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Bank, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditBank(short? id)
        {
            BankViewModel model = new BankViewModel();
            model = BankService.GetBankById(id);
            if (model == null)
            {
                model = new BankViewModel();
            }
            return PartialView(model);

        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Bank, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult AddEditBank(BankViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = BankService.CreateBank(model);
                }
                else
                {
                    model = BankService.UpdateBank(model);
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
            model.Message = EduSuiteUIResources.Failed;

            return PartialView(model);
        }
        public JsonResult GetBank(string searchText)
        {
            int page = 1, rows = 15;
            List<BankViewModel> BankList = new List<BankViewModel>();
            BankList = BankService.GetBank(searchText);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = BankList.Count();
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = BankList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Bank, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteBank(short id)
        {
            BankViewModel objViewModel = new BankViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = BankService.DeleteBank(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }
    }
}