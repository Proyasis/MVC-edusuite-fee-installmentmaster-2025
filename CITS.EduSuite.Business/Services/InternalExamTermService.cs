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
    public class InternalExamTermService : IInternalExamTermService
    {
        private EduSuiteDatabase dbContext;
        public InternalExamTermService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public List<InternalExamTermViewModel> GetInternalExamTerm(string searchText)
        {
            try
            {
                var internalExamTermList = (from p in dbContext.InternalExamTerms
                                            orderby p.RowKey descending
                                            where (p.InternalExamTermName.Contains(searchText))
                                            select new InternalExamTermViewModel
                                            {
                                                RowKey = p.RowKey,
                                                InternalExamTermName = p.InternalExamTermName,
                                                IsActive = p.IsActive,
                                            }).ToList();
                return internalExamTermList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<InternalExamTermViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.InternalExamTerm, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<InternalExamTermViewModel>();
            }
        }
        public InternalExamTermViewModel GetInternalExamTermById(int? id)
        {
            try
            {
                InternalExamTermViewModel model = new InternalExamTermViewModel();
                model = dbContext.InternalExamTerms.Select(row => new InternalExamTermViewModel
                {
                    RowKey = row.RowKey,
                    InternalExamTermName = row.InternalExamTermName,
                    IsActive = row.IsActive
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new InternalExamTermViewModel();
                }
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.InternalExamTerm, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new InternalExamTermViewModel();


            }
        }
        public InternalExamTermViewModel CreateInternalExamTerm(InternalExamTermViewModel model)
        {
            var InternalExamTermCheck = dbContext.InternalExamTerms.Where(row => row.InternalExamTermName.ToLower() == model.InternalExamTermName.ToLower()).Count();
            InternalExamTerm InternalExamTermModel = new InternalExamTerm();
            if (InternalExamTermCheck != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.InternalExamTerm);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    int MaxKey = dbContext.InternalExamTerms.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    InternalExamTermModel.RowKey = Convert.ToInt16(MaxKey + 1);
                    InternalExamTermModel.InternalExamTermName = model.InternalExamTermName;
                    InternalExamTermModel.IsActive = model.IsActive;
                    dbContext.InternalExamTerms.Add(InternalExamTermModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.InternalExamTerm, ActionConstants.Add, DbConstants.LogType.Info, InternalExamTermModel.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.InternalExamTerm);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.InternalExamTerm, ActionConstants.Add, DbConstants.LogType.Error, InternalExamTermModel.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public InternalExamTermViewModel UpdateInternalExamTerm(InternalExamTermViewModel model)
        {
            var InternalExamTermCheck = dbContext.InternalExamTerms.Where(row => row.InternalExamTermName.ToLower() == model.InternalExamTermName.ToLower()
               && row.RowKey != model.RowKey).ToList();

            InternalExamTerm InternalExamTermModel = new InternalExamTerm();
            if (InternalExamTermCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.InternalExamTerm);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    InternalExamTermModel = dbContext.InternalExamTerms.SingleOrDefault(x => x.RowKey == model.RowKey);
                    InternalExamTermModel.InternalExamTermName = model.InternalExamTermName;
                    InternalExamTermModel.IsActive = model.IsActive;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.InternalExamTerm, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.InternalExamTerm);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.InternalExamTerm, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;

        }
        public InternalExamTermViewModel DeleteInternalExamTerm(InternalExamTermViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    InternalExamTerm InternalExamTermModel = dbContext.InternalExamTerms.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.InternalExamTerms.Remove(InternalExamTermModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.InternalExamTerm, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.InternalExamTerm);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.InternalExamTerm, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.InternalExamTerm);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.InternalExamTerm, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }
        
    }
}
