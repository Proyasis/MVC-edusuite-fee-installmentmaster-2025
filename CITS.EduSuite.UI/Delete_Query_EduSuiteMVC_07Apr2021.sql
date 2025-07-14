



--Edusuite MVC Delete application Query
-------------------------------


delete from InternalExamResult
delete from InternalExamDivision
delete from InternalExamDetails
delete from InternalExam
delete from StudentsCertificateReturn
delete from UniversityCertificate
delete from EducationQualification
delete from StudentIDCard
delete from PromotionDetails
delete from Promotion
delete from AttendanceDetails
delete from Attendance
delete from StudentDivisionAllocation
delete from FeePaymentDetail
delete from FeePaymentMaster
delete from ApplicationFamilyDetails
delete from StudentDocument
delete from UniversityPaymentCancelation
delete from UniversityPaymentDetail
delete from UniversityPaymentMaster
delete from AdmissionFee
delete from ExamResult
delete from ExamSchedule
delete from StudentFeeInstallment
delete from IssueOfStudyMaterials
delete from UnitTestResult
delete from UnitTestTopics
delete from UnitTestSchedule
delete from StudentPapers
delete from ElectivePaper
delete from StudentDiary
delete from StudentLateDetails
delete from StudentLeaveDetails
delete from StudentAbsconders
delete from StudentEarlyDeparture
delete from ExamTestAnswer
delete from ExamTestSection
delete from ExamTest
delete from FeeRefundDetail
delete from FeeRefundMaster
delete from FeePaymentDetail
delete from FeePaymentMaster
delete from BonafiedCertificate
delete from CourseCompletionCertificate
delete from TransferAdmissionFee
delete from StudentCourseTransfer
delete from StudentTCDetails
delete from StudentTCMaster
delete from StudentCourseTransfer
delete from ApplicationWebForm
delete from Application

update Application set AppuserKey=null
update Application set EnquiryKey=null
----------------------------------

--select *  from AppUser
update AppUser set FirstName='Admin'
update AppUser set LastName=' '
update PaymentReceiptNumberConfiguration set MaxValue=0



delete from   ScholarshipExamResult
delete from   ScholarshipExamSchedule
delete from   Scholarship


--Enquiry
----------------
delete from   EnquiryFeedback
delete from Enquiry
delete from EnquiryLeadFeedback
delete from  EnquiryLead
------------------------

-- Employee
-----------------------

delete from EmployeeContact
delete from EmployeeEducation
delete from EmployeeAccount
delete from EmployeeAchievement
delete from EmployeeAttendance
delete from EmployeeAttendancelog
delete from EmployeeExperience
delete from EmployeeFileHandover
delete from EmployeeIdentity
delete from EmployeeLanguageSkill
delete from EmployeeLoan
delete from EmployeeLoanPayment
delete from EmployeeMonthlyPF
delete from EmployeeOvertime
delete from EmployeeSalaryDetail
delete from EmployeeSalaryMaster
delete from EmployeeSalaryPayment
delete from EmployeeTask
delete from UserPermission
delete from EmployeeSalaryAdvanceDetails
delete from EmployeeSalaryAdvanceReturnDetails
delete from EmployeeSalaryAdvancePayment
delete from EmployeeSalaryDetail
delete from EmployeeSalaryMaster
delete from EmployeeSalaryPayment
delete from EmployeeSalaryAdvanceReturnDetails
delete from EmployeeSalaryOtherAmount
delete from EmployeeSalaryAdvanceReturnMaster
delete from TeacherClassAllocation
delete from TeacherWorkScheduleDetails
delete from TeacherWorkScheduleMaster
delete from TeacherSubjectAllocation
delete from TeacherSubjectModules
delete from Studenttimetable
delete from TimeTableTempDetails
delete from TimeTableTempMaster
delete from EmployeeAttendance
delete from EmployeeAttendanceLog
delete from DocumentTrack
delete from DashBoardUserPermission
delete from EmployeeShiftDetail
delete from EmployeeHierarchy
delete from LeaveApplication
delete from EmployeeEnquiryTargetDetails
delete from EmployeeEnquiryTarget
delete from Employee

update Employee set AppuserKey=null

delete from ForgotPassword
delete from ActivityLog
delete from Audit
delete from AppUser where RowKey>2


-----------------------------------


-- Account
------------------------
delete from CashFlow

delete from BankStatementDetails
delete from BankStatementMaster
delete from OtherBankTransactions
delete from BankReconciliation
delete from ChequeClearance

delete from JournalDetails
delete from JournalMaster
delete from BankTransaction
delete from CashTransaction

delete from FutureTransactionPayment
delete from FutureTransactions
delete from  Accountflow
--select * from  Accountflow
--select * from  accounthead
delete from accounthead where rowkey>26
-----------------------------------------

delete from SubBranch
delete from BankAccount
delete from BranchAccount

delete from AccountHeadOpeningBalance
delete from  BranchDepartment
delete from  AttendanceConfiguration
delete from Branch
--delete from Bank


------------------------------------- Library -
delete from MemberRegistration
delete from MemberPlanDetails
delete from BookIssueReturnDetails
delete from BookIssueReturnMaster
delete from BookCopy
delete from LibraryBook
delete from Rack
delete from SubRack


----------------------------


--Master--

delete from ClassDetails
delete from Batch
delete from FeeType
delete from UniversityCourseFee
delete from UniversityCourseFeeInstallment
delete from UniversityCourse
delete from CourseSubjectDetails
delete from CourseSubjectMaster
delete from StudyMaterials

delete from ModuleTopics
delete from SubjectModules
delete from StudentStudyMaterialDetails
delete from StudentStudyMaterial
delete from Subjects

delete from UniversityMaster
delete from Course
delete from Coursetype

delete from StudyMaterials
delete from Books
delete from AttendanceTypeMasterClasses
delete from AttendanceTypeDetails
delete from AttendanceTypeMaster
delete from AttendanceType
delete from HolidayClasses
delete from Holiday
delete from Agent
delete from CertificateType
delete from InternalExamTerm



update PaymentReceiptNumberConfiguration set MaxValue=0 
-- Online Class



delete from TestPaper
delete from TestModule
delete from TestSection
delete from TestQuestion
delete from TestQuestionOptions

