using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;



namespace CITS.EduSuite.Business.Services
{
    public class TestPaperService : ITestPaperService
    {
        private EduSuiteDatabase dbContext;
        public TestPaperService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;

        }


        public TestPaperViewModel FillStaticDropDown(TestPaperViewModel model)
        {
            FillBranches(model);
            FillSecondLanguage(model);
            FillExamTypes(model);
            FillMedium(model);
            FillMode(model);
            FillBatches(model);
            return model;
        }

        public TestPaperViewModel FillDynamicDropDownlists(TestPaperViewModel model)
        {
            FillAcademicTerm(model);
            FillCourseTypes(model);
            FillCourse(model);
            FillUniversity(model);
            FillClassMode(model);
            FillClassDetails(model);
           
            return model;
        }

        public void FillExamTypes(TestPaperViewModel model)
        {

            model.ExamTypes = dbContext.ExamTypes.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ExamTypeName
            }).ToList();
        }

        public void FillAcademicTerm(TestPaperViewModel model)
        {
            IQueryable<TestPaperFilter> selectList=null;
      
                selectList = dbContext.CourseSubjectDetails.Select(row => new TestPaperFilter
                {
                    RowKey = row.CourseSubjectMaster.AcademicTermKey,
                    Text = row.CourseSubjectMaster.AcademicTerm.AcademicTermName,
                    SubjectKey=row.SubjectKey

                });


            if (model.SubjectKeys.Count > 0)
            {
                selectList = selectList.Where(x => model.SubjectKeys.Contains(x.SubjectKey));
            }



            model.AcademicTerms = selectList.Select(row=>new SelectListModel
                {
                    RowKey= row.RowKey,
                    Text=row.Text

                }).GroupBy(row => row.RowKey).Select(row => row.FirstOrDefault()).ToList();
        }


        public void FillCourseTypes(TestPaperViewModel model)
        {
            IQueryable<TestPaperFilter> selectList = null;
            selectList = dbContext.CourseSubjectDetails.Select(row => new TestPaperFilter
            {
                RowKey = row.CourseSubjectMaster.Course.CourseTypeKey,
                Text = row.CourseSubjectMaster.Course.CourseType.CourseTypeName,
                SubjectKey = row.SubjectKey,
                AcademicTermKey=row.CourseSubjectMaster.AcademicTermKey
            });


            if (model.SubjectKeys.Count > 0)
            {
                selectList = selectList.Where(x => model.SubjectKeys.Contains(x.SubjectKey));
            }

            if (model.AcademicTermsKeys.Count>0)
            {
                selectList = selectList.Where(x => model.AcademicTermsKeys.Contains(x.AcademicTermKey));
            }

            model.CourseTypes = selectList.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.Text

            }).GroupBy(row => row.RowKey).Select(row => row.FirstOrDefault()).ToList();

        }


        public void FillCourse(TestPaperViewModel model)
        {
            IQueryable<TestPaperFilter> selectList = null;
            selectList = dbContext.CourseSubjectDetails.Select(row => new TestPaperFilter
            {
                RowKey = row.CourseSubjectMaster.Course.RowKey,
                Text = row.CourseSubjectMaster.Course.CourseName,
                SubjectKey = row.SubjectKey,
                AcademicTermKey = row.CourseSubjectMaster.AcademicTermKey,
                CourseTypeKey=row.CourseSubjectMaster.Course.CourseTypeKey
            });


            if (model.SubjectKeys.Count > 0)
            {
                selectList = selectList.Where(x => model.SubjectKeys.Contains(x.SubjectKey));
            }


            if (model.AcademicTermsKeys.Count > 0)
            {
                selectList = selectList.Where(x => model.AcademicTermsKeys.Contains(x.AcademicTermKey));
            }

            if (model.CourseTypeKeys.Count > 0)
            {
                selectList = selectList.Where(x => model.CourseTypeKeys.Contains(x.CourseTypeKey));
            }

            model.Courses = selectList.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.Text

            }).GroupBy(row => row.RowKey).Select(row => row.FirstOrDefault()).ToList();

        }

        public void FillUniversity(TestPaperViewModel model)
        {
            IQueryable<TestPaperFilter> selectList = null;
            selectList = dbContext.CourseSubjectDetails.Select(row => new TestPaperFilter
            {
                RowKey = row.CourseSubjectMaster.UniversityMasterKey,
                Text = row.CourseSubjectMaster.UniversityMaster.UniversityMasterName,
                SubjectKey = row.SubjectKey,
                AcademicTermKey = row.CourseSubjectMaster.AcademicTermKey,
                CourseTypeKey = row.CourseSubjectMaster.Course.CourseTypeKey,
                CourseKey=row.CourseSubjectMaster.CourseKey
            });


            if (model.SubjectKeys.Count > 0)
            {
                selectList = selectList.Where(x => model.SubjectKeys.Contains(x.SubjectKey));
            }


            if (model.AcademicTermsKeys.Count > 0)
            {
                selectList = selectList.Where(x => model.AcademicTermsKeys.Contains(x.AcademicTermKey));
            }

            if (model.CourseTypeKeys.Count > 0)
            {
                selectList = selectList.Where(x => model.CourseTypeKeys.Contains(x.CourseTypeKey));
            }

            if (model.CourseKeys.Count > 0)
            {
                selectList = selectList.Where(x => model.CourseKeys.Contains(x.CourseKey));
            }

            model.UniversityMasters = selectList.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.Text

            }).GroupBy(row => row.RowKey).Select(row => row.FirstOrDefault()).ToList();

        }

        //public FillCourseYears(TestPaperViewModel model)
        //{
        //    ApplicationPersonalViewModel applicationModel = new ApplicationPersonalViewModel();

        //}


        public List<SelectListModel> GetCourseYear(ApplicationPersonalViewModel model)
        {
            int StartYear = dbContext.Modes.Where(row => row.RowKey == model.ModeKey).Select(row => row.StartYear).SingleOrDefault();

            UniversityCourse universityCourse = dbContext.UniversityCourses.SingleOrDefault(row => row.CourseKey == model.CourseKey && row.UniversityMasterKey == model.UniversityKey && row.AcademicTermKey == model.AcademicTermKey);



            decimal duration;
            if (universityCourse != null)
            {
                var CourseDuration = universityCourse.Course.CourseDuration;
                duration = Math.Ceiling((Convert.ToDecimal(universityCourse.AcademicTermKey == DbConstants.AcademicTerm.Semester ? CourseDuration / 6 : CourseDuration / 12)));
            }
            else
            {
                var CourseDuration = dbContext.VwCourseSelectActiveOnlies.Where(row => row.RowKey == model.CourseKey).Select(row => row.CourseDuration).SingleOrDefault();
                duration = Math.Ceiling((Convert.ToDecimal(Convert.ToBoolean(model.AcademicTermKey ?? 0) ? ((CourseDuration ?? 0) / 6) : ((CourseDuration ?? 0) / 12))));

            }

            if (model.ModeKey != DbConstants.Mode.REGULAR)
            {
                if (model.AcademicTermKey == DbConstants.AcademicTerm.Semester)
                {
                    StartYear = ++StartYear;
                }
            }
            else
            {
                if (duration > 1)
                {
                    duration = StartYear;
                }
            }


            if (duration < 1)
            {
                model.AdmittedYear.Add(new SelectListModel
                {
                    RowKey = 1,
                    Text = "Short Term"
                });
            }
            else
            {
                for (int i = StartYear; i <= duration; i++)
                {
                    model.AdmittedYear.Add(new SelectListModel
                    {
                        RowKey = i,
                        Text = CommonUtilities.GetYearDescriptionByCode(i, model.AcademicTermKey ?? 0)
                    });
                }
            }

            return model.AdmittedYear;
        }

        public void FillMedium(TestPaperViewModel model)
        {

            model.Mediums = dbContext.VwMediumSelectActiveOnlies.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.MediumName
            }).ToList();
        }

        public void FillSecondLanguage(TestPaperViewModel model)
        {

            model.SecondLanguages = dbContext.SecondLanguages.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.SecondLanguageName
            }).ToList();
        }

        public void FillBatches(TestPaperViewModel model)
        {

            model.Batches = dbContext.Batches.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BatchName
            }).ToList();
        }


        public void FillMode(TestPaperViewModel model)
        {

            model.Modes = dbContext.Modes.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ModeName
            }).ToList();
        }
        public void FillClassMode(TestPaperViewModel model)
        {

            model.ClassModes = dbContext.ClassModes.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ClassModeName
            }).ToList();
        }



        public void FillClassDetails(TestPaperViewModel model)
        {
            IQueryable<TestPaperFilter> selectList = null;
            selectList = dbContext.VwClassDetailsSelectActiveOnlies.Select(row => new TestPaperFilter
            {
                RowKey = row.RowKey,
                Text = row.ClassCode,
                AcademicTermKey = row.AcademicTermKey,
                CourseTypeKey = row.CourseTypeKey,
                CourseKey = row.CourseKey,
                UniversityMasterKey = row.UniversityMasterKey,
                StudentYear=row.StudentYear
            });

            //if (model.SubjectKey != 0)
            //{
            //    selectList = selectList.Where(x => x.SubjectKey == model.SubjectKey);
            //}

            if (model.AcademicTermsKeys.Count > 0)
            {
                selectList = selectList.Where(x => model.AcademicTermsKeys.Contains(x.AcademicTermKey));
            
            }

            if (model.CourseTypeKeys.Count > 0)
            {
                selectList = selectList.Where(x => model.CourseTypeKeys.Contains(x.CourseTypeKey));
             
            }

            if (model.CourseKeys.Count > 0)
            {
                selectList = selectList.Where(x => model.CourseKeys.Contains(x.CourseKey));
              
            }

            if (model.UniversityMasterKeys.Count > 0)
            {
                selectList = selectList.Where(x => model.UniversityMasterKeys.Contains(x.UniversityMasterKey));
            
            }


            model.ClassCodes = selectList.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.Text

            }).GroupBy(row => row.RowKey).Select(row => row.FirstOrDefault()).ToList();

        }

        public void FillBranches(TestPaperViewModel model)
        {

            model.Branches = dbContext.Branches.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BranchName
            }).ToList();
        }


        public void FillCourseType(TestPaperViewModel model)
        {
            IQueryable<SelectListModel> selectList = null;


        }

            public List<TestPaperViewModel> GetTestPapers(string searchText)
        {
            try
            {

                List<TestPaperViewModel> TestPaperList = dbContext.VwTestPapers.Where(row => row.SubjectName.Contains(searchText))
                    .Select(row => new TestPaperViewModel
                {
                    RowKey = row.RowKey,
                    TestPaperName = row.TestPaperName,                    
                    TestDuration =row.TestDuration,
                    ExamDateTime=row.ExamDate,
                    SubjectName=row.SubjectName,
                    

                }).ToList();


                return TestPaperList;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.TestPaper, ActionConstants.MenuAccess, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return new List<TestPaperViewModel>();

            }

        }

        public TestPaperViewModel GetQuestionPaperById(TestPaperViewModel model)
        {
            try
            {
               
                TestPaperViewModel objViewModel = new TestPaperViewModel();
                if (model.TestSectionKey == null)
                {
                    model.TestSectionKey = dbContext.TestSections.Where(row => row.TestModule.TestPaperKey == model.RowKey && row.TestModule.ModuleKey == model.ModuleKey).Select(row => row.RowKey).FirstOrDefault();
                }

                if (model.TestSectionKey != 0)
                {
                    objViewModel = dbContext.TestSections.Where(row => row.TestModule.TestPaperKey == model.RowKey && row.TestModule.ModuleKey == model.ModuleKey).Select(row => new TestPaperViewModel
                    {
                        RowKey = row.TestModule.TestPaperKey,
                        TestPaperName = row.TestModule.TestPaper.TestPaperName,
                        SubjectName=row.TestModule.TestPaper.Subject.SubjectName,
                        SupportedFileName = row.SupportFile,
                        QuestionPaperFileName = row.QuestionPaperFile,
                        TestSectionKey = row.RowKey,
                        TestModuleKey = row.TestModuleKey,
                        ExamTypeKey = row.TestModule.TestPaper.ExamTypeKey,
                        //FromDate = row.TestModule.TestPaper.FromDate,
                        //ToDate = row.TestModule.TestPaper.ToDate,
                        ExamDate = row.TestModule.TestPaper.ExamDate,
                        ExamTime = DbFunctions.CreateTime(row.TestModule.TestPaper.ExamDate.Value.Hour, row.TestModule.TestPaper.ExamDate.Value.Minute, row.TestModule.TestPaper.ExamDate.Value.Second),
                        TestDuration = row.TestModule.TestDuration,
                        IsActive = row.TestModule.TestPaper.IsActive ?? false,
                        PlanKeyText = row.TestModule.TestPaper.PlanKeys,
                        //SubjectKey = row.TestModule.TestPaper.SubjectKey ?? 0,
                        TestInstructionFileName= UrlConstants.TestInstructionFileUrl + row.TestModule.TestPaper.InstructionFile,
                        ModuleKey=row.TestModule.ModuleKey,
                        SubjectKeys_=row.TestModule.TestPaper.SubjectKeys,
                        AcademicTermsKeys_ = row.TestModule.TestPaper.AcademicTermKeys,
                        CourseTypeKeys_ = row.TestModule.TestPaper.CourseTypeKeys,
                        CourseKeys_ = row.TestModule.TestPaper.CourseKeys,
                        UniversityMasterKeys_ = row.TestModule.TestPaper.UniversityKeys,
                        MeadiumKeys_ = row.TestModule.TestPaper.MeadiumKeys,
                        SecondLanguageKeys_ = row.TestModule.TestPaper.SecondLanguageKeys,
                        BatchKeys_ = row.TestModule.TestPaper.BatchKeys,
                        ModeKeys_ = row.TestModule.TestPaper.ModeKeys,
                        CourseYearsKeys_ = row.TestModule.TestPaper.StartYearKeys,
                        ClassModeKeys_ = row.TestModule.TestPaper.ClassModeKeys,
                        ClassCodeKeys_ = row.TestModule.TestPaper.ClassCodeKeys,
                        BranchKeys_ = row.TestModule.TestPaper.BranchKeys,
                        TestQuestions = dbContext.TestQuestions.Where(x => x.TestSection.TestModule.TestPaperKey == model.RowKey).Select(x => new TestQuestionViewModel
                        {
                            RowKey = x.RowKey,
                            QuestionNumber = x.QuestionNumber,
                            QuestionTypeKey = x.QuestionTypeKey,
                            QuestionPaper=x.TestSection.QuestionContent,
                            TestSectionKey=x.TestSection.RowKey,
                            Mark=x.Mark,
                            NegativeMark=x.NegativeMark,
                            MarkGroupKey=x.MarkGroupKey,
                            QuestionDuration=x.QuestionDuration,
                            QuestionPaperFileName=x.TestSection.QuestionPaperFile,
                        
                          
                            QuestionOptions = x.TestSection.TestQuestionOptions.Select
                                 (m => new TestQuestionOptionsViewModel
                                 {
                                     OptionText=m.OptionText,
                                     TestSectionKey=m.TestSectionKey,
                                     OptionValue=m.OptionValue,
                                     RowKey=m.RowKey,
                                     IsAnswerKey=m.OptionValue==x.AnswerKey?true:false
                                    
                                 } ).ToList()
                           

                        }).ToList()


                    }).FirstOrDefault();


                    foreach(var item in objViewModel.TestQuestions)
                    {
                        if(item.QuestionOptions==null || item.QuestionOptions.Count == 0)
                        {
                            TestQuestionOptionsViewModel qo = new TestQuestionOptionsViewModel();
                            item.QuestionOptions = new List<TestQuestionOptionsViewModel>();
                            item.QuestionTypeKey = DbConstants.QuestionType.Optional;
                            item.QuestionOptions.Add(qo);
                        }
                    }

                }
                else
                {
                    objViewModel = dbContext.TestSections.Where(row => row.TestModule.TestPaperKey == model.RowKey && row.TestModule.ModuleKey == model.ModuleKey).Select(row => new TestPaperViewModel
                    {
                        RowKey = row.TestModule.TestPaperKey,
                        TestPaperName = row.TestModule.TestPaper.TestPaperName,
                        SubjectName = row.TestModule.TestPaper.Subject.SubjectName,
                        SupportedFileName = row.SupportFile,
                        TestSectionKey = 0,
                        TestModuleKey = row.TestModuleKey,
                        //FromDate = row.TestModule.TestPaper.FromDate,
                        //ToDate = row.TestModule.TestPaper.ToDate,
                        ExamDate = row.TestModule.TestPaper.ExamDate,
                        ExamTime = DbFunctions.CreateTime(row.TestModule.TestPaper.ExamDate.Value.Hour, row.TestModule.TestPaper.ExamDate.Value.Minute, row.TestModule.TestPaper.ExamDate.Value.Second),
                        TestDuration = row.TestModule.TestDuration,
                        IsActive = row.TestModule.TestPaper.IsActive ?? false,
                        PlanKeyText = row.TestModule.TestPaper.PlanKeys,
                        //SubjectKey = row.TestModule.TestPaper.SubjectKey ?? 0,
                        QuestionPaper = row.QuestionContent,
                        TestInstructionFileName = UrlConstants.TestInstructionFileUrl + row.TestModule.TestPaper.InstructionFile,
                        QuestionPaperFileName = row.QuestionPaperFile,
                        ModuleKey = row.TestModule.ModuleKey,
                        SubjectKeys_ = row.TestModule.TestPaper.SubjectKeys,
                        AcademicTermsKeys_ = row.TestModule.TestPaper.AcademicTermKeys,
                        CourseTypeKeys_ = row.TestModule.TestPaper.CourseTypeKeys,
                        CourseKeys_ = row.TestModule.TestPaper.CourseKeys,
                        UniversityMasterKeys_=row.TestModule.TestPaper.UniversityKeys,
                        MeadiumKeys_=row.TestModule.TestPaper.MeadiumKeys,
                        SecondLanguageKeys_= row.TestModule.TestPaper.SecondLanguageKeys,
                        BatchKeys_ = row.TestModule.TestPaper.BatchKeys,
                        ModeKeys_ = row.TestModule.TestPaper.ModeKeys,
                        CourseYearsKeys_= row.TestModule.TestPaper.StartYearKeys,
                        ClassModeKeys_ = row.TestModule.TestPaper.ClassModeKeys,
                        ClassCodeKeys_ = row.TestModule.TestPaper.ClassCodeKeys,
                        BranchKeys_= row.TestModule.TestPaper.BranchKeys,
                        //MarkGroupKey = row.TestQuestions.Select(x => x.MarkGroupKey).FirstOrDefault(),



                    }).FirstOrDefault();
                }


                if (objViewModel == null)
                {
                    objViewModel = dbContext.TestPapers.Where(row => row.RowKey == model.RowKey).Select(row => new TestPaperViewModel
                    {
                        RowKey = row.RowKey,
                        TestPaperName = row.TestPaperName,
                        SubjectName=row.Subject.SubjectName,
                        ExamDate=row.ExamDate,
                        ExamTime= DbFunctions.CreateTime(row.ExamDate.Value.Hour,row.ExamDate.Value.Minute,row.ExamDate.Value.Second),
                        PlanKeyText = row.PlanKeys,
                        TestModuleKey = row.TestModules.Where(x => x.ModuleKey == model.ModuleKey).Select(x => x.RowKey).FirstOrDefault()
                    }).FirstOrDefault();

                    if (objViewModel == null)
                    {
                        objViewModel = new TestPaperViewModel { ModuleKey = model.ModuleKey };

                        TestQuestionViewModel tq = new TestQuestionViewModel();
                        objViewModel.TestQuestions = new List<TestQuestionViewModel>();
                        tq.QuestionTypeKey = DbConstants.QuestionType.Optional;
                        tq.QuestionNumber = 1;
                        objViewModel.TestQuestions.Add(tq);

                        foreach (var item in objViewModel.TestQuestions)
                        {
                            if (item.QuestionOptions.Count == 0)
                            {
                                TestQuestionOptionsViewModel qo = new TestQuestionOptionsViewModel();
                                item.QuestionOptions = new List<TestQuestionOptionsViewModel>();
                                item.QuestionOptions.Add(qo);
                            }
                        }

                    }
                    FillBranches(objViewModel);
                }
                else
                {
                    objViewModel.SubjectKeys = objViewModel.SubjectKeys_ != null ? objViewModel.SubjectKeys_.Split(',').Select(long.Parse).ToList() : objViewModel.SubjectKeys;
                    objViewModel.AcademicTermsKeys = objViewModel.AcademicTermsKeys_ != null ? objViewModel.AcademicTermsKeys_.Split(',').Select(long.Parse).ToList() : objViewModel.AcademicTermsKeys;
                    objViewModel.CourseTypeKeys = objViewModel.CourseTypeKeys_ != null ? objViewModel.CourseTypeKeys_.Split(',').Select(long.Parse).ToList() : objViewModel.CourseTypeKeys;
                    objViewModel.CourseKeys = objViewModel.CourseKeys_ != null ? objViewModel.CourseKeys_.Split(',').Select(long.Parse).ToList() : objViewModel.CourseKeys;
                    objViewModel.UniversityMasterKeys = objViewModel.UniversityMasterKeys_ != null ? objViewModel.UniversityMasterKeys_.Split(',').Select(long.Parse).ToList() : objViewModel.UniversityMasterKeys;
                    objViewModel.MeadiumKeys = objViewModel.MeadiumKeys_ != null ? objViewModel.MeadiumKeys_.Split(',').Select(long.Parse).ToList() : objViewModel.MeadiumKeys;
                    objViewModel.SecondLanguageKeys = objViewModel.SecondLanguageKeys_ != null ? objViewModel.SecondLanguageKeys_.Split(',').Select(long.Parse).ToList() : objViewModel.SecondLanguageKeys;
                    objViewModel.BatchKeys = objViewModel.BatchKeys_ != null ? objViewModel.BatchKeys_.Split(',').Select(long.Parse).ToList() : objViewModel.BatchKeys;
                    objViewModel.ModeKeys = objViewModel.ModeKeys_ != null ? objViewModel.ModeKeys_.Split(',').Select(long.Parse).ToList() : objViewModel.ModeKeys;
                    objViewModel.CourseYearsKeys = objViewModel.CourseYearsKeys_ != null ? objViewModel.CourseYearsKeys_.Split(',').Select(long.Parse).ToList() : objViewModel.CourseYearsKeys;
                    objViewModel.ClassModeKeys = objViewModel.ClassModeKeys_ != null ? objViewModel.ClassModeKeys_.Split(',').Select(long.Parse).ToList() : objViewModel.ClassModeKeys;
                    objViewModel.ClassCodeKeys = objViewModel.ClassCodeKeys_ != null ? objViewModel.ClassCodeKeys_.Split(',').Select(long.Parse).ToList() : objViewModel.ClassCodeKeys;
                    objViewModel.BranchKeys = objViewModel.BranchKeys_ != null ? objViewModel.BranchKeys_.Split(',').Select(long.Parse).ToList() : objViewModel.BranchKeys;
                    FillDynamicDropDownlists(objViewModel);
                }
                objViewModel.LastQuestionNumber = dbContext.TestQuestions.Where(row => row.TestSection.TestModule.TestPaperKey == model.RowKey).OrderByDescending(row => row.QuestionNumber).Select(row => row.QuestionNumber).FirstOrDefault();

                if (!String.IsNullOrEmpty(objViewModel.PlanKeyText))
                {
                    objViewModel.PlanKeys = objViewModel.PlanKeyText.Split(',').Select(Byte.Parse).ToList();
                }
                if (objViewModel.TestDuration != null)
                {
                    objViewModel.ExamDuration = convertToFullTime(objViewModel.TestDuration ?? 0);
                }


               
            
                return objViewModel;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.TestPaper, (model.RowKey != 0 ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return new TestPaperViewModel();

            }

        }

        private TimeSpan convertToFullTime(int Minuts)
        {
           
            var result = TimeSpan.FromMinutes(Minuts);
            return result;
        }

        public TestPaperViewModel CreateTestPaper(TestPaperViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    long Maxkey = dbContext.TestPapers.Select(x => x.RowKey).DefaultIfEmpty().Max();


                    TestPaper TestPaperModel = new TestPaper();
                    TestPaperModel.RowKey = ++Maxkey;

                    TestPaperModel.TestPaperName = model.TestPaperName;
                    TestPaperModel.InstructionFile = TestPaperModel.RowKey + ".html";
                    model.TestInstructionFileName = TestPaperModel.InstructionFile;
                    TestPaperModel.ExamTypeKey = model.ExamTypeKey;

                    #region Filters
                    if (model.ExamTypeKey == DbConstants.ExamTypes.NormalExam)
                    {
                        TestPaperModel.SubjectKeys = model.SubjectKeys.Count == 0 ? null : String.Join(",", model.SubjectKeys);
                    }
                    else
                    {
                        TestPaperModel.SubjectKeys = null;
                    }
                    TestPaperModel.AcademicTermKeys = model.AcademicTermsKeys.Count == 0 ? null : String.Join(",", model.AcademicTermsKeys);
                    TestPaperModel.CourseTypeKeys = model.CourseTypeKeys.Count == 0 ? null : String.Join(",", model.CourseTypeKeys);
                    TestPaperModel.CourseKeys = model.CourseKeys.Count == 0 ? null : String.Join(",", model.CourseKeys);
                    TestPaperModel.UniversityKeys = model.UniversityMasterKeys.Count == 0 ? null : String.Join(",", model.UniversityMasterKeys);
                    TestPaperModel.MeadiumKeys = model.Mediums.Count == 0 ? null : String.Join(",", model.MeadiumKeys);
                    TestPaperModel.SecondLanguageKeys = model.SecondLanguageKeys.Count == 0 ? null : String.Join(",", model.SecondLanguageKeys);
                    TestPaperModel.BatchKeys = model.BatchKeys.Count == 0 ? null : String.Join(",", model.BatchKeys);
                    TestPaperModel.ModeKeys = model.ModeKeys.Count == 0 ? null : String.Join(",", model.ModeKeys);
                    TestPaperModel.StartYearKeys = model.CourseYearsKeys.Count == 0 ? null : String.Join(",", model.CourseYearsKeys);
                    TestPaperModel.ClassModeKeys = model.ClassModeKeys.Count == 0 ? null : String.Join(",", model.ClassModeKeys);
                    TestPaperModel.ClassCodeKeys = model.ClassCodeKeys.Count == 0 ? null : String.Join(",", model.ClassCodeKeys);
                    TestPaperModel.BranchKeys = model.BranchKeys.Count == 0 ? null : String.Join(",", model.BranchKeys);
                    #endregion

                    //TestPaperModel.FromDate = model.FromDate;
                    //TestPaperModel.ToDate = model.ToDate;
                    if (model.ExamTime != null)
                    {
                        TestPaperModel.ExamDate = model.ExamDate.Value.Add(model.ExamTime.Value);
                    }
                    else
                    {
                        TestPaperModel.ExamDate = model.ExamDate;
                    }
                    //TestPaperModel.SubjectKey = model.SubjectKey;
                    TestPaperModel.IsActive = model.IsActive;
                    model.RowKey = TestPaperModel.RowKey;
                    dbContext.TestPapers.Add(TestPaperModel);
                    dbContext.SaveChanges();
                    //TestPaperModel.PlanKeys = String.Join(",", model.PlanKeys);
                    CreateTestModule(model, TestPaperModel.RowKey);
                    CreateQuestionOptions(model);
                    
                
                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.TestPaper, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    //model.ExceptionMessage = ex.GetBaseException().Message;
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.TestPaper);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.TestPaper, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }

                return model;



            }
        }
        public TestPaperViewModel UpdateTestPaper(TestPaperViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    TestPaper TestPaperModel = dbContext.TestPapers.SingleOrDefault(x => x.RowKey == model.RowKey);
                    TestPaperModel.TestPaperName = model.TestPaperName;
                    //TestPaperModel.FromDate = model.FromDate;
                    //TestPaperModel.ToDate = model.ToDate;
                    //TestPaperModel.SubjectKey = model.SubjectKey;
                    TestPaperModel.IsActive = model.IsActive;
                    TestPaperModel.InstructionFile = TestPaperModel.RowKey + ".html";
                    model.TestInstructionFileName = TestPaperModel.InstructionFile;
                    TestPaperModel.ExamTypeKey = model.ExamTypeKey;

                    #region Filters
                    if (model.ExamTypeKey == DbConstants.ExamTypes.NormalExam)
                    {
                        TestPaperModel.SubjectKeys = model.SubjectKeys.Count == 0 ? null : String.Join(",", model.SubjectKeys);
                    }
                    else
                    {
                        TestPaperModel.SubjectKeys = null;
                    }
                    TestPaperModel.AcademicTermKeys = model.AcademicTermsKeys.Count == 0 ? null : String.Join(",", model.AcademicTermsKeys);
                    TestPaperModel.CourseTypeKeys = model.CourseTypeKeys.Count == 0 ? null : String.Join(",", model.CourseTypeKeys);
                    TestPaperModel.CourseKeys = model.CourseKeys.Count == 0 ? null : String.Join(",", model.CourseKeys);
                    TestPaperModel.UniversityKeys = model.UniversityMasterKeys.Count == 0 ? null : String.Join(",", model.UniversityMasterKeys);
                    TestPaperModel.MeadiumKeys = model.MeadiumKeys.Count == 0 ? null : String.Join(",", model.MeadiumKeys);
                    TestPaperModel.SecondLanguageKeys = model.SecondLanguageKeys.Count == 0 ? null : String.Join(",", model.SecondLanguageKeys);
                    TestPaperModel.BatchKeys = model.BatchKeys.Count == 0 ? null : String.Join(",", model.BatchKeys);
                    TestPaperModel.ModeKeys = model.ModeKeys.Count == 0 ? null : String.Join(",", model.ModeKeys);
                    TestPaperModel.StartYearKeys = model.CourseYearsKeys.Count == 0 ? null : String.Join(",", model.CourseYearsKeys);
                    TestPaperModel.ClassModeKeys = model.ClassModeKeys.Count == 0 ? null : String.Join(",", model.ClassModeKeys);
                    TestPaperModel.ClassCodeKeys = model.ClassCodeKeys.Count == 0 ? null : String.Join(",", model.ClassCodeKeys);
                    TestPaperModel.BranchKeys = model.BranchKeys.Count == 0 ? null : String.Join(",", model.BranchKeys);
                    #endregion


                    if (model.ExamTime != null)
                    {
                        TestPaperModel.ExamDate = model.ExamDate.Value.Add(model.ExamTime.Value);
                    }
                    else
                    {
                        TestPaperModel.ExamDate = model.ExamDate;
                    }

                    //TestPaperModel.PlanKeys = String.Join(",", model.PlanKeys);
                    if ((model.TestModuleKey ?? 0) == 0)
                    {
                        CreateTestModule(model, TestPaperModel.RowKey);
                    }
                    else
                    {
                       
                        UpdateTestModule(model);
                    }
                    CreateQuestionOptions(model);

                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.TestPaper, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    //model.ExceptionMessage = ex.GetBaseException().Message;
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.TestPaper);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.TestPaper, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }

                return model;
            }
        }

        
        private void CreateTestModule(TestPaperViewModel model, long TestPaperKey)
        {
            TestModule TestModuleModel = new TestModule();
            long Maxkey = dbContext.TestModules.Select(x => x.RowKey).DefaultIfEmpty().Max();

            TestModuleModel.RowKey = ++Maxkey;
           
            TestModuleModel.TestPaperKey = TestPaperKey;
            TestModuleModel.ModuleKey = model.ModuleKey;
            TestModuleModel.TestDuration =(ConvertToMin(model.ExamDuration.Value.Hours)) +model.ExamDuration.Value.Minutes;
            dbContext.TestModules.Add(TestModuleModel);
            model.TestModuleKey = TestModuleModel.RowKey;
            CreateTestSection(model, TestModuleModel.RowKey);

        }

        public int ConvertToMin(int Hours)
        {
            return Hours * 60;
        }
        private void UpdateTestModule(TestPaperViewModel model)
        {
            TestModule TestModuleModel = dbContext.TestModules.SingleOrDefault(x => x.RowKey == model.TestModuleKey);
            TestModuleModel.TestDuration = (ConvertToMin(model.ExamDuration.Value.Hours)) + model.ExamDuration.Value.Minutes;

            if (model.RowKey == 0)
            {
                CreateTestSection(model, TestModuleModel.RowKey);
            }
            else
            {
                CreateTestSection(model, TestModuleModel.RowKey);
                UpdateTestSection(model);
            }
           

            //if ((model.TestSectionKey ?? 0) == 0)
            //{
            //    CreateTestSection(model, TestModuleModel.RowKey);

            //}
            //else
            //{
            //    UpdateTestSection(model);
            //}



        }


        private void CreateTestSection(TestPaperViewModel model, long TestModuleKey)
        {
           
            long Maxkey = dbContext.TestSections.Select(x => x.RowKey).DefaultIfEmpty().Max();
            long TestQuestionMaxkey = dbContext.TestQuestions.Select(x => x.RowKey).DefaultIfEmpty().Max();
            foreach (var item in model.TestQuestions.Where(x=>x.RowKey==0))
            {
                TestSection TestSectionModel = new TestSection();

                TestSectionModel.RowKey = (Maxkey+1);
                item.QuestionPaperFileName = TestSectionModel.RowKey.ToString() + ".html";
                if (model.SupportedFilePath != null)
                    model.SupportedFileName = model.SectionNumber.ToString() + model.SupportedFileName;

                TestSectionModel.SectionName = model.TestSectionName;
                TestSectionModel.TestModuleKey = TestModuleKey;
                TestSectionModel.QuestionPaperFile = item.QuestionPaperFileName;
                TestSectionModel.SupportFile = model.SupportedFileName;
                //TestSectionModel.QuestionContent = item.QuestionPaper;
                dbContext.TestSections.Add(TestSectionModel);
                model.TestSectionKey = TestSectionModel.RowKey;
                item.TestSectionKey = TestSectionModel.RowKey;
               
                CreateTestQuestions(model.TestQuestions.Where(row => row.RowKey == 0 && row.QuestionNumber==item.QuestionNumber).ToList(), TestSectionModel.RowKey, model,ref TestQuestionMaxkey);
                
                Maxkey++;
            }

        }
        private void UpdateTestSection(TestPaperViewModel model)
        {
            long TestQuestionMaxkey = dbContext.TestQuestions.Select(x => x.RowKey).DefaultIfEmpty().Max();
            foreach (var item in model.TestQuestions.Where(x=>x.RowKey>0))
            {
                TestSection TestSectionModel = dbContext.TestSections.SingleOrDefault(x => x.RowKey == item.TestSectionKey);

                item.QuestionPaperFileName = TestSectionModel.QuestionPaperFile;
            if (model.SupportedFilePath != null)
            {
                model.SupportedFileName = model.SectionNumber.ToString() + model.SupportedFileName;
                TestSectionModel.SupportFile = model.SupportedFileName;
            }
                //TestSectionModel.QuestionContent = item.QuestionPaper;
                item.TestSectionKey = TestSectionModel.RowKey;
             
                UpdateTestQuestions(model.TestQuestions.Where(row => row.RowKey != 0).ToList(), model);
                //CreateTestQuestions(model.TestQuestions.Where(row => row.RowKey == 0).ToList(), item.TestSectionKey ?? 0, model, TestQuestionMaxkey);
            
            }
        }

        private void CreateTestQuestions(List<TestQuestionViewModel> modelList, long TestSectionKey, TestPaperViewModel TestModel, ref long TestQuestionMaxkey)
        {
         
          
            foreach (TestQuestionViewModel model in modelList)
            {
                TestQuestion TestQuestionModel = new TestQuestion();
                TestQuestionModel.RowKey = (TestQuestionMaxkey+1);
                TestQuestionModel.TestSectionKey = TestSectionKey;
                TestQuestionModel.QuestionTypeKey = DbConstants.QuestionType.Optional;
                TestQuestionModel.QuestionNumber = model.QuestionNumber;
                TestQuestionModel.Mark = model.Mark;
                TestQuestionModel.QuestionDuration = model.QuestionDuration;
                TestQuestionModel.NegativeMark = model.NegativeMark??0;
                TestQuestionModel.Mark = model.Mark;
                TestQuestionModel.MarkGroupKey = model.MarkGroupKey;
                TestQuestionModel.AnswerKey = model.QuestionOptions.Where(x => x.IsAnswerKey == true).Select(x => x.OptionValue).FirstOrDefault();
                dbContext.TestQuestions.Add(TestQuestionModel);
                TestQuestionMaxkey++;
  
            }
        }
        private void UpdateTestQuestions(List<TestQuestionViewModel> modelList, TestPaperViewModel TestModel)
        {
            foreach (TestQuestionViewModel model in modelList)
            {
                TestQuestion TestQuestionModel = dbContext.TestQuestions.SingleOrDefault(x => x.RowKey == model.RowKey);
                TestQuestionModel.QuestionTypeKey = DbConstants.QuestionType.Optional;
                TestQuestionModel.QuestionNumber = model.QuestionNumber;
                TestQuestionModel.QuestionDuration = model.QuestionDuration;
                TestQuestionModel.Mark = model.Mark;
                TestQuestionModel.NegativeMark = model.NegativeMark??0;
                TestQuestionModel.Mark = model.Mark;
                TestQuestionModel.MarkGroupKey = model.MarkGroupKey;
                TestQuestionModel.AnswerKey = model.QuestionOptions.Where(x => x.IsAnswerKey == true).Select(x => x.OptionValue).FirstOrDefault();
            }
        }

        private void CreateQuestionOptions(TestPaperViewModel Model)
        {

            long Maxkey = dbContext.TestQuestionOptions.Select(x => x.RowKey).DefaultIfEmpty().Max();

            foreach (var modal in Model.TestQuestions)
            {

                foreach (var option in modal.QuestionOptions.Where(x => x.RowKey == 0))
                {
                    TestQuestionOption dbQuestionOption = new TestQuestionOption();
                    dbQuestionOption.RowKey = (Maxkey + 1);
                    dbQuestionOption.TestSectionKey = modal.TestSectionKey ?? 0;                 
                    dbQuestionOption.OptionValue = option.OptionValue;
                    dbQuestionOption.OptionText = option.OptionText;
                    dbContext.TestQuestionOptions.Add(dbQuestionOption);
                    Maxkey++;
                }

                foreach (var option in modal.QuestionOptions.Where(x => x.RowKey > 0))
                {
                    TestQuestionOption dbQuestionOption = dbContext.TestQuestionOptions.Where(x => x.RowKey == option.RowKey).SingleOrDefault();             
                    dbQuestionOption.TestSectionKey = modal.TestSectionKey ?? 0;
                    dbQuestionOption.OptionValue = option.OptionValue;
                    dbQuestionOption.OptionText = option.OptionText;
                
                }

            }



        }




        public TestPaperViewModel DeleteTestPaper(TestPaperViewModel model)
        {


            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    TestPaper testPaper = dbContext.TestPapers.SingleOrDefault(row => row.RowKey == model.RowKey);

                    List<TestQuestion> testQuestionList = dbContext.TestQuestions.Where(row => row.TestSection.TestModule.TestPaperKey == model.RowKey).ToList();
                    List<TestModule> testModuleList = dbContext.TestModules.Where(row => row.TestPaperKey == model.RowKey).ToList();
                    List<TestSection> testSectionList = dbContext.TestSections.Where(row => row.TestModule.TestPaperKey == model.RowKey).ToList();
                    List<TestQuestionOption> TestQuestionOptionList = dbContext.TestQuestionOptions.Where(row => row.TestSection.TestModule.TestPaperKey == model.RowKey).ToList();

                    dbContext.TestQuestions.RemoveRange(testQuestionList);                  
                    dbContext.TestModules.RemoveRange(testModuleList);                   
                    dbContext.TestSections.RemoveRange(testSectionList);
                    dbContext.TestQuestionOptions.RemoveRange(TestQuestionOptionList);
                    dbContext.TestPapers.Remove(testPaper);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.TestPaper, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.TestPaper);
                        model.IsSuccessful = false;
                    }
                    ActivityLog.CreateActivityLog(MenuConstants.TestPaper, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.TestPaper);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.TestPaper, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public TestPaperViewModel DeleteTestSection(TestPaperViewModel model)
        {


            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    TestSection testSection = dbContext.TestSections.SingleOrDefault(row => row.RowKey == model.RowKey);
                    List<TestQuestion> testQuestionList = dbContext.TestQuestions.Where(row => row.TestSectionKey == model.RowKey).ToList();
                    dbContext.TestQuestions.RemoveRange(testQuestionList);

                    dbContext.TestSections.Remove(testSection);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    //ActivityLog.CreateActivityLog(MenuConstants.TestPaper, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.TestPaper);
                        model.IsSuccessful = false;
                    }
                    //ActivityLog.CreateActivityLog(MenuConstants.TestPaper, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.TestPaper);
                    model.IsSuccessful = false;
                    //ActivityLog.CreateActivityLog(MenuConstants.TestPaper, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public TestPaperViewModel DeleteQuestion(TestPaperViewModel model)
        {


            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    TestQuestion testQuestion = dbContext.TestQuestions.SingleOrDefault(row => row.RowKey == model.RowKey);
                    List<TestSection> testsection = dbContext.TestSections.Where(row => row.RowKey == testQuestion.TestSectionKey).ToList();         
                    List<TestQuestionOption> testQuestionOptions = dbContext.TestQuestionOptions.Where(x => x.TestSection.TestModule.TestPaperKey == testQuestion.TestSection.TestModule.TestPaperKey && x.TestSectionKey==testQuestion.TestSectionKey).ToList();              
                    dbContext.TestQuestionOptions.RemoveRange(testQuestionOptions);
                    dbContext.TestSections.RemoveRange(testsection);
                    dbContext.TestQuestions.Remove(testQuestion);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.TestPaper, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.TestPaper);
                        model.IsSuccessful = false;
                    }
                    ActivityLog.CreateActivityLog(MenuConstants.TestPaper, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.TestPaper);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.TestPaper, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }


        public TestPaperViewModel DeleteQuestionOption(TestPaperViewModel model)
        {


            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    TestQuestionOption testQuestionOptions = dbContext.TestQuestionOptions.Where(x => x.RowKey == model.RowKey).SingleOrDefault();            
                    dbContext.TestQuestionOptions.Remove(testQuestionOptions);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.TestPaperOption, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.TestPaper);
                        model.IsSuccessful = false;
                    }
                    ActivityLog.CreateActivityLog(MenuConstants.TestPaperOption, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.TestPaper);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.TestPaper, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public TestPaperViewModel GetTestInstructionById(long Id)
        {
            try
            {
                TestPaperViewModel model = dbContext.TestPapers.Where(row => row.RowKey == Id).Select(row => new TestPaperViewModel
                    {
                        RowKey = row.RowKey,
                        TestInstructionFileName = UrlConstants.TestInstructionFileUrl + row.InstructionFile

                    }).FirstOrDefault();
                if (model == null)
                {
                    model = new TestPaperViewModel();
                }
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.TestPaper, (Id != 0 ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return new TestPaperViewModel();

            }
        }
        public TestPaperViewModel UpdateTestInstruction(TestPaperViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    TestPaper TestPaperModel = dbContext.TestPapers.SingleOrDefault(x => x.RowKey == model.RowKey);
                    TestPaperModel.InstructionFile = TestPaperModel.RowKey + ".html";
                    model.TestInstructionFileName = TestPaperModel.InstructionFile;

                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    //ActivityLog.CreateActivityLog(MenuConstants.TesPaper, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    //model.ExceptionMessage = ex.GetBaseException().Message;
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.TestPaper);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.TestPaper, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }

                return model;
            }
        }

        public TestPaperViewModel GetTestAnswerKeyById(TestPaperViewModel model)
        {
            try
            {
                TestPaperViewModel objViewModel = new TestPaperViewModel();
                objViewModel = dbContext.TestPapers.Where(row => row.RowKey == model.RowKey).Select(row => new TestPaperViewModel
                {
                    RowKey = row.RowKey,
                    QuestionSections = dbContext.TestSections.Where(x => x.TestModule.TestPaperKey == row.RowKey && x.TestModule.ModuleKey == model.ModuleKey).Select(x => new TestSectionViewModel
                    {
                        RowKey = x.RowKey,
                        TestSectionName = x.SectionName,
                        TestSectionFileName = UrlConstants.TestQuestionPaperUrl + row.RowKey + "/" + x.TestModule.ModuleKey + "/" + x.QuestionPaperFile,
                        QuestionDetails = dbContext.TestQuestions.Where(y => y.TestSectionKey == x.RowKey).Select(y => new TestQuestionViewModel
                        {
                            QuestionNumber = y.QuestionNumber
                        }).ToList()
                    }).ToList()

                }).FirstOrDefault();
                if (objViewModel == null)
                {
                    objViewModel = new TestPaperViewModel();
                }
                objViewModel.QuestionModules = dbContext.TestQuestions.Where(x => x.TestSection.TestModule.TestPaperKey == model.RowKey && x.TestSection.TestModule.ModuleKey == model.ModuleKey).Select(x => new SelectListModel
                     {
                         RowKey = x.QuestionNumber,
                         Text = x.AnswerKey

                     }).ToList();
                return objViewModel;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.TestPaper, (model.RowKey != 0 ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return new TestPaperViewModel();

            }
        }
        public TestPaperViewModel UpdateTestAnswerKey(TestPaperViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    List<TestQuestion> TestPaperList = dbContext.TestQuestions.Where(x => x.TestSection.TestModule.TestPaperKey == model.RowKey && x.TestSection.TestModule.ModuleKey == model.ModuleKey).ToList();
                    TestPaperList.ForEach(TestPaper => TestPaper.AnswerKey = model.QuestionModules.Where(row => row.RowKey == TestPaper.QuestionNumber).Select(row => row.Text).FirstOrDefault());
                    dbContext.SaveChanges();



                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.TestPaper, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    //model.ExceptionMessage = ex.GetBaseException().Message;
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.TestPaper);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.TestPaper, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }

                return model;
            }
        }

   
        public bool CheckTestNameExists(string TestPaperName, long RowKey)
        {
            return !dbContext.TestPapers.Where(row => row.RowKey != RowKey && row.TestPaperName == TestPaperName).Any();
        }



        public string GetFileTypeById(byte id)
        {
            return dbContext.Modules.Where(row => row.RowKey == id).Select(row => row.SupportFileType).FirstOrDefault();
        }

        #region Valuation

        public dynamic GetExamValuations(string searchText, int PageIndex, int PageSize, out long TotalRecords)
        {
            try
            {
                var Take = PageSize;
                var Skip = (PageIndex - 1) * PageSize;
                var examList = dbContext.Applications.Where(row => row.StudentName.Contains(searchText) || row.StudentMobile.Contains(searchText) || row.StudentEmail.Contains(searchText));
                TotalRecords = examList.Count();
                return examList
                    //.OrderBy(row => row.TestPaperKey).Skip(Skip).Take(Take)
                    .Select(app => new
                {

                    ApplicantKey = app.RowKey,
                    ApplicantName = app.StudentName,
                    ExamTests = app.ExamTests.Select(test => new
                    {
                        TestKey = test.TestPaperKey,
                        TestName = test.TestPaper.Subject.SubjectName,
                       
                        ExamModules = test.ExamTestSections.Select(x => new
                        {
                            ExamTestSectionKey = test.RowKey,
                            ModuleKey = x.ModuleKey,
                            ModuleName = x.Module.ModuleName,
                            TotalScore = x.ExamTestAnswers.Select(y=>y.Marks??0).DefaultIfEmpty().Sum(),
                            Color = dbContext.BandStatus.Where(y => y.RowKey == x.ModuleKey && y.BandScore <= x.TotalMark).OrderByDescending(y => y.BandScore).Select(y => y.BandColor).FirstOrDefault()
                        }),
                    }).ToList()
                }).ToList();
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.ExamTest, ActionConstants.MenuAccess, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return new List<ExamTestViewModel>();

            }

        }

        public ExamTestViewModel GetExamValuationById(ExamTestViewModel model)
        {
            try
            {
                ExamTestViewModel objViewModel = new ExamTestViewModel();

                objViewModel = dbContext.ExamTestSections.Where(row => row.ExamTest.TestPaperKey == model.TestPaperKey && row.ModuleKey == model.ModuleKey && row.ExamTest.ApplicationKey == model.ApplicationKey).Select(row => new ExamTestViewModel
                {
                    RowKey = row.ExamTest.RowKey,
                    ApplicationKey = row.ExamTest.ApplicationKey,
                    ModuleKey = row.ModuleKey,
                    ApplicationUserKey = row.ExamTest.Application.AppUsers.Select(x => x.RowKey).FirstOrDefault(),
                    TotalScore = row.TotalMark,
                    TestPaperKey = row.ExamTest.TestPaperKey,
                    ExamTestSectionKey = row.RowKey,
                    TestPaperName = row.ExamTest.TestPaper.TestPaperName,
                    SubjectName=row.ExamTest.TestPaper.Subject.SubjectName,
                    TestSections = dbContext.TestSections.Where(x => x.TestModule.TestPaperKey == row.ExamTest.TestPaperKey && x.TestModule.ModuleKey == model.ModuleKey).Select(x => new TestSectionViewModel
                    {
                        RowKey = x.RowKey,
                        TestSectionName = x.SectionName,
                        TestSectionFileName = UrlConstants.TestQuestionPaperUrl + row.ExamTest.TestPaperKey + "/" + x.TestModule.ModuleKey + "/" + x.QuestionPaperFile,
                        SupportedFileName = UrlConstants.TestSupportFileUrl + row.ExamTest.TestPaperKey + "/" + x.TestModule.ModuleKey + "/" + x.SupportFile,
                        QuestionDetails = dbContext.TestQuestions.Where(y => y.TestSectionKey == x.RowKey).Select(y => new TestQuestionViewModel
                        {
                            QuestionNumber = y.QuestionNumber

                        }).ToList()

                    }).OrderBy(x => x.RowKey).ToList(),



                }).FirstOrDefault();
                if (objViewModel == null)
                {
                    objViewModel = dbContext.TestPapers.Where(row => row.RowKey == model.TestPaperKey).Select(row => new ExamTestViewModel
                    {
                        TestPaperKey = row.RowKey,
                        TestPaperName = row.TestPaperName,
                        SubjectName = row.Subject.SubjectName,
                        SupportedFileName = UrlConstants.TestSupportFileUrl + row.RowKey + "/" + dbContext.TestSections.Where(x => x.TestModule.TestPaperKey == row.RowKey && x.TestModule.ModuleKey == model.ModuleKey).Select(x => x.SupportFile).FirstOrDefault(),
                        TestSections = dbContext.TestSections.Where(x => x.TestModule.TestPaperKey == row.RowKey && x.TestModule.ModuleKey == model.ModuleKey).Select(x => new TestSectionViewModel
                        {
                            RowKey = x.RowKey,
                            TestSectionName = x.SectionName,
                            TestSectionFileName = UrlConstants.TestQuestionPaperUrl + row.RowKey + "/" + x.TestModule.ModuleKey + "/" + x.QuestionPaperFile,
                            SupportedFileName = UrlConstants.TestSupportFileUrl + row.RowKey + "/" + x.TestModule.ModuleKey + "/" + x.SupportFile,
                            QuestionDetails = dbContext.TestQuestions.Where(y => y.TestSectionKey == x.RowKey).Select(y => new TestQuestionViewModel
                            {
                                QuestionNumber = y.QuestionNumber

                            }).ToList()
                        }).OrderBy(x => x.RowKey).ToList()

                    }).FirstOrDefault();
                    if (objViewModel == null)
                        objViewModel = new ExamTestViewModel();


                }
                else
                {
                    foreach (TestSectionViewModel item in objViewModel.TestSections)
                    {
                        item.QuestionDetails = (from qd in dbContext.TestQuestions.Where(y => y.TestSectionKey == item.RowKey)
                                                join ets in dbContext.ExamTestAnswers.Where(row => row.ExamTestSection.ExamTestKey == objViewModel.RowKey && row.ExamTestSection.ModuleKey == model.ModuleKey)
                                                 on qd.QuestionNumber equals ets.QuestionNumber into etsj
                                                from ets in etsj.DefaultIfEmpty()
                                                select new TestQuestionViewModel
                                                {
                                                    QuestionNumber = qd.QuestionNumber,
                                                    AnswerKey = qd.AnswerKey,
                                                    AnswerText = ets.AnswerText,
                                                    IsCorrect = ets != null ? ets.IsCorrect : false,
                                                    Mark = ets.Marks
                                                }).ToList();
                    }
                }
                TestModule TestModule = dbContext.TestModules.Where(row => row.TestPaperKey == model.TestPaperKey && row.ModuleKey == model.ModuleKey).SingleOrDefault();
                if (TestModule != null)
                {
                    objViewModel.MaximumScore = TestModule.Module.MaximumBandScore ?? 0;
                }

                return objViewModel;
            }
            catch (Exception ex)
            {
                //ActivityLog.CreateActivityLog(MenuConstants.TestPaper, (model.RowKey != 0 ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return new ExamTestViewModel();

            }
        }
        public ExamTestViewModel UpdateExamResult(ExamTestViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    if (DbConstants.QuestionModule.AswerKeyModules.Contains(model.ModuleKey))
                    {
                        List<ExamTestAnswer> ExamTestAnswerList = dbContext.ExamTestAnswers.Where(x => x.ExamTestSection.ExamTest.ApplicationKey == model.ApplicationKey && x.ExamTestSection.ModuleKey == model.ModuleKey && x.ExamTestSection.ExamTest.TestPaperKey == model.TestPaperKey && x.AnswerText != null && x.Marks == null).ToList();
                        if (ExamTestAnswerList.Count > 0)
                        {
                            foreach (ExamTestAnswer ExamTestAnswer in ExamTestAnswerList)
                            {

                                List<string> AnswerKeys = new List<string>();
                                string AnswerKey = dbContext.TestQuestions.Where(row => row.QuestionNumber == ExamTestAnswer.QuestionNumber && row.TestSection.TestModule.ModuleKey == model.ModuleKey && row.TestSection.TestModule.TestPaperKey == model.TestPaperKey).Select(row => row.AnswerKey).FirstOrDefault();
                                if (!String.IsNullOrEmpty(AnswerKey))
                                {
                                    decimal Mark = dbContext.TestQuestions.Where(row => row.QuestionNumber == ExamTestAnswer.QuestionNumber && row.TestSection.TestModule.TestPaperKey == model.TestPaperKey).Select(row => row.Mark ?? 0).FirstOrDefault();
                                    decimal NegativeMark = (-dbContext.TestQuestions.Where(row => row.QuestionNumber == ExamTestAnswer.QuestionNumber && row.TestSection.TestModule.TestPaperKey == model.TestPaperKey).Select(row => row.NegativeMark ?? 0).FirstOrDefault());
                                    AnswerKeys = AnswerKey.Split('|').Select(x => x.ToLower()).ToList();
                                    ExamTestAnswer.IsCorrect = ExamTestAnswer.AnswerText != null ? AnswerKeys.Contains(ExamTestAnswer.AnswerText.ToLower()) : false;
                                    ExamTestAnswer.Marks = ExamTestAnswer.AnswerText != null ? (decimal?)(ExamTestAnswer.IsCorrect ? Mark : (NegativeMark)) : null;
                                    ExamTestAnswer.AnswerKeyStatus = true;
                                }
                            }
                            //if (!ExamTestAnswerList.Any(x => x.Marks == null))
                            //{
                            //    int CorrectCount = ExamTestAnswerList.Where(row => row.IsCorrect).Count();
                            //    ExamTestSection ExamTestSectionModel = dbContext.ExamTestSections.Where(row => row.ExamTest.TestPaperKey == model.TestPaperKey && row.ModuleKey == model.ModuleKey && row.ExamTest.ApplicationKey == model.ApplicationKey).SingleOrDefault();
                            //    if (ExamTestSectionModel != null)
                            //    {
                            //        ExamTestSectionModel.TotalMark = dbContext.ModuleBands.Where(row => row.ModuleKey == model.ModuleKey && row.FromScore <= CorrectCount && (row.ToScore ?? CorrectCount) >= CorrectCount).Select(row => row.BandScore).FirstOrDefault();
                            //    }
                            //}
                        }
                        else
                        {
                            model.Message = EduSuiteUIResources.AlreadyUpdated;
                            model.IsSuccessful = false;
                        }
                    }
                    else
                    {
                        if (model.ExamAnswers.Count > 0)
                        {
                            foreach (ExamTestAnswerViewModel ExamTestAnswer in model.ExamAnswers)
                            {
                                ExamTestAnswer ExamTestAnswersModel = dbContext.ExamTestAnswers.Where(x => x.ExamTestSection.ExamTest.TestPaperKey == model.RowKey && x.ExamTestSection.ExamTest.ApplicationKey == model.ApplicationKey && x.ExamTestSection.ModuleKey == model.ModuleKey && x.QuestionNumber == ExamTestAnswer.QuestionNumber).SingleOrDefault();
                                ExamTestAnswersModel.Marks = ExamTestAnswer.TotalScore;
                            }
                            if (!model.ExamAnswers.Any(x => x.TotalScore == null))
                            {
                                ExamTestSection ExamTestSectionModel = dbContext.ExamTestSections.Where(row => row.ExamTest.TestPaperKey == model.TestPaperKey && row.ModuleKey == model.ModuleKey && row.ExamTest.ApplicationKey == model.ApplicationKey).SingleOrDefault();
                                if (ExamTestSectionModel != null)
                                {
                                    decimal BandScore = model.ExamAnswers.Select(row => row.TotalScore).DefaultIfEmpty().Sum() / model.ExamAnswers.Count;
                                    ExamTestSectionModel.TotalMark = Math.Round(BandScore * 2, MidpointRounding.AwayFromZero) / 2;
                                }
                            }
                        }
                        else
                        {
                            model.Message = EduSuiteUIResources.AlreadyUpdated;
                            model.IsSuccessful = false;
                        }
                    }

                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.TestValuation, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    //model.ExceptionMessage = ex.GetBaseException().Message;
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.TestPaper);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.TestValuation, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }

                return model;
            }
        }
        #endregion
    }
}