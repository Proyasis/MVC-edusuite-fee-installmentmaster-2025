using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Provider;
using CITS.EduSuite.Business.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;

namespace CITS.EduSuite.UI
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
            container.RegisterType<ICookieAuthentationProvider, CookieProvider>();
            container.RegisterType<ICourseTypeService, CourseTypeService>();
            container.RegisterType<IBranchService, BranchService>();
            container.RegisterType<IFeeTypeService, FeeTypeService>();
            container.RegisterType<IBookService, BookService>();
            container.RegisterType<IUniversityCourseService, UniversityCourseService>();
            container.RegisterType<IAttendanceService, AttendanceService>();
            container.RegisterType<IApplicationService, ApplicationService>();
            container.RegisterType<IApplicationPersonalService, ApplicationPersonalService>();
            container.RegisterType<IApplicationEducationalDetailsService, ApplicationEducationalDetailsService>();
            container.RegisterType<IApplicationFeePaymentService, ApplicationFeePaymentService>();
            container.RegisterType<IApplicationFeeInstallmentService, ApplicationFeeInstallmentService>();
            container.RegisterType<IApplicationDocumentService, ApplicationDocumentService>();
            container.RegisterType<IApplicationElectivePaperService, ApplicationElectivePaperService>();
            container.RegisterType<ILoginService, LoginService>();
            container.RegisterType<IEnquiryLeadService, EnquiryLeadService>();
            container.RegisterType<IEnquiryService, EnquiryService>();
            container.RegisterType<ICourseService, CourseService>();
            container.RegisterType<IUniversityMasterService, UniversityMasterService>();
            container.RegisterType<IStudyMaterialService, StudyMaterialService>();
            container.RegisterType<IEnquiryScheduleService, EnquiryScheduleService>();
            container.RegisterType<IUniversityPaymentService, UniversityPaymentService>();
            container.RegisterType<IDivisionAllocationService, DivisionAllocationService>();
            container.RegisterType<IStudentIDCardService, StudentIDCardService>();
            container.RegisterType<IStudentsPromotionService, StudentsPromotionService>();
            container.RegisterType<IStudentsCertificateReturnService, StudentsCertificateReturnService>();
            container.RegisterType<IUniversityCertificateService, UniversityCertificateService>();
            container.RegisterType<IInternalExamScheduleService, InternalExamService>();
            container.RegisterType<IExamScheduleService, ExamScheduleService>();
            container.RegisterType<ISecondLanguageService, SecondLanguageService>();
            container.RegisterType<IExamCentreService, ExamCentreService>();
            container.RegisterType<IExamTermService, ExamTermService>();
            container.RegisterType<IDepartmentMasterService, DepartmentMasterService>();
            container.RegisterType<ICertificateTypeService, CertificateTypeService>();
            container.RegisterType<INatureOfEnquiryService, NatureOfEnquiryService>();
            container.RegisterType<IReligionService, ReligionService>();
            container.RegisterType<IBaseActivityLogSearchService, BaseActivityLogSearchService>();

            container.RegisterType<IEmployeeContactService, EmployeeContactService>();
            container.RegisterType<IEmployeeExperienceService, EmployeeExperienceService>();
            container.RegisterType<IEmployeeIdentityService, EmployeeIdentityService>();
            container.RegisterType<IEmployeeService, EmployeeService>();
            container.RegisterType<IEmployeePersonalService, EmployeePersonalService>();
            container.RegisterType<IEmployeeAccountService, EmployeeAccountService>();
            container.RegisterType<IEmployeeEducationService, EmployeeEducationService>();
            container.RegisterType<IEmployeeLanguageSkillService, EmployeeLanguageSkillService>();
            container.RegisterType<IEmployeeLoanService, EmployeeLoanService>();

            container.RegisterType<IEmployeeSalarySettingsService, EmployeeSalarySettingsService>();
            container.RegisterType<IEmployeeSalaryService, EmployeeSalaryService>();
            container.RegisterType<IEmployeeAttendanceService, EmployeeAttendanceService>();
            container.RegisterType<ILeaveApplicationService, LeaveApplicationService>();
            container.RegisterType<IShiftService, ShiftService>();
            container.RegisterType<ISalaryComponentService, SalaryComponentService>();

            container.RegisterType<IEmployeeTaskService, EmployeeTaskService>();
            container.RegisterType<IEmployeeUserAccountService, EmployeeUserAccountService>();
            container.RegisterType<IEmployeeUserPermissionService, EmployeeUserPermissionService>();
            container.RegisterType<IEmployeeClassAllocationService, EmployeeClassAllocationService>();
            container.RegisterType<ISharedService, SharedService>();
            container.RegisterType<IReportService, ReportService>();
            container.RegisterType<IInternalExamResultService, InternalExamResultService>();
            container.RegisterType<IBuildingService, BuildingService>();
            container.RegisterType<IBatchService, BatchService>();
            container.RegisterType<IBaseStudentSearchService, BaseStudentSearchService>();
            container.RegisterType<IInternalExamTermService, InternalExamTermService>();
            container.RegisterType<IAttendanceTypeService, AttendanceTypeService>();
            container.RegisterType<IDivisionService, DivisionService>();
            container.RegisterType<IStudentStatusService, StudentStatusService>();
            container.RegisterType<IMediumService, MediumService>();
            container.RegisterType<IClassModeService, ClassModeService>();
            //container.RegisterType<IBookIssueSummaryReportService, BookIssueSummaryReportService>();
            container.RegisterType<IApplicationFamilyDetailsService, ApplicationFamilyDetailsService>();
            container.RegisterType<IAccountFlowService, AccountFlowService>();
            container.RegisterType<IBankAccountService, BankAccountService>();
            container.RegisterType<IJournalService, JournalService>();
            container.RegisterType<IAccountHeadService, AccountHeadService>();
            container.RegisterType<IDesignationGradeService, DesignationGradeService>();
            container.RegisterType<INotificationTemplateService, NotificationTemplateService>();

            container.RegisterType<IBankStatementService, BankStatementService>();
            container.RegisterType<IBankReconciliationService, BankReconciliationService>();
            container.RegisterType<IFutureTransactionService, FutureTransactionService>();
            container.RegisterType<IChequeClearanceService, ChequeClearanceService>();
            container.RegisterType<ICourseSubjectService, CourseSubjectService>();
            container.RegisterType<IExamResultService, ExamResultService>();
            container.RegisterType<IEnquiryCallStatusService, EnquiryCallStatusService>();
            container.RegisterType<ISyllabusAndStudyMaterialService, SyllabusAndStudyMaterialService>();
            container.RegisterType<IScholarshipService, ScholarshipService>();
            container.RegisterType<IScholarshipExamScheduleService, ScholarshipExamScheduleService>();
            container.RegisterType<IUnitTestScheduleService, UnitTestScheduleService>();
            container.RegisterType<ICashFlowService, CashFlowService>();
            container.RegisterType<IPrintInvoiceService, PrintInvoiceService>();
            container.RegisterType<IAccountHeadOpeningBalanceService, AccountHeadOpeningBalanceService>();
            container.RegisterType<IWorkScheduleService, WorkScheduleService>();
            container.RegisterType<IBulkEmailSmsService, BulkEmailSmsService>();

            container.RegisterType<IAssetService, AssetService>();
            container.RegisterType<IAssetPurchaseService, AssetPurchaseService>();
            container.RegisterType<IAssetTypeService, AssetTypeService>();
            container.RegisterType<IDepreciationService, DepreciationService>();
            container.RegisterType<IPartyService, PartyService>();

            //BookSuite

            container.RegisterType<IDesignationService, DesignationService>();
            container.RegisterType<IStatusService, StatusService>();
            container.RegisterType<ILanguageService, LanguageService>();
            container.RegisterType<IRackService, RackService>();
            container.RegisterType<ITransactionTypeService, TransactionTypeService>();
            container.RegisterType<IBookCategoryService, BookCategoryService>();

            container.RegisterType<IBookStatusService, BookStatusService>();
            container.RegisterType<IPublisherService, PublisherService>();
            container.RegisterType<IAuthorService, AuthorService>();
            container.RegisterType<IBookIssueTypeService, BookIssueTypeService>();
            container.RegisterType<IBorrowerTypeService, BorrowerTypeService>();
            container.RegisterType<IMemberTypeService, MemberTypeService>();
            container.RegisterType<ILibraryBookService, LibraryBookService>();
            container.RegisterType<IBookCopyService, BookCopyService>();
            container.RegisterType<IMemberRegistrationService, MemberRegistrationService>();
            container.RegisterType<IBookIssueReturnService, BookIssueReturnService>();
            container.RegisterType<IMemberPlanDetailsService, MemberPlanDetailsService>();
            container.RegisterType<IStudentPortalService, StudentPortalService>();
            container.RegisterType<INotificationDataService, NotificationDataService>();
            container.RegisterType<ISelectListService, SelectListService>();
            container.RegisterType<ILibraryReportService, LibraryReportService>();



            container.RegisterType<IHolidayService, HolidayService>();
            container.RegisterType<IStudentLateService, StudentLateService>();
            container.RegisterType<IStudentAbscondersService, StudentAbscondersService>();
            container.RegisterType<IStudentEarlyDepartureService, StudentEarlyDepartureService>();
            container.RegisterType<IStudentDiaryService, StudentDiaryService>();
            container.RegisterType<IStudentLeaveService, StudentLeaveService>();
            container.RegisterType<ISelectListService, SelectListService>();
            container.RegisterType<IEmployeeSubjectModuleService, EmployeeSubjectModuleService>();
            container.RegisterType<IStudentTimeTableService, StudentTimeTableService>();
            container.RegisterType<IAttendanceTypeMasterService, AttendanceTypeMasterService>();
            container.RegisterType<ITimeTableTempService, TimeTableTempService>();
            container.RegisterType<IStudentPortalService, StudentPortalService>();
            container.RegisterType<INotificationDataService, NotificationDataService>();
            container.RegisterType<IStudentStudyMaterialService, StudentStudyMaterialService>();
            container.RegisterType<IDashBoardService, DashBoardService>();
            container.RegisterType<IAttendanceConfigurationService, AttendanceConfigurationService>();
            container.RegisterType<IAttendanceCategoryService, AttendanceCategoryService>();
            container.RegisterType<IDepartmentShiftService, DepartmentShiftService>();
            container.RegisterType<IEmployeeShiftService, EmployeeShiftService>();

            container.RegisterType<IStudentTCService, StudentTCService>();
            container.RegisterType<IBankTransactionService, BankTransactionService>();
            container.RegisterType<ICashTransactionService, CashTransactionService>();
            container.RegisterType<IFeeRefundService, FeeRefundService>();
            container.RegisterType<IDocumentTrackService, DocumentTrackService>();
            container.RegisterType<IAgentService, AgentService>();
            container.RegisterType<IUniversityCancelationService, UniversityCancelationService>();
            container.RegisterType<IBonafiedCertificateService, BonafiedCertificateService>();
            container.RegisterType<ICourseCompletionCertificateService, CourseCompletionCertificateService>();
            container.RegisterType<ICourseTransferService, CourseTransferService>();
            container.RegisterType<IApplicationWebFormService, ApplicationWebFormService>();
            container.RegisterType<IMenuDetailViewService, MenuDetailViewService>();
            container.RegisterType<IMenuTypeService, MenuTypeService>();
            container.RegisterType<ICounsellingTimeService, CounsellingTimeService>();
            container.RegisterType<IEmployeeHeirarchyService, EmployeeHeirarchyService>();
            container.RegisterType<IEmployeeEnquiryTargetService, EmployeeEnquiryTargetService>();
            container.RegisterType<ICasteService, CasteService>();
            container.RegisterType<IBankService, BankService>();
            container.RegisterType<IAcademicTermService, AcademicTermService>();
            container.RegisterType<IGSTMasterService, GSTMasterService>();
            container.RegisterType<IDashBoardTypeService, DashBoardTypeService>();
            container.RegisterType<IDashBoardContentService, DashBoardContentService>();
            container.RegisterType<ISalaryOtherAmountTypeService, SalaryOtherAmountTypeService>();
            container.RegisterType<IFieldValidationService, FieldValidationService>();
            container.RegisterType<ITCReasonMasterService, TCReasonMasterService>();
            container.RegisterType<IFeeFollowUpService, FeeFollowUpService>();
            container.RegisterType<ICompanyService, CompanyService>();
            container.RegisterType<IApplicationScheduleService, ApplicationScheduleService>();
            container.RegisterType<IApplicationScheduleCallStatusService, ApplicationScheduleCallStatusService>();
            container.RegisterType<IApplicationScheduleTypeService, ApplicationScheduleTypeService>();
            container.RegisterType<IESSLAttendanceService, ESSLAttendanceService>();
            container.RegisterType<IUserManualService, UserManualService>();


            #region exam
            container.RegisterType<ITestPaperService, TestPaperService>();
            container.RegisterType<IExamTestService, ExamTestService>();
            container.RegisterType<IMarkGroupService, MarkGroupService>();
            container.RegisterType<ISelectListService, SelectListService>();
            container.RegisterType<IVideoService, VideoService>();
            #endregion
        }
    }
}