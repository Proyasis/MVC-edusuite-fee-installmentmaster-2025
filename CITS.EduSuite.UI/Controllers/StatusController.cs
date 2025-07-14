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
    public class StatusController : BaseController
    {
        private IStatusService statusService;

        public StatusController(IStatusService objStatusService)
        {
            this.statusService = objStatusService;
        }

        [HttpGet]
        public ActionResult StatusList()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetStatuses(string searchText)
        {
            int page = 1, rows = 10;

            List<StatusViewModel> statusList = new List<StatusViewModel>();
            statusList = statusService.GetStatuses(searchText);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = statusList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = statusList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddEditStatus(short? id)
        {
            var Statuses = statusService.GetStatusById(id);
            if (Statuses == null)
            {
                Statuses = new StatusViewModel();
            }
            return View(Statuses);
        }

        [HttpPost]
        public ActionResult AddEditStatus(StatusViewModel status)
        {

            if (ModelState.IsValid)
            {
                if (status.RowKey == 0)
                {
                    status = statusService.CreateStatus(status);
                }
                else
                {
                    status = statusService.UpdateStatus(status);
                }

                if (status.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", status.Message);
                }
                else
                {
                    return RedirectToAction("StatusList");
                }

                status.Message = "";
                return View(status);
            }

            status.Message = EduSuiteUIResources.Failed;
            return View(status);

        }

        [HttpPost]
        public ActionResult DeleteStatus(Int16 id)
        {
            StatusViewModel objViewModel = new StatusViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = statusService.DeleteStatus(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }

    }
}