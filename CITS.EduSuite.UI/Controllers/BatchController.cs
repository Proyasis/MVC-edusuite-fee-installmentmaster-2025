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
    public class BatchController : BaseController
    {
        private IBatchService BatchService;
        public BatchController(IBatchService objBatchService)
        {
            this.BatchService = objBatchService;
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Batch, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult BatchList()
        {
            return View();
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Batch, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditBatch(short? id)
        {
            BatchViewModel model = new BatchViewModel();
            model = BatchService.GetBatchById(id);
            if (model == null)
            {
                model = new BatchViewModel();
            }
            return PartialView(model);

        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Batch, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult AddEditBatch(BatchViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.Rowkey == 0)
                {
                    model = BatchService.CreateBatch(model);
                }
                else
                {
                    model = BatchService.UpdateBatch(model);
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
        public JsonResult GetBatch(string searchText)
        {
            int page = 1, rows = 15;
            List<BatchViewModel> BatchList = new List<BatchViewModel>();
            BatchList = BatchService.GetBatch(searchText);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = BatchList.Count();
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = BatchList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Batch, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteBatch(short id)
        {
            BatchViewModel objViewModel = new BatchViewModel();

            objViewModel.Rowkey = id;
            try
            {
                objViewModel = BatchService.DeleteBatch(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }

        [HttpGet]
        public JsonResult CheckBatchCodeExists(string BatchCode, short? RowKey)
        {
            BatchViewModel model = new BatchViewModel();
            model = BatchService.CheckBatchCodeExist(BatchCode, RowKey ?? 0);
            return Json(model.IsSuccessful, JsonRequestBehavior.AllowGet);
        }

    }
}