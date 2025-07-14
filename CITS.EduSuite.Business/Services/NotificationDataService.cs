using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System.Linq.Expressions;


namespace CITS.EduSuite.Business.Services
{
    public class NotificationDataService : INotificationDataService
    {

        private EduSuiteDatabase dbContext;

        public NotificationDataService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<NotificationDataViewModel> GetPushNotification(NotificationDataViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;

                IQueryable<NotificationDataViewModel> PushNotificationList = (from a in dbContext.PushNotifications
                                                                              //orderby e.RowKey
                                                                              where ((a.NotificationTitle.Contains(model.SearchText)) || (a.NotificationContent.Contains(model.SearchText)))
                                                                              select new NotificationDataViewModel
                                                                                 {
                                                                                     RowKey = a.RowKey,
                                                                                     PushNotificationTitle = a.NotificationTitle,
                                                                                     PushNotificationContent = a.NotificationContent,
                                                                                     CreatedDate = a.DateAdded,
                                                                                     AddedByText = dbContext.AppUsers.Where(x => x.RowKey == a.AddedBy).Select(y => y.AppUserName).FirstOrDefault(),
                                                                                     PushNotificationRedirectUrl = a.NotificationRedirectUrl,
                                                                                     AppUserKey = a.AddedBy
                                                                                 });

                if (model.AppUserKey != 0 && model.AppUserKey != null)
                    PushNotificationList = PushNotificationList.Where(row => row.AppUserKey == model.AppUserKey);

                if (model.SearchFromDate != null && model.SearchToDate != null)
                    PushNotificationList = PushNotificationList.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.CreatedDate) >= System.Data.Entity.DbFunctions.TruncateTime(model.SearchFromDate) && System.Data.Entity.DbFunctions.TruncateTime(row.CreatedDate) <= System.Data.Entity.DbFunctions.TruncateTime(model.SearchToDate));
                else if (model.SearchFromDate != null)
                    PushNotificationList = PushNotificationList.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.CreatedDate) >= System.Data.Entity.DbFunctions.TruncateTime(model.SearchFromDate));
                else if (model.SearchToDate != null)
                    PushNotificationList = PushNotificationList.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.CreatedDate) <= System.Data.Entity.DbFunctions.TruncateTime(model.SearchToDate));


                PushNotificationList = PushNotificationList.GroupBy(x => x.RowKey).Select(y => y.FirstOrDefault());
                if (model.SortBy != "")
                {
                    PushNotificationList = SortPushNotification(PushNotificationList, model.SortBy, model.SortOrder);
                }
                TotalRecords = PushNotificationList.Count();
                return PushNotificationList.Skip(Skip).Take(Take).ToList<NotificationDataViewModel>();
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.NotificationData, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<NotificationDataViewModel>();

            }
        }


        private IQueryable<NotificationDataViewModel> SortPushNotification(IQueryable<NotificationDataViewModel> Query, string SortName, string SortOrder)
        {

            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(NotificationDataViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<NotificationDataViewModel>(resultExpression);

        }


        public List<NotificationDataViewModel> GetNotification(NotificationDataViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;

                IQueryable<NotificationDataViewModel> NotificationList = (from a in dbContext.NotificationDatas
                                                                          //orderby e.RowKey
                                                                          where ((a.NotificationTitle.Contains(model.SearchText)) || (a.NotificationContent.Contains(model.SearchText)))

                                                                          select new NotificationDataViewModel
                                                                          {
                                                                              RowKey = a.RowKey,
                                                                              PushNotificationTitle = a.NotificationTitle,
                                                                              PushNotificationContent = a.NotificationContent,
                                                                              CreatedDate = a.DateAdded,
                                                                              AddedByText = dbContext.AppUsers.Where(x => x.RowKey == a.AddedBy).Select(y => y.AppUserName).FirstOrDefault(),
                                                                              AppUserKey = a.AddedBy

                                                                          });
                if (model.AppUserKey != 0 && model.AppUserKey != null)
                    NotificationList = NotificationList.Where(row => row.AppUserKey == model.AppUserKey);

                if (model.SearchFromDate != null && model.SearchToDate != null)
                    NotificationList = NotificationList.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.CreatedDate) >= System.Data.Entity.DbFunctions.TruncateTime(model.SearchFromDate) && System.Data.Entity.DbFunctions.TruncateTime(row.CreatedDate) <= System.Data.Entity.DbFunctions.TruncateTime(model.SearchToDate));
                else if (model.SearchFromDate != null)
                    NotificationList = NotificationList.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.CreatedDate) >= System.Data.Entity.DbFunctions.TruncateTime(model.SearchFromDate));
                else if (model.SearchToDate != null)
                    NotificationList = NotificationList.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.CreatedDate) <= System.Data.Entity.DbFunctions.TruncateTime(model.SearchToDate));

                NotificationList = NotificationList.GroupBy(x => x.RowKey).Select(y => y.FirstOrDefault());
                if (model.SortBy != "")
                {
                    NotificationList = SortPushNotification(NotificationList, model.SortBy, model.SortOrder);
                }
                TotalRecords = NotificationList.Count();
                return NotificationList.Skip(Skip).Take(Take).ToList<NotificationDataViewModel>();
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.NotificationData, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<NotificationDataViewModel>();

            }
        }


        private IQueryable<NotificationDataViewModel> SortNotification(IQueryable<NotificationDataViewModel> Query, string SortName, string SortOrder)
        {

            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(NotificationDataViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<NotificationDataViewModel>(resultExpression);

        }
    }
}
