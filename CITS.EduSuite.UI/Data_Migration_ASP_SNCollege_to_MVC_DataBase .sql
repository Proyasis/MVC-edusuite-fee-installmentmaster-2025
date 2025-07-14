insert into IAMT_Migrate_02July2022.dbo.Agent
 select A.RowKey,A.AgentName,A.AgentEmail,A.AgentOpeningBalance,A.AgentOpeningBalanceType,A.AgentAddress,A.AgentPhone,A.AgentMobile,A.AgentActive,A.AgentCode,
 A.DateAdded,A.AddedBy,A.DateModified,A.ModifiedBy from
  (select AG.AgentId as RowKey,AG.AgentName,AG.AgentEmail,AG.AgentOpeningBalance,AG.AgentOpeningBalanceType,
AG.AgentAddress,AG.AgentPhone,AG.AgentMobile,
CASE WHEN AG.AgentActive='Y' THEN 1 ElSE 0 End as AgentActive,AG.AgentCode,
AG.CreatedOn as DateAdded,1 as AddedBy,null as DateModified,null as	ModifiedBy from iamtGlobal_02July2022.dbo.M_Agent AG) as A

  select * from  iamtGlobal_02July2022.dbo.M_Agent
select * from  IAMT_Migrate_02July2022.dbo.Agent
-----------------

--Bank

insert into IAMT_Migrate_02July2022.dbo.Bank
Select BNK.BankId as RowKey,BNK.BankName,null as BankNameLocal,BNK.BankId as DisplayOrder,
CASE WHEN BNK.BankActive='Y' THEN 1 ElSE 0 End as IsActive,
BNK.CreatedOn as DateAdded,1 as AddedBy,null as DateModified,null as	ModifiedBy from iamtGlobal_02July2022.dbo.M_Bank BNK

select * from  iamtGlobal_02July2022.dbo.M_Bank
select * from  IAMT_Migrate_02July2022.dbo.Bank


-------------

--Batch
insert into IAMT_Migrate_02July2022.dbo.Batch
Select distinct B.BatchId as RowKey,B.BatchName,B.BatchCode,1 as IsActive,
B.CreatedOn as DateAdded,1 as AddedBy,null as DateModified,null as	ModifiedBy,null as Durationfromyear,null as DurationToYear 
from iamtGlobal_02July2022.dbo.M_Batch B
inner join iamtGlobal_02July2022.dbo.T_Application A on B.BatchId=A.BatchId
where Convert(date,A.CreatedOn) > '2022-01-01'

select * from  iamtGlobal_02July2022.dbo.M_Batch
select * from  IAMT_Migrate_02July2022.dbo.Batch
-----------------

--Branch

insert into IAMT_Migrate_02July2022.dbo.Branch
Select BR.BranchId as RowKey,1 as CompanyKey, BR.BranchCode,BR.BranchName,NULL as BranchNameLocal,
isnull(BR.BranchAddress,'Ace' )as AddressLine1,Null as AddressLine2,Null as AddressLine3,BR.BranchLocation as CityName,1 as DistrictKey,
Null as PostalCode,ISNULL(BR.BranchPhone,'9446051055') as PhoneNumber1,BR.BranchMobile as PhoneNumber2,NULL as FaxNumber
,BR.BranchEmail as EmailAddress,NULL as OpeningCashBalance,NULL as CurrentCashBalance,NULL as ContactPersonName,
NULL as ContactPersonPhone,NULL as DesignationKey,
ROW_number()Over(Order By BranchId) as DisplayOrder,1 as IsActive,BR.CreatedOn as DateAdded,
1 as AddedBy,null as DateModified,null as	ModifiedBy,NULL as TotalWorkingDays,NULL as TotalWorkingHours,
0 as IsFranchise,null as BranchLogo
from  iamtGlobal_02July2022.dbo.M_Branch BR

select * from  iamtGlobal_02July2022.dbo.M_Branch
select * from  IAMT_Migrate_02July2022.dbo.Branch



----------------------

-- PaymentMode

insert into IAMT_Migrate_02July2022.dbo.PaymentMode
SELECT cashTypeid as RowKey
      ,CashTypeName AS PaymentModeName
      ,NULL AS PaymentModeLocal
      ,cashTypeid as DisplayOrder
      ,1 as [IsActive]   
      ,Getdate() as [CreatedOn]
      ,1 as CreatedBy
      ,null as [ModifiedOn]
      ,null as [ModifiedBy]
      
  FROM iamtGlobal_02July2022.dbo.M_CashType 
  
  select * from  iamtGlobal_02July2022.dbo.M_CashType
select * from  IAMT_Migrate_02July2022.dbo.PaymentMode
------------------------


------ CertificateType 

insert into IAMT_Migrate_02July2022.dbo.CertificateType
SELECT CertificateTypeId as [RowKey],[CertificateTypeName] ,CASE WHEN IsActive='Y' THEN 1 ElSE 0 End as IsActive,
     CreatedOn as DateAdded,1 as AddedBy,null as DateModified,null as	ModifiedBy
  FROM iamtGlobal_02July2022.dbo.M_CertificateType 
  
  select * from  iamtGlobal_02July2022.dbo.M_CertificateType
select * from  IAMT_Migrate_02July2022.dbo.CertificateType
  
---------------------


------ ClassMode 
insert into IAMT_Migrate_02July2022.dbo.ClassMode 
SELECT ClassModeid as [RowKey],[ClassModeName],
     CreatedOn as DateAdded,1 as AddedBy,null as DateModified,null as	ModifiedBy ,1 as IsActive
  FROM iamtGlobal_02July2022.dbo.M_ClassMode 
  
    select * from  iamtGlobal_02July2022.dbo.M_ClassMode
select * from  IAMT_Migrate_02July2022.dbo.ClassMode
  
  
  -------------------------
  
  
  
------ Course Type Start
insert into IAMT_Migrate_02July2022.dbo.CourseType
Select CT.CourseTypeId as RowKey,CT.CourseTypeName,
CASE WHEN CT.CourseTypeActive='Y' THEN 1 ElSE 0 End as IsActive,0 as HasSecondLanguage,
CT.CreatedOn as DateAdded,1 as AddedBy,null as DateModified,null as	ModifiedBy from iamtGlobal_02July2022.dbo.M_CourseType CT

  select * from  iamtGlobal_02July2022.dbo.M_CourseType
select * from  IAMT_Migrate_02July2022.dbo.CourseType
--------------------


----- Course --
insert into IAMT_Migrate_02July2022.dbo.Course
select C.courseId As RowKey,CourseCode,	CourseName,	CourseYear,
CASE WHEN C.CourseActive='Y' THEN 1 ElSE 0 End as IsActive,C.CourseTypeId as CourseTypeKey,	CourseDuration,
C.CreatedOn as DateAdded,1 as AddedBy,null as DateModified,null as	ModifiedBy from iamtGlobal_02July2022.dbo.M_Course C 
inner join iamtGlobal_02July2022.dbo.M_CourseType CT on C.CourseTypeId=CT.CourseTypeId 


 select distinct * from  iamtGlobal_02July2022.dbo.M_Course C left join iamtGlobal_02July2022.dbo.M_CourseType CT on C.CourseTypeId=CT.CourseTypeId 
  
select * from  IAMT_Migrate_02July2022.dbo.Course

---------------------



----- Department 
insert into IAMT_Migrate_02July2022.dbo.Department
select DepartmentId as [RowKey]
      ,null as [DepartmentCode]
      ,[DepartmentName]
      ,ROW_number()Over(Order By DepartmentId) as[DisplayOrder],
CASE WHEN IsActive='Y' THEN 1 ElSE 0 End as IsActive,
CreatedOn as DateAdded,1 as AddedBy,null as DateModified,null as	ModifiedBy from iamtGlobal_02July2022.dbo.M_Department


  select * from  iamtGlobal_02July2022.dbo.M_Department
select * from  IAMT_Migrate_02July2022.dbo.Department
--delete from IAMT_Migrate_02July2022.dbo.Department
--delete from IAMT_Migrate_02July2022.dbo.DepartmentShift
----------------------

----- Designation 
insert into IAMT_Migrate_02July2022.dbo.Designation
select DesignationId as [RowKey],[DesignationName],null as [HigherDesignationKey],
ROW_number()Over(Order By DesignationId) as[DisplayOrder],1 as IsActive,
CreatedOn as DateAdded,1 as AddedBy,null as DateModified,null as	ModifiedBy from iamtGlobal_02July2022.dbo.M_Designation
where DesignationId<>1

 select * from  iamtGlobal_02July2022.dbo.M_Designation
