using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
   public interface ICompanyService
    {
        List<CompanyViewModel> GetCompanies(string searchText);
        CompanyViewModel GetCompanyById(short? id);
        CompanyViewModel UpdateCompany(CompanyViewModel model);
        CompanyViewModel DeleteCompanyLogo(short Id);
    }
}
