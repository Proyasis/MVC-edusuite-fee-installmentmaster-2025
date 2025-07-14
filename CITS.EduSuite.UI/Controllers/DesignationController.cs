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
    public class DesignationController : BaseController
    {
        private IDesignationService designationService;

        public DesignationController(IDesignationService objDesignationService)
        {
            this.designationService = objDesignationService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Designation, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult DesignationList()
        {
            DesignationViewModel Designation = new DesignationViewModel();
            return View(Designation);
        }

        [HttpGet]
        public JsonResult GetDesignation(string searchText)
        {
            int page = 1, rows = 10;

            List<DesignationViewModel> DesignationList = new List<DesignationViewModel>();
            DesignationList = designationService.GetDesignations(searchText);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = DesignationList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = DesignationList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Designation, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditDesignation(short? id)
        {
            var Designation = designationService.GetDesignationById(id);
            if (Designation == null)
            {
                Designation = new DesignationViewModel();
            }
            return PartialView(Designation);
        }

        [HttpPost]
        public ActionResult AddEditDesignation(DesignationViewModel Designation)
        {

            if (ModelState.IsValid)
            {
                if (Designation.RowKey == 0)
                {
                    Designation = designationService.CreateDesignation(Designation);
                }
                else
                {
                    Designation = designationService.UpdateDesignation(Designation);
                }

                if (Designation.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", Designation.Message);
                }
                else
                {
                    return Json(Designation);
                }

                Designation.Message = "";
                return PartialView(Designation);
            }

            Designation.Message = EduSuiteUIResources.Failed;
            return PartialView(Designation);

        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Agent, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteDesignation(Int16 id)
        {
            DesignationViewModel objViewModel = new DesignationViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = designationService.DeleteDesignation(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }


        [HttpGet]
        [EncryptedActionParameter]
        public ActionResult AddEditDesignationPermission(short? id)
        {
            DesignationViewModel ObjViewModel = new DesignationViewModel();
            ObjViewModel.RowKey = id ?? 0;
            return View(ObjViewModel);
        }

        [HttpPost]
        public ActionResult AddEditDesignationPermission(DesignationViewModel model)
        {

            model = designationService.UpdateDesignationPermission(model);
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

        [HttpGet]
        public JsonResult GetDesignationPermissionById(short id)
        {
            DesignationViewModel designationModel = new DesignationViewModel();
            designationModel = designationService.GetDesignationPermissionsById(id);
            var tree = designationModel.DesignationPermissions.Where(row => row.ActionKey == null).OrderBy(row => row.MenuName).Select(row => new
                {
                    id = row.MenuKey,
                    text = row.MenuName,
                    state = new { selected = (row.IsActive ?? false) ? true : false },
                    children = designationModel.DesignationPermissions.OrderBy(x => x.ActionKey).Where(x => x.MenuKey == row.MenuKey && x.ActionKey != null).Select(Y => new
                    {
                        id = "M" + Y.MenuKey + "A" + Y.ActionKey,
                        text = Y.ActionName,
                        state = new { selected = (Y.IsActive ?? false) ? true : false },
                        data = new { key = Y.RowKey, mid = Y.MenuKey, aid = Y.ActionKey }
                    })
                }).ToList();

            return Json(tree, JsonRequestBehavior.AllowGet);

        }




        [HttpGet]
        public ActionResult DesignationChart()
        {
            return View();
        }


        [HttpGet]
        public JsonResult GetDesignationChart()
        {
            List<DesignationViewModel> designationList = new List<DesignationViewModel>();
            designationList = designationService.GetDesignationChart();

            return Json(designationList, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AddEditDesignationChart(List<DesignationViewModel> modelList)
        {
            DesignationViewModel model = new DesignationViewModel();
            model = designationService.UpdateDesignationChart(modelList);
            return Json(model);
        }

    }
}