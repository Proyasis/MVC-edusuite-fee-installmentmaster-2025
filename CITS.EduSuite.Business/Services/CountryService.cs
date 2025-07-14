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
    public class CountryService : ICountryService
    {


        private EduSuiteDatabase dbContext;

        public CountryService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<CountryViewModel> GetCountries(string searchText)
        {

            try
            {
                var countriesList = (from c in dbContext.Countries
                                         //orderby e.RowKey
                                     where (c.CountryName.Contains(searchText))
                                     select new CountryViewModel
                                     {
                                         RowKey = c.RowKey,
                                         CountryName = c.CountryName,
                                         CountryNameLocal = c.CountryNameLocal,
                                         CountryShortName = c.CountryShortName,
                                         NationalityName = c.NationalityName,
                                         CapitalCityName = c.CapitalCity,
                                         LanguageName = c.Language.LanguageName,
                                         TelephoneCode = c.TelephoneCode,
                                         DisplayOrder = c.DisplayOrder,
                                         IsActiveText = c.IsActive == true ? ApplicationResources.Yes : ApplicationResources.No
                                     }).ToList();

                return countriesList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<CountryViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Country, ActionConstants.MenuAccess, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return new List<CountryViewModel>();
            }
        }




        public CountryViewModel GetCountryById(short? Id)
        {
            try
            {
                CountryViewModel model = new CountryViewModel();
                model = dbContext.Countries.Select(row => new CountryViewModel
                {
                    RowKey = row.RowKey,
                    CountryName = row.CountryName,
                    CountryNameLocal = row.CountryNameLocal,
                    CountryShortName = row.CountryShortName,
                    LanguageKey = row.LanguageKey,
                    TelephoneCode = row.TelephoneCode,
                    DisplayOrder = row.DisplayOrder,
                    NationalityName = row.NationalityName,
                    CapitalCityName = row.CapitalCity,
                    CurrencyKey = row.CurrencyKey,
                    IsActive = row.IsActive
                }).Where(x => x.RowKey == Id).FirstOrDefault();
                if (model == null)
                {
                    model = new CountryViewModel();
                }
                FillDropdownLists(model);
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Country, ((Id ?? 0) != 0 ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, Id, ex.GetBaseException().Message);
                return new CountryViewModel();
            }
        }

        public CountryViewModel CreateCountry(CountryViewModel model)
        {
            Country contryModel = new Country();
            var contryCheck = dbContext.Countries.Where(row => row.CountryName.ToLower() == model.CountryName.ToLower()).ToList();
            CountryViewModel countryModel = new CountryViewModel();
            FillDropdownLists(model);
            if (contryCheck.Count != 0)
            {
                model.Message = ApplicationResources.ErrorCountryExists;
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {

                try
                {


                    //Add  details to Country table.
                    Int16 maxKey = dbContext.Countries.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    contryModel.RowKey = Convert.ToInt16(maxKey + 1);
                    contryModel.CountryName = model.CountryName;
                    contryModel.CountryNameLocal = model.CountryNameLocal;
                    contryModel.CountryShortName = model.CountryShortName;
                    contryModel.NationalityName = model.NationalityName;
                    contryModel.CapitalCity = model.CapitalCityName;
                    contryModel.LanguageKey = model.LanguageKey;
                    contryModel.TelephoneCode = model.TelephoneCode;
                    contryModel.IsActive = model.IsActive;
                    contryModel.DisplayOrder = Convert.ToInt16(maxKey + 1);
                    contryModel.CurrencyKey = model.CurrencyKey;
                    dbContext.Countries.Add(contryModel);




                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = ApplicationResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Country, ActionConstants.Add, DbConstants.LogType.Info, countryModel.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = ApplicationResources.FailedToSaveCountry;
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Country, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public CountryViewModel UpdateCountry(CountryViewModel model)
        {
            Country contryModel = new Country();
            var contryCheck = dbContext.Countries.Where(row => row.CountryName.ToLower() == model.CountryName.ToLower() && row.RowKey != model.RowKey).ToList();
            CountryViewModel countryModel = new CountryViewModel();
            FillDropdownLists(model);
            if (contryCheck.Count != 0)
            {
                model.Message = ApplicationResources.ErrorCountryExists;
                model.IsSuccessful = false;
                return model;
            }
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    contryModel = dbContext.Countries.SingleOrDefault(row => row.RowKey == model.RowKey);
                    contryModel.CountryName = model.CountryName;
                    contryModel.CountryNameLocal = model.CountryNameLocal;
                    contryModel.CountryShortName = model.CountryShortName;
                    contryModel.NationalityName = model.NationalityName;
                    contryModel.CapitalCity = model.CapitalCityName;
                    contryModel.LanguageKey = model.LanguageKey;
                    contryModel.TelephoneCode = model.TelephoneCode;
                    contryModel.IsActive = model.IsActive;
                    contryModel.LanguageKey = model.LanguageKey;
                    contryModel.CurrencyKey = model.CurrencyKey;
                    // contryModel.DisplayOrder = Convert.ToInt16(maxKey + 1);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = ApplicationResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Country, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = ApplicationResources.FailedToSaveCountry;
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Country, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public CountryViewModel DeleteCountry(CountryViewModel model)
        {


            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Country country = dbContext.Countries.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.Countries.Remove(country);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = ApplicationResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Country, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey,model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(ApplicationResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = ApplicationResources.CantDeleteCountry;
                        model.IsSuccessful = false;
                    }
                    ActivityLog.CreateActivityLog(MenuConstants.Country, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = ApplicationResources.FailedToDeleteCountry;
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Country, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            //}
            return model;
        }


        private void FillDropdownLists(CountryViewModel model)
        {
            FillLanguages(model);
            FillCurrencies(model);
        }
        private void FillLanguages(CountryViewModel model)
        {
            model.LanguageNames = dbContext.VwLanguageSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.LanguageName
            }).ToList();
        }
        private void FillCurrencies(CountryViewModel model)
        {
            model.Currencies = dbContext.Currencies.Where(Row => Row.IsActive).OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.CurrencyName
            }).ToList();
        }
    }
}

