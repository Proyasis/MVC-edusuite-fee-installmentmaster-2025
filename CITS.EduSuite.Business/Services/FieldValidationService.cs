using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Services
{
    public class FieldValidationService : IFieldValidationService
    {
        private EduSuiteDatabase dbContext;

        public FieldValidationService()
        {
            this.dbContext = new EduSuiteDatabase();
        }

        public FieldValidationViewModel GetFieldValidationById(short FieldType)
        {
            try
            {
                FieldValidationViewModel model = new FieldValidationViewModel();

                if (FieldType == DbConstants.FieldValidationType.Applicationconfig)
                {
                    model.FieldItems = (from M in dbContext.ApplicationConfigs
                                        select new FieldItemsModel
                                        {
                                            RowKey = M.RowKey,
                                            Name = M.Name,
                                            Value = M.Value,
                                            IsActive = (M.Value == "True" || M.Value == "true") ? true : false
                                        }).ToList();
                }
                else if (FieldType == DbConstants.FieldValidationType.EmployeeConfig)
                {
                    model.FieldItems = (from M in dbContext.EmployeeConfigs
                                        select new FieldItemsModel
                                        {
                                            RowKey = M.RowKey,
                                            Name = M.Name,
                                            Value = M.Value,
                                            IsActive = (M.Value == "True" || M.Value == "true") ? true : false
                                        }).ToList();
                }
                else
                {
                    model.FieldItems = (from M in dbContext.LibraryConfigs
                                        select new FieldItemsModel
                                        {
                                            RowKey = M.RowKey,
                                            Name = M.Name,
                                            Value = M.Value,
                                            IsActive = (M.Value == "True" || M.Value == "true") ? true : false
                                        }).ToList();
                }

                model.FieldType = FieldType;

                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.FieldValidation, ActionConstants.View, DbConstants.LogType.Error, FieldType, ex.GetBaseException().Message);
                return new FieldValidationViewModel();
            }
        }

        public FieldValidationViewModel UpdateFieldValidaion(FieldValidationViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    foreach (FieldItemsModel item in model.FieldItems)
                    {
                        if (model.FieldType == DbConstants.FieldValidationType.Applicationconfig)
                        {
                            ApplicationConfig applicationconfigmodel = new ApplicationConfig();
                            applicationconfigmodel = dbContext.ApplicationConfigs.SingleOrDefault(row => row.RowKey == item.RowKey);
                            applicationconfigmodel.Value = item.IsActive == true ? "True" : "False";
                        }
                        else if (model.FieldType == DbConstants.FieldValidationType.EmployeeConfig)
                        {
                            EmployeeConfig applicationconfigmodel = new EmployeeConfig();
                            applicationconfigmodel = dbContext.EmployeeConfigs.SingleOrDefault(row => row.RowKey == item.RowKey);
                            applicationconfigmodel.Value = item.IsActive == true ? "True" : "False";
                        }
                        else if (model.FieldType == DbConstants.FieldValidationType.LibraryConfig)
                        {
                            LibraryConfig applicationconfigmodel = new LibraryConfig();
                            applicationconfigmodel = dbContext.LibraryConfigs.SingleOrDefault(row => row.RowKey == item.RowKey);
                            applicationconfigmodel.Value = item.IsActive == true ? "True" : "False";
                        }


                    }

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;



                    ActivityLog.CreateActivityLog(MenuConstants.FieldValidation, ActionConstants.Edit, DbConstants.LogType.Info, model.FieldType, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.FieldValidation);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.FieldValidation, ActionConstants.Edit, DbConstants.LogType.Info, model.FieldType, model.Message);
                }

            }
            return model;
        }

        public List<FieldValidationViewModel> GetFieldValidation(string searchText)
        {
            try
            {
                var FieldValidationList = typeof(DbConstants.FieldValidationType).GetFields().Select(row => new FieldValidationViewModel
                {
                    FieldType = Convert.ToByte((row.GetValue(null).ToString())),
                    FieldTypeName = row.Name
                }).ToList();
                return FieldValidationList.GroupBy(x => x.FieldType).Select(y => y.First()).ToList<FieldValidationViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.FieldValidation, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<FieldValidationViewModel>();


            }
        }
    }
}
