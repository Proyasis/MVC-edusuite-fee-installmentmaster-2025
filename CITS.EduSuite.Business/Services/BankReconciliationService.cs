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
    public class BankReconciliationService : IBankReconciliationService
    {
        private EduSuiteDatabase dbContext;
        public BankReconciliationService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public BankReconciliationViewModel GetBankReconciliationById(BankReconciliationViewModel model)
        {
            try
            {
                model.ReconcileCount = dbContext.BankReconciliations.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.TransactionDate) >= System.Data.Entity.DbFunctions.TruncateTime(model.FromDate) &&
                                                   System.Data.Entity.DbFunctions.TruncateTime(row.TransactionDate) <= System.Data.Entity.DbFunctions.TruncateTime(model.ToDate) &&
                                                   row.BankAccountKey == model.BankAccountKey).Count();
                model.BranchKey = dbContext.BankAccounts.Where(x => x.RowKey == model.BankAccountKey).Select(x => x.BranchKey ?? 0).DefaultIfEmpty().FirstOrDefault();
                model.BankStatementDetails = dbContext.BankStatementDetails
                .Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.TransactionDate) >= System.Data.Entity.DbFunctions.TruncateTime(model.FromDate) &&
                        System.Data.Entity.DbFunctions.TruncateTime(row.TransactionDate) <= System.Data.Entity.DbFunctions.TruncateTime(model.ToDate) &&
                        row.BankStatementMaster.BankAccountKey == model.BankAccountKey && !dbContext.BankReconciliations.Any(x => x.BankStatementKey == row.RowKey))
                .Select(row => new BankReconciliationDetailViewModel
                {
                    RowKey = row.RowKey,
                    ReferenceKey = row.ReferenceKey,
                    BankStatementKey = row.RowKey,
                    BankTransactionTypeKey = row.BankTransactionTypeKey,
                    CashFlowTypeKey = row.CashFlowTypeKey,
                    Amount = row.Amount,
                    TransactionDate = row.TransactionDate,
                    Particulars = row.Particulars,
                    BankTransactionTypeName = row.BankTransactionType.BankTransactionTypeName,
                    CashFlowTypeName = row.CashFlowType.CashFlowTypeName,
                    IsReconcile = false,
                    PaymentModeKey = DbConstants.PaymentMode.Bank
                }).ToList();
                int chequeValidity = dbContext.BankSettings.Select(y => y.ChequeValidity).FirstOrDefault();
                DateTime chequeDate = model.FromDate.AddDays(-chequeValidity);
                model.DefaultBankPaymentDetails = dbContext.VwBankPaymentSelects
                .Where(row => (System.Data.Entity.DbFunctions.TruncateTime(row.TransactionDate) >= System.Data.Entity.DbFunctions.TruncateTime(model.FromDate) &&
                        System.Data.Entity.DbFunctions.TruncateTime(row.TransactionDate) <= System.Data.Entity.DbFunctions.TruncateTime(model.ToDate) &&
                        row.BankAccountKey == model.BankAccountKey && !dbContext.BankReconciliations.Any(x => x.TransactionTypeKey == row.TransactionTypeKey && x.TransactionKey == row.TransactionKey))
                        || (dbContext.BankStatementDetails
                .Where(z => System.Data.Entity.DbFunctions.TruncateTime(z.TransactionDate) >= System.Data.Entity.DbFunctions.TruncateTime(model.FromDate) &&
                        System.Data.Entity.DbFunctions.TruncateTime(z.TransactionDate) <= System.Data.Entity.DbFunctions.TruncateTime(model.ToDate) &&
                        z.BankStatementMaster.BankAccountKey == model.BankAccountKey && !dbContext.BankReconciliations.Any(x => x.BankStatementKey == z.RowKey))).Any(x => (x.ReferenceKey == row.ReferenceNumber || (x.Amount == row.Amount && x.CashFlowTypeKey == row.CashFlowTypeKey)) && (row.PaymentModeKey == DbConstants.PaymentMode.Cheque && x.TransactionDate > chequeDate)))
                .Select(row => new BankReconciliationDetailViewModel
                {
                    RowKey = row.TransactionKey,
                    TransactionKey = row.TransactionKey,
                    TransactionTypeKey = row.TransactionTypeKey ?? 0,
                    ReferenceKey = row.ReferenceNumber,
                    CashFlowTypeKey = row.CashFlowTypeKey ?? 0,
                    Amount = row.Amount ?? 0,
                    TransactionDate = row.TransactionDate ?? DateTimeUTC.Now,
                    Particulars = row.Purpose,
                    BankTransactionTypeName = "",
                    CashFlowTypeName = row.CashFlowTypeKey == DbConstants.CashFlowType.In ? EduSuiteUIResources.In : EduSuiteUIResources.Out,
                    IsReconcile = false,
                    PaymentModeKey = row.PaymentModeKey,
                    AccountHeadKey = row.AccountHeadKey,
                    IsAdvance = row.IsAdvance,
                    //IsOrderPayment = row.IsOrderPayment
                }).ToList();
                if (model == null)
                {
                    model = new BankReconciliationViewModel();
                }
                FillDropDowns(model);
                return model;
            }
            catch (Exception ex)
            {
                model = new BankReconciliationViewModel();
                model.Message = ex.GetBaseException().Message;

                ActivityLog.CreateActivityLog(MenuConstants.BankReconciliation, ActionConstants.Edit, DbConstants.LogType.Error, model.BankAccountKey, ex.GetBaseException().Message);
                return model;
            }
        }
        public BankReconciliationViewModel ViewBankReconciliation(BankReconciliationViewModel model)
        {
            try
            {
                model.BranchKey = dbContext.BankAccounts.Where(x => x.RowKey == model.BankAccountKey).Select(x => x.BranchKey ?? 0).DefaultIfEmpty().FirstOrDefault();
                model.BankReconciliationDetails = (from BR in dbContext.BankReconciliations.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.TransactionDate) >= System.Data.Entity.DbFunctions.TruncateTime(model.FromDate) &&
                                                   System.Data.Entity.DbFunctions.TruncateTime(row.TransactionDate) <= System.Data.Entity.DbFunctions.TruncateTime(model.ToDate) &&
                                                   row.BankAccountKey == model.BankAccountKey)
                                                   join DB in dbContext.VwBankPaymentSelects on new { BR.TransactionTypeKey, BR.TransactionKey } equals new { TransactionTypeKey = (byte)DB.TransactionTypeKey, DB.TransactionKey } into DBJ
                                                   from DB in DBJ.DefaultIfEmpty()
                                                   join OB in dbContext.OtherBankTransactions on new { BR.TransactionTypeKey, BR.TransactionKey } equals new { TransactionTypeKey = DbConstants.TransactionType.OtherBankTransaction, TransactionKey = OB.RowKey } into OBJ
                                                   from OB in OBJ.DefaultIfEmpty()
                                                   join BS in dbContext.BankStatementDetails on BR.BankStatementKey equals BS.RowKey into BSJ
                                                   from BS in BSJ.DefaultIfEmpty()
                                                   select new BankReconciliationSelectViewModel
                                                  {
                                                      StatementReferenceKey = BS.ReferenceKey,
                                                      StatementAmount = BS.Amount,
                                                      StatementTransactionDate = BS.TransactionDate,
                                                      StatementParticulars = BS.Particulars,
                                                      StatementBankTransactionTypeName = BS.BankTransactionType.BankTransactionTypeName,
                                                      StatementCashFlowTypeName = BS.CashFlowType.CashFlowTypeName,
                                                      DefaultReferenceKey = BR.ReferenceNumber,
                                                      DefaultAmount = BR.Amount ?? 0,
                                                      DefaultTransactionDate = BR.TransactionDate ?? OB.TransactionDate,
                                                      DefaultParticulars = BR.Purpose,
                                                      DefaultBankTransactionTypeName = "",
                                                      DefaultCashFlowTypeName = (BR.CashFlowTypeKey) == DbConstants.CashFlowType.In ? EduSuiteUIResources.In : EduSuiteUIResources.Out,
                                                      Remark = BR.Remark,
                                                      Status = BR.ProcessStatu.ProcessStatusName,
                                                      ProcessStatusKey = BR.ProcessStatusKey
                                                  }).ToList();

                if (model == null)
                {
                    model = new BankReconciliationViewModel();
                }
                return model;
            }
            catch (Exception ex)
            {
                model = new BankReconciliationViewModel();
                model.Message = ex.GetBaseException().Message;
                ActivityLog.CreateActivityLog(MenuConstants.BankReconciliation, ActionConstants.View, DbConstants.LogType.Error, model.BankAccountKey, ex.GetBaseException().Message);
                return model;
            }
        }
        public BankReconciliationViewModel CreateBankReconciliation(BankReconciliationViewModel model)
        {
            FillDropDowns(model);
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    if (model.DefaultBankPaymentDetails.Any(x => x.PaymentModeKey == DbConstants.PaymentMode.Cheque && x.ProcessStatusKey != DbConstants.ProcessStatus.Pending && x.IsReconcile == true))
                    {
                        createChequeClearance(model);
                    }
                    if (model.DefaultBankPaymentDetails.Any(x => x.TransactionTypeKey == DbConstants.TransactionType.OtherBankTransaction && x.IsReconcile == true))
                    {
                        createOtherBankTransaction(model);
                    }
                    long maxKey = dbContext.BankReconciliations.Select(x => x.RowKey).DefaultIfEmpty().Max();
                    foreach (BankReconciliationDetailViewModel modelItem in model.DefaultBankPaymentDetails.Where(x => x.IsReconcile == true))
                    {
                        BankReconciliation BankReconciliation = new BankReconciliation();
                        BankReconciliation.RowKey = maxKey + 1;
                        BankReconciliation.TransactionKey = modelItem.TransactionKey;
                        BankReconciliation.TransactionTypeKey = modelItem.TransactionTypeKey;
                        BankReconciliation.ProcessStatusKey = modelItem.ProcessStatusKey;
                        BankReconciliation.TransactionDate = modelItem.TransactionDate;
                        BankReconciliation.CashFlowTypeKey = modelItem.CashFlowTypeKey;
                        BankReconciliation.Amount = modelItem.Amount;
                        BankReconciliation.BankAccountKey = model.BankAccountKey;
                        BankReconciliation.ReferenceNumber = modelItem.ReferenceKey;
                        BankReconciliation.Purpose = modelItem.Particulars;
                        BankReconciliation.Remark = modelItem.Remark;
                        BankReconciliation.BankStatementKey = modelItem.BankStatementKey == 0 ? null : modelItem.BankStatementKey;
                        dbContext.BankReconciliations.Add(BankReconciliation);
                        maxKey++;
                    }
                    if (model.DefaultBankPaymentDetails.Any(x => x.ProcessStatusKey == DbConstants.ProcessStatus.Rejected && x.PaymentModeKey == DbConstants.PaymentMode.Bank))
                    {
                        DeletePayment(model);
                    }
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.BankReconciliation, ActionConstants.Add, DbConstants.LogType.Info, model.BankAccountKey, model.Message);
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
                    model.Message = ex.GetBaseException().Message;
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.BankReconciliation);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.BankReconciliation, ActionConstants.Add, DbConstants.LogType.Error, model.BankAccountKey, ex.GetBaseException().Message);
                }
            }
            //if (!model.IsSecond && model.IsSuccessful)
            //{

            //    model.UpdateType = DbConstants.UpdationType.Create;
            //    TriggerDatabaseInBackground(model);
            //}
            return model;
        }
        private void createOtherBankTransaction(BankReconciliationViewModel model)
        {
            List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
            long maxKey = dbContext.OtherBankTransactions.Select(x => x.RowKey).DefaultIfEmpty().Max();
            foreach (BankReconciliationDetailViewModel modelItem in model.DefaultBankPaymentDetails.Where(x => x.TransactionTypeKey == DbConstants.TransactionType.OtherBankTransaction && x.IsReconcile == true).ToList())
            {
                OtherBankTransaction OtherBankTransaction = new OtherBankTransaction();
                OtherBankTransaction.RowKey = maxKey + 1;
                OtherBankTransaction.ReferenceNumber = modelItem.ReferenceKey;
                OtherBankTransaction.BankAccountKey = model.BankAccountKey;
                OtherBankTransaction.BankStatementKey = modelItem.BankStatementKey ?? 0;
                OtherBankTransaction.AccountHeadKey = modelItem.AccountHeadKey ?? 0;
                OtherBankTransaction.ChequeStatusKey = modelItem.ProcessStatusKey;
                OtherBankTransaction.TransactionDate = modelItem.TransactionDate;
                OtherBankTransaction.Purpose = modelItem.Particulars;
                OtherBankTransaction.Amount = modelItem.Amount;
                OtherBankTransaction.CashFlowTypeKey = modelItem.CashFlowTypeKey;
                dbContext.OtherBankTransactions.Add(OtherBankTransaction);
                modelItem.RowKey = OtherBankTransaction.RowKey;
                modelItem.TransactionKey = OtherBankTransaction.RowKey;
                accountFlowModelList = DebitAmountList(modelItem, model, accountFlowModelList, false);
                maxKey++;

                //BankAccountViewModel bankModel = new BankAccountViewModel();
                //BankAccountService bankAccountService = new BankAccountService(dbContext);
                //bankModel.RowKey = model.BankAccountKey;
                //bankModel.Amount = modelItem.CashFlowTypeKey == DbConstants.CashFlowType.Out ? -(modelItem.Amount) : (modelItem.Amount);
                //bankAccountService.UpdateCurrentAccountBalance(bankModel,false, (modelItem.CashFlowTypeKey == DbConstants.CashFlowType.Out) ? true : false,null);

            }
            if (accountFlowModelList.Count != 0)
            {
                CreateAccountFlow(accountFlowModelList, false);
            }
        }
        private void createChequeClearance(BankReconciliationViewModel model)
        {
            List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
            long maxKey = dbContext.ChequeClearances.Select(x => x.RowKey).DefaultIfEmpty().Max();
            foreach (BankReconciliationDetailViewModel modelItem in model.DefaultBankPaymentDetails.Where(x => x.PaymentModeKey == DbConstants.PaymentMode.Cheque && x.ProcessStatusKey != DbConstants.ProcessStatus.Pending && x.IsReconcile == true).ToList())
            {
                ChequeClearanceViewModel ChequeClearanceModel = new ChequeClearanceViewModel();
                ChequeClearanceModel.RowKey = maxKey + 1;
                ChequeClearanceModel.ClearanceDate = modelItem.TransactionDate;
                ChequeClearanceModel.PaymentModeKey = DbConstants.PaymentMode.Bank;
                ChequeClearanceModel.BankAccountKey = model.BankAccountKey;
                ChequeClearanceModel.TransactionKey = modelItem.TransactionKey;
                ChequeClearanceModel.TransactionTypeKey = modelItem.TransactionTypeKey;
                ChequeClearanceModel.Purpose = modelItem.Particulars;
                ChequeClearanceModel.IsApproved = modelItem.ProcessStatusKey == DbConstants.ProcessStatus.Approved ? true : false;
                ChequeClearanceModel.Remark = modelItem.Remark;
                ChequeClearanceModel.IsAdvance = modelItem.IsAdvance ?? false;
                ChequeClearanceModel.IsOrderPayment = modelItem.IsOrderPayment ?? false;
                ChequeClearanceModel.OldBankAccountKey = model.BankAccountKey;
                ChequeClearanceModel.BranchKey = model.BranchKey;
                ChequeClearanceModel.AccountHeadKey = modelItem.AccountHeadKey ?? 0;
                ChequeClearanceModel.Amount = modelItem.Amount;
                ChequeClearanceModel.CashFlowTypeKey = modelItem.CashFlowTypeKey;
                ChequeClearanceService chequeClearanceService = new ChequeClearanceService(dbContext);
                chequeClearanceService.createChequeClearance(ChequeClearanceModel, maxKey);
                maxKey++;
            }
            if (accountFlowModelList.Count != 0)
            {
                CreateAccountFlow(accountFlowModelList, false);
            }
        }
        private void DeletePayment(BankReconciliationViewModel model)
        {
            AccountFlowViewModel accountFlowModel = new AccountFlowViewModel();
            AccountFlowService accountFlowService = new AccountFlowService(dbContext);
            long bankKey = 0;
            foreach (BankReconciliationDetailViewModel modelItem in model.DefaultBankPaymentDetails.Where(x => x.ProcessStatusKey == DbConstants.ProcessStatus.Rejected && x.PaymentModeKey == DbConstants.PaymentMode.Bank))
            {
                if (modelItem.TransactionTypeKey == DbConstants.TransactionType.CashFlow)
                {
                    long partyTypeKey = 0;
                    // if (dbContext.PartyCreditBalancePayments.Any(x => x.CashFlowKey == modelItem.TransactionKey))
                    //{
                    //    PartyCreditBalancePayment paymentModel = dbContext.PartyCreditBalancePayments.SingleOrDefault(x => x.CashFlowKey == modelItem.TransactionKey);
                    //   partyTypeKey = paymentModel.Party.PartyTypeKey;
                    //   dbContext.PartyCreditBalancePayments.Remove(paymentModel);
                    // }
                    //  if (dbContext.PurchaseOrderPayments.Any(x => x.CashFlowKey == modelItem.TransactionKey))
                    // {
                    //    List<PurchaseOrderPayment> paymentModelList = dbContext.PurchaseOrderPayments.Where(x => x.CashFlowKey == modelItem.TransactionKey).ToList();
                    //     partyTypeKey = paymentModelList.Select(x => x.PurchaseOrderMaster.Party.PartyTypeKey).FirstOrDefault();
                    //    paymentModelList.ForEach(row => dbContext.PurchaseOrderPayments.Remove(row));
                    // }
                    //if (dbContext.SalesOrderReceipts.Any(x => x.CashFlowKey == modelItem.TransactionKey))
                    //{
                    //    List<SalesOrderReceipt> paymentModelList = dbContext.SalesOrderReceipts.Where(x => x.CashFlowKey == modelItem.TransactionKey).ToList();
                    //    partyTypeKey = paymentModelList.Select(x => x.SalesOrderMaster.Party.PartyTypeKey).FirstOrDefault();
                    //    paymentModelList.ForEach(row => dbContext.SalesOrderReceipts.Remove(row));
                    //}
                    //if (dbContext.RawMaterialSalesPayments.Any(x => x.CashFlowKey == modelItem.TransactionKey))
                    //{
                    //    List<RawMaterialSalesPayment> paymentModelList = dbContext.RawMaterialSalesPayments.Where(x => x.CashFlowKey == modelItem.TransactionKey).ToList();
                    //    partyTypeKey = paymentModelList.Select(x => x.RawMaterialSalesMaster.Party.PartyTypeKey).FirstOrDefault();
                    //    paymentModelList.ForEach(row => dbContext.RawMaterialSalesPayments.Remove(row));
                    //}
                    //if (dbContext.SalesOrderOutSourcePayments.Any(x => x.CashFlowKey == modelItem.TransactionKey))
                    //{
                    //    List<SalesOrderOutSourcePayment> paymentModelList = dbContext.SalesOrderOutSourcePayments.Where(x => x.CashFlowKey == modelItem.TransactionKey).ToList();
                    //    partyTypeKey = paymentModelList.Select(x => x.SalesOrderOutSourceDetail.Party.PartyTypeKey).FirstOrDefault();
                    //    paymentModelList.ForEach(row => dbContext.SalesOrderOutSourcePayments.Remove(row));
                    //}
                    //if (dbContext.AccountFlows.Any(x => x.TransactionTypeKey == DbConstants.TransactionType.DiscountCashFlow && x.TransactionKey == modelItem.TransactionKey))
                    //{
                    //    accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.DiscountCashFlow;
                    //    accountFlowModel.TransactionKey = modelItem.TransactionKey;
                    //    accountFlowService.DeleteAccountFlow(accountFlowModel);
                    //    accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.DiscountCashFlowParty;
                    //    accountFlowService.DeleteAccountFlow(accountFlowModel);
                    //}
                    CashFlow cashflow = dbContext.CashFlows.SingleOrDefault(x => x.RowKey == modelItem.TransactionKey);
                    //modelItem.RefKey = cashflow.RefKey;
                    bankKey = cashflow.BankAccountKey ?? 0;
                    accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.CashFlow;
                    accountFlowModel.TransactionKey = modelItem.TransactionKey;
                    accountFlowService.DeleteAccountFlow(accountFlowModel);

                    accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.CashFlowRecievable;

                    accountFlowService.DeleteAccountFlow(accountFlowModel);
                }

                //else if (modelItem.TransactionTypeKey == DbConstants.TransactionType.Sales)
                // {
                //SalesOrderReceipt paymentModel = dbContext.SalesOrderReceipts.SingleOrDefault(x => x.RowKey == modelItem.TransactionKey);
                //bool isAdvance = paymentModel.IsAdavance;
                //byte orderStatusKey = paymentModel.SalesOrderMaster.OrderStatusKey ?? 0;
                //modelItem.RefKey = paymentModel.RefKey;
                //dbContext.SalesOrderReceipts.Remove(paymentModel);
                //accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.Sales;
                accountFlowModel.TransactionKey = modelItem.TransactionKey;
                accountFlowService.DeleteAccountFlow(accountFlowModel);
                //if (isAdvance != true)
                // {
                //    accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.SalesRecievableReciept;
                // }
                //  else
                //  {
                //if (orderStatusKey == DbConstants.OrderStatus.Delivery)
                // {
                //  accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.SalesAdvanceRecievable;
                //   accountFlowService.UpdateAccountFlowAmount(accountFlowModel);
                // }
                //accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.SalesAdvance;
                // }
                accountFlowService.DeleteAccountFlow(accountFlowModel);
                // }

                //else if (modelItem.TransactionTypeKey == DbConstants.TransactionType.RawMaterialSales)
                //{
                //    //RawMaterialSalesPayment paymentModel = dbContext.RawMaterialSalesPayments.SingleOrDefault(x => x.RowKey == modelItem.TransactionKey);
                //    //modelItem.RefKey = paymentModel.RefKey;
                //    //dbContext.RawMaterialSalesPayments.Remove(paymentModel);
                //    accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.RawMaterialSales;
                //    accountFlowModel.TransactionKey = modelItem.TransactionKey;
                //    accountFlowService.DeleteAccountFlow(accountFlowModel);
                //    accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.RawMaterialSalesPayablePayment;
                //    accountFlowService.DeleteAccountFlow(accountFlowModel);
                //}

                //else if (modelItem.TransactionTypeKey == DbConstants.TransactionType.OutSource)
                //{
                //    //SalesOrderOutSourcePayment paymentModel = dbContext.SalesOrderOutSourcePayments.SingleOrDefault(x => x.RowKey == modelItem.TransactionKey);
                //    //modelItem.RefKey = paymentModel.RefKey;
                //    //dbContext.SalesOrderOutSourcePayments.Remove(paymentModel);
                //    accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.OutSource;
                //    accountFlowModel.TransactionKey = modelItem.TransactionKey;
                //    accountFlowService.DeleteAccountFlow(accountFlowModel);
                //    accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.OutSourcePayable;
                //    accountFlowService.DeleteAccountFlow(accountFlowModel);
                //}

                //else if (modelItem.TransactionTypeKey == DbConstants.TransactionType.Purchase)
                //{
                //    //PurchaseOrderPayment paymentModel = dbContext.PurchaseOrderPayments.SingleOrDefault(x => x.RowKey == modelItem.TransactionKey);
                //    //modelItem.RefKey = paymentModel.RefKey;
                //    //dbContext.PurchaseOrderPayments.Remove(paymentModel);
                //    accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.Purchase;
                //    accountFlowModel.TransactionKey = modelItem.TransactionKey;
                //    accountFlowService.DeleteAccountFlow(accountFlowModel);
                //    accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.PurchasePayable;
                //    accountFlowService.DeleteAccountFlow(accountFlowModel);
                //}

                if (modelItem.TransactionTypeKey == DbConstants.TransactionType.Salary)
                {
                    //EmployeeSalaryPayment paymentModel = dbContext.EmployeeSalaryPayments.SingleOrDefault(x => x.RowKey == modelItem.TransactionKey);
                    //modelItem.RefKey = paymentModel.RefKey;
                    //dbContext.EmployeeSalaryPayments.Remove(paymentModel);
                    accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.Salary;
                    accountFlowModel.TransactionKey = modelItem.TransactionKey;
                    accountFlowService.DeleteAccountFlow(accountFlowModel);
                }

                else if (modelItem.TransactionTypeKey == DbConstants.TransactionType.SalaryAdvance)
                {
                    //EmployeeSalaryAdvancePayment paymentModel = dbContext.EmployeeSalaryAdvancePayments.SingleOrDefault(x => x.RowKey == modelItem.TransactionKey);
                    //modelItem.RefKey = paymentModel.RefKey;
                    // dbContext.EmployeeSalaryAdvancePayments.Remove(paymentModel);
                    accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.SalaryAdvance;
                    accountFlowModel.TransactionKey = modelItem.TransactionKey;
                    accountFlowService.DeleteAccountFlow(accountFlowModel);
                }
                //BankAccountViewModel bankModel = new BankAccountViewModel();
                //BankAccountService bankAccountService = new BankAccountService(dbContext);
                //bankModel.RowKey = bankKey;
                //bankModel.Amount = modelItem.CashFlowTypeKey == DbConstants.CashFlowType.Out ? modelItem.Amount : -(modelItem.Amount);
                //bankAccountService.UpdateCurrentAccountBalance(bankModel,true,true, modelItem.Amount);
            }
        }
        private void FillDropDowns(BankReconciliationViewModel model)
        {
            GetBranches(model);
            FillBankAccounts(model);
            FillAccountHeads(model);
        }
        private void FillAccountHeads(BankReconciliationViewModel model)
        {
            model.AccountHeads = (from ac in dbContext.VwAccountHeadSelectActiveOnlies
                                  //join p in dbContext.VwPartySelectActiveOnlies on ac.AccountHeadCode equals p.AccountHeadCode into pj
                                  //from p in pj.DefaultIfEmpty()
                                  //join rm in dbContext.RawMaterials on ac.AccountHeadCode equals rm.AccountHeadCode into rmj
                                  //from rm in rmj.DefaultIfEmpty()
                                  //where p.RowKey == null && rm.RowKey == null && ac.RowKey != DbConstants.AccountHead.CashSale
                                  select new SelectListModel
                                  {
                                      RowKey = ac.RowKey,
                                      Text = ac.AccountHeadName
                                  }).ToList();
        }
        public BankReconciliationViewModel GetBranches(BankReconciliationViewModel model)
        {
            var UserBranchKey = dbContext.Employees.Where(row => row.AppUserKey == DbConstants.User.UserKey).Select(row => row.BranchKey).FirstOrDefault();

            IQueryable<vwBranchSelectActiveOnly> Branches = dbContext.vwBranchSelectActiveOnlies;
            if (UserBranchKey != 0)
            {
                Branches = Branches.Where(row => row.RowKey == UserBranchKey);
                model.BranchKey = UserBranchKey;
            }
            model.Branches = Branches.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BranchName
            }).ToList();
            return model;
        }
        public BankReconciliationViewModel FillBankAccounts(BankReconciliationViewModel model)
        {
            model.BankAccounts = dbContext.BranchAccounts.Where(x => x.BranchKey == model.BranchKey && x.BankAccount.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.BankAccount.RowKey,
                Text = (row.BankAccount.NameInAccount ?? row.BankAccount.AccountNumber) + EduSuiteUIResources.Hyphen + row.BankAccount.Bank.BankName
            }).ToList();

            return model;
        }

        #region Account
        private void CreateAccountFlow(List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate)
        {
            AccountFlowService accounFlowService = new AccountFlowService(dbContext);
            if (IsUpdate == false)
            {
                accounFlowService.CreateAccountFlow(accountFlowModelList);
            }
            else
            {
                accounFlowService.UpdateAccountFlow(accountFlowModelList);
            }
        }
        private List<AccountFlowViewModel> DebitAmountList(BankReconciliationDetailViewModel modelItem, BankReconciliationViewModel model, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate)
        {
            long ExtraUpdateKey = 0;
            long accountHeadKey = dbContext.BankAccounts.Where(x => x.RowKey == model.BankAccountKey).Select(x => x.AccountHeadKey).DefaultIfEmpty().FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = modelItem.CashFlowTypeKey,
                AccountHeadKey = accountHeadKey,
                Amount = modelItem.Amount,
                TransactionTypeKey = DbConstants.TransactionType.OtherBankTransaction,
                VoucherTypeKey = DbConstants.VoucherType.OtherBankTransaction,
                TransactionDate = modelItem.TransactionDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                Purpose = modelItem.Particulars,
                BranchKey = model.BranchKey,
                TransactionKey = modelItem.RowKey,
            });
            accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == modelItem.AccountHeadKey).Select(x => x.RowKey).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = modelItem.CashFlowTypeKey == DbConstants.CashFlowType.Out ? DbConstants.CashFlowType.In : DbConstants.CashFlowType.Out,
                AccountHeadKey = accountHeadKey,
                Amount = modelItem.Amount,
                TransactionTypeKey = DbConstants.TransactionType.OtherBankTransaction,
                VoucherTypeKey = DbConstants.VoucherType.OtherBankTransaction,
                TransactionDate = modelItem.TransactionDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                Purpose = modelItem.Particulars,
                BranchKey = model.BranchKey,
                TransactionKey = modelItem.RowKey,
            });
            return accountFlowModelList;
        }

        #endregion

        
    }
}
