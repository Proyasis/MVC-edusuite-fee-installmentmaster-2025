using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Provider;
using System.Web.Security;
using CITS.EduSuite.Business.Models.Security;



namespace CITS.EduSuite.UI.Controllers
{
    public class ReportController : BaseController
    {
        // GET: StudentsSummaryReport
        private IReportService ReportService;
        private IBaseStudentSearchService baseStudentSearchService;
        private IEnquiryScheduleService enquiryScheduleService;
        public IWorkScheduleService WorkScheduleService;
        public ISelectListService selectListService;
        public ReportController(IReportService ObjReportService, IBaseStudentSearchService objBaseStudentSearchService, IEnquiryScheduleService objenquiryScheduleService, IWorkScheduleService objWorkScheduleService, ISelectListService objselectListService)
        {
            this.ReportService = ObjReportService;
            this.baseStudentSearchService = objBaseStudentSearchService;
            this.enquiryScheduleService = objenquiryScheduleService;
            this.WorkScheduleService = objWorkScheduleService;
            this.selectListService = objselectListService;
        }

        #region Student Summary
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentsSummaryReport, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult StudentsSummaryReportList()
        {
            ReportViewModel model = new ReportViewModel();
            baseStudentSearchService.FillClassModes(model);
            baseStudentSearchService.FillReligions(model);
            baseStudentSearchService.FillSecondLanguages(model);
            baseStudentSearchService.FillMediums(model);
            baseStudentSearchService.FillIncomes(model);
            baseStudentSearchService.FillNatureOfEnquiry(model);
            baseStudentSearchService.FillAgents(model);
            baseStudentSearchService.FillRegistrationCatagory(model);
            baseStudentSearchService.FillCaste(model);
            baseStudentSearchService.FillCommunityType(model);
            baseStudentSearchService.FillBloodGroup(model);

            return View(model);
        }

        [HttpPost]
        public JsonResult GetStudentsSummaryReport(ReportViewModel model)
        {               
            long TotalRecords = 0;
            List<dynamic> StudentSummaryReportList = new List<dynamic>();
            StudentSummaryReportList = ReportService.GetStudentsSummaryReport(model, out TotalRecords);
            int pageindex = Convert.ToInt32(model.page) - 1;
            int pagesize = model.rows ?? 0;
            long totalrecords = TotalRecords;
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);

