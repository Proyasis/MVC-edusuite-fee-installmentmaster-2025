using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface ILibraryReportService
    {
        List<LibraryReportViewModel> GetBookSummary(LibraryReportViewModel model);
        void FillDropdownLists(LibraryReportViewModel model);

        List<BookCopyViewModel> BindBookCopies(BookCopyViewModel model);
        List<LibraryReportViewModel> GetMemberPlanSummary(LibraryReportViewModel model);
        void FillMemberplanDropDownList(LibraryReportViewModel model);
        LibraryReportViewModel GetLibraryBookDetails(LibraryReportViewModel model);
        List<LibraryReportViewModel> GetBookIssueSummary(LibraryReportViewModel model);

        void FillBookDrownList(LibraryReportViewModel model);
        LibraryReportViewModel FillRacks(LibraryReportViewModel model);
        LibraryReportViewModel FillSubRack(LibraryReportViewModel model);
        LibraryReportViewModel FillLibraryBooks(LibraryReportViewModel model);
        LibraryReportViewModel FillLibraryBooksCopies(LibraryReportViewModel model);
    }
}
