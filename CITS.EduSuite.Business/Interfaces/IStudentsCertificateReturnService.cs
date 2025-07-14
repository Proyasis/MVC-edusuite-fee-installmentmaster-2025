using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;


namespace CITS.EduSuite.Business.Interfaces
{
    public interface IStudentsCertificateReturnService
    {
        List<ApplicationViewModel> GetApplications(ApplicationViewModel model, out long TotalRecords);
        StudentsCertificateReturnViewModel GetStudentsCertificateById(StudentsCertificateReturnViewModel model);
        StudentsCertificateReturnViewModel UpdateStudentsCertificateProcess(StudentsCertificateReturnViewModel model);

        StudentsCertificateReturnViewModel DeleteStudentsCertificateProcess(StudentsCertificateReturnDetail model);

        List<StudentsCertificateReturnDetail> GetCertificateDetailsByApplication(long ApplicationKey);
    }
}
