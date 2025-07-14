using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System.Data.SqlClient;
using CITS.EduSuite.Business.Extensions;

namespace CITS.EduSuite.Business.Services
{
    public class DashBoardService : IDashBoardService
    {
        private EduSuiteDatabase dbContext;

        public DashBoardService(EduSuiteDatabase objDb)
        {
            this.dbContext = objDb;
        }

        #region Enquiry DashBoard


        #region Enquiry Common

        public List<dynamic> EnquiryCommon(DashBoardViewModel model)
        {
            Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).FirstOrDefault();
            string EmployeeAccessList = "";
            if (DbConstants.User.RoleKey == DbConstants.Role.Staff)
            {
                if (Employee != null)
                {
                    List<long> EmployeeKeys = new List<long>();
                    EmployeeKeys = dbContext.EmployeeHierarchies.Where(x => x.EmployeeKey == Employee.RowKey).Select(y => y.ToEmployeeKey ?? 0).ToList();
                    EmployeeKeys.Add(Employee.RowKey);
                    if (EmployeeKeys.Count > 0)
                    {
                        EmployeeAccessList = String.Join(",", EmployeeKeys);
                    }

                }
            }

            List<dynamic> EnquiryCountList = new List<dynamic>();



            dbContext.LoadStoredProc("dbo.DB_Enquiry")
               .WithSqlParam("@UserKey", DbConstants.User.UserKey)
               .WithSqlParam("@BranchKey", model.BranchKey)
              .WithSqlParam("@FromDate", model.FromDate != null ? model.FromDate.Value.ToString("yyyy-MM-dd") : null)
               .WithSqlParam("@ToDate", model.ToDate != null ? model.ToDate.Value.ToString("yyyy-MM-dd") : null)
               .WithSqlParam("@EmployeeKey", model.EmployeeKey)
                .WithSqlParam("@EmployeeAccessKeys", EmployeeAccessList)
               .WithSqlParam("@FetchType", model.FetchType)


            .ExecuteStoredProc((handler) =>
            {
                EnquiryCountList = handler.ReadToDynamicList<dynamic>() as List<dynamic>;

            });
            return EnquiryCountList;
        }

        #endregion Enquiry Common


        public List<dynamic> EnquiryEmployeeCount(DashBoardViewModel model)
        {
            List<dynamic> EnquiryEmployeeCountList = new List<dynamic>();

            dbContext.LoadStoredProc("dbo.DB_Enquiry")
               .WithSqlParam("@UserKey", model.AppUserKey)
               .WithSqlParam("@BranchKey", model.BranchKey)
              .WithSqlParam("@FromDate", model.FromDate != null ? model.FromDate.Value.ToString("yyyy-MM-dd") : null)
               .WithSqlParam("@ToDate", model.ToDate != null ? model.ToDate.Value.ToString("yyyy-MM-dd") : null)
               .WithSqlParam("@EmployeeKey", model.EmployeeKey)
               .WithSqlParam("@FetchType", model.FetchType)


            .ExecuteStoredProc((handler) =>
            {
                EnquiryEmployeeCountList = handler.ReadToDynamicList<dynamic>() as List<dynamic>;

            });
            return EnquiryEmployeeCountList;
        }


        #endregion Enquiry DashBoard

        #region Application DashBoard


        #region Students Common
        public List<dynamic> StudentsCommon(DashBoardViewModel model)
        {
            List<dynamic> StudentsCountList = new List<dynamic>();



            dbContext.LoadStoredProc("dbo.DB_Students")
               .WithSqlParam("@UserKey", DbConstants.User.UserKey)
               .WithSqlParam("@BranchKey", model.BranchKey)
              .WithSqlParam("@FromDate", model.FromDate != null ? model.FromDate.Value.ToString("yyyy-MM-dd") : null)
               .WithSqlParam("@ToDate", model.ToDate != null ? model.ToDate.Value.ToString("yyyy-MM-dd") : null)
               .WithSqlParam("@EmployeeKey", model.EmployeeKey)
               .WithSqlParam("@FetchType", model.FetchType)


            .ExecuteStoredProc((handler) =>
            {
                StudentsCountList = handler.ReadToDynamicList<dynamic>() as List<dynamic>;

            });
            return StudentsCountList;
        }
        #endregion Students Common


        #endregion Application DashBoard

        #region Library DashBoard


        #region Library Common
        public List<dynamic> LibraryCommon(DashBoardViewModel model)
        {
            List<dynamic> LibraryCommon = new List<dynamic>();



            dbContext.LoadStoredProc("dbo.DB_Students")
               .WithSqlParam("@UserKey", DbConstants.User.UserKey)
               .WithSqlParam("@BranchKey", model.BranchKey)
              .WithSqlParam("@FromDate", model.FromDate != null ? model.FromDate.Value.ToString("yyyy-MM-dd") : null)
               .WithSqlParam("@ToDate", model.ToDate != null ? model.ToDate.Value.ToString("yyyy-MM-dd") : null)
               .WithSqlParam("@EmployeeKey", model.EmployeeKey)
               .WithSqlParam("@FetchType", model.FetchType)


            .ExecuteStoredProc((handler) =>
            {
                LibraryCommon = handler.ReadToDynamicList<dynamic>() as List<dynamic>;

            });
            return LibraryCommon;
        }
        #endregion Library Common


        #endregion Library DashBoard

        #region Accounts DashBoard


        #region Accounts Common
        public List<dynamic> AccountsCommon(DashBoardViewModel model)
        {
            List<dynamic> Accounts = new List<dynamic>();



            dbContext.LoadStoredProc("dbo.DB_Accounts")
               .WithSqlParam("@UserKey", DbConstants.User.UserKey)
               .WithSqlParam("@BranchKey", model.BranchKey)
              .WithSqlParam("@FromDate", model.FromDate != null ? model.FromDate.Value.ToString("yyyy-MM-dd") : null)
               .WithSqlParam("@ToDate", model.ToDate != null ? model.ToDate.Value.ToString("yyyy-MM-dd") : null)
               .WithSqlParam("@EmployeeKey", model.EmployeeKey)
               .WithSqlParam("@FetchType", model.FetchType)


            .ExecuteStoredProc((handler) =>
            {
                Accounts = handler.ReadToDynamicList<dynamic>() as List<dynamic>;

            });
            return Accounts;
        }
        #endregion Accounts Common


        #endregion Accounts DashBoard
    }
}
