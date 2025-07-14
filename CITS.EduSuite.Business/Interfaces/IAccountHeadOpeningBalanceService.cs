using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IAccountHeadOpeningBalanceService
    {
        AccountHeadOpeningBalanceViewModel GetAccountHeadOpeningBalanceById(short? id);
        AccountHeadOpeningBalanceViewModel CreateAccountHeadOpeningBalance(AccountHeadOpeningBalanceViewModel model);
        AccountHeadOpeningBalanceViewModel DeleteAccountHeadOpeningBalance(AccountHeadOpeningBalanceViewModel model);
        List<AccountHeadOpeningBalanceViewModel> GetAccountHeadOpeningBalance(short BranchKey);
        AccountHeadOpeningBalanceViewModel FillBranches(AccountHeadOpeningBalanceViewModel model);
    }
}
