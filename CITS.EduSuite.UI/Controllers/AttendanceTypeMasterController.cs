using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI.Controllers
{
    public class AttendanceTypeMasterController : BaseController
    {
        // GET: AttendanceTypeMaster
        private IAttendanceTypeMasterService AttendanceTypeMasterService;
        public AttendanceTypeMasterController(IAttendanceTypeMasterService objAttendanceTypeMasterService)
        {
            this.AttendanceTypeMasterService = objAttendanceTypeMasterService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.AttendanceTypeMaster, ActionCode = ActionConstants.View)]
        [HttpGet]
        public ActionResult AttendanceTypeMasterList()
        {
            AttendanceTypeMasterViewModel Model = new AttendanceTypeMasterViewModel();
            return View(Model);
        }

        [HttpGet]
        public JsonResult GetAttendanceTypeMasterDetails(string searchText)
        {
            int page = 1; int rows = 15;
            List<AttendanceTypeMasterViewModel> AttendanceTypeMasterList = new List<AttendanceTypeMasterViewModel>();
            AttendanceTypeMasterViewModel objViewModel = new AttendanceTypeMasterViewModel();
            
            AttendanceTypeMasterList = AttendanceTypeMasterService.GetAttendanceTypeMaster(searchText);
            int pageindex = Convert.ToInt32(page) - 1;
            int pagesize = rows;
            int totalrecords = AttendanceTypeMasterList.Count();
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)rows);
            var jsonData = new
            {
                total = totalpage,
                page,
                records = totalrecords,
                rows = AttendanceTypeMasterList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.AttendanceTypeMaster, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditAttendanceTypeMaster(short? id)
        {
            AttendanceTypeMasterViewModel model = new AttendanceTypeMasterViewModel();
            model.RowKey = id ?? 0;
            model = AttendanceTypeMasterService.GetAttendaneTypeMasterById(id);
            //VerificationStatusService.FillDesignation(model.VerificationList);

            if (model == null)
            {
                model = new AttendanceTypeMasterViewModel();
            }
            return PartialView(model);
        }


        [HttpPost]
        public ActionResult AddEditAttendanceTypeMaster(AttendanceTypeMasterViewModel model)
        {
            if (ModelState.IsValid)
            {

                if (model.RowKey != 0)
                {
                    model = AttendanceTypeMasterService.UpdateAttendanceTypeMaster(model);
                }
                else
                {
                    model = AttendanceTypeMasterService.CreateAttendanceTypeMaster(model);
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
            foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            {
            }

            model.Message = EduSuiteUIResources.Failed;
            return PartialView(model);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.AttendanceTypeMaster, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteAttendanceTypeMaster(short? id)
        {
            AttendanceTypeMasterViewModel model = new AttendanceTypeMasterViewModel();
            try
            {
                model = AttendanceTypeMasterService.DeleteAttendanceTypeMaster(id ?? 0);
            }
            catch (Exception)
            {
                model.Message = EduSuiteUIResources.Failed;
            }
            return Json(model);

        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.AttendanceTypeMaster, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteAttendanceTypeDetails(short? id)
        {
            AttendanceTypeMasterViewModel model = new AttendanceTypeMasterViewModel();
            try
            {
                model = AttendanceTypeMasterService.DeleteAttendanceTypeDetails(id ?? 0);
            }
            catch (Exception)
            {
                model.Message = EduSuiteUIResources.Failed;
            }
            return Json(model);

        }

        [HttpGet]
        public JsonResult CheckAttendanceTypeExists(short? AttendanceTypeKey)
        {
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}