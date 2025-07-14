using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Services
{
    public class BankService : IBankService
    {
        private EduSuiteDatabase dbContext;
        public BankService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<BankViewModel> GetBank(string searchText)
        {
            try
            {
                var BankList = (from p in dbContext.Banks
                                orderby p.RowKey descending
                                where (p.BankName.Contains(searchText))
                                select new BankViewModel
                                {
                                    RowKey = p.RowKey,
                                    BankName = p.BankName,
                                    DisplayOrder = p.DisplayOrder,
                                    IsActive = p.IsActive,
                                }).ToList();
                return BankList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<BankViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Bank, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<BankViewModel>();
            }
        }
        public BankViewModel GetBankById(int? id)
        {
            try
            {
                BankViewModel model = new BankViewModel();
                model = dbContext.Banks.Select(row => new BankViewModel
                {
                    RowKey = row.RowKey,
                    BankName = row.BankName,
                    DisplayOrder = row.DisplayOrder,
                    IsActive = row.IsActive
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new BankViewModel();
                }
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Bank, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new BankViewModel();
            }
        }
        public BankViewModel CreateBank(BankViewModel model)
        {
            var BankCheck = dbContext.Banks.Where(row => row.BankName.ToLower() == model.BankName.ToLower()).Count();
            Bank BankModel = new Bank();
            if (BankCheck != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Bank);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    int MaxKey = dbContext.Banks.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    BankModel.RowKey = Convert.ToInt16(MaxKey + 1);
                    BankModel.BankName = model.BankName;
                    BankModel.DisplayOrder = BankModel.RowKey;
                    BankModel.IsActive = model.IsActive;
                    dbContext.Banks.Add(BankModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.Bank, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Bank);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Bank, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public BankViewModel UpdateBank(BankViewModel model)
        {
            var BankCheck = dbContext.Banks.Where(row => row.BankName.ToLower() == model.BankName.ToLower()
               && row.RowKey != model.RowKey).ToList();

            Bank BankModel = new Bank();
            if (BankCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Bank);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    BankModel = dbContext.Banks.SingleOrDefault(x => x.RowKey == model.RowKey);
                    BankModel.BankName = model.BankName;
                    BankModel.IsActive = model.IsActive;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.Bank, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Bank);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Bank, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;

        }
        public BankViewModel DeleteBank(BankViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Bank BankModel = dbContext.Banks.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.Banks.Remove(BankModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Bank, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Bank);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.Bank, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Bank);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Bank, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }
    }
}
