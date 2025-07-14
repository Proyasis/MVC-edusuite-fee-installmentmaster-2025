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
    public class BookStatusService : IBookStatusService
    {
        private EduSuiteDatabase dbContext;
        public BookStatusService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public List<BookStatusViewModel> GetBookStatus(string searchText)
        {
            try
            {
                var bookStatusList = (from bs in dbContext.BookStatus
                                      orderby bs.RowKey descending
                                      where (bs.BookStatusName.Contains(searchText))
                                      select new BookStatusViewModel
                                      {
                                          RowKey = bs.RowKey,
                                          BookStatusName = bs.BookStatusName,
                                          IsActive = bs.IsActive ?? false
                                      }).ToList();
                return bookStatusList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<BookStatusViewModel>();

            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.BookStatus, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<BookStatusViewModel>();
                
            }
        }
        public BookStatusViewModel GetBookStatusById(byte? id)
        {
            try
            {
                BookStatusViewModel model = new BookStatusViewModel();
                model = dbContext.BookStatus.Select(row => new BookStatusViewModel
                {
                    RowKey = row.RowKey,
                    BookStatusName = row.BookStatusName,


                    CanIssue = row.CanIssue,
                    IsActive = row.IsActive ?? false

                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {

                    model = new BookStatusViewModel();
                }


                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.BookStatus, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new BookStatusViewModel();
               

            }
        }
        public BookStatusViewModel CreateBookStatus(BookStatusViewModel model)
        {
            var BookStatusNameCheck = dbContext.BookStatus.Where(row => row.BookStatusName.ToLower() == model.BookStatusName.ToLower()).ToList();

            BookStatu BookStatusModel = new BookStatu();
            if (BookStatusNameCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.BookStatusName);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    byte maxKey = dbContext.BookStatus.Select(b => b.RowKey).DefaultIfEmpty().Max();
                    BookStatusModel.RowKey = Convert.ToByte(maxKey + 1);
                    BookStatusModel.BookStatusName = model.BookStatusName;
                    BookStatusModel.IsActive = model.IsActive;
                    BookStatusModel.CanIssue = model.CanIssue;
                    dbContext.BookStatus.Add(BookStatusModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.BookStatus, ActionConstants.Add, DbConstants.LogType.Info, BookStatusModel.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.BookStatus);
                    ActivityLog.CreateActivityLog(MenuConstants.BookStatus, ActionConstants.Add, DbConstants.LogType.Error, BookStatusModel.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }
        public BookStatusViewModel UpdateBooKStatus(BookStatusViewModel model)
        {
            var BookStatusNameCheck = dbContext.BookStatus.Where(row => row.BookStatusName.ToLower() == model.BookStatusName.ToLower() && row.RowKey != model.RowKey).ToList();

            BookStatu BookStatusModel = new BookStatu();
            if (BookStatusNameCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.BookStatusName);
                model.IsSuccessful = false;
                return model;
            }
            else
            {

                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        BookStatusModel = dbContext.BookStatus.SingleOrDefault(row => row.RowKey == model.RowKey);
                        BookStatusModel.BookStatusName = model.BookStatusName;
                        BookStatusModel.IsActive = model.IsActive;
                        BookStatusModel.CanIssue = model.CanIssue;
                        dbContext.SaveChanges();
                        transaction.Commit();
                        model.Message = EduSuiteUIResources.Success;
                        model.IsSuccessful = true;
                        ActivityLog.CreateActivityLog(MenuConstants.BookStatus, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.BookStatus);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.BookStatus, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                return model;
            }
        }
        public BookStatusViewModel DeleteBookStatus(BookStatusViewModel model)
        {

            using (var transaction = dbContext.Database.BeginTransaction())
            {

                try
                {
                    BookStatu BooKStatus = dbContext.BookStatus.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.BookStatus.Remove(BooKStatus);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.BookStatus, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.BookStatus);
                        model.IsSuccessful = false;

                        ActivityLog.CreateActivityLog(MenuConstants.BookStatus, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                    }

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.BookStatus);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.BookStatus, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }
    }
}




