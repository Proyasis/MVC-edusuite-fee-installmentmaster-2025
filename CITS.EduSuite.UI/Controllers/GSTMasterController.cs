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
    public class GSTMasterController : BaseController
    {
        private IGSTMasterService GSTMasterService;
        public GSTMasterController(IGSTMasterService objGSTMasterService)
        {
            this.GSTMasterService = objGSTMasterService;
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.GSTMaster, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult GSTMasterList()
        {
            return View();
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.GSTMaster, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditGSTMaster(short? id)
        {
            GSTMasterViewModel model = new GSTMasterViewModel();
            model = GSTMasterService.GetGSTMasterById(id);
            if (model == null)
            {
                model = new GSTMasterViewModel();
            }
            return PartialView(model);

        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.GSTMaster, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult AddEditGSTMaster(GSTMasterViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = GSTMasterService.CreateGSTMaster(model);
                }
                else
                {
                    model = GSTMasterService.UpdateGSTMaster(model);
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
        public JsonResult GetGSTMaster(string searchText)
        {
            int page = 1, rows = 15;
            List<GSTMasterViewModel> GSTMasterList = new List<GSTMasterViewModel>();
            GSTMasterList = GSTMasterService.GetGSTMaster(searchText);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = GSTMasterList.Count();
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = GSTMasterList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.GSTMaster, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteGSTMaster(short id)
        {
            GSTMasterViewModel objViewModel = new GSTMasterViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = GSTMasterService.DeleteGSTMaster(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }
    }
}