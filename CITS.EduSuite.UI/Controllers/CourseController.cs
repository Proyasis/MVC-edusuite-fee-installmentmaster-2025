
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
    public class CourseController : BaseController
    {
        private ICourseService CourseService;
        private ISharedService sharedService;
        private ISelectListService selectListService;
        public CourseController(ICourseService objCourseService,
            ISharedService objSharedService, ISelectListService objselectListService)
        {
            this.CourseService = objCourseService;
            this.sharedService = objSharedService;
            this.selectListService = objselectListService;
        }
        [HttpGet]
        public ActionResult CourseList()
        {
            ViewBag.IsMultipleUniversity = sharedService.CheckMenuActive(MenuConstants.UniversityCourse);
            CourseViewModel model = new CourseViewModel();
            short? SearchAcademicTermKey = null;
            model.CourseTypeList = selectListService.FillCourseTypesById(SearchAcademicTermKey);

            return View(model);
        }
        [HttpGet]
        public ActionResult AddEditCourse(int? id)
        {
            CourseViewModel model = new CourseViewModel();
            model = CourseService.GetCourseById(id);
            if (model == null)
            {
                model = new CourseViewModel();
            }
            return PartialView(model);

        }
        [HttpPost]
        public ActionResult AddEditCourse(CourseViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = CourseService.CreateCourse(model);
                }
                else
                {
                    model = CourseService.UpdateCourse(model);
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
        public JsonResult GetCourse(string SearchText, short? SearchCourseTypeKey, string sidx, string sord, int page, int rows)
        {
            long TotalRecords = 0;

            List<CourseViewModel> CourseList = new List<CourseViewModel>();
            CourseViewModel objViewModel = new CourseViewModel();

            objViewModel.SearchText = SearchText ?? "";
            objViewModel.SearchCourseTypeKey = SearchCourseTypeKey ?? 0;
            objViewModel.PageIndex = page;
            objViewModel.PageSize = rows;
            objViewModel.SortBy = sidx;
            objViewModel.SortOrder = sord;
            CourseList = CourseService.GetCourse(objViewModel, out TotalRecords);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            var totalPages = (int)Math.Ceiling((float)TotalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = TotalRecords,
                rows = CourseList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteCourse(int? id, long? UniversityCourseKey)
        {
            CourseViewModel objViewModel = new CourseViewModel();

            objViewModel.RowKey = id ?? 0;
            objViewModel.UniversityCourseKey = UniversityCourseKey;
            try
            {
                objViewModel = CourseService.DeleteCourse(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }
        [HttpGet]
        public JsonResult CheckCourseCodeExists(string CourseCode, long RowKey)
        {
            CourseViewModel model = new CourseViewModel();
            model.RowKey = RowKey;
            model.CourseCode = CourseCode;
            model = CourseService.CheckCourseCodeExists(model);
            return Json(model.IsSuccessful, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult CheckCourseNameExists(string CourseName, long RowKey, short CourseTypeKey)
        {
            CourseViewModel model = new CourseViewModel();
            model.RowKey = RowKey;
            model.CourseName = CourseName;
            model.CourseTypeKey = CourseTypeKey;
            model = CourseService.CheckCourseNameExists(model);
            return Json(model.IsSuccessful, JsonRequestBehavior.AllowGet);
        }
    }
}