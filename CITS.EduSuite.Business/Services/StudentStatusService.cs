using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System.Data.Entity.Infrastructure;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class StudentStatusService : IStudentStatusService
    {
        private EduSuiteDatabase dbContext;
        public StudentStatusService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public StudentStatusViewModel GetStudentStatusById(int? id)
        {
            try
            {
                StudentStatusViewModel model = new StudentStatusViewModel();
                model = dbContext.StudentStatus.Select(row => new StudentStatusViewModel
                {
                    RowKey = row.RowKey,
                    StudentStatusName = row.StudentStatusName,
                    IsActive = row.IsActive
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new StudentStatusViewModel();
                }
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.StudentStatus, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);

                return new StudentStatusViewModel();

            }
        }
        public StudentStatusViewModel CreateStudentStatus(StudentStatusViewModel model)
        {
            var StudentStatusCheck = dbContext.StudentStatus.Where(row => row.StudentStatusName.ToLower() == model.StudentStatusName.ToLower()).Count();
            StudentStatu StudentStatusModel = new StudentStatu();
            if (StudentStatusCheck != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.StudentStatus);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    int MaxKey = dbContext.StudentStatus.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    StudentStatusModel.RowKey = Convert.ToInt16(MaxKey + 1);
                    StudentStatusModel.StudentStatusName = model.StudentStatusName;
                    StudentStatusModel.IsActive = model.IsActive;
                    dbContext.StudentStatus.Add(StudentStatusModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentStatus, ActionConstants.Add, DbConstants.LogType.Info, StudentStatusModel.RowKey, model.Message);


                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message =String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.StudentStatus);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentStatus, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public StudentStatusViewModel UpdateStudentStatus(StudentStatusViewModel model)
        {
            var StudentStatusCheck = dbContext.StudentStatus.Where(row => row.StudentStatusName.ToLower() == model.StudentStatusName.ToLower()
               && row.RowKey != model.RowKey).ToList();

            StudentStatu StudentStatusModel = new StudentStatu();
            if (StudentStatusCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.StudentStatus);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    StudentStatusModel = dbContext.StudentStatus.SingleOrDefault(x => x.RowKey == model.RowKey);
                    StudentStatusModel.StudentStatusName = model.StudentStatusName;
                    StudentStatusModel.IsActive = model.IsActive;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentStatus, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.StudentStatus);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentStatus, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;

        }
        public StudentStatusViewModel DeleteStudentStatus(StudentStatusViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    StudentStatu StudentStatusModel = dbContext.StudentStatus.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.StudentStatus.Remove(StudentStatusModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentStatus, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.StudentStatus);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.StudentStatus, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.StudentStatus);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentStatus, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }
        public List<StudentStatusViewModel> GetStudentStatus(string searchText)
        {
            try
            {
                var StudentStatusList = (from p in dbContext.StudentStatus
                                         orderby p.RowKey
                                         where (p.StudentStatusName.Contains(searchText))
                                         select new StudentStatusViewModel
                                  {
                                      RowKey = p.RowKey,
                                      StudentStatusName = p.StudentStatusName,
                                      //IsActiveText = p.IsActive == true ? ApplicationResources.Yes : ApplicationResources.No
                                      IsActive = p.IsActive,
                                  }).ToList();
                return StudentStatusList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<StudentStatusViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.StudentStatus, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<StudentStatusViewModel>();
                

            }
        }
    }
}
