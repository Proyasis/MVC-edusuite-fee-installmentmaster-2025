using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
   public interface IBonafiedCertificateService
    {
        List<dynamic> GetApplications(ApplicationViewModel model, out long TotalRecords);
        List<dynamic> FetchBonafiedCertificate(BonafiedCertificateViewModel model);
        BonafiedCertificateViewModel GetBonafiedCertificateById(BonafiedCertificateViewModel model);
        BonafiedCertificateViewModel CreateBonafiedCertificate(BonafiedCertificateViewModel model);
        BonafiedCertificateViewModel UpdateBonafiedCertificate(BonafiedCertificateViewModel model);
        BonafiedCertificateViewModel DeleteBonafiedCertificate(long RowKey);
    }
}
