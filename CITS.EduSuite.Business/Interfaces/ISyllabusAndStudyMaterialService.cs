using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;
namespace CITS.EduSuite.Business.Interfaces
{
    public interface ISyllabusAndStudyMaterialService
    {
        List<SyllabusAndStudyMaterialViewModel> GetSubject(SyllabusAndStudyMaterialViewModel model, out long TotalRecords);
        SyllabusAndStudyMaterialViewModel GetStudyMaterialById(long SubjectKey);
        SyllabusAndStudyMaterialViewModel UpdateStudyMaterials(SyllabusAndStudyMaterialViewModel model);
        SyllabusAndStudyMaterialViewModel CheckStudyMaterialNameExist(StudyMaterialModel model);
        SyllabusAndStudyMaterialViewModel CheckStudyMaterialCodeExist(StudyMaterialModel model);
        SyllabusAndStudyMaterialViewModel DeleteStudyMaterial(long Id);
        void FillDropDown(SyllabusAndStudyMaterialViewModel model);

        SyllabusAndStudyMaterialViewModel FillUniversity(SyllabusAndStudyMaterialViewModel model);
        SyllabusAndStudyMaterialViewModel FillCourseYear(SyllabusAndStudyMaterialViewModel model);
        #region Subject Modules
        SyllabusAndStudyMaterialViewModel GetSubjectModulesById(long SubjectKey);       
        SyllabusAndStudyMaterialViewModel UpdateSubjectModules(SyllabusAndStudyMaterialViewModel model);
        //SyllabusAndStudyMaterialViewModel CheckStudyMaterialNameExist(StudyMaterialModel model);
        //SyllabusAndStudyMaterialViewModel CheckStudyMaterialCodeExist(StudyMaterialModel model);
        SyllabusAndStudyMaterialViewModel DeleteSubjectModule(long Id);
        SyllabusAndStudyMaterialViewModel DeleteModuleTopic(long Id);

        SyllabusAndStudyMaterialViewModel DeleteModuleTopicAll(long Id);
        #endregion Subject Modules

    }
}
