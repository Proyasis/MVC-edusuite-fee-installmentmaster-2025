using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IScholarshipService
    {
        List<ScholarshipViewModel> GetScholarships(ScholarshipViewModel model, out long TotalRecords);
        ScholarshipViewModel GetScholarshipById(ScholarshipViewModel model);
        ScholarshipViewModel CreateScholarship(ScholarshipViewModel model);
        ScholarshipViewModel UpdateScholarship(ScholarshipViewModel model);
        ScholarshipViewModel FillBranches(ScholarshipViewModel model);
        ScholarshipViewModel DeleteScholarship(ScholarshipViewModel model);
        ScholarshipViewModel GetSearchDropDownLists(ScholarshipViewModel model);
        ScholarshipViewModel MoveToEnquiry(List<long> ScholarShipKeys, ScholarshipViewModel objViewModel);
        ScholarshipViewModel FillSerachBranches(ScholarshipViewModel model);
        void FillScholarshipType(ScholarshipExamScheduleViewModel model);
        //ScholarshipExamScheduleViewModel UpdateScholarshipExamSchedule(ScholarshipExamScheduleViewModel model);
    }
}
