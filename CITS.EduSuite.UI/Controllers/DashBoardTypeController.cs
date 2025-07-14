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
    public class DashBoardTypeController : BaseController
    {
        private IDashBoardTypeService DashBoardTypeService;
        public DashBoardTypeController(IDashBoardTypeService objDashBoardTypeService)
        {
            this.DashBoardTypeService = objDashBoardTypeService;
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.DashBoardType, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult DashBoardTypeList()
        {
            return View();
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.DashBoardType, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditDashBoardType(short? id)
        {
            DashBoardTypeViewModel model = new DashBoardTypeViewModel();
            model = DashBoardTypeService.GetDashBoardTypeById(id);
            if (model == null)
            {
                model = new DashBoardTypeViewModel();
            }
            return PartialView(model);

        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.DashBoardType, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult AddEditDashBoardType(DashBoardTypeViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = DashBoardTypeService.CreateDashBoardType(model);
                }
                else
                {
                    model = DashBoardTypeService.UpdateDashBoardType(model);
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
        public JsonResult GetDashBoardType(string searchText)
        {
            int page = 1, rows = 15;
            List<DashBoardTypeViewModel> DashBoardTypeList = new List<DashBoardTypeViewModel>();
            DashBoardTypeList = DashBoardTypeService.GetDashBoardType(searchText);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = DashBoardTypeList.Count();
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = DashBoardTypeList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.DashBoardType, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteDashBoardType(short id)
        {
            DashBoardTypeViewModel objViewModel = new DashBoardTypeViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = DashBoardTypeService.DeleteDashBoardType(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }
    }
}