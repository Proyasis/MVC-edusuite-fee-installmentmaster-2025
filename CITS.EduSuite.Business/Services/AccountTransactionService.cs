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
    public class AccountTransactionService : IAccountTransactionService
    {
        private EduSuiteDatabase dbContext;

        public AccountTransactionService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<AccountTransactionViewModel> GetAccountTransactions(AccountTransactionViewModel model)
        {
            List<AccountTransactionViewModel> modelAccountTransaction = new List<AccountTransactionViewModel>();
            try
            {
                var AccountTransactionList = (from c in dbContext.AccountTransactions
                                              orderby c.DateAdded descending
                                              select new AccountTransactionViewModel
                                              {
                                                  RowKey = c.RowKey,
                                                  BranchKey = c.BranchKey,
                                                  BranchName = c.Branch.BranchName,
                                                  TransactionDate = c.TransactionDate,
                                                  TransactionTypeKey = c.AccountLedger.AccountLedgerType.LedgerFlowTypeKey,
                                                  AccountLedgerName = c.AccountLedger.AccountLedgerName,
                                                  PartyKey = c.PartyKey,
                                                  PartyTypeKey = c.PartyTypeKey,
                                                  PartyTypeName = c.PartyType.PartyTypeName,
                                                  Amount = c.Amount,
                                                  Remarks = c.Remarks,
                                                  TransactionStatusKey = c.TransactionStatusKey,
                                                  TransactionStatusName = c.ProcessStatu.ProcessStatusName
                                              }).ToList();
                if (model.BranchKey != 0)
                {
                    AccountTransactionList = AccountTransactionList.Where(row => row.BranchKey == model.BranchKey).ToList();
                }

                modelAccountTransaction = AccountTransactionList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<AccountTransactionViewModel>();
                return modelAccountTransaction;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.AccountTrancasaction, ActionConstants.View, DbConstants.LogType.Error, model.BranchKey, ex.GetBaseException().Message);
                return new List<AccountTransactionViewModel>();
               
            }
        }

        public AccountTransactionViewModel GetAccountTransactionById(AccountTransactionViewModel model)
        {
            try
            {
                AccountTransactionViewModel accountTransactionViewModel = dbContext.AccountTransactions.Select(row => new AccountTransactionViewModel
                {
                    RowKey = row.RowKey,
                    TransactionDate = row.TransactionDate,
                    TransactionTypeKey = row.AccountLedger.AccountLedgerType.LedgerFlowTypeKey,
                    PartyKey = row.PartyKey,
                    PartyTypeKey = row.PartyTypeKey,
                    Amount = row.Amount,
                    Remarks = row.Remarks,
                    BranchKey = row.BranchKey,
                    TransactionStatusKey = row.TransactionStatusKey,
                    AccountLedgerKey = row.AccountLedgerKey,
                    TransactionDueDate = row.TransactionDueDate,
                    DueFineAmount = row.DueFineAmount
                }).Where(x => x.RowKey == model.RowKey).FirstOrDefault();
                if (accountTransactionViewModel == null)
                {
                    accountTransactionViewModel = new AccountTransactionViewModel();
                    accountTransactionViewModel.BranchKey = model.BranchKey;
                }
                FillDropdownLists(accountTransactionViewModel);
                return accountTransactionViewModel;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.AccountTrancasaction, ActionConstants.View, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                return new AccountTransactionViewModel();
                
            }
        }

        public AccountTransactionViewModel CreateAccountTransaction(AccountTransactionViewModel model)
        {
            FillDropdownLists(model);
            AccountTransaction AccountTransactionModel = new AccountTransaction();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    long maxKey = dbContext.AccountTransactions.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    AccountTransactionModel.RowKey = Convert.ToInt64(maxKey + 1);
                    AccountTransactionModel.TransactionDate = Convert.ToDateTime(model.TransactionDate);
                    AccountTransactionModel.BranchKey = model.BranchKey;
                    AccountTransactionModel.AccountLedgerKey = model.AccountLedgerKey;
                    AccountTransactionModel.PartyTypeKey = model.PartyTypeKey;
                    AccountTransactionModel.PartyKey = model.PartyKey;
                    AccountTransactionModel.Amount = Convert.ToDecimal(model.Amount);
                    AccountTransactionModel.TransactionDueDate = model.TransactionDueDate;
                    AccountTransactionModel.DueFineAmount = model.DueFineAmount;
                    AccountTransactionModel.Remarks = model.Remarks;
                    AccountTransactionModel.TransactionStatusKey = DbConstants.ProcessStatus.Pending;
                    dbContext.AccountTransactions.Add(AccountTransactionModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.AccountTrancasaction, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // model.Message = AccountTransactionModel.AccountTransactionTypeKey == 1 ? ApplicationResources.FailedToSaveReceipt : ApplicationResources.FailedToSavePayment;
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.AccountTrancasaction, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public AccountTransactionViewModel UpdateAccountTransaction(AccountTransactionViewModel model)
        {
            FillDropdownLists(model);
            AccountTransaction AccountTransactionModel = new AccountTransaction();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    AccountTransactionModel.BranchKey = model.BranchKey;
                    AccountTransactionModel.TransactionDate = Convert.ToDateTime(model.TransactionDate);
                    AccountTransactionModel.AccountLedgerKey = model.AccountLedgerKey;
                    AccountTransactionModel.PartyTypeKey = model.PartyTypeKey;
                    AccountTransactionModel.PartyKey = model.PartyKey;
                    AccountTransactionModel.Amount = Convert.ToDecimal(model.Amount);
                    AccountTransactionModel.Remarks = model.Remarks;

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.AccountTrancasaction, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Transactions);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.AccountTrancasaction, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }


        public AccountTransactionViewModel DeleteAccountTransaction(AccountTransactionViewModel model)
        {
            AccountTransaction AccountTransactionModel = new AccountTransaction();
            AccountTransactionPayment AccountTransactionPayment = new AccountTransactionPayment();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    AccountTransactionModel = dbContext.AccountTransactions.SingleOrDefault(row => row.RowKey == model.RowKey);
                    AccountTransactionPayment = dbContext.AccountTransactionPayments.SingleOrDefault(row => row.AccountTransactionKey == AccountTransactionModel.RowKey);
                    if (AccountTransactionPayment != null)
                    {
                        dbContext.AccountTransactionPayments.Remove(AccountTransactionPayment);
                        AccountManagement accountManagement = new AccountManagement(dbContext);
                        CashFlow cashFlowModel = new CashFlow();
                        cashFlowModel = dbContext.CashFlows.Where(row => row.TransactionTypeKey == DbConstants.TransactionType.Transaction && row.TransactionKey == AccountTransactionPayment.RowKey).SingleOrDefault();
                        if (cashFlowModel != null)
                        {
                            accountManagement.DeleteCashFlowAccount(cashFlowModel);
                        }
                    }
                    dbContext.AccountTransactions.Remove(AccountTransactionModel);



                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.AccountTrancasaction, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Transactions);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.AccountTrancasaction, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Transactions);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.AccountTrancasaction, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public List<SelectListModel> GetPartyByPartyType(AccountTransactionViewModel model)
        {
            List<SelectListModel> List = new List<SelectListModel>();

            switch (model.PartyTypeKey)
            {
                case DbConstants.PartyType.Employee:
                    List = dbContext.Employees.Where(row => row.BranchKey == model.BranchKey).Select(row => new SelectListModel
                    {
                        RowKey = row.RowKey,
                        Text = row.FirstName + " " + (row.MiddleName ?? "") + " " + row.LastName,
                    }).ToList();
                    break;
            }
            return List;
        }

        public AccountTransactionViewModel GetBranches(AccountTransactionViewModel model)
        {

            model.Branches = dbContext.vwBranchSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BranchName
            }).ToList();

            return model;
        }

        public byte GetTransactionTypeByLedger(int LedgerKey)
        {
            return dbContext.AccountLedgers.Where(row => row.RowKey == LedgerKey).Select(row => row.AccountLedgerType.LedgerFlowTypeKey).FirstOrDefault();
        }

        private void FillDropdownLists(AccountTransactionViewModel model)
        {

            FillPartyTypes(model);
            GetBranches(model);
            FillAccountLedgers(model);
            model.Parties = GetPartyByPartyType(model);
        }

        private void FillPartyTypes(AccountTransactionViewModel model)
        {
            model.PartyTypes = dbContext.PartyTypes.Where(row => row.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.PartyTypeName
            }).ToList();
        }

        private void FillAccountLedgers(AccountTransactionViewModel model)
        {
            model.AccountLedgers = dbContext.VwAccountLedgerSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AccountLedgerName
            }).ToList();
        }



        private string GetPartyNameByPartyType(byte PartyType, long PartyKey)
        {
            string PartyName = "";
            switch (PartyType)
            {
                case 1:
                    PartyName = (from e in dbContext.Employees
                                 where e.RowKey == PartyKey
                                 select (e.FirstName + " " + (e.MiddleName ?? "") + " " + e.LastName)
                    ).SingleOrDefault().ToString();
                    break;
            }
            return PartyName;
        }


        #region Transaction Payment

        public PaymentWindowViewModel GetAccountTransactionPaymentById(long Id)
        {
            PaymentWindowViewModel model = new PaymentWindowViewModel();
            AccountTransaction accountTransaction = new AccountTransaction();
            accountTransaction = dbContext.AccountTransactions.SingleOrDefault(row => row.RowKey == Id);
            model = dbContext.AccountTransactionPayments.Where(x => x.AccountTransactionKey == Id).Select(row => new PaymentWindowViewModel
            {
                PaymentKey = row.RowKey,
                MasterKey = row.AccountTransactionKey,
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
                AmountToPay = row.AccountTransaction.Amount,
                Remarks = row.Remarks

            }).FirstOrDefault();
            if (model == null)
            {
                model = new PaymentWindowViewModel();
                model.PaymentModeKey = DbConstants.PaymentMode.Cash;
                model.MasterKey = Id;
                model.Purpose = accountTransaction.AccountLedger.AccountLedgerName;
                if (accountTransaction.AccountLedger.AccountLedgerType.LedgerFlowTypeKey == DbConstants.CashFlowType.In)
                {
                    model.PaidBy = GetPartyNameByPartyType(accountTransaction.PartyTypeKey, accountTransaction.PartyKey);
                }
                else
                {
                    model.ReceivedBy = GetPartyNameByPartyType(accountTransaction.PartyTypeKey, accountTransaction.PartyKey);
                }
                model.AmountToPay = accountTransaction.Amount;
                if (accountTransaction.TransactionDueDate != null && Convert.ToDateTime(accountTransaction.TransactionDueDate).Date < DateTimeUTC.Now.Date)
                {
                    model.AmountToPay = accountTransaction.Amount + (accountTransaction.DueFineAmount ?? 0);
                }
            }

            FillTransactionPaymentDropdownLists(model);

            return model;
        }

        public PaymentWindowViewModel CreateTransactionPayment(PaymentWindowViewModel model)
        {
            AccountTransactionPayment AccountTransactionPaymentModel = new AccountTransactionPayment();
            FillTransactionPaymentDropdownLists(model);

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Int64 maxKey = dbContext.AccountTransactionPayments.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    AccountTransactionPaymentModel.RowKey = Convert.ToInt64(maxKey + 1);
                    AccountTransactionPaymentModel.AccountTransactionKey = model.MasterKey;
                    AccountTransactionPaymentModel.PaidAmount = Convert.ToDecimal(model.PaidAmount);
                    AccountTransactionPaymentModel.BalanceAmount = model.BalanceAmount;
                    AccountTransactionPaymentModel.PaymentDate = Convert.ToDateTime(model.PaymentDate);
                    AccountTransactionPaymentModel.PaymentModeKey = model.PaymentModeKey;
                    AccountTransactionPaymentModel.BankAccountKey = model.BankAccountKey;
                    AccountTransactionPaymentModel.PaymentModeKey = model.PaymentModeKey;
                    AccountTransactionPaymentModel.CardNumber = model.CardNumber;
                    AccountTransactionPaymentModel.BankAccountKey = model.BankAccountKey;
                    AccountTransactionPaymentModel.ChequeOrDDNumber = model.ChequeOrDDNumber;
                    AccountTransactionPaymentModel.ChequeOrDDDate = model.ChequeOrDDDate;
                    AccountTransactionPaymentModel.Purpose = model.Purpose;
                    AccountTransactionPaymentModel.PaidBy = model.PaidBy;
                    AccountTransactionPaymentModel.AuthorizedBy = model.AuthorizedBy;
                    AccountTransactionPaymentModel.ReceivedBy = model.ReceivedBy;
                    AccountTransactionPaymentModel.OnBehalfOf = model.OnBehalfOf;
                    AccountTransactionPaymentModel.Remarks = model.Remarks;

                    model.PaymentKey = AccountTransactionPaymentModel.RowKey;
                    CreateAccount(model);
                    dbContext.AccountTransactionPayments.Add(AccountTransactionPaymentModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.AccountTrancasaction, ActionConstants.Add, DbConstants.LogType.Info, AccountTransactionPaymentModel.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Payment);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.AccountTrancasaction, ActionConstants.Add, DbConstants.LogType.Error, AccountTransactionPaymentModel.RowKey, ex.GetBaseException().Message);
                }

            }
            return model;
        }


        public PaymentWindowViewModel UpdateTransactionPayment(PaymentWindowViewModel model)
        {
            AccountTransactionPayment AccountTransactionPaymentModel = new AccountTransactionPayment();
            FillTransactionPaymentDropdownLists(model);

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    AccountTransactionPaymentModel = dbContext.AccountTransactionPayments.SingleOrDefault(row => row.RowKey == model.PaymentKey);
                    AccountTransactionPaymentModel.PaidAmount = Convert.ToDecimal(model.PaidAmount);
                    AccountTransactionPaymentModel.BalanceAmount = model.BalanceAmount;
                    AccountTransactionPaymentModel.PaymentDate = Convert.ToDateTime(model.PaymentDate);
                    AccountTransactionPaymentModel.PaymentModeKey = model.PaymentModeKey;
                    AccountTransactionPaymentModel.BankAccountKey = model.BankAccountKey;
                    AccountTransactionPaymentModel.PaymentModeKey = model.PaymentModeKey;
                    AccountTransactionPaymentModel.CardNumber = model.CardNumber;
                    AccountTransactionPaymentModel.BankAccountKey = model.BankAccountKey;
                    AccountTransactionPaymentModel.ChequeOrDDNumber = model.ChequeOrDDNumber;
                    AccountTransactionPaymentModel.ChequeOrDDDate = model.ChequeOrDDDate;
                    AccountTransactionPaymentModel.Purpose = model.Purpose;
                    AccountTransactionPaymentModel.PaidBy = model.PaidBy;
                    AccountTransactionPaymentModel.AuthorizedBy = model.AuthorizedBy;
                    AccountTransactionPaymentModel.ReceivedBy = model.ReceivedBy;
                    AccountTransactionPaymentModel.OnBehalfOf = model.OnBehalfOf;
                    AccountTransactionPaymentModel.Remarks = model.Remarks;

                    UpdateAccount(model);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.AccountTrancasaction, ActionConstants.Edit, DbConstants.LogType.Info, model.PaymentKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Payment);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.AccountTrancasaction, ActionConstants.Edit, DbConstants.LogType.Error, model.PaymentKey, ex.GetBaseException().Message);
                }

            }
            return model;
        }

        private void CreateAccount(PaymentWindowViewModel model)
        {
            AccountManagement accountManagement = new AccountManagement(dbContext);
            CashFlowViewModel cashFlowModel = new CashFlowViewModel();
            AccountTransaction AccountTransaction = dbContext.AccountTransactions.SingleOrDefault(row => row.RowKey == model.MasterKey);
            cashFlowModel.CashFlowDate = Convert.ToDateTime(model.PaymentDate);
            cashFlowModel.CashFlowTypeKey = DbConstants.CashFlowType.Out;
            cashFlowModel.PartyKey = AccountTransaction.PartyKey;
            //cashFlowModel.PartyTypeKey = AccountTransaction.PartyTypeKey;
            //cashFlowModel.VoucherNumber = model.VoucherNumber;
            cashFlowModel.Amount = Convert.ToDecimal(model.PaidAmount);
            cashFlowModel.PaymentModeKey = model.PaymentModeKey;
            cashFlowModel.BankAccountKey = model.BankAccountKey;
            cashFlowModel.TransactionTypeKey = DbConstants.TransactionType.Transaction;
            cashFlowModel.TransactionKey = model.PaymentKey;
            cashFlowModel.Purpose = model.Purpose;
            cashFlowModel.PaidBy = model.PaidBy;
            cashFlowModel.AuthorizedBy = model.AuthorizedBy;
            cashFlowModel.ReceivedBy = model.ReceivedBy;
            cashFlowModel.OnBehalfOf = model.OnBehalfOf;
            cashFlowModel.Remarks = model.Remarks;
            cashFlowModel.BranchKey = AccountTransaction.BranchKey;

            //accountManagement.CreateCashFlowAccount(cashFlowModel);
        }

        private void UpdateAccount(PaymentWindowViewModel model)
        {
            AccountManagement accountManagement = new AccountManagement(dbContext);
            CashFlowViewModel cashFlowModel = new CashFlowViewModel();
            AccountTransaction accountTransaction = dbContext.AccountTransactions.SingleOrDefault(row => row.RowKey == model.MasterKey);

            cashFlowModel.CashFlowDate = Convert.ToDateTime(model.PaymentDate);
            cashFlowModel.CashFlowTypeKey = DbConstants.CashFlowType.Out;
            cashFlowModel.PartyKey = accountTransaction.PartyKey;
            //cashFlowModel.PartyTypeKey = accountTransaction.PartyTypeKey;
            //cashFlowModel.VoucherNumber = model.VoucherNumber;
            cashFlowModel.Amount = Convert.ToDecimal(model.PaidAmount);
            cashFlowModel.PaymentModeKey = model.PaymentModeKey;
            cashFlowModel.BankAccountKey = model.BankAccountKey;
            cashFlowModel.TransactionTypeKey = DbConstants.TransactionType.Transaction;
            cashFlowModel.TransactionKey = model.PaymentKey;
            cashFlowModel.Purpose = model.Purpose;
            cashFlowModel.PaidBy = model.PaidBy;
            cashFlowModel.AuthorizedBy = model.AuthorizedBy;
            cashFlowModel.ReceivedBy = model.ReceivedBy;
            cashFlowModel.OnBehalfOf = model.OnBehalfOf;
            cashFlowModel.Remarks = model.Remarks;
            cashFlowModel.BranchKey = accountTransaction.BranchKey;

            accountManagement.UpdateCashFlowAccount(cashFlowModel);
        }

        private void FillTransactionPaymentDropdownLists(PaymentWindowViewModel model)
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
            var BranchKey = dbContext.AccountTransactions.Where(row => row.RowKey == model.MasterKey).Select(row => row.BranchKey).FirstOrDefault();
           
            model.BankAccounts = dbContext.BranchAccounts.Where(x => x.BranchKey == BranchKey && x.BankAccount.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.BankAccount.RowKey,
                Text = (row.BankAccount.NameInAccount ?? row.BankAccount.AccountNumber) + EduSuiteUIResources.Hyphen + row.BankAccount.Bank.BankName
            }).ToList();

        }

        #endregion

    }
}
