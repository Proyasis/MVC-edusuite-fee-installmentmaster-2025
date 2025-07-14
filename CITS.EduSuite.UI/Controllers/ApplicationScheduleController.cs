using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Provider;
using CITS.EduSuite.Business.Models.Security;
using System.Web.Security;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI.Controllers
{
    public class ApplicationScheduleController : BaseController
    {
        private IApplicationScheduleService applicationScheduleService;
        public ISelectListService selectListService;
        public ApplicationScheduleController(IApplicationScheduleService objApplicationScheduleService, ISelectListService objselectListService)
        {
            this.applicationScheduleService = objApplicationScheduleService;
            this.selectListService = objselectListService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.ApplicationSchedule, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult ApplicationScheduleList()
        {
            ApplicationScheduleViewModel model = new ApplicationScheduleViewModel();
           
            model.Branches = selectListService.FillBranches();
            model.Batches = selectListService.FillBatches(model.SearchBranchKey);
            model.Courses = selectListService.FillCoursesById(null);
            model.Universities = selectListService.FillUniversitiesById(null);
            model.StudentStatuses = selectListService.FillStudentStatuses();
            applicationScheduleService.FillSearchApplicationScheduleTypes(model);
            applicationScheduleService.FillSearchApplicationCallStatus(model);
            applicationScheduleService.FillSearchCallTypes(model);
            applicationScheduleService.FillSearchProcessStatus(model);
            model.SearchTabKey = DbConstants.ScheduleStatus.Today;
            return View(model);
        }

        [HttpPost]
        public ActionResult ApplicationScheduleDetails(ApplicationScheduleViewModel model)
        {
            applicationScheduleService.GetApplicationSchedule(model);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult ApplicationScheduleCounts(ApplicationScheduleViewModel model)
        {
            applicationScheduleService.GetApplicationSchedule(model);
            return Json(model.TotalRecords);
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.ApplicationSchedule, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditApplicationSchedule(long? id, long? ApplicationKey,short? ApplicationScheduleTypeKey)
        {
            ApplicationScheduleDetailsViewModel model = new ApplicationScheduleDetailsViewModel();
            model.RowKey = id ?? 0;
            model.ApplicationKey = ApplicationKey ?? 0;
            model.ApplicationScheduleTypeKey = ApplicationScheduleTypeKey ?? 0;

            model = applicationScheduleService.GetApplicationScheduleById(model);
            return PartialView(model);
        }


        [HttpPost]
        public ActionResult AddEditApplicationSchedule(ApplicationScheduleDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = applicationScheduleService.CreateApplicationSchedule(model);
                }
                else
                {
                    model = applicationScheduleService.UpdateApplicationSchedule(model);
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
                return PartialView(model);
            }

            model.Message = EduSuiteUIResources.Failed;
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult DeleteApplicationschedule(Int16 id)
        {
            ApplicationScheduleDetailsViewModel objViewModel = new ApplicationScheduleDetailsViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = applicationScheduleService.DeleteApplicationSchedule(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }

        public List<ApplicationScheduleViewModel> ScheduleList { get; set; }
        public object EduSuiteResourceManager { get; private set; }

        [HttpGet]
        public JsonResult CheckDuration(short? id)
        {
            ApplicationScheduleDetailsViewModel model = new ApplicationScheduleDetailsViewModel();
            model.ApplicationCallStatusKey = id ?? 0;
            model = applicationScheduleService.CheckDuration(model);
            return Json(model.IsDuration, JsonRequestBehavior.AllowGet);
        }   
        
    }
}