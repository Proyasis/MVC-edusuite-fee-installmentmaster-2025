using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface ISelectListService
    {
        List<SelectListModel> FillBranches();
        List<SelectListModel> FillSalaryType();
        List<SelectListModel> FillClasses(short BranchKey);
        List<SelectListModel> FillTeachersByClass(short BranchKey, long ClassDetailsKey);
        List<ApplicationPersonalViewModel> FillStudentsByClass(short BranchKey, long ClassDetailsKey);
        List<SelectListModel> FillStudentStatuses();
        List<SelectListModel> FillAttendanceTypeByDate(DateTime Date, long ApplicationKey, short AttendanceStatusKey, short AttendanceTypeKey);
        List<ApplicationPersonalViewModel> FillStudentsByClassForParentsMeet(long RowKey, long ClassDetailsKey);
        List<SelectListModel> FillAllTeachers(short BranchKey);
        List<SelectListModel> FillApplicationSubjects(long RowKey);
        List<AutoCompleteModel> AutoCompleteEmployeeCode(string Text);
        List<AutoCompleteModel> AutoCompleteAdmissionNo(string Text);
        List<SelectListModel> FillNatureOfEnquiry();
  

        #region Online exam
        List<SelectListModel> FillQuestionTypes();
        List<SelectListModel> FillQuestionModules();
        List<TestPaperViewModel> FillExamModules(long TestPaperKey, long? ApplicationKey);
        List<SelectListModel> FillAnswerKeyModules(long TestPaperKey);
        List<TestSectionViewModel> FillQuestionSectionById(long TestPaperKey, byte ModuleKey);
        List<SelectListModel> FillSubjects();
        List<MarkGroupViewModel> FillMarkGroups();
        #endregion Online exam
        List<SelectListModel> FillAcademicTerms();
        List<SelectListModel> FillCourseTypesById(short? AcademicTermKey);
        List<SelectListModel> FillCoursesById(short? CourseTypeKey);
        List<SelectListModel> FillUniversitiesById(long? CourseKey);
        List<SelectListModel> FillYearsById(short AcademicTermKey, long CourseKey);
        List<SelectListModel> FillBatches(short? BranchKey);
        List<SelectListModel> FillStudentsById(short? BranchKey, short? BatchKey, short? AcademicTermKey, long? CourseKey, short? UniversityKey, bool ClassRequired);
        List<SelectListModel> FillSubjectsById(short? AcademicTermKey, long? CourseKey, short? UniversityKey, int? StudentYearKey);
        List<SelectListModel> FillStudyMaterialById(short? AcademicTermKey, long? CourseKey, short? UniversityKey, int? StudentYearKey);
        List<SelectListModel> FillCertificates();
        List<SelectListModel> FillExamTerms();
        List<SelectListModel> FillExamCenters();
        List<SelectListModel> FillInternalExamTerms();
        List<SelectListModel> FillEmployeesById(short BranchKey);
        List<SelectListModel> FillTelephoneCodes();
        List<SelectListModel> FillPaymentModeSub(short? PaymentModeKey);
        List<SelectListModel> FillAppUserById(short BranchKey);
        List<SelectListModel> FillDashBoardTypes();
        List<SelectListModel> ApplicationCourseYear(long ApplicationKey);
        List<SelectListModel> FillActions();
        List<SelectListModel> FillMenuCatagory();
        List<SelectListModel> FillMenuType();
        List<SelectListModel> FillMenu(short? MenuTypeKey);
        List<SelectListModel> FillSearchBatch(short? BranchKey);
        List<SelectListModel> FillSearchCourse(short? BranchKey);
        List<SelectListModel> FillSearchUniversity(short? BranchKey);
        List<SelectListModel> FillSearchBankAccounts(short? BranchKey);
        List<SelectListModel> FillDesignation();
        List<SelectListModel> FillDepartment();
        List<SelectListModel> FillEmployeeStatus();
        List<SelectListModel> FillYears();
        List<SelectListModel> FillMonths();
        List<SelectListModel> FillEmployeesByBranchKeys(List<long> BranchKeys);
        List<SelectListModel> FillEmployeeAttendanceStatus(short? BranchKey);
        List<SelectListModel> FillUserManualTypes();
    }
}
