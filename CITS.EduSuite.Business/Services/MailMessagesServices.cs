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
using System.Web;
using System.Data.Entity.Core.Objects;

namespace CITS.EduSuite.Business.Services
{
    public class MailMessagesServices : IMailMessagesServices
    {
        private EduSuiteDatabase dbContext;
        public MailMessagesServices(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public List<MailMessagesViewModel> GetMessages(MailMessagesViewModel model)
        {

            ObjectParameter InboxCount = new ObjectParameter("InboxCount", typeof(Int64));
            ObjectParameter InboxNewCount = new ObjectParameter("InboxNewCount", typeof(Int64));
            ObjectParameter OutboxCount = new ObjectParameter("OutboxCount", typeof(Int64));
            ObjectParameter DraftCount = new ObjectParameter("DraftCount", typeof(Int64));
            ObjectParameter TrashCount = new ObjectParameter("TrashCount", typeof(Int64));
            ObjectParameter StarredCount = new ObjectParameter("StarredCount", typeof(Int64));
            ObjectParameter TotalCount = new ObjectParameter("TotalCount", typeof(Int64));

            var MessagesList = dbContext.spMailMessageSelect(model.UserKey, model.TabKey, DbConstants.MailMessage.InBox, DbConstants.MailMessage.Sent, DbConstants.MailMessage.Draft
                , DbConstants.MailMessage.Trash, DbConstants.MailMessage.Starred, model.PageIndex, model.PageSize, InboxCount, InboxNewCount, OutboxCount, DraftCount, TrashCount, StarredCount, TotalCount)
                                  .Select(row => new MailMessagesViewModel
                                  {

                                      MessageContent = row.MessageContent,
                                      MessageSubject = row.MessageSubject,
                                      CreatedOn = row.CreatedOn,
                                      MailMessageKey = row.MailMessageKey,
                                      MailFromUserName = row.MailFromUserName,
                                      MailToUserName = row.MailToUserName,
                                      IsRead = row.IsRead,
                                      TabKey = row.TabKey,
                                      RowKey = row.RowKey,
                                      ActiveTabKey = model.ActiveTabKey,
                                      DirectionName = row.DirectionName
                                  }).ToList();


            model.InboxCount = InboxCount.Value != DBNull.Value ? Convert.ToInt64(InboxCount.Value) : 0;
            model.InboxNewCount = InboxNewCount.Value != DBNull.Value ? Convert.ToInt64(InboxNewCount.Value) : 0;
            model.OutBoxCount = OutboxCount.Value != DBNull.Value ? Convert.ToInt64(OutboxCount.Value) : 0;
            model.DraftCount = DraftCount.Value != DBNull.Value ? Convert.ToInt64(DraftCount.Value) : 0;
            model.TrashCount = TrashCount.Value != DBNull.Value ? Convert.ToInt64(TrashCount.Value) : 0;
            model.StaredCount = StarredCount.Value != DBNull.Value ? Convert.ToInt64(StarredCount.Value) : 0;
            model.TotalRecords = TotalCount.Value != DBNull.Value ? Convert.ToInt64(TotalCount.Value) : 0;

            switch (model.TabKey)
            {
                case DbConstants.MailMessage.InBox:
                    model.InboxCount = model.TotalRecords;
                    break;
                case DbConstants.MailMessage.Sent:
                    model.OutBoxCount = model.TotalRecords;
                    break;
                case DbConstants.MailMessage.Draft:
                    model.DraftCount = model.TotalRecords;
                    break;
                case DbConstants.MailMessage.Trash:
                    model.TrashCount = model.TotalRecords;
                    break;
                case DbConstants.MailMessage.Starred:
                    model.StaredCount = model.TotalRecords;
                    break;

            }


            return MessagesList;
        }
        public MailMessagesViewModel GetMessagesContent(MailMessagesViewModel model)
        {
            if (model.TabKey == 1)
            {
                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    MailInboxMessage InboxMessages = dbContext.MailInboxMessages.Where(x => x.RowKey == model.RowKey).SingleOrDefault();
                    model.MailMessageKey = InboxMessages.MailMessage.RowKey;
                    model.MessageSubject = InboxMessages.MailMessage.MessageSubject;
                    model.MessageContent = InboxMessages.MailMessage.MessageContent;
                    model.CreatedOn = InboxMessages.MailMessage.DateAdded;
                    model.MailFromUserName = InboxMessages.AppUser1.AppUserName;
                    model.MailToUserName = InboxMessages.AppUser.AppUserName;
                    model.ActiveTabKey = 1;
                    model.FilesNames = GetMessagesFiles(model);

                    InboxMessages.IsRead = true;
                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = ApplicationResources.Success;
                    model.IsSuccessful = true;
                }


            }
            else if (model.TabKey == 2)
            {
                MailOutBoxMessage MailOutBoxMessages = dbContext.MailOutBoxMessages.Where(x => x.RowKey == model.RowKey).SingleOrDefault();
                model.MailMessageKey = MailOutBoxMessages.MailMessage.RowKey;
                model.MessageSubject = MailOutBoxMessages.MailMessage.MessageSubject;
                model.MessageContent = MailOutBoxMessages.MailMessage.MessageContent;
                model.CreatedOn = MailOutBoxMessages.MailMessage.DateAdded;
                model.MailFromUserName = MailOutBoxMessages.AppUser1.AppUserName;
                model.MailToUserName = MailOutBoxMessages.AppUser.AppUserName;
                model.ActiveTabKey = 2;
                model.FilesNames = GetMessagesFiles(model);

            }
            else if (model.TabKey == 3)
            {
                MailDraftMessage MailDraftMessages = dbContext.MailDraftMessages.Where(x => x.RowKey == model.RowKey).SingleOrDefault();
                model.MailMessageKey = MailDraftMessages.MailMessage.RowKey;
                model.MessageSubject = MailDraftMessages.MailMessage.MessageSubject;
                model.MessageContent = MailDraftMessages.MailMessage.MessageContent;
                model.CreatedOn = MailDraftMessages.MailMessage.DateAdded;
                model.MailFromUserName = MailDraftMessages.AppUser.AppUserName;
                model.ActiveTabKey = 3;
                model.FilesNames = GetMessagesFiles(model);
            }


            return model;
        }
        public List<MailMessageFileNameList> GetMessagesFiles(MailMessagesViewModel model)
        {
            try
            {
                var FileNameList = (from r in dbContext.MailMessageFiles
                                    //orderby e.RowKey
                                    where (r.MailMessageKey == model.MailMessageKey)
                                    select new MailMessageFileNameList
                                    {
                                        FileName = r.DataFileName
                                    }).ToList();

                return FileNameList;
            }
            catch (Exception ex)
            {
                return new List<MailMessageFileNameList>();
                ActivityLog.CreateActivityLog(MenuConstants.MailBox, ActionConstants.View, DbConstants.LogType.Error, model.MailMessageKey, ex.GetBaseException().Message);
            }
        }
        public MailMessagesViewModel GetMailMessageById(MailMessagesViewModel model)
        {
            if (model.TabKey == DbConstants.MailMessage.InBox)
            {

                MailInboxMessage InboxMessages = dbContext.MailInboxMessages.Where(x => x.RowKey == model.RowKey).SingleOrDefault();
                model.MailMessageKey = InboxMessages.MailMessage.RowKey;
                model.MessageSubject = InboxMessages.MailMessage.MessageSubject;
                model.MessageContent = InboxMessages.MailMessage.MessageContent;
                model.MailFromUserName = InboxMessages.AppUser1.AppUserName;
                model.MailToUserName = InboxMessages.AppUser.AppUserName;
                model.ActiveTabKey = 1;
                model.FilesNames = GetMessagesFiles(model);
                model.Message = ApplicationResources.Success;
                model.IsSuccessful = true;

            }
            else if (model.TabKey == DbConstants.MailMessage.Sent)
            {
                MailOutBoxMessage MailOutBoxMessages = dbContext.MailOutBoxMessages.Where(x => x.RowKey == model.RowKey).SingleOrDefault();
                model.MailMessageKey = MailOutBoxMessages.MailMessage.RowKey;
                model.MessageSubject = MailOutBoxMessages.MailMessage.MessageSubject;
                model.MessageContent = MailOutBoxMessages.MailMessage.MessageContent;
                model.MailFromUserName = MailOutBoxMessages.AppUser1.AppUserName;
                model.MailToUserName = MailOutBoxMessages.AppUser.AppUserName;
                model.ActiveTabKey = 2;
                model.FilesNames = GetMessagesFiles(model);

            }
            else if (model.TabKey == DbConstants.MailMessage.Draft)
            {
                MailDraftMessage MailDraftMessages = dbContext.MailDraftMessages.Where(x => x.RowKey == model.RowKey).SingleOrDefault();
                model.MailMessageKey = MailDraftMessages.MailMessage.RowKey;
                model.MessageSubject = MailDraftMessages.MailMessage.MessageSubject;
                model.MessageContent = MailDraftMessages.MailMessage.MessageContent;
                model.MailFromUserName = MailDraftMessages.AppUser.AppUserName;
                model.ActiveTabKey = 3;
                model.FilesNames = GetMessagesFiles(model);
            }
            return model;
        }
        public MailMessagesViewModel CreateMessage(MailMessagesViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {

                try
                {
                    MailMessage MailMessageModel = new MailMessage();
                    Int64 maxKey = dbContext.MailMessages.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    MailMessageModel.RowKey = (maxKey + 1);
                    MailMessageModel.MessageSubject = model.MessageSubject;
                    MailMessageModel.MessageContent = model.MessageContent;
                    model.MailMessageKey = MailMessageModel.RowKey;
                    dbContext.MailMessages.Add(MailMessageModel);

                    if (model.ActiveTabKey == DbConstants.MailMessage.Draft)
                    {
                        SaveDraft(model);
                    }
                    else
                    {
                        CreateMailOutBox(model);
                        CreateMailInbox(model);

                        if (model.TabKey == DbConstants.MailMessage.Draft)
                        {
                            MailDraftMessage Draft = dbContext.MailDraftMessages.Where(x => x.RowKey == model.RowKey).SingleOrDefault();
                            dbContext.MailDraftMessages.Remove(Draft);
                        }
                    }
                    CreateMailMessageFile(model);

                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = ApplicationResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.MailBox, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = ApplicationResources.FailedToSaveMailMessage;
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.MailBox, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public MailMessagesViewModel SaveDraft(MailMessagesViewModel model)
        {

            Int64 maxKey = dbContext.MailDraftMessages.Select(p => p.RowKey).DefaultIfEmpty().Max();

            MailDraftMessage MailDraftMessageItem = new MailDraftMessage();
            maxKey = maxKey + 1;
            MailDraftMessageItem.RowKey = maxKey;
            MailDraftMessageItem.MailMessageKey = model.MailMessageKey;
            MailDraftMessageItem.MailMessageFromUserKey = model.UserKey;
            dbContext.MailDraftMessages.Add(MailDraftMessageItem);

            model.IsSuccessful = true;
            return model;
        }
        public MailMessagesViewModel CreateMailOutBox(MailMessagesViewModel model)
        {
            int i = 0;
            Int64 maxKey = dbContext.MailOutBoxMessages.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (int ToUserKey in model.MailToUserKeys)
            {
                MailOutBoxMessage MailOutboxItem = new MailOutBoxMessage();
                maxKey = maxKey + 1;
                MailOutboxItem.RowKey = maxKey;
                MailOutboxItem.MailMessageKey = model.MailMessageKey;
                MailOutboxItem.MessageStatusKey = 0;
                MailOutboxItem.MailMessageToUserKey = ToUserKey;
                MailOutboxItem.MailMessageFromUserKey = model.UserKey;
                dbContext.MailOutBoxMessages.Add(MailOutboxItem);
                i = i + 1;
            }

            return model;
        }
        public MailMessagesViewModel CreateMailInbox(MailMessagesViewModel model)
        {
            int i = 0;

            Int64 maxKey = dbContext.MailInboxMessages.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (int ToUserKey in model.MailToUserKeys)
            {
                MailInboxMessage MailInboxItem = new MailInboxMessage();
                maxKey = maxKey + 1;
                MailInboxItem.RowKey = maxKey;
                MailInboxItem.MailMessageToUserKey = ToUserKey;
                MailInboxItem.MailMessageFromUserKey = model.UserKey;
                MailInboxItem.IsRead = false;
                MailInboxItem.MailMessageKey = model.MailMessageKey;
                MailInboxItem.MessageStatusKey = 0;
                dbContext.MailInboxMessages.Add(MailInboxItem);
                i = i + 1;
            }
            return model;
        }
        public MailMessagesViewModel CreateMailMessageFile(MailMessagesViewModel model)
        {
            Int64 maxKey = dbContext.MailMessageFiles.Select(p => p.RowKey).DefaultIfEmpty().Max();
            int i = 0;



            foreach (HttpPostedFileBase List in model.MessageFiles)
            {
                MailMessageFile MailMessageFileItem = new MailMessageFile();
                maxKey = maxKey + 1;
                MailMessageFileItem.RowKey = maxKey;
                MailMessageFileItem.MailMessageKey = model.MailMessageKey;
                MailMessageFileItem.DataFileName = maxKey.ToString() + "." + List.FileName.Split('.').Last();
                model.NewFileNames = model.NewFileNames + "|" + MailMessageFileItem.DataFileName;
                dbContext.MailMessageFiles.Add(MailMessageFileItem);
                i = i + 1;
            }


            if (model.ForwardFileNames != null)
            {
                string[] ForwardFileNames = model.ForwardFileNames.Split('|');
                foreach (string FileName in ForwardFileNames)
                {

                    MailMessageFile MailMessageFileItem = new MailMessageFile();
                    maxKey = maxKey + 1;
                    MailMessageFileItem.RowKey = maxKey;
                    MailMessageFileItem.MailMessageKey = model.MailMessageKey;
                    MailMessageFileItem.DataFileName = FileName;
                    dbContext.MailMessageFiles.Add(MailMessageFileItem);
                    i = i + 1;
                }
            }

            return model;
        }
        public MailMessagesViewModel UpdateTrash(MailMessagesViewModel model)
        {


            using (var transaction = dbContext.Database.BeginTransaction())
            {

                try
                {
                    if (model.TabKey == 1)
                    {
                        MailInboxMessage Inbox = dbContext.MailInboxMessages.SingleOrDefault(x => x.RowKey == model.RowKey);
                        Inbox.MessageStatusKey = 1;


                    }
                    else if (model.TabKey == 2)
                    {
                        MailOutBoxMessage Outbox = dbContext.MailOutBoxMessages.SingleOrDefault(x => x.RowKey == model.RowKey);
                        Outbox.MessageStatusKey = 1;

                    }
                    else if (model.TabKey == 3)
                    {
                        MailDraftMessage Draft = dbContext.MailDraftMessages.SingleOrDefault(x => x.RowKey == model.RowKey);
                        Draft.MessageStatusKey = 1;

                    }

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = ApplicationResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.MailBox, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = ApplicationResources.FailedToSaveTrash;
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.MailBox, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }
        public MailMessagesViewModel UpdateStar(MailMessagesViewModel model)
        {

            using (var transaction = dbContext.Database.BeginTransaction())
            {

                try
                {
                    if (model.TabKey == DbConstants.MailMessage.InBox)
                    {
                        MailInboxMessage Inbox = dbContext.MailInboxMessages.Where(x => x.RowKey == model.RowKey).SingleOrDefault();
                        Inbox.MessageStatusKey = 2;
                    }
                    else if (model.TabKey == DbConstants.MailMessage.Sent)
                    {
                        MailOutBoxMessage Outbox = dbContext.MailOutBoxMessages.Where(x => x.RowKey == model.RowKey).SingleOrDefault();
                        Outbox.MessageStatusKey = 2;
                    }
                    else if (model.TabKey == DbConstants.MailMessage.Draft)
                    {
                        MailDraftMessage Draft = dbContext.MailDraftMessages.Where(x => x.RowKey == model.RowKey).SingleOrDefault();
                        Draft.MessageStatusKey = 2;

                    }

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = ApplicationResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.MailBox, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = ApplicationResources.FailedToSaveStar;
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.MailBox, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }
        public MailMessagesViewModel DeleteMailMessage(MailMessagesViewModel model)
        {

            using (var transaction = dbContext.Database.BeginTransaction())
            {

                try
                {
                    if (model.TabKey == 1)
                    {
                        MailInboxMessage Inbox = dbContext.MailInboxMessages.Where(x => x.RowKey == model.RowKey).SingleOrDefault();
                        dbContext.MailInboxMessages.Remove(Inbox);

                    }

                    else if (model.TabKey == 2)
                    {
                        MailOutBoxMessage Outbox = dbContext.MailOutBoxMessages.Where(x => x.RowKey == model.RowKey).SingleOrDefault();
                        dbContext.MailOutBoxMessages.Remove(Outbox);

                    }
                    else if (model.TabKey == 3)
                    {
                        MailDraftMessage Draft = dbContext.MailDraftMessages.Where(x => x.RowKey == model.RowKey).SingleOrDefault();
                        dbContext.MailDraftMessages.Remove(Draft);

                    }

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = ApplicationResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.MailBox, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = ApplicationResources.FailedToDeleteMailMessage;
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.MailBox, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }
        public List<MailMessagesSenderListViewModel> GetEmployeeByText(MailMessagesViewModel model)
        {
            try
            {
                var AppUsers = (from r in dbContext.AppUsers

                                where (r.AppUserName.Contains(model.SearchText))
                                select new MailMessagesSenderListViewModel
                                {

                                    MessageToUserKey = r.RowKey,
                                    MailMessageUserName = r.FirstName + " " + (r.MiddleName ?? "") + " " + r.LastName

                                }).ToList();

                return AppUsers;
            }
            catch (Exception ex)
            {
                return new List<MailMessagesSenderListViewModel>();
                ActivityLog.CreateActivityLog(MenuConstants.MailBox, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
            }
        }

        public void GetEmployees(MailMessagesViewModel model)
        {

            model.ToEmployees = dbContext.AppUsers.Where(row => row.RowKey != model.UserKey).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.FirstName + " " + (row.MiddleName ?? "") + " " + row.LastName
            }).ToList();
        }
        //public void GetHigherEmployees(MailMessagesViewModel model)
        //{
        //    if (DbConstants.AdminKey != model.UserKey)
        //    {
        //        model.ToEmployees = dbContext.AppUsers.Where(row => row.RowKey == DbConstants.AdminKey).Select(row => new SelectListModel
        //        {
        //            RowKey = row.RowKey,
        //            Text = row.FirstName + " " + (row.MiddleName ?? "") + " " + row.LastName
        //        }).Union(dbContext.fnParentEmployees(model.UserKey).Where(row => row.AppUserKey != model.UserKey)
        //            .Select(row => new SelectListModel
        //        {
        //            RowKey = row.AppUserKey ?? 0,
        //            Text = row.EmployeeName
        //        })).ToList();
        //    }
        //    else
        //    {
        //        model.ToEmployees = dbContext.AppUsers.Where(row => row.RowKey != model.UserKey).Select(row => new SelectListModel
        //        {
        //            RowKey = row.RowKey,
        //            Text = row.FirstName + " " + (row.MiddleName ?? "") + " " + row.LastName
        //        }).ToList();
        //    }
        //}
    }

}
