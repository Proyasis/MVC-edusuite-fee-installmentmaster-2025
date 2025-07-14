using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IBookService
    {
        BookViewModel GetBookById(BookViewModel model);

        BookViewModel CreateBook(BookViewModel model);

        BookViewModel UpdateBook(BookViewModel model);

        BookViewModel DeleteBookAll(BookViewModel model);

        BookViewModel DeleteBook(long Id);

        List<BookViewModel> GetBook(string SearchText);

        BookViewModel CheckBookCodeExist(BookDetailsViewModel model);

        BookViewModel FillCourse(BookViewModel model);

        BookViewModel FillUniversity(BookViewModel model);


    }
}
