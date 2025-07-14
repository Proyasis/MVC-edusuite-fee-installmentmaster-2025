using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Interfaces;

namespace CITS.EduSuite.UI.Controllers
{
    public class HomeController : BaseController
    {
        private ISelectListService selectListService;
        private IDashBoardService dashBoardService;
        private IEmployeeUserPermissionService dashBoardPermissionService;

        public HomeController(ISelectListService objSelectListService, IDashBoardService objDashBoardService, IEmployeeUserPermissionService objdashBoardPermissionService)
        {
            this.selectListService = objSelectListService;
            this.dashBoardService = objDashBoardService;
            this.dashBoardPermissionService = objdashBoardPermissionService;
        }
        public ActionResult DashBoard()
        {
            DashBoardViewModel model = new DashBoardViewModel();
            model.Branches = selectListService.FillBranches();
            model.AppUsers = selectListService.FillAppUserById(DbConstants.User.BranchKey ?? 0);
            model.DashBoardTypes = selectListService.FillDashBoardTypes();

            if (DbConstants.User.EmployeeKey != null && DbConstants.User.EmployeeKey != 0)
            {
                model.EmployeeKey = DbConstants.User.EmployeeKey;
            }
            return View(model);
        }

        #region Enquiry DashBoard
        public JsonResult EnquiryCounts(DashBoardViewModel model)
        {
            List<dynamic> EnquiryCounts = new List<dynamic>();
            bool allowed = checkPermission(DbConstants.DashBoardContent.EnquiryCounts);
            if (allowed)
            {
                EnquiryCounts = dashBoardService.EnquiryCommon(model);
            }

            return Json(EnquiryCounts, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LeadSource(DashBoardViewModel model)
        {
            List<dynamic> LeadSource = new List<dynamic>();

            bool allowed = checkPermission(DbConstants.DashBoardContent.LeadSource);
            if (allowed)
            {

                LeadSource = dashBoardService.EnquiryCommon(model);
            }

            return Json(LeadSource, JsonRequestBehavior.AllowGet);
        }
        public JsonResult EnquiryRecentCalls(DashBoardViewModel model)
        {
            List<dynamic> EnquiryRecentCalls = new List<dynamic>();
            bool allowed = checkPermission(DbConstants.DashBoardContent.RecentCallDetails);

            if (allowed)
            {
                EnquiryRecentCalls = dashBoardService.EnquiryCommon(model);
            }
            return Json(EnquiryRecentCalls, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LeadStageFlows(DashBoardViewModel model)
        {
            List<dynamic> LeadStageFlows = new List<dynamic>();
            bool allowed = checkPermission(DbConstants.DashBoardContent.LeadStageFlows);
            if (allowed)
            {
                LeadStageFlows = dashBoardService.EnquiryCommon(model);
            }

            return Json(LeadStageFlows, JsonRequestBehavior.AllowGet);
        }
        public JsonResult EnquiryMonthlyGraph(DashBoardViewModel model)
        {
            List<dynamic> EnquiryMonthlyGraph = new List<dynamic>();
            bool allowed = checkPermission(DbConstants.DashBoardContent.EnquirySurvey);
            if (allowed)
            {
                EnquiryMonthlyGraph = dashBoardService.EnquiryCommon(model);
            }

            return Json(EnquiryMonthlyGraph, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EnquiryEmployeeCounts(DashBoardViewModel model)
        {
            List<dynamic> EnquiryEmployeeCounts = new List<dynamic>();
            bool allowed = checkPermission(DbConstants.DashBoardContent.EmployeeCounts);
            if (allowed)
            {
                EnquiryEmployeeCounts = dashBoardService.EnquiryEmployeeCount(model);
            }
            return Json(EnquiryEmployeeCounts, JsonRequestBehavior.AllowGet);
        }


        public JsonResult FillEmployeeByBranch(short BranchKey)
        {
            DashBoardViewModel model = new DashBoardViewModel();

            model.AppUsers = selectListService.FillAppUserById(BranchKey);

            return Json(model.AppUsers, JsonRequestBehavior.AllowGet);
        }
        #endregion Enquiry DashBoard

        #region Application DashBoard

        public JsonResult StudentsCount(DashBoardViewModel model)
        {
            List<dynamic> StudentsCount = new List<dynamic>();
            bool allowed = checkPermission(DbConstants.DashBoardContent.StudentsCounts);
            if (allowed)
            {

                StudentsCount = dashBoardService.StudentsCommon(model);
            }

            return Json(StudentsCount, JsonRequestBehavior.AllowGet);
        }
        public JsonResult YearlyAdmissionGraph(DashBoardViewModel model)
        {
            List<dynamic> YearlyAdmissionGraph = new List<dynamic>();
            bool allowed = checkPermission(DbConstants.DashBoardContent.StudentsSurvey);
            if (allowed)
            {
                YearlyAdmissionGraph = dashBoardService.StudentsCommon(model);
            }

            return Json(YearlyAdmissionGraph, JsonRequestBehavior.AllowGet);
        }
        public JsonResult StudentCountByCourse(DashBoardViewModel model)
        {
            List<dynamic> StudentCountByCourse = new List<dynamic>();
            bool allowed = checkPermission(DbConstants.DashBoardContent.StudentsByCourse);
            if (allowed)
            {
                StudentCountByCourse = dashBoardService.StudentsCommon(model);
            }
            return Json(StudentCountByCourse, JsonRequestBehavior.AllowGet);
        }

        public JsonResult StudentCountByCourseType(DashBoardViewModel model)
        {
            List<dynamic> StudentCountByCourse = new List<dynamic>();
            bool allowed = checkPermission(DbConstants.DashBoardContent.StudentsByCoursetype);
            if (allowed)
            {
                StudentCountByCourse = dashBoardService.StudentsCommon(model);
            }
            return Json(StudentCountByCourse, JsonRequestBehavior.AllowGet);
        }
        public JsonResult TodayAbsentList(DashBoardViewModel model)
        {
            List<dynamic> TodayAbsentList = new List<dynamic>();
            bool allowed = checkPermission(DbConstants.DashBoardContent.AbsentList);
            if (allowed)
            {
                TodayAbsentList = dashBoardService.StudentsCommon(model);
            }
            return Json(TodayAbsentList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult StudentDiaryDetails(DashBoardViewModel model)
        {
            List<dynamic> StudentDiaryDetails = new List<dynamic>();
            bool allowed = checkPermission(DbConstants.DashBoardContent.StudentDiary);
            if (allowed)
            {
                StudentDiaryDetails = dashBoardService.StudentsCommon(model);
            }
            return Json(StudentDiaryDetails, JsonRequestBehavior.AllowGet);
        }
        public JsonResult StudentNewAdmission(DashBoardViewModel model)
        {
            List<dynamic> StudentNewAdmission = new List<dynamic>();
            bool allowed = checkPermission(DbConstants.DashBoardContent.RecentlyAdmitted);
            if (allowed)
            {
                StudentNewAdmission = dashBoardService.StudentsCommon(model);
            }
            return Json(StudentNewAdmission, JsonRequestBehavior.AllowGet);
        }

        #endregion Application DashBoard


        #region Accounts DashBoard

        public JsonResult AccountsCount(DashBoardViewModel model)
        {
            List<dynamic> AccountsCount = new List<dynamic>();
            bool allowed = checkPermission(DbConstants.DashBoardContent.AccountsCount);
            if (allowed)
            {
                AccountsCount = dashBoardService.AccountsCommon(model);
            }
            return Json(AccountsCount, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CashFlowGraph(DashBoardViewModel model)
        {
            List<dynamic> CashFlowGraph = new List<dynamic>();
            bool allowed = checkPermission(DbConstants.DashBoardContent.CashFlowGraph);
            if (allowed)
            {
                CashFlowGraph = dashBoardService.AccountsCommon(model);
            }
            return Json(CashFlowGraph, JsonRequestBehavior.AllowGet);
        }
        public JsonResult IncomeandExpenseChart(DashBoardViewModel model)
        {
            List<dynamic> IncomeandExpenseChart = new List<dynamic>();
            bool allowed = checkPermission(DbConstants.DashBoardContent.IncomeandExpenseChart);
            if (allowed)
            {
                IncomeandExpenseChart = dashBoardService.AccountsCommon(model);
            }
            return Json(IncomeandExpenseChart, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ChequeDetails(DashBoardViewModel model)
        {
            List<dynamic> ChequeDetailsList = new List<dynamic>();
            bool allowed = checkPermission(DbConstants.DashBoardContent.ChequeDetails);
            if (allowed)
            {
                ChequeDetailsList = dashBoardService.AccountsCommon(model);
            }
            return Json(ChequeDetailsList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RecentTransaction(DashBoardViewModel model)
        {
            List<dynamic> RecentTransactionList = new List<dynamic>();
            bool allowed = checkPermission(DbConstants.DashBoardContent.RecentTransaction);
            if (allowed)
            {
                RecentTransactionList = dashBoardService.AccountsCommon(model);
            }
            return Json(RecentTransactionList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SalaryDetails(DashBoardViewModel model)
        {
            List<dynamic> RecentTransactionList = new List<dynamic>();
            bool allowed = checkPermission(DbConstants.DashBoardContent.SalaryDetails);
            if (allowed)
            {
                RecentTransactionList = dashBoardService.AccountsCommon(model);
            }
            return Json(RecentTransactionList, JsonRequestBehavior.AllowGet);
        }

        #endregion Application DashBoard



        public bool checkPermission(short? DashBoardContentKey)
        {
            DashBoardPermissionViewModel dashBoardPermissionmodel = new DashBoardPermissionViewModel();
            dashBoardPermissionmodel.DashBoardContentKey = DashBoardContentKey;
            bool allowed = false;
            if (DbConstants.AdminKey == DbConstants.User.UserKey)
            {
                allowed = true;
            }
            else
            {
                allowed = dashBoardPermissionService.CheckDashBoardPermission(dashBoardPermissionmodel);
            }
            return allowed;
        }
    }
}