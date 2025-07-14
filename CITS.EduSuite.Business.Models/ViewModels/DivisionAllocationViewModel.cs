using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;
using System.ComponentModel.DataAnnotations;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class DivisionAllocationViewModel : BaseModel
    {
        public DivisionAllocationViewModel()
        {
            CourseTypes = new List<SelectListModel>();
            Batches = new List<SelectListModel>();
            Branches = new List<SelectListModel>();
            ClassDetails = new List<SelectListModel>();
            DivisionAllocationDetails = new List<DivisionAllocationDetailsModel>();
            IsTeacher = false;
            IfResetRollNo = false;
        }

        public List<SelectListModel> CourseTypes { get; set; }
        public List<SelectListModel> Batches { get; set; }
        public List<SelectListModel> Branches { get; set; }
        public List<SelectListModel> ClassDetails { get; set; }
        public List<DivisionAllocationDetailsModel> DivisionAllocationDetails { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ClassDetailsRequired")]
        public long ClassDetailsKey { get; set; }
        public string ClassDetailsName { get; set; }
        public short? CourseYear { get; set; }
        public string CourseYearName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BranchRequired")]
        public short BranchKey { get; set; }
        public string BranchName { get; set; }
        public short? DivisionKey { get; set; }
        public string DivisionName { get; set; }
        public int? StaffKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CourseTypeRequired")]
        public short CourseTypeKey { get; set; }
        public long? UniversityMasterKey { get; set; }
        public string UniversityName { get; set; }

        // [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CourseKeyRequired")]
        public long? CourseKey { get; set; }
        public string CourseName { get; set; }
        public short? AcademicTermKey { get; set; }
        public string AcademicTermName { get; set; }
        public bool IsTeacher { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BatchRequired")]
        public short BatchKey { get; set; }
        public string BatchName { get; set; }
        public int NoOfStudents { get; set; }

        public bool CheckStatus { get; set; }
        public string ApplicationKeys { get; set; }
        public bool IsActive { get; set; }
        public bool? IfEdit { get; set; }
        public bool? IfResetRollNo { get; set; }
        public string searchText { get; set; }
        public byte? GenderPriority { get; set; }
        public int? GenderPriorityCount { get; set; }
        public int? CourseDuration { get; set; }
    }

    public class DivisionAllocationDetailsModel
    {
        public DivisionAllocationDetailsModel()
        {
            IsActive = true;
        }

        public long RowKey { get; set; }
        public long ApplicationKey { get; set; }
        //public short DivisionKey { get; set; }
        public int? RollNumber { get; set; }
        public short StudentYear { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public string EnrollmentNo { get; set; }
        public string AdmissionNo { get; set; }
        public string StudentYearText { get; set; }
        public long StudentDivisionAllocationKey { get; set; }
        public long ClassDetailsKey { get; set; }
        public string RollNoCode { get; set; }
        public string Gender { get; set; }
        public byte? GenderKey { get; set; }
        //public bool IsDivision { get; set; }


    }
}
