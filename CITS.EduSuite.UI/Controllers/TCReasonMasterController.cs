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
    public class TCReasonMasterController : BaseController
    {
        private ITCReasonMasterService TCReasonMasterService;
        public TCReasonMasterController(ITCReasonMasterService objTCReasonMasterService)
        {
            this.TCReasonMasterService = objTCReasonMasterService;
        }
        [HttpGet]
        public ActionResult TCReasonMasterList()
        {
            return View();
        }
        [HttpGet]
        public ActionResult AddEditTCReasonMaster(int? id)
        {
            TCReasonMasterViewModel model = new TCReasonMasterViewModel();
            model = TCReasonMasterService.GetTCReasonMasterById(id);
            if (model == null)
            {
                model = new TCReasonMasterViewModel();
            }
            return PartialView(model);

        }
        [HttpPost]
        public ActionResult AddEditTCReasonMaster(TCReasonMasterViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = TCReasonMasterService.CreateTCReasonMaster(model);
                }
                else
                {
                    model = TCReasonMasterService.UpdateTCReasonMaster(model);
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
        public JsonResult GetTCReasonMaster(string searchText)
        {
            int page = 1, rows = 15;
            List<TCReasonMasterViewModel> TCReasonMasterList = new List<TCReasonMasterViewModel>();
            TCReasonMasterList = TCReasonMasterService.GetTCReasonMaster(searchText);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = TCReasonMasterList.Count();
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = TCReasonMasterList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteTCReasonMaster(short id)
        {
            TCReasonMasterViewModel objViewModel = new TCReasonMasterViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = TCReasonMasterService.DeleteTCReasonMaster(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }
    }
}