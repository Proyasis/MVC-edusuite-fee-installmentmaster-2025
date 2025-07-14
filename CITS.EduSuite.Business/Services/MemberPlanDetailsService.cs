using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;
using System.Linq.Expressions;

namespace CITS.EduSuite.Business.Services
{
    public class MemberPlanDetailsService : IMemberPlanDetailsService
    {
        private EduSuiteDatabase dbContext;
        public MemberPlanDetailsService(EduSuiteDatabase objDb)
        {
            dbContext = objDb;
        }
        public List<MemberPlanDetailsViewModel> GetMemberPlanDetails(MemberPlanDetailsViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;


                //IQueryable<MemberPlanDetailsViewModel> MemberPlanDetailsList = (from a in dbContext.MemberPlanDetails
                //                                                                join AP in dbContext.Applications on new { a.ApplicationTypeKey, a.ApplicationKey } equals new { ApplicationTypeKey = DbConstants.ApplicationType.Student, ApplicationKey = AP.RowKey } into APJ
                //                                                                from AP in APJ.DefaultIfEmpty()
                //                                                                join EP in dbContext.Employees on new { a.ApplicationTypeKey, a.ApplicationKey } equals new { ApplicationTypeKey = DbConstants.ApplicationType.Staff, ApplicationKey = EP.RowKey } into EPJ
                //                                                                from EP in EPJ.DefaultIfEmpty()
                //                                                                join MR in dbContext.MemberRegistrations on new { a.ApplicationTypeKey, a.ApplicationKey } equals new { ApplicationTypeKey = DbConstants.ApplicationType.Other, ApplicationKey = MR.RowKey } into MRJ
                //                                                                from MR in MRJ.DefaultIfEmpty()
                //                                                                where a.CardId.Contains(model.SearchText)// || a.ApplicationTypeKey == DbConstants.ApplicationType.Student ? (AP.StudentName.Contains(model.SearchText)) : a.ApplicationTypeKey == DbConstants.ApplicationType.Staff ? ((EP.FirstName + " " + EP.LastName).Contains(model.SearchText)) : ((MR.MemberFirstName + " " + MR.MemberLastName).Contains(model.SearchText))
                //                                                                select new MemberPlanDetailsViewModel
                //                                                                {
                //                                                                    RowKey = a.RowKey,
                //                                                                    MemberTypeName = a.MemberType.MemberTypeName,
                //                                                                    BorrowerTypeName = a.BorrowerType.BorrowerTypeName,
                //                                                                    MemberFullName = a.ApplicationTypeKey == DbConstants.ApplicationType.Student ? AP.StudentName : a.ApplicationTypeKey == DbConstants.ApplicationType.Staff ? (EP.FirstName + " " + EP.LastName) : (MR.MemberFirstName + " " + MR.MemberLastName),

                //                                                                    ApplicationTypeName = a.ApplicationType.ApplicationTypeName,
                //                                                                    BranchKey = a.BranchKey,
                //                                                                    CardId = a.CardId,
                //                                                                    ApplicationTypeKey = a.ApplicationTypeKey
                //                                                                });


                IQueryable<MemberPlanDetailsViewModel> MemberPlanDetailsList = (from a in dbContext.VW_MemberPlanDetails

                                                                                where a.CardId.Contains(model.SearchText)// || a.ApplicationTypeKey == DbConstants.ApplicationType.Student ? (AP.StudentName.Contains(model.SearchText)) : a.ApplicationTypeKey == DbConstants.ApplicationType.Staff ? ((EP.FirstName + " " + EP.LastName).Contains(model.SearchText)) : ((MR.MemberFirstName + " " + MR.MemberLastName).Contains(model.SearchText))
                                                                                select new MemberPlanDetailsViewModel
                                                                                {
                                                                                    RowKey = a.RowKey,
                                                                                    MemberTypeName = a.MemberTypeName,
                                                                                    BorrowerTypeName = a.BorrowerTypeName,
                                                                                    MemberFullName = a.MemberName,
                                                                                    ApplicationTypeName = a.ApplicationTypeName,
                                                                                    BranchKey = a.BranchKey,
                                                                                    CardId = a.CardId,
                                                                                    ApplicationTypeKey = a.ApplicationTypeKey
                                                                                });

                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
                if (Employee != null)
                {
                    if (Employee.BranchAccess != null)
                    {
                        var Branches = Employee.BranchAccess.Split(',').Select(Int16.Parse).ToList();
                        MemberPlanDetailsList = MemberPlanDetailsList.Where(row => Branches.Contains(row.BranchKey));
                    }
                }

                if (model.BranchKey != 0)
                {
                    MemberPlanDetailsList = MemberPlanDetailsList.Where(x => x.BranchKey == model.BranchKey);
                }
                MemberPlanDetailsList = MemberPlanDetailsList.GroupBy(x => x.RowKey).Select(y => y.FirstOrDefault());
                if (model.SortBy != "")
                {
                    MemberPlanDetailsList = SortApplications(MemberPlanDetailsList, model.SortBy, model.SortOrder);
                }
                TotalRecords = MemberPlanDetailsList.Count();
                return MemberPlanDetailsList.OrderBy(Row => Row.MemberFullName).Skip(Skip).Take(Take).ToList<MemberPlanDetailsViewModel>();
                //return MemberPlanDetailsList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<MemberPlanDetailsViewModel>();
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.MemberPlanDetails, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<MemberPlanDetailsViewModel>();


            }
        }
        private IQueryable<MemberPlanDetailsViewModel> SortApplications(IQueryable<MemberPlanDetailsViewModel> Query, string SortName, string SortOrder)
        {

            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(MemberPlanDetailsViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<MemberPlanDetailsViewModel>(resultExpression);

        }

        public MemberPlanDetailsViewModel GetMemberPlanDetailsById(long? id)
        {
            MemberPlanDetailsViewModel model = new MemberPlanDetailsViewModel();
            model = (from a in dbContext.MemberPlanDetails
                     join AP in dbContext.Applications on new { a.ApplicationTypeKey, a.ApplicationKey } equals new { ApplicationTypeKey = DbConstants.ApplicationType.Student, ApplicationKey = AP.RowKey } into APJ
                     from AP in APJ.DefaultIfEmpty()
                     join EP in dbContext.Employees on new { a.ApplicationTypeKey, a.ApplicationKey } equals new { ApplicationTypeKey = DbConstants.ApplicationType.Staff, ApplicationKey = EP.RowKey } into EPJ
                     from EP in EPJ.DefaultIfEmpty()
                     join MR in dbContext.MemberRegistrations on new { a.ApplicationTypeKey, a.ApplicationKey } equals new { ApplicationTypeKey = DbConstants.ApplicationType.Other, ApplicationKey = MR.RowKey } into MRJ
                     from MR in MRJ.DefaultIfEmpty()
                     select new MemberPlanDetailsViewModel
                     {
                         RowKey = a.RowKey,
                         MemberTypeKey = a.MemberTypeKey,
                         BorrowerTypeKey = a.BorrowerTypeKey,
                         CardId = a.CardId,
                         CardSerialNo = a.CardSerialNo,
                         IsBlockMember = a.IsBlockMember,
                         ApplicationTypeKey = a.ApplicationTypeKey,
                         MemberFullName = a.ApplicationTypeKey == DbConstants.ApplicationType.Student ? AP.StudentName : a.ApplicationTypeKey == DbConstants.ApplicationType.Staff ? (EP.FirstName + " " + EP.LastName) : (MR.MemberFirstName + " " + MR.MemberLastName),
                         ApplicationKey = a.ApplicationKey,
                         ApplicationTypeName = a.ApplicationType.ApplicationTypeName
                     }).Where(x => x.RowKey == id).FirstOrDefault();
            if (model == null)
            {
                model = new MemberPlanDetailsViewModel();
            }
            FillDropdownLists(model);
            return model;
        }
        //public MemberPlanDetailsViewModel CreateMemberPlanDetails(MemberPlanDetailsViewModel model)
        //{
        //    FillDropdownLists(model);
        //    using (var transaction = dbContext.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            Int64 maxKey = dbContext.MemberPlanDetails.Select(p => p.RowKey).DefaultIfEmpty().Max();
        //            //foreach (long Key in (model.ApplicationTypeKey == DbConstants.ApplicationType.Student ? model.StudentKey : model.EmployeeKey))
        //            //{
        //            //    MemberPlanDetail memberPlanDetailModel = new MemberPlanDetail();
        //            //    memberPlanDetailModel.RowKey = Convert.ToInt64(maxKey + 1);
        //            //    memberPlanDetailModel.MemberTypeKey = model.MemberTypeKey;
        //            //    memberPlanDetailModel.BorrowerTypeKey = model.BorrowerTypeKey;
        //            //    memberPlanDetailModel.ApplicationKey = Key;
        //            //    memberPlanDetailModel.ApplicationTypeKey = model.ApplicationTypeKey;
        //            //    memberPlanDetailModel.CardId = model.ApplicationTypeKey == DbConstants.ApplicationType.Student ? dbContext.Applications.Where(x => x.RowKey == Key).Select(x => x.AdmissionNo).FirstOrDefault() : dbContext.Employees.Where(x => x.RowKey == Key).Select(x => x.EmployeeCode).FirstOrDefault();
        //            //    memberPlanDetailModel.IsBlockMember = model.IsBlockMember;
        //            //    dbContext.MemberPlanDetails.Add(memberPlanDetailModel);
        //            //    maxKey++;
        //            //}

        //            dbContext.SaveChanges();
        //            transaction.Commit();
        //            model.Message = EduSuiteUIResources.Success;
        //            model.IsSuccessful = true;
        //            ActivityLog.CreateActivityLog(MenuConstants.MemberPlanDetails, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);

        //        }
        //        catch (Exception ex)
        //        {
        //            transaction.Rollback();
        //            model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.MemberPlanDetails);
        //            model.IsSuccessful = false;
        //            ActivityLog.CreateActivityLog(MenuConstants.MemberPlanDetails, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
        //        }
        //    }

        //    return model;
        //}
        public MemberPlanDetailsViewModel UpdateMemberPlanDetails(MemberPlanDetailsViewModel model)
        {
            MemberPlanDetail memberPlanDetailModel = new MemberPlanDetail();

            FillDropdownLists(model);

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    memberPlanDetailModel = dbContext.MemberPlanDetails.SingleOrDefault(row => row.RowKey == model.RowKey);
                    memberPlanDetailModel.MemberTypeKey = model.MemberTypeKey ?? 0;
                    memberPlanDetailModel.BorrowerTypeKey = model.BorrowerTypeKey ?? 0;
                    memberPlanDetailModel.ApplicationKey = model.ApplicationKey;
                    memberPlanDetailModel.ApplicationTypeKey = model.ApplicationTypeKey;
                    memberPlanDetailModel.CardId = model.CardId;
                    memberPlanDetailModel.IsBlockMember = model.IsBlockMember;

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.MemberPlanDetails, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.MemberPlanDetails);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.MemberPlanDetails, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }
        public MemberPlanDetailsViewModel DeleteMemberPlanDetails(MemberPlanDetailsViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    MemberPlanDetail memberPlanDetail = dbContext.MemberPlanDetails.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.MemberPlanDetails.Remove(memberPlanDetail);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.MemberPlanDetails, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.MemberPlanDetails);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.MemberPlanDetails, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.MemberPlanDetails);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.MemberPlanDetails, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public void FillDropdownLists(MemberPlanDetailsViewModel model)
        {
            FillAcademicTerm(model);
            FillApplicationType(model);

            FillBranches(model);
            FillBatches(model);
            FillCourses(model);
            FillUniversity(model);
            FillMemberType(model);
            FillBorrowerType(model);
            FillDesignation(model);
        }

        private void FillApplicationType(MemberPlanDetailsViewModel model)
        {
            model.ApplicationTypes = dbContext.ApplicationTypes.Where(x => x.RowKey != DbConstants.ApplicationType.Other).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ApplicationTypeName
            }).ToList();
        }
        private void FillBatches(MemberPlanDetailsViewModel model)
        {
            model.Batches = dbContext.Batches.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BatchName
            }).ToList();
        }
        public void FillCourses(MemberPlanDetailsViewModel model)
        {
            model.Courses = dbContext.Courses.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.CourseName
            }).ToList();
        }

        private void FillUniversity(MemberPlanDetailsViewModel model)
        {
            model.Universities = dbContext.UniversityMasters.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.UniversityMasterName
            }).ToList();
        }
        private void FillAcademicTerm(MemberPlanDetailsViewModel model)
        {
            model.AcademicTerms = dbContext.AcademicTerms.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AcademicTermName
            }).ToList();
        }
        public void FillMemberType(MemberPlanDetailsViewModel model)
        {
            model.MemberType = dbContext.MemberTypes.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.MemberTypeName
            }).ToList();
        }
        public void FillBorrowerType(MemberPlanDetailsViewModel model)
        {
            model.BorrowerType = dbContext.BorrowerTypes.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BorrowerTypeName
            }).ToList();
        }
        public void FillBranches(MemberPlanDetailsViewModel model)
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

        private void FillDesignation(MemberPlanDetailsViewModel model)
        {
            model.Designation = dbContext.Designations.Where(x => x.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.DesignationName
            }).ToList();
        }

        public List<MemberPlanDetailsViewModel> GetMemberPlan(MemberPlanDetailsViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;

                IQueryable<MemberPlanDetailsViewModel> MemberDetails = null;


                if (model.ApplicationTypeKey != 0 && model.ApplicationTypeKey != null)
                {
                    if (model.ApplicationTypeKey == DbConstants.ApplicationType.Student)
                    {
                        List<long> oldApplications = dbContext.MemberPlanDetails.Where(x => x.ApplicationTypeKey == DbConstants.ApplicationType.Student).Select(x => x.ApplicationKey).ToList();
                        oldApplications = oldApplications != null ? oldApplications : new List<long>();

                        MemberDetails = (from a in dbContext.Applications.Where(x => !oldApplications.Contains(x.RowKey))
                                         where (a.StudentName.Contains(model.SearchText)
                                                || a.StudentMobile.Contains(model.SearchText) || a.AdmissionNo.Contains(model.SearchText))
                                         select new MemberPlanDetailsViewModel
                                         {
                                             RowKey = 0,
                                             MemberFullName = a.StudentName,
                                             BranchKey = a.BranchKey,
                                             ApplicationTypeKey = DbConstants.ApplicationType.Student,
                                             ApplicationKey = a.RowKey,
                                             CourseKey = a.CourseKey,
                                             UniversityMasterKey = a.UniversityMasterKey,
                                             BatchKey = a.BatchKey,
                                             AcademicTermKey = a.AcademicTermKey,
                                             MemberTypeKey = 0,
                                             BorrowerTypeKey = 0,
                                             CardId = "",
                                             CardSerialNo = 0,
                                             CheckStatus = false,
                                             MobilleNo = a.StudentMobile,
                                             UniversityCourse = a.Course.CourseName + " " + a.UniversityMaster.UniversityMasterName,
                                             BatchName = a.Batch.BatchName,
                                             CurrentYear = a.CurrentYear,
                                             CourseDuration = a.Course.CourseDuration,
                                             AdmissionNo = a.AdmissionNo
                                         });



                        if (model.CourseKey != 0 && model.CourseKey != null)
                        {
                            MemberDetails = MemberDetails.Where(x => x.CourseKey == model.CourseKey);
                        }
                        if (model.UniversityMasterKey != 0 && model.UniversityMasterKey != null)
                        {
                            MemberDetails = MemberDetails.Where(x => x.UniversityMasterKey == model.UniversityMasterKey);
                        }
                        if (model.BatchKey != 0 && model.BatchKey != null)
                        {
                            MemberDetails = MemberDetails.Where(x => x.BatchKey == model.BatchKey);
                        }

                    }
                    else if (model.ApplicationTypeKey == DbConstants.ApplicationType.Staff)
                    {
                        List<long> oldEmployees = dbContext.MemberPlanDetails.Where(x => x.ApplicationTypeKey == DbConstants.ApplicationType.Staff).Select(x => x.ApplicationKey).ToList();
                        oldEmployees = oldEmployees != null ? oldEmployees : new List<long>();

                        MemberDetails = (from e in dbContext.Employees.Where(x => !oldEmployees.Contains(x.RowKey))
                                         where (e.FirstName.Contains(model.SearchText) || e.LastName.Contains(model.SearchText)
                                                || e.MobileNumber.Contains(model.SearchText) || e.EmployeeCode.Contains(model.SearchText))
                                         select new MemberPlanDetailsViewModel
                                         {
                                             RowKey = 0,
                                             MemberFullName = e.FirstName,
                                             BranchKey = e.BranchKey,
                                             ApplicationTypeKey = DbConstants.ApplicationType.Staff,
                                             ApplicationKey = e.RowKey,
                                             MemberTypeKey = 0,
                                             BorrowerTypeKey = 0,
                                             CardId = "",
                                             CardSerialNo = 0,
                                             CheckStatus = false,
                                             MobilleNo = e.MobileNumber,
                                             DesignationName = e.Designation.DesignationName,
                                             DesignationKey = e.DesignationKey
                                         });

                        if (model.DesignationKey != 0 && model.DesignationKey != null)
                        {
                            MemberDetails = MemberDetails.Where(x => x.DesignationKey == model.DesignationKey);
                        }
                    }
                }



                if (model.BranchKey != 0 && model.BranchKey != null)
                {
                    MemberDetails = MemberDetails.Where(x => x.BranchKey == model.BranchKey);
                }

                //MemberDetails = MemberDetails.GroupBy(x => x.ApplicationTypeKey).Select(y => y.FirstOrDefault());
                TotalRecords = MemberDetails.Count();
                var MemberList = MemberDetails.OrderBy(Row => Row.MemberFullName).Skip(Skip).Take(Take).ToList<MemberPlanDetailsViewModel>();
                return MemberList;

                //return MemberDetails.GroupBy(x => x.ApplicationKey).Select(y => y.First()).ToList<MemberPlanDetailsViewModel>();
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.MemberPlanDetails, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<MemberPlanDetailsViewModel>();


            }
        }

        public MemberPlanDetailsViewModel CreateMemberPlans(List<MemberPlanDetailsViewModel> modelList)
        {
            MemberPlanDetailsViewModel model = new MemberPlanDetailsViewModel();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Int64 maxKey = dbContext.MemberPlanDetails.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    foreach (MemberPlanDetailsViewModel objmodel in modelList.Where(x => x.CheckStatus))
                    {
                        MemberPlanDetail memberplandetailmodel = new MemberPlanDetail();
                        memberplandetailmodel.RowKey = Convert.ToInt64(maxKey + 1);
                        memberplandetailmodel.BranchKey = objmodel.BranchKey;
                        memberplandetailmodel.MemberTypeKey = objmodel.MemberTypeKey ?? 0;
                        memberplandetailmodel.BorrowerTypeKey = objmodel.BorrowerTypeKey ?? 0;
                        memberplandetailmodel.ApplicationKey = objmodel.ApplicationKey;
                        memberplandetailmodel.ApplicationTypeKey = objmodel.ApplicationTypeKey;

                        memberplandetailmodel.CardId = dbContext.Database.SqlQuery<string>("select dbo.F_GenerateLibraryCardId(" + objmodel.BranchKey + "," + objmodel.MemberTypeKey + "," + objmodel.BorrowerTypeKey + ")").Single().ToString();
                        memberplandetailmodel.CardSerialNo = dbContext.Database.SqlQuery<int>("select dbo.F_GenerateLibraryCardSerialNo(" + objmodel.BranchKey + "," + objmodel.MemberTypeKey + "," + objmodel.BorrowerTypeKey + ")").Single();


                        memberplandetailmodel.IsBlockMember = false;
                        dbContext.MemberPlanDetails.Add(memberplandetailmodel);
                        dbContext.SaveChanges();
                        maxKey++;
                    }


                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.MemberPlanDetails, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.MemberPlanDetails);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.MemberPlanDetails, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }
    }
}
