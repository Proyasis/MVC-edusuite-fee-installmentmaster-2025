using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Services
{
    public class ESSLAttendanceService : IESSLAttendanceService
    {

        private EduSuiteDatabase dbContext = new EduSuiteDatabase();


        public ESSLAttendanceService()
        {
            //this.dbContext = objDB;
        }
        public DataTable PullAttendanceData(ESSLAttendanceViewModel model)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["EsslConnection"].ConnectionString.ToString());

            DataTable dataTable = new DataTable();
            //string query = "select * from AttendanceLogs";
            //SqlCommand cmd = new SqlCommand(query, conn);
            //conn.Open();           
            //SqlDataAdapter da = new SqlDataAdapter(cmd);           
            //da.Fill(dataTable);
            //conn.Close();
            //da.Dispose();

            conn.Open();
            SqlCommand cmd = new SqlCommand("AttendanceDetails");
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.Add("@AttendanceDate", SqlDbType.VarChar).Value = model.AttendanceDate;
            //cmd.Parameters.Add("@AttendanceLogId", SqlDbType.Int).Value = model.AttendanceLogId;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dataTable);
            conn.Close();
            da.Dispose();
            return dataTable;
        }
        public ESSLStudentsViewModel GetEmployeeDetails(List<ESSLStudentsViewModel> StudentsList1)
        {
            ESSLStudentsViewModel model = new ESSLStudentsViewModel();


            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    if (StudentsList1.Count > 0)
                    {
                        long MaxKey = dbContext.EsslStudents.Select(p => p.RowKey).DefaultIfEmpty().Max();
                        foreach (ESSLStudentsViewModel item in StudentsList1)
                        {
                            EsslStudent dbEsslStudent = new EsslStudent();
                            var esslStudents = dbContext.EsslStudents.Where(row => row.EmployeeId == item.EmployeeId && row.EmployeeCode == item.EmployeeCode).Count();

                            if (esslStudents == 0)
                            {
                                dbEsslStudent.RowKey = MaxKey + 1;
                                dbEsslStudent.EmployeeId = item.EmployeeId;
                                dbEsslStudent.EmployeeCode = item.EmployeeCode;
                                dbEsslStudent.EmployeeName = item.EmployeeName;
                                dbEsslStudent.Gender = item.Gender;
                                dbEsslStudent.Status = item.Status;
                                dbEsslStudent.IsConnected = false;
                                dbContext.EsslStudents.Add(dbEsslStudent);
                                MaxKey++;
                            }
                        }
                        dbContext.SaveChanges();
                        transaction.Commit();
                        model.Message = EduSuiteUIResources.Success;
                        model.IsSuccessful = true;
                        //ActivityLog.CreateActivityLog(MenuConstants.AcademicTerm, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);
                    }
                    else
                    {
                        model.IsSuccessful = false;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.EsslStudent);
                    model.IsSuccessful = false;
                    //ActivityLog.CreateActivityLog(MenuConstants.AcademicTerm, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public ESSLAttendanceViewModel GetAttendanceDetails(List<ESSLAttendanceViewModel> AttendanceList)
        {
            ESSLAttendanceViewModel model = new ESSLAttendanceViewModel();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    if (AttendanceList.Count > 0)
                    {
                        long MaxKey = dbContext.Attendances.Select(p => p.RowKey).DefaultIfEmpty().Max();
                        long AttendanceDetailsMaxKey = dbContext.AttendanceDetails.Select(p => p.RowKey).DefaultIfEmpty().Max();
                        foreach (ESSLAttendanceViewModel item in AttendanceList)
                        {
                            Application application = dbContext.Applications.FirstOrDefault(x => x.EsslStudent.EmployeeId == item.EmployeeId && x.ClassDetailsKey != null);
                            if (application != null)
                            {
                                ClassDetail dbclassdetails = dbContext.ClassDetails.FirstOrDefault(x => x.RowKey == application.ClassDetailsKey);
                                GeneralConfiguration dvgeneralConfiguration = dbContext.GeneralConfigurations.FirstOrDefault();
                                Attendance AttendanceModel = new Attendance();
                                AttendanceModel.RowKey = ++MaxKey;
                                AttendanceModel.AttendanceDate = item.AttendanceDate;
                                AttendanceModel.BranchKey = application.BranchKey;
                                AttendanceModel.ClassDetailsKey = application.ClassDetailsKey ?? 0;
                                AttendanceModel.BatchKey = application.BatchKey;
                                AttendanceModel.EmployeeKey = null;
                                AttendanceModel.ApplicationKey = application.RowKey;
                                AttendanceModel.RollNumber = application.RollNumber;
                                AttendanceModel.AttendanceLogId = item.AttendanceLogId;
                                dbContext.Attendances.Add(AttendanceModel);


                                AttendanceDetail attendanceDetailModel = new AttendanceDetail();

                                attendanceDetailModel.RowKey = ++AttendanceDetailsMaxKey;
                                attendanceDetailModel.AttendanceTypeKey = DbConstants.AttendanceType.OneTime;
                                attendanceDetailModel.AttendanceStatus = true;
                                attendanceDetailModel.Remarks = "";
                                attendanceDetailModel.AttendanceMasterKey = AttendanceModel.RowKey;

                                TimeSpan? ClassStartTime = dbclassdetails.StartTime != null ? dbclassdetails.StartTime : dvgeneralConfiguration.ClassStartTime;
                                TimeSpan? ClassEndTime = dbclassdetails.EndTime != null ? dbclassdetails.EndTime : dvgeneralConfiguration.ClassEndTime;
                                int? LateMinutes = dbclassdetails.LateMinutes != null ? dbclassdetails.LateMinutes : dvgeneralConfiguration.ClassLateMinutes;

                                DateTime DateBeginTime = item.AttendanceDate.Date;
                                DateTime Intime = Convert.ToDateTime(item.InTime);

                                DateBeginTime = DateBeginTime.Add((ClassStartTime ?? TimeSpan.Zero));
                                int TotalLate = (int)(Intime - DateBeginTime).TotalMinutes;
                                if (TotalLate > LateMinutes)
                                {
                                    attendanceDetailModel.AttendanceStatusKey = DbConstants.AttendanceStatus.HalfDay;
                                }
                                else
                                {
                                    attendanceDetailModel.AttendanceStatusKey = DbConstants.AttendanceStatus.Present;
                                }
                                attendanceDetailModel.ClassInTime = ClassStartTime;
                                attendanceDetailModel.PunchInTime = Intime;
                                attendanceDetailModel.LateMinutes = TotalLate;
                                dbContext.AttendanceDetails.Add(attendanceDetailModel);
                            }
                            //else
                            //{
                            //    model.IsSuccessful = false;
                            //    return model;
                            //}

                        }
                        dbContext.SaveChanges();
                        transaction.Commit();
                        model.Message = EduSuiteUIResources.Success;
                        model.IsSuccessful = true;
                        //ActivityLog.CreateActivityLog(MenuConstants.AcademicTerm, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);
                    }
                    else
                    {
                        model.IsSuccessful = false;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.EsslStudent);
                    model.IsSuccessful = false;
                    //ActivityLog.CreateActivityLog(MenuConstants.AcademicTerm, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public List<ESSLStudentsDetailsViewModel> GetESSLStudents(ESSLStudentsDetailsViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;

                IQueryable<ESSLStudentsDetailsViewModel> applicationList = (from a in dbContext.EsslStudents
                                                                            where (a.EmployeeCode.Contains(model.EmployeeCode) || a.EmployeeName.Contains(model.EmployeeCode))
                                                                            select new ESSLStudentsDetailsViewModel
                                                                            {
                                                                                RowKey = a.RowKey,
                                                                                EmployeeName = a.EmployeeName,
                                                                                EmployeeCode = a.EmployeeCode,
                                                                                Gender = a.Gender,
                                                                                Status = a.Status,
                                                                                IsConnected = a.IsConnected,
                                                                                AdmissionNo = dbContext.Applications.Where(x => x.BioMetricsId == a.RowKey).Select(y => y.AdmissionNo).FirstOrDefault()
                                                                            });

                if (model.IsConnected)
                {
                    applicationList = applicationList.Where(row => row.IsConnected);
                }
                applicationList = applicationList.GroupBy(x => x.RowKey).Select(y => y.FirstOrDefault());
                if (model.SortBy != "")
                {
                    applicationList = SortApplications(applicationList, model.SortBy, model.SortOrder);
                }
                TotalRecords = applicationList.Count();
                return applicationList.Skip(Skip ?? 0).Take(Take ?? 0).ToList<ESSLStudentsDetailsViewModel>();
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                //ActivityLog.CreateActivityLog(MenuConstants.ApplicationWebForm, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<ESSLStudentsDetailsViewModel>();

            }
        }
        private IQueryable<ESSLStudentsDetailsViewModel> SortApplications(IQueryable<ESSLStudentsDetailsViewModel> Query, string SortName, string SortOrder)
        {
            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(ESSLStudentsDetailsViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<ESSLStudentsDetailsViewModel>(resultExpression);
        }

    }
}
