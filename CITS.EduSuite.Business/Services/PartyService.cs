using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.Security;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Services
{
    public class PartyService : IPartyService
    {
        private EduSuiteDatabase dbContext;

        public PartyService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public PartyViewModel GetPartyById(long? id)
        {
            try
            {
                PartyViewModel model = new PartyViewModel();
                model = dbContext.Parties.Select(row => new PartyViewModel
                {
                    RowKey = row.RowKey,
                    PartyName = row.PartyName,
                    MobileNumber1 = row.MobileNumber1,
                    MobileNumber2 = row.MobileNumber2,
                    Address = row.Address,
                    PartyTypeKey = row.PartyTypeKey,
                    BranchKey = row.CompanyBranchKey,
                    LocationName = row.Location,
                    IsActive = row.IsActive,
                    ContactPerson = row.ContactPerson,
                    Designation = row.Designation,
                    EmailId = row.EmailId,
                    CreditBalance = row.CreditBalance,
                    IsTax = row.IsTax,
                    GSTINNumber = row.GSTINNumber,
                    StateKey = row.ProvinceKey,
                    IsOpeningBalance = row.IsOpeningBalance,
                    CashFlowTypeKey = row.CashFlowTypeKey,
                    OtherPartyTypeNames = row.PartyTypes
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new PartyViewModel();
                }
                else
                {
                    if (model.OtherPartyTypeNames != null && model.OtherPartyTypeNames != "")
                    {
                        model.OtherPartyTypeKeys = model.OtherPartyTypeNames.Split(',').Select(Byte.Parse).ToList();
                    }
                }
                FillDropDowns(model);
                return model;
            }
            catch (Exception ex)
            {
                PartyViewModel model = new PartyViewModel();
                ActivityLog.CreateActivityLog(MenuConstants.AssetType, ((id ?? 0) != 0 ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return model;
            }
        }

        public PartyViewModel CreateParty(PartyViewModel model)
        {
            var PartyCheck = dbContext.Parties.Where(row => row.PartyTypeKey == model.PartyTypeKey
               && row.CompanyBranchKey == model.BranchKey
               && row.PartyName.ToLower() == model.PartyName.ToLower()
               && row.MobileNumber1 == model.MobileNumber1).ToList();

            Party partyModel = new Party();
            FillDropDowns(model);
            if (PartyCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Party);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    int i = 0;
                    foreach (byte OtherPartyTypeKey in model.OtherPartyTypeKeys)
                    {
                        if (i > 0)
                        {
                            model.OtherPartyTypeNames = model.OtherPartyTypeNames + "," + OtherPartyTypeKey;
                        }
                        else
                        {
                            model.OtherPartyTypeNames = model.OtherPartyTypeNames + OtherPartyTypeKey;
                        }
                        i = i + 1;
                    }
                    AccountHeadService accountHeadService = new AccountHeadService(dbContext);
                    AccountHeadViewModel accountHeadViewModel = new AccountHeadViewModel();
                    accountHeadViewModel.AccountHeadName = model.PartyName;
                    accountHeadViewModel.AccountHeadTypeKey = DbConstants.AccountHeadType.SundryCreditors;
                    accountHeadViewModel.IsActive = true;
                    accountHeadViewModel.IsSystemAccount = true;
                    accountHeadViewModel.HideDaily = true;
                    accountHeadViewModel.HideFuture = true;
                    accountHeadViewModel = accountHeadService.createAccountChart(accountHeadViewModel);
                    long MaxKey = dbContext.Parties.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    partyModel.RowKey = Convert.ToInt64(MaxKey + 1);
                    partyModel.PartyName = model.PartyName;
                    partyModel.ProvinceKey = model.StateKey;
                    partyModel.MobileNumber1 = model.MobileNumber1;
                    partyModel.MobileNumber2 = model.MobileNumber2;
                    partyModel.ContactPerson = model.ContactPerson;
                    partyModel.Designation = model.Designation;
                    decimal oldAmount = partyModel.CreditBalance ?? 0;
                    partyModel.CreditBalance = model.CreditBalance;
                    partyModel.Address = model.Address;
                    partyModel.PartyTypeKey = Convert.ToByte(model.PartyTypeKey);
                    partyModel.Location = model.LocationName;
                    partyModel.IsActive = model.IsActive;
                    partyModel.CompanyBranchKey = model.BranchKey;
                    partyModel.EmailId = model.EmailId;
                    partyModel.IsTax = model.IsTax;
                    partyModel.GSTINNumber = model.IsTax == true ? model.GSTINNumber : "";
                    partyModel.IsOpeningBalance = model.IsOpeningBalance;
                    partyModel.CashFlowTypeKey = model.CashFlowTypeKey;
                    partyModel.PartyTypes = model.OtherPartyTypeNames;
                    partyModel.AccountHeadKey = accountHeadViewModel.RowKey;
                    dbContext.Parties.Add(partyModel);
                    partyModel.PartyType = dbContext.PartyTypes.SingleOrDefault(x => x.RowKey == model.PartyTypeKey);

                    model.RowKey = partyModel.RowKey;
                    long oldHead = 0;
                    byte? oldCashFlow = null;
                    //createAccountHeadOpening(partyModel, oldAmount, oldHead, oldCashFlow);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Party, ActionConstants.Add, DbConstants.LogType.Info, partyModel.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Party);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Party, ActionConstants.Add, DbConstants.LogType.Error, partyModel.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public PartyViewModel UpdateParty(PartyViewModel model)
        {
            FillDropDowns(model);
            var PartyCheck = dbContext.Parties.Where(row => row.PartyTypeKey == model.PartyTypeKey
               && row.CompanyBranchKey == model.BranchKey
               && row.PartyName.ToLower() == model.PartyName.ToLower()
               && row.MobileNumber1 == model.MobileNumber1
               && row.RowKey != model.RowKey).ToList();

            Party partyModel = new Party();
            if (PartyCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Party);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    int i = 0;
                    foreach (byte OtherPartyTypeKey in model.OtherPartyTypeKeys)
                    {
                        if (i > 0)
                        {
                            model.OtherPartyTypeNames = model.OtherPartyTypeNames + "," + OtherPartyTypeKey;
                        }
                        else
                        {
                            model.OtherPartyTypeNames = model.OtherPartyTypeNames + OtherPartyTypeKey;
                        }
                        i = i + 1;
                    }
                    AccountHeadService accountHeadService = new AccountHeadService(dbContext);
                    AccountHeadViewModel accountHeadViewModel = new AccountHeadViewModel();
                    accountHeadViewModel.AccountHeadName = model.PartyName;
                    accountHeadViewModel.AccountHeadTypeKey = DbConstants.AccountHeadType.SundryCreditors;
                    accountHeadViewModel.IsActive = true;
                    accountHeadViewModel.IsSystemAccount = true;
                    accountHeadViewModel.HideDaily = true;
                    accountHeadViewModel.HideFuture = true;
                    partyModel = dbContext.Parties.SingleOrDefault(row => row.RowKey == model.RowKey);
                    accountHeadViewModel.RowKey = partyModel.AccountHeadKey;
                    accountHeadViewModel.RowKey = dbContext.AccountHeads.Where(x => x.RowKey == partyModel.AccountHeadKey).Select(x => x.RowKey).FirstOrDefault();
                    accountHeadViewModel = accountHeadService.updateAccountChart(accountHeadViewModel);
                    decimal? oldOpeningBalance = partyModel.CreditBalance;
                    byte? oldCashFlow = partyModel.CashFlowTypeKey;
                    bool oldIsOpeningBalance = partyModel.IsOpeningBalance;
                    partyModel.PartyName = model.PartyName;
                    partyModel.MobileNumber1 = model.MobileNumber1;
                    partyModel.MobileNumber2 = model.MobileNumber2;
                    partyModel.ProvinceKey = model.StateKey;
                    partyModel.ContactPerson = model.ContactPerson;
                    partyModel.Designation = model.Designation;
                    partyModel.PartyTypes = model.OtherPartyTypeNames;
                    decimal oldAmount = partyModel.CreditBalance ?? 0;
                    byte oldPartyType = partyModel.PartyTypeKey;
                    partyModel.CreditBalance = model.CreditBalance;
                    partyModel.IsTax = model.IsTax;
                    partyModel.GSTINNumber = model.IsTax == true ? model.GSTINNumber : "";
                    partyModel.Address = model.Address;
                    partyModel.Location = model.LocationName;
                    partyModel.IsActive = model.IsActive;
                    partyModel.PartyTypeKey = Convert.ToByte(model.PartyTypeKey);
                    partyModel.CompanyBranchKey = model.BranchKey;
                    partyModel.AccountHeadKey = accountHeadViewModel.RowKey;
                    partyModel.EmailId = model.EmailId;
                    partyModel.IsOpeningBalance = model.IsOpeningBalance;
                    partyModel.CashFlowTypeKey = model.CashFlowTypeKey;
                    partyModel.PartyType = dbContext.PartyTypes.SingleOrDefault(x => x.RowKey == model.PartyTypeKey);
                    long oldHead = 0;

                    //createAccountHeadOpening(partyModel, oldAmount, oldHead, oldCashFlow);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Party, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Party);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Party, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;

        }

        public PartyViewModel DeleteParty(PartyViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Party party = dbContext.Parties.SingleOrDefault(row => row.RowKey == model.RowKey);
                    long AccountHeadKey = party.AccountHeadKey;
                    dbContext.Parties.Remove(party);

                    AccountFlowService accountFlowService = new AccountFlowService(dbContext);
                    accountFlowService.DeleteAccountHead(AccountHeadKey);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Party, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Party);
                        model.IsSuccessful = false;
                    }
                    ActivityLog.CreateActivityLog(MenuConstants.Party, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Party);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Party, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public List<PartyViewModel> GetParty(string searchText, PartyViewModel model)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;
                IQueryable<PartyViewModel> partyList = (from p in dbContext.Parties
                                                        where ((model.BranchKey != 0) ? p.CompanyBranchKey == model.BranchKey &&
                                                       (model.PartyTypeKey != null ? (p.PartyTypeKey == model.PartyTypeKey && (p.PartyName.Contains(searchText) || (p.MobileNumber1 ?? "").Contains(searchText))) : p.PartyName.Contains(searchText) || (p.MobileNumber1 ?? "").Contains(searchText)) :
                                                        model.PartyTypeKey != null ? (p.PartyTypeKey == model.PartyTypeKey && (p.PartyName.Contains(searchText) || (p.MobileNumber1 ?? "").Contains(searchText))) : p.PartyName.Contains(searchText) || (p.MobileNumber1 ?? "").Contains(searchText)
                                                        )
                                                        select new PartyViewModel
                                                        {
                                                            RowKey = p.RowKey,
                                                            PartyName = p.PartyName,
                                                            MobileNumber1 = p.MobileNumber1 + "," + p.MobileNumber2,
                                                            Address = p.Address,
                                                            LocationName = p.Location,
                                                            PartyTypeName = p.PartyType.PartyTypeName,
                                                            StatusName = p.IsActive ? EduSuiteUIResources.Active : EduSuiteUIResources.DeActive,
                                                            CompanyBranchName = p.Branch.BranchName,
                                                            EmailId = p.EmailId,
                                                            CreditBalance = p.CreditBalance,
                                                            GSTINNumber = p.GSTINNumber,
                                                            PartyTypeKey = p.PartyTypeKey,
                                                            TotalAmount = (p.AssetPurchaseMasters.Where(x => x.PartyKey == p.RowKey).Select(x => x.TotalAmount).DefaultIfEmpty().Sum()),
                                                            TotalPaidAmount = (dbContext.AssetPurchasePayments.Where(x => x.AssetPurchaseMaster.PartyKey == p.RowKey && x.ChequeStatusKey != DbConstants.ProcessStatus.Rejected).Select(x => x.PaidAmount ?? 0).DefaultIfEmpty().Sum())
                                                        });

                if (model.isBalanceDue)
                {
                    partyList = partyList.Where(x => (x.TotalAmount ?? 0) - (x.TotalPaidAmount ?? 0) > 0);
                }
                if (model.SortBy != "")
                {
                    partyList = SortSalesOrder(partyList, model.SortBy, model.SortOrder);
                }
                model.TotalRecords = partyList.Count();
                model.TotalAmount = partyList.Select(x => x.TotalAmount).DefaultIfEmpty().Sum();
                model.TotalPaidAmount = partyList.Select(x => x.TotalPaidAmount).DefaultIfEmpty().Sum();
                model.TotalBalanceAmount = partyList.Select(x => (x.TotalAmount ?? 0) - (x.TotalPaidAmount ?? 0)).DefaultIfEmpty().Sum();
                return model.PageSize != 0 ? partyList.Skip(Skip).Take(Take).ToList() : partyList.ToList();

            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Party, ActionConstants.MenuAccess, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return new List<PartyViewModel>();
            }
        }
        private IQueryable<PartyViewModel> SortSalesOrder(IQueryable<PartyViewModel> Query, string SortName, string SortOrder)
        {

            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(PartyViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<PartyViewModel>(resultExpression);

        }
        private void PartyAmountDetails(List<PartyViewModel> partyList)
        {
            foreach (PartyViewModel party in partyList)
            {
                party.TotalAmount = dbContext.AssetPurchasePayments.Where(x => x.AssetPurchaseMaster.PartyKey == party.RowKey).Select(x => x.AssetPurchaseMaster.TotalAmount).DefaultIfEmpty().Sum();
                party.TotalPaidAmount = dbContext.AssetPurchasePayments.Where(x => x.ChequeStatusKey != DbConstants.ProcessStatus.Rejected && x.AssetPurchaseMaster.PartyKey == party.RowKey).Select(x => x.PaidAmount ?? 0).DefaultIfEmpty().Sum();
                party.TotalBalanceAmount = (party.TotalAmount ?? 0) - (party.TotalPaidAmount ?? 0);

            }
        }

        private void FillDropDowns(PartyViewModel model)
        {
            FillPartyType(model);
            FillStates(model);
            FillCashFlowType(model);
            FillPartyTypeById(model);
            //For Notification
            if (model.AutoEmail == null && model.AutoSMS == null && model.RowKey == 0)
                FillNotificationDetail(model);
        }

        public PartyViewModel FillPartyType(PartyViewModel model)
        {
            model.PartyTypes = dbContext.PartyTypes.Where(x => x.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.PartyTypeName
            }).ToList();
            return model;
        }

        public PartyViewModel FillPartyTypeById(PartyViewModel model)
        {
            model.OtherPartyTypes = dbContext.PartyTypes.Where(x => x.RowKey != model.PartyTypeKey).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.PartyTypeName
            }).ToList();
            return model;
        }
        private void FillCashFlowType(PartyViewModel model)
        {
            model.CashFlowTypes = dbContext.CashFlowTypes.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.CashFlowTypeName
            }).ToList();
        }
        private void FillStates(PartyViewModel model)
        {
            model.States = dbContext.Provinces.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.Provincename
            }).ToList();
        }

        public PartyViewModel GetPartyOrdersId(Int32? Id)
        {
            try
            {
                PartyViewModel model = new PartyViewModel();
                model = dbContext.Parties.Where(x => x.RowKey == Id).Select(row => new PartyViewModel
                {
                    RowKey = row.RowKey,
                    PartyName = row.PartyName,
                    MobileNumber1 = row.MobileNumber1,
                    MobileNumber2 = row.MobileNumber2,
                    Address = row.Address,
                    PartyTypeKey = row.PartyTypeKey,
                    CompanyBranchName = row.Branch.BranchName,
                    LocationName = row.Location,
                    ContactPerson = row.ContactPerson,
                    Designation = row.Designation,
                    EmailId = row.EmailId,
                    CreditBalance = row.CreditBalance,
                    GSTINNumber = row.GSTINNumber,
                    StateName = row.Province.Provincename,
                    CashFlowTypeName = row.CashFlowTypeKey == DbConstants.CashFlowType.In ? EduSuiteUIResources.In : EduSuiteUIResources.Out,
                }).FirstOrDefault();
                return model;
            }
            catch (Exception ex)
            {
                PartyViewModel model = new PartyViewModel();
                ActivityLog.CreateActivityLog(MenuConstants.Party, ActionConstants.View, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return model;
            }
        }

        public PartyViewModel FillBranches(PartyViewModel model)
        {
            if (model.BranchKey == null || model.BranchKey == 0)
            {
                model.BranchKey = dbContext.AppUsers.Where(row => row.RowKey == DbConstants.User.UserKey).Select(row => row.Employees.Select(x=>x.BranchKey).FirstOrDefault()).FirstOrDefault();
                if (model.BranchKey == 0)
                {
                    model.Branches = dbContext.vwBranchSelectActiveOnlies.Select(row => new SelectListModel
                    {
                        RowKey = row.RowKey,
                        Text = row.BranchName
                    }).ToList();
                    if (model.Branches.Count == 1)
                    {
                        model.BranchKey = dbContext.vwBranchSelectActiveOnlies.Select(x => x.RowKey).FirstOrDefault();
                    }
                }
            }
            return model;
        }

        public PartyViewModel CheckGSTINNumberExists(string GSTINNumber, byte? PartyTypeKey, long? RowKey)
        {
            PartyViewModel model = new PartyViewModel();
            if (dbContext.Parties.Where(row => row.GSTINNumber.ToUpper() == GSTINNumber.ToUpper() && row.PartyTypeKey == PartyTypeKey && row.RowKey != RowKey).Any())
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.GSTINNumber);
                model.IsSuccessful = false;
            }
            else
            {
                model.Message = "";
                model.IsSuccessful = true;
            }
            return model;
        }

        private void FillNotificationDetail(PartyViewModel model)
        {
            //NotificationTemplate notificationTemplateModel = dbContext.NotificationTemplates.SingleOrDefault(row => row.RowKey == DbConstants.NotificationTemplate.WelcomeNotificationParty);
            //if (notificationTemplateModel != null)
            //{
            //    model.AutoEmail = notificationTemplateModel.AutoEmail;
            //    model.AutoSMS = notificationTemplateModel.AutoSMS;
            //    model.TemplateKey = notificationTemplateModel.RowKey;
            //}
        }
        
    }
}
