using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class ApplicationDocumentViewModel : BaseModel
    {
        public ApplicationDocumentViewModel()
        {
            DocumentTypes = new List<SelectListModel>();
            ApplicationDocuments = new List<DocumentViewModel>();
        }
        public long ApplicationKey { get; set; }
        public string AdmissionNo { get; set; }

        public List<SelectListModel> DocumentTypes { get; set; }
        public List<DocumentViewModel> ApplicationDocuments { get; set; }
    }
    public class DocumentViewModel
    {
        public DocumentViewModel()
        {
            IsActive = true;
        }
        public long RowKey { get; set; }       
        public string DocumentTypeName { get; set; }
        public HttpPostedFileBase DocumentFile { get; set; }
        
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Document", ResourceType = typeof(EduSuiteUIResources))]
        public string StudentDocumentName { get; set; }
        public string StudentDocumentPath { get; set; }
        public string StudentDocumentPathText { get; set; }
        public bool IsActive { get; set; }
    }
}
