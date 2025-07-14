using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;


namespace CITS.EduSuite.Business.Interfaces
{
    public interface IExamScheduleService
    {
        ExamScheduleViewModel GetExamScheduleById(ExamScheduleViewModel model);

        //ExamScheduleViewModel CreateExamSchedule(ExamScheduleViewModel model);

        ExamScheduleViewModel UpdateExamSchedule(ExamScheduleViewModel model);

        ExamScheduleViewModel DeleteExamSchedule(ExamScheduleViewModel model);

        List<ExamScheduleViewModel> GetExamSchedule(ExamScheduleViewModel model, out long TotalRecords);

        void FillExamDetailsViewModel(ExamScheduleViewModel model);

        ExamScheduleViewModel FillCourse(ExamScheduleViewModel model);

        ExamScheduleViewModel FillUniversity(ExamScheduleViewModel model);

        ExamScheduleViewModel FillBatch(ExamScheduleViewModel model);

        ExamScheduleViewModel FillCourseYear(ExamScheduleViewModel model);
        void FillDropDown(ExamScheduleViewModel model);
        ExamScheduleViewModel ResetExamSchedule(long Id);

        ExamScheduleViewModel FillSubjects(ExamScheduleViewModel model);

        List<ApplicationViewModel> GetApplications(ApplicationViewModel model, out long TotalRecords);

        ExamScheduleViewModel GetExamScheduleByIndividualId(ExamScheduleViewModel model);
        void FillExamDetailsIndividualViewModel(ExamScheduleViewModel model);

        #region Old Code

        //List<ExamScheduleList> GetExamScheduleStudentsList(ExamScheduleViewModel model);
        //ExamScheduleViewModel CreateExamSchedule(ExamScheduleViewModel model);
        //ExamScheduleViewModel GetCourseTypeBySyllabus(ExamScheduleViewModel model);
        //ExamScheduleViewModel GetCourseByCourseType(ExamScheduleViewModel model);
        //ExamScheduleViewModel GetUniversityByCourse(ExamScheduleViewModel model);
        //ExamScheduleViewModel GetBatches(ExamScheduleViewModel model);
        //ExamScheduleViewModel GetExamSubjects(ExamScheduleViewModel model);
        //ExamScheduleViewModel GetYears(ExamScheduleViewModel model);
        //ExamScheduleViewModel DeleteExamSchedule(ExamScheduleViewModel model);
        //void FillDrodownLists(ExamScheduleViewModel model);

        #endregion Old Code
    }
}
