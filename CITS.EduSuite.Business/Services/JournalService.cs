using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System.Data.Entity.Infrastructure;
using CITS.EduSuite.Business.Models.Resources;
using System.Linq.Expressions;

namespace CITS.EduSuite.Business.Services
{
    public class JournalService : IJournalService
    {
        private EduSuiteDatabase dbContext;
        public JournalService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public JournalViewModel GetJournalById(JournalViewModel model)
        {
            try
            {
                JournalViewModel Objviewmodel = new JournalViewModel();
                Objviewmodel = dbContext.JournalMasters.Where(x => x.RowKey == model.RowKey).Select(row => new JournalViewModel
                {
                    BranchKey = row.BranchKey,
                    RowKey = row.RowKey,
                    Remark = row.Remarks,
                    JournalDate = row.JournalDate
                }).FirstOrDefault();
                if (Objviewmodel == null)
                {
                    Objviewmodel = new JournalViewModel();
                }


                JournalDetails(Objviewmodel);
                FillBranches(Objviewmodel);
                return Objviewmodel;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Journal, ActionConstants.AddEdit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                return model;
            }
        }
        private void JournalDetails(JournalViewModel model)
        {
            model.JournalDetails = (from jd in dbContext.JournalDetails
                                    where jd.MasterKey == model.RowKey
                                    select new JournalDetailsViewModel
                                    {
                                        RowKey = jd.RowKey,
                                        AccountHeadKey = jd.AccountHeadKey,
                                        Debit = jd.Debit,
                                        Credit = jd.Credit,
                                        MasterKey = jd.MasterKey,
                                        Remark = jd.Remark,
                                        AccountGroupKey = jd.AccountHead.AccountHeadType.AccountGroupKey
                                    }).ToList();
            if (model.JournalDetails.Count == 0)
            {
                model.JournalDetails.Add(new JournalDetailsViewModel
                {
                    RowKey = 0,
                    MasterKey = 0
                });
            }
            foreach (JournalDetailsViewModel modelitem in model.JournalDetails)
            {
                FillAccountGroup(modelitem);
                FillAcountHead(modelitem);
            }

        }

