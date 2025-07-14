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
    public class ApplicationDocumentService : IApplicationDocumentService
    {
        private EduSuiteDatabase dbContext;

        public ApplicationDocumentService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public ApplicationDocumentViewModel GetApplicationDocumentsById(Int64 ApplicationId)
        {
            try
            {
                ApplicationDocumentViewModel model = new ApplicationDocumentViewModel();
                model.ApplicationDocuments = dbContext.StudentDocuments.Where(x => x.ApplicationKey == ApplicationId).Select(row => new DocumentViewModel
                {
                    RowKey = row.RowKey,
                    StudentDocumentName = row.StudentDocumentName,
                    StudentDocumentPath = row.OldDocumentPath != null ? row.OldDocumentPath : UrlConstants.ApplicationUrl + row.Application.RowKey + "/Document/" + row.StudentDocumentPath,
                    StudentDocumentPathText = row.StudentDocumentPath
                }).ToList();
                if (model.ApplicationDocuments.Count == 0)
                {
                    model.ApplicationDocuments.Add(new DocumentViewModel());
                }
               
                if (model == null)
                {
                    model = new ApplicationDocumentViewModel();
                }
                model.ApplicationKey = ApplicationId;
                //FillDropdownList(model);
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.ApplicationDocument, ActionConstants.View, DbConstants.LogType.Error, ApplicationId != null ? ApplicationId : 0, ex.GetBaseException().Message);
                return new ApplicationDocumentViewModel();
            }
        }

        public ApplicationDocumentViewModel UpdateApplicationDocument(ApplicationDocumentViewModel model)
        {
            // FillDropdownList(model);
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    CreateDocument(model.ApplicationDocuments.Where(row => row.RowKey == 0).ToList(), model.ApplicationKey);
                    UpdateDocument(model.ApplicationDocuments.Where(row => row.RowKey != 0).ToList());
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.AdmissionNo = dbContext.Applications.Where(row => row.RowKey == model.ApplicationKey).Select(row => row.AdmissionNo).FirstOrDefault();
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationDocument, (model.ApplicationDocuments.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Info, model.ApplicationKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Document);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationDocument, (model.ApplicationDocuments.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, model.ApplicationKey, ex.GetBaseException().Message);

                }

            }
            return model;
        }

        private void CreateDocument(List<DocumentViewModel> modelList, Int64 ApplicationKey)
        {

            Int64 maxKey = dbContext.StudentDocuments.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (DocumentViewModel model in modelList)
            {

                StudentDocument applicationDocumentModel = new StudentDocument();
                applicationDocumentModel.RowKey = Convert.ToInt64(maxKey + 1);
                applicationDocumentModel.ApplicationKey = ApplicationKey;
                applicationDocumentModel.StudentDocumentName = model.StudentDocumentName;
                if (model.DocumentFile != null)
                {
                    applicationDocumentModel.StudentDocumentPath = applicationDocumentModel.RowKey + model.StudentDocumentPath;
                }
                dbContext.StudentDocuments.Add(applicationDocumentModel);
                model.StudentDocumentPath = applicationDocumentModel.StudentDocumentPath;

                maxKey++;
            }
        }

        public void UpdateDocument(List<DocumentViewModel> modelList)
        {
            foreach (DocumentViewModel model in modelList)
            {
                StudentDocument applicationDocumentModel = new StudentDocument();
                applicationDocumentModel = dbContext.StudentDocuments.SingleOrDefault(row => row.RowKey == model.RowKey);
                applicationDocumentModel.StudentDocumentName = model.StudentDocumentName;
                if (model.DocumentFile != null)
                {
                    applicationDocumentModel.StudentDocumentPath = applicationDocumentModel.RowKey + model.StudentDocumentPath;
                }
                model.StudentDocumentPath = applicationDocumentModel.StudentDocumentPath;
            }
        }

        public ApplicationDocumentViewModel DeleteApplicationDocument(DocumentViewModel model)
        {
            ApplicationDocumentViewModel applicationDocumentModel = new ApplicationDocumentViewModel();
           
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    StudentDocument applicationDocument = dbContext.StudentDocuments.SingleOrDefault(row => row.RowKey == model.RowKey);
                    applicationDocumentModel.ApplicationDocuments.Add(new DocumentViewModel { StudentDocumentName = applicationDocument.StudentDocumentName });
                    if (applicationDocument != null)
                    {
                        applicationDocumentModel.ApplicationKey = applicationDocument.ApplicationKey ?? 0;
                    }
                    dbContext.StudentDocuments.Remove(applicationDocument);
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
                        applicationDocumentModel.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Documents);
                        applicationDocumentModel.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.ApplicationDocument, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    applicationDocumentModel.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Documents);
                    applicationDocumentModel.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationDocument, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return applicationDocumentModel;
        }
    }
}
