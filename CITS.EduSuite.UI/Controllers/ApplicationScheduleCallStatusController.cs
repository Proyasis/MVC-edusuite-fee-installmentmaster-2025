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
    public class ApplicationScheduleCallStatusController : Controller
    {
        private IApplicationScheduleCallStatusService ApplicationScheduleCallStatusService;

        public ApplicationScheduleCallStatusController(IApplicationScheduleCallStatusService objApplicationScheduleCallStatusService)
        {
            this.ApplicationScheduleCallStatusService = objApplicationScheduleCallStatusService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.ApplicationScheduleCallStatus, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult ApplicationScheduleCallStatusList()
        {
            return View();
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.ApplicationScheduleCallStatus, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditApplicationScheduleCallStatus(short? id)
        {
            ApplicationScheduleCallStatusViewModel model = new ApplicationScheduleCallStatusViewModel();
            model = ApplicationScheduleCallStatusService.GetApplicationScheduleCallStatusById(id);
            if (model == null)
            {
                model = new ApplicationScheduleCallStatusViewModel();
            }
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult AddEditApplicationScheduleCallStatus(ApplicationScheduleCallStatusViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = ApplicationScheduleCallStatusService.CreateApplicationScheduleCallStatus(model);
                }
                else
                {
                    model = ApplicationScheduleCallStatusService.UpdateApplicationScheduleCallStatus(model);
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
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.ApplicationScheduleCallStatus, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteApplicationScheduleCallStatus(Int16 id)
        {
            ApplicationScheduleCallStatusViewModel objViewModel = new ApplicationScheduleCallStatusViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = ApplicationScheduleCallStatusService.DeleteApplicationScheduleCallStatus(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }

        public JsonResult GetApplicationScheduleCallStatus(string searchText)
        {
            int page = 1, rows = 10;

            List<ApplicationScheduleCallStatusViewModel> ApplicationScheduleCallStatusList = new List<ApplicationScheduleCallStatusViewModel>();
            ApplicationScheduleCallStatusList = ApplicationScheduleCallStatusService.GetApplicationScheduleCallStatus(searchText);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = ApplicationScheduleCallStatusList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = ApplicationScheduleCallStatusList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
    }
}