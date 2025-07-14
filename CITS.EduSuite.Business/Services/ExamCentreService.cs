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
    public class ExamCentreService : IExamCentreService
    {
        private EduSuiteDatabase dbContext;
        public ExamCentreService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<ExamCentreViewModel> GetExamCentre(string searchText)
        {
            try
            {
                var ExamCentreList = (from p in dbContext.ExamCenters
                                      orderby p.RowKey descending
                                      where (p.ExamCentreName.Contains(searchText))
                                      select new ExamCentreViewModel
                                      {
                                          RowKey = p.RowKey,
                                          ExamCentreName = p.ExamCentreName,
                                          ExamCentreCode = p.ExamCentreCode,
                                          // IsActiveText = p.IsActive == true ? ApplicationResources.Yes : ApplicationResources.No
                                          IsActive = p.IsActive ?? false,
                                      }).ToList();
                return ExamCentreList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<ExamCentreViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.ExamCentre, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<ExamCentreViewModel>();

            }
        }
        public ExamCentreViewModel GetExamCentreById(int? id)
        {
            try
            {
                ExamCentreViewModel model = new ExamCentreViewModel();
                model = dbContext.ExamCenters.Select(row => new ExamCentreViewModel
                {
                    RowKey = row.RowKey,
                    ExamCentreName = row.ExamCentreName,
                    ExamCentreCode = row.ExamCentreCode,
                    IsActive = row.IsActive ?? false
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new ExamCentreViewModel();
                }
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.ExamCentre, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new ExamCentreViewModel();

            }
        }
        public ExamCentreViewModel CreateExamCentre(ExamCentreViewModel model)
        {
            ExamCenter ExamCentreModel = new ExamCenter();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    long MaxKey = dbContext.ExamCenters.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    ExamCentreModel.RowKey = Convert.ToInt16(MaxKey + 1);
                    ExamCentreModel.ExamCentreName = model.ExamCentreName;
                    ExamCentreModel.ExamCentreCode = model.ExamCentreCode;
                    ExamCentreModel.DateAdded = DateTimeUTC.Now;
                    ExamCentreModel.IsActive = model.IsActive;
                    dbContext.ExamCenters.Add(ExamCentreModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.ExamCentre, ActionConstants.Add, DbConstants.LogType.Info, ExamCentreModel.RowKey, model.Message);


                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.ExamCentre);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.ExamCentre, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public ExamCentreViewModel UpdateExamCentre(ExamCentreViewModel model)
        {
            ExamCenter ExamCentreModel = new ExamCenter();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    ExamCentreModel = dbContext.ExamCenters.SingleOrDefault(x => x.RowKey == model.RowKey);
                    ExamCentreModel.ExamCentreName = model.ExamCentreName;
                    ExamCentreModel.ExamCentreCode = model.ExamCentreCode;
                    ExamCentreModel.IsActive = model.IsActive;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.ExamCentre, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.ExamCentre);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.ExamCentre, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;

        }
        public ExamCentreViewModel DeleteExamCentre(ExamCentreViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    ExamCenter ExamCentreModel = dbContext.ExamCenters.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.ExamCenters.Remove(ExamCentreModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.ExamCentre, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);


                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.ExamCentre);
                        model.IsSuccessful = false;

                        ActivityLog.CreateActivityLog(MenuConstants.ExamCentre, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.ExamCentre);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.ExamCentre, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }
        public ExamCentreViewModel CheckExamCentreCodeExists(ExamCentreViewModel model)
        {
            if (dbContext.ExamCenters.Where(x => x.ExamCentreCode.ToLower() == model.ExamCentreCode.ToLower() && x.RowKey != model.RowKey).Any())
            {
                model.IsSuccessful = false;
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.ExamCentre+ EduSuiteUIResources.BlankSpace+ EduSuiteUIResources.Code);
            }
            else
            {
                model.IsSuccessful = true;
                model.Message = "";
            }
            return model;
        }
        public ExamCentreViewModel CheckExamCentreNameExists(ExamCentreViewModel model)
        {
            if (dbContext.ExamCenters.Where(x => x.ExamCentreName.ToLower() == model.ExamCentreName.ToLower() && x.RowKey != model.RowKey).Any())
            {
                model.IsSuccessful = false;
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.ExamCentre + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Name);
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
