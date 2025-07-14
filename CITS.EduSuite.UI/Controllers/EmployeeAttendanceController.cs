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
    public class EmployeeAttendanceController : BaseController
    {
        // GET: EmployeeAttendance
        private IEmployeeAttendanceService employeeAttendanceService;
        public ISelectListService selectListService;

        public EmployeeAttendanceController(IEmployeeAttendanceService objEmployeeAttendanceService, ISelectListService objselectListService)
        {
            this.employeeAttendanceService = objEmployeeAttendanceService;
            this.selectListService = objselectListService;
        }


        [HttpGet]
        public ActionResult EmployeeAttendanceList()
        {
            EmployeeAttendanceViewModel objViewModel = new EmployeeAttendanceViewModel();
            objViewModel = employeeAttendanceService.GetBranches(objViewModel);
            objViewModel.EmployeeAttendanceStatus = selectListService.FillEmployeeAttendanceStatus(objViewModel.BranchKey);
            return View(objViewModel);
        }


        [HttpPost]
        public JsonResult GetEmployeeAttendanceList(long? EmployeeKey, short? BranchKey, short? AttendanceStatusKey, DateTime? FromDate, DateTime? ToDate, string sidx, string sord, int page, int rows)
        {
            List<EmployeeAttendanceViewModel> employeeAttendanceList = new List<EmployeeAttendanceViewModel>();
            EmployeeAttendanceViewModel employeeAttendanceViewModel = new EmployeeAttendanceViewModel();
            employeeAttendanceViewModel.EmployeeKey = EmployeeKey ?? 0;
            employeeAttendanceViewModel.BranchKey = BranchKey ?? 0;
            employeeAttendanceViewModel.AttendanceStatusKey = AttendanceStatusKey ?? 0;
            employeeAttendanceViewModel.SearchFromDate = FromDate;
            employeeAttendanceViewModel.SearchToDate = ToDate;
            employeeAttendanceViewModel.PageIndex = page;
            employeeAttendanceViewModel.PageSize = rows;
            employeeAttendanceViewModel.SortBy = sidx;
            employeeAttendanceViewModel.SortOrder = sord;

            employeeAttendanceList = employeeAttendanceService.GetEmployeeAttendance(employeeAttendanceViewModel);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            long totalRecords = employeeAttendanceViewModel.TotalRecords;
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = employeeAttendanceList
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult DeleteEmployeeAttendance(Int16 id)
        {
            EmployeeAttendanceViewModel objViewModel = new EmployeeAttendanceViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = employeeAttendanceService.DeleteEmployeeAttendance(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }


        [HttpGet]
        public ActionResult AddEditEmployeeAttendance(long? id, long? employeeKey, short? branchKey)
        {
            EmployeeAttendanceViewModel employeeAttendance = new EmployeeAttendanceViewModel();
            employeeAttendance.RowKey = id ?? 0;
            employeeAttendance.EmployeeKey = employeeKey ?? 0;
            employeeAttendance.BranchKey = branchKey ?? 0;
            employeeAttendance = employeeAttendanceService.GetEmployeeAttendanceById(employeeAttendance);
            return PartialView(employeeAttendance);
        }


        [HttpPost]
        public ActionResult AddEditEmployeeAttendance(EmployeeAttendanceViewModel model)
        {

            List<EmployeeAttendanceViewModel> modelList = new List<EmployeeAttendanceViewModel>();
            modelList.Add(model);
            if (ModelState.IsValid)
            {
                model = employeeAttendanceService.UpdateEmployeesAttendance(modelList, true);

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    model.Message = "";
                    return Json(model);
                }

                return PartialView(model);
            }

            model.Message = EduSuiteUIResources.Failed;
            return PartialView(model);

        }


        [HttpPost]
        public ActionResult AddEditEmployeesAttendance(List<EmployeeAttendanceViewModel> modelList)
        {
            EmployeeAttendanceViewModel model = employeeAttendanceService.UpdateEmployeesAttendance(modelList, false);

            if (model.Message != AppConstants.Common.SUCCESS)
            {
                ModelState.AddModelError("error_msg", model.Message);
            }
            else
            {
                model.Message = "";
                return Json(model);
            }
            return Json(model);
        }


        [HttpGet]
        public ActionResult QuickAttendance()
        {
            EmployeeAttendanceViewModel objViewModel = new EmployeeAttendanceViewModel();
            objViewModel = employeeAttendanceService.GetBranches(objViewModel);
            objViewModel.AttendanceConfigType = employeeAttendanceService.GetAttendanceConfigTypeForQuickAttendance(objViewModel);
            objViewModel.AttendanceDate = DateTimeUTC.Now;
            return View(objViewModel);
        }

        [HttpGet]
        public ActionResult AttendanceSheet()
        {
            EmployeeAttendanceViewModel objViewModel = new EmployeeAttendanceViewModel();
            objViewModel = employeeAttendanceService.GetBranches(objViewModel);
            return View(objViewModel);
        }

        [HttpPost]
        public ActionResult AttendanceSheet(List<EmployeeAttendanceViewModel> modelList)
        {
            EmployeeAttendanceViewModel model = employeeAttendanceService.UpdateEmployeesAttendance(modelList, true);

            if (model.Message != AppConstants.Common.SUCCESS)
            {
                ModelState.AddModelError("error_msg", model.Message);
            }
            else
            {
                return Json(model);
            }
            return Json(model);
        }

        public ActionResult GetAttendanceSheet(short? id)
        {
            var attendanceList = new List<EmployeeAttendanceViewModel>();
            attendanceList.Add(new EmployeeAttendanceViewModel { BranchKey = id ?? 0 });
            employeeAttendanceService.FillMultipleDropdownList(attendanceList);
            return PartialView(attendanceList);
        }
        [HttpPost]
        public ActionResult ReadExcel(List<EmployeeAttendanceViewModel> modelList)
        {
            if (modelList == null)
            {
                modelList = new List<EmployeeAttendanceViewModel>();
            }
            employeeAttendanceService.FillMultipleDropdownList(modelList);
            return PartialView("GetAttendanceSheet", modelList);
        }

        [HttpPost]
        public JsonResult GetQuickEmployeeAttendance(short BranchId, DateTime? AttendanceDate)
        {
            int page = 1, rows = 10;

            EmployeeAttendanceViewModel employeeAttendanceViewModel = new EmployeeAttendanceViewModel();
            employeeAttendanceViewModel.BranchKey = BranchId;
            employeeAttendanceViewModel.AttendanceDate = AttendanceDate;
            var employeeAttendanceList = employeeAttendanceService.GetMultipleEmployeeAttendance(employeeAttendanceViewModel, true);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = employeeAttendanceList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = employeeAttendanceList
            };


            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetEmployeesByBranchId(short? id)
        {
            EmployeeAttendanceViewModel objViewModel = new EmployeeAttendanceViewModel();
            objViewModel.BranchKey = id ?? 0;
            objViewModel = employeeAttendanceService.GetEmployeesByBranchId(objViewModel);
            return Json(objViewModel.Employees, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult AddEditEmployeeMultipleAttendance()
        {
            EmployeeAttendanceViewModel employeeAttendance = new EmployeeAttendanceViewModel();
            employeeAttendance = employeeAttendanceService.GetBranches(employeeAttendance);
            return View(employeeAttendance);
        }
        [HttpPost]
        public ActionResult AddEditEmployeeMultipleAttendance(List<EmployeeAttendanceViewModel> modelList)
        {

            EmployeeAttendanceViewModel model = employeeAttendanceService.UpdateEmployeesAttendance(modelList, true);

            if (model.Message != AppConstants.Common.SUCCESS)
            {
                ModelState.AddModelError("error_msg", model.Message);
            }
            else
            {
                return Json(model);
            }
            return Json(model);
        }



        [HttpPost]
        public ActionResult GetMultipleEmployees(EmployeeAttendanceViewModel model)
        {
            var modelList = employeeAttendanceService.GetMultipleEmployeeAttendance(model, false);
            return PartialView(modelList);
        }

        [HttpGet]
        public JsonResult GetEmployeesAttendanceLog(long? id)
        {

            var employeeAttendancelogList = employeeAttendanceService.GetEmployeesAttendanceLog(id ?? 0);
            var jsonData = new
            {
                rows = employeeAttendancelogList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddEditQuickEmployeeAttendance(EmployeeAttendanceViewModel model)
        {

            List<EmployeeAttendanceViewModel> modelList = new List<EmployeeAttendanceViewModel>();
            modelList.Add(model);
            if (ModelState.IsValid)
            {
                model = employeeAttendanceService.UpdateEmployeesAttendance(modelList, false);


                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {

                    return Json(model);
                }


                return Json(model);
            }
            foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            {
            }

            model.Message = EduSuiteUIResources.Failed;
            return Json(model);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EmployeeAttendance, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteEmployeeAttendanceBulk(string Keys)
        {
            EmployeeAttendanceViewModel model = new EmployeeAttendanceViewModel();
            List<long> RowKeys = new List<long>();
            if (Keys != null)
            {
                Keys = String.IsNullOrEmpty((Keys ?? "0")) ? "0" : (Keys ?? "0");
                RowKeys = (Keys).Split(',').Select(row => Int64.Parse(row)).ToList();
            }

            model = employeeAttendanceService.DeleteBulkEmployeeAttendance(model, RowKeys);

            return Json(model);
        }
    }
}