        public JournalViewModel CreateJournal(JournalViewModel model)
        {
            FillBranches(model);
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    JournalMaster Journal = new JournalMaster();
                    long MaxKey = dbContext.JournalMasters.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    Journal.RowKey = MaxKey + 1;
                    model.RowKey = Journal.RowKey;
                    Journal.BranchKey = model.BranchKey;
                    Journal.JournalDate = model.JournalDate;
                    Journal.Remarks = model.Remark;
                    dbContext.JournalMasters.Add(Journal);
                    createJournalDetails(model.JournalDetails.Where(x => x.RowKey == 0).ToList(), model, model.RowKey);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Journal, ActionConstants.AddEdit, DbConstants.LogType.Info, null, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = ex.GetBaseException().Message;
                    ActivityLog.CreateActivityLog(MenuConstants.Journal, ActionConstants.AddEdit, DbConstants.LogType.Info, null, ex.GetBaseException().Message);
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Journal);
                    model.IsSuccessful = false;
                }
            }
            return model;
        }

        public JournalViewModel UpdateJournal(JournalViewModel model)
        {
            FillBranches(model);
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    JournalMaster Journal = new JournalMaster();
                    Journal = dbContext.JournalMasters.SingleOrDefault(p => p.RowKey == model.RowKey);
                    Journal.BranchKey = model.BranchKey;
                    Journal.JournalDate = model.JournalDate;
                    Journal.Remarks = model.Remark;

                    createJournalDetails(model.JournalDetails.Where(x => x.RowKey == 0).ToList(), model, model.RowKey);
                    updateJournalDetails(model.JournalDetails.Where(x => x.RowKey != 0).ToList(), model);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Journal, ActionConstants.AddEdit, DbConstants.LogType.Info, null, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = ex.GetBaseException().Message;
                    ActivityLog.CreateActivityLog(MenuConstants.Journal, ActionConstants.AddEdit, DbConstants.LogType.Info, null, ex.GetBaseException().Message);
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Journal);
                    model.IsSuccessful = false;

                }
            }
            return model;
        }
        private void createJournalDetails(List<JournalDetailsViewModel> model, JournalViewModel masterModel, long MasterKey)
        {
            List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
            long MaxKey = dbContext.JournalDetails.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (JournalDetailsViewModel modelItem in model)
            {
                JournalDetail JournalModel = new JournalDetail();
                JournalModel.RowKey = Convert.ToInt16(MaxKey + 1);
                JournalModel.MasterKey = MasterKey;
                JournalModel.Debit = modelItem.Debit;
                JournalModel.AccountHeadKey = modelItem.AccountHeadKey;
                JournalModel.Remark = modelItem.Remark;
                JournalModel.Credit = modelItem.Credit;
                JournalModel.AccountHead = dbContext.AccountHeads.SingleOrDefault(x => x.RowKey == modelItem.AccountHeadKey);
                dbContext.JournalDetails.Add(JournalModel);
                accountFlowModelList = DebitAmountList(JournalModel, masterModel, accountFlowModelList, false);
                MaxKey++;
            }
            CreateAccountFlow(accountFlowModelList, false);
        }
        private void updateJournalDetails(List<JournalDetailsViewModel> model, JournalViewModel masterModel)
        {
            List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
            foreach (JournalDetailsViewModel modelItem in model)
            {
                JournalDetail JournalModel = new JournalDetail();
                JournalModel = dbContext.JournalDetails.SingleOrDefault(x => x.RowKey == modelItem.RowKey);
                JournalModel.Credit = modelItem.Credit;
                JournalModel.AccountHeadKey = modelItem.AccountHeadKey;
                JournalModel.Remark = modelItem.Remark;
                JournalModel.Debit = modelItem.Debit;
                JournalModel.AccountHead = dbContext.AccountHeads.SingleOrDefault(x => x.RowKey == modelItem.AccountHeadKey);
                accountFlowModelList = DebitAmountList(JournalModel, masterModel, accountFlowModelList, true);
            }
            CreateAccountFlow(accountFlowModelList, true);
        }
        public JournalViewModel DeleteJournal(JournalViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    JournalMaster JournalMaster = dbContext.JournalMasters.SingleOrDefault(row => row.RowKey == model.RowKey);
                    List<JournalDetail> JournalDetail = dbContext.JournalDetails.Where(row => row.MasterKey == model.RowKey).ToList();
                    List<long> Keys = JournalDetail.Select(x => Convert.ToInt64(x.RowKey)).ToList();
                    List<AccountFlow> AccountList = dbContext.AccountFlows.Where(x => x.TransactionTypeKey == DbConstants.TransactionType.Journal && Keys.Contains(x.TransactionKey)).ToList();
                    dbContext.AccountFlows.RemoveRange(AccountList);
                    dbContext.JournalDetails.RemoveRange(JournalDetail);
                    dbContext.JournalMasters.Remove(JournalMaster);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Journal, ActionConstants.Delete, DbConstants.LogType.Info, null, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = ex.GetBaseException().Message;
                        ActivityLog.CreateActivityLog(MenuConstants.Journal, ActionConstants.Delete, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Journal);
                        model.IsSuccessful = false;
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Journal);
                    model.IsSuccessful = false;
                }
            }

            return model;
        }
        public JournalViewModel DeleteJournalItem(JournalViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    JournalDetail JournalDetail = dbContext.JournalDetails.SingleOrDefault(row => row.RowKey == model.RowKey);
                    JournalMaster journalnaster = dbContext.JournalMasters.SingleOrDefault(x => x.RowKey == JournalDetail.MasterKey);

                    List<AccountFlow> AccountList = dbContext.AccountFlows.Where(x => x.TransactionTypeKey == DbConstants.TransactionType.Journal && x.TransactionKey == model.RowKey).ToList();
                    dbContext.AccountFlows.RemoveRange(AccountList);
                    dbContext.JournalDetails.Remove(JournalDetail);
                    var IsJournalDetail = dbContext.JournalDetails.Any(row => row.MasterKey == JournalDetail.MasterKey);
                    if (IsJournalDetail == false)
                    {

                        dbContext.JournalMasters.Remove(journalnaster);
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

                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Journal);
                        model.IsSuccessful = false;
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Journal);
                    model.IsSuccessful = false;
                }
            }

            return model;
        }
        public List<JournalViewModel> GetJournal(JournalViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;
                IQueryable<JournalViewModel> JournalList = (from jm in dbContext.JournalMasters
                                                            select new JournalViewModel
                                                            {
                                                                RowKey = jm.RowKey,
                                                                BranchKey = jm.BranchKey,
                                                                JournalDate = jm.JournalDate,
                                                                Remark = jm.Remarks,
                                                                BranchName = jm.Branch.BranchName,
                                                                TotalCreditAmount = jm.JournalDetails.Select(x => x.Credit).Sum(),
                                                                TotalDebitAmount = jm.JournalDetails.Select(x => x.Debit).Sum(),
                                                            });
                if (model.BranchKey != 0)
                {
                    JournalList = JournalList.Where(x => x.BranchKey == model.BranchKey);
                }
                if (model.SortBy != "")
                {
                    JournalList = SortApplications(JournalList, model.SortBy, model.SortOrder);
                }
                TotalRecords = JournalList.Count();
                return JournalList.Skip(Skip).Take(Take).ToList<JournalViewModel>();
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.Journal, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<JournalViewModel> { new JournalViewModel { Message = ex.GetBaseException().Message } };
            }
        }

        private IQueryable<JournalViewModel> SortApplications(IQueryable<JournalViewModel> Query, string SortName, string SortOrder)
        {
            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(JournalViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<JournalViewModel>(resultExpression);
        }
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
        private List<AccountFlowViewModel> DebitAmountList(JournalDetail model, JournalViewModel MasterModel, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate)
        {
            long accountHeadKey;
            long ExtraUpdateKey = 0;
            accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == model.AccountHeadKey).Select(x => x.RowKey).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = (model.Debit ?? 0) != 0 ? DbConstants.CashFlowType.Out : DbConstants.CashFlowType.In,
                AccountHeadKey = accountHeadKey,
                Amount = (model.Debit ?? 0) != 0 ? (model.Debit ?? 0) : (model.Credit ?? 0),
                TransactionTypeKey = DbConstants.TransactionType.Journal,
                VoucherTypeKey = DbConstants.VoucherType.Journal,
                TransactionDate = MasterModel.JournalDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                Purpose = model.AccountHead.AccountHeadName + EduSuiteUIResources.BlankSpace + MasterModel.Remark + EduSuiteUIResources.BlankSpace + model.Remark,
                BranchKey = MasterModel.BranchKey,
                TransactionKey = model.RowKey,
            });
            return accountFlowModelList;
        }
        public JournalViewModel FillBranches(JournalViewModel model)
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

        private void FillAccountGroup(JournalDetailsViewModel model)
        {
            model.AccountGroups = dbContext.AccountGroups.Where(x => x.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AccountGroupName
            }).ToList();
        }
        public JournalDetailsViewModel FillAcountHead(JournalDetailsViewModel model)
        {
            model.AccountHeads = (from ac in dbContext.AccountHeads.Where(x => x.IsActive && x.AccountHeadType.AccountGroupKey == model.AccountGroupKey)

                                  where (ac.RowKey != DbConstants.AccountHead.CashSale)
                                  select new SelectListModel
                                  {
                                      RowKey = ac.RowKey,
                                      Text = ac.AccountHeadName
                                  }).ToList();
            return model;
        }

    }
}
