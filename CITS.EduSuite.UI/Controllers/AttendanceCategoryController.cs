using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.UI;
using CITS.EduSuite.UI.Controllers;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Models.Security;
using CITS.EduSuite.Business.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI.Controllers
{
    public class AttendanceCategoryController : BaseController
    {
        private IAttendanceCategoryService AttendanceCategoryService;
        public AttendanceCategoryController(IAttendanceCategoryService objAttendanceCategoryService)
        {
            this.AttendanceCategoryService = objAttendanceCategoryService;
        }
        //[ActionAuthenticationAttribute(MenuCode = MenuConstants.AttendanceCategory, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult AttendanceCategoryList()
        {
            return View();
        }

        //[ActionAuthenticationAttribute(MenuCode = MenuConstant.AttendanceCategory, ActionCode = ActionConstant.AddEdit)]
        [HttpGet]
        public ActionResult AddEditAttendanceCategory(int? id)
        {
            var AttendanceCategory = AttendanceCategoryService.GeAttendanceCategoryById(id);
            if (AttendanceCategory == null)
            {
                AttendanceCategory = new AttendanceCategoryViewModel();
            }

            return PartialView(AttendanceCategory);
        }
        [HttpPost]
        public ActionResult AddEditAttendanceCategory(AttendanceCategoryViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.MasterRowKey == 0)
                {
                    model = AttendanceCategoryService.CreateAttendanceCategory(model);

                }
                else
                {
                    model = AttendanceCategoryService.UpdateAttendanceCategory(model);

                }
                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    model.Message = "";
                    return Json(model);
                }
                return PartialView(model);
            }
            foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            {
            }
            model.Message = EduSuiteUIResources.Failed;
            return PartialView(model);
        }

        public JsonResult GetAttendanceCategory(string searchText)
        {
            int page = 1, rows = 15;
            List<AttendanceCategoryViewModel> AttendanceCategoryList = new List<AttendanceCategoryViewModel>();
            AttendanceCategoryList = AttendanceCategoryService.GetAttendanceCategory(searchText);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = AttendanceCategoryList.Count();
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);



            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = AttendanceCategoryList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        //[ActionAuthenticationAttribute(MenuCode = MenuConstant.AttendanceCategory, ActionCode = ActionConstant.Delete)]
        [HttpPost]
        public ActionResult DeleteAttendanceCategory(int? id)
        {
            AttendanceCategoryViewModel model = new AttendanceCategoryViewModel();
            model.MasterRowKey = (byte)(id);
            try
            {
                model = AttendanceCategoryService.DeleteAttendanceCategory(model);

            }
            catch (Exception ex)
            {
                model.Message = EduSuiteUIResources.Failed;
            }
            return Json(model);
        }

        [HttpPost]
        public ActionResult DeleteAttendanceCategoryWeekOff(int? id)
        {
            AttendanceCategoryViewModel model = new AttendanceCategoryViewModel();

            try
            {
                model = AttendanceCategoryService.DeleteAttendanceCategoryWeekOff(id ?? 0);

            }
            catch (Exception ex)
            {
                model.Message = EduSuiteUIResources.Failed;
            }
            return Json(model);
        }

        [HttpGet]
        public JsonResult CheckAttendanceCategoryCodeExists(string AttendanceCategoryCode, byte? MasterRowKey)
        {
            return Json(AttendanceCategoryService.CheckAttendanceCategoryCodeExists(AttendanceCategoryCode, MasterRowKey ?? 0).IsSuccessful, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult CheckWeekOffDayExists(string WeekOffDayKey, byte? RowKey)
        {
            return Json(true, JsonRequestBehavior.AllowGet);
        }


    }
}