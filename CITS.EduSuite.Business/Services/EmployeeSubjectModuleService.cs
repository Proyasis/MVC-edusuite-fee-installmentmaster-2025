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
    public class EmployeeSubjectModuleService : IEmployeeSubjectModuleService
    {
        private EduSuiteDatabase dbContext;

        public EmployeeSubjectModuleService(EduSuiteDatabase ObjViewDataBase)
        {
            this.dbContext = ObjViewDataBase;
        }
        public EmployeeSubjectModuleViewModel FillEmployeeSubjectDetails(EmployeeSubjectModuleViewModel model)
        {

            model.EmployeeSubjectDetailsModel = (from TSA in dbContext.TeacherSubjectAllocations
                                                 join TCA in dbContext.TeacherClassAllocations on TSA.TeacherClassAllocationKey equals TCA.RowKey
                                                 where (TSA.Employeekey == model.EmployeesKey && TCA.IsActive)
                                                 select new EmployeeSubjectDetailsModel
                                              {
                                                  TeacherClassAllocationKey = TCA.RowKey != null ? TCA.RowKey : 0,
                                                  ClassDetailsKey = TCA.ClassDetailsKey,
                                                  ClassDetailsName = TCA.ClassDetail.ClassCode,
                                                  SubjectKey = TSA.SubjectKey,
                                                  SubjectName = TSA.Subject.SubjectName,
                                                  SubjectCode=TSA.Subject.SubjectCode,
                                                  EmployeeKey = TCA.EmployeeKey,
                                                  ModuleCount = TCA.TeacherSubjectModules.Where(x => x.IsActive&& x.SubjectKey==TSA.SubjectKey && x.ClassDetailsKey==TCA.ClassDetailsKey).Count(),
                                                  TotalModuleCount = dbContext.SubjectModules.Where(x => x.IsActive && x.SubjectKey == TSA.SubjectKey).Count(),
                                              }).ToList();

            return model;
        }

        public EmployeeSubjectModuleViewModel FillSubjectModules(EmployeeSubjectDetailsModel model)
        {
            EmployeeSubjectModuleViewModel objviewmodel = new EmployeeSubjectModuleViewModel();
            objviewmodel.EmployeeSubjectDetailsModel = (from SM in dbContext.SubjectModules
                                                        join TSM in dbContext.TeacherSubjectModules.Where(x => x.TeacherClassAllocationKey == model.TeacherClassAllocationKey && x.ClassDetailsKey == model.ClassDetailsKey && x.Employeekey == model.EmployeeKey) on SM.RowKey equals TSM.ModuleKey into TSMD
                                                        from TSM in TSMD.DefaultIfEmpty()
                                                        where (SM.SubjectKey == model.SubjectKey && SM.IsActive)
                                                        select new EmployeeSubjectDetailsModel
                                                        {
                                                            RowKey = TSM.RowKey != null ? TSM.RowKey : 0,
                                                            TeacherClassAllocationKey = model.TeacherClassAllocationKey,
                                                            ClassDetailsKey = model.ClassDetailsKey,
                                                            SubjectKey = SM.SubjectKey,
                                                            ModuleKey = SM.RowKey,
                                                            Modulename = SM.ModuleName,
                                                            EmployeeKey = model.EmployeeKey,
                                                            IsActive = TSM.RowKey != null ? TSM.IsActive : false
                                                        }).ToList();
            objviewmodel.EmployeesKey = model.EmployeeKey;

            return objviewmodel;
        }

        public EmployeeSubjectModuleViewModel UpdateEmployeeSubjectModule(EmployeeSubjectModuleViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    CreateEmployeeSubjectModuleDetails(model.EmployeeSubjectDetailsModel.Where(row => row.RowKey == 0).ToList(), model);
                    UpdateEmployeeSubjectModuleDetails(model.EmployeeSubjectDetailsModel.Where(row => row.RowKey != 0).ToList(), model);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    //model.AdmissionNo = dbContext.T_Application.Where(row => row.RowKey == model.ApplicationKey).Select(row => row.AdmissionNo).FirstOrDefault();
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.InternalExamResult, (model.EmployeeSubjectDetailsModel.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Info, model.EmployeesKey, model.Message);
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
                    model.Message =String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.EmployeeSubjectModules);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.TeacherModuleAllocation, (model.EmployeeSubjectDetailsModel.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, model.EmployeesKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        private void CreateEmployeeSubjectModuleDetails(List<EmployeeSubjectDetailsModel> modelList, EmployeeSubjectModuleViewModel objviewmodel)
        {
            Int64 MaxKey = dbContext.TeacherSubjectModules.Select(p => p.RowKey).DefaultIfEmpty().Max();

            foreach (EmployeeSubjectDetailsModel model in modelList)
            {

                TeacherSubjectModule teacherSubjectModuleModel = new TeacherSubjectModule();
                teacherSubjectModuleModel.RowKey = Convert.ToInt64(MaxKey + 1);
                teacherSubjectModuleModel.Employeekey = model.EmployeeKey;
                teacherSubjectModuleModel.SubjectKey = model.SubjectKey;
                teacherSubjectModuleModel.ClassDetailsKey = model.ClassDetailsKey;
                teacherSubjectModuleModel.TeacherClassAllocationKey = model.TeacherClassAllocationKey ?? 0;
                teacherSubjectModuleModel.ModuleKey = model.ModuleKey;
                teacherSubjectModuleModel.IsActive = model.IsActive;

                dbContext.TeacherSubjectModules.Add(teacherSubjectModuleModel);
                MaxKey++;

            }
        }

        private void UpdateEmployeeSubjectModuleDetails(List<EmployeeSubjectDetailsModel> modelList, EmployeeSubjectModuleViewModel objviewmodel)
        {
            foreach (EmployeeSubjectDetailsModel model in modelList)
            {

                TeacherSubjectModule teacherSubjectModuleModel = new TeacherSubjectModule();
                teacherSubjectModuleModel = dbContext.TeacherSubjectModules.SingleOrDefault(row => row.RowKey == model.RowKey);
                teacherSubjectModuleModel.IsActive = model.IsActive;

            }
        }


    }
}
