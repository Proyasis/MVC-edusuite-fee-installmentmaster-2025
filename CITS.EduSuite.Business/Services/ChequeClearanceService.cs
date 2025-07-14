using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.Security;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Services
{
    public class ChequeClearanceService : IChequeClearanceService
    {
        private EduSuiteDatabase dbContext;
        public ChequeClearanceService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public ChequeClearanceViewModel GetChequeClearanceById(ChequeClearanceViewModel model)
        {
            try
            {
                model = dbContext.VwChequeClearanceSelects.Where(row => row.TransactionTypeKey == model.TransactionTypeKey && row.TransactionKey == model.TransactionKey).Select(row => new ChequeClearanceViewModel
                {
                    RowKey = row.TransactionKey,
                    ChequeOrDDNumber = row.ChequeOrDDNumber,
                    //AccountHeadName = row.AccountHeadName,
                    AccountHeadKey = row.AccountHeadKey ?? 0,
                    ChequeOrDDDate = row.ChequeClearanceDate,
                    TransactionTypeKey = row.TransactionTypeKey ?? 0,
                    TransactionKey = row.TransactionKey,
                    Amount = row.Amount,
                    IsApproved = row.IsApproved ?? true,
                    IsAdvance = row.IsAdvance ?? false,
                    CashFlowTypeKey = row.CashFlowTypeKey ?? 0,
                    BankAccountKey = row.BankAccountKey,
                    OldBankAccountKey = row.BankAccountKey,
                    BranchKey = row.BranchKey ?? 0,
                    Purpose = row.Purpose,
                    //IsOrderPayment = row.IsOrderPayment ?? false
                }).FirstOrDefault();

                FillDropdownLists(model);
                if (model == null)
                {
                    model = new ChequeClearanceViewModel();
                }
                return model;
            }
            catch (Exception ex)
            {
                model = new ChequeClearanceViewModel();
                model.Message = ex.GetBaseException().Message;
                return model;
            }
        }
        public ChequeClearanceViewModel CreateChequeClearance(ChequeClearanceViewModel model)
        {

            FillDropdownLists(model);

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    long MaxKey = dbContext.ChequeClearances.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    createChequeClearance(model, MaxKey);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    Exception raise = dbEx;
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = string.Format("{0}:{1}", validationErrors.Entry.Entity.ToString(), validationError.ErrorMessage);
                            //raise a new exception inserting the current one as the InnerException
                            raise = new InvalidOperationException(message, raise);
                        }
                        throw raise;
                    }

                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationPersenol, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, dbEx.GetBaseException().Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.ChequeClearance);
                    model.IsSuccessful = false;
                }
            }

            return model;
        }
        public void createChequeClearance(ChequeClearanceViewModel model, long MaxKey)
        {
            ChequeClearance ChequeClearanceModel = new ChequeClearance();

            ChequeClearanceModel.RowKey = Convert.ToInt32(MaxKey + 1);
            ChequeClearanceModel.ClearanceDate = model.ClearanceDate;
            ChequeClearanceModel.PaymentModeKey = model.PaymentModeKey;
            ChequeClearanceModel.BankAccountKey = model.BankAccountKey;
            ChequeClearanceModel.TransactionKey = model.TransactionKey;
            ChequeClearanceModel.TransactionTypeKey = model.TransactionTypeKey;
            ChequeClearanceModel.Purpose = model.Purpose;
            ChequeClearanceModel.IsApproved = model.IsApproved;
            ChequeClearanceModel.Remark = model.Remark;
            model.RowKey = ChequeClearanceModel.RowKey;

            dbContext.ChequeClearances.Add(ChequeClearanceModel);

            if (model.TransactionTypeKey == DbConstants.TransactionType.Salary)
            {
                EmployeeSalaryPayment EmployeeSalaryPaymentModel = new EmployeeSalaryPayment();
                EmployeeSalaryPaymentModel = dbContext.EmployeeSalaryPayments.SingleOrDefault(row => row.RowKey == model.TransactionKey);
                EmployeeSalaryPaymentModel.ChequeStatusKey = model.IsApproved == false ? DbConstants.ProcessStatus.Rejected : DbConstants.ProcessStatus.Approved;
            }
            else if (model.TransactionTypeKey == DbConstants.TransactionType.SalaryAdvance)
            {
                EmployeeSalaryAdvancePayment EmployeeSalaryAdvancePaymentModel = new EmployeeSalaryAdvancePayment();
                EmployeeSalaryAdvancePaymentModel = dbContext.EmployeeSalaryAdvancePayments.SingleOrDefault(row => row.RowKey == model.TransactionKey);
                EmployeeSalaryAdvancePaymentModel.ChequeStatusKey = model.IsApproved == false ? DbConstants.ProcessStatus.Rejected : DbConstants.ProcessStatus.Approved;
            }
            else if (model.TransactionTypeKey == DbConstants.TransactionType.CashFlow)
            {
                CashFlow CashFlowModel = new CashFlow();
                CashFlowModel = dbContext.CashFlows.SingleOrDefault(row => row.RowKey == model.TransactionKey);

                CashFlowModel.ChequeStatusKey = model.IsApproved == false ? DbConstants.ProcessStatus.Rejected : DbConstants.ProcessStatus.Approved;
            }
            else if (model.TransactionTypeKey == DbConstants.TransactionType.FeeMaster)
            {
                FeePaymentMaster FeePaymentMasterModel = new FeePaymentMaster();
                FeePaymentMasterModel = dbContext.FeePaymentMasters.SingleOrDefault(row => row.RowKey == model.TransactionKey);
                FeePaymentMasterModel.ChequeStatusKey = model.IsApproved == false ? DbConstants.ProcessStatus.Rejected : DbConstants.ProcessStatus.Approved;
                FeePaymentMasterModel.ChequeApprovedRejectedDate = model.ClearanceDate;
            }
            else if (model.TransactionTypeKey == DbConstants.TransactionType.UniversityFee)
            {
                UniversityPaymentMaster UniversityPaymentMasterModel = new UniversityPaymentMaster();
                UniversityPaymentMasterModel = dbContext.UniversityPaymentMasters.SingleOrDefault(row => row.RowKey == model.TransactionKey);
                UniversityPaymentMasterModel.ChequeStatusKey = model.IsApproved == false ? DbConstants.ProcessStatus.Rejected : DbConstants.ProcessStatus.Approved;
                UniversityPaymentMasterModel.ChequeApprovedRejectedDate = model.ClearanceDate;

            }
            else if (model.TransactionTypeKey == DbConstants.TransactionType.FutureTransactionPayment)
            {
                FutureTransactionPayment futuretransactionPayment = new FutureTransactionPayment();
                futuretransactionPayment = dbContext.FutureTransactionPayments.SingleOrDefault(row => row.RowKey == model.TransactionKey);
                futuretransactionPayment.ChequeStatusKey = model.IsApproved == false ? DbConstants.ProcessStatus.Rejected : DbConstants.ProcessStatus.Approved;
            }
            byte TransactionTypeKey = model.TransactionTypeKey;
            if (model.TransactionTypeKey == DbConstants.TransactionType.CashFlow)
            {
                model.TransactionTypeKey = dbContext.CashFlows.Where(x => x.RowKey == model.TransactionKey).Select(x => x.TransactionTypeKey ?? 0).FirstOrDefault(); ;
            }
            if (model.IsApproved != false)
            {
                if (model.PaymentModeKey != DbConstants.PaymentMode.Bank)
                {
                    List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
                    accountFlowModelList = CreditAmountList(model, accountFlowModelList);
                    accountFlowModelList = DebitAmountList(model, accountFlowModelList);
                    CreateAccountFlow(accountFlowModelList);
                }
                else if (model.PaymentModeKey == DbConstants.PaymentMode.Bank)
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
                    bankAccountService.UpdateCurrentAccountBalance(bankAccountModel, true, (model.CashFlowTypeKey == DbConstants.CashFlowType.Out) ? true : false, model.Amount);
                }
            }

            else
            {
                //List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
                //accountFlowModelList = CreditAmountList(model, accountFlowModelList);
                //accountFlowModelList = DebitAmountList(model, accountFlowModelList);
                //CreateAccountFlow(accountFlowModelList);
                List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
                var AccountFlowList = dbContext.AccountFlows.Where(x => x.TransactionKey == model.TransactionKey && x.TransactionTypeKey == model.TransactionTypeKey).ToList();


                foreach (var item in AccountFlowList)
                {
                    model.CashFlowTypeKey = item.CashFlowTypeKey == DbConstants.CashFlowType.In ? DbConstants.CashFlowType.Out : DbConstants.CashFlowType.In;
                    model.AccountHeadKey = item.AccountHeadKey;
                    model.Purpose = "Rejected " + item.Purpose;
                    model.Amount = item.Amount;
                    model.ClearanceDate = item.TransactionDate;
                    model.TransactionTypeKey = item.TransactionTypeKey;
                    model.RowKey = item.TransactionKey;
                    model.BranchKey = item.BranchKey;
                    //model.AccountHeadKey = dbContext.AccountHeads.Where(x => x.AccountHeadCode == item.AccountHeadCode).Select(x => x.RowKey).SingleOrDefault();

                    accountFlowModelList = DebitAmountList(model, accountFlowModelList);
                }

                //accountFlowModelList = CreditAmountList(model, accountFlowModelList);
                //accountFlowModelList = DebitAmountList(model, accountFlowModelList);

                CreateAccountFlow(accountFlowModelList);
            }



            model.RowKey = ChequeClearanceModel.RowKey;
            model.TransactionTypeKey = TransactionTypeKey;
        }
        public List<ChequeClearanceViewModel> GetChequeClearance(string searchText, short branchKey)
        {
            try
            {
                var ChequeClearanceList = (from p in dbContext.VwChequeClearanceSelects
                                           select new ChequeClearanceViewModel
                                           {
                                               RowKey = p.TransactionKey,
                                               ChequeOrDDNumber = p.ChequeOrDDNumber,
                                               //AccountHeadName = p.AccountHeadName,
                                               ChequeOrDDDate = p.ChequeClearanceDate,
                                               TransactionTypeName = p.TransactionTypeName,
                                               TransactionTypeKey = p.TransactionTypeKey ?? 0,
                                               TransactionKey = p.TransactionKey,
                                               CashFlowTypeKey = p.CashFlowTypeKey ?? 0,
                                               CashFlowTypeName = p.CashFlowTypeKey == DbConstants.CashFlowType.In ? EduSuiteUIResources.Receipt : EduSuiteUIResources.Payment,
                                               Amount = p.Amount,
                                               BranchKey = p.BranchKey ?? 0,
                                               BranchName = p.BranchName
                                           }).ToList();
                if (branchKey != 0)
                {
                    ChequeClearanceList = ChequeClearanceList.Where(x => x.BranchKey == branchKey).ToList();
                }
                return ChequeClearanceList.ToList<ChequeClearanceViewModel>();
            }
            catch (Exception ex)
            {
                return new List<ChequeClearanceViewModel>();

            }
        }
        private void FillDropdownLists(ChequeClearanceViewModel model)
        {
            FillPaymentModes(model);
            FillBankAccounts(model);
        }
        private void FillPaymentModes(ChequeClearanceViewModel model)
        {
            model.PaymentModes = dbContext.PaymentModes.Where(row => row.IsActive == true && row.RowKey != DbConstants.PaymentMode.Cheque).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.PaymentModeName
            }).ToList();
        }
        private void FillBankAccounts(ChequeClearanceViewModel model)
        {
            model.BankAccounts = dbContext.BranchAccounts.Where(x => x.BranchKey == model.BranchKey && x.BankAccount.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.BankAccount.RowKey,
                Text = (row.BankAccount.NameInAccount ?? row.BankAccount.AccountNumber) + EduSuiteUIResources.Hyphen + row.BankAccount.Bank.BankName
            }).ToList();

        }

        #region Account
        private void CreateAccountFlow(List<AccountFlowViewModel> modelList)
        {
            AccountFlowService accounFlowService = new AccountFlowService(dbContext);
            List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
            accounFlowService.CreateAccountFlow(modelList);
        }
        private List<AccountFlowViewModel> CreditAmountList(ChequeClearanceViewModel model, List<AccountFlowViewModel> accountFlowModelList)
        {
            long accountHeadKey = dbContext.BankAccounts.Where(x => x.RowKey == model.OldBankAccountKey).Select(x => x.AccountHeadKey).FirstOrDefault();
            long ExtraUpdateKey = 0;
            if (model.CashFlowTypeKey == DbConstants.CashFlowType.Out)
            {
                accountFlowModelList.Add(new AccountFlowViewModel
                {
                    CashFlowTypeKey = DbConstants.CashFlowType.In,
                    AccountHeadKey = accountHeadKey,
                    Amount = model.Amount ?? 0,
                    TransactionTypeKey = DbConstants.TransactionType.ChequeClearance,
                    //VoucherTypeKey = model.TransactionTypeKey == DbConstants.TransactionType.FeeMaster ? DbConstants.VoucherType.Fee : DbConstants.VoucherType.UniversityFee,
                    VoucherTypeKey = DbConstants.VoucherType.Payment,
                    TransactionKey = model.RowKey,
                    TransactionDate = model.ClearanceDate,
                    ExtraUpdateKey = ExtraUpdateKey,
                    BranchKey = model.BranchKey,
                    IsUpdate = false,
                    Purpose = model.Purpose + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.ChequeClearance + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.OpenBracket + (model.IsApproved == true ? EduSuiteUIResources.Approved : EduSuiteUIResources.Rejected) + EduSuiteUIResources.CloseBracket + EduSuiteUIResources.BlankSpace + (model.PaymentModeKey == DbConstants.PaymentMode.Cash ? EduSuiteUIResources.Cash : EduSuiteUIResources.BlankSpace),
                });
            }
            else
            {
                accountFlowModelList.Add(new AccountFlowViewModel
                {
                    CashFlowTypeKey = DbConstants.CashFlowType.Out,
                    AccountHeadKey = accountHeadKey,
                    Amount = model.Amount ?? 0,
                    TransactionTypeKey = DbConstants.TransactionType.ChequeClearance,
                    //VoucherTypeKey = model.TransactionTypeKey == DbConstants.TransactionType.FeeMaster ? DbConstants.VoucherType.Fee : DbConstants.VoucherType.UniversityFee,
                    VoucherTypeKey = DbConstants.VoucherType.Reciept,
                    TransactionKey = model.RowKey,
                    TransactionDate = model.ClearanceDate,
                    ExtraUpdateKey = ExtraUpdateKey,
                    BranchKey = model.BranchKey,
                    IsUpdate = false,
                    Purpose = model.Purpose + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.ChequeClearance + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.OpenBracket + (model.IsApproved == true ? EduSuiteUIResources.Approved : EduSuiteUIResources.Rejected) + EduSuiteUIResources.CloseBracket + EduSuiteUIResources.BlankSpace + (model.PaymentModeKey == DbConstants.PaymentMode.Cash ? EduSuiteUIResources.Cash : EduSuiteUIResources.BlankSpace),
                });
            }
            return accountFlowModelList;
        }
        private List<AccountFlowViewModel> DebitAmountList(ChequeClearanceViewModel model, List<AccountFlowViewModel> accountFlowModelList)
        {
            long ExtraUpdateKey = 0;
            long accountHeadKey = 0;
            if (model.IsApproved == true)
            {
                if (model.PaymentModeKey == DbConstants.PaymentMode.Bank)
                {
                    if (model.BankAccountKey != model.OldBankAccountKey)
                    {
                        accountHeadKey = dbContext.BankAccounts.Where(x => x.RowKey == model.BankAccountKey).Select(x => x.AccountHeadKey).FirstOrDefault();
                    }
                }
                else
                {
                    accountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.CashAccount).Select(x => x.RowKey).FirstOrDefault();
                }
            }
            else
            {
                accountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == model.AccountHeadKey).Select(x => x.RowKey).FirstOrDefault();

            }



            if (model.CashFlowTypeKey == DbConstants.CashFlowType.Out)
            {
                //if (model.TransactionTypeKey == DbConstants.TransactionType.Fee || model.TransactionTypeKey == DbConstants.TransactionType.FeeDetails || (model.TransactionTypeKey == DbConstants.TransactionType.CashFlow && model.IsOrderPayment == true && model.CashFlowTypeKey == DbConstants.CashFlowType.Out))
                //{
                // accountHeadCode = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.AccountsPayable).Select(x => x.AccountHeadCode).FirstOrDefault();
                accountFlowModelList.Add(new AccountFlowViewModel
                {
                    CashFlowTypeKey = DbConstants.CashFlowType.Out,
                    AccountHeadKey = accountHeadKey,
                    Amount = model.Amount ?? 0,
                    //TransactionTypeKey = DbConstants.TransactionType.ChequeClearancePayable,
                    //VoucherTypeKey = model.TransactionTypeKey == DbConstants.TransactionType.FeeMaster ? DbConstants.VoucherType.Fee : DbConstants.VoucherType.UniversityFee,
                    TransactionTypeKey = DbConstants.TransactionType.ChequeClearance,
                    VoucherTypeKey = DbConstants.VoucherType.Payment,
                    TransactionKey = model.RowKey,
                    TransactionDate = model.ClearanceDate,
                    ExtraUpdateKey = ExtraUpdateKey,
                    BranchKey = model.BranchKey,
                    IsUpdate = false,
                    Purpose = model.Purpose + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.ChequeClearance + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.OpenBracket + (model.IsApproved == true ? EduSuiteUIResources.Approved : EduSuiteUIResources.Rejected) + EduSuiteUIResources.CloseBracket + EduSuiteUIResources.BlankSpace + (model.PaymentModeKey == DbConstants.PaymentMode.Cash ? EduSuiteUIResources.Cash : EduSuiteUIResources.BlankSpace),
                });
            }
            //else if (model.TransactionTypeKey == DbConstants.TransactionType.UniversityFee || (model.TransactionTypeKey == DbConstants.TransactionType.CashFlow && model.IsOrderPayment == true && model.CashFlowTypeKey == DbConstants.CashFlowType.In))
            //{
            //    if (model.IsAdvance != true)
            //    {
            //        accountHeadCode = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.AccountsReceivable).Select(x => x.AccountHeadCode).FirstOrDefault();
            //    }
            //    else
            //    {
            //        accountHeadCode = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.AdvancePayable).Select(x => x.AccountHeadCode).FirstOrDefault();
            //    }
            else
            {
                accountFlowModelList.Add(new AccountFlowViewModel
                {
                    CashFlowTypeKey = DbConstants.CashFlowType.In,
                    AccountHeadKey = accountHeadKey,
                    Amount = model.Amount ?? 0,
                    //TransactionTypeKey = DbConstants.TransactionType.ChequeClearanceRecievable,
                    //VoucherTypeKey = DbConstants.VoucherType.UniversityFee,
                    TransactionTypeKey = DbConstants.TransactionType.ChequeClearance,
                    VoucherTypeKey = DbConstants.VoucherType.Reciept,
                    TransactionKey = model.RowKey,
                    TransactionDate = model.ClearanceDate,
                    ExtraUpdateKey = ExtraUpdateKey,
                    BranchKey = model.BranchKey,
                    IsUpdate = false,
                    Purpose = model.Purpose + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.ChequeClearance + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.OpenBracket + (model.IsApproved == true ? EduSuiteUIResources.Approved : EduSuiteUIResources.Rejected) + EduSuiteUIResources.CloseBracket + EduSuiteUIResources.BlankSpace + (model.PaymentModeKey == DbConstants.PaymentMode.Cash ? EduSuiteUIResources.Cash : EduSuiteUIResources.BlankSpace),
                });
            }
            // accountHeadCode = dbContext.AccountHeads.Where(x => x.RowKey == model.AccountHeadKey).Select(x => x.AccountHeadCode).FirstOrDefault();
            //}

            //if (model.CashFlowTypeKey == DbConstants.CashFlowType.Out)
            //{
            //    accountFlowModelList.Add(new AccountFlowViewModel
            //    {
            //        CashFlowTypeKey = DbConstants.CashFlowType.Out,
            //        AccountHeadCode = accountHeadCode,
            //        Amount = model.Amount ?? 0,
            //        TransactionTypeKey = DbConstants.TransactionType.ChequeClearance,
            //        VoucherTypeKey = model.TransactionTypeKey == DbConstants.TransactionType.Purchase ? DbConstants.VoucherType.Purchase : model.TransactionTypeKey == DbConstants.TransactionType.OutSource ? DbConstants.VoucherType.OutSource : model.TransactionTypeKey == DbConstants.TransactionType.Salary ? DbConstants.VoucherType.Salary : model.TransactionTypeKey == DbConstants.TransactionType.SalaryAdvance ? DbConstants.VoucherType.SalaryAdvance : DbConstants.VoucherType.Payment,
            //        TransactionKey = model.RowKey,
            //        TransactionDate = model.ClearanceDate,
            //        ExtraUpdateKey = ExtraUpdateKey,
            //        BranchKey = model.BranchKey,
            //        IsUpdate = false,
            //        Purpose = model.Purpose + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.ChequeClearance + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.OpenBracket + (model.IsApproved == true ? EduSuiteUIResources.Approved : EduSuiteUIResources.Rejected) + EduSuiteUIResources.CloseBracket + EduSuiteUIResources.BlankSpace + (model.PaymentModeKey == DbConstants.PaymentMode.Cash ? EduSuiteUIResources.Cash : EduSuiteUIResources.BlankSpace),
            //    });
            //}
            //else
            //{
            //    accountFlowModelList.Add(new AccountFlowViewModel
            //    {
            //        CashFlowTypeKey = DbConstants.CashFlowType.In,
            //        AccountHeadCode = accountHeadCode,
            //        Amount = model.Amount ?? 0,
            //        TransactionTypeKey = DbConstants.TransactionType.ChequeClearance,
            //        VoucherTypeKey = model.TransactionTypeKey == DbConstants.TransactionType.Sales || model.TransactionTypeKey == DbConstants.TransactionType.RawMaterialSales ? DbConstants.VoucherType.Sales : DbConstants.VoucherType.Reciept,
            //        TransactionKey = model.RowKey,
            //        TransactionDate = model.ClearanceDate,
            //        ExtraUpdateKey = ExtraUpdateKey,
            //        BranchKey = model.BranchKey,
            //        IsUpdate = false,
            //        Purpose = model.Purpose + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.ChequeClearance + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.OpenBracket + (model.IsApproved == true ? EduSuiteUIResources.Approved : EduSuiteUIResources.Rejected) + EduSuiteUIResources.CloseBracket + EduSuiteUIResources.BlankSpace + (model.PaymentModeKey == DbConstants.PaymentMode.Cash ? EduSuiteUIResources.Cash : EduSuiteUIResources.BlankSpace),
            //    });
            //}
            return accountFlowModelList;
        }
        #endregion
        public decimal CheckShortBalance(short PaymentModeKey, long Rowkey, long BankAccountKey, short branchKey)
        {
            decimal Balance = 0;
            if (PaymentModeKey == DbConstants.PaymentMode.Cash)
            {
                long accountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.CashAccount).Select(x => x.RowKey).FirstOrDefault();
                decimal totalDebit = dbContext.AccountFlows.Where(x => x.AccountHeadKey == accountHeadKey && x.CashFlowTypeKey == DbConstants.CashFlowType.In && x.BranchKey == branchKey).Select(x => x.Amount).DefaultIfEmpty().Sum();
                decimal totalCredit = dbContext.AccountFlows.Where(x => x.AccountHeadKey == accountHeadKey && x.CashFlowTypeKey == DbConstants.CashFlowType.Out && x.BranchKey == branchKey).Select(x => x.Amount).DefaultIfEmpty().Sum();
                Balance = totalDebit - totalCredit;
            }
            else if (PaymentModeKey == DbConstants.PaymentMode.Bank || PaymentModeKey == DbConstants.PaymentMode.Cheque)
            {
                if (BankAccountKey != 0 && BankAccountKey != null)
                {
                    Balance = dbContext.BankAccounts.Where(x => x.RowKey == BankAccountKey).Select(x => x.CurrentAccountBalance ?? 0).FirstOrDefault();
                }
            }
            return Balance;
        }
        public ChequeClearanceViewModel FillBranch(ChequeClearanceViewModel model)
        {
            if (model.BranchKey == 0 || model.BranchKey == null)
            {
                model.BranchKey = dbContext.Employees.Where(row => row.AppUserKey == DbConstants.User.UserKey).Select(row => row.BranchKey).FirstOrDefault();
                if (model.BranchKey == 0)
                {
                    model.Branches = dbContext.vwBranchSelectActiveOnlies.Select(row => new SelectListModel
                    {
                        RowKey = row.RowKey,
                        Text = row.BranchName
                    }).ToList();
                    if (model.Branches.Count == 1)
                    {
                        model.BranchKey = dbContext.vwBranchSelectActiveOnlies.Select(x => x.RowKey).FirstOrDefault();
                    }
                }
            }
            return model;
        }

        //#region TriggerDatabase
        //public void TriggerDatabaseInBackground(ChequeClearanceViewModel model)
        //{
        //    if (Thread.CurrentPrincipal.Identity.IsAuthenticated && (Thread.CurrentPrincipal as CITSPrintSWPrincipal).RoleKey != DbConstants.TaxUserRoleKey && ConnectionTool.CheckConnection())
        //    {

        //        Thread bgThread = new Thread(new ParameterizedThreadStart(TriggerDatabase));
        //        bgThread.IsBackground = true;
        //        bgThread.Start(model);
        //    }

        //}
        //private void TriggerDatabase(Object ObjViewModel)
        //{
        //    try
        //    {
        //        ChequeClearanceViewModel model = (ChequeClearanceViewModel)ObjViewModel;
        //        if (model.TransactionTypeKey == DbConstants.TransactionType.Sales)
        //        {
        //            model.TransactionKey = dbContext.SalesOrderReceipts.Where(row => row.RowKey == model.TransactionKey).Select(row => row.RefKey ?? 0).SingleOrDefault();
        //        }
        //        else if (model.TransactionTypeKey == DbConstants.TransactionType.RawMaterialSales)
        //        {
        //            model.TransactionKey = dbContext.RawMaterialSalesPayments.Where(row => row.RowKey == model.TransactionKey).Select(row => row.RefKey ?? 0).SingleOrDefault();
        //        }
        //        else if (model.TransactionTypeKey == DbConstants.TransactionType.Purchase)
        //        {
        //            model.TransactionKey = dbContext.PurchaseOrderPayments.Where(row => row.RowKey == model.TransactionKey).Select(row => row.RefKey ?? 0).SingleOrDefault();
        //        }
        //        else if (model.TransactionTypeKey == DbConstants.TransactionType.OutSource)
        //        {
        //            model.TransactionKey = dbContext.SalesOrderOutSourcePayments.Where(row => row.RowKey == model.TransactionKey).Select(row => row.RefKey ?? 0).SingleOrDefault();
        //        }
        //        else if (model.TransactionTypeKey == DbConstants.TransactionType.CashFlow)
        //        {
        //            model.TransactionKey = dbContext.CashFlows.Where(row => row.RowKey == model.TransactionKey).Select(row => row.RefKey ?? 0).SingleOrDefault();

        //        }
        //        dbContext = new CITSPrintSWDatabase2();
        //        var RowKey = model.RowKey;
        //        model.IsSecond = true;
        //        if (model.UpdateType == DbConstants.UpdationType.Create)
        //        {
        //            CreateChequeClearance(model);
        //            dbContext = new CITSPrintSWDatabase();
        //            ChequeClearance ChequeClearanceModel = dbContext.ChequeClearances.SingleOrDefault(row => row.RowKey == RowKey);
        //            if (ChequeClearanceModel != null)
        //            {
        //                ChequeClearanceModel.RefKey = model.RowKey;
        //            }
        //            dbContext.SaveChanges();
        //        }


        //        //Thread.CurrentThread.IsAlive();
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    finally
        //    {
        //        dbContext = new CITSPrintSWDatabase();
        //        Thread.CurrentThread.Abort();
        //    }
        //}

        //#endregion
    }
}
