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
    public class ApplicationScheduleTypeController : BaseController
    {
        private IApplicationScheduleTypeService ApplicationScheduleTypeService;

        public ApplicationScheduleTypeController(IApplicationScheduleTypeService objApplicationScheduleTypeService)
        {
            this.ApplicationScheduleTypeService = objApplicationScheduleTypeService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.ApplicationScheduleType, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult ApplicationScheduleTypeList()
        {
            return View();
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.ApplicationScheduleType, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditApplicationScheduleType(short? id)
        {
            ApplicationScheduleTypeViewModel model = new ApplicationScheduleTypeViewModel();
            model = ApplicationScheduleTypeService.GetApplicationScheduleTypeById(id);
            if (model == null)
            {
                model = new ApplicationScheduleTypeViewModel();
            }
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult AddEditApplicationScheduleType(ApplicationScheduleTypeViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = ApplicationScheduleTypeService.CreateApplicationScheduleType(model);
                }
                else
                {
                    model = ApplicationScheduleTypeService.UpdateApplicationScheduleType(model);
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
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.ApplicationScheduleType, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteApplicationScheduleType(Int16 id)
        {
            ApplicationScheduleTypeViewModel objViewModel = new ApplicationScheduleTypeViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = ApplicationScheduleTypeService.DeleteApplicationScheduleType(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }

        public JsonResult GetApplicationScheduleType(string searchText)
        {
            int page = 1, rows = 10;

            List<ApplicationScheduleTypeViewModel> ApplicationScheduleTypeList = new List<ApplicationScheduleTypeViewModel>();
            ApplicationScheduleTypeList = ApplicationScheduleTypeService.GetApplicationScheduleType(searchText);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = ApplicationScheduleTypeList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = ApplicationScheduleTypeList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
    }
}