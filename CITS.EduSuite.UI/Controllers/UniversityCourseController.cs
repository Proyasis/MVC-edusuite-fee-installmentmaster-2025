using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.UI;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI.Controllers
{
    [MessagesActionFilter]
    public class UniversityCourseController : BaseController
    {
        public IUniversityCourseService UniversityCourseService;
        private ISharedService sharedService;
        private ISelectListService selectListService;
        public UniversityCourseController(IUniversityCourseService objUniversityCourseService,
            ISharedService objSharedService, ISelectListService objselectListService)
        {
            this.UniversityCourseService = objUniversityCourseService;
            this.sharedService = objSharedService;
            this.selectListService = objselectListService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UniversityCourse, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult UniversityCourseList()
        {
            UniversityCourseViewModel model = new UniversityCourseViewModel();
            model.AcademicTerms = selectListService.FillAcademicTerms();
            model.CourseTypes = selectListService.FillCourseTypesById(model.SearchAcademicTermKey);
            model.Courses = selectListService.FillCoursesById(model.SearchCourseTypeKey);
            model.Universities = selectListService.FillUniversitiesById(model.SearchCourseKey);
            return View(model);
        }

        [HttpGet]
        public JsonResult GetUniversityCourse(string SearchText, short? SearchCourseTypeKey, long? SearchCourseKey, short? SearchUniversityMasterKey, short? SearchAcademicTermKey, string sidx, string sord, int page, int rows)
        {
            long TotalRecords = 0;

            List<UniversityCourseViewModel> UniversityCourseList = new List<UniversityCourseViewModel>();
            UniversityCourseViewModel objViewModel = new UniversityCourseViewModel();

            objViewModel.SearchText = SearchText ?? "";
            objViewModel.SearchCourseKey = SearchCourseKey ?? 0;
            objViewModel.SearchCourseTypeKey = SearchCourseTypeKey ?? 0;
            objViewModel.SearchUniversityMasterKey = SearchUniversityMasterKey ?? 0;
            objViewModel.SearchAcademicTermKey = SearchAcademicTermKey ?? 0;
            objViewModel.PageIndex = page;
            objViewModel.PageSize = rows;
            objViewModel.SortBy = sidx;
            objViewModel.SortOrder = sord;
            UniversityCourseList = UniversityCourseService.GetUniversityCourse(objViewModel, out TotalRecords);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            var totalPages = (int)Math.Ceiling((float)TotalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = TotalRecords,
                rows = UniversityCourseList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UniversityCourse, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditUniversityCourse(int? id)
        {
            UniversityCourseViewModel model = new UniversityCourseViewModel();
            model = UniversityCourseService.GetUniversityCourseById(id);
            if (model == null)
            {
                model = new UniversityCourseViewModel();
            }
            ViewBag.ShowAcademicTerm = sharedService.ShowAcademicTerm();
            return PartialView(model);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UniversityCourse, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult AddEditUniversityCourse(UniversityCourseViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = UniversityCourseService.CreateUniversityCourse(model);
                }
                else
                {
                    model = UniversityCourseService.UpdateUniversityCourse(model);
                }
                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    return Json(model);
                }

                ViewBag.ShowAcademicTerm = sharedService.ShowAcademicTerm();
                return PartialView(model);
            }
            model.Message = EduSuiteUIResources.Failed;
            return PartialView(model);

        }

       
        [HttpPost]
        public ActionResult DeleteUniversityCourse(long id)
        {
            UniversityCourseViewModel objViewmodel = new UniversityCourseViewModel();
            objViewmodel.RowKey = id;
            try
            {
                objViewmodel = UniversityCourseService.DeleteUniversityCourse(objViewmodel);
            }
            catch (Exception)
            {
                objViewmodel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewmodel);
        }

        public JsonResult FillCourse(short key)
        {
            UniversityCourseViewModel model = new UniversityCourseViewModel();
            model.CourseTypeKey = key;
            model = UniversityCourseService.FillCourse(model);
            return Json(model.Courses, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FillAcademicTerm(short key)
        {
            UniversityCourseViewModel model = new UniversityCourseViewModel();
            model.CourseKey = key;
            model = UniversityCourseService.FillAcademicTerm(model);
            return Json(model.AcademicTerms, JsonRequestBehavior.AllowGet);
        }

        #region Class Details

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UniversityCourse, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditClassDetails(long? Id, short? StudentYear)
        {
            UniversityCourseViewModel objViewModel = new UniversityCourseViewModel();
            objViewModel.RowKey = Id ?? 0;
            objViewModel.StudentYear = StudentYear ?? 0;
            var ClassIdentities = UniversityCourseService.GetClassDetailsById(objViewModel);
            if (ClassIdentities == null)
            {
                ClassIdentities = new UniversityCourseViewModel();
            }
            return View(ClassIdentities);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UniversityCourse, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult AddEditClassDetails(UniversityCourseViewModel model)
        {
            if (ModelState.IsValid)
            {
                model = UniversityCourseService.UpdateClassModule(model);

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    return Json(model);
                }

                return Json(model);
            }

            model.Message = EduSuiteUIResources.Failed;
            return Json(model);
        }

        [HttpGet]
        public ActionResult GetClassDetails(short? StudentYear, long? RowKey)
        {
            UniversityCourseViewModel objViewModel = new UniversityCourseViewModel();
            objViewModel.RowKey = RowKey ?? 0;
            objViewModel.StudentYear = StudentYear ?? 0;
            var ClassIdentities = UniversityCourseService.GetClassDetailsById(objViewModel);
            if (ClassIdentities == null)
            {
                ClassIdentities = new UniversityCourseViewModel();
            }
            return View("AddEditClassDetails", ClassIdentities);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UniversityCourse, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteClassDetails(long? id)
        {
            ClassDetailsModel objViewModel = new ClassDetailsModel();
            UniversityCourseViewModel objUniversityCourseViewModel = new UniversityCourseViewModel();
            objViewModel.RowKey = id ?? 0;
            try
            {
                objUniversityCourseViewModel = UniversityCourseService.DeleteClassDetails(objViewModel);

            }
            catch (Exception)
            {
                objUniversityCourseViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objUniversityCourseViewModel);
        }

        [HttpGet]
        public JsonResult CheckDivisionKey(short? DivisionKey)
        {
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CheckClassCodeExists(string ClassCode, long? RowKey)
        {
            UniversityCourseViewModel model = new UniversityCourseViewModel();
            model = UniversityCourseService.CheckClassCodeExists(ClassCode, RowKey ?? 0);
            return Json(model.IsSuccessful, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult CheckRoomNoExists(long? BuildingDetailsKey, long? RowKey)
        {
            UniversityCourseViewModel model = new UniversityCourseViewModel();
            model = UniversityCourseService.CheckRoomNoExists(BuildingDetailsKey, RowKey ?? 0);
            return Json(model.IsSuccessful, JsonRequestBehavior.AllowGet);
        }
        #endregion Class Details

        #region University Course Fee
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UniversityCourse, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditUniversityCourseAmount(long? Id, short? StudentYear)
        {
            UniversityCourseViewModel objViewModel = new UniversityCourseViewModel();
            objViewModel.RowKey = Id ?? 0;
            objViewModel.StudentYear = StudentYear ?? 0;
            var ClassIdentities = UniversityCourseService.GetUniversityCourseFeeById(objViewModel);
            if (ClassIdentities.UniversityCourseFeeModel.Count == 0)
            {
                this.AddToastMessage(EduSuiteUIResources.Warning, EduSuiteUIResources.AddFeeTypeWarning, ToastType.Warning);
                return null;
            }
            return PartialView(ClassIdentities);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UniversityCourse, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult AddEditUniversityCourseAmount(UniversityCourseViewModel model)
        {
            if (ModelState.IsValid)
            {
                model = UniversityCourseService.UpdateUniversityCourseFeeModule(model);

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {

                    return Json(model);
                }

                return Json(model);
            }
            foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            {
            }

            model.Message = EduSuiteUIResources.Failed;
            return Json(model);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UniversityCourse, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteUniversityCourseAmount(long? id)
        {
            UniversityCourseFeeModel objViewModel = new UniversityCourseFeeModel();
            UniversityCourseViewModel objUniversityCourseViewModel = new UniversityCourseViewModel();
            objViewModel.RowKey = id ?? 0;
            try
            {
                objUniversityCourseViewModel = UniversityCourseService.ResetUniversityCourseFee(objViewModel);

            }
            catch (Exception)
            {
                objUniversityCourseViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objUniversityCourseViewModel);
        }
        #endregion University Course Fee

        #region University Course Fee Installment

        [HttpGet]
        public ActionResult AddEditUniverstyCourseFeeInstallment(long? id)
        {
            UniversityCourseFeeInstallmentModel objViewModel = new UniversityCourseFeeInstallmentModel();
            objViewModel.UniversityCourseKey = id ?? 0;
            var universityCourseFeeInstallments = UniversityCourseService.GetFeeInstallmentById(objViewModel);
            if (universityCourseFeeInstallments == null)
            {
                universityCourseFeeInstallments = new UniversityCourseFeeInstallmentModel();
            }
            return View(universityCourseFeeInstallments);
        }

        [HttpPost]
        public ActionResult AddEditUniverstyCourseFeeInstallment(UniversityCourseFeeInstallmentModel model)
        {

            if (ModelState.IsValid)
            {


                model = UniversityCourseService.UpdateFeeInstallment(model);

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

            model.Message = EduSuiteUIResources.Failed;
            return Json(model);

        }

        [HttpPost]
        public ActionResult DeleteUniversityCourseFeeInstallment(Int64 id)
        {
            FeeInstallmentModel objViewModel = new FeeInstallmentModel();
            UniversityCourseFeeInstallmentModel objApplicationFeeInstallmentViewModel = new UniversityCourseFeeInstallmentModel();
            objViewModel.RowKey = id;
            try
            {
                objApplicationFeeInstallmentViewModel = UniversityCourseService.DeleteFeeInstallment(objViewModel);

            }
            catch (Exception)
            {
                objApplicationFeeInstallmentViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objApplicationFeeInstallmentViewModel);
        }

        [HttpGet]
        public ActionResult GetFeeAmount(long? UniversityCourseKey, int? FeeYear)
        {
            UniversityCourseFeeInstallmentModel objViewModel = new UniversityCourseFeeInstallmentModel();
            objViewModel.UniversityCourseKey = UniversityCourseKey ?? 0;
            objViewModel.FeeYear = FeeYear ?? 0;
            var universityCourseFeeInstallments = UniversityCourseService.GetFeeInstallmentById(objViewModel);
            if (universityCourseFeeInstallments == null)
            {
                universityCourseFeeInstallments = new UniversityCourseFeeInstallmentModel();
            }
            return PartialView("AddEditUniverstyCourseFeeInstallment", universityCourseFeeInstallments);
        }

        private void DeleteFile(string FilePath)
        {
            if (!System.IO.File.Exists(Server.MapPath(FilePath)))
            {
                System.IO.File.Delete(Server.MapPath(FilePath));
            }

        }

        #endregion University Course Fee Installment
    }
}