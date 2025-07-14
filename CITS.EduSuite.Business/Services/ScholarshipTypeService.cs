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
    public class ScholarshipTypeService : IScholarshipTypeService
    {
        private EduSuiteDatabase dbContext;

        public ScholarshipTypeService(EduSuiteDatabase objdb)
        {
            this.dbContext = objdb;
        }
        public ScholarshipTypeViewModel GetScholarshipTypeById(int? id)
        {
            try
            {
                ScholarshipTypeViewModel model = new ScholarshipTypeViewModel();
                model = dbContext.ScholarshipTypes.Select(row => new ScholarshipTypeViewModel
                {
                    RowKey = row.RowKey,
                    ScholarShipTypeName = row.ScholarshipTypeName,
                    ScholarShipTypeCode = row.ScholarshipTypeCode,
                    IsActive = row.IsActive
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new ScholarshipTypeViewModel();
                }
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.ScholarshipType, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new ScholarshipTypeViewModel();

            }
        }
        public ScholarshipTypeViewModel CreateScholarshipType(ScholarshipTypeViewModel model)
        {
            ScholarshipType ScholarshipTypeModel = new ScholarshipType();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    long MaxKey = dbContext.ScholarshipTypes.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    ScholarshipTypeModel.RowKey = Convert.ToInt16(MaxKey + 1);
                    ScholarshipTypeModel.ScholarshipTypeName = model.ScholarShipTypeName;
                    ScholarshipTypeModel.ScholarshipTypeCode = model.ScholarShipTypeCode;
                    ScholarshipTypeModel.IsActive = model.IsActive;
                    dbContext.ScholarshipTypes.Add(ScholarshipTypeModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.ScholarshipType, ActionConstants.Add, DbConstants.LogType.Info, ScholarshipTypeModel.RowKey, model.Message);


                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Scholarship+ EduSuiteUIResources.BlankSpace+ EduSuiteUIResources.Type);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.ScholarshipType, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public ScholarshipTypeViewModel UpdateScholarshipType(ScholarshipTypeViewModel model)
        {
            ScholarshipType ScholarshipTypeModel = new ScholarshipType();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    ScholarshipTypeModel = dbContext.ScholarshipTypes.SingleOrDefault(x => x.RowKey == model.RowKey);
                    ScholarshipTypeModel.ScholarshipTypeName = model.ScholarShipTypeName;
                    ScholarshipTypeModel.ScholarshipTypeCode = model.ScholarShipTypeCode;
                    ScholarshipTypeModel.IsActive = model.IsActive;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.ScholarshipType, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Scholarship + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Type);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.ScholarshipType, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;

        }
        public ScholarshipTypeViewModel DeleteScholarshipType(ScholarshipTypeViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    ScholarshipType ScholarshipTypeModel = dbContext.ScholarshipTypes.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.ScholarshipTypes.Remove(ScholarshipTypeModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.ScholarshipType, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);


                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Scholarship + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Type);
                        model.IsSuccessful = false;

                        ActivityLog.CreateActivityLog(MenuConstants.ScholarshipType, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Scholarship + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Type);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.ScholarshipType, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }
        public List<ScholarshipTypeViewModel> GetScholarshipType(string searchText)
        {
            try
            {
                var ScholarshipTypeList = (from p in dbContext.ScholarshipTypes
                                           orderby p.RowKey
                                           where (p.ScholarshipTypeName.Contains(searchText))
                                           select new ScholarshipTypeViewModel
                                           {
                                               RowKey = p.RowKey,
                                               ScholarShipTypeName = p.ScholarshipTypeName,
                                               ScholarShipTypeCode = p.ScholarshipTypeCode,
                                               IsActiveText = p.IsActive == true ? EduSuiteUIResources.Yes : EduSuiteUIResources.No,
                                               IsActive = p.IsActive,
                                           }).ToList();
                return ScholarshipTypeList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<ScholarshipTypeViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.ScholarshipType, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<ScholarshipTypeViewModel>();

            }
        }
        public ScholarshipTypeViewModel CheckScholarshipTypeCodeExists(ScholarshipTypeViewModel model)
        {
            if (dbContext.ScholarshipTypes.Where(x => x.ScholarshipTypeCode.ToLower() == model.ScholarShipTypeCode.ToLower() && x.RowKey != model.RowKey).Any())
            {
                model.IsSuccessful = false;
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Scholarship + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Type);
            }
            else
            {
                model.IsSuccessful = true;
                model.Message = "";
            }
            return model;
        }
        public ScholarshipTypeViewModel CheckScholarshipTypeNameExists(ScholarshipTypeViewModel model)
        {
            if (dbContext.ScholarshipTypes.Where(x => x.ScholarshipTypeName.ToLower() == model.ScholarShipTypeName.ToLower() && x.RowKey != model.RowKey).Any())
            {
                model.IsSuccessful = false;
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Scholarship + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Type);
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
