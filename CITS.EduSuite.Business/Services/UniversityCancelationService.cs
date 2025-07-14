using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Data;
using CITS.EduSuite.Business.Models.ViewModels;
using System.Linq.Expressions;
using CITS.EduSuite.Business.Models.Resources;
using System.Data.Entity.Infrastructure;
using CITS.EduSuite.Business.Interfaces;

namespace CITS.EduSuite.Business.Services
{
    public class UniversityCancelationService : IUniversityCancelationService
    {
        private EduSuiteDatabase dbContext;

        public UniversityCancelationService(EduSuiteDatabase objEduSuiteDatabase)
        {
            this.dbContext = objEduSuiteDatabase;
        }

        public List<UniversityCancelationViewModel> GetUniversityCancelation(UniversityCancelationViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;

                IQueryable<UniversityCancelationViewModel> UniversityCancelationList = (from UC in dbContext.UniversityPaymentCancelations
                                                                                        where (UC.UniversityPaymentDetail.UniversityPaymentMaster.VoucherNo.Contains(model.SearchText) ||
                                                                                       UC.UniversityPaymentDetail.UniversityPaymentMaster.Application.StudentName.Contains(model.SearchText)
                                                                                        || UC.UniversityPaymentDetail.UniversityPaymentMaster.Application.AdmissionNo.Contains(model.SearchText))
                                                                                        select new UniversityCancelationViewModel
                                                                                        {
                                                                                            RowKey = UC.RowKey,
                                                                                            ApplicationKey = UC.ApplicationKey,
                                                                                            UniversityPaymentDetailsKey = UC.UniversityPaymentDetailsKey,
                                                                                            TotalAmount = UC.TotalAmount,
                                                                                            IfServiceCharge = UC.IfServiceCharge,
                                                                                            ServiceFee = UC.ServiceFee,
                                                                                            AccountHeadKey = UC.AccountHeadKey,
                                                                                            TotalDeductionFee = UC.TotalDeductionFee,
                                                                                            Remarks = UC.Remarks,
                                                                                            CancelDate = UC.CancelDate,
                                                                                            StudentName = UC.UniversityPaymentDetail.UniversityPaymentMaster.Application.StudentName,
                                                                                            VoucherNo = UC.UniversityPaymentDetail.UniversityPaymentMaster.VoucherNo,
                                                                                            PaymentModeName = UC.UniversityPaymentDetail.UniversityPaymentMaster.PaymentMode.PaymentModeName,
                                                                                            PaymentModeSubName = UC.UniversityPaymentDetail.UniversityPaymentMaster.PaymentModeSub.PaymentModeSubName,
                                                                                            ChequeOrDDNumber = UC.UniversityPaymentDetail.UniversityPaymentMaster.ChequeOrDDNumber,
                                                                                            BranchKey = UC.UniversityPaymentDetail.UniversityPaymentMaster.Application.BranchKey
                                                                                        });

                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
                if (Employee != null)
                {
                    if (Employee.BranchAccess != null)
                    {
                        var Branches = Employee.BranchAccess.Split(',').Select(Int16.Parse).ToList();
                        UniversityCancelationList = UniversityCancelationList.Where(row => Branches.Contains(row.BranchKey ?? 0));
                    }
                }

                if (model.BranchKey != 0)
                {
                    UniversityCancelationList = UniversityCancelationList.Where(row => row.BranchKey == model.BranchKey);
                }

                UniversityCancelationList = UniversityCancelationList.GroupBy(x => x.RowKey).Select(y => y.FirstOrDefault());
                if (model.SortBy != "")
                {
                    UniversityCancelationList = SortApplications(UniversityCancelationList, model.SortBy, model.SortOrder);
                }
                TotalRecords = UniversityCancelationList.Count();
                return UniversityCancelationList.Skip(Skip).Take(Take).ToList<UniversityCancelationViewModel>();
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.UniversityPaymentCancel, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<UniversityCancelationViewModel>();
            }

        }

