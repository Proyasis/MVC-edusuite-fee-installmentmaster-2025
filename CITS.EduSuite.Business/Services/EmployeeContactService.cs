using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;

namespace CITS.EduSuite.Business.Services
{
    public class EmployeeContactService : IEmployeeContactService
    {
        private EduSuiteDatabase dbContext;

        public EmployeeContactService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public EmployeeContactViewModel GetEmployeeContactById(long EmployeeId)
        {
            try
            {
                EmployeeContactViewModel model = new EmployeeContactViewModel();
                model.EmployeeContacts = dbContext.EmployeeContacts.Where(x => x.EmployeeKey == EmployeeId).Select(ec => new ContactViewModel
                {
                    RowKey = ec.Address.RowKey,
                    AddressTypeKey = ec.Address.AddressTypeKey,
                    AddressLine1 = ec.Address.AddressLine1,
                    AddressLine2 = ec.Address.AddressLine2,
                    AddressLine3 = ec.Address.AddressLine3,
                    CityName = ec.Address.CityName,
                    DistrictKey = ec.Address.DistrictKey,
                    ProvinceKey = ec.Address.District.ProvinceKey,
                    CountryKey = ec.Address.District.Province.CountryKey,
                    PostalCode = ec.Address.PostalCode,

                }).ToList();
                if (model.EmployeeContacts.Count == 0)
                {
                    model.EmployeeContacts.Add(new ContactViewModel());
                }
                if (model == null)
                {
                    model = new EmployeeContactViewModel();
                }
                model.EmployeeKey = EmployeeId;
                FillDropdownLists(model.EmployeeContacts);
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.EmployeeContact, ActionConstants.View, DbConstants.LogType.Error, EmployeeId, ex.GetBaseException().Message);
                return new EmployeeContactViewModel();
                
            }
        }

        public EmployeeContactViewModel UpdateEmployeeContact(EmployeeContactViewModel model)
        {
            FillDropdownLists(model.EmployeeContacts);

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    CreateContact(model.EmployeeContacts.Where(row => row.RowKey == 0).ToList(), model.EmployeeKey);
                    UpdateContact(model.EmployeeContacts.Where(row => row.RowKey != 0).ToList(), model.EmployeeKey);

                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeContact, (model.EmployeeContacts.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Info, model.EmployeeKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.EmployeeContacts);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeContact, (model.EmployeeContacts.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, model.EmployeeKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        private void CreateContact(List<ContactViewModel> modelList, Int64 EmployeeKey)
        {

            Int64 maxAddressKey = dbContext.Addresses.Select(p => p.RowKey).DefaultIfEmpty().Max();
            Int64 maxEmployeeContactKey = dbContext.EmployeeContacts.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (ContactViewModel model in modelList)
            {

                Address addressModel = new Address();
                EmployeeContact employeeContactModel = new EmployeeContact();

                addressModel.RowKey = Convert.ToInt64(maxAddressKey + 1);
                addressModel.AddressTypeKey = model.AddressTypeKey;
                addressModel.AddressLine1 = model.AddressLine1;
                addressModel.AddressLine2 = model.AddressLine2;
                addressModel.AddressLine3 = model.AddressLine3;
                addressModel.CityName = model.CityName;
                addressModel.DistrictKey = model.DistrictKey;
                addressModel.PostalCode = model.PostalCode;
             
                dbContext.Addresses.Add(addressModel);


                employeeContactModel.RowKey = Convert.ToInt64(maxEmployeeContactKey + 1);
                employeeContactModel.AddressKey = addressModel.RowKey;
                employeeContactModel.EmployeeKey = EmployeeKey;
                dbContext.EmployeeContacts.Add(employeeContactModel);
                maxAddressKey++;
                maxEmployeeContactKey++;

            }

        }

        public void UpdateContact(List<ContactViewModel> modelList, Int64 EmployeeKey)
        {

            foreach (ContactViewModel model in modelList)
            {

                Address addressModel = new Address();
                addressModel = dbContext.Addresses.SingleOrDefault(row => row.RowKey == model.RowKey);
                addressModel.AddressTypeKey = model.AddressTypeKey;
                addressModel.AddressLine1 = model.AddressLine1;
                addressModel.AddressLine2 = model.AddressLine2;
                addressModel.AddressLine3 = model.AddressLine3;
                addressModel.CityName = model.CityName;
                addressModel.DistrictKey = model.DistrictKey;
                addressModel.PostalCode = model.PostalCode;
               
            }
        }

        public EmployeeContactViewModel DeleteEmployeeContact(ContactViewModel model)
        {
            EmployeeContactViewModel employeeContactViewModel = new EmployeeContactViewModel();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    EmployeeContact employeeContact = dbContext.EmployeeContacts.SingleOrDefault(row => row.AddressKey == model.RowKey);
                    dbContext.EmployeeContacts.Remove(employeeContact);
                    Address address = dbContext.Addresses.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.Addresses.Remove(address); 
                    dbContext.SaveChanges();
                    transaction.Commit();
                    employeeContactViewModel.Message = EduSuiteUIResources.Success;
                    employeeContactViewModel.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeContact, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, employeeContactViewModel.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        employeeContactViewModel.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.EmployeeContacts);
                        employeeContactViewModel.IsSuccessful = false;
                           ActivityLog.CreateActivityLog(MenuConstants.EmployeeContact, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    employeeContactViewModel.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.EmployeeContacts);
                    employeeContactViewModel.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeContact, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return employeeContactViewModel;
        }


        public ContactViewModel GetProvinceByCountry(ContactViewModel model)
        {
            model.Provinces = dbContext.VwProvinceSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Where(row => row.CountryKey == model.CountryKey).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.Provincename
            }).ToList();
            return model;
        }

        public ContactViewModel GetDistrictByProvince(ContactViewModel model)
        {
            model.Districts = dbContext.VwDistrictSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Where(row => row.ProvinceKey == model.ProvinceKey).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.DistrictName
            }).ToList();

            return model;
        }

        public EmployeeContactViewModel CheckAddressTypeExists(Int16 AddressTypeKey, Int64 EmployeeKey, Int64 RowKey)
        {
            EmployeeContactViewModel model = new EmployeeContactViewModel();
            if (dbContext.EmployeeContacts.Where(row => row.Address.AddressTypeKey == AddressTypeKey && row.EmployeeKey == EmployeeKey && row.RowKey != RowKey).Any())
            {
                model.IsSuccessful = false;

            }
            else
            {
                model.IsSuccessful = true;
            }
            return model;
        }

     

        private void FillDropdownLists(List<ContactViewModel> modelList)
        {
            foreach (ContactViewModel model in modelList)
            {
                FillAddressTypes(model);
                FillCountries(model);
                GetProvinceByCountry(model);
                GetDistrictByProvince(model);
            }
        }

        private void FillAddressTypes(ContactViewModel model)
        {
            model.AddressTypes = dbContext.VwAddressTypeSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AddressTypeName
            }).ToList();
        }

        private void FillCountries(ContactViewModel model)
        {
            model.Countries = dbContext.VwCountrySelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.CountryName
            }).ToList();
        }

    }
}
