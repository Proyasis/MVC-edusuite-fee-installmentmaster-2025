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
    public class DashBoardContentController : BaseController
    {
        private IDashBoardContentService DashBoardContentService;
        public DashBoardContentController(IDashBoardContentService objDashBoardContentService)
        {
            this.DashBoardContentService = objDashBoardContentService;
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.DashBoardContent, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult DashBoardContentList()
        {
            return View();
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.DashBoardContent, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditDashBoardContent(short? id)
        {
            DashBoardContentViewModel model = new DashBoardContentViewModel();
            model = DashBoardContentService.GetDashBoardContentById(id);
            if (model == null)
            {
                model = new DashBoardContentViewModel();
            }
            return PartialView(model);

        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.DashBoardContent, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult AddEditDashBoardContent(DashBoardContentViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = DashBoardContentService.CreateDashBoardContent(model);
                }
                else
                {
                    model = DashBoardContentService.UpdateDashBoardContent(model);
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
        public JsonResult GetDashBoardContent(string searchText)
        {
            int page = 1, rows = 15;
            List<DashBoardContentViewModel> DashBoardContentList = new List<DashBoardContentViewModel>();
            DashBoardContentList = DashBoardContentService.GetDashBoardContent(searchText);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = DashBoardContentList.Count();
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = DashBoardContentList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.DashBoardContent, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteDashBoardContent(short id)
        {
            DashBoardContentViewModel objViewModel = new DashBoardContentViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = DashBoardContentService.DeleteDashBoardContent(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }
    }
}