using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
   public interface IStudentStatusService
    {
        StudentStatusViewModel GetStudentStatusById(int? id);
        StudentStatusViewModel CreateStudentStatus(StudentStatusViewModel model);
        StudentStatusViewModel UpdateStudentStatus(StudentStatusViewModel model);
        StudentStatusViewModel DeleteStudentStatus(StudentStatusViewModel model);
        List<StudentStatusViewModel> GetStudentStatus(string searchText);
    }
}
