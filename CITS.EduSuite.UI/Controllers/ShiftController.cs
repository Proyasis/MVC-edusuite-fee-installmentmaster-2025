using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CITS.EduSuite.UI.Controllers
{
    public class ShiftController : BaseController
    {

        private IShiftService ShiftService;
        public ShiftController(IShiftService objShiftService)
        {
            this.ShiftService = objShiftService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Shift, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult ShiftList()
        {
            ShiftViewModel shift = new ShiftViewModel();
            return View(shift);
        }

        [HttpGet]
        public JsonResult GetShift(string searchText)
        {
            int page = 1, rows = 10;

            List<ShiftViewModel> shiftList = new List<ShiftViewModel>();
            shiftList = ShiftService.GetShift(searchText);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = shiftList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = shiftList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Shift, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditShift(int? id)
        {
            var shift = ShiftService.GetShiftById(id);
            if (shift == null)
            {
                shift = new ShiftViewModel();
            }
            return PartialView(shift);
        }

        [HttpPost]
        public ActionResult AddEditShift(ShiftViewModel shift)
        {

            if (ModelState.IsValid)
            {
                if (shift.MasterRowKey == 0)
                {
                    shift = ShiftService.CreateShift(shift);
                }
                else
                {
                    shift = ShiftService.UpdateShift(shift);
                }

                if (shift.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", shift.Message);
                }
                else
                {
                    return Json(shift);
                }

                shift.Message = "";
                return PartialView(shift);
            }

            shift.Message =  EduSuiteUIResources.Failed;  
            return PartialView(shift);

        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Shift, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteShift(int id)
        {
            ShiftViewModel objViewModel = new ShiftViewModel();

            objViewModel.MasterRowKey = id;
            try
            {
                objViewModel = ShiftService.DeleteShift(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message =  EduSuiteUIResources.Failed;  
            }
            return Json(objViewModel);

        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Shift, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteShiftBreak(int id)
        {
            ShiftViewModel objViewModel = new ShiftViewModel();
            ShiftBreakModel model = new ShiftBreakModel();

            model.RowKey = id;
            try
            {
                objViewModel = ShiftService.DeleteShiftBreak(model);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);

        }

        [HttpGet]
        public JsonResult CheckShiftCodeExists(string shiftCode, int? RowKey)
        {
            return Json(ShiftService.CheckshiftCodeExists(shiftCode, RowKey ?? 0).IsSuccessful, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult CheckShiftBreakExists(string BreakName, long? RowKey)
        {
            return Json(true, JsonRequestBehavior.AllowGet);
        }

    }
}


