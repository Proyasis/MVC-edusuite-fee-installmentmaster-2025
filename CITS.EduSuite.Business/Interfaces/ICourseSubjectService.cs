using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface ICourseSubjectService
    {
        CourseSubjectMasterViewModel GetCourseSubjectById(CourseSubjectMasterViewModel model);
        CourseSubjectMasterViewModel CreateCourseSubject(CourseSubjectMasterViewModel model);
        CourseSubjectMasterViewModel UpdateCourseSubject(CourseSubjectMasterViewModel model);
        CourseSubjectMasterViewModel DeleteCourseSubjectAll(CourseSubjectMasterViewModel model);
        CourseSubjectMasterViewModel DeleteCourseSubject(long Id);
        CourseSubjectMasterViewModel DeleteStudyMaterialAll(long Id);
        CourseSubjectMasterViewModel DeleteStudyMaterial(long Id);
        List<CourseSubjectMasterViewModel> GetCourseSubject(string SearchText);
        bool CheckSubjectCodeExist(string Value, short CourseYear);
        bool CheckSubjectNameExist(string Value, short CourseYear);
        bool CheckStudyMaterialNameExist(string Value, long SubjectKey);
        bool CheckStudyMaterialCodeExist(string Value, long SubjectKey);
        CourseSubjectMasterViewModel FillCourse(CourseSubjectMasterViewModel model);
        CourseSubjectMasterViewModel FillUniversity(CourseSubjectMasterViewModel model);
        CourseSubjectMasterViewModel FillCourseYear(CourseSubjectMasterViewModel model);
        void FillCourseSubjectDetailsViewModel(CourseSubjectMasterViewModel model);
        
    }
}
