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
    public class CasteController : BaseController
    {
        private ICasteService CasteService;
        public CasteController(ICasteService objCasteService)
        {
            this.CasteService = objCasteService;
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Caste, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult CasteList()
        {
            return View();
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Caste, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditCaste(short? id)
        {
            CasteViewModel model = new CasteViewModel();
            model = CasteService.GetCasteById(id);
            if (model == null)
            {
                model = new CasteViewModel();
            }
            return PartialView(model);

        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Caste, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult AddEditCaste(CasteViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = CasteService.CreateCaste(model);
                }
                else
                {
                    model = CasteService.UpdateCaste(model);
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
        public JsonResult GetCaste(string searchText)
        {
            int page = 1, rows = 15;
            List<CasteViewModel> CasteList = new List<CasteViewModel>();
            CasteList = CasteService.GetCaste(searchText);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = CasteList.Count();
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = CasteList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Caste, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteCaste(short id)
        {
            CasteViewModel objViewModel = new CasteViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = CasteService.DeleteCaste(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }
    }
}