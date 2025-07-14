using CITS.EduSuite.Business.Models.Resources;
using CITS.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class WorkScheduleViewModel : BaseModel
    {
        public WorkScheduleViewModel()
        {
            WorkscheduleSubjectmodel = new List<WorkscheduleSubjectmodel>();
            Batches = new List<SelectListModel>();
            Branches = new List<SelectListModel>();
            Subjects = new List<SelectListModel>();
            Employees = new List<SelectListModel>();
            ClassDetails = new List<SelectListModel>();
        }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ClassDetailsRequired")]
        public long ClassDetailsKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SubjectRequired")]
        public long SubjectKey { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BranchRequired")]
        public short BranchKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BatchRequired")]
        public short BatchKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployeeRequired")]
        public long? EmployeeKey { get; set; }
        public string EmployeeName { get; set; }
        
        

        public List<SelectListModel> Branches { get; set; }
        public List<SelectListModel> Batches { get; set; }
        public List<SelectListModel> Subjects { get; set; }
        public List<SelectListModel> ClassDetails { get; set; }
        public List<SelectListModel> Employees { get; set; }

        public long? ScheduledEmployeeKey { get; set; }
        public List<WorkscheduleSubjectmodel> WorkscheduleSubjectmodel { get; set; }
    }

    public class WorkscheduleSubjectmodel : BaseModel
    {
        public WorkscheduleSubjectmodel()
        {
            WorkScheduleDate = DateTimeUTC.Now;
        }

        public long MasterRowKey { get; set; }
        public long? SubjectModuleKey { get; set; }
        public string ModuleName { get; set; }
        public long? TopicKey { get; set; }
        public string TopicName { get; set; }

        public int? ProgressStatus { get; set; }
        public int? TotalWorkDuration { get; set; }
        public short? BatchKey { get; set; }
        public short? BranchKey { get; set; }
        public long? SubjectKey { get; set; }
        public string SubjectName { get; set; }
        public long? ClassDetailsKey { get; set; }
        public long RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "WorkScheduleDateRequired")]
        public DateTime WorkScheduleDate { get; set; }
        public long? TecherScheduleMasterKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DurationRequired")]
        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "WorkDurationExpressionErrorMessage")]
        public int? Duration { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "WorkProgressStatusRequired")]
        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "WorkProgressStatusExpressionErrorMessage")]
        public int? CurrentProgressStatus { get; set; }
        public int? OldCurrentProgressStatus { get; set; }
        public TimeSpan? TimeIn { get; set; }

        [GreaterThan("TimeIn", PassOnNull = true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExamStartTimeLessThanErrorMessage")]
        public TimeSpan? TimeOut { get; set; }
        public long EmployeeKey { get; set; }
        public long? AppUserKey { get; set; }
        public string EmployeeName { get; set; }
        public string BatchName { get; set; }
        public string ClassDetailsName { get; set; }
        public string BranchName { get; set; }


    }

}
