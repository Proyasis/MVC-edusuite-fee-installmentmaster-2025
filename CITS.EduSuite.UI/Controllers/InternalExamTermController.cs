using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.UI.Controllers
{
    public class InternalExamTermController : BaseController
    {
        private IInternalExamTermService InternalExamTermService;

        public InternalExamTermController(IInternalExamTermService objInternalExamTermService)
        {
            this.InternalExamTermService = objInternalExamTermService;
        }

        [HttpGet]
        public ActionResult InternalExamTermList()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AddEditInternalExamTerm(int? id)
        {
            InternalExamTermViewModel model = new InternalExamTermViewModel();
            model = InternalExamTermService.GetInternalExamTermById(id);
            if (model == null)
            {
                model = new InternalExamTermViewModel();
            }
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult AddEditInternalExamTerm(InternalExamTermViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = InternalExamTermService.CreateInternalExamTerm(model);
                }
                else
                {
                    model = InternalExamTermService.UpdateInternalExamTerm(model);

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
        public JsonResult GetInternalExamTerm(string searchText)
        {
            int page = 1, rows = 15;
            List<InternalExamTermViewModel> InternalExamTermList = new List<InternalExamTermViewModel>();
            InternalExamTermList = InternalExamTermService.GetInternalExamTerm(searchText);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = InternalExamTermList.Count();
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = InternalExamTermList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteInternalExamTrem(short id)
        {
            InternalExamTermViewModel objViewModel = new InternalExamTermViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = InternalExamTermService.DeleteInternalExamTerm(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }
    }
}