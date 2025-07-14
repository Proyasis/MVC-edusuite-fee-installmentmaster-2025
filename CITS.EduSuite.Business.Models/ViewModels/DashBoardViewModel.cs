using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class DashBoardViewModel : BaseModel
    {
        public DashBoardViewModel()
        {
            Branches = new List<SelectListModel>();
            Employees = new List<SelectListModel>();
            AppUsers = new List<SelectListModel>();
            DashBoardTypes = new List<SelectListModel>();
        }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BranchRequired")]
        public short? BranchKey { get; set; }
        public string BranchName { get; set; }
        public List<SelectListModel> Branches { get; set; }
        public List<SelectListModel> Employees { get; set; }
        public List<SelectListModel> AppUsers { get; set; }

        public long? EmployeeKey { get; set; }
        public long? AppUserKey { get; set; }
        public string EmployeeName { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string FetchType { get; set; }

        public short? DashBoardTypeKey { get; set; }
        public List<SelectListModel> DashBoardTypes { get; set; }

    }
}
