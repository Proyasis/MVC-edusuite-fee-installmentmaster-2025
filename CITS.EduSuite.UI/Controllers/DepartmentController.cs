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
    public class DepartmentController : BaseController
    {
        // GET: ExamCenter
        private IDepartmentMasterService DepartmentMasterService;
        public DepartmentController(IDepartmentMasterService objDepartmentMasterService)
          {
             this.DepartmentMasterService = objDepartmentMasterService;
          }
        [HttpGet]
        public ActionResult DepartmentList()
        {
            return View();
        }
        [HttpGet]
        public ActionResult AddEditDepartment(short? id)
        {
            DepartmentViewModel model = new DepartmentViewModel();
              model = DepartmentMasterService.GetDepartmentById(id);
            if (model == null)
            {
                model = new DepartmentViewModel();
            }
            return PartialView(model);
        }


        [HttpPost]
        public ActionResult AddEditDepartment(DepartmentViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                     model = DepartmentMasterService.CreateDepartment(model);
                }
                else
                {
                      model = DepartmentMasterService.UpdateDepartment(model);
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
        public JsonResult GetDepartment(string searchText)
        {
            int page = 1, rows = 15;
            List<DepartmentViewModel> DepartmentList = new List<DepartmentViewModel>();
              DepartmentList = DepartmentMasterService.GetDepartment(searchText);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = DepartmentList.Count();
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = DepartmentList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteDepartment(short id)
        {
            DepartmentViewModel objViewModel = new DepartmentViewModel();

            objViewModel.RowKey = id;
            try
            {
                  objViewModel = DepartmentMasterService.DeleteDepartment(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }
        [HttpGet]
        public JsonResult CheckDepartmentCodeExists(string DepartmentCode, short RowKey)
        {
            DepartmentViewModel model = new DepartmentViewModel();
            model.RowKey = RowKey;
            model.DepartmentCode = DepartmentCode;
            model = DepartmentMasterService.CheckDepartmentCodeExists(model);
            return Json(model.IsSuccessful, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult CheckDepartmentNameExists(string DepartmentName, short RowKey)
        {
            DepartmentViewModel model = new DepartmentViewModel();
            model.RowKey = RowKey;
            model.DepartmentName = DepartmentName;
            model = DepartmentMasterService.CheckDepartmentNameExists(model);
            return Json(model.IsSuccessful, JsonRequestBehavior.AllowGet);
        }
    }
}