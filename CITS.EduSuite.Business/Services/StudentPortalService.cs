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

namespace CITS.EduSuite.Business.Services
{
    public class StudentPortalService : IStudentPortalService
    {
        private EduSuiteDatabase dbContext;

        public StudentPortalService(EduSuiteDatabase objDb)
        {
            this.dbContext = objDb;
        }
        public string GetPersonalDetails(long ApplicationKey)
        {


            List<string> PersonalDetails = dbContext.Database.SqlQuery<string>("SP_ViewApplication @ApplicationKey",
                                                                              new SqlParameter("ApplicationKey", ApplicationKey)).ToList();
            return string.Join(",", PersonalDetails);

        }

        #region Attendance Details
        public string GetAttendanceDetails(long Applicationkey, long? ClassDetailsKey)
        {
            IEnumerable<string> AttendanceDetails = dbContext.Database.SqlQuery<string>("Sp_ApplicationDashBoardDetails @ApplicationKey,@ClassDetailsKey,@TemplateKey",
                                                                            new SqlParameter("ApplicationKey", Applicationkey),
                                                                             new SqlParameter("ClassDetailsKey", ClassDetailsKey),
                                                                            new SqlParameter("TemplateKey", 1));
            return string.Join(",", AttendanceDetails);
        }


        #endregion Attendance Details

        public string GetAllExamResults(long ApplicationKey)
        {


            List<string> ExamResults = dbContext.Database.SqlQuery<string>("GetStudentAllExamResults @ApplicationKey",
                                                                              new SqlParameter("ApplicationKey", ApplicationKey)).ToList();
            return string.Join("", ExamResults);

        }
      


        public string GetUnitExamResults(long ApplicationKey, long SubjectKey)
        {


            List<string> UnitExamResults = dbContext.Database.SqlQuery<string>("Sp_ApplicationUnitTestResult @ApplicationKey,@SubjectKey",
                                                                              new SqlParameter("ApplicationKey", ApplicationKey),
                                                                              new SqlParameter("SubjectKey", SubjectKey)).ToList();

            return string.Join("", UnitExamResults);

        }
        public string GetNotification(long Applicationkey, long? ClassDetailsKey)
        {
            List<string> Notification = dbContext.Database.SqlQuery<string>("Sp_ApplicationDashBoardDetails @ApplicationKey,@ClassDetailsKey,@TemplateKey",
                                                                            new SqlParameter("ApplicationKey", Applicationkey),
                                                                             new SqlParameter("ClassDetailsKey", ClassDetailsKey),
                                                                            new SqlParameter("TemplateKey", 2)).ToList();
            return string.Join("", Notification);
        }
        public string BindTotalFeeDetails(long Applicationkey, long? ClassDetailsKey)
        {
            List<string> FeeDetails = dbContext.Database.SqlQuery<string>("Sp_ApplicationDashBoardDetails @ApplicationKey,@ClassDetailsKey,@TemplateKey",
                                                                           new SqlParameter("ApplicationKey", Applicationkey),
                                                                            new SqlParameter("ClassDetailsKey", ClassDetailsKey),
                                                                           new SqlParameter("TemplateKey", 3)).ToList();
            return string.Join("", FeeDetails);
        }
        public string BindInstallmentFeeDetails(long Applicationkey, long? ClassDetailsKey)
        {
            List<string> FeeDetails = dbContext.Database.SqlQuery<string>("Sp_ApplicationDashBoardDetails @ApplicationKey,@ClassDetailsKey,@TemplateKey",
                                                                           new SqlParameter("ApplicationKey", Applicationkey),
                                                                            new SqlParameter("ClassDetailsKey", ClassDetailsKey),
                                                                           new SqlParameter("TemplateKey", 4)).ToList();
            return string.Join("", FeeDetails);
        }
        public string BindFeePaymentDetails(long Applicationkey, long? ClassDetailsKey)
        {
            List<string> FeePaymentDetails = dbContext.Database.SqlQuery<string>("Sp_ApplicationDashBoardDetails @ApplicationKey,@ClassDetailsKey,@TemplateKey",
                                                                           new SqlParameter("ApplicationKey", Applicationkey),
                                                                            new SqlParameter("ClassDetailsKey", ClassDetailsKey),
                                                                           new SqlParameter("TemplateKey", 5)).ToList();
            return string.Join("", FeePaymentDetails);
        }
    }



}


