using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System.Linq.Expressions;
using CITS.EduSuite.Business.Models.Resources;
using System.Globalization;

namespace CITS.EduSuite.Business.Services
{
    public class UniversityPaymentService : IUniversityPaymentService
    {
        private EduSuiteDatabase dbContext;

        public UniversityPaymentService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<UniversityPaymentViewmodel> GetUniversityFeePaymentsByApplication(long Id)
        {
            try
            {

                Application Application = dbContext.Applications.SingleOrDefault(row => row.RowKey == Id);


                var CourseDuration = Application.Course.CourseDuration;
                var duration = Math.Ceiling((Convert.ToDecimal(Application.AcademicTermKey == DbConstants.AcademicTerm.Semester ? CourseDuration / 6 : CourseDuration / 12)));

                short AcademicTermKey = Application.AcademicTermKey;

                var applicationFeeList = (from es in dbContext.UniversityPaymentMasters.Where(row => row.ApplicationKey == Id)
                                          orderby es.UniversityPaymentDate descending
                                          select new UniversityPaymentViewmodel
                                          {
                                              RowKey = es.RowKey,
                                              ApplicationKey = es.ApplicationKey,
                                              ApplicantName = es.Application.StudentName,
                                              AdmissionNo = es.Application.AdmissionNo,
                                              PaymentModeKey = es.PaymentModeKey,
                                              PaymentModeName = es.PaymentMode.PaymentModeName,
                                              PaymentModeSubKey = es.PaymentModeSubKey,
                                              PaymentModeSubName = es.PaymentModeSub.PaymentModeSubName,
                                              BankAccountKey = es.BankAccountKey ?? 0,
                                              BankAccountName = es.BankAccount.Bank.BankName + "-" + es.BankAccount.AccountNumber,
                                              CardNumber = es.CardNumber,
                                              ReferenceNumber = es.ReferenceNumber,
                                              UniversityPaymentDate = es.UniversityPaymentDate,
                                              ChequeOrDDNumber = es.ChequeOrDDNumber,
                                              ChequeClearanceDate = es.ChequeClearanceDate,
                                              FeeDescription = es.UniversityPaymentNote,
                                              CenterShareAmount = es.CenterShareAmount,
                                              VoucherNo = es.VoucherNo,
                                              ChequeStatusKey = es.ChequeStatusKey,
                                              ChequeAction = es.ChequeStatusKey == null ? "" : (es.ChequeStatusKey == DbConstants.ProcessStatus.Approved ? EduSuiteUIResources.Approved : EduSuiteUIResources.Rejected),
                                              ChequeApprovedRejectedDate = es.ChequeApprovedRejectedDate,
                                              ChequeRejectedRemarks = es.ChequeRejectedRemarks,
                                              PaidBranchKey = es.PaidBranchKey ?? 0,

                                              UniversityFeePaymentDetails = dbContext.UniversityPaymentDetails.Where(row => row.UniversiyPaymenMasterKey == es.RowKey).Select(row => new UniversityPaymentDetailsmodel
                                              {

                                                  FeeTypeKey = row.FeeTypeKey,
                                                  UniversityPaymentAmount = row.UniversityPaymentAmount,
                                                  UniversityPaymentYear = row.UniversityPaymentYear,

                                                  FeeTypeName = row.FeeType.FeeTypeName

                                              }).ToList(),
                                              IsCancel = es.UniversityPaymentDetails.Where(y => dbContext.UniversityPaymentCancelations.Select(x => x.UniversityPaymentDetailsKey).ToList().Contains(y.RowKey)).Any(),

                                          }).ToList();

                foreach (UniversityPaymentViewmodel PaymentDetails in applicationFeeList)
                {
                    foreach (UniversityPaymentDetailsmodel item in PaymentDetails.UniversityFeePaymentDetails)
                    {
                        item.FeeYearText = item.UniversityPaymentYear != null ? (CommonUtilities.GetYearDescriptionByCodeDetails(Application.Course.CourseDuration ?? 0, item.UniversityPaymentYear ?? 0, Application.AcademicTermKey)) : "";
                    }
                }



                return applicationFeeList.GroupBy(x => x.VoucherNo).Select(y => y.First()).OrderByDescending(row => row.VoucherNo).ToList<UniversityPaymentViewmodel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.UniversityPayment, ActionConstants.View, DbConstants.LogType.Error, Id, ex.GetBaseException().Message);
                return new List<UniversityPaymentViewmodel>();


            }
        }

        public UniversityPaymentViewmodel GetUniversityFeePaymentById(UniversityPaymentViewmodel model)
        {
            Application Application = dbContext.Applications.SingleOrDefault(row => row.RowKey == model.ApplicationKey);
            UniversityPaymentViewmodel universityPaymentViewmodel = new UniversityPaymentViewmodel();
            try
            {
                universityPaymentViewmodel = dbContext.UniversityPaymentMasters.Where(row => row.RowKey == model.RowKey).Select(row => new UniversityPaymentViewmodel
                {
                    RowKey = row.RowKey,
                    ApplicationKey = row.ApplicationKey,
                    ApplicantName = row.Application.StudentName,
                    AdmissionNo = row.Application.AdmissionNo,
                    UniversityPaymentDate = row.UniversityPaymentDate,
                    VoucherNo = row.VoucherNo,
                    PaymentModeKey = row.PaymentModeKey,
                    PaymentModeName = row.PaymentMode.PaymentModeName,
                    PaymentModeSubKey = row.PaymentModeSubKey,
                    PaymentModeSubName = row.PaymentModeSub.PaymentModeSubName,
                    UniversityPaymentChalanDDNumber = row.UniversityPaymentChalanDDNumber,
                    UniversityPaymentNote = row.UniversityPaymentNote,
                    BankAccountKey = row.BankAccountKey ?? 0,
                    ChequeClearanceDate = row.ChequeClearanceDate,
                    ChequeOrDDNumber = row.ChequeOrDDNumber,
                    FeeDescription = row.UniversityPaymentNote,
                    ReferenceNumber = row.ReferenceNumber,
                    CardNumber = row.CardNumber,
                    ChequeStatusKey = row.ChequeStatusKey,
                    PaidBranchKey = row.PaidBranchKey ?? 0,

                }).SingleOrDefault();
                if (universityPaymentViewmodel == null)
                {
                    universityPaymentViewmodel = new UniversityPaymentViewmodel();
                    universityPaymentViewmodel.RowKey = model.RowKey;
                    universityPaymentViewmodel.ApplicationKey = model.ApplicationKey;
                    GenerateReceiptNo(universityPaymentViewmodel);

                }
                FillUniversityFeePaymentDetails(universityPaymentViewmodel);
                if (universityPaymentViewmodel.UniversityFeePaymentDetails.Count == 0)
                {

                    // universityPaymentViewmodel.InitialPayment = dbContext.T_StudentFeeInstallment.Where(x => x.ApplicationKey == model.ApplicationKey).Select(x => x.InitialPayment).FirstOrDefault();
                    universityPaymentViewmodel.UniversityFeePaymentDetails.Add(new UniversityPaymentDetailsmodel());
                }

                //FillUniversityFeePaymentDetails(universityPaymentViewmodel);
                universityPaymentViewmodel.ApplicationKey = model.ApplicationKey;
                universityPaymentViewmodel.BranchKey = Application.BranchKey;
                universityPaymentViewmodel.ApplicantName = Application.StudentName;
                universityPaymentViewmodel.AdmissionNo = Application.AdmissionNo;
                universityPaymentViewmodel.StudentMobile = Application.StudentMobile;
                universityPaymentViewmodel.StudentEmail = Application.StudentEmail;
                FillDropdownLists(universityPaymentViewmodel);
                FillNotificationDetail(universityPaymentViewmodel);
                return universityPaymentViewmodel;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.UniversityPayment, ActionConstants.View, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                return new UniversityPaymentViewmodel();


            }
        }

        //public void FillApplicationFeeDropdownLists(ApplicationFeePaymentViewModel model)
        //{
        //    FillDropdownLists(model);

        //    model.TotalFee = dbContext.Application.Where(row => row.RowKey == model.ApplicationKey).Select(row => row.StudentTotalFee).FirstOrDefault();
        //    model.TotalPaid = dbContext.FeePaymentDetails.Where(row => row.FeePaymentMaster.ApplicationKey == model.ApplicationKey && row.M_FeeType.CashFlowTypeKey == DbConstants.CashFlowType.In).Select(row => row.FeeAmount).DefaultIfEmpty().Sum();
        //    model.BalanceFee = model.TotalFee - model.TotalPaid;
        //}

