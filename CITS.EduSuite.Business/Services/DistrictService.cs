using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Data;
using System.Data.Entity.Infrastructure;

namespace CITS.EduSuite.Business.Services
{
    public class DistrictService : IDistrictService
    {
        private EduSuiteDatabase dbContext;
        public DistrictService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<DistrictViewModel> GetDistricts(string searchText)
        {
            try
            {
                var districtsList = (from d in dbContext.Districts
                                         //orderby e.RowKey
                                     where (d.DistrictName.Contains(searchText))
                                     select new DistrictViewModel
                                     {
                                         RowKey = d.RowKey,
                                         Districtname = d.DistrictName,
                                         DistrictnameLocal = d.DistrictNameLocal,
                                         ProvinceName = d.Province.Provincename,
                                         CountryName = d.Province.Country.CountryName,
                                         // DisplayOrder = d.DisplayOrder,
                                         IsActiveText = d.IsActive == true ? ApplicationResources.Yes : ApplicationResources.No
                                     }).ToList();

                return districtsList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<DistrictViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.District, ActionConstants.MenuAccess, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return new List<DistrictViewModel>();
            }
        }

        public DistrictViewModel GetDistrictById(int? id)
        {
            try
            {
                DistrictViewModel model = new DistrictViewModel();
                model = dbContext.Districts.Select(row => new DistrictViewModel
                {
                    RowKey = row.RowKey,
                    CountryKey = row.Province.CountryKey,
                    ProvinceKey = row.ProvinceKey,
                    Districtname = row.DistrictName,
                    DistrictnameLocal = row.DistrictNameLocal,
                    // DisplayOrder = row.DisplayOrder,                

                    IsActive = row.IsActive
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new DistrictViewModel();
                }
                FillDropdownLists(model);
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.District, ((id ?? 0) != 0 ? ActionConstants.Add : ActionConstants.Edit), DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new DistrictViewModel();
            }
        }
        public DistrictViewModel CreateDistrict(DistrictViewModel model)
        {
            District DistrictModel = new District();
            var DistrictnameCheck = dbContext.Districts.Where(row => row.DistrictName.ToLower() == model.Districtname.ToLower()).ToList();
            FillDropdownLists(model);
            if (DistrictnameCheck.Count != 0)
            {
                model.Message = ApplicationResources.ErrorDistrictExists;
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {

                try
                {


                    //Add  details to District table.
                    int maxKey = dbContext.Districts.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    DistrictModel.RowKey = maxKey + 1;
                    DistrictModel.DistrictName = model.Districtname;
                    DistrictModel.DistrictNameLocal = model.DistrictnameLocal;
                    DistrictModel.ProvinceKey = model.ProvinceKey;
                    DistrictModel.IsActive = model.IsActive;
                    DistrictModel.DisplayOrder = maxKey + 1;
                    dbContext.Districts.Add(DistrictModel);
                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = ApplicationResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.District, ActionConstants.Add, DbConstants.LogType.Info, DistrictModel.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = ApplicationResources.FailedToSaveDistrict;
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.District, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public DistrictViewModel UpdateDistrict(DistrictViewModel model)
        {
            District DistrictModel = new District();
            var districtCheck = dbContext.Districts.Where(row => row.DistrictName.ToLower() == model.Districtname.ToLower() && row.RowKey != model.RowKey).ToList();

            FillDropdownLists(model);
            if (districtCheck.Count != 0)
            {
                model.Message = ApplicationResources.ErrorDistrictExists;
                model.IsSuccessful = false;
                return model;
            }
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    DistrictModel = dbContext.Districts.SingleOrDefault(row => row.RowKey == model.RowKey);
                    DistrictModel.DistrictName = model.Districtname;
                    DistrictModel.DistrictNameLocal = model.DistrictnameLocal;
                    DistrictModel.ProvinceKey = model.ProvinceKey;
                    DistrictModel.IsActive = model.IsActive;
                    // DistrictModel.DisplayOrder = maxKey + 1;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = ApplicationResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.District, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = ApplicationResources.FailedToSaveDistrict;
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.District, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public DistrictViewModel DeleteDistrict(DistrictViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    District DistrictModel = dbContext.Districts.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.Districts.Remove(DistrictModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = ApplicationResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.District, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(ApplicationResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = ApplicationResources.CantDeleteDistrict;
                        model.IsSuccessful = false;
                    }
                    ActivityLog.CreateActivityLog(MenuConstants.District, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = ApplicationResources.FailedToDeleteDistrict;
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.District, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }

            }
            return model;
        }

        public DistrictViewModel GetProvinceByCountry(short CountryKey)
        {
            DistrictViewModel model = new DistrictViewModel();
            model.CountryKey = CountryKey;
            FillProvinceById(model);
            return model;
        }

        private void FillDropdownLists(DistrictViewModel model)
        {
            FillCountries(model);
            FillProvinceById(model);

        }
        private void FillCountries(DistrictViewModel model)
        {
            model.Countries = dbContext.VwCountrySelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.CountryName
            }).ToList();
        }

        private void FillProvinceById(DistrictViewModel model)
        {
            model.Provinces = dbContext.VwProvinceSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Where(row => row.CountryKey == model.CountryKey).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.Provincename
            }).ToList();
        }
    }
}




