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
    public class MenuDetailViewService : IMenuDetailViewService
    {
        private EduSuiteDatabase dbContext;

        public MenuDetailViewService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public MenuDetailViewModel GetMenuDetailViewById(int? id)
        {
            try
            {
                MenuDetailViewModel model = new MenuDetailViewModel();
                model = dbContext.Menus.Select(row => new MenuDetailViewModel
                {
                    RowKey = row.RowKey,
                    MenuTypeKey = row.MenuTypeKey,
                    MenuCode = row.MenuCode,
                    MenuName = row.MenuName,
                    ActionName = row.ActionName,
                    ControllerName = row.ControllerName,
                    OptionalParameter = row.OptionalParameter,
                    IconClassName = row.IconClassName,
                    MenucatagoryKey = row.MenuCatagoryKey ?? 0,
                    DisplayOrder = row.DisplayOrder,
                    IsActive = row.IsActive,
                    MenuActionsList = (from x in row.MenuActions
                                       select new MenuActionModel
                                       {
                                           RowKey = x.RowKey,
                                           MenuKey = x.MenuKey,
                                           ActionKey = x.ActionKey,
                                           IsActive = x.IsActive,
                                       }).ToList(),

                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new MenuDetailViewModel();
                    MenuActionModel MenuAction = new MenuActionModel();
                    model.MenuActionsList.Add(MenuAction);
                }

                if (model.MenuActionsList == null || model.MenuActionsList.Count == 0)
                {
                    MenuActionModel MenuAction = new MenuActionModel();
                    model.MenuActionsList.Add(MenuAction);
                }

                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Menu, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new MenuDetailViewModel();


            }
        }

        public MenuDetailViewModel CreateMenuDetailView(MenuDetailViewModel model)
        {

            //model = CheckJobPositionNameExists(model);
            //if (model.IsSuccessful == false)
            //{
            //    return model;
            //}


            Menu MenuDetailViewModel = new Menu();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    int MaxKey = dbContext.Menus.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    MenuDetailViewModel.RowKey = Convert.ToInt16(MaxKey + 1);
                    MenuDetailViewModel.MenuTypeKey = model.MenuTypeKey ?? 0;
                    MenuDetailViewModel.MenuCode = model.MenuCode;
                    MenuDetailViewModel.MenuName = model.MenuName;
                    MenuDetailViewModel.ActionName = model.ActionName;
                    MenuDetailViewModel.ControllerName = model.ControllerName;
                    MenuDetailViewModel.OptionalParameter = model.OptionalParameter;
                    MenuDetailViewModel.IconClassName = model.IconClassName;
                    MenuDetailViewModel.MenuCatagoryKey = model.MenucatagoryKey;
                    MenuDetailViewModel.IsActive = model.IsActive;
                    MenuDetailViewModel.DisplayOrder = model.DisplayOrder;
                    dbContext.Menus.Add(MenuDetailViewModel);
                    model.RowKey = MenuDetailViewModel.RowKey;
                    CreateMenuAction(model);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Agent, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Menu);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Menu, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;
        }

        public MenuDetailViewModel UpdateMenuDetailView(MenuDetailViewModel model)
        {

            Menu MenuDetailViewModel = new Menu();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    MenuDetailViewModel = dbContext.Menus.SingleOrDefault(x => x.RowKey == model.RowKey);
                    MenuDetailViewModel.MenuTypeKey = model.MenuTypeKey ?? 0;
                    MenuDetailViewModel.MenuCode = model.MenuCode;
                    MenuDetailViewModel.MenuName = model.MenuName;
                    MenuDetailViewModel.ActionName = model.ActionName;
                    MenuDetailViewModel.ControllerName = model.ControllerName;
                    MenuDetailViewModel.OptionalParameter = model.OptionalParameter;
                    MenuDetailViewModel.IconClassName = model.IconClassName;
                    MenuDetailViewModel.MenuCatagoryKey = model.MenucatagoryKey;
                    MenuDetailViewModel.DisplayOrder = model.DisplayOrder;
                    MenuDetailViewModel.IsActive = model.IsActive;
                    CreateMenuAction(model);
                    UpdateMenuAction(model);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Menu, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Menu);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Menu, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;

        }

        private void CreateMenuAction(MenuDetailViewModel model)
        {

            short MaxKey = dbContext.MenuActions.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (var item in model.MenuActionsList.Where(x => x.RowKey == null))
            {
                MenuAction DbMenuAction = new MenuAction();
                DbMenuAction.RowKey = Convert.ToInt16(MaxKey + 1);
                DbMenuAction.MenuKey = model.RowKey;
                DbMenuAction.ActionKey = item.ActionKey;
                DbMenuAction.IsActive = item.IsActive;
                dbContext.MenuActions.Add(DbMenuAction);
                MaxKey++;

            }
        }

        private void UpdateMenuAction(MenuDetailViewModel model)
        {

            foreach (var item in model.MenuActionsList.Where(x => x.RowKey != 0 && x.RowKey != null))
            {
                MenuAction DbMenuAction = dbContext.MenuActions.Where(x => x.RowKey == item.RowKey).SingleOrDefault();

                DbMenuAction.MenuKey = model.RowKey;
                DbMenuAction.ActionKey = item.ActionKey;
                DbMenuAction.IsActive = item.IsActive;
            }
        }

        public MenuDetailViewModel DeleteMenuDetailView(MenuDetailViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    List<MenuAction> MenuActionList = dbContext.MenuActions.Where(x => x.MenuKey == model.RowKey).ToList();
                    Menu MenuDetailViewModel = dbContext.Menus.SingleOrDefault(row => row.RowKey == model.RowKey);

                    dbContext.MenuActions.RemoveRange(MenuActionList);
                    dbContext.Menus.Remove(MenuDetailViewModel);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Menu, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Menu);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.Menu, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Menu);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Menu, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }

        public MenuDetailViewModel DeleteMenuAction(MenuDetailViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    MenuAction MenuActionModel = dbContext.MenuActions.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.MenuActions.Remove(MenuActionModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Menu, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Menu);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.Menu, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Menu);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Menu, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }

        public List<MenuDetailViewModel> GetMenuDetailView(MenuDetailViewModel model)
        {
            try
            {
                IQueryable<MenuDetailViewModel> MenuList = (from m in dbContext.Menus
                                                            orderby m.RowKey
                                                            where (m.MenuName.Contains(model.MenuName))
                                                            select new MenuDetailViewModel
                                                            {
                                                                RowKey = m.RowKey,
                                                                MenuTypeName = m.MenuType.MenuTypeName,
                                                                MenuCode = m.MenuCode,
                                                                MenuName = m.MenuName,
                                                                ActionName = m.ActionName,
                                                                ControllerName = m.ControllerName,
                                                                OptionalParameter = m.OptionalParameter,
                                                                IconClassName = m.IconClassName,
                                                                MenucatagoryName = m.MenuCatagory.CatagoryName,
                                                                DisplayOrder = m.DisplayOrder,
                                                                IsActive = m.IsActive,
                                                                MenuTypeKey = m.MenuTypeKey
                                                            });
                if (model.MenuTypeKey != null)
                {
                    MenuList = MenuList.Where(x => x.MenuTypeKey == model.MenuTypeKey);
                }

                return MenuList.ToList().GroupBy(x => x.RowKey).Select(y => y.First()).ToList<MenuDetailViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Menu, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);

                return new List<MenuDetailViewModel>();

            }
        }
    }
}
