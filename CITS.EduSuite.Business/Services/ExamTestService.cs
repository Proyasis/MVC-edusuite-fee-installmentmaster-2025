using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class ExamTestService : IExamTestService
    {
        private EduSuiteDatabase dbContext;
        private long ExamAnswerMaxkey;
        public ExamTestService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
            ExamAnswerMaxkey = 0;
        }

        public List<ExamTestViewModel> GetExamTestsAll(ExamTestViewModel model, out long TotalRecords)
        {
            try
            {


                IQueryable<ExamTestViewModel> TestPaperResults = (from row in dbContext.ExamTests
                                                                  orderby row.RowKey descending
                                                                  select new ExamTestViewModel
                                                                  {
                                                                      RowKey = row.RowKey,
                                                                      ApplicantName = row.Application.StudentName,
                                                                      ApplicationKey = row.ApplicationKey,
                                                                      TestPaperKey = row.TestPaperKey,
                                                                      TestPaperName = row.TestPaper.TestPaperName,
                                                                      ExamStart = row.ExamTestSections.Select(x => x.StartTime).FirstOrDefault(),
                                                                      ExamEnd = row.ExamTestSections.Select(x => x.EndTime).FirstOrDefault(),
                                                                      ExamDuration = row.TestPaper.TestModules.Select(x => x.TestDuration ?? 0).FirstOrDefault(),
                                                                      ModuleKey = row.ExamTestSections.Select(x => x.ModuleKey).FirstOrDefault(),
                                                                      SubjectName = row.TestPaper.Subject.SubjectName
                                                                  });


                if (model.SearchText != "")
                {
                    TestPaperResults = TestPaperResults.Where(x => x.ApplicantName.Contains(model.SearchText));
                }

                TotalRecords = TestPaperResults.Count();

                if (model.SortBy != "")
                {
                    TestPaperResults = SortApplications(TestPaperResults, model.SortBy, model.SortOrder);
                }



                return TestPaperResults.ToList();
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.ExamTest, ActionConstants.MenuAccess, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return new List<ExamTestViewModel>();

            }

        }

        private IQueryable<ExamTestViewModel> SortApplications(IQueryable<ExamTestViewModel> Query, string SortName, string SortOrder)
        {

            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(ExamTestViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<ExamTestViewModel>(resultExpression);

        }

        //public List<ExamTestViewModel> GetExamTests()
        //{
        //    try
        //    {

        //        var ApplicationKey = dbContext.AppUsers.Where(row => row.RowKey == DbConstants.User.UserKey).Select(row => row.ApplicationKey).FirstOrDefault();


        //        IQueryable<ExamTestViewModel> TestPaperList = (from TP in dbContext.TestPapers
        //                                                 join ET in dbContext.ExamTests.Where(row => row.ApplicationKey == ApplicationKey) on TP.RowKey equals ET.TestPaperKey
        //                                                 into ETJ
        //                                                 from ET in ETJ.DefaultIfEmpty()
        //                                                 select new ExamTestViewModel
        //                                                 {
        //                                                     RowKey = ET.RowKey != null ? ET.RowKey : 0,
        //                                                     TestPaperKey = TP.RowKey,
        //                                                     TestPaperName = TP.TestPaperName,
        //                                                     SubjectName=TP.Subject.SubjectName,
        //                                                     IsFinished = ET.RowKey != null ? dbContext.TestSections.Where(row => row.TestModule.TestPaperKey == TP.RowKey).GroupBy(row => row.TestModule.ModuleKey).Select(row => row.FirstOrDefault()).Count() == ET.ExamTestSections.Count() : false,
        //                                                     ExamStart = TP.ExamDate,
        //                                                     ExamStatusKey=ET.ExamStatusKey??0
        //                                                 });


        //        TestPaperList = TestPaperList.Where(x => x.ExamStart <= DateTimeUTC.Now || x.IsFinished==true);



        //        return TestPaperList.ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        ActivityLog.CreateActivityLog(MenuConstants.ExamTest, ActionConstants.MenuAccess, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
        //        return new List<ExamTestViewModel>();

        //    }

        //}

        public ExamTestViewModel GetExamTests(ExamTestViewModel model)
        {

            var ApplicationKey = dbContext.Applications.Where(row => row.AppUserKey == DbConstants.User.UserKey).Select(row => row.RowKey).FirstOrDefault();
            ObjectParameter TotalCount = new ObjectParameter("TotalRecordCount", typeof(Int64));
            model.ExamTestList = dbContext.SpSelectTestPaperList(ApplicationKey,
                1,
                model.PageIndex,
                model.PageSize,
                TotalCount,
                model.SortBy,
                model.SortOrder
                ).Select(row => new ExamTestListViewModel
                {
                    TestPaperKey = row.TestPaperKey,
                    RowKey = row.RowKey,
                    ExamStatusKey = row.ExamStatusKey,
                    SubjectName = row.SubjectName,
                    CourseName = row.CourseName,
                    TestPaperTypeName = row.TestPaperTypeName,
                    ExamDate = row.ExamDate,
                    TestPaperTypeKey = row.TestPaperTypeKey,
                    TestPaperName = row.TestPaperName
                }).ToList();


            model.TotalRecords = TotalCount.Value != DBNull.Value ? Convert.ToInt64(TotalCount.Value) : 0;

            return model;
        }

        public ExamTestViewModel GetExamTestById(ExamTestViewModel model)
        {
            try
            {
                long? ApplicationKey = 0;
                string AdmissionNo;
                if (DbConstants.User.RoleKey != DbConstants.Role.Students)
                {
                    ApplicationKey = model.ApplicationKey;
                }
                else
                {
                    ApplicationKey = dbContext.AppUsers.Where(row => row.RowKey == DbConstants.User.UserKey).Select(row => row.ApplicationKey).FirstOrDefault();
                }

                long ExamTestSectionKey = dbContext.ExamTestSections.Where(x => x.ExamTest.TestPaperKey == model.TestPaperKey && x.ExamTest.ApplicationKey == ApplicationKey).Select(x => x.RowKey).SingleOrDefault();

                AdmissionNo = dbContext.Applications.Where(x => x.RowKey == ApplicationKey).Select(x => x.AdmissionNo).SingleOrDefault();


                ExamTestViewModel objViewModel = new ExamTestViewModel();

                if (ExamTestSectionKey != 0)
                {


                    model.ExamAnswers = (from row in dbContext.ExamTestAnswers
                                         where row.ExamTestSectionKey == ExamTestSectionKey
                                         select new ExamTestAnswerViewModel
                                         {
                                             QuestionNumber = row.QuestionNumber,
                                             AnswerText = row.AnswerText,
                                             OptionRowKey = row.TestQuestionOption.RowKey,
                                             IsCorrect = row.IsCorrect,
                                             TotalScore = row.Marks ?? 0
                                         }
                                          ).ToList();
                    model.TestPaperKey = model.TestPaperKey;

                    using (var transaction = dbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            UpdateExamAnswers(model, ExamTestSectionKey);
                            dbContext.SaveChanges();
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                        }
                    }
                }





                objViewModel = dbContext.ExamTestSections.Where(row => row.ExamTest.TestPaperKey == model.TestPaperKey && row.ModuleKey == model.ModuleKey && row.ExamTest.ApplicationKey == ApplicationKey).Select(row => new ExamTestViewModel
                {
                    RowKey = row.ExamTest.RowKey,
                    TotalScore = row.TotalMark,
                    TestPaperTypeKey = row.ExamTest.TestPaper.ExamTypeKey,
                    TestPaperKey = row.ExamTest.TestPaperKey,
                    ExamTestSectionKey = row.RowKey,
                    TestPaperName = row.ExamTest.TestPaper.TestPaperName,
                    ExamStart = row.StartTime,
                    ExamEnd = row.EndTime,
                    SubjectName = row.ExamTest.TestPaper.Subject.SubjectName,
                    ExamStatusKey = row.ExamTest.ExamStatusKey ?? 0,
                    ExamKey = row.ExamTest.ExamKey,
                    StudentProfilePhoto = UrlConstants.ApplicationUrl + AdmissionNo + "/" + row.ExamTest.Application.StudentPhotoPath,
                    ModuleKey = row.ModuleKey,


                    TestSections = dbContext.TestSections.Where(x => x.TestModule.TestPaperKey == row.ExamTest.TestPaperKey && x.TestModule.ModuleKey == model.ModuleKey).Select(x => new TestSectionViewModel
                    {
                        RowKey = x.RowKey,
                        TestSectionName = x.SectionName,
                        TestSectionFileName = x.QuestionContent,
                        SupportedFileName = UrlConstants.TestSupportFileUrl + row.ExamTest.TestPaperKey + "/" + x.TestModule.ModuleKey + "/" + x.SupportFile,
                        QuestionPaperFileName = x.QuestionPaperFile,

                        QuestionDetails = dbContext.TestQuestions.Where(y => y.TestSectionKey == x.RowKey).Select(y => new TestQuestionViewModel
                        {
                            QuestionNumber = y.QuestionNumber,
                            QuestionStatusKey = DbConstants.QuestionStatus.NotVisited,
                            MaximumMark = y.Mark,
                            Mark = y.Mark,
                            QuestionDuration = y.QuestionDuration,
                            QuestionOptions = y.TestSection.TestQuestionOptions.Select(m => new TestQuestionOptionsViewModel
                            {
                                OptionValue = m.OptionValue,
                                OptionText = m.OptionText,
                                RowKey = m.RowKey,
                                TestSectionKey = m.TestSectionKey
                            }
                            ).ToList(),




                        }).ToList()

                    }).OrderBy(x => x.RowKey).ToList(),



                }).FirstOrDefault();

                if (objViewModel == null)
                {
                    objViewModel = dbContext.TestPapers.Where(row => row.RowKey == model.TestPaperKey).Select(row => new ExamTestViewModel
                    {
                        TestPaperKey = row.RowKey,
                        TestPaperTypeKey = row.ExamTypeKey,
                        TestPaperName = row.TestPaperName,
                        SubjectName = row.Subject.SubjectName,
                        StudentProfilePhoto = UrlConstants.ApplicationUrl + "/" + AdmissionNo + "/" + dbContext.Applications.Where(x => x.RowKey == ApplicationKey).Select(x => x.StudentPhotoPath).FirstOrDefault(),
                        ModuleKey = row.TestModules.Select(x => x.ModuleKey).FirstOrDefault(),
                        ExamStatusKey = 0,
                        ExamKey = row.ExamTests.Select(x => x.ExamKey).FirstOrDefault(),
                        SupportedFileName = UrlConstants.TestSupportFileUrl + row.RowKey + "/" + dbContext.TestSections.Where(x => x.TestModule.TestPaperKey == row.RowKey && x.TestModule.ModuleKey == model.ModuleKey).Select(x => x.SupportFile).FirstOrDefault(),
                        TestSections = dbContext.TestSections.Where(x => x.TestModule.TestPaperKey == row.RowKey && x.TestModule.ModuleKey == model.ModuleKey).Select(x => new TestSectionViewModel
                        {
                            RowKey = x.RowKey,
                            TestSectionName = x.SectionName,
                            TestSectionFileName = x.QuestionContent,
                            SupportedFileName = UrlConstants.TestSupportFileUrl + row.RowKey + "/" + x.TestModule.ModuleKey + "/" + x.SupportFile,
                            QuestionPaperFileName = x.QuestionPaperFile,

                            QuestionDetails = dbContext.TestQuestions.Where(y => y.TestSectionKey == x.RowKey).Select(y => new TestQuestionViewModel
                            {
                                QuestionNumber = y.QuestionNumber,
                                QuestionStatusKey = DbConstants.QuestionStatus.NotVisited,
                                MaximumMark = y.Mark,
                                QuestionOptions = y.TestSection.TestQuestionOptions.Select(m => new TestQuestionOptionsViewModel
                                {
                                    OptionValue = m.OptionValue,
                                    OptionText = m.OptionText,
                                    RowKey = m.RowKey,
                                    TestSectionKey = m.TestSectionKey
                                }
                            ).ToList(),
                                Mark = x.TestQuestions.Select(m => m.Mark).FirstOrDefault(),
                                QuestionDuration = y.QuestionDuration,


                            }).ToList()
                        }).OrderBy(x => x.RowKey).ToList(),


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
                                                    Mark = ets.Marks,
                                                    QuestionStatusKey = ets.QuestionStatusKey ?? DbConstants.QuestionStatus.NotAnswered,
                                                    AnswerKeyStatus = ets.AnswerKeyStatus ?? false,
                                                    MaximumMark = qd.Mark,
                                                    QuestionDuration = qd.QuestionDuration,
                                                    QuestionPaperFileName = qd.TestSection.QuestionPaperFile,


                                                    QuestionOptions = qd.TestSection.TestQuestionOptions.Select(m => new TestQuestionOptionsViewModel
                                                    {
                                                        OptionValue = m.OptionValue,
                                                        OptionText = m.OptionText,
                                                        RowKey = m.RowKey,
                                                        TestSectionKey = m.TestSectionKey
                                                    }


                            ).ToList(),


                                                }).ToList();
                    }
                }


                objViewModel.RowKey = dbContext.ExamTestSections.Where(row => row.ExamTest.TestPaperKey == model.TestPaperKey && row.ExamTest.ApplicationKey == ApplicationKey).Select(row => row.RowKey).FirstOrDefault();
                TestModule TestModule = dbContext.TestModules.Where(row => row.TestPaperKey == model.TestPaperKey && row.ModuleKey == model.ModuleKey).SingleOrDefault();
                if (TestModule != null)
                {

                    objViewModel.ApplicantName = dbContext.Applications.Where(x => x.RowKey == ApplicationKey).Select(x => x.StudentName).SingleOrDefault();

                    objViewModel.ExamDuration = TestModule.TestDuration ?? 0;
                    objViewModel.MaximumScore = TestModule.Module.MaximumBandScore ?? 0;
                    objViewModel.TotalNegativeMarks = dbContext.ExamTestAnswers.Where(x => x.AnswerKeyStatus == true && x.ExamTestSection.ExamTest.ApplicationKey == ApplicationKey && x.ExamTestSectionKey == objViewModel.ExamTestSectionKey && x.IsCorrect == false && x.QuestionStatu.QuestionStatusCategory == DbConstants.QuestionStatus.Answered && x.Marks < 0).Select(x => x.Marks ?? 0).DefaultIfEmpty().Sum();
                    objViewModel.TotalScore = (objViewModel.TestSections.Select(x => x.QuestionDetails.Where(y => y.AnswerKeyStatus).Select(y => y.Mark).DefaultIfEmpty().Sum()).DefaultIfEmpty().Sum());
                    objViewModel.TotalAttept = dbContext.ExamTestAnswers.Where(x => x.ExamTestSectionKey == objViewModel.ExamTestSectionKey && x.QuestionStatu.QuestionStatusCategory == DbConstants.QuestionStatus.Answered).Count();
                    //objViewModel.TotalQuestions= objViewModel.TestSections.Select(x => x.QuestionDetails.Where(y=>y.QuestionStatusKey==DbConstants.QuestionStatus.Answered || y.QuestionStatusKey==DbConstants.QuestionStatus.AnsweredAndMarkedForReview).DefaultIfEmpty().Count()).DefaultIfEmpty().Count();
                    objViewModel.TotalQuestions = dbContext.TestQuestions.Where(x => x.TestSection.TestModule.TestPaper.RowKey == objViewModel.TestPaperKey).Count();
                    objViewModel.TotalCorrectAnswers = dbContext.ExamTestAnswers.Where(x => x.AnswerKeyStatus == true && x.ExamTestSection.ExamTest.ApplicationKey == ApplicationKey && x.ExamTestSectionKey == objViewModel.ExamTestSectionKey && x.IsCorrect == true && x.QuestionStatu.QuestionStatusCategory == DbConstants.QuestionStatus.Answered).Count();
                    objViewModel.TotalInCorrectAnswers = dbContext.ExamTestAnswers.Where(x => x.AnswerKeyStatus == true && x.ExamTestSection.ExamTest.ApplicationKey == ApplicationKey && x.ExamTestSectionKey == objViewModel.ExamTestSectionKey && x.IsCorrect == false && x.QuestionStatu.QuestionStatusCategory == DbConstants.QuestionStatus.Answered).Count();
                    objViewModel.AnswerKeyStatus = dbContext.ExamTestAnswers.Where(x => x.AnswerKeyStatus == false && x.ExamTestSection.ExamTest.ApplicationKey == ApplicationKey && x.ExamTestSectionKey == objViewModel.ExamTestSectionKey && x.QuestionStatu.QuestionStatusCategory == DbConstants.QuestionStatus.Answered).Any();
                    objViewModel.MaximumScore = objViewModel.TestSections.ToList().Select(x => x.QuestionDetails.ToList().Select(m => m.MaximumMark ?? 0).DefaultIfEmpty().Sum()).Sum();

                    //if (objViewModel.ExamStart != null)
                    //{
                    //    objViewModel.EndTimeMilliSeconds = Convert.ToInt32(((DateTimeUTC.Now.Subtract(objViewModel.ExamStart.Value))).TotalMilliseconds).ToString();
                    //}
                    //else
                    //{
                    //    objViewModel.EndTimeMilliSeconds = "0";
                    //}
                }

                //objViewModel.ModuleKey = dbContext.ExamTestSections.Where(row => row.ExamTest.TestPaperKey == model.TestPaperKey && row.ExamTest.ApplicationKey == model.ApplicationKey).OrderByDescending(row => row.ModuleKey).Select(row => row.ModuleKey).FirstOrDefault();
                objViewModel.InstructionFileName = dbContext.TestPapers.Where(row => row.RowKey == model.TestPaperKey).Select(row => UrlConstants.TestInstructionFileUrl + row.InstructionFile).FirstOrDefault();


                objViewModel.ExamStart = DateTimeUTC.Now;
                objViewModel.IsExamExists = dbContext.ExamTests.Where(x => x.TestPaperKey == model.TestPaperKey && x.ApplicationKey == ApplicationKey).Any();
                objViewModel.ApplicationKey = ApplicationKey ?? 0;


                objViewModel.IsShowAllQuestions = dbContext.ExamTypes.Where(x => x.RowKey == objViewModel.TestPaperTypeKey).Select(x => x.IsShowAllQuestions ?? false).SingleOrDefault();


                //if (DbConstants.User.RoleKey == DbConstants.Role.Student && model.TestPaperKey != 0 && model.RowKey==0)
                //{

                //}



                return objViewModel;
            }
            catch (Exception ex)
            {
                //ActivityLog.CreateActivityLog(MenuConstants.TestPaper, (model.RowKey != 0 ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return new ExamTestViewModel();

            }
        }





        //public ExamTestViewModel GetExamTestById(ExamTestViewModel model)
        //{
        //    try
        //    {
        //        var ApplicationKey = dbContext.AppUsers.Where(row => row.RowKey == DbConstants.User.UserKey).Select(row => row.ApplicationKey).FirstOrDefault();


        //        ExamTestViewModel objViewModel = new ExamTestViewModel();

        //        objViewModel = dbContext.ExamTestSections.Where(row => row.ExamTest.TestPaperKey == model.TestPaperKey && row.ModuleKey == model.ModuleKey && row.ExamTest.ApplicationKey == ApplicationKey).Select(row => new ExamTestViewModel
        //        {
        //            RowKey = row.ExamTest.RowKey,
        //            TotalScore = row.TotalMark,
        //            TestPaperKey = row.ExamTest.TestPaperKey,
        //            ExamTestSectionKey = row.RowKey,
        //            TestPaperName = row.ExamTest.TestPaper.TestPaperName,
        //            TestSections = dbContext.TestSections.Where(x => x.TestModule.TestPaperKey == row.ExamTest.TestPaperKey && x.TestModule.ModuleKey == model.ModuleKey).Select(x => new TestSectionViewModel
        //             {
        //                 RowKey = x.RowKey,
        //                 TestSectionName = x.SectionName,
        //                 TestSectionFileName = UrlConstants.TestQuestionPaperUrl + row.ExamTest.TestPaperKey + "/" + x.TestModule.ModuleKey + "/" + x.QuestionPaperFile,
        //                 SupportedFileName = UrlConstants.TestSupportFileUrl + row.ExamTest.TestPaperKey + "/" + x.TestModule.ModuleKey + "/" + x.SupportFile,
        //                 QuestionDetails = dbContext.TestQuestions.Where(y => y.TestSectionKey == x.RowKey).Select(y => new TestQuestionViewModel
        //               {
        //                   QuestionNumber = y.QuestionNumber

        //               }).ToList()

        //             }).OrderBy(x => x.RowKey).ToList(),



        //        }).FirstOrDefault();
        //        if (objViewModel == null)
        //        {
        //            objViewModel = dbContext.TestPapers.Where(row => row.RowKey == model.TestPaperKey).Select(row => new ExamTestViewModel
        //            {
        //                TestPaperKey = row.RowKey,
        //                TestPaperName = row.TestPaperName,
        //                SupportedFileName = UrlConstants.TestSupportFileUrl + row.RowKey + "/" + dbContext.TestSections.Where(x => x.TestModule.TestPaperKey == row.RowKey && x.TestModule.ModuleKey == model.ModuleKey).Select(x => x.SupportFile).FirstOrDefault(),
        //                TestSections = dbContext.TestSections.Where(x => x.TestModule.TestPaperKey == row.RowKey && x.TestModule.ModuleKey == model.ModuleKey).Select(x => new TestSectionViewModel
        //                {
        //                    RowKey = x.RowKey,
        //                    TestSectionName = x.SectionName,
        //                    TestSectionFileName = UrlConstants.TestQuestionPaperUrl + row.RowKey + "/" + x.TestModule.ModuleKey + "/" + x.QuestionPaperFile,
        //                    SupportedFileName = UrlConstants.TestSupportFileUrl + row.RowKey + "/" + x.TestModule.ModuleKey + "/" + x.SupportFile,
        //                    QuestionDetails = dbContext.TestQuestions.Where(y => y.TestSectionKey == x.RowKey).Select(y => new TestQuestionViewModel
        //              {
        //                  QuestionNumber = y.QuestionNumber

        //              }).ToList()
        //                }).OrderBy(x => x.RowKey).ToList()

        //            }).FirstOrDefault();
        //            if (objViewModel == null)
        //                objViewModel = new ExamTestViewModel();


        //        }
        //        else
        //        {
        //            foreach (TestSectionViewModel item in objViewModel.TestSections)
        //            {
        //                item.QuestionDetails = (from qd in dbContext.TestQuestions.Where(y => y.TestSectionKey == item.RowKey)
        //                                        join ets in dbContext.ExamTestAnswers.Where(row => row.ExamTestSection.ExamTestKey == objViewModel.RowKey && row.ExamTestSection.ModuleKey == model.ModuleKey)
        //                                         on qd.QuestionNumber equals ets.QuestionNumber into etsj
        //                                        from ets in etsj.DefaultIfEmpty()
        //                                        select new TestQuestionViewModel
        //                 {
        //                     QuestionNumber = qd.QuestionNumber,
        //                     AnswerKey = qd.AnswerKey,
        //                     AnswerText = ets.AnswerText,
        //                     IsCorrect = ets != null ? ets.IsCorrect : false,
        //                     Mark = ets.Marks
        //                 }).ToList();
        //            }
        //        }
        //        objViewModel.RowKey = dbContext.ExamTestSections.Where(row => row.ExamTest.TestPaperKey == model.TestPaperKey && row.ExamTest.ApplicationKey == ApplicationKey).Select(row => row.RowKey).FirstOrDefault();
        //        TestModule TestModule = dbContext.TestModules.Where(row => row.TestPaperKey == model.TestPaperKey && row.ModuleKey == model.ModuleKey).SingleOrDefault();
        //        if (TestModule != null)
        //        {
        //            objViewModel.ExamDuration = TestModule.TestDuration ?? 0;
        //            objViewModel.MaximumScore = TestModule.Module.MaximumBandScore ?? 0;
        //        }
        //        objViewModel.ExamStart = DateTimeUTC.Now;
        //        return objViewModel;
        //    }
        //    catch (Exception ex)
        //    {
        //        //ActivityLog.CreateActivityLog(MenuConstants.TestPaper, (model.RowKey != 0 ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
        //        return new ExamTestViewModel();

        //    }
        //}

        public string generateID()
        {
            return Guid.NewGuid().ToString("N");
        }

        public ExamTestViewModel CreateExamTest(ExamTestViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    long Maxkey = dbContext.ExamTests.Select(x => x.RowKey).DefaultIfEmpty().Max();
                    long ApplicationKey = dbContext.AppUsers.Where(row => row.RowKey == DbConstants.User.UserKey).Select(row => row.ApplicationKey ?? 0).FirstOrDefault();




                    ExamTest ExamTestModel = new ExamTest();

                    if (!dbContext.ExamTests.Where(x => x.TestPaperKey == model.TestPaperKey && x.ApplicationKey == ApplicationKey).Any())
                    {
                        ExamTestModel.RowKey = ++Maxkey;
                        model.RowKey = ExamTestModel.RowKey;
                        ExamTestModel.TestPaperKey = model.TestPaperKey;
                        ExamTestModel.ExamStatusKey = DbConstants.OnlineExamStatus.Started;
                        ExamTestModel.ApplicationKey = ApplicationKey;
                        //ExamTestModel.ExamKey = generateID();
                        dbContext.ExamTests.Add(ExamTestModel);
                        CreateExamSection(model, ExamTestModel.RowKey);
                        model.TestPaperKey = ExamTestModel.RowKey;
                        model.ExamStatusKey = DbConstants.OnlineExamStatus.Started;
                        model.ExamKey = ExamTestModel.ExamKey;
                        bool anyFinished = model.ExamAnswers.Where(x => x.QuestionStatusKey == DbConstants.QuestionStatus.Answered || x.QuestionStatusKey == DbConstants.QuestionStatus.AnsweredAndMarkedForReview).Any();

                        if (anyFinished)
                        {
                            ExamTestModel.ExamStatusKey = DbConstants.OnlineExamStatus.Finished;
                        }

                    }
                    else
                    {
                        ExamTestModel = dbContext.ExamTests.Where(x => x.TestPaperKey == model.TestPaperKey && x.ApplicationKey == ApplicationKey).SingleOrDefault();
                        model.RowKey = ExamTestModel.RowKey;
                        if (ExamTestModel.ExamKey == null)
                        {
                            ExamTestModel.ExamKey = generateID();
                        }

                        bool anyFinished = model.ExamAnswers.Where(x => x.QuestionStatusKey == DbConstants.QuestionStatus.Answered || x.QuestionStatusKey == DbConstants.QuestionStatus.AnsweredAndMarkedForReview).Any();

                        if (anyFinished)
                        {
                            ExamTestModel.ExamStatusKey = DbConstants.OnlineExamStatus.Finished;
                        }

                        CreateExamSection(model, ExamTestModel.RowKey);
                    }


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
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.ExamTest);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ExamTest, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }

                return model;



            }
        }

        public ExamTestViewModel UpdateExamStatus(ExamTestViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    ExamTest dbExamTest = dbContext.ExamTests.Where(x => x.RowKey == model.TestPaperKey && x.ApplicationKey == model.ApplicationKey).SingleOrDefault();
                    dbExamTest.ExamStatusKey = model.ExamStatusKey;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.IsSuccessful = true;
                    model.Message = EduSuiteUIResources.Success;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.IsSuccessful = false;
                    model.Message = EduSuiteUIResources.Failed;
                }
            }
            return model;
        }

        public ExamTestViewModel UpdateExamKey(ExamTestViewModel model)
        {
            CreateExamTest(model);
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    ExamTest dbExamTest = dbContext.ExamTests.Where(x => x.TestPaperKey == model.TestPaperKey && x.ApplicationKey == model.ApplicationKey).SingleOrDefault();
                    if (dbExamTest.ExamKey == null)
                    {
                        dbExamTest.ExamKey = generateID();
                    }

                    if (dbExamTest.ExamTestSections.Select(x => x.StartTime).FirstOrDefault() != null)
                    {
                        model.EndTimeMilliSeconds = Convert.ToInt32(((DateTimeUTC.Now.Subtract(dbExamTest.ExamTestSections.Select(x => x.StartTime).FirstOrDefault()))).TotalMilliseconds).ToString();
                    }
                    else
                    {
                        model.EndTimeMilliSeconds = "0";
                    }

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.IsSuccessful = true;
                    model.Message = EduSuiteUIResources.Success;
                    model.ExamKey = dbExamTest.ExamKey;

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.IsSuccessful = false;
                    model.Message = EduSuiteUIResources.Failed;
                }
            }
            return model;
        }

        public ExamTestViewModel UpdateExamTest(ExamTestViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    ExamTest ExamTestModel = dbContext.ExamTests.SingleOrDefault(x => x.RowKey == model.RowKey);
                    model.ApplicationKey = dbContext.AppUsers.Where(row => row.RowKey == DbConstants.User.UserKey).Select(row => row.ApplicationKey ?? 0).FirstOrDefault();

                    if (model.ModuleKey != 0)
                    {
                        CreateExamSection(model, ExamTestModel.RowKey);
                    }

                    ExamTestModel.ExamStatusKey = DbConstants.OnlineExamStatus.Finished;

                    //if (ExamTestModel.ExamKey == null)
                    //{
                    //    ExamTestModel.ExamKey = generateID();
                    //}

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
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.ExamTest);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ExamTest, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }

                return model;
            }
        }
        private void CreateExamSection(ExamTestViewModel model, long ExamTestKey)
        {


            ExamTestSection ExamTestSectionModel = new ExamTestSection();

            if (!dbContext.ExamTestSections.Where(x => x.ExamTestKey == model.RowKey).Any())
            {
                long Maxkey = dbContext.ExamTestSections.Select(x => x.RowKey).DefaultIfEmpty().Max();
                ExamTestSectionModel.RowKey = ++Maxkey;
                ExamTestSectionModel.ExamTestKey = ExamTestKey;
                ExamTestSectionModel.ModuleKey = 1;
                ExamTestSectionModel.StartTime = Convert.ToDateTime(model.ExamStart ?? DateTimeUTC.Now);
                ExamTestSectionModel.EndTime = DateTimeUTC.Now;
                dbContext.ExamTestSections.Add(ExamTestSectionModel);
            }
            else
            {
                ExamTestSectionModel = dbContext.ExamTestSections.Where(x => x.ExamTestKey == model.RowKey).SingleOrDefault();
            }

            UpdateExamAnswers(model, ExamTestSectionModel.RowKey);
            int CorrectCount = model.ExamAnswers.Where(row => row.IsCorrect).Count();

            //ExamTestSectionModel.TotalMark = dbContext.ModuleBands.Where(row => row.ModuleKey == model.ModuleKey && row.FromScore <= CorrectCount && (row.ToScore ?? CorrectCount) >= CorrectCount).Select(row => row.BandScore).FirstOrDefault();

            //if (model.ExamAnswers.Where(row => row.AnswerText == null).Count() == model.ExamAnswers.Count)
            //{
            //    ExamTestSectionModel.TotalMark = 0;
            //}

        }



        public void UpdateExamAnswers(ExamTestViewModel model, long ExamTestSectionKey)
        {

            if (ExamAnswerMaxkey == 0)
                ExamAnswerMaxkey = dbContext.ExamTestAnswers.Select(x => x.RowKey).DefaultIfEmpty().Max();
            foreach (ExamTestAnswerViewModel item in model.ExamAnswers)
            {
                ExamTestAnswer ExamTestAnswerModel = new ExamTestAnswer();
                bool checkExists = dbContext.ExamTestAnswers.Where(x => x.QuestionNumber == item.QuestionNumber && x.ExamTestSectionKey == ExamTestSectionKey).Any();
                if (!checkExists)
                {
                    ExamTestAnswerModel.RowKey = ++ExamAnswerMaxkey;
                    ExamTestAnswerModel.ExamTestSectionKey = ExamTestSectionKey;
                    ExamTestAnswerModel.QuestionNumber = item.QuestionNumber;
                }
                else
                {
                    ExamTestAnswerModel = dbContext.ExamTestAnswers.Where(x => x.QuestionNumber == item.QuestionNumber && x.ExamTestSectionKey == ExamTestSectionKey).SingleOrDefault();
                }


                ExamTestAnswerModel.AnswerText = item.AnswerText;
                ExamTestAnswerModel.AnswerOptionKey = item.OptionRowKey;
                ExamTestAnswerModel.QuestionStatusKey = item.QuestionStatusKey == 0 ? ExamTestAnswerModel.QuestionStatusKey : item.QuestionStatusKey;


                List<string> AnswerKeys = new List<string>();
                string AnswerKey = dbContext.TestQuestions.Where(row => row.QuestionNumber == ExamTestAnswerModel.QuestionNumber && row.TestSection.TestModule.TestPaperKey == model.TestPaperKey).Select(row => row.AnswerKey).FirstOrDefault();


                if (!String.IsNullOrEmpty(AnswerKey))
                {
                    decimal Mark = dbContext.TestQuestions.Where(row => row.QuestionNumber == ExamTestAnswerModel.QuestionNumber && row.TestSection.TestModule.TestPaperKey == model.TestPaperKey).Select(row => row.Mark ?? 0).FirstOrDefault();
                    decimal NegativeMark = (-dbContext.TestQuestions.Where(row => row.QuestionNumber == ExamTestAnswerModel.QuestionNumber && row.TestSection.TestModule.TestPaperKey == model.TestPaperKey).Select(row => row.NegativeMark ?? 0).FirstOrDefault());
                    AnswerKeys = AnswerKey.Split('|').Select(x => x.ToLower()).ToList();
                    if (ExamTestAnswerModel.QuestionStatusKey != DbConstants.QuestionStatus.Markedforreview)
                    {
                        ExamTestAnswerModel.IsCorrect = item.AnswerText != null ? AnswerKeys.Contains(item.AnswerText.ToLower()) : false;
                        ExamTestAnswerModel.Marks = item.AnswerText != null ? (decimal?)(ExamTestAnswerModel.IsCorrect ? Mark : (-NegativeMark)) : null;
                        ExamTestAnswerModel.AnswerKeyStatus = true;
                        ExamTestAnswerModel.AnswerOptionKey = item.OptionRowKey;
                        item.IsCorrect = ExamTestAnswerModel.IsCorrect;
                    }
                    else
                    {
                        ExamTestAnswerModel.AnswerKeyStatus = false;
                        item.IsCorrect = false;
                        ExamTestAnswerModel.Marks = null;
                        ExamTestAnswerModel.IsCorrect = false;

                    }

                }
                else
                {
                    ExamTestAnswerModel.AnswerKeyStatus = false;
                    item.IsCorrect = false;
                    ExamTestAnswerModel.Marks = null;
                    ExamTestAnswerModel.IsCorrect = false;
                }



                if (!checkExists)
                {
                    dbContext.ExamTestAnswers.Add(ExamTestAnswerModel);
                }
            }
        }


        public void InitializeData(ExamTestViewModel model)
        {
            long ApplicationKey = 0;
            if (DbConstants.User.RoleKey != DbConstants.Role.Students)
            {
                ApplicationKey = model.ApplicationKey;
            }
            else
            {
                ApplicationKey = dbContext.AppUsers.Where(row => row.RowKey == DbConstants.User.UserKey).Select(row => row.ApplicationKey ?? 0).FirstOrDefault();

            }


            model.ModuleKey = dbContext.ExamTestSections.Where(row => row.ExamTest.TestPaperKey == model.TestPaperKey && row.ExamTest.ApplicationKey == ApplicationKey).OrderByDescending(row => row.ModuleKey).Select(row => row.ModuleKey).FirstOrDefault();
            model.InstructionFileName = dbContext.TestPapers.Where(row => row.RowKey == model.TestPaperKey).Select(row => UrlConstants.TestInstructionFileUrl + row.InstructionFile).FirstOrDefault();

            model.ApplicationKey = ApplicationKey;
        }



        public dynamic GetExamReviewById(long id)
        {
            long ApplicationKey = dbContext.AppUsers.Where(row => row.RowKey == DbConstants.User.UserKey).Select(row => row.ApplicationKey ?? 0).FirstOrDefault();
            return dbContext.ExamTestAnswers.Where(row => row.ExamTestSection.ExamTest.TestPaperKey == id && row.ExamTestSection.ExamTest.ApplicationKey == ApplicationKey).GroupBy(row => new { row.ExamTestSection.ModuleKey, row.ExamTestSection.Module.ModuleName }).Select(row =>
                new
                {
                    ModuleName = row.Key.ModuleName,
                    TotalCount = row.Count(),
                    NotAttendedCount = row.Where(x => x.AnswerText == null).Count(),
                    CorrectCount = row.Where(x => x.IsCorrect).Count(),
                    IncorrectCount = row.Where(x => !x.IsCorrect).Count(),

                }).ToList();

        }


        public dynamic GetExamTestsById(long ApplicationKey)
        {
            if (ApplicationKey == 0)
            {
                ApplicationKey = dbContext.AppUsers.Where(row => row.RowKey == DbConstants.User.UserKey).Select(row => row.ApplicationKey ?? 0).FirstOrDefault();
            }
            return
            new
            {
                Modules = dbContext.Modules.Select(row => new
                {
                    RowKey = row.RowKey,
                    ModuleName = row.ModuleName
                }).ToList(),
                ExamTests = dbContext.ExamTests.Where(row => row.ApplicationKey == ApplicationKey).Select(row => new
                {
                    TestName = row.TestPaper.Subject.SubjectName,

                    ExamModules = row.ExamTestSections.Select(x => new
                    {
                        ModuleKey = x.ModuleKey,
                        ModuleName = x.Module.ModuleName,
                        TotalScore = x.TotalMark,
                        Color = dbContext.BandStatus.Where(y => y.RowKey == x.ModuleKey && y.BandScore <= x.TotalMark).OrderByDescending(y => y.BandScore).Select(y => y.BandColor).FirstOrDefault()
                    }),
                }).ToList()

            };

        }

        public bool IsFeePaid()
        {
            Application dbApplication = dbContext.Applications.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
            bool IsFeePaid = dbContext.FeePaymentMasters.Where(x => x.ApplicationKey == dbApplication.RowKey).Any();
            return IsFeePaid;
        }
    }




}
