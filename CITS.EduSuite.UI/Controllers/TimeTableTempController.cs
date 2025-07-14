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
    public class TimeTableTempController : BaseController
    {
        // GET: TimeTableTemp

        private ITimeTableTempService TimeTableTempService;
        public TimeTableTempController(ITimeTableTempService objTimeTableTempService)
        {
            this.TimeTableTempService = objTimeTableTempService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.TimeTableMaster, ActionCode = ActionConstants.View)]
        [HttpGet]
        public ActionResult TimeTableTempList()
        {
            TimeTableTempMasterViewModel Model = new TimeTableTempMasterViewModel();
            return View(Model);
        }

        [HttpGet]
        public JsonResult GetTimeTableTempMaster(string searchText)
        {
            int page = 1; int rows = 15;
            List<TimeTableTempMasterViewModel> TimeTableTempList = new List<TimeTableTempMasterViewModel>();
            TimeTableTempMasterViewModel objViewModel = new TimeTableTempMasterViewModel();

            TimeTableTempList = TimeTableTempService.GetTimeTableTempMaster(searchText);
            int pageindex = Convert.ToInt32(page) - 1;
            int pagesize = rows;
            int totalrecords = TimeTableTempList.Count();
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)rows);
            var jsonData = new
            {
                total = totalpage,
                page,
                records = totalrecords,
                rows = TimeTableTempList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.TimeTableMaster, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditTimeTableTemp(short? id)
        {
            TimeTableTempMasterViewModel model = new TimeTableTempMasterViewModel();
            model.RowKey = id ?? 0;
            model = TimeTableTempService.GetTimeTableMasterById(model);

            if (model == null)
            {
                model = new TimeTableTempMasterViewModel();
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult FillTimeTableDetails(TimeTableTempMasterViewModel model)
        {
            TimeTableTempService.FillTimeTableDetails(model);
            return PartialView(model);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.TimeTableMaster, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult AddEditTimeTableTemp(TimeTableTempMasterViewModel model)
        {
            if (ModelState.IsValid)
            {

                if (model.RowKey != 0)
                {
                    model = TimeTableTempService.UpdateTimeTableTemp(model);
                }
                else
                {
                    model = TimeTableTempService.CreateTimeTableTemp(model);
                }




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
            }

            foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            {
            }
            return Json(model);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.AttendanceTypeMaster, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteTimeTableTempMaster(long? id)
        {
            TimeTableTempMasterViewModel model = new TimeTableTempMasterViewModel();
            try
            {
                model = TimeTableTempService.DeleteTimeTableTempMaster(id ?? 0);
            }
            catch (Exception)
            {
                model.Message = EduSuiteUIResources.Failed;
            }
            return Json(model);

        }

        //[ActionAuthenticationAttribute(MenuCode = MenuConstants.TimeTableMaster, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult ViewTimeTableEmployee()
        {
            TimeTableTempMasterViewModel model = new TimeTableTempMasterViewModel();
            //GetUserKey(model);
            model = TimeTableTempService.ViewTimeTableEmployee(model);

            if (model == null)
            {
                model = new TimeTableTempMasterViewModel();
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult FillTimeTableForEmployee(TimeTableTempMasterViewModel model)
        {
            //GetUserKey(model);
            TimeTableTempService.FillTimeTableEmployee(model);
            return PartialView(model);
        }

        //private void GetUserKey(TimeTableTempMasterViewModel model)
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