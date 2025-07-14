using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Services
{
    public class BookCategoryService : IBookCategoryService
    {
        private EduSuiteDatabase dbContext;

        public BookCategoryService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<BookCategoryViewModel> GetBookCategory(string searchText)
        {
            try
            {
                var bookCategoryList = (from p in dbContext.BookCategories
                                        orderby p.RowKey descending
                                        where (p.BookCategoryName.Contains(searchText))
                                        select new BookCategoryViewModel
                                     {
                                         RowKey = p.RowKey,
                                         BookCategoryName = p.BookCategoryName,
                                         IsActive = p.IsActive ?? false
                                     }).ToList();

                return bookCategoryList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<BookCategoryViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.BookCategory, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<BookCategoryViewModel>();
            }
        }

        public BookCategoryViewModel GetBookCategoryById(short? id)
        {
            try
            {
                BookCategoryViewModel model = new BookCategoryViewModel();
                model = dbContext.BookCategories.Select(row => new BookCategoryViewModel
                {
                    RowKey = row.RowKey,
                    BookCategoryName = row.BookCategoryName,
                    IsActive = row.IsActive ?? false

                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new BookCategoryViewModel();
                }
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.BookCategory, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new BookCategoryViewModel();
                

            }
        }

        public BookCategoryViewModel CreateBookCategory(BookCategoryViewModel model)
        {
           
            var BookCategoryNameCheck = dbContext.BookCategories.Where(row => row.BookCategoryName.ToLower() == model.BookCategoryName.ToLower()).ToList();
            BookCategory BookCategoryModel = new BookCategory();

            if (BookCategoryNameCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.BookCategory);
                model.IsSuccessful = false;
                return model;
            }
           

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Int16 maxKey = dbContext.BookCategories.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    BookCategoryModel.RowKey = Convert.ToInt16(maxKey + 1);
                    BookCategoryModel.BookCategoryName = model.BookCategoryName;
                    BookCategoryModel.IsActive = model.IsActive;
                    dbContext.BookCategories.Add(BookCategoryModel);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.BookCategory, ActionConstants.Add, DbConstants.LogType.Info, BookCategoryModel.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.BookCategory);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.BookCategory, ActionConstants.Add, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                }
            }
            
            return model;
        }

        public BookCategoryViewModel UpdateBookCategory(BookCategoryViewModel model)
        {
           
            var bookCategoryNameCheck = dbContext.BookCategories.Where(row => row.BookCategoryName.ToLower() == model.BookCategoryName.ToLower() && row.RowKey != model.RowKey).ToList();
            BookCategory rackModel = new BookCategory();

            if (bookCategoryNameCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.BookCategory);
                model.IsSuccessful = false;
                return model;
            }
            
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    rackModel = dbContext.BookCategories.SingleOrDefault(row => row.RowKey == model.RowKey);
                    rackModel.BookCategoryName = model.BookCategoryName;
                    rackModel.IsActive = model.IsActive;

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.BookCategory, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.BookCategory);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.BookCategory, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            
            return model;
        }

        public BookCategoryViewModel DeleteBookCategory(BookCategoryViewModel model)
        {
           
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    BookCategory bookCategory = dbContext.BookCategories.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.BookCategories.Remove(bookCategory);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.BookCategory, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.BookCategory);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.BookCategory, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.BookCategory);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.BookCategory, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                }
            }
           
            return model;
        }

     

    }
}
