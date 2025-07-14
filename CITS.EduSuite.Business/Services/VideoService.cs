using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;
using System.Data.Common;
using CITS.EduSuite.Business.Extensions;
using System.Linq.Expressions;

namespace CITS.EduSuite.Business.Services
{
    public class VideoService : IVideoService
    {
        private EduSuiteDatabase dbContext;
        public VideoService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public List<VideoViewModel> GetVideos(VideoViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;



                IQueryable<VideoViewModel> VideoList = (from a in dbContext.Videos.Where(x => new[] { x.Subject.SubjectName, x.VideoTitle }.Any(s => s.Contains(model.SearchText)))
                                                        select new VideoViewModel
                                                        {
                                                            RowKey = a.RowKey,
                                                            SubjectName = a.Subject.SubjectName,
                                                            SubjectKey = a.SubjectKey,
                                                            VideoCount = a.VideoDetails.Count(),
                                                            VideoTitle = a.VideoTitle,
                                                            IsActive = a.IsActive,
                                                        });

                if (model.SortBy != "")
                {
                    VideoList = SortApplications(VideoList, model.SortBy, model.SortOrder);
                }
                TotalRecords = VideoList.Count();
                return VideoList.Skip(Skip).Take(Take).ToList<VideoViewModel>();
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.Video, ActionConstants.MenuAccess, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return new List<VideoViewModel>();
            }
        }
        private IQueryable<VideoViewModel> SortApplications(IQueryable<VideoViewModel> Query, string SortName, string SortOrder)
        {

            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(VideoViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<VideoViewModel>(resultExpression);

        }

        public List<VideoViewModel> GetVideoByVideoKey(VideoViewModel model)
        {
            try
            {
                IQueryable<VideoViewModel> VideoList = (from a in dbContext.Videos
                                                        select new VideoViewModel
                                                        {
                                                            RowKey = a.RowKey,
                                                            SubjectName = a.Subject.SubjectName,
                                                            SubjectKey = a.SubjectKey,
                                                            VideoList = dbContext.VideoDetails.Where(x => x.VideoKey == model.RowKey).Select(x => new VideoDetailsViewModel
                                                            {
                                                                RowKey = x.RowKey,
                                                                VideoFileName = x.VideoFileName,
                                                                VideoDecription = x.VideoDescription,
                                                                IsActive = x.IsActive,
                                                                IsAllowDownload = x.AllowDownload,
                                                                VisibilityTypeKey = x.VisibilityTypeKey,
                                                                VisibilityName = x.VisibilityType.VisibilityTypeName,
                                                                VideoTypeKey = x.VideoTypeKey,
                                                                YouTubeLinks = x.YoutubeLink,
                                                                VideoViewCount = dbContext.DocumentTracks.Where(y => y.RowDataKey == x.RowKey && y.DocumentType == DbConstants.DocumentType.Video && y.IfDownload == false).Count(),
                                                               VideoDownloadCount = dbContext.DocumentTracks.Where(y => y.RowDataKey == x.RowKey && y.DocumentType == DbConstants.DocumentType.Video && y.IfDownload == true).Count(),
                                                            }).ToList(),
                                                            VideoTitle = a.VideoTitle,
                                                            IsActive = a.IsActive,
                                                        });


                if (model.RowKey != 0)
                {
                    VideoList = VideoList.Where(x => x.RowKey == model.RowKey);
                }
                if (model.SortBy != "")
                {
                    //VideoList = SortApplications(VideoList, model.SortBy, model.SortOrder);
                }

                return VideoList.ToList<VideoViewModel>();
            }
            catch (Exception ex)
            {

                ActivityLog.CreateActivityLog(MenuConstants.Video, ActionConstants.MenuAccess, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return new List<VideoViewModel>();
            }
        }
        public List<VideoViewModel> GetVideoByVideoDetailsKey(VideoViewModel model)
        {
            try
            {
                IQueryable<VideoViewModel> VideoList = (from a in dbContext.VideoDetails
                                                        select new VideoViewModel
                                                        {
                                                            RowKey = a.Video.RowKey,
                                                            VideoDetailsKey = a.RowKey,
                                                            SubjectName = a.Video.Subject.SubjectName,
                                                            SubjectKey = a.Video.SubjectKey,
                                                            VideoList = dbContext.VideoDetails.Where(x => x.RowKey == model.VideoDetailsKey).Select(x => new VideoDetailsViewModel
                                                            {
                                                                RowKey = x.RowKey,
                                                                VideoFileName = x.VideoFileName,
                                                                VideoDecription = x.VideoDescription,
                                                                IsActive = x.IsActive,
                                                                VisibilityTypeKey = x.VisibilityTypeKey,
                                                                VideoTypeKey = x.VideoTypeKey,
                                                                YouTubeLinks = x.YoutubeLink
                                                            }).ToList(),
                                                            VideoTitle = a.Video.VideoTitle




                                                        });


                if (model.VideoDetailsKey != 0)
                {
                    VideoList = VideoList.Where(x => x.VideoDetailsKey == model.VideoDetailsKey);
                }


                if (model.SortBy != "")
                {
                    //VideoList = SortApplications(VideoList, model.SortBy, model.SortOrder);
                }

                return VideoList.ToList<VideoViewModel>();
            }
            catch (Exception ex)
            {

                ActivityLog.CreateActivityLog(MenuConstants.Video, ActionConstants.MenuAccess, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return new List<VideoViewModel>();
            }
        }

        public VideoViewModel GetVideoById(VideoViewModel model)
        {
            try
            {

                model = dbContext.Videos.Where(x => x.RowKey == model.RowKey).Select(row => new VideoViewModel
                {
                    RowKey = row.RowKey,
                    SubjectKey = row.SubjectKey,
                    VideoTitle = row.VideoTitle,
                    VideoList = dbContext.VideoDetails.Where(x => x.VideoKey == model.RowKey).Select(x => new VideoDetailsViewModel
                    {
                        RowKey = x.RowKey,
                        VideoFileName = x.VideoFileName,
                        VideoDecription = x.VideoDescription,
                        IsActive = x.IsActive,
                        IsAllowDownload = x.AllowDownload,
                        VisibilityTypeKey = x.VisibilityTypeKey,
                        VideoTypeKey = x.VideoTypeKey,
                        YouTubeLinks = x.YoutubeLink

                    }).ToList(),
                    SubjectName = row.Subject.SubjectName,
                    IsActive = row.IsActive,


                }).SingleOrDefault();




                if (model == null)
                {
                    model = new VideoViewModel();
                    model.IsActive = true;
                }
                else
                {

                }

                if (model.VideoList.Count() == 0)
                {
                    VideoDetailsViewModel VideoDetailsModel = new VideoDetailsViewModel();
                    VideoDetailsModel.IsActive = true;
                    model.VideoList.Add(VideoDetailsModel);
                }

                FillDropDownList(model);


                return model;
            }
            catch (Exception ex)
            {
                //ActivityLog.CreateActivityLog(MenuConstants.Video, ActionConstants.View, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                return new VideoViewModel();
            }
        }

        public VideoViewModel CreateVideo(VideoViewModel model)
        {
            Int64 maxKey = dbContext.Videos.Select(p => p.RowKey).DefaultIfEmpty().Max();

            using (var transaction = dbContext.Database.BeginTransaction())
            {

                try
                {


                    Video dbModel = new Video();
                    dbModel.RowKey = (maxKey + 1);
                    dbModel.SubjectKey = model.SubjectKey;
                    dbModel.VideoTitle = model.VideoTitle;

                    dbModel.IsActive = model.IsActive;
                    dbContext.Videos.Add(dbModel);
                    model.RowKey = dbModel.RowKey;
                    CreateVideoDetails(model);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Video, ActionConstants.Add, DbConstants.LogType.Info, dbModel.RowKey, model.Message);


                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Video);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Video, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;

        }

        public VideoViewModel UpdateVideo(VideoViewModel model)
        {

            using (var transaction = dbContext.Database.BeginTransaction())
            {

                try
                {

                    Video dbModel = dbContext.Videos.Where(x => x.RowKey == model.RowKey).SingleOrDefault();
                    dbModel.SubjectKey = model.SubjectKey;
                    dbModel.VideoTitle = model.VideoTitle;
                    dbModel.IsActive = model.IsActive;
                    UpdateVideoDetails(model);
                    CreateVideoDetails(model);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.Video, ActionConstants.Edit, DbConstants.LogType.Info, dbModel.RowKey, model.Message);


                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Video);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Video, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;

        }

        public void CreateVideoDetails(VideoViewModel model)
        {
            Int64 maxKey = dbContext.VideoDetails.Select(p => p.RowKey).DefaultIfEmpty().Max();

            foreach (var item in model.VideoList.Where(x => x.RowKey == 0))
            {
                VideoDetail dbModel = new VideoDetail();


                dbModel.RowKey = (maxKey + 1);
                dbModel.VideoFileName = dbModel.RowKey + item.VideoFileName ?? "";
                dbModel.VideoDescription = item.VideoDecription;
                dbModel.VideoKey = model.RowKey;
                dbModel.IsActive = item.IsActive;
                dbModel.AllowDownload = item.IsAllowDownload;
                dbModel.VisibilityTypeKey = item.VisibilityTypeKey ?? 1;
                dbModel.VideoTypeKey = item.VideoTypeKey;
                dbModel.YoutubeLink = item.YouTubeLinks;
                dbContext.VideoDetails.Add(dbModel);
                maxKey++;
                item.VideoFileName = dbModel.VideoFileName;
            }
        }

        public void UpdateVideoDetails(VideoViewModel model)
        {

            foreach (var item in model.VideoList.Where(x => x.RowKey > 0))
            {
                VideoDetail dbModel = dbContext.VideoDetails.Where(x => x.RowKey == item.RowKey).SingleOrDefault();
                dbModel.VideoDescription = item.VideoDecription;
                dbModel.VideoKey = model.RowKey;
                dbModel.AllowDownload = item.IsAllowDownload;
                dbModel.IsActive = item.IsActive;
                dbModel.VisibilityTypeKey = item.VisibilityTypeKey ?? 1;
                dbModel.VideoTypeKey = item.VideoTypeKey;
                dbModel.YoutubeLink = item.YouTubeLinks;
                if (item.VideoFileName != null)
                {
                    dbModel.VideoFileName = dbModel.RowKey + item.VideoFileName;
                    item.VideoFileName = dbModel.VideoFileName;
                }

            }
        }


        public void FillDropDownList(VideoViewModel model)
        {
            FillSubject(model);
            FillSubject(model);
            FillVisibility(model);
            FillVideoTypes(model);
        }

        private void FillSubject(VideoViewModel model)
        {
            model.VideoSubject = dbContext.Subjects.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.SubjectName
            }).ToList();
        }

        private void FillVisibility(VideoViewModel model)
        {
            model.VisibilityTypes = dbContext.VisibilityTypes.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.VisibilityTypeName
            }).ToList();
        }

        private void FillVideoTypes(VideoViewModel model)
        {
            model.VideoTypes = typeof(DbConstants.VideoType).GetFields().Select(x => new SelectListModel
            {
                RowKey = Convert.ToByte(x.GetValue(null).ToString()),
                Text = x.Name
            }).ToList();
        }

        public VideoViewModel DeleteVideo(VideoViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Video dbVideo = dbContext.Videos.SingleOrDefault(row => row.RowKey == model.RowKey);
                    List<VideoDetail> dbVideoDetails = dbContext.VideoDetails.Where(row => row.VideoKey == model.RowKey).ToList();
                    dbContext.VideoDetails.RemoveRange(dbVideoDetails);
                    dbContext.Videos.Remove(dbVideo);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Video, ActionConstants.Delete, DbConstants.LogType.Info, model.VideoDetailsKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Video);
                        model.IsSuccessful = false;
                    }
                    ActivityLog.CreateActivityLog(MenuConstants.Video, ActionConstants.Delete, DbConstants.LogType.Error, model.VideoDetailsKey, ex.GetBaseException().Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Video);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Video, ActionConstants.Delete, DbConstants.LogType.Error, model.VideoDetailsKey, ex.GetBaseException().Message);
                }

                return model;
            }
        }

        public VideoViewModel DeleteVideoDetails(VideoViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    VideoDetail dbVideoDetails = dbContext.VideoDetails.SingleOrDefault(row => row.RowKey == model.VideoDetailsKey);
                    model.VideoList.Add(new VideoDetailsViewModel { VideoFileName = dbVideoDetails.VideoFileName });
                    model.RowKey = dbVideoDetails.VideoKey;
                    dbContext.VideoDetails.Remove(dbVideoDetails);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Video, ActionConstants.Delete, DbConstants.LogType.Info, model.VideoDetailsKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Video);
                        model.IsSuccessful = false;
                    }
                    ActivityLog.CreateActivityLog(MenuConstants.Video, ActionConstants.Delete, DbConstants.LogType.Error, model.VideoDetailsKey, ex.GetBaseException().Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Video);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Video, ActionConstants.Delete, DbConstants.LogType.Error, model.VideoDetailsKey, ex.GetBaseException().Message);
                }

                return model;
            }
        }


        //for students
        public VideoViewModel GetSubjects(VideoViewModel model)
        {

            long userkey = DbConstants.User.UserKey;

            List<long> SubjectKeys = (from a in dbContext.Applications
                                      join b in dbContext.CourseSubjectMasters on a.CourseKey equals b.CourseKey
                                      join c in dbContext.CourseSubjectDetails on b.RowKey equals c.CourseSubjectMasterKey
                                      join s in dbContext.Subjects on c.SubjectKey equals s.RowKey
                                      where a.UniversityMasterKey == b.UniversityMasterKey && a.AcademicTermKey == b.AcademicTermKey
                                      && a.AppUserKey == userkey
                                      select new CourseSubjectDetailViewModel
                                      {
                                          SubjectKey = c.SubjectKey
                                      }).Select(x => x.SubjectKey).ToList();

            model.SubjectList = (from a in dbContext.Subjects
                                 where a.IsActive && SubjectKeys.Contains(a.RowKey)
                                 select new SubjectList
                                 {
                                     SubjectKey = a.RowKey,
                                     SubjectName = a.SubjectName,
                                 }).ToList();

            return model;

        }

        public VideoViewModel GetVideoList(VideoViewModel model)
        {

            var Take = model.PageSize;
            var skip = (model.PageIndex - 1) * model.PageSize;

            IQueryable<VideoViewModel> VideoQuary = (
                               from VD in dbContext.Videos.OrderByDescending(row => new { row.DateAdded })
                               where VD.IsActive && VD.Subject.IsActive
                               select new VideoViewModel
                               {
                                   RowKey = VD.RowKey,
                                   SubjectName = VD.Subject.SubjectName,
                                   VideoTitle = VD.VideoTitle,
                                   SubjectKey = VD.SubjectKey,
                                   VideoCount = VD.VideoDetails.Where(x => x.IsActive).Count(),


                               });


            if (model.SubjectKey != 0)
            {
                VideoQuary = VideoQuary.Where(x => x.SubjectKey == model.SubjectKey);
            }

            model.TotalRecords = VideoQuary.Count();
            model.Videos = VideoQuary.OrderByDescending(Row => Row.RowKey).Skip(skip).Take(Take).ToList();

            return model;

        }

        public VideoViewModel GetVideoDetailsList(VideoViewModel model)
        {



            IQueryable<VideoDetailsViewModel> VideoQuary = (
                               from VD in dbContext.VideoDetails.OrderByDescending(row => new { row.DateAdded })
                               where VD.IsActive && VD.Video.IsActive && VD.Video.Subject.IsActive
                               select new VideoDetailsViewModel
                               {
                                   RowKey = VD.Video.RowKey,
                                   SubjectName = VD.Video.Subject.SubjectName,
                                   VideoTitle = VD.Video.VideoTitle,
                                   SubjectKey = VD.Video.SubjectKey,
                                   CreatedDate = VD.DateAdded,
                                   VideoFileName = VD.VideoFileName,
                                   VideoDecription = VD.VideoDescription,
                                   IsAllowDownload = VD.AllowDownload,
                                   VideoTypeKey=VD.VideoTypeKey,
                                   YouTubeLinks=VD.YoutubeLink,
                                  
                               });


            if (model.RowKey != 0)
            {
                VideoQuary = VideoQuary.Where(x => x.RowKey == model.RowKey);
            }

            model.TotalRecords = VideoQuary.Count();
            model.VideoList = VideoQuary.ToList();

            return model;

        }

    }
}
