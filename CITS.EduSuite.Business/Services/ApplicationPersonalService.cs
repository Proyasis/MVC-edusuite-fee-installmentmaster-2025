using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System.Configuration;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class ApplicationPersonalService : IApplicationPersonalService
    {
        private EduSuiteDatabase dbContext;

        public ApplicationPersonalService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public ApplicationPersonalViewModel GetApplicationPersonalById(ApplicationPersonalViewModel model)
        {
            ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();
            try
            {
                objViewModel = dbContext.Applications.Where(x => x.RowKey == model.RowKey).Select(row => new ApplicationPersonalViewModel
                {
                    RowKey = row.RowKey,
                    AdmissionNo = row.AdmissionNo,
                    AcademicTermKey = row.AcademicTermKey,
                    CourseTypeKey = row.Course.CourseTypeKey,
                    CourseKey = row.CourseKey,
                    UniversityKey = row.UniversityMasterKey,
                    ApplicantName = row.StudentName,
                    ApplicantGuardianName = row.StudentGuardian,
                    ApplicantMotherName = row.StudentMotherName,
                    PermenantAddress = row.StudentPermanentAddress,
                    PresentAddress = row.StudentPresentAddress,
                    StudentPhone = row.StudentPhone,
                    MobileNumber = row.StudentMobile,
                    StudentEmail = row.StudentEmail,
                    DateOfBirth = row.StudentDOB,
                    Gender = row.StudentGender,
                    ReligionKey = row.ReligionKey,
                    BatchKey = row.BatchKey,
                    NatureOfEnquiryKey = row.NatureOfEnquiryKey,
                    DateOfApplication = row.StudentDateOfAdmission,
                    BranchKey = row.BranchKey,
                    AgentKey = row.AgentKey,
                    TotalFeeAmount = row.StudentTotalFee,
                    //TotalFeeAmountRupee = row.TotalFeeAmountRupee,
                    Remarks = row.Remarks,
                    ModeKey = row.ModeKey,
                    StudentClassRequired = row.StudentClassRequired,
                    ClassDetailsKey = row.ClassDetailsKey ?? 0,
                    RollNumber = row.RollNumber,
                    ClassModeKey = row.ClassModeKey,
                    StartYear = row.StartYear,
                    Mediumkey = row.Mediumkey,
                    SecondLanguageKey = row.SecondLanguageKey,
                    IncomeKey = row.IncomeKey,
                    HasOffer = row.HasOffer,
                    OfferKey = row.OfferKey,
                    OfferName = row.Offer.OfferName,
                    OfferValue = row.Offer.OfferValue,
                    AdmissionCurrentYear = row.AdmissionCurrentYear,
                    CurrentYear = row.CurrentYear,
                    HasConcession = row.HasConcession,
                    HasInstallment = row.HasInstallment,
                    //CasteKey = row.CasteKey,
                    BloodGroupKey = row.BloodGroupKey,
                    ApplicationNo = row.ApplicationNo,
                    IsTax = row.IsTax ?? false,
                    AllowLogin = row.AllowLogin,
                    EnquiryKey = row.EnquiryKey,
                    CommunityTypeKey = row.CommunityTypeKey,
                    AllowOldPaid = row.AllowOldPaid ?? false,
                    EducationTypeKey = row.EducationTypeKey ?? 0,
                    RegistrationCatagoryKey = row.RegistrationCatagoryKey ?? 0,
                    AllowTransportation = row.AllowTransportation,
                    TransportLocation = row.TransportLocation,
                    PassPortNumber = row.PassPortNumber,
                    EmiratesId = row.EmiratesId,
                    GuardianMobile = row.GuardianMobile,
                    EmployeeKey = row.EmployeeKey,
                    HasBioMetric = row.HasBioMetric,
                    BioMetricsId = row.BioMetricsId,
                    CertificateKeys = dbContext.ApplicationCertificates.Where(x => x.ApplicationKey == row.RowKey).Select(y => y.CertificateTypeKey).ToList(),

                    HasElective = dbContext.Subjects.Any(S => S.CourseSubjectDetails.Select(y => y.CourseSubjectMaster.CourseKey).FirstOrDefault() == row.CourseKey &&
                                                                            S.CourseSubjectDetails.Select(y => y.CourseSubjectMaster.UniversityMasterKey).FirstOrDefault() == row.UniversityMasterKey &&
                                                                             S.CourseSubjectDetails.Select(y => y.CourseSubjectMaster.AcademicTermKey).FirstOrDefault() == row.AcademicTermKey && S.IsElective == true)

                }).FirstOrDefault();
                if (objViewModel == null)
                {
                    objViewModel = new ApplicationPersonalViewModel();

                    if (model.ApplicationWebFormKey != 0)
                    {
                        objViewModel = dbContext.ApplicationWebForms.Where(x => x.RowKey == model.ApplicationWebFormKey).Select(row => new ApplicationPersonalViewModel
                        {
                            ApplicationWebFormKey = row.RowKey,
                            AcademicTermKey = row.AcademicTermKey,
                            CourseTypeKey = row.Course.CourseTypeKey,
                            CourseKey = row.CourseKey,
                            UniversityKey = row.UniversityMasterKey,
                            ApplicantName = row.StudentName,
                            ApplicantGuardianName = row.StudentGuardian,
                            PermenantAddress = row.StudentPermanentAddress,
                            MobileNumber = row.StudentMobile,
                            StudentEmail = row.StudentEmail,
                            DateOfBirth = row.StudentDOB,
                            Gender = row.StudentGender ?? 0,
                            ReligionKey = row.ReligionKey,
                            NatureOfEnquiryKey = row.NatureOfEnquiryKey,
                            BranchKey = row.BranchKey,
                            AgentKey = row.AgentKey,
                            //TotalFeeAmountRupee = row.TotalFeeAmountRupee,
                            Remarks = row.Remarks,
                            SecondLanguageKey = row.SecondLanguageKey,
                            EnquiryKey = row.EnquiryKey,
                            CommunityTypeKey = row.CommunityTypeKey,
                            GuardianMobile = row.GuardianMobile,
                        }).FirstOrDefault();

                        objViewModel.EnquiryKey = dbContext.Enquiries.Where(x => x.MobileNumber == objViewModel.MobileNumber).Select(y => y.RowKey).FirstOrDefault();
                    }
                }

                FillDropdownLists(objViewModel);
                FillNotificationDetail(objViewModel);
                return objViewModel;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.ApplicationPersenol, ActionConstants.View, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                return new ApplicationPersonalViewModel();
            }
        }
        public ApplicationPersonalViewModel CreateApplicationPersonal(ApplicationPersonalViewModel model)
        {
            Application applicationModel = new Application();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Int64 maxKey = dbContext.Applications.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    applicationModel.RowKey = Convert.ToInt64(maxKey + 1);

                    applicationModel.AdmissionNo = dbContext.Database.SqlQuery<string>("select dbo.F_GenerateAdmissionNo(" + model.BranchKey + "," + model.BatchKey + "," + model.UniversityKey + ")").Single().ToString();
                    applicationModel.SerialNumber = dbContext.Database.SqlQuery<int>("select dbo.F_GenerateSerialNo(" + model.BranchKey + "," + model.BatchKey + "," + model.UniversityKey + ")").Single();

                    applicationModel.CourseKey = model.CourseKey;
                    applicationModel.UniversityMasterKey = model.UniversityKey;
                    applicationModel.StudentName = model.ApplicantName;
                    applicationModel.StudentGuardian = model.ApplicantGuardianName;
                    applicationModel.StudentMotherName = model.ApplicantMotherName;
                    applicationModel.StudentPermanentAddress = model.PermenantAddress;
                    applicationModel.StudentPresentAddress = model.PresentAddress;
                    applicationModel.BranchKey = model.BranchKey ?? 0;
                    applicationModel.EmployeeKey = model.EmployeeKey;
                    //applicationModel.TelephoneCodeKey = model.TelephoneCodeKey;
                    applicationModel.StudentPhone = model.StudentPhone;
                    applicationModel.StudentMobile = model.MobileNumber;
                    applicationModel.StudentEmail = model.StudentEmail;
                    applicationModel.StudentGender = model.Gender;
                    applicationModel.StudentDOB = Convert.ToDateTime(model.DateOfBirth);
                    applicationModel.ReligionKey = model.ReligionKey;
                    applicationModel.BatchKey = model.BatchKey;
                    applicationModel.NatureOfEnquiryKey = model.NatureOfEnquiryKey;
                    applicationModel.StudentDateOfAdmission = Convert.ToDateTime(model.DateOfApplication);
                    applicationModel.BranchKey = model.BranchKey ?? 0;
                    applicationModel.AgentKey = model.AgentKey;
                    applicationModel.StudentTotalFee = Convert.ToDecimal(model.GrandTotalFeeAmount);
                    applicationModel.EnquiryKey = model.EnquiryKey;
                    if (model.EnquiryKey != null && model.EnquiryKey != 0)
                    {
                        applicationModel.IsFromEnquiry = "Y";
                        applicationModel.EnquiryKey = model.EnquiryKey;
                    }
                    else
                    {
                        applicationModel.IsFromEnquiry = "N";
                        applicationModel.EnquiryKey = null;
                    }
                    applicationModel.Remarks = model.Remarks;
                    applicationModel.StudentStatusKey = DbConstants.StudentStatus.Ongoing;
                    applicationModel.Mediumkey = model.Mediumkey;
                    applicationModel.IncomeKey = model.IncomeKey;
                    applicationModel.SecondLanguageKey = model.SecondLanguageKey;
                    applicationModel.ModeKey = model.ModeKey;
                    applicationModel.StartYear = model.StartYear;
                    if (DbConstants.GeneralConfiguration.AllowAdmissionToAccoount)
                    {
                        applicationModel.CurrentYear = Convert.ToInt16(model.AdmissionCurrentYear);
                        applicationModel.AdmissionCurrentYear = Convert.ToInt16(model.AdmissionCurrentYear);
                    }
                    else
                    {
                        applicationModel.CurrentYear = Convert.ToInt16(model.StartYear);
                        applicationModel.AdmissionCurrentYear = Convert.ToInt16(model.StartYear);
                    }
                    applicationModel.StudentClassRequired = model.StudentClassRequired;
                    if (model.StudentClassRequired == true && model.ClassDetailsKey != null)
                    {
                        int RollNumber = 0;

                        RollNumber = dbContext.StudentDivisionAllocations.Where(row => row.Application.BatchKey == model.BatchKey && row.Application.CurrentYear == model.AdmissionCurrentYear &&
                                          row.Application.BranchKey == model.BranchKey && row.Application.StudentStatusKey == DbConstants.ApplicationStatus.OnGoing && row.ClassDetailsKey == model.ClassDetailsKey)
                                          .Select(p => p.RollNumber).DefaultIfEmpty().Max();

                        applicationModel.ClassDetailsKey = model.ClassDetailsKey;
                        applicationModel.RollNumber = RollNumber + 1;
                        applicationModel.RollNoCode = dbContext.Database.SqlQuery<string>("select dbo.F_GenerateRollNoCode(" + model.ClassDetailsKey + "," + RollNumber + ")").Single().ToString();
                    }
                    applicationModel.ClassModeKey = model.ClassModeKey;
                    applicationModel.HasConcession = model.HasConcession;
                    applicationModel.HasInstallment = model.HasInstallment;
                    applicationModel.HasOffer = model.HasOffer;
                    applicationModel.AcademicTermKey = model.AcademicTermKey ?? 0;
                    applicationModel.OfferKey = model.HasOffer == true ? model.OfferKey : null;
                    applicationModel.CasteKey = model.CasteKey;
                    applicationModel.BloodGroupKey = model.BloodGroupKey;
                    applicationModel.ApplicationNo = model.ApplicationNo;
                    applicationModel.IsTax = model.IsTax;
                    applicationModel.AllowLogin = model.AllowLogin;
                    applicationModel.AllowOldPaid = model.AllowOldPaid;
                    applicationModel.CommunityTypeKey = model.CommunityTypeKey;
                    applicationModel.EducationTypeKey = model.EducationTypeKey;
                    applicationModel.RegistrationCatagoryKey = model.RegistrationCatagoryKey;
                    applicationModel.AllowTransportation = model.AllowTransportation;
                    applicationModel.TransportLocation = model.TransportLocation;
                    applicationModel.PassPortNumber = model.PassPortNumber;
                    applicationModel.EmiratesId = model.EmiratesId;
                    applicationModel.GuardianMobile = model.GuardianMobile;
                    applicationModel.HasBioMetric = model.HasBioMetric;
                    applicationModel.BioMetricsId = model.BioMetricsId;
                    if (model.BioMetricsId != null && model.BioMetricsId != 0)
                    {
                        EsslStudent dbEsslStudents= dbContext.EsslStudents.FirstOrDefault(row => row.RowKey == model.BioMetricsId);
                        if(dbEsslStudents!=null)
                        {
                            dbEsslStudents.IsConnected = true;
                        }
                    }
                    if (model.ApplicationWebFormKey != 0)
                    {
                        ApplicationWebForm applicationwebform = dbContext.ApplicationWebForms.SingleOrDefault(row => row.RowKey == model.ApplicationWebFormKey);
                        if (applicationwebform != null)
                        {
                            applicationwebform.ApplicationKey = applicationModel.RowKey;
                            applicationwebform.ConvertedToApplication = true;
                        }
                    }
                    if (model.AllowLogin)
                    {
                        applicationModel.AppUserKey = CreateStudentUserAccount(applicationModel);

                    }
                    if (model.EnquiryKey != null && model.EnquiryKey != 0)
                    {
                        applicationModel.EnquiryKey = model.EnquiryKey;
                        Enquiry enquiry = dbContext.Enquiries.SingleOrDefault(row => row.RowKey == model.EnquiryKey);
                        if (enquiry != null)
                        {
                            List<EnquiryFeedback> enquiryFeedbackList = dbContext.EnquiryFeedbacks.Where(row => row.EnquiryKey == enquiry.RowKey).ToList();
                            enquiryFeedbackList.ForEach(enquiryFeedback => enquiryFeedback.EnquiryFeedbackReminderStatus = false);
                            enquiry.EnquiryStatusKey = DbConstants.EnquiryStatus.AdmissionTaken;
                        }
                    }

                    dbContext.Applications.Add(applicationModel);
                    CreateAdmissionFee(model.AdmissionFees.Where(row => row.RowKey == 0).ToList(), applicationModel);

                    List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();

                    if (DbConstants.GeneralConfiguration.AllowAdmissionToAccoount)
                    {
                        if (DbConstants.GeneralConfiguration.AllowSplitCostOfService)
                        {
                            IncomeSplitAmountList(model.AdmissionFees.Where(x => x.AdmissionFeeYear == null || x.AdmissionFeeYear <= applicationModel.CurrentYear).ToList(), accountFlowModelList, false, applicationModel);
                        }
                        else
                        {
                            decimal TotalIncome = model.AdmissionFees.Where(x => (x.AdmissionFeeYear == null || x.AdmissionFeeYear <= applicationModel.CurrentYear)).Select(row => ((row.IsUniversity ? (row.CenterShareAmountPer ?? 0) : 100) * ((row.ActualAmount ?? 0) - (row.OldPaid ?? 0))) / 100).Sum();
                            IncomeAmountList(TotalIncome, accountFlowModelList, false, applicationModel);
                        }

                        decimal TotalReceivable = 0;
                        decimal TotalPayable = model.AdmissionFees.Where(x => ((x.AdmissionFeeYear == null || x.AdmissionFeeYear <= applicationModel.CurrentYear) && x.IsUniversity == true)).Select(row => ((row.ActualAmount ?? 0) - (row.OldPaid ?? 0)) - ((row.IsUniversity ? (row.CenterShareAmountPer ?? 0) : 100) * ((row.ActualAmount ?? 0) - (row.OldPaid ?? 0))) / 100).Sum();
                        decimal TotalConcessionExpense = model.AdmissionFees.Where(x => (x.AdmissionFeeYear == null || x.AdmissionFeeYear <= applicationModel.CurrentYear)).Select(row => (row.ConcessionAmount ?? 0)).Sum();
                        //decimal TotalSGSTAmount = model.AdmissionFees.Where(x => (x.AdmissionFeeYear == null || x.AdmissionFeeYear <= applicationModel.StartYear)).Select(row => (row.SGSTRate ?? 0)).Sum();
                        //decimal TotalCGSTAmount = model.AdmissionFees.Where(x => (x.AdmissionFeeYear == null || x.AdmissionFeeYear <= applicationModel.StartYear)).Select(row => (row.CGSTRate ?? 0)).Sum();
                        TotalReceivable = model.AdmissionFees.Where(x => (x.AdmissionFeeYear == null || x.AdmissionFeeYear <= applicationModel.CurrentYear)).Select(row => ((row.ActualAmount ?? 0) - (row.OldPaid ?? 0))).Sum();

                        RecievableAmountList(TotalReceivable, accountFlowModelList, false, applicationModel);
                        PayableAmountList(TotalPayable, accountFlowModelList, false, applicationModel);
                        //CGSTAmountList(TotalCGSTAmount, accountFlowModelList, false, applicationModel);
                        //SGSTAmountList(TotalSGSTAmount, accountFlowModelList, false, applicationModel);
                        ConcessionAmountList(TotalConcessionExpense, accountFlowModelList, false, applicationModel);
                        CreateAccountFlow(accountFlowModelList, false);
                    }

                    if (model.StudentClassRequired == true && model.ClassDetailsKey != null)
                    {
                        CreateDivisionAllocation(applicationModel.RowKey, applicationModel.RollNumber, applicationModel.ClassDetailsKey, applicationModel.RollNoCode, applicationModel.BatchKey);
                    }
                    UpdateApplicationCertificates(model, applicationModel.RowKey);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.RowKey = applicationModel.RowKey;
                    model.AdmissionNo = applicationModel.AdmissionNo;
                    model.HasInstallment = applicationModel.HasInstallment;
                    model.HasElective = dbContext.Books.Any(row => row.CourseKey == applicationModel.CourseKey && row.UniversityKey == applicationModel.UniversityMasterKey && row.AcademicTermKey == applicationModel.AcademicTermKey && row.BookType == DbConstants.BookType.Elective);
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;


                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationPersenol, ActionConstants.Add, DbConstants.LogType.Info, model.AdmissionNo, model.Message);
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    Exception raise = dbEx;
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = string.Format("{0}:{1}", validationErrors.Entry.Entity.ToString(), validationError.ErrorMessage);
                            //raise a new exception inserting the current one as the InnerException
                            raise = new InvalidOperationException(message, raise);
                        }
                    }
                    throw raise;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Application);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationPersenol, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            FillDropdownLists(model);
            return model;
        }
        public ApplicationPersonalViewModel UpdateApplicationPersonal(ApplicationPersonalViewModel model)
        {
            Application applicationModel = new Application();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    AdmissionNoConfig GC = dbContext.AdmissionNoConfigs.FirstOrDefault();
                    applicationModel = dbContext.Applications.SingleOrDefault(row => row.RowKey == model.RowKey);

                    short? OldBatchKey = applicationModel.BatchKey;
                    short? OldBranchkey = applicationModel.BranchKey;
                    GeneralConfiguration GNC = dbContext.GeneralConfigurations.FirstOrDefault();
                    if (GNC.AllowChangeAdmissionNo)
                    {
                        if (applicationModel.BatchKey != model.BatchKey || applicationModel.BranchKey != model.BranchKey)
                        {
                            if (GC.IsBatch || GC.IsBranch)
                            {
                                applicationModel.AdmissionNo = dbContext.Database.SqlQuery<string>("select dbo.F_GenerateAdmissionNo(" + model.BranchKey + "," + model.BatchKey + "," + model.UniversityKey + ")").Single().ToString();
                                applicationModel.SerialNumber = dbContext.Database.SqlQuery<int>("select dbo.F_GenerateSerialNo(" + model.BranchKey + "," + model.BatchKey + "," + model.UniversityKey + ")").Single();
                            }
                        }
                    }
                    applicationModel.CourseKey = model.CourseKey;
                    applicationModel.UniversityMasterKey = model.UniversityKey;
                    applicationModel.StudentName = model.ApplicantName;
                    applicationModel.StudentGuardian = model.ApplicantGuardianName;
                    applicationModel.StudentMotherName = model.ApplicantMotherName;
                    applicationModel.StudentPermanentAddress = model.PermenantAddress;
                    applicationModel.StudentPresentAddress = model.PresentAddress;
                    applicationModel.BranchKey = model.BranchKey ?? 0;
                    applicationModel.EmployeeKey = model.EmployeeKey;
                    //applicationModel.TelephoneCodeKey = model.TelephoneCodeKey;
                    applicationModel.StudentPhone = model.StudentPhone;
                    applicationModel.StudentMobile = model.MobileNumber;
                    applicationModel.StudentEmail = model.StudentEmail;
                    applicationModel.StudentGender = model.Gender;
                    applicationModel.StudentDOB = Convert.ToDateTime(model.DateOfBirth);
                    applicationModel.ReligionKey = model.ReligionKey;
                    applicationModel.BatchKey = model.BatchKey;

                    applicationModel.NatureOfEnquiryKey = model.NatureOfEnquiryKey;
                    applicationModel.StudentDateOfAdmission = Convert.ToDateTime(model.DateOfApplication);
                    applicationModel.BranchKey = model.BranchKey ?? 0;
                    applicationModel.AgentKey = model.AgentKey;
                    applicationModel.StudentTotalFee = Convert.ToDecimal(model.GrandTotalFeeAmount);

                    // applicationModel.EnquiryKey = model.EnquiryKey;
                    applicationModel.Remarks = model.Remarks;
                    applicationModel.Mediumkey = model.Mediumkey;
                    applicationModel.IncomeKey = model.IncomeKey;
                    applicationModel.SecondLanguageKey = model.SecondLanguageKey;
                    applicationModel.ModeKey = model.ModeKey;
                    applicationModel.StartYear = model.StartYear;
                    applicationModel.ClassModeKey = model.ClassModeKey;
                    applicationModel.HasConcession = model.HasConcession;
                    applicationModel.HasInstallment = model.HasInstallment;
                    applicationModel.AcademicTermKey = model.AcademicTermKey ?? 0;
                    applicationModel.HasOffer = model.HasOffer;
                    applicationModel.OfferKey = model.HasOffer == true ? model.OfferKey : null;
                    applicationModel.CasteKey = model.CasteKey;
                    applicationModel.BloodGroupKey = model.BloodGroupKey;
                    applicationModel.ApplicationNo = model.ApplicationNo;
                    //applicationModel.CurrentYear = Convert.ToInt16(model.CurrentYear);
                    //applicationModel.AdmissionCurrentYear = Convert.ToInt16(model.AdmissionCurrentYear);
                    applicationModel.IsTax = model.IsTax;
                    applicationModel.AllowLogin = model.AllowLogin;
                    applicationModel.AllowOldPaid = model.AllowOldPaid;
                    applicationModel.CommunityTypeKey = model.CommunityTypeKey;
                    applicationModel.EducationTypeKey = model.EducationTypeKey;
                    applicationModel.RegistrationCatagoryKey = model.RegistrationCatagoryKey;
                    applicationModel.AllowTransportation = model.AllowTransportation;
                    applicationModel.TransportLocation = model.TransportLocation;
                    applicationModel.PassPortNumber = model.PassPortNumber;
                    applicationModel.EmiratesId = model.EmiratesId;
                    applicationModel.GuardianMobile = model.GuardianMobile;
                    applicationModel.HasBioMetric = model.HasBioMetric;
                    applicationModel.BioMetricsId = model.BioMetricsId;
                    if (model.BioMetricsId != null && model.BioMetricsId != 0)
                    {
                        EsslStudent dbEsslStudents = dbContext.EsslStudents.FirstOrDefault(row => row.RowKey == model.BioMetricsId);
                        if (dbEsslStudents != null)
                        {
                            dbEsslStudents.IsConnected = true;
                        }
                    }
                    AppUser appUser = dbContext.AppUsers.FirstOrDefault(row => row.RowKey == applicationModel.AppUserKey);
                    if (model.AllowLogin)
                    {
                        if (appUser != null)
                        {
                            appUser.AppUserName = applicationModel.AdmissionNo;
                            appUser.FirstName = applicationModel.StudentName;
                            appUser.Phone1 = applicationModel.StudentMobile;
                            appUser.EmailAddress = applicationModel.StudentEmail ?? "";
                            appUser.Image = applicationModel.StudentPhotoPath;
                            appUser.RoleKey = DbConstants.Role.Students;
                            appUser.IsActive = true;
                            appUser.ApplicationKey = model.RowKey;
                        }
                        else
                        {
                            applicationModel.AppUserKey = CreateStudentUserAccount(applicationModel);
                        }
                    }
                    else
                    {
                        if (appUser != null)
                        {
                            appUser.IsActive = model.AllowLogin;
                        }
                    }

                    applicationModel.StudentClassRequired = model.StudentClassRequired;

                    if (applicationModel.StudentClassRequired && applicationModel.ClassDetailsKey != null)
                    {
                        if (applicationModel.ClassDetailsKey != model.ClassDetailsKey)
                        {
                            StudentDivisionAllocation studentDivisionAllocation = dbContext.StudentDivisionAllocations.SingleOrDefault(p => p.ClassDetailsKey == applicationModel.ClassDetailsKey && p.ApplicationKey == applicationModel.RowKey && p.IsActive);

                            if (studentDivisionAllocation != null)
                            {
                                StudentDivisionAllocation studentDivisionAllocationmodel = dbContext.StudentDivisionAllocations.SingleOrDefault(p => p.RowKey == studentDivisionAllocation.RowKey);
                                //studentDivisionAllocationmodel.IsActive = false;
                                dbContext.StudentDivisionAllocations.Remove(studentDivisionAllocationmodel);
                                model.RollNumber = null;
                                applicationModel.ClassDetailsKey = null;
                                applicationModel.RollNumber = null;
                                applicationModel.RollNoCode = null;
                            }
                        }
                    }

                    if (model.StudentClassRequired == true && model.ClassDetailsKey != null && (model.RollNumber == 0 || model.RollNumber == null))
                    {
                        int RollNumber = 0;

                        RollNumber = dbContext.StudentDivisionAllocations.Where(row => row.Application.BatchKey == model.BatchKey && row.Application.CurrentYear == applicationModel.CurrentYear &&
                                          row.Application.BranchKey == model.BranchKey && row.Application.StudentStatusKey == DbConstants.ApplicationStatus.OnGoing && row.ClassDetailsKey == model.ClassDetailsKey && row.IsActive)
                                          .Select(p => p.RollNumber).DefaultIfEmpty().Max();

                        applicationModel.ClassDetailsKey = model.ClassDetailsKey;
                        applicationModel.RollNumber = RollNumber + 1;
                        applicationModel.RollNoCode = dbContext.Database.SqlQuery<string>("select dbo.F_GenerateRollNoCode(" + model.ClassDetailsKey + "," + RollNumber + ")").Single().ToString();
                        CreateDivisionAllocation(applicationModel.RowKey, applicationModel.RollNumber, applicationModel.ClassDetailsKey, applicationModel.RollNoCode, applicationModel.BatchKey);
                    }
                    if (model.ClassDetailsKey != null && model.RollNumber != null)
                    {
                        //UpdateDivisionAllocation(applicationModel.RowKey, applicationModel.ClassDetailsKey, model);
                    }
                    CreateAdmissionFee(model.AdmissionFees.Where(row => row.RowKey == 0).ToList(), applicationModel);
                    UpdateAdmissionFee(model.AdmissionFees.Where(row => row.RowKey != 0).ToList(), model.RowKey);

                    List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();

                    if (DbConstants.GeneralConfiguration.AllowAdmissionToAccoount)
                    {
                        if (DbConstants.GeneralConfiguration.AllowSplitCostOfService)
                        {
                            IncomeSplitAmountList(model.AdmissionFees.Where(x => x.AdmissionFeeYear == null || x.AdmissionFeeYear <= applicationModel.CurrentYear).ToList(), accountFlowModelList, true, applicationModel);
                        }
                        else
                        {
                            decimal TotalIncome = model.AdmissionFees.Where(x => (x.AdmissionFeeYear == null || x.AdmissionFeeYear <= applicationModel.CurrentYear)).Select(row => ((row.IsUniversity ? (row.CenterShareAmountPer ?? 0) : 100) * ((row.ActualAmount ?? 0) - (row.OldPaid ?? 0))) / 100).Sum();
                            IncomeAmountList(TotalIncome, accountFlowModelList, true, applicationModel);
                        }

                        decimal TotalReceivable = 0;
                        decimal TotalPayable = model.AdmissionFees.Where(x => ((x.AdmissionFeeYear == null || x.AdmissionFeeYear <= applicationModel.CurrentYear) && x.IsUniversity == true)).Select(row => ((row.ActualAmount ?? 0) - (row.OldPaid ?? 0)) - ((row.IsUniversity ? (row.CenterShareAmountPer ?? 0) : 100) * ((row.ActualAmount ?? 0) - (row.OldPaid ?? 0))) / 100).Sum();
                        decimal TotalConcessionExpense = model.AdmissionFees.Where(x => (x.AdmissionFeeYear == null || x.AdmissionFeeYear <= applicationModel.CurrentYear)).Select(row => (row.ConcessionAmount ?? 0)).Sum();
                        //decimal TotalSGSTAmount = model.AdmissionFees.Where(x => (x.AdmissionFeeYear == null || x.AdmissionFeeYear <= applicationModel.StartYear)).Select(row => (row.SGSTRate ?? 0)).Sum();
                        //decimal TotalCGSTAmount = model.AdmissionFees.Where(x => (x.AdmissionFeeYear == null || x.AdmissionFeeYear <= applicationModel.StartYear)).Select(row => (row.CGSTRate ?? 0)).Sum();
                        TotalReceivable = model.AdmissionFees.Where(x => (x.AdmissionFeeYear == null || x.AdmissionFeeYear <= applicationModel.CurrentYear)).Select(row => ((row.ActualAmount ?? 0) - (row.OldPaid ?? 0))).Sum();

                        //TotalReceivable = (TotalReceivable - TotalConcessionExpense);

                        RecievableAmountList(TotalReceivable, accountFlowModelList, true, applicationModel);
                        PayableAmountList(TotalPayable, accountFlowModelList, true, applicationModel);
                        //CGSTAmountList(TotalCGSTAmount, accountFlowModelList, true, applicationModel);
                        //SGSTAmountList(TotalSGSTAmount, accountFlowModelList, true, applicationModel);
                        ConcessionAmountList(TotalConcessionExpense, accountFlowModelList, true, applicationModel);
                        CreateAccountFlow(accountFlowModelList, true);

                    }
                    UpdateApplicationCertificates(model, applicationModel.RowKey);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.RowKey = applicationModel.RowKey;
                    model.HasInstallment = applicationModel.HasInstallment;
                    model.HasElective = dbContext.Subjects.Any(S => S.CourseSubjectDetails.Select(y => y.CourseSubjectMaster.CourseKey).FirstOrDefault() == applicationModel.CourseKey &&
                                                                            S.CourseSubjectDetails.Select(y => y.CourseSubjectMaster.UniversityMasterKey).FirstOrDefault() == applicationModel.UniversityMasterKey &&
                                                                             S.CourseSubjectDetails.Select(y => y.CourseSubjectMaster.AcademicTermKey).FirstOrDefault() == applicationModel.AcademicTermKey && S.IsElective == true);

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationPersenol, ActionConstants.Edit, DbConstants.LogType.Info, model.AdmissionNo, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Application);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationPersenol, ActionConstants.Edit, DbConstants.LogType.Error, model.AdmissionNo, ex.GetBaseException().Message);
                }
            }
            FillDropdownLists(model);
            return model;
        }
        private void CreateAdmissionFee(List<AdmissionFeeModel> modelList, Application applicationModel)
        {
            List<AdmissionFee> FeeModel = dbContext.AdmissionFees.Where(p => p.ApplicationKey == applicationModel.RowKey).ToList();
            if (FeeModel != null && modelList.Count > 0)
            {
                dbContext.AdmissionFees.RemoveRange(FeeModel);
            }
            long MaxKey = dbContext.AdmissionFees.Select(p => p.RowKey).DefaultIfEmpty().Max();

            foreach (AdmissionFeeModel model in modelList)
            {

                AdmissionFee FeeModels = new AdmissionFee();

                FeeModels.RowKey = Convert.ToInt64(MaxKey + 1);
                FeeModels.ApplicationKey = applicationModel.RowKey;
                FeeModels.AdmissionFeeYear = model.AdmissionFeeYear;
                FeeModels.AdmissionFeeAmount = Convert.ToDecimal(model.AdmissionFeeAmount);
                FeeModels.ConcessionAmount = model.ConcessionAmount;
                FeeModels.ActualAmount = Convert.ToDecimal(model.ActualAmount);
                FeeModels.FeeTypeKey = model.FeeTypeKey;
                FeeModels.OldPaid = model.OldPaid;
                FeeModels.IsActive = true;

                dbContext.AdmissionFees.Add(FeeModels);
                MaxKey++;
            }


        }
        private void UpdateAdmissionFee(List<AdmissionFeeModel> modelList, long ApplicationKey)
        {

            long MaxKey = dbContext.AdmissionFees.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (AdmissionFeeModel model in modelList)
            {
                AdmissionFee FeeModel = dbContext.AdmissionFees.SingleOrDefault(p => p.RowKey == model.RowKey);
                short oldFeeTypeKey = FeeModel.FeeTypeKey ?? 0;
                FeeModel.ApplicationKey = ApplicationKey;
                FeeModel.AdmissionFeeYear = model.AdmissionFeeYear;
                FeeModel.AdmissionFeeAmount = Convert.ToDecimal(model.AdmissionFeeAmount);
                FeeModel.ConcessionAmount = model.ConcessionAmount;
                FeeModel.ActualAmount = Convert.ToDecimal(model.ActualAmount);
                FeeModel.FeeTypeKey = model.FeeTypeKey;
                FeeModel.OldPaid = model.OldPaid;
                FeeModel.IsActive = true;

            }
        }
        #region Account
        private void CreateAccountFlow(List<AccountFlowViewModel> modelList, bool IsUpdate)
        {
            AccountFlowService accounFlowService = new AccountFlowService(dbContext);
            List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
            if (IsUpdate != true)
            {
                accounFlowService.CreateAccountFlow(modelList);
            }
            else
            {
                accounFlowService.UpdateAccountFlow(modelList);
            }
        }
        private void PayableAmountList(decimal Amount, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, Application ApplicationModel)
        {

            long ExtraUpdateKey = 0;
            //long AccountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.AccountsPayable).Select(x => x.RowKey).FirstOrDefault();
            long AccountHeadKey = dbContext.UniversityMasters.Where(x => x.RowKey == ApplicationModel.UniversityMasterKey).Select(x => x.AccountHeadKey ?? 0).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                AccountHeadKey = AccountHeadKey,
                Amount = Amount,
                TransactionTypeKey = DbConstants.TransactionType.Application,
                TransactionDate = Convert.ToDateTime(ApplicationModel.StudentDateOfAdmission),
                TransactionKey = ApplicationModel.RowKey,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                VoucherTypeKey = DbConstants.VoucherType.Admission,
                BranchKey = ApplicationModel.BranchKey,
                Purpose = EduSuiteUIResources.Admission + EduSuiteUIResources.BlankSpace + ApplicationModel.StudentName + " ( " + ApplicationModel.AdmissionNo + " ) ",
            });


        }
        private void RecievableAmountList(decimal Amount, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, Application ApplicationModel)
        {
            long ExtraUpdateKey = 0;
            long AccountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.AccountsReceivable).Select(x => x.RowKey).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.In,
                AccountHeadKey = AccountHeadKey,
                Amount = Amount,
                TransactionTypeKey = DbConstants.TransactionType.Application,
                TransactionDate = Convert.ToDateTime(ApplicationModel.StudentDateOfAdmission),
                TransactionKey = ApplicationModel.RowKey,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                VoucherTypeKey = DbConstants.VoucherType.Admission,
                BranchKey = ApplicationModel.BranchKey,
                Purpose = EduSuiteUIResources.Admission + EduSuiteUIResources.BlankSpace + ApplicationModel.StudentName + " ( " + ApplicationModel.AdmissionNo + " ) ",
            });

        }
        private void IncomeAmountList(decimal Amount, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, Application ApplicationModel)
        {
            long AccountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.CostOfService).Select(x => x.RowKey).FirstOrDefault();
            long ExtraUpdateKey = 0;
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                AccountHeadKey = AccountHeadKey,
                Amount = Amount,
                TransactionTypeKey = DbConstants.TransactionType.Application,
                TransactionDate = Convert.ToDateTime(ApplicationModel.StudentDateOfAdmission),
                TransactionKey = ApplicationModel.RowKey,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                VoucherTypeKey = DbConstants.VoucherType.Admission,
                BranchKey = ApplicationModel.BranchKey,
                Purpose = EduSuiteUIResources.Admission + EduSuiteUIResources.BlankSpace + ApplicationModel.StudentName + " ( " + ApplicationModel.AdmissionNo + " ) ",
            });

        }
        private void IncomeSplitAmountList(List<AdmissionFeeModel> modelList, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, Application ApplicationModel)
        {

            long ExtraUpdateKey = 0;

            foreach (AdmissionFeeModel item in modelList)
            {
                long AccountHeadKey = dbContext.FeeTypes.Where(x => x.RowKey == item.FeeTypeKey).Select(x => x.AccountHeadKey ?? 0).FirstOrDefault();
                accountFlowModelList.Add(new AccountFlowViewModel
                {
                    CashFlowTypeKey = DbConstants.CashFlowType.Out,
                    AccountHeadKey = AccountHeadKey,
                    Amount = (item.IsUniversity ? (item.CenterShareAmountPer ?? 0) : 100) * ((item.ActualAmount ?? 0) - (item.OldPaid ?? 0)) / 100,
                    TransactionTypeKey = DbConstants.TransactionType.Application,
                    TransactionDate = Convert.ToDateTime(ApplicationModel.StudentDateOfAdmission),
                    TransactionKey = ApplicationModel.RowKey,
                    ExtraUpdateKey = ExtraUpdateKey,
                    IsUpdate = IsUpdate,
                    VoucherTypeKey = DbConstants.VoucherType.Admission,
                    BranchKey = ApplicationModel.BranchKey,
                    Purpose = EduSuiteUIResources.Admission + EduSuiteUIResources.BlankSpace + ApplicationModel.StudentName + " ( " + ApplicationModel.AdmissionNo + " ) ",
                });
            }
        }
        private void CGSTAmountList(decimal Amount, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, Application ApplicationModel)
        {
            long ExtraUpdateKey = 0;
            long AccountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.OutputTaxCGST).Select(x => x.RowKey).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                AccountHeadKey = AccountHeadKey,
                Amount = Amount,
                TransactionTypeKey = DbConstants.TransactionType.Application,
                TransactionDate = Convert.ToDateTime(ApplicationModel.StudentDateOfAdmission),
                TransactionKey = ApplicationModel.RowKey,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                VoucherTypeKey = DbConstants.VoucherType.Admission,
                BranchKey = ApplicationModel.BranchKey,
                Purpose = EduSuiteUIResources.Admission + EduSuiteUIResources.BlankSpace + ApplicationModel.StudentName + " ( " + ApplicationModel.AdmissionNo + " ) ",
            });
        }
        private void SGSTAmountList(decimal Amount, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, Application ApplicationModel)
        {
            long ExtraUpdateKey = 0;
            long AccountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.OutputTaxSGST).Select(x => x.RowKey).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                AccountHeadKey = AccountHeadKey,
                Amount = Amount,
                TransactionTypeKey = DbConstants.TransactionType.Application,
                TransactionDate = Convert.ToDateTime(ApplicationModel.StudentDateOfAdmission),
                TransactionKey = ApplicationModel.RowKey,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = ApplicationModel.BranchKey,
                VoucherTypeKey = DbConstants.VoucherType.Admission,
                Purpose = EduSuiteUIResources.Admission + EduSuiteUIResources.BlankSpace + ApplicationModel.StudentName + " ( " + ApplicationModel.AdmissionNo + " ) ",
            });


        }
        private void ConcessionAmountList(decimal Amount, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, Application ApplicationModel)
        {
            long ExtraUpdateKey = 0;
            long AccountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.DiscountTaken).Select(x => x.RowKey).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.In,
                AccountHeadKey = AccountHeadKey,
                Amount = Amount,
                TransactionTypeKey = DbConstants.TransactionType.ApplicationConsession,
                TransactionDate = Convert.ToDateTime(ApplicationModel.StudentDateOfAdmission),
                TransactionKey = ApplicationModel.RowKey,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = ApplicationModel.BranchKey,
                VoucherTypeKey = DbConstants.VoucherType.Admission,
                Purpose = EduSuiteUIResources.Admission + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Concession + EduSuiteUIResources.BlankSpace + ApplicationModel.StudentName + " ( " + ApplicationModel.AdmissionNo + " ) ",
            });
            AccountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.AccountsReceivable).Select(x => x.RowKey).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                AccountHeadKey = AccountHeadKey,
                Amount = Amount,
                TransactionTypeKey = DbConstants.TransactionType.ApplicationConsession,
                TransactionDate = Convert.ToDateTime(ApplicationModel.StudentDateOfAdmission),
                TransactionKey = ApplicationModel.RowKey,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = ApplicationModel.BranchKey,
                VoucherTypeKey = DbConstants.VoucherType.Admission,
                Purpose = EduSuiteUIResources.Admission + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Concession + EduSuiteUIResources.BlankSpace + ApplicationModel.StudentName + " ( " + ApplicationModel.AdmissionNo + " ) ",
            });

        }

        #endregion
        private void CreateDivisionAllocation(long ApplicationKey, int? RollNumber, long? ClassDetailsKey, string RollNoCode, short? BatchKey)
        {
            long MaxKey = dbContext.StudentDivisionAllocations.Select(p => p.RowKey).DefaultIfEmpty().Max();

            StudentDivisionAllocation studentDivisionAllocationmodel = new StudentDivisionAllocation();
            studentDivisionAllocationmodel.RowKey = MaxKey + 1;
            studentDivisionAllocationmodel.ApplicationKey = ApplicationKey;
            studentDivisionAllocationmodel.RollNumber = RollNumber ?? 0;
            studentDivisionAllocationmodel.RollNoCode = RollNoCode;
            studentDivisionAllocationmodel.ClassDetailsKey = ClassDetailsKey ?? 0;
            studentDivisionAllocationmodel.BatchKey = BatchKey ?? 0;

            studentDivisionAllocationmodel.IsActive = true;
            dbContext.StudentDivisionAllocations.Add(studentDivisionAllocationmodel);
        }
        private void UpdateDivisionAllocation(long ApplicationKey, long? ClassDetailsKey, ApplicationPersonalViewModel model)
        {
            StudentDivisionAllocation studentDivisionAllocation = dbContext.StudentDivisionAllocations.SingleOrDefault(p => p.ClassDetailsKey == ClassDetailsKey && p.ApplicationKey == ApplicationKey && p.IsActive);

            if (studentDivisionAllocation != null)
            {
                StudentDivisionAllocation studentDivisionAllocationmodel = dbContext.StudentDivisionAllocations.SingleOrDefault(p => p.RowKey == studentDivisionAllocation.RowKey);

                if (model.StudentClassRequired == false)
                {
                    studentDivisionAllocationmodel.IsActive = false;
                }
                else
                {
                    studentDivisionAllocationmodel.IsActive = true;
                }
            }
        }
        public ApplicationPersonalViewModel CheckPhoneExists(string MobileNumber, Int64 RowKey)
        {
            ApplicationPersonalViewModel model = new ApplicationPersonalViewModel();
            if (dbContext.Applications.Where(row => row.StudentMobile == MobileNumber && row.RowKey != RowKey).Any())
            {
                model.IsSuccessful = false;
            }
            else
            {
                model.IsSuccessful = true;
            }
            return model;
        }
        public ApplicationPersonalViewModel CheckEmailExists(string EmailAddress, Int64 RowKey)
        {
            ApplicationPersonalViewModel model = new ApplicationPersonalViewModel();
            if (dbContext.Applications.Where(row => row.StudentEmail == EmailAddress && row.RowKey != RowKey).Any())
            {
                model.IsSuccessful = false;

            }
            else
            {
                model.IsSuccessful = true;
            }
            return model;
        }
        public void GetCourseType(ApplicationPersonalViewModel model)
        {

            model.CourseTypes = dbContext.CourseTypes.Where(row => row.IsActive && row.Courses.Any(x => x.UniversityCourses.Any(y => y.AcademicTermKey == model.AcademicTermKey && y.IsActive))).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.CourseTypeName
            }).ToList();

        }
        public void GetCourseByCourseType(ApplicationPersonalViewModel model)
        {

            model.Courses = dbContext.UniversityCourses.Where(row => row.Course.CourseTypeKey == model.CourseTypeKey && row.AcademicTermKey == model.AcademicTermKey && row.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.Course.RowKey,
                Text = row.Course.CourseName
            }).Distinct().ToList();

        }
        public void GetUniversity(ApplicationPersonalViewModel model)
        {
            model.Universities = dbContext.UniversityCourses.Where(row => row.CourseKey == model.CourseKey && row.AcademicTermKey == model.AcademicTermKey && row.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.UniversityMaster.RowKey,
                Text = row.UniversityMaster.UniversityMasterName
            }).Distinct().ToList();


        }
        public void GetYearByMode(ApplicationPersonalViewModel model)
        {
            int StartYear = dbContext.Modes.Where(row => row.RowKey == model.ModeKey).Select(row => row.StartYear).SingleOrDefault();

            UniversityCourse universityCourse = dbContext.UniversityCourses.SingleOrDefault(row => row.CourseKey == model.CourseKey && row.UniversityMasterKey == model.UniversityKey && row.AcademicTermKey == model.AcademicTermKey && row.IsActive);



            decimal duration;
            if (universityCourse != null)
            {
                var CourseDuration = universityCourse.Course.CourseDuration;
                duration = Math.Ceiling((Convert.ToDecimal(universityCourse.AcademicTermKey == DbConstants.AcademicTerm.Semester ? CourseDuration / 6 : CourseDuration / 12)));
            }
            else
            {
                var CourseDuration = dbContext.VwCourseSelectActiveOnlies.Where(row => row.RowKey == model.CourseKey).Select(row => row.CourseDuration).SingleOrDefault();
                duration = Math.Ceiling((Convert.ToDecimal(Convert.ToBoolean(model.AcademicTermKey ?? 0) ? ((CourseDuration ?? 0) / 6) : ((CourseDuration ?? 0) / 12))));

            }

            if (model.ModeKey != DbConstants.Mode.REGULAR)
            {
                if (model.AcademicTermKey == DbConstants.AcademicTerm.Semester)
                {
                    StartYear = ++StartYear;
                }
            }
            else
            {
                if (duration > 1)
                {
                    duration = StartYear;
                }
            }


            if (duration < 1)
            {
                model.AdmittedYear.Add(new SelectListModel
                {
                    RowKey = 1,
                    Text = "Short Term"
                });


            }
            else
            {
                for (int i = StartYear; i <= duration; i++)
                {
                    model.AdmittedYear.Add(new SelectListModel
                    {
                        RowKey = i,
                        Text = CommonUtilities.GetYearDescriptionByCode(i, model.AcademicTermKey ?? 0)
                    });
                }
            }
        }
        public ApplicationPersonalViewModel GetCurrentYearByYear(ApplicationPersonalViewModel model)
        {
            int StartYear = 0;
            if (model.StartYear == 0 || model.StartYear == null)
            {
                StartYear = dbContext.Modes.Where(row => row.RowKey == model.ModeKey).Select(row => row.StartYear).SingleOrDefault();
            }
            else
            {
                StartYear = model.StartYear ?? 0;
            }


            UniversityCourse universityCourse = dbContext.UniversityCourses.SingleOrDefault(row => row.CourseKey == model.CourseKey && row.UniversityMasterKey == model.UniversityKey && row.AcademicTermKey == model.AcademicTermKey && row.IsActive);



            decimal duration;
            if (universityCourse != null)
            {
                var CourseDuration = universityCourse.Course.CourseDuration;
                duration = Math.Ceiling((Convert.ToDecimal(universityCourse.AcademicTermKey == DbConstants.AcademicTerm.Semester ? CourseDuration / 6 : CourseDuration / 12)));
            }
            else
            {
                var CourseDuration = dbContext.VwCourseSelectActiveOnlies.Where(row => row.RowKey == model.CourseKey).Select(row => row.CourseDuration).SingleOrDefault();
                duration = Math.Ceiling((Convert.ToDecimal(Convert.ToBoolean(model.AcademicTermKey ?? 0) ? ((CourseDuration ?? 0) / 6) : ((CourseDuration ?? 0) / 12))));

            }

            if (model.ModeKey != DbConstants.Mode.REGULAR)
            {
                if (model.AcademicTermKey == DbConstants.AcademicTerm.Semester)
                {
                    StartYear = ++StartYear;
                }
            }
            //else
            //{
            //    if (duration > 1)
            //    {
            //        duration = StartYear;
            //    }
            //}


            if (duration < 1)
            {
                model.CurrentYears.Add(new SelectListModel
                {
                    RowKey = 1,
                    Text = "Short Term"
                });


            }
            else
            {
                for (int i = StartYear; i <= duration; i++)
                {
                    model.CurrentYears.Add(new SelectListModel
                    {
                        RowKey = i,
                        Text = CommonUtilities.GetYearDescriptionByCode(i, model.AcademicTermKey ?? 0)
                    });
                }
            }
            return model;
        }
        private void FillDropdownLists(ApplicationPersonalViewModel model)
        {
            FillBranches(model);
            GetEmployeesByBranchId(model);
            FillAcademicTerm(model);
            GetUniversity(model);
            GetCourseType(model);
            GetCourseByCourseType(model);
            FillAgents(model);
            FillBatches(model);
            FillReligions(model);
            FillNatureOfEnquiries(model);
            FillMedium(model);
            FillIncomeGroup(model);
            FillMode(model);
            FillClassMode(model);
            FillSecoundLanguage(model);
            GetYearByMode(model);
            GetCurrentYearByYear(model);
            FillClassDetails(model);
            FillCaste(model);
            FillBloodGroup(model);
            FillCommunityTypes(model);
            FillEducationTypes(model);
            FillRegistrationCatagory(model);
            FillCertificateType(model);
            FillStudentBioMetrics(model);
        }
        private void FillBranches(ApplicationPersonalViewModel model)
        {
            IQueryable<SelectListModel> BranchQuery = dbContext.vwBranchSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BranchName
            });

            Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
            if (Employee != null)
            {
                if (Employee.BranchAccess != null)
                {
                    List<long> Branches = Employee.BranchAccess.Split(',').Select(Int64.Parse).ToList();
                    model.Branches = BranchQuery.Where(row => Branches.Contains(row.RowKey)).ToList();
                    //model.BranchKey = Employee.BranchKey;
                }
                else
                {
                    model.Branches = BranchQuery.Where(x => x.RowKey == Employee.BranchKey).ToList();
                    //model.BranchKey = Employee.BranchKey;
                }
            }
            else
            {
                model.Branches = BranchQuery.ToList();
            }

            if (model.Branches.Count == 1)
            {
                long? branchkey = model.Branches.Select(x => x.RowKey).FirstOrDefault();
                model.BranchKey = Convert.ToInt16(branchkey);
            }
        }
        private void FillAgents(ApplicationPersonalViewModel model)
        {

            model.Agents = dbContext.Agents.Where(row => row.AgentActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AgentName
            }).ToList();
        }
        private void FillAcademicTerm(ApplicationPersonalViewModel model)
        {
            model.AcademicTerms = dbContext.VwAcadamicTermSelectActiveOnlies.Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.AcademicTermName,
            }).ToList();

        }
        private void FillReligions(ApplicationPersonalViewModel model)
        {
            model.Religions = dbContext.Religions.Where(row => row.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ReligionName
            }).ToList();
        }
        public ApplicationPersonalViewModel FillCaste(ApplicationPersonalViewModel model)
        {
            model.Caste = dbContext.Castes.Where(x => x.ReligionKey == model.ReligionKey && x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.CasteName
            }).ToList();
            return model;
        }
        private void FillBloodGroup(ApplicationPersonalViewModel model)
        {
            model.BloodGroups = dbContext.BloodGroups.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BloodGroupName
            }).ToList();
        }
        private void FillBatches(ApplicationPersonalViewModel model)
        {
            model.Batches = dbContext.VwBatchSelectActiveOnlies.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BatchName
            }).ToList();
        }
        private void FillMedium(ApplicationPersonalViewModel model)
        {
            model.Medium = dbContext.VwMediumSelectActiveOnlies.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.MediumName
            }).ToList();
        }
        private void FillIncomeGroup(ApplicationPersonalViewModel model)
        {
            model.IncomeGroups = dbContext.VwIncomeSelectActiveOnlies.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.IncomeName
            }).ToList();
        }
        private void FillMode(ApplicationPersonalViewModel model)
        {
            model.Modes = dbContext.VwModeSelectActiveOnlies.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ModeName
            }).ToList();
        }
        private void FillClassMode(ApplicationPersonalViewModel model)
        {
            model.ClassModes = dbContext.VwClassModeSelectActiveOnlies.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ClassModeName
            }).ToList();
        }
        private void FillSecoundLanguage(ApplicationPersonalViewModel model)
        {
            model.SecondLanguages = dbContext.VwSecoundLanguageSelectActiveOnlies.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.SecondLanguageName
            }).ToList();
        }
        private void FillNatureOfEnquiries(ApplicationPersonalViewModel model)
        {
            model.NatureOfEnquiries = dbContext.NatureOfEnquiries.Where(row => row.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.NatureOfEnquiryName
            }).ToList();
        }
        private void FillCommunityTypes(ApplicationPersonalViewModel model)
        {
            model.CommunityTypes = dbContext.CommunityTypes.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.Name
            }).ToList();
        }
        private void FillEducationTypes(ApplicationPersonalViewModel model)
        {
            model.EducationTypes = typeof(DbConstants.EducationType).GetFields().Select(row => new SelectListModel
            {
                RowKey = Convert.ToByte(row.GetValue(null).ToString()),
                Text = row.Name
            }).ToList();
            if (DbConstants.EducationType.Both == DbConstants.GeneralConfiguration.EducationTypeKey)
            {
                model.EducationTypes = model.EducationTypes.Where(x => x.RowKey != DbConstants.EducationType.Both).ToList();
            }
            else if (DbConstants.EducationType.RegulerEducation == DbConstants.GeneralConfiguration.EducationTypeKey)
            {
                model.EducationTypes = model.EducationTypes.Where(x => x.RowKey == DbConstants.EducationType.RegulerEducation).ToList();
            }
            else
            {
                model.EducationTypes = model.EducationTypes.Where(x => x.RowKey == DbConstants.EducationType.DistanceEducation).ToList();
            }
        }
        private void FillRegistrationCatagory(ApplicationPersonalViewModel model)
        {
            model.RegistrationCatagory = dbContext.RegistratonCatagories.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.CatagoryName
            }).ToList();

        }
        public ApplicationPersonalViewModel GetAdmissionFeesByCourse(ApplicationPersonalViewModel model)
        {
            UniversityCourse universityCourse = dbContext.UniversityCourses.SingleOrDefault(row => row.CourseKey == model.CourseKey && row.UniversityMasterKey == model.UniversityKey && row.AcademicTermKey == model.AcademicTermKey && row.IsActive);

            if (universityCourse != null)
            {
                model.UniversityCourseKey = universityCourse.RowKey;
            }
            decimal duration;
            decimal Cduration;

            if (universityCourse != null)
            {
                var CourseDuration = universityCourse.Course.CourseDuration;
                duration = Math.Ceiling((Convert.ToDecimal(universityCourse.AcademicTermKey == DbConstants.AcademicTerm.Semester ? CourseDuration / 6 : CourseDuration / 12)));
                Cduration = Math.Ceiling((Convert.ToDecimal(universityCourse.AcademicTermKey == DbConstants.AcademicTerm.Semester ? CourseDuration / 6 : CourseDuration / 12)));
            }
            else
            {
                var CourseDuration = dbContext.VwCourseSelectActiveOnlies.Where(row => row.RowKey == model.CourseKey).Select(row => row.CourseDuration).SingleOrDefault();
                duration = Math.Ceiling((Convert.ToDecimal(Convert.ToBoolean(model.AcademicTermKey ?? 0) ? ((CourseDuration ?? 0) / 6) : ((CourseDuration ?? 0) / 12))));
                Cduration = Math.Ceiling((Convert.ToDecimal(Convert.ToBoolean(model.AcademicTermKey ?? 0) ? ((CourseDuration ?? 0) / 6) : ((CourseDuration ?? 0) / 12))));
            }


            model.AdmissionFees = dbContext.AdmissionFees.Where(row => row.ApplicationKey == model.RowKey).OrderBy(row => row.AdmissionFeeYear).ToList().Select(row => new AdmissionFeeModel
            {
                RowKey = row.RowKey,
                AdmissionFeeAmount = row.AdmissionFeeAmount,
                ConcessionAmount = row.ConcessionAmount,
                ActualAmount = row.ActualAmount,
                AdmissionFeeYear = row.AdmissionFeeYear,
                AdmissionFeeYearText = row.AdmissionFeeYear != null ? (duration < 1 ? "Short Term" : CommonUtilities.GetYearDescriptionByCode(row.AdmissionFeeYear ?? 0, row.Application.AcademicTermKey)) : "",

                HasConcession = row.Application.HasConcession || row.Application.HasOffer,
                FeeTypeKey = row.FeeTypeKey,
                FeeTypeName = row.FeeType.FeeTypeName,
                //CGSTRate = row.CGSTRate,
                //SGSTRate = row.SGSTRate,
                //IGSTRate = row.IGSTRate,
                //CGSTAmount = row.CGSTAmount,
                //SGSTAmount = row.SGSTAmount,
                //IGSTAmount = row.IGSTAmount,
                IsUniversity = row.FeeType.IsUniverisity,
                OldPaid = row.OldPaid,
                CenterShareAmountPer = dbContext.UniversityCourseFees.Where(x => x.FeeTypeKey == row.FeeTypeKey && x.FeeYear == row.AdmissionFeeYear && x.UniversityCourse.AcademicTermKey == row.Application.AcademicTermKey && x.UniversityCourse.CourseKey == row.Application.CourseKey && x.UniversityCourse.UniversityMasterKey == row.Application.UniversityMasterKey).Select(x => x.CenterShareAmountPer ?? 0).FirstOrDefault(),

            }).ToList();


            var StartYear = model.StartYear ?? 0;
            if (duration != 0)
            {
                model.CourseDuration = Convert.ToInt32(duration);
            }
            else if (duration == 0)
            {
                duration = 1;
            }

            if (model.AdmissionFees.Count == 0)
            {
                if (universityCourse != null)
                {
                    model.AdmissionFees = universityCourse.UniversityCourseFees.Where(x => x.IsActive == true && x.FeeAmount != null && (x.FeeYear == null || (x.FeeYear >= StartYear && x.FeeYear <= duration))).Select(x => new AdmissionFeeModel
                    {
                        AdmissionFeeYearText = x.FeeYear != null ? (Cduration < 1 ? "Short Term" : CommonUtilities.GetYearDescriptionByCode(x.FeeYear ?? 0, model.AcademicTermKey ?? 0)) : "",
                        AdmissionFeeYear = x.FeeYear,
                        ActualAmount = x.FeeAmount,
                        FeeTypeKey = x.FeeTypeKey,
                        FeeTypeName = x.FeeType.FeeTypeName,

                        IsUniversity = x.FeeType.IsUniverisity,
                        CenterShareAmountPer = x.CenterShareAmountPer
                    }).OrderBy(x => x.AdmissionFeeYear).ToList();
                }
            }
            return model;
        }
        public ApplicationPersonalViewModel GetOfferDetails(ApplicationPersonalViewModel model)
        {

            model = dbContext.VwOfferSelectActiveOnlies.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.FromDate) <= System.Data.Entity.DbFunctions.TruncateTime(DateTimeUTC.Now) && System.Data.Entity.DbFunctions.TruncateTime(row.Todate) >= System.Data.Entity.DbFunctions.TruncateTime(DateTimeUTC.Now))
                  .Select(row => new ApplicationPersonalViewModel
                  {
                      OfferName = row.OfferName,
                      OfferValue = row.OfferValue,
                      OfferKey = row.RowKey,
                  }).FirstOrDefault();


            if (model == null)
            {
                model = new ApplicationPersonalViewModel();
                model.Message = EduSuiteUIResources.OfferMessage;
                model.IsSuccessful = false;
            }
            else
            {
                model.IsSuccessful = true;
            }

            return model;
        }
        public ApplicationPersonalViewModel FetchApplicationFromEnquiry(ApplicationPersonalViewModel model)
        {
            try
            {
                short CourseTypeKey = 0;
                ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();
                objViewModel = dbContext.Enquiries.Where(x => x.RowKey == model.EnquiryKey).Select(row => new ApplicationPersonalViewModel
                {
                    EnquiryKey = row.RowKey,
                    AcademicTermKey = row.AcademicTermKey ?? 0,
                    CourseTypeKey = row.CourseKey != null ? row.Course.CourseTypeKey : CourseTypeKey,
                    CourseKey = row.CourseKey ?? 0,
                    UniversityKey = row.UniversityKey ?? 0,
                    ApplicantName = row.EnquiryName,
                    PermenantAddress = row.EnquiryAddress,
                    MobileNumber = row.MobileNumber,
                    StudentPhone = row.PhoneNumber,
                    StudentEmail = row.EmailAddress,
                    DateOfBirth = row.DateOfBirth,
                    Gender = row.Gender ?? 1,
                    NatureOfEnquiryKey = row.NatureOfEnquiryKey,
                    BranchKey = row.BranchKey,
                    //ScheduledEmployeeName = row.Employee.FirstName + " " + (row.Employee.MiddleName ?? "") + " " + row.Employee.LastName
                }).FirstOrDefault();
                if (objViewModel == null)
                {
                    objViewModel = new ApplicationPersonalViewModel();
                    //objViewModel.AcademicTermKey = DbConstants.AcademicTerm.Study;

                }


                Int64 maxKey = dbContext.Applications.Select(p => p.RowKey).DefaultIfEmpty().Max();
                objViewModel.AdmissionNo = (maxKey + 1).ToString();

                FillDropdownLists(objViewModel);
                return objViewModel;
            }
            catch (Exception)
            {
                return new ApplicationPersonalViewModel();
            }
        }
        public ApplicationPersonalViewModel FillClassDetails(ApplicationPersonalViewModel model)
        {
            model.ClassDetails = dbContext.VwClassDetailsSelectActiveOnlies.Where(x => x.AcademicTermKey == model.AcademicTermKey && x.CourseKey == model.CourseKey
                && x.UniversityMasterKey == model.UniversityKey && x.StudentYear == (model.CurrentYear != null ? model.CurrentYear : model.AdmissionCurrentYear)).Select(x => new SelectListModel
                {
                    RowKey = x.RowKey,
                    Text = x.ClassCode + x.ClassCodeDescription
                }).Distinct().ToList();
            return model;
        }
        public long CreateStudentUserAccount(Application applicationModel)
        {

            AppUser appUser = new AppUser();

            Int64 maxKey = dbContext.AppUsers.Select(p => p.RowKey).DefaultIfEmpty().Max();
            appUser.RowKey = Convert.ToInt32(maxKey + 1);
            appUser.AppUserName = applicationModel.AdmissionNo;
            appUser.FirstName = applicationModel.StudentName;
            appUser.Phone1 = applicationModel.StudentMobile;
            appUser.EmailAddress = applicationModel.StudentEmail ?? "";
            appUser.Image = applicationModel.StudentPhotoPath;
            appUser.Password = SecurityManagement.Encrypt(applicationModel.StudentMobile);

            appUser.RoleKey = DbConstants.Role.Students;
            appUser.IsActive = true;
            appUser.ApplicationKey = applicationModel.RowKey;
            appUser.PasswordHint = applicationModel.AdmissionNo;
            dbContext.AppUsers.Add(appUser);

            return appUser.RowKey;
        }
        private void FillNotificationDetail(ApplicationPersonalViewModel model)
        {
            NotificationTemplate notificationTemplateModel = dbContext.NotificationTemplates.SingleOrDefault(row => row.RowKey == DbConstants.NotificationTemplate.Application);
            if (notificationTemplateModel != null)
            {
                model.AutoEmail = notificationTemplateModel.AutoEmail;
                model.AutoSMS = notificationTemplateModel.AutoSMS;
                model.TemplateKey = notificationTemplateModel.RowKey;
            }
        }
        public ApplicationPersonalViewModel GetEmployeesByBranchId(ApplicationPersonalViewModel model)
        {
            //Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();

            //if (DbConstants.User.UserKey != DbConstants.AdminKey)
            //{
            //    if (Employee != null)
            //    {
            //        var Branches = Employee.BranchAccess.Split(',').Select(Int16.Parse).ToList();

            //        if (Branches.Count > 1)
            //        {
            //            model.Employees = dbContext.Employees.Where(row => row.EmployeeStatusKey == DbConstants.EmployeeStatus.Working && row.IsActive == true && (Branches.Contains(row.BranchKey))).Select(row => new SelectListModel
            //            {
            //                RowKey = row.RowKey,
            //                Text = row.FirstName,

            //            }).OrderBy(row => row.Text).ToList();
            //        }
            //        else
            //        {
            //            model.Employees = dbContext.Employees.Where(row => row.EmployeeStatusKey == DbConstants.EmployeeStatus.Working && row.IsActive == true && row.RowKey == Employee.RowKey).Select(row => new SelectListModel
            //            {
            //                RowKey = row.RowKey,
            //                Text = row.FirstName,
            //            }).OrderBy(row => row.Text).ToList();
            //        }
            //    }
            //}
            //else
            //{
            //    model.Employees = dbContext.Employees.Where(row => row.EmployeeStatusKey == DbConstants.EmployeeStatus.Working && (row.BranchKey == model.BranchKey || model.BranchKey == 0)).Select(row => new SelectListModel
            //    {
            //        RowKey = row.RowKey,
            //        Text = row.FirstName + " " + (row.MiddleName ?? "") + " " + row.LastName,

            //    }).OrderBy(row => row.Text).ToList();
            //}

            IQueryable<EmployeePersonalViewModel> EmployeesList = dbContext.Employees.Where(y => y.IsActive == true && y.EmployeeStatusKey == DbConstants.EmployeeStatus.Working).Select(x => new EmployeePersonalViewModel
            {
                FirstName = x.FirstName + " " + (x.MiddleName ?? "") + " " + x.LastName,
                RowKey = x.RowKey,
                BranchKey = x.BranchKey,
                DesignationKey = x.DesignationKey,
                AppUserKey = x.AppUserKey,
                EmployeeCode = x.EmployeeCode
            });
            var Employees = EmployeesList.ToList();
            List<long> EmployeeKeys = new List<long>();
            if (!DbConstants.Role.AdminUserTypes.Contains(DbConstants.User.RoleKey))
            {
                Employee employer = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).FirstOrDefault();
                if (employer != null)
                {
                    EmployeeKeys = dbContext.EmployeeHierarchies.Where(x => x.EmployeeKey == employer.RowKey).Select(y => y.ToEmployeeKey ?? 0).ToList();
                    EmployeeKeys.Add(employer.RowKey);
                    if (EmployeeKeys.Count > 0)
                    {
                        Employees = Employees.Where(x => EmployeeKeys.Contains(x.RowKey)).ToList();
                    }
                    else
                    {
                        Employees = Employees.Where(x => x.RowKey == employer.RowKey).ToList();
                    }
                }
            }

            if (model.BranchKey != null)
            {
                Employees = Employees.Where(x => x.BranchKey == model.BranchKey).ToList();
            }
            model.Employees = Employees.Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.FirstName
            }).ToList();

            return model;
        }
        private void FillCertificateType(ApplicationPersonalViewModel model)
        {
            model.Certificates = dbContext.VwCertificateTypeSelectActiveOnlies.Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.CertificateTypeName,
            }).ToList();
        }
        private void UpdateApplicationCertificates(ApplicationPersonalViewModel model, long? ApplicationKey)
        {
            long MaxKey = dbContext.ApplicationCertificates.Select(x => x.RowKey).DefaultIfEmpty().Max();
            List<short> universityCertificateKeys = dbContext.UniversityCertificates.Where(x => x.ApplicationKey == model.RowKey).Select(y => y.CertificateTypeKey).ToList();
            List<ApplicationCertificate> applicationCertificates = dbContext.ApplicationCertificates.Where(x => x.ApplicationKey == model.RowKey && !universityCertificateKeys.Contains(x.CertificateTypeKey)).ToList();
            if (applicationCertificates.Count > 0)
            {
                dbContext.ApplicationCertificates.RemoveRange(applicationCertificates);
            }
            model.CertificateKeys.RemoveAll(y => universityCertificateKeys.Contains(y));
            if (model.CertificateKeys != null && model.CertificateKeys.Count > 0)
            {
                foreach (short CertificateKey in model.CertificateKeys)
                {
                    if (CertificateKey != 0)
                    {
                        ApplicationCertificate objApplicationCertificatemodel = new ApplicationCertificate();

                        objApplicationCertificatemodel.RowKey = MaxKey + 1;
                        objApplicationCertificatemodel.ApplicationKey = ApplicationKey ?? 0;
                        objApplicationCertificatemodel.CertificateTypeKey = CertificateKey;
                        objApplicationCertificatemodel.IsActive = true;

                        dbContext.ApplicationCertificates.Add(objApplicationCertificatemodel);
                        dbContext.SaveChanges();
                        MaxKey++;
                    }
                }
            }
        }
        public bool CheckSecondLanguage(short? CourseTypekey)
        {
            return dbContext.CourseTypes.Where(x => x.RowKey == CourseTypekey).Select(x => x.HasSecondLanguage).FirstOrDefault();
        }
        private void FillStudentBioMetrics(ApplicationPersonalViewModel model)
        {
            model.StudentBioMetrics = dbContext.EsslStudents.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.EmployeeCode + (row.EmployeeName != null ? EduSuiteUIResources.OpenBracketWithSpace + row.EmployeeName + EduSuiteUIResources.ClosingBracketWithSpace : "")
            }).ToList();

        }
    }
}
