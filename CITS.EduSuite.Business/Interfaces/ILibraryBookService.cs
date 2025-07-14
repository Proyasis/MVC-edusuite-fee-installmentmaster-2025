using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface ILibraryBookService
    {
        List<LibraryBookViewModel> GetLibraryBooks(LibraryBookViewModel model, out long TotalRecords);
        LibraryBookViewModel GetLibraryBookById(long? id);
        LibraryBookViewModel CreateLibraryBook(LibraryBookViewModel model);
        LibraryBookViewModel UpdateLibraryBook(LibraryBookViewModel model);
        LibraryBookViewModel DeleteLibraryBook(LibraryBookViewModel model);
        LibraryBookViewModel FillRacks(LibraryBookViewModel model);
        LibraryBookViewModel FillSubRack(LibraryBookViewModel model);
        void FillBranch(LibraryBookViewModel model);

    }
}
