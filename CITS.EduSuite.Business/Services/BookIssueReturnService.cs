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
    public class BookIssueReturnService : IBookIssueReturnService
    {
        private EduSuiteDatabase dbContext;

        public BookIssueReturnService(EduSuiteDatabase objDb)
        {
            dbContext = objDb;
        }

        public List<BookIssueReturnMasterViewModel> GetBookIssueReturn(BookIssueReturnMasterViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;

                IQueryable<BookIssueReturnMasterViewModel> BookIssueReturnList = (from BIT in dbContext.BookIssueReturnMasters
                                                                                  join MP in dbContext.MemberPlanDetails on BIT.MemberRegistrationKey equals MP.RowKey into MPJ
                                                                                  from MP in MPJ.DefaultIfEmpty()
                                                                                  join AP in dbContext.Applications on new { MP.ApplicationTypeKey, MP.ApplicationKey } equals new { ApplicationTypeKey = DbConstants.ApplicationType.Student, ApplicationKey = AP.RowKey } into APJ
                                                                                  from AP in APJ.DefaultIfEmpty()
                                                                                  join EP in dbContext.Employees on new { MP.ApplicationTypeKey, MP.ApplicationKey } equals new { ApplicationTypeKey = DbConstants.ApplicationType.Staff, ApplicationKey = EP.RowKey } into EPJ
                                                                                  from EP in EPJ.DefaultIfEmpty()
                                                                                  join MR in dbContext.MemberRegistrations on new { MP.ApplicationTypeKey, MP.ApplicationKey } equals new { ApplicationTypeKey = DbConstants.ApplicationType.Other, ApplicationKey = MR.RowKey } into MRJ
                                                                                  from MR in MRJ.DefaultIfEmpty()
                                                                                  where MP.ApplicationTypeKey == DbConstants.ApplicationType.Student ? (AP.StudentName.Contains(model.SearchText)) : MP.ApplicationTypeKey == DbConstants.ApplicationType.Staff ? ((EP.FirstName + " " + EP.LastName).Contains(model.SearchText)) : ((MR.MemberFirstName + " " + MR.MemberLastName).Contains(model.SearchText))
                                                                                  orderby (BIT.RowKey)

                                                                                  select new BookIssueReturnMasterViewModel
                                                                                  {
                                                                                      RowKey = BIT.RowKey,
                                                                                      CardId = MP.CardId,
                                                                                      MemberName = MP.ApplicationTypeKey == DbConstants.ApplicationType.Student ? AP.StudentName : MP.ApplicationTypeKey == DbConstants.ApplicationType.Staff ? (EP.FirstName + " " + EP.LastName) : (MR.MemberFirstName + " " + MR.MemberLastName),
                                                                                      ApplicationTypeName = BIT.MemberPlanDetail.ApplicationType.ApplicationTypeName,
                                                                                      BranchKey = BIT.MemberPlanDetail.BranchKey,
                                                                                      ApplicationTypeKey = BIT.MemberPlanDetail.ApplicationTypeKey,
                                                                                      MemberTypeName = BIT.MemberPlanDetail.MemberType.MemberTypeName,
                                                                                      BorrowerTypeName = BIT.MemberPlanDetail.BorrowerType.BorrowerTypeName,
                                                                                      IssueDate = BIT.IssueDate,
                                                                                      DueDate = BIT.DueDate,
                                                                                      NumberofBooks = BIT.BookIssueReturnDetails.Count(),
                                                                                      //NumberofBooks = dbContext.BookIssueReturnDetails.Where(x => x.BookIssueReturnMaster.MemberPlanDetail.CardId == MP.CardId && x.BookIssueReturnMaster.IssueDate == BIT.IssueDate && x.ReturnDate == null).Count(),
                                                                                      NumberofReturnBooks = BIT.BookIssueReturnDetails.Count(y => y.ReturnDate != null),

                                                                                  });

                if (model.BranchKey != 0)
                {
                    BookIssueReturnList = BookIssueReturnList.Where(x => x.BranchKey == model.BranchKey);
                }
                if (model.ApplicationTypeKey != 0)
                {
                    BookIssueReturnList = BookIssueReturnList.Where(x => x.ApplicationTypeKey == model.ApplicationTypeKey);
                }
                BookIssueReturnList = BookIssueReturnList.GroupBy(x => x.RowKey).Select(y => y.FirstOrDefault());
                if (model.SortBy != "")
                {
                    BookIssueReturnList = SortApplications(BookIssueReturnList, model.SortBy, model.SortOrder);
                }
                TotalRecords = BookIssueReturnList.Count();
                return BookIssueReturnList.OrderBy(Row => Row.MemberName).Skip(Skip).Take(Take).ToList<BookIssueReturnMasterViewModel>();
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.BookIssueReturn, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<BookIssueReturnMasterViewModel>();

            }
        }

        private IQueryable<BookIssueReturnMasterViewModel> SortApplications(IQueryable<BookIssueReturnMasterViewModel> Query, string SortName, string SortOrder)
        {

            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(BookIssueReturnMasterViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<BookIssueReturnMasterViewModel>(resultExpression);

        }


        public BookIssueReturnMasterViewModel GetBookIssueReturnById(BookIssueReturnMasterViewModel model)
        {
            BookIssueReturnMasterViewModel objmodel = new BookIssueReturnMasterViewModel();
            objmodel = dbContext.BookIssueReturnMasters.Select(row => new BookIssueReturnMasterViewModel
            {
                RowKey = row.RowKey,
                IssueDate = row.IssueDate,
                DueDate = row.DueDate,
                CardId = row.MemberPlanDetail.CardId
            }).Where(x => x.RowKey == model.RowKey).FirstOrDefault();
            if (objmodel == null)
            {
                objmodel = new BookIssueReturnMasterViewModel();
                objmodel.CardId = model.CardId;
                objmodel.IssueDate = model.IssueDate;
            }
            if (objmodel.IssueDate != null && objmodel.CardId != null)
            {
                FillBookCount(objmodel);
            }
            FillBookIssueReturnDetails(objmodel);
            FillDropdown(objmodel.BookIssueReturnDetails);
            return objmodel;
        }

        public BookIssueReturnMasterViewModel GetBookIssueByValues(BookIssueReturnMasterViewModel model)
        {
            BookIssueReturnMasterViewModel Tempmodel = new BookIssueReturnMasterViewModel();
            Tempmodel.CardId = model.CardId;
            Tempmodel.IssueDate = model.IssueDate;
            model = dbContext.BookIssueReturnMasters.Select(row => new BookIssueReturnMasterViewModel
            {
                RowKey = row.RowKey,
                IssueDate = row.IssueDate,
                DueDate = row.DueDate,
                CardId = row.MemberPlanDetail.CardId
            }).Where(x => x.CardId == model.CardId && x.IssueDate == model.IssueDate).FirstOrDefault();

            if (model == null)
            {
                model = Tempmodel;
            }
            FillBookCount(model);
            FillBookIssueReturnDetails(model);
            FillDropdown(model.BookIssueReturnDetails);
            return model;
        }

        private void FillBookIssueReturnDetails(BookIssueReturnMasterViewModel model)
        {
            model.BookIssueReturnDetails = dbContext.BookIssueReturnDetails.Where(row => row.BookIssueReturnMasterKey == model.RowKey && row.ReturnDate == null).Select(row => new BookIssueReturnDetailsViewModel
            {
                RowKey = row.RowKey,
                BookCopyKey = row.BookCopyKey,
                BookKey = row.BookCopy.LibraryBook.RowKey,
                BookName = row.BookCopy.LibraryBook.BookName,
                BookCopySlNo = row.BookCopy.BookCopySlNo,
                BookEdition = row.BookCopy.BookEdition,
                BookCategoryKey = row.BookCopy.LibraryBook.BookCategory.RowKey,
                Remark = row.Remark,
                ReturnDate = row.ReturnDate,
                BookStatusKey = row.BookStatusKey,
                IfAnyFine = row.IfAnyFine

            }).ToList();
            //Dummy Row creation if no value exist
            if (model.BookIssueReturnDetails.Count == 0)
            {
                model.BookIssueReturnDetails.Add(new BookIssueReturnDetailsViewModel());
            }

        }

        public BookIssueReturnMasterViewModel CreateBookIssue(BookIssueReturnMasterViewModel model)
        {

            BookIssueReturnMaster BookIssueReturnModel = new BookIssueReturnMaster();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Int32 maxKey = dbContext.BookIssueReturnMasters.Select(x => x.RowKey).DefaultIfEmpty().Max();
                    BookIssueReturnModel.RowKey = Convert.ToInt32(maxKey + 1);
                    BookIssueReturnModel.IssueDate = Convert.ToDateTime(model.IssueDate);
                    var MemberRegistrationList = dbContext.MemberPlanDetails.SingleOrDefault(x => x.CardId == model.CardId);
                    BookIssueReturnModel.MemberRegistrationKey = MemberRegistrationList.RowKey;
                    BookIssueReturnModel.DueDate = Convert.ToDateTime(model.IssueDate).AddDays(Convert.ToDouble(MemberRegistrationList.MemberType.ReturnPeriod));
                    CreateBookIssueDetails(model.BookIssueReturnDetails.Where(x => x.RowKey == 0 || x.RowKey == null).ToList(), BookIssueReturnModel.RowKey);

                    dbContext.BookIssueReturnMasters.Add(BookIssueReturnModel);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.BookIssueReturn, ActionConstants.Add, DbConstants.LogType.Info, BookIssueReturnModel.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.BookIssue);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.BookIssueReturn, ActionConstants.Add, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                }

            }
            return model;
        }

        public BookIssueReturnMasterViewModel UpdateBookIssue(BookIssueReturnMasterViewModel model)
        {

            BookIssueReturnMaster BookIssueReturnModel = new BookIssueReturnMaster();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    BookIssueReturnModel = dbContext.BookIssueReturnMasters.SingleOrDefault(x => x.RowKey == model.RowKey);
                    BookIssueReturnModel.IssueDate = Convert.ToDateTime(model.IssueDate);
                    BookIssueReturnModel.DueDate = Convert.ToDateTime(model.IssueDate).AddDays(Convert.ToDouble(BookIssueReturnModel.MemberPlanDetail.MemberType.ReturnPeriod));
                    var MemberRegistrationList = dbContext.MemberPlanDetails.SingleOrDefault(x => x.CardId == model.CardId);
                    BookIssueReturnModel.MemberRegistrationKey = MemberRegistrationList.RowKey;
                    CreateBookIssueDetails(model.BookIssueReturnDetails.Where(x => x.RowKey == 0 || x.RowKey == null).ToList(), BookIssueReturnModel.RowKey);
                    UpdateBookIssueDetails(model.BookIssueReturnDetails.Where(x => x.RowKey != 0 && x.RowKey != null).ToList(), BookIssueReturnModel.RowKey);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.BookIssueReturn, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.BookIssue);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.BookIssueReturn, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }

            }
            return model;
        }

        private void CreateBookIssueDetails(List<BookIssueReturnDetailsViewModel> modelList, Int32 Id)
        {
            Int64 maxKey = dbContext.BookIssueReturnDetails.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (BookIssueReturnDetailsViewModel model in modelList)
            {
                BookIssueReturnDetail BookIssueReturnModel = new BookIssueReturnDetail();

                BookIssueReturnModel.RowKey = Convert.ToInt32(maxKey + 1);
                BookIssueReturnModel.Remark = model.Remark;
                BookIssueReturnModel.BookCopyKey = model.BookCopyKey;
                BookIssueReturnModel.BookIssueReturnMasterKey = Id;

                dbContext.BookIssueReturnDetails.Add(BookIssueReturnModel);
                BookCopy BookCopyModel = new BookCopy();
                BookCopyModel = dbContext.BookCopies.SingleOrDefault(row => row.RowKey == model.BookCopyKey);
                BookCopyModel.IsIssued = true;
                maxKey++;
            }
        }

        private void UpdateBookIssueDetails(List<BookIssueReturnDetailsViewModel> modelList, Int64 Id)
        {
            foreach (BookIssueReturnDetailsViewModel model in modelList)
            {
                BookIssueReturnDetail BookIssueReturnModel = new BookIssueReturnDetail();
                BookCopy BookCopyModel = new BookCopy();
                BookIssueReturnModel = dbContext.BookIssueReturnDetails.SingleOrDefault(x => x.RowKey == model.RowKey);
                BookIssueReturnModel.Remark = model.Remark;
                if (BookIssueReturnModel.BookCopyKey != model.BookCopyKey)
                {
                    BookCopyModel = dbContext.BookCopies.SingleOrDefault(row => row.RowKey == BookIssueReturnModel.BookCopyKey);
                    BookCopyModel.IsIssued = false;
                }
                BookIssueReturnModel.BookCopyKey = model.BookCopyKey;
                BookCopyModel = dbContext.BookCopies.SingleOrDefault(row => row.RowKey == model.BookCopyKey);
                BookCopyModel.IsIssued = true;
            }
        }

        public BookIssueReturnMasterViewModel UpdateBookReturnDetails(BookIssueReturnMasterViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    BookIssueReturnDetail BookIssueReturnModel = new BookIssueReturnDetail();
                    BookCopy BookCopyModel = new BookCopy();
                    foreach (BookIssueReturnDetailsViewModel DetailModelList in model.BookIssueReturnDetails)
                    {
                        BookIssueReturnModel = dbContext.BookIssueReturnDetails.SingleOrDefault(x => x.RowKey == DetailModelList.RowKey);
                        BookIssueReturnModel.ReturnDate = DetailModelList.ReturnDate;
                        BookIssueReturnModel.IfAnyFine = DetailModelList.IfAnyFine;
                        BookIssueReturnModel.BookStatusKey = DetailModelList.BookStatusKey;
                        BookCopyModel = dbContext.BookCopies.SingleOrDefault(row => row.RowKey == DetailModelList.BookCopyKey);
                        BookCopyModel.IsIssued = false;
                        BookCopyModel.BookStatusKey = DetailModelList.BookStatusKey;
                    }
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.BookIssueReturn, ActionConstants.Edit, DbConstants.LogType.Info, null, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.BookIssueReturn);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.BookIssueReturn, ActionConstants.Edit, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public BookIssueReturnMasterViewModel CheckCardIdExists(BookIssueReturnMasterViewModel model)
        {
            if (dbContext.MemberPlanDetails.Where(x => x.CardId.ToLower() == model.CardId.ToLower()).Any())
            {
                FillBookCount(model);
                model.Message = "";
                model.IsSuccessful = true;
            }
            else
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.CardId);
                model.IsSuccessful = false;
            }
            return model;
        }

        private void FillBookCount(BookIssueReturnMasterViewModel model)
        {
            if (dbContext.MemberPlanDetails.Where(x => x.CardId.ToLower() == model.CardId.ToLower()).Any())
            {
                var MemberList = dbContext.MemberPlanDetails.SingleOrDefault(x => x.CardId == model.CardId);
                var BookTakeCount = dbContext.BookIssueReturnDetails.Where(x => x.BookIssueReturnMaster.MemberPlanDetail.CardId == model.CardId && x.ReturnDate == null).Count();
                var BookTakeCountDay = dbContext.BookIssueReturnDetails.Where(x => x.BookIssueReturnMaster.MemberPlanDetail.CardId == model.CardId && x.BookIssueReturnMaster.IssueDate == model.IssueDate && x.ReturnDate == null).Count();
                var BookDayCount = MemberList.BorrowerType.NoOfBookIssueAtATime;
                if (MemberList != null)
                {
                    model.NumberofBooksDay = BookDayCount;
                    model.NumberofBooks = MemberList.MemberType.NumberOfBooksAllowed;
                    model.NumberofBooksRemain = (MemberList.MemberType.NumberOfBooksAllowed) - BookTakeCount;
                    DateTime Issue = Convert.ToDateTime(model.IssueDate);
                    if (model.NumberofBooksRemain == 0)
                    {
                        model.NumberofBooksDayRemain = 0;
                    }
                    else if (dbContext.BookIssueReturnMasters.Where(x => x.IssueDate == Issue && x.MemberPlanDetail.CardId == model.CardId).Any())
                    {
                        var BookCountDayRemain = BookDayCount - BookTakeCountDay;
                        var Sum = BookCountDayRemain + BookTakeCount;
                        if (Sum > model.NumberofBooks)
                        {
                            model.NumberofBooksDayRemain = BookCountDayRemain - (Sum - model.NumberofBooks);
                        }
                        else
                        {
                            model.NumberofBooksDayRemain = BookCountDayRemain;
                        }
                    }
                    else
                    {
                        if (model.NumberofBooksRemain >= BookDayCount)
                        {
                            model.NumberofBooksDayRemain = MemberList.BorrowerType.NoOfBookIssueAtATime;
                        }
                        else
                        {
                            model.NumberofBooksDayRemain = model.NumberofBooksRemain;
                        }
                    }
                }
            }
        }

        private void FillDropdown(List<BookIssueReturnDetailsViewModel> modelList)
        {
            foreach (BookIssueReturnDetailsViewModel model in modelList)
            {
                FillBookCategory(model);
                FillBook(model);
                FillBookCopy(model);
                FillBookStatus(model);
            }
        }

        private void FillBookCategory(BookIssueReturnDetailsViewModel model)
        {
            model.BookCategory = dbContext.BookCategories.Select(x => new SelectListModel
            {
                Text = x.BookCategoryName,
                RowKey = x.RowKey
            }).ToList();
        }

        public BookIssueReturnDetailsViewModel FillBook(BookIssueReturnDetailsViewModel model)
        {

            IQueryable<LibraryBookViewModel> LibraryBooks = dbContext.LibraryBooks.Where(x => x.IsActive).Select(row => new LibraryBookViewModel
            {
                RowKey = row.RowKey,
                BookName = row.BookName,
                BookName_Optional = row.BookName_Optional,
                BookCategoryKey = row.BookCategoryKey,
            });

            if (model.BookCategoryKey != 0 && model.BookCategoryKey != null)
            {
                LibraryBooks = LibraryBooks.Where(x => x.BookCategoryKey == model.BookCategoryKey);
            }

            model.Book = LibraryBooks.Select(x => new SelectListModel
            {
                Text = x.BookName_Optional != null ? x.BookName_Optional + EduSuiteUIResources.OpenBracketWithSpace + x.BookName + EduSuiteUIResources.ClosingBracketWithSpace : x.BookName,
                RowKey = x.RowKey,
                IntValue = x.BookCategoryKey
            }).ToList();
            return model;
        }

        public BookIssueReturnDetailsViewModel FillBookCopy(BookIssueReturnDetailsViewModel model)
        {
            //long BookCopyId = 0;
            //if (model.RowKey != null && model.RowKey != 0)
            //{
            //    var BookCopy = dbContext.BookIssueReturnDetails.SingleOrDefault(x => x.RowKey == model.RowKey);
            //    long BookKey = BookCopy.BookCopy.LibraryBook.RowKey;
            //    if (BookKey == model.BookKey)
            //    {
            //        BookCopyId = BookCopy.BookCopyKey;
            //    }

            //}

            IQueryable<BookCopyViewModel> LibraryBookCopies = dbContext.BookCopies.Where(x => x.IsActive == true && x.BookStatu.CanIssue == true).Select(row => new BookCopyViewModel
            {
                RowKey = row.RowKey,
                BookKey = row.BookKey,
                BookCopySlNo = row.BookCopySlNo + EduSuiteUIResources.OpenBracketWithSpace + row.LibraryBook.BookName + EduSuiteUIResources.ClosingBracketWithSpace,
                IsIssued = row.IsIssued
            });


            if (model.BookKey != 0 && model.BookKey != null)
            {
                LibraryBookCopies = LibraryBookCopies.Where(x => x.BookKey == model.BookKey);
            }


            if (model.RowKey == 0 || model.RowKey == null)
            {
                LibraryBookCopies = LibraryBookCopies.Where(x => x.IsIssued == false);
            }


            model.BookCopy = LibraryBookCopies.Select(x => new SelectListModel
            {
                Text = x.BookCopySlNo,
                RowKey = x.RowKey
            }).ToList();

            return model;
        }

        private void FillBookStatus(BookIssueReturnDetailsViewModel model)
        {
            model.BookStatus = dbContext.BookStatus.Select(x => new SelectListModel
            {
                Text = x.BookStatusName,
                RowKey = x.RowKey
            }).ToList();
        }

        public BookIssueReturnMasterViewModel DeleteBookIssueReturn(BookIssueReturnMasterViewModel model)
        {
            BookIssueReturnMaster BookIssueReturnMasterModel = new BookIssueReturnMaster();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    BookCopy BookCopyModel = new BookCopy();

                    List<BookIssueReturnDetail> BookIssueReturnDetailsModel = dbContext.BookIssueReturnDetails.Where(row => row.BookIssueReturnMasterKey == model.RowKey).ToList();

                    foreach (BookIssueReturnDetail DetailModel in BookIssueReturnDetailsModel)
                    {
                        BookCopyModel = dbContext.BookCopies.SingleOrDefault(row => row.RowKey == DetailModel.BookCopyKey);
                        BookCopyModel.IsIssued = false;
                        dbContext.BookIssueReturnDetails.Remove(DetailModel);
                    }

                    BookIssueReturnMasterModel = dbContext.BookIssueReturnMasters.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.BookIssueReturnMasters.Remove(BookIssueReturnMasterModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.BookIssueReturn, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.BookIssueReturn);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.BookIssueReturn, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.BookIssueReturn);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.BookIssueReturn, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public BookIssueReturnMasterViewModel DeleteBookIssueReturnDetails(BookIssueReturnDetailsViewModel model)
        {

            BookIssueReturnMasterViewModel BookIssueReturnMasterModel = new BookIssueReturnMasterViewModel();
            FillDropdown(BookIssueReturnMasterModel.BookIssueReturnDetails);

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    BookCopy BookCopyModel = new BookCopy();
                    BookIssueReturnDetail BookIssueReturnDetailModel = dbContext.BookIssueReturnDetails.SingleOrDefault(row => row.RowKey == model.RowKey);
                    BookCopyModel = dbContext.BookCopies.SingleOrDefault(row => row.RowKey == BookIssueReturnDetailModel.BookCopyKey);
                    BookCopyModel.IsIssued = false;
                    dbContext.BookIssueReturnDetails.Remove(BookIssueReturnDetailModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    BookIssueReturnMasterModel.Message = EduSuiteUIResources.Success;
                    BookIssueReturnMasterModel.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.BookIssueReturn, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, BookIssueReturnMasterModel.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        BookIssueReturnMasterModel.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.BookIssueReturn);
                        BookIssueReturnMasterModel.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.BookIssueReturn, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    BookIssueReturnMasterModel.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.BookIssueReturn);
                    BookIssueReturnMasterModel.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.BookIssueReturn, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return BookIssueReturnMasterModel;
        }

        public BookIssueReturnDetailsViewModel FillIfAnyFine(BookIssueReturnDetailsViewModel model)
        {
            var BookIssueReturnList = dbContext.BookIssueReturnDetails.SingleOrDefault(row => row.RowKey == model.RowKey);
            if (BookIssueReturnList != null)
            {
                DateTime DueDate = Convert.ToDateTime(BookIssueReturnList.BookIssueReturnMaster.DueDate);
                int DayDifferent = Convert.ToInt32((Convert.ToDateTime(model.ReturnDate) - Convert.ToDateTime(DueDate)).TotalDays);
                decimal Fine = BookIssueReturnList.BookIssueReturnMaster.MemberPlanDetail.MemberType.LateFeePerDay;
                if (DayDifferent > 0)
                {
                    model.IfAnyFine = Convert.ToDecimal(DayDifferent * Fine);
                }
                else
                {
                    model.IfAnyFine = 0;
                }
            }
            return model;
        }

        public void FillBranches(BookIssueReturnMasterViewModel model)
        {

            IQueryable<SelectListModel> BranchQuery = dbContext.vwBranchSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BranchName
            });

            if (!DbConstants.Role.AdminUserTypes.Contains(DbConstants.User.RoleKey))
            {
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
                    if (model.Branches.Count == 1)
                    {
                        long? branchkey = model.Branches.Select(x => x.RowKey).FirstOrDefault();
                        model.BranchKey = Convert.ToInt16(branchkey);

                    }
                }
            }
            else
            {

                model.Branches = BranchQuery.ToList();
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
                if (model.Branches.Count == 1)
                {
                    long? branchkey = model.Branches.Select(x => x.RowKey).FirstOrDefault();
                    model.BranchKey = Convert.ToInt16(branchkey);

                }
            }
        }
        private void FillApplicationType(BookIssueReturnMasterViewModel model)
        {
            model.ApplicationTypes = dbContext.ApplicationTypes.Where(x => x.RowKey != DbConstants.ApplicationType.Other).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ApplicationTypeName
            }).ToList();
        }

        public void FillDropdownLists(BookIssueReturnMasterViewModel model)
        {
            FillApplicationType(model);
            FillBranches(model);
        }
    }
}
