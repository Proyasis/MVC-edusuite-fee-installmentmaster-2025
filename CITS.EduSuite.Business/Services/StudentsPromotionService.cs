using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Data;
using CITS.EduSuite.Business.Models.ViewModels;
using System.Data.Entity.Infrastructure;
using CITS.EduSuite.Business.Models.Resources;


namespace CITS.EduSuite.Business.Services
{
    public class StudentsPromotionService : IStudentsPromotionService
    {
        private EduSuiteDatabase dbContext;

        public StudentsPromotionService(EduSuiteDatabase Objdb)
        {
            this.dbContext = Objdb;
        }
        public StudentsPromotionViewModel GetPromotionById(StudentsPromotionViewModel model)
        {
            try
            {
                StudentsPromotionViewModel objViewModel = new StudentsPromotionViewModel();
                objViewModel = dbContext.Promotions.Where(x => x.RowKey == model.RowKey).Select(row => new StudentsPromotionViewModel
                {
                    RowKey = row.RowKey,
                    IsUpdate = true,
                    BranchKey = row.BranchKey,
                    CourseTypeKey = row.ClassDetail.UniversityCourse.Course.CourseTypeKey,
                    BatchKey = row.BatchKey ?? 0,
                    CurrentClassDetailsKey = row.CurrentClassDetailsKey,
                    CurrentYear = row.CurrentYear,
                    ClassDetailsKey = row.CurrentClassDetailsKey ?? 0
                }).SingleOrDefault();

                if (objViewModel == null)
                {
                    objViewModel = new StudentsPromotionViewModel();
                }

                FillDropDown(objViewModel);
                model.IsSuccessful = true;
                return objViewModel;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.StudentsPromotion, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new StudentsPromotionViewModel();
            }
        }

        public StudentsPromotionViewModel FillPromotionDetailsViewModel(StudentsPromotionViewModel model)
        {
            if (model.BranchKey != 0 && model.ClassDetailsKey != 0 && model.BatchKey != 0)
            {
                var checkquery = dbContext.Promotions.Where(x => x.BranchKey == model.BranchKey && x.BatchKey == model.BatchKey && x.CurrentClassDetailsKey == model.ClassDetailsKey).Select(y => y.RowKey);
                if (checkquery.Any())
                {
                    model.RowKey = checkquery.FirstOrDefault();
                }
                else
                {
                    model.RowKey = 0;
                }
            }

            if (model.RowKey != 0 && model.RowKey != null)
            {
                model.StudentsPromotionDetails = (from A in dbContext.Applications
                                                  join P in dbContext.PromotionDetails.Where(x => x.PromotionMasterKey == model.RowKey) on A.RowKey equals P.ApplicationKey into PDA
                                                  from P in PDA.DefaultIfEmpty()
                                                  where (A.BatchKey == model.BatchKey && (P.RowKey != null ? model.ClassDetailsKey : A.ClassDetailsKey) == (model.ClassDetailsKey != 0 ? model.ClassDetailsKey : A.ClassDetailsKey) && A.BranchKey == model.BranchKey)
                                                  select new StudentsPromotionDetailsViewModel
                                                  {
                                                      RollNumber = A.RollNumber ?? 0,
                                                      RowKey = P.RowKey != null ? P.RowKey : 0,
                                                      Name = A.StudentName,
                                                      ApplicationKey = A.RowKey,
                                                      AdmissionNo = A.AdmissionNo,
                                                      //CurrentClassDetailsKey = P.CurrentClassDetailsKey ?? 0,                                                     
                                                      CurrentYear = P.Promotion.CurrentYear ?? A.CurrentYear,
                                                      PromotionStatusKey = P.PromotionStatusKey ?? 0,
                                                      PromotedClassDetailsKey = P.PromotedClassDetailsKey ?? 0,
                                                      PromotedYear = P.PromotedYear ?? 0,
                                                      ClassCode = P.ClassDetail.ClassCode != null ? P.ClassDetail.ClassCode : A.ClassDetail.ClassCode,
                                                      AcademicTermKey = A.AcademicTermKey,
                                                      CourseDuration = A.Course.CourseDuration//(A.Course.CourseDuration / A.AcademicTerm.Duration)
                                                  }).ToList();

                foreach (StudentsPromotionDetailsViewModel objmodel in model.StudentsPromotionDetails)
                {
                    objmodel.CurrentYearText = CommonUtilities.GetYearDescriptionByCodeDetails(objmodel.CourseDuration ?? 0, objmodel.CurrentYear ?? 0, objmodel.AcademicTermKey ?? 0);
                }
            }
            else
            {
                model.StudentsPromotionDetails = (from A in dbContext.Applications
                                                  where (A.StudentStatusKey == DbConstants.StudentStatus.Ongoing && A.BatchKey == model.BatchKey && A.ClassDetailsKey == (model.ClassDetailsKey != 0 ? model.ClassDetailsKey : A.ClassDetailsKey) && A.BranchKey == model.BranchKey)
                                                  select new StudentsPromotionDetailsViewModel
                                                  {
                                                      RollNumber = A.RollNumber ?? 0,
                                                      RowKey = 0,
                                                      Name = A.StudentName,
                                                      ApplicationKey = A.RowKey,
                                                      AdmissionNo = A.AdmissionNo,
                                                      //CurrentClassDetailsKey = A.CurrentClassDetailsKey ?? 0,                                                     
                                                      CurrentYear = A.CurrentYear,
                                                      PromotionStatusKey = 0,
                                                      PromotedClassDetailsKey = 0,
                                                      PromotedYear = 0,
                                                      ClassCode = A.ClassDetail.ClassCode,
                                                      AcademicTermKey = A.AcademicTermKey,
                                                      CourseDuration = A.Course.CourseDuration//(A.Course.CourseDuration / A.AcademicTerm.Duration)
                                                  }).ToList();

                foreach (StudentsPromotionDetailsViewModel objmodel in model.StudentsPromotionDetails)
                {
                    objmodel.CurrentYearText = CommonUtilities.GetYearDescriptionByCodeDetails(objmodel.CourseDuration ?? 0, objmodel.CurrentYear ?? 0, objmodel.AcademicTermKey ?? 0);
                }
            }
            if (model.ClassDetailsKey != 0)
            {
                ClassDetail classDetailList = dbContext.ClassDetails.SingleOrDefault(x => x.RowKey == model.ClassDetailsKey);
                model.CurrentClassDetailsKey = model.ClassDetailsKey;
                model.CurrentYear = classDetailList.StudentYear;
            }
            FillDropDown(model);
            return model;
        }

