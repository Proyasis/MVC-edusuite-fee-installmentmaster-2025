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
    public class EmployeeLoanController : BaseController
    {
        // GET: EmployeeLoan
      private IEmployeeLoanService employeeLoanService;

      public EmployeeLoanController(IEmployeeLoanService objEmployeeLoanService)
        {
            this.employeeLoanService = objEmployeeLoanService;
        }

        [HttpGet]
        public ActionResult EmployeeLoanList()
        {
            EmployeeLoanViewModel objViewModel = new EmployeeLoanViewModel();
            objViewModel = employeeLoanService.GetBranches(objViewModel);
            return View(objViewModel);
        }



        [HttpGet]
        public JsonResult GetEmployeeLoan(long? employeeId, short? branchId)
        {
            int page = 1, rows = 10;

            List<EmployeeLoanViewModel> employeeLoanList = new List<EmployeeLoanViewModel>();
            EmployeeLoanViewModel employeeLoanViewModel = new EmployeeLoanViewModel();
            employeeLoanViewModel.EmployeeKey = employeeId??0;
            employeeLoanViewModel.BranchKey = branchId ?? 0;
            employeeLoanList = employeeLoanService.GetEmployeeLoan(employeeLoanViewModel);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = employeeLoanList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = employeeLoanList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult AddEditEmployeeLoan(long? id, long? employeeKey,short? branchKey)
        {
            EmployeeLoanViewModel employeeLoan = new EmployeeLoanViewModel();
            employeeLoan.EmployeeKey = employeeKey ??0;
            employeeLoan.BranchKey = branchKey ?? 0;
            employeeLoan.RowKey = id ?? 0;
            employeeLoan = employeeLoanService.GetEmployeeLoanById(employeeLoan);
            return PartialView(employeeLoan);
        }



        [HttpPost]
        public ActionResult AddEditEmployeeLoan(EmployeeLoanViewModel employeeLoan)
        {

            if (ModelState.IsValid)
            {
                if (employeeLoan.RowKey == 0)
                {
                    employeeLoan = employeeLoanService.CreateEmployeeLoan(employeeLoan);
                }
                else
                {
                    employeeLoan = employeeLoanService.UpdateEmployeeLoan(employeeLoan);
                }

                if (employeeLoan.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", employeeLoan.Message);
                }
                else
                {
                    return Json(employeeLoan);
                }

                employeeLoan.Message = "";
                return PartialView(employeeLoan);
            }

            employeeLoan.Message = EduSuiteUIResources.Failed;
            return PartialView(employeeLoan);

        }

        [HttpPost]
        public ActionResult DeleteEmployeeLoan(Int64 id)
        {
            EmployeeLoanViewModel objViewModel = new EmployeeLoanViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = employeeLoanService.DeleteEmployeeLoan(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }

        [HttpGet]
        public ActionResult AddEditEmployeeLoanPayment(long? id)
        {
            var employeeLoanPayment = employeeLoanService.GetEmployeeLoanPaymentById(id ?? 0);
            var Url = "~/Views/Shared/PaymentWindow.cshtml";
            return PartialView(Url, employeeLoanPayment);
        }

        [HttpPost]
        public ActionResult AddEditEmployeeLoanPayment(PaymentWindowViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.PaymentKey == 0)
                {
                    model = employeeLoanService.CreateLoanPayment(model);
                }
                else
                {
                    model = employeeLoanService.UpdateLoanPayment(model);
                }

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    return Json(model);
                }

                //model.Message = "";
                return Json(model);
            }

            model.Message = EduSuiteUIResources.Failed;
            return PartialView(model);

        }

        [HttpGet]
        public JsonResult GetEmployeesByBranchId(short? id)
        {
            EmployeeLoanViewModel objViewModel = new EmployeeLoanViewModel();
            objViewModel.BranchKey = id ?? 0;
            objViewModel = employeeLoanService.GetEmployeesByBranchId(objViewModel);
            return Json(objViewModel.Employees,JsonRequestBehavior.AllowGet);

        }


    }
}