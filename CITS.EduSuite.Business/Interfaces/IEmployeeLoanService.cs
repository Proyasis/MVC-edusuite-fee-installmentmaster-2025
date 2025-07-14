using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;
namespace CITS.EduSuite.Business.Interfaces
{
   public interface IEmployeeLoanService
    {

       List<EmployeeLoanViewModel> GetEmployeeLoan(EmployeeLoanViewModel model);
       EmployeeLoanViewModel GetEmployeeLoanById(EmployeeLoanViewModel model);

        EmployeeLoanViewModel CreateEmployeeLoan(EmployeeLoanViewModel model);

        EmployeeLoanViewModel UpdateEmployeeLoan(EmployeeLoanViewModel model);

        EmployeeLoanViewModel DeleteEmployeeLoan(EmployeeLoanViewModel model);

        EmployeeLoanViewModel GetBranches(EmployeeLoanViewModel model);
        EmployeeLoanViewModel GetEmployeesByBranchId(EmployeeLoanViewModel model);

        PaymentWindowViewModel GetEmployeeLoanPaymentById(Int64 Id);

        PaymentWindowViewModel CreateLoanPayment(PaymentWindowViewModel model);

        PaymentWindowViewModel UpdateLoanPayment(PaymentWindowViewModel model);

    }
}
