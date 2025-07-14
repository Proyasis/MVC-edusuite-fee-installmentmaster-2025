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
    public class LeaveApplicationController : BaseController
    {
        // GET: LeaveApplication
        private ILeaveApplicationService leaveApplicationService;

        public LeaveApplicationController(ILeaveApplicationService objLeaveApplicationService)
        {
            this.leaveApplicationService = objLeaveApplicationService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.LeaveApplication, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult LeaveApplicationList()
        {
            LeaveApplicationViewModel objViewModel = new LeaveApplicationViewModel();
            objViewModel = leaveApplicationService.GetBranches(objViewModel);
            return View(objViewModel);
        }

        [HttpGet]
        public JsonResult GetLeaveApplication(long? employeeId, short? branchId)
        {
            int page = 1, rows = 10;

            List<LeaveApplicationViewModel> leaveApplicationsList = new List<LeaveApplicationViewModel>();
            LeaveApplicationViewModel leaveApplicationViewModel = new LeaveApplicationViewModel();
            leaveApplicationViewModel.EmployeeKey = employeeId ?? 0;
            leaveApplicationViewModel.BranchKey = branchId ?? 0;
            leaveApplicationsList = leaveApplicationService.GetLeaveApplication(leaveApplicationViewModel);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = leaveApplicationsList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = leaveApplicationsList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.LeaveApplication, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditLeaveApplication(long? id, long? employeeKey, short? branchKey)
        {
            LeaveApplicationViewModel LeaveApplication = new LeaveApplicationViewModel();
            LeaveApplication.EmployeeKey = employeeKey ?? 0;
            LeaveApplication.BranchKey = branchKey ?? 0;
            LeaveApplication.RowKey = id ?? 0;
            LeaveApplication = leaveApplicationService.GetLeaveApplicationById(LeaveApplication);
            if (LeaveApplication == null)
            {
                LeaveApplication = new LeaveApplicationViewModel();
            }
            return PartialView(LeaveApplication);
        }

        [HttpPost]
        public ActionResult AddEditLeaveApplication(LeaveApplicationViewModel leaveApplication)
        {

            if (ModelState.IsValid)
            {
                if (leaveApplication.RowKey == 0)
                {
                    leaveApplication = leaveApplicationService.CreateLeaveApplication(leaveApplication);
                }
                else
                {
                    leaveApplication = leaveApplicationService.UpdateLeaveApplication(leaveApplication);
                }

                if (leaveApplication.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", leaveApplication.Message);
                }
                else
                {
                    return Json(leaveApplication);
                }

                leaveApplication.Message = "";
                return PartialView(leaveApplication);
            }

            leaveApplication.Message =  EduSuiteUIResources.Failed;  
            return PartialView(leaveApplication);

        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.LeaveApplication, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteLeaveApplication(Int64 id)
        {
            LeaveApplicationViewModel objViewModel = new LeaveApplicationViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = leaveApplicationService.DeleteLeaveApplication(objViewModel);
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
            LeaveApplicationViewModel objViewModel = new LeaveApplicationViewModel();
            objViewModel.BranchKey = id ?? 0;
            objViewModel = leaveApplicationService.GetEmployeesByBranchId(objViewModel);
            return Json(objViewModel.Employees, JsonRequestBehavior.AllowGet);

        }

        
        [HttpPost]
        public ActionResult ApproveLeaveApplication(long? RowKey, short? LeaveStatusKey)
        {
            LeaveApplicationViewModel objViewModel = new LeaveApplicationViewModel();

            objViewModel.RowKey = RowKey ?? 0;
            objViewModel.LeaveStatusKey = LeaveStatusKey ?? 0;
            try
            {
                objViewModel = leaveApplicationService.ApproveLeaveApplication(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message =  EduSuiteUIResources.Failed;  
            }
            return Json(objViewModel);
        }
    }
}