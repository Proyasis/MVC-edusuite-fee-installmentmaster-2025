using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;


namespace CITS.EduSuite.Business.Services
{
    public class AccountHeadService : IAccountHeadService
    {
        private EduSuiteDatabase dbContext;
        public AccountHeadService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public AccountHeadViewModel GetAccountHeadById(int? id, short? type, byte? group)
        {
            try
            {
                AccountHeadViewModel model = new AccountHeadViewModel();
                model = dbContext.AccountHeads.Select(row => new AccountHeadViewModel
                {
                    RowKey = row.RowKey,
                    AccountHeadName = row.AccountHeadName,
                    AccountHeadCode = row.AccountHeadCode,
                    AccountHeadTypeKey = row.AccountHeadTypeKey,
                    IsActive = row.IsActive,
                    AccountGroupKey = row.AccountHeadType.AccountGroupKey,
                    IsSystemAccount = row.IsSystemAccount,
                    HideDaily = row.HideDaily ?? false,
                    HideFuture = row.HideFuture ?? false,
                    FutureAccountHeads = row.FutureAccountHeads,
                    PaymentConfigTypeKey = row.PaymentConfigTypeKey
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new AccountHeadViewModel();
                    model.AccountGroupKey = group ?? 0;
                    model.AccountHeadTypeKey = type ?? 0;
                }
                else
                {
                    if (model.FutureAccountHeads != null && model.FutureAccountHeads != "")
                    {
                        model.FutureAccountHeadKeys = model.FutureAccountHeads.Split(',').Select(Int64.Parse).ToList();
                    }
                }
                FillDropDown(model);
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.AccountHead, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new AccountHeadViewModel();

            }
        }
        public AccountHeadViewModel CreateAccountHead(AccountHeadViewModel model)
        {
            var AccountHeadCheck = dbContext.AccountHeads.Where(row => row.AccountHeadName.ToLower() == model.AccountHeadName.ToLower() && row.AccountHeadTypeKey == model.AccountHeadTypeKey).ToList();
            FillDropDown(model);
            if (AccountHeadCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.AccountHeadName);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    if (model.FutureAccountHeadKeys != null)
                    {
                        int i = 0;
                        foreach (long FutureAccountHead in model.FutureAccountHeadKeys)
                        {
                            if (i > 0)
                            {
                                model.FutureAccountHeads = model.FutureAccountHeads + "," + FutureAccountHead;
                            }
                            else
                            {
                                model.FutureAccountHeads = model.FutureAccountHeads + FutureAccountHead;
                            }
                            i = i + 1;
                        }

                    }
                    model = createAccountChart(model);
                    transaction.Commit();
                    ActivityLog.CreateActivityLog(MenuConstants.AccountHead, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = ex.GetBaseException().Message;
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.AccountHead);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.AccountHead, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public AccountHeadViewModel createAccountChart(AccountHeadViewModel model)
        {
            AccountHead AccountHeadModel = new AccountHead();
            long MaxKey = dbContext.AccountHeads.Select(p => p.RowKey).DefaultIfEmpty().Max();
            MaxKey = MaxKey < 50 ? 50 : MaxKey;
            List<AccountHead> AccountheadList = dbContext.AccountHeads.Where(p => p.AccountHeadTypeKey == model.AccountHeadTypeKey).OrderByDescending(row => row.RowKey).ToList();
            List<long> longCodes = dbContext.AccountHeads.Where(p => p.AccountHeadTypeKey == model.AccountHeadTypeKey).OrderByDescending(row => row.RowKey).Select(p => p.AccountHeadCode.Replace(p.AccountHeadType.AccountHeadTypeCode, "")).ToList().Select(x => Int64.Parse(x)).ToList();
            long CodeMax = 0;
            if (longCodes.Count > 0)
            {
                CodeMax = longCodes.Max();
            }
            //string CodeMax = dbContext.AccountHeads.Where(p => p.AccountHeadTypeKey == model.AccountHeadTypeKey).OrderByDescending(row => row.RowKey).Select(p => p.AccountHeadCode).FirstOrDefault();

            string AccountTypeCode = dbContext.AccountHeadTypes.Where(x => x.RowKey == model.AccountHeadTypeKey).Select(x => x.AccountHeadTypeCode).FirstOrDefault();
            string AccountHeadCode = "";
            if (CodeMax != 0)
            {
                //CodeMax = CodeMax.Replace(AccountTypeCode, "");
                long UniqueId = CodeMax + 1;
                if (UniqueId >= 10)
                {
                    AccountHeadCode = AccountTypeCode + UniqueId;
                }
                else
                {
                    AccountHeadCode = AccountTypeCode + "0" + UniqueId;
                }
            }
            else
            {
                AccountHeadCode = AccountTypeCode + "0" + 1;
            }

            AccountHeadModel.RowKey = Convert.ToInt16(MaxKey + 1);
            AccountHeadModel.AccountHeadName = model.AccountHeadName;
            AccountHeadModel.AccountHeadCode = AccountHeadCode;
            AccountHeadModel.AccountHeadTypeKey = model.AccountHeadTypeKey;
            AccountHeadModel.IsActive = model.IsActive;
            AccountHeadModel.DisplayOrder = AccountHeadModel.RowKey;
            AccountHeadModel.IsSystemAccount = model.IsSystemAccount;
            AccountHeadModel.TotalCreditAmount = AccountHeadModel.TotalCreditAmount;
            AccountHeadModel.TotalDebitAmount = AccountHeadModel.TotalDebitAmount;
            AccountHeadModel.FutureAccountHeads = model.FutureAccountHeads;
            AccountHeadModel.HideFuture = model.HideFuture;
            AccountHeadModel.HideDaily = model.HideDaily;
            AccountHeadModel.PaymentConfigTypeKey = model.PaymentConfigTypeKey;
            dbContext.AccountHeads.Add(AccountHeadModel);
            dbContext.SaveChanges();
            model.AccountHeadCode = AccountHeadCode;
            model.RowKey = AccountHeadModel.RowKey;
            model.Message = EduSuiteUIResources.Success;
            model.IsSuccessful = true;
            return model;
        }
        public AccountHeadViewModel UpdateAccountHead(AccountHeadViewModel model)
        {
            FillDropDown(model);
            var AccountHeadCheck = dbContext.AccountHeads.Where(row => row.AccountHeadName.ToLower() == model.AccountHeadName.ToLower()
               && row.AccountHeadTypeKey == model.AccountHeadTypeKey && row.RowKey != model.RowKey).ToList();

            if (AccountHeadCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.AccountHeadName);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    if (model.FutureAccountHeadKeys != null)
                    {
                        int i = 0;
                        foreach (long AccountHead in model.FutureAccountHeadKeys)
                        {
                            if (i > 0)
                            {
                                model.FutureAccountHeads = model.FutureAccountHeads + "," + AccountHead;
                            }
                            else
                            {
                                model.FutureAccountHeads = model.FutureAccountHeads + AccountHead;
                            }
                            i = i + 1;
                        }

                    }
                    model = updateAccountChart(model);
                    transaction.Commit();
                    ActivityLog.CreateActivityLog(MenuConstants.AccountHead, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = ex.GetBaseException().Message;
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.AccountHead);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.AccountHead, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;

        }
        public AccountHeadViewModel updateAccountChart(AccountHeadViewModel model)
        {
            AccountHead AccountHeadModel = new AccountHead();
            AccountHeadModel = dbContext.AccountHeads.SingleOrDefault(row => row.RowKey == model.RowKey);
            if (AccountHeadModel.AccountHeadTypeKey != model.AccountHeadTypeKey)
            {
                List<long> longCodes = dbContext.AccountHeads.Where(p => p.AccountHeadTypeKey == model.AccountHeadTypeKey).OrderByDescending(row => row.RowKey).Select(p => p.AccountHeadCode.Replace(p.AccountHeadType.AccountHeadTypeCode, "")).ToList().Select(x => Int64.Parse(x)).ToList();
                long CodeMax = 0;
                if (longCodes.Count > 0)
                {
                    CodeMax = longCodes.Max();
                }
                //string CodeMax = dbContext.AccountHeads.Where(p => p.AccountHeadTypeKey == model.AccountHeadTypeKey).OrderByDescending(row => row.RowKey).Select(p => p.AccountHeadCode).FirstOrDefault();
                var AccountHeadTypeList = dbContext.AccountHeadTypes.SingleOrDefault(x => x.RowKey == model.AccountHeadTypeKey);
                string AccountTypeCode = AccountHeadTypeList.AccountHeadTypeCode;
                string AccountHeadCode = "";
                if (CodeMax != 0)
                {
                    //CodeMax = CodeMax.Replace(AccountTypeCode, "");
                    long UniqueId = CodeMax + 1;
                    if (UniqueId >= 10)
                    {
                        AccountHeadCode = AccountTypeCode + UniqueId;
                    }
                    else
                    {
                        AccountHeadCode = AccountTypeCode + "0" + UniqueId;
                    }
                }
                else
                {
                    AccountHeadCode = AccountTypeCode + "0" + 1;
                }
                AccountHeadModel.AccountHeadCode = AccountHeadCode;
            }
            AccountHeadModel.AccountHeadName = model.AccountHeadName;
            AccountHeadModel.AccountHeadTypeKey = model.AccountHeadTypeKey;
            AccountHeadModel.TotalCreditAmount = AccountHeadModel.TotalCreditAmount;
            AccountHeadModel.TotalDebitAmount = AccountHeadModel.TotalDebitAmount;
            AccountHeadModel.IsActive = model.IsActive;
            AccountHeadModel.FutureAccountHeads = model.FutureAccountHeads;
            AccountHeadModel.HideFuture = model.HideFuture;
            AccountHeadModel.HideDaily = model.HideDaily;
            AccountHeadModel.PaymentConfigTypeKey = model.PaymentConfigTypeKey;
            dbContext.SaveChanges();
            model.AccountHeadCode = AccountHeadModel.AccountHeadCode;
            model.Message = EduSuiteUIResources.Success;
            model.IsSuccessful = true;
            return model;
        }
        public AccountHeadViewModel DeleteAccountHead(AccountHeadViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    AccountHead AccountHeadModel = dbContext.AccountHeads.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.AccountHeads.Remove(AccountHeadModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.AccountHead, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = ex.GetBaseException().Message;
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.AccountHead);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.AccountHead, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = ex.GetBaseException().Message;
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.AccountHead);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.AccountHead, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }
        public List<AccountHeadViewModel> GetAccountHead(string searchText, AccountHeadViewModel model)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;
                IQueryable<AccountHeadViewModel> AccountHeadList = (from p in dbContext.AccountHeads
                                                                    orderby p.DisplayOrder
                                                                    where (p.AccountHeadName.Contains(searchText) || p.AccountHeadCode.Contains(searchText))
                                                                    select new AccountHeadViewModel
                                                                    {
                                                                        RowKey = p.RowKey,
                                                                        AccountHeadName = p.AccountHeadName,
                                                                        AccountHeadCode = p.AccountHeadCode,
                                                                        AccountHeadTypeName = p.AccountHeadType.AccountHeadTypeName,
                                                                        IsSystemAccount = p.IsSystemAccount,
                                                                        AccountGroupName = p.AccountHeadType.AccountGroup.AccountGroupName,
                                                                        AccountGroupKey = p.AccountHeadType.AccountGroupKey,
                                                                        AccountHeadTypeKey = p.AccountHeadTypeKey
                                                                    });

                if (model.SearchAccountGroupKey != 0)
                {
                    AccountHeadList = AccountHeadList.Where(x => x.AccountGroupKey == model.SearchAccountGroupKey);
                }
                if (model.SearchAccountHeadTypeKey != 0)
                {
                    AccountHeadList = AccountHeadList.Where(x => x.AccountHeadTypeKey == model.SearchAccountHeadTypeKey);
                }
                if (model.SortBy != "")
                {
                    AccountHeadList = SortSAccountHead(AccountHeadList, model.SortBy, model.SortOrder);
                }
                model.TotalRecords = AccountHeadList.Count();
                return model.PageIndex != 0 ? AccountHeadList.Skip(Skip).Take(Take).ToList() : AccountHeadList.ToList();

            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.AccountHead, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<AccountHeadViewModel>();

            }
        }
        private IQueryable<AccountHeadViewModel> SortSAccountHead(IQueryable<AccountHeadViewModel> Query, string SortName, string SortOrder)
        {

            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(AccountHeadViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<AccountHeadViewModel>(resultExpression);

        }
        private void FillDropDown(AccountHeadViewModel model)
        {
            FillAccountGroup(model);
            FillAccountHeadType(model);

            FillAccountHeads(model);
            FillPaymentConfigs(model);
        }
        public AccountHeadViewModel FillAccountHeadType(AccountHeadViewModel model)
        {
            model.AccountHeadType = dbContext.VwAccountHeadTypes.Where(x => x.AccountGroupKey == model.AccountGroupKey).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AccountHeadTypeName
            }).ToList();
            return model;
        }
        public void FillAccountGroup(AccountHeadViewModel model)
        {
            model.AccountGroup = dbContext.VwAccountGroups.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AccountGroupName
            }).ToList();
        }
        private void FillAccountHeads(AccountHeadViewModel model)
        {
            model.AccountHeads = dbContext.AccountHeads.Where(x => (x.AccountHeadType.AccountGroupKey == DbConstants.AccountGroup.Income || x.AccountHeadType.AccountGroupKey == DbConstants.AccountGroup.Expenses) && x.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AccountHeadName
            }).ToList();
        }

        public void FillPaymentConfigs(AccountHeadViewModel model)
        {
            model.PaymentConfigs = typeof(DbConstants.PaymentReceiptConfigType).GetFields().Select(row => new SelectListModel
            {
                RowKey = Convert.ToByte(row.GetValue(null).ToString()),
                Text = Convert.ToByte(row.GetValue(null).ToString()) == 9 ? "Payment & Receipt" : row.Name
            }).ToList();
            List<long> PaymentReceiptConfigTypes = new List<long>();
            PaymentReceiptConfigTypes.Add(DbConstants.PaymentReceiptConfigType.Receipt);
            PaymentReceiptConfigTypes.Add(DbConstants.PaymentReceiptConfigType.Refund);
            PaymentReceiptConfigTypes.Add(DbConstants.PaymentReceiptConfigType.SalaryVoucher);
            model.PaymentConfigs = model.PaymentConfigs.Where(x => !PaymentReceiptConfigTypes.Contains(x.RowKey)).ToList();
        }

        public AccountHeadViewModel FillSearchAccountHeadType(AccountHeadViewModel model)
        {

            if (model.SearchAccountGroupKey != 0)
            {
                model.SearchAccountHeadType = dbContext.VwAccountHeadTypes.Where(x => x.AccountGroupKey == model.SearchAccountGroupKey).Select(row => new SelectListModel
                {
                    RowKey = row.RowKey,
                    Text = row.AccountHeadTypeName
                }).ToList();
            }
            else
            {
                model.SearchAccountHeadType = dbContext.VwAccountHeadTypes.Select(row => new SelectListModel
                {
                    RowKey = row.RowKey,
                    Text = row.AccountHeadTypeName
                }).ToList();
            }
            return model;
        }
    }

}
