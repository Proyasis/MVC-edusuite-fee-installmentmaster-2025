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
   public class AcademicTermService: IAcademicTermService
    {
        private EduSuiteDatabase dbContext;
        public AcademicTermService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<AcademicTermViewModel> GetAcademicTerm(string searchText)
        {
            try
            {
                var AcademicTermList = (from p in dbContext.AcademicTerms
                                orderby p.RowKey descending
                                where (p.AcademicTermName.Contains(searchText))
                                select new AcademicTermViewModel
                                {
                                    RowKey = p.RowKey,
                                    AcademicTermName = p.AcademicTermName,
                                    Duration = p.Duration,
                                    IsActive = p.IsActive,
                                }).ToList();
                return AcademicTermList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<AcademicTermViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.AcademicTerm, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<AcademicTermViewModel>();
            }
        }
        public AcademicTermViewModel GetAcademicTermById(int? id)
        {
            try
            {
                AcademicTermViewModel model = new AcademicTermViewModel();
                model = dbContext.AcademicTerms.Select(row => new AcademicTermViewModel
                {
                    RowKey = row.RowKey,
                    AcademicTermName = row.AcademicTermName,
                    Duration = row.Duration,
                    IsActive = row.IsActive
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new AcademicTermViewModel();
                }
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.AcademicTerm, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new AcademicTermViewModel();
            }
        }
        public AcademicTermViewModel CreateAcademicTerm(AcademicTermViewModel model)
        {
            var AcademicTermCheck = dbContext.AcademicTerms.Where(row => row.AcademicTermName.ToLower() == model.AcademicTermName.ToLower()).Count();
            AcademicTerm AcademicTermModel = new AcademicTerm();
            if (AcademicTermCheck != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.AcademicTerm);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    int MaxKey = dbContext.AcademicTerms.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    AcademicTermModel.RowKey = Convert.ToInt16(MaxKey + 1);
                    AcademicTermModel.AcademicTermName = model.AcademicTermName;
                    AcademicTermModel.Duration = model.Duration;
                    AcademicTermModel.IsActive = model.IsActive;
                    dbContext.AcademicTerms.Add(AcademicTermModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.AcademicTerm, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.AcademicTerm);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.AcademicTerm, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public AcademicTermViewModel UpdateAcademicTerm(AcademicTermViewModel model)
        {
            var AcademicTermCheck = dbContext.AcademicTerms.Where(row => row.AcademicTermName.ToLower() == model.AcademicTermName.ToLower()
               && row.RowKey != model.RowKey).ToList();

            AcademicTerm AcademicTermModel = new AcademicTerm();
            if (AcademicTermCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.AcademicTerm);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    AcademicTermModel = dbContext.AcademicTerms.SingleOrDefault(x => x.RowKey == model.RowKey);
                    AcademicTermModel.AcademicTermName = model.AcademicTermName;
                    AcademicTermModel.Duration = model.Duration;
                    AcademicTermModel.IsActive = model.IsActive;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.AcademicTerm, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.AcademicTerm);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.AcademicTerm, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;

        }
        public AcademicTermViewModel DeleteAcademicTerm(AcademicTermViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    AcademicTerm AcademicTermModel = dbContext.AcademicTerms.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.AcademicTerms.Remove(AcademicTermModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.AcademicTerm, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.AcademicTerm);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.AcademicTerm, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.AcademicTerm);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.AcademicTerm, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }
    }
}
