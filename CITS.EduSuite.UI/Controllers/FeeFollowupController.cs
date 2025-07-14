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
    public class FeeFollowupController : BaseController
    {
        private IFeeFollowUpService FeeFollowUpService;
        public ISelectListService selectListService;
        public FeeFollowupController(IFeeFollowUpService objFeeFollowUpService,ISelectListService objselectListService)
        {
            this.FeeFollowUpService = objFeeFollowUpService;
            this.selectListService = objselectListService;
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.FeeSchdeule, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult FollowupList()
        {
            ApplicationFeeFollowupViewModel model = new ApplicationFeeFollowupViewModel();
            
            model.Branches = selectListService.FillBranches();
            model.Batches = selectListService.FillBatches(model.SearchBranchKey);
            model.Courses = selectListService.FillCoursesById(null);
            model.Universities = selectListService.FillUniversitiesById(null);
            model.StudentStatuses = selectListService.FillStudentStatuses();
            FeeFollowUpService.FillSearchProcessStatus(model);

            model.SearchTabKey =  DbConstants.ScheduleStatus.Today;
            return View(model);
        }

        [HttpPost]
        public ActionResult FollowupPartial(ApplicationFeeFollowupViewModel model)
        {
            FeeFollowUpService.GetFollowup(model);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult FollowupCounts(ApplicationFeeFollowupViewModel model)
        {
            FeeFollowUpService.GetFollowup(model);
            return Json(model.TotalRecords);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.FeeSchdeule, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditFeeFollowUp(long? id, long? ApplicationKey)
        {
            ApplicationFeeFollowupDetailsViewModel model = new ApplicationFeeFollowupDetailsViewModel();

            model.RowKey = id ?? 0;
            model.ApplicationKey = ApplicationKey ?? 0;

            model = FeeFollowUpService.GetFeeFollowupById(model);
           
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult AddEditFeeFollowUp(ApplicationFeeFollowupDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {                
                if (model.RowKey == 0)
                {
                    model = FeeFollowUpService.CreateFeeFollowup(model);
                }
                else
                {
                    model = FeeFollowUpService.UpdateFeeFollowup(model);
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
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList();
            }

            model.Message = EduSuiteUIResources.Failed;

            //foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            //{
            //}
            return PartialView(model);

        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.FeeSchdeule, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteFeeFollowup(long? id)
        {
            ApplicationFeeFollowupDetailsViewModel objViewModel = new ApplicationFeeFollowupDetailsViewModel();

            objViewModel.RowKey = id ?? 0;
            try
            {
                objViewModel = FeeFollowUpService.DeleteFeeFollowup(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }
    }
}