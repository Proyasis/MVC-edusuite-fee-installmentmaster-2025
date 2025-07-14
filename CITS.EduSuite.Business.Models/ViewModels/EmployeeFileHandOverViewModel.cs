using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class EmployeeFileHandOverViewModel : BaseModel
    {
        public EmployeeFileHandOverViewModel()
        {
            Employees = new List<GroupSelectListModel>();
            FileHandoverTypes = new List<SelectListModel>();
            IfFeedback = false;
        }
        public long RowKey { get; set; }

        public int HandoverType { get; set; }
        public long EmployeeFromKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployeeRequired")]
        public long EmployeeToKey { get; set; }
        public long EmployeeName { get; set; }
        public byte? FileHandoverTypeKey { get; set; }
        public string FileHandoverTypeName { get; set; }
        public long FileKey { get; set; }
        public string FileName { get; set; }
        public string FileEmail { get; set; }
        public string FileMobile { get; set; }
        public string FileStatusName { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateAdded { get; set; }
        public List<GroupSelectListModel> Employees { get; set; }
        public List<SelectListModel> FileHandoverTypes { get; set; }
        public string Remarks { get; set; }
        public bool? IfFeedback { get; set; }

    }
}
