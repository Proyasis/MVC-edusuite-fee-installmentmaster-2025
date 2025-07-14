using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IEmployeeContactService
    {

        EmployeeContactViewModel GetEmployeeContactById(Int64 EmployeeId);

        EmployeeContactViewModel UpdateEmployeeContact(EmployeeContactViewModel model);

        EmployeeContactViewModel DeleteEmployeeContact(ContactViewModel model);

        EmployeeContactViewModel CheckAddressTypeExists(Int16 AddressTypeKey, Int64 EmployeeKey, Int64 RowKey);

        ContactViewModel GetProvinceByCountry(ContactViewModel model);
        ContactViewModel GetDistrictByProvince(ContactViewModel model);
    }
}
