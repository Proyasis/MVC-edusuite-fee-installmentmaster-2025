using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
   public interface IStudentLeaveService
    {
        StudentLeaveViewModel GetStudentLeaveById(StudentLeaveViewModel model);
        StudentLeaveViewModel CreateStudentLeaveDate(StudentLeaveViewModel model);
        StudentLeaveViewModel UpdateStudentLeaveDate(StudentLeaveViewModel model);
        StudentLeaveViewModel DeleteStudentLeave(long? Id);
        List<StudentLeaveViewModel> GetStudentLeaveDetails(long ApplicationKey);
    }
}
