using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System.Linq.Expressions;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class CashFlowService : ICashFlowService
    {
        private EduSuiteDatabase dbContext;
        public CashFlowService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        #region CashFlow

        public decimal CheckShortBalance(short PaymentModeKey, long Rowkey, long BankAccountKey, short branchKey, byte CashFlowTypeKey)
        {
            decimal Balance = 0;
            if (PaymentModeKey == DbConstants.PaymentMode.Cash)
            {
                long accountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.CashAccount).Select(x => x.RowKey).FirstOrDefault();
                decimal totalDebit = dbContext.AccountFlows.Where(x => x.AccountHeadKey == accountHeadKey && x.CashFlowTypeKey == DbConstants.CashFlowType.In && x.BranchKey == branchKey).Select(x => x.Amount).DefaultIfEmpty().Sum();
                decimal totalCredit = dbContext.AccountFlows.Where(x => x.AccountHeadKey == accountHeadKey && x.CashFlowTypeKey == DbConstants.CashFlowType.Out && x.BranchKey == branchKey).Select(x => x.Amount).DefaultIfEmpty().Sum();
                Balance = totalDebit - totalCredit;
                if (Rowkey != 0)
                {
                    var purchaseList = dbContext.CashFlows.SingleOrDefault(x => x.RowKey == Rowkey);
                    if (PaymentModeKey == purchaseList.PaymentModeKey)
                    {
                        Balance = Balance + (CashFlowTypeKey == DbConstants.CashFlowType.In ? -purchaseList.Amount : purchaseList.Amount);
                    }
                }
            }
            else if (PaymentModeKey == DbConstants.PaymentMode.Bank || PaymentModeKey == DbConstants.PaymentMode.Cheque)
            {
                if (BankAccountKey != 0 && BankAccountKey != null)
                {
                    long accountHeadKey = dbContext.BankAccounts.Where(x => x.RowKey == BankAccountKey).Select(x => x.AccountHeadKey).FirstOrDefault();
                    decimal totalDebit = dbContext.AccountFlows.Where(x => x.AccountHeadKey == accountHeadKey && x.CashFlowTypeKey == DbConstants.CashFlowType.In).Select(x => x.Amount).DefaultIfEmpty().Sum();
                    decimal totalCredit = dbContext.AccountFlows.Where(x => x.AccountHeadKey == accountHeadKey && x.CashFlowTypeKey == DbConstants.CashFlowType.Out).Select(x => x.Amount).DefaultIfEmpty().Sum();
                    Balance = totalDebit - totalCredit;
                    if (Rowkey != 0)
                    {
                        var purchaseList = dbContext.CashFlows.SingleOrDefault(x => x.RowKey == Rowkey);
                        if (BankAccountKey == purchaseList.BankAccountKey)
                        {
                            //Balance = Balance + purchaseList.Amount;
                            Balance = Balance + (CashFlowTypeKey == DbConstants.CashFlowType.In ? -purchaseList.Amount : purchaseList.Amount);
                        }
                    }
                }
            }
            return Balance;
        }
        public List<CashFlowViewModel> GetCashFlows(CashFlowViewModel model)
        {
            List<CashFlowViewModel> modelCashflow = new List<CashFlowViewModel>();
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;
                var cashflowList = (from c in dbContext.CashFlows.Where(x => model.BranchKey != 0 ? (x.TransactionTypeKey == DbConstants.TransactionType.CashFlow && x.BranchKey == model.BranchKey) : (x.TransactionTypeKey == DbConstants.TransactionType.CashFlow))
                                    join ah in dbContext.AccountHeads on c.AccountHeadKey equals ah.RowKey
                                    where (ah.AccountHeadName.Contains(model.AccountHeadName) || ah.AccountHeadCode.Contains(model.AccountHeadName) || c.ReceiptNumber.Contains(model.AccountHeadName))
                                    orderby c.DateAdded descending
                                    select new CashFlowViewModel
                                    {
                                        CashFlowKey = c.RowKey,
                                        CashFlowDate = c.CashFlowDate,
                                        CashFlowTypeKey = c.CashFlowTypeKey,
                                        CashFlowTypeName = c.CashFlowTypeKey == DbConstants.CashFlowType.In ? EduSuiteUIResources.Receipt : EduSuiteUIResources.Payment,
                                        AccountHeadName = ah.AccountHeadName,
                                        AccountHeadKey = c.AccountHeadKey,
                                        VoucherNumber = c.VoucherNumber,
                                        Amount = c.Amount,
                                        PaymentModeKey = c.PaymentModeKey,
                                        PaymentModeName = c.PaymentMode.PaymentModeName,
                                        PaymentModeSubKey = c.PaymentModeSubKey,
                                        PaymentModeSubName = c.PaymentModeSub.PaymentModeSubName,
                                        CardNumber = c.CardNumber,
                                        ReferenceNumber = c.ReferenceNumber,
                                        BankAccountName = c.BankAccount.Bank.BankName,
                                        ChequeOrDDNumber = c.ChequeOrDDNumber,
                                        ChequeOrDDDate = c.ChequeOrDDDate,
                                        TransactionTypeKey = c.TransactionTypeKey,
                                        //TransactionTypeName = c.TransactionType.TransactionTypeName,
                                        TransactionKey = c.TransactionKey,
                                        Purpose = c.Purpose,
                                        PaidBy = c.PaidBy,
                                        AuthorizedBy = c.AuthorizedBy,
                                        ReceivedBy = c.ReceivedBy,
                                        OnBehalfOf = c.OnBehalfOf,
                                        TotalBalance = c.BalanceAmount,
                                        Remarks = c.Remarks,
                                        BranchKey = c.BranchKey,
                                        BranchName = c.Branch.BranchName,
                                        ReceiptNumber = c.ReceiptNumber,
                                        ChequeStatusKey = c.ChequeStatusKey,
                                        ChequeAction = c.ChequeStatusKey == null ? "" : (c.ChequeStatusKey == DbConstants.ProcessStatus.Approved ? EduSuiteUIResources.Approved : EduSuiteUIResources.Rejected)

                                    });
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
                if (Employee != null)
                {
                    if (Employee.BranchAccess != null)
                    {
                        var Branches = Employee.BranchAccess.Split(',').Select(Int16.Parse).ToList();
                        cashflowList = cashflowList.Where(row => Branches.Contains(row.BranchKey));
                    }
                }
                if (model.CashFlowTypeKey != 0)
                {
                    cashflowList = cashflowList.Where(x => x.CashFlowTypeKey == model.CashFlowTypeKey);
                }
                if (model.AccountHeadKey != 0)
                {
                    cashflowList = cashflowList.Where(x => x.AccountHeadKey == model.AccountHeadKey);
                }
                if (model.SearchDate != null)
                {
                    cashflowList = cashflowList.Where(x => System.Data.Entity.DbFunctions.TruncateTime(x.CashFlowDate) == System.Data.Entity.DbFunctions.TruncateTime(model.SearchDate));
                }
                if (model.SortBy != "")
                {
                    cashflowList = SortCashFlow(cashflowList, model.SortBy, model.SortOrder);
                }

                model.TotalRecords = cashflowList.Count();

                modelCashflow = cashflowList.Skip(Skip).Take(Take).ToList<CashFlowViewModel>();
                return modelCashflow;
            }
            catch (Exception ex)
            {
                model.Message = ex.GetBaseException().Message;
                return new List<CashFlowViewModel>();
            }
        }
        private IQueryable<CashFlowViewModel> SortCashFlow(IQueryable<CashFlowViewModel> Query, string SortName, string SortOrder)
        {

            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(CashFlowViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<CashFlowViewModel>(resultExpression);

        }
        public CashFlowViewModel GetCashFlowByPartyId(CashFlowViewModel model)
        {
            try
            {
                CashFlowViewModel cashFlowViewModel = (from row in dbContext.CashFlows
                                                       join ah in dbContext.AccountHeads on row.AccountHeadKey equals ah.RowKey
                                                       select new CashFlowViewModel
                                                       {
                                                           CashFlowKey = row.RowKey,
                                                           CashFlowDate = row.CashFlowDate,
                                                           CashFlowTypeKey = row.CashFlowTypeKey,
                                                           AccountGroupKey = ah.AccountHeadType.AccountGroupKey,
                                                           AccountHeadTypeKey = ah.AccountHeadTypeKey,
                                                           AccountHeadCode = ah.AccountHeadCode,
                                                           AccountHeadKey = row.AccountHeadKey,
                                                           VoucherNumber = row.VoucherNumber,
                                                           Amount = row.Amount,
                                                           PaymentModeKey = row.PaymentModeKey,
                                                           PaymentModeName = row.PaymentMode.PaymentModeName,
                                                           PaymentModeSubKey = row.PaymentModeSubKey,
                                                           PaymentModeSubName = row.PaymentModeSub.PaymentModeSubName,
                                                           CardNumber = row.CardNumber,
                                                           BankAccountKey = row.BankAccountKey,
                                                           ChequeOrDDNumber = row.ChequeOrDDNumber,
                                                           ChequeOrDDDate = row.ChequeOrDDDate,
                                                           TransactionTypeKey = row.TransactionTypeKey,
                                                           TransactionKey = row.TransactionKey,
                                                           Purpose = row.Purpose,
                                                           PaidBy = row.PaidBy,
                                                           AuthorizedBy = row.AuthorizedBy,
                                                           ReceivedBy = row.ReceivedBy,
                                                           OnBehalfOf = row.OnBehalfOf,
                                                           Remarks = row.Remarks,
                                                           //IsOrderPayment = row.IsOrderPayment,
                                                           BankAccountBalance = row.BankAccount.CurrentAccountBalance,
                                                           BranchKey = row.BranchKey,
                                                           ReferenceNumber = row.ReferenceNumber
                                                       }).Where(x => x.CashFlowKey == model.CashFlowKey).FirstOrDefault();
                if (cashFlowViewModel == null)
                {
                    //cashFlowViewModel = new CashFlowViewModel();
                    //var PartyList = dbContext.Parties.SingleOrDefault(x => x.RowKey == model.AccountHeadKey);
                    //cashFlowViewModel.PaymentModeKey = DbConstants.PaymentMode.Cash;
                    //cashFlowViewModel.IsOrderPayment = true;
                    //cashFlowViewModel.CashFlowTypeKey = (PartyList.PartyTypeKey == DbConstants.PartyType.Supplier || PartyList.PartyTypeKey == DbConstants.PartyType.OutSource) ? DbConstants.CashFlowType.Out : DbConstants.CashFlowType.In;
                    //cashFlowViewModel.BranchKey = model.BranchKey;
                    //cashFlowViewModel.PartyKey = model.AccountHeadKey;
                    //cashFlowViewModel.AccountHeadCode = PartyList.AccountHeadCode;
                }


                FillDropdownLists(cashFlowViewModel);
                //if (cashFlowViewModel.IsOrderPayment == true)
                //{
                //    cashFlowViewModel.AccountHeadKey = dbContext.VwPartySelectActiveOnlies.Where(x => x.AccountHeadCode == cashFlowViewModel.AccountHeadCode).Select(x => x.RowKey).FirstOrDefault();
                //}
                //cashFlowViewModel.ShowAdminBankBalance = dbContext.GeneralConfigurations.Select(x => x.ShowAdminBankBalance ?? false).FirstOrDefault();


                return cashFlowViewModel;
            }
            catch (Exception ex)
            {
                model = new CashFlowViewModel();
                model.Message = ex.GetBaseException().Message;
                return model;
            }
        }
        public CashFlowViewModel GetPendingAccountByPartyId(CashFlowViewModel model)
        {
            //try
            //{
            //    CashFlowViewModel cashFlowViewModel = new CashFlowViewModel();
            //    cashFlowViewModel = (from row in dbContext.CashFlows
            //                         join ah in dbContext.AccountHeads on row.AccountHeadKey equals ah.RowKey
            //                         join p in dbContext.Parties on ah.AccountHeadCode equals p.AccountHeadCode
            //                         select new CashFlowViewModel
            //                         {
            //                             CashFlowKey = row.RowKey,
            //                             CashFlowDate = row.CashFlowDate,
            //                             CashFlowTypeKey = row.CashFlowTypeKey,
            //                             AccountGroupKey = ah.AccountHeadType.AccountGroupKey,
            //                             AccountHeadTypeKey = ah.AccountHeadTypeKey,
            //                             AccountHeadCode = ah.AccountHeadCode,
            //                             AccountHeadKey = row.AccountHeadKey,
            //                             VoucherNumber = row.VoucherNumber,
            //                             Amount = row.Amount,
            //                             PaymentModeKey = row.PaymentModeKey,
            //                             CardNumber = row.CardNumber,
            //                             BankAccountKey = row.BankAccountKey,
            //                             ChequeOrDDNumber = row.ChequeOrDDNumber,
            //                             ChequeOrDDDate = row.ChequeOrDDDate,
            //                             TransactionTypeKey = row.TransactionTypeKey,
            //                             TransactionKey = row.TransactionKey,
            //                             Purpose = row.Purpose,
            //                             PaidBy = row.PaidBy,
            //                             AuthorizedBy = row.AuthorizedBy,
            //                             ReceivedBy = row.ReceivedBy,
            //                             OnBehalfOf = row.OnBehalfOf,
            //                             Remarks = row.Remarks,
            //                             IsOrderPayment = row.IsOrderPayment,
            //                             BankAccountBalance = row.BankAccount.CurrentAccountBalance,
            //                             BranchKey = row.BranchKey,
            //                             ReferenceNumber = row.ReferenceNumber,
            //                             PartyKey = p.RowKey,
            //                             PartyName = p.PartyName,
            //                             PartyTypeKey = p.PartyTypeKey,
            //                             PartyTypeName = p.PartyType.PartyTypeName,
            //                         }).Where(x => x.CashFlowKey == model.CashFlowKey).FirstOrDefault();
            //    if (cashFlowViewModel == null)
            //    {
            //        cashFlowViewModel = new CashFlowViewModel();
            //        var PartyList = dbContext.Parties.SingleOrDefault(x => x.RowKey == model.AccountHeadKey);
            //        cashFlowViewModel.PaymentModeKey = DbConstants.PaymentMode.Cash;
            //        cashFlowViewModel.IsOrderPayment = true;
            //        cashFlowViewModel.CashFlowTypeKey = (PartyList.PartyTypeKey == DbConstants.PartyType.Customer || PartyList.PartyTypeKey == DbConstants.PartyType.Agent) ? DbConstants.CashFlowType.In : DbConstants.CashFlowType.Out;
            //        cashFlowViewModel.BranchKey = PartyList.CompanyBranchKey ?? 0;
            //        cashFlowViewModel.PartyKey = model.AccountHeadKey;
            //        cashFlowViewModel.AccountHeadKey = model.AccountHeadKey;
            //        cashFlowViewModel.AccountHeadCode = PartyList.AccountHeadCode;
            //        cashFlowViewModel.PartyName = PartyList.PartyName;
            //        cashFlowViewModel.PartyTypeName = PartyList.PartyType.PartyTypeName;
            //        cashFlowViewModel.PartyTypeKey = PartyList.PartyTypeKey;
            //    }

            //    GetPendingAccountById(cashFlowViewModel);
            //    cashFlowViewModel.Excess = (cashFlowViewModel.Amount ?? 0) - (cashFlowViewModel.PendingAccount.Select(x => x.PaidUpdate ?? 0).DefaultIfEmpty().Sum());
            //    FillPaymentModes(cashFlowViewModel);
            //    FillBankAccounts(cashFlowViewModel);
            //    FillNotificationDetail(cashFlowViewModel);
            //    if (cashFlowViewModel.IsOrderPayment == true)
            //    {
            //        cashFlowViewModel.AccountHeadKey = dbContext.VwPartySelectActiveOnlies.Where(x => x.AccountHeadCode == cashFlowViewModel.AccountHeadCode).Select(x => x.RowKey).FirstOrDefault();
            //    }
            //    cashFlowViewModel.ShowAdminBankBalance = dbContext.GeneralConfigurations.Select(x => x.ShowAdminBankBalance ?? false).FirstOrDefault();
            //    return cashFlowViewModel;
            //}
            //catch (Exception ex)
            //{
            //    model = new CashFlowViewModel();
            //    model.Message = ex.GetBaseException().Message;
            //    return model;
            //}

            return model;
        }
        public CashFlowViewModel GetCashFlowById(CashFlowViewModel model)
        {
            try
            {
                CashFlowViewModel cashFlowViewModel = (from row in dbContext.CashFlows
                                                       join ah in dbContext.AccountHeads on row.AccountHeadKey equals ah.RowKey
                                                       select new CashFlowViewModel
                                                       {
                                                           CashFlowKey = row.RowKey,
                                                           CashFlowDate = row.CashFlowDate,
                                                           CashFlowTypeKey = row.CashFlowTypeKey,
                                                           AccountGroupKey = ah.AccountHeadType.AccountGroupKey,
                                                           AccountHeadTypeKey = ah.AccountHeadTypeKey,
                                                           AccountHeadCode = ah.AccountHeadCode,
                                                           AccountHeadKey = row.AccountHeadKey,
                                                           VoucherNumber = row.VoucherNumber,
                                                           Amount = row.Amount,
                                                           PaymentModeKey = row.PaymentModeKey,
                                                           PaymentModeName = row.PaymentMode.PaymentModeName,
                                                           PaymentModeSubKey = row.PaymentModeSubKey,
                                                           PaymentModeSubName = row.PaymentModeSub.PaymentModeSubName,
                                                           CardNumber = row.CardNumber,
                                                           BankAccountKey = row.BankAccountKey,
                                                           ChequeOrDDNumber = row.ChequeOrDDNumber,
                                                           ChequeOrDDDate = row.ChequeOrDDDate,
                                                           TransactionTypeKey = row.TransactionTypeKey,
                                                           TransactionKey = row.TransactionKey,
                                                           Purpose = row.Purpose,
                                                           PaidBy = row.PaidBy,
                                                           AuthorizedBy = row.AuthorizedBy,
                                                           ReceivedBy = row.ReceivedBy,
                                                           OnBehalfOf = row.OnBehalfOf,
                                                           Remarks = row.Remarks,
                                                           //IsOrderPayment = row.IsOrderPayment,
                                                           BankAccountBalance = row.BankAccount.CurrentAccountBalance,
                                                           BranchKey = row.BranchKey,
                                                           ReferenceNumber = row.ReferenceNumber,
                                                           IsContra = row.IsContra ?? false,
                                                           ChequeStatusKey = row.ChequeStatusKey


                                                       }).Where(x => x.CashFlowKey == model.CashFlowKey).FirstOrDefault();
                if (cashFlowViewModel == null)
                {
                    cashFlowViewModel = new CashFlowViewModel();
                    cashFlowViewModel.PaymentModeKey = DbConstants.PaymentMode.Cash;
                    cashFlowViewModel.CashFlowTypeKey = model.CashFlowTypeKey;
                    cashFlowViewModel.BranchKey = model.BranchKey;
                    cashFlowViewModel.IsOrderPayment = false;
                    cashFlowViewModel.TransactionTypeKey = DbConstants.TransactionType.CashFlow;
                    cashFlowViewModel.BankAccountBalance = 0;
                }
                if (model.IsPayment == true)
                {
                    cashFlowViewModel.CashFlowTypeKey = DbConstants.CashFlowType.Out;
                    cashFlowViewModel.AccountGroupKey = DbConstants.AccountGroup.Expenses;
                }
                else
                {
                    cashFlowViewModel.CashFlowTypeKey = DbConstants.CashFlowType.In;
                    cashFlowViewModel.AccountGroupKey = DbConstants.AccountGroup.Income;
                }
                FillDropdownLists(cashFlowViewModel);
                return cashFlowViewModel;
            }
            catch (Exception ex)
            {
                model = new CashFlowViewModel();
                model.Message = ex.GetBaseException().Message;
                return model;
            }


        }
        public CashFlowViewModel CreateCashFlow(CashFlowViewModel model)
        {
            //FillDropdownLists(model);
            CashFlow cashFlowModel = new CashFlow();
            AccountManagement accountManagement = new AccountManagement(dbContext);

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var accountHeadKey = model.AccountHeadKey;

                    ConfigurationViewModel ConfigModel = new ConfigurationViewModel();
                    ConfigModel.BranchKey = model.BranchKey;
                    ConfigModel.ConfigType = model.CashFlowTypeKey == DbConstants.CashFlowType.In ? DbConstants.PaymentReceiptConfigType.ReceiptVoucher : DbConstants.PaymentReceiptConfigType.Payment;
                    Configurations.GenerateReceipt(dbContext, ConfigModel);
                    accountManagement.CreateCashFlowAccount(model, ConfigModel);
                    model.ReceiptNumber = ConfigModel.ReceiptNumber;
                    List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
                    purposeGeneration(model);
                    CreateAccountFlow(model, false, accountFlowModelList);
                    model.AccountHeadKey = accountHeadKey;
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
                    }
                    throw raise;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.CashFlow);
                    // model.Message = cashFlowModel.CashFlowTypeKey == 1 ? EduSuiteUIResources.FailedToSaveReceipt : EduSuiteUIResources.FailedToSavePayment;
                    model.IsSuccessful = false;
                }
            }
            return model;
        }
        private List<AccountFlowViewModel> CreateSalesOrderReciept(CashFlowViewModel model, List<AccountFlowViewModel> accountFlowModelList)
        {

            return accountFlowModelList;
        }
        public CashFlowViewModel UpdateCashFlow(CashFlowViewModel model)
        {
            //FillDropdownLists(model);
            CashFlow cashFlowModel = new CashFlow();
            AccountManagement accountManagement = new AccountManagement(dbContext);
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var accountHeadKey = model.AccountHeadKey;

                    model.TransactionTypeKey = DbConstants.TransactionType.CashFlow;
                    model.TransactionKey = model.CashFlowKey;
                    //accountManagement.UpdateCashFlowAccount(model);
                    List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
                    // if (model.IsOrderPayment == true)


                    accountManagement.UpdateCashFlowAccount(model);
                    purposeGeneration(model);
                    CreateAccountFlow(model, true, accountFlowModelList);

                    model.AccountHeadKey = accountHeadKey;
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
                    }
                    throw raise;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.CashFlow);
                    model.IsSuccessful = false;
                }
            }

            return model;
        }
        public CashFlowViewModel DeleteCashFlow(CashFlowViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    CashFlow CashFlow = dbContext.CashFlows.SingleOrDefault(row => row.RowKey == model.RowKey);
                    if (CashFlow != null)
                    {
                        if (CashFlow.CashFlowTypeKey == DbConstants.CashFlowType.In)
                        {
                            decimal balance = 0;
                            decimal Availablebalance = CheckShortBalance(CashFlow.PaymentModeKey, CashFlow.RowKey, CashFlow.BankAccountKey ?? 0, CashFlow.BranchKey, CashFlow.CashFlowTypeKey);

                            //  balance = Availablebalance - CashFlow.Amount;
                            if (Availablebalance < 0)
                            {
                                model.IsSuccessful = false;
                                model.Message = EduSuiteUIResources.InSufficentBalnceMessage;
                                return model;
                            }
                        }
                    }

                    ConfigurationViewModel ConfigModel = new ConfigurationViewModel();
                    ConfigModel.BranchKey = CashFlow.BranchKey;
                    ConfigModel.SerialNumber = CashFlow.SerialNumber ?? 0;
                    ConfigModel.IsDelete = true;
                    ConfigModel.ConfigType = CashFlow.CashFlowTypeKey == DbConstants.CashFlowType.In ? DbConstants.PaymentReceiptConfigType.ReceiptVoucher : DbConstants.PaymentReceiptConfigType.Payment;
                    Configurations.GenerateReceipt(dbContext, ConfigModel);
                    AccountFlowViewModel accountFlowModel = new AccountFlowViewModel();
                    accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.CashFlow;
                    accountFlowModel.TransactionKey = CashFlow.RowKey;


                    dbContext.CashFlows.Remove(CashFlow);


                    AccountFlowService accountFlowService = new AccountFlowService(dbContext);
                    accountFlowService.DeleteAccountFlow(accountFlowModel);

                    bool IsChequeExist = dbContext.ChequeClearances.Where(x => x.TransactionKey == model.RowKey && x.TransactionTypeKey == DbConstants.TransactionType.CashFlow).Any();
                    if (IsChequeExist)
                    {
                        ChequeClearance dbChequeClearance = dbContext.ChequeClearances.Where(x => x.TransactionKey == model.RowKey && x.TransactionTypeKey == DbConstants.TransactionType.CashFlow).SingleOrDefault();
                        dbContext.ChequeClearances.Remove(dbChequeClearance);
                        accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.ChequeClearance;
                        accountFlowModel.TransactionKey = dbChequeClearance.TransactionKey;
                        accountFlowService.DeleteAccountFlow(accountFlowModel);
                    }


                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = ex.GetBaseException().Message;
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.DailyTransaction);
                        model.IsSuccessful = false;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = ex.GetBaseException().Message;
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.DailyTransaction);
                    model.IsSuccessful = false;
                }
            }

            return model;
        }

        private void purposeGeneration(CashFlowViewModel model)
        {
            model.Purpose = (model.Purpose != null && model.Purpose != "" ? model.Purpose : model.AccountHeadName + EduSuiteUIResources.BlankSpace + (model.CashFlowTypeKey == DbConstants.CashFlowType.In ? EduSuiteUIResources.Receipt : EduSuiteUIResources.Payment) + EduSuiteUIResources.BlankSpace);
            string PaidBy = model.IsOrderPayment == true && model.CashFlowTypeKey == DbConstants.CashFlowType.In ? (EduSuiteUIResources.RecievedFrom + EduSuiteUIResources.BlankSpace + (model.PaidBy != null && model.PaidBy != "" ? model.PaidBy : model.AccountHeadName) + EduSuiteUIResources.BlankSpace) : (model.PaidBy != null && model.PaidBy != "" ? EduSuiteUIResources.Paid + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.By + EduSuiteUIResources.BlankSpace + model.PaidBy + EduSuiteUIResources.BlankSpace : "");
            string ReceivedBy = model.IsOrderPayment == true && model.CashFlowTypeKey == DbConstants.CashFlowType.Out ? (EduSuiteUIResources.PaidTo + EduSuiteUIResources.BlankSpace + (model.ReceivedBy != null && model.ReceivedBy != "" ? model.ReceivedBy : model.AccountHeadName) + EduSuiteUIResources.BlankSpace) : (model.ReceivedBy != null && model.ReceivedBy != "" ? EduSuiteUIResources.Recieved + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.By + EduSuiteUIResources.BlankSpace + model.ReceivedBy + EduSuiteUIResources.BlankSpace : "");
            string OnBehalfOf = model.OnBehalfOf != null && model.OnBehalfOf != "" ? EduSuiteUIResources.OnBehalfOf + EduSuiteUIResources.BlankSpace + model.OnBehalfOf + EduSuiteUIResources.BlankSpace : "";
            string AuthorizedBy = model.AuthorizedBy != null && model.AuthorizedBy != "" ? EduSuiteUIResources.AuthorizedBy + EduSuiteUIResources.BlankSpace + model.AuthorizedBy + EduSuiteUIResources.BlankSpace : "";
            model.Remarks = model.Remarks != null && model.Remarks != "" ? EduSuiteUIResources.OpenBracket + model.Remarks + EduSuiteUIResources.CloseBracket : "";
            model.PaymentModeName = model.IsContra ? "" : (EduSuiteUIResources.BlankSpace + EduSuiteUIResources.By + EduSuiteUIResources.BlankSpace + (model.PaymentModeKey == DbConstants.PaymentMode.Cash ? EduSuiteUIResources.Cash : model.PaymentModeKey == DbConstants.PaymentMode.Bank ? EduSuiteUIResources.Bank + EduSuiteUIResources.OpenBracket + model.BankAccountName + EduSuiteUIResources.CloseBracket : model.PaymentModeKey == DbConstants.PaymentMode.Cheque ? EduSuiteUIResources.Cheque + EduSuiteUIResources.OpenBracket + model.BankAccountName + EduSuiteUIResources.CloseBracket : ""));
            model.Purpose = model.Purpose + (model.CashFlowTypeKey == DbConstants.CashFlowType.In ? PaidBy + ReceivedBy : ReceivedBy + PaidBy) + OnBehalfOf + AuthorizedBy + model.PaymentModeName;

        }
        public CashFlowViewModel GetBranches(CashFlowViewModel model)
        {

            IQueryable<SelectListModel> BranchQuery = dbContext.vwBranchSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BranchName
            });

            Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
            if (Employee != null)
            {
                if (Employee.BranchAccess != null)
                {
                    List<long> Branches = Employee.BranchAccess.Split(',').Select(Int64.Parse).ToList();
                    model.Branches = BranchQuery.Where(row => Branches.Contains(row.RowKey)).ToList();
                    //model.BranchKey = Employee.BranchKey;
                }
                else
                {
                    model.Branches = BranchQuery.Where(x => x.RowKey == Employee.BranchKey).ToList();
                    //model.BranchKey = Employee.BranchKey;
                }
            }
            else
            {
                model.Branches = BranchQuery.ToList();
            }

            if (model.Branches.Count == 1)
            {
                long? branchkey = model.Branches.Select(x => x.RowKey).FirstOrDefault();
                model.BranchKey = Convert.ToInt16(branchkey);
            }
            return model;
        }

        private void FillDropdownLists(CashFlowViewModel model)
        {
            FillPaymentModes(model);
            FillPaymentModeSub(model);
            FillBankAccounts(model);
            FillAcountHead(model);
            GetBranches(model);
        }

        public CashFlowViewModel FillDropdownListsForList(CashFlowViewModel model)
        {
            FillAccountGroup(model);
            FillAcountHeadType(model);
            FillAcountHead(model);
            GetBranches(model);
            FillCashFlowType(model);
            return model;
        }

        private void FillPaymentModes(CashFlowViewModel model)
        {
            model.PaymentModes = dbContext.PaymentModes.Where(row => row.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.PaymentModeName
            }).ToList();
        }

        public void FillPaymentModeSub(CashFlowViewModel model)
        {
            model.PaymentModeSub = dbContext.PaymentModeSubs.Where(x => x.IsActive && x.PaymentModeKey == DbConstants.PaymentMode.Bank).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.PaymentModeSubName
            }).ToList();

        }
        private void FillAccountGroup(CashFlowViewModel model)
        {

            model.AccountGroups = dbContext.AccountGroups.Where(x => x.RowKey == (model.IsPayment == true ? DbConstants.AccountGroup.Expenses : DbConstants.AccountGroup.Income)).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AccountGroupName
            }).ToList();
        }

        private void FillCashFlowType(CashFlowViewModel model)
        {
            model.CashFlowTypes = dbContext.CashFlowTypes.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.RowKey == DbConstants.CashFlowType.In ? EduSuiteUIResources.Receipt : EduSuiteUIResources.Payment
            }).ToList();
        }

        public CashFlowViewModel FillAcountHeadType(CashFlowViewModel model)
        {
            model.AccountHeadTypes = dbContext.VwAccountHeadTypes.Where(row => row.AccountGroupKey == model.AccountGroupKey).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AccountHeadTypeName
            }).ToList();
            return model;
        }

        public CashFlowViewModel FillAcountHead(CashFlowViewModel model)
        {
            List<long> AccountHeadKeys = new List<long>();
            string accoundheadkeys = string.Join(",", dbContext.FeeTypes.Select(x => x.AccountHeadKey).ToList());
            //accoundheadkeys = accoundheadkeys + "," + string.Join(",", dbContext.UniversityMasters.Select(x => x.AccountHeadKey).ToList());
            if (accoundheadkeys != "")
            {
                AccountHeadKeys = accoundheadkeys.Split(',').Select(Int64.Parse).ToList();
            }


            if (model.IsContra)
            {
                model.AccountHeads = (from ac in dbContext.AccountHeads
                                      join p in dbContext.BankAccounts.Where(x => x.IsActive) on ac.RowKey equals p.AccountHeadKey into pj
                                      from p in pj.DefaultIfEmpty()
                                      where !AccountHeadKeys.Contains(ac.RowKey) && ac.RowKey == DbConstants.AccountHead.CashAccount || p.RowKey != null
                                      select new SelectListModel
                                      {
                                          RowKey = ac.RowKey,
                                          Text = ac.AccountHeadName
                                      }).ToList();
            }
            else
            {
                model.AccountHeads = (from ac in dbContext.AccountHeads.Where(x => (x.AccountHeadType.AccountGroupKey == model.AccountGroupKey && x.RowKey != DbConstants.AccountHead.CashAccount)
                                     || (x.PaymentConfigTypeKey == (model.CashFlowTypeKey == DbConstants.CashFlowType.In ? DbConstants.PaymentReceiptConfigType.ReceiptVoucher : DbConstants.PaymentReceiptConfigType.Payment)
                                      || x.PaymentConfigTypeKey == DbConstants.PaymentReceiptConfigType.PaymentAndReceipt))
                                          //join p in dbContext.VwPartySelectActiveOnlies on ac.AccountHeadCode equals p.AccountHeadCode into pj
                                          //from p in pj.DefaultIfEmpty()
                                          //join rm in dbContext.RawMaterials on ac.AccountHeadCode equals rm.AccountHeadCode into rmj
                                          //from rm in rmj.DefaultIfEmpty()
                                      where (ac.HideDaily ?? false) == false && ac.IsActive == true && !AccountHeadKeys.Contains(ac.RowKey)
                                      select new SelectListModel
                                      {
                                          RowKey = ac.RowKey,
                                          Text = ac.AccountHeadName
                                      }).ToList();
            }
            return model;
        }

        public CashFlowViewModel FillBankAccounts(CashFlowViewModel model)
        {
            long accountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == model.AccountHeadKey).Select(x => x.RowKey).FirstOrDefault();

            model.BankAccounts = dbContext.BranchAccounts.Where(x => x.BranchKey == model.BranchKey && x.BankAccount.IsActive && x.BankAccount.AccountHeadKey != accountHeadKey).Select(row => new SelectListModel
            {
                RowKey = row.BankAccount.RowKey,
                Text = (row.BankAccount.NameInAccount ?? row.BankAccount.AccountNumber) + EduSuiteUIResources.Hyphen + row.BankAccount.Bank.BankName
            }).ToList();


            return model;
        }

        public CashFlowViewModel PrintCashFlowById(CashFlowViewModel model)
        {
            var CashFlowList = dbContext.CashFlows.SingleOrDefault(x => x.RowKey == model.CashFlowKey);
            model.AccountHeadKey = CashFlowList.AccountHeadKey;
            model.CashFlowDate = CashFlowList.CashFlowDate;
            CashFlowViewModel cashFlowViewModel = new CashFlowViewModel();
            string AccountHeadCode = "";
            if (cashFlowViewModel != null)
            {
                AccountHeadCode = dbContext.AccountHeads.Where(x => x.RowKey == CashFlowList.AccountHeadKey).Select(x => x.AccountHeadCode).FirstOrDefault();
            }
            try
            {

                cashFlowViewModel = (from row in dbContext.CashFlows
                                     join ah in dbContext.AccountHeads on row.AccountHeadKey equals ah.RowKey
                                     select new CashFlowViewModel
                                     {
                                         CashFlowKey = row.RowKey,
                                         CashFlowDate = row.CashFlowDate,
                                         AccountHeadName = row.CashFlowTypeKey == DbConstants.CashFlowType.In ? row.PaidBy : row.ReceivedBy,
                                         VoucherNumber = row.VoucherNumber,
                                         Amount = row.Amount,
                                         CashFlowTypeKey = row.CashFlowTypeKey,
                                         Purpose = row.Purpose ?? ah.AccountHeadName,
                                         PaymentModeName = row.PaymentMode.PaymentModeName,
                                         CardNumber = row.CardNumber,
                                         BankAccountKey = row.BankAccountKey,
                                         ChequeOrDDNumber = row.ChequeOrDDNumber,
                                         ChequeOrDDDate = row.ChequeOrDDDate,
                                         TransactionTypeKey = row.TransactionTypeKey,
                                         TransactionKey = row.TransactionKey,
                                         AmountPaid = row.Amount,
                                         AmountBalace = 0,
                                         // ConversionSize = dbContext.PrintSizeConvertionCharts.Where(x => x.FromSizeKey == DbConstants.PrintPaperSizes.A4 && x.ToSizeKey == (dbContext.PrintConfigurations.Where(y => y.RowKey == DbConstants.PrintConfiguration.ReceiptPrint).Select(y => y.PaperSizeKey).FirstOrDefault())).Select(x => x.Size).FirstOrDefault(),
                                         // PaperSize = dbContext.PrintConfigurations.Where(y => y.RowKey == DbConstants.PrintConfiguration.ReceiptPrint).Select(y => y.PrintPaperSize.PaperSize).FirstOrDefault(),
                                         CompanyName = row.Branch.BranchName,
                                         //CompanyImageUrl = row.CompanyBranch.CompanyLogoes.Select(x => x.FileName).FirstOrDefault(),
                                         //CompanyEmailId = row.CompanyBranch.EmailId,
                                         //CompanyAddress = row.CompanyBranch.AddressLine1,
                                         //CompanyWebsite = row.CompanyBranch.Company.Website,
                                         ReceiptNumber = row.ReceiptNumber
                                     }).Where(x => x.CashFlowKey == model.CashFlowKey).FirstOrDefault();

                if (cashFlowViewModel == null)
                {
                    cashFlowViewModel = new CashFlowViewModel();
                    cashFlowViewModel.PaymentModeKey = DbConstants.PaymentMode.Cash;
                    cashFlowViewModel.CashFlowTypeKey = model.CashFlowTypeKey;
                    cashFlowViewModel.BranchKey = model.BranchKey;
                }

                return cashFlowViewModel;
            }
            catch (Exception ex)
            {
                model = new CashFlowViewModel();
                model.Message = ex.GetBaseException().Message;
                return model;

            }
        }
        #endregion

        #region Account
        private void CreateAccountFlow(CashFlowViewModel model, bool IsUpdate, List<AccountFlowViewModel> accountFlowModelList)
        {
            AccountFlowService accounFlowService = new AccountFlowService(dbContext);
            if (model.Purpose == null || model.Purpose == "")
            {
                model.Purpose = dbContext.AccountHeads.Where(x => x.RowKey == model.AccountHeadKey).Select(x => x.AccountHeadName).FirstOrDefault();
            }
            accountFlowModelList = CreditAmountList(model, accountFlowModelList, IsUpdate);
            accountFlowModelList = DebitAmountList(model, accountFlowModelList, IsUpdate);
            if (IsUpdate == false)
            {
                accounFlowService.CreateAccountFlow(accountFlowModelList);
            }
            else
            {
                accounFlowService.UpdateAccountFlow(accountFlowModelList);
            }
        }
        private List<AccountFlowViewModel> CreditAmountList(CashFlowViewModel model, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate)
        {                       
            long ExtraUpdateKey = 0;            
            if (model.OldAccountHeadKey != null && model.OldAccountHeadKey != 0 && model.OldAccountHeadKey != model.AccountHeadKey)
            {
                IsUpdate = false;
                ExtraUpdateKey = model.OldAccountHeadKey;
            }
            if (model.CashFlowTypeKey == DbConstants.CashFlowType.In)
            {
                accountFlowModelList.Add(new AccountFlowViewModel
                {
                    OldAccountHeadKey = model.OldAccountHeadKey,
                    CashFlowTypeKey = DbConstants.CashFlowType.Out,
                    AccountHeadKey = model.AccountHeadKey,
                    Amount = model.Amount ?? 0,
                    TransactionTypeKey = DbConstants.TransactionType.CashFlow,
                    VoucherTypeKey = DbConstants.VoucherType.Reciept,
                    TransactionKey = model.CashFlowKey,
                    TransactionDate = model.PaymentModeKey != DbConstants.PaymentMode.Cheque ? model.CashFlowDate : (model.ChequeOrDDDate ?? model.CashFlowDate),
                    ExtraUpdateKey = ExtraUpdateKey,
                    IsUpdate = IsUpdate,
                    BranchKey = model.BranchKey,
                    Purpose = model.Purpose + model.Remarks,
                });
            }
            else
            {
                accountFlowModelList.Add(new AccountFlowViewModel
                {
                    OldAccountHeadKey = model.OldAccountHeadKey,
                    CashFlowTypeKey = DbConstants.CashFlowType.In,
                    AccountHeadKey = model.AccountHeadKey,
                    Amount = model.Amount ?? 0,
                    TransactionTypeKey = DbConstants.TransactionType.CashFlow,
                    VoucherTypeKey = DbConstants.VoucherType.Payment,
                    TransactionKey = model.CashFlowKey,
                    TransactionDate = model.PaymentModeKey != DbConstants.PaymentMode.Cheque ? model.CashFlowDate : (model.ChequeOrDDDate ?? model.CashFlowDate),
                    ExtraUpdateKey = ExtraUpdateKey,
                    IsUpdate = IsUpdate,
                    BranchKey = model.BranchKey,
                    Purpose = model.Purpose + model.Remarks,
                });
            }
            return accountFlowModelList;
        }
        private List<AccountFlowViewModel> AdvanceAmountList(CashFlowViewModel model, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, decimal Amount, string OrderNumber)
        {            
            long ExtraUpdateKey = 0;
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                AccountHeadKey = DbConstants.AccountHead.AdvancePayable,
                Amount = Amount,
                TransactionTypeKey = DbConstants.TransactionType.CashFlowAdvance,
                VoucherTypeKey = DbConstants.VoucherType.Reciept,
                TransactionKey = model.CashFlowKey,
                TransactionDate = model.PaymentModeKey != DbConstants.PaymentMode.Cheque ? model.CashFlowDate : (model.ChequeOrDDDate ?? model.CashFlowDate),
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = model.BranchKey,
                Purpose = (model.IsOrderPayment == true ? OrderNumber + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Receipt + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.RecievedFrom + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Receipt + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Receipt + EduSuiteUIResources.BlankSpace + model.AccountHeadName : model.AccountHeadName + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Receipt + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Recieved) + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.By + EduSuiteUIResources.BlankSpace + (model.PaymentModeKey == DbConstants.PaymentMode.Cash ? EduSuiteUIResources.Cash : model.PaymentModeKey == DbConstants.PaymentMode.Bank ? EduSuiteUIResources.Bank + EduSuiteUIResources.OpenBracket + model.BankAccountName + EduSuiteUIResources.CloseBracket : model.PaymentModeKey == DbConstants.PaymentMode.Cheque ? EduSuiteUIResources.Cheque + EduSuiteUIResources.OpenBracket + model.BankAccountName + EduSuiteUIResources.CloseBracket : ""),
            });
            return accountFlowModelList;
        }
        private List<AccountFlowViewModel> ExtraAdvanceAmountList(CashFlowViewModel model, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate)
        {
            long ExtraUpdateKey = 0;
            if (model.CashFlowTypeKey == DbConstants.CashFlowType.In)
            {
                accountFlowModelList.Add(new AccountFlowViewModel
                {
                    CashFlowTypeKey = DbConstants.CashFlowType.Out,
                    AccountHeadKey = DbConstants.AccountHead.AdvancePayable,
                    Amount = model.AdvanceAmount ?? 0,
                    TransactionTypeKey = DbConstants.TransactionType.CashFlowExcess,
                    VoucherTypeKey = DbConstants.VoucherType.Reciept,
                    TransactionKey = model.CashFlowKey,
                    TransactionDate = model.PaymentModeKey != DbConstants.PaymentMode.Cheque ? model.CashFlowDate : (model.ChequeOrDDDate ?? model.CashFlowDate),
                    ExtraUpdateKey = ExtraUpdateKey,
                    IsUpdate = IsUpdate,
                    BranchKey = model.BranchKey,
                    Purpose = EduSuiteUIResources.Receipt + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Receipt + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.RecievedFrom + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Receipt + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Application + EduSuiteUIResources.BlankSpace + model.AccountHeadName + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.By + EduSuiteUIResources.BlankSpace + (model.PaymentModeKey == DbConstants.PaymentMode.Cash ? EduSuiteUIResources.Cash : model.PaymentModeKey == DbConstants.PaymentMode.Bank ? EduSuiteUIResources.Bank + EduSuiteUIResources.OpenBracket + model.BankAccountName + EduSuiteUIResources.CloseBracket : model.PaymentModeKey == DbConstants.PaymentMode.Cheque ? EduSuiteUIResources.Cheque + EduSuiteUIResources.OpenBracket + model.BankAccountName + EduSuiteUIResources.CloseBracket : ""),
                });
            }
            else
            {
                accountFlowModelList.Add(new AccountFlowViewModel
                {
                    CashFlowTypeKey = DbConstants.CashFlowType.In,
                    AccountHeadKey = DbConstants.AccountHead.AdvanceReceivable,
                    Amount = model.AdvanceAmount ?? 0,
                    TransactionTypeKey = DbConstants.TransactionType.CashFlowExcess,
                    VoucherTypeKey = DbConstants.VoucherType.Reciept,
                    TransactionKey = model.CashFlowKey,
                    TransactionDate = model.PaymentModeKey != DbConstants.PaymentMode.Cheque ? model.CashFlowDate : (model.ChequeOrDDDate ?? model.CashFlowDate),
                    ExtraUpdateKey = ExtraUpdateKey,
                    IsUpdate = IsUpdate,
                    BranchKey = model.BranchKey,
                    Purpose = EduSuiteUIResources.Receipt + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Payment + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.PaidTo + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Receipt + EduSuiteUIResources.BlankSpace + model.AccountHeadName + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.By + EduSuiteUIResources.BlankSpace + (model.PaymentModeKey == DbConstants.PaymentMode.Cash ? EduSuiteUIResources.Cash : model.PaymentModeKey == DbConstants.PaymentMode.Bank ? EduSuiteUIResources.Bank + EduSuiteUIResources.OpenBracket + model.BankAccountName + EduSuiteUIResources.CloseBracket : model.PaymentModeKey == DbConstants.PaymentMode.Cheque ? EduSuiteUIResources.Cheque + EduSuiteUIResources.OpenBracket + model.BankAccountName + EduSuiteUIResources.CloseBracket : ""),
                });
            }
            return accountFlowModelList;
        }
        private List<AccountFlowViewModel> AccountRecievableAmountList(CashFlowViewModel model, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, decimal Amount, string OrderNumber)
        {            
            long ExtraUpdateKey = 0;
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                AccountHeadKey = DbConstants.AccountHead.AccountsReceivable,
                Amount = Amount,
                TransactionTypeKey = DbConstants.TransactionType.CashFlowRecievable,
                VoucherTypeKey = DbConstants.VoucherType.Reciept,
                TransactionKey = model.CashFlowKey,
                TransactionDate = model.PaymentModeKey != DbConstants.PaymentMode.Cheque ? model.CashFlowDate : (model.ChequeOrDDDate ?? model.CashFlowDate),
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = model.BranchKey,
                Purpose = (model.IsOrderPayment == true ? OrderNumber + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Receipt + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.RecievedFrom + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Receipt + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Receipt + EduSuiteUIResources.BlankSpace + model.AccountHeadName : model.AccountHeadName + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Receipt + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Recieved) + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.By + EduSuiteUIResources.BlankSpace + (model.PaymentModeKey == DbConstants.PaymentMode.Cash ? EduSuiteUIResources.Cash : model.PaymentModeKey == DbConstants.PaymentMode.Bank ? EduSuiteUIResources.Bank + EduSuiteUIResources.OpenBracket + model.BankAccountName + EduSuiteUIResources.CloseBracket : model.PaymentModeKey == DbConstants.PaymentMode.Cheque ? EduSuiteUIResources.Cheque + EduSuiteUIResources.OpenBracket + model.BankAccountName + EduSuiteUIResources.CloseBracket : ""),
            });
            return accountFlowModelList;
        }
        private List<AccountFlowViewModel> DebitAmountList(CashFlowViewModel model, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate)
        {
            long oldAccountHeadKey;
            long oldBankAccountHeadKey = 0;
            if (model.OldPaymentModeKey == DbConstants.PaymentMode.Bank || model.OldPaymentModeKey == DbConstants.PaymentMode.Cheque)
            {
                oldAccountHeadKey = dbContext.BankAccounts.Where(x => x.RowKey == model.OldBankKey).Select(x => x.AccountHeadKey).FirstOrDefault();
                oldBankAccountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == oldAccountHeadKey).Select(x => x.RowKey).FirstOrDefault();
            }
            else
            {
                oldAccountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.CashAccount).Select(x => x.RowKey).FirstOrDefault();

            }
            long accountHeadKey;
            long ExtraUpdateKey = 0;
            if (model.PaymentModeKey == DbConstants.PaymentMode.Bank || model.PaymentModeKey == DbConstants.PaymentMode.Cheque)
            {
                accountHeadKey = dbContext.BankAccounts.Where(x => x.RowKey == model.BankAccountKey).Select(x => x.AccountHeadKey).FirstOrDefault();
            }
            else
            {
                accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.CashAccount).Select(x => x.RowKey).FirstOrDefault();
            }
            if (model.OldPaymentModeKey != null && model.OldPaymentModeKey != 0 && model.OldPaymentModeKey != model.PaymentModeKey)
            {
                IsUpdate = false;
                ExtraUpdateKey = model.OldPaymentModeKey == DbConstants.PaymentMode.Cash ? DbConstants.AccountHead.CashAccount : oldBankAccountHeadKey;

            }
            if (model.CashFlowTypeKey == DbConstants.CashFlowType.In)
            {
                accountFlowModelList.Add(new AccountFlowViewModel
                {
                    CashFlowTypeKey = DbConstants.CashFlowType.In,
                    AccountHeadKey = accountHeadKey,
                    Amount = model.Amount ?? 0,
                    TransactionTypeKey = DbConstants.TransactionType.CashFlow,
                    VoucherTypeKey = model.IsOrderPayment == true ? DbConstants.VoucherType.Reciept : DbConstants.VoucherType.Reciept,
                    TransactionDate = model.PaymentModeKey != DbConstants.PaymentMode.Cheque ? model.CashFlowDate : (model.ChequeOrDDDate ?? model.CashFlowDate),
                    ExtraUpdateKey = ExtraUpdateKey,
                    IsUpdate = IsUpdate,
                    Purpose = model.Purpose + model.Remarks,
                    BranchKey = model.BranchKey,
                    TransactionKey = model.CashFlowKey,
                    OldAccountHeadKey = oldAccountHeadKey
                });
            }
            else
            {
                accountFlowModelList.Add(new AccountFlowViewModel
                {
                    CashFlowTypeKey = DbConstants.CashFlowType.Out,
                    AccountHeadKey = accountHeadKey,
                    Amount = model.Amount ?? 0,
                    TransactionTypeKey = DbConstants.TransactionType.CashFlow,
                    VoucherTypeKey = DbConstants.VoucherType.Payment,
                    TransactionDate = model.PaymentModeKey != DbConstants.PaymentMode.Cheque ? model.CashFlowDate : (model.ChequeOrDDDate ?? model.CashFlowDate),
                    ExtraUpdateKey = ExtraUpdateKey,
                    IsUpdate = IsUpdate,
                    Purpose = model.Purpose + model.Remarks,
                    BranchKey = model.BranchKey,
                    TransactionKey = model.CashFlowKey,
                    OldAccountHeadKey = oldAccountHeadKey
                });

            }
            return accountFlowModelList;
        }
        #endregion
        //This function is Not Using now as per Afnas on 25 Jan 2018
        public decimal GetAccountHeadBalance(decimal Amount, long Rowkey, long AccountHeadKey)
        {
            decimal Balance = 0;
            long accountHeadKey = AccountHeadKey;
            if (AccountHeadKey == DbConstants.AccountHead.CashAccount)
            {
                Balance = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.CashAccount).Select(x => (x.TotalDebitAmount ?? 0) - (x.TotalCreditAmount ?? 0)).DefaultIfEmpty().FirstOrDefault();
                if (Rowkey != 0)
                {
                    var purchaseList = dbContext.CashFlows.SingleOrDefault(x => x.RowKey == Rowkey);
                    if (AccountHeadKey == purchaseList.AccountHeadKey)
                    {
                        Balance = Balance + purchaseList.Amount;
                    }
                }
            }
            else if (dbContext.BankAccounts.Where(x => x.AccountHeadKey == accountHeadKey).Any())
            {
                long BankAccountKey = dbContext.BankAccounts.Where(x => x.AccountHeadKey == AccountHeadKey).Select(x => x.RowKey).FirstOrDefault();
                if (BankAccountKey != 0 && BankAccountKey != null)
                {
                    long HeadKey = dbContext.BankAccounts.Where(x => x.RowKey == BankAccountKey).Select(x => x.AccountHeadKey).FirstOrDefault();
                    Balance = dbContext.AccountHeads.Where(x => x.RowKey == HeadKey).Select(x => (x.TotalDebitAmount ?? 0) - (x.TotalCreditAmount ?? 0)).DefaultIfEmpty().FirstOrDefault();
                    if (Rowkey != 0)
                    {
                        var purchaseList = dbContext.CashFlows.SingleOrDefault(x => x.RowKey == Rowkey);
                        if (AccountHeadKey == purchaseList.AccountHeadKey)
                        {
                            Balance = Balance + purchaseList.Amount;
                        }
                    }
                }
            }
            else
            {
                Balance = Amount;
            }
            return Balance;
        }


        //private void FillNotificationDetail(CashFlowViewModel model)
        //{
        //    NotificationTemplate notificationTemplateModel = dbContext.NotificationTemplates.SingleOrDefault(row => row.RowKey == DbConstants.NotificationTemplate.PaymentReceiptNotification);
        //    if (notificationTemplateModel != null)
        //    {
        //        model.AutoEmail = notificationTemplateModel.AutoEmail;
        //        model.AutoSMS = notificationTemplateModel.AutoSMS;
        //        model.TemplateKey = notificationTemplateModel.RowKey;
        //    }
        //}


        public CashFlowViewModel FillSearchAcountHead(CashFlowViewModel model)
        {
            List<long> AccountHeadKeys = new List<long>();
            string accoundheadkeys = string.Join(",", dbContext.FeeTypes.Select(x => x.AccountHeadKey).ToList());
            //accoundheadkeys = accoundheadkeys + "," + string.Join(",", dbContext.UniversityMasters.Select(x => x.AccountHeadKey).ToList());
            if (accoundheadkeys != "")
            {
                AccountHeadKeys = accoundheadkeys.Split(',').Select(Int64.Parse).ToList();
            }
            List<long> AccountgroupKeys = new List<long>();
            AccountgroupKeys.Add(DbConstants.AccountGroup.Income);
            AccountgroupKeys.Add(DbConstants.AccountGroup.Expenses);
            List<byte> PaymentConfigTypeKeys = new List<byte>();
            PaymentConfigTypeKeys.Add(DbConstants.PaymentReceiptConfigType.ReceiptVoucher);
            PaymentConfigTypeKeys.Add(DbConstants.PaymentReceiptConfigType.Payment);
            PaymentConfigTypeKeys.Add(DbConstants.PaymentReceiptConfigType.PaymentAndReceipt);

            model.AccountHeads = (from ac in dbContext.AccountHeads.Where(x => (AccountgroupKeys.Contains(x.AccountHeadType.AccountGroupKey) && x.RowKey != DbConstants.AccountHead.CashAccount)
                                     || (PaymentConfigTypeKeys.Contains(x.PaymentConfigTypeKey ?? 0)))                                      
                                  where (ac.HideDaily ?? false) == false && ac.IsActive == true && !AccountHeadKeys.Contains(ac.RowKey)
                                  select new SelectListModel
                                  {
                                      RowKey = ac.RowKey,
                                      Text = ac.AccountHeadName
                                  }).ToList();

            return model;
        }
    }

}
