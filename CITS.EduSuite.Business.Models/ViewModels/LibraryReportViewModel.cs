using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class LibraryReportViewModel
    {
        public LibraryReportViewModel()
        {
            Authors = new List<SelectListModel>();
            BookCategories = new List<SelectListModel>();
            BookIssueTypes = new List<SelectListModel>();
            Languages = new List<SelectListModel>();
            Publishers = new List<SelectListModel>();
            Racks = new List<SelectListModel>();
            SubRacks = new List<SelectListModel>();
            Branches = new List<SelectListModel>();
            LibraryBooks = new List<SelectListModel>();
            LibraryBookCopies = new List<SelectListModel>();            
            Columns = new List<SelectListModel>();
            AuthorKeys = new List<long>();
            BookCategoryKeys = new List<long>();
            BookIssueTypeKeys = new List<long>();
            LanguageKeys = new List<long>();
            PublisherKeys = new List<long>();
            RackKeys = new List<long>();
            SubRackKeys = new List<long>();
            BranchKeys = new List<long>();
            MemberTypeKeys = new List<long>();
            BorrowerTypeKeys = new List<long>();
            ApplicationTypeKeys = new List<long>();
            CourseKeys = new List<long>();
            UniversityKeys = new List<long>();
            BatchKeys = new List<long>();
            ClassDetailsKeys = new List<long>();
            ClassDetailsKeys = new List<long>();
            LibraryBookKeys = new List<long>();
            LibraryBookCopyKeys = new List<long>();
        }


        public List<long> AuthorKeys { get; set; }
        public List<long> BookCategoryKeys { get; set; }
        public List<long> BookIssueTypeKeys { get; set; }
        public List<long> LanguageKeys { get; set; }
        public List<long> PublisherKeys { get; set; }
        public List<long> RackKeys { get; set; }
        public List<long> SubRackKeys { get; set; }
        public List<long> BranchKeys { get; set; }
        public List<long> MemberTypeKeys { get; set; }
        public List<long> BorrowerTypeKeys { get; set; }
        public List<long> ApplicationTypeKeys { get; set; }
        public List<long> CourseKeys { get; set; }
        public List<long> UniversityKeys { get; set; }
        public List<long> BatchKeys { get; set; }
        public List<long> ClassDetailsKeys { get; set; }
        public List<long> LibraryBookKeys { get; set; }
        public List<long> LibraryBookCopyKeys { get; set; }



        public string SearchAnyText { get; set; }
        public DateTime? DateAddedFrom { get; set; }
        public DateTime? DateAddedTo { get; set; }
        public DateTime? DateAdded { get; set; }
        public string sidx { get; set; }
        public string sord { get; set; }
        public int? page { get; set; }
        public int? rows { get; set; }
        public int? draw { get; set; }
        //public int PageSize { get; set; }
        //public int PageIndex { get; set; }
        public long TotalRecords { get; set; }


        public long? RowKey { get; set; }
        public long? BookCopyKey { get; set; }
        public string BookName { get; set; }
        public string BookName_Optional { get; set; }
        public string BookCode { get; set; }
        public string CoverPhoto { get; set; }
        public string AuthorName { get; set; }
        public string BookCategoryName { get; set; }
        public string BookIssueTypeName { get; set; }
        public string LanguageName { get; set; }
        public string BranchName { get; set; }
        public string PublisherName { get; set; }
        public string SubRackName { get; set; }
        public string RackName { get; set; }
        public int? TotalCopy { get; set; }
        public int? IssuedCount { get; set; }
        public int? NotIssuedCount { get; set; }
        public int? DamageCount { get; set; }
        public string AddedBy { get; set; }
        public string Activetext { get; set; }
        public string CreatedBy { get; set; }
        public int? RackKey { get; set; }

        public byte? ApplicationTypeKey { get; set; }

        public string CardId { get; set; }       
        public string ApplicationTypeName { get; set; }
        public string MemberTypeName { get; set; }
        public string BorrowerTypeName { get; set; }
        public string MemberName { get; set; }
        public string MobileNo { get; set; }
        public string EmailAddress { get; set; }
        public string Gender { get; set; }
        public string MemberPhoto { get; set; }
        public string Descreption { get; set; }
        public string UniqueCode { get; set; }
        public string MemberphotoPath { get; set; }
        public int? NumberOfBooksAllowed { get; set; }
        public int? TotalIssued { get; set; }
        public int? BalanceReturn { get; set; }
        public bool? IsBlockMember { get; set; }



        public string BookEdition { get; set; }
        public string ISBN { get; set; }
        public string BookCopySlNo { get; set; }
        public string BookBarCode { get; set; }
        public short? BookPrintYear { get; set; }
        public short? NoOfPages { get; set; }
        public decimal? BookPrice { get; set; }
        public decimal? FineAmount { get; set; }
        public string BookStatusName { get; set; }
        public string Remarks { get; set; }
        public DateTime? IssuedDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public DateTime? DueDate { get; set; }


        public List<SelectListModel> Authors { get; set; }
        public List<SelectListModel> BookCategories { get; set; }
        public List<SelectListModel> BookIssueTypes { get; set; }
        public List<SelectListModel> Languages { get; set; }
        public List<SelectListModel> Publishers { get; set; }
        public List<SelectListModel> Racks { get; set; }
        public List<SelectListModel> SubRacks { get; set; }
        public List<SelectListModel> Branches { get; set; }
        public List<SelectListModel> Columns { get; set; }
        public List<SelectListModel> MemberTypes { get; set; }
        public List<SelectListModel> BorrowerTypes { get; set; }
        public List<SelectListModel> ApplicationTypes { get; set; }
        public List<SelectListModel> Courses { get; set; }
        public List<SelectListModel> Universities { get; set; }
        public List<SelectListModel> Batches { get; set; }
        public List<SelectListModel> ClassDetails { get; set; }
        public List<SelectListModel> LibraryBooks { get; set; }
        public List<SelectListModel> LibraryBookCopies { get; set; }

        public List<LibraryReportBookDetailsmodel> LibraryReportBookDetailsmodel { get; set; }
    }

    public class LibraryReportBookDetailsmodel
    {

        public string BookName { get; set; }
        public string BookName_Optional { get; set; }
        public string BookEdition { get; set; }
        public string ISBN { get; set; }
        public string BookCopySlNo { get; set; }
        public string BookBarCode { get; set; }
        public short? BookPrintYear { get; set; }
        public short? NoOfPages { get; set; }
        public decimal? BookPrice { get; set; }
        public decimal? FineAmount { get; set; }
        public bool IsIssued { get; set; }
        public byte? BookStatusKey { get; set; }
        public string BookStatusName { get; set; }
        public DateTime? IssuedDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string AuthorName { get; set; }
        public string BookCategoryName { get; set; }
        public string LanguageName { get; set; }
        public string PublisherName { get; set; }
        public string RackName { get; set; }
        public string SubRackName { get; set; }
        public string Remarks { get; set; }

        public string CoverPhoto { get; set; }
       
    }
}
