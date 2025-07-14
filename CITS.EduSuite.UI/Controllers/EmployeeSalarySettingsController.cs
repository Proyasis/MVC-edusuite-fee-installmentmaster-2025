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
    public class EmployeeSalarySettingsController : BaseController
    {
        // GET: EmployeeSalarySettings
             private IEmployeeSalarySettingsService employeeSalarySettingsService;

         public EmployeeSalarySettingsController(IEmployeeSalarySettingsService objEmployeeIdentityService)
        {
            this.employeeSalarySettingsService = objEmployeeIdentityService;
        }




         [HttpGet]
         public ActionResult AddEditEmployeeSalarySettings(long? id)
         {
             long EmployeeKey = id != null ? Convert.ToInt64(id) : 0;
             var employeeSalarySettings = employeeSalarySettingsService.GetEmployeeSalarySettingsById(EmployeeKey);
             if (employeeSalarySettings == null)
             {
                 employeeSalarySettings = new EmployeeSalaryMasterViewModel();
             }
             return PartialView(employeeSalarySettings);
         }


         [HttpPost]
         public ActionResult AddEditEmployeeSalarySettings(EmployeeSalarySettingsViewModel model)
         {

             if (ModelState.IsValid)
             {

                 model = employeeSalarySettingsService.UpdateEmployeeSalarySettings(model);

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
             return PartialView(model);

         }




         public ActionResult DeleteEmployeeSalarySettings(Int64 id)
         {
             AdditionalSalaryComponentViewModel objViewModel = new AdditionalSalaryComponentViewModel();
             EmployeeSalarySettingsViewModel objEmployeeSalarySettingsViewModel = new EmployeeSalarySettingsViewModel();
             objViewModel.RowKey = id;
             try
             {
                 objEmployeeSalarySettingsViewModel = employeeSalarySettingsService.DeleteEmployeeSalarySettings(objViewModel);
             }
             catch (Exception)
             {
                 objEmployeeSalarySettingsViewModel.Message = EduSuiteUIResources.Failed;
             }
             return Json(objEmployeeSalarySettingsViewModel);
         }

         //[HttpGet]
         //public JsonResult CheckEmployeeSalarySettingsTypeExists(short AdditionalComponentTypeKey, long employeeKey, long rowKey)
         //{
         //    EmployeeSalarySettingsViewModel objEmployeeSalarySettingsViewModel = new EmployeeSalarySettingsViewModel();
         //    objEmployeeSalarySettingsViewModel = employeeSalarySettingsService.CheckEmployeeSalarySettingsTypeExists(AdditionalComponentTypeKey, employeeKey, rowKey);
         //    return Json(objEmployeeSalarySettingsViewModel.IsSuccessful, JsonRequestBehavior.AllowGet);
         //}









    }
}