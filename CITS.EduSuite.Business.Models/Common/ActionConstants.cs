using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class MenuConstants
    {
        public const string Book = "BK";
        public const string BookStatus = "BKSTS";
        public const string BookCategory = "BKCTGRY";
        public const string BookCopy = "BKCPY";
        public const string BookIssueReturn = "BKISRTN";
        public const string BookIssueSummaryReport = "BKISSR";
        public const string BookIssueType = "BKISTYP";
        public const string BorrowerType = "BRWRTYP";
        public const string Language = "LG";
        public const string LibraryBook = "LBBK";
        public const string Login = "LOGIN";
        public const string Medium = "MD";
        public const string MemberPlanDetails = "MBRPLN";
        public const string MemberRegistration = "MBRRG";
        public const string MemberType = "MBRTYP";
        public const string Publisher = "PB";
        public const string Role = "Rl";
        public const string Rack = "RK";
        public const string Status = "STS";
        public const string TransactionType = "TT";
        public const string Holiday = "HLDY";
        public const string Author = "ATR";
        public const string LibraryBookSummary = "LBS";
        public const string LibraryBookIssueSummary = "LBIS";
        public const string LibraryMemberDetailsSummary = "LBMS";
        public const string ChangePassword = "CPSWD";
        public const string MailBox = "MailBox";
        public const string BulkEmailSMS = "BESMS";

        #region Account
        public const string BankAccount = "BAC";
        public const string BankTransaction = "BTR";
        public const string AccountFlow = "AF";
        public const string AccountTrancasaction = "AT";
        public const string BankStatement = "BSMT";
        public const string BankReconciliation = "BRC";
        public const string FutureTransaction = "FUT";
        public const string ChequeClearance = "CCL";
        public const string CashFlow = "CASHF";
        public const string AccountHead = "ACCH";
        public const string AccountHeadOpeningBalance = "AHOB";
        public const string CashTransaction = "CTR";


        #endregion Account

        #region Master

        public const string AttendanceType = "ATTYP";
        public const string Batch = "BT";
        public const string Branch = "BR";
        public const string Building = "BL";
        public const string ClassMode = "CLSMD";
        public const string Course = "C";
        public const string CourseType = "CT";
        public const string Department = "DPT";
        public const string Designation = "D";
        public const string Eligibility = "ELG";
        public const string OtherFeeType = "OFT";
        public const string GradeWiseSalaryStructure = "DSG";
        public const string Country = "CN";
        public const string Religion = "RLG";
        public const string NatureOfEnquiry = "NOE";
        public const string Agent = "AGT";
        public const string UniversityCourse = "UC";
        public const string UniversityMaster = "UM";
        public const string Division = "DV";
        public const string District = "DTC";
        public const string FeeType = "FT";
        public const string InternalExamTerm = "IET";
        public const string ExamCentre = "EXMC";
        public const string ExamTerm = "EXMT";
        public const string CertificateType = "CRTT";
        public const string SyllabusAndStudyMaterial = "SYSM";
        public const string Installment = "VFC";
        public const string CourseSubject = "CS";
        public const string StudentStatus = "STSTS";
        public const string StudyMaterial = "SM";
        public const string ScholarshipType = "ST";
        public const string SecondLanguage = "SDLG";
        public const string Menu = "MN";
        public const string CounsellingTime = "CLT";
        public const string MenuType = "MT";
        public const string AttendanceTypeMaster = "ATYPM";
        public const string GSTMaster = "GSTM";
        public const string Caste = "CST";
        public const string Bank = "BNK";
        public const string AcademicTerm = "ACDT";
        public const string DashBoardType = "DBT";
        public const string DashBoardContent = "DBC";
        public const string SalaryOtherAmountType = "SOAT";
        public const string NotificationTemplate = "NT";
        public const string NotificationData = "ND";
        public const string StudentTimeTable = "STT";
        public const string TimeTableMaster = "TTM";
        public const string FieldValidation = "FV";
        public const string TCReasonMaster = "TCRSM";
        public const string Company = "CMP";
        public const string UserManual = "USM";
        public const string ESSLStudnts = "ESSLS";


        #endregion Master

        #region Application
        public const string Application = "APP";
        public const string ApplicationPersenol = "APPER";
        public const string ApplicationEducational = "APPED";
        public const string ApplicationDocument = "APPDC";
        public const string ApplicationElectivePaper = "APPELCTPR";
        public const string ApplicationFamilyDetails = "APPFD";
        public const string ApplicationFeeInstallment = "APPFI";
        public const string ApplicationFeePayment = "APPFP";
        public const string FeeDetails = "FD";
        public const string StudentAttendance = "SA";
        public const string InternalExam = "IE";
        public const string InternalExamResult = "IER";
        public const string ExamSchedule = "EXMS";
        public const string GenerateRollNumber = "GR";
        public const string UniversityPayment = "UNP";
        public const string UnitTestResult = "UTR";
        public const string UniversityCertificate = "UNCRT";
        public const string StudentIDCard = "STID";
        public const string StudentsCertificateReturn = "STCRR";
        public const string StudentsPromotion = "STPR";
        public const string ExamResult = "ER";
        public const string StudentTC = "STC";
        public const string StudentLate = "SLT";
        public const string StudentEarlyDeparture = "SED";
        public const string StudentDiary = "SD";
        public const string StudentAbsconders = "SAC";
        public const string StudentLeave = "SLV";
        public const string ApplicationWebForm = "AWF";
        public const string StudentFeeRefund = "SFR";
        public const string DocumentTrack = "DT";
        public const string UniversityPaymentCancel = "UPC";
        public const string CourseCompletionCertificate = "CCC";
        public const string BonafiedCertificate = "BC";
        public const string CourseTransfer = "CSTR";
        public const string FeeSchdeule = "FSH";
        public const string ApplicationSchedule = "ASH";
        public const string ApplicationScheduleType = "ASTP";
        public const string ApplicationScheduleCallStatus = "ASCS";


        #endregion Application

        #region Reoprts
        public const string StudentsFeeSummary = "SFS";
        public const string StudyMaterialIssueSummary = "SMIS";
        public const string StudentIDCardSummary = "SIDS";
        public const string InternalExamResultSummary = "IERS";
        public const string UniversityPaymentSummary = "SUFS";
        public const string StudentsAttendanceSummary = "SAS";
        public const string StudentCertificateSummary = "SCS";
        public const string UniversityCertificateSummary = "UCS";
        public const string UnitTestExamResultSummary = "UTS";
        public const string ExamScheduleSummary = "ESS";
        public const string EnquiryCallSummary = "ECS";
        public const string ActivityLog = "ALS";
        public const string StudentsLateReport = "SLTR";
        public const string StudentsLeaveReport = "SLVR";
        public const string StudentsAbscondersReport = "SABR";
        public const string StudentsEarlyDepartureReport = "SEDR";
        public const string StudentsFeeRefundReport = "SFRR";
        public const string FeeCollectionReport = "FCR";
        public const string UniversityFeeCollectionReport = "UFCR";
        public const string EmployeeWorkScheduleSummary = "EMPWSS";
        public const string EnquiryLeadCountSummary = "ELS";
        public const string EnquiryCountSummary = "ECLS";
        public const string CashFlowSummary = "CSFSR";
        public const string EnquiryTargetSummary = "ETS";
        public const string FeeInstallmentSummary = "FIS";
        public const string FeePaidSummary = "FPS";
        public const string BalanceSheet = "BS";
        public const string DayBook = "DB";
        public const string Journal = "J";
        public const string IncomeStatment = "IS";
        public const string CashBook = "CB";
        public const string BankBook = "BB";
        public const string ProfitAndLossAccount = "PLA";
        public const string Ledger = "LEDG";
        public const string TrialBalance = "TBL";
        public const string CashFlowStatement = "CSFS";
        public const string SalarySummary = "SS";
        public const string IncomeExpenseSummary = "IES";


        #endregion Reoprts

        #region Enquiry
        public const string Enquiry = "EQ";
        public const string EnquirySchedule = "EQS";
        public const string EnquiryLead = "EQL";
        public const string EnquiryFeedback = "ENQFD";
        public const string EnquiryLeadFeedback = "ENQLFD";
        public const string EnquiryCallStatus = "ENQCS";
        public const string StudentsSummaryReport = "SSR";


        public const string CounsellingScheduleList = "CSHL";
        public const string CounsellingCompletedScheduleList = "CCSHL";
        public const string CounsellingApproval = "EQAPRVL";
        public const string CounsellingSummary = "CLNGSM";

        #endregion Enquiry

        #region Employee
        public const string Employee = "EMP";
        public const string Shift = "SFT";
        public const string EmployeeAccount = "EMPAC";
        public const string EmployeeAttendance = "EMPAT";
        public const string EmployeeClassAllocation = "EMPCA";
        public const string EmployeeContact = "EMPCT";
        public const string EmployeeEducation = "EMPED";
        public const string EmployeeExperience = "EMPEX";
        public const string EmployeeIdentity = "EMPID";
        public const string EmployeeLanguageSkill = "EMPLGS";
        public const string EmployeeLoan = "EMPLN";
        public const string EmployeePersonal = "EMPPER";
        public const string EmployeeSalary = "EMPSLR";
        public const string EmployeeSalarySettings = "EMPSLRS";
        public const string EmployeeSalaryAdvance = "EMPSA";
        public const string EmployeeSalaryAdvanceReturn = "EMPSAR";
        public const string EmployeeTask = "EMPTSK";
        public const string EmployeeUserAccount = "EMPUAC";
        public const string EmployeeUserPermission = "EMPUPR";

        public const string EmployeeWorkSchedule = "EMPWS";
        public const string TeacherModuleAllocation = "EMPMA";


        public const string AttendanceCategory = "ATCT";
        public const string AttendanceConfiguration = "ATCFG";
        public const string LeaveApplication = "LAPP";
        public const string QuickAttendance = "QATT";
        public const string AttendanceSheet = "ATST";
        public const string DepartmentShift = "DSFT";
        public const string EmployeeShift = "ESFT";

        public const string Depreciation = "DPC";
        public const string AssetType = "ASTYP";
        public const string AssetPurchase = "ASTPR";
        public const string Asset = "AST";
        public const string Party = "PTY";
        public const string LeadAllocation = "LDAT";
        public const string EmployeeHeirarchy = "EHR";
        public const string EmployeeEnquiryTarget = "EET";
        public const string EmployeeAttendanceSummary = "EAS";



        #endregion Employee

        #region Scholarship
        public const string Scholarship = "SCP";
        public const string ScholarshipExamSchedule = "SPES";
        public const string ScholarshipExamResult = "SPER";
        #endregion Scholarship
            
        #region Exam
        public const string TestPaper = "TSPR";
        public const string ExamTest = "EXTS";
        public const string TestPaperOption = "TPO";
        public const string TestValuation = "TVLT";
        public const string MarkGroup = "MRKG";
        #endregion

        #region studentPortal
        public const string Video = "VVDO";
        public const string StudentStudyMaterial = "STDMTR";
        #endregion

    }
    public class ActionConstants
    {
        public const string MenuAccess = "MA";
        public const string Add = "A";
        public const string Delete = "D";
        public const string Edit = "E";
        public const string AddEdit = "AE";
        public const string View = "V";
        public const string FeeStructure = "FS";
        public const string BulkAdd = "BA";
        public const string Login = "LI";
        public const string Logout = "LO";
        public const string Payment = "PM";
        public const string UserPermission = "UP";
        public const string EmployeeHeirarchy = "EH";
        public const string UserAccount = "UA";
        public const string ApplicationStatus = "AS";
        public const string ChangeClass = "CC";
        public const string LeadTransfer = "LT";

    }
}
