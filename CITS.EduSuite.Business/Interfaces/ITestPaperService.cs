using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface ITestPaperService
    {
        List<TestPaperViewModel> GetTestPapers(string searchText);
        TestPaperViewModel GetQuestionPaperById(TestPaperViewModel model);
        TestPaperViewModel CreateTestPaper(TestPaperViewModel model);
        TestPaperViewModel UpdateTestPaper(TestPaperViewModel model);


        TestPaperViewModel DeleteTestPaper(TestPaperViewModel model);
        TestPaperViewModel DeleteTestSection(TestPaperViewModel model);
        TestPaperViewModel DeleteQuestion(TestPaperViewModel model);
        TestPaperViewModel GetTestInstructionById(long Id);
        TestPaperViewModel UpdateTestInstruction(TestPaperViewModel model);
        TestPaperViewModel GetTestAnswerKeyById(TestPaperViewModel model);
        TestPaperViewModel UpdateTestAnswerKey(TestPaperViewModel model);
        bool CheckTestNameExists(string TestPaperName, long RowKey);
        string GetFileTypeById(byte id);

        dynamic GetExamValuations(string searchText, int PageIndex, int PageSize, out long TotalRecords);

        ExamTestViewModel UpdateExamResult(ExamTestViewModel model);
        ExamTestViewModel GetExamValuationById(ExamTestViewModel model);
        TestPaperViewModel DeleteQuestionOption(TestPaperViewModel model);
        TestPaperViewModel FillStaticDropDown(TestPaperViewModel model);
        TestPaperViewModel FillDynamicDropDownlists(TestPaperViewModel model);
       
    }
}
