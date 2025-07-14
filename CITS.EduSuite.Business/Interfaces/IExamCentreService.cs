using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IExamCentreService
    {
        ExamCentreViewModel GetExamCentreById(int? id);
        ExamCentreViewModel CreateExamCentre(ExamCentreViewModel model);
        ExamCentreViewModel UpdateExamCentre(ExamCentreViewModel model);
        ExamCentreViewModel DeleteExamCentre(ExamCentreViewModel model);
        List<ExamCentreViewModel> GetExamCentre(string searchText);
        ExamCentreViewModel CheckExamCentreCodeExists(ExamCentreViewModel model);
        ExamCentreViewModel CheckExamCentreNameExists(ExamCentreViewModel model);
    }
}
