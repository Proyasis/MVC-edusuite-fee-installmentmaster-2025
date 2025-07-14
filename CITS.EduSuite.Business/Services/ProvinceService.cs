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

namespace CITS.EduSuite.Business.Services
{
    public class ProvinceService : IProvinceService
    {

        private EduSuiteDatabase dbContext;
        public ProvinceService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<ProvinceViewModel> GetProvinces(string searchText)
        {
            try
            {
                var provincesList = (from p in dbContext.Provinces
                                         //orderby e.RowKey
                                     where (p.Provincename.Contains(searchText))
                                     select new ProvinceViewModel
                                     {
                                         RowKey = p.RowKey,
                                         Provincename = p.Provincename,
                                         ProvincenameLocal = p.ProvincenameLocal,
                                         CountryName = p.Country.CountryName,
                                         LanguageName = p.Language.LanguageName,
                                         // DisplayOrder = p.DisplayOrder,
                                         IsActiveText = p.IsActive == true ? ApplicationResources.Yes : ApplicationResources.No
                                     }).ToList();

                return provincesList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<ProvinceViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Province, ActionConstants.MenuAccess, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return new List<ProvinceViewModel>();
            }
        }


        public ProvinceViewModel GetProvinceById(int? id)
        {
            try
            {
                ProvinceViewModel model = new ProvinceViewModel();
                model = dbContext.Provinces.Select(row => new ProvinceViewModel
                {
                    RowKey = row.RowKey,
                    CountryKey = row.CountryKey,
                    Provincename = row.Provincename,
                    ProvincenameLocal = row.ProvincenameLocal,
                    // DisplayOrder = row.DisplayOrder,                

                    LanguageKey = row.LanguageKey,
                    IsActive = row.IsActive
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new ProvinceViewModel();
                }
                FillDropdownLists(model);
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Province, ((id ?? 0) != 0 ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new ProvinceViewModel();
            }
        }


        public ProvinceViewModel CreateProvince(ProvinceViewModel model)
        {
            Province provinceModel = new Province();
            var ProvincenameCheck = dbContext.Provinces.Where(row => row.Provincename.ToLower() == model.Provincename.ToLower()).ToList();
            FillDropdownLists(model);
            if (ProvincenameCheck.Count != 0)
            {
                model.Message = ApplicationResources.ErrorProvinceExists;
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {

                try
                {


                    //Add  details to Province table.
                    int maxKey = dbContext.Provinces.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    provinceModel.RowKey = maxKey + 1;
                    provinceModel.Provincename = model.Provincename;
                    provinceModel.ProvincenameLocal = model.ProvincenameLocal;
                    provinceModel.CountryKey = model.CountryKey;
                    provinceModel.LanguageKey = model.LanguageKey;

                    provinceModel.IsActive = model.IsActive;
                    provinceModel.DisplayOrder = maxKey + 1;

                    dbContext.Provinces.Add(provinceModel);




                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = ApplicationResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Province, ActionConstants.Add, DbConstants.LogType.Info, provinceModel.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = ApplicationResources.FailedToSaveProvince;
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Province, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;
        }

        public ProvinceViewModel UpdateProvince(ProvinceViewModel model)
        {
            Province provinceModel = new Province();
            var contryCheck = dbContext.Provinces.Where(row => row.Provincename.ToLower() == model.Provincename.ToLower() && row.RowKey != model.RowKey).ToList();

            FillDropdownLists(model);
            if (contryCheck.Count != 0)
            {
                model.Message = ApplicationResources.ErrorProvinceExists;
                model.IsSuccessful = false;
                return model;
            }
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    provinceModel = dbContext.Provinces.SingleOrDefault(row => row.RowKey == model.RowKey);
                    provinceModel.Provincename = model.Provincename;
                    provinceModel.ProvincenameLocal = model.ProvincenameLocal;
                    provinceModel.CountryKey = model.CountryKey;
                    provinceModel.LanguageKey = model.LanguageKey;
                    provinceModel.IsActive = model.IsActive;
                    // ProvinceModel.DisplayOrder = maxKey + 1;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = ApplicationResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Province, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = ApplicationResources.FailedToSaveProvince;
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Province, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;
        }
        public ProvinceViewModel DeleteProvince(ProvinceViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Province provinceModel = dbContext.Provinces.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.Provinces.Remove(provinceModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = ApplicationResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Province, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(ApplicationResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = ApplicationResources.CantDeleteProvince;
                        model.IsSuccessful = false;
                    }
                    ActivityLog.CreateActivityLog(MenuConstants.Province, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = ApplicationResources.FailedToDeleteProvince;
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Province, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            //}
            return model;
        }
        private void FillDropdownLists(ProvinceViewModel model)
        {
            FillLanguages(model);
            FillCountries(model);
        }
        private void FillLanguages(ProvinceViewModel model)
        {
            model.LanguageNames = dbContext.VwLanguageSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.LanguageName
            }).ToList();
        }
        private void FillCountries(ProvinceViewModel model)
        {
            model.CountryNames = dbContext.VwCountrySelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.CountryName
            }).ToList();
        }
    }
}
