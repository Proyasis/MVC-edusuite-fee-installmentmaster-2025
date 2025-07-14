using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Provider;
using System.Web.Security;
using CITS.EduSuite.Business.Models.Security;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI.Controllers
{
    public class HolidayController : BaseController
    {
        // GET: HoliDay
        private IHolidayService holidayService;

        public HolidayController(IHolidayService objHolidayService)
        {
            this.holidayService = objHolidayService;
        }

        [HttpGet]
        public ActionResult HolidayList()
        {
            HolidayViewModel objViewModel = new HolidayViewModel();
            objViewModel.HolidayMonthKey = (byte)DateTimeUTC.Now.Month;
            objViewModel.HolidayYearKey = (short)DateTimeUTC.Now.Year;
            //GetUserKey(objViewModel);
            holidayService.FillBranches(objViewModel);
            return View(objViewModel);
        }

        [HttpGet]
        public JsonResult GetHolidays(short? BranchKey, DateTime? start, DateTime? end)
        {

            List<HolidayViewModel> holidaysList = new List<HolidayViewModel>();
            HolidayViewModel holidayViewModel = new HolidayViewModel();
            holidayViewModel.HolidayFrom = start;
            holidayViewModel.HolidayTo = end;
            holidayViewModel.BranchKey = BranchKey ?? 0;
            holidaysList = holidayService.GetHolidays(holidayViewModel);

            var jsonData = holidaysList.Select(row => new
            {
                eventID = row.RowKey,
                title = row.HolidayTitle,
                description = row.Remarks,
                start = Convert.ToDateTime(row.HolidayFrom).ToString("yyyy-MM-dd hh:mm:ss"),
                end = Convert.ToDateTime(row.HolidayTo).ToString("yyyy-MM-dd hh:mm:ss"),
                color = (row.IsDayOff ? "green" : "red")
            });
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetHolidayByDate(DateTime Date, short? BranchKey)
        {
            HolidayViewModel holidayViewModel = new HolidayViewModel();
            holidayViewModel.HolidayFrom = Date;
            holidayViewModel.BranchKey = BranchKey ?? 0;
            holidayViewModel = holidayService.GetHolidayByDate(holidayViewModel);
            if (holidayViewModel.HolidayFrom == null)
            {
                holidayViewModel.HolidayFrom = Date;
                holidayViewModel.HolidayTo = Date;
            }
            var jsonData = new
            {
                eventID = holidayViewModel.RowKey,
                title = holidayViewModel.HolidayTitle,
                description = holidayViewModel.Remarks,
                start = Convert.ToDateTime(holidayViewModel.HolidayFrom).ToString("yyyy-MM-dd hh:mm:ss"),
                end = Convert.ToDateTime(holidayViewModel.HolidayTo).ToString("yyyy-MM-dd hh:mm:ss"),
                color = "red"
            };
            return Json(jsonData);
        }


        [HttpGet]
        public ActionResult AddEditHoliday(long? id, DateTime? Date, short? branchKey)
        {
            HolidayViewModel Holiday = new HolidayViewModel();
            Holiday.BranchKey = branchKey ?? 0;
            Holiday.RowKey = id ?? 0;
            Holiday.HolidayFrom = Date;
            Holiday.HolidayTo = Date;
            Holiday = holidayService.GetHolidayById(Holiday);
            if (Holiday == null)
            {
                Holiday = new HolidayViewModel();
            }
            return PartialView(Holiday);
        }

        [HttpPost]
        public ActionResult AddEditHoliday(HolidayViewModel holiday)
        {

            if (ModelState.IsValid)
            {
                if (holiday.RowKey == 0)
                {
                    holiday = holidayService.CreateHoliday(holiday);
                }
                else
                {
                    holiday = holidayService.UpdateHoliday(holiday);
                }

                if (holiday.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", holiday.Message);
                }
                else
                {
                    return Json(holiday);
                }

                holiday.Message = "";
                return PartialView(holiday);
            }

            holiday.Message = EduSuiteUIResources.Failed;
            return PartialView(holiday);

        }

        [HttpPost]
        public ActionResult DeleteHoliday(Int64 id)
        {
            HolidayViewModel objViewModel = new HolidayViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = holidayService.DeleteHoliday(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }

        //private void GetUserKey(HolidayViewModel model)
        //{

        //    CookieProvider cookieProvider = new CookieProvider();
        //    HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
        //    if (authCookie != null)
        //    {
        //        CITSEduSuitePrincipalData userData = cookieProvider.GetCookie(authCookie);
        //        model.UserKey = userData.UserKey;
        //        model.RoleKey = userData.RoleKey;
        //    }
        //}

    }
}