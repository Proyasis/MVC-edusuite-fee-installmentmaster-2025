using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IUnitTestScheduleService
    {
        UnitTestScheduleViewModel GetUnitTestScheduleById(UnitTestScheduleViewModel model);
        UnitTestScheduleViewModel FillUnitTestDetailsViewModel(UnitTestScheduleViewModel model);
        UnitTestScheduleViewModel CreateUnitTestSchedule(UnitTestScheduleViewModel model);
        UnitTestScheduleViewModel UpdateUnitTestSchedule(UnitTestScheduleViewModel model);
        List<UnitTestScheduleViewModel> GetUnitTestSchedule(UnitTestScheduleViewModel model, out long TotalRecords);
        UnitTestScheduleViewModel FillSubjectModules(UnitTestScheduleViewModel model);
        UnitTestScheduleViewModel FillModuleTopics(UnitTestScheduleViewModel model);
        UnitTestScheduleViewModel FillBatch(UnitTestScheduleViewModel model);
        UnitTestScheduleViewModel FillClassDetails(UnitTestScheduleViewModel model);
        UnitTestScheduleViewModel DeleteUnitTest(UnitTestScheduleViewModel model);
        UnitTestScheduleViewModel GetSearchDropdownList(UnitTestScheduleViewModel model);
        UnitTestScheduleViewModel FillSearchClassDetails(UnitTestScheduleViewModel model);
        UnitTestScheduleViewModel FillSearchBatch(UnitTestScheduleViewModel model);
        UnitTestScheduleViewModel FillSubjects(UnitTestScheduleViewModel model);
        UnitTestScheduleViewModel FillCourseType(UnitTestScheduleViewModel model);
    }
}
