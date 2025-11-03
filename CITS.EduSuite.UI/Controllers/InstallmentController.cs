using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Security;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

using System.Configuration;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace CITS.EduSuite.UI.Controllers
{
    public class InstallmentController : Controller
    {

        public ActionResult AddInstallmentEntry()
        {
            var model = new InstallmentScheduleViewModel
            {
                ScheduleList = new List<InstallmentScheduleViewModel>()
            };

            return View(model);
        }
  public ActionResult GetInstallmentRow(long id, string year)
{
    // year parameter use panni, dynamic-aa row prepare pannunga
    var model = new InstallmentScheduleViewModel
    {
        YearLabel = year,
        InstallmentId = id
    };

    return PartialView("_InstallmentRow", model);
}


        [HttpGet]
        public ActionResult FeeInstallmentList()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["EduSuiteADO"].ConnectionString;
            var list = new List<InstallmentListItem>();

            using (var conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT fi.*, at.AcademicTermName, c.CourseName, d.BatchName , e.CourseTypeName,f.UniversityMasterName,g.FeeTypeName
                         FROM Fee_Installment fi
                         INNER JOIN AcademicTerm at ON at.RowKey = fi.AcademicTermKey
                         INNER JOIN Course c ON c.RowKey = fi.CourseKey
                          INNER JOIN Batch d ON d.RowKey = fi.BatchKey
                        INNER JOIN CourseType e ON e.RowKey = fi.CourseTypeKey
						 INNER JOIN UniversityMaster f ON f.RowKey = fi.UniversityKey
 LEFT JOIN FeeType g ON g.RowKey = fi.FeeTypeKey
						ORDER BY fi.CreatedOn DESC";

                using (var cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var vm = new InstallmentListItem
                            {
                                UniversityMasterName = reader["UniversityMasterName"].ToString(),
                                AcademicTermName = reader["AcademicTermName"].ToString(),
                                CourseName = reader["CourseName"].ToString(),
                                FeeAmount = Convert.ToDecimal(reader["FeeAmount"]),
                                InitialPayment = Convert.ToDecimal(reader["InitialPayment"]),
                                BalancePayment = Convert.ToDecimal(reader["BalancePayment"]),
                                InstallmentId = Convert.ToInt64(reader["InstallmentId"]),
                            };
                            list.Add(vm);
                        }
                    }
                }
            }

            return View(list);
        }


        private long GetLatestInstallmentId()
        {
            string query = "SELECT MAX(InstallmentId) FROM Fee_Installment";
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["EduSuiteADO"].ConnectionString))
            {
                using (var cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    return Convert.ToInt64(cmd.ExecuteScalar());
                }
            }
        }

        private IApplicationService applicationService;


        private readonly IApplicationPersonalService applicationPersonalService;

        public InstallmentController(IApplicationService applicationService, IApplicationPersonalService applicationPersonalService)
        {
            this.applicationService = applicationService;
            this.applicationPersonalService = applicationPersonalService;
        }

        [HttpGet]
        public ActionResult AddInstallmentEntry(long? id)
        {
            var model = new InsatallmentViewModel();
            var dataModel = new ApplicationPersonalViewModel();

            dataModel = applicationPersonalService.FillBatches(dataModel);
            dataModel = applicationPersonalService.FillAcademicTerm(dataModel);
            dataModel = applicationPersonalService.GetCourseType1(dataModel);
            dataModel = applicationPersonalService.GetCourseByCourseType1(dataModel);
            dataModel = applicationPersonalService.GetUniversity1(dataModel);

            model.Batches = dataModel.Batches;
            model.AcademicTerms = dataModel.AcademicTerms;
            model.CourseTypes = dataModel.CourseTypes;
            model.Courses = dataModel.Courses;
            model.Universities = dataModel.Universities;

            string connectionString = ConfigurationManager.ConnectionStrings["EduSuiteADO"].ConnectionString;
            var feeTypes = new List<SelectListItem>();

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                
                string feeTypeQuery = "SELECT RowKey AS FeeTypeKey, FeeTypeName FROM FeeType";
                using (var cmd = new SqlCommand(feeTypeQuery, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            feeTypes.Add(new SelectListItem
                            {
                                Value = reader["FeeTypeKey"].ToString(),
                                Text = reader["FeeTypeName"].ToString()
                            });
                        }
                    }
                }

                model.FeeTypes = feeTypes;

               
                if (id.HasValue)
                {
                    string durationQuery = @"
SELECT d.YearLabel, d.RegistrationFee, d.AdmissionFee, d.TuitionFee,
       d.BalancePayment, d.IntialPayment, d.FeeTypeKey, f.FeeTypeName
FROM Fee_Installment_Duration d
LEFT JOIN FeeType f ON d.FeeTypeKey = f.RowKey
WHERE d.InstallmentId = @id";
                    using (var durationCmd = new SqlCommand(durationQuery, conn))
                    {
                        durationCmd.Parameters.AddWithValue("@id", id.Value);
                        using (var durationReader = durationCmd.ExecuteReader())
                        {
                            var durations = new List<FeeDuration>();
                            while (durationReader.Read())
                            {
                                durations.Add(new FeeDuration
                                {
                                    YearLabel = durationReader["YearLabel"].ToString(),
                                    RegistrationFee = durationReader["RegistrationFee"] != DBNull.Value ? Convert.ToDecimal(durationReader["RegistrationFee"]) : (decimal?)null,
                                    AdmissionFee = durationReader["AdmissionFee"] != DBNull.Value ? Convert.ToDecimal(durationReader["AdmissionFee"]) : (decimal?)null,
                                    TuitionFee = Convert.ToDecimal(durationReader["TuitionFee"]),
                                    BalancePayment = durationReader["BalancePayment"].ToString(),
                                    IntialPayment = durationReader["IntialPayment"].ToString(),
                                    FeeTypeKey = durationReader["FeeTypeKey"].ToString(),
                                  
                                    FeeTypeName = durationReader["FeeTypeName"].ToString()
                                });
                            }


                            model.FeeDurations = durations;
                        }
                    }

                  
                    string query = @"SELECT * FROM Fee_Installment WHERE InstallmentId = @id";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id.Value);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                model.InstallmentId = id.Value;
                                model.SelectedBatchKey = Convert.ToInt16(reader["BatchKey"]);
                                model.SelectedAcademicTermKey = Convert.ToInt32(reader["AcademicTermKey"]);
                                model.SelectedCourseTypeKey = Convert.ToInt16(reader["CourseTypeKey"]);
                                model.SelectedCourseKey = Convert.ToInt64(reader["CourseKey"]);
                                model.SelectedUniversityKey = Convert.ToInt64(reader["UniversityKey"]);
                                model.FeeAmount = Convert.ToDecimal(reader["FeeAmount"]);
                                model.InitialPayment = Convert.ToDecimal(reader["InitialPayment"]);
                                model.BalancePayment = Convert.ToDecimal(reader["BalancePayment"]);
                                model.SelectedFeeTypeKey = Convert.ToInt64(reader["FeeTypeKey"]);
                            }
                        }
                    }

                   
                    string scheduleQuery = "SELECT * FROM Installment_Schedule WHERE InstallmentId = @id ORDER BY ScheduleId";
                    using (var scheduleCmd = new SqlCommand(scheduleQuery, conn))
                    {
                        scheduleCmd.Parameters.AddWithValue("@id", id.Value);
                        using (var reader = scheduleCmd.ExecuteReader())
                        {
                            var schedules = new List<InstallmentScheduleViewModel>();
                            while (reader.Read())
                            {
                                schedules.Add(new InstallmentScheduleViewModel
                                {
                                    ScheduleId = Convert.ToInt64(reader["ScheduleId"]),
                                    InstallmentMonth = reader["InstallmentMonth"].ToString(),
                                    PaymentDay = reader["PaymentDay"] != DBNull.Value ? Convert.ToInt32(reader["PaymentDay"]) : (int?)null,
                                    DueDuration = reader["DueDuration"] != DBNull.Value ? Convert.ToInt32(reader["DueDuration"]) : (int?)null,
                                    Amount = reader["Amount"] != DBNull.Value ? Convert.ToDecimal(reader["Amount"]) : (decimal?)null,
                                    DueFineAmount = reader["DueFineAmount"] != DBNull.Value ? Convert.ToDecimal(reader["DueFineAmount"]) : (decimal?)null,
                                    SuperFineAmount = reader["SuperFineAmount"] != DBNull.Value ? Convert.ToDecimal(reader["SuperFineAmount"]) : (decimal?)null,
                                    AutoSMS = reader["AutoSMS"] != DBNull.Value && Convert.ToBoolean(reader["AutoSMS"]),
                                    AutoEmail = reader["AutoEmail"] != DBNull.Value && Convert.ToBoolean(reader["AutoEmail"]),
                                    BeforeDue = reader["BeforeDue"] != DBNull.Value ? Convert.ToInt32(reader["BeforeDue"]) : (int?)null,
                                    AfterDue = reader["AfterDue"] != DBNull.Value ? Convert.ToInt32(reader["AfterDue"]) : (int?)null,
                                       YearLabel = reader["YearLabel"] != DBNull.Value ? reader["YearLabel"].ToString() : ""  // ✅ FIX

                                });
                            }

                            model.ScheduleList = schedules;
                        }
                    }
                }
            }

            return View(model);
        }





        [HttpPost]
        public JsonResult DeleteSchedule(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["EduSuiteADO"].ConnectionString;

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string deleteSchedules = "DELETE FROM Installment_Schedule WHERE ScheduleId = @id";
                using (var scheduleCmd = new SqlCommand(deleteSchedules, conn))
                {
                    scheduleCmd.Parameters.AddWithValue("@id", id);
                    scheduleCmd.ExecuteNonQuery();
                }
            }

            return Json(new { success = true, message = "Installment Schedule deleted successfully." });
        }

        [HttpPost]
        public ActionResult DeleteInstallment(long id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["EduSuiteADO"].ConnectionString;

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

               
                string deleteSchedules = "DELETE FROM Installment_Schedule WHERE InstallmentId = @id";
                using (var scheduleCmd = new SqlCommand(deleteSchedules, conn))
                {
                    scheduleCmd.Parameters.AddWithValue("@id", id);
                    scheduleCmd.ExecuteNonQuery();
                }

          
                string deleteInstallment = "DELETE FROM Fee_Installment WHERE InstallmentId = @id";
                using (var installmentCmd = new SqlCommand(deleteInstallment, conn))
                {
                    installmentCmd.Parameters.AddWithValue("@id", id);
                    installmentCmd.ExecuteNonQuery();
                }
            }

            TempData["Success"] = "Installment deleted successfully.";
            return RedirectToAction("FeeInstallmentList");
        }


        [HttpGet]
        public JsonResult GetTotalCourseFee(int academicTermKey, long courseKey, long universityKey)
        {
            var result = applicationPersonalService.GetCourseFeeAndDuration(academicTermKey, courseKey, universityKey);

            if (result == null)
            {
                return Json(new { success = false, message = "Fee details not found." }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                success = true,
                totalFee = result.TotalFee,
                duration = result.Duration,
                registrationFee = result.RegistrationFee ?? 0,
                admissionFee = result.AdmissionFee ?? 0,
                firstYearFee = result.FirstYearFee ?? 0,
                secondYearFee = result.SecondYearFee ?? 0,
                thirdYearFee = result.ThirdYearFee ?? 0
            }, JsonRequestBehavior.AllowGet);
        }




        [HttpPost]
        public ActionResult AddInstallmentEntry(InsatallmentViewModel model, string DurationJson)
        {
            if (ModelState.IsValid)
            {
                using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["EduSuiteADO"].ConnectionString))
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        string mainQuery = (model.InstallmentId.HasValue)
                            ? @"UPDATE Fee_Installment SET 
                        BatchKey = @BatchKey,
                        AcademicTermKey = @AcademicTermKey,
                        CourseTypeKey = @CourseTypeKey,
                        CourseKey = @CourseKey,
                        UniversityKey = @UniversityKey,
                        FeeAmount = @FeeAmount,
                        InitialPayment = @InitialPayment,
                        BalancePayment = @BalancePayment,
                        FeeTypeKey = @FeeTypeKey
                    WHERE InstallmentId = @InstallmentId"
                            : @"INSERT INTO Fee_Installment 
                        (BatchKey, AcademicTermKey, CourseTypeKey, CourseKey, UniversityKey, FeeAmount,   CreatedOn,FeeTypeKey,InitialPayment,BalancePayment)
                    VALUES (@BatchKey, @AcademicTermKey, @CourseTypeKey, @CourseKey, @UniversityKey, @FeeAmount,  GETDATE(),@FeeTypeKey,@InitialPayment,@BalancePayment);
                    SELECT SCOPE_IDENTITY();";

                        SqlCommand cmd = new SqlCommand(mainQuery, conn, transaction);
                        cmd.Parameters.AddWithValue("@BatchKey", model.SelectedBatchKey);
                        cmd.Parameters.AddWithValue("@AcademicTermKey", model.SelectedAcademicTermKey);
                        cmd.Parameters.AddWithValue("@CourseTypeKey", model.SelectedCourseTypeKey);
                        cmd.Parameters.AddWithValue("@CourseKey", model.SelectedCourseKey);
                        cmd.Parameters.AddWithValue("@UniversityKey", model.SelectedUniversityKey);
                        cmd.Parameters.AddWithValue("@FeeAmount", model.FeeAmount);
                        cmd.Parameters.AddWithValue("@FeeTypeKey", model.SelectedFeeTypeKey);
                        cmd.Parameters.AddWithValue("@InitialPayment", model.InitialPayment);
                        cmd.Parameters.AddWithValue("@BalancePayment", model.BalancePayment);

                        if (model.InstallmentId.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@InstallmentId", model.InstallmentId.Value);
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            model.InstallmentId = Convert.ToInt64(cmd.ExecuteScalar());
                        }

                        var feeDurations = JsonConvert.DeserializeObject<List<FeeDuration>>(DurationJson);

                        foreach (var duration in feeDurations)
                        {
                            SqlCommand durationCmd = new SqlCommand(@"
                        INSERT INTO Fee_Installment_Duration
                        (InstallmentId, YearLabel, RegistrationFee, AdmissionFee, TuitionFee, FeeTypeKey,IntialPayment,BalancePayment)
                        VALUES (@InstallmentId, @YearLabel, @RegistrationFee, @AdmissionFee, @TuitionFee,@FeeTypeKey,@IntialPayment,@BalancePayment)", conn, transaction);

                            durationCmd.Parameters.AddWithValue("@InstallmentId", model.InstallmentId.Value);
                            durationCmd.Parameters.AddWithValue("@YearLabel", duration.YearLabel);
                            durationCmd.Parameters.AddWithValue("@RegistrationFee", duration.RegistrationFee ?? (object)DBNull.Value);
                            durationCmd.Parameters.AddWithValue("@AdmissionFee", duration.AdmissionFee ?? (object)DBNull.Value);
                            durationCmd.Parameters.AddWithValue("@TuitionFee", duration.TuitionFee);
                            durationCmd.Parameters.AddWithValue("@FeeTypeKey", duration.SelectedFeeTypeKey);
                            durationCmd.Parameters.AddWithValue("@IntialPayment", duration.IntialPayment);
                            durationCmd.Parameters.AddWithValue("@BalancePayment", duration.BalancePayment);
                            durationCmd.ExecuteNonQuery();
                        }

                        transaction.Commit();

                        return Json(new
                        {
                            success = true,
                            message = "Saved successfully!",
                            installmentId = model.InstallmentId
                        });
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return Json(new { success = false, message = "Error: " + ex.Message });
                    }
                }
            }

            return Json(new { success = false, message = "Model validation failed." });
        }


        [HttpPost]
        public ActionResult AddFeeSchedule(InsatallmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Model validation failed." });
            }

            string connectionString = ConfigurationManager.ConnectionStrings["EduSuiteADO"].ConnectionString;
            string scheduleJson = Request["ScheduleListJson"];
            List<InstallmentScheduleViewModel> scheduleList = JsonConvert.DeserializeObject<List<InstallmentScheduleViewModel>>(scheduleJson);

            long installmentId = model.InstallmentId ?? GetLatestInstallmentId();

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            foreach (var item in scheduleList)
                            {
                                if (item.ScheduleId.HasValue && item.ScheduleId.Value > 0)
                                    continue;

                                bool isEmptyRow =
                                    string.IsNullOrWhiteSpace(item.InstallmentMonth) &&
                                    item.PaymentDay == null &&
                                    item.DueDuration == null &&
                                    item.Amount == null &&
                                    item.DueFineAmount == null &&
                                    item.SuperFineAmount == null &&
                                    item.BeforeDue == null &&
                                    item.AfterDue == null &&
                                    item.YearLabel == null;

                                if (isEmptyRow)
                                    continue;

                                string insertQuery = @"INSERT INTO Installment_Schedule 
                            (InstallmentId, InstallmentMonth, PaymentDay, DueDuration, Amount, DueFineAmount, SuperFineAmount, AutoSMS, AutoEmail, BeforeDue, AfterDue, YearLabel)
                            VALUES 
                            (@InstallmentId, @InstallmentMonth, @PaymentDay, @DueDuration, @Amount, @DueFineAmount, @SuperFineAmount, @AutoSMS, @AutoEmail, @BeforeDue, @AfterDue, @YearLabel)";

                                using (var cmd = new SqlCommand(insertQuery, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@InstallmentId", installmentId);
                                    cmd.Parameters.AddWithValue("@InstallmentMonth", item.InstallmentMonth ?? "");
                                    cmd.Parameters.AddWithValue("@PaymentDay", item.PaymentDay ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@DueDuration", item.DueDuration ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@Amount", item.Amount ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@DueFineAmount", item.DueFineAmount ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@SuperFineAmount", item.SuperFineAmount ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@AutoSMS", item.AutoSMS);
                                    cmd.Parameters.AddWithValue("@AutoEmail", item.AutoEmail);
                                    cmd.Parameters.AddWithValue("@BeforeDue", item.BeforeDue ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@AfterDue", item.AfterDue ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@YearLabel", item.YearLabel ?? (object)DBNull.Value);

                                    cmd.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Json(new { success = false, message = "Error: " + ex.Message });
                        }
                    }
                }

                return Json(new
                {
                    success = true,
                    message = "Installment saved successfully!",
                    installmentId = installmentId
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }


        public class InstallmentListItem
        {
            public long InstallmentId { get; set; }
            public string UniversityMasterName { get; set; }
            
            public string AcademicTermName { get; set; }
            public string CourseName { get; set; }
            public decimal FeeAmount { get; set; }
            public decimal InitialPayment { get; set; }
            public decimal BalancePayment { get; set; }
            
        }

     

    }
}