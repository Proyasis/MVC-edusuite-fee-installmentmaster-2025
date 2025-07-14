using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IDesignationGradeService
    {
        List<DesignationGradeViewModel> GetDesignationGrades(string searchText);
        List<DesignationGradeDetailViewModel> GetDesignationGradeDetailsById(short Id);

        DesignationGradeViewModel GetDesignationGradebyId(int Id);

        DesignationGradeViewModel UpdateDesignationGrade(List<DesignationGradeViewModel> modelList);

        DesignationGradeViewModel UpdateDesignationGradeName(DesignationGradeViewModel model);

        DesignationGradeViewModel GetDesignations(DesignationGradeViewModel model);

        DesignationGradeViewModel DeleteDesignationGrade(DesignationGradeViewModel model);

        DesignationGradeViewModel DeleteDesignationGradeDetail(DesignationGradeViewModel model);

 

    }
}
