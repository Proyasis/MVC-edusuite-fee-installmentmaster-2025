using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
   public interface IStudyMaterialService
    {
       List<dynamic> GetApplications(ApplicationViewModel model, out long TotalRecords);
       StudyMaterialViewModel GetStudyMaterialById(StudyMaterialViewModel model);
       StudyMaterialViewModel CreateStudyMaterial(StudyMaterialViewModel model);

       StudyMaterialViewModel UpdateStudyMaterial(StudyMaterialViewModel model);

       StudyMaterialViewModel DeleteStudyMaterial(StudyMaterialDetailsModel model);
        void BindAvailableBooks(StudyMaterialViewModel model);
        void FillFeeYears(StudyMaterialViewModel model);

        StudyMaterialViewModel UpdateStudyMaterials(StudyMaterialViewModel MasterModel);
    }

}
