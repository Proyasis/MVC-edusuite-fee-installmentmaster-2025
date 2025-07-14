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
    public class AcademicTermController : BaseController
    {
        private IAcademicTermService AcademicTermService;
        public AcademicTermController(IAcademicTermService objAcademicTermService)
        {
            this.AcademicTermService = objAcademicTermService;
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.AcademicTerm, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult AcademicTermList()
        {
            return View();
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.AcademicTerm, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditAcademicTerm(short? id)
        {
            AcademicTermViewModel model = new AcademicTermViewModel();
            model = AcademicTermService.GetAcademicTermById(id);
            if (model == null)
            {
                model = new AcademicTermViewModel();
            }
            return PartialView(model);

        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.AcademicTerm, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult AddEditAcademicTerm(AcademicTermViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = AcademicTermService.CreateAcademicTerm(model);
                }
                else
                {
                    model = AcademicTermService.UpdateAcademicTerm(model);
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
        public JsonResult GetAcademicTerm(string searchText)
        {
            int page = 1, rows = 15;
            List<AcademicTermViewModel> AcademicTermList = new List<AcademicTermViewModel>();
            AcademicTermList = AcademicTermService.GetAcademicTerm(searchText);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = AcademicTermList.Count();
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = AcademicTermList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.AcademicTerm, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteAcademicTerm(short id)
        {
            AcademicTermViewModel objViewModel = new AcademicTermViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = AcademicTermService.DeleteAcademicTerm(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }
    }
}