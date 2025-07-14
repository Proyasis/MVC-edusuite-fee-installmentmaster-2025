using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Services;

namespace CITS.EduSuite.UI.Controllers
{
    public class EnquiryCallStatusController : BaseController
    {
        private IEnquiryCallStatusService EnquiryCallStatusService;

        public EnquiryCallStatusController(IEnquiryCallStatusService objEnquiryCallStatusService)
        {
            this.EnquiryCallStatusService = objEnquiryCallStatusService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EnquiryCallStatus, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult EnquiryCallStatusList()
        {
            return View();
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EnquiryCallStatus, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditEnquiryCallStatus(short? id)
        {
            EnquiryCallStatusViewModel model = new EnquiryCallStatusViewModel();
            model = EnquiryCallStatusService.GetEnquiryCallStatusById(id);
            if (model == null)
            {
                model = new EnquiryCallStatusViewModel();
            }
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult AddEditEnquiryCallStatus(EnquiryCallStatusViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = EnquiryCallStatusService.CreateEnquiryCallStatus(model);
                }
                else
                {
                    model = EnquiryCallStatusService.UpdateEnquiryCallStatus(model);
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
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EnquiryCallStatus, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteEnquiryCallStatus(Int16 id)
        {
            EnquiryCallStatusViewModel objViewModel = new EnquiryCallStatusViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = EnquiryCallStatusService.DeleteEnquiryCallStatus(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }

        public JsonResult GetEnquiryCallStatus(string searchText)
        {
            int page = 1, rows = 10;

            List<EnquiryCallStatusViewModel> EnquiryCallStatusList = new List<EnquiryCallStatusViewModel>();
            EnquiryCallStatusList = EnquiryCallStatusService.GetEnquiryCallStatus(searchText);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = EnquiryCallStatusList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = EnquiryCallStatusList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
    }
}