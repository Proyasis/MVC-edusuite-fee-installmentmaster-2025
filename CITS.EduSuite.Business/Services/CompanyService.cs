using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Services
{
    public class CompanyService : ICompanyService
    {
        private EduSuiteDatabase dbContext;
        public CompanyService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public List<CompanyViewModel> GetCompanies(string searchText)
        {
            try
            {
                var CompanysList = (from b in dbContext.Companies
                                    orderby b.RowKey descending
                                    where (b.CompanyName.Contains(searchText))
                                    select new CompanyViewModel
                                    {
                                        RowKey = b.RowKey,
                                        CompanyName = b.CompanyName,
                                        CompanySubName = b.CompanySubName,
                                        Website = b.Website,
                                        HasMultipleBranches = b.HasMultipleBranches,

                                        CompanyLogo = b.CompanyLogo,
                                        CompanyLogoPath = b.CompanyLogo != null ? UrlConstants.CompanyLogo + "/" + b.CompanyLogo : EduSuiteUIResources.DefaultPhotoUrl,
                                    }).ToList();

                return CompanysList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<CompanyViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Company, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<CompanyViewModel>();
            }
        }
        public CompanyViewModel GetCompanyById(short? id)
        {
            try
            {
                CompanyViewModel model = new CompanyViewModel();
                model = dbContext.Companies.Select(row => new CompanyViewModel
                {
                    RowKey = row.RowKey,
                    CompanyName = row.CompanyName,
                    CompanySubName = row.CompanySubName,
                    Website = row.Website,
                    HasMultipleBranches = row.HasMultipleBranches,
                    CompanyLogo = row.CompanyLogo,
                    CompanyLogoPath = row.CompanyLogo != null ? UrlConstants.CompanyLogo + "/" + row.CompanyLogo : EduSuiteUIResources.DefaultPhotoUrl,
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new CompanyViewModel();
                }

                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Company, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new CompanyViewModel();
            }
        }
        public CompanyViewModel UpdateCompany(CompanyViewModel model)
        {

            Company companyModel = new Company();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    companyModel = dbContext.Companies.SingleOrDefault(row => row.RowKey == model.RowKey);
                    companyModel.CompanyName = model.CompanyName;
                    companyModel.CompanySubName = model.CompanySubName;
                    companyModel.Website = model.Website;
                    companyModel.HasMultipleBranches = model.HasMultipleBranches;

                    if (model.PhotoFile != null)
                    {
                        string Extension = Path.GetExtension(model.PhotoFile.FileName);
                        string FileName = model.CompanyName + Extension;
                        companyModel.CompanyLogo = FileName;
                    }

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Company, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Company);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Company, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public CompanyViewModel DeleteCompanyLogo(short Id)
        {
            CompanyViewModel model = new CompanyViewModel();
            Company Company = new Company();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    Company = dbContext.Companies.SingleOrDefault(row => row.RowKey == Id);
                    model.CompanyLogo = Company.CompanyLogo;
                    model.CompanyLogoPath = UrlConstants.CompanyLogo + "/" + Company.CompanyLogo;
                    model.RowKey = Company.RowKey;
                    model.CompanyName = Company.CompanyName;
                    Company.CompanyLogo = null;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Company, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Company);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Company, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
    }
}
