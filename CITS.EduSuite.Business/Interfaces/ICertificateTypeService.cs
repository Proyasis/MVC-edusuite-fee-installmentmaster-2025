using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface ICertificateTypeService
    {
        CertificateTypeViewModel GetCertificateTypeById(int? id);
        CertificateTypeViewModel CreateCertificateType(CertificateTypeViewModel model);
        CertificateTypeViewModel UpdateCertificateType(CertificateTypeViewModel model);
        CertificateTypeViewModel DeleteCertificateType(CertificateTypeViewModel model);
        List<CertificateTypeViewModel> GetCertificateType(string searchText);
        CertificateTypeViewModel CheckCertificateTypeNameExists(CertificateTypeViewModel model);
    }
}
