using CITS.EduSuite.Business.Models.Resources;
using CITS.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class BookIssueReturnMasterViewModel : BaseModel
    {
        public BookIssueReturnMasterViewModel()
        {
            BookIssueReturnDetails = new List<BookIssueReturnDetailsViewModel>();
            IssueDate = DateTime.Today;

        }
        public int RowKey { get; set; }
        public int MemberRegistrationKey { get; set; }

        public string MemberName { get; set; }
        public string BookName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "IssueDateRequired")]
        public DateTime? IssueDate { get; set; }

        public DateTime? DueDate { get; set; }
        

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CardIdRequired")]
        [StringLength(20, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CardIdLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z ,()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CardIdRegularExpressionErrorMessage")]
        [System.Web.Mvc.Remote("CheckCardIdExists", "BookIssueReturn", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CardIdExistsErrorMessage")]
        public string CardId { get; set; }
        public int NumberofBooks { get; set; }
        public int? NumberofReturnBooks { get; set; }
        public int NumberofBooksDay { get; set; }

        [RegularExpressionIf(@"^[^0]+$", "RowKey", 0, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "NumberofBooksDayRegularExpressionErrorMessage")]
        public int NumberofBooksDayRemain { get; set; }
        public int NumberofBooksRemain { get; set; }

        public List<BookIssueReturnDetailsViewModel> BookIssueReturnDetails { get; set; }

        public short? BranchKey { get; set; }
        public string BranchName { get; set; }
        public byte? BorrowerTypeKey { get; set; }
        public byte? MemberTypeKey { get; set; }
        public byte? ApplicationTypeKey { get; set; }
        public string BorrowerTypeName { get; set; }
        public string MemberTypeName { get; set; }
        public string ApplicationTypeName { get; set; }
        public string SearchText { get; set; }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public string SortBy { get; set; }
        public string SortOrder { get; set; }

        public List<SelectListModel> Branches { get; set; }
        public List<SelectListModel> MemberType { get; set; }
        public List<SelectListModel> BorrowerType { get; set; }
        public List<SelectListModel> ApplicationTypes { get; set; }

    }
    public class BookIssueReturnDetailsViewModel
    {
        public BookIssueReturnDetailsViewModel()
        {
            BookCategory = new List<SelectListModel>();
            Book = new List<SelectListModel>();
            BookCopy = new List<SelectListModel>();
            BookStatus = new List<SelectListModel>();
        }
        [StringLength(200, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RemarkLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z ,()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RemarkRegularExpressionErrorMessage")]
        public string Remark { get; set; }

        public long? RowKey { get; set; }

       
        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        //[Display(Name = "BookCategory", ResourceType = typeof(EduSuiteUIResources))]
        public int? BookCategoryKey { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        //[Display(Name = "Book", ResourceType = typeof(EduSuiteUIResources))]
        public long? BookKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [System.Web.Mvc.Remote("CheckBookCopyExists", "BookIssueReturn", AdditionalFields = "BookKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExistsErrorMessage")]
        [Display(Name = "BookCopy", ResourceType = typeof(EduSuiteUIResources))]
        public long BookCopyKey { get; set; }
        public string BookName { get; set; }
        public string BookEdition { get; set; }
        public string BookCopySlNo { get; set; }

        //[GreaterThan("DueDate", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ErrorGreaterThanReturnDate")]
        public DateTime? ReturnDate { get; set; }

        [RequiredIfNotEmpty("ReturnDate", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BookStatusRequired")]
        public byte? BookStatusKey { get; set; }
        public decimal? IfAnyFine { get; set; }

        public List<SelectListModel> BookCategory { get; set; }
        public List<SelectListModel> Book { get; set; }
        public List<SelectListModel> BookCopy { get; set; }
        public List<SelectListModel> BookStatus { get; set; }
    }
}