        private IQueryable<UniversityCancelationViewModel> SortApplications(IQueryable<UniversityCancelationViewModel> Query, string SortName, string SortOrder)
        {

            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(UniversityCancelationViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<UniversityCancelationViewModel>(resultExpression);

        }

        public UniversityCancelationViewModel GetUniversityCancelationById(UniversityCancelationViewModel model)
        {
            UniversityCancelationViewModel objViewModel = new UniversityCancelationViewModel();
            try
            {
                if (model.RowKey != 0)
                {
                    objViewModel = dbContext.UniversityPaymentCancelations.Where(x => x.RowKey == model.RowKey).Select(UC => new UniversityCancelationViewModel
                    {
                        RowKey = UC.RowKey,
                        ApplicationKey = UC.ApplicationKey,
                        UniversityPaymentDetailsKey = UC.UniversityPaymentDetailsKey,
                        TotalAmount = UC.TotalAmount,
                        IfServiceCharge = UC.IfServiceCharge,
                        ServiceFee = UC.ServiceFee,
                        AccountHeadKey = UC.AccountHeadKey,
                        TotalDeductionFee = UC.TotalDeductionFee,
                        Remarks = UC.Remarks,
                        CancelDate = UC.CancelDate

                    }).FirstOrDefault();
                }
                else
                {
                    objViewModel = dbContext.UniversityPaymentDetails.Where(x => x.RowKey == model.UniversityPaymentDetailsKey).Select(UC => new UniversityCancelationViewModel
                    {
                        RowKey = 0,
                        ApplicationKey = UC.UniversityPaymentMaster.ApplicationKey,
                        UniversityPaymentDetailsKey = UC.RowKey,
                        TotalAmount = UC.UniversityPaymentAmount,
                        IfServiceCharge = false,
                        ServiceFee = null,
                        AccountHeadKey = null,
                        TotalDeductionFee = UC.UniversityPaymentAmount,
                        Remarks = "",
                        CancelDate = DateTimeUTC.Now

                    }).FirstOrDefault();
                }
                if (objViewModel == null)
                {
                    objViewModel = new UniversityCancelationViewModel();
                }
                FillAccountHead(objViewModel);
                return objViewModel;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.UniversityPaymentCancel, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new UniversityCancelationViewModel();
            }
        }

        public UniversityCancelationViewModel CreateUniversityCancelation(UniversityCancelationViewModel model)
        {

            UniversityPaymentCancelation UniversityCancelationModel = new UniversityPaymentCancelation();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Application application = dbContext.Applications.Where(x => x.RowKey == model.ApplicationKey).FirstOrDefault();
                    Int64 maxKey = dbContext.UniversityPaymentCancelations.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    UniversityCancelationModel.RowKey = maxKey + 1;
                    UniversityCancelationModel.ApplicationKey = model.ApplicationKey;
                    UniversityCancelationModel.UniversityPaymentDetailsKey = model.UniversityPaymentDetailsKey;
                    UniversityCancelationModel.CancelDate = Convert.ToDateTime(model.CancelDate);

                    UniversityCancelationModel.TotalAmount = Convert.ToDecimal(model.TotalAmount);
                    UniversityCancelationModel.IfServiceCharge = model.IfServiceCharge;
                    UniversityCancelationModel.ServiceFee = model.ServiceFee;
                    UniversityCancelationModel.AccountHeadKey = model.AccountHeadKey;
                    UniversityCancelationModel.TotalDeductionFee = model.TotalDeductionFee;
                    UniversityCancelationModel.Remarks = model.Remarks;
                    dbContext.UniversityPaymentCancelations.Add(UniversityCancelationModel);

                    model.RowKey = UniversityCancelationModel.RowKey;
                    long oldAccountHeadKey = 0;

                    List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
                    accountFlowModelList = ReturnAmountList(UniversityCancelationModel, accountFlowModelList, false, application, oldAccountHeadKey);
                    CreateAccountFlow(accountFlowModelList, false);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.UniversityPaymentCancel, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.UniversityPaymentCancelation);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.UniversityPaymentCancel, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public UniversityCancelationViewModel UpdateUniversityCancelation(UniversityCancelationViewModel model)
        {
            UniversityPaymentCancelation UniversityCancelationModel = new UniversityPaymentCancelation();

            using (var transaction = dbContext.Database.BeginTransaction())
            {

                try
                {
                    Application application = dbContext.Applications.Where(x => x.RowKey == model.ApplicationKey).FirstOrDefault();
                    UniversityCancelationModel = dbContext.UniversityPaymentCancelations.SingleOrDefault(p => p.RowKey == model.RowKey);
                    long oldAccountHeadKey = UniversityCancelationModel.AccountHeadKey ?? 0;
                    oldAccountHeadKey = oldAccountHeadKey == 0 ? DbConstants.AccountHead.UniversityPaymentCancelation : oldAccountHeadKey;
                    UniversityCancelationModel.CancelDate = Convert.ToDateTime(model.CancelDate);

                    UniversityCancelationModel.TotalAmount = Convert.ToDecimal(model.TotalAmount);
                    UniversityCancelationModel.IfServiceCharge = model.IfServiceCharge;
                    UniversityCancelationModel.ServiceFee = model.ServiceFee;
                    UniversityCancelationModel.AccountHeadKey = model.AccountHeadKey;
                    UniversityCancelationModel.TotalDeductionFee = model.TotalDeductionFee;
                    UniversityCancelationModel.Remarks = model.Remarks;

                    List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
                    accountFlowModelList = ReturnAmountList(UniversityCancelationModel, accountFlowModelList, true, application, oldAccountHeadKey);
                    CreateAccountFlow(accountFlowModelList, true);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.UniversityPaymentCancel, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.UniversityPaymentCancelation);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.UniversityPaymentCancel, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public UniversityCancelationViewModel DeleteUniversityCancelation(UniversityCancelationViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    UniversityPaymentCancelation UniversityCancelationModel = dbContext.UniversityPaymentCancelations.SingleOrDefault(row => row.RowKey == model.RowKey);

                    dbContext.UniversityPaymentCancelations.Remove(UniversityCancelationModel);
                    AccountFlowViewModel accountFlowModel = new AccountFlowViewModel();
                    accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.UniversityPaymentCancelation;
                    accountFlowModel.TransactionKey = model.RowKey;
                    AccountFlowService accountFlowService = new AccountFlowService(dbContext);
                    accountFlowService.DeleteAccountFlow(accountFlowModel);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.UniversityPaymentCancel, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.UniversityPaymentCancelation);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.UniversityPaymentCancel, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.UniversityPaymentCancelation);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.UniversityPaymentCancel, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public void FillAccountHead(UniversityCancelationViewModel model)
        {

            UniversityPaymentDetail universityPaymentDetail = dbContext.UniversityPaymentDetails.Where(x => x.RowKey == model.UniversityPaymentDetailsKey).FirstOrDefault();

            model.AccountHead = dbContext.FeeTypes.Where(row => row.RowKey != universityPaymentDetail.FeeTypeKey && row.IsActive && row.IsUniverisity == true && row.AccountHead.AccountHeadType.AccountGroupKey == DbConstants.AccountGroup.Expenses).Select(row => new SelectListModel
            {
                RowKey = row.AccountHeadKey ?? 0,
                Text = row.AccountHead.AccountHeadName
            }).ToList();
        }

        private List<AccountFlowViewModel> ReturnAmountList(UniversityPaymentCancelation model, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, Application application, long oldAccountHeadKey)
        {
            UniversityPaymentDetail universityPaymentDetail = dbContext.UniversityPaymentDetails.Where(x => x.RowKey == model.UniversityPaymentDetailsKey).FirstOrDefault();
            short? BranchKey = universityPaymentDetail.UniversityPaymentMaster.PaidBranchKey != null ? universityPaymentDetail.UniversityPaymentMaster.PaidBranchKey : application.BranchKey;

            long accountHeadKey;
            long ExtraUpdateKey = 0;




            accountHeadKey = model.AccountHeadKey ?? 0;
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                OldAccountHeadKey = oldAccountHeadKey,
                CashFlowTypeKey = DbConstants.CashFlowType.In,
                AccountHeadKey = accountHeadKey == 0 ? DbConstants.AccountHead.UniversityPaymentCancelation : accountHeadKey,
                Amount = model.ServiceFee ?? 0,
                TransactionTypeKey = DbConstants.TransactionType.UniversityPaymentCancelation,
                VoucherTypeKey = DbConstants.VoucherType.UniversityPaymentCancelation,
                TransactionKey = model.RowKey,
                TransactionDate = model.CancelDate ?? DateTimeUTC.Now,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = BranchKey,
                Purpose = EduSuiteUIResources.UniversityPaymentCancelation + EduSuiteUIResources.BlankSpace + application.StudentName + " ( " + application.AdmissionNo + " ) " + model.Remarks,
            });




            if (universityPaymentDetail.UniversityPaymentMaster.PaymentModeKey == DbConstants.PaymentMode.Bank || universityPaymentDetail.UniversityPaymentMaster.PaymentModeKey == DbConstants.PaymentMode.Cheque)
            {
                accountHeadKey = dbContext.BankAccounts.Where(x => x.RowKey == universityPaymentDetail.UniversityPaymentMaster.BankAccountKey).Select(x => x.AccountHeadKey).FirstOrDefault();
            }
            else
            {
                accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.CashAccount).Select(x => x.RowKey).FirstOrDefault();
            }

            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.In,
                AccountHeadKey = accountHeadKey,
                Amount = model.TotalDeductionFee ?? 0,
                TransactionTypeKey = DbConstants.TransactionType.UniversityPaymentCancelation,
                VoucherTypeKey = DbConstants.VoucherType.UniversityPaymentCancelation,
                TransactionDate = model.CancelDate ?? DateTimeUTC.Now,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = BranchKey,
                TransactionKey = model.RowKey,
                Purpose = EduSuiteUIResources.UniversityPaymentCancelation + EduSuiteUIResources.BlankSpace + application.StudentName + " ( " + application.AdmissionNo + " ) " + model.Remarks,

            });

            long FeetypeAccountHeadkey = 0;

            FeetypeAccountHeadkey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == universityPaymentDetail.FeeType.AccountHeadKey).Select(x => x.RowKey).FirstOrDefault();

            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                AccountHeadKey = FeetypeAccountHeadkey,
                Amount = universityPaymentDetail.UniversityPaymentAmount,
                TransactionTypeKey = DbConstants.TransactionType.UniversityPaymentCancelation,
                VoucherTypeKey = DbConstants.VoucherType.UniversityPaymentCancelation,
                TransactionDate = model.CancelDate ?? DateTimeUTC.Now,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = BranchKey,
                TransactionKey = model.RowKey,
                Purpose = EduSuiteUIResources.UniversityPaymentCancelation + EduSuiteUIResources.BlankSpace + application.StudentName + " ( " + application.AdmissionNo + " ) " + model.Remarks,

            });
            return accountFlowModelList;
        }

        private void CreateAccountFlow(List<AccountFlowViewModel> accountFlowModelList, bool isUpadte)
        {
            AccountFlowService accounFlowService = new AccountFlowService(dbContext);
            if (isUpadte == false)
            {
                accounFlowService.CreateAccountFlow(accountFlowModelList);
            }
            else
            {
                accounFlowService.UpdateAccountFlow(accountFlowModelList);
            }
        }
    }
}
