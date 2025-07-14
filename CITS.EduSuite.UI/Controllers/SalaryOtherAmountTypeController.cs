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
    public class SalaryOtherAmountTypeController : BaseController
    {
        private ISalaryOtherAmountTypeService SalaryOtherAmountTypeService;
        public SalaryOtherAmountTypeController(ISalaryOtherAmountTypeService objSalaryOtherAmountTypeService)
        {
            this.SalaryOtherAmountTypeService = objSalaryOtherAmountTypeService;
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.SalaryOtherAmountType, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult SalaryOtherAmountTypeList()
        {
            return View();
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.SalaryOtherAmountType, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditSalaryOtherAmountType(short? id)
        {
            SalaryOtherAmountTypeViewModel model = new SalaryOtherAmountTypeViewModel();
            model = SalaryOtherAmountTypeService.GetSalaryOtherAmountTypeById(id);
            if (model == null)
            {
                model = new SalaryOtherAmountTypeViewModel();
            }
            return PartialView(model);

        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.SalaryOtherAmountType, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult AddEditSalaryOtherAmountType(SalaryOtherAmountTypeViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = SalaryOtherAmountTypeService.CreateSalaryOtherAmountType(model);
                }
                else
                {
                    model = SalaryOtherAmountTypeService.UpdateSalaryOtherAmountType(model);
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
        public JsonResult GetSalaryOtherAmountType(string searchText)
        {
            int page = 1, rows = 15;
            List<SalaryOtherAmountTypeViewModel> SalaryOtherAmountTypeList = new List<SalaryOtherAmountTypeViewModel>();
            SalaryOtherAmountTypeList = SalaryOtherAmountTypeService.GetSalaryOtherAmountType(searchText);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = SalaryOtherAmountTypeList.Count();
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = SalaryOtherAmountTypeList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.SalaryOtherAmountType, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteSalaryOtherAmountType(short id)
        {
            SalaryOtherAmountTypeViewModel objViewModel = new SalaryOtherAmountTypeViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = SalaryOtherAmountTypeService.DeleteSalaryOtherAmountType(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }
    }
}