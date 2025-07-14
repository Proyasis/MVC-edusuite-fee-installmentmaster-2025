using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;

namespace CITS.EduSuite.Business.Services
{
    public class MemberTypeService : IMemberTypeService
    {


        private EduSuiteDatabase dbContext;
        public MemberTypeService(EduSuiteDatabase objDB)
        {
            dbContext = objDB;
        }
        public List<MemberTypeViewModel> GetMemberTypes(string searchText)
        {
            try
            {
                var memberTypeList = (from mt in dbContext.MemberTypes
                                      orderby mt.RowKey descending
                                      where mt.MemberTypeName.Contains(searchText)
                                      select new MemberTypeViewModel
                                      {
                                          RowKey = mt.RowKey,
                                          MemberTypeName = mt.MemberTypeName,
                                          MemberTypeCode = mt.MemberTypeCode,
                                          NumberOfBooksAllowed = mt.NumberOfBooksAllowed,
                                          RegistrationFee = mt.RegistrationFee,
                                          MemberShipFee = mt.MemberShipFee,
                                          LateFeePerDay = mt.LateFeePerDay,
                                          IsActive = mt.IsActive ?? false

                                      }).ToList();
                return memberTypeList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<MemberTypeViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.MemberType, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<MemberTypeViewModel>();


            }
        }

        public MemberTypeViewModel GetMemberTypeById(byte? id)
        {
            MemberTypeViewModel model = new MemberTypeViewModel();
            model = dbContext.MemberTypes.Select(row => new MemberTypeViewModel
            {
                RowKey = row.RowKey,
                MemberTypeName = row.MemberTypeName,
                MemberTypeCode = row.MemberTypeCode,
                NumberOfBooksAllowed = row.NumberOfBooksAllowed,
                RegistrationFee = row.RegistrationFee,
                MemberShipFee = row.MemberShipFee,
                LateFeePerDay = row.LateFeePerDay,
                IsActive = row.IsActive ?? false,
                ReturnPeriod = row.ReturnPeriod

            }).Where(x => x.RowKey == id).FirstOrDefault();
            if (model == null)
            {
                model = new MemberTypeViewModel();
            }
            FillStatus(model);
            return model;
        }

        public MemberTypeViewModel CreateMemberType(MemberTypeViewModel model)
        {
            var memberTypeNameCheck = dbContext.MemberTypes.Where(row => row.MemberTypeName.ToLower() == model.MemberTypeName.ToLower()).ToList();
            MemberType memberTypeModel = new MemberType();

            FillStatus(model);
            if (memberTypeNameCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.MemberType);
                model.IsSuccessful = false;
                return model;
            }
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Byte maxKey = dbContext.MemberTypes.Select(a => a.RowKey).DefaultIfEmpty().Max();
                    memberTypeModel.RowKey = Convert.ToByte(maxKey + 1);
                    memberTypeModel.MemberTypeName = model.MemberTypeName;
                    memberTypeModel.MemberTypeCode = model.MemberTypeCode;
                    memberTypeModel.NumberOfBooksAllowed = Convert.ToByte(model.NumberOfBooksAllowed);
                    memberTypeModel.RegistrationFee = Convert.ToDecimal(model.RegistrationFee);
                    memberTypeModel.MemberShipFee = Convert.ToDecimal(model.MemberShipFee);
                    memberTypeModel.LateFeePerDay = Convert.ToDecimal(model.LateFeePerDay);
                    memberTypeModel.IsActive = model.IsActive;
                    memberTypeModel.ReturnPeriod = model.ReturnPeriod;
                    dbContext.MemberTypes.Add(memberTypeModel);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.MemberType, ActionConstants.Add, DbConstants.LogType.Info, memberTypeModel.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.MemberType);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.MemberType, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public MemberTypeViewModel UpdateMemberType(MemberTypeViewModel model)
        {
            var memberTypeNameCheck = dbContext.MemberTypes.Where(row => row.RowKey != model.RowKey && row.MemberTypeName.ToLower() == model.MemberTypeName.ToLower()).ToList();
            MemberType memberTypeModel = new MemberType();

            FillStatus(model);
            if (memberTypeNameCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.MemberType);
                model.IsSuccessful = false;
                return model;
            }
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    memberTypeModel = dbContext.MemberTypes.SingleOrDefault(row => row.RowKey == model.RowKey);
                    memberTypeModel.MemberTypeName = model.MemberTypeName;
                    memberTypeModel.MemberTypeCode = model.MemberTypeCode;
                    memberTypeModel.NumberOfBooksAllowed = Convert.ToByte(model.NumberOfBooksAllowed);
                    memberTypeModel.RegistrationFee = Convert.ToDecimal(model.RegistrationFee);
                    memberTypeModel.MemberShipFee = Convert.ToDecimal(model.MemberShipFee);
                    memberTypeModel.LateFeePerDay = Convert.ToDecimal(model.LateFeePerDay);
                    memberTypeModel.ReturnPeriod = model.ReturnPeriod;
                    memberTypeModel.IsActive = model.IsActive;

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.MemberType, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.MemberType);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.MemberType, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public MemberTypeViewModel DeleteMemberType(MemberTypeViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    MemberType memberType = dbContext.MemberTypes.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.MemberTypes.Remove(memberType);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.MemberType, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.MemberType);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.MemberType, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.MemberType);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.MemberType, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        private void FillStatus(MemberTypeViewModel model)
        {
            model.Statuses = dbContext.Status.Where(row => row.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.StatusName
            }).ToList();
        }

        public MemberTypeViewModel CheckMemberTypeCodeExist(MemberTypeViewModel model)
        {

            model.IsSuccessful = !dbContext.MemberTypes.Where(x => x.MemberTypeCode.ToLower() == model.MemberTypeCode.ToLower() && x.RowKey != model.RowKey).Any();

            return model;
        }
    }
}
