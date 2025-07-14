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
    public class CourseTypeService : ICourseTypeService
    {
        private EduSuiteDatabase dbContext;
        public CourseTypeService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<CourseTypeViewModel> GetCourseType(string searchText)
        {
            try
            {
                var CourseTypeList = (from p in dbContext.CourseTypes
                                      orderby p.RowKey descending
                                      where (p.CourseTypeName.Contains(searchText))
                                      select new CourseTypeViewModel
                                      {
                                          RowKey = p.RowKey,
                                          CourseTypeName = p.CourseTypeName,
                                          HasSecondLanguageText = p.HasSecondLanguage == true ? EduSuiteUIResources.Yes : EduSuiteUIResources.No,
                                          IsActive = p.IsActive,
                                       

                                      }).ToList();
                return CourseTypeList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<CourseTypeViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.CourseType, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<CourseTypeViewModel>();


            }
        }
        public CourseTypeViewModel GetCourseTypeById(int? id)
        {
            try
            {
                CourseTypeViewModel model = new CourseTypeViewModel();
                model = dbContext.CourseTypes.Select(row => new CourseTypeViewModel
                {
                    RowKey = row.RowKey,
                    CourseTypeName = row.CourseTypeName,
                    HasSecondLanguage = row.HasSecondLanguage,
                    IsActive = row.IsActive
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new CourseTypeViewModel();
                }
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.CourseType, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new CourseTypeViewModel();

            }
        }
        public CourseTypeViewModel CreateCourseType(CourseTypeViewModel model)
        {
            var CourseTypeCheck = dbContext.CourseTypes.Where(row => row.CourseTypeName.ToLower() == model.CourseTypeName.ToLower()).Count();
            CourseType CourseTypeModel = new CourseType();
            if (CourseTypeCheck != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.CourseType);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    int MaxKey = dbContext.CourseTypes.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    CourseTypeModel.RowKey = Convert.ToInt16(MaxKey + 1);
                    CourseTypeModel.CourseTypeName = model.CourseTypeName;
                    CourseTypeModel.HasSecondLanguage = model.HasSecondLanguage;
                    CourseTypeModel.IsActive = model.IsActive;
                    dbContext.CourseTypes.Add(CourseTypeModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.CourseType, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.CourseType);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.CourseType, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public CourseTypeViewModel UpdateCourseType(CourseTypeViewModel model)
        {
            var CourseTypeCheck = dbContext.CourseTypes.Where(row => row.CourseTypeName.ToLower() == model.CourseTypeName.ToLower()
               && row.RowKey != model.RowKey).ToList();

            CourseType CourseTypeModel = new CourseType();
            if (CourseTypeCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.CourseType);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    CourseTypeModel = dbContext.CourseTypes.SingleOrDefault(x => x.RowKey == model.RowKey);
                    CourseTypeModel.CourseTypeName = model.CourseTypeName;
                    CourseTypeModel.HasSecondLanguage = model.HasSecondLanguage;
                    CourseTypeModel.IsActive = model.IsActive;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.CourseType, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.CourseType);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.CourseType, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;

        }
        public CourseTypeViewModel DeleteCourseType(CourseTypeViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    CourseType CourseTypeModel = dbContext.CourseTypes.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.CourseTypes.Remove(CourseTypeModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.CourseType, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.CourseType);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.CourseType, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.CourseType);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.CourseType, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }

    }
}
