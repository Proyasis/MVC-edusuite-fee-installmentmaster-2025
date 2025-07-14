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
    public class BookCopyService : IBookCopyService
    {
        private EduSuiteDatabase dbContext;
        public BookCopyService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public List<BookCopyViewModel> GetBookCopies(long BookId)
        {
            try
            {
                var bookCopyList = (from a in dbContext.BookCopies
                                    orderby a.RowKey
                                    where a.BookKey == BookId
                                    select new BookCopyViewModel
                                    {
                                        RowKey = a.RowKey,
                                        BookKey = a.BookKey,
                                        BookCopySlNo = a.BookCopySlNo,
                                        SerialNo = a.SerialNo,
                                        ISBN = a.ISBN,
                                        BookBarCode = a.BookBarCode,
                                        BookEdition = a.BookEdition,
                                        BookPrintYear = a.BookPrintYear,
                                        NoOfPages = a.NoOfPages,
                                        BookPrice = a.BookPrice,
                                        FineAmount = a.FineAmount,
                                        IsIssued = a.IsIssued,
                                        //RackName = a.Rack.RackName,
                                        IsActive = a.IsActive??false,
                                        BookStatusName=a.BookStatu.BookStatusName
                                    }).ToList();
                return bookCopyList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<BookCopyViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.BookCopy, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<BookCopyViewModel>();
                

            }
        }

        public BookCopyViewModel GetBookCopyById(long? id)
        {
            BookCopyViewModel model = new BookCopyViewModel();
            model = dbContext.BookCopies.Select(row => new BookCopyViewModel
            {
                RowKey = row.RowKey,
                BookCopySlNo = row.BookCopySlNo,
                SerialNo = row.SerialNo,
                ISBN = row.ISBN,
                BookBarCode = row.BookBarCode,
                BookEdition = row.BookEdition,
                BookPrintYear = row.BookPrintYear,
                NoOfPages = row.NoOfPages,
                BookPrice = row.BookPrice,
                FineAmount = row.FineAmount,
                IsIssued = row.IsIssued,
                //RackKey = row.RackKey,
                IsActive = row.IsActive??false,
                BookStatusKey = row.BookStatusKey

            }).Where(x => x.RowKey == id).FirstOrDefault();
            if (model == null)
            {
                model = new BookCopyViewModel();
            }
            FillDropdownLists(model);
            return model;
        }

        public BookCopyViewModel CreateBookCopy(BookCopyViewModel model)
        {
            Employee employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();

            var bookCopyNameCheck = dbContext.BookCopies.Where(row => row.ISBN != null && row.ISBN != "" && row.ISBN.ToLower() == model.ISBN.ToLower()).ToList();
            BookCopy bookCopyModel = new BookCopy();

            FillDropdownLists(model);
            if (bookCopyNameCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.ISBN);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    LibraryBook books = dbContext.LibraryBooks.SingleOrDefault(x => x.RowKey == model.BookKey);
                    var subRack = dbContext.SubRacks.SingleOrDefault(x => x.RowKey == books.SubRackKey);
                    var rack = dbContext.Racks.SingleOrDefault(x => x.RowKey == subRack.RackKey);

                    Int64 maxKey = dbContext.BookCopies.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    Int64 maxSerialNo = dbContext.BookCopies.Where(x => x.LibraryBook.SubRackKey == subRack.RowKey && x.LibraryBook.SubRack.Rack.RowKey == rack.RowKey && x.LibraryBook.BranchKey == books.BranchKey).Select(x => x.SerialNo).DefaultIfEmpty().Max();

                    //Int16 maxSerialNo = dbContext.BookCopies.Select(p => p.BookCopySlNo).DefaultIfEmpty().Max();
                    bookCopyModel.RowKey = Convert.ToInt64(maxKey + 1);
                    bookCopyModel.BookKey = model.BookKey;
                    //bookCopyModel.BookCopySlNo = Convert.ToInt16(maxSerialNo + 1);
                    bookCopyModel.SerialNo = maxSerialNo + 1;
                    bookCopyModel.BookCopySlNo = books.Branch.BranchCode + rack.RackCode + subRack.SubRackCode + (maxSerialNo + 1);
                    bookCopyModel.ISBN = model.ISBN;
                    bookCopyModel.BookBarCode = model.BookBarCode;
                    bookCopyModel.BookEdition = model.BookEdition;
                    bookCopyModel.BookPrintYear = model.BookPrintYear;
                    bookCopyModel.NoOfPages = model.NoOfPages;
                    bookCopyModel.BookPrice = Convert.ToDecimal(model.BookPrice);
                    bookCopyModel.FineAmount = Convert.ToDecimal(model.FineAmount);
                    bookCopyModel.IsIssued = false;
                    //bookCopyModel.RackKey = model.RackKey;
                    bookCopyModel.IsActive = model.IsActive;
                    bookCopyModel.BookStatusKey = model.BookStatusKey;
                    dbContext.BookCopies.Add(bookCopyModel);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.BookCopy, ActionConstants.Add, DbConstants.LogType.Info, bookCopyModel.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.BookCopy);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.BookCopy, ActionConstants.Add, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                }
            }

            return model;
        }

        public BookCopyViewModel UpdateBookCopy(BookCopyViewModel model)
        {
            var bookCopyNameCheck = dbContext.BookCopies.Where(row => row.RowKey != model.RowKey && row.ISBN != null && row.ISBN != "" && row.ISBN.ToLower() == model.ISBN.ToLower()).ToList();
            BookCopy bookCopyModel = new BookCopy();

            FillDropdownLists(model);
            if (bookCopyNameCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.ISBN);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    bookCopyModel = dbContext.BookCopies.SingleOrDefault(row => row.RowKey == model.RowKey);
                    bookCopyModel.BookCopySlNo = model.BookCopySlNo;
                    bookCopyModel.ISBN = model.ISBN;
                    bookCopyModel.BookBarCode = model.BookBarCode;
                    bookCopyModel.BookEdition = model.BookEdition;
                    bookCopyModel.BookPrintYear = model.BookPrintYear;
                    bookCopyModel.NoOfPages = model.NoOfPages;
                    bookCopyModel.BookPrice = Convert.ToDecimal(model.BookPrice);
                    bookCopyModel.FineAmount = Convert.ToDecimal(model.FineAmount);
                    //bookCopyModel.IsIssued = model.IsIssued;
                    //bookCopyModel.RackKey = model.RackKey;
                    bookCopyModel.BookStatusKey = model.BookStatusKey;
                    bookCopyModel.IsActive = model.IsActive;

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.BookCopy, ActionConstants.Edit, DbConstants.LogType.Info, bookCopyModel.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.BookCopy);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.BookCopy, ActionConstants.Edit, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                }
            }

            return model;
        }

        public BookCopyViewModel DeleteBookCopy(BookCopyViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    BookCopy bookCopy = dbContext.BookCopies.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.BookCopies.Remove(bookCopy);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.BookCopy, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.BookCopy);
                        model.IsSuccessful = false;

                        ActivityLog.CreateActivityLog(MenuConstants.BookCopy, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.BookCopy);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.BookCopy, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        private void FillDropdownLists(BookCopyViewModel model)
        {
            FillBooks(model);
            FillBookStatus(model);

        }

        private void FillBooks(BookCopyViewModel model)
        {
            model.Books = dbContext.LibraryBooks.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BookName
            }).ToList();
        }

        //private void FillRacks(BookCopyViewModel model)
        //{
        //    model.Racks = dbContext.Racks.Select(row => new SelectListModel
        //    {
        //        RowKey = row.RowKey,
        //        Text = row.RackName
        //    }).ToList();
        //}

        private void FillBookStatus(BookCopyViewModel model)
        {
            model.BookStatuses = dbContext.BookStatus.Select(x => new SelectListModel
            {
                Text = x.BookStatusName,
                RowKey = x.RowKey
            }).ToList();
        }

        public List<BookCopyViewModel> GetBooksById(List<long> BookIds)
        {
            try
            {
                var bookCopyList = (from a in dbContext.BookCopies
                                    orderby a.BookCopySlNo
                                    select new BookCopyViewModel
                                    {
                                        RowKey = a.RowKey,
                                        BookKey = a.BookKey,
                                        BookName = a.LibraryBook.BookName,
                                        BookBarCode = a.BookBarCode,
                                        BookCopySlNo = a.BookCopySlNo

                                    });
                if (BookIds.Count > 0)
                {
                    bookCopyList = bookCopyList.Where(row => BookIds.Contains(row.BookKey));
                }
                return bookCopyList.ToList<BookCopyViewModel>();
            }
            catch (Exception ex)
            {
                return new List<BookCopyViewModel>();
            }
        }


    }
}
