
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
    public class EmployeeTaskController : BaseController
    {
        // GET: EmployeeTask
        private IEmployeeTaskService employeeTaskService;

        public EmployeeTaskController(IEmployeeTaskService objEmployeeTaskService)
        {
            this.employeeTaskService = objEmployeeTaskService;
        }

        [HttpGet]
        public ActionResult EmployeeTaskList()
        {
            EmployeeTaskViewModel objViewModel = new EmployeeTaskViewModel();
            objViewModel = employeeTaskService.GetBranches(objViewModel);
            return View(objViewModel);
        }

        [HttpGet]
        public JsonResult GetEmployeeTask(long? employeeId, short? branchId)
        {
            int page = 1, rows = 10;

            List<EmployeeTaskViewModel> employeeTasksList = new List<EmployeeTaskViewModel>();
            EmployeeTaskViewModel employeeTaskViewModel = new EmployeeTaskViewModel();
            employeeTaskViewModel.EmployeeKey = employeeId??0;
            employeeTaskViewModel.BranchKey = branchId ?? 0;
            employeeTasksList = employeeTaskService.GetEmployeeTasks(employeeTaskViewModel);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = employeeTasksList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = employeeTasksList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddEditEmployeeTask(long? id, long? employeeKey, short? branchKey)
        {
            EmployeeTaskViewModel employeeTask = new EmployeeTaskViewModel();
            employeeTask.EmployeeKey = employeeKey ??0;
            employeeTask.BranchKey = branchKey ?? 0;
            employeeTask.RowKey = id ?? 0;
            employeeTask = employeeTaskService.GetEmployeeTaskById(employeeTask);
            return PartialView(employeeTask);
        }

        [HttpPost]
        public ActionResult AddEditEmployeeTask(EmployeeTaskViewModel employeeTask)
        {

            if (ModelState.IsValid)
            {
                if (employeeTask.RowKey == 0)
                {
                    employeeTask = employeeTaskService.CreateEmployeeTask(employeeTask);
                }
                else
                {
                    employeeTask = employeeTaskService.UpdateEmployeeTask(employeeTask);
                }

                if (employeeTask.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", employeeTask.Message);
                }
                else
                {
                    return Json(employeeTask);
                }

                employeeTask.Message = "";
                return PartialView(employeeTask);
            }

            employeeTask.Message = EduSuiteUIResources.Failed;
            return PartialView(employeeTask);

        }

        [HttpPost]
        public JsonResult DeleteEmployeeTask(Int64 id)
        {
            EmployeeTaskViewModel objViewModel = new EmployeeTaskViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = employeeTaskService.DeleteEmployeeTask(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }

        [HttpGet]
        public JsonResult GetEmployeesByBranchId(short? id)
        {
            EmployeeTaskViewModel objViewModel = new EmployeeTaskViewModel();
            objViewModel.BranchKey = id ?? 0;
            objViewModel = employeeTaskService.GetEmployeesByBranchId(objViewModel);
            return Json(objViewModel.Employees, JsonRequestBehavior.AllowGet);

        }

        

    }
}