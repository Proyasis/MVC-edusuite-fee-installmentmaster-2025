using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using CITS.EduSuite.Business.Services;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Common
{
    public class AccountManagement
    {
        private EduSuiteDatabase dbContext;

        public AccountManagement(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public void CreateCashFlowAccount(CashFlowViewModel model, ConfigurationViewModel configModel)
        {

            CashFlow cashFlowModel = new CashFlow();

            Int64 maxKey = dbContext.CashFlows.Select(s => s.RowKey).DefaultIfEmpty().Max();

            UpdateBankAccount(model, cashFlowModel);
            CashFlow PrevAccount = dbContext.CashFlows.Where(row => row.PaymentModeKey == model.PaymentModeKey).OrderByDescending(x => x.RowKey).FirstOrDefault();
            if (DbConstants.PaymentMode.BankPaymentModes.Contains(Convert.ToByte(model.PaymentModeKey)))
            {
                PrevAccount = dbContext.CashFlows.Where(row => row.PaymentModeKey == model.PaymentModeKey && row.BankAccountKey == model.BankAccountKey).OrderByDescending(x => x.RowKey).FirstOrDefault();
            }
            else if (DbConstants.PaymentMode.CashPaymentModes.Contains(Convert.ToByte(model.PaymentModeKey)))
            {
                PrevAccount = dbContext.CashFlows.Where(row => row.PaymentModeKey == model.PaymentModeKey && row.BranchKey == model.BranchKey).OrderByDescending(x => x.RowKey).FirstOrDefault();

            }

            if (PrevAccount == null)
            {
                PrevAccount = new CashFlow();
                if (DbConstants.PaymentMode.BankPaymentModes.Contains(Convert.ToByte(model.PaymentModeKey)))
                {
                    PrevAccount.AccountBalanceAmount = dbContext.BankAccounts.Where(row => row.RowKey == model.BankAccountKey).Select(row => row.CurrentAccountBalance ?? 0).SingleOrDefault();
                }
                else if (DbConstants.PaymentMode.CashPaymentModes.Contains(Convert.ToByte(model.PaymentModeKey)))
                {
                    PrevAccount.AccountBalanceAmount = dbContext.Branches.Where(row => row.RowKey == model.BranchKey).Select(row => row.OpeningCashBalance).SingleOrDefault();
                }

            }
            if (model.CashFlowTypeKey == DbConstants.CashFlowType.In)
            {
                cashFlowModel.AccountBalanceAmount = (PrevAccount.AccountBalanceAmount ?? 0) + model.Amount;
            }
            else
            {
                cashFlowModel.AccountBalanceAmount = (PrevAccount.AccountBalanceAmount ?? 0) - model.Amount;

            }

            cashFlowModel.RowKey = maxKey + 1;
            model.CashFlowKey = cashFlowModel.RowKey;
            cashFlowModel.CashFlowDate = model.CashFlowDate;
            cashFlowModel.CashFlowTypeKey = model.CashFlowTypeKey;
            cashFlowModel.AccountHeadKey = model.AccountHeadKey;
            cashFlowModel.VoucherNumber = model.VoucherNumber;
            cashFlowModel.Amount = Convert.ToDecimal(model.Amount);
            cashFlowModel.PaymentModeKey = model.PaymentModeKey;
            cashFlowModel.PaymentModeSubKey = model.PaymentModeSubKey;
            cashFlowModel.CardNumber = model.CardNumber;
            cashFlowModel.BankAccountKey = model.BankAccountKey;
            cashFlowModel.ChequeOrDDNumber = model.ChequeOrDDNumber;
            cashFlowModel.ChequeOrDDDate = model.ChequeOrDDDate;
            cashFlowModel.TransactionTypeKey = model.TransactionTypeKey;
            cashFlowModel.TransactionKey = model.TransactionKey ?? cashFlowModel.RowKey;
            cashFlowModel.Purpose = model.Purpose;
            cashFlowModel.PaidBy = model.PaidBy;
            cashFlowModel.AuthorizedBy = model.AuthorizedBy;
            cashFlowModel.ReceivedBy = model.ReceivedBy;
            cashFlowModel.OnBehalfOf = model.OnBehalfOf;
            cashFlowModel.Remarks = model.Remarks;
            cashFlowModel.BranchKey = model.BranchKey;
            cashFlowModel.ReferenceNumber = model.ReferenceNumber;
            cashFlowModel.BalanceAmount = model.TotalAmountToBePaid;
            
            cashFlowModel.SerialNumber = configModel.SerialNumber;
            cashFlowModel.ReceiptNumber = configModel.ReceiptNumber;
            cashFlowModel.IsContra = model.IsContra;
            if (model.BankAccountKey != null && model.BankAccountKey != 0)
            {
                var BankAccountList = dbContext.BankAccounts.SingleOrDefault(x => x.RowKey == model.BankAccountKey);
                model.BankAccountName = (BankAccountList.NameInAccount ?? BankAccountList.AccountNumber) + EduSuiteUIResources.Hyphen + BankAccountList.Bank.BankName;
            }
            dbContext.CashFlows.Add(cashFlowModel);
            dbContext.SaveChanges();
            var AccountHeadList = dbContext.AccountHeads.Where(x => x.RowKey == model.AccountHeadKey).FirstOrDefault();
            model.AccountHeadName = AccountHeadList.AccountHeadName;
            long accountHeadKey = model.AccountHeadKey;
            if (accountHeadKey != null && accountHeadKey != 0)
            {
                long bankKey = dbContext.BankAccounts.Where(x => x.AccountHeadKey == accountHeadKey).Select(x => x.RowKey).FirstOrDefault();
                if (bankKey != null && bankKey != 0)
                {
                    BankAccountService bankAccountService = new BankAccountService(dbContext);
                    BankAccountViewModel bankAccountModel = new BankAccountViewModel();
                    bankAccountModel.RowKey = bankKey;
                    if (model.CashFlowTypeKey == DbConstants.CashFlowType.Out)
                    {
                        bankAccountModel.Amount = model.Amount;
                    }
                    else
                    {
                        bankAccountModel.Amount = -(model.Amount ?? 0);
                    }

                    //bankAccountService.UpdateCurrentAccountBalance(bankAccountModel,false, (model.CashFlowTypeKey == DbConstants.CashFlowType.Out)?true:false,null);
                }
            }
            if (model.PaymentModeKey == DbConstants.PaymentMode.Bank)
            {
                BankAccountService bankAccountService = new BankAccountService(dbContext);
                BankAccountViewModel bankAccountModel = new BankAccountViewModel();
                bankAccountModel.RowKey = model.BankAccountKey ?? 0;
                if (model.CashFlowTypeKey == DbConstants.CashFlowType.In)
                {
                    bankAccountModel.Amount = model.Amount;
                }
                else
                {
                    bankAccountModel.Amount = -(model.Amount ?? 0);
                }

                //bankAccountService.UpdateCurrentAccountBalance(bankAccountModel, false, (model.CashFlowTypeKey == DbConstants.CashFlowType.Out) ? true : false, null);
            }
        }
        public void UpdateCashFlowAccount(CashFlowViewModel model)
        {

            CashFlow cashFlowModel = new CashFlow();

            cashFlowModel = dbContext.CashFlows.SingleOrDefault(row => row.RowKey == model.CashFlowKey);
            if (model.CashFlowTypeKey != cashFlowModel.CashFlowTypeKey)
            {
                ConfigurationViewModel ConfigModel = new ConfigurationViewModel();
                ConfigModel.BranchKey = model.BranchKey;
                ConfigModel.ConfigType = model.CashFlowTypeKey == DbConstants.CashFlowType.In ? DbConstants.PaymentReceiptConfigType.Receipt : DbConstants.PaymentReceiptConfigType.Payment;
                ConfigModel.OldConfigType = cashFlowModel.CashFlowTypeKey == DbConstants.CashFlowType.In ? DbConstants.PaymentReceiptConfigType.Receipt : DbConstants.PaymentReceiptConfigType.Payment;
                ConfigModel.SerialNumber = cashFlowModel.SerialNumber ?? 0;
                Configurations.GenerateReceipt(dbContext, ConfigModel);
                cashFlowModel.SerialNumber = ConfigModel.SerialNumber;
                cashFlowModel.ReceiptNumber = ConfigModel.ReceiptNumber;
            }
            model.ReceiptNumber = cashFlowModel.ReceiptNumber;
            model.OldCashFlowTypeKey = cashFlowModel.CashFlowTypeKey;
            model.OldPaymentModeKey = cashFlowModel.PaymentModeKey;
            model.OldAccountHeadKey = cashFlowModel.AccountHeadKey;
            model.OldBankKey = cashFlowModel.BankAccountKey ?? 0;
            decimal oldAmount = cashFlowModel.Amount;
            cashFlowModel.CashFlowDate = model.CashFlowDate;
            cashFlowModel.CashFlowTypeKey = model.CashFlowTypeKey;
            cashFlowModel.AccountHeadKey = model.AccountHeadKey;
            cashFlowModel.VoucherNumber = model.VoucherNumber;
            cashFlowModel.Amount = Convert.ToDecimal(model.Amount);
            cashFlowModel.PaymentModeKey = model.PaymentModeKey;
            cashFlowModel.PaymentModeSubKey = model.PaymentModeSubKey;
            cashFlowModel.CardNumber = model.CardNumber;
            cashFlowModel.BankAccountKey = model.BankAccountKey;
            cashFlowModel.ChequeOrDDNumber = model.ChequeOrDDNumber;
            cashFlowModel.ChequeOrDDDate = model.ChequeOrDDDate;
            cashFlowModel.TransactionTypeKey = model.TransactionTypeKey;
            cashFlowModel.TransactionKey = model.TransactionKey ?? cashFlowModel.RowKey;
            cashFlowModel.Purpose = model.Purpose;
            cashFlowModel.PaidBy = model.PaidBy;
            cashFlowModel.AuthorizedBy = model.AuthorizedBy;
            cashFlowModel.ReceivedBy = model.ReceivedBy;
            cashFlowModel.OnBehalfOf = model.OnBehalfOf;
            cashFlowModel.ReferenceNumber = model.ReferenceNumber;
            cashFlowModel.Remarks = model.Remarks;
            cashFlowModel.BranchKey = model.BranchKey;
            
            cashFlowModel.BalanceAmount = model.TotalAmountToBePaid;
            cashFlowModel.IsContra = model.IsContra;
            if (model.BankAccountKey != null && model.BankAccountKey != 0)
            {
                var BankAccountList = dbContext.BankAccounts.SingleOrDefault(x => x.RowKey == model.BankAccountKey);
                model.BankAccountName = (BankAccountList.NameInAccount ?? BankAccountList.AccountNumber) + EduSuiteUIResources.Hyphen + BankAccountList.Bank.BankName;
            }
            model.AccountHeadName = dbContext.AccountHeads.Where(x => x.RowKey == model.AccountHeadKey).Select(x => x.AccountHeadName).FirstOrDefault();



            if (DbConstants.PaymentMode.BankPaymentModes.Contains(model.OldPaymentModeKey) && model.OldBankKey != (model.BankAccountKey ?? 0))
            {
                BankAccountService bankAccountService = new BankAccountService(dbContext);
                BankAccountViewModel bankAccountModel = new BankAccountViewModel();
                bankAccountModel.RowKey = model.OldBankKey;
                bankAccountModel.Amount = -(oldAmount);
                bankAccountService.UpdateCurrentAccountBalance(bankAccountModel, true, true, oldAmount);
            }
            else if (DbConstants.PaymentMode.BankPaymentModes.Contains(model.OldPaymentModeKey) && DbConstants.PaymentMode.BankPaymentModes.Contains(model.PaymentModeKey) && model.OldBankKey == (model.BankAccountKey ?? 0))
            {

                BankAccountService bankAccountService = new BankAccountService(dbContext);
                BankAccountViewModel bankAccountModel = new BankAccountViewModel();
                bankAccountModel.RowKey = model.BankAccountKey ?? 0;
                bankAccountModel.Amount = cashFlowModel.Amount;
                bankAccountService.UpdateCurrentAccountBalance(bankAccountModel, true, false, oldAmount);

            }
            else if (!DbConstants.PaymentMode.BankPaymentModes.Contains(model.OldPaymentModeKey) && DbConstants.PaymentMode.BankPaymentModes.Contains(model.PaymentModeKey))
            {
                BankAccountService bankAccountService = new BankAccountService(dbContext);
                BankAccountViewModel bankAccountModel = new BankAccountViewModel();
                bankAccountModel.RowKey = model.BankAccountKey ?? 0;
                bankAccountModel.Amount = cashFlowModel.Amount;
                bankAccountService.UpdateCurrentAccountBalance(bankAccountModel, false, false, null);
            }




            //if (model.PaymentModeKey == DbConstants.PaymentMode.Bank)
            //{
            //    BankAccountService bankAccountService = new BankAccountService(dbContext);
            //    BankAccountViewModel bankAccountModel = new BankAccountViewModel();
            //    bankAccountModel.RowKey = model.BankAccountKey ?? 0;
            //    if (model.CashFlowTypeKey == DbConstants.CashFlowType.In)
            //    {
            //        bankAccountModel.Amount = model.Amount;
            //    }
            //    else
            //    {
            //        bankAccountModel.Amount = -(model.Amount ?? 0);
            //    }
            //    bankAccountService.UpdateCurrentAccountBalance(bankAccountModel, true, (model.CashFlowTypeKey == DbConstants.CashFlowType.Out) ? true : false, null);

            //    if (model.OldPaymentModeKey == DbConstants.PaymentMode.Bank)
            //    {
            //        bankAccountModel.RowKey = model.OldBankKey;
            //        if (model.CashFlowTypeKey == DbConstants.CashFlowType.In)
            //        {
            //            bankAccountModel.Amount = -(oldAmount);
            //        }
            //        else
            //        {
            //            bankAccountModel.Amount = oldAmount;
            //        }
            //        bankAccountService.UpdateCurrentAccountBalance(bankAccountModel);
            //    }
            //}
            //else if (model.OldPaymentModeKey == DbConstants.PaymentMode.Bank)
            //{
            //    BankAccountService bankAccountService = new BankAccountService(dbContext);
            //    BankAccountViewModel bankAccountModel = new BankAccountViewModel();
            //    bankAccountModel.RowKey = model.OldBankKey;
            //    if (model.CashFlowTypeKey == DbConstants.CashFlowType.In)
            //    {
            //        bankAccountModel.Amount = -(oldAmount);
            //    }
            //    else
            //    {
            //        bankAccountModel.Amount = oldAmount;
            //    }
            //    bankAccountService.UpdateCurrentAccountBalance(bankAccountModel);
            //}

        }



        public void DeleteCashFlowAccount(CashFlow cashFlowModel)
        {

            UpdateAccountWhenDelete(cashFlowModel);
            dbContext.CashFlows.Remove(cashFlowModel);
        }

        private void UpdateBankAccount(CashFlowViewModel model, CashFlow OldAccount)
        {
            BankAccount bankAccountModel = new BankAccount();
            Branch branchModel = new Branch();
            if (DbConstants.PaymentMode.BankPaymentModes.Contains(Convert.ToByte(model.PaymentModeKey)))
            {
                bankAccountModel = dbContext.BankAccounts.SingleOrDefault(row => row.RowKey == model.BankAccountKey);
                if (model.CashFlowTypeKey == DbConstants.CashFlowType.In)
                {
                    bankAccountModel.CurrentAccountBalance = (bankAccountModel.CurrentAccountBalance ?? 0) + (model.Amount ?? 0);
                }
                else
                {
                    bankAccountModel.CurrentAccountBalance = (bankAccountModel.CurrentAccountBalance ?? 0) - (model.Amount ?? 0);

                }
            }
            //else if (DbConstants.PaymentMode.CashPaymentModes.Contains(Convert.ToByte(model.PaymentModeKey)))
            //{

            //    branchModel = dbContext.CompanyBranches.SingleOrDefault(row => row.RowKey == model.BranchKey);
            //    if (model.CashFlowTypeKey == DbConstants.CashFlowType.In)
            //    {
            //        branchModel.OpeningBalance = (branchModel.OpeningBalance) + (model.Amount ?? 0);
            //    }
            //    else
            //    {
            //        branchModel.OpeningBalance = (branchModel.OpeningBalance) - (model.Amount ?? 0);

            //    }

            //}
            if (DbConstants.PaymentMode.BankPaymentModes.Contains(Convert.ToByte(OldAccount.PaymentModeKey)))
            {

                bankAccountModel = dbContext.BankAccounts.SingleOrDefault(row => row.RowKey == OldAccount.BankAccountKey);
                if (model.CashFlowTypeKey == DbConstants.CashFlowType.In)
                {
                    bankAccountModel.CurrentAccountBalance = (bankAccountModel.CurrentAccountBalance ?? 0) - OldAccount.Amount;
                }
                else
                {
                    bankAccountModel.CurrentAccountBalance = (bankAccountModel.CurrentAccountBalance ?? 0) + (OldAccount.Amount);
                }
            }
            //else if (DbConstants.PaymentMode.CashPaymentModes.Contains(Convert.ToByte(OldAccount.PaymentModeKey)))
            //{

            //    branchModel = dbContext.CompanyBranches.SingleOrDefault(row => row.RowKey == OldAccount.BranchKey);
            //    if (model.CashFlowTypeKey == DbConstants.CashFlowType.In)
            //    {
            //        branchModel.OpeningBalance = (branchModel.OpeningBalance - OldAccount.Amount);
            //    }
            //    else
            //    {
            //        branchModel.OpeningBalance = (branchModel.OpeningBalance + OldAccount.Amount);

            //    }
            //}

        }

        private void UpdateAccountWhenDelete(CashFlow cashFlow)
        {
            BankAccount bankAccountModel = new BankAccount();
            Branch branchModel = new Branch();
            CashFlow oldCashflowModel = new CashFlow();
            if (DbConstants.PaymentMode.BankPaymentModes.Contains(Convert.ToByte(cashFlow.PaymentModeKey)))
            {
                oldCashflowModel = dbContext.CashFlows.Where(row => row.BankAccountKey == cashFlow.BankAccountKey && DbConstants.PaymentMode.BankPaymentModes.Contains((byte)row.PaymentModeKey) && row.RowKey < cashFlow.RowKey).OrderByDescending(row => row.DateAdded).FirstOrDefault();
                bankAccountModel = dbContext.BankAccounts.SingleOrDefault(row => row.RowKey == cashFlow.BankAccountKey);
                if (oldCashflowModel != null)
                {
                    bankAccountModel.CurrentAccountBalance = oldCashflowModel.AccountBalanceAmount;
                }
                else if (!dbContext.CashFlows.Where(row => row.BankAccountKey == cashFlow.BankAccountKey && DbConstants.PaymentMode.CashPaymentModes.Contains((byte)row.PaymentModeKey) && row.RowKey < cashFlow.RowKey).Any())
                {
                    bankAccountModel.CurrentAccountBalance = bankAccountModel.OpeningAccountBalance;
                }
            }
            //else if (DbConstants.PaymentMode.CashPaymentModes.Contains(Convert.ToByte(cashFlow.PaymentModeKey)))
            //{

            //    branchModel = dbContext.CompanyBranches.SingleOrDefault(row => row.RowKey == cashFlow.BranchKey);
            //    oldCashflowModel = dbContext.CashFlows.Where(row => row.BranchKey == cashFlow.BranchKey && DbConstants.PaymentMode.CashPaymentModes.Contains((byte)row.PaymentModeKey) && row.RowKey < cashFlow.RowKey).OrderByDescending(row => row.CreatedDate).FirstOrDefault();

            //    if (oldCashflowModel != null)
            //    {
            //        branchModel.OpeningBalance = oldCashflowModel.AccountBalanceAmount ?? 0;
            //    }
            //    else if (!dbContext.CashFlows.Where(row => row.BranchKey == cashFlow.BranchKey && DbConstants.PaymentMode.CashPaymentModes.Contains((byte)row.PaymentModeKey) && row.RowKey != cashFlow.RowKey).Any())
            //    {
            //        branchModel.OpeningBalance = branchModel.OpeningBalance;
            //    }

            //}

        }
    }
}
