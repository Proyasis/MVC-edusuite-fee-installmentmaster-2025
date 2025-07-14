using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IBookStatusService
    {
        List<BookStatusViewModel> GetBookStatus(string searchText);

        BookStatusViewModel GetBookStatusById(byte? id);

        BookStatusViewModel CreateBookStatus(BookStatusViewModel model);

        BookStatusViewModel UpdateBooKStatus(BookStatusViewModel model);

        BookStatusViewModel DeleteBookStatus(BookStatusViewModel model);
     
    }
}
