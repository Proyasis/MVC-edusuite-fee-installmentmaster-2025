using CITS.EduSuite.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using CITS.EduSuite.Business.Common;
using System.Data.Entity.Infrastructure;
using CITS.EduSuite.Business.Models.Resources;
using System.Linq.Expressions;

namespace CITS.EduSuite.Business.Services
{
    public class AuthorService : IAuthorService
    {
        private EduSuiteDatabase dbContext;
        public AuthorService(EduSuiteDatabase objDB)
        {
            dbContext = objDB;
        }
        public List<AuthorViewModel> GetAuthors(AuthorViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;

                IQueryable<AuthorViewModel> authorList = (from a in dbContext.Authors
                                                          orderby a.RowKey descending
                                                          where (a.AuthorName.Contains(model.SearchText)|| a.AuthorNickName.Contains(model.SearchText) || a.AuthorPhone.Contains(model.SearchText))
                                                          select new AuthorViewModel
                                                          {
                                                              RowKey = a.RowKey,
                                                              AuthorName = a.AuthorName,
                                                              AuthorNickName = a.AuthorNickName,
                                                              AuthorAddress = a.AuthorAddress,
                                                              AuthorPhone = a.AuthorPhone,
                                                              IsActive = a.IsActive ?? false

                                                          });
               
                authorList = authorList.GroupBy(x => x.RowKey).Select(y => y.FirstOrDefault());
                if (model.SortBy != "")
                {
                    authorList = SortApplications(authorList, model.SortBy, model.SortOrder);
                }
                TotalRecords = authorList.Count();
                return authorList.OrderBy(Row => Row.RowKey).Skip(Skip).Take(Take).ToList<AuthorViewModel>();
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.Author, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);

                return new List<AuthorViewModel>();
            }
        }
        private IQueryable<AuthorViewModel> SortApplications(IQueryable<AuthorViewModel> Query, string SortName, string SortOrder)
        {

            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(AuthorViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<AuthorViewModel>(resultExpression);

        }


        public AuthorViewModel GetAuthorById(Int32? id)
        {
            AuthorViewModel model = new AuthorViewModel();
            model = dbContext.Authors.Select(row => new AuthorViewModel
            {
                RowKey = row.RowKey,
                AuthorName = row.AuthorName,
                AuthorNickName = row.AuthorNickName,
                AuthorAddress = row.AuthorAddress,
                AuthorPhone = row.AuthorPhone,
                IsActive = row.IsActive ?? false

            }).Where(x => x.RowKey == id).FirstOrDefault();
            if (model == null)
            {
                model = new AuthorViewModel();
            }

            return model;
        }

        public AuthorViewModel CreateAuthor(AuthorViewModel model)
        {
            var authorNameCheck = dbContext.Authors.Where(row => row.AuthorName.ToLower() == model.AuthorName.ToLower() && row.AuthorPhone != "" && row.AuthorPhone == model.AuthorPhone).ToList();
            Author authorModel = new Author();


            if (authorNameCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Author + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Name);
                model.IsSuccessful = false;
                return model;
            }
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Int32 maxKey = dbContext.Authors.Select(a => a.RowKey).DefaultIfEmpty().Max();
                    authorModel.RowKey = Convert.ToInt32(maxKey + 1);
                    authorModel.AuthorName = model.AuthorName;
                    authorModel.AuthorNickName = model.AuthorNickName;
                    authorModel.AuthorAddress = model.AuthorAddress;
                    authorModel.AuthorPhone = model.AuthorPhone;
                    authorModel.IsActive = model.IsActive;
                    dbContext.Authors.Add(authorModel);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Author);
                    model.IsSuccessful = false;
                }
            }
            return model;
        }

        public AuthorViewModel UpdateAuthor(AuthorViewModel model)
        {
            var authorNameCheck = dbContext.Authors.Where(row => row.RowKey != model.RowKey && row.AuthorName.ToLower() == model.AuthorName.ToLower() && row.AuthorPhone != "" && row.AuthorPhone == model.AuthorPhone).ToList();
            Author authorModel = new Author();


            if (authorNameCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Author + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Name);
                model.IsSuccessful = false;
                return model;
            }
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    authorModel = dbContext.Authors.SingleOrDefault(row => row.RowKey == model.RowKey);
                    authorModel.AuthorName = model.AuthorName;
                    authorModel.AuthorNickName = model.AuthorNickName;
                    authorModel.AuthorAddress = model.AuthorAddress;
                    authorModel.AuthorPhone = model.AuthorPhone;
                    authorModel.IsActive = model.IsActive;

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Author);
                    model.IsSuccessful = false;
                }
            }
            return model;
        }

        public AuthorViewModel DeleteAuthor(AuthorViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Author author = dbContext.Authors.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.Authors.Remove(author);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Author);
                        model.IsSuccessful = false;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Author);
                    model.IsSuccessful = false;
                }
            }
            return model;
        }


    }
}
