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
    public class CourseTypeController : BaseController
    {
        private ICourseTypeService CourseTypeService;
        public CourseTypeController(ICourseTypeService objCourseTypeService)
        {
            this.CourseTypeService = objCourseTypeService;
        }
        [HttpGet]
        public ActionResult CourseTypeList()
        {
            return View();
        }
        [HttpGet]
        public ActionResult AddEditCourseType(int? id)
        {
            CourseTypeViewModel model = new CourseTypeViewModel();
            model = CourseTypeService.GetCourseTypeById(id);
            if (model == null)
            {
                model = new CourseTypeViewModel();
            }
            return PartialView(model);

        }
        [HttpPost]
        public ActionResult AddEditCourseType(CourseTypeViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = CourseTypeService.CreateCourseType(model);
                }
                else
                {
                    model = CourseTypeService.UpdateCourseType(model);
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
        public JsonResult GetCourseType(string searchText)
        {
            int page = 1, rows = 15;
            List<CourseTypeViewModel> CourseTypeList = new List<CourseTypeViewModel>();
            CourseTypeList = CourseTypeService.GetCourseType(searchText);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = CourseTypeList.Count();
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = CourseTypeList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteCourseType(short id)
        {
            CourseTypeViewModel objViewModel = new CourseTypeViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = CourseTypeService.DeleteCourseType(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }
    }
}