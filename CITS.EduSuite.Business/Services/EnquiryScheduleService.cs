using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System.Data.Entity;
using CITS.EduSuite.Business.Common;
using System.Data.Entity.Infrastructure;
using CITS.EduSuite.Business.Models.Security;
using System.Threading;
using System.Data.Entity.Core.Objects;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class EnquiryScheduleService : IEnquiryScheduleService
    {
        private EduSuiteDatabase dbContext;
        public EnquiryScheduleService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public List<EnquiryScheduleViewModel> GetEnquiryLeadSchedule(EnquiryScheduleViewModel model)
        {
            try
            {
                var Take = model.PageSize;
                var skip = (model.PageIndex - 1) * model.PageSize;


                //if (model.ScheduleStatusKey == DbConstants.ScheduleStatus.History)
                //{
                //    GetCallDurationReport(model, enquiryLeadListQuery);
                //    GetCallCountReport(model, enquiryLeadListQuery);
                //}
                Employee Employee = null;
                Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
                //if ((Employee != null && model.SearchEmployeeKey != null && Employee.RowKey != model.SearchEmployeeKey))
                //{
                //    model.IsEditable = false;
                //}
                //else
                //{
                //    model.IsEditable = true;
                //}


                ObjectParameter TotalCount = new ObjectParameter("TotalCount", typeof(Int64));
                ObjectParameter TodayCount = new ObjectParameter("TodayCount", typeof(Int64));
                ObjectParameter TomorrowCount = new ObjectParameter("TomorrowCount", typeof(Int64));
                ObjectParameter PendingCount = new ObjectParameter("PendingCount", typeof(Int64));
                ObjectParameter UpcomingCount = new ObjectParameter("UpcomingCount", typeof(Int64));
                ObjectParameter HistoryCount = new ObjectParameter("HistoryCount", typeof(Int64));
                ObjectParameter TodayRescheduleCount = new ObjectParameter("TodayRescheduleCount", typeof(Int64));
                ObjectParameter CouncellingCount = new ObjectParameter("CouncellingCount", typeof(Int64));
                ObjectParameter NewLeadCount = new ObjectParameter("NewLeadCount", typeof(Int64));
                ObjectParameter UnallocatedLeadCount = new ObjectParameter("UnallocatedLeadCount", typeof(Int64));
                ObjectParameter ShortlistedCount = new ObjectParameter("ShortlistedCount", typeof(Int64));
                ObjectParameter ShortlistPendingCount = new ObjectParameter("ShortlistPendingCount", typeof(Int64));
                ObjectParameter ClosedCount = new ObjectParameter("ClosedCount", typeof(Int64));

                string Today = DateTimeUTC.Now.ToString("yyyy-MM-dd");
                string Tomorrow = DateTimeUTC.Tomorrow.ToString("yyyy-MM-dd");
                string FromDate = model.SearchFromDate != null ? Convert.ToDateTime(model.SearchFromDate).ToString("yyyy-MM-dd") : null;
                string ToDate = model.SearchToDate != null ? Convert.ToDateTime(model.SearchToDate).ToString("yyyy-MM-dd") : null;

                var enquiryLeadList = dbContext.spEnquiryScheduleSelect(
                    DbConstants.User.UserKey,
                    model.SearchEmployeeKey,
                    model.SearchBranchKey,
                    model.SearchCallStatusKey,
                    model.SearchCallTypeKey,
                    model.SearchScheduleStatusKey,
                    FromDate,
                    ToDate,
                    model.ScheduleSelectTypeKey,
                    Today,
                    Tomorrow,
                    model.ScheduleStatusKey,
                    DbConstants.ScheduleStatus.NewLead,
                    DbConstants.ScheduleStatus.Today,
                    DbConstants.ScheduleStatus.Tomorrow,
                    DbConstants.ScheduleStatus.Pending,
                    DbConstants.ScheduleStatus.Upcoming,
                    DbConstants.ScheduleStatus.History,
                    DbConstants.ScheduleStatus.TodayReshceduled,
                    DbConstants.ScheduleStatus.CounsellingSchedule,
                    DbConstants.ScheduleStatus.Unallocated,
                    DbConstants.ScheduleStatus.Shortlisted,
                    DbConstants.ScheduleStatus.ShortlistPending,
                    DbConstants.EnquiryCallStatus.Counselling,
                    DbConstants.FileHandoverType.Enquiry,
                    DbConstants.FileHandoverType.EnquiryLead,
                    DbConstants.EnquiryStatus.FollowUp,
                    DbConstants.EnquiryStatus.Intersted,
                    VerifyData(model.SearchName),
                   VerifyData(model.SearchPhone),
                    VerifyData(model.SearchEmail),
                    VerifyData(model.SearchLocation),
                        model.IsShortListed,
                    model.PageIndex,
                    model.PageSize,
                    TotalCount,
                    TodayCount,
                    TomorrowCount,
                    PendingCount,
                    UpcomingCount,
                    HistoryCount,
                    TodayRescheduleCount,
                    CouncellingCount,
                    NewLeadCount,
                        UnallocatedLeadCount,
                        ShortlistedCount,
                        ShortlistPendingCount,
                        model.ModuleKey,
                        ClosedCount,
                        DbConstants.ScheduleStatus.Closed
                    ).Select(row => new EnquiryScheduleViewModel
                    {
                        RowNumber = row.RowNumber,
                        RowKey = row.RowKey,
                        BranchKey = row.BranchKey ?? 0,
                        EmployeeKey = row.EmployeeKey,
                        Name = row.Name,
                        EmailAddress = row.Email,
                        Qualification = row.Qualification,
                        EmployeeName = row.EmployeeName,
                        MobileNumber = row.Phone,
                        LeadFrom = GetLeadFromByMobile(row.Phone),
                        EnquiryStatusKey = row.EnquiryStatusKey,
                        //DepartmentKey = row.DepartmentKey,
                        FeedbackCreatedDate = row.FeedbackCreatedDate,
                        Feedback = row.Feedback,
                        NextCallScheduleDate = row.NextCallScheduleDate,
                        EnquiryFeedbackReminderStatus = row.EnquiryFeedbackReminderStatus,
                        CallStatusName = row.EnquiryCallStatusName,
                        CallTypeName = row.CallTypeName,
                        CallDuration = row.CallDuration,
                        CreatedBy = row.CreatedBy,
                        CreateOn = row.CreateOn,
                        ScheduleTypeKey = row.ScheduleTypeKey,
                        LastCallStatusKey = row.EnquiryCallStatusKey,
                        ScheduleStatusKey = model.ScheduleStatusKey,
                        IsNew = row.IsNew ?? false,
                        IsEditable = true,
                        CouncellingTime = row.CouncellingTime,
                        LocationName = row.LocationName,
                        FeedbackKey = row.FeedbackKey ?? 0,
                        ModuleKey = row.ModuleKey ?? 0
                    }).ToList();
                model.TotalRecords = TotalCount.Value != DBNull.Value ? Convert.ToInt64(TotalCount.Value) : 0;
                model.TodaysScheduleCount = TodayCount.Value != DBNull.Value ? Convert.ToInt64(TodayCount.Value) : 0;
                model.TomorrowCount = TomorrowCount.Value != DBNull.Value ? Convert.ToInt64(TomorrowCount.Value) : 0;
                model.PendingScheduleCount = PendingCount.Value != DBNull.Value ? Convert.ToInt64(PendingCount.Value) : 0;
                model.UpcomingScheduleCount = UpcomingCount.Value != DBNull.Value ? Convert.ToInt64(UpcomingCount.Value) : 0;
                model.HistoryCount = HistoryCount.Value != DBNull.Value ? Convert.ToInt64(HistoryCount.Value) : 0;
                model.TodaysRecheduleCount = TodayRescheduleCount.Value != DBNull.Value ? Convert.ToInt64(TodayRescheduleCount.Value) : 0;
                model.CouncellingScheduleCount = CouncellingCount.Value != DBNull.Value ? Convert.ToInt64(CouncellingCount.Value) : 0;
                model.NewLeadCount = NewLeadCount.Value != DBNull.Value ? Convert.ToInt64(NewLeadCount.Value) : 0;
                model.UnallocatedLeadCount = UnallocatedLeadCount.Value != DBNull.Value ? Convert.ToInt64(UnallocatedLeadCount.Value) : 0;
                model.ShortlistedCount = ShortlistedCount.Value != DBNull.Value ? Convert.ToInt64(ShortlistedCount.Value) : 0;
                model.ShortlistPendingCount = ShortlistPendingCount.Value != DBNull.Value ? Convert.ToInt64(ShortlistPendingCount.Value) : 0;
                model.ClosedCount = ClosedCount.Value != DBNull.Value ? Convert.ToInt64(ClosedCount.Value) : 0;



                switch (model.ScheduleStatusKey)
                {
                    case DbConstants.ScheduleStatus.NewLead:
                        model.NewLeadCount = model.TotalRecords;
                        break;
                    case DbConstants.ScheduleStatus.Today:
                        model.TodaysScheduleCount = model.TotalRecords;
                        break;
                    case DbConstants.ScheduleStatus.Pending:
                        model.PendingScheduleCount = model.TotalRecords;
                        break;
                    case DbConstants.ScheduleStatus.Upcoming:
                        model.UpcomingScheduleCount = model.TotalRecords;
                        break;
                    case DbConstants.ScheduleStatus.Tomorrow:
                        model.TomorrowCount = model.TotalRecords;
                        break;
                    case DbConstants.ScheduleStatus.History:
                        model.HistoryCount = model.TotalRecords;
                        break;
                    case DbConstants.ScheduleStatus.TodayReshceduled:
                        model.TodaysRecheduleCount = model.TotalRecords;
                        break;
                    case DbConstants.ScheduleStatus.CounsellingSchedule:
                        model.CouncellingScheduleCount = model.TotalRecords;
                        break;
                    case DbConstants.ScheduleStatus.Unallocated:
                        model.UnallocatedLeadCount = model.TotalRecords;
                        break;
                    case DbConstants.ScheduleStatus.Shortlisted:
                        model.ShortlistedCount = model.TotalRecords;
                        break;
                    case DbConstants.ScheduleStatus.ShortlistPending:
                        model.ShortlistPendingCount = model.TotalRecords;
                        break;
                    case DbConstants.ScheduleStatus.Closed:
                        model.ClosedCount = model.TotalRecords;
                        break;

                }

                if (!DbConstants.Role.AdminUserTypes.Contains(DbConstants.User.RoleKey))
                {
                    List<EmployeeHeirarchyDetailsModel> EmployeeHeirarchyDetailsList = new List<EmployeeHeirarchyDetailsModel>();

                    EmployeeHeirarchyDetailsList = (from EH in dbContext.FN_EmployeeHeirarchy(DbConstants.User.UserKey)
                                                    select new EmployeeHeirarchyDetailsModel
                                                    {
                                                        RowKey = 0,
                                                        ToEmployeeKey = EH.EmployeeKey,
                                                        DataAccess = EH.DataAccess ?? false,
                                                        IsActive = EH.IsActive ?? false
                                                    }).ToList();

                    enquiryLeadList = (from ESL in enquiryLeadList
                                       join FNEH in EmployeeHeirarchyDetailsList.Where(x => x.IsActive) on ESL.EmployeeKey equals FNEH.ToEmployeeKey
                                       select new EnquiryScheduleViewModel
                                       {
                                           RowNumber = ESL.RowNumber,
                                           RowKey = ESL.RowKey,
                                           BranchKey = ESL.BranchKey,
                                           EmployeeKey = ESL.EmployeeKey,
                                           Name = ESL.Name,
                                           EmailAddress = ESL.EmailAddress,
                                           Qualification = ESL.Qualification,
                                           EmployeeName = ESL.EmployeeName,
                                           MobileNumber = ESL.MobileNumber,
                                           LeadFrom = ESL.LeadFrom,
                                           EnquiryStatusKey = ESL.EnquiryStatusKey,
                                           //DepartmentKey = ESL.DepartmentKey,
                                           FeedbackCreatedDate = ESL.FeedbackCreatedDate,
                                           Feedback = ESL.Feedback,
                                           NextCallScheduleDate = ESL.NextCallScheduleDate,
                                           EnquiryFeedbackReminderStatus = ESL.EnquiryFeedbackReminderStatus,
                                           CallStatusName = ESL.CallStatusName,
                                           CallTypeName = ESL.CallTypeName,
                                           CallDuration = ESL.CallDuration,
                                           CreatedBy = ESL.CreatedBy,
                                           CreateOn = ESL.CreateOn,
                                           ScheduleTypeKey = ESL.ScheduleTypeKey,
                                           LastCallStatusKey = ESL.LastCallStatusKey,
                                           ScheduleStatusKey = model.ScheduleStatusKey,
                                           IsNew = ESL.IsNew,
                                           IsEditable = FNEH != null ? FNEH.DataAccess : false,
                                           CouncellingTime = ESL.CouncellingTime,
                                           LocationName = ESL.LocationName,
                                           FeedbackKey = ESL.FeedbackKey,
                                           ModuleKey = ESL.ModuleKey
                                       }).ToList();
                }


                return enquiryLeadList;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.EnquirySchedule, ActionConstants.MenuAccess, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return new List<EnquiryScheduleViewModel>();
            }
        }
        public string GetLeadFromByMobile(string mobile)
        {
            var result = dbContext.Database.SqlQuery<string>(
                "EXEC GetleadFrom @p0", mobile).FirstOrDefault();

            return result;
        }


        private IQueryable<EnquiryScheduleViewModel> EnquiryScheduleQuery(EnquiryScheduleViewModel model)
        {
            List<short> EnquiryLeadStatusKeys = new List<short>() { DbConstants.EnquiryStatus.FollowUp, 0 };
            List<short> EnquiryStatusKeys = new List<short>() { DbConstants.EnquiryStatus.FollowUp, DbConstants.EnquiryStatus.Intersted };

            Employee Employee = null;
            Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
            if ((Employee != null && model.SearchEmployeeKey != null && Employee.RowKey != model.SearchEmployeeKey))
            {
                model.IsEditable = false;
            }
            else
            {
                model.IsEditable = true;
            }

            IQueryable<EnquiryScheduleViewModel> enquiryLeadListQuery = (from EL in dbContext.EnquiryLeads
                                                                         join ELF in dbContext.EnquiryLeadFeedbacks
                                                                         on EL.RowKey equals ELF.EnquiryLeadKey
                                                                         join AU in dbContext.AppUsers on ELF.AddedBy equals AU.RowKey
                                                                         select new EnquiryScheduleViewModel
                                                                         {
                                                                             RowKey = EL.RowKey,
                                                                             BranchKey = EL.BranchKey ?? 0,
                                                                             EmployeeKey = EL.EmployeeKey ?? 0,
                                                                             Name = EL.Name,
                                                                             EmailAddress = EL.EmailAddress,
                                                                             Qualification = EL.Qualification,
                                                                             EmployeeName = EL.Employee.FirstName + " " + EL.Employee.MiddleName ?? "" + " " + EL.Employee.LastName,
                                                                             MobileNumber = EL.MobileNumber,
                                                                             EnquiryStatusKey = EL.EnquiryLeadStatusKey ?? 0,
                                                                             //DepartmentKey = EL.DepartmentKey ?? 0,
                                                                             FeedbackCreatedDate = ELF.DateAdded,
                                                                             Feedback = ELF.Feedback,
                                                                             CallTypeKey = ELF.CallTypeKey,
                                                                             NextCallScheduleDate = ELF.NextCallSchedule,
                                                                             EnquiryFeedbackReminderStatus = ELF.EnquiryFeedbackReminderStatus,
                                                                             CallStatusName = ELF.EnquiryCallStatu.EnquiryCallStatusName,
                                                                             CallTypeName = ELF.CallType.CallTypeName,
                                                                             CallDuration = ELF.CallDuration,
                                                                             CreatedBy = AU.FirstName + " " + (AU.MiddleName ?? "") + " " + AU.LastName,
                                                                             CreateOn = ELF.DateAdded,
                                                                             ScheduleTypeKey = 0,
                                                                             LastCallStatusKey = ELF.EnquiryLeadCallStatusKey ?? 0,
                                                                             ScheduleStatusKey = model.ScheduleStatusKey,
                                                                             IsNew = false,
                                                                             IsEditable = model.IsEditable,
                                                                             CouncellingTime = ""

                                                                         }).Union(from EL in dbContext.Enquiries
                                                                                  join ELF in dbContext.EnquiryFeedbacks
                                                                                  on EL.RowKey equals ELF.EnquiryKey
                                                                                  join AU in dbContext.AppUsers on ELF.AddedBy equals AU.RowKey

                                                                                  select new EnquiryScheduleViewModel
                                                                                  {
                                                                                      RowKey = EL.RowKey,
                                                                                      BranchKey = EL.BranchKey,
                                                                                      EmployeeKey = EL.EmployeeKey ?? 0,
                                                                                      Name = EL.EnquiryName,
                                                                                      EmailAddress = EL.EmailAddress,
                                                                                      Qualification = EL.EnquiryEducationQualification,
                                                                                      EmployeeName = EL.Employee.FirstName + " " + EL.Employee.MiddleName ?? "" + " " + EL.Employee.LastName,
                                                                                      MobileNumber = EL.MobileNumber,
                                                                                      EnquiryStatusKey = EL.EnquiryStatusKey ?? 0,
                                                                                      //DepartmentKey = EL.DepartmentKey,
                                                                                      FeedbackCreatedDate = ELF.DateAdded,
                                                                                      Feedback = ELF.EnquiryFeedbackDesc,
                                                                                      CallTypeKey = ELF.CallTypeKey,
                                                                                      NextCallScheduleDate = ELF.EnquiryFeedbackReminderDate,
                                                                                      EnquiryFeedbackReminderStatus = ELF.EnquiryFeedbackReminderStatus ?? false,
                                                                                      CallStatusName = ELF.EnquiryCallStatu.EnquiryCallStatusName,
                                                                                      CallTypeName = ELF.CallType.CallTypeName,
                                                                                      CallDuration = ELF.EnquiryDuration,
                                                                                      CreatedBy = AU.FirstName + " " + (AU.MiddleName ?? "") + " " + AU.LastName,
                                                                                      CreateOn = ELF.DateAdded,
                                                                                      ScheduleTypeKey = 1,
                                                                                      LastCallStatusKey = ELF.EnquiryCallStatusKey ?? 0,
                                                                                      ScheduleStatusKey = model.ScheduleStatusKey,
                                                                                      IsNew = false,
                                                                                      IsEditable = model.IsEditable,
                                                                                      CouncellingTime = ELF.CouncellingTime


                                                                                  }).Union(from EL in dbContext.EnquiryLeads
                                                                                           join AU in dbContext.AppUsers on EL.AddedBy equals AU.RowKey
                                                                                           where (EL.IsNewLead == 0 && EL.EmployeeKey != null)
                                                                                           select new EnquiryScheduleViewModel
                                                                                           {
                                                                                               RowKey = EL.RowKey,
                                                                                               BranchKey = EL.BranchKey ?? 0,
                                                                                               EmployeeKey = EL.EmployeeKey ?? 0,
                                                                                               Name = EL.Name,
                                                                                               EmailAddress = EL.EmailAddress,
                                                                                               Qualification = EL.Qualification,
                                                                                               EmployeeName = EL.Employee.FirstName + " " + EL.Employee.MiddleName ?? "" + " " + EL.Employee.LastName,
                                                                                               MobileNumber = EL.MobileNumber,
                                                                                               EnquiryStatusKey = EL.EnquiryLeadStatusKey ?? 0,
                                                                                               //DepartmentKey = EL.DepartmentKey ?? 0,
                                                                                               FeedbackCreatedDate = EL.DateAdded,
                                                                                               Feedback = string.Empty,
                                                                                               CallTypeKey = null,
                                                                                               NextCallScheduleDate = EL.LeadDate,
                                                                                               EnquiryFeedbackReminderStatus = true,
                                                                                               CallStatusName = string.Empty,
                                                                                               CallTypeName = string.Empty,
                                                                                               CallDuration = null,
                                                                                               CreatedBy = AU.FirstName + " " + (AU.MiddleName ?? "") + " " + AU.LastName,
                                                                                               CreateOn = EL.DateAdded,
                                                                                               ScheduleTypeKey = 0,
                                                                                               LastCallStatusKey = 0,
                                                                                               ScheduleStatusKey = model.ScheduleStatusKey,
                                                                                               IsNew = true,
                                                                                               IsEditable = model.IsEditable,
                                                                                               CouncellingTime = ""

                                                                                           }).Where(row => ((model.SearchName ?? "") == "" || (row.Name ?? "").Contains((model.SearchName ?? "").Trim()))
                                                                                  && ((model.SearchPhone ?? "") == "" || (row.MobileNumber ?? "").Contains((model.SearchPhone ?? "").Trim()))
                                                                                  && ((model.SearchEmail ?? "") == "" || (row.EmailAddress ?? "").Contains((model.SearchEmail ?? "").Trim())));


            if (model.SearchBranchKey != null)
                enquiryLeadListQuery = enquiryLeadListQuery.Where(row => row.BranchKey == model.SearchBranchKey);

            if (model.SearchEmployeeKey != null)
            {
                Employee EmployeeSelected = dbContext.Employees.Where(x => x.RowKey == model.SearchEmployeeKey).SingleOrDefault();
                if (EmployeeSelected != null)
                    enquiryLeadListQuery = (from E in enquiryLeadListQuery
                                            join EFH in dbContext.EmployeeFileHandovers.Where(row => row.IsActive) on new { FileKey = E.RowKey, FileHandoverTypeKey = DbConstants.FileHandoverType.Enquiry } equals new { EFH.FileKey, EFH.FileHandoverTypeKey }
                                            into EFJ
                                            from EFH in EFJ.DefaultIfEmpty()
                                            where ((E.EmployeeKey == E.EmployeeKey || E.EmployeeKey == 0) && E.BranchKey == EmployeeSelected.BranchKey && (EFH.EmployeeFromKey == null || EFH.EmployeeFromKey != EmployeeSelected.RowKey) && (EFH.FileKey == null || EFH.FileKey == E.RowKey))
                                            || (EFH.FileKey != null && E.RowKey == EFH.FileKey && EFH.EmployeeToKey == EmployeeSelected.RowKey && EFH.FileHandoverTypeKey == DbConstants.FileHandoverType.Enquiry)
                                            select E).Union(from E in enquiryLeadListQuery
                                                            join EFH in dbContext.EmployeeFileHandovers.Where(row => row.IsActive) on new { FileKey = E.RowKey, FileHandoverTypeKey = DbConstants.FileHandoverType.EnquiryLead } equals new { EFH.FileKey, EFH.FileHandoverTypeKey }
                                                            into EFJ
                                                            from EFH in EFJ.DefaultIfEmpty()
                                                            where ((E.EmployeeKey == EmployeeSelected.RowKey && (EFH.EmployeeFromKey == null || EFH.EmployeeFromKey != E.EmployeeKey) && (EFH.FileKey == null || EFH.FileKey == E.RowKey))
                                                            || (EFH.FileKey != null && E.RowKey == EFH.FileKey && EFH.EmployeeToKey == EmployeeSelected.RowKey && EFH.FileHandoverTypeKey == DbConstants.FileHandoverType.EnquiryLead))
                                                            select E);
            }
            if (model.ScheduleSelectTypeKey == 1)
                enquiryLeadListQuery = enquiryLeadListQuery.Where(x => x.ScheduleTypeKey == 0);
            else if (model.ScheduleSelectTypeKey == 2)
                enquiryLeadListQuery = enquiryLeadListQuery.Where(x => x.ScheduleTypeKey == 1);

            DateTime Today = DateTimeUTC.Now;
            DateTime Tomorrow = DateTimeUTC.Tomorrow;
            var HistoryQuery = enquiryLeadListQuery;

            enquiryLeadListQuery = enquiryLeadListQuery.Where(row => (EnquiryLeadStatusKeys.Contains(row.EnquiryStatusKey) && row.ScheduleTypeKey == 0) || (EnquiryStatusKeys.Contains(row.EnquiryStatusKey) && row.ScheduleTypeKey == 1));


            var TodaysQuery = enquiryLeadListQuery.Where(row => (System.Data.Entity.DbFunctions.TruncateTime(row.NextCallScheduleDate) == System.Data.Entity.DbFunctions.TruncateTime(Today) && row.EnquiryFeedbackReminderStatus == true));
            var TomorrowQuery = enquiryLeadListQuery.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.NextCallScheduleDate) == System.Data.Entity.DbFunctions.TruncateTime(Tomorrow) && row.EnquiryFeedbackReminderStatus == true);
            var PendingQuery = enquiryLeadListQuery.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.NextCallScheduleDate) < System.Data.Entity.DbFunctions.TruncateTime(Today) && row.EnquiryFeedbackReminderStatus == true);
            var UpcomingQuery = enquiryLeadListQuery.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.NextCallScheduleDate) > System.Data.Entity.DbFunctions.TruncateTime(Today) && System.Data.Entity.DbFunctions.TruncateTime(row.NextCallScheduleDate) != System.Data.Entity.DbFunctions.TruncateTime(Tomorrow) && row.EnquiryFeedbackReminderStatus == true);
            var TodaysRecheduleQuery = enquiryLeadListQuery.Where(row => (System.Data.Entity.DbFunctions.TruncateTime(row.NextCallScheduleDate) == System.Data.Entity.DbFunctions.TruncateTime(Today) && System.Data.Entity.DbFunctions.TruncateTime(row.FeedbackCreatedDate) == System.Data.Entity.DbFunctions.TruncateTime(Today)) && row.EnquiryFeedbackReminderStatus == true);
            var CouncellingScheduleQuery = enquiryLeadListQuery.Where(row => (System.Data.Entity.DbFunctions.TruncateTime(row.NextCallScheduleDate) == System.Data.Entity.DbFunctions.TruncateTime(Today) && row.EnquiryFeedbackReminderStatus == true && row.LastCallStatusKey == DbConstants.EnquiryCallStatus.Counselling));







            //model.AllScheduleCount = enquiryLeadListQuery.Count().ToString();
            //model.EnquiryScheduleCount = enquiryLeadListQuery.Where(x => x.ScheduleTypeKey == 0).ToList().Count.ToString();
            //model.LeadScheduleCount = enquiryLeadListQuery.Where(x => x.ScheduleTypeKey == 1).ToList().Count.ToString();




            //model.TodaysScheduleCount = TodaysQuery.Count();
            //model.TomorrowCount = TomorrowQuery.Count();
            //model.PendingScheduleCount = PendingQuery.Count();
            //model.UpcomingScheduleCount = UpcomingQuery.Count();
            //if (model.ScheduleStatusKey == 3)
            //{
            //    model.HistoryCount = HistoryQuery.Count();

            //}
            //else
            //{
            //    model.HistoryCount = HistoryQuery.Where(row => (System.Data.Entity.DbFunctions.TruncateTime(row.CreateOn) == System.Data.Entity.DbFunctions.TruncateTime(Today))).Count();

            //}
            //model.TodaysRecheduleCount = TodaysRecheduleQuery.Count();
            //model.CouncellingScheduleCount = CouncellingScheduleQuery.Count();


            switch (model.ScheduleStatusKey)
            {
                case DbConstants.ScheduleStatus.Today:
                    enquiryLeadListQuery = TodaysQuery;
                    model.TomorrowCount = TomorrowQuery.Count();
                    model.PendingScheduleCount = PendingQuery.Count();
                    model.UpcomingScheduleCount = UpcomingQuery.Count();
                    model.HistoryCount = HistoryQuery.Where(row => (System.Data.Entity.DbFunctions.TruncateTime(row.CreateOn) == System.Data.Entity.DbFunctions.TruncateTime(Today))).Count();
                    model.TodaysRecheduleCount = TodaysRecheduleQuery.Count();
                    model.CouncellingScheduleCount = CouncellingScheduleQuery.Count();
                    break;
                case DbConstants.ScheduleStatus.Pending:
                    enquiryLeadListQuery = PendingQuery;
                    model.TodaysScheduleCount = TodaysQuery.Count();
                    model.TomorrowCount = TomorrowQuery.Count();
                    model.UpcomingScheduleCount = UpcomingQuery.Count();
                    model.HistoryCount = HistoryQuery.Where(row => (System.Data.Entity.DbFunctions.TruncateTime(row.CreateOn) == System.Data.Entity.DbFunctions.TruncateTime(Today))).Count();
                    model.TodaysRecheduleCount = TodaysRecheduleQuery.Count();
                    model.CouncellingScheduleCount = CouncellingScheduleQuery.Count();
                    break;
                case DbConstants.ScheduleStatus.Upcoming:
                    enquiryLeadListQuery = UpcomingQuery;
                    model.TodaysScheduleCount = TodaysQuery.Count();
                    model.TomorrowCount = TomorrowQuery.Count();
                    model.PendingScheduleCount = PendingQuery.Count();
                    model.HistoryCount = HistoryQuery.Where(row => (System.Data.Entity.DbFunctions.TruncateTime(row.CreateOn) == System.Data.Entity.DbFunctions.TruncateTime(Today))).Count();
                    model.TodaysRecheduleCount = TodaysRecheduleQuery.Count();
                    model.CouncellingScheduleCount = CouncellingScheduleQuery.Count();
                    break;
                case DbConstants.ScheduleStatus.Tomorrow:
                    enquiryLeadListQuery = TomorrowQuery;
                    model.TodaysScheduleCount = TodaysQuery.Count();
                    model.PendingScheduleCount = PendingQuery.Count();
                    model.UpcomingScheduleCount = UpcomingQuery.Count();
                    model.HistoryCount = HistoryQuery.Where(row => (System.Data.Entity.DbFunctions.TruncateTime(row.CreateOn) == System.Data.Entity.DbFunctions.TruncateTime(Today))).Count();
                    model.TodaysRecheduleCount = TodaysRecheduleQuery.Count();
                    model.CouncellingScheduleCount = CouncellingScheduleQuery.Count();
                    break;
                case DbConstants.ScheduleStatus.History:
                    enquiryLeadListQuery = HistoryQuery;
                    model.TodaysScheduleCount = TodaysQuery.Count();
                    model.TomorrowCount = TomorrowQuery.Count();
                    model.PendingScheduleCount = PendingQuery.Count();
                    model.UpcomingScheduleCount = UpcomingQuery.Count();
                    model.TodaysRecheduleCount = TodaysRecheduleQuery.Count();
                    model.CouncellingScheduleCount = CouncellingScheduleQuery.Count();
                    break;
                case DbConstants.ScheduleStatus.TodayReshceduled:
                    enquiryLeadListQuery = TodaysRecheduleQuery;
                    model.TodaysScheduleCount = TodaysQuery.Count();
                    model.TomorrowCount = TomorrowQuery.Count();
                    model.PendingScheduleCount = PendingQuery.Count();
                    model.UpcomingScheduleCount = UpcomingQuery.Count();
                    model.HistoryCount = HistoryQuery.Where(row => (System.Data.Entity.DbFunctions.TruncateTime(row.CreateOn) == System.Data.Entity.DbFunctions.TruncateTime(Today))).Count();
                    model.CouncellingScheduleCount = CouncellingScheduleQuery.Count();
                    break;
                case DbConstants.ScheduleStatus.CounsellingSchedule:
                    enquiryLeadListQuery = CouncellingScheduleQuery;
                    model.TodaysScheduleCount = TodaysQuery.Count();
                    model.TomorrowCount = TomorrowQuery.Count();
                    model.PendingScheduleCount = PendingQuery.Count();
                    model.UpcomingScheduleCount = UpcomingQuery.Count();
                    model.HistoryCount = HistoryQuery.Where(row => (System.Data.Entity.DbFunctions.TruncateTime(row.CreateOn) == System.Data.Entity.DbFunctions.TruncateTime(Today))).Count();
                    model.TodaysRecheduleCount = TodaysRecheduleQuery.Count();
                    break;

            }





            if (model.SearchCallStatusKey != null)
                enquiryLeadListQuery = enquiryLeadListQuery.Where(row => row.LastCallStatusKey == model.SearchCallStatusKey);

            if (model.SearchScheduleStatusKey != null)
            {
                enquiryLeadListQuery = enquiryLeadListQuery.Where(row => row.EnquiryStatusKey == model.SearchScheduleStatusKey);

            }


            if (model.SearchCallTypeKey != null)
                enquiryLeadListQuery = enquiryLeadListQuery.Where(row => row.CallTypeKey == model.SearchCallTypeKey);




            if (model.ScheduleStatusKey == DbConstants.ScheduleStatus.History)
            {
                if (model.SearchFromDate != null && model.SearchToDate != null)
                    enquiryLeadListQuery = enquiryLeadListQuery.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.CreateOn) >= System.Data.Entity.DbFunctions.TruncateTime(model.SearchFromDate) && System.Data.Entity.DbFunctions.TruncateTime(row.CreateOn) <= System.Data.Entity.DbFunctions.TruncateTime(model.SearchToDate));
                else if (model.SearchFromDate != null)
                    enquiryLeadListQuery = enquiryLeadListQuery.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.CreateOn) >= System.Data.Entity.DbFunctions.TruncateTime(model.SearchFromDate));
                else if (model.SearchToDate != null)
                    enquiryLeadListQuery = enquiryLeadListQuery.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.CreateOn) <= System.Data.Entity.DbFunctions.TruncateTime(model.SearchToDate));

            }
            else
            {
                if (model.SearchFromDate != null && model.SearchToDate != null)
                    enquiryLeadListQuery = enquiryLeadListQuery.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.NextCallScheduleDate) >= System.Data.Entity.DbFunctions.TruncateTime(model.SearchFromDate) && System.Data.Entity.DbFunctions.TruncateTime(row.NextCallScheduleDate) <= System.Data.Entity.DbFunctions.TruncateTime(model.SearchToDate));
                else if (model.SearchFromDate != null)
                    enquiryLeadListQuery = enquiryLeadListQuery.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.NextCallScheduleDate) >= System.Data.Entity.DbFunctions.TruncateTime(model.SearchFromDate));
                else if (model.SearchToDate != null)
                    enquiryLeadListQuery = enquiryLeadListQuery.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.NextCallScheduleDate) <= System.Data.Entity.DbFunctions.TruncateTime(model.SearchToDate));

            }
            model.TotalRecords = enquiryLeadListQuery.Count();
            switch (model.ScheduleStatusKey)
            {
                case DbConstants.ScheduleStatus.Today:
                    model.TodaysScheduleCount = model.TotalRecords;
                    break;
                case DbConstants.ScheduleStatus.Pending:
                    model.PendingScheduleCount = model.TotalRecords;
                    break;
                case DbConstants.ScheduleStatus.Upcoming:
                    model.UpcomingScheduleCount = model.TotalRecords;
                    break;
                case DbConstants.ScheduleStatus.Tomorrow:
                    model.TomorrowCount = model.TotalRecords;
                    break;
                case DbConstants.ScheduleStatus.History:
                    model.HistoryCount = model.TotalRecords;
                    break;
                case DbConstants.ScheduleStatus.TodayReshceduled:
                    model.TodaysRecheduleCount = model.TotalRecords;
                    break;
                case DbConstants.ScheduleStatus.CounsellingSchedule:
                    model.CouncellingScheduleCount = model.TotalRecords;
                    break;

            }

            return enquiryLeadListQuery;
        }
        public EnquiryScheduleViewModel GetSearchDropDownLists(EnquiryScheduleViewModel model)
        {
            FillBranches(model);
            GetEmployeesByBranchId(model);
            FillCallTypes(model);
            FillScheduleStatuses(model);
            model.ScheduleStatuses.Insert(0, new SelectListModel { RowKey = 0, Text = EduSuiteUIResources.NewLead });
            model.ScheduleCallStatuses = dbContext.EnquiryCallStatus.Where(row => row.IsActive && row.EnquiryStatusKey != DbConstants.EnquiryStatus.AdmissionTaken).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.EnquiryCallStatusName
            }).ToList();
            model.NatureOfEnquiries = FillNatureOfEnquiry(model);
            return model;
        }
        public List<SelectListModel> FillNatureOfEnquiry(EnquiryScheduleViewModel model)
        {
            return dbContext.NatureOfEnquiries
                .Where(x => x.IsActive)
                .Select(x => new SelectListModel
                {
                    RowKey = x.RowKey,
                    Text = x.NatureOfEnquiryName
                }).ToList();
        }

        private void FillBranches(EnquiryScheduleViewModel model)
        {

            IQueryable<SelectListModel> BranchQuery = dbContext.vwBranchSelectActiveOnlies.OrderBy(row => row.BranchName).Select(row => new SelectListModel
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
            }
            if (model.Branches.Count == 1)
            {
                long? branchkey = model.Branches.Select(x => x.RowKey).FirstOrDefault();
                model.BranchKey = Convert.ToInt16(branchkey);
                model.SearchBranchKey = model.BranchKey;
            }
        }
        public EnquiryScheduleViewModel GetEmployeesByBranchId(EnquiryScheduleViewModel model)
        {
            //Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();

            //if (model.SearchBranchKey != null)
            //    model.BranchKey = model.SearchBranchKey ?? 0;

            //if (DbConstants.User.UserKey != DbConstants.AdminKey)
            //{
            //    if (!DbConstants.Role.AdminUserTypes.Contains(DbConstants.User.RoleKey))
            //    {
            //        if (Employee != null)
            //        {
            //            //if (model.BranchKey == Employee.BranchKey)
            //            //{
            //            //    model.Employees = dbContext.fnChildEmployees(DbConstants.User.UserKey).Where(row => (row.BranchKey == model.BranchKey || model.BranchKey != Employee.BranchKey)).Select(row => new GroupSelectListModel
            //            //    {
            //            //        RowKey = row.RowKey ?? 0,
            //            //        Text = row.EmployeeName,
            //            //        GroupKey = row.DepartmentKey ?? 0,
            //            //        GroupName = row.DepartmentName
            //            //    }).OrderBy(row => row.Text).ToList();
            //            model.SearchEmployeeKey = model.EmployeeKey = Employee.RowKey;
            //            //}
            //            //else
            //            //{
            //            model.Employees = dbContext.Employees.Where(row => (row.BranchKey == model.BranchKey || model.BranchKey != Employee.BranchKey)).Select(row => new GroupSelectListModel
            //            {
            //                RowKey = row.RowKey,
            //                Text = row.AppUser.FirstName + " " + (row.AppUser.MiddleName ?? "") + " " + row.AppUser.LastName,
            //                GroupKey = row.DepartmentKey,
            //                GroupName = row.Department.DepartmentName
            //            }).OrderBy(row => row.Text).ToList();

            //            //}
            //        }
            //    }
            //    else
            //    {
            //        if (Employee != null)
            //        {
            //            model.Employees = dbContext.Employees.Where(row => (row.BranchKey == model.BranchKey)).Select(row => new GroupSelectListModel
            //            {
            //                RowKey = row.RowKey,
            //                Text = row.AppUser.FirstName + " " + (row.AppUser.MiddleName ?? "") + " " + row.AppUser.LastName,
            //                GroupKey = row.DepartmentKey,
            //                GroupName = row.Department.DepartmentName
            //            }).OrderBy(row => row.Text).ToList();
            //            model.SearchEmployeeKey = model.EmployeeKey = Employee.RowKey;
            //        }
            //    }
            //}
            //else
            //{
            //    model.Employees = dbContext.Employees.Where(row => row.EmployeeStatusKey == DbConstants.EmployeeStatus.Working && (row.BranchKey == model.BranchKey || model.BranchKey == 0)).Select(row => new GroupSelectListModel
            //    {
            //        RowKey = row.RowKey,
            //        Text = row.FirstName + " " + (row.MiddleName ?? "") + " " + row.LastName,
            //        GroupKey = row.DepartmentKey,
            //        GroupName = row.Department.DepartmentName,


            //    }).OrderBy(row => row.Text).ToList();


            //    if (Employee != null)
            //    {
            //        model.SearchEmployeeKey = model.EmployeeKey = Employee.RowKey;
            //    }
            //}


            //return model;

            //Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();

            //if (DbConstants.User.UserKey != DbConstants.AdminKey)
            //{
            //    if (Employee != null)
            //    {

            //        model.Employees = dbContext.Employees.Where(row => row.IsActive == true && (row.BranchKey == model.BranchKey || model.BranchKey == 0) && row.RowKey == Employee.RowKey).Select(row => new GroupSelectListModel
            //        {
            //            RowKey = row.RowKey,
            //            Text = row.FirstName,
            //            GroupKey = row.DepartmentKey,
            //            GroupName = row.Department.DepartmentName
            //        }).OrderBy(row => row.Text).ToList();


            //        model.SearchEmployeeKey = model.EmployeeKey = Employee.RowKey;
            //    }
            //    else
            //    {
            //        model.Employees = dbContext.Employees.Where(row => row.IsActive == true && (row.BranchKey == model.BranchKey || model.BranchKey == 0)).Select(row => new GroupSelectListModel
            //        {
            //            RowKey = row.RowKey,
            //            Text = row.FirstName,
            //            GroupKey = row.DepartmentKey,
            //            GroupName = row.Department.DepartmentName

            //        }).OrderBy(row => row.Text).ToList();
            //    }
            //}
            //else
            //{
            //    model.Employees = dbContext.Employees.Where(row => row.IsActive == true && (row.BranchKey == model.BranchKey || model.BranchKey == 0)).Select(row => new GroupSelectListModel
            //    {
            //        RowKey = row.RowKey,
            //        Text = row.FirstName,
            //        GroupKey = row.DepartmentKey,
            //        GroupName = row.Department.DepartmentName

            //    }).OrderBy(row => row.Text).ToList();
            //    if (Employee != null)
            //    {
            //        model.SearchEmployeeKey = model.EmployeeKey = Employee.RowKey;
            //    }
            //}

            //return model;

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

            if (model.BranchKey != 0)
            {
                Employees = Employees.Where(x => x.BranchKey == model.BranchKey).ToList();
            }



            model.Employees = Employees.Select(x => new GroupSelectListModel
            {
                RowKey = x.RowKey,
                Text = x.FirstName,
                GroupKey = x.DepartmentKey,
                GroupName = x.EmployeeCode
            }).ToList();

            return model;
        }
        public void GetCallStatusByEnquiryStatus(EnquiryScheduleViewModel model)
        {
            if (model.ScheduleStatusKey != null)
            {
                model.ScheduleCallStatuses = dbContext.EnquiryCallStatus.Where(x => x.IsActive && x.EnquiryStatusKey == model.ScheduleStatusKey).Select(row => new SelectListModel
                {
                    RowKey = row.RowKey,
                    Text = row.EnquiryCallStatusName
                }).ToList();
            }
            else
            {
                model.ScheduleCallStatuses = dbContext.EnquiryCallStatus.Where(row => row.IsActive && row.EnquiryStatusKey != DbConstants.EnquiryStatus.AdmissionTaken).Select(row => new SelectListModel
                {
                    RowKey = row.RowKey,
                    Text = row.EnquiryCallStatusName
                }).ToList();
            }
        }
        private void FillCallTypes(EnquiryScheduleViewModel model)
        {
            model.CallTypes = dbContext.CallTypes.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.CallTypeName
            }).ToList();

        }
        private void FillScheduleStatuses(EnquiryScheduleViewModel model)
        {
            model.ScheduleStatuses = dbContext.EnquiryStatus.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.EnquiryStatusName
            }).ToList();
        }
        private void FillEnquiryLeadCallStatus(EnquiryScheduleViewModel model)
        {
            model.ScheduleCallStatuses = dbContext.EnquiryCallStatus.Where(row => row.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.EnquiryCallStatusName
            }).ToList();
        }

        //private void GetCallDurationReport(EnquiryScheduleViewModel model, IQueryable<EnquiryScheduleViewModel> Query)
        //{
        //    model.CallDurationList = Query.Where(row => row.LastCallStatusKey != null).GroupBy(a => new { a.LastCallStatusKey, a.CallStatusName })
        //                 .Select(n => new SelectListModel { Text = n.Key.CallStatusName, RowKey = n.Count() }).ToList();
        //}
        //private void GetCallCountReport(EnquiryScheduleViewModel model, IQueryable<EnquiryScheduleViewModel> Query)
        //{
        //    model.CallDurationList = Query.Where(row => row.CallTypeKey != null).GroupBy(a => new { a.CallTypeKey, a.CallTypeName })
        //               .ToList().Select(n => new SelectListModel { Text = n.Key.CallTypeName, RowKey = n.Sum(row => (row.CallDuration ?? TimeSpan.Zero).Ticks) }).ToList();
        //}

        //public EnquiryScheduleViewModel FillFeedbackDrodownLists(EnquiryScheduleViewModel model)
        //{
        //    FillEnquiryLeadCallStatus(model);
        //    FillCallTypes(model);
        //    return model;
        //}
        //LeadFeedback
        public EnquiryLeadFeedbackViewModel CreateEnquiryLeadFeedback(EnquiryLeadFeedbackViewModel model)
        {            
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    model.EnquiryLeadKey = dbContext.EnquiryLeadFeedbacks.Where(x => x.RowKey == model.RowKey).Select(x => x.EnquiryLeadKey).SingleOrDefault();
                    dbContext.EnquiryLeadFeedbacks.Where(x => x.EnquiryLeadKey == model.EnquiryLeadKey).ToList().ForEach(x =>
                    {
                        x.EnquiryFeedbackReminderStatus = false;
                        x.IsLastFeedback = false;
                    });

                    EnquiryLeadFeedback enquiryLeadFeedbackModel = new EnquiryLeadFeedback();
                    Int64 maxKey = dbContext.EnquiryLeadFeedbacks.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    enquiryLeadFeedbackModel.RowKey = Convert.ToInt64(maxKey + 1);
                    enquiryLeadFeedbackModel.Feedback = model.Feedback;
                    enquiryLeadFeedbackModel.CallDuration = model.CallDuration;
                    enquiryLeadFeedbackModel.CallTypeKey = model.CallTypeKey;
                    enquiryLeadFeedbackModel.NextCallSchedule = model.NextCallSchedule;
                    enquiryLeadFeedbackModel.EnquiryLeadKey = model.EnquiryLeadKey;
                    enquiryLeadFeedbackModel.Feedback = model.Feedback;
                    enquiryLeadFeedbackModel.EnquiryFeedbackReminderStatus = true;
                    enquiryLeadFeedbackModel.EnquiryLeadCallStatusKey = model.EnquiryLeadCallStatusKey;
                    enquiryLeadFeedbackModel.IsLastFeedback = true;
                    dbContext.EnquiryLeadFeedbacks.Add(enquiryLeadFeedbackModel);

                    EnquiryLead EnquiryLeadModel = dbContext.EnquiryLeads.Where(x => x.RowKey == model.EnquiryLeadKey).SingleOrDefault();
                    EnquiryLeadModel.IsNewLead = 1;

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EnquirySchedule, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.EnquiryLeadFeedback);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EnquirySchedule, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;
        }
        public EnquiryLeadFeedbackViewModel UpdateEnquiryLeadFeedback(EnquiryLeadFeedbackViewModel model)
        {           
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    EnquiryLeadFeedback enquiryLeadFeedbackModel = new EnquiryLeadFeedback();
                    enquiryLeadFeedbackModel = dbContext.EnquiryLeadFeedbacks.SingleOrDefault(row => row.RowKey == model.RowKey);
                    enquiryLeadFeedbackModel.Feedback = model.Feedback;
                    enquiryLeadFeedbackModel.CallDuration = model.CallDuration;
                    enquiryLeadFeedbackModel.CallTypeKey = model.CallTypeKey;
                    enquiryLeadFeedbackModel.NextCallSchedule = model.NextCallSchedule;
                    enquiryLeadFeedbackModel.Feedback = model.Feedback;
                    enquiryLeadFeedbackModel.EnquiryLeadKey = model.EnquiryLeadKey;
                    enquiryLeadFeedbackModel.EnquiryLeadCallStatusKey = model.EnquiryLeadCallStatusKey;

                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EnquirySchedule, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.EnquiryLeadFeedback);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EnquirySchedule, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public EnquiryScheduleViewModel AllocateMultipleStaff(EnquiryScheduleViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    List<EmployeeFileHandOverViewModel> FileHandOverList = new List<EmployeeFileHandOverViewModel>();
                    List<EnquiryFeedbackViewModel> EnquiryFeedbackList = new List<EnquiryFeedbackViewModel>();
                    List<EnquiryLeadFeedbackViewModel> EnquiryLeadFeedbackList = new List<EnquiryLeadFeedbackViewModel>();
                    foreach (long key in model.EnquiryScheduleKeys)
                    {
                        EmployeeFileHandover employeeFileHandover = new EmployeeFileHandover();
                        EmployeeFileHandOverViewModel FileHandOvers = new EmployeeFileHandOverViewModel();

                        short? BranchKey = dbContext.Employees.Where(x => x.RowKey == model.EmployeeKey).Select(x => x.BranchKey).SingleOrDefault();

                        if (model.ScheduleSelectTypeKey == 1)
                        {
                            EnquiryLead dbEnquiryLead = dbContext.EnquiryLeads.Where(x => x.RowKey == key).SingleOrDefault();

                            if (dbEnquiryLead != null)
                            {
                                employeeFileHandover = dbContext.EmployeeFileHandovers.SingleOrDefault(row => row.FileKey == dbEnquiryLead.RowKey && row.FileHandoverTypeKey == DbConstants.FileHandoverType.EnquiryLead && row.IsActive);
                                if (employeeFileHandover != null)
                                {
                                    employeeFileHandover.IsActive = false;
                                }
                                if (dbEnquiryLead.EmployeeKey != null)
                                {
                                    FileHandOvers.FileHandoverTypeKey = DbConstants.FileHandoverType.EnquiryLead;
                                    FileHandOvers.EmployeeFromKey = dbEnquiryLead.EmployeeKey ?? 0;
                                    FileHandOvers.EmployeeToKey = model.EmployeeKey;
                                    FileHandOvers.FileKey = dbEnquiryLead.RowKey;
                                    FileHandOvers.IsActive = true;
                                    FileHandOvers.IfFeedback = model.IfFeedback;
                                    FileHandOvers.Remarks = model.FHRemarks;
                                    FileHandOverList.Add(FileHandOvers);
                                }
                                if (model.IfFeedback)
                                {
                                    EnquiryLeadFeedbackViewModel leadfeedbackmodel = new EnquiryLeadFeedbackViewModel();
                                    dbContext.EnquiryLeadFeedbacks.Where(x => x.EnquiryLeadKey == dbEnquiryLead.RowKey).ToList().ForEach(x =>
                                    {
                                        x.EnquiryFeedbackReminderStatus = false;
                                        x.IsLastFeedback = false;
                                    });
                                    leadfeedbackmodel.Feedback = model.FHRemarks;
                                    leadfeedbackmodel.CallDuration = model.CallDuration;
                                    leadfeedbackmodel.CallTypeKey = DbConstants.CallType.Outgoing;
                                    leadfeedbackmodel.NextCallSchedule = model.FHNextCallScheduleDate;
                                    leadfeedbackmodel.EnquiryLeadKey = dbEnquiryLead.RowKey;
                                    leadfeedbackmodel.EnquiryLeadCallStatusKey = model.FHCallStatusKey;
                                    EnquiryLeadFeedbackList.Add(leadfeedbackmodel);

                                    dbEnquiryLead.EnquiryLeadStatusKey = model.FHEnquiryStatusKey;
                                    dbEnquiryLead.IsNewLead = 1;
                                }
                                dbEnquiryLead.EmployeeKey = model.EmployeeKey;
                                dbEnquiryLead.BranchKey = BranchKey;
                            }

                        }
                        else if (model.ScheduleSelectTypeKey == 2)
                        {
                            Enquiry dbEnquiry = dbContext.Enquiries.Where(x => x.RowKey == key).SingleOrDefault();

                            if (dbEnquiry != null)
                            {
                                employeeFileHandover = dbContext.EmployeeFileHandovers.SingleOrDefault(row => row.FileKey == dbEnquiry.RowKey && row.FileHandoverTypeKey == DbConstants.FileHandoverType.Enquiry && row.IsActive);
                                if (employeeFileHandover != null)
                                {
                                    employeeFileHandover.IsActive = false;
                                }
                                if (dbEnquiry.EmployeeKey != null)
                                {
                                    FileHandOvers.FileHandoverTypeKey = DbConstants.FileHandoverType.Enquiry;
                                    FileHandOvers.EmployeeFromKey = dbEnquiry.EmployeeKey ?? 0;
                                    FileHandOvers.EmployeeToKey = model.EmployeeKey;
                                    FileHandOvers.FileKey = dbEnquiry.RowKey;
                                    FileHandOvers.IsActive = true;
                                    FileHandOvers.IfFeedback = model.IfFeedback;
                                    FileHandOvers.Remarks = model.FHRemarks;
                                    FileHandOverList.Add(FileHandOvers);
                                }
                                if (model.IfFeedback)
                                {
                                    EnquiryFeedbackViewModel enquiryfeedbackmodel = new EnquiryFeedbackViewModel();
                                    dbContext.EnquiryFeedbacks.Where(x => x.EnquiryKey == dbEnquiry.RowKey).ToList().ForEach(x =>
                                    {
                                        x.EnquiryFeedbackReminderStatus = false;
                                        x.IsLastFeedback = false;
                                    });
                                    enquiryfeedbackmodel.Feedback = model.FHRemarks;
                                    enquiryfeedbackmodel.CallDuration = model.CallDuration;
                                    enquiryfeedbackmodel.CallTypeKey = DbConstants.CallType.Outgoing;
                                    enquiryfeedbackmodel.NextCallSchedule = model.FHNextCallScheduleDate;
                                    enquiryfeedbackmodel.EnquiryStatusKey = model.FHEnquiryStatusKey ?? 0;
                                    enquiryfeedbackmodel.EnquiryKey = dbEnquiry.RowKey;
                                    enquiryfeedbackmodel.EnquiryCallStatusKey = model.FHCallStatusKey;
                                    EnquiryFeedbackList.Add(enquiryfeedbackmodel);

                                    dbEnquiry.EnquiryStatusKey = model.FHEnquiryStatusKey;
                                }
                                dbEnquiry.EmployeeKey = model.EmployeeKey;
                                dbEnquiry.BranchKey = BranchKey ?? 0;
                            }
                        }
                    }
                    UpdateEmployeesFileHandover(FileHandOverList);
                    CreateBulkEnquiryFeedback(EnquiryFeedbackList);
                    CreateBulkLeadFeedback(EnquiryLeadFeedbackList);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.LeadAllocation, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
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
                    ActivityLog.CreateActivityLog(MenuConstants.LeadAllocation, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, dbEx.GetBaseException().Message);
                    throw raise;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Join(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.EnquiryLead);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.LeadAllocation, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public EmployeeFileHandOverViewModel UpdateEmployeesFileHandover(List<EmployeeFileHandOverViewModel> modelList)
        {
            EmployeeFileHandOverViewModel employeeFileHandoverViewModel = new EmployeeFileHandOverViewModel();

            long maxKey = dbContext.EmployeeFileHandovers.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (EmployeeFileHandOverViewModel model in modelList)
            {
                EmployeeFileHandover EmployeeFileHandoverModel = new EmployeeFileHandover();
                EmployeeFileHandoverModel.RowKey = Convert.ToInt64(maxKey + 1);
                EmployeeFileHandoverModel.FileHandoverTypeKey = model.FileHandoverTypeKey ?? 0;
                EmployeeFileHandoverModel.EmployeeFromKey = model.EmployeeFromKey;
                EmployeeFileHandoverModel.EmployeeToKey = model.EmployeeToKey;
                EmployeeFileHandoverModel.FileKey = model.FileKey;
                EmployeeFileHandoverModel.IsActive = true;
                EmployeeFileHandoverModel.IfFeedback = model.IfFeedback;
                EmployeeFileHandoverModel.Remarks = model.Remarks;
                dbContext.EmployeeFileHandovers.Add(EmployeeFileHandoverModel);
                maxKey++;
            }

            return employeeFileHandoverViewModel;
        }

        private void CreateBulkEnquiryFeedback(List<EnquiryFeedbackViewModel> EnquiryFeedbackList)
        {

            Int64 maxKey = dbContext.EnquiryFeedbacks.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (EnquiryFeedbackViewModel model in EnquiryFeedbackList)
            {
                EnquiryFeedback enquiryFeedbackModel = new EnquiryFeedback();

                enquiryFeedbackModel.RowKey = Convert.ToInt64(maxKey + 1);
                enquiryFeedbackModel.EnquiryFeedbackDesc = model.Feedback;
                enquiryFeedbackModel.EnquiryDuration = model.CallDuration;
                enquiryFeedbackModel.CallTypeKey = DbConstants.CallType.Outgoing;
                enquiryFeedbackModel.EnquiryFeedbackReminderDate = model.NextCallSchedule;
                enquiryFeedbackModel.CouncellingTime = model.ConcellingTimeKey;
                if (model.NextCallSchedule == null)
                {
                    enquiryFeedbackModel.EnquiryFeedbackReminderStatus = false;
                }
                else
                {
                    enquiryFeedbackModel.EnquiryFeedbackReminderStatus = true;
                }
                enquiryFeedbackModel.IsLastFeedback = true;

                enquiryFeedbackModel.EnquiryKey = model.EnquiryKey;
                enquiryFeedbackModel.EnquiryCallStatusKey = model.EnquiryCallStatusKey;
                dbContext.EnquiryFeedbacks.Add(enquiryFeedbackModel);
                maxKey++;
            }
        }
        private void CreateBulkLeadFeedback(List<EnquiryLeadFeedbackViewModel> EnquiryLedFeedbackList)
        {
            Int64 maxKey = dbContext.EnquiryLeadFeedbacks.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (EnquiryLeadFeedbackViewModel model in EnquiryLedFeedbackList)
            {
                EnquiryLeadFeedback enquiryLeadFeedbackModel = new EnquiryLeadFeedback();

                enquiryLeadFeedbackModel.RowKey = Convert.ToInt64(maxKey + 1);
                enquiryLeadFeedbackModel.CallDuration = model.CallDuration;
                enquiryLeadFeedbackModel.CallTypeKey = model.CallTypeKey;
                enquiryLeadFeedbackModel.NextCallSchedule = model.NextCallSchedule;
                enquiryLeadFeedbackModel.EnquiryLeadKey = model.EnquiryLeadKey;
                enquiryLeadFeedbackModel.Feedback = model.Feedback;
                if (model.NextCallSchedule == null)
                {
                    enquiryLeadFeedbackModel.EnquiryFeedbackReminderStatus = false;
                }
                else
                {
                    enquiryLeadFeedbackModel.EnquiryFeedbackReminderStatus = true;
                }
                enquiryLeadFeedbackModel.IsLastFeedback = true;

                enquiryLeadFeedbackModel.EnquiryLeadCallStatusKey = model.EnquiryLeadCallStatusKey;
                dbContext.EnquiryLeadFeedbacks.Add(enquiryLeadFeedbackModel);
                maxKey++;
            }

        }

        private IQueryable<EnquiryScheduleViewModel> ScheduleHistoryQuery(EnquiryScheduleViewModel model)
        {
            IQueryable<EnquiryScheduleViewModel> enquiryLeadListQuery = (from EL in dbContext.EnquiryLeads
                                                                         join ELF in dbContext.EnquiryLeadFeedbacks
                                                                         on EL.RowKey equals ELF.EnquiryLeadKey
                                                                         join AU in dbContext.AppUsers on ELF.AddedBy equals AU.RowKey
                                                                         select new EnquiryScheduleViewModel
                                                                         {
                                                                             RowKey = EL.RowKey,
                                                                             BranchKey = EL.BranchKey ?? 0,
                                                                             EmployeeKey = EL.EmployeeKey ?? 0,
                                                                             Name = EL.Name,
                                                                             EmailAddress = EL.EmailAddress,
                                                                             Qualification = EL.Qualification,
                                                                             EmployeeName = EL.Employee.FirstName + " " + EL.Employee.MiddleName ?? "" + " " + EL.Employee.LastName,
                                                                             MobileNumber = EL.MobileNumber,
                                                                             EnquiryStatusKey = EL.EnquiryLeadStatusKey ?? 0,
                                                                             //DepartmentKey = EL.DepartmentKey ?? 0,
                                                                             FeedbackCreatedDate = ELF.DateAdded,
                                                                             Feedback = ELF.Feedback,
                                                                             CallTypeKey = ELF.CallTypeKey,
                                                                             NextCallScheduleDate = ELF.NextCallSchedule,
                                                                             EnquiryFeedbackReminderStatus = ELF.EnquiryFeedbackReminderStatus,
                                                                             CallStatusName = ELF.EnquiryCallStatu.EnquiryCallStatusName,
                                                                             CallTypeName = ELF.CallType.CallTypeName,
                                                                             CallDuration = ELF.CallDuration,
                                                                             CreatedBy = AU.FirstName + " " + (AU.MiddleName ?? "") + " " + AU.LastName,
                                                                             UserKey = ELF.AddedBy,
                                                                             CreateOn = ELF.DateAdded,
                                                                             ScheduleTypeKey = 0,
                                                                             LastCallStatusKey = ELF.EnquiryLeadCallStatusKey ?? 0,
                                                                             ScheduleStatusKey = model.ScheduleStatusKey,
                                                                             IsNew = false,
                                                                             IsEditable = model.IsEditable,
                                                                             CouncellingTime = "",

                                                                             BranchName = EL.Branch.BranchName,
                                                                             LocationName = ""

                                                                         }).Union(from EL in dbContext.Enquiries
                                                                                  join ELF in dbContext.EnquiryFeedbacks
                                                                                  on EL.RowKey equals ELF.EnquiryKey
                                                                                  join AU in dbContext.AppUsers on ELF.AddedBy equals AU.RowKey

                                                                                  select new EnquiryScheduleViewModel
                                                                                  {
                                                                                      RowKey = EL.RowKey,
                                                                                      BranchKey = EL.OtherBranchKey == null ? EL.BranchKey : EL.OtherBranchKey ?? 0,
                                                                                      EmployeeKey = EL.EmployeeKey ?? 0,
                                                                                      Name = EL.EnquiryName,
                                                                                      EmailAddress = EL.EmailAddress,
                                                                                      Qualification = EL.EnquiryEducationQualification,
                                                                                      EmployeeName = EL.Employee.FirstName + " " + EL.Employee.MiddleName ?? "" + " " + EL.Employee.LastName,
                                                                                      MobileNumber = EL.MobileNumber,
                                                                                      EnquiryStatusKey = EL.EnquiryStatusKey ?? 0,
                                                                                      //DepartmentKey = EL.DepartmentKey,
                                                                                      FeedbackCreatedDate = ELF.DateAdded,
                                                                                      Feedback = ELF.EnquiryFeedbackDesc,
                                                                                      CallTypeKey = ELF.CallTypeKey,
                                                                                      NextCallScheduleDate = ELF.EnquiryFeedbackReminderDate,
                                                                                      EnquiryFeedbackReminderStatus = ELF.EnquiryFeedbackReminderStatus ?? false,
                                                                                      CallStatusName = ELF.EnquiryCallStatu.EnquiryCallStatusName,
                                                                                      CallTypeName = ELF.CallType.CallTypeName,
                                                                                      CallDuration = ELF.EnquiryDuration,
                                                                                      CreatedBy = AU.FirstName + " " + (AU.MiddleName ?? "") + " " + AU.LastName,
                                                                                      UserKey = ELF.AddedBy,
                                                                                      CreateOn = ELF.DateAdded,
                                                                                      ScheduleTypeKey = 1,
                                                                                      LastCallStatusKey = ELF.EnquiryCallStatusKey ?? 0,
                                                                                      ScheduleStatusKey = model.ScheduleStatusKey,
                                                                                      IsNew = false,
                                                                                      IsEditable = model.IsEditable,
                                                                                      CouncellingTime = ELF.CouncellingTime,

                                                                                      BranchName = EL.Branch.BranchName,
                                                                                      LocationName = EL.LocationName

                                                                                  });

            if (model.SearchBranchKey != null)
                enquiryLeadListQuery = enquiryLeadListQuery.Where(row => row.BranchKey == model.SearchBranchKey);
            if (model.ScheduleSelectTypeKey == 1)
                enquiryLeadListQuery = enquiryLeadListQuery.Where(x => x.ScheduleTypeKey == 0);
            else if (model.ScheduleSelectTypeKey == 2)
                enquiryLeadListQuery = enquiryLeadListQuery.Where(x => x.ScheduleTypeKey == 1);

            DateTime Today = DateTimeUTC.Now;
            DateTime Tomorrow = DateTimeUTC.Tomorrow;
            var HistoryQuery = enquiryLeadListQuery;

            if (model.SearchCallStatusKey != null)
                enquiryLeadListQuery = enquiryLeadListQuery.Where(row => row.LastCallStatusKey == model.SearchCallStatusKey);

            if (model.SearchScheduleStatusKey != null)
            {
                enquiryLeadListQuery = enquiryLeadListQuery.Where(row => row.EnquiryStatusKey == model.SearchScheduleStatusKey);

            }
            if (model.SearchCallTypeKey != null)
                enquiryLeadListQuery = enquiryLeadListQuery.Where(row => row.CallTypeKey == model.SearchCallTypeKey);





            return enquiryLeadListQuery;
        }
        public EnquiryScheduleViewModel GetScheduleSummary(EnquiryScheduleViewModel model)
        {
            IQueryable<EnquiryScheduleViewModel> Query = ScheduleHistoryQuery(model);
            FillCallTypes(model);
            FillEnquiryLeadCallStatus(model);
            GetEmployeesByBranchId(model);
            var checkDate = DateTimeUTC.Tomorrow;
            var DateToday = DateTimeUTC.Now.AddDays(-1);
            if (Convert.ToInt32(checkDate.DayOfWeek) == 0)
            {
                checkDate = DateTimeUTC.Tomorrow.AddDays(1);
            }


            model.IntrestedList = Query.Where(x => x.LastCallStatusKey == DbConstants.EnquiryCallStatus.Counselling && System.Data.Entity.DbFunctions.TruncateTime(x.NextCallScheduleDate) == System.Data.Entity.DbFunctions.TruncateTime(checkDate)).Select(
            x => new EnquiryViewModel
            {
                EnquiryName = x.Name,
                LocationName = x.LocationName,

                BranchName = x.BranchName,
                LastCallScheduleDate = x.NextCallScheduleDate,
                CouncellingTime = x.CouncellingTime,
                EmployeeKey = x.EmployeeKey,
                CreatedOn = x.CreateOn
            }).GroupBy(row => row.EmployeeKey).Select(row => row.OrderByDescending(E => E.CreatedOn).FirstOrDefault()).ToList();


            model.TodaysCouncelling = dbContext.Applications.OrderByDescending(x => x.DateAdded).Where(x => x.EnquiryKey != null && System.Data.Entity.DbFunctions.TruncateTime(x.DateAdded) == System.Data.Entity.DbFunctions.TruncateTime(DateToday)).Select(
                x => new EnquiryViewModel
                {
                    EnquiryName = x.StudentName,
                    LocationName = x.Enquiry.LocationName,

                    BranchName = x.Branch.BranchName,
                    LastCallScheduleDate = dbContext.EnquiryFeedbacks.Where(row => row.EnquiryKey == x.EnquiryKey).OrderByDescending(m => m.DateAdded).Select(m => m.EnquiryFeedbackReminderDate).FirstOrDefault(),
                    CouncellingTime = dbContext.EnquiryFeedbacks.Where(row => row.EnquiryKey == x.EnquiryKey).OrderByDescending(m => m.DateAdded).Select(m => m.CouncellingTime).FirstOrDefault(),
                    EmployeeKey = x.Enquiry.EmployeeKey,
                    CreatedOn = x.DateAdded,
                    CounselledBy = x.Enquiry.Employee.FirstName + " " + (x.Enquiry.Employee.MiddleName ?? "") + " " + x.Enquiry.Employee.LastName,
                    ScheduledBy = x.Enquiry.Employee.FirstName + " " + (x.Enquiry.Employee.MiddleName ?? "") + " " + x.Enquiry.Employee.LastName,

                }).ToList();


            if (model.SearchFromDate != null && model.SearchToDate != null)
                Query = Query.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.CreateOn) >= System.Data.Entity.DbFunctions.TruncateTime(model.SearchFromDate) && System.Data.Entity.DbFunctions.TruncateTime(row.CreateOn) <= System.Data.Entity.DbFunctions.TruncateTime(model.SearchToDate));
            else if (model.SearchFromDate != null)
                Query = Query.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.CreateOn) >= System.Data.Entity.DbFunctions.TruncateTime(model.SearchFromDate));
            else if (model.SearchToDate != null)
                Query = Query.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.CreateOn) <= System.Data.Entity.DbFunctions.TruncateTime(model.SearchToDate));


            model.CallCountList = Query.Where(row => row.LastCallStatusKey != null && row.EmployeeKey != 0 && row.UserKey != DbConstants.AdminKey).GroupBy(row => new { row.LastCallStatusKey, row.EmployeeKey }).
                                   Select(row => new EnquiryViewModel { EnquiryCallStatusKey = row.Key.LastCallStatusKey, TotalRecords = row.Count(), EmployeeKey = row.Key.EmployeeKey }).ToList();
            model.CallDurationList = Query.Where(row => row.CallTypeKey != null && row.EmployeeKey != 0 && row.UserKey != DbConstants.AdminKey).GroupBy(row => new { row.CallTypeKey, row.EmployeeKey }).
                                   Select(row => new EnquiryViewModel { CallTypeKey = row.Key.CallTypeKey ?? 0, TotalRecords = row.Sum(S => EntityFunctions.DiffSeconds(TimeSpan.Zero, (S.CallDuration ?? TimeSpan.Zero)) ?? 0), EmployeeKey = row.Key.EmployeeKey }).ToList();


            model.CallCountList.AddRange(Query.Where(row => row.LastCallStatusKey != null && row.EmployeeKey != 0 && row.UserKey != DbConstants.AdminKey).GroupBy(row => new { row.EmployeeKey }).
                                   Select(row => new EnquiryViewModel { TotalRecords = row.Select(x => x.RowKey).Distinct().Count(), EmployeeKey = row.Key.EmployeeKey }).ToList());


            model.ProductiveIncomingCallList = Query.Where(row => row.CallTypeKey != null && row.EmployeeKey != 0 && row.UserKey != DbConstants.AdminKey).GroupBy(x => new { x.EmployeeKey }).
                                 Select(row => new EnquiryViewModel { TotalRecords = row.Where(x => x.CallDuration >= (EntityFunctions.AddMinutes(TimeSpan.Zero, 2)) && x.CallTypeKey == DbConstants.CallType.Incoming).Distinct().Count(), EmployeeKey = row.Key.EmployeeKey }).ToList();

            model.ProductiveOutgoingCallList = Query.Where(row => row.CallTypeKey != null && row.EmployeeKey != 0 && row.UserKey != DbConstants.AdminKey).GroupBy(x => new { x.EmployeeKey }).
                                Select(row => new EnquiryViewModel { TotalRecords = row.Where(x => x.CallDuration >= (EntityFunctions.AddMinutes(TimeSpan.Zero, 2)) && x.CallTypeKey == DbConstants.CallType.Outgoing).Distinct().Count(), EmployeeKey = row.Key.EmployeeKey }).ToList();

            model.ProductiveWalkingList = Query.Where(row => row.CallTypeKey != null && row.EmployeeKey != 0 && row.UserKey != DbConstants.AdminKey).GroupBy(x => new { x.EmployeeKey }).
                                Select(row => new EnquiryViewModel { TotalRecords = row.Where(x => x.CallDuration >= (EntityFunctions.AddMinutes(TimeSpan.Zero, 2)) && x.CallTypeKey == DbConstants.CallType.Walking).Distinct().Count(), EmployeeKey = row.Key.EmployeeKey }).ToList();

            return model;

        }
        public EnquiryScheduleViewModel ProductiveCallsHistory(EnquiryScheduleViewModel model)
        {
            List<EmployeeDurationConfig> dbModel = dbContext.EmployeeDurationConfigs.ToList();
            int minDuration = dbModel.Select(x => x.MinDurationCount).Take(1).SingleOrDefault();
            int maxCount = dbModel.OrderByDescending(x => x.TotalDurationCount).Select(x => x.TotalDurationCount).FirstOrDefault() ?? 0;
            string MaxColorCode = dbModel.OrderByDescending(x => x.TotalDurationCount).Select(x => x.ColorCode).FirstOrDefault();

            model.ProductiveCallList = dbContext.EnquiryFeedbacks.Where(row => row.CallTypeKey != null && row.Enquiry.EmployeeKey != 0 && row.AddedBy != DbConstants.AdminKey && System.Data.Entity.DbFunctions.TruncateTime(row.DateAdded).Value.Month == System.Data.Entity.DbFunctions.TruncateTime(DateTimeUTC.Now).Value.Month && row.AddedBy == DbConstants.User.UserKey && System.Data.Entity.DbFunctions.TruncateTime(row.DateAdded).Value.Year == System.Data.Entity.DbFunctions.TruncateTime(DateTimeUTC.Now).Value.Year).GroupBy(row => new { row.Enquiry.EmployeeKey }).
                             Select(row => new EnquiryViewModel { TotalRecords = row.Where(x => x.EnquiryDuration >= (EntityFunctions.AddMinutes(TimeSpan.Zero, minDuration))).Distinct().Count(), EmployeeKey = row.Key.EmployeeKey }).Union(
                             dbContext.EnquiryLeadFeedbacks.Where(row => row.CallTypeKey != null && row.EnquiryLead.EmployeeKey != 0 && row.AddedBy != DbConstants.AdminKey && System.Data.Entity.DbFunctions.TruncateTime(row.DateAdded).Value.Month == System.Data.Entity.DbFunctions.TruncateTime(DateTimeUTC.Now).Value.Month && row.AddedBy == DbConstants.User.UserKey && System.Data.Entity.DbFunctions.TruncateTime(row.DateAdded).Value.Year == System.Data.Entity.DbFunctions.TruncateTime(DateTimeUTC.Now).Value.Year).GroupBy(row => new { row.EnquiryLead.EmployeeKey }).
                             Select(row => new EnquiryViewModel { TotalRecords = row.Where(x => x.CallDuration >= (EntityFunctions.AddMinutes(TimeSpan.Zero, minDuration))).Distinct().Count(), EmployeeKey = row.Key.EmployeeKey })
                             ).ToList();


            model.TotalRecords = model.ProductiveCallList.Select(x => x.TotalRecords).DefaultIfEmpty().Sum();


            if (model.TotalRecords > maxCount)
            {
                model.ColorCode = MaxColorCode;
                return model;
            }

            foreach (EmployeeDurationConfig item in dbModel.OrderByDescending(x => x.RowKey))
            {
                if (item.TotalDurationCount >= model.TotalRecords)
                {
                    model.ColorCode = item.ColorCode;
                }
            }


            return model;

        }
        public int CheckDuration(long EmployeeKey)
        {
            int count = dbContext.EnquiryFeedbacks.Where(x => TimeSpan.Parse(x.EnquiryDuration.ToString()).TotalSeconds > 2).Count();
            return count;
        }

        //public List<EnquiryScheduleViewModel> GetHistoryByMobileNumber(EnquiryScheduleViewModel model)
        //{

        //    var History = ((from r in dbContext.EnquiryFeedbacks
        //                    join AU in dbContext.AppUsers on r.AddedBy equals AU.RowKey
        //                    select new EnquiryScheduleViewModel
        //                    {
        //                        Feedback = r.EnquiryFeedbackDesc,
        //                        MobileNumber = r.Enquiry.MobileNumber,
        //                        CallStatusName = r.EnquiryCallStatu.EnquiryCallStatusName,
        //                        Name = r.Enquiry.EnquiryName,

        //                        CallDuration = r.EnquiryDuration,
        //                        FeedbackCreatedDate = r.DateAdded,
        //                        NextCallScheduleDate = r.EnquiryFeedbackReminderDate,
        //                        CreatedBy = AU.AppUserName,
        //                        LocationName = r.Enquiry.LocationName,
        //                        EnquiryStatusKey = r.Enquiry.EnquiryStatusKey ?? 0




        //                    }).Union(from r in dbContext.EnquiryLeadFeedbacks
        //                             join AU in dbContext.AppUsers on r.AddedBy equals AU.RowKey
        //                             select new EnquiryScheduleViewModel
        //                             {
        //                                 Feedback = r.Feedback,
        //                                 MobileNumber = r.EnquiryLead.MobileNumber,
        //                                 CallStatusName = r.EnquiryCallStatu.EnquiryCallStatusName,
        //                                 Name = r.EnquiryLead.Name,
        //                                 CallDuration = r.CallDuration,
        //                                 FeedbackCreatedDate = r.DateAdded,
        //                                 NextCallScheduleDate = r.NextCallSchedule,
        //                                 CreatedBy = AU.AppUserName,
        //                                 LocationName = "",
        //                                 EnquiryStatusKey = r.EnquiryLead.EnquiryLeadStatusKey ?? 0

        //                             })).Where(x => x.MobileNumber == model.MobileNumber && x.EnquiryStatusKey == DbConstants.EnquiryStatus.Closed).ToList();


        //    return History;
        //}

        //public EnquiryScheduleViewModel GetAllHistoryByMobileNumber(EnquiryScheduleViewModel model)
        //{

        //    MobileNumberSearchViewModel localModel = new MobileNumberSearchViewModel();

        //    localModel.EnquirySchedule = (from r in dbContext.EnquiryFeedbacks
        //                                  join AU in dbContext.AppUsers on r.AddedBy equals AU.RowKey
        //                                  where ((r.Enquiry.MobileNumber == model.MobileNumber && r.Enquiry.TelephoneCodeKey == model.SearchHistoryTelephoneCodeKey) || (r.Enquiry.MobileNumberOptional == model.MobileNumber && r.Enquiry.TelePhoneCodeOptionalkey == model.SearchHistoryTelephoneCodeKey))
        //                                  select new EnquiryScheduleViewModel
        //                                    {
        //                                        Feedback = r.EnquiryFeedbackDesc,
        //                                        MobileNumber = r.Enquiry.MobileNumber,
        //                                        CallStatusName = r.EnquiryCallStatu.EnquiryCallStatusName,
        //                                        Name = r.Enquiry.EnquiryName,
        //                                        CallDuration = r.EnquiryDuration,
        //                                        FeedbackCreatedDate = r.DateAdded,
        //                                        NextCallScheduleDate = r.EnquiryFeedbackReminderDate,
        //                                        CreatedBy = AU.FirstName + " " + (AU.MiddleName ?? "") + " " + AU.LastName,
        //                                        LocationName = r.Enquiry.LocationName,
        //                                        EnquiryStatusKey = r.Enquiry.EnquiryStatusKey ?? 0,
        //                                        ScheduleTypeName = EduSuiteUIResources.Enquiry,
        //                                        TelephoneCodeKey = r.Enquiry.TelephoneCodeKey,
        //                                        BranchName = r.Enquiry.Branch1.BranchName,
        //                                        ScheduledBy = "",
        //                                        CouncellingBy = "",
        //                                        RowKey = r.Enquiry.RowKey
        //                                    }).ToList();


        //    localModel.EnquiryLeadSchedule = (from r in dbContext.EnquiryLeadFeedbacks
        //                                      join AU in dbContext.AppUsers on r.AddedBy equals AU.RowKey
        //                                      where ((r.EnquiryLead.MobileNumber == model.MobileNumber && r.EnquiryLead.TelephoneCodeKey == model.SearchHistoryTelephoneCodeKey) || (r.EnquiryLead.MobileNumberOptional == model.MobileNumber && r.EnquiryLead.TelePhoneCodeOptionalkey == model.SearchHistoryTelephoneCodeKey))
        //                                      select new EnquiryScheduleViewModel
        //                                  {
        //                                      Feedback = r.Feedback,
        //                                      MobileNumber = r.EnquiryLead.MobileNumber,
        //                                      CallStatusName = r.EnquiryCallStatu.EnquiryCallStatusName,
        //                                      Name = r.EnquiryLead.Name,
        //                                      CallDuration = r.CallDuration,
        //                                      FeedbackCreatedDate = r.DateAdded,
        //                                      NextCallScheduleDate = r.NextCallSchedule,
        //                                      CreatedBy = AU.FirstName + " " + (AU.MiddleName ?? "") + " " + AU.LastName,
        //                                      LocationName = "",
        //                                      EnquiryStatusKey = r.EnquiryLead.EnquiryLeadStatusKey ?? 0,
        //                                      ScheduleTypeName = EduSuiteUIResources.EnquiryLead,
        //                                      TelephoneCodeKey = r.EnquiryLead.TelephoneCodeKey ?? 0,
        //                                      BranchName = r.EnquiryLead.Branch.BranchName,
        //                                      ScheduledBy = "",
        //                                      CouncellingBy = "",
        //                                      RowKey = r.EnquiryLead.RowKey
        //                                  }).ToList();



        //    if (localModel.EnquirySchedule.Count > 0)
        //    {

        //        long EnquiryKey = localModel.EnquirySchedule[0].RowKey;
        //        Enquiry EnquiryModel = dbContext.Enquiries.Where(x => x.RowKey == EnquiryKey).SingleOrDefault();
        //        Employee EmployeeModel = dbContext.Employees.Where(x => x.RowKey == EnquiryModel.EmployeeKey).SingleOrDefault();
        //        long EnquiryScheduleUserKey = dbContext.EnquiryFeedbacks.Where(x => x.EnquiryKey == EnquiryModel.RowKey).OrderByDescending(x => x.DateAdded).Select(x => x.AddedBy).FirstOrDefault();
        //        model.UserKeys.Add(EnquiryScheduleUserKey);


        //        localModel.MobileNumber = EnquiryModel.MobileNumber;
        //        localModel.LocationName = EnquiryModel.DistrictName + " district - " + EnquiryModel.LocationName;
        //        localModel.Name = EnquiryModel.EnquiryName;
        //        if (EmployeeModel != null)
        //        {
        //            localModel.ScheduledBy = EmployeeModel.FirstName + " " + (EmployeeModel.MiddleName ?? "") + " " + EmployeeModel.LastName;
        //        }

        //    }

        //    localModel.EnquirySchedule = localModel.EnquirySchedule.Union(localModel.EnquiryLeadSchedule).Union(localModel.ApplicationSchedule).Union(localModel.DocumentSchedule).ToList();
        //    if (localModel.EnquirySchedule.Count > 0)
        //    {
        //        model.MobileNumberSearch.Add(localModel);
        //    }





        //    return model;

        //}

        public List<EnquiryScheduleViewModel> GetHistoryByMobileNumber(EnquiryScheduleViewModel model)
        {

            var History = ((from r in dbContext.EnquiryFeedbacks
                            join AU in dbContext.AppUsers on r.AddedBy equals AU.RowKey
                            select new EnquiryScheduleViewModel
                            {
                                Feedback = r.EnquiryFeedbackDesc,
                                MobileNumber = r.Enquiry.MobileNumber,
                                CallStatusName = r.EnquiryCallStatu.EnquiryCallStatusName,
                                Name = r.Enquiry.EnquiryName,

                                CallDuration = r.EnquiryDuration,
                                FeedbackCreatedDate = r.DateAdded,
                                NextCallScheduleDate = r.EnquiryFeedbackReminderDate,
                                CreatedBy = AU.AppUserName,
                                LocationName = r.Enquiry.LocationName,
                                EnquiryStatusKey = r.Enquiry.EnquiryStatusKey ?? 0




                            }).Union(from r in dbContext.EnquiryLeadFeedbacks
                                     join AU in dbContext.AppUsers on r.AddedBy equals AU.RowKey
                                     select new EnquiryScheduleViewModel
                                     {
                                         Feedback = r.Feedback,
                                         MobileNumber = r.EnquiryLead.MobileNumber,
                                         CallStatusName = r.EnquiryCallStatu.EnquiryCallStatusName,
                                         Name = r.EnquiryLead.Name,
                                         CallDuration = r.CallDuration,
                                         FeedbackCreatedDate = r.DateAdded,
                                         NextCallScheduleDate = r.NextCallSchedule,
                                         CreatedBy = AU.AppUserName,
                                         LocationName = "",
                                         EnquiryStatusKey = r.EnquiryLead.EnquiryLeadStatusKey ?? 0

                                     })).Where(x => x.MobileNumber == model.MobileNumber && x.EnquiryStatusKey == DbConstants.EnquiryStatus.Closed).ToList();


            return History;
        }
        public EnquiryScheduleViewModel GetAllHistoryByMobileNumber(EnquiryScheduleViewModel model)
        {

            try
            {

                MobileNumberSearchViewModel localModel = new MobileNumberSearchViewModel();


                //localModel.EnquiryNewLeadSchedule = (from r in dbContext.EnquiryLeads
                //                                     join AU in dbContext.AppUsers on r.AddedBy equals AU.RowKey
                //                                     where ((r.Country.TelephoneCode + r.MobileNumber == model.MobileNumber) || (r.Country1.TelephoneCode + r.MobileNumberOptional == model.MobileNumber))
                //                                     //where ((r.Country.TelephoneCode + r.MobileNumber.Contains(model.MobileNumber)) || (r.MobileNumberOptional.Contains(model.MobileNumber)))

                //                                     select new EnquiryScheduleViewModel
                //                                     {
                //                                         Feedback = r.Feedback,
                //                                         MobileNumber = r.MobileNumber,
                //                                         CallStatusName = "",
                //                                         Name = r.Name,
                //                                         CallDuration = model.CallDuration,
                //                                         FeedbackCreatedDate = r.DateAdded,
                //                                         NextCallScheduleDate = r.LeadDate,
                //                                         CreatedBy = AU.FirstName + " " + (AU.MiddleName ?? "") + " " + AU.LastName,
                //                                         LocationName = "",
                //                                         EnquiryStatusKey = r.EnquiryLeadStatusKey ?? 0,
                //                                         ScheduleTypeName = EduSuiteUIResources.NewLead,
                //                                         TelephoneCodeKey = r.TelephoneCodeKey ?? 0,
                //                                         BranchName = r.Branch.BranchName,
                //                                         ScheduledBy = "",
                //                                         CouncellingBy = "",
                //                                         RowKey = r.RowKey,
                //                                         ScheduleTypeKey = 0
                //                                     }).ToList();

                localModel.EnquiryNewLeadSchedule = (from r in dbContext.VwEnquiryFeedbackReportSelectAlls.Where(x => x.ScheduleType == 1 && x.FeedbackKey == null).OrderByDescending(y => y.CreatedDate)
                                                     where (r.MobileNumber.Contains(model.MobileNumber))

                                                     select new EnquiryScheduleViewModel
                                                     {
                                                         Feedback = r.Feedback,
                                                         MobileNumber = r.MobileNumber,
                                                         CallStatusName = "",
                                                         Name = r.Name,
                                                         CallDuration = model.CallDuration,
                                                         FeedbackCreatedDate = r.CreatedDate,
                                                         NextCallScheduleDate = r.CreatedDate,
                                                         CreatedBy = r.CreatedBy,
                                                         LocationName = r.Location,
                                                         EnquiryStatusKey = r.EnquiryStatusKey ?? 0,
                                                         ScheduleTypeName = EduSuiteUIResources.NewLead,
                                                         TelephoneCodeKey = r.TelephoneCodeKey ?? 0,
                                                         BranchName = r.Branch,
                                                         ScheduledBy = "",
                                                         CouncellingBy = "",
                                                         RowKey = r.RowKey,
                                                         ScheduleTypeKey = 0
                                                     }).ToList();


                //localModel.EnquiryLeadSchedule = (from r in dbContext.EnquiryLeadFeedbacks
                //                                  join AU in dbContext.AppUsers on r.AddedBy equals AU.RowKey
                //                                  where ((r.EnquiryLead.Country.TelephoneCode + r.EnquiryLead.MobileNumber == model.MobileNumber) || (r.EnquiryLead.Country1.TelephoneCode + r.EnquiryLead.MobileNumberOptional == model.MobileNumber))
                //                                  //where ((r.EnquiryLead.MobileNumber.Contains(model.MobileNumber)) || (r.EnquiryLead.MobileNumberOptional.Contains(model.MobileNumber)))

                //                                  select new EnquiryScheduleViewModel
                //                                  {
                //                                      Feedback = r.Feedback,
                //                                      MobileNumber = r.EnquiryLead.MobileNumber,
                //                                      CallStatusName = r.EnquiryCallStatu.EnquiryCallStatusName,
                //                                      Name = r.EnquiryLead.Name,
                //                                      CallDuration = r.CallDuration,
                //                                      FeedbackCreatedDate = r.DateAdded,
                //                                      NextCallScheduleDate = r.NextCallSchedule,
                //                                      CreatedBy = AU.FirstName + " " + (AU.MiddleName ?? "") + " " + AU.LastName,
                //                                      LocationName = "",
                //                                      EnquiryStatusKey = r.EnquiryLead.EnquiryLeadStatusKey ?? 0,
                //                                      ScheduleTypeName = EduSuiteUIResources.EnquiryLead,
                //                                      TelephoneCodeKey = r.EnquiryLead.TelephoneCodeKey ?? 0,
                //                                      BranchName = r.EnquiryLead.Branch.BranchName,
                //                                      ScheduledBy = "",
                //                                      CouncellingBy = "",
                //                                      RowKey = r.EnquiryLead.RowKey,
                //                                      ScheduleTypeKey = 1
                //                                  }).ToList();

                localModel.EnquiryLeadSchedule = (from r in dbContext.VwEnquiryFeedbackReportSelectAlls.Where(x => x.ScheduleType == 1 && x.FeedbackKey != null).OrderByDescending(y => y.CalledDate)
                                                  where (r.MobileNumber.Contains(model.MobileNumber))

                                                  select new EnquiryScheduleViewModel
                                                  {
                                                      Feedback = r.Feedback,
                                                      MobileNumber = r.MobileNumber,
                                                      CallStatusName = r.EnquiryCallStatusName,
                                                      Name = r.Name,
                                                      CallDuration = r.CallDuration,
                                                      FeedbackCreatedDate = r.CalledDate,
                                                      NextCallScheduleDate = r.NextScheduleDate,
                                                      CreatedBy = r.CreatedBy,
                                                      LocationName = r.Location,
                                                      EnquiryStatusKey = r.EnquiryStatusKey ?? 0,
                                                      ScheduleTypeName = EduSuiteUIResources.EnquiryLead,
                                                      TelephoneCodeKey = r.TelephoneCodeKey ?? 0,
                                                      BranchName = r.Branch,
                                                      ScheduledBy = "",
                                                      CouncellingBy = "",
                                                      RowKey = r.RowKey,
                                                      ScheduleTypeKey = 1
                                                  }).ToList();


                //localModel.EnquirySchedule = (from r in dbContext.EnquiryFeedbacks
                //                              join AU in dbContext.AppUsers on r.AddedBy equals AU.RowKey
                //                              join EN in dbContext.Enquiries on r.EnquiryKey equals EN.RowKey
                //                              where ((r.Enquiry.Country.TelephoneCode + r.Enquiry.MobileNumber == model.MobileNumber) || (r.Enquiry.Country.TelephoneCode + r.Enquiry.MobileNumberOptional == model.MobileNumber))
                //                              //where ((dbContext.Countries.Where(x => x.RowKey == EN.TelephoneCodeKey).Select(y => y.TelephoneCode).FirstOrDefault() + r.Enquiry.MobileNumber == model.MobileNumber) || (dbContext.Countries.Where(x => x.RowKey == EN.TelePhoneCodeOptionalkey).Select(y => y.TelephoneCode).FirstOrDefault() + r.Enquiry.MobileNumberOptional == model.MobileNumber))
                //                              //where ((r.Enquiry.MobileNumber.Contains( model.MobileNumber)) || (r.Enquiry.MobileNumberOptional.Contains(model.MobileNumber)))
                //                              select new EnquiryScheduleViewModel
                //                              {
                //                                  Feedback = r.EnquiryFeedbackDesc,
                //                                  MobileNumber = r.Enquiry.MobileNumber,
                //                                  CallStatusName = r.EnquiryCallStatu.EnquiryCallStatusName,
                //                                  Name = r.Enquiry.EnquiryName,
                //                                  CallDuration = r.EnquiryDuration,
                //                                  FeedbackCreatedDate = r.DateAdded,
                //                                  NextCallScheduleDate = r.EnquiryFeedbackReminderDate,
                //                                  CreatedBy = AU.FirstName + " " + (AU.MiddleName ?? "") + " " + AU.LastName,
                //                                  LocationName = r.Enquiry.LocationName,
                //                                  EnquiryStatusKey = r.Enquiry.EnquiryStatusKey ?? 0,
                //                                  ScheduleTypeName = EduSuiteUIResources.Enquiry,
                //                                  TelephoneCodeKey = r.Enquiry.TelephoneCodeKey,
                //                                  BranchName = r.Enquiry.Branch.BranchName,
                //                                  ScheduledBy = "",
                //                                  CouncellingBy = "",
                //                                  RowKey = r.Enquiry.RowKey,
                //                                  ScheduleTypeKey = 2
                //                              }).ToList();

                localModel.EnquirySchedule = (from r in dbContext.VwEnquiryFeedbackReportSelectAlls.Where(x => x.ScheduleType == 2).OrderByDescending(y => y.CalledDate)
                                              where (r.MobileNumber.Contains(model.MobileNumber))
                                              select new EnquiryScheduleViewModel
                                              {
                                                  Feedback = r.Feedback,
                                                  MobileNumber = r.MobileNumber,
                                                  CallStatusName = r.EnquiryCallStatusName,
                                                  Name = r.Name,
                                                  CallDuration = r.CallDuration,
                                                  FeedbackCreatedDate = r.CalledDate,
                                                  NextCallScheduleDate = r.NextScheduleDate,
                                                  CreatedBy = r.CreatedBy,
                                                  LocationName = r.Location,
                                                  EnquiryStatusKey = r.EnquiryStatusKey ?? 0,
                                                  ScheduleTypeName = EduSuiteUIResources.Enquiry,
                                                  TelephoneCodeKey = r.TelephoneCodeKey ?? 0,
                                                  BranchName = r.Branch,
                                                  ScheduledBy = "",
                                                  CouncellingBy = "",
                                                  RowKey = r.RowKey,
                                                  ScheduleTypeKey = 2
                                              }).ToList();


                VwEnquiryFeedbackReportSelectAll VwEnquiryFeedbackReportSelectAll = dbContext.VwEnquiryFeedbackReportSelectAlls.Where(r => r.ScheduleType == 2 && r.MobileNumber.Contains(model.MobileNumber)).FirstOrDefault();
                Application applicationModel = null;
                if (VwEnquiryFeedbackReportSelectAll != null)
                {
                    applicationModel = dbContext.Applications.Where(r => r.EnquiryKey == VwEnquiryFeedbackReportSelectAll.RowKey).FirstOrDefault();
                }
                if (applicationModel != null)
                {

                    Enquiry EnquiryModel = dbContext.Enquiries.Where(x => x.RowKey == applicationModel.EnquiryKey).SingleOrDefault();
                    if (EnquiryModel != null)
                    {
                        Employee EmployeeModel = dbContext.Employees.Where(x => x.RowKey == EnquiryModel.EmployeeKey).SingleOrDefault();
                        if (EmployeeModel != null)
                        {
                            localModel.ScheduledBy = EmployeeModel.FirstName + " " + (EmployeeModel.MiddleName ?? "") + " " + EmployeeModel.LastName;
                        }
                    }



                    //localModel.CouncellingBy = applicationModel.Employee.FirstName + " " + (applicationModel.Employee.MiddleName ?? "") + " " + applicationModel.Employee.LastName;
                    localModel.MobileNumber = applicationModel.StudentMobile;

                    localModel.ApplicationStatusName = applicationModel.StudentStatu.StudentStatusName;
                    localModel.Name = localModel.Name = (applicationModel.StudentName ?? "") + " " + applicationModel.StudentName;
                    localModel.ApplicationStatusKey = applicationModel.StudentStatusKey ?? 0;

                    if (localModel.ApplicationSchedule.Count == 0)
                    {
                        EnquiryScheduleViewModel newmod = new EnquiryScheduleViewModel();
                        localModel.ApplicationSchedule.Add(newmod);
                    }

                }
                else if (localModel.EnquirySchedule.Count > 0)
                {

                    long EnquiryKey = localModel.EnquirySchedule[0].RowKey;
                    Enquiry EnquiryModel = dbContext.Enquiries.Where(x => x.RowKey == EnquiryKey).SingleOrDefault();
                    Employee EmployeeModel = dbContext.Employees.Where(x => x.RowKey == EnquiryModel.EmployeeKey).SingleOrDefault();
                    long EnquiryScheduleUserKey = dbContext.EnquiryFeedbacks.Where(x => x.EnquiryKey == EnquiryModel.RowKey).OrderByDescending(x => x.DateAdded).Select(x => x.AddedBy).FirstOrDefault();
                    if (EmployeeModel != null)
                    {
                        localModel.CouncellingBy = EmployeeModel.FirstName + " " + (EmployeeModel.MiddleName ?? "") + " " + EmployeeModel.LastName;
                    }

                    localModel.MobileNumber = EnquiryModel.MobileNumber;
                    localModel.LocationName = EnquiryModel.DistrictName + " district - " + EnquiryModel.LocationName;

                    localModel.Name = (EnquiryModel.EnquiryName ?? "") + " " + EnquiryModel.CompanyName;
                    if (EmployeeModel != null)
                    {
                        localModel.ScheduledBy = EmployeeModel.FirstName + " " + (EmployeeModel.MiddleName ?? "") + " " + EmployeeModel.LastName;
                    }

                }
                else if (localModel.EnquiryLeadSchedule.Count > 0)
                {
                    long EnquiryLeadKey = localModel.EnquiryLeadSchedule[0].RowKey;
                    EnquiryLead EnquiryLeadModel = dbContext.EnquiryLeads.Where(x => x.RowKey == EnquiryLeadKey).SingleOrDefault();
                    localModel.MobileNumber = EnquiryLeadModel.MobileNumber;
                    localModel.LocationName = EnquiryLeadModel.District + " district - " + EnquiryLeadModel.Location;
                    localModel.Name = (EnquiryLeadModel.Name ?? "");

                    Employee EmployeeModel = dbContext.Employees.Where(x => x.RowKey == EnquiryLeadModel.EmployeeKey).SingleOrDefault();
                    if (EmployeeModel != null)
                    {
                        localModel.CouncellingBy = EmployeeModel.FirstName + " " + (EmployeeModel.MiddleName ?? "") + " " + EmployeeModel.LastName;
                    }
                }
                else if (localModel.EnquiryNewLeadSchedule.Count > 0)
                {
                    long EnquiryLeadKey = localModel.EnquiryNewLeadSchedule[0].RowKey;
                    EnquiryLead EnquiryLeadModel = dbContext.EnquiryLeads.Where(x => x.RowKey == EnquiryLeadKey).SingleOrDefault();
                    localModel.MobileNumber = EnquiryLeadModel.MobileNumber;
                    localModel.LocationName = EnquiryLeadModel.District + " district - " + EnquiryLeadModel.Location;
                    localModel.Name = (EnquiryLeadModel.Name ?? "");
                    Employee EmployeeModel = dbContext.Employees.Where(x => x.RowKey == EnquiryLeadModel.EmployeeKey).SingleOrDefault();
                    if (EmployeeModel != null)
                    {
                        localModel.CouncellingBy = EmployeeModel.FirstName + " " + (EmployeeModel.MiddleName ?? "") + " " + EmployeeModel.LastName;
                    }
                }


                var List = localModel.EnquirySchedule.Union(localModel.EnquiryLeadSchedule.Union(localModel.EnquiryNewLeadSchedule)).Union(localModel.ApplicationSchedule).ToList();
                localModel.EnquirySchedule = List;
                if (localModel.EnquirySchedule.Count > 0)
                {
                    model.MobileNumberSearch.Add(localModel);
                }
                //}

                var TelephoneCodeNumber = dbContext.Countries.Where(x => x.RowKey == model.SearchHistoryTelephoneCodeKey).Select(x => x.TelephoneCode).SingleOrDefault();

            }

            catch (Exception ex)
            {

            }

            return model;
        }
        private string VerifyData(string Data)
        {
            if (Data != null && Data != "")
            {
                Data = "%" + Data + "%";
            }
            else
            {
                Data = "%";
            }
            return Data;
        }
        public void FillTelephoneCodes(EnquiryScheduleViewModel model)
        {
            model.TelephoneCodes = dbContext.VwCountrySelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.TelephoneCode
            }).ToList();



        }

        public EnquiryScheduleViewModel GetFHDropDownLists(EnquiryScheduleViewModel model)
        {
            FillBranches(model);
            GetEmployeesByBranchId(model);
            FillCallTypes(model);
            FillFHEnquiryStatuses(model);
            FillFHCallStatuses(model);
            return model;
        }

        private void FillFHEnquiryStatuses(EnquiryScheduleViewModel model)
        {
            List<short> LeadStatuses = new List<short>();
            LeadStatuses.Add(DbConstants.EnquiryStatus.FollowUp);
            LeadStatuses.Add(DbConstants.EnquiryStatus.Closed);
            LeadStatuses.Add(DbConstants.EnquiryStatus.Intersted);

            if (model.ModuleKey == DbConstants.EnquiryModule.Enquiry)
            {

                if (model.ScheduleSelectTypeKey == 1)
                {
                    LeadStatuses.Remove(DbConstants.EnquiryStatus.Intersted);
                }


                model.FHEnquiryStatuses = dbContext.EnquiryStatus.Where(x => LeadStatuses.Contains(x.RowKey)).Select(row => new SelectListModel
                {
                    RowKey = row.RowKey,
                    Text = row.EnquiryStatusName
                }).ToList();
            }
            else
            {
                LeadStatuses.Remove(DbConstants.EnquiryStatus.FollowUp);

                model.FHEnquiryStatuses = dbContext.EnquiryStatus.Where(x => LeadStatuses.Contains(x.RowKey)).Select(row => new SelectListModel
                {
                    RowKey = row.RowKey,
                    Text = row.EnquiryStatusName
                }).ToList();
            }
        }
        public EnquiryScheduleViewModel FillFHCallStatuses(EnquiryScheduleViewModel model)
        {
            model.FHCallStatuses = dbContext.EnquiryCallStatus.Where(row => row.IsActive && row.EnquiryStatusKey == model.FHEnquiryStatusKey).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.EnquiryCallStatusName
            }).ToList();
            return model;
        }
    }
}
