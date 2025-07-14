using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;


namespace CITS.EduSuite.Business.Interfaces
{
    public interface ITransactionTypeService
    {
        List<TransactionTypeViewModel> GetTransactionType(string searchText);

        TransactionTypeViewModel GetTransactionTypeById(byte? id);

        TransactionTypeViewModel CreateTransactionType(TransactionTypeViewModel model);

        TransactionTypeViewModel UpdateTransactionType(TransactionTypeViewModel model);

        TransactionTypeViewModel DeleteTransactionType(TransactionTypeViewModel model);
    }

}