select * from  IAMT_Migrate_02July2022.dbo.Designation
--delete from IAMT_Migrate_02July2022.dbo.Designation where Rowkey<>1
--delete from IAMT_Migrate_02July2022.dbo.DesignationGrade
--delete from IAMT_Migrate_02July2022.dbo.DesignationGradeDetail

-----------------------

insert into IAMT_Migrate_02July2022.dbo.EnquiryCallStatus
select EnquiryCallStatusid as [RowKey],[EnquiryCallStatusName],EnquiryStatusid as [EnquiryStatusKey],
0 as [IsDuration],IsActive,CreatedOn as DateAdded,1 as AddedBy,null as DateModified,null as	ModifiedBy,
'' as [ShowInMenuKeys] from iamtGlobal_02July2022.dbo.M_EnquiryCallStatus


 select * from  iamtGlobal_02July2022.dbo.M_EnquiryCallStatus

select * from  IAMT_Migrate_02July2022.dbo.EnquiryCallStatus


-----------


----- EnquiryLeadCallStatus 

insert into IAMT_Migrate_02July2022.dbo.EnquiryLeadCallStatus 
select EnquiryLeadCallStatusid as [RowKey],[EnquiryleadCallStatusName] from iamtGlobal_02July2022.dbo. M_EnquiryLeadCallStatus


 select * from  iamtGlobal_02July2022.dbo.M_EnquiryLeadCallStatus
select * from  IAMT_Migrate_02July2022.dbo.EnquiryLeadCallStatus 
----- EnquiryLeadCallStatus End --------




----- EnquiryStatus Start --------

 select * from  iamtGlobal_02July2022.dbo.M_EnquiryStatus
select * from  IAMT_Migrate_02July2022.dbo.EnquiryStatus 

UPDATE  EnquiryStatus set EnquiryStatusCode =ES.EnquiryStatusCode
    from iamtGlobal_02July2022.dbo.M_EnquiryStatus ES where ES.EnquiryStatusId=RowKey

----------


----- ExamCentre Start --------
insert into IAMT_Migrate_02July2022.dbo.ExamCenter
select ExamCentreId as [RowKey],[ExamCentreName]
      ,[ExamCentreCode],
      CASE WHEN IsActive='Y' THEN 1 ElSE 0 End as IsActive,
      ISNull(CreatedOn,GETDATE()) as DateAdded,1 as AddedBy,null as DateModified,null as	ModifiedBy from iamtGlobal_02July2022.dbo.M_ExamCentre
      

 select * from  iamtGlobal_02July2022.dbo.M_ExamCentre
select * from  IAMT_Migrate_02July2022.dbo.ExamCenter 

--delete from IAMT_Migrate_02July2022.dbo.ExamCenter
----------

--Exam term

insert into IAMT_Migrate_02July2022.dbo.ExamTerm
select ExamId as [RowKey],[ExamName],
      CASE WHEN IsActive='Y' THEN 1 ElSE 0 End as IsActive,
      CreatedOn as DateAdded,1 as AddedBy,null as DateModified,null as	ModifiedBy from iamtGlobal_02July2022.dbo.M_ExamName
      
     select * from  iamtGlobal_02July2022.dbo.M_ExamName
select * from  IAMT_Migrate_02July2022.dbo.ExamTerm 

--delete from IAMT_Migrate_02July2022.dbo.ExamTerm  
--------------------------


----- FeeType

insert into IAMT_Migrate_02July2022.dbo.FeeType
select FeeTypeid as RowKey,FeeTypeName,1 as CashFlowTypeKey,IsDeductFee as IsDeduct,null as FeeTypeModeKey,
1 as IsTax,null as GSTMasterkey,1 as IsActive,
CreatedOn as DateAdded,1 as AddedBy,null as DateModified,null as	ModifiedBy,0 as IsUniverisity,null as	AccountHeadKey, 1 as ReceiptNumberconfigurationKey from iamtGlobal_02July2022.dbo.M_FeeType


 select * from  iamtGlobal_02July2022.dbo.M_FeeType
select * from  IAMT_Migrate_02July2022.dbo.FeeType 
--mustly enter with Account head
--------------------------

----- Income

insert into IAMT_Migrate_02July2022.dbo.Income
select IncomeId as RowKey,IncomeName,1 as IsActive, CreatedOn as DateAdded,
1 as AddedBy,null as DateModified,null as	ModifiedBy from iamtGlobal_02July2022.dbo.M_Income

--------------------------

----- InternalExamTerm 

insert into IAMT_Migrate_02July2022.dbo.InternalExamTerm
select InternalExamTermId as RowKey,InternalExamTermName, CASE WHEN IsActive='Y' THEN 1 ElSE 0 End as IsActive,
CreatedOn as DateAdded,
1 as AddedBy,null as DateModified,null as	ModifiedBy from iamtGlobal_02July2022.dbo.M_InternalExamTerm


 select * from  iamtGlobal_02July2022.dbo.M_InternalExamTerm
select * from  IAMT_Migrate_02July2022.dbo.InternalExamTerm 

--------------------------

---AppUser

insert into IAMT_Migrate_02July2022.dbo.APpUser
select Id as RowKey,UserId as AppUserName ,S.StaffName as[FirstName] ,'' as[MiddleName]
,'' as[LastName] ,'' as[SurName] ,null as[EmailAddress] ,1as[Phone1]
,null as[Phone2],null as[Image],null as[BloodGroup],null as[Password],null as[DesignationKey]
,UserTypeId as[RoleKey],CASE WHEN Active='Y' THEN 1 ElSE 0 End as IsActive,null as[PasswordHint], 
GETDATE() as DateAdded,1 as AddedBy,null as DateModified,null as	ModifiedBy,1as[IsLocked],null as ApplicationKey from iamtGlobal_02July2022.dbo.M_Login L
 inner Join iamtGlobal_02July2022.dbo.M_Staff S on L.UserId=S.StaffLoginId
where Id not in (1,2)



select * from  IAMT_Migrate_02July2022.dbo.AppUser	  
select * from  iamtGlobal_02July2022.dbo.M_login


----------------




----- Medium  
insert into IAMT_Migrate_02July2022.dbo.Medium
select MediumId as RowKey,MediumName, 
CreatedOn as DateAdded,
1 as AddedBy,null as DateModified,null as	ModifiedBy,
CASE WHEN IsActive='Y' THEN 1 ElSE 0 End as IsActive from iamtGlobal_02July2022.dbo.M_Medium


----------------




----- Mode  

insert into IAMT_Migrate_02July2022.dbo.Mode
select ModeId as RowKey,ModeName,1 as StartYear,
CreatedOn as DateAdded,
1 as AddedBy,null as DateModified,null as	ModifiedBy,1 as IsActive from iamtGlobal_02July2022.dbo.M_Mode

 select * from  iamtGlobal_02July2022.dbo.M_Mode
select * from  IAMT_Migrate_02July2022.dbo.Mode 
----------------



----- NatureOfEnquiry 

insert into IAMT_Migrate_02July2022.dbo.NatureOfEnquiry 
select NatureofEnquiryid as RowKey,NatureOfEnquiryName,1 as IsActive,
CreatedOn as DateAdded,
1 as AddedBy,null as DateModified,null as	ModifiedBy,null as IsAPI from  iamtGlobal_02July2022.dbo.M_NatureOfEnquiry
where NatureOfEnquiryId>6

 select * from  iamtGlobal_02July2022.dbo.M_NatureOfEnquiry
select * from  IAMT_Migrate_02July2022.dbo.NatureOfEnquiry 
------------------



----- PromotionStatus  

insert into IAMT_Migrate_02July2022.dbo.PromotionStatus
select PromotionStatusId as RowKey,PromotionStatusName, CASE WHEN IsActive='Y' THEN 1 ElSE 0 End as IsActive,
CreatedOn as DateAdded,
1 as AddedBy,null as DateModified,null as	ModifiedBy from iamtGlobal_02July2022.dbo.M_PromotionStatus


------------------


----- Religion 

