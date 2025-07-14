
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace CITS.EduSuite.UI.Controllers
{
    public class EmployeeAccountController : BaseController
    {
        
            private IEmployeeAccountService employeeAccountService;

            public EmployeeAccountController(IEmployeeAccountService objEmployeeAccountService)
            {
                this.employeeAccountService = objEmployeeAccountService;
            }

            [HttpGet]
            public ActionResult EmployeeAccountList()
            {
                return PartialView();
            }

            [HttpGet]
            //public JsonResult GetEmployee(string searchText)
            //{
            //    int page = 1, rows = 10;

            //    List<EmployeeViewModel> employeeList = new List<EmployeeViewModel>();
            //    employeeList = employeeAccountService.GetEmployee(searchText);

            //    int pageIndex = Convert.ToInt32(page) - 1;
            //    int pageSize = rows;
            //    int totalRecords = employeeList.Count();
            //    var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            //    var jsonData = new
            //    {
            //        total = totalPages,
            //        page,
            //        records = totalRecords,
            //        rows = employeeList
            //    };
            //    return Json(jsonData, JsonRequestBehavior.AllowGet);
            //}

       
            public ActionResult AddEditEmployeeAccount(long? id)
            {
                long EmployeeKey = id != null ? Convert.ToInt64(id) : 0;
                var employeeaccount = employeeAccountService.GetEmployeeAccountById(EmployeeKey);
                if (employeeaccount == null)
                {
                    employeeaccount = new EmployeeAccountViewModel();
                }
                return PartialView(employeeaccount);
            }

            [HttpPost]
            public ActionResult AddEditEmployeeAccount(EmployeeAccountViewModel model)
            {

                if (ModelState.IsValid)
                {
                    if (model.RowKey == 0)
                    {
                        model = employeeAccountService.CreateEmployeeAccount(model);
                    }
                    else
                    {
                        model = employeeAccountService.UpdateEmployeeAccount(model);
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
                    return Json(model);
                }

                model.Message = EduSuiteUIResources.Failed;
                return Json(model);

            }

            [HttpGet]
            public JsonResult CheckAccountNumberExists(string AccountNumber, long rowKey)
            {
                EmployeeAccountViewModel employeeAccount = new EmployeeAccountViewModel();
                employeeAccount = employeeAccountService.CheckAccountNumberExists(AccountNumber, rowKey);
                return Json(employeeAccount.IsSuccessful, JsonRequestBehavior.AllowGet);
            }

            [HttpGet]
            public JsonResult CheckAdharNumberExists(string AdharNumber, long rowKey)
            {
                EmployeeAccountViewModel employeeAccount = new EmployeeAccountViewModel();
                employeeAccount = employeeAccountService.CheckAdharNumberExists(AdharNumber, rowKey);
                return Json(employeeAccount.IsSuccessful, JsonRequestBehavior.AllowGet);
            }

            [HttpPost]
            public ActionResult DeleteEmployeeAccount(Int64 id)
            {
                EmployeeAccountViewModel objViewModel = new EmployeeAccountViewModel();

                objViewModel.RowKey = id;
                try
                {
                    objViewModel = employeeAccountService.DeleteEmployeeAccount(objViewModel);
                }
                catch (Exception)
                {
                    objViewModel.Message = EduSuiteUIResources.Failed;
                }
                return Json(objViewModel);




            }

            //public JsonResult GetDepartmentByBranchId(Int16 id)
            //{
            //    EmployeeAccountViewModel objViewModel = new EmployeeAccountViewModel();
            //    objViewModel = employeeAccountService.GetDepartmentByBranchId(id);
            //    return Json(objViewModel, JsonRequestBehavior.AllowGet);



            
        }
    }
