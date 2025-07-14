using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.UI.Controllers
{
    public class StudentTimeTableController : BaseController
    {
        // GET: StudentTimeTable
        private IStudentTimeTableService studentTimeTableService;
        public StudentTimeTableController(IStudentTimeTableService objstudentTimeTableService)
        {
            this.studentTimeTableService = objstudentTimeTableService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentTimeTable, ActionCode = ActionConstants.View)]
        [HttpGet]
        public ActionResult StudentTimeTableList()
        {
            StudentTimeTableViewModel model = new StudentTimeTableViewModel();
            studentTimeTableService.FillDropDown(model);
            return View(model);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentTimeTable, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult AddEditStudentTimeTable(List<StudentTimeTableViewModel> modelList)
        {
            StudentTimeTableViewModel model = new StudentTimeTableViewModel();

            model = studentTimeTableService.UpdateStudentTimeTableList(modelList);

            if (model.Message != AppConstants.Common.SUCCESS)
            {
                ModelState.AddModelError("error_msg", model.Message);
            }
            else
            {
                //GeneratePdfPayslip(employeeSalaryModel.SalaryMasterKeyList);
                return Json(model);
            }

            //model.Message = "";
            return Json(model);
            //return View("StudentTimeTableList",model);
        }
        [HttpGet]
        public ActionResult GetStudentTimeTable(byte DayKey,short? BranchKey)
        {
            List<StudentTimeTableViewModel> timeTableList = new List<StudentTimeTableViewModel>();
            StudentTimeTableViewModel timeTableViewModel = new StudentTimeTableViewModel();
            timeTableViewModel.Day = DayKey;
            timeTableViewModel.BranchKey = BranchKey;
            timeTableList = studentTimeTableService.GetTimeTable(timeTableViewModel);
            studentTimeTableService.FillDropDown(timeTableViewModel);
            ViewBag.Periods = timeTableViewModel.WeeklyPeriods;
            ViewBag.Classes = timeTableViewModel.ClassDetails;
            return PartialView(timeTableList);
        }
    }
}