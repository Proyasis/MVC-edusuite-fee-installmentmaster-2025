using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System.Globalization;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class ApplicationFeeInstallmentService : IApplicationFeeInstallmentService
    {
        private EduSuiteDatabase dbContext;
        public ApplicationFeeInstallmentService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public ApplicationFeeInstallmentViewModel GetFeeInstallmentById(ApplicationFeeInstallmentViewModel model)
        {
            try
            {
                ApplicationFeeInstallmentViewModel objViewModel = new ApplicationFeeInstallmentViewModel();
                objViewModel.StartYear = dbContext.Applications.Where(x => x.RowKey == model.ApplicationKey).Select(y => y.StartYear).FirstOrDefault();
                if (model.FeeYear == null)
                {
                    model.FeeYear = objViewModel.StartYear;
                }
                objViewModel.FeeYear = model.FeeYear;

                objViewModel.ApplicationFeeInstallments = dbContext.StudentFeeInstallments.Where(x => x.ApplicationKey == model.ApplicationKey && x.FeeYear == model.FeeYear).Select(row => new FeeInstallmentModel
                {
                    RowKey = row.RowKey,
                    InstallmentYear = row.InstallmentYear,
                    InstallmentMonth = row.InstallmentMonth,
                    FeePaymentDay = row.FeePaymentDay,
                    DueDuration = row.DueDuration,
                    InstallmentAmount = row.InstallmentAmount,
                    DueFineAmount = row.DueFineAmount,
                    SuperFineAmount = row.SuperFineAmount,
                    AutoSMS = row.AutoSMS,
                    AutoEmail = row.AutoEmail,
                    AutoNotificationBeforeDue = row.AutoNotificationBeforeDue,
                    AutoNotificationAfterDue = row.AutoNotificationAfterDue,

                    InitialPayment = row.InitialPayment,
                    BalancePayment = row.BalancePayment,
                    IsInitialPayment = objViewModel.FeeYear == objViewModel.StartYear
                }).ToList();


                if (objViewModel.ApplicationFeeInstallments.Count == 0)
                {
                    objViewModel.ApplicationFeeInstallments.Add(new FeeInstallmentModel { IsInitialPayment = objViewModel.FeeYear == objViewModel.StartYear });

                }


                objViewModel.ApplicationKey = model.ApplicationKey;
                FillDropdownList(objViewModel);

                return objViewModel;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.ApplicationFeeInstallment, ActionConstants.View, DbConstants.LogType.Debug, model.ApplicationKey, ex.GetBaseException().Message);
                return new ApplicationFeeInstallmentViewModel();


            }
        }

        public ApplicationFeeInstallmentViewModel UpdateFeeInstallment(ApplicationFeeInstallmentViewModel model)
        {
            // FillDropdownList(model);
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                   

                    CreateInstallment(model.ApplicationFeeInstallments.Where(row => row.RowKey == 0).ToList(), model.FeeYear ?? 0, model.ApplicationKey);

                   

                    UpdateInstallment(model.ApplicationFeeInstallments.Where(row => row.RowKey != 0).ToList());
                    //CreateAccountFlow(accountFlowModelList, false);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.AdmissionNo = dbContext.Applications.Where(row => row.RowKey == model.ApplicationKey).Select(row => row.AdmissionNo).FirstOrDefault();
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationDocument, (model.ApplicationFeeInstallments.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Info, model.ApplicationKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.FeeInstallment);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationDocument, (model.ApplicationFeeInstallments.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, model.ApplicationKey, ex.GetBaseException().Message);
                }

            }
            return model;
        }

        #region Account
        private void CreateAccountFlow(List<AccountFlowViewModel> modelList, bool IsUpdate)
        {
            AccountFlowService accounFlowService = new AccountFlowService(dbContext);
            List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
            if (IsUpdate != true)
            {
                accounFlowService.CreateAccountFlow(modelList);
            }
            else
            {
                accounFlowService.UpdateAccountFlow(modelList);
            }
        }
        private void RecievableAmountList(decimal Amount, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, Application ApplicationModel)
        {
            long ExtraUpdateKey = 0;
            long AccountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.AccountsReceivable).Select(x => x.RowKey).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.In,
                AccountHeadKey = AccountHeadKey,
                Amount = Amount,
                TransactionTypeKey = DbConstants.TransactionType.Application,
                TransactionDate = Convert.ToDateTime(ApplicationModel.StudentDateOfAdmission),
                TransactionKey = ApplicationModel.RowKey,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                VoucherTypeKey = DbConstants.VoucherType.FeeInstallment,
                BranchKey = ApplicationModel.BranchKey,
                Purpose = EduSuiteUIResources.FeeInstallment + EduSuiteUIResources.BlankSpace + ApplicationModel.StudentName + " ( " + ApplicationModel.AdmissionNo + " ) ",
            });

        }
        private void RecievableInstallmentAmountList(decimal Amount, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, Application ApplicationModel, FeeInstallmentModel feeinstallmentmodel)
        {
            long ExtraUpdateKey = 0;
            long AccountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.AccountsReceivable).Select(x => x.RowKey).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.In,
                AccountHeadKey = AccountHeadKey,
                Amount = Amount,
                TransactionTypeKey = DbConstants.TransactionType.FeeInstallment,
                TransactionDate = Convert.ToDateTime(feeinstallmentmodel.InstallmentYear + "-" + feeinstallmentmodel.InstallmentMonth + "-" + feeinstallmentmodel.FeePaymentDay),
                TransactionKey = feeinstallmentmodel.RowKey,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                VoucherTypeKey = DbConstants.VoucherType.FeeInstallment,
                BranchKey = ApplicationModel.BranchKey,
                Purpose = EduSuiteUIResources.FeeInstallment + EduSuiteUIResources.BlankSpace + ApplicationModel.StudentName + " ( " + ApplicationModel.AdmissionNo + " ) ",
            });

        }

        #endregion
        private void CreateInstallment(List<FeeInstallmentModel> modelList, int FeeYear, Int64 ApplicationKey)
        {
            Int64 maxKey = dbContext.StudentFeeInstallments.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (FeeInstallmentModel model in modelList)
            {
                StudentFeeInstallment applicationFeeInstallmentModel = new StudentFeeInstallment();
                applicationFeeInstallmentModel.RowKey = Convert.ToInt64(maxKey + 1);
                applicationFeeInstallmentModel.ApplicationKey = ApplicationKey;
                applicationFeeInstallmentModel.InstallmentMonth = model.InstallmentMonth;
                applicationFeeInstallmentModel.InstallmentYear = model.InstallmentYear;
                applicationFeeInstallmentModel.FeePaymentDay = model.FeePaymentDay;
                applicationFeeInstallmentModel.DueDuration = model.DueDuration;
                applicationFeeInstallmentModel.InstallmentAmount = model.InstallmentAmount;
                applicationFeeInstallmentModel.DueFineAmount = model.DueFineAmount;
                applicationFeeInstallmentModel.SuperFineAmount = model.SuperFineAmount;
                applicationFeeInstallmentModel.AutoSMS = model.AutoSMS;
                applicationFeeInstallmentModel.AutoEmail = model.AutoEmail;
                applicationFeeInstallmentModel.AutoNotificationBeforeDue = model.AutoNotificationBeforeDue;
                applicationFeeInstallmentModel.AutoNotificationAfterDue = model.AutoNotificationAfterDue;
                applicationFeeInstallmentModel.FeeYear = FeeYear;
                applicationFeeInstallmentModel.InitialPayment = model.InitialPayment;
                applicationFeeInstallmentModel.BalancePayment = model.BalancePayment;


                dbContext.StudentFeeInstallments.Add(applicationFeeInstallmentModel);

                maxKey++;

            }

        }

        public void UpdateInstallment(List<FeeInstallmentModel> modelList)
        {
            foreach (FeeInstallmentModel model in modelList)
            {
                StudentFeeInstallment applicationFeeInstallmentModel = new StudentFeeInstallment();

                applicationFeeInstallmentModel = dbContext.StudentFeeInstallments.SingleOrDefault(row => row.RowKey == model.RowKey);
                applicationFeeInstallmentModel.InstallmentMonth = model.InstallmentMonth;
                applicationFeeInstallmentModel.InstallmentYear = model.InstallmentYear;
                applicationFeeInstallmentModel.FeePaymentDay = model.FeePaymentDay;
                applicationFeeInstallmentModel.DueDuration = model.DueDuration;
                applicationFeeInstallmentModel.InstallmentAmount = model.InstallmentAmount;
                applicationFeeInstallmentModel.DueFineAmount = model.DueFineAmount;
                applicationFeeInstallmentModel.SuperFineAmount = model.SuperFineAmount;
                applicationFeeInstallmentModel.AutoSMS = model.AutoSMS;
                applicationFeeInstallmentModel.AutoEmail = model.AutoEmail;
                applicationFeeInstallmentModel.AutoNotificationBeforeDue = model.AutoNotificationBeforeDue;
                applicationFeeInstallmentModel.AutoNotificationAfterDue = model.AutoNotificationAfterDue;
                applicationFeeInstallmentModel.InitialPayment = model.InitialPayment;
                applicationFeeInstallmentModel.BalancePayment = model.BalancePayment;

            }
        }

        private void FillFeeYears(ApplicationFeeInstallmentViewModel model)
        {
            Application Application = dbContext.Applications.SingleOrDefault(row => row.RowKey == model.ApplicationKey);

            if (Application != null)
            {
                var CourseDuration = Application.Course.CourseDuration;
              var duration = Math.Ceiling((Convert.ToDecimal(Application.AcademicTermKey == DbConstants.AcademicTerm.Semester ? CourseDuration / 6 : CourseDuration / 12)));

                var StartYear = Application.StartYear ?? 0;

                if (duration < 1)
                {
                    model.FeeYears.Add(new SelectListModel
                    {
                        RowKey = 1,
                        Text = " Short Term"
                    });
                }
                else
                {
                    for (int i = StartYear; i <= duration; i++)
                    {
                        model.FeeYears.Add(new SelectListModel
                        {
                            RowKey = i,
                            Text = i + (Application.AcademicTermKey == DbConstants.AcademicTerm.Semester ? " Semester" : " Year")
                        });
                    }
                }
            }


        }

        public ApplicationFeeInstallmentViewModel DeleteFeeInstallment(FeeInstallmentModel model)
        {
            ApplicationFeeInstallmentViewModel applicationFeeInstallmentModel = new ApplicationFeeInstallmentViewModel();
            // FillDropdownList(applicationDocumentModel);
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    StudentFeeInstallment FeeInstallment = dbContext.StudentFeeInstallments.SingleOrDefault(row => row.RowKey == model.RowKey);

                    dbContext.StudentFeeInstallments.Remove(FeeInstallment);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    applicationFeeInstallmentModel.Message = EduSuiteUIResources.Success;
                    applicationFeeInstallmentModel.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationFeeInstallment, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, applicationFeeInstallmentModel.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        applicationFeeInstallmentModel.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.FeeInstallment);
                        applicationFeeInstallmentModel.IsSuccessful = false;

                        ActivityLog.CreateActivityLog(MenuConstants.ApplicationFeeInstallment, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    applicationFeeInstallmentModel.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.FeeInstallment);
                    applicationFeeInstallmentModel.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationFeeInstallment, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return applicationFeeInstallmentModel;
        }


        private void FillDropdownList(ApplicationFeeInstallmentViewModel model)
        {
            FillFeeYears(model);
            FillAmount(model);
            FillInstallmentMonth(model);
        }


        private void FillAmount(ApplicationFeeInstallmentViewModel model)
        {
            List<AdmissionFee> AdmissionFee = dbContext.AdmissionFees.Where(row => row.ApplicationKey == model.ApplicationKey && (row.AdmissionFeeYear ?? model.StartYear) == model.FeeYear).ToList();

            if (AdmissionFee != null)
            {
                model.FeeAmount = AdmissionFee.Select(x => x.AdmissionFeeAmount).Sum();
            }

        }

        private void FillInstallmentMonth(ApplicationFeeInstallmentViewModel model)
        {
            var application = dbContext.Applications.Where(x => x.RowKey == model.ApplicationKey).SingleOrDefault();

            short Duartion = application.AcademicTerm.Duration;

            var start = Convert.ToDateTime(application.StudentDateOfAdmission).AddMonths(Duartion * ((model.FeeYear ?? 0) - 1) + 1);
            var end = Convert.ToDateTime(application.StudentDateOfAdmission).AddMonths(Duartion * (model.FeeYear ?? 0) + 1);

            // set end-date to end of month
            end = new DateTime(end.Year, end.Month, DateTime.DaysInMonth(end.Year, end.Month));

            var Dates = Enumerable.Range(0, Int32.MaxValue)
                                 .Select(e => start.AddMonths(e))
                                 .TakeWhile(e => e <= end)
                                 .Select(e => e);

            foreach (DateTime Date in Dates)
            {
                model.InstallMentMonth.Add(new SelectListModel
                {
                    Text = Date.ToString("MMM yyyy"),
                    ValueText = Date.ToString("yyyy-MM")
                });
            }

        }

    }
}
