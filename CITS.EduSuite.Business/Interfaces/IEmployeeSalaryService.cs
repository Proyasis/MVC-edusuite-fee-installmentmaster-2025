using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IEmployeeSalaryService
    {
        List<EmployeeSalaryMasterViewModel> GetEmployeeSalaries(EmployeeSalaryMasterViewModel model, out long TotalRecords);
        EmployeeSalaryMasterViewModel GetEmployeeSalaryById(EmployeeSalaryMasterViewModel model);

        EmployeeSalaryMasterViewModel GetEmployeeSalaryByMonth(EmployeeSalaryMasterViewModel model);

        List<EmployeeSalaryMasterViewModel> GetEmployeesSalaryByMonth(EmployeeSalaryMasterViewModel model);

        EmployeeSalaryMasterViewModel CreateEmployeeSalary(EmployeeSalaryMasterViewModel model);

        EmployeeSalaryMasterViewModel UpdateEmployeeSalary(EmployeeSalaryMasterViewModel model);

        EmployeeSalaryMasterViewModel UpdateEmployeesSalaryList(List<EmployeeSalaryMasterViewModel> modelList);

        List<EmployeeSalaryMasterViewModel> GetPaySlipDetailsByEmployee(EmployeeSalaryMasterViewModel model);

        EmployeeSalaryMasterViewModel DeleteEmployeeSalary(EmployeeSalaryMasterViewModel model);

        EmployeeSalaryMasterViewModel DeleteEmployeeSalaryComponent(EmployeeSalaryDetailViewModel model);

        //EmployeeSalaryMasterViewModel GetBranches(EmployeeSalaryMasterViewModel model);
        //EmployeeSalaryMasterViewModel GetEmployeesByBranchId(EmployeeSalaryMasterViewModel model);
        EmployeeSalaryMasterViewModel FillEmployeesMaster(EmployeeSalaryMasterViewModel model);
        EmployeeSalaryMasterViewModel GetSalaryHeads(EmployeeSalaryMasterViewModel model);

        PaymentWindowViewModel GetEmployeeSalaryPaymentById(Int64 Id);

        PaymentWindowViewModel CreateSalaryPayment(PaymentWindowViewModel model);

        PaymentWindowViewModel UpdateSalaryPayment(PaymentWindowViewModel model);

        //List<SalaryPaymentSlipViewModel> GetSalaryPaymentSlipByIds(List<long> Ids);
        List<SalaryPaymentSlipViewModel> GetSalaryPaymentSlipByIds(Int64 Id);


        List<EmployeeSalaryAdvanceViewModel> GetEmployeeSalaryAdvances(EmployeeSalaryAdvanceViewModel model, string FromDate, string ToDate);

        EmployeeSalaryAdvanceViewModel GetEmployeeSalaryAdvancePaymentById(EmployeeSalaryAdvanceViewModel model);

        EmployeeSalaryAdvanceViewModel CreateSalaryAdvancePayment(EmployeeSalaryAdvanceViewModel model);

        EmployeeSalaryAdvanceViewModel UpdateSalaryAdvancePayment(EmployeeSalaryAdvanceViewModel model);

        EmployeeSalaryAdvanceViewModel DeleteSalaryAdvancePayment(EmployeeSalaryAdvanceViewModel model);

        EmployeeSalaryAdvanceViewModel FillEmployees(EmployeeSalaryAdvanceViewModel model);

        decimal GetBalanceforAdvance(short PaymentModeKey, long Rowkey, long BankAccountKey, short branchKey);
        void GetBranches(EmployeeSalaryAdvanceViewModel model);
        EmployeeSalaryAdvanceViewModel FillBankAccounts(EmployeeSalaryAdvanceViewModel model);

        void FillMasterDropdownLists(EmployeeSalaryMasterViewModel model);
        #region AdvanceReturn

        List<EmployeeSalaryAdvanceReturnViewModel> GetEmployeeSalaryAdvanceReturn(EmployeeSalaryAdvanceReturnViewModel model, string fromDate, string toDate);
        EmployeeSalaryAdvanceReturnViewModel GetEmployeeSalaryAdvanceReturnById(EmployeeSalaryAdvanceReturnViewModel model);
        EmployeeSalaryAdvanceReturnViewModel CreateSalaryAdvanceReturn(EmployeeSalaryAdvanceReturnViewModel model);
        EmployeeSalaryAdvanceReturnViewModel UpdateSalaryAdvanceReturn(EmployeeSalaryAdvanceReturnViewModel model);
        EmployeeSalaryAdvanceReturnViewModel DeleteSalaryAdvanceReturn(EmployeeSalaryAdvanceReturnViewModel model);
        EmployeeSalaryAdvanceReturnViewModel fillAdvances(EmployeeSalaryAdvanceReturnViewModel model);

        #endregion

        PaymentWindowViewModel FillPaymentModeSub(PaymentWindowViewModel model);
        EmployeeSalaryAdvanceViewModel FillPaymentModeSub(EmployeeSalaryAdvanceViewModel model);
        List<PaymentWindowViewModel> GetEmployeePaymentDetails(long SalaryMasterKey);
        PaymentWindowViewModel GetEmployeePaymentById(long Id);
        PaymentWindowViewModel DeleteSalaryPayment(long? PaymentKey);
    }
}
