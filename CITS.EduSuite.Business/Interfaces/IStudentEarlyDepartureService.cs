using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
   public interface IStudentEarlyDepartureService
    {
        StudentEarlyDepartureViewModel GetStudentEarlyDepartureById(StudentEarlyDepartureViewModel model);
        StudentEarlyDepartureViewModel CreateStudentEarlyDepartureDate(StudentEarlyDepartureViewModel model);
        StudentEarlyDepartureViewModel UpdateStudentEarlyDepartureDate(StudentEarlyDepartureViewModel model);
        StudentEarlyDepartureViewModel DeleteStudentEarlyDeparture(long? Id);
        List<StudentEarlyDepartureViewModel> GetStudentEarlyDepartureDetails(long ApplicationKey);
    }
}
