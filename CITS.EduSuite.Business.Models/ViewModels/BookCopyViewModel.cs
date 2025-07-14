using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class BookCopyViewModel : BaseModel
    {
        public BookCopyViewModel()
        {
            Books = new List<SelectListModel>();
            //Racks = new List<SelectListModel>();
            IsActive = true;
            BookStatuses = new List<SelectListModel>();
        }

        public long RowKey { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BookRequired")]
        public long BookKey { get; set; }
        public string BookName { get; set; }

        //public short BookCopySlNo { get; set; }
        public long SerialNo { get; set; }
        public string BookCopySlNo { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ISBNLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z ,()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ISBNExpressionErrorMessage")]
        public string ISBN { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BookBarCodeLengthErrorMessage")]
        public string BookBarCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BookEditionRequired")]
        [StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BookEditionLengthErrorMessage")]
        public string BookEdition { get; set; }

        [RegularExpression(@"^\d{4}$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BookPrintYearExpressionErrorMessage")]
        public short? BookPrintYear { get; set; }


        [Range(1, Int16.MaxValue, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "NoOfPagesRangeErrorMessage")]
        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "NoOfPagesExpressionErrorMessage")]
        public short? NoOfPages { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BookPriceRequired")]
        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BookPriceExpressionErrorMessage")]
        public decimal? BookPrice { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FineAmountRequired")]
        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FineAmountExpressionErrorMessage")]
        public decimal? FineAmount { get; set; }

        public bool IsIssued { get; set; }


        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RackRequired")]
        //public int RackKey { get; set; }
        //public string RackName { get; set; }


        public bool IsActive { get; set; }

        public string IsActiveText { get { return IsActive ? EduSuiteUIResources.Yes : EduSuiteUIResources.No; } }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BookStatusRequired")]
        public byte? BookStatusKey { get; set; }
        public string BookStatusName { get; set; }
        
        public string IssueStatus { get; set; }

        public List<SelectListModel> Books { get; set; }
        public List<SelectListModel> Racks { get; set; }
        public List<SelectListModel> BookStatuses { get; set; }
    }
}
