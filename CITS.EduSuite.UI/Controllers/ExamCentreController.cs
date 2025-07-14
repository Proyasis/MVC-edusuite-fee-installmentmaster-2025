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
    public class ExamCentreController : BaseController
    {
        // GET: ExamCenter
         private IExamCentreService ExamCentreService;
         public ExamCentreController(IExamCentreService objExamCentreService)
        {
            this.ExamCentreService = objExamCentreService;
        }
         [HttpGet]
         public ActionResult ExamCentreList()
         {
             return View();
         }
        [HttpGet]
        public ActionResult AddEditExamCentre(short? id)
        {
            ExamCentreViewModel model = new ExamCentreViewModel();
            model = ExamCentreService.GetExamCentreById(id);
            if (model == null)
            {
                model = new ExamCentreViewModel();
            }
            return PartialView(model);
        }


        [HttpPost]
        public ActionResult AddEditExamCentre(ExamCentreViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = ExamCentreService.CreateExamCentre(model);
                }
                else
                {
                    model = ExamCentreService.UpdateExamCentre(model);
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
        public JsonResult GetExamCentre(string searchText)
        {
            int page = 1, rows = 15;
            List<ExamCentreViewModel> ExamCentreList = new List<ExamCentreViewModel>();
            ExamCentreList = ExamCentreService.GetExamCentre(searchText);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = ExamCentreList.Count();
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = ExamCentreList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteExamCentre(short id)
        {
            ExamCentreViewModel objViewModel = new ExamCentreViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = ExamCentreService.DeleteExamCentre(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }
        [HttpGet]
        public JsonResult CheckExamCentreCodeExists(string ExamCentreCode, short RowKey)
        {
            ExamCentreViewModel model = new ExamCentreViewModel();
            model.RowKey = RowKey;
            model.ExamCentreCode = ExamCentreCode;
            model = ExamCentreService.CheckExamCentreCodeExists(model);
            return Json(model.IsSuccessful, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult CheckExamCentreNameExists(string ExamCentreName, short RowKey)
        {
            ExamCentreViewModel model = new ExamCentreViewModel();
            model.RowKey = RowKey;
            model.ExamCentreName = ExamCentreName;
            model = ExamCentreService.CheckExamCentreNameExists(model);
            return Json(model.IsSuccessful, JsonRequestBehavior.AllowGet);
        }
    }
}