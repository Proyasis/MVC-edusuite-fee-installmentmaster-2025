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
    public class AttendanceTypeController : BaseController
    {
        private IAttendanceTypeService AttendanceTypeService;
        public AttendanceTypeController(IAttendanceTypeService objAttendanceTypeService)
        {
            this.AttendanceTypeService = objAttendanceTypeService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.AttendanceType, ActionCode = ActionConstants.View)]
        [HttpGet]
        public ActionResult AttendanceTypeList()
        {
            AttendanceTypeViewModel Model = new AttendanceTypeViewModel();
            return View(Model);
        }

        [HttpGet]
        public JsonResult GetAttendanceTypeDetails(string searchText)
        {
            int page = 1; int rows = 15;
            List<AttendanceTypeViewModel> AttendanceTypeList = new List<AttendanceTypeViewModel>();
            AttendanceTypeViewModel objViewModel = new AttendanceTypeViewModel();
            objViewModel.AttendanceTypeName = searchText;
            AttendanceTypeList = AttendanceTypeService.GetAttendanceType(searchText);
            int pageindex = Convert.ToInt32(page) - 1;
            int pagesize = rows;
            int totalrecords = AttendanceTypeList.Count();
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)rows);
            var jsonData = new
            {
                total = totalpage,
                page,
                records = totalrecords,
                rows = AttendanceTypeList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.AttendanceType, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditAttendanceType(short? id)
        {
            AttendanceTypeViewModel model = new AttendanceTypeViewModel();
            model.RowKey = id ?? 0;
            model = AttendanceTypeService.GetAttendaneTypeById(id);
            //VerificationStatusService.FillDesignation(model.VerificationList);

            if (model == null)
            {
                model = new AttendanceTypeViewModel();
            }
            return PartialView(model);
        }

        
        [HttpPost]
        public ActionResult AddEditAttendanceType(AttendanceTypeViewModel model)
        {
            if (ModelState.IsValid)
            {

                if (model.RowKey != 0)
                {
                    model = AttendanceTypeService.UpdateAttendanceType(model);
                }
                else
                {
                    model = AttendanceTypeService.CreateAttendanceType(model);
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

         [ActionAuthenticationAttribute(MenuCode = MenuConstants.AttendanceType, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteAttendanceTypeAll(short? id)
        {
            AttendanceTypeViewModel model = new AttendanceTypeViewModel();
            try
            {
                model = AttendanceTypeService.DeleteAttendanceTypeAll(id ?? 0);
            }
            catch (Exception)
            {
                model.Message = EduSuiteUIResources.Failed;
            }
            return Json(model);

        }

    }
}