        public StudentsPromotionViewModel UpdatePromotion(StudentsPromotionViewModel model)
        {
            Promotion StudentPromotionmodel = new Promotion();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    StudentPromotionmodel = dbContext.Promotions.Where(p => p.RowKey == model.RowKey).SingleOrDefault();
                    StudentPromotionmodel.BranchKey = model.BranchKey;
                    StudentPromotionmodel.BatchKey = model.BatchKey;
                    StudentPromotionmodel.CurrentClassDetailsKey = model.ClassDetailsKey;
                    StudentPromotionmodel.CurrentYear = model.CurrentYear;
                    StudentPromotionmodel.IsActive = true;

                    CreatePromotionDetails(model.StudentsPromotionDetails.Where(x => x.RowKey == 0).ToList(), StudentPromotionmodel);
                    UpdatePromotiondetails(model.StudentsPromotionDetails.Where(x => x.RowKey != 0).ToList(), StudentPromotionmodel);


                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentsPromotion, (model.StudentsPromotionDetails.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Info, null, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.StudentsPromotion);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentsPromotion, (model.StudentsPromotionDetails.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public StudentsPromotionViewModel CreatePromotion(StudentsPromotionViewModel model)
        {
            Promotion StudentPromotionmodel = new Promotion();

           
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    long MaxKey = dbContext.Promotions.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    StudentPromotionmodel.RowKey = MaxKey + 1;
                    StudentPromotionmodel.BranchKey = model.BranchKey;
                    StudentPromotionmodel.BatchKey = model.BatchKey;
                    StudentPromotionmodel.CurrentClassDetailsKey = model.ClassDetailsKey;
                    StudentPromotionmodel.CurrentYear = model.CurrentYear;
                    StudentPromotionmodel.IsActive = true;
                    dbContext.Promotions.Add(StudentPromotionmodel);
                    CreatePromotionDetails(model.StudentsPromotionDetails.Where(x => x.RowKey == 0).ToList(), StudentPromotionmodel);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentsPromotion, (model.StudentsPromotionDetails.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Info, null, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.StudentsPromotion);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentsPromotion, (model.StudentsPromotionDetails.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        private void CreatePromotionDetails(List<StudentsPromotionDetailsViewModel> modelList, Promotion Promotionmodel)
        {
            long MaxKey = dbContext.PromotionDetails.Select(p => p.RowKey).DefaultIfEmpty().Max();
            long DivisionAllocationMaxKey = dbContext.StudentDivisionAllocations.Select(p => p.RowKey).DefaultIfEmpty().Max();
            int RollNumber = 0;
            string PurposeYear = "";
            foreach (StudentsPromotionDetailsViewModel modelDetails in modelList)
            {
                if (modelDetails.PromotionStatusKey != null)
                {
                    PromotionDetail StudentPromotionDetsilmodel = new PromotionDetail();
                    StudentDivisionAllocation DivisionAllocationNew = new StudentDivisionAllocation();
                    Application applicationDetails = dbContext.Applications.SingleOrDefault(x => x.RowKey == modelDetails.ApplicationKey);

                    // Add to Promotion Detsil
                    StudentPromotionDetsilmodel.PromotedClassDetailsKey = modelDetails.PromotedClassDetailsKey;
                    StudentPromotionDetsilmodel.PromotedYear = modelDetails.PromotedYear;
                    StudentPromotionDetsilmodel.RowKey = MaxKey + 1;
                    StudentPromotionDetsilmodel.ApplicationKey = modelDetails.ApplicationKey;
                    StudentPromotionDetsilmodel.PromotionMasterKey = Promotionmodel.RowKey;
                    StudentPromotionDetsilmodel.PromotionStatusKey = modelDetails.PromotionStatusKey;

                    if (modelDetails.PromotionStatusKey == DbConstants.PromotionStatus.Promoted)
                    {
                        RollNumber = dbContext.StudentDivisionAllocations.Where(row => //row.Application.CourseKey == model.CourseKey &&
                          row.Application.BatchKey == Promotionmodel.BatchKey &&
                          row.Application.BranchKey == Promotionmodel.BranchKey &&
                          row.Application.StudentStatusKey == DbConstants.ApplicationStatus.OnGoing && row.ClassDetailsKey == modelDetails.PromotedClassDetailsKey &&
                          row.IsActive == true).Select(p => p.RollNumber).DefaultIfEmpty().Max();

                        if (applicationDetails != null)
                        {
                            //applicationDetails.DivisionKey = modelDetails.PromotedDivisionKey;
                            applicationDetails.RollNumber = RollNumber + 1;
                            applicationDetails.CurrentYear = modelDetails.PromotedYear ?? 0;
                            applicationDetails.StudentStatusKey = DbConstants.StudentStatus.Ongoing;
                            applicationDetails.ClassDetailsKey = modelDetails.PromotedClassDetailsKey;
                        }
                        //StudentDivisionAllocation DivisionAllocation = dbContext.StudentDivisionAllocations.SingleOrDefault(x => x.ApplicationKey == modelDetails.ApplicationKey && x.IsActive == true && x.ClassDetailsKey == Promotionmodel.CurrentClassDetailsKey);
                        //if (DivisionAllocation != null)
                        //{
                        //    DivisionAllocation.IsActive = false;
                        //    dbContext.SaveChanges();
                        //}

                        List<StudentDivisionAllocation> DivisionAllocationList = dbContext.StudentDivisionAllocations.Where(x => x.ApplicationKey == modelDetails.ApplicationKey && x.IsActive == true && x.ClassDetailsKey == Promotionmodel.CurrentClassDetailsKey).ToList();
                        foreach (StudentDivisionAllocation item in DivisionAllocationList)
                            if (item.IsActive)
                            {
                                item.IsActive = false;
                                dbContext.SaveChanges();
                            }

                        // Add in T_StudentDivisionAllocation
                        DivisionAllocationNew.RowKey = DivisionAllocationMaxKey + 1;
                        DivisionAllocationNew.ApplicationKey = modelDetails.ApplicationKey;
                        //DivisionAllocationNew.StudentYear = modelDetails.PromotedYear ?? 0;
                        DivisionAllocationNew.ClassDetailsKey = modelDetails.PromotedClassDetailsKey ?? 0;
                        DivisionAllocationNew.RollNumber = RollNumber + 1;
                        DivisionAllocationNew.IsActive = true;

                        dbContext.StudentDivisionAllocations.Add(DivisionAllocationNew);
                        DivisionAllocationMaxKey++;
                        RollNumber++;
                        dbContext.SaveChanges();


                        List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();

                        var Application = dbContext.Applications.Where(x => x.RowKey == modelDetails.ApplicationKey).SingleOrDefault();

                        if (DbConstants.GeneralConfiguration.AllowAdmissionToAccoount)
                        {

                            //decimal CenterShareAmountPer = dbContext.UniversityCourseFees.Where(x => x.FeeYear == modelDetails.PromotedYear && x.UniversityCourse.AcademicTermKey == Application.AcademicTermKey && x.UniversityCourse.CourseKey == Application.CourseKey && x.UniversityCourse.UniversityMasterKey == Application.UniversityMasterKey).Select(x => x.CenterShareAmountPer ?? 0).FirstOrDefault();
                            List<UniversityCourseFee> UniversityCourseFeeList = dbContext.UniversityCourseFees.Where(x => x.FeeYear == modelDetails.PromotedYear && x.UniversityCourse.AcademicTermKey == Application.AcademicTermKey && x.UniversityCourse.CourseKey == Application.CourseKey && x.UniversityCourse.UniversityMasterKey == Application.UniversityMasterKey).ToList();
                            decimal TotalReceivable = Application.AdmissionFees.Where(row => (row.AdmissionFeeYear ?? 0) == modelDetails.PromotedYear).Select(row => row.ActualAmount ?? 0).Sum();
                            decimal TotalPayable = Application.AdmissionFees.Where(row => (row.AdmissionFeeYear ?? 0) == modelDetails.PromotedYear).Select(row => (row.ActualAmount ?? 0) - ((row.FeeType.IsUniverisity ? (UniversityCourseFeeList.Where(x => x.FeeTypeKey == row.FeeTypeKey).Select(y => y.CenterShareAmountPer).FirstOrDefault() ?? 0) : 100) * (row.ActualAmount ?? 0)) / 100).Sum();
                            decimal TotalConcessionExpense = Application.AdmissionFees.Where(row => (row.AdmissionFeeYear ?? 0) == modelDetails.PromotedYear).Select(row => (row.ConcessionAmount ?? 0)).Sum();

                            if (DbConstants.GeneralConfiguration.AllowSplitCostOfService)
                            {
                                IncomeSplitAmountList(Application.AdmissionFees.Where(x => x.AdmissionFeeYear == modelDetails.PromotedYear).ToList(), accountFlowModelList, false, Application, StudentPromotionDetsilmodel.RowKey);
                            }
                            else
                            {
                                decimal TotalIncome = Application.AdmissionFees.Where(row => (row.AdmissionFeeYear ?? 0) == modelDetails.PromotedYear).Select(row => ((row.FeeType.IsUniverisity ? (UniversityCourseFeeList.Where(x => x.FeeTypeKey == row.FeeTypeKey).Select(y => y.CenterShareAmountPer).FirstOrDefault() ?? 0) : 100) * (row.ActualAmount ?? 0)) / 100).Sum();

                                IncomeAmountList(TotalIncome, accountFlowModelList, false, Application, StudentPromotionDetsilmodel.RowKey);
                            }
                            //decimal TotalSGSTAmount = Application.AdmissionFees.Where(row => (row.AdmissionFeeYear ?? 0) == modelDetails.PromotedYear).Select(row => (row.SGSTRate ?? 0)).Sum();
                            //decimal TotalCGSTAmount = Application.AdmissionFees.Where(row => (row.AdmissionFeeYear ?? 0) == modelDetails.PromotedYear).Select(row => (row.CGSTRate ?? 0)).Sum();


                            RecievableAmountList(TotalReceivable, accountFlowModelList, false, Application, StudentPromotionDetsilmodel.RowKey);
                            PayableAmountList(TotalPayable, accountFlowModelList, false, Application, StudentPromotionDetsilmodel.RowKey);


                            //CGSTAmountList(TotalCGSTAmount, accountFlowModelList, false, Application);
                            //SGSTAmountList(TotalSGSTAmount, accountFlowModelList, false, Application);
                            ConcessionAmountList(TotalConcessionExpense, accountFlowModelList, false, Application, StudentPromotionDetsilmodel.RowKey);

                        }
                        else
                        {
                            ExistingSplitAmountList(dbContext.FeePaymentDetails.Where(x => x.FeePaymentMaster.ApplicationKey == modelDetails.ApplicationKey && x.FeeYear == modelDetails.PromotedYear).ToList(), accountFlowModelList, false, Application, StudentPromotionDetsilmodel.RowKey);

                        }
                        if (Application.EducationTypeKey == DbConstants.EducationType.RegulerEducation)
                        {
                            decimal TotalAdavancePayable = dbContext.FeePaymentDetails.Where(x => x.FeePaymentMaster.ApplicationKey == modelDetails.ApplicationKey && x.FeeYear == modelDetails.PromotedYear).Select(x => x.TotalAmount ?? 0).DefaultIfEmpty().Sum();
                            AdvancePayableAmountList(TotalAdavancePayable, accountFlowModelList, false, Application, StudentPromotionDetsilmodel.RowKey);
                        }
                        if (accountFlowModelList.Count > 0)
                        {
                            CreateAccountFlow(accountFlowModelList, false);
                        }
                    }
                    else
                    {
                        if (modelDetails.PromotionStatusKey == DbConstants.PromotionStatus.Completed)
                        {
                            if (applicationDetails != null)
                            {
                                applicationDetails.StudentStatusKey = DbConstants.StudentStatus.Completed;
                                applicationDetails.CurrentYear = Promotionmodel.CurrentYear ?? 0;
                            }
                            StudentPromotionDetsilmodel.PromotedYear = null;
                            StudentPromotionDetsilmodel.PromotedClassDetailsKey = null;
                        }
                        else
                        {
                            if (applicationDetails != null)
                            {
                                applicationDetails.StudentStatusKey = DbConstants.StudentStatus.Droped;
                                applicationDetails.CurrentYear = Promotionmodel.CurrentYear ?? 0;
                            }
                            StudentPromotionDetsilmodel.PromotedClassDetailsKey = null;
                            StudentPromotionDetsilmodel.PromotedYear = null;
                        }

                    }
                    //StudentPromotionmodel.RowKey = MaxKey + 1;
                    //StudentPromotionmodel.ApplicationKey = modelDetails.ApplicationKey;
                    //StudentPromotionmodel.CurrentClassDetailsKey = modelDetails.CurrentClassDetailsKey;
                    //StudentPromotionmodel.CurrentYear = modelDetails.CurrentYear;
                    //StudentPromotionmodel.PromotionStatusKey = modelDetails.PromotionStatusKey;
                    //StudentPromotionmodel.IsActive = true;
                    dbContext.PromotionDetails.Add(StudentPromotionDetsilmodel);
                    MaxKey++;
                }
            }
        }

        private void UpdatePromotiondetails(List<StudentsPromotionDetailsViewModel> modelList, Promotion Promotionmodel)
        {
            long DivisionAllocationMaxKey = dbContext.StudentDivisionAllocations.Select(p => p.RowKey).DefaultIfEmpty().Max();
            int RollNumber = 0;

            foreach (StudentsPromotionDetailsViewModel modelDetails in modelList)
            {

                if (modelDetails.PromotionStatusKey != null)
                {
                    RollNumber = dbContext.StudentDivisionAllocations.Where(row =>// row.Application.CourseKey == model.CourseKey &&
                           row.Application.BatchKey == Promotionmodel.BatchKey &&
                           row.Application.BranchKey == Promotionmodel.BranchKey &&
                           row.Application.StudentStatusKey == DbConstants.ApplicationStatus.OnGoing && row.ClassDetailsKey == modelDetails.PromotedClassDetailsKey &&
                           row.IsActive == true).Select(p => p.RollNumber).DefaultIfEmpty().Max();

                    PromotionDetail StudentPromotionDetailmodel = dbContext.PromotionDetails.SingleOrDefault(p => p.RowKey == modelDetails.RowKey);
                    Application applicationDetails = dbContext.Applications.SingleOrDefault(x => x.RowKey == modelDetails.ApplicationKey);

                    if (modelDetails.PromotionStatusKey == DbConstants.PromotionStatus.Promoted)
                    {
                        if (applicationDetails != null)
                        {
                            // applicationDetails.DivisionKey = modelDetails.PromotedDivisionKey;
                            applicationDetails.RollNumber = RollNumber + 1;
                            applicationDetails.CurrentYear = modelDetails.PromotedYear ?? 0;
                            applicationDetails.StudentStatusKey = DbConstants.StudentStatus.Ongoing;
                            applicationDetails.ClassDetailsKey = modelDetails.PromotedClassDetailsKey;

                        }
                        //StudentDivisionAllocation DivisionAllocation = dbContext.StudentDivisionAllocations.SingleOrDefault(x => x.ApplicationKey == modelDetails.ApplicationKey && x.IsActive == true && x.ClassDetailsKey == Promotionmodel.CurrentClassDetailsKey);
                        //if (DivisionAllocation != null)
                        //{
                        //    DivisionAllocation.IsActive = false;
                        //    dbContext.SaveChanges();
                        //}
                        List<StudentDivisionAllocation> DivisionAllocationList = dbContext.StudentDivisionAllocations.Where(x => x.ApplicationKey == modelDetails.ApplicationKey && x.IsActive == true && x.ClassDetailsKey == Promotionmodel.CurrentClassDetailsKey).ToList();
                        foreach (StudentDivisionAllocation item in DivisionAllocationList)
                            if (item.IsActive)
                            {
                                item.IsActive = false;
                                dbContext.SaveChanges();
                            }

                        // Add to Promotion
                        StudentPromotionDetailmodel.PromotedClassDetailsKey = modelDetails.PromotedClassDetailsKey;
                        StudentPromotionDetailmodel.PromotedYear = modelDetails.PromotedYear;
                        if (StudentPromotionDetailmodel.PromotionStatusKey != DbConstants.PromotionStatus.Promoted)
                        {
                            List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();

                            var Application = dbContext.Applications.Where(x => x.RowKey == modelDetails.ApplicationKey).SingleOrDefault();

                            if (DbConstants.GeneralConfiguration.AllowAdmissionToAccoount)
                            {

                                //decimal CenterShareAmountPer = dbContext.UniversityCourseFees.Where(x => x.FeeYear == modelDetails.PromotedYear && x.UniversityCourse.AcademicTermKey == Application.AcademicTermKey && x.UniversityCourse.CourseKey == Application.CourseKey && x.UniversityCourse.UniversityMasterKey == Application.UniversityMasterKey).Select(x => x.CenterShareAmountPer ?? 0).FirstOrDefault();
                                List<UniversityCourseFee> UniversityCourseFeeList = dbContext.UniversityCourseFees.Where(x => x.FeeYear == modelDetails.PromotedYear && x.UniversityCourse.AcademicTermKey == Application.AcademicTermKey && x.UniversityCourse.CourseKey == Application.CourseKey && x.UniversityCourse.UniversityMasterKey == Application.UniversityMasterKey).ToList();

                                decimal TotalReceivable = Application.AdmissionFees.Where(row => (row.AdmissionFeeYear ?? 0) == modelDetails.PromotedYear).Select(row => row.ActualAmount ?? 0).Sum();
                                decimal TotalPayable = Application.AdmissionFees.Where(row => (row.AdmissionFeeYear ?? 0) == modelDetails.PromotedYear).Select(row => (row.ActualAmount ?? 0) - ((row.FeeType.IsUniverisity ? (UniversityCourseFeeList.Where(x => x.FeeTypeKey == row.FeeTypeKey).Select(y => y.CenterShareAmountPer).FirstOrDefault() ?? 0) : 100) * (row.ActualAmount ?? 0)) / 100).Sum();
                                decimal TotalIncome = Application.AdmissionFees.Where(row => (row.AdmissionFeeYear ?? 0) == modelDetails.PromotedYear).Select(row => ((row.FeeType.IsUniverisity ? (UniversityCourseFeeList.Where(x => x.FeeTypeKey == row.FeeTypeKey).Select(y => y.CenterShareAmountPer).FirstOrDefault() ?? 0) : 100) * (row.ActualAmount ?? 0)) / 100).Sum();
                                decimal TotalConcessionExpense = Application.AdmissionFees.Where(row => (row.AdmissionFeeYear ?? 0) == modelDetails.PromotedYear).Select(row => (row.ConcessionAmount ?? 0)).Sum();
                                //decimal TotalSGSTAmount = Application.AdmissionFees.Where(row => (row.AdmissionFeeYear ?? 0) == modelDetails.PromotedYear).Select(row => (row.SGSTRate ?? 0)).Sum();
                                //decimal TotalCGSTAmount = Application.AdmissionFees.Where(row => (row.AdmissionFeeYear ?? 0) == modelDetails.PromotedYear).Select(row => (row.CGSTRate ?? 0)).Sum();


                                if (DbConstants.GeneralConfiguration.AllowSplitCostOfService)
                                {
                                    IncomeSplitAmountList(Application.AdmissionFees.Where(x => x.AdmissionFeeYear == modelDetails.PromotedYear).ToList(), accountFlowModelList, false, Application, StudentPromotionDetailmodel.RowKey);
                                }
                                else
                                {
                                    decimal TotalIncomes = Application.AdmissionFees.Where(row => (row.AdmissionFeeYear ?? 0) == modelDetails.PromotedYear).Select(row => ((row.FeeType.IsUniverisity ? (UniversityCourseFeeList.Where(x => x.FeeTypeKey == row.FeeTypeKey).Select(y => y.CenterShareAmountPer).FirstOrDefault() ?? 0) : 100) * (row.ActualAmount ?? 0)) / 100).Sum();

                                    IncomeAmountList(TotalIncomes, accountFlowModelList, false, Application, StudentPromotionDetailmodel.RowKey);
                                }

                                RecievableAmountList(TotalReceivable, accountFlowModelList, false, Application, StudentPromotionDetailmodel.RowKey);
                                PayableAmountList(TotalPayable, accountFlowModelList, false, Application, StudentPromotionDetailmodel.RowKey);
                                IncomeAmountList(TotalIncome, accountFlowModelList, false, Application, StudentPromotionDetailmodel.RowKey);
                                //CGSTAmountList(TotalCGSTAmount, accountFlowModelList, false, Application);
                                //SGSTAmountList(TotalSGSTAmount, accountFlowModelList, false, Application);
                                ConcessionAmountList(TotalConcessionExpense, accountFlowModelList, false, Application, StudentPromotionDetailmodel.RowKey);

                                // CreateAccountFlow(accountFlowModelList, false);
                            }
                            else
                            {
                                ExistingSplitAmountList(dbContext.FeePaymentDetails.Where(x => x.FeePaymentMaster.ApplicationKey == modelDetails.ApplicationKey && x.FeeYear == modelDetails.PromotedYear).ToList(), accountFlowModelList, false, Application, StudentPromotionDetailmodel.RowKey);
                                //CreateAccountFlow(accountFlowModelList, false);
                            }
                            if (Application.EducationTypeKey == DbConstants.EducationType.RegulerEducation)
                            {
                                decimal TotalAdavancePayable = dbContext.FeePaymentDetails.Where(x => x.FeePaymentMaster.ApplicationKey == modelDetails.ApplicationKey && x.FeeYear == modelDetails.PromotedYear).Select(x => x.TotalAmount ?? 0).DefaultIfEmpty().Sum();
                                AdvancePayableAmountList(TotalAdavancePayable, accountFlowModelList, false, Application, StudentPromotionDetailmodel.RowKey);
                            }

                            if (accountFlowModelList.Count > 0)
                            {
                                CreateAccountFlow(accountFlowModelList, false);
                            }

                            StudentDivisionAllocation DivisionAllocationNew = new StudentDivisionAllocation();
                            // Add in T_StudentDivisionAllocation
                            DivisionAllocationNew.RowKey = DivisionAllocationMaxKey + 1;
                            DivisionAllocationNew.ApplicationKey = modelDetails.ApplicationKey;
                            //DivisionAllocationNew.StudentYear = modelDetails.PromotedYear ?? 0;
                            DivisionAllocationNew.ClassDetailsKey = modelDetails.PromotedClassDetailsKey ?? 0;
                            DivisionAllocationNew.RollNumber = RollNumber + 1;
                            DivisionAllocationNew.IsActive = true;

                            dbContext.StudentDivisionAllocations.Add(DivisionAllocationNew);
                            DivisionAllocationMaxKey++;
                            RollNumber++;
                        }
                    }
                    else
                    {
                        StudentDivisionAllocation CurrentdivisionAllocation = dbContext.StudentDivisionAllocations.SingleOrDefault(x => x.ApplicationKey == StudentPromotionDetailmodel.ApplicationKey && x.ClassDetailsKey == Promotionmodel.CurrentClassDetailsKey);

                        if (StudentPromotionDetailmodel.PromotionStatusKey == DbConstants.PromotionStatus.Promoted)
                        {
                            AccountFlowViewModel accountFlowModel = new AccountFlowViewModel();
                            accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.Promotion;
                            accountFlowModel.TransactionKey = StudentPromotionDetailmodel.RowKey;
                            accountFlowModel.IsDelete = false;
                            AccountFlowService accountFlowService = new AccountFlowService(dbContext);
                            accountFlowService.DeleteAccountFlow(accountFlowModel);

                            StudentDivisionAllocation PromoteddivisionAllocation = dbContext.StudentDivisionAllocations.SingleOrDefault(x => x.ApplicationKey == StudentPromotionDetailmodel.ApplicationKey && x.ClassDetailsKey == StudentPromotionDetailmodel.PromotedClassDetailsKey);
                            CurrentdivisionAllocation.IsActive = true;
                            if (PromoteddivisionAllocation != null)
                                dbContext.StudentDivisionAllocations.Remove(PromoteddivisionAllocation);
                        }
                        else if (modelDetails.PromotionStatusKey == DbConstants.PromotionStatus.Completed)
                        {
                            if (applicationDetails != null)
                            {
                                applicationDetails.RollNumber = CurrentdivisionAllocation.RollNumber;
                                applicationDetails.StudentStatusKey = DbConstants.StudentStatus.Completed;
                                applicationDetails.CurrentYear = Promotionmodel.CurrentYear ?? 0;
                                applicationDetails.ClassDetailsKey = Promotionmodel.CurrentClassDetailsKey;

                            }
                            //StudentPromotionmodel.PromotedYear = 0;
                            //StudentPromotionmodel.PromotedClassDetailsKey = 0;
                        }
                        else
                        {
                            if (applicationDetails != null)
                            {
                                applicationDetails.RollNumber = CurrentdivisionAllocation.RollNumber;
                                applicationDetails.StudentStatusKey = DbConstants.StudentStatus.Droped;
                                applicationDetails.CurrentYear = Promotionmodel.CurrentYear ?? 0;
                                applicationDetails.ClassDetailsKey = Promotionmodel.CurrentClassDetailsKey;
                            }
                            //StudentPromotionmodel.PromotedClassDetailsKey = 0;
                            //StudentPromotionmodel.PromotedYear = 0;
                        }
                    }
                    StudentPromotionDetailmodel.ApplicationKey = modelDetails.ApplicationKey;
                    StudentPromotionDetailmodel.PromotionStatusKey = modelDetails.PromotionStatusKey;
                    //dbContext.SaveChanges();
                }
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
        private void PayableAmountList(decimal Amount, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, Application ApplicationModel, long PromotionKey)
        {
            long ExtraUpdateKey = 0;
            //long accountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.AccountsPayable).Select(x => x.RowKey).FirstOrDefault();
            long accountHeadKey = dbContext.UniversityMasters.Where(x => x.RowKey == ApplicationModel.UniversityMasterKey).Select(x => x.AccountHeadKey ?? 0).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                AccountHeadKey = accountHeadKey,
                Amount = Amount,
                TransactionTypeKey = DbConstants.TransactionType.Promotion,
                TransactionDate = DateTimeUTC.Now,
                TransactionKey = PromotionKey,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                VoucherTypeKey = DbConstants.VoucherType.Promotion,
                BranchKey = ApplicationModel.BranchKey,
                Purpose = EduSuiteUIResources.Promotion + EduSuiteUIResources.BlankSpace + ApplicationModel.StudentName + " ( " + ApplicationModel.AdmissionNo + " ) ",
            });
        }
        private void RecievableAmountList(decimal Amount, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, Application ApplicationModel, long PromotionKey)
        {
            long ExtraUpdateKey = 0;
            long accountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.AccountsReceivable).Select(x => x.RowKey).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.In,
                AccountHeadKey = accountHeadKey,
                Amount = Amount,
                TransactionTypeKey = DbConstants.TransactionType.Promotion,
                TransactionDate = DateTimeUTC.Now,
                TransactionKey = PromotionKey,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                VoucherTypeKey = DbConstants.VoucherType.Promotion,
                BranchKey = ApplicationModel.BranchKey,
                Purpose = EduSuiteUIResources.Promotion + EduSuiteUIResources.BlankSpace + ApplicationModel.StudentName + " ( " + ApplicationModel.AdmissionNo + " ) ",
            });

        }
        private void IncomeAmountList(decimal Amount, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, Application ApplicationModel, long PromotionKey)
        {
            long accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.CostOfService).Select(x => x.RowKey).FirstOrDefault();
            long ExtraUpdateKey = 0;
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                AccountHeadKey = accountHeadKey,
                Amount = Amount,
                TransactionTypeKey = DbConstants.TransactionType.Promotion,
                TransactionDate = DateTimeUTC.Now,
                TransactionKey = PromotionKey,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                VoucherTypeKey = DbConstants.VoucherType.Promotion,
                BranchKey = ApplicationModel.BranchKey,
                Purpose = EduSuiteUIResources.Promotion + EduSuiteUIResources.BlankSpace + ApplicationModel.StudentName + " ( " + ApplicationModel.AdmissionNo + " ) ",
            });

        }
        private void CGSTAmountList(decimal Amount, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, Application ApplicationModel, long PromotionKey)
        {
            long ExtraUpdateKey = 0;
            long accountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.OutputTaxCGST).Select(x => x.RowKey).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                AccountHeadKey = accountHeadKey,
                Amount = Amount,
                TransactionTypeKey = DbConstants.TransactionType.Promotion,
                TransactionDate = DateTimeUTC.Now,
                TransactionKey = PromotionKey,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                VoucherTypeKey = DbConstants.VoucherType.Promotion,
                BranchKey = ApplicationModel.BranchKey,
                Purpose = EduSuiteUIResources.Promotion + EduSuiteUIResources.BlankSpace + ApplicationModel.StudentName + " ( " + ApplicationModel.AdmissionNo + " ) ",
            });
        }
        private void SGSTAmountList(decimal Amount, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, Application ApplicationModel, long PromotionKey)
        {
            long ExtraUpdateKey = 0;
            long accountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.OutputTaxSGST).Select(x => x.RowKey).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                AccountHeadKey = accountHeadKey,
                Amount = Amount,
                TransactionTypeKey = DbConstants.TransactionType.Promotion,
                TransactionDate = DateTimeUTC.Now,
                TransactionKey = PromotionKey,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = ApplicationModel.BranchKey,
                VoucherTypeKey = DbConstants.VoucherType.Promotion,
                Purpose = EduSuiteUIResources.Promotion + EduSuiteUIResources.BlankSpace + ApplicationModel.StudentName + " ( " + ApplicationModel.AdmissionNo + " ) ",
            });


        }
        private void ConcessionAmountList(decimal Amount, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, Application ApplicationModel, long PromotionKey)
        {
            long ExtraUpdateKey = 0;
            long accountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.DiscountTaken).Select(x => x.RowKey).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.In,
                AccountHeadKey = accountHeadKey,
                Amount = Amount,
                TransactionTypeKey = DbConstants.TransactionType.Promotion,
                TransactionDate = DateTimeUTC.Now,
                TransactionKey = PromotionKey,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = ApplicationModel.BranchKey,
                VoucherTypeKey = DbConstants.VoucherType.Promotion,
                Purpose = EduSuiteUIResources.Promotion + EduSuiteUIResources.BlankSpace + ApplicationModel.StudentName + " ( " + ApplicationModel.AdmissionNo + " ) ",
            });


        }


        private void IncomeSplitAmountList(List<AdmissionFee> modelList, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, Application ApplicationModel, long PromotionKey)
        {
            long ExtraUpdateKey = 0;
            foreach (AdmissionFee item in modelList)
            {
                long AccountHeadKey = dbContext.FeeTypes.Where(x => x.RowKey == item.FeeTypeKey).Select(x => x.AccountHeadKey ?? 0).FirstOrDefault();

                decimal CenterShareAmountPer = dbContext.UniversityCourseFees.Where(x => x.FeeYear == item.AdmissionFeeYear && x.FeeTypeKey == item.FeeTypeKey && x.UniversityCourse.AcademicTermKey == ApplicationModel.AcademicTermKey && x.UniversityCourse.CourseKey == ApplicationModel.CourseKey && x.UniversityCourse.UniversityMasterKey == ApplicationModel.UniversityMasterKey).Select(x => x.CenterShareAmountPer ?? 0).FirstOrDefault();
                FeeType feetype = dbContext.FeeTypes.Where(x => x.RowKey == item.FeeTypeKey).FirstOrDefault();
                accountFlowModelList.Add(new AccountFlowViewModel
                {
                    CashFlowTypeKey = DbConstants.CashFlowType.Out,
                    AccountHeadKey = AccountHeadKey,
                    Amount = (feetype.IsUniverisity ? (CenterShareAmountPer) : 100) * ((item.ActualAmount ?? 0) - (item.OldPaid ?? 0)) / 100,
                    TransactionTypeKey = DbConstants.TransactionType.Promotion,
                    TransactionDate = DateTimeUTC.Now,
                    TransactionKey = PromotionKey,
                    ExtraUpdateKey = ExtraUpdateKey,
                    IsUpdate = IsUpdate,
                    VoucherTypeKey = DbConstants.VoucherType.Promotion,
                    BranchKey = ApplicationModel.BranchKey,
                    Purpose = EduSuiteUIResources.Promotion + EduSuiteUIResources.BlankSpace + ApplicationModel.StudentName + " ( " + ApplicationModel.AdmissionNo + " ) ",
                });
            }
        }


        private void ExistingSplitAmountList(List<FeePaymentDetail> modelList, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, Application ApplicationModel, long PromotionKey)
        {
            long ExtraUpdateKey = 0;
            var ApplicationDetails = dbContext.Applications.SingleOrDefault(x => x.RowKey == ApplicationModel.RowKey);
            decimal TotalIncome = modelList.Select(x => x.TotalAmount ?? 0).DefaultIfEmpty().Sum();

            if (DbConstants.GeneralConfiguration.AllowSplitCostOfService)
            {

                foreach (FeePaymentDetail item in modelList)
                {
                    long AccountHeadKey = dbContext.FeeTypes.Where(x => x.RowKey == item.FeeTypeKey).Select(x => x.AccountHeadKey ?? 0).FirstOrDefault();
                    accountFlowModelList.Add(new AccountFlowViewModel
                    {
                        CashFlowTypeKey = DbConstants.CashFlowType.Out,
                        AccountHeadKey = AccountHeadKey,
                        Amount = item.TaxableAmount ?? 0,
                        TransactionTypeKey = DbConstants.TransactionType.Promotion,
                        TransactionDate = DateTimeUTC.Now,
                        TransactionKey = PromotionKey,
                        ExtraUpdateKey = ExtraUpdateKey,
                        IsUpdate = IsUpdate,
                        VoucherTypeKey = DbConstants.VoucherType.Promotion,
                        BranchKey = ApplicationDetails.BranchKey,
                        Purpose = EduSuiteUIResources.Promotion + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.RecievedFrom + EduSuiteUIResources.BlankSpace + ApplicationDetails.StudentName + " ( " + ApplicationDetails.AdmissionNo + " ) ",

                    });
                }
            }
            else
            {

                long AccountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.CostOfService).Select(x => x.RowKey).FirstOrDefault();

                accountFlowModelList.Add(new AccountFlowViewModel
                {
                    CashFlowTypeKey = DbConstants.CashFlowType.Out,
                    AccountHeadKey = AccountHeadKey,
                    Amount = TotalIncome,
                    TransactionTypeKey = DbConstants.TransactionType.Promotion,
                    TransactionDate = DateTimeUTC.Now,
                    TransactionKey = PromotionKey,
                    ExtraUpdateKey = ExtraUpdateKey,
                    IsUpdate = IsUpdate,
                    VoucherTypeKey = DbConstants.VoucherType.Promotion,
                    BranchKey = ApplicationDetails.BranchKey,
                    Purpose = EduSuiteUIResources.Promotion + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.RecievedFrom + EduSuiteUIResources.BlankSpace + ApplicationDetails.StudentName + " ( " + ApplicationDetails.AdmissionNo + " ) ",

                });
            }


            long accountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.AdvancePayable && x.IsActive == true).Select(x => x.RowKey).FirstOrDefault();

            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.In,
                AccountHeadKey = accountHeadKey,
                Amount = TotalIncome,
                TransactionTypeKey = DbConstants.TransactionType.Promotion,
                TransactionKey = PromotionKey,
                TransactionDate = DateTimeUTC.Now,
                VoucherTypeKey = DbConstants.VoucherType.Promotion,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = ApplicationDetails.BranchKey,
                Purpose = EduSuiteUIResources.Promotion + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.RecievedFrom + EduSuiteUIResources.BlankSpace + ApplicationDetails.StudentName + " ( " + ApplicationDetails.AdmissionNo + " ) ",
            });
        }

        private void AdvancePayableAmountList(decimal Amount, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, Application ApplicationModel, long PromotionKey)
        {
            long ExtraUpdateKey = 0;
            long accountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.AdvancePayable && x.IsActive == true).Select(x => x.RowKey).FirstOrDefault();

            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.In,
                AccountHeadKey = accountHeadKey,
                Amount = Amount,
                TransactionTypeKey = DbConstants.TransactionType.Promotion,
                TransactionKey = PromotionKey,
                TransactionDate = DateTimeUTC.Now,
                VoucherTypeKey = DbConstants.VoucherType.Promotion,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = ApplicationModel.BranchKey,
                Purpose = EduSuiteUIResources.Promotion + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.RecievedFrom + EduSuiteUIResources.BlankSpace + ApplicationModel.StudentName + " ( " + ApplicationModel.AdmissionNo + " ) ",
            });
        }
        #endregion

        public StudentsPromotionViewModel DeletePromotion(long Id)
        {
            StudentsPromotionViewModel model = new StudentsPromotionViewModel();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Promotion promotion = dbContext.Promotions.Where(x => x.RowKey == Id).SingleOrDefault();
                    List<PromotionDetail> PromotionDetailsList = dbContext.PromotionDetails.Where(x => x.PromotionMasterKey == Id).ToList();
                    if (PromotionDetailsList.Count > 0)
                    {

                        foreach (PromotionDetail objmodel in PromotionDetailsList)
                        {
                            PromotionDetail promotionDetail = dbContext.PromotionDetails.SingleOrDefault(x => x.RowKey == objmodel.RowKey);
                            Attendance attendanceDetails = dbContext.Attendances.SingleOrDefault(x => x.ApplicationKey == promotionDetail.ApplicationKey && x.ClassDetailsKey == promotionDetail.PromotedClassDetailsKey);

                            if (promotionDetail.PromotionStatusKey == DbConstants.StudentStatus.Completed || attendanceDetails == null)
                            {
                                StudentDivisionAllocation CurrentdivisionAllocation = dbContext.StudentDivisionAllocations.SingleOrDefault(x => x.ApplicationKey == promotionDetail.ApplicationKey && x.ClassDetailsKey == promotionDetail.Promotion.CurrentClassDetailsKey);
                                StudentDivisionAllocation PromoteddivisionAllocation = dbContext.StudentDivisionAllocations.SingleOrDefault(x => x.ApplicationKey == promotionDetail.ApplicationKey && x.ClassDetailsKey == promotionDetail.PromotedClassDetailsKey);
                                Application applicationDetails = dbContext.Applications.SingleOrDefault(x => x.RowKey == promotionDetail.ApplicationKey);

                                if (applicationDetails != null && CurrentdivisionAllocation != null)
                                {
                                    applicationDetails.RollNumber = CurrentdivisionAllocation.RollNumber;
                                    applicationDetails.ClassDetailsKey = CurrentdivisionAllocation.ClassDetailsKey;
                                    applicationDetails.CurrentYear = promotionDetail.Promotion.CurrentYear ?? 0;
                                    applicationDetails.StudentStatusKey = DbConstants.StudentStatus.Ongoing;
                                    CurrentdivisionAllocation.IsActive = true;

                                }
                                if (PromoteddivisionAllocation != null)
                                    dbContext.StudentDivisionAllocations.Remove(PromoteddivisionAllocation);
                                dbContext.PromotionDetails.Remove(promotionDetail);

                                AccountFlowViewModel accountFlowModel = new AccountFlowViewModel();
                                accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.Promotion;
                                accountFlowModel.TransactionKey = objmodel.RowKey;
                                accountFlowModel.IsDelete = false;
                                AccountFlowService accountFlowService = new AccountFlowService(dbContext);
                                accountFlowService.DeleteAccountFlow(accountFlowModel);
                            }
                            else
                            {
                                transaction.Rollback();

                                model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.StudentsPromotion);
                                model.IsSuccessful = false;
                                ActivityLog.CreateActivityLog(MenuConstants.GenerateRollNumber, ActionConstants.Delete, DbConstants.LogType.Info, Id, model.Message);
                                return model;
                            }
                        }
                    }
                    List<PromotionDetail> PromotionList = dbContext.PromotionDetails.Where(x => x.PromotionMasterKey == Id).ToList();
                    if (PromotionList.Count == 0)
                    {
                        dbContext.Promotions.Remove(promotion);
                        dbContext.SaveChanges();
                        transaction.Commit();
                        model.Message = EduSuiteUIResources.Success;
                        model.IsSuccessful = true;
                        ActivityLog.CreateActivityLog(MenuConstants.StudentsPromotion, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);

                    }
                    else
                    {
                        transaction.Rollback();

                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.StudentsPromotion);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.GenerateRollNumber, ActionConstants.Delete, DbConstants.LogType.Info, Id, model.Message);
                        return model;
                    }


                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.StudentsPromotion);
                        model.IsSuccessful = false;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.StudentsPromotion);
                    model.IsSuccessful = false;
                }
            }
            return model;
        }

        public StudentsPromotionViewModel ResetPromotion(long Id)
        {
            StudentsPromotionViewModel model = new StudentsPromotionViewModel();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    PromotionDetail promotionDetail = dbContext.PromotionDetails.SingleOrDefault(x => x.RowKey == Id);
                    Attendance attendanceDetails = dbContext.Attendances.SingleOrDefault(x => x.ApplicationKey == promotionDetail.ApplicationKey && x.ClassDetailsKey == promotionDetail.PromotedClassDetailsKey);

                    if (promotionDetail.PromotionStatusKey == DbConstants.StudentStatus.Completed || attendanceDetails == null)
                    {
                        StudentDivisionAllocation CurrentdivisionAllocation = dbContext.StudentDivisionAllocations.SingleOrDefault(x => x.ApplicationKey == promotionDetail.ApplicationKey && x.ClassDetailsKey == promotionDetail.Promotion.CurrentClassDetailsKey);
                        StudentDivisionAllocation PromoteddivisionAllocation = dbContext.StudentDivisionAllocations.SingleOrDefault(x => x.ApplicationKey == promotionDetail.ApplicationKey && x.ClassDetailsKey == promotionDetail.PromotedClassDetailsKey);
                        Application applicationDetails = dbContext.Applications.SingleOrDefault(x => x.RowKey == promotionDetail.ApplicationKey);

                        if (applicationDetails != null && CurrentdivisionAllocation != null)
                        {
                            applicationDetails.RollNumber = CurrentdivisionAllocation.RollNumber;
                            applicationDetails.ClassDetailsKey = CurrentdivisionAllocation.ClassDetailsKey;
                            applicationDetails.CurrentYear = promotionDetail.Promotion.CurrentYear ?? 0;
                            applicationDetails.StudentStatusKey = DbConstants.StudentStatus.Ongoing;
                            CurrentdivisionAllocation.IsActive = true;

                        }
                        if (PromoteddivisionAllocation != null)
                            dbContext.StudentDivisionAllocations.Remove(PromoteddivisionAllocation);
                        dbContext.PromotionDetails.Remove(promotionDetail);

                        AccountFlowViewModel accountFlowModel = new AccountFlowViewModel();
                        accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.Promotion;
                        accountFlowModel.TransactionKey = Id;
                        accountFlowModel.IsDelete = false;
                        AccountFlowService accountFlowService = new AccountFlowService(dbContext);
                        accountFlowService.DeleteAccountFlow(accountFlowModel);

                        dbContext.SaveChanges();
                        transaction.Commit();
                        model.Message = EduSuiteUIResources.Success;
                        model.IsSuccessful = true;
                        ActivityLog.CreateActivityLog(MenuConstants.StudentsPromotion, ActionConstants.Delete, DbConstants.LogType.Info, Id, model.Message);

                    }
                    else
                    {
                        transaction.Rollback();

                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.StudentsPromotion);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.GenerateRollNumber, ActionConstants.Delete, DbConstants.LogType.Info, Id, model.Message);
                    }

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.StudentsPromotion);
                        model.IsSuccessful = false;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.StudentsPromotion);
                    model.IsSuccessful = false;
                }
            }
            return model;
        }

        public List<StudentsPromotionViewModel> GetPromotion(StudentsPromotionViewModel model)
        {
            try
            {
                IQueryable<StudentsPromotionViewModel> PromotionList = (from p in dbContext.Promotions
                                                                        orderby p.RowKey descending
                                                                        select new StudentsPromotionViewModel
                                                                        {
                                                                            RowKey = p.RowKey,
                                                                            AcademicTermKey = p.ClassDetail.UniversityCourse.AcademicTermKey,
                                                                            BranchKey = p.BranchKey,
                                                                            BranchName = p.Branch.BranchName,
                                                                            ClassDetailsKey = p.CurrentClassDetailsKey ?? 0,
                                                                            ClassDetailsName = p.ClassDetail.ClassCode,
                                                                            //CourseKey = p.ClassDetail.UniversityCourse.CourseKey,
                                                                            CourseName = p.ClassDetail.UniversityCourse.Course.CourseName,
                                                                            //UniversityMasterKey = p.ClassDetail.UniversityCourse.UniversityMasterKey,
                                                                            UniversityName = p.ClassDetail.UniversityCourse.UniversityMaster.UniversityMasterName,
                                                                            BatchKey = p.BatchKey ?? 0,
                                                                            BatchName = p.Batch.BatchName,
                                                                            CourseYear = p.CurrentYear,
                                                                            CourseDuration = p.ClassDetail.UniversityCourse.Course.CourseDuration,
                                                                            PromotedCount = dbContext.PromotionDetails.Where(x => x.PromotionStatusKey == DbConstants.PromotionStatus.Promoted && x.PromotionMasterKey == p.RowKey).Count(),
                                                                            CompletedCount = dbContext.PromotionDetails.Where(x => x.PromotionStatusKey == DbConstants.PromotionStatus.Completed && x.PromotionMasterKey == p.RowKey).Count(),
                                                                            DiscontinuedCount = dbContext.PromotionDetails.Where(x => x.PromotionStatusKey == DbConstants.PromotionStatus.Discontinued && x.PromotionMasterKey == p.RowKey).Count(),
                                                                            TotalStudentCount = dbContext.Applications.Where(x => x.ClassDetailsKey == p.CurrentClassDetailsKey && x.CurrentYear == p.CurrentYear && p.BatchKey == p.BatchKey && x.BranchKey == p.BranchKey).Count(),

                                                                        });


                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();

                if (Employee != null)
                {
                    if (Employee.BranchAccess != null)
                    {
                        var Branches = Employee.BranchAccess.Split(',').Select(Int16.Parse).ToList();
                        PromotionList = PromotionList.Where(row => Branches.Contains(row.BranchKey ?? 0));
                    }
                }

                if (model.BranchKey != 0)
                {
                    PromotionList = PromotionList.Where(row => row.BranchKey == model.BranchKey);
                }
                if (model.ClassDetailsKey != 0)
                {
                    PromotionList = PromotionList.Where(row => row.ClassDetailsKey == model.ClassDetailsKey);
                }
                if (model.BatchKey != 0)
                {
                    PromotionList = PromotionList.Where(row => row.BatchKey == model.BatchKey);
                }

                return PromotionList.GroupBy(x => new { x.RowKey }).Select(y => y.FirstOrDefault()).ToList<StudentsPromotionViewModel>();

            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.StudentsPromotion, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<StudentsPromotionViewModel>();


            }
        }



        #region DropDownLists
        private void FillDropDown(StudentsPromotionViewModel model)
        {
            FillBranch(model);
            FillCourseType(model);
            FillClassDetails(model);
            FillBatch(model);
            FillPromtionStatus(model);

            if (model.StudentsPromotionDetails.Count > 0)
            {
                FillPromotedClassDetails(model);
            }
        }
        private void FillBranch(StudentsPromotionViewModel model)
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
        public StudentsPromotionViewModel FillCourseType(StudentsPromotionViewModel model)
        {
            model.CourseTypes = (from CT in dbContext.VwCourseTypeSelectActiveOnlies
                                 join A in dbContext.Applications on CT.RowKey equals A.Course.CourseTypeKey
                                 where (A.BranchKey == model.BranchKey)
                                 select new SelectListModel
                                 {
                                     RowKey = CT.RowKey,
                                     Text = CT.CourseTypeName
                                 }).Distinct().ToList();


            return model;
        }
        public StudentsPromotionViewModel FillBatch(StudentsPromotionViewModel model)
        {

            if (model.ClassDetailsKey != 0)
            {

                //ClassDetail classDetailList = dbContext.ClassDetails.SingleOrDefault(x => x.RowKey == model.ClassDetailsKey);


                if (model.RowKey != 0)
                {
                    model.Batches = (from p in dbContext.Promotions
                                     join B in dbContext.VwBatchSelectActiveOnlies on p.BatchKey equals B.RowKey
                                     orderby B.RowKey
                                     where (p.BranchKey == model.BranchKey && p.CurrentClassDetailsKey == model.ClassDetailsKey)
                                     select new SelectListModel
                                     {
                                         RowKey = B.RowKey,
                                         Text = B.BatchName
                                     }).Distinct().ToList();
                }
                else
                {

                    model.Batches = (from p in dbContext.Applications
                                     join SDA in dbContext.StudentDivisionAllocations on p.RowKey equals SDA.ApplicationKey
                                     join B in dbContext.VwBatchSelectActiveOnlies on p.BatchKey equals B.RowKey
                                     orderby B.RowKey
                                     where (p.ClassDetailsKey == model.ClassDetailsKey && p.BranchKey == model.BranchKey && SDA.IsActive == true)
                                     select new SelectListModel
                                     {
                                         RowKey = B.RowKey,
                                         Text = B.BatchName
                                     }).Distinct().ToList();
                }

                //model.Batches = (from p in dbContext.Applications
                //                 join SDA in dbContext.StudentDivisionAllocations on p.RowKey equals SDA.ApplicationKey
                //                 join B in dbContext.VwBatchSelectActiveOnlies on p.BatchKey equals B.RowKey
                //                 orderby B.RowKey
                //                 where (p.ClassDetailsKey == model.ClassDetailsKey && p.BranchKey == model.BranchKey && (model.RowKey != 0 ? SDA.IsActive != null : SDA.IsActive == true))
                //                 select new SelectListModel
                //                 {
                //                     RowKey = B.RowKey,
                //                     Text = B.BatchName
                //                 }).Distinct().ToList();

            }



            return model;
        }
        private void FillPromtionStatus(StudentsPromotionViewModel model)
        {
            model.PromotionStatus = dbContext.VwPromotionStatusSelectActiveOnlies.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.PromotionStatusName
            }).ToList();

        }

        private void FillPromotedClassDetails(StudentsPromotionViewModel model)
        {
            ClassDetail classDetailList = dbContext.ClassDetails.SingleOrDefault(x => x.RowKey == model.ClassDetailsKey);

            model.UniversityCourseKey = classDetailList.UniversityCourseKey;
            model.CourseYear = classDetailList.StudentYear;

            model.PromotedClasses = (from CD in dbContext.VwClassDetailsSelectActiveOnlies

                                     where (CD.IsActive == true && CD.StudentYear == (model.CourseYear + 1) && CD.UniversityCourseKey == model.UniversityCourseKey)
                                     select new SelectListModel
                                     {
                                         RowKey = CD.RowKey,
                                         Text = CD.ClassCode + CD.ClassCodeDescription
                                     }).Distinct().ToList();
        }
        public StudentsPromotionViewModel FillClassDetails(StudentsPromotionViewModel model)
        {


            model.ClassDetails = (from CD in dbContext.VwClassDetailsSelectActiveOnlies

                                  join SDA in dbContext.StudentDivisionAllocations on CD.RowKey equals SDA.ClassDetailsKey
                                  //join A in dbContext.Applications on CD.RowKey equals A.ClassDetailsKey
                                  where (CD.CourseTypeKey == model.CourseTypeKey && (model.RowKey != 0 ? SDA.IsActive != null : SDA.IsActive == true))// && A.BranchKey == model.BranchKey)
                                  select new SelectListModel
                                  {
                                      RowKey = CD.RowKey,
                                      Text = CD.ClassCode + CD.ClassCodeDescription
                                  }).Distinct().ToList();

            return model;
        }

        public StudentsPromotionViewModel FillSearchClassDetails(StudentsPromotionViewModel model)
        {

            if (model.BranchKey != 0 && model.BranchKey != null)
            {
                model.ClassDetails = (from CD in dbContext.VwClassDetailsSelectActiveOnlies

                                      join SDA in dbContext.StudentDivisionAllocations on CD.RowKey equals SDA.ClassDetailsKey
                                      join A in dbContext.Applications on CD.RowKey equals A.ClassDetailsKey
                                      where (SDA.IsActive == true && A.BranchKey == model.BranchKey)
                                      select new SelectListModel
                                      {
                                          RowKey = CD.RowKey,
                                          Text = CD.ClassCode + CD.ClassCodeDescription
                                      }).Distinct().ToList();
            }
            else
            {
                model.ClassDetails = (from CD in dbContext.VwClassDetailsSelectActiveOnlies

                                      join SDA in dbContext.StudentDivisionAllocations on CD.RowKey equals SDA.ClassDetailsKey
                                      where (SDA.IsActive == true)
                                      select new SelectListModel
                                      {
                                          RowKey = CD.RowKey,
                                          Text = CD.ClassCode + CD.ClassCodeDescription
                                      }).Distinct().ToList();
            }

            return model;
        }
        public StudentsPromotionViewModel FillSearchBatch(StudentsPromotionViewModel model)
        {

            if ((model.BranchKey != 0 && model.BranchKey != null) || (model.ClassDetailsKey != 0 && model.ClassDetailsKey != null))
            {
                model.Batches = (from p in dbContext.Applications
                                 join SDA in dbContext.StudentDivisionAllocations on p.RowKey equals SDA.ApplicationKey
                                 join B in dbContext.VwBatchSelectActiveOnlies on p.BatchKey equals B.RowKey
                                 orderby B.RowKey
                                 //where (p.CourseKey == model.CourseKey && p.BranchKey == model.BranchKey && p.UniversityMasterKey == model.UniversityMasterKey)
                                 where (p.ClassDetailsKey == model.ClassDetailsKey || p.BranchKey == model.BranchKey)
                                 select new SelectListModel
                                 {
                                     RowKey = B.RowKey,
                                     Text = B.BatchName
                                 }).Distinct().ToList();
            }
            else
            {
                model.Batches = (from p in dbContext.Applications
                                 join SDA in dbContext.StudentDivisionAllocations on p.RowKey equals SDA.ApplicationKey
                                 join B in dbContext.VwBatchSelectActiveOnlies on p.BatchKey equals B.RowKey
                                 orderby B.RowKey
                                 //where (p.CourseKey == model.CourseKey && p.BranchKey == model.BranchKey && p.UniversityMasterKey == model.UniversityMasterKey)
                                 //where (p.ClassDetailsKey == model.ClassDetailsKey && p.BranchKey == model.BranchKey)
                                 select new SelectListModel
                                 {
                                     RowKey = B.RowKey,
                                     Text = B.BatchName
                                 }).Distinct().ToList();
            }
            return model;
        }
        public StudentsPromotionViewModel GetSearchDropdownList(StudentsPromotionViewModel model)
        {
            FillBranch(model);
            FillSearchClassDetails(model);
            FillSearchBatch(model);
            return model;
        }
        #endregion DropDownLists
    }
}
