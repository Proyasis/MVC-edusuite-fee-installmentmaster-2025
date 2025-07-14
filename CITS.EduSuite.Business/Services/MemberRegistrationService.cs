using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using CITS.EduSuite.Business.Common;
using System.Data.Entity.Infrastructure;
using System.Web;
using System.IO;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class MemberRegistrationService : IMemberRegistrationService
    {
        private EduSuiteDatabase dbContext;
        public MemberRegistrationService(EduSuiteDatabase objdb)
        {
            dbContext = objdb;
        }

        public List<MemberRegistrationViewModel> GetMemberRegistration(string SearchText, short? BranchKey)
        {
            try
            {
                var MemberRegistrationList = (from MR in dbContext.MemberRegistrations
                                              join MP in dbContext.MemberPlanDetails.Where(x => x.ApplicationTypeKey == DbConstants.ApplicationType.Other) on MR.RowKey equals MP.ApplicationKey
                                              orderby MR.RowKey
                                              where MR.MemberFirstName.Contains(SearchText)
                                              select new MemberRegistrationViewModel
                                              {
                                                  RowKey = MR.RowKey,
                                                  MemberFullName = MR.MemberFirstName + " " + MR.MemberLastName,
                                                  IdentificationNumber = MR.IdentificationNumber,
                                                  PhoneNo = MR.PhoneNo,
                                                  EmailAddress = MR.EmailAddress,
                                                  MemberTypeName = MP.MemberType.MemberTypeName,
                                                  BorrowerTypeName = MP.BorrowerType.BorrowerTypeName,
                                                  RegistrationDate = MR.RegistrationDate,
                                                  IsBlockMember = MP.IsBlockMember,
                                                  CardId = MP.CardId,
                                                  BranchName = MR.Branch.BranchName,
                                                  BranchKey = MR.BranchKey
                                              }).ToList();
                if (BranchKey != null && BranchKey != 0)
                {
                    MemberRegistrationList = MemberRegistrationList.Where(x => x.BranchKey == BranchKey).ToList();
                }
                return MemberRegistrationList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<MemberRegistrationViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.MemberRegistration, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<MemberRegistrationViewModel>();
            }
        }

        public MemberRegistrationViewModel GetMemberRegistrationById(Int32? Id)
        {
            MemberRegistrationViewModel model = new MemberRegistrationViewModel();
            model = dbContext.MemberRegistrations.Select(row => new MemberRegistrationViewModel
            {
                RowKey = row.RowKey,
                MemberTitleKey = row.MemberTitleKey,
                MemberFirstName = row.MemberFirstName,
                MemberLastName = row.MemberLastName,
                MemberDOB = row.MemberDOB,
                MemberPhoto = row.MemberPhoto,
                IdentificationTypeKey = row.IdentificationTypeKey,
                IdentificationNumber = row.IdentificationNumber,
                StreetName = row.StreetName,
                CityName = row.CityName,
                StateKey = row.StateKey,
                ZipCode = row.ZipCode,
                PhoneNo = row.PhoneNo,
                EmailAddress = row.EmailAddress,
                Gender = row.Gender,
                RegistrationDate = row.RegistrationDate,


                BranchKey = row.BranchKey
            }).Where(x => x.RowKey == Id).FirstOrDefault();
            if (model == null)
            {
                model = new MemberRegistrationViewModel();
            }
            else
            {
                var planDetails = dbContext.MemberPlanDetails.FirstOrDefault(x => x.ApplicationTypeKey == DbConstants.ApplicationType.Other && x.ApplicationKey == Id);
                model.IsBlockMember = planDetails.IsBlockMember;
                model.CardId = planDetails.CardId;
                model.BorrowerTypeKey = planDetails.BorrowerTypeKey;
                model.MemberTypeKey = planDetails.MemberTypeKey;
                model.MemberShipFee = planDetails.MemberType.MemberShipFee;
                model.RegistrationFee = planDetails.MemberType.RegistrationFee;
                model.MemberPhotoUrl = model.MemberPhoto != null ? UrlConstants.LibraryMemberUrl + model.CardId + "/" + model.MemberPhoto : EduSuiteUIResources.DefaultPhotoUrl;
            }
            FillDropdownLists(model);
            return model;
        }

        public MemberRegistrationViewModel CreateMemberRregistration(MemberRegistrationViewModel model)
        {
            FillDropdownLists(model);
            MemberRegistration MemberRegistrationModel = new MemberRegistration();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    long Maxkey = dbContext.MemberRegistrations.Select(x => x.RowKey).DefaultIfEmpty().Max();
                    MemberRegistrationModel.RowKey = Convert.ToInt32(Maxkey + 1);
                    MemberRegistrationModel.MemberTitleKey = model.MemberTitleKey;
                    MemberRegistrationModel.MemberFirstName = model.MemberFirstName;
                    MemberRegistrationModel.MemberLastName = model.MemberLastName;
                    MemberRegistrationModel.MemberDOB = model.MemberDOB;
                    MemberRegistrationModel.MemberPhoto = model.MemberPhoto;
                    MemberRegistrationModel.IdentificationTypeKey = model.IdentificationTypeKey;
                    MemberRegistrationModel.IdentificationNumber = model.IdentificationNumber;
                    MemberRegistrationModel.StreetName = model.StreetName;
                    MemberRegistrationModel.CityName = model.CityName;
                    MemberRegistrationModel.StateKey = model.StateKey;
                    MemberRegistrationModel.ZipCode = model.ZipCode;
                    MemberRegistrationModel.PhoneNo = model.PhoneNo;
                    MemberRegistrationModel.EmailAddress = model.EmailAddress;
                    MemberRegistrationModel.Gender = model.Gender;
                    MemberRegistrationModel.BranchKey = model.BranchKey;
                    MemberRegistrationModel.RegistrationDate = Convert.ToDateTime(model.RegistrationDate);

                    var BorrowerTypeList = dbContext.BorrowerTypes.SingleOrDefault(row => row.RowKey == model.BorrowerTypeKey);
                    var MemberTypeList = dbContext.MemberTypes.SingleOrDefault(row => row.RowKey == model.MemberTypeKey);


                    model.CardId = dbContext.Database.SqlQuery<string>("select dbo.F_GenerateLibraryCardId(" + model.BranchKey + "," + model.MemberTypeKey + "," + model.BorrowerTypeKey + ")").Single().ToString();
                    model.CardSerialNo = dbContext.Database.SqlQuery<int>("select dbo.F_GenerateLibraryCardSerialNo(" + model.BranchKey + "," + model.MemberTypeKey + "," + model.BorrowerTypeKey + ")").Single();


                    if (model.MFile != null)
                    {
                        string Extension = Path.GetExtension(model.MFile.FileName);
                        string FileName = model.CardId + Extension;
                        MemberRegistrationModel.MemberPhoto = FileName;
                    }
                    dbContext.MemberRegistrations.Add(MemberRegistrationModel);

                    MemberPlanDetail MemberPlanDetailModel = new MemberPlanDetail();
                    Int64 MemberPlanMaxkey = dbContext.MemberPlanDetails.Select(x => x.RowKey).DefaultIfEmpty().Max();
                    MemberPlanDetailModel.RowKey = (MemberPlanMaxkey + 1);
                    MemberPlanDetailModel.MemberTypeKey = model.MemberTypeKey;
                    MemberPlanDetailModel.BorrowerTypeKey = model.BorrowerTypeKey;
                    MemberPlanDetailModel.IsBlockMember = model.IsBlockMember;
                    MemberPlanDetailModel.CardId = model.CardId;
                    MemberPlanDetailModel.CardSerialNo = model.CardSerialNo;
                    MemberPlanDetailModel.ApplicationTypeKey = DbConstants.ApplicationType.Other;
                    MemberPlanDetailModel.ApplicationKey = MemberRegistrationModel.RowKey;
                    MemberPlanDetailModel.BranchKey = MemberRegistrationModel.BranchKey;

                    dbContext.MemberPlanDetails.Add(MemberPlanDetailModel);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.MemberRegistration, ActionConstants.Add, DbConstants.LogType.Info, MemberPlanDetailModel.RowKey, model.Message);


                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.MemberRegistration);
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.MemberRegistration, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public MemberRegistrationViewModel UpdateMemberRregistration(MemberRegistrationViewModel model)
        {
            FillDropdownLists(model);
            MemberRegistration MemberRegistrationModel = new MemberRegistration();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    MemberRegistrationModel = dbContext.MemberRegistrations.SingleOrDefault(row => row.RowKey == model.RowKey);
                    MemberRegistrationModel.MemberTitleKey = model.MemberTitleKey;
                    MemberRegistrationModel.MemberFirstName = model.MemberFirstName;
                    MemberRegistrationModel.MemberLastName = model.MemberLastName;
                    MemberRegistrationModel.MemberDOB = model.MemberDOB;
                    MemberRegistrationModel.IdentificationTypeKey = model.IdentificationTypeKey;
                    MemberRegistrationModel.IdentificationNumber = model.IdentificationNumber;
                    MemberRegistrationModel.StreetName = model.StreetName;
                    MemberRegistrationModel.CityName = model.CityName;
                    MemberRegistrationModel.StateKey = model.StateKey;
                    MemberRegistrationModel.ZipCode = model.ZipCode;
                    MemberRegistrationModel.PhoneNo = model.PhoneNo;
                    MemberRegistrationModel.EmailAddress = model.EmailAddress;
                    MemberRegistrationModel.Gender = model.Gender;
                    MemberRegistrationModel.BranchKey = model.BranchKey;
                    MemberRegistrationModel.RegistrationDate = Convert.ToDateTime(model.RegistrationDate);


                    MemberPlanDetail MemberPlanDetailModel = new MemberPlanDetail();
                    MemberPlanDetailModel = dbContext.MemberPlanDetails.FirstOrDefault(x => x.ApplicationTypeKey == DbConstants.ApplicationType.Other && x.ApplicationKey == MemberRegistrationModel.RowKey);
                    MemberPlanDetailModel.MemberTypeKey = model.MemberTypeKey;
                    MemberPlanDetailModel.BorrowerTypeKey = model.BorrowerTypeKey;
                    MemberPlanDetailModel.IsBlockMember = model.IsBlockMember;
                    MemberPlanDetailModel.BranchKey = MemberRegistrationModel.BranchKey;

                    if (model.MFile != null)
                    {
                        string Extension = Path.GetExtension(model.MFile.FileName);
                        string FileName = MemberPlanDetailModel.CardId + Extension;
                        MemberRegistrationModel.MemberPhoto = FileName;
                    }

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.MemberRegistration, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.MemberRegistration);
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.MemberRegistration, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public MemberRegistrationViewModel DeleteMemberRegistration(MemberRegistrationViewModel model)
        {
            MemberRegistration MemberRegistrationModel = new MemberRegistration();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    MemberRegistrationModel = dbContext.MemberRegistrations.SingleOrDefault(row => row.RowKey == model.RowKey);
                    model.MemberPhotoUrl = UrlConstants.LibraryMemberUrl + MemberRegistrationModel.MemberPhoto;
                    dbContext.MemberRegistrations.Remove(MemberRegistrationModel);
                    MemberPlanDetail MemberPlanDetailModel = new MemberPlanDetail();
                    MemberPlanDetailModel = dbContext.MemberPlanDetails.FirstOrDefault(row => row.ApplicationTypeKey == DbConstants.ApplicationType.Other && row.ApplicationKey == model.RowKey);
                    dbContext.MemberPlanDetails.Remove(MemberPlanDetailModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.MemberRegistration, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.MemberRegistration);
                        model.IsSuccessful = false;

                        ActivityLog.CreateActivityLog(MenuConstants.MemberRegistration, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.MemberRegistration);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.MemberRegistration, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public MemberRegistrationViewModel CheckPhoneExists(string PhoneNo, int RowKey)
        {
            MemberRegistrationViewModel model = new MemberRegistrationViewModel();
            if (dbContext.MemberRegistrations.Where(row => row.PhoneNo.ToLower() == PhoneNo.ToLower() && row.RowKey != RowKey).Any())
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.PhoneNo);
                model.IsSuccessful = false;
            }
            else
            {
                model.Message = "";
                model.IsSuccessful = true;
            }
            return model;
        }

        public MemberRegistrationViewModel CheckEmailExists(string EmailAddress, int RowKey)
        {
            MemberRegistrationViewModel model = new MemberRegistrationViewModel();
            if (dbContext.MemberRegistrations.Where(row => row.EmailAddress.ToLower() == EmailAddress.ToLower() && row.RowKey != RowKey).Any())
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Email);
                model.IsSuccessful = false;
            }
            else
            {
                model.Message = "";
                model.IsSuccessful = true;
            }
            return model;
        }

        private void FillMemberTitle(MemberRegistrationViewModel model)
        {
            model.MemberTitle = dbContext.MemberTitles.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.MemberTitleName
            }).ToList();
        }

        private void FillState(MemberRegistrationViewModel model)
        {
            model.State = dbContext.Provinces.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.Provincename
            }).ToList();
        }

        private void FillIdentificationType(MemberRegistrationViewModel model)
        {
            model.IdentificationType = dbContext.IdentificationTypes.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.IdentificationTypeName
            }).ToList();
        }

        private void FillMemberType(MemberRegistrationViewModel model)
        {
            model.MemberType = dbContext.MemberTypes.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.MemberTypeName
            }).ToList();
        }

        private void FillBorrowerType(MemberRegistrationViewModel model)
        {
            model.BorrowerType = dbContext.BorrowerTypes.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BorrowerTypeName
            }).ToList();
        }

        private void FillDropdownLists(MemberRegistrationViewModel model)
        {
            FillBorrowerType(model);
            FillIdentificationType(model);
            FillMemberType(model);
            FillMemberTitle(model);
            FillState(model);
            FillBranches(model);
        }

        public MemberRegistrationViewModel GetFeesById(MemberRegistrationViewModel model)
        {
            if (model.MemberTypeKey != null && model.MemberTypeKey != 0)
            {
                var MemberTypeList = dbContext.MemberTypes.SingleOrDefault(x => x.RowKey == model.MemberTypeKey);
                if (MemberTypeList != null)
                {
                    model.RegistrationFee = MemberTypeList.RegistrationFee;
                    model.MemberShipFee = MemberTypeList.MemberShipFee;
                }
            }
            return model;
        }

        public MemberRegistrationViewModel CheckIdentificationExists(MemberRegistrationViewModel model)
        {
            if (dbContext.MemberRegistrations.Where(row => row.IdentificationTypeKey == model.IdentificationTypeKey && row.IdentificationNumber == model.IdentificationNumber && row.RowKey != model.RowKey).Any())
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.IdentificationType);
                model.IsSuccessful = false;
            }
            else
            {
                model.Message = "";
                model.IsSuccessful = true;
            }
            return model;
        }
        public void FillBranches(MemberRegistrationViewModel model)
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
    }
}
