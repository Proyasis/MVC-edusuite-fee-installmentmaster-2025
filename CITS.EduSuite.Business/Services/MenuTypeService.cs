using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;

namespace CITS.EduSuite.Business.Services
{
    public class MenuTypeService : IMenuTypeService
    {
        private EduSuiteDatabase dbContext;
        public MenuTypeService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<MenuTypeViewModel> GetMenuType(string searchText)
        {
            try
            {
                var MenuTypeList = (from p in dbContext.MenuTypes
                                    orderby p.RowKey descending
                                    where (p.MenuTypeName.Contains(searchText))
                                    select new MenuTypeViewModel
                                    {
                                        RowKey = p.RowKey,
                                        MenuTypeName = p.MenuTypeName,
                                        DisplayOrder = p.DisplayOrder,
                                        IconCLassName = p.IconCLassName,
                                        IsActive = p.IsActive

                                    }).ToList();
                return MenuTypeList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<MenuTypeViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.MenuType, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<MenuTypeViewModel>();


            }
        }
        public MenuTypeViewModel GetMenuTypeById(int? id)
        {
            try
            {
                MenuTypeViewModel model = new MenuTypeViewModel();
                model = dbContext.MenuTypes.Select(row => new MenuTypeViewModel
                {
                    RowKey = row.RowKey,
                    MenuTypeName = row.MenuTypeName,
                    DisplayOrder = row.DisplayOrder,
                    IconCLassName = row.IconCLassName,
                    IsActive = row.IsActive
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new MenuTypeViewModel();
                }
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.MenuType, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new MenuTypeViewModel();

            }
        }
        public MenuTypeViewModel CreateMenuType(MenuTypeViewModel model)
        {
            var MenuTypeCheck = dbContext.MenuTypes.Where(row => row.MenuTypeName.ToLower() == model.MenuTypeName.ToLower()).Count();
            MenuType MenuTypeModel = new MenuType();
            if (MenuTypeCheck != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.MenuType);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    int MaxKey = dbContext.MenuTypes.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    MenuTypeModel.RowKey = Convert.ToInt16(MaxKey + 1);
                    MenuTypeModel.MenuTypeName = model.MenuTypeName;
                    MenuTypeModel.IconCLassName = model.IconCLassName;
                    MenuTypeModel.DisplayOrder = model.DisplayOrder ?? 0;
                    MenuTypeModel.IsActive = model.IsActive;
                    dbContext.MenuTypes.Add(MenuTypeModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.MenuType, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.MenuType);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.MenuType, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public MenuTypeViewModel UpdateMenuType(MenuTypeViewModel model)
        {
            var MenuTypeCheck = dbContext.MenuTypes.Where(row => row.MenuTypeName.ToLower() == model.MenuTypeName.ToLower()
               && row.RowKey != model.RowKey).ToList();

            MenuType MenuTypeModel = new MenuType();
            if (MenuTypeCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.MenuType);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    MenuTypeModel = dbContext.MenuTypes.SingleOrDefault(x => x.RowKey == model.RowKey);
                    MenuTypeModel.MenuTypeName = model.MenuTypeName;
                    MenuTypeModel.MenuTypeName = model.MenuTypeName;
                    MenuTypeModel.IconCLassName = model.IconCLassName;
                    MenuTypeModel.DisplayOrder = model.DisplayOrder ?? 0;
                    MenuTypeModel.IsActive = model.IsActive;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.MenuType, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.MenuType);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.MenuType, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;

        }
        public MenuTypeViewModel DeleteMenuType(MenuTypeViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    MenuType MenuTypeModel = dbContext.MenuTypes.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.MenuTypes.Remove(MenuTypeModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.MenuType, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.MenuType);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.MenuType, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.MenuType);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.MenuType, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }
    }
}
