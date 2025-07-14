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
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI.Controllers
{
    public class DepartmentShiftController : BaseController
    {
        private IDepartmentShiftService DepartmentShiftService;
        public DepartmentShiftController(IDepartmentShiftService objDepartmentShiftService)
        {
            this.DepartmentShiftService = objDepartmentShiftService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.DepartmentShift, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult DepartmentShiftList()
        {
            return View();
        }

        [HttpGet]

        public JsonResult GetDepartmentShifts(string searchText)
        {
            int page = 1, rows = 10;

            List<DepartmentShiftViewModel> DepartmentShiftList = new List<DepartmentShiftViewModel>();
            DepartmentShiftList = DepartmentShiftService.GetDepartmentShifts(searchText);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = DepartmentShiftList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = DepartmentShiftList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        //[ActionAuthenticationAttribute(MenuCode = MenuConstants.DepartmentShift, ActionCode = ActionConstants.AddEdit)]
        public ActionResult AddEditDepartmentShift(int? id)
        {
            var Departmentshift = DepartmentShiftService.GetDepartmentShiftById(id ?? 0);
            if (Departmentshift == null)
            {
                Departmentshift = new DepartmentShiftViewModel();
            }
            return PartialView(Departmentshift);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.DepartmentShift, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult AddEditDepartmentShift(DepartmentShiftViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = DepartmentShiftService.CreateDepartmentShift(model);
                    //if (model.ExceptionMessage != null && model.ExceptionMessage != "")
                    //{
                    //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstants.DepartmentShift, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, model.ExceptionMessage);
                    //}
                    //else
                    //{
                    //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstants.DepartmentShift, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);
                    //}
                }
                else
                {
                    model = DepartmentShiftService.UpdateDepartmentShift(model);
                    //if (model.ExceptionMessage != null && model.ExceptionMessage != "")
                    //{
                    //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstants.DepartmentShift, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, model.ExceptionMessage);
                    //}
                    //else
                    //{
                    //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstants.DepartmentShift, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                    //}
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

            model.Message = EduSuiteUIResources.Failed;
            return PartialView(model);

        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.DepartmentShift, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteDepartmentShift(Int32 id)
        {
            DepartmentShiftViewModel objViewModel = new DepartmentShiftViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = DepartmentShiftService.DeleteDepartmentShift(objViewModel);
                //if (objViewModel.ExceptionMessage != null && objViewModel.ExceptionMessage != "")
                //{
                //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstants.DepartmentShift, ActionConstants.Delete, DbConstants.LogType.Error, id, objViewModel.ExceptionMessage);
                //}
                //else
                //{
                //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstants.DepartmentShift, ActionConstants.Delete, DbConstants.LogType.Info, id, objViewModel.Message);
                //}
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);

        }
        //private int GetUserKey()
        //{
        //    CookieProvider cookieProvider = new CookieProvider();
        //    HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
        //    if (authCookie != null)
        //    {
        //        EduSuitePrincipalData userData = cookieProvider.GetCookie(authCookie);
        //        return userData.UserKey;
        //    }
        //    return 0;
        //}

    }
}