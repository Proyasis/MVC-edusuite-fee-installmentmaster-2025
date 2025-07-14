using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class EmplyeeHeirarchyViewModel : BaseModel
    {
        public EmplyeeHeirarchyViewModel()
        {
            EmployeeHeirarchyDetails = new List<EmployeeHeirarchyDetailsModel>();
            Employees = new List<SelectListModel>();
        }
        public long? EmployeeKey { get; set; }
        public string EmployeeName { get; set; }
        public List<EmployeeHeirarchyDetailsModel> EmployeeHeirarchyDetails { get; set; }
        public List<SelectListModel> Employees { get; set; }
    }
    public class EmployeeHeirarchyDetailsModel
    {
        public long? RowKey { get; set; }
        public long? ToEmployeeKey { get; set; }
        public string ToEmployeeName { get; set; }

        public bool IsActive { get; set; }
        public bool DataAccess { get; set; }
    }
}
