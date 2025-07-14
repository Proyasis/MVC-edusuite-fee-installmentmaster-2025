using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using CITS.EduSuite.Business.Models.Security;
using System.Threading;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class EnquiryService : IEnquiryService
    {
        private EduSuiteDatabase dbContext;
        public EnquiryService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public List<EnquiryViewModel> GetEnquiries(EnquiryViewModel model, out int TotalRecords)
        {
            var Take = model.PageSize;
            var skip = (model.PageIndex - 1) * model.PageSize;
            Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();

            IQueryable<EnquiryViewModel> enquiryListQuery = (
                               from EL in dbContext.Enquiries.OrderByDescending(row => new { row.DateAdded })
                               join AU in dbContext.AppUsers on EL.AddedBy equals AU.RowKey

                               join ELF in dbContext.EnquiryFeedbacks.GroupBy(x => x.EnquiryKey).Select(y => y.OrderByDescending(row => row.DateAdded).FirstOrDefault())
                               on EL.RowKey equals ELF.EnquiryKey into ELF
                               from EJ in ELF.DefaultIfEmpty()
                               join AUF in dbContext.AppUsers on EJ.AddedBy equals AUF.RowKey into AUF
                               from AUJ in AUF.DefaultIfEmpty()

                               orderby EJ.DateAdded descending
                               where ((model.SearchName ?? "") == "" || EL.EnquiryName.Contains((model.SearchName ?? "").Trim()) || EL.EmailAddress.Contains((model.SearchName ?? "").Trim()) || EL.DistrictName.Contains((model.SearchName ?? "").Trim())
                                                                   || EL.LocationName.Contains((model.SearchName ?? "").Trim()) || EL.MobileNumber.Contains((model.SearchName ?? "").Trim()) || EL.MobileNumberOptional.Contains((model.SearchName ?? "").Trim()))

                               select new EnquiryViewModel
                               {
                                   RowKey = EL.RowKey,
                                   BranchKey = EL.BranchKey,
                                   //DepartmentKey = EL.DepartmentKey,
                                   EnquiryName = EL.EnquiryName,
                                   EmailAddress = EL.EmailAddress,
                                   BranchName = EL.Branch1.BranchName,
                                   EnquiryEducationQualification = EL.EnquiryEducationQualification,
                                   // MobileNumber = EL.Country1.TelephoneCode + EL.MobileNumber,
                                   MobileNumber = EL.MobileNumber,
                                   EnquiryStatusKey = EL.EnquiryStatusKey ?? 0,
                                   AcademicTermKey = EL.AcademicTermKey,
                                   AcademicTermName = EL.AcademicTerm.AcademicTermName,
                                   EnquiryFeedback = EJ.EnquiryFeedbackDesc,
                                   //DepartmentName = EL.Department.DepartmentName,
                                   CourseName = EL.Course.CourseName,
                                   UniversityName = EL.UniversityMaster.UniversityMasterName,
                                   LastCallScheduleDate = EJ.EnquiryFeedbackReminderDate,
                                   LastCallStatusKey = EJ.EnquiryCallStatusKey,
                                   EnquiryLastUpdatedDate = EJ.DateAdded,
                                   CreatedBy = AU.FirstName + " " + (AU.MiddleName ?? "") + " " + AU.LastName,
                                   LastUpdatedBy = AUJ.FirstName + " " + (AUJ.MiddleName ?? "") + " " + AUJ.LastName,
                                   CreatedOn = EL.DateAdded,
                                   AddedEmployeeKey = EL.AddedBy,
                                   EmployeeKey = EL.EmployeeKey ?? 0,
                                   EmployeeName = EL.Employee.FirstName + " " + (EL.Employee.MiddleName ?? "") + " " + EL.Employee.LastName,
                                   EnquiryCallStatusName = EJ.EnquiryCallStatu.EnquiryCallStatusName,
                                   IsProccessed = EL.IsProcessed,
                                   EnquiryStatusesName = EL.EnquiryStatu.EnquiryStatusName,
                                   IsEditable = true,
                                   CouncellingTime = EJ.CouncellingTime,
                                   DistrictName = EL.DistrictName,
                                   LocationName = EL.LocationName,
                                   NatureOfEnquiryKey = EL.NatureOfEnquiryKey

                               });

            if (Employee != null)
            {
                if (Employee.BranchAccess != null)
                {
                    var Branches = Employee.BranchAccess.Split(',').Select(Int16.Parse).ToList();
                    enquiryListQuery = enquiryListQuery.Where(row => Branches.Contains(row.BranchKey));
                }
            }

            if (model.SearchBranchKey != null)
                enquiryListQuery = enquiryListQuery.Where(row => row.BranchKey == model.SearchBranchKey);

            if (model.SearchCallStatusKey != null)
                enquiryListQuery = enquiryListQuery.Where(row => row.LastCallStatusKey == model.SearchCallStatusKey);

            if (model.SearchAcademicTermKey != null)
                enquiryListQuery = enquiryListQuery.Where(row => row.AcademicTermKey == model.SearchAcademicTermKey);

            if (model.SearchEnquiryStatusKey != null)
                enquiryListQuery = enquiryListQuery.Where(row => row.EnquiryStatusKey == model.SearchEnquiryStatusKey);
            else
                enquiryListQuery = enquiryListQuery.Where(row => row.EnquiryStatusKey != DbConstants.EnquiryStatus.AdmissionTaken);


            if (model.SearchFromDate != null && model.SearchToDate != null)
                enquiryListQuery = enquiryListQuery.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.CreatedOn) >= System.Data.Entity.DbFunctions.TruncateTime(model.SearchFromDate) && System.Data.Entity.DbFunctions.TruncateTime(row.CreatedOn) <= System.Data.Entity.DbFunctions.TruncateTime(model.SearchToDate));
            else if (model.SearchFromDate != null)
                enquiryListQuery = enquiryListQuery.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.CreatedOn) >= System.Data.Entity.DbFunctions.TruncateTime(model.SearchFromDate));
            else if (model.SearchToDate != null)
                enquiryListQuery = enquiryListQuery.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.CreatedOn) <= System.Data.Entity.DbFunctions.TruncateTime(model.SearchToDate));

            if (model.SearchEmployeeKey != null)
            {
                enquiryListQuery = enquiryListQuery.Where(row => row.EmployeeKey == model.SearchEmployeeKey);
            }
            if (model.NatureOfEnquiryKey != null)
            {
                enquiryListQuery = enquiryListQuery.Where(row => row.NatureOfEnquiryKey == model.NatureOfEnquiryKey);
            }

            //if (!DbConstants.Role.AdminUserTypes.Contains(DbConstants.User.RoleKey))
            //{
            //    Employee EmployeeSelected = dbContext.Employees.Where(x => x.RowKey == model.SearchEmployeeKey).SingleOrDefault();
            //    if (EmployeeSelected != null)
            //    {
            //        var OutBoxQUery = enquiryListQuery.Where(x => (x.AddedEmployeeKey == DbConstants.User.UserKey && x.IsProccessed == false));

            //        var InBoxQUery = (from E in enquiryListQuery
            //                          where (E.EmployeeKey == 0 && E.BranchKey == EmployeeSelected.BranchKey)
            //                          select E);

            //        var ProccessingQuery = (from E in enquiryListQuery

            //                                where (E.EmployeeKey == EmployeeSelected.RowKey)
            //                                select E);

            //        model.EnquiryOutboxCount = OutBoxQUery.Count().ToString();
            //        model.EnquiryInboxCount = InBoxQUery.Count().ToString();
            //        model.EnquiryProccessingCount = ProccessingQuery.Count().ToString();

            //        if (model.TabKey == 2)
            //            enquiryListQuery = OutBoxQUery;
            //        else if (model.TabKey == 1)
            //            enquiryListQuery = InBoxQUery;
            //        else if (model.TabKey == 0)
            //            enquiryListQuery = ProccessingQuery;
            //    }
            //}
            //else
            //{
            //if (model.SearchEmployeeKey != null)
            //{
            //    enquiryListQuery = enquiryListQuery.Where(row => row.EmployeeKey == model.SearchEmployeeKey);
            //}
            //}

            var enqiurylist = enquiryListQuery.OrderByDescending(row => row.CreatedOn).ToList();

            if (!DbConstants.Role.AdminUserTypes.Contains(DbConstants.User.RoleKey))
            {

                List<EmployeeHeirarchyDetailsModel> EmployeeHeirarchyDetailsList = new List<EmployeeHeirarchyDetailsModel>();

                EmployeeHeirarchyDetailsList = (from EH in dbContext.FN_EmployeeHeirarchy(DbConstants.User.UserKey)
                                                select new EmployeeHeirarchyDetailsModel
                                                {
                                                    RowKey = 0,
                                                    ToEmployeeKey = EH.EmployeeKey,
                                                    DataAccess = EH.DataAccess ?? false,
                                                    IsActive = EH.IsActive ?? false
                                                }).ToList();

                enqiurylist = (from EL in enqiurylist
                               join FNEH in EmployeeHeirarchyDetailsList.Where(x => x.IsActive) on EL.EmployeeKey equals FNEH.ToEmployeeKey
                               select new EnquiryViewModel
                               {
                                   RowKey = EL.RowKey,
                                   BranchKey = EL.BranchKey,
                                   EnquiryName = EL.EnquiryName,
                                   EmailAddress = EL.EmailAddress,
                                   BranchName = EL.BranchName,
                                   EnquiryEducationQualification = EL.EnquiryEducationQualification,
                                   MobileNumber = EL.MobileNumber,
                                   EnquiryStatusKey = EL.EnquiryStatusKey,
                                   AcademicTermKey = EL.AcademicTermKey,
                                   AcademicTermName = EL.AcademicTermName,
                                   EnquiryFeedback = EL.EnquiryFeedback,
                                   CourseName = EL.CourseName,
                                   UniversityName = EL.UniversityName,
                                   LastCallScheduleDate = EL.LastCallScheduleDate,
                                   LastCallStatusKey = EL.EnquiryCallStatusKey,
                                   EnquiryLastUpdatedDate = EL.EnquiryLastUpdatedDate,
                                   CreatedBy = EL.CreatedBy,
                                   LastUpdatedBy = EL.LastUpdatedBy,
                                   CreatedOn = EL.CreatedOn,
                                   AddedEmployeeKey = EL.AddedEmployeeKey,
                                   EmployeeKey = EL.EmployeeKey ?? 0,
                                   EmployeeName = EL.EmployeeName,
                                   EnquiryCallStatusName = EL.EnquiryCallStatusName,
                                   IsProccessed = EL.IsProccessed,
                                   EnquiryStatusesName = EL.EnquiryStatusesName,
                                   IsEditable = FNEH != null ? FNEH.DataAccess : false,
                                   CouncellingTime = EL.CouncellingTime,
                                   DistrictName = EL.DistrictName,
                                   LocationName = EL.LocationName

                               }).ToList();
            }

            TotalRecords = enqiurylist.Count();
            var enquiryList = enqiurylist.OrderByDescending(Row => Row.CreatedOn).Skip(skip).Take(Take).ToList();
            return enquiryList;
        }
        public EnquiryViewModel GetEnquiryById(EnquiryViewModel model)
        {
            EnquiryViewModel objViewModel = new EnquiryViewModel();
            try
            {
                objViewModel = dbContext.Enquiries.Where(row => row.RowKey == model.RowKey).Select(row => new EnquiryViewModel
                {
                    RowKey = row.RowKey,
                    EnquiryName = row.EnquiryName,
                    TelephoneCodeKey = row.TelephoneCodeKey,
                    MobileNumber = row.MobileNumber,
                    TelephoneCodeOptionalKey = row.TelePhoneCodeOptionalkey,
                    MobileNumberOptional = row.MobileNumberOptional,
                    PhoneNumber = row.PhoneNumber,
                    EmailAddress = row.EmailAddress,
                    EnquiryEducationQualification = row.EnquiryEducationQualification,
                    DateOfBirth = row.DateOfBirth,
                    AcademicTermKey = row.AcademicTermKey,
                    CourseTypeKey = row.Course.CourseTypeKey,
                    CourseKey = row.CourseKey,
                    UniversityKey = row.UniversityKey,
                    BranchKey = row.BranchKey,
                    //DepartmentKey = row.DepartmentKey,
                    EnquiryFeedback = row.EnquiryFeedback,
                    NatureOfEnquiryKey = row.NatureOfEnquiryKey,
                    EnquiryStatusKey = row.EnquiryStatusKey ?? 0,
                    Gender = row.Gender ?? 0,
                    DistrictName = row.DistrictName,
                    LocationName = row.LocationName,
                    Remarks = row.Remarks,
                    EnquiryLeadKey = row.EnquiryLeads.Select(EL => EL.RowKey).FirstOrDefault(),
                    EmployeeKey = row.EmployeeKey,
                    ConcellingTimeKey = row.EnquiryFeedbacks.Where(x => x.EnquiryKey == row.RowKey).OrderByDescending(x => x.DateAdded).Select(x => x.CouncellingTime).FirstOrDefault(),
                    IsShortListed = row.IsShortlisted ?? false,
                    MaxPhoneLength = row.Country.MaxPhoneLength,
                    MinPhoneLength = row.Country.MinPhoneLength
                }).SingleOrDefault();
                if (objViewModel == null)
                {
                    objViewModel = new EnquiryViewModel();
                    if (model.EnquiryLeadKey != null)
                    {
                        objViewModel = dbContext.EnquiryLeads.Where(row => row.RowKey == model.EnquiryLeadKey).Select(row => new EnquiryViewModel
                        {
                            EnquiryLeadKey = row.RowKey,
                            EnquiryName = row.Name,
                            TelephoneCodeKey = row.TelephoneCodeKey ?? 0,
                            MobileNumber = row.MobileNumber,
                            TelephoneCodeOptionalKey = row.TelePhoneCodeOptionalkey,
                            MobileNumberOptional = row.MobileNumberOptional,
                            PhoneNumber = row.PhoneNumber,
                            EmailAddress = row.EmailAddress,
                            EnquiryEducationQualification = row.Qualification,
                            NatureOfEnquiryKey = DbConstants.NatureOfEnquiry.Lead,
                            BranchKey = row.BranchKey ?? 0,
                            DistrictName = row.District,
                            LocationName = row.Location,
                            EmployeeKey = row.EmployeeKey
                        }).FirstOrDefault();
                    }
                    //objViewModel.AcademicTermKey = DbConstants.AcademicTerm.Yearly;
                    objViewModel.EnquiryStatusKey = DbConstants.EnquiryStatus.FollowUp;
                }

                EnquiryFeedback EnquiryFeedbackModel = dbContext.EnquiryFeedbacks.Where(x => x.EnquiryKey == model.RowKey).OrderByDescending(x => x.DateAdded).Take(1).SingleOrDefault();
                if (EnquiryFeedbackModel != null)
                {
                    objViewModel.CallTypeKey = EnquiryFeedbackModel.CallTypeKey ?? 0;
                    objViewModel.EnquiryCallStatusKey = EnquiryFeedbackModel.EnquiryCallStatusKey;
                    objViewModel.NextCallSchedule = EnquiryFeedbackModel.EnquiryFeedbackReminderDate;
                    objViewModel.EnquiryDuration = EnquiryFeedbackModel.EnquiryDuration;
                    objViewModel.EnquiryFeedback = EnquiryFeedbackModel.EnquiryFeedbackDesc;
                    objViewModel.CouncellingTime = EnquiryFeedbackModel.CouncellingTime;
                    objViewModel.ConcellingTimeKey = EnquiryFeedbackModel.CouncellingTime;
                }

                FillEnquiryDrodownLists(objViewModel);
                return objViewModel;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Enquiry, ActionConstants.View, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                return new EnquiryViewModel();
            }
        }
        public List<EnquiryFeedbackViewModel> GetEnquiryFeedbackByEnquiryId(long id)
        {
            List<EnquiryFeedbackViewModel> EnquiryFeedbacks =

              (from row in dbContext.EnquiryFeedbacks.OrderByDescending(row => row.DateAdded)
               join AU in dbContext.AppUsers on row.AddedBy equals AU.RowKey
               where row.EnquiryKey == id
               orderby row.DateAdded descending
               select new EnquiryFeedbackViewModel
               {
                   RowKey = row.RowKey,
                   Feedback = row.EnquiryFeedbackDesc,
                   CallDuration = row.EnquiryDuration,
                   NextCallSchedule = row.EnquiryFeedbackReminderDate,
                   CallTypeKey = row.CallTypeKey ?? 0,
                   EnquiryStatusKey = row.Enquiry.EnquiryStatusKey ?? 0,
                   EnquiryCallStatusKey = row.EnquiryCallStatusKey ?? 0,
                   CallTypeName = row.CallType.CallTypeName,
                   EnquiryCallStatusName = row.EnquiryCallStatu.EnquiryCallStatusName,
                   CouncellingTime = row.CouncellingTime,
                   EnquiryKey = row.EnquiryKey,
                   DateAdded = row.DateAdded,

                   PostedBy = AU.FirstName + " " + (AU.MiddleName ?? "") + " " + AU.LastName

               }).ToList();
            return EnquiryFeedbacks;
        }
        public List<EnquiryLeadViewModel> GetEnquiryLead(EnquiryLeadViewModel model)
        {
            try
            {
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();

                IQueryable<EnquiryLeadViewModel> EnquiryLeadListQuery = (from r in dbContext.EnquiryLeads
                                                                         where r.EmployeeKey != null && (r.Name.Contains(model.SearchText) || r.MobileNumber.Contains(model.SearchText))
                                                                         && r.EnquiryLeadStatusKey == DbConstants.EnquiryStatus.FollowUp
                                                                         select new EnquiryLeadViewModel
                                                                         {
                                                                             RowKey = r.RowKey,
                                                                             Name = r.Name,
                                                                             MobileNumber = r.MobileNumber,
                                                                             TelephoneCodeKey = r.TelephoneCodeKey ?? 0,
                                                                             MobileNumberOptional = r.MobileNumberOptional,
                                                                             TelephoneCodeOptionalKey = r.TelePhoneCodeOptionalkey,
                                                                             PhoneNumber = r.PhoneNumber,
                                                                             EmailAddress = r.EmailAddress,
                                                                             BranchName = r.Branch.BranchName,
                                                                             EmployeeKey = r.EmployeeKey ?? 0,
                                                                             Qualification = r.Qualification,
                                                                             EmployeeName = r.Employee.FirstName,
                                                                             EnquiryLeadStatusName = r.EnquiryStatu.EnquiryStatusName
                                                                         });
                if (Employee != null)
                {
                    if (!DbConstants.Role.AdminUserTypes.Contains(Employee.AppUser.RoleKey))
                    {
                        EnquiryLeadListQuery = EnquiryLeadListQuery.Where(x => x.EmployeeKey == Employee.RowKey);
                    }

                }
                return EnquiryLeadListQuery.ToList();


            }



            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Enquiry, ActionConstants.View, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                return new List<EnquiryLeadViewModel>();


            }
        }
        public EnquiryFeedbackViewModel GetEnquiryFeedbackById(EnquiryFeedbackViewModel model)
        {

            EnquiryFeedbackViewModel FeedbackModel = dbContext.EnquiryFeedbacks.Where(row => row.RowKey == model.RowKey).Select(row => new EnquiryFeedbackViewModel
            {
                RowKey = row.RowKey,
                EnquiryKey = row.EnquiryKey,
                Feedback = row.EnquiryFeedbackDesc,
                CallDuration = row.EnquiryDuration,
                NextCallSchedule = row.EnquiryFeedbackReminderDate,
                CallTypeKey = row.CallTypeKey ?? 0,
                EnquiryStatusKey = row.Enquiry.EnquiryStatusKey ?? 0,
                EnquiryCallStatusKey = row.EnquiryCallStatusKey ?? 0,
                ConcellingTimeKey = row.CouncellingTime,
                DateAdded = row.DateAdded

            }).SingleOrDefault();
            if (FeedbackModel == null)
            {
                FeedbackModel = new EnquiryFeedbackViewModel();
                FeedbackModel.EnquiryStatusKey = DbConstants.EnquiryStatus.FollowUp;
                Enquiry enquiry = dbContext.Enquiries.Where(row => row.RowKey == model.EnquiryKey).FirstOrDefault();

                FeedbackModel.EnquiryStatusKey = enquiry.EnquiryStatusKey ?? 0;

                EnquiryFeedback dbFeedback = dbContext.EnquiryFeedbacks.Where(x => x.EnquiryKey == enquiry.RowKey).OrderByDescending(x => x.RowKey).FirstOrDefault();
                if (dbFeedback != null)
                {
                    FeedbackModel.EnquiryCallStatusKey = dbFeedback.EnquiryCallStatusKey;
                }

                if (enquiry != null)
                {
                    FeedbackModel.IsShortListed = enquiry.IsShortlisted ?? false;
                }
            }

            FeedbackModel.ModuleKey = model.ModuleKey;

            FillFeedbackDrodownLists(FeedbackModel);

            if (model.BranchKey != null)
            {
                FeedbackModel.Branches = FeedbackModel.Branches.Where(x => x.RowKey != model.BranchKey).ToList();
            }
            return FeedbackModel;
        }
        public EnquiryViewModel CreateEnquiry(EnquiryViewModel model)
        {
            FillEnquiryDrodownLists(model);
            bool result = checkMobileNumber(model);
            if (result)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.MobileNumber);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {

                try
                {

                    Enquiry enquiryModel = new Enquiry();
                    Int64 maxKey = dbContext.Enquiries.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    enquiryModel.RowKey = Convert.ToInt64(maxKey + 1);
                    enquiryModel.AcademicTermKey = model.AcademicTermKey;
                    enquiryModel.CourseKey = model.CourseKey;
                    enquiryModel.UniversityKey = model.UniversityKey;
                    enquiryModel.NatureOfEnquiryKey = model.NatureOfEnquiryKey;
                    enquiryModel.EnquiryName = model.EnquiryName;
                    enquiryModel.EmailAddress = model.EmailAddress;
                    enquiryModel.TelephoneCodeKey = model.TelephoneCodeKey;
                    enquiryModel.MobileNumber = model.MobileNumber;
                    enquiryModel.TelePhoneCodeOptionalkey = model.TelephoneCodeOptionalKey;
                    enquiryModel.MobileNumberOptional = model.MobileNumberOptional;
                    enquiryModel.PhoneNumber = model.PhoneNumber;
                    enquiryModel.Gender = model.Gender;
                    enquiryModel.EnquiryEducationQualification = model.EnquiryEducationQualification;
                    enquiryModel.DateOfBirth = model.DateOfBirth;
                    enquiryModel.BranchKey = model.BranchKey;
                    //enquiryModel.DepartmentKey = model.DepartmentKey;
                    //enquiryModel.IsProcessed = model.DepartmentKey == model.UserDepartmentKey ? true : false;
                    enquiryModel.IsProcessed = true;
                    enquiryModel.EnquiryStatusKey = model.EnquiryStatusKey;
                    enquiryModel.DistrictName = model.DistrictName;
                    enquiryModel.LocationName = model.LocationName;
                    enquiryModel.Remarks = model.Remarks;
                    enquiryModel.IsShortlisted = model.IsShortListed;
                    //enquiryModel.IsSpam = false;


                    enquiryModel.EmployeeKey = model.EmployeeKey;


                    dbContext.Enquiries.Add(enquiryModel);

                    if (model.EnquiryLeadKey != null)
                    {
                        EnquiryLead EnquiryLeadModel = dbContext.EnquiryLeads.SingleOrDefault(x => x.RowKey == model.EnquiryLeadKey);
                        EnquiryLeadModel.EnquiryKey = enquiryModel.RowKey;
                        EnquiryLeadModel.EnquiryLeadStatusKey = DbConstants.EnquiryStatus.Intersted;
                        EnquiryLeadModel.IsNewLead = 2;
                    }

                    EnquiryFeedbackViewModel FeedbackModel = new EnquiryFeedbackViewModel();
                    FeedbackModel.EnquiryKey = enquiryModel.RowKey;
                    FeedbackModel.NextCallSchedule = model.NextCallSchedule;
                    FeedbackModel.CallTypeKey = model.CallTypeKey;
                    FeedbackModel.CallDuration = model.EnquiryDuration;
                    FeedbackModel.Feedback = model.EnquiryFeedback;
                    FeedbackModel.EnquiryCallStatusKey = model.EnquiryCallStatusKey;
                    FeedbackModel.EnquiryStatusKey = model.EnquiryStatusKey;
                    FeedbackModel.ConcellingTimeKey = model.ConcellingTimeKey;
                    CreateFeedback(FeedbackModel);



                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.Enquiry, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);


                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Enquiry);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Enquiry, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;
        }
        public EnquiryViewModel UpdateEnquiry(EnquiryViewModel model)
        {
            FillEnquiryDrodownLists(model);
            using (var transaction = dbContext.Database.BeginTransaction())
            {

                try
                {

                    Enquiry enquiryModel = dbContext.Enquiries.SingleOrDefault(row => row.RowKey == model.RowKey);
                    enquiryModel.AcademicTermKey = model.AcademicTermKey;
                    enquiryModel.CourseKey = model.CourseKey;
                    enquiryModel.UniversityKey = model.UniversityKey;
                    enquiryModel.NatureOfEnquiryKey = model.NatureOfEnquiryKey;
                    enquiryModel.EnquiryName = model.EnquiryName;
                    enquiryModel.EmailAddress = model.EmailAddress;
                    enquiryModel.TelephoneCodeKey = model.TelephoneCodeKey;
                    enquiryModel.MobileNumber = model.MobileNumber;
                    enquiryModel.TelePhoneCodeOptionalkey = model.TelephoneCodeOptionalKey;
                    enquiryModel.MobileNumberOptional = model.MobileNumberOptional;
                    enquiryModel.PhoneNumber = model.PhoneNumber;
                    enquiryModel.Gender = model.Gender;
                    enquiryModel.EnquiryEducationQualification = model.EnquiryEducationQualification;
                    enquiryModel.DateOfBirth = model.DateOfBirth;
                    enquiryModel.BranchKey = model.BranchKey;
                    //enquiryModel.DepartmentKey = model.DepartmentKey;
                    enquiryModel.EnquiryStatusKey = model.EnquiryStatusKey;
                    //enquiryModel.IsProcessed = model.DepartmentKey == model.UserDepartmentKey ? true : false;
                    enquiryModel.IsProcessed = true;
                    enquiryModel.DistrictName = model.DistrictName;
                    enquiryModel.LocationName = model.LocationName;
                    enquiryModel.Remarks = model.Remarks;

                    Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();

                    if (Employee != null)// && Employee.DepartmentKey == enquiryModel.DepartmentKey)
                    {
                        enquiryModel.EmployeeKey = Employee.RowKey;
                    }
                    else
                    {
                        enquiryModel.EmployeeKey = model.EmployeeKey;
                    }

                    EnquiryFeedback DbEnquiryFeedbackModel = dbContext.EnquiryFeedbacks.Where(x => x.EnquiryKey == model.RowKey).OrderByDescending(x => x.DateAdded).Take(1).SingleOrDefault();
                    EnquiryFeedbackViewModel EnquiryFeedbackModel = new EnquiryFeedbackViewModel();

                    EnquiryFeedbackModel.EnquiryKey = enquiryModel.RowKey;
                    EnquiryFeedbackModel.CallTypeKey = model.CallTypeKey;
                    EnquiryFeedbackModel.EnquiryCallStatusKey = model.EnquiryCallStatusKey;
                    EnquiryFeedbackModel.NextCallSchedule = model.NextCallSchedule;
                    EnquiryFeedbackModel.CallDuration = model.EnquiryDuration;
                    EnquiryFeedbackModel.Feedback = model.EnquiryFeedback;

                    EnquiryFeedbackModel.EnquiryStatusKey = model.EnquiryStatusKey;
                    EnquiryFeedbackModel.ConcellingTimeKey = model.ConcellingTimeKey;
                    if (DbEnquiryFeedbackModel == null)
                    {
                        EnquiryFeedbackModel.RowKey = 0;
                        CreateFeedback(EnquiryFeedbackModel);
                    }
                    else
                    {
                        EnquiryFeedbackModel.RowKey = DbEnquiryFeedbackModel.RowKey;
                        UpdateFeedback(EnquiryFeedbackModel);
                    }


                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Enquiry, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Enquiry);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Enquiry, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public EnquiryFeedbackViewModel CreateEnquiryFeedback(EnquiryFeedbackViewModel model)
        {
            FillFeedbackDrodownLists(model);
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    //model.EnquiryKey = dbContext.EnquiryFeedbacks.Where(x => x.RowKey == model.EnquiryKey).Select(x => x.EnquiryKey).SingleOrDefault();
                    dbContext.EnquiryFeedbacks.Where(x => x.EnquiryKey == model.EnquiryKey).ToList().ForEach(x =>
                    {
                        x.EnquiryFeedbackReminderStatus = false;
                        x.IsLastFeedback = false;
                    });

                    Employee Employee = dbContext.Employees.SingleOrDefault(x => x.AppUserKey == DbConstants.User.UserKey);
                    Enquiry EnquiryModel = dbContext.Enquiries.SingleOrDefault(x => x.RowKey == model.EnquiryKey);
                    int EmployeeBranchKey = EnquiryModel.Employee != null ? EnquiryModel.Employee.BranchKey : 0;

                    if (EmployeeBranchKey != model.OtherBranchKey)
                    {
                        EnquiryModel.OtherBranchKey = model.OtherBranchKey;
                    }
                    else
                    {
                        EnquiryModel.OtherBranchKey = null;
                    }

                    EmployeeFileHandover employeeFileHandover = new EmployeeFileHandover();
                    if (EnquiryModel != null)
                    {
                        employeeFileHandover = dbContext.EmployeeFileHandovers.SingleOrDefault(row => row.FileKey == EnquiryModel.RowKey && row.FileHandoverTypeKey == DbConstants.FileHandoverType.Enquiry && row.IsActive);
                    }
                    //if (DbConstants.Role.AdminUserTypes.Contains(DbConstants.User.RoleKey))
                    //{
                    CreateFeedback(model);
                    //}
                    //else
                    //{
                    //    if (EnquiryModel.EmployeeKey == null || (Employee != null ? Employee.RowKey : 0) == (EnquiryModel.EmployeeKey))// || Employee.DepartmentKey == EnquiryModel.DepartmentKey))
                    //    {

                    //        CreateFeedback(model);
                    //    }
                    //    else
                    //    {
                    //        model.Message = EduSuiteUIResources.EnquiryAlreadyProcessedByAnotherUser;
                    //        model.IsSuccessful = false;
                    //        return model;
                    //    }
                    //}

                    if (EnquiryModel != null)
                    {
                        EnquiryModel.IsShortlisted = model.IsShortListed;
                        EnquiryModel.EnquiryStatusKey = model.EnquiryStatusKey;
                    }
                    if (Employee != null)
                    {
                        //EnquiryModel.EmployeeKey = Employee.RowKey;
                        //EnquiryModel.DepartmentKey = Employee.DepartmentKey;
                        EnquiryModel.IsProcessed = true;
                    }
                    if (employeeFileHandover != null)
                    {
                        employeeFileHandover.IsActive = false;
                    }

                    if (model.EmployeeKey != null)
                    {
                        List<EmployeeFileHandOverViewModel> FileHandOverList = new List<EmployeeFileHandOverViewModel>();
                        EmployeeFileHandOverViewModel FileHandOvers = new EmployeeFileHandOverViewModel();
                        FileHandOvers.FileHandoverTypeKey = DbConstants.FileHandoverType.Enquiry;

                        FileHandOvers.EmployeeFromKey = EnquiryModel.EmployeeKey ?? 0;
                        FileHandOvers.EmployeeToKey = model.EmployeeKey ?? 0;
                        FileHandOvers.FileKey = model.EnquiryKey;
                        FileHandOvers.IsActive = true;
                        FileHandOverList.Add(FileHandOvers);
                        UpdateEmployeesFileHandover(FileHandOverList);

                        EnquiryModel.EmployeeKey = model.EmployeeKey;
                    }

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.Enquiry, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Enquiryfeedback);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Enquiry, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public EnquiryFeedbackViewModel UpdateEnquiryFeedback(EnquiryFeedbackViewModel model)
        {
            FillFeedbackDrodownLists(model);
            using (var transaction = dbContext.Database.BeginTransaction())
            {

                try
                {
                    UpdateFeedback(model);
                    Enquiry EnquiryModel = dbContext.Enquiries.Where(x => x.RowKey == model.EnquiryKey).SingleOrDefault();
                    EnquiryModel.EnquiryStatusKey = model.EnquiryStatusKey;


                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Enquiry, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Enquiryfeedback);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Enquiry, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        private void CreateFeedback(EnquiryFeedbackViewModel model)
        {
            EnquiryFeedback enquiryFeedbackModel = new EnquiryFeedback();
            Int64 maxKey = dbContext.EnquiryFeedbacks.Select(p => p.RowKey).DefaultIfEmpty().Max();
            enquiryFeedbackModel.RowKey = Convert.ToInt64(maxKey + 1);
            enquiryFeedbackModel.EnquiryFeedbackDesc = model.Feedback;
            enquiryFeedbackModel.EnquiryDuration = model.CallDuration;
            enquiryFeedbackModel.CallTypeKey = model.CallTypeKey;
            enquiryFeedbackModel.EnquiryFeedbackReminderDate = model.NextCallSchedule;
            enquiryFeedbackModel.CouncellingTime = model.ConcellingTimeKey;
            if (model.NextCallSchedule == null)
            {
                enquiryFeedbackModel.EnquiryFeedbackReminderStatus = false;
            }
            else
            {
                enquiryFeedbackModel.EnquiryFeedbackReminderStatus = true;
            }
            enquiryFeedbackModel.IsLastFeedback = true;

            enquiryFeedbackModel.EnquiryKey = model.EnquiryKey;
            enquiryFeedbackModel.EnquiryCallStatusKey = model.EnquiryCallStatusKey;
            dbContext.EnquiryFeedbacks.Add(enquiryFeedbackModel);

            model.Feedback = "";
            //var AcademicTermKey = dbContext.Enquiries.Where(row => row.RowKey == model.EnquiryKey).Select(row => row.AcademicTermKey).FirstOrDefault();
            if (!DbConstants.Role.AdminUserTypes.Contains(DbConstants.User.RoleKey))//&& AcademicTermKey != DbConstants.AcademicTerm.HR)
            {
                CheckFeedBackAccountBlockCount(model);

                if (model.IsClosedNotification == true)
                {

                    //DbConstants.User.UserKeys = dbContext.AppUsers.Where(row => row.RowKey == DbConstants.AdminKey).Select(row => row.RowKey)
                    //        .Union(dbContext.fnParentEmployees(DbConstants.User.UserKey).Where(row => row.AppUserKey != DbConstants.User.UserKey)
                    //       .Select(row => row.AppUserKey ?? 0)).ToList();
                }

            }


        }
        private void UpdateFeedback(EnquiryFeedbackViewModel model)
        {
            EnquiryFeedback enquiryFeedbackModel = new EnquiryFeedback();

            enquiryFeedbackModel = dbContext.EnquiryFeedbacks.SingleOrDefault(row => row.RowKey == model.RowKey);
            enquiryFeedbackModel.EnquiryFeedbackDesc = model.Feedback;
            enquiryFeedbackModel.EnquiryDuration = model.CallDuration;
            enquiryFeedbackModel.CallTypeKey = model.CallTypeKey;
            enquiryFeedbackModel.CouncellingTime = model.ConcellingTimeKey;
            if (model.NextCallSchedule == null)
            {
                enquiryFeedbackModel.EnquiryFeedbackReminderStatus = false;
            }
            else
            {
                enquiryFeedbackModel.EnquiryFeedbackReminderStatus = true;
            }
            enquiryFeedbackModel.EnquiryCallStatusKey = model.EnquiryCallStatusKey;
            enquiryFeedbackModel.EnquiryFeedbackReminderDate = model.NextCallSchedule;
            Enquiry EnquiryModel = dbContext.Enquiries.SingleOrDefault(row => row.RowKey == enquiryFeedbackModel.EnquiryKey);
            EnquiryModel.EnquiryStatusKey = model.EnquiryStatusKey;
            model.EnquiryKey = enquiryFeedbackModel.EnquiryKey;
            AppUser user = dbContext.AppUsers.Where(x => x.RowKey == enquiryFeedbackModel.AddedBy).SingleOrDefault();

            model.Feedback = "";
            if (!DbConstants.Role.AdminUserTypes.Contains(DbConstants.User.RoleKey))
            {
                CheckFeedBackAccountBlockCount(model);
            }

        }
        public EmployeeFileHandOverViewModel UpdateEmployeesFileHandover(List<EmployeeFileHandOverViewModel> modelList)
        {
            EmployeeFileHandOverViewModel employeeFileHandoverViewModel = new EmployeeFileHandOverViewModel();

            long maxKey = dbContext.EmployeeFileHandovers.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (EmployeeFileHandOverViewModel model in modelList)
            {
                EmployeeFileHandover EmployeeFileHandoverModel = new EmployeeFileHandover();
                EmployeeFileHandoverModel.RowKey = Convert.ToInt64(maxKey + 1);
                EmployeeFileHandoverModel.FileHandoverTypeKey = model.FileHandoverTypeKey ?? 0;
                EmployeeFileHandoverModel.EmployeeFromKey = model.EmployeeFromKey;
                EmployeeFileHandoverModel.EmployeeToKey = model.EmployeeToKey;
                EmployeeFileHandoverModel.FileKey = model.FileKey;
                EmployeeFileHandoverModel.IsActive = true;
                dbContext.EmployeeFileHandovers.Add(EmployeeFileHandoverModel);
                maxKey++;
            }

            return employeeFileHandoverViewModel;
        }
        public EnquiryViewModel DeleteEnquiry(EnquiryViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Enquiry enquiry = dbContext.Enquiries.SingleOrDefault(row => row.RowKey == model.RowKey);
                    List<EnquiryFeedback> enquiryFeedbackList = dbContext.EnquiryFeedbacks.Where(row => row.EnquiryKey == model.RowKey).ToList();
                    var isSchoalrship = dbContext.Scholarships.Any(x => x.EnquiryKey == model.RowKey);
                    if (isSchoalrship)
                    {
                        Scholarship scholarship = dbContext.Scholarships.SingleOrDefault(row => row.EnquiryKey == model.RowKey);
                        scholarship.EnquiryKey = null;
                    }
                    enquiryFeedbackList.ForEach(enquiryFeedback => dbContext.EnquiryFeedbacks.Remove(enquiryFeedback));
                    dbContext.Enquiries.Remove(enquiry);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Enquiry, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Enquiry);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.Enquiry, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Enquiry);
                    model.IsSuccessful = false;
                    model.IsSuccessful = false; ActivityLog.CreateActivityLog(MenuConstants.Enquiry, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            //}
            return model;
        }
        public EnquiryFeedbackViewModel DeleteEnquiryFeeback(EnquiryFeedbackViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    EnquiryFeedback enquiryFeedback = dbContext.EnquiryFeedbacks.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.EnquiryFeedbacks.Remove(enquiryFeedback);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Enquiry, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Enquiryfeedback);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.Enquiry, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);

                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Enquiryfeedback);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Enquiry, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            //}
            return model;
        }
        public EnquiryViewModel GetSearchDropDownLists(EnquiryViewModel model)
        {
            FillEnquiryCallStatus(model);
            FillBranches(model);
            GetDepartmentByBranchId(model);
            FillAcademicTerms(model);
            GetEmployeesByBranchId(model);
            FillEnquiryStatus(model);
            FillNatureOfEnquiry(model);
            return model;
        }
        public EnquiryViewModel CheckMobileNumberExists(string MobileNumber, long RowKey, long? EnquiryLeadKey)
        {
            EnquiryViewModel model = new EnquiryViewModel();
            var result = dbContext.Enquiries.Where(row => row.RowKey != RowKey).Select
                (row => new EnquiryViewModel
                {
                    //AcademicTermKey = row.AcademicTermKey, 
                    //TelephoneCodeKey = row.TelephoneCodeKey, 
                    MobileNumber = row.MobileNumber
                })
                .Union(dbContext.EnquiryLeads.Where(row => row.RowKey != EnquiryLeadKey).Select
                (row => new EnquiryViewModel
                {
                    //AcademicTermKey = row.AcademicTermKey ?? 0, 
                    //TelephoneCodeKey = row.TelephoneCodeKey,
                    MobileNumber = row.MobileNumber
                })).Union(dbContext.Applications.Where(row => row.EnquiryKey != RowKey).Select
                (row => new EnquiryViewModel
                {
                    //AcademicTermKey = row.M_Course.AcademicTermKey, 
                    //TelephoneCodeKey = row.TelephoneCodeKey,
                    MobileNumber = row.StudentMobile
                }))
                .Any(row => row.MobileNumber == MobileNumber /*&& (row.TelephoneCodeKey == TelephoneCodeKey || TelephoneCodeKey == 0) && ((AcademicTermKeys.Contains(AcademicTermKey) && AcademicTermKeys.Contains(row.AcademicTermKey)) || row.AcademicTermKey == AcademicTermKey || AcademicTermKey == 0)*/);
            if (result)
            {
                model.IsSuccessful = false;

            }
            else
            {
                model.IsSuccessful = true;
            }
            return model;
        }
        //public EnquiryViewModel CheckMobileNumberExists2(string MobileNumber, short? TelephoneCodeKey, long RowKey, long? EnquiryLeadKey, byte AcademicTermKey)
        //{
        //    List<byte> AcademicTermKeys = new List<byte>() { DbConstants.AcademicTerm.Study, DbConstants.AcademicTerm.Migration };
        //    EnquiryViewModel model = new EnquiryViewModel();
        //    var result = dbContext.Enquiries.Where(row => row.RowKey != RowKey).Select(row => new EnquiryViewModel { AcademicTermKey = row.AcademicTermKey, TelephoneCodeKey = row.TelephoneCodeKey, MobileNumber = row.MobileNumber, TelephoneCodeOptionalKey = row.TelePhoneCodeOptionalkey, MobileNumberOptional = row.MobileNumberOptional })
        //        .Union(dbContext.EnquiryLeads.Where(row => row.RowKey != EnquiryLeadKey).Select(row => new EnquiryViewModel { AcademicTermKey = row.AcademicTermKey ?? 0, TelephoneCodeKey = row.TelephoneCodeKey, MobileNumber = row.MobileNumber, TelephoneCodeOptionalKey = row.TelePhoneCodeOptionalkey, MobileNumberOptional = row.MobileNumberOptional }))
        //        .Union(dbContext.Applications.Where(row => row.EnquiryKey != RowKey).Select(row => new EnquiryViewModel { AcademicTermKey = row.Course.AcademicTermKey, TelephoneCodeKey = row.TelephoneCodeKey, MobileNumber = row.MobileNumber, TelephoneCodeOptionalKey = row.TelePhoneCodeOptionalkey, MobileNumberOptional = row.MobileNumberOptional }))
        //        .Any(row => (row.MobileNumber == MobileNumber && (row.TelephoneCodeKey == TelephoneCodeKey || TelephoneCodeKey == 0) || row.MobileNumberOptional == MobileNumber && (row.TelephoneCodeOptionalKey == TelephoneCodeKey || TelephoneCodeKey == 0)) && ((AcademicTermKeys.Contains(AcademicTermKey) && AcademicTermKeys.Contains(row.AcademicTermKey)) || row.AcademicTermKey == AcademicTermKey || AcademicTermKey == 0));
        //    if (result)
        //    {
        //        model.IsSuccessful = false;

        //    }
        //    else
        //    {
        //        model.IsSuccessful = true;
        //    }
        //    return model;
        //}
        public EnquiryViewModel CheckEmailAddressExists(string EmailAddress, long RowKey, long? EnquiryLeadKey)
        {
            EnquiryViewModel model = new EnquiryViewModel();
            var result = dbContext.Enquiries.Where(row => row.RowKey != RowKey).Select
                (row => new EnquiryViewModel
                {
                    //AcademicTermKey = row.AcademicTermKey, 
                    EmailAddress = row.EmailAddress
                })
            .Union(dbContext.EnquiryLeads.Where
            (row => row.RowKey != EnquiryLeadKey).Select
            (row => new EnquiryViewModel
            {
                //AcademicTermKey = row.AcademicTermKey ?? 0,
                EmailAddress = row.EmailAddress
            }))
            .Union(dbContext.Applications.Where(row => row.EnquiryKey != RowKey).Select
            (row => new EnquiryViewModel
            {
                //AcademicTermKey = row.Program.AcademicTermKey, 
                EmailAddress = row.StudentEmail
            }))
            .Any(row => row.EmailAddress == EmailAddress /*&& ((AcademicTermKeys.Contains(AcademicTermKey) && AcademicTermKeys.Contains(row.AcademicTermKey)) || row.AcademicTermKey == AcademicTermKey || AcademicTermKey == 0)*/);
            if (result)
            {
                model.IsSuccessful = false;

            }
            else
            {
                model.IsSuccessful = true;
            }
            return model;
        }
        public EnquiryViewModel CheckCounsellingTimeExists(EnquiryViewModel model)
        {
            //List<byte> AcademicTermKeys = new List<byte>() { DbConstants.AcademicTerm.Study, DbConstants.AcademicTerm.Migration };
            var result = dbContext.EnquiryFeedbacks.Any(row => row.EnquiryKey != model.RowKey && row.AddedBy == DbConstants.User.UserKey && row.CouncellingTime == model.ConcellingTimeKey && row.EnquiryFeedbackReminderDate == model.NextCallSchedule && row.EnquiryFeedbackReminderStatus == true);
            if (result)
            {
                model.IsSuccessful = false;

            }
            else
            {
                model.IsSuccessful = true;
            }
            return model;
        }
        public EnquiryViewModel GetDepartmentByBranchId(EnquiryViewModel model)
        {
            IQueryable<SelectListModel> DepartmentQuery = dbContext.BranchDepartments.Where(row => row.BranchKey == model.BranchKey).Select(row => new SelectListModel
            {
                RowKey = row.Department.RowKey,
                Text = row.Department.DepartmentName
            });
            if (!DbConstants.Role.AdminUserTypes.Contains(DbConstants.User.RoleKey))
            {
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
                if (Employee != null)
                {
                    model.Departments = DepartmentQuery.Where(x => x.RowKey == Employee.DepartmentKey).ToList();
                    // model.SearchDepartmentKey = model.DepartmentKey = Employee.DepartmentKey;
                }

            }
            else
            {
                model.Departments = DepartmentQuery.ToList();
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
                if (Employee != null)
                {
                    //model.SearchDepartmentKey = model.DepartmentKey = Employee.DepartmentKey;
                }

            }
            return model;
        }
        public EnquiryViewModel GetCourseTypeByAcademicTerm(EnquiryViewModel model)
        {
            //model.ProgramTypes = dbContext.UniversityPrograms.Where(row => row.University.CountryKey == model.CountryKey && row.Program.AcademicTermKey == model.AcademicTermKey).Select(row => new SelectListModel
            //{
            //    RowKey = row.Program.ProgramType.RowKey,
            //    Text = row.Program.ProgramType.ProgramTypeName
            //}).GroupBy(row => row.RowKey).Select(row => row.FirstOrDefault()).ToList();

            //bool AcademicTermKey=Convert.ToBoolean(model.AcademicTermKey);

            //model.CourseTypes = dbContext.UniversityCourses.Where(x => x.AcademicTermKey == model.AcademicTermKey).Select(row => new SelectListModel
            //{
            //    RowKey = row.Course.CourseType.RowKey,
            //    Text = row.Course.CourseType.CourseTypeName

            //}).GroupBy(row => row.RowKey).Select(row => row.FirstOrDefault()).ToList();

            //model.CourseTypes = dbContext.UniversityCourses.Select(row => new SelectListModel
            //{
            //    RowKey = row.Course.CourseType.RowKey,
            //    Text = row.Course.CourseType.CourseTypeName

            //}).GroupBy(row => row.RowKey).Select(row => row.FirstOrDefault()).ToList();

            model.CourseTypes = dbContext.CourseTypes.Where(row => row.IsActive && row.Courses.Any(x => x.UniversityCourses.Any(y => y.AcademicTermKey == model.AcademicTermKey))).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.CourseTypeName
            }).GroupBy(row => row.RowKey).Select(row => row.FirstOrDefault()).ToList();

            return model;
        }
        public EnquiryViewModel GetCountryByAcademicTerm(EnquiryViewModel model)
        {

            //model.Countries = dbContext.UniversityPrograms.Where(row => row.Program.AcademicTermKey == model.AcademicTermKey).Select(row => new SelectListModel
            //{
            //    RowKey = row.Country.RowKey,
            //    Text = row.Country.CountryName
            //}).GroupBy(row => row.RowKey).Select(row => row.FirstOrDefault()).ToList();


            return model;
        }
        public EnquiryViewModel GetCourseByCourseType(EnquiryViewModel model)
        {
            //bool AcademicTermKey = Convert.ToBoolean(model.AcademicTermKey);
            model.Courses = dbContext.UniversityCourses.Where(row =>/* row.UniversityMasterKey == model.UniversityKey && */  row.Course.CourseTypeKey == model.CourseTypeKey && row.AcademicTermKey == model.AcademicTermKey).Select(row => new SelectListModel
            {
                RowKey = row.Course.RowKey,
                Text = row.Course.CourseName
            }).GroupBy(row => row.RowKey).Select(row => row.FirstOrDefault()).ToList();


            return model;
        }
        public EnquiryViewModel GetUniversityByCourse(EnquiryViewModel model)
        {
            //if (model.AcademicTermKey == DbConstants.AcademicTerm.Study)
            //{

            //model.Universities = dbContext.UniversityPrograms.Where(row => row.University.CountryKey == model.CountryKey && row.Program.AcademicTermKey == model.AcademicTermKey).Select(row => new SelectListModel
            //{
            //    RowKey = row.University.RowKey,
            //    Text = row.University.UniversityMasterName
            //}).GroupBy(row => row.RowKey).Select(row => row.FirstOrDefault()).ToList();
            //}
            //bool AcademicTermKey = Convert.ToBoolean(model.AcademicTermKey);

            model.Universities = dbContext.UniversityCourses.Where(row => row.AcademicTermKey == model.AcademicTermKey && row.CourseKey == model.CourseKey).Select(row => new SelectListModel
            {
                RowKey = row.UniversityMaster.RowKey,
                Text = row.UniversityMaster.UniversityMasterName
            }).GroupBy(row => row.RowKey).Select(row => row.FirstOrDefault()).ToList();



            return model;
        }
        private void FillFeedbackDrodownLists(EnquiryFeedbackViewModel model)
        {
            FillEnquiryStatus(model);
            FillEnquiryCallStatus(model);
            CheckCallStatusDuration(model);
            FillCallTypes(model);
            FillConcellingTime(model);
            FillBranches(model);
            GetEmployees(model);
        }
        public void GetEmployees(EnquiryFeedbackViewModel model)
        {
            model.Employees = dbContext.Employees.Where(x => x.EmployeeStatusKey == DbConstants.EmployeeStatus.Working).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.FirstName + " " + (row.MiddleName ?? "") + " " + row.LastName,
            }).ToList();
        }
        private void FillBranches(EnquiryFeedbackViewModel model)
        {
            //IQueryable<SelectListModel> BranchQuery = dbContext.vwBranchSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            //{
            //    RowKey = row.RowKey,
            //    Text = row.BranchName
            //});

            //if (!DbConstants.Role.AdminUserTypes.Contains(DbConstants.User.RoleKey))
            //{
            //    Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
            //    if (Employee != null)
            //    {
            //        if (Employee.BranchAccess != null)
            //        {
            //            List<long> Branches = Employee.BranchAccess.Split(',').Select(Int64.Parse).ToList();
            //            model.Branches = BranchQuery.Where(row => Branches.Contains(row.RowKey)).ToList();
            //            model.BranchKey = Employee.BranchKey;

            //        }
            //        else
            //        {
            //            model.Branches = BranchQuery.Where(x => x.RowKey == Employee.BranchKey).ToList();
            //            model.BranchKey = Employee.BranchKey;
            //        }
            //    }
            //    else
            //    {
            //        model.Branches = BranchQuery.ToList();
            //    }
            //}
            //else
            //{

            //    model.Branches = BranchQuery.ToList();
            //    Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
            //    if (Employee != null)
            //    {
            //        if (Employee.BranchAccess != null)
            //        {
            //            List<long> Branches = Employee.BranchAccess.Split(',').Select(Int64.Parse).ToList();
            //            model.Branches = BranchQuery.Where(row => Branches.Contains(row.RowKey)).ToList();
            //            model.BranchKey = Employee.BranchKey;

            //        }
            //        else
            //        {
            //            model.Branches = BranchQuery.Where(x => x.RowKey == Employee.BranchKey).ToList();
            //            model.BranchKey = Employee.BranchKey;
            //        }
            //    }
            //}

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
                    if (model.Branches.Count == 1)
                    {
                        long? branchkey = model.Branches.Select(x => x.RowKey).FirstOrDefault();
                        model.BranchKey = Convert.ToInt16(branchkey);

                    }
                }
            }
            else
            {

                model.Branches = BranchQuery.ToList();
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
                if (model.Branches.Count == 1)
                {
                    long? branchkey = model.Branches.Select(x => x.RowKey).FirstOrDefault();
                    model.BranchKey = Convert.ToInt16(branchkey);

                }
            }


        }
        private void FillEnquiryDrodownLists(EnquiryViewModel model)
        {
            FillBranches(model);
            GetDepartmentByBranchId(model);
            FillAcademicTerms(model);
            GetCountryByAcademicTerm(model);
            GetUniversityByCourse(model);
            GetCourseTypeByAcademicTerm(model);
            GetCourseByCourseType(model);
            CheckCallStatusDuration(model);
            FillCallTypes(model);
            GetCallStatusByEnquiryStatus(model);
            FillAddEditEnquiryStatus(model);
            FillEnquiryCallStatus(model);
            FillNatureOfEnquiry(model);
            GetEmployeesByBranchId(model);
            FillConcellingTime(model);
            FillTelephoneCodes(model);
            GetPhoneNumberLength(model);
        }
        public EnquiryViewModel GetEmployeesByBranchId(EnquiryViewModel model)
        {
            //Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();

            //if (DbConstants.User.UserKey != DbConstants.AdminKey)
            //{
            //    if (Employee != null)
            //    {
            //        var Branches = Employee.BranchAccess.Split(',').Select(Int16.Parse).ToList();

            //        if (Branches.Count > 1)
            //        {
            //            model.Employees = dbContext.Employees.Where(row => row.EmployeeStatusKey == DbConstants.EmployeeStatus.Working && row.IsActive == true && (Branches.Contains(row.BranchKey))).Select(row => new GroupSelectListModel
            //            {
            //                RowKey = row.RowKey,
            //                Text = row.FirstName,
            //                GroupKey = row.DepartmentKey,
            //                GroupName = row.Department.DepartmentName

            //            }).OrderBy(row => row.Text).ToList();
            //        }
            //        else
            //        {
            //            model.Employees = dbContext.Employees.Where(row => row.EmployeeStatusKey == DbConstants.EmployeeStatus.Working && row.IsActive == true && row.RowKey == Employee.RowKey).Select(row => new GroupSelectListModel
            //            {
            //                RowKey = row.RowKey,
            //                Text = row.FirstName,
            //                GroupKey = row.DepartmentKey,
            //                GroupName = row.Department.DepartmentName
            //            }).OrderBy(row => row.Text).ToList();
            //        }
            //        model.SearchEmployeeKey = model.EmployeeKey = Employee.RowKey;
            //    }
            //}
            //else
            //{
            //    model.Employees = dbContext.Employees.Where(row => row.EmployeeStatusKey == DbConstants.EmployeeStatus.Working && (row.BranchKey == model.BranchKey || model.BranchKey == 0)).Select(row => new GroupSelectListModel
            //    {
            //        RowKey = row.RowKey,
            //        Text = row.FirstName + " " + (row.MiddleName ?? "") + " " + row.LastName,
            //        GroupKey = row.DepartmentKey,
            //        GroupName = row.EmployeeCode

            //    }).OrderBy(row => row.Text).ToList();

            //    if (Employee != null)
            //    {
            //        model.SearchEmployeeKey = model.EmployeeKey = Employee.RowKey;
            //    }
            //}

            //return model;

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

            if (model.BranchKey != 0)
            {
                Employees = Employees.Where(x => x.BranchKey == model.BranchKey).ToList();
            }



            model.Employees = Employees.Select(x => new GroupSelectListModel
            {
                RowKey = x.RowKey,
                Text = x.FirstName,
                GroupKey = x.DepartmentKey,
                GroupName = x.EmployeeCode
            }).ToList();

            return model;
        }
        public EnquiryFeedbackViewModel CheckCallStatusDuration(EnquiryFeedbackViewModel model)
        {
            model.IsDuration = dbContext.EnquiryCallStatus.Where(row => row.IsActive == true && row.RowKey == model.EnquiryCallStatusKey).Select(row => row.IsDuration).FirstOrDefault();
            return model;
        }
        public void CheckCallStatusDuration(EnquiryViewModel model)
        {
            model.IsDuration = dbContext.EnquiryCallStatus.Where(row => row.IsActive == true && row.RowKey == model.EnquiryCallStatusKey).Select(row => row.IsDuration).FirstOrDefault();
        }

        //private void GetUserDetails(EnquiryViewModel model)
        //{
        //    Employee Emp = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
        //    DbConstants.User.RoleKey = Emp.AppUser.RoleKey;
        //}
        private void FillBranches(EnquiryViewModel model)
        {

            //IQueryable<SelectListModel> BranchQuery = dbContext.vwBranchSelectActiveOnlies.OrderBy(row => row.BranchName).Select(row => new SelectListModel
            // {
            //     RowKey = row.RowKey,
            //     Text = row.BranchName
            // });


            //if (!DbConstants.Role.AdminUserTypes.Contains(DbConstants.User.RoleKey))
            //{
            //    Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
            //    if (Employee != null)
            //    {
            //        if (Employee.BranchAccess != null)
            //        {
            //            List<long> Branches = Employee.BranchAccess.Split(',').Select(Int64.Parse).ToList();
            //            model.Branches = BranchQuery.Where(row => Branches.Contains(row.RowKey)).ToList();
            //            model.SearchBranchKey = model.BranchKey = Employee.BranchKey;

            //        }
            //        else
            //        {
            //            model.Branches = BranchQuery.Where(x => x.RowKey == Employee.BranchKey).ToList();
            //            model.SearchBranchKey = model.BranchKey = Employee.BranchKey;
            //        }
            //    }
            //    else
            //    {
            //        model.Branches = BranchQuery.ToList();
            //        if (model.Branches.Count == 1)
            //        {
            //            long? branchkey = model.Branches.Select(x => x.RowKey).FirstOrDefault();
            //            model.BranchKey = Convert.ToInt16(branchkey);
            //            model.SearchBranchKey = model.BranchKey;

            //        }
            //    }
            //}
            //else
            //{

            //    model.Branches = BranchQuery.ToList();
            //    Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
            //    if (Employee != null)
            //    {
            //        if (Employee.BranchAccess != null)
            //        {
            //            List<long> Branches = Employee.BranchAccess.Split(',').Select(Int64.Parse).ToList();
            //            model.Branches = BranchQuery.Where(row => Branches.Contains(row.RowKey)).ToList();
            //            model.SearchBranchKey = model.BranchKey = Employee.BranchKey;

            //        }
            //        else
            //        {
            //            model.Branches = BranchQuery.Where(x => x.RowKey == Employee.BranchKey).ToList();
            //            model.SearchBranchKey = model.BranchKey = Employee.BranchKey;
            //        }
            //    }
            //    if (model.Branches.Count == 1)
            //    {
            //        long? branchkey = model.Branches.Select(x => x.RowKey).FirstOrDefault();
            //        model.BranchKey = Convert.ToInt16(branchkey);
            //        model.SearchBranchKey = model.BranchKey;
            //    }
            //}


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
                    //model.SearchBranchKey = model.BranchKey = Employee.BranchKey;
                }
                else
                {
                    model.Branches = BranchQuery.Where(x => x.RowKey == Employee.BranchKey).ToList();
                    //model.SearchBranchKey = model.BranchKey = Employee.BranchKey;
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
                model.SearchBranchKey = model.BranchKey;
            }

        }
        private void FillEnquiryCallStatus(EnquiryViewModel model)
        {
            model.LastEnquiryCallStatuses = dbContext.EnquiryCallStatus.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.EnquiryCallStatusName
            }).ToList();

        }
        private void FillCallTypes(EnquiryFeedbackViewModel model)
        {
            model.CallTypes = dbContext.CallTypes.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.CallTypeName
            }).ToList();

        }
        public void GetCallStatusByEnquiryStatus(EnquiryViewModel model)
        {


            string ShowInList = DbConstants.Menu.Enquiry.ToString();
            List<int> CounsellingCompletedCallStatus = new List<int> { 0 };
            if (model.ModuleKey == DbConstants.EnquiryModule.Enquiry)
            {
                CounsellingCompletedCallStatus = new List<int> { DbConstants.EnquiryCallStatus.CounsellingCompleted };
            }


            if (model.EnquiryStatusKey == 0)
            {
                model.CallStatuses = dbContext.EnquiryCallStatus.Where(x => x.IsActive == true && !CounsellingCompletedCallStatus.Contains(x.RowKey) && x.ShowInMenuKeys.Contains(ShowInList)).Select(row => new SelectListModel
                {
                    RowKey = row.RowKey,
                    Text = row.EnquiryCallStatusName
                }).ToList();
            }
            else
            {
                model.CallStatuses = dbContext.EnquiryCallStatus.Where(x => x.IsActive == true && !CounsellingCompletedCallStatus.Contains(x.RowKey) && x.EnquiryStatusKey == model.EnquiryStatusKey && x.ShowInMenuKeys.Contains(ShowInList)).Select(row => new SelectListModel
                {
                    RowKey = row.RowKey,
                    Text = row.EnquiryCallStatusName
                }).ToList();
            }

            //string ShowInList = DbConstants.Menu.Enquiry.ToString();
            //model.CallStatuses = dbContext.EnquiryCallStatus.Where(x => x.IsActive == true && x.EnquiryStatusKey == model.EnquiryStatusKey && x.ShowInMenuKeys.Contains(ShowInList)).Select(row => new SelectListModel
            //{
            //    RowKey = row.RowKey,
            //    Text = row.EnquiryCallStatusName
            //}).ToList();
        }
        public void FillAddEditEnquiryStatus(EnquiryViewModel model)
        {
            model.EnquiryStatuses = dbContext.EnquiryStatus.Where(x => x.RowKey != DbConstants.EnquiryStatus.AdmissionTaken).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.EnquiryStatusName
            }).ToList();
        }
        private void FillEnquiryStatus(EnquiryViewModel model)
        {
            model.EnquiryStatuses = dbContext.EnquiryStatus.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.EnquiryStatusName
            }).ToList();
        }
        private void FillCallTypes(EnquiryViewModel model)
        {
            model.CallTypes = dbContext.CallTypes.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.CallTypeName
            }).ToList();

        }
        private void FillEnquiryCallStatus(EnquiryFeedbackViewModel model)
        {

            string ShowInList = DbConstants.Menu.Enquiry.ToString();
            List<int> CounsellingCompletedCallStatus = new List<int> { 0 };
            if (model.ModuleKey == DbConstants.EnquiryModule.Enquiry)
            {
                CounsellingCompletedCallStatus = new List<int> { DbConstants.EnquiryCallStatus.CounsellingCompleted };
            }


            model.EnquiryCallStatuses = dbContext.EnquiryCallStatus.Where(x => x.IsActive == true && !CounsellingCompletedCallStatus.Contains(x.RowKey) && x.EnquiryStatusKey == model.EnquiryStatusKey && x.ShowInMenuKeys.Contains(ShowInList)).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.EnquiryCallStatusName
            }).ToList();

            //string ShowInList = DbConstants.Menu.Enquiry.ToString();
            //model.EnquiryCallStatuses = dbContext.EnquiryCallStatus.Where(x => x.IsActive == true && x.EnquiryStatusKey == model.EnquiryStatusKey && x.ShowInMenuKeys.Contains(ShowInList)).Select(row => new SelectListModel
            //{
            //    RowKey = row.RowKey,
            //    Text = row.EnquiryCallStatusName
            //}).ToList();

        }
        private void FillAcademicTerms(EnquiryViewModel model)
        {
            model.AcademicTerms = dbContext.AcademicTerms.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AcademicTermName
            }).ToList();
        }
        private void FillNatureOfEnquiry(EnquiryViewModel model)
        {
            model.NatureOfEnquiries = dbContext.NatureOfEnquiries.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.NatureOfEnquiryName
            }).ToList();
        }
        private void FillEnquiryStatus(EnquiryFeedbackViewModel model)
        {

            List<short> FilterStatus = new List<short> { DbConstants.EnquiryStatus.FollowUp };
            var LastCallStatusKey = model.EnquiryCallStatusKey;


            if (LastCallStatusKey == DbConstants.EnquiryCallStatus.Counselling || LastCallStatusKey == DbConstants.EnquiryCallStatus.CounsellingCompleted)
            {
                model.EnquiryStatuses = dbContext.EnquiryStatus.Where(x => !FilterStatus.Contains(x.RowKey) && x.RowKey != DbConstants.EnquiryStatus.AdmissionTaken).Select(row => new SelectListModel
                {
                    RowKey = row.RowKey,
                    Text = row.EnquiryStatusName
                }).ToList();
            }
            else
            {
                model.EnquiryStatuses = dbContext.EnquiryStatus.Where(x => x.RowKey != DbConstants.EnquiryStatus.AdmissionTaken).Select(row => new SelectListModel
                {
                    RowKey = row.RowKey,
                    Text = row.EnquiryStatusName
                }).ToList();
            }


            //model.EnquiryStatuses = dbContext.EnquiryStatus.Where(x => x.RowKey != DbConstants.EnquiryStatus.AdmissionTaken).Select(row => new SelectListModel
            //{
            //    RowKey = row.RowKey,
            //    Text = row.EnquiryStatusName
            //}).ToList();
        }
        private void FillConcellingTime(EnquiryFeedbackViewModel model)
        {

            model.ConcellingTimes = dbContext.CounsellingTimes.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.Times
            }).ToList();

        }
        private void FillConcellingTime(EnquiryViewModel model)
        {

            model.ConcellingTimes = dbContext.CounsellingTimes.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.Times
            }).ToList();

        }
        private void FillTelephoneCodes(EnquiryViewModel model)
        {
            model.TelephoneCodes = dbContext.VwCountrySelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.TelephoneCode
            }).ToList();
        }
        public EnquiryViewModel FillCourseDuration(EnquiryViewModel model)
        {
            model.CourseDuration = dbContext.UniversityCourses.Where(x => x.Course.CourseTypeKey == model.CourseTypeKey && x.AcademicTermKey == model.AcademicTermKey && x.UniversityMasterKey == model.UniversityKey).Select(row => new SelectListModel
            {
                RowKey = row.Course.CourseDuration ?? 0,
                Text = ""
            }).Distinct().ToList();



            return model;
        }
        private string MonthToYear(short Month)
        {
            return Month.ToString();
        }
        private void CheckFeedBackAccountBlockCount(EnquiryFeedbackViewModel model)
        {
            IQueryable<EnquiryScheduleViewModel> List = dbContext.EnquiryFeedbacks.Select(row => new EnquiryScheduleViewModel { UserKey = row.AddedBy, CreateOn = row.DateAdded, LastCallStatusKey = row.EnquiryCallStatusKey ?? 0 }).Union(dbContext.EnquiryLeadFeedbacks.Select(row => new EnquiryScheduleViewModel { UserKey = row.AddedBy, CreateOn = row.DateAdded, LastCallStatusKey = row.EnquiryLeadCallStatusKey ?? 0 }));
            EnquiryConfiguration Configuation = dbContext.EnquiryConfigurations.SingleOrDefault();
            List<int> CallStatuses = Configuation.CallStatuses.Split('|').Select(Int32.Parse).ToList();
            int Count = 0;
            string Message = "";

            if (CallStatuses.Contains(model.EnquiryCallStatusKey ?? 0))
            {
                DateTime Today = DateTimeUTC.Now;
                EnquiryUserBlockList EnquiryUserBlockListModel = dbContext.EnquiryUserBlockLists.Where(x => System.Data.Entity.DbFunctions.TruncateTime(x.DateAdded) == System.Data.Entity.DbFunctions.TruncateTime(Today) && x.AddedBy == DbConstants.User.UserKey).OrderByDescending(x => x.DateAdded).Take(1).SingleOrDefault();
                string CallStatusName = dbContext.EnquiryCallStatus.Where(x => x.IsActive == true && x.RowKey == model.EnquiryCallStatusKey).Select(x => x.EnquiryCallStatusName).SingleOrDefault();
                if (EnquiryUserBlockListModel == null)
                {
                    List = List.Where(x => x.UserKey == DbConstants.User.UserKey && System.Data.Entity.DbFunctions.TruncateTime(x.CreateOn) == System.Data.Entity.DbFunctions.TruncateTime(Today) && CallStatuses.Contains(x.LastCallStatusKey)).OrderByDescending(x => x.CreateOn);
                    Count = List.ToList().Count + 1;
                }
                else
                {
                    List = dbContext.EnquiryFeedbacks.Where(x => x.AddedBy == DbConstants.User.UserKey && (System.Data.Entity.DbFunctions.TruncateTime(x.DateAdded) == System.Data.Entity.DbFunctions.TruncateTime(Today)) && x.DateAdded > EnquiryUserBlockListModel.DateAdded && CallStatuses.Contains(x.EnquiryCallStatusKey ?? 0)).Select(row => new EnquiryScheduleViewModel { UserKey = row.AddedBy, CreateOn = row.DateAdded, LastCallStatusKey = row.EnquiryCallStatusKey ?? 0 })
                       .Union(dbContext.EnquiryLeadFeedbacks.Where(x => x.AddedBy == DbConstants.User.UserKey && (System.Data.Entity.DbFunctions.TruncateTime(x.DateAdded) == System.Data.Entity.DbFunctions.TruncateTime(Today)) && x.DateAdded > EnquiryUserBlockListModel.DateAdded && CallStatuses.Contains(x.EnquiryLeadCallStatusKey ?? 0)).Select(row => new EnquiryScheduleViewModel { UserKey = row.AddedBy, CreateOn = row.DateAdded, LastCallStatusKey = row.EnquiryLeadCallStatusKey ?? 0 })).OrderByDescending(x => x.CreateOn);


                    Count = List.ToList().Count() + 1;
                }




                if (List.Where(x => CallStatuses.Contains(x.LastCallStatusKey)).Count() >= Configuation.CountLimit - 1)
                {

                    if (Configuation.CountLimit == (Count))
                    {
                        Message = Message + CallStatusName + "-" + Count + ",";

                        if (Configuation.IsSentEmail == true)
                            model.NotificationEmails = Configuation.Email;
                        if (Configuation.IsSentSms == true)
                            model.NotificationMobileNo = Configuation.Mobile;
                        model.IsUserBlocked = true;

                        AppUser UserAccount = dbContext.AppUsers.Where(x => x.RowKey == DbConstants.User.UserKey).SingleOrDefault();
                        UserAccount.IsLocked = true;

                        EnquiryUserBlockList EnquiryUserBlockList = new EnquiryUserBlockList();
                        int maxKey = dbContext.EnquiryUserBlockLists.Select(p => p.RowKey).DefaultIfEmpty().Max();
                        EnquiryUserBlockList.RowKey = maxKey + 1;
                        EnquiryUserBlockList.Descriptions = Message;
                        EnquiryUserBlockList.TotalCount = Count;
                        model.IsClosedNotification = true;
                        dbContext.EnquiryUserBlockLists.Add(EnquiryUserBlockList);
                        model.Feedback = string.Format("Your Account has been blocked for {0} ( {1} ).Please contact administrator.", Configuation.CountLimit, Message);
                    }

                }
                else
                {
                    model.Feedback = string.Format("Your may be blocked after {0} consecutive statuses ({1}).", Configuation.CountLimit, Message);
                }
            }



        }
        public List<EnquiryViewModel> GetEnquiry(EnquiryViewModel model)
        {
            try
            {
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();

                IQueryable<EnquiryViewModel> EnquiryListQuery = (from r in dbContext.Enquiries
                                                                 where (r.EnquiryName.Contains(model.SearchText) || r.MobileNumber.Contains(model.SearchText))
                                                                 select new EnquiryViewModel
                                                                 {
                                                                     RowKey = r.RowKey,
                                                                     EnquiryName = r.EnquiryName,
                                                                     MobileNumber = r.MobileNumber,
                                                                     EmailAddress = r.EmailAddress,
                                                                     BranchName = r.Branch1.BranchName,
                                                                     EmployeeKey = r.EmployeeKey ?? 0,
                                                                     EnquiryEducationQualification = r.EnquiryEducationQualification,
                                                                     EmployeeName = r.Employee.FirstName,
                                                                     EnquiryStatusesName = r.EnquiryStatu.EnquiryStatusName,
                                                                     EnquiryStatusKey = r.EnquiryStatusKey ?? 0,
                                                                     BranchKey = r.BranchKey,
                                                                     SearchEmployeeKey = r.EmployeeKey
                                                                 });
                if (Employee != null)
                {
                    if (!DbConstants.Role.AdminUserTypes.Contains(Employee.AppUser.RoleKey))
                    {
                        EnquiryListQuery = EnquiryListQuery.Where(x => x.EmployeeKey == Employee.RowKey);
                    }

                }
                return EnquiryListQuery.ToList();


            }



            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Enquiry, ActionConstants.View, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);

                return new List<EnquiryViewModel>();

            }
        }
        private bool checkMobileNumber(EnquiryViewModel model)
        {
            var result = dbContext.Enquiries.Select
              (row => new EnquiryViewModel
              {
                  MobileNumber = row.MobileNumber
              })
              .Union(dbContext.EnquiryLeads.Where(row => row.RowKey != model.EnquiryLeadKey).Select
              (row => new EnquiryViewModel
              {
                  MobileNumber = row.MobileNumber
              })).Union(dbContext.Applications.Where(row => row.EnquiryKey != model.RowKey).Select
              (row => new EnquiryViewModel
              {
                  MobileNumber = row.StudentMobile
              }))
              .Any(row => row.MobileNumber == model.MobileNumber);

            return result;
        }
        public EnquiryViewModel GetPhoneNumberLength(EnquiryViewModel model)
        {
            Country country = dbContext.Countries.Where(x => x.RowKey == model.TelephoneCodeKey).FirstOrDefault();
            if (country != null)
            {
                model.MaxPhoneLength = country.MaxPhoneLength;
                model.MinPhoneLength = country.MinPhoneLength;
            }
            Country country1 = dbContext.Countries.Where(x => x.RowKey == model.TelephoneCodeOptionalKey).FirstOrDefault();
            if (country1 != null)
            {
                model.MaxPhoneLengthOptional = country1.MaxPhoneLength;
                model.MinPhoneLengthOptional = country1.MinPhoneLength;
            }
            return model;
        }
    }
}
