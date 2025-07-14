using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;

namespace CITS.EduSuite.Business.Services
{
    public class ApplicationEducationalDetailsService : IApplicationEducationalDetailsService
    {
        private EduSuiteDatabase dbContext;

        public ApplicationEducationalDetailsService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public ApplicationEducationDetailsViewModel GetApplicationDocumentsById(Int64 ApplicationId)
        {
            try
            {
                ApplicationEducationDetailsViewModel model = new ApplicationEducationDetailsViewModel();
                model.ApplicationEducationalDetails = dbContext.EducationQualifications.Where(x => x.ApplicationKey == ApplicationId).Select(row => new EducationDetailViewModel
                {
                    RowKey = row.RowKey,
                    EducationQualificationCourse = row.EducationQualificationCourse,
                    EducationQualificationUniversity = row.EducationQualificationUniversity,
                    EducationQualificationYear = row.EducationQualificationYear,
                    EducationQualificationPercentage = row.EducationQualificationPercentage,
                    OriginalIssuedDate = row.OriginalIssuedDate,
                    IsOriginalIssued = row.IsOriginalIssued,
                    IsCopyIssued = row.IsCopyIssued,
                    EducationQualificationCertificatePath = row.OldDocumentPath != null ? row.OldDocumentPath : UrlConstants.ApplicationUrl + row.Application.RowKey + "/EducationQualification/" + row.EducationQualificationCertificatePath,
                    EducationQualificationCertificatePathText = row.EducationQualificationCertificatePath

                }).ToList();
                if (model.ApplicationEducationalDetails.Count == 0)
                {
                    model.ApplicationEducationalDetails.Add(new EducationDetailViewModel());
                }
                if (model == null)
                {
                    model = new ApplicationEducationDetailsViewModel();
                }
                model.ApplicationKey = ApplicationId;
                //FillDropdownList(model);
                return model;
            }
            catch (Exception ex)
            {
                return new ApplicationEducationDetailsViewModel();
            }
        }