insert into IAMT_Migrate_02July2022.dbo.Religion 
select ReligionId as RowKey,ReligionName,''as[ReligionNameLocal],
ROW_number()Over(Order By ReligionId) as[DisplayOrder],
1 as IsActive,CreatedOn as DateAdded,1 as AddedBy,null as DateModified,null as	ModifiedBy from iamtGlobal_02July2022.dbo.M_Religion

 select * from  iamtGlobal_02July2022.dbo.M_Religion
select * from  IAMT_Migrate_02July2022.dbo.Religion 
------------------



----- SecondLanguage 

insert into IAMT_Migrate_02July2022.dbo.SecondLanguage
select SeCondLanguageId as RowKey,SecondLanguageName,
CreatedOn as DateAdded,
1 as AddedBy,null as DateModified,null as	ModifiedBy,1 as IsActive from iamtGlobal_02July2022.dbo.M_SecondLanguage

 select * from  iamtGlobal_02July2022.dbo.M_SecondLanguage
select * from  IAMT_Migrate_02July2022.dbo.SecondLanguage 
-----------------



----- UniversityMaster
insert into IAMT_Migrate_02July2022.dbo.UniversityMaster
select UM.UniversityMasterId As RowKey,ISNULL(Um.UniversityMasterCode,'CU'),	UniversityMasterName,
CASE WHEN UM.IsActive='Y' THEN 1 ElSE 0 End as IsActive,
UM.CreatedOn as DateAdded,1 as AddedBy,null as DateModified,null as	ModifiedBy,null as AccountHeadKey  from iamtGlobal_02July2022.dbo.M_UniversityMaster UM


select * from  iamtGlobal_02July2022.dbo.M_UniversityMaster
select * from  IAMT_Migrate_02July2022.dbo.UniversityMaster 
-----------------




----- UniversityCourse 

insert into IAMT_Migrate_02July2022.dbo.UniversityCourse
select UC.UniversityId As RowKey,UC.UniversityMasterid as UniversityMasterKey,UC.Courseid as CourseKey,
1  as AcademicTermKey,
CASE WHEN UC.UniversityActive='Y' THEN 1 ElSE 0 End as IsActive,
UC.CreatedOn as DateAdded,1 as AddedBy,null as DateModified,null as	ModifiedBy  from iamtGlobal_02July2022.dbo.M_UniversityCourse UC


select * from  iamtGlobal_02July2022.dbo.M_UniversityCourse
select * from  IAMT_Migrate_02July2022.dbo.UniversityCourse


----------- Employee 

insert into IAMT_Migrate_02July2022.dbo.Employee
select StaffId as RowKey
	  
      ,1 as[SalutationKey]
      ,''as[EmployeeCode]
      ,staffName as[FirstName]
      ,''as[LastName]
      ,''as[MiddleName]
      ,Getdate() as[DateOfBirth]
      ,'' as[Gender]
      ,51 as[TelephoneCodeKey]
      ,null as[PhoneNumber]
      ,ISNULL(StaffMobile,'9846252413') as[MobileNumber]
      ,StaffEmail as[EmailAddress]
      ,0as[NotificationByEmail]
      ,0as[NotificationBySMS]
      ,null as[BloodGroupKey]
      ,null as[NationalityKey]
      ,null as[ReligionKey]
      ,null as[MaritalStatusKey]
      ,ISNULL(StaffDateOfJoin,GETDATE()) as[JoiningDate]
      ,StaffDateOfJoin as[ReleiveDate]
      ,BranchId as[BranchKey]
      ,DepartmentId as[DepartmentKey]
      ,1as[EmployeeCategoryKey]
      ,ISNULL(DesignationId,1) as[DesignationKey]
      ,null as[GradeKey]
      ,1as[EmployeeStatusKey]
      ,null as[BiomatricID]
      ,null as[Photo]
      ,null as[EmergencyContactPerson]
      ,null as[ContactPersonRelationship]
      ,null as[ContactPersonNumber]
      ,0as[HasTarget]
      ,'' as[TargetTitle]
      ,null as[TargetCount]
      ,L.Id as[AppUserKey]
      ,null as[HigherEmployeeUserKey]
      ,null as[BranchAccess]      
	  ,ISNULL(CreatedOn,GETDATE()) as DateAdded,
	  1 as AddedBy,null as DateModified,null as	ModifiedBy,	  
	  CASE WHEN StaffActive='Y' THEN 1 ElSE 0 End as IsActive,
	  0 as [IsTeacher]
	  ,null as[SalaryTypeKey]
	  ,null as[SalaryAmount]
	  ,null as[WorkTypeKey]
	  ,null as[AttendanceCategoryKey]
	  ,null as[AttendanceConfigTypeKey]
	  ,null as[ShiftKey]
	  ,null as[AccountHeadKey]
	   from iamtGlobal_02July2022.dbo.M_Staff S
	    inner Join iamtGlobal_02July2022.dbo.M_Login L on L.UserId=S.StaffLoginId
		where S.StaffId not in (1,2)

select * from  IAMT_Migrate_02July2022.dbo.Employee	 
select * from  iamtGlobal_02July2022.dbo.M_Staff


select * from  IAMT_Migrate_02July2022.dbo.AppUser	  
select * from  iamtGlobal_02July2022.dbo.M_login
-------------------



----- StudentStatus  

insert into IAMT_Migrate_02July2022.dbo.StudentStatus
select StudentStatusId as RowKey,StudentStatusName,1 as IsActive,
CreatedOn as DateAdded,
1 as AddedBy,null as DateModified,null as	ModifiedBy from iamtGlobal_02July2022.dbo.M_StudentStatus

-------------------



---------------------- TeacherClassAllocation  

insert into IAMT_Migrate_02July2022.dbo.TeacherClassAllocation
select ClassAllocationId as RowKey,StaffId as EmployeeKey,null as ClassDetailsKey,
Incharge,ForAttendance as IsAttendance,1 as IsActive,CreatedOn as DateAdded,
1 as AddedBy,null as DateModified,null as	ModifiedBy,null as BatchKey from iamtGlobal_02July2022.dbo.M_TeacherClassAllocation


select * from  IAMT_Migrate_02July2022.dbo.TeacherClassAllocation	  
select * from  iamtGlobal_02July2022.dbo.M_TeacherClassAllocation
----------------------

----- TeacherSubjectAllocation   

insert into IAMT_Migrate_02July2022.dbo.TeacherSubjectAllocation
select SubjectAllocationId as RowKey,StaffId as EmployeeKey,BookId as BookKey
,1 as IsActive,CreatedOn as DateAdded,1 as AddedBy,null as DateModified,null as ModifiedBy
,null as ClassDetailsKey,1 as TeacherClassAllocationKey from iamtGlobal_02July2022.dbo.M_TeacherSubjectAllocation TSA




select * from  IAMT_Migrate_02July2022.dbo.TeacherSubjectAllocation	  
select * from  iamtGlobal_02July2022.dbo.M_TeacherSubjectAllocation
------------------------


--------Application 

