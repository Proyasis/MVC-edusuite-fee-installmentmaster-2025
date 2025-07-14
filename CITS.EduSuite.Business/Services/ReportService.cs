using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System.Data.Entity;
using CITS.EduSuite.Business.Common;
using System.Data.Entity.Infrastructure;
using CITS.EduSuite.Business.Models.Security;
using System.Threading;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Dynamic;
using CITS.EduSuite.Business.Models.Resources;
using System.Data.Common;
using CITS.EduSuite.Business.Extensions;
using System.Linq.Expressions;

namespace CITS.EduSuite.Business.Services
{
    public class ReportService : IReportService
    {
        private EduSuiteDatabase dbContext;

        public ReportService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        #region StudentSummary
        //public List<ReportViewModel> GetStudentsSummaryReport(ReportViewModel model)
        //{
        //    var text = model.SearchAnyText.VerifyData();
        //    ObjectParameter objTotalRecords = new ObjectParameter("TotalRecords", typeof(Int64));

        //    if (model.sidx != "")
        //    {
        //        model.sidx = model.sidx + " " + model.sord;
        //    }

        //    var StudentSummaryReports = (
        //                                 from Application in dbContext.Sp_StudentSummary_Report
        //                                     (
        //                                       String.Join(",", model.CourseKeys),
        //                                       String.Join(",", model.CourseTypeKeys),
        //                                       String.Join(",", model.UniversityMasterKeys),
        //                                       String.Join(",", model.BatchKeys),
        //                                       String.Join(",", model.ReligionKeys),
        //                                       String.Join(",", model.IncomeGroupKeys),
        //                                       String.Join(",", model.SecondLanguageKeys),
        //                                       String.Join(",", model.ModeKeys),
        //                                       String.Join(",", model.ClassModeKeys),
        //                                       String.Join(",", model.NatureOfEnquiryKeys),
        //                                       String.Join(",", model.BranchKeys),
        //                                       String.Join(",", model.AgentKeys),
        //                                       String.Join(",", model.StudentStatusKeys),
        //                                       String.Join(",", model.MeadiumKeys),
        //                                       String.Join(",", model.ClassKeys),
        //                                        model.AcademicTermKey,
        //                                       String.Join(",", model.CourseYearsKeys),
        //                                        model.GenderKey,
        //                                        model.ClassRequiredKey,
        //                                        (model.DateOfAdmission != null ? Convert.ToDateTime(model.DateOfAdmission).ToString("yyyy-MM-dd") : ""),
        //                                        (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""),
        //                                        (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""),
        //                                        model.SearchAnyText.VerifyData(),
        //                                         model.IsTaxKey,
        //                                        model.IsInstallmentKey,
        //                                        model.IsConsessionKey,
        //                                         String.Join(",", model.RegistrationStatusKeys),

        //                                        model.page,
        //                                        model.rows,
        //                                        model.sidx,
        //                                        objTotalRecords
        //                                     )
        //                                 select new ReportViewModel
        //                                 {
        //                                     RowKey = Application.RowKey
        //                                        ,
        //                                     ApplicationNo = Application.ApplicationNo
        //                                        ,
        //                                     Name = Application.StudentName
        //                                        ,
        //                                     GuardianName = Application.StudentGuardian
        //                                        ,
        //                                     MotherName = Application.StudentMotherName
        //                                        ,
        //                                     PermanentAddress = Application.StudentPermanentAddress
        //                                        ,
        //                                     PresentAddress = Application.StudentPresentAddress
        //                                        ,
        //                                     Email = Application.StudentEmail
        //                                        ,
        //                                     Phone = Application.StudentPhone
        //                                        ,
        //                                     Mobile = Application.StudentMobile
        //                                        ,
        //                                     DOB = Application.StudentDOB
        //                                        ,
        //                                     StartYear = Application.StartYear
        //                                        ,
        //                                     TotalFee = Application.StudentTotalFee
        //                                        ,
        //                                     ClassRequiredDesc = Application.StudentClassRequiredDesc
        //                                        ,
        //                                     PresentJob_CourseOfStudyId = Application.PresentJob_CourseOfStudyId
        //                                        ,
        //                                     DateOfAdmission = Application.StudentDateOfAdmission
        //                                        ,
        //                                     StudyMaterialIssueStatus = Application.StudyMaterialIssueStatus
        //                                        ,
        //                                     EnrollmentNo = Application.StudentEnrollmentNo
        //                                        ,
        //                                     PhotoPath = Application.StudentPhotoPath
        //                                        ,
        //                                     ExamRegisterNo = Application.ExamRegisterNo
        //                                        ,
        //                                     Remarks = Application.Remarks
        //                                        ,
        //                                     AdmissionNo = Application.AdmissionNo
        //                                        ,
        //                                     SerialNumber = Application.SerialNumber
        //                                        ,
        //                                     CurrentYear = Application.CurrentYear
        //                                        ,
        //                                     RollNumber = Application.RollNumber
        //                                        ,
        //                                     AcademicTermKey = Application.AcademicTermKey
        //                                        ,
        //                                     DateAdded = Application.DateAdded
        //                                        ,
        //                                     Course = Application.CourseName
        //                                        ,
        //                                     CourseType = Application.CourseTypeName
        //                                        ,
        //                                     Affiliations = Application.UniversityMasterName
        //                                        ,
        //                                     Batch = Application.BatchName
        //                                        ,
        //                                     Mode = Application.ModeName
        //                                        ,
        //                                     ClassMode = Application.ClassModeName
        //                                        ,
        //                                     Religion = Application.ReligionName
        //                                        ,
        //                                     AcademicTerm = Application.AcademicTermName
        //                                        ,
        //                                     SecondLanguage = Application.SecondLanguageName
        //                                        ,
        //                                     Medium = Application.MediumName
        //                                        ,
        //                                     Income = Application.IncomeName
        //                                        ,
        //                                     NatureOfEnquiry = Application.NatureOfEnquiryName
        //                                        ,
        //                                     Agent = Application.AgentName
        //                                        ,
        //                                     StudentStatus = Application.StudentStatusName
        //                                        ,
        //                                     BranchName = Application.BranchName
        //                                        ,
        //                                     Class = Application.ClassCode
        //                                        ,
        //                                     Gender = Application.StudentGender == 1 ? "Male" : "Female"
        //                                        ,
        //                                     ClassRequiredKey = Application.ClassRequired ? 1 : 0
        //                                        ,
        //                                     ClassRequired = Application.ClassRequired == true ? EduSuiteUIResources.Yes : EduSuiteUIResources.No
        //                                        ,
        //                                     CurrentYearText = CommonUtilities.GetYearDescriptionByCodeDetails(Application.CourseDuration ?? 0, Application.CurrentYear, Application.AcademicTermKey)
        //                                        ,
        //                                     IsTaxText = Application.IsTax == true ? EduSuiteUIResources.Yes : EduSuiteUIResources.No
        //                                        ,
        //                                     IsInstallmentText = Application.HasInstallment == true ? EduSuiteUIResources.Yes : EduSuiteUIResources.No
        //                                        ,
        //                                     IsConsessionText = Application.HasConcession == true ? EduSuiteUIResources.Yes : EduSuiteUIResources.No
        //                                      ,
        //                                     CatagoryName = Application.CatagoryName
        //                                 }).ToList();



        //    model.TotalRecords = objTotalRecords.Value != DBNull.Value ? Convert.ToInt64(objTotalRecords.Value) : 0;
        //    //StudentSummaryReports = StudentSummaryReports.OrderByDescending(Row => Row.DateAdded).Skip(skip).Take(Take).ToList();



        //    return StudentSummaryReports;
        //}

        public List<dynamic> GetStudentsSummaryReport(ReportViewModel model, out long TotalRecords)
        {
            try
            {
                List<dynamic> applicationList = new List<dynamic>();
                DbParameter TotalRecordsParam = null;

                if (model.sidx != "")
                {
                    model.sidx = model.sidx + " " + model.sord;
                }
                dbContext.LoadStoredProc("dbo.Sp_StudentSummary_Report")

                    .WithSqlParam("@CourseKeyList", String.Join(",", model.CourseKeys))
                    .WithSqlParam("@CourseTypeKeyList", String.Join(",", model.CourseTypeKeys))
                    .WithSqlParam("@UniversityMasterKeyList", String.Join(",", model.UniversityMasterKeys))
                    .WithSqlParam("@BatchKeyList", String.Join(",", model.BatchKeys))
                    .WithSqlParam("@ReligionKeyList", String.Join(",", model.ReligionKeys))
                    .WithSqlParam("@IncomeKeyList", String.Join(",", model.IncomeGroupKeys))
                    .WithSqlParam("@SecondLanguageKeyList", String.Join(",", model.SecondLanguageKeys))
                    .WithSqlParam("@ModeKeyList", String.Join(",", model.ModeKeys))
                    .WithSqlParam("@ClassModeKeyList", String.Join(",", model.ClassModeKeys))
                    .WithSqlParam("@NatureOfEnquiryKeyList", String.Join(",", model.NatureOfEnquiryKeys))
                    .WithSqlParam("@BranchKeyList", String.Join(",", model.BranchKeys))
                    .WithSqlParam("@AgentKeyList", String.Join(",", model.AgentKeys))
                    .WithSqlParam("@StudentStatusKeyList", String.Join(",", model.StudentStatusKeys))
                    .WithSqlParam("@MediumkeyList", String.Join(",", model.MeadiumKeys))
                    .WithSqlParam("@ClassDetailsKeyList", String.Join(",", model.ClassKeys))
                    .WithSqlParam("@AcademicTermKey", model.AcademicTermKey)
                    .WithSqlParam("@CourseYearsKeyList", String.Join(",", model.CourseYearsKeys))
                    .WithSqlParam("@GenderKey", model.GenderKey)
                    .WithSqlParam("@ClassRequiredKey", model.ClassRequiredKey)
                    .WithSqlParam("@AdmissionDate", (model.DateOfAdmission != null ? Convert.ToDateTime(model.DateOfAdmission).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@DateAddedFrom", (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@DateAddedTo", (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@SearchAnyText", model.SearchAnyText.VerifyData())
                    .WithSqlParam("@IsTaxKey", model.IsTaxKey)
                    .WithSqlParam("@IsInstallmentKey", model.IsInstallmentKey)
                    .WithSqlParam("@IsConsessionKey", model.IsConsessionKey)
                    .WithSqlParam("@IsFromEnquiry", model.IsEnquiryKey)
                    .WithSqlParam("@RegistrationCatagoryKeyList", String.Join(",", model.RegistrationCatagoryKeys))
                    .WithSqlParam("@CasteKeyList", String.Join(",", model.CasteKeys))
                    .WithSqlParam("@CommunityTypeKeyList", String.Join(",", model.CommunityTypeKeys))
                    .WithSqlParam("@BloodGroupKeyList", String.Join(",", model.BloodGroupKeys))
                    .WithSqlParam("@UserKey", DbConstants.User.UserKey)
                    .WithSqlParam("@PageIndex", model.page)
                    .WithSqlParam("@PageSize", model.rows)
                    .WithSqlParam("@SortBy", model.sidx)
                    .WithSqlParam("@TotalRecords", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalRecordsParam = dbParam;
                    }).ExecuteStoredProc((handler) =>
                    {
                        applicationList = handler.ReadToDynamicList<dynamic>() as List<dynamic>;
                        //applicationList = handler.ReadToList<ApplicationViewModel>() as List<ApplicationViewModel>;
                    });
                TotalRecords = Convert.ToInt64((TotalRecordsParam.Value ?? 0));
                return applicationList;
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.StudentsSummaryReport, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<dynamic>();
            }
        }


        #endregion StudentSummary

        #region FeePaymentSummary
        public void FillDropDownLists(ReportViewModel model)
        {
            FillPaymentStatus(model);
            FillFeeTypes(model);
            FillSubjects(model);
            FillInternalExamTerm(model);
            FillSubjectModules(model);

        }
        private void FillPaymentStatus(ReportViewModel model)
        {
            model.PaymentStatuses = typeof(DbConstants.PaymentStatus).GetFields().Select(row => new SelectListModel
            {
                RowKey = Convert.ToByte((row.GetValue(null).ToString())),
                Text = row.Name
            }).ToList();
        }
        private void FillFeeTypes(ReportViewModel model)
        {
            model.FeeTypes = dbContext.FeeTypes.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.FeeTypeName
            }).ToList();
        }

        //public List<ReportViewModel> GetStudentsFeePaymentSummaryReport(ReportViewModel model)
        //{

        //    ObjectParameter objTotalRecords = new ObjectParameter("TotalRecords", typeof(Int64));

        //    if (model.sidx != "")
        //    {
        //        model.sidx = model.sidx + " " + model.sord;
        //    }

        //    var StudentSummaryReports = (
        //                                 from Application in dbContext.Sp_StudentFeePaymentSummary_Report
        //                                     (
        //                                        String.Join(",", model.CourseKeys),
        //                                       String.Join(",", model.CourseTypeKeys),
        //                                       String.Join(",", model.UniversityMasterKeys),
        //                                       String.Join(",", model.BatchKeys),
        //                                       String.Join(",", model.ReligionKeys),
        //                                       String.Join(",", model.IncomeGroupKeys),
        //                                       String.Join(",", model.SecondLanguageKeys),
        //                                       String.Join(",", model.ModeKeys),
        //                                       String.Join(",", model.ClassModeKeys),
        //                                       String.Join(",", model.NatureOfEnquiryKeys),
        //                                       String.Join(",", model.BranchKeys),
        //                                       String.Join(",", model.AgentKeys),
        //                                       String.Join(",", model.StudentStatusKeys),
        //                                       String.Join(",", model.MeadiumKeys),
        //                                       String.Join(",", model.ClassKeys),
        //                                        model.AcademicTermKey,
        //                                       String.Join(",", model.CourseYearsKeys),
        //                                        model.GenderKey,
        //                                        model.ClassRequiredKey,
        //                                        (model.DateOfAdmission != null ? Convert.ToDateTime(model.DateOfAdmission).ToString("yyyy-MM-dd") : ""),
        //                                        (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""),
        //                                        (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""),
        //                                         model.SearchAnyText.VerifyData(),
        //                                        model.PaymentStatus,
        //                                        model.IsTaxKey,
        //                                        model.IsInstallmentKey,
        //                                        model.IsConsessionKey,
        //                                        model.page,
        //                                        model.rows,
        //                                        model.sidx,
        //                                        objTotalRecords
        //                                     )
        //                                 select new ReportViewModel
        //                                 {
        //                                     RowKey = Application.RowKey
        //                                        ,
        //                                     ApplicationNo = Application.ApplicationNo
        //                                        ,
        //                                     Name = Application.StudentName
        //                                        ,
        //                                     GuardianName = Application.StudentGuardian
        //                                        ,
        //                                     MotherName = Application.StudentMotherName
        //                                        ,
        //                                     PermanentAddress = Application.StudentPermanentAddress
        //                                        ,
        //                                     PresentAddress = Application.StudentPresentAddress
        //                                        ,
        //                                     Email = Application.StudentEmail
        //                                        ,
        //                                     Phone = Application.StudentPhone
        //                                        ,
        //                                     Mobile = Application.StudentMobile
        //                                        ,
        //                                     DOB = Application.StudentDOB
        //                                        ,
        //                                     StartYear = Application.StartYear
        //                                        ,
        //                                     TotalFee = Application.StudentTotalFee
        //                                        ,
        //                                     ClassRequiredDesc = Application.StudentClassRequiredDesc
        //                                        ,
        //                                     PresentJob_CourseOfStudyId = Application.PresentJob_CourseOfStudyId
        //                                        ,
        //                                     DateOfAdmission = Application.StudentDateOfAdmission
        //                                        ,
        //                                     StudyMaterialIssueStatus = Application.StudyMaterialIssueStatus
        //                                        ,
        //                                     EnrollmentNo = Application.StudentEnrollmentNo
        //                                        ,
        //                                     PhotoPath = Application.StudentPhotoPath
        //                                        ,
        //                                     ExamRegisterNo = Application.ExamRegisterNo
        //                                        ,
        //                                     Remarks = Application.Remarks
        //                                        ,
        //                                     AdmissionNo = Application.AdmissionNo
        //                                        ,
        //                                     SerialNumber = Application.SerialNumber
        //                                        ,
        //                                     CurrentYear = Application.CurrentYear
        //                                        ,
        //                                     RollNumber = Application.RollNumber
        //                                        ,
        //                                     AcademicTermKey = Application.AcademicTermKey
        //                                        ,
        //                                     DateAdded = Application.DateAdded
        //                                        ,
        //                                     Course = Application.CourseName
        //                                        ,
        //                                     CourseType = Application.CourseTypeName
        //                                        ,
        //                                     Affiliations = Application.UniversityMasterName
        //                                        ,
        //                                     Batch = Application.BatchName
        //                                        ,
        //                                     Mode = Application.ModeName
        //                                        ,
        //                                     ClassMode = Application.ClassModeName
        //                                        ,
        //                                     Religion = Application.ReligionName
        //                                        ,
        //                                     AcademicTerm = Application.AcademicTermName
        //                                        ,
        //                                     SecondLanguage = Application.SecondLanguageName
        //                                        ,
        //                                     Medium = Application.MediumName
        //                                        ,
        //                                     Income = Application.IncomeName
        //                                        ,
        //                                     NatureOfEnquiry = Application.NatureOfEnquiryName
        //                                        ,
        //                                     Agent = Application.AgentName
        //                                        ,
        //                                     StudentStatus = Application.StudentStatusName
        //                                        ,
        //                                     BranchName = Application.BranchName
        //                                        ,
        //                                     Class = Application.ClassCode
        //                                        ,
        //                                     Gender = Application.StudentGender == 1 ? "Male" : "Female"
        //                                        ,
        //                                     ClassRequiredKey = Application.ClassRequired ? 1 : 0
        //                                        ,
        //                                     ClassRequired = Application.ClassRequired == true ? EduSuiteUIResources.Yes : EduSuiteUIResources.No
        //                                        ,
        //                                     TotalPaid = Application.TotalPaid
        //                                        ,
        //                                     BalanceFee = Application.BalanceAmount
        //                                      ,
        //                                     CurrentYearText = CommonUtilities.GetYearDescriptionByCodeDetails(Application.CourseDuration ?? 0, Application.CurrentYear, Application.AcademicTermKey)
        //                                      ,
        //                                     IsTaxText = Application.IsTax == true ? EduSuiteUIResources.Yes : EduSuiteUIResources.No
        //                                        ,
        //                                     IsInstallmentText = Application.HasInstallment == true ? EduSuiteUIResources.Yes : EduSuiteUIResources.No
        //                                        ,
        //                                     IsConsessionText = Application.HasConcession == true ? EduSuiteUIResources.Yes : EduSuiteUIResources.No

        //                                 }).ToList();



        //    model.TotalRecords = objTotalRecords.Value != DBNull.Value ? Convert.ToInt64(objTotalRecords.Value) : 0;
        //    //StudentSummaryReports = StudentSummaryReports.OrderByDescending(Row => Row.DateAdded).Skip(skip).Take(Take).ToList();

        //    model.TotalPaidSum = StudentSummaryReports.Select(x => x.TotalPaid).DefaultIfEmpty().Sum();
        //    model.BalanceFeeSum = StudentSummaryReports.Select(x => x.BalanceFee).DefaultIfEmpty().Sum();
        //    model.TotalFeeSum = StudentSummaryReports.Select(x => x.TotalFee).DefaultIfEmpty().Sum();

        //    return StudentSummaryReports;
        //}

        public List<dynamic> GetStudentsFeePaymentSummaryReport(ReportViewModel model, out long TotalRecords)
        {
            try
            {
                List<dynamic> applicationList = new List<dynamic>();
                DbParameter TotalRecordsParam = null;
                DbParameter TotalPaidSumParam = null;
                DbParameter BalanceFeeSumParam = null;
                DbParameter TotalFeeSumParam = null;

                if (model.sidx != "")
                {
                    model.sidx = model.sidx + " " + model.sord;
                }
                dbContext.LoadStoredProc("dbo.Sp_StudentFeePaymentSummary_Report")

                    .WithSqlParam("@CourseKeyList", String.Join(",", model.CourseKeys))
                    .WithSqlParam("@CourseTypeKeyList", String.Join(",", model.CourseTypeKeys))
                    .WithSqlParam("@UniversityMasterKeyList", String.Join(",", model.UniversityMasterKeys))
                    .WithSqlParam("@BatchKeyList", String.Join(",", model.BatchKeys))
                    .WithSqlParam("@ReligionKeyList", String.Join(",", model.ReligionKeys))
                    .WithSqlParam("@IncomeKeyList", String.Join(",", model.IncomeGroupKeys))
                    .WithSqlParam("@SecondLanguageKeyList", String.Join(",", model.SecondLanguageKeys))
                    .WithSqlParam("@ModeKeyList", String.Join(",", model.ModeKeys))
                    .WithSqlParam("@ClassModeKeyList", String.Join(",", model.ClassModeKeys))
                    .WithSqlParam("@NatureOfEnquiryKeyList", String.Join(",", model.NatureOfEnquiryKeys))
                    .WithSqlParam("@BranchKeyList", String.Join(",", model.BranchKeys))
                    .WithSqlParam("@AgentKeyList", String.Join(",", model.AgentKeys))
                    .WithSqlParam("@StudentStatusKeyList", String.Join(",", model.StudentStatusKeys))
                    .WithSqlParam("@MediumkeyList", String.Join(",", model.MeadiumKeys))
                    .WithSqlParam("@ClassDetailsKeyList", String.Join(",", model.ClassKeys))
                    .WithSqlParam("@AcademicTermKey", model.AcademicTermKey)
                    .WithSqlParam("@CourseYearsKeyList", String.Join(",", model.CourseYearsKeys))
                    .WithSqlParam("@GenderKey", model.GenderKey)
                    .WithSqlParam("@ClassRequiredKey", model.ClassRequiredKey)
                    .WithSqlParam("@AdmissionDate", (model.DateOfAdmission != null ? Convert.ToDateTime(model.DateOfAdmission).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@DateAddedFrom", (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@DateAddedTo", (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@SearchAnyText", model.SearchAnyText.VerifyData())
                    .WithSqlParam("@PaymentStatus", model.PaymentStatus)
                    .WithSqlParam("@IsTaxKey", model.IsTaxKey)
                    .WithSqlParam("@IsInstallmentKey", model.IsInstallmentKey)
                    .WithSqlParam("@IsConsessionKey", model.IsConsessionKey)
                    .WithSqlParam("@RegistrationCatagoryKeyList", String.Join(",", model.RegistrationCatagoryKeys))
                    .WithSqlParam("@CasteKeyList", String.Join(",", model.CasteKeys))
                    .WithSqlParam("@CommunityTypeKeyList", String.Join(",", model.CommunityTypeKeys))
                    .WithSqlParam("@BloodGroupKeyList", String.Join(",", model.BloodGroupKeys))
                    .WithSqlParam("@UserKey", DbConstants.User.UserKey)
                    .WithSqlParam("@PageIndex", model.page)
                    .WithSqlParam("@PageSize", model.rows)
                    .WithSqlParam("@SortBy", model.sidx)
                    .WithSqlParam("@TotalRecords", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalRecordsParam = dbParam;
                    })
                    .WithSqlParam("@TotalPaidSum", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalPaidSumParam = dbParam;
                    })
                    .WithSqlParam("@BalanceFeeSum", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        BalanceFeeSumParam = dbParam;
                    })
                    .WithSqlParam("@TotalFeeSum", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalFeeSumParam = dbParam;
                    }).ExecuteStoredProc((handler) =>
                    {
                        applicationList = handler.ReadToDynamicList<dynamic>() as List<dynamic>;
                        //applicationList = handler.ReadToList<ApplicationViewModel>() as List<ApplicationViewModel>;
                    });

                TotalRecords = Convert.ToInt64((TotalRecordsParam.Value ?? 0));
                model.TotalPaidSum = Convert.ToInt64((TotalPaidSumParam.Value ?? 0));
                model.BalanceFeeSum = Convert.ToInt64((BalanceFeeSumParam.Value ?? 0));
                model.TotalFeeSum = Convert.ToInt64((TotalFeeSumParam.Value ?? 0));
                return applicationList;
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.StudentsFeeSummary, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<dynamic>();
            }
        }

        public List<ApplicationFeePaymentDetailViewModel> BindTotalFeeDetails(ApplicationFeePaymentViewModel model)
        {
            try
            {
                List<ApplicationFeePaymentDetailViewModel> TotalFeeDetails = new List<ApplicationFeePaymentDetailViewModel>();
                Application Application = dbContext.Applications.SingleOrDefault(row => row.RowKey == model.ApplicationKey);

                if (Application != null)
                {
                    var CourseDuration = Application.Course.CourseDuration;
                    var duration = Math.Ceiling((Convert.ToDecimal(Application.AcademicTermKey == DbConstants.AcademicTerm.Semester ? CourseDuration / 6 : CourseDuration / 12)));

                    short AcademicTermKey = dbContext.Applications.Where(row => row.RowKey == model.ApplicationKey).Select(row => row.AcademicTermKey).FirstOrDefault();

                    TotalFeeDetails = dbContext.CalculateFeeDetails(model.ApplicationKey)
                                         .Select(row => new ApplicationFeePaymentDetailViewModel
                                         {
                                             FeeYear = row.AdmissionFeeYear,
                                             FeeTypeKey = row.FeeTypeKey ?? 0,
                                             FeeTypeName = row.FeeTypeName,
                                             //FeeYearText = row.AdmissionFeeYear != null ? (duration < 1 ? "Short Term" : CommonUtilities.GetYearDescriptionByCode(row.AdmissionFeeYear ?? 0, AcademicTermKey)) : "",
                                             FeeYearText = row.AdmissionFeeYear != null ? CommonUtilities.GetYearDescriptionByCodeDetails(CourseDuration ?? 0, row.AdmissionFeeYear ?? 0, Application.AcademicTermKey) : row.FeeTypeName,
                                             TotalAmount = row.AdmissionFeeAmount,
                                             FeeAmount = row.FeePaid,
                                             BalanceAmount = row.BalanceFee,
                                             OldPaid = row.Oldpaid

                                         }).ToList();


                }
                return TotalFeeDetails;
            }
            catch (Exception)
            {
                return new List<ApplicationFeePaymentDetailViewModel>();
            }
        }
        #endregion

        #region BookIssue

        //public List<ReportViewModel> GetBookIssueSummaryReport(ReportViewModel model)
        //{

        //    ObjectParameter objTotalRecords = new ObjectParameter("TotalRecords", typeof(Int64));

        //    if (model.sidx != "")
        //    {
        //        model.sidx = model.sidx + " " + model.sord;
        //    }

        //    var StudentSummaryReports = (
        //                                 from Application in dbContext.Sp_BookIssueSummary_Report
        //                                     (
        //                                        String.Join(",", model.CourseKeys),
        //                                       String.Join(",", model.CourseTypeKeys),
        //                                       String.Join(",", model.UniversityMasterKeys),
        //                                       String.Join(",", model.BatchKeys),
        //                                       String.Join(",", model.ReligionKeys),
        //                                       String.Join(",", model.IncomeGroupKeys),
        //                                       String.Join(",", model.SecondLanguageKeys),
        //                                       String.Join(",", model.ModeKeys),
        //                                       String.Join(",", model.ClassModeKeys),
        //                                       String.Join(",", model.NatureOfEnquiryKeys),
        //                                       String.Join(",", model.BranchKeys),
        //                                       String.Join(",", model.AgentKeys),
        //                                       String.Join(",", model.StudentStatusKeys),
        //                                       String.Join(",", model.MeadiumKeys),
        //                                       String.Join(",", model.ClassKeys),
        //                                        model.AcademicTermKey,
        //                                       String.Join(",", model.CourseYearsKeys),
        //                                        model.GenderKey,
        //                                        model.ClassRequiredKey,
        //                                        (model.DateOfAdmission != null ? Convert.ToDateTime(model.DateOfAdmission).ToString("yyyy-MM-dd") : ""),
        //                                        (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""),
        //                                        (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""),

