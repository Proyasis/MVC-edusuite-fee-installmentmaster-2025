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
    public class DepreciationController : BaseController
    {
        private IDepreciationService DepreciationService;

        public DepreciationController(IDepreciationService objDepreciationService)
        {
            this.DepreciationService = objDepreciationService;

        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Depreciation, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult DepreciationList()
        {
            return View();
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Depreciation, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditDepreciation(int? id, long? AssetDetailKey, int? Period)
        {
            var Depreciation = DepreciationService.GetDepreciationById(id, AssetDetailKey, Period);
            if (Depreciation == null)
            {
                Depreciation = new DepreciationViewModel();
            }
            return PartialView(Depreciation);

        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Depreciation, ActionCode = ActionConstants.View)]
        [HttpGet]
        public ActionResult ViewDepreciation(int? id)
        {
            var Depreciation = DepreciationService.ViewDepreciation(id);
            if (Depreciation == null)
            {
                Depreciation = new DepreciationViewModel();
            }
            return PartialView(Depreciation);

        }

        [HttpPost]
        public ActionResult AddEditDepreciation(DepreciationViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = DepreciationService.CreateDepreciation(model);
                }
                else
                {
                    model = DepreciationService.UpdateDepreciation(model);
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
            model.Message =  EduSuiteUIResources.Failed;  

            return PartialView(model);
        }

        public JsonResult GetDepreciation(string searchText, string sidx, string sord, int page, int rows)
        {
            List<DepreciationViewModel> DepreciationList = new List<DepreciationViewModel>();
            DepreciationViewModel model = new DepreciationViewModel();
            model.PageIndex = page;
            model.PageSize = rows;
            model.SortBy = sidx;
            model.SortOrder = sord;
            DepreciationList = DepreciationService.GetDepreciation(searchText, model);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = model.TotalRecords;
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            if (DepreciationList.Count > 0)
            {
                model = DepreciationList[0];
            }

            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = DepreciationList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Depreciation, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteDepreciation(int id)
        {
            DepreciationViewModel objViewModel = new DepreciationViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = DepreciationService.DeleteDepreciation(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message =  EduSuiteUIResources.Failed;  
            }
            return Json(objViewModel);
        }
        
    }
}