using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CITS.EduSuite.UI.Controllers
{
    public class TeacherPortalController : BaseController
    {
        public ISelectListService selectListService;
        public IAttendanceService attendanceService;
        public IWorkScheduleService workScheduleService;
        public IUnitTestScheduleService unitTestScheduleService;
        public IApplicationService applicationService;
        //public IParentMeetScheduleService parentMeetScheduleService;
        public IEmployeeService employeeService;
        public TeacherPortalController(
            ISelectListService objSelectListService,
            IAttendanceService objAttendanceService,
            IWorkScheduleService objWorkScheduleService,
            IUnitTestScheduleService objUnitTestScheduleService,
            IApplicationService objApplicationService,
            //IParentMeetScheduleService objParentMeetScheduleService,
            IEmployeeService objemployeeService
            )
        {
            this.selectListService = objSelectListService;
            this.attendanceService = objAttendanceService;
            this.workScheduleService = objWorkScheduleService;
            this.unitTestScheduleService = objUnitTestScheduleService;
            this.applicationService = objApplicationService;
            // this.parentMeetScheduleService = objParentMeetScheduleService;
            this.employeeService = objemployeeService;
        }
        public ActionResult Index()
        {
            ViewBag.Branches = selectListService.FillBranches();
            return View();
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentAttendance, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult Attendance(long? Id)
        {
            AttendanceViewModel model = new AttendanceViewModel();

            model.RowKey = Id ?? 0;
            model = attendanceService.GetAttendanceById(model);
            return PartialView("~/Views/Attendance/AddEditAttendance.cshtml", model);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UnitTestResult, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult UnitTest(long? Id)
        {
            UnitTestScheduleViewModel model = new UnitTestScheduleViewModel();
            model.RowKey = Id ?? 0;
            model = unitTestScheduleService.GetUnitTestScheduleById(model);
            return PartialView("~/Views/UnitTest/AddEditUnitTestSchedule.cshtml", model);

        }

        [HttpGet]
        public JsonResult GetStudents(short? BranchKey, long? ClassDetailsKey)
        {
            return Json(selectListService.FillStudentsByClass(BranchKey ?? 0, ClassDetailsKey ?? 0), JsonRequestBehavior.AllowGet);

        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Application, ActionCode = ActionConstants.View)]
        [HttpGet]
        public ActionResult ViewStudent(long? id)
        {
            ApplicationViewModel objViewModel = new ApplicationViewModel();
            objViewModel = applicationService.GetApplicationDetailsById(id ?? 0);
            return PartialView("~/Views/Application/ViewApplication.cshtml", objViewModel);

        }

        // [ActionAuthenticationAttribute(MenuCode = MenuConstants.ParentsMeetScheduleDetails, ActionCode = ActionConstants.View)]
        //[HttpGet]
        //public ActionResult ViewProgressCard(long? id)
        //{
        //    ParentsMeetDetailsViewModel objViewModel = new ParentsMeetDetailsViewModel();
        //    objViewModel.ApplicationKey = id ?? 0;
        //    objViewModel = parentMeetScheduleService.ViewProgressCard(objViewModel);
        //    return Json(objViewModel.ProgressCardData, JsonRequestBehavior.AllowGet);

        //}


        //[HttpGet]
        //public JsonResult GetParentsMeetSchedule(short? BranchKey, long? ClassDetailsKey)
        //{
        //    return Json(parentMeetScheduleService.FillParentsMeetSchedules(ClassDetailsKey ?? 0), JsonRequestBehavior.AllowGet);

        //}

        [HttpGet]
        public JsonResult GetStudentsForParentsMeet(short? BranchKey, long? RowKey, long? ClassDetailsKey)
        {
            return Json(selectListService.FillStudentsByClassForParentsMeet(RowKey ?? 0, ClassDetailsKey ?? 0), JsonRequestBehavior.AllowGet);

        }
        //public ActionResult EventsScheduledList()
        //{
        //    EventDetailsViewModel objViewModel = new EventDetailsViewModel();
        //    objViewModel.EventMonthKey = (byte)DateTimeUTC.Now.Month;
        //    objViewModel.EventYearKey = (short)DateTimeUTC.Now.Year;

        //    return View(objViewModel);
        //}

        //[HttpGet]
        //public JsonResult GetEventDetails(short? BranchId, DateTime? start, DateTime? end)
        //{

        //    List<EventDetailsViewModel> EventDetailssList = new List<EventDetailsViewModel>();
        //    EventDetailsViewModel EventDetailsViewModel = new EventDetailsViewModel();
        //    EventDetailsViewModel.EventFrom = start;
        //    EventDetailsViewModel.EventTo = end;
        //    //EventDetailsViewModel. = BranchId ?? 0;
        //    EventDetailssList = employeeService.GetEmployeeEventDetails(EventDetailsViewModel);

        //    var jsonData = EventDetailssList.Select(row => new
        //    {
        //        eventID = row.RowKey,
        //        title = row.EventTitle,
        //        description = row.Remarks,
        //        start = Convert.ToDateTime(row.EventFrom).ToString("yyyy-MM-dd hh:mm:ss"),
        //        end = Convert.ToDateTime(row.EventTo).ToString("yyyy-MM-dd hh:mm:ss"),
        //        color = (row.IsDayOff ? "green" : "red")
        //    });
        //    return Json(jsonData, JsonRequestBehavior.AllowGet);
        //}


        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EmployeeWorkSchedule, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult WorkUpdates(long? Id)
        {
            WorkScheduleViewModel model = new WorkScheduleViewModel();

            model = workScheduleService.AddEditWorkSchedule(model);
            return PartialView("~/Views/WorkSchedule/AddEditWorkSchedule.cshtml", model);

        }

    }
}