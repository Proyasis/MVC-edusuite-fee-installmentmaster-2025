using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IBaseStudentSearchService
    {
        void FillDropDownLists(BaseSearchStudentsViewModel model);
        void FillCourseTypes(BaseSearchStudentsViewModel model);
        void FillCourse(BaseSearchStudentsViewModel model);
        void FillUniversityMasters(BaseSearchStudentsViewModel model);
        void FillYears(BaseSearchStudentsViewModel model);
        void FillClassModes(BaseSearchStudentsViewModel model);
        void FillReligions(BaseSearchStudentsViewModel model);
        void FillSecondLanguages(BaseSearchStudentsViewModel model);
        void FillMediums(BaseSearchStudentsViewModel model);
        void FillIncomes(BaseSearchStudentsViewModel model);
        void FillNatureOfEnquiry(BaseSearchStudentsViewModel model);
        void FillAgents(BaseSearchStudentsViewModel model);
        void FillRegistrationCatagory(BaseSearchStudentsViewModel model);
        void FillCaste(BaseSearchStudentsViewModel model);
        void FillCommunityType(BaseSearchStudentsViewModel model);
        void FillBloodGroup(BaseSearchStudentsViewModel model);

    }
}
