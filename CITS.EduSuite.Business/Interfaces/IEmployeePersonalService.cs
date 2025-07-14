using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IEmployeePersonalService
    {
      

         EmployeePersonalViewModel GetEmployeePersonalById(Int64? id);

         EmployeePersonalViewModel CreateEmployeePersonal(EmployeePersonalViewModel model);

         EmployeePersonalViewModel UpdateEmployeePersonal(EmployeePersonalViewModel model);

      
         //EmployeePersonalViewModel GetDepartmentByBranchId(Int16 BranchKey);

        EmployeePersonalViewModel GetGradeByDesignationId(Int16 GradeKey);

        EmployeePersonalViewModel GetGradeDetailsById(Int32 DesignationKey);

         EmployeePersonalViewModel GetHigherEmployeesById(EmployeePersonalViewModel model);

         EmployeePersonalViewModel CheckEmployeeCodeExists(string EmployeeCode, Int64 RowKey);

         EmployeePersonalViewModel CheckMobileNumberExists(string MobileNumber, Int64 RowKey);
         EmployeePersonalViewModel CheckEmailAddressExists(string EmailAddress, Int64 RowKey);
         ContactVerificationViewModel UpdateContactVerification(ContactVerificationViewModel model);
         ContactVerificationViewModel ConfirmContactVerification(ContactVerificationViewModel model);
    }
}
