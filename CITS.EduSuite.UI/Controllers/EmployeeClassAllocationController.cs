using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Interfaces;


using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI.Controllers
{
    public class EmployeeClassAllocationController : BaseController
    {
        private IEmployeeClassAllocationService employeeClassAllocationService;
        private ISharedService sharedService;
        public EmployeeClassAllocationController(IEmployeeClassAllocationService objClassAllocationService, ISharedService objsharedService)
        {
            this.employeeClassAllocationService = objClassAllocationService;
            this.sharedService = objsharedService;
        }
        [HttpGet]
        public ActionResult AddEditEmployeeClassAllocation(int id, long? EmployeesKey)
        {

            EmployeeClassAllocationViewModel model = new EmployeeClassAllocationViewModel();
            model.RowKey = id;
            model.EmployeesKey = EmployeesKey ?? 0;
            model = employeeClassAllocationService.GetEmployeeClassAllocationById(model);

            return PartialView(model);
        }

        [HttpGet]
        public ActionResult GetSubjectDetails(long? ClassDetailsKey)
        {



            EmployeeClassAllocationViewModel model = new EmployeeClassAllocationViewModel();
            model.ClassDetailsKey = ClassDetailsKey ?? 0;
            model = employeeClassAllocationService.GetSubjectByClassDetailskey(model);
            return Json(model.SubjectDetails, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetBatchDetails(long? ClassDetailsKey, long? EmployeesKey)
        {

            EmployeeClassAllocationViewModel model = new EmployeeClassAllocationViewModel();
            model.ClassDetailsKey = ClassDetailsKey ?? 0;
            model.EmployeesKey = EmployeesKey ?? 0;
            model = employeeClassAllocationService.FillBatchDetails(model);
            return Json(model.Batches, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AddEditEmployeeClassAllocation(EmployeeClassAllocationViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = employeeClassAllocationService.CreateEmployeeClassAllocation(model);
                }
                else
                {
                    model = employeeClassAllocationService.UpdateEmployeeClassAllocation(model);
                }
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

        [HttpGet]
        public JsonResult CheckClassCodeExists(long? ClassDetialsKey)
        {
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteClassAllocation(long? id)
        {
            EmployeeClassAllocationViewModel model = new EmployeeClassAllocationViewModel();
            try
            {
                model = employeeClassAllocationService.DeleteEmployeeClassAllocation(id ?? 0);
            }
            catch (Exception)
            {
                model.Message = EduSuiteUIResources.Failed;
            }
            return Json(model);

        }

        [HttpGet]
        public ActionResult CheckIncharge(long? EmployeesKey, long? ClassDetailsKey, short? BatchKey)
        {
            EmployeeClassAllocationViewModel model = new EmployeeClassAllocationViewModel();
            Int64 Employee = EmployeesKey ?? 0;
            Int64 ClassKey = ClassDetailsKey ?? 0;
            model = employeeClassAllocationService.CheckInCharge(Employee, ClassKey, BatchKey ?? 0);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EmployeeClass(long? id)
        {
            EmployeePersonalViewModel objViewModel = new EmployeePersonalViewModel();
            objViewModel.EmployeeKey = id ?? 0;
            objViewModel = sharedService.FillEmployeeDetails("", objViewModel.EmployeeKey);
            return View(objViewModel);
        }

        [HttpGet]
        public ActionResult EmployeeClassDetails(string EmployeeCode, long? EmployeeKey)
        {

            EmployeePersonalViewModel objViewModel = new EmployeePersonalViewModel();
            objViewModel = sharedService.FillEmployeeDetails(EmployeeCode, EmployeeKey);
            if (objViewModel.Message != AppConstants.Common.SUCCESS)
            {
                ModelState.AddModelError("error_msg", objViewModel.Message);
            }
            return PartialView(objViewModel);
        }


        [HttpGet]
        public ActionResult BindEmployeeClassDetails(long? id)
        {
            List<EmployeeClassAllocationViewModel> objViewModel = new List<EmployeeClassAllocationViewModel>();

            objViewModel = employeeClassAllocationService.GetEmployeeClassDetails(id ?? 0);
            ViewBag.EmployeeKey = id ?? 0;

            return PartialView(objViewModel);
        }
    }
}