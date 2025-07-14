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
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class ApplicationElectivePaperService : IApplicationElectivePaperService
    {
        private EduSuiteDatabase dbContext;

        public ApplicationElectivePaperService(EduSuiteDatabase objDb)
        {
            this.dbContext = objDb;
        }
        public ApplicationElectivePaperViewModel GetApplicationElectivePaperById(Int64 ApplicationId)
        {
            try
            {
                ApplicationElectivePaperViewModel model = new ApplicationElectivePaperViewModel();
                model.ElectivePapers = dbContext.ElectivePapers.Where(x => x.ApplicationKey == ApplicationId).Select(row => new ElectivePaperViewModel
                {
                    RowKey = row.RowKey,
                    SubjectKey = row.SubjectKey,
                    SubjectCode = row.Subject.SubjectCode,
                    SubjectName = row.Subject.SubjectName,
                    IsActive = row.IsActive,
                    AcademicTermKey = row.Subject.CourseSubjectDetails.Select(x => x.CourseSubjectMaster.AcademicTermKey).FirstOrDefault(),
                    CourseDuration = row.Subject.CourseSubjectDetails.Select(x => x.CourseSubjectMaster.Course.CourseDuration).FirstOrDefault(),
                    SubjectYear = row.Subject.CourseSubjectDetails.Select(y => y.CourseSubjectMaster.CourseYear).FirstOrDefault(),

                }).ToList();

                model.ElectivePapers = model.ElectivePapers.OrderBy(x => x.SubjectYear).ToList();
                foreach (ElectivePaperViewModel PapperList in model.ElectivePapers)
                {
                    PapperList.SubjectYearText = CommonUtilities.GetYearDescriptionByCodeDetails(PapperList.CourseDuration ?? 0, PapperList.SubjectYear ?? 0, PapperList.AcademicTermKey ?? 0);

                }

                Application application = dbContext.Applications.SingleOrDefault(x => x.RowKey == ApplicationId);
                model.CourseKey = application.CourseKey;
                model.UniversityKey = application.UniversityMasterKey;
                model.AcademicTermKey = application.AcademicTermKey;


                if (model.ElectivePapers.Count == 0)
                {
                    model.ElectivePapers = dbContext.VwSubjectSelectActiveOnlies.Where(x => x.CourseKey == model.CourseKey && x.UniversityMasterKey == model.UniversityKey && x.AcademicTermKey == model.AcademicTermKey && x.IsElective == true).Select(row => new ElectivePaperViewModel
                    {
                        SubjectCode = row.SubjectCode,
                        SubjectKey = row.RowKey,
                        SubjectName = row.SubjectName,
                        AcademicTermKey = row.AcademicTermKey,
                        CourseDuration = row.CourseDuration,
                        SubjectYear=row.CourseYear

                    }).ToList();

                    model.ElectivePapers = model.ElectivePapers.OrderBy(x => x.SubjectYear).ToList();
                    foreach (ElectivePaperViewModel PapperList in model.ElectivePapers)
                    {
                        PapperList.SubjectYearText = CommonUtilities.GetYearDescriptionByCodeDetails(PapperList.CourseDuration ?? 0, PapperList.SubjectYear ?? 0, PapperList.AcademicTermKey ?? 0);

                    }
                }

                if (model == null)
                {
                    model = new ApplicationElectivePaperViewModel();
                }
                model.ApplicationKey = ApplicationId;

                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.ApplicationElectivePaper, ActionConstants.View, DbConstants.LogType.Debug, ApplicationId, ex.GetBaseException().Message);
                return new ApplicationElectivePaperViewModel();

               

            }
        }

        public ApplicationElectivePaperViewModel UpdateApplicationElectivePaper(ApplicationElectivePaperViewModel model)
        {
            // FillDropdownList(model);
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    CreateElectivePaper(model.ElectivePapers.Where(row => row.RowKey == 0).ToList(), model.ApplicationKey);
                    UpdateElectivePaper(model.ElectivePapers.Where(row => row.RowKey != 0).ToList());
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    //model.AdmissionNo = dbContext.T_Application.Where(row => row.RowKey == model.ApplicationKey).Select(row => row.AdmissionNo).FirstOrDefault();
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationElectivePaper, (model.ElectivePapers.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Info, model.ApplicationKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Elective);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationElectivePaper, (model.ElectivePapers.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, model.ApplicationKey, ex.GetBaseException().Message);
                }

            }
            return model;
        }

        private void CreateElectivePaper(List<ElectivePaperViewModel> modelList, Int64 ApplicationKey)
        {

            Int64 maxKey = dbContext.ElectivePapers.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (ElectivePaperViewModel model in modelList)
            {

                ElectivePaper applicationElectivePaperModel = new ElectivePaper();
                applicationElectivePaperModel.RowKey = Convert.ToInt64(maxKey + 1);
                applicationElectivePaperModel.ApplicationKey = ApplicationKey;
                applicationElectivePaperModel.SubjectKey = model.SubjectKey;
                applicationElectivePaperModel.IsActive = model.IsActive;

                dbContext.ElectivePapers.Add(applicationElectivePaperModel);

                maxKey++;

            }

        }

        public void UpdateElectivePaper(List<ElectivePaperViewModel> modelList)
        {

            foreach (ElectivePaperViewModel model in modelList)
            {

                ElectivePaper applicationElectivePaperModel = new ElectivePaper();

                applicationElectivePaperModel = dbContext.ElectivePapers.SingleOrDefault(row => row.RowKey == model.RowKey);
                applicationElectivePaperModel.SubjectKey = model.SubjectKey;
                applicationElectivePaperModel.IsActive = model.IsActive;


                //dbContext.T_EducationQualification.Add(applicationEducationQualificationModel);

            }
        }

        public ApplicationElectivePaperViewModel DeleteApplicationElectivePaper(ElectivePaperViewModel model)
        {
            ApplicationElectivePaperViewModel applicationDocumentModel = new ApplicationElectivePaperViewModel();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    ElectivePaper applicationElectivePaper = dbContext.ElectivePapers.SingleOrDefault(row => row.RowKey == model.RowKey);

                    dbContext.ElectivePapers.Remove(applicationElectivePaper);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    applicationDocumentModel.Message = EduSuiteUIResources.Success;
                    applicationDocumentModel.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationElectivePaper, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, applicationDocumentModel.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        applicationDocumentModel.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Elective);
                        applicationDocumentModel.IsSuccessful = false;

                        ActivityLog.CreateActivityLog(MenuConstants.ApplicationElectivePaper, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);

                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    applicationDocumentModel.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Elective);
                    applicationDocumentModel.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationElectivePaper, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return applicationDocumentModel;
        }



    }
}
