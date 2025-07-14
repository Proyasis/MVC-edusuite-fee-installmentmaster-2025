using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class StudentEarlyDepartureViewModel : BaseModel
    {
        public StudentEarlyDepartureViewModel()
        {
            EarlyDepartureDate = DateTimeUTC.Now;
            AttendanceTypes = new List<SelectListModel>();
        }

        public long RowKey { get; set; }
        public long? ApplicationsKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Date", ResourceType = typeof(EduSuiteUIResources))]
        public DateTime? EarlyDepartureDate { get; set; }
        public TimeSpan? EarlyDepartureTime { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Remarks", ResourceType = typeof(EduSuiteUIResources))]
        public string Remarks { get; set; }
        public string AttachmentPath { get; set; }
        public string StudentEmail { get; set; }
        public string StudentMobile { get; set; }
        public short? AttendanceTypeKey { get; set; }
        public List<SelectListModel> AttendanceTypes { get; set; }
        public string StudentGuardianPhone { get; set; }
        public string ClassCode { get; set; }
        public bool MarkHalfDay { get; set; }
    }
}