        //                                        model.StudyMaterialIssueStatus,
        //                                         model.SearchAnyText.VerifyData(),
        //                                        model.page,
        //                                        model.rows,
        //                                        model.sidx,
        //                                        objTotalRecords
        //                                     )
        //                                 select new ReportViewModel
        //                                 {
        //                                     RowKey = Application.RowKey
        //                                        ,
        //                                     ApplicationNo = Application.ApplicationNo
        //                                        ,
        //                                     Name = Application.StudentName
        //                                        ,
        //                                     GuardianName = Application.StudentGuardian
        //                                        ,
        //                                     MotherName = Application.StudentMotherName
        //                                        ,
        //                                     PermanentAddress = Application.StudentPermanentAddress
        //                                        ,
        //                                     PresentAddress = Application.StudentPresentAddress
        //                                        ,
        //                                     Email = Application.StudentEmail
        //                                        ,
        //                                     Phone = Application.StudentPhone
        //                                        ,
        //                                     Mobile = Application.StudentMobile
        //                                        ,
        //                                     DOB = Application.StudentDOB
        //                                        ,
        //                                     StartYear = Application.StartYear
        //                                        ,
        //                                     TotalFee = Application.StudentTotalFee
        //                                        ,
        //                                     ClassRequiredDesc = Application.StudentClassRequiredDesc
        //                                        ,
        //                                     PresentJob_CourseOfStudyId = Application.PresentJob_CourseOfStudyId
        //                                        ,
        //                                     DateOfAdmission = Application.StudentDateOfAdmission
        //                                        ,
        //                                     StudyMaterialIssueStatus = Application.StudyMaterialIssueStatus
        //                                        ,
        //                                     EnrollmentNo = Application.StudentEnrollmentNo
        //                                        ,
        //                                     PhotoPath = Application.StudentPhotoPath
        //                                        ,
        //                                     ExamRegisterNo = Application.ExamRegisterNo
        //                                        ,
        //                                     Remarks = Application.Remarks
        //                                        ,
        //                                     AdmissionNo = Application.AdmissionNo
        //                                        ,
        //                                     SerialNumber = Application.SerialNumber
        //                                        ,
        //                                     CurrentYear = Application.CurrentYear
        //                                        ,
        //                                     RollNumber = Application.RollNumber
        //                                        ,
        //                                     AcademicTermKey = Application.AcademicTermKey
        //                                        ,
        //                                     DateAdded = Application.DateAdded
        //                                        ,
        //                                     Course = Application.CourseName
        //                                        ,
        //                                     CourseType = Application.CourseTypeName
        //                                        ,
        //                                     Affiliations = Application.UniversityMasterName
        //                                        ,
        //                                     Batch = Application.BatchName
        //                                        ,
        //                                     Mode = Application.ModeName
        //                                        ,
        //                                     ClassMode = Application.ClassModeName
        //                                        ,
        //                                     Religion = Application.ReligionName
        //                                        ,
        //                                     AcademicTerm = Application.AcademicTermName
        //                                        ,
        //                                     SecondLanguage = Application.SecondLanguageName
        //                                        ,
        //                                     Medium = Application.MediumName
        //                                        ,
        //                                     Income = Application.IncomeName
        //                                        ,
        //                                     NatureOfEnquiry = Application.NatureOfEnquiryName
        //                                        ,
        //                                     Agent = Application.AgentName
        //                                        ,
        //                                     StudentStatus = Application.StudentStatusName
        //                                        ,
        //                                     BranchName = Application.BranchName
        //                                        ,
        //                                     Class = Application.ClassCode
        //                                        ,
        //                                     Gender = Application.StudentGender == 1 ? "Male" : "Female"
        //                                        ,
        //                                     ClassRequiredKey = Application.ClassRequired ? 1 : 0
        //                                        ,
        //                                     ClassRequired = Application.ClassRequired == true ? EduSuiteUIResources.Yes : EduSuiteUIResources.No
        //                                        ,
        //                                     IssuedBookCount = Application.IssuedBookCount
        //                                        ,
        //                                     AvailableBookCount = Application.AvailableBookCount
        //                                        ,
        //                                     TotalBooks = Application.TotalBooks
        //                                        ,
        //                                     CurrentYearText = CommonUtilities.GetYearDescriptionByCodeDetails(Application.CourseDuration ?? 0, Application.CurrentYear, Application.AcademicTermKey)
        //                                     //   ,
        //                                     //ChalanAmount = Application.ChalanAmount
        //                                     //   ,
        //                                     //ChalanDate = Application.ChalanDate
        //                                     //   ,
        //                                     //ChalanNo = Application.ChalanNo
        //                                     //   ,
        //                                     //BankName = Application.BankName

        //                                 }).ToList();



        //    model.TotalRecords = objTotalRecords.Value != DBNull.Value ? Convert.ToInt64(objTotalRecords.Value) : 0;

