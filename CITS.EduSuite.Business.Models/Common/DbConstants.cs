using CITS.EduSuite.Business.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class DbConstants
    {
        public static string EncryptionKey = "cits$key";
        public const int AdminKey = 1;

        public static class Role
        {
            public static readonly List<short> AdminUserTypes = new List<short>() { 1, 2 };

            public static short SuperAdmin = 1;
            public static short Admin = 2;
            public static short Staff = 3;
            public static short Students = 4;
            public static short Parents = 5;
        }
        public static class User
        {
            public static long UserKey { get { return (Thread.CurrentPrincipal.Identity.IsAuthenticated ? (Thread.CurrentPrincipal as CITSEduSuitePrincipal).UserKey : 0); } }
            public static short RoleKey { get { return (short)(Thread.CurrentPrincipal.Identity.IsAuthenticated ? (Thread.CurrentPrincipal as CITSEduSuitePrincipal).RoleKey : 0); } }
            public static string Name { get { return (Thread.CurrentPrincipal.Identity.IsAuthenticated ? (Thread.CurrentPrincipal as CITSEduSuitePrincipal).Name : ""); } }
            public static short? BranchKey { get { return (short?)(Thread.CurrentPrincipal.Identity.IsAuthenticated ? (Thread.CurrentPrincipal as CITSEduSuitePrincipal).BranchKey : 0); } }
            public static short? CompanyKey { get { return (short?)(Thread.CurrentPrincipal.Identity.IsAuthenticated ? (Thread.CurrentPrincipal as CITSEduSuitePrincipal).CompanyKey : 0); } }
            public static long? EmployeeKey { get { return (Thread.CurrentPrincipal.Identity.IsAuthenticated ? (Thread.CurrentPrincipal as CITSEduSuitePrincipal).EmployeeKey : 0); } }
            public static bool IsTeacher { get { return (Thread.CurrentPrincipal.Identity.IsAuthenticated ? (Thread.CurrentPrincipal as CITSEduSuitePrincipal).IsTeacher : false); } }
            public static string Photo { get { return (Thread.CurrentPrincipal.Identity.IsAuthenticated ? (Thread.CurrentPrincipal as CITSEduSuitePrincipal).Photo : ""); } }
            public static long? ApplicationKey { get { return (Thread.CurrentPrincipal.Identity.IsAuthenticated ? (Thread.CurrentPrincipal as CITSEduSuitePrincipal).ApplicationKey : 0); } }
            public static string CompanyLogo { get { return (Thread.CurrentPrincipal.Identity.IsAuthenticated ? (Thread.CurrentPrincipal as CITSEduSuitePrincipal).CompanyLogo : ""); } }

        }
        public static class Module
        {
            public const short HR = 1;
            public const short Account = 2;
        }
        public static class CashFlowType
        {
            public const byte In = 1;
            public const byte Out = 2;

        }
        public static class Status
        {
            public const short Active = 1;
            public const short Deactive = 2;
        }
        public static class BookType
        {
            public const string Compulsory = "C";
            public const string Elective = "E";
        }
        public static class PartyType
        {
            public const byte Employee = 1;
            public const byte Student = 2;
            public const byte BankAccount = 3;
            public const byte Branch = 4;
            public const byte Other = 5;
        }
        public static class TransactionType
        {
            public const byte Salary = 1;
            public const byte Fee = 2;
            public const byte UniversityFee = 3;
            public const byte CashFlow = 4;
            public const byte BankAccount = 5;
            public const byte Loan = 6;
            public const byte Transaction = 7;
            public const byte Application = 8;
            public const byte FeeMaster = 9;
            public const byte FeeDetails = 10;
            public const byte FeeInstallment = 11;
            public const byte Promotion = 12;
            public const byte Branch = 13;
            public const byte Journal = 14;
            public const byte SalaryAdvance = 15;
            public const byte SalaryPayable = 16;
            public const byte SalaryAdvanceRecieved = 17;
            public const byte OtherBankTransaction = 18;
            public const byte FutureTransaction = 19;
            public const byte FutureTransactionPayment = 20;
            public const byte FutureTransactionOther = 21;
            public const byte FutureTransactionCGST = 22;
            public const byte FutureTransactionSGST = 23;
            public const byte FutureTransactionInOpeningBalance = 24;
            public const byte FutureTransactionOutOpeningBalance = 25;
            public const byte ChequeClearance = 26;
            public const byte ChequeClearancePayable = 27;
            public const byte ChequeClearanceRecievable = 28;
            public const byte CompanyBranchTransaction = 29;

            public const byte CashFlowRecievable = 30;
            public const byte CashFlowPayable = 31;
            public const byte CashFlowAdvance = 32;
            public const byte CashFlowExcess = 33;
            public const byte DiscountCashFlow = 34;
            public const byte ApplicationConsession = 35;

            public const byte Depreciation = 36;
            public const byte PurchaseDepreciation = 37;
            public const byte Asset = 38;
            public const byte RoundOffPurchase = 39;
            public const byte DiscountPurchase = 40;
            public const byte Purchase = 41;
            public const byte AssetPurchasePayable = 42;
            public const byte AssetPurchaseAdvance = 43;
            public const byte AssetPurchase = 44;
            public const byte AssetPurchaseInventory = 45;
            public const byte AssetPurchaseMaster = 46;
            public const byte AssetPurchaseExcessAmount = 47;
            public const byte AssetPurchaseCGST = 48;
            public const byte AssetPurchaseSGST = 49;
            public const byte DiscountAssetPurchase = 50;
            public const byte DiscountAssetPurchasePayable = 51;
            public const byte RoundOffAssetPurchase = 52;
            public const byte RoundOffAssetPurchasePayable = 53;
            public const byte SalaryAdvanceReturn = 54;
            public const byte CashTransaction = 55;
            public const byte FeeRefund = 56;
            public const byte UniversityPaymentCancelation = 57;
        }

        public static class ApplicationType
        {
            public const byte Staff = 1;
            public const byte Student = 2;
            public const byte Other = 3;
        }


        public static class PaymentMode
        {
            public static readonly List<short> CashPaymentModes = new List<short>() { 1 };
            public static readonly List<short> BankPaymentModes = new List<short>() { 3 };
            public const short Cash = 1;
            public const short Cheque = 2;
            public const short Bank = 3;
        }
        public static class PaymentModeSub
        {
            public const short DemandDraft = 1;
            public const short Card = 2;
            public const short OnlinePayment = 3;
            public const short DirectDeposit = 4;
        }
        public static class ProcessStatus
        {
            public const short Pending = 1;
            public const short Approved = 2;
            public const short Rejected = 3;
        }
        public static class SalaryHeadType
        {
            public const short MonthlyPayments = 1;
            public const short StatutoryDeductions = 2;
            public const short StatutoryPayments = 3;
            public const short AnnualPayments = 4;
            public const short LeaveCompensation = 5;
            public const short CostToCompany = 6;
            public const short DirectCostToCompany = 7;
            public const short OtherCosts = 8;
        }
        public static class BankTransactionType
        {
            public const short Deposit = 1;
            public const short Withdrawal = 2;
            public const short AccountTransfer = 3;

        }
        public static class LeaveDurationType
        {
            public const byte MultipleDays = 1;
            public const byte FullDay = 2;
            public const byte HalfDay = 3;

        }
        public static class EmployeeStatus
        {
            public const short Working = 1;
            public const short Fired = 2;
            public const short Resigned = 3;
        }

        public static class IdentityType
        {
            public const int AdharNumber = 1;
        }

        public static class AttendanceStatus
        {
            public const byte Present = 1;
            public const byte Absent = 2;
            public const byte Leave = 3;
            public const byte Off = 4;
            public const byte WeekOff = 5;
            public const byte Holyday = 6;
            public const byte Late = 7;
            public const byte Absconders = 8;
            public const byte EarlyDeparture = 9;
            public const byte HalfDay = 10;
        }
        public static class AttendanceConfigType
        {
            public const byte InOut = 1;
            public const byte InOutWithBreak = 2;
            public const byte MarkPresent = 3;

        }
        public static class UnitType
        {
            public const byte Amount = 1;
            public const byte Percentage = 2;
        }

        #region EduSuite
        public static class EnquiryCallStatus
        {
            public const int Counselling = 6;
            public const int CounsellingCompleted = 9;
        }
        public static class EnquiryStatus
        {
            public const short FollowUp = 1;
            public const short AdmissionTaken = 2;
            public const short Intersted = 3;
            public const short Closed = 4;
        }

        public static class NatureOfEnquiry
        {
            public const short Lead = 1;
            public const short WebSite = 2;
            public const short Scholership = 3;
            public const short NewsPapper = 4;

            public const int Facebook = 5;
            public const int Instagram = 6;


        }
        public static class FacebookPlatForm
        {
            public const string FB = "fb";
            public const string IG = "ig";

        }
        public static class NatureOfEnquiryAPI
        {
            public const int Facebook = 5;
            public const int Instagram = 6;

            public static readonly List<int> NatureOfEnquiryAPIList = new List<int>() { 5, 6 };

        }

        public static class StudentStatus
        {
            public const short Ongoing = 1;
            public const short Completed = 2;
            public const short Droped = 3;
            public const short Refunded = 4;
        }

        public static class ServiceType
        {
            public const byte Study = 1;
            public const byte Migration = 2;
            public const byte HR = 3;

        }
        public static class FileHandoverType
        {
            public const byte EnquiryLead = 1;
            public const byte Enquiry = 2;
        }

        public static class AutoNumberConfig
        {
            public const byte AdmissionNo = 1;
            public const byte ReceiptNo = 2;
        }
        #endregion



        public static class AccountGroup
        {
            public const byte Asset = 1;
            public const byte Liability = 2;
            public const byte Income = 3;
            public const byte Expenses = 4;
        }

        public static class AccountHeadType
        {
            public const short FixedAssets = 1;
            public const short CurrentAssets = 2;
            public const short FixedLiabilities = 3;
            public const short CurrentLiabilities = 4;
            public const short IndirectIncome = 5;
            public const short DirectIncome = 6;
            public const short IndirectExpense = 7;
            public const short DirectExpense = 8;
            public const short Debitors = 9;
            public const short Creditors = 10;
            public const short SundryDebitors = 11;
            public const short SundryCreditors = 12;
            public const short Capital = 13;
            public const short Drawings = 14;

        }

        public static class AccountHead
        {
            public const long CashSale = 1;
            public const long CashAccount = 2;
            public const long AccountsReceivable = 3;
            public const long InputTaxCGST = 4;
            public const long InputTaxSGST = 5;
            public const long AccountsPayable = 6;
            public const long OutputTaxSGST = 7;
            public const long OutputTaxCGST = 8;
            public const long Depreciation = 9;
            public const long CostOfService = 10;
            public const long Inventory = 11;
            public const long AccumulatedDepreciation = 12;
            public const long AdvanceReceivable = 13;
            public const long AdvancePayable = 14;
            public const long DirectMaterials = 15;
            public const long OpeningBalance = 16;
            public const long DiscountAllowed = 17;
            public const long DiscountTaken = 18;
            public const long InputTaxIGST = 19;
            public const long OutputTaxIGST = 20;
            public const long Salary = 21;
            public const long SalaryPayable = 22;
            public const long SalaryAdvanceRecievable = 23;
            public const long OutputTaxCess = 24;
            public const long FeeRefund = 25;
            public const long UniversityPaymentCancelation = 26;

        }

        public static class ScheduleStatus
        {
            public const int Today = 0;
            public const int Pending = 1;
            public const int Upcoming = 2;
            public const int History = 3;
            public const int Tomorrow = 4;
            public const int TodayReshceduled = 5;
            public const int CounsellingSchedule = 6;
            public const int NewLead = 7;
            public const int Unallocated = 8;
            public const int Shortlisted = 9;
            public const int ShortlistPending = 10;
            public const int Closed = 11;
            public const int Total = 12;
        }

        public static class MailMessage
        {
            public const int InBox = 1;
            public const int Sent = 2;
            public const int Draft = 3;
            public const int Trash = 4;
            public const int Starred = 5;
        }
        public static class ApplicationStatus
        {
            public const short OnGoing = 1;
            public const short Completed = 2;
            public const short Droped = 3;
            public const short Refunded = 4;

        }
        public static class Consession
        {
            public const short Yes = 1;
        }

        public static class StudentClassRequird
        {
            public const bool Yes = true;
            public const bool No = false;

        }
        public static class Mode
        {
            public const short REGULAR = 1;
            public const short LATERAL_ENTRY = 2;
            public const short RE_ADMISSION = 3;
        }

        public static class PromotionStatus
        {
            public const short Promoted = 1;
            public const short Completed = 2;
            public const short Discontinued = 3;

        }

        public static class AcademicTerm
        {
            public const short Semester = 1;
            public const short Yearly = 2;
            public const short ShortTerm = 3;

        }

        public static class ResultStatus
        {
            public const string Absent = "A";
            public const string Passed = "P";
            public const string Fail = "F";
        }
        public static class FeeTypeMode
        {
            public const short Single = 1;
            public const short Multiple = 2;
        }

        public static class LogType
        {
            public const byte Info = 1;
            public const byte Error = 2;
            public const byte Warn = 3;
            public const byte Debug = 4;
            public const byte Fatal = 5;
        }
        public static class VoucherType
        {
            public const byte OpeningBalance = 1;
            public const byte Fee = 2;
            public const byte UniversityFee = 3;
            public const byte Payment = 4;
            public const byte Reciept = 5;
            public const byte Salary = 6;
            public const byte Admission = 7;
            public const byte FeeInstallment = 8;
            public const byte Promotion = 9;
            public const byte SalaryAdvance = 10;
            public const byte OtherBankTransaction = 11;
            public const byte FutureTransaction = 12;
            public const byte InputTax = 13;
            public const byte OutputTax = 14;
            public const byte Journal = 15;
            public const byte Cess = 16;
            public const byte Depreciation = 17;
            public const byte Asset = 18;
            public const byte Purchase = 19;
            public const byte Stock = 20;
            public const byte AdvanceReturn = 21;
            public const byte CashTransaction = 22;
            public const byte FeeRefund = 23;
            public const byte UniversityPaymentCancelation = 24;
        }
        public static class Menu
        {
            public const short EnquiryLead = 5;
            public const short Enquiry = 4;
            public const short Application = 1;
        }

        public static class Country
        {
            public const short India = 51;
        }
        public static class CallType
        {
            public const byte Incoming = 1;
            public const byte Outgoing = 2;
            public const byte Walking = 3;

        }
        public static class NotificationType
        {
            public const int SMS = 1;
            public const int Email = 2;
            public const int Push = 3;
        }
        public static class PushNotificationTemplate
        {

            public const int Application = 1;
            public const int Fee = 2;
            public const int ExtendInstallmentDate = 3;
            public const int StudentAttendance = 4;
            public const int StudentLate = 5;
            public const int StudentEarlyDeparture = 6;
            public const int StudentAbsconders = 7;
            public const int StudentLeave = 8;
            public const int LeadCountlimit = 9;
            public const int LeadEmployeeLocked = 10;
            public const int EnquiryAdmission = 11;
            public const int IncentiveProductivity = 12;
            public const int MobileNumberSearch = 13;
            public const int StudentTC = 15;
            public const int CourseCompletionCertificate = 16;
            public const int BonafiedCertificate = 17;

        }
        public static class NotificationTemplate
        {
            public const int Application = 1;
            public const int Fee = 2;
            public const int UniversityFee = 3;
            public const int StudentAttendance = 4;
            public const int StudentLate = 5;
            public const int StudentEarlyDeparture = 6;
            public const int StudentAbsconders = 7;
            public const int StudentLeave = 8;
            public const int StudentDiary = 9;
            public const int ExamResult = 10;
            public const int InternalExamSchedule = 11;

            public const int StudentIDCard = 12;
            public const int ExamSchedule = 13;
            public const int InternalExamResult = 14;
            public const int StudentTC = 15;
            public const int CourseCompletionCertificate = 16;
            public const int BonafiedCertificate = 17;
            public const int CourseTransfer = 18;
        }
        // For Bank And Future transaction on 07 Mar 2019
        public static class OrderProcessType
        {
            public const byte Default = 0;
            public const byte Percentage = 1;
            public const byte NoTax = 2;
        }

        public static class CertificateProcessType
        {
            public const byte Received = 1;
            public const byte Verified = 2;
            public const byte Returned = 3;
        }
        public static class ExamStatus
        {
            public const byte Reguler = 1;
            public const byte Supply = 2;
            public const byte Improvement = 3;

        }
        public static class PaymentStatus
        {
            public const byte Complete = 1;
            public const byte Pending = 2;

        }
        public static class TaxRateType
        {
            public const byte Exclusive = 1;
            public const byte Inclusive = 2;
            public const byte NonTax = 3;
        }
        public static class PaymentReceiptConfigType
        {
            public const byte Receipt = 1;
            public const byte Payment = 2;
            public const byte Refund = 3;
            public const byte ReceiptVoucher = 8;
            public const byte PaymentAndReceipt = 9;
            public const byte SalaryVoucher = 10;

        }
        public static class InvoicePrintType
        {
            public static int Receipt = 1;
            public static int OrderPrint = 2;
            public static int GSTPrint = 3;
            public static int InvoicePrint = 4;
            public static int QuotationDealerPrint = 5;
            public static int OrderDealerPrint = 6;
            public static int MultipleInvoicePrint = 7;
            public static int RawMaterialSalePrint = 8;
            public static int RawMaterialSaleGSTPrint = 9;
            public static int QuotationPrint = 10;
        }

        public static class HolidayType
        {
            public const byte Fixed = 1;
            public const byte Dynamic = 2;
        }

        #region Enquiry Report
        public static class ProductiveCalls
        {
            public const int Limit = 2;
        }
        public static class ScheduleTypes
        {
            public const int NewLead = 0;
            public const int EnquiryLead = 1;
            public const int Enquiry = 2;
            public const int Application = 3;
            public const int Document = 4;
            public const int Document1 = 7;
            public const int Visa = 8;
            public const int Visa2 = 10;
            public const int Traveling = 11;

        }
        public static class DateTypes
        {
            public const int CalledDate = 1;
            public const int NextScheduleDate = 2;
            public const int CreatedDate = 3;
            public const int EnquiryCounsellingDate = 4;
            public const int EnquiryCounsellingCalledDate = 5;
            public const int AddedToEnquiryDate = 6;
            public const int EnquiryFetchDate = 7;
        }
        public static class EmployeeFilterTypes
        {
            public const int CounsellingBy = 1;
            public const int ScheduledBy = 2;
            public const int CalledBy = 3;

        }
        #endregion Enquiry Report

        public static class WeekDays
        {
            public const byte Sunday = 1;
            public const byte Monday = 2;
            public const byte Tuesday = 3;
            public const byte Wednesday = 4;
            public const byte Thursday = 5;
            public const byte Friday = 6;
            public const byte Saturday = 7;
           
        }
        public static class WeeklyPeriods
        {
            public const byte Period1 = 1;
            public const byte Period2 = 2;
            public const byte Period3 = 3;
            public const byte Period4 = 4;
            public const byte Period5 = 5;
            public const byte Period6 = 6;
            public const byte Period7 = 7;
        }


        public static class QuestionStatus
        {
            public const int NotVisited = 1;
            public const int NotAnswered = 2;
            public const int Answered = 3;
            public const int Markedforreview = 4;
            public const int AnsweredAndMarkedForReview = 5;
        }
        public static class OnlineExamStatus
        {
            public const int Started = 1;
            public const int Finished = 2;
            public const int NotCompleted = 3;
        }

        public static class QuestionType
        {
            public const byte Writing = 1;
            public const byte Completion = 2;
            public const byte Optional = 3;
            public const byte Matching = 4;
            public const byte Groups = 5;
        }
        public static class QuestionModule
        {
            public static List<byte> AswerKeyModules = new List<byte> { 1, 2 };
            public const byte Listening = 1;
            public const byte Reading = 2;
            public const byte Writing = 3;
            public const byte Speaking = 4;

        }

        public static class ExamTypes
        {
            public const short NormalExam = 1;
            public const short OtherExam = 2;


        }

        public static class DepreciationMethod
        {
            public const byte StrightLine = 1;
            public const byte Doubledecliningbalance = 2;
            public const byte Unitsofproduction = 3;
            public const byte Sumofyearsdigits = 4;
        }

        public static class PeriodType
        {
            public const byte Day = 1;
            public const byte Week = 2;
            public const byte Month = 3;
            public const byte Year = 4;
        }

        public static class OverTimeFormulaType
        {
            public const byte OverTimeNotApplicable = 1;
            public const byte OutPunchMinusShiftEndTime = 2;
            public const byte TotalDurationMinusShiftHours = 3;
            public const byte EarlyComingPlusLateGoing = 4;
        }

        public static class LeaveType
        {
            public const short CasualLeave = 1;
            public const short SickLeave = 2;
            public const short PaidLeave = 3;
        }

        public static class WorkType
        {
            public const byte Office = 1;
            public const byte Site = 2;
        }


        public static class ShiftAllocationType
        {
            public static byte PreviousDayShift = 1;
            public static byte AutoShift = 2;
        }
        public static class SalaryType
        {
            public const byte Monthly = 1;
            public const byte Daily = 2;
            public const byte Hourly = 3;
        }

        public static class EmployeeAttendanceStatus
        {
            public const short Present = 1;
            public const short Absent = 2;           
            public const short Leave = 3;
            public const short Off = 4;
            public const short WeeklyOff = 5;
            public const short Holyday = 6;
            public const short Halfday = 7;
        }


        public static class AbsentDayTypes
        {
            public const byte Halfday = 1;
            public const byte FullDay = 2;
        }
        public static class AttendancePresentStatus
        {
            public const byte CheckIn = 1;
            public const byte CheckOut = 2;
            public const byte BreakOut = 3;
            public const byte BreakIn = 4;
        }

        public static class LeaveCountType
        {
            public const byte Monthly = 1;
            public const byte Yearly = 2;
        }

        public static class MenuType
        {
            public const short Account = 3;
            public const short Reports = 4;
            public const short Masters = 5;
            public const short Stock = 6;
            public const short PayRoll = 7;
            public const short Attendance = 8;
        }
        public static class RateTypes
        {
            public const byte SqFeet = 1;
            public const byte Numbers = 2;
            public const byte SqWidth = 3;
            public const byte SqHeight = 4;
        }

        public static class CondentType
        {
            public const short Optional = 1;
            public const short Date = 2;
            public const short Text = 3;
        }

        public static class EnquiryModule
        {
            public const int Enquiry = 3;
            public const int CounsellingCompleted = 2;
            public const int CounsellingSchedule = 1;
        }

        public static class DashBoardType
        {
            public const string Enquiry = "EQ-tab";
            public const string Students = "ST-tab";
            public const string Accounts = "AC-tab";
            public const string Library = "LB-tab";
        }

        public static class DashBoardContent
        {
            public const short EnquiryCounts = 1;
            public const short LeadStageFlows = 2;
            public const short RecentCallDetails = 3;
            public const short LeadSource = 4;
            public const short EnquirySurvey = 5;
            public const short StudentsCounts = 6;
            public const short StudentsSurvey = 7;
            public const short StudentsByCoursetype = 8;
            public const short StudentsByCourse = 9;
            public const short AbsentList = 10;
            public const short StudentDiary = 11;
            public const short RecentlyAdmitted = 12;
            public const short AccountsCount = 13;
            public const short CashFlowGraph = 14;
            public const short IncomeandExpenseChart = 15;
            public const short ChequeDetails = 16;
            public const short RecentTransaction = 17;
            public const short EmployeeCounts = 18;
            public const short SalaryDetails = 19;
        }

        public static class VideoType
        {
            public const byte YouTubeLink = 1;
            public const byte UploadFile = 2;
        }

        public static class FileExtension
        {
            public const string PDF = ".PDF";
            public const string Video = "ST-tab";
            public const string Image = "AC-tab";
        }

        public static class DocumentType
        {
            public const short Video = 1;
            public const short PDF = 2;
        }
        public static class EducationType
        {
            public const byte DistanceEducation = 1;
            public const byte RegulerEducation = 2;
            public const byte Both = 3;
        }
        public static class CourseDurationType
        {
            public const short Months = 1;
            public const short Days = 2;
            public const short Years = 3;
        }

        public static class FieldValidationType
        {
            public const short Applicationconfig = 1;
            public const short EmployeeConfig = 2;
            public const short LibraryConfig = 3;
        }
        public static class ApplicationScheduleType
        {
            public const short Application = 1;
            public const short Fee = 2;
            public const short StudyMaterial = 3;
            public const short AffiliationPayment = 4;
            public const short ReturnCertificate = 5;
            public const short AffliationCertificate = 6;
            public const short ExamSchedule = 7;
            public const short ExamResult = 8;
        }
        public static class AttendanceType
        {
            public const short OneTime = 1;
        }
        public static class UserManualType
        {
            public const short DashBoard = 1;
            public const short Menu = 2;
            public const short Other = 3;
        }
        public static class GeneralConfiguration
        {
            public static bool AllowAdmissionToAccoount { get { return (Thread.CurrentPrincipal.Identity.IsAuthenticated ? (Thread.CurrentPrincipal as CITSEduSuitePrincipal).AllowAdmissionToAccoount : false); } }
            public static bool AllowSplitCostOfService { get { return (Thread.CurrentPrincipal.Identity.IsAuthenticated ? (Thread.CurrentPrincipal as CITSEduSuitePrincipal).AllowSplitCostOfService : false); } }
            public static bool AllowCenterShare { get { return (Thread.CurrentPrincipal.Identity.IsAuthenticated ? (Thread.CurrentPrincipal as CITSEduSuitePrincipal).AllowCenterShare : false); } }
            public static bool AllowUniversityAccountHead { get { return (Thread.CurrentPrincipal.Identity.IsAuthenticated ? (Thread.CurrentPrincipal as CITSEduSuitePrincipal).AllowUniversityAccountHead : false); } }
            public static byte? EducationTypeKey { get { return (Thread.CurrentPrincipal.Identity.IsAuthenticated ? (Thread.CurrentPrincipal as CITSEduSuitePrincipal).EducationTypeKey : 0); } }

        }

    }
}
