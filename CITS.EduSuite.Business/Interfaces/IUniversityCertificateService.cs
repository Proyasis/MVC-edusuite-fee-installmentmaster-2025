using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;


namespace CITS.EduSuite.Business.Interfaces
{
   public interface IUniversityCertificateService
    {
        List<ApplicationViewModel> GetApplications(ApplicationViewModel model, out long TotalRecords);
        UniversityCertificateViewModel GetUniversityCertificateById(Int64 ApplicationKey);
        UniversityCertificateViewModel UpdateUniversityCertificate(UniversityCertificateViewModel model);

        UniversityCertificateViewModel DeleteUniversityCertificate(Int64 EducationQualificationKey);
        UniversityCertificateViewModel UpdateUniversityCertificates(UniversityCertificateViewModel MasterModel);
    }
}
