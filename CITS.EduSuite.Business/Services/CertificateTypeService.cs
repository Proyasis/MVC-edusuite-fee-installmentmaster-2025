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
    public class CertificateTypeService : ICertificateTypeService
    {
        private EduSuiteDatabase dbContext;
        public CertificateTypeService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public CertificateTypeViewModel GetCertificateTypeById(int? id)
        {
            try
            {
                CertificateTypeViewModel model = new CertificateTypeViewModel();
                model = dbContext.CertificateTypes.Select(row => new CertificateTypeViewModel
                {
                    RowKey = row.RowKey,
                    CertificateTypeName = row.CertificateTypeName,

                    IsActive = row.IsActive
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new CertificateTypeViewModel();
                }
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.CertificateType, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new CertificateTypeViewModel();

            }
        }
        public CertificateTypeViewModel CreateCertificateType(CertificateTypeViewModel model)
        {
            CertificateType CertificateTypeModel = new CertificateType();

            var CertificateTypeCheck = dbContext.CertificateTypes.Where(row => row.CertificateTypeName.ToLower() == model.CertificateTypeName.ToLower()).Count();

            if (CertificateTypeCheck != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.CertificateType);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    long MaxKey = dbContext.CertificateTypes.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    CertificateTypeModel.RowKey = Convert.ToInt16(MaxKey + 1);
                    CertificateTypeModel.CertificateTypeName = model.CertificateTypeName;
                    CertificateTypeModel.DateAdded = DateTimeUTC.Now;
                    CertificateTypeModel.IsActive = model.IsActive;
                    dbContext.CertificateTypes.Add(CertificateTypeModel);
                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.CertificateType, ActionConstants.Add, DbConstants.LogType.Info, CertificateTypeModel.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.CertificateType);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.CertificateType, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public CertificateTypeViewModel UpdateCertificateType(CertificateTypeViewModel model)
        {
            CertificateType CertificateTypeModel = new CertificateType();

            var CertificateTypeCheck = dbContext.CertificateTypes.Where(row => row.CertificateTypeName.ToLower() == model.CertificateTypeName.ToLower() && row.RowKey != model.RowKey).Count();

            if (CertificateTypeCheck != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.CertificateType);
                model.IsSuccessful = false;
                return model;
            }
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    CertificateTypeModel = dbContext.CertificateTypes.SingleOrDefault(x => x.RowKey == model.RowKey);
                    CertificateTypeModel.CertificateTypeName = model.CertificateTypeName;

                    CertificateTypeModel.IsActive = model.IsActive;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.CertificateType, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.CertificateType);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.CertificateType, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;

        }
        public CertificateTypeViewModel DeleteCertificateType(CertificateTypeViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    CertificateType CertificateTypeModel = dbContext.CertificateTypes.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.CertificateTypes.Remove(CertificateTypeModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.CertificateType, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);


                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.CertificateType);
                        model.IsSuccessful = false;

                        ActivityLog.CreateActivityLog(MenuConstants.CertificateType, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.CertificateType);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.CertificateType, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }
        public List<CertificateTypeViewModel> GetCertificateType(string searchText)
        {
            try
            {
                var CertificateTypeList = (from p in dbContext.CertificateTypes
                                           orderby p.RowKey descending
                                           where (p.CertificateTypeName.Contains(searchText))
                                           select new CertificateTypeViewModel
                                           {
                                               RowKey = p.RowKey,
                                               CertificateTypeName = p.CertificateTypeName,
                                               //IsActiveText = p.IsActive == true ? ApplicationResources.Yes : ApplicationResources.No
                                               IsActive = p.IsActive,
                                           }).ToList();
                return CertificateTypeList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<CertificateTypeViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.CertificateType, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<CertificateTypeViewModel>();

            }
        }

        public CertificateTypeViewModel CheckCertificateTypeNameExists(CertificateTypeViewModel model)
        {
            if (dbContext.CertificateTypes.Where(x => x.CertificateTypeName.ToLower() == model.CertificateTypeName.ToLower() && x.RowKey != model.RowKey).Any())
            {
                model.IsSuccessful = false;
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.CertificateType);
            }
            else
            {
                model.IsSuccessful = true;
                model.Message = "";
            }
            return model;
        }
    }
}
