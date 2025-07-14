using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Data;
using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Services
{
    public class BaseStudentSearchService : IBaseStudentSearchService
    {
        private EduSuiteDatabase dbContext;

        public BaseStudentSearchService(EduSuiteDatabase objdb)
        {
            this.dbContext = objdb;
        }
        public void FillDropDownLists(BaseSearchStudentsViewModel model)
        {
            FillAcademicTerms(model);
            FillCourseTypes(model);
            FillCourse(model);
            FillUniversityMasters(model);
            FillModes(model);
            FillBatches(model);
            FillBranches(model);
            //FillClassModes(model);
            //FillReligions(model);
            //FillSecondLanguages(model);
            //FillMediums(model);
            //FillIncomes(model);
            //FillNatureOfEnquiry(model);
            //FillAgents(model);
            FillStudentStatus(model);
            FillClass(model);

        }
        public void FillAcademicTerms(BaseSearchStudentsViewModel model)
        {
            model.AcademicTerms = dbContext.VwAcadamicTermSelectActiveOnlies.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AcademicTermName
            }).ToList();
        }
        public void FillCourseTypes(BaseSearchStudentsViewModel model)
        {

            if (model.AcademicTermKey != null)
            {
                model.CourseTypes = dbContext.UniversityCourses.Where(x => (x.AcademicTermKey == model.AcademicTermKey) && (x.Course.CourseType.IsActive == true) && x.IsActive == true).Select(row => new SelectListModel
                {
                    RowKey = row.Course.CourseType.RowKey,
                    Text = row.Course.CourseType.CourseTypeName

                }).GroupBy(row => row.RowKey).Select(row => row.FirstOrDefault()).ToList();
            }
            else
            {
                model.CourseTypes = dbContext.VwCourseTypeSelectActiveOnlies.Select(row => new SelectListModel
                {
                    RowKey = row.RowKey,
                    Text = row.CourseTypeName
                }).ToList();
            }
        }
        public void FillCourse(BaseSearchStudentsViewModel model)
        {

            var Courses = dbContext.UniversityCourses.Select(row => new UniversityCourseViewModel
            {
                CourseKey = row.CourseKey,
                CourseName = row.Course.CourseName,
                CourseTypeKey = row.Course.CourseTypeKey
            });
            if (model.CourseTypeKeys.Count > 0)
            {
                Courses = Courses.Where(x => model.CourseTypeKeys.Contains(x.CourseTypeKey));
            }
            model.Courses = Courses.Select(row => new SelectListModel
            {
                RowKey = row.CourseKey,
                Text = row.CourseName
            }).GroupBy(row => row.RowKey).Select(row => row.FirstOrDefault()).ToList();

        }
        public void FillUniversityMasters(BaseSearchStudentsViewModel model)
        {


            var UniversityMasters = dbContext.UniversityCourses.Select(row => new UniversityCourseViewModel
            {
                CourseKey = row.CourseKey,
                UniversityName = row.UniversityMaster.UniversityMasterName,
                UniversityMasterKey = row.UniversityMasterKey
            });

            if (model.CourseKeys.Count > 0)
            {
                UniversityMasters = UniversityMasters.Where(x => model.CourseKeys.Contains(x.CourseKey));
            }
            model.UniversityMasters = UniversityMasters.Select(row => new SelectListModel
            {
                RowKey = row.UniversityMasterKey,
                Text = row.UniversityName
            }).GroupBy(row => row.RowKey).Select(row => row.FirstOrDefault()).ToList();

        }
        public void FillYears(BaseSearchStudentsViewModel model)
        {


            model.CourseYears = dbContext.fnStudentYear(model.AcademicTermKey).Select(x => new SelectListModel
            {
                RowKey = x.RowKey ?? 0,
                Text = x.YearName
            }).ToList();

        }
        public void FillModes(BaseSearchStudentsViewModel model)
        {
            model.Modes = dbContext.VwModeSelectActiveOnlies.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ModeName
            }).ToList();
        }
        private void FillBatches(BaseSearchStudentsViewModel model)
        {
            model.Batches = dbContext.VwBatchSelectActiveOnlies.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BatchName
            }).ToList();
        }
        private void FillBranches(BaseSearchStudentsViewModel model)
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
                    model.BranchKeys.Add(Employee.BranchKey);
                }
                else
                {
                    model.Branches = BranchQuery.Where(x => x.RowKey == Employee.BranchKey).ToList();
                    model.BranchKeys.Add(Employee.BranchKey);
                }
            }
            else
            {
                model.Branches = BranchQuery.ToList();
            }

            if (model.Branches.Count == 1)
            {
                long? branchkey = model.Branches.Select(x => x.RowKey).FirstOrDefault();
                model.BranchKeys.Add(branchkey ?? 0);
            }
        }
        public void FillClassModes(BaseSearchStudentsViewModel model)
        {
            model.ClassModes = dbContext.VwClassModeSelectActiveOnlies.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ClassModeName
            }).ToList();
        }
        public void FillReligions(BaseSearchStudentsViewModel model)
        {
            model.Religions = dbContext.VwReligionSelectActiveOnlies.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ReligionName
            }).ToList();
        }
        public void FillSecondLanguages(BaseSearchStudentsViewModel model)
        {
            model.SecondLanguages = dbContext.VwSecoundLanguageSelectActiveOnlies.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.SecondLanguageName
            }).ToList();
        }
        public void FillMediums(BaseSearchStudentsViewModel model)
        {
            model.Mediums = dbContext.VwMediumSelectActiveOnlies.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.MediumName
            }).ToList();
        }
        public void FillIncomes(BaseSearchStudentsViewModel model)
        {
            model.Incomes = dbContext.VwIncomeSelectActiveOnlies.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.IncomeName
            }).ToList();
        }
        public void FillNatureOfEnquiry(BaseSearchStudentsViewModel model)
        {
            model.NatureOfEnquiries = dbContext.NatureOfEnquiries.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.NatureOfEnquiryName
            }).ToList();
        }
        public void FillAgents(BaseSearchStudentsViewModel model)
        {
            model.Agents = dbContext.Agents.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AgentName
            }).ToList();
        }
        private void FillStudentStatus(BaseSearchStudentsViewModel model)
        {
            model.StudentStatuses = dbContext.StudentStatus.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.StudentStatusName
            }).ToList();
        }
        private void FillClass(BaseSearchStudentsViewModel model)
        {
            model.Classes = dbContext.VwClassDetailsSelectActiveOnlies.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ClassCode
            }).ToList();
        }
        private List<long> SplitKey(string Data)
        {
            return Data.Split(',').Select(long.Parse).ToList();
        }
        public void FillRegistrationCatagory(BaseSearchStudentsViewModel model)
        {
            model.RegistrationStatus = dbContext.RegistratonCatagories.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.CatagoryName
            }).ToList();
        }
        public void FillCaste(BaseSearchStudentsViewModel model)
        {

            var castes = dbContext.Castes.Where(x => x.IsActive).Select(row => new ApplicationPersonalViewModel
            {
                CasteKey = row.RowKey,
                CasteName = row.CasteName,
                ReligionKey = row.ReligionKey
            });
            if (model.ReligionKeys.Count > 0)
            {
                castes = castes.Where(x => model.ReligionKeys.Contains(x.ReligionKey ?? 0));
            }
            model.Castes = castes.Select(row => new SelectListModel
            {
                RowKey = row.CasteKey ?? 0,
                Text = row.CasteName
            }).GroupBy(row => row.RowKey).Select(row => row.FirstOrDefault()).ToList();
        }
        public void FillCommunityType(BaseSearchStudentsViewModel model)
        {
            model.CommunityTypes = dbContext.CommunityTypes.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.Name
            }).ToList();
        }
        public void FillBloodGroup(BaseSearchStudentsViewModel model)
        {
            model.BloodGroups = dbContext.BloodGroups.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BloodGroupName
            }).ToList();
        }

    }
}
