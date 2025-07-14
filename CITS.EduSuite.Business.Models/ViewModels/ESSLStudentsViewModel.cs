using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class ESSLStudentsViewModel : BaseModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string Gender { get; set; }
        public string Status { get; set; }       

    }
    public class ESSLAttendanceViewModel : BaseModel
    {
        public int EmployeeId { get; set; }
        public int AttendanceLogId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string InTime { get; set; }
        public string OutTime { get; set; }
        public string InDeviceId { get; set; }
    }

    public class ESSLStudentsDetailsViewModel : BaseModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string Gender { get; set; }
        public string Status { get; set; }
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public long? RowKey { get; set; }
        public bool IsConnected { get; set; }
        public string AdmissionNo { get; set; }

    }
}
