using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Data;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using System.Data.Entity.Infrastructure;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class BatchService : IBatchService
    {
        private EduSuiteDatabase dbcontext;

        public BatchService(EduSuiteDatabase objdb)
        {
            this.dbcontext = objdb;
        }

        public BatchViewModel GetBatchById(short? id)
        {
            try
            {
                BatchViewModel model = new BatchViewModel();
                model = dbcontext.Batches.Select(row => new BatchViewModel
                {
                    Rowkey = row.RowKey,
                    BatchName = row.BatchName,
                    BatchCode = row.BatchCode,
                    DurationFromYear = row.DurationFromYear,
                    DurationToYear = row.DurationToYear,
                    IsActive = row.IsActive,
                    DurationFromDate = row.DurationFromDate,
                    DurationToDate = row.DurationToDate,
                }).Where(x => x.Rowkey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new BatchViewModel();
                }
                return model;
            }
            catch (Exception Ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Batch, ActionConstants.View, DbConstants.LogType.Error, id, Ex.GetBaseException().Message);
                return new BatchViewModel();
                
            }
        }

        public BatchViewModel CreateBatch(BatchViewModel model)
        {
            var BatchNameCheck = dbcontext.Batches.Where(x => x.BatchName.ToLower() == model.BatchName.ToLower()).Count();
            if (BatchNameCheck > 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Batch);
                model.IsSuccessful = false;
                return model;
            }
            using (var transaction = dbcontext.Database.BeginTransaction())
            {
                try
                {
                    Batch BatchViewModel = new Batch();
                    int Maxkey = dbcontext.Batches.Select(x => x.RowKey).DefaultIfEmpty().Max();
                    BatchViewModel.RowKey = Convert.ToInt16(Maxkey + 1);
                    BatchViewModel.BatchName = model.BatchName;
                    BatchViewModel.BatchCode = model.BatchCode;
                    BatchViewModel.DurationFromYear = model.DurationFromYear;
                    BatchViewModel.DurationToYear = model.DurationToYear;
                    BatchViewModel.DurationFromDate = model.DurationFromDate;
                    BatchViewModel.DurationToDate = model.DurationToDate;
                    BatchViewModel.IsActive = model.IsActive;
                    dbcontext.Batches.Add(BatchViewModel);
                    dbcontext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.Batch, ActionConstants.Add, DbConstants.LogType.Info, BatchViewModel.RowKey, model.Message);
                }
                catch (Exception Ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Batch);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Batch, ActionConstants.Add, DbConstants.LogType.Error, null, Ex.GetBaseException().Message);
                }
            }
            return model;

        }

        public BatchViewModel UpdateBatch(BatchViewModel model)
        {
            var BatchNameCheck = dbcontext.Batches.Where(x => x.BatchName.ToLower() == model.BatchName.ToLower() && x.RowKey != model.Rowkey).ToList();
            if (BatchNameCheck.Count > 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Batch);
                model.IsSuccessful = false;
                return model;
            }
            using (var transaction = dbcontext.Database.BeginTransaction())
            {
                try
                {
                    Batch BatchViewModel = new Batch();

                    BatchViewModel = dbcontext.Batches.SingleOrDefault(x => x.RowKey == model.Rowkey);
                    BatchViewModel.BatchName = model.BatchName;
                    BatchViewModel.BatchCode = model.BatchCode;
                    BatchViewModel.DurationFromYear = model.DurationFromYear;
                    BatchViewModel.DurationToYear = model.DurationToYear;
                    BatchViewModel.DurationFromDate = model.DurationFromDate;
                    BatchViewModel.DurationToDate = model.DurationToDate;
                    BatchViewModel.IsActive = model.IsActive;
                    dbcontext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Batch, ActionConstants.Edit, DbConstants.LogType.Info, model.Rowkey, model.Message);
                }
                catch (Exception Ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Batch);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.Batch, ActionConstants.Edit, DbConstants.LogType.Error, model.Rowkey, Ex.GetBaseException().Message);
                }
            }
            return model;

        }
        public BatchViewModel DeleteBatch(BatchViewModel model)
        {
            using (var transaction = dbcontext.Database.BeginTransaction())
            {
                try
                {
                    Batch BatchModel = dbcontext.Batches.SingleOrDefault(row => row.RowKey == model.Rowkey);
                    dbcontext.Batches.Remove(BatchModel);
                    dbcontext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Batch, ActionConstants.Delete, DbConstants.LogType.Info, model.Rowkey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Batch);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.Batch, ActionConstants.Delete, DbConstants.LogType.Debug, model.Rowkey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Batch);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Batch, ActionConstants.Delete, DbConstants.LogType.Debug, model.Rowkey, ex.GetBaseException().Message);
                }
            }

            return model;
        }
        public List<BatchViewModel> GetBatch(string searchText)
        {
            try
            {
                var BatchList = (from B in dbcontext.Batches
                                 orderby B.RowKey descending
                                 where (B.BatchName.Contains(searchText) || B.BatchCode.Contains(searchText))
                                 select new BatchViewModel
                                 {
                                     Rowkey = B.RowKey,
                                     BatchName = B.BatchName,
                                     BatchCode = B.BatchCode,
                                     DurationFromYear = B.DurationFromYear,
                                     DurationToYear = B.DurationToYear,
                                     IsActive = B.IsActive,
                                 }).ToList();
                return BatchList.GroupBy(x => x.Rowkey).Select(y => y.First()).ToList<BatchViewModel>();
            }
            catch (Exception Ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Batch, ActionConstants.View, DbConstants.LogType.Error, null, Ex.GetBaseException().Message);
                return new List<BatchViewModel>();
                
            }
        }

        public BatchViewModel CheckBatchCodeExist(string BatchCode, short Rowkey)
        {
            BatchViewModel model = new BatchViewModel();
            if (dbcontext.Batches.Where(x => x.BatchCode.ToLower() == BatchCode.ToLower() && x.RowKey != Rowkey).Any())
            {
                model.IsSuccessful = false;
            }
            else
            {
                model.IsSuccessful = true;
            }
            return model;
        }
    }
}
