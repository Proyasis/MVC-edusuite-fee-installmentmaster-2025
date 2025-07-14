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
    public class ExamTermService : IExamTermService
    {
        private EduSuiteDatabase dbContext;
        public ExamTermService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public ExamTermViewModel GetExamTermById(int? id)
        {
            try
            {
                ExamTermViewModel model = new ExamTermViewModel();
                model = dbContext.ExamTerms.Select(row => new ExamTermViewModel
                {
                    RowKey = row.RowKey,
                    ExamTermName = row.ExamTermName,

                    IsActive = row.IsActive ?? false
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new ExamTermViewModel();
                }
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.ExamTerm, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new ExamTermViewModel();

            }
        }
        public ExamTermViewModel CreateExamTerm(ExamTermViewModel model)
        {
            ExamTerm ExamTermModel = new ExamTerm();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    long MaxKey = dbContext.ExamTerms.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    ExamTermModel.RowKey = Convert.ToInt16(MaxKey + 1);
                    ExamTermModel.ExamTermName = model.ExamTermName;
                    ExamTermModel.DateAdded = DateTimeUTC.Now;
                    ExamTermModel.IsActive = model.IsActive;
                    dbContext.ExamTerms.Add(ExamTermModel);
                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.ExamTerm, ActionConstants.Add, DbConstants.LogType.Info, ExamTermModel.RowKey, model.Message);


                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.ExamTerm);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.ExamTerm, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public ExamTermViewModel UpdateExamTerm(ExamTermViewModel model)
        {
            ExamTerm ExamTermModel = new ExamTerm();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    ExamTermModel = dbContext.ExamTerms.SingleOrDefault(x => x.RowKey == model.RowKey);
                    ExamTermModel.ExamTermName = model.ExamTermName;

                    ExamTermModel.IsActive = model.IsActive;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.ExamTerm, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.ExamTerm);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.ExamTerm, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;

        }
        public ExamTermViewModel DeleteExamTerm(ExamTermViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    ExamTerm ExamTermModel = dbContext.ExamTerms.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.ExamTerms.Remove(ExamTermModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.ExamTerm, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);


                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.ExamTerm);
                        model.IsSuccessful = false;

                        ActivityLog.CreateActivityLog(MenuConstants.ExamTerm, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.ExamTerm);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.ExamTerm, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }
        public List<ExamTermViewModel> GetExamTerm(string searchText)
        {
            try
            {
                var ExamTermList = (from p in dbContext.ExamTerms
                                    orderby p.RowKey descending
                                    where (p.ExamTermName.Contains(searchText))
                                    select new ExamTermViewModel
                                    {
                                        RowKey = p.RowKey,
                                        ExamTermName = p.ExamTermName,
                                        //IsActiveText = p.IsActive == true ? ApplicationResources.Yes : ApplicationResources.No
                                        IsActive = p.IsActive ?? false,
                                    }).ToList();
                return ExamTermList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<ExamTermViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.ExamTerm, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<ExamTermViewModel>();

            }
        }

        public ExamTermViewModel CheckExamTermNameExists(ExamTermViewModel model)
        {
            if (dbContext.ExamTerms.Where(x => x.ExamTermName.ToLower() == model.ExamTermName.ToLower() && x.RowKey != model.RowKey).Any())
            {
                model.IsSuccessful = false;
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.ExamTerm);
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
