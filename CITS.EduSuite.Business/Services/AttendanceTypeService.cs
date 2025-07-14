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
    public class AttendanceTypeService : IAttendanceTypeService
    {
        private EduSuiteDatabase dbcontext;

        public AttendanceTypeService(EduSuiteDatabase objdb)
        {
            this.dbcontext = objdb;
        }

        public List<AttendanceTypeViewModel> GetAttendanceType(string searchText)
        {
            try
            {
                var AttendanceTypeList = (from AT in dbcontext.AttendanceTypes
                                          orderby AT.RowKey descending
                                          where (AT.AttendanceTypeName.Contains(searchText))
                                          select new AttendanceTypeViewModel
                                          {
                                              RowKey = AT.RowKey,
                                              AttendanceTypeName = AT.AttendanceTypeName,
                                              StartTime = AT.StartTime,
                                              EndTime = AT.EndTime,
                                              GraceTime = AT.GraceTime,
                                              IsActive = AT.IsActive
                                          }).ToList();
                return AttendanceTypeList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<AttendanceTypeViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.AttendanceType, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<AttendanceTypeViewModel>();

            }

        }


        public AttendanceTypeViewModel GetAttendaneTypeById(short? id)
        {
            AttendanceTypeViewModel objViewModel = new AttendanceTypeViewModel();
            objViewModel = dbcontext.AttendanceTypes.Select(row => new AttendanceTypeViewModel
                {
                    RowKey = row.RowKey,
                    AttendanceTypeName = row.AttendanceTypeName,
                    StartTime = row.StartTime,
                    EndTime = row.EndTime,
                    GraceTime = row.GraceTime,
                    IsActive = row.IsActive
                }).Where(row => row.RowKey == id).FirstOrDefault();
            if (objViewModel == null)
            {
                objViewModel = new AttendanceTypeViewModel {  };
            }
           
            return objViewModel;
        }
     
        public AttendanceTypeViewModel CreateAttendanceType(AttendanceTypeViewModel model)
        {
            using (var transaction = dbcontext.Database.BeginTransaction())
            {
                try
                {
                    int MaxKey = dbcontext.AttendanceTypes.Select(x => x.RowKey).DefaultIfEmpty().Max();

                    AttendanceType AttendanceTypeModel = new AttendanceType();
                    AttendanceTypeModel.RowKey = Convert.ToInt16(MaxKey + 1);
                    AttendanceTypeModel.AttendanceTypeName = model.AttendanceTypeName;
                    AttendanceTypeModel.StartTime = model.StartTime;
                    AttendanceTypeModel.EndTime = model.EndTime;
                    AttendanceTypeModel.GraceTime = model.GraceTime;
                    AttendanceTypeModel.IsActive = model.IsActive;
                    dbcontext.AttendanceTypes.Add(AttendanceTypeModel);
                    dbcontext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.AttendanceType, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception Ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.AttendanceType);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.AttendanceType, ActionConstants.Add, DbConstants.LogType.Error, null, Ex.GetBaseException().Message);
                }
                return model;
            }
        }
        public AttendanceTypeViewModel UpdateAttendanceType(AttendanceTypeViewModel model)
        {
            using (var transaction = dbcontext.Database.BeginTransaction())
            {
                try
                {
                    AttendanceType AttendanceTypeModel = new AttendanceType();

                    AttendanceTypeModel = dbcontext.AttendanceTypes.SingleOrDefault(x => x.RowKey == model.RowKey);
                    AttendanceTypeModel.AttendanceTypeName = model.AttendanceTypeName;
                    AttendanceTypeModel.StartTime = model.StartTime;
                    AttendanceTypeModel.EndTime = model.EndTime;
                    AttendanceTypeModel.GraceTime = model.GraceTime;
                    AttendanceTypeModel.IsActive = model.IsActive;
                 

                    dbcontext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.AttendanceType, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception Ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.AttendanceType);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.AttendanceType, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, Ex.GetBaseException().Message);
                }
                return model;
            }
        }

        public AttendanceTypeViewModel DeleteAttendanceTypeAll(short? Id)
        {
            AttendanceTypeViewModel model = new AttendanceTypeViewModel();

            using (var transaction = dbcontext.Database.BeginTransaction())
            {
                try
                {
                    AttendanceType AttendanceTypeList = dbcontext.AttendanceTypes.SingleOrDefault(x => x.RowKey == Id);
                    dbcontext.AttendanceTypes.Remove(AttendanceTypeList);
                    dbcontext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.AttendanceType, ActionConstants.Delete, DbConstants.LogType.Info, Id, model.Message);
                }
                catch (DbUpdateException Ex)
                {
                    transaction.Rollback();
                    if (Ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.AttendanceType);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.AttendanceType, ActionConstants.Delete, DbConstants.LogType.Debug, Id, Ex.GetBaseException().Message);
                    }
                }
                catch (Exception Ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.AttendanceType);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.AttendanceType, ActionConstants.Delete, DbConstants.LogType.Error, Id, Ex.GetBaseException().Message);
                }
            }
            return model;
        }

       
    }
}
