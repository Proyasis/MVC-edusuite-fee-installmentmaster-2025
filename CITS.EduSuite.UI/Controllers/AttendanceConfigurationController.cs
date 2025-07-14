using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.UI.Controllers
{
    public class AttendanceConfigurationController : BaseController
    {
       private IAttendanceConfigurationService AttendanceConfigurationService;

        public AttendanceConfigurationController(IAttendanceConfigurationService objAttendanceConfigurationService)
        {
            this.AttendanceConfigurationService = objAttendanceConfigurationService;
        }

        [HttpGet]
        public ActionResult AttendanceConfigurationList()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetAttendanceConfigurations()
        {
            int page = 1, rows = 10;

            List<AttendanceConfigurationViewModel> attendanceConfigurationList = new List<AttendanceConfigurationViewModel>();
            attendanceConfigurationList = AttendanceConfigurationService.GetAttendanceConfigurations();

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = attendanceConfigurationList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = attendanceConfigurationList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        public ActionResult AddEditAttendanceConfiguration(int? id)
        {
            AttendanceConfigurationViewModel model = new AttendanceConfigurationViewModel();
            model.RowKey = id ?? 0;
            var AttendanceConfiguration = AttendanceConfigurationService.GetAttendanceConfigurationById(model);
            if (AttendanceConfiguration == null)
            {
                AttendanceConfiguration = new AttendanceConfigurationViewModel();
            }
            return PartialView(AttendanceConfiguration);
        }

        [HttpPost]
        public ActionResult AddEditAttendanceConfiguration(AttendanceConfigurationViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = AttendanceConfigurationService.CreateAttendanceConfiguration(model);
                }
                else
                {
                    model = AttendanceConfigurationService.UpdateAttendanceConfiguration(model);
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

            model.Message =  EduSuiteUIResources.Failed;  
            return PartialView(model);

        }

        [HttpPost]
        public ActionResult DeleteAttendanceConfiguration(Int32 id)
        {
            AttendanceConfigurationViewModel objViewModel = new AttendanceConfigurationViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = AttendanceConfigurationService.DeleteAttendanceConfiguration(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message =  EduSuiteUIResources.Failed;  
            }
            return Json(objViewModel);




        }
    }
}