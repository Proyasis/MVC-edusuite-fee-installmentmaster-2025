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
using CITS.EduSuite.Business.Models.Security;
using System.Threading;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class DesignationGradeService : IDesignationGradeService
    {
        private EduSuiteDatabase dbContext;

        public DesignationGradeService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<DesignationGradeViewModel> GetDesignationGrades(string searchText)
        {

            try
            {
                //var monthlyPayments = dbContext.DesignationGradeDetails.GroupBy(G => G.DesignationGradeKey).Select(row => new
                //{
                //    TotalMonthlyPayments = row.Sum(S => S.AmountUnit),
                //    DesignationGradeKey = row.Key
                //});
                //var statutoryDeductions = dbContext.DesignationGradeDetails.Where(row => row.SalaryHead.SalaryHeadTypeKey == DbConstants.SalaryHeadType.StatutoryDeductions).GroupBy(G => G.DesignationGradeKey).Select(row => new
                //{
                //    TotalStatutoryDeductions = row.Sum(S => S.AmountUnit),
                //    DesignationGradeKey = row.Key
                //});
                var designationGradeList = (from D in dbContext.DesignationGrades.Where(row => row.IsActive && row.Designation.DesignationName.Contains(searchText))
                                                //join MP in monthlyPayments on D.RowKey equals MP.DesignationGradeKey
                                                //join SD in statutoryDeductions on D.RowKey equals SD.DesignationGradeKey
                                            select new DesignationGradeViewModel
                                            {
                                                RowKey = D.RowKey,
                                                DesignationKey = D.DesignationKey,
                                                DesignationName = D.Designation.DesignationName,
                                                DesignationGradeName = D.DesignationGradeName,
                                                MonthlySalary = D.MonthlySalary ?? 0
                                            }).ToList();

                return designationGradeList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<DesignationGradeViewModel>();
            }
            catch (Exception ex)
            {
                return new List<DesignationGradeViewModel>();
            }
        }

        public DesignationGradeViewModel GetDesignationGradebyId(int Id)
        {
            try
            {
                DesignationGradeViewModel model = dbContext.DesignationGrades.Where(row => row.RowKey == Id).Select(row => new DesignationGradeViewModel
                {
                    RowKey = row.RowKey,
                    DesignationGradeName = row.DesignationGradeName,
                    DesignationKey = row.DesignationKey,
                    MonthlySalary = row.MonthlySalary,


                }).SingleOrDefault();
                if (model == null)
                {
                    model = new DesignationGradeViewModel();

                }
                model.DesignationGradeDetails = (from x in dbContext.SalaryHeads.Where(row => row.IsActive && row.SalaryHeadType.IsActive)
                                                 join y in dbContext.DesignationGradeDetails.Where(x => x.DesignationGradeKey == Id) on x.RowKey equals y.SalaryHeadKey into yj
                                                 from y in yj.DefaultIfEmpty()
                                                 select new DesignationGradeDetailViewModel
                                                 {
                                                     RowKey = y.RowKey != null ? y.RowKey : 0,
                                                     SalaryHeadTypeKey = y.RowKey != null ? y.SalaryHead.SalaryHeadTypeKey : x.SalaryHeadTypeKey,
                                                     SalaryHeadKey = y.RowKey != null ? y.SalaryHeadKey : x.RowKey,
                                                     SalaryHeadCode = x.SalaryHeadCode,
                                                     SalaryHeadName = x.SalaryHeadName,
                                                     SalaryHeadTypeName = x.SalaryHeadType.SalaryHeadTypeName,
                                                     Formula = y.Formula,
                                                     Applicable = y.ApplicableFormula,
                                                     AmountUnit = y.RowKey != null ? y.AmountUnit : 0,
                                                     IsInclude = y.SalaryHeadKey != null ? y.IsInclude ?? true : x.IsInclude ?? true,
                                                     IsFixed = y.SalaryHeadKey != null ? y.IsFixed ?? true : x.IsFixed ?? true,
                                                 }).ToList();
                GetDesignations(model);
                return model;
            }
            catch (Exception ex)
            {
                DesignationGradeViewModel model = new DesignationGradeViewModel();
                return model;
            }
        }

        public DesignationGradeViewModel UpdateDesignationGrade(List<DesignationGradeViewModel> modelList)
        {
            DesignationGradeViewModel designationGradeModel = new DesignationGradeViewModel();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    DesignationGradeDetailViewModel designationDetailModel = new DesignationGradeDetailViewModel();
                    int maxKey = dbContext.DesignationGrades.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    long DetailMaxKey = dbContext.DesignationGradeDetails.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    designationDetailModel.RowKey = Convert.ToInt64(DetailMaxKey + 1);

                    foreach (DesignationGradeViewModel modelItem in modelList)
                    {
                        DesignationGrade gradeCheck = dbContext.DesignationGrades.Where(row => row.RowKey == modelItem.RowKey).SingleOrDefault();
                        if (gradeCheck != null)
                        {
                            modelItem.RowKey = gradeCheck.RowKey;
                            UpdateDesginationGrade(modelItem, designationDetailModel);
                        }
                        else
                        {
                            modelItem.RowKey = Convert.ToInt32(maxKey + 1);
                            CreateDesginationGrade(modelItem, designationDetailModel);
                            maxKey++;
                        }
                    }
                    dbContext.SaveChanges();
                    transaction.Commit();
                    designationGradeModel.Message = EduSuiteUIResources.Success;
                    designationGradeModel.IsSuccessful = true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    designationGradeModel.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.DesignationGrade);
                    designationGradeModel.IsSuccessful = false;
                }
            }

            return designationGradeModel;
        }

        public DesignationGradeViewModel GetDesignations(DesignationGradeViewModel model)
        {
            model.Designations = dbContext.VwDesignationWithoutAdminSelectActiveOnlies.OrderBy(row => row.RowKey).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.DesignationName
            }).ToList();
            return model;
        }
        private void CreateDesginationGrade(DesignationGradeViewModel model, DesignationGradeDetailViewModel detailModel)
        {

            DesignationGrade designationGrade = new DesignationGrade();
            designationGrade.RowKey = model.RowKey;
            designationGrade.DesignationKey = model.DesignationKey;
            designationGrade.DesignationGradeName = model.DesignationGradeName;
            designationGrade.DisplayOrder = designationGrade.RowKey;
            designationGrade.ColumnLetter = model.ColumnLetter;
            designationGrade.MonthlySalary = model.MonthlySalary;
            designationGrade.IsActive = true;
            dbContext.DesignationGrades.Add(designationGrade);
            foreach (DesignationGradeDetailViewModel modelDetail in model.DesignationGradeDetails)
            {
                modelDetail.DesignationGradeKey = designationGrade.RowKey;
                modelDetail.RowKey = detailModel.RowKey;
                CreateDesginationGradeDetails(modelDetail);
                detailModel.RowKey++;
            }
            model.RowKey = designationGrade.RowKey;
            //model.UpdateType = DbConstants.UpdationType.Create;
        }
        private void UpdateDesginationGrade(DesignationGradeViewModel model, DesignationGradeDetailViewModel detailModel)
        {
            DesignationGrade designationGrade = new DesignationGrade();
            designationGrade = dbContext.DesignationGrades.SingleOrDefault(row => row.RowKey == model.RowKey);
            designationGrade.ColumnLetter = model.ColumnLetter;
            designationGrade.DesignationGradeName = model.DesignationGradeName;
            designationGrade.DesignationKey = model.DesignationKey;
            designationGrade.MonthlySalary = model.MonthlySalary;
            long maxKey = dbContext.DesignationGradeDetails.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (DesignationGradeDetailViewModel modelDetail in model.DesignationGradeDetails)
            {
                modelDetail.DesignationGradeKey = model.RowKey;
                DesignationGradeDetail gradeDetailCheck = dbContext.DesignationGradeDetails.Where(row => row.DesignationGradeKey == modelDetail.DesignationGradeKey && row.SalaryHead.SalaryHeadCode == modelDetail.SalaryHeadCode).SingleOrDefault();
                if (gradeDetailCheck != null)
                {
                    modelDetail.RowKey = gradeDetailCheck.RowKey;

                    UpdateDesginationGradeDetail(modelDetail);
                }
                else
                {
                    modelDetail.RowKey = detailModel.RowKey;
                    CreateDesginationGradeDetails(modelDetail);
                    detailModel.RowKey++;
                }

            }
            //model.RefKey = designationGrade.RefKey;
            //model.UpdateType = DbConstants.UpdationType.Update;
        }

        private void CreateDesginationGradeDetails(DesignationGradeDetailViewModel model)
        {

            DesignationGradeDetail designationGradeDetail = new DesignationGradeDetail();
            designationGradeDetail.RowKey = model.RowKey;
            designationGradeDetail.DesignationGradeKey = model.DesignationGradeKey;
            designationGradeDetail.SalaryHeadKey = dbContext.SalaryHeads.Where(row => row.SalaryHeadCode == model.SalaryHeadCode).Select(row => row.RowKey).SingleOrDefault();
            designationGradeDetail.AmountUnit = model.AmountUnit;
            designationGradeDetail.Formula = model.Formula;
            designationGradeDetail.ApplicableFormula = model.Applicable;
            designationGradeDetail.IsInclude = model.IsInclude;
            designationGradeDetail.IsFixed = model.IsFixed;
            dbContext.DesignationGradeDetails.Add(designationGradeDetail);
        }
        private void UpdateDesginationGradeDetail(DesignationGradeDetailViewModel model)
        {
            DesignationGradeDetail designationGradeDetail = dbContext.DesignationGradeDetails.SingleOrDefault(row => row.RowKey == model.RowKey);
            designationGradeDetail.SalaryHeadKey = dbContext.SalaryHeads.Where(row => row.SalaryHeadCode == model.SalaryHeadCode).Select(row => row.RowKey).SingleOrDefault();
            designationGradeDetail.AmountUnit = model.AmountUnit;
            designationGradeDetail.Formula = model.Formula;
            designationGradeDetail.ApplicableFormula = model.Applicable;
            designationGradeDetail.IsInclude = model.IsInclude;
            designationGradeDetail.IsFixed = model.IsFixed;

        }

        public DesignationGradeViewModel UpdateDesignationGradeName(DesignationGradeViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    DesignationGrade gradeCheck = dbContext.DesignationGrades.Where(row => row.RowKey == model.RowKey).SingleOrDefault();

                    // model.RefKey = gradeCheck.RefKey;

                    gradeCheck.DesignationGradeName = model.DesignationGradeName;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // model.ExceptionMessage = ex.GetBaseException().Message;
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.DesignationGrade);
                    model.IsSuccessful = false;
                }
            }

            return model;

        }

        public List<DesignationGradeDetailViewModel> GetDesignationGradeDetailsById(short Id)
        {
            List<DesignationGradeDetailViewModel> designationGradeList = dbContext.spSelectDesignationSalarySetting(Id).Select(row => new DesignationGradeDetailViewModel
            {
                DesignationGradeName = row.DesignationGradeName,
                Formula = row.Amount,
                SalaryHeadName = row.SalaryHeadName,
                SalaryHeadCode = row.SalaryHeadCode,
                SalaryHeadTypeKey = row.SalaryHeadTypeKey
            }).ToList();
            return designationGradeList;
        }

        public DesignationGradeViewModel DeleteDesignationGrade(DesignationGradeViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    List<DesignationGrade> designationGradeList = dbContext.DesignationGrades.Where(row => row.DesignationKey == model.DesignationKey).ToList();
                    // model.RefKey = dbContext.Roles.Where(row => row.RowKey == model.DesignationKey).Select(row => row.RefKey).FirstOrDefault();
                    foreach (DesignationGrade designationGrade in designationGradeList)
                    {
                        List<DesignationGradeDetail> designationGradeDetailList = dbContext.DesignationGradeDetails.Where(row => row.DesignationGrade.DesignationKey == model.DesignationKey).ToList();
                        designationGradeDetailList.ForEach(designationGradeDetail => dbContext.DesignationGradeDetails.Remove(designationGradeDetail));
                        dbContext.DesignationGrades.Remove(designationGrade);
                    }

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.DesignationGrade);
                        model.IsSuccessful = false;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.DesignationGrade);
                    model.IsSuccessful = false;
                }
            }

            return model;
        }

        public DesignationGradeViewModel DeleteDesignationGradeDetail(DesignationGradeViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    DesignationGrade designationGrade = dbContext.DesignationGrades.SingleOrDefault(row => row.RowKey == model.RowKey);

                    // model.RefKey = designationGrade.RefKey;

                    // char ColumnLetter = Convert.ToChar(designationGrade.ColumnLetter);
                    List<DesignationGradeDetail> designationGradeDetailList = dbContext.DesignationGradeDetails.Where(row => row.DesignationGradeKey == designationGrade.RowKey).ToList();
                    //designationGradeDetailList.ForEach(designationGradeDetail => dbContext.DesignationGradeDetails.Remove(designationGradeDetail));
                    //foreach (DesignationGradeDetail designationGradeDetail in dbContext.DesignationGradeDetails.Where(row => !row.Formula.Contains(row.DesignationGrade.ColumnLetter)).ToList())
                    //{
                    //    if ((int)Convert.ToChar(designationGradeDetail.DesignationGrade.ColumnLetter) > (int)ColumnLetter)
                    //    {
                    //        designationGradeDetail.Formula = designationGradeDetail.AmountUnit.ToString();
                    //    }
                    //}
                    dbContext.DesignationGradeDetails.RemoveRange(designationGradeDetailList);
                    dbContext.DesignationGrades.Remove(designationGrade);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    //model.ExceptionMessage = ex.GetBaseException().Message;
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.DesignationGrade);
                        model.IsSuccessful = false;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // model.ExceptionMessage = ex.GetBaseException().Message;
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.DesignationGrade);
                    model.IsSuccessful = false;
                }
            }

            return model;
        }

        public List<string> GetFormulasById(DesignationGradeDetailViewModel model)
        {
            return dbContext.DesignationGradeDetails.Where(row => row.SalaryHeadKey == model.SalaryHeadKey).Select(row => row.Formula).ToList();
        }

        //#region TriggerDatabase



        ////Salary

        //public void TriggerDatabaseInBackgroundDesignationGradeList(List<DesignationGradeViewModel> model)
        //{
        //    if (Thread.CurrentPrincipal.Identity.IsAuthenticated && (Thread.CurrentPrincipal as CITSEduSuitePrincipal).RoleKey != DbConstants.TaxUserRoleKey && ConnectionTool.CheckConnection())
        //    {

        //        Thread bgThread = new Thread(new ParameterizedThreadStart(TriggerDatabaseDesignationGradeList));
        //        bgThread.IsBackground = true;
        //        bgThread.Start(model);
        //    }

        //}



        //private void TriggerDatabaseDesignationGradeList(Object ObjViewModel)
        //{
        //    try
        //    {
        //        List<DesignationGradeViewModel> model = (List<DesignationGradeViewModel>)ObjViewModel;
        //        dbContext = new CITSPrintSWDatabase2();
        //        foreach (DesignationGradeViewModel row in model)
        //        {
        //            var rowKey = row.RowKey;
        //            row.RowKey = (int)(row.RefKey ?? 0);
        //            row.RefKey = rowKey;
        //            row.IsSecond = true;
        //        }

        //        UpdateDesignationGrade(model);

        //        dbContext = new CITSPrintSWDatabase();
        //        foreach (DesignationGradeViewModel item in model)
        //        {
        //            DesignationGrade objModel = dbContext.DesignationGrades.SingleOrDefault(row => row.RowKey == item.RefKey);
        //            if (objModel != null)
        //            {
        //                objModel.RefKey = item.RowKey;
        //            }
        //        }
        //        dbContext.SaveChanges();

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

        //public void TriggerDatabaseInBackgroundDesignationGrade(DesignationGradeViewModel model)
        //{
        //    if (Thread.CurrentPrincipal.Identity.IsAuthenticated && (Thread.CurrentPrincipal as CITSPrintSWPrincipal).RoleKey != DbConstants.TaxUserRoleKey && ConnectionTool.CheckConnection())
        //    {

        //        Thread bgThread = new Thread(new ParameterizedThreadStart(TriggerDatabaseDesignationGrade));
        //        bgThread.IsBackground = true;
        //        bgThread.Start(model);
        //    }

        //}



        //private void TriggerDatabaseDesignationGrade(Object ObjViewModel)
        //{
        //    try
        //    {
        //        DesignationGradeViewModel model = (DesignationGradeViewModel)ObjViewModel;
        //        model.IsSecond = true;
        //        dbContext = new CITSPrintSWDatabase2();
        //        if (model.UpdateType == DbConstants.UpdationType.Delete)
        //        {
        //            model.DesignationKey = (short)(model.RefKey ?? 0);
        //            DeleteDesignationGrade(model);
        //        }

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

        //public void TriggerDatabaseInBackgroundDesignationGradeItem(DesignationGradeViewModel model)
        //{
        //    if (Thread.CurrentPrincipal.Identity.IsAuthenticated && (Thread.CurrentPrincipal as CITSPrintSWPrincipal).RoleKey != DbConstants.TaxUserRoleKey && ConnectionTool.CheckConnection())
        //    {

        //        Thread bgThread = new Thread(new ParameterizedThreadStart(TriggerDatabaseDesignationGradeItem));
        //        bgThread.IsBackground = true;
        //        bgThread.Start(model);
        //    }

        //}



        //private void TriggerDatabaseDesignationGradeItem(Object ObjViewModel)
        //{
        //    try
        //    {
        //        DesignationGradeViewModel model = (DesignationGradeViewModel)ObjViewModel;
        //        dbContext = new CITSPrintSWDatabase2();
        //        model.IsSecond = true;
        //        if (model.UpdateType == DbConstants.UpdationType.Delete)
        //        {
        //            model.RowKey = (int)(model.RefKey ?? 0);
        //            DeleteDesignationGradeDetail(model);
        //        }

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

        //public void TriggerDatabaseInBackgroundGradeName(DesignationGradeViewModel model)
        //{
        //    if (Thread.CurrentPrincipal.Identity.IsAuthenticated && (Thread.CurrentPrincipal as CITSPrintSWPrincipal).RoleKey != DbConstants.TaxUserRoleKey && ConnectionTool.CheckConnection())
        //    {

        //        Thread bgThread = new Thread(new ParameterizedThreadStart(TriggerDatabaseGradeName));
        //        bgThread.IsBackground = true;
        //        bgThread.Start(model);
        //    }

        //}
        //private void TriggerDatabaseGradeName(Object ObjViewModel)
        //{
        //    try
        //    {
        //        DesignationGradeViewModel model = (DesignationGradeViewModel)ObjViewModel;
        //        dbContext = new CITSPrintSWDatabase2();
        //        model.IsSecond = true;
        //        model.RowKey = (int)(model.RefKey ?? 0);
        //        UpdateDesignationGradeName(model);

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
