using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace CITS.EduSuite.Business.Services
{
    public class SelectListService : ISelectListService
    {
        private EduSuiteDatabase dbContext;

        public SelectListService(EduSuiteDatabase objdb)
        {
            this.dbContext = objdb;
        }

        public List<SelectListModel> FillBranches()
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
                    return BranchQuery.Where(row => Branches.Contains(row.RowKey)).ToList();
                }
                else
                {
                    return BranchQuery.Where(x => x.RowKey == Employee.BranchKey).ToList();
                }
            }
            else
            {
                return BranchQuery.ToList();
            }

        }
        public List<SelectListModel> FillSalaryType()
        {

            return dbContext.SalaryTypes.Where(row => (bool)row.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.SalaryTypeName
            }).ToList();

        }

        public List<SelectListModel> FillClasses(short BranchKey)
        {
            Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey && x.IsTeacher == true).SingleOrDefault();
            if (Employee != null)
            {
                return (from CD in dbContext.VwClassDetailsSelectActiveOnlies
                        join SDA in dbContext.StudentDivisionAllocations on CD.RowKey equals SDA.ClassDetailsKey
                        join TCA in dbContext.TeacherClassAllocations on CD.RowKey equals TCA.ClassDetailsKey
                        join A in dbContext.Applications on CD.RowKey equals A.ClassDetailsKey
                        where TCA.EmployeeKey == Employee.RowKey && TCA.IsAttendance == true && TCA.IsActive == true && (SDA.IsActive == true && A.BranchKey == Employee.BranchKey)
                        select new SelectListModel
                        {
                            RowKey = CD.RowKey,
                            Text = CD.ClassCode + CD.ClassCodeDescription,
                            Selected = TCA.IsAttendance
                        }).Distinct().Union(from CD in dbContext.VwClassDetailsSelectActiveOnlies
                                            join SDA in dbContext.TimeTableTempDetails on CD.RowKey equals SDA.ClassDetailsKey
                                            join TCA in dbContext.TimeTableTempMasters on SDA.TempMasterKey equals TCA.RowKey
                                            join A in dbContext.Applications on CD.RowKey equals A.ClassDetailsKey
                                            where SDA.EmployeeKey == Employee.RowKey && System.Data.Entity.DbFunctions.TruncateTime(DateTimeUTC.Now) >= System.Data.Entity.DbFunctions.TruncateTime(TCA.FromDate) && System.Data.Entity.DbFunctions.TruncateTime(DateTimeUTC.Now) <= System.Data.Entity.DbFunctions.TruncateTime(TCA.ToDate) && (A.BranchKey == Employee.BranchKey)
                                            select new SelectListModel
                                            {
                                                RowKey = CD.RowKey,
                                                Text = CD.ClassCode + CD.ClassCodeDescription,
                                                Selected = true
                                            }).ToList();

            }
            else
            {
                return (from CD in dbContext.VwClassDetailsSelectActiveOnlies

                        join SDA in dbContext.StudentDivisionAllocations on CD.RowKey equals SDA.ClassDetailsKey
                        join A in dbContext.Applications on CD.RowKey equals A.ClassDetailsKey
                        where (SDA.IsActive == true && A.BranchKey == BranchKey)
                        select new SelectListModel
                        {
                            RowKey = CD.RowKey,
                            Text = CD.ClassCode + CD.ClassCodeDescription
                        }).Distinct().ToList();
            }



        }
        public List<SelectListModel> FillTeachersByClass(short BranchKey, long ClassDetailsKey)
        {

            return (from CD in dbContext.ClassDetails
                    join TCA in dbContext.TeacherClassAllocations on CD.RowKey equals TCA.ClassDetailsKey
                    join E in dbContext.Employees on TCA.EmployeeKey equals E.RowKey
                    where CD.RowKey == ClassDetailsKey && TCA.IsActive == true && E.BranchKey == BranchKey
                    select new SelectListModel
                    {
                        RowKey = E.RowKey,
                        Text = E.FirstName
                    }).Distinct().ToList();
        }
        public List<SelectListModel> FillAllTeachers(short BranchKey)
        {

            return (from E in dbContext.Employees
                    where E.IsTeacher == true && E.EmployeeStatusKey == DbConstants.EmployeeStatus.Working && E.IsActive == true && E.BranchKey == BranchKey
                    select new SelectListModel
                    {
                        RowKey = E.RowKey,
                        Text = E.FirstName
                    }).Distinct().ToList();
        }
        public List<ApplicationPersonalViewModel> FillStudentsByClass(short BranchKey, long ClassDetailsKey)
        {
            return (from A in dbContext.Applications
                    where (A.BranchKey == BranchKey &&
                A.ClassDetailsKey == ClassDetailsKey)
                    select new ApplicationPersonalViewModel
                    {
                        ApplicationKey = A.RowKey,
                        ApplicantName = A.StudentName,
                        AdmissionNo = A.AdmissionNo,
                        RollNoCode = A.RollNoCode,
                        ApplicantPhoto = A.StudentPhotoPath

                    }).ToList();
        }
        public List<SelectListModel> FillStudentStatuses()
        {
            return dbContext.StudentStatus.Where(row => row.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.StudentStatusName
            }).ToList();
        }
        public List<SelectListModel> FillAttendanceTypeByDate(DateTime Date, long ApplicationKey, short AttendanceStatusKey, short AttendanceTypeKey)
        {
            if (AttendanceStatusKey == DbConstants.AttendanceStatus.Absent)
            {
                if (dbContext.AttendanceDetails.Any(row => System.Data.Entity.DbFunctions.TruncateTime(row.Attendance.AttendanceDate) == System.Data.Entity.DbFunctions.TruncateTime(Date) && row.Attendance.ApplicationKey == ApplicationKey))
                {

                    return dbContext.AttendanceDetails.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.Attendance.AttendanceDate) == System.Data.Entity.DbFunctions.TruncateTime(Date) && row.Attendance.ApplicationKey == ApplicationKey && row.AttendanceStatusKey == DbConstants.AttendanceStatus.Absent)
                     .Select(row => new SelectListModel
                     {
                         RowKey = row.AttendanceTypeKey,
                         Text = row.AttendanceType.AttendanceTypeName
                     }).Union(dbContext.AttendanceTypes.Where(row => row.RowKey == AttendanceTypeKey)
                       .Select(row => new SelectListModel
                       {
                           RowKey = row.RowKey,
                           Text = row.AttendanceTypeName
                       })).ToList();
                }
                else
                {
                    return new List<SelectListModel>();
                    //return dbContext.AttendanceTypes.Where(row => row.IsActive)
                    //   .Select(row => new SelectListModel
                    //   {
                    //       RowKey = row.RowKey,
                    //       Text = row.AttendanceTypeName
                    //   }).ToList();

                }

            }
            else
            {
                return dbContext.AttendanceDetails.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.Attendance.AttendanceDate) == System.Data.Entity.DbFunctions.TruncateTime(Date) && row.Attendance.ApplicationKey == ApplicationKey && (row.AttendanceStatusKey == DbConstants.AttendanceStatus.Present || row.AttendanceTypeKey == AttendanceTypeKey))
                     .OrderByDescending(row => row.RowKey).Select(row => new SelectListModel
                     {
                         RowKey = row.AttendanceTypeKey,
                         Text = row.AttendanceType.AttendanceTypeName
                     }).Take(1).ToList();
            }
        }
        public List<ApplicationPersonalViewModel> FillStudentsByClassForParentsMeet(long RowKey, long ClassDetailsKey)
        {

            return (from A in dbContext.Applications
                    where (A.ClassDetailsKey == ClassDetailsKey)
                    select new ApplicationPersonalViewModel
                    {
                        RowKey = RowKey,
                        ApplicationKey = A.RowKey,
                        ApplicantName = A.StudentName,
                        AdmissionNo = A.AdmissionNo,
                        RollNoCode = A.RollNoCode,
                        ApplicantPhoto = A.StudentPhotoPath,
                        ClassDetailsKey = A.ClassDetailsKey,
                        ClassDetailsName = A.ClassDetail.ClassCode,
                        BranchKey = A.BranchKey,
                        //ParentsMeetSigntext = dbContext.ParentsMeetDetails.Any(x => x.ParentMeetScheduleKey == RowKey && x.ApplicationKey == A.RowKey) ? true : false,

                    }).ToList();
        }
        public List<SelectListModel> FillApplicationSubjects(long RowKey)
        {
            Application application = dbContext.Applications.Where(x => x.RowKey == RowKey).FirstOrDefault();

            return (from S in dbContext.Subjects
                    join CSD in dbContext.CourseSubjectDetails on S.RowKey equals CSD.SubjectKey

                    where (CSD.CourseSubjectMaster.AcademicTermKey == application.AcademicTermKey && CSD.CourseSubjectMaster.UniversityMasterKey == application.UniversityMasterKey
                      && CSD.CourseSubjectMaster.CourseKey == application.CourseKey)
                    select new SelectListModel
                    {
                        RowKey = S.RowKey,
                        Text = S.SubjectCode,
                    }).ToList();
        }
        public List<AutoCompleteModel> AutoCompleteEmployeeCode(string Text)
        {

            return dbContext.Employees.Where(row => row.IsActive == true && row.EmployeeCode.Contains(Text) || row.FirstName.Contains(Text) || row.LastName.Contains(Text)).Select(row => new AutoCompleteModel
            {
                Value = row.EmployeeCode,
                Name = row.FirstName + " ( " + row.EmployeeCode + " ) "
            }).ToList();
        }
        public List<AutoCompleteModel> AutoCompleteAdmissionNo(string Text)
        {

            return dbContext.Applications.Where(row => row.AdmissionNo.Contains(Text) || row.RollNoCode.Contains(Text) || row.StudentName.Contains(Text)).Select(row => new AutoCompleteModel
            {
                Value = row.AdmissionNo,
                Name = row.StudentName + " ( " + row.AdmissionNo + " ) "
            }).ToList();
        }

        #region Online exam
        public List<SelectListModel> FillSubjects()
        {
            return dbContext.Subjects.Where(row => row.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.SubjectName
            }).ToList();
        }
        public List<SelectListModel> FillAnswerKeyModules(long TestPaperKey)
        {
            return dbContext.TestModules.Where(row => row.TestPaperKey == TestPaperKey).Select(row => new SelectListModel
            {
                RowKey = row.ModuleKey,
                Text = row.Module.ModuleName,
            }).GroupBy(x => x.RowKey).Select(y => y.FirstOrDefault()).ToList();
        }
        public List<TestPaperViewModel> FillExamModules(long TestPaperKey, long? ApplicationKey)
        {

            if (DbConstants.User.RoleKey == DbConstants.Role.Students)
            {


                ApplicationKey = dbContext.AppUsers.Where(row => row.RowKey == DbConstants.User.UserKey).Select(row => row.ApplicationKey ?? 0).FirstOrDefault();
            }
            return dbContext.TestModules.Where(row => row.TestPaperKey == TestPaperKey).Select(row => new TestPaperViewModel
            {
                ModuleKey = row.ModuleKey,
                ModuleName = row.Module.ModuleName,
                TestDuration = row.TestDuration,
                QuestionCount = dbContext.TestQuestions.Where(x => x.TestSection.TestModuleKey == row.RowKey).Count(),
                //SectionCount = row.TestSections.Count(),
                SectionCount = 1,
                TotalMark = dbContext.ExamTestSections.Where(x => x.ExamTest.TestPaperKey == TestPaperKey && x.ExamTest.ApplicationKey == ApplicationKey).Select(x => x.TotalMark).FirstOrDefault()
            }).OrderBy(row => row.ModuleKey).ToList();
        }
        public List<MarkGroupViewModel> FillMarkGroups()
        {
            return dbContext.VwSelectMarkGroups.OrderBy(X => X.MarkGroupName).Select(row => new MarkGroupViewModel
            {
                RowKey = row.RowKey,
                MarkGroupName = row.MarkGroupName2,
                Mark = row.Mark ?? 0,
                NegativeMark = row.NegativeMark ?? 0
            }).ToList();
        }
        public List<SelectListModel> FillProvinceById(short CountryKey)
        {
            return dbContext.Provinces.OrderBy(row => row.DisplayOrder).Where(row => row.CountryKey == CountryKey).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.Provincename
            }).ToList();
        }
        public List<SelectListModel> FillQuestionModules()
        {
            return dbContext.Modules.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ModuleName,

            }).ToList();
        }
        public List<TestSectionViewModel> FillQuestionSectionById(long TestPaperKey, byte ModuleKey)
        {
            return dbContext.TestSections.Where(row => row.TestModule.TestPaperKey == TestPaperKey).Select(row => new TestSectionViewModel
            {
                RowKey = row.RowKey,
                TestSectionName = row.SectionName

            }).ToList();
        }
        public List<SelectListModel> FillQuestionTypes()
        {

            return dbContext.QuestionTypes.Where(row => row.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.QuestionTypeName
            }).ToList();
        }

        #endregion Online exam
        public List<SelectListModel> FillServiceTypes()
        {
            return dbContext.ServiceTypes.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ServiceTypeName
            }).ToList();
        }
        public List<SelectListModel> FillFeeTypes()
        {
            return dbContext.FeeTypes.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.FeeTypeName
            }).ToList();
        }
        public List<SelectListModel> FillAcademicTerms()
        {
            return dbContext.VwAcadamicTermSelectActiveOnlies.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AcademicTermName
            }).ToList();
        }
        public List<SelectListModel> FillCourseTypesById(short? AcademicTermKey)
        {

            if (AcademicTermKey != null)
            {
                return dbContext.UniversityCourses.Where(x => (x.AcademicTermKey == (AcademicTermKey ?? x.AcademicTermKey)) && (x.Course.CourseType.IsActive == true) && x.IsActive == true).Select(row => new SelectListModel
                {
                    RowKey = row.Course.CourseType.RowKey,
                    Text = row.Course.CourseType.CourseTypeName

                }).GroupBy(row => row.RowKey).Select(row => row.FirstOrDefault()).ToList();
            }
            else
            {
                return dbContext.VwCourseTypeSelectActiveOnlies.Select(row => new SelectListModel
                {
                    RowKey = row.RowKey,
                    Text = row.CourseTypeName
                }).ToList();
            }
        }
        public List<SelectListModel> FillCoursesById(short? CourseTypeKey)
        {

            var Courses = dbContext.UniversityCourses.Where(x => x.Course.CourseTypeKey == (CourseTypeKey ?? x.Course.CourseTypeKey)).Select(row => new UniversityCourseViewModel
            {
                CourseKey = row.CourseKey,
                CourseName = row.Course.CourseName,
                CourseTypeKey = row.Course.CourseTypeKey
            });
            return Courses.Select(row => new SelectListModel
            {
                RowKey = row.CourseKey,
                Text = row.CourseName
            }).GroupBy(row => row.RowKey).Select(row => row.FirstOrDefault()).ToList();

        }
        public List<SelectListModel> FillUniversitiesById(long? CourseKey)
        {
            var UniversityMasters = dbContext.UniversityCourses.Where(x => x.CourseKey == (CourseKey ?? x.CourseKey)).Select(row => new UniversityCourseViewModel
            {
                CourseKey = row.CourseKey,
                UniversityName = row.UniversityMaster.UniversityMasterName,
                UniversityMasterKey = row.UniversityMasterKey
            });

            return UniversityMasters.Select(row => new SelectListModel
            {
                RowKey = row.UniversityMasterKey,
                Text = row.UniversityName
            }).GroupBy(row => row.RowKey).Select(row => row.FirstOrDefault()).ToList();

        }
        public List<SelectListModel> FillYearsById(short AcademicTermKey, long CourseKey)
        {
            decimal CourseDuration = dbContext.Courses.Where(x => x.RowKey == CourseKey).Select(x => x.CourseDuration ?? 0).FirstOrDefault();
            decimal Duration = dbContext.AcademicTerms.Where(x => x.RowKey == AcademicTermKey).Select(x => x.Duration).FirstOrDefault();
            Duration = Duration != 0 ? Duration : 1;
            var duration = Math.Ceiling((Convert.ToDecimal(AcademicTermKey == DbConstants.AcademicTerm.Semester ? CourseDuration / Duration : CourseDuration / Duration)));
            return dbContext.fnStudentYear(AcademicTermKey).Select(x => new SelectListModel
            {
                RowKey = x.RowKey ?? 0,
                Text = x.YearName
            }).Where(x => x.RowKey <= duration).ToList();

        }
        public List<SelectListModel> FillBatches(short? BranchKey)
        {

            if (BranchKey != 0 && BranchKey != null)
            {
                return (from p in dbContext.Applications
                        join B in dbContext.VwBatchSelectActiveOnlies on p.BatchKey equals B.RowKey
                        orderby B.RowKey

                        where (p.BranchKey == BranchKey)
                        select new SelectListModel
                        {
                            RowKey = B.RowKey,
                            Text = B.BatchName
                        }).Distinct().ToList();
            }
            else
            {
                return (from B in dbContext.VwBatchSelectActiveOnlies
                        orderby B.RowKey
                        select new SelectListModel
                        {
                            RowKey = B.RowKey,
                            Text = B.BatchName
                        }).Distinct().ToList();
            }

        }
        public List<SelectListModel> FillStudentsById(short? BranchKey, short? BatchKey, short? AcademicTermKey, long? CourseKey, short? UniversityKey, bool ClassRequired)
        {
            var Applications = dbContext.Applications.Where(x => x.StudentStatusKey == DbConstants.StudentStatus.Ongoing && x.BranchKey == (BranchKey ?? x.BranchKey) && x.BatchKey == (BatchKey ?? x.BatchKey) && x.AcademicTermKey == AcademicTermKey && x.CourseKey == CourseKey && x.UniversityMasterKey == UniversityKey);
            if (ClassRequired == true)
            {
                Applications = Applications.Where(x => x.ClassDetailsKey != null);
            }
            return Applications.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                ValueText = row.AdmissionNo,
                Text = row.StudentName
            }).ToList();
        }
        public List<SelectListModel> FillSubjectsById(short? AcademicTermKey, long? CourseKey, short? UniversityKey, int? StudentYearKey)
        {
            return dbContext.VwSubjectSelectActiveOnlies.Where(x => x.AcademicTermKey == AcademicTermKey && x.CourseKey == CourseKey && x.UniversityMasterKey == UniversityKey && x.CourseYear == (StudentYearKey ?? x.CourseYear)).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                ValueText = row.SubjectCode,
                Text = row.SubjectName
            }).ToList();
        }
        public List<SelectListModel> FillStudyMaterialById(short? AcademicTermKey, long? CourseKey, short? UniversityKey, int? StudentYearKey)
        {
            return dbContext.VwStudyMaterialSelectActiveOnlies.Where(x => x.AcademicTermKey == AcademicTermKey && x.CourseKey == CourseKey && x.UniversityMasterKey == UniversityKey && x.CourseYear == (StudentYearKey ?? x.CourseYear)).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                ValueText = row.StudyMaterialCode,
                Text = row.StudyMaterialName
            }).ToList();
        }
        public List<SelectListModel> FillCertificates()
        {
            return dbContext.VwCertificateTypeSelectActiveOnlies.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.CertificateTypeName
            }).ToList();
        }
        public List<SelectListModel> FillExamTerms()
        {
            return dbContext.ExamTerms.Where(x => (x.IsActive ?? true)).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ExamTermName
            }).ToList();
        }
        public List<SelectListModel> FillExamCenters()
        {
            return dbContext.ExamCenters.Where(x => (x.IsActive ?? true)).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                ValueText = row.ExamCentreCode,
                Text = row.ExamCentreName
            }).ToList();
        }
        public List<SelectListModel> FillInternalExamTerms()
        {
            return dbContext.VwInternalExamTermSelectActiveOnlies.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.InternalExamTermName
            }).ToList();
        }
        public List<SelectListModel> FillEmployeesById(short BranchKey)
        {
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

            if (BranchKey != 0)
            {
                Employees = Employees.Where(x => x.BranchKey == BranchKey).ToList();
            }

            List<SelectListModel> EmployeeList = new List<SelectListModel>();
            EmployeeList = Employees.Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.FirstName,
                ValueText = x.EmployeeCode
            }).ToList();


            return EmployeeList;
        }
        public List<SelectListModel> FillAppUserById(short BranchKey)
        {
            List<short> RoleKeys = new List<short>();
            RoleKeys.Add(DbConstants.Role.Parents);
            RoleKeys.Add(DbConstants.Role.Students);

            List<SelectListModel> Employees = new List<SelectListModel>();

            Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();

            if (DbConstants.User.UserKey != DbConstants.AdminKey)
            {
                if (Employee != null)
                {
                    var Branches = Employee.BranchAccess.Split(',').Select(Int16.Parse).ToList();

                    if (Branches.Count > 1)
                    {
                        Employees = (from AU in dbContext.AppUsers
                                     join E in dbContext.Employees on AU.RowKey equals E.AppUserKey into EAU
                                     from E in EAU.DefaultIfEmpty()
                                     where (E.IsActive == true && AU.RoleKey == DbConstants.Role.Staff && Branches.Contains((E.BranchKey == null ? BranchKey : E.BranchKey)))
                                     select new SelectListModel
                                     {
                                         RowKey = AU.RowKey,
                                         Text = AU.FirstName + " " + (AU.MiddleName ?? "") + " " + AU.LastName,

                                     }).OrderBy(row => row.RowKey).ToList();
                    }
                    else
                    {

                        Employees = (from AU in dbContext.AppUsers
                                     join E in dbContext.Employees on AU.RowKey equals E.AppUserKey into EAU
                                     from E in EAU.DefaultIfEmpty()
                                     where (E.IsActive == true && AU.RoleKey == DbConstants.Role.Staff && E.RowKey == Employee.RowKey)
                                     select new SelectListModel
                                     {
                                         RowKey = AU.RowKey,
                                         Text = AU.FirstName + " " + (AU.MiddleName ?? "") + " " + AU.LastName,

                                     }).OrderBy(row => row.RowKey).ToList();
                    }
                }
            }
            else
            {
                Employees = (from AU in dbContext.AppUsers
                             join E in dbContext.Employees.Where(x => x.IsActive == true && x.EmployeeStatusKey == DbConstants.EmployeeStatus.Working && (x.BranchKey == null ? BranchKey : x.BranchKey) == BranchKey) on AU.RowKey equals E.AppUserKey into EAU
                             from E in EAU.DefaultIfEmpty()
                             where (!RoleKeys.Contains(AU.RoleKey))
                             select new SelectListModel
                             {
                                 RowKey = AU.RowKey,
                                 Text = AU.FirstName + " " + (AU.MiddleName ?? "") + " " + AU.LastName,

                             }).OrderBy(row => row.Text).ToList();


            }
            return Employees;

        }
        public List<SelectListModel> FillTelephoneCodes()
        {
            return dbContext.VwCountrySelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.TelephoneCode
            }).ToList();
        }
        public List<SelectListModel> FillPaymentModeSub(short? PaymentModeKey)
        {
            return dbContext.PaymentModeSubs.Where(x => x.IsActive && x.PaymentModeKey == DbConstants.PaymentMode.Bank).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.PaymentModeSubName
            }).ToList();

        }
        public List<SelectListModel> FillDashBoardTypes()
        {
            IQueryable<SelectListModel> DashBoardTypeQuery = dbContext.DashBoardTypes.Where(x => x.IsActive).OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.DashBoardTypeName,
                ValueText = row.DashBoardTypeCode
            });

            Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
            if (Employee != null)
            {
                if (Employee.DashBoardUserPermissions != null)
                {
                    List<short> DashBoardTypes = Employee.DashBoardUserPermissions.Select(x => x.DashBoardContent.DashBoardTypeKey).Distinct().ToList();
                    List<long> DashBoardTypess = DashBoardTypes.Select(i => (long)i).ToList();
                    return DashBoardTypeQuery.Where(row => DashBoardTypess.Contains(row.RowKey)).ToList();

                }
                else
                {
                    return DashBoardTypeQuery.Where(x => x.RowKey == 0).ToList();

                }
            }
            else
            {
                return DashBoardTypeQuery.ToList();
            }
        }
        public List<SelectListModel> ApplicationCourseYear(long ApplicationKey)
        {
            Application Application = dbContext.Applications.SingleOrDefault(row => row.RowKey == ApplicationKey);

            List<SelectListModel> courseyear = new List<SelectListModel>();
            if (Application != null)
            {
                var CourseDuration = Application.Course.CourseDuration;
                var duration = Math.Ceiling((Convert.ToDecimal(Application.AcademicTermKey == DbConstants.AcademicTerm.Semester ? CourseDuration / 6 : CourseDuration / 12)));

                var StartYear = Application.StartYear ?? 0;
                if (duration < 1)
                {
                    courseyear.Add(new SelectListModel
                    {
                        RowKey = 1,
                        Text = " Short Term"
                    });
                }
                else
                {
                    for (int i = StartYear; i <= duration; i++)
                    {
                        courseyear.Add(new SelectListModel
                        {
                            RowKey = i,
                            Text = i + (Application.AcademicTermKey == DbConstants.AcademicTerm.Semester ? " Semester" : " Year")
                        });
                    }
                }
            }
            return courseyear.ToList();
        }
        public List<SelectListModel> FillMenuType()
        {
            return dbContext.MenuTypes.Where(x => x.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.MenuTypeName
            }).ToList();
        }
        public List<SelectListModel> FillMenuCatagory()
        {
            return dbContext.MenuCatagories.Where(x => x.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.CatagoryName
            }).ToList();
        }
        public List<SelectListModel> FillMenu(short? MenuTypeKey)
        {
            if (MenuTypeKey != 0 && MenuTypeKey != null)
            {
                return (from p in dbContext.Menus
                        orderby p.RowKey
                        where (p.MenuTypeKey == MenuTypeKey)
                        select new SelectListModel
                        {
                            RowKey = p.RowKey,
                            Text = p.MenuName
                        }).Distinct().ToList();
            }
            else
            {
                return (from p in dbContext.Menus
                        orderby p.RowKey
                        select new SelectListModel
                        {
                            RowKey = p.RowKey,
                            Text = p.MenuName
                        }).Distinct().ToList();
            }
        }
        public List<SelectListModel> FillActions()
        {
            return dbContext.Actions.Where(x => x.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ActionName
            }).ToList();
        }
        public List<SelectListModel> FillSearchBatch(short? BranchKey)
        {
            if (BranchKey != 0 && BranchKey != null)
            {
                return (from p in dbContext.Applications
                        join B in dbContext.VwBatchSelectActiveOnlies on p.BatchKey equals B.RowKey
                        orderby B.RowKey descending
                        where (p.BranchKey == BranchKey)
                        select new SelectListModel
                        {
                            RowKey = B.RowKey,
                            Text = B.BatchName
                        }).Distinct().ToList();
            }
            else
            {
                return (from p in dbContext.Applications
                        join B in dbContext.VwBatchSelectActiveOnlies on p.BatchKey equals B.RowKey
                        orderby B.RowKey descending
                        select new SelectListModel
                        {
                            RowKey = B.RowKey,
                            Text = B.BatchName
                        }).Distinct().ToList();
            }
        }
        public List<SelectListModel> FillSearchCourse(short? BranchKey)
        {
            if (BranchKey != 0 && BranchKey != null)
            {
                return (from p in dbContext.Applications
                        join B in dbContext.VwCourseSelectActiveOnlies on p.CourseKey equals B.RowKey
                        orderby B.RowKey
                        where (p.BranchKey == BranchKey)
                        select new SelectListModel
                        {
                            RowKey = B.RowKey,
                            Text = B.CourseName
                        }).Distinct().ToList();
            }
            else
            {
                return (from p in dbContext.Applications
                        join B in dbContext.VwCourseSelectActiveOnlies on p.CourseKey equals B.RowKey
                        orderby B.RowKey
                        select new SelectListModel
                        {
                            RowKey = B.RowKey,
                            Text = B.CourseName
                        }).Distinct().ToList();
            }
        }
        public List<SelectListModel> FillSearchUniversity(short? BranchKey)
        {
            if (BranchKey != 0 && BranchKey != null)
            {
                return (from p in dbContext.Applications
                        join B in dbContext.VwUniversityMasterSelectActiveOnlies on p.UniversityMasterKey equals B.RowKey
                        orderby B.RowKey
                        where (p.BranchKey == BranchKey)
                        select new SelectListModel
                        {
                            RowKey = B.RowKey,
                            Text = B.UniversityMasterName
                        }).Distinct().ToList();
            }
            else
            {
                return (from p in dbContext.Applications
                        join B in dbContext.VwUniversityMasterSelectActiveOnlies on p.UniversityMasterKey equals B.RowKey
                        orderby B.RowKey
                        select new SelectListModel
                        {
                            RowKey = B.RowKey,
                            Text = B.UniversityMasterName
                        }).Distinct().ToList();
            }
        }
        public List<SelectListModel> FillSearchBankAccounts(short? BranchKey)
        {
            if (BranchKey != 0 && BranchKey != null)
            {
                return (from B in dbContext.BranchAccounts
                        orderby B.RowKey descending
                        where (B.BranchKey == BranchKey && B.BankAccount.IsActive)
                        select new SelectListModel
                        {
                            RowKey = B.BankAccount.RowKey,
                            Text = (B.BankAccount.NameInAccount ?? B.BankAccount.AccountNumber) + EduSuiteUIResources.Hyphen + B.BankAccount.Bank.BankName
                        }).Distinct().ToList();
            }
            else
            {
                return (from B in dbContext.BankAccounts.Where(x => x.IsActive)
                        orderby B.RowKey descending
                        select new SelectListModel
                        {
                            RowKey = B.RowKey,
                            Text = (B.NameInAccount ?? B.AccountNumber) + EduSuiteUIResources.Hyphen + B.Bank.BankName
                        }).Distinct().ToList();
            }
        }
        public List<SelectListModel> FillDesignation()
        {
            return dbContext.Designations.Where(x => x.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.DesignationName
            }).ToList();
        }
        public List<SelectListModel> FillDepartment()
        {
            return dbContext.Departments.Where(x => x.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.DepartmentName
            }).ToList();
        }
        public List<SelectListModel> FillEmployeeStatus()
        {
            return dbContext.EmployeeStatus.Where(x => x.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.EmployeeStatusName
            }).ToList();
        }
         public List<SelectListModel> FillNatureOfEnquiry()
    {
        return dbContext.NatureOfEnquiries
            .Where(x => x.IsActive)
            .Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.NatureOfEnquiryName
            }).ToList();
    }

        public List<SelectListModel> FillYears()
        {
            int SalaryYear = Convert.ToInt32(WebConfigurationManager.AppSettings["SalaryYear"]);
            int Currentyear = DateTimeUTC.Now.Year;
            List<SelectListModel> YearList = new List<SelectListModel>();
            for (int year = SalaryYear; year <= Currentyear; year++)
            {
                YearList.Add(new SelectListModel
                {
                    RowKey = year,
                    Text = year.ToString()
                });
            }
            return YearList.ToList();
        }
        public List<SelectListModel> FillMonths()
        {
            List<SelectListModel> MonthList = new List<SelectListModel>();
            for (int month = 1; month <= 12; month++)
            {
                string monthName = DateTimeFormatInfo.CurrentInfo.GetMonthName(month);
                MonthList.Add(new SelectListModel
                {
                    RowKey = month,
                    Text = monthName
                });
            }
            return MonthList.ToList();
        }
        public List<SelectListModel> FillEmployeesByBranchKeys(List<long> BranchKeys)
        {
            IQueryable<EmployeePersonalViewModel> EmployeesList = dbContext.Employees.Where(y => y.IsActive == true).Select(x => new EmployeePersonalViewModel
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

            if (BranchKeys.Count > 0)
            {
                Employees = Employees.Where(x => BranchKeys.Contains(x.BranchKey)).ToList();
            }

            List<SelectListModel> EmployeeList = new List<SelectListModel>();
            EmployeeList = Employees.Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.FirstName
            }).ToList();


            return EmployeeList;
        }
        public List<SelectListModel> FillEmployeeAttendanceStatus(short? BranchKey)
        {
            var AttendanceConfigType = dbContext.AttendanceConfigurations.Where(x => x.BranchKey == BranchKey).Select(y => y.AttendanceConfigTypeKey).FirstOrDefault();
            List<short> AttendenceStatusKeys = new List<short> { DbConstants.EmployeeAttendanceStatus.Present, DbConstants.EmployeeAttendanceStatus.Absent, DbConstants.EmployeeAttendanceStatus.Halfday };
            if (AttendanceConfigType != DbConstants.AttendanceConfigType.MarkPresent && AttendanceConfigType != 0)
            {
                AttendenceStatusKeys = new List<short> { DbConstants.EmployeeAttendanceStatus.Present, DbConstants.EmployeeAttendanceStatus.Absent };
            }
            return dbContext.EmployeeAttendanceStatus.Where(row => row.IsActive == true && AttendenceStatusKeys.Contains(row.RowKey)).OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AttendanceStatusCode,
                ValueText = row.AttendanceStatusName
            }).ToList();
        }
        public List<SelectListModel> FillUserManualTypes()
        {
            return dbContext.UserManualTypes.Where(x => x.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.UserManualTypeName
            }).ToList();
        }
    }
}
