using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;
using CITS.Validations;
using CITS.EduSuite.Business.Models.Security;
using System.Threading;


namespace CITS.EduSuite.Business.Models.ViewModels
{
    public  class StudentIDCardViewModels:BaseModel
    {
        public StudentIDCardViewModels()
        {
            StudentIDCardList = new List<StudentIDCardList>();
            CourseTypes = new List<SelectListModel>();
            Courses = new List<SelectListModel>();
            Universities = new List<SelectListModel>();
            Batches = new List<SelectListModel>();
            Branches = new List<SelectListModel>();
        }
        public long RowKey { get; set; }
        public short? SearchCourseTypeKey { get; set; }
        public long? SearchCourseKey { get; set; }
        public short? SearchUniversityKey { get; set; }
        public short? SearchBatchKey { get; set; }
        public string SearchAdmissionNo { get; set; }
        public string SearchName { get; set; }

        public short? SearchBranchKey { get; set; }

        public string PrintApplicationKeys { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SelectUpdateDate")]
        public DateTime? UpdateDate { get; set; }
        public bool IsSendSms { get; set; }
        public List<StudentIDCardList> StudentIDCardList { get; set; }
        public List<SelectListModel> CourseTypes { get; set; }
        public List<SelectListModel> Courses { get; set; }
        public List<SelectListModel> Universities { get; set; }
        public List<SelectListModel> Batches { get; set; }
        public int TotalRecords { get; set; }
        public List<SelectListModel> Branches { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public class StudentIDCardList
    {
        public long? StudentIDCardRowKey { get; set; }
        public string AdmissionNo { get; set; }
        public string StudentName { get; set; }
        public string StudentAddress { get; set; }
        public string CourseName { get; set; }
        public string UniversityName { get; set; }
        public long ApplicationKey { get; set; }
        public string StudentMobile { get; set; }
        public string StudentEmail { get; set; }
        public string BranchName { get; set; }
        public short? BranchKey { get; set; }
        public string BranchAddress { get; set; }
        public string StudentPhoto { get; set; }   
        public string StudentPhotoPath { get; set; }

        [RequiredIfTrue("IsReceived", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "StudentEnrollmentNumberRequired")]
        public string StudentEnrollmentNo { get; set; }	
        public bool IsReceived { get; set; }
        public bool IsIssued { get; set; }	
        public DateTime? ReceivedDate { get; set; }
        public string ReceivedBy { get; set; }
        public DateTime? IssuedDate { get; set; }
        public string IssuedBy { get; set; }
        public short CourseTypeKey { get; set; }
        public long CourseKey { get; set; }
        public short UniversityKey { get; set; }
        public short BatchKey { get; set; }
        public string BatchName { get; set; }
        public DateTime? StudentDateOfAdmission { get; set; }
       public int? CourseDuration { get; set; }
        public string CurrentYearText { get; set; }
       public int? CurrentYear { get; set; }
        public short? AcademicTermKey { get; set; }
        public string BloodGroup { get; set; }
        public string Religion { get; set; }
        public string Caste { get; set; }
        public DateTime? DateOfBirth { get; set; }

    }

}
