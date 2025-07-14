using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Services
{
    public class PublisherService : IPublisherService
    {
        private EduSuiteDatabase dbContext;

        public PublisherService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<PublisherViewModel> GetPublisher(PublisherViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;

                IQueryable<PublisherViewModel> publisherList = (from p in dbContext.Publishers
                                                                orderby p.RowKey descending
                                                                where (p.PublisherName.Contains(model.SearchText) || p.PublisherPhone.Contains(model.SearchText))
                                                                select new PublisherViewModel
                                                                {
                                                                    RowKey = p.RowKey,
                                                                    PublisherName = p.PublisherName,
                                                                    PublisherAddress = p.PublisherAddress,
                                                                    PublisherPhone = p.PublisherPhone,
                                                                    IsActive = p.IsActive ?? false,
                                                                });

                publisherList = publisherList.GroupBy(x => x.RowKey).Select(y => y.FirstOrDefault());
                if (model.SortBy != "")
                {
                    publisherList = SortApplications(publisherList, model.SortBy, model.SortOrder);
                }
                TotalRecords = publisherList.Count();
                return publisherList.OrderBy(Row => Row.RowKey).Skip(Skip).Take(Take).ToList<PublisherViewModel>();
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.Publisher, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<PublisherViewModel>();

            }
        }
        private IQueryable<PublisherViewModel> SortApplications(IQueryable<PublisherViewModel> Query, string SortName, string SortOrder)
        {

            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(PublisherViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<PublisherViewModel>(resultExpression);

        }
        public PublisherViewModel GetPublisherById(Int32? id)
        {
            try
            {
                PublisherViewModel model = new PublisherViewModel();
                model = dbContext.Publishers.Select(row => new PublisherViewModel
                {
                    RowKey = row.RowKey,
                    PublisherName = row.PublisherName,
                    PublisherAddress = row.PublisherAddress,
                    PublisherPhone = row.PublisherPhone,
                    IsActive = row.IsActive ?? false,
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new PublisherViewModel();
                }

                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Publisher, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new PublisherViewModel();

            }
        }

        public PublisherViewModel CreatePublisher(PublisherViewModel model)
        {
            var publisherNameCheck = dbContext.Publishers.Where(row => row.PublisherName.ToLower() == model.PublisherName.ToLower()).ToList();
            Publisher publisherModel = new Publisher();


            if (publisherNameCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.PublisherName);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Int32 maxKey = dbContext.Publishers.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    publisherModel.RowKey = Convert.ToInt32(maxKey + 1);
                    publisherModel.PublisherName = model.PublisherName;
                    publisherModel.PublisherAddress = model.PublisherAddress;
                    publisherModel.PublisherPhone = model.PublisherPhone;
                    publisherModel.IsActive = model.IsActive;
                    dbContext.Publishers.Add(publisherModel);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Publisher, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Publisher);
                    ActivityLog.CreateActivityLog(MenuConstants.Publisher, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public PublisherViewModel UpdatePublisher(PublisherViewModel model)
        {

            var publisherNameCheck = dbContext.Publishers.Where(row => row.PublisherName.ToLower() == model.PublisherName.ToLower() && row.RowKey != model.RowKey).ToList();
            Publisher publisherModel = new Publisher();

            if (publisherNameCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.PublisherName);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    publisherModel = dbContext.Publishers.SingleOrDefault(row => row.RowKey == model.RowKey);
                    publisherModel.PublisherName = model.PublisherName;
                    publisherModel.PublisherAddress = model.PublisherAddress;
                    publisherModel.PublisherPhone = model.PublisherPhone;
                    publisherModel.IsActive = model.IsActive;

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Publisher, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Publisher);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Publisher, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public PublisherViewModel DeletePublisher(PublisherViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Publisher publisher = dbContext.Publishers.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.Publishers.Remove(publisher);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Publisher, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Publisher);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.Publisher, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Publisher);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Publisher, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }


    }
}
