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
    public class ScholarshipTypeController : BaseController
    {
        private IScholarshipTypeService ScholarshipTypeService;

         public ScholarshipTypeController(IScholarshipTypeService objScholarshipTypeService)
        {
            this.ScholarshipTypeService = objScholarshipTypeService;
        }
        [HttpGet]
        public ActionResult ScholarshipTypeList()
        {
            return View();
        }
        [HttpGet]
        public ActionResult AddEditScholarshipType(int? id)
        {
            ScholarshipTypeViewModel model = new ScholarshipTypeViewModel();
            model = ScholarshipTypeService.GetScholarshipTypeById(id);
            if (model == null)
            {
                model = new ScholarshipTypeViewModel();
            }
            return PartialView(model);

        }
        [HttpPost]
        public ActionResult AddEditScholarshipType(ScholarshipTypeViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = ScholarshipTypeService.CreateScholarshipType(model);
                }
                else
                {
                    model = ScholarshipTypeService.UpdateScholarshipType(model);
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
            foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            {
            }
            

            model.Message = EduSuiteUIResources.Failed;

            return PartialView(model);
        }
        public JsonResult GetScholarshipType(string searchText)
        {
            int page = 1, rows = 15;
            List<ScholarshipTypeViewModel> ScholarshipTypeList = new List<ScholarshipTypeViewModel>();
            ScholarshipTypeList = ScholarshipTypeService.GetScholarshipType(searchText);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = ScholarshipTypeList.Count();
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = ScholarshipTypeList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteScholarshipType(short id)
        {
            ScholarshipTypeViewModel objViewModel = new ScholarshipTypeViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = ScholarshipTypeService.DeleteScholarshipType(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }
        [HttpGet]
        public JsonResult CheckScholarShipTypeCodeExists(string ScholarshipTypeCode, short RowKey)
        {
            ScholarshipTypeViewModel model = new ScholarshipTypeViewModel();
            model.RowKey = RowKey;
            model.ScholarShipTypeCode = ScholarshipTypeCode;
            model = ScholarshipTypeService.CheckScholarshipTypeCodeExists(model);
            return Json(model.IsSuccessful, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult CheckScholarShipTypeNameExists(string ScholarshipTypeName, short RowKey)
        {
            ScholarshipTypeViewModel model = new ScholarshipTypeViewModel();
            model.RowKey = RowKey;
            model.ScholarShipTypeName = ScholarshipTypeName;
            model = ScholarshipTypeService.CheckScholarshipTypeNameExists(model);
            return Json(model.IsSuccessful, JsonRequestBehavior.AllowGet);
        }
    }
}