            var jsonData = new
            {
                total = totalpage,
                model.page,
                records = totalrecords,
                rows = StudentSummaryReportList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion Student Summary

        #region FeePaymentSummary
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentsFeeSummary, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult StudentsFeeSummaryReportList()
        {
            ReportViewModel model = new ReportViewModel();
            ReportService.FillDropDownLists(model);
            baseStudentSearchService.FillClassModes(model);
            baseStudentSearchService.FillReligions(model);
            baseStudentSearchService.FillSecondLanguages(model);
            baseStudentSearchService.FillMediums(model);
            baseStudentSearchService.FillIncomes(model);
            baseStudentSearchService.FillNatureOfEnquiry(model);
            baseStudentSearchService.FillAgents(model);
            baseStudentSearchService.FillRegistrationCatagory(model);
            baseStudentSearchService.FillCaste(model);
            baseStudentSearchService.FillCommunityType(model);
            baseStudentSearchService.FillBloodGroup(model);
            return View(model);
        }

        [HttpPost]
        public JsonResult GetStudentsFeeSummaryReport(ReportViewModel model)
        {
            //List<ReportViewModel> StudentSummaryReportList = new List<ReportViewModel>();
            //StudentSummaryReportList = ReportService.GetStudentsFeePaymentSummaryReport(model);
            //int pageindex = Convert.ToInt32(model.page) - 1;
            //int pagesize = model.rows ?? 0;
            //long totalrecords = model.TotalRecords;
            //var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);            
            //var jsonData = new
            //{
            //    total = totalpage,
            //    model.page,
            //    records = totalrecords,
            //    rows = StudentSummaryReportList,
            //    userData = new
            //    {
            //        TotalPaid = model.TotalPaidSum,
            //        BalanceFee = model.BalanceFeeSum,
            //        TotalFee = model.TotalFeeSum
            //    }
            //};
            //return Json(jsonData, JsonRequestBehavior.AllowGet);

            long TotalRecords = 0;
            List<dynamic> StudentSummaryReportList = new List<dynamic>();
            StudentSummaryReportList = ReportService.GetStudentsFeePaymentSummaryReport(model, out TotalRecords);
            int pageindex = Convert.ToInt32(model.page) - 1;
            int pagesize = model.rows ?? 0;
            long totalrecords = TotalRecords;
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);

            var jsonData = new
            {
                total = totalpage,
                model.page,
                records = totalrecords,
                rows = StudentSummaryReportList,
                userData = new
                {
                    TotalPaid = model.TotalPaidSum,
                    BalanceFee = model.BalanceFeeSum,
                    TotalFee = model.TotalFeeSum
                }
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetStudentTotalFeeDetails(string id)
        {

            ApplicationFeePaymentViewModel objViewModel = new ApplicationFeePaymentViewModel();
            objViewModel.ApplicationKey = Convert.ToInt64(id);
            var TotalFeeDetails = ReportService.BindTotalFeeDetails(objViewModel);
            var jsonData = new
            {
                rows = TotalFeeDetails
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Subject Issue
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudyMaterialIssueSummary, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult StudyMaterialIssueReportList()
        {
            ReportViewModel model = new ReportViewModel();
            return View(model);
        }

        [HttpPost]
        public JsonResult GetStudyMaterialIssueReport(ReportViewModel model)
        {
            //List<ReportViewModel> BookIssueReportList = new List<ReportViewModel>();
            //BookIssueReportList = ReportService.GetBookIssueSummaryReport(model);
            //int pageindex = Convert.ToInt32(model.page) - 1;
            //int pagesize = model.rows ?? 0;
            //long totalrecords = model.TotalRecords;
            //var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);

            //var jsonData = new
            //{
            //    total = totalpage,
            //    model.page,
            //    records = totalrecords,
            //    rows = BookIssueReportList
            //};
            //return Json(jsonData, JsonRequestBehavior.AllowGet);

            long TotalRecords = 0;
            List<dynamic> BookIssueReportList = new List<dynamic>();
            BookIssueReportList = ReportService.GetBookIssueSummaryReport(model, out TotalRecords);
            int pageindex = Convert.ToInt32(model.page) - 1;
            int pagesize = model.rows ?? 0;
            long totalrecords = TotalRecords;
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);

            var jsonData = new
            {
                total = totalpage,
                model.page,
                records = totalrecords,
                rows = BookIssueReportList,
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetStudyMaterialById(string id)
        {
            StudyMaterialViewModel objViewModel = new StudyMaterialViewModel();
            objViewModel.ApplicationKey = Convert.ToInt64(id);
            var StudyMaterialDetails = ReportService.GetStudyMaterialById(objViewModel);
            var jsonData = new
            {
                rows = StudyMaterialDetails
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public JsonResult GetExportStudentsSummaryReport(BookIssueSummaryReportViewModel model)
        //{
        //    List<BookIssueSummaryReportViewModel> StudentSummaryReportList = new List<BookIssueSummaryReportViewModel>();
        //    StudentSummaryReportList = bookIssueSummaryReportService.GetBookIssueSummaryReport(model);
        //    return Json(StudentSummaryReportList, JsonRequestBehavior.AllowGet);
        //}

        #endregion Subject Issue

        #region Student Id Card Issue Report

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentIDCard, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult StududentIdCardIssueReportList()
        {
            ReportViewModel model = new ReportViewModel();
            return View(model);
        }

        [HttpPost]
        public JsonResult GetStududentIdCardIssueReport(ReportViewModel model)
        {
            //List<ReportViewModel> StudentIdCardIssueReportList = new List<ReportViewModel>();
            //StudentIdCardIssueReportList = ReportService.GetStudentIdCardIssueSummaryReport(model);
            //int pageindex = Convert.ToInt32(model.page) - 1;
            //int pagesize = model.rows ?? 0;
            //long totalrecords = model.TotalRecords;
            //var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);            
            //var jsonData = new
            //{
            //    total = totalpage,
            //    model.page,
            //    records = totalrecords,
            //    rows = StudentIdCardIssueReportList
            //};
            //return Json(jsonData, JsonRequestBehavior.AllowGet);

            long TotalRecords = 0;
            List<dynamic> StudentIdCardIssueReportList = new List<dynamic>();
            StudentIdCardIssueReportList = ReportService.GetStudentIdCardIssueSummaryReport(model, out TotalRecords);
            int pageindex = Convert.ToInt32(model.page) - 1;
            int pagesize = model.rows ?? 0;
            long totalrecords = TotalRecords;
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);

            var jsonData = new
            {
                total = totalpage,
                model.page,
                records = totalrecords,
                rows = StudentIdCardIssueReportList,
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        #endregion Student Id Card Issue Report

        #region Internal Exam Result
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.InternalExamResultSummary, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult InternalExamResultReportList()
        {
            ReportViewModel model = new ReportViewModel();
            ReportService.FillDropDownLists(model);
            return View(model);
        }

        [HttpPost]
        public JsonResult GetInternalExamResultReport(ReportViewModel model)
        {
            //List<ReportViewModel> InternalExamResultReportList = new List<ReportViewModel>();
            //InternalExamResultReportList = ReportService.GetInternalExamResultSummaryReport(model);
            //int pageindex = Convert.ToInt32(model.page) - 1;
            //int pagesize = model.rows ?? 0;
            //long totalrecords = model.TotalRecords;
            //var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);

            //var jsonData = new
            //{
            //    total = totalpage,
            //    model.page,
            //    records = totalrecords,
            //    rows = InternalExamResultReportList
            //};
            //return Json(jsonData, JsonRequestBehavior.AllowGet);

            long TotalRecords = 0;
            List<dynamic> InternalExamResultReportList = new List<dynamic>();
            InternalExamResultReportList = ReportService.GetInternalExamResultSummaryReport(model, out TotalRecords);
            int pageindex = Convert.ToInt32(model.page) - 1;
            int pagesize = model.rows ?? 0;
            long totalrecords = TotalRecords;
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);

            var jsonData = new
            {
                total = totalpage,
                model.page,
                records = totalrecords,
                rows = InternalExamResultReportList,
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetStudentMarkDetails(string InternalExamDetailsKey, string ClassDetailsKey, string InternalExamKey, string SubjectKey)
        {

            ReportViewModel objViewModel = new ReportViewModel();
            objViewModel.InternalExamDetailsKey = Convert.ToInt64(InternalExamDetailsKey);
            objViewModel.ClassDetailsKey = Convert.ToInt64(ClassDetailsKey);
            objViewModel.InternalExamKey = Convert.ToInt64(InternalExamKey);
            objViewModel.SubjectKey = Convert.ToInt64(SubjectKey);
            var StudentMarkDetails = ReportService.BindStudentMarkDetails(objViewModel);
            var jsonData = new
            {
                rows = StudentMarkDetails
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region UniversityFeePaymentSummary
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UniversityPaymentSummary, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult StudentsUniversityFeeSummaryList()
        {
            ReportViewModel model = new ReportViewModel();
            ReportService.FillDropDownLists(model);
            return View(model);
        }

        [HttpPost]
        public JsonResult GetStudentsUniversityFeeReport(ReportViewModel model)
        {
            //List<ReportViewModel> StudentSummaryReportList = new List<ReportViewModel>();
            //StudentSummaryReportList = ReportService.GetUniversityFeePaymentSummaryReport(model);
            //int pageindex = Convert.ToInt32(model.page) - 1;
            //int pagesize = model.rows ?? 0;
            //long totalrecords = model.TotalRecords;
            //var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);           
            //var jsonData = new
            //{
            //    total = totalpage,
            //    model.page,
            //    records = totalrecords,
            //    rows = StudentSummaryReportList,
            //    userData = new
            //    {
            //        TotalPaid = model.TotalPaidSum,
            //        TotalUniversityPaid = model.TotalUniversityPaidSum,
            //        TotalFee = model.TotalFeeSum
            //    }
            //};
            //return Json(jsonData, JsonRequestBehavior.AllowGet);
            long TotalRecords = 0;
            List<dynamic> StudentSummaryReportList = new List<dynamic>();
            StudentSummaryReportList = ReportService.GetUniversityFeePaymentSummaryReport(model, out TotalRecords);
            int pageindex = Convert.ToInt32(model.page) - 1;
            int pagesize = model.rows ?? 0;
            long totalrecords = TotalRecords;
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);

            var jsonData = new
            {
                total = totalpage,
                model.page,
                records = totalrecords,
                rows = StudentSummaryReportList,
                userData = new
                {
                    TotalPaid = model.TotalPaidSum,
                    TotalUniversityPaid = model.TotalUniversityPaidSum
                }
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetStudentUniversityFeeDetails(string id)
        {

            UniversityPaymentViewmodel objViewModel = new UniversityPaymentViewmodel();
            objViewModel.ApplicationKey = Convert.ToInt64(id);
            var TotalFeeDetails = ReportService.BindUniversityTotalFeeDetails(objViewModel);
            var jsonData = new
            {
                rows = TotalFeeDetails
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Attendance
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentsAttendanceSummary, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult AttendaceReportList()
        {
            ReportViewModel model = new ReportViewModel();

            model.DateAddedFrom = DateTimeUTC.Now;
            model.DateAddedTo = DateTimeUTC.Now;
            model.Branches = selectListService.FillBranches();
            return View(model);
        }



        [HttpPost]
        public string GetAttendanceReport(ReportViewModel model)
        {

            var StudentSummaryReportList = ReportService.GetAttendanceSummaryReport(model);

            return StudentSummaryReportList;
        }
        [HttpPost]
        public ActionResult GetApplicationAttendance(ReportViewModel model)
        {
            model = ReportService.GetApplicationById(model);

            return PartialView(model);
        }


        [HttpPost]
        public JsonResult GetAttendanceSummaryConvert(ReportViewModel model)
        {
            //int page = 1; int rows = 15;
            long TotalRecords = 0;
            List<dynamic> AttendanceSummaryConvertList = new List<dynamic>();
            AttendanceSummaryConvertList = ReportService.GetAttendanceSummaryConvert(model, out TotalRecords);
            int pageindex = Convert.ToInt32(model.page) - 1;
            int pagesize = model.rows ?? 0;
            long totalrecords = TotalRecords;
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);

            var jsonData = new
            {
                total = totalpage,
                model.page,
                records = totalrecords,
                rows = AttendanceSummaryConvertList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }



        #endregion

        #region Students Certificate Summary
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentCertificateSummary, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult StudentCertificateReportList()
        {
            ReportViewModel model = new ReportViewModel();
            ReportService.FillDropDownLists(model);
            return View(model);
        }

        [HttpPost]
        public JsonResult GetStudentCertificateReport(ReportViewModel model)
        {
            //List<ReportViewModel> StudentSummaryReportList = new List<ReportViewModel>();
            //StudentSummaryReportList = ReportService.GetStudentCerticateSummaryReport(model);
            //int pageindex = Convert.ToInt32(model.page) - 1;
            //int pagesize = model.rows ?? 0;
            //long totalrecords = model.TotalRecords;
            //var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);

            //var jsonData = new
            //{
            //    total = totalpage,
            //    model.page,
            //    records = totalrecords,
            //    rows = StudentSummaryReportList
            //};
            //return Json(jsonData, JsonRequestBehavior.AllowGet);

            long TotalRecords = 0;
            List<dynamic> StudentSummaryReportList = new List<dynamic>();
            StudentSummaryReportList = ReportService.GetStudentCerticateSummaryReport(model, out TotalRecords);
            int pageindex = Convert.ToInt32(model.page) - 1;
            int pagesize = model.rows ?? 0;
            long totalrecords = TotalRecords;
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);

            var jsonData = new
            {
                total = totalpage,
                model.page,
                records = totalrecords,
                rows = StudentSummaryReportList,
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCertificateDetailsByApplication(string id)
        {
            ReportViewModel objViewModel = new ReportViewModel();
            var StudentCertificateDetails = ReportService.GetCertificateDetailsByApplication(Convert.ToInt64(id));
            var jsonData = new
            {
                rows = StudentCertificateDetails
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }



        #endregion Studens Certificate Summary

        #region University Certificate Summary
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UniversityCertificateSummary, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult UniversityCertificateReportList()
        {
            ReportViewModel model = new ReportViewModel();
            ReportService.FillDropDownLists(model);
            return View(model);
        }

        [HttpPost]
        public JsonResult GetUniversityCertificateReport(ReportViewModel model)
        {
            //List<ReportViewModel> UniversityCertificateReportList = new List<ReportViewModel>();
            //UniversityCertificateReportList = ReportService.GetUniversityCerticateSummaryReport(model);
            //int pageindex = Convert.ToInt32(model.page) - 1;
            //int pagesize = model.rows ?? 0;
            //long totalrecords = model.TotalRecords;
            //var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);

            //var jsonData = new
            //{
            //    total = totalpage,
            //    model.page,
            //    records = totalrecords,
            //    rows = UniversityCertificateReportList
            //};
            //return Json(jsonData, JsonRequestBehavior.AllowGet);

            long TotalRecords = 0;
            List<dynamic> UniversityCertificateReportList = new List<dynamic>();
            UniversityCertificateReportList = ReportService.GetUniversityCerticateSummaryReport(model, out TotalRecords);
            int pageindex = Convert.ToInt32(model.page) - 1;
            int pagesize = model.rows ?? 0;
            long totalrecords = TotalRecords;
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);

            var jsonData = new
            {
                total = totalpage,
                model.page,
                records = totalrecords,
                rows = UniversityCertificateReportList,
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetUniversityCertificateDetailsByApplication(string id)
        {
            ReportViewModel objViewModel = new ReportViewModel();
            var StudentCertificateDetails = ReportService.GetUniversityCertificateDetailsByApplication(Convert.ToInt64(id));
            var jsonData = new
            {
                rows = StudentCertificateDetails
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }



        #endregion University Certificate Summary

        #region Enquiry Report

        [HttpPost]
        public ActionResult GetHistoryList(int TelephoneCodeKey, string MobileNumber, int? SearchType, string Feedback)
        {
            EnquiryScheduleViewModel model = new EnquiryScheduleViewModel();
            model.SearchHistoryTelephoneCodeKey = TelephoneCodeKey;
            model.MobileNumber = MobileNumber;
            model.Feedback = Feedback;
            EnquiryScheduleViewModel list = enquiryScheduleService.GetAllHistoryByMobileNumber(model);
            model.MobileNumber = TelephoneCodeKey + model.MobileNumber;

            return PartialView(list);
        }

        // GET: EnquiryReport
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EnquiryCallSummary, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult EnquiryReportsList()
        {
            EnquiryReportViewModel model = new EnquiryReportViewModel();

            model.SearchFromDate = DateTimeUTC.Now.Date;
            model.SearchToDate = DateTimeUTC.Now.Date;
            model.DateToday = DateTimeUTC.Now.Date;
            model.MonthStartDate = new DateTime(DateTimeUTC.Now.Year, DateTimeUTC.Now.Month, 1);
            model.MonthEndDate = model.MonthStartDate.Value.AddMonths(1).AddDays(-1);
            model.DateYesterday = DateTimeUTC.Now.Date.AddDays(-1);
            model.DateTomorrow = DateTimeUTC.Now.Date.AddDays(1);
            model.DateUpcoming = DateTimeUTC.Now.Date.AddDays(2);


            //if(DateTimeUTC.Now.Date.DayOfWeek==DayOfWeek.Saturday)
            //{
            //    model.DateTomorrow = DateTimeUTC.Now.Date.AddDays(2);
            //}
            //else
            //{
            //    model.DateTomorrow = DateTimeUTC.Now.Date.AddDays(1);
            //}



            ReportService.FillDropdownLists(model);
            return View(model);
        }

        [HttpPost]
        public JsonResult GetCallReports(EnquiryReportViewModel model)
        {
            long TotalRecords = 0;
            List<CallReportCountData> CallReportsList = new List<CallReportCountData>();
            EnquiryReportViewModel objViewModel = new EnquiryReportViewModel();


            //objViewModel.ApplicantName = ApplicantName;
            //objViewModel.SearchEmployeeKey = SearchEmployeeKey;
            //objViewModel.SearchApplicationStatusKey = SearchApplicationStatusKey;
            //objViewModel.SearchApplicationScheduleStatusKey = SearchApplicationScheduleStatusKey;
            //objViewModel.BranchKey = BranchKey ?? 0;
            //objViewModel.SearchDateAddedFrom = SearchDateAddedFrom == "" ? objViewModel.SearchDateAddedFrom : Convert.ToDateTime(SearchDateAddedFrom);
            //objViewModel.SearchDateAddedTo = SearchDateAddedTo == "" ? objViewModel.SearchDateAddedTo : Convert.ToDateTime(SearchDateAddedTo);
            //objViewModel.ShowSpam = IsSpam;
            //objViewModel.SearchApplicationVisaStatusKey = SearchApplicationVisaStatusKey;
            //objViewModel.SearchInTakeKey = SearchInTakeKey;
            //objViewModel.SearchCountryKey = SearchCountryKey;
            //objViewModel.ShowClose = ShowClose;
            //objViewModel.SearchLocation = LocationName;


            //objViewModel.PageIndex = model.page;
            //objViewModel.PageSize = model.rows;
            //objViewModel.SortBy = model.sidx;
            //objViewModel.SortOrder = model.sord;

            CallReportsList = ReportService.GetCallReports(model, out TotalRecords);

            int pageIndex = Convert.ToInt32(model.page) - 1;
            int pageSize = model.rows;
            var totalPages = (int)Math.Ceiling((float)TotalRecords / (float)model.rows);

            var jsonData = new
            {
                total = totalPages,
                model.page,
                records = TotalRecords,
                rows = CallReportsList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetCallReportsDetailed()
        {

            return PartialView();
        }

        [HttpPost]
        public ActionResult ReportData(EnquiryReportViewModel model)
        {
            long TotalRecords = 0;
            List<EnquiryReportViewModel> CallReportsDetailsList = new List<EnquiryReportViewModel>();
            EnquiryReportViewModel objViewModel = new EnquiryReportViewModel();

            model.CallReportsDetailsList = ReportService.GetCallReportsDetails(model, out TotalRecords);

            return PartialView(model);
        }



        #endregion Enquiry report

        #region Activity Log Report
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.ActivityLog, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult ActivityLogReportList()
        {
            ReportViewModel model = new ReportViewModel();
            ReportService.FillAppUsers(model);
            ReportService.FillMenus(model);
            return View(model);
        }

        [HttpPost]
        public JsonResult GetActivityLogReport(ReportViewModel model)
        {
            long TotalRecords = 0;
            List<dynamic> ActivityLogReportList = new List<dynamic>();
            ActivityLogReportList = ReportService.GetActivityLogReport(model, out TotalRecords);
            int pageindex = Convert.ToInt32(model.page) - 1;
            int pagesize = model.rows ?? 0;
            long totalrecords = TotalRecords;
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);

            var jsonData = new
            {
                total = totalpage,
                model.page,
                records = totalrecords,
                rows = ActivityLogReportList
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);  
        }

        #endregion Activity Log Report

        #region UnitTest Exam Result

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UnitTestExamResultSummary, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult UnitTestResultReportList()
        {
            ReportViewModel model = new ReportViewModel();
            ReportService.FillDropDownLists(model);
            return View(model);
        }

        [HttpPost]
        public JsonResult GetUnitTestExamResultSummary(ReportViewModel model)
        {

            //List<ReportViewModel> UnitTestResultReportList = new List<ReportViewModel>();
            //UnitTestResultReportList = ReportService.GetUnitTestExamResultSummary(model);
            //int pageindex = Convert.ToInt32(model.page) - 1;
            //int pagesize = model.rows ?? 0;
            //long totalrecords = model.TotalRecords;
            //var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);

            //var jsonData = new
            //{
            //    total = totalpage,
            //    model.page,
            //    records = totalrecords,
            //    rows = UnitTestResultReportList
            //};
            //return Json(jsonData, JsonRequestBehavior.AllowGet);

            long TotalRecords = 0;
            List<dynamic> UnitTestResultReportList = new List<dynamic>();
            UnitTestResultReportList = ReportService.GetUnitTestExamResultSummary(model, out TotalRecords);
            int pageindex = Convert.ToInt32(model.page) - 1;
            int pagesize = model.rows ?? 0;
            long totalrecords = TotalRecords;
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);

            var jsonData = new
            {
                total = totalpage,
                model.page,
                records = totalrecords,
                rows = UnitTestResultReportList,
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetUnitTestStudentMarkDetails(string UnitTestScheduledKey, string ClassDetailsKey, string SubjectKey)
        {

            ReportViewModel objViewModel = new ReportViewModel();
            objViewModel.UnitTestScheduledKey = Convert.ToInt64(UnitTestScheduledKey);
            //objViewModel.ClassDetailsKey = Convert.ToInt64(ClassDetailsKey);

            //objViewModel.SubjectKey = Convert.ToInt64(SubjectKey);
            var StudentMarkDetails = ReportService.BindUnitTestStudentMarkDetails(objViewModel);
            var jsonData = new
            {
                rows = StudentMarkDetails
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Student ExamSchedule Summary
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.ExamScheduleSummary, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult ExamScheduleReportList()
        {
            ReportViewModel model = new ReportViewModel();
            ReportService.FillDropDownLists(model);
            return View(model);
        }

        [HttpPost]
        public JsonResult GetStudentExamScheduleSummary(ReportViewModel model)
        {
            //List<ReportViewModel> ExamScheduleReportList = new List<ReportViewModel>();
            //ExamScheduleReportList = ReportService.GetStudentExamScheduleSummary(model);
            //int pageindex = Convert.ToInt32(model.page) - 1;
            //int pagesize = model.rows ?? 0;
            //long totalrecords = model.TotalRecords;
            //var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);

            //var jsonData = new
            //{
            //    total = totalpage,
            //    model.page,
            //    records = totalrecords,
            //    rows = ExamScheduleReportList
            //};
            //return Json(jsonData, JsonRequestBehavior.AllowGet);

            long TotalRecords = 0;
            List<dynamic> ExamScheduleReportList = new List<dynamic>();
            ExamScheduleReportList = ReportService.GetStudentExamScheduleSummary(model, out TotalRecords);
            int pageindex = Convert.ToInt32(model.page) - 1;
            int pagesize = model.rows ?? 0;
            long totalrecords = TotalRecords;
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);

            var jsonData = new
            {
                total = totalpage,
                model.page,
                records = totalrecords,
                rows = ExamScheduleReportList,
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetExamSchdeuleDetailsByApplication(string id)
        {
            ReportViewModel objViewModel = new ReportViewModel();
            var StudentExamScheduleDetails = ReportService.GetExamSchdeuleDetailsByApplication(Convert.ToInt64(id));
            var jsonData = new
            {
                rows = StudentExamScheduleDetails
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        #endregion Student ExamSchedule Summary

        #region Teacher Work Schedule
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EmployeeWorkScheduleSummary, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult TeacherWorkScheduleReportList()
        {
            ReportViewModel model = new ReportViewModel();
            ReportService.FillDropDownLists(model);
            return View(model);
        }

        [HttpPost]
        public JsonResult GetTeacherWorkScheduleSummary(ReportViewModel model)
        {

            //List<ReportViewModel> TeacherWorkScheduleSummaryList = new List<ReportViewModel>();
            //TeacherWorkScheduleSummaryList = ReportService.GetTeacherWorkScheduleSummary(model);
            //int pageindex = Convert.ToInt32(model.page) - 1;
            //int pagesize = model.rows ?? 0;
            //long totalrecords = model.TotalRecords;
            //var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);            
            //var jsonData = new
            //{
            //    total = totalpage,
            //    model.page,
            //    records = totalrecords,
            //    rows = TeacherWorkScheduleSummaryList
            //};
            //return Json(jsonData, JsonRequestBehavior.AllowGet);


            long TotalRecords = 0;
            List<dynamic> TeacherWorkScheduleSummaryList = new List<dynamic>();
            TeacherWorkScheduleSummaryList = ReportService.GetTeacherWorkScheduleSummary(model, out TotalRecords);
            int pageindex = Convert.ToInt32(model.page) - 1;
            int pagesize = model.rows ?? 0;
            long totalrecords = TotalRecords;
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);

            var jsonData = new
            {
                total = totalpage,
                model.page,
                records = totalrecords,
                rows = TeacherWorkScheduleSummaryList,
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetHistoryWorkSchedule(string BatchKey, string ClassDetailsKey, string SubjectKey, string BranchKey, string SubjectModuleKey)
        {

            ReportViewModel objViewModel = new ReportViewModel();
            objViewModel.BatchKey = Convert.ToInt16(BatchKey);
            objViewModel.ClassDetailsKey = Convert.ToInt64(ClassDetailsKey);
            objViewModel.SubjectKey = Convert.ToInt64(SubjectKey);
            objViewModel.BranchKey = Convert.ToInt16(BranchKey);
            objViewModel.SubjectModuleKey = Convert.ToInt64(SubjectModuleKey);

            //objViewModel.SubjectKey = Convert.ToInt64(SubjectKey);
            var StudentMarkDetails = ReportService.GetHistoryWorkSchedule(objViewModel);
            var jsonData = new
            {
                rows = StudentMarkDetails
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetWorkScheduleTopicHistory(long? MasterRowKey)
        {
            WorkscheduleSubjectmodel objViewModel = new WorkscheduleSubjectmodel();
            objViewModel.MasterRowKey = MasterRowKey ?? 0;
            var HistoryWorkSchedule = WorkScheduleService.GetHistoryWorkSchedule(objViewModel);
            return PartialView(HistoryWorkSchedule);

        }

        #endregion Teacher Work Schedule

        #region Student Late Summary
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentsLateReport, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult StudentsLateReportList()
        {
            ReportViewModel model = new ReportViewModel();
            ReportService.FillAttendanceTypes(model);

            return View(model);
        }

        [HttpPost]
        public JsonResult GetStudentsLateReport(ReportViewModel model)
        {
            model.ReportType = "Late";
            long TotalRecords = 0;
            List<dynamic> StudentSummaryReportList = new List<dynamic>();
            StudentSummaryReportList = ReportService.GetStudentSummary_For_LeaveLateAbscondersED(model, out TotalRecords);
            int pageindex = Convert.ToInt32(model.page) - 1;
            int pagesize = model.rows ?? 0;
            long totalrecords = TotalRecords;
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);

            var jsonData = new
            {
                total = totalpage,
                model.page,
                records = totalrecords,
                rows = StudentSummaryReportList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion Student Late Summary

        #region Student Leave Summary
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentsLeaveReport, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult StudentsLeaveReportList()
        {
            ReportViewModel model = new ReportViewModel();
            ReportService.FillAttendanceTypes(model);

            return View(model);
        }

        [HttpPost]
        public JsonResult GetStudentsLeaveReport(ReportViewModel model)
        {
            model.ReportType = "Leave";
            long TotalRecords = 0;
            List<dynamic> StudentSummaryReportList = new List<dynamic>();
            StudentSummaryReportList = ReportService.GetStudentSummary_For_LeaveLateAbscondersED(model, out TotalRecords);
            int pageindex = Convert.ToInt32(model.page) - 1;
            int pagesize = model.rows ?? 0;
            long totalrecords = TotalRecords;
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);

            var jsonData = new
            {
                total = totalpage,
                model.page,
                records = totalrecords,
                rows = StudentSummaryReportList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion Student Leave Summary

        #region Student Absconders Summary
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentsAbscondersReport, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult StudentsAbscondersReportList()
        {
            ReportViewModel model = new ReportViewModel();
            ReportService.FillAttendanceTypes(model);

            return View(model);
        }

        [HttpPost]
        public JsonResult GetStudentsAbscondersReport(ReportViewModel model)
        {
            model.ReportType = "Absconders";
            long TotalRecords = 0;
            List<dynamic> StudentSummaryReportList = new List<dynamic>();
            StudentSummaryReportList = ReportService.GetStudentSummary_For_LeaveLateAbscondersED(model, out TotalRecords);
            int pageindex = Convert.ToInt32(model.page) - 1;
            int pagesize = model.rows ?? 0;
            long totalrecords = TotalRecords;
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);

            var jsonData = new
            {
                total = totalpage,
                model.page,
                records = totalrecords,
                rows = StudentSummaryReportList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion Student Absconders Summary

        #region Student EarlyDeparture Summary
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentsEarlyDepartureReport, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult StudentsEarlyDepartureReportList()
        {
            ReportViewModel model = new ReportViewModel();
            ReportService.FillAttendanceTypes(model);

            return View(model);
        }

        [HttpPost]
        public JsonResult GetStudentsEarlyDepartureReport(ReportViewModel model)
        {
            model.ReportType = "EarlyDeparture";
            long TotalRecords = 0;
            List<dynamic> StudentSummaryReportList = new List<dynamic>();
            StudentSummaryReportList = ReportService.GetStudentSummary_For_LeaveLateAbscondersED(model, out TotalRecords);
            int pageindex = Convert.ToInt32(model.page) - 1;
            int pagesize = model.rows ?? 0;
            long totalrecords = TotalRecords;
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);

            var jsonData = new
            {
                total = totalpage,
                model.page,
                records = totalrecords,
                rows = StudentSummaryReportList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion Student EarlyDeparture Summary

        #region DayToDay Fee Summary

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.FeeCollectionReport, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult DayToDayFeeSummaryList()
        {
            ReportViewModel model = new ReportViewModel();
            ReportService.FillDropDownForDayToDayFee(model);

            return View(model);
        }

        public ActionResult GetDayToDayFees(ReportViewModel model)
        {
            List<ReportViewModel> objViewmodel = new List<ReportViewModel>();
            objViewmodel = ReportService.GetStudentsDayToDayFeePaymentSummary(model);
            return PartialView(objViewmodel);
        }

        #endregion DayToDay Fee Summary

        #region DayToDay University Fee Summary
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UniversityFeeCollectionReport, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult DayToDayUniversityFeeSummaryList()
        {
            ReportViewModel model = new ReportViewModel();
            ReportService.FillDropDownForDayToDayFee(model);

            return View(model);
        }

        public ActionResult GetDayToDayUniversityFees(ReportViewModel model)
        {
            List<ReportViewModel> objViewmodel = new List<ReportViewModel>();
            objViewmodel = ReportService.GetStudentsDayToDayUniversityPaymentSummary(model);
            return PartialView(objViewmodel);
        }

        #endregion DayToDay University Fee Summary

        #region Fee Refund Summary
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentsFeeRefundReport, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult FeeRefundReportList()
        {
            ReportViewModel model = new ReportViewModel();
            ReportService.FillDropDownForDayToDayFee(model);
            return View(model);
        }

        [HttpPost]
        public JsonResult GetFeeRefundSummary(ReportViewModel model)
        {
            //int page = 1; int rows = 15;
            long TotalRecords = 0;
            List<dynamic> FeeRefundSummaryList = new List<dynamic>();
            FeeRefundSummaryList = ReportService.FeeRefundSummary(model, out TotalRecords);
            int pageindex = Convert.ToInt32(model.page) - 1;
            int pagesize = model.rows ?? 0;
            long totalrecords = TotalRecords;
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);

            var jsonData = new
            {
                total = totalpage,
                model.page,
                records = totalrecords,
                rows = FeeRefundSummaryList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetStudentRefundDetails(string id)
        {

            FeeRefundViewModel objViewModel = new FeeRefundViewModel();
            objViewModel.ApplicationKey = Convert.ToInt64(id);
            var FeeRefundDetailList = ReportService.FillFeeRefundDetails(objViewModel);
            var jsonData = new
            {
                rows = FeeRefundDetailList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Enquiry Lead Count Summary
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EnquiryLeadCountSummary, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult EnquiryLeadSummaryList()
        {
            EnquiryReportViewModel model = new EnquiryReportViewModel();
            model.SearchFromDate = new DateTime(DateTimeUTC.Now.Year, DateTimeUTC.Now.Month, 1);
            model.SearchToDate = DateTimeUTC.Now;
            model.Branches = selectListService.FillBranches();
            model.Employees = selectListService.FillEmployeesById(model.BranchKey);
            model.ScheduleTypeKeysList = "1";
            return View(model);
        }

        [HttpPost]
        public JsonResult EnquiryLeadSummary(EnquiryReportViewModel model)
        {
            long TotalRecords = 0;
            List<dynamic> EnquiryLeadSummaryList = new List<dynamic>();
            EnquiryLeadSummaryList = ReportService.GetEnquiryLeadSummary(model, out TotalRecords);
            int pageindex = Convert.ToInt32(model.page) - 1;
            int pagesize = model.rows;
            long totalrecords = TotalRecords;
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);

            var jsonData = new
            {
                total = totalpage,
                model.page,
                records = totalrecords,
                rows = EnquiryLeadSummaryList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion Enquiry Lead Count Summary

        #region Enquiry Count Summary
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EnquiryCountSummary, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult EnquirySummaryList()
        {
            EnquiryReportViewModel model = new EnquiryReportViewModel();
            model.SearchFromDate = new DateTime(DateTimeUTC.Now.Year, DateTimeUTC.Now.Month, 1);
            model.SearchToDate = DateTimeUTC.Now;
            model.Branches = selectListService.FillBranches();
            model.Employees = selectListService.FillEmployeesById(model.BranchKey);
            model.ScheduleTypeKeysList = "2";
            return View(model);
        }

        [HttpPost]
        public JsonResult EnquirySummary(EnquiryReportViewModel model)
        {
            long TotalRecords = 0;
            List<dynamic> EnquirySummaryList = new List<dynamic>();
            EnquirySummaryList = ReportService.GetEnquirySummary(model, out TotalRecords);
            int pageindex = Convert.ToInt32(model.page) - 1;
            int pagesize = model.rows;
            long totalrecords = TotalRecords;
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);

            var jsonData = new
            {
                total = totalpage,
                model.page,
                records = totalrecords,
                rows = EnquirySummaryList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion Enquiry Summary

        [HttpPost]
        public ActionResult EnquiryandLeadReportData(EnquiryReportViewModel model)
        {
            long TotalRecords = 0;
            List<EnquiryReportViewModel> CallReportsDetailsList = new List<EnquiryReportViewModel>();
            EnquiryReportViewModel objViewModel = new EnquiryReportViewModel();

            model.CallReportsDetailsList = ReportService.GetEnquiryandLeadDetails(model, out TotalRecords);

            //return RedirectToAction("ReportData", model);
            return PartialView("ReportData", model);
        }

        #region Cash Flow Summary
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.CashFlowSummary, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult CashFlowSummaryList()
        {
            ReportViewModel model = new ReportViewModel();
            model.Branches = selectListService.FillBranches();
            ReportService.FillDropDownForDayToDayFee(model);
            return View(model);
        }

        [HttpPost]
        public JsonResult GetCashFlowSummary(ReportViewModel model)
        {
            long TotalRecords = 0;
            List<dynamic> CashFlowSummaryList = new List<dynamic>();
            CashFlowSummaryList = ReportService.GetCashFlowSummary(model, out TotalRecords);
            int pageindex = Convert.ToInt32(model.page) - 1;
            int pagesize = model.rows ?? 0;
            long totalrecords = TotalRecords;
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);

            var jsonData = new
            {
                total = totalpage,
                model.page,
                records = totalrecords,
                rows = CashFlowSummaryList,
                userData = new
                {
                    Amount = model.TotalAmount
                }
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion Cash Flow Summary

        #region Employee Enquiry Target Summary
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EnquiryTargetSummary, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult EnquiryTargetSummaryList()
        {
            ReportViewModel model = new ReportViewModel();
            model.DateAddedFrom = DateTimeUTC.Now;
            return View(model);
        }

        [HttpPost]
        public JsonResult GetEnquiryTargetSummary(ReportViewModel model)
        {
            List<dynamic> EnquiryTargetSummary = new List<dynamic>();
            EnquiryTargetSummary = ReportService.GetEmployeeEnquiryTargetSummary(model);
            long totalrecords = EnquiryTargetSummary.Count;
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);

            var jsonData = new
            {
                total = totalpage,
                model.page,
                records = totalrecords,
                rows = EnquiryTargetSummary
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Fee Installment Summary
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.FeeInstallmentSummary, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult FeeInstallmentSummaryList()
        {
            ReportViewModel model = new ReportViewModel();
            ReportService.FillDropDownLists(model);

            baseStudentSearchService.FillClassModes(model);
            baseStudentSearchService.FillReligions(model);
            baseStudentSearchService.FillSecondLanguages(model);
            baseStudentSearchService.FillMediums(model);
            baseStudentSearchService.FillIncomes(model);
            baseStudentSearchService.FillNatureOfEnquiry(model);
            baseStudentSearchService.FillAgents(model);
            baseStudentSearchService.FillRegistrationCatagory(model);
            baseStudentSearchService.FillCaste(model);
            baseStudentSearchService.FillCommunityType(model);
            model.DateAdded = DateTimeUTC.Now;

            return View(model);
        }

        [HttpPost]
        public JsonResult GetFeeInstallmentSummary(ReportViewModel model)
        {

            List<ReportViewModel> feeInstallmentSummarylist = new List<ReportViewModel>();
            model.AcademicTermKey = model.AcademicTermKey ?? 0;
            model.PaymentStatus = model.PaymentStatus ?? 0;
            feeInstallmentSummarylist = ReportService.GetFeeInstallmentSummary(model);


            int pageindex = Convert.ToInt32(model.page) - 1;
            int pagesize = model.rows ?? 0;
            long totalrecords = feeInstallmentSummarylist.Count();
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);

            var jsondata = new
            {
                total = totalpage,
                model.page,
                records = totalrecords,
                rows = feeInstallmentSummarylist
            };
            return Json(jsondata, JsonRequestBehavior.AllowGet);




        }

        [HttpGet]
        public JsonResult GetStudentFeeInstallmentDetails(string id)
        {
            ApplicationFeePaymentViewModel objViewModel = new ApplicationFeePaymentViewModel();
            objViewModel.ApplicationKey = Convert.ToInt64(id);
            var InstallmentFeeDetails = ReportService.BindInstallmentFeeDetails(objViewModel);
            var jsonData = new
            {
                rows = InstallmentFeeDetails
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        #endregion Fee Installment Summary

        #region Students Fee Paid and un Paid Summary
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.FeePaidSummary, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult FeePaidOrUnPaidSummaryList()
        {
            ReportViewModel model = new ReportViewModel();
            ReportService.FillCommonFeeTypes(model);
            model.DateAddedFrom = DateTimeUTC.Now;
            model.DateAddedTo = DateTimeUTC.Now;
            model.Branches = selectListService.FillBranches();
            
            return View(model);
        }

        [HttpPost]
        public string GetStudentFeePaymentSummaryByDate(ReportViewModel model)
        {

            var StudentFeeSummaryByDateList = ReportService.GetStudentFeeSummaryByDate(model);

            return StudentFeeSummaryByDateList;
        }

        #endregion

        #region Employee Salary Summary

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.SalarySummary, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult EmployeeSalarySummaryList()
        {
            ReportViewModel model = new ReportViewModel();
            model.Branches = selectListService.FillBranches();
            model.Employees = selectListService.FillEmployeesByBranchKeys(model.BranchKeys);
            model.Designations = selectListService.FillDesignation();
            model.Departments = selectListService.FillDepartment();
            model.EmployeeStatus = selectListService.FillEmployeeStatus();
            model.SalaryYears = selectListService.FillYears();
            model.SalaryMonths = selectListService.FillMonths();
            return View(model);
        }

        [HttpPost]
        public JsonResult GetEmployeeSalarySummary(ReportViewModel model)
        {
            long TotalRecords = 0;
            List<dynamic> EmployeeSalarySummaryList = new List<dynamic>();
            EmployeeSalarySummaryList = ReportService.GetEmployeeSalarySummary(model, out TotalRecords);
            int pageindex = Convert.ToInt32(model.page) - 1;
            int pagesize = model.rows ?? 0;
            long totalrecords = TotalRecords;
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);

            var jsonData = new
            {
                total = totalpage,
                model.page,
                records = totalrecords,
                rows = EmployeeSalarySummaryList,
                userData = new
                {
                    TotalSalary = model.TotalSalary,
                    BalanceAmount = model.BalanceAmount,
                    TotalPaid = model.TotalPaid,
                }
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetEmployeesByBranchKeys(ReportViewModel model)
        {
            model.Employees = selectListService.FillEmployeesByBranchKeys(model.BranchKeys);
            return Json(model.Employees, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region AccountHead Income And Expense Summary
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.IncomeExpenseSummary, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult AccountHeadIncomeExpenseSummaryList()
        {
            ReportViewModel model = new ReportViewModel();

            model.DateAddedFrom = DateTimeUTC.Now;
            model.DateAddedTo = DateTimeUTC.Now;
            model.Branches = selectListService.FillBranches();
            ReportService.FillAccountHeadByBank(model,false);
            return View(model);
        }

        [HttpPost]
        public JsonResult GetAccountHeadIncomeExpenseSummary(ReportViewModel model)
        {
            long TotalRecords = 0;
            List<dynamic> AccountHeadIncomeExpenseSummaryList = new List<dynamic>();
            AccountHeadIncomeExpenseSummaryList = ReportService.GetAHIncomeExpenseSummary(model, out TotalRecords);
            int pageindex = Convert.ToInt32(model.page) - 1;
            int pagesize = model.rows ?? 0;
            long totalrecords = TotalRecords;
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);

            var jsonData = new
            {
                total = totalpage,
                model.page,
                records = totalrecords,
                rows = AccountHeadIncomeExpenseSummaryList,
                userData = new
                {
                    TotalIncome = model.TotalIncome,
                    TotalExpense = model.TotalExpense,
                    TotalBalance = model.TotalBalance,
                    Income = model.AHIncomes,
                    Expense = model.Expense,
                    Balance = model.Balance,
                }
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Employee Attendance
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EmployeeAttendanceSummary, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult EmployeeAttendaceReportList()
        {
            ReportViewModel model = new ReportViewModel();

            model.DateAddedFrom = DateTimeUTC.Now;
            model.DateAddedTo = DateTimeUTC.Now;
            model.Branches = selectListService.FillBranches();
            return View(model);
        }



        [HttpPost]
        public string GetEmployeeAttendanceReport(ReportViewModel model)
        {
            var EmployeeAttendanceSummaryList = ReportService.GetEmpoyeeAttendanceSummaryReport(model);

            return EmployeeAttendanceSummaryList;
        }
                 

        #endregion
    }
}