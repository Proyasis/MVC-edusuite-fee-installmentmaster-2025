using CITS.EduSuite.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using CITS.EduSuite.Business.Common;
using System.Text.RegularExpressions;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class NotificationTemplateService : INotificationTemplateService
    {
        private EduSuiteDatabase dbContext;
        public NotificationTemplateService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<NotificationTemplateViewModel> GetNotificationTemplates(string searchText)
        {
            try
            {
                var NotificationTemplateList = (from p in dbContext.NotificationTemplates
                                                orderby p.RowKey
                                                where (p.NotificationTemplateName.Contains(searchText))
                                                select new NotificationTemplateViewModel
                                                {
                                                    RowKey = p.RowKey,
                                                    NotificationTemplateName = p.NotificationTemplateName,
                                                    AutoSMSText = p.AutoSMS != null ? (p.AutoSMS == true ? EduSuiteUIResources.Automatic : EduSuiteUIResources.Manual) : EduSuiteUIResources.Off,
                                                    AutoEmailText = p.AutoEmail != null ? (p.AutoEmail == true ? EduSuiteUIResources.Automatic : EduSuiteUIResources.Manual) : EduSuiteUIResources.Off,
                                                }).ToList();
                return NotificationTemplateList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<NotificationTemplateViewModel>();
            }
            catch (Exception ex)
            {
                return new List<NotificationTemplateViewModel>();
            }
        }

        public NotificationTemplateViewModel GetNotificationTemplateById(int id)
        {
            try
            {
                NotificationTemplateViewModel model = new NotificationTemplateViewModel();
                model = dbContext.NotificationTemplates.Where(row => row.RowKey == id).Select(row => new NotificationTemplateViewModel
                {
                    NotificationTemplateName = row.NotificationTemplateName,
                    RowKey = row.RowKey,
                    EmailTemplateFileName = row.EmailTemplateFileName,
                    SMSTemplate = row.SMSTemplate,
                    NotificationColumnGroupKeys = row.NotificationColumnGroupKeys,
                    EmailSubject = row.EmailSubject,
                    AutoEmailValue = row.AutoEmail == null ? 0 : (row.AutoEmail == false ? 1 : 2),
                    AutoSMSValue = row.AutoSMS == null ? 0 : (row.AutoSMS == false ? 1 : 2),
                    GuardianSMSValue = row.GuardianSMS == null ? 0 : (row.GuardianSMS == false ? 1 : 2),
                    GuardianSMSTemplate = row.GuardianSMSTemplate,
                    SMSTemplateID = row.SMSTemplateID,
                    SMSTemplateName = row.SMSTemplateName,
                    SMSTemplateContent = row.SMSTemplateContent,
                }).FirstOrDefault();
                if (model == null)
                {
                    model = new NotificationTemplateViewModel();
                }
                FillDropdownLists(model);
                return model;
            }
            catch (Exception ex)
            {
                NotificationTemplateViewModel model = new NotificationTemplateViewModel();
                return model;
            }
        }

        public NotificationTemplateViewModel UpdateSMSNotification(NotificationTemplateViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    NotificationTemplate NotififcationTemplateModel = dbContext.NotificationTemplates.SingleOrDefault(row => row.RowKey == model.RowKey);
                    Regex regex = new Regex("\\<[^\\>]*\\>");
                    model.SMSTemplate = regex.Replace(model.SMSTemplate, String.Empty);
                    regex = new Regex("&nbsp;|\r\n");
                    model.SMSTemplate = regex.Replace(model.SMSTemplate, " ");
                    NotififcationTemplateModel.SMSTemplate = model.SMSTemplate;
                    if (model.AutoSMSValue == 0)
                    {
                        NotififcationTemplateModel.AutoSMS = null;
                    }
                    else
                    {
                        NotififcationTemplateModel.AutoSMS = model.AutoSMSValue == 1 ? false : true;
                    }
                    NotififcationTemplateModel.SMSTemplateID = model.SMSTemplateID;
                    NotififcationTemplateModel.SMSTemplateName = model.SMSTemplateName;
                    NotififcationTemplateModel.SMSTemplateContent = model.SMSTemplateContent;

                    Regex regexGuardian = new Regex("\\<[^\\>]*\\>");
                    model.GuardianSMSTemplate = regexGuardian.Replace(model.SMSTemplate, String.Empty);
                    regex = new Regex("&nbsp;|\r\n");
                    model.GuardianSMSTemplate = regex.Replace(model.GuardianSMSTemplate, " ");
                    NotififcationTemplateModel.GuardianSMSTemplate = model.GuardianSMSTemplate;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    //model.Message = EduSuiteUIResources.FailedToSaveNotificationTemplate;
                    model.IsSuccessful = false;
                }
            }
            FillDropdownLists(model);
            return model;
        }

        public NotificationTemplateViewModel UpdateEmailNotification(NotificationTemplateViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    NotificationTemplate NotififcationTemplateModel = dbContext.NotificationTemplates.SingleOrDefault(row => row.RowKey == model.RowKey);
                    if (model.AutoEmailValue == 0)
                    {
                        NotififcationTemplateModel.AutoEmail = null;
                    }
                    else
                    {
                        NotififcationTemplateModel.AutoEmail = model.AutoEmailValue == 1 ? false : true;
                    }
                    NotififcationTemplateModel.EmailSubject = model.EmailSubject;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    // model.Message = EduSuiteUIResources.FailedToSaveNotificationTemplate;
                    model.IsSuccessful = false;
                }
            }
            FillDropdownLists(model);
            return model;
        }

        private void FillDropdownLists(NotificationTemplateViewModel model)
        {
            FillNotificationColumns(model);
            FillNotificationStatuses(model);
        }

        private void FillNotificationColumns(NotificationTemplateViewModel model)
        {
            List<int> NotificationColumnGroupKeys = new List<int>();
            if (model.NotificationColumnGroupKeys != null)
            {
                NotificationColumnGroupKeys = model.NotificationColumnGroupKeys.Split(',').Select(Int32.Parse).ToList();
            }
            model.NotificationColumns = dbContext.NotificationColumns.Where(row => row.IsActive && NotificationColumnGroupKeys.Contains(row.NotificationColumnGroupKey)).Select(row => new NotificationColumnViewModel
            {
                NotificationColumnKey = row.NotificationColumnKey,
                NotificationColumnName = row.NotificationColumnName,
                NotificationColumnGroupName = row.NotificationColumnGroup.NotificationColumnGroupName,
                NotificationColumnGroupKey = row.NotificationColumnGroupKey
            }).OrderBy(x => x.NotificationColumnGroupKey).ToList();


            if (model.RowKey == DbConstants.NotificationTemplate.StudentTC)
            {
                List<NotificationColumnViewModel> columnlist = new List<NotificationColumnViewModel>();
                NotificationColumnGroup NCGroup = dbContext.NotificationColumnGroups.Where(x => x.RowKey == DbConstants.NotificationTemplate.StudentTC).FirstOrDefault();
                columnlist = dbContext.TCConfigColumns.Select(row => new NotificationColumnViewModel
                {
                    NotificationColumnKey = row.ColumnName.Replace(" ",""),
                    NotificationColumnName = row.ColumnName,
                    NotificationColumnGroupName = NCGroup.NotificationColumnGroupName,
                    NotificationColumnGroupKey = NCGroup.RowKey
                }).OrderBy(x => x.NotificationColumnGroupKey).ToList();

                model.NotificationColumns.AddRange(columnlist);


                //List<NotificationColumnViewModel> lsColumns = new List<NotificationColumnViewModel>();
                //StudentTCMaster TcMaster = new StudentTCMaster();
                //List<string> TCMasterCOlumnName = new List<string>();
                
                //TCMasterCOlumnName.Add("DateOfApplicationForTC");
                //TCMasterCOlumnName.Add("GeneratedDate");
                //TCMasterCOlumnName.Add("IssuedDate");

                //lsColumns = TCMasterCOlumnName.Select(row => new NotificationColumnViewModel
                //{
                //    NotificationColumnKey = row,
                //    NotificationColumnName = row,
                //    NotificationColumnGroupName = NCGroup.NotificationColumnGroupName,
                //    NotificationColumnGroupKey = NCGroup.RowKey
                //}).OrderBy(x => x.NotificationColumnGroupKey).ToList();

                //model.NotificationColumns.AddRange(lsColumns);

            }
        }

        public void FillNotificationStatuses(NotificationTemplateViewModel model)
        {
            model.NotificationStatuses.Add(new SelectListModel { RowKey = 0, Text = EduSuiteUIResources.Off });
            model.NotificationStatuses.Add(new SelectListModel { RowKey = 1, Text = EduSuiteUIResources.Manual });
            model.NotificationStatuses.Add(new SelectListModel { RowKey = 2, Text = EduSuiteUIResources.Automatic });
        }

        public NotificationDataViewModel GetNotificationData(NotificationDataViewModel model)
        {
            try
            {
                int[] NotificationColumnsGroups = { };
                NotificationDataViewModel objViewModel = new NotificationDataViewModel();
                NotificationTemplate NotificationTemplateModel = dbContext.NotificationTemplates.SingleOrDefault(row => row.RowKey == model.TemplateKey);
                if (NotificationTemplateModel != null)
                {
                    objViewModel.SMSTemplate = NotificationTemplateModel.SMSTemplate;
                    objViewModel.EmailTemplateName = NotificationTemplateModel.EmailTemplateFileName;
                    objViewModel.EmailSubject = NotificationTemplateModel.EmailSubject;
                    objViewModel.GuardianSMSTemplate = NotificationTemplateModel.GuardianSMSTemplate;
                    objViewModel.SMSTemplateID = NotificationTemplateModel.SMSTemplateID;
                    objViewModel.SMSTemplateContent = NotificationTemplateModel.SMSTemplateContent;
                    objViewModel.GuardianSMS = NotificationTemplateModel.GuardianSMS;

                    NotificationColumnsGroups = NotificationTemplateModel.NotificationColumnGroupKeys.Split(',').Select(Int32.Parse).ToArray();
                }
                IEnumerable<string> results = dbContext.Database.SqlQuery<string>("exec spSelectNotificationData @TemplateKey,@RowKey",
                                                                                    new SqlParameter("TemplateKey", model.TemplateKey),
                                                                                    new SqlParameter("RowKey", model.RowKey));


                objViewModel.NotificationData = String.Join("", results);  



                objViewModel.MobileNumber = model.MobileNumber;
                objViewModel.EmailAddess = model.EmailAddess;
                objViewModel.GuardianMobileNumber = model.GuardianMobileNumber;
                objViewModel.AdminEmailAddress = dbContext.AppUsers.Where(row => row.RowKey == DbConstants.AdminKey).Select(row => row.EmailAddress).FirstOrDefault();

                return objViewModel;
            }
            catch (Exception ex)
            {
                model = new NotificationDataViewModel();

                return model;
            }
            return model;

        }


        public NotificationDataViewModel CreateNotification(List<NotificationDataViewModel> modelList)
        {
            NotificationDataViewModel model = new NotificationDataViewModel();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    CreateNotificationData(modelList.Where(row => row.NotificationTypeKey != DbConstants.NotificationType.Push).ToList());
                    CreatePushNotificationData(modelList.Where(row => row.NotificationTypeKey == DbConstants.NotificationType.Push).ToList());
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    //model.Message = EduSuiteUIResources.FailedToSaveNotificationTemplate;
                    model.IsSuccessful = false;
                }
            }
            return model;
        }
        private void CreateNotificationData(List<NotificationDataViewModel> modelList)
        {
            long maxKey = dbContext.NotificationDatas.Select(x => x.RowKey).DefaultIfEmpty().Max();

            foreach (NotificationDataViewModel model in modelList)
            {
                NotificationData notificationDataModel = new NotificationData();
                notificationDataModel.RowKey = ++maxKey;
                notificationDataModel.NotificationTitle = model.EmailSubject;
                notificationDataModel.NotificationContent = model.SMSTemplate;
                notificationDataModel.ReceiverEmail = model.EmailAddess;
                notificationDataModel.MobileNumber = model.MobileNumber;
                notificationDataModel.NotificaionTypeKey = model.NotificationTypeKey;
                dbContext.NotificationDatas.Add(notificationDataModel);
            }
        }
        private void CreatePushNotificationData(List<NotificationDataViewModel> modelList)
        {
            long maxKey = dbContext.PushNotifications.Select(x => x.RowKey).DefaultIfEmpty().Max();

            long maxDetailKey = dbContext.PushNotificationDetails.Select(x => x.RowKey).DefaultIfEmpty().Max();

            foreach (NotificationDataViewModel model in modelList)
            {
                PushNotification pushNotificationModel = new PushNotification();
                pushNotificationModel.RowKey = ++maxKey;
                pushNotificationModel.NotificationTitle = model.PushNotificationTitle;
                pushNotificationModel.NotificationContent = model.PushNotificationContent;
                pushNotificationModel.NotificationRedirectUrl = model.PushNotificationRedirectUrl;
                pushNotificationModel.NotificationClass = model.PushNotificationType;
                dbContext.PushNotifications.Add(pushNotificationModel);
                if (model.PushNotificationUserkeys != null)
                {
                    foreach (int UserKey in model.PushNotificationUserkeys)
                    {
                        PushNotificationDetail pushNotificationDetailModel = new PushNotificationDetail();
                        pushNotificationDetailModel.RowKey = ++maxDetailKey;
                        pushNotificationDetailModel.AppUserKey = UserKey;
                        pushNotificationDetailModel.PushNotificationKey = pushNotificationModel.RowKey;
                        pushNotificationDetailModel.IsView = false;
                        pushNotificationDetailModel.IsRead = false;
                        pushNotificationDetailModel.IsDeleted = false;
                        dbContext.PushNotificationDetails.Add(pushNotificationDetailModel);

                    }
                }
            }
        }

        #region PushNotificaion

        public NotificationDataViewModel GetPushNotificationData(NotificationDataViewModel model)
        {
            try
            {
                int[] NotificationColumnsGroups = { };
                NotificationDataViewModel objViewModel = new NotificationDataViewModel();
                PushNotificationTemplate NotificationTemplateModel = dbContext.PushNotificationTemplates.SingleOrDefault(row => row.RowKey == model.PushNotificationTemplateKey);
                if (NotificationTemplateModel != null)
                {
                    // List<short> RoleKeys = NotificationTemplateModel.PushNotificationUserKeys.Split(',').Select(Int16.Parse).ToList();
                    //  List<int> UserKeys = dbContext.AppUsers.Where(row => RoleKeys.Contains(row.RoleKey ?? 0) && (row.BranchKey ?? model.BranchKey) == model.BranchKey && row.RowKey != DbConstants.User.UserKey).Select(row => row.RowKey).ToList();

                    objViewModel.PushNotificationTitle = NotificationTemplateModel.PushNotificationTitle;
                    objViewModel.PushNotificationContent = NotificationTemplateModel.PushNotificationContent;
                    objViewModel.PushNotificationUserkeys = model.PushNotificationUserkeys;
                    objViewModel.PushNotificationType = NotificationTemplateModel.PushNotificationType;
                    objViewModel.MobileNumber = model.MobileNumber;
                }
                if (NotificationTemplateModel.IsActive)
                {
                    IEnumerable<string> results = dbContext.Database.SqlQuery<string>("exec spSelectPushNotificationData @TemplateKey,@RowKey",
                                                                                    new SqlParameter("TemplateKey", model.PushNotificationTemplateKey),
                                                                                    new SqlParameter("RowKey", model.RowKey));


                    objViewModel.NotificationData = String.Join("", results);

                }


                return objViewModel;
            }
            catch (Exception ex)
            {
                model = new NotificationDataViewModel();

                return model;
            }

        }

        public IQueryable<NotificationDataViewModel> GetLatestNotification(NotificationDataViewModel model)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;


                IQueryable<PushNotificationDetail> modelList = dbContext.PushNotificationDetails.Where(row => row.AppUserKey == DbConstants.User.UserKey)
                    .OrderByDescending(row => row.PushNotification.DateAdded);
                model.TotalLatestRecords = modelList.Where(row => !row.IsView).Count();
                model.TotalUnreadRecords = modelList.Where(row => !row.IsRead).Count();


                model.TotalRecords = modelList.Count();

                if (model.PageIndex == 1)
                {
                    UpdateViewNotification(model);
                }
                return modelList.Skip(Skip).Take(Take).Select(row => new NotificationDataViewModel
                {
                    PushNotificationKey = row.PushNotification.RowKey,
                    PushNotificationContent = row.PushNotification.NotificationContent,
                    PushNotificationTitle = row.PushNotification.NotificationTitle,
                    PushNotificationRedirectUrl = row.PushNotification.NotificationRedirectUrl,
                    PushNotificationType = row.PushNotification.NotificationClass,
                    CreatedDate = row.PushNotification.DateAdded,
                    PushNotificationRead = row.IsRead
                });
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public NotificationDataViewModel UpdateViewNotification(NotificationDataViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    List<PushNotificationDetail> PushNotificationDetailList = dbContext.PushNotificationDetails.Where(row => row.AppUserKey == DbConstants.User.UserKey && !row.IsView).ToList();
                    foreach (PushNotificationDetail PushNotificationDetail in PushNotificationDetailList)
                    {
                        PushNotificationDetail.IsView = true;
                        if (PushNotificationDetail.PushNotification.NotificationRedirectUrl == null)
                        {
                            PushNotificationDetail.IsRead = true;
                        }

                    };
                    dbContext.SaveChanges();

                    dbContext.SaveChanges();

                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                }
                catch (Exception ex)
                {

                    model.IsSuccessful = false;
                    transaction.Rollback();

                }
                return model;
            }
        }
        public NotificationDataViewModel UpdateReadNotification(NotificationDataViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    PushNotificationDetail PushNotificationDetail = dbContext.PushNotificationDetails.Where(row => row.RowKey == model.PushNotificationKey && row.AppUserKey == DbConstants.User.UserKey && !row.IsView).SingleOrDefault();
                    PushNotificationDetail.IsView = true;
                    PushNotificationDetail.IsRead = true;
                    dbContext.SaveChanges();

                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                }
                catch (Exception ex)
                {

                    model.IsSuccessful = false;
                    transaction.Rollback();

                }
                return model;
            }
        }
        #endregion
    }
}
