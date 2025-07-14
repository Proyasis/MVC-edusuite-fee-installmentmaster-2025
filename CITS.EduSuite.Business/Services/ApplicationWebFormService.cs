using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System.Configuration;
using CITS.EduSuite.Business.Models.Resources;
using System.Linq.Expressions;
using System.Data.Entity.Infrastructure;

namespace CITS.EduSuite.Business.Services
{
    public class ApplicationWebFormService : IApplicationWebFormService
    {
        private EduSuiteDatabase dbContext;

        public ApplicationWebFormService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public ApplicationWebFormViewModel GetApplicationWebFormById(ApplicationWebFormViewModel model)
        {
            ApplicationWebFormViewModel objViewModel = new ApplicationWebFormViewModel();
            try
            {
                objViewModel = dbContext.ApplicationWebForms.Where(x => x.RowKey == model.RowKey).Select(row => new ApplicationWebFormViewModel
                {
                    RowKey = row.RowKey,
                    AcademicTermKey = row.AcademicTermKey,
                    CourseTypeKey = row.Course.CourseTypeKey,
                    CourseKey = row.CourseKey,
                    UniversityKey = row.UniversityMasterKey,
                    ApplicantName = row.StudentName,
                    ApplicantGuardianName = row.StudentGuardian,
                    PermenantAddress = row.StudentPermanentAddress,
                    MobileNumber = row.StudentMobile,
                    StudentEmail = row.StudentEmail,
                    DateOfBirth = row.StudentDOB,
                    Gender = row.StudentGender ?? 0,
                    ReligionKey = row.ReligionKey,
                    NatureOfEnquiryKey = row.NatureOfEnquiryKey,
                    BranchKey = row.BranchKey,
                    AgentKey = row.AgentKey,
                    Remarks = row.Remarks,
                    SecondLanguageKey = row.SecondLanguageKey,
                    ConvertedToEnquiry = row.ConvertedToEnquiry,
                    EnquiryKey = row.EnquiryKey,
                    ConvertedToApplication = row.ConvertedToApplication,
                    ApplicationKey = row.ApplicationKey,
                    CommunityTypeKey = row.CommunityTypeKey,
                    EmployeeKey = row.EmployeeKey,
                    GuardianMobile = row.GuardianMobile,
                }).FirstOrDefault();
                if (objViewModel == null)
                {
                    objViewModel = new ApplicationWebFormViewModel();
                }

                FillDropdownLists(objViewModel);
                FillNotificationDetail(objViewModel);
                return objViewModel;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.ApplicationWebForm, ActionConstants.View, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                return new ApplicationWebFormViewModel();
            }
        }
        public ApplicationWebFormViewModel CreateApplicationWebForm(ApplicationWebFormViewModel model)
        {
            ApplicationWebForm applicationwebformModel = new ApplicationWebForm();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Int64 maxKey = dbContext.ApplicationWebForms.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    applicationwebformModel.RowKey = Convert.ToInt64(maxKey + 1);
                    applicationwebformModel.CourseKey = model.CourseKey;
                    applicationwebformModel.UniversityMasterKey = model.UniversityKey;
                    applicationwebformModel.StudentName = model.ApplicantName;
                    applicationwebformModel.StudentGuardian = model.ApplicantGuardianName;
                    applicationwebformModel.StudentPermanentAddress = model.PermenantAddress;
                    applicationwebformModel.BranchKey = model.BranchKey ?? 0;
                    applicationwebformModel.StudentMobile = model.MobileNumber;
                    applicationwebformModel.StudentEmail = model.StudentEmail;
                    applicationwebformModel.StudentGender = model.Gender;
                    applicationwebformModel.StudentDOB = Convert.ToDateTime(model.DateOfBirth);
                    applicationwebformModel.ReligionKey = model.ReligionKey;
                    applicationwebformModel.NatureOfEnquiryKey = model.NatureOfEnquiryKey;
                    applicationwebformModel.BranchKey = model.BranchKey ?? 0;
                    applicationwebformModel.AgentKey = model.AgentKey;
                    applicationwebformModel.EnquiryKey = model.EnquiryKey;
                    applicationwebformModel.ConvertedToEnquiry = model.ConvertedToEnquiry;
                    applicationwebformModel.ConvertedToApplication = model.ConvertedToApplication;
                    applicationwebformModel.ApplicationKey = model.ApplicationKey;

                    applicationwebformModel.Remarks = model.Remarks;
                    applicationwebformModel.SecondLanguageKey = model.SecondLanguageKey;
                    applicationwebformModel.AcademicTermKey = model.AcademicTermKey ?? 0;

                    applicationwebformModel.CasteKey = model.CasteKey;
                    applicationwebformModel.CommunityTypeKey = model.CommunityTypeKey;
                    applicationwebformModel.GuardianMobile = model.GuardianMobile;
                    dbContext.ApplicationWebForms.Add(applicationwebformModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;


                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationWebForm, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);
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
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Application);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationWebForm, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            FillDropdownLists(model);
            return model;
        }
        public ApplicationWebFormViewModel UpdateApplicationWebForm(ApplicationWebFormViewModel model)
        {
            ApplicationWebForm applicationwebformModel = new ApplicationWebForm();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    applicationwebformModel = dbContext.ApplicationWebForms.SingleOrDefault(row => row.RowKey == model.RowKey);

                    applicationwebformModel.CourseKey = model.CourseKey;
                    applicationwebformModel.UniversityMasterKey = model.UniversityKey;
                    applicationwebformModel.StudentName = model.ApplicantName;
                    applicationwebformModel.StudentGuardian = model.ApplicantGuardianName;
                    applicationwebformModel.StudentPermanentAddress = model.PermenantAddress;
                    applicationwebformModel.BranchKey = model.BranchKey ?? 0;
                    applicationwebformModel.StudentMobile = model.MobileNumber;
                    applicationwebformModel.StudentEmail = model.StudentEmail;
                    applicationwebformModel.StudentGender = model.Gender;
                    applicationwebformModel.StudentDOB = Convert.ToDateTime(model.DateOfBirth);
                    applicationwebformModel.ReligionKey = model.ReligionKey;
                    applicationwebformModel.NatureOfEnquiryKey = model.NatureOfEnquiryKey;
                    applicationwebformModel.BranchKey = model.BranchKey ?? 0;
                    applicationwebformModel.AgentKey = model.AgentKey;
                    applicationwebformModel.EnquiryKey = model.EnquiryKey;
                    applicationwebformModel.ConvertedToEnquiry = model.ConvertedToEnquiry;
                    applicationwebformModel.ConvertedToApplication = model.ConvertedToApplication;
                    applicationwebformModel.ApplicationKey = model.ApplicationKey;

                    applicationwebformModel.Remarks = model.Remarks;
                    applicationwebformModel.SecondLanguageKey = model.SecondLanguageKey;
                    applicationwebformModel.AcademicTermKey = model.AcademicTermKey ?? 0;

                    applicationwebformModel.CasteKey = model.CasteKey;
                    applicationwebformModel.CommunityTypeKey = model.CommunityTypeKey;
                    applicationwebformModel.GuardianMobile = model.GuardianMobile;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.RowKey = applicationwebformModel.RowKey;

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationWebForm, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Application);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationWebForm, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            FillDropdownLists(model);
            return model;
        }

        private void FillDropdownLists(ApplicationWebFormViewModel model)
        {
            FillBranches(model);
            GetEmployeesByBranchId(model);
            FillAcademicTerm(model);
            GetUniversity(model);
            GetCourseType(model);
            GetCourseByCourseType(model);
            FillAgents(model);

            FillReligions(model);
            FillNatureOfEnquiries(model);

            FillSecoundLanguage(model);

            FillCaste(model);

            FillCommunityTypes(model);

        }
        public void FillBranches(ApplicationWebFormViewModel model)
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
                }
                else
                {
                    model.Branches = BranchQuery.Where(x => x.RowKey == Employee.BranchKey).ToList();
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
        private void FillAgents(ApplicationWebFormViewModel model)
        {

            model.Agents = dbContext.Agents.Where(row => row.AgentActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AgentName
            }).ToList();
        }
        private void FillAcademicTerm(ApplicationWebFormViewModel model)
        {
            model.AcademicTerms = dbContext.VwAcadamicTermSelectActiveOnlies.Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.AcademicTermName,
            }).ToList();

        }
        private void FillReligions(ApplicationWebFormViewModel model)
        {
            model.Religions = dbContext.Religions.Where(row => row.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ReligionName
            }).ToList();
        }
        public ApplicationWebFormViewModel FillCaste(ApplicationWebFormViewModel model)
        {
            model.Caste = dbContext.Castes.Where(x => x.ReligionKey == model.ReligionKey && x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.CasteName
            }).ToList();
            return model;
        }
        private void FillSecoundLanguage(ApplicationWebFormViewModel model)
        {
            model.SecondLanguages = dbContext.VwSecoundLanguageSelectActiveOnlies.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.SecondLanguageName
            }).ToList();
        }
        private void FillNatureOfEnquiries(ApplicationWebFormViewModel model)
        {
            model.NatureOfEnquiries = dbContext.NatureOfEnquiries.Where(row => row.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.NatureOfEnquiryName
            }).ToList();
        }
        private void FillCommunityTypes(ApplicationWebFormViewModel model)
        {
            model.CommunityTypes = dbContext.CommunityTypes.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.Name
            }).ToList();
        }
        public void GetCourseType(ApplicationWebFormViewModel model)
        {
            model.CourseTypes = dbContext.CourseTypes.Where(row => row.IsActive && row.Courses.Any(x => x.UniversityCourses.Any(y => y.AcademicTermKey == model.AcademicTermKey && y.IsActive))).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.CourseTypeName
            }).ToList();
        }
        public void GetCourseByCourseType(ApplicationWebFormViewModel model)
        {
            model.Courses = dbContext.UniversityCourses.Where(row => row.Course.CourseTypeKey == model.CourseTypeKey && row.AcademicTermKey == model.AcademicTermKey && row.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.Course.RowKey,
                Text = row.Course.CourseName
            }).Distinct().ToList();
        }
        public void GetUniversity(ApplicationWebFormViewModel model)
        {
            model.Universities = dbContext.UniversityCourses.Where(row => row.CourseKey == model.CourseKey && row.AcademicTermKey == model.AcademicTermKey && row.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.UniversityMaster.RowKey,
                Text = row.UniversityMaster.UniversityMasterName
            }).Distinct().ToList();
        }
        public ApplicationWebFormViewModel GetEmployeesByBranchId(ApplicationWebFormViewModel model)
        {
            IQueryable<EmployeePersonalViewModel> EmployeesList = dbContext.Employees.Where(y => y.IsActive == true && y.EmployeeStatusKey == DbConstants.EmployeeStatus.Working).Select(x => new EmployeePersonalViewModel
            {
                FirstName = x.FirstName + " " + (x.MiddleName ?? "") + " " + x.LastName,
                RowKey = x.RowKey,
                BranchKey = x.BranchKey,
                DesignationKey = x.DesignationKey,
                AppUserKey = x.AppUserKey,
                EmployeeCode = x.EmployeeCode
            });
            var Employees = EmployeesList.ToList();
            List<long> EmployeeKeys = new List<long>();
            if (!DbConstants.Role.AdminUserTypes.Contains(DbConstants.User.RoleKey))
            {
                Employee employer = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).FirstOrDefault();
                if (employer != null)
                {
                    EmployeeKeys = dbContext.EmployeeHierarchies.Where(x => x.EmployeeKey == employer.RowKey).Select(y => y.ToEmployeeKey ?? 0).ToList();
                    EmployeeKeys.Add(employer.RowKey);
                    if (EmployeeKeys.Count > 0)
                    {
                        Employees = Employees.Where(x => EmployeeKeys.Contains(x.RowKey)).ToList();
                    }
                    else
                    {
                        Employees = Employees.Where(x => x.RowKey == employer.RowKey).ToList();
                    }
                }
            }

            if (model.BranchKey != null)
            {
                Employees = Employees.Where(x => x.BranchKey == model.BranchKey).ToList();
            }
            model.Employees = Employees.Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.FirstName
            }).ToList();

            return model;
        }
        private void FillNotificationDetail(ApplicationWebFormViewModel model)
        {
            NotificationTemplate notificationTemplateModel = dbContext.NotificationTemplates.SingleOrDefault(row => row.RowKey == DbConstants.NotificationTemplate.Application);
            if (notificationTemplateModel != null)
            {
                model.AutoEmail = notificationTemplateModel.AutoEmail;
                model.AutoSMS = notificationTemplateModel.AutoSMS;
                model.TemplateKey = notificationTemplateModel.RowKey;
            }
        }
        public ApplicationWebFormViewModel CheckPhoneExists(string MobileNumber, Int64 RowKey)
        {
            ApplicationWebFormViewModel model = new ApplicationWebFormViewModel();
            if (dbContext.ApplicationWebForms.Where(row => row.StudentMobile == MobileNumber && row.RowKey != RowKey).Any())
            {
                model.IsSuccessful = false;
            }
            else
            {
                model.IsSuccessful = true;
            }
            return model;
        }

        public List<ApplicationWebFormViewModel> GetApplicationWebForm(ApplicationWebFormViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;

                IQueryable<ApplicationWebFormViewModel> applicationList = (from a in dbContext.ApplicationWebForms
                                                                           where (a.StudentName.Contains(model.ApplicantName) || a.Course.CourseName.Contains(model.ApplicantName) ||
                                                                           a.UniversityMaster.UniversityMasterName.Contains(model.ApplicantName)
                                                                           || a.StudentMobile.Contains(model.ApplicantName))
                                                                           select new ApplicationWebFormViewModel
                                                                           {
                                                                               RowKey = a.RowKey,
                                                                               ApplicantName = a.StudentName,
                                                                               AcademicTermName = a.AcademicTerm.AcademicTermName,
                                                                               CourseName = a.Course.CourseName,
                                                                               UniversityName = a.UniversityMaster.UniversityMasterName,
                                                                               MobileNumber = a.StudentMobile,
                                                                               ConvertedToEnquiry = a.ConvertedToEnquiry,
                                                                               ConvertedToApplication = a.ConvertedToApplication,
                                                                               BranchKey = a.BranchKey,
                                                                               BranchName = dbContext.vwBranchSelectActiveOnlies.Where(x => x.RowKey == a.BranchKey).Select(y => y.BranchName).FirstOrDefault(),
                                                                               AcademicTermKey = a.AcademicTermKey,
                                                                               Gender = a.StudentGender ?? 0,
                                                                               CreatedDate = a.DateAdded,
                                                                               EnquiryStatusName = dbContext.Enquiries.Where(x => x.MobileNumber == a.StudentMobile).Select(y => y.EnquiryStatu.EnquiryStatusName).FirstOrDefault(),
                                                                               EnquiryStatusKey = dbContext.Enquiries.Where(x => x.MobileNumber == a.StudentMobile).Select(y => y.EnquiryStatusKey ?? 0).FirstOrDefault(),
                                                                           });
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
                if (Employee != null)
                {
                    if (Employee.BranchAccess != null)
                    {
                        var Branches = Employee.BranchAccess.Split(',').Select(Int16.Parse).ToList();
                        applicationList = applicationList.Where(row => Branches.Contains(row.BranchKey ?? 0));
                    }
                }

                if (model.BranchKey != 0)
                {
                    applicationList = applicationList.Where(row => row.BranchKey == model.BranchKey);
                }
                if (model.WebFormStatusKey != 0)
                {
                    applicationList = applicationList.Where(row => row.ConvertedToApplication == (model.WebFormStatusKey == 1 ? true : false));
                }
                if (model.EnquiryStatusKey != null)
                {
                    applicationList = applicationList.Where(row => row.EnquiryStatusKey == model.EnquiryStatusKey);
                }

                applicationList = applicationList.GroupBy(x => x.RowKey).Select(y => y.FirstOrDefault());
                if (model.SortBy != "")
                {
                    applicationList = SortApplications(applicationList, model.SortBy, model.SortOrder);
                }
                TotalRecords = applicationList.Count();
                return applicationList.Skip(Skip).Take(Take).ToList<ApplicationWebFormViewModel>();
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.ApplicationWebForm, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<ApplicationWebFormViewModel>();

            }
        }
        private IQueryable<ApplicationWebFormViewModel> SortApplications(IQueryable<ApplicationWebFormViewModel> Query, string SortName, string SortOrder)
        {
            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(ApplicationWebFormViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<ApplicationWebFormViewModel>(resultExpression);
        }

        public ApplicationWebFormViewModel DeleteApplicationWebForm(long Id)
        {
            ApplicationWebFormViewModel applicationPersonaModel = new ApplicationWebFormViewModel();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    ApplicationWebForm applicationWebForm = dbContext.ApplicationWebForms.SingleOrDefault(row => row.RowKey == Id);
                    dbContext.ApplicationWebForms.Remove(applicationWebForm);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    applicationPersonaModel.Message = EduSuiteUIResources.Success;
                    applicationPersonaModel.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationWebForm, ActionConstants.Delete, DbConstants.LogType.Info, Id, applicationPersonaModel.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        applicationPersonaModel.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.ApplicationWebForm);
                        applicationPersonaModel.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.ApplicationWebForm, ActionConstants.Delete, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    applicationPersonaModel.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.ApplicationWebForm);
                    applicationPersonaModel.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationWebForm, ActionConstants.Delete, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);
                }
            }
            return applicationPersonaModel;
        }

        public void FillWebFormStatus(ApplicationWebFormViewModel model)
        {
            model.WebFormStatus.Add(new SelectListModel { RowKey = 1, Text = "Converted to Application" });
            model.WebFormStatus.Add(new SelectListModel { RowKey = 2, Text = "Pending" });
        }

        public void FillWebEnquiryStatus(ApplicationWebFormViewModel model)
        {
            model.WebEnquiryStatus = dbContext.EnquiryStatus.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.EnquiryStatusName
            }).ToList();

            model.WebEnquiryStatus.Add(new SelectListModel { RowKey = 0, Text = "Not in Enquiry" });
        }
    }
}
