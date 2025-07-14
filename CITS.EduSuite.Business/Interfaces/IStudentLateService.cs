using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
   public interface IStudentLateService
    {
        StudentLateViewModel GetStudentLateById(StudentLateViewModel model);
        StudentLateViewModel CreateStudentLateDate(StudentLateViewModel model);
        StudentLateViewModel UpdateStudentLateDate(StudentLateViewModel model);
        StudentLateViewModel DeleteStudentLate(long? Id);
        List<StudentLateViewModel> GetStudentLateDetails(long ApplicationKey);
        StudentLateViewModel GetLateMinuteById(short Id, DateTime Date, long ApplicationKey);

    }
}
