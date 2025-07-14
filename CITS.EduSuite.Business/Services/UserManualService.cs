using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Services
{
    public class UserManualService : IUserManualService
    {
        private EduSuiteDatabase dbContext;

        public UserManualService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public UserManualViewModel GetUserManualById(int? id)
        {
            try
            {
                UserManualViewModel model = new UserManualViewModel();
                model = dbContext.UserManualMasters.Select(row => new UserManualViewModel
                {
                    RowKey = row.RowKey,
                    UserManualTypeKey = row.UserManualTypeKey,
                    DashBoardTypeKey = row.DashBoardTypeKey ?? 0,
                    MenuTypeKey = row.MenuTypeKey ?? 0,
                    MenuKey = row.MenuKey ?? 0,
                    Descreption = row.Descreption,
                    Document = row.DocumentPath,
                    IsActive = row.IsActive,
                    Notes = row.Notes,
                    DocumentPath = UrlConstants.UserManual + row.RowKey + "/" + row.DocumentPath,

                    UserManualDetails = (from x in row.UserManualDetails
                                         select new UserManualDetailsViewModel
                                         {
                                             RowKey = x.RowKey,
                                             UserManualMasterKey = x.UserManualMasterKey,
                                             Descreption = x.Descreption,
                                             Document = x.DocumentPath,
                                             DocumentPath = UrlConstants.UserManual + x.UserManualMasterKey + "/UserManualDetails/" + x.DocumentPath,
                                         }).ToList(),

                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new UserManualViewModel();
                    UserManualDetailsViewModel UserManualDetails = new UserManualDetailsViewModel();
                    model.UserManualDetails.Add(UserManualDetails);
                }

                if (model.UserManualDetails == null || model.UserManualDetails.Count == 0)
                {
                    UserManualDetailsViewModel UserManualDetails = new UserManualDetailsViewModel();
                    model.UserManualDetails.Add(UserManualDetails);
                }

                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.UserManual, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new UserManualViewModel();
            }
        }

        public UserManualViewModel CreateUserManual(UserManualViewModel model)
        {
            UserManualMaster UserManualModel = new UserManualMaster();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    long MaxKey = dbContext.UserManualMasters.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    UserManualModel.RowKey = Convert.ToInt16(MaxKey + 1);
                    UserManualModel.UserManualTypeKey = model.UserManualTypeKey;
                    UserManualModel.DashBoardTypeKey = model.DashBoardTypeKey;
                    UserManualModel.MenuTypeKey = model.MenuTypeKey;
                    UserManualModel.MenuKey = model.MenuKey;
                    UserManualModel.Descreption = model.Descreption;
                    UserManualModel.IsActive = model.IsActive;
                    UserManualModel.Notes = model.Notes;
                    if (model.DocumentFile != null)
                    {
                        string Extension = Path.GetExtension(model.DocumentFile.FileName);
                        string FileName = UserManualModel.RowKey + Extension;
                        UserManualModel.DocumentPath = FileName;
                    }
                    dbContext.UserManualMasters.Add(UserManualModel);
                    model.RowKey = UserManualModel.RowKey;
                    CreateUserMenuDetails(model);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.UserManual, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Menu);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.UserManual, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;
        }

        public UserManualViewModel UpdateUserManual(UserManualViewModel model)
        {
            UserManualMaster UserManualModel = new UserManualMaster();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    UserManualModel = dbContext.UserManualMasters.SingleOrDefault(x => x.RowKey == model.RowKey);
                    UserManualModel.UserManualTypeKey = model.UserManualTypeKey;
                    UserManualModel.DashBoardTypeKey = model.DashBoardTypeKey;
                    UserManualModel.MenuTypeKey = model.MenuTypeKey;
                    UserManualModel.MenuKey = model.MenuKey;
                    UserManualModel.Descreption = model.Descreption;
                    UserManualModel.IsActive = model.IsActive;
                    UserManualModel.Notes = model.Notes;
                    if (model.DocumentFile != null)
                    {
                        string Extension = Path.GetExtension(model.DocumentFile.FileName);
                        string FileName = UserManualModel.RowKey + Extension;
                        UserManualModel.DocumentPath = FileName;
                    }
                    UpdateUserMenuDetails(model);
                    CreateUserMenuDetails(model);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.UserManual, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Menu);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.UserManual, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;

        }

        private void CreateUserMenuDetails(UserManualViewModel model)
        {

            long MaxKey = dbContext.UserManualDetails.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (var item in model.UserManualDetails.Where(x => x.RowKey == 0))
            {
                UserManualDetail DbUserManualDetail = new UserManualDetail();
                DbUserManualDetail.RowKey = Convert.ToInt16(MaxKey + 1);
                DbUserManualDetail.UserManualMasterKey = model.RowKey;
                DbUserManualDetail.Descreption = item.Descreption;
                DbUserManualDetail.IsActive = true;
                if (item.DocumentFileDetails.ContentLength > 0)
                {
                    DbUserManualDetail.DocumentPath = DbUserManualDetail.RowKey + item.DocumentPath;
                }
                dbContext.UserManualDetails.Add(DbUserManualDetail);
                MaxKey++;
                item.DocumentPath = DbUserManualDetail.DocumentPath;
                item.RowKey = DbUserManualDetail.RowKey;
            }
        }

        private void UpdateUserMenuDetails(UserManualViewModel model)
        {

            foreach (var item in model.UserManualDetails.Where(x => x.RowKey != 0))
            {
                UserManualDetail DbUserManualDetail = dbContext.UserManualDetails.Where(x => x.RowKey == item.RowKey).SingleOrDefault();
                DbUserManualDetail.UserManualMasterKey = model.RowKey;
                DbUserManualDetail.Descreption = item.Descreption;
                if (item.DocumentFileDetails.ContentLength > 0)
                {
                    DbUserManualDetail.DocumentPath = DbUserManualDetail.RowKey + item.DocumentPath;
                }
                item.DocumentPath = DbUserManualDetail.DocumentPath;

            }
        }

        public UserManualViewModel DeleteUserManual(UserManualViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    List<UserManualDetail> UserManualDetailList = dbContext.UserManualDetails.Where(x => x.UserManualMasterKey == model.RowKey).ToList();
                    UserManualMaster UserManualModel = dbContext.UserManualMasters.SingleOrDefault(row => row.RowKey == model.RowKey);

                    dbContext.UserManualDetails.RemoveRange(UserManualDetailList);
                    dbContext.UserManualMasters.Remove(UserManualModel);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.UserManual, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Menu);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.UserManual, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Menu);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.UserManual, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }

        public UserManualDetailsViewModel DeleteUserManualDetails(UserManualDetailsViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    UserManualDetail dbUserManualDetail = dbContext.UserManualDetails.SingleOrDefault(row => row.RowKey == model.RowKey);
                    model.DocumentPath = dbUserManualDetail.DocumentPath;
                    model.UserManualMasterKey = dbUserManualDetail.UserManualMasterKey;
                    dbContext.UserManualDetails.Remove(dbUserManualDetail);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.UserManual, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Menu);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.UserManual, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Menu);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.UserManual, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }

        public List<UserManualViewModel> GetUserManual(UserManualViewModel model)
        {
            try
            {
                IQueryable<UserManualViewModel> MenuList = (from row in dbContext.UserManualMasters
                                                            orderby row.RowKey
                                                            //where (row.Menu.Contains(model.us))
                                                            select new UserManualViewModel
                                                            {
                                                                RowKey = row.RowKey,
                                                                UserManualTypeKey = row.UserManualTypeKey,
                                                                UserManalTypeName = row.UserManualType.UserManualTypeName,
                                                                DashBoardTypeKey = row.DashBoardTypeKey ?? 0,
                                                                DashBoardTypeName = row.DashBoardType.DashBoardTypeName,
                                                                MenuTypeKey = row.MenuTypeKey ?? 0,
                                                                MenuTypeName = row.MenuType.MenuTypeName,
                                                                MenuKey = row.MenuKey ?? 0,
                                                                MenuName = row.Menu.MenuName,
                                                                Descreption = row.Descreption,
                                                                DocumentPath = row.DocumentPath,
                                                                IsActive = row.IsActive,
                                                                Notes = row.Notes,
                                                            });
                if (model.MenuTypeKey != null)
                {
                    MenuList = MenuList.Where(x => x.MenuTypeKey == model.MenuTypeKey);
                }

                return MenuList.ToList().GroupBy(x => x.RowKey).Select(y => y.First()).ToList<UserManualViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.UserManual, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);

                return new List<UserManualViewModel>();

            }
        }

        public UserManualViewModel DeleteUserManualDocument(short Id)
        {
            UserManualViewModel model = new UserManualViewModel();
            UserManualMaster dbUserManualMaster = new UserManualMaster();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    dbUserManualMaster = dbContext.UserManualMasters.SingleOrDefault(row => row.RowKey == Id);
                    model.Document = dbUserManualMaster.DocumentPath;
                    model.DocumentPath = UrlConstants.UserManual + dbUserManualMaster.RowKey + "/" + dbUserManualMaster.DocumentPath;
                    model.RowKey = dbUserManualMaster.RowKey;
                    dbUserManualMaster.DocumentPath = null;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.UserManual, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Branch);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.UserManual, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public UserManualAllViewModel ViewUserManualAll()
        {
            UserManualAllViewModel model = new UserManualAllViewModel();

            model.UserManualViewAll = dbContext.UserManualMasters.Select(row => new UserManualViewModel
            {
                RowKey = row.RowKey,
                UserManualTypeKey = row.UserManualTypeKey,
                UserManalTypeName = row.UserManualType.UserManualTypeName,
                DashBoardTypeKey = row.DashBoardTypeKey ?? 0,
                DashBoardTypeName = row.DashBoardType.DashBoardTypeName,
                MenuTypeKey = row.MenuTypeKey ?? 0,
                MenuTypeName = row.MenuType.MenuTypeName,
                MenuKey = row.MenuKey ?? 0,
                MenuName = row.Menu.MenuName,
                IsActive = row.IsActive,
                Descreption=row.Descreption,
                Notes = row.Notes,
                DocumentPath = UrlConstants.UserManual + row.RowKey + "/" + row.DocumentPath,

                UserManualDetails = (from x in row.UserManualDetails
                                     select new UserManualDetailsViewModel
                                     {
                                         RowKey = x.RowKey,
                                         UserManualMasterKey = x.UserManualMasterKey,
                                         Descreption = x.Descreption,
                                         Document = x.DocumentPath,
                                         DocumentPath = UrlConstants.UserManual + x.UserManualMasterKey + "/UserManualDetails/" + x.DocumentPath,
                                     }).ToList(),
            }).ToList();

            return model;
        }
    }
}
