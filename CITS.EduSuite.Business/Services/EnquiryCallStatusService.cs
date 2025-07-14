using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System.Data.Entity.Infrastructure;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class EnquiryCallStatusService : IEnquiryCallStatusService
    {
        private EduSuiteDatabase dbContext;

        public EnquiryCallStatusService(EduSuiteDatabase objDb)
        {
            this.dbContext = objDb;
        }
        public List<EnquiryCallStatusViewModel> GetEnquiryCallStatus(string serachText)
        {
            try
            {
                var EnquiryCallStatusList = (from PT in dbContext.EnquiryCallStatus
                                             where (PT.EnquiryCallStatusName.Contains(serachText))
                                             select new EnquiryCallStatusViewModel
                                             {
                                                 RowKey = PT.RowKey,
                                                 EnquiryCallStatusName = PT.EnquiryCallStatusName,
                                                 EnquiryStatusName = PT.EnquiryStatu.EnquiryStatusName,
                                                 IsActive = PT.IsActive

                                             }).ToList();
                return EnquiryCallStatusList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<EnquiryCallStatusViewModel>();

            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.EnquiryCallStatus, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<EnquiryCallStatusViewModel>();

            }

        }

        public EnquiryCallStatusViewModel GetEnquiryCallStatusById(short? id)
        {
            try
            {
                EnquiryCallStatusViewModel model = new EnquiryCallStatusViewModel();
                model = dbContext.EnquiryCallStatus.Select(row => new EnquiryCallStatusViewModel
                {
                    RowKey = row.RowKey,
                    EnquiryCallStatusName = row.EnquiryCallStatusName,
                    EnquiryStatusKey = row.EnquiryStatusKey,
                    IsDuration = row.IsDuration,
                    ShowInMenuKeysList = row.ShowInMenuKeys,
                    IsActive = row.IsActive
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new EnquiryCallStatusViewModel();
                }

                if (model.ShowInMenuKeysList != null && model.ShowInMenuKeysList != "")
                {
                    model.ShowInMenuKeys = model.ShowInMenuKeysList.Split(',').Select(Int32.Parse).ToList();
                }
                FillEnquiryStatus(model);
                FillMenus(model);
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.EnquiryCallStatus, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new EnquiryCallStatusViewModel();

            }
        }

        public EnquiryCallStatusViewModel CreateEnquiryCallStatus(EnquiryCallStatusViewModel model)
        {
            EnquiryCallStatu EnquiryCallStatusmodel = new EnquiryCallStatu();
            var EnquiryCallStatusCheck = dbContext.EnquiryCallStatus.Where(row => row.EnquiryStatusKey == model.EnquiryStatusKey && row.EnquiryCallStatusName.ToLower() == model.EnquiryCallStatusName.ToLower()).Count();
            FillEnquiryStatus(model);
            if (EnquiryCallStatusCheck != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.EnquiryCallStatus);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    int MaxKey = dbContext.EnquiryCallStatus.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    EnquiryCallStatusmodel.RowKey = Convert.ToInt16(MaxKey + 1);
                    EnquiryCallStatusmodel.EnquiryCallStatusName = model.EnquiryCallStatusName;
                    EnquiryCallStatusmodel.EnquiryStatusKey = model.EnquiryStatusKey;
                    EnquiryCallStatusmodel.IsDuration = model.IsDuration;
                    EnquiryCallStatusmodel.IsActive = model.IsActive;
                    EnquiryCallStatusmodel.ShowInMenuKeys = String.Join(",", model.ShowInMenuKeys);
                    dbContext.EnquiryCallStatus.Add(EnquiryCallStatusmodel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EnquiryCallStatus, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.EnquiryCallStatus);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EnquiryCallStatus, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;
        }

        public EnquiryCallStatusViewModel UpdateEnquiryCallStatus(EnquiryCallStatusViewModel model)
        {
            var EnquiryCallStatusCheck = dbContext.EnquiryCallStatus.Where(row => row.EnquiryStatusKey == model.EnquiryStatusKey && row.EnquiryCallStatusName.ToLower() == model.EnquiryCallStatusName.ToLower()
                 && row.RowKey != model.RowKey).ToList();

            EnquiryCallStatu EnquiryCallStatusmodel = new EnquiryCallStatu();
            FillEnquiryStatus(model);
            if (EnquiryCallStatusCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.EnquiryCallStatus);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    EnquiryCallStatusmodel = dbContext.EnquiryCallStatus.SingleOrDefault(x => x.RowKey == model.RowKey);
                    EnquiryCallStatusmodel.EnquiryCallStatusName = model.EnquiryCallStatusName;
                    EnquiryCallStatusmodel.EnquiryStatusKey = model.EnquiryStatusKey;
                    EnquiryCallStatusmodel.IsDuration = model.IsDuration;
                    EnquiryCallStatusmodel.ShowInMenuKeys = String.Join(",", model.ShowInMenuKeys);
                    EnquiryCallStatusmodel.IsActive = model.IsActive;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EnquiryCallStatus, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.EnquiryCallStatus);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EnquiryCallStatus, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;
        }

        public EnquiryCallStatusViewModel DeleteEnquiryCallStatus(EnquiryCallStatusViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    EnquiryCallStatu EnquiryCallStatusmodel = dbContext.EnquiryCallStatus.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.EnquiryCallStatus.Remove(EnquiryCallStatusmodel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EnquiryCallStatus, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);


                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.EnquiryCallStatus);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.EnquiryCallStatus, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);

                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.EnquiryCallStatus);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EnquiryCallStatus, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }

            return model;
        }


        private void FillEnquiryStatus(EnquiryCallStatusViewModel model)
        {
            model.EnquiryStatusList = dbContext.VwEnquiryStatusSelectAlls.OrderBy(row => row.RowKey).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.EnquiryStatusName
            }).ToList();
        }

        private void FillMenus(EnquiryCallStatusViewModel model)
        {
            model.MenuList = dbContext.VwMenuSelectActiveOnlies.Where(
                    x =>
                    x.RowKey == DbConstants.Menu.Enquiry
                 ||
                    x.RowKey == DbConstants.Menu.EnquiryLead
                ).OrderBy(row => row.RowKey).Select(row => new SelectListModel
                {
                    RowKey = row.RowKey,
                    Text = row.MenuName
                }).ToList();
        }

    }
}
