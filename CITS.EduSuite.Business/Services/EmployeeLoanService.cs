using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;


namespace CITS.EduSuite.Business.Services
{
    public class EmployeeLoanService : IEmployeeLoanService
    {
        private EduSuiteDatabase dbContext;

        public EmployeeLoanService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }


        public List<EmployeeLoanViewModel> GetEmployeeLoan(EmployeeLoanViewModel model)
        {
            try
            {
                var EmployeeLoanList = (from el in dbContext.EmployeeLoans
                                        orderby el.LoanDate descending
                                        select new EmployeeLoanViewModel
                                             {
                                                 RowKey = el.RowKey,
                                                 EmployeeKey = el.EmployeeKey,
                                                 EmployeeName = el.Employee.FirstName + " " + (el.Employee.MiddleName != null ? el.Employee.MiddleName : "") + " " + el.Employee.LastName,
                                                 Amount = el.Amount,
                                                 LoanDate = el.LoanDate,
                                                 LoanInPaySlip = el.LoanInPaySlip,
                                                 MonthlyRepaymentAmount = el.MonthlyRepaymentAmount,
                                                 RepaymentStartDate = el.RepaymentStartDate,
                                                 Remarks = el.Remarks,
                                                 LoanStatusName = el.ProcessStatu.ProcessStatusName,
                                                 LoanStatusKey = el.ProcessStatu.RowKey,
                                                 BranchKey = el.Employee.BranchKey
                                             }).ToList();
                if (model.EmployeeKey != 0)
                {
                    EmployeeLoanList = EmployeeLoanList.Where(row => row.EmployeeKey == model.EmployeeKey).ToList();
                }
                if (model.BranchKey != 0)
                {
                    EmployeeLoanList = EmployeeLoanList.Where(row => row.BranchKey == model.BranchKey).ToList();
                }

                return EmployeeLoanList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<EmployeeLoanViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.EmployeeLoan, ActionConstants.View, DbConstants.LogType.Error, model.EmployeeKey, ex.GetBaseException().Message);
                return new List<EmployeeLoanViewModel>();

            }
        }

        public EmployeeLoanViewModel GetEmployeeLoanById(EmployeeLoanViewModel model)
        {
            try
            {

                EmployeeLoanViewModel employeeLoanViewModel = new EmployeeLoanViewModel();

                employeeLoanViewModel = dbContext.EmployeeLoans.Select(row => new EmployeeLoanViewModel
                {
                    RowKey = row.RowKey,
                    EmployeeKey = row.EmployeeKey,
                    Amount = row.Amount,
                    LoanDate = row.LoanDate,
                    LoanInPaySlip = row.LoanInPaySlip,
                    MonthlyRepaymentAmount = row.MonthlyRepaymentAmount,
                    RepaymentStartDate = row.RepaymentStartDate,
                    LoanStatusKey = row.LoanStatusKey,
                    Remarks = row.Remarks,
                    BranchKey = row.Employee.BranchKey

                }).Where(x => x.RowKey == model.RowKey).FirstOrDefault();
                if (employeeLoanViewModel == null)
                {
                    employeeLoanViewModel = new EmployeeLoanViewModel();
                    employeeLoanViewModel.EmployeeKey = model.EmployeeKey;
                    employeeLoanViewModel.BranchKey = model.BranchKey;

                }
                FillDropdownList(employeeLoanViewModel);
                return employeeLoanViewModel;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.EmployeeLoan, ActionConstants.View, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                return new EmployeeLoanViewModel();


            }
        }

        public EmployeeLoanViewModel CreateEmployeeLoan(EmployeeLoanViewModel model)
        {

            EmployeeLoan employeeLoanModel = new EmployeeLoan();
            FillDropdownList(model);
            using (var transaction = dbContext.Database.BeginTransaction())
            {

                try
                {
                    Int64 maxLoanKey = dbContext.EmployeeLoans.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    employeeLoanModel.RowKey = Convert.ToInt64(maxLoanKey + 1);
                    employeeLoanModel.EmployeeKey = model.EmployeeKey;
                    employeeLoanModel.Amount = Convert.ToDecimal(model.Amount);
                    employeeLoanModel.LoanInPaySlip = model.LoanInPaySlip;
                    employeeLoanModel.LoanDate = Convert.ToDateTime(model.LoanDate);
                    employeeLoanModel.MonthlyRepaymentAmount = Convert.ToDecimal(model.MonthlyRepaymentAmount);
                    employeeLoanModel.RepaymentStartDate = model.RepaymentStartDate;
                    employeeLoanModel.Remarks = model.Remarks;
                    employeeLoanModel.LoanStatusKey = DbConstants.ProcessStatus.Pending;
                    dbContext.EmployeeLoans.Add(employeeLoanModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeLoan, ActionConstants.Add, DbConstants.LogType.Info, employeeLoanModel.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.EmployeeLoan);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeLoan, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }



        public EmployeeLoanViewModel UpdateEmployeeLoan(EmployeeLoanViewModel model)
        {
            EmployeeLoan employeeLoanModel = new EmployeeLoan();
            FillDropdownList(model);

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    employeeLoanModel = dbContext.EmployeeLoans.SingleOrDefault(row => row.RowKey == model.RowKey);
                    employeeLoanModel.Amount = Convert.ToDecimal(model.Amount);
                    employeeLoanModel.EmployeeKey = model.EmployeeKey;
                    employeeLoanModel.LoanInPaySlip = model.LoanInPaySlip;
                    employeeLoanModel.LoanDate = Convert.ToDateTime(model.LoanDate);
                    employeeLoanModel.MonthlyRepaymentAmount = Convert.ToDecimal(model.MonthlyRepaymentAmount);
                    employeeLoanModel.RepaymentStartDate = model.RepaymentStartDate;
                    employeeLoanModel.Remarks = model.Remarks;
                    dbContext.SaveChanges();
                    transaction.Commit();



                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeLoan, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.EmployeeLoan);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeLoan, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }


        public EmployeeLoanViewModel DeleteEmployeeLoan(EmployeeLoanViewModel model)
        {

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    EmployeeLoan employeeLoan = dbContext.EmployeeLoans.SingleOrDefault(row => row.RowKey == model.RowKey);
                    EmployeeLoanPayment employeeLoanPayment = dbContext.EmployeeLoanPayments.SingleOrDefault(row => row.EmployeeLoanKey == employeeLoan.RowKey);
                    if (employeeLoanPayment != null)
                    {
                        dbContext.EmployeeLoanPayments.Remove(employeeLoanPayment);
                        AccountManagement accountManagement = new AccountManagement(dbContext);
                        CashFlow cashFlowModel = new CashFlow();
                        cashFlowModel = dbContext.CashFlows.Where(row => row.TransactionTypeKey == DbConstants.TransactionType.Loan && row.TransactionKey == employeeLoanPayment.RowKey).SingleOrDefault();
                        if (cashFlowModel != null)
                        {
                            accountManagement.DeleteCashFlowAccount(cashFlowModel);
                        }

                    }
                    dbContext.EmployeeLoans.Remove(employeeLoan);


                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeLoan, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.EmployeeLoan);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.EmployeeLoan, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.EmployeeLoan);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeLoan, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public EmployeeLoanViewModel GetBranches(EmployeeLoanViewModel model)
        {

            model.Branches = dbContext.vwBranchSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BranchName
            }).ToList();

            return model;
        }
        public EmployeeLoanViewModel GetEmployeesByBranchId(EmployeeLoanViewModel model)
        {
            model.Employees = dbContext.Employees.Where(row => row.BranchKey == model.BranchKey).Select(row => new GroupSelectListModel
            {
                RowKey = row.RowKey,
                Text = row.FirstName + " " + (row.MiddleName ?? "") + " " + row.LastName,
                GroupKey = row.DepartmentKey,
                GroupName = row.Department.DepartmentName
            }).OrderBy(row => row.Text).ToList();

            return model;
        }

        private void FillDropdownList(EmployeeLoanViewModel model)
        {
            GetBranches(model);
            GetEmployeesByBranchId(model);
        }

        #region Loan Payment

        public PaymentWindowViewModel GetEmployeeLoanPaymentById(long Id)
        {
            PaymentWindowViewModel model = new PaymentWindowViewModel();
            EmployeeLoan EmployeeLoanMaster = new EmployeeLoan();
            EmployeeLoanMaster = dbContext.EmployeeLoans.SingleOrDefault(row => row.RowKey == Id);
            model = dbContext.EmployeeLoanPayments.Where(x => x.EmployeeLoanKey == Id).Select(row => new PaymentWindowViewModel
            {
                PaymentKey = row.RowKey,
                MasterKey = row.EmployeeLoanKey,
                PaymentModeKey = row.PaymentModeKey,
                PaidAmount = row.PaidAmount,
                BalanceAmount = row.BalanceAmount,
                PaymentDate = row.PaymentDate,
                BankAccountKey = row.BankAccountKey,
                BankAccountBalance = row.BankAccount.CurrentAccountBalance,
                CardNumber = row.CardNumber,
                ChequeOrDDNumber = row.ChequeOrDDNumber,
                ChequeOrDDDate = row.ChequeOrDDDate,
                Purpose = row.Purpose,
                ReceivedBy = row.ReceivedBy,
                OnBehalfOf = row.OnBehalfOf,
                PaidBy = row.PaidBy,
                AuthorizedBy = row.AuthorizedBy,
                AmountToPay = row.EmployeeLoan.Amount ?? 0,
                Remarks = row.Remarks

            }).FirstOrDefault();
            if (model == null)
            {
                model = new PaymentWindowViewModel();
                model.PaymentModeKey = DbConstants.PaymentMode.Cash;
                model.MasterKey = Id;
                model.Purpose = EduSuiteUIResources.Loan;
                model.ReceivedBy = EmployeeLoanMaster.Employee.FirstName + " " + (EmployeeLoanMaster.Employee.MiddleName ?? "") + " " + EmployeeLoanMaster.Employee.LastName;
                model.AmountToPay = dbContext.EmployeeLoans.Where(row => row.RowKey == Id).Select(row => row.Amount ?? 0).FirstOrDefault();
            }

            FillLoanPaymentDropdownLists(model);

            return model;
        }

        public PaymentWindowViewModel CreateLoanPayment(PaymentWindowViewModel model)
        {
            EmployeeLoanPayment EmployeeLoanPaymentModel = new EmployeeLoanPayment();
            FillLoanPaymentDropdownLists(model);

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Int64 maxKey = dbContext.EmployeeLoanPayments.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    EmployeeLoanPaymentModel.RowKey = Convert.ToInt64(maxKey + 1);
                    EmployeeLoanPaymentModel.EmployeeLoanKey = model.MasterKey;
                    EmployeeLoanPaymentModel.PaidAmount = Convert.ToDecimal(model.PaidAmount);
                    EmployeeLoanPaymentModel.BalanceAmount = model.BalanceAmount;
                    EmployeeLoanPaymentModel.PaymentDate = Convert.ToDateTime(model.PaymentDate);
                    EmployeeLoanPaymentModel.PaymentModeKey = model.PaymentModeKey;
                    EmployeeLoanPaymentModel.BankAccountKey = model.BankAccountKey;
                    EmployeeLoanPaymentModel.PaymentModeKey = model.PaymentModeKey;
                    EmployeeLoanPaymentModel.CardNumber = model.CardNumber;
                    EmployeeLoanPaymentModel.BankAccountKey = model.BankAccountKey;
                    EmployeeLoanPaymentModel.ChequeOrDDNumber = model.ChequeOrDDNumber;
                    EmployeeLoanPaymentModel.ChequeOrDDDate = model.ChequeOrDDDate;
                    EmployeeLoanPaymentModel.Purpose = model.Purpose;
                    EmployeeLoanPaymentModel.PaidBy = model.PaidBy;
                    EmployeeLoanPaymentModel.AuthorizedBy = model.AuthorizedBy;
                    EmployeeLoanPaymentModel.ReceivedBy = model.ReceivedBy;
                    EmployeeLoanPaymentModel.OnBehalfOf = model.OnBehalfOf;
                    EmployeeLoanPaymentModel.Remarks = model.Remarks;

                    model.PaymentKey = EmployeeLoanPaymentModel.RowKey;
                    CreateAccount(model);
                    dbContext.EmployeeLoanPayments.Add(EmployeeLoanPaymentModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeLoan, ActionConstants.Add, DbConstants.LogType.Info, EmployeeLoanPaymentModel.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.EmployeeLoan);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeLoan, ActionConstants.Add, DbConstants.LogType.Error, EmployeeLoanPaymentModel.RowKey, ex.GetBaseException().Message);

                }

            }
            return model;
        }


        public PaymentWindowViewModel UpdateLoanPayment(PaymentWindowViewModel model)
        {
            EmployeeLoanPayment EmployeeLoanPaymentModel = new EmployeeLoanPayment();
            FillLoanPaymentDropdownLists(model);

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    EmployeeLoanPaymentModel = dbContext.EmployeeLoanPayments.SingleOrDefault(row => row.RowKey == model.PaymentKey);
                    EmployeeLoanPaymentModel.PaidAmount = Convert.ToDecimal(model.PaidAmount);
                    EmployeeLoanPaymentModel.BalanceAmount = model.BalanceAmount;
                    EmployeeLoanPaymentModel.PaymentDate = Convert.ToDateTime(model.PaymentDate);
                    EmployeeLoanPaymentModel.PaymentModeKey = model.PaymentModeKey;
                    EmployeeLoanPaymentModel.BankAccountKey = model.BankAccountKey;
                    EmployeeLoanPaymentModel.PaymentModeKey = model.PaymentModeKey;
                    EmployeeLoanPaymentModel.CardNumber = model.CardNumber;
                    EmployeeLoanPaymentModel.BankAccountKey = model.BankAccountKey;
                    EmployeeLoanPaymentModel.ChequeOrDDNumber = model.ChequeOrDDNumber;
                    EmployeeLoanPaymentModel.ChequeOrDDDate = model.ChequeOrDDDate;
                    EmployeeLoanPaymentModel.Purpose = model.Purpose;
                    EmployeeLoanPaymentModel.PaidBy = model.PaidBy;
                    EmployeeLoanPaymentModel.AuthorizedBy = model.AuthorizedBy;
                    EmployeeLoanPaymentModel.ReceivedBy = model.ReceivedBy;
                    EmployeeLoanPaymentModel.OnBehalfOf = model.OnBehalfOf;
                    EmployeeLoanPaymentModel.Remarks = model.Remarks;

                    UpdateAccount(model);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeLoan, ActionConstants.Edit, DbConstants.LogType.Info, EmployeeLoanPaymentModel.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.EmployeeLoan);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeLoan, ActionConstants.Edit, DbConstants.LogType.Error, EmployeeLoanPaymentModel.RowKey, ex.GetBaseException().Message);

                }

            }
            return model;
        }

        private void CreateAccount(PaymentWindowViewModel model)
        {
            AccountManagement accountManagement = new AccountManagement(dbContext);
            CashFlowViewModel cashFlowModel = new CashFlowViewModel();
            EmployeeLoan EmployeeLoanMaster = dbContext.EmployeeLoans.SingleOrDefault(row => row.RowKey == model.MasterKey);
            cashFlowModel.CashFlowDate = Convert.ToDateTime(model.PaymentDate);
            cashFlowModel.CashFlowTypeKey = DbConstants.CashFlowType.Out;
            cashFlowModel.PartyKey = EmployeeLoanMaster.EmployeeKey;
            //cashFlowModel.PartyTypeKey = DbConstants.PartyType.Employee;
            //cashFlowModel.VoucherNumber = model.VoucherNumber;
            cashFlowModel.Amount = Convert.ToDecimal(model.PaidAmount);
            cashFlowModel.PaymentModeKey = model.PaymentModeKey;
            cashFlowModel.BankAccountKey = model.BankAccountKey;
            cashFlowModel.TransactionTypeKey = DbConstants.TransactionType.Loan;
            cashFlowModel.TransactionKey = model.PaymentKey;
            cashFlowModel.Purpose = model.Purpose;
            cashFlowModel.PaidBy = model.PaidBy;
            cashFlowModel.AuthorizedBy = model.AuthorizedBy;
            cashFlowModel.ReceivedBy = model.ReceivedBy;
            cashFlowModel.OnBehalfOf = model.OnBehalfOf;
            cashFlowModel.Remarks = model.Remarks;
            cashFlowModel.BranchKey = EmployeeLoanMaster.Employee.BranchKey;

            //accountManagement.CreateCashFlowAccount(cashFlowModel);
        }

        private void UpdateAccount(PaymentWindowViewModel model)
        {
            AccountManagement accountManagement = new AccountManagement(dbContext);
            CashFlowViewModel cashFlowModel = new CashFlowViewModel();
            EmployeeLoan EmployeeLoanMaster = dbContext.EmployeeLoans.SingleOrDefault(row => row.RowKey == model.MasterKey);

            cashFlowModel.CashFlowDate = Convert.ToDateTime(model.PaymentDate);
            cashFlowModel.CashFlowTypeKey = DbConstants.CashFlowType.Out;
            cashFlowModel.PartyKey = EmployeeLoanMaster.EmployeeKey;
            //cashFlowModel.PartyTypeKey = DbConstants.PartyType.Employee;
            //cashFlowModel.VoucherNumber = model.VoucherNumber;
            cashFlowModel.Amount = Convert.ToDecimal(model.PaidAmount);
            cashFlowModel.PaymentModeKey = model.PaymentModeKey;
            cashFlowModel.BankAccountKey = model.BankAccountKey;
            cashFlowModel.TransactionTypeKey = DbConstants.TransactionType.Loan;
            cashFlowModel.TransactionKey = model.PaymentKey;
            cashFlowModel.Purpose = model.Purpose;
            cashFlowModel.PaidBy = model.PaidBy;
            cashFlowModel.AuthorizedBy = model.AuthorizedBy;
            cashFlowModel.ReceivedBy = model.ReceivedBy;
            cashFlowModel.OnBehalfOf = model.OnBehalfOf;
            cashFlowModel.Remarks = model.Remarks;
            cashFlowModel.BranchKey = EmployeeLoanMaster.Employee.BranchKey;

            accountManagement.UpdateCashFlowAccount(cashFlowModel);
        }

        private void FillLoanPaymentDropdownLists(PaymentWindowViewModel model)
        {

            FillPaymentModes(model);
            FillBankAccounts(model);

        }


        private void FillPaymentModes(PaymentWindowViewModel model)
        {
            model.PaymentModes = dbContext.VwPaymentModeSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.PaymentModeName
            }).ToList();
        }

        private void FillBankAccounts(PaymentWindowViewModel model)
        {
            var BranchKey = dbContext.EmployeeLoans.Where(row => row.RowKey == model.MasterKey).Select(row => row.Employee.BranchKey).FirstOrDefault();
            
            model.BankAccounts = dbContext.BranchAccounts.Where(x => x.BranchKey == BranchKey && x.BankAccount.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.BankAccount.RowKey,
                Text = (row.BankAccount.NameInAccount ?? row.BankAccount.AccountNumber) + EduSuiteUIResources.Hyphen + row.BankAccount.Bank.BankName
            }).ToList();
        }

        #endregion
    }
}
