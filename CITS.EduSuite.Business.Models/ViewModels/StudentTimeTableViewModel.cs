using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class StudentTimeTableViewModel : BaseModel
    {
        public StudentTimeTableViewModel()
        {
            ClassDetails = new List<SelectListModel>();
            WeeklyPeriods = new List<SelectListModel>();
            WeekDays = new List<SelectListModel>();
           // StudentTimeTableDetailsModel = new List<StudentTimeTableDetailsModel>();
            Subjects = new List<SelectListModel>();
            Employees = new List<SelectListModel>();
            Branches = new List<SelectListModel>();
        }

        public long RowKey { get; set; }       
        public long ClassDetailsKey { get; set; }
        public List<SelectListModel> ClassDetails { get; set; }
        public List<SelectListModel> WeeklyPeriods { get; set; }
        public List<SelectListModel> WeekDays { get; set; }        

        //public List<StudentTimeTableDetailsModel> StudentTimeTableDetailsModel { get; set; }

        public List<SelectListModel> Subjects { get; set; }
        public List<SelectListModel> Employees { get; set; }
        public List<SelectListModel> Branches { get; set; }

        //public long RowKey { get; set; }
        public long? SubjectKey { get; set; }
        public string SubjectName { get; set; }
        public long? EmployeeKey { get; set; }
        public string EmployeeName { get; set; }
        public byte Day { get; set; }
        public byte PeriodKey { get; set; }
        public bool IsActive { get; set; }
        public short? BranchKey { get; set; }
        public string BranchName { get; set; }
    }

    //public class StudentTimeTableDetailsModel
    //{
    //    public StudentTimeTableDetailsModel()
    //    {
    //        Subjects = new List<SelectListModel>();
    //        Employees = new List<SelectListModel>();
           
    //    }
    //    public long RowKey { get; set; }
    //    public long SubjectKey { get; set; }
    //    public string SubjectName { get; set; }
    //    public long EmployeeKey { get; set; }
    //    public string EmployeeName { get; set; }
    //    public byte Day { get; set; }
    //    public byte PeriodKey { get; set; }

    //    public bool IsActive { get; set; }
    //    public long MasterKey { get; set; }

    //    public List<SelectListModel> Subjects { get; set; }
    //    public List<SelectListModel> Employees { get; set; }
      


    //}
}
