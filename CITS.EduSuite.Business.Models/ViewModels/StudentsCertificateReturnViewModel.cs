using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class StudentsCertificateReturnViewModel : BaseModel
    {
        public StudentsCertificateReturnViewModel()
        {
            StudentCertificatedetails = new List<StudentsCertificateReturnDetail>();
            ProcessDate = DateTimeUTC.Now;
            CommonDate = DateTimeUTC.Now;
            Branches = new List<SelectListModel>();
            Batches = new List<SelectListModel>();
        }
        public long ApplicationKey { get; set; }
        public byte CertificateStatusKey { get; set; }

        public DateTime? ProcessDate { get; set; }

        public DateTime? CommonDate { get; set; }

        
        
        public List<StudentsCertificateReturnDetail> StudentCertificatedetails { get; set; }
        public List<SelectListModel> Branches { get; set; }
        public List<SelectListModel> Batches { get; set; }
        public bool IsPermenant { get; set; }
        public short? BranchKey { get; set; }
        public short? BatchKey { get; set; }
    }
    public class StudentsCertificateReturnDetail
    {
        public StudentsCertificateReturnDetail()
        {
            IsActive = true;
        }
        public long RowKey { get; set; }
        public long EducationQualificationKey { get; set; }
        public string EducationQualificationName { get; set; }
        public string EducationQualificationUniversity { get; set; }
        public bool CertificateStatus { get; set; }
        public bool IsActive { get; set; }
        public string Remarks { get; set; }
        public long ApplicationKey { get; set; }
        public int? IssuedBy { get; set; }
        public DateTime? IssuedDate { get; set; }
        public string CertificateStatusName { get; set; }
        public string CertificateStatusBy { get; set; }
        public int? EducationQualificationYear { get; set; }
        public int? EducationQualificationPercentage { get; set; }
        public long? LastKey { get; set; }
        public int? ListCount { get; set; }
        public bool IsVerified { get; set; }
        public List<ReturnCertificateDetails> ReturnCertificateDetails { get; set; }
        public string EducationQualificationCertificatePath { get; set; }
        public string EducationQualificationCertificateType { get; set; }

    }
    public class ReturnCertificateDetails
    {
        public string CertificateStatusName { get; set; }
        public string CertificateStatusBy { get; set; }
        public DateTime? CertificatestatusDate { get; set; }
    }
}
