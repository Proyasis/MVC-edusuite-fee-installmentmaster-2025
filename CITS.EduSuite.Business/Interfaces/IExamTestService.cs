using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IExamTestService
    {
        List<ExamTestViewModel> GetExamTestsAll(ExamTestViewModel model, out long TotalRecords);
        //List<ExamTestViewModel> GetExamTests();
        ExamTestViewModel GetExamTestById(ExamTestViewModel model);
        void InitializeData(ExamTestViewModel model);
        ExamTestViewModel CreateExamTest(ExamTestViewModel model);
        ExamTestViewModel UpdateExamTest(ExamTestViewModel model);
        dynamic GetExamReviewById(long id);
        dynamic GetExamTestsById(long ApplicationKey);
        ExamTestViewModel UpdateExamStatus(ExamTestViewModel model);
        ExamTestViewModel UpdateExamKey(ExamTestViewModel model);
        ExamTestViewModel GetExamTests(ExamTestViewModel model);
        bool IsFeePaid();
    }
}
