using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Data;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Common;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class UniversityCertificateService : IUniversityCertificateService
    {
        private EduSuiteDatabase dbContext;
        public UniversityCertificateService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public UniversityCertificateViewModel GetUniversityCertificateById(long ApplicationKey)
        {
            try
            {
                UniversityCertificateViewModel model = new UniversityCertificateViewModel();

                model.UniversityCertificateDetails = dbContext.UniversityCertificates.Where(x => x.ApplicationKey == ApplicationKey).Select(row => new UniversityCertificateDetails
                {
                    RowKey = row.RowKey,
                    ApplicationKey = row.ApplicationKey,
                    CertificateTypeKey = row.CertificateTypeKey,
                    UniversityCertificateDescription = row.UniversityCertificateDescription,
                    IsReceived = row.IsReceived,
                    ReceivedBy = row.ReceivedBy,
                    //ReceivedDate = row.ReceivedDate,
                    IsIssued = row.IsIssued,
                    IssuedBy = row.IssuedBy,
                    // IssuedDate = row.IssuedDate,
                    IsActive = row.IsActive,
                    CertificatePath =  UrlConstants.ApplicationUrl + row.Application.RowKey + "/AffiliatedCertificates/" + row.DocumentPath,
                    CertificatePathText = row.DocumentPath
                }).ToList();

                if (model.UniversityCertificateDetails.Count == 0)
                {
                    model.UniversityCertificateDetails.Add(new UniversityCertificateDetails());
                }
                if (model == null)
                {
                    model = new UniversityCertificateViewModel();
                }
                model.ApplicationKey = ApplicationKey;
                FillCertificateType(model);
                return model;

            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.UniversityCertificate, ActionConstants.View, DbConstants.LogType.Error, ApplicationKey, ex.GetBaseException().Message);
                return new UniversityCertificateViewModel();


            }
        }

        private void FillCertificateType(UniversityCertificateViewModel model)
        {
            if (ApplicationSettingModel.CertificatesEnabled)
            {
                model.CertificateTypeList = dbContext.ApplicationCertificates.Where(x => x.IsActive && x.ApplicationKey == model.ApplicationKey).Select(x => new SelectListModel
                {
                    RowKey = x.CertificateTypeKey,
                    Text = x.CertificateType.CertificateTypeName,
                }).ToList();
            }
            else
            {
                model.CertificateTypeList = dbContext.CertificateTypes.Where(x => x.IsActive).Select(x => new SelectListModel
                {
                    RowKey = x.RowKey,
                    Text = x.CertificateTypeName,
                }).ToList();
            }

        }

        public UniversityCertificateViewModel UpdateUniversityCertificate(UniversityCertificateViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    UpdateUniversityCertificateDetails(model.UniversityCertificateDetails.Where(row => row.RowKey != 0).ToList(), model);
                    CreateUniversityCertificateDetails(model.UniversityCertificateDetails.Where(row => row.RowKey == 0).ToList(), model);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.UniversityCertificate, (model.UniversityCertificateDetails.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Info, model.ApplicationKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.AffiliationsTieUps + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Certificate);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.UniversityCertificate, (model.UniversityCertificateDetails.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, model.ApplicationKey, ex.GetBaseException().Message);

                }
                return model;
            }
        }

        private void CreateUniversityCertificateDetails(List<UniversityCertificateDetails> ModelList, UniversityCertificateViewModel objViewModel)
        {
            Int64 MaxKey = dbContext.UniversityCertificates.Select(p => p.RowKey).DefaultIfEmpty().Max();

            foreach (UniversityCertificateDetails model in ModelList)
            {
                UniversityCertificate universityCertificateModel = new UniversityCertificate();

                universityCertificateModel.RowKey = Convert.ToInt64(MaxKey + 1);
                universityCertificateModel.ApplicationKey = objViewModel.ApplicationKey;
                universityCertificateModel.CertificateTypeKey = model.CertificateTypeKey;
                universityCertificateModel.UniversityCertificateDescription = model.UniversityCertificateDescription;
                universityCertificateModel.IsReceived = model.IsReceived;
                if (model.IsReceived == true)
                {
                    universityCertificateModel.ReceivedDate = objViewModel.CommonDate;
                    universityCertificateModel.ReceivedBy = DbConstants.User.UserKey;
                }
                universityCertificateModel.IsIssued = model.IsIssued;
                if (model.IsIssued == true)
                {
                    universityCertificateModel.IssuedDate = objViewModel.CommonDate;
                    universityCertificateModel.IssuedBy = DbConstants.User.UserKey;
                }
                if (model.DocumentFile != null)
                {
                    universityCertificateModel.DocumentPath = universityCertificateModel.RowKey + model.CertificatePath;
                }
                dbContext.UniversityCertificates.Add(universityCertificateModel);
                model.CertificatePath = universityCertificateModel.DocumentPath;
                model.RowKey = universityCertificateModel.RowKey;
                MaxKey++;
            }
        }

        private void UpdateUniversityCertificateDetails(List<UniversityCertificateDetails> ModelList, UniversityCertificateViewModel objViewModel)
        {
            foreach (UniversityCertificateDetails model in ModelList)
            {
                UniversityCertificate universityCertificateModel = new UniversityCertificate();

                universityCertificateModel = dbContext.UniversityCertificates.SingleOrDefault(x => x.RowKey == model.RowKey);
                universityCertificateModel.ApplicationKey = objViewModel.ApplicationKey;
                universityCertificateModel.CertificateTypeKey = model.CertificateTypeKey;
                universityCertificateModel.UniversityCertificateDescription = model.UniversityCertificateDescription;

                if (universityCertificateModel.IsReceived != true && model.IsReceived == true)
                {
                    universityCertificateModel.IsReceived = model.IsReceived;
                    universityCertificateModel.ReceivedDate = objViewModel.CommonDate;
                    universityCertificateModel.ReceivedBy = DbConstants.User.UserKey;
                }

                if (universityCertificateModel.IsIssued != true && model.IsIssued == true)
                {
                    universityCertificateModel.IsIssued = model.IsIssued;
                    universityCertificateModel.IssuedDate = objViewModel.CommonDate;
                    universityCertificateModel.IssuedBy = DbConstants.User.UserKey;
                }
                if (model.DocumentFile != null)
                {
                    universityCertificateModel.DocumentPath = universityCertificateModel.RowKey + model.CertificatePath;
                }
                model.CertificatePath = universityCertificateModel.DocumentPath;
            }
        }

        public UniversityCertificateViewModel DeleteUniversityCertificate(long RowKey)
        {
            UniversityCertificateViewModel model = new UniversityCertificateViewModel();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    UniversityCertificate UniversityCertificateList = dbContext.UniversityCertificates.Where(row => row.RowKey == RowKey).FirstOrDefault();
                    model.UniversityCertificateDetails.Add(new UniversityCertificateDetails { CertificatePath = UniversityCertificateList.DocumentPath });
                    if (UniversityCertificateList != null)
                    {
                        model.ApplicationKey = UniversityCertificateList.ApplicationKey;
                    }
                    dbContext.UniversityCertificates.Remove(UniversityCertificateList);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.UniversityCertificate, ActionConstants.Delete, DbConstants.LogType.Info, RowKey, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.AffiliationsTieUps + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Certificate);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.UniversityCertificate, ActionConstants.Delete, DbConstants.LogType.Error, RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.AffiliationsTieUps + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Certificate);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.UniversityCertificate, ActionConstants.Delete, DbConstants.LogType.Error, RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public List<ApplicationViewModel> GetApplications(ApplicationViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;

                IQueryable<ApplicationViewModel> applicationList = (from a in dbContext.Applications
                                                                    where (a.StudentName.Contains(model.ApplicantName)) && (a.StudentMobile.Contains(model.MobileNumber))
                                                                    select new ApplicationViewModel
                                                                    {
                                                                        RowKey = a.RowKey,
                                                                        AdmissionNo = a.AdmissionNo,
                                                                        ApplicantName = a.StudentName,

                                                                        CourseName = a.Course.CourseName,
                                                                        UniversityName = a.UniversityMaster.UniversityMasterName,
                                                                        MobileNumber = a.StudentMobile,
                                                                        ApplicationStatusName = a.StudentStatu.StudentStatusName,
                                                                        BatchName = a.Batch.BatchName,
                                                                        BatchKey = a.BatchKey,
                                                                        BranchKey = a.BranchKey,
                                                                        CourseKey = a.CourseKey,
                                                                        UniversityKey = a.UniversityMasterKey,
                                                                        BranchName = a.Branch.BranchName,
                                                                        AcademicTermKey = a.AcademicTermKey,
                                                                        CurrentYear = a.CurrentYear,
                                                                        CourseDuration = a.Course.CourseDuration ?? 0,
                                                                        NoOfCertificate = a.UniversityCertificates.Count(),
                                                                        NoOfRecieved = a.UniversityCertificates.Count(x => x.IsReceived),
                                                                        NoOfVerified = a.UniversityCertificates.Count(x => x.IsIssued),
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
                applicationList = applicationList.GroupBy(x => x.RowKey).Select(y => y.FirstOrDefault());
                if (model.SortBy != "")
                {
                    applicationList = SortApplications(applicationList, model.SortBy, model.SortOrder);
                }
                TotalRecords = applicationList.Count();
                return applicationList.Skip(Skip).Take(Take).ToList<ApplicationViewModel>();
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.UniversityCertificate, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<ApplicationViewModel>();
            }
        }

        private IQueryable<ApplicationViewModel> SortApplications(IQueryable<ApplicationViewModel> Query, string SortName, string SortOrder)
        {
            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(ApplicationViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<ApplicationViewModel>(resultExpression);
        }


        public UniversityCertificateViewModel UpdateUniversityCertificates(UniversityCertificateViewModel MasterModel)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    MasterModel.UniversityCertificateDetails = MasterModel.UniversityCertificateDetails.Except(CheckCertificateAvailableExists(MasterModel.UniversityCertificateDetails)).ToList();
                    Int64 maxKey = dbContext.UniversityCertificates.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    dbContext.Configuration.AutoDetectChangesEnabled = false;
                    int Count = 0;
                    foreach (UniversityCertificateDetails model in MasterModel.UniversityCertificateDetails)
                    {
                        ++Count;

                        dbContext.AddToContext(new UniversityCertificate
                        {
                            RowKey = ++maxKey,
                            ApplicationKey = model.ApplicationKey,
                            CertificateTypeKey = model.CertificateTypeKey,
                            IsReceived = model.IsReceived,
                            ReceivedBy = DbConstants.User.UserKey,
                            ReceivedDate = model.ReceivedDate

                        }, Count);



                    }
                    dbContext.SaveChanges();
                    dbContext.Configuration.AutoDetectChangesEnabled = true;
                    transaction.Commit();

                    MasterModel.Message = EduSuiteUIResources.Success;
                    MasterModel.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.UniversityCertificate, ActionConstants.Edit, DbConstants.LogType.Info, DbConstants.User.UserKey, MasterModel.Message);

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
                    ActivityLog.CreateActivityLog(MenuConstants.UniversityCertificate, ActionConstants.Edit, DbConstants.LogType.Error, DbConstants.User.UserKey, dbEx.GetBaseException().Message);

                    throw raise;

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MasterModel.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Certificate);
                    MasterModel.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.UniversityCertificate, ActionConstants.Edit, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);

                }
            }
            return MasterModel;
        }
        public List<UniversityCertificateDetails> CheckCertificateAvailableExists(List<UniversityCertificateDetails> modelList)
        {
            EnquiryLeadViewModel model = new EnquiryLeadViewModel();
            var result = (from DL in modelList
                          join EL in dbContext.UniversityCertificates
                          on new { DL.ApplicationKey, DL.CertificateTypeKey } equals new { EL.ApplicationKey, EL.CertificateTypeKey }
                          select DL).ToList();
            return result;
        }
    }
}
