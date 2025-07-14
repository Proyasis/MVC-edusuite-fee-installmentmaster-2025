using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;

namespace CITS.EduSuite.Business.Services
{
    public class LibraryReportService : ILibraryReportService
    {
        private EduSuiteDatabase dbContext;

        public LibraryReportService(EduSuiteDatabase objDB)
        {
            dbContext = objDB;
        }

        #region Book Summary


        public List<LibraryReportViewModel> GetBookSummary(LibraryReportViewModel model)
        {
            ObjectParameter objTotalRecords = new ObjectParameter("TotalRecords", typeof(Int64));

            if (model.sidx != "")
            {
                model.sidx = model.sidx + " " + model.sord;
            }

            var BookSummary = (
                                         from Application in dbContext.Sp_LibraryBookSummary
                                             (
                                                String.Join(",", model.AuthorKeys),
                                               String.Join(",", model.BookCategoryKeys),
                                               String.Join(",", model.BookIssueTypeKeys),
                                               String.Join(",", model.LanguageKeys),
                                               String.Join(",", model.BranchKeys),
                                               String.Join(",", model.PublisherKeys),
                                               String.Join(",", model.RackKeys),
                                               String.Join(",", model.SubRackKeys),
                                                model.SearchAnyText.VerifyData(),

                                                (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""),
                                                (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""),

                                                model.page,
                                                model.rows,
                                                model.sidx,
                                                objTotalRecords
                                             )
                                         select new LibraryReportViewModel
                                         {
                                             RowKey = Application.RowKey
                                                ,
                                             BookName = Application.BookName
                                                ,
                                             BookName_Optional = Application.BookName_Optional
                                                ,
                                             BookCode = Application.BookCode
                                                ,
                                             CoverPhoto = Application.CoverPhoto
                                                ,
                                             AuthorName = Application.AuthorName
                                                ,
                                             BookCategoryName = Application.BookCategoryName
                                                ,
                                             BookIssueTypeName = Application.BookIssueTypeName
                                                ,
                                             LanguageName = Application.LanguageName
                                                ,
                                             BranchName = Application.BranchName
                                                ,
                                             PublisherName = Application.PublisherName
                                                ,
                                             SubRackName = Application.SubRackName
                                                ,
                                             RackName = Application.RackName
                                                ,
                                             DateAdded = Application.DateAdded
                                                ,
                                             TotalCopy = Application.TotalCopy
                                                ,
                                             IssuedCount = Application.IssuedCount
                                                ,
                                             NotIssuedCount = Application.NotIssuedCount
                                                ,
                                             DamageCount = Application.DamageCount
                                                ,
                                             CreatedBy = Application.AppUserName
                                                ,
                                             Activetext = Application.IsActive == true ? EduSuiteUIResources.Yes : EduSuiteUIResources.No


                                         }).ToList();



            model.TotalRecords = objTotalRecords.Value != DBNull.Value ? Convert.ToInt64(objTotalRecords.Value) : 0;

            return BookSummary;
        }

        //public List<MembershipSummaryReportViewModel> GetMembershipSummaryReport(MembershipSummaryReportViewModel model)
        //{


        //    ObjectParameter objTotalRecords = new ObjectParameter("TotalRecords", typeof(Int64));
        //    if (model.sidx != "")
        //    {
        //        model.sidx = model.sidx + " " + model.sord;
        //    }

        //    var MembershipSummaryReports = (
        //                                 from Report in dbContext.spMembershipSummaryReport
        //                                     (

        //                                       model.MemberShipName, model.PhoneNo, model.CardId, model.MemberTypeKey, model.BorrowerTypeKey, (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""),
        //                                      (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""),


        //                                        model.page,
        //                                        model.rows,
        //                                        model.sidx,
        //                                         objTotalRecords
        //                                     )
        //                                 select new MembershipSummaryReportViewModel
        //                                 {


