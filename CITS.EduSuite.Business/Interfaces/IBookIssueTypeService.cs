using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IBookIssueTypeService
    {
        List<BookIssueTypeViewModel> GetBookIssueTypes(string searchText);
        BookIssueTypeViewModel GetBookIssueTypeById(byte? id);
        BookIssueTypeViewModel CreateBookIssueType(BookIssueTypeViewModel model);
        BookIssueTypeViewModel UpdateBookIssueType(BookIssueTypeViewModel model);
        BookIssueTypeViewModel DeleteBookIssueType(BookIssueTypeViewModel model);
    }
}
