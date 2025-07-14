using CITS.EduSuite.Business.Common;
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
    public class TransactionTypeService : ITransactionTypeService
    {
        private EduSuiteDatabase dbContext;

        public TransactionTypeService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<TransactionTypeViewModel> GetTransactionType(string searchText)
        {
            try
            {
                var transactionList = (from t in dbContext.TransactionTypes
                                       orderby t.TransactionTypeName
                                       where (t.TransactionTypeName.Contains(searchText))
                                       select new TransactionTypeViewModel
                                       {
                                           RowKey = t.RowKey,
                                           TransactionTypeName = t.TransactionTypeName,
                                           TransactionTypeNameLocal = t.TransactionTypeNameLocal,
                                           StatusKey = t.StatusKey,
                                           StatusName = t.Status.StatusName
                                       }).ToList();

                return transactionList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<TransactionTypeViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.TransactionType, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<TransactionTypeViewModel>();
               

            }
        }

        public TransactionTypeViewModel GetTransactionTypeById(Byte? id)
        {
            try
            {
                TransactionTypeViewModel model = new TransactionTypeViewModel();
                model = dbContext.TransactionTypes.Select(row => new TransactionTypeViewModel
                {
                    RowKey = row.RowKey,
                    TransactionTypeName = row.TransactionTypeName,
                    TransactionTypeNameLocal = row.TransactionTypeNameLocal,
                    StatusKey = row.StatusKey,
                    StatusName = row.Status.StatusName
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new TransactionTypeViewModel();
                }
                FillStatus(model);
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.TransactionType, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new TransactionTypeViewModel();
               

            }
        }

        public TransactionTypeViewModel CreateTransactionType(TransactionTypeViewModel model)
        {
            TransactionType TransactionTypeModel = new TransactionType();
            FillStatus(model);

            var TransactionTypeNameCheck = dbContext.TransactionTypes.Where(row => row.TransactionTypeName.ToLower().Trim() == model.TransactionTypeName.ToLower().Trim()).ToList();
            if (TransactionTypeNameCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.TransactionType);
                model.IsSuccessful = false;
                return model;
            }
            else
            {
                var TransactionTypeShortNameExistCheck = dbContext.TransactionTypes.Where(row => row.TransactionTypeNameLocal.ToLower().Trim() == model.TransactionTypeNameLocal.ToLower().Trim()).ToList();
                if (TransactionTypeShortNameExistCheck.Count != 0)
                {
                    model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.TransactionType);
                    model.IsSuccessful = false;
                    return model;
                }
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    byte maxKey = dbContext.TransactionTypes.Select(l => l.RowKey).DefaultIfEmpty().Max();
                    TransactionTypeModel.RowKey = Convert.ToByte(maxKey + 1);
                    TransactionTypeModel.TransactionTypeName = model.TransactionTypeName.Trim();
                    TransactionTypeModel.TransactionTypeNameLocal = model.TransactionTypeNameLocal.Trim();
                    TransactionTypeModel.StatusKey = model.StatusKey;
                    dbContext.TransactionTypes.Add(TransactionTypeModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.TransactionType, ActionConstants.Add, DbConstants.LogType.Info, TransactionTypeModel.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.TransactionType);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.TransactionType, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public TransactionTypeViewModel UpdateTransactionType(TransactionTypeViewModel model)
        {
            TransactionType TransactionTypeModel = new TransactionType();
            FillStatus(model);

            var TransactionTypeNameCheck = dbContext.TransactionTypes.Where(row => row.TransactionTypeName.ToLower().Trim() == model.TransactionTypeName.ToLower().Trim() && row.RowKey != model.RowKey).ToList();
            if (TransactionTypeNameCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.TransactionType);
                model.IsSuccessful = false;
                return model;
            }
            else
            {
                var TransactionTypeShortNameExistCheck = dbContext.TransactionTypes.Where(row => row.TransactionTypeNameLocal.ToLower().Trim() == model.TransactionTypeNameLocal.ToLower().Trim() && row.RowKey != model.RowKey).ToList();
                if (TransactionTypeShortNameExistCheck.Count != 0)
                {
                    model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.TransactionType);
                    model.IsSuccessful = false;
                    return model;
                }
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    TransactionTypeModel = dbContext.TransactionTypes.SingleOrDefault(row => row.RowKey == model.RowKey);
                    TransactionTypeModel.TransactionTypeName = model.TransactionTypeName.Trim();
                    TransactionTypeModel.TransactionTypeNameLocal = model.TransactionTypeNameLocal.Trim();
                    TransactionTypeModel.StatusKey = model.StatusKey;

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                     ActivityLog.CreateActivityLog(MenuConstants.TransactionType, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

  
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.TransactionType);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.TransactionType, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public TransactionTypeViewModel DeleteTransactionType(TransactionTypeViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    TransactionType TransactionType = dbContext.TransactionTypes.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.TransactionTypes.Remove(TransactionType);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.TransactionType, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey,model.Message);


 
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.TransactionType);
                        model.IsSuccessful = false;
                          ActivityLog.CreateActivityLog(MenuConstants.TransactionType, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
            }
            return model;
        }

        private void FillStatus(TransactionTypeViewModel model)
        {
            model.Statuses = dbContext.Status.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.StatusName
            }).ToList();
        }
    }

}

