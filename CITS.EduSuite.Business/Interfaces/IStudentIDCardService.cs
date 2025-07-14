using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;


namespace CITS.EduSuite.Business.Interfaces
{
    public interface IStudentIDCardService
    {
        List<StudentIDCardList> GetStudentIDCards(StudentIDCardViewModels model);
        StudentIDCardViewModels CreateStudentIDCard(StudentIDCardViewModels model);
        StudentIDCardViewModels ResetStudentIDCardList(StudentIDCardViewModels model);
        StudentIDCardViewModels GetCourseByCourseType(StudentIDCardViewModels model);
        StudentIDCardViewModels GetUniversityByCourse(StudentIDCardViewModels model);
        void FillDrodownLists(StudentIDCardViewModels model);
    }
}
