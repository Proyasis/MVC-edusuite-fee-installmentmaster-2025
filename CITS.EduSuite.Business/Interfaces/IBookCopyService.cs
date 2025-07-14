using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IBookCopyService
    {
        List<BookCopyViewModel> GetBookCopies(long BookId);
        BookCopyViewModel GetBookCopyById(long? id);
        BookCopyViewModel CreateBookCopy(BookCopyViewModel model);
        BookCopyViewModel UpdateBookCopy(BookCopyViewModel model);
        BookCopyViewModel DeleteBookCopy(BookCopyViewModel model);
        List<BookCopyViewModel> GetBooksById(List<long> BookIds);
    }
}
