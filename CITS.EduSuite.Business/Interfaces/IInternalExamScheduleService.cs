using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IInternalExamScheduleService
    {
        InternalExamViewModel GetInternalExamScheduleById(InternalExamViewModel model);

        InternalExamViewModel CreateInternalExamSchedule(InternalExamViewModel model);

        InternalExamViewModel UpdateInternalExamSchedule(InternalExamViewModel model);

        InternalExamViewModel DeleteInternalExamSchedule(InternalExamViewModel model);

        List<InternalExamViewModel> GetInternalExamSchedule(InternalExamViewModel model);

        void FillInternalExamDetailsViewModel(InternalExamViewModel model);

        InternalExamViewModel FillCourse(InternalExamViewModel model);

        InternalExamViewModel FillUniversity(InternalExamViewModel model);

        InternalExamViewModel FillBatch(InternalExamViewModel model);

        InternalExamViewModel FillCourseYear(InternalExamViewModel model);
        void FillDropDown(InternalExamViewModel model);
        InternalExamViewModel ResetInternalExamSchedule(long Id, long InternalExamKey);
        InternalExamViewModel FillClassDetails(InternalExamViewModel model);
        InternalExamViewModel GetSearchDropdownList(InternalExamViewModel model);

        InternalExamViewModel FillSearchBatch(InternalExamViewModel model);
    }
}
