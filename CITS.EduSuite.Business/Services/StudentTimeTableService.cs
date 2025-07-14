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
using System.Data.Entity;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class StudentTimeTableService : IStudentTimeTableService
    {
        private EduSuiteDatabase dbContext;
        public StudentTimeTableService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public StudentTimeTableViewModel GetTimeTableById(StudentTimeTableViewModel model)
        {
            try
            {
                //model = dbContext.StudentTimeTableMasters.Where(row => row.RowKey == model.RowKey).Select(row => new StudentTimeTableViewModel
                //{
                //    RowKey = row.RowKey,
                //    ClassDetailsKey = row.ClassDetailsKey,
                //    AcademicYearKey = row.AcademicYearKey


                //}).SingleOrDefault();
                if (model == null)
                {
                    model = new StudentTimeTableViewModel();

                }
                FillDropDown(model);
                return model;
            }
            catch (Exception Ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.StudentTimeTable, ActionConstants.View, DbConstants.LogType.Error, null, Ex.GetBaseException().Message);
                return new StudentTimeTableViewModel();
            }
        }

        public void FillTimeTableDetails(StudentTimeTableViewModel model)
        {

            //var CheckQuery = dbContext.StudentTimeTableMasters.Where(x => x.ClassDetailsKey == model.ClassDetailsKey && x.AcademicYearKey == model.AcademicYearKey).Select(row => row.RowKey);
            //if (CheckQuery.Any())
            //{
            //    model.RowKey = CheckQuery.FirstOrDefault();
            //}
            //else
            //{
            //    model.RowKey = 0;
            //}

            //model.StudentTimeTableDetailsModel = (from row in dbContext.StudentTimeTables.Where(x => x.MasterKey == model.RowKey)
            //                                      select new StudentTimeTableDetailsModel
            //                                      {
            //                                          RowKey = row.RowKey,
            //                                          SubjectKey = row.SubjectKey ?? 0,
            //                                          SubjectName = row.Subject.SubjectName,
            //                                          EmployeeKey = row.EmployeeKey ?? 0,
            //                                          EmployeeName = row.Employee.EmployeeName,
            //                                          Day = row.Day,
            //                                          PeriodKey = row.PeriodKey,
            //                                          MasterKey = row.MasterKey,

            //                                      }).ToList();
            //if (model.StudentTimeTableDetailsModel.Count == 0)
            //{
            //    model.StudentTimeTableDetailsModel.Add(new StudentTimeTableDetailsModel());
            //}


        }

        public List<StudentTimeTableViewModel> GetTimeTable(StudentTimeTableViewModel model)
        {
            try
            {
                List<StudentTimeTableViewModel> timeTableList = (from row in dbContext.StudentTimeTables
                                                                 where (row.Day == model.Day && row.BranchKey == model.BranchKey)
                                                                 select new StudentTimeTableViewModel
                                                                 {
                                                                     RowKey = row.RowKey,
                                                                     SubjectKey = row.SubjectKey ?? 0,
                                                                     SubjectName = row.Subject.SubjectName,
                                                                     EmployeeKey = row.EmployeeKey ?? 0,
                                                                     EmployeeName = row.Employee.FirstName,
                                                                     ClassDetailsKey = row.ClassDetailsKey ?? 0,
                                                                     Day = row.Day,
                                                                     PeriodKey = row.PeriodKey,
                                                                     BranchKey = row.BranchKey,

                                                                 }).ToList();


                return timeTableList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<StudentTimeTableViewModel>();

            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.StudentTimeTable, ActionConstants.MenuAccess, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return new List<StudentTimeTableViewModel>();
            }
        }

        public StudentTimeTableViewModel UpdateStudentTimeTableList(List<StudentTimeTableViewModel> modelList)
        {
            StudentTimeTableViewModel model = new StudentTimeTableViewModel();

            // GetBranches(model);

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    CreateStudentTimeTable(modelList.Where(row => row.RowKey == 0).ToList());
                    UpdateStudentTimeTable(modelList.Where(row => row.RowKey != 0 && row.EmployeeKey != null).ToList());
                    DeleteStudentTimeTable(modelList.Where(row => row.RowKey != 0 && row.EmployeeKey == null).ToList());
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentTimeTable, ActionConstants.Edit, DbConstants.LogType.Info, model.EmployeeKey, model.Message);
                    model.IsSuccessful = true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ActivityLog.CreateActivityLog(MenuConstants.StudentTimeTable, ActionConstants.Edit, DbConstants.LogType.Error, model.EmployeeKey, ex.GetBaseException().Message);
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.StudentTimeTable);
                    model.IsSuccessful = false;
                }

            }
            return model;
        }

        private void CreateStudentTimeTable(List<StudentTimeTableViewModel> modelList)
        {
            long MaxKey = dbContext.StudentTimeTables.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (StudentTimeTableViewModel modelDetails in modelList.Where(row => row.RowKey == 0))
            {
                StudentTimeTable StudentTimeTablemodel = new StudentTimeTable();


                StudentTimeTablemodel.RowKey = MaxKey + 1;
                StudentTimeTablemodel.EmployeeKey = modelDetails.EmployeeKey;
                StudentTimeTablemodel.Day = modelDetails.Day;
                StudentTimeTablemodel.PeriodKey = modelDetails.PeriodKey;
                StudentTimeTablemodel.IsActive = true;
                StudentTimeTablemodel.ClassDetailsKey = modelDetails.ClassDetailsKey;
                StudentTimeTablemodel.SubjectKey = modelDetails.SubjectKey;
                StudentTimeTablemodel.BranchKey = modelDetails.BranchKey;

                dbContext.StudentTimeTables.Add(StudentTimeTablemodel);
                MaxKey++;
            }
        }

        private void UpdateStudentTimeTable(List<StudentTimeTableViewModel> modelList)
        {
            foreach (StudentTimeTableViewModel modelDetails in modelList.Where(row => row.RowKey != 0))
            {
                StudentTimeTable StudentTimeTablemodel = dbContext.StudentTimeTables.SingleOrDefault(p => p.RowKey == modelDetails.RowKey);

                StudentTimeTablemodel.EmployeeKey = modelDetails.EmployeeKey;
                StudentTimeTablemodel.Day = modelDetails.Day;
                StudentTimeTablemodel.PeriodKey = modelDetails.PeriodKey;
                StudentTimeTablemodel.IsActive = true;
                StudentTimeTablemodel.ClassDetailsKey = modelDetails.ClassDetailsKey;
                StudentTimeTablemodel.SubjectKey = modelDetails.SubjectKey;
                StudentTimeTablemodel.BranchKey = modelDetails.BranchKey;
            }
        }
        private void DeleteStudentTimeTable(List<StudentTimeTableViewModel> modelList)
        {
            foreach (StudentTimeTableViewModel modelDetails in modelList.Where(row => row.RowKey != 0 && row.EmployeeKey == null))
            {
                StudentTimeTable StudentTimeTablemodel = dbContext.StudentTimeTables.SingleOrDefault(p => p.RowKey == modelDetails.RowKey);

                dbContext.StudentTimeTables.Remove(StudentTimeTablemodel);
            }
        }

        public void FillDropDown(StudentTimeTableViewModel model)
        {
            FillClassDetails(model);
            FillWeekDays(model);
            FillWeeklyPeriods(model);
            FillBranches(model);
        }
        public void FillClassDetails(StudentTimeTableViewModel model)
        {

            model.ClassDetails = dbContext.VwClassDetailsSelectActiveOnlies.Select(CD => new SelectListModel
            {
                RowKey = CD.RowKey,
                Text = CD.ClassCode + CD.ClassCodeDescription
            }).ToList();

        }

        public void FillWeekDays(StudentTimeTableViewModel model)
        {
            model.WeekDays = typeof(DbConstants.WeekDays).GetFields().Select(row => new SelectListModel
            {
                RowKey = Convert.ToByte(row.GetValue(null).ToString()),
                Text = row.Name
            }).ToList();
        }
        public void FillWeeklyPeriods(StudentTimeTableViewModel model)
        {
            model.WeeklyPeriods = typeof(DbConstants.WeeklyPeriods).GetFields().Select(row => new SelectListModel
            {
                RowKey = Convert.ToByte(row.GetValue(null).ToString()),
                Text = row.Name
            }).ToList();
        }

        public void FillBranches(StudentTimeTableViewModel model)
        {
            model.Branches = dbContext.Branches.Where(x=>x.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BranchName
            }).ToList();
        }
    }
}
