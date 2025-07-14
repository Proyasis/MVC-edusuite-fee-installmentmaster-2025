using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
namespace CITS.EduSuite.Business.Services
{
    public class LibraryBookService : ILibraryBookService
    {
        private EduSuiteDatabase dbContext;
        public LibraryBookService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public List<LibraryBookViewModel> GetLibraryBooks(LibraryBookViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;

                IQueryable<LibraryBookViewModel> bookList = (from a in dbContext.LibraryBooks
                                                             orderby a.RowKey descending
                                                             where (a.BookName.Contains(model.SearchText) || a.BookName_Optional.Contains(model.SearchText) || a.Author.AuthorName.Contains(model.SearchText))
                                                             select new LibraryBookViewModel
                                                             {
                                                                 RowKey = a.RowKey,
                                                                 BookName = a.BookName,
                                                                 BookName_Optional = a.BookName_Optional,
                                                                 AuthorName = a.Author.AuthorName,
                                                                 BookCategoryName = a.BookCategory.BookCategoryName,
                                                                 BookIssueTypeName = a.BookIssueType.BookIssueTypeName,
                                                                 LanguageName = a.Language.LanguageName,
                                                                 PublisherName = a.Publisher.PublisherName,
                                                                 NoOfBooks = a.BookCopies.Count,
                                                                 BranchName = a.Branch.BranchName,
                                                                 BranchKey = a.BranchKey
                                                             });
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
                if (Employee != null)
                {
                    if (Employee.BranchAccess != null)
                    {
                        var Branches = Employee.BranchAccess.Split(',').Select(Int16.Parse).ToList();
                        bookList = bookList.Where(row => Branches.Contains(row.BranchKey));
                    }
                }
                if (model.BranchKey != 0)
                {
                    bookList = bookList.Where(x => x.BranchKey == model.BranchKey);
                }
                bookList = bookList.GroupBy(x => x.RowKey).Select(y => y.FirstOrDefault());
                if (model.SortBy != "")
                {
                    bookList = SortApplications(bookList, model.SortBy, model.SortOrder);
                }
                TotalRecords = bookList.Count();
                return bookList.OrderBy(Row => Row.RowKey).Skip(Skip).Take(Take).ToList<LibraryBookViewModel>();
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.LibraryBook, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<LibraryBookViewModel>();


            }
        }

        private IQueryable<LibraryBookViewModel> SortApplications(IQueryable<LibraryBookViewModel> Query, string SortName, string SortOrder)
        {

            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(LibraryBookViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<LibraryBookViewModel>(resultExpression);

        }
        
        public LibraryBookViewModel GetLibraryBookById(long? id)
        {
            LibraryBookViewModel model = new LibraryBookViewModel();
            model = dbContext.LibraryBooks.Select(row => new LibraryBookViewModel
            {
                RowKey = row.RowKey,
                BookName = row.BookName,
                BookName_Optional=row.BookName_Optional,
                AuthorKey = row.AuthorKey,
                BookCategoryKey = row.BookCategoryKey,
                BookIssueTypeKey = row.BookIssueTypeKey,
                LanguageKey = row.LanguageKey,
                PublisherKey = row.PublisherKey,
                RackKey = row.SubRack.RackKey,
                SubRackKey = row.SubRackKey ?? 0,
                BranchKey = row.BranchKey,
                CoverPhoto=row.CoverPhoto,
                CoverPhotoPath = row.CoverPhoto != null ? UrlConstants.LibraryBooksUrl + "/" + row.CoverPhoto : EduSuiteUIResources.DefaultPhotoUrl,

        }).Where(x => x.RowKey == id).FirstOrDefault();
            if (model == null)
            {
                model = new LibraryBookViewModel();
            }
            FillDropdownLists(model);
            return model;
        }

        public LibraryBookViewModel CreateLibraryBook(LibraryBookViewModel model)
        {

            var bookNameCheck = dbContext.LibraryBooks.Where(row => row.BookName.ToLower() == model.BookName.ToLower() && row.AuthorKey == model.AuthorKey).ToList();
            LibraryBook bookModel = new LibraryBook();

            FillDropdownLists(model);
            if (bookNameCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Book);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Int64 maxKey = dbContext.LibraryBooks.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    bookModel.RowKey = Convert.ToInt64(maxKey + 1);
                    bookModel.BookName = model.BookName;
                    bookModel.BookName_Optional = model.BookName_Optional;
                    bookModel.AuthorKey = model.AuthorKey;
                    bookModel.BookCategoryKey = model.BookCategoryKey;
                    bookModel.BookIssueTypeKey = model.BookIssueTypeKey;
                    bookModel.LanguageKey = model.LanguageKey;
                    bookModel.PublisherKey = model.PublisherKey;
                    bookModel.IsActive = true;
                    bookModel.SubRackKey = model.SubRackKey;
                    bookModel.BranchKey = model.BranchKey;
                    if (model.PhotoFile != null)
                    {
                        string Extension = Path.GetExtension(model.PhotoFile.FileName);
                        string FileName = model.BookName + Extension;
                        bookModel.CoverPhoto = FileName;
                    }
                    dbContext.LibraryBooks.Add(bookModel);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    model.RowKey = bookModel.RowKey;
                    ActivityLog.CreateActivityLog(MenuConstants.LibraryBook, ActionConstants.Add, DbConstants.LogType.Info, bookModel.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Book);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.LibraryBook, ActionConstants.Add, DbConstants.LogType.Error, bookModel.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }

        public LibraryBookViewModel UpdateLibraryBook(LibraryBookViewModel model)
        {
            var bookNameCheck = dbContext.LibraryBooks.Where(row => row.RowKey != model.RowKey && row.BookName.ToLower() == model.BookName.ToLower() && row.AuthorKey == model.AuthorKey).ToList();
            LibraryBook bookModel = new LibraryBook();

            FillDropdownLists(model);
            if (bookNameCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Book);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    bookModel = dbContext.LibraryBooks.SingleOrDefault(row => row.RowKey == model.RowKey);
                    bookModel.BookName = model.BookName;
                    bookModel.BookName_Optional = model.BookName_Optional;   
                    bookModel.AuthorKey = model.AuthorKey;
                    bookModel.BookCategoryKey = model.BookCategoryKey;
                    bookModel.BookIssueTypeKey = model.BookIssueTypeKey;
                    bookModel.LanguageKey = model.LanguageKey;
                    bookModel.PublisherKey = model.PublisherKey;
                    bookModel.SubRackKey = model.SubRackKey;
                    bookModel.BranchKey = model.BranchKey;
                    if (model.PhotoFile != null)
                    {
                        string Extension = Path.GetExtension(model.PhotoFile.FileName);
                        string FileName = model.BookName + Extension;
                        bookModel.CoverPhoto = FileName;
                    }
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.LibraryBook, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Book);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.LibraryBook, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }

