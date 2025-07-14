using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
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
    public class BookIssueTypeService : IBookIssueTypeService
    {
        private EduSuiteDatabase dbContext;
        public BookIssueTypeService(EduSuiteDatabase objDB)
        {
            dbContext = objDB;
        }
        public List<BookIssueTypeViewModel> GetBookIssueTypes(string searchText)
        {
            try
            {
                var bookIssueTypeList = (from bt in dbContext.BookIssueTypes
                                         orderby bt.RowKey descending
                                         where bt.BookIssueTypeName.Contains(searchText)
                                         select new BookIssueTypeViewModel
                                         {
                                             RowKey = bt.RowKey,
                                             BookIssueTypeName = bt.BookIssueTypeName,
                                             IsActive = bt.IsActive??false

                                         }).ToList();
                return bookIssueTypeList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<BookIssueTypeViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.BookIssueType, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<BookIssueTypeViewModel>();
               
            }
        }

        public BookIssueTypeViewModel GetBookIssueTypeById(byte? id)
        {
            BookIssueTypeViewModel model = new BookIssueTypeViewModel();
            model = dbContext.BookIssueTypes.Select(row => new BookIssueTypeViewModel
            {
                RowKey = row.RowKey,
                BookIssueTypeName = row.BookIssueTypeName,
                IsActive = row.IsActive ?? false

            }).Where(x => x.RowKey == id).FirstOrDefault();
            if (model == null)
            {
                model = new BookIssueTypeViewModel();
            }
            return model;
        }

        public BookIssueTypeViewModel CreateBookIssueType(BookIssueTypeViewModel model)
        {
            var bookIssueTypeNameCheck = dbContext.BookIssueTypes.Where(row => row.BookIssueTypeName.ToLower() == model.BookIssueTypeName.ToLower()).ToList();
            BookIssueType bookIssueTypeModel = new BookIssueType();

            if (bookIssueTypeNameCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.BookIssueType);
                model.IsSuccessful = false;
                return model;
            }
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Byte maxKey = dbContext.BookIssueTypes.Select(a => a.RowKey).DefaultIfEmpty().Max();
                    bookIssueTypeModel.RowKey = Convert.ToByte(maxKey + 1);
                    bookIssueTypeModel.BookIssueTypeName = model.BookIssueTypeName;
                    bookIssueTypeModel.IsActive = model.IsActive;
                    dbContext.BookIssueTypes.Add(bookIssueTypeModel);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.BookIssueType, ActionConstants.Add, DbConstants.LogType.Info, bookIssueTypeModel.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.BookIssueType);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.BookIssueType, ActionConstants.Add, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public BookIssueTypeViewModel UpdateBookIssueType(BookIssueTypeViewModel model)
        {
            var bookIssueTypeNameCheck = dbContext.BookIssueTypes.Where(row => row.RowKey != model.RowKey && row.BookIssueTypeName.ToLower() == model.BookIssueTypeName.ToLower()).ToList();
            BookIssueType bookIssueTypeModel = new BookIssueType();

            if (bookIssueTypeNameCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.BookIssueType);
                model.IsSuccessful = false;
                return model;
            }
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    bookIssueTypeModel = dbContext.BookIssueTypes.SingleOrDefault(row => row.RowKey == model.RowKey);
                    bookIssueTypeModel.BookIssueTypeName = model.BookIssueTypeName;
                    bookIssueTypeModel.IsActive = model.IsActive;

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.BookIssueType, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.BookIssueType);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.BookIssueType, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public BookIssueTypeViewModel DeleteBookIssueType(BookIssueTypeViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    BookIssueType bookIssueType = dbContext.BookIssueTypes.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.BookIssueTypes.Remove(bookIssueType);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.BookIssueType, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.BookIssueType);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.BookIssueType, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException

().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.BookIssueType);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.BookIssueType, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException

().Message);
                }
            }
            return model;
        }

       
    }
}
