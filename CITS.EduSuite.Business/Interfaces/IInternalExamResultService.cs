using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;


namespace CITS.EduSuite.Business.Interfaces
{
    public interface IInternalExamResultService
    {
        InternalExamResultViewModel UpdateInternalExamResult(InternalExamResultViewModel model);
        InternalExamResultViewModel GetInternalExamResultDetails(InternalExamResultViewModel model);
        List<InternalExamResultViewModel> GetInternalExamResult(InternalExamResultViewModel model);
        InternalExamResultViewModel StudentMarkDetils(InternalExamResultViewModel model);
        InternalExamResultViewModel GetEmployeesByBranchId(InternalExamResultViewModel model);
        InternalExamResultViewModel GetSearchDropDownLists(InternalExamResultViewModel model);
        InternalExamResultViewModel DeleteInternalExamResult(long? InternalExamKey, long? InternalExamDetailsKey, long? BookKey);
        InternalExamResultViewModel FillSearchClassDetails(InternalExamResultViewModel model);
        InternalExamResultViewModel FillSearchBatch(InternalExamResultViewModel model);
        InternalExamResultViewModel UpdateInternalExamResults(InternalExamResultViewModel MasterModel);
        List<SelectListModel> FillClassDetailsBy(InternalExamViewModel model);
        List<SelectListModel> FillSubjectsById(InternalExamViewModel model);
        List<SelectListModel> FillStudentsById(InternalExamViewModel model);
        bool CheckexistingExam(InternalExamViewModel model);
    }
}
