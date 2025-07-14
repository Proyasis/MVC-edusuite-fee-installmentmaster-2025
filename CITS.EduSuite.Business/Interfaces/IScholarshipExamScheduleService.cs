using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IScholarshipExamScheduleService
    {
        List<ScholarshipExamScheduleViewModel> GetScholarshipExamSchedules(ScholarshipExamScheduleViewModel model, out long TotalRecords);
        //ScholarshipExamScheduleViewModel GetScholarshipExamScheduleById(ScholarshipExamScheduleViewModel model);
        //ScholarshipExamScheduleViewModel CreateScholarshipExamSchedule(ScholarshipExamScheduleViewModel model);
        ScholarshipExamScheduleViewModel UpdateScholarshipExamSchedule(ScholarshipExamScheduleViewModel model);
        ScholarshipExamScheduleViewModel FillBranches(ScholarshipExamScheduleViewModel model);
        ScholarshipExamScheduleViewModel DeleteScholarshipExamSchedule(ScholarshipExamScheduleViewModel model);
        ScholarshipExamScheduleViewModel GetSearchDropDownLists(ScholarshipExamScheduleViewModel model);

        ScholarshipExamScheduleViewModel FillSerachBranches(ScholarshipExamScheduleViewModel model);
        void FillScholarshipType(ScholarshipExamScheduleViewModel model);
        //void FillScholarshipExamScheduleDetails(ScholarshipExamScheduleViewModel model);
        string GetPrintHallTicket(long? RowKey, int Type);

        ScholarshipExamScheduleViewModel CreateScholarshipExamSchedule(ScholarshipExamScheduleViewModel objViewModel);

        ScholarshipExamScheduleViewModel FillSubBranches(ScholarshipExamScheduleViewModel model);

        long getScholarshipid(ScholarshipExamScheduleViewModel model);

        List<ScholarshipExamScheduleViewModel> GetScholarshipExamResult(ScholarshipExamScheduleViewModel model, out long TotalRecords);
        ScholarshipExamResultViewModel GetScholarshipExamResultDetails(ScholarshipExamResultViewModel model, List<long> ScholarshipKeys);

        ScholarshipExamResultViewModel UpdateScholarshipExamResult(ScholarshipExamResultViewModel model);
        ScholarshipExamResultViewModel UpdateBulkScholarshipExamResult(List<ScholarshipExamDetails> modelList);
    }
}
