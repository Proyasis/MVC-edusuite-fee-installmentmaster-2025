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
using System.Data.SqlClient;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class ScholarshipExamScheduleService : IScholarshipExamScheduleService
    {
        private EduSuiteDatabase dbContext;

        public ScholarshipExamScheduleService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public List<ScholarshipExamScheduleViewModel> GetScholarshipExamSchedules(ScholarshipExamScheduleViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.rows;
                var skip = (model.page - 1) * model.rows;
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();



                IQueryable<ScholarshipExamScheduleViewModel> scholarshipList = (from a in dbContext.Scholarships
                                                                                join b in dbContext.ScholarshipExamSchedules on a.RowKey equals b.ScholarshipKey into abc
                                                                                from b in abc.DefaultIfEmpty()
                                                                                orderby a.RowKey
                                                                                where ((a.ScholerShipName.Contains(model.SearchName)) ||
                                                                                        (a.MiddleName ?? "").Contains(model.SearchName) || (a.LastName ?? "").Contains(model.SearchName) ||
                                                                                        a.Branch.CityName.Contains(model.SearchName))
                                                                                        && a.MobileNumber.Contains(model.SearchPhone)
                                                                                select new ScholarshipExamScheduleViewModel
                                                                                        {
                                                                                            RowKey = a.RowKey,
                                                                                            ScholarshipExamScheduleKey = b.RowKey != null ? b.RowKey : 0,
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
                                                                                            SearchScholarshipTypeKey = a.ScholarshipTypeKey ?? 0,
                                                                                            ScholarshipDate = a.DateAdded,
                                                                                            //ExamCenterKey = a.ExamCenterKey ?? 0,
                                                                                            ExamRegNo = b.ExamRegNo,
                                                                                            ExamDate = b.ExamDate ?? null,
                                                                                            ExamCentername = b.Branch.BranchName,
                                                                                            SubBranchKey = b.ExamSubCenterKey ?? 0
                                                                                        }).Where(row => (row.ScholarshipExamScheduleKey != 0) == model.ScheduleStatus);



                if (model.SearchBranchKey != null && model.SearchBranchKey != 0)
                    scholarshipList = scholarshipList.Where(row => row.BranchKey == model.SearchBranchKey);

                if (model.SearchDistrictKey != null && model.SearchDistrictKey != 0)
                    scholarshipList = scholarshipList.Where(row => row.DistrictKey == model.SearchDistrictKey);

                if (model.SearchScholarshipTypeKey != null && model.SearchScholarshipTypeKey != 0)
                    scholarshipList = scholarshipList.Where(row => row.SearchScholarshipTypeKey == model.SearchScholarshipTypeKey);

                if (model.SubBranchKey != null && model.ScheduleStatus)
                    scholarshipList = scholarshipList.Where(row => row.SubBranchKey == model.SubBranchKey);


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

                return (model.page != 0 ? scholarshipList.Skip(skip).Take(Take).ToList<ScholarshipExamScheduleViewModel>() : scholarshipList.ToList<ScholarshipExamScheduleViewModel>());
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.ScholarshipExamSchedule, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<ScholarshipExamScheduleViewModel>();

            }
        }

        private IQueryable<ScholarshipExamScheduleViewModel> SortScholarship(IQueryable<ScholarshipExamScheduleViewModel> Query, string SortName, string SortOrder)
        {

            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(ScholarshipExamScheduleViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<ScholarshipExamScheduleViewModel>(resultExpression);

        }
        public ScholarshipExamScheduleViewModel GetScholarshipExamScheduleById(long id)
        {
            ScholarshipExamScheduleViewModel model = new ScholarshipExamScheduleViewModel();
            try
            {
                model = dbContext.ScholarshipExamSchedules.Select(row => new ScholarshipExamScheduleViewModel
                {
                    RowKey = row.RowKey,
                    //ScholarShipName = row.Scholarship.ScholerShipName,
                    ExamRegNo = row.ExamRegNo,
                    ExamCenterKey = row.ExamCenterKey ?? 0,
                    ExamStartTime = row.ExamStartTime,
                    ExamEndTime = row.ExamEndTime,
                    ExamDate = row.ExamDate ?? DateTimeUTC.Now,
                    ScholarshipTypeKey = row.ScholarshipTypeKey ?? 0,
                    //ScholarshipKey = row.ScholarshipKey,

                    //DistrictKey = row.Scholarship.Branch.DistrictKey,
                    //BranchKey = row.Scholarship.BranchKey,
                    //IsActive = row.IsActive ,                    

                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new ScholarshipExamScheduleViewModel();


                }





                FillDrodownLists(model);
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.ScholarshipExamSchedule, ActionConstants.View, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                return new ScholarshipExamScheduleViewModel();


            }
        }

        public ScholarshipExamScheduleViewModel UpdateScholarshipExamSchedule(ScholarshipExamScheduleViewModel model)
        {
            FillDrodownLists(model);
            using (var transaction = dbContext.Database.BeginTransaction())
            {

                try
                {
                    ScholarshipExamSchedule scholarshipExamModel = new ScholarshipExamSchedule();

                    scholarshipExamModel = dbContext.ScholarshipExamSchedules.SingleOrDefault(x => x.RowKey == model.RowKey);
                    //scholarshipExamModel.ScholarshipKey = model.ScholarshipKey;
                    //scholarshipExamModel.ScholarshipTypeKey = model.ScholarshipTypeKey ?? 0;
                    scholarshipExamModel.ExamRegNo = model.ExamRegNo;
                    scholarshipExamModel.ExamDate = model.ExamDate;
                    scholarshipExamModel.ExamCenterKey = model.ExamCenterKey;
                    scholarshipExamModel.ExamStartTime = model.ExamStartTime;
                    scholarshipExamModel.ExamEndTime = model.ExamEndTime;


                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.ScholarshipExamSchedule, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);


                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.ScholarshipExam);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ScholarshipExamSchedule, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;
        }

        private void FillDrodownLists(ScholarshipExamScheduleViewModel model)
        {
            //FillScholarshipType(model);
            //FillDistrict(model);

            FillBranches(model);
            FillSubBranches(model);

        }
        public ScholarshipExamScheduleViewModel FillBranches(ScholarshipExamScheduleViewModel model)
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

        public ScholarshipExamScheduleViewModel FillSubBranches(ScholarshipExamScheduleViewModel model)
        {

           

            model.ExamSubCenters = dbContext.SubBranches.Where(x => x.BranchKey == model.SearchBranchKey && x.IsActive == true).OrderBy(row => row.SubBranchName).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.SubBranchName
            }).ToList();

            model.ExamSubCenters.Insert(0, new SelectListModel { RowKey = 0, Text = EduSuiteUIResources.None });

            return model;
        }


        private void FillDistrict(ScholarshipExamScheduleViewModel model)
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

        public ScholarshipExamScheduleViewModel FillSerachBranches(ScholarshipExamScheduleViewModel model)
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
        public ScholarshipExamScheduleViewModel DeleteScholarshipExamSchedule(ScholarshipExamScheduleViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    ScholarshipExamSchedule scholarship = dbContext.ScholarshipExamSchedules.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.ScholarshipExamSchedules.Remove(scholarship);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.ScholarshipExamSchedule, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.ScholarshipExam);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.ScholarshipExamSchedule, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.ScholarshipExam);
                    model.IsSuccessful = false;
                    model.IsSuccessful = false; ActivityLog.CreateActivityLog(MenuConstants.ScholarshipExamSchedule, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            //}
            return model;
        }

        public ScholarshipExamScheduleViewModel GetSearchDropDownLists(ScholarshipExamScheduleViewModel model)
        {
            FillDistrict(model);
            FillSerachBranches(model);
            FillScholarshipType(model);
            FillSubBranches(model);

            return model;
        }


        //public ScholarshipExamScheduleViewModel CreateScholarshipExamSchedule(ScholarshipExamScheduleViewModel model)
        //{
        //    using (var transaction = dbContext.Database.BeginTransaction())
        //    {
        //        try
        //        {

        //            Int64 maxKey = dbContext.ScholarshipExamSchedules.Select(p => p.RowKey).DefaultIfEmpty().Max();
        //            Int64 SerialNumbermaxKey = dbContext.ScholarshipExamSchedules.Select(p => p.SerialNumber ?? 0).DefaultIfEmpty().Max();
        //            if (SerialNumbermaxKey == 0)
        //            {
        //                SerialNumbermaxKey = dbContext.ScholarshipExamRegNoConfigs.Select(p => p.StartValue).SingleOrDefault();
        //            }
        //            foreach (ScholarshipExamScheduleDetails objmodel in model.ScholarshipExamScheduleDetails)
        //            {
        //                if (objmodel.IsActive == true)
        //                {
        //                    ScholarshipExamSchedule ScholarshipExamScheduleViewmodel = new ScholarshipExamSchedule();

        //                    ScholarshipExamScheduleViewmodel.RowKey = Convert.ToInt64(maxKey + 1);

        //                    ScholarshipExamScheduleViewmodel.ExamRegNo = dbContext.Database.SqlQuery<string>("select dbo.F_GenerateScholarshipExamRegNo(" + objmodel.BranchKey + "," + objmodel.ScholarshipTypeKey + ")").Single().ToString();
        //                    //ScholarshipExamScheduleViewmodel.SerialNumber = dbContext.Database.SqlQuery<int>("select dbo.F_GenerateScholarshipExamSerialNo(" + objmodel.BranchKey + "," + objmodel.ScholarshipTypeKey + ")").Single();
        //                    ScholarshipExamScheduleViewmodel.SerialNumber = SerialNumbermaxKey + 1;
        //                    ScholarshipExamScheduleViewmodel.ExamRegNo = ScholarshipExamScheduleViewmodel.ExamRegNo + ScholarshipExamScheduleViewmodel.SerialNumber;

        //                    ScholarshipExamScheduleViewmodel.ScholarshipKey = objmodel.ScholarshipKey ?? 0;
        //                    ScholarshipExamScheduleViewmodel.ScholarshipTypeKey = objmodel.ScholarshipTypeKey ?? 0;
        //                    ScholarshipExamScheduleViewmodel.ExamDate = objmodel.ExamDate;
        //                    ScholarshipExamScheduleViewmodel.ExamCenterKey = objmodel.ExamCenterKey;
        //                    ScholarshipExamScheduleViewmodel.ExamStartTime = objmodel.ExamStartTime;
        //                    ScholarshipExamScheduleViewmodel.ExamEndTime = objmodel.ExamEndTime;
        //                    ScholarshipExamScheduleViewmodel.IsActive = objmodel.IsActive;


        //                    dbContext.ScholarshipExamSchedules.Add(ScholarshipExamScheduleViewmodel);
        //                    maxKey++;
        //                    SerialNumbermaxKey++;
        //                }
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

        public ScholarshipExamScheduleViewModel CreateScholarshipExamSchedule(ScholarshipExamScheduleViewModel objViewModel)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    ScholarshipExamRegNoConfig ScholarshipExamRegNoConfigs = dbContext.ScholarshipExamRegNoConfigs.SingleOrDefault();
                    Int64 maxKey = dbContext.ScholarshipExamSchedules.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    Int64 SerialNumbermaxKey = dbContext.ScholarshipExamSchedules.Select(p => p.SerialNumber ?? 0).DefaultIfEmpty().Max();

                    if (SerialNumbermaxKey == 0)
                    {

                        SerialNumbermaxKey = dbContext.ScholarshipExamRegNoConfigs.Select(p => p.StartValue).SingleOrDefault();

                    }
                    foreach (long key in objViewModel.ScholarshipKeyList)
                    {
                        ScholarshipExamSchedule ScholarshipExamScheduleViewmodel = new ScholarshipExamSchedule();
                        Scholarship Scholarships = dbContext.Scholarships.SingleOrDefault(x => x.RowKey == key);
                        ScholarshipExamScheduleViewmodel.RowKey = Convert.ToInt64(maxKey + 1);

                        ScholarshipExamScheduleViewmodel.ExamRegNo = dbContext.Database.SqlQuery<string>("select dbo.F_GenerateScholarshipExamRegNo(" + Scholarships.BranchKey + "," + objViewModel.SearchScholarshipTypeKey + ")").Single().ToString();
                        //ScholarshipExamScheduleViewmodel.SerialNumber = dbContext.Database.SqlQuery<int>("select dbo.F_GenerateScholarshipExamSerialNo(" + objmodel.BranchKey + "," + objmodel.ScholarshipTypeKey + ")").Single();
                        ScholarshipExamScheduleViewmodel.SerialNumber = SerialNumbermaxKey + 1;
                        ScholarshipExamScheduleViewmodel.ExamRegNo = ScholarshipExamScheduleViewmodel.ExamRegNo + ScholarshipExamScheduleViewmodel.SerialNumber;

                        ScholarshipExamScheduleViewmodel.ScholarshipKey = key;
                        ScholarshipExamScheduleViewmodel.ScholarshipTypeKey = Scholarships.ScholarshipTypeKey ?? 0;
                        ScholarshipExamScheduleViewmodel.ExamDate = objViewModel.ExamDate;
                        ScholarshipExamScheduleViewmodel.ExamCenterKey = Scholarships.BranchKey;
                        ScholarshipExamScheduleViewmodel.ExamStartTime = objViewModel.ExamStartTime;
                        ScholarshipExamScheduleViewmodel.ExamEndTime = objViewModel.ExamEndTime;

                        ScholarshipExamScheduleViewmodel.ExamSubCenterKey = objViewModel.SubBranchKey;
                        ScholarshipExamScheduleViewmodel.IsActive = true;

                        dbContext.ScholarshipExamSchedules.Add(ScholarshipExamScheduleViewmodel);
                        maxKey++;
                        SerialNumbermaxKey++;
                        objViewModel.ScholarshipExamScheduleDetails.Add(new ScholarshipExamScheduleDetails
                            {
                                ScholarshipKey = ScholarshipExamScheduleViewmodel.ScholarshipKey,
                                ScholarShipName = Scholarships.ScholerShipName,
                                EmailAddress = Scholarships.EmailAddress,
                                MobileNumber = Scholarships.MobileNumber,
                            });
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
                    objViewModel.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.ScholarshipExam);
                    objViewModel.IsSuccessful = false;


                    ActivityLog.CreateActivityLog(MenuConstants.ScholarshipExamSchedule, ActionConstants.Edit, DbConstants.LogType.Info, null, objViewModel.Message);
                }
                return objViewModel;
            }

        }
        public void FillScholarshipType(ScholarshipExamScheduleViewModel model)
        {
            model.ScholarshipTypes = dbContext.ScholarshipTypes.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ScholarshipTypeName
            }).ToList();
        }

        



        public string GetPrintHallTicket(long? RowKey, int Type)
        {
            IEnumerable<string> results = dbContext.Database.SqlQuery<string>("exec spPrintTemplates @Type,@RowKey",
                                                                                    new SqlParameter("Type", Type),
                                                                                    new SqlParameter("RowKey", RowKey ?? 0)
                                                                                );


            return String.Join("", results);
        }

        public long getScholarshipid(ScholarshipExamScheduleViewModel model)
        {
            long Id = dbContext.ScholarshipExamSchedules.Where(row => row.Scholarship.MobileNumber == model.MobileNumber && System.Data.Entity.DbFunctions.TruncateTime(row.Scholarship.DateOfBirth) == System.Data.Entity.DbFunctions.TruncateTime(model.ScholarshipDate)).Select(x => x.ScholarshipKey).FirstOrDefault();
            return Id;
        }

        public List<ScholarshipExamScheduleViewModel> GetScholarshipExamResult(ScholarshipExamScheduleViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.rows;
                var skip = (model.page - 1) * model.rows;
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();



                IQueryable<ScholarshipExamScheduleViewModel> scholarshipList = (from a in dbContext.ScholarshipExamSchedules
                                                                                join b in dbContext.ScholarshipExamResults on a.RowKey equals b.ScholarshipExamScheduleKey into abc
                                                                                from b in abc.DefaultIfEmpty()
                                                                                orderby a.ExamRegNo ascending
                                                                                where ((a.Scholarship.ScholerShipName.Contains(model.SearchName)) ||
                                                                                        (a.Scholarship.MiddleName ?? "").Contains(model.SearchName) || (a.Scholarship.LastName ?? "").Contains(model.SearchName) ||
                                                                                        a.Branch.CityName.Contains(model.SearchName))
                                                                                        && a.Scholarship.MobileNumber.Contains(model.SearchPhone)
                                                                                select new ScholarshipExamScheduleViewModel
                                                                                {
                                                                                    RowKey = a.RowKey,
                                                                                    ScholarshipExamScheduleKey = b.RowKey != null ? b.RowKey : 0,
                                                                                    ScholarShipName = a.Scholarship.ScholerShipName + " " + (a.Scholarship.MiddleName ?? "") + " " + (a.Scholarship.LastName ?? ""),
                                                                                    EmailAddress = a.Scholarship.EmailAddress,
                                                                                    // CountryName = a.Country.CountryName,
                                                                                    //ServiceTypeName = a..ServiceTypeName,
                                                                                    ScholarShipEducationQualification = a.Scholarship.ScholerShipEducationQualification,
                                                                                    BranchName = a.Branch.BranchName,
                                                                                    LocationName = a.Branch.CityName,
                                                                                    MobileNumber = a.Scholarship.MobileNumber,
                                                                                    DistrictName = a.Branch.District.DistrictName,
                                                                                    ScholarshipTypeName = a.ScholarshipType.ScholarshipTypeName,
                                                                                    BranchKey = a.Scholarship.BranchKey,
                                                                                    DistrictKey = a.Branch.DistrictKey,
                                                                                    SearchScholarshipTypeKey = a.ScholarshipTypeKey ?? 0,
                                                                                    ScholarshipDate = a.DateAdded,
                                                                                    //ExamCenterKey = a.ExamCenterKey ?? 0,
                                                                                    ExamRegNo = a.ExamRegNo,
                                                                                    ExamDate = a.ExamDate ?? null,
                                                                                    ExamCentername = b.Scholarship.Branch.BranchName,
                                                                                    SubBranchKey = a.ExamSubCenterKey ?? 0,
                                                                                    Mark = b.Mark,
                                                                                    ResultStatus = b.ResultStatus
                                                                                });



                if (model.SearchBranchKey != null && model.SearchBranchKey != 0)
                    scholarshipList = scholarshipList.Where(row => row.BranchKey == model.SearchBranchKey);

                if (model.SearchDistrictKey != null && model.SearchDistrictKey != 0)
                    scholarshipList = scholarshipList.Where(row => row.DistrictKey == model.SearchDistrictKey);

                if (model.SearchScholarshipTypeKey != null && model.SearchScholarshipTypeKey != 0)
                    scholarshipList = scholarshipList.Where(row => row.SearchScholarshipTypeKey == model.SearchScholarshipTypeKey);

                if (model.SubBranchKey != null)
                    scholarshipList = scholarshipList.Where(row => row.SubBranchKey == model.SubBranchKey);


                if (model.SearchFromDate != null && model.SearchToDate != null)
                    scholarshipList = scholarshipList.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.ScholarshipDate) >= System.Data.Entity.DbFunctions.TruncateTime(model.SearchFromDate) && System.Data.Entity.DbFunctions.TruncateTime(row.ScholarshipDate) <= System.Data.Entity.DbFunctions.TruncateTime(model.SearchToDate));
                else if (model.SearchFromDate != null)
                    scholarshipList = scholarshipList.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.ScholarshipDate) >= System.Data.Entity.DbFunctions.TruncateTime(model.SearchFromDate));
                else if (model.SearchToDate != null)
                    scholarshipList = scholarshipList.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.ScholarshipDate) <= System.Data.Entity.DbFunctions.TruncateTime(model.SearchToDate));


                if (model.sidx != "")
                {
                    scholarshipList = SortScholarshipExamResult(scholarshipList, model.sidx, model.sord);
                }

                TotalRecords = scholarshipList.Count();

                return (model.page != 0 ? scholarshipList.Skip(skip).Take(Take).ToList<ScholarshipExamScheduleViewModel>() : scholarshipList.ToList<ScholarshipExamScheduleViewModel>());
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.ScholarshipExamSchedule, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<ScholarshipExamScheduleViewModel>();

            }
        }

        private IQueryable<ScholarshipExamScheduleViewModel> SortScholarshipExamResult(IQueryable<ScholarshipExamScheduleViewModel> Query, string SortName, string SortOrder)
        {

            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(ScholarshipExamScheduleViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<ScholarshipExamScheduleViewModel>(resultExpression);

        }

        public ScholarshipExamResultViewModel GetScholarshipExamResultDetails(ScholarshipExamResultViewModel model, List<long> ScholarshipKeys)
        {
            IQueryable<ScholarshipExamDetails> scholarshipExamResultList = (from a in dbContext.ScholarshipExamSchedules
                                                                            join b in dbContext.ScholarshipExamResults on a.RowKey equals b.ScholarshipExamScheduleKey into abc
                                                                            from b in abc.DefaultIfEmpty()
                                                                            where ScholarshipKeys.Contains(a.RowKey)
                                                                            orderby a.RowKey

                                                                            select new ScholarshipExamDetails
                                                                            {
                                                                                ScholarshipExamScheduleKey = a.RowKey,
                                                                                RowKey = b.RowKey != null ? b.RowKey : 0,
                                                                                ScholarShipName = a.Scholarship.ScholerShipName + " " + (a.Scholarship.MiddleName ?? "") + " " + (a.Scholarship.LastName ?? ""),
                                                                                //EmailAddress = a.Scholarship.EmailAddress,
                                                                                //// CountryName = a.Country.CountryName,
                                                                                ////ServiceTypeName = a..ServiceTypeName,
                                                                                //ScholarShipEducationQualification = a.Scholarship.ScholerShipEducationQualification,
                                                                                //BranchName = a.Branch.BranchName,
                                                                                //LocationName = a.Branch.CityName,
                                                                                //MobileNumber = a.Scholarship.MobileNumber,
                                                                                //DistrictName = a.Branch.District.DistrictName,
                                                                                //ScholarshipTypeName = a.ScholarshipType.ScholarshipTypeName,
                                                                                //BranchKey = a.Scholarship.BranchKey,
                                                                                //DistrictKey = a.Branch.DistrictKey,
                                                                                //SearchScholarshipTypeKey = a.ScholarshipTypeKey ?? 0,
                                                                                //ScholarshipDate = a.DateAdded,
                                                                                ////ExamCenterKey = a.ExamCenterKey ?? 0,
                                                                                ExamRegNo = a.ExamRegNo,
                                                                                //ExamDate = a.ExamDate ?? null,
                                                                                //ExamCentername = b.Scholarship.Branch.BranchName,
                                                                                //SubBranchKey = a.ExamSubCenterKey ?? 0,
                                                                                Mark = b.Mark,
                                                                                Remarks = b.Remarks,
                                                                                ResultStatus = b.ResultStatus,
                                                                                AbsentStatus = (b.ResultStatus == DbConstants.ResultStatus.Absent ? true : false),
                                                                            });

            model.ScholarshipExamDetails = scholarshipExamResultList.ToList();

            if (model.ScholarshipExamDetails.Count == 0)
            {
                model.ScholarshipExamDetails.Add(new ScholarshipExamDetails());
            }
            if (model == null)
            {
                model = new ScholarshipExamResultViewModel();
            }

            return model;

        }

        public ScholarshipExamResultViewModel UpdateScholarshipExamResult(ScholarshipExamResultViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    CreateScholarshipExamResult(model.ScholarshipExamDetails.Where(x => x.RowKey == 0).ToList(), model);
                    UpdateScholarshipExamResult(model.ScholarshipExamDetails.Where(x => x.RowKey != 0).ToList(), model);
                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationPersenol, ActionConstants.Edit, DbConstants.LogType.Info, null, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.ScholarshipExam);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationPersenol, ActionConstants.Edit, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                }
                return model;
            }
        }


        private void CreateScholarshipExamResult(List<ScholarshipExamDetails> modelList, ScholarshipExamResultViewModel Objviewmodel)
        {
            Int64 Maxkey = dbContext.ScholarshipExamResults.Select(p => p.RowKey).DefaultIfEmpty().Max();

            foreach (ScholarshipExamDetails model in modelList)
            {
                ScholarshipExamResult scholarshipexamresult = new ScholarshipExamResult();
                scholarshipexamresult.RowKey = Maxkey + 1;
                scholarshipexamresult.ScholarshipKey = model.ScholarshipKey;
                scholarshipexamresult.ScholarshipExamScheduleKey = model.ScholarshipExamScheduleKey ?? 0;
                scholarshipexamresult.Mark = model.Mark;
                scholarshipexamresult.ResultStatus = model.ResultStatus;
                scholarshipexamresult.Remarks = model.Remarks;

                dbContext.ScholarshipExamResults.Add(scholarshipexamresult);
                ++Maxkey;
            }
        }
        private void UpdateScholarshipExamResult(List<ScholarshipExamDetails> modelList, ScholarshipExamResultViewModel Objviewmodel)
        {

            foreach (ScholarshipExamDetails model in modelList)
            {
                ScholarshipExamResult scholarshipexamresult = dbContext.ScholarshipExamResults.SingleOrDefault(x => x.RowKey == model.RowKey);

                scholarshipexamresult.Mark = model.Mark;
                scholarshipexamresult.ResultStatus = model.ResultStatus;
                scholarshipexamresult.Remarks = model.Remarks;

            }
        }

        public ScholarshipExamResultViewModel UpdateBulkScholarshipExamResult(List<ScholarshipExamDetails> modelList)
        {
            ScholarshipExamResultViewModel objviewmodel = new ScholarshipExamResultViewModel();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Int64 Maxkey = dbContext.ScholarshipExamResults.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    modelList = (from item in modelList
                                 join es in dbContext.ScholarshipExamSchedules on item.ExamRegNo equals es.ExamRegNo
                                 join er in dbContext.ScholarshipExamResults on es.RowKey equals er.ScholarshipExamScheduleKey
                                 into er
                                 from erj in er.DefaultIfEmpty()
                                 select new ScholarshipExamDetails
                                 {
                                     RowKey = erj != null ? erj.RowKey : 0,
                                     ExamRegNo = item.ExamRegNo,
                                     ScholarshipKey = es.ScholarshipKey,
                                     ScholarshipExamScheduleKey = es.RowKey,
                                     Mark = item.Mark,


                                 }).ToList();
                    long i = 0;
                    foreach (ScholarshipExamDetails model in modelList)
                    {
                        //ScholarshipExamSchedule scholarshipexamschedule = dbContext.ScholarshipExamSchedules.SingleOrDefault(x => x.ExamRegNo == model.ExamRegNo);
                        //long i = 0;
                        //if (scholarshipexamschedule != null)
                        //{


                        //    ScholarshipExamResult scholarshipExamResult = dbContext.ScholarshipExamResults.SingleOrDefault(x => x.ScholarshipExamScheduleKey == scholarshipexamschedule.RowKey);
                        //    model.ScholarshipKey = scholarshipexamschedule.ScholarshipKey;
                        //    model.ScholarshipExamScheduleKey = scholarshipexamschedule.RowKey;

                        if (model.RowKey == 0)
                        {

                            model.RowKey = ++Maxkey;
                            CreateScholarshipExamResultBulk(model);

                        }
                        else
                        {
                            model.RowKey = model.RowKey;
                            UpdateScholarshipExamResultBulk(model);

                        }
                        if (i != 0 && i % 100 == 0)
                        {
                            dbContext.SaveChanges();
                        }
                        i++;
                        //}

                    }

                    dbContext.SaveChanges();
                    transaction.Commit();

                    objviewmodel.Message = EduSuiteUIResources.Success;
                    objviewmodel.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.ScholarshipExamResult, ActionConstants.Edit, DbConstants.LogType.Info, null, objviewmodel.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    objviewmodel.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.ScholarshipExam);
                    objviewmodel.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ScholarshipExamResult, ActionConstants.Edit, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                }
                return objviewmodel;
            }
        }

        private void CreateScholarshipExamResultBulk(ScholarshipExamDetails modelList)
        {


            ScholarshipExamResult scholarshipexamresult = new ScholarshipExamResult();
            scholarshipexamresult.RowKey = modelList.RowKey;
            scholarshipexamresult.ScholarshipKey = modelList.ScholarshipKey;
            scholarshipexamresult.ScholarshipExamScheduleKey = modelList.ScholarshipExamScheduleKey ?? 0;
            scholarshipexamresult.Mark = modelList.Mark;
            if (modelList.Mark == null)
            {
                scholarshipexamresult.ResultStatus = "A";
            }
            else
            {
                scholarshipexamresult.ResultStatus = "P";
            }

            scholarshipexamresult.Remarks = modelList.Remarks;

            dbContext.ScholarshipExamResults.Add(scholarshipexamresult);


        }
        private void UpdateScholarshipExamResultBulk(ScholarshipExamDetails modelList)
        {

            ScholarshipExamResult scholarshipexamresult = dbContext.ScholarshipExamResults.SingleOrDefault(x => x.RowKey == modelList.RowKey);

            scholarshipexamresult.Mark = modelList.Mark;
            if (modelList.Mark == null)
            {
                scholarshipexamresult.ResultStatus = "A";
            }
            else
            {
                scholarshipexamresult.ResultStatus = "P";
            }
            scholarshipexamresult.Remarks = modelList.Remarks;


        }
    }
}