        public UniversityPaymentViewmodel CreateUniversityFee(UniversityPaymentViewmodel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    GenerateReceiptNo(model);

                    long maxKey = dbContext.UniversityPaymentMasters.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    UniversityPaymentMaster universityFeePaymentModel = new UniversityPaymentMaster();
                    universityFeePaymentModel.RowKey = Convert.ToInt64(maxKey + 1);
                    //universityFeePaymentModel.SerialNumber = Convert.ToInt64(model.SerialNumber);
                    //universityFeePaymentModel.VoucherNo = model.VoucherNo;

                    universityFeePaymentModel.VoucherNo = model.VoucherNo;
                    universityFeePaymentModel.SerialNumber = model.SerialNumber ?? 0;


                    universityFeePaymentModel.PaymentModeKey = model.PaymentModeKey;
                    universityFeePaymentModel.PaymentModeSubKey = model.PaymentModeSubKey;
                    universityFeePaymentModel.UniversityPaymentDate = Convert.ToDateTime(model.UniversityPaymentDate);
                    universityFeePaymentModel.BankAccountKey = model.BankAccountKey;
                    universityFeePaymentModel.CardNumber = model.CardNumber;
                    universityFeePaymentModel.ReferenceNumber = model.ReferenceNumber;
                    universityFeePaymentModel.ChequeOrDDNumber = model.ChequeOrDDNumber;
                    universityFeePaymentModel.ChequeClearanceDate = model.ChequeClearanceDate;
                    universityFeePaymentModel.UniversityPaymentNote = model.FeeDescription;
                    universityFeePaymentModel.CenterShareAmount = model.CenterShareAmount;
                    universityFeePaymentModel.ApplicationKey = model.ApplicationKey;
                    universityFeePaymentModel.PaidBranchKey = model.PaidBranchKey;

                    dbContext.UniversityPaymentMasters.Add(universityFeePaymentModel);

                    var ApplicationDetails = dbContext.Applications.SingleOrDefault(x => x.RowKey == model.ApplicationKey);
                    decimal totalDebitAmount = model.UniversityFeePaymentDetails.Select(x => x.UniversityPaymentAmount ?? 0).DefaultIfEmpty().Sum();
                    decimal AdvancePayment = 0;
                    if (ApplicationDetails.EducationTypeKey == DbConstants.EducationType.RegulerEducation)
                    {
                        AdvancePayment = model.UniversityFeePaymentDetails.Where(x => x.UniversityPaymentYear > ApplicationDetails.CurrentYear).Select(x => x.UniversityPaymentAmount ?? 0).DefaultIfEmpty().Sum();
                    }
                    long oldBankKey = 0;
                    short oldPaymentModeKey = 0;

                    model.RowKey = universityFeePaymentModel.RowKey;
                    CreateUniversityFeeDetail(model.UniversityFeePaymentDetails.Where(row => row.RowKey == 0).ToList(), model);

                    string purpose = String.Join(",", model.PurposeList);

                    List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
                    GeneralConfiguration generalConfiguration = dbContext.GeneralConfigurations.FirstOrDefault();
                    if (generalConfiguration.UniversityPaymentAccountDate == null || universityFeePaymentModel.UniversityPaymentDate >= generalConfiguration.UniversityPaymentAccountDate)
                    {
                        if (totalDebitAmount != 0)
                        {

                            if (!DbConstants.GeneralConfiguration.AllowUniversityAccountHead)
                            {
                                decimal TotalIncomeAmount = totalDebitAmount - AdvancePayment;
                                ExpenseSplitAmountList(model.UniversityFeePaymentDetails.ToList(), accountFlowModelList, universityFeePaymentModel, false, purpose, TotalIncomeAmount);
                            }
                            else
                            {
                                PayableAmountList(totalDebitAmount - AdvancePayment, accountFlowModelList, universityFeePaymentModel, false, purpose);
                            }

                            PaymentModeAmountList(universityFeePaymentModel, accountFlowModelList, false, oldBankKey, oldPaymentModeKey, totalDebitAmount, purpose);

                            if (ApplicationDetails.EducationTypeKey == DbConstants.EducationType.RegulerEducation)
                            {
                                AdvanceAmountList(AdvancePayment, accountFlowModelList, universityFeePaymentModel, false, purpose);
                            }
                            CreateAccountFlow(accountFlowModelList, false);
                        }
                    }


                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.UniversityPayment, ActionConstants.Add, DbConstants.LogType.Info, universityFeePaymentModel.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.AffiliationsTieUpsPayment);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.UniversityPayment, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            FillDropdownLists(model);
            return model;

        }

        public UniversityPaymentViewmodel UpdateUniversityFee(UniversityPaymentViewmodel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    UniversityPaymentMaster universityFeePaymentModel = new UniversityPaymentMaster();
                    universityFeePaymentModel = dbContext.UniversityPaymentMasters.SingleOrDefault(row => row.RowKey == model.RowKey);

                    short oldPaymentModeKey = universityFeePaymentModel.PaymentModeKey;
                    long oldBankAccountKey = universityFeePaymentModel.BankAccountKey ?? 0;
                    decimal OldAmount = universityFeePaymentModel.UniversityPaymentDetails.Select(x => x.UniversityPaymentAmount).DefaultIfEmpty().Sum();

                    universityFeePaymentModel.SerialNumber = Convert.ToInt64(model.SerialNumber);
                    //universityFeePaymentModel.VoucherNo = model.VoucherNo;
                    universityFeePaymentModel.PaymentModeKey = model.PaymentModeKey;
                    universityFeePaymentModel.PaymentModeSubKey = model.PaymentModeSubKey;
                    universityFeePaymentModel.UniversityPaymentDate = Convert.ToDateTime(model.UniversityPaymentDate);
                    universityFeePaymentModel.BankAccountKey = model.BankAccountKey;
                    universityFeePaymentModel.CardNumber = model.CardNumber;
                    universityFeePaymentModel.ReferenceNumber = model.ReferenceNumber;
                    universityFeePaymentModel.ChequeOrDDNumber = model.ChequeOrDDNumber;
                    universityFeePaymentModel.ChequeClearanceDate = model.ChequeClearanceDate;
                    universityFeePaymentModel.UniversityPaymentNote = model.FeeDescription;
                    universityFeePaymentModel.CenterShareAmount = model.CenterShareAmount;
                    //universityFeePaymentModel.ApplicationKey = model.ApplicationKey;
                    universityFeePaymentModel.PaidBranchKey = model.PaidBranchKey;


                    var ApplicationDetails = dbContext.Applications.SingleOrDefault(x => x.RowKey == model.ApplicationKey);
                    decimal AdvancePayment = 0;
                    if (ApplicationDetails.EducationTypeKey == DbConstants.EducationType.RegulerEducation)
                    {
                        AdvancePayment = model.UniversityFeePaymentDetails.Where(x => x.UniversityPaymentYear > ApplicationDetails.CurrentYear).Select(x => x.UniversityPaymentAmount ?? 0).DefaultIfEmpty().Sum();

                    }
                    decimal totalDebitAmount = model.UniversityFeePaymentDetails.Select(x => x.UniversityPaymentAmount ?? 0).DefaultIfEmpty().Sum();
                    //decimal totalCreditAmount = model.UniversityFeePaymentDetails.Where(x => x.CashFlowTypeKey == DbConstants.CashFlowType.Out).Select(x => x.UniversityPaymentAmount ?? 0).DefaultIfEmpty().Sum();

                    CreateUniversityFeeDetail(model.UniversityFeePaymentDetails.Where(row => row.RowKey == 0).ToList(), model);
                    UpdateUniversityFeeDetail(model.UniversityFeePaymentDetails.Where(row => row.RowKey != 0).ToList(), model);

                    string purpose = String.Join(",", model.PurposeList);
                    List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
                    GeneralConfiguration generalConfiguration = dbContext.GeneralConfigurations.FirstOrDefault();
                    if (generalConfiguration.UniversityPaymentAccountDate == null || universityFeePaymentModel.UniversityPaymentDate >= generalConfiguration.UniversityPaymentAccountDate)
                    {
                        if (totalDebitAmount != 0)
                        {

                            if (!DbConstants.GeneralConfiguration.AllowUniversityAccountHead)
                            {
                                decimal TotalIncomeAmount = totalDebitAmount - AdvancePayment;
                                ExpenseSplitAmountList(model.UniversityFeePaymentDetails.ToList(), accountFlowModelList, universityFeePaymentModel, true, purpose, TotalIncomeAmount);
                            }
                            else
                            {
                                PayableAmountList(totalDebitAmount - AdvancePayment, accountFlowModelList, universityFeePaymentModel, true, purpose);
                            }


                            PaymentModeAmountList(universityFeePaymentModel, accountFlowModelList, true, oldBankAccountKey, oldPaymentModeKey, totalDebitAmount, purpose);
                            if (ApplicationDetails.EducationTypeKey == DbConstants.EducationType.RegulerEducation)
                            {
                                AdvanceAmountList(AdvancePayment, accountFlowModelList, universityFeePaymentModel, true, purpose);
                            }
                            CreateAccountFlow(accountFlowModelList, true);
                        }
                    }


                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.UniversityPayment, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.AffiliationsTieUpsPayment);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.UniversityPayment, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            FillDropdownLists(model);
            return model;
        }

        private void CreateUniversityFeeDetail(List<UniversityPaymentDetailsmodel> modelList, UniversityPaymentViewmodel objViewmodel)
        {
            Int64 MaxKey = dbContext.UniversityPaymentDetails.Select(p => p.RowKey).DefaultIfEmpty().Max();
            Application Application = dbContext.Applications.FirstOrDefault(row => row.RowKey == objViewmodel.ApplicationKey);
            foreach (UniversityPaymentDetailsmodel model in modelList)
            {

                UniversityPaymentDetail universityPaymentDetailModel = new UniversityPaymentDetail();
                universityPaymentDetailModel.RowKey = Convert.ToInt64(MaxKey + 1);
                universityPaymentDetailModel.FeeTypeKey = model.FeeTypeKey;
                universityPaymentDetailModel.UniversityPaymentAmount = Convert.ToDecimal(model.UniversityPaymentAmount);
                universityPaymentDetailModel.UniversityPaymentYear = model.UniversityPaymentYear;
                universityPaymentDetailModel.UniversiyPaymenMasterKey = objViewmodel.RowKey;
                dbContext.UniversityPaymentDetails.Add(universityPaymentDetailModel);
                MaxKey++;

                string FeeTypeName = dbContext.FeeTypes.Where(x => x.RowKey == model.FeeTypeKey).Select(y => y.FeeTypeName).FirstOrDefault();
                string FeeYearText = model.UniversityPaymentYear != null ? (CommonUtilities.GetYearDescriptionByCodeDetails(Application.Course.CourseDuration ?? 0, model.UniversityPaymentYear ?? 0, Application.AcademicTermKey)) : "";

                FeeTypeName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(FeeTypeName.ToLower());
                string Purpose = FeeYearText + EduSuiteUIResources.BlankSpace + FeeTypeName;
                objViewmodel.PurposeList.Add(Purpose);
            }
        }

        private void UpdateUniversityFeeDetail(List<UniversityPaymentDetailsmodel> modelList, UniversityPaymentViewmodel objViewmodel)
        {
            Application Application = dbContext.Applications.FirstOrDefault(row => row.RowKey == objViewmodel.ApplicationKey);
            foreach (UniversityPaymentDetailsmodel model in modelList)
            {
                UniversityPaymentDetail universityPaymentDetailModel = new UniversityPaymentDetail();
                universityPaymentDetailModel = dbContext.UniversityPaymentDetails.SingleOrDefault(x => x.RowKey == model.RowKey);
                universityPaymentDetailModel.FeeTypeKey = model.FeeTypeKey;
                universityPaymentDetailModel.UniversityPaymentAmount = Convert.ToDecimal(model.UniversityPaymentAmount);
                universityPaymentDetailModel.UniversityPaymentYear = model.UniversityPaymentYear;
                //universityPaymentDetailModel.UniversiyPaymenMasterKey = objViewmodel.RowKey;


                string FeeTypeName = dbContext.FeeTypes.Where(x => x.RowKey == model.FeeTypeKey).Select(y => y.FeeTypeName).FirstOrDefault();
                string FeeYearText = model.UniversityPaymentYear != null ? (CommonUtilities.GetYearDescriptionByCodeDetails(Application.Course.CourseDuration ?? 0, model.UniversityPaymentYear ?? 0, Application.AcademicTermKey)) : "";

                FeeTypeName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(FeeTypeName.ToLower());
                string Purpose = FeeYearText + EduSuiteUIResources.BlankSpace + FeeTypeName;
                objViewmodel.PurposeList.Add(Purpose);
            }
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
        private List<AccountFlowViewModel> PaymentModeAmountList(UniversityPaymentMaster universityPaymentMastermodel, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, long oldBankKey, short oldPaymentModeKey, decimal totalDebitAmount, string purpose)
        {
            long accountHeadKey;
            long ExtraUpdateKey = 0;
            long oldBankAccountHeadKey = 0;
            long oldAccountHeadKey;
            var ApplicationDetails = dbContext.Applications.SingleOrDefault(x => x.RowKey == universityPaymentMastermodel.ApplicationKey);
            if (oldPaymentModeKey == DbConstants.PaymentMode.Bank || oldPaymentModeKey == DbConstants.PaymentMode.Cheque)
            {
                oldAccountHeadKey = dbContext.BankAccounts.Where(x => x.RowKey == oldBankKey).Select(x => x.AccountHeadKey).FirstOrDefault();
                oldBankAccountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == oldAccountHeadKey).Select(x => x.RowKey).FirstOrDefault();
            }
            else
            {
                oldAccountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.CashAccount).Select(x => x.RowKey).FirstOrDefault();

            }
            if (universityPaymentMastermodel.PaymentModeKey == DbConstants.PaymentMode.Bank || universityPaymentMastermodel.PaymentModeKey == DbConstants.PaymentMode.Cheque)
            {
                accountHeadKey = dbContext.BankAccounts.Where(x => x.RowKey == universityPaymentMastermodel.BankAccountKey).Select(x => x.AccountHeadKey).FirstOrDefault();
            }
            else
            {
                accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.CashAccount).Select(x => x.RowKey).FirstOrDefault();
            }


            if (oldPaymentModeKey != null && oldPaymentModeKey != 0 && oldPaymentModeKey != universityPaymentMastermodel.PaymentModeKey)
            {
                IsUpdate = false;
                ExtraUpdateKey = oldPaymentModeKey == DbConstants.PaymentMode.Cash ? DbConstants.AccountHead.CashAccount : oldBankAccountHeadKey;
            }


            accountFlowModelList.Add(new AccountFlowViewModel
            {
                OldAccountHeadKey = oldAccountHeadKey,
                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                AccountHeadKey = accountHeadKey,
                Amount = totalDebitAmount,
                TransactionTypeKey = DbConstants.TransactionType.UniversityFee,
                TransactionKey = universityPaymentMastermodel.RowKey,
                TransactionDate = universityPaymentMastermodel.UniversityPaymentDate,
                VoucherTypeKey = DbConstants.VoucherType.UniversityFee,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = universityPaymentMastermodel.PaidBranchKey != null ? universityPaymentMastermodel.PaidBranchKey : ApplicationDetails.BranchKey,
                Purpose = purpose + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.AffiliationsTieUps + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Payment + EduSuiteUIResources.BlankSpace + ApplicationDetails.StudentName + " ( " + ApplicationDetails.AdmissionNo + " ) ",
            });

            return accountFlowModelList;
        }
        private void AdvanceAmountList(decimal FeeAmount, List<AccountFlowViewModel> accountFlowModelList, UniversityPaymentMaster universityFeePaymentModel, bool IsUpdate, string purpose)
        {
            long accountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.AdvancePayable).Select(x => x.RowKey).FirstOrDefault();
            long ExtraUpdateKey = 0;
            var ApplicationDetails = dbContext.Applications.SingleOrDefault(x => x.RowKey == universityFeePaymentModel.ApplicationKey);
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.In,
                AccountHeadKey = accountHeadKey,
                Amount = FeeAmount,
                TransactionTypeKey = DbConstants.TransactionType.UniversityFee,
                TransactionKey = universityFeePaymentModel.RowKey,
                TransactionDate = Convert.ToDateTime(universityFeePaymentModel.UniversityPaymentDate),
                VoucherTypeKey = DbConstants.VoucherType.UniversityFee,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = universityFeePaymentModel.PaidBranchKey != null ? universityFeePaymentModel.PaidBranchKey : ApplicationDetails.BranchKey,
                Purpose = purpose + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.AffiliationsTieUps + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Payment + EduSuiteUIResources.BlankSpace + ApplicationDetails.StudentName + " ( " + ApplicationDetails.AdmissionNo + " ) ",
            });
        }
        private void PayableAmountList(decimal Amount, List<AccountFlowViewModel> accountFlowModelList, UniversityPaymentMaster universityFeePaymentModel, bool IsUpdate, string purpose)
        {

            long ExtraUpdateKey = 0;
            var ApplicationDetails = dbContext.Applications.SingleOrDefault(x => x.RowKey == universityFeePaymentModel.ApplicationKey);
            //long accountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.AccountsPayable).Select(x => x.RowKey).FirstOrDefault();
            long accountHeadKey = dbContext.UniversityMasters.Where(x => x.RowKey == ApplicationDetails.UniversityMasterKey).Select(x => x.AccountHeadKey ?? 0).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.In,
                AccountHeadKey = accountHeadKey,
                Amount = Amount,
                TransactionTypeKey = DbConstants.TransactionType.UniversityFee,
                TransactionDate = Convert.ToDateTime(universityFeePaymentModel.UniversityPaymentDate),
                TransactionKey = universityFeePaymentModel.RowKey,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                VoucherTypeKey = DbConstants.VoucherType.UniversityFee,
                BranchKey = universityFeePaymentModel.PaidBranchKey != null ? universityFeePaymentModel.PaidBranchKey : ApplicationDetails.BranchKey,
                Purpose = purpose + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.AffiliationsTieUps + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Payment + EduSuiteUIResources.BlankSpace + ApplicationDetails.StudentName + " ( " + ApplicationDetails.AdmissionNo + " ) ",
            });
        }

        private void IncomeAmountList(decimal Amount, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, Application ApplicationModel, long PromotionKey)
        {
            long accountHeadKey = dbContext.UniversityMasters.Where(x => x.RowKey == ApplicationModel.UniversityMasterKey).Select(x => x.AccountHeadKey ?? 0).FirstOrDefault();
            long ExtraUpdateKey = 0;
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                AccountHeadKey = accountHeadKey,
                Amount = Amount,
                TransactionTypeKey = DbConstants.TransactionType.UniversityFee,
                TransactionDate = DateTimeUTC.Now,
                TransactionKey = PromotionKey,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                VoucherTypeKey = DbConstants.VoucherType.UniversityFee,
                BranchKey = ApplicationModel.BranchKey,
                Purpose = EduSuiteUIResources.Promotion + EduSuiteUIResources.BlankSpace + ApplicationModel.StudentName + " ( " + ApplicationModel.AdmissionNo + " ) ",
            });

        }

        private void ExpenseSplitAmountList(List<UniversityPaymentDetailsmodel> modelList, List<AccountFlowViewModel> accountFlowModelList, UniversityPaymentMaster FeePaymentMasterModel, bool IsUpdate, string purpose, decimal TotalIncomeAmount)
        {
            var ApplicationDetails = dbContext.Applications.SingleOrDefault(x => x.RowKey == FeePaymentMasterModel.ApplicationKey);

            if (DbConstants.GeneralConfiguration.AllowSplitCostOfService)
            {

                long ExtraUpdateKey = 0;

                foreach (UniversityPaymentDetailsmodel item in modelList)
                {
                    //if ((item.UniversityPaymentYear == null || item.UniversityPaymentYear <= ApplicationDetails.CurrentYear))
                    //{
                    long AccountHeadKey = dbContext.FeeTypes.Where(x => x.RowKey == item.FeeTypeKey).Select(x => x.AccountHeadKey ?? 0).FirstOrDefault();
                    accountFlowModelList.Add(new AccountFlowViewModel
                    {
                        CashFlowTypeKey = DbConstants.CashFlowType.In,
                        AccountHeadKey = AccountHeadKey,
                        Amount = item.UniversityPaymentAmount ?? 0,
                        TransactionTypeKey = DbConstants.TransactionType.UniversityFee,
                        TransactionDate = Convert.ToDateTime(FeePaymentMasterModel.UniversityPaymentDate),
                        TransactionKey = FeePaymentMasterModel.RowKey,
                        ExtraUpdateKey = ExtraUpdateKey,
                        IsUpdate = IsUpdate,
                        VoucherTypeKey = DbConstants.VoucherType.UniversityFee,
                        BranchKey = FeePaymentMasterModel.PaidBranchKey != null ? FeePaymentMasterModel.PaidBranchKey : ApplicationDetails.BranchKey,
                        Purpose = purpose + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.AffiliationsTieUps + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Payment + EduSuiteUIResources.BlankSpace + ApplicationDetails.StudentName + " ( " + ApplicationDetails.AdmissionNo + " ) ",

                    });
                    //}
                }
            }
            else
            {
                long AccountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.CostOfService).Select(x => x.RowKey).FirstOrDefault();
                long ExtraUpdateKey = 0;
                accountFlowModelList.Add(new AccountFlowViewModel
                {
                    CashFlowTypeKey = DbConstants.CashFlowType.In,
                    AccountHeadKey = AccountHeadKey,
                    Amount = TotalIncomeAmount,
                    TransactionTypeKey = DbConstants.TransactionType.UniversityFee,
                    TransactionDate = Convert.ToDateTime(FeePaymentMasterModel.UniversityPaymentDate),
                    TransactionKey = FeePaymentMasterModel.RowKey,
                    ExtraUpdateKey = ExtraUpdateKey,
                    IsUpdate = IsUpdate,
                    VoucherTypeKey = DbConstants.VoucherType.UniversityFee,
                    BranchKey = FeePaymentMasterModel.PaidBranchKey != null ? FeePaymentMasterModel.PaidBranchKey : ApplicationDetails.BranchKey,
                    Purpose = purpose + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.AffiliationsTieUps + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Payment + EduSuiteUIResources.BlankSpace + ApplicationDetails.StudentName + " ( " + ApplicationDetails.AdmissionNo + " ) ",

                });
            }
        }

        private void ExpenseSplitAmountBulkList(List<AccountFlowViewModel> accountFlowModelList, UniversityPaymentMaster FeePaymentMasterModel, bool IsUpdate, string purpose, decimal TotalIncomeAmount, UniversityPaymentBulklist List)
        {
            var ApplicationDetails = dbContext.Applications.SingleOrDefault(x => x.RowKey == FeePaymentMasterModel.ApplicationKey);
            GeneralConfiguration generalConfiguration = dbContext.GeneralConfigurations.FirstOrDefault();
            if (generalConfiguration.AllowSplitCostOfService)
            {

                long ExtraUpdateKey = 0;

                //if ((List.UniversityPaymentYear == null || List.UniversityPaymentYear <= ApplicationDetails.CurrentYear))
                //{
                long AccountHeadKey = dbContext.FeeTypes.Where(x => x.RowKey == List.FeeTypeKey).Select(x => x.AccountHeadKey ?? 0).FirstOrDefault();
                accountFlowModelList.Add(new AccountFlowViewModel
                {
                    CashFlowTypeKey = DbConstants.CashFlowType.In,
                    AccountHeadKey = AccountHeadKey,
                    Amount = List.UniversityPaymentAmount ?? 0,
                    TransactionTypeKey = DbConstants.TransactionType.UniversityFee,
                    TransactionDate = Convert.ToDateTime(FeePaymentMasterModel.UniversityPaymentDate),
                    TransactionKey = FeePaymentMasterModel.RowKey,
                    ExtraUpdateKey = ExtraUpdateKey,
                    IsUpdate = IsUpdate,
                    VoucherTypeKey = DbConstants.VoucherType.UniversityFee,
                    BranchKey = FeePaymentMasterModel.PaidBranchKey != null ? FeePaymentMasterModel.PaidBranchKey : ApplicationDetails.BranchKey,
                    Purpose = purpose + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.AffiliationsTieUps + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Payment + EduSuiteUIResources.BlankSpace + ApplicationDetails.StudentName + " ( " + ApplicationDetails.AdmissionNo + " ) ",

                });
                //}

            }
            else
            {
                long AccountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.CostOfService).Select(x => x.RowKey).FirstOrDefault();
                long ExtraUpdateKey = 0;
                accountFlowModelList.Add(new AccountFlowViewModel
                {
                    CashFlowTypeKey = DbConstants.CashFlowType.Out,
                    AccountHeadKey = AccountHeadKey,
                    Amount = TotalIncomeAmount,
                    TransactionTypeKey = DbConstants.TransactionType.UniversityFee,
                    TransactionDate = Convert.ToDateTime(FeePaymentMasterModel.UniversityPaymentDate),
                    TransactionKey = FeePaymentMasterModel.RowKey,
                    ExtraUpdateKey = ExtraUpdateKey,
                    IsUpdate = IsUpdate,
                    VoucherTypeKey = DbConstants.VoucherType.UniversityFee,
                    BranchKey = FeePaymentMasterModel.PaidBranchKey != null ? FeePaymentMasterModel.PaidBranchKey : ApplicationDetails.BranchKey,
                    Purpose = purpose + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.AffiliationsTieUps + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Payment + EduSuiteUIResources.BlankSpace + ApplicationDetails.StudentName + " ( " + ApplicationDetails.AdmissionNo + " ) ",

                });
            }
        }

        #endregion
        public UniversityPaymentViewmodel DeleteUniversityFee(long Id)
        {
            UniversityPaymentViewmodel universityPaymentModel = new UniversityPaymentViewmodel();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    UniversityPaymentMaster universityFeePayment = dbContext.UniversityPaymentMasters.SingleOrDefault(row => row.RowKey == Id);
                    List<UniversityPaymentDetail> UniversityPaymentDetailList = dbContext.UniversityPaymentDetails.Where(row => row.UniversiyPaymenMasterKey == Id).ToList();

                    foreach (UniversityPaymentDetail item in UniversityPaymentDetailList)
                    {
                        var ifcancelation = dbContext.UniversityPaymentCancelations.Any(x => x.UniversityPaymentDetailsKey == item.RowKey);
                        if (ifcancelation)
                        {
                            transaction.Rollback();
                            universityPaymentModel.Message = EduSuiteUIResources.PaymentAlreadyCanceledMessage;
                            universityPaymentModel.IsSuccessful = false;
                            return universityPaymentModel;
                        }
                    }

                    decimal? OldAmount = dbContext.UniversityPaymentDetails.Where(row => row.UniversiyPaymenMasterKey == Id).Select(x => x.UniversityPaymentAmount).DefaultIfEmpty().Sum();


                    dbContext.UniversityPaymentDetails.RemoveRange(UniversityPaymentDetailList);


                    dbContext.UniversityPaymentMasters.Remove(universityFeePayment);



                    AccountFlowViewModel accountFlowModel = new AccountFlowViewModel();
                    accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.UniversityFee;
                    accountFlowModel.TransactionKey = Id;
                    accountFlowModel.IsDelete = false;
                    AccountFlowService accountFlowService = new AccountFlowService(dbContext);
                    accountFlowService.DeleteAccountFlow(accountFlowModel);

                    bool IsChequeExist = dbContext.ChequeClearances.Where(x => x.TransactionKey == Id && x.TransactionTypeKey == DbConstants.TransactionType.UniversityFee).Any();
                    if (IsChequeExist)
                    {
                        ChequeClearance dbChequeClearance = dbContext.ChequeClearances.Where(x => x.TransactionKey == Id && x.TransactionTypeKey == DbConstants.TransactionType.UniversityFee).SingleOrDefault();
                        dbContext.ChequeClearances.Remove(dbChequeClearance);
                        accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.ChequeClearance;
                        accountFlowModel.TransactionKey = dbChequeClearance.TransactionKey;
                        accountFlowService.DeleteAccountFlow(accountFlowModel);
                    }

                    dbContext.SaveChanges();
                    transaction.Commit();
                    universityPaymentModel.Message = EduSuiteUIResources.Success;
                    universityPaymentModel.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.UniversityPayment, ActionConstants.Delete, DbConstants.LogType.Info, Id, universityPaymentModel.Message);


                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        universityPaymentModel.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.AffiliationsTieUpsPayment);
                        universityPaymentModel.IsSuccessful = false;

                        ActivityLog.CreateActivityLog(MenuConstants.UniversityPayment, ActionConstants.Delete, DbConstants.LogType.Error, Id, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    universityPaymentModel.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.AffiliationsTieUpsPayment);
                    universityPaymentModel.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.UniversityPayment, ActionConstants.Delete, DbConstants.LogType.Error, Id, ex.GetBaseException().Message);
                }
            }
            return universityPaymentModel;
        }

        public UniversityPaymentViewmodel DeleteUniversityFeeOneByOne(long Id)
        {
            UniversityPaymentViewmodel universityPaymentModel = new UniversityPaymentViewmodel();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    UniversityPaymentDetail universityFeePaymentDetails = dbContext.UniversityPaymentDetails.SingleOrDefault(row => row.RowKey == Id);
                    var UniversiyPaymenMasterKey = universityFeePaymentDetails.UniversiyPaymenMasterKey;
                    UniversityPaymentMaster universityFeePaymentmaster = dbContext.UniversityPaymentMasters.SingleOrDefault(row => row.RowKey == UniversiyPaymenMasterKey);


                    Application Application = dbContext.Applications.FirstOrDefault(row => row.RowKey == universityFeePaymentmaster.ApplicationKey);
                    List<UniversityPaymentDetail> universityFeePaymentDetailList = dbContext.UniversityPaymentDetails.Where(row => row.UniversiyPaymenMasterKey == UniversiyPaymenMasterKey).ToList();
                    if (universityFeePaymentDetailList.Count <= 1)
                    {

                        List<UniversityPaymentDetail> UniversityPaymentDetailListAll = dbContext.UniversityPaymentDetails.Where(row => row.UniversiyPaymenMasterKey == UniversiyPaymenMasterKey).ToList();
                        dbContext.UniversityPaymentDetails.RemoveRange(UniversityPaymentDetailListAll);

                        dbContext.UniversityPaymentMasters.Remove(universityFeePaymentmaster);


                        AccountFlowViewModel accountFlowModel = new AccountFlowViewModel();
                        accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.UniversityFee;
                        accountFlowModel.TransactionKey = UniversiyPaymenMasterKey;
                        accountFlowModel.IsDelete = false;
                        AccountFlowService accountFlowService = new AccountFlowService(dbContext);
                        accountFlowService.DeleteAccountFlow(accountFlowModel);

                        bool IsChequeExist = dbContext.ChequeClearances.Where(x => x.TransactionKey == UniversiyPaymenMasterKey && x.TransactionTypeKey == DbConstants.TransactionType.UniversityFee).Any();
                        if (IsChequeExist)
                        {
                            ChequeClearance dbChequeClearance = dbContext.ChequeClearances.Where(x => x.TransactionKey == UniversiyPaymenMasterKey && x.TransactionTypeKey == DbConstants.TransactionType.UniversityFee).SingleOrDefault();
                            dbContext.ChequeClearances.Remove(dbChequeClearance);
                            accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.ChequeClearance;
                            accountFlowModel.TransactionKey = dbChequeClearance.TransactionKey;
                            accountFlowService.DeleteAccountFlow(accountFlowModel);
                        }
                    }
                    else
                    {
                        long oldBankKey = universityFeePaymentDetails.UniversityPaymentMaster.BankAccountKey ?? 0;
                        short oldPaymentModeKey = universityFeePaymentDetails.UniversityPaymentMaster.PaymentModeKey;
                        decimal totalDebitAmount = universityFeePaymentDetails.UniversityPaymentMaster.UniversityPaymentDetails.Where(row => row.RowKey != universityFeePaymentDetails.RowKey).Select(x => x.UniversityPaymentAmount).DefaultIfEmpty().Sum();
                        decimal AdvancePayment = 0;
                        if (Application.EducationTypeKey == DbConstants.EducationType.RegulerEducation)
                        {
                            AdvancePayment = universityFeePaymentDetails.UniversityPaymentMaster.UniversityPaymentDetails.Where(row => row.RowKey != universityFeePaymentDetails.RowKey && row.UniversityPaymentYear > row.UniversityPaymentMaster.Application.CurrentYear).Select(x => x.UniversityPaymentAmount).DefaultIfEmpty().Sum();
                        }
                        List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();

                        string FeeTypeName = dbContext.FeeTypes.Where(x => x.RowKey == universityFeePaymentDetails.FeeTypeKey).Select(y => y.FeeTypeName).FirstOrDefault();
                        string FeeYearText = universityFeePaymentDetails.UniversityPaymentYear != null ? (CommonUtilities.GetYearDescriptionByCodeDetails(Application.Course.CourseDuration ?? 0, universityFeePaymentDetails.UniversityPaymentYear ?? 0, Application.AcademicTermKey)) : "";

                        string Purpose = FeeYearText + EduSuiteUIResources.BlankSpace + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(FeeTypeName.ToLower());

                        PaymentModeAmountList(universityFeePaymentDetails.UniversityPaymentMaster, accountFlowModelList, true, oldBankKey, oldPaymentModeKey, totalDebitAmount, Purpose);
                        PayableAmountList(totalDebitAmount - AdvancePayment, accountFlowModelList, universityFeePaymentDetails.UniversityPaymentMaster, true, Purpose);
                        AdvanceAmountList(AdvancePayment, accountFlowModelList, universityFeePaymentDetails.UniversityPaymentMaster, true, Purpose);
                        CreateAccountFlow(accountFlowModelList, true);

                        //if (DbConstants.PaymentMode.BankPaymentModes.Contains(universityFeePaymentmaster.PaymentModeKey))
                        //{
                        //    BankAccountService bankAccountService = new BankAccountService(dbContext);
                        //    BankAccountViewModel bankAccountModel = new BankAccountViewModel();
                        //    bankAccountModel.RowKey = universityFeePaymentmaster.BankAccountKey ?? 0;
                        //    bankAccountModel.Amount = universityFeePaymentDetails.UniversityPaymentAmount;
                        //    bankAccountService.UpdateCurrentAccountBalance(bankAccountModel, true, true, universityFeePaymentDetails.UniversityPaymentAmount);
                        //}



                        dbContext.UniversityPaymentDetails.Remove(universityFeePaymentDetails);
                    }

                    dbContext.SaveChanges();
                    transaction.Commit();
                    universityPaymentModel.Message = EduSuiteUIResources.Success;
                    universityPaymentModel.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.UniversityPayment, ActionConstants.Delete, DbConstants.LogType.Info, Id, universityPaymentModel.Message);


                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        universityPaymentModel.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.AffiliationsTieUpsPayment);
                        universityPaymentModel.IsSuccessful = false;

                        ActivityLog.CreateActivityLog(MenuConstants.UniversityPayment, ActionConstants.Delete, DbConstants.LogType.Error, Id, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    universityPaymentModel.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.AffiliationsTieUpsPayment);
                    universityPaymentModel.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.UniversityPayment, ActionConstants.Delete, DbConstants.LogType.Error, Id, ex.GetBaseException().Message);
                }
            }
            return universityPaymentModel;
        }

        private void FillUniversityFeePaymentDetails(UniversityPaymentViewmodel model)
        {
            long Applicationkey = dbContext.UniversityPaymentMasters.Where(row => row.RowKey == model.RowKey).Select(x => x.ApplicationKey).SingleOrDefault();
            Application Application = dbContext.Applications.SingleOrDefault(row => row.RowKey == Applicationkey);

            model.UniversityFeePaymentDetails = dbContext.UniversityPaymentDetails.Where(row => row.UniversiyPaymenMasterKey == model.RowKey)
                .Select(row => new UniversityPaymentDetailsmodel
                {
                    RowKey = row.RowKey,
                    FeeTypeKey = row.FeeTypeKey,
                    UniversityPaymentAmount = row.UniversityPaymentAmount,
                    UniversityPaymentYear = row.UniversityPaymentYear,
                    FeeTypeName = row.FeeType.FeeTypeName,
                    CashFlowTypeKey = row.FeeType.CashFlowTypeKey,
                    IsFeeTypeYear = row.FeeType.FeeTypeModeKey == DbConstants.FeeTypeMode.Single ? true : false
                }).ToList();
            foreach (UniversityPaymentDetailsmodel PaymentDetails in model.UniversityFeePaymentDetails)
            {
                PaymentDetails.FeeYearText = PaymentDetails.UniversityPaymentYear != null ? (CommonUtilities.GetYearDescriptionByCodeDetails(Application.Course.CourseDuration ?? 0, PaymentDetails.UniversityPaymentYear ?? 0, Application.AcademicTermKey)) : "";

            }

        }

        private void FillDropdownLists(UniversityPaymentViewmodel model)
        {
            FillPaymentModes(model);
            FillPaymentModeSub(model);
            FillFeeTypes(model);
            FillFeeYears(model);
            FillBankAccounts(model);
            FillPaidBranches(model);
        }

        private void FillPaymentModes(UniversityPaymentViewmodel model)
        {
            model.PaymentModes = dbContext.VwPaymentModeSelectActiveOnlies.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.PaymentModeName
            }).ToList();
        }

        public void FillPaymentModeSub(UniversityPaymentViewmodel model)
        {
            model.PaymentModeSub = dbContext.PaymentModeSubs.Where(x => x.IsActive && x.PaymentModeKey == DbConstants.PaymentMode.Bank).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.PaymentModeSubName
            }).ToList();

        }

        private void FillBankAccounts(UniversityPaymentViewmodel model)
        {
            var application = dbContext.Applications.SingleOrDefault(x => x.RowKey == model.ApplicationKey);
            model.BankAccounts = dbContext.BranchAccounts.Where(x => x.BranchKey == application.BranchKey && x.BankAccount.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.BankAccount.RowKey,
                Text = (row.BankAccount.NameInAccount ?? row.BankAccount.AccountNumber) + EduSuiteUIResources.Hyphen + row.BankAccount.Bank.BankName
            }).ToList();
        }

        private void FillFeeTypes(UniversityPaymentViewmodel model)
        {
            model.FeeTypes = dbContext.FeeTypes.Where(row => row.IsActive && row.IsUniverisity == true && row.AccountHead.AccountHeadType.AccountGroupKey == DbConstants.AccountGroup.Expenses).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.FeeTypeName
            }).ToList();
        }

        private void FillFeeYears(UniversityPaymentViewmodel model)
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

        private void GenerateReceiptNo(UniversityPaymentViewmodel model)
        {
            ConfigurationViewModel ConfigModel = new ConfigurationViewModel();
            ConfigModel.BranchKey = model.PaidBranchKey != null ? model.PaidBranchKey ?? 0 : model.BranchKey;
            ConfigModel.ConfigType = DbConstants.PaymentReceiptConfigType.Payment;
            Configurations.GenerateReceipt(dbContext, ConfigModel);

            model.VoucherNo = ConfigModel.ReceiptNumber;
            model.SerialNumber = ConfigModel.SerialNumber;
        }

        public byte FillCashFlowTypeKey(short id)
        {
            byte CashFlowTypeKey = dbContext.FeeTypes.Where(x => x.RowKey == id).Select(x => x.CashFlowTypeKey).FirstOrDefault();
            return CashFlowTypeKey;
        }
        public UniversityFeePrintViewModel ViewFeePrint(long Id)
        {
            try
            {
                UniversityFeePrintViewModel model = new UniversityFeePrintViewModel();
                model = dbContext.UniversityPaymentMasters.Where(row => row.RowKey == Id).Select(row => new UniversityFeePrintViewModel
                {
                    CompanyName = row.Application.Branch.BranchName,
                    CompanyAddress = row.Application.Branch.AddressLine1,
                    //AddressLine2 = row.Application.M_Branch.AddressLine2,
                    //AddressLine3 = row.Application.M_Branch.AddressLine3,
                    //CityName = row.Application.M_Branch.CityName,
                    //PostalCode = row.Application.M_Branch.PostalCode,
                    AdmissionNo = row.Application.AdmissionNo,
                    StudentName = row.Application.StudentName,
                    ProgramName = row.Application.Course.CourseName,
                    UniversityName = row.Application.UniversityMaster.UniversityMasterName
                }).SingleOrDefault();
                model.UniversityPaymentViewmodel = (from es in dbContext.UniversityPaymentMasters.Where(row => row.RowKey == Id)
                                                    select new UniversityPaymentViewmodel
                                                    {
                                                        RowKey = es.RowKey,
                                                        FeeDate = es.UniversityPaymentDate,
                                                        RecieptNo = es.VoucherNo,
                                                        PaymentModeKey = es.PaymentModeKey,
                                                        PaymentModeName = es.PaymentMode.PaymentModeName,
                                                        PaymentModeSubKey = es.PaymentModeSubKey,
                                                        PaymentModeSubName = es.PaymentModeSub.PaymentModeSubName,
                                                        BankAccountName = es.BankAccount.Bank.BankName + "-" + es.BankAccount.AccountNumber,
                                                        CardNumber = es.CardNumber,
                                                        ReferenceNumber = es.ReferenceNumber,
                                                        ChequeOrDDNumber = es.UniversityPaymentChalanDDNumber,
                                                        ChequeClearanceDate = es.ChequeClearanceDate,
                                                        FeeDescription = es.UniversityPaymentNote
                                                    }).SingleOrDefault();
                if (model.UniversityPaymentViewmodel == null)
                {
                    model.UniversityPaymentViewmodel = new UniversityPaymentViewmodel();
                }
                FillPaymentModes(model.UniversityPaymentViewmodel);
                FillUniversityFeePaymentDetails(model.UniversityPaymentViewmodel);

                return model;
            }
            catch (Exception ex)
            {
                return new UniversityFeePrintViewModel();
            }

        }

        public List<ApplicationViewModel> GetApplications(ApplicationViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;

                IQueryable<ApplicationViewModel> applicationList = (from a in dbContext.Applications
                                                                        //orderby e.RowKey
                                                                    where (a.StudentName.Contains(model.ApplicantName)) && (a.StudentMobile.Contains(model.MobileNumber))
                                                                    select new ApplicationViewModel
                                                                    {
                                                                        RowKey = a.RowKey,
                                                                        AdmissionNo = a.AdmissionNo,
                                                                        ApplicantName = a.StudentName,
                                                                        CourseName = a.Course.CourseName,
                                                                        UniversityName = a.UniversityMaster.UniversityMasterName,
                                                                        MobileNumber = a.StudentMobile,
                                                                        ApplicationStatusName = a.StudentStatu.StudentStatusName,
                                                                        BatchName = a.Batch.BatchName,
                                                                        BranchKey = a.BranchKey,
                                                                        BatchKey = a.BatchKey,
                                                                        CourseKey = a.CourseKey,
                                                                        UniversityKey = a.UniversityMasterKey,
                                                                        BranchName = a.Branch.BranchName,
                                                                        AcademicTermKey = a.AcademicTermKey,
                                                                        CurrentYear = a.CurrentYear,
                                                                        CourseDuration = a.Course.CourseDuration ?? 0,
                                                                        TotalPaid = a.UniversityPaymentMasters.Select(row => row.UniversityPaymentDetails.Where(p => p.FeeType.CashFlowTypeKey == DbConstants.CashFlowType.Out).Select(p => p.UniversityPaymentAmount).DefaultIfEmpty().Sum()).Sum()

                                                                    });
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
                if (Employee != null)
                {
                    if (Employee.BranchAccess != null)
                    {
                        var Branches = Employee.BranchAccess.Split(',').Select(Int16.Parse).ToList();
                        applicationList = applicationList.Where(row => Branches.Contains(row.BranchKey ?? 0));
                    }
                }
                if (model.BranchKey != 0)
                {
                    applicationList = applicationList.Where(row => row.BranchKey == model.BranchKey);
                }
                if (model.BatchKey != 0)
                {
                    applicationList = applicationList.Where(row => row.BatchKey == model.BatchKey);
                }
                if (model.CourseKey != 0)
                {
                    applicationList = applicationList.Where(row => row.CourseKey == model.CourseKey);
                }
                if (model.UniversityKey != 0)
                {
                    applicationList = applicationList.Where(row => row.UniversityKey == model.UniversityKey);
                }
                applicationList = applicationList.GroupBy(x => x.RowKey).Select(y => y.FirstOrDefault());
                if (model.SortBy != "")
                {
                    applicationList = SortApplications(applicationList, model.SortBy, model.SortOrder);
                }
                TotalRecords = applicationList.Count();
                return applicationList.Skip(Skip).Take(Take).ToList<ApplicationViewModel>();
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.UniversityPayment, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<ApplicationViewModel>();


            }
        }

        private IQueryable<ApplicationViewModel> SortApplications(IQueryable<ApplicationViewModel> Query, string SortName, string SortOrder)
        {

            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(ApplicationViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<ApplicationViewModel>(resultExpression);

        }

        public UniversityPaymentDetailsmodel CheckFeeTypeMode(short id, int? Year, long ApplicationKey)
        {
            var IsFeeTypeYear = dbContext.FeeTypes.Any(x => x.RowKey == id && x.FeeTypeModeKey == DbConstants.FeeTypeMode.Single);

            UniversityPaymentDetailsmodel model = new UniversityPaymentDetailsmodel();

            Application application = dbContext.Applications.SingleOrDefault(row => row.RowKey == ApplicationKey);

            UniversityCourse universityCourse = dbContext.UniversityCourses.SingleOrDefault(row => row.UniversityMasterKey == application.UniversityMasterKey && row.CourseKey == application.CourseKey && row.AcademicTermKey == application.AcademicTermKey);

            UniversityCourseFee universityCourseFee = dbContext.UniversityCourseFees.SingleOrDefault(row => row.UniversityCourseKey == universityCourse.RowKey && row.FeeTypeKey == id && row.FeeYear == (IsFeeTypeYear == true ? (int?)null : Year));



            if (universityCourseFee != null)
            {
                if (universityCourseFee.CenterShareAmountPer != 0 && IsFeeTypeYear == true || (IsFeeTypeYear == false && (Year != null && Year != 0)))
                {
                    model = dbContext.AdmissionFees.Where(x => x.ApplicationKey == ApplicationKey && x.FeeTypeKey == id && x.AdmissionFeeYear == (IsFeeTypeYear == true ? (int?)null : Year)) //(Year ?? x.AdmissionFeeYear))
                                  .Select(row => new UniversityPaymentDetailsmodel
                                  {
                                      //RowKey = row.RowKey,
                                      //FeeTypeKey = row.FeeTypeKey ?? 0,
                                      //UniversityPaymentAmount = null,
                                      UniversityPaymentAmount = (row.ActualAmount * universityCourseFee.CenterShareAmountPer / 100) - row.Application.UniversityPaymentMasters
                                    .Select(y => y.UniversityPaymentDetails.Where(x => x.FeeTypeKey == row.FeeTypeKey && x.UniversityPaymentYear == (row.AdmissionFeeYear ?? x.UniversityPaymentYear))
                                        .Select(x => x.UniversityPaymentAmount).DefaultIfEmpty().Sum()).DefaultIfEmpty().Sum(),
                                      //FeeYear = row.AdmissionFeeYear,
                                      //FeeTypeName = row.FeeType.FeeTypeName,
                                      //CashFlowTypeKey = row.FeeType.CashFlowTypeKey,
                                      IsFeeTypeYear = row.FeeType.FeeTypeModeKey == DbConstants.FeeTypeMode.Single ? true : false,
                                  }).FirstOrDefault();

                }
                else
                {
                    model = new UniversityPaymentDetailsmodel { IsFeeTypeYear = IsFeeTypeYear, UniversityPaymentAmount = null };

                }
            }
            else
            {
                model = new UniversityPaymentDetailsmodel { IsFeeTypeYear = IsFeeTypeYear, UniversityPaymentAmount = null };

            }
            return model;
        }

        //Bulk University Payment

        public List<UniversityPaymentBulklist> GetUniversityPaymentStudentsList(UniversityPaymentBulkViewmodel model)
        {

            var Take = model.PageSize;
            var skip = (model.PageIndex - 1) * model.PageSize;

            var UniversitypaymentBulkDetails = dbContext.spUniversityPaymentBulkSelect(model.FeeTypeKey, model.SearchCourseKey, model.SearchUniversityKey, model.SearchBatchKey, model.SearchYearKey, model.SearchAcademicTermKey, model.SearchBranchKey)
                                          .Select(row => new UniversityPaymentBulklist
                                          {
                                              UniversityPaymentDetailsKey = row.UniversityDetailsKey,
                                              AdmissionNo = row.AdmissionNo.ToUpper(),
                                              ApplicationKey = row.ApplicationKey,
                                              BranchKey = row.BranchKey,
                                              StudentName = row.StudentName,
                                              BankAccountKey = row.BankAccountKey,
                                              UniversityPaymentAmount = row.UniversityPaymentAmount,
                                              UniversityPaymentChalanDDNumber = row.UniversityPaymentChalanDDNumber,
                                              UniversityPaymentDate = DateTimeUTC.Now,
                                              UniversityPaymentNote = row.UniversityPaymentNote,
                                              //UniversityPaymentYear = row.UniversityPaymentYear,
                                              UniversiyPaymentMasterKey = row.UniversityMasterKey,
                                              IsActive = row.IsActive ?? false,
                                              PaymentModeKey = DbConstants.PaymentMode.Cash,
                                              BankAccounts = dbContext.BankAccounts.Where(x => x.IsActive == true).Select(x => new SelectListModel
                                              {
                                                  RowKey = x.RowKey,
                                                  Text = x.Bank.BankName + "-" + x.AccountNumber

                                              }).ToList(),
                                              PaymentModes = dbContext.VwPaymentModeSelectActiveOnlies.Select(x => new SelectListModel
                                              {
                                                  RowKey = x.RowKey,
                                                  Text = x.PaymentModeName
                                              }).ToList(),
                                              PaymentModeSub = dbContext.PaymentModeSubs.Where(x => x.IsActive && x.PaymentModeKey == DbConstants.PaymentMode.Bank).Select(x => new SelectListModel
                                              {
                                                  RowKey = x.RowKey,
                                                  Text = x.PaymentModeSubName
                                              }).ToList()

                                          }).ToList();


            model.TotalRecords = UniversitypaymentBulkDetails.Count();

            UniversitypaymentBulkDetails = UniversitypaymentBulkDetails.OrderByDescending(Row => Row.AdmissionNo).Skip(skip).Take(Take).ToList();
            return UniversitypaymentBulkDetails;
        }


        //private void GenerateReceiptNo(UniversityPaymentBulkViewmodel model)
        //{
        //    long? MaxKey = (dbContext.UniversityPaymentMasters.Select(p => p.SerialNumber).DefaultIfEmpty().Max());
        //    model.SerialNumber = MaxKey + 1;
        //    model.VoucherNo = model.SerialNumber.ToString();

        //}

        public UniversityPaymentBulkViewmodel CreateUniversityPaymentBulk(UniversityPaymentBulkViewmodel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    List<UniversityPaymentBulklist> UniversitypaymentBulkDetailsList = model.UniversityPaymentBulklist.Where(x => x.UniversiyPaymentMasterKey == null && x.IsActive == true).ToList();
                    long Maxkey = dbContext.UniversityPaymentMasters.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    foreach (UniversityPaymentBulklist List in UniversitypaymentBulkDetailsList)
                    { 
                        var ApplicationDetails = dbContext.Applications.SingleOrDefault(x => x.RowKey == List.ApplicationKey);

                        ConfigurationViewModel ConfigModel = new ConfigurationViewModel();
                        ConfigModel.BranchKey = ApplicationDetails.BranchKey;
                        ConfigModel.ConfigType = DbConstants.PaymentReceiptConfigType.Payment;
                        Configurations.GenerateReceipt(dbContext, ConfigModel);
                        UniversityPaymentMaster universityFeePaymentModel = new UniversityPaymentMaster();
                        universityFeePaymentModel.RowKey = Convert.ToInt64(Maxkey + 1);
                        universityFeePaymentModel.VoucherNo = ConfigModel.ReceiptNumber;
                        universityFeePaymentModel.SerialNumber = ConfigModel.SerialNumber;

                        universityFeePaymentModel.PaymentModeKey = List.PaymentModeKey;
                        universityFeePaymentModel.PaymentModeSubKey = List.PaymentModeSubKey;
                        universityFeePaymentModel.UniversityPaymentDate = Convert.ToDateTime(List.UniversityPaymentDate);
                        universityFeePaymentModel.BankAccountKey = List.BankAccountKey;
                        universityFeePaymentModel.UniversityPaymentChalanDDNumber = List.UniversityPaymentChalanDDNumber;
                        universityFeePaymentModel.CardNumber = List.CardNumber;
                        universityFeePaymentModel.ReferenceNumber = List.ReferenceNumber;
                        universityFeePaymentModel.ChequeOrDDNumber = List.ChequeOrDDNumber;
                        universityFeePaymentModel.ChequeClearanceDate = List.ChequeClearanceDate;
                        universityFeePaymentModel.UniversityPaymentNote = List.UniversityPaymentNote;
                        universityFeePaymentModel.CenterShareAmount = model.CenterShareAmount;
                        universityFeePaymentModel.ApplicationKey = List.ApplicationKey;
                        universityFeePaymentModel.IsActive = true;
                        dbContext.UniversityPaymentMasters.Add(universityFeePaymentModel);
                        if (model.IsFeeTypeYear == true)
                        {
                            model.SearchYearKey = null;
                        }
                        else
                        {
                            List.UniversityPaymentYear = model.SearchYearKey;
                        }

                        List.FeeTypeKey = model.FeeTypeKey;
                        decimal totalDebitAmount = model.UniversityPaymentBulklist.Where(row => row.ApplicationKey == List.ApplicationKey).Select(x => x.UniversityPaymentAmount ?? 0).DefaultIfEmpty().Sum();
                        decimal AdvancePayment = model.UniversityPaymentBulklist.Where(x => x.ApplicationKey == List.ApplicationKey && x.UniversityPaymentYear > ApplicationDetails.CurrentYear).Select(x => x.UniversityPaymentAmount ?? 0).DefaultIfEmpty().Sum();

                        long oldBankKey = 0;
                        short oldPaymentModeKey = 0;

                        CreateUniversityFeeDetailsBulk(universityFeePaymentModel.RowKey, List);

                        string purpose = String.Join(",", List.PurposeList);

                        List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
                        GeneralConfiguration generalConfiguration = dbContext.GeneralConfigurations.FirstOrDefault();
                        if (generalConfiguration.UniversityPaymentAccountDate == null || universityFeePaymentModel.UniversityPaymentDate >= generalConfiguration.UniversityPaymentAccountDate)
                        {
                            if (totalDebitAmount != 0)
                            {
                                if (!DbConstants.GeneralConfiguration.AllowUniversityAccountHead)
                                {
                                    decimal TotalIncomeAmount = totalDebitAmount - AdvancePayment;
                                    ExpenseSplitAmountBulkList(accountFlowModelList, universityFeePaymentModel, false, purpose, TotalIncomeAmount, List);
                                }
                                else
                                {
                                    PayableAmountList(totalDebitAmount - AdvancePayment, accountFlowModelList, universityFeePaymentModel, false, purpose);
                                }

                                PaymentModeAmountList(universityFeePaymentModel, accountFlowModelList, false, oldBankKey, oldPaymentModeKey, totalDebitAmount, purpose);
                                AdvanceAmountList(AdvancePayment, accountFlowModelList, universityFeePaymentModel, false, purpose);
                                CreateAccountFlow(accountFlowModelList, false);
                            }
                        }

                        dbContext.SaveChanges();
                        Maxkey++;
                    }

                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.UniversityPayment, ActionConstants.BulkAdd, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.AffiliationsTieUpsPayment);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.UniversityPayment, ActionConstants.BulkAdd, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            FillDrodownLists(model);
            return model;
        }

        private void CreateUniversityFeeDetailsBulk(long UniversityPaymentMasterKey, UniversityPaymentBulklist model)
        {
            Int64 MaxKey = dbContext.UniversityPaymentDetails.Select(p => p.RowKey).DefaultIfEmpty().Max();

            Application Application = dbContext.Applications.FirstOrDefault(row => row.RowKey == model.ApplicationKey);

            UniversityPaymentDetail universityPaymentDetailModel = new UniversityPaymentDetail();
            universityPaymentDetailModel.RowKey = Convert.ToInt64(MaxKey + 1);
            universityPaymentDetailModel.FeeTypeKey = model.FeeTypeKey;
            universityPaymentDetailModel.UniversityPaymentAmount = Convert.ToDecimal(model.UniversityPaymentAmount);
            universityPaymentDetailModel.UniversityPaymentYear = model.UniversityPaymentYear;
            universityPaymentDetailModel.UniversiyPaymenMasterKey = UniversityPaymentMasterKey;
            dbContext.UniversityPaymentDetails.Add(universityPaymentDetailModel);
            MaxKey++;


            string FeeTypeName = dbContext.FeeTypes.Where(x => x.RowKey == model.FeeTypeKey).Select(y => y.FeeTypeName).FirstOrDefault();
            string FeeYearText = model.UniversityPaymentYear != null ? (CommonUtilities.GetYearDescriptionByCodeDetails(Application.Course.CourseDuration ?? 0, model.UniversityPaymentYear ?? 0, Application.AcademicTermKey)) : "";

            FeeTypeName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(FeeTypeName.ToLower());
            string Purpose = FeeYearText + EduSuiteUIResources.BlankSpace + FeeTypeName;
            model.PurposeList.Add(Purpose);

        }

        public UniversityPaymentBulkViewmodel GetCourseTypeBySyllabus(UniversityPaymentBulkViewmodel model)
        {
            short SearchAcademicTermKey = model.SearchAcademicTermKey ?? 0;
            model.CourseTypes = dbContext.UniversityCourses.Where(x => x.AcademicTermKey == SearchAcademicTermKey).Select(row => new SelectListModel
            {
                RowKey = row.Course.CourseType.RowKey,
                Text = row.Course.CourseType.CourseTypeName

            }).GroupBy(row => row.RowKey).Select(row => row.FirstOrDefault()).ToList();


            return model;
        }

        public UniversityPaymentBulkViewmodel GetCourseByCourseType(UniversityPaymentBulkViewmodel model)
        {
            short SearchAcademicTermKey = model.SearchAcademicTermKey ?? 0;
            model.Courses = dbContext.UniversityCourses.Where(row => row.Course.CourseTypeKey == model.SearchCourseTypeKey && row.AcademicTermKey == SearchAcademicTermKey).Select(row => new SelectListModel
            {
                RowKey = row.Course.RowKey,
                Text = row.Course.CourseName
            }).GroupBy(row => row.RowKey).Select(row => row.FirstOrDefault()).ToList();


            return model;
        }

        public UniversityPaymentBulkViewmodel GetUniversityByCourse(UniversityPaymentBulkViewmodel model)
        {
            model.Universities = dbContext.UniversityCourses.Where(row => row.CourseKey == model.SearchCourseKey).Select(row => new SelectListModel
            {
                RowKey = row.UniversityMaster.RowKey,
                Text = row.UniversityMaster.UniversityMasterName
            }).GroupBy(row => row.RowKey).Select(row => row.FirstOrDefault()).ToList();

            return model;
        }

        public UniversityPaymentBulkViewmodel GetBatches(UniversityPaymentBulkViewmodel model)
        {
            model.Batches = dbContext.Batches.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BatchName
            }).ToList();

            return model;
        }

        public UniversityPaymentBulkViewmodel GetYears(UniversityPaymentBulkViewmodel model)
        {

            //model.Years = dbContext.fnStudentYear(model.SearchAcademicTermKey).Select(x => new SelectListModel
            //{
            //    RowKey = x.RowKey ?? 0,
            //    Text = x.YearName
            //}).ToList();

            //var CourseDuration = dbContext.Courses.Where(row => row.RowKey == model.SearchCourseKey).Select(row => row.CourseDuration).FirstOrDefault();
            //if (CourseDuration != 0)
            //{
            //    CourseDuration = Convert.ToBoolean(model.SearchAcademicTermKey) ? CourseDuration / 6 : CourseDuration / 12;

            //    if (CourseDuration < 1)
            //    {
            //        model.Years = model.Years.Where(row => row.RowKey == 0).ToList();

            //    }
            //    else
            //    {
            //        model.Years = model.Years.Where(row => row.RowKey <= CourseDuration && row.RowKey > 0).ToList();
            //    }
            //}
            var CourseDuration = dbContext.Courses.Where(row => row.RowKey == model.SearchCourseKey).Select(row => row.CourseDuration).FirstOrDefault();
            //bool IsSemester = Application.M_UniversityMaster.M_UniversityCourse.Select(row => row.IsSemester).FirstOrDefault();
            //byte? IsSemester = Application.SyllabusKey;

            // var duration = Math.Ceiling((Convert.ToDecimal(CourseDuration ?? 0) / 12));
            if (CourseDuration != 0 && CourseDuration != null)
            {
                var duration = Math.Ceiling((Convert.ToDecimal(model.SearchAcademicTermKey == DbConstants.AcademicTerm.Semester ? CourseDuration / 6 : CourseDuration / 12)));

                var StartYear = 1;
                if (duration < 1)
                {
                    model.Years.Add(new SelectListModel
                    {
                        RowKey = 1,
                        Text = " Short Term"
                    });
                }
                else
                {
                    for (int i = StartYear; i <= duration; i++)
                    {
                        model.Years.Add(new SelectListModel
                        {
                            RowKey = i,
                            Text = i + (model.SearchAcademicTermKey == DbConstants.AcademicTerm.Semester ? " Semester" : " Year")
                        });
                    }
                }

            }
            return model;
        }
        public UniversityPaymentBulkViewmodel GetSyllabus(UniversityPaymentBulkViewmodel model)
        {
            model.AcademicTerms = dbContext.AcademicTerms.Where(x => x.IsActive == true).Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.AcademicTermName,

            }).ToList();

            return model;
        }

        private void FillFeeTypesBulk(UniversityPaymentBulkViewmodel model)
        {
            model.FeeTypes = dbContext.FeeTypes.Where(row => row.IsActive && row.IsUniverisity == true && row.AccountHead.AccountHeadType.AccountGroupKey == DbConstants.AccountGroup.Expenses).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.FeeTypeName
            }).ToList();
        }
        private void FillBank(UniversityPaymentBulkViewmodel model)
        {
            model.BankAccounts = dbContext.BankAccounts.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.Bank.BankName + "-" + row.AccountNumber
            }).ToList();
        }
        public void FillDrodownLists(UniversityPaymentBulkViewmodel model)
        {
            GetSyllabus(model);
            GetCourseTypeBySyllabus(model);
            GetCourseByCourseType(model);
            GetUniversityByCourse(model);
            GetBatches(model);
            FillBank(model);
            FillFeeTypesBulk(model);
            GetYears(model);
            FillPaymentModesBulk(model);
            FillPaymentModeSubBulk(model);

        }
        private void FillPaymentModesBulk(UniversityPaymentBulkViewmodel model)
        {
            model.PaymentModes = dbContext.VwPaymentModeSelectActiveOnlies.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.PaymentModeName
            }).ToList();
        }
        public void FillPaymentModeSubBulk(UniversityPaymentBulkViewmodel model)
        {
            model.PaymentModeSub = dbContext.PaymentModeSubs.Where(x => x.IsActive && x.PaymentModeKey == DbConstants.PaymentMode.Bank).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.PaymentModeSubName
            }).ToList();

        }
        public UniversityPaymentBulkViewmodel CheckFeeTypeModeBulk(short id)
        {
            UniversityPaymentBulkViewmodel model = new UniversityPaymentBulkViewmodel();


            model.IsFeeTypeYear = dbContext.FeeTypes.Any(x => x.RowKey == id && x.FeeTypeModeKey == DbConstants.FeeTypeMode.Single);

            return model;
        }
        public List<UniversityPaymentDetailsmodel> BindTotalFeeDetails(UniversityPaymentViewmodel model)
        {
            try
            {
                List<UniversityPaymentDetailsmodel> TotalFeeDetails = new List<UniversityPaymentDetailsmodel>();
                Application Application = dbContext.Applications.SingleOrDefault(row => row.RowKey == model.ApplicationKey);

                if (Application != null)
                {
                    var CourseDuration = Application.Course.CourseDuration;
                    var duration = Math.Ceiling((Convert.ToDecimal(Application.AcademicTermKey == DbConstants.AcademicTerm.Semester ? CourseDuration / 6 : CourseDuration / 12)));

                    short AcademicTermKey = dbContext.Applications.Where(row => row.RowKey == model.ApplicationKey).Select(row => row.AcademicTermKey).FirstOrDefault();

                    TotalFeeDetails = dbContext.CalculateUniversityFeeDetails(model.ApplicationKey)
                                         .Select(row => new UniversityPaymentDetailsmodel
                                         {
                                             UniversityPaymentYear = row.AdmissionFeeYear,
                                             FeeTypeKey = row.FeeTypeKey ?? 0,
                                             FeeTypeName = row.FeeTypeName,
                                             FeeYearText = row.AdmissionFeeYear != null ? (duration < 1 ? "Short Term" : CommonUtilities.GetYearDescriptionByCode(row.AdmissionFeeYear ?? 0, AcademicTermKey)) : "",
                                             TotalAmount = row.AdmissionFeeAmount,
                                             FeePaid = row.FeePaid,
                                             BalanceAmount = row.BalanceFee

                                         }).ToList();


                }
                return TotalFeeDetails;
            }
            catch (Exception ex)
            {
                return new List<UniversityPaymentDetailsmodel>();
            }
        }

        private void FillNotificationDetail(UniversityPaymentViewmodel model)
        {
            NotificationTemplate notificationTemplateModel = dbContext.NotificationTemplates.SingleOrDefault(row => row.RowKey == DbConstants.NotificationTemplate.UniversityFee);
            if (notificationTemplateModel != null)
            {
                model.AutoEmail = notificationTemplateModel.AutoEmail;
                model.AutoSMS = notificationTemplateModel.AutoSMS;
                model.TemplateKey = notificationTemplateModel.RowKey;
            }
        }

        public decimal CheckShortBalance(short PaymentModeKey, long Rowkey, long BankAccountKey, short branchKey)
        {
            decimal Balance = 0;
            if (PaymentModeKey == DbConstants.PaymentMode.Cash)
            {
                long accountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.CashAccount).Select(x => x.RowKey).FirstOrDefault();
                decimal totalDebit = dbContext.AccountFlows.Where(x => x.AccountHeadKey == accountHeadKey && x.CashFlowTypeKey == DbConstants.CashFlowType.In && x.BranchKey == branchKey).Select(x => x.Amount).DefaultIfEmpty().Sum();
                decimal totalCredit = dbContext.AccountFlows.Where(x => x.AccountHeadKey == accountHeadKey && x.CashFlowTypeKey == DbConstants.CashFlowType.Out && x.BranchKey == branchKey).Select(x => x.Amount).DefaultIfEmpty().Sum();
                Balance = totalDebit - totalCredit;
                if (Rowkey != 0)
                {
                    var purchaseList = dbContext.UniversityPaymentMasters.SingleOrDefault(x => x.RowKey == Rowkey);
                    decimal UniversityPaymentAmount = dbContext.UniversityPaymentDetails.Where(x => x.UniversiyPaymenMasterKey == Rowkey).Select(y => y.UniversityPaymentAmount).DefaultIfEmpty().Max();
                    if (PaymentModeKey == purchaseList.PaymentModeKey)
                    {
                        Balance = Balance + UniversityPaymentAmount;
                    }
                }
            }
            else if (PaymentModeKey == DbConstants.PaymentMode.Bank || PaymentModeKey == DbConstants.PaymentMode.Cheque)
            {
                if (BankAccountKey != 0 && BankAccountKey != null)
                {
                    long accountHeadKey = dbContext.BankAccounts.Where(x => x.RowKey == BankAccountKey).Select(x => x.AccountHeadKey).FirstOrDefault();
                    decimal totalDebit = dbContext.AccountFlows.Where(x => x.AccountHeadKey == accountHeadKey && x.CashFlowTypeKey == DbConstants.CashFlowType.In).Select(x => x.Amount).DefaultIfEmpty().Sum();
                    decimal totalCredit = dbContext.AccountFlows.Where(x => x.AccountHeadKey == accountHeadKey && x.CashFlowTypeKey == DbConstants.CashFlowType.Out).Select(x => x.Amount).DefaultIfEmpty().Sum();
                    Balance = totalDebit - totalCredit;
                    if (Rowkey != 0)
                    {
                        var purchaseList = dbContext.UniversityPaymentMasters.SingleOrDefault(x => x.RowKey == Rowkey);
                        decimal UniversityPaymentAmount = dbContext.UniversityPaymentDetails.Where(x => x.UniversiyPaymenMasterKey == Rowkey).Select(y => y.UniversityPaymentAmount).DefaultIfEmpty().Max();
                        if (BankAccountKey == purchaseList.BankAccountKey)
                        {
                            //Balance = Balance + purchaseList.Amount;
                            Balance = Balance + UniversityPaymentAmount;
                        }
                    }
                }
            }
            return Balance;
        }

        private void FillPaidBranches(UniversityPaymentViewmodel model)
        {
            model.PaidBranches = dbContext.Branches.Where(row => row.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BranchName
            }).ToList();
        }
    }
}
