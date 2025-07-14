using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System.Linq.Expressions;
using System.Data.Entity;
using CITS.EduSuite.Business.Models.Resources;
using System.Data.Entity.Infrastructure;

namespace CITS.EduSuite.Business.Services
{
    public class FeeRefundService : IFeeRefundService
    {
        private EduSuiteDatabase dbContext;
        public FeeRefundService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<FeeRefundViewModel> GetStudentFeeRefund(FeeRefundViewModel model, string fromDate, string toDate, out long TotalRecords)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;
                DateTime? ToDate = null;
                DateTime? FromDate = null;
                ToDate = (toDate ?? "") != "" ? Convert.ToDateTime(toDate) : ToDate;
                FromDate = (fromDate ?? "") != "" ? Convert.ToDateTime(fromDate) : FromDate;
                IQueryable<FeeRefundViewModel> StudentRefundList = (from c in dbContext.FeeRefundMasters
                                                                    where (c.Application.StudentName.Contains(model.ApplicantName)|| c.Application.StudentMobile.Contains(model.ApplicantName))
                                                                    select new FeeRefundViewModel
                                                                    {
                                                                        RowKey = c.RowKey,
                                                                        ApplicationKey = c.ApplicationKey,
                                                                        RefundAmount = c.RefundAmount,
                                                                        ChequeStatusKey = c.ChequeStatusKey,
                                                                        RefundDate = c.RefundDate,
                                                                        PaymentModeKey = c.PaymentModeKey,
                                                                        PaymentModeName = c.PaymentMode.PaymentModeName,
                                                                        PaymentModeSubKey = c.PaymentModeSubKey,
                                                                        PaymentModeSubName = c.PaymentModeSub.PaymentModeSubName,
                                                                        CardNumber = c.CardNumber,
                                                                        BankAccountName = c.BankAccount.Bank.BankName,
                                                                        ChequeOrDDNumber = c.ChequeOrDDNumber,
                                                                        ChequeClearanceDate = c.ChequeClearanceDate,
                                                                        Remarks = c.Remarks,
                                                                        ProcessStatus = c.ProcessStatus,
                                                                        ProcessStatusName = c.ProcessStatu1.ProcessStatusName,
                                                                        ApplicantName = c.Application.StudentName,
                                                                        AdmissionNo = c.Application.AdmissionNo,
                                                                        StudentMobile = c.Application.StudentMobile,
                                                                        BatchKey = c.Application.BatchKey,                                                                       
                                                                        BatchName = c.Application.Batch.BatchName,
                                                                        AcademicTermKey = c.Application.AcademicTermKey,
                                                                        CurrentYear = c.Application.CurrentYear,
                                                                        CourseDuration = c.Application.Course.CourseDuration ?? 0,
                                                                        AcademicTermName = c.Application.AcademicTerm.AcademicTermName,
                                                                        CourseName = c.Application.Course.CourseName,
                                                                        UniversityName = c.Application.UniversityMaster.UniversityMasterName,
                                                                        BranchKey = c.Application.BranchKey,
                                                                        BranchName = c.Application.Branch.BranchName,
                                                                        CourseKey = c.Application.CourseKey,
                                                                        UniversityKey = c.Application.UniversityMasterKey,
                                                                    });
                if (model.BranchKey != 0)
                {
                    StudentRefundList = StudentRefundList.Where(row => row.BranchKey == model.BranchKey);
                }
                if (model.BatchKey != 0)
                {
                    StudentRefundList = StudentRefundList.Where(row => row.BatchKey == model.BatchKey);
                }
                if (FromDate != null)
                {
                    StudentRefundList = StudentRefundList.Where(row => row.RefundDate >= FromDate);
                }
                if (ToDate != null)
                {
                    StudentRefundList = StudentRefundList.Where(row => row.RefundDate <= ToDate);
                }
                if (model.CourseKey != 0)
                {
                    StudentRefundList = StudentRefundList.Where(row => row.CourseKey == model.CourseKey);
                }
                if (model.UniversityKey != 0)
                {
                    StudentRefundList = StudentRefundList.Where(row => row.UniversityKey == model.UniversityKey);
                }



