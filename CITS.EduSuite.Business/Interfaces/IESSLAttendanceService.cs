using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
   public interface IESSLAttendanceService
    {
        DataTable PullAttendanceData(ESSLAttendanceViewModel model);
        ESSLStudentsViewModel GetEmployeeDetails(List<ESSLStudentsViewModel> StudentsList1);
        ESSLAttendanceViewModel GetAttendanceDetails(List<ESSLAttendanceViewModel> AttendanceList);
        List<ESSLStudentsDetailsViewModel> GetESSLStudents(ESSLStudentsDetailsViewModel model, out long TotalRecords);
    }
}