        public LibraryBookViewModel DeleteLibraryBook(LibraryBookViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    LibraryBook book = dbContext.LibraryBooks.SingleOrDefault(row => row.RowKey == model.RowKey);
                    model.CoverPhotoPath = UrlConstants.LibraryBooksUrl + book.CoverPhoto;
                    IEnumerable<BookCopy> bookCopies = dbContext.BookCopies.Where(row => row.BookKey == model.RowKey);
                    bookCopies.ToList().ForEach(row => dbContext.BookCopies.Remove(row));

                    dbContext.LibraryBooks.Remove(book);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.LibraryBook, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Book);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.LibraryBook, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Book);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.LibraryBook, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        private void FillDropdownLists(LibraryBookViewModel model)
        {
            FillAuthors(model);
            FillBookCategories(model);
            FillBookIssueTypes(model);
            FillLanguages(model);
            FillPublishers(model);
            FillRacks(model);
            FillSubRack(model);
            FillBranch(model);

        }

        private void FillAuthors(LibraryBookViewModel model)
        {
            model.Authors = dbContext.Authors.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AuthorName
            }).ToList();
        }

        private void FillBookCategories(LibraryBookViewModel model)
        {
            model.BookCategories = dbContext.BookCategories.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BookCategoryName
            }).ToList();
        }

        private void FillBookIssueTypes(LibraryBookViewModel model)
        {
            model.BookIssueTypes = dbContext.BookIssueTypes.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BookIssueTypeName
            }).ToList();
        }

        private void FillLanguages(LibraryBookViewModel model)
        {
            model.Languages = dbContext.Languages.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.LanguageName
            }).ToList();
        }

        private void FillPublishers(LibraryBookViewModel model)
        {
            model.Publishers = dbContext.Publishers.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.PublisherName
            }).ToList();
        }

        public LibraryBookViewModel FillRacks(LibraryBookViewModel model)
        {

            model.Racks = dbContext.Racks.Where(x => x.BranchKey == model.BranchKey && x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.RackName
            }).ToList();
            return model;
        }

        public LibraryBookViewModel FillSubRack(LibraryBookViewModel model)
        {

            model.SubRacks = (from sr in dbContext.SubRacks
                              join r in dbContext.Racks on sr.RackKey equals r.RowKey
                              where (sr.RackKey == model.RackKey)
                              select new SelectListModel
                              {
                                  RowKey = sr.RowKey,
                                  Text = sr.SubRackName
                              }).ToList();

            return model;
        }

        public void FillBranch(LibraryBookViewModel model)
        {
            IQueryable<SelectListModel> BranchQuery = dbContext.vwBranchSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BranchName
            });

            Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
            if (Employee != null)
            {
                if (Employee.BranchAccess != null)
                {
                    List<long> Branches = Employee.BranchAccess.Split(',').Select(Int64.Parse).ToList();
                    model.Branches = BranchQuery.Where(row => Branches.Contains(row.RowKey)).ToList();
                    //model.BranchKey = Employee.BranchKey;
                }
                else
                {
                    model.Branches = BranchQuery.Where(x => x.RowKey == Employee.BranchKey).ToList();
                    //model.BranchKey = Employee.BranchKey;
                }
            }
            else
            {
                model.Branches = BranchQuery.ToList();
            }

            if (model.Branches.Count == 1)
            {
                long? branchkey = model.Branches.Select(x => x.RowKey).FirstOrDefault();
                model.BranchKey = Convert.ToInt16(branchkey);
            }
        }

    }
}
