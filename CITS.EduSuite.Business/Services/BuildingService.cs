using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Data;
using CITS.EduSuite.Business.Models.ViewModels;
using System.Data.Entity.Infrastructure;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class BuildingService : IBuildingService
    {
        private EduSuiteDatabase dbContext;

        public BuildingService(EduSuiteDatabase objdb)
        {
            this.dbContext = objdb;
        }

        public BuildingViewModel CreateBuildingMaster(BuildingViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    int MaxKey = dbContext.BuildingMasters.Select(x => x.RowKey).DefaultIfEmpty().Max();
                    short? DisplayOrder = dbContext.BuildingMasters.Select(x => x.DisplayOrder).DefaultIfEmpty().Max();


                    BuildingMaster buildingMastermodel = new BuildingMaster();

                    buildingMastermodel.RowKey = Convert.ToInt32(MaxKey + 1);
                    buildingMastermodel.BuildingMasterName = model.BuildingMasterName;
                    buildingMastermodel.RoomCount = model.RoomCount;
                    buildingMastermodel.DisplayOrder = Convert.ToInt16(DisplayOrder + 1);
                    buildingMastermodel.IsActive = model.IsActive;
                    dbContext.BuildingMasters.Add(buildingMastermodel);

                    CreateBuildingDetails(model.BuildingDetails.Where(x => x.RowKey == 0).ToList(), buildingMastermodel.RowKey);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Building, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception Ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.BuildingDetails);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Building, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, Ex.GetBaseException().Message);
                }
                return model;
            }
        }

        public BuildingViewModel UpdateBuildingMaster(BuildingViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {


                    BuildingMaster buildingMastermodel = new BuildingMaster();

                    buildingMastermodel = dbContext.BuildingMasters.SingleOrDefault(row => row.RowKey == model.RowKey);
                    buildingMastermodel.BuildingMasterName = model.BuildingMasterName;
                    buildingMastermodel.RoomCount = model.RoomCount;
                    buildingMastermodel.IsActive = model.IsActive;

                    CreateBuildingDetails(model.BuildingDetails.Where(x => x.RowKey == 0).ToList(), buildingMastermodel.RowKey);
                    UpdateBuildingDetails(model.BuildingDetails.Where(x => x.RowKey != 0).ToList(), buildingMastermodel.RowKey);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.Building, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception Ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.BuildingDetails);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Building, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, Ex.GetBaseException

().Message);
                }
                return model;
            }
        }

        private void CreateBuildingDetails(List<BuildingDetailsModel> ModelList, int BuildingMasterKey)
        {
            long MaxKey = dbContext.BuildingDetails.Select(p => p.RowKey).DefaultIfEmpty().Max();
            short? DisplayOrder = dbContext.BuildingDetails.Select(x => x.DisplayOrder).DefaultIfEmpty().Max();
            foreach (BuildingDetailsModel model in ModelList)
            {
                BuildingDetail buildingDetailsmodel = new BuildingDetail();

                buildingDetailsmodel.RowKey = Convert.ToInt64(MaxKey + 1);
                buildingDetailsmodel.BuildingDetailsName = model.BuildingDetailsName;
                buildingDetailsmodel.BuildingMasterKey = BuildingMasterKey;
                buildingDetailsmodel.DisplayOrder = Convert.ToInt16(DisplayOrder + 1);
                buildingDetailsmodel.IsActive = model.IsActive;

                dbContext.BuildingDetails.Add(buildingDetailsmodel);
                MaxKey++;
                DisplayOrder++;
            }

        }

        private void UpdateBuildingDetails(List<BuildingDetailsModel> ModelList, int BuildingMasterKey)
        {

            foreach (BuildingDetailsModel model in ModelList)
            {
                BuildingDetail buildingDetailsmodel = new BuildingDetail();

                buildingDetailsmodel = dbContext.BuildingDetails.SingleOrDefault(x => x.RowKey == model.RowKey);
                buildingDetailsmodel.BuildingDetailsName = model.BuildingDetailsName;
                buildingDetailsmodel.BuildingMasterKey = BuildingMasterKey;
                buildingDetailsmodel.IsActive = model.IsActive;


            }

        }

        public BuildingViewModel GetBuildingDetailsById(int? id)
        {
            BuildingViewModel objviewmodel = new BuildingViewModel();
            objviewmodel = dbContext.BuildingMasters.Select(row => new BuildingViewModel
                {
                    RowKey = row.RowKey,
                    BuildingMasterName = row.BuildingMasterName,
                    RoomCount = row.RoomCount,
                    IsActive = row.IsActive,

                }).Where(row => row.RowKey == id).FirstOrDefault();
            //if (objviewmodel == null)
            //{
            //    objviewmodel = new BuildingViewModel();
            //}
            if (objviewmodel == null)
            {
                objviewmodel = new BuildingViewModel { RoomCount = null };
            }
            FillBuildingDetails(objviewmodel);
            return objviewmodel;
        }
        private void FillBuildingDetails(BuildingViewModel model)
        {


            model.BuildingDetails = (from row in dbContext.BuildingDetails.Where(x => x.BuildingMasterKey == model.RowKey)
                                     select new BuildingDetailsModel
                                 {
                                     RowKey = row.RowKey,
                                     BuildingDetailsName = row.BuildingDetailsName,
                                     BuildingMasterKey = row.BuildingMasterKey,
                                     IsActive = row.IsActive
                                 }).ToList();
            ////if (model.BuildingDetails.Count == 0)
            ////{
            ////    model.BuildingDetails.Add(new BuildingDetailsModel());
            ////}
            //if (model.BuildingDetails.Count == 0)
            //{
            //    model.BuildingDetails.Add(new BuildingDetailsModel { BuildingDetailsName = model.BuildingMasterName != null ? model.BuildingMasterName + "1" : "" });
            //}

        }

        public List<BuildingViewModel> GetBuildingDetails(string searchText)
        {
            try
            {
                var BuildingDetailsList = (from BD in dbContext.BuildingMasters
                                           orderby BD.RowKey
                                           where (BD.BuildingMasterName.Contains(searchText))
                                           select new BuildingViewModel
                                           {
                                               RowKey = BD.RowKey,
                                               BuildingMasterName = BD.BuildingMasterName,
                                               RoomCount = BD.RoomCount,
                                               IsActive = BD.IsActive
                                           }).ToList();
                return BuildingDetailsList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<BuildingViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Building, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<BuildingViewModel>();
               
            }

        }



        public BuildingViewModel DeleteBuildingDetailsAll(int Id)
        {
            BuildingViewModel model = new BuildingViewModel();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    List<BuildingDetail> BuildingDetailsList = dbContext.BuildingDetails.Where(x => x.BuildingMasterKey == Id).ToList();
                    BuildingMaster BuildingmasterList = dbContext.BuildingMasters.SingleOrDefault(x => x.RowKey == Id);

                    dbContext.BuildingDetails.RemoveRange(BuildingDetailsList);
                    dbContext.BuildingMasters.Remove(BuildingmasterList);




                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.Building, ActionConstants.Delete, DbConstants.LogType.Info, Id, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.BuildingDetails);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.Building, ActionConstants.Delete, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);

                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.BuildingDetails);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Building, ActionConstants.Delete, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public BuildingViewModel DeleteBuildingDetails(long Id, int BuildingMasterKey)
        {
            BuildingViewModel model = new BuildingViewModel();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    BuildingDetail BuildingDetailsList = dbContext.BuildingDetails.SingleOrDefault(x => x.RowKey == Id);

                    var classDetails = dbContext.ClassDetails.Any(x => x.BuildingDetailsKey == Id);
                    if (classDetails == false)
                    {
                        dbContext.BuildingDetails.Remove(BuildingDetailsList);

                        var BuildingDetails = dbContext.BuildingDetails.Any(x => x.BuildingMasterKey == BuildingMasterKey);
                        BuildingMaster BuildingmasterList = dbContext.BuildingMasters.SingleOrDefault(x => x.RowKey == BuildingMasterKey);
                        if (BuildingDetails == false)
                        {

                            dbContext.BuildingMasters.Remove(BuildingmasterList);
                        }
                        else
                        {
                            BuildingmasterList.RoomCount = BuildingmasterList.RoomCount - 1;
                        }
                    }
                    else
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.BuildingDetails);
                        model.IsSuccessful = false;
                    }


                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Building, ActionConstants.Delete, DbConstants.LogType.Info, Id, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.BuildingDetails);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.Building, ActionConstants.Delete, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);

                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.BuildingDetails);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Building, ActionConstants.Delete, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);
                }
            }
            return model;


        }
    }
}
