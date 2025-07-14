using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
   public interface IInternalExamTermService
    {
        InternalExamTermViewModel GetInternalExamTermById(int? id);
        InternalExamTermViewModel CreateInternalExamTerm(InternalExamTermViewModel model);
        InternalExamTermViewModel UpdateInternalExamTerm(InternalExamTermViewModel model);
        InternalExamTermViewModel DeleteInternalExamTerm(InternalExamTermViewModel model);
        List<InternalExamTermViewModel> GetInternalExamTerm(string searchText);
    }
}
