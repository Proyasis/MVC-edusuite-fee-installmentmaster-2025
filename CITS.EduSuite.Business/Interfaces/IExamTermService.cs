using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IExamTermService
    {
        ExamTermViewModel GetExamTermById(int? id);
        ExamTermViewModel CreateExamTerm(ExamTermViewModel model);
        ExamTermViewModel UpdateExamTerm(ExamTermViewModel model);
        ExamTermViewModel DeleteExamTerm(ExamTermViewModel model);
        List<ExamTermViewModel> GetExamTerm(string searchText);
        ExamTermViewModel CheckExamTermNameExists(ExamTermViewModel model);
    }
}
