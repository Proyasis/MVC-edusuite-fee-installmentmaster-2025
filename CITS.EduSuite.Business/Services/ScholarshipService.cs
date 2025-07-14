using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using CITS.EduSuite.Business.Models.Security;
using System.Threading;
using System.Linq.Expressions;
using System.Data.Entity.Infrastructure;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class ScholarshipService : IScholarshipService
    {
        private EduSuiteDatabase dbContext;

        public ScholarshipService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public List<ScholarshipViewModel> GetScholarships(ScholarshipViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.rows;
                var skip = (model.page - 1) * model.rows;
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();



                IQueryable<ScholarshipViewModel> scholarshipList = (from a in dbContext.Scholarships
                                                                    //orderby e.RowKey
                                                                    where ((a.ScholerShipName.Contains(model.SearchName)) ||
                                                                            (a.MiddleName ?? "").Contains(model.SearchName) || (a.LastName ?? "").Contains(model.SearchName) ||
                                                                            a.Branch.CityName.Contains(model.SearchName)
                                                                            ) && a.MobileNumber.Contains(model.SearchPhone)
                                                                    select new ScholarshipViewModel
                                                                            {
                                                                                RowKey = a.RowKey,
                                                                                ScholarShipName = a.ScholerShipName + " " + (a.MiddleName ?? "") + " " + (a.LastName ?? ""),
                                                                                EmailAddress = a.EmailAddress,
                                                                                // CountryName = a.Country.CountryName,
                                                                                //ServiceTypeName = a..ServiceTypeName,
                                                                                ScholarShipEducationQualification = a.ScholerShipEducationQualification,
                                                                                BranchName = a.Branch.BranchName,
                                                                                LocationName = a.Branch.CityName,
                                                                                MobileNumber = a.MobileNumber,
                                                                                DistrictName = a.Branch.District.DistrictName,
                                                                                ScholarshipTypeName = a.ScholarshipType.ScholarshipTypeName,
                                                                                BranchKey = a.BranchKey,
                                                                                DistrictKey = a.Branch.DistrictKey,
                                                                                ScholarshipTypeKey = a.ScholarshipTypeKey ?? 0,
                                                                                ScholarshipDate = a.DateAdded,
                                                                                EnquiryKey = a.EnquiryKey ?? 0
                                                                            });



                if (model.SearchBranchKey != null && model.SearchBranchKey != 0)
                    scholarshipList = scholarshipList.Where(row => row.BranchKey == model.SearchBranchKey);

                if (model.SearchDistrictKey != null && model.SearchDistrictKey != 0)
                    scholarshipList = scholarshipList.Where(row => row.DistrictKey == model.SearchDistrictKey);

                if (model.SearchScholarshipTypeKey != null && model.SearchScholarshipTypeKey != 0)
                    scholarshipList = scholarshipList.Where(row => row.ScholarshipTypeKey == model.SearchScholarshipTypeKey);


                if (model.SearchFromDate != null && model.SearchToDate != null)
                    scholarshipList = scholarshipList.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.ScholarshipDate) >= System.Data.Entity.DbFunctions.TruncateTime(model.SearchFromDate) && System.Data.Entity.DbFunctions.TruncateTime(row.ScholarshipDate) <= System.Data.Entity.DbFunctions.TruncateTime(model.SearchToDate));
                else if (model.SearchFromDate != null)
                    scholarshipList = scholarshipList.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.ScholarshipDate) >= System.Data.Entity.DbFunctions.TruncateTime(model.SearchFromDate));
                else if (model.SearchToDate != null)
                    scholarshipList = scholarshipList.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.ScholarshipDate) <= System.Data.Entity.DbFunctions.TruncateTime(model.SearchToDate));


                if (model.sidx != "")
                {
                    scholarshipList = SortScholarship(scholarshipList, model.sidx, model.sord);
                }

                TotalRecords = scholarshipList.Count();

                return scholarshipList.Skip(skip).Take(Take).ToList<ScholarshipViewModel>();
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.Scholarship, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<ScholarshipViewModel>();

            }
        }

        private IQueryable<ScholarshipViewModel> SortScholarship(IQueryable<ScholarshipViewModel> Query, string SortName, string SortOrder)
        {

            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(ScholarshipViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<ScholarshipViewModel>(resultExpression);

        }
        public ScholarshipViewModel GetScholarshipById(ScholarshipViewModel model)
        {
            ScholarshipViewModel objViewModel = new ScholarshipViewModel();
            try
            {
                objViewModel = dbContext.Scholarships.Where(row => row.RowKey == model.RowKey).Select(row => new ScholarshipViewModel
                {
                    RowKey = row.RowKey,
                    ScholarShipName = row.ScholerShipName,
                    MiddleName = row.MiddleName,
                    LastName = row.LastName,
                    MobileNumber = row.MobileNumber,
                    MobileNumberOptional = row.MobileNumberOptional,
                    EmailAddress = row.EmailAddress,
                    ScholarShipEducationQualification = row.ScholerShipEducationQualification,
                    DateOfBirth = row.DateOfBirth ?? DateTimeUTC.Now,
                    ScholarshipTypeKey = row.ScholarshipTypeKey ?? 0,
                    DistrictKey = row.Branch.DistrictKey,
                    BranchKey = row.BranchKey,
                    Gender = row.Gender ?? 0,
                    LocationName = row.LocationName,
                    Remarks = row.Remarks,

                }).SingleOrDefault();
                if (objViewModel == null)
                {
                    objViewModel = new ScholarshipViewModel();


                }
                
                




                FillDrodownLists(objViewModel);
                return objViewModel;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Scholarship, ActionConstants.View, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                return new ScholarshipViewModel();


            }
        }

        public ScholarshipViewModel CreateScholarship(ScholarshipViewModel model)
        {
            FillDrodownLists(model);
            using (var transaction = dbContext.Database.BeginTransaction())
            {

                try
                {
                    Scholarship scholarshipModel = new Scholarship();
                    Int64 maxKey = dbContext.Scholarships.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    scholarshipModel.RowKey = Convert.ToInt64(maxKey + 1);
                    scholarshipModel.ScholarshipTypeKey = model.ScholarshipTypeKey;
                    scholarshipModel.ScholerShipName = model.ScholarShipName;
                    scholarshipModel.MiddleName = model.MiddleName;
                    scholarshipModel.LastName = model.LastName;
                    scholarshipModel.EmailAddress = model.EmailAddress;
                    scholarshipModel.MobileNumber = model.MobileNumber;
                    scholarshipModel.MobileNumberOptional = model.MobileNumberOptional;
                    scholarshipModel.Gender = model.Gender;
                    scholarshipModel.ScholerShipEducationQualification = model.ScholarShipEducationQualification;
                    scholarshipModel.DateOfBirth = model.DateOfBirth;
                    scholarshipModel.BranchKey = model.BranchKey;

                    scholarshipModel.LocationName = model.LocationName;
                    scholarshipModel.Remarks = model.Remarks;
                    dbContext.Scholarships.Add(scholarshipModel);


                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.Scholarship, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);


                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Scholarship);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Scholarship, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;
        }
        public ScholarshipViewModel UpdateScholarship(ScholarshipViewModel model)
        {
            FillDrodownLists(model);
            using (var transaction = dbContext.Database.BeginTransaction())
            {

                try
                {

                    Scholarship scholarshipModel = dbContext.Scholarships.SingleOrDefault(row => row.RowKey == model.RowKey);
                    scholarshipModel.ScholarshipTypeKey = model.ScholarshipTypeKey;
                    scholarshipModel.ScholerShipName = model.ScholarShipName;
                    scholarshipModel.MiddleName = model.MiddleName;
                    scholarshipModel.LastName = model.LastName;
                    scholarshipModel.EmailAddress = model.EmailAddress;
                    scholarshipModel.MobileNumber = model.MobileNumber;
                    scholarshipModel.MobileNumberOptional = model.MobileNumberOptional;
                    scholarshipModel.Gender = model.Gender;
                    scholarshipModel.ScholerShipEducationQualification = model.ScholarShipEducationQualification;
                    scholarshipModel.DateOfBirth = model.DateOfBirth;
                    scholarshipModel.BranchKey = model.BranchKey;

                    scholarshipModel.LocationName = model.LocationName;
                    scholarshipModel.Remarks = model.Remarks;




                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Scholarship, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Scholarship);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Scholarship, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        private void FillDrodownLists(ScholarshipViewModel model)
        {
            FillScholarshipType(model);
            FillDistrict(model);

            FillBranches(model);

        }
        public ScholarshipViewModel FillBranches(ScholarshipViewModel model)
        {

            IQueryable<SelectListModel> BranchQuery = dbContext.vwBranchSelectActiveOnlies.Where(x => x.DistrictKey == model.DistrictKey).OrderBy(row => row.BranchName).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BranchName
            });


            if (!DbConstants.Role.AdminUserTypes.Contains(DbConstants.User.RoleKey))
            {
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
                if (Employee != null)
                {
                    List<long> branches = Employee.BranchAccess.ToString().Split(',').Select(Int64.Parse).ToList();
                    model.Branches = BranchQuery.Where(x => branches.Contains(x.RowKey)).ToList();
                    //model.SearchBranchKey = model.BranchKey = Employee.BranchKey;
                }
                else
                {
                    model.Branches = BranchQuery.ToList();
                }
            }
            else
            {

                model.Branches = BranchQuery.ToList();
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
                //if (Employee != null)
                //{
                //    model.SearchBranchKey = model.BranchKey = Employee.BranchKey;
                //}
            }
            if (model.Branches.Count == 1)
            {
                long? branchkey = model.Branches.Select(x => x.RowKey).FirstOrDefault();
                model.BranchKey = Convert.ToInt16(branchkey);

            }
            return model;
        }

        private void FillScholarshipType(ScholarshipViewModel model)
        {
            model.ScholarshipTypes = dbContext.ScholarshipTypes.Where(x => x.IsActive == true).Select(row => new SelectListModel
                {
                    RowKey = row.RowKey,
                    Text = row.ScholarshipTypeName
                }).ToList();
        }
        private void FillDistrict(ScholarshipViewModel model)
        {
            //model.Districts = dbContext.Branches.Where(x => x.IsActive == true && x.District.IsActive == true).Select(row => new SelectListModel
            //{
            //    RowKey = row.DistrictKey,
            //    Text = row.District.DistrictName
            //}).Distinct().ToList();

            IQueryable<SelectListModel> DistrictQuery = dbContext.Branches.Where(x => x.IsActive == true && x.District.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.DistrictKey,
                Text = row.District.DistrictName
            });


            if (!DbConstants.Role.AdminUserTypes.Contains(DbConstants.User.RoleKey))
            {
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
                if (Employee != null)
                {
                    model.Districts = DistrictQuery.Where(x => x.RowKey == Employee.Branch.DistrictKey).Distinct().ToList();
                    model.SearchDistrictKey = model.DistrictKey = Employee.Branch.DistrictKey;
                }
                else
                {
                    model.Districts = DistrictQuery.Distinct().ToList();
                }
            }
            else
            {

                model.Districts = DistrictQuery.Distinct().ToList();
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
                if (Employee != null)
                {
                    model.Districts = DistrictQuery.Where(x => x.RowKey == Employee.Branch.DistrictKey).Distinct().ToList();
                    model.SearchDistrictKey = model.DistrictKey = Employee.Branch.DistrictKey;
                }
            }
        }

        public ScholarshipViewModel FillSerachBranches(ScholarshipViewModel model)
        {


            IQueryable<SelectListModel> BranchQuery = dbContext.vwBranchSelectActiveOnlies.OrderBy(row => row.BranchName).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BranchName
            });

            if (model.SearchDistrictKey != null && model.SearchDistrictKey != 0)
            {
                IQueryable<SelectListModel> BranchQuereis = dbContext.vwBranchSelectActiveOnlies.Where(x => x.DistrictKey == model.SearchDistrictKey).OrderBy(row => row.BranchName).Select(row => new SelectListModel
                {
                    RowKey = row.RowKey,
                    Text = row.BranchName
                });

                BranchQuery = BranchQuereis;
            }


            if (!DbConstants.Role.AdminUserTypes.Contains(DbConstants.User.RoleKey))
            {
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
                if (Employee != null)
                {
                    if (Employee.BranchAccess != null)
                    {
                        List<long> Branches = Employee.BranchAccess.Split(',').Select(Int64.Parse).ToList();
                        model.Branches = BranchQuery.Where(row => Branches.Contains(row.RowKey)).ToList();
                        //model.SearchBranchKey = model.BranchKey = Employee.BranchKey;

                    }
                    else
                    {
                        model.Branches = BranchQuery.Where(x => x.RowKey == Employee.BranchKey).ToList();
                        //model.SearchBranchKey = model.BranchKey = Employee.BranchKey;
                    }
                }
                else
                {
                    model.Branches = BranchQuery.ToList();
                }
            }
            else
            {

                model.Branches = BranchQuery.ToList();
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
                if (Employee != null)
                {
                    if (Employee.BranchAccess != null)
                    {
                        List<long> Branches = Employee.BranchAccess.Split(',').Select(Int64.Parse).ToList();
                        model.Branches = BranchQuery.Where(row => Branches.Contains(row.RowKey)).ToList();
                        //model.SearchBranchKey = model.BranchKey = Employee.BranchKey;

                    }
                    else
                    {
                        model.Branches = BranchQuery.Where(x => x.RowKey == Employee.BranchKey).ToList();
                        //model.SearchBranchKey = model.BranchKey = Employee.BranchKey;
                    }
                }
            }
            if (model.Branches.Count == 1)
            {
                long? branchkey = model.Branches.Select(x => x.RowKey).FirstOrDefault();
                model.BranchKey = Convert.ToInt16(branchkey);

            }
            return model;

        }
        public ScholarshipViewModel DeleteScholarship(ScholarshipViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Scholarship scholarship = dbContext.Scholarships.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.Scholarships.Remove(scholarship);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Scholarship, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Scholarship);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.Scholarship, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Scholarship);
                    model.IsSuccessful = false;
                    model.IsSuccessful = false; ActivityLog.CreateActivityLog(MenuConstants.Scholarship, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            //}
            return model;
        }

        public ScholarshipViewModel GetSearchDropDownLists(ScholarshipViewModel model)
        {
            FillDistrict(model);
            FillSerachBranches(model);
            FillScholarshipType(model);

            return model;
        }

        public ScholarshipViewModel MoveToEnquiry(List<long> ScholarShipKeys, ScholarshipViewModel objViewModel)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    List<EnquiryViewModel> enquirymodel = new List<EnquiryViewModel>();
                    enquirymodel = dbContext.Scholarships.Where(x => ScholarShipKeys.Contains(x.RowKey)).Select(row => new EnquiryViewModel
                    {
                        ScholershipKey = row.RowKey,
                        EnquiryName = row.ScholerShipName,
                        MobileNumber = row.MobileNumber,
                        MobileNumberOptional = row.MobileNumberOptional,
                        EmailAddress = row.EmailAddress,
                        EnquiryEducationQualification = row.ScholerShipEducationQualification,
                        DateOfBirth = row.DateOfBirth,
                        BranchKey = row.BranchKey,
                       // DepartmentKey = row.Branch.BranchDepartments.Select(x => x.DepartmentKey).FirstOrDefault(),
                        NatureOfEnquiryKey = DbConstants.NatureOfEnquiry.Scholership,
                        EnquiryStatusKey = DbConstants.EnquiryStatus.FollowUp,
                        Gender = row.Gender ?? 0,
                        DistrictName = row.Branch.District.DistrictName,
                        LocationName = row.LocationName,
                        Remarks = row.Remarks,
                    }).ToList();
                    Int64 maxKey = dbContext.Enquiries.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    foreach (EnquiryViewModel model in enquirymodel)
                    {

                        Enquiry enquiryModel = new Enquiry();

                        enquiryModel.RowKey = Convert.ToInt64(maxKey + 1);
                        //enquiryModel.AcademicTermKey = model.AcademicTermKey;
                        //enquiryModel.CourseKey = model.CourseKey;
                        //enquiryModel.UniversityKey = model.UniversityKey;
                        enquiryModel.NatureOfEnquiryKey = model.NatureOfEnquiryKey;
                        enquiryModel.EnquiryName = model.EnquiryName;
                        enquiryModel.EmailAddress = model.EmailAddress;
                        //enquiryModel.TelephoneCodeKey = model.TelephoneCodeKey;
                        enquiryModel.MobileNumber = model.MobileNumber;
                        //enquiryModel.TelePhoneCodeOptionalkey = model.TelephoneCodeOptionalKey;
                        //enquiryModel.MobileNumberOptional = model.MobileNumberOptional;
                        //enquiryModel.PhoneNumber = model.PhoneNumber;
                        enquiryModel.Gender = model.Gender;
                        enquiryModel.EnquiryEducationQualification = model.EnquiryEducationQualification;
                        enquiryModel.DateOfBirth = model.DateOfBirth;
                        enquiryModel.BranchKey = model.BranchKey;
                        //enquiryModel.DepartmentKey = model.DepartmentKey;
                        enquiryModel.IsProcessed = false;
                        enquiryModel.EnquiryStatusKey = model.EnquiryStatusKey;
                        enquiryModel.DistrictName = model.DistrictName;
                        enquiryModel.LocationName = model.LocationName;
                        enquiryModel.Remarks = model.Remarks;
                        //enquiryModel.IsSpam = false;

                        Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();

                        if (Employee != null)// && Employee.DepartmentKey == enquiryModel.DepartmentKey)
                        {
                            enquiryModel.EmployeeKey = Employee.RowKey;
                        }

                        dbContext.Enquiries.Add(enquiryModel);
                        maxKey++;

                        Scholarship scholarshipModel = dbContext.Scholarships.SingleOrDefault(row => row.RowKey == model.ScholershipKey);
                        scholarshipModel.EnquiryKey = enquiryModel.RowKey;

                    }


                    dbContext.SaveChanges();
                    transaction.Commit();
                    objViewModel.Message = EduSuiteUIResources.Success;
                    objViewModel.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.Scholarship, ActionConstants.Edit, DbConstants.LogType.Info, null, objViewModel.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    objViewModel.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Scholarship);
                    objViewModel.IsSuccessful = false;


                    ActivityLog.CreateActivityLog(MenuConstants.Scholarship, ActionConstants.Edit, DbConstants.LogType.Info, null, objViewModel.Message);
                }
                return objViewModel;
            }

        }

        //public ScholarshipExamScheduleViewModel UpdateScholarshipExamSchedule(ScholarshipExamScheduleViewModel model)
        //{
        //    using (var transaction = dbContext.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            List<ScholarshipExamScheduleViewModel> scholarshipExammodel = new List<ScholarshipExamScheduleViewModel>();
        //            scholarshipExammodel = dbContext.Scholarships.Where(x => x.ScholarshipTypeKey == model.ScholarshipTypeKey).Select(row => new ScholarshipExamScheduleViewModel
        //            {
        //                ScholarshipKey = row.RowKey,
        //                ScholarshipTypeKey = row.ScholarshipTypeKey,

        //            }).ToList();
        //            Int64 maxKey = dbContext.ScholarshipExamSchedules.Select(p => p.RowKey).DefaultIfEmpty().Max();
        //            foreach (ScholarshipExamScheduleViewModel objmodel in scholarshipExammodel)
        //            {

        //                ScholarshipExamSchedule ScholarshipExamScheduleViewmodel = new ScholarshipExamSchedule();

        //                ScholarshipExamScheduleViewmodel.RowKey = Convert.ToInt64(maxKey + 1);
        //                //enquiryModel.AcademicTermKey = model.AcademicTermKey;
        //                //enquiryModel.CourseKey = model.CourseKey;
        //                //enquiryModel.UniversityKey = model.UniversityKey;
        //                ScholarshipExamScheduleViewmodel.ScholarshipKey = objmodel.ScholarshipKey??0;
        //                ScholarshipExamScheduleViewmodel.ScholarshipTypeKey = objmodel.ScholarshipTypeKey ?? 0;
        //                ScholarshipExamScheduleViewmodel.ExamRegNo = objmodel.ExamRegNo ;
        //                ScholarshipExamScheduleViewmodel.ExamDate = objmodel.ExamDate;
        //                ScholarshipExamScheduleViewmodel.ExamCenter = objmodel.ExamCenter;
        //                ScholarshipExamScheduleViewmodel.ExamStartTime = objmodel.ExamStartTime;
        //                ScholarshipExamScheduleViewmodel.ExamEndTime = objmodel.ExamEndTime;

        //                dbContext.ScholarshipExamSchedules.Add(ScholarshipExamScheduleViewmodel);
        //                maxKey++;
        //            }




        //            dbContext.SaveChanges();
        //            transaction.Commit();
        //            model.Message = EduSuiteUIResources.Success;
        //            model.IsSuccessful = true;

        //            ActivityLog.CreateActivityLog(MenuConstants.Scholarship, ActionConstants.Edit, DbConstants.LogType.Info, null, model.Message);
        //        }
        //        catch (Exception ex)
        //        {
        //            transaction.Rollback();
        //            model.Message = EduSuiteUIResources.FailedToSaveScholarship;
        //            model.IsSuccessful = false;


        //            ActivityLog.CreateActivityLog(MenuConstants.Scholarship, ActionConstants.Edit, DbConstants.LogType.Info, null, model.Message);
        //        }
        //        return model;
        //    }
        //}

        public void FillScholarshipType(ScholarshipExamScheduleViewModel model)
        {
            model.ScholarshipTypes = dbContext.ScholarshipTypes.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ScholarshipTypeName
            }).ToList();
        }


    }
}
