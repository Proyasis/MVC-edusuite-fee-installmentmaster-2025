using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI.Controllers
{
    public class CourseSubjectController : BaseController
    {
        public ICourseSubjectService CourseSubjectService;
        private ISharedService sharedService;
        public CourseSubjectController(ICourseSubjectService objCourseSubjectService,
            ISharedService objSharedService)
        {
            this.CourseSubjectService = objCourseSubjectService;
            this.sharedService = objSharedService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.CourseSubject, ActionCode = ActionConstants.View)]
        public ActionResult CourseSubjectList()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetCourseSubject(string SearchText)
        {
            int page = 1; int rows = 15;
            List<CourseSubjectMasterViewModel> CourseSubjectList = new List<CourseSubjectMasterViewModel>();
            CourseSubjectList = CourseSubjectService.GetCourseSubject(SearchText);
            int pageindex = Convert.ToInt32(page) - 1;
            int pagesize = rows;
            int totalrecords = CourseSubjectList.Count();
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)rows);

            var jsonData = new
            {
                total = totalpage,
                page,
                records = totalrecords,
                rows = CourseSubjectList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.CourseSubject, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditCourseSubject(long? Id)
        {

            CourseSubjectMasterViewModel model = new CourseSubjectMasterViewModel();
            model.RowKey = Id ?? 0;
            model = CourseSubjectService.GetCourseSubjectById(model);
            ViewBag.ShowAcademicTerm = sharedService.ShowAcademicTerm();
            ViewBag.ShowUniversity = sharedService.ShowUniversity();
            return View(model);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.CourseSubject, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult AddEditCourseSubject(CourseSubjectMasterViewModel model)
        {
            if (ModelState.IsValid)
            {

                if (model.RowKey != 0)
                {
                    model = CourseSubjectService.UpdateCourseSubject(model);
                }
                else
                {
                    model = CourseSubjectService.CreateCourseSubject(model);
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
                return Json(model);
            }
            foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            {
            }

            model.Message = EduSuiteUIResources.Failed;
            return Json(model);
        }

        [HttpPost]
        public ActionResult GetCourseSubjectDetails(CourseSubjectMasterViewModel model)
        {
            //CourseSubjectMasterViewModel model = new CourseSubjectMasterViewModel();
            //model.RowKey = Id ?? 0;
            CourseSubjectService.FillCourseSubjectDetailsViewModel(model);
            return PartialView(model);
        }

        public JsonResult FillCourse(short key)
        {
            CourseSubjectMasterViewModel model = new CourseSubjectMasterViewModel();
            model.CourseTypeKey = key;
            model = CourseSubjectService.FillCourse(model);
            return Json(model.Courses, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FillUniversity(long key, short AcademicTermKey)
        {
            CourseSubjectMasterViewModel model = new CourseSubjectMasterViewModel();
            model.AcademicTermKey = AcademicTermKey;
            model.CourseKey = key;
            model = CourseSubjectService.FillUniversity(model);
            return Json(model.Universities, JsonRequestBehavior.AllowGet);
        }


        public ActionResult FillCourseYear(long key, short AcademicTermKey)
        {
            CourseSubjectMasterViewModel model = new CourseSubjectMasterViewModel();
            model.AcademicTermKey = AcademicTermKey;
            model.CourseKey = key;
            model = CourseSubjectService.FillCourseYear(model);
            return PartialView(model);
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.CourseSubject, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteCourseSubject(long id)
        {
            CourseSubjectMasterViewModel Model = new CourseSubjectMasterViewModel();
            try
            {
                Model = CourseSubjectService.DeleteCourseSubject(id);
            }
            catch (Exception Ex)
            {
                Model.Message = EduSuiteUIResources.Failed;
            }
            return Json(Model);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.CourseSubject, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteCourseSubjectAll(long? id)
        {
            CourseSubjectMasterViewModel Model = new CourseSubjectMasterViewModel();
            Model.RowKey = id ?? 0;
            try
            {
                Model = CourseSubjectService.DeleteCourseSubjectAll(Model);
            }
            catch (Exception Ex)
            {
                Model.Message = EduSuiteUIResources.Failed;
            }
            return Json(Model);
        }

        [HttpPost]
        public ActionResult GetStudyMaterials(List<StudyMaterialModel> modelList, int? index)
        {
            CourseSubjectDetailViewModel model = new CourseSubjectDetailViewModel();
            if (modelList == null)
            {
                modelList = new List<StudyMaterialModel>();
            }
            ViewBag.Index = index ?? 0;
            model.StudyMaterials = modelList;
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult AddEditStudyMaterialsDetails(CourseSubjectDetailViewModel model, int index)
        {
            List<StudyMaterialModel> modelList = new List<StudyMaterialModel>();
            modelList = model.StudyMaterials;
            if (modelList == null)
            {
                modelList = new List<StudyMaterialModel>();

            }
            //foreach (CourseSubjectDetailViewModel modelItem in model)
            //{
            //    CourseSubjectDetailViewModel.FillDealers(modelItem);
            //}
            ViewBag.Index = index;
            model.StudyMaterials = modelList;
            return PartialView(model);
        }


        [HttpPost]
        public ActionResult DeleteStudyMaterialAll(long? id)
        {
            CourseSubjectMasterViewModel Model = new CourseSubjectMasterViewModel();

            try
            {
                Model = CourseSubjectService.DeleteStudyMaterialAll(id ?? 0);
            }
            catch (Exception Ex)
            {
                Model.Message = EduSuiteUIResources.Failed;
            }
            return Json(Model);
        }
        [HttpPost]
        public ActionResult DeleteStudyMaterial(long? id)
        {
            CourseSubjectMasterViewModel Model = new CourseSubjectMasterViewModel();

            try
            {
                Model = CourseSubjectService.DeleteStudyMaterial(id ?? 0);
            }
            catch (Exception Ex)
            {
                Model.Message = EduSuiteUIResources.Failed;
            }
            return Json(Model);
        }

        [HttpGet]
        public JsonResult CheckSubjectCodeExist(string SubjectCode, short? CourseYearKey)
        {
            //var Result = CourseSubjectService.CheckSubjectCodeExist(SubjectCode, CourseYearKey ?? 0);
            //return Json(Result, JsonRequestBehavior.AllowGet);
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult CheckSubjectNameExist(string SubjectName, short? CourseYearKey)
        {
            //var Result = CourseSubjectService.CheckSubjectNameExist(SubjectName, CourseYearKey ?? 0);
            //return Json(Result, JsonRequestBehavior.AllowGet);
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult CheckStudyMaterialNameExist(string StudyMaterialName, long? SubjectKey)
        {
            //var Result = CourseSubjectService.CheckStudyMaterialNameExist(StudyMaterialName, SubjectKey ?? 0);
            //return Json(Result, JsonRequestBehavior.AllowGet);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CheckStudyMaterialCodeExist(string StudyMaterialCode, long? SubjectKey)
        {
            //var Result = CourseSubjectService.CheckStudyMaterialCodeExist(StudyMaterialCode, SubjectKey ?? 0);
            //return Json(Result, JsonRequestBehavior.AllowGet);
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}