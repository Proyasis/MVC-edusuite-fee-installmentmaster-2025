using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface ICourseCompletionCertificateService
    {
        List<dynamic> GetApplications(ApplicationViewModel model, out long TotalRecords);
        List<dynamic> FetchCourseCompleteCertificate(CourseCompletionCertificateViewModel model);
        CourseCompletionCertificateViewModel GetCourseCompletionCertificateById(CourseCompletionCertificateViewModel model);
        CourseCompletionCertificateViewModel CreateCourseCompletionCertificate(CourseCompletionCertificateViewModel model);
        CourseCompletionCertificateViewModel UpdateCourseCompletionCertificate(CourseCompletionCertificateViewModel model);
        CourseCompletionCertificateViewModel DeleteCourseCompletionCertificate(long RowKey);
    }
}
