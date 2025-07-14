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
    public class UniversityMasterController : BaseController
    {
        private IUniversityMasterService UniversityMasterService;
        public UniversityMasterController(IUniversityMasterService objUniversityMasterService)
        {
            this.UniversityMasterService = objUniversityMasterService;
        }
        [HttpGet]
        public ActionResult UniversityMasterList()
        {
            return View();
        }
        [HttpGet]
        public ActionResult AddEditUniversityMaster(int? id)
        {
            UniversityMasterViewModel model = new UniversityMasterViewModel();
            model = UniversityMasterService.GetUniversityMasterById(id);
            if (model == null)
            {
                model = new UniversityMasterViewModel();
            }
            return PartialView(model);

        }
        [HttpPost]
        public ActionResult AddEditUniversityMaster(UniversityMasterViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = UniversityMasterService.CreateUniversityMaster(model);
                }
                else
                {
                    model = UniversityMasterService.UpdateUniversityMaster(model);
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
        public JsonResult GetUniversityMaster(string searchText)
        {
            int page = 1, rows = 15;
            List<UniversityMasterViewModel> UniversityMasterList = new List<UniversityMasterViewModel>();
            UniversityMasterList = UniversityMasterService.GetUniversityMaster(searchText);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = UniversityMasterList.Count();
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = UniversityMasterList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteUniversityMaster(short id)
        {
            UniversityMasterViewModel objViewModel = new UniversityMasterViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = UniversityMasterService.DeleteUniversityMaster(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }
        [HttpGet]
        public JsonResult CheckUniversityMasterCodeExists(string UniversityMasterCode,short RowKey)
        {
            UniversityMasterViewModel model = new UniversityMasterViewModel();
            model.RowKey = RowKey;
            model.UniversityMasterCode = UniversityMasterCode;
            model = UniversityMasterService.CheckUniversityMasterCodeExists(model);
            return Json(model.IsSuccessful, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult CheckUniversityMasterNameExists(string UniversityMasterName, short RowKey)
        {
            UniversityMasterViewModel model = new UniversityMasterViewModel();
            model.RowKey = RowKey;
            model.UniversityMasterName = UniversityMasterName;
            model = UniversityMasterService.CheckUniversityMasterNameExists(model);
            return Json(model.IsSuccessful, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FillAccountHeadType(byte key)
        {
            UniversityMasterViewModel model = new UniversityMasterViewModel();
            model.AccountGroupKey = key;
            model = UniversityMasterService.FillAccountHeadType(model);
            return Json(model.AccountHeadType, JsonRequestBehavior.AllowGet);
        }
    }
}