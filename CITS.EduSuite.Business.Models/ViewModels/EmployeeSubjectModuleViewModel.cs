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
    public class EmployeeSubjectModuleViewModel : BaseModel
    {
        public EmployeeSubjectModuleViewModel()
        {
            EmployeeSubjectDetailsModel = new List<EmployeeSubjectDetailsModel>();
        }
        public long? EmployeesKey { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string DesignationName { get; set; }

        public List<EmployeeSubjectDetailsModel> EmployeeSubjectDetailsModel { get; set; }
    }
    public class EmployeeSubjectDetailsModel : BaseModel
    {
        public EmployeeSubjectDetailsModel()
        {

            ModulesList = new List<string>();
        }

        public long RowKey { get; set; }
        public long? TeacherClassAllocationKey { get; set; }
        public long? EmployeeKey { get; set; }

        public long? ClassDetailsKey { get; set; }
        public string ClassDetailsName { get; set; }
        public long? SubjectKey { get; set; }
        public string SubjectName { get; set; }
        public string SubjectCode { get; set; }

        public long? ModuleKey { get; set; }
        public string Modulename { get; set; }
        public bool IsActive { get; set; }
        public int? ModuleCount { get; set; }
        public int? TotalModuleCount { get; set; }

        public List<string> ModulesList { get; set; }

    }
}
