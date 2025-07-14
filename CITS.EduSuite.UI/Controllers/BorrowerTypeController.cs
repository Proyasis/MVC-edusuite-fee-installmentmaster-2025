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
    public class BorrowerTypeController : BaseController
    {
        private IBorrowerTypeService borrowerTypeService;

        public BorrowerTypeController(IBorrowerTypeService objBorrowerTypeService)
        {
            borrowerTypeService = objBorrowerTypeService;
        }
        [HttpGet]
        public ActionResult BorrowerTypeList()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetBorrowerTypes(string searchText)
        {
            int page = 1, rows = 10;

            List<BorrowerTypeViewModel> borrowerTypesList = new List<BorrowerTypeViewModel>();
            borrowerTypesList = borrowerTypeService.GetBorrowerTypes(searchText);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = borrowerTypesList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = borrowerTypesList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddEditBorrowerType(byte? id)
        {
            var borrowerTypes = borrowerTypeService.GetBorrowerTypeById(id);
            if (borrowerTypes == null)
            {
                borrowerTypes = new BorrowerTypeViewModel();
            }
            return PartialView(borrowerTypes);
        }

        [HttpPost]
        public ActionResult AddEditBorrowerType(BorrowerTypeViewModel borrowerType)
        {

            if (ModelState.IsValid)
            {
                if (borrowerType.RowKey == 0)
                {
                    borrowerType = borrowerTypeService.CreateBorrowerType(borrowerType);
                }
                else
                {
                    borrowerType = borrowerTypeService.UpdateBorrowerType(borrowerType);
                }

                if (borrowerType.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", borrowerType.Message);
                }
                else
                {
                    return Json(borrowerType);
                }

                borrowerType.Message = "";
                return PartialView(borrowerType);
            }

            borrowerType.Message = EduSuiteUIResources.Failed;
            return PartialView(borrowerType);

        }

        [HttpPost]
        public ActionResult DeleteBorrowerType(byte id)
        {
            BorrowerTypeViewModel objViewModel = new BorrowerTypeViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = borrowerTypeService.DeleteBorrowerType(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }

        [HttpGet]
        public JsonResult CheckBorrowerTypeCodeExist(string BorrowerTypeCode, byte? RowKey)
        {
            BorrowerTypeViewModel model = new BorrowerTypeViewModel();
            model.BorrowerTypeCode = BorrowerTypeCode;
            model.RowKey = RowKey ?? 0;
            var Result = borrowerTypeService.CheckBorrowerTypeCodeExist(model);
            return Json(Result.IsSuccessful, JsonRequestBehavior.AllowGet);
        }
    }
}