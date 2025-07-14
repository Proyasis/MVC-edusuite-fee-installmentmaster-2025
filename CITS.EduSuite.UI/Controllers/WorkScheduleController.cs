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

namespace CITS.EduSuite.UI.Controllers
{
    public class WorkScheduleController : BaseController
    {

        public IWorkScheduleService WorkScheduleService;

        public WorkScheduleController(IWorkScheduleService objWorkScheduleService)
        {
            this.WorkScheduleService = objWorkScheduleService;
        }
        public ActionResult WorkScheduleList()
        {
            return View();
        }


        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EmployeeWorkSchedule, ActionCode = ActionConstants.View)]
        [HttpGet]
        public ActionResult AddEditWorkSchedule()
        {
            WorkScheduleViewModel model = new WorkScheduleViewModel();
           
            model = WorkScheduleService.AddEditWorkSchedule(model);
            return View(model);
        }

        [HttpPost]

        public ActionResult GetWorkscheduleSubjectDetails(WorkScheduleViewModel model)
        {
            WorkScheduleService.FIllWorkscheduleSubjectDetails(model);

            if (model.Message != null && model.Message != AppConstants.Common.SUCCESS)
            {
                ModelState.AddModelError("error_msg", model.Message);
            }

            return PartialView(model);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EmployeeWorkSchedule, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult AddEditWorkScheduleDetail(WorkscheduleSubjectmodel model)
        {

            model = WorkScheduleService.AddEditWorkScheduleDetail(model);

            return PartialView("AddEditWorkScheduleDetails", model);
        }
        [HttpPost]
        public ActionResult AddEditWorkScheduleDetails(WorkscheduleSubjectmodel model)
        {
            if (ModelState.IsValid)
            {
                if (model.MasterRowKey == 0)
                {
                    model = WorkScheduleService.CreateWorkSchedule(model);
                }
                else
                {
                    model = WorkScheduleService.UpdateWorkSchedule(model);
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
        public ActionResult GetHistoryWorkSchedule(long? id)
        {
            WorkscheduleSubjectmodel objViewModel = new WorkscheduleSubjectmodel();
            objViewModel.MasterRowKey = id ?? 0;
            var HistoryWorkSchedule = WorkScheduleService.GetHistoryWorkSchedule(objViewModel);
            return PartialView(HistoryWorkSchedule);

        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EmployeeWorkSchedule, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteWorkScheduleDetails(long? Id)
        {
            WorkscheduleSubjectmodel model = new WorkscheduleSubjectmodel();
            model.RowKey = Id ?? 0;
            try
            {
                model = WorkScheduleService.DeleteWorkSchedule(model);
            }
            catch (Exception)
            {
                model.Message = EduSuiteUIResources.Failed;
            }
            return Json(model);
        }

        [HttpGet]
        public JsonResult FillClassDetails(long EmployeeKey)
        {
            WorkScheduleViewModel model = new WorkScheduleViewModel();
            
            model.EmployeeKey = EmployeeKey;
            model = WorkScheduleService.FillClassDetails(model);
            return Json(model.ClassDetails, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FillSubject(long EmployeeKey, long ClassDetailsKey, short BatchKey)
        {
            WorkScheduleViewModel model = new WorkScheduleViewModel();
            
            model.EmployeeKey = EmployeeKey;
            model.ClassDetailsKey = ClassDetailsKey;
            model.BatchKey = BatchKey;
            model = WorkScheduleService.FillSubjects(model);
            return Json(model.Subjects, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult FillBatch(long EmployeeKey, long ClassDetailsKey)
        {
            WorkScheduleViewModel model = new WorkScheduleViewModel();
           
            model.EmployeeKey = EmployeeKey;
            model.ClassDetailsKey = ClassDetailsKey;
            model = WorkScheduleService.FillBatch(model);
            return Json(model.Batches, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult FillTeacher(short BranchKey)
        {
            WorkScheduleViewModel model = new WorkScheduleViewModel();
            //
            model.BranchKey = BranchKey;
            model = WorkScheduleService.FillTeacher(model);
            return Json(model.Employees, JsonRequestBehavior.AllowGet);
        }
      
    }
}