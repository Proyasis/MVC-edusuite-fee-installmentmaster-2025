using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Data;
using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class BookService : IBookService
    {
        private EduSuiteDatabase dbContext;

        public BookService(EduSuiteDatabase objdb)
        {
            this.dbContext = objdb;
        }

        public BookViewModel GetBookById(BookViewModel model)
        {
            try
            {
                BookViewModel bookViewModel = new BookViewModel();
                bookViewModel = dbContext.Books.Select(row => new BookViewModel
                {
                    //RowKey = row.RowKey,
                    //BookCode = row.BookCode,
                    //BookName = row.BookName,
                    CourseTypeKey = row.Course.CourseTypeKey,
                    CourseKey = row.CourseKey,
                    AcademicTermKey = row.AcademicTermKey,
                    UniversityKey = row.UniversityKey,
                    //BookYear = row.BookYear,
                    //BookType = row.BookType,
                    //HasBook = row.HasBook,
                    //IsActive = row.IsActive
                }).Where(x => x.UniversityKey == model.UniversityKey && x.CourseKey == model.CourseKey && x.AcademicTermKey == model.AcademicTermKey).FirstOrDefault();
                if (bookViewModel == null)
                {
                    bookViewModel = new BookViewModel();
                    bookViewModel.AcademicTermKey = model.AcademicTermKey;
                    bookViewModel.CourseKey = model.CourseKey;
                    bookViewModel.UniversityKey = model.UniversityKey;
                    bookViewModel.CourseTypeKey = dbContext.Courses.Where(row => row.RowKey == model.CourseKey).Select(row => row.CourseTypeKey).FirstOrDefault();
                }
               FillDropDown(bookViewModel);
                FillBookDetailsViewModel(bookViewModel);
                return bookViewModel;
            }
            catch (Exception Ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Book, ActionConstants.View, DbConstants.LogType.Error, null, Ex.GetBaseException().Message);
                return new BookViewModel();
                
            }
        }


        public BookViewModel CreateBook(BookViewModel model)
        {

            FillDropDown(model);
            FillBookDetailsViewModel(model);
            using (var transaction = dbContext.Database.BeginTransaction())
            {

                try
                {
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                }
                catch (Exception Ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Book);
                    model.IsSuccessful = false;
                }
            }
            return model;
        }

        public BookViewModel UpdateBook(BookViewModel model)
        {
            Book BookModel = new Book();
            FillDropDown(model);
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    CreateBookDetail(model);
                    UpdateBookDetail(model);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                     ActivityLog.CreateActivityLog(MenuConstants.Book, (model.BookDetails.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Info, null, model.Message);
                }
                catch (Exception Ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Book);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.Book, (model.BookDetails.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, null, Ex.GetBaseException().Message);
                }
            }
            return model;
        }

        private void CreateBookDetail(BookViewModel model)
        {
            long MaxKey = dbContext.Books.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (BookDetailsViewModel modelDetail in model.BookDetails.Where(row => row.RowKey == 0))
            {
                Book BookModel = new Book();
                BookModel.RowKey = MaxKey + 1;
                BookModel.BookCode = modelDetail.BookCode;
                BookModel.BookName = modelDetail.BookName;
                BookModel.CourseKey = model.CourseKey;
                BookModel.UniversityKey = model.UniversityKey;
                BookModel.AcademicTermKey = model.AcademicTermKey;
                BookModel.BookYear = modelDetail.BookYearKey;
                BookModel.BookType = modelDetail.BookType;
                BookModel.HasBook = modelDetail.HasBook;
                BookModel.IsActive = modelDetail.IsActive;
                dbContext.Books.Add(BookModel);
                MaxKey++;
            }
        }

        private void UpdateBookDetail(BookViewModel model)
        {
            long MaxKey = dbContext.Books.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (BookDetailsViewModel modelDetail in model.BookDetails.Where(row => row.RowKey != 0))
            {
                Book BookModel = dbContext.Books.SingleOrDefault(p => p.RowKey == modelDetail.RowKey);
                BookModel.BookCode = modelDetail.BookCode;
                BookModel.BookName = modelDetail.BookName;
                BookModel.CourseKey = model.CourseKey;
                BookModel.UniversityKey = model.UniversityKey;
                BookModel.AcademicTermKey = model.AcademicTermKey;
                BookModel.BookYear = modelDetail.BookYearKey;
                BookModel.BookType = modelDetail.BookType;
                BookModel.HasBook = modelDetail.HasBook;
                BookModel.IsActive = modelDetail.IsActive;
            }
        }

        public BookViewModel DeleteBookAll(BookViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {


                    List<Book> BookList = dbContext.Books.Where(x => x.UniversityKey == model.UniversityKey && x.CourseKey == model.CourseKey
                        && x.AcademicTermKey == model.AcademicTermKey).ToList();


                    BookList.ForEach(Book => dbContext.Books.Remove(Book));
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Book, ActionConstants.Delete, DbConstants.LogType.Info, null, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Book);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.Book, ActionConstants.Delete, DbConstants.LogType.Debug, null, ex.GetBaseException().Message);

                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Book);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Book, ActionConstants.Delete, DbConstants.LogType.Debug, null, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public BookViewModel DeleteBook(long Id)
        {
            BookViewModel model = new BookViewModel();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Book Bookmodel = dbContext.Books.SingleOrDefault(p => p.RowKey == Id);
                    dbContext.Books.Remove(Bookmodel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Book, ActionConstants.Delete, DbConstants.LogType.Info, Id, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Book);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.Book, ActionConstants.Delete, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);

                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Book);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Book, ActionConstants.Delete, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);
                }
            }
            return model;
        }


        public List<BookViewModel> GetBook(string SearchText)
        {
            try
            {
                var BookList = (from p in dbContext.Books
                                orderby p.RowKey
                                where (p.BookCode.Contains(SearchText) || p.Course.CourseName.Contains(SearchText))
                                select new BookViewModel
                                {
                                    CourseKey = p.CourseKey,
                                    UniversityKey = p.UniversityKey,
                                    CourseName = p.Course.CourseName,
                                    AcademicTermKey = p.AcademicTermKey,
                                    UniversityName = p.UniversityMaster.UniversityMasterName,
                                    AcademicTermName = p.AcademicTerm.AcademicTermName,


                                }).ToList();
                return BookList.GroupBy(x => new { x.CourseKey, x.UniversityKey, x.AcademicTermKey }).Select(y => y.First()).ToList<BookViewModel>();

            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Book, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<BookViewModel>();
                
            }
        }

        public BookViewModel CheckBookCodeExist(BookDetailsViewModel model)
        {
            BookViewModel bookViewModel = new BookViewModel();
            if (dbContext.Books.Where(x => x.BookCode.ToLower() == model.BookCode.ToLower() && x.RowKey != model.RowKey).Any())
            {
                bookViewModel.IsSuccessful = false;
            }
            else
            {
                bookViewModel.IsSuccessful = true;
            }
            return bookViewModel;
        }

        private void FillDropDown(BookViewModel model)
        {
            FillCourseType(model);
            FillSyllabus(model);
            FillCourse(model);
            FillUniversity(model);
            FillBookYear(model);
        }

        private void FillBookDetailsViewModel(BookViewModel model)
        {
            Book m_BooksModel = new Book();

            model.BookDetails = (from row in dbContext.Books.Where(x => x.UniversityKey == model.UniversityKey && x.CourseKey == model.CourseKey && x.AcademicTermKey == model.AcademicTermKey)
                                 select new BookDetailsViewModel
                                 {
                                     RowKey = row.RowKey,
                                     BookCode = row.BookCode,
                                     BookName = row.BookName,
                                     BookYearKey = row.BookYear,
                                     BookType = row.BookType,
                                     HasBook = row.HasBook,
                                     IsActive = row.IsActive
                                 }).ToList();
            if (model.BookDetails.Count == 0)
            {
                model.BookDetails.Add(new BookDetailsViewModel());
            }

        }


        private void FillCourseType(BookViewModel model)
        {
            model.CourseTypes = dbContext.VwCourseTypeSelectActiveOnlies.Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.CourseTypeName
            }).ToList();
        }
        public BookViewModel FillCourse(BookViewModel model)
        {
            model.Courses = dbContext.VwCourseSelectActiveOnlies.Where(x => x.CourseTypeKey == model.CourseTypeKey).Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.CourseName
            }).ToList();
            return model;
        }
        public BookViewModel FillUniversity(BookViewModel model)
        {
            model.Universities = dbContext.VwUniversitySelectActiveOnlies.Where(x => x.CourseKey == model.CourseKey && x.AcademicTermKey == model.AcademicTermKey).Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.UniversityMasterName
            }).ToList();
            return model;
        }

        private void FillSyllabus(BookViewModel model)
        {
            model.AcademicTerms = dbContext.AcademicTerms.Select(x => new SelectListModel
                {
                    RowKey = x.RowKey,
                    Text = x.AcademicTermName,

                }).ToList();
        }

        private void FillBookYear(BookViewModel model)
        {
            model.BookYears = dbContext.fnStudentYear(model.AcademicTermKey).Select(x => new SelectListModel
            {
                RowKey = x.RowKey ?? 0,
                Text = x.YearName
            }).ToList();

            var CourseDuration = dbContext.Courses.Where(row => row.RowKey == model.CourseKey).Select(row => row.CourseDuration).FirstOrDefault();
            if (CourseDuration != 0)
            {
                CourseDuration = Convert.ToBoolean(model.AcademicTermKey) ? CourseDuration / 6 : CourseDuration / 12;

                if (CourseDuration < 1)
                {
                    model.BookYears = model.BookYears.Where(row => row.RowKey == 0).ToList();

                }
                else
                {
                    model.BookYears = model.BookYears.Where(row => row.RowKey <= CourseDuration && row.RowKey > 0).ToList();
                }
            }

        }



    }
}