        //                                     RowKey = Report.RowKey,
        //                                     MemberShipName = Report.MemberShipName,
        //                                     MemberDOB = Report.MemberDOB,
        //                                     Gender = Report.Gender,
        //                                     IdentificationDocument = Report.IdentificationDocument,
        //                                     Address = Report.Address,
        //                                     PhoneNo = Report.PhoneNo,
        //                                     EmailAddress = Report.EmailAddress,
        //                                     RegistrationDate = Report.RegistrationDate,
        //                                     RegistrationFee = Report.RegistrationFee,
        //                                     MemberTypeName = Report.MemberTypeName,
        //                                     MemberShipFee = Report.MemberShipFee,
        //                                     NumberOfBooksAllowed = Report.NumberOfBooksAllowed,
        //                                     ReturnPeriod = Report.ReturnPeriod,
        //                                     LateFeePerDay = Report.LateFeePerDay,
        //                                     BorrowerTypeName = Report.BorrowerTypeName,
        //                                     NoOfBookIssueAtATime = Report.NoOfBookIssueAtATime,
        //                                     IsBlockMember = Report.IsBlockMember,
        //                                     CardId = Report.CardId,
        //                                     ApplicationTypeKey = Report.ApplicationTypeKey





        //                                 }).ToList();



        //    model.TotalRecords = objTotalRecords.Value != DBNull.Value ? Convert.ToInt64(objTotalRecords.Value) : 0;
        //    //StudentSummaryReports = StudentSummaryReports.OrderByDescending(Row => Row.DateAdded).Skip(skip).Take(Take).ToList();



        //    return MembershipSummaryReports;
        //}


        //public List<BookCopyListViewModel> GetBookCopyListReport(BookCopyListViewModel model)
        //{


        //    ObjectParameter objTotalRecords = new ObjectParameter("TotalRecords", typeof(Int64));
        //    if (model.sidx != "")
        //    {
        //        model.sidx = model.sidx + " " + model.sord;
        //    }

        //    var BookCopyListReports = (
        //                                 from Report in dbContext.spBookCopyListReport
        //                                     (

        //                                       // (model.BookKey != null ? Convert.ToString(model.BookKey) : ""),
        //                                       model.BookKey,
        //                                     //(model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""),
        //                                     // (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""),

        //                                        model.page,
        //                                        model.rows,
        //                                        model.sidx,
        //                                         objTotalRecords
        //                                     )
        //                                 select new BookCopyListViewModel
        //                                 {


        //                                     RowKey = Report.RowKey,
        //                                     BookCopySlNo = Report.BookCopySlNo,
        //                                      ISBN = Report.ISBN,
        //                                     BookBarCode = Report.BookBarCode,

        //                                     BookEdition = Report.BookEdition,

        //                                     BookPrintYear = Report.BookPrintYear
        //                                          ,
        //                                     NoOfPages = Report.NoOfPages
        //                                        ,
        //                                     BookPrice = Report.BookPrice
        //                                        ,
        //                                     FineAmount = Report.FineAmount
        //                                     ,
        //                                     BookStatus = Report.BookStatus,

        //                                     IssueStatus=Report.IssueStatus






        //                                 }).ToList();



        //    model.TotalRecords = objTotalRecords.Value != DBNull.Value ? Convert.ToInt64(objTotalRecords.Value) : 0;
        //    //StudentSummaryReports = StudentSummaryReports.OrderByDescending(Row => Row.DateAdded).Skip(skip).Take(Take).ToList();



        //    return BookCopyListReports;
        //}

        //public List<BookIssueReturnReportViewModel> GetBookIssueReturnSummaryReport(BookIssueReturnReportViewModel model)
        //{


        //    ObjectParameter objTotalRecords = new ObjectParameter("TotalRecords", typeof(Int64));
        //    if (model.sidx != "")
        //    {
        //        model.sidx = model.sidx + " " + model.sord;
        //    }


        //    var BookIssueReturnSummaryReports = (
        //                                from Report in dbContext.spBookIssueReturnReport
        //                                    (

        //                                       model.CardId, model.BookName, model.MemberName, model.BookCopySlNo,
        //                                       (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""),
        //                                       (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""),
        //                                       model.CheckBit,


        //                                       model.page,
        //                                       model.rows,
        //                                       model.sidx,
        //                                       objTotalRecords
        //                                    )
        //                                select new BookIssueReturnReportViewModel
        //                                {


