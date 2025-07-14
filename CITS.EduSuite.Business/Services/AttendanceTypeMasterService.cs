using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class AttendanceTypeMasterService : IAttendanceTypeMasterService
    {
        private EduSuiteDatabase dbcontext;

        public AttendanceTypeMasterService(EduSuiteDatabase objdb)
        {
            this.dbcontext = objdb;
        }

        public AttendanceTypeMasterViewModel GetAttendaneTypeMasterById(short? id)
        {
            AttendanceTypeMasterViewModel objViewModel = new AttendanceTypeMasterViewModel();
            objViewModel = dbcontext.AttendanceTypeMasters.Select(row => new AttendanceTypeMasterViewModel
                {
                    RowKey = row.RowKey,
                    FromDate = row.FromDate,
                    ToDate = row.ToDate,
                    ClassKeys = row.AttendanceTypeMasterClasses.Select(x => x.ClassDetailsKey).ToList(),
                    AttendanceTypeDetailsModel = row.AttendanceTypeDetails.Select(x => new AttendanceTypeDetailsModel
                    {
                        AttendanceTypeKey = x.AttendanceTypeKey,
                        StartTime = x.StartTime,
                        EndTime = x.EndTime,
                        GraceTime = x.GraceTime,
                        RowKey = x.RowKey,
                        AttendanceTypeMasterKey = x.AttendanceTypeMasterKey
                    }).ToList(),

                }).Where(row => row.RowKey == id).FirstOrDefault();


            if (objViewModel == null)
            {
                objViewModel = new AttendanceTypeMasterViewModel { };
                objViewModel.AttendanceTypeDetailsModel.Add(new AttendanceTypeDetailsModel());
            }
            //else
            //{
            //    objViewModel.ClassKeys = objViewModel.Classess.Split(',').Select(Int64.Parse).ToList();
            //}
            FillDropdownList(objViewModel);
            return objViewModel;
        }

        public AttendanceTypeMasterViewModel CreateAttendanceTypeMaster(AttendanceTypeMasterViewModel model)
        {
            using (var transaction = dbcontext.Database.BeginTransaction())
            {
                try
                {
                    int MaxKey = dbcontext.AttendanceTypeMasters.Select(x => x.RowKey).DefaultIfEmpty().Max();

                    AttendanceTypeMaster AttendanceTypeMasterModel = new AttendanceTypeMaster();
                    AttendanceTypeMasterModel.RowKey = Convert.ToInt16(MaxKey + 1);
                    AttendanceTypeMasterModel.FromDate = model.FromDate;
                    AttendanceTypeMasterModel.ToDate = model.ToDate;
                    
                    dbcontext.AttendanceTypeMasters.Add(AttendanceTypeMasterModel);
                    model.RowKey = AttendanceTypeMasterModel.RowKey;
                    UpdateClassAllocation(model);
                    CreateAttendanceTypeDetails(model.AttendanceTypeDetailsModel.Where(x => x.RowKey == 0).ToList(), model);
                    dbcontext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.AttendanceTypeMaster, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception Ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.AttendanceTypeMaster);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.AttendanceTypeMaster, ActionConstants.Add, DbConstants.LogType.Error, null, Ex.GetBaseException().Message);
                }
                return model;
            }
        }

        public AttendanceTypeMasterViewModel UpdateAttendanceTypeMaster(AttendanceTypeMasterViewModel model)
        {
            using (var transaction = dbcontext.Database.BeginTransaction())
            {
                try
                {
                    AttendanceTypeMaster AttendanceTypeMasterModel = new AttendanceTypeMaster();

                    AttendanceTypeMasterModel = dbcontext.AttendanceTypeMasters.SingleOrDefault(x => x.RowKey == model.RowKey);
                    AttendanceTypeMasterModel.FromDate = model.FromDate;
                    AttendanceTypeMasterModel.ToDate = model.ToDate;
                    UpdateClassAllocation(model);
                    CreateAttendanceTypeDetails(model.AttendanceTypeDetailsModel.Where(x => x.RowKey == 0).ToList(), model);
                    UpdateAttendanceTypeDetails(model.AttendanceTypeDetailsModel.Where(x => x.RowKey != 0).ToList(), model);

                    dbcontext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.AttendanceTypeMaster, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception Ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.AttendanceTypeMaster);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.AttendanceTypeMaster, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, Ex.GetBaseException().Message);
                }
                return model;
            }
        }

        private void CreateAttendanceTypeDetails(List<AttendanceTypeDetailsModel> modelList, AttendanceTypeMasterViewModel model)
        {
            short MaxKey = dbcontext.AttendanceTypeDetails.Select(x => x.RowKey).DefaultIfEmpty().Max();

            foreach (AttendanceTypeDetailsModel item in modelList)
            {
                AttendanceTypeDetail AttendanceTypeDetailModel = new AttendanceTypeDetail();
                AttendanceTypeDetailModel.RowKey = ++MaxKey;
                AttendanceTypeDetailModel.AttendanceTypeKey = item.AttendanceTypeKey;
                AttendanceTypeDetailModel.StartTime = item.StartTime;
                AttendanceTypeDetailModel.EndTime = item.EndTime;
                AttendanceTypeDetailModel.GraceTime = item.GraceTime;
                AttendanceTypeDetailModel.AttendanceTypeMasterKey = model.RowKey;
                dbcontext.AttendanceTypeDetails.Add(AttendanceTypeDetailModel);
            }
        }
        private void UpdateAttendanceTypeDetails(List<AttendanceTypeDetailsModel> modelList, AttendanceTypeMasterViewModel model)
        {

            foreach (AttendanceTypeDetailsModel item in modelList)
            {
                AttendanceTypeDetail AttendanceTypeDetailModel = new AttendanceTypeDetail();
                AttendanceTypeDetailModel = dbcontext.AttendanceTypeDetails.Where(x => x.RowKey == item.RowKey).SingleOrDefault();
                AttendanceTypeDetailModel.AttendanceTypeKey = item.AttendanceTypeKey;
                AttendanceTypeDetailModel.StartTime = item.StartTime;
                AttendanceTypeDetailModel.EndTime = item.EndTime;
                AttendanceTypeDetailModel.GraceTime = item.GraceTime;
                AttendanceTypeDetailModel.AttendanceTypeMasterKey = model.RowKey;
            }
        }
        public List<AttendanceTypeMasterViewModel> GetAttendanceTypeMaster(string searchText)
        {
            try
            {
                var AttendanceTypeList = (from AT in dbcontext.AttendanceTypeMasters
                                          orderby AT.RowKey
                                         
                                          select new AttendanceTypeMasterViewModel
                                          {
                                              RowKey = AT.RowKey,
                                              FromDate = AT.FromDate,
                                              ToDate = AT.ToDate,

                                          }).ToList();
                return AttendanceTypeList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<AttendanceTypeMasterViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.AttendanceTypeMaster, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<AttendanceTypeMasterViewModel>();

            }

        }

        public AttendanceTypeMasterViewModel DeleteAttendanceTypeMaster(short? Id)
        {
            AttendanceTypeMasterViewModel model = new AttendanceTypeMasterViewModel();

            using (var transaction = dbcontext.Database.BeginTransaction())
            {
                try
                {
                    AttendanceTypeMaster attendanceTypeMaster = dbcontext.AttendanceTypeMasters.SingleOrDefault(x => x.RowKey == Id);
                    List<AttendanceTypeDetail> attendanceTypeDetailList = dbcontext.AttendanceTypeDetails.Where(x => x.AttendanceTypeMasterKey == Id).ToList();
                    List<AttendanceTypeMasterClass> attendanceTypeMasterClassList = dbcontext.AttendanceTypeMasterClasses.Where(x => x.AttendanceTypeMasterKey == Id).ToList();

                    dbcontext.AttendanceTypeMasterClasses.RemoveRange(attendanceTypeMasterClassList);
                    dbcontext.AttendanceTypeDetails.RemoveRange(attendanceTypeDetailList);
                    dbcontext.AttendanceTypeMasters.Remove(attendanceTypeMaster);

                    dbcontext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.AttendanceTypeMaster, ActionConstants.Delete, DbConstants.LogType.Info, Id, model.Message);
                }
                catch (DbUpdateException Ex)
                {
                    transaction.Rollback();
                    if (Ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.AttendanceTypeMaster);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.AttendanceTypeMaster, ActionConstants.Delete, DbConstants.LogType.Debug, Id, Ex.GetBaseException().Message);
                    }
                }
                catch (Exception Ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.AttendanceTypeMaster);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.AttendanceTypeMaster, ActionConstants.Delete, DbConstants.LogType.Error, Id, Ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public AttendanceTypeMasterViewModel DeleteAttendanceTypeDetails(short? Id)
        {
            AttendanceTypeMasterViewModel model = new AttendanceTypeMasterViewModel();

            using (var transaction = dbcontext.Database.BeginTransaction())
            {
                try
                {
                    AttendanceTypeDetail attendanceTypeDetailList = dbcontext.AttendanceTypeDetails.SingleOrDefault(x => x.RowKey == Id);
                    dbcontext.AttendanceTypeDetails.Remove(attendanceTypeDetailList);
                    dbcontext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.AttendanceTypeMaster, ActionConstants.Delete, DbConstants.LogType.Info, Id, model.Message);
                }
                catch (DbUpdateException Ex)
                {
                    transaction.Rollback();
                    if (Ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.AttendanceTypeMaster);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.AttendanceTypeMaster, ActionConstants.Delete, DbConstants.LogType.Debug, Id, Ex.GetBaseException().Message);
                    }
                }
                catch (Exception Ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.AttendanceTypeMaster);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.AttendanceTypeMaster, ActionConstants.Delete, DbConstants.LogType.Error, Id, Ex.GetBaseException().Message);
                }
            }
            return model;
        }

        private void FillDropdownList(AttendanceTypeMasterViewModel model)
        {
            FillAttendanceType(model);
            FillClass(model);
        }
        public void FillClass(AttendanceTypeMasterViewModel model)
        {
            model.ÇlassDetails = dbcontext.VwClassDetailsSelectActiveOnlies.Where(row => row.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ClassCode + row.ClassCodeDescription
            }).ToList();
        }

        public void FillAttendanceType(AttendanceTypeMasterViewModel model)
        {
            model.AttendanceTypes = dbcontext.AttendanceTypes.Where(row => row.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AttendanceTypeName
            }).ToList();
        }

        private void UpdateClassAllocation(AttendanceTypeMasterViewModel model)
        {
            Int64 MaxKey = dbcontext.AttendanceTypeMasterClasses.Select(x => x.RowKey).DefaultIfEmpty().Max();
            List<AttendanceTypeMasterClass> AttendanceTypeMasterClasses = dbcontext.AttendanceTypeMasterClasses.Where(x => x.AttendanceTypeMasterKey == model.RowKey).ToList();

            if (AttendanceTypeMasterClasses.Count > 0)
            {
                dbcontext.AttendanceTypeMasterClasses.RemoveRange(AttendanceTypeMasterClasses);
            }
            if (model.ClassKeys != null && model.ClassKeys.Count > 0)
            {
                foreach (long ClassKey in model.ClassKeys)
                {
                    AttendanceTypeMasterClass objAttendanceTypeMasterClassmodel = new AttendanceTypeMasterClass();

                    objAttendanceTypeMasterClassmodel.RowKey = MaxKey + 1;
                    objAttendanceTypeMasterClassmodel.AttendanceTypeMasterKey = model.RowKey;
                    objAttendanceTypeMasterClassmodel.ClassDetailsKey = ClassKey;

                    dbcontext.AttendanceTypeMasterClasses.Add(objAttendanceTypeMasterClassmodel);
                    dbcontext.SaveChanges();
                    MaxKey++;
                }
            }
        }

    }
}
