using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IWorkScheduleService
    {
        WorkScheduleViewModel AddEditWorkSchedule(WorkScheduleViewModel model);

        WorkScheduleViewModel FIllWorkscheduleSubjectDetails(WorkScheduleViewModel model);
        WorkScheduleViewModel FillTeacher(WorkScheduleViewModel model);
        WorkScheduleViewModel FillClassDetails(WorkScheduleViewModel model);
        WorkScheduleViewModel FillBatch(WorkScheduleViewModel model);
        WorkScheduleViewModel FillSubjects(WorkScheduleViewModel model);
        WorkscheduleSubjectmodel CreateWorkSchedule(WorkscheduleSubjectmodel model);
        WorkscheduleSubjectmodel UpdateWorkSchedule(WorkscheduleSubjectmodel model);
        List<WorkscheduleSubjectmodel> GetHistoryWorkSchedule(WorkscheduleSubjectmodel model);
        WorkscheduleSubjectmodel AddEditWorkScheduleDetail(WorkscheduleSubjectmodel model);
        WorkscheduleSubjectmodel DeleteWorkSchedule(WorkscheduleSubjectmodel model);
       
    }
}