        //                                    RowKey = Report.RowKey,
        //                                    CardId = Report.LibraryID,
        //                                    MemberName = Report.NameofPerson,
        //                                    BookName = Report.BookName,
        //                                    BookCopySlNo = Report.BookCopySlNo,
        //                                    IssueDate = Report.IssueDate,
        //                                    ReturnDate = Report.ReturnDate,
        //                                    DueDate = Report.DueDate,
        //                                    IfAnyFine = Report.IfAnyFine,
        //                                    BookStatusName = Report.BookStatus





        //                                }).ToList();



        //    model.TotalRecords = objTotalRecords.Value != DBNull.Value ? Convert.ToInt64(objTotalRecords.Value) : 0;




        //    return BookIssueReturnSummaryReports;



        //}
        public List<BookCopyViewModel> BindBookCopies(BookCopyViewModel model)
        {



            var TotalBookCopyDetails = (from bc in dbContext.BookCopies
                                        join bs in dbContext.BookStatus on bc.BookStatusKey equals bs.RowKey
                                        where bc.BookKey == model.BookKey
                                        orderby bc.RowKey

                                        select new BookCopyViewModel
                                        {
                                            RowKey = bc.RowKey,
                                            BookCopySlNo = bc.BookCopySlNo,
                                            ISBN = bc.ISBN,
                                            BookBarCode = bc.BookBarCode,

                                            BookEdition = bc.BookEdition,

                                            BookPrintYear = bc.BookPrintYear
                                                ,
                                            NoOfPages = bc.NoOfPages
                                              ,
                                            BookPrice = bc.BookPrice
                                              ,
                                            FineAmount = bc.FineAmount
                                           ,
                                            BookStatusName = bs.BookStatusName,

                                            IsIssued = bc.IsIssued,

                                            IssueStatus = bc.IsIssued == true ? "Issued" : "Available"

                                        }).ToList();


            return TotalBookCopyDetails;




        }

        #endregion Book Summary

        #region Fill DropDowns

        public void FillDropdownLists(LibraryReportViewModel model)
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

