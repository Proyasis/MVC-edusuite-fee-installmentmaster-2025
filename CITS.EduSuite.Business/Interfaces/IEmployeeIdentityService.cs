using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IEmployeeIdentityService
    {

        EmployeeIdentityViewModel GetEmployeeIdentitiesById(Int64 EmployeeId);

        EmployeeIdentityViewModel UpdateEmployeeIdentity(EmployeeIdentityViewModel model);

        EmployeeIdentityViewModel CheckIdentityTypeExists(Int16 IdentityTypeKey, Int64 EmployeeKey, Int64 RowKey);

        EmployeeIdentityViewModel CheckAdharNumberExists(string IdentyUniqueID, Int64 RowKey);

        EmployeeIdentityViewModel DeleteEmployeeIdentity(IdentityViewModel model);
    }
}
