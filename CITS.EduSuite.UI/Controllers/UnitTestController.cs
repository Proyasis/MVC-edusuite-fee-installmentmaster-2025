using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Provider;
using System.Web.Security;
using CITS.EduSuite.Business.Models.Security;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI.Controllers
{
    public class UnitTestController : BaseController
    {
        public IUnitTestScheduleService unitTestScheduleService;

        public UnitTestController(IUnitTestScheduleService objunitTestScheduleService)
        {
            this.unitTestScheduleService = objunitTestScheduleService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UnitTestResult, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult UnitExamSchduleList()
        {
            UnitTestScheduleViewModel model = new UnitTestScheduleViewModel();
            //
            model = unitTestScheduleService.GetSearchDropdownList(model);
            return View(model);
        }

        [HttpPost]
        public JsonResult GetUnitTestSchedule(string searchText, string SearchDate, short? BranchKey, long? ClassDetailsKey, short? BatchKey, string sidx, string sord, int page, int rows)
        {
            long TotalRecords = 0;

            UnitTestScheduleViewModel model = new UnitTestScheduleViewModel();
            List<UnitTestScheduleViewModel> unitTestScheduleList = new List<UnitTestScheduleViewModel>();
            model.searchText = searchText;
            
            model.BranchKey = BranchKey ?? 0;
            model.ClassDetailsKey = ClassDetailsKey ?? 0;
            model.BatchKey = BatchKey ?? 0;
            DateTime? FromDate = new DateTime();
            FromDate = null;
            model.SearchDate = SearchDate != "" ? DateTime.ParseExact(SearchDate, "dd/MM/yyyy", null) : FromDate;
            model.PageIndex = page;
            model.PageSize = rows;
            model.SortBy = sidx;
            model.SortOrder = sord;

            unitTestScheduleList = unitTestScheduleService.GetUnitTestSchedule(model,out TotalRecords);
           
            var totalPages = (int)Math.Ceiling((float)TotalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = TotalRecords,
                rows = unitTestScheduleList
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UnitTestResult, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditUnitTestSchedule(long? Id)
        {

            UnitTestScheduleViewModel model = new UnitTestScheduleViewModel();
            model.RowKey = Id ?? 0;
           // 
            model = unitTestScheduleService.GetUnitTestScheduleById(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult GetUnitTestDetails(UnitTestScheduleViewModel model)
        {
            unitTestScheduleService.FillUnitTestDetailsViewModel(model);

            if (model.Message != null && model.Message != AppConstants.Common.SUCCESS)
            {
                ModelState.AddModelError("error_msg", model.Message);
            }

            return PartialView(model);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UnitTestResult, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult AddEditUnitTestScheduleSubmit(UnitTestScheduleViewModel model)
        {
            if (ModelState.IsValid)
            {
                //
                if (model.RowKey != 0)
                {
                    model = unitTestScheduleService.UpdateUnitTestSchedule(model);
                }
                else
                {
                    model = unitTestScheduleService.CreateUnitTestSchedule(model);
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

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UnitTestResult, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteUnitTest(long? RowKey)
        {
            UnitTestScheduleViewModel model = new UnitTestScheduleViewModel();
            //
            model.RowKey = RowKey ?? 0;
            try
            {
                model = unitTestScheduleService.DeleteUnitTest(model);
            }
            catch (Exception)
            {
                model.Message = EduSuiteUIResources.Failed;
            }
            return Json(model);
        }
        //private void GetUserKey(UnitTestScheduleViewModel model)
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

        #region DropDownChange Events
        [HttpGet]
        public JsonResult FillCourseType(short BranchKey)
        {
            UnitTestScheduleViewModel model = new UnitTestScheduleViewModel();
            //
            model.BranchKey = BranchKey;
            model = unitTestScheduleService.FillCourseType(model);
            return Json(model.CourseTypes, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult FillSearchClassDetails(short Key)
        {
            UnitTestScheduleViewModel model = new UnitTestScheduleViewModel();
            //
            model.BranchKey = Key;
            model = unitTestScheduleService.FillSearchClassDetails(model);
            return Json(model.ClassDetails, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FillSearchBatch(short? BranchKey, long? ClassDetailsKey)
        {
            UnitTestScheduleViewModel model = new UnitTestScheduleViewModel();
            //
            model.BranchKey = BranchKey ?? 0;
            model.ClassDetailsKey = ClassDetailsKey ?? 0;
            model = unitTestScheduleService.FillSearchBatch(model);
            return Json(model.Batches, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FillClassDetails(short Key, short? Branchkey)
        {
            UnitTestScheduleViewModel model = new UnitTestScheduleViewModel();
            //
            model.CourseTypeKey = Key;
            model.BranchKey = Branchkey ?? 0;
            model = unitTestScheduleService.FillClassDetails(model);
            return Json(model.ClassDetails, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FillSubject(long ClassDetailsKey,short BatchKey)
        {
            UnitTestScheduleViewModel model = new UnitTestScheduleViewModel();
            //
            model.ClassDetailsKey = ClassDetailsKey;
            model.BatchKey = BatchKey;
            model = unitTestScheduleService.FillSubjects(model);
            return Json(model.Subjects, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult FillBatch(short BranchKey, long ClassDetailsKey)
        {
            UnitTestScheduleViewModel model = new UnitTestScheduleViewModel();
            //
            model.BranchKey = BranchKey;
            model.ClassDetailsKey = ClassDetailsKey;
            model = unitTestScheduleService.FillBatch(model);
            return Json(model.Batches, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult FillSubjectModule(long SubjectKey)
        {
            UnitTestScheduleViewModel model = new UnitTestScheduleViewModel();
            //
            model.SubjectKey = SubjectKey;
            model = unitTestScheduleService.FillSubjectModules(model);
            return Json(model.SubjectModules, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult FillModuleTopics(long SubjectModuleKey)
        {
            UnitTestScheduleViewModel model = new UnitTestScheduleViewModel();
            model.SubjectModuleKey = SubjectModuleKey;
            model = unitTestScheduleService.FillModuleTopics(model);
            return Json(model.ModuleTopics, JsonRequestBehavior.AllowGet);
        }
        #endregion DropDownChange Events
    }
}