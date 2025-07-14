using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class LibraryBookViewModel : BaseModel
    {
        public LibraryBookViewModel()
        {
            Authors = new List<SelectListModel>();
            BookCategories = new List<SelectListModel>();
            BookIssueTypes = new List<SelectListModel>();
            Languages = new List<SelectListModel>();
            Publishers = new List<SelectListModel>();
            Racks = new List<SelectListModel>();
            SubRacks = new List<SelectListModel>();
            Branches = new List<SelectListModel>();
            CoverPhotoPath = Resources.ModelResources.DefaultPhotoUrl;
        }

        public long RowKey { get; set; }


        [StringLength(200, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z ,()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BookNameExpressionErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Book", ResourceType = typeof(EduSuiteUIResources))]

        public string BookName { get; set; }

        [StringLength(200, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [Display(Name = "Book", ResourceType = typeof(EduSuiteUIResources))]
        public string BookName_Optional { get; set; }

        public string BookCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Author", ResourceType = typeof(EduSuiteUIResources))]
        public int AuthorKey { get; set; }
        public string AuthorName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "BookCategory", ResourceType = typeof(EduSuiteUIResources))]
        public short BookCategoryKey { get; set; }
        public string BookCategoryName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "BookIssueType", ResourceType = typeof(EduSuiteUIResources))]
        public byte BookIssueTypeKey { get; set; }
        public string BookIssueTypeName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Language", ResourceType = typeof(EduSuiteUIResources))]
        public short LanguageKey { get; set; }
        public string LanguageName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Publisher", ResourceType = typeof(EduSuiteUIResources))]
        public int PublisherKey { get; set; }
        public string PublisherName { get; set; }

        public int NoOfBooks { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "WardrobeRack", ResourceType = typeof(EduSuiteUIResources))]
        public long SubRackKey { get; set; }
        public string SubRackName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Wardrobe", ResourceType = typeof(EduSuiteUIResources))]
        public int RackKey { get; set; }
        public string RackName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Branch", ResourceType = typeof(EduSuiteUIResources))]
        public short BranchKey { get; set; }
        public string BranchName { get; set; }
        public string SearchText { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public HttpPostedFileBase PhotoFile { get; set; }
        public string CoverPhoto { get; set; }
        public string CoverPhotoPath { get; set; }

        public List<SelectListModel> Authors { get; set; }
        public List<SelectListModel> BookCategories { get; set; }
        public List<SelectListModel> BookIssueTypes { get; set; }
        public List<SelectListModel> Languages { get; set; }
        public List<SelectListModel> Publishers { get; set; }

        public List<SelectListModel> Racks { get; set; }
        public List<SelectListModel> SubRacks { get; set; }
        public List<SelectListModel> Branches { get; set; }


    }

}
