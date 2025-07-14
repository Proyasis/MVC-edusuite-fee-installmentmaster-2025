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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Services
{
    public class BankStatementService : IBankStatementService
    {
        private EduSuiteDatabase dbContext;
        public BankStatementService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public long detailMaxKey;
        public BankStatementMasterViewModel GetBankStatementById(short? id)
        {
            try
            {
                BankStatementMasterViewModel model = new BankStatementMasterViewModel();
                model = dbContext.BankStatementMasters.Where(x => x.RowKey == id).Select(row => new BankStatementMasterViewModel
                {
                    RowKey = row.RowKey,
                    BankAccountKey = row.BankAccountKey,
                    BranchKey = row.BranchKey,
                    YearKey = row.YearKey,
                    MonthKey = row.MonthKey,

                }).FirstOrDefault();
                if (model == null)
                {
                    model = new BankStatementMasterViewModel();
                    model.YearKey = DateTimeUTC.Now.Year;
                    model.MonthKey = Convert.ToByte(DateTimeUTC.Now.Month);
                }
                FillDropDowns(model);
                //GetBankStatementDetails(model);
                return model;

            }
            catch (Exception Ex)
            {
                BankStatementMasterViewModel model = new BankStatementMasterViewModel();
               
                ActivityLog.CreateActivityLog(MenuConstants.BankStatement, ActionConstants.View, DbConstants.LogType.Error, id, Ex.GetBaseException().Message);
                return model;
            }
        }
        public List<BankStatementMasterViewModel> GetBankStatements(BankStatementMasterViewModel model, string searchText)
        {
            try
            {
                var BankStatementMasterList = (from row in dbContext.BankStatementMasters
                                               orderby row.RowKey
                                               select new BankStatementMasterViewModel
                                               {
                                                   RowKey = row.RowKey,
                                                   BankAccountName = (row.BankAccount.NameInAccount ?? row.BankAccount.AccountNumber) + EduSuiteUIResources.Hyphen + row.BankAccount.Bank.BankName,
                                                   BranchName = row.Branch.BranchName + EduSuiteUIResources.BlankSpace + row.Branch.CityName,
                                                   BankAccountKey = row.BankAccountKey,
                                                   BranchKey = row.BranchKey,
                                                   YearKey = row.YearKey,
                                                   MonthKey = row.MonthKey
                                               });
                if (model.BranchKey != 0)
                {
                    BankStatementMasterList = BankStatementMasterList.Where(x => x.BranchKey == model.BranchKey);
                }
                return BankStatementMasterList.ToList<BankStatementMasterViewModel>();
            }

            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.BankStatement, ActionConstants.View, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                return new List<BankStatementMasterViewModel> ();
            }
        }
        public BankStatementMasterViewModel GetBankStatementDetails(BankStatementMasterViewModel model)
        {
            model.RowKey = dbContext.BankStatementMasters.Where(x => x.BankAccountKey == model.BankAccountKey && x.YearKey == model.YearKey && x.MonthKey == model.MonthKey).Select(x => x.RowKey).FirstOrDefault();
            model.BankStatementDetails = dbContext.BankStatementDetails.Where(x => x.MasterKey == model.RowKey).Select(row => new BankStatementDetailsViewModel
            {
                RowKey = row.RowKey,
                ReferenceKey = row.ReferenceKey,
                Particulars = row.Particulars,
                BankTransactionTypeKey = row.BankTransactionTypeKey,
                CashFlowTypeKey = row.CashFlowTypeKey,
                Amount = row.Amount,
                TransactionDate = row.TransactionDate
            }).ToList();
            if (model.BankStatementDetails.Count == 0)
            {
                model.BankStatementDetails.Add(new BankStatementDetailsViewModel
                {
                    RowKey = 0,
                    ReferenceKey = "",
                    Particulars = "",
                    BankTransactionTypeKey = 0,
                    CashFlowTypeKey = 0,
                    TransactionDate = DateTimeUTC.Now
                });
            }
            foreach (BankStatementDetailsViewModel ModelItem in model.BankStatementDetails)
            {
                FillBanKTransactionTypes(ModelItem);
                FillCashFlowType(ModelItem);
            }
            return model;
        }
        private void createBankStatementDetails(List<BankStatementDetailsViewModel> modelList, long MaterKey)
        {
            if (modelList.Count != 0)
            {
                foreach (BankStatementDetailsViewModel ModelItem in modelList)
                {
                    BankStatementDetail BankStatementDetailModel = new BankStatementDetail();
                    BankStatementDetailModel.RowKey = detailMaxKey + 1;
                    BankStatementDetailModel.MasterKey = MaterKey;
                    BankStatementDetailModel.ReferenceKey = ModelItem.ReferenceKey;
                    BankStatementDetailModel.Particulars = ModelItem.Particulars;
                    BankStatementDetailModel.CashFlowTypeKey = ModelItem.CashFlowTypeKey;
                    BankStatementDetailModel.Amount = ModelItem.Amount ?? 0;
                    BankStatementDetailModel.BankTransactionTypeKey = ModelItem.BankTransactionTypeKey;
                    BankStatementDetailModel.TransactionDate = Convert.ToDateTime(ModelItem.TransactionDate);
                    dbContext.BankStatementDetails.Add(BankStatementDetailModel);
                    ModelItem.RowKey = BankStatementDetailModel.RowKey;
                   // ModelItem.UpdateType = DbConstants.UpdationType.Create;
                    detailMaxKey++;
                }
            }
        }
        private void updateBankStatementDetails(List<BankStatementDetailsViewModel> modelList)
        {
            if (modelList.Count != 0)
            {
                foreach (BankStatementDetailsViewModel ModelItem in modelList)
                {
                    BankStatementDetail BankStatementDetailModel = new BankStatementDetail();
                    BankStatementDetailModel = dbContext.BankStatementDetails.SingleOrDefault(x => x.RowKey == ModelItem.RowKey);
                    BankStatementDetailModel.ReferenceKey = ModelItem.ReferenceKey;
                    BankStatementDetailModel.Particulars = ModelItem.Particulars;
                    BankStatementDetailModel.CashFlowTypeKey = ModelItem.CashFlowTypeKey;
                    BankStatementDetailModel.Amount = ModelItem.Amount ?? 0;
                    BankStatementDetailModel.BankTransactionTypeKey = ModelItem.BankTransactionTypeKey;
                    BankStatementDetailModel.TransactionDate = Convert.ToDateTime(ModelItem.TransactionDate);
                    //if (ModelItem.RefKey == null)
                    //    ModelItem.RefKey = BankStatementDetailModel.RefKey;
                    //ModelItem.UpdateType = DbConstants.UpdationType.Update;
                }
            }
        }
        public BankStatementMasterViewModel CreateBankStatementMaster(BankStatementMasterViewModel model)
        {
            FillDropDowns(model);
            try
            {
                List<BankStatementMasterViewModel> BankStatementList = model.BankStatementDetails
                .GroupBy(x => new
                {
                    x.TransactionDate.Value.Month,
                    x.TransactionDate.Value.Year
                })
                .Select(x => new BankStatementMasterViewModel
                {
                    MonthKey = Convert.ToByte(x.Key.Month),
                    YearKey = x.Key.Year,
                    BankAccountKey = model.BankAccountKey,
                    BranchKey = model.BranchKey,
                    BankStatementDetails = model.BankStatementDetails.Where(y => y.TransactionDate.Value.Month == x.Key.Month && y.TransactionDate.Value.Year == x.Key.Year).ToList()
                }).ToList();
                CreateMultipleBankStatementMaster(BankStatementList, model);
                ActivityLog.CreateActivityLog(MenuConstants.BankStatement, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);
            }
            catch (Exception Ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.BankStatement, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, Ex.GetBaseException().Message);
                model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.BankStatement);
                model.IsSuccessful = false;
            }

            return model;
        }
        private void CreateMultipleBankStatementMaster(List<BankStatementMasterViewModel> modelList, BankStatementMasterViewModel objViewModel)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    detailMaxKey = dbContext.BankStatementDetails.Select(m => m.RowKey).DefaultIfEmpty().Max();
                    Int64 maxKey = dbContext.BankStatementMasters.Select(m => m.RowKey).DefaultIfEmpty().Max();
                    foreach (BankStatementMasterViewModel model in modelList)
                    {
                        long masterKey = dbContext.BankStatementMasters.Where(x => x.BankAccountKey == model.BankAccountKey && x.MonthKey == model.MonthKey && x.YearKey == model.YearKey).Select(x => x.RowKey).DefaultIfEmpty().FirstOrDefault();
                        if (masterKey != 0)
                        {
                            //model.RefKey = dbContext.BankStatementMasters.Where(x => x.BankAccountKey == model.BankAccountKey).Select(x => x.RefKey).FirstOrDefault();
                            createBankStatementDetails(model.BankStatementDetails.Where(x => x.TransactionDate.Value.Month == model.MonthKey && x.TransactionDate.Value.Year == model.YearKey).ToList(), masterKey);

                        }
                        else
                        {
                            BankStatementMaster BankStatementMasterModel = new BankStatementMaster();
                            BankStatementMasterModel.RowKey = Convert.ToInt32(maxKey + 1);
                            BankStatementMasterModel.BankAccountKey = model.BankAccountKey;
                            BankStatementMasterModel.BranchKey = model.BranchKey;
                            BankStatementMasterModel.YearKey = model.YearKey ?? 0;
                            BankStatementMasterModel.MonthKey = model.MonthKey ?? 0;
                            dbContext.BankStatementMasters.Add(BankStatementMasterModel);
                            maxKey++;
                            createBankStatementDetails(model.BankStatementDetails.Where(x => x.TransactionDate.Value.Month == model.MonthKey && x.TransactionDate.Value.Year == model.YearKey).ToList(), BankStatementMasterModel.RowKey);
                            model.RowKey = BankStatementMasterModel.RowKey;
                           // model.UpdateType = DbConstants.UpdationType.Create;
                        }
                    }
                    dbContext.SaveChanges();
                    transaction.Commit();
                    objViewModel.Message = EduSuiteUIResources.Success;
                    objViewModel.IsSuccessful = true;
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    Exception raise = dbEx;
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = string.Format("{0}:{1}", validationErrors.Entry.Entity.ToString(), validationError.ErrorMessage);
                            //raise a new exception inserting the current one as the InnerException
                            raise = new InvalidOperationException(message, raise);
                        }
                    }
                    throw raise;
                }
                catch (Exception ex)
                {
                    objViewModel.Message = ex.GetBaseException().Message;
                    objViewModel.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.BankStatement);
                    objViewModel.IsSuccessful = false;
                }
            }
           

        }
        public BankStatementMasterViewModel UpdateBankStatementMaster(BankStatementMasterViewModel model)
        {
            FillDropDowns(model);

            BankStatementMaster BankStatementMasterModel = new BankStatementMaster();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    BankStatementMasterModel = dbContext.BankStatementMasters.SingleOrDefault(row => row.RowKey == model.RowKey);

                    BankStatementMasterModel.BankAccountKey = model.BankAccountKey;
                    BankStatementMasterModel.BranchKey = model.BranchKey;
                    BankStatementMasterModel.MonthKey = model.MonthKey ?? 0;
                    BankStatementMasterModel.YearKey = model.YearKey ?? 0;
                    detailMaxKey = dbContext.BankStatementDetails.Select(m => m.RowKey).DefaultIfEmpty().Max();
                    updateBankStatementDetails(model.BankStatementDetails.Where(x => x.RowKey != 0).ToList());
                    createBankStatementDetails(model.BankStatementDetails.Where(x => x.RowKey == 0).ToList(), BankStatementMasterModel.RowKey);
                    dbContext.SaveChanges();
                    transaction.Commit();
                   // model.RefKey = BankStatementMasterModel.RefKey;
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.BankStatement, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
				}
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    Exception raise = dbEx;
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = string.Format("{0}:{1}", validationErrors.Entry.Entity.ToString(), validationError.ErrorMessage);
                            //raise a new exception inserting the current one as the InnerException
                            raise = new InvalidOperationException(message, raise);
                        }
                    }
                    throw raise;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.BankStatement);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.BankStatement, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
           
            return model;
        }
        private void FillDropDowns(BankStatementMasterViewModel model)
        {
            GetBranches(model);
            FillBankAccounts(model);
        }
        private void FillCashFlowType(BankStatementDetailsViewModel model)
        {
            model.CashFlowTypes = dbContext.CashFlowTypes.Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.CashFlowTypeName
            }).ToList();
        }
        private void FillBanKTransactionTypes(BankStatementDetailsViewModel model)
        {
            model.BankTransactionTypes = dbContext.BankTransactionTypes.Select(x => new GroupSelectListModel
            {
                RowKey = x.RowKey,
                Text = x.BankTransactionTypeName + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.OpenBracket + x.BankTransactionTypeCode + EduSuiteUIResources.CloseBracket,
                GroupName = x.BankTransactionTypeCode
            }).ToList();
        }
        public BankStatementMasterViewModel GetBranches(BankStatementMasterViewModel model)
        {
            var UserBranchKey = dbContext.Employees.Where(row => row.AppUserKey == DbConstants.User.UserKey).Select(row => row.BranchKey).FirstOrDefault();

            IQueryable<vwBranchSelectActiveOnly> Branches = dbContext.vwBranchSelectActiveOnlies;
            if (UserBranchKey != 0)
            {
                Branches = Branches.Where(row => row.RowKey == UserBranchKey);
                model.BranchKey = UserBranchKey;
            }
            model.Branches = Branches.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BranchName
            }).ToList();
            return model;
        }
        public BankStatementMasterViewModel FillBankAccounts(BankStatementMasterViewModel model)
        {           
            model.BankAccounts = dbContext.BranchAccounts.Where(x => x.BranchKey == model.BranchKey && x.BankAccount.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.BankAccount.RowKey,
                Text = (row.BankAccount.NameInAccount ?? row.BankAccount.AccountNumber) + EduSuiteUIResources.Hyphen + row.BankAccount.Bank.BankName
            }).ToList();

            return model;
        }
        public BankStatementMasterViewModel DeleteBankStatement(BankStatementMasterViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    BankStatementMaster BankStatementMaster = dbContext.BankStatementMasters.SingleOrDefault(row => row.RowKey == model.RowKey);
                    List<BankStatementDetail> BankStatementDetailList = dbContext.BankStatementDetails.Where(row => row.MasterKey == model.RowKey).ToList();
                    BankStatementDetailList.ForEach(row => dbContext.BankStatementDetails.Remove(row));
                    //model.RefKey = BankStatementMaster.RefKey;
                    dbContext.BankStatementMasters.Remove(BankStatementMaster);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.BankStatement, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }

                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.BankStatement);
                        model.IsSuccessful = false;
                    }
                    ActivityLog.CreateActivityLog(MenuConstants.BankStatement, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = ex.GetBaseException().Message;
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.BankStatement);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.BankStatement, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            //if (!model.IsSecond && model.IsSuccessful)
            //{

            //    model.UpdateType = DbConstants.UpdationType.Delete;
            //    TriggerDatabaseInBackground(model);
            //}
            return model;
        }
        public BankStatementDetailsViewModel DeleteBankStatementItem(BankStatementDetailsViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    BankStatementDetail BankStatementDetail = dbContext.BankStatementDetails.SingleOrDefault(row => row.RowKey == model.RowKey);
                    //model.RefKey = BankStatementDetail.RefKey;
                    dbContext.BankStatementDetails.Remove(BankStatementDetail);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.BankStatement, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.BankStatement);
                        model.IsSuccessful = false;
                    }
                    ActivityLog.CreateActivityLog(MenuConstants.BankStatement, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = ex.GetBaseException().Message;
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.BankStatement);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.BankStatement, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            //if (!model.IsSecond && model.IsSuccessful)
            //{

            //    model.UpdateType = DbConstants.UpdationType.Delete;
            //    TriggerDatabaseInBackgroundItem(model);
            //}
            return model;
        }

        //#region TriggerDatabase
        //public void TriggerDatabaseInBackground(BankStatementMasterViewModel model)
        //{
        //    if (Thread.CurrentPrincipal.Identity.IsAuthenticated && (Thread.CurrentPrincipal as CITSPrintSWPrincipal).RoleKey != DbConstants.TaxUserRoleKey && ConnectionTool.CheckConnection())
        //    {
        //        Thread bgThread = new Thread(new ParameterizedThreadStart(TriggerDatabase));
        //        bgThread.IsBackground = true;
        //        bgThread.Start(model);
        //    }

        //}
        //public void TriggerDatabaseInBackgroundItem(BankStatementDetailsViewModel model)
        //{
        //    if (Thread.CurrentPrincipal.Identity.IsAuthenticated && (Thread.CurrentPrincipal as CITSPrintSWPrincipal).RoleKey != DbConstants.TaxUserRoleKey && ConnectionTool.CheckConnection())
        //    {
        //        Thread bgThread = new Thread(new ParameterizedThreadStart(TriggerDatabaseItem));
        //        bgThread.IsBackground = true;
        //        bgThread.Start(model);
        //    }

        //}
        //public void TriggerDatabaseInBackgroundList(List<BankStatementMasterViewModel> model)
        //{
        //    if (Thread.CurrentPrincipal.Identity.IsAuthenticated && (Thread.CurrentPrincipal as CITSPrintSWPrincipal).RoleKey != DbConstants.TaxUserRoleKey && ConnectionTool.CheckConnection())
        //    {
        //        Thread bgThread = new Thread(new ParameterizedThreadStart(TriggerDatabaseList));
        //        bgThread.IsBackground = true;
        //        bgThread.Start(model);
        //    }

        //}
        //private void TriggerDatabaseList(Object ObjViewModel)
        //{
        //    try
        //    {
        //        BankStatementMasterViewModel objViewModel = new BankStatementMasterViewModel();
        //        List<BankStatementMasterViewModel> modelList = (List<BankStatementMasterViewModel>)ObjViewModel;
        //        dbContext = new CITSPrintSWDatabase2();
        //        foreach (BankStatementMasterViewModel model in modelList)
        //        {
        //            var masterRowKey = model.RowKey;
        //            model.RowKey = (model.RefKey ?? 0);
        //            model.RefKey = masterRowKey;
        //            model.IsSecond = true;
        //            foreach (BankStatementDetailsViewModel row in model.BankStatementDetails)
        //            {
        //                var rowKey = row.RowKey;
        //                row.RowKey = (row.RefKey ?? 0);
        //                row.RefKey = rowKey;
        //            }
        //        }
        //        CreateMultipleBankStatementMaster(modelList, objViewModel);
        //        dbContext = new CITSPrintSWDatabase();
        //        foreach (BankStatementMasterViewModel model in modelList)
        //        {
        //            BankStatementMaster objModel = dbContext.BankStatementMasters.SingleOrDefault(row => row.RowKey == model.RefKey);
        //            if (objModel != null)
        //            {
        //                objModel.RefKey = model.RowKey;
        //            }
        //            foreach (BankStatementDetailsViewModel item in model.BankStatementDetails)
        //            {
        //                BankStatementDetail objDetailModel = dbContext.BankStatementDetails.SingleOrDefault(row => row.RowKey == item.RefKey);
        //                if (objDetailModel != null)
        //                {
        //                    objDetailModel.RefKey = item.RowKey;
        //                }
        //            }
        //        }
        //        dbContext.SaveChanges();

        //        //Thread.CurrentThread.IsAlive();
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    finally
        //    {
        //        dbContext = new CITSPrintSWDatabase();
        //        Thread.CurrentThread.Abort();
        //    }
        //}
        //private void TriggerDatabase(Object ObjViewModel)
        //{
        //    try
        //    {
        //        BankStatementMasterViewModel model = (BankStatementMasterViewModel)ObjViewModel;
        //        dbContext = new CITSPrintSWDatabase2();
        //        var RowKey = model.RowKey;
        //        model.IsSecond = true;

        //        foreach (BankStatementDetailsViewModel row in model.BankStatementDetails)
        //        {
        //            var rowKey = row.RowKey;
        //            row.RowKey = (row.RefKey ?? 0);
        //            row.RefKey = rowKey;
        //        }

        //        if (model.UpdateType == DbConstants.UpdationType.Create)
        //        {
        //            CreateBankStatementMaster(model);
        //            dbContext = new CITSPrintSWDatabase();
        //            BankStatementMaster BankStatementMasterModel = dbContext.BankStatementMasters.SingleOrDefault(row => row.RowKey == RowKey);
        //            if (BankStatementMasterModel != null)
        //            {
        //                BankStatementMasterModel.RefKey = model.RowKey;
        //            }
        //            dbContext.SaveChanges();
        //        }
        //        else if (model.UpdateType == DbConstants.UpdationType.Update)
        //        {
        //            model.RowKey = (int)(model.RefKey ?? 0);
        //            if ((model.RefKey ?? 0) != 0)
        //                UpdateBankStatementMaster(model);
        //        }
        //        else if (model.UpdateType == DbConstants.UpdationType.Delete)
        //        {
        //            model.RowKey = (int)(model.RefKey ?? 0);
        //            if ((model.RefKey ?? 0) != 0)
        //                DeleteBankStatement(model);
        //        }
        //        if (model.UpdateType != DbConstants.UpdationType.Delete)
        //        {
        //            dbContext = new CITSPrintSWDatabase();

        //            BankStatementMaster BankStatementMasterModel = dbContext.BankStatementMasters.SingleOrDefault(row => row.RowKey == RowKey);
        //            if (BankStatementMasterModel != null)
        //            {
        //                BankStatementMasterModel.RefKey = model.RowKey;

        //            }
        //            foreach (BankStatementDetailsViewModel item in model.BankStatementDetails)
        //            {
        //                BankStatementDetail objModel = dbContext.BankStatementDetails.SingleOrDefault(row => row.RowKey == item.RefKey);
        //                if (objModel != null)
        //                {
        //                    objModel.RefKey = item.RowKey;
        //                }
        //            }
        //        }
        //        //Thread.CurrentThread.IsAlive();
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    finally
        //    {
        //        dbContext = new CITSPrintSWDatabase();
        //        Thread.CurrentThread.Abort();
        //    }
        //}
        //private void TriggerDatabaseItem(Object ObjViewModel)
        //{
        //    try
        //    {
        //        BankStatementDetailsViewModel model = (BankStatementDetailsViewModel)ObjViewModel;
        //        dbContext = new CITSPrintSWDatabase2();
        //        var RowKey = model.RowKey;
        //        model.IsSecond = true;
        //        if (model.UpdateType == DbConstants.UpdationType.Delete)
        //        {
        //            model.RowKey = (int)(model.RefKey ?? 0);
        //            if ((model.RefKey ?? 0) != 0)
        //                DeleteBankStatementItem(model);
        //        }
        //        //Thread.CurrentThread.IsAlive();
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    finally
        //    {
        //        dbContext = new CITSPrintSWDatabase();
        //        Thread.CurrentThread.Abort();
        //    }
        //}

        //#endregion
    }
}
