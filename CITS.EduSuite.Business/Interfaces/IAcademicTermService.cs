using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
   public interface IAcademicTermService
    {
        AcademicTermViewModel GetAcademicTermById(int? id);
        AcademicTermViewModel CreateAcademicTerm(AcademicTermViewModel model);
        AcademicTermViewModel UpdateAcademicTerm(AcademicTermViewModel model);
        AcademicTermViewModel DeleteAcademicTerm(AcademicTermViewModel model);
        List<AcademicTermViewModel> GetAcademicTerm(string searchText);
    }
}
