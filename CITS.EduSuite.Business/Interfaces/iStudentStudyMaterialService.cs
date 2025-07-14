using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
   public interface  IStudentStudyMaterialService
    {
        List<StudentStudyMaterialViewModel> GetStudyMaterials(StudentStudyMaterialViewModel model, out long TotalRecords);
        StudentStudyMaterialViewModel GetStudyMaterialById(StudentStudyMaterialViewModel model);
        StudentStudyMaterialViewModel CreateStudyMaterial(StudentStudyMaterialViewModel model);
        StudentStudyMaterialViewModel UpdateStudyMaterial(StudentStudyMaterialViewModel model);
        void FillDropDownList(StudentStudyMaterialViewModel model);
        StudentStudyMaterialViewModel DeleteStudyMaterial(StudentStudyMaterialViewModel model);

        List<StudentStudyMaterialViewModel> GetStudyMaterialByStudyMaterialKey(StudentStudyMaterialViewModel model);
        StudentStudyMaterialViewModel GetStudyMaterialCategories(StudentStudyMaterialViewModel model);
        StudentStudyMaterialViewModel GetStudyMaterialList(StudentStudyMaterialViewModel model);
        StudentStudyMaterialViewModel DeleteStudyMaterialDetails(long? RowKey);
    }
}
