using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Interfaces;

using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Provider;
using System.Web.Security;
using CITS.EduSuite.Business.Models.Security;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI.Controllers
{
    public class StudentsPromotionController : BaseController
    {
        public IStudentsPromotionService studentPromotionService;

        public StudentsPromotionController(IStudentsPromotionService objStudentPromotionService)
        {
            this.studentPromotionService = objStudentPromotionService;
        }

        public ActionResult StudentsPromotionList()
        {

            StudentsPromotionViewModel objViewModel = new StudentsPromotionViewModel();
           
            objViewModel = studentPromotionService.GetSearchDropdownList(objViewModel);

            return View(objViewModel);
        }

        [HttpPost]
        public JsonResult GetStudentsPromotiondetails(short? BranchKey, long? ClassDetailsKey, short? BatchKey)
        {
            int page = 1; int rows = 15;
            StudentsPromotionViewModel model = new StudentsPromotionViewModel();
            model.BranchKey = BranchKey ?? 0;
            model.ClassDetailsKey = ClassDetailsKey ?? 0;
            model.BatchKey = BatchKey ?? 0;

            List<StudentsPromotionViewModel> StudentsPromotionList = new List<StudentsPromotionViewModel>();
            StudentsPromotionList = studentPromotionService.GetPromotion(model);
            int pageindex = Convert.ToInt32(page) - 1;
            int pagesize = rows;
            int totalrecords = StudentsPromotionList.Count();
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)rows);

            var jsonData = new
            {
                total = totalpage,
                page,
                records = totalrecords,
                rows = StudentsPromotionList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddEditStudentsPromotion(long? Id)
        {
            StudentsPromotionViewModel model = new StudentsPromotionViewModel();
            if (model == null)
            {
                model = new StudentsPromotionViewModel();
            }
            model.RowKey = Id ?? 0;
            model = studentPromotionService.GetPromotionById(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult GetStudentsPromotion(StudentsPromotionViewModel model)
        {

            model = studentPromotionService.FillPromotionDetailsViewModel(model);

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult AddEditStudentsPromotionSubmit(StudentsPromotionViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.RowKey != 0)
                {
                    model = studentPromotionService.UpdatePromotion(model);
                }
                else
                {
                    model = studentPromotionService.CreatePromotion(model);
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

        [HttpGet]
        public JsonResult FillBatch(short BranchKey, long ClassDetailsKey)
        {
            StudentsPromotionViewModel model = new StudentsPromotionViewModel();
            model.BranchKey = BranchKey;
            model.ClassDetailsKey = ClassDetailsKey;
            model = studentPromotionService.FillBatch(model);
            return Json(model.Batches, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FillClassDetails(short Key, short BranchKey)
        {
            StudentsPromotionViewModel model = new StudentsPromotionViewModel();
            model.CourseTypeKey = Key;
            model.BranchKey = BranchKey;
            model = studentPromotionService.FillClassDetails(model);
            return Json(model.ClassDetails, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult FillSearchClassDetails(short Key)
        {
            StudentsPromotionViewModel model = new StudentsPromotionViewModel();
            model.BranchKey = Key;
            model = studentPromotionService.FillSearchClassDetails(model);
            return Json(model.ClassDetails, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult FillSearchBatch(short? BranchKey, long? ClassDetailsKey)
        {
            StudentsPromotionViewModel model = new StudentsPromotionViewModel();
            model.BranchKey = BranchKey ?? 0;
            model.ClassDetailsKey = ClassDetailsKey ?? 0;
            model = studentPromotionService.FillSearchBatch(model);
            return Json(model.Batches, JsonRequestBehavior.AllowGet);
        }
       

        public ActionResult ResetPromotion(long Id)
        {
            StudentsPromotionViewModel model = new StudentsPromotionViewModel();
            try
            {
                model = studentPromotionService.ResetPromotion(Id);
            }
            catch (Exception Ex)
            {
                model.Message = EduSuiteUIResources.Failed;
            }
            return Json(model);
        }

        public ActionResult DeletePromotion(long Id)
        {
            StudentsPromotionViewModel model = new StudentsPromotionViewModel();
            try
            {
                model = studentPromotionService.DeletePromotion(Id);
            }
            catch (Exception Ex)
            {
                model.Message = EduSuiteUIResources.Failed;
            }
            return Json(model);
        }

        [HttpGet]
        public JsonResult FillCourseType(short BranchKey)
        {
            StudentsPromotionViewModel model = new StudentsPromotionViewModel();
            
            model.BranchKey = BranchKey;
            model = studentPromotionService.FillCourseType(model);
            return Json(model.CourseTypes, JsonRequestBehavior.AllowGet);
        }

    }
}