insert into IAMT_Migrate_02July2022.dbo.Application
select AdmissionNo as[RowKey]
      ,[ApplicationNo]
      ,A.CourseId as[CourseKey]
      ,UC.UniversityMasterId as[UniversityMasterKey]
      ,[StudentName]
      ,[StudentGuardian]
      ,[StudentMotherName]
      ,[StudentPermanentAddress]
      ,[StudentPresentAddress]
      ,[StudentEmail]
      ,[StudentPhone]
      ,[StudentMobile]
      ,[StudentDOB]
      ,Batchid as [BatchKey]
      ,case when StudentGender='M' Then 1 Else 2 End as StudentGender
      ,case When ReligionId=3 Then 1 when ReligionId=1 then 2  else 3 End as [ReligionKey]
      ,case When IncomeId=0 Then null else IncomeId End as [IncomeKey]
      ,case When SecondLanguageId=0 Then null
	  when SecondLanguageId=11 Then 4 else SecondLanguageId End as [SecondLanguageKey]
      ,
	  case When ModeId=4 Then 1
	  else ModeId end  as[ModeKey]
      ,case When ClassModeId=0 Then null else ClassModeId End as [ClassModeKey]
      ,[StartYear]
      ,ISNULL(StudentTotalFee,0)as StudentTotalFee
      ,case When StudentClassRequired='Y' Then 1 Else 0 End as StudentClassRequired
      ,[StudentClassRequiredDesc]
      ,[PresentJob_CourseOfStudyId]
      ,case When NatureOfEnquiryId=5 Then 7
			When NatureOfEnquiryId=22 Then 1
			When NatureOfEnquiryId=14 Then null
			When NatureOfEnquiryId=0 Then null
		else NatureOfEnquiryId End as [NatureOfEnquiryKey]
      ,BranchId as[BranchKey]
      ,[StudentDateOfAdmission]
      ,case When AgentId=0 Then null else AgentId End as [AgentKey]
      ,StudentStatusId as[StudentStatusKey]
      ,[StudyMaterialIssueStatus]
      ,[StudentEnrollmentNo]
      ,[StudentPhotoPath]
      ,[ExamRegisterNo]
      ,[Remarks]
      ,case When IsFromEnquiry='Y' Then 1 else 0 End As[IsFromEnquiry]
      ,case When EnquiryId=0 Then null
			when EnquiryId=14063 then null
			when EnquiryId=17948 then null

			
			else EnquiryId End as[EnquiryKey]
      ,case When MediumId=0 Then null else MediumId End as [Mediumkey]
      ,AdmissionNoNew as[AdmissionNo]
      ,[SerialNumber]
      ,0 as [HasInstallment]
      ,0 as [HasConcession]
      ,[StartYear] as [CurrentYear]
      ,null as [RollNumber]
      ,1  as AcademicTermKey
      ,0 as[HasOffer]
      ,null as[OfferKey]
      ,ISNULL(A.CreatedOn,GETDATE()) as[DateAdded]
      ,L.Id as[AddedBy]
      ,null as[DateModified]
      ,null as[ModifiedBy]
      ,null as[ClassDetailsKey]
      ,null as CasteKey
      ,null as BloodGroupKey
      ,0 as ISTAX
      ,1 as AllowLogin
      ,null as AppUserKey
      ,null as RollNoCode
      ,null as CommunityTypeKey
      ,null as EmployeeKey
      ,0 as AllowoldPaid
      ,[StartYear] as AdmissionCurrentYear
	  ,1 as EducationTypeKey
	  ,null as RegistrationCatagoryKey
      from iamtGlobal_02July2022.dbo.T_Application A
      inner join iamtGlobal_02July2022.dbo.M_UniversityCourse UC on A.UniversityId=UC.UniversityId 
      inner Join iamtGlobal_02July2022.dbo.M_Login L on L.UserId=A.CreatedBy
     
	  where Convert(date,A.CreatedOn) > '2022-01-01'
      
      
      
select * from  IAMT_Migrate_02July2022.dbo.Application	  
select * from  iamtGlobal_02July2022.dbo.T_Application where CreatedOn >
--------------------
      
      
---------Admission Fee

insert into IAMT_Migrate_02July2022.dbo.AdmissionFee
select AdmissionFeeId as [RowKey]
      ,AF.AdmissionNo as [ApplicationKey]
      ,[AdmissionFeeYear]
      ,[AdmissionFeeAmount]
      ,[ConcessionAmount]
      ,[ActualAmount]
      ,2 as[FeeTypeKey]
      ,1 as[IsActive]
      ,AF.CreatedOn as[DateAdded]
      ,1as[AddedBy]
      ,null as[DateModified]
      ,null as[ModifiedBy]
      ,null as[CGSTRate]
      ,null as[SGSTRate]
      ,null as[IGSTRate]
      ,null as[CGSTAmount]
      ,null as[SGSTAmount]
      ,null as[IGSTAmount]
      ,null as[OldPaid]
      
      from iamtGlobal_02July2022.dbo.T_AdmissionFee AF
      inner join iamtGlobal_02July2022.dbo.T_Application A on A.AdmissionNo=AF.AdmissionNo
		where Convert(date,A.CreatedOn) > '2022-01-01'

select * from  IAMT_Migrate_02July2022.dbo.AdmissionFee	  
select * from  iamtGlobal_02July2022.dbo.T_AdmissionFee
--------------------------------



-------- EducationQualification 

insert into IAMT_Migrate_02July2022.dbo.EducationQualification
SELECT EducationQualificationId as[RowKey]
      ,[EducationQualificationCourse]
      ,[EducationQualificationUniversity]
      ,[EducationQualificationYear]
      ,case When EducationQualificationResult='P' Then 1 else 0 End As[EducationQualificationResult]
      ,E.AdmissionNo as[ApplicationKey]
      ,[EducationQualificationPercentage]
      ,[EducationQualificationCertificatePath]
      ,case When IsOriginalIssued='Y' Then 1 else 0 End As[IsOriginalIssued]
      ,[OriginalIssuedDate]
      ,case When IsCopyIssued='Y' Then 1 else 0 End As[IsCopyIssued]
      ,[CopyIssuedDate]
      ,case When IsOriginalReturn='Y' Then 1 else 0 End As[IsOriginalReturn]
      ,[OriginalReturnDate]
      ,case When IsVerified='Y' Then 1 else 0 End As[IsVerified]
      ,[VerifiedDate]
      ,case when IsOriginalReturn='Y' then 3 else 2 end  as CertificateStatusKey
	  ,E.CreatedOn as[DateAdded]
      ,L.Id as[AddedBy]
      ,null as[DateModified]
      ,null as[ModifiedBy]
      ,null as originalIssuedBy
      ,null as originalReturnBy
      ,null as VerifiedBy
  FROM iamtGlobal_02July2022.dbo.T_EducationQualification E
   inner Join iamtGlobal_02July2022.dbo.M_Login L on L.UserId=E.CreatedBy
   inner join iamtGlobal_02July2022.dbo.T_Application A on A.AdmissionNo=E.AdmissionNo
   where Convert(date,A.CreatedOn) > '2022-01-01'
   
   select * from  IAMT_Migrate_02July2022.dbo.EducationQualification	  
select * from  iamtGlobal_02July2022.dbo.T_EducationQualification
--------------------------------
  



--------#Region Enquiry Start ----------

insert into IAMT_Migrate_02July2022.dbo.Enquiry
SELECT E.EnquiryId as[RowKey]
      ,1  as AcademicTermKey
      ,E.CourseId as[CourseKey]
      ,UC.UniversityMasterId as[UniversityKey]
      ,[EnquiryName]
      ,[EnquiryAddress]
      ,E.EnquiryEmail as[EmailAddress]
      ,EnquiryPhone as [MobileNumber]
      ,[EnquiryEducationQualification]
      ,1 as[NatureOfEnquiryKey]
      ,BranchId as[BranchKey]
      ,[EnquiryDateOfAdmission]
      ,[EnquiryFeedback]
      ,[DateOfBirth]
       ,case when Gender='M' Then 1 Else 2 End as [Gender]
      --,DepartmentId as[DepartmentKey]
      ,EnquiryStatusId as[EnquiryStatusKey]
      ,E.StaffID as[EmployeeKey]
      ,1as[IsProcessed]
      ,0as[IsDeleted]
      ,''as[DistrictName]
      ,''as[LocationName]
      ,''as[Remarks]
      ,null as[MobileNumberOptional]
      ,null as[PhoneNumber]
      ,E.CreatedOn as[DateAdded]
      ,L.Id as[AddedBy]
      ,null as[DateModified]
      ,null as[ModifiedBy]
      ,91 as[TelephoneCodeKey]
      ,91 as[TelePhoneCodeOptionalkey]
      ,null as[OtherBranchKey]
      ,CountryId as[CountryKey]
	  ,0 as [IsShortlisted]
	  ,null as [CompanyName]
  FROM iamtGlobal_02July2022.dbo.T_Enquiry E
   inner Join iamtGlobal_02July2022.dbo.M_Login L on L.UserId=E.CreatedBy
    inner join iamtGlobal_02July2022.dbo.M_UniversityCourse UC on E.UniversityId=UC.UniversityId 
    where Convert(date,E.CreatedOn) > '2022-01-01' and NatureOfEnquiryId=22
    

	--ALTER TABLE EnquiryLead ADD CONSTRAINT FK_EnquiryLead_Enquiry FOREIGN KEY (EnquiryKey) REFERENCES Enquiry(RowKey);

select * from  IAMT_Migrate_02July2022.dbo.Enquiry	  
select * from  iamtGlobal_02July2022.dbo.T_Enquiry


-----------------------


-------- EnquiryFeedback 

