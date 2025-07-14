using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CITS.EduSuite.UI.Controllers
{
    public class EmployeeEnquiryTargetController : BaseController
    {
        private IEmployeeEnquiryTargetService employeeEnquiryTargetService;
        private ISelectListService selectListService;
        public EmployeeEnquiryTargetController(IEmployeeEnquiryTargetService objIEmployeeEnquiryTargetService, ISelectListService objselectListService)
        {
            this.employeeEnquiryTargetService = objIEmployeeEnquiryTargetService;
            this.selectListService = objselectListService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EmployeeEnquiryTarget, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult EmployeeEnquiryTargetList()
        {
            EmployeeEnquiryTargetViewModel model = new EmployeeEnquiryTargetViewModel();

            model.Branches = selectListService.FillBranches();

            return View(model);
        }

        [HttpGet]
        public JsonResult GetEmployeeList(string searchText, short? BranchKey, short? EmployeeStatusKey, string sidx, string sord, int page, int rows)
        {

            long TotalRecords = 0;
            List<EmployeeEnquiryTargetViewModel> employeeList = new List<EmployeeEnquiryTargetViewModel>();
            EmployeeEnquiryTargetViewModel objViewModel = new EmployeeEnquiryTargetViewModel();

            objViewModel.EmployeeName = searchText;
            objViewModel.BranchKey = BranchKey ?? 0;
            objViewModel.EmployeeStatusKey = EmployeeStatusKey ?? 0;
            objViewModel.PageIndex = page;
            objViewModel.PageSize = rows;
            objViewModel.SortBy = sidx;
            objViewModel.SortOrder = sord;

            employeeList = employeeEnquiryTargetService.GetEmployeeList(objViewModel, out TotalRecords);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            var totalPages = (int)Math.Ceiling((float)TotalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = TotalRecords,
                rows = employeeList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EmployeeEnquiryTarget, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditEmployeeEnquiryTarget(long? id)
        {
            EmployeeEnquiryTargetViewModel model = new EmployeeEnquiryTargetViewModel();
            model.EmployeeKey = id ?? 0;
            model = employeeEnquiryTargetService.GetEmployeeEnquiryTargetId(model);
            if (model == null)
            {
                model = new EmployeeEnquiryTargetViewModel();
            }
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult AddEditEmployeeEnquiryTarget(EmployeeEnquiryTargetViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = employeeEnquiryTargetService.CreateEnquiryTarget(model);
                }
                else
                {
                    model = employeeEnquiryTargetService.UpdateEnquiryTarget(model);
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
            foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            {
            }

            return Json(model);
        }


        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EmployeeEnquiryTarget, ActionCode = ActionConstants.Delete)]
        public ActionResult DeleteEnquiryTargetDetails(long? id)
        {
            EmployeeEnquiryTargetDetailsViewModel model = new EmployeeEnquiryTargetDetailsViewModel();
            try
            {
                model.RowKey = id ?? 0;
                model = employeeEnquiryTargetService.DeleteEnquiryTargetDetails(model);
            }
            catch (Exception)
            {

            }
            return Json(model);
        }
    }
}