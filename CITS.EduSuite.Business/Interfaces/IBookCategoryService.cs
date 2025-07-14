using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IBookCategoryService
    {
        List<BookCategoryViewModel> GetBookCategory(string searchText);

        BookCategoryViewModel GetBookCategoryById(short? id);

        BookCategoryViewModel CreateBookCategory(BookCategoryViewModel model);

        BookCategoryViewModel UpdateBookCategory(BookCategoryViewModel model);

        BookCategoryViewModel DeleteBookCategory(BookCategoryViewModel model);

    }
}