insert into IAMT_Migrate_02July2022.dbo.EnquiryFeedback
SELECT EnquiryFeedbackId as[RowKey]
      ,[EnquiryFeedbackDesc]
      ,EN.EnquiryId as[EnquiryKey]
      ,[EnquiryFeedbackReminderDate]
      ,case When EnquiryFeedbackReminderStatus='Y' Then 1 Else 0 End AS[EnquiryFeedbackReminderStatus]
      ,[EnquiryDuration]
      ,CallTypeId as[CallTypeKey]
      ,[IsCompleted]
      ,EnquiryCallStatusID as[EnquiryCallStatusKey]
      ,null as[CouncellingTime]
      ,E.CreatedOn as[DateAdded]
      ,L.Id as[AddedBy]
      ,null as[DateModified]
      ,null as[ModifiedBy]
  FROM iamtGlobal_02July2022.dbo.T_EnquiryFeedback E
  INNER JOIN iamtGlobal_02July2022.dbo.T_Enquiry EN on EN.EnquiryId=E.EnquiryId
    inner Join iamtGlobal_02July2022.dbo.M_Login L on L.UserId=E.CreatedBy
	 where Convert(date,EN.CreatedOn) > '2022-01-01' 

	select * from  IAMT_Migrate_02July2022.dbo.EnquiryFeedback	  
select * from  iamtGlobal_02July2022.dbo.T_EnquiryFeedback

-----------------------



insert into IAMT_Migrate_02July2022.dbo.EnquiryLead
SELECT E.EnquiryLeadid as[RowKey]
      ,[Name]
      ,Email as[EmailAddress]
      ,[Qualification]
      ,E.BranchId as[BranchKey]
      ,E.Departmentid as[DepartmentKey]
      ,E.StaffId as[EmployeeKey]
      ,[Feedback]
      ,E.Phone as[MobileNumber]
      ,[Remarks]
      ,[IsNewLead]
      ,[LeadDate]
      ,[LeadFrom]
      ,E.LeadStatusId as[EnquiryLeadStatusKey]
      ,E.EnquiryId as[EnquiryKey]
      ,null as[MobileNumberOptional]
      ,null as[PhoneNumber]
      ,E.CreatedOn as[DateAdded]
      ,L.Id as[AddedBy]
      ,null as[DateModified]
      ,null as[ModifiedBy]
      ,51 as[TelephoneCodeKey]
      ,51 as[TelePhoneCodeOptionalkey]
      ,null as[RefLeadID]
      ,'' as[Location]
      ,'' as[District]
	  ,null as [NatureofEnquiryKey]
	  ,null as [LeadApiKey]
	  ,null as [AdsAPIKey]
     
 FROM iamtGlobal_02July2022.dbo.T_EnquiryLead E
 inner Join iamtGlobal_02July2022.dbo.M_Login L on L.UserId=E.CreatedBy
 where Convert(date,E.CreatedOn) > '2022-01-01'

 
 --ALTER TABLE IAMT_Migrate_02July2022.EnquiryLead DROP CONSTRAINT FK_EnquiryLead_Enquiry;

 
select * from  IAMT_Migrate_02July2022.dbo.EnquiryLead	  
select * from  iamtGlobal_02July2022.dbo.T_EnquiryLead

select * from  iamtGlobal_02July2022.dbo.M_EnquiryStatus


------------------------------



-------- EnquiryFeedback 

insert into IAMT_Migrate_02July2022.dbo.EnquiryLeadFeedback
SELECT E.EnquiryLeadFeedbackid as[RowKey]
      ,E.[Feedback]
      ,[CallDuration]
      ,[NextCallSchedule]
      ,E.EnquiryLeadid as[EnquiryLeadKey]
      ,CallTypeId as[CallTypeKey]
      ,EnquiryLeadCallStatusId as [EnquiryLeadCallStatusKey]
      ,case when NextCallScheduleStatus='Y' then 1 else 0 end  as[EnquiryFeedbackReminderStatus]
      ,E.CreatedOn as[DateAdded]
      ,L.Id as[AddedBy]
      ,null as[DateModified]
      ,null as[ModifiedBy]
  FROM iamtGlobal_02July2022.dbo.T_EnquiryLeadFeedback E
   INNER JOIN iamtGlobal_02July2022.dbo.T_EnquiryLead EN on EN.EnquiryLeadId=E.EnquiryLeadId
    inner Join iamtGlobal_02July2022.dbo.M_Login L on L.UserId=E.CreatedBy
	where Convert(date,EN.CreatedOn) > '2022-01-01'

	
select * from  IAMT_Migrate_02July2022.dbo.EnquiryLeadFeedback	  
select * from  iamtGlobal_02July2022.dbo.T_EnquiryLeadFeedback

--------------------------




-------- InternalExam 

insert into IAMT_Migrate_02July2022.dbo.InternalExam
SELECT I.InternalExamId as[RowKey]
      ,CASE WHEN UC.IsSemester=1 THEN 1 ELSE 2 END  as AcademicTermKey
      ,B.CourseId as[CourseKey]
      ,I.UniversityId as[UniversityMasterKey]
      ,I.BatchId as[BatchKey]
      ,[ExamYear]
      ,I.InternalExamTermId as[InternalExamTermKey]
      ,I.BranchId as[BranchKey]
      ,I.CreatedOn as[DateAdded]
      ,L.Id as[AddedBy]
      ,null as[DateModified]
      ,null as[ModifiedBy]
  FROM iamtGlobal_02July2022.dbo.T_InternalExam I
   inner Join iamtGlobal_02July2022.dbo.M_Login L on L.UserId=I.CreatedBy
    inner join iamtGlobal_02July2022.dbo.M_UniversityCourse UC on I.UniversityId=UC.UniversityId 
    inner join iamtGlobal_02July2022.dbo.T_InternalExamDetails IED on I.InternalExamId=IED.InternalExamId 
    inner join iamtGlobal_02July2022.dbo.M_books B on IED.BookId=B.BooksId 
    

-------------------


-------- InternalExam Details 

insert into IAMT_Migrate_02July2022.dbo.InternalExamDetails
SELECT IED.InternalExamDetailsId as[RowKey]
      ,IED.InternalExamId as[InternalExamKey]
      ,IED.BookId as[SubjectKey]
      ,[ExamDate]
      ,[ExamStatus]
      ,[MaximumMark]
      ,[MinimumMark]
      ,[ExamStartTime]
      ,[ExamEndTime]
  FROM iamtGlobal_02July2022.dbo.T_InternalExamDetails IED
 
 
 -----------------
 
 
 
--------#Region InternalExamDivision  Start ----------

insert into IAMT_Migrate_02July2022.dbo.InternalExamDivision
SELECT Row_number()Over(Order by InternalExamId)as[RowKey]
      ,null as[ClassDetailsKey]
      ,InternalExamId as[InternalExamKey]
  FROM iamtGlobal_02July2022.dbo.T_InternalExamDivision
  
  
----------------------
  
  
--------#Region InternalExamResult  Start ----------

insert into IAMT_Migrate_02July2022.dbo.InternalExamResult
SELECT I.InternalExamResultId as[RowKey]
      ,I.InternalExamId as[InternalExamKey]
      ,I.AdmissionNo As[ApplicationKey]
      ,I.BookId as[SubjectKey]
      ,[ResultStatus]
      ,[Mark]
      ,[Remarks]
      ,I.InternalExamDetailsId as[InternalExamDetailsKey]
	  ,I.CreatedOn as[DateAdded]
      ,L.Id as[AddedBy]
      ,null as[DateModified]
      ,null as[ModifiedBy]
      ,null as[ClassDetailsKey]

  FROM iamtGlobal_02July2022.dbo.T_InternalExamResult I
  inner Join iamtGlobal_02July2022.dbo.M_Login L on L.UserId=I.CreatedBy
  
  
-----------------------



---------- IssueOfStudyMaterials    pending

--insert into IAMT_Migrate_02July2022.dbo.IssueOfStudyMaterials
--SELECT I.IssueofStudyMaterialsId as[RowKey]
--      ,I.BooksId as[BookKey]
--      ,I.AddmissionNo as[ApplicationKey]
--      ,CASE WHEN I.IsAvailable='Y' THEN 1 ElSE 0 End as [IsAvailable]
--      ,CASE WHEN I.IsIssued='Y' THEN 1 ElSE 0 End as [IsIssued]
--	  ,ISNull(I.IssuedDate,getdate()) as[DateAdded]
--      ,1 as[AddedBy]
--      ,null as[DateModified]
--      ,null as[ModifiedBy]
--      ,null as[IssuedDate]
--      ,null as[IssuedBy]
--	  ,null as[AvailableDate]
--      ,null as[AvailableBy]
--  FROM iamtGlobal_02July2022.dbo.T_IssueOfStudyMaterials I
----  inner Join iamtGlobal_02July2022.dbo.M_Login L on L.UserId=I.IssuedBy
  
