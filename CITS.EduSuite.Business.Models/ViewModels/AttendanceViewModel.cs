using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class AttendanceViewModel : BaseModel
    {

        public AttendanceViewModel()
        {
            Batches = new List<SelectListModel>();
            Teachers = new List<SelectListModel>();
            Branches = new List<SelectListModel>();
            AttendanceDetails = new List<AttendanceDetailsViewModel>();
            ClassDetails = new List<SelectListModel>();
            AttendanceDate = DateTimeUTC.Now;
            AttendanceTypes = new List<SelectListModel>();
            AttendanceStatus = new List<SelectListModel>();
            IfPresent = false;
        }

        public long RowKey { get; set; }
        public List<SelectListModel> Batches { get; set; }
        public List<SelectListModel> Teachers { get; set; }
        public List<SelectListModel> Branches { get; set; }
        public List<AttendanceDetailsViewModel> AttendanceDetails { get; set; }
        public List<SelectListModel> ClassDetails { get; set; }
        public List<SelectListModel> AttendanceStatus { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ClassDetailsRequired")]
        public long ClassDetailsKey { get; set; }
        public string ClassDetailsName { get; set; }
        public bool IsUpdate { get; set; }
        public DateTime AttendanceDate { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BranchRequired")]
        public short? BranchKey { get; set; }
        public string BranchName { get; set; }
        public long? EmployeeKey { get; set; }
        public bool IsTeacher { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BatchRequired")]
        public short BatchKey { get; set; }
        public string BatchName { get; set; }
        public string CourseName { get; set; }
        public int TotalCount { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int? NoOfStudents { get; set; }
        public bool IfPresent { get; set; }
        public string SearchText { get; set; }
        public List<SelectListModel> AttendanceTypes { get; set; }
        public string RollNoCode { get; set; }
        public string StudentName { get; set; }
        public string AttendanceStatusName { get; set; }
        public string AttendanceTypeName { get; set; }
        public byte? AttendanceStatusKey { get; set; }
        public DateTime? SearchFromDate { get; set; }
        public long? SlNo { get; set; }
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
    }

    public class AttendanceDetailsViewModel
    {
        public AttendanceDetailsViewModel()
        {
            AttendanceStatusDetailsViewModel = new List<AttendanceStatusDetailsViewModel>();
        }
        public long RowKey { get; set; }
        public long? ClassDetailsKey { get; set; }
        public string ClassDetailsName { get; set; }
        public int? RollNumber { get; set; }
        public string Remarks { get; set; }
        public long ApplicationKey { get; set; }
        public string AdmissionNo { get; set; }
        public string RollNoCode { get; set; }
        public string StudentName { get; set; }
        public string EnrollmentNo { get; set; }
        public string StudentEmail { get; set; }
        public string MobileNumber { get; set; }
        public string GuardianMobileNumber { get; set; }
        public DateTime? AttendanceDate { get; set; }
        public string ApplicantPhoto { get; set; }
        public string StudentPhotoPath { get; set; }
        public List<AttendanceStatusDetailsViewModel> AttendanceStatusDetailsViewModel { get; set; }
    }


    public class AttendanceStatusDetailsViewModel
    {
        public AttendanceStatusDetailsViewModel()
        {

        }
        public long AttendanceDetailRowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AttendanceTypeRequired")]
        public short AttendanceTypeKey { get; set; }
        public long AttendanceMasterKey { get; set; }
        public bool AttendanceStatus { get; set; }
        public byte AttendanceStatusKey { get; set; }
        public string AttendanceStatusCode { get; set; }
        public string AttendanceStatusColor { get; set; }
        public string Remarks { get; set; }
        public DateTime AttendanceDate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int LateMinutes { get; set; }
    }
}
