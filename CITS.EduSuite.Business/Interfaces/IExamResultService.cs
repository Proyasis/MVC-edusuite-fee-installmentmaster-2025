using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IExamResultService
    {
        ExamResultViewModel UpdateExamResult(ExamResultViewModel model);
        ExamResultViewModel GetExamResultDetails(ExamResultViewModel model);
        List<ExamResultViewModel> GetExamResult(ExamResultViewModel model, out long TotalRecords);
        ExamResultViewModel StudentMarkDetils(ExamResultViewModel model);
        ExamResultViewModel DeleteExamResult(long? ExamScheduleKey, long? SubjectKey);
        ExamResultViewModel ResetExamResult(long? ExamResultKey);
        List<ApplicationViewModel> GetApplications(ApplicationViewModel model, out long TotalRecords);
        ExamResultViewModel StudentMarkDetilsByIndividual(ExamResultViewModel model);
        ExamResultViewModel UpdateExamResults(ExamResultViewModel MasterModel);
        
    }
}
