using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.UI.Controllers
{
    public class MenuTypeController : BaseController
    {
        private IMenuTypeService MenuTypeService;
        public MenuTypeController(IMenuTypeService objMenuTypeService)
        {
            this.MenuTypeService = objMenuTypeService;
        }
        [HttpGet]
        public ActionResult MenuTypeList()
        {
            return View();
        }
        [HttpGet]
        public ActionResult AddEditMenuType(int? id)
        {
            MenuTypeViewModel model = new MenuTypeViewModel();
            model = MenuTypeService.GetMenuTypeById(id);
            if (model == null)
            {
                model = new MenuTypeViewModel();
            }
            return PartialView(model);

        }
        [HttpPost]
        public ActionResult AddEditMenuType(MenuTypeViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = MenuTypeService.CreateMenuType(model);
                }
                else
                {
                    model = MenuTypeService.UpdateMenuType(model);
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
        public JsonResult GetMenuType(string searchText)
        {
            int page = 1, rows = 15;
            List<MenuTypeViewModel> MenuTypeList = new List<MenuTypeViewModel>();
            MenuTypeList = MenuTypeService.GetMenuType(searchText);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = MenuTypeList.Count();
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = MenuTypeList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteMenuType(short id)
        {
            MenuTypeViewModel objViewModel = new MenuTypeViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = MenuTypeService.DeleteMenuType(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }
    }
}