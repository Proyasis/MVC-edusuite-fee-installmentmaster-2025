using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IStudentAbscondersService
    {
        StudentAbscondersViewModel GetStudentAbscondersById(StudentAbscondersViewModel model);
        StudentAbscondersViewModel CreateStudentAbscondersDate(StudentAbscondersViewModel model);
        StudentAbscondersViewModel UpdateStudentAbscondersDate(StudentAbscondersViewModel model);
        StudentAbscondersViewModel DeleteStudentAbsconders(long? Id);
        List<StudentAbscondersViewModel> GetStudentAbscondersDetails(long ApplicationKey);
        StudentAbscondersViewModel UpdateStatusStudentAbsconders(long? Id);
    }
}
