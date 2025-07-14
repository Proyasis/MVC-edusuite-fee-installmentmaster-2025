using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IApplicationFamilyDetailsService
    {
        ApplicationFamilyDetailsViewModel GetApplicationFamilyDetailsById(Int64 EmployeeId);

        ApplicationFamilyDetailsViewModel UpdateApplicationFamilyDetails(ApplicationFamilyDetailsViewModel model);

        ApplicationFamilyDetailsViewModel DeleteApplicationFamilyDetails(Int64 ApplicationKey);

       // ApplicationFamilyDetailsViewModel CreateGuardianUserAccount(ApplicationFamilyDetailsViewModel model);

       // ApplicationFamilyDetailsViewModel UpdateGuardianUserAccount(ApplicationFamilyDetailsViewModel model);


        ApplicationFamilyDetailsViewModel CheckUserNameExists(string UserName, int RowKey);
    }
}
