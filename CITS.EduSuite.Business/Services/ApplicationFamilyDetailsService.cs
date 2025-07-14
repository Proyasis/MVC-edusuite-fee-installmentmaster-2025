using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System.Data.Entity.Infrastructure;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class ApplicationFamilyDetailsService : IApplicationFamilyDetailsService
    {
        private EduSuiteDatabase dbContext;

        public ApplicationFamilyDetailsService(EduSuiteDatabase objdb)
        {
            this.dbContext = objdb;
        }

        public ApplicationFamilyDetailsViewModel GetApplicationFamilyDetailsById(Int64 ApplicationKey)
        {
            try
            {
                ApplicationFamilyDetailsViewModel model = new ApplicationFamilyDetailsViewModel();
                model.FamilyDetailsModel = (from FMT in dbContext.FamilyMemberTypes
                                            join AFD in dbContext.ApplicationFamilyDetails.Where(x => x.ApplicationKey == ApplicationKey) on new { FamilyMemberTypeKey = FMT.RowKey } equals new { FamilyMemberTypeKey = AFD.FamilyMemberTypeKey ?? 0 } into row
                                            from AFD in row.DefaultIfEmpty()
                                            select new FamilyDetailsModel
                                            {
                                                RowKey = AFD.RowKey != null ? AFD.RowKey : 0,
                                                FamilymemberTypeName = FMT.MemberTypeName,
                                                FamilyMemberTypeKey = FMT.RowKey,
                                                Name = AFD.Name,
                                                Phone = AFD.Phone,
                                                Email = AFD.Email,
                                                Occupation = AFD.Occupation,
                                                SendMail = AFD.SendMail != null ? AFD.SendMail : false,
                                                sendSMS = AFD.sendSMS != null ? AFD.sendSMS : false,
                                                IsActive = AFD.IsActive != null ? AFD.IsActive : false,
                                                //VerifiedDate = row.VerifiedDate,
                                                ApplicationKey = AFD.ApplicationKey != null ? AFD.ApplicationKey : 0,
                                                IfLogin = AFD.IfLogin != null ? AFD.IfLogin : false
                                            }).ToList();

                if (model == null)
                {
                    model = new ApplicationFamilyDetailsViewModel();
                }
                model.ApplicationKey = ApplicationKey;

                return model;
            }
            catch (Exception Ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.ApplicationFamilyDetails, ActionConstants.View, DbConstants.LogType.Debug, ApplicationKey, Ex.GetBaseException().Message);
                return new ApplicationFamilyDetailsViewModel();


            }
        }

        public ApplicationFamilyDetailsViewModel UpdateApplicationFamilyDetails(ApplicationFamilyDetailsViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    CreateFamilydetails(model.FamilyDetailsModel.Where(row => row.RowKey == 0).ToList(), model);
                    UpdateFamilydetails(model.FamilyDetailsModel.Where(row => row.RowKey != 0).ToList(), model);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.AdmissionNo = dbContext.Applications.Where(row => row.RowKey == model.ApplicationKey).Select(row => row.AdmissionNo).FirstOrDefault();
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationFamilyDetails, (model.FamilyDetailsModel.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Info, model.ApplicationKey, model.Message);

                }
                catch (Exception Ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.FamilyDetails);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationFamilyDetails, (model.FamilyDetailsModel.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, model.ApplicationKey, Ex.GetBaseException().Message);
                }
            }
            return model;
        }

        private void CreateFamilydetails(List<FamilyDetailsModel> modelList, ApplicationFamilyDetailsViewModel objviewmodel)
        {
            Int64 maxKey = dbContext.ApplicationFamilyDetails.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (FamilyDetailsModel model in modelList)
            {
                if (model.Name != null)
                {
                    ApplicationFamilyDetail applicationFamilyDetailModel = new ApplicationFamilyDetail();
                    applicationFamilyDetailModel.RowKey = Convert.ToInt64(maxKey + 1);
                    applicationFamilyDetailModel.ApplicationKey = objviewmodel.ApplicationKey;
                    applicationFamilyDetailModel.FamilyMemberTypeKey = model.FamilyMemberTypeKey;
                    applicationFamilyDetailModel.Name = model.Name;
                    applicationFamilyDetailModel.Phone = model.Phone;
                    applicationFamilyDetailModel.Email = model.Email;
                    applicationFamilyDetailModel.Occupation = model.Occupation;
                    applicationFamilyDetailModel.SendMail = model.SendMail;
                    applicationFamilyDetailModel.sendSMS = model.sendSMS;
                    applicationFamilyDetailModel.AppuserKey = model.AppuserKey;
                    applicationFamilyDetailModel.IsActive = model.IsActive;
                    applicationFamilyDetailModel.IfLogin = model.IfLogin;


                    dbContext.ApplicationFamilyDetails.Add(applicationFamilyDetailModel);
                    if (model.IfLogin == true)
                    {
                        CreateGuardianUserAccount(applicationFamilyDetailModel, objviewmodel);
                    }
                    maxKey++;
                }

            }
        }
        private void UpdateFamilydetails(List<FamilyDetailsModel> modelList, ApplicationFamilyDetailsViewModel objviewmodel)
        {

            foreach (FamilyDetailsModel model in modelList)
            {
                ApplicationFamilyDetail applicationFamilyDetailModel = new ApplicationFamilyDetail();
                applicationFamilyDetailModel = dbContext.ApplicationFamilyDetails.SingleOrDefault(x => x.RowKey == model.RowKey);
                applicationFamilyDetailModel.ApplicationKey = objviewmodel.ApplicationKey;
                applicationFamilyDetailModel.FamilyMemberTypeKey = model.FamilyMemberTypeKey;
                applicationFamilyDetailModel.Name = model.Name;
                applicationFamilyDetailModel.Phone = model.Phone;
                applicationFamilyDetailModel.Email = model.Email;
                applicationFamilyDetailModel.Occupation = model.Occupation;
                applicationFamilyDetailModel.SendMail = model.SendMail;
                applicationFamilyDetailModel.sendSMS = model.sendSMS;
                applicationFamilyDetailModel.AppuserKey = model.AppuserKey;
                applicationFamilyDetailModel.IsActive = model.IsActive;
                applicationFamilyDetailModel.IfLogin = model.IfLogin;

                if (model.IfLogin == true)
                {
                    UpdateGuardianUserAccount(applicationFamilyDetailModel, objviewmodel);
                }
            }
        }


        public ApplicationFamilyDetailsViewModel DeleteApplicationFamilyDetails(Int64 ApplicationFamilyDetailsKey)
        {
            ApplicationFamilyDetailsViewModel model = new ApplicationFamilyDetailsViewModel();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    ApplicationFamilyDetail ApplicationFamilyDetails = dbContext.ApplicationFamilyDetails.SingleOrDefault(row => row.RowKey == ApplicationFamilyDetailsKey);
                    dbContext.ApplicationFamilyDetails.Remove(ApplicationFamilyDetails);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationFamilyDetails, ActionConstants.Delete, DbConstants.LogType.Info, ApplicationFamilyDetailsKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.FamilyDetails);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.ApplicationFamilyDetails, ActionConstants.Delete, DbConstants.LogType.Debug, ApplicationFamilyDetailsKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.FamilyDetails);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationFamilyDetails, ActionConstants.Delete, DbConstants.LogType.Debug, ApplicationFamilyDetailsKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }



        public void CreateGuardianUserAccount(ApplicationFamilyDetail applicationFamilyDetailModel, ApplicationFamilyDetailsViewModel model)
        {

            AppUser appUser = new AppUser();
            //var Guardian = dbContext.ApplicationFamilyDetails.SingleOrDefault(row => row.ApplicationKey == model.ApplicationKey);
            try
            {

                Int64 maxKey = dbContext.AppUsers.Select(p => p.RowKey).DefaultIfEmpty().Max();
                appUser.RowKey = Convert.ToInt32(maxKey + 1);
                appUser.AppUserName = model.UserName;
                appUser.FirstName = applicationFamilyDetailModel.Name;
                appUser.Phone1 = applicationFamilyDetailModel.Phone;
                appUser.EmailAddress = applicationFamilyDetailModel.Email;
                appUser.Password = SecurityManagement.Encrypt(model.Password);
                appUser.RoleKey = DbConstants.Role.Parents;
                appUser.IsActive = true;
                appUser.PasswordHint = model.PasswordHint;
                dbContext.AppUsers.Add(appUser);

                applicationFamilyDetailModel.AppuserKey = appUser.RowKey;

                model.Message = EduSuiteUIResources.Success;
                model.IsSuccessful = true;
            }
            catch (Exception ex)
            {
                model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.FamilyDetails);
                model.IsSuccessful = false;
            }

        }

        public void UpdateGuardianUserAccount(ApplicationFamilyDetail applicationFamilyDetailModel, ApplicationFamilyDetailsViewModel model)
        {

            AppUser appUser = new AppUser();
            //var Guardian = dbContext.ApplicationFamilyDetails.SingleOrDefault(row => row.ApplicationKey == model.ApplicationKey);
            try
            {

                appUser = dbContext.AppUsers.SingleOrDefault(row => row.RowKey == applicationFamilyDetailModel.AppuserKey);
                appUser.AppUserName = model.UserName;
                appUser.Password = SecurityManagement.Encrypt(model.Password);
                appUser.FirstName = applicationFamilyDetailModel.Name;
                appUser.Phone1 = applicationFamilyDetailModel.Phone;
                appUser.EmailAddress = applicationFamilyDetailModel.Email ?? "";

                appUser.IsActive = true;
                if (model.Password != null)
                {
                    appUser.Password = SecurityManagement.Encrypt(model.Password);
                    appUser.PasswordHint = model.PasswordHint;
                }
                model.Message = EduSuiteUIResources.Success;
                model.IsSuccessful = true;
            }
            catch (Exception ex)
            {
                model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.FamilyDetails);
                model.IsSuccessful = false;
            }

        }

        public ApplicationFamilyDetailsViewModel CheckUserNameExists(string UserName, int RowKey)
        {
            ApplicationFamilyDetailsViewModel model = new ApplicationFamilyDetailsViewModel();
            if (dbContext.AppUsers.Where(row => row.AppUserName == UserName.Trim() && row.RowKey != RowKey).Any())
            {
                model.IsSuccessful = false;

            }
            else
            {
                model.IsSuccessful = true;
            }
            return model;
        }

    }
}
