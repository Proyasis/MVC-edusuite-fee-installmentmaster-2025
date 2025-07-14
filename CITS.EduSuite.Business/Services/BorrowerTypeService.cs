using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
namespace CITS.EduSuite.Business.Services
{
    public class BorrowerTypeService : IBorrowerTypeService
    {

        private EduSuiteDatabase dbContext;
        public BorrowerTypeService(EduSuiteDatabase objDB)
        {
            dbContext = objDB;
        }
        public List<BorrowerTypeViewModel> GetBorrowerTypes(string searchText)
        {
            try
            {
                var borrowerTypeList = (from bt in dbContext.BorrowerTypes
                                        orderby bt.RowKey descending
                                        where bt.BorrowerTypeName.Contains(searchText)
                                        select new BorrowerTypeViewModel
                                         {
                                             RowKey = bt.RowKey,
                                             BorrowerTypeName = bt.BorrowerTypeName,
                                            BorrowerTypeCode = bt.BorrowerTypeCode,
                                            NoOfBookIssueAtATime = bt.NoOfBookIssueAtATime,
                                             IsActive = bt.IsActive ?? false

                                         }).ToList();
                return borrowerTypeList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<BorrowerTypeViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.BorrowerType, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<BorrowerTypeViewModel>();
                
            }
        }

        public BorrowerTypeViewModel GetBorrowerTypeById(byte? id)
        {
            BorrowerTypeViewModel model = new BorrowerTypeViewModel();
            model = dbContext.BorrowerTypes.Select(row => new BorrowerTypeViewModel
            {
                RowKey = row.RowKey,
                BorrowerTypeName = row.BorrowerTypeName,
                BorrowerTypeCode = row.BorrowerTypeCode,
                NoOfBookIssueAtATime = row.NoOfBookIssueAtATime,
                IsActive = row.IsActive ?? false

            }).Where(x => x.RowKey == id).FirstOrDefault();
            if (model == null)
            {
                model = new BorrowerTypeViewModel();
            }
            return model;
        }

        public BorrowerTypeViewModel CreateBorrowerType(BorrowerTypeViewModel model)
        {
            var borrowerTypeNameCheck = dbContext.BorrowerTypes.Where(row => row.BorrowerTypeName.ToLower() == model.BorrowerTypeName.ToLower()).ToList();
            BorrowerType borrowerTypeModel = new BorrowerType();
            if (borrowerTypeNameCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.BorrowerTypeName);
                model.IsSuccessful = false;
                ActivityLog.CreateActivityLog(MenuConstants.BorrowerType, ActionConstants.View, DbConstants.LogType.Error, null, model.Message);
                return model;
                

            }
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Byte maxKey = dbContext.BorrowerTypes.Select(a => a.RowKey).DefaultIfEmpty().Max();
                    borrowerTypeModel.RowKey = Convert.ToByte(maxKey + 1);
                    borrowerTypeModel.BorrowerTypeName = model.BorrowerTypeName;
                    borrowerTypeModel.BorrowerTypeCode = model.BorrowerTypeCode;
                    borrowerTypeModel.NoOfBookIssueAtATime = model.NoOfBookIssueAtATime;
                    borrowerTypeModel.IsActive = model.IsActive;
                    dbContext.BorrowerTypes.Add(borrowerTypeModel);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.BorrowerType, ActionConstants.Add, DbConstants.LogType.Info, borrowerTypeModel.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.BorrowerType);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.BorrowerType, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public BorrowerTypeViewModel UpdateBorrowerType(BorrowerTypeViewModel model)
        {
            var borrowerTypeNameCheck = dbContext.BorrowerTypes.Where(row => row.RowKey != model.RowKey && row.BorrowerTypeName.ToLower() == model.BorrowerTypeName.ToLower()).ToList();
            BorrowerType borrowerTypeModel = new BorrowerType();
            if (borrowerTypeNameCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.BorrowerTypeName);
                model.IsSuccessful = false;
                return model;
            }
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    borrowerTypeModel = dbContext.BorrowerTypes.SingleOrDefault(row => row.RowKey == model.RowKey);
                    borrowerTypeModel.BorrowerTypeName = model.BorrowerTypeName;
                    borrowerTypeModel.BorrowerTypeCode = model.BorrowerTypeCode;
                    borrowerTypeModel.NoOfBookIssueAtATime = model.NoOfBookIssueAtATime;
                    borrowerTypeModel.IsActive = model.IsActive;

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.BorrowerType, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.BorrowerType);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.BorrowerType, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public BorrowerTypeViewModel DeleteBorrowerType(BorrowerTypeViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    BorrowerType borrowerType = dbContext.BorrowerTypes.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.BorrowerTypes.Remove(borrowerType);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.BorrowerType, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.BorrowerType);
                        model.IsSuccessful = false;

                        ActivityLog.CreateActivityLog(MenuConstants.BorrowerType, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.BorrowerType);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.BorrowerType, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public BorrowerTypeViewModel CheckBorrowerTypeCodeExist(BorrowerTypeViewModel model)
        {
           
            model.IsSuccessful = !dbContext.BorrowerTypes.Where(x => x.BorrowerTypeCode.ToLower() == model.BorrowerTypeCode.ToLower() && x.RowKey != model.RowKey).Any();

            return model;
        }
    }
}
