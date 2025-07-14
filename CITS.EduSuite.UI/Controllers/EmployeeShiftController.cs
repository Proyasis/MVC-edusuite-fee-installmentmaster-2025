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
    public class EmployeeShiftController : BaseController
    {
        IEmployeeShiftService EmployeeShiftService;
        public EmployeeShiftController(IEmployeeShiftService objEmployeeShiftService)
        {
            this.EmployeeShiftService = objEmployeeShiftService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EmployeeShift, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult EmployeeShiftList()
        {
            return View();
        }

        [HttpGet]

        public JsonResult GetEmployeeShifts(string searchText)
        {
            int page = 1, rows = 10;

            List<EmployeeShiftViewModel> EmployeeShiftList = new List<EmployeeShiftViewModel>();
            EmployeeShiftList = EmployeeShiftService.GetEmployeeShifts(searchText);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = EmployeeShiftList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = EmployeeShiftList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EmployeeShift, ActionCode = ActionConstants.AddEdit)]
        public ActionResult AddEditEmployeeShift(int? id)
        {
            var Employeeshift = EmployeeShiftService.GetEmployeeShiftById(id ?? 0);
            if (Employeeshift == null)
            {
                Employeeshift = new EmployeeShiftViewModel();
            }
            return PartialView(Employeeshift);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EmployeeShift, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult AddEditEmployeeShift(EmployeeShiftViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = EmployeeShiftService.CreateEmployeeShift(model);
                    //if (model.ExceptionMessage != null && model.ExceptionMessage != "")
                    //{
                    //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstants.EmployeeShift, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, model.ExceptionMessage);
                    //}
                    //else
                    //{
                    //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstants.EmployeeShift, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);
                    //}
                }
                else
                {
                    model = EmployeeShiftService.UpdateEmployeeShift(model);
                    //if (model.ExceptionMessage != null && model.ExceptionMessage != "")
                    //{
                    //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstants.EmployeeShift, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, model.ExceptionMessage);
                    //}
                    //else
                    //{
                    //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstants.EmployeeShift, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
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
                return Json(model);
            }
            foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            {
            }


            model.Message =  EduSuiteUIResources.Failed;  
            return PartialView(model);

        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EmployeeShift, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteEmployeeShift(Int32 id)
        {
            EmployeeShiftViewModel objViewModel = new EmployeeShiftViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = EmployeeShiftService.DeleteEmployeeShift(objViewModel);
                //if (objViewModel.ExceptionMessage != null && objViewModel.ExceptionMessage != "")
                //{
                //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstants.EmployeeShift, ActionConstants.Delete, DbConstants.LogType.Error, id, objViewModel.ExceptionMessage);
                //}
                //else
                //{
                //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstants.EmployeeShift, ActionConstants.Delete, DbConstants.LogType.Info, id, objViewModel.Message);
                //}
            }
            catch (Exception)
            {
                objViewModel.Message =  EduSuiteUIResources.Failed;  
            }
            return Json(objViewModel);

        }
        [HttpPost]
        public ActionResult DeleteEmployeeShiftDetails(long id)
        {
            EmployeeShiftViewModel objViewModel = new EmployeeShiftViewModel();

            try
            {
                objViewModel = EmployeeShiftService.DeleteEmployeeShiftDetails(id);
                //if (objViewModel.ExceptionMessage != null && objViewModel.ExceptionMessage != "")
                //{
                //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstants.EmployeeShift, ActionConstants.Delete, DbConstants.LogType.Error, id, objViewModel.ExceptionMessage);
                //}
                //else
                //{
                //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstants.EmployeeShift, ActionConstants.Delete, DbConstants.LogType.Info, id, objViewModel.Message);
                //}
            }
            catch (Exception)
            {
                objViewModel.Message =  EduSuiteUIResources.Failed;  
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