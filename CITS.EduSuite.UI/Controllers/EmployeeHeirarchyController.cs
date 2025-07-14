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
    public class EmployeeHeirarchyController : BaseController
    {
        private IEmployeeHeirarchyService EmployeeHeirarchyService;

        public EmployeeHeirarchyController(IEmployeeHeirarchyService objEmployeeHeirarchyService)
        {
            this.EmployeeHeirarchyService = objEmployeeHeirarchyService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Employee, ActionCode = ActionConstants.EmployeeHeirarchy)]
        [HttpGet]
        public ActionResult AddEditEmployeeHeirarchy(long? id)
        {
            EmplyeeHeirarchyViewModel emplyeeHeirarchyModel = new EmplyeeHeirarchyViewModel();
            emplyeeHeirarchyModel = EmployeeHeirarchyService.GetEmployeeHeirarchyById(id);
            return PartialView(emplyeeHeirarchyModel);
        }

        [HttpPost]
        public ActionResult AddEditEmployeeHeirarchy(EmplyeeHeirarchyViewModel model)
        {
            if (ModelState.IsValid)
            {
                model = EmployeeHeirarchyService.UpdateEmplyeeHeirarchyPermission(model);
                return Json(model);
            }

            model.Message = EduSuiteUIResources.Failed;
            return Json(model);
        }
    }
}