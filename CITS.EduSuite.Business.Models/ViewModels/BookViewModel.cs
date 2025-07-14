using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class BookViewModel : BaseModel
    {
        public BookViewModel()
        {
           
            CourseTypes = new List<SelectListModel>();
            Courses = new List<SelectListModel>();
            Universities = new List<SelectListModel>();
            AcademicTerms = new List<SelectListModel>();
            BookYears = new List<SelectListModel>();
            BookTypes = new List<SelectListModel>();
            BookDetails = new List<BookDetailsViewModel>();

        }





        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CourseKeyRequired")]
        public long CourseKey { get; set; }
        public string CourseName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CourseTypeRequired")]
        public short CourseTypeKey { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "UniversityKeyRequired")]
        public short UniversityKey { get; set; }
        public string UniversityName { get; set; }




        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SyllabusRequired")]
        public short AcademicTermKey { get; set; }
        public string AcademicTermName { get; set; }


        public List<SelectListModel> CourseTypes { get; set; }
        public List<SelectListModel> Courses { get; set; }
        public List<SelectListModel> Universities { get; set; }


        public List<SelectListModel> AcademicTerms { get; set; }

        public List<BookDetailsViewModel> BookDetails { get; set; }

        public List<SelectListModel> BookYears { get; set; }
        public List<SelectListModel> BookTypes { get; set; }
    }

    public class BookDetailsViewModel
    {
        public BookDetailsViewModel()
        {
            HasBook = true;
            IsActive = true;
        }
        public long RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BookCodeRequired")]
        [StringLength(10, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BookCodeLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BookCodeExpressionErrorMessage")]
        [System.Web.Mvc.Remote("CheckBookCodeExist", "Book", AdditionalFields = "RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BookCodeExistsErrorMessage")]
        public string BookCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BookNameRequired")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BookNameLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BookNameExpressionErrorMessage")]
        public string BookName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BookYearRequired")]
        public short BookYearKey { get; set; }
        public string BookYearName { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BookTypeRequired")]
        public string BookType { get; set; }
        public bool HasBook { get; set; }

        public bool IsActive { get; set; }
        public string IsActiveText { get { return IsActive ? EduSuiteUIResources.Yes : EduSuiteUIResources.No; } }

    }
}
