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
    public class ExamTermController : BaseController
    {
        // GET: ExamCenter
        private IExamTermService ExamTermService;
        public ExamTermController(IExamTermService objExamTermService)
        {
            this.ExamTermService = objExamTermService;
        }
        [HttpGet]
        public ActionResult ExamTermList()
        {
            return View();
        }
        [HttpGet]
        public ActionResult AddEditExamTerm(short? id)
        {
            ExamTermViewModel model = new ExamTermViewModel();
            model = ExamTermService.GetExamTermById(id);
            if (model == null)
            {
                model = new ExamTermViewModel();
            }
            return PartialView(model);
        }


        [HttpPost]
        public ActionResult AddEditExamTerm(ExamTermViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = ExamTermService.CreateExamTerm(model);
                }
                else
                {
                    model = ExamTermService.UpdateExamTerm(model);
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
        public JsonResult GetExamTerm(string searchText)
        {
            int page = 1, rows = 15;
            List<ExamTermViewModel> ExamTermList = new List<ExamTermViewModel>();
            ExamTermList = ExamTermService.GetExamTerm(searchText);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = ExamTermList.Count();
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = ExamTermList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteExamTerm(short id)
        {
            ExamTermViewModel objViewModel = new ExamTermViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = ExamTermService.DeleteExamTerm(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }

        [HttpGet]
        public JsonResult CheckExamTermNameExists(string ExamTermName, short RowKey)
        {
            ExamTermViewModel model = new ExamTermViewModel();
            model.RowKey = RowKey;
            model.ExamTermName = ExamTermName;
            model = ExamTermService.CheckExamTermNameExists(model);
            return Json(model.IsSuccessful, JsonRequestBehavior.AllowGet);
        }
    }
}