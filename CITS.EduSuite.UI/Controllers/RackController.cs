using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Services;
using CITS.EduSuite.Business.Provider;
using System.Web.Security;
using CITS.EduSuite.Business.Models.Security;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI.Controllers
{
    public class RackController : BaseController
    {

        private IRackService rackService;
        public RackController(IRackService objRackService)
        {
            this.rackService = objRackService;
        }

        [HttpGet]
        public ActionResult RackList()
        {
            RackViewModel model = new RackViewModel();
            rackService.FillBranches(model);
            return View(model);
        }

        [HttpGet]
        public JsonResult GetRack(string searchText, short? branchkey)
        {
            int page = 1, rows = 10;

            List<RackViewModel> racksList = new List<RackViewModel>();
            racksList = rackService.GetRack(searchText, branchkey ?? 0);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = racksList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = racksList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddEditRack(short? id)
        {
            var racks = rackService.GetRackById(id);
            if (racks == null)
            {
                racks = new RackViewModel();
            }
            return PartialView(racks);
        }

        [HttpPost]
        public ActionResult AddEditRack(RackViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.MasterRowKey == 0)
                {
                    model = rackService.CreateRack(model);
                }
                else
                {
                    model = rackService.UpdateRack(model);
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

        [HttpPost]
        public ActionResult DeleteRack(Int16 id)
        {
            RackViewModel objViewModel = new RackViewModel();

            objViewModel.MasterRowKey = id;
            try
            {
                objViewModel = rackService.DeleteRack(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }

        public ActionResult DeleteSubRack(long Id)
        {
            RackViewModel objViewModel = new RackViewModel();

            try
            {
                objViewModel = rackService.DeleteSubRack(Id);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }


        [HttpGet]
        public JsonResult CheckRackCodeExists(string RackCode, int? MasterRowKey)
        {
            RackViewModel model = new RackViewModel();
            model = rackService.CheckRackCodeExists(RackCode, MasterRowKey ?? 0);
            return Json(model.IsSuccessful, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CheckSubRackCodeExists(string SubRackCode, long? Rowkey)
        {
            RackViewModel model = new RackViewModel();
            model = rackService.CheckSubRackCodeExists(SubRackCode, Rowkey ?? 0);
            return Json(model.IsSuccessful, JsonRequestBehavior.AllowGet);
        }
    }
}