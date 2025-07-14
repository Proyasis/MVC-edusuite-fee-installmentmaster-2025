using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IBookIssueReturnService
    {
        List<BookIssueReturnMasterViewModel> GetBookIssueReturn(BookIssueReturnMasterViewModel model, out long TotalRecords);

        BookIssueReturnMasterViewModel GetBookIssueReturnById(BookIssueReturnMasterViewModel model);

        BookIssueReturnMasterViewModel GetBookIssueByValues(BookIssueReturnMasterViewModel model);

        BookIssueReturnMasterViewModel CreateBookIssue(BookIssueReturnMasterViewModel model);

        BookIssueReturnMasterViewModel UpdateBookIssue(BookIssueReturnMasterViewModel model);

        BookIssueReturnDetailsViewModel FillBook(BookIssueReturnDetailsViewModel model);

        BookIssueReturnDetailsViewModel FillBookCopy(BookIssueReturnDetailsViewModel model);

        BookIssueReturnMasterViewModel CheckCardIdExists(BookIssueReturnMasterViewModel model);

        BookIssueReturnMasterViewModel DeleteBookIssueReturn(BookIssueReturnMasterViewModel model);

        BookIssueReturnMasterViewModel DeleteBookIssueReturnDetails(BookIssueReturnDetailsViewModel model);

        BookIssueReturnMasterViewModel UpdateBookReturnDetails(BookIssueReturnMasterViewModel model);

        BookIssueReturnDetailsViewModel FillIfAnyFine(BookIssueReturnDetailsViewModel model);

        void FillDropdownLists(BookIssueReturnMasterViewModel model);

    }
}