        //    return StudentSummaryReports;
        //}
        public List<dynamic> GetBookIssueSummaryReport(ReportViewModel model, out long TotalRecords)
        {
            try
            {
                List<dynamic> applicationList = new List<dynamic>();
                DbParameter TotalRecordsParam = null;

                if (model.sidx != "")
                {
                    model.sidx = model.sidx + " " + model.sord;
                }
                dbContext.LoadStoredProc("dbo.Sp_BookIssueSummary_Report")

                    .WithSqlParam("@CourseKeyList", String.Join(",", model.CourseKeys))
                    .WithSqlParam("@CourseTypeKeyList", String.Join(",", model.CourseTypeKeys))
                    .WithSqlParam("@UniversityMasterKeyList", String.Join(",", model.UniversityMasterKeys))
                    .WithSqlParam("@BatchKeyList", String.Join(",", model.BatchKeys))
                    .WithSqlParam("@ReligionKeyList", String.Join(",", model.ReligionKeys))
                    .WithSqlParam("@IncomeKeyList", String.Join(",", model.IncomeGroupKeys))
                    .WithSqlParam("@SecondLanguageKeyList", String.Join(",", model.SecondLanguageKeys))
                    .WithSqlParam("@ModeKeyList", String.Join(",", model.ModeKeys))
                    .WithSqlParam("@ClassModeKeyList", String.Join(",", model.ClassModeKeys))
                    .WithSqlParam("@NatureOfEnquiryKeyList", String.Join(",", model.NatureOfEnquiryKeys))
                    .WithSqlParam("@BranchKeyList", String.Join(",", model.BranchKeys))
                    .WithSqlParam("@AgentKeyList", String.Join(",", model.AgentKeys))
                    .WithSqlParam("@StudentStatusKeyList", String.Join(",", model.StudentStatusKeys))
                    .WithSqlParam("@MediumkeyList", String.Join(",", model.MeadiumKeys))
                    .WithSqlParam("@ClassDetailsKeyList", String.Join(",", model.ClassKeys))
                    .WithSqlParam("@AcademicTermKey", model.AcademicTermKey)
                    .WithSqlParam("@CourseYearsKeyList", String.Join(",", model.CourseYearsKeys))
                    .WithSqlParam("@GenderKey", model.GenderKey)
                    .WithSqlParam("@ClassRequiredKey", model.ClassRequiredKey)
                    .WithSqlParam("@AdmissionDate", (model.DateOfAdmission != null ? Convert.ToDateTime(model.DateOfAdmission).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@DateAddedFrom", (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@DateAddedTo", (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@StudyMaterialStatus", model.StudyMaterialIssueStatus)
                    .WithSqlParam("@SearchAnyText", model.SearchAnyText.VerifyData())
                    .WithSqlParam("@UserKey", DbConstants.User.UserKey)
                    .WithSqlParam("@PageIndex", model.page)
                    .WithSqlParam("@PageSize", model.rows)
                    .WithSqlParam("@SortBy", model.sidx)
                    .WithSqlParam("@TotalRecords", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalRecordsParam = dbParam;
                    }).ExecuteStoredProc((handler) =>
                    {
                        applicationList = handler.ReadToDynamicList<dynamic>() as List<dynamic>;
                        //applicationList = handler.ReadToList<ApplicationViewModel>() as List<ApplicationViewModel>;
                    });

                TotalRecords = Convert.ToInt64((TotalRecordsParam.Value ?? 0));
                return applicationList;
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.StudyMaterialIssueSummary, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<dynamic>();
            }
        }

        public List<StudyMaterialDetailsModel> GetStudyMaterialById(StudyMaterialViewModel model)
        {
            try
            {
                Application application = dbContext.Applications.SingleOrDefault(x => x.RowKey == model.ApplicationKey);
                List<StudyMaterialDetailsModel> StudyMaterialList = new List<StudyMaterialDetailsModel>();
                StudyMaterialList = (from A in dbContext.F_StudentBooks(model.ApplicationKey).Where(x => x.HasStudyMaterial)
                                     join B in dbContext.IssueOfStudyMaterials.AsEnumerable()
                                     on new { StudyMaterialKey = A.RowKey, A.ApplicationKey } equals new { B.StudyMaterialKey, B.ApplicationKey } into B
                                     from A1 in B.DefaultIfEmpty()
                                     select new StudyMaterialDetailsModel
                                     {
                                         StudyMaterialName = A.StudyMaterialName,
                                         StudyMaterialCode = A.StudyMaterialCode,
                                         SubjectYear = A.CourseYear,
                                         IsAvailable = A1.IsAvailable != null ? A1.IsAvailable : false,
                                         IsIssued = A1.IsIssued != null ? A1.IsIssued : false,
                                         IssuedDate = A1.IssuedDate,
                                         IssuedBy = A1.IssuedBy,
                                         StudyMaterialStatusBy = dbContext.AppUsers.Where(x => x.RowKey == A1.IssuedBy).Select(y => y.AppUserName).FirstOrDefault(),
                                     }).ToList();

                foreach (StudyMaterialDetailsModel MaterialList in StudyMaterialList)
                {
                    MaterialList.SubjectYearText = CommonUtilities.GetYearDescriptionByCodeDetails(application.Course.CourseDuration ?? 0, MaterialList.SubjectYear ?? 0, application.AcademicTermKey);

                }

                return StudyMaterialList;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.StudyMaterial, ActionConstants.View, DbConstants.LogType.Error, model.ApplicationKey, ex.GetBaseException().Message);
                return new List<StudyMaterialDetailsModel>();


            }
        }

        #endregion BookIssue

        #region Student Id Card Issue Report
        //public List<ReportViewModel> GetStudentIdCardIssueSummaryReport(ReportViewModel model)
        //{
        //    ObjectParameter objTotalRecords = new ObjectParameter("TotalRecords", typeof(Int64));

        //    if (model.sidx != "")
        //    {
        //        model.sidx = model.sidx + " " + model.sord;
        //    }

        //    var StudentIdCardSummaryReports = (
        //                                 from Application in dbContext.Sp_StudentIdCardSummary_Report
        //                                     (
        //                                        String.Join(",", model.CourseKeys),
        //                                       String.Join(",", model.CourseTypeKeys),
        //                                       String.Join(",", model.UniversityMasterKeys),
        //                                       String.Join(",", model.BatchKeys),
        //                                       String.Join(",", model.ReligionKeys),
        //                                       String.Join(",", model.IncomeGroupKeys),
        //                                       String.Join(",", model.SecondLanguageKeys),
        //                                       String.Join(",", model.ModeKeys),
        //                                       String.Join(",", model.ClassModeKeys),
        //                                       String.Join(",", model.NatureOfEnquiryKeys),
        //                                       String.Join(",", model.BranchKeys),
        //                                       String.Join(",", model.AgentKeys),
        //                                       String.Join(",", model.StudentStatusKeys),
        //                                       String.Join(",", model.MeadiumKeys),
        //                                       String.Join(",", model.ClassKeys),
        //                                        model.AcademicTermKey,
        //                                       String.Join(",", model.CourseYearsKeys),
        //                                        model.GenderKey,
        //                                        model.ClassRequiredKey,
        //                                        (model.DateOfAdmission != null ? Convert.ToDateTime(model.DateOfAdmission).ToString("yyyy-MM-dd") : ""),
        //                                        (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""),
        //                                        (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""),
        //                                        model.ReceivedStatusKey,
        //                                        model.IssuedStatusKey,
        //                                          model.SearchAnyText.VerifyData(),
        //                                        model.page,
        //                                        model.rows,
        //                                        model.sidx,
        //                                        objTotalRecords
        //                                     )
        //                                 select new ReportViewModel
        //                                 {
        //                                     RowKey = Application.RowKey
        //                                        ,
        //                                     ApplicationNo = Application.ApplicationNo
        //                                        ,
        //                                     Name = Application.StudentName
        //                                        ,
        //                                     GuardianName = Application.StudentGuardian
        //                                        ,
        //                                     MotherName = Application.StudentMotherName
        //                                        ,
        //                                     PermanentAddress = Application.StudentPermanentAddress
        //                                        ,
        //                                     PresentAddress = Application.StudentPresentAddress
        //                                        ,
        //                                     Email = Application.StudentEmail
        //                                        ,
        //                                     Phone = Application.StudentPhone
        //                                        ,
        //                                     Mobile = Application.StudentMobile
        //                                        ,
        //                                     DOB = Application.StudentDOB
        //                                        ,
        //                                     StartYear = Application.StartYear
        //                                        ,
        //                                     TotalFee = Application.StudentTotalFee
        //                                        ,
        //                                     ClassRequiredDesc = Application.StudentClassRequiredDesc
        //                                        ,
        //                                     PresentJob_CourseOfStudyId = Application.PresentJob_CourseOfStudyId
        //                                        ,
        //                                     DateOfAdmission = Application.StudentDateOfAdmission
        //                                        ,
        //                                     StudyMaterialIssueStatus = Application.StudyMaterialIssueStatus
        //                                        ,
        //                                     EnrollmentNo = Application.StudentEnrollmentNo
        //                                        ,
        //                                     PhotoPath = Application.StudentPhotoPath
        //                                        ,
        //                                     ExamRegisterNo = Application.ExamRegisterNo
        //                                        ,
        //                                     Remarks = Application.Remarks
        //                                        ,
        //                                     AdmissionNo = Application.AdmissionNo
        //                                        ,
        //                                     SerialNumber = Application.SerialNumber
        //                                        ,
        //                                     CurrentYear = Application.CurrentYear
        //                                        ,
        //                                     RollNumber = Application.RollNumber
        //                                        ,
        //                                     AcademicTermKey = Application.AcademicTermKey
        //                                        ,
        //                                     DateAdded = Application.DateAdded
        //                                        ,
        //                                     Course = Application.CourseName
        //                                        ,
        //                                     CourseType = Application.CourseTypeName
        //                                        ,
        //                                     Affiliations = Application.UniversityMasterName
        //                                        ,
        //                                     Batch = Application.BatchName
        //                                        ,
        //                                     Mode = Application.ModeName
        //                                        ,
        //                                     ClassMode = Application.ClassModeName
        //                                        ,
        //                                     Religion = Application.ReligionName
        //                                        ,
        //                                     AcademicTerm = Application.AcademicTermName
        //                                        ,
        //                                     SecondLanguage = Application.SecondLanguageName
        //                                        ,
        //                                     Medium = Application.MediumName
        //                                        ,
        //                                     Income = Application.IncomeName
        //                                        ,
        //                                     NatureOfEnquiry = Application.NatureOfEnquiryName
        //                                        ,
        //                                     Agent = Application.AgentName
        //                                        ,
        //                                     StudentStatus = Application.StudentStatusName
        //                                        ,
        //                                     BranchName = Application.BranchName
        //                                        ,
        //                                     Class = Application.ClassCode
        //                                        ,
        //                                     Gender = Application.StudentGender == 1 ? "Male" : "Female"
        //                                        ,
        //                                     ClassRequiredKey = Application.ClassRequired ? 1 : 0
        //                                        ,
        //                                     ClassRequired = Application.ClassRequired == true ? EduSuiteUIResources.Yes : EduSuiteUIResources.No
        //                                        ,
        //                                     CurrentYearText = CommonUtilities.GetYearDescriptionByCodeDetails(Application.CourseDuration ?? 0, Application.CurrentYear, Application.AcademicTermKey)
        //                                          ,
        //                                     IssuedStatus = Application.IsIssued ? 1 : 0
        //                                        ,
        //                                     ReceivedStatus = Application.IsReceived ? 1 : 0
        //                                        ,
        //                                     ReceivedBy = Application.ReceivedBy != 0 ? dbContext.AppUsers.Where(x => x.RowKey == Application.ReceivedBy).Select(y => y.AppUserName).SingleOrDefault() : ""
        //                                       ,
        //                                     IssuedBy = Application.IssuedBy != 0 ? dbContext.AppUsers.Where(x => x.RowKey == Application.IssuedBy).Select(y => y.AppUserName).SingleOrDefault() : ""
        //                                       ,
        //                                     ReceivedDate = Application.ReceivedDate
        //                                       ,
        //                                     IssuedDate = Application.IssuedDate



        //                                 }).ToList();



        //    model.TotalRecords = objTotalRecords.Value != DBNull.Value ? Convert.ToInt64(objTotalRecords.Value) : 0;
        //    //StudentSummaryReports = StudentSummaryReports.OrderByDescending(Row => Row.DateAdded).Skip(skip).Take(Take).ToList();



        //    return StudentIdCardSummaryReports;
        //}

        public List<dynamic> GetStudentIdCardIssueSummaryReport(ReportViewModel model, out long TotalRecords)
        {
            try
            {
                List<dynamic> applicationList = new List<dynamic>();
                DbParameter TotalRecordsParam = null;

                if (model.sidx != "")
                {
                    model.sidx = model.sidx + " " + model.sord;
                }
                dbContext.LoadStoredProc("dbo.Sp_StudentIdCardSummary_Report")

                    .WithSqlParam("@CourseKeyList", String.Join(",", model.CourseKeys))
                    .WithSqlParam("@CourseTypeKeyList", String.Join(",", model.CourseTypeKeys))
                    .WithSqlParam("@UniversityMasterKeyList", String.Join(",", model.UniversityMasterKeys))
                    .WithSqlParam("@BatchKeyList", String.Join(",", model.BatchKeys))
                    .WithSqlParam("@ReligionKeyList", String.Join(",", model.ReligionKeys))
                    .WithSqlParam("@IncomeKeyList", String.Join(",", model.IncomeGroupKeys))
                    .WithSqlParam("@SecondLanguageKeyList", String.Join(",", model.SecondLanguageKeys))
                    .WithSqlParam("@ModeKeyList", String.Join(",", model.ModeKeys))
                    .WithSqlParam("@ClassModeKeyList", String.Join(",", model.ClassModeKeys))
                    .WithSqlParam("@NatureOfEnquiryKeyList", String.Join(",", model.NatureOfEnquiryKeys))
                    .WithSqlParam("@BranchKeyList", String.Join(",", model.BranchKeys))
                    .WithSqlParam("@AgentKeyList", String.Join(",", model.AgentKeys))
                    .WithSqlParam("@StudentStatusKeyList", String.Join(",", model.StudentStatusKeys))
                    .WithSqlParam("@MediumkeyList", String.Join(",", model.MeadiumKeys))
                    .WithSqlParam("@ClassDetailsKeyList", String.Join(",", model.ClassKeys))
                    .WithSqlParam("@AcademicTermKey", model.AcademicTermKey)
                    .WithSqlParam("@CourseYearsKeyList", String.Join(",", model.CourseYearsKeys))
                    .WithSqlParam("@GenderKey", model.GenderKey)
                    .WithSqlParam("@ClassRequiredKey", model.ClassRequiredKey)
                    .WithSqlParam("@AdmissionDate", (model.DateOfAdmission != null ? Convert.ToDateTime(model.DateOfAdmission).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@DateAddedFrom", (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@DateAddedTo", (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@ReceivedStatusKey", model.ReceivedStatusKey)
                    .WithSqlParam("@IssuedStatusKey", model.IssuedStatusKey)
                    .WithSqlParam("@SearchAnyText", model.SearchAnyText.VerifyData())
                    .WithSqlParam("@UserKey", DbConstants.User.UserKey)
                    .WithSqlParam("@PageIndex", model.page)
                    .WithSqlParam("@PageSize", model.rows)
                    .WithSqlParam("@SortBy", model.sidx)
                    .WithSqlParam("@TotalRecords", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalRecordsParam = dbParam;
                    }).ExecuteStoredProc((handler) =>
                    {
                        applicationList = handler.ReadToDynamicList<dynamic>() as List<dynamic>;
                        //applicationList = handler.ReadToList<ApplicationViewModel>() as List<ApplicationViewModel>;
                    });

                TotalRecords = Convert.ToInt64((TotalRecordsParam.Value ?? 0));
                return applicationList;
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.StudentIDCardSummary, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<dynamic>();
            }
        }


        #endregion Student Id Card Issue Report

        #region University FeePaymentSummary

        //public List<ReportViewModel> GetUniversityFeePaymentSummaryReport(ReportViewModel model)
        //{
        //    ObjectParameter objTotalRecords = new ObjectParameter("TotalRecords", typeof(Int64));

        //    if (model.sidx != "")
        //    {
        //        model.sidx = model.sidx + " " + model.sord;
        //    }

        //    var UniversityPaymentSummaryReports = (
        //                                 from Application in dbContext.Sp_UniversityFeePaymentSummary_Report
        //                                     (
        //                                        String.Join(",", model.CourseKeys),
        //                                       String.Join(",", model.CourseTypeKeys),
        //                                       String.Join(",", model.UniversityMasterKeys),
        //                                       String.Join(",", model.BatchKeys),
        //                                       String.Join(",", model.ReligionKeys),
        //                                       String.Join(",", model.IncomeGroupKeys),
        //                                       String.Join(",", model.SecondLanguageKeys),
        //                                       String.Join(",", model.ModeKeys),
        //                                       String.Join(",", model.ClassModeKeys),
        //                                       String.Join(",", model.NatureOfEnquiryKeys),
        //                                       String.Join(",", model.BranchKeys),
        //                                       String.Join(",", model.AgentKeys),
        //                                       String.Join(",", model.StudentStatusKeys),
        //                                       String.Join(",", model.MeadiumKeys),
        //                                       String.Join(",", model.ClassKeys),
        //                                        model.AcademicTermKey,
        //                                       String.Join(",", model.CourseYearsKeys),
        //                                        model.GenderKey,
        //                                        model.ClassRequiredKey,
        //                                        (model.DateOfAdmission != null ? Convert.ToDateTime(model.DateOfAdmission).ToString("yyyy-MM-dd") : ""),
        //                                        (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""),
        //                                        (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""),
        //                                         model.SearchAnyText.VerifyData(),
        //                                        model.PaymentStatus,
        //                                        model.page,
        //                                        model.rows,
        //                                        model.sidx,
        //                                        objTotalRecords
        //                                     )
        //                                 select new ReportViewModel
        //                                 {
        //                                     RowKey = Application.RowKey
        //                                        ,
        //                                     ApplicationNo = Application.ApplicationNo
        //                                        ,
        //                                     Name = Application.StudentName
        //                                        ,
        //                                     GuardianName = Application.StudentGuardian
        //                                        ,
        //                                     MotherName = Application.StudentMotherName
        //                                        ,
        //                                     PermanentAddress = Application.StudentPermanentAddress
        //                                        ,
        //                                     PresentAddress = Application.StudentPresentAddress
        //                                        ,
        //                                     Email = Application.StudentEmail
        //                                        ,
        //                                     Phone = Application.StudentPhone
        //                                        ,
        //                                     Mobile = Application.StudentMobile
        //                                        ,
        //                                     DOB = Application.StudentDOB
        //                                        ,
        //                                     StartYear = Application.StartYear

        //                                        ,
        //                                     ClassRequiredDesc = Application.StudentClassRequiredDesc
        //                                        ,
        //                                     PresentJob_CourseOfStudyId = Application.PresentJob_CourseOfStudyId
        //                                        ,
        //                                     DateOfAdmission = Application.StudentDateOfAdmission
        //                                        ,
        //                                     StudyMaterialIssueStatus = Application.StudyMaterialIssueStatus
        //                                        ,
        //                                     EnrollmentNo = Application.StudentEnrollmentNo
        //                                        ,
        //                                     PhotoPath = Application.StudentPhotoPath
        //                                        ,
        //                                     ExamRegisterNo = Application.ExamRegisterNo
        //                                        ,
        //                                     Remarks = Application.Remarks
        //                                        ,
        //                                     AdmissionNo = Application.AdmissionNo
        //                                        ,
        //                                     SerialNumber = Application.SerialNumber
        //                                        ,
        //                                     CurrentYear = Application.CurrentYear
        //                                        ,
        //                                     RollNumber = Application.RollNumber
        //                                        ,
        //                                     AcademicTermKey = Application.AcademicTermKey
        //                                        ,
        //                                     DateAdded = Application.DateAdded
        //                                        ,
        //                                     Course = Application.CourseName
        //                                        ,
        //                                     CourseType = Application.CourseTypeName
        //                                        ,
        //                                     Affiliations = Application.UniversityMasterName
        //                                        ,
        //                                     Batch = Application.BatchName
        //                                        ,
        //                                     Mode = Application.ModeName
        //                                        ,
        //                                     ClassMode = Application.ClassModeName
        //                                        ,
        //                                     Religion = Application.ReligionName
        //                                        ,
        //                                     AcademicTerm = Application.AcademicTermName
        //                                        ,
        //                                     SecondLanguage = Application.SecondLanguageName
        //                                        ,
        //                                     Medium = Application.MediumName
        //                                        ,
        //                                     Income = Application.IncomeName
        //                                        ,
        //                                     NatureOfEnquiry = Application.NatureOfEnquiryName
        //                                        ,
        //                                     Agent = Application.AgentName
        //                                        ,
        //                                     StudentStatus = Application.StudentStatusName
        //                                        ,
        //                                     BranchName = Application.BranchName
        //                                        ,
        //                                     Class = Application.ClassCode
        //                                        ,
        //                                     Gender = Application.StudentGender == 1 ? "Male" : "Female"
        //                                        ,
        //                                     ClassRequiredKey = Application.ClassRequired ? 1 : 0
        //                                        ,
        //                                     ClassRequired = Application.ClassRequired == true ? EduSuiteUIResources.Yes : EduSuiteUIResources.No
        //                                        ,
        //                                     TotalPaid = Application.TotalPaid
        //                                        ,
        //                                     TotalUniversityPaid = Application.TotalUniversityPaid
        //                                      ,
        //                                     CurrentYearText = CommonUtilities.GetYearDescriptionByCodeDetails(Application.CourseDuration ?? 0, Application.CurrentYear, Application.AcademicTermKey)


        //                                 }).ToList();



        //    model.TotalRecords = objTotalRecords.Value != DBNull.Value ? Convert.ToInt64(objTotalRecords.Value) : 0;
        //    //StudentSummaryReports = StudentSummaryReports.OrderByDescending(Row => Row.DateAdded).Skip(skip).Take(Take).ToList();
        //    model.TotalPaidSum = UniversityPaymentSummaryReports.Select(x => x.TotalPaid).DefaultIfEmpty().Sum();
        //    model.TotalUniversityPaidSum = UniversityPaymentSummaryReports.Select(x => x.TotalUniversityPaid).DefaultIfEmpty().Sum();


        //    return UniversityPaymentSummaryReports;
        //}

        public List<dynamic> GetUniversityFeePaymentSummaryReport(ReportViewModel model, out long TotalRecords)
        {
            try
            {
                List<dynamic> applicationList = new List<dynamic>();
                DbParameter TotalRecordsParam = null;
                DbParameter TotalPaidSumParam = null;
                DbParameter TotalUniversityPaidSumParam = null;
                if (model.sidx != "")
                {
                    model.sidx = model.sidx + " " + model.sord;
                }
                dbContext.LoadStoredProc("dbo.Sp_UniversityFeePaymentSummary_Report")

                    .WithSqlParam("@CourseKeyList", String.Join(",", model.CourseKeys))
                    .WithSqlParam("@CourseTypeKeyList", String.Join(",", model.CourseTypeKeys))
                    .WithSqlParam("@UniversityMasterKeyList", String.Join(",", model.UniversityMasterKeys))
                    .WithSqlParam("@BatchKeyList", String.Join(",", model.BatchKeys))
                    .WithSqlParam("@ReligionKeyList", String.Join(",", model.ReligionKeys))
                    .WithSqlParam("@IncomeKeyList", String.Join(",", model.IncomeGroupKeys))
                    .WithSqlParam("@SecondLanguageKeyList", String.Join(",", model.SecondLanguageKeys))
                    .WithSqlParam("@ModeKeyList", String.Join(",", model.ModeKeys))
                    .WithSqlParam("@ClassModeKeyList", String.Join(",", model.ClassModeKeys))
                    .WithSqlParam("@NatureOfEnquiryKeyList", String.Join(",", model.NatureOfEnquiryKeys))
                    .WithSqlParam("@BranchKeyList", String.Join(",", model.BranchKeys))
                    .WithSqlParam("@AgentKeyList", String.Join(",", model.AgentKeys))
                    .WithSqlParam("@StudentStatusKeyList", String.Join(",", model.StudentStatusKeys))
                    .WithSqlParam("@MediumkeyList", String.Join(",", model.MeadiumKeys))
                    .WithSqlParam("@ClassDetailsKeyList", String.Join(",", model.ClassKeys))
                    .WithSqlParam("@AcademicTermKey", model.AcademicTermKey)
                    .WithSqlParam("@CourseYearsKeyList", String.Join(",", model.CourseYearsKeys))
                    .WithSqlParam("@GenderKey", model.GenderKey)
                    .WithSqlParam("@ClassRequiredKey", model.ClassRequiredKey)
                    .WithSqlParam("@AdmissionDate", (model.DateOfAdmission != null ? Convert.ToDateTime(model.DateOfAdmission).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@DateAddedFrom", (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@DateAddedTo", (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@SearchAnyText", model.SearchAnyText.VerifyData())
                    .WithSqlParam("@PaymentStatus", model.PaymentStatus)
                    .WithSqlParam("@UserKey", DbConstants.User.UserKey)
                    .WithSqlParam("@PageIndex", model.page)
                    .WithSqlParam("@PageSize", model.rows)
                    .WithSqlParam("@SortBy", model.sidx)
                    .WithSqlParam("@TotalRecords", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalRecordsParam = dbParam;
                    })
                    .WithSqlParam("@TotalPaidSum", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalPaidSumParam = dbParam;
                    })
                    .WithSqlParam("@TotalUniversityPaidSum", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalUniversityPaidSumParam = dbParam;
                    }).ExecuteStoredProc((handler) =>
                    {
                        applicationList = handler.ReadToDynamicList<dynamic>() as List<dynamic>;
                        //applicationList = handler.ReadToList<ApplicationViewModel>() as List<ApplicationViewModel>;
                    });

                TotalRecords = Convert.ToInt64((TotalRecordsParam.Value ?? 0));
                model.TotalPaidSum = Convert.ToInt64((TotalPaidSumParam.Value ?? 0));
                model.TotalUniversityPaidSum = Convert.ToInt64((TotalUniversityPaidSumParam.Value ?? 0));
                return applicationList;
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.UniversityPaymentSummary, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<dynamic>();
            }
        }

        public List<UniversityPaymentDetailsmodel> BindUniversityTotalFeeDetails(UniversityPaymentViewmodel model)
        {
            try
            {
                List<UniversityPaymentDetailsmodel> TotalFeeDetails = new List<UniversityPaymentDetailsmodel>();
                Application Application = dbContext.Applications.SingleOrDefault(row => row.RowKey == model.ApplicationKey);

                if (Application != null)
                {
                    var CourseDuration = Application.Course.CourseDuration;
                    var duration = Math.Ceiling((Convert.ToDecimal(Application.AcademicTermKey == DbConstants.AcademicTerm.Semester ? CourseDuration / 6 : CourseDuration / 12)));

                    short AcademicTermKey = dbContext.Applications.Where(row => row.RowKey == model.ApplicationKey).Select(row => row.AcademicTermKey).FirstOrDefault();

                    TotalFeeDetails = dbContext.CalculateUniversityFeeReportDetails(model.ApplicationKey)
                                         .Select(row => new UniversityPaymentDetailsmodel
                                         {
                                             UniversityPaymentYear = row.AdmissionFeeYear,
                                             FeeTypeKey = row.FeeTypeKey ?? 0,
                                             FeeTypeName = row.FeeTypeName,
                                             //FeeYearText = row.AdmissionFeeYear != null ? (duration < 1 ? "Short Term" : CommonUtilities.GetYearDescriptionByCode(row.AdmissionFeeYear ?? 0, AcademicTermKey)) : "",
                                             FeeYearText = row.AdmissionFeeYear != null ? CommonUtilities.GetYearDescriptionByCodeDetails(CourseDuration ?? 0, row.AdmissionFeeYear ?? 0, Application.AcademicTermKey) : row.FeeTypeName,
                                             TotalAmount = row.AdmissionFeeAmount,
                                             FeePaid = row.FeePaid,
                                             UniversityFeePaid = row.UniversityFeePaid

                                         }).ToList();


                }
                return TotalFeeDetails;
            }
            catch (Exception ex)
            {
                return new List<UniversityPaymentDetailsmodel>();
            }
        }
        #endregion

        #region Internal Exam Result
        private void FillSubjects(ReportViewModel model)
        {
            model.Subjects = dbContext.Subjects.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.SubjectName
            }).ToList();
        }
        private void FillInternalExamTerm(ReportViewModel model)
        {
            model.InternalExamTerm = dbContext.InternalExamTerms.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.InternalExamTermName
            }).ToList();
        }
        //public List<ReportViewModel> GetInternalExamResultSummaryReport(ReportViewModel model)
        //{

        //    ObjectParameter objTotalRecords = new ObjectParameter("TotalRecords", typeof(Int64));

        //    if (model.sidx != "")
        //    {
        //        model.sidx = model.sidx + " " + model.sord;
        //    }

        //    var InternalExamResultSummaryReports = (
        //                                 from Application in dbContext.Sp_InternalExamResultSummary
        //                                     (
        //                                        String.Join(",", model.CourseKeys),
        //                                       String.Join(",", model.CourseTypeKeys),
        //                                       String.Join(",", model.UniversityMasterKeys),
        //                                       String.Join(",", model.BatchKeys),
        //                                       String.Join(",", model.BranchKeys),
        //                                       String.Join(",", model.ClassKeys),
        //                                       model.AcademicTermKey,
        //                                       String.Join(",", model.CourseYearsKeys),
        //                                       (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""),
        //                                       (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""),
        //                                       String.Join(",", model.SubjectKeys),
        //                                       String.Join(",", model.InternalExamTermKeys),
        //                                       model.page,
        //                                       model.rows,
        //                                       model.sidx,
        //                                       objTotalRecords
        //                                     )
        //                                 select new ReportViewModel
        //                                 {
        //                                     RowKey = Application.RowKey ?? 0
        //                                        ,
        //                                     InternalExamKey = Application.InternalExamKey
        //                                        ,
        //                                     InternalExamDetailsKey = Application.InternalExamDetailsKey
        //                                        ,
        //                                     ClassDetailsKey = Application.ClassDetailsKey
        //                                        ,
        //                                     SubjectKey = Application.SubjectKey
        //                                        ,
        //                                     SubjectName = Application.SubjectName
        //                                        ,
        //                                     ExamDate = Application.ExamDate
        //                                        ,
        //                                     ExamStartTime = Application.ExamStartTime
        //                                        ,
        //                                     ExamEndTime = Application.ExamEndTime
        //                                        ,
        //                                     MaximumMark = Application.MaximumMark
        //                                        ,
        //                                     MinimumMark = Application.MinimumMark
        //                                        ,
        //                                     InternalExamTermName = Application.InternalExamTermName
        //                                        ,
        //                                     BatchName = Application.BatchName
        //                                        ,
        //                                     CurrentYear = Application.CourseYear
        //                                        ,
        //                                     AcademicTermKey = Application.AcademicTermKey
        //                                        ,
        //                                     Course = Application.CourseName
        //                                        ,
        //                                     Affiliations = Application.UniversityMasterName
        //                                        ,
        //                                     Batch = Application.BatchName
        //                                        ,
        //                                     BranchName = Application.BranchName
        //                                        ,
        //                                     Class = Application.ClassCode
        //                                        ,
        //                                     Passed = Application.Passed
        //                                        ,
        //                                     Failed = Application.Failed
        //                                        ,
        //                                     Absent = Application.Absent
        //                                        ,
        //                                     TotalStudents = Application.TotalStudents
        //                                        ,
        //                                     CurrentYearText = CommonUtilities.GetYearDescriptionByCodeDetails(Application.CourseDuration ?? 0, Application.CourseYear, Application.AcademicTermKey)

        //                                 }).ToList();



        //    model.TotalRecords = objTotalRecords.Value != DBNull.Value ? Convert.ToInt64(objTotalRecords.Value) : 0;
        //    return InternalExamResultSummaryReports;
        //}

        public List<dynamic> GetInternalExamResultSummaryReport(ReportViewModel model, out long TotalRecords)
        {
            try
            {
                List<dynamic> applicationList = new List<dynamic>();
                DbParameter TotalRecordsParam = null;

                if (model.sidx != "")
                {
                    model.sidx = model.sidx + " " + model.sord;
                }
                dbContext.LoadStoredProc("dbo.Sp_InternalExamResultSummary")

                    .WithSqlParam("@CourseKeyList", String.Join(",", model.CourseKeys))
                    .WithSqlParam("@CourseTypeKeyList", String.Join(",", model.CourseTypeKeys))
                    .WithSqlParam("@UniversityMasterKeyList", String.Join(",", model.UniversityMasterKeys))
                    .WithSqlParam("@BatchKeyList", String.Join(",", model.BatchKeys))

                    .WithSqlParam("@BranchKeyList", String.Join(",", model.BranchKeys))
                    .WithSqlParam("@ClassDetailsKeyList", String.Join(",", model.ClassKeys))
                    .WithSqlParam("@AcademicTermKey", model.AcademicTermKey)
                    .WithSqlParam("@CourseYearsKeyList", String.Join(",", model.CourseYearsKeys))

                    .WithSqlParam("@DateAddedFrom", (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@DateAddedTo", (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""))

                    .WithSqlParam("@SubjectKeyList", String.Join(",", model.SubjectKeys))
                    .WithSqlParam("@InternalExamKeyList", String.Join(",", model.InternalExamTermKeys))
                    .WithSqlParam("@UserKey", DbConstants.User.UserKey)
                    .WithSqlParam("@PageIndex", model.page)
                    .WithSqlParam("@PageSize", model.rows)
                    .WithSqlParam("@SortBy", model.sidx)
                    .WithSqlParam("@TotalRecords", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalRecordsParam = dbParam;
                    }).ExecuteStoredProc((handler) =>
                    {
                        applicationList = handler.ReadToDynamicList<dynamic>() as List<dynamic>;
                        //applicationList = handler.ReadToList<ApplicationViewModel>() as List<ApplicationViewModel>;
                    });

                TotalRecords = Convert.ToInt64((TotalRecordsParam.Value ?? 0));
                return applicationList;
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.InternalExamResultSummary, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<dynamic>();
            }
        }

        public List<InternalExamResultDetail> BindStudentMarkDetails(ReportViewModel model)
        {
            try
            {
                List<InternalExamResultDetail> InternalExamResultDetails = new List<InternalExamResultDetail>();


                InternalExamResultDetails = (from App in dbContext.Applications
                                             join SDA in dbContext.StudentDivisionAllocations on App.RowKey equals SDA.ApplicationKey
                                             join IE in dbContext.InternalExams on new { App.BatchKey, App.UniversityMasterKey, App.BranchKey }
                                             equals new { IE.BatchKey, IE.UniversityMasterKey, IE.BranchKey }
                                             join IED in dbContext.InternalExamDetails on IE.RowKey equals IED.InternalExamKey
                                             join IER in dbContext.InternalExamResults on new { ApplicationKey = App.RowKey, InternalExamKey = IE.RowKey, IED.SubjectKey }
                                             equals new { IER.ApplicationKey, IER.InternalExamKey, IER.SubjectKey } into IERD
                                             from IER in IERD.DefaultIfEmpty()
                                             where (App.StudentStatusKey == DbConstants.StudentStatus.Ongoing && SDA.IsActive == true &&
                                             IED.RowKey == model.InternalExamDetailsKey && IE.RowKey == model.InternalExamKey && IED.SubjectKey == model.SubjectKey && (IER.ClassDetailsKey != null ? IER.ClassDetailsKey : App.ClassDetailsKey) == model.ClassDetailsKey)
                                             select new InternalExamResultDetail
                                             {
                                                 ApplicationKey = App.RowKey,
                                                 StudentName = App.StudentName,
                                                 AdmissionNo = App.AdmissionNo,
                                                 SubjectKey = IED.SubjectKey,
                                                 ResultStatus = IER.ResultStatus,
                                                 Mark = IER.Mark,
                                                 MaximumMark = IED.MaximumMark,
                                                 MinimumMark = IED.MinimumMark,
                                                 Remarks = IER.Remarks,

                                                 InternalExamDetailsKey = IED.RowKey
                                             }).ToList();



                return InternalExamResultDetails;
            }
            catch (Exception ex)
            {
                return new List<InternalExamResultDetail>();
            }
        }
        #endregion

        #region Students Attendance Summary
        public string GetAttendanceSummaryReport(ReportViewModel model)
        {

            IEnumerable<string> results = dbContext.Database.SqlQuery<string>("Sp_AttendaceSummery @AttendanceFromDate,@AttendanceToDate,@StudentName,@StudentStatusKeyList,@BranchKey,@ClassDetailsKeyList,@ApplicationKey,@ClassDetailsKey",
                                                                                     new SqlParameter("AttendanceFromDate", (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : "")),
                                                                                     new SqlParameter("AttendanceToDate", (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : "")),
                                                                                     new SqlParameter("StudentName", model.SearchAnyText.VerifyData()),
                                                                                    new SqlParameter("StudentStatusKeyList", String.Join(",", model.StudentStatusKeys)),
                                                                                     new SqlParameter("BranchKey", model.BranchKey),
                                                                                    new SqlParameter("ClassDetailsKeyList", String.Join(",", model.ClassKeys)),
                                                                                     new SqlParameter("ApplicationKey", model.ApplicationKey),
                                                                                     new SqlParameter("ClassDetailsKey", model.ClassDetailsKey)).ToList();


            return String.Join("", results);
        }
        //public List<dynamic> GetAttendanceSummaryReport(ReportViewModel model)
        //{



        //    var AttendanceSummaryReports = dbContext.CollectionFromSql("Sp_AttendaceSummery",
        //                                                                            new Dictionary<string, object>() { {"AttendaceDate", model.DateAdded },
        //                                                                                { "ApplicationKey", model.ApplicationKey },
        //                                                                                { "UniversityDays", model.UniversityDays},
        //                                                                                { "EmployeeKey", model.EmployeeKey},
        //                                                                                { "SubjectKey", model.SubjectKey },
        //                                                                                { "StudentName", model.Name.VerifyData() },
        //                                                                                { "StudentStatusKeyList", String.Join(",", model.StudentStatusKeys)},
        //                                                                                { "CourseTypeKeyList", String.Join(",", model.CourseTypeKeys) },
        //                                                                                { "CourseKeyList", String.Join(",", model.CourseKeys) },
        //                                                                                { "UniversityMasterKeyList", String.Join(",", model.UniversityMasterKeys) },
        //                                                                                { "BatchKeyList", String.Join(",", model.BatchKeys) },
        //                                                                                { "BranchKeyList", String.Join(",", model.BranchKeys) },
        //                                                                                { "ClassDetailsKeyList", String.Join(",", model.ClassKeys) },
        //                                                                                { "AttendanceYearKeyList", String.Join(",", model.CourseYearsKeys) },
        //                                                                            }).ToList();


        //    return AttendanceSummaryReports;
        //}
        public ReportViewModel GetApplicationById(ReportViewModel model)
        {



            model = dbContext.Applications.Where(row => row.RowKey == model.ApplicationKey).Select(row => new ReportViewModel
            {
                ApplicationKey = row.RowKey,
                AdmissionNo = row.AdmissionNo,
                Name = row.StudentName,
                Course = row.Course.CourseName,
                Affiliations = row.UniversityMaster.UniversityMasterName,
                BatchName = row.Batch.BatchName,
                CurrentYear = row.CurrentYear,
                ClassKeys = model.ClassKeys,
                CourseYearsKeys = model.CourseYearsKeys,
                AcademicTermKey = row.AcademicTermKey
            }).SingleOrDefault();
            if (model == null)
            {
                model = new ReportViewModel();
            }
            model.CurrentYearText = CommonUtilities.GetYearDescriptionByCode(model.CurrentYear ?? 0, model.AcademicTermKey ?? 0);
            return model;
        }
        public List<dynamic> GetAttendanceSummaryConvert(ReportViewModel model, out long TotalRecords)
        {
            try
            {
                List<dynamic> applicationList = new List<dynamic>();
                DbParameter TotalRecordsParam = null;

                if (model.sidx != "")
                {
                    model.sidx = model.sidx + " " + model.sord;
                }
                dbContext.LoadStoredProc("dbo.Sp_AttendaceSummery_Convert")
                    .WithSqlParam("@AttendanceDate", model.DateAdded)
                    .WithSqlParam("@StudentName", model.SearchAnyText.VerifyData())
                    .WithSqlParam("@StudentStatusKeyList", String.Join(",", model.StudentStatusKeys))
                    .WithSqlParam("@BranchKey", model.BranchKey)
                    .WithSqlParam("@ClassDetailsKeyList", String.Join(",", model.ClassKeys))
                    .WithSqlParam("@ApplicationKey", model.ApplicationKey)
                    .WithSqlParam("@ClassDetailsKey", model.ClassDetailsKey)
                    .WithSqlParam("@PageIndex", model.page)
                    .WithSqlParam("@PageSize", model.rows)
                    .WithSqlParam("@SortBy", model.sidx)
                    .WithSqlParam("@TotalRecords", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalRecordsParam = dbParam;
                    }).ExecuteStoredProc((handler) =>
                    {
                        applicationList = handler.ReadToDynamicList<dynamic>() as List<dynamic>;
                        //applicationList = handler.ReadToList<ApplicationViewModel>() as List<ApplicationViewModel>;
                    });
                TotalRecords = Convert.ToInt64((TotalRecordsParam.Value ?? 0));
                return applicationList;
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.StudentsAttendanceSummary, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<dynamic>();
            }
        }

        public void FillAttendanceTypes(ReportViewModel model)
        {
            model.AttendanceTypes = dbContext.AttendanceTypes.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AttendanceTypeName
            }).ToList();
        }

        #endregion

        #region Student Certificate Summary

        public List<dynamic> GetStudentCerticateSummaryReport(ReportViewModel model, out long TotalRecords)
        {
            try
            {
                List<dynamic> applicationList = new List<dynamic>();
                DbParameter TotalRecordsParam = null;

                if (model.sidx != "")
                {
                    model.sidx = model.sidx + " " + model.sord;
                }
                dbContext.LoadStoredProc("dbo.Sp_StudentCertificateSummary_Report")

                    .WithSqlParam("@CourseKeyList", String.Join(",", model.CourseKeys))
                    .WithSqlParam("@CourseTypeKeyList", String.Join(",", model.CourseTypeKeys))
                    .WithSqlParam("@UniversityMasterKeyList", String.Join(",", model.UniversityMasterKeys))
                    .WithSqlParam("@BatchKeyList", String.Join(",", model.BatchKeys))
                    .WithSqlParam("@ReligionKeyList", String.Join(",", model.ReligionKeys))
                    .WithSqlParam("@IncomeKeyList", String.Join(",", model.IncomeGroupKeys))
                    .WithSqlParam("@SecondLanguageKeyList", String.Join(",", model.SecondLanguageKeys))
                    .WithSqlParam("@ModeKeyList", String.Join(",", model.ModeKeys))
                    .WithSqlParam("@ClassModeKeyList", String.Join(",", model.ClassModeKeys))
                    .WithSqlParam("@NatureOfEnquiryKeyList", String.Join(",", model.NatureOfEnquiryKeys))
                    .WithSqlParam("@BranchKeyList", String.Join(",", model.BranchKeys))
                    .WithSqlParam("@AgentKeyList", String.Join(",", model.AgentKeys))
                    .WithSqlParam("@StudentStatusKeyList", String.Join(",", model.StudentStatusKeys))
                    .WithSqlParam("@MediumkeyList", String.Join(",", model.MeadiumKeys))
                    .WithSqlParam("@ClassDetailsKeyList", String.Join(",", model.ClassKeys))
                    .WithSqlParam("@AcademicTermKey", model.AcademicTermKey)
                    .WithSqlParam("@CourseYearsKeyList", String.Join(",", model.CourseYearsKeys))
                    .WithSqlParam("@GenderKey", model.GenderKey)
                    .WithSqlParam("@ClassRequiredKey", model.ClassRequiredKey)
                    .WithSqlParam("@AdmissionDate", (model.DateOfAdmission != null ? Convert.ToDateTime(model.DateOfAdmission).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@DateAddedFrom", (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@DateAddedTo", (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@CertificateStatus", model.CertificateStatus)
                    .WithSqlParam("@SearchAnyText", model.SearchAnyText.VerifyData())
                    .WithSqlParam("@UserKey", DbConstants.User.UserKey)
                    .WithSqlParam("@PageIndex", model.page)
                    .WithSqlParam("@PageSize", model.rows)
                    .WithSqlParam("@SortBy", model.sidx)
                    .WithSqlParam("@TotalRecords", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalRecordsParam = dbParam;
                    }).ExecuteStoredProc((handler) =>
                    {
                        applicationList = handler.ReadToDynamicList<dynamic>() as List<dynamic>;
                    });

                TotalRecords = Convert.ToInt64((TotalRecordsParam.Value ?? 0));
                return applicationList;
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.StudentCertificateSummary, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<dynamic>();
            }
        }
        public List<StudentsCertificateReturnDetail> GetCertificateDetailsByApplication(long ApplicationKey)
        {

            var certificateDetails = (from scr in dbContext.StudentsCertificateReturns.Where(x => x.ApplicationKey == ApplicationKey)
                                      orderby scr.RowKey descending
                                      select new StudentsCertificateReturnDetail
                                      {
                                          EducationQualificationName = scr.EducationQualification.EducationQualificationCourse,
                                          EducationQualificationUniversity = scr.EducationQualification.EducationQualificationUniversity,
                                          EducationQualificationKey = scr.EducationQualification.RowKey,
                                          RowKey = scr.RowKey,
                                          CertificateStatusName = (scr.CertificateStatusKey == DbConstants.CertificateProcessType.Received ? EduSuiteUIResources.Recieved : (scr.CertificateStatusKey == DbConstants.CertificateProcessType.Returned && scr.EducationQualification.IsOriginalReturn == true ? EduSuiteUIResources.Returned : (scr.CertificateStatusKey == DbConstants.CertificateProcessType.Returned ? EduSuiteUIResources.TempReturned : EduSuiteUIResources.Verified))),
                                          CertificateStatusBy = dbContext.AppUsers.Where(x => x.RowKey == scr.IssuedBy).Select(y => y.AppUserName).FirstOrDefault(),
                                          IssuedDate = scr.IssuedDate
                                      }).ToList();

            certificateDetails = certificateDetails.GroupBy(x => x.EducationQualificationKey).Select(y => y.FirstOrDefault()).ToList();
            return certificateDetails;
        }

        #endregion

        #region University Certificate Summary        
        public List<dynamic> GetUniversityCerticateSummaryReport(ReportViewModel model, out long TotalRecords)
        {
            try
            {
                List<dynamic> applicationList = new List<dynamic>();
                DbParameter TotalRecordsParam = null;

                if (model.sidx != "")
                {
                    model.sidx = model.sidx + " " + model.sord;
                }
                dbContext.LoadStoredProc("dbo.Sp_UniversityCertificateSummary_Report")

                    .WithSqlParam("@CourseKeyList", String.Join(",", model.CourseKeys))
                    .WithSqlParam("@CourseTypeKeyList", String.Join(",", model.CourseTypeKeys))
                    .WithSqlParam("@UniversityMasterKeyList", String.Join(",", model.UniversityMasterKeys))
                    .WithSqlParam("@BatchKeyList", String.Join(",", model.BatchKeys))
                    .WithSqlParam("@ReligionKeyList", String.Join(",", model.ReligionKeys))
                    .WithSqlParam("@IncomeKeyList", String.Join(",", model.IncomeGroupKeys))
                    .WithSqlParam("@SecondLanguageKeyList", String.Join(",", model.SecondLanguageKeys))
                    .WithSqlParam("@ModeKeyList", String.Join(",", model.ModeKeys))
                    .WithSqlParam("@ClassModeKeyList", String.Join(",", model.ClassModeKeys))
                    .WithSqlParam("@NatureOfEnquiryKeyList", String.Join(",", model.NatureOfEnquiryKeys))
                    .WithSqlParam("@BranchKeyList", String.Join(",", model.BranchKeys))
                    .WithSqlParam("@AgentKeyList", String.Join(",", model.AgentKeys))
                    .WithSqlParam("@StudentStatusKeyList", String.Join(",", model.StudentStatusKeys))
                    .WithSqlParam("@MediumkeyList", String.Join(",", model.MeadiumKeys))
                    .WithSqlParam("@ClassDetailsKeyList", String.Join(",", model.ClassKeys))
                    .WithSqlParam("@AcademicTermKey", model.AcademicTermKey)
                    .WithSqlParam("@CourseYearsKeyList", String.Join(",", model.CourseYearsKeys))
                    .WithSqlParam("@GenderKey", model.GenderKey)
                    .WithSqlParam("@ClassRequiredKey", model.ClassRequiredKey)
                    .WithSqlParam("@AdmissionDate", (model.DateOfAdmission != null ? Convert.ToDateTime(model.DateOfAdmission).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@DateAddedFrom", (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@DateAddedTo", (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@CertificateStatus", model.CertificateStatus)
                    .WithSqlParam("@SearchAnyText", model.SearchAnyText.VerifyData())
                    .WithSqlParam("@UserKey", DbConstants.User.UserKey)
                    .WithSqlParam("@PageIndex", model.page)
                    .WithSqlParam("@PageSize", model.rows)
                    .WithSqlParam("@SortBy", model.sidx)
                    .WithSqlParam("@TotalRecords", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalRecordsParam = dbParam;
                    }).ExecuteStoredProc((handler) =>
                    {
                        applicationList = handler.ReadToDynamicList<dynamic>() as List<dynamic>;
                        //applicationList = handler.ReadToList<ApplicationViewModel>() as List<ApplicationViewModel>;
                    });

                TotalRecords = Convert.ToInt64((TotalRecordsParam.Value ?? 0));
                return applicationList;
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.UniversityCertificateSummary, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<dynamic>();
            }
        }
        public List<UniversityCertificateDetails> GetUniversityCertificateDetailsByApplication(long ApplicationKey)
        {

            return (from scr in dbContext.UniversityCertificates.Where(x => x.ApplicationKey == ApplicationKey)
                    select new UniversityCertificateDetails
                    {
                        CertificateTypeName = scr.CertificateType.CertificateTypeName,
                        UniversityCertificateDescription = scr.UniversityCertificateDescription,
                        CertificateTypeKey = scr.CertificateTypeKey,
                        RowKey = scr.RowKey,
                        IsReceived = scr.IsReceived,
                        IsIssued = scr.IsIssued,
                        ReceivedByName = scr.IsReceived != false ? dbContext.AppUsers.Where(x => x.RowKey == scr.ReceivedBy).Select(y => y.AppUserName).FirstOrDefault() : "",
                        ReceivedDate = scr.ReceivedDate,
                        IssuedByName = scr.IsIssued != false ? dbContext.AppUsers.Where(x => x.RowKey == scr.ReceivedBy).Select(y => y.AppUserName).FirstOrDefault() : "",
                        IssuedDate = scr.IssuedDate
                    }).ToList();
        }

        #endregion

        #region Enquiry Report
        public List<CallReportCountData> GetCallReports(EnquiryReportViewModel model, out long TotalRecords)
        {
            try
            {
                bool isSplit = false;
                string[] listStrLineElements = null;
                var Take = model.rows;
                var Skip = (model.page - 1) * model.rows;

                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
                string EmployeeAccessList = "";
                if (DbConstants.User.RoleKey == DbConstants.Role.Staff)
                {
                    if (Employee != null)
                    {
                        if (!DbConstants.Role.AdminUserTypes.Contains(DbConstants.User.RoleKey))
                        {
                            var ChildEmployees = dbContext.fnChildEmployees(DbConstants.User.UserKey).Where(row => (row.BranchKey == model.BranchKey || model.BranchKey == 0)).Select(row => new GroupSelectListModel
                            {
                                RowKey = row.RowKey ?? 0,
                                Text = row.EmployeeName,

                            }).OrderBy(row => row.Text).ToList();

                            List<long> branches = Employee.BranchAccess.ToString().Split(',').Select(Int64.Parse).ToList();

                            var OtherEmployees = dbContext.Employees.Where(row => branches.Contains(row.BranchKey) && row.BranchKey != Employee.BranchKey).Select(row => new GroupSelectListModel
                            {
                                RowKey = row.RowKey,
                                Text = row.FirstName + " " + (row.MiddleName ?? "") + " " + row.LastName,

                            }).OrderBy(row => row.Text).ToList();

                            EmployeeAccessList = String.Join(",", OtherEmployees.Select(x => x.RowKey).ToList().Union(ChildEmployees.Select(x => x.RowKey)));
                        }

                    }
                }

                string FromDate = model.SearchFromDate != null ? Convert.ToDateTime(model.SearchFromDate).ToString("yyyy-MM-dd") : null;
                string ToDate = model.SearchToDate != null ? Convert.ToDateTime(model.SearchToDate).ToString("yyyy-MM-dd") : null;

                ObjectParameter TotalRecordsCount = new ObjectParameter("TotalRecordCount", typeof(Int64));

                if (model.SearchFromDate != null || model.SearchToDate != null)
                {
                    if (model.SearchFromDate != null || model.SearchToDate != null)
                    {
                        DateTime StartDate = model.SearchFromDate == null ? DateTimeUTC.Now : Convert.ToDateTime(model.SearchFromDate);
                        DateTime EndDate = model.SearchToDate == null ? DateTimeUTC.Now : Convert.ToDateTime(model.SearchToDate);
                        double DateDiff = (EndDate.Date - StartDate.Date).TotalDays;

                        if (model.ScheduleTypeKeysList != null)
                        {
                            listStrLineElements = model.ScheduleTypeKeysList.Split(',').ToArray();
                        }

                        if (listStrLineElements == null)
                        {
                            listStrLineElements = ("1,2,3,4").ToString().Split(',').ToArray();
                            isSplit = true;
                        }
                        else if (listStrLineElements.Count() > 1)
                        {
                            isSplit = true;
                        }
                    }
                }

                dbContext.Database.CommandTimeout = 0;
                var EnquiryReportSplit = new List<CallReportCountData>();
                var EnquiryReport = new List<CallReportCountData>();
                var EmployeeDistinct = new List<CallReportCountData>();

                if (isSplit == false)
                {
                    EnquiryReport = dbContext.SpEnquiryReportSelect(
                         model.ScheduleTypeKeysList,
                         model.SearchDateTypeKey,
                         model.EmployeeFilterTypeKey,
                         DbConstants.EnquiryStatus.FollowUp,
                         DbConstants.EnquiryStatus.AdmissionTaken,
                         DbConstants.EnquiryStatus.Intersted,
                         DbConstants.EnquiryStatus.Closed,
                         DbConstants.ProductiveCalls.Limit,
                         model.SearchEmployeeKey,
                         model.SearchScheduledEmployeeKey,
                         model.SearchBranchKey,
                         model.SearchCounselllingBranchKey,
                         model.SearchCountryKey,
                         model.SearchInTakeKey,
                         model.SearchAcademicTermKey,
                         VerifyData(model.SearchLocation),
                         VerifyData(model.SearchAnyText),
                         model.SearchApplicationStatusKey,
                         model.ApplicationStatusKeysList,
                         model.SearchEnquiryStatusKey,
                         model.SearchCallStatusKey,
                         model.SearchCallTypeKey,
                         FromDate,
                         ToDate,
                         model.PageIndex,
                         model.PageSize,
                         TotalRecordsCount,
                          model.ReminderStatusKey,
                          model.SearchIsClose,
                          model.SearchIsClosePending,
                            model.SearchIsOnCallStatusVise,
                                  model.SearchSubEnquiryStatusKey,
                                  DbConstants.User.UserKey,
                                  DbConstants.User.RoleKey,
                                  EmployeeAccessList
                         ).Select(row => new CallReportCountData
                         {
                             EmployeeKey = row.EmployeeKey ?? 0,
                             EmployeeName = row.EmployeeName,
                             TotalEnquiryCount = row.TotalEnquiryCount ?? 0,
                             TotalFollowUpCount = row.TotalFollowUpCount ?? 0,
                             TotalAdmissionTakenCount = row.TotalAdmissionTakenCount ?? 0,
                             TotalIntrestedCount = row.TotalIntrestedCount ?? 0,
                             TotalClosedCount = row.TotalClosedCount ?? 0,
                             TotalProductiveCallsCount = row.TotalProductiveCallsCount ?? 0,
                             TotalCallsCount = row.TotalCallsCount ?? 0,
                             TotalRepeatedCallsCount = row.RepeatedCallsCount ?? 0
                         }).OrderByDescending(x => x.TotalFollowUpCount).ToList();

                    TotalRecords = EnquiryReport.Count();

                    return EnquiryReport.Skip(Skip).Take(Take).ToList();
                }
                else
                {
                    foreach (var ScheduleType in listStrLineElements)
                    {
                        model.ScheduleTypeKeysList = ScheduleType;

                        EnquiryReportSplit = dbContext.SpEnquiryReportSelect(
                         model.ScheduleTypeKeysList,
                         model.SearchDateTypeKey,
                         model.EmployeeFilterTypeKey,
                         DbConstants.EnquiryStatus.FollowUp,
                         DbConstants.EnquiryStatus.AdmissionTaken,
                         DbConstants.EnquiryStatus.Intersted,
                         DbConstants.EnquiryStatus.Closed,
                         DbConstants.ProductiveCalls.Limit,
                         model.SearchEmployeeKey,
                         model.SearchScheduledEmployeeKey,
                         model.SearchBranchKey,
                         model.SearchCounselllingBranchKey,
                         model.SearchCountryKey,
                         model.SearchInTakeKey,
                         model.SearchAcademicTermKey,
                         VerifyData(model.SearchLocation),
                         VerifyData(model.SearchAnyText),
                         model.SearchApplicationStatusKey,
                         model.ApplicationStatusKeysList,
                         model.SearchEnquiryStatusKey,
                         model.SearchCallStatusKey,
                         model.SearchCallTypeKey,
                         FromDate,
                         ToDate,
                         model.PageIndex,
                         model.PageSize,
                         TotalRecordsCount,
                         model.ReminderStatusKey,
                          model.SearchIsClose,
                          model.SearchIsClosePending,
                           model.SearchIsOnCallStatusVise,
                            model.SearchSubEnquiryStatusKey,
                            DbConstants.User.UserKey,
                            DbConstants.User.RoleKey,
                            EmployeeAccessList
                         ).Select(row => new CallReportCountData
                         {
                             EmployeeKey = row.EmployeeKey ?? 0,
                             EmployeeName = row.EmployeeName,
                             TotalEnquiryCount = row.TotalEnquiryCount ?? 0,
                             TotalFollowUpCount = row.TotalFollowUpCount ?? 0,
                             TotalAdmissionTakenCount = row.TotalAdmissionTakenCount ?? 0,
                             TotalIntrestedCount = row.TotalIntrestedCount ?? 0,
                             TotalClosedCount = row.TotalClosedCount ?? 0,
                             TotalProductiveCallsCount = row.TotalProductiveCallsCount ?? 0,
                             TotalCallsCount = row.TotalCallsCount ?? 0,
                             TotalRepeatedCallsCount = row.RepeatedCallsCount ?? 0
                         }).ToList();
                        EnquiryReport = EnquiryReport.Union(EnquiryReportSplit).ToList();

                    }

                    EnquiryReportSplit = EnquiryReportSplit.Select(row => new CallReportCountData
                    {
                        EmployeeKey = row.EmployeeKey,
                        EmployeeName = row.EmployeeName,
                        TotalEnquiryCount = EnquiryReport.Where(x => x.EmployeeKey == row.EmployeeKey).Sum(x => x.TotalEnquiryCount),
                        TotalFollowUpCount = EnquiryReport.Where(x => x.EmployeeKey == row.EmployeeKey).Sum(x => x.TotalFollowUpCount),
                        TotalAdmissionTakenCount = EnquiryReport.Where(x => x.EmployeeKey == row.EmployeeKey).Sum(x => x.TotalAdmissionTakenCount),
                        TotalIntrestedCount = EnquiryReport.Where(x => x.EmployeeKey == row.EmployeeKey).Sum(x => x.TotalIntrestedCount),
                        TotalClosedCount = EnquiryReport.Where(x => x.EmployeeKey == row.EmployeeKey).Sum(x => x.TotalClosedCount),
                        TotalProductiveCallsCount = EnquiryReport.Where(x => x.EmployeeKey == row.EmployeeKey).Sum(x => x.TotalProductiveCallsCount),
                        TotalRefundCount = EnquiryReport.Where(x => x.EmployeeKey == row.EmployeeKey).Sum(x => x.TotalRefundCount),
                        TotalCallsCount = EnquiryReport.Where(x => x.EmployeeKey == row.EmployeeKey).Sum(x => x.TotalCallsCount),
                        TotalRepeatedCallsCount = EnquiryReport.Where(x => x.EmployeeKey == row.EmployeeKey).Sum(x => x.TotalRepeatedCallsCount),

                    }).OrderByDescending(x => x.TotalFollowUpCount).ToList();

                    EnquiryReport = EnquiryReportSplit;
                    TotalRecords = EnquiryReport.Count();
                    return EnquiryReport.Skip(Skip).Take(Take).ToList();
                }


                //TotalRecords = EnquiryReport.ToList().Count;

            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.EnquiryCallSummary, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<CallReportCountData>();

            }
        }

        public List<EnquiryReportViewModel> GetCallReportsDetails(EnquiryReportViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.rows;
                var Skip = (model.page - 1) * model.rows;


                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
                string EmployeeAccessList = "";
                if (DbConstants.User.RoleKey == DbConstants.Role.Staff)
                {


                    if (Employee != null)
                    {

                        if (!DbConstants.Role.AdminUserTypes.Contains(DbConstants.User.RoleKey))
                        {

                            var ChildEmployees = dbContext.fnChildEmployees(DbConstants.User.UserKey).Where(row => (row.BranchKey == model.BranchKey || model.BranchKey == 0)).Select(row => new GroupSelectListModel
                            {
                                RowKey = row.RowKey ?? 0,
                                Text = row.EmployeeName,

                            }).OrderBy(row => row.Text).ToList();

                            List<long> branches = Employee.BranchAccess.ToString().Split(',').Select(Int64.Parse).ToList();

                            var OtherEmployees = dbContext.Employees.Where(row => branches.Contains(row.BranchKey) && row.BranchKey != Employee.BranchKey).Select(row => new GroupSelectListModel
                            {
                                RowKey = row.RowKey,
                                Text = row.FirstName + " " + (row.MiddleName ?? "") + " " + row.LastName,

                            }).OrderBy(row => row.Text).ToList();

                            EmployeeAccessList = String.Join(",", OtherEmployees.Select(x => x.RowKey).ToList().Union(ChildEmployees.Select(x => x.RowKey)));
                        }

                    }
                }




                string FromDate = model.SearchFromDate != null ? Convert.ToDateTime(model.SearchFromDate).ToString("yyyy-MM-dd") : null;
                string ToDate = model.SearchToDate != null ? Convert.ToDateTime(model.SearchToDate).ToString("yyyy-MM-dd") : null;

                ObjectParameter TotalRecordsCount = new ObjectParameter("TotalRecordCount", typeof(Int64));
                dbContext.Database.CommandTimeout = 0;
                var EnquiryReport = dbContext.SpEnquiryReportSelectDetails(
                      model.ScheduleTypeKeysList,
                      model.SearchDateTypeKey,
                      model.EmployeeFilterTypeKey,
                      DbConstants.EnquiryStatus.FollowUp,
                      DbConstants.EnquiryStatus.AdmissionTaken,
                      DbConstants.EnquiryStatus.Intersted,
                      DbConstants.EnquiryStatus.Closed,
                      DbConstants.ProductiveCalls.Limit,
                      model.SearchEmployeeKey,
                      model.SearchScheduledEmployeeKey,
                      model.SearchBranchKey,
                      model.SearchCounselllingBranchKey,
                      model.SearchCountryKey,
                      model.SearchInTakeKey,
                      model.SearchAcademicTermKey,
                      VerifyData(model.SearchLocation),
                      VerifyData(model.SearchAnyText),
                      model.SearchApplicationStatusKey,
                      model.ApplicationStatusKeysList,
                      model.SearchEnquiryStatusKey,
                      model.SearchCallStatusKey,
                      model.SearchCallTypeKey,
                      FromDate,
                      ToDate,
                      model.PageIndex,
                      model.PageSize,
                      TotalRecordsCount,
                      model.ReminderStatusKey,
                      model.SearchIsClose,
                      model.SearchIsClosePending,
                      model.SearchIsOnCallStatusVise,
                      model.SearchSubEnquiryStatusKey,
                      DbConstants.User.UserKey,
                      DbConstants.User.RoleKey,
                      EmployeeAccessList
                      ).Select(row => new EnquiryReportViewModel
                      {
                          RowNumber = row.RowNumber ?? 0,
                          EmployeeName = row.EmployeeName,
                          Name = row.Name,
                          MobileNumber = row.MobileNumber,
                          Email = row.Email,
                          Qualificatin = row.Qualification,
                          Branch = row.Branch,
                          AcademicTermName = row.ServiceTypeName,
                          Country = row.Country,
                          Program = row.Program,
                          District = row.District,
                          Location = row.Location,

                          CallStatusName = row.LastCallStatus,
                          ScheduleTypeName = row.ScheduleTypeName,
                          NextCallSchedule = row.NextCallSchedule,
                          CallTypeName = row.CallTypeName,
                          Feedback = row.CallFeedback,
                          CallDuration = row.CallDuration,
                          CalledDate = row.CalledDate,
                          EmployeeKey = row.EmployeeKey ?? 0,
                          PostedBy = row.CreatedByEmployeeName,
                          ScheduledEmployeeName = row.ScheduledEmployeeName,
                          IsClosePending = row.IsClose ?? false,
                          StatusName = row.StatusName,
                          CounsellingBranchName = row.CounsellingBranchName,
                          EnquiryStatusOnCall = row.EnquiryStatusNameOnCall,

                          EnquiryStatusKey = row.EnquiryStatusKey,
                          ApplicationStatusName = row.ApplicationStatusName,
                          CounsellingTime = row.CounsellingTime,
                          //EnquiryCounsellingDate = row.EnquiryCounsellingDate,
                          //EnquiryCounsellingCalledDate = row.EnquiryCounsellingCalledDate,
                          CreatedOn = row.CreatedOn,
                          ApplicationStatusKey = row.ApplicationStatusKey,
                          TelephoneCodeKey = row.TelephoneCodeKey,
                          RowKey = row.RowKey ?? 0

                      }).ToList();

                model.TotalRecordsCount = TotalRecordsCount.Value != DBNull.Value ? Convert.ToInt64(TotalRecordsCount.Value) : 0;

                //foreach (var item in EnquiryReport)
                //{
                //    if (item.ScheduleTypeKey == DbConstants.ScheduleTypes.EnquiryLead)
                //    {
                //        item.FeedbackList = (from ef in dbContext.EnquiryLeadFeedbacks
                //                             join au in dbContext.AppUsers on ef.AddedBy equals au.RowKey
                //                             where ef.EnquiryLeadKey==item.RowKey
                //                             select new CallFeedbacksList
                //                             {
                //                                 NextCallSchedule = ef.NextCallSchedule,
                //                                 CallDuration = ef.CallDuration,
                //                                 CalledDate = ef.DateAdded,
                //                                 AddedBy = au.FirstName + " " + (au.MiddleName ?? "") + " " + au.LastName,
                //                                 CallStatusName = ef.EnquiryCallStatu.EnquiryCallStatusName,
                //                                 CallTypeName = ef.CallType.CallTypeName,
                //                                 ScheduleTypeName = EduSuiteUIResources.EnquiryLead


                //                             }).OrderByDescending(x=>x.CalledDate).Take(1).ToList();

                //    }
                //    else if (item.ScheduleTypeKey == DbConstants.ScheduleTypes.Enquiry)
                //    {
                //        item.FeedbackList = (from ef in dbContext.EnquiryFeedbacks
                //                             join au in dbContext.AppUsers on ef.AddedBy equals au.RowKey
                //                             where ef.EnquiryKey == item.RowKey
                //                             select new CallFeedbacksList
                //                             {
                //                                 NextCallSchedule = ef.EnquiryFeedbackReminderDate,
                //                                 CallDuration = ef.EnquiryDuration,
                //                                 CalledDate = ef.DateAdded,
                //                                 AddedBy = au.FirstName + " " + (au.MiddleName ?? "") + " " + au.LastName,
                //                                 CallStatusName = ef.EnquiryCallStatu.EnquiryCallStatusName,
                //                                 CallTypeName = ef.CallType.CallTypeName,
                //                                 ScheduleTypeName = EduSuiteUIResources.Enquiry


                //                             }).OrderByDescending(x => x.CalledDate).Take(1).ToList();
                //    }
                //    else if (item.ScheduleTypeKey == DbConstants.ScheduleTypes.Application)
                //    {
                //        item.FeedbackList = (from ef in dbContext.ApplicationSchedules
                //                             join au in dbContext.AppUsers on ef.AddedBy equals au.RowKey
                //                             where ef.ApplicationKey == item.RowKey
                //                             select new CallFeedbacksList
                //                             {
                //                                 NextCallSchedule = ef.ReminderDate,
                //                                 CallDuration = ef.Duration,
                //                                 CalledDate = ef.DateAdded,
                //                                 AddedBy = au.FirstName + " " + (au.MiddleName ?? "") + " " + au.LastName,
                //                                 CallStatusName = ef.EnquiryCallStatu.EnquiryCallStatusName,
                //                                 CallTypeName = ef.CallType.CallTypeName,
                //                                 ScheduleTypeName = EduSuiteUIResources.Application


                //                             }).OrderByDescending(x => x.CalledDate).Take(1).ToList();
                //    }

                //}

                //TotalRecords = EnquiryReport.ToList().Count;
                TotalRecords = 0;


                //return EnquiryReport.Skip(Skip).Take(Take).ToList();
                return EnquiryReport.ToList();
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                return new List<EnquiryReportViewModel>();
                //ActivityLog.CreateActivityLog(MenuConstants.Application, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);

            }
        }
        private string VerifyData(string Data)
        {
            if (Data != null && Data != "")
            {
                Data = "%" + Data + "%";
            }
            else
            {
                Data = "%";
            }
            return Data;
        }
        public void FillDropdownLists(EnquiryReportViewModel model)
        {
            FillApplicationStatus(model);
            FillScheduleTypes(model);
            FillBranches(model);
            FillServiceTypes(model);
            FillTelephoneCodes(model);
            FillNatureOfEnquiries(model);
            GetEmployeesByBranchId(model);

            FillCallStatuses(model);
            FillDateTypes(model);
            EmployeeFilterTypes(model);
            GetCallTypes(model);
            FillEnquiryStatus(model);

        }
        private void FillApplicationStatus(EnquiryReportViewModel model)
        {
            model.ApplicationStatuses = dbContext.StudentStatus.Where(row => row.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.StudentStatusName
            }).ToList();
        }
        private void FillScheduleTypes(EnquiryReportViewModel model)
        {
            model.ScheduleTypes.Add(new SelectListModel
            {
                RowKey = DbConstants.ScheduleTypes.EnquiryLead,
                Text = EduSuiteUIResources.EnquiryLead
            });
            model.ScheduleTypes.Add(new SelectListModel
            {
                RowKey = DbConstants.ScheduleTypes.Enquiry,
                Text = EduSuiteUIResources.Enquiry
            });

            model.ScheduleTypes.Add(new SelectListModel
            {
                RowKey = DbConstants.ScheduleTypes.Application,
                Text = EduSuiteUIResources.Application
            });

        }
        private void EmployeeFilterTypes(EnquiryReportViewModel model)
        {


            model.EmployeeFilterTypes.Add(new SelectListModel
            {
                RowKey = DbConstants.EmployeeFilterTypes.CounsellingBy,
                Text = EduSuiteUIResources.CounsellingBy
            });

            model.EmployeeFilterTypes.Add(new SelectListModel
            {
                RowKey = DbConstants.EmployeeFilterTypes.ScheduledBy,
                Text = EduSuiteUIResources.ScheduledBy
            });
            model.EmployeeFilterTypes.Add(new SelectListModel
            {
                RowKey = DbConstants.EmployeeFilterTypes.CalledBy,
                Text = EduSuiteUIResources.CalledBy
            });


        }
        private void FillDateTypes(EnquiryReportViewModel model)
        {
            model.DateTypes.Add(new SelectListModel
            {
                RowKey = DbConstants.DateTypes.CalledDate,
                Text = EduSuiteUIResources.CalledDate
            });
            model.DateTypes.Add(new SelectListModel
            {
                RowKey = DbConstants.DateTypes.NextScheduleDate,
                Text = EduSuiteUIResources.NextScheduleDate
            });

            model.DateTypes.Add(new SelectListModel
            {
                RowKey = DbConstants.DateTypes.CreatedDate,
                Text = EduSuiteUIResources.CreatedDate
            });

            model.DateTypes.Add(new SelectListModel
            {
                RowKey = DbConstants.DateTypes.EnquiryCounsellingDate,
                Text = EduSuiteUIResources.EnquiryCounsellingDate
            });

            model.DateTypes.Add(new SelectListModel
            {
                RowKey = DbConstants.DateTypes.EnquiryCounsellingCalledDate,
                Text = EduSuiteUIResources.EnquiryCounsellingCalledDate
            });

            model.DateTypes.Add(new SelectListModel
            {
                RowKey = DbConstants.DateTypes.AddedToEnquiryDate,
                Text = EduSuiteUIResources.AddedToEnquiryDate
            });

            model.DateTypes.Add(new SelectListModel
            {
                RowKey = DbConstants.DateTypes.EnquiryFetchDate,
                Text = EduSuiteUIResources.EnquiryFetchDate
            });


        }
        private void FillBranches(EnquiryReportViewModel model)
        {

            IQueryable<SelectListModel> BranchQuery = dbContext.vwBranchSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BranchName
            });


            if (!DbConstants.Role.AdminUserTypes.Contains(DbConstants.User.RoleKey))
            {
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();

                if (Employee != null)
                {
                    List<long> branches = Employee.BranchAccess.ToString().Split(',').Select(Int64.Parse).ToList();
                    model.Branches = BranchQuery.Where(x => branches.Contains(x.RowKey)).ToList();
                    //model.BranchKey = Employee.BranchKey;
                    //model.SearchBranchKey = Employee.BranchKey;
                }
                else
                {
                    model.Branches = BranchQuery.ToList();
                }

            }
            else
            {
                if (DbConstants.User.RoleKey == DbConstants.AdminKey)
                {
                    model.Branches = BranchQuery.ToList();
                }
                else
                {
                    Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
                    List<long> branches = Employee.BranchAccess.ToString().Split(',').Select(Int64.Parse).ToList();
                    model.Branches = BranchQuery.Where(x => branches.Contains(x.RowKey)).ToList();
                }
            }




        }
        public EnquiryReportViewModel GetCallTypes(EnquiryReportViewModel model)
        {

            model.CallTypes = dbContext.CallTypes.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.CallTypeName
            }).GroupBy(row => row.RowKey).Select(row => row.FirstOrDefault()).ToList();
            return model;
        }
        public EnquiryReportViewModel GetEmployeesByBranchId(EnquiryReportViewModel model)
        {
            //Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
            //if (!DbConstants.Role.AdminUserTypes.Contains(DbConstants.User.RoleKey))
            //{
            //    if (Employee != null)
            //    {
            //        if (model.BranchKey == Employee.BranchKey)
            //        {
            //            model.Employees = dbContext.fnChildEmployees(DbConstants.User.UserKey).Where(row => (row.BranchKey == model.BranchKey || model.BranchKey == 0)).Select(row => new SelectListModel
            //            {
            //                RowKey = row.RowKey ?? 0,
            //                Text = row.EmployeeName,

            //            }).OrderBy(row => row.Text).ToList();

            //            model.SearchEmployeeKey = Employee.RowKey;
            //        }
            //        else
            //        {
            //            if ((model.BranchKey != Employee.BranchKey) && model.BranchKey != 0)
            //            {
            //                model.Employees = dbContext.Employees.Where(row => row.BranchKey == model.BranchKey && row.EmployeeStatusKey == DbConstants.EmployeeStatus.Working).Select(row => new SelectListModel
            //                {
            //                    RowKey = row.RowKey,
            //                    Text = row.FirstName + " " + (row.MiddleName ?? "") + " " + row.LastName,
            //                }).OrderBy(row => row.Text).ToList();
            //            }
            //            else
            //            {
            //                var Branches = Employee.BranchAccess.Split(',').Select(Int16.Parse).ToList();

            //                var Employee1 = dbContext.Employees.Where(row => Branches.Contains(row.BranchKey) && row.BranchKey != Employee.BranchKey && row.EmployeeStatusKey == DbConstants.EmployeeStatus.Working).Select(row => new SelectListModel
            //                {
            //                    RowKey = row.RowKey,
            //                    Text = row.FirstName + " " + (row.MiddleName ?? "") + " " + row.LastName,
            //                }).ToList();
            //                var Employee2 = dbContext.fnChildEmployees(DbConstants.User.UserKey).Select(row => new SelectListModel
            //                {
            //                    RowKey = row.RowKey ?? 0,
            //                    Text = row.EmployeeName,

            //                }).ToList();
            //                model.Employees = Employee1.Union(Employee2).OrderBy(row => row.Text).ToList();
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    if (DbConstants.User.RoleKey == DbConstants.AdminKey)
            //    {
            //        model.Employees = dbContext.Employees.Where(row => row.EmployeeStatusKey == DbConstants.EmployeeStatus.Working && (row.BranchKey == model.BranchKey || model.BranchKey == 0)).Select(row => new SelectListModel
            //        {
            //            RowKey = row.RowKey,
            //            Text = row.FirstName + " " + (row.MiddleName ?? "") + " " + row.LastName,
            //        }).OrderBy(row => row.Text).ToList();
            //    }
            //    else
            //    {
            //        if (model.BranchKey != 0)
            //        {
            //            Employee.BranchAccess = model.BranchKey.ToString();
            //        }
            //        var Branches = Employee.BranchAccess.Split(',').Select(Int16.Parse).ToList();

            //        model.Employees = dbContext.Employees.Where(row => Branches.Contains(row.BranchKey) && row.EmployeeStatusKey == DbConstants.EmployeeStatus.Working).Select(row => new SelectListModel
            //        {
            //            RowKey = row.RowKey,
            //            Text = row.FirstName + " " + (row.MiddleName ?? "") + " " + row.LastName,
            //        }).OrderBy(row => row.Text).ToList();
            //    }
            //}


            Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();

            if (DbConstants.User.UserKey != DbConstants.AdminKey)
            {
                if (Employee != null)
                {
                    var Branches = Employee.BranchAccess.Split(',').Select(Int16.Parse).ToList();

                    if (Branches.Count > 1)
                    {
                        model.Employees = dbContext.Employees.Where(row => row.EmployeeStatusKey == DbConstants.EmployeeStatus.Working && row.IsActive == true && (row.BranchKey == model.BranchKey || model.BranchKey == 0)).Select(row => new SelectListModel
                        {
                            RowKey = row.RowKey,
                            Text = row.FirstName + " " + (row.MiddleName ?? "") + " " + row.LastName


                        }).OrderBy(row => row.Text).ToList();
                    }
                    else
                    {
                        model.Employees = dbContext.Employees.Where(row => row.EmployeeStatusKey == DbConstants.EmployeeStatus.Working && row.IsActive == true && (row.BranchKey == model.BranchKey || model.BranchKey == 0) && row.RowKey == Employee.RowKey).Select(row => new SelectListModel
                        {
                            RowKey = row.RowKey,
                            Text = row.FirstName + " " + (row.MiddleName ?? "") + " " + row.LastName

                        }).OrderBy(row => row.Text).ToList();
                    }

                    model.SearchEmployeeKey = model.EmployeeKey = Employee.RowKey;
                }
            }
            else
            {
                model.Employees = dbContext.Employees.Where(row => row.EmployeeStatusKey == DbConstants.EmployeeStatus.Working && (row.BranchKey == model.BranchKey || model.BranchKey == 0)).Select(row => new SelectListModel
                {
                    RowKey = row.RowKey,
                    Text = row.FirstName + " " + (row.MiddleName ?? "") + " " + row.LastName

                }).OrderBy(row => row.Text).ToList();

                if (Employee != null)
                {
                    model.SearchEmployeeKey = model.EmployeeKey = Employee.RowKey;
                }
            }
            return model;
        }
        private void FillAgents(EnquiryReportViewModel model)
        {

            model.Agents = dbContext.Agents.Where(row => row.AgentActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AgentName
            }).ToList();
        }
        private void FillServiceTypes(EnquiryReportViewModel model)
        {
            model.AcademicTerms = dbContext.AcademicTerms.Where(row => row.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AcademicTermName
            }).ToList();
        }
        private void FillReligions(EnquiryReportViewModel model)
        {
            model.Religions = dbContext.Religions.Where(row => row.IsActive).OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ReligionName
            }).ToList();
        }
        private void FillBatches(EnquiryReportViewModel model)
        {
            model.Batches = dbContext.Batches.Where(row => row.IsActive).OrderBy(row => row.RowKey).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BatchName
            }).ToList();
        }
        private void FillNatureOfEnquiries(EnquiryReportViewModel model)
        {
            model.NatureOfEnquiries = dbContext.NatureOfEnquiries.Where(row => row.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.NatureOfEnquiryName
            }).ToList();
        }
        private void FillTelephoneCodes(EnquiryReportViewModel model)
        {
            model.TelephoneCodes = dbContext.VwCountrySelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.TelephoneCode
            }).ToList();
        }
        public void FillCallStatuses(EnquiryReportViewModel model)
        {

            model.EnquiryCallStatuses = dbContext.EnquiryCallStatus.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.EnquiryCallStatusName
            }).ToList();
        }
        public void FillEnquiryStatus(EnquiryReportViewModel model)
        {

            model.EnquiryStatus = dbContext.EnquiryStatus.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.EnquiryStatusName
            }).ToList();
        }
        #endregion Enquiry Report

        #region Activitylog Summary

        public void FillAppUsers(ReportViewModel model)
        {
            model.AppUsers = dbContext.AppUsers.Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.AppUserName
            }).ToList();


        }
        public void FillMenus(ReportViewModel model)
        {
            model.Menus = dbContext.Menus.Where(x => x.IsActive).Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.MenuName
            }).ToList();


        }

        public List<dynamic> GetActivityLogReport(ReportViewModel model, out long TotalRecords)
        {
            try
            {
                List<dynamic> ActivityLogReportsList = new List<dynamic>();
                DbParameter TotalRecordsParam = null;

                if (model.sidx != "")
                {
                    model.sidx = model.sidx + " " + model.sord;
                }
                dbContext.LoadStoredProc("dbo.Sp_ActivityLogSummary")
                    .WithSqlParam("@UserKey", DbConstants.User.UserKey)
                    .WithSqlParam("@RoleKey", DbConstants.User.RoleKey)
                    .WithSqlParam("@UserKeyList", String.Join(",", model.UserKeys))
                    .WithSqlParam("@MenuKeyList", String.Join(",", model.MenuKeys))
                    .WithSqlParam("@DateAddedFrom", (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@DateAddedTo", (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@PageIndex", model.page)
                    .WithSqlParam("@PageSize", model.rows)
                    .WithSqlParam("@SortBy", model.sidx)
                    .WithSqlParam("@TotalRecords", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalRecordsParam = dbParam;
                    }).ExecuteStoredProc((handler) =>
                    {
                        ActivityLogReportsList = handler.ReadToDynamicList<dynamic>() as List<dynamic>;

                    });
                TotalRecords = Convert.ToInt64((TotalRecordsParam.Value ?? 0));
                return ActivityLogReportsList;
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.ActivityLog, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<dynamic>();
            }
        }

        #endregion Activitylog Summary

        #region UnitTest Exam Result
        public ReportViewModel FillSubjectModules(ReportViewModel model)
        {
            IQueryable<SelectListModel> SubjectModulesQuery = dbContext.SubjectModules.OrderBy(row => row.ModuleName).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ModuleName
            });
            if (model.SubjectKeys.Count > 0)
            {
                model.SubjectModules = SubjectModulesQuery.Where(row => model.SubjectKeys.Contains(row.RowKey)).ToList();
            }
            else
            {
                model.SubjectModules = dbContext.SubjectModules.Where(x => x.IsActive == true).Select(row => new SelectListModel
                {
                    RowKey = row.RowKey,
                    Text = row.ModuleName
                }).ToList();
            }

            return model;
        }

        public List<dynamic> GetUnitTestExamResultSummary(ReportViewModel model, out long TotalRecords)
        {
            try
            {
                List<dynamic> applicationList = new List<dynamic>();
                DbParameter TotalRecordsParam = null;

                if (model.sidx != "")
                {
                    model.sidx = model.sidx + " " + model.sord;
                }
                dbContext.LoadStoredProc("dbo.Sp_UnitExamResultSummary")

                    .WithSqlParam("@CourseKeyList", String.Join(",", model.CourseKeys))
                    .WithSqlParam("@CourseTypeKeyList", String.Join(",", model.CourseTypeKeys))
                    .WithSqlParam("@UniversityMasterKeyList", String.Join(",", model.UniversityMasterKeys))
                    .WithSqlParam("@BatchKeyList", String.Join(",", model.BatchKeys))

                    .WithSqlParam("@BranchKeyList", String.Join(",", model.BranchKeys))
                    .WithSqlParam("@ClassDetailsKeyList", String.Join(",", model.ClassKeys))
                    .WithSqlParam("@AcademicTermKey", model.AcademicTermKey)
                    .WithSqlParam("@CourseYearsKeyList", String.Join(",", model.CourseYearsKeys))

                    .WithSqlParam("@DateAddedFrom", (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@DateAddedTo", (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""))

                    .WithSqlParam("@SubjectKeyList", String.Join(",", model.SubjectKeys))
                    .WithSqlParam("@SubjectModuleKeyList", String.Join(",", model.SubjectModuleKeys))
                    .WithSqlParam("@ModuleTopicKeyList", String.Join(",", model.SubjectModuleKeys))
                    .WithSqlParam("@UserKey", DbConstants.User.UserKey)
                    .WithSqlParam("@PageIndex", model.page)
                    .WithSqlParam("@PageSize", model.rows)
                    .WithSqlParam("@SortBy", model.sidx)
                    .WithSqlParam("@TotalRecords", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalRecordsParam = dbParam;
                    }).ExecuteStoredProc((handler) =>
                    {
                        applicationList = handler.ReadToDynamicList<dynamic>() as List<dynamic>;
                        //applicationList = handler.ReadToList<ApplicationViewModel>() as List<ApplicationViewModel>;
                    });

                TotalRecords = Convert.ToInt64((TotalRecordsParam.Value ?? 0));
                return applicationList;
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.UnitTestExamResultSummary, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<dynamic>();
            }
        }

        public List<UnitTestResultViewModel> BindUnitTestStudentMarkDetails(ReportViewModel model)
        {
            try
            {
                List<UnitTestResultViewModel> InternalExamResultDetails = new List<UnitTestResultViewModel>();


                //InternalExamResultDetails = (from App in dbContext.Applications
                //                             join SDA in dbContext.StudentDivisionAllocations on App.RowKey equals SDA.ApplicationKey
                //                             join UTS in dbContext.UnitTestSchedules on new { App.BatchKey, ClassDetailsKey = App.ClassDetailsKey ?? 0, App.BranchKey }
                //                             equals new { UTS.BatchKey, UTS.ClassDetailsKey, UTS.BranchKey }
                //                             join UTR in dbContext.UnitTestResults on new { ApplicationKey = App.RowKey, UnitTestScheduleKey = UTS.RowKey }
                //                             equals new { UTR.ApplicationKey, UTR.UnitTestScheduleKey } into UTRD
                //                             from IER in UTRD.DefaultIfEmpty()
                //                             where (App.StudentStatusKey == DbConstants.StudentStatus.Ongoing && SDA.IsActive == true &&
                //                             UTS.RowKey == model.UnitTestScheduledKey && UTS.SubjectKey == model.SubjectKey && (UTS.ClassDetailsKey != null ? UTS.ClassDetailsKey : App.ClassDetailsKey) == model.ClassDetailsKey)
                //                             select new UnitTestResultViewModel
                //                             {
                //                                 ApplicationKey = App.RowKey,
                //                                 StudentName = App.StudentName,
                //                                 AdmissionNo = App.AdmissionNo,
                //                                 UnitTestScheduleKey = UTS.RowKey,
                //                                 ResultStatus = IER.ResultStatus,
                //                                 Mark = IER.Mark,
                //                                 MaximumMarks = UTS.MaximumMark,
                //                                 MinimumMarks = UTS.MinimumMark,
                //                                 Remarks = IER.Remarks,
                //                             }).ToList();

                //InternalExamResultDetails = dbContext.UnitTestResults.Where(x => x.UnitTestScheduleKey == model.UnitTestScheduledKey).Select(x => new UnitTestResultViewModel
                //{
                //    RowKey = x.RowKey,
                //    UnitTestScheduleKey = x.UnitTestScheduleKey,
                //    ApplicationKey = x.Application.RowKey,
                //    StudentName = x.Application.StudentName,
                //    AdmissionNo = x.Application.AdmissionNo,
                //    ResultStatus = x.ResultStatus,
                //    Mark = x.Mark,
                //    MaximumMarks = x.UnitTestSchedule.MaximumMark,
                //    MinimumMarks = x.UnitTestSchedule.MinimumMark,
                //    Remarks = x.Remarks,
                //    AbsentStatus = (x.ResultStatus == DbConstants.ResultStatus.Absent ? true : false),
                //    //TopicNames = x.UnitTestSchedule.UnitTestTopics.Select(p => new { p.ModuleTopic.TopicName, InternalExamResultDetails = String.Join(",", p.InternalExamResultDetails.or })
                //}).ToList();

                var query = (from a in dbContext.UnitTestTopics.ToList()
                             where a.UnitTestScheduleKey == model.UnitTestScheduledKey
                             group a by a.UnitTestScheduleKey into g
                             select new
                             {
                                 UnitTestScheduleKey = g.Key,
                                 TopicName = string.Join(",", g.Select(x =>
                                 x.ModuleTopic.TopicName))
                             });

                var query2 = (from a in query
                              join x in dbContext.UnitTestResults on a.UnitTestScheduleKey equals x.UnitTestScheduleKey
                              where a.UnitTestScheduleKey == model.UnitTestScheduledKey
                              select new UnitTestResultViewModel
                              {
                                  RowKey = x.RowKey,
                                  UnitTestScheduleKey = x.UnitTestScheduleKey,
                                  ApplicationKey = x.Application.RowKey,
                                  StudentName = x.Application.StudentName,
                                  AdmissionNo = x.Application.AdmissionNo,
                                  ResultStatus = x.ResultStatus,
                                  Mark = x.Mark,
                                  MaximumMarks = x.UnitTestSchedule.MaximumMark,
                                  MinimumMarks = x.UnitTestSchedule.MinimumMark,
                                  Remarks = x.Remarks,
                                  AbsentStatus = (x.ResultStatus == DbConstants.ResultStatus.Absent ? true : false),
                                  TopicNames = a.TopicName
                              });
                InternalExamResultDetails = query2.ToList();
                return InternalExamResultDetails;
            }
            catch (Exception ex)
            {
                return new List<UnitTestResultViewModel>();
            }
        }
        #endregion UnitTest Exam Result

        #region Exam Schedule Summary

        //public List<ReportViewModel> GetStudentExamScheduleSummary(ReportViewModel model)
        //{
        //    ObjectParameter objTotalRecords = new ObjectParameter("TotalRecords", typeof(Int64));



        //    if (model.sidx != "")
        //    {
        //        model.sidx = model.sidx + " " + model.sord;
        //    }

        //    var StudetExamscheduleSummaryReports = (
        //                                 from Application in dbContext.Sp_StudentExamSchedule_Summary
        //                                     (
        //                                        String.Join(",", model.CourseKeys),
        //                                       String.Join(",", model.CourseTypeKeys),
        //                                       String.Join(",", model.UniversityMasterKeys),
        //                                       String.Join(",", model.BatchKeys),
        //                                       String.Join(",", model.ModeKeys),
        //                                       String.Join(",", model.ClassModeKeys),
        //                                       String.Join(",", model.BranchKeys),
        //                                       String.Join(",", model.StudentStatusKeys),
        //                                       String.Join(",", model.MeadiumKeys),
        //                                       String.Join(",", model.ClassKeys),
        //                                        model.AcademicTermKey,
        //                                       String.Join(",", model.CourseYearsKeys),
        //                                        model.ClassRequiredKey,
        //                                        (model.DateOfAdmission != null ? Convert.ToDateTime(model.DateOfAdmission).ToString("yyyy-MM-dd") : ""),
        //                                        (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""),
        //                                        (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""),

        //                                        model.ExamStatus,
        //                                        model.ExamResultStatus,
        //                                         model.SearchAnyText.VerifyData(),
        //                                        model.page,
        //                                        model.rows,
        //                                        model.sidx,
        //                                        objTotalRecords
        //                                     )
        //                                 select new ReportViewModel
        //                                 {
        //                                     RowKey = Application.RowKey
        //                                        ,
        //                                     ApplicationNo = Application.ApplicationNo
        //                                        ,
        //                                     Name = Application.StudentName
        //                                        ,
        //                                     GuardianName = Application.StudentGuardian
        //                                        ,
        //                                     MotherName = Application.StudentMotherName
        //                                        ,
        //                                     PermanentAddress = Application.StudentPermanentAddress
        //                                        ,
        //                                     PresentAddress = Application.StudentPresentAddress
        //                                        ,
        //                                     Email = Application.StudentEmail
        //                                        ,
        //                                     Phone = Application.StudentPhone
        //                                        ,
        //                                     Mobile = Application.StudentMobile
        //                                        ,
        //                                     DOB = Application.StudentDOB
        //                                        ,
        //                                     StartYear = Application.StartYear
        //                                        ,
        //                                     TotalFee = Application.StudentTotalFee
        //                                        ,
        //                                     ClassRequiredDesc = Application.StudentClassRequiredDesc
        //                                        ,
        //                                     PresentJob_CourseOfStudyId = Application.PresentJob_CourseOfStudyId
        //                                        ,
        //                                     DateOfAdmission = Application.StudentDateOfAdmission
        //                                        ,
        //                                     StudyMaterialIssueStatus = Application.StudyMaterialIssueStatus
        //                                        ,
        //                                     EnrollmentNo = Application.StudentEnrollmentNo
        //                                        ,
        //                                     PhotoPath = Application.StudentPhotoPath
        //                                        ,
        //                                     ExamRegisterNo = Application.ExamRegisterNo
        //                                        ,
        //                                     Remarks = Application.Remarks
        //                                        ,
        //                                     AdmissionNo = Application.AdmissionNo
        //                                        ,
        //                                     SerialNumber = Application.SerialNumber
        //                                        ,
        //                                     CurrentYear = Application.CurrentYear
        //                                        ,
        //                                     RollNumber = Application.RollNumber
        //                                        ,
        //                                     AcademicTermKey = Application.AcademicTermKey
        //                                        ,
        //                                     DateAdded = Application.DateAdded
        //                                        ,
        //                                     Course = Application.CourseName
        //                                        ,
        //                                     CourseType = Application.CourseTypeName
        //                                        ,
        //                                     Affiliations = Application.UniversityMasterName
        //                                        ,
        //                                     Batch = Application.BatchName
        //                                        ,
        //                                     Mode = Application.ModeName
        //                                        ,
        //                                     ClassMode = Application.ClassModeName
        //                                        ,
        //                                     Religion = Application.ReligionName
        //                                        ,
        //                                     AcademicTerm = Application.AcademicTermName
        //                                        ,
        //                                     SecondLanguage = Application.SecondLanguageName
        //                                        ,
        //                                     Medium = Application.MediumName
        //                                        ,
        //                                     Income = Application.IncomeName
        //                                        ,
        //                                     NatureOfEnquiry = Application.NatureOfEnquiryName
        //                                        ,
        //                                     Agent = Application.AgentName
        //                                        ,
        //                                     StudentStatus = Application.StudentStatusName
        //                                        ,
        //                                     BranchName = Application.BranchName
        //                                        ,
        //                                     Class = Application.ClassCode
        //                                        ,
        //                                     Gender = Application.StudentGender == 1 ? "Male" : "Female"
        //                                        ,
        //                                     ClassRequiredKey = Application.ClassRequired ? 1 : 0
        //                                        ,
        //                                     ClassRequired = Application.ClassRequired == true ? EduSuiteUIResources.Yes : EduSuiteUIResources.No
        //                                        ,
        //                                     TotalBooks = Application.TotalSubjects
        //                                        ,
        //                                     AppliedSubject = Application.AppliedSubject
        //                                        ,
        //                                     Passed = Application.Passed
        //                                        ,
        //                                     Failed = Application.Failed
        //                                        ,
        //                                     Absent = Application.Absent
        //                                        ,
        //                                     CurrentYearText = CommonUtilities.GetYearDescriptionByCodeDetails(Application.CourseDuration ?? 0, Application.CurrentYear, Application.AcademicTermKey)


        //                                 }).ToList();



        //    model.TotalRecords = objTotalRecords.Value != DBNull.Value ? Convert.ToInt64(objTotalRecords.Value) : 0;

        //    return StudetExamscheduleSummaryReports;
        //}

        public List<dynamic> GetStudentExamScheduleSummary(ReportViewModel model, out long TotalRecords)
        {
            try
            {
                List<dynamic> applicationList = new List<dynamic>();
                DbParameter TotalRecordsParam = null;

                if (model.sidx != "")
                {
                    model.sidx = model.sidx + " " + model.sord;
                }
                dbContext.LoadStoredProc("dbo.Sp_StudentExamSchedule_Summary")

                    .WithSqlParam("@CourseKeyList", String.Join(",", model.CourseKeys))
                    .WithSqlParam("@CourseTypeKeyList", String.Join(",", model.CourseTypeKeys))
                    .WithSqlParam("@UniversityMasterKeyList", String.Join(",", model.UniversityMasterKeys))
                    .WithSqlParam("@BatchKeyList", String.Join(",", model.BatchKeys))
                    .WithSqlParam("@ReligionKeyList", String.Join(",", model.ReligionKeys))
                    .WithSqlParam("@IncomeKeyList", String.Join(",", model.IncomeGroupKeys))
                    .WithSqlParam("@SecondLanguageKeyList", String.Join(",", model.SecondLanguageKeys))
                    .WithSqlParam("@ModeKeyList", String.Join(",", model.ModeKeys))
                    .WithSqlParam("@ClassModeKeyList", String.Join(",", model.ClassModeKeys))
                    .WithSqlParam("@NatureOfEnquiryKeyList", String.Join(",", model.NatureOfEnquiryKeys))
                    .WithSqlParam("@BranchKeyList", String.Join(",", model.BranchKeys))
                    .WithSqlParam("@AgentKeyList", String.Join(",", model.AgentKeys))
                    .WithSqlParam("@StudentStatusKeyList", String.Join(",", model.StudentStatusKeys))
                    .WithSqlParam("@MediumkeyList", String.Join(",", model.MeadiumKeys))
                    .WithSqlParam("@ClassDetailsKeyList", String.Join(",", model.ClassKeys))
                    .WithSqlParam("@AcademicTermKey", model.AcademicTermKey)
                    .WithSqlParam("@CourseYearsKeyList", String.Join(",", model.CourseYearsKeys))
                    .WithSqlParam("@GenderKey", model.GenderKey)
                    .WithSqlParam("@ClassRequiredKey", model.ClassRequiredKey)
                    .WithSqlParam("@AdmissionDate", (model.DateOfAdmission != null ? Convert.ToDateTime(model.DateOfAdmission).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@DateAddedFrom", (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@DateAddedTo", (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@ExamStatus", model.ExamStatus)
                    .WithSqlParam("@ExamResultStatus", model.ExamResultStatus)
                    .WithSqlParam("@SearchAnyText", model.SearchAnyText.VerifyData())
                    .WithSqlParam("@UserKey", DbConstants.User.UserKey)
                    .WithSqlParam("@PageIndex", model.page)
                    .WithSqlParam("@PageSize", model.rows)
                    .WithSqlParam("@SortBy", model.sidx)
                    .WithSqlParam("@TotalRecords", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalRecordsParam = dbParam;
                    }).ExecuteStoredProc((handler) =>
                    {
                        applicationList = handler.ReadToDynamicList<dynamic>() as List<dynamic>;
                        //applicationList = handler.ReadToList<ApplicationViewModel>() as List<ApplicationViewModel>;
                    });

                TotalRecords = Convert.ToInt64((TotalRecordsParam.Value ?? 0));
                return applicationList;
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.ExamScheduleSummary, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<dynamic>();
            }
        }

        public List<ExamScheduleSummary> GetExamSchdeuleDetailsByApplication(long ApplicationKey)
        {
            Application application = dbContext.Applications.SingleOrDefault(x => x.RowKey == ApplicationKey);

            //var ExamSchdeuleDetails = (from CSD in dbContext.CourseSubjectDetails.Where(row => row.CourseSubjectMaster.AcademicTermKey == application.AcademicTermKey
            //                                && row.CourseSubjectMaster.CourseKey == application.CourseKey && row.CourseSubjectMaster.UniversityMasterKey == application.UniversityMasterKey
            //                                )//&& row.CourseSubjectMaster.CourseYear == model.CourseYear)
            //                           join es in dbContext.ExamSchedules
            //                           on new { ApplicationKey = application.RowKey } equals new { es.ApplicationKey } into esa
            //                           from es in esa.DefaultIfEmpty()
            //                           join er in dbContext.ExamResults on new { ExamScheduleKey = es.RowKey, ApplicationKey = application.RowKey } equals new { er.ExamScheduleKey, er.ApplicationKey } into erj
            //                           from er in erj.DefaultIfEmpty()
            //                           //&&CSD.CourseSubjectMaster.CourseYear == model.CourseYear 
            //                           select new ExamScheduleSummary
            //                           {

            //                               SubjectName = CSD.Subject.SubjectName,
            //                               AppearenceCount = es.AppearenceCount != null ? es.AppearenceCount : 1,

            //                               ExamRegisterNumber = application.ExamRegisterNo,
            //                               MaximumMark = es.MaximumMark ?? 0,
            //                               MinimumMark = es.MinimumMark ?? 0,
            //                               ExamStartTime = es.ExamStartTime,
            //                               ExamEndTime = es.ExamEndTime,
            //                               ExamDate = es.ExamDate != null ? es.ExamDate : DateTimeUTC.Now,
            //                               ExamTermName = es.ExamTerm.ExamTermName,
            //                               ExamAttempName = "",
            //                               Mark = er.Mark,
            //                               ExamResultStatus = er.ResultStatus,
            //                               ExamStatus = es.ExamStatus == DbConstants.ExamStatus.Reguler ? "Reguler" : (es.ExamStatus == DbConstants.ExamStatus.Supply ? "Supply" : "Improvement"),
            //                               SubjectYear = CSD.CourseSubjectMaster.CourseYear,
            //                               AcademicTermKey = CSD.CourseSubjectMaster.AcademicTermKey,
            //                               CourseDuration = CSD.CourseSubjectMaster.Course.CourseDuration
            //                           }).ToList();
            //foreach (ExamScheduleSummary objmodel in ExamSchdeuleDetails)
            //{
            //    objmodel.SubjectYearName = CommonUtilities.GetYearDescriptionByCodeDetails(objmodel.CourseDuration ?? 0, objmodel.SubjectYear ?? 0, objmodel.AcademicTermKey ?? 0);

            //}

            var ExamSchdeuleDetails = (from row in dbContext.Sp_StudentExamScheduleDetails_Summary(ApplicationKey)
                                       select new ExamScheduleSummary
                                       {

                                           SubjectName = row.subjectName,
                                           AppearenceCount = row.AppearenceCount,

                                           ExamRegisterNumber = application.ExamRegisterNo,
                                           MaximumMark = row.MaximumMark ?? 0,
                                           MinimumMark = row.MinimumMark ?? 0,
                                           ExamStartTime = row.ExamStartTime,
                                           ExamEndTime = row.ExamEndTime,
                                           ExamDate = row.ExamDate,
                                           ExamTermName = row.ExamTermName,
                                           ExamAttempName = "",
                                           Mark = row.Mark,
                                           ResultStatus = row.ResultStatus,
                                           //ExamStatus = row.ExamStatus == DbConstants.ExamStatus.Reguler ? "Reguler" : (es.ExamStatus == DbConstants.ExamStatus.Supply ? "Supply" : "Improvement"),
                                           SubjectYear = row.CourseYear,
                                           AcademicTermKey = row.AcademicTermKey,
                                           CourseDuration = row.CourseDuration,
                                           AppliedStatus = row.AppliedStatus
                                       }).ToList();
            foreach (ExamScheduleSummary objmodel in ExamSchdeuleDetails)
            {
                objmodel.SubjectYearName = CommonUtilities.GetYearDescriptionByCodeDetails(objmodel.CourseDuration ?? 0, objmodel.SubjectYear ?? 0, objmodel.AcademicTermKey ?? 0);

            }
            return ExamSchdeuleDetails;
        }

        #endregion

        #region Teacher Work Schedule
        //public List<ReportViewModel> GetTeacherWorkScheduleSummary(ReportViewModel model)
        //{
        //    try
        //    {
        //        ObjectParameter objTotalRecords = new ObjectParameter("TotalRecords", typeof(Int64));

        //        if (model.sidx != "")
        //        {
        //            model.sidx = model.sidx + " " + model.sord;
        //        }

        //        var TeacherWorkScheduleSummary = (
        //                                     from Application in dbContext.Sp_TeacherWorkScheduleSummary
        //                                         (
        //                                            String.Join(",", model.CourseKeys),
        //                                           String.Join(",", model.CourseTypeKeys),
        //                                           String.Join(",", model.UniversityMasterKeys),
        //                                           String.Join(",", model.BatchKeys),
        //                                           String.Join(",", model.BranchKeys),
        //                                           String.Join(",", model.ClassKeys),
        //                                           model.AcademicTermKey,
        //                                           String.Join(",", model.CourseYearsKeys),
        //                                           (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""),
        //                                           (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""),
        //                                           String.Join(",", model.SubjectKeys),
        //                                           String.Join(",", model.SubjectModuleKeys),
        //                                           String.Join(",", model.EmployeeKey),
        //                                           model.page,
        //                                           model.rows,
        //                                           model.sidx,
        //                                           objTotalRecords
        //                                         )
        //                                     select new ReportViewModel
        //                                     {
        //                                         RowKey = Application.Rowkey ?? 0
        //                                         ,
        //                                         SubjectModuleKey = Application.SubjectModuleKey
        //                                            ,
        //                                         ClassDetailsKey = Application.ClassDetailsKey
        //                                            ,
        //                                         SubjectKey = Application.SubjectKey
        //                                            ,
        //                                         BatchKey = Application.BatchKey
        //                                            ,
        //                                         BranchKey = Application.BranchKey
        //                                            ,
        //                                         SubjectName = Application.SubjectName
        //                                            ,
        //                                         ModuleName = Application.ModuleName
        //                                            ,
        //                                         BatchName = Application.BatchName
        //                                            ,
        //                                         CurrentYear = Application.StudentYear
        //                                            ,
        //                                         AcademicTermKey = Application.AcademicTermKey
        //                                            ,
        //                                         Course = Application.CourseName
        //                                            ,
        //                                         Affiliations = Application.UniversityMasterName
        //                                            ,
        //                                         Batch = Application.BatchName
        //                                            ,
        //                                         BranchName = Application.BranchName
        //                                            ,
        //                                         Class = Application.ClassCode
        //                                            ,
        //                                         Duration = Application.Duration
        //                                            ,
        //                                         ProgressStatus = Application.ProgressStatus
        //                                            ,
        //                                         CurrentYearText = CommonUtilities.GetYearDescriptionByCodeDetails(Application.CourseDuration ?? 0, Application.StudentYear, Application.AcademicTermKey)


        //                                     }).ToList();



        //        model.TotalRecords = objTotalRecords.Value != DBNull.Value ? Convert.ToInt64(objTotalRecords.Value) : 0;



        //        return TeacherWorkScheduleSummary;
        //    }
        //    catch (Exception ex)
        //    {                
        //        ActivityLog.CreateActivityLog(MenuConstants.EmployeeWorkScheduleSummary, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
        //        return new List<ReportViewModel>();
        //    }
        //}

        public List<dynamic> GetTeacherWorkScheduleSummary(ReportViewModel model, out long TotalRecords)
        {
            try
            {
                List<dynamic> applicationList = new List<dynamic>();
                DbParameter TotalRecordsParam = null;

                if (model.sidx != "")
                {
                    model.sidx = model.sidx + " " + model.sord;
                }

                dbContext.LoadStoredProc("dbo.Sp_TeacherWorkScheduleSummary")

                    .WithSqlParam("@CourseKeyList", String.Join(",", model.CourseKeys))
                    .WithSqlParam("@CourseTypeKeyList", String.Join(",", model.CourseTypeKeys))
                    .WithSqlParam("@UniversityMasterKeyList", String.Join(",", model.UniversityMasterKeys))
                    .WithSqlParam("@BatchKeyList", String.Join(",", model.BatchKeys))
                    .WithSqlParam("@BranchKeyList", String.Join(",", model.BranchKeys))
                    .WithSqlParam("@ClassDetailsKeyList", String.Join(",", model.ClassKeys))
                    .WithSqlParam("@AcademicTermKey", model.AcademicTermKey)
                    .WithSqlParam("@CourseYearsKeyList", String.Join(",", model.CourseYearsKeys))
                    .WithSqlParam("@DateAddedFrom", (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@DateAddedTo", (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@SubjectKeyList", String.Join(",", model.SubjectKeys))
                    .WithSqlParam("@SubjectModuleKeyList", String.Join(",", model.SubjectModuleKeys))
                    .WithSqlParam("@EmployeeKeyList", String.Join(",", model.EmployeeKey))
                    .WithSqlParam("@UserKey", DbConstants.User.UserKey)
                    .WithSqlParam("@PageIndex", model.page)
                    .WithSqlParam("@PageSize", model.rows)
                    .WithSqlParam("@SortBy", model.sidx)
                    .WithSqlParam("@TotalRecords", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalRecordsParam = dbParam;
                    }).ExecuteStoredProc((handler) =>
                    {
                        applicationList = handler.ReadToDynamicList<dynamic>() as List<dynamic>;

                    });

                TotalRecords = Convert.ToInt64((TotalRecordsParam.Value ?? 0));
                return applicationList;



            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.EmployeeWorkScheduleSummary, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<dynamic>();
            }
        }

        public List<WorkscheduleSubjectmodel> GetHistoryWorkSchedule(ReportViewModel model)
        {
            try
            {
                //List<WorkscheduleSubjectmodel> objviewModel = new List<WorkscheduleSubjectmodel>();
                //objviewModel = dbContext.TeacherWorkScheduleDetails.Where(x => x.TeacherWorkScheduleMaster.BatchKey == model.BatchKey
                //    && x.TeacherWorkScheduleMaster.BranchKey == model.BranchKey && x.TeacherWorkScheduleMaster.ClassDetailsKey == model.ClassDetailsKey
                //    && x.TeacherWorkScheduleMaster.SubjectKey == model.SubjectKey && x.TeacherWorkScheduleMaster.SubjectModuleKey == model.SubjectModuleKey).Select(row => new WorkscheduleSubjectmodel
                //{
                //    RowKey = row.RowKey,
                //    TopicKey = row.TeacherWorkScheduleMaster.TopicKey,
                //    TopicName = row.TeacherWorkScheduleMaster.ModuleTopic.TopicName,
                //    ModuleName = row.TeacherWorkScheduleMaster.SubjectModule.ModuleName,
                //    MasterRowKey = row.TecherScheduleMasterKey,
                //    WorkScheduleDate = row.WorkScheduleDate,
                //    Duration = row.Duration,
                //    TimeIn = row.TimeIn,
                //    TimeOut = row.TimeOut,
                //    CurrentProgressStatus = row.CurrentProgressStatus,
                //    EmployeeKey = row.EmployeeKey,
                //    EmployeeName = row.Employee.FirstName
                //}).ToList();



                List<WorkscheduleSubjectmodel> objviewModel = new List<WorkscheduleSubjectmodel>();
                objviewModel = dbContext.TeacherWorkScheduleMasters.Where(x => x.BatchKey == model.BatchKey
                    && x.BranchKey == model.BranchKey && x.ClassDetailsKey == model.ClassDetailsKey
                    && x.SubjectKey == model.SubjectKey && x.SubjectModuleKey == model.SubjectModuleKey)
                    .Select(row => new WorkscheduleSubjectmodel
                    {
                        RowKey = row.RowKey,
                        TopicKey = row.TopicKey,
                        TopicName = row.ModuleTopic.TopicName,
                        ModuleName = row.SubjectModule.ModuleName,
                        Duration = row.TeacherWorkScheduleDetails.Sum(x => x.Duration),

                        CurrentProgressStatus = row.TeacherWorkScheduleDetails.Sum(x => x.CurrentProgressStatus),

                    }).ToList();





                return objviewModel.GroupBy(x => x.TopicKey).Select(y => y.FirstOrDefault()).ToList();
                //return objviewModel;
            }

            catch (Exception Ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.EmployeeWorkScheduleSummary, ActionConstants.View, DbConstants.LogType.Error, null, Ex.GetBaseException().Message);
                return new List<WorkscheduleSubjectmodel>();

            }
        }

        #endregion

        #region DayToDayFeePaymentSummary


        public void FillDropDownForDayToDayFee(ReportViewModel model)
        {
            FillCommonFeeTypes(model);
            FillPaymentMode(model);
            FillProcessStatus(model);
            FillPaymentStatus(model);
            FillAccountHead(model);
            FillBankAccount(model);
            FillCashFlowType(model);
        }
        public void FillCommonFeeTypes(ReportViewModel model)
        {
            model.FeeTypes = dbContext.FeeTypes.Where(row => row.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.FeeTypeName
            }).ToList();
        }
        public void FillPaymentMode(ReportViewModel model)
        {
            model.PaymentModes = dbContext.PaymentModes.Where(row => row.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.PaymentModeName
            }).ToList();
        }
        public List<ReportViewModel> GetStudentsDayToDayFeePaymentSummary(ReportViewModel model)
        {

            ObjectParameter objTotalRecords = new ObjectParameter("TotalRecords", typeof(Int64));

            if (model.sidx != "")
            {
                model.sidx = model.sidx + " " + model.sord;
            }

            var StudentSummaryReports = (
                                         from Application in dbContext.Sp_StudentDayToDayFeePaymentSummary
                                             (
                                                String.Join(",", model.CourseKeys),
                                               String.Join(",", model.CourseTypeKeys),
                                                String.Join(",", model.UniversityMasterKeys),
                                                 model.AcademicTermKey,
                                               String.Join(",", model.BatchKeys),
                                               String.Join(",", model.ReligionKeys),
                                               String.Join(",", model.BranchKeys),
                                               String.Join(",", model.StudentStatusKeys),
                                               String.Join(",", model.ClassKeys),
                                               String.Join(",", model.CourseYearsKeys),
                                                String.Join(",", model.MeadiumKeys),
                                                model.GenderKey,
                                                (model.DateOfAdmission != null ? Convert.ToDateTime(model.DateOfAdmission).ToString("yyyy-MM-dd") : ""),
                                                (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""),
                                                (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""),
                                                 model.SearchAnyText.VerifyData(),
                                                String.Join(",", model.PaymentModeKeys),
                                                String.Join(",", model.FeeTypeKeys),
                                                model.page,
                                                model.rows,
                                                model.sidx,
                                                objTotalRecords
                                             )
                                         select new ReportViewModel
                                         {
                                             RowKey = Application.RowKey
                                                ,
                                             ApplicationNo = Application.ApplicationNo
                                                ,
                                             Name = Application.StudentName
                                                ,
                                             GuardianName = Application.StudentGuardian

                                                ,
                                             Email = Application.StudentEmail

                                                ,
                                             Mobile = Application.StudentMobile
                                                ,
                                             DOB = Application.StudentDOB
                                                //   ,
                                                //TotalFee = Application.StudentTotalFee
                                                ,
                                             DateOfAdmission = Application.StudentDateOfAdmission
                                                ,
                                             EnrollmentNo = Application.StudentEnrollmentNo
                                                ,
                                             PhotoPath = Application.StudentPhotoPath
                                                ,
                                             ExamRegisterNo = Application.ExamRegisterNo
                                                ,
                                             Remarks = Application.Remarks
                                                ,
                                             AdmissionNo = Application.AdmissionNo
                                                ,
                                             SerialNumber = Application.SerialNumber

                                                ,
                                             RollNumber = Application.RollNumber
                                                ,
                                             RollNoCode = Application.RollNoCode
                                                ,
                                             DateAdded = Application.DateAdded
                                                ,
                                             Course = Application.CourseName
                                                ,
                                             CourseType = Application.CourseTypeName
                                                ,
                                             BloodGroupName = Application.BloodGroupName
                                                ,
                                             Batch = Application.BatchName
                                                ,
                                             Mode = Application.ModeName
                                                ,
                                             Religion = Application.ReligionName
                                                ,
                                             StudentStatus = Application.StudentStatusName
                                                ,
                                             BranchName = Application.BranchName
                                                ,
                                             Class = Application.ClassCode
                                                ,
                                             Gender = Application.StudentGender == 1 ? "Male" : "Female"
                                                   ,
                                             FeeDate = Application.FeeDate
                                                 ,
                                             FeeTypeName = Application.FeeTypeName
                                               ,
                                             PaymentModeName = Application.PaymentModeName
                                                   ,
                                             ReceiptNo = Application.ReceiptNo
                                               ,
                                             FeeAmount = Application.FeeAmount
                                               ,
                                             CGSTAmount = Application.CGSTAmount
                                               ,
                                             SGSTAmount = Application.SGSTAmount
                                               ,
                                             IGSTAmount = Application.IGSTAmount
                                               ,
                                             CessAmount = Application.CessAmount
                                               ,
                                             TaxableAmount = Application.TaxableAmount
                                               ,
                                             TotalAmount = Application.TotalAmount
                                               ,
                                             CGSTRate = Application.CGSTRate
                                               ,
                                             SGSTRate = Application.SGSTRate
                                               ,
                                             CessRate = Application.CessRate
                                                ,
                                             FeeDescreption = Application.FeeDescription
                                                ,
                                             CurrentYearText = Application.PaymentYear != null ? CommonUtilities.GetYearDescriptionByCodeDetails(Application.CourseDuration ?? 0, Application.PaymentYear ?? 0, Application.AcademicTermKey) : ""
                                                ,
                                             ChequeClearanceDate = Application.ChequeClearanceDate
                                                ,
                                             ChequeOrDDNumber = Application.ChequeOrDDNumber
                                                ,
                                             CardNumber = Application.CardNumber
                                                ,
                                             BankName = Application.BankName
                                                ,
                                             PaymentModeSubName = Application.PaymentModeSubName
                                                ,
                                             IsRefund = Application.IsRefund == 1 ? true : false
                                                ,
                                             RefundAmount = Application.RefundAmount
                                         }).ToList();



            model.TotalRecords = objTotalRecords.Value != DBNull.Value ? Convert.ToInt64(objTotalRecords.Value) : 0;


            model.TotalPaidSum = StudentSummaryReports.Select(x => x.TotalAmount).DefaultIfEmpty().Sum();


            return StudentSummaryReports;
        }


        #endregion

        #region DayToDayUniversityPaymentSummary

        public List<ReportViewModel> GetStudentsDayToDayUniversityPaymentSummary(ReportViewModel model)
        {

            ObjectParameter objTotalRecords = new ObjectParameter("TotalRecords", typeof(Int64));

            if (model.sidx != "")
            {
                model.sidx = model.sidx + " " + model.sord;
            }

            var StudentSummaryReports = (
                                         from Application in dbContext.Sp_StudentDayToDayUniversityPaymentSummary
                                             (
                                                String.Join(",", model.CourseKeys),
                                               String.Join(",", model.CourseTypeKeys),
                                                String.Join(",", model.UniversityMasterKeys),
                                                 model.AcademicTermKey,
                                               String.Join(",", model.BatchKeys),
                                               String.Join(",", model.ReligionKeys),
                                               String.Join(",", model.BranchKeys),
                                               String.Join(",", model.StudentStatusKeys),
                                               String.Join(",", model.ClassKeys),
                                               String.Join(",", model.CourseYearsKeys),
                                               String.Join(",", model.MeadiumKeys),
                                                model.GenderKey,
                                                (model.DateOfAdmission != null ? Convert.ToDateTime(model.DateOfAdmission).ToString("yyyy-MM-dd") : ""),
                                                (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""),
                                                (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""),
                                                 model.SearchAnyText.VerifyData(),
                                                String.Join(",", model.PaymentModeKeys),
                                                String.Join(",", model.FeeTypeKeys),
                                                model.page,
                                                model.rows,
                                                model.sidx,
                                                objTotalRecords
                                             )
                                         select new ReportViewModel
                                         {
                                             RowKey = Application.RowKey
                                                ,
                                             ApplicationNo = Application.ApplicationNo
                                                ,
                                             Name = Application.StudentName
                                                ,
                                             GuardianName = Application.StudentGuardian

                                                ,
                                             Email = Application.StudentEmail

                                                ,
                                             Mobile = Application.StudentMobile
                                                ,
                                             DOB = Application.StudentDOB
                                                //   ,
                                                //TotalFee = Application.StudentTotalFee
                                                ,
                                             DateOfAdmission = Application.StudentDateOfAdmission
                                                ,
                                             EnrollmentNo = Application.StudentEnrollmentNo
                                                ,
                                             PhotoPath = Application.StudentPhotoPath
                                                ,
                                             ExamRegisterNo = Application.ExamRegisterNo
                                                ,
                                             Remarks = Application.Remarks
                                                ,
                                             AdmissionNo = Application.AdmissionNo
                                                ,
                                             SerialNumber = Application.SerialNumber

                                                ,
                                             RollNumber = Application.RollNumber
                                                ,
                                             RollNoCode = Application.RollNoCode
                                                ,
                                             DateAdded = Application.DateAdded
                                                ,
                                             Course = Application.CourseName
                                                ,
                                             CourseType = Application.CourseTypeName
                                                ,
                                             BloodGroupName = Application.BloodGroupName
                                                ,
                                             Batch = Application.BatchName
                                                ,
                                             Mode = Application.ModeName
                                                ,
                                             Religion = Application.ReligionName
                                                ,
                                             StudentStatus = Application.StudentStatusName
                                                ,
                                             BranchName = Application.BranchName
                                                ,
                                             Class = Application.ClassCode
                                                ,
                                             Gender = Application.StudentGender == 1 ? "Male" : "Female"
                                                   ,
                                             FeeDate = Application.FeeDate
                                                 ,
                                             FeeTypeName = Application.FeeTypeName
                                               ,
                                             PaymentModeName = Application.PaymentModeName
                                                   ,
                                             ReceiptNo = Application.ReceiptNo
                                               ,
                                             FeeAmount = Application.FeeAmount
                                               ,
                                             CGSTAmount = Application.CGSTAmount
                                               ,
                                             SGSTAmount = Application.SGSTAmount
                                               ,
                                             IGSTAmount = Application.IGSTAmount
                                               ,
                                             CGSTRate = Application.CGSTRate
                                               ,
                                             SGSTRate = Application.SGSTRate
                                               ,
                                             FeeDescreption = Application.FeeDescription
                                                ,
                                             CurrentYearText = Application.Paymentyear != null ? CommonUtilities.GetYearDescriptionByCodeDetails(Application.CourseDuration ?? 0, Application.Paymentyear ?? 0, Application.AcademicTermKey) : ""
                                                ,
                                             ChequeClearanceDate = Application.ChequeClearanceDate
                                             ,
                                             ChequeOrDDNumber = Application.ChequeOrDDNumber
                                             ,
                                             CardNumber = Application.CardNumber
                                             ,
                                             BankName = Application.BankName
                                             ,
                                             PaymentModeSubName = Application.PaymentModeSubName
                                             ,
                                             IsCancel = Application.IsCancel == 1 ? true : false
                                             ,
                                             IfServiceCharge = Application.IfServiceCharge
                                             ,
                                             ServiceFee = Application.ServiceFee
                                             ,
                                             TotalDeductionFee = Application.TotalDeductionFee
                                             ,
                                             CancelDate = Application.CancelDate
                                             ,
                                             CancelRemarks = Application.CancelRemarks
                                             ,
                                             AccountHeadName = Application.AccountHeadName
                                             ,
                                             UniversityPaymentDetailsKey = Application.UniversityPaymentDetailsKey

                                         }).ToList();



            model.TotalRecords = objTotalRecords.Value != DBNull.Value ? Convert.ToInt64(objTotalRecords.Value) : 0;


            model.TotalPaidSum = StudentSummaryReports.Select(x => x.TotalAmount).DefaultIfEmpty().Sum();


            return StudentSummaryReports;
        }


        #endregion

        #region Student Late Summary

        public List<dynamic> GetStudentLateSummary(ReportViewModel model, out long TotalRecords)
        {
            try
            {
                List<dynamic> applicationList = new List<dynamic>();
                DbParameter TotalRecordsParam = null;

                if (model.sidx != "")
                {
                    model.sidx = model.sidx + " " + model.sord;
                }
                dbContext.LoadStoredProc("dbo.Sp_StudentLateSummary_Report")

                    .WithSqlParam("@CourseKeyList", String.Join(",", model.CourseKeys))
                    .WithSqlParam("@CourseTypeKeyList", String.Join(",", model.CourseTypeKeys))
                    .WithSqlParam("@UniversityMasterKeyList", String.Join(",", model.UniversityMasterKeys))
                    .WithSqlParam("@BatchKeyList", String.Join(",", model.BatchKeys))
                    .WithSqlParam("@ReligionKeyList", String.Join(",", model.ReligionKeys))
                    .WithSqlParam("@IncomeKeyList", String.Join(",", model.IncomeGroupKeys))
                    .WithSqlParam("@SecondLanguageKeyList", String.Join(",", model.SecondLanguageKeys))
                    .WithSqlParam("@ModeKeyList", String.Join(",", model.ModeKeys))
                    .WithSqlParam("@ClassModeKeyList", String.Join(",", model.ClassModeKeys))
                    .WithSqlParam("@NatureOfEnquiryKeyList", String.Join(",", model.NatureOfEnquiryKeys))
                    .WithSqlParam("@BranchKeyList", String.Join(",", model.BranchKeys))
                    .WithSqlParam("@AgentKeyList", String.Join(",", model.AgentKeys))
                    .WithSqlParam("@StudentStatusKeyList", String.Join(",", model.StudentStatusKeys))
                    .WithSqlParam("@MediumkeyList", String.Join(",", model.MeadiumKeys))
                    .WithSqlParam("@ClassDetailsKeyList", String.Join(",", model.ClassKeys))
                    .WithSqlParam("@AcademicTermKey", model.AcademicTermKey)
                    .WithSqlParam("@CourseYearsKeyList", String.Join(",", model.CourseYearsKeys))
                    .WithSqlParam("@GenderKey", model.GenderKey)
                    .WithSqlParam("@ClassRequiredKey", model.ClassRequiredKey)
                    .WithSqlParam("@AdmissionDate", (model.DateOfAdmission != null ? Convert.ToDateTime(model.DateOfAdmission).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@DateAddedFrom", (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@DateAddedTo", (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@SearchAnyText", model.SearchAnyText.VerifyData())
                    .WithSqlParam("@AttendanceTypeKeyList", String.Join(",", model.AttendanceTypeKeys))
                    .WithSqlParam("@UserKey", DbConstants.User.UserKey)
                    .WithSqlParam("@PageIndex", model.page)
                    .WithSqlParam("@PageSize", model.rows)
                    .WithSqlParam("@SortBy", model.sidx)
                    .WithSqlParam("@TotalRecords", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalRecordsParam = dbParam;
                    }).ExecuteStoredProc((handler) =>
                    {
                        applicationList = handler.ReadToDynamicList<dynamic>() as List<dynamic>;
                    });

                TotalRecords = Convert.ToInt64((TotalRecordsParam.Value ?? 0));
                return applicationList;
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.StudentsLateReport, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<dynamic>();
            }
        }
        //public List<ReportViewModel> GetStudentLateSummary(ReportViewModel model)
        //{

        //    ObjectParameter objTotalRecords = new ObjectParameter("TotalRecords", typeof(Int64));

        //    if (model.sidx != "")
        //    {
        //        model.sidx = model.sidx + " " + model.sord;
        //    }

        //    var StudentLateSummary = (
        //                                 from Application in dbContext.Sp_StudentLateSummary_Report
        //                                     (
        //                                        String.Join(",", model.CourseKeys),
        //                                       String.Join(",", model.CourseTypeKeys),
        //                                       String.Join(",", model.UniversityMasterKeys),
        //                                       String.Join(",", model.BatchKeys),
        //                                       String.Join(",", model.ReligionKeys),
        //                                       String.Join(",", model.IncomeGroupKeys),
        //                                       String.Join(",", model.SecondLanguageKeys),
        //                                       String.Join(",", model.ModeKeys),
        //                                       String.Join(",", model.ClassModeKeys),
        //                                       String.Join(",", model.NatureOfEnquiryKeys),
        //                                       String.Join(",", model.BatchKeys),
        //                                       String.Join(",", model.AgentKeys),
        //                                       String.Join(",", model.StudentStatusKeys),
        //                                       String.Join(",", model.MeadiumKeys),
        //                                       String.Join(",", model.ClassKeys),
        //                                        model.AcademicTermKey,
        //                                       String.Join(",", model.CourseYearsKeys),
        //                                        model.GenderKey,
        //                                        model.ClassRequiredKey,
        //                                        (model.DateOfAdmission != null ? Convert.ToDateTime(model.DateOfAdmission).ToString("yyyy-MM-dd") : ""),
        //                                        (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""),
        //                                        (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""),
        //                                        model.SearchAnyText.VerifyData(),
        //                                          String.Join(",", model.AttendanceTypeKeys),
        //                                        model.page,
        //                                        model.rows,
        //                                        model.sidx,
        //                                        objTotalRecords
        //                                     )
        //                                 select new ReportViewModel
        //                                 {
        //                                     RowKey = Application.RowKey
        //                                        ,
        //                                     ApplicationNo = Application.ApplicationNo
        //                                        ,
        //                                     Name = Application.StudentName
        //                                        ,
        //                                     GuardianName = Application.StudentGuardian
        //                                        ,
        //                                     MotherName = Application.StudentMotherName
        //                                        ,
        //                                     PermanentAddress = Application.StudentPermanentAddress
        //                                        ,
        //                                     PresentAddress = Application.StudentPresentAddress
        //                                        ,
        //                                     Email = Application.StudentEmail
        //                                        ,
        //                                     Phone = Application.StudentPhone
        //                                        ,
        //                                     Mobile = Application.StudentMobile
        //                                        ,
        //                                     DOB = Application.StudentDOB
        //                                        ,
        //                                     StartYear = Application.StartYear
        //                                        ,
        //                                     TotalFee = Application.StudentTotalFee
        //                                        ,
        //                                     ClassRequiredDesc = Application.StudentClassRequiredDesc
        //                                        ,
        //                                     PresentJob_CourseOfStudyId = Application.PresentJob_CourseOfStudyId
        //                                        ,
        //                                     DateOfAdmission = Application.StudentDateOfAdmission
        //                                        ,
        //                                     StudyMaterialIssueStatus = Application.StudyMaterialIssueStatus
        //                                        ,
        //                                     EnrollmentNo = Application.StudentEnrollmentNo
        //                                        ,
        //                                     PhotoPath = Application.StudentPhotoPath
        //                                        ,
        //                                     ExamRegisterNo = Application.ExamRegisterNo
        //                                        ,
        //                                     Remarks = Application.Remarks
        //                                        ,
        //                                     AdmissionNo = Application.AdmissionNo
        //                                        ,
        //                                     SerialNumber = Application.SerialNumber
        //                                        ,
        //                                     CurrentYear = Application.CurrentYear
        //                                        ,
        //                                     RollNumber = Application.RollNumber
        //                                        ,
        //                                     AcademicTermKey = Application.AcademicTermKey
        //                                        ,
        //                                     DateAdded = Application.DateAdded
        //                                        ,
        //                                     Course = Application.CourseName
        //                                        ,
        //                                     CourseType = Application.CourseTypeName
        //                                        ,
        //                                     Affiliations = Application.UniversityMasterName
        //                                        ,
        //                                     Batch = Application.BatchName
        //                                        ,
        //                                     Mode = Application.ModeName
        //                                        ,
        //                                     ClassMode = Application.ClassModeName
        //                                        ,
        //                                     Religion = Application.ReligionName
        //                                        ,
        //                                     AcademicTerm = Application.AcademicTermName
        //                                        ,
        //                                     SecondLanguage = Application.SecondLanguageName
        //                                        ,
        //                                     Medium = Application.MediumName
        //                                        ,
        //                                     Income = Application.IncomeName
        //                                        ,
        //                                     NatureOfEnquiry = Application.NatureOfEnquiryName
        //                                        ,
        //                                     Agent = Application.AgentName
        //                                        ,
        //                                     StudentStatus = Application.StudentStatusName
        //                                        ,
        //                                     BranchName = Application.BranchName
        //                                        ,
        //                                     Class = Application.ClassCode
        //                                        ,
        //                                     Gender = Application.StudentGender == 1 ? "Male" : "Female"
        //                                        ,
        //                                     ClassRequiredKey = Application.ClassRequired ? 1 : 0
        //                                        ,
        //                                     ClassRequired = Application.ClassRequired == true ? EduSuiteUIResources.Yes : EduSuiteUIResources.No
        //                                        ,
        //                                     CurrentYearText = CommonUtilities.GetYearDescriptionByCodeDetails(Application.CourseDuration ?? 0, Application.CurrentYear, Application.AcademicTermKey)
        //                                        ,
        //                                     LateDate = Application.LateDate
        //                                     ,
        //                                     LateMinutes = Application.LateMinutes
        //                                     ,
        //                                     AttachmentPath = Application.AttachmentPath
        //                                     ,
        //                                     LateRemarks = Application.LateRemarks
        //                                 }).ToList();



        //    model.TotalRecords = objTotalRecords.Value != DBNull.Value ? Convert.ToInt64(objTotalRecords.Value) : 0;
        //    //StudentSummaryReports = StudentSummaryReports.OrderByDescending(Row => Row.DateAdded).Skip(skip).Take(Take).ToList();



        //    return StudentLateSummary;
        //}
        #endregion Student Late Summary

        #region Student Leave Summary

        public List<dynamic> GetStudentLeaveSummary(ReportViewModel model, out long TotalRecords)
        {
            try
            {
                List<dynamic> applicationList = new List<dynamic>();
                DbParameter TotalRecordsParam = null;

                if (model.sidx != "")
                {
                    model.sidx = model.sidx + " " + model.sord;
                }
                dbContext.LoadStoredProc("dbo.Sp_StudentLeaveSummary_Report")
                    .WithSqlParam("@CourseKeyList", String.Join(",", model.CourseKeys))
                    .WithSqlParam("@CourseTypeKeyList", String.Join(",", model.CourseTypeKeys))
                    .WithSqlParam("@UniversityMasterKeyList", String.Join(",", model.UniversityMasterKeys))
                    .WithSqlParam("@BatchKeyList", String.Join(",", model.BatchKeys))
                    .WithSqlParam("@ReligionKeyList", String.Join(",", model.ReligionKeys))
                    .WithSqlParam("@IncomeKeyList", String.Join(",", model.IncomeGroupKeys))
                    .WithSqlParam("@SecondLanguageKeyList", String.Join(",", model.SecondLanguageKeys))
                    .WithSqlParam("@ModeKeyList", String.Join(",", model.ModeKeys))
                    .WithSqlParam("@ClassModeKeyList", String.Join(",", model.ClassModeKeys))
                    .WithSqlParam("@NatureOfEnquiryKeyList", String.Join(",", model.NatureOfEnquiryKeys))
                    .WithSqlParam("@BranchKeyList", String.Join(",", model.BranchKeys))
                    .WithSqlParam("@AgentKeyList", String.Join(",", model.AgentKeys))
                    .WithSqlParam("@StudentStatusKeyList", String.Join(",", model.StudentStatusKeys))
                    .WithSqlParam("@MediumkeyList", String.Join(",", model.MeadiumKeys))
                    .WithSqlParam("@ClassDetailsKeyList", String.Join(",", model.ClassKeys))
                    .WithSqlParam("@AcademicTermKey", model.AcademicTermKey)
                    .WithSqlParam("@CourseYearsKeyList", String.Join(",", model.CourseYearsKeys))
                    .WithSqlParam("@GenderKey", model.GenderKey)
                    .WithSqlParam("@ClassRequiredKey", model.ClassRequiredKey)
                    .WithSqlParam("@AdmissionDate", (model.DateOfAdmission != null ? Convert.ToDateTime(model.DateOfAdmission).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@DateAddedFrom", (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@DateAddedTo", (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@SearchAnyText", model.SearchAnyText.VerifyData())
                    .WithSqlParam("@AttendanceTypeKeyList", String.Join(",", model.AttendanceTypeKeys))
                    .WithSqlParam("@UserKey", DbConstants.User.UserKey)
                    .WithSqlParam("@PageIndex", model.page)
                    .WithSqlParam("@PageSize", model.rows)
                    .WithSqlParam("@SortBy", model.sidx)
                    .WithSqlParam("@TotalRecords", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalRecordsParam = dbParam;
                    }).ExecuteStoredProc((handler) =>
                    {
                        applicationList = handler.ReadToDynamicList<dynamic>() as List<dynamic>;
                    });

                TotalRecords = Convert.ToInt64((TotalRecordsParam.Value ?? 0));
                return applicationList;
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.StudentsLeaveReport, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<dynamic>();
            }
        }
        //public List<ReportViewModel> GetStudentLeaveSummary(ReportViewModel model)
        //{

        //    ObjectParameter objTotalRecords = new ObjectParameter("TotalRecords", typeof(Int64));

        //    if (model.sidx != "")
        //    {
        //        model.sidx = model.sidx + " " + model.sord;
        //    }

        //    var StudentLeaveSummary = (
        //                                 from Application in dbContext.Sp_StudentLeaveSummary_Report
        //                                     (
        //                                       String.Join(",", model.CourseKeys),
        //                                       String.Join(",", model.CourseTypeKeys),
        //                                       String.Join(",", model.UniversityMasterKeys),
        //                                       String.Join(",", model.BatchKeys),
        //                                       String.Join(",", model.ReligionKeys),
        //                                       String.Join(",", model.IncomeGroupKeys),
        //                                       String.Join(",", model.SecondLanguageKeys),
        //                                       String.Join(",", model.ModeKeys),
        //                                       String.Join(",", model.ClassModeKeys),
        //                                       String.Join(",", model.NatureOfEnquiryKeys),
        //                                       String.Join(",", model.BatchKeys),
        //                                       String.Join(",", model.AgentKeys),
        //                                       String.Join(",", model.StudentStatusKeys),
        //                                       String.Join(",", model.MeadiumKeys),
        //                                       String.Join(",", model.ClassKeys),
        //                                        model.AcademicTermKey,
        //                                       String.Join(",", model.CourseYearsKeys),
        //                                        model.GenderKey,
        //                                        model.ClassRequiredKey,
        //                                        (model.DateOfAdmission != null ? Convert.ToDateTime(model.DateOfAdmission).ToString("yyyy-MM-dd") : ""),
        //                                        (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""),
        //                                        (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""),
        //                                        model.SearchAnyText.VerifyData(),
        //                                          String.Join(",", model.AttendanceTypeKeys),
        //                                        model.page,
        //                                        model.rows,
        //                                        model.sidx,
        //                                        objTotalRecords
        //                                     )
        //                                 select new ReportViewModel
        //                                 {
        //                                     RowKey = Application.RowKey
        //                                        ,
        //                                     ApplicationNo = Application.ApplicationNo
        //                                        ,
        //                                     Name = Application.StudentName
        //                                        ,
        //                                     GuardianName = Application.StudentGuardian
        //                                        ,
        //                                     MotherName = Application.StudentMotherName
        //                                        ,
        //                                     PermanentAddress = Application.StudentPermanentAddress
        //                                        ,
        //                                     PresentAddress = Application.StudentPresentAddress
        //                                        ,
        //                                     Email = Application.StudentEmail
        //                                        ,
        //                                     Phone = Application.StudentPhone
        //                                        ,
        //                                     Mobile = Application.StudentMobile
        //                                        ,
        //                                     DOB = Application.StudentDOB
        //                                        ,
        //                                     StartYear = Application.StartYear
        //                                        ,
        //                                     TotalFee = Application.StudentTotalFee
        //                                        ,
        //                                     ClassRequiredDesc = Application.StudentClassRequiredDesc
        //                                        ,
        //                                     PresentJob_CourseOfStudyId = Application.PresentJob_CourseOfStudyId
        //                                        ,
        //                                     DateOfAdmission = Application.StudentDateOfAdmission
        //                                        ,
        //                                     StudyMaterialIssueStatus = Application.StudyMaterialIssueStatus
        //                                        ,
        //                                     EnrollmentNo = Application.StudentEnrollmentNo
        //                                        ,
        //                                     PhotoPath = Application.StudentPhotoPath
        //                                        ,
        //                                     ExamRegisterNo = Application.ExamRegisterNo
        //                                        ,
        //                                     Remarks = Application.Remarks
        //                                        ,
        //                                     AdmissionNo = Application.AdmissionNo
        //                                        ,
        //                                     SerialNumber = Application.SerialNumber
        //                                        ,
        //                                     CurrentYear = Application.CurrentYear
        //                                        ,
        //                                     RollNumber = Application.RollNumber
        //                                        ,
        //                                     AcademicTermKey = Application.AcademicTermKey
        //                                        ,
        //                                     DateAdded = Application.DateAdded
        //                                        ,
        //                                     Course = Application.CourseName
        //                                        ,
        //                                     CourseType = Application.CourseTypeName
        //                                        ,
        //                                     Affiliations = Application.UniversityMasterName
        //                                        ,
        //                                     Batch = Application.BatchName
        //                                        ,
        //                                     Mode = Application.ModeName
        //                                        ,
        //                                     ClassMode = Application.ClassModeName
        //                                        ,
        //                                     Religion = Application.ReligionName
        //                                        ,
        //                                     AcademicTerm = Application.AcademicTermName
        //                                        ,
        //                                     SecondLanguage = Application.SecondLanguageName
        //                                        ,
        //                                     Medium = Application.MediumName
        //                                        ,
        //                                     Income = Application.IncomeName
        //                                        ,
        //                                     NatureOfEnquiry = Application.NatureOfEnquiryName
        //                                        ,
        //                                     Agent = Application.AgentName
        //                                        ,
        //                                     StudentStatus = Application.StudentStatusName
        //                                        ,
        //                                     BranchName = Application.BranchName
        //                                        ,
        //                                     Class = Application.ClassCode
        //                                        ,
        //                                     Gender = Application.StudentGender == 1 ? "Male" : "Female"
        //                                        ,
        //                                     ClassRequiredKey = Application.ClassRequired ? 1 : 0
        //                                        ,
        //                                     ClassRequired = Application.ClassRequired == true ? EduSuiteUIResources.Yes : EduSuiteUIResources.No
        //                                        ,
        //                                     CurrentYearText = CommonUtilities.GetYearDescriptionByCodeDetails(Application.CourseDuration ?? 0, Application.CurrentYear, Application.AcademicTermKey)
        //                                        ,
        //                                     LeaveDateFrom = Application.LeaveDateFrom
        //                                     ,
        //                                     LeaveDateTo = Application.LeaveDateTo
        //                                     ,
        //                                     AttachmentPath = Application.AttachmentPath
        //                                     ,
        //                                     LeaveRemarks = Application.LeaveRemarks
        //                                     ,
        //                                 }).ToList();



        //    model.TotalRecords = objTotalRecords.Value != DBNull.Value ? Convert.ToInt64(objTotalRecords.Value) : 0;
        //    //StudentSummaryReports = StudentSummaryReports.OrderByDescending(Row => Row.DateAdded).Skip(skip).Take(Take).ToList();



        //    return StudentLeaveSummary;
        //}
        #endregion Student Leave Summary

        #region Student Absconders Summary

        public List<dynamic> GetStudentAbscondersSummary(ReportViewModel model, out long TotalRecords)
        {
            try
            {
                List<dynamic> applicationList = new List<dynamic>();
                DbParameter TotalRecordsParam = null;

                if (model.sidx != "")
                {
                    model.sidx = model.sidx + " " + model.sord;
                }
                dbContext.LoadStoredProc("dbo.Sp_StudentAbscondersSummary_Report")
                    .WithSqlParam("@CourseKeyList", String.Join(",", model.CourseKeys))
                    .WithSqlParam("@CourseTypeKeyList", String.Join(",", model.CourseTypeKeys))
                    .WithSqlParam("@UniversityMasterKeyList", String.Join(",", model.UniversityMasterKeys))
                    .WithSqlParam("@BatchKeyList", String.Join(",", model.BatchKeys))
                    .WithSqlParam("@ReligionKeyList", String.Join(",", model.ReligionKeys))
                    .WithSqlParam("@IncomeKeyList", String.Join(",", model.IncomeGroupKeys))
                    .WithSqlParam("@SecondLanguageKeyList", String.Join(",", model.SecondLanguageKeys))
                    .WithSqlParam("@ModeKeyList", String.Join(",", model.ModeKeys))
                    .WithSqlParam("@ClassModeKeyList", String.Join(",", model.ClassModeKeys))
                    .WithSqlParam("@NatureOfEnquiryKeyList", String.Join(",", model.NatureOfEnquiryKeys))
                    .WithSqlParam("@BranchKeyList", String.Join(",", model.BranchKeys))
                    .WithSqlParam("@AgentKeyList", String.Join(",", model.AgentKeys))
                    .WithSqlParam("@StudentStatusKeyList", String.Join(",", model.StudentStatusKeys))
                    .WithSqlParam("@MediumkeyList", String.Join(",", model.MeadiumKeys))
                    .WithSqlParam("@ClassDetailsKeyList", String.Join(",", model.ClassKeys))
                    .WithSqlParam("@AcademicTermKey", model.AcademicTermKey)
                    .WithSqlParam("@CourseYearsKeyList", String.Join(",", model.CourseYearsKeys))
                    .WithSqlParam("@GenderKey", model.GenderKey)
                    .WithSqlParam("@ClassRequiredKey", model.ClassRequiredKey)
                    .WithSqlParam("@AdmissionDate", (model.DateOfAdmission != null ? Convert.ToDateTime(model.DateOfAdmission).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@DateAddedFrom", (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@DateAddedTo", (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@SearchAnyText", model.SearchAnyText.VerifyData())
                    .WithSqlParam("@AttendanceTypeKeyList", String.Join(",", model.AttendanceTypeKeys))
                    .WithSqlParam("@UserKey", DbConstants.User.UserKey)
                    .WithSqlParam("@PageIndex", model.page)
                    .WithSqlParam("@PageSize", model.rows)
                    .WithSqlParam("@SortBy", model.sidx)
                    .WithSqlParam("@TotalRecords", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalRecordsParam = dbParam;
                    }).ExecuteStoredProc((handler) =>
                    {
                        applicationList = handler.ReadToDynamicList<dynamic>() as List<dynamic>;
                    });

                TotalRecords = Convert.ToInt64((TotalRecordsParam.Value ?? 0));
                return applicationList;
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.StudentsAbscondersReport, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<dynamic>();
            }
        }
        //public List<ReportViewModel> GetStudentAbscondersSummary(ReportViewModel model)
        //{

        //    ObjectParameter objTotalRecords = new ObjectParameter("TotalRecords", typeof(Int64));

        //    if (model.sidx != "")
        //    {
        //        model.sidx = model.sidx + " " + model.sord;
        //    }

        //    var StudentAbscondersSummary = (
        //                                 from Application in dbContext.Sp_StudentAbscondersSummary_Report
        //                                     (
        //                                       String.Join(",", model.CourseKeys),
        //                                       String.Join(",", model.CourseTypeKeys),
        //                                       String.Join(",", model.UniversityMasterKeys),
        //                                       String.Join(",", model.BatchKeys),
        //                                       String.Join(",", model.ReligionKeys),
        //                                       String.Join(",", model.IncomeGroupKeys),
        //                                       String.Join(",", model.SecondLanguageKeys),
        //                                       String.Join(",", model.ModeKeys),
        //                                       String.Join(",", model.ClassModeKeys),
        //                                       String.Join(",", model.NatureOfEnquiryKeys),
        //                                       String.Join(",", model.BatchKeys),
        //                                       String.Join(",", model.AgentKeys),
        //                                       String.Join(",", model.StudentStatusKeys),
        //                                       String.Join(",", model.MeadiumKeys),
        //                                       String.Join(",", model.ClassKeys),
        //                                        model.AcademicTermKey,
        //                                       String.Join(",", model.CourseYearsKeys),
        //                                        model.GenderKey,
        //                                        model.ClassRequiredKey,
        //                                        (model.DateOfAdmission != null ? Convert.ToDateTime(model.DateOfAdmission).ToString("yyyy-MM-dd") : ""),
        //                                        (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""),
        //                                        (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""),
        //                                        model.SearchAnyText.VerifyData(),
        //                                          String.Join(",", model.AttendanceTypeKeys),
        //                                        model.page,
        //                                        model.rows,
        //                                        model.sidx,
        //                                        objTotalRecords
        //                                     )
        //                                 select new ReportViewModel
        //                                 {
        //                                     RowKey = Application.RowKey
        //                                        ,
        //                                     ApplicationNo = Application.ApplicationNo
        //                                        ,
        //                                     Name = Application.StudentName
        //                                        ,
        //                                     GuardianName = Application.StudentGuardian
        //                                        ,
        //                                     MotherName = Application.StudentMotherName
        //                                        ,
        //                                     PermanentAddress = Application.StudentPermanentAddress
        //                                        ,
        //                                     PresentAddress = Application.StudentPresentAddress
        //                                        ,
        //                                     Email = Application.StudentEmail
        //                                        ,
        //                                     Phone = Application.StudentPhone
        //                                        ,
        //                                     Mobile = Application.StudentMobile
        //                                        ,
        //                                     DOB = Application.StudentDOB
        //                                        ,
        //                                     StartYear = Application.StartYear
        //                                        ,
        //                                     TotalFee = Application.StudentTotalFee
        //                                        ,
        //                                     ClassRequiredDesc = Application.StudentClassRequiredDesc
        //                                        ,
        //                                     PresentJob_CourseOfStudyId = Application.PresentJob_CourseOfStudyId
        //                                        ,
        //                                     DateOfAdmission = Application.StudentDateOfAdmission
        //                                        ,
        //                                     StudyMaterialIssueStatus = Application.StudyMaterialIssueStatus
        //                                        ,
        //                                     EnrollmentNo = Application.StudentEnrollmentNo
        //                                        ,
        //                                     PhotoPath = Application.StudentPhotoPath
        //                                        ,
        //                                     ExamRegisterNo = Application.ExamRegisterNo
        //                                        ,
        //                                     Remarks = Application.Remarks
        //                                        ,
        //                                     AdmissionNo = Application.AdmissionNo
        //                                        ,
        //                                     SerialNumber = Application.SerialNumber
        //                                        ,
        //                                     CurrentYear = Application.CurrentYear
        //                                        ,
        //                                     RollNumber = Application.RollNumber
        //                                        ,
        //                                     AcademicTermKey = Application.AcademicTermKey
        //                                        ,
        //                                     DateAdded = Application.DateAdded
        //                                        ,
        //                                     Course = Application.CourseName
        //                                        ,
        //                                     CourseType = Application.CourseTypeName
        //                                        ,
        //                                     Affiliations = Application.UniversityMasterName
        //                                        ,
        //                                     Batch = Application.BatchName
        //                                        ,
        //                                     Mode = Application.ModeName
        //                                        ,
        //                                     ClassMode = Application.ClassModeName
        //                                        ,
        //                                     Religion = Application.ReligionName
        //                                        ,
        //                                     AcademicTerm = Application.AcademicTermName
        //                                        ,
        //                                     SecondLanguage = Application.SecondLanguageName
        //                                        ,
        //                                     Medium = Application.MediumName
        //                                        ,
        //                                     Income = Application.IncomeName
        //                                        ,
        //                                     NatureOfEnquiry = Application.NatureOfEnquiryName
        //                                        ,
        //                                     Agent = Application.AgentName
        //                                        ,
        //                                     StudentStatus = Application.StudentStatusName
        //                                        ,
        //                                     BranchName = Application.BranchName
        //                                        ,
        //                                     Class = Application.ClassCode
        //                                        ,
        //                                     Gender = Application.StudentGender == 1 ? "Male" : "Female"
        //                                        ,
        //                                     ClassRequiredKey = Application.ClassRequired ? 1 : 0
        //                                        ,
        //                                     ClassRequired = Application.ClassRequired == true ? EduSuiteUIResources.Yes : EduSuiteUIResources.No
        //                                        ,
        //                                     CurrentYearText = CommonUtilities.GetYearDescriptionByCodeDetails(Application.CourseDuration ?? 0, Application.CurrentYear, Application.AcademicTermKey)
        //                                        ,
        //                                     AbscondersDate = Application.AbscondersDate
        //                                     ,
        //                                     StudentsAbscondersRemarks = Application.StudentAbscondersRemarks
        //                                     ,
        //                                     AttachmentPath = Application.AttachmentPath
        //                                     ,
        //                                     IsAbsconders = Application.IsAbsconders
        //                                      ,
        //                                     AbscondersRemarks = Application.AbscondersRemarks
        //                                 }).ToList();



        //    model.TotalRecords = objTotalRecords.Value != DBNull.Value ? Convert.ToInt64(objTotalRecords.Value) : 0;
        //    //StudentSummaryReports = StudentSummaryReports.OrderByDescending(Row => Row.DateAdded).Skip(skip).Take(Take).ToList();



        //    return StudentAbscondersSummary;
        //}
        #endregion Student Absconders Summary

        #region Student EarlyDeparture Summary
        public List<dynamic> GetStudentEarlyDepartureSummary(ReportViewModel model, out long TotalRecords)
        {
            try
            {
                List<dynamic> applicationList = new List<dynamic>();
                DbParameter TotalRecordsParam = null;

                if (model.sidx != "")
                {
                    model.sidx = model.sidx + " " + model.sord;
                }
                dbContext.LoadStoredProc("dbo.Sp_StudentEarlyDepartureSummary_Report")
                    .WithSqlParam("@CourseKeyList", String.Join(",", model.CourseKeys))
                    .WithSqlParam("@CourseTypeKeyList", String.Join(",", model.CourseTypeKeys))
                    .WithSqlParam("@UniversityMasterKeyList", String.Join(",", model.UniversityMasterKeys))
                    .WithSqlParam("@BatchKeyList", String.Join(",", model.BatchKeys))
                    .WithSqlParam("@ReligionKeyList", String.Join(",", model.ReligionKeys))
                    .WithSqlParam("@IncomeKeyList", String.Join(",", model.IncomeGroupKeys))
                    .WithSqlParam("@SecondLanguageKeyList", String.Join(",", model.SecondLanguageKeys))
                    .WithSqlParam("@ModeKeyList", String.Join(",", model.ModeKeys))
                    .WithSqlParam("@ClassModeKeyList", String.Join(",", model.ClassModeKeys))
                    .WithSqlParam("@NatureOfEnquiryKeyList", String.Join(",", model.NatureOfEnquiryKeys))
                    .WithSqlParam("@BranchKeyList", String.Join(",", model.BranchKeys))
                    .WithSqlParam("@AgentKeyList", String.Join(",", model.AgentKeys))
                    .WithSqlParam("@StudentStatusKeyList", String.Join(",", model.StudentStatusKeys))
                    .WithSqlParam("@MediumkeyList", String.Join(",", model.MeadiumKeys))
                    .WithSqlParam("@ClassDetailsKeyList", String.Join(",", model.ClassKeys))
                    .WithSqlParam("@AcademicTermKey", model.AcademicTermKey)
                    .WithSqlParam("@CourseYearsKeyList", String.Join(",", model.CourseYearsKeys))
                    .WithSqlParam("@GenderKey", model.GenderKey)
                    .WithSqlParam("@ClassRequiredKey", model.ClassRequiredKey)
                    .WithSqlParam("@AdmissionDate", (model.DateOfAdmission != null ? Convert.ToDateTime(model.DateOfAdmission).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@DateAddedFrom", (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@DateAddedTo", (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@SearchAnyText", model.SearchAnyText.VerifyData())
                    .WithSqlParam("@AttendanceTypeKeyList", String.Join(",", model.AttendanceTypeKeys))
                    .WithSqlParam("@UserKey", DbConstants.User.UserKey)
                    .WithSqlParam("@PageIndex", model.page)
                    .WithSqlParam("@PageSize", model.rows)
                    .WithSqlParam("@SortBy", model.sidx)
                    .WithSqlParam("@TotalRecords", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalRecordsParam = dbParam;
                    }).ExecuteStoredProc((handler) =>
                    {
                        applicationList = handler.ReadToDynamicList<dynamic>() as List<dynamic>;
                    });

                TotalRecords = Convert.ToInt64((TotalRecordsParam.Value ?? 0));
                return applicationList;
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.StudentsAbscondersReport, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<dynamic>();
            }
        }
        //public List<ReportViewModel> GetStudentEarlyDepartureSummary(ReportViewModel model)
        //{

        //    ObjectParameter objTotalRecords = new ObjectParameter("TotalRecords", typeof(Int64));

        //    if (model.sidx != "")
        //    {
        //        model.sidx = model.sidx + " " + model.sord;
        //    }

        //    var StudentEarlyDepartureSummary = (
        //                                 from Application in dbContext.Sp_StudentEarlyDepartureSummary_Report
        //                                     (
        //                                       String.Join(",", model.CourseKeys),
        //                                       String.Join(",", model.CourseTypeKeys),
        //                                       String.Join(",", model.UniversityMasterKeys),
        //                                       String.Join(",", model.BatchKeys),
        //                                       String.Join(",", model.ReligionKeys),
        //                                       String.Join(",", model.IncomeGroupKeys),
        //                                       String.Join(",", model.SecondLanguageKeys),
        //                                       String.Join(",", model.ModeKeys),
        //                                       String.Join(",", model.ClassModeKeys),
        //                                       String.Join(",", model.NatureOfEnquiryKeys),
        //                                       String.Join(",", model.BatchKeys),
        //                                       String.Join(",", model.AgentKeys),
        //                                       String.Join(",", model.StudentStatusKeys),
        //                                       String.Join(",", model.MeadiumKeys),
        //                                       String.Join(",", model.ClassKeys),
        //                                        model.AcademicTermKey,
        //                                       String.Join(",", model.CourseYearsKeys),
        //                                        model.GenderKey,
        //                                        model.ClassRequiredKey,
        //                                        (model.DateOfAdmission != null ? Convert.ToDateTime(model.DateOfAdmission).ToString("yyyy-MM-dd") : ""),
        //                                        (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""),
        //                                        (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""),
        //                                        model.SearchAnyText.VerifyData(),
        //                                        String.Join(",", model.AttendanceTypeKeys),
        //                                        model.page,
        //                                        model.rows,
        //                                        model.sidx,
        //                                        objTotalRecords
        //                                     )
        //                                 select new ReportViewModel
        //                                 {
        //                                     RowKey = Application.RowKey
        //                                        ,
        //                                     ApplicationNo = Application.ApplicationNo
        //                                        ,
        //                                     Name = Application.StudentName
        //                                        ,
        //                                     GuardianName = Application.StudentGuardian
        //                                        ,
        //                                     MotherName = Application.StudentMotherName
        //                                        ,
        //                                     PermanentAddress = Application.StudentPermanentAddress
        //                                        ,
        //                                     PresentAddress = Application.StudentPresentAddress
        //                                        ,
        //                                     Email = Application.StudentEmail
        //                                        ,
        //                                     Phone = Application.StudentPhone
        //                                        ,
        //                                     Mobile = Application.StudentMobile
        //                                        ,
        //                                     DOB = Application.StudentDOB
        //                                        ,
        //                                     StartYear = Application.StartYear
        //                                        ,
        //                                     TotalFee = Application.StudentTotalFee
        //                                        ,
        //                                     ClassRequiredDesc = Application.StudentClassRequiredDesc
        //                                        ,
        //                                     PresentJob_CourseOfStudyId = Application.PresentJob_CourseOfStudyId
        //                                        ,
        //                                     DateOfAdmission = Application.StudentDateOfAdmission
        //                                        ,
        //                                     StudyMaterialIssueStatus = Application.StudyMaterialIssueStatus
        //                                        ,
        //                                     EnrollmentNo = Application.StudentEnrollmentNo
        //                                        ,
        //                                     PhotoPath = Application.StudentPhotoPath
        //                                        ,
        //                                     ExamRegisterNo = Application.ExamRegisterNo
        //                                        ,
        //                                     Remarks = Application.Remarks
        //                                        ,
        //                                     AdmissionNo = Application.AdmissionNo
        //                                        ,
        //                                     SerialNumber = Application.SerialNumber
        //                                        ,
        //                                     CurrentYear = Application.CurrentYear
        //                                        ,
        //                                     RollNumber = Application.RollNumber
        //                                        ,
        //                                     AcademicTermKey = Application.AcademicTermKey
        //                                        ,
        //                                     DateAdded = Application.DateAdded
        //                                        ,
        //                                     Course = Application.CourseName
        //                                        ,
        //                                     CourseType = Application.CourseTypeName
        //                                        ,
        //                                     Affiliations = Application.UniversityMasterName
        //                                        ,
        //                                     Batch = Application.BatchName
        //                                        ,
        //                                     Mode = Application.ModeName
        //                                        ,
        //                                     ClassMode = Application.ClassModeName
        //                                        ,
        //                                     Religion = Application.ReligionName
        //                                        ,
        //                                     AcademicTerm = Application.AcademicTermName
        //                                        ,
        //                                     SecondLanguage = Application.SecondLanguageName
        //                                        ,
        //                                     Medium = Application.MediumName
        //                                        ,
        //                                     Income = Application.IncomeName
        //                                        ,
        //                                     NatureOfEnquiry = Application.NatureOfEnquiryName
        //                                        ,
        //                                     Agent = Application.AgentName
        //                                        ,
        //                                     StudentStatus = Application.StudentStatusName
        //                                        ,
        //                                     BranchName = Application.BranchName
        //                                        ,
        //                                     Class = Application.ClassCode
        //                                        ,
        //                                     Gender = Application.StudentGender == 1 ? "Male" : "Female"
        //                                        ,
        //                                     ClassRequiredKey = Application.ClassRequired ? 1 : 0
        //                                        ,
        //                                     ClassRequired = Application.ClassRequired == true ? EduSuiteUIResources.Yes : EduSuiteUIResources.No
        //                                        ,
        //                                     CurrentYearText = CommonUtilities.GetYearDescriptionByCodeDetails(Application.CourseDuration ?? 0, Application.CurrentYear, Application.AcademicTermKey)
        //                                        ,
        //                                     EarlyDepartureDate = Application.EarlyDepartureDate
        //                                     ,
        //                                     EarlyDepartureTime = Application.EarlyDepartureTime
        //                                     ,
        //                                     AttachmentPath = Application.AttachmentPath
        //                                     ,
        //                                     EarlyDepartureRemarks = Application.EarlyDepartureRemarks
        //                                 }).ToList();



        //    model.TotalRecords = objTotalRecords.Value != DBNull.Value ? Convert.ToInt64(objTotalRecords.Value) : 0;
        //    //StudentSummaryReports = StudentSummaryReports.OrderByDescending(Row => Row.DateAdded).Skip(skip).Take(Take).ToList();



        //    return StudentEarlyDepartureSummary;
        //}
        #endregion Student EarlyDeparture Summary


        #region Student LeaveLateAbscondersED Summary
        public List<dynamic> GetStudentSummary_For_LeaveLateAbscondersED(ReportViewModel model, out long TotalRecords)
        {
            try
            {
                List<dynamic> applicationList = new List<dynamic>();
                DbParameter TotalRecordsParam = null;

                if (model.sidx != "")
                {
                    model.sidx = model.sidx + " " + model.sord;
                }
                dbContext.LoadStoredProc("dbo.Sp_StudentSummary_For_LeaveLateAbscondersED")
                    .WithSqlParam("@CourseKeyList", String.Join(",", model.CourseKeys))
                    .WithSqlParam("@CourseTypeKeyList", String.Join(",", model.CourseTypeKeys))
                    .WithSqlParam("@UniversityMasterKeyList", String.Join(",", model.UniversityMasterKeys))
                    .WithSqlParam("@BatchKeyList", String.Join(",", model.BatchKeys))
                    .WithSqlParam("@ReligionKeyList", String.Join(",", model.ReligionKeys))
                    .WithSqlParam("@IncomeKeyList", String.Join(",", model.IncomeGroupKeys))
                    .WithSqlParam("@SecondLanguageKeyList", String.Join(",", model.SecondLanguageKeys))
                    .WithSqlParam("@ModeKeyList", String.Join(",", model.ModeKeys))
                    .WithSqlParam("@ClassModeKeyList", String.Join(",", model.ClassModeKeys))
                    .WithSqlParam("@NatureOfEnquiryKeyList", String.Join(",", model.NatureOfEnquiryKeys))
                    .WithSqlParam("@BranchKeyList", String.Join(",", model.BranchKeys))
                    .WithSqlParam("@AgentKeyList", String.Join(",", model.AgentKeys))
                    .WithSqlParam("@StudentStatusKeyList", String.Join(",", model.StudentStatusKeys))
                    .WithSqlParam("@MediumkeyList", String.Join(",", model.MeadiumKeys))
                    .WithSqlParam("@ClassDetailsKeyList", String.Join(",", model.ClassKeys))
                    .WithSqlParam("@AcademicTermKey", model.AcademicTermKey)
                    .WithSqlParam("@CourseYearsKeyList", String.Join(",", model.CourseYearsKeys))
                    .WithSqlParam("@GenderKey", model.GenderKey)
                    .WithSqlParam("@ClassRequiredKey", model.ClassRequiredKey)
                    .WithSqlParam("@AdmissionDate", (model.DateOfAdmission != null ? Convert.ToDateTime(model.DateOfAdmission).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@DateAddedFrom", (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@DateAddedTo", (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@SearchAnyText", model.SearchAnyText.VerifyData())
                    .WithSqlParam("@AttendanceTypeKeyList", String.Join(",", model.AttendanceTypeKeys))
                    .WithSqlParam("@ReportType", model.ReportType)
                    .WithSqlParam("@UserKey", DbConstants.User.UserKey)
                    .WithSqlParam("@PageIndex", model.page)
                    .WithSqlParam("@PageSize", model.rows)
                    .WithSqlParam("@SortBy", model.sidx)
                    .WithSqlParam("@TotalRecords", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalRecordsParam = dbParam;
                    }).ExecuteStoredProc((handler) =>
                    {
                        applicationList = handler.ReadToDynamicList<dynamic>() as List<dynamic>;
                    });

                TotalRecords = Convert.ToInt64((TotalRecordsParam.Value ?? 0));
                return applicationList;
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.StudentsAbscondersReport, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<dynamic>();
            }
        }

        #endregion Student LeaveLateAbscondersED Summary

        public List<dynamic> GeTotalFeeCollectionSummary(ReportViewModel model)
        {
            try
            {

                List<dynamic> DayToDayFees = new List<dynamic>();

                DbParameter TotalRecordsParam = null;

                if (model.sidx != null)
                {
                    model.sidx = model.sidx + " " + model.sord;
                }


                dbContext.LoadStoredProc("dbo.Sp_StudentDayToDayFeePaymentSummary_For_SN")
                    .WithSqlParam("@CourseKeyList", String.Join(",", model.CourseKeys))
                    .WithSqlParam("@CourseTypeKeyList", String.Join(",", model.CourseTypeKeys))
                    .WithSqlParam("@BatchKeyList", String.Join(",", model.BatchKeys))
                    .WithSqlParam("@BranchKeyList", String.Join(",", model.BranchKeys))
                    .WithSqlParam("@StudentStatusKeyList", String.Join(",", model.StudentStatusKeys))
                    .WithSqlParam("@ClassDetailsKeyList", String.Join(",", model.ClassKeys))
                    .WithSqlParam("@GenderKey", model.GenderKey)
                    .WithSqlParam("@DateAddedFrom", (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@DateAddedTo", (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@SearchAnyText", model.SearchAnyText.VerifyData())
                    .WithSqlParam("@PaymentModeKeyList", String.Join(",", model.PaymentModeKeys))
                    .WithSqlParam("@FeeTypeKeyList", String.Join(",", model.FeeTypeKeys))
                    .WithSqlParam("@PageIndex", model.page)
                    .WithSqlParam("@PageSize", model.rows)
                    .WithSqlParam("@SortBy", model.sidx)

                    .WithSqlParam("@TotalRecords", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalRecordsParam = dbParam;
                    }).ExecuteStoredProc((handler) =>
                    {
                        DayToDayFees = handler.ReadToDynamicList<dynamic>() as List<dynamic>;

                    });
                model.TotalRecords = Convert.ToInt64((TotalRecordsParam.Value ?? 0));
                return DayToDayFees;
            }
            catch (Exception ex)
            {

                ActivityLog.CreateActivityLog(MenuConstants.StudentsFeeSummary, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<dynamic>();
            }
        }

        public void BindHeading(ReportViewModel model)
        {
            model.FeeTypes = dbContext.FeePaymentDetails.Where(x => x.FeeType.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.FeeType.RowKey,
                Text = row.FeeType.FeeTypeName,

            }).Distinct().ToList();
        }

        #region Fee Refund Report

        public List<dynamic> FeeRefundSummary(ReportViewModel model, out long TotalRecords)
        {
            try
            {
                List<dynamic> applicationList = new List<dynamic>();
                DbParameter TotalRecordsParam = null;

                if (model.sidx != "")
                {
                    model.sidx = model.sidx + " " + model.sord;
                }
                dbContext.LoadStoredProc("dbo.Sp_StudentFeeRefundSummary")
                     .WithSqlParam("@CourseKeyList", String.Join(",", model.CourseKeys))
                    .WithSqlParam("@CourseTypeKeyList", String.Join(",", model.CourseTypeKeys))
                    .WithSqlParam("@UniversityMasterKeyList", String.Join(",", model.UniversityMasterKeys))
                    .WithSqlParam("@BatchKeyList", String.Join(",", model.BatchKeys))
                    .WithSqlParam("@ModeKeyList", String.Join(",", model.ModeKeys))
                    .WithSqlParam("@BranchKeyList", String.Join(",", model.BranchKeys))
                    .WithSqlParam("@AgentKeyList", String.Join(",", model.AgentKeys))
                    .WithSqlParam("@StudentStatusKeyList", String.Join(",", model.StudentStatusKeys))
                    .WithSqlParam("@MediumkeyList", String.Join(",", model.MeadiumKeys))
                    .WithSqlParam("@ClassDetailsKeyList", String.Join(",", model.ClassKeys))
                    .WithSqlParam("@AcademicTermKey", model.AcademicTermKey)
                    .WithSqlParam("@CourseYearsKeyList", String.Join(",", model.CourseYearsKeys))
                    .WithSqlParam("@GenderKey", model.GenderKey)
                    .WithSqlParam("@AdmissionDate", (model.DateOfAdmission != null ? Convert.ToDateTime(model.DateOfAdmission).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@DateAddedFrom", (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@DateAddedTo", (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@SearchAnyText", model.SearchAnyText.VerifyData())
                    .WithSqlParam("@PaymentStatus", model.PaymentStatus)
                    .WithSqlParam("@ProcessStatusKey", model.ProcessStatusKey)
                    .WithSqlParam("@FeeRefundTabKey", model.FeeRefundTabKey)
                    .WithSqlParam("@PaymentModeKeyList", String.Join(",", model.PaymentModeKeys))
                    .WithSqlParam("@FeeTypeKeyList", String.Join(",", model.FeeTypeKeys))
                    .WithSqlParam("@PageIndex", model.page)
                    .WithSqlParam("@PageSize", model.rows)
                    .WithSqlParam("@SortBy", model.sidx)
                    .WithSqlParam("@TotalRecords", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalRecordsParam = dbParam;
                    }).ExecuteStoredProc((handler) =>
                    {
                        applicationList = handler.ReadToDynamicList<dynamic>() as List<dynamic>;
                        //applicationList = handler.ReadToList<ApplicationViewModel>() as List<ApplicationViewModel>;
                    });
                TotalRecords = Convert.ToInt64((TotalRecordsParam.Value ?? 0));
                return applicationList;
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.StudentsAttendanceSummary, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<dynamic>();
            }
        }


        private void FillProcessStatus(ReportViewModel model)
        {
            model.ProcessStatuses = dbContext.ProcessStatus.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ProcessStatusName
            }).ToList();
        }

        public List<FeeRefundDetailViewModel> FillFeeRefundDetails(FeeRefundViewModel model)
        {
            List<FeeRefundDetailViewModel> FeeRefundDetailList = new List<FeeRefundDetailViewModel>();
            long Applicationkey = dbContext.FeeRefundMasters.Where(row => row.RowKey == model.RowKey).Select(x => x.ApplicationKey).SingleOrDefault();
            Application Application = dbContext.Applications.SingleOrDefault(row => row.RowKey == Applicationkey);

            FeeRefundDetailList = dbContext.FeeRefundDetails.Where(row => row.FeeRefundMasterKey == model.RowKey)
                .Select(row => new FeeRefundDetailViewModel
                {
                    RowKey = row.RowKey,
                    FeeTypeKey = row.FeeTypeKey,
                    ReturnAmount = row.FeeAmount,
                    FeeYear = row.FeeYear,
                    FeeTypeName = row.FeeType.FeeTypeName,
                    RecieptNo = row.FeePaymentDetail.FeePaymentMaster.ReceiptNo,
                    IsFeeTypeYear = row.FeeType.FeeTypeModeKey == DbConstants.FeeTypeMode.Single ? true : false
                }).ToList();
            foreach (FeeRefundDetailViewModel PaymentDetails in model.FeeRefundDetails)
            {
                PaymentDetails.FeeYearText = PaymentDetails.FeeYear != null ? (CommonUtilities.GetYearDescriptionByCodeDetails(Application.Course.CourseDuration ?? 0, PaymentDetails.FeeYear ?? 0, Application.AcademicTermKey)) : "";

            }

            return FeeRefundDetailList;
        }

        #endregion Fee Refund Report

        #region Enquiry Lead Count Summary

        public List<dynamic> GetEnquiryLeadSummary(EnquiryReportViewModel model, out long TotalRecords)
        {
            try
            {
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).FirstOrDefault();
                string EmployeeAccessList = "";
                if (DbConstants.User.RoleKey == DbConstants.Role.Staff)
                {
                    if (Employee != null)
                    {
                        List<long> EmployeeKeys = new List<long>();
                        EmployeeKeys = dbContext.EmployeeHierarchies.Where(x => x.EmployeeKey == Employee.RowKey).Select(y => y.ToEmployeeKey ?? 0).ToList();
                        EmployeeKeys.Add(Employee.RowKey);
                        if (EmployeeKeys.Count > 0)
                        {
                            EmployeeAccessList = String.Join(",", EmployeeKeys);
                        }

                    }
                }

                List<dynamic> enquiryLeadSummaryList = new List<dynamic>();
                DbParameter TotalRecordsParam = null;

                if (model.sidx != "")
                {
                    model.sidx = model.sidx + " " + model.sord;
                }
                dbContext.LoadStoredProc("dbo.SP_EnquiryLeadSummary")
                    .WithSqlParam("@UserKey", DbConstants.User.UserKey)
                    .WithSqlParam("@RoleKey", DbConstants.User.RoleKey)
                    .WithSqlParam("@BranchKey", model.BranchKey)
                    .WithSqlParam("@EmployeeKey", model.EmployeeKey)
                    .WithSqlParam("@FromDate", (model.SearchFromDate != null ? Convert.ToDateTime(model.SearchFromDate).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@ToDate", (model.SearchToDate != null ? Convert.ToDateTime(model.SearchToDate).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@PageIndex", model.page)
                    .WithSqlParam("@PageSize", model.rows)
                    .WithSqlParam("@SortBy", model.sidx)
                    .WithSqlParam("@EmployeeAccessKeys", EmployeeAccessList)
                    .WithSqlParam("@HideRecords", model.IsSelected)
                    .WithSqlParam("@TotalRecords", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalRecordsParam = dbParam;
                    }).ExecuteStoredProc((handler) =>
                    {
                        enquiryLeadSummaryList = handler.ReadToDynamicList<dynamic>() as List<dynamic>;

                    });
                TotalRecords = Convert.ToInt64((TotalRecordsParam.Value ?? 0));
                return enquiryLeadSummaryList;
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.EnquiryLeadCountSummary, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<dynamic>();
            }
        }
        #endregion

        #region Enquiry Count Summary

        public List<dynamic> GetEnquirySummary(EnquiryReportViewModel model, out long TotalRecords)
        {
            try
            {
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).FirstOrDefault();
                string EmployeeAccessList = "";
                if (DbConstants.User.RoleKey == DbConstants.Role.Staff)
                {
                    if (Employee != null)
                    {
                        List<long> EmployeeKeys = new List<long>();
                        EmployeeKeys = dbContext.EmployeeHierarchies.Where(x => x.EmployeeKey == Employee.RowKey).Select(y => y.ToEmployeeKey ?? 0).ToList();
                        EmployeeKeys.Add(Employee.RowKey);
                        if (EmployeeKeys.Count > 0)
                        {
                            EmployeeAccessList = String.Join(",", EmployeeKeys);
                        }
                    }
                }

                List<dynamic> enquirySummaryList = new List<dynamic>();
                DbParameter TotalRecordsParam = null;

                if (model.sidx != "")
                {
                    model.sidx = model.sidx + " " + model.sord;
                }
                dbContext.LoadStoredProc("dbo.SP_EnquirySummary")
                    .WithSqlParam("@UserKey", DbConstants.User.UserKey)
                    .WithSqlParam("@RoleKey", DbConstants.User.RoleKey)
                    .WithSqlParam("@BranchKey", model.BranchKey)
                    .WithSqlParam("@EmployeeKey", model.EmployeeKey)
                    .WithSqlParam("@FromDate", (model.SearchFromDate != null ? Convert.ToDateTime(model.SearchFromDate).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@ToDate", (model.SearchToDate != null ? Convert.ToDateTime(model.SearchToDate).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@PageIndex", model.page)
                    .WithSqlParam("@PageSize", model.rows)
                    .WithSqlParam("@SortBy", model.sidx)
                    .WithSqlParam("@EmployeeAccessKeys", EmployeeAccessList)
                    .WithSqlParam("@HideRecords", model.IsSelected)
                    .WithSqlParam("@TotalRecords", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalRecordsParam = dbParam;
                    }).ExecuteStoredProc((handler) =>
                    {
                        enquirySummaryList = handler.ReadToDynamicList<dynamic>() as List<dynamic>;
                    });
                TotalRecords = Convert.ToInt64((TotalRecordsParam.Value ?? 0));
                return enquirySummaryList;
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.EnquiryCountSummary, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<dynamic>();
            }
        }
        #endregion

        #region Enquiry And Lead Details

        public List<EnquiryReportViewModel> GetEnquiryandLeadDetails(EnquiryReportViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.rows;
                var Skip = (model.page - 1) * model.rows;


                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
                string EmployeeAccessList = "";
                //if (DbConstants.User.RoleKey == DbConstants.Role.Staff)
                //{
                //    if (Employee != null)
                //    {
                //        if (!DbConstants.Role.AdminUserTypes.Contains(DbConstants.User.RoleKey))
                //        {
                //            var ChildEmployees = dbContext.fnChildEmployees(DbConstants.User.UserKey).Where(row => (row.BranchKey == model.BranchKey || model.BranchKey == 0)).Select(row => new GroupSelectListModel
                //            {
                //                RowKey = row.RowKey ?? 0,
                //                Text = row.EmployeeName,

                //            }).OrderBy(row => row.Text).ToList();

                //            List<long> branches = Employee.BranchAccess.ToString().Split(',').Select(Int64.Parse).ToList();

                //            var OtherEmployees = dbContext.Employees.Where(row => branches.Contains(row.BranchKey) && row.BranchKey != Employee.BranchKey).Select(row => new GroupSelectListModel
                //            {
                //                RowKey = row.RowKey,
                //                Text = row.FirstName + " " + (row.MiddleName ?? "") + " " + row.LastName,

                //            }).OrderBy(row => row.Text).ToList();

                //            EmployeeAccessList = String.Join(",", OtherEmployees.Select(x => x.RowKey).ToList().Union(ChildEmployees.Select(x => x.RowKey)));
                //        }
                //    }
                //}




                string FromDate = model.SearchFromDate != null ? Convert.ToDateTime(model.SearchFromDate).ToString("yyyy-MM-dd") : null;
                string ToDate = model.SearchToDate != null ? Convert.ToDateTime(model.SearchToDate).ToString("yyyy-MM-dd") : null;

                ObjectParameter TotalRecordsCount = new ObjectParameter("TotalRecordCount", typeof(Int64));
                dbContext.Database.CommandTimeout = 0;
                var EnquiryReport = dbContext.SP_EnquiryandLeadSelectDetails(
                      model.ScheduleTypeKeysList,
                      model.SearchDateTypeKey,
                      model.EmployeeFilterTypeKey,
                      DbConstants.EnquiryStatus.FollowUp,
                      DbConstants.EnquiryStatus.AdmissionTaken,
                      DbConstants.EnquiryStatus.Intersted,
                      DbConstants.EnquiryStatus.Closed,
                      DbConstants.ProductiveCalls.Limit,
                      model.SearchEmployeeKey,
                      model.SearchScheduledEmployeeKey,
                      model.SearchBranchKey,
                      model.SearchCounselllingBranchKey,
                      VerifyData(model.SearchLocation),
                      VerifyData(model.SearchAnyText),
                      model.SearchApplicationStatusKey,
                      model.ApplicationStatusKeysList,
                      model.SearchEnquiryStatusKey,
                      model.SearchCallStatusKey,
                      model.SearchCallTypeKey,
                      FromDate,
                      ToDate,
                      model.PageIndex,
                      model.PageSize,
                      TotalRecordsCount,
                      model.ReminderStatusKey,
                      model.SearchIsClosePending,
                      model.SearchIsOnCallStatusVise,
                      DbConstants.User.UserKey,
                      DbConstants.User.RoleKey,
                      EmployeeAccessList
                      ).Select(row => new EnquiryReportViewModel
                      {
                          RowNumber = row.RowNumber ?? 0,
                          EmployeeName = row.EmployeeName,
                          Name = row.Name,
                          MobileNumber = row.MobileNumber,
                          Email = row.Email,
                          Qualificatin = row.Qualification,
                          Branch = row.Branch,
                          AcademicTermName = row.ServiceTypeName,
                          Country = row.Country,
                          Program = row.Program,
                          District = row.District,
                          Location = row.Location,

                          CallStatusName = row.LastCallStatus,
                          ScheduleTypeName = row.ScheduleTypeName,
                          NextCallSchedule = row.NextCallSchedule,
                          CallTypeName = row.CallTypeName,
                          Feedback = row.CallFeedback,
                          CallDuration = row.CallDuration,
                          CalledDate = row.CalledDate,
                          EmployeeKey = row.EmployeeKey ?? 0,
                          PostedBy = row.CreatedByEmployeeName,
                          ScheduledEmployeeName = row.ScheduledEmployeeName,
                          IsClosePending = row.IsClose ?? false,
                          StatusName = row.StatusName,
                          CounsellingBranchName = row.CounsellingBranchName,
                          EnquiryStatusOnCall = row.EnquiryStatusNameOnCall,

                          EnquiryStatusKey = row.EnquiryStatusKey,
                          ApplicationStatusName = row.ApplicationStatusName,
                          CounsellingTime = row.CounsellingTime,
                          //EnquiryCounsellingDate = row.EnquiryCounsellingDate,
                          //EnquiryCounsellingCalledDate = row.EnquiryCounsellingCalledDate,
                          CreatedOn = row.CreatedOn,
                          ApplicationStatusKey = row.ApplicationStatusKey,
                          TelephoneCodeKey = row.TelephoneCodeKey,
                          RowKey = row.RowKey ?? 0

                      }).ToList();

                model.TotalRecordsCount = TotalRecordsCount.Value != DBNull.Value ? Convert.ToInt64(TotalRecordsCount.Value) : 0;


                TotalRecords = 0;


                return EnquiryReport.ToList();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.EnquiryLeadCountSummary, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);

                TotalRecords = 0;
                return new List<EnquiryReportViewModel>();

            }
        }

        #endregion Enquiry And Lead Details

        #region CashFlow Summary

        public List<dynamic> GetCashFlowSummary(ReportViewModel model, out long TotalRecords)
        {
            try
            {
                List<dynamic> cashflowList = new List<dynamic>();
                DbParameter TotalRecordsParam = null;
                DbParameter TotalPaidSumParam = null;

                if (model.sidx != "")
                {
                    model.sidx = model.sidx + " " + model.sord;
                }
                dbContext.LoadStoredProc("dbo.Sp_CashflowSummary")
                    .WithSqlParam("@AccountHeadKeyList", String.Join(",", model.AccountHeadKeys))
                    .WithSqlParam("@PaymentModeKeyList", String.Join(",", model.PaymentModeKeys))
                    .WithSqlParam("@CashFlowTypeKeyList", String.Join(",", model.CashFlowTypeKeys))
                    .WithSqlParam("@BankKeyList", String.Join(",", model.BankAccountKeys))
                    .WithSqlParam("@BranchKeyList", String.Join(",", model.BranchKeys))
                    .WithSqlParam("@DateAddedFrom", (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@DateAddedTo", (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@SearchAnyText", model.SearchAnyText.VerifyData())
                    .WithSqlParam("@UserKey", DbConstants.User.UserKey)
                    .WithSqlParam("@RoleKey", DbConstants.User.RoleKey)
                    .WithSqlParam("@PageIndex", model.page)
                    .WithSqlParam("@PageSize", model.rows)
                    .WithSqlParam("@SortBy", model.sidx)
                    .WithSqlParam("@TotalRecords", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalRecordsParam = dbParam;
                    }).WithSqlParam("@TotalAmountSum", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalPaidSumParam = dbParam;
                    })
                    .ExecuteStoredProc((handler) =>
                    {
                        cashflowList = handler.ReadToDynamicList<dynamic>() as List<dynamic>;
                    });
                TotalRecords = Convert.ToInt64((TotalRecordsParam.Value ?? 0));
                model.TotalAmount = Convert.ToInt64((TotalPaidSumParam.Value ?? 0));
                return cashflowList;
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.CashFlowSummary, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<dynamic>();
            }
        }

        public void FillAccountHead(ReportViewModel model)
        {
            List<long> AccountHeadKeys = new List<long>();
            string accoundheadkeys = string.Join(",", dbContext.FeeTypes.Where(y => y.AccountHeadKey != null).Select(x => x.AccountHeadKey).ToList());
            string employeeaccoundheadkeys = string.Join(",", dbContext.Employees.Where(y => y.AccountHeadKey != null).Select(x => x.AccountHeadKey).ToList());
            if (employeeaccoundheadkeys != "")
            {
                accoundheadkeys = accoundheadkeys + "," + employeeaccoundheadkeys;
            }


            if (accoundheadkeys != "")
            {
                AccountHeadKeys = accoundheadkeys.Split(',').Select(Int64.Parse).ToList();
            }

            model.AccountHeads = (from ac in dbContext.AccountHeads
                                  where !AccountHeadKeys.Contains(ac.RowKey)
                                  select new SelectListModel
                                  {
                                      RowKey = ac.RowKey,
                                      Text = ac.AccountHeadName
                                  }).ToList();
        }

        public void FillBankAccount(ReportViewModel model)
        {
            model.BankAccounts = (from ac in dbContext.BankAccounts

                                  select new SelectListModel
                                  {
                                      RowKey = ac.RowKey,
                                      Text = ac.AccountNumber + "" + ac.Bank.BankName
                                  }).ToList();
        }

        private void FillCashFlowType(ReportViewModel model)
        {
            model.CashFlowTypes = dbContext.CashFlowTypes.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.RowKey == DbConstants.CashFlowType.In ? EduSuiteUIResources.Receipt : EduSuiteUIResources.Payment
            }).ToList();
        }
        #endregion

        #region Employee Enquiry Target Summary
        public List<dynamic> GetEmployeeEnquiryTargetSummary(ReportViewModel model)
        {
            try
            {
                List<dynamic> EnquiryTargetSummary = new List<dynamic>();

                if (model.sidx != "")
                {
                    model.sidx = model.sidx + " " + model.sord;
                }
                dbContext.LoadStoredProc("dbo.SP_EnquiryTargetSummary")
                    .WithSqlParam("@Date", (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@EmployeeKey", model.EmployeeKey)
                    .WithSqlParam("@UserKey", DbConstants.User.UserKey)
                    .WithSqlParam("@RoleKey", DbConstants.User.RoleKey)
                    .WithSqlParam("@BranchKey", model.BranchKey)
                    .ExecuteStoredProc((handler) =>
                    {
                        EnquiryTargetSummary = handler.ReadToDynamicList<dynamic>() as List<dynamic>;
                    });

                return EnquiryTargetSummary;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.EnquiryTargetSummary, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<dynamic>();
            }


        }
        #endregion

        #region Fee Installment Summary
        public List<ReportViewModel> GetFeeInstallmentSummary(ReportViewModel model)
        {
            try
            {
                var Take = model.rows;
                var Skip = (model.page - 1) * model.rows;

                DateTime InstallmentDate = Convert.ToDateTime(model.DateAdded);
                int year = InstallmentDate.Year;
                int month = InstallmentDate.Month;
                IEnumerable<ReportViewModel> applicationList = (from a in dbContext.SP_FeeInstallmentSummary(model.DateAdded, DbConstants.User.UserKey)
                                                                select new ReportViewModel
                                                                {
                                                                    RowKey = a.ApplicationKey,
                                                                    CourseKey = a.CourseKey,
                                                                    UniversityMasterKey = a.UniversityMasterKey,
                                                                    AcademicTermKey = a.AcademicTermKey,
                                                                    CourseTypeKey = a.CourseTypeKey,
                                                                    ReligionKey = a.ReligionKey,
                                                                    StudentStatusKey = a.StudentStatusKey,
                                                                    ModeKey = a.ModeKey,
                                                                    BranchKey = a.BranchKey,
                                                                    BatchKey = a.BatchKey,
                                                                    ClassDetailsKey = a.ClassDetailsKey,
                                                                    AgentKey = a.AgentKey,
                                                                    CasteKey = a.CasteKey,
                                                                    CurrentYear = a.CurrentYear,
                                                                    Gender = a.Gender == 1 ? "Male" : "Female",
                                                                    AdmissionNo = a.AdmissionNo,
                                                                    Email = a.Email,
                                                                    Name = a.name,
                                                                    AcademicTerm = a.AcademicTerm,
                                                                    CourseType = a.CourseType,
                                                                    CurrentYearText = a.CurrentYearText,
                                                                    Mode = a.Mode,
                                                                    Agent = a.Agent,
                                                                    CasteName = a.CasteName,
                                                                    Religion = a.Religion,
                                                                    Course = a.Course,
                                                                    Affiliations = a.Affiliations,
                                                                    Mobile = a.Mobile,
                                                                    StudentStatus = a.StudentStatus,
                                                                    BatchName = a.Batch,
                                                                    BranchName = a.BranchName,
                                                                    Class = a.Class,
                                                                    TotalFee = a.StudentTotalFee,
                                                                    TotalPaid = a.TotalPaidFee,
                                                                    BalanceFee = a.TotalBalance,
                                                                    InstallmentNo = a.InstallmentNo,
                                                                    InstallmentMonth = a.InstallmentMonth,
                                                                    InstallmentMonthKey = a.InstallmentMonthKey,
                                                                    InstallmentYear = a.InstallmentYear,
                                                                    FeeYear = a.FeeYear,
                                                                    InstallmentAmount = a.InstallmentAmount,
                                                                    InstallmentPaid = a.InstallmentPaid,
                                                                    BalanceDue = a.BalanceDue,
                                                                    DueDate = a.DueDate,
                                                                    DueFineAmount = a.DueFineAmount,
                                                                    SuperFineAmount = a.SuperFineAmount
                                                                });
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
                if (Employee != null)
                {
                    if (Employee.BranchAccess != null)
                    {
                        var Branches = Employee.BranchAccess.Split(',').Select(Int16.Parse).ToList();
                        applicationList = applicationList.Where(row => Branches.Contains(row.BranchKey ?? 0));
                    }
                }

                if (model.SearchAnyText != null && model.SearchAnyText != "")
                {

                    //applicationList = applicationList.Where(row => row.Name.Contains(model.SearchAnyText) || row.Mobile.Contains(model.SearchAnyText) || row.AdmissionNo.Contains(model.SearchAnyText));
                    //applicationList = applicationList.Where(row => model.SearchAnyText.ToLower().Trim().Contains(row.Name.ToLower()));
                    applicationList = applicationList.Where(row => model.SearchAnyText.ToLower().Trim() == row.Name.ToLower().Trim());

                }
                if (year != null && month != null)
                {
                    applicationList = applicationList.Where(row => row.InstallmentYear == year && row.InstallmentMonthKey == month);
                }
                if (model.PaymentStatus != 0)
                {
                    if (model.PaymentStatus == DbConstants.PaymentStatus.Complete)
                    {
                        applicationList = applicationList.Where(row => row.BalanceDue <= 0);
                    }
                    else
                    {
                        applicationList = applicationList.Where(row => row.BalanceDue > 0);
                    }
                }

                if (model.BranchKeys.Count > 0)
                {
                    applicationList = applicationList.Where(row => model.BranchKeys.Contains(row.BranchKey ?? 0));
                }
                if (model.CourseKeys.Count > 0)
                {
                    applicationList = applicationList.Where(row => model.CourseKeys.Contains(row.CourseKey ?? 0));
                }
                if (model.UniversityMasterKeys.Count > 0)
                {
                    applicationList = applicationList.Where(row => model.UniversityMasterKeys.Contains(row.UniversityMasterKey ?? 0));
                }
                if (model.AcademicTermKey != 0)
                {
                    applicationList = applicationList.Where(row => row.AcademicTermKey == model.AcademicTermKey);
                }
                if (model.CourseTypeKeys.Count > 0)
                {
                    applicationList = applicationList.Where(row => model.CourseTypeKeys.Contains(row.CourseTypeKey ?? 0));
                }
                if (model.BatchKeys.Count > 0)
                {
                    applicationList = applicationList.Where(row => model.BatchKeys.Contains(row.BatchKey ?? 0));
                }
                if (model.StudentStatusKeys.Count > 0)
                {
                    applicationList = applicationList.Where(row => model.StudentStatusKeys.Contains(row.StudentStatusKey ?? 0));
                }
                if (model.ClassKeys.Count > 0)
                {
                    applicationList = applicationList.Where(row => model.ClassKeys.Contains(row.ClassDetailsKey ?? 0));
                }
                if (model.CourseYearsKeys.Count > 0)
                {
                    applicationList = applicationList.Where(row => model.CourseYearsKeys.Contains(row.CurrentYear ?? 0));
                }
                return applicationList.GroupBy(x => x.RowKey).Skip(Skip ?? 0).Take(Take ?? 0).Select(y => y.FirstOrDefault()).ToList<ReportViewModel>();



                //return applicationList.Skip(Skip ?? 0).Take(Take ?? 0).ToList<ReportViewModel>();
            }
            catch (Exception ex)
            {

                ActivityLog.CreateActivityLog(MenuConstants.FeeInstallmentSummary, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<ReportViewModel>();

            }
        }
        public List<ApplicationFeePaymentViewModel> BindInstallmentFeeDetails(ApplicationFeePaymentViewModel model)
        {
            try
            {
                Application Application = dbContext.Applications.SingleOrDefault(row => row.RowKey == model.ApplicationKey);
                List<ApplicationFeePaymentViewModel> TotalInstallmentDetails = new List<ApplicationFeePaymentViewModel>();

                if (Application != null)
                {
                    var CourseDuration = Application.Course.CourseDuration;
                    var duration = Math.Ceiling((Convert.ToDecimal(Application.AcademicTermKey == DbConstants.AcademicTerm.Semester ? CourseDuration / 6 : CourseDuration / 12)));

                    short AcademicTermKey = dbContext.Applications.Where(row => row.RowKey == model.ApplicationKey).Select(row => row.AcademicTermKey).FirstOrDefault();

                    TotalInstallmentDetails = dbContext.Sp_FeeInstallmentDetails_Select_ByType(model.ApplicationKey)
                        .Select(row => new ApplicationFeePaymentViewModel
                        {
                            InitialPaymentYear = row.FeeYear,
                            InitialPaymentYearText = row.FeeYear != null ? (duration < 1 ? "Short Term" : CommonUtilities.GetYearDescriptionByCode(row.FeeYear ?? 0, AcademicTermKey)) : "",
                            InstallmentYear = row.InstallmentYear,
                            InitialPaymentMonth = row.InstallmentMonth,
                            InitialPaymentAmount = row.InstallmentAmount,
                            InitialPaymentAmountPaid = row.InstallmentPaid,
                            InitialPaymentbalanceDue = row.BalanceDue,
                            InitialPaymentDueDate = row.DueDate

                        }).ToList();
                }
                return TotalInstallmentDetails;
            }
            catch (Exception ex)
            {
                return new List<ApplicationFeePaymentViewModel>();
            }
        }
        #endregion

        #region Students Fee Paid and un Paid Summary
        public string GetStudentFeeSummaryByDate(ReportViewModel model)
        {

            IEnumerable<string> results = dbContext.Database.SqlQuery<string>("Sp_FeePaymentSummaryByDate @FromDate,@ToDate,@SearchText,@StudentStatusKeyList,@BranchKeyList,@ClassDetailsKeyList,@CourseKeyList,@UniversityMasterKeyList,@BatchKeyList,@CourseYearsKeyList,@CourseTypeKeyList",
                                                                                     new SqlParameter("FromDate", (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : "")),
                                                                                     new SqlParameter("ToDate", (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : "")),
                                                                                     new SqlParameter("SearchText", model.SearchAnyText.VerifyData()),
                                                                                     new SqlParameter("StudentStatusKeyList", String.Join(",", model.StudentStatusKeys)),
                                                                                     new SqlParameter("BranchKeyList", String.Join(",", model.BranchKeys)),
                                                                                     new SqlParameter("ClassDetailsKeyList", String.Join(",", model.ClassKeys)),
                                                                                     new SqlParameter("CourseKeyList", String.Join(",", model.CourseKeys)),
                                                                                     new SqlParameter("UniversityMasterKeyList", String.Join(",", model.UniversityMasterKeys)),
                                                                                     new SqlParameter("BatchKeyList", String.Join(",", model.BatchKeys)),
                                                                                     new SqlParameter("CourseYearsKeyList", String.Join(",", model.CourseYearsKeys)),
                                                                                     new SqlParameter("CourseTypeKeyList", String.Join(",", model.CourseTypeKeys)),
                                                                                     new SqlParameter("FeeTypeKeyList", String.Join(",", model.FeeTypeKeys))).ToList();


            return String.Join("", results);
        }
        #endregion

        #region Employee Salary Summary


        public List<dynamic> GetEmployeeSalarySummary(ReportViewModel model, out long TotalRecords)
        {
            try
            {
                List<dynamic> cashflowList = new List<dynamic>();
                DbParameter TotalRecordsParam = null;
                DbParameter TotalSalarySum = null;
                DbParameter BalanceSalarySum = null;
                DbParameter TotalPaidSalarySum = null;

                if (model.sidx != "")
                {
                    model.sidx = model.sidx + " " + model.sord;
                }
                dbContext.LoadStoredProc("dbo.SP_EmployeeSalarySummary")
                    .WithSqlParam("@BranchKeyList", String.Join(",", model.BranchKeys))
                    .WithSqlParam("@EmployeeKeyList", String.Join(",", model.EmployeeKeys))
                    .WithSqlParam("@DesignationKeyList", String.Join(",", model.DesignationKeys))
                    .WithSqlParam("@DepartmentKeyList", String.Join(",", model.DepartmentKeys))
                    .WithSqlParam("@EmployeeStatusKeyList", String.Join(",", model.EmployeeStatusKeys))
                    .WithSqlParam("@SalaryYearKeyList", String.Join(",", model.SalaryYearKeys))
                    .WithSqlParam("@SalaryMonthKeyList", String.Join(",", model.SalaryMonthKeys))
                    .WithSqlParam("@SearchAnyText", model.SearchAnyText.VerifyData())
                    .WithSqlParam("@JoiningDate", (model.DateAdded != null ? Convert.ToDateTime(model.DateAdded).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@DateAddedFrom", (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@DateAddedTo", (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@SalaryStatus", model.PaymentStatus)
                    .WithSqlParam("@UserKey", DbConstants.User.UserKey)
                    .WithSqlParam("@RoleKey", DbConstants.User.RoleKey)
                    .WithSqlParam("@PageIndex", model.page)
                    .WithSqlParam("@PageSize", model.rows)
                    .WithSqlParam("@SortBy", model.sidx)
                    .WithSqlParam("@TotalRecords", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalRecordsParam = dbParam;
                    }).WithSqlParam("@TotalSalarySum", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalSalarySum = dbParam;
                    }).WithSqlParam("@BalanceSalarySum", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        BalanceSalarySum = dbParam;
                    }).WithSqlParam("@TotalPaidSalarySum", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalPaidSalarySum = dbParam;
                    })
                    .ExecuteStoredProc((handler) =>
                    {
                        cashflowList = handler.ReadToDynamicList<dynamic>() as List<dynamic>;
                    });
                TotalRecords = Convert.ToInt64((TotalRecordsParam.Value ?? 0));
                model.TotalSalary = Convert.ToInt64((TotalSalarySum.Value ?? 0));
                model.BalanceAmount = Convert.ToInt64((BalanceSalarySum.Value ?? 0));
                model.TotalPaid = Convert.ToInt64((TotalPaidSalarySum.Value ?? 0));
                return cashflowList;
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.SalarySummary, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<dynamic>();
            }
        }
        #endregion Employee Salary Summary

        #region AccountHead Income And Expense Summary

        public List<dynamic> GetAHIncomeExpenseSummary(ReportViewModel model, out long TotalRecords)
        {
            try
            {
                List<dynamic> accountHeadIncomeExpenseList = new List<dynamic>();
                DbParameter TotalRecordsParam = null;
                DbParameter TotalIncomeSum = null;
                DbParameter TotalExpenseSum = null;
                DbParameter TotalBalanceSum = null;
                DbParameter IncomeSum = null;
                DbParameter ExpenseSum = null;
                DbParameter BalanceSum = null;

                if (model.sidx != "")
                {
                    model.sidx = model.sidx + " " + model.sord;
                }
                if (model.AccountHeadKeys.Count == 0)
                {
                    FillAccountHeadByBank(model, true);
                }
                dbContext.LoadStoredProc("dbo.SP_AH_IncomeExpense")
                    .WithSqlParam("@BranchKeyList", String.Join(",", model.BranchKeys))
                    .WithSqlParam("@AccountHeadKeyList", String.Join(",", model.AccountHeadKeys))
                    .WithSqlParam("@DateAddedFrom", (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@DateAddedTo", (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""))
                    .WithSqlParam("@UserKey", DbConstants.User.UserKey)
                    .WithSqlParam("@RoleKey", DbConstants.User.RoleKey)
                    .WithSqlParam("@PageIndex", model.page)
                    .WithSqlParam("@PageSize", model.rows)
                    .WithSqlParam("@SortBy", model.sidx)
                    .WithSqlParam("@TotalRecords", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalRecordsParam = dbParam;
                    }).WithSqlParam("@TotalIncomeSum", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalIncomeSum = dbParam;
                    }).WithSqlParam("@TotalExpenseSum", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalExpenseSum = dbParam;

                    }).WithSqlParam("@TotalBalanceSum", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalBalanceSum = dbParam;
                    }).WithSqlParam("@IncomeSum", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        IncomeSum = dbParam;
                    }).WithSqlParam("@ExpenseSum", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        ExpenseSum = dbParam;
                    }).WithSqlParam("@BalanceSum", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        BalanceSum = dbParam;
                    })

                    .ExecuteStoredProc((handler) =>
                    {
                        accountHeadIncomeExpenseList = handler.ReadToDynamicList<dynamic>() as List<dynamic>;
                    });
                TotalRecords = Convert.ToInt64(TotalRecordsParam.Value ?? 0);
                model.TotalIncome = Convert.ToInt64(TotalIncomeSum.Value ?? 0);
                model.TotalExpense = Convert.ToInt64(TotalExpenseSum.Value ?? 0);
                model.TotalBalance = Convert.ToInt64(TotalBalanceSum.Value ?? 0);
                model.AHIncomes = Convert.ToInt64(IncomeSum.Value ?? 0);
                model.Expense = Convert.ToInt64(ExpenseSum.Value ?? 0);
                model.Balance = Convert.ToInt64(BalanceSum.Value ?? 0);
                return accountHeadIncomeExpenseList;
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.CashFlowSummary, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<dynamic>();
            }
        }

        public void FillAccountHeadByBank(ReportViewModel model, bool IsAccountHeadKeys)
        {
            List<short> Branches = new List<short>();
            Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
            if (Employee != null)
            {
                if (Employee.BranchAccess != null)
                {
                    Branches = Employee.BranchAccess.Split(',').Select(Int16.Parse).ToList();

                }
            }
            else
            {
                Branches = dbContext.Branches.Select(x => x.RowKey).ToList();
            }

            List<long> AccountHeadKeys = new List<long>();
            string accoundheadkeys = string.Join(",", dbContext.BranchAccounts.Where(y => Branches.Contains(y.BranchKey)).Select(x => x.BankAccount.AccountHeadKey).Distinct().ToList());
            if (accoundheadkeys != "")
            {
                accoundheadkeys = accoundheadkeys + "," + DbConstants.AccountHead.CashAccount;
            }


            if (accoundheadkeys != "")
            {
                AccountHeadKeys = accoundheadkeys.Split(',').Select(Int64.Parse).ToList();

                if (IsAccountHeadKeys)
                {
                    model.AccountHeadKeys = AccountHeadKeys;
                }
            }

            model.AccountHeads = (from ac in dbContext.AccountHeads
                                  where AccountHeadKeys.Contains(ac.RowKey)
                                  select new SelectListModel
                                  {
                                      RowKey = ac.RowKey,
                                      Text = ac.AccountHeadName
                                  }).ToList();
        }


        #endregion

        #region Employee Attendance Summary
        public string GetEmpoyeeAttendanceSummaryReport(ReportViewModel model)
        {

            IEnumerable<string> results = dbContext.Database.SqlQuery<string>("Sp_EmployeeAttendaceSummery @AttendanceFromDate,@AttendanceToDate,@BranchKey,@EmployeeKey,@SearchText,@DesignationKeyList,@DepartmentKeyList,@EmployeeKeyList",
                                                                                     new SqlParameter("AttendanceFromDate", (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : "")),
                                                                                     new SqlParameter("AttendanceToDate", (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : "")),
                                                                                     new SqlParameter("BranchKey", model.BranchKey),
                                                                                     new SqlParameter("EmployeeKey", model.EmployeeKey),
                                                                                     new SqlParameter("SearchText", model.SearchAnyText.VerifyData()),
                                                                                     new SqlParameter("DesignationKeyList", String.Join(",", model.StudentStatusKeys)),
                                                                                     new SqlParameter("DepartmentKeyList", String.Join(",", model.StudentStatusKeys)),
                                                                                     new SqlParameter("EmployeeKeyList", String.Join(",", model.StudentStatusKeys))).ToList();


            return String.Join("", results);
        }
        #endregion
    }

}


