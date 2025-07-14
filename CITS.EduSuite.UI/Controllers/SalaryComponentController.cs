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
    public class SalaryComponentController : BaseController
    {
        
        private ISalaryComponentService salaryComponentService;

        public SalaryComponentController(ISalaryComponentService objsalaryComponentService)
        {
            this.salaryComponentService = objsalaryComponentService;
        }

        [HttpGet]
        public ActionResult SalaryComponentList()
        {
            SalaryComponentViewModel objSalaryComponentViewModel = new SalaryComponentViewModel();
            objSalaryComponentViewModel = salaryComponentService.GetComponentTypes(objSalaryComponentViewModel);
            objSalaryComponentViewModel = salaryComponentService.GetBranches(objSalaryComponentViewModel);
            return View(objSalaryComponentViewModel);
        }



        [HttpGet]
        public JsonResult GetSalaryComponentsByType(short ComponentTypeId,long? employeeId, short? branchId)
        {
            int page = 1, rows = 10;

            List<SalaryComponentViewModel> salaryComponentList = new List<SalaryComponentViewModel>();
            SalaryComponentViewModel objSalaryComponentViewModel = new SalaryComponentViewModel();
            objSalaryComponentViewModel.SalaryComponentTypeKey = ComponentTypeId;
            objSalaryComponentViewModel.EmployeeKey = employeeId??0;
            objSalaryComponentViewModel.BranchKey = branchId ?? 0;
            salaryComponentList = salaryComponentService.GetSalaryComponentsByType(objSalaryComponentViewModel);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = salaryComponentList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = salaryComponentList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult AddEditSalaryComponent(long? id, short ComponentTypeKey, long? employeeKey, short? branchKey)
        {
            SalaryComponentViewModel salaryComponent = new SalaryComponentViewModel();
            salaryComponent.RowKey = id??0;
            salaryComponent.SalaryComponentTypeKey = ComponentTypeKey;
            salaryComponent.EmployeeKey = employeeKey??0;
            salaryComponent.BranchKey = branchKey ?? 0;
            salaryComponent = salaryComponentService.GetSalaryComponentById(salaryComponent);        
            return PartialView(salaryComponent);
        }



        [HttpPost]
        public ActionResult AddEditSalaryComponent(SalaryComponentViewModel salaryComponent)
        {

            if (ModelState.IsValid)
            {
                if (salaryComponent.RowKey == 0)
                {
                    salaryComponent = salaryComponentService.CreateSalaryComponent(salaryComponent);
                }
                else
                {
                    salaryComponent = salaryComponentService.UpdateSalaryComponent(salaryComponent);
                }

                if (salaryComponent.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", salaryComponent.Message);
                }
                else
                {
                    return Json(salaryComponent);
                }

                salaryComponent.Message = "";
                return PartialView(salaryComponent);
            }

            salaryComponent.Message =  EduSuiteUIResources.Failed;  
            return PartialView(salaryComponent);

        }

        [HttpPost]
        public ActionResult DeleteSalaryComponent(Int64 id)
        {
            SalaryComponentViewModel objViewModel = new SalaryComponentViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = salaryComponentService.DeleteSalaryComponent(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message =  EduSuiteUIResources.Failed;  
            }
            return Json(objViewModel);
        }

        [HttpGet]
        public JsonResult GetEmployeesByBranchId(short? id)
        {
            SalaryComponentViewModel objViewModel = new SalaryComponentViewModel();
            objViewModel.BranchKey = id ?? 0;
            objViewModel = salaryComponentService.GetEmployeesByBranchId(objViewModel);
            return Json(objViewModel.Employees, JsonRequestBehavior.AllowGet);

        }

    }
}