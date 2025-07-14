using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CITS.EduSuite.UI.Controllers
{
    public class MarkGroupController : BaseController
    {
        // GET: MarkGroup
        private IMarkGroupService markGroupService;

        public MarkGroupController(IMarkGroupService objMarkGroupService)
        {
            this.markGroupService = objMarkGroupService;
          
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.MarkGroup, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult MarkGroupList()
        {
            MarkGroupViewModel model = new MarkGroupViewModel();
            return View();
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.MarkGroup, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditMarkGroup(int? id)
        {
            MarkGroupViewModel model = new MarkGroupViewModel();
            model = markGroupService.GetMarkGroupById(id);       
            if (model == null)
            {
                model = new MarkGroupViewModel();
            }
            return PartialView(model);

        }

        [HttpPost]
        public ActionResult AddEditMarkGroup(MarkGroupViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = markGroupService.CreateMarkGroup(model);
                }
                else
                {
                    model = markGroupService.UpdateMarkGroup(model);
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

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.MarkGroup, ActionCode = ActionConstants.View)]
        public JsonResult GetMarkGroup(string searchText)
        {
            int page = 1, rows = 15;
            List<MarkGroupViewModel> markGroupList = new List<MarkGroupViewModel>();
            markGroupList = markGroupService.GetMarkGroup(searchText);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = markGroupList.Count();
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = markGroupList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.MarkGroup, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteMarkGroup(short id)
        {
            MarkGroupViewModel objViewModel = new MarkGroupViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = markGroupService.DeleteMarkGroup(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }

    }
}