
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
    public class ClassModeController : BaseController
    {
       private IClassModeService ClassModeservice;
       public ClassModeController(IClassModeService objClassModeService)
        {
            this.ClassModeservice = objClassModeService;
        }
        [HttpGet]
        public ActionResult ClassModeList()
        {
            return View();
        }
        [HttpGet]
        public ActionResult AddEditClassMode(int? id)
        {
            ClassModeViewModel model = new ClassModeViewModel();
            model = ClassModeservice.GetClassModeById(id);
            if (model == null)
            {
                model = new ClassModeViewModel();
            }
            return PartialView(model);

        }
        [HttpPost]
        public ActionResult AddEditClassMode(ClassModeViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = ClassModeservice.CreateClassMode(model);
                }
                else
                {
                    model = ClassModeservice.UpdateClassMode(model);
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
        public JsonResult GetClassMode(string searchText)
        {
            int page = 1, rows = 15;
            List<ClassModeViewModel> ClassModeList = new List<ClassModeViewModel>();
            ClassModeList = ClassModeservice.GetClassMode(searchText);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = ClassModeList.Count();
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = ClassModeList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteClassMode(short id)
        {
            ClassModeViewModel objViewModel = new ClassModeViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = ClassModeservice.DeleteClassMode(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }
    }
}