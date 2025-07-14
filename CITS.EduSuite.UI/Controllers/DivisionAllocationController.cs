using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Interfaces;

using CITS.EduSuite.Business.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Provider;
using CITS.EduSuite.Business.Models.Security;
using System.Web.Security;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI.Controllers
{
    public class DivisionAllocationController : BaseController
    {
        public IDivisionAllocationService divisionAllocationService;

        public DivisionAllocationController(IDivisionAllocationService objDivisionAllocationService)
        {
            this.divisionAllocationService = objDivisionAllocationService;
        }


        [ActionAuthenticationAttribute(MenuCode = MenuConstants.GenerateRollNumber, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult DivisionAllocationList()
        {
            DivisionAllocationViewModel model = new DivisionAllocationViewModel();

            model = divisionAllocationService.GetSearchDropdownList(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult GetDivisionAllocation(DivisionAllocationViewModel model)
        {

            model = divisionAllocationService.GetDivisionAllocationById(model);
            //if (!model.IsSuccessful)
            //{
            //    return Json(model);
            //}
            return PartialView(model.DivisionAllocationDetails);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.GenerateRollNumber, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditDivisionAllocation(DivisionAllocationViewModel model)
        {
            if (model == null)
            {
                model = new DivisionAllocationViewModel();
            }

            model = divisionAllocationService.GetDivisionAllocationById(model);
            return View(model);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.GenerateRollNumber, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult AddEditDivisionAllocationSubmit(List<DivisionAllocationDetailsModel> modelList, long? ClassDetailsKey, short? BatchKey)
        {
            DivisionAllocationViewModel model = new DivisionAllocationViewModel();
            if (ModelState.IsValid)
            {

                model.DivisionAllocationDetails = modelList;
                model.ClassDetailsKey = ClassDetailsKey ?? 0;
                model.BatchKey = BatchKey ?? 0;
                model = divisionAllocationService.UpdateDivisionAllocation(model);
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

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.GenerateRollNumber, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteDivisionAllocation(long? CourseKey, short? UniversityMasterKey, short? AcademicTermKey, short? BatchKey, short? CourseYear, long? ClassDetailsKey, short? BranchKey)
        {
            DivisionAllocationViewModel model = new DivisionAllocationViewModel();


            model.CourseKey = CourseKey ?? 0;
            model.UniversityMasterKey = UniversityMasterKey ?? 0;
            model.AcademicTermKey = AcademicTermKey ?? 0;
            model.BatchKey = BatchKey ?? 0;
            model.CourseYear = CourseYear ?? 0;
            model.ClassDetailsKey = ClassDetailsKey ?? 0;
            model.BranchKey = BranchKey ?? 0;

            try
            {
                model = divisionAllocationService.DeleteDivisionAllocation(model);
            }
            catch (Exception)
            {
                model.Message = EduSuiteUIResources.Failed;
            }
            return Json(model);
        }

        [HttpPost]
        public JsonResult GetDivisionAllocationdetails(string SearchText, short? BranchKey, short? BatchKey)
        {
            int page = 1; int rows = 15;
            DivisionAllocationViewModel model = new DivisionAllocationViewModel();
            List<DivisionAllocationViewModel> DivisionAllocationList = new List<DivisionAllocationViewModel>();
            model.searchText = SearchText;
            model.BranchKey = BranchKey ?? 0;
            model.BatchKey = BatchKey ?? 0;
            DivisionAllocationList = divisionAllocationService.GetDivisionAllocation(model);
            int pageindex = Convert.ToInt32(page) - 1;
            int pagesize = rows;
            int totalrecords = DivisionAllocationList.Count();
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)rows);

            var jsonData = new
            {
                total = totalpage,
                page,
                records = totalrecords,
                rows = DivisionAllocationList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult FillCourseType(short key)
        //{
        //    DivisionAllocationViewModel model = new DivisionAllocationViewModel();
        //    model.AcademicTermKey = key;
        //    model = divisionAllocationService.FillCourseType(model);
        //    return Json(model.CourseTypes, JsonRequestBehavior.AllowGet);
        //}


        //public JsonResult FillCourse(short key)
        //{
        //    DivisionAllocationViewModel model = new DivisionAllocationViewModel();
        //    model.CourseTypeKey = key;
        //    model = divisionAllocationService.FillCourse(model);
        //    return Json(model.Courses, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult FillUniversity(long key, short AcademicTermKey)
        //{
        //    DivisionAllocationViewModel model = new DivisionAllocationViewModel();
        //    model.AcademicTermKey = AcademicTermKey;
        //    model.CourseKey = key;
        //    model = divisionAllocationService.FillUniversity(model);
        //    return Json(model.Universities, JsonRequestBehavior.AllowGet);
        //}


        //public JsonResult FillCourseYear(long key, short AcademicTermKey)
        //{
        //    DivisionAllocationViewModel model = new DivisionAllocationViewModel();
        //    model.AcademicTermKey = AcademicTermKey;
        //    model.CourseKey = key;
        //    model = divisionAllocationService.FillCourseYear(model);
        //    return Json(model.CourseYears, JsonRequestBehavior.AllowGet);
        //}

        [HttpGet]
        public JsonResult FillClassDetails(short Key)
        {
            DivisionAllocationViewModel model = new DivisionAllocationViewModel();
            model.CourseTypeKey = Key;
            model = divisionAllocationService.FillClassDetails(model);
            return Json(model.ClassDetails, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FillBatch(short BranchKey, long ClassDetailsKey)
        {
            DivisionAllocationViewModel model = new DivisionAllocationViewModel();
            model.BranchKey = BranchKey;
            model.ClassDetailsKey = ClassDetailsKey;
            //model.UniversityMasterKey = UniversityMasterKey;
            model = divisionAllocationService.FillBatch(model);
            return Json(model.Batches, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FillSearchBatch(short? BranchKey)
        {
            DivisionAllocationViewModel model = new DivisionAllocationViewModel();
            model.BranchKey = BranchKey ?? 0;
            model = divisionAllocationService.FillSearchBatch(model);
            return Json(model.Batches, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //public ActionResult GenerateRollnumber(string ApplicationKeys, short? BatchKey, long? ClassDetailsKey, bool? IfResetRollNo)
        public ActionResult GenerateRollnumber(DivisionAllocationViewModel model)
        {
            // DivisionAllocationViewModel objViewModel = new DivisionAllocationViewModel();

            //objViewModel.ApplicationKeys = ApplicationKeys;


            //objViewModel.BatchKey = BatchKey ?? 0;
            //objViewModel.ClassDetailsKey = ClassDetailsKey ?? 0;
            //objViewModel.IfResetRollNo = IfResetRollNo ?? false;

            model = divisionAllocationService.GenerateRollnumber(model);
            return PartialView(model);
        }

        [HttpGet]
        public JsonResult GetRollNumberList(string ApplicationKeys)
        {

            long TotalRecords = 0;
            DivisionAllocationViewModel objViewModel = new DivisionAllocationViewModel();

            var jsonData = new
            {
                records = objViewModel.DivisionAllocationDetails.Count,
                rows = objViewModel.DivisionAllocationDetails
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ResetDivisionAllocation(long Id)
        {
            DivisionAllocationViewModel objviewModel = new DivisionAllocationViewModel();

            try
            {
                objviewModel = divisionAllocationService.ResetDivisionAllocation(Id);
            }
            catch (Exception)
            {
                objviewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objviewModel);

        }

    }
}