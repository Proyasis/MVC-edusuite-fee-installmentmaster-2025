using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace CITS.EduSuite.UI
{
    public class BundleConfig
    {

        public static void RegisterBundles(BundleCollection bundles)
        {

            #region PluginStyles
            bundles.Add(new StyleBundle("~/Content/BootstrapCSS").Include(
                    "~/Content/bootstrap-grid.css",
                    "~/Content/bootstrap.min.css"
                    //"~/Content/bootstrap-responsive.css"
                    ));

            bundles.Add(new StyleBundle("~/Content/MultiselectCSS").Include("~/Content/Multiselect/bootstrap-multiselect.css"));

            bundles.Add(new StyleBundle("~/Content/LoadMaskCSS").Include("~/Content/jquery.mloading.css"));

            bundles.Add(new StyleBundle("~/Content/ToastrCSS").Include("~/Content/toastr.min.css", "~/Content/mvc.toastrnotification.css"));

            bundles.Add(new StyleBundle("~/Content/JCropCSS").Include("~/Content/jquery.Jcrop.min.css"));

            bundles.Add(new StyleBundle("~/Content/DatePickerCSS").Include("~/Content/bootstrap-datepicker3.min.css"));

            bundles.Add(new StyleBundle("~/Content/TimePickerCSS").Include("~/Content/Timepicker/bootstrap-timepicker.css"));

            bundles.Add(new StyleBundle("~/Content/Select2CSS").Include("~/Content/css/select2.min.css"));

            bundles.Add(new StyleBundle("~/Content/BootstrapSelectCSS").Include("~/Content/bootstrap-select.css"));

            bundles.Add(new StyleBundle("~/Content/treestyle/css").Include("~/Content/style.css"));

            bundles.Add(new StyleBundle("~/Content/JqGridCSS").Include("~/Content/jquery.jqGrid/ui.jqgrid.css"));

            bundles.Add(new StyleBundle("~/Content/DatatableCSS").Include(
          "~/Content/DataTables/css/jquery.dataTables.min.css",
           "~/Content/DataTables/css/dataTables.bootstrap4.min.css",
           "~/Content/DataTables/css/dataTables.responsive.min.css",
           "~/Content/DataTables/css/responsive.bootstrap4.min.css"));


            //       bundles.Add(new StyleBundle("~/Content/DatatableCSS").Include(
            //           "~/Content/DataTables/css/jquery.dataTables.min.css",
            //"~/Content/DataTables/css/dataTables.bootstrap4.min.css",
            // "~/Content/DataTables/css/buttons.bootstrap4.min.css",
            //  "~/Content/DataTables/css/select.bootstrap4.min.css"
            //));

            bundles.Add(new StyleBundle("~/Content/ChartCSS").Include("~/Content/Chart.min.css"));

            bundles.Add(new StyleBundle("~/Content/FullCalendarCSS").Include("~/Content/fullcalendar.min.css"));


            #endregion

            #region PluginScripts

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include("~/Scripts/jquery-ui-1.12.1.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include("~/Scripts/jquery.validate.js", "~/Scripts/jquery.validate.unobtrusive.js"));

            bundles.Add(new ScriptBundle("~/bundles/CitsValidation").Include("~/Content/CitsValidation/citsvalidations.min.js", "~/Content/CitsValidation/citsvalidations.unobtrusive.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrapJS").Include("~/Scripts/umd/popper.min.js", "~/Scripts/bootstrap.min.js", "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/Content/MultiselectJs").Include("~/Content/Multiselect/bootstrap-multiselect.js"));

            bundles.Add(new ScriptBundle("~/Content/ToastrJs").Include("~/Scripts/toastr.min.js", "~/Scripts/mvc.toastrnotification.js"));

            bundles.Add(new ScriptBundle("~/Content/JCropJS").Include("~/Scripts/jquery.Jcrop.min.js"));

            bundles.Add(new ScriptBundle("~/Content/DatePickerJS").Include("~/Scripts/bootstrap-datepicker.js"));

            bundles.Add(new ScriptBundle("~/Content/TimePickerJS").Include("~/Content/Timepicker/bootstrap-timepicker.js"));

            bundles.Add(new ScriptBundle("~/Content/Select2JS").Include("~/Scripts/select2.min.js"));

            bundles.Add(new ScriptBundle("~/Content/BootstrapSelectJS").Include("~/Scripts/bootstrap-select.min.js"));

            bundles.Add(new ScriptBundle("~/Content/JqueryRepeater").Include("~/Scripts/jquery.repeater.js"));

            bundles.Add(new ScriptBundle("~/Content/MomentJS").Include("~/Scripts/moment.min.js", "~/Scripts/moment-with-locales.min.js", "~/Scripts/moment-timezone/moment-timezone.min.js"));


            bundles.Add(new ScriptBundle("~/Content/ExcelSheetJS").Include(
            "~/Content/ExcelSheet/excel-formula.min.js",
            "~/Content/ExcelSheet/xlsx.full.min.js",
            "~/Content/ExcelSheet/jquery.jexcel.js",
            "~/Content/ExcelSheet/FileSaver.js",
            "~/Content/ExcelSheet/jquery.jcalendar.js"));


            bundles.Add(new StyleBundle("~/Content/ExcelSheetCSS").Include("~/Content/ExcelSheet/jquery.jexcel.css", "~/Content/ExcelSheet/jquery.jcalendar.min.css"));

            bundles.Add(new ScriptBundle("~/Scripts/QRCode").Include("~/Scripts/QRCode/QRCodeJS.js"));

            bundles.Add(new ScriptBundle("~/Content/LoadMaskJS").Include("~/Scripts/jquery.mloading.js"));

            bundles.Add(new ScriptBundle("~/Scripts/PrintArea").Include("~/Scripts/jquery.PrintArea.js"));

            bundles.Add(new ScriptBundle("~/Scripts/mustache").Include("~/Scripts/mustache.js"));

            bundles.Add(new ScriptBundle("~/Content/JqGridJS").Include(
             "~/Scripts/i18n/grid.locale-en.js",
            "~/Scripts/jquery.jqGrid.src.js",
            "~/Scripts/jquery.jqgrid.showhidecolumnmenu.js",
             "~/Scripts/jquery.tablednd.js"));



            bundles.Add(new ScriptBundle("~/Content/ExcelExportJS").Include("~/Content/ExcelExport/excelexportjs.js"));

            bundles.Add(new ScriptBundle("~/Content/InputMaskJS").Include(
                "~/Scripts/jquery.inputmask/inputmask.js",
                 "~/Scripts/jquery.inputmask/jquery.inputmask.js",
                  "~/Scripts/jquery.inputmask/inputmask.extensions.js",
                  "~/Scripts/jquery.inputmask/inputmask.date.extensions.js"));

            bundles.Add(new ScriptBundle("~/Content/DatatableJS").Include(
              "~/Scripts/DataTables/jquery.dataTables.min.js",
               "~/Scripts/DataTables/dataTables.responsive.min.js",
              "~/Scripts/DataTables/responsive.bootstrap4.min.js",
               "~/Scripts/DataTables/dataTables.bootstrap4.min.js"));

            bundles.Add(new ScriptBundle("~/Content/JSTreeJS").Include(
              "~/Scripts/jstree.min.js"));

            //           bundles.Add(new ScriptBundle("~/Content/DatatableJS").Include(
            //               "~/Scripts/DataTables/jquery.dataTables.min.js",
            //"~/Scripts/DataTables/dataTables.bootstrap4.min.js",
            // "~/Scripts/DataTables/buttons.bootstrap4.min.js",
            //  "~/Scripts/DataTables/select.bootstrap4.min.js"
            //));

            bundles.Add(new ScriptBundle("~/Scripts/handlebars").Include("~/Scripts/handlebars.min.js", "~/Scripts/handlebars.js"));

            bundles.Add(new ScriptBundle("~/Scripts/BarCode").Include("~/Scripts/jquery-barcode-last.min"));

            bundles.Add(new ScriptBundle("~/Scripts/QRCode").Include("~/Scripts/QRCodeJS.js"));

            bundles.Add(new ScriptBundle("~/Content/BootstrapTableJS").Include("~/Scripts/bootstrap-table.js"));

            bundles.Add(new ScriptBundle("~/Scripts/ChartJS").Include("~/Scripts/Chart.min.js"));

            //bundles.Add(new ScriptBundle("~/Content/tinyMCEJS").Include(
            //     "~/Scripts/tinymce/jquery.tinymce.js",
            //     "~/Scripts/tinymce/tiny_mce.js",
            //     "~/Scripts/tinymce/tiny_mce_popup.js",
            //     "~/Scripts/tinymce/tiny_mce_src.js"
            //     ));

            bundles.Add(new ScriptBundle("~/Content/tinyMCEJS").Include(
                "~/Scripts/tinymce/jquery.tinymce.min.js",
                "~/Scripts/tinymce/tinymce.js",
                  "~/Scripts/tinymce/tinymce.min.js"

            ));


            bundles.Add(new ScriptBundle("~/Content/FullCalendarJS").Include("~/Scripts/fullcalendar.min.js"));

            #endregion

            #region AppStyles



            bundles.Add(new StyleBundle("~/Styles/CSS").Include("~/Styles/main.css"));

            bundles.Add(new StyleBundle("~/Styles/LoginCSS").Include("~/Styles/pages/login.min.css"));

            bundles.Add(new StyleBundle("~/Styles/StyleCSS").Include("~/Styles/style.css"));

            bundles.Add(new StyleBundle("~/Styles/DashboardCSS").Include("~/Styles/Dashboard.css"));
            #endregion

            #region AppScripts

            bundles.Add(new ScriptBundle("~/Scripts/Common").Include(
                "~/CITSEduSuiteScripts/Common/AppCommon.js",
                "~/CITSEduSuiteScripts/Common/ModelPopup.js",
                "~/CITSEduSuiteScripts/Common/Custom.js",
                "~/Scripts/AjaxHelper.js",
                "~/Scripts/jquery-confirm.min.js",
                "~/Scripts/Common/ControlStyle.js",
                "~/Scripts/enscroll-0.6.2.min.js",
                "~/CITSEduSuiteScripts/Common/Num2words.js"
            ));
            bundles.Add(new ScriptBundle("~/Scripts/CourseType").Include("~/CITSEduSuiteScripts/CourseType.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Branch").Include("~/CITSEduSuiteScripts/Branch.js"));

            bundles.Add(new ScriptBundle("~/Scripts/FeeType").Include("~/CITSEduSuiteScripts/FeeType.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Book").Include("~/CITSEduSuiteScripts/Book.js"));

            bundles.Add(new ScriptBundle("~/Scripts/UniversityCourse").Include("~/CITSEduSuiteScripts/UniversityCourse.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Attendance").Include("~/CITSEduSuiteScripts/Attendance.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Application").Include("~/CITSEduSuiteScripts/Application.js"));

            bundles.Add(new ScriptBundle("~/Scripts/ApplicationPersonal").Include("~/CITSEduSuiteScripts/ApplicationPersonal.js"));

            bundles.Add(new ScriptBundle("~/Scripts/ApplicationEducationalDetails").Include("~/CITSEduSuiteScripts/ApplicationEducationalDetails.js"));

            bundles.Add(new ScriptBundle("~/Scripts/ApplicationFeePayment").Include("~/CITSEduSuiteScripts/ApplicationFeePayment.js"));

            bundles.Add(new ScriptBundle("~/Scripts/PhotoUpload").Include("~/CITSEduSuiteScripts/PhotoUpload.js"));

            bundles.Add(new ScriptBundle("~/Scripts/ApplicationDocument").Include("~/CITSEduSuiteScripts/ApplicationDocument.js"));

            bundles.Add(new ScriptBundle("~/Scripts/ApplicationFeeInstallment").Include("~/CITSEduSuiteScripts/ApplicationFeeInstallment.js"));

            bundles.Add(new ScriptBundle("~/Scripts/ApplicationElectivePaper").Include("~/CITSEduSuiteScripts/ApplicationElectivePaper.js"));

            bundles.Add(new ScriptBundle("~/Scripts/FeeDetails").Include("~/CITSEduSuiteScripts/FeeDetails.js"));

            bundles.Add(new ScriptBundle("~/Scripts/EnquiryLead").Include("~/CITSEduSuiteScripts/EnquiryLead.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Enquiry").Include("~/CITSEduSuiteScripts/Enquiry.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Course").Include("~/CITSEduSuiteScripts/Course.js"));

            bundles.Add(new ScriptBundle("~/Scripts/UniversityMaster").Include("~/CITSEduSuiteScripts/UniversityMaster.js"));

            bundles.Add(new ScriptBundle("~/Scripts/ExamCentre").Include("~/CITSEduSuiteScripts/ExamCentre.js"));

            bundles.Add(new ScriptBundle("~/Scripts/ExamTerm").Include("~/CITSEduSuiteScripts/ExamTerm.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Department").Include("~/CITSEduSuiteScripts/Department.js"));

            bundles.Add(new ScriptBundle("~/Scripts/CertificateType").Include("~/CITSEduSuiteScripts/CertificateType.js"));

            bundles.Add(new ScriptBundle("~/Scripts/NatureOfEnquiry").Include("~/CITSEduSuiteScripts/NatureOfEnquiry.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Religion").Include("~/CITSEduSuiteScripts/Religion.js"));

            bundles.Add(new ScriptBundle("~/Scripts/StudyMaterial").Include("~/CITSEduSuiteScripts/StudyMaterial.js"));

            bundles.Add(new ScriptBundle("~/Scripts/EnquiryLeadSchedule").Include("~/CITSEduSuiteScripts/EnquiryLeadSchedule.js"));

            bundles.Add(new ScriptBundle("~/Scripts/UniversityPayment").Include("~/CITSEduSuiteScripts/UniversityPayment.js"));

            bundles.Add(new ScriptBundle("~/Scripts/DivisionAllocation").Include("~/CITSEduSuiteScripts/DivisionAllocation.js"));

            bundles.Add(new ScriptBundle("~/Scripts/StudentIDCard").Include("~/CITSEduSuiteScripts/StudentIDCard.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Roles").Include("~/CITSEduSuiteScripts/Roles.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Employee").Include("~/CITSEduSuiteScripts/Employee.js"));

            bundles.Add(new ScriptBundle("~/Scripts/EmployeePersonal").Include("~/CITSEduSuiteScripts/EmployeePersonal.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Employee").Include("~/CITSEduSuiteScripts/Employee.js"));

            bundles.Add(new ScriptBundle("~/Scripts/EmployeeContact").Include("~/CITSEduSuiteScripts/EmployeeContact.js"));

            bundles.Add(new ScriptBundle("~/Scripts/EmployeeExperience").Include("~/CITSEduSuiteScripts/EmployeeExperience.js"));

            bundles.Add(new ScriptBundle("~/Scripts/EmployeeIdentity").Include("~/CITSEduSuiteScripts/EmployeeIdentity.js"));

            bundles.Add(new ScriptBundle("~/Scripts/EmployeeAccount").Include("~/CITSEduSuiteScripts/EmployeeAccount.js"));

            bundles.Add(new ScriptBundle("~/Scripts/EmployeeEducation").Include("~/CITSEduSuiteScripts/EmployeeEducation.js"));

            bundles.Add(new ScriptBundle("~/Scripts/EmployeeLanguageSkill").Include("~/CITSEduSuiteScripts/EmployeeLanguageSkill.js"));

            bundles.Add(new ScriptBundle("~/Scripts/EmployeeLoan").Include("~/CITSEduSuiteScripts/EmployeeLoan.js"));

            bundles.Add(new ScriptBundle("~/Scripts/EmployeeSalarySettings").Include("~/CITSEduSuiteScripts/EmployeeSalarySettings.js"));

            bundles.Add(new ScriptBundle("~/Scripts/EmployeeSalary").Include("~/CITSEduSuiteScripts/EmployeeSalary.js"));

            bundles.Add(new ScriptBundle("~/Scripts/EmployeeSalaryAdvance").Include("~/CITSEduSuiteScripts/EmployeeSalaryAdvance.js"));

            bundles.Add(new ScriptBundle("~/Scripts/EmployeeAttendance").Include("~/CITSEduSuiteScripts/EmployeeAttendance.js"));

            bundles.Add(new ScriptBundle("~/Scripts/EmployeeUserPermission").Include("~/CITSEduSuiteScripts/EmployeeUserPermission.js"));

            bundles.Add(new ScriptBundle("~/Scripts/LeaveApplication").Include("~/CITSEduSuiteScripts/LeaveApplication.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Shift").Include("~/CITSEduSuiteScripts/Shift.js"));

            bundles.Add(new ScriptBundle("~/Scripts/SalaryComponent").Include("~/CITSEduSuiteScripts/SalaryComponent.js"));

            bundles.Add(new ScriptBundle("~/Scripts/EmployeeTask").Include("~/CITSEduSuiteScripts/EmployeeTask.js"));

            bundles.Add(new ScriptBundle("~/Scripts/District").Include("~/CITSEduSuiteScripts/District.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Country").Include("~/CITSEduSuiteScripts/Country.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Branch").Include("~/CITSEduSuiteScripts/Branch.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Religion").Include("~/CITSEduSuiteScripts/Religion.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Department").Include("~/CITSEduSuiteScripts/Department.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Province").Include("~/CITSEduSuiteScripts/Province.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Batch").Include("~/CITSEduSuiteScripts/Batch.js"));

            bundles.Add(new ScriptBundle("~/Scripts/TransactionType").Include("~/CITSEduSuiteScripts/TransactionType.js"));

            bundles.Add(new ScriptBundle("~/Scripts/StudentsSummary").Include("~/CITSEduSuiteScripts/StudentsSummary.js"));

            bundles.Add(new ScriptBundle("~/Scripts/ActivityLog").Include("~/CITSEduSuiteScripts/ActivityLog.js"));

            bundles.Add(new ScriptBundle("~/Scripts/InternalExamResult").Include("~/CITSEduSuiteScripts/InternalExamResult.js"));

            bundles.Add(new ScriptBundle("~/Scripts/BuildingDetails").Include("~/CITSEduSuiteScripts/BuildingDetails.js"));

            bundles.Add(new ScriptBundle("~/Scripts/BaseStudentsSearch").Include("~/CITSEduSuiteScripts/BaseStudentsSearch.js"));

            bundles.Add(new ScriptBundle("~/Scripts/InternalExamTerm").Include("~/CITSEduSuiteScripts/InternalExamTerm.js"));

            bundles.Add(new ScriptBundle("~/Scripts/AttendanceType").Include("~/CITSEduSuiteScripts/AttendanceType.js"));

            bundles.Add(new ScriptBundle("~/Scripts/SecondLanguage").Include("~/CITSEduSuiteScripts/SecondLanguage.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Division").Include("~/CITSEduSuiteScripts/Division.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Medium").Include("~/CITSEduSuiteScripts/Medium.js"));

            bundles.Add(new ScriptBundle("~/Scripts/StudentStatus").Include("~/CITSEduSuiteScripts/StudentStatus.js"));

            bundles.Add(new ScriptBundle("~/Scripts/ClassMode").Include("~/CITSEduSuiteScripts/ClassMode.js"));

            bundles.Add(new ScriptBundle("~/Scripts/BookIssueSummaryReport").Include("~/CITSEduSuiteScripts/BookIssueSummaryReport.js"));

            bundles.Add(new ScriptBundle("~/Scripts/ApplicationFamilyDetails").Include("~/CITSEduSuiteScripts/ApplicationFamilyDetails.js"));

            bundles.Add(new ScriptBundle("~/Scripts/BankAccount").Include("~/CITSEduSuiteScripts/BankAccount.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Journal").Include("~/CITSEduSuiteScripts/Journal.js"));

            bundles.Add(new ScriptBundle("~/Scripts/AccountHead").Include("~/CITSEduSuiteScripts/AccountHead.js"));

            bundles.Add(new ScriptBundle("~/Scripts/PushNotification").Include("~/CITSEduSuiteScripts/PushNotification.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Designation").Include("~/CITSEduSuiteScripts/Designation.js"));

            bundles.Add(new ScriptBundle("~/Scripts/DesignationGrade").Include("~/CITSEduSuiteScripts/DesignationGrade.js"));

            bundles.Add(new ScriptBundle("~/Scripts/PaymentWindow").Include("~/CITSEduSuiteScripts/PaymentWindow.js"));

            bundles.Add(new ScriptBundle("~/Scripts/EmployeeClassAllocation").Include("~/CITSEduSuiteScripts/EmployeeClassAllocation.js"));

            bundles.Add(new ScriptBundle("~/Scripts/StudentsPromotion").Include("~/CITSEduSuiteScripts/StudentsPromotion.js"));

            bundles.Add(new ScriptBundle("~/Scripts/StudentsCertificateReturn").Include("~/CITSEduSuiteScripts/StudentsCertificateReturn.js"));

            bundles.Add(new ScriptBundle("~/Scripts/UniversityCertificate").Include("~/CITSEduSuiteScripts/UniversityCertificate.js"));

            bundles.Add(new ScriptBundle("~/Scripts/InternalExamSchedule").Include("~/CITSEduSuiteScripts/InternalExamSchedule.js"));

            bundles.Add(new ScriptBundle("~/Scripts/ExamSchedule").Include("~/CITSEduSuiteScripts/ExamSchedule.js"));

            bundles.Add(new ScriptBundle("~/Scripts/UniversityPaymentBulk").Include("~/CITSEduSuiteScripts/UniversityPaymentBulk.js"));
            //BookSuite

            bundles.Add(new ScriptBundle("~/Scripts/Publisher").Include("~/CITSEduSuiteScripts/Publisher.js"));

            bundles.Add(new ScriptBundle("~/Scripts/MemberRegistration").Include("~/CITSEduSuiteScripts/MemberRegistration.js"));

            bundles.Add(new ScriptBundle("~/Scripts/BookIssueReturn").Include("~/CITSEduSuiteScripts/BookIssueReturn.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Language").Include("~/CITSEduSuiteScripts/Language.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Rack").Include("~/CITSEduSuiteScripts/Rack.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Designations").Include("~/CITSEduSuiteScripts/Designations.js"));

            bundles.Add(new ScriptBundle("~/Scripts/BookCategory").Include("~/CITSEduSuiteScripts/BookCategory.js"));

            bundles.Add(new ScriptBundle("~/Scripts/BookStatus").Include("~/CITSEduSuiteScripts/BookStatus.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Author").Include("~/CITSEduSuiteScripts/Author.js"));

            bundles.Add(new ScriptBundle("~/Scripts/BookIssueType").Include("~/CITSEduSuiteScripts/BookIssueType.js"));

            bundles.Add(new ScriptBundle("~/Scripts/BorrowerType").Include("~/CITSEduSuiteScripts/BorrowerType.js"));

            bundles.Add(new ScriptBundle("~/Scripts/MemberType").Include("~/CITSEduSuiteScripts/MemberType.js"));

            bundles.Add(new ScriptBundle("~/Scripts/LibraryBook").Include("~/CITSEduSuiteScripts/LibraryBook.js"));

            bundles.Add(new ScriptBundle("~/Scripts/BookCopy").Include("~/CITSEduSuiteScripts/BookCopy.js"));

            bundles.Add(new ScriptBundle("~/Scripts/MemberPlanDetails").Include("~/CITSEduSuiteScripts/MemberPlanDetails.js"));

            bundles.Add(new ScriptBundle("~/Scripts/BankStatement").Include("~/CITSEduSuiteScripts/BankStatement.js"));

            bundles.Add(new ScriptBundle("~/Scripts/BankReconciliation").Include("~/CITSEduSuiteScripts/BankReconciliation.js"));

            bundles.Add(new ScriptBundle("~/Scripts/FutureTransaction").Include("~/CITSEduSuiteScripts/FutureTransaction.js"));

            bundles.Add(new ScriptBundle("~/Scripts/FutureTransactionPayment").Include("~/CITSEduSuiteScripts/FutureTransactionPayment.js"));

            bundles.Add(new ScriptBundle("~/Scripts/ChequeClearance").Include("~/CITSEduSuiteScripts/ChequeClearance.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Shared").Include("~/CITSEduSuiteScripts/Shared.js"));

            bundles.Add(new ScriptBundle("~/Scripts/CourseSubject").Include("~/CITSEduSuiteScripts/CourseSubject.js"));

            bundles.Add(new ScriptBundle("~/Scripts/ExamResult").Include("~/CITSEduSuiteScripts/ExamResult.js"));

            bundles.Add(new ScriptBundle("~/Scripts/EnquiryCallStatus").Include("~/CITSEduSuiteScripts/EnquiryCallStatus.js"));

            bundles.Add(new ScriptBundle("~/Scripts/SyllabusAndStudyMaterial").Include("~/CITSEduSuiteScripts/SyllabusAndStudyMaterial.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Scholarship").Include("~/CITSEduSuiteScripts/Scholarship.js"));

            bundles.Add(new ScriptBundle("~/Scripts/ScholarshipExamSchedule").Include("~/CITSEduSuiteScripts/ScholarshipExamSchedule.js"));

            bundles.Add(new ScriptBundle("~/Scripts/ScholarshipExamResult").Include("~/CITSEduSuiteScripts/ScholarshipExamResult.js"));

            bundles.Add(new ScriptBundle("~/Scripts/UnitTestSchedule").Include("~/CITSEduSuiteScripts/UnitTestSchedule.js"));

            bundles.Add(new ScriptBundle("~/Scripts/CashFlow").Include("~/CITSEduSuiteScripts/CashFlow.js"));

            bundles.Add(new ScriptBundle("~/Scripts/PrintInvoice").Include("~/CITSEduSuiteScripts/PrintInvoice.js"));

            bundles.Add(new ScriptBundle("~/Scripts/AccountHeadOpeningBalance").Include("~/CITSEduSuiteScripts/AccountHeadOpeningBalance.js"));

            bundles.Add(new ScriptBundle("~/Scripts/AttendanceReport").Include("~/CITSEduSuiteScripts/AttendanceReport.js"));

            bundles.Add(new ScriptBundle("~/Scripts/EnquiryCallReport").Include("~/CITSEduSuiteScripts/EnquiryCallReport.js"));

            bundles.Add(new ScriptBundle("~/Scripts/WorkSchedule").Include("~/CITSEduSuiteScripts/WorkSchedule.js"));

            bundles.Add(new ScriptBundle("~/Scripts/BulkEmailSMS").Include("~/CITSEduSuiteScripts/BulkEmailSMS.js"));

            bundles.Add(new ScriptBundle("~/Scripts/NotificationTemplate").Include("~/CITSEduSuiteScripts/NotificationTemplate.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Holiday").Include("~/CITSEduSuiteScripts/Holiday.js"));

            bundles.Add(new ScriptBundle("~/Scripts/StudentLate").Include("~/CITSEduSuiteScripts/StudentLate.js"));

            bundles.Add(new ScriptBundle("~/Scripts/StudentAbsconders").Include("~/CITSEduSuiteScripts/StudentAbsconders.js"));

            bundles.Add(new ScriptBundle("~/Scripts/StudentEarlyDeparture").Include("~/CITSEduSuiteScripts/StudentEarlyDeparture.js"));

            bundles.Add(new ScriptBundle("~/Scripts/StudentDiary").Include("~/CITSEduSuiteScripts/StudentDiary.js"));

            bundles.Add(new ScriptBundle("~/Scripts/StudentLeave").Include("~/CITSEduSuiteScripts/StudentLeave.js"));

            bundles.Add(new ScriptBundle("~/Scripts/TeacherPortal").Include("~/CITSEduSuiteScripts/TeacherPortal.js"));

            bundles.Add(new ScriptBundle("~/Scripts/EmployeeSubjectModule").Include("~/CITSEduSuiteScripts/EmployeeSubjectModule.js"));

            bundles.Add(new ScriptBundle("~/Scripts/TimeTable").Include("~/CITSEduSuiteScripts/TimeTable.js"));

            bundles.Add(new ScriptBundle("~/Scripts/AttendanceTypeMaster").Include("~/CITSEduSuiteScripts/AttendanceTypeMaster.js"));

            bundles.Add(new ScriptBundle("~/Scripts/TimeTableTemp").Include("~/CITSEduSuiteScripts/TimeTableTemp.js"));

            bundles.Add(new ScriptBundle("~/Scripts/EmployeeAttendanceReport").Include("~/CITSEduSuiteScripts/EmployeeAttendanceReport.js"));

            bundles.Add(new ScriptBundle("~/Scripts/StudentPortal").Include("~/CITSEduSuiteScripts/StudentPortal.js"));

            bundles.Add(new ScriptBundle("~/Scripts/NotificationData").Include("~/CITSEduSuiteScripts/NotificationData.js"));

            bundles.Add(new ScriptBundle("~/Scripts/LibraryReport").Include("~/CITSEduSuiteScripts/LibraryReport.js"));

            //exam
            bundles.Add(new ScriptBundle("~/Scripts/TestPaper").Include("~/CITSEduSuiteScripts/TestPaper.js"));

            bundles.Add(new ScriptBundle("~/Scripts/ExamTest").Include("~/CITSEduSuiteScripts/ExamTest.js"));

            bundles.Add(new ScriptBundle("~/Scripts/OnlineExamResult").Include("~/CITSEduSuiteScripts/OnlineExamResult.js"));

            bundles.Add(new ScriptBundle("~/Scripts/MarkGroup").Include("~/CITSEduSuiteScripts/MarkGroup.js"));

            bundles.Add(new ScriptBundle("~/Scripts/AccountFlow").Include("~/CITSEduSuiteScripts/AccountFlow.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Import").Include("~/CITSEduSuiteScripts/Import.js"));

            bundles.Add(new ScriptBundle("~/Scripts/StudentStudyMaterial").Include("~/CITSEduSuiteScripts/StudentStudyMaterial.js"));

            bundles.Add(new ScriptBundle("~/Scripts/StudyMaterialUpload").Include("~/CITSEduSuiteScripts/StudyMaterialUpload.js"));

            bundles.Add(new ScriptBundle("~/Scripts/VideoUpload").Include("~/CITSEduSuiteScripts/VideoUpload.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Videos").Include("~/CITSEduSuiteScripts/Videos.js"));

            bundles.Add(new ScriptBundle("~/Scripts/DashBoard").Include("~/CITSEduSuiteScripts/DashBoard.js"));

            bundles.Add(new ScriptBundle("~/Scripts/AttendanceCategory").Include("~/CITSEduSuiteScripts/AttendanceCategory.js"));

            bundles.Add(new ScriptBundle("~/Scripts/AttendanceConfiguration").Include("~/CITSEduSuiteScripts/AttendanceConfiguration.js"));

            bundles.Add(new ScriptBundle("~/Scripts/LeaveApplication").Include("~/CITSEduSuiteScripts/LeaveApplication.js"));

            bundles.Add(new ScriptBundle("~/Scripts/DepartmentShift").Include("~/CITSEduSuiteScripts/DepartmentShift.js"));

            bundles.Add(new ScriptBundle("~/Scripts/EmployeeShift").Include("~/CITSEduSuiteScripts/EmployeeShift.js"));

            bundles.Add(new ScriptBundle("~/Scripts/EmployeeAdvanceReturn").Include("~/CITSEduSuiteScripts/EmployeeAdvanceReturn.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Asset").Include("~/CITSEduSuiteScripts/Asset.js"));

            bundles.Add(new ScriptBundle("~/Scripts/AssetPurchase").Include("~/CITSEduSuiteScripts/AssetPurchase.js"));

            bundles.Add(new ScriptBundle("~/Scripts/AssetPurchasePayment").Include("~/CITSEduSuiteScripts/AssetPurchasePayment.js"));

            bundles.Add(new ScriptBundle("~/Scripts/AssetType").Include("~/CITSEduSuiteScripts/AssetType.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Depreciation").Include("~/CITSEduSuiteScripts/Depreciation.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Party").Include("~/CITSEduSuiteScripts/Party.js"));

            bundles.Add(new ScriptBundle("~/Scripts/LeadManagement").Include("~/CITSEduSuiteScripts/LeadManagement.js"));

            bundles.Add(new ScriptBundle("~/Scripts/StudentTC").Include("~/CITSEduSuiteScripts/StudentTC.js"));

            bundles.Add(new ScriptBundle("~/Scripts/BankTransaction").Include("~/CITSEduSuiteScripts/BankTransaction.js"));

            bundles.Add(new ScriptBundle("~/Scripts/FeeRefund").Include("~/CITSEduSuiteScripts/FeeRefund.js"));

            bundles.Add(new ScriptBundle("~/Scripts/CashTransaction").Include("~/CITSEduSuiteScripts/CashTransaction.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Agent").Include("~/CITSEduSuiteScripts/Agent.js"));

            bundles.Add(new ScriptBundle("~/Scripts/UniversityPaymentCancelation").Include("~/CITSEduSuiteScripts/UniversityPaymentCancelation.js"));

            bundles.Add(new ScriptBundle("~/Scripts/BonafiedCertificate").Include("~/CITSEduSuiteScripts/BonafiedCertificate.js"));

            bundles.Add(new ScriptBundle("~/Scripts/CourseCompletionCertificate").Include("~/CITSEduSuiteScripts/CourseCompletionCertificate.js"));

            bundles.Add(new ScriptBundle("~/Scripts/CourseTransfer").Include("~/CITSEduSuiteScripts/CourseTransfer.js"));

            bundles.Add(new ScriptBundle("~/Scripts/ApplicationWebForm").Include("~/CITSEduSuiteScripts/ApplicationWebForm.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Menu").Include("~/CITSEduSuiteScripts/Menu.js"));

            bundles.Add(new ScriptBundle("~/Scripts/MenuType").Include("~/CITSEduSuiteScripts/MenuType.js"));

            bundles.Add(new ScriptBundle("~/Scripts/CounsellingTime").Include("~/CITSEduSuiteScripts/CounsellingTime.js"));

            bundles.Add(new ScriptBundle("~/Scripts/EmployeeEnquiryTarget").Include("~/CITSEduSuiteScripts/EmployeeEnquiryTarget.js"));

            bundles.Add(new ScriptBundle("~/Scripts/EnquiryLeadCountSummary").Include("~/CITSEduSuiteScripts/EnquiryLead_Enquiry_CountSummary.js"));

            bundles.Add(new ScriptBundle("~/Scripts/EnquiryTargetSummary").Include("~/CITSEduSuiteScripts/EnquiryTargetSummary.js"));

            bundles.Add(new ScriptBundle("~/Scripts/FeePaidorUnPaidSummary").Include("~/CITSEduSuiteScripts/FeePaidorUnPaidSummary.js"));

            bundles.Add(new ScriptBundle("~/Scripts/AcademicTerm").Include("~/CITSEduSuiteScripts/AcademicTerm.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Bank").Include("~/CITSEduSuiteScripts/Bank.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Caste").Include("~/CITSEduSuiteScripts/Caste.js"));

            bundles.Add(new ScriptBundle("~/Scripts/GSTMaster").Include("~/CITSEduSuiteScripts/GSTMaster.js"));

            bundles.Add(new ScriptBundle("~/Scripts/DashBoardType").Include("~/CITSEduSuiteScripts/DashBoardType.js"));

            bundles.Add(new ScriptBundle("~/Scripts/DashBoardContent").Include("~/CITSEduSuiteScripts/DashBoardContent.js"));

            bundles.Add(new ScriptBundle("~/Scripts/SalaryOtherAmountType").Include("~/CITSEduSuiteScripts/SalaryOtherAmountType.js"));

            bundles.Add(new ScriptBundle("~/Scripts/FieldValidation").Include("~/CITSEduSuiteScripts/FieldValidation.js"));

            bundles.Add(new ScriptBundle("~/Scripts/TCReasonMaster").Include("~/CITSEduSuiteScripts/TCReasonMaster.js"));

            bundles.Add(new ScriptBundle("~/Scripts/FeeFollowUp").Include("~/CITSEduSuiteScripts/FeeFollowUp.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Company").Include("~/CITSEduSuiteScripts/Company.js"));

            bundles.Add(new ScriptBundle("~/Scripts/ApplicationSchedule").Include("~/CITSEduSuiteScripts/ApplicationSchedule.js"));

            bundles.Add(new ScriptBundle("~/Scripts/ApplicationScheduleType").Include("~/CITSEduSuiteScripts/ApplicationScheduleType.js"));

            bundles.Add(new ScriptBundle("~/Scripts/ApplicationScheduleCallStatus").Include("~/CITSEduSuiteScripts/ApplicationScheduleCallStatus.js"));

            bundles.Add(new ScriptBundle("~/Scripts/ESSLStudents").Include("~/CITSEduSuiteScripts/ESSLStudents.js"));

            bundles.Add(new ScriptBundle("~/Scripts/UserManual").Include("~/CITSEduSuiteScripts/UserManual.js"));



            #endregion
        }


    }
}