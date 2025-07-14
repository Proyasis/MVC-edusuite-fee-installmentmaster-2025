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
using System.Data.Common;
using CITS.EduSuite.Business.Extensions;

namespace CITS.EduSuite.Business.Services
{
    public class ApplicationFeePaymentService : IApplicationFeePaymentService
    {
        private EduSuiteDatabase dbContext;
        public ApplicationFeePaymentService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<ApplicationFeePaymentViewModel> GetApplicationFeePaymentsByApplication(long Id)
        {
            try
            {
                Application Application = dbContext.Applications.SingleOrDefault(row => row.RowKey == Id);


                var CourseDuration = Application.Course.CourseDuration;
                var duration = Math.Ceiling((Convert.ToDecimal(Application.AcademicTermKey == DbConstants.AcademicTerm.Semester ? CourseDuration / 6 : CourseDuration / 12)));

                short AcademicTermKey = Application.AcademicTermKey;

                var applicationFeeList = (from es in dbContext.FeePaymentMasters.Where(row => row.ApplicationKey == Id)
                                          orderby es.FeeDate descending
                                          select new ApplicationFeePaymentViewModel
                                          {
                                              RowKey = es.RowKey,
                                              ApplicationKey = es.ApplicationKey,
                                              ApplicantName = es.Application.StudentName,
                                              AdmissionNo = es.Application.AdmissionNo,
                                              FeeDate = es.FeeDate,
                                              RecieptNo = es.ReceiptNo,
                                              PaymentModeKey = es.PaymentModeKey,
                                              PaymentModeName = es.PaymentMode.PaymentModeName,
                                              PaymentModeSubKey = es.PaymentModeKey,
                                              PaymentModeSubName = es.PaymentModeSub.PaymentModeSubName,
                                              BankAccountKey = es.BankAccountKey,
                                              BankAccountName = es.BankAccount.Bank.BankName + "-" + es.BankAccount.AccountNumber,
                                              CardNumber = es.CardNumber,
                                              ReferenceNumber = es.ReferenceNumber,
                                              ChequeClearanceDate = es.ChequeClearanceDate,
                                              ChequeOrDDNumber = es.ChequeOrDDNumber,
                                              FeeDescription = es.FeeDescription,
                                              ChequeStatusKey = es.ChequeStatusKey,
                                              ChequeAction = es.ChequeStatusKey == null ? "" : (es.ChequeStatusKey == DbConstants.ProcessStatus.Approved ? EduSuiteUIResources.Approved : EduSuiteUIResources.Rejected),
                                              ChequeApprovedRejectedDate = es.ChequeApprovedRejectedDate,
                                              ChequeRejectedRemarks = es.ChequeRejectedRemarks,
                                              ApplicationFeePaymentDetails = dbContext.FeePaymentDetails.Where(row => row.FeePaymentMasterKey == es.RowKey).Select(row => new ApplicationFeePaymentDetailViewModel
                                              {

                                                  FeeTypeKey = row.FeeTypeKey,
                                                  FeeYear = row.FeeYear,
                                                  FeeTypeName = row.FeeType.FeeTypeName,
                                                  TotalAmount = row.TotalAmount,
                                              }).ToList(),
                                              IsRefunded = es.FeePaymentDetails.Where(y => dbContext.FeeRefundDetails.Where(p => p.FeeRefundMaster.ProcessStatus != DbConstants.ProcessStatus.Rejected).Select(x => x.FeePaymentDetailKey).ToList().Contains(y.RowKey)).Any(),
                                              ReceiptNumberConfigurationKey = es.ReceiptNumberConfigurationKey,
                                              ReceiptNumberConfigurationName = es.PaymentReceiptNumberConfiguration.ConfigName


                                          }).ToList();
                foreach (ApplicationFeePaymentViewModel PaymentDetails in applicationFeeList)
                {
                    foreach (ApplicationFeePaymentDetailViewModel item in PaymentDetails.ApplicationFeePaymentDetails)
                    {
                        item.FeeYearText = item.FeeYear != null ? (CommonUtilities.GetYearDescriptionByCodeDetails(Application.Course.CourseDuration ?? 0, item.FeeYear ?? 0, Application.AcademicTermKey)) : "";
                    }
                }

                return applicationFeeList.GroupBy(x => x.RecieptNo).Select(y => y.First()).OrderByDescending(row => row.SerialNumber).ToList<ApplicationFeePaymentViewModel>();

            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.ApplicationFeePayment, ActionConstants.View, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);
                return new List<ApplicationFeePaymentViewModel>();
            }
        }
        public ApplicationFeePaymentViewModel GetApplicationFeePaymentById(ApplicationFeePaymentViewModel model)
        {
            ApplicationFeePaymentViewModel applicationFeePaymentViewModel = new ApplicationFeePaymentViewModel();
            try
            {
                Application Application = dbContext.Applications.SingleOrDefault(row => row.RowKey == model.ApplicationKey);

                applicationFeePaymentViewModel = dbContext.FeePaymentMasters.Where(row => row.RowKey == model.RowKey).Select(row => new ApplicationFeePaymentViewModel
                {
                    RowKey = row.RowKey,
                    ApplicationKey = row.ApplicationKey,
                    FeeDate = row.FeeDate,
                    RecieptNo = row.ReceiptNo,
                    PaymentModeKey = row.PaymentModeKey,
                    PaymentModeName = row.PaymentMode.PaymentModeName,
                    PaymentModeSubKey = row.PaymentModeSubKey,
                    PaymentModeSubName = row.PaymentModeSub.PaymentModeSubName,
                    CardNumber = row.CardNumber,
                    ReferenceNumber = row.ReferenceNumber,
                    ChequeClearanceDate = row.ChequeClearanceDate,
                    ChequeOrDDNumber = row.ChequeOrDDNumber,
                    FeeDescription = row.FeeDescription,
                    BankAccountKey = row.BankAccount.RowKey,
                    TaxRateTypeKey = row.TaxRateTypeKey,
                    ChequeStatusKey = row.ChequeStatusKey ?? 0,
                    PaidBranchKey = row.PaidBranchKey ?? 0,
                    ReceiptNumberConfigurationKey = row.ReceiptNumberConfigurationKey,
                    ReceiptNumberConfigurationName = row.PaymentReceiptNumberConfiguration.ConfigName
                }).SingleOrDefault();
                if (applicationFeePaymentViewModel == null)
                {
                    applicationFeePaymentViewModel = new ApplicationFeePaymentViewModel();
                    applicationFeePaymentViewModel.RowKey = model.RowKey;
                    applicationFeePaymentViewModel.ApplicationKey = model.ApplicationKey;
                    GenerateReceiptNo(applicationFeePaymentViewModel);
                }
                FillApplicationFeePaymentDetails(applicationFeePaymentViewModel);
                if (applicationFeePaymentViewModel.ApplicationFeePaymentDetails.Count == 0)
                {
                    applicationFeePaymentViewModel.ApplicationFeePaymentDetails.Add(new ApplicationFeePaymentDetailViewModel());
                }

                applicationFeePaymentViewModel.HasInstallment = Application.HasInstallment;
                applicationFeePaymentViewModel.InitialPayment = dbContext.StudentFeeInstallments.Where(x => x.ApplicationKey == model.ApplicationKey && x.InitialPayment != null).Select(x => x.InitialPayment).FirstOrDefault();


                FillApplicationFeeDropdownLists(applicationFeePaymentViewModel);
                applicationFeePaymentViewModel.IsTax = Application.IsTax ?? false;
                applicationFeePaymentViewModel.ApplicantName = Application.StudentName;
                applicationFeePaymentViewModel.AdmissionNo = Application.AdmissionNo;
                applicationFeePaymentViewModel.StudentMobile = Application.StudentMobile;
                applicationFeePaymentViewModel.StudentEmail = Application.StudentEmail;
                FillNotificationDetail(applicationFeePaymentViewModel);
                return applicationFeePaymentViewModel;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.ApplicationFeePayment, ActionConstants.View, DbConstants.LogType.Debug, model.ApplicationKey, ex.GetBaseException().Message);
                return new ApplicationFeePaymentViewModel();
            }
        }
        public void FillApplicationFeeDropdownLists(ApplicationFeePaymentViewModel model)
        {
            FillDropdownLists(model);
            model.TotalPaid = dbContext.FeePaymentDetails.Where(row => row.FeePaymentMaster.Application.RowKey == model.ApplicationKey && row.FeeType.CashFlowTypeKey == DbConstants.CashFlowType.In).Select(row => row.FeeAmount).DefaultIfEmpty().Sum();

        }
        public ApplicationFeePaymentViewModel CreateApplicationFee(ApplicationFeePaymentViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    GenerateReceiptNo(model);
                    long maxKey = dbContext.FeePaymentMasters.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    FeePaymentMaster applicationFeePaymentModel = new FeePaymentMaster();
                    applicationFeePaymentModel.RowKey = Convert.ToInt64(maxKey + 1);
                    //applicationFeePaymentModel.ReceiptNo = model.RecieptNo;
                    //applicationFeePaymentModel.SerialNumber = Convert.ToInt64(model.SerialNumber);

                    applicationFeePaymentModel.ReceiptNo = model.RecieptNo;
                    applicationFeePaymentModel.SerialNumber = model.SerialNumber ?? 0;

                    applicationFeePaymentModel.PaymentModeKey = model.PaymentModeKey;
                    applicationFeePaymentModel.PaymentModeSubKey = model.PaymentModeSubKey;
                    applicationFeePaymentModel.FeeDate = Convert.ToDateTime(model.FeeDate);
                    applicationFeePaymentModel.BankAccountKey = model.BankAccountKey;
                    applicationFeePaymentModel.CardNumber = model.CardNumber;
                    applicationFeePaymentModel.ReferenceNumber = model.ReferenceNumber;

                    applicationFeePaymentModel.ChequeClearanceDate = model.ChequeClearanceDate;
                    applicationFeePaymentModel.ChequeOrDDNumber = model.ChequeOrDDNumber;
                    applicationFeePaymentModel.FeeDescription = model.FeeDescription;
                    applicationFeePaymentModel.ApplicationKey = model.ApplicationKey;
                    applicationFeePaymentModel.TaxRateTypeKey = model.IsTax == true ? model.TaxRateTypeKey : null;
                    applicationFeePaymentModel.PaidBranchKey = model.PaidBranchKey;
                    applicationFeePaymentModel.ReceiptNumberConfigurationKey = model.ReceiptNumberConfigurationKey;
                    decimal? PaidAmount = model.ApplicationFeePaymentDetails.Where(y => y.IsDeduct == false).Select(x => x.TotalAmount ?? 0).DefaultIfEmpty().Sum();
                    if (PaidAmount > 0)
                    {
                        applicationFeePaymentModel.TotalAmount = dbContext.CalculateFeeDetails(model.ApplicationKey).Select(x => x.AdmissionFeeAmount).DefaultIfEmpty().Sum();
                        decimal? TotalPaid = dbContext.CalculateFeeDetails(model.ApplicationKey).Select(x => x.TotalPaid).DefaultIfEmpty().Sum();
                        decimal? BillBalance = applicationFeePaymentModel.TotalAmount - TotalPaid;
                        decimal? BalanceAmount = BillBalance - PaidAmount;
                        applicationFeePaymentModel.BillBalance = BillBalance;
                        applicationFeePaymentModel.PaidAmount = PaidAmount;
                        applicationFeePaymentModel.BalanceAmount = BalanceAmount;
                    }
                    dbContext.FeePaymentMasters.Add(applicationFeePaymentModel);
                    maxKey++;
                    var ApplicationDetails = dbContext.Applications.SingleOrDefault(x => x.RowKey == model.ApplicationKey);

                    decimal totalDebitAmount = model.ApplicationFeePaymentDetails.Select(x => x.TotalAmount ?? 0).DefaultIfEmpty().Sum();
                    decimal totalDebitTaxableAmount = model.ApplicationFeePaymentDetails.Select(x => x.TaxableAmount ?? 0).DefaultIfEmpty().Sum();
                    //decimal AdvancePayment = model.ApplicationFeePaymentDetails.Where(x => x.FeeYear > ApplicationDetails.CurrentYear && dbContext.AdmissionFees.Any(y => y.ApplicationKey== model.ApplicationKey && y.AdmissionFeeYear == x.FeeYear && y.FeeTypeKey == x.FeeTypeKey)).Select(x => x.TotalAmount ?? 0).DefaultIfEmpty().Sum();
                    decimal AdvancePayment = 0;
                    decimal AdvancePaymentTaxable = 0;
                    if (ApplicationDetails.EducationTypeKey == DbConstants.EducationType.RegulerEducation)
                    {
                        AdvancePayment = model.ApplicationFeePaymentDetails.Where(x => x.FeeYear > ApplicationDetails.CurrentYear).Select(x => x.TotalAmount ?? 0).DefaultIfEmpty().Sum();
                        AdvancePaymentTaxable = model.ApplicationFeePaymentDetails.Where(x => x.FeeYear > ApplicationDetails.CurrentYear).Select(x => x.TaxableAmount ?? 0).DefaultIfEmpty().Sum();
                    }
                    long oldBankKey = 0;
                    short oldPaymentModeKey = 0;
                    decimal TotalSGSTAmount = model.ApplicationFeePaymentDetails.Where(x => (x.FeeYear == null || x.FeeYear <= ApplicationDetails.CurrentYear)).Select(row => (row.SGSTAmount ?? 0)).Sum();
                    decimal TotalCGSTAmount = model.ApplicationFeePaymentDetails.Where(x => (x.FeeYear == null || x.FeeYear <= ApplicationDetails.CurrentYear)).Select(row => (row.CGSTAmount ?? 0)).Sum();
                    decimal TotalCessAmount = model.ApplicationFeePaymentDetails.Where(x => (x.FeeYear == null || x.FeeYear <= ApplicationDetails.CurrentYear)).Select(row => (row.CessAmount ?? 0)).Sum();

                    CreateApplicationFeeDetail(model.ApplicationFeePaymentDetails.Where(row => row.RowKey == 0).ToList(), applicationFeePaymentModel, model);

                    string purpose = String.Join(",", model.PurposeList);

                    List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();

                    GeneralConfiguration generalConfiguration = dbContext.GeneralConfigurations.FirstOrDefault();
                    if (generalConfiguration.FeePaymentAccountDate == null || applicationFeePaymentModel.FeeDate >= generalConfiguration.FeePaymentAccountDate)
                    {
                        if (totalDebitAmount != 0)
                        {
                            if (!DbConstants.GeneralConfiguration.AllowAdmissionToAccoount)
                            {
                                decimal TotalIncomeAmount = totalDebitTaxableAmount - AdvancePaymentTaxable;
                                IncomeSplitAmountList(model.ApplicationFeePaymentDetails.ToList(), accountFlowModelList, applicationFeePaymentModel, false, purpose, TotalIncomeAmount);
                            }
                            else
                            {
                                RecievableAmountList(totalDebitTaxableAmount - AdvancePaymentTaxable, accountFlowModelList, applicationFeePaymentModel, false, purpose);
                            }

                            PaymentModeAmountList(totalDebitAmount, applicationFeePaymentModel, accountFlowModelList, false, oldBankKey, oldPaymentModeKey, purpose);

                            CGSTAmountList(TotalCGSTAmount, accountFlowModelList, false, applicationFeePaymentModel, purpose);
                            SGSTAmountList(TotalSGSTAmount, accountFlowModelList, false, applicationFeePaymentModel, purpose);
                            CessAmountList(TotalCessAmount, accountFlowModelList, false, applicationFeePaymentModel, purpose);
                            if (ApplicationDetails.EducationTypeKey == DbConstants.EducationType.RegulerEducation)
                            {
                                AdvanceAmountList(AdvancePayment, accountFlowModelList, applicationFeePaymentModel, false, purpose);
                            }
                            CreateAccountFlow(accountFlowModelList, false);
                        }
                    }
                    if (model.IsSchedule)
                    {
                        CreateFeeFollowup(model);
                    }
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.RowKey = applicationFeePaymentModel.RowKey;
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationFeePayment, ActionConstants.Add, DbConstants.LogType.Info, model.ApplicationKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.FeeDetails);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationFeePayment, ActionConstants.Add, DbConstants.LogType.Error, model.ApplicationKey, ex.GetBaseException().Message);
                }
            }
            FillDropdownLists(model);
            return model;
        }
        public ApplicationFeePaymentViewModel UpdateApplicationFee(ApplicationFeePaymentViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    //GenerateReceiptNo(model);

                    FeePaymentMaster applicationFeePaymentModel = new FeePaymentMaster();
                    applicationFeePaymentModel = dbContext.FeePaymentMasters.SingleOrDefault(row => row.RowKey == model.RowKey);

                    short oldPaymentModeKey = applicationFeePaymentModel.PaymentModeKey ?? 0;
                    long oldBankAccountKey = applicationFeePaymentModel.BankAccountKey ?? 0;
                    decimal OldAmount = applicationFeePaymentModel.FeePaymentDetails.Select(x => x.FeeAmount).DefaultIfEmpty().Sum();

                    applicationFeePaymentModel.PaymentModeKey = model.PaymentModeKey;
                    applicationFeePaymentModel.PaymentModeSubKey = model.PaymentModeSubKey;
                    applicationFeePaymentModel.FeeDate = Convert.ToDateTime(model.FeeDate);
                    applicationFeePaymentModel.BankAccountKey = model.BankAccountKey;
                    applicationFeePaymentModel.CardNumber = model.CardNumber;
                    applicationFeePaymentModel.ReferenceNumber = model.ReferenceNumber;
                    applicationFeePaymentModel.ChequeClearanceDate = model.ChequeClearanceDate;
                    applicationFeePaymentModel.ChequeOrDDNumber = model.ChequeOrDDNumber;
                    applicationFeePaymentModel.ApplicationKey = model.ApplicationKey;
                    applicationFeePaymentModel.FeeDescription = model.FeeDescription;
                    applicationFeePaymentModel.ApplicationKey = model.ApplicationKey;
                    applicationFeePaymentModel.TaxRateTypeKey = model.IsTax == true ? model.TaxRateTypeKey : null;
                    applicationFeePaymentModel.PaidBranchKey = model.PaidBranchKey;
                    applicationFeePaymentModel.ReceiptNumberConfigurationKey = model.ReceiptNumberConfigurationKey;

                    decimal? TotalAdmissionAmount = dbContext.CalculateFeeDetails(model.ApplicationKey).Select(x => x.AdmissionFeeAmount).DefaultIfEmpty().Sum();
                    decimal? PaidAmount = model.ApplicationFeePaymentDetails.Where(y => y.IsDeduct == false).Select(x => x.TotalAmount ?? 0).DefaultIfEmpty().Sum();
                    if (TotalAdmissionAmount != applicationFeePaymentModel.TotalAmount)
                    {
                        decimal? TotalAdmissionAmountDiffrent = applicationFeePaymentModel.TotalAmount - TotalAdmissionAmount;
                        if (TotalAdmissionAmountDiffrent > 0)
                        {
                            applicationFeePaymentModel.TotalAmount = TotalAdmissionAmount;
                            applicationFeePaymentModel.BillBalance = applicationFeePaymentModel.BillBalance - TotalAdmissionAmountDiffrent;
                            applicationFeePaymentModel.PaidAmount = PaidAmount;
                            applicationFeePaymentModel.BalanceAmount = applicationFeePaymentModel.BillBalance - PaidAmount;
                        }
                        else if(TotalAdmissionAmountDiffrent < 0)
                        {
                            applicationFeePaymentModel.TotalAmount = TotalAdmissionAmount;
                            applicationFeePaymentModel.BillBalance = applicationFeePaymentModel.BillBalance - TotalAdmissionAmountDiffrent;
                            applicationFeePaymentModel.PaidAmount = PaidAmount;
                            applicationFeePaymentModel.BalanceAmount = applicationFeePaymentModel.BillBalance - PaidAmount;
                        }
                    }

                    if (PaidAmount != applicationFeePaymentModel.PaidAmount)
                    {

                        //applicationFeePaymentModel.TotalAmount = dbContext.CalculateFeeDetails(model.ApplicationKey).Select(x => x.AdmissionFeeAmount).DefaultIfEmpty().Sum();
                        //decimal? TotalPaid = dbContext.CalculateFeeDetails(model.ApplicationKey).Select(x => x.TotalPaid).DefaultIfEmpty().Sum();
                        //decimal? BillBalance = applicationFeePaymentModel.TotalAmount - (TotalPaid - OldAmount);
                        //decimal? BalanceAmount = BillBalance - PaidAmount;
                        //applicationFeePaymentModel.BillBalance = BillBalance;
                        //applicationFeePaymentModel.PaidAmount = PaidAmount;
                        //applicationFeePaymentModel.BalanceAmount = BalanceAmount;
                        applicationFeePaymentModel.PaidAmount = PaidAmount;
                        applicationFeePaymentModel.BalanceAmount = applicationFeePaymentModel.BillBalance - PaidAmount;

                    }
                    var ApplicationDetails = dbContext.Applications.SingleOrDefault(x => x.RowKey == model.ApplicationKey);
                    decimal totalDebitAmount = model.ApplicationFeePaymentDetails.Select(x => x.TotalAmount ?? 0).DefaultIfEmpty().Sum();
                    decimal totalDebitTaxableAmount = model.ApplicationFeePaymentDetails.Select(x => x.TaxableAmount ?? 0).DefaultIfEmpty().Sum();

                    decimal AdvancePayment = 0;
                    decimal AdvancePaymentTaxable = 0;
                    if (ApplicationDetails.EducationTypeKey == DbConstants.EducationType.RegulerEducation)
                    {
                        AdvancePayment = model.ApplicationFeePaymentDetails.Where(x => x.FeeYear > ApplicationDetails.CurrentYear).Select(x => x.TotalAmount ?? 0).DefaultIfEmpty().Sum();
                        AdvancePaymentTaxable = model.ApplicationFeePaymentDetails.Where(x => x.FeeYear > ApplicationDetails.CurrentYear).Select(x => x.TaxableAmount ?? 0).DefaultIfEmpty().Sum();
                    }
                    decimal TotalSGSTAmount = model.ApplicationFeePaymentDetails.Where(x => (x.FeeYear == null || x.FeeYear <= ApplicationDetails.CurrentYear)).Select(row => (row.SGSTAmount ?? 0)).Sum();
                    decimal TotalCGSTAmount = model.ApplicationFeePaymentDetails.Where(x => (x.FeeYear == null || x.FeeYear <= ApplicationDetails.CurrentYear)).Select(row => (row.CGSTAmount ?? 0)).Sum();
                    decimal TotalCessAmount = model.ApplicationFeePaymentDetails.Where(x => (x.FeeYear == null || x.FeeYear <= ApplicationDetails.CurrentYear)).Select(row => (row.CessAmount ?? 0)).Sum();

                    CreateApplicationFeeDetail(model.ApplicationFeePaymentDetails.Where(row => row.RowKey == 0).ToList(), applicationFeePaymentModel, model);
                    UpdateApplicationFeeDetail(model.ApplicationFeePaymentDetails.Where(row => row.RowKey != 0).ToList(), applicationFeePaymentModel, model);

                    string purpose = String.Join(",", model.PurposeList);
                    purpose = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(purpose.ToLower());
                    List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
                    GeneralConfiguration generalConfiguration = dbContext.GeneralConfigurations.FirstOrDefault();
                    if (generalConfiguration.FeePaymentAccountDate == null || applicationFeePaymentModel.FeeDate >= generalConfiguration.FeePaymentAccountDate)
                    {
                        if (totalDebitAmount != 0)
                        {

                            if (!DbConstants.GeneralConfiguration.AllowAdmissionToAccoount)
                            {
                                decimal TotalIncomeAmount = totalDebitTaxableAmount - AdvancePaymentTaxable;
                                IncomeSplitAmountList(model.ApplicationFeePaymentDetails.ToList(), accountFlowModelList, applicationFeePaymentModel, true, purpose, TotalIncomeAmount);
                            }
                            else
                            {
                                RecievableAmountList(totalDebitTaxableAmount - AdvancePaymentTaxable, accountFlowModelList, applicationFeePaymentModel, true, purpose);
                            }
                            PaymentModeAmountList(totalDebitAmount, applicationFeePaymentModel, accountFlowModelList, true, oldBankAccountKey, oldPaymentModeKey, purpose);

                            CGSTAmountList(TotalCGSTAmount, accountFlowModelList, true, applicationFeePaymentModel, purpose);
                            SGSTAmountList(TotalSGSTAmount, accountFlowModelList, true, applicationFeePaymentModel, purpose);
                            CessAmountList(TotalCessAmount, accountFlowModelList, true, applicationFeePaymentModel, purpose);
                            if (ApplicationDetails.EducationTypeKey == DbConstants.EducationType.RegulerEducation)
                            {
                                AdvanceAmountList(AdvancePayment, accountFlowModelList, applicationFeePaymentModel, true, purpose);
                            }

                            CreateAccountFlow(accountFlowModelList, true);
                        }
                    }
                    if (model.IsSchedule)
                    {
                        CreateFeeFollowup(model);
                    }

                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationFeePayment, ActionConstants.Edit, DbConstants.LogType.Info, model.ApplicationKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.FeeDetails);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationFeePayment, ActionConstants.Edit, DbConstants.LogType.Error, model.ApplicationKey, ex.GetBaseException().Message);
                }
            }
            FillDropdownLists(model);
            return model;

        }
        private void CreateApplicationFeeDetail(List<ApplicationFeePaymentDetailViewModel> modelList, FeePaymentMaster applicationFeePaymentModel, ApplicationFeePaymentViewModel ObjViewModel)
        {
            Int64 MaxKey = dbContext.FeePaymentDetails.Select(p => p.RowKey).DefaultIfEmpty().Max();

            Application Application = dbContext.Applications.FirstOrDefault(row => row.RowKey == applicationFeePaymentModel.ApplicationKey);
            foreach (ApplicationFeePaymentDetailViewModel model in modelList)
            {

                FeePaymentDetail applicationFeePaymentDetailModel = new FeePaymentDetail();
                applicationFeePaymentDetailModel.RowKey = Convert.ToInt64(MaxKey + 1);
                applicationFeePaymentDetailModel.FeeTypeKey = model.FeeTypeKey;
                applicationFeePaymentDetailModel.FeeAmount = Convert.ToDecimal(model.FeeAmount);
                applicationFeePaymentDetailModel.FeeYear = model.FeeYear;
                applicationFeePaymentDetailModel.FeePaymentMasterKey = applicationFeePaymentModel.RowKey;

                applicationFeePaymentDetailModel.TaxableAmount = model.TaxableAmount;
                applicationFeePaymentDetailModel.TotalAmount = model.TotalAmount;

                string FeeTypeName = dbContext.FeeTypes.Where(x => x.RowKey == model.FeeTypeKey).Select(y => y.FeeTypeName).FirstOrDefault();
                string FeeYearText = model.FeeYear != null ? (CommonUtilities.GetYearDescriptionByCodeDetails(Application.Course.CourseDuration ?? 0, model.FeeYear ?? 0, Application.AcademicTermKey)) : "";

                FeeTypeName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(FeeTypeName.ToLower());
                string Purpose = FeeYearText + EduSuiteUIResources.BlankSpace + FeeTypeName;
                ObjViewModel.PurposeList.Add(Purpose);

                if (ObjViewModel.IsTax == true)
                {
                    if (ObjViewModel.TaxRateTypeKey == DbConstants.TaxRateType.Inclusive || ObjViewModel.TaxRateTypeKey == DbConstants.TaxRateType.Exclusive)
                    {
                        applicationFeePaymentDetailModel.CGSTRate = model.CGSTRate;
                        applicationFeePaymentDetailModel.SGSTRate = model.SGSTRate;
                        applicationFeePaymentDetailModel.IGSTRate = model.IGSTRate;
                        applicationFeePaymentDetailModel.CGSTAmount = model.CGSTAmount;
                        applicationFeePaymentDetailModel.SGSTAmount = model.SGSTAmount;
                        applicationFeePaymentDetailModel.IGSTAmount = model.IGSTAmount;
                        applicationFeePaymentDetailModel.CessRate = model.CessRate;
                        applicationFeePaymentDetailModel.CessAmount = model.CessAmount;
                    }
                }
                else
                {

                    applicationFeePaymentDetailModel.CGSTRate = model.CGSTRate;
                    applicationFeePaymentDetailModel.SGSTRate = model.SGSTRate;
                    applicationFeePaymentDetailModel.IGSTRate = model.IGSTRate;
                    applicationFeePaymentDetailModel.CGSTAmount = null;
                    applicationFeePaymentDetailModel.SGSTAmount = null;
                    applicationFeePaymentDetailModel.IGSTAmount = null;
                    applicationFeePaymentDetailModel.CessRate = model.CessRate;
                    applicationFeePaymentDetailModel.CessAmount = null;
                }
                dbContext.FeePaymentDetails.Add(applicationFeePaymentDetailModel);
                MaxKey++;

            }
        }
        private void UpdateApplicationFeeDetail(List<ApplicationFeePaymentDetailViewModel> modelList, FeePaymentMaster applicationFeePaymentModel, ApplicationFeePaymentViewModel ObjViewModel)
        {
            Application Application = dbContext.Applications.FirstOrDefault(row => row.RowKey == applicationFeePaymentModel.ApplicationKey);
            foreach (ApplicationFeePaymentDetailViewModel model in modelList)
            {
                FeePaymentDetail applicationFeePaymentDetailModel = new FeePaymentDetail();
                applicationFeePaymentDetailModel = dbContext.FeePaymentDetails.SingleOrDefault(x => x.RowKey == model.RowKey);
                short oldFeeTypeKey = applicationFeePaymentDetailModel.FeeTypeKey;
                applicationFeePaymentDetailModel.FeeTypeKey = model.FeeTypeKey;
                applicationFeePaymentDetailModel.FeeAmount = Convert.ToDecimal(model.FeeAmount);
                applicationFeePaymentDetailModel.FeeYear = model.FeeYear;
                applicationFeePaymentDetailModel.FeePaymentMasterKey = applicationFeePaymentModel.RowKey;

                applicationFeePaymentDetailModel.TaxableAmount = model.TaxableAmount;
                applicationFeePaymentDetailModel.TotalAmount = model.TotalAmount;

                string FeeTypeName = dbContext.FeeTypes.Where(x => x.RowKey == model.FeeTypeKey).Select(y => y.FeeTypeName).FirstOrDefault();
                string FeeYearText = model.FeeYear != null ? (CommonUtilities.GetYearDescriptionByCodeDetails(Application.Course.CourseDuration ?? 0, model.FeeYear ?? 0, Application.AcademicTermKey)) : "";

                FeeTypeName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(FeeTypeName.ToLower());

                string Purpose = FeeYearText + EduSuiteUIResources.BlankSpace + FeeTypeName;
                ObjViewModel.PurposeList.Add(Purpose);

                if (ObjViewModel.IsTax == true)
                {
                    if (ObjViewModel.TaxRateTypeKey == DbConstants.TaxRateType.Inclusive || ObjViewModel.TaxRateTypeKey == DbConstants.TaxRateType.Exclusive)
                    {
                        applicationFeePaymentDetailModel.CGSTRate = model.CGSTRate;
                        applicationFeePaymentDetailModel.SGSTRate = model.SGSTRate;
                        applicationFeePaymentDetailModel.IGSTRate = model.IGSTRate;
                        applicationFeePaymentDetailModel.CGSTAmount = model.CGSTAmount;
                        applicationFeePaymentDetailModel.SGSTAmount = model.SGSTAmount;
                        applicationFeePaymentDetailModel.IGSTAmount = model.IGSTAmount;
                        applicationFeePaymentDetailModel.CessRate = model.CessRate;
                        applicationFeePaymentDetailModel.CessAmount = model.CessAmount;
                    }
                }
                else
                {

                    applicationFeePaymentDetailModel.CGSTRate = model.CGSTRate;
                    applicationFeePaymentDetailModel.SGSTRate = model.SGSTRate;
                    applicationFeePaymentDetailModel.IGSTRate = model.IGSTRate;
                    applicationFeePaymentDetailModel.CGSTAmount = null;
                    applicationFeePaymentDetailModel.SGSTAmount = null;
                    applicationFeePaymentDetailModel.IGSTAmount = null;
                    applicationFeePaymentDetailModel.CessRate = model.CessRate;
                    applicationFeePaymentDetailModel.CessAmount = null;
                }

                Application ApplicationDetails = dbContext.Applications.SingleOrDefault(x => x.RowKey == applicationFeePaymentModel.ApplicationKey);
                FeeType FeeTypeDetails = dbContext.FeeTypes.SingleOrDefault(x => x.RowKey == applicationFeePaymentDetailModel.FeeTypeKey);
                List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();


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
        private void PaymentModeAmountList(decimal Amount, FeePaymentMaster feePaymentMastermodel, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, long oldBankKey, short oldPaymentModeKey, string purpose)
        {

            long accountHeadKey;
            long ExtraUpdateKey = 0;
            long oldBankAccountHeadKey = 0;
            long oldAccountHeadKey;
            var ApplicationDetails = dbContext.Applications.SingleOrDefault(x => x.RowKey == feePaymentMastermodel.ApplicationKey);
            if (oldPaymentModeKey == DbConstants.PaymentMode.Bank || oldPaymentModeKey == DbConstants.PaymentMode.Cheque)
            {
                oldAccountHeadKey = dbContext.BankAccounts.Where(x => x.RowKey == oldBankKey).Select(x => x.AccountHeadKey).FirstOrDefault();
                oldBankAccountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == oldAccountHeadKey).Select(x => x.RowKey).FirstOrDefault();
            }
            else
            {
                oldAccountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.CashAccount).Select(x => x.RowKey).FirstOrDefault();

            }
            if (feePaymentMastermodel.PaymentModeKey == DbConstants.PaymentMode.Bank || feePaymentMastermodel.PaymentModeKey == DbConstants.PaymentMode.Cheque)
            {
                accountHeadKey = dbContext.BankAccounts.Where(x => x.RowKey == feePaymentMastermodel.BankAccountKey).Select(x => x.AccountHeadKey).FirstOrDefault();
            }
            else
            {
                accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.CashAccount).Select(x => x.RowKey).FirstOrDefault();
            }
            if (oldPaymentModeKey != null && oldPaymentModeKey != 0 && oldPaymentModeKey != feePaymentMastermodel.PaymentModeKey)
            {
                IsUpdate = false;
                ExtraUpdateKey = oldPaymentModeKey == DbConstants.PaymentMode.Cash ? DbConstants.AccountHead.CashAccount : oldBankAccountHeadKey;
            }


            accountFlowModelList.Add(new AccountFlowViewModel
            {
                OldAccountHeadKey = oldAccountHeadKey,
                CashFlowTypeKey = DbConstants.CashFlowType.In,
                AccountHeadKey = accountHeadKey,
                Amount = Amount,
                TransactionTypeKey = DbConstants.TransactionType.FeeMaster,
                TransactionKey = feePaymentMastermodel.RowKey,
                TransactionDate = feePaymentMastermodel.FeeDate,
                VoucherTypeKey = DbConstants.VoucherType.Fee,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = feePaymentMastermodel.PaidBranchKey != null ? feePaymentMastermodel.PaidBranchKey : ApplicationDetails.BranchKey,
                Purpose = purpose + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.RecievedFrom + EduSuiteUIResources.BlankSpace + ApplicationDetails.StudentName + " ( " + ApplicationDetails.AdmissionNo + " ) ",
            });

        }
        private void AdvanceAmountList(decimal FeeAmount, List<AccountFlowViewModel> accountFlowModelList, FeePaymentMaster feePaymentMastermodel, bool IsUpdate, string purpose)
        {
            long accountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.AdvancePayable && x.IsActive == true).Select(x => x.RowKey).FirstOrDefault();
            long ExtraUpdateKey = 0;
            var ApplicationDetails = dbContext.Applications.SingleOrDefault(x => x.RowKey == feePaymentMastermodel.ApplicationKey);
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                AccountHeadKey = accountHeadKey,
                Amount = FeeAmount,
                TransactionTypeKey = DbConstants.TransactionType.FeeMaster,
                TransactionKey = feePaymentMastermodel.RowKey,
                TransactionDate = feePaymentMastermodel.FeeDate,
                VoucherTypeKey = DbConstants.VoucherType.Fee,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = feePaymentMastermodel.PaidBranchKey != null ? feePaymentMastermodel.PaidBranchKey : ApplicationDetails.BranchKey,
                Purpose = purpose + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.RecievedFrom + EduSuiteUIResources.BlankSpace + ApplicationDetails.StudentName + " ( " + ApplicationDetails.AdmissionNo + " ) ",
            });
        }
        private void RecievableAmountList(decimal FeeAmount, List<AccountFlowViewModel> accountFlowModelList, FeePaymentMaster feePaymentMastermodel, bool IsUpdate, string purpose)
        {
            long accountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.AccountsReceivable).Select(x => x.RowKey).FirstOrDefault();
            long ExtraUpdateKey = 0;
            var ApplicationDetails = dbContext.Applications.SingleOrDefault(x => x.RowKey == feePaymentMastermodel.ApplicationKey);
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                AccountHeadKey = accountHeadKey,
                Amount = FeeAmount,
                TransactionTypeKey = DbConstants.TransactionType.FeeMaster,
                TransactionKey = feePaymentMastermodel.RowKey,
                TransactionDate = feePaymentMastermodel.FeeDate,
                VoucherTypeKey = DbConstants.VoucherType.Fee,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = feePaymentMastermodel.PaidBranchKey != null ? feePaymentMastermodel.PaidBranchKey : ApplicationDetails.BranchKey,
                Purpose = purpose + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.RecievedFrom + EduSuiteUIResources.BlankSpace + ApplicationDetails.StudentName + " ( " + ApplicationDetails.AdmissionNo + " ) ",
            });
        }

        private void CGSTAmountList(decimal Amount, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, FeePaymentMaster feePaymentMastermodel, string purpose)
        {
            long ExtraUpdateKey = 0;
            long accountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.OutputTaxCGST && x.IsActive == true).Select(x => x.RowKey).FirstOrDefault();
            var ApplicationDetails = dbContext.Applications.SingleOrDefault(x => x.RowKey == feePaymentMastermodel.ApplicationKey);
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                AccountHeadKey = accountHeadKey,
                Amount = Amount,
                TransactionTypeKey = DbConstants.TransactionType.FeeMaster,
                TransactionDate = feePaymentMastermodel.FeeDate,
                TransactionKey = feePaymentMastermodel.RowKey,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                VoucherTypeKey = DbConstants.VoucherType.Fee,
                BranchKey = feePaymentMastermodel.PaidBranchKey != null ? feePaymentMastermodel.PaidBranchKey : ApplicationDetails.BranchKey,
                Purpose = purpose + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.RecievedFrom + EduSuiteUIResources.BlankSpace + ApplicationDetails.StudentName + " ( " + ApplicationDetails.AdmissionNo + " ) ",
            });
        }
        private void SGSTAmountList(decimal Amount, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, FeePaymentMaster feePaymentMastermodel, string purpose)
        {
            long ExtraUpdateKey = 0;
            long accountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.OutputTaxSGST && x.IsActive == true).Select(x => x.RowKey).FirstOrDefault();
            var ApplicationDetails = dbContext.Applications.SingleOrDefault(x => x.RowKey == feePaymentMastermodel.ApplicationKey);
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                AccountHeadKey = accountHeadKey,
                Amount = Amount,
                TransactionTypeKey = DbConstants.TransactionType.FeeMaster,
                TransactionDate = feePaymentMastermodel.FeeDate,
                TransactionKey = feePaymentMastermodel.RowKey,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = feePaymentMastermodel.PaidBranchKey != null ? feePaymentMastermodel.PaidBranchKey : ApplicationDetails.BranchKey,
                VoucherTypeKey = DbConstants.VoucherType.Fee,
                Purpose = purpose + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.RecievedFrom + EduSuiteUIResources.BlankSpace + ApplicationDetails.StudentName + " ( " + ApplicationDetails.AdmissionNo + " ) ",
            });


        }

        private void CessAmountList(decimal Amount, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, FeePaymentMaster feePaymentMastermodel, string purpose)
        {
            long ExtraUpdateKey = 0;
            long accountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.OutputTaxCess && x.IsActive == true).Select(x => x.RowKey).FirstOrDefault();
            var ApplicationDetails = dbContext.Applications.SingleOrDefault(x => x.RowKey == feePaymentMastermodel.ApplicationKey);
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                AccountHeadKey = accountHeadKey,
                Amount = Amount,
                TransactionTypeKey = DbConstants.TransactionType.FeeMaster,
                TransactionDate = feePaymentMastermodel.FeeDate,
                TransactionKey = feePaymentMastermodel.RowKey,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = feePaymentMastermodel.PaidBranchKey != null ? feePaymentMastermodel.PaidBranchKey : ApplicationDetails.BranchKey,
                VoucherTypeKey = DbConstants.VoucherType.Fee,
                Purpose = purpose + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.RecievedFrom + EduSuiteUIResources.BlankSpace + ApplicationDetails.StudentName + " ( " + ApplicationDetails.AdmissionNo + " ) ",
            });


        }

        private void IncomeSplitAmountList(List<ApplicationFeePaymentDetailViewModel> modelList, List<AccountFlowViewModel> accountFlowModelList, FeePaymentMaster FeePaymentMasterModel, bool IsUpdate, string purpose, decimal TotalIncomeAmount)
        {
            var ApplicationDetails = dbContext.Applications.SingleOrDefault(x => x.RowKey == FeePaymentMasterModel.ApplicationKey);

            if (DbConstants.GeneralConfiguration.AllowSplitCostOfService)
            {
                long ExtraUpdateKey = 0;
                foreach (ApplicationFeePaymentDetailViewModel item in modelList)
                {
                    //if ((item.FeeYear == null || item.FeeYear <= ApplicationDetails.CurrentYear) && dbContext.AdmissionFees.Any(y => y.ApplicationKey == ApplicationDetails.RowKey && y.AdmissionFeeYear == item.FeeYear && y.FeeTypeKey == item.FeeTypeKey))
                    //var CenterShareAmountPer = dbContext.UniversityCourseFees.Where(x => x.FeeType.IsUniverisity && x.FeeTypeKey == item.FeeTypeKey && x.FeeYear == item.FeeYear && x.UniversityCourse.AcademicTermKey == ApplicationDetails.AcademicTermKey && x.UniversityCourse.CourseKey == ApplicationDetails.CourseKey && x.UniversityCourse.UniversityMasterKey == ApplicationDetails.UniversityMasterKey).Select(x => x.CenterShareAmountPer ?? 0).FirstOrDefault();

                    decimal? CenterShareAmountPer = dbContext.UniversityCourseFees.Where(x => DbConstants.GeneralConfiguration.AllowCenterShare && x.FeeTypeKey == item.FeeTypeKey && x.FeeYear == item.FeeYear && x.UniversityCourse.AcademicTermKey == ApplicationDetails.AcademicTermKey && x.UniversityCourse.CourseKey == ApplicationDetails.CourseKey && x.UniversityCourse.UniversityMasterKey == ApplicationDetails.UniversityMasterKey).Select(x => x.CenterShareAmountPer ?? 100).FirstOrDefault();
                    CenterShareAmountPer = (CenterShareAmountPer == 0 || CenterShareAmountPer == null) ? 100 : CenterShareAmountPer;

                    decimal CenterShareAmount = ((item.TaxableAmount ?? 0) * (CenterShareAmountPer ?? 100)) / 100;

                    decimal UniversityAmount = (item.TaxableAmount ?? 0) - (CenterShareAmount);
                    if (ApplicationDetails.EducationTypeKey == DbConstants.EducationType.RegulerEducation)
                    {
                        if ((item.FeeYear == null || item.FeeYear <= ApplicationDetails.CurrentYear))
                        {
                            long AccountHeadKey = dbContext.FeeTypes.Where(x => x.RowKey == item.FeeTypeKey).Select(x => x.AccountHeadKey ?? 0).FirstOrDefault();
                            accountFlowModelList.Add(new AccountFlowViewModel
                            {
                                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                                AccountHeadKey = AccountHeadKey,
                                Amount = CenterShareAmount,
                                TransactionTypeKey = DbConstants.TransactionType.FeeMaster,
                                TransactionDate = Convert.ToDateTime(FeePaymentMasterModel.FeeDate),
                                TransactionKey = FeePaymentMasterModel.RowKey,
                                ExtraUpdateKey = ExtraUpdateKey,
                                IsUpdate = IsUpdate,
                                VoucherTypeKey = DbConstants.VoucherType.Fee,
                                BranchKey = FeePaymentMasterModel.PaidBranchKey != null ? FeePaymentMasterModel.PaidBranchKey : ApplicationDetails.BranchKey,
                                Purpose = purpose + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.RecievedFrom + EduSuiteUIResources.BlankSpace + ApplicationDetails.StudentName + " ( " + ApplicationDetails.AdmissionNo + " ) ",

                            });

                            if (UniversityAmount != 0)
                            {
                                AccountHeadKey = ApplicationDetails.UniversityMaster.AccountHeadKey ?? 0;
                                accountFlowModelList.Add(new AccountFlowViewModel
                                {
                                    CashFlowTypeKey = DbConstants.CashFlowType.Out,
                                    AccountHeadKey = AccountHeadKey,
                                    Amount = UniversityAmount,
                                    TransactionTypeKey = DbConstants.TransactionType.FeeMaster,
                                    TransactionDate = Convert.ToDateTime(FeePaymentMasterModel.FeeDate),
                                    TransactionKey = FeePaymentMasterModel.RowKey,
                                    ExtraUpdateKey = ExtraUpdateKey,
                                    IsUpdate = IsUpdate,
                                    VoucherTypeKey = DbConstants.VoucherType.Fee,
                                    BranchKey = FeePaymentMasterModel.PaidBranchKey != null ? FeePaymentMasterModel.PaidBranchKey : ApplicationDetails.BranchKey,
                                    Purpose = purpose + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.RecievedFrom + EduSuiteUIResources.BlankSpace + ApplicationDetails.StudentName + " ( " + ApplicationDetails.AdmissionNo + " ) ",

                                });
                            }
                        }
                    }
                    else if (ApplicationDetails.EducationTypeKey == DbConstants.EducationType.DistanceEducation)
                    {
                        long AccountHeadKey = dbContext.FeeTypes.Where(x => x.RowKey == item.FeeTypeKey).Select(x => x.AccountHeadKey ?? 0).FirstOrDefault();
                        accountFlowModelList.Add(new AccountFlowViewModel
                        {
                            CashFlowTypeKey = DbConstants.CashFlowType.Out,
                            AccountHeadKey = AccountHeadKey,
                            Amount = CenterShareAmount,
                            TransactionTypeKey = DbConstants.TransactionType.FeeMaster,
                            TransactionDate = Convert.ToDateTime(FeePaymentMasterModel.FeeDate),
                            TransactionKey = FeePaymentMasterModel.RowKey,
                            ExtraUpdateKey = ExtraUpdateKey,
                            IsUpdate = IsUpdate,
                            VoucherTypeKey = DbConstants.VoucherType.Fee,
                            BranchKey = FeePaymentMasterModel.PaidBranchKey != null ? FeePaymentMasterModel.PaidBranchKey : ApplicationDetails.BranchKey,
                            Purpose = purpose + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.RecievedFrom + EduSuiteUIResources.BlankSpace + ApplicationDetails.StudentName + " ( " + ApplicationDetails.AdmissionNo + " ) ",

                        });

                        if (UniversityAmount != 0)
                        {
                            AccountHeadKey = ApplicationDetails.UniversityMaster.AccountHeadKey ?? 0;
                            accountFlowModelList.Add(new AccountFlowViewModel
                            {
                                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                                AccountHeadKey = AccountHeadKey,
                                Amount = UniversityAmount,
                                TransactionTypeKey = DbConstants.TransactionType.FeeMaster,
                                TransactionDate = Convert.ToDateTime(FeePaymentMasterModel.FeeDate),
                                TransactionKey = FeePaymentMasterModel.RowKey,
                                ExtraUpdateKey = ExtraUpdateKey,
                                IsUpdate = IsUpdate,
                                VoucherTypeKey = DbConstants.VoucherType.Fee,
                                BranchKey = FeePaymentMasterModel.PaidBranchKey != null ? FeePaymentMasterModel.PaidBranchKey : ApplicationDetails.BranchKey,
                                Purpose = purpose + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.RecievedFrom + EduSuiteUIResources.BlankSpace + ApplicationDetails.StudentName + " ( " + ApplicationDetails.AdmissionNo + " ) ",

                            });
                        }
                    }
                }
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
                    TransactionTypeKey = DbConstants.TransactionType.FeeMaster,
                    TransactionDate = Convert.ToDateTime(FeePaymentMasterModel.FeeDate),
                    TransactionKey = FeePaymentMasterModel.RowKey,
                    ExtraUpdateKey = ExtraUpdateKey,
                    IsUpdate = IsUpdate,
                    VoucherTypeKey = DbConstants.VoucherType.Admission,
                    BranchKey = FeePaymentMasterModel.PaidBranchKey != null ? FeePaymentMasterModel.PaidBranchKey : ApplicationDetails.BranchKey,
                    Purpose = purpose + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.RecievedFrom + EduSuiteUIResources.BlankSpace + ApplicationDetails.StudentName + " ( " + ApplicationDetails.AdmissionNo + " ) ",

                });
            }
        }

        #endregion
        public ApplicationFeePaymentViewModel DeleteApplicationFee(long Id)
        {
            ApplicationFeePaymentViewModel applicationFeePaymentModel = new ApplicationFeePaymentViewModel();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    FeePaymentMaster applicationFeePayment = dbContext.FeePaymentMasters.SingleOrDefault(row => row.RowKey == Id);
                    List<FeePaymentDetail> applicationFeePaymentDetailList = dbContext.FeePaymentDetails.Where(row => row.FeePaymentMasterKey == Id).ToList();

                    decimal? OldAmount = dbContext.FeePaymentDetails.Where(row => row.FeePaymentMasterKey == Id).Select(x => x.TaxableAmount).DefaultIfEmpty().Sum();

                    if (DbConstants.PaymentMode.BankPaymentModes.Contains(applicationFeePayment.PaymentModeKey ?? 0))
                    {
                        BankAccountService bankAccountService = new BankAccountService(dbContext);
                        BankAccountViewModel bankAccountModel = new BankAccountViewModel();
                        bankAccountModel.RowKey = applicationFeePayment.BankAccountKey ?? 0;
                        bankAccountModel.Amount = OldAmount;
                        bankAccountService.UpdateCurrentAccountBalance(bankAccountModel, true, true, OldAmount);
                    }
                    PaymentReceiptNumberConfiguration dbPaymentReceiptNumberConfiguration = dbContext.PaymentReceiptNumberConfigurations.FirstOrDefault(x => x.RowKey == applicationFeePayment.ReceiptNumberConfigurationKey);
                    if (dbPaymentReceiptNumberConfiguration != null)
                    {
                        if (dbPaymentReceiptNumberConfiguration.MaxValue == applicationFeePayment.SerialNumber)
                        {
                            dbPaymentReceiptNumberConfiguration.MaxValue = applicationFeePayment.SerialNumber - 1;
                        }
                    }
                    dbContext.FeePaymentDetails.RemoveRange(applicationFeePaymentDetailList);

                    dbContext.FeePaymentMasters.Remove(applicationFeePayment);

                    AccountFlowViewModel accountFlowModel = new AccountFlowViewModel();
                    accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.FeeMaster;
                    accountFlowModel.TransactionKey = Id;
                    accountFlowModel.IsDelete = false;
                    AccountFlowService accountFlowService = new AccountFlowService(dbContext);
                    accountFlowService.DeleteAccountFlow(accountFlowModel);

                    bool IsChequeExist = dbContext.ChequeClearances.Where(x => x.TransactionKey == Id && x.TransactionTypeKey == DbConstants.TransactionType.FeeMaster).Any();
                    if (IsChequeExist)
                    {
                        ChequeClearance dbChequeClearance = dbContext.ChequeClearances.Where(x => x.TransactionKey == Id && x.TransactionTypeKey == DbConstants.TransactionType.FeeMaster).SingleOrDefault();
                        dbContext.ChequeClearances.Remove(dbChequeClearance);
                        accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.ChequeClearance;
                        accountFlowModel.TransactionKey = dbChequeClearance.TransactionKey;
                        accountFlowService.DeleteAccountFlow(accountFlowModel);
                    }
                    dbContext.SaveChanges();

                    transaction.Commit();
                    applicationFeePaymentModel.Message = EduSuiteUIResources.Success;
                    applicationFeePaymentModel.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationFeePayment, ActionConstants.Delete, DbConstants.LogType.Info, Id, applicationFeePaymentModel.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        applicationFeePaymentModel.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.FeeDetails);
                        applicationFeePaymentModel.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.ApplicationFeePayment, ActionConstants.Delete, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    applicationFeePaymentModel.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.FeeDetails);
                    applicationFeePaymentModel.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationFeePayment, ActionConstants.Delete, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);
                }
            }
            return applicationFeePaymentModel;
        }
        public ApplicationFeePaymentViewModel DeleteApplicationFeeOneByOne(long Id)
        {
            ApplicationFeePaymentViewModel applicationFeePaymentModel = new ApplicationFeePaymentViewModel();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    FeePaymentDetail applicationFeePaymentDetails = dbContext.FeePaymentDetails.SingleOrDefault(row => row.RowKey == Id);
                    var FeePaymentMasterKey = applicationFeePaymentDetails.FeePaymentMasterKey;
                    FeePaymentMaster feePaymentMaster = dbContext.FeePaymentMasters.SingleOrDefault(row => row.RowKey == FeePaymentMasterKey);


                    Application Application = dbContext.Applications.FirstOrDefault(row => row.RowKey == feePaymentMaster.ApplicationKey);

                    List<FeePaymentDetail> applicationFeePaymentDetailList = dbContext.FeePaymentDetails.Where(row => row.FeePaymentMasterKey == FeePaymentMasterKey).ToList();


                    if (applicationFeePaymentDetailList.Count <= 1)
                    {
                        FeePaymentMaster applicationFeePaymentmaster = dbContext.FeePaymentMasters.SingleOrDefault(row => row.RowKey == FeePaymentMasterKey);

                        List<FeePaymentDetail> FeePaymentDetailsAll = dbContext.FeePaymentDetails.Where(row => row.FeePaymentMasterKey == FeePaymentMasterKey).ToList();
                        dbContext.FeePaymentDetails.RemoveRange(applicationFeePaymentDetailList);


                        dbContext.FeePaymentMasters.Remove(applicationFeePaymentmaster);



                        AccountFlowViewModel accountFlowModel = new AccountFlowViewModel();
                        accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.FeeMaster;
                        accountFlowModel.TransactionKey = FeePaymentMasterKey ?? 0;
                        accountFlowModel.IsDelete = false;
                        AccountFlowService accountFlowService = new AccountFlowService(dbContext);
                        accountFlowService.DeleteAccountFlow(accountFlowModel);

                        bool IsChequeExist = dbContext.ChequeClearances.Where(x => x.TransactionKey == FeePaymentMasterKey && x.TransactionTypeKey == DbConstants.TransactionType.FeeMaster).Any();
                        if (IsChequeExist)
                        {
                            ChequeClearance dbChequeClearance = dbContext.ChequeClearances.Where(x => x.TransactionKey == FeePaymentMasterKey && x.TransactionTypeKey == DbConstants.TransactionType.FeeMaster).SingleOrDefault();
                            dbContext.ChequeClearances.Remove(dbChequeClearance);
                            accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.ChequeClearance;
                            accountFlowModel.TransactionKey = dbChequeClearance.TransactionKey;
                            accountFlowService.DeleteAccountFlow(accountFlowModel);
                        }
                    }
                    else
                    {


                        decimal totalDebitAmount = dbContext.FeePaymentDetails.Where(row => row.RowKey != applicationFeePaymentDetails.RowKey && row.FeePaymentMasterKey == FeePaymentMasterKey).Select(x => x.TotalAmount ?? 0).DefaultIfEmpty().Sum();
                        decimal totalDebitTaxableAmount = dbContext.FeePaymentDetails.Where(row => row.RowKey != applicationFeePaymentDetails.RowKey && row.FeePaymentMasterKey == FeePaymentMasterKey).Select(x => x.TaxableAmount ?? 0).DefaultIfEmpty().Sum();

                        decimal AdvancePayment = 0;
                        decimal AdvancePaymentTaxable = 0;
                        if (Application.EducationTypeKey == DbConstants.EducationType.RegulerEducation)
                        {
                            AdvancePayment = dbContext.FeePaymentDetails.Where(x => x.RowKey != applicationFeePaymentDetails.RowKey && x.FeeYear > feePaymentMaster.Application.CurrentYear).Select(x => x.TotalAmount ?? 0).DefaultIfEmpty().Sum();
                            AdvancePaymentTaxable = dbContext.FeePaymentDetails.Where(x => x.RowKey != applicationFeePaymentDetails.RowKey && x.FeeYear > feePaymentMaster.Application.CurrentYear).Select(x => x.TaxableAmount ?? 0).DefaultIfEmpty().Sum();
                        }
                        long oldBankKey = applicationFeePaymentDetails.FeePaymentMaster.BankAccountKey ?? 0;
                        short oldPaymentModeKey = applicationFeePaymentDetails.FeePaymentMaster.PaymentModeKey ?? 0;
                        decimal TotalSGSTAmount = dbContext.FeePaymentDetails.Where(row => row.RowKey != applicationFeePaymentDetails.RowKey && row.FeePaymentMasterKey == FeePaymentMasterKey).Select(row => (row.SGSTAmount ?? 0)).Sum();
                        decimal TotalCGSTAmount = dbContext.FeePaymentDetails.Where(row => row.RowKey != applicationFeePaymentDetails.RowKey && row.FeePaymentMasterKey == FeePaymentMasterKey).Select(row => (row.CGSTAmount ?? 0)).Sum();
                        decimal TotalCessAmount = dbContext.FeePaymentDetails.Where(row => row.RowKey != applicationFeePaymentDetails.RowKey && row.FeePaymentMasterKey == FeePaymentMasterKey).Select(row => (row.CessAmount ?? 0)).Sum();

                        string FeeTypeName = dbContext.FeeTypes.Where(x => x.RowKey == applicationFeePaymentDetails.FeeTypeKey).Select(y => y.FeeTypeName).FirstOrDefault();
                        string FeeYearText = applicationFeePaymentDetails.FeeYear != null ? (CommonUtilities.GetYearDescriptionByCodeDetails(Application.Course.CourseDuration ?? 0, applicationFeePaymentDetails.FeeYear ?? 0, Application.AcademicTermKey)) : "";

                        string Purpose = FeeYearText + EduSuiteUIResources.BlankSpace + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(FeeTypeName.ToLower());

                        List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();

                        PaymentModeAmountList(totalDebitAmount, feePaymentMaster, accountFlowModelList, true, oldBankKey, oldPaymentModeKey, Purpose);
                        RecievableAmountList(totalDebitTaxableAmount - AdvancePaymentTaxable, accountFlowModelList, feePaymentMaster, true, Purpose);
                        AdvanceAmountList(AdvancePayment, accountFlowModelList, feePaymentMaster, true, Purpose);
                        CGSTAmountList(TotalCGSTAmount, accountFlowModelList, true, feePaymentMaster, Purpose);
                        SGSTAmountList(TotalSGSTAmount, accountFlowModelList, true, feePaymentMaster, Purpose);
                        CessAmountList(TotalCessAmount, accountFlowModelList, true, feePaymentMaster, Purpose);

                        CreateAccountFlow(accountFlowModelList, true);


                        if (DbConstants.PaymentMode.BankPaymentModes.Contains(feePaymentMaster.PaymentModeKey ?? 0))
                        {
                            BankAccountService bankAccountService = new BankAccountService(dbContext);
                            BankAccountViewModel bankAccountModel = new BankAccountViewModel();
                            bankAccountModel.RowKey = feePaymentMaster.BankAccountKey ?? 0;
                            bankAccountModel.Amount = applicationFeePaymentDetails.TaxableAmount;
                            bankAccountService.UpdateCurrentAccountBalance(bankAccountModel, true, true, applicationFeePaymentDetails.TaxableAmount);
                        }

                        dbContext.FeePaymentDetails.Remove(applicationFeePaymentDetails);


                    }

                    dbContext.SaveChanges();
                    transaction.Commit();
                    applicationFeePaymentModel.Message = EduSuiteUIResources.Success;
                    applicationFeePaymentModel.IsSuccessful = true;
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        applicationFeePaymentModel.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.FeeDetails);
                        applicationFeePaymentModel.IsSuccessful = false;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    applicationFeePaymentModel.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.FeeDetails);
                    applicationFeePaymentModel.IsSuccessful = false;
                }
            }
            return applicationFeePaymentModel;
        }
        private void FillApplicationFeePaymentDetails(ApplicationFeePaymentViewModel model)
        {
            long Applicationkey = dbContext.FeePaymentMasters.Where(row => row.RowKey == model.RowKey).Select(x => x.ApplicationKey).SingleOrDefault();
            Application Application = dbContext.Applications.SingleOrDefault(row => row.RowKey == Applicationkey);

            model.ApplicationFeePaymentDetails = dbContext.FeePaymentDetails.Where(row => row.FeePaymentMasterKey == model.RowKey)
                .Select(row => new ApplicationFeePaymentDetailViewModel
                {
                    RowKey = row.RowKey,
                    FeeTypeKey = row.FeeTypeKey,
                    FeeAmount = row.FeeAmount,
                    FeeYear = row.FeeYear,
                    FeeTypeName = row.FeeType.FeeTypeName,
                    CashFlowTypeKey = row.FeeType.CashFlowTypeKey,
                    IsFeeTypeYear = row.FeeType.FeeTypeModeKey == DbConstants.FeeTypeMode.Single ? true : false,
                    IsDeduct = row.FeeType.IsDeduct,
                    CGSTAmount = row.CGSTAmount,
                    SGSTAmount = row.SGSTAmount,
                    IGSTAmount = row.IGSTAmount,
                    CGSTRate = row.CGSTRate,
                    SGSTRate = row.SGSTRate,
                    IGSTRate = row.IGSTRate,
                    CessRate = row.CessRate,
                    CessAmount = row.CessAmount,
                    TaxableAmount = row.TaxableAmount,
                    TotalAmount = row.TotalAmount,
                    GSTAmount = row.CGSTAmount + row.SGSTAmount,
                    GSTRate = row.CGSTRate + row.SGSTRate,
                    HSNCode = row.FeeType.GSTMaster.HSNCode,

                }).ToList();
            foreach (ApplicationFeePaymentDetailViewModel PaymentDetails in model.ApplicationFeePaymentDetails)
            {
                PaymentDetails.FeeYearText = PaymentDetails.FeeYear != null ? (CommonUtilities.GetYearDescriptionByCodeDetails(Application.Course.CourseDuration ?? 0, PaymentDetails.FeeYear ?? 0, Application.AcademicTermKey)) : "";
            }

        }
        private void FillDropdownLists(ApplicationFeePaymentViewModel model)
        {
            FillPaymentModes(model);
            FillPaymentModeSub(model);
            if (model.RowKey != 0)
            {
                FillFeeTypes(model);
            }
            FillFeeYears(model);
            FillBankAccounts(model);
            FillPaidBranches(model);
            FillReceiptType(model);
            FillTaxRateTypes(model);
            FillScheduleFeeTypes(model);
        }
        private void FillPaymentModes(ApplicationFeePaymentViewModel model)
        {
            model.PaymentModes = dbContext.VwPaymentModeSelectActiveOnlies.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.PaymentModeName
            }).ToList();
        }
        public ApplicationFeePaymentViewModel FillPaymentModeSub(ApplicationFeePaymentViewModel model)
        {
            model.PaymentModeSub = dbContext.PaymentModeSubs.Where(x => x.IsActive && x.PaymentModeKey == DbConstants.PaymentMode.Bank).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.PaymentModeSubName
            }).ToList();
            return model;
        }
        private void FillBankAccounts(ApplicationFeePaymentViewModel model)
        {
            var application = dbContext.Applications.SingleOrDefault(x => x.RowKey == model.ApplicationKey);
            model.BankAccounts = dbContext.BranchAccounts.Where(x => x.BranchKey == application.BranchKey && x.BankAccount.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.BankAccount.RowKey,
                Text = (row.BankAccount.NameInAccount ?? row.BankAccount.AccountNumber) + EduSuiteUIResources.Hyphen + row.BankAccount.Bank.BankName
            }).ToList();
        }
        public ApplicationFeePaymentViewModel FillFeeTypes(ApplicationFeePaymentViewModel model)
        {
            if (model.ReceiptNumberConfigurationKey != null && model.ReceiptNumberConfigurationKey != 0)
            {
                if (model.ReceiptNumberConfigurationKey == DbConstants.PaymentReceiptConfigType.Receipt)
                {
                    model.FeeTypes = dbContext.FeeTypes.Where(row => row.IsActive && row.AccountHead.AccountHeadType.AccountGroupKey != DbConstants.AccountGroup.Expenses && (row.ReceiptNumberConfigurationKey ?? 0) == 0).Select(row => new SelectListModel
                    {
                        RowKey = row.RowKey,
                        Text = row.FeeTypeName
                    }).ToList();
                }
                else
                {
                    model.FeeTypes = dbContext.FeeTypes.Where(row => row.IsActive && row.AccountHead.AccountHeadType.AccountGroupKey != DbConstants.AccountGroup.Expenses && row.ReceiptNumberConfigurationKey == model.ReceiptNumberConfigurationKey).Select(row => new SelectListModel
                    {
                        RowKey = row.RowKey,
                        Text = row.FeeTypeName
                    }).ToList();
                }

            }
            else
            {
                model.FeeTypes = dbContext.FeeTypes.Where(row => row.IsActive && row.AccountHead.AccountHeadType.AccountGroupKey != DbConstants.AccountGroup.Expenses).Select(row => new SelectListModel
                {
                    RowKey = row.RowKey,
                    Text = row.FeeTypeName
                }).ToList();
            }
            return model;
        }
        private void FillFeeYears(ApplicationFeePaymentViewModel model)
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
        public ApplicationFeePaymentViewModel GenerateReceiptNo(ApplicationFeePaymentViewModel model)
        {
            //long? MaxKey = (dbContext.FeePaymentMasters.Select(p => p.SerialNumber).DefaultIfEmpty().Max());
            //model.SerialNumber = MaxKey + 1;
            //model.RecieptNo = model.SerialNumber.ToString();

            ConfigurationViewModel ConfigModel = new ConfigurationViewModel();
            ConfigModel.BranchKey = model.PaidBranchKey != null ? model.PaidBranchKey ?? 0 : model.BranchKey;
            PaymentReceiptNumberConfiguration dbpaymentReceiptNumberConfiguration = dbContext.PaymentReceiptNumberConfigurations.Where(x => x.RowKey == model.ReceiptNumberConfigurationKey).FirstOrDefault();
            if (dbpaymentReceiptNumberConfiguration != null)
            {
                ConfigModel.ConfigType = dbpaymentReceiptNumberConfiguration.Type;
            }
            else
            {
                ConfigModel.ConfigType = DbConstants.PaymentReceiptConfigType.Receipt;
            }
            Configurations.GenerateReceipt(dbContext, ConfigModel);

            model.RecieptNo = ConfigModel.ReceiptNumber;
            model.SerialNumber = ConfigModel.SerialNumber;
            return model;
        }
        public byte FillCashFlowTypeKey(short id)
        {

            byte CashFlowTypeKey = dbContext.FeeTypes.Where(x => x.RowKey == id).Select(x => x.CashFlowTypeKey).FirstOrDefault();
            return CashFlowTypeKey;
        }
        public ApplicationFeePaymentDetailViewModel CheckFeeTypeMode(short id, int? Year, long ApplicationKey)
        {
            ApplicationFeePaymentDetailViewModel model = new ApplicationFeePaymentDetailViewModel();

            var IsFeeTypeYear = dbContext.FeeTypes.Any(x => x.RowKey == id && x.FeeTypeModeKey == DbConstants.FeeTypeMode.Single);
            var IsDeduct = dbContext.FeeTypes.Any(x => x.RowKey == id && x.IsDeduct == true);


            if (IsFeeTypeYear == true || (IsFeeTypeYear == false && Year != null))
            {
                model = dbContext.AdmissionFees.Where(x => x.ApplicationKey == ApplicationKey && x.FeeTypeKey == id && x.AdmissionFeeYear == (IsFeeTypeYear == true ? (int?)null : Year)) //(Year ?? x.AdmissionFeeYear))
                              .Select(row => new ApplicationFeePaymentDetailViewModel
                              {
                                  TaxableAmount = null,
                                  IsFeeTypeYear = row.FeeType.FeeTypeModeKey == DbConstants.FeeTypeMode.Single ? true : false,
                                  IsDeduct = row.FeeTypeKey != 0 ? false : true,
                                  CGSTRate = row.Application.IsTax == true ? (row.FeeType.IsTax == true ? row.FeeType.GSTMaster.CGSTRate : 0) : 0,
                                  SGSTRate = row.Application.IsTax == true ? (row.FeeType.IsTax == true ? row.FeeType.GSTMaster.SGSTRate : 0) : 0,
                                  IGSTRate = row.Application.IsTax == true ? (row.FeeType.IsTax == true ? row.FeeType.GSTMaster.IGSTRate : 0) : 0,
                                  CessRate = row.Application.IsTax == true ? (row.FeeType.IsTax == true ? dbContext.GeneralConfigurations.Where(x => System.Data.Entity.DbFunctions.TruncateTime(x.CessToDate) >= System.Data.Entity.DbFunctions.TruncateTime(DateTime.UtcNow)).Select(y => y.CessRate ?? 0).FirstOrDefault() : 0) : 0,
                              }).FirstOrDefault();
            }
            if (model == null)
            {
                model = new ApplicationFeePaymentDetailViewModel();
                model.IsFeeTypeYear = IsFeeTypeYear;
                model.IsDeduct = true;
                model.TaxableAmount = null;
                model.CGSTRate = 0;
                model.SGSTRate = 0;
                model.IGSTRate = 0;
                model.CessRate = 0;
            }
            return model;
        }
        public ApplicationFeePrintViewModel ViewFeePrint(long Id)
        {
            try
            {
                ApplicationFeePrintViewModel model = new ApplicationFeePrintViewModel();
                model = dbContext.FeePaymentMasters.Where(row => row.RowKey == Id).Select(row => new ApplicationFeePrintViewModel
                {
                    CompanyName = row.Application.Branch.Company.CompanyName,
                    BranchName = row.Application.Branch.BranchName,
                    CompanyAddress = row.Application.Branch.AddressLine1,
                    AddressLine2 = row.Application.Branch.AddressLine2,
                    AddressLine3 = row.Application.Branch.AddressLine3,
                    PhoneNumber1 = row.Application.Branch.PhoneNumber1,
                    CityName = row.Application.Branch.CityName,
                    PostalCode = row.Application.Branch.PostalCode,
                    AdmissionNo = row.Application.AdmissionNo,
                    StudentName = row.Application.StudentName,
                    StudentMobile = row.Application.StudentMobile,
                    ProgramName = row.Application.Course.CourseName,
                    UniversityName = row.Application.UniversityMaster.UniversityMasterName,
                    GSTINNumber = row.Application.Branch.Company.GSTINNumber,
                    ApplicationKey = row.Application.RowKey,
                    CompanyLogo = row.Application.Branch.IsFranchise == true ? row.Application.Branch.BranchLogo : row.Application.Branch.Company.CompanyLogo,
                    CompanyLogoPath = row.Application.Branch.IsFranchise == true ? UrlConstants.BranchLogo + row.Application.Branch.BranchLogo : UrlConstants.CompanyLogo + row.Application.Branch.Company.CompanyLogo,
                    IsFranchise = row.Application.Branch.IsFranchise,
                    CompanyWebsite = row.Application.Branch.Company.Website,
                    Branchmail = row.Application.Branch.EmailAddress,
                    BillAddedBy = dbContext.AppUsers.Where(x => x.RowKey == row.AddedBy).Select(y => y.FirstName + " " + y.LastName).FirstOrDefault()
                }).SingleOrDefault();
                model.ApplicationFeePaymentViewModel = (from es in dbContext.FeePaymentMasters.Where(row => row.RowKey == Id)
                                                        select new ApplicationFeePaymentViewModel
                                                        {
                                                            RowKey = es.RowKey,
                                                            FeeDate = es.FeeDate,
                                                            RecieptNo = es.ReceiptNo,
                                                            PaymentModeKey = es.PaymentModeKey,
                                                            PaymentModeName = es.PaymentMode.PaymentModeName,
                                                            PaymentModeSubKey = es.PaymentModeKey,
                                                            PaymentModeSubName = es.PaymentModeSub.PaymentModeSubName,
                                                            BankAccountName = es.BankAccount.Bank.BankName + "-" + es.BankAccount.AccountNumber,
                                                            CardNumber = es.CardNumber,
                                                            ReferenceNumber = es.ReferenceNumber,
                                                            ChequeClearanceDate = es.ChequeClearanceDate,
                                                            ChequeOrDDNumber = es.ChequeOrDDNumber,
                                                            FeeDescription = es.FeeDescription,
                                                            TotalFee = es.TotalAmount,
                                                            TotalPaid = (es.TotalAmount - es.BillBalance) + (es.PaidAmount),
                                                            BalanceFee = es.BalanceAmount,
                                                        }).SingleOrDefault();
                if (model.ApplicationFeePaymentViewModel == null)
                {
                    model.ApplicationFeePaymentViewModel = new ApplicationFeePaymentViewModel();
                }
                FillPaymentModes(model.ApplicationFeePaymentViewModel);
                FillApplicationFeePaymentDetails(model.ApplicationFeePaymentViewModel);

                List<ApplicationFeePaymentDetailViewModel> TotalFeeDetails = new List<ApplicationFeePaymentDetailViewModel>();


                model.AllowHideFeeBalance = dbContext.GeneralConfigurations.Select(x => x.AllowHideFeeBalance).FirstOrDefault();
                return model;
            }
            catch (Exception ex)
            {
                return new ApplicationFeePrintViewModel();
            }
        }
        public List<ApplicationFeePaymentDetailViewModel> BindTotalFeeDetails(ApplicationFeePaymentViewModel model)
        {
            try
            {
                List<ApplicationFeePaymentDetailViewModel> TotalFeeDetails = new List<ApplicationFeePaymentDetailViewModel>();
                Application Application = dbContext.Applications.SingleOrDefault(row => row.RowKey == model.ApplicationKey);

                if (Application != null)
                {
                    var CourseDuration = Application.Course.CourseDuration;
                    var duration = Math.Ceiling((Convert.ToDecimal(Application.AcademicTermKey == DbConstants.AcademicTerm.Semester ? CourseDuration / 6 : CourseDuration / 12)));

                    short AcademicTermKey = dbContext.Applications.Where(row => row.RowKey == model.ApplicationKey).Select(row => row.AcademicTermKey).FirstOrDefault();

                    TotalFeeDetails = dbContext.CalculateFeeDetails(model.ApplicationKey)
                                         .Select(row => new ApplicationFeePaymentDetailViewModel
                                         {
                                             FeeYear = row.AdmissionFeeYear,
                                             FeeTypeKey = row.FeeTypeKey ?? 0,
                                             FeeTypeName = row.FeeTypeName,
                                             FeeYearText = row.AdmissionFeeYear != null ? (duration < 1 ? "Short Term" : CommonUtilities.GetYearDescriptionByCode(row.AdmissionFeeYear ?? 0, AcademicTermKey)) : "",
                                             TotalAmount = row.AdmissionFeeAmount,
                                             FeeAmount = row.FeePaid,
                                             BalanceAmount = row.BalanceFee,
                                             GSTAmount = row.GSTAmount,
                                             TotalCessAmount = row.CessAmount,
                                             GSTRate = row.GSTRate,
                                             TotalCessRate = row.CessRate,
                                             TotalPaid = row.TotalPaid,
                                             OldPaid = row.Oldpaid
                                         }).ToList();


                }
                return TotalFeeDetails;
            }
            catch (Exception ex)
            {
                return new List<ApplicationFeePaymentDetailViewModel>();
            }
        }
        public List<ApplicationFeePaymentViewModel> BindInstallmentFeeDetails(ApplicationFeePaymentViewModel model)
        {
            try
            {
                Application Application = dbContext.Applications.SingleOrDefault(row => row.RowKey == model.ApplicationKey);
                List<ApplicationFeePaymentViewModel> TotalInstallmentDetails = new List<ApplicationFeePaymentViewModel>();

                if (Application != null)
                {
                    var CourseDuration = Application.Course.CourseDuration;
                    var duration = Math.Ceiling((Convert.ToDecimal(Application.AcademicTermKey == DbConstants.AcademicTerm.Semester ? CourseDuration / 6 : CourseDuration / 12)));

                    short AcademicTermKey = dbContext.Applications.Where(row => row.RowKey == model.ApplicationKey).Select(row => row.AcademicTermKey).FirstOrDefault();

                    TotalInstallmentDetails = dbContext.Sp_FeeInstallmentDetails_Select_ByType(model.ApplicationKey)
                        .Select(row => new ApplicationFeePaymentViewModel
                        {
                            InitialPaymentYear = row.FeeYear,
                            InitialPaymentYearText = row.FeeYear != null ? (duration < 1 ? "Short Term" : CommonUtilities.GetYearDescriptionByCode(row.FeeYear ?? 0, AcademicTermKey)) : "",
                            InitialPaymentMonth = row.InstallmentMonth,
                            InitialPaymentAmount = row.InstallmentAmount,
                            InitialPaymentAmountPaid = row.InstallmentPaid,
                            InitialPaymentbalanceDue = row.BalanceDue,
                            InitialPaymentDueDate = row.DueDate

                        }).ToList();
                }
                return TotalInstallmentDetails;
            }
            catch (Exception ex)
            {
                return new List<ApplicationFeePaymentViewModel>();
            }
        }
        public List<ApplicationFeeFollowupDetailsViewModel> BindFeeScheduleDetails(ApplicationFeePaymentViewModel model)
        {
            try
            {
                Application Application = dbContext.Applications.SingleOrDefault(row => row.RowKey == model.ApplicationKey);
                List<ApplicationFeeFollowupDetailsViewModel> FeeScheduleDetails = new List<ApplicationFeeFollowupDetailsViewModel>();

                if (Application != null)
                {
                    FeeScheduleDetails = dbContext.ApplicationFeeFollowups.Where(x => x.ApplicationKey == model.ApplicationKey)
                        .Select(row => new ApplicationFeeFollowupDetailsViewModel
                        {
                            RowKey = row.RowKey,
                            ApplicationKey = row.ApplicationKey,
                            FollowupDate = row.FollowupDate,
                            Remarks = row.Remarks,
                            ProcessStatusKey = row.ProcessStatusKey,
                            ProcessStatusName = row.ProcessStatusKey == DbConstants.ProcessStatus.Approved ? "Completed" : row.ProcessStatu.ProcessStatusName,
                            ApplicationFeeFollowupFeeTypes = dbContext.FeeFollowUpFeeTypes.Where(y => y.ApplicationFeeFollowUpKey == row.RowKey).Select(p => new ApplicationFeeFollowupFeeTypes { FeeTypeName = p.FeeType.FeeTypeName }).ToList(),
                            ApplicationFeeFollowupExtendDates = dbContext.ExtendFeeFollowUpDates.Where(y => y.ApplicationFeeFollowupKey == row.RowKey).Select(p => new ApplicationFeeFollowupExtendDates
                            {
                                OldFollowUpDate = p.OldFollowUpDate,
                                NextFollowUpDate = p.NextFollowUpDate,
                                DateAdded = p.DateAdded,
                                AddedBy = dbContext.AppUsers.Where(j => j.RowKey == p.AddedBy).Select(x => x.AppUserName).FirstOrDefault()
                            }).ToList(),

                        }).ToList();
                }
                return FeeScheduleDetails;
            }
            catch (Exception ex)
            {
                return new List<ApplicationFeeFollowupDetailsViewModel>();
            }
        }
        public List<dynamic> GetApplications(ApplicationViewModel model, out long TotalRecords)
        {
            try
            {
                List<dynamic> applicationList = new List<dynamic>();
                DbParameter TotalRecordsParam = null;

                if (model.SortBy != "")
                {
                    model.SortBy = model.SortBy + " " + model.SortOrder;
                }
                dbContext.LoadStoredProc("dbo.SP_StudentFeeDetails")
                    .WithSqlParam("@BranchKey", model.BranchKey)
                    .WithSqlParam("@BatchKey", model.BatchKey)
                    .WithSqlParam("@CourseKey", model.CourseKey)
                    .WithSqlParam("@UniversityKey", model.UniversityKey)
                    .WithSqlParam("@SearchText", model.ApplicantName.VerifyData())
                    .WithSqlParam("@SearchMobileNumber", model.MobileNumber.VerifyData())
                    .WithSqlParam("@PageIndex", model.PageIndex)
                    .WithSqlParam("@PageSize", model.PageSize)
                    .WithSqlParam("@UserKey", DbConstants.User.UserKey)
                    .WithSqlParam("@SortBy", model.SortBy)
                    .WithSqlParam("@TotalRecords", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalRecordsParam = dbParam;
                    }).ExecuteStoredProc((handler) =>
                    {
                        applicationList = handler.ReadToDynamicList<dynamic>() as List<dynamic>;
                        //applicationList = handler.ReadToList<ApplicationViewModel>() as List<ApplicationViewModel>;
                    });
                TotalRecords = Convert.ToInt64((TotalRecordsParam.Value ?? 0));
                return applicationList;
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                return new List<dynamic>();
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
        private void FillTaxRateTypes(ApplicationFeePaymentViewModel model)
        {
            model.TaxRateTypes = dbContext.TaxRateTypes.Where(row => row.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Code = row.Code,
                Text = row.Name
            }).ToList();
        }
        private void FillBranches(ApplicationViewModel model)
        {
            IQueryable<SelectListModel> BranchQuery = dbContext.vwBranchSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BranchName
            });

            Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
            if (Employee != null)
            {
                if (Employee.BranchAccess != null)
                {
                    List<long> Branches = Employee.BranchAccess.Split(',').Select(Int64.Parse).ToList();
                    model.Branches = BranchQuery.Where(row => Branches.Contains(row.RowKey)).ToList();
                    //model.BranchKey = Employee.BranchKey;
                }
                else
                {
                    model.Branches = BranchQuery.Where(x => x.RowKey == Employee.BranchKey).ToList();
                    //model.BranchKey = Employee.BranchKey;
                }
            }
            else
            {
                model.Branches = BranchQuery.ToList();
            }

            if (model.Branches.Count == 1)
            {
                long? branchkey = model.Branches.Select(x => x.RowKey).FirstOrDefault();
                model.BranchKey = Convert.ToInt16(branchkey);
            }
        }
        public ApplicationViewModel GetSearchDropdownList(ApplicationViewModel model)
        {
            FillBranches(model);
            return model;
        }
        private void FillNotificationDetail(ApplicationFeePaymentViewModel model)
        {
            NotificationTemplate notificationTemplateModel = dbContext.NotificationTemplates.SingleOrDefault(row => row.RowKey == DbConstants.NotificationTemplate.Fee);
            if (notificationTemplateModel != null)
            {
                model.AutoEmail = notificationTemplateModel.AutoEmail;
                model.AutoSMS = notificationTemplateModel.AutoSMS;
                model.TemplateKey = notificationTemplateModel.RowKey;
            }
        }
        private void FillPaidBranches(ApplicationFeePaymentViewModel model)
        {
            model.PaidBranches = dbContext.Branches.Where(row => row.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BranchName
            }).ToList();
        }
        private void FillReceiptType(ApplicationFeePaymentViewModel model)
        {
            List<short> ReceiptTypeKeys = new List<short>();
            ReceiptTypeKeys.Add(DbConstants.PaymentReceiptConfigType.Payment);
            // ReceiptTypeKeys.Add(DbConstants.PaymentReceiptConfigType.Receipt);
            ReceiptTypeKeys.Add(DbConstants.PaymentReceiptConfigType.Refund);
            ReceiptTypeKeys.Add(DbConstants.PaymentReceiptConfigType.ReceiptVoucher);
            ReceiptTypeKeys.Add(DbConstants.PaymentReceiptConfigType.PaymentAndReceipt);
            ReceiptTypeKeys.Add(DbConstants.PaymentReceiptConfigType.SalaryVoucher);


            model.ReceiptTypes = dbContext.PaymentReceiptNumberConfigurations.Where(x => x.IsActive == true && !ReceiptTypeKeys.Contains(x.Type)).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ConfigName,
            }).ToList();
        }
        public void FillScheduleFeeTypes(ApplicationFeePaymentViewModel model)
        {
            model.ScheduleFeeTypes = dbContext.FeeTypes.Where(x => x.IsActive && x.AccountHead.AccountHeadType.AccountGroupKey != DbConstants.AccountGroup.Expenses).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.FeeTypeName
            }).ToList();

        }
        private void CreateFeeFollowup(ApplicationFeePaymentViewModel model)
        {
            ApplicationFeeFollowup ApplicationFeeFollowupModel = new ApplicationFeeFollowup();

            try
            {
                long MaxKey = dbContext.ApplicationFeeFollowups.Select(p => p.RowKey).DefaultIfEmpty().Max();

                ApplicationFeeFollowupModel.RowKey = MaxKey + 1;
                ApplicationFeeFollowupModel.ApplicationKey = model.ApplicationKey;
                ApplicationFeeFollowupModel.FollowupDate = model.ScheduleFollowupDate;
                ApplicationFeeFollowupModel.Remarks = model.ScheduleFeeRemarks;
                ApplicationFeeFollowupModel.ProcessStatusKey = DbConstants.ProcessStatus.Pending;
                ApplicationFeeFollowupModel.IfExtendDate = false;
                dbContext.ApplicationFeeFollowups.Add(ApplicationFeeFollowupModel);

                UpdateFeeTypes(model, ApplicationFeeFollowupModel.RowKey);
            }
            catch (Exception ex)
            {
                model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.AffiliationsTieUpsCourse);
                model.IsSuccessful = false;
                ActivityLog.CreateActivityLog(MenuConstants.FeeSchdeule, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
            }
        }
        private void UpdateFeeTypes(ApplicationFeePaymentViewModel model, long? ApplicationFeeFollowUpKey)
        {
            long MaxKey = dbContext.FeeFollowUpFeeTypes.Select(x => x.RowKey).DefaultIfEmpty().Max();

            if (model.ScheduleFeeTypeKeys != null && model.ScheduleFeeTypeKeys.Count > 0)
            {
                foreach (short FeeTypekey in model.ScheduleFeeTypeKeys)
                {
                    FeeFollowUpFeeType FeeFollowUpFeeTypemodel = new FeeFollowUpFeeType();

                    FeeFollowUpFeeTypemodel.RowKey = MaxKey + 1;
                    FeeFollowUpFeeTypemodel.ApplicationFeeFollowUpKey = ApplicationFeeFollowUpKey ?? 0;
                    FeeFollowUpFeeTypemodel.FeeTypeKey = FeeTypekey;
                    dbContext.FeeFollowUpFeeTypes.Add(FeeFollowUpFeeTypemodel);
                    MaxKey++;
                }
            }
        }
    }

}
