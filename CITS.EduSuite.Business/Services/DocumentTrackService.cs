using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Data;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Interfaces;
using System.Linq.Expressions;

namespace CITS.EduSuite.Business.Services
{
    public class DocumentTrackService : IDocumentTrackService
    {

        private EduSuiteDatabase dbContext;

        public DocumentTrackService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public DocumentTrackViewModel CreateDocumentTrack(DocumentTrackViewModel model)
        {
            DocumentTrack documentTrackModel = new DocumentTrack();
            using (var Transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    long MaxKey = dbContext.DocumentTracks.Select(x => x.RowKey).DefaultIfEmpty().Max();
                    documentTrackModel.RowKey = MaxKey + 1;
                    documentTrackModel.AppUserKey = DbConstants.User.UserKey;
                    documentTrackModel.Date = DateTimeUTC.Now;
                    documentTrackModel.RowDataKey = model.RowDataKey;
                    documentTrackModel.DocumentType = model.DocumentType;
                    documentTrackModel.FilePath = model.FilePath;
                    documentTrackModel.IfDownload = model.IfDownload;

                    dbContext.DocumentTracks.Add(documentTrackModel);
                    dbContext.SaveChanges();
                    Transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.DocumentTrack, ActionConstants.Add, DbConstants.LogType.Info, documentTrackModel.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    Transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.DocumentTrack);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.DocumentTrack, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public List<DocumentTrackViewModel> GetDocumentTrackList(DocumentTrackViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;



                IQueryable<DocumentTrackViewModel> DocumentTrackList = (from a in dbContext.DocumentTracks
                                                                        //where (a.AppUser.Application.StudentName.Contains(model.SearchText) || a.AppUser.Application.AdmissionNo.Contains(model.SearchText))
                                                                        select new DocumentTrackViewModel
                                                                        {
                                                                            RowKey = a.RowKey,
                                                                            AppUserKey = a.AppUserKey,
                                                                            StudentName = a.AppUser.Application.StudentName,
                                                                            AdmissionNo = a.AppUser.Application.AdmissionNo,
                                                                            Course = a.AppUser.Application.Course.CourseName,
                                                                            Date = a.Date,
                                                                            RowDataKey = a.RowDataKey ?? 0,
                                                                            DocumentType = a.DocumentType,
                                                                            FilePath = a.FilePath,
                                                                            IfDownload = a.IfDownload,
                                                                            MobileNumber = a.AppUser.Application.StudentMobile,
                                                                            ApplicationStatusName = a.AppUser.Application.StudentStatu.StudentStatusName,
                                                                            Batch = a.AppUser.Application.Batch.BatchName,
                                                                            BranchKey = a.AppUser.Application.BranchKey,
                                                                            BatchKey = a.AppUser.Application.BatchKey,
                                                                            Branch = a.AppUser.Application.Branch.BranchName,
                                                                            AcademicTermKey = a.AppUser.Application.AcademicTermKey,
                                                                            CurrentYear = a.AppUser.Application.CurrentYear,
                                                                            CourseDuration = a.AppUser.Application.Course.CourseDuration ?? 0,
                                                                            //DocumentViewCount=dbContext.DocumentTracks.Where(x=>x.AppUserKey==a.AppUserKey && x.DocumentType==model.DocumentType && x.RowDataKey == model.RowDataKey && x.IfDownload == false).Count(),
                                                                            //DocumentDownloadCount=dbContext.DocumentTracks.Where(x=>x.AppUserKey==a.AppUserKey && x.DocumentType==model.DocumentType && x.RowDataKey == model.RowDataKey && x.IfDownload == true).Count(),
                                                                        });

                if (model.DocumentType != 0)
                {
                    DocumentTrackList = DocumentTrackList.Where(x => x.DocumentType == model.DocumentType);
                }
                if (model.RowDataKey != 0)
                {
                    DocumentTrackList = DocumentTrackList.Where(x => x.RowDataKey == model.RowDataKey);
                }
                DocumentTrackList = DocumentTrackList.GroupBy(x => x.RowKey).Select(y => y.FirstOrDefault());
                if (model.SortBy != "")
                {
                    DocumentTrackList = SortApplications(DocumentTrackList, model.SortBy, model.SortOrder);
                }
                TotalRecords = DocumentTrackList.Count();
                return DocumentTrackList.Skip(Skip).Take(Take).ToList<DocumentTrackViewModel>();
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.Video, ActionConstants.MenuAccess, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return new List<DocumentTrackViewModel>();
            }
        }
        private IQueryable<DocumentTrackViewModel> SortApplications(IQueryable<DocumentTrackViewModel> Query, string SortName, string SortOrder)
        {

            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(DocumentTrackViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<DocumentTrackViewModel>(resultExpression);

        }


    }
}
