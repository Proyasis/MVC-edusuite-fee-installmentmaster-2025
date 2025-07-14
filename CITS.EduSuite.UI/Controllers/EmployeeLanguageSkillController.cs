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
    public class EmployeeLanguageSkillController : BaseController
    {
        private IEmployeeLanguageSkillService employeeLanguageSkillsService;

        public EmployeeLanguageSkillController(IEmployeeLanguageSkillService objEmployeeLanguageSkillService)
        {
            this.employeeLanguageSkillsService = objEmployeeLanguageSkillService;
        }

        [HttpGet]
        public ActionResult EmployeeLanguageSkillList()
        {
            return View();
        }

    
        [HttpGet]
        public ActionResult AddEditEmployeeLanguageSkills(long? id)
        {
            long EmployeeKey = id != null ? Convert.ToInt64(id) : 0;
            var employeeLanguageSkills = employeeLanguageSkillsService.GetEmployeeLanguageSkillsById(EmployeeKey);
            if (employeeLanguageSkills == null)
            {
                employeeLanguageSkills = new EmployeeLanguageSkillViewModel();
            }
            return PartialView(employeeLanguageSkills);
        }

        [HttpPost]
        public ActionResult AddEditEmployeeLanguageSkills(EmployeeLanguageSkillViewModel model)
        {

            if (ModelState.IsValid)
            {

                model = employeeLanguageSkillsService.UpdateEmployeeLanguageSkills(model);

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
        public ActionResult DeleteEmployeeLanguageSkill(Int64 id)
        {
            languageSkillsViewModel objViewModel = new languageSkillsViewModel();
            EmployeeLanguageSkillViewModel objEmployeeLanguageSkillViewModel = new EmployeeLanguageSkillViewModel();
            objViewModel.RowKey = id;
            try
            {
                objEmployeeLanguageSkillViewModel = employeeLanguageSkillsService.DeleteEmployeeLanguageSkills(objViewModel);
            }
            catch (Exception)
            {
                objEmployeeLanguageSkillViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objEmployeeLanguageSkillViewModel);
        }

        [HttpGet]
        public JsonResult CheckLanguageSkillExists(short LanguageKey, long EmployeeKey, long RowKey)
        {
            EmployeeLanguageSkillViewModel employeeLanguageSkiils = new EmployeeLanguageSkillViewModel();
            employeeLanguageSkiils = employeeLanguageSkillsService.CheckIdentityTypeExists(LanguageKey,EmployeeKey,  RowKey);
            return Json(employeeLanguageSkiils.IsSuccessful, JsonRequestBehavior.AllowGet);
        }
    }
}