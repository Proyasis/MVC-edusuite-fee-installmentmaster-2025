using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Provider;
using System.Web.Security;
using CITS.EduSuite.Business.Models.Security;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI.Controllers
{
    public class MenuController : BaseController
    {
        private IMenuDetailViewService menuDetailViewService;
        private ISelectListService selectListService;
        public MenuController(IMenuDetailViewService objmenuDetailViewService, ISelectListService objselectListService)
        {
            this.menuDetailViewService = objmenuDetailViewService;
            this.selectListService = objselectListService;
        }

        [HttpGet]
        public ActionResult MenuDetailViewList()
        {
            MenuDetailViewModel MenuDetailView = new MenuDetailViewModel();
            MenuDetailView.MenuTypes = selectListService.FillMenuType();
            return View(MenuDetailView);
        }

        [HttpGet]
        public JsonResult GetMenuDetailView(string searchText, short? MenuTypekey)
        {
            int page = 1, rows = 10;

            List<MenuDetailViewModel> MenuDetailViewList = new List<MenuDetailViewModel>();
            MenuDetailViewModel model = new MenuDetailViewModel();
            model.MenuName = searchText;
            model.MenuTypeKey = MenuTypekey;
            MenuDetailViewList = menuDetailViewService.GetMenuDetailView(model);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = MenuDetailViewList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = MenuDetailViewList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Menu, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditMenuDetailView(int? id)
        {
            MenuDetailViewModel model = new MenuDetailViewModel();
            model = menuDetailViewService.GetMenuDetailViewById(id);
            if (model == null)
            {
                model = new MenuDetailViewModel();
            }
            model.MenuTypes = selectListService.FillMenuType();
            model.Actions = selectListService.FillActions();
            model.MenuCatagories = selectListService.FillMenuCatagory();
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult AddEditMenuDetailView(MenuDetailViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = menuDetailViewService.CreateMenuDetailView(model);
                }
                else
                {
                    model = menuDetailViewService.UpdateMenuDetailView(model);
                }

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    return Json(model);
                }


                return Json(model);
            }
            foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            {
            }

            model.Message = EduSuiteUIResources.Failed;
            return Json(model);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Menu, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteMenuDetailView(Int16 id)
        {
            MenuDetailViewModel objViewModel = new MenuDetailViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = menuDetailViewService.DeleteMenuDetailView(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }


        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Menu, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteMenuAction(Int16 id)
        {
            MenuDetailViewModel objViewModel = new MenuDetailViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = menuDetailViewService.DeleteMenuAction(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }
    }
}