                StudentRefundList = StudentRefundList.GroupBy(x => x.RowKey).Select(y => y.FirstOrDefault());
                if (model.SortBy != "")
                {
                    StudentRefundList = SortApplications(StudentRefundList, model.SortBy, model.SortOrder);
                }
                TotalRecords = StudentRefundList.Count();
                return StudentRefundList.Skip(Skip).Take(Take).ToList<FeeRefundViewModel>();

            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.EmployeeSalaryAdvanceReturn, ActionConstants.MenuAccess, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return new List<FeeRefundViewModel>();
            }
        }
        private IQueryable<FeeRefundViewModel> SortApplications(IQueryable<FeeRefundViewModel> Query, string SortName, string SortOrder)
        {

            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(FeeRefundViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<FeeRefundViewModel>(resultExpression);

        }
        public FeeRefundViewModel GetStudentFeeRefundById(FeeRefundViewModel model)
        {

            try
            {
                FeeRefundViewModel objViewModel = new FeeRefundViewModel();
                objViewModel = dbContext.FeeRefundMasters.Where(x => x.RowKey == model.RowKey).Select(row => new FeeRefundViewModel
                {
                    RowKey = row.RowKey,
                    ApplicationKey = row.ApplicationKey,
                    PaymentModeKey = row.PaymentModeKey,
                    PaymentModeSubKey = row.PaymentModeSubKey,
                    RefundAmount = row.RefundAmount,
                    RefundDate = row.RefundDate,
                    BankAccountKey = row.BankAccountKey,
                    BankAccountBalance = row.BankAccount.CurrentAccountBalance,
                    CardNumber = row.CardNumber,
                    ChequeOrDDNumber = row.ChequeOrDDNumber,
                    ChequeClearanceDate = row.ChequeClearanceDate,
                    VoucherNo = row.VoucherNo,
                    Remarks = row.Remarks,
                    BranchKey = row.Application.BranchKey,
                    ReferenceNumber = row.ReferenceNumber,
                    ProcessStatus = row.ProcessStatus,
                    PaidBranchKey = row.PaidBranchKey

                }).FirstOrDefault();
                if (objViewModel == null)
                {
                    objViewModel = new FeeRefundViewModel();
                    objViewModel.PaymentModeKey = DbConstants.PaymentMode.Cash;
                    objViewModel.ApplicationKey = model.ApplicationKey;
                    objViewModel.ProcessStatus = DbConstants.ProcessStatus.Pending;
                }

                FillSalaryAdvanceReturnDropdownLists(objViewModel);

                return objViewModel;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.StudentFeeRefund, (model.RowKey != 0 ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return new FeeRefundViewModel(); ;
            }
        }
        private void FillSalaryAdvanceReturnDropdownLists(FeeRefundViewModel model)
        {
            FillPaymentModes(model);
            FillPaymentModeSub(model);
            FillBankAccounts(model);
            FillPaidBranches(model);
        }
        private void FillPaymentModes(FeeRefundViewModel model)
        {
            model.PaymentModes = dbContext.VwPaymentModeSelectActiveOnlies.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.PaymentModeName
            }).ToList();
        }
        public FeeRefundViewModel FillPaymentModeSub(FeeRefundViewModel model)
        {
            model.PaymentModeSub = dbContext.PaymentModeSubs.Where(x => x.IsActive && x.PaymentModeKey == DbConstants.PaymentMode.Bank).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.PaymentModeSubName
            }).ToList();
            return model;
        }
        private void FillBankAccounts(FeeRefundViewModel model)
        {
            var application = dbContext.Applications.SingleOrDefault(x => x.RowKey == model.ApplicationKey);
            model.BankAccounts = dbContext.BranchAccounts.Where(x => x.BankAccount.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.BankAccount.RowKey,
                Text = (row.BankAccount.NameInAccount ?? row.BankAccount.AccountNumber) + EduSuiteUIResources.Hyphen + row.BankAccount.Bank.BankName
            }).Distinct().ToList();
        }
        public FeeRefundViewModel fillAdvances(FeeRefundViewModel model)
        {
            Application Application = dbContext.Applications.FirstOrDefault(row => row.RowKey == model.ApplicationKey);

            if (Application != null)
            {
                var CourseDuration = Application.Course.CourseDuration;
                var duration = Math.Ceiling((Convert.ToDecimal(Application.AcademicTermKey == DbConstants.AcademicTerm.Semester ? CourseDuration / 6 : CourseDuration / 12)));

                short AcademicTermKey = dbContext.Applications.Where(row => row.RowKey == model.ApplicationKey).Select(row => row.AcademicTermKey).FirstOrDefault();

                if (model.RowKey != 0)
                {
                    model.FeeRefundDetails = dbContext.FeeRefundDetails.Where(x => x.FeeRefundMasterKey == model.RowKey).ToList().Select(x => new FeeRefundDetailViewModel
                    {
                        RowKey = x.RowKey,
                        FeeRefundMasterKey = x.FeeRefundMasterKey ?? 0,
                        FeePaymentDetailKey = x.FeePaymentDetailKey,
                        FeeTypeKey = x.FeeTypeKey,
                        FeeTypeName = x.FeeType.FeeTypeName,
                        FeeYear = x.FeeYear,
                        FeeYearText = x.FeeYear != null ? (duration < 1 ? "Short Term" : CommonUtilities.GetYearDescriptionByCode(x.FeeYear ?? 0, AcademicTermKey)) : "",
                        ReturnAmount = x.FeeAmount,
                        BeforeTakenAdvance = (x.FeePaymentDetail.FeeAmount) - ((dbContext.FeeRefundDetails.Where(y => y.FeePaymentDetailKey == x.FeePaymentDetailKey).Select(y => y.FeeAmount).DefaultIfEmpty().Sum()) - x.FeeAmount),
                        IsDeduct = true,
                        RecieptNo = x.FeePaymentDetail.FeePaymentMaster.ReceiptNo,
                        FeeRecieptDate = x.FeePaymentDetail.FeePaymentMaster.FeeDate,
                    }).Union(dbContext.FeePaymentDetails.Where(x => x.FeePaymentMaster.ApplicationKey == model.ApplicationKey
                    && !dbContext.FeeRefundDetails.Where(p => p.FeePaymentDetailKey == x.RowKey).Any()
                        && ((x.FeePaymentMaster.ChequeStatusKey ?? DbConstants.ProcessStatus.Approved) == DbConstants.ProcessStatus.Approved)).ToList().Select(x => new FeeRefundDetailViewModel
                        {
                            RowKey = 0,
                            FeeRefundMasterKey = 0,
                            FeePaymentDetailKey = x.RowKey,
                            FeeTypeKey = x.FeeTypeKey,
                            FeeTypeName = x.FeeType.FeeTypeName,
                            FeeYearText = x.FeeYear != null ? (duration < 1 ? "Short Term" : CommonUtilities.GetYearDescriptionByCode(x.FeeYear ?? 0, AcademicTermKey)) : "",
                            FeeYear = x.FeeYear,
                            ReturnAmount = x.FeeAmount,
                            BeforeTakenAdvance = x.FeeAmount,
                            IsDeduct = false,
                            RecieptNo = x.FeePaymentMaster.ReceiptNo,
                            FeeRecieptDate = x.FeePaymentMaster.FeeDate,
                        })).ToList();


                }
                else
                {

                    model.FeeRefundDetails = dbContext.FeePaymentDetails.Where(x => x.FeePaymentMaster.ApplicationKey == model.ApplicationKey && ((x.FeePaymentMaster.ChequeStatusKey ?? DbConstants.ProcessStatus.Approved) == DbConstants.ProcessStatus.Approved)).OrderBy(row => row.FeeYear).ToList().Select(x => new FeeRefundDetailViewModel
                    {
                        RowKey = 0,
                        FeeRefundMasterKey = 0,
                        FeePaymentDetailKey = x.RowKey,
                        FeeTypeKey = x.FeeTypeKey,
                        FeeTypeName = x.FeeType.FeeTypeName,
                        FeeYear = x.FeeYear,
                        FeeYearText = x.FeeYear != null ? (duration < 1 ? "Short Term" : CommonUtilities.GetYearDescriptionByCode(x.FeeYear ?? 0, AcademicTermKey)) : "",
                        ReturnAmount = x.FeeAmount - (x.FeeRefundDetails.Where(y => y.FeeRefundMaster.ProcessStatus != DbConstants.ProcessStatus.Rejected).Select(y => y.FeeAmount).DefaultIfEmpty().Sum()),
                        BeforeTakenAdvance = x.FeeAmount - (x.FeeRefundDetails.Where(y => y.FeeRefundMaster.ProcessStatus != DbConstants.ProcessStatus.Rejected).Select(y => y.FeeAmount).DefaultIfEmpty().Sum()),
                        IsDeduct = true,
                        RecieptNo = x.FeePaymentMaster.ReceiptNo,
                        FeeRecieptDate = x.FeePaymentMaster.FeeDate,
                    }).ToList();
                }
            }
            return model;
        }
        public decimal GetBalanceforRefund(short PaymentModeKey, long Rowkey, long BankAccountKey, short branchKey)
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
                    var purchaseList = dbContext.FeeRefundMasters.SingleOrDefault(x => x.RowKey == Rowkey);
                    if (PaymentModeKey == purchaseList.PaymentModeKey)
                    {
                        Balance = Balance + purchaseList.RefundAmount ?? 0;
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
                        var purchaseList = dbContext.FeeRefundMasters.SingleOrDefault(x => x.RowKey == Rowkey);
                        if (BankAccountKey == purchaseList.BankAccountKey)
                        {
                            Balance = Balance + purchaseList.RefundAmount ?? 0;
                        }
                    }
                }
            }
            return Balance;
        }
        public FeeRefundViewModel DeleteFeeRefund(FeeRefundViewModel model)
        {
            FeeRefundMaster FeeRefundMasterModel = new FeeRefundMaster();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    FeeRefundMasterModel = dbContext.FeeRefundMasters.SingleOrDefault(row => row.RowKey == model.RowKey);
                    long RowKey = FeeRefundMasterModel.RowKey;


                    List<FeeRefundDetail> DetailList = dbContext.FeeRefundDetails.Where(x => x.FeeRefundMasterKey == model.RowKey).ToList();
                    if (DetailList.Count > 0)
                    {

                        dbContext.FeeRefundDetails.RemoveRange(DetailList);
                    }

                    dbContext.FeeRefundMasters.Remove(FeeRefundMasterModel);
                    AccountFlowViewModel accountFlowModel = new AccountFlowViewModel();
                    AccountFlowService accountFlowService = new AccountFlowService(dbContext);
                    accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.FeeRefund;
                    accountFlowModel.TransactionKey = RowKey;
                    accountFlowService.DeleteAccountFlow(accountFlowModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeSalaryAdvanceReturn, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.SalaryPayment);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeSalaryAdvanceReturn, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }

            }

            return model;
        }
        public FeeRefundViewModel CreateFeeRefund(FeeRefundViewModel model)
        {
            FeeRefundMaster FeeRefundModel = new FeeRefundMaster();
            //FillSalaryAdvanceReturnDropdownLists(model);

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Int64 maxKey = dbContext.FeeRefundMasters.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    FeeRefundModel.RowKey = Convert.ToInt64(maxKey + 1);
                    FeeRefundModel.ApplicationKey = Convert.ToInt32(model.ApplicationKey);
                    FeeRefundModel.RefundAmount = Convert.ToDecimal(model.RefundAmount);
                    FeeRefundModel.RefundDate = Convert.ToDateTime(model.RefundDate);
                    FeeRefundModel.PaymentModeKey = model.PaymentModeKey;
                    FeeRefundModel.BankAccountKey = model.BankAccountKey;
                    FeeRefundModel.PaymentModeSubKey = model.PaymentModeSubKey;
                    FeeRefundModel.CardNumber = model.CardNumber;
                    FeeRefundModel.BankAccountKey = model.BankAccountKey;
                    FeeRefundModel.ChequeOrDDNumber = model.ChequeOrDDNumber;
                    FeeRefundModel.ChequeClearanceDate = model.ChequeClearanceDate;
                    FeeRefundModel.ProcessStatus = DbConstants.ProcessStatus.Pending;

                    FeeRefundModel.ReferenceNumber = model.ReferenceNumber;

                    FeeRefundModel.Remarks = model.Remarks;
                    FeeRefundModel.PaidBranchKey = model.PaidBranchKey;

                    if (model.PaymentModeKey == DbConstants.PaymentMode.Cheque)
                    {
                        FeeRefundModel.ChequeStatusKey = DbConstants.ProcessStatus.Pending;
                    }
                    long bankKey = 0;
                    model.RowKey = FeeRefundModel.RowKey;
                    model.ApplicantName = dbContext.Applications.Where(x => x.RowKey == model.ApplicationKey).Select(x => x.StudentName).FirstOrDefault();

                    dbContext.FeeRefundMasters.Add(FeeRefundModel);
                    if (model.BankAccountKey != null && model.BankAccountKey != 0)
                    {
                        var BankAccountList = dbContext.BankAccounts.SingleOrDefault(x => x.RowKey == model.BankAccountKey);
                        model.BankAccountName = (BankAccountList.NameInAccount ?? BankAccountList.AccountNumber) + "-" + BankAccountList.Bank.BankName;
                    }
                    createFeeRefundDetail(model);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    model.RowKey = FeeRefundModel.RowKey;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentFeeRefund, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.StudentFeeRefund);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentFeeRefund, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }

            }

            return model;
        }
        public FeeRefundViewModel UpdateFeeRefund(FeeRefundViewModel model)
        {
            FeeRefundMaster FeeRefundModel = new FeeRefundMaster();
            //FillSalaryAdvanceReturnDropdownLists(model);

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    FeeRefundModel = dbContext.FeeRefundMasters.SingleOrDefault(row => row.RowKey == model.RowKey);
                    FeeRefundModel.RefundAmount = Convert.ToDecimal(model.RefundAmount);
                    FeeRefundModel.RefundDate = Convert.ToDateTime(model.RefundDate);
                    model.OldPaymentModeKey = FeeRefundModel.PaymentModeKey;
                    long oldBank = FeeRefundModel.BankAccountKey ?? 0;
                    FeeRefundModel.PaymentModeKey = model.PaymentModeKey;
                    FeeRefundModel.BankAccountKey = model.BankAccountKey;
                    FeeRefundModel.PaymentModeSubKey = model.PaymentModeSubKey;
                    FeeRefundModel.CardNumber = model.CardNumber;

                    FeeRefundModel.BankAccountKey = model.BankAccountKey;
                    FeeRefundModel.ChequeOrDDNumber = model.ChequeOrDDNumber;
                    FeeRefundModel.ChequeClearanceDate = model.ChequeClearanceDate;
                    FeeRefundModel.ReferenceNumber = model.ReferenceNumber;
                    FeeRefundModel.Remarks = model.Remarks;
                    FeeRefundModel.PaidBranchKey = model.PaidBranchKey;

                    if (model.PaymentModeKey == DbConstants.PaymentMode.Cheque)
                    {
                        FeeRefundModel.ChequeStatusKey = DbConstants.ProcessStatus.Pending;
                    }
                    model.ApplicantName = dbContext.Applications.Where(x => x.RowKey == model.ApplicationKey).Select(x => x.StudentName).FirstOrDefault();

                    if (model.BankAccountKey != null && model.BankAccountKey != 0)
                    {
                        var BankAccountList = dbContext.BankAccounts.SingleOrDefault(x => x.RowKey == model.BankAccountKey);
                        model.BankAccountName = (BankAccountList.NameInAccount ?? BankAccountList.AccountNumber) + "-" + BankAccountList.Bank.BankName;
                    }
                    updateFeeRefundDetail(model);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentFeeRefund, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.StudentFeeRefund);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentFeeRefund, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }

            }

            return model;
        }
        private void updateFeeRefundDetail(FeeRefundViewModel model)
        {
            long maxkey = dbContext.FeeRefundDetails.Select(x => x.RowKey).DefaultIfEmpty().Max();
            if (model.FeeRefundDetails.Count() != 0)
            {
                decimal advance = 0;
                decimal Amount = (model.RefundAmount ?? 0);
                foreach (FeeRefundDetailViewModel advanceList in model.FeeRefundDetails)
                {

                    if (dbContext.FeeRefundDetails.Any(x => x.RowKey == advanceList.RowKey))
                    {
                        FeeRefundDetail feeRefundDetail = new FeeRefundDetail();
                        feeRefundDetail = dbContext.FeeRefundDetails.SingleOrDefault(x => x.RowKey == advanceList.RowKey);

                        if (advanceList.IsDeduct == true)
                        {
                            if (Amount != 0)
                            {
                                if (advanceList.ReturnAmount <= Amount)
                                {

                                    Amount = Amount - (advanceList.ReturnAmount ?? 0);
                                    feeRefundDetail.FeeRefundMasterKey = model.RowKey;
                                    feeRefundDetail.FeePaymentDetailKey = advanceList.FeePaymentDetailKey;
                                    feeRefundDetail.FeeAmount = (advanceList.ReturnAmount);
                                    feeRefundDetail.FeeYear = advanceList.FeeYear;
                                    feeRefundDetail.FeeTypeKey = advanceList.FeeTypeKey ?? 0;
                                }
                                else
                                {
                                    advance = advance + Amount;
                                    feeRefundDetail.FeeRefundMasterKey = model.RowKey;
                                    feeRefundDetail.FeePaymentDetailKey = advanceList.FeePaymentDetailKey;
                                    feeRefundDetail.FeeAmount = Amount;
                                    feeRefundDetail.FeeYear = advanceList.FeeYear;
                                    feeRefundDetail.FeeTypeKey = advanceList.FeeTypeKey ?? 0;
                                    Amount = 0;
                                }
                            }
                        }
                        else
                        {

                            dbContext.FeeRefundDetails.Remove(feeRefundDetail);
                        }
                    }
                    else
                    {
                        if (advanceList.IsDeduct == true && Amount != 0)
                        {

                            FeeRefundDetail feeRefundDetail = new FeeRefundDetail();
                            if (advanceList.ReturnAmount <= Amount)
                            {

                                Amount = Amount - (advanceList.ReturnAmount ?? 0);
                                feeRefundDetail.RowKey = maxkey + 1;
                                feeRefundDetail.FeeRefundMasterKey = model.RowKey;
                                feeRefundDetail.FeePaymentDetailKey = advanceList.FeePaymentDetailKey;
                                feeRefundDetail.FeeAmount = advanceList.ReturnAmount;
                                feeRefundDetail.FeeYear = advanceList.FeeYear;
                                feeRefundDetail.FeeTypeKey = advanceList.FeeTypeKey ?? 0;
                            }
                            else
                            {
                                advance = advance + Amount;
                                feeRefundDetail.RowKey = maxkey + 1;
                                feeRefundDetail.FeeRefundMasterKey = model.RowKey;
                                feeRefundDetail.FeePaymentDetailKey = advanceList.FeePaymentDetailKey;
                                feeRefundDetail.FeeAmount = Amount;
                                feeRefundDetail.FeeYear = advanceList.FeeYear;
                                feeRefundDetail.FeeTypeKey = advanceList.FeeTypeKey ?? 0;
                                Amount = 0;
                            }
                            dbContext.FeeRefundDetails.Add(feeRefundDetail);
                            maxkey++;
                        }
                    }
                }
            }
        }
        private void createFeeRefundDetail(FeeRefundViewModel model)
        {
            if (model.FeeRefundDetails.Where(x => x.IsDeduct == true).Count() != 0)
            {
                decimal Amount = model.RefundAmount ?? 0;
                long maxkey = dbContext.FeeRefundDetails.Select(x => x.RowKey).DefaultIfEmpty().Max();
                foreach (FeeRefundDetailViewModel advanceList in model.FeeRefundDetails.Where(x => x.IsDeduct == true))
                {
                    if (Amount != 0)
                    {

                        FeeRefundDetail feeRefundDetail = new FeeRefundDetail();
                        if (advanceList.ReturnAmount <= Amount)
                        {
                            feeRefundDetail.RowKey = maxkey + 1;
                            feeRefundDetail.FeeRefundMasterKey = model.RowKey;
                            feeRefundDetail.FeePaymentDetailKey = advanceList.FeePaymentDetailKey;
                            feeRefundDetail.FeeAmount = advanceList.ReturnAmount;
                            feeRefundDetail.FeeYear = advanceList.FeeYear;
                            feeRefundDetail.FeeTypeKey = advanceList.FeeTypeKey ?? 0;
                        }
                        else
                        {
                            feeRefundDetail.RowKey = maxkey + 1;
                            feeRefundDetail.FeeRefundMasterKey = model.RowKey;
                            feeRefundDetail.FeePaymentDetailKey = advanceList.FeePaymentDetailKey;
                            feeRefundDetail.FeeAmount = advanceList.ReturnAmount;
                            feeRefundDetail.FeeYear = advanceList.FeeYear;
                            feeRefundDetail.FeeTypeKey = advanceList.FeeTypeKey ?? 0;
                            Amount = 0;
                        }
                        dbContext.FeeRefundDetails.Add(feeRefundDetail);
                        maxkey++;
                    }
                }
            }
        }
        public FeeRefundViewModel ApproveFeeRefund(FeeRefundViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    FeeRefundMaster feeRefundMaster = dbContext.FeeRefundMasters.SingleOrDefault(row => row.RowKey == model.RowKey);
                    Application application = dbContext.Applications.FirstOrDefault(x => x.RowKey == feeRefundMaster.ApplicationKey);
                    if (model.ProcessStatus == DbConstants.ProcessStatus.Approved)
                    {
                        GenerateReceiptNo(model);
                        feeRefundMaster.VoucherNo = model.VoucherNo;
                        feeRefundMaster.SerialNumber = model.SerialNumber ?? 0;
                        feeRefundMaster.ProcessStatus = DbConstants.ProcessStatus.Approved;
                        List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
                        accountFlowModelList = ReturnAmountList(feeRefundMaster, accountFlowModelList, false, application);
                        CreateAccountFlow(accountFlowModelList, false);
                    }
                    else
                    {
                        feeRefundMaster.ProcessStatus = DbConstants.ProcessStatus.Rejected;
                    }
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentFeeRefund, ActionConstants.View, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantApproveRejectErrorMessage, EduSuiteUIResources.StudentFeeRefund);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.StudentFeeRefund, ActionConstants.View, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.StudentFeeRefund);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentFeeRefund, ActionConstants.View, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        private List<AccountFlowViewModel> ReturnAmountList(FeeRefundMaster model, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, Application application)
        {
            long accountHeadKey;
            long ExtraUpdateKey = 0;
            accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.FeeRefund).Select(x => x.RowKey).FirstOrDefault();
            //accountHeadKey = dbContext.Employees.Where(x => x.RowKey == model.EmployeeKey).Select(x => x.AccountHeadKey ?? 0).FirstOrDefault();

            List<FeeRefundDetail> FeeRefundList = dbContext.FeeRefundDetails.Where(x => x.FeeRefundMasterKey == model.RowKey).ToList();

            foreach (FeeRefundDetail advanceList in FeeRefundList)
            {

                accountFlowModelList.Add(new AccountFlowViewModel
                {
                    CashFlowTypeKey = DbConstants.CashFlowType.In,
                    AccountHeadKey = advanceList.FeePaymentDetail.FeeType.AccountHeadKey ?? 0,
                    Amount = advanceList.FeeAmount ?? 0,
                    TransactionTypeKey = DbConstants.TransactionType.FeeRefund,
                    VoucherTypeKey = DbConstants.VoucherType.FeeRefund,
                    TransactionKey = model.RowKey,
                    TransactionDate = model.PaymentModeKey != DbConstants.PaymentMode.Cheque ? model.RefundDate : (model.ChequeClearanceDate ?? DateTimeUTC.Now),
                    ExtraUpdateKey = ExtraUpdateKey,
                    IsUpdate = IsUpdate,

                    BranchKey = model.PaidBranchKey != null ? model.PaidBranchKey : model.Application.BranchKey,
                    Purpose = EduSuiteUIResources.StudentFeeRefund + EduSuiteUIResources.BlankSpace + application.StudentName + " ( " + application.AdmissionNo + " ) " + model.Remarks,
                });
            }





            long oldBankAccountHeadKey = 0;

            if (model.PaymentModeKey == DbConstants.PaymentMode.Bank || model.PaymentModeKey == DbConstants.PaymentMode.Cheque)
            {
                accountHeadKey = dbContext.BankAccounts.Where(x => x.RowKey == model.BankAccountKey).Select(x => x.AccountHeadKey).FirstOrDefault();
            }
            else
            {
                accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.CashAccount).Select(x => x.RowKey).FirstOrDefault();
            }

            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                AccountHeadKey = accountHeadKey,
                Amount = model.RefundAmount ?? 0,
                TransactionTypeKey = DbConstants.TransactionType.FeeRefund,
                VoucherTypeKey = DbConstants.VoucherType.FeeRefund,
                TransactionDate = model.PaymentModeKey != DbConstants.PaymentMode.Cheque ? model.RefundDate : (model.ChequeClearanceDate ?? DateTimeUTC.Now),
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = model.PaidBranchKey != null ? model.PaidBranchKey : model.Application.BranchKey,
                TransactionKey = model.RowKey,
                Purpose = EduSuiteUIResources.StudentFeeRefund + EduSuiteUIResources.BlankSpace + application.StudentName + " ( " + application.AdmissionNo + " ) " + model.Remarks,

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
        private void FillPaidBranches(FeeRefundViewModel model)
        {
            model.PaidBranches = dbContext.Branches.Where(row => row.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BranchName
            }).ToList();
        }
        private void GenerateReceiptNo(FeeRefundViewModel model)
        {
            ConfigurationViewModel ConfigModel = new ConfigurationViewModel();
            ConfigModel.BranchKey = model.PaidBranchKey != null ? model.PaidBranchKey ?? 0 : model.BranchKey ?? 0;
            ConfigModel.ConfigType = DbConstants.PaymentReceiptConfigType.Refund;
            Configurations.GenerateReceipt(dbContext, ConfigModel);

            model.VoucherNo = ConfigModel.ReceiptNumber;
            model.SerialNumber = ConfigModel.SerialNumber;
        }
        public FeeRefundPrintViewModel ViewFeePrint(long Id)
        {
            try
            {
                FeeRefundPrintViewModel model = new FeeRefundPrintViewModel();
                model = dbContext.FeeRefundMasters.Where(row => row.RowKey == Id).Select(row => new FeeRefundPrintViewModel
                {
                    CompanyName = row.Application.Branch.Company.CompanyName,
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
                }).SingleOrDefault();
                model.FeeRefundViewModel = (from es in dbContext.FeeRefundMasters.Where(row => row.RowKey == Id)
                                            select new FeeRefundViewModel
                                            {
                                                RowKey = es.RowKey,
                                                RefundDate = es.RefundDate,
                                                VoucherNo = es.VoucherNo,
                                                PaymentModeKey = es.PaymentModeKey,
                                                PaymentModeName = es.PaymentMode.PaymentModeName,
                                                PaymentModeSubKey = es.PaymentModeSubKey,
                                                PaymentModeSubName = es.PaymentModeSub.PaymentModeSubName,
                                                BankAccountName = es.BankAccount.Bank.BankName + "-" + es.BankAccount.AccountNumber,
                                                CardNumber = es.CardNumber,
                                                ReferenceNumber = es.ReferenceNumber,
                                                ChequeOrDDNumber = es.ChequeOrDDNumber,
                                                ChequeClearanceDate = es.ChequeClearanceDate,
                                                Remarks = es.Remarks,
                                                RefundAmount = es.RefundAmount
                                            }).SingleOrDefault();
                if (model.FeeRefundViewModel == null)
                {
                    model.FeeRefundViewModel = new FeeRefundViewModel();
                }
                FillPaymentModes(model.FeeRefundViewModel);
                FillFeeRefundDetails(model.FeeRefundViewModel);

                return model;
            }
            catch (Exception ex)
            {
                return new FeeRefundPrintViewModel();
            }

        }
        private void FillFeeRefundDetails(FeeRefundViewModel model)
        {
            long Applicationkey = dbContext.FeeRefundMasters.Where(row => row.RowKey == model.RowKey).Select(x => x.ApplicationKey).SingleOrDefault();
            Application Application = dbContext.Applications.SingleOrDefault(row => row.RowKey == Applicationkey);

            model.FeeRefundDetails = dbContext.FeeRefundDetails.Where(row => row.FeeRefundMasterKey == model.RowKey)
                .Select(row => new FeeRefundDetailViewModel
                {
                    RowKey = row.RowKey,
                    FeeTypeKey = row.FeeTypeKey,
                    ReturnAmount = row.FeeAmount,
                    FeeYear = row.FeeYear,
                    FeeTypeName = row.FeeType.FeeTypeName,
                    RecieptNo = row.FeePaymentDetail.FeePaymentMaster.ReceiptNo,
                    IsFeeTypeYear = row.FeeType.FeeTypeModeKey == DbConstants.FeeTypeMode.Single ? true : false
                }).ToList();
            foreach (FeeRefundDetailViewModel PaymentDetails in model.FeeRefundDetails)
            {
                PaymentDetails.FeeYearText = PaymentDetails.FeeYear != null ? (CommonUtilities.GetYearDescriptionByCodeDetails(Application.Course.CourseDuration ?? 0, PaymentDetails.FeeYear ?? 0, Application.AcademicTermKey)) : "";

            }

        }

    }
}
