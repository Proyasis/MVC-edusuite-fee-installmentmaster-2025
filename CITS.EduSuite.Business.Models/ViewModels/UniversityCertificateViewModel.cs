using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using CITS.EduSuite.Business.Models.Resources;
using System.Web;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class UniversityCertificateViewModel : BaseModel
    {
        public UniversityCertificateViewModel()
        {
            UniversityCertificateDetails = new List<ViewModels.UniversityCertificateDetails>();
            CommonDate = DateTimeUTC.Now;
            CertificateTypeList = new List<SelectListModel>();
            Branches = new List<SelectListModel>();
            Batches = new List<SelectListModel>();
        }

        public long ApplicationKey { get; set; }
        public DateTime? CommonDate { get; set; }
              
        public List<SelectListModel> CertificateTypeList { get; set; }
        public List<UniversityCertificateDetails> UniversityCertificateDetails { get; set; }
        public List<SelectListModel> Branches { get; set; }
        public List<SelectListModel> Batches { get; set; }
        public short? BranchKey { get; set; }
        public short? BatchKey { get; set; }
    }

    public class UniversityCertificateDetails
    {
        public UniversityCertificateDetails()
        {
            IsActive = true;
        }
        public long RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CertificateTypeRequired")]
        [System.Web.Mvc.Remote("CheckCertificateTypeExists", "UniversityCertificate", AdditionalFields = "RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CertificateTypeExists")]
        public short CertificateTypeKey { get; set; }
        public string CertificateTypeName { get; set; }        
        public long ApplicationKey { get; set; }
        public string UniversityCertificateDescription { get; set; }
        public bool IsReceived { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public long? ReceivedBy { get; set; }
        public bool IsIssued { get; set; }
        public DateTime? IssuedDate { get; set; }
        public long? IssuedBy { get; set; }
        public bool IsActive { get; set; }
        public string ReceivedByName { get; set; }
        public string IssuedByName { get; set; }
        public string CertificatePath { get; set; }
        public string CertificatePathText { get; set; }
        public HttpPostedFileBase DocumentFile { get; set; }
    }
}
