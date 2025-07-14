using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IAuthorService
    {
        List<AuthorViewModel> GetAuthors(AuthorViewModel model, out long TotalRecords);
        AuthorViewModel GetAuthorById(Int32? id);
        AuthorViewModel CreateAuthor(AuthorViewModel model);
        AuthorViewModel UpdateAuthor(AuthorViewModel model);
        AuthorViewModel DeleteAuthor(AuthorViewModel model);



    }
}
