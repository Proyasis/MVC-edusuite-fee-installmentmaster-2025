using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IStudentTCService
    {

        List<dynamic> GetApplications(ApplicationViewModel model, out long TotalRecords);
        List<dynamic> FetchTCDetails(StudentTCViewModel model);
        StudentTCViewModel GetStudentTcById(long ApplicationKey);
        StudentTCViewModel GetTCColumnDetails(StudentTCViewModel model);
        StudentTCViewModel CreateStudentTC(StudentTCViewModel model);
        StudentTCViewModel UpdateStudentTC(StudentTCViewModel model);
        StudentTCViewModel IssueStudentTC(StudentTCViewModel model);
        StudentTCDetailsViewModel DeleteStudentTCDetails(long RowKey);
        StudentTCViewModel DeleteStudentTC(long RowKey);
    }
}