--  select * from  IAMT_Migrate_02July2022.dbo.IssueOfStudyMaterials	  
--select * from  iamtGlobal_02July2022.dbo.T_IssueOfStudyMaterials
 -----------------------
 
 
 
 
-------- Promotion    Pending

insert into IAMT_Migrate_02July2022.dbo.Promotion
SELECT Promotionid as[RowKey]
      ,AdmissionNo as[ApplicationKey]
      ,CurrentDivisionId as[CurrentClassDetailsKey]
      ,[CurrentYear]
      ,PromotedDivisionId as[PromotedClassDetailsKey]
      ,[PromotedYear]
      ,PromotionStatusId[PromotionStatusKey]
      ,CASE WHEN [IsActive]='Y' THEN 1 ElSE 0 End as [IsActive]
      ,P.CreatedOn as[DateAdded]
      ,L.Id as[AddedBy]
      ,null as[DateModified]
      ,null as[ModifiedBy]
  FROM iamtGlobal_02July2022.dbo.T_Promotion P
    inner Join iamtGlobal_02July2022.dbo.M_Login L on L.UserId=P.CreatedBy
    
    
 ---------------------------
   
   
   
-------- StudentDivisionAllocation 

insert into IAMT_Migrate_02July2022.dbo.StudentDivisionAllocation
SELECT StudentDivisionAllocationId as[RowKey]
      ,AdmissionNo as[ApplicationKey]
      ,[RollNumber]
      ,null as[ClassDetailsKey]     
      ,CASE WHEN [IsActive]='Y' THEN 1 ElSE 0 End as [IsActive]
      ,SDA.CreatedOn as[DateAdded]
      ,L.Id as[AddedBy]
      ,null as[DateModified]
      ,null as[ModifiedBy]
      ,null as[ClassDetailsKey]
      ,null as[BatchKey]
      ,null as[RollNoCode]
  FROM iamtGlobal_02July2022.dbo.T_StudentDivisionAllocation SDA
    inner Join iamtGlobal_02July2022.dbo.M_Login L on L.UserId=SDA.CreatedBy
    
    
select * from  IAMT_Migrate_02July2022.dbo.StudentDivisionAllocation	  
select * from  iamtGlobal_02July2022.dbo.T_StudentDivisionAllocation
 ---------------------------
 
 
 
 

-------- StudentDocument   ----------

insert into IAMT_Migrate_02July2022.dbo.StudentDocument
SELECT StudentDocumentId as[RowKey]
      ,AdmissionNo as[ApplicationKey]
      ,[StudentDocumentName]
      ,[StudentDocumentPath]    
      ,1 as [IsActive]
      ,SD.CreatedOn as[DateAdded]
      ,L.Id as[AddedBy]
      ,null as[DateModified]
      ,null as[ModifiedBy]
  FROM iamtGlobal_02July2022.dbo.T_StudentDocument SD
    inner Join iamtGlobal_02July2022.dbo.M_Login L on L.UserId=SD.CreatedBy



 ---------------------------
 
 
 
-------- StudentFeeInstallment  

insert into IAMT_Migrate_02July2022.dbo.StudentFeeInstallment
SELECT FeeInstallmentId as[RowKey]
      ,SFI.AdmissionNo as[ApplicationKey]
      ,[FeeYear]
      ,YEAR(A.StudentDateOfAdmission) as[InstallmentYear]
      ,[InstallmentMonth]
      ,[FeePaymentDay]
      ,[DueDuration]
      ,[InstallmentAmount]
      ,[DueFineAmount]
      ,[AutoSMS]
      ,[AutoEmail]
      ,[AutoNotificationBeforeDue]
      ,[AutoNotificationAfterDue]
      ,null as[InitialPayment]
      ,null as[BalancePayment]
      ,[SuperFineAmount]
      
      ,SFI.CreatedOn as[DateAdded]
      ,L.Id as[AddedBy]
      ,null as[DateModified]
      ,null as[ModifiedBy]
  FROM iamtGlobal_02July2022.dbo.T_StudentFeeInstallment SFI
  Inner Join iamtGlobal_02July2022.dbo.T_Application A on A.AdmissionNo=SFI.AdmissionNo
    inner Join iamtGlobal_02July2022.dbo.M_Login L on L.UserId=SFI.CreatedBy
    
    
    
 ---------------------------
 
 
 

-------- StudentIDCard  

insert into IAMT_Migrate_02July2022.dbo.StudentIDCard
SELECT StudentIDCardId as[RowKey]
      ,SIC.AdmissionNo as[ApplicationKey]
      ,CASE WHEN IsReceived='Y' THEN 1 ElSE 0 End as[IsReceived]
      ,[ReceivedDate]
     ,L.Id as[ReceivedBy]
      ,CASE WHEN IsIssued='Y' THEN 1 ElSE 0 End[IsIssued]
      ,[IssuedDate]
      ,L.Id as[IssuedBy] 
      ,getdate() as[DateAdded]
      ,L.Id as[AddedBy]
      ,null as[DateModified]
      ,null as[ModifiedBy]
  FROM iamtGlobal_02July2022.dbo.T_StudentIDCard SIC
    inner Join iamtGlobal_02July2022.dbo.M_Login L on L.UserId=SIC.ReceivedBy 
  inner join iamtGlobal_02July2022.dbo.T_Application A on A.AdmissionNo=SIC.AdmissionNo
   where Convert(date,A.CreatedOn) > '2022-01-01'

  
select * from  IAMT_Migrate_02July2022.dbo.StudentIDCard	  
select * from  iamtGlobal_02July2022.dbo.T_StudentIDCard
   ---------------------------
   
   
   


-------- StudentPapers 

insert into IAMT_Migrate_02July2022.dbo.StudentPapers
SELECT StudentPaperId as[RowKey]
      ,BookId as[BookKey]
      ,AdmissionNo as[ApplicationKey]       
     ,SP.CreatedOn as[DateAdded]
      ,L.Id as[AddedBy]
      ,null as[DateModified]
      ,null as[ModifiedBy]
      ,1 as[IsActive]  
  FROM iamtGlobal_02July2022.dbo.T_StudentPapers SP
    inner Join iamtGlobal_02July2022.dbo.M_Login L on L.UserId=SP.CreatedBy





-------- UniversityCertificate  

insert into IAMT_Migrate_02July2022.dbo.UniversityCertificate
SELECT UniversityCertificateId as[RowKey]
      ,AdmissionNo as[ApplicationKey]
      ,CertificateTypeId as[CertificateTypeKey]
      ,[UniversityCertificateDescription]
      ,CASE WHEN IsReceived='Y' THEN 1 ElSE 0 End as[IsReceived]
      ,[ReceivedDate]
      ,null as[ReceivedBy]
      ,CASE WHEN IsIssued='Y' THEN 1 ElSE 0 End as[IsIssued]
      ,[IssuedDate]
      ,null as[IssuedBy]
      ,1 as[IsActive]   
     ,UC.CreatedOn as[DateAdded]
      ,L.Id as[AddedBy]
      ,null as[DateModified]
      ,null as[ModifiedBy]
  FROM iamtGlobal_02July2022.dbo.T_UniversityCertificate UC
    inner Join iamtGlobal_02July2022.dbo.M_Login L on L.UserId=UC.CreatedBy


------------------------------




------------------- FeeDetails 


select Count(FeeId)as Total ,ReceiptNo from  iamtGlobal_02July2022.dbo.T_FeeDetails group by ReceiptNo having Count(FeeId)>1





insert into IAMT_Migrate_02July2022.dbo.FeePaymentDetail
SELECT  ROW_number()Over(Order By FeeID) As [RowKey]
      ,Case When FeeTypeId=0 Then 2 Else FeeTypeId end as FeeTypeKey
      ,[FeeAmount]
      ,[FeeYear]  
     
      ,DENSE_RANK()Over( Order By ReceiptNo ) as[FeePaymentMasterKey]
      ,null as[CGSTRate]
      ,null as[SGSTRate]
      ,null as[IGSTRate]
      ,null as[CGSTAmount]
      ,null as[SGSTAmount]
      ,null as[IGSTAmount]
      ,FeeAmount as[TaxableAmount]
      ,null as[CessRate]
      ,null as[CessAmount]
      ,FeeAmount as[TotalAmount]
  FROM iamtGlobal_02July2022.dbo.T_FeeDetails  FD
   inner join iamtGlobal_02July2022.dbo.T_Application A on A.AdmissionNo=FD.AdmissionNo    
   where Convert(date,A.CreatedOn) > '2022-01-01'
  
  