        public ApplicationEducationDetailsViewModel UpdateApplicationDocument(ApplicationEducationDetailsViewModel model)
        {
            // FillDropdownList(model);
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    CreateCertificateProcessLog(model.ApplicationEducationalDetails.Where(row => row.RowKey == 0).ToList(), model.ApplicationKey);
                    UpdateCertificateProcessLog(model.ApplicationEducationalDetails.Where(row => row.RowKey != 0).ToList());
                    UpdateDocument(model.ApplicationEducationalDetails.Where(row => row.RowKey != 0).ToList());
                    CreateDocument(model.ApplicationEducationalDetails.Where(row => row.RowKey == 0).ToList(), model.ApplicationKey);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.AdmissionNo = dbContext.Applications.Where(row => row.RowKey == model.ApplicationKey).Select(row => row.AdmissionNo).FirstOrDefault();
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationEducational, (model.ApplicationEducationalDetails.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Info, model.ApplicationKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.EducationQualification);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationEducational, (model.ApplicationEducationalDetails.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, model.ApplicationKey, ex.GetBaseException().Message);
                }

            }
            return model;
        }

        private void CreateDocument(List<EducationDetailViewModel> modelList, Int64 ApplicationKey)
        {

            Int64 maxKey = dbContext.EducationQualifications.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (EducationDetailViewModel model in modelList)
            {

                EducationQualification applicationEducationQualificationModel = new EducationQualification();
                applicationEducationQualificationModel.RowKey = ++maxKey;
                applicationEducationQualificationModel.ApplicationKey = ApplicationKey;
                applicationEducationQualificationModel.EducationQualificationCourse = model.EducationQualificationCourse;
                applicationEducationQualificationModel.EducationQualificationUniversity = model.EducationQualificationUniversity;
                applicationEducationQualificationModel.EducationQualificationYear = model.EducationQualificationYear;
                applicationEducationQualificationModel.EducationQualificationResult = model.EducationQualificationResult;
                applicationEducationQualificationModel.EducationQualificationPercentage = model.EducationQualificationPercentage;
                //applicationEducationQualificationModel.EducationQualificationCertificatePath = model.EducationQualificationCertificatePath;
                applicationEducationQualificationModel.CertificateStatusKey = DbConstants.CertificateProcessType.Received;
                applicationEducationQualificationModel.IsOriginalIssued = model.IsOriginalIssued;
                applicationEducationQualificationModel.IsCopyIssued = model.IsCopyIssued;
                if (model.IsOriginalIssued)
                    applicationEducationQualificationModel.OriginalIssuedDate = model.OriginalIssuedDate;
                if (model.IsCopyIssued)
                    applicationEducationQualificationModel.CopyIssuedDate = model.OriginalIssuedDate;

                applicationEducationQualificationModel.OriginalIssuedBy = CommonUtilities.GetLoggedUser().UserKey;

                if (model.DocumentFile != null)
                {
                    applicationEducationQualificationModel.EducationQualificationCertificatePath = applicationEducationQualificationModel.RowKey + model.EducationQualificationCertificatePath;
                }
                dbContext.EducationQualifications.Add(applicationEducationQualificationModel);
                model.EducationQualificationCertificatePath = applicationEducationQualificationModel.EducationQualificationCertificatePath;
                model.RowKey = applicationEducationQualificationModel.RowKey;

            }

        }

        public void UpdateDocument(List<EducationDetailViewModel> modelList)
        {

            foreach (EducationDetailViewModel model in modelList)
            {

                EducationQualification applicationEducationQualificationModel = new EducationQualification();

                applicationEducationQualificationModel = dbContext.EducationQualifications.SingleOrDefault(row => row.RowKey == model.RowKey);
                applicationEducationQualificationModel.EducationQualificationCourse = model.EducationQualificationCourse;
                applicationEducationQualificationModel.EducationQualificationUniversity = model.EducationQualificationUniversity;
                applicationEducationQualificationModel.EducationQualificationYear = model.EducationQualificationYear;
                applicationEducationQualificationModel.EducationQualificationResult = model.EducationQualificationResult;
                applicationEducationQualificationModel.EducationQualificationPercentage = model.EducationQualificationPercentage;
                // applicationEducationQualificationModel.EducationQualificationCertificatePath = model.EducationQualificationCertificatePath;
                applicationEducationQualificationModel.IsOriginalIssued = model.IsOriginalIssued;
                applicationEducationQualificationModel.CertificateStatusKey = DbConstants.CertificateProcessType.Received;
                applicationEducationQualificationModel.IsCopyIssued = model.IsCopyIssued;
                if (model.IsOriginalIssued)
                    applicationEducationQualificationModel.OriginalIssuedDate = model.OriginalIssuedDate;
                if (model.IsCopyIssued)
                    applicationEducationQualificationModel.CopyIssuedDate = model.OriginalIssuedDate;

                if (model.DocumentFile != null)
                {
                    applicationEducationQualificationModel.EducationQualificationCertificatePath = applicationEducationQualificationModel.RowKey + model.EducationQualificationCertificatePath;
                }

                model.EducationQualificationCertificatePath = applicationEducationQualificationModel.EducationQualificationCertificatePath;

                //dbContext.T_EducationQualification.Add(applicationEducationQualificationModel);

            }
        }

        private void CreateCertificateProcessLog(List<EducationDetailViewModel> modelList, Int64 ApplicationKey)
        {
            Int64 MaxKey = dbContext.StudentsCertificateReturns.Select(p => p.RowKey).DefaultIfEmpty().Max();
            Int64 EducationQualificationMaxKey = dbContext.EducationQualifications.Select(p => p.RowKey).DefaultIfEmpty().Max();


            foreach (EducationDetailViewModel model in modelList)
            {

                StudentsCertificateReturn studentsCertificateReturnModel = new StudentsCertificateReturn();
                studentsCertificateReturnModel.RowKey = Convert.ToInt64(++MaxKey);
                studentsCertificateReturnModel.ApplicationKey = ApplicationKey;
                studentsCertificateReturnModel.EducationQualificationKey = Convert.ToInt64(++EducationQualificationMaxKey);
                studentsCertificateReturnModel.CertificateStatusKey = DbConstants.CertificateProcessType.Received;
                studentsCertificateReturnModel.IssuedDate = model.OriginalIssuedDate;
                studentsCertificateReturnModel.IssuedBy = CommonUtilities.GetLoggedUser().UserKey;
                studentsCertificateReturnModel.IsTempReturn = false;


                dbContext.StudentsCertificateReturns.Add(studentsCertificateReturnModel);
                //MaxKey++;
                //EducationQualificationMaxKey++;

            }
        }

        private void UpdateCertificateProcessLog(List<EducationDetailViewModel> modelList)
        {

            foreach (EducationDetailViewModel model in modelList)
            {
                StudentsCertificateReturn studentsCertificateReturnModel = dbContext.StudentsCertificateReturns.FirstOrDefault(row => row.EducationQualificationKey == model.RowKey);
                studentsCertificateReturnModel.IssuedDate = model.OriginalIssuedDate;
            }
        }

        public ApplicationEducationDetailsViewModel DeleteApplicationDocument(EducationDetailViewModel model)
        {
            ApplicationEducationDetailsViewModel applicationDocumentModel = new ApplicationEducationDetailsViewModel();
            // FillDropdownList(applicationDocumentModel);
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    EducationQualification educationQualification = dbContext.EducationQualifications.SingleOrDefault(row => row.RowKey == model.RowKey);
                    List<StudentsCertificateReturn> studentsCertificateReturnList = dbContext.StudentsCertificateReturns.Where(row => row.EducationQualificationKey == model.RowKey).ToList();

                    applicationDocumentModel.ApplicationEducationalDetails.Add(new EducationDetailViewModel { EducationQualificationCertificatePath = educationQualification.EducationQualificationCertificatePath });

                    if (educationQualification != null)
                    {
                        applicationDocumentModel.ApplicationKey = educationQualification.ApplicationKey;
                    }
                    dbContext.StudentsCertificateReturns.RemoveRange(studentsCertificateReturnList);
                    dbContext.EducationQualifications.Remove(educationQualification);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    applicationDocumentModel.Message = EduSuiteUIResources.Success;
                    applicationDocumentModel.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationDocument, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, applicationDocumentModel.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        applicationDocumentModel.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.EducationQualification);
                        applicationDocumentModel.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.ApplicationDocument, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    applicationDocumentModel.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.EducationQualification);
                    applicationDocumentModel.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationDocument, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return applicationDocumentModel;
        }

        //public ApplicationDocumentViewModel CheckDocumentTypeExists(Int16 DocumentTypeKey, Int64 ApplicationKey, Int64 RowKey)
        //{
        //    ApplicationDocumentViewModel model = new ApplicationDocumentViewModel();
        //    if (dbContext.ApplicationEducationalDetails.Where(row => row.DocumentTypeKey == DocumentTypeKey && row.ApplicationKey == ApplicationKey && row.RowKey != RowKey).Any())
        //    {
        //        model.IsSuccessful = false;

        //    }
        //    else
        //    {
        //        model.IsSuccessful = true;
        //    }
        //    return model;
        //}

        //private void FillDropdownList(ApplicationDocumentViewModel model)
        //{
        //    FillDocumentTypes(model);
        //}
        //private void FillDocumentTypes(ApplicationDocumentViewModel model)
        //{
        //    model.DocumentTypes = dbContext.DocumentTypes.Where(row => row.IsActive).OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
        //    {
        //        RowKey = row.RowKey,
        //        Text = row.DocumentTypeName
        //    }).ToList();
        //}



    }
}
