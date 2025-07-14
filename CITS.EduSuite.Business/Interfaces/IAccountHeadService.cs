using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IAccountHeadService
    {
        AccountHeadViewModel GetAccountHeadById(int? id, short? type, byte? group);
        AccountHeadViewModel CreateAccountHead(AccountHeadViewModel model);
        AccountHeadViewModel UpdateAccountHead(AccountHeadViewModel model);
        AccountHeadViewModel DeleteAccountHead(AccountHeadViewModel model);
        List<AccountHeadViewModel> GetAccountHead(string searchText, AccountHeadViewModel model);
        AccountHeadViewModel FillAccountHeadType(AccountHeadViewModel model);
        AccountHeadViewModel createAccountChart(AccountHeadViewModel model);
        AccountHeadViewModel updateAccountChart(AccountHeadViewModel model);
        void FillAccountGroup(AccountHeadViewModel model);
        AccountHeadViewModel FillSearchAccountHeadType(AccountHeadViewModel model);
    }
}
