using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;

namespace CITS.EduSuite.Business.Services
{
    public class ApplicationService : IApplicationService
    {
        private EduSuiteDatabase dbContext;

        public ApplicationService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<ApplicationPersonalViewModel> GetApplications(ApplicationPersonalViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;

                IQueryable<ApplicationPersonalViewModel> applicationList = (from a in dbContext.Applications
                                                                            where (a.StudentName.Contains(model.ApplicantName)
                                                                            || a.AdmissionNo.Contains(model.ApplicantName)) && (a.StudentMobile.Contains(model.MobileNumber))
                                                                            select new ApplicationPersonalViewModel
                                                                            {
                                                                                RowKey = a.RowKey,
                                                                                AdmissionNo = a.AdmissionNo,
                                                                                ApplicantName = a.StudentName,
                                                                                AcademicTermName = a.AcademicTerm.AcademicTermName,
                                                                                CourseName = a.Course.CourseName,
                                                                                UniversityName = a.UniversityMaster.UniversityMasterName,
                                                                                MobileNumber = a.StudentMobile,
                                                                                ApplicationStatusName = a.StudentStatu.StudentStatusName,
                                                                                BatchName = a.Batch.BatchName,
                                                                                BranchKey = a.BranchKey,
                                                                                BatchKey = a.BatchKey,
                                                                                CourseKey = a.CourseKey,
                                                                                UniversityKey = a.UniversityMasterKey,
                                                                                BranchName = a.Branch.BranchName,
                                                                                AcademicTermKey = a.AcademicTermKey,
                                                                                CurrentYear = a.CurrentYear,
                                                                                CourseDuration = a.Course.CourseDuration ?? 0,
                                                                                ApplicationStatusKey = a.StudentStatusKey ?? 0,
                                                                                Gender = a.StudentGender,
                                                                                ClassDetailsName = a.ClassDetail.ClassCode,
                                                                                ApplicantPhoto = (a.OldStudentPhotoPath != null && a.OldStudentPhotoPath != "") ? a.OldStudentPhotoPath : UrlConstants.ApplicationUrl + a.RowKey + "/" + a.StudentPhotoPath,
                                                                                StudentPhotoPath = a.StudentPhotoPath,
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

                if (model.BranchKey != 0)
                {
                    applicationList = applicationList.Where(row => row.BranchKey == model.BranchKey);
                }
                if (model.BatchKey != 0)
                {
                    applicationList = applicationList.Where(row => row.BatchKey == model.BatchKey);
                }
                if (model.CourseKey != 0)
                {
                    applicationList = applicationList.Where(row => row.CourseKey == model.CourseKey);
                }
                if (model.UniversityKey != 0)
                {
                    applicationList = applicationList.Where(row => row.UniversityKey == model.UniversityKey);
                }
                if (model.HasOffer)
                {
                    applicationList = applicationList.Where(row => row.StudentPhotoPath != null);
                }
                applicationList = applicationList.GroupBy(x => x.RowKey).Select(y => y.FirstOrDefault());
                if (model.SortBy != "")
                {
                    applicationList = SortApplications(applicationList, model.SortBy, model.SortOrder);
                }
                TotalRecords = applicationList.Count();
                return applicationList.Skip(Skip).Take(Take).ToList<ApplicationPersonalViewModel>();
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.Application, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<ApplicationPersonalViewModel>();

            }
        }
        private IQueryable<ApplicationPersonalViewModel> SortApplications(IQueryable<ApplicationPersonalViewModel> Query, string SortName, string SortOrder)
        {

            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(ApplicationPersonalViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<ApplicationPersonalViewModel>(resultExpression);

        }

        public ApplicationPersonalViewModel DeleteApplication(long Id)
        {
            ApplicationPersonalViewModel applicationPersonaModel = new ApplicationPersonalViewModel();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Application applicationPersonal = dbContext.Applications.SingleOrDefault(row => row.RowKey == Id);
                    List<StudentCourseTransfer> studentCourseTransferList = dbContext.StudentCourseTransfers.Where(row => row.ApplicationKey == Id).ToList();
                    List<TransferAdmissionFee> transferAdmissionFeeList = dbContext.TransferAdmissionFees.Where(row => row.ApplicationKey == Id).ToList();
                    applicationPersonaModel.AdmissionNo = applicationPersonal.AdmissionNo;
                    List<ElectivePaper> ElectivePapperDetailList = dbContext.ElectivePapers.Where(row => row.ApplicationKey == Id).ToList();
                    List<AdmissionFee> AdmissionFeeDetailList = dbContext.AdmissionFees.Where(row => row.ApplicationKey == Id).ToList();
                    List<Attendance> attendanceList = dbContext.Attendances.Where(row => row.ApplicationKey == Id).ToList();
                    List<StudentDivisionAllocation> studentDivisionAllocation = dbContext.StudentDivisionAllocations.Where(row => row.ApplicationKey == Id).ToList();
                    List<ApplicationCertificate> applicationCertificateList = dbContext.ApplicationCertificates.Where(row => row.ApplicationKey == Id).ToList();
                    List<AccountFlow> AccountList = dbContext.AccountFlows.Where(x => (x.TransactionTypeKey == DbConstants.TransactionType.Application ||
                                                    x.TransactionTypeKey == DbConstants.TransactionType.ApplicationConsession
                                                   ) && x.TransactionKey == Id).ToList();

                    if (applicationPersonal.AllowLogin && applicationPersonal.AppUserKey != null)
                    {
                        AppUser appUser = dbContext.AppUsers.Where(row => row.RowKey == applicationPersonal.AppUserKey && row.RoleKey == DbConstants.Role.Students).FirstOrDefault();
                        dbContext.ActivityLogs.RemoveRange(dbContext.ActivityLogs.Where(row => row.AppUserKey == appUser.RowKey).ToList());
                        List<DocumentTrack> DocumentTrackList = dbContext.DocumentTracks.Where(row => row.AppUserKey == appUser.RowKey).ToList();

                        dbContext.DocumentTracks.RemoveRange(DocumentTrackList);

                        dbContext.AppUsers.Remove(appUser);
                    }

                    //AccountFlowService accounFlowService = new AccountFlowService(dbContext);
                    //AccountFlowViewModel accountFlowModel = new AccountFlowViewModel();
                    //accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.Application;
                    //accountFlowModel.TransactionKey = Id;
                    //accountFlowModel.IsDelete = true;
                    //accounFlowService.DeleteAccountFlow(accountFlowModel);

                    dbContext.AccountFlows.RemoveRange(AccountList);
                    dbContext.ElectivePapers.RemoveRange(ElectivePapperDetailList);
                    dbContext.AdmissionFees.RemoveRange(AdmissionFeeDetailList);
                    dbContext.TransferAdmissionFees.RemoveRange(transferAdmissionFeeList);
                    dbContext.StudentCourseTransfers.RemoveRange(studentCourseTransferList);
                    dbContext.ApplicationCertificates.RemoveRange(applicationCertificateList);
                    dbContext.Applications.Remove(applicationPersonal);

                    if (attendanceList.Count == 0 && studentDivisionAllocation.Count > 0)
                    {
                        dbContext.StudentDivisionAllocations.RemoveRange(studentDivisionAllocation);
                    }


                    dbContext.SaveChanges();
                    transaction.Commit();
                    applicationPersonaModel.Message = EduSuiteUIResources.Success;
                    applicationPersonaModel.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Application, ActionConstants.Delete, DbConstants.LogType.Info, Id, applicationPersonaModel.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        applicationPersonaModel.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Application);
                        applicationPersonaModel.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.Application, ActionConstants.Delete, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    applicationPersonaModel.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Application);
                    applicationPersonaModel.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Application, ActionConstants.Delete, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);
                }
            }
            return applicationPersonaModel;
        }

        public ApplicationPersonalViewModel GetSearchDropdownList(ApplicationPersonalViewModel model)
        {
            FillBranches(model);
            FillSearchBatch(model);
            FillSearchCourse(model);
            FillSearchUniversity(model);
            return model;
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

        public ApplicationPersonalViewModel GetApplicantPhotoById(long Id)
        {
            try
            {
                ApplicationPersonalViewModel model = new ApplicationPersonalViewModel();
                model = dbContext.Applications.Select(row => new ApplicationPersonalViewModel
                {
                    RowKey = row.RowKey,
                    ApplicantPhoto = (row.OldStudentPhotoPath != null && row.OldStudentPhotoPath != "") ? row.OldStudentPhotoPath : UrlConstants.ApplicationUrl + row.RowKey + "/" + row.StudentPhotoPath,
                    StudentPhotoPath = row.StudentPhotoPath

                }).Where(x => x.RowKey == Id).FirstOrDefault();
                if (model == null)
                {
                    model.RowKey = Id;
                }
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Application, ActionConstants.View, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);
                return new ApplicationPersonalViewModel();

            }
        }

        public ApplicationPersonalViewModel UpdateApplicantPhoto(ApplicationPersonalViewModel model)
        {
            Application applicationModel = new Application();


            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    applicationModel = dbContext.Applications.SingleOrDefault(row => row.RowKey == model.RowKey);
                    applicationModel.StudentPhotoPath = applicationModel.RowKey + model.ApplicantPhoto + ".jpg";
                    applicationModel.OldStudentPhotoPath = "";

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.ApplicantPhoto = applicationModel.StudentPhotoPath;
                    model.AdmissionNo = applicationModel.AdmissionNo;
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Application, ActionConstants.AddEdit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.ApplicationPhoto);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Application, ActionConstants.AddEdit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;
        }

        public ApplicationPersonalViewModel DeleteApplicantPhoto(long Id)
        {
            ApplicationPersonalViewModel model = new ApplicationPersonalViewModel();
            Application applicationModel = new Application();


            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    applicationModel = dbContext.Applications.SingleOrDefault(row => row.RowKey == Id);
                    model.ApplicantPhoto = applicationModel.StudentPhotoPath;
                    model.StudentPhotoPath = applicationModel.OldStudentPhotoPath;
                    model.RowKey = applicationModel.RowKey;
                    model.AdmissionNo = applicationModel.AdmissionNo;
                    applicationModel.StudentPhotoPath = null;
                    applicationModel.OldStudentPhotoPath = null;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Application, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.ApplicationPhoto);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Application, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public ApplicationViewModel GetApplicationDetailsById(long Id)
        {
            ApplicationViewModel model = new ApplicationViewModel();

            var application = dbContext.Applications.SingleOrDefault(x => x.RowKey == Id);
            model.RowKey = application.RowKey;
            model.IsTax = application.IsTax;
            #region Personal Details

            model.PersonalDetails = dbContext.Applications.Where(row => row.RowKey == Id).Select(row => new ApplicationPersonalViewModel
            {

                AdmissionNo = row.AdmissionNo,
                ApplicationNo = row.ApplicationNo,
                ApplicantName = row.StudentName,
                ApplicationStatusKey = row.StudentStatusKey ?? 0,
                ApplicationStatusName = row.StudentStatu.StudentStatusName,
                //ApplicationStatusColor = row.StudentStatu.,
                AcademicTermName = row.AcademicTerm.AcademicTermName,
                StudentEnrollmentNo = row.StudentEnrollmentNo,
                ExamRegisterNo = row.ExamRegisterNo,
                CourseName = row.Course.CourseName,
                UniversityName = row.UniversityMaster.UniversityMasterName,
                BatchName = row.Batch.BatchName,
                DateOfBirth = row.StudentDOB,
                Gender = row.StudentGender,
                StudentEmail = row.StudentEmail,
                MobileNumber = row.StudentMobile,
                RollNoCode = row.RollNoCode,
                StudentPhone = row.StudentPhone,
                PermenantAddress = row.StudentPermanentAddress,
                PresentAddress = row.StudentPresentAddress,
                ApplicantGuardianName = row.StudentGuardian,
                ApplicantMotherName = row.StudentMotherName,
                ReligionName = row.Religion.ReligionName,
                CasteName = row.Caste.CasteName,
                NatureOfEnquiryName = row.NatureOfEnquiry.NatureOfEnquiryName,
                DateOfApplication = row.StudentDateOfAdmission,
                TotalFeeAmount = row.StudentTotalFee,
                BranchName = row.Branch.BranchName,
                BranchKey = row.BranchKey,
                AgentName = row.Agent.AgentName,
                Remarks = row.Remarks,
                ModeName = row.Mode.ModeName,
                MediumName = row.Medium.MediumName,
                ClassRequired = row.StudentClassRequired == true ? "Yes" : "No",
                HasInstallment = row.HasInstallment,
                HasConcession = row.HasConcession,
                HasOffer = row.HasOffer,

                ApplicantPhoto = (row.OldStudentPhotoPath != null && row.OldStudentPhotoPath != "") ? row.OldStudentPhotoPath : UrlConstants.ApplicationUrl + row.RowKey + "/" + row.StudentPhotoPath,
                StudentPhotoPath = row.StudentPhotoPath,
                ClassDetailsName = row.ClassDetail.ClassCode,
                //ScheduledEmployeeName = row.Enquiry.Employee.FirstName + " " + (row.Enquiry.Employee.MiddleName ?? "") + " " + row.Enquiry.Employee.LastName,
                //CounsellingEmployeeName = row.Employee.FirstName + " " + (row.Employee.MiddleName ?? "") + " " + row.Employee.LastName,
                //DocumentEmployeeName = row.Employee1.FirstName + " " + (row.Employee1.MiddleName ?? "") + " " + row.Employee1.LastName,
                CourseDuration = row.Course.CourseDuration ?? 0,
                AcademicTermKey = row.AcademicTermKey,
                CurrentYear = row.CurrentYear,
                CompanyLogo = row.Branch.IsFranchise == true ? row.Branch.BranchLogo : row.Branch.Company.CompanyLogo,
                CompanyLogoPath = row.Branch.IsFranchise == true ? UrlConstants.BranchLogo + row.Branch.BranchLogo : UrlConstants.CompanyLogo + row.Branch.Company.CompanyLogo,


            }).SingleOrDefault();
            if (model.PersonalDetails == null)
            {
                model.PersonalDetails = new ApplicationPersonalViewModel();
            }
            else
            {
                model.PersonalDetails.CurrentYearText = CommonUtilities.GetYearDescriptionByCodeDetails(model.PersonalDetails.CourseDuration, model.PersonalDetails.CurrentYear ?? 0, model.PersonalDetails.AcademicTermKey ?? 0);
            }
            #endregion Personal Details

            #region Educational Details

            var StudentsCertificateReturnDetail = (from scr in dbContext.StudentsCertificateReturns.Where(x => x.ApplicationKey == Id)
                                                   orderby scr.RowKey descending
                                                   select new StudentsCertificateReturnDetail
                                                   {
                                                       EducationQualificationName = scr.EducationQualification.EducationQualificationCourse,
                                                       EducationQualificationUniversity = scr.EducationQualification.EducationQualificationUniversity,
                                                       EducationQualificationCertificateType = scr.EducationQualification.IsOriginalIssued == true ? "Original" : scr.EducationQualification.IsCopyIssued == true ? "Copy" : "",
                                                       EducationQualificationKey = scr.EducationQualification.RowKey,
                                                       RowKey = scr.RowKey,
                                                       CertificateStatusName = (scr.CertificateStatusKey == DbConstants.CertificateProcessType.Received ? EduSuiteUIResources.Recieved : (scr.CertificateStatusKey == DbConstants.CertificateProcessType.Returned && scr.EducationQualification.IsOriginalReturn == true ? EduSuiteUIResources.Returned : (scr.CertificateStatusKey == DbConstants.CertificateProcessType.Returned ? EduSuiteUIResources.TempReturned : EduSuiteUIResources.Verified))),
                                                       CertificateStatusBy = dbContext.AppUsers.Where(x => x.RowKey == scr.IssuedBy).Select(y => y.AppUserName).FirstOrDefault(),
                                                       IssuedDate = scr.IssuedDate,
                                                       EducationQualificationYear = scr.EducationQualification.EducationQualificationYear,
                                                       EducationQualificationPercentage = scr.EducationQualification.EducationQualificationPercentage,
                                                       //EducationQualificationCertificatePath = scr.EducationQualification.EducationQualificationCertificatePath != null ? UrlConstants.ApplicationUrl + scr.Application.AdmissionNo + "/Document/" + scr.EducationQualification.EducationQualificationCertificatePath : scr.EducationQualification.EducationQualificationCertificatePath,
                                                       EducationQualificationCertificatePath = scr.EducationQualification.OldDocumentPath != null ? scr.EducationQualification.OldDocumentPath : UrlConstants.ApplicationUrl + scr.EducationQualification.Application.RowKey + "/EducationQualification/" + scr.EducationQualification.EducationQualificationCertificatePath,
                                                       ReturnCertificateDetails = dbContext.StudentsCertificateReturns.Where(x => x.ApplicationKey == Id && x.EducationQualificationKey == scr.EducationQualificationKey).Select(row => new ReturnCertificateDetails
                                                       {
                                                           CertificateStatusName = (row.CertificateStatusKey == DbConstants.CertificateProcessType.Received ? EduSuiteUIResources.Recieved : (row.CertificateStatusKey == DbConstants.CertificateProcessType.Returned && row.IsTempReturn == true ? EduSuiteUIResources.TempReturned : (row.CertificateStatusKey == DbConstants.CertificateProcessType.Returned && row.EducationQualification.IsOriginalReturn == true ? EduSuiteUIResources.Returned : EduSuiteUIResources.Verified))),
                                                           CertificateStatusBy = dbContext.AppUsers.Where(x => x.RowKey == row.IssuedBy).Select(y => y.AppUserName).FirstOrDefault(),
                                                           CertificatestatusDate = row.IssuedDate,
                                                       }).ToList()
                                                   }).ToList();

            model.StudentsCertificateReturnDetail = StudentsCertificateReturnDetail.GroupBy(x => x.EducationQualificationKey).Select(y => y.FirstOrDefault()).ToList();


            #endregion Educational Details

            #region Fee Details

            model.FeePyamentDetails = dbContext.FeePaymentMasters.Where(row => row.ApplicationKey == Id).Select(row => new ApplicationFeePaymentViewModel
            {
                RecieptNo = row.ReceiptNo,
                FeeDate = row.FeeDate,
                PaymentModeName = row.PaymentMode.PaymentModeName,
                PaymentModeKey = row.PaymentModeKey,
                PaymentModeSubName = row.PaymentModeSub.PaymentModeSubName,
                PaymentModeSubKey = row.PaymentModeSubKey,
                //CardNumber = row.CardNumber,
                BankAccountName = row.BankAccount.Bank.BankName,
                FeeDescription = row.FeeDescription,
                ChequeClearanceDate = row.ChequeClearanceDate,
                ChequeOrDDNumber = row.ChequeOrDDNumber,
                RowKey = row.RowKey,
                ChequeStatusKey = row.ChequeStatusKey,
                ChequeAction = row.ChequeStatusKey == null ? "" : (row.ChequeStatusKey == DbConstants.ProcessStatus.Approved ? EduSuiteUIResources.Approved : EduSuiteUIResources.Rejected),
                ChequeApprovedRejectedDate = row.ChequeApprovedRejectedDate,
                ChequeRejectedRemarks = row.ChequeRejectedRemarks,
                ApplicationFeePaymentDetails = row.FeePaymentDetails.Select(x => new ApplicationFeePaymentDetailViewModel
                {

                    FeeTypeKey = x.FeeTypeKey,
                    FeeAmount = x.FeeAmount,
                    FeeYear = x.FeeYear,
                    FeeTypeName = x.FeeType.FeeTypeName,
                    RowKey = x.RowKey

                }).ToList(),
                IsRefunded = row.FeePaymentDetails.Where(y => dbContext.FeeRefundDetails.Where(p => p.FeeRefundMaster.ProcessStatus != DbConstants.ProcessStatus.Rejected).Select(x => x.FeePaymentDetailKey).ToList().Contains(y.RowKey)).Any(),


            }).ToList();


            foreach (ApplicationFeePaymentViewModel PaymentDetails in model.FeePyamentDetails)
            {
                foreach (ApplicationFeePaymentDetailViewModel item in PaymentDetails.ApplicationFeePaymentDetails)
                {
                    item.FeeYearText = item.FeeYear != null ? (CommonUtilities.GetYearDescriptionByCodeDetails(application.Course.CourseDuration ?? 0, item.FeeYear ?? 0, application.AcademicTermKey)) : "";
                }
            }
            model.TotalPaid = dbContext.FeePaymentDetails.Where(row => row.FeePaymentMaster.ApplicationKey == Id).Select(row => row.FeeAmount).DefaultIfEmpty().Sum();
            model.BalanceFee = (model.PersonalDetails.TotalFeeAmount ?? 0) - (model.TotalPaid);

            #endregion Fee Details

            #region Call History Details

            model.CallHistory = ((from r in dbContext.EnquiryFeedbacks
                                  join AU in dbContext.AppUsers on r.AddedBy equals AU.RowKey
                                  where (r.Enquiry.MobileNumber == application.StudentMobile && r.Enquiry.RowKey == application.EnquiryKey)
                                  select new EnquiryScheduleViewModel
                                  {
                                      Feedback = r.EnquiryFeedbackDesc,
                                      MobileNumber = r.Enquiry.MobileNumber,
                                      CallStatusName = r.EnquiryCallStatu.EnquiryCallStatusName,
                                      Name = r.Enquiry.EnquiryName,
                                      //AcademicTermName = r.Enquiry.AcademicTerm.AcademicTermName,
                                      CallDuration = r.EnquiryDuration,
                                      FeedbackCreatedDate = r.DateAdded,
                                      NextCallScheduleDate = r.EnquiryFeedbackReminderDate,
                                      CreatedBy = AU.FirstName + " " + (AU.MiddleName ?? "") + " " + AU.LastName,
                                      LocationName = r.Enquiry.LocationName,
                                      EnquiryStatusKey = r.Enquiry.EnquiryStatusKey ?? 0,
                                      ScheduleTypeName = EduSuiteUIResources.Enquiry,
                                      //TelephoneCodeKey = r.Enquiry.TelephoneCodeKey,
                                      BranchName = r.Enquiry.Branch.BranchName

                                  }).Union(from r in dbContext.EnquiryLeadFeedbacks
                                           join AU in dbContext.AppUsers on r.AddedBy equals AU.RowKey
                                           where (r.EnquiryLead.MobileNumber == application.StudentMobile && r.EnquiryLead.Enquiry.RowKey == application.EnquiryKey)
                                           select new EnquiryScheduleViewModel
                                           {
                                               Feedback = r.Feedback,
                                               MobileNumber = r.EnquiryLead.MobileNumber,
                                               CallStatusName = r.EnquiryCallStatu.EnquiryCallStatusName,
                                               Name = r.EnquiryLead.Name,
                                               //AcademicTermName = r.EnquiryLead.AcademicTerm.AcademicTermName,
                                               CallDuration = r.CallDuration,
                                               FeedbackCreatedDate = r.DateAdded,
                                               NextCallScheduleDate = r.NextCallSchedule,
                                               CreatedBy = AU.FirstName + " " + (AU.MiddleName ?? "") + " " + AU.LastName,
                                               LocationName = "",
                                               EnquiryStatusKey = r.EnquiryLead.EnquiryLeadStatusKey ?? 0,
                                               ScheduleTypeName = EduSuiteUIResources.EnquiryLead,
                                               //TelephoneCodeKey = r.EnquiryLead.TelephoneCodeKey,
                                               BranchName = r.EnquiryLead.Branch.BranchName

                                           })).OrderByDescending(x => x.FeedbackCreatedDate).ToList();

            #endregion Call History Details

            #region StudyMaterials


            model.StudyMaterials = (from A in dbContext.F_StudentBooks(Id).Where(x => x.HasStudyMaterial)
                                    join B in dbContext.IssueOfStudyMaterials.AsEnumerable()
                                    on new { StudyMaterialKey = A.RowKey, A.ApplicationKey } equals new { B.StudyMaterialKey, B.ApplicationKey } into B
                                    from A1 in B.DefaultIfEmpty()
                                    select new StudyMaterialDetailsModel
                                    {
                                        StudyMaterialName = A.StudyMaterialName,
                                        StudyMaterialCode = A.StudyMaterialCode,
                                        SubjectType = A1.StudyMaterial.Subject.IsElective == true ? EduSuiteUIResources.Elective : EduSuiteUIResources.Compulsory,
                                        SubjectYear = A.CourseYear,
                                        IsAvailable = A1.IsAvailable != null ? A1.IsAvailable : false,
                                        IsIssued = A1.IsIssued != null ? A1.IsIssued : false,
                                        IssuedDate = A1.IssuedDate,
                                        AvailableDate = A1.AvailableDate,
                                        IssuedBy = A1.IssuedBy,
                                        StudyMaterialStatusBy = dbContext.AppUsers.Where(x => x.RowKey == A1.IssuedBy).Select(y => y.AppUserName).FirstOrDefault(),
                                        IssuedByText = dbContext.AppUsers.Where(x => x.RowKey == A1.IssuedBy).Select(y => y.FirstName).FirstOrDefault(),
                                        AvailableByText = dbContext.AppUsers.Where(x => x.RowKey == A1.AvailableBy).Select(y => y.FirstName).FirstOrDefault()
                                    }).OrderBy(x => x.SubjectYear).ToList();

            foreach (StudyMaterialDetailsModel MaterialList in model.StudyMaterials)
            {
                MaterialList.SubjectYearText = CommonUtilities.GetYearDescriptionByCodeDetails(application.Course.CourseDuration ?? 0, MaterialList.SubjectYear ?? 0, application.AcademicTermKey);
            }

            #endregion StudyMaterials

            #region Documant Details

            model.DocumentDetails = dbContext.StudentDocuments.Where(row => row.ApplicationKey == Id).Select(row => new DocumentViewModel
            {
                DocumentTypeName = row.StudentDocumentName,
                //StudentDocumentPath = row.StudentDocumentPath != null ? UrlConstants.ApplicationUrl + row.Application.AdmissionNo + "/Document/" + row.StudentDocumentPath : row.StudentDocumentPath,
                StudentDocumentPath = row.OldDocumentPath != null ? row.OldDocumentPath : UrlConstants.ApplicationUrl + row.Application.RowKey + "/Document/" + row.StudentDocumentPath,

                StudentDocumentName = row.StudentDocumentPath
            }).ToList();

            #endregion Documant Details

            #region University Fee Details


            model.UniversityPyamentDetails = dbContext.UniversityPaymentMasters.Where(row => row.ApplicationKey == Id).Select(row => new UniversityPaymentViewmodel
            {

                PaymentModeName = row.PaymentMode.PaymentModeName,
                PaymentModeKey = row.PaymentModeKey,
                PaymentModeSubKey = row.PaymentModeSubKey,
                PaymentModeSubName = row.PaymentModeSub.PaymentModeSubName,
                //CardNumber = row.CardNumber,
                BankAccountName = row.BankAccount.Bank.BankName,
                RowKey = row.RowKey,
                ApplicationKey = row.ApplicationKey,
                UniversityPaymentDate = row.UniversityPaymentDate,
                VoucherNo = row.VoucherNo,
                ChequeOrDDNumber = row.ChequeOrDDNumber,
                ChequeClearanceDate = row.ChequeClearanceDate,
                FeeDescription = row.UniversityPaymentNote,
                BankAccountKey = row.BankAccountKey ?? 0,
                ChequeStatusKey = row.ChequeStatusKey,
                ChequeAction = row.ChequeStatusKey == null ? "" : (row.ChequeStatusKey == DbConstants.ProcessStatus.Approved ? EduSuiteUIResources.Approved : EduSuiteUIResources.Rejected),
                ChequeApprovedRejectedDate = row.ChequeApprovedRejectedDate,
                ChequeRejectedRemarks = row.ChequeRejectedRemarks,
                UniversityFeePaymentDetails = row.UniversityPaymentDetails.Select(y => new UniversityPaymentDetailsmodel
                {
                    RowKey = y.RowKey,
                    FeeTypeKey = y.FeeTypeKey,
                    UniversityPaymentAmount = y.UniversityPaymentAmount,
                    UniversityPaymentYear = y.UniversityPaymentYear,
                    FeeTypeName = y.FeeType.FeeTypeName,
                    CashFlowTypeKey = y.FeeType.CashFlowTypeKey,
                    IsFeeTypeYear = y.FeeType.FeeTypeModeKey == DbConstants.FeeTypeMode.Single ? true : false
                }).ToList(),
                IsCancel = row.UniversityPaymentDetails.Where(y => dbContext.UniversityPaymentCancelations.Select(x => x.UniversityPaymentDetailsKey).ToList().Contains(y.RowKey)).Any(),



            }).ToList();

            foreach (UniversityPaymentViewmodel PaymentDetails in model.UniversityPyamentDetails)
            {
                foreach (UniversityPaymentDetailsmodel item in PaymentDetails.UniversityFeePaymentDetails)
                {
                    item.FeeYearText = item.UniversityPaymentYear != null ? (CommonUtilities.GetYearDescriptionByCodeDetails(application.Course.CourseDuration ?? 0, item.UniversityPaymentYear ?? 0, application.AcademicTermKey)) : "";
                }
            }
            model.TotalPaid = dbContext.FeePaymentDetails.Where(row => row.FeePaymentMaster.ApplicationKey == Id).Select(row => row.FeeAmount).DefaultIfEmpty().Sum();
            model.BalanceFee = (model.PersonalDetails.TotalFeeAmount ?? 0) - (model.TotalPaid);

            #endregion University Fee Details

            #region Student Id Card Details

            model.StudentIDCardDetails = dbContext.StudentIDCards.Where(row => row.ApplicationKey == Id).Select(row => new StudentIDCardList
            {
                StudentEnrollmentNo = row.Application.StudentEnrollmentNo,
                IsReceived = row.IsReceived ?? false,
                ReceivedBy = dbContext.AppUsers.Where(x => x.RowKey == row.ReceivedBy).Select(y => y.AppUserName).FirstOrDefault(),
                ReceivedDate = row.ReceivedDate,
                IsIssued = row.IsIssued ?? false,
                IssuedBy = dbContext.AppUsers.Where(x => x.RowKey == row.IssuedBy).Select(y => y.AppUserName).FirstOrDefault(),
                IssuedDate = row.IssuedDate,
            }).ToList();


            #endregion Student Id Card Details

            #region University Certificate Details

            model.UniversityCertificateDetails = (from scr in dbContext.UniversityCertificates.Where(x => x.ApplicationKey == Id)
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
                                                      IssuedDate = scr.IssuedDate,
                                                      CertificatePath = UrlConstants.ApplicationUrl + scr.Application.RowKey + "/AffiliatedCertificates/" + scr.DocumentPath,

                                                      CertificatePathText = scr.DocumentPath
                                                  }).ToList();


            #endregion University Certificate Details

            #region Exam Schedule Details
            model.ExamScheduleSummary = (from row in dbContext.Sp_StudentExamScheduleDetails_Summary(Id)
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
                                         }).OrderBy(x => x.SubjectYear).ToList();
            foreach (ExamScheduleSummary objmodel in model.ExamScheduleSummary)
            {
                objmodel.SubjectYearName = CommonUtilities.GetYearDescriptionByCodeDetails(objmodel.CourseDuration ?? 0, objmodel.SubjectYear ?? 0, objmodel.AcademicTermKey ?? 0);

            }
            #endregion Exam Schedule Details

            #region Fill CourseYear
            //Application Application = dbContext.Applications.SingleOrDefault(row => row.RowKey == Id);

            if (application != null)
            {
                var CourseDuration = application.Course.CourseDuration;

                var duration = Math.Ceiling((Convert.ToDecimal(application.AcademicTermKey == DbConstants.AcademicTerm.Semester ? CourseDuration / 6 : CourseDuration / 12)));

                var StartYear = application.StartYear ?? 0;
                if (duration < 1)
                {
                    model.CourseYears.Add(new SelectListModel
                    {
                        RowKey = 1,
                        Text = " Short Term"
                    });
                }
                else
                {
                    for (int i = StartYear; i <= duration; i++)
                    {
                        model.CourseYears.Add(new SelectListModel
                        {
                            RowKey = i,
                            Text = i + (application.AcademicTermKey == DbConstants.AcademicTerm.Semester ? " Semester" : " Year")
                        });
                    }
                }
            }
            #endregion Fill CourseYear

            #region Student Leave Details

            model.StudentLeaveDetail = (from es in dbContext.StudentLeaveDetails.Where(row => row.ApplicationKey == Id)// && row.ClassDetailsKey == (application.ClassDetailsKey != null ? application.ClassDetailsKey : 0))
                                        orderby es.RowKey ascending
                                        select new StudentLeaveViewModel
                                        {
                                            RowKey = es.RowKey,
                                            LeaveDateFrom = es.LeaveDateFrom,
                                            LeaveDateTo = es.LeaveDateTo,
                                            Remarks = es.Remarks,
                                            AttachmentPath = es.AttachmentPath,
                                            ApplicationsKey = es.ApplicationKey,
                                            ClassCode = es.ClassDetail.ClassCode
                                        }).ToList();

            #endregion Student Leave Details

            #region Student Absconders Details

            model.StudentAbscondersDetails = (from es in dbContext.StudentAbsconders.Where(row => row.ApplicationKey == Id)//&& row.ClassDetailsKey == (application.ClassDetailsKey != null ? application.ClassDetailsKey : 0))
                                              orderby es.RowKey ascending
                                              select new StudentAbscondersViewModel
                                              {
                                                  RowKey = es.RowKey,
                                                  AbscondersDate = es.AbscondersDate,
                                                  IsAbsconders = es.IsAbsconders ?? false,
                                                  Remarks = es.Remarks,
                                                  AttachmentPath = es.AttachmentPath,
                                                  ApplicationsKey = es.ApplicationKey,
                                                  ClassCode = es.ClassDetail.ClassCode
                                              }).ToList();

            #endregion Student Absconders Details

            #region Student Late Details

            model.StudentLateDetails = (from es in dbContext.StudentLateDetails.Where(row => row.ApplicationKey == Id)// && row.ClassDetailsKey == (application.ClassDetailsKey != null ? application.ClassDetailsKey : 0))
                                        orderby es.RowKey ascending
                                        select new StudentLateViewModel
                                        {
                                            RowKey = es.RowKey,
                                            LateDate = es.LateDate,
                                            LateMinutes = es.LateMinutes,
                                            Remarks = es.Remarks,
                                            AttachmentPath = es.AttachmentPath,
                                            ApplicationsKey = es.ApplicationKey,
                                            ClassCode = es.ClassDetail.ClassCode
                                        }).ToList();

            #endregion Student Late Details

            #region Student Early Depature Details

            model.StudentEarlyDepartureDetails = (from es in dbContext.StudentEarlyDepartures.Where(row => row.ApplicationKey == Id && row.ClassDetailsKey == (application.ClassDetailsKey != null ? application.ClassDetailsKey : 0))
                                                  orderby es.RowKey ascending
                                                  select new StudentEarlyDepartureViewModel
                                                  {
                                                      RowKey = es.RowKey,
                                                      EarlyDepartureDate = es.EarlyDepartureDate,
                                                      EarlyDepartureTime = es.EarlyDepartureTime,
                                                      Remarks = es.Remarks,
                                                      AttachmentPath = es.AttachmentPath,
                                                      ApplicationsKey = es.ApplicationKey,
                                                      ClassCode = es.ClassDetail.ClassCode
                                                  }).ToList();

            #endregion Student Early Depature Details

            #region Student Diary Details

            model.StudentDiaryDetails = (from es in dbContext.StudentDiaries.Where(row => row.ApplicationKey == Id)// && row.ClassDetailsKey == (application.ClassDetailsKey != null ? application.ClassDetailsKey : 0))
                                         orderby es.RowKey ascending
                                         select new StudentDiaryViewModel
                                         {
                                             RowKey = es.RowKey,
                                             StudentDiaryDate = es.StudentDiaryDate,
                                             Subject = es.Subject,
                                             Remarks = es.Remarks,
                                             AttachmentPath = es.AttachmentPath,
                                             ApplicationsKey = es.ApplicationKey,
                                             ClassCode = es.ClassDetail.ClassCode
                                         }).ToList();

            #endregion Student Diary Details

            #region Unit Test Details

            model.UnitTestResultDetails = dbContext.UnitTestResults.Where(x => x.ApplicationKey == Id && x.UnitTestSchedule.ClassDetailsKey == (application.ClassDetailsKey != null ? application.ClassDetailsKey : 0)).Select(x => new UnitTestResultViewModel
            {
                RowKey = x.RowKey,
                UnitTestScheduleKey = x.UnitTestScheduleKey,
                ApplicationKey = x.Application.RowKey,
                RollNoCode = x.Application.RollNoCode,
                StudentName = x.Application.StudentName,
                AdmissionNo = x.Application.AdmissionNo,
                ApplicantPhoto = x.Application.StudentPhotoPath,
                ResultStatus = x.ResultStatus,
                Mark = x.Mark,
                Remarks = x.Remarks,
                AbsentStatus = (x.ResultStatus == DbConstants.ResultStatus.Absent ? true : false),
                SubjectName = x.UnitTestSchedule.Subject.SubjectName,
                MaximumMark = x.UnitTestSchedule.MaximumMark,
                MinimumMark = x.UnitTestSchedule.MinimumMark,
                ExamDate = x.UnitTestSchedule.ExamDate
            }).ToList();

            #endregion Unit Test Details---------

            #region Fee Followup Details

            model.FeeFollowupDetails = dbContext.ApplicationFeeFollowups.Where(x => x.ApplicationKey == Id)
                        .Select(row => new ApplicationFeeFollowupDetailsViewModel
                        {
                            RowKey = row.RowKey,
                            ApplicationKey = row.ApplicationKey,
                            FollowupDate = row.FollowupDate,
                            Remarks = row.Remarks,
                            ProcessStatusKey = row.ProcessStatusKey,
                            ProcessStatusName = row.ProcessStatusKey == DbConstants.ProcessStatus.Approved ? "Completed" : row.ProcessStatu.ProcessStatusName,
                            ApplicationFeeFollowupFeeTypes = dbContext.FeeFollowUpFeeTypes.Where(y => y.ApplicationFeeFollowUpKey == row.RowKey).Select(p => new ApplicationFeeFollowupFeeTypes { FeeTypeName = p.FeeType.FeeTypeName }).ToList(),
                            ApplicationFeeFollowupExtendDates = dbContext.ExtendFeeFollowUpDates.Where(y => y.ApplicationFeeFollowupKey == row.RowKey).Select(p => new ApplicationFeeFollowupExtendDates
                            {
                                OldFollowUpDate = p.OldFollowUpDate,
                                NextFollowUpDate = p.NextFollowUpDate,
                                DateAdded = p.DateAdded,
                                AddedBy = dbContext.AppUsers.Where(j => j.RowKey == p.AddedBy).Select(x => x.AppUserName).FirstOrDefault()
                            }).ToList(),

                        }).ToList();

            #endregion Fee Followup Details
            return model;

        }
        public ApplicationViewModel ViewApplicationById(long Id)
        {
            ApplicationViewModel model = new ApplicationViewModel();

            var application = dbContext.Applications.SingleOrDefault(x => x.RowKey == Id);
            model.RowKey = application.RowKey;
            //model.IsTax = application.IsTax;


            Application NextApplication = dbContext.Applications.Where(x => x.ClassDetailsKey == application.ClassDetailsKey && x.RollNumber > application.RollNumber).OrderBy(y => y.RollNumber).FirstOrDefault();
            Application PrevApplication = dbContext.Applications.Where(x => x.ClassDetailsKey == application.ClassDetailsKey && x.RollNumber < application.RollNumber).OrderByDescending(y => y.RollNumber).FirstOrDefault();
            if (NextApplication != null)
            {
                model.NextApplicationAdmissionNo = NextApplication.AdmissionNo;
                model.NextApplicationKey = NextApplication.RowKey;
                model.NextApplicationRollNoCode = NextApplication.RollNoCode;
                model.NextApplicationName = NextApplication.StudentName;
            }
            if (PrevApplication != null)
            {
                model.PrevApplicationAdmissionNo = PrevApplication.AdmissionNo;
                model.PrevApplicationKey = PrevApplication.RowKey;
                model.PrevApplicationRollNoCode = PrevApplication.RollNoCode;
                model.PrevApplicationName = PrevApplication.StudentName;
            }
            return model;

        }

        #region Attendance and Academic Perfomance
        public List<List<dynamic>> AttendanceAcademicPerfomance(long Applicationkey)
        {
            var AttendanceAcademicePerfomance = new List<List<dynamic>>();
            AttendanceAcademicePerfomance = dbContext.CollectionFromSqlSets("Sp_AttendnaceAcadamic_Perfomance",
             new Dictionary<string, object>() { {"ApplicationKey", Applicationkey }
                                                                                    }).ToList();
            return AttendanceAcademicePerfomance;
        }

        #endregion Attendance and Academice Perfomance

        public IQueryable<EnquiryViewModel> GetInterestedEnquiry(EnquiryViewModel model)
        {
            try
            {
                List<string> ApplicationWebFormList = dbContext.ApplicationWebForms.Select(x => x.StudentMobile).ToList();

                IQueryable<EnquiryViewModel> EnquiryList = null;
                if (!DbConstants.Role.AdminUserTypes.Contains(DbConstants.User.RoleKey))
                {
                    EnquiryList = (from E in dbContext.Enquiries.Where(row => row.EnquiryStatusKey == DbConstants.EnquiryStatus.Intersted && !ApplicationWebFormList.Contains(row.MobileNumber))
                                   join EM in dbContext.Employees.Where(row => row.AppUserKey == DbConstants.User.UserKey) on E.BranchKey equals EM.BranchKey
                                   where (E.EnquiryName.Contains(model.EnquiryName) || (E.MobileNumber.Contains(model.EnquiryName)))
                                   select new EnquiryViewModel
                                   {
                                       RowKey = E.RowKey,
                                       BranchKey = E.BranchKey,
                                       EnquiryName = E.EnquiryName,
                                       PhoneNumber = E.MobileNumber,
                                       EmailAddress = E.EmailAddress != null ? E.EmailAddress : EduSuiteUIResources.NA,
                                       AcademicTermName = E.AcademicTerm.AcademicTermName,
                                       CourseName = E.Course.CourseName,
                                       UniversityName = E.UniversityMaster.UniversityMasterName,
                                       NextCallSchedule = E.EnquiryFeedbacks.GroupBy(x => x.EnquiryKey).Select(y => y.OrderByDescending(row => row.AddedBy).Select(row => row.EnquiryFeedbackReminderDate).FirstOrDefault()).FirstOrDefault(),
                                       EmployeeName = E.Employee.FirstName + " " + (E.Employee.MiddleName ?? "") + " " + E.Employee.LastName,
                                       LocationName = E.LocationName,
                                       DistrictName = E.DistrictName,
                                       isCounsellingCompleted = dbContext.EnquiryFeedbacks.Where(x => x.EnquiryKey == E.RowKey && x.EnquiryCallStatusKey == DbConstants.EnquiryCallStatus.CounsellingCompleted && x.EnquiryFeedbackReminderStatus == true).Any()

                                   });
                    Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
                    if (Employee != null)
                    {

                        List<long> branches = Employee.BranchAccess.ToString().Split(',').Select(Int64.Parse).ToList();
                        EnquiryList = EnquiryList.Where(row => branches.Contains(row.BranchKey));
                    }
                }
                else
                {



                    EnquiryList = (from E in dbContext.Enquiries.Where(row => row.EnquiryStatusKey == DbConstants.EnquiryStatus.Intersted && !ApplicationWebFormList.Contains(row.MobileNumber))
                                   where (E.EnquiryName.Contains(model.EnquiryName) || (E.PhoneNumber.Contains(model.EnquiryName)))
                                   select new EnquiryViewModel
                                   {
                                       RowKey = E.RowKey,
                                       BranchKey = E.BranchKey,
                                       BranchName = E.Branch.BranchName,
                                       EnquiryName = E.EnquiryName,
                                       PhoneNumber = E.MobileNumber,
                                       EmailAddress = E.EmailAddress,
                                       AcademicTermName = E.AcademicTerm.AcademicTermName,
                                       CourseName = E.Course.CourseName,
                                       UniversityName = E.UniversityMaster.UniversityMasterName,
                                       NextCallSchedule = E.EnquiryFeedbacks.GroupBy(x => x.EnquiryKey).Select(y => y.OrderByDescending(row => row.AddedBy).Select(row => row.EnquiryFeedbackReminderDate).FirstOrDefault()).FirstOrDefault(),
                                       EmployeeName = E.Employee.FirstName + " " + (E.Employee.MiddleName ?? "") + " " + E.Employee.LastName,
                                       LocationName = E.LocationName,
                                       DistrictName = E.DistrictName,
                                       isCounsellingCompleted = dbContext.EnquiryFeedbacks.Where(x => x.EnquiryKey == E.RowKey && x.EnquiryCallStatusKey == DbConstants.EnquiryCallStatus.CounsellingCompleted && x.EnquiryFeedbackReminderStatus == true).Any()

                                   });
                }
                EnquiryList = EnquiryList.Where(x => x.isCounsellingCompleted == true);
                return EnquiryList;

            }
            catch (Exception Ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Application, ActionConstants.View, DbConstants.LogType.Error, null, Ex.GetBaseException().Message);
                return null;

            }

        }

        public ApplicationEnRollmentNoViewModel GetEnrollmentNo(ApplicationEnRollmentNoViewModel model, List<long> ApplicationKeys)
        {
            FillApplicationStatus(model);
            model.EnRollmentNoDetailsViewModel = dbContext.Applications.Where(x => ApplicationKeys.Contains(x.RowKey)).Select(Row => new EnRollmentNoDetailsViewModel
            {
                ApplicationKey = Row.RowKey,
                ApplicantName = Row.StudentName,
                AdmissionNo = Row.AdmissionNo,
                StudentEnrollmentNo = Row.StudentEnrollmentNo,
                ExamRegisterNo = Row.ExamRegisterNo,
                ApplicationStatusKey = Row.StudentStatusKey,
                CurrentYear = Row.CurrentYear,
                ClassDetailsKey = Row.ClassDetailsKey,
                RollNoCode = Row.RollNoCode,
                RollNumber = Row.RollNumber,
                ClassDetails = dbContext.VwClassDetailsSelectActiveOnlies.Where(x => x.AcademicTermKey == Row.AcademicTermKey && x.CourseKey == Row.CourseKey
                  && x.UniversityMasterKey == Row.UniversityMasterKey && x.StudentYear == (Row.CurrentYear)).Select(x => new SelectListModel
                  {
                      RowKey = x.RowKey,
                      Text = x.ClassCode + x.ClassCodeDescription
                  }).Distinct().ToList(),
                IsClass = model.IsClass

            }).ToList();

            if (model.EnRollmentNoDetailsViewModel.Count == 0)
            {
                model.EnRollmentNoDetailsViewModel.Add(new EnRollmentNoDetailsViewModel());
            }
            if (model == null)
            {
                model = new ApplicationEnRollmentNoViewModel();
            }

            return model;
        }

        private void StudentList(ApplicationEnRollmentNoViewModel model)
        {

            model.StudentList = dbContext.Applications.Select(Row => new SelectListModel
            {
                RowKey = Row.RowKey,
                Text = Row.StudentName
            }).ToList();


        }

        public ApplicationEnRollmentNoViewModel GetStudentDetailsByStudentKey(long Applicationkey)
        {
            ApplicationEnRollmentNoViewModel model = new ApplicationEnRollmentNoViewModel();
            FillStudentDetailsById(model, Applicationkey);

            return model;
        }

        private void FillStudentDetailsById(ApplicationEnRollmentNoViewModel model, long Applicationkey)
        {

            var StudentDetailsList = dbContext.Applications.Where(x => x.RowKey == Applicationkey).Select(Row => new StudentDetailsModel
            {
                ApplicationKey = Row.RowKey,
                ApplicantName = Row.StudentName,
                AdmissionNo = Row.AdmissionNo,
                StudentEnrollmentNo = Row.StudentEnrollmentNo,
                ExamRegisterNo = Row.ExamRegisterNo
            }).ToList();
            model.StudentDetailsModel = StudentDetailsList.GroupBy(x => x.ApplicationKey).Distinct().Select(x => x.First()).ToList();

        }

        public ApplicationEnRollmentNoViewModel UpdateEnrollmentNo(ApplicationEnRollmentNoViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    foreach (EnRollmentNoDetailsViewModel ObjViewModel in model.EnRollmentNoDetailsViewModel)
                    {
                        Application applicationModel = dbContext.Applications.SingleOrDefault(x => x.RowKey == ObjViewModel.ApplicationKey);
                        if (model.IsClass)
                        {
                            if (applicationModel.StudentClassRequired && applicationModel.ClassDetailsKey != null)
                            {
                                if (applicationModel.ClassDetailsKey != ObjViewModel.ClassDetailsKey)
                                {
                                    StudentDivisionAllocation studentDivisionAllocation = dbContext.StudentDivisionAllocations.SingleOrDefault(p => p.ClassDetailsKey == applicationModel.ClassDetailsKey && p.ApplicationKey == applicationModel.RowKey && p.IsActive);

                                    if (studentDivisionAllocation != null)
                                    {
                                        StudentDivisionAllocation studentDivisionAllocationmodel = dbContext.StudentDivisionAllocations.SingleOrDefault(p => p.RowKey == studentDivisionAllocation.RowKey);
                                        //studentDivisionAllocationmodel.IsActive = false;
                                        //studentDivisionAllocationmodel.Remarks = ObjViewModel.ClassRemarks;
                                        ObjViewModel.ClassRemarks = ObjViewModel.ClassRemarks + ": Old Class is  " + studentDivisionAllocationmodel.ClassDetail.ClassCode + "( ClassDetailsKey key = " + studentDivisionAllocationmodel.ClassDetailsKey + ")";

                                        dbContext.StudentDivisionAllocations.Remove(studentDivisionAllocationmodel);
                                        ObjViewModel.RollNumber = null;
                                        applicationModel.ClassDetailsKey = null;
                                        applicationModel.RollNumber = null;
                                        applicationModel.RollNoCode = null;

                                    }
                                }
                            }

                            if (ObjViewModel.ClassDetailsKey != null && (ObjViewModel.RollNumber == 0 || ObjViewModel.RollNumber == null))
                            {
                                int RollNumber = 0;

                                RollNumber = dbContext.StudentDivisionAllocations.Where(row => row.Application.BatchKey == applicationModel.BatchKey && row.Application.CurrentYear == applicationModel.CurrentYear &&
                                                  row.Application.BranchKey == applicationModel.BranchKey && row.Application.StudentStatusKey == DbConstants.ApplicationStatus.OnGoing && row.ClassDetailsKey == ObjViewModel.ClassDetailsKey && row.IsActive)
                                                  .Select(p => p.RollNumber).DefaultIfEmpty().Max();

                                applicationModel.ClassDetailsKey = ObjViewModel.ClassDetailsKey;
                                applicationModel.RollNumber = RollNumber + 1;
                                applicationModel.RollNoCode = dbContext.Database.SqlQuery<string>("select dbo.F_GenerateRollNoCode(" + ObjViewModel.ClassDetailsKey + "," + RollNumber + ")").Single().ToString();
                                CreateDivisionAllocation(applicationModel.RowKey, applicationModel.RollNumber, applicationModel.ClassDetailsKey, applicationModel.RollNoCode, applicationModel.BatchKey, ObjViewModel.ClassRemarks);
                            }
                        }
                        else
                        {
                            applicationModel.StudentEnrollmentNo = ObjViewModel.StudentEnrollmentNo;
                            applicationModel.ExamRegisterNo = ObjViewModel.ExamRegisterNo;
                            applicationModel.StudentStatusKey = ObjViewModel.ApplicationStatusKey;
                            applicationModel.CurrentYear = ObjViewModel.CurrentYear ?? 0;
                        }

                    }
                    dbContext.SaveChanges();
                    transaction.Commit();
                    FillApplicationStatus(model);
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationPersenol, ActionConstants.Edit, DbConstants.LogType.Info, null, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.EnrollmentNo);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationPersenol, ActionConstants.Edit, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                }
                return model;
            }
        }
        private void CreateDivisionAllocation(long ApplicationKey, int? RollNumber, long? ClassDetailsKey, string RollNoCode, short? BatchKey, string ClassRemarks)
        {
            long MaxKey = dbContext.StudentDivisionAllocations.Select(p => p.RowKey).DefaultIfEmpty().Max();

            StudentDivisionAllocation studentDivisionAllocationmodel = new StudentDivisionAllocation();
            studentDivisionAllocationmodel.RowKey = MaxKey + 1;
            studentDivisionAllocationmodel.ApplicationKey = ApplicationKey;
            studentDivisionAllocationmodel.RollNumber = RollNumber ?? 0;
            studentDivisionAllocationmodel.RollNoCode = RollNoCode;
            studentDivisionAllocationmodel.ClassDetailsKey = ClassDetailsKey ?? 0;
            studentDivisionAllocationmodel.BatchKey = BatchKey ?? 0;
            studentDivisionAllocationmodel.Remarks = ClassRemarks;

            studentDivisionAllocationmodel.IsActive = true;
            dbContext.StudentDivisionAllocations.Add(studentDivisionAllocationmodel);
            dbContext.SaveChanges();
        }

        public ApplicationPersonalViewModel FillSearchBatch(ApplicationPersonalViewModel model)
        {

            if (model.BranchKey != 0 && model.BranchKey != null)
            {
                model.Batches = (from p in dbContext.Applications
                                 join B in dbContext.VwBatchSelectActiveOnlies on p.BatchKey equals B.RowKey
                                 orderby B.RowKey
                                 //where (p.CourseKey == model.CourseKey && p.BranchKey == model.BranchKey && p.UniversityMasterKey == model.UniversityMasterKey)
                                 where (p.BranchKey == model.BranchKey)
                                 select new SelectListModel
                                 {
                                     RowKey = B.RowKey,
                                     Text = B.BatchName
                                 }).Distinct().ToList();
            }
            else
            {
                model.Batches = (from p in dbContext.Applications
                                 join B in dbContext.VwBatchSelectActiveOnlies on p.BatchKey equals B.RowKey
                                 orderby B.RowKey
                                 select new SelectListModel
                                 {
                                     RowKey = B.RowKey,
                                     Text = B.BatchName
                                 }).Distinct().ToList();
            }
            return model;
        }

        public ApplicationPersonalViewModel FillSearchCourse(ApplicationPersonalViewModel model)
        {

            if (model.BranchKey != 0 && model.BranchKey != null)
            {
                model.Courses = (from p in dbContext.Applications
                                 join B in dbContext.VwCourseSelectActiveOnlies on p.CourseKey equals B.RowKey
                                 orderby B.RowKey
                                 where (p.BranchKey == model.BranchKey)
                                 select new SelectListModel
                                 {
                                     RowKey = B.RowKey,
                                     Text = B.CourseName
                                 }).Distinct().ToList();
            }
            else
            {
                model.Courses = (from p in dbContext.Applications
                                 join B in dbContext.VwCourseSelectActiveOnlies on p.CourseKey equals B.RowKey
                                 orderby B.RowKey
                                 select new SelectListModel
                                 {
                                     RowKey = B.RowKey,
                                     Text = B.CourseName
                                 }).Distinct().ToList();
            }
            return model;
        }
        public ApplicationPersonalViewModel FillSearchUniversity(ApplicationPersonalViewModel model)
        {

            if (model.BranchKey != 0 && model.BranchKey != null)
            {
                model.Universities = (from p in dbContext.Applications
                                      join B in dbContext.VwUniversityMasterSelectActiveOnlies on p.UniversityMasterKey equals B.RowKey
                                      orderby B.RowKey
                                      //where (p.CourseKey == model.CourseKey && p.BranchKey == model.BranchKey && p.UniversityMasterKey == model.UniversityMasterKey)
                                      where (p.BranchKey == model.BranchKey)
                                      select new SelectListModel
                                      {
                                          RowKey = B.RowKey,
                                          Text = B.UniversityMasterName
                                      }).Distinct().ToList();
            }
            else
            {
                model.Universities = (from p in dbContext.Applications
                                      join B in dbContext.VwUniversityMasterSelectActiveOnlies on p.UniversityMasterKey equals B.RowKey
                                      orderby B.RowKey
                                      select new SelectListModel
                                      {
                                          RowKey = B.RowKey,
                                          Text = B.UniversityMasterName
                                      }).Distinct().ToList();
            }
            return model;
        }

        private void FillApplicationStatus(ApplicationEnRollmentNoViewModel model)
        {
            model.ApplicationStatus = dbContext.StudentStatus.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.StudentStatusName
            }).ToList();
        }

        public ApplicationPersonalViewModel GetApplicationPhoneNo(long Id)
        {
            try
            {
                ApplicationPersonalViewModel model = new ApplicationPersonalViewModel();
                model = dbContext.Applications.Select(row => new ApplicationPersonalViewModel
                {
                    RowKey = row.RowKey,
                    MobileNumber = row.StudentMobile
                }).Where(x => x.RowKey == Id).FirstOrDefault();
                if (model == null)
                {
                    model.RowKey = Id;
                }
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Application, ActionConstants.View, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);
                return new ApplicationPersonalViewModel();

            }
        }
        public ApplicationPersonalViewModel UpdateApplicantionPhoneNo(ApplicationPersonalViewModel model)
        {
            Application applicationModel = new Application();


            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    applicationModel = dbContext.Applications.SingleOrDefault(row => row.RowKey == model.RowKey);
                    applicationModel.StudentMobile = model.MobileNumber;

                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.AdmissionNo = applicationModel.AdmissionNo;
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Application, ActionConstants.AddEdit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.MobileNumber);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Application, ActionConstants.AddEdit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;
        }
    }
}
