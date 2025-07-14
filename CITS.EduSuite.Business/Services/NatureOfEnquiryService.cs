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
    public class NatureOfEnquiryService : INatureOfEnquiryService
    {
        private EduSuiteDatabase dbContext;
        public NatureOfEnquiryService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public NatureOfEnquiryViewModel GetNatureOfEnquiryById(int? id)
        {
            try
            {
                NatureOfEnquiryViewModel model = new NatureOfEnquiryViewModel();
                model = dbContext.NatureOfEnquiries.Select(row => new NatureOfEnquiryViewModel
                {
                    RowKey = row.RowKey,
                    NatureOfEnquiryName = row.NatureOfEnquiryName,

                    IsActive = row.IsActive
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new NatureOfEnquiryViewModel();
                }
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.NatureOfEnquiry, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new NatureOfEnquiryViewModel();

            }
        }
        public NatureOfEnquiryViewModel CreateNatureOfEnquiry(NatureOfEnquiryViewModel model)
        {
            NatureOfEnquiry NatureOfEnquiryModel = new NatureOfEnquiry();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    long MaxKey = dbContext.NatureOfEnquiries.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    NatureOfEnquiryModel.RowKey = Convert.ToInt16(MaxKey + 1);
                    NatureOfEnquiryModel.NatureOfEnquiryName = model.NatureOfEnquiryName;
                    NatureOfEnquiryModel.DateAdded = DateTimeUTC.Now;
                    NatureOfEnquiryModel.IsActive = model.IsActive;
                    dbContext.NatureOfEnquiries.Add(NatureOfEnquiryModel);
                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.NatureOfEnquiry, ActionConstants.Add, DbConstants.LogType.Info, NatureOfEnquiryModel.RowKey, model.Message);


                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.NatureOfEnquiry);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.NatureOfEnquiry, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public NatureOfEnquiryViewModel UpdateNatureOfEnquiry(NatureOfEnquiryViewModel model)
        {
            NatureOfEnquiry NatureOfEnquiryModel = new NatureOfEnquiry();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    NatureOfEnquiryModel = dbContext.NatureOfEnquiries.SingleOrDefault(x => x.RowKey == model.RowKey);
                    NatureOfEnquiryModel.NatureOfEnquiryName = model.NatureOfEnquiryName;

                    NatureOfEnquiryModel.IsActive = model.IsActive;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.NatureOfEnquiry, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.NatureOfEnquiry);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.NatureOfEnquiry, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;

        }
        public NatureOfEnquiryViewModel DeleteNatureOfEnquiry(NatureOfEnquiryViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    NatureOfEnquiry NatureOfEnquiryModel = dbContext.NatureOfEnquiries.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.NatureOfEnquiries.Remove(NatureOfEnquiryModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.NatureOfEnquiry, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);


                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.NatureOfEnquiry);
                        model.IsSuccessful = false;

                        ActivityLog.CreateActivityLog(MenuConstants.NatureOfEnquiry, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.NatureOfEnquiry);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.NatureOfEnquiry, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }
        public List<NatureOfEnquiryViewModel> GetNatureOfEnquiry(string searchText)
        {
            try
            {
                var NatureOfEnquiryList = (from p in dbContext.NatureOfEnquiries
                                    orderby p.RowKey descending
                                    where (p.NatureOfEnquiryName.Contains(searchText))
                                    select new NatureOfEnquiryViewModel
                                    {
                                        RowKey = p.RowKey,
                                        NatureOfEnquiryName = p.NatureOfEnquiryName,
                                        //IsActiveText = p.IsActive == true ? ApplicationResources.Yes : ApplicationResources.No
                                        IsActive = p.IsActive,
                                    }).ToList();
                return NatureOfEnquiryList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<NatureOfEnquiryViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.NatureOfEnquiry, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<NatureOfEnquiryViewModel>();

            }
        }

        public NatureOfEnquiryViewModel CheckNatureOfEnquiryNameExists(NatureOfEnquiryViewModel model)
        {
            if (dbContext.NatureOfEnquiries.Where(x => x.NatureOfEnquiryName.ToLower() == model.NatureOfEnquiryName.ToLower() && x.RowKey != model.RowKey).Any())
            {
                model.IsSuccessful = false;
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.NatureOfEnquiry);
            }
            else
            {
                model.IsSuccessful = true;
                model.Message = "";
            }
            return model;
        }

    }
}