select * from  IAMT_Migrate_02July2022.dbo.FeePaymentDetail	  
select * from  IAMT_Migrate_02July2022.dbo.FeePaymentMaster	  
select * from  iamtGlobal_02July2022.dbo.T_FeeDetails
  


-------- FeeDetails 


---------- Fee Master 

insert into IAMT_Migrate_02July2022.dbo.FeePaymentMaster
select RowKey,FeeDate
      ,ApplicationKey
      ,PaymentModeKey
      ,FeeCashTypeDesc
     
      ,ReceiptNo
      ,FeeDescription
      ,ChequeClearanceDate
      ,ChequeApprovedRejectedDate
      ,ChequeStatusKey
      ,IsChequeProcessCompleted
      ,ChequeRejectedRemarks
      ,SerialNumber
      ,IsActive
      ,ChequeOrDDNumber
      ,BankAccountKey     
      ,DateAdded
      ,AddedBy
      ,DateModified
      ,ModifiedBy
      ,ReferenceNumber
      ,TaxRateTypeKey
      ,CardNumber
      ,PaymentmodeSubKey
      ,PaidBranchKey
	  ,ReceiptNumberConfigurationkey
       from (
SELECT 
	   DENSE_RANK()Over( Order By ReceiptNo ) as[RowKey]
      ,[FeeDate]
      ,FD.AdmissionNo as[ApplicationKey]
      ,Case When CashTypeId=0 Then 1
       Else CashTypeId end as[PaymentModeKey]
      ,[FeeCashTypeDesc]
     
      ,ReceiptNo
      ,[FeeDescription]
      ,[ChequeClearanceDate]
      ,[ChequeApprovedRejectedDate]
      , case when ChequeAction='P' Then 1 else 2 End as[ChequeStatusKey]
      ,[IsChequeProcessCompleted]
      ,[ChequeRejectedRemarks]
      ,ReceiptNo as[SerialNumber]
      ,1 as[IsActive]
      ,null as[ChequeOrDDNumber]
      ,null as[BankAccountKey]      
      ,FD.CreatedOn as[DateAdded]
      ,ISNUll(L.Id,1) as[AddedBy]
      ,null as[DateModified]
      ,null as[ModifiedBy]
      ,null as[ReferenceNumber]
      ,null as[TaxRateTypeKey]
      ,null as[CardNumber]
      ,1 as [PaymentmodeSubKey]
      ,null as [PaidBranchKey]
	  ,1 as [ReceiptNumberConfigurationkey]
      ,ROW_number()Over(Partition by ReceiptNo Order By FeeID) As RowNumber
  FROM iamtGlobal_02July2022.dbo.T_FeeDetails FD
  Left Join iamtGlobal_02July2022.dbo.M_Login L on L.UserId=FD.CreatedBy
   inner join iamtGlobal_02July2022.dbo.T_Application A on A.AdmissionNo=FD.AdmissionNo
	where Convert(date,A.CreatedOn) > '2022-01-01'
   )as a where a.RowNumber=1
  
  
select * from IAMT_Migrate_02July2022.dbo.FeePaymentMaster
select * from iamtGlobal_02July2022.dbo.T_FeeDetails
where FeeDescription like '%siju%'



-----------------------------------




-------- UniversityPayment 

insert into IAMT_Migrate_02July2022.dbo.UniversityPaymentMaster      
 select a.RowKey,
 a.UniversityPaymentChalanDDNumber
 ,a.PaymentModeKey
 ,a.UniversityPaymentDate
 ,a.BankAccountKey
 ,a.UniversityPaymentNote
 ,a.ApplicationKey
 ,a.CenterShareAmount
 ,a.SerialNumber
 ,a.VoucherNo
 ,a.DateAdded
 ,a.AddedBy
 ,a.DateModified
 ,a.ModifiedBy
 ,a.IsActive
 ,a.ReferenceNumber
 ,a.ChequeClearanceDate
 ,a.ChequeApprovedRejectedDate
 ,a.ChequeStatusKey
 ,a.IsChequeProcessCompleted
 ,a.ChequeRejectedRemarks
 ,a.ChequeOrDDNumber
  ,CardNumber
      ,PaymentmodeSubKey
      ,PaidBranchKey
 from(
SELECT DENSE_RANK()Over( Order By VoucherNo ) as[RowKey]
      ,[UniversityPaymentChalanDDNumber]
      ,3 as [PaymentModeKey]
      ,[UniversityPaymentDate]
      ,null as[BankAccountKey]
      ,[UniversityPaymentNote]
      ,UP.AdmissionNo as[ApplicationKey]
      ,[CenterShareAmount]
      ,VoucherNo as[SerialNumber]
      ,cast([VoucherNo] as varchar(10))VoucherNo
       ,UP.CreatedOn as[DateAdded]
    ,ISNUll(L.Id,1) as[AddedBy]
      ,null as[DateModified]
      ,null as[ModifiedBy]
      ,1as[IsActive]
      ,null as[ReferenceNumber]
      ,null as[ChequeClearanceDate]
      ,null as[ChequeApprovedRejectedDate]
      ,null as[ChequeStatusKey]
      ,0 as[IsChequeProcessCompleted]
      ,null as[ChequeRejectedRemarks]
      ,null as[ChequeOrDDNumber]
	  ,null as[CardNumber]
      ,null as [PaymentmodeSubKey]
      ,null as [PaidBranchKey]
      ,ROW_number()Over(Partition by VoucherNo Order By UniversityPaymentId) As RowNumber
  FROM iamtGlobal_02July2022.dbo.T_UniversityPayment UP
  left Join iamtGlobal_02July2022.dbo.M_Login L on L.UserId=UP.CreatedBy
  inner join iamtGlobal_02July2022.dbo.T_Application A on A.AdmissionNo=UP.AdmissionNo
	 
  where  VoucherNo is not null and Convert(date,A.CreatedOn) > '2022-01-01'
  ) as a where a.RowNumber=1 
  
  
  select * from UniversityPaymentMaster
  
--------#EndRegion Fee Master End -------------



insert into IAMT_Migrate_02July2022.dbo.UniversityPaymentDetail
SELECT ROW_number()Over(Order By UniversityPaymentId) As [RowKey]
      ,[UniversityPaymentAmount]
      ,[UniversityPaymentYear]
      ,U.FeeTypeId as[FeeTypeKey]
      ,DENSE_RANK()Over( Order By VoucherNo ) as[UniversiyPaymenMasterKey]
      ,Null as[CGSTRate]
      ,Null as[SGSTRate]
      ,Null as[IGSTRate]
      ,Null as[CGSTAmount]
      ,Null as[SGSTAmount]
      ,Null as[IGSTAmount]
  FROM iamtGlobal_02July2022.dbo.T_UniversityPayment U
  left join iamtGlobal_02July2022.dbo.M_FeeType F on  U.FeeTypeId=F.FeeTypeId
 where  VoucherNo is not null
GO

  select * from UniversityPaymentDetail
  select * from UniversityPaymentMaster


select distinct F.*,U.*   FROM iamtGlobal_02July2022.dbo.T_UniversityPayment U
  left join iamtGlobal_02July2022.dbo.M_FeeType F on  U.FeeTypeId=F.FeeTypeId


---------------------------------------------------------------

---Bank Account

insert into IAMT_Migrate_02July2022.dbo.BankAccount
select 
BNK.BankId as RowKey,	
1 as BranchKey,
BNK.BankId as BankKey,
'Palakkad'as BranchLocation,
BNK.BankIFSC as IFSCCode,
'123456' as AccountNumber,
null as NameInAccount,
1 as AccountTypeKey,
null as MICRCode,
null as OpeningAccountBalance,
null as CurrentAccountBalance,
1 as IsActive,
BNK.CreatedOn as DateAdded	,
1 as AddedBy	,
null as DateModified,
null as ModifiedBy
,26 as AccountheadKey
from iamtGlobal_02July2022.dbo.M_Bank BNK


  select * from  IAMT_Migrate_02July2022.dbo.BankAccount	  
select * from  iamtGlobal_02July2022.dbo.M_Bank

