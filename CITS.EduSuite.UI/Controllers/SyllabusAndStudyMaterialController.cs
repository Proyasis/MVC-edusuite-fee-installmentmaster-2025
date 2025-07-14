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
    public class SyllabusAndStudyMaterialController : BaseController
    {
        public ISyllabusAndStudyMaterialService SyllabusAndStudyMaterialService;
        private ISharedService sharedService;
        public SyllabusAndStudyMaterialController(ISyllabusAndStudyMaterialService objSyllabusAndStudyMaterialService, ISharedService objSharedService)
        {
            this.SyllabusAndStudyMaterialService = objSyllabusAndStudyMaterialService;
            this.sharedService = objSharedService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.SyllabusAndStudyMaterial, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult SyllabusAndStudyMaterialList()
        {
            SyllabusAndStudyMaterialViewModel model = new SyllabusAndStudyMaterialViewModel();
            SyllabusAndStudyMaterialService.FillDropDown(model);
            ViewBag.ShowAcademicTerm = sharedService.ShowAcademicTerm();
            ViewBag.ShowUniversity = sharedService.ShowUniversity();
            return View(model);
        }

        [HttpGet]
        public JsonResult GetSyllabusAndStudyMaterial(string SearchText, short? AcademicTermKey, long? CourseKey, short? UniversityMasterKey, short? SubjectYear, string sidx, string sord, int page, int rows)
        {
            long TotalRecords = 0;

            List<SyllabusAndStudyMaterialViewModel> SubjectList = new List<SyllabusAndStudyMaterialViewModel>();
            SyllabusAndStudyMaterialViewModel model = new SyllabusAndStudyMaterialViewModel();
            string[] SortOrderList = sidx.Split(',');
            model.SortBy = SortOrderList.Length == 3 ? SortOrderList[2] : sidx;
            model.SortBy = model.SortBy.Trim();
            model.SortOrder = sord;
            model.PageIndex = page;
            model.PageSize = rows;
            model.AcademicTermKey = AcademicTermKey ?? 0;
            model.CourseKey = CourseKey ?? 0;
            model.UniversityMasterKey = UniversityMasterKey ?? 0;
            model.SubjectYear = SubjectYear ?? 0;
            model.SubjectName = SearchText;


            SubjectList = SyllabusAndStudyMaterialService.GetSubject(model, out TotalRecords);

            foreach (SyllabusAndStudyMaterialViewModel objmodel in SubjectList)
            {
                objmodel.SubjectYearText = CommonUtilities.GetYearDescriptionByCodeDetails(objmodel.CourseDuration ?? 0, objmodel.SubjectYear ?? 0, objmodel.AcademicTermKey ?? 0);
                objmodel.UniversityCourse = objmodel.Course + " - " + objmodel.University;

            }

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            var totalPages = (int)Math.Ceiling((float)TotalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = TotalRecords,
                rows = SubjectList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.SyllabusAndStudyMaterial, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditStudyMaterial(long? Id)
        {

            SyllabusAndStudyMaterialViewModel model = new SyllabusAndStudyMaterialViewModel();

            model = SyllabusAndStudyMaterialService.GetStudyMaterialById(Id ?? 0);
            return PartialView(model);
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.SyllabusAndStudyMaterial, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult AddEditStudyMaterial(SyllabusAndStudyMaterialViewModel model)
        {
            if (ModelState.IsValid)
            {


                model = SyllabusAndStudyMaterialService.UpdateStudyMaterials(model);


                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    return Json(model);

                    //return View("SyllabusAndStudyMaterialList");
                    //return RedirectToAction("SyllabusAndStudyMaterialList");

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

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.SyllabusAndStudyMaterial, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteStudyMaterial(long? id)
        {
            SyllabusAndStudyMaterialViewModel Model = new SyllabusAndStudyMaterialViewModel();

            try
            {
                Model = SyllabusAndStudyMaterialService.DeleteStudyMaterial(id ?? 0);
            }
            catch (Exception Ex)
            {
                Model.Message = EduSuiteUIResources.Failed;
            }
            return Json(Model);
        }

        //public JsonResult CheckStudyMaterialNameExist(string StudyMaterialName, long? RowKey)
        //{
        //    StudyMaterialModel model = new StudyMaterialModel();
        //    model.StudyMaterialName = StudyMaterialName;
        //    model.RowKey = RowKey ?? 0;
        //    var Result = SyllabusAndStudyMaterialService.CheckStudyMaterialNameExist(model);
        //    return Json(Result.IsSuccessful, JsonRequestBehavior.AllowGet);
        //}

        //[HttpGet]
        //public JsonResult CheckStudyMaterialCodeExist(string StudyMaterialCode, long? RowKey)
        //{
        //    StudyMaterialModel model = new StudyMaterialModel();
        //    model.StudyMaterialCode = StudyMaterialCode;
        //    model.RowKey = RowKey ?? 0;
        //    var Result = SyllabusAndStudyMaterialService.CheckStudyMaterialCodeExist(model);
        //    return Json(Result.IsSuccessful, JsonRequestBehavior.AllowGet);
        //}

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.SyllabusAndStudyMaterial, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditSubjectModules(long? Id)
        {

            SyllabusAndStudyMaterialViewModel model = new SyllabusAndStudyMaterialViewModel();

            model = SyllabusAndStudyMaterialService.GetSubjectModulesById(Id ?? 0);
            return View(model);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.SyllabusAndStudyMaterial, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult AddEditSubjectModules(SyllabusAndStudyMaterialViewModel model)
        {
            if (ModelState.IsValid)
            {

                model = SyllabusAndStudyMaterialService.UpdateSubjectModules(model);


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

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.SyllabusAndStudyMaterial, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteSubjectModules(long? id)
        {
            SyllabusAndStudyMaterialViewModel Model = new SyllabusAndStudyMaterialViewModel();

            try
            {
                Model = SyllabusAndStudyMaterialService.DeleteSubjectModule(id ?? 0);
            }
            catch (Exception Ex)
            {
                Model.Message = EduSuiteUIResources.Failed;
            }
            return Json(Model);
        }


        [HttpPost]
        public ActionResult GetModuleTopics(List<ModulesTopicModel> modelList, int? index)
        {
            SubjectModulesModel model = new SubjectModulesModel();
            if (modelList == null)
            {
                modelList = new List<ModulesTopicModel>();
            }
            ViewBag.Index = index ?? 0;
            model.ModulesTopicModel = modelList;
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult AddEditModuleTopics(SubjectModulesModel model, int index)
        {
            List<ModulesTopicModel> modelList = new List<ModulesTopicModel>();
            modelList = model.ModulesTopicModel;
            if (modelList == null)
            {
                modelList = new List<ModulesTopicModel>();

            }
            //foreach (CourseSubjectDetailViewModel modelItem in model)
            //{
            //    CourseSubjectDetailViewModel.FillDealers(modelItem);
            //}
            ViewBag.Index = index;
            model.ModulesTopicModel = modelList;
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult DeleteModuleTopic(long? id)
        {
            SyllabusAndStudyMaterialViewModel Model = new SyllabusAndStudyMaterialViewModel();

            try
            {
                Model = SyllabusAndStudyMaterialService.DeleteModuleTopic(id ?? 0);
            }
            catch (Exception Ex)
            {
                Model.Message = EduSuiteUIResources.Failed;
            }
            return Json(Model);
        }

        [HttpGet]
        public JsonResult CheckModuleNameExist(string ModuleName, long RowKey)
        {
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CheckTopicNameExist(string TopicName, long RowKey)
        {
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteModuleTopicAll(long? id)
        {
            SyllabusAndStudyMaterialViewModel Model = new SyllabusAndStudyMaterialViewModel();

            try
            {
                Model = SyllabusAndStudyMaterialService.DeleteModuleTopicAll(id ?? 0);
            }
            catch (Exception Ex)
            {
                Model.Message = EduSuiteUIResources.Failed;
            }
            return Json(Model);
        }



        public JsonResult FillUniversity(long key, short AcademicTermKey)
        {
            SyllabusAndStudyMaterialViewModel model = new SyllabusAndStudyMaterialViewModel();
            model.AcademicTermKey = AcademicTermKey;
            model.CourseKey = key;
            model = SyllabusAndStudyMaterialService.FillUniversity(model);
            return Json(model.Universities, JsonRequestBehavior.AllowGet);
        }


        public ActionResult FillCourseYear(long key, short AcademicTermKey)
        {
            SyllabusAndStudyMaterialViewModel model = new SyllabusAndStudyMaterialViewModel();
            model.AcademicTermKey = AcademicTermKey;
            model.CourseKey = key;
            model = SyllabusAndStudyMaterialService.FillCourseYear(model);
            return Json(model.CourseYear, JsonRequestBehavior.AllowGet);
        }
    }
}