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
    public class FieldValidationController : Controller
    {
        private IFieldValidationService FieldValidationServic;
        public FieldValidationController(IFieldValidationService objFieldValidationServic)
        {
            this.FieldValidationServic = objFieldValidationServic;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.FieldValidation, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult FieldValidationList()
        {
            return View();
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.FieldValidation, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditFieldValidation(short id)
        {
            FieldValidationViewModel model = new FieldValidationViewModel();
            model = FieldValidationServic.GetFieldValidationById(id);
            if (model == null)
            {
                model = new FieldValidationViewModel();
            }
            return View(model);

        }
        [HttpPost]
        public ActionResult AddEditFieldValidation(FieldValidationViewModel model)
        {

            if (ModelState.IsValid)
            {

                model = FieldValidationServic.UpdateFieldValidaion(model);


                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    SettingsMapping settings = new SettingsMapping();

                    settings.SyncOrderSettings();

                    return RedirectToAction("FieldValidationList");
                }

                model.Message = "";
                return View(model);
            }



            model.Message = EduSuiteUIResources.Failed;

            return View(model);
        }
        public JsonResult GetFieldValidation(string searchText)
        {
            int page = 1, rows = 15;
            List<FieldValidationViewModel> FieldValidationList = new List<FieldValidationViewModel>();
            FieldValidationList = FieldValidationServic.GetFieldValidation(searchText);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = FieldValidationList.Count();
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = FieldValidationList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

    }
}