----------------------------------------

-------- Books TO Subject

insert into IAMT_Migrate_02July2022.dbo.Subjects
select
 S.BooksId as RowKey,
 S.BookCode as SubjectCode,
 S.Booksname as SubjectName,
 Case when S.BookType='C' then 0 else 1 end as IsElective,
 1 as StudyMaterialCount,
 1 as HasStudyMaterial,
 0 as IsCommonSubject,
 Case when S.BooksActive='Y' then 1 else 0 end as IsActive,
 S.CreatedOn as DateAdded,
 ISNULL(L.Id,1) as AddedBy,
 null as DateModified,
 null as ModifiedBy

from iamtGlobal_02July2022.dbo.M_Books S
Left Join iamtGlobal_02July2022.dbo.M_Login L on L.UserId=S.CreatedBy

select * from IAMT_Migrate_02July2022.dbo.Subjects

----------------------------------------

-------- Books TO StudyMaterials

insert into IAMT_Migrate_02July2022.dbo.StudyMaterials
select
 S.BooksId as RowKey,
 S.BooksId as SubjectKey,
 S.BookCode as StudyMaterialCode,
 S.Booksname as StudyMaterialName,
 Case when S.BooksActive='Y' then 1 else 0 end as IsActive
from iamtGlobal_02July2022.dbo.M_Books S

-----------------------------------


---- CourseSubjectMaster

insert into IAMT_Migrate_02July2022.dbo.CourseSubjectMaster
select RowKey,CourseKey,UniversityMasterKey,AcademicTermKey,CourseYear,IsActive, DateAdded,AddedBy,DateModified,ModifiedBy from (
select 
 DENSE_RANK()Over( Order By S.CourseId,S.UniversityId,S.BookYear ) as[RowKey],
 S.CourseId as CourseKey,
 UM.UniversityMasterId as UniversityMasterKey,
 2 as AcademicTermKey,
 S.BookYear as CourseYear,
 1 as IsActive,
 S.CreatedOn as DateAdded,
 ISNULL(L.Id,1) as AddedBy,
 null as DateModified,
 null as ModifiedBy,
 ROW_number()Over(Partition by S.CourseId,S.UniversityId,S.BookYear Order By BooksId) As RowNumber
from iamtGlobal_02July2022.dbo.M_Books S
 INNER JOIN  iamtGlobal_02July2022.dbo.M_UniversityCourse UC on S.UniversityId=UC.UniversityId
 INNER JOIN iamtGlobal_02July2022.dbo.M_UniversityMaster UM on UC.UniversityMasterId=UM.UniversityMasterId
 LEFT JOIN iamtGlobal_02July2022.dbo.M_Login L on L.UserId=S.CreatedBy)as a where a.RowNumber=1

-----------------------------------


---- CourseSubjectDetails

insert into IAMT_Migrate_02July2022.dbo.CourseSubjectDetails
select 
 S.BooksId as RowKey,
 DENSE_RANK()Over( Order By S.CourseId,S.UniversityId,S.BookYear ) as CourseSubjectMasterKey,
 S.BooksId as SubjectKey,
 1 as IsActive
from iamtGlobal_02July2022.dbo.M_Books S
 INNER JOIN  iamtGlobal_02July2022.dbo.M_UniversityCourse UC on S.UniversityId=UC.UniversityId
 INNER JOIN iamtGlobal_02July2022.dbo.M_UniversityMaster UM on UC.UniversityMasterId=UM.UniversityMasterId
 LEFT JOIN iamtGlobal_02July2022.dbo.M_Login L on L.UserId=S.CreatedBy

-----------------------------------




---- IssueOfStudyMaterials

insert into IAMT_Migrate_02July2022.dbo.IssueOfStudyMaterials
select 
 I.IssueOfStudyMaterialsId as RowKey, 
 I.BooksId as StudyMaterialKey,
 I.AddmissionNo as ApplicationKey,
 case when I.IsAvailable='Y' then 1 else 0 end   as IsAvailable,
 case when I.IsIssued='Y' then 1 else 0 end   as IsIssued,
 I.AvalableDate as DateAdded,
 1 as AddedBy,
 I.IssuedDate as DateModified,
 case when I.IsIssued='Y' then 1 else null end   as ModifiedBy,
 I.IssuedDate as IssuedDate,
 case when I.IsIssued='Y' then 1 else null end  as IssuedBy,
 I.AvalableDate as AvalableDate,
 case when I.IsAvailable='Y' then 1 else null end  as AvailableBy
 from iamtGlobal_02July2022.dbo.T_IssueOfStudyMaterials I

  
  

  
insert into IAMT_Migrate_02July2022.dbo.cashflow
select
ATR.AccountTransactionId as [RowKey]
,Case when ATR.AccountTypeId=1 then 21
when ATR.AccountTypeId=3 then 60
when ATR.AccountTypeId=4 then 61
when ATR.AccountTypeId=5 then 62
when ATR.AccountTypeId=6 then 63
when ATR.AccountTypeId=7 then 64
when ATR.AccountTypeId=8 then 65
when ATR.AccountTypeId=9 then 66
when ATR.AccountTypeId=10 then 67
when ATR.AccountTypeId=11 then 68
when ATR.AccountTypeId=12 then 69
when ATR.AccountTypeId=13 then 70
when ATR.AccountTypeId=14 then 96
when ATR.AccountTypeId=15 then 71
when ATR.AccountTypeId=16 then 72
when ATR.AccountTypeId=17 then 73
when ATR.AccountTypeId=18 then 74
when ATR.AccountTypeId=20 then 75
when ATR.AccountTypeId=22 then 76
when ATR.AccountTypeId=23 then 77
when ATR.AccountTypeId=24 then 78
when ATR.AccountTypeId=25 then 79
when ATR.AccountTypeId=26 then 97
when ATR.AccountTypeId=27 then 80
when ATR.AccountTypeId=28 then 81
when ATR.AccountTypeId=30 then 82
when ATR.AccountTypeId=32 then 83
when ATR.AccountTypeId=33 then 84
when ATR.AccountTypeId=34 then 53
when ATR.AccountTypeId=35 then 85
when ATR.AccountTypeId=37 then 86
when ATR.AccountTypeId=38 then 87
when ATR.AccountTypeId=39 then 98
when ATR.AccountTypeId=41 then 88
when ATR.AccountTypeId=43 then 89
when ATR.AccountTypeId=44 then 90
when ATR.AccountTypeId=45 then 91
when ATR.AccountTypeId=46 then 92
when ATR.AccountTypeId=47 then 93
when ATR.AccountTypeId=49 then 94
when ATR.AccountTypeId=50 then 95
else ATR.AccountTypeId end as
	  [AccountHeadKey]
      ,Null as [ChequeStatusKey]
      ,ATR.AccountTransactionDate as [CashFlowDate]
      ,case when ATR.AccountTransactionType='P' then 2 else 1 end   as [CashFlowTypeKey]
      ,null as[VoucherNumber]
      ,AccountTransactionAmount as [Amount]
      ,1 as [PaymentModeKey]
      ,null as[CardNumber]
      ,null as[BankAccountKey]
      ,null as[ChequeOrDDNumber]
      ,null as[ChequeOrDDDate]
      ,null as[AccountBalanceAmount]
      ,4 as[TransactionTypeKey]
      ,ATR.AccountTransactionId as[TransactionKey]
      ,AccountTransactionPurpose as[Purpose]
      ,AccountTransactionPaid as[PaidBy]
      ,AccountTransactionAuthorizedBy as[AuthorizedBy]
      ,AccountTransactionReceivedBy as[ReceivedBy]
      ,'' as[OnBehalfOf]
      ,AccountTransactionDescription as[Remarks]
      ,ATR.branchId as [BranchKey]
      ,ATR.CreatedOn as[DateAdded]
      ,L.Id as[AddedBy]
      ,null as[DateModified]
      ,null as[ModifiedBy]
      ,null as[ReferenceNumber]
      ,null as[BalanceAmount]
      ,0 as[IsContra]
      ,AccountTransactionNo as [SerialNumber]
      ,AccountTransactionNo as[ReceiptNumber]
      ,null as[PaymentModeSubKey] 

from iamtGlobal_02July2022.dbo.T_AccountTransaction ATR
Left Join iamtGlobal_02July2022.dbo.M_Login L on L.UserId=ATR.CreatedBy
where Convert(date,ATR.CreatedOn) > '2022-01-01'