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
using CITS.EduSuite.Business.Models.Resources;
using System.Data.Entity.Core.Objects;

namespace CITS.EduSuite.Business.Services
{
    public class EnquiryLeadService : IEnquiryLeadService
    {
        private EduSuiteDatabase dbContext = new EduSuiteDatabase();

        public EnquiryLeadService()
        {
            //this.dbContext = objDB;
        }
        public EnquiryLeadViewModel GetLeadsAllocation(EnquiryLeadViewModel model)
        {
            EduSuiteDatabase dbContext = new EduSuiteDatabase();
            string FromDate = model.SearchFromDate != null ? Convert.ToDateTime(model.SearchFromDate).ToString("yyyy-MM-dd") : null;
            string ToDate = model.SearchToDate != null ? Convert.ToDateTime(model.SearchToDate).ToString("yyyy-MM-dd") : null;
            ObjectParameter TotalCount = new ObjectParameter("TotalCount", typeof(Int64));

            model.LeadsList = dbContext.SpLeadSelectClient(
                model.IsNewLead,
                DbConstants.NatureOfEnquiry.Facebook,
                model.EnquiryLeadStatusKey,
                model.SearchEmployeeKey,
                model.SearchBranchKey,
                model.SearchText,
                FromDate,
                ToDate,
                model.UserKey,
                model.PageSize,
                model.PageIndex,
                TotalCount
                ).Select(row => new EnquiryLeadViewModel()
                {
                    RowKey = row.RowKey,
                    Name = row.Name,
                    MobileNumber = row.MobileNumber,
                    PhoneNumber = row.PhoneNumber,
                    MobileNumberOptional = row.MobileNumberOptional,
                    EmailAddress = row.EmailAddress,
                    BranchKey = row.BranchKey,
                    EmployeeKey = row.EmployeeKey,
                    EnquiryLeadStatusKey = row.EnquiryLeadStatusKey,
                    CreateOn = row.DateAdded,
                    TelephoneCodeKey = row.TelephoneCodeKey,
                    BranchName = row.BranchName,
                    EmployeeName = row.EmployeeName,
                    EnquiryLeadStatusName = row.EnquiryStatusName,
                    Location = row.Location,
                    DateAddedTxt = row.CreatedOnText,
                    AdName = row.AdName,
                    NatureOfEnquiryName = row.NatureOfEnquiryName





                }).ToList();
            model.TotalRecords = TotalCount.Value != DBNull.Value ? Convert.ToInt64(TotalCount.Value) : 0;
            return model;
        }
        public EnquiryLeadViewModel GetLeadValues(EnquiryLeadViewModel model)
        {
            long EmployeeKey = dbContext.Employees.Where(x => x.EmployeeCode.ToLower() == model.EmployeeCode.ToLower()).Select(x => x.RowKey).SingleOrDefault();
            short BranchKey = dbContext.Branches.Where(x => x.BranchCode.ToLower() == model.BranchCode.ToLower()).Select(x => x.RowKey).SingleOrDefault();
            // byte ServiceTypeKey = dbContext.ServiceTypes.Where(x => x.ServiceTypeCode.ToLower() == model.ServiceTypeCode.ToLower()).Select(x => x.RowKey).SingleOrDefault();


            model.EmployeeKey = EmployeeKey == 0 ? model.EmployeeKey : EmployeeKey;
            model.BranchKey = BranchKey == 0 ? model.BranchKey : BranchKey;
            // model.ServiceTypeKey = ServiceTypeKey == 0 ? model.ServiceTypeKey : ServiceTypeKey;
            return model;
        }
        public EnquiryLeadViewModel AllocateNewLeads(EnquiryLeadViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    foreach (long EnquiryLeadKey in model.EnquiryLeadKeys)
                    {
                        EnquiryLead dbEnquiryLead = dbContext.EnquiryLeads.Where(x => x.RowKey == EnquiryLeadKey).SingleOrDefault();
                        short? DepartmentKey = dbContext.Employees.Where(x => x.RowKey == model.EmployeeKey).Select(x => x.DepartmentKey).SingleOrDefault();
                        dbEnquiryLead.EmployeeKey = model.EmployeeKey;
                        dbEnquiryLead.LeadDate = model.LeadDate;
                        dbEnquiryLead.DepartmentKey = DepartmentKey;
                        if (model.SearchAllocateBranchKey != null)
                        {
                            dbEnquiryLead.BranchKey = model.SearchAllocateBranchKey ?? 0;
                        }
                    }

                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.LeadAllocation, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
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
                    ActivityLog.CreateActivityLog(MenuConstants.LeadAllocation, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, dbEx.GetBaseException().Message);
                    throw raise;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Join(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.EnquiryLead);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.LeadAllocation, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public List<EnquiryLeadViewModel> GetEnquiryLeads(EnquiryLeadViewModel model, out int TotalRecords)
        {
            var Take = model.PageSize;
            var skip = (model.PageIndex - 1) * model.PageSize;


            Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();

            IQueryable<EnquiryLeadViewModel> enquiryLeadListQuery = (from EL in dbContext.EnquiryLeads.OrderByDescending(row => new { row.DateAdded })
                                                                     join AU in dbContext.AppUsers on EL.AddedBy equals AU.RowKey

                                                                     join ELF in dbContext.EnquiryLeadFeedbacks.GroupBy(x => x.EnquiryLeadKey).Select(y => y.OrderByDescending(row => row.DateAdded).FirstOrDefault())
                                                                     on EL.RowKey equals ELF.EnquiryLeadKey into ELF
                                                                     from EJ in ELF.DefaultIfEmpty()

                                                                     join AUF in dbContext.AppUsers on EJ.AddedBy equals AUF.RowKey into AUF
                                                                     from AUJ in AUF.DefaultIfEmpty()
                                                                     orderby EJ.DateAdded descending

                                                                     where
                                                                     ((model.SearchName ?? "") == "" || EL.Name.Contains((model.SearchName ?? "").Trim()) || EL.EmailAddress.Contains((model.SearchName ?? "").Trim()) || EL.District.Contains((model.SearchName ?? "").Trim())
                                                                     || EL.Location.Contains((model.SearchName ?? "").Trim()) || EL.MobileNumber.Contains((model.SearchName ?? "").Trim()) || EL.MobileNumberOptional.Contains((model.SearchName ?? "").Trim()))

                                                                     select new EnquiryLeadViewModel
                                                                     {
                                                                         RowKey = EL.RowKey,
                                                                         BranchKey = EL.BranchKey,
                                                                         EmployeeKey = EL.EmployeeKey ?? 0,
                                                                         Name = EL.Name,
                                                                         EmailAddress = EL.EmailAddress,
                                                                         Qualification = EL.Qualification,
                                                                         EmployeeName = EL.Employee.FirstName + (EL.Employee.MiddleName ?? "") + " " + EL.Employee.LastName,
                                                                         MobileNumber = EL.MobileNumber,
                                                                         MobileNumberOptional = EL.MobileNumberOptional,
                                                                         Remarks = EL.Remarks,
                                                                         BranchName = EL.Branch.BranchName,
                                                                         EnquiryLeadStatusKey = EL.EnquiryLeadStatusKey ?? 0,
                                                                         EnquiryLeadStatusName = EL.EnquiryStatu.EnquiryStatusName,
                                                                         LeadDate = EL.LeadDate,
                                                                         NextCallSchedule = EJ.NextCallSchedule,
                                                                         EnquiryLeadCallStatusKey = EJ.EnquiryLeadCallStatusKey ?? 0,
                                                                         EnquiryLeadCallStatusName = EJ.EnquiryCallStatu.EnquiryCallStatusName,
                                                                         CreatedBy = AU.FirstName + " " + (AU.MiddleName ?? "") + " " + AU.LastName,
                                                                         CreateOn = EL.DateAdded,
                                                                         IsEditable = true,
                                                                         District = EL.District,
                                                                         Location = EL.Location,
                                                                         LeadReference = dbContext.EnquiryLeads.Where(x => x.RowKey == EL.RefLeadID).Select(y => y.Name + " (" + y.MobileNumber + ")").FirstOrDefault(),
                                                                         NatureOfEnquiryKey = EL.NatureOfEnquiryKey
                                                                     });

            if (Employee != null)
            {
                if (Employee.BranchAccess != null)
                {
                    var Branches = Employee.BranchAccess.Split(',').Select(Int16.Parse).ToList();
                    enquiryLeadListQuery = enquiryLeadListQuery.Where(row => Branches.Contains(row.BranchKey ?? 0));
                }
            }

            if (model.SearchBranchKey != null)
            {
                enquiryLeadListQuery = enquiryLeadListQuery.Where(row => row.BranchKey == model.SearchBranchKey);
            }
            if (model.SearchEmployeeKey != null)
                enquiryLeadListQuery = (from E in enquiryLeadListQuery
                                        join EFH in dbContext.EmployeeFileHandovers.Where(row => row.IsActive) on new { FileKey = E.RowKey, FileHandoverTypeKey = DbConstants.FileHandoverType.EnquiryLead } equals new { EFH.FileKey, EFH.FileHandoverTypeKey }
                                        into EFJ
                                        from EFH in EFJ.DefaultIfEmpty()
                                        where (E.EmployeeKey == model.SearchEmployeeKey && (EFH.EmployeeFromKey == null || EFH.EmployeeFromKey != model.SearchEmployeeKey) && (EFH.FileKey == null || EFH.FileKey == E.RowKey)) ||
                                         ((EFH.EmployeeToKey == model.SearchEmployeeKey) && (EFH.FileKey == E.RowKey))
                                        select E);

            if (model.NatureOfEnquiryKey != null)
            {
                enquiryLeadListQuery = enquiryLeadListQuery.Where(row => row.NatureOfEnquiryKey == model.NatureOfEnquiryKey);
            }


            if (model.SearchCallStatusKey != null)
                enquiryLeadListQuery = enquiryLeadListQuery.Where(row => row.EnquiryLeadCallStatusKey == model.SearchCallStatusKey);

            if (model.SearchEnquiryLeadStatusKey != null)
            {
                enquiryLeadListQuery = enquiryLeadListQuery.Where(row => row.EnquiryLeadStatusKey == model.SearchEnquiryLeadStatusKey);
            }
            else
            {
                enquiryLeadListQuery = enquiryLeadListQuery.Where(row => row.EnquiryLeadStatusKey == DbConstants.EnquiryStatus.FollowUp || row.EnquiryLeadStatusKey == 0);
            }

            if (model.SearchFromDate != null && model.SearchToDate != null)
                enquiryLeadListQuery = enquiryLeadListQuery.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.LeadDate) >= System.Data.Entity.DbFunctions.TruncateTime(model.SearchFromDate) && System.Data.Entity.DbFunctions.TruncateTime(row.LeadDate) <= System.Data.Entity.DbFunctions.TruncateTime(model.SearchToDate));
            else if (model.SearchFromDate != null)
                enquiryLeadListQuery = enquiryLeadListQuery.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.LeadDate) >= System.Data.Entity.DbFunctions.TruncateTime(model.SearchFromDate));
            else if (model.SearchToDate != null)
                enquiryLeadListQuery = enquiryLeadListQuery.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.LeadDate) <= System.Data.Entity.DbFunctions.TruncateTime(model.SearchToDate));

            var leadlist = enquiryLeadListQuery.OrderByDescending(row => row.CreateOn).ToList();

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

                leadlist = (from EL in leadlist
                            join FNEH in EmployeeHeirarchyDetailsList.Where(x => x.IsActive) on EL.EmployeeKey equals FNEH.ToEmployeeKey
                            select new EnquiryLeadViewModel
                            {
                                RowKey = EL.RowKey,
                                BranchKey = EL.BranchKey,
                                EmployeeKey = EL.EmployeeKey ?? 0,
                                Name = EL.Name,
                                EmailAddress = EL.EmailAddress,
                                Qualification = EL.Qualification,
                                EmployeeName = EL.EmployeeName,
                                MobileNumber = EL.MobileNumber,
                                MobileNumberOptional = EL.MobileNumberOptional,
                                Remarks = EL.Remarks,
                                BranchName = EL.BranchName,
                                EnquiryLeadStatusKey = EL.EnquiryLeadStatusKey ?? 0,
                                EnquiryLeadStatusName = EL.EnquiryLeadStatusName,
                                LeadDate = EL.LeadDate,
                                NextCallSchedule = EL.NextCallSchedule,
                                EnquiryLeadCallStatusKey = EL.EnquiryLeadCallStatusKey ?? 0,
                                EnquiryLeadCallStatusName = EL.EnquiryLeadCallStatusName,
                                CreatedBy = EL.CreatedBy,
                                CreateOn = EL.CreateOn,
                                IsEditable = FNEH != null ? FNEH.DataAccess : false,
                                District = EL.District,
                                Location = EL.Location,
                                LeadReference = EL.LeadReference
                            }).ToList();
            }

            TotalRecords = leadlist.Count();
            var enquiryLeadList = leadlist.OrderByDescending(row => row.CreateOn).Skip(skip).Take(Take).ToList();

            return enquiryLeadList;
        }
        public EnquiryLeadViewModel GetEnquiryLeadById(EnquiryLeadViewModel model)
        {
            try
            {
                EnquiryLeadViewModel objViewModel = dbContext.EnquiryLeads.Where(row => row.RowKey == model.RowKey).Select(row => new EnquiryLeadViewModel
                {
                    RowKey = row.RowKey,
                    Name = row.Name,
                    TelephoneCodeKey = row.TelephoneCodeKey ?? 0,
                    MobileNumber = row.MobileNumber,
                    TelephoneCodeOptionalKey = row.TelePhoneCodeOptionalkey,
                    MobileNumberOptional = row.MobileNumberOptional,
                    PhoneNumber = row.PhoneNumber,
                    EmailAddress = row.EmailAddress,
                    Qualification = row.Qualification,
                    BranchKey = row.BranchKey,
                    EmployeeKey = row.EmployeeKey ?? 0,
                    Remarks = row.Remarks,
                    LeadDate = row.LeadDate,
                    LeadFrom = row.LeadFrom,
                    District = row.District,
                    Location = row.Location,
                    LeadApiKey = row.LeadApiKey,
                    AdsAPIKey = row.AdsAPIKey,
                    NatureOfEnquiryKey = row.NatureOfEnquiryKey,
                    MaxPhoneLength = row.Country.MaxPhoneLength,
                    MinPhoneLength = row.Country.MinPhoneLength
                }).SingleOrDefault();
                if (objViewModel == null)
                {
                    objViewModel = new EnquiryLeadViewModel();

                }
                EnquiryLeadFeedback EnquiryLeadFeedbackModel = dbContext.EnquiryLeadFeedbacks.Where(x => x.EnquiryLeadKey == model.RowKey).OrderByDescending(x => x.DateAdded).Take(1).SingleOrDefault();
                if (EnquiryLeadFeedbackModel != null)
                {
                    objViewModel.EnquiryLeadFeedbackKey = EnquiryLeadFeedbackModel.RowKey;
                    objViewModel.CallTypeKey = EnquiryLeadFeedbackModel.CallTypeKey ?? 0;
                    objViewModel.EnquiryLeadCallStatusKey = EnquiryLeadFeedbackModel.EnquiryLeadCallStatusKey;
                    objViewModel.NextCallSchedule = EnquiryLeadFeedbackModel.NextCallSchedule;
                    objViewModel.CallDuration = EnquiryLeadFeedbackModel.CallDuration;
                    objViewModel.Feedback = EnquiryLeadFeedbackModel.Feedback;
                    objViewModel.EnquiryLeadStatusKey = EnquiryLeadFeedbackModel.EnquiryLead.EnquiryLeadStatusKey;
                }

                FillEnquiryLeadDrodownLists(objViewModel);
                return objViewModel;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.EnquiryLead, ActionConstants.View, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return new EnquiryLeadViewModel();
            }
        }
        public List<EnquiryLeadFeedbackViewModel> GetEnquiryLeadFeedbackByLeadId(long id)
        {
            List<EnquiryLeadFeedbackViewModel> EnquiryLeadFeedbacks = dbContext.EnquiryLeadFeedbacks.OrderByDescending(row => row.DateAdded).Where(row => row.EnquiryLeadKey == id).Select(row => new EnquiryLeadFeedbackViewModel
            {
                RowKey = row.RowKey,
                Feedback = row.Feedback,
                CallDuration = row.CallDuration,
                NextCallSchedule = row.NextCallSchedule,
                EnquiryKey = row.EnquiryLead.EnquiryKey,
                CallTypeKey = row.CallTypeKey ?? 0,
                EnquiryLeadCallStatusKey = row.EnquiryLeadCallStatusKey,
                CallTypeName = row.CallType.CallTypeName,
                EnquiryLeadCallStatusName = row.EnquiryCallStatu.EnquiryCallStatusName,
                EnquiryLeadKey = row.EnquiryLeadKey,
                CallDate = row.DateAdded
            }).ToList();
            return EnquiryLeadFeedbacks;
        }
        public EnquiryLeadFeedbackViewModel GetEnquiryLeadFeedbackById(EnquiryLeadFeedbackViewModel model)
        {
            EnquiryLeadFeedbackViewModel objViewModel = new EnquiryLeadFeedbackViewModel();

            objViewModel = dbContext.EnquiryLeadFeedbacks.Where(row => row.RowKey == model.RowKey).Select(row => new EnquiryLeadFeedbackViewModel
            {
                RowKey = row.RowKey,
                Feedback = row.Feedback,
                CallDuration = row.CallDuration,
                NextCallSchedule = row.NextCallSchedule,
                CallTypeKey = row.CallTypeKey ?? 0,
                EnquiryLeadStatusKey = row.EnquiryLead.EnquiryLeadStatusKey,
                EnquiryLeadCallStatusKey = row.EnquiryLeadCallStatusKey,
                EnquiryLeadKey = row.EnquiryLeadKey,
                DateAdded = row.DateAdded,

            }).SingleOrDefault();
            if (objViewModel == null)
            {
                objViewModel = new EnquiryLeadFeedbackViewModel();
                objViewModel.EnquiryLeadKey = model.EnquiryLeadKey;

                objViewModel.EnquiryLeadStatusKey = DbConstants.EnquiryStatus.FollowUp;
                EnquiryLead enquiryLead = dbContext.EnquiryLeads.Where(row => row.RowKey == objViewModel.EnquiryLeadKey).FirstOrDefault();
                objViewModel.EnquiryLeadStatusKey = enquiryLead.EnquiryLeadStatusKey;
                EnquiryLeadFeedback dbFeedback = dbContext.EnquiryLeadFeedbacks.Where(x => x.EnquiryLeadKey == objViewModel.EnquiryLeadKey).OrderBy(x => x.DateAdded).FirstOrDefault();
                if (dbFeedback != null)
                {
                    objViewModel.EnquiryLeadCallStatusKey = dbFeedback.EnquiryLeadCallStatusKey;


                }

            }

            ReferenceList Refmodel = new ReferenceList();
            Refmodel.ReferenceName = "";
            Refmodel.TelephoneCodeKey = DbConstants.Country.India;
            objViewModel.LeadEmployeeKey = objViewModel.EmployeeKey;
            objViewModel.ReferenceList.Add(Refmodel);
            FillTelephoneCodes(Refmodel);
            FillAcademicTerms(Refmodel);



            FillFeedbackDrodownLists(objViewModel);
            return objViewModel;
        }
        //public EnquiryLeadViewModel UpdateEnquiryLeads(List<EnquiryLeadViewModel> model)
        //{
        //    EnquiryLeadViewModel enquiryLeadViewModel = new EnquiryLeadViewModel();
        //    FillEnquiryLeadDrodownLists(enquiryLeadViewModel);
        //    using (var transaction = dbContext.Database.BeginTransaction())
        //    {

        //        try
        //        {

        //            CreateEnquiryLead(model.Where(row => row.RowKey == 0).ToList());
        //            UpdateEnquiryLead(model.Where(row => row.RowKey != 0).ToList());
        //            dbContext.SaveChanges();
        //            transaction.Commit();

        //            enquiryLeadViewModel.Message = ApplicationResources.Success;
        //            enquiryLeadViewModel.IsSuccessful = true;
        //            ActivityLog.CreateActivityLog(MenuConstants.EnquiryLead, (model.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Info, enquiryLeadViewModel.RowKey, enquiryLeadViewModel.Message);
        //        }
        //        catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
        //        {
        //            Exception raise = dbEx;
        //            foreach (var validationErrors in dbEx.EntityValidationErrors)
        //            {
        //                foreach (var validationError in validationErrors.ValidationErrors)
        //                {
        //                    string message = string.Format("{0}:{1}", validationErrors.Entry.Entity.ToString(), validationError.ErrorMessage);
        //                    //raise a new exception inserting the current one as the InnerException
        //                    raise = new InvalidOperationException(message, raise);
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            transaction.Rollback();
        //            enquiryLeadViewModel.Message = ApplicationResources.FailedToSaveEnquiryLead;
        //            enquiryLeadViewModel.IsSuccessful = false;
        //            ActivityLog.CreateActivityLog(MenuConstants.EnquiryLead, (model.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, enquiryLeadViewModel.RowKey, ex.GetBaseException().Message);
        //        }
        //    }
        //    return enquiryLeadViewModel;
        //}
        public EnquiryLeadViewModel UpdateEnquiryLeads(List<EnquiryLeadViewModel> modelList)
        {
            EnquiryLeadViewModel enquiryLeadViewModel = new EnquiryLeadViewModel();
            FillEnquiryLeadDrodownLists(enquiryLeadViewModel);
            using (var transaction = dbContext.Database.BeginTransaction())
            {

                try
                {

                    modelList = modelList.Except(CheckBulkMobileNumberExists(modelList)).ToList();
                    Int64 maxKey = dbContext.EnquiryLeads.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    dbContext.Configuration.AutoDetectChangesEnabled = false;
                    int Count = 0;
                    foreach (EnquiryLeadViewModel model in modelList)
                    {
                        if (DbConstants.NatureOfEnquiryAPI.NatureOfEnquiryAPIList.Contains(model.NatureOfEnquiryKey ?? 0))
                        {
                            CreatAdsApiDetails(model);
                        }
                        ++Count;
                        if (model.RowKey == 0)
                        {
                            dbContext.AddToContext(new EnquiryLead
                            {
                                RowKey = ++maxKey,
                                Name = model.Name,
                                EmailAddress = model.EmailAddress,
                                TelephoneCodeKey = model.TelephoneCodeKey,
                                MobileNumber = model.MobileNumber,
                                TelePhoneCodeOptionalkey = model.TelephoneCodeOptionalKey,
                                MobileNumberOptional = model.MobileNumberOptional,
                                PhoneNumber = model.PhoneNumber,
                                Qualification = model.Qualification,
                                BranchKey = model.BranchKey,
                                Remarks = model.Remarks,
                                DepartmentKey = model.DepartmentKey == 0 ? null : model.DepartmentKey,
                                EmployeeKey = model.EmployeeKey,
                                LeadDate = model.LeadDate,
                                IsNewLead = 0,
                                LeadFrom = model.LeadFrom,
                                EnquiryLeadStatusKey = model.EnquiryLeadStatusKey,

                                Location = model.Location,
                                District = model.District,
                                NatureOfEnquiryKey = model.NatureOfEnquiryKey,
                                LeadApiKey = model.LeadApiKey,
                                AdsAPIKey = model.AdsAPIKey

                            }, Count);

                        }
                        else
                        {
                            byte? IsNewLead = dbContext.EnquiryLeads.Where(x => x.RowKey == model.RowKey).Select(y => y.IsNewLead).FirstOrDefault();

                            dbContext.AttachToContext(new EnquiryLead
                            {
                                RowKey = model.RowKey,
                                Name = model.Name,
                                EmailAddress = model.EmailAddress,
                                TelephoneCodeKey = model.TelephoneCodeKey,
                                MobileNumber = model.MobileNumber,
                                TelePhoneCodeOptionalkey = model.TelephoneCodeOptionalKey,
                                MobileNumberOptional = model.MobileNumberOptional,
                                PhoneNumber = model.PhoneNumber,
                                Qualification = model.Qualification,
                                BranchKey = model.BranchKey,
                                Remarks = model.Remarks,
                                DepartmentKey = model.DepartmentKey == 0 ? null : model.DepartmentKey,
                                EmployeeKey = model.EmployeeKey,
                                LeadDate = model.LeadDate,
                                IsNewLead = IsNewLead,
                                LeadFrom = model.LeadFrom,
                                EnquiryLeadStatusKey = model.EnquiryLeadStatusKey,
                                Location = model.Location,
                                District = model.District,
                                NatureOfEnquiryKey = model.NatureOfEnquiryKey,
                            }, Count);
                        }

                    }
                    dbContext.SaveChanges();
                    dbContext.Configuration.AutoDetectChangesEnabled = true;
                    transaction.Commit();

                    enquiryLeadViewModel.Message = EduSuiteUIResources.Success;
                    enquiryLeadViewModel.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EnquiryLead, ActionConstants.Edit, DbConstants.LogType.Info, DbConstants.User.UserKey, enquiryLeadViewModel.Message);

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
                    ActivityLog.CreateActivityLog(MenuConstants.EnquiryLead, ActionConstants.Edit, DbConstants.LogType.Error, DbConstants.User.UserKey, dbEx.GetBaseException().Message);

                    throw raise;

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    enquiryLeadViewModel.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.EnquiryLead);
                    enquiryLeadViewModel.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EnquiryLead, ActionConstants.Edit, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);

                }
            }
            return enquiryLeadViewModel;
        }
        public EnquiryLeadViewModel CreatAdsApiDetails(EnquiryLeadViewModel model)
        {
            bool isAdd = false;
            if (model.AdKey != null)
            {
                Int64 maxKey = dbContext.AdsAPIs.Select(p => p.RowKey).DefaultIfEmpty().Max();
                AdsAPI dbAdsApi = new AdsAPI();
                dbAdsApi = dbContext.AdsAPIs.Where(x => x.AdKey == model.AdKey).SingleOrDefault();
                if (dbAdsApi == null)
                {
                    dbAdsApi = new AdsAPI();
                    dbAdsApi.RowKey = Convert.ToInt64(maxKey + 1);
                    dbAdsApi.AdKey = model.AdKey;
                    isAdd = true;
                }

                dbAdsApi.AdName = model.AdName;
                if (isAdd == true)
                {
                    dbContext.AdsAPIs.Add(dbAdsApi);
                }
                model.AdsAPIKey = dbAdsApi.RowKey;


            }
            return model;
        }
        private void CreateEnquiryLead(List<EnquiryLeadViewModel> modelList)
        {
            List<EnquiryLead> EnquiryLeads = new List<EnquiryLead>();
            Int64 maxKey = dbContext.EnquiryLeads.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (EnquiryLeadViewModel model in modelList)
            {
                //if (model.EmployeeCode != null)
                //{
                //    Employee employee = dbContext.Employees.SingleOrDefault(row => row.EmployeeCode == model.EmployeeCode);
                //    if (employee != null)
                //    {
                //        model.EmployeeKey = employee.RowKey;
                //        model.DepartmentKey = employee.DepartmentKey;
                //        model.BranchKey = employee.BranchKey;
                //    }
                //}
                //else 
                if (model.EmployeeKey != null)
                {
                    model.DepartmentKey = dbContext.Employees.Where(row => row.RowKey == model.EmployeeKey).Select(row => row.DepartmentKey).FirstOrDefault();
                }
                EnquiryLead enquiryLeadModel = new EnquiryLead();
                enquiryLeadModel.RowKey = Convert.ToInt64(maxKey + 1);
                enquiryLeadModel.Name = model.Name;
                enquiryLeadModel.EmailAddress = model.EmailAddress;
                enquiryLeadModel.TelephoneCodeKey = model.TelephoneCodeKey;
                enquiryLeadModel.MobileNumber = model.MobileNumber;
                enquiryLeadModel.TelePhoneCodeOptionalkey = model.TelephoneCodeOptionalKey;
                enquiryLeadModel.MobileNumberOptional = model.MobileNumberOptional;
                enquiryLeadModel.PhoneNumber = model.PhoneNumber;
                enquiryLeadModel.Qualification = model.Qualification;
                // enquiryLeadModel.IsSpam = false;
                enquiryLeadModel.BranchKey = model.BranchKey;
                enquiryLeadModel.Remarks = model.Remarks;
                enquiryLeadModel.DepartmentKey = model.DepartmentKey == 0 ? null : model.DepartmentKey;
                enquiryLeadModel.EmployeeKey = model.EmployeeKey;
                enquiryLeadModel.LeadDate = model.LeadDate;
                enquiryLeadModel.IsNewLead = 0;
                enquiryLeadModel.LeadFrom = model.LeadFrom;
                enquiryLeadModel.EnquiryLeadStatusKey = model.EnquiryLeadStatusKey;
                enquiryLeadModel.District = model.District;
                enquiryLeadModel.Location = model.Location;
                EnquiryLeads.Add(enquiryLeadModel);


                if (model.EnquiryLeadStatusKey != null && modelList.Count == 1)
                {
                    EnquiryLeadFeedbackViewModel EnquiryFeedbackModel = new EnquiryLeadFeedbackViewModel();

                    EnquiryFeedbackModel.EnquiryLeadKey = enquiryLeadModel.RowKey;
                    EnquiryFeedbackModel.CallTypeKey = model.CallTypeKey ?? 0;
                    EnquiryFeedbackModel.EnquiryLeadCallStatusKey = model.EnquiryLeadCallStatusKey;
                    EnquiryFeedbackModel.NextCallSchedule = model.NextCallSchedule;
                    EnquiryFeedbackModel.CallDuration = model.CallDuration;
                    EnquiryFeedbackModel.Feedback = model.Feedback;
                    EnquiryFeedbackModel.UserKey = DbConstants.User.UserKey;
                    EnquiryFeedbackModel.EnquiryLeadStatusKey = model.EnquiryLeadStatusKey;

                    EnquiryFeedbackModel.RowKey = 0;
                    CreateLeadFeedback(EnquiryFeedbackModel, enquiryLeadModel);
                }

                maxKey++;
            }
            dbContext.EnquiryLeads.AddRange(EnquiryLeads);

        }
        private void UpdateEnquiryLead(List<EnquiryLeadViewModel> modelList)
        {

            foreach (EnquiryLeadViewModel model in modelList)
            {
                //if (model.EmployeeCode != null)
                //{
                //    Employee employee = dbContext.Employees.SingleOrDefault(row => row.EmployeeCode == model.EmployeeCode);
                //    if (employee != null)
                //    {
                //        model.EmployeeKey = employee.RowKey;
                //        model.DepartmentKey = employee.DepartmentKey;
                //        model.BranchKey = employee.BranchKey;
                //    }
                //}
                //else 
                if (model.EmployeeKey != null)
                {
                    model.DepartmentKey = dbContext.Employees.Where(row => row.RowKey == model.EmployeeKey).Select(row => row.DepartmentKey).FirstOrDefault();
                }
                EnquiryLead enquiryLeadModel = dbContext.EnquiryLeads.SingleOrDefault(row => row.RowKey == model.RowKey);
                enquiryLeadModel.Name = model.Name;
                enquiryLeadModel.EmailAddress = model.EmailAddress;
                enquiryLeadModel.TelephoneCodeKey = model.TelephoneCodeKey;
                enquiryLeadModel.MobileNumber = model.MobileNumber;
                enquiryLeadModel.TelePhoneCodeOptionalkey = model.TelephoneCodeOptionalKey;
                enquiryLeadModel.MobileNumberOptional = model.MobileNumberOptional;
                enquiryLeadModel.PhoneNumber = model.PhoneNumber;
                enquiryLeadModel.Qualification = model.Qualification;
                enquiryLeadModel.BranchKey = model.BranchKey;
                enquiryLeadModel.Remarks = model.Remarks;
                enquiryLeadModel.DepartmentKey = model.DepartmentKey == 0 ? null : model.DepartmentKey;
                enquiryLeadModel.EmployeeKey = model.EmployeeKey;
                enquiryLeadModel.District = model.District;
                enquiryLeadModel.Location = model.Location;
                if (model.LeadDate != null)
                {
                    enquiryLeadModel.LeadDate = model.LeadDate;
                }
                enquiryLeadModel.LeadFrom = model.LeadFrom;

                if (model.EnquiryLeadStatusKey != null && modelList.Count == 1)
                {
                    EnquiryLeadFeedbackViewModel EnquiryFeedbackModel = new EnquiryLeadFeedbackViewModel();

                    EnquiryFeedbackModel.EnquiryLeadKey = enquiryLeadModel.RowKey;
                    EnquiryFeedbackModel.CallTypeKey = model.CallTypeKey ?? 0;
                    EnquiryFeedbackModel.EnquiryLeadCallStatusKey = model.EnquiryLeadCallStatusKey;
                    EnquiryFeedbackModel.NextCallSchedule = model.NextCallSchedule;
                    EnquiryFeedbackModel.CallDuration = model.CallDuration;
                    EnquiryFeedbackModel.Feedback = model.Feedback;
                    EnquiryFeedbackModel.UserKey = DbConstants.User.UserKey;
                    EnquiryFeedbackModel.EnquiryLeadStatusKey = model.EnquiryLeadStatusKey;
                    EnquiryFeedbackModel.RowKey = model.EnquiryLeadFeedbackKey ?? 0;
                    EnquiryFeedbackModel.RowKey = 0;
                    if (EnquiryFeedbackModel.RowKey == 0)
                    {
                        CreateLeadFeedback(EnquiryFeedbackModel, enquiryLeadModel);
                    }
                    else
                    {
                        UpdateLeadFeedback(EnquiryFeedbackModel);
                    }


                }
            }
        }

        #region REMOVE
        //public EnquiryLeadFeedbackViewModel CreateEnquiryLeadFeedback(EnquiryLeadFeedbackViewModel model)
        //{
        //    FillFeedbackDrodownLists(model);
        //    using (var transaction = dbContext.Database.BeginTransaction())
        //    {

        //        try
        //        {

        //            EnquiryLeadFeedback enquiryLeadFeedbackModel = new EnquiryLeadFeedback();

        //            Int64 maxKey = dbContext.EnquiryLeadFeedbacks.Select(p => p.RowKey).DefaultIfEmpty().Max();
        //            enquiryLeadFeedbackModel.RowKey = Convert.ToInt64(maxKey + 1);
        //            enquiryLeadFeedbackModel.Feedback = model.Feedback;
        //            enquiryLeadFeedbackModel.CallDuration = model.CallDuration;
        //            enquiryLeadFeedbackModel.CallTypeKey = model.CallTypeKey;
        //            enquiryLeadFeedbackModel.NextCallSchedule = model.NextCallSchedule;
        //            enquiryLeadFeedbackModel.EnquiryLeadKey = model.EnquiryLeadKey;
        //            enquiryLeadFeedbackModel.EnquiryLeadCallStatusKey = model.EnquiryLeadCallStatusKey;

        //            dbContext.EnquiryLeadFeedbacks.Add(enquiryLeadFeedbackModel);


        //            dbContext.SaveChanges();
        //            transaction.Commit();

        //            model.Message = EduSuiteUIResources.Success;
        //            model.IsSuccessful = true;
        //        }
        //        catch (Exception ex)
        //        {
        //            transaction.Rollback();
        //            model.Message = EduSuiteUIResources.FailedToSaveEnquiryLead;
        //            model.IsSuccessful = false;
        //        }
        //    }
        //    return model;
        //}

        //public EnquiryLeadFeedbackViewModel UpdateEnquiryLeadFeedback(EnquiryLeadFeedbackViewModel model)
        //{
        //    FillFeedbackDrodownLists(model);
        //    using (var transaction = dbContext.Database.BeginTransaction())
        //    {

        //        try
        //        {

        //            EnquiryLeadFeedback enquiryLeadFeedbackModel = new EnquiryLeadFeedback();

        //            enquiryLeadFeedbackModel = dbContext.EnquiryLeadFeedbacks.SingleOrDefault(row => row.RowKey == model.RowKey);
        //            enquiryLeadFeedbackModel.Feedback = model.Feedback;
        //            enquiryLeadFeedbackModel.CallDuration = model.CallDuration;
        //            enquiryLeadFeedbackModel.CallTypeKey = model.CallTypeKey;
        //            enquiryLeadFeedbackModel.NextCallSchedule = model.NextCallSchedule;
        //            enquiryLeadFeedbackModel.EnquiryLeadKey = model.EnquiryLeadKey;
        //            enquiryLeadFeedbackModel.EnquiryLeadCallStatusKey = model.EnquiryLeadCallStatusKey;

        //            dbContext.SaveChanges();
        //            transaction.Commit();

        //            model.Message = EduSuiteUIResources.Success;
        //            model.IsSuccessful = true;
        //        }
        //        catch (Exception ex)
        //        {
        //            transaction.Rollback();
        //            model.Message = EduSuiteUIResources.FailedToSaveEnquiryLead;
        //            model.IsSuccessful = false;
        //        }
        //    }
        //    return model;
        //}

        //public EnquiryLeadViewModel DeleteEnquiryLead(EnquiryLeadViewModel model)
        //{
        //    using (var transaction = dbContext.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            EnquiryLead enquiryLead = dbContext.EnquiryLeads.SingleOrDefault(row => row.RowKey == model.RowKey);
        //            List<EnquiryLeadFeedback> enquiryLeadFeedbackList = dbContext.EnquiryLeadFeedbacks.Where(row => row.EnquiryLeadKey == model.RowKey).ToList();
        //            enquiryLeadFeedbackList.ForEach(enquiryLeadFeedback => dbContext.EnquiryLeadFeedbacks.Remove(enquiryLeadFeedback));
        //            dbContext.EnquiryLeads.Remove(enquiryLead);
        //            dbContext.SaveChanges();
        //            transaction.Commit();
        //            model.Message = EduSuiteUIResources.Success;
        //            model.IsSuccessful = true;
        //        }
        //        catch (DbUpdateException ex)
        //        {
        //            transaction.Rollback();
        //            if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
        //            {
        //                model.Message = EduSuiteUIResources.CantDeleteEnquiryLead;
        //                model.IsSuccessful = false;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            transaction.Rollback();
        //            model.Message = EduSuiteUIResources.FailedToDeleteEnquiryLead;
        //            model.IsSuccessful = false;
        //        }
        //    }
        //    //}
        //    return model;
        //}


        //public EnquiryLeadFeedbackViewModel DeleteEnquiryLeadFeeback(EnquiryLeadFeedbackViewModel model)
        //{
        //    using (var transaction = dbContext.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            EnquiryLeadFeedback enquiryLeadFeedback = dbContext.EnquiryLeadFeedbacks.SingleOrDefault(row => row.RowKey == model.RowKey);
        //            dbContext.EnquiryLeadFeedbacks.Remove(enquiryLeadFeedback);
        //            dbContext.SaveChanges();
        //            transaction.Commit();
        //            model.Message = EduSuiteUIResources.Success;
        //            model.IsSuccessful = true;
        //        }
        //        catch (DbUpdateException ex)
        //        {
        //            transaction.Rollback();
        //            if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
        //            {
        //                model.Message = EduSuiteUIResources.CantDeleteEnquiryLead;
        //                model.IsSuccessful = false;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            transaction.Rollback();
        //            model.Message = EduSuiteUIResources.FailedToDeleteEnquiryLead;
        //            model.IsSuccessful = false;
        //        }
        //    }
        //    //}
        //    return model;
        //}

        #endregion REMOVE
        public EnquiryLeadFeedbackViewModel CreateEnquiryLeadFeedback(EnquiryLeadFeedbackViewModel model)
        {
            FillFeedbackDrodownLists(model);
            using (var transaction = dbContext.Database.BeginTransaction())
            {

                try
                {
                    //model.EnquiryLeadKey = dbContext.EnquiryLeadFeedbacks.Where(x => x.RowKey == model.EnquiryLeadKey).Select(x => x.EnquiryLeadKey).SingleOrDefault();
                    EnquiryLead EnquiryLeadModel = dbContext.EnquiryLeads.SingleOrDefault(x => x.RowKey == model.EnquiryLeadKey);

                    CreateLeadFeedback(model, EnquiryLeadModel);

                    //if (model.EmployeeKey != null && EnquiryLeadModel.EmployeeKey != null)
                    //{
                    //    List<EmployeeFileHandOverViewModel> FileHandOverList = new List<EmployeeFileHandOverViewModel>();
                    //    EmployeeFileHandOverViewModel FileHandOvers = new EmployeeFileHandOverViewModel();
                    //    FileHandOvers.FileHandoverTypeKey = DbConstants.FileHandoverType.EnquiryLead;
                    //    FileHandOvers.EmployeeFromKey = EnquiryLeadModel.EmployeeKey ?? 0;
                    //    FileHandOvers.EmployeeToKey = model.EmployeeKey ?? 0;
                    //    FileHandOvers.FileKey = EnquiryLeadModel.RowKey;
                    //    FileHandOvers.IsActive = true;
                    //    FileHandOverList.Add(FileHandOvers);
                    //    UpdateEmployeesFileHandover(FileHandOverList);
                    //}

                    dbContext.SaveChanges();
                    transaction.Commit();


                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.EnquiryLead, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.EnquiryLeadFeedback);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EnquiryLead, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public EnquiryLeadFeedbackViewModel UpdateEnquiryLeadFeedback(EnquiryLeadFeedbackViewModel model)
        {
            FillFeedbackDrodownLists(model);
            using (var transaction = dbContext.Database.BeginTransaction())
            {

                try
                {

                    UpdateLeadFeedback(model);
                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.EnquiryLead, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.EnquiryLeadFeedback);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EnquiryLead, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public void CreateReference(EnquiryLeadFeedbackViewModel model, EnquiryLead EnquiryLead)
        {
            Int64 maxKey = dbContext.EnquiryLeads.Select(p => p.RowKey).DefaultIfEmpty().Max();
            //List<EnquiryLead> EnquiryLeads = new List<EnquiryLead>();
            foreach (ReferenceList item in model.ReferenceList)
            {
                if (item.AcademicTermKey != null)
                {

                    EnquiryLead dbmodel = new EnquiryLead();
                    dbmodel.RowKey = (maxKey + 1);
                    dbmodel.MobileNumber = item.MobileNumber;
                    dbmodel.Name = item.ReferenceName;
                    dbmodel.EmployeeKey = EnquiryLead.EmployeeKey;
                    dbmodel.LeadDate = DateTimeUTC.Now;
                    dbmodel.RefLeadID = model.EnquiryLeadKey;
                    dbmodel.IsNewLead = 0;
                    //dbmodel.IsSpam = false;
                    dbmodel.TelephoneCodeKey = item.TelephoneCodeKey;
                    dbmodel.BranchKey = EnquiryLead.BranchKey;
                    dbContext.EnquiryLeads.Add(dbmodel);
                    maxKey++;
                }
            }
        }
        private void CreateLeadFeedback(EnquiryLeadFeedbackViewModel model, EnquiryLead EnquiryLeadModel)
        {
            dbContext.EnquiryLeadFeedbacks.Where(x => x.EnquiryLeadKey == model.EnquiryLeadKey).ToList().ForEach(x =>
            {
                x.EnquiryFeedbackReminderStatus = false;
                x.IsLastFeedback = false;
            });

            EnquiryLeadFeedback enquiryLeadFeedbackModel = new EnquiryLeadFeedback();
            Int64 maxKey = dbContext.EnquiryLeadFeedbacks.Select(p => p.RowKey).DefaultIfEmpty().Max();
            enquiryLeadFeedbackModel.RowKey = Convert.ToInt64(maxKey + 1);
            enquiryLeadFeedbackModel.Feedback = model.Feedback;
            enquiryLeadFeedbackModel.CallDuration = model.CallDuration;
            enquiryLeadFeedbackModel.CallTypeKey = model.CallTypeKey;
            enquiryLeadFeedbackModel.NextCallSchedule = model.NextCallSchedule;
            enquiryLeadFeedbackModel.EnquiryLeadKey = model.EnquiryLeadKey;
            enquiryLeadFeedbackModel.Feedback = model.Feedback;
            EnquiryLeadModel.IsNewLead = 1;

            if (model.NextCallSchedule == null)
            {
                enquiryLeadFeedbackModel.EnquiryFeedbackReminderStatus = false;
            }
            else
            {
                enquiryLeadFeedbackModel.EnquiryFeedbackReminderStatus = true;
            }
            enquiryLeadFeedbackModel.IsLastFeedback = true;

            enquiryLeadFeedbackModel.EnquiryLeadCallStatusKey = model.EnquiryLeadCallStatusKey;
            dbContext.EnquiryLeadFeedbacks.Add(enquiryLeadFeedbackModel);

            EnquiryLeadModel.EnquiryLeadStatusKey = model.EnquiryLeadStatusKey;
            EnquiryLeadModel.IsNewLead = 1;
            var EnquiryLead = dbContext.EnquiryLeads.Count(x => x.EmployeeKey == DbConstants.User.UserKey && x.IsNewLead == 0 && x.LeadDate != null);//&& x.IsSpam == false);

            Employee employee = dbContext.Employees.SingleOrDefault(row => row.AppUserKey == DbConstants.User.UserKey);
            if (EnquiryLeadModel != null)
            {
                //EmployeeFileHandover employeeFileHandover = dbContext.EmployeeFileHandovers.SingleOrDefault(row => row.FileKey == EnquiryLeadModel.RowKey && row.FileHandoverTypeKey == DbConstants.FileHandoverType.EnquiryLead);
                //if (employeeFileHandover != null)
                //{

                //    if (employee != null)
                //    {
                //        EnquiryLeadModel.EmployeeKey = employee.RowKey;
                //        EnquiryLeadModel.DepartmentKey = employee.DepartmentKey;
                //        EnquiryLeadModel.BranchKey = employee.BranchKey;
                //    }
                //    employeeFileHandover.IsActive = false;
                //}
            }
            model.Feedback = "";
            if (!DbConstants.Role.AdminUserTypes.Contains(DbConstants.User.RoleKey))//&& model.AcademicTermKey != DbConstants.AcademicTerm.HR)
            {
                CheckFeedBackAccountBlockCount(model);
            }

            EnquiryConfiguration Configuation = dbContext.EnquiryConfigurations.SingleOrDefault();

            if ((EnquiryLead - 1) <= Configuation.LeadCountLimit)
            {
                model.IsNewLeadNotification = true;
            }


            if (model.IsNewLeadNotification || model.IsClosedNotification == true)
            {

                //DbConstants.User.UserKeys = dbContext.AppUsers.Where(row => row.RowKey == DbConstants.AdminKey).Select(row => row.RowKey)
                //        .Union(dbContext.fnParentEmployees(DbConstants.User.UserKey).Where(row => row.AppUserKey != DbConstants.User.UserKey)
                //       .Select(row => row.AppUserKey ?? 0)).ToList();
            }

            if (model.ReferenceList != null)
            {
                CreateReference(model, EnquiryLeadModel);
            }

        }
        private void UpdateLeadFeedback(EnquiryLeadFeedbackViewModel model)
        {
            EnquiryLeadFeedback enquiryLeadFeedbackModel = new EnquiryLeadFeedback();

            enquiryLeadFeedbackModel = dbContext.EnquiryLeadFeedbacks.SingleOrDefault(row => row.RowKey == model.RowKey);
            enquiryLeadFeedbackModel.Feedback = model.Feedback;
            enquiryLeadFeedbackModel.CallDuration = model.CallDuration;
            enquiryLeadFeedbackModel.CallTypeKey = model.CallTypeKey;
            enquiryLeadFeedbackModel.Feedback = model.Feedback;

            if (DbConstants.EnquiryStatus.Closed == model.EnquiryLeadStatusKey)
            {
                enquiryLeadFeedbackModel.EnquiryFeedbackReminderStatus = false;
                enquiryLeadFeedbackModel.NextCallSchedule = null;
            }
            else
            {

                if (model.NextCallSchedule == null)
                {
                    enquiryLeadFeedbackModel.EnquiryFeedbackReminderStatus = false;
                    enquiryLeadFeedbackModel.NextCallSchedule = null;
                }
                else
                {
                    enquiryLeadFeedbackModel.NextCallSchedule = model.NextCallSchedule;
                    enquiryLeadFeedbackModel.EnquiryFeedbackReminderStatus = true;
                }
            }

            enquiryLeadFeedbackModel.EnquiryLeadCallStatusKey = model.EnquiryLeadCallStatusKey;

            EnquiryLead EnquiryLeadModel = dbContext.EnquiryLeads.SingleOrDefault(x => x.RowKey == enquiryLeadFeedbackModel.EnquiryLeadKey);
            EnquiryLeadModel.EnquiryLeadStatusKey = model.EnquiryLeadStatusKey;
            EnquiryLeadModel.IsNewLead = 1;
            model.Feedback = "";
            if (!DbConstants.Role.AdminUserTypes.Contains(DbConstants.User.RoleKey))
            {
                CheckFeedBackAccountBlockCount(model);
            }


        }
        //public EmployeeFileHandOverViewModel UpdateEmployeesFileHandover(List<EmployeeFileHandOverViewModel> modelList)
        //{
        //    EmployeeFileHandOverViewModel employeeFileHandoverViewModel = new EmployeeFileHandOverViewModel();


        //    long maxKey = dbContext.EmployeeFileHandovers.Select(p => p.RowKey).DefaultIfEmpty().Max();
        //    foreach (EmployeeFileHandOverViewModel model in modelList)
        //    {
        //        EmployeeFileHandover EmployeeFileHandoverModel = new EmployeeFileHandover();
        //        EmployeeFileHandoverModel.RowKey = Convert.ToInt64(maxKey + 1);
        //        EmployeeFileHandoverModel.FileHandoverTypeKey = model.FileHandoverTypeKey ?? 0;
        //        EmployeeFileHandoverModel.EmployeeFromKey = model.EmployeeFromKey;
        //        EmployeeFileHandoverModel.EmployeeToKey = model.EmployeeToKey;
        //        EmployeeFileHandoverModel.FileKey = model.FileKey;
        //        EmployeeFileHandoverModel.IsActive = true;
        //        dbContext.EmployeeFileHandovers.Add(EmployeeFileHandoverModel);
        //        maxKey++;
        //    }
        //    return employeeFileHandoverViewModel;
        //}
        public EnquiryLeadViewModel DeleteEnquiryLead(EnquiryLeadViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    EnquiryLead enquiryLead = dbContext.EnquiryLeads.SingleOrDefault(row => row.RowKey == model.RowKey);
                    List<EnquiryLeadFeedback> enquiryLeadFeedbackList = dbContext.EnquiryLeadFeedbacks.Where(row => row.EnquiryLeadKey == model.RowKey).ToList();

                    dbContext.EnquiryLeadFeedbacks.RemoveRange(enquiryLeadFeedbackList);
                    //enquiryLeadFeedbackList.ForEach(enquiryLeadFeedback => dbContext.EnquiryLeadFeedbacks.Remove(enquiryLeadFeedback));
                    dbContext.EnquiryLeads.Remove(enquiryLead);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.EnquiryLead, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.EnquiryLead);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.EnquiryLead, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.EnquiryLead);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EnquiryLead, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            //}
            return model;
        }
        public EnquiryLeadFeedbackViewModel DeleteEnquiryLeadFeeback(EnquiryLeadFeedbackViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    EnquiryLeadFeedback enquiryLeadFeedback = dbContext.EnquiryLeadFeedbacks.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.EnquiryLeadFeedbacks.Remove(enquiryLeadFeedback);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EnquiryLead, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.EnquiryLeadFeedback);
                        model.IsSuccessful = false;

                        ActivityLog.CreateActivityLog(MenuConstants.EnquiryLead, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.EnquiryLeadFeedback);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EnquiryLead, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            //}
            return model;
        }
        public EnquiryLeadViewModel GetSearchDropDownLists(EnquiryLeadViewModel model)
        {
            long[] LeadStatuses = { DbConstants.EnquiryStatus.FollowUp, DbConstants.EnquiryStatus.Closed };
            FillEnquiryLeadStatuses(model);
            FillBranches(model);
            GetEmployeesByBranchId(model);
            model.EnquiryStatuses.Insert(0, new SelectListModel { RowKey = 0, Text = EduSuiteUIResources.NewLead });
            model.EnquiryLeadCallStatuses = dbContext.EnquiryCallStatus.Where(x => x.IsActive == true && LeadStatuses.Contains(x.EnquiryStatusKey)).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.EnquiryCallStatusName
            }).ToList();
            FillNatureOfEnquiry(model);
            return model;
        }
        public EnquiryLeadViewModel CheckMobileNumberExists(string MobileNumber, short TelephoneCodeKey, long RowKey)
        {
            EnquiryLeadViewModel model = new EnquiryLeadViewModel();
            var result = dbContext.EnquiryLeads.Where(row => row.RowKey != RowKey).Select(row => new EnquiryViewModel { MobileNumber = row.MobileNumber })
               .Union(dbContext.Enquiries.Select(row => new EnquiryViewModel { MobileNumber = row.MobileNumber }))
               .Union(dbContext.Applications.Select(row => new EnquiryViewModel { MobileNumber = row.StudentMobile }))
               .Any(row => row.MobileNumber == MobileNumber);
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

        //public EnquiryLeadViewModel CheckMobileNumberExists2(string MobileNumber, short? TelephoneCodeKey, long RowKey, byte AcademicTermKey)
        //{
        //    List<byte> AcademicTermKeys = new List<byte>() { DbConstants.AcademicTerm.Study, DbConstants.AcademicTerm.Migration };
        //    EnquiryLeadViewModel model = new EnquiryLeadViewModel();
        //    var result = dbContext.EnquiryLeads.Where(row => row.RowKey != RowKey).Select(row => new EnquiryViewModel { AcademicTermKey = row.AcademicTermKey ?? 0, TelephoneCodeKey = row.TelephoneCodeKey, MobileNumber = row.MobileNumber, TelephoneCodeOptionalKey = row.TelePhoneCodeOptionalkey, MobileNumberOptional = row.MobileNumberOptional })
        //       .Union(dbContext.Enquiries.Select(row => new EnquiryViewModel { AcademicTermKey = row.AcademicTermKey, TelephoneCodeKey = row.TelephoneCodeKey, MobileNumber = row.MobileNumber, TelephoneCodeOptionalKey = row.TelePhoneCodeOptionalkey, MobileNumberOptional = row.MobileNumberOptional }))
        //       .Union(dbContext.Applications.Select(row => new EnquiryViewModel { AcademicTermKey = row.Course.AcademicTermKey, TelephoneCodeKey = row.TelephoneCodeKey, MobileNumber = row.MobileNumber, TelephoneCodeOptionalKey = row.TelePhoneCodeOptionalkey, MobileNumberOptional = row.MobileNumberOptional }))
        //       .Any(row => ((row.MobileNumber == MobileNumber) && (row.TelephoneCodeKey == TelephoneCodeKey || TelephoneCodeKey == 0) || (row.MobileNumberOptional == MobileNumber) && (row.TelephoneCodeOptionalKey == TelephoneCodeKey || TelephoneCodeKey == 0)) && ((AcademicTermKeys.Contains(AcademicTermKey) && AcademicTermKeys.Contains(row.AcademicTermKey)) || row.AcademicTermKey == AcademicTermKey || AcademicTermKey == 0));
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
        public EnquiryLeadViewModel CheckEmailAddressExists(string EmailAddress, long RowKey)
        {
            EnquiryLeadViewModel model = new EnquiryLeadViewModel();
            var result = dbContext.EnquiryLeads.Where(row => row.RowKey != RowKey).Select(row => new EnquiryViewModel { EmailAddress = row.EmailAddress })
            .Union(dbContext.Enquiries.Select(row => new EnquiryViewModel { EmailAddress = row.EmailAddress }))
            .Union(dbContext.Applications.Select(row => new EnquiryViewModel { EmailAddress = row.StudentEmail }))
            .Any(row => row.EmailAddress == EmailAddress);

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

        //public List<EnquiryLeadViewModel> CheckBulkMobileNumberExists(List<EnquiryLeadViewModel> modelList)
        //{
        //    EnquiryLeadViewModel model = new EnquiryLeadViewModel();
        //    var result = (from DL in modelList
        //                  join EL in dbContext.EnquiryLeads.Select(row => new EnquiryViewModel { MobileNumber = row.MobileNumber, IsEditable = true })
        //        .Union(dbContext.Enquiries.Select(row => new EnquiryViewModel { MobileNumber = row.MobileNumber, IsEditable = false }))
        //        .Union(dbContext.Applications.Select(row => new EnquiryViewModel { MobileNumber = row.StudentMobile, IsEditable = false }))
        //         on new { DL.MobileNumber, DL.TelephoneCodeKey } equals new { EL.MobileNumber, EL.TelephoneCodeKey }
        //                  //where ((EL.IsEditable && EL.RowKey != DL.RowKey) || !EL.IsEditable) && EL.MobileNumber == DL.MobileNumber && EL.TelephoneCodeKey == DL.TelephoneCodeKey
        //                  select new EnquiryLeadViewModel
        //                  {
        //                      RowKey = DL.PageIndex

        //                  }).ToList();


        //    // return null;
        //    return result;
        //}
        //public List<EnquiryLeadViewModel> CheckBulkEmailAddressExists(List<EnquiryLeadViewModel> modelList)
        //{

        //    //List<byte> AcademicTermKeys = new List<byte>() { DbConstants.AcademicTerm.Study, DbConstants.AcademicTerm.Migration };
        //    var result = (from DL in modelList
        //                  join EL in dbContext.EnquiryLeads.Select(row => new EnquiryViewModel { EmailAddress = row.EmailAddress, IsEditable = true })
        //    .Union(dbContext.Enquiries.Select(row => new EnquiryViewModel { EmailAddress = row.EmailAddress, IsEditable = false }))
        //    .Union(dbContext.Applications.Select(row => new EnquiryViewModel { EmailAddress = row.StudentEmail, IsEditable = false }))
        //    on new { DL.EmailAddress } equals new { EL.EmailAddress }
        //                  where (EL.IsEditable && EL.RowKey != DL.RowKey) && EL.EmailAddress == DL.EmailAddress
        //                  select new EnquiryLeadViewModel
        //                  {
        //                      RowKey = DL.PageIndex

        //                  }).ToList();


        //    //return null;
        //    return result;
        //}


        //public List<EnquiryLeadViewModel> CheckBulkMobileNumberExists(List<EnquiryLeadViewModel> modelList)
        //{
        //    EnquiryLeadViewModel model = new EnquiryLeadViewModel();
        //    var result = (from DL in modelList
        //                  join EL in dbContext.VwMobileNumbers
        //                  on new { TelephoneCodeKey = DL.TelephoneCodeKey ?? 0, DL.MobileNumber } equals new { TelephoneCodeKey = EL.TelephoneCodeKey ?? 0, EL.MobileNumber }
        //                  where ((EL.RowKey != DL.RowKey || EL.ContactType != DbConstants.FileHandoverType.EnquiryLead) && EL.MobileNumber == DL.MobileNumber && (EL.TelephoneCodeKey == DL.TelephoneCodeKey || EL.TelephoneCodeKey == 0 || DL.TelephoneCodeKey == 0))
        //                  select DL).ToList();
        //    return result;
        //}
        public List<EnquiryLeadViewModel> CheckBulkMobileNumberExists(List<EnquiryLeadViewModel> modelList)
        {
            EnquiryLeadViewModel model = new EnquiryLeadViewModel();
            var result = (from DL in modelList
                          join C in dbContext.Countries on DL.TelephoneCodeKey equals C.RowKey into bycountries
                          from D in bycountries.DefaultIfEmpty()
                          join EL in dbContext.VwMobileNumbersWithCountries on (D == null ? "" : D.TelephoneCode) + "" + DL.MobileNumber equals EL.CountryMobileNumber
                          where ((EL.RowKey != DL.RowKey || EL.ContactType != DbConstants.FileHandoverType.EnquiryLead) && EL.CountryMobileNumber == (D == null ? "" : D.TelephoneCode) + "" + DL.MobileNumber)
                          select DL).ToList();
            return result;
        }
        public List<EnquiryLeadViewModel> CheckBulkEmailAddressExists(List<EnquiryLeadViewModel> modelList)
        {


            var result = (from DL in modelList
                          join EL in dbContext.VwEmailAddesses
                          on new { DL.EmailAddress } equals new { EL.EmailAddress }
                          where ((EL.RowKey != DL.RowKey || EL.ContactType != DbConstants.FileHandoverType.EnquiryLead) && EL.EmailAddress == DL.EmailAddress)
                          select DL).ToList();

            return result;
        }
        public void GetCallStatusByEnquiryStatus(EnquiryLeadFeedbackViewModel model)
        {
            string ShowInList = DbConstants.Menu.EnquiryLead.ToString();
            model.EnquiryLeadCallStatuses = dbContext.EnquiryCallStatus.Where(x => x.IsActive == true && x.EnquiryStatusKey == model.EnquiryLeadStatusKey && x.ShowInMenuKeys.Contains(ShowInList)).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.EnquiryCallStatusName
            }).ToList();
        }
        public List<EnquiryLeadViewModel> GetPendingLeadByBranch(EnquiryLeadViewModel model, out int TotalRecords)
        {
            var Take = model.PageSize;
            var skip = (model.PageIndex - 1) * model.PageSize;
            IQueryable<EnquiryLeadViewModel> modelListQuery = dbContext.EnquiryLeads.Where(row => row.EmployeeKey == null && row.BranchKey == model.BranchKey).Select(row => new EnquiryLeadViewModel
            {
                RowKey = row.RowKey,
                BranchKey = row.BranchKey,
                EmployeeKey = row.EmployeeKey ?? 0,
                EmployeeCode = row.Employee.EmployeeCode,
                Name = row.Name,
                EmailAddress = row.EmailAddress,
                Qualification = row.Qualification,
                TelephoneCodeKey = row.TelephoneCodeKey ?? 0,
                MobileNumber = row.MobileNumber,
                Remarks = row.Remarks,
                LeadDate = null,
                LeadFrom = row.LeadFrom,
                District = row.District,
                Location = row.Location
            });
            TotalRecords = modelListQuery.Count();
            List<EnquiryLeadViewModel> modelList = modelListQuery.OrderBy(row => row.RowKey).Skip(skip).Take(Take).ToList();
            if (modelList.Count == 0)
            {
                modelList.Add(new EnquiryLeadViewModel());
            }
            FillTelephoneCodes(model);
            GetEmployeesByBranchId(model);

            return modelList;
        }
        public EnquiryLeadViewModel GetBranches(EnquiryLeadViewModel model)
        {

            FillBranches(model);
            return model;
        }
        public EnquiryLeadViewModel GetEmployeesWithCodeByBranchId(EnquiryLeadViewModel model)
        {
            model.Employees = dbContext.Employees.Where(row => row.BranchKey == model.BranchKey).Select(row => new GroupSelectListModel
            {
                RowKey = row.RowKey,
                Text = row.FirstName + " " + (row.MiddleName ?? "") + " " + row.LastName,
                GroupName = row.EmployeeCode
            }).OrderBy(row => row.Text).ToList();
            return model;
        }
        public EnquiryLeadFeedbackViewModel CheckCallStatusDuration(EnquiryLeadFeedbackViewModel model)
        {
            model.IsDuration = dbContext.EnquiryCallStatus.Where(row => row.IsActive == true && row.RowKey == model.EnquiryLeadCallStatusKey).Select(row => row.IsDuration).FirstOrDefault();
            return model;
        }
        public EnquiryLeadViewModel GetEmployeesByBranchId(EnquiryLeadViewModel model)
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

            if (model.BranchKey != null)
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
        private void FillFeedbackDrodownLists(EnquiryLeadFeedbackViewModel model)
        {

            FillEnquiryLeadStatuses(model);
            GetCallStatusByEnquiryStatus(model);
            CheckCallStatusDuration(model);
            FillCallTypes(model);

        }
        private void FillEnquiryLeadDrodownLists(EnquiryLeadViewModel model)
        {
            FillBranches(model);
            GetEmployeesByBranchId(model);
            FillCallTypes(model);
            FillEnquiryLeadStatuses(model);
            GetCallStatusByEnquiryStatus(model);
            model.IsDuration = dbContext.EnquiryCallStatus.Where(row => row.IsActive == true && row.RowKey == model.EnquiryLeadCallStatusKey).Select(row => row.IsDuration).FirstOrDefault();
            FillTelephoneCodes(model);
            FillNatureOfEnquiry(model);
            GetPhoneNumberLength(model);
        }
        private void FillBranches(EnquiryLeadViewModel model)
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
        private void FillEnquiryLeadStatuses(EnquiryLeadViewModel model)
        {
            long[] LeadStatuses = { DbConstants.EnquiryStatus.FollowUp, DbConstants.EnquiryStatus.Closed };
            model.EnquiryLeadStatuses = dbContext.EnquiryStatus.Where(x => LeadStatuses.Contains(x.RowKey)).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.EnquiryStatusName
            }).ToList();
        }
        private void FillEnquiryLeadStatuses(EnquiryLeadFeedbackViewModel model)
        {
            short[] LeadStatuses = { DbConstants.EnquiryStatus.FollowUp, DbConstants.EnquiryStatus.Closed };

            model.EnquiryLeadStatuses = dbContext.EnquiryStatus.Where(x => LeadStatuses.Contains(x.RowKey)).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.EnquiryStatusName
            }).ToList();

        }
        private void GetCallStatusByEnquiryStatus(EnquiryLeadViewModel model)
        {
            model.EnquiryLeadCallStatuses = dbContext.EnquiryCallStatus.Where(x => x.IsActive == true && x.EnquiryStatusKey == model.EnquiryLeadStatusKey).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.EnquiryCallStatusName
            }).ToList();
        }
        private void FillCallTypes(EnquiryLeadFeedbackViewModel model)
        {
            model.CallTypes = dbContext.CallTypes.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.CallTypeName
            }).ToList();

        }
        private void FillCallTypes(EnquiryLeadViewModel model)
        {
            model.CallTypes = dbContext.CallTypes.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.CallTypeName
            }).ToList();

        }
        private void FillTelephoneCodes(EnquiryLeadViewModel model)
        {
            model.TelephoneCodes = dbContext.VwCountrySelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.TelephoneCode
            }).ToList();
        }
        private void FillTelephoneCodes(ReferenceList model)
        {
            model.TelephoneCodes = dbContext.VwCountrySelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.TelephoneCode
            }).ToList();
        }
        private void FillAcademicTerms(ReferenceList model)
        {
            model.AcademicTerms = dbContext.VwAcadamicTermSelectActiveOnlies.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AcademicTermName
            }).ToList();
        }
        public List<EnquiryLeadViewModel> GetEnquiryLead(EnquiryLeadViewModel model)
        {
            try
            {
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();

                IQueryable<EnquiryLeadViewModel> EnquiryLeadListQuery = (from r in dbContext.EnquiryLeads
                                                                         where (r.Name.Contains(model.SearchText) || r.MobileNumber.Contains(model.SearchText))
                                                                         select new EnquiryLeadViewModel
                                                                         {
                                                                             RowKey = r.RowKey,
                                                                             Name = r.Name,
                                                                             MobileNumber = r.MobileNumber,
                                                                             EmailAddress = r.EmailAddress,
                                                                             BranchName = r.Branch.BranchName,
                                                                             EmployeeKey = r.EmployeeKey ?? 0,
                                                                             Qualification = r.Qualification,
                                                                             EmployeeName = r.Employee.FirstName,
                                                                             EnquiryLeadStatusName = r.EnquiryStatu.EnquiryStatusName,
                                                                             EnquiryLeadStatusKey = r.EnquiryLeadStatusKey,
                                                                             BranchKey = r.BranchKey,
                                                                             SearchEmployeeKey = r.EmployeeKey
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
                ActivityLog.CreateActivityLog(MenuConstants.EnquiryLead, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<EnquiryLeadViewModel>();

            }
        }
        private void CheckFeedBackAccountBlockCount(EnquiryLeadFeedbackViewModel model)
        {
            IQueryable<EnquiryScheduleViewModel> List = dbContext.EnquiryFeedbacks.Select(row => new EnquiryScheduleViewModel { UserKey = row.AddedBy, CreateOn = row.DateAdded, LastCallStatusKey = row.EnquiryCallStatusKey ?? 0 }).Union(dbContext.EnquiryLeadFeedbacks.Select(row => new EnquiryScheduleViewModel { UserKey = row.AddedBy, CreateOn = row.DateAdded, LastCallStatusKey = row.EnquiryLeadCallStatusKey ?? 0 }));
            EnquiryConfiguration Configuation = dbContext.EnquiryConfigurations.SingleOrDefault();
            List<int> CallStatuses = Configuation.CallStatuses.Split('|').Select(Int32.Parse).ToList();
            int Count = 0;
            string Message = "";

            if (CallStatuses.Contains(model.EnquiryLeadCallStatusKey ?? 0))
            {
                DateTime Today = DateTimeUTC.Now;
                EnquiryUserBlockList EnquiryUserBlockListModel = dbContext.EnquiryUserBlockLists.Where(x => System.Data.Entity.DbFunctions.TruncateTime(x.DateAdded) == System.Data.Entity.DbFunctions.TruncateTime(Today) && x.AddedBy == DbConstants.User.UserKey).OrderByDescending(x => x.DateAdded).Take(1).SingleOrDefault();
                string CallStatusName = dbContext.EnquiryCallStatus.Where(x => x.IsActive == true && x.RowKey == model.EnquiryLeadCallStatusKey).Select(x => x.EnquiryCallStatusName).SingleOrDefault();


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

        //public EnquiryLeadViewModel SpamData(EnquiryLeadViewModel model)
        //{
        //    EnquiryLead enquiryLeadModel = new EnquiryLead();
        //    using (var transaction = dbContext.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            enquiryLeadModel = dbContext.EnquiryLeads.SingleOrDefault(row => row.RowKey == model.RowKey);
        //            //enquiryLeadModel.IsSpam = true;
        //            dbContext.SaveChanges();
        //            transaction.Commit();
        //            model.Message = EduSuiteUIResources.Success;
        //            model.IsSuccessful = true;
        //            ActivityLog.CreateActivityLog(MenuConstants.EnquiryLead, ActionConstants.Spam, DbConstants.LogType.Info, model.RowKey, model.Message);
        //        }
        //        catch (Exception ex)
        //        {
        //            transaction.Rollback();
        //            model.Message = EduSuiteUIResources.FailedToSaveEnquiryLead;
        //            model.IsSuccessful = false;
        //            ActivityLog.CreateActivityLog(MenuConstants.EnquiryLead, ActionConstants.Spam, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
        //        }
        //    }
        //    return model;
        //}
        private void FillNatureOfEnquiry(EnquiryLeadViewModel model)
        {
            model.NatureOfEnquiries = dbContext.NatureOfEnquiries.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.NatureOfEnquiryName
            }).ToList();
        }
        public EnquiryLeadViewModel GetPhoneNumberLength(EnquiryLeadViewModel model)
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