        private void FillAuthors(LibraryReportViewModel model)
        {
            model.Authors = dbContext.Authors.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AuthorName
            }).ToList();
        }

        private void FillBookCategories(LibraryReportViewModel model)
        {
            model.BookCategories = dbContext.BookCategories.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BookCategoryName
            }).ToList();
        }

        private void FillBookIssueTypes(LibraryReportViewModel model)
        {
            model.BookIssueTypes = dbContext.BookIssueTypes.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BookIssueTypeName
            }).ToList();
        }

        private void FillLanguages(LibraryReportViewModel model)
        {
            model.Languages = dbContext.Languages.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.LanguageName
            }).ToList();
        }

        private void FillPublishers(LibraryReportViewModel model)
        {
            model.Publishers = dbContext.Publishers.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.PublisherName
            }).ToList();
        }

        public LibraryReportViewModel FillRacks(LibraryReportViewModel model)
        {
            IQueryable<RackViewModel> RackList = dbContext.Racks.Where(x => x.IsActive == true).Select(row => new RackViewModel
            {
                MasterRowKey = row.RowKey,
                RackName = row.RackName + EduSuiteUIResources.OpenBracketWithSpace + row.RackCode + EduSuiteUIResources.ClosingBracketWithSpace,
                BranchKey = row.BranchKey
            });

            if (model.BranchKeys.Count > 0)
            {
                RackList = RackList.Where(x => model.BranchKeys.Contains(x.BranchKey));
            }

            model.Racks = RackList.Select(row => new SelectListModel
            {
                RowKey = row.MasterRowKey,
                Text = row.RackName
            }).ToList();
            return model;
        }

        public LibraryReportViewModel FillSubRack(LibraryReportViewModel model)
        {
            IQueryable<SubRackDetailsModel> SubRackList = dbContext.SubRacks.Select(row => new SubRackDetailsModel
            {
                RowKey = row.RowKey,
                SubRackName = row.SubRackName + EduSuiteUIResources.OpenBracketWithSpace + row.SubRackCode + EduSuiteUIResources.ClosingBracketWithSpace,
                RackKey = row.RackKey
            });

            if (model.RackKeys.Count > 0)
            {
                SubRackList = SubRackList.Where(x => model.RackKeys.Contains(x.RackKey));
            }


            model.SubRacks = SubRackList.Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.SubRackName
            }).ToList();

            return model;
        }

        public void FillBranch(LibraryReportViewModel model)
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

                    }
                    else
                    {
                        model.Branches = BranchQuery.Where(x => x.RowKey == Employee.BranchKey).ToList();

                    }
                }
                else
                {
                    model.Branches = BranchQuery.ToList();
                    if (model.Branches.Count == 1)
                    {
                        long? branchkey = model.Branches.Select(x => x.RowKey).FirstOrDefault();

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
                        // model.BranchKey = Employee.BranchKey;
                    }
                }
                if (model.Branches.Count == 1)
                {
                    long? branchkey = model.Branches.Select(x => x.RowKey).FirstOrDefault();
                    //model.BranchKey = Convert.ToInt16(branchkey);

                }
            }
        }

        public void FillMemberplanDropDownList(LibraryReportViewModel model)
        {
            FillBranch(model);
            FillMemberType(model);
            FillBorrowerType(model);
            FillApplicationType(model);
            FillCourses(model);
            FillUniversity(model);
            FillBatch(model);
            FillClassDetails(model);
        }
        private void FillMemberType(LibraryReportViewModel model)
        {
            model.MemberTypes = dbContext.MemberTypes.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.MemberTypeName
            }).ToList();
        }

        private void FillBorrowerType(LibraryReportViewModel model)
        {
            model.BorrowerTypes = dbContext.BorrowerTypes.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BorrowerTypeName
            }).ToList();
        }

        private void FillApplicationType(LibraryReportViewModel model)
        {
            model.ApplicationTypes = dbContext.ApplicationTypes.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ApplicationTypeName
            }).ToList();
        }

        private void FillCourses(LibraryReportViewModel model)
        {
            model.Courses = dbContext.Courses.Where(x => x.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.CourseName
            }).ToList();
        }

        private void FillUniversity(LibraryReportViewModel model)
        {
            model.Universities = dbContext.UniversityMasters.Where(x => x.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.UniversityMasterName
            }).ToList();
        }

        private void FillBatch(LibraryReportViewModel model)
        {
            model.Batches = dbContext.Batches.Where(x => x.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BatchName
            }).ToList();
        }
        private void FillClassDetails(LibraryReportViewModel model)
        {
            model.ClassDetails = dbContext.VwClassDetailsSelectActiveOnlies.Where(x => x.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ClassCode + row.ClassCodeDescription
            }).ToList();
        }

        public void FillBookDrownList(LibraryReportViewModel model)
        {
            FillLibraryBooks(model);
            FillLibraryBooksCopies(model);
        }

        public LibraryReportViewModel FillLibraryBooks(LibraryReportViewModel model)
        {
            IQueryable<LibraryBookViewModel> LibraryBooks = dbContext.LibraryBooks.Where(x => x.IsActive).Select(row => new LibraryBookViewModel
            {
                RowKey = row.RowKey,
                BookName = row.BookName,
                BookName_Optional = row.BookName_Optional,
                AuthorKey = row.AuthorKey,
                BookCategoryKey = row.BookCategoryKey,
                BookIssueTypeKey = row.BookIssueTypeKey,
                LanguageKey = row.LanguageKey,
                PublisherKey = row.PublisherKey,
                RackKey = row.SubRack.RackKey,
                SubRackKey = row.SubRackKey ?? 0,
                BranchKey = row.BranchKey,
                CoverPhoto = row.CoverPhoto,
            });



            if (model.AuthorKeys.Count > 0)
            {
                LibraryBooks = LibraryBooks.Where(x => model.AuthorKeys.Contains(x.AuthorKey));
            }
            if (model.BookCategoryKeys.Count > 0)
            {
                LibraryBooks = LibraryBooks.Where(x => model.AuthorKeys.Contains(x.BookCategoryKey));
            }
            if (model.BookIssueTypeKeys.Count > 0)
            {
                LibraryBooks = LibraryBooks.Where(x => model.AuthorKeys.Contains(x.BookIssueTypeKey));
            }
            if (model.LanguageKeys.Count > 0)
            {
                LibraryBooks = LibraryBooks.Where(x => model.AuthorKeys.Contains(x.LanguageKey));
            }
            if (model.PublisherKeys.Count > 0)
            {
                LibraryBooks = LibraryBooks.Where(x => model.AuthorKeys.Contains(x.PublisherKey));
            }
            if (model.RackKeys.Count > 0)
            {
                LibraryBooks = LibraryBooks.Where(x => model.AuthorKeys.Contains(x.RackKey));
            }
            if (model.SubRackKeys.Count > 0)
            {
                LibraryBooks = LibraryBooks.Where(x => model.AuthorKeys.Contains(x.SubRackKey));
            }
            if (model.BranchKeys.Count > 0)
            {
                LibraryBooks = LibraryBooks.Where(x => model.AuthorKeys.Contains(x.BranchKey));
            }


            model.LibraryBooks = LibraryBooks.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BookName_Optional != null ? row.BookName_Optional + EduSuiteUIResources.OpenBracketWithSpace + row.BookName + EduSuiteUIResources.ClosingBracketWithSpace : row.BookName
            }).ToList();

            return model;
        }


        public LibraryReportViewModel FillLibraryBooksCopies(LibraryReportViewModel model)
        {
            IQueryable<BookCopyViewModel> LibraryBookCopies = dbContext.BookCopies.Where(x => x.IsActive == true).Select(row => new BookCopyViewModel
            {
                RowKey = row.RowKey,
                BookKey = row.BookKey,
                BookCopySlNo = row.BookCopySlNo + EduSuiteUIResources.OpenBracketWithSpace + row.LibraryBook.BookName + EduSuiteUIResources.ClosingBracketWithSpace,

            });



            if (model.LibraryBookKeys.Count > 0)
            {
                LibraryBookCopies = LibraryBookCopies.Where(x => model.AuthorKeys.Contains(x.BookKey));
            }



            model.LibraryBookCopies = LibraryBookCopies.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BookCopySlNo
            }).ToList();

            return model;
        }

        #endregion Fill DropDowns

        #region Memberplan Summary
        public List<LibraryReportViewModel> GetMemberPlanSummary(LibraryReportViewModel model)
        {
            ObjectParameter objTotalRecords = new ObjectParameter("TotalRecords", typeof(Int64));


            if (model.sidx != "")
            {
                model.sidx = model.sidx + " " + model.sord;
            }

            var MemberplanSummary = (from Application in dbContext.Sp_MemberPlanSummary
  (
     String.Join(",", model.MemberTypeKeys),
    String.Join(",", model.BorrowerTypeKeys),
    String.Join(",", model.ApplicationTypeKeys),
    String.Join(",", model.CourseKeys),
    String.Join(",", model.UniversityKeys),
    String.Join(",", model.BatchKeys),
    String.Join(",", model.ClassDetailsKeys),
    String.Join(",", model.BranchKeys),
     model.SearchAnyText.VerifyData(),

     (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""),
     (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""),

     model.page,
     model.rows,
     model.sidx,
     objTotalRecords

  )
                                     select new LibraryReportViewModel
                                     {
                                         RowKey = Application.RowKey
                                            ,
                                         CardId = Application.CardId
                                            ,
                                         ApplicationTypeName = Application.ApplicationTypeName
                                            ,
                                         MemberTypeName = Application.MemberTypeName
                                            ,
                                         BorrowerTypeName = Application.BorrowerTypeName
                                            ,
                                         MemberName = Application.MemberName
                                            ,
                                         MobileNo = Application.MobileNo
                                            ,
                                         EmailAddress = Application.EmailAddress
                                            ,
                                         BranchName = Application.BranchName
                                            ,
                                         Gender = Application.Gender
                                            ,
                                         MemberPhoto = Application.MemberPhoto
                                            ,
                                         Descreption = Application.Descreption
                                            ,
                                         DateAdded = Application.DateAdded
                                            ,
                                         NumberOfBooksAllowed = Application.NumberOfBooksAllowed
                                            ,
                                         TotalIssued = Application.TotalIssued
                                            ,
                                         BalanceReturn = Application.BalanceReturn
                                            ,
                                         IsBlockMember = Application.IsBlockMember



                                     }).ToList();



            model.TotalRecords = objTotalRecords.Value != DBNull.Value ? Convert.ToInt64(objTotalRecords.Value) : 0;

            return MemberplanSummary;
        }

        public LibraryReportViewModel GetLibraryBookDetails(LibraryReportViewModel model)
        {
            model = (from vw in dbContext.VW_MemberPlanDetails.Where(x => x.RowKey == model.RowKey)
                     select new LibraryReportViewModel
                     {
                         RowKey = vw.RowKey,
                         MemberTypeName = vw.MemberTypeName,
                         BorrowerTypeName = vw.BorrowerTypeName,
                         CardId = vw.CardId,
                         EmailAddress = vw.EmailAddress,
                         IsBlockMember = vw.IsBlockMember,
                         MemberName = vw.MemberName,
                         ApplicationTypeName = vw.ApplicationTypeName,
                         MobileNo = vw.MobileNo,
                         Descreption = vw.Descreption,
                         UniqueCode = vw.UniqueCode,
                         DateAdded = vw.DateAdded,
                         NumberOfBooksAllowed = vw.NumberOfBooksAllowed,
                         TotalIssued = dbContext.BookIssueReturnDetails.Where(x => x.BookIssueReturnMaster.MemberPlanDetail.CardId == model.CardId).Count(),
                         BalanceReturn = dbContext.BookIssueReturnDetails.Where(x => x.BookIssueReturnMaster.MemberPlanDetail.CardId == model.CardId && x.ReturnDate == null).Count(),
                         MemberPhoto = vw.MemberPhoto,
                         MemberphotoPath = vw.ApplicationTypeKey == DbConstants.ApplicationType.Student ? UrlConstants.ApplicationUrl + vw.UniqueCode + "/" + vw.MemberPhoto : vw.ApplicationTypeKey == DbConstants.ApplicationType.Staff ? (UrlConstants.EmployeeUrl + vw.UniqueCode + "/" + vw.MemberPhoto) : (UrlConstants.LibraryMemberUrl + vw.CardId + "/" + vw.MemberPhoto),

                     }).FirstOrDefault();

            model.LibraryReportBookDetailsmodel = (from row in dbContext.BookIssueReturnDetails.Where(x => x.BookIssueReturnMaster.MemberRegistrationKey == model.RowKey)
                                                   select new LibraryReportBookDetailsmodel
                                                   {
                                                       BookName = row.BookCopy.LibraryBook.BookName,
                                                       BookName_Optional = row.BookCopy.LibraryBook.BookName_Optional,
                                                       BookEdition = row.BookCopy.BookEdition,
                                                       ISBN = row.BookCopy.ISBN,
                                                       BookCopySlNo = row.BookCopy.BookCopySlNo,
                                                       BookBarCode = row.BookCopy.BookBarCode,
                                                       BookPrintYear = row.BookCopy.BookPrintYear,
                                                       NoOfPages = row.BookCopy.NoOfPages,
                                                       BookPrice = row.BookCopy.BookPrice,
                                                       AuthorName = row.BookCopy.LibraryBook.Author.AuthorName,
                                                       BookCategoryName = row.BookCopy.LibraryBook.BookCategory.BookCategoryName,
                                                       LanguageName = row.BookCopy.LibraryBook.Language.LanguageName,
                                                       PublisherName = row.BookCopy.LibraryBook.Publisher.PublisherName,
                                                       RackName = row.BookCopy.LibraryBook.SubRack.Rack.RackName,
                                                       SubRackName = row.BookCopy.LibraryBook.SubRack.SubRackName,
                                                       CoverPhoto = row.BookCopy.LibraryBook.CoverPhoto,
                                                       IssuedDate = row.BookIssueReturnMaster.IssueDate,
                                                       DueDate = row.BookIssueReturnMaster.DueDate,
                                                       ReturnDate = row.ReturnDate,
                                                       FineAmount = row.IfAnyFine,
                                                       Remarks = row.Remark,
                                                       BookStatusName = row.BookStatu.BookStatusName
                                                   }).ToList();
            return model;
        }
        #endregion MemberPlan Summary

        #region Book Issue Summary
        public List<LibraryReportViewModel> GetBookIssueSummary(LibraryReportViewModel model)
        {
            ObjectParameter objTotalRecords = new ObjectParameter("TotalRecords", typeof(Int64));


            if (model.sidx != "")
            {
                model.sidx = model.sidx + " " + model.sord;
            }

            var MemberplanSummary = (from Application in dbContext.Sp_libraryBookIssueSummary
  (
     String.Join(",", model.MemberTypeKeys),
            String.Join(",", model.BorrowerTypeKeys),
    String.Join(",", model.ApplicationTypeKeys),
    String.Join(",", model.CourseKeys),
    String.Join(",", model.UniversityKeys),
    String.Join(",", model.BatchKeys),
    String.Join(",", model.ClassDetailsKeys),
    String.Join(",", model.BranchKeys),
    String.Join(",", model.AuthorKeys),
    String.Join(",", model.BookCategoryKeys),
    String.Join(",", model.BookIssueTypeKeys),
    String.Join(",", model.LanguageKeys),
    String.Join(",", model.PublisherKeys),
    String.Join(",", model.RackKeys),
     String.Join(",", model.SubRackKeys),
     String.Join(",", model.LibraryBookKeys),
     String.Join(",", model.LibraryBookCopyKeys),
     model.SearchAnyText.VerifyData(),

     (model.DateAddedFrom != null ? Convert.ToDateTime(model.DateAddedFrom).ToString("yyyy-MM-dd") : ""),
     (model.DateAddedTo != null ? Convert.ToDateTime(model.DateAddedTo).ToString("yyyy-MM-dd") : ""),

     model.page,
     model.rows,
     model.sidx,
     objTotalRecords

  )
                                     select new LibraryReportViewModel
                                     {
                                         RowKey = Application.RowKey
                                            ,
                                         BookCopyKey = Application.BookCopyKey
                                            ,
                                         CardId = Application.CardId
                                            ,
                                         ApplicationTypeName = Application.ApplicationTypeName
                                            ,
                                         MemberTypeName = Application.MemberTypeName
                                            ,
                                         BorrowerTypeName = Application.BorrowerTypeName
                                            ,
                                         MemberName = Application.MemberName
                                            ,
                                         MobileNo = Application.MobileNo
                                            ,
                                         EmailAddress = Application.EmailAddress
                                            ,
                                         BranchName = Application.BranchName
                                            ,
                                         Gender = Application.Gender
                                            ,
                                         MemberPhoto = Application.MemberPhoto
                                            ,
                                         Descreption = Application.Descreption
                                            ,
                                         DateAdded = Application.DateAdded
                                            ,
                                         NumberOfBooksAllowed = Application.NumberOfBooksAllowed
                                            ,
                                         BookName = Application.BookName
                                                ,
                                         BookName_Optional = Application.BookName_Optional
                                                ,
                                         BookCode = Application.BookCode
                                                ,
                                         CoverPhoto = Application.CoverPhoto
                                                ,
                                         AuthorName = Application.AuthorName
                                                ,
                                         BookCategoryName = Application.BookCategoryName
                                                ,
                                         BookIssueTypeName = Application.BookIssueTypeName
                                                ,
                                         LanguageName = Application.LanguageName
                                                ,
                                         PublisherName = Application.PublisherName
                                                ,
                                         SubRackName = Application.SubRackName
                                                ,
                                         RackName = Application.RackName
                                                ,
                                         IsBlockMember = Application.IsBlockMember
                                                ,
                                         BookCopySlNo = Application.BookCopySlNo
                                                ,
                                         ISBN = Application.ISBN
                                                ,
                                         BookBarCode = Application.BookBarCode
                                                ,
                                         BookEdition = Application.BookEdition
                                                ,
                                         BookPrintYear = Application.BookPrintYear
                                                ,
                                         NoOfPages = Application.NoOfPages
                                                ,
                                         BookPrice = Application.BookPrice
                                                ,
                                         IssuedDate = Application.IssueDate
                                                ,
                                         DueDate = Application.DueDate
                                                ,
                                         ReturnDate = Application.ReturnDate
                                                ,
                                         BookStatusName = Application.BookStatusName
                                                ,
                                         FineAmount = Application.IfAnyFine
                                                ,
                                         Remarks = Application.Remark
                                                ,
                                         CreatedBy = Application.IssuedBy
                                     }).ToList();



            model.TotalRecords = objTotalRecords.Value != DBNull.Value ? Convert.ToInt64(objTotalRecords.Value) : 0;

            return MemberplanSummary;
        }
        #endregion Book Issue Summary
    }
}
