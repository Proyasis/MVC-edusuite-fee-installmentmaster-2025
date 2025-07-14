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
   public class SalaryOtherAmountTypeService: ISalaryOtherAmountTypeService
    {
        private EduSuiteDatabase dbContext;
        public SalaryOtherAmountTypeService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<SalaryOtherAmountTypeViewModel> GetSalaryOtherAmountType(string searchText)
        {
            try
            {
                var SalaryOtherAmountTypeList = (from p in dbContext.SalaryOtherAmountTypes
                                orderby p.RowKey descending
                                where (p.OtherSalaryHeadName.Contains(searchText))
                                select new SalaryOtherAmountTypeViewModel
                                {
                                    RowKey = p.RowKey,
                                    OtherSalaryHeadName = p.OtherSalaryHeadName,
                                    IsAddition = p.IsAddition,
                                    IsActive = p.IsActive,
                                }).ToList();
                return SalaryOtherAmountTypeList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<SalaryOtherAmountTypeViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.SalaryOtherAmountType, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<SalaryOtherAmountTypeViewModel>();
            }
        }
        public SalaryOtherAmountTypeViewModel GetSalaryOtherAmountTypeById(int? id)
        {
            try
            {
                SalaryOtherAmountTypeViewModel model = new SalaryOtherAmountTypeViewModel();
                model = dbContext.SalaryOtherAmountTypes.Select(row => new SalaryOtherAmountTypeViewModel
                {
                    RowKey = row.RowKey,
                    OtherSalaryHeadName = row.OtherSalaryHeadName,
                    IsAddition = row.IsAddition,
                    IsActive = row.IsActive
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new SalaryOtherAmountTypeViewModel();
                }
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.SalaryOtherAmountType, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new SalaryOtherAmountTypeViewModel();
            }
        }
        public SalaryOtherAmountTypeViewModel CreateSalaryOtherAmountType(SalaryOtherAmountTypeViewModel model)
        {
            var SalaryOtherAmountTypeCheck = dbContext.SalaryOtherAmountTypes.Where(row => row.OtherSalaryHeadName.ToLower() == model.OtherSalaryHeadName.ToLower()).Count();
            SalaryOtherAmountType SalaryOtherAmountTypeModel = new SalaryOtherAmountType();
            if (SalaryOtherAmountTypeCheck != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.SalaryOtherAmountType);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    int MaxKey = dbContext.SalaryOtherAmountTypes.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    SalaryOtherAmountTypeModel.RowKey = Convert.ToInt16(MaxKey + 1);
                    SalaryOtherAmountTypeModel.OtherSalaryHeadName = model.OtherSalaryHeadName;
                    SalaryOtherAmountTypeModel.IsAddition = model.IsAddition;
                    SalaryOtherAmountTypeModel.IsActive = model.IsActive;
                    SalaryOtherAmountTypeModel.DisplayOrder = SalaryOtherAmountTypeModel.RowKey;
                    dbContext.SalaryOtherAmountTypes.Add(SalaryOtherAmountTypeModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.SalaryOtherAmountType, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.SalaryOtherAmountType);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.SalaryOtherAmountType, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public SalaryOtherAmountTypeViewModel UpdateSalaryOtherAmountType(SalaryOtherAmountTypeViewModel model)
        {
            var SalaryOtherAmountTypeCheck = dbContext.SalaryOtherAmountTypes.Where(row => row.OtherSalaryHeadName.ToLower() == model.OtherSalaryHeadName.ToLower()
               && row.RowKey != model.RowKey).ToList();

            SalaryOtherAmountType SalaryOtherAmountTypeModel = new SalaryOtherAmountType();
            if (SalaryOtherAmountTypeCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.SalaryOtherAmountType);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    SalaryOtherAmountTypeModel = dbContext.SalaryOtherAmountTypes.SingleOrDefault(x => x.RowKey == model.RowKey);
                    SalaryOtherAmountTypeModel.OtherSalaryHeadName = model.OtherSalaryHeadName;
                    SalaryOtherAmountTypeModel.IsAddition = model.IsAddition;
                    SalaryOtherAmountTypeModel.IsActive = model.IsActive;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.SalaryOtherAmountType, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.SalaryOtherAmountType);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.SalaryOtherAmountType, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;

        }
        public SalaryOtherAmountTypeViewModel DeleteSalaryOtherAmountType(SalaryOtherAmountTypeViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    SalaryOtherAmountType SalaryOtherAmountTypeModel = dbContext.SalaryOtherAmountTypes.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.SalaryOtherAmountTypes.Remove(SalaryOtherAmountTypeModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.SalaryOtherAmountType, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.SalaryOtherAmountType);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.SalaryOtherAmountType, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.SalaryOtherAmountType);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.SalaryOtherAmountType, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }
    }
}
