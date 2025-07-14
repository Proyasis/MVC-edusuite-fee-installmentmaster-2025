using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Provider;
using System.Web.Security;
using CITS.EduSuite.Business.Models.Security;
using CITS.EduSuite.Business.Models.Resources;
using System.IO;

namespace CITS.EduSuite.UI.Controllers
{
    public class EmployeeSubjectModuleController : BaseController
    {
        public IEmployeeSubjectModuleService EmployeeSubjectModuleService;
        private ISharedService sharedService;
        public EmployeeSubjectModuleController(IEmployeeSubjectModuleService objEmployeeSubjectModuleService, ISharedService objsharedService)
        {
            this.EmployeeSubjectModuleService = objEmployeeSubjectModuleService;
            this.sharedService = objsharedService;
        }


        [ActionAuthenticationAttribute(MenuCode = MenuConstants.TeacherModuleAllocation, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult EmployeeSubjectModuleList(long? id)
        {
            EmployeePersonalViewModel objViewModel = new EmployeePersonalViewModel();

            objViewModel.EmployeeKey = id ?? 0;
            objViewModel = sharedService.FillEmployeeDetails("", objViewModel.EmployeeKey);
            return View(objViewModel);
        }

        [HttpGet]
        public ActionResult BindEmployeeSubjectModule(long? id)
        {
            EmployeeSubjectModuleViewModel objViewModel = new EmployeeSubjectModuleViewModel();
            objViewModel.EmployeesKey = id;
            objViewModel = EmployeeSubjectModuleService.FillEmployeeSubjectDetails(objViewModel);
            ViewBag.EmployeeKey = id ?? 0;

            return PartialView(objViewModel);
        }

        [HttpGet]
        public ActionResult EmployeeSubjectModule(string EmployeeCode, long? EmployeeKey)
        {
            EmployeePersonalViewModel objViewModel = new EmployeePersonalViewModel();
            objViewModel = sharedService.FillEmployeeDetails(EmployeeCode, EmployeeKey);
            if (objViewModel.Message != AppConstants.Common.SUCCESS)
            {
                ModelState.AddModelError("error_msg", objViewModel.Message);
            }
            return PartialView(objViewModel);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.TeacherModuleAllocation, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult AddEditEmployeeSubjectModule(EmployeeSubjectDetailsModel model)
        {
            EmployeeSubjectModuleViewModel objViewModel = new EmployeeSubjectModuleViewModel();
            //EmployeeSubjectDetailsModel model = new EmployeeSubjectDetailsModel();


            //model.SubjectKey = SubjectKey;
            //model.TeacherClassAllocationKey = TeacherClassAllocationKey ?? 0;
            //model.EmployeeKey = EmployeeKey;
            //model.ClassDetailsKey = ClassDetailsKey;

            objViewModel = EmployeeSubjectModuleService.FillSubjectModules(model);

            return PartialView(objViewModel);


        }

        //[ActionAuthenticationAttribute(MenuCode = MenuConstants.TeacherModuleAllocation, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult AddEditEmployeeSubjectModules(EmployeeSubjectModuleViewModel model)
        {
            EmployeePersonalViewModel objViewModel = new EmployeePersonalViewModel();

            objViewModel.EmployeeKey = model.EmployeesKey;
            if (ModelState.IsValid)
            {

                model = EmployeeSubjectModuleService.UpdateEmployeeSubjectModule(model);
                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    if (model.AutoEmail == true || model.AutoSMS == true)
                        //SendNotificationInBackground(model);
                        return Json(model);

                }

                model.Message = "";
                return Json(model);
            }

            model.Message = EduSuiteUIResources.Failed;
            return Json(model);

        